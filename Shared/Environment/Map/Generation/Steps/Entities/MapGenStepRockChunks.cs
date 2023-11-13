using System.Linq.Expressions;
using Bitspoke.Core.Profiling;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Terrain;

namespace Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Entities;

public class MapGenStepRockChunks : MapGenStep
{
    #region Properties

    public override string StepName => nameof(MapGenStepRockChunks);
    
    #endregion

    #region Constructors and Initialisation

    public MapGenStepRockChunks(Map map, MapGenStepDef mapGenStepDef) : base(map, mapGenStepDef) { }
    
    #endregion

    #region Methods

    protected override void StepGenerate()
    {
        Profiler.Start();
        
        //Log.TODO("Implement");
        
        Profiler.End();
    }
    
    
    private Expression<Func<TerrainDef, bool>> SearchTerrainExpression (float fertility)
    {
        return ste => ste.Fertility >= fertility;
    }
    
    #endregion


    

    
    
}