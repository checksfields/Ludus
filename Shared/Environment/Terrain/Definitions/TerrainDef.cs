using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Definitions.Parts.Graphics;
using Bitspoke.Core.Utils.Json;
using Bitspoke.Ludus.Shared.Common.Definitions.Placement;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Terrain.Definitions;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class TerrainDef : Def
{
    #region Properties
    public override string Key { get; set; }
    public override string ClassName => GetType().FullName;
    public override string? AssemblyName => GetType().Assembly.GetName().Name;

    public float? Fertility { get; set; } = null;
    public bool IsNaturalStructure { get; set; } = false;
    public List<PlacementMaskDef> AcceptablePlacementMaskDefs { get; set; } = null;

    public GraphicDef GraphicDef { get; set; } = null;
    [JsonPropertyName("Graphic")] public GraphicDef Graphic { get; set; } = null;

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