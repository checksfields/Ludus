using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bitspoke.Ludus.Shared.Common.States.Games;

[Flags]
[JsonConverter(typeof(StringEnumConverter))]
public enum GameState
{
    None,
    Initialising,
    Entry,      
    Main,       
    Game,       
    GameConfig, 
    WorldGen,   
    MapGen,     
    Loading,    
    Saving,     
    WorldView,  
    MapView,
}