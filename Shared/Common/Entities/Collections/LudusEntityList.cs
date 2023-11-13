using System;
using System.Collections;
using System.Collections.Generic;
using Bitspoke.Core.Common.Collections.Lists;
using Bitspoke.Core.Entities.Containers;
using Bitspoke.Ludus.Shared.Entities.Containers.Extensions;
using Bitspoke.Ludus.Shared.Entities.Definitions;
using Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;
using Bitspoke.Ludus.Shared.Entities.Natural.Plants;

namespace Bitspoke.Ludus.Shared.Common.Entities.Collections;

public class LudusEntityList : BitspokeList<LudusEntity>, IEnumerable
{
    #region Properties

    public int? MaxEntities { get; set; }
    public bool IsFull => Count >= MaxEntities;
    
    #endregion

    #region Constructors and Initialisation

    public LudusEntityList(int? maxEntities = null)
    {
        MaxEntities = maxEntities;
    }

    public LudusEntityList(List<LudusEntity> entities) : base(entities)
    {
        
    }
    
    #endregion

    #region Methods

    #endregion


    public bool HasPlant()
    {
        return this
            .Select(s => s)
            .Any(a => a.Def.Type == EntityType.Plant);
    }

    public List<PlantDef> PlantDefs()
    {
        throw new NotImplementedException();
    }
    
    public Dictionary<PlantDef, LudusEntityList> PlantsByDef()
    {
        return OrderedByDef<PlantDef>();
    }
    
    //public static List<PlantDef>? PlantDefs(this List<LudusEntity> list)
    public List<PlantDef>? PlantDefs(LudusEntityList list)
    {
        return list
            .Where(w => w.Def.Type == EntityType.Plant)
            .Select(s => (PlantDef) s.Def)
            .ToList();
    }
    
    public List<PlantDef>? PlantDefs(LudusEntityList list, bool isWild = true, bool canCluster = false)
    {
        return list
            .Where(w => w.Def.Type == EntityType.Plant 
                        && ((PlantDef) w.Def).IsWild == isWild 
                        && ((PlantDef) w.Def).CanCluster == canCluster)
            .Select(s => (PlantDef) s.Def)
            .ToList();
    }
    
    public Dictionary<T, LudusEntityList> ByDef<T>(LudusEntityList container) where T : EntityDef
    {
        var result = container
            .Select(s => s).Cast<LudusEntity>()
            .GroupBy(g => ((LudusEntity)g).Def)
            .ToDictionary(d => (T) d.Key, d => d.Cast<LudusEntityList>() as LudusEntityList );

        return result;
    }
    
    public Dictionary<TDef, LudusEntityList> OrderedByDef<TDef>() where TDef : EntityDef
    {
        var result = this
            .Select(s => s)
            .GroupBy(g => g.Def)
            .ToDictionary(d => (TDef) d.Key, d => new LudusEntityList(d.ToList()));

        return result;
    }
}