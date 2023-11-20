using System;
using System.Collections.Generic;
using Bitspoke.Core.Components.Time;
using Bitspoke.Core.Definitions.TypeDatas.Time;
using Bitspoke.Core.Systems;
using Bitspoke.Core.Systems.Time;

namespace Bitspoke.Ludus.Shared.Systems.Environmental;

public class WeatherSystem : BitspokeSystem, ITickConsumer
{
    #region Properties

    private static WeatherSystem? instance { get; set; }
    public static WeatherSystem Instance
    {
        get => instance ??= new WeatherSystem();
        set
        {
            if (instance != null)
                Log.Exception("Cannot create a second instance of a WeatherSystem.", -9999999);

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
        TimeSystem.RegisterForTick(TickTypeData.LONG_TICK_KEY, this);
    }

    #endregion
    
    #region Methods

    public void OnTick() { Log.Debug(); }
    
    #endregion
}