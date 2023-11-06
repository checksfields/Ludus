using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bitspoke.Ludus.Shared.Common.Components.Movement;

[JsonConverter(typeof(StringEnumConverter))]
public enum MovementCostType
{
    None,
    Impassable
}