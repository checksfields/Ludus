using Bitspoke.Core.Signal;
using Bitspoke.Core.Utils.Reflection;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;
using Profile = OpenQA.Selenium.DevTools.V114.Profiler.Profile;

namespace Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps;

public abstract class MapGenStep
{
    #region Properties

    public const string COMPLETE = nameof(Complete);
    public delegate void Complete(string genStepKey);

    public abstract string StepName { get; }
    protected Map Map { get; set; }
    public MapGenStepDef MapGenStepDef { get; set; }
    public int WaitForAntecedentStepsDelayMs => MapGenStepDef?.WaitForAntecedentStepsDelayMs ?? 0;

    public bool IsInitialised { get; set; }
        
    private Dictionary<string, bool> AntecedentStepsCompletedFlags { get; set; } = new Dictionary<string, bool>();

        
    #endregion

    #region Constructors and Initialisation

    public MapGenStep(Map map, MapGenStepDef mapGenStepDef)
    {
        Map = map;
        MapGenStepDef = mapGenStepDef;
            
        Init();
    }
        
    private void Init()
    {
        HandleAntecedentSteps();
        IsInitialised = true;
    }
        
    #endregion

    #region Methods

    protected void HandleAntecedentSteps()
    {
        if (MapGenStepDef.HasAntecedentSteps)
        {
            foreach (var runAfterGenStepKey in MapGenStepDef.AntecedentStepKeys)
            {
                var def = Find.DB.MapGenStepDefs.Defs[runAfterGenStepKey];
                if (def != null)
                {
                    var assembly = def.GenStepAssemblyName.GetAssembly();
                    var genStepType = assembly.GetType(def.GenStepClassName);
                    var key = $"{genStepType.Name}_Complete";

                    AntecedentStepsCompletedFlags.Add(key, false);
                        
                    SignalManager.Connect(new SignalDetails(key, MapGenStep.COMPLETE, typeof(MapGenStep), this, nameof(OnAntecedentStepCompleted)));
                }
            }
        }
    }
        
    private void OnAntecedentStepCompleted(string genStepDefKey)
    {
        if (!AntecedentStepsCompletedFlags.ContainsKey(genStepDefKey))
            Log.Exception($"RunAfterGenStepsCompleted does not contain a key matching {genStepDefKey}", -9999999);

        AntecedentStepsCompletedFlags[genStepDefKey] = true;
    }

    protected bool CanRun()
    {
        return !AntecedentStepsCompletedFlags.ContainsValue(false);
    }

    public void Generate()
    {
        if (!IsInitialised)
            Log.Error($"{MapGenStepDef.GenStepClassName} has not been initialised.  Please call Init first.", -9999999);

        while (!CanRun())
            Task.Run(async () => { await Task.Delay(WaitForAntecedentStepsDelayMs); }).Wait();
            
        StepGenerate();
            
        SignalManager.Emit($"{StepName}_Complete", $"{StepName}_Complete");
    }

    protected abstract void StepGenerate();

    #endregion


}