using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;

[Flags]
[JsonConverter(typeof(StringEnumConverter))]
public enum PlantType : int
{
    Undefined = 0,
    Cover = 1,
    Bush = 2,
    Tree = 4,
    Harvestable = 8,
    Deciduous = 16
}