using Bitspoke.Core.Common.Vector;
using Bitspoke.Core.Components.Identity;
using Godot;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map;

public class MapInitConfig
{
    #region Properties

    public int SeedPart { get; set; } = -1;
    [JsonIgnore] public IDComponent WorldID { get; set; }

    [JsonIgnore] public string BiomeKey { get; set; }
    public Vector2I Size { get; set; } = Vector2I.Zero;

    public string ElevationTypeDataKey { get; set; }
    public string VegetationDensityTypeDataKey { get; set; }

    public List<string>? AvailableRockDefKeys { get; set; }

    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    #endregion


}