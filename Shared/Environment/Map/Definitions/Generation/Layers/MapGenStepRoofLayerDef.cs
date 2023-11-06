using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation.Layers;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class MapGenStepRoofLayerDef : MapGenStepDef
{
    #region Properties

    
        
    // TODO: Do we replace this with a set of roof definitions.
    // SEEALSO: MapGenStepRockFormations
    public float? MinElevation { get; set; }
    
    public int? MinRoofSize { get; set; }

    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    #endregion

        
}