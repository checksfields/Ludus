using Bitspoke.Core.Common.Collections.Lists;
using Bitspoke.Core.Components.Life;
using Bitspoke.Core.Components.Time;
using Bitspoke.Core.Definitions.TypeDatas.Time;
using Bitspoke.Core.Systems;
using Bitspoke.Core.Systems.Time;

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
    
    //public Map Map { get; set; }
    public BitspokeList<GrowthComponent> GrowthComponents { get; set; }
    public TickComponent? TickComponent { get; }
    
    #endregion

    #region Constructors and Initialisation

    
    public GrowthSystem() : base()
    {
        TimeSystem.RegisterForTick(TickTypeData.MEDIUM_TICK_KEY, this);
    }
    
    #endregion

    #region Overrides

    public override void Init()
    {
        GrowthComponents = new();
    }
    
    public override void AddComponents() { }
    public override void ConnectSignals() { }

    #endregion
    
    #region Methods

    public static void Register(GrowthComponent growthComponent)
    {
        
    }
    
    public void OnTick()
    {
        Profile(ProcessTick);
    }

    private void ProcessTick()
    {
        foreach (var growthComponent in GrowthComponents)
        {
            growthComponent.Growth += growthComponent.GrowIncrementPerGrowthUpdate;
            growthComponent.Growth = Math.Min(growthComponent.Growth, growthComponent.MaxGrowthPercent);
        }
    }
    
    #endregion
}