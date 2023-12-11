using System.Linq.Expressions;
using Bitspoke.Core.Profiling;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Terrain;

namespace Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Structures.Natural;

public class MapGenStepMineralFormations : MapGenStep
{
    #region Properties

    public override string StepName => nameof(MapGenStepMineralFormations);
    
    #endregion

    #region Constructors and Initialisation

    public MapGenStepMineralFormations(Map map, MapGenStepDef mapGenStepDef) : base(map, mapGenStepDef) { }
    
    #endregion

    #region Methods

    protected override void StepGenerate()
    {
        Profiler.Start("StepGenerate");
        //Log.TODO("Implement");

        var key = "TerrainSoil";
        Profile(message: $"Find.DB.DefDB.Get<TerrainDef>('{key}')", toProfile:() => {
            var terrainDef_Soil = Find.DB.TerrainDefs[key];
        });
        
        
        Profile(message: $"Find.DB.DefDB.Get<TerrainDef>('{key}')", toProfile:() => {
            key = "TerrainMud";
            var terrainDef_Mud = Find.DB.TerrainDefs[key];
        });
        
        
        Profiler.End(additionalKey:"StepGenerate");
    }
    
    
    private Expression<Func<TerrainDef, bool>> SearchTerrainExpression (float fertility)
    {
        return ste => ste.Fertility >= fertility;
    }
    
    #endregion


    

    
    
}