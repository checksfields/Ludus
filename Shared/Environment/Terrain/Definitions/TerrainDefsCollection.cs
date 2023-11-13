using Bitspoke.Core.Definitions.Collections;
using Bitspoke.Core.Definitions.Parts.Graphics;
using Bitspoke.Core.Definitions.Parts.Graphics.Links;
using Bitspoke.Core.Definitions.Parts.Graphics.Textures;
using Bitspoke.Core.Definitions.Parts.Graphics.Textures.Types;
using Bitspoke.Core.Profiling;
using Bitspoke.GodotEngine.Utils.Files;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Terrain.Definitions;

public class TerrainDefsCollection : DefCollection<TerrainDef>
{
    #region Properties

    public override string Key { get; set; } = nameof(TerrainDefsCollection);
    public override string ClassName => GetType().FullName;
    public override string? AssemblyName => GetType().Assembly.GetName().Name;
        
        
        
    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    #endregion

    #region Bootstrap
        
    public static void Bootstrap(bool writeToFile = false)
    {
        Profiler.Start();
            
        //BootstrapElevationTerrainDefs(writeToFile);
        BootstrapTerrainDefs(writeToFile);
            
        Profiler.End();
    }
        
    private static void BootstrapTerrainDefs(bool writeToFile = false)
    {
        Profiler.Start();
        var defs = new TerrainDefsCollection();
        defs.Key = $"Fertility{defs.Key}";
            
        var key = string.Empty;
        defs.Add(key = "soil",       BootstrapTerrainDef(key, 1.00000001f));
        defs.Add(key = "soilrich",   BootstrapTerrainDef(key, 1.40000001f));
        defs.Add(key = "marshy",     BootstrapTerrainDef(key, 1.00000001f));
        defs.Add(key = "mossy",      BootstrapTerrainDef(key, 1.00000001f));
        defs.Add(key = "sand",       BootstrapTerrainDef(key, 0.10000001f));
        defs.Add(key = "softsand",   BootstrapTerrainDef(key, 0f));
        defs.Add(key = "mud",        BootstrapTerrainDef(key, 0f));
        defs.Add(key = "gravel",     BootstrapTerrainDef(key, 0f));
        defs.Add(key = "ice",        BootstrapTerrainDef(key, 0f));
            
        if (writeToFile)
        {
            var filePath = $"{GodotGlobal.RES_ROOT_PATH}{GodotGlobal.DEFINITIONS_ROOT_PATH}/{defs.Key}{GodotGlobal.SUPPORTED_DEF_TYPE}";
            GodotFileUtils.WriteToFile(filePath, JsonConvert.SerializeObject(defs, Formatting.Indented, CoreGlobal.JsonSerializerSettings));
        }
            
        Profiler.End();
    }
        
    private static TerrainDef BootstrapTerrainDef(string key, float fertility = 0f, bool isRenderable = true)
    {
        var terrainDef = new TerrainDef();

        terrainDef.Key = key;
        terrainDef.IsModdable = true;
        terrainDef.Name = key;
        terrainDef.LabelKey = $"LOC_{key.ToUpper()}";
        terrainDef.DescriptionKey = $"LOC_{key.ToUpper()}_DESC";
        terrainDef.Fertility = fertility;
        terrainDef.GraphicDef = new GraphicDef();
        terrainDef.GraphicDef.TextureDef = new TextureDef
        {
            TextureResourcePath = $"{GodotGlobal.TEXTURES_RESOURCE_ROOT_PATH}/{Global.MAP_LAYERS_ROOT_PATH}/{key}",
            TextureTypeDetails = new SingleTextureTypeDetailsDef(),
        };
        terrainDef.GraphicDef.LinkDef = new LinkDef
        {
            LinkFlags = LinkFlags.Edge | LinkFlags.Rock | LinkFlags.Wall
        };
            
        return terrainDef;
    }
        
    #endregion

}