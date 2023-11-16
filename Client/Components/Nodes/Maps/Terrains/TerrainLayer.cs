using System;
using Bitspoke.GodotEngine.Components.Nodes._2D;
using Bitspoke.GodotEngine.Utils.Images;
using Bitspoke.Ludus.Client.Components.Nodes.Shaders;
using Bitspoke.Ludus.Shared.Environment.Map;
using Godot;

namespace Bitspoke.Ludus.Client.Components.Nodes.Maps.Terrains;

public partial class TerrainLayer : GodotNode2D
{
    #region Properties

    public virtual string NodeName => GetType().Name;
    public virtual Node Node => this;

    public const String TERRAIN_ATLAS = "Terrain/Atlas";
    public const String BLEND_TEXTURE = "Support/tileblend";
    public const String SHADER_KEY = "Layers/terrain";

    private ImageTexture TerrainTextureAtlas { get; set; } 
    private ImageTexture? TileBlendTexture { get; set; }
    private TerrainShaderMaterial? TerrainShaderMaterial { get; set; }
    private Shader Shader { get; set; }
    
    public Sprite2D TerrainSprite2D { get; private set; }
    
    public Map Map { get; set; }
    public Rect2I RegionToRender { get; set; }
    public ImageTexture MapData { get; set; }
    
    
    #endregion

    #region Constructors and Initialisation

    public TerrainLayer(Map map) : base()
    {
        Map = map;
    }

    #endregion

    #region Overrides
    public override void Init()
    {
        Profile(() => { 
            RegionToRender = new Rect2I(Vector2I.Zero, Map.Size);
            TileBlendTexture = ImageUtils.CreateTileBlendTexture();
            TerrainTextureAtlas = (ImageTexture)Find.DB.TextureDB[TERRAIN_ATLAS];
            Shader = Find.DB.ShaderDB[SHADER_KEY];
            
            InitTerrainShaderMaterial();
        });
        
        Render();
    }
    
    private void InitTerrainShaderMaterial()
    {
        Profile(() =>
        {
            MapData = Map.GenerateTerrainDefsTexture(RegionToRender);
            TerrainShaderMaterial = new TerrainShaderMaterial(Shader, TerrainTextureAtlas, TileBlendTexture, Map.Size);
            TerrainShaderMaterial.SetShaderParameter(TerrainShaderMaterial.SHADER_PARAM_BLEND_FLAG, true);
            TerrainShaderMaterial.SetShaderParameter(TerrainShaderMaterial.SHADER_PARAM_MAP_DATA, MapData);
        });

        
    }
    
    public override void AddComponents() { }
    public override void ConnectSignals() { }

    #endregion

    #region Methods

    public void Render()
    {
        Profile(() =>
        {
            if (TerrainSprite2D != null)
                TerrainSprite2D.QueueFree();
            
            TerrainSprite2D = new Sprite2D();
            TerrainSprite2D.Texture = Find.DB.TextureDB["default"];
            TerrainSprite2D.Centered = false;
            TerrainSprite2D.Material = TerrainShaderMaterial;
            TerrainSprite2D.Scale = RegionToRender.Size;
            TerrainSprite2D.ZIndex = -1;
            TerrainSprite2D.ZAsRelative = false;

            AddChild(TerrainSprite2D);
            
        });
    }

    #endregion


    
}