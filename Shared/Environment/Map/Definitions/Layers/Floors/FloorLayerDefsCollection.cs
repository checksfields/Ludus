using Bitspoke.Core.Profiling;
using Bitspoke.GodotEngine.Utils.IO;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Floors;

public class FloorLayerDefsCollection : LayerDefsCollection<FloorLayerDef>
{
    #region Properties

    public override string Key { get; set; } = nameof(FloorLayerDefsCollection);
    public override string ClassName => GetType().FullName;
    public override string? AssemblyName => GetType().Assembly.GetName().Name;
    
    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    #endregion

    #region Bootstrap

    public static FloorLayerDefsCollection Bootstrap(bool writeToFile = false)
    {
        Profiler.Start();
        
        var defs = new FloorLayerDefsCollection();
        defs.OrderIndex = 200;

        var floorLayerDef = new FloorLayerDef { Key = "wood" };
        defs.Add(floorLayerDef.Key, floorLayerDef);
        
        if (writeToFile)
        {
            var filePath = $"{GodotGlobal.RES_ROOT_PATH}{GodotGlobal.DEFINITIONS_ROOT_PATH}/{nameof(FloorLayerDefsCollection)}{GodotGlobal.SUPPORTED_DEF_TYPE}";
            GodotFileUtils.WriteToFile(filePath, JsonConvert.SerializeObject(defs, Formatting.Indented, CoreGlobal.JsonSerializerSettings));
        }
        
        Profiler.End();
        return defs;
    }

    #endregion
}