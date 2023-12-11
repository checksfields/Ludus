using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Definitions.Generation;
using Bitspoke.Core.Definitions.Parts.Common.Noise;
using Bitspoke.Core.Utils.Json;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class MapGenStepDef : GenStepDef
{
    #region Properties

    public override string Key { get; set; }
    public NoiseDef NoiseDef { get; set; }
    [JsonPropertyName("GenStepType")] public string GenStepType { get; set; }

    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods
    public override IDef Deserialize(JsonNode node)
    {
        return node.DeserializeAnonymousType(this);
    }
    #endregion

        
}