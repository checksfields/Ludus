using Bitspoke.Core.Definitions.Collections;
using Bitspoke.Core.Definitions.Common.Affordances;
using Bitspoke.GodotEngine.Utils.Colors;
using Bitspoke.GodotEngine.Utils.Files;
using Godot;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Affordances;

public class LayerAffordanceDefsCollection : DefCollection<AffordanceDef>
{
    #region Properties

    public override string Key { get; set; } = nameof(LayerAffordanceDefsCollection);
    public override string ClassName => GetType().FullName;
    public override string? AssemblyName => GetType().Assembly.GetName().Name;



    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    #endregion

    #region Bootstrap

    public static LayerAffordanceDefsCollection Bootstrap(bool writeToFile = false)
    {
        Profiler.Start();
        
        var defs = new LayerAffordanceDefsCollection();

        var index = 0;
        defs.Add(AffordanceDef.Bootstrap("LightStructures",  index += 100, Colors.LightGreen.ToColorDef()));
        defs.Add(AffordanceDef.Bootstrap("MediumStructures", index += 100, Colors.Orange.ToColorDef()));
        defs.Add(AffordanceDef.Bootstrap("HeavyStructures",  index += 100, Colors.Red.ToColorDef()));
        defs.Add(AffordanceDef.Bootstrap("WaterResistant",   index += 100, Colors.LightBlue.ToColorDef()));
        defs.Add(AffordanceDef.Bootstrap("WaterProof",       index += 100, Colors.Blue.ToColorDef()));
        defs.Add(AffordanceDef.Bootstrap("MovingFluid",      index += 100, Colors.DarkBlue.ToColorDef()));
        defs.Add(AffordanceDef.Bootstrap("Growable",         index += 100));
        defs.Add(AffordanceDef.Bootstrap("Diggable",         index += 100));


        if (writeToFile)
        {
            var filePath = $"{GodotGlobal.RES_ROOT_PATH}{GodotGlobal.DEFINITIONS_ROOT_PATH}/{nameof(LayerAffordanceDefsCollection)}{GodotGlobal.SUPPORTED_DEF_TYPE}";
            GodotFileUtils.WriteToFile(filePath, JsonConvert.SerializeObject(defs, Formatting.Indented, CoreGlobal.JsonSerializerSettings));
        }
        
        Profiler.End();
        return defs;
    }

    #endregion
}