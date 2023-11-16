using Bitspoke.Ludus.Shared.Environment.Map;
using Bitspoke.Ludus.Shared.Environment.World;

namespace Bitspoke.Ludus.Shared;

public static class FindExtension
{
    #region Properties

    public static World FindWorld(this ulong? worldID) => Find.World(worldID.Value);
    public static Map FindMap(this ulong mapID) => Find.Map(mapID);
    
    #endregion
    
}