using Bitspoke.Core.Common.Generation.Steps;
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
        var steps = Find.DB.MapGenStepDefs.Values.Count;
        var currentStep = 1;

        // initialise the steps ... ensure all step dependencies are set correctly
        var genSteps = new SortedList<int, MapGenStep>();
        foreach (var mapGenStepDef in Find.DB.MapGenStepDefs.Values.OrderBy(o => o.OrderIndex))
        {
            var genStepTypeName = mapGenStepDef.GenStepType;
            var genStep = genStepTypeName.GetInstanceFromTypeFullName<MapGenStep>(map, mapGenStepDef);
                 
            genSteps.Add(genStep.MapGenStepDef.OrderIndex, genStep);
        }
        
        // process the steps ... run in threads if possible
        foreach (var genStep in genSteps.Values)
        {
            tasks.Add(Task.Run(() =>
            {
                Profile(genStep.Generate);
                Log.Debug($"Step {currentStep++}/{steps} - {genStep.MapGenStepDef.OrderIndex}:{genStep.StepName} Complete");
            }));
        }
        Task.WaitAll(tasks.ToArray());
    }
        
    #endregion

}