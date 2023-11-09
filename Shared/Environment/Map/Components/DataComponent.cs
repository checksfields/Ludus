using Bitspoke.Core.Common.Grids;
using Bitspoke.Core.Common.Grids.GridItems;
using Bitspoke.Core.Components;

namespace Bitspoke.Ludus.Shared.Environment.Map.Components;

public class DataComponent : SimpleComponent
{
    #region Properties
        
    public override string ComponentName => nameof(DataComponent);

    public Grid<ValueGridItem<float>> ElevationGrid { get; set; }
    public Grid<ValueGridItem<float>> FertilityGrid { get; set; }
        
    #endregion

    #region Constructors and Initialisation

    public override void Init()
    {
        base.Init();
        ElevationGrid = new Grid<ValueGridItem<float>>();
        FertilityGrid = new Grid<ValueGridItem<float>>();
    }

    #endregion

    #region Methods

    #endregion


        
}