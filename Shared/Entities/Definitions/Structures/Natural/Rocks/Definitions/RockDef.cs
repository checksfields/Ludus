using Bitspoke.Core.Definitions.Parts.Common;
using Bitspoke.Core.Definitions.Parts.Graphics;
using Bitspoke.Core.Definitions.Parts.Graphics.Links;
using Bitspoke.Core.Definitions.Parts.Graphics.Textures;
using Bitspoke.Core.Definitions.Parts.Graphics.Textures.Types;
using Bitspoke.Ludus.Shared.Common.Entities;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Entities.Definitions.Structures.Natural.Rocks.Definitions;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class RockDef : NaturalStructureDef
{
    #region Properties

    public override string Key { get; set; }
    public override EntityType Type { get; set; } = EntityType.Structure;
    public override int SubTypesFlag => -1;

    public GraphicDef? GraphicDef { get; set; }
    public LinkDef?    LinkDef { get; set; }

    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    public RockDef Clone()
    {
        var toClone = new RockDef();
        base.Clone(toClone);
        
        toClone.GraphicDef = GraphicDef;
        toClone.LinkDef = LinkDef;
        
        return toClone;
    }
    
    public override bool HasSubTypeFlag(int? flagAsInt)
    {
        //return SubTypes.HasFlag((PlantType)flagAsInt);
        return false;
    }
    
    #endregion

    #region Bootstrap

    public static RockDef Bootstrap(string key, ColourDef modulate)
    {
        var textureDef = new TextureDef
        {
            TextureResourcePath = $"{GodotGlobal.RES_ROOT_PATH}/{GodotGlobal.TEXTURES_RESOURCE_ROOT_PATH}/Entities/Structure/Natural/Rocks/rockatlas.png",
            TextureTypeDetails = new AtlasTextureTypeDetailsDef()
        };

        return new RockDef
        {
            Key = key,
            GraphicDef = new GraphicDef { TextureDef = textureDef, Modulate = modulate },
            LinkDef = new() { LinkFlags = LinkFlags.Rock | LinkFlags.Edge }
        };
    }

    #endregion
}
