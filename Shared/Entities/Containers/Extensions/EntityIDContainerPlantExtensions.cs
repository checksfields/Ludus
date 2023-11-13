using Bitspoke.Core.Definitions.Parts.Common;
using Bitspoke.Core.Entities.Containers;
using Bitspoke.Core.Profiling;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;
using Bitspoke.Ludus.Shared.Entities.Natural.Plants;
using Bitspoke.Ludus.Shared.Environment.Map;

namespace Bitspoke.Ludus.Shared.Entities.Containers.Extensions;

public static class EntityIDContainerPlantExtensions
{
    public static bool HasPlant(this EntityIDContainer<EntityType> container)
    {
        return container.ContainsValue(EntityType.Plant);
    }
    
    public static int PlantCount(this EntityIDContainer<EntityType> container)
    {
        return container.Count(w => w.Value == EntityType.Plant);
    }

    // public static List<LudusEntity>? EntityList(this EntityIDContainer<EntityType> container, Map? map = null, MapCell? mapCell = null)
    // {
    //     Profiler.Start();
    //     if (map == null)
    //         map = Find.CurrentMap;
    //
    //     //var entities = map.AllCommonEntities.Where(w => container.ContainsKey (w.IDComponent.ID)).ToList();
    //     var entities = map.Entities.GetByCellID(mapCell.Index);//.Where(w => container.ContainsKey (w.IDComponent.ID)).ToList();
    //     Profiler.End();
    //     return entities?.EntitiesList ?? null;
    // }

    public static EntitiesContainer<LudusEntity> Entities(this EntityIDContainer<EntityType> container, Map? map = null)
    {
        //Profiler.Start();
        var entityContainer = new EntitiesContainer<LudusEntity>(container.MaxSize);
        foreach (var entityID in container)
        {
            var entity = ((EntitiesContainer<LudusEntity>) map.CommonEntities[entityID.Value])[entityID.Key];
            entityContainer.Add(entity);
        }

        //Profiler.End();

        return entityContainer;
    }


    public static List<Plant>? Plants(this EntityIDContainer<EntityType> container, Map? map = null)
    {
        Profiler.Start();
        if (map == null)
            map = Find.CurrentMap;

        var entities = map.Plants.EntitiesList.Where(w => container.ContainsKey(w.IDComponent.ID) && container.ContainsValue(EntityType.Plant)).ToList();
        Profiler.End();
        return entities;
    }
    
    public static List<Plant>? WildPlants(this EntityIDContainer<EntityType> container, Map? map = null)
    {
        Profiler.Start();
        var plants = container.Plants(map)?.Where(s => ((PlantDef)s.Def).IsWild).ToList();
        Profiler.End();
        return plants;
    }
    
    public static List<LudusEntity>? Clustered(this List<LudusEntity> entityList)
    {
        return entityList.Where(s => s.Def.HasDefComponent<ClusterDef>()).ToList();
    }

    public static Dictionary<PlantDef, List<Plant>> PlantsByDef(this EntityIDContainer<EntityType> container, Map? map = null)
    {
        var plants = container.Plants(map);
        return plants?.OrderedByDef()!;
    }

    public static Dictionary<PlantDef, List<Plant>> OrderedByDef(this List<Plant>? entitiesList)
    {
        var result = entitiesList
            .GroupBy(g => g.Def)
            .ToDictionary(d => (PlantDef) d.Key, d => d.ToList());

        return result;
    }
    


}