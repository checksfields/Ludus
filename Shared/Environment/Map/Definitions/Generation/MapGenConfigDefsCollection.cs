using System.Text.Json.Nodes;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Definitions.Collections;
using Bitspoke.Core.Profiling;
using Bitspoke.Core.Utils.Json;
using Bitspoke.GodotEngine.Utils.IO;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;

public class MapGenConfigDefsCollection : DefCollection<MapGenConfigDef>
{
    #region Properties

    public override string Key { get; set; } = nameof(MapGenConfigDefsCollection);
    public override string ClassName => GetType().FullName;
    public override string? AssemblyName => GetType().Assembly.GetName().Name;

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

    public static MapGenConfigDefsCollection Bootstrap(bool writeToFile = false)
    {
        Profiler.Start();
            
        var defs = new MapGenConfigDefsCollection();

        defs.Add(new MapGenConfigDef
        {
            Key = "NormalMap",
            MapGenStepKeys = new()
            {
                "MapGenStepDef_Elevation", 
                "MapGenStepDef_Fertility", 
                "MapGenStepDef_NaturalFeatures", 
                "MapGenStepDef_Terrain", 
                "MapGenStepDef_TerrainPatches",
            }
        });
            
        if (writeToFile)
        {
            var filePath = $"{GodotGlobal.RES_ROOT_PATH}{GodotGlobal.DEFINITIONS_ROOT_PATH}/{nameof(MapGenConfigDefsCollection)}{GodotGlobal.SUPPORTED_DEF_TYPE}";
            GodotFileUtils.WriteToFile(filePath, JsonConvert.SerializeObject(defs, Formatting.Indented, CoreGlobal.JsonSerializerSettings));
        }
            
        Profiler.End();
        return defs;
    }

    #endregion

}