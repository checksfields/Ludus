using System;
using System.Collections.Generic;
using Bitspoke.Core.Components.Time;
using Bitspoke.Core.Definitions.TypeDatas.Time;
using Bitspoke.Core.Systems;
using Bitspoke.Core.Systems.Time;

namespace Bitspoke.Ludus.Shared.Systems.Time;

public class CalendarSystem: BitspokeSystem, ITickConsumer
{
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
        TimeSystem.RegisterForTick(TickTypeData.MEDIUM_TICK_KEY, this);
    }

    #endregion
    
    #region Methods

    public void OnTick() { Log.Debug(); }
    
    #endregion
}