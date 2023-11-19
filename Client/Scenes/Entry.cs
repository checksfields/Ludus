using System;
using System.Collections.Generic;
using Bitspoke.Core.Common.Vector;
using Bitspoke.Core.Components.Time;
using Bitspoke.Core.Definitions.TypeDatas.Time;
using Bitspoke.Core.Signal;
using Bitspoke.Core.Signal.UI;
using Bitspoke.Core.Systems.Time;
using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Components.Camera;
using Bitspoke.GodotEngine.Components.Camera._2D;
using Bitspoke.GodotEngine.Components.Nodes;
using Bitspoke.GodotEngine.Components.Nodes._2D;
using Bitspoke.GodotEngine.Components.Nodes.CanvasLayers;
using Bitspoke.GodotEngine.Components.Performance;
using Bitspoke.GodotEngine.Controllers.Inputs;
using Bitspoke.GodotEngine.Controllers.Resources;
using Bitspoke.Ludus.Client.Components;
using Bitspoke.Ludus.Client.Components.Nodes.Maps.Terrains;
using Bitspoke.Ludus.Client.Components.Regions.Containers;
using Bitspoke.Ludus.Client.Components.UI.Information;
using Bitspoke.Ludus.Shared.Common.Controllers.Admin;
using Bitspoke.Ludus.Shared.Common.TypeDatas.Game.States;
using Bitspoke.Ludus.Shared.Environment.Map;
using Bitspoke.Ludus.Shared.Environment.World;
using Bitspoke.Ludus.Shared.Tests.Maps;
using Godot;
using Console = Bitspoke.GodotEngine.Components.Console.Console;


namespace Bitspoke.Ludus.Client.Scenes;

public partial class Entry : GodotNode2D, ITickConsumer
{
	#region Properties

	private int TotalInstances { get; set; }
	
	public LudusCamera2D LudusCamera2D { get; set; }
	private InputController InputController = null;
	public TickComponent TickComponent { get; }

	public Console Console { get; set; }
	public PerformanceComponent PerformanceComponent { get; set; }
	public GameStateInformationComponent GameStateInformationComponent { get; set; }
	
	public ToolTipCanvasLayer ToolTipLayer { get; set; }
	
//	public Node2D RegionsContainer { get; set; }
//	public Node2D PlantRegionsContainer { get; set; }
//	public Dictionary<int, RegionNode_Old> RegionNodes { get; set; }
	
//	public Dictionary<int, TerrainRegionNodeOld> TerrainRegionNodes { get; set; }
	public TerrainLayer TerrainLayer { get; set; }
	
	public RegionsContainer RegionsContainer { get; set; }

	
	private Map Map { get; set; }
	
	public float TotalFPS { get; set; } = 0;
	public int TotalProcessCalls { get; set; }
	
	public override string NodeName => GetType().Name;
	public override Node Node => this;
	
	#endregion
	
	public override void Init()
	{
		Log.Info();
	}

	public override void AddComponents()
	{
		//this.AddComponent(SettingsComponent = new LudusGameSettingsComponent());
		this.AddGodotNode(PerformanceComponent = new PerformanceComponent());
		this.AddGodotNode(GameStateInformationComponent = new GameStateInformationComponent());
		this.AddGodotNode(ToolTipLayer = new ToolTipCanvasLayer());
	}

	public override void ConnectSignals()
	{
		SignalManager.Connect(new SignalDetails(UISignals.NOTIFICATION_UPDATE_REQUEST, typeof(UISignals), this, nameof(OnNotificationUpdate)));
		SignalManager.Connect(new SignalDetails(UISignals.NOTIFICATION_CLOSE_REQUEST, typeof(UISignals), this, nameof(OnNotificationClose)));
		SignalManager.Connect(new SignalDetails(ResourceController.RESOURCES_LOADED, typeof(ResourceController), this, nameof(OnResourcesLoaded)));
		SignalManager.Connect(new SignalDetails(AdminCommandControllerSignals.SET_PLANTS_VISIBLE, typeof(AdminCommandControllerSignals), this, nameof(OnShowPlants)));
		SignalManager.Connect(new SignalDetails(AdminCommandControllerSignals.SET_TERRAIN_LAYER_VISIBLE, typeof(AdminCommandControllerSignals), this, nameof(OnShowTerrainLayer)));
		SignalManager.Connect(new SignalDetails(CameraZoomComponent.ZOOM_CHANGE, typeof(CameraZoomComponent), this, nameof(OnZoomChanged)));
		
		TimeSystem.RegisterForTick(TickTypeData.LONG_TICK_KEY, this);
		
		GodotGlobal.Actions.UIMouseOverEnter += OnUIMouseOverEnter;
		GodotGlobal.Actions.UIMouseOverExit += OnUIMouseOverExit;
	}
	
	
	
	private void OnUIMouseOverEnter(GodotNode2D toShow)
	{
		ToolTipLayer.RemoveAllChildren();
		ToolTipLayer.AddChild(toShow);
	}
	
	private void OnUIMouseOverExit()
	{
		if (IsInstanceValid(ToolTipLayer))
		{
			ToolTipLayer.RemoveAllChildren();	
		}
	}

	public ulong ElapsedTime { get; set; } = 0;
	
	public override void _EnterTree()
	{
		ElapsedTime = 0u;
		base._EnterTree();
		
		BuildWorld();
		
		AddCamera2D();
		
		this.AddGodotNode(RegionsContainer = new RegionsContainer(Map));		
	}

	private void AddCamera2D()
	{
		AddChild(LudusCamera2D = new LudusCamera2D(
			new GodotNodeCollection()
			{
				new Camera2DZoomComponent(),
				new Camera2DDragComponent()
			}));
		
		LudusCamera2D.MakeCurrent();
		LudusCamera2D.AnchorMode = Camera2D.AnchorModeEnum.FixedTopLeft;
	}

	#region Draw

	// public override void _Draw()
 //    	{
 //    		base._Draw();
 //    		if (CoreGlobal.DEBUG_DRAW_ENABLED)
 //    		{
 //    			foreach (var child in GetChildren())
 //    			{
 //    				if (child is IGodotComponentCollection)
 //    					DrawNode2Ds((IGodotComponentCollection) child);
 //    			}
 //    		}
 //    	}
 //    
 //    	private void DrawNode2Ds(IGodotComponentCollection nodeCollection)
 //    	{
 //    		foreach (var node in nodeCollection.Components?.Values)
 //    		{
 //    			if (node is IGodotComponentCollection)
 //    			{
 //    				DrawNode2Ds((IGodotComponentCollection)node);
 //    			}
 //    			
 //    			DrawNode2D(node);
 //    		}
 //    	}
 //    	
 //    	private void DrawNode2D(Node node)
 //    	{
 //    		if (node is Control)
 //    		{
 //    			var pos = ((Control)node).Position;
 //    			var size = ((Control)node).Size;
 //    			var rect2 = new Rect2(pos, size);
 //    			DrawRect(rect2, Colors.Magenta, false, width:1f);
 //    		}
 //    		
 //    		if (node is Node2D)
 //    		{
 //    			var pos = ((Node2D)node).Position;
 //    			var rect2 = new Rect2(pos, new Vector2(50f, 50f));
 //    			DrawRect(rect2, Colors.Magenta, false, width:1f);		
 //    		}
 //    	}


	#endregion
	
	public override void _Ready()
	{
		base._Ready();
		
		CoreFind.Managers.GameStateManager.SetState(LudusGameStatesTypeData.MAIN_KEY);
		
		Profile(() => { AddChild(TerrainLayer = new TerrainLayer(Map)); });
	}
	
	public override void _Process(double delta)
	{
		if (ElapsedTime == 0)
		{
			ElapsedTime = Time.GetTicksMsec();
			Log.Debug($"Total Elapsed Time since _EnterTree = {ElapsedTime} ms");
		}
		
		if (CoreGlobal.DEBUG_DRAW_ENABLED)
			QueueRedraw();
		
		SignalManager.Emit(TimeSystem.UPDATE, delta);
	}

	protected void OnResourcesLoaded() { }
	protected void OnNotificationUpdate(string message) { Log.Info($"!!!!!!!!!!! {message}"); }
	protected void OnNotificationClose(string message) { Log.Info(message); }
	
	private void BuildWorld(bool debug = false)
	{
		Profile(() => { 
		
			// TEST: World3D Creation and Initialisation
			// TEST: World3D Generation
			var worldInitConfig = new WorldInitConfig
			{
				SeedPart = 1987654320,
				Dimensions = new Vec2Int(100, 100),
			};
			WorldManager.GenerateWorld(worldInitConfig);
			
			// Generate the map
			var mapInitConfig = new MapInitConfig
			{
				SeedPart = 1234567890,
				BiomeKey = "BiomeA", 
				WorldID = Find.CurrentWorld.WorldID, 
				Size = new Vector2I(275, 275),
				ElevationTypeDataKey = Find.DB.TypeData.ElevationTypeData["HILLS"].Key,
				VegetationDensityTypeDataKey = Find.DB.TypeData.VegetationDensityTypeData["MED"].Key,
				AvailableRockDefKeys  = new() { "sandstone", "slate", "marble" }
			};

			Map = MapManager.GenerateMap(mapInitConfig);
			Find.CurrentWorld.Maps.Current = Map;
			
		});
	}

	private void DebugPrintMap()
	{
		Profile(Find.CurrentMap.DebugPrintAscii);
	}
	
//	private void AddPlants(Map map)
//	{
//		Profile(() => { 
//		
//		
//		for (var regionIndex = 0; regionIndex < map.Data.RegionsContainer.Regions.Length; regionIndex++)
//		{
//			var regionNode = new PlantRegionNodeOld(map.Data.RegionsContainer.Regions[regionIndex]);
//			
//			RegionNodes[regionIndex] = regionNode;
//			PlantRegionsContainer.AddChild(regionNode);
//		}
//		
//		});
//	}

	private ImageTexture TerrainTextureAtlas { get; set; }
	ImageTexture? TileBlendTexture { get; set; }
	private Shader Shader { get; set; }
	
	private void OnZoomChanged()
	{
		//var zoom = GetViewport().GetCamera2D().Zoom;
		//RenderTerrain(CalculateRenderRect());
	}

	private Rect2 CalculateRenderRect()
	{
		var ctrans = GetCanvasTransform();
		var min_pos = (-ctrans.Origin / ctrans.Scale / 64f);
		var view_size = (GetViewportRect().Size / ctrans.Scale / 64f * 1.5f);
		if (view_size.X > Map.Width)
			view_size = Map.Size;

		return new Rect2(min_pos, view_size.Ceil());
	}

	private void OnShowPlants(bool visible, string? plantDefKey = null)
	{
		throw new NotImplementedException();
		// Start(additionalKey:"OnShowPlants");
		// if (visible)
		// {
		// 	// AddPlants(Map);
		// }
		// else
		// {
		// 	// foreach (var (key, value) in RegionNodes)
		// 	// {
		// 	// 	var regionNode = value;
		// 	// 	
		// 	// 	if (regionNode is PlantRegionNode)
		// 	// 		regionNode.QueueFree();
		// 	//
		// 	// 	RegionNodes[key] = null;
		// 	// }
		// }
		//
		// End(additionalKey:"OnShowPlants",message:$"SetTerrainLayerVisible: {visible}");
	}
	
	private void OnShowTerrainLayer(bool visible, string? terrainDefKey = null)
	{
		throw new NotImplementedException();
		// Start();
		// if (visible)
		// 	TestRenderTerrainTypeOverlay(Map, terrainDefKey);
		// else
		// {
		// 	foreach (var regionNode in DebugRegionNodes.Values)
		// 	{
		// 		PlantRegionsContainer.RemoveChild(regionNode);
		// 		regionNode.QueueFree();
		// 	}
		// 	DebugRegionNodes.Clear();
		// }
		//
		// End(message:$"SetTerrainLayerVisible: {visible}");
	}
	
	public void OnTick()
	{
		return;
		
//		PlantRegionsContainer.Hide();
		Log.Debug("TICK!");
	
	}
}