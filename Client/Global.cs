global using CoreGlobal  = Bitspoke.Core.Global;
global using GodotGlobal = Bitspoke.GodotEngine.Global;
global using SharedGlobal = Bitspoke.Ludus.Shared.Global;
global using CoreFind = Bitspoke.Core.Find;
global using GodotFind = Bitspoke.GodotEngine.Find;
global using Find = Bitspoke.Ludus.Shared.Find;
global using static Bitspoke.Core.Profiling.Profiler;
global using Log = Bitspoke.Core.Common.Logging.Log;

namespace Bitspoke.Ludus.Client;

public static class Global
{
    #region Properties

    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    #endregion


    public static void Init()
    {
        SharedGlobal.Init();

        GodotGlobal.ZOOM_2D_MAX = 2.0f;
        GodotGlobal.ZOOM_2D_MIN = 0.25f;
        
        GodotGlobal.RUN_BOOTSTRAP_ENABLED  = false;
    }
}