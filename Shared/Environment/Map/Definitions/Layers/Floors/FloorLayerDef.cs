using System.Text.Json.Nodes;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Utils.Json;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Floors;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class FloorLayerDef : LayerDef
{
    #region Properties

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