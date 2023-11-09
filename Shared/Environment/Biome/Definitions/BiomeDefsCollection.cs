using Bitspoke.Core.Definitions.Collections;
using Bitspoke.Core.Definitions.Parts.Common;
using Bitspoke.Core.Definitions.Parts.Common.Noise;
using Bitspoke.GodotEngine.Utils.Files;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Biome.Definitions;

public class BiomeDefsCollection : DefCollection<BiomeDef>
{
    #region Properties

    public override string Key { get; set; } = nameof(BiomeDefsCollection);
    public override string ClassName => GetType().FullName;
    public override string AssemblyName => GetType().Assembly.GetName().Name;
        
    #endregion

    #region Constructors and Initialisation

    public BiomeDefsCollection() : base()
    {
            
    }
        
    #endregion

    #region Methods

    #endregion

    #region Bootstrap

    public static BiomeDefsCollection Bootstrap(bool writeToFile = false)
    {
        Profiler.Start();
            
        var biomeDefs = new BiomeDefsCollection();
        var biomeDef = new BiomeDef();

        biomeDef.Key = "BiomeA";
        biomeDef.IsModdable = true;
        biomeDef.Name = biomeDef.Key;
        biomeDef.LabelKey = $"{biomeDef.Key}_DESC".ToUpper();
        biomeDef.DescriptionKey = $"{biomeDef.Key}_DESC".ToUpper();

        biomeDef.FertilityTerrainDefs = new Dictionary<string, ProbabilityDef>
        {
            { "soil",     new ProbabilityDef { Key = "soil", Min = -9999.00f, Max = 0.7000000009f } },
            { "soilrich", new ProbabilityDef { Key = "soilrich", Min = 0.700000001f, Max = 9999.00f } },
        };
        biomeDef.ElevationTerrainDefs = new Dictionary<string, ProbabilityDef>
        {
            { "gravel",     new ProbabilityDef { Key = "gravel", Min = 0.490000001f, Max = 0.530000001f } },
        };
        biomeDef.TerrainPatchDefs = new List<PatchDef>
        {
            new PatchDef
            {
                ThresholdDefs = new List<ThresholdDef>
                {
                    new ThresholdDef { Key = "mud", Min = 0.290000001f, Max = 0.3600000009f },
                    new ThresholdDef { Key = "water", Min = 0.360000001f, Max = 0.4800000009f },
                    new ThresholdDef { Key = "waterdeep", Min = 0.480000001f, Max = 1000.00f },
                },
                NoiseDef = new NoiseDef()
                {
                    Frequency = 35.00f,
                    Lacunarity = 2f,
                    Persistence = 0.5f,
                    Octaves = 6
                }
            }
        };
            
        biomeDef.BiomePlantsDef = BiomePlantDef.Bootstrap(0.65f, 10,
            new Dictionary<string, FrequencyDef>
            {
                { "grass",     new FrequencyDef { Key = "grass", Frequency = 4f } },
                { "tallgrass", new FrequencyDef { Key = "tallgrass", Frequency = 1.5f } },
                { "brambles",  new FrequencyDef { Key = "brambles", Frequency = 1f } },
                { "bush",      new FrequencyDef { Key = "bush", Frequency = 0.6f } },
                { "dandelion", new FrequencyDef { Key = "dandelion", Frequency = 0.5f } },

                    
                { "treeoak", new FrequencyDef { Key = "treeoak", Frequency = 0.5f } },
                { "treepoplar", new FrequencyDef { Key = "treepoplar", Frequency = 0.5f } },
                    
                { "healroot", new FrequencyDef { Key = "healroot", Frequency = 0.05f } },
                { "berryplant", new FrequencyDef { Key = "berry", Frequency = 0.05f } },

            }
        );
            
        biomeDef.BiomeAnimalsDef = BiomeAnimalsDef.Bootstrap(5f, -1, new Dictionary<string, FrequencyDef>());
        
        biomeDefs.Add(biomeDef.Key, biomeDef);
            

        if (writeToFile)
        {
            var filePath = $"{GodotGlobal.RES_ROOT_PATH}{GodotGlobal.DEFINITIONS_ROOT_PATH}/{nameof(BiomeDefsCollection)}{GodotGlobal.SUPPORTED_DEF_TYPE}";
            GodotFileUtils.WriteToFile(filePath, JsonConvert.SerializeObject(biomeDefs, Formatting.Indented, CoreGlobal.JsonSerializerSettings));
        }
            
        Profiler.End();
        return biomeDefs;
    }

    #endregion

}