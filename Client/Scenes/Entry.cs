using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bitspoke.Core.Common.Vector;
using Bitspoke.Core.Components;
using Bitspoke.Core.Profile;
using Bitspoke.Core.Signal;
using Bitspoke.Core.Signal.UI;
using Bitspoke.Core.Systems.Time;
using Bitspoke.Core.Types.Game.States;
using Bitspoke.GodotEngine.Components.Camera;
using Bitspoke.GodotEngine.Components.Camera._2D;
using Bitspoke.GodotEngine.Components.Nodes;
using Bitspoke.GodotEngine.Components.Performance;
using Bitspoke.GodotEngine.Components.Settings;
using Bitspoke.GodotEngine.Controllers.Inputs;
using Bitspoke.GodotEngine.Controllers.Resources;
using Bitspoke.GodotEngine.Utils.Files;
using Bitspoke.GodotEngine.Utils.Images;
using Bitspoke.GodotEngine.Utils.Vector;
using Bitspoke.Ludus.Shared.Common.Controllers.Admin;
using Bitspoke.Ludus.Shared.Common.TypeDatas.Game.States;
using Bitspoke.Ludus.Shared.Environment.Map;
using Bitspoke.Ludus.Shared.Environment.World;
using Bitspoke.Ludus.Shared.Tests.Maps;
using Client.Components;
using Client.Components.Node.Shaders;
using Client.Components.Regions;
using Client.Components.Regions.Debug;
using Godot;
using Newtonsoft.Json;
using Console = Bitspoke.GodotEngine.Components.Console.Console;


public partial class Entry : GodotNode2D
{
	public LudusCamera2D LudusCamera2D { get; set; }
	private int TotalInstances { get; set; }

	private InputController InputController = null;

	public Console Console { get; set; }
	public Node2D LayerContainer { get; set; }
	public Dictionary<int, RegionNode> RegionNodes { get; set; }
	public Dictionary<int, TerrainRegionNode> TerrainRegionNodes { get; set; }

	public SettingsComponent SettingsComponent { get; set; }
	
	protected override void Init()
	{
		Log.Info();
	}

	protected override void AddComponents()
	{
		Log.Info();
		// AddChild(new NetworkComponent());
		// AddChild(InputController = new InputController(true));
		AddChild(new PerformanceComponent());
		AddChild(SettingsComponent = new SettingsComponent());
	}

	protected override void ConnectSignals()
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
		
		Log.Info("Adding Camera");
		AddChild(LudusCamera2D = new LudusCamera2D(
			new ComponentCollection()
			{
				new Camera2DZoomComponent(),
				new Camera2DDragComponent()
			}));
		
		LudusCamera2D.MakeCurrent();
		LudusCamera2D.AnchorMode = Camera2D.AnchorModeEnum.FixedTopLeft;
		
		Log.Info("Adding Layer Container");
		LayerContainer = new Node2D();
		LayerContainer.Name = "layer_container";
		AddChild(LayerContainer);

		Log.Info("Initialising Collections");
		RegionNodes = new Dictionary<int, RegionNode>();
		TerrainRegionNodes = new Dictionary<int, TerrainRegionNode>();
		
	}

	public override void _Ready()
	{
		base._Ready();
		
		
		
		//Find.DB.TypeData.All["GameStatesTypeData"];
		Profiler.Start();
		// TODO: RemoveAt Test
		
		TestBuildWorld();
		//DebugPrintMap();
		TestRenderPlants(Map);
		TestRenderTerrain(Map);
		RenderTerrain(new Rect2(new Vector2(0, 0), Map.Size));

		Profiler.End();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (Input.IsKeyPressed(Key.Escape))
			SettingsComponent.ShowPopup();
	}

	public float TotalFPS { get; set; } = 0;
	public int TotalProcessCalls { get; set; }
	
	public override void _Process(double delta)
	{
		if (ElapsedTime == 0)
		{
			ElapsedTime = Time.GetTicksMsec();
			Log.Debug($"Total Elapsed Time since _EnterTree = {ElapsedTime} ms");
		}
		
		SignalManager.Emit(TimeSystem.UPDATE, delta);
	}

	protected void OnResourcesLoaded() { }
	protected void OnNotificationUpdate(string message) { Log.Info(message); }
	protected void OnNotificationClose(string message) { Log.Info(message); }

	private Map Map { get; set; }
	private void TestBuildWorld(bool debug = false)
	{
		Profiler.Start();
		
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
		var map = MapManager.GenerateMap(mapInitConfig);
		Find.CurrentWorld.Maps.Current = map;
		
		// Profiler.Start(additionalKey:"SaveWorld");
		// WorldManager.SaveWorld($"map{map.MapID}_savegame");
		// Profiler.End(message:"Save Game", additionalKey:"SaveWorld"); 

		if (debug)
		{
			var tasks = new Task[]
			{
				Task.Run(() => TestSaveLayer($"map{map.MapID}_elevation", map.Cells.Elevations)),
				Task.Run(() => TestSaveLayer($"map{map.MapID}_fertility", map.Cells.Fertilities)),
				Task.Run(() => TestSaveLayer($"map{map.MapID}_terrain", map.Cells.TerrainDefKeys)),
				Task.Run(() => TestSaveLayer($"map{map.MapID}_structure", map.Cells.StructureDefKeys)),
				Task.Run(() => TestSaveLayer($"map{map.MapID}_roof", map.Cells.RoofDefKeys)),
			};
			Task.WaitAll(tasks);
		}

		Map = map;
		
		Profiler.End();
	}

	private void DebugPrintMap()
	{
		Profiler.Start();
		Find.CurrentMap.DebugPrintAscii();
		Profiler.End();
	}
	
	private void TestSaveLayer(string fileName, object toSave)
	{
		GodotFileUtils.WriteToFile($"{GodotGlobal.SAVE_ROOT_PATH}/{fileName}{GodotGlobal.SUPPORTED_SAVE_TYPE}", JsonConvert.SerializeObject(toSave, Formatting.Indented));
	}

	private void TestRenderPlants(Map map)
	{
		Profiler.Start();
		
		// for (var regionIndex = 0; regionIndex < map.Data.RegionsContainer.Regions.Length; regionIndex++)
		// {
		// 	var regionNode = new PlantRegionNode(map.Data.RegionsContainer.Regions[regionIndex]);
		// 	
		// 	RegionNodes[regionIndex] = regionNode;
		// 	LayerContainer.AddChild(regionNode);
		// }


		for (int x = 0; x < 3; x++)
		{
			for (int y = 0; y < 3; y++)
			{
				var regionIndex = x + y * map.Data.RegionsContainer.Width;
				var regionNode = new PlantRegionNode(map.Data.RegionsContainer.Regions[regionIndex]);
			
				RegionNodes[regionIndex] = regionNode;
				LayerContainer.AddChild(regionNode);
			}
			
		}
		
		
		// for (var regionIndex = 0; regionIndex < 1; regionIndex++)
		// {
		// 	var regionNode = new PlantRegionNode(map.Data.RegionsContainer.Regions[regionIndex]);
		// 	
		// 	RegionNodes[regionIndex] = regionNode;
		// 	LayerContainer.AddChild(regionNode);
		// }
		
		
		
		// foreach (var mapRegion in map.Regions.MapRegions.Values)
		// {
		// 	var regionNode = new PlantRegionNode(mapRegion);
		//
		// 	RegionNodes[mapRegion.Index] = regionNode;
		// 	
		// 	LayerContainer.AddChild(regionNode);
		// }
		
		Profiler.End();
	}
	
	public const String TERRAIN_ATLAS = "Terrain/Atlas";
	public const String BLEND_TEXTURE = "Support/tileblend";
	public const String SHADER_KEY = "Layers/terrain";
	
	private void TestRenderTerrain(Map map)
	{
		TileBlendTexture = ImageUtils.CreateTileBlendTexture();
		Shader = Find.DB.ShaderDB[SHADER_KEY];
	}

	private ImageTexture TerrainTextureAtlas { get; set; }
	ImageTexture? TileBlendTexture { get; set; }
	private Shader Shader { get; set; }

	private void RenderTerrain(Rect2 regionToRender)
	{
		//Profiler.Start();

		TerrainTextureAtlas = (ImageTexture)Find.DB.TextureDB[TERRAIN_ATLAS];

		//var renderSize = Map.Size;
		//var renderSize = new Vec2Int(125, 125);
		//var renderSize = GetViewport().Size.ToVec2Int() / 64f;
		var name = "terrainRender";
		foreach (Node child in GetChildren())
		{
			if (child.Name == name)
				child.Free();
		}

		// var zoom = GetViewport().GetCamera2D().Zoom;
		// var viewportRect = GetViewportRect();
		// var viewportPosition = viewportRect.Position * -1.1f;
		// var viewportSize = ((viewportRect.Size / GodotGlobal.STANDARD_CELL_SIZE)*1.5f*zoom).Ceil();
		// viewportRect = new Rect2(viewportPosition, viewportSize);
		
		var mapData = Map.GenerateTerrainDefsTexture(CalculateRenderRect());
		
		var terrainRender = new Sprite2D();
		terrainRender.Name = name;
		AddChild(terrainRender);
		terrainRender.Texture = Find.DB.TextureDB["default"];
		terrainRender.Centered = false;
		var shaderMaterial2 = new TerrainShaderMaterial(Shader, TerrainTextureAtlas, TileBlendTexture, regionToRender.Size.ToVec2Int());
		terrainRender.Material = shaderMaterial2;
		shaderMaterial2.SetShaderParameter(TerrainShaderMaterial.SHADER_PARAM_MAP_DATA, mapData);
		shaderMaterial2.SetShaderParameter(TerrainShaderMaterial.SHADER_PARAM_BLEND_FLAG, true);
		terrainRender.Scale = regionToRender.Size;
		terrainRender.ZIndex = -1;
		terrainRender.ZAsRelative = false;
		
		//Profiler.End();
		//RenderRegionTerrain();
	}
	
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

		return new Rect2(min_pos, view_size);
	}

	private void OnShowPlants(bool visible, string? plantDefKey = null)
	{
		// TODO ... plant specific
		Log.TODO("implement");
		
		Profiler.Start(additionalKey:"OnShowPlants");
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

		Profiler.End(additionalKey:"OnShowPlants",message:$"SetTerrainLayerVisible: {visible}");
	}
	
	private void OnShowTerrainLayer(bool visible, string? terrainDefKey = null)
	{
		Profiler.Start();
		if (visible)
			TestRenderTerrainTypeOverlay(Map, terrainDefKey);
		else
		{
			foreach (var regionNode in DebugRegionNodes.Values)
			{
				LayerContainer.RemoveChild(regionNode);
				regionNode.QueueFree();
			}
			DebugRegionNodes.Clear();
		}

		Profiler.End(message:$"SetTerrainLayerVisible: {visible}");
	}
	
	public Dictionary<int, RegionNode> DebugRegionNodes { get; set; } = new ();
	private void TestRenderTerrainTypeOverlay(Map map, string terrainTypeKey = null)
	{
		Profiler.Start();
		foreach (var mapRegion in map.Regions.MapRegions.Values)
		{
			var regionNode = new DebugRegionNode(mapRegion);
			regionNode.ProcessTerrain(terrainTypeKey);
			
			DebugRegionNodes.Add(mapRegion.Index, regionNode);
			LayerContainer.AddChild(regionNode);
		}
		Profiler.End();


		// var timer = new Timer();
		// AddChild(timer);
		// timer.Start(1);
		// timer.Connect("timeout",new Callable(this,"OnTimerTimeout"));

	}
}