using Bitspoke.Core.Common.Collections;
using Bitspoke.Core.Common.Vector;
using Bitspoke.Core.Components.Identity;
using Bitspoke.Core.Entities.Containers;
using Bitspoke.Core.Models.Cells;
using Bitspoke.Core.Utils.Primatives.Int;
using Bitspoke.Ludus.Shared.Common.Components;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Entities.Components;


[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
[Obsolete("Deprecated. Use Map.Data.EntityContainer instead.", true)]
public class MapEntityContainerComponent : LudusComponent
{
    #region Properties

    public override string ComponentName => nameof(MapEntityContainerComponent);

    public ulong MapID { get; set; }
    private Map Map => MapID.FindMap();
    
    private Dictionary<EntityType, List<LudusEntity>> EntitiesByType { get; set; }
    private BucketCollection<int, EntitiesContainer<LudusEntity>> EntitiesByRegion { get; set; }
    private BucketCollection<int, EntitiesContainer<LudusEntity>> EntitiesByCellID { get; set; }
    private BucketCollection<ulong, LudusEntity> Entities  { get; set; }

    private Dictionary<Vec2Int, int> PositionCellIndexMap { get; set; }

    #endregion

    #region Constructors and Initialisation

    public MapEntityContainerComponent(IDComponent mapID) : base()
    {
        MapID = mapID.ID.Value;
        InternalInit();
    }

    public void InternalInit()
    {
        var map = Map;
        var buckets = map.Regions.MapRegions.Count;

        Entities = new BucketCollection<ulong, LudusEntity>(buckets);
        EntitiesByType = new Dictionary<EntityType, List<LudusEntity>>();
        EntitiesByRegion = new BucketCollection<int, EntitiesContainer<LudusEntity>>(map.Regions.Area, bucketKeyCalculator: k =>
        {
            var pos = k.GetHashCode().ToVec2Int(map.Width);
            var regionWidth = map.Regions.RegionSize.x;
            var regionCellWidth = map.Width / regionWidth;
            var xRegionIndex = (pos.x) / regionWidth;
            var yRegionIndex = ((pos.y) / regionWidth) * regionCellWidth;
            var regionIndex = xRegionIndex + yRegionIndex;
            return regionIndex;
        });
        EntitiesByCellID = new BucketCollection<int, EntitiesContainer<LudusEntity>>(buckets);

        PositionCellIndexMap = new Dictionary<Vec2Int, int>();
    }

    #endregion

    #region Methods

    public void Add(LudusEntity entity, MapCell cell)
    {
        if (entity == null)
        {
            Log.Exception($"Cannot add a null entity", -9999999);
            return;
        }


        if (cell == null)
        {
            // TODO: Check that this is an exception once development complete ... can we have entities not in a cell?
            Log.Exception($"Cannot add an entity without an associated MapCell", -9999999);
            return;
        }

        // BY ID
        var id = entity.ID;
        Entities.Add(id, entity);
        
        // BY CELL
        // is the bucket created
        var bucket = EntitiesByCellID.GetBucketByKey(cell.Index);
        if (bucket == null)
        {
            EntitiesByCellID.Add(cell.Index, new EntitiesContainer<LudusEntity>());
            bucket = EntitiesByCellID.GetBucketByKey(cell.Index);
        }
        
        // is there is no container for this cell in the bucket?
        if (!bucket.ContainsKey(cell.Index))
        {
            bucket.Add(cell.Index, new EntitiesContainer<LudusEntity>());    
        }
        bucket[cell.Index].Add(entity);
        PositionCellIndexMap.Add(cell.Location, cell.Index);
        

        // BY DEF
        var entityType = entity.Def.Type;
        if (!EntitiesByType.ContainsKey(entityType))
        {
            EntitiesByType.Add(entityType, new List<LudusEntity>());
        }
        EntitiesByType[entityType].Add(entity);
        
        // BY REGION
        var regionBucket = EntitiesByRegion.GetBucketByKey(cell.Index);
        if (regionBucket == null)
        {
            EntitiesByRegion.Add(cell.Index, new EntitiesContainer<LudusEntity>());
            regionBucket = EntitiesByRegion.GetBucketByKey(cell.Index);
        }
        
        if (!regionBucket.ContainsKey(cell.Index))
        {
            regionBucket.Add(cell.Index, new EntitiesContainer<LudusEntity>());    
        }
        regionBucket[cell.Index].Add(entity);
    }

    #region By Type

    public List<LudusEntity> Get(EntityType entityType)
    {
        return EntitiesByType[entityType];
    }

    #endregion

    #region By Cell

    public EntitiesContainer<LudusEntity>? GetByCell(Cell cell)
    {
        return GetByCellIndex(cell.Index);
    }
    
    public EntitiesContainer<LudusEntity>? GetByCellIndex(int mapCellIndex)
    {
        var bucketKey = EntitiesByCellID.GetBucketKey(mapCellIndex);
        
        return (EntitiesByCellID.Collection.ContainsKey(bucketKey) ? EntitiesByCellID[mapCellIndex] : null) ?? null;
    }
    
    public EntitiesContainer<LudusEntity>? GetByCellLocation(Vec2Int mapCellLocation)
    {
        return GetByCellIndex(mapCellLocation.ToIndex(Map.Width));
    }

    #endregion

    #region By Region

    public List<LudusEntity> GetByRegion(int regionIndex)
    {
        return EntitiesByRegion.GetBucketByBucketKey(regionIndex).Values.SelectMany(s => s.EntitiesList).ToList();
    }

    #endregion
    
    
    #endregion
}