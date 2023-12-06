using System.Text.Json.Nodes;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Utils.Json;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class MapGenConfigDef : Def
{
    #region Properties

    public override string Key { get; set; }
    public override string ClassName => null;
    public override string? AssemblyName  => null;

    public List<string> MapGenStepKeys { get; set; }
    
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