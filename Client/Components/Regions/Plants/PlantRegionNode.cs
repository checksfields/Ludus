using System.Collections.Generic;
using System.Linq;
using Bitspoke.Core.Common.Graphics.Textures;
using Bitspoke.Core.Databases.Keys;
using Bitspoke.Core.Definitions.Parts.Graphics;
using Bitspoke.Ludus.Client.Components.Nodes.Sprites;
using Bitspoke.Ludus.Client.Components.Nodes.Sprites.Plants.Natural;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Components.Entities.Living;
using Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;
using Bitspoke.Ludus.Shared.Environment.Map.Regions;
using Godot;

namespace Bitspoke.Ludus.Client.Components.Regions.Plants;

public partial class PlantRegionNode : RegionNode
{
    #region Properties

    public List<LudusEntity>? Entities { get; set; }

    //public Dictionary<string, List<LudusEntity>> PlantsByType { get; set; }
    public System.Collections.Generic.Dictionary<string, Texture2D> Textures { get; set; }
    public System.Collections.Generic.Dictionary<string, PlantDef> PlantDefs { get; set; }
    
    public override string NodeName => GetType().Name;
    public override Node Node => this;

    public bool IsDirty { get; set; }
    public bool SpritesReadyForRefresh { get; set; } = false;

    private Dictionary<Texture2D, List<LudusEntity>> EntitiesToAdd { get; set; } = new();
    private Dictionary<string, Dictionary<ulong, NaturalPlantSprite2D>> SpriteNodes { get; set; } = new();
    private Dictionary<string, List<ulong>> EntitiesToRemove { get; set; } = new();
    
    public bool MeshIsDirty { get; set; }
    public bool MeshesReadyForRefresh { get; set; } = false;
    public System.Collections.Generic.Dictionary<int, MultiMeshRegionLayer> MeshStagingNodes { get; set; }

    
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
        
        RegionLayers = new System.Collections.Generic.Dictionary<int, RegionLayer>();
            
        foreach (var plantByTypeKey in Region.PlantsByType().Keys)
        {
            var def = Find.DB.PlantDefs[plantByTypeKey];
            PlantDefs.Add(plantByTypeKey, def);
            
            foreach (var (key, value) in def.Graphic.Texture.Variations)
            {                    
                // TODO - Tier 1: Critical Fix this ... it is only getting the first instance of the variations.  It needs to hold all of them
                var ok = false;
                
                foreach (var textureVariationDef in value)
                {
                    var fullPath = $"{def.Graphic.Texture.RootPath }{textureVariationDef.Path}";
                    Find.DB.TextureDB.TryGetValue(KeyFactory.TextureDatabaseKey(fullPath), out var texture);
                    if (texture == null)
                        continue;
                    
                    ok = Textures.TryAdd(plantByTypeKey, texture);
                    if (ok)
                        break;
                }
                
                if (ok)
                    break;
            }
        }
        Name = $"{RegionID}_{NodeName}_{GetInstanceId()}";
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
            //Log.Debug($"*** Total Nodes in Tree: {GetTree().CurrentScene.GetChildCount()}");
            ProcessIsDirty();
        }

        if (SpritesReadyForRefresh == true)
        {
            if (Sprites == null || !IsInstanceValid(Sprites) || Sprites.IsQueuedForDeletion())
            {
                AddChild(Sprites = new());
                Sprites.Name = $"SpriteContainer_{Sprites.GetInstanceId()}";
            }
            
            foreach (var toAdd in EntitiesToAdd)
            {
                var texture = toAdd.Key;
                foreach (var ludusEntity in toAdd.Value)
                {
                    var sprite2D = ludusEntity.BuildSprite<NaturalPlantSprite2D>(GlobalPosition, texture);

                    var key = ludusEntity.Def.Key;
                    if (!SpriteNodes.ContainsKey(key))
                        SpriteNodes.Add(key, new Dictionary<ulong, NaturalPlantSprite2D>());
                    
                    if (SpriteNodes[key].TryAdd(ludusEntity.ID, sprite2D))
                    {
                        Sprites.AddChild(sprite2D);
                    }
                }
            }
            EntitiesToAdd.Clear();
            SpritesReadyForRefresh = false;
            
            foreach (var toRemove in EntitiesToRemove)
            {
                foreach (var id in toRemove.Value)
                {
                    var sprite = SpriteNodes[toRemove.Key][id]; 
                    SpriteNodes[toRemove.Key].Remove(id);
                    sprite.QueueFree();  
                }
            }
            EntitiesToRemove.Clear();
        }

        if (MeshesReadyForRefresh)
        {    foreach (var stagingMeshes in MeshStagingNodes)
            {
                RegionLayers.Add(stagingMeshes.Key, stagingMeshes.Value);
                AddChild(stagingMeshes.Value);
            }
            
            MeshesReadyForRefresh = false;
            MeshStagingNodes.Clear();// = null;
        }
    }

    private void ProcessIsDirty()
    {
        //Profile(message: $"[{Region.Index}]:", toProfile:() => ProcessPlants(MeshIsDirty));
        ProcessPlants(MeshIsDirty);
        IsDirty = false;
        // Task.Run(() =>
        // {
        //     ProcessPlants(MeshIsDirty);
        //     IsDirty = false;
        // }).ContinueWith(OnProcessIsDirtyComplete);
    }

    // private void OnProcessIsDirtyComplete(Task completedTask) { }

    public void GrowthRefresh()
    {
        // Log.Debug();
        // we are coming in from another thread so we can't update directly ... we need to flag we need to do an update
        // and then let _Process handle the update
        IsDirty = true;
    }
    
    public void AgeRefresh()
    {
        // Log.Debug();
        // we are coming in from another thread so we can't update directly ... we need to flag we need to do an update
        // and then let _Process handle the update
        //IsDirty = true;
    }
    
    
    private void ProcessPlants(bool includeMultiMesh = true)
    {
        //var plantsByType = Map.Data.EntitiesContainer.EntitiesByRegion[RegionID];
        //Log.Debug($"Region[{RegionID}] - ProcessPlants");
        var spriteNodeKeys = new List<string>(SpriteNodes.Keys);
        int layerID = 0;
        
        var plantsByType = Region.PlantsByType();
        foreach (var plantByType in plantsByType)
        {
            spriteNodeKeys.Remove(plantByType.Key);
            var def = PlantDefs[plantByType.Key];
            var textureType = def.Graphic.Texture.TextureTypeDetails.TextureType;

            ItemCount += plantByType.Value.Count;
                
            switch (textureType)
            {
                case TextureType.MultiMesh:
                    if (!includeMultiMesh) break;
                    
                    // layerID++;
                    //     
                    // MultiMeshRegionLayer layer;
                    //     
                    // // TODO: Fix
                    // if (RegionLayers.ContainsKey(layerID))
                    //     layer = (MultiMeshRegionLayer) RegionLayers[layerID];
                    // else
                    // {
                    //     layer = new MultiMeshRegionLayer(layerID, def.GraphicDef, plantByType.Value);
                    //     RegionLayers.Add(layerID, layer);
                    //     AddChild(layer);
                    // }
                    layerID++;
                    AddMeshes(layerID, def.Graphic, plantByType.Value);
                        
                    break;
                case TextureType.Single:
                    layerID++;
                    var texture = Textures[plantByType.Key];
                    UpdateSprites(texture, def.Key, plantByType.Value);
                    break;
                default:
                    break;
            }
        }
        
        foreach (var spriteNodeKey in spriteNodeKeys)
        {
            var nodeIDList = SpriteNodes[spriteNodeKey]?.Keys.ToList() ?? new List<ulong>();
            if (nodeIDList.Count > 0)
            {
                if (EntitiesToRemove.ContainsKey(spriteNodeKey))
                {
                    EntitiesToRemove[spriteNodeKey].AddRange(nodeIDList);
                    continue;
                }
                
                EntitiesToRemove.Add(spriteNodeKey, nodeIDList);
            }
        }
        
    }
    
    #endregion

    private void AddMeshes(int layerID, GraphicDef graphicDef, List<LudusEntity> regionEntities)
    {
        if (MeshStagingNodes == null)
            MeshStagingNodes = new();
        
        MultiMeshRegionLayer layer;
                        
        // TODO: Fix
        if (MeshStagingNodes.ContainsKey(layerID))
            layer = (MultiMeshRegionLayer) MeshStagingNodes[layerID];
        else
        {
            layer = new MultiMeshRegionLayer(layerID, graphicDef, regionEntities);
            MeshStagingNodes.Add(layerID, layer);
        }

        MeshesReadyForRefresh = true;
    }
    
    private void UpdateSprites(Texture2D texture, string entityTypeKey, List<LudusEntity> regionEntities)
    {
        var unprocessedIDs = SpriteNodes.ContainsKey(entityTypeKey) ? SpriteNodes[entityTypeKey]?.Keys.ToList() : new();
        
        foreach (var ludusEntity in regionEntities)
        {
            var key = ludusEntity.Def.Key;
            var id = ludusEntity.ID;

            // if (ludusEntity.GetComponent<AgeComponent>().IsExpired)
            // {
            //     // don't add this entity to the processed IDs list as we want it to be removed later
            //     continue;
            // }
            
            if (SpriteNodes.ContainsKey(key))
            {
                if (SpriteNodes[key].ContainsKey(id))
                {
                    unprocessedIDs.Remove(id);
                    SpriteNodes[key][id].UpdateSprite();
                    continue;
                }
            }
            
            // new entity to add
            if (!EntitiesToAdd.ContainsKey(texture))
                EntitiesToAdd.Add(texture, new List<LudusEntity>());
            
            EntitiesToAdd[texture].Add(ludusEntity);
        }

        if (SpriteNodes.ContainsKey(entityTypeKey))
        {
            var toBeRemoved = SpriteNodes[entityTypeKey].Keys.Where(w => unprocessedIDs.Contains(w)).ToList();
            EntitiesToRemove.Add(entityTypeKey, toBeRemoved);
        }

        SpritesReadyForRefresh = true;
    }
}