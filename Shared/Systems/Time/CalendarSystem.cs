using System;
using System.Collections.Generic;
using Bitspoke.Core.Components.Time;
using Bitspoke.Core.Definitions.TypeDatas.Time;
using Bitspoke.Core.Systems;
using Bitspoke.Core.Systems.Time;

namespace Bitspoke.Ludus.Shared.Systems.Time;

public class CalendarSystem: BitspokeSystem//, ITickConsumer
{
    public static class StandardCalendarIntervals
    {
        public const ulong HOUR = 2400;
        public const ulong DAY = HOUR * 24;
        public const ulong MONTH = DAY * 5;
        public const ulong YEAR = MONTH * 4;
    }
    
    #region Properties

    private static CalendarSystem? instance { get; set; }
    public static CalendarSystem Instance
    {
        get => instance ??= new CalendarSystem();
        set
        {
            if (instance != null)
                Log.Exception("Cannot create a second instance of a CalendarSystem.", -9999999);

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
        //TimeSystem.RegisterForTick(TickTypeData.MEDIUM_TICK_KEY, this);
        TickSystem.Register(TickSystem.StandardTickIntervals.MEDIUM, OnTick);
    }

    #endregion
    
    #region Methods

    public void OnTick(ulong ticks)
    {
        //Log.Debug();
    }
    
    #endregion
}