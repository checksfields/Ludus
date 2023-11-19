using Bitspoke.Core.Common.Collections.Arrays;
using Bitspoke.Core.Common.Collections.Dictionaries;
using Bitspoke.Core.Common.Collections.Matrices;
using Bitspoke.Core.Common.Direction;
using Bitspoke.Core.Profiling;
using Bitspoke.Core.Utils.Primatives.Float;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Terrain;
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
    [JsonIgnore] public CellNeighbourMatrix NeighbourMatrix { get; protected set; }
    
    public MapCell? this[int index] => Cells?[index] ?? null;
    
    #endregion

    #region Constructors and Initialisation
    
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

        Profile(() => {
            // generate the neighbour cell matrix
            NeighbourMatrix = new CellNeighbourMatrix();
            foreach (var mapCell in Cells)
            {
                NeighbourMatrix.AddOrUpdate(mapCell.Index, GetAllNeighbourCellsFor(mapCell));
            }
        });
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

            var bucketsArray = Cells.Array.Slice(numberOfBuckets);
            for (int i = 0; i < numberOfBuckets; i++)
                buckets.Add(i, new BitspokeArray<MapCell>(bucketsArray[i]));
            
            CellBucketsCache.Add(numberOfBuckets, buckets);
        }

        Profiler.End();
        
        return CellBucketsCache[numberOfBuckets];
    }

    public MapCell[] GetAllNeighbourCellsFor(MapCell cell, bool addUpdateMatrix = true)
    {
        var neighbours = new MapCell[8];

        // north
        var nLoc = cell.Location + Cardinal.NORTH;
        var nIdx = nLoc.y >= 0 ? nLoc.ToIndex(Map.Width) : -1;
        if (nIdx >= 0) neighbours[1] = Cells[nIdx];

        // east
        var eLoc = cell.Location + Cardinal.EAST;
        var eIdx = eLoc.x <= Map.Width - 1 ? eLoc.ToIndex(Map.Width) : -1;
        if (eIdx >= 0) neighbours[3] = Cells[eIdx];

        // south
        var sLoc = cell.Location + Cardinal.SOUTH;
        var sIdx = sLoc.y <= Map.Height - 1 ? sLoc.ToIndex(Map.Width) : -1;
        if (sIdx >= 0) neighbours[5] = Cells[sIdx];

        // west
        var wLoc = cell.Location + Cardinal.WEST;
        var wIdx = wLoc.x >= 0 ? wLoc.ToIndex(Map.Width) : -1;
        if (wIdx >= 0) neighbours[7] = Cells[wIdx];

        // north-west
        if (nIdx >= 0 && wIdx >= 0) neighbours[0] = Cells[nIdx + 1];

        // north-east
        if (nIdx >= 0 && eIdx >= 0) neighbours[2] = Cells[nIdx + 1];

        // south-east
        if (sIdx >= 0 && eIdx >= 0) neighbours[4] = Cells[sIdx + 1];

        // south-west
        if (sIdx >= 0 && wIdx >= 0) neighbours[6] = Cells[sIdx - 1];

        if (addUpdateMatrix)
            NeighbourMatrix.AddOrUpdate(cell.Index, neighbours);


        return neighbours;
    }

    [JsonIgnore] public List<TerrainDef?> TerrainDefs => Cells.Array
        .Select(s => s)
        .Where(w => w.TerrainDef != null)
        .Select(s2 => s2.TerrainDef)
        .ToList();
    
    #endregion
    
}