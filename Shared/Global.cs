global using CoreGlobal  = Bitspoke.Core.Global;
global using GodotGlobal = Bitspoke.GodotEngine.Global;
global using CoreFind = Bitspoke.Core.Find;
global using GodotFind = Bitspoke.GodotEngine.Find;
global using Find = Bitspoke.Ludus.Shared.Find;
global using static Bitspoke.Core.Profiling.Profiler;
global using Log = Bitspoke.Core.Common.Logging.Log;
global using Bitspoke.Core.Common.Logging;
using System.Reflection;
using Bitspoke.Core.Common.Vector;
using Bitspoke.Core.Definitions.TypeDatas.Time;
using Bitspoke.Core.Profiling;
using Godot;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared;

public static class Global
{
        
        
    public static bool RUN_BOOTSTRAP_ENABLED { get; set; } = true;
    public static bool RUN_TESTS_ENABLED { get; set; } = true;
    public static bool DEBUG_ENABLED { get; set; } = true;
    public static bool PROFILER_ENABLED { get; set; } = true;
        
    public static string MAP_LAYERS_ROOT_PATH { get; set; } = "Layers";
        
    public static Vec3Int MAX_MAP_DIMENSIONS { get; set; } = new Vec3Int(275, 275);
    //public static Vec3Int DEFAULT_MAP_REGION_DIMENSIONS { get; set; } = new Vec3Int(25, 25);
    public static Vector2I DEFAULT_MAP_REGION_DIMENSIONS => new Vector2I(25, 25);

    public static float PLANT_GROW_DAY_TICKS => 60000.0f;
        
        
    public static void Init()
    {
        GodotGlobal.Init();
            
        // core global overrides
        CoreGlobal.APPLICATION_ASSEMBLY_NAME = Assembly.GetExecutingAssembly().GetName().Name;
        CoreGlobal.RUN_TESTS_ENABLED = RUN_TESTS_ENABLED;
        CoreGlobal.RUN_BOOTSTRAP_ENABLED = RUN_BOOTSTRAP_ENABLED;
        CoreGlobal.DEBUG_ENABLED = DEBUG_ENABLED;
        CoreGlobal.PROFILER_ENABLED = PROFILER_ENABLED;
        CoreGlobal.MIN_PROFILER_PRIORITY_LEVEL = ProfilerPriorityLevel.Low;
            
        CoreGlobal.LogSettings = new LogSettings
        {
            DebugEnabled = CoreGlobal.DEBUG_ENABLED,
            ProfilerEnabled = CoreGlobal.PROFILER_ENABLED,
            TODOLogEnabled = CoreGlobal.TODO_LOG_ENABLED,
            LogLevels = LogLevels.All | LogLevels.File
        };
            
        CoreGlobal.JsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.None,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            //SerializationBinder = new TypeNameSerializationBinder(),
        };
            
        Log.TODO("Implement and Move");
        #region MOVE TO GAME CONFIG / SAVE GAME FILE

        CoreGlobal.DEFAULT_GAME_SPEED = GameSpeedTypeData.NORMAL_KEY;

        #endregion


    }
        
}