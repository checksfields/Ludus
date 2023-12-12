using Bitspoke.Core.Components.Entities;
using Bitspoke.Core.Entities;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Systems.Growth;
using Range = Bitspoke.Core.Common.Values.Range;

namespace Bitspoke.Ludus.Shared.Components.Entities.Living;

public class GrowthComponent : EntityComponent
{
    #region Properties

    public LudusEntity? LudusEntity => (Entity is LudusEntity ludusEntity) ? ludusEntity : null;
    
    public Range GrowthRange { get; set; }

    public float GrowDays { get; set; }
    public float GrowDaysInTicks => GrowDays * CoreGlobal.CalendarConstants.TICKS_PER_DAY;
    public float CurrentGrowDays => CurrentGrowDaysInTicks / CoreGlobal.CalendarConstants.TICKS_PER_DAY;
    public float CurrentGrowDaysInTicks { get; set; }
    
    public float CurrentGrowthPercent => CurrentGrowDaysInTicks / GrowDaysInTicks;
    
    public bool IsFullyGrown => CurrentGrowthPercent >= 1.0;
    

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

    public string GetCurrentGrowDaysForDisplay() => CurrentGrowDays.ToString();

    #endregion

    public string GetCurrentGrowPercentForDisplay() => CurrentGrowthPercent.ToString("P");
}
