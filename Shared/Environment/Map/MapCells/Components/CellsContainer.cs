using System.Linq;
using Bitspoke.Core.Common.Collections.Arrays;
using Bitspoke.Core.Common.Collections.Dictionaries;
using Bitspoke.Core.Common.Collections.Lists;
using Bitspoke.Core.Utils.Primatives.Float;
using Bitspoke.GodotEngine.Common.Vector;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.MapCells.Components;

public class CellsContainer
{
    #region Properties
    // none
    public Map Map { get; set; }

    public BitspokeArray<BitspokeList<MapCell>> CellsByRegion { get; set; }
    
    [JsonIgnore] public  BitspokeArray<MapCell> Cells => new (CellsByRegion.SelectMany(m => m));
    [JsonIgnore] private Dictionary<int, BitspokeDictionary<int, BitspokeArray<MapCell>>> CellBucketsCache { get; set; } = new ();
    
    public MapCell? this[int index] => Cells?[index] ?? null;
    
    #endregion

    #region Constructors and Initialisation

    public CellsContainer(int mapID) : this(Find.Map(mapID)) { }
    
    public CellsContainer(Map map)
    {
        Map = map;
        Init();
    }

    protected void Init()
    {
        CellsByRegion = new BitspokeArray<BitspokeList<MapCell>>(Map.TotalRegions);
        InitialiseCells();
    }

    private void InitialiseCells()
    {
        Profiler.Start();
        var mapID = Map.ID;
        var cellsPerRegion = Global.DEFAULT_MAP_REGION_DIMENSIONS.Area();

        for (int regionIndex = 0; regionIndex < CellsByRegion.Length; regionIndex++)
        {
            
            var startCellIndex = regionIndex * cellsPerRegion;
            var maxCellIndexForRegion = Math.Min(startCellIndex + cellsPerRegion, Map.Area);
            CellsByRegion[regionIndex] ??= new BitspokeList<MapCell>();

            for (int cellIndex = startCellIndex; cellIndex < maxCellIndexForRegion; cellIndex++)
            {
                var cell = new MapCell(cellIndex, mapID);
                cell.RegionIndex = regionIndex;
                
                CellsByRegion[regionIndex].Add(cell);    
            }
        }
        
        // preload some common data queries
        GetCellBucket(GodotGlobal.CPU_CORES);
        
        Profiler.End();
    }

    #endregion

    #region Methods
    
    public BitspokeDictionary<int, BitspokeArray<MapCell>> GetCellBucket(int numberOfBuckets)
    {
        Profiler.Start();

        if (!CellBucketsCache.ContainsKey(numberOfBuckets))
        {
            var buckets = new BitspokeDictionary<int, BitspokeArray<MapCell>>();
            var cellsPerBucket = (Cells.Length / (float)numberOfBuckets).Ceiling();
            for (int i = 0; i < numberOfBuckets; i++)
            {
                var length = Math.Min(cellsPerBucket, Map.Area - (i * cellsPerBucket));
                var slice = Cells.Array.Slice(i * cellsPerBucket, length);
                buckets.Add(i, new BitspokeArray<MapCell>(slice));
            }

            CellBucketsCache.Add(numberOfBuckets, buckets);
        }

        Profiler.End();
        
        return CellBucketsCache[numberOfBuckets];
    }
    
    #endregion
    
}