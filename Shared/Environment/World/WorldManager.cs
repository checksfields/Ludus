using Bitspoke.Core.Profiling;
using Bitspoke.Core.Random;
using Bitspoke.GodotEngine.Managers;
using Bitspoke.GodotEngine.Utils.IO;
using Bitspoke.Ludus.Shared.Environment.World.Generation;
using Godot;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.World;

public partial class WorldManager : Manager
{
    #region Properties

    private static WorldManager instance { get; set; } = null;
    public static WorldManager Instance
    {
        get
        {
            if (instance == null)
                instance = new WorldManager();

            return instance;
        }
    }
        
    public World CurrentWorld { get; set; } = null;
    
    public override string NodeName => GetType().Name;
    public override Node Node => this;
    
    

    #endregion

    #region Constructors and Initialisation
    // none    
    #endregion

    #region Overrides

    public override void Init() {}
    public override void AddComponents() {}
    public override void ConnectSignals() {}

    #endregion
    
    #region Methods

    public static void SaveWorld(string fileName, World toSave = null)
    {
        if (toSave == null)
            toSave = Instance.CurrentWorld;

        GodotFileUtils.WriteToFile($"{GodotGlobal.SAVE_ROOT_PATH}/{fileName}{GodotGlobal.SUPPORTED_SAVE_TYPE}", JsonConvert.SerializeObject(toSave, Formatting.Indented));
    }
        
    public static World GenerateWorld(WorldInitConfig initConfig)
    {
        Profiler.Start();
            
        Rand.PushState(initConfig.SeedPart);
        if (Instance.CurrentWorld != null)
            Log.Exception("Cannot create a new world as one already exists.", -9999999);

        Instance.CurrentWorld = WorldGenerator.Generate(initConfig);
        Rand.PopState();
            
        Profiler.End();
        return Instance.CurrentWorld;
            
    }
        
    #endregion


        
}