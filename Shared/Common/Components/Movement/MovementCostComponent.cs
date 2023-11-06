namespace Bitspoke.Ludus.Shared.Common.Components.Movement;

public class MovementCostComponent : LudusComponent
{
    #region Properties

    public override string ComponentName => nameof(MovementCostComponent);

    public MovementCostType Type { get; set; } = MovementCostType.None;
    public float Cost { get; set; } = 0f;

    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    #endregion


    public bool Is(MovementCostType type)
    {
        return Type == type;
    }
}