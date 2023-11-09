using Bitspoke.Core.Common.Vector;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.World;

public class WorldInitConfig
{
    #region Properties

    [JsonIgnore] public Vec2Int Dimensions { get; set; }
    public int SeedPart { get; set; } = -1;

    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    #endregion


}