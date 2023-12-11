using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Definitions.Parts.Graphics;
using Bitspoke.Core.Definitions.Parts.Graphics.Links;
using Bitspoke.Core.Definitions.Parts.Graphics.Textures;
using Bitspoke.Core.Definitions.Parts.Graphics.Textures.Types;
using Bitspoke.Core.Utils.Json;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Terrain;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class TerrainDef : LayerDef
{
    #region Properties

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)] public int OrderIndex { get; set; }
    public float? Fertility { get; set; }
   
    
    [JsonPropertyName("Graphic")] public GraphicDef Graphic { get; set; } = null;
    
    #endregion
    
    #region Constructors and Initialisation

    #endregion

    #region Methods
    public override IDef Deserialize(JsonNode node)
    {
        return node.DeserializeAnonymousType(this);
    }
    public TerrainDef Clone()
    {
        var toClone = new TerrainDef();
        base.Clone(this, toClone);
        
        toClone.OrderIndex = OrderIndex;
        toClone.Fertility = Fertility;

        return toClone;
    }
    
    #endregion

    #region Bootstrap

    public static TerrainDef Bootstrap(int orderIndex, string key, float fertility = 0f)
    {
        var terrainDef = new TerrainDef();

        terrainDef.OrderIndex = orderIndex;
        terrainDef.Key = key;
        terrainDef.IsModdable = true;
        terrainDef.Name = key;
        terrainDef.LabelKey = $"LOC_{key.ToUpper()}";
        terrainDef.DescriptionKey = $"LOC_{key.ToUpper()}_DESC";
        terrainDef.Fertility = fertility;
        terrainDef.GraphicDef = new GraphicDef
        {
            Texture = new TextureDef
            {
                TextureResourcePath = $"{GodotGlobal.TEXTURES_RESOURCE_ROOT_PATH}/{Global.MAP_LAYERS_ROOT_PATH}/{key}",
                TextureTypeDetails = new SingleTextureTypeDetailsDef(),
            },
            LinkDef = new LinkDef { LinkFlags = LinkFlags.Edge | LinkFlags.Rock | LinkFlags.Wall }
        };

        return terrainDef;
    }
    
    public static TerrainDef BootstrapRuntime(int index, string key)
    {
        var terrainLayerDef = Bootstrap(index, key, 0f);
        terrainLayerDef.AvailableAffordanceKeys = new() { "LightStructures", "MediumStructures", "HeavyStructures" };
        terrainLayerDef.GraphicDef.Texture.TextureResourcePath = $"{GodotGlobal.TEXTURES_RESOURCE_ROOT_PATH}/{Global.MAP_LAYERS_ROOT_PATH}/RoughStone";
        
        return terrainLayerDef;
    }

    #endregion

}