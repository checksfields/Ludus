using System;
using System.Collections.Generic;
using Bitspoke.Core.Definitions.Parts.TypeData;
using Bitspoke.Core.Definitions.TypeData.Time;
using Bitspoke.Core.Types.Game.States;
using Bitspoke.GodotEngine.Utils.Files;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Common.TypeDatas.Game.States;

public class LudusGameStateTypeData : GameStateTypeData
{
    #region Properties

    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    #endregion

    #region Bootstrap
    
    public static void Bootstrap(bool writeToFile = false)
    {
        Profiler.Start();
            
        LudusGameStateTypeData gameStateTypeData = new LudusGameStateTypeData();

        var key = GameStateTypeData.DEFAULT_KEY;
        gameStateTypeData.Add(key,                       GameStateTypeDef.BootstrapTypeData<string>(key, "",false, new List<string>()));
        gameStateTypeData.Add(key = NONE_KEY,            GameStateTypeDef.BootstrapTypeData<string>(key, "", false, new List<string>()));
        gameStateTypeData.Add(key = IN_GAME_KEY,         GameStateTypeDef.BootstrapTypeData<string>(key, "", true, new List<string>()));
        gameStateTypeData.Add(key = CONSOLE_OPEN_KEY,    GameStateTypeDef.BootstrapTypeData<string>(key, "", false, new List<string>()));
            
        if (writeToFile)
        {
            var filePath = $"{GodotGlobal.RES_ROOT_PATH}{GodotGlobal.TYPE_DATA_ROOT_PATH}/{nameof(GameStateTypeData)}{GodotGlobal.SUPPORTED_TYPE_DATA_TYPE}";
            GodotFileUtils.WriteToFile(filePath, JsonConvert.SerializeObject(gameStateTypeData, Formatting.Indented, CoreGlobal.JsonSerializerSettings));
        }
            
        Profiler.End();
    }
    
    #endregion
    
}