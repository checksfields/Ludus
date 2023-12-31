﻿using Bitspoke.Core.Common.Grids;
using Bitspoke.Core.Common.Grids.GridItems;
using Bitspoke.Core.Components;

namespace Bitspoke.Ludus.Shared.Environment.Map.Components;

public class DataComponent : Component
{
    #region Properties
        
    public override string ComponentName => nameof(DataComponent);

    public Grid<ValueGridItem<float>> ElevationGrid { get; set; }
    public Grid<ValueGridItem<float>> FertilityGrid { get; set; }
        
    #endregion

    #region Constructors and Initialisation

    public override void Init()
    {
        ElevationGrid = new Grid<ValueGridItem<float>>();
        FertilityGrid = new Grid<ValueGridItem<float>>();
    }

    public override void AddComponents() { }
    public override void ConnectSignals() { }

    #endregion

    #region Methods

    #endregion


        
}