﻿using System.Linq.Expressions;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Terrain;

namespace Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Layers;

public class MapGenStepFogLayer : MapGenStep
{
    #region Properties

    public override string StepName => nameof(MapGenStepFogLayer);
    
    #endregion

    #region Constructors and Initialisation

    public MapGenStepFogLayer(Map map, MapGenStepDef mapGenStepDef) : base(map, mapGenStepDef) { }
    
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