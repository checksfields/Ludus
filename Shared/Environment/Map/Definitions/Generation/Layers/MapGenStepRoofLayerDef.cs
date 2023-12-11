using System.Text.Json.Nodes;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Utils.Json;
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
    // none
    #endregion

    #region Overrides

    public override IDef Deserialize(JsonNode node)
    {
        return node.DeserializeAnonymousType(this);
    }

    #endregion
    
    #region Methods
    // none
    #endregion

        
}