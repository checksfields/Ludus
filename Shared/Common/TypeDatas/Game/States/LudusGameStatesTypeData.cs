using Bitspoke.Core.Profiling;
using Bitspoke.Core.Types.Game.States;
using Bitspoke.GodotEngine.Utils.Files;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Common.TypeDatas.Game.States;

public class LudusGameStatesTypeData : GameStatesTypeData
{
    #region Properties

    public const string MAP_GENERATION_KEY = "MAP_GENERATION";
    
    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    #endregion

    #region Bootstrap
    
    public static void Bootstrap(bool writeToFile = false)
    {
        Profiler.Start();
            
        LudusGameStatesTypeData gameStatesTypeData = new LudusGameStatesTypeData();

        var key = GameStatesTypeData.DEFAULT_KEY;
        gameStatesTypeData.Add(key,                       GameStateDefaultTypeDef.BootstrapTypeData<string>(key, false, new List<string>()));
        gameStatesTypeData.Add(key = INITIALISING_KEY,    GameStateDefaultTypeDef.BootstrapTypeData<string>(key, false));
        gameStatesTypeData.Add(key = MAIN_KEY,            GameStateDefaultTypeDef.BootstrapTypeData<string>(key, false));
        gameStatesTypeData.Add(key = NONE_KEY,            GameStateDefaultTypeDef.BootstrapTypeData<string>(key, false, new List<string>()));
        gameStatesTypeData.Add(key = IN_GAME_KEY,         GameStateDefaultTypeDef.BootstrapTypeData<string>(key, true, new List<string> { "ANY" }));
        gameStatesTypeData.Add(key = CONSOLE_OPEN_KEY,    GameStateDefaultTypeDef.BootstrapTypeData<string>(key, false));
        gameStatesTypeData.Add(key = MAP_GENERATION_KEY,  GameStateDefaultTypeDef.BootstrapTypeData<string>(key, false, new List<string> { MAIN_KEY }));
            
        if (writeToFile)
        {
            var filePath = $"{GodotGlobal.RES_ROOT_PATH}{GodotGlobal.TYPE_DATA_ROOT_PATH}/{nameof(GameStatesTypeData)}{GodotGlobal.SUPPORTED_TYPE_DATA_TYPE}";
            GodotFileUtils.WriteToFile(filePath, JsonConvert.SerializeObject(gameStatesTypeData, Formatting.Indented, CoreGlobal.JsonSerializerSettings));
        }
            
        Profiler.End();
    }

    

    #endregion
}