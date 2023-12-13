using Bitspoke.Core.Common.Collections.Arrays;
using Bitspoke.Core.Common.Collections.Lists;
using Bitspoke.Core.Systems;
using Bitspoke.Core.Systems.Time;
using Bitspoke.Ludus.Shared.Components.Entities.Living;

namespace Bitspoke.Ludus.Shared.Systems.Age;

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
    public ulong DeltaTicks { get; set; } = 0u;
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
    
    public void OnTick(ulong? ticks)
    {
        Ticks++;
        DeltaTicks = ticks.Value;
        
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

                if (comps == null || comps.Length == 0)
                    return;

                foreach (var component in comps)
                {
                    if (component != null)
                    {
                        component.CurrentAgeInTicks += DeltaTicks;
                        if (component.IsExpired)
                        {
                            Find.CurrentMap.Data.RemoveEntity(component.LudusEntity);
                        }
                    }
                }
            }));
        }

        Task.WaitAll(tasks.ToArray());
    }
    
    private void OnProcessTickComplete(Task obj)
    {
        PurgeComponents();  
        TickComplete.Invoke();
    }

    private void PurgeComponents()
    {
        // @PERFORMANCE => 20231117 Benchmark: ~ 1.0 ms
        lock (AgeComponents)
        {
            
            
            //GrownComponents.AddRange(GrowthComponents.Where(w => w.IsFullyGrown));
            AgeComponents.RemoveAll(c => c.IsExpired);
        }
    }
    
    #endregion
}