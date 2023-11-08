using System.Collections.Generic;
using Bitspoke.Core;
using Bitspoke.Core.Profile;
using Bitspoke.GodotEngine;
using Bitspoke.Ludus.Shared;
using Bitspoke.Ludus.Shared.Environment.Map;
using Bitspoke.Ludus.Shared.Environment.Map.Regions;
using Client.Components.Node.Shaders;
using Godot;

namespace Client.Components.Regions;

public partial class TerrainRegionNode : RegionNode
{
    public TerrainShaderMaterial TerrainShaderMaterial { get; }
    public Sprite2D TerrainSprite { get; private set; }
    
    public ImageTexture MapData { get; private set; }
    
    #region Properties

    #endregion

    #region Constructors and Initialisation

    public TerrainRegionNode()
    {
        
    }
    
    public TerrainRegionNode(Map map, Rect2 rect2, TerrainShaderMaterial terrainShaderMaterial) : base()
    {
        Map = map;
        Dimension = rect2;
        TerrainShaderMaterial = new TerrainShaderMaterial(terrainShaderMaterial);
    }
    
    public TerrainRegionNode(Region region, TerrainShaderMaterial terrainShaderMaterial) : base(region)
    {
       TerrainShaderMaterial = new TerrainShaderMaterial(terrainShaderMaterial);
    }
    
    public override void Init()
    {
        GlobalPosition = Dimension.Position * CoreGlobal.STANDARD_CELL_SIZE;
        RegionLayers = new Dictionary<int, RegionLayer>();
        
        ItemCount = 0;

        MapData = Map.GenerateTerrainDefsTexture(Dimension);
        //ProcessTerrain();
    }
    
    #endregion

    #region Methods

    private void ProcessTerrain()
    {
        TerrainSprite = new Sprite2D();
        AddChild(TerrainSprite);
        TerrainSprite.Texture = Find.DB.TextureDB["default"];
        TerrainSprite.Centered = false;
        
        TerrainSprite.Material = TerrainShaderMaterial;
        TerrainShaderMaterial.SetShaderParameter(TerrainShaderMaterial.SHADER_PARAM_MAP_DATA, MapData);
        TerrainShaderMaterial.SetShaderParameter(TerrainShaderMaterial.SHADER_PARAM_BLEND_FLAG, true);
        
        TerrainSprite.Scale = Dimension.Size;
        TerrainSprite.ZIndex = -1;
        TerrainSprite.ZAsRelative = true;
    }
    
    protected override void OnShow()
    {
        Profiler.Start();
        ProcessTerrain();
        Visible = true;
        Profiler.End();
    }
    
    protected override void OnHide()
    {
        TerrainSprite.QueueFree();
        foreach (var regionLayer in RegionLayers.Values)
        {
            regionLayer.QueueFree();
        }
        
        RegionLayers.Clear();
        Visible = false;
    }
    
    #endregion


    
}