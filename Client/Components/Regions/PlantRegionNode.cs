using System.Collections.Generic;
using Bitspoke.Core.Common.Graphics.Textures;
using Bitspoke.Core.Profile;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;
using Bitspoke.Ludus.Shared.Environment.Map.Regions;
using Godot;
using OpenQA.Selenium.DevTools.V115.Profiler;

namespace Client.Components.Regions;

public partial class PlantRegionNode : RegionNode
{
    #region Properties

    public List<LudusEntity>? Entities { get; set; }

    public Dictionary<string, List<LudusEntity>> PlantsByType { get; set; }
    public Dictionary<string, Texture2D> Textures { get; set; }
    public Dictionary<string, PlantDef> PlantDefs { get; set; }

    #endregion

    #region Constructors and Initialisation

    public PlantRegionNode(Region region) : base(region)
    {
        Init();
    }

    protected override void Init()
    {
        Textures = new();
        PlantDefs = new();
            
        GlobalPosition = Region.Dimension.Position * 64;
        RegionLayers = new Dictionary<int, RegionLayer>();
            
        //Map.Data.EntitiesContainer.EntitiesByTypeAndRegion[Region.Index]
        //Profiler.Start();
        PlantsByType = Region.PlantsByType();
        //Profiler.End();
            
        foreach (var plantByTypeKey in PlantsByType.Keys)
        {
            var def = Find.DB.PlantDefs[plantByTypeKey];
            PlantDefs.Add(plantByTypeKey, def);
            Textures.Add(plantByTypeKey, Find.DB.TextureDB[def.GraphicDef.TextureDef.TextureResourcePath]);
        }
    }

    public override void _Ready()
    {
        base._Ready();
        ProcessPlants();
    }

    private void ProcessPlants()
    {
            

        int layerID = 0;
        foreach (var plantByType in PlantsByType)
        {
            var def = PlantDefs[plantByType.Key];
            var textureType = def.GraphicDef.TextureDef.TextureTypeDetails.TextureType;

            ItemCount += plantByType.Value.Count;
                
            switch (textureType)
            {
                case TextureType.MultiMesh:
                    layerID++;
                        
                    MultiMeshRegionLayer layer;
                        
                    // TODO: Fix
                    if (RegionLayers.ContainsKey(layerID))
                        layer = (MultiMeshRegionLayer) RegionLayers[layerID];
                    else
                    {
                        layer = new MultiMeshRegionLayer(layerID, def.GraphicDef, plantByType.Value);
                        RegionLayers.Add(layerID, layer);
                        AddChild(layer);
                    }
                        
                    break;
                case TextureType.Single:
                    layerID++;
                    SpritesCount += plantByType.Value.Count;
                    var texture = Textures[plantByType.Key];
                    AddSprites(texture, Region, plantByType.Value);
                    break;
                default:
                    break;
            }
        }
    }

    protected override void OnShow()
    {
        //Profiler.Start();

        //ProcessPlants();
        Visible = true;

        //Profiler.End();

        foreach (var regionLayer in RegionLayers)
        {
            regionLayer.Value.Refresh();
        }
    }

    protected override void OnHide()
    {
        // foreach (var regionLayer in RegionLayers.Values)
        // {
        //     regionLayer.QueueFree();
        // }
        //
        // RegionLayers.Clear();
        Visible = false;
    }


    #endregion



}