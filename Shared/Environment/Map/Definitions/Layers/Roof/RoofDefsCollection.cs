using Bitspoke.Core.Definitions.Collections;
using Bitspoke.Core.Profiling;
using Bitspoke.GodotEngine.Utils.Files;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Roof;

public class RoofDefsCollection : DefCollection<RoofDef>
{
    #region Properties

    public override string Key { get; set; } = nameof(RoofDefsCollection);
    public override string ClassName => GetType().FullName;
    public override string? AssemblyName => GetType().Assembly.GetName().Name;

    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    #endregion

    #region Bootstrap

    public static RoofDefsCollection Bootstrap(bool writeToFile = false)
    {
        Profiler.Start();
        
        var defs = new RoofDefsCollection();

        defs.Add(RoofDef.Bootstrap("constructed",1.4f, false));
        defs.Add(RoofDef.Bootstrap("thick",  1.4f));
        defs.Add(RoofDef.Bootstrap("normal", 1.0f));
        
        if (writeToFile)
        {
            var filePath = $"{GodotGlobal.RES_ROOT_PATH}{GodotGlobal.DEFINITIONS_ROOT_PATH}/{nameof(RoofDefsCollection)}{GodotGlobal.SUPPORTED_DEF_TYPE}";
            GodotFileUtils.WriteToFile(filePath, JsonConvert.SerializeObject(defs, Formatting.Indented, CoreGlobal.JsonSerializerSettings));
        }
        
        Profiler.End();
        return defs;
    }

    #endregion

}