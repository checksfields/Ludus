using Bitspoke.Ludus.Client.Components.Common.Display;
using Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;
using Bitspoke.Ludus.Shared.Entities.Natural.Plants;

namespace Bitspoke.Ludus.Client.Components.Entities.Plants.Natural;

public partial class NaturalPlantDetailsDisplayNode : PlantDetailsDisplayNode
{
    #region Properties

    #endregion

    #region Constructors and Initialisation

    public NaturalPlantDetailsDisplayNode(Plant plant) : base(plant)
    {
        if (!plant.PlantDef?.PlantDetails.IsWild ?? false)
            Log.Exception($"Plant must be of a Natural plant.", -9999999);
    }
    
    #endregion

    #region Overrides

    public override void BuildNode()
    {
        base.BuildNode();
    }

    #endregion
    
    #region Methods

    #endregion


    
}