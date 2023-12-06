using System.Text.Json.Nodes;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Definitions.Collections;
using Bitspoke.Core.Definitions.Parts.TypeDatas;
using Bitspoke.Core.Profiling;
using Bitspoke.Core.Utils.Json;
using Bitspoke.GodotEngine.Utils.IO;
using Newtonsoft.Json;


namespace Bitspoke.Ludus.Shared.Environment.World.TypeData;

public class ElevationTypeData : DefCollection<DefaultTypeDataDef>
{
    #region Properties

    public override string Key { get; set; } = nameof(ElevationTypeData);
    public override string ClassName => GetType().FullName;
    public override string? AssemblyName => GetType().Assembly.GetName().Name;
        
    public const string DEFAULT_KEY         = "DEFAULT";
    public const string NONE_KEY            = "NONE";
    public const string HILLS_KEY           = "HILLS";
    public const string LARGE_HILLS_KEY     = "LARGE_HILLS";
    public const string MOUNTAINS_KEY       = "MOUNTAINS";
    public const string LARGE_MOUNTAINS_KEY = "LARGE_MOUNTAINS";
        
    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods
    public override IDef Deserialize(JsonNode node)
    {
        return node.DeserializeAnonymousType(this);
    }
    #endregion
        
        
    #region Bootstrap

    public static void Bootstrap(bool writeToFile = false)
    {
        Profiler.Start();
            
        ElevationTypeData entityTypeData = new ElevationTypeData();

        var key = DEFAULT_KEY;
        entityTypeData.Add(key,                       DefaultTypeDataDef.BootstrapTypeData<float>(key, 0.0f));
        entityTypeData.Add(key = NONE_KEY,            DefaultTypeDataDef.BootstrapTypeData<float>(key, 0.8f));
        entityTypeData.Add(key = HILLS_KEY,           DefaultTypeDataDef.BootstrapTypeData<float>(key, 0.9f));
        entityTypeData.Add(key = LARGE_HILLS_KEY,     DefaultTypeDataDef.BootstrapTypeData<float>(key, 1.0f));
        entityTypeData.Add(key = MOUNTAINS_KEY,       DefaultTypeDataDef.BootstrapTypeData<float>(key, 1.1f));
        entityTypeData.Add(key = LARGE_MOUNTAINS_KEY, DefaultTypeDataDef.BootstrapTypeData<float>(key, 1.2f));
            
            

            
        if (writeToFile)
        {
            var filePath = $"{GodotGlobal.RES_ROOT_PATH}{GodotGlobal.TYPE_DATA_ROOT_PATH}/{nameof(ElevationTypeData)}{GodotGlobal.SUPPORTED_TYPE_DATA_TYPE}";
            GodotFileUtils.WriteToFile(filePath, JsonConvert.SerializeObject(entityTypeData, Formatting.Indented, CoreGlobal.JsonSerializerSettings));
        }
            
        Profiler.End();
    }

    #endregion
        
}