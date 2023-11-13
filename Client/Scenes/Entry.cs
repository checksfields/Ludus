using System;
using System.Collections.Generic;
using Bitspoke.Core.Common.Vector;
using Bitspoke.Core.Components;
using Bitspoke.Core.Components.Time;
using Bitspoke.Core.Definitions.TypeDatas.Time;
using Bitspoke.Core.Signal;
using Bitspoke.Core.Signal.UI;
using Bitspoke.Core.Systems.Time;
using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Components.Camera;
using Bitspoke.GodotEngine.Components.Camera._2D;
using Bitspoke.GodotEngine.Components.Nodes;
using Bitspoke.GodotEngine.Components.Performance;
using Bitspoke.GodotEngine.Controllers.Inputs;
using Bitspoke.GodotEngine.Controllers.Resources;
using Bitspoke.GodotEngine.Utils.Files;
using Bitspoke.Ludus.Client.Components;
using Bitspoke.Ludus.Client.Components.Nodes.Maps.Terrains;
using Bitspoke.Ludus.Client.Components.Regions;
using Bitspoke.Ludus.Client.Components.Regions.Debug;
using Bitspoke.Ludus.Client.Components.UI.Information;
using Bitspoke.Ludus.Shared.Common.Controllers.Admin;
using Bitspoke.Ludus.Shared.Common.TypeDatas.Game.States;
using Bitspoke.Ludus.Shared.Environment.Map;
using Bitspoke.Ludus.Shared.Environment.World;
using Bitspoke.Ludus.Shared.Tests.Maps;
using Godot;
using Newtonsoft.Json;
using Console = Bitspoke.GodotEngine.Components.Console.Console;


namespace Bitspoke.Ludus.Client.Scenes;

public partial class Entry : GodotNode2D, ITickConsumer
{
	#region Properties

	public LudusCamera2D LudusCamera2D { get; set; }
	private int TotalInstances { get; set; }

	private InputController InputController = null;

	public Console Console { get; set; }
	public Node2D PlantRegionsContainer { get; set; }
	public Dictionary<int, RegionNode> RegionNodes { get; set; }
	public Dictionary<int, TerrainRegionNode> TerrainRegionNodes { get; set; }
	public TerrainLayer TerrainLayer { get; set; }


	public PerformanceComponent PerformanceComponent { get; set; }
	public GameStateInformationComponent GameStateInformationComponent { get; set; }
	public TickComponent TickComponent { get; }
	
	private Map Map { get; set; }
	
	public float TotalFPS { get; set; } = 0;
	public int TotalProcessCalls { get; set; }
	
	public override string Name => GetType().Name;	
	
	#endregion
	
	public override void Init()
	{
		Log.Info();
	}

	public override void AddComponents()
	{
		//this.AddComponent(SettingsComponent = new LudusGameSettingsComponent());
		this.AddComponent(PerformanceComponent = new PerformanceComponent());
		this.AddComponent(GameStateInformationComponent = new GameStateInformationComponent());
		
		TimeSystem.RegisterForTick(TickTypeData.LONG_TICK_KEY, this);
	}

	public override void ConnectSignals()
	{
		SignalManager.Connect(new SignalDetails(UISignals.NOTIFICATION_UPDATE_REQUEST, typeof(UISignals), this, nameof(OnNotificationUpdate)));
		SignalManager.Connect(new SignalDetails(UISignals.NOTIFICATION_CLOSE_REQUEST, typeof(UISignals), this, nameof(OnNotificationClose)));
		SignalManager.Connect(new SignalDetails(ResourceController.RESOURCES_LOADED, typeof(ResourceController), this, nameof(OnResourcesLoaded)));
		SignalManager.Connect(new SignalDetails(AdminCommandControllerSignals.SET_PLANTS_VISIBLE, typeof(AdminCommandControllerSignals), this, nameof(OnShowPlants)));
		SignalManager.Connect(new SignalDetails(AdminCommandControllerSignals.SET_TERRAIN_LAYER_VISIBLE, typeof(AdminCommandControllerSignals), this, nameof(OnShowTerrainLayer)));
		SignalManager.Connect(new SignalDetails(CameraZoomComponent.ZOOM_CHANGE, typeof(CameraZoomComponent), this, nameof(OnZoomChanged)));
	}

	public ulong ElapsedTime { get; set; } = 0;
	
	public override void _EnterTree()
	{
		ElapsedTime = 0u;
		base._EnterTree();
		
		AddCamera2D();
		
		PlantRegionsContainer = new Node2D();
		PlantRegionsContainer.Name = "PlantRegionsContainer";
		AddChild(PlantRegionsContainer);

		Log.Info("Initialising Collections");
		RegionNodes = new Dictionary<int, RegionNode>();
		TerrainRegionNodes = new Dictionary<int, TerrainRegionNode>();
		
	}

	private void AddCamera2D()
	{
		AddChild(LudusCamera2D = new LudusCamera2D(
			new ComponentCollection()
			{
				new Camera2DZoomComponent(),
				new Camera2DDragComponent()
			}));
		
		LudusCamera2D.MakeCurrent();
		LudusCamera2D.AnchorMode = Camera2D.AnchorModeEnum.FixedTopLeft;
	}

	#region Draw

	public override void _Draw()
    	{
    		base._Draw();
    		if (CoreGlobal.DEBUG_DRAW_ENABLED)
    		{
    			foreach (var child in GetChildren())
    			{
    				if (child is IGodotComponentCollection)
    					DrawNode2Ds((IGodotComponentCollection) child);
    			}
    		}
    	}
    
    	private void DrawNode2Ds(IGodotComponentCollection nodeCollection)
    	{
    		foreach (var node in nodeCollection.Components?.Values)
    		{
    			if (node is IGodotComponentCollection)
    			{
    				DrawNode2Ds((IGodotComponentCollection)node);
    			}
    			
    			DrawNode2D(node);
    		}
    	}
    	
    	private void DrawNode2D(Node node)
    	{
    		if (node is Control)
    		{
    			var pos = ((Control)node).Position;
    			var size = ((Control)node).Size;
    			var rect2 = new Rect2(pos, size);
    			DrawRect(rect2, Colors.Magenta, false, width:1f);
    		}
    		
    		if (node is Node2D)
    		{
    			var pos = ((Node2D)node).Position;
    			var rect2 = new Rect2(pos, new Vector2(50f, 50f));
    			DrawRect(rect2, Colors.Magenta, false, width:1f);		
    		}
    	}


	#endregion
	
	public override void _Ready()
	{
		base._Ready();
	
		CoreFind.Managers.GameStateManager.SetState(LudusGameStatesTypeData.MAIN_KEY);
		
		//return;

		Profile(() => {
		
		TestBuildWorld();
		TestRenderPlants(Map);
		
		AddChild(TerrainLayer = new TerrainLayer(Map));
		TerrainLayer.Render();
		
		});
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
	
	private void TestBuildWorld(bool debug = false)
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
	
	private void TestSaveLayer(string fileName, object toSave)
	{
		GodotFileUtils.WriteToFile($"{GodotGlobal.SAVE_ROOT_PATH}/{fileName}{GodotGlobal.SUPPORTED_SAVE_TYPE}", JsonConvert.SerializeObject(toSave, Formatting.Indented));
	}

	private void TestRenderPlants(Map map)
	{
		Profile(() => { 
		
		
		for (var regionIndex = 0; regionIndex < map.Data.RegionsContainer.Regions.Length; regionIndex++)
		{
			var regionNode = new PlantRegionNode(map.Data.RegionsContainer.Regions[regionIndex]);
			
			RegionNodes[regionIndex] = regionNode;
			PlantRegionsContainer.AddChild(regionNode);
		}

		// var renderRect = CalculateRenderRect();
		//
		// var startRegionX = (renderRect.Position.X / SharedGlobal.DEFAULT_MAP_REGION_DIMENSIONS.X).Floor();
		// var startRegionY = (renderRect.Position.Y / SharedGlobal.DEFAULT_MAP_REGION_DIMENSIONS.Y).Floor();
		// var startRegionPos = new Vector2I(startRegionX, startRegionY);
		// var endRegionX = (renderRect.End.X / SharedGlobal.DEFAULT_MAP_REGION_DIMENSIONS.X).Ceiling();
		// var endRegionY = (renderRect.End.Y / SharedGlobal.DEFAULT_MAP_REGION_DIMENSIONS.Y).Ceiling();
		// var endRegionPos = new Vector2I(endRegionX, endRegionY);
		//
		//
		// for (int x = startRegionPos.X; x <= endRegionPos.X; x++)
		// {
		// 	for (int y = startRegionPos.Y; y <= endRegionPos.Y; y++)
		// 	{
		// 		var regionIndex = x + y * map.Data.RegionsContainer.Width;
		// 		var regionNode = new PlantRegionNode(map.Data.RegionsContainer.Regions[regionIndex]);
		// 	
		// 		RegionNodes[regionIndex] = regionNode;
		// 		LayerContainer.AddChild(regionNode);
		// 	}
		// }
		
		// for (int x = 0; x < 11; x++)
		// {
		// 	for (int y = 0; y < 11; y++)
		// 	{
		// 		var regionIndex = x + y * map.Data.RegionsContainer.Width;
		// 		var r = new List<int> { 0, 1, 2, 11, 12, 13, 22, 23, 25 };
		// 		if (r.Contains(regionIndex))
		// 			continue;
		// 		
		// 		var regionNode = RegionNodes[regionIndex];
		// 		regionNode.Sprites?.Hide();
		// 	}
		// }
		
		});
	}
	
	public const String TERRAIN_ATLAS = "Terrain/Atlas";
	public const String BLEND_TEXTURE = "Support/tileblend";
	public const String SHADER_KEY = "Layers/terrain";
	
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
		// TODO ... plant specific
		Log.TODO("implement");
		
		Start(additionalKey:"OnShowPlants");
		if (visible)
			TestRenderPlants(Map);
		else
		{
			foreach (var (key, value) in RegionNodes)
			{
				var regionNode = value;
				
				if (regionNode is PlantRegionNode)
					regionNode.QueueFree();

				RegionNodes[key] = null;
			}
		}

		End(additionalKey:"OnShowPlants",message:$"SetTerrainLayerVisible: {visible}");
	}
	
	private void OnShowTerrainLayer(bool visible, string? terrainDefKey = null)
	{
		Start();
		if (visible)
			TestRenderTerrainTypeOverlay(Map, terrainDefKey);
		else
		{
			foreach (var regionNode in DebugRegionNodes.Values)
			{
				PlantRegionsContainer.RemoveChild(regionNode);
				regionNode.QueueFree();
			}
			DebugRegionNodes.Clear();
		}

		End(message:$"SetTerrainLayerVisible: {visible}");
	}
	
	public Dictionary<int, RegionNode> DebugRegionNodes { get; set; } = new ();
	private void TestRenderTerrainTypeOverlay(Map map, string terrainTypeKey = null)
	{
		Start();
		foreach (var mapRegion in map.Regions.MapRegions.Values)
		{
			var regionNode = new DebugRegionNode(mapRegion);
			regionNode.ProcessTerrain(terrainTypeKey);
			
			DebugRegionNodes.Add(mapRegion.Index, regionNode);
			PlantRegionsContainer.AddChild(regionNode);
		}
		End();


		// var timer = new Timer();
		// AddChild(timer);
		// timer.Start(1);
		// timer.Connect("timeout",new Callable(this,"OnTimerTimeout"));

	}

	public void OnTick()
	{
		return;
		
		PlantRegionsContainer.Hide();
		Log.Debug("TICK!");
	
	}
}