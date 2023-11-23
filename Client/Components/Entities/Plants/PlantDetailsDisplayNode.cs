using Bitspoke.Core.Systems.Time;
using Bitspoke.Ludus.Client.Components.Common.Display;
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
        
        TickSystem.Register(60, OnTick);
    }

    private void OnTick(ulong? ticks)
    {
        Details.OnTick(ticks);
    }

    #endregion

    #region Overrides

    public override void _ExitTree()
    {
        base._EnterTree();
        TickSystem.Deregister(60, OnTick);
    }

    public override void BuildNode()
    {
        base.BuildNode();

        Details.AddGrowthComponent(Plant.GrowthComponent);
        Details.AddAgeComponent(Plant.AgeComponent);
    }
    
    #endregion
    
    #region Methods
    // none
    #endregion

    
}