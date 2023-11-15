using Bitspoke.Core.Components;
using Bitspoke.Core.Components.Collections;
using Bitspoke.Core.Entities;
using Bitspoke.Ludus.Shared.Common.Components.Movement;

namespace Bitspoke.Ludus.Shared.Common.Entities.Utils;

public static class LudusEntityExtensions
{
    
    #region MovementCostComponent
    public static MovementCostComponent? MovementCost(this ComponentCollection<IComponent> components)
    {
        foreach (var component in components.Values)
        {
            if (component is MovementCostComponent)
                return component as MovementCostComponent;
        }

        return null;
    }
    
    public static MovementCostComponent? MovementCost(this Entity entity)
    {
        return entity.Components.MovementCost();
    }
    #endregion
    
    #region PlantGrowerComponent
    //[Obsolete("PlantGrowthComponent is no longer supported",true)]
    // public static PlantGrowthComponent? PlantGrower(this ComponentCollection components)
    // {
    //     if (components.Contains<PlantGrowthComponent>())
    //         return components.Get<PlantGrowthComponent>();
    //
    //     return null;
    // }
    //
    // [Obsolete("PlantGrowthComponent is no longer supported",true)]
    // public static PlantGrowthComponent? PlantGrower(this Entity entity)
    // {
    //     return entity.Components.PlantGrower();
    // }
    #endregion
    
    
}