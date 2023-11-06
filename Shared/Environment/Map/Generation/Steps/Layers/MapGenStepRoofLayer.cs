using Bitspoke.Core.Common.Grids;
using Bitspoke.Core.Common.Vector;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation.Layers;

namespace Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Layers;

public class MapGenStepRoofLayer : MapGenStep
{
    #region Properties

    public override string StepName => nameof(MapGenStepRoofLayer);

    #endregion

    #region Constructors and Initialisation

    public MapGenStepRoofLayer(Map map, MapGenStepDef mapGenStepDef) : base(map, mapGenStepDef) { }
    
    #endregion

    #region Methods

    protected override void StepGenerate()
    {
        Profiler.Start();
        var minElevation = ((MapGenStepRoofLayerDef)MapGenStepDef).MinElevation ?? 0.61f * 1.1f;

        foreach (var mapCell in Map.Cells.All)
        {
            var cellElevation = mapCell.Elevation;
            if (cellElevation <= minElevation)
                continue;
            
            // TODO: Should this be using a collection of RoofDefs rather than just getting the normal one??
            mapCell.RoofDef = Find.DB.RoofDefs["normal"].Clone();
            mapCell.RoofDef.Index = mapCell.Index;
            mapCell.Values.Add("RoofDef", mapCell.RoofDefKey);
        }
        Profiler.End();

        RemoveSmallPatches();
    }

    private void RemoveSmallPatches()
    {
        Profiler.Start();
        //Log.Debug($"Total Cells with Roofs Pre FloodFill: {Map.Cells.RoofDefs.Count}");
     
        var minRoofSize = ((MapGenStepRoofLayerDef) MapGenStepDef).MinRoofSize ?? 20;
        
        //var floodFiller = new FloodFiller();
        var floodFiller = new MapFloodFiller(Map);

        var visited = new SimpleGrid<bool>(Map.Area);
        var roofPatch = new List<int>();
        
        var debugTotalFloodFillTime = 0d;
        var debugTotalFloodFills = 0;
        
        foreach (var mapCell in Map.Cells.Ordered.Values)
        {
            roofPatch.Clear();
            var mapCellIndex = mapCell.Index;
            var isNaturalRoof = mapCell.RoofDef?.IsNatural ?? false;
            if (!(visited[mapCellIndex]) && isNaturalRoof)
            {
                Profiler.Start(additionalKey:"FloodFill");
                
                // floodFiller.StackFloodFill(
                //     mapCellIndex, 
                //     Validator,
                //     x => { visited[x] = true; roofPatch.Add(x); },
                //     Neighbours);
                
                
                floodFiller.FloodFill(
                    mapCell, 
                    Validator,
                    x => { var index = x.ToIndex(Map.Width); visited[index] = true; roofPatch.Add(index); }
                );
                
                debugTotalFloodFillTime += Profiler.End(log:false, additionalKey:"FloodFill");
                debugTotalFloodFills++;
            }

            if (roofPatch.Count > 0)
            {
                if (roofPatch.Count < minRoofSize)
                {
                    foreach (var index in roofPatch)
                    {
                        Map.Cells.Ordered[index].RoofDef = null;
                    }

                    //Log.Debug($"Removing Roof Patch with {roofPatch.Count} cells.");
                }
                //else
                //    Log.Debug($"Leaving Roof Patch with {roofPatch.Count} cells.");
            }
        }
        
        //Log.Debug($"Total Cells with Roofs Post FloodFill: {Map.Cells.RoofDefs.Count}");
        Profiler.End();
        
        if (CoreGlobal.DEBUG_ENABLED)
        {
            Log.Warning($"Total Processing time for MapFloodFiller.FloodFill: {debugTotalFloodFillTime}ms of {debugTotalFloodFills} flood fills @ {debugTotalFloodFillTime/debugTotalFloodFills}ms/fill");
            Log.Warning($"Total Processing time for MapFloodFiller.FloodFill.Clear: {floodFiller.debugTotalClearTime}ms of {floodFiller.debugTotalClears} clears @ {floodFiller.debugTotalClearTime/floodFiller.debugTotalClears}ms/clear");
        }
    }

    private int[] Neighbours(int index)
    {
        return Map.Cells.NeighbourMatrix[index].Where(w => w != null).Select(s =>  s.Index).ToArray();
    }

    private void Processor(int index)
    {
        
    }

    private bool Validator(int index)
    {
        return Map.Cells.Ordered[index].RoofDef?.IsNatural ?? false;
    }
    
    private bool Validator(Vec2Int cellLocation)
    {
        var index = cellLocation.ToIndex(Map.Width);
        return Map.Cells.Ordered[index].RoofDef?.IsNatural ?? false;
    }

    #endregion


    

    
    
}