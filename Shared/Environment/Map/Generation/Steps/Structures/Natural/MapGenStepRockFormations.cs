using Bitspoke.Core.Common.Collections.Arrays;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation.Structures.Natural;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells;

namespace Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Structures.Natural;

public class MapGenStepRockFormations : MapGenStep
{
    #region Properties

    public override string StepName => nameof(MapGenStepRockFormations);
    
    #endregion

    #region Constructors and Initialisation

    public MapGenStepRockFormations(Map map, MapGenStepDef mapGenStepDef) : base(map, mapGenStepDef) { }
    
    #endregion

    #region Methods

    protected override void StepGenerate()
    {
        Profiler.Start();
        
        ProcessCells();
        ProcessCellsOld();
        
        Profiler.End();
    }

    private void ProcessCells()
    {
        Profiler.Start();
        // TODO: Get default value (0.7f) from config
        var minElevation = ((MapGenStepRockFormationsDef)MapGenStepDef).MinElevation ?? 0.7f;
        var tasks = new List<Task>();   
        foreach (var cells in Map.Data.CellsContainer.CellsByRegion.Array)
        {
            tasks.Add(Task.Run(() =>
            {
                foreach (var mapCell in cells)
                {
                    var cellElevation = mapCell.Elevation;
                    if (cellElevation <= minElevation)
                        continue;
            
                    // TODO: Check other data layers????  Caves for example

                    // spawn the rock formation
                    mapCell.StructureDef = Find.DB.RockDefs[mapCell.Stratum].Clone();
                    mapCell.StructureDef.Index = mapCell.Index;
                }
                        
            }));
        }
        Task.WaitAll(tasks.ToArray());
        Profiler.End(message: "+++ NEW");
    }
    
    private void ProcessCellsOld(MapCell[]? cells = null)
    {
        Profiler.Start();

        cells ??= Map.Cells.All.ToArray();
        
        if (cells.IsNullOrEmpty())
        {
            Log.Exception($"MapCell[] cannot be null or empty");
            return;
        }
        
        // TODO: Get default value (0.7f) from config
        var minElevation = ((MapGenStepRockFormationsDef)MapGenStepDef).MinElevation ?? 0.7f;
        
        foreach (var mapCell in cells!)
        {
            var cellElevation = mapCell.Elevation;
            if (cellElevation <= minElevation)
                continue;
            
            // TODO: Check other data layers????  Caves for example

            // spawn the rock formation
            mapCell.StructureDef = Find.DB.RockDefs[mapCell.Stratum].Clone();
            mapCell.StructureDef.Index = mapCell.Index;
        }

        Profiler.End(message: "+++ OLD");
    }
    
    #endregion


    

    
    
}