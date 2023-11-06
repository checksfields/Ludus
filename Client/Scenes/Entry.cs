using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bitspoke.Core.Common.Logging;
using Bitspoke.Core.Common.Vector;
using Bitspoke.Core.Components;
using Bitspoke.Core.Profile;
using Bitspoke.Core.Signal;
using Bitspoke.Core.Signal.UI;
using Bitspoke.Core.Systems.Time;
using Bitspoke.GodotEngine.Common.Vector;
using Bitspoke.GodotEngine.Components.Settings;
using Bitspoke.GodotEngine.Controllers.Inputs;
using Bitspoke.GodotEngine.Controllers.Resources;
using Bitspoke.GodotEngine.Utils.Files;
using Bitspoke.GodotEngine.Utils.Images;
using Bitspoke.GodotEngine.Utils.Vector;
using Bitspoke.Ludus.Shared.Common.Controllers.Admin;
using Bitspoke.Ludus.Shared.Environment.Map;
using Bitspoke.Ludus.Shared.Environment.World;
using Client.Components.Node.Shaders;
using Client.Components.Regions;
using Client.Components.Regions.Debug;
using Godot;
using Newtonsoft.Json;
using Bitspoke.GodotEngine.Components.Camera._2D;
using Bitspoke.GodotEngine.Components.Nodes;
using Bitspoke.GodotEngine.Components.Performance;
using Bitspoke.Ludus.Shared;
using Bitspoke.Ludus.Shared.Tests.Maps;
using Client.Components;
using Client.Managers;
using Console = Bitspoke.GodotEngine.Components.Console.Console;
using CoreFind = Bitspoke.Core.Find;


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

	public override void Init()
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
		
		//SignalManager.Connect(new SignalDetails(BitspokeCamera2D.ZOOM_CHANGE, typeof(BitspokeCamera2D), this, nameof(OnZoomChanged)));
		//LudusCamera2D.ConnectSignal(BitspokeCamera2D.ZOOM_CHANGE, this, nameof(OnZoomChanged));
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
		
		// var regionChildrenCount = 0;
		// var visibleRegions = 0;
		// foreach (var child in LayerContainer.GetChildren())
		// {
		//     if (child is RegionNode)
		//     {
		//         if (((RegionNode)child).Visible)
		//         {
		//             visibleRegions++;
		//             regionChildrenCount += ((RegionNode)child).GetChildCount();
		//         }
		//     }
		// }
		//
		//
		// var visibleSprites = RegionNodes.Where(w => w.Value is { Visible: true }).Sum(s => s.Value.SpritesCount);
		// var totalSprites = RegionNodes.Sum(s => s.Value?.SpritesCount ?? 0);
		//
		// var visibleMeshes = RegionNodes.Where(w => w.Value is { Visible: true }).Sum(s => s.Value.MeshesCount);
		// var totalMeshes = RegionNodes.Sum(s => s.Value?.MeshesCount ?? 0);
		//
		// var totalEntities = RegionNodes.Sum(s => s.Value?.ItemCount ?? 0);
		//
		// var fps = Engine.GetFramesPerSecond();
		// TotalFPS += fps;
		// var avgFPS = TotalFPS / ++TotalProcessCalls;
		// OS.SetWindowTitle($"FPS: {fps}," 
		//                   + $" Avg. FPS: {avgFPS},"
		//                   //+ $" RegionNodes: {visibleRegions}/{RegionNodes.Count},"
		//                   + $" Visible RegionNodes Layers: {regionChildrenCount}"
		//                   + $" Sprites: {visibleSprites}/{totalSprites}"
		//                   + $" Meshes: {visibleMeshes}/{totalMeshes}"
		//                   + $" Total Plants: {totalEntities}");
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
			ElevationTypeDataKey = Find.TypeData.ElevationTypeData["HILLS"].Key,
			VegetationDensityTypeDataKey = Find.TypeData.VegetationDensityTypeData["MED"].Key,
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
		
		for (var regionIndex = 0; regionIndex < map.Data.RegionsContainer.Regions.Length; regionIndex++)
		{
			var regionNode = new PlantRegionNode(map.Data.RegionsContainer.Regions[regionIndex]);
			
			RegionNodes[regionIndex] = regionNode;
			LayerContainer.AddChild(regionNode);
		}
		
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
		// Profiler.Start();
		// var shaderMaterial = new TerrainShaderMaterial(shader, terrainTextureAtlas, tileBlendTexture, Map.Regions.RegionSize.ToVec2Int());
		// foreach (var mapRegion in map.Regions.MapRegions.Values)
		// {
		//     var regionNode = new TerrainRegionNode(mapRegion, shaderMaterial);
		//
		//     if (TerrainRegionNodes.ContainsKey(mapRegion.Index))
		//         TerrainRegionNodes[mapRegion.Index] = regionNode;
		//     else
		//         TerrainRegionNodes.Add(mapRegion.Index, regionNode);
		//     
		//     LayerContainer.AddChild(regionNode);
		// }
		// Profiler.End();
		
		// Profiler.Start();
		// var maxX = 3;
		// var maxY = 3;
		//
		// var shaderMaterial = new TerrainShaderMaterial(shader, terrainTextureAtlas, tileBlendTexture, new Vec2Int(map.Width / maxX, map.Height / maxY));
		//
		// for (int y = 0; y < maxY; y++)
		// {
		//     for (int x = 0; x < maxX; x++)
		//     {
		//         var pos = new Vector2(x, y);
		//         var index = pos.ToIndex(map.Width);
		//
		//         var cellSize = 1;
		//         
		//         var start = new Vector2(x * cellSize * (map.Width / maxX), y * cellSize * (map.Height / maxY));
		//         var size = new Vector2(cellSize * map.Width / maxX, cellSize * map.Height / maxY);
		//         
		//         var regionNode = new TerrainRegionNode(map, new Rect2(start, size), shaderMaterial);
		//         
		//         if (TerrainRegionNodes.ContainsKey(index))
		//             TerrainRegionNodes[index] = regionNode;
		//         else
		//             TerrainRegionNodes.Add(index, regionNode);
		//
		//         LayerContainer.AddChild(regionNode);
		//     }
		// }
		// Profiler.End();

		//
		Profiler.Start();
		TileBlendTexture = ImageUtils.CreateTileBlendTexture(debug:true);
		// TileBlendTexture = new ImageTexture();
		// var compressedTexture2D = (CompressedTexture2D)Find.DB.TextureDB[BLEND_TEXTURE];
		// if (compressedTexture2D != null)
		// {
		// 	TileBlendTexture = new ImageTexture();
		// 	TileBlendTexture.SetImage(compressedTexture2D.GetImage());
		// }
		// else 
		//	TileBlendTexture = ImageUtils.CreateTileBlendTexture();
			
			//TileBlendTexture = ImageUtils.CreateTileBlendTexture(smoothEdge : 32f, smoothCorner : 28f, textureSize:CoreGlobal.STANDARD_CHUNK_SIZE);
		//TileBlendTexture = ImageUtils.CreateTileBlendTexture(debug: true, debugSaveDir: GodotGlobal.DEBUG_ROOT_PATH);
		// smoothEdge : 24f, smoothCorner : 16f, edgePercentage : 50, cornerPercentage : 25, textureSize : 64
		
		
		Shader = Find.DB.ShaderDB[SHADER_KEY];
		

		Profiler.End();
		//OnZoomChanged();
	}

	private ImageTexture TerrainTextureAtlas { get; set; }
	ImageTexture? TileBlendTexture { get; set; }
	private Shader Shader { get; set; }

	private void RenderTerrain(Rect2 regionToRender)
	{
		Profiler.Start();

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
		
		var mapData = Map.GenerateTerrainTexture(regionToRender);
		
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
		terrainRender.ZAsRelative = true;
		
		Profiler.End();
	}

	

	private void OnZoomChanged()
	{
		Log.Debug();
		
		//UIScaleComponent.SetUIScale(UIScaleComponent.Instance.UIScale + 0.5f);

		// // Profiler.Start();
		// var ctrans = GetCanvasTransform();
		// var min_pos = (-ctrans.origin / ctrans.Scale / 64f).ToVec2Int();
		// var view_size = (GetViewportRect().Size / ctrans.Scale / 64f * 1.5f).ToVec2Int();
		// if (view_size.Width > Map.Size.Width)
		//     view_size = Map.Size;
		// // Profiler.End();
		// //
		// //
		// RenderTerrain(new Rect2(min_pos.ToVector2(), view_size.ToVector2()));

	}

	private void OnShowPlants(bool visible, string? plantDefKey = null)
	{
		// TODO ... plant specific
		Log.TODO("implement");
		
		Profiler.Start();
		if (visible)
			TestRenderPlants(Map);
		else
		{
			for (var i = 0; i < RegionNodes.Count; i++)
			{
				var regionNode = RegionNodes[i];
				
				if (regionNode is PlantRegionNode)
					regionNode.QueueFree();

				RegionNodes[i] = null;
			}
		}

		Profiler.End(message:$"SetTerrainLayerVisible: {visible}");
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
