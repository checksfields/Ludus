using Bitspoke.Core.Common.Collections.Arrays;
using Bitspoke.Core.Common.Collections.Lists;
using Bitspoke.Core.Components.Identity;
using Bitspoke.Core.Components.Time;
using Bitspoke.Core.Definitions.Time;
using Bitspoke.Core.Definitions.TypeDatas.Time;
using Bitspoke.Core.Systems.Identifier;
using Bitspoke.Core.Systems.Time;
using Bitspoke.Ludus.Shared.Components.Entities.Living;

namespace Bitspoke.Core.Systems.Age;

public class AgeSystem : BitspokeSystem//, ITickConsumer
{
    #region Properties

    private static AgeSystem? instance { get; set; }
    public static AgeSystem Instance
    {
        get => instance ??= new AgeSystem();
        set
        {
            if (instance != null)
                Log.Exception("Cannot create a second instance of a AgeSystem.", -9999999);

            instance = value;
        }
    }

    public Action TickComplete { get; set; }
    
    //public Map Map { get; set; }
    public BitspokeList<AgeComponent> AgeComponents { get; set; }
    public TickComponent? TickComponent { get; } = new(IDProvisionSystem.NextID(IDType.TimerComponent));
    
    public ulong Ticks { get; set; } = 0u;
    
    #endregion

    #region Constructors and Initialisation

    public AgeSystem()
    {
        if (instance != null)
            Log.Exception("Can only have one instance of the AgeSystem.  Please use AgeSystem.Instance instead of creating a new one.", -9999999);
        
        instance = this;
    }
    
    #endregion

    #region Overrides

    public override void Init()
    {
        AgeComponents = new();
    }
    
    public override void AddComponents() { }

    public override void ConnectSignals()
    {
        // if (TimeSystem.HasInstance)
        //     TimeSystem.RegisterForTick(TickTypeData.MEDIUM_TICK_KEY, this);
        
        TickSystem.Register(300, OnTick);
    }

    #endregion
    
    #region Methods

    public static void Register(AgeComponent component)
    {
        Instance.AgeComponents.Add(component);
    }
    
    public static void Deregister(AgeComponent component)
    {
        Instance.AgeComponents.Remove(component);
    }

    // public void OnTick2(ulong ticks)
    // {
    //     
    // }
    
    public void OnTick(ulong ticks)
    {
        Ticks++;
        
        // throwing ProcessTick into a task so we can return control asap
        //Profile(() => { Task.Run(ProcessTick).ContinueWith(OnProcessTickComplete); });
        Task.Run(ProcessTick).ContinueWith(OnProcessTickComplete); 
    }
    
    private void ProcessTick()
    {
        var arrayCopy = new AgeComponent[AgeComponents.Count];
        lock (AgeComponents)
            AgeComponents.CopyTo(arrayCopy, 0);
        
        var slices = arrayCopy.Slice(12);
        var tasks = new List<Task>();
        
        for (int i = 0; i < slices.Length; i++)
        {
            var index = i;
            tasks.Add(Task.Run(() =>
            {
                var comps = slices[index];
                
                foreach (var component in comps)
                {
                    component.Age += component.AgeToAppendOnTick;
                }
            }));    
        }
        Task.WaitAll(tasks.ToArray());    
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
    
    #endregion
}