using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Bitspoke.Ludus.Shared.Common.Entities;

[JsonConverter(typeof(JsonStringEnumConverter))]
[Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
public enum EntityType
{
    None,
    Pawn,
    Structure,
    Item,
    Plant,
    Animal
}