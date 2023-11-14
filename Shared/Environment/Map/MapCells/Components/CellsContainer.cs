using Bitspoke.Core.Common.Collections.Arrays;
using Bitspoke.Core.Common.Collections.Dictionaries;
using Bitspoke.Core.Common.Collections.Lists;
using Bitspoke.Core.Profiling;
using Bitspoke.Core.Utils.Primatives.Float;
using Bitspoke.GodotEngine.Common.Vector;
using Bitspoke.GodotEngine.Utils.Vector;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.MapCells.Components;

public class CellsContainer
{
    #region Properties
    // none
    public Map Map { get; set; }

    public BitspokeArray<BitspokeDictionary<int, MapCell>> CellsByRegion { get; set; }
    
    [JsonIgnore] public  BitspokeArray<MapCell> Cells { get; set; } //=> new (CellsByRegion.SelectMany(m => m));
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
        CellsByRegion = new BitspokeArray<BitspokeDictionary<int, MapCell>>(Map.TotalRegions);
        Cells = new BitspokeArray<MapCell>(Map.TotalCells);
        InitialiseCells();
    }

    private void InitialiseCells()
    {
        Profiler.Start();
        
        var mapID = Map.ID;
        
        for (int cellIndex = 0; cellIndex < Map.TotalCells; cellIndex++)
        {
            var cell = new MapCell(cellIndex, mapID);
            CellsByRegion[cell.RegionIndex] ??= new BitspokeDictionary<int, MapCell>();
                
            Cells[cellIndex] = cell;
            if (CellsByRegion[cell.RegionIndex] == null)
                CellsByRegion[cell.RegionIndex] = new BitspokeDictionary<int, MapCell>();
                
            CellsByRegion[cell.RegionIndex].Add(cellIndex, cell);    
        }
        
        // for (int regionIndex = 0; regionIndex < CellsByRegion.Length; regionIndex++)
        // {
        //     var region = Map.Data.RegionsContainer[regionIndex];
        //     var regionDimension = region.Dimension;
        //
        //     
        //
        //     
        //     
        //     
        //     // for (var x = regionDimension.Position.X; x < regionDimension.End.X; x++)
        //     // {
        //     //     for (var y = regionDimension.Position.Y; y < regionDimension.End.Y; y++)
        //     //     {
        //     //         var cellIndex = y * cellsPerRegion + x;
        //     //         var cell = new MapCell(cellIndex, mapID);
        //     //         
        //     //         Cells[cellIndex] = cell;
        //     //         if (CellsByRegion[regionIndex] == null)
        //     //             CellsByRegion[regionIndex] = new BitspokeDictionary<int, MapCell>();
        //     //     
        //     //         CellsByRegion[regionIndex].Add(cellIndex, cell);
        //     //     }
        //     // }
        //     
        //     // var startCellIndex = regionIndex * cellsPerRegion;
        //     // var maxCellIndexForRegion = Math.Min(startCellIndex + cellsPerRegion, Map.Area);
        //     // CellsByRegion[regionIndex] ??= new BitspokeDictionary<int, MapCell>();
        //     //
        //     // for (int cellIndex = startCellIndex; cellIndex < maxCellIndexForRegion; cellIndex++)
        //     // {
        //     //     var cell = new MapCell(cellIndex, mapID);
        //     //     // RegionIndex is set in the ctor
        //     //     //cell.RegionIndex = regionIndex;
        //     //
        //     //     Cells[cellIndex] = cell;
        //     //     if (CellsByRegion[regionIndex] == null)
        //     //         CellsByRegion[regionIndex] = new BitspokeDictionary<int, MapCell>();
        //     //     
        //     //     CellsByRegion[regionIndex].Add(cellIndex, cell);    
        //     // }
        // }
        
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