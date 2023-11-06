using Bitspoke.Core.Definitions.Parts.Common;
using Bitspoke.Core.Entities.Containers;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;
using Bitspoke.Ludus.Shared.Entities.Natural.Plants;

namespace Bitspoke.Ludus.Shared.Entities.Containers.Extensions;

public static class EntityContainerPlantExtensions
{
    public static bool HasPlant(this EntitiesContainer<LudusEntity> container)
    {
        return container.EntitiesList.HasPlant();
    }
    
    public static bool HasPlant(this List<LudusEntity> container)
    {
        return container
            .Select(s => s)
            .Any(a => a.Def.Type == EntityType.Plant);
    }
    
    public static int PlantCount(this EntitiesContainer<LudusEntity> container)
    {
        return container.EntitiesList.PlantCount();
    }
    
    public static int PlantCount(this List<LudusEntity> list)
    {
        return list
            .Select(s => s)
            .Count(a => a.Def.Type == EntityType.Plant);
    }
    
    public static List<LudusEntity>? WildPlants(this EntitiesContainer<LudusEntity> container)
    {
        return container.EntitiesList.WildPlants();
    }
    
    public static List<LudusEntity>? WildPlants(this List<LudusEntity> list)
    {
        return list
            .Select(s => s).Cast<LudusEntity>()
            .Where(w => w.Def.Type == EntityType.Plant && ((PlantDef) w.Def).IsWild)
            .ToList();
    }
    
    public static List<LudusEntity>? Clustered(this List<LudusEntity> entityList)
    {
        return entityList.Where(s => s.Def.HasDefComponent<ClusterDef>()).ToList();
    }

    public static Dictionary<PlantDef, List<Plant>> PlantsByDef(this EntitiesContainer<Plant> plants)
    {
        return plants.OrderedByDef<Plant, PlantDef>();
    }

    public static List<PlantDef>? PlantDefs(this EntitiesContainer<LudusEntity> container, bool isWild = true, bool canCluster = false)
    {
        return  container.EntitiesList.PlantDefs(isWild, canCluster);
    }
    
    public static List<PlantDef>? PlantDefs(this List<LudusEntity> list)
    {
        return list
            .Where(w => w.Def.Type == EntityType.Plant)
            .Select(s => (PlantDef) s.Def)
            .ToList();
    }
    
    public static List<PlantDef>? PlantDefs(this List<LudusEntity> list, bool isWild = true, bool canCluster = false)
    {
        return list
            .Where(w => w.Def.Type == EntityType.Plant 
                        && ((PlantDef) w.Def).IsWild == isWild 
                        && ((PlantDef) w.Def).CanCluster == canCluster)
            .Select(s => (PlantDef) s.Def)
            .ToList();
    }
    
    

}