using Bitspoke.Core.Common.Vector;
using Bitspoke.GodotEngine.Utils.Images;
using Godot;

namespace Client.Components.Node.Shaders;

public partial class TerrainShaderMaterial : ShaderMaterial
{

    #region Properties

    public const string TERRAIN_ATLAS = "Terrain/Atlas";
    public const string BLEND_TEXTURE = "Support/tileblend";
    public const string SHADER_KEY = "Layers/terrain";
                     
    public const string SHADER_PARAM_TEXTURE_ATLAS = "textureAtlas";
    public const string SHADER_PARAM_BLEND_TEXTURE = "blendTexture";
    public const string SHADER_PARAM_MAP_DATA = "mapData";
    public const string SHADER_PARAM_MAP_TILES_WIDTH = "mapTilesCountX";
    public const string SHADER_PARAM_MAP_TILES_HEIGHT = "mapTilesCountY";
    public const string SHADER_PARAM_TILE_SIZE_PIXELS = "tileSizeInPixels";
    public const string SHADER_PARAM_HALF_TILE_SIZE_PIXELS = "halfTileSizeInPixels";
    public const string SHADER_PARAM_BLEND_FLAG = "blend";

    public Vec2Int Dimension { get; set; } = Vec2Int.ZERO;
    public ImageTexture TerrainAtlas { get; set; }
    public ImageTexture BlendTexture { get; set; }

    #endregion

    #region Constructors and Initialisation

    public TerrainShaderMaterial(TerrainShaderMaterial toClone) : this (toClone.Shader, toClone.TerrainAtlas, toClone.BlendTexture, toClone.Dimension)
    {
            
    }
        
    public TerrainShaderMaterial(Vec2Int dimension) : this(Find.DB.ShaderDB[SHADER_KEY],
        (ImageTexture)Find.DB.TextureDB[TERRAIN_ATLAS],
        ImageUtils.CreateTileBlendTexture(),
        dimension)
    {
    }

    public TerrainShaderMaterial(Shader shader, ImageTexture terrainAtlas, ImageTexture blendTexture, Vec2Int dimension) : base()
    {
        TerrainAtlas = terrainAtlas;
        BlendTexture = blendTexture;
        Dimension = dimension;
        Shader = shader;
        SetShaderParameter(SHADER_PARAM_TEXTURE_ATLAS, TerrainAtlas);
        SetShaderParameter(SHADER_PARAM_BLEND_TEXTURE, BlendTexture);
        SetShaderParameter(SHADER_PARAM_MAP_TILES_WIDTH, Dimension.Width);
        SetShaderParameter(SHADER_PARAM_MAP_TILES_HEIGHT, Dimension.Height);
        SetShaderParameter(SHADER_PARAM_TILE_SIZE_PIXELS, CoreGlobal.STANDARD_CELL_SIZE);
        SetShaderParameter(SHADER_PARAM_HALF_TILE_SIZE_PIXELS, CoreGlobal.STANDARD_CELL_SIZE / 2f);
    }

    #endregion

    #region Methods



    #endregion


}