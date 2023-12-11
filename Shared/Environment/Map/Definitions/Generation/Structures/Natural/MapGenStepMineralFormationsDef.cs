using System.Text.Json.Nodes;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Utils.Json;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation.Structures.Natural;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class MapGenStepMineralFormationsDef : MapGenStepDef
{
    #region Properties

    public float? MinElevation { get; set; }

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