using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bitspoke.Ludus.Shared.Common.Entities;

[JsonConverter(typeof(StringEnumConverter))]
public enum EntityType
{
    None,
    Pawn,
    Structure,
    Item,
    Plant,
    Animal
}