using System.Text.Json.Nodes;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Definitions.Parts;
using Bitspoke.Core.Utils.Json;
using Bitspoke.Ludus.Shared.Common.Components.Movement;

namespace Bitspoke.Ludus.Shared.Common.Definitions.Movement;

public class MovementCostDef : DefPart
{
    #region Properties

    public MovementCostType Type { get; set; } = MovementCostType.None;
    public float Cost { get; set; } = 0f;

    #endregion

    #region Constructors and Initialisation

    public override T Clone<T>()
    {
        var def = new MovementCostDef();

        def.Type = Type;
        def.Cost = Cost;

        return def as T;
    }

    public MovementCostDef Clone()
    {
        return Clone<MovementCostDef>();
    }
    
    #endregion

    #region Methods
    public override IDef Deserialize(JsonNode node)
    {
        return node.DeserializeAnonymousType(this);
    }
    #endregion


    public bool Is(MovementCostType type)
    {
        return Type == type;
    }

    
}