using Bitspoke.Core.Components.Entities;
using Bitspoke.Core.Entities;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Systems.Growth;

namespace Bitspoke.Ludus.Shared.Components.Entities.Living;

public class GrowthComponent : EntityComponent
{
    #region Properties

    public LudusEntity? LudusEntity => (Entity is LudusEntity ludusEntity) ? ludusEntity : null;
    
    public float MinGrowthPercent { get; set; } = 0f;
    public float MaxGrowthPercent { get; set; } = 1f;

    public float GrowDays { get; set; }
    public float GrowDaysInTicks => GrowDays * CoreGlobal.CalendarConstants.TICKS_PER_DAY;
    public float CurrentGrowDays { get; set; }
    public float CurrentGrowDaysInTicks => CurrentGrowDays * CoreGlobal.CalendarConstants.TICKS_PER_DAY;
    
    public float CurrentGrowthPercent => CurrentGrowDays / GrowDays;
    
    
    public float GrowIncrementPerGrowthUpdate { get; set; } = 0.01f; 
    
    // protected float growthInt = 0.15f;
    // protected int ageInt;
    // public const float BaseGrowthPercent = 0.15f;
    //
    // private const float GridPosRandomnessFactor = 0.3f;
    //
    // public const float MinGrowthTemperature = 0.0f;
    // public const float MinOptimalGrowthTemperature = 6f;
    // public const float MaxOptimalGrowthTemperature = 42f;
    // public const float MaxGrowthTemperature = 58f;
    
    public bool IsFullyGrown => CurrentGrowthPercent >= MaxGrowthPercent;
    

    #endregion

    #region Constructors and Initialisation

    public GrowthComponent(Entity entity) : base(entity)
    {
    }
    
    public override void Init()
    {
        // self register with the GrowthSystem
        GrowthSystem.Register(this);
        
    }
    
    #endregion

    #region Overloads

       
    public override void ConnectSignals() { }
 
    #endregion

    #region Methods

    #endregion
}
