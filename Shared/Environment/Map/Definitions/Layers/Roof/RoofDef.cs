using System.Text.Json.Nodes;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Utils.Json;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Roof;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class RoofDef : LayerDef
{
    #region Properties

    public float? ElevationMultiplier { get; set; }
    
    
    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods
    public override IDef Deserialize(JsonNode node)
    {
        return node.DeserializeAnonymousType(this);
    }
    public RoofDef Clone()
    {
        var clone = new RoofDef();
        base.Clone(this, clone);

        clone.ElevationMultiplier = ElevationMultiplier;

        return clone;
    }
    
    #endregion

    #region Bootstrap

    public static RoofDef Bootstrap(string key, float elevationMultiplier, bool isNatural = true)
    {
        var def = new RoofDef();

        def.Key = key;
        def.ElevationMultiplier = elevationMultiplier;
        def.IsNatural = isNatural;

        def.LabelKey = $"LOC_{def.Key.ToUpper()}";
        def.DescriptionKey = $"LOC_DESC_{def.Key.ToUpper()}";
        
        return def;
    }

    #endregion
}