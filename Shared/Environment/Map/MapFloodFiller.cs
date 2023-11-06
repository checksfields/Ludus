using Bitspoke.Core.Common.Collections.Node;
using Bitspoke.Core.Common.Direction;
using Bitspoke.Core.Common.Vector;
using Bitspoke.Core.Utils.Primatives.Int;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells;

namespace Bitspoke.Ludus.Shared.Environment.Map;

public class MapFloodFiller : FloodFiller
{
    #region Properties

    public Map Map { get; set; }
    private int MapArea { get; set; }
    private int[] TraversalDistance { get; set; }
    private List<int> Visited { get; set; }
    private Dictionary<int, Vec2Int> Parents { get; set; }
    private Queue<Vec2Int> ToProcess { get; set; }

    private bool IsWorkingLock { get; set; } = false;
    
    #endregion

    #region Constructors and Initialisation

    public MapFloodFiller(Map map) : base()
    {
        Map = map;
        MapArea = Map.Area;
        Visited = new List<int>();
        TraversalDistance = new int[MapArea];
        TraversalDistance.Clear(-1);
        Parents = new Dictionary<int, Vec2Int>();
        ToProcess = new Queue<Vec2Int>();
    }
    
    #endregion

    #region Methods

    public void FloodFill(MapCell start, 
                          Predicate<Vec2Int> checker, 
                          Action<Vec2Int> processor, 
                          int maxCellsToProcess = int.MaxValue, 
                          bool rememberParents = false, 
                          IEnumerable<Vec2Int> extraStarts = null)
    {
        FloodFill(start, 
                  checker, 
                  (Func<Vec2Int, int, bool>) ((mapCell, TraversalDistance) => { processor(mapCell); return false; }),
                  maxCellsToProcess, 
                  rememberParents, 
                  extraStarts);
         


    }
    
    public void FloodFill(MapCell start, 
        Predicate<Vec2Int> checker, 
        Func<Vec2Int, int, bool> processor, 
        int maxCellsToProcess = int.MaxValue, 
        bool rememberParents = false, 
        IEnumerable<Vec2Int> extraStarts = null)
    {
        if (IsWorkingLock)
            Log.Error("Cannot execute parallel FloodFill calls", -9999999);

        // lock this filler
        IsWorkingLock = true;

        Clear();

        if (rememberParents && Parents == null)
            Parents = new Dictionary<int, Vec2Int>();
        
        if (start.IsValid && extraStarts == null && !checker(start.Location))
        {
            if (rememberParents)
                Parents[start.Index] = Vec2Int.DEFAULT;

            IsWorkingLock = false;
        }
        else
        {
            InnerFloodFill(start, checker, processor, rememberParents, extraStarts);
            Process(checker, processor, maxCellsToProcess);
        }

        IsWorkingLock = false;
    }

    private void InnerFloodFill(MapCell start, 
                                Predicate<Vec2Int> checker, 
                                Func<Vec2Int, int, bool> processor,
                                bool rememberParents = false, 
                                IEnumerable<Vec2Int> extraStarts = null)
    {
        ToProcess.Clear();

        if (start.IsValid)
            AddToFill(start);

        if (extraStarts != null)
        {
            foreach (var extraStart in extraStarts)
                AddToFill(extraStart);
        }

        if (rememberParents)
        {
            for (var i = 0; i < Visited.Count; i++)
            {
                var location = Visited[i].ToVec2Int(MapArea);
                Parents[Visited[i]] = checker(location) ? location : Vec2Int.DEFAULT;
            }
        }
    }

    private void Process(Predicate<Vec2Int> checker, 
                         Func<Vec2Int, int, bool> processor, 
                         int maxCellsToProcess = int.MaxValue,
                         bool rememberParents = false)
    {
        var cellsProcessed = 0;
        while (ToProcess.Count > 0)
        {
            var location = ToProcess.Dequeue();
            var traversalDistance = TraversalDistance[location.ToIndex(Map.Width)];
            if (!processor(location, traversalDistance))
            {
                ++cellsProcessed;
                if (cellsProcessed != maxCellsToProcess)
                {
                    for (var i = 0; i < Cardinal.CardinalDirections.Count; i++)
                    {
                        var neighbourCell = location + Cardinal.CardinalDirections[i];
                        var neighbourCellIndex = neighbourCell.ToIndex(Map.Width);
                        
                        if (neighbourCell.IsInBounds(Map.Width) && TraversalDistance[neighbourCellIndex] == -1 && checker(neighbourCell))
                        {
                            AddToFill(neighbourCell, traversalDistance, rememberParents, location);
                        }
                    }

                    if (ToProcess.Count > MapArea)
                    {
                        Log.Error(
                            $"We cannot have more cells to process [{ToProcess.Count}], than the area of the Map [{MapArea}]",
                            -9999999);
                        IsWorkingLock = false;
                        return;
                    }
                }
                else
                    break;
            }
            else
                break;
        }

        IsWorkingLock = false;
    }

    private void AddToFill(MapCell mapCell)
    {
        var index = mapCell.Index;
        Visited.Add(index);
        TraversalDistance[index] = 0;
        ToProcess.Enqueue(mapCell.Location);
    }
    
    private void AddToFill(Vec2Int location)
    {
        var index = location.ToIndex(Map.Width);
        Visited.Add(index);
        TraversalDistance[index] = 0;
        ToProcess.Enqueue(location);
    }
    
    private void AddToFill(Vec2Int location, int traversalDistance, bool rememberParents = false, Vec2Int parent = null)
    {
        var index = location.ToIndex(Map.Width);
        Visited.Add(index);
        TraversalDistance[index] = traversalDistance + 1;
        ToProcess.Enqueue(location);
        if (rememberParents)
            Parents[index] = parent;
    }

    public double debugTotalClearTime = 0d;
    public int debugTotalClears = 0;
    
    private void Clear()
    {
        Profiler.Start();

        for (int index = 0; index < Visited.Count; index++)
        {
            var value = Visited[index];
            TraversalDistance[value] = -1;
            if (Parents != null)
            {
                Parents[value] = Vec2Int.DEFAULT;
            }
        }

        Visited.Clear();
        ToProcess.Clear();
                
        debugTotalClears++;
        debugTotalClearTime += Profiler.End(log:false);

        
    }

    #endregion

    
}