using System.Collections.Generic;
using Bitspoke.Core.Common.Graphics.Textures;
using Bitspoke.Core.Components.Location;
using Bitspoke.GodotEngine.Common.Vector;
using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Utils.Vector;
using Bitspoke.Ludus.Client.Components.Nodes.Sprites;
using Bitspoke.Ludus.Client.Components.Nodes.Sprites.Plants.Natural;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;
using Bitspoke.Ludus.Shared.Environment.Map.Regions;
using Godot;

namespace Bitspoke.Ludus.Client.Components.Regions.Plants;

public partial class PlantRegionNode : RegionNode
{
    #region Properties

    public List<LudusEntity>? Entities { get; set; }

    //public Dictionary<string, List<LudusEntity>> PlantsByType { get; set; }
    public Dictionary<string, Texture2D> Textures { get; set; }
    public Dictionary<string, PlantDef> PlantDefs { get; set; }
    
    public override string NodeName => GetType().Name;
    public override Node Node => this;

    public bool IsDirty { get; set; }
    
    #endregion

    #region Constructors and Initialisation

    public PlantRegionNode(Region region) : base(region)
    {
        //Init();
    }

    public override void Init()
    {
        Textures = new();
        PlantDefs = new();
        
        RegionLayers = new Dictionary<int, RegionLayer>();
            
        foreach (var plantByTypeKey in Region.PlantsByType().Keys)
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

    private int nullSpriteOccurences = 0;
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (IsDirty)
        {
            //Sprites.QueueFree();
            if (Sprites != null)
                Sprites.Free();
            
            //Profile(message: $"[{Region.Index}]:", toProfile:() => ProcessPlants());
            ProcessPlants();
            IsDirty = false;
        }
        
    }

    public void Refresh()
    {
        // we are coming in from another thread so we can't update directly ... we need to flag we need to do an update
        // and then let _Process handle the update
        IsDirty = true;
    }
    
    
    private void ProcessPlants(bool includeMultiMesh = true)
    {
        //var plantsByType = Map.Data.EntitiesContainer.EntitiesByRegion[RegionID];
        //Log.Debug($"Region[{RegionID}] - ProcessPlants");
        int layerID = 0;
        foreach (var plantByType in Region.PlantsByType())
        {
            var def = PlantDefs[plantByType.Key];
            var textureType = def.GraphicDef.TextureDef.TextureTypeDetails.TextureType;

            ItemCount += plantByType.Value.Count;
                
            switch (textureType)
            {
                case TextureType.MultiMesh:
                    if (!includeMultiMesh) break;
                    
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
                    AddSprites(texture, plantByType.Value);
                    break;
                default:
                    break;
            }
        }
    }
    
    #endregion

    protected new void AddSprites(Texture2D texture, List<LudusEntity> regionEntities)
    {
        if (Sprites == null || !IsInstanceValid(Sprites) || Sprites.IsQueuedForDeletion())
            AddChild(Sprites = new());

        foreach (var ludusEntity in regionEntities)
        {
            Sprites.AddSprite<NaturalPlantSprite2D>(ludusEntity, texture);
        }
    }
    
}