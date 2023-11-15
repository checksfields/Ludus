using System;
using System.Collections.Generic;
using Bitspoke.Core.Common.Collections.Arrays;
using Bitspoke.Core.Components.Life;
using Bitspoke.Core.Components.Time;
using Bitspoke.Core.Definitions.TypeDatas.Time;
using Bitspoke.Core.Systems;
using Bitspoke.Core.Systems.Time;
using Bitspoke.Ludus.Shared.Environment.Map;

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
    public BitspokeArray<GrowthComponent> GrowthComponents { get; set; }
    public TickComponent? TickComponent { get; }
    
    #endregion

    #region Constructors and Initialisation

    
    public GrowthSystem() : base()
    {
        TimeSystem.RegisterForTick(TickTypeData.MEDIUM_TICK_KEY, this);
    }
    
    #endregion

    #region Overrides

    public override void Init() { }
    public override void AddComponents() { }
    public override void ConnectSignals() { }

    #endregion
    
    #region Methods
    // none
    #endregion
    
    public void OnTick()
    {
        Log.Debug("Tick!");
        
        
        
    }
}