using Bitspoke.Core.Components;
using Bitspoke.Core.Components.Entities;
using Bitspoke.Core.Entities;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Systems.Age;

namespace Bitspoke.Ludus.Shared.Components.Entities.Living;

public class AgeComponent : EntityComponent
{
    public AgeComponent(Entity entity) : base(entity) { }

    #region Properties

    public LudusEntity? LudusEntity => (Entity is LudusEntity ludusEntity) ? ludusEntity : null;
    
    public float CurrentAge => (float) CurrentAgeInTicks / CoreGlobal.CalendarConstants.TICKS_PER_DAY;
    public ulong CurrentAgeInTicks { get; set; }
    public float MaxAge { get; set; }
    public ulong MaxAgeInTicks { get; set; }

    public bool IsExpired => MaxAgeInTicks <= CurrentAgeInTicks;

    public string DisplayAge() => CurrentAge.ToString("F");

    #endregion

    #region Constructors and Initialisation
    // none
    #endregion

    #region Overrides

    public override void Init()
    {
        // self register with the GrowthSystem
        AgeSystem.Register(this);
        
    }
    
    public override void ConnectSignals() { }

    #endregion
    
    #region Methods
    // none
    #endregion
    
    
}