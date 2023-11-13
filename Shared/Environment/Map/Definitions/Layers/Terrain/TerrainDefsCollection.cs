using Bitspoke.Core.Profiling;
using Bitspoke.GodotEngine.Utils.Files;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Terrain;

public class TerrainDefsCollection : LayerDefsCollection<TerrainDef>
{
    public static TerrainDef? DEFAULT_DEF;

    #region Properties

    public override string Key { get; set; } = nameof(TerrainDefsCollection);
    public override string ClassName => GetType().FullName;
    public override string? AssemblyName => GetType().Assembly.GetName().Name;
    
    #endregion

    #region Constructors and Initialisation

    public TerrainDefsCollection()
    {
        Key = nameof(TerrainDefsCollection);
    }
    
    #endregion

    #region Methods
    
    #endregion

    #region Bootstrap

    public static TerrainDefsCollection Bootstrap(bool writeToFile = false)
    {
        Profiler.Start();
        
        var defs = new TerrainDefsCollection();
        defs.OrderIndex = 100;
        var terrainOrderIndex = 0;
        
        var soil = TerrainDef.Bootstrap(terrainOrderIndex++,"soil", 1.00000001f);
        soil.Ascii = '.';
        soil.AvailableAffordanceKeys = new() { "LightStructures", "MediumStructures", "HeavyStructures", "Growable", "Diggable" };
        defs.Add(soil);
        DEFAULT_DEF = soil;

        var richSoil = TerrainDef.Bootstrap(terrainOrderIndex++,"soilrich", 1.40000001f);
        richSoil.Ascii = 'r';
        richSoil.AvailableAffordanceKeys = soil.AvailableAffordanceKeys;
        defs.Add(richSoil);
        
        var gravel = TerrainDef.Bootstrap(terrainOrderIndex++,"gravel", 0.70000001f);
        gravel.Ascii = Convert.ToChar(176);
        gravel.AvailableAffordanceKeys = soil.AvailableAffordanceKeys;
        defs.Add(gravel);
        
        var softsand = TerrainDef.Bootstrap(terrainOrderIndex++,"softsand");
        softsand.Ascii = 'f';
        softsand.AvailableAffordanceKeys = new() { "LightStructures", "Diggable" };
        defs.Add(softsand);
        
        var sand = TerrainDef.Bootstrap(terrainOrderIndex++,"sand", 0.10000001f);
        sand.Ascii = 'n';
        sand.AvailableAffordanceKeys = new() { "LightStructures", "MediumStructures", "Diggable" };
        defs.Add(sand);

        var mud = TerrainDef.Bootstrap(terrainOrderIndex++,"mud");
        mud.Ascii = 'm';
        // no affordances
        defs.Add(mud);
        
        var mossy = TerrainDef.Bootstrap(terrainOrderIndex++,"mossy", 1.00000001f);
        mossy.Ascii = 'y';
        mossy.AvailableAffordanceKeys = soil.AvailableAffordanceKeys;
        defs.Add(mossy);
        
        var marshy = TerrainDef.Bootstrap(terrainOrderIndex++,"marshy", 1.00000001f);
        marshy.Ascii = 'h';
        marshy.AvailableAffordanceKeys = new() { "LightStructures", "Growable", "Diggable" };
        defs.Add(marshy);

        var ice = TerrainDef.Bootstrap(terrainOrderIndex++,"ice");
        ice.Ascii = 'i';
        ice.AvailableAffordanceKeys = sand.AvailableAffordanceKeys;
        defs.Add(ice);
        
        var water = TerrainDef.Bootstrap(terrainOrderIndex++,"water", 0f);
        water.Ascii = 'w';
        //water.AvailableAffordanceKeys.Add("affordancedef_key2");
        defs.Add(water);
            
        var waterDeep = TerrainDef.Bootstrap(terrainOrderIndex++,"waterdeep", 0f);
        waterDeep.Ascii = 'd';
        //waterDeep.AvailableAffordanceKeys.Add("affordancedef_key2");
        defs.Add(waterDeep);
        
        if (writeToFile)
        {
            var filePath = $"{GodotGlobal.RES_ROOT_PATH}{GodotGlobal.DEFINITIONS_ROOT_PATH}/{nameof(TerrainDefsCollection)}{GodotGlobal.SUPPORTED_DEF_TYPE}";
            GodotFileUtils.WriteToFile(filePath, JsonConvert.SerializeObject(defs, Formatting.Indented, CoreGlobal.JsonSerializerSettings));
        }
        
        Profiler.End();
        return defs;
    }

    #endregion
    
}