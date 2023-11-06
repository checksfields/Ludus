using Bitspoke.Core.Components;
using Bitspoke.Core.Entities;
using Bitspoke.Ludus.Shared.Common.Components.Movement;
using Bitspoke.Ludus.Shared.Entities.Components.Containers;

namespace Bitspoke.Ludus.Shared.Common.Entities.Utils;

public static class LudusEntityExtensions
{
    
    #region MovementCostComponent
    public static MovementCostComponent? MovementCost(this ComponentCollection components)
    {
        if (components.Contains<MovementCostComponent>())
            return components.Get<MovementCostComponent>();

        return null;
    }

    public static MovementCostComponent? MovementCost(this Entity entity)
    {
        return entity.Components.MovementCost();
    }
    #endregion
    
    #region PlantGrowerComponent
    public static PlantGrowerComponent? PlantGrower(this ComponentCollection components)
    {
        if (components.Contains<PlantGrowerComponent>())
            return components.Get<PlantGrowerComponent>();

        return null;
    }

    public static PlantGrowerComponent? PlantGrower(this Entity entity)
    {
        return entity.Components.PlantGrower();
    }
    #endregion
    
    
}