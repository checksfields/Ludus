using System;
using System.Collections.Generic;
using Bitspoke.Core.Components.Time;
using Bitspoke.Core.Definitions.TypeDatas.Time;
using Bitspoke.Core.Systems;
using Bitspoke.Core.Systems.Time;

namespace Bitspoke.Ludus.Shared.Systems.Schedule;

public class ScheduleSystem: BitspokeSystem//, ITickConsumer
{
    #region Properties

    private static ScheduleSystem? instance { get; set; }
    public static ScheduleSystem Instance
    {
        get => instance ??= new ScheduleSystem();
        set
        {
            if (instance != null)
                Log.Exception("Cannot create a second instance of a ScheduleSystem.", -9999999);

            instance = value;
        }
    }
    
    public TickComponent? TickComponent { get; }
    public Action TickComplete { get; set; }

    #endregion

    #region Constructors and Initialisation
    // none
    #endregion

    #region Overrides

    public override void Init() { }
    public override void AddComponents() { }

    public override void ConnectSignals()
    {
        //TimeSystem.RegisterForTick(TickTypeData.SHORT_TICK_KEY, this);
        TickSystem.Register(300, OnTick);
    }
    
    #endregion
    
    #region Methods

    public void OnTick(ulong ticks)
    {
    }
    
    #endregion
}