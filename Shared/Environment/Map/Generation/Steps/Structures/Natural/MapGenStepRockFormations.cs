using Bitspoke.Core.Common.Collections.Arrays;
using Bitspoke.Core.Profiling;
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
        Profile(ProcessCells);
    }

    private void ProcessCells()
    {
        Log.Debug();
        
        // TODO: Get default value (0.7f) from config
        var minElevation = ((MapGenStepRockFormationsDef)MapGenStepDef).MinElevation ?? 0.7f;
        
        var tasks = new List<Task>();   
        foreach (var cells in Map.Data.CellsContainer.CellsByRegion.Array)
        {
            tasks.Add(Task.Run(() =>
            {
                foreach (var mapCell in cells.Values)
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
    }
    
    #endregion


    

    
    
}