using Bitspoke.Core.Common.Collections.Dictionaries;
using Bitspoke.Core.Common.Collections.Lists;
using BitspokeEntitiesContainer = Bitspoke.Core.Entities.Containers.EntitiesContainer<Bitspoke.Ludus.Shared.Common.Entities.LudusEntity>;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells;

namespace Bitspoke.Ludus.Shared.Entities.Containers;

public class EntitiesContainer : BitspokeEntitiesContainer
{
    #region Properties

    private BitspokeDictionary<int, int> EntityCellMap { get; set; } = new();
    private BitspokeDictionary<int, int> EntityRegionMap { get; set; } = new();
    
    public BitspokeDictionary<EntityType, BitspokeList<LudusEntity>> EntitiesByType { get; set; } = new();
    
    public BitspokeDictionary<int, EntitiesContainer> EntitiesByRegion { get; set; } = new();
    public BitspokeDictionary<EntityType, BitspokeDictionary<int, EntitiesContainer?>> EntitiesByTypeAndRegion { get; set; } = new();
    
    #endregion

    #region Constructors and Initialisation
    // none
    #endregion

    #region Methods

    
    public void Add(LudusEntity entity, MapCell? cell)
    {
        // add the entity to the full list of entities
        base.Add(entity);
        
        AddEntityByType(entity, cell);
        AddEntityToCell(entity, cell);
    }

    private void AddEntityByType(LudusEntity entity, MapCell? cell = null)
    {
        // add to entities by type collection
        var entityType = entity.Def.Type;
        if (!EntitiesByType.ContainsKey(entityType))
            EntitiesByType.Add(entityType, new BitspokeList<LudusEntity>());
        
        EntitiesByType[entityType].Add(entity);

        if (cell != null)
        {
            if (!EntitiesByTypeAndRegion.ContainsKey(entityType))
                EntitiesByTypeAndRegion.Add(entityType, new BitspokeDictionary<int, EntitiesContainer?>());

            var regionIndex = cell.RegionIndex;
            if (!EntitiesByTypeAndRegion[entityType].ContainsKey(regionIndex))
                EntitiesByTypeAndRegion[entityType].Add(regionIndex, new EntitiesContainer());
            
            EntitiesByTypeAndRegion[entityType][regionIndex]?.Add(entity);
        }
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
        if (entity is ISpawnableEntity)
        {
            var canSpawn = ((ISpawnableEntity)entity).GetSpawnSystem(cell.MapID).CanSpawnAt(cell);
        
            if (!canSpawn)
                return;
        }
        
        // we are either a non-spawnable entity OR we can spawn here
        // so proceed to add us
        EntityCellMap.Add(entity.ID, cell.Index);
        
        if (cell.EntitiesNew == null)
            cell.EntitiesNew = new EntitiesContainer();
        
        cell.EntitiesNew?.Add(entity);
        
        // now add it to the region ... no spawn check needed as we did it above
        EntityRegionMap.Add(entity.ID, cell.RegionIndex);
        
        if (!EntitiesByRegion.ContainsKey(cell.RegionIndex))
            EntitiesByRegion.Add(cell.RegionIndex, new EntitiesContainer());
        
        EntitiesByRegion[cell.RegionIndex].Add(entity);

    }

    #endregion

    
}