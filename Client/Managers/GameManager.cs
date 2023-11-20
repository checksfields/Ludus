using System.Linq;
using Bitspoke.Core.Common.Maths.Geometry;
using Bitspoke.Core.Common.States.Games;
using Bitspoke.Core.Common.Vector;
using Bitspoke.Core.Definitions.TypeDatas.Time;
using Bitspoke.Core.Signal;
using Bitspoke.Core.Systems.Age;
using Bitspoke.Core.Systems.Time;
using Bitspoke.Core.Utils.Primatives.Float;
using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Components.Nodes._2D;
using Bitspoke.GodotEngine.Components.UI.Scale;
using Bitspoke.GodotEngine.Controllers.Resources;
using Bitspoke.GodotEngine.Controllers.Resources.Loaders.Implementations;
using Bitspoke.GodotEngine.Utils.Files;
using Bitspoke.Ludus.Shared.Common.TypeDatas.Game.States;
using Bitspoke.Ludus.Shared.Components.Settings.Game;
using Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;
using Bitspoke.Ludus.Shared.Entities.Definitions.Structures.Natural.Rocks.Definitions;
using Bitspoke.Ludus.Shared.Environment.Biome.Definitions;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Affordances;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Floors;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Roof;
using Bitspoke.Ludus.Shared.Environment.World.TypeData;
using Bitspoke.Ludus.Shared.Systems.Environmental;
using Bitspoke.Ludus.Shared.Systems.Growth;
using Bitspoke.Ludus.Shared.Systems.Time;
using Godot;
using TerrainDefsCollection = Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Terrain.TerrainDefsCollection;

namespace Bitspoke.Ludus.Client.Managers;

public partial class GameManager : GodotNode2D
{
    #region Properties

    public static string QUIT_APPLICATION => nameof(QuitRequest);
    private delegate void QuitRequest();
        
    private static GameManager instance { get; set; }
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                Log.Exception("Instance is not set.  Make sure Instance is set through the Godot Game loop",
                    -9999999);

            return instance;
        }
    }
    public override string NodeName => GetType().Name;
    public override Node Node => this;

    public LudusGameSettingsComponent LudusGameSettingsComponent => LudusGameSettingsComponent.Instance; 

    #endregion

    #region Constructors and Initialisation

    // public GameManager() : base()
    // {
    //     Log.Info("GameManager Constructor");
    //     Init();
    //     AddComponents();
    //     ConnectSignals();
    // }

    public override void _EnterTree()
    {
        Log.Info("_EnterTree");
        base._EnterTree();
    }

    public override void Init()
    {
        Log.Info("Init");
        Global.Init();

        Log.Info("Setting Instance");
        instance = this;

        if (GodotGlobal.RUN_BOOTSTRAP_ENABLED)
        {
            Log.Info("Running Bootstrap");
            RunBootstrap();
        }
                

        Log.Info("Adding Resource Controller");
        //var rc =;
        AddChild( new ResourceController());
        //AddChild( new AdminConsole());
        //rc.Init();
    }

    public override void AddComponents()
    {
        Log.Info("AddComponents");
        
        this.AddGodotNode(new LudusGameSettingsComponent());
            
        _ = new GameStateManager(LudusGameStatesTypeData.INITIALISING_KEY);
        _ = new GameSpeedSystem();
        _ = new TimeSystem(true);
        _ = new GrowthSystem();
        _ = new AgeSystem();
        _ = new WeatherSystem();
        _ = new CalendarSystem();
            
        LoadCachedData();
        LoadRuntime();
        LoadDefaultSettings();
        
        AddChild(new UIScaleComponent());
    }

    public override void ConnectSignals()
    {
        Log.Info();
        SignalManager.Connect(new SignalDetails(QUIT_APPLICATION, GetType(), this, nameof(OnQuitRequest)));
    }
    #endregion

    #region Methods

    private void LoadRuntime()
    {
        Log.Info();
        // load any application specific runtime resources
        // terrain atlas texture
        var terrainDefsKeys = Find.DB.TerrainDefs.OrderBy(td => td.Value.OrderIndex).Select(s => s.Key).ToList();
        TextureLoader.Instance.LoadAtlasTexture("Terrain/", "Terrain/Atlas", terrainDefsKeys);
    }
        
    private void LoadCachedData()
    {
        Log.Info();
        Circle.PrimeCache((LudusGameSettingsComponent.MapSize.x / 2f).Ceiling());
    }
        
    private void RunBootstrap()
    {
        Log.Info();
        BiomeDefsCollection.Bootstrap(true);
        MapGenStepDefsCollection.Bootstrap(true);
        LayerAffordanceDefsCollection.Bootstrap(true);
        TerrainDefsCollection.Bootstrap(true);
        FloorLayerDefsCollection.Bootstrap(true);
        RockDefsCollection.Bootstrap(true);
        RoofDefsCollection.Bootstrap(true);
        PlantDefsCollection.Bootstrap(true);

        ElevationTypeData.Bootstrap(true);
        LudusGameStatesTypeData.Bootstrap(true);
            
        var gameSpeedTypeDataFilePath = $"{GodotGlobal.RES_ROOT_PATH}{GodotGlobal.TYPE_DATA_ROOT_PATH}/{nameof(GameSpeedTypeData)}{GodotGlobal.SUPPORTED_TYPE_DATA_TYPE}";
        GameSpeedTypeData.Bootstrap(GodotFileUtils.WriteToFile, gameSpeedTypeDataFilePath, true);
            
        var tickTypeDataFilePath = $"{GodotGlobal.RES_ROOT_PATH}{GodotGlobal.TYPE_DATA_ROOT_PATH}/{nameof(TickTypeData)}{GodotGlobal.SUPPORTED_TYPE_DATA_TYPE}";
        TickTypeData.BootstrapTypeData(GodotFileUtils.WriteToFile, tickTypeDataFilePath, true);
            
        VegetationDensityTypeData.Bootstrap(true);
    }

    private void OnQuitRequest()
    {
        GetTree().AutoAcceptQuit = true;
        GetTree().Quit();
    }

    private void LoadDefaultSettings()
    {
        // TODO: Load from default or saved settings
        LudusGameSettingsComponent.Instance.GraphicsSettingsComponent.WindowSize = new Vec2Int(1920, 1080);
        LudusGameSettingsComponent.Instance.GraphicsSettingsComponent.MaxFPS = 0;
        LudusGameSettingsComponent.Instance.GraphicsSettingsComponent.VSyncMode = DisplayServer.VSyncMode.Enabled;

        LudusGameSettingsComponent.Instance.GraphicsSettingsComponent.WindowSetPositionFromDecoration = true;
        LudusGameSettingsComponent.Instance.GraphicsSettingsComponent.WindowFullScreen = false;
        LudusGameSettingsComponent.Instance.GraphicsSettingsComponent.WindowBorderless = false;
            
        LudusGameSettingsComponent.Instance.GraphicsSettingsComponent.SetWindowPositionCentre();
            
    }
        
    #endregion


}