using Bitspoke.Core.Profiling;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;

namespace Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Layers;

public class MapGenStepTerrainPatches : MapGenStepLayer
{

    #region Properties

    public override string StepName => GetType().Name;

    #endregion

    #region Constructors and Initialisation
        
    public MapGenStepTerrainPatches(Map map, MapGenStepDef mapGenStepDef) : base(map, mapGenStepDef)
    {
    }
        
    #endregion

    #region Methods

        
        
    protected override void StepGenerate()
    {
        Profiler.Start();
        foreach (var mapCell in  Map.Data.CellsContainer.Cells)
        {
                
        }
        Profiler.End();

    }
        
    #endregion
        
}