using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;

[Flags]
[JsonConverter(typeof(JsonStringEnumConverter))]
[Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
public enum PlantType : int
{
    Undefined = 0,
    Cover = 1,
    Bush = 2,
    Tree = 4,
    Harvestable = 8,
    Deciduous = 16
}