using Bitspoke.Ludus.Shared.Environment.Map;
using Bitspoke.Ludus.Shared.Environment.World;

namespace Bitspoke.Ludus.Shared;

public static class FindExtension
{
    #region Properties

    public static World FindWorld(this int worldID) => Find.World(worldID);
    public static Map FindMap(this int mapID) => Find.Map(mapID);
    
    #endregion
    
}