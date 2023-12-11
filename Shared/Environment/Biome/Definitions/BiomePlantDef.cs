using System.Text.Json.Nodes;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Definitions.Parts;
using Bitspoke.Core.Definitions.Parts.Common;
using Bitspoke.Core.Utils.Json;
using Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Biome.Definitions;

public class BiomePlantDef : DefPart
{
    #region Properties

    public float Density { get; set; }
    public int   GrowthRate { get; set; }
    public Dictionary<string, FrequencyDef> PlantWeights { get; set; }
        
        
    public bool  WildPlantsIgnoreFertility = false; 
    [JsonIgnore] private List<PlantDef>? CachedWildPlants { get; set; }
    [JsonIgnore] public List<PlantDef> WildPlants => CachedWildPlants ??= Find.DB.WildPlantDefs
        .Where(w => w.PlantDetails.CanCluster && PlantWeights.ContainsKey(w.Key))
        .ToList();

    [JsonIgnore] private float? CachedMaxClusterRadius { get; set; }
    [JsonIgnore] public float MaxClusterRadius => CachedMaxClusterRadius ??= WildPlants?.Max(m => m.PlantDetails.ClusterDef?.Radius ?? -1) ?? 0;
        
    [JsonIgnore] public float TotalWeight => PlantWeights.Values.Sum(s => s.Frequency);
        
    #endregion

    #region Constructors and Initialisation

    public BiomePlantDef()
    {
        Init();
    }

    private void Init()
    {
        PlantWeights = new Dictionary<string, FrequencyDef>();
    }
        
    public override T Clone<T>()
    {
        var def = new BiomePlantDef();

        def.Density = Density;
        def.GrowthRate = GrowthRate;
        def.PlantWeights = new Dictionary<string, FrequencyDef>(PlantWeights);
            
        return def as T;
    }

    public BiomePlantDef Clone()
    {
        return Clone<BiomePlantDef>();
    }
        
    #endregion

    #region Methods
    public override IDef Deserialize(JsonNode node)
    {
        return node.DeserializeAnonymousType(this);
    }  
    #endregion

    #region Bootstrap

    public static BiomePlantDef Bootstrap(float density, int growthRate, Dictionary<string, FrequencyDef> vegetationDefs)
    {
        return new BiomePlantDef()
        {
            Density = density,
            GrowthRate = growthRate,
            PlantWeights = new Dictionary<string, FrequencyDef>(vegetationDefs)
        };
    }

    #endregion
        
}