using Bitspoke.Core.Common.Collections.Arrays;
using Bitspoke.Core.Common.Collections.Dictionaries;
using Bitspoke.Core.Definitions;
using BitspokeEntitiesContainer = Bitspoke.Core.Entities.Containers.EntitiesContainer<Bitspoke.Ludus.Shared.Common.Entities.LudusEntity>;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Common.Entities.Collections;
using Bitspoke.Ludus.Shared.Environment.Map;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells;

namespace Bitspoke.Ludus.Shared.Entities.Containers;

public class EntitiesContainer : BitspokeEntitiesContainer
{
    #region Properties

    public Map Map { get; set; }
    
    private BitspokeDictionary<ulong, int> EntityCellMap { get; set; } = new();
    private BitspokeDictionary<ulong, int> EntityRegionMap { get; set; } = new();
    
    public BitspokeDictionary<EntityType, LudusEntityList> EntitiesByType { get; set; } = new();
    public BitspokeDictionary<EntityType, BitspokeDictionary<Def, LudusEntityList>> EntitiesByTypeAndDef { get; set; } = new();
    
    public BitspokeArray<LudusEntityList> EntitiesByRegion { get; set; }
    //public BitspokeDictionary<int, EntitiesContainer> EntitiesByRegion { get; set; } = new();
    public BitspokeDictionary<EntityType, BitspokeDictionary<int, LudusEntityList>> EntitiesByTypeAndRegion { get; set; } = new();
    
    #endregion

    #region Constructors and Initialisation
    public EntitiesContainer(Map map)
    {
        Map = map;
        Init();
    }

    protected void Init()
    {
        EntitiesByRegion = new BitspokeArray<LudusEntityList>(Map.TotalRegions);
        for (int i = 0; i < EntitiesByRegion.Length; i++)
        {
            EntitiesByRegion[i] = new LudusEntityList();
        }    
    }
    
    #endregion

    #region Methods

    
    public void Add(LudusEntity entity, MapCell? cell)
    {
        // Benchmark Testing = 0 ms 
        //Profile(() => { 
        // add the entity to the full list of entities
        base.Add(entity);
        
        AddEntityByType(entity, cell);
        AddEntityToCell(entity, cell);

        AddEntityByTypeAndDef(entity);
        //});
    }

    private void AddEntityByType(LudusEntity entity, MapCell? cell = null)
    {
        // add to entities by type collection
        var entityType = entity.Def.Type;
        if (!EntitiesByType.ContainsKey(entityType))
            EntitiesByType.Add(entityType, new LudusEntityList());
        
        EntitiesByType[entityType].Add(entity);

        if (cell != null)
        {
            if (!EntitiesByTypeAndRegion.ContainsKey(entityType))
                EntitiesByTypeAndRegion.Add(entityType, new BitspokeDictionary<int, LudusEntityList>());

            var regionIndex = cell.RegionIndex;
            if (!EntitiesByTypeAndRegion[entityType].ContainsKey(regionIndex))
                EntitiesByTypeAndRegion[entityType].Add(regionIndex, new LudusEntityList());
            
            EntitiesByTypeAndRegion[entityType][regionIndex]?.Add(entity);
        }
    }
    
    private void AddEntityByTypeAndDef(LudusEntity entity)
    {
        var entityDef = entity.Def;
        var entityType = entityDef.Type;

        // if we don't have a type key then add the dictionary to the collection
        if (!EntitiesByTypeAndDef.ContainsKey(entityType))
            EntitiesByTypeAndDef.Add(entityType, new());
        
        // if we don't have a def key for the entity type then add the list to the collection for the entity type
        if (!EntitiesByTypeAndDef[entityType].ContainsKey(entityDef))
            EntitiesByTypeAndDef[entityType].Add(entityDef, new());
        
        EntitiesByTypeAndDef[entityType][entityDef].Add(entity);
    }
    
    private void AddEntityToCell(LudusEntity entity, MapCell? cell = null)
    {
        if (cell == null)
            return;
        
        var entityID = entity.ID;
        if (cell == null)
            Log.Exception($"Cannot add Entity[{entityID}] to Cell because the Cell does not exist", -9999999);
        
        // ... a cell CANNOT exist without a region
        if (cell.Region == null)
            Log.Exception($"Cannot add Entity[{entityID}] to Cell because the Cell does not have an associate region", -9999999);
        
        // if the entity is spawnable ... then check if we can spawn here
        // if (entity is ISpawnableEntity)
        // {
        //     var canSpawn = ((ISpawnableEntity)entity).GetSpawnSystem(cell.MapID).CanSpawnAt(cell);
        //
        //     if (!canSpawn)
        //         return;
        // }
        
        // we are either a non-spawnable entity OR we can spawn here
        // so proceed to add us
        EntityCellMap.Add(entity.ID, cell.Index);
        
        if (cell.EntitiesNew == null)
            cell.EntitiesNew = new LudusEntityList();
        
        cell.EntitiesNew?.Add(entity);
        
        // now add it to the region ... no spawn check needed as we did it above
        EntityRegionMap.Add(entity.ID, cell.RegionIndex);
        
        if (EntitiesByRegion[cell.RegionIndex] == null)
            EntitiesByRegion[cell.RegionIndex] = new LudusEntityList();
        
        
        EntitiesByRegion[cell.RegionIndex].Add(entity);

    }

    #endregion

    
}