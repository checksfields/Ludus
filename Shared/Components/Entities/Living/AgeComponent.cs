﻿using Bitspoke.Core.Components;
using Bitspoke.Core.Entities;
using Bitspoke.Core.Systems.Age;

namespace Bitspoke.Ludus.Shared.Components.Entities.Living;

public class AgeComponent : Component
{
    public AgeComponent(Entity entity) : base(entity) { }

    #region Properties

    public override string ComponentName => nameof(AgeComponent);
    public float CurrentAge { get; set; }
    public ulong CurrentAgeInTicks { get; set; }
    public float MaxAge { get; set; }
    public ulong MaxAgeInTicks { get; set; }

    public string DisplayAge() => CurrentAge.ToString();

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
    
    public override void AddComponents() { }
    public override void ConnectSignals() { }

    #endregion
    
    #region Methods
    // none
    #endregion
    
    
}