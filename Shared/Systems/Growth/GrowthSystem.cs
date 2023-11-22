using Bitspoke.Core.Common.Collections.Lists;
using Bitspoke.Core.Components.Time;
using Bitspoke.Core.Definitions.TypeDatas.Time;
using Bitspoke.Core.Systems;
using Bitspoke.Core.Systems.Time;
using Bitspoke.Core.Common.Collections.Arrays;
using Bitspoke.Core.Components.Identity;
using Bitspoke.Core.Components.Location;
using Bitspoke.Core.Systems.Identifier;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Components.Entities.Living;
using Bitspoke.Ludus.Shared.Entities.Natural.Plants;
using Bitspoke.Ludus.Shared.Environment.Map;
using Profile = OpenQA.Selenium.DevTools.V115.Profiler.Profile;

namespace Bitspoke.Ludus.Shared.Systems.Growth;

public class GrowthSystem : BitspokeSystem, ITickConsumer
{
    #region Properties

    private static GrowthSystem? instance { get; set; }
    public static GrowthSystem Instance
    {
        get => instance ??= new GrowthSystem();
        set
        {
            if (instance != null)
                Log.Exception("Cannot create a second instance of a GrowthSystem.", -9999999);

            instance = value;
        }
    }

    public Action TickComplete { get; set; }
    
    //public Map Map { get; set; }
    public BitspokeList<GrowthComponent> GrowthComponents { get; set; }
    public BitspokeArray<GrowthComponent> GrownComponents => new (GrowthComponents.Where(w => w.IsFullyGrown));
    public TickComponent? TickComponent { get; } = new(IDProvisionSystem.NextID(IDType.TimerComponent));
    
    public ulong Ticks { get; set; } = 0u;
    
    #endregion

    #region Constructors and Initialisation

    public GrowthSystem()
    {
        if (instance != null)
            Log.Exception("Can only have one instance of the GrowthSystem.  Please use GrowthSystem.Instance instead of creating a new one.", -9999999);
        
        instance = this;
    }
    
    #endregion

    #region Overrides

    public override void Init()
    {
        GrowthComponents = new();
    }
    
    public override void AddComponents() { }

    public override void ConnectSignals()
    {
        if (TimeSystem.HasInstance)
            TimeSystem.RegisterForTick(TickTypeData.MEDIUM_TICK_KEY, this);
    }

    #endregion
    
    #region Methods

    public static void Register(GrowthComponent growthComponent)
    {
        Instance.GrowthComponents.Add(growthComponent);
    }
    
    public static void Deregister(GrowthComponent growthComponent)
    {
        Instance.GrowthComponents.Remove(growthComponent);
    }
    
    public void OnTick()
    {
        Ticks++;
        
        // throwing ProcessTick into a task so we can return control asap
        //Profile(() => { Task.Run(ProcessTick).ContinueWith(OnProcessTickComplete); });
        Task.Run(ProcessTick).ContinueWith(OnProcessTickComplete);
    }
    
    /// <summary>
    /// @PERFORMANCE => 20231117 Benchmark: ~ 0.3 to 0.4 ms
    /// </summary>
    private void ProcessTick()
    {
        //Profile(() => { 
            
        var map = Find.CurrentMap;    
            
        var arrayCopy = new GrowthComponent[GrowthComponents.Count];
        lock (GrowthComponents)
            GrowthComponents.CopyTo(arrayCopy, 0);
        
        var slices = arrayCopy.Slice(12);
        var tasks = new List<Task>();
        
        for (int i = 0; i < slices.Length; i++)
        {
            var index = i;
            tasks.Add(Task.Run(() =>
            {
                var comps = slices[index];
                
                foreach (var growthComponent in comps)
                {
                    var map = Find.CurrentMap;
                    
                    var growthIncrement = growthComponent.GrowIncrementPerGrowthUpdate;
                    growthIncrement = GrowthRateModifierForMapTemp(growthIncrement, map.Data.Temp);
                    growthIncrement = GrowthRateModifierForMapLight(growthIncrement, map.Data.Light);
                    growthIncrement = GrowthRateModifierForMapMoisture(growthIncrement, map.Data.Moisture);
                    
                    growthComponent.CurrentGrowthPercent += growthIncrement;
                }
            }));    
        }
        Task.WaitAll(tasks.ToArray());
        //});
    }
    
    private void OnProcessTickComplete(Task obj)
    {
        // 20031117 -> removed for now as the performance of iterating through all components is around 0.3-0.4 ms
        //             so there is no need to cull the components list
        
        // // @Performance: this could cause sync issues if the processing time runs longer that the OnTick timer.
        // //               to fix this we can move this back to the ProcessTick method, but it may slow that down, but 
        // //               it will be thread safe.
        // Task.Run(PurgeComponents).ContinueWith(OnPurgeComponentsComplete);
        
        TickComplete.Invoke();
    }

    private void PurgeComponents()
    {
        // @PERFORMANCE => 20231117 Benchmark: ~ 1.0 ms
        lock (GrowthComponents)
        {
            //GrownComponents.AddRange(GrowthComponents.Where(w => w.IsFullyGrown));
            GrowthComponents.RemoveAll(c => c.IsFullyGrown);
        }
    }
    
    private void OnPurgeComponentsComplete(Task obj) { }


    #region Calculators

    public static float GrowthRateModifierForMapTemp(float growthIncrement, float temp)
    {
        // temp is in Celsius.
        
        // TODO - Tier 1 - implement growth modifier calculation
        var growthModifier = 1f;
        return growthIncrement * growthModifier;
    }

    public static float GrowthRateModifierForMapMoisture(float growthIncrement, float moisture)
    {
        // TODO - Tier 1 - implement growth modifier calculation
        var growthModifier = 1f;
        return growthIncrement * growthModifier;
    }
    
    public static float GrowthRateModifierForMapLight(float growthIncrement, float light)
    {
        // TODO - Tier 1 - implement growth modifier calculation
        var growthModifier = 1f;
        return growthIncrement * growthModifier;
    }
    
    #endregion
    
    
    
    
    
    #endregion
}