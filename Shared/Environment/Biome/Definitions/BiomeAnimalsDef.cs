using System.Text.Json.Nodes;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Definitions.Parts;
using Bitspoke.Core.Definitions.Parts.Common;
using Bitspoke.Core.Utils.Json;

namespace Bitspoke.Ludus.Shared.Environment.Biome.Definitions;

public class BiomeAnimalsDef : DefPart
{
    #region Properties

    public float Density { get; set; }
    public int   GrowthRate { get; set; }
    public Dictionary<string, FrequencyDef> AnimalDefs { get; set; }
        
    #endregion

    #region Constructors and Initialisation

    public BiomeAnimalsDef()
    {
        Init();
    }

    private void Init()
    {
        AnimalDefs = new Dictionary<string, FrequencyDef>();
    }
        
    public override T Clone<T>()
    {
        var def = new BiomeAnimalsDef();

        def.Density = Density;
        def.GrowthRate = GrowthRate;
        def.AnimalDefs = new Dictionary<string, FrequencyDef>(AnimalDefs);
            
        return def as T;
    }

    public BiomeAnimalsDef Clone()
    {
        return Clone<BiomeAnimalsDef>();
    }
        
    #endregion

    #region Methods
    public override IDef Deserialize(JsonNode node)
    {
        return node.DeserializeAnonymousType(this);
    }
    #endregion

    #region Bootstrap

    public static BiomeAnimalsDef Bootstrap(float density, int growthRate, Dictionary<string, FrequencyDef> animalDefs)
    {
        return new BiomeAnimalsDef()
        {
            Density = density,
            GrowthRate = growthRate,
            AnimalDefs = new Dictionary<string, FrequencyDef>(animalDefs)
        };
    }

    #endregion
        
}