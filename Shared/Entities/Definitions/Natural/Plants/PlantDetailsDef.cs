using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Definitions.Parts;
using Bitspoke.Core.Definitions.Parts.Common;
using Bitspoke.Core.Definitions.Parts.Entity.Living;
using Bitspoke.Core.Utils.Json;

namespace Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;

public class PlantDetailsDef : DefPart
{
    #region Properties
    
    [JsonPropertyName("IsWild")] public bool IsWild { get; set; }
    [JsonPropertyName("IsFarmable")] public bool IsFarmable { get; set; }
    [JsonPropertyName("Fertility")] public FertilityDef FertilityDef { get; set; }
    
    [JsonPropertyName("Cluster")] public ClusterDef ClusterDef { get; set; }
    [JsonIgnore] public bool CanCluster => ClusterDef is { Radius: > 0, Wieght: > 0 };
    
    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    #endregion


    public override T Clone<T>()
    {
        var def = new PlantDetailsDef();

        def.FertilityDef = FertilityDef.Clone();
        
        return def as T;
    }

    public PlantDetailsDef Clone()
    {
        return Clone<PlantDetailsDef>();
    }

    public override IDef Deserialize(JsonNode node)
    {
        return node.DeserializeAnonymousType(this);
    }
}