using Bitspoke.Core.Common.Collections.Lists;
using Bitspoke.GodotEngine.Common.Vector;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Common.States.Games;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells;
using Godot;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Regions;

public class Region
{
    #region Properties

    [JsonIgnore] public Map Map { get; set; }
    [JsonIgnore] public Rect2 Dimension { get; set; }
    [JsonIgnore] public int Index { get; set; }

    public List<MapCell>? cachedCells;
    public List<MapCell>? CachedCells {
        get {
            if (cachedCells == null)
                // @PERFORMANCE: about 12ms call
                cachedCells = Map.Cells.Ordered.Values.Where(s => Dimension.HasPoint(s.Location.ToVector2())).ToList();

            return cachedCells;
        }
    }

    // private List<LudusEntity>? CachedEntities { get; set; } = null; 
    // public List<LudusEntity>? Entities
    // {
    //     get => (CachedEntities == null || ((GameState)Find.GameStateManager.CurrentState) == GameState.MapGen) ? CachedEntities = Map.Entities.GetByRegion(Index) : CachedEntities;
    //     set => CachedEntities = value;
    // }

    private CachedList<LudusEntity>? entities { get; set; } = null; 
    public CachedList<LudusEntity>? Entities
    {
        get
        {
            if (entities == null) 
                entities = new();
            
            if (entities.IsDirty)
                entities.Values = Map.Entities.GetByRegion(Index);

            return entities;
        }
    }

    #endregion

    #region Constructors and Initialisation

    public Region(int index, Map map, Rect2 dimension)
    {
        Index = index;
        Map = map;
        Dimension = dimension;
    }
    
    #endregion

    #region Methods

    public void ClearAll()
    {
        ClearEntitiesCache();
    }
    
    public void ClearEntitiesCache()
    {
        Entities?.Clear();
        entities = null;
    }
    
    public List<LudusEntity> EntitiesBy(EntityType type)
    {
        var result = new List<LudusEntity>();
        foreach (var entity in Entities)
        //foreach (var entity in CachedEntities ?? Entities)
        {
            if (entity.Def.Type == type)
                result.Add(entity);
        }

        return result;
    }

    public Dictionary<string, List<LudusEntity>> PlantsByType()
    {
        var result = new Dictionary<string, List<LudusEntity>>();

        //var entities = Map.Entities.GetByRegion(Index);
        var entities = Map.Data.EntitiesContainer.EntitiesByRegion[Index];
        
        foreach (var entity in entities)
        {
            if (entity.Def.Type == EntityType.Plant)
            {
                var key = entity.Def.Key;
                if (!result.ContainsKey(key))
                    result.Add(key, new List<LudusEntity>());
            
                result[key].Add(entity);
            }
        }
    
        return result;
    }
    
    public Dictionary<string, List<LudusEntity>> PlantsByType_Old()
    {
        var result = new Dictionary<string, List<LudusEntity>>();
        
        
        //foreach (var entity in Entities)
        foreach (var entity in Map.Entities.GetByRegion(Index))
        {
            if (entity.Def.Type == EntityType.Plant)
            {
                var key = entity.Def.Key;
                if (!result.ContainsKey(key))
                    result.Add(key, new List<LudusEntity>());
            
                result[key].Add(entity);
            }
        }
    
        return result;
    }

    public int CachedBaseDesiredPlantsCountForTick { get; set; } = -999999;
    public double CachedBaseDesiredPlantsCount { get; set; }
    
    public double GetDesiredPlantsCount(bool allowCache = true)
    {
        int ticksGame = Find.Systems.TimeSystem.TimerTicks;
        if (allowCache && ticksGame - this.CachedBaseDesiredPlantsCountForTick < 2500)
            return this.CachedBaseDesiredPlantsCount;
        this.CachedBaseDesiredPlantsCount = 0.0f;
        Map map = this.Map;
        foreach (var mapCell in this.CachedCells)
        {
            this.CachedBaseDesiredPlantsCount += (double) mapCell.TerrainDef.Fertility;
        }
        
            
        this.CachedBaseDesiredPlantsCountForTick = ticksGame;
        return this.CachedBaseDesiredPlantsCount;
    
    }

    public Dictionary<string, List<MapCell>> TerrainsByType()
    {
        var result = new Dictionary<string, List<MapCell>>();
        foreach (var mapCell in CachedCells)
        {
            var key = mapCell.TerrainDef.Key;
            if (!result.ContainsKey(key))
                result.Add(key, new List<MapCell>());
        
            result[key].Add(mapCell);
        }
        
        return result;
    }

    #endregion
    
}