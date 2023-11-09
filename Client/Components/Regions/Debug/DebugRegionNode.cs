using System.Collections.Generic;
using Bitspoke.Ludus.Shared.Environment.Map.Regions;
using Godot;

namespace Client.Components.Regions.Debug;

public partial class DebugRegionNode : RegionNode
{
    #region Properties

    public override string Name => GetType().Name;
    
    #endregion

    #region Constructors and Initialisation
    
    public DebugRegionNode(Region region) : base(region)
    {
    }
    
    protected override void Init()
    {
        GlobalPosition = Region.Dimension.Position * 64;
        
        //ProcessTerrain();
        //ProcessPlants();
        
        Visible = false;
    }
    
    public override void _Draw()
    {
        base._Draw();
        if (CoreGlobal.DEBUG_ENABLED)
        {
                
            var size = Region.Dimension.Size * CoreGlobal.STANDARD_CELL_SIZE;
            var rect2 = new Rect2(Vector2.Zero, size);
            DrawRect(rect2, Colors.Red, false, width:1f);
        }
    }

    public void ProcessTerrain(string terrainTypeKey = null)
    {
        var terrainsByType = Region.TerrainsByType();
        
        if (RegionLayers == null || RegionLayers == default)
            RegionLayers = new Dictionary<int, RegionLayer>();

        int layerID = RegionLayers.Count;
        foreach (var terrainByType in terrainsByType)
        {
            if (terrainTypeKey != null && terrainByType.Key != terrainTypeKey)
                continue;
            
            var def = Find.DB.TerrainDefs[terrainByType.Key];
            //var texture = Find.DB.TextureDB[def.GraphicDef.TextureDef.TextureResourcePath];
            var texture = Find.DB.TextureDB["default"];
            var textureType = def.GraphicDef.TextureDef.TextureTypeDetails.TextureType;

            ItemCount += terrainByType.Value.Count;
            var layer = new DebugMultiMeshRegionLayer(layerID, texture, terrainByType.Value);
            RegionLayers.Add(layerID++, layer);
            AddChild(layer);

            //var layer = new MultiMeshRegionLayer(layerID, texture, terrainByType.Value);


            // switch (textureType)
            // {
            //     case TextureType.MultiMesh:
            //         var layer = new MultiMeshRegionLayer(layerID, texture, plantByType.Value);
            //         RegionLayers.Add(layerID++, layer);
            //         AddChild(layer);
            //         break;
            //     case TextureType.Single:
            //         layerID++;
            //         SpritesCount += plantByType.Value.Count;
            //         AddSprites(texture, Region, plantByType.Value);    
            //         break;
            //     default:
            //         break;
            // }
        }

    }

    // private void ProcessPlants()
    // {
    //     var plantsByType = Region.PlantsByType();
    //     
    //     int layerID = 0;
    //     foreach (var plantByType in plantsByType)
    //     {
    //         var def = Find.DB.PlantDefs[plantByType.Key];
    //         var texture = Find.DB.TextureDB[def.GraphicDef.TextureDef.TextureResourcePath];
    //         var textureType = def.GraphicDef.TextureDef.TextureTypeDetails.TextureType;
    //
    //         ItemCount += plantByType.Value.Count;
    //         
    //         switch (textureType)
    //         {
    //             case TextureType.MultiMesh:
    //                 var layer = new MultiMeshRegionLayer(layerID, texture, plantByType.Value);
    //                 RegionLayers.Add(layerID++, layer);
    //                 AddChild(layer);
    //                 break;
    //             case TextureType.Single:
    //                 layerID++;
    //                 SpritesCount += plantByType.Value.Count;
    //                 AddSprites(texture, Region, plantByType.Value);    
    //                 break;
    //             default:
    //                 break;
    //         }
    //     }
    //
    // }

    #endregion

    #region Methods

    protected override void OnShow() { }
    protected override void OnHide() { }

    #endregion



}