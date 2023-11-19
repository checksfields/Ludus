using Bitspoke.Ludus.Shared.Entities.Natural.Plants;

namespace Bitspoke.Ludus.Client.Components.Entities.Plants;

public abstract partial class PlantDetailsDisplayNode : EntityDetailsDisplayNode
{
    #region Properties

    public Plant Plant => (Plant) LudusEntity;
    
    #endregion

    #region Constructors and Initialisation

    protected  PlantDetailsDisplayNode(Plant plant) : base(plant)
    {
        if (plant == null)
            Log.Exception($"LudusEntity must be of type Plant", -9999999);
    }
    
    #endregion

    #region Overrides

    public override void BuildNode()
    {
        base.BuildNode();
        Details.AddNameValuePair("PlantDetailsDisplayNode:", () => "_PlantDetailsDisplayNode");
    }

    #endregion
    
    #region Methods
    // none
    #endregion

    
}