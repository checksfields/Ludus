using System.Linq.Expressions;
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


        Profiler.Start();
        var terrainDef = Find.DB.DefDB.Get<TerrainDef>("soil");
        Profiler.End(message:"Find.DB.DefDB.Get<TerrainDef>('soil')");
        
        Profiler.Start();
        var terrainDef2 = Find.DB.DefDB.Get<TerrainDef>()["mud"];
        Profiler.End(message:"Find.DB.DefDB.Get<TerrainDef>()['mud']");
        //var terrainDefs = Find.DB.DefDB.Query<TerrainDef>(t => t?.Fertility > 0.5f);
        
        Profiler.Start();
        var terrainDef_Soil = Find.DB.TerrainDefs["soil"];
        Profiler.End(message:"Find.DB.TerrainDefs2['soil']");
        
        Profiler.Start();
        var terrainDef_Mud = Find.DB.TerrainDefs["mud"];
        Profiler.End(message:"Find.DB.TerrainDefs2['mud']");
        
        
        Profiler.End(additionalKey:"StepGenerate");
    }
    
    
    private Expression<Func<TerrainDef, bool>> SearchTerrainExpression (float fertility)
    {
        return ste => ste.Fertility >= fertility;
    }
    
    #endregion


    

    
    
}