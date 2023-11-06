using Bitspoke.Core.Definitions.Collections;
using Bitspoke.Core.Definitions.Parts.TypeData;
using Bitspoke.GodotEngine.Utils.Files;
using Newtonsoft.Json;


namespace Bitspoke.Ludus.Shared.Environment.World.TypeData
{
    public class VegetationDensityTypeData : DefCollection<TypeDataDef>
    {
        public override string Key { get; set; } = nameof(VegetationDensityTypeData);
        public override string ClassName => GetType().FullName;
        public override string? AssemblyName => GetType().Assembly.GetName().Name;
        
        
        #region Bootstrap

        public static void Bootstrap(bool writeToFile = false)
        {
            Profiler.Start();
            
            VegetationDensityTypeData entityTypeData = new VegetationDensityTypeData();

            var key = "NONE";
            entityTypeData.Add(key,          TypeDataDef.BootstrapTypeData<float>(key, 0.0f));
            entityTypeData.Add(key = "LOW",  TypeDataDef.BootstrapTypeData<float>(key, 0.3f));
            entityTypeData.Add(key = "MED",  TypeDataDef.BootstrapTypeData<float>(key, 0.6f));
            entityTypeData.Add(key = "HIGH", TypeDataDef.BootstrapTypeData<float>(key, 0.8f));
            
            if (writeToFile)
            {
                var filePath = $"{GodotGlobal.RES_ROOT_PATH}{GodotGlobal.TYPE_DATA_ROOT_PATH}/{nameof(VegetationDensityTypeData)}{GodotGlobal.SUPPORTED_TYPE_DATA_TYPE}";
                GodotFileUtils.WriteToFile(filePath, JsonConvert.SerializeObject(entityTypeData, Formatting.Indented, CoreGlobal.JsonSerializerSettings));
            }
            
            Profiler.End();
        }

        #endregion

    }
}