using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation.Structures.Natural;

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
        var minElevation = ((MapGenStepRockFormationsDef)MapGenStepDef).MinElevation ?? 0.7f;
        
        foreach (var mapCell in Map.Cells.All)
        {
            var cellElevation = mapCell.Elevation;
            if (cellElevation <= minElevation)
                continue;
            
            // TODO: Check other data layers????  Caves for example

            // spawn the rock formation
            mapCell.StructureDef = Find.DB.RockDefs[mapCell.Stratum].Clone();
            mapCell.StructureDef.Index = mapCell.Index;
            mapCell.Values.Add("StructureDef", mapCell.StructureDefKey);
        }
        Profiler.End();
    }
    
    #endregion


    

    
    
}