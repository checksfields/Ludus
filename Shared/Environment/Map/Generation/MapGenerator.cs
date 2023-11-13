using Bitspoke.Core.Profiling;
using Bitspoke.Core.Random;
using Bitspoke.Core.Utils.Reflection;
using Bitspoke.Ludus.Shared.Common.TypeDatas.Game.States;
using Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps;

namespace Bitspoke.Ludus.Shared.Environment.Map.Generation;

public class MapGenerator
{
    #region Properties

    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    public static Map Generate(MapInitConfig initConfig)
    {
        Profiler.Start();
            
        Find.GameStateManager.SetState(LudusGameStatesTypeData.MAP_GENERATION_KEY);
        Rand.PushState(initConfig.SeedPart);
            
        var map = CreateMap(initConfig);

        RunGenSteps(map);   
            
        Rand.PopState();
        Find.GameStateManager.SetState(LudusGameStatesTypeData.MAIN_KEY);
            
        Profiler.End();
        return map;
    }

    private static Map CreateMap(MapInitConfig initConfig)
    {
        return new Map(initConfig);
    }
        
    private static void RunGenSteps(Map map)
    {
        var tasks = new List<Task>();
        var steps = Find.DB.MapGenStepDefs.Defs.Values.Count;
        var currentStep = 1;
        foreach (var mapGenStepDef in Find.DB.MapGenStepDefs.Defs.Values.OrderBy(o => o.OrderIndex))
        {
            var assembly = mapGenStepDef.GenStepAssemblyName.GetAssembly();
            var genStep = assembly.GetInstance<MapGenStep>(mapGenStepDef.GenStepClassName, map, mapGenStepDef);
                
            tasks.Add(Task.Run(() =>
            {
                genStep.Generate();
                Log.Debug($"Step {currentStep++}/{steps}");
            }));
        }

        Task.WaitAll(tasks.ToArray());
            
            
            
        Profiler.Start();
        var all2 = map.Data.CellsContainer.CellsByRegion.Array.SelectMany(a => a.Values);
        Profiler.End();
            
        Profiler.Start();
        var all = map.Data.CellsContainer.CellsByRegion.Array.AsParallel().SelectMany(a => a.Values);
        Profiler.End();

    }
        
    #endregion

}