using Bitspoke.Core.Entities.Containers;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Entities.Definitions;

namespace Bitspoke.Ludus.Shared.Entities.Containers.Extensions;

public static class EntityContainerExtensions
{
    public static Dictionary<T, List<LudusEntity>> ByDef<T>(this GenericEntitiesContainer container) where T : EntityDef
    {
        var result = container.EntitiesList
            .Select(s => s).Cast<LudusEntity>()
            .GroupBy(g => ((LudusEntity)g).Def)
            .ToDictionary(d => (T) d.Key, d => d.ToList());

        return result;
    }
    
    public static Dictionary<TDef, List<T>> OrderedByDef<T, TDef>(this EntitiesContainer<T> container) where T : LudusEntity where TDef : EntityDef
    {
        var result = container.EntitiesList
            .Select(s => s)
            .GroupBy(g => g.Def)
            .ToDictionary(d => (TDef) d.Key, d => d.ToList());

        return result;
    }
    
    public static Dictionary<T, List<LudusEntity>> OrderedByDef<T>(this List<LudusEntity> entitiesList) where T : EntityDef
    {
        var result = entitiesList
            .GroupBy(g => g.Def)
            .ToDictionary(d => (T) d.Key, d => d.ToList());

        return result;
    }
    
   
    
}