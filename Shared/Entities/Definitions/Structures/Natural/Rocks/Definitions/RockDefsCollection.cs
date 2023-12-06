using System.Text.Json.Nodes;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Definitions.Collections;
using Bitspoke.Core.Profiling;
using Bitspoke.Core.Signal;
using Bitspoke.Core.Utils.Json;
using Bitspoke.Core.Utils.Primatives.Chars;
using Bitspoke.GodotEngine.Controllers.Resources.Loaders;
using Bitspoke.GodotEngine.Controllers.Resources.Loaders.Implementations;
using Bitspoke.GodotEngine.Utils.Colors;
using Bitspoke.GodotEngine.Utils.IO;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Terrain;
using Godot;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Entities.Definitions.Structures.Natural.Rocks.Definitions;

public class RockDefsCollection : DefCollection<RockDef>
{
    #region Properties

    public override string Key { get; set; } = nameof(RockDefsCollection);
    public override string ClassName => GetType().FullName;
    public override string? AssemblyName => GetType().Assembly.GetName().Name;
    
    #endregion

    #region Constructors and Initialisation

    public RockDefsCollection() : this(true)
    {
        
    }
    
    public RockDefsCollection(bool connectSignals)
    {
        if (connectSignals)
            SignalManager.Connect(new SignalDetails(DefLoader.LOAD_ALL_COMPLETE, typeof(Loader<>), this, nameof(OnLoadAllComplete)));
    }
    
    #endregion

    #region Methods

    public override IDef Deserialize(JsonNode node)
    {
        return node.DeserializeAnonymousType(this);
    }
    
    /// <summary>
    /// Called once the loader for this class has finished loading all defs. 
    /// </summary>
    private void OnLoadAllComplete()
    {
        var  orderIndexMax = Find.DB.TerrainDefs.Select(s => s.Value.OrderIndex).Max();
        foreach (var rockDef in Find.DB.RockDefsList)
        {
            var layerKey = $"rock_{rockDef.Key.ToLower()}";
            var layerDef = TerrainDef.BootstrapRuntime(++orderIndexMax, layerKey);
            layerDef.Ascii = rockDef.Ascii?.ToUpper();
            rockDef.AssociatedTerrainDefKey = layerDef.Key;
            
            Find.DB.TerrainDefs.Add(layerKey, layerDef);
        }
    }
    
    #endregion

    #region Bootstrap

    public static RockDefsCollection Bootstrap(bool writeToFile = false)
    {
        Profiler.Start();

        var defs = new RockDefsCollection(false);

        RockDef sandstoneDef; 
        defs.Add(sandstoneDef = RockDef.Bootstrap("sandstone", Color.Color8(126,104,094).ToColorDef()));
        sandstoneDef.Ascii = 's';
        
        RockDef granite; 
        defs.Add(granite = RockDef.Bootstrap("granite",   Color.Color8(105,095,097).ToColorDef()));
        granite.Ascii = 'g';
        
        RockDef marble; 
        defs.Add(marble = RockDef.Bootstrap("marble",    Color.Color8(132,135,132).ToColorDef()));
        marble.Ascii = 'm';
        
        RockDef limestone; 
        defs.Add(limestone = RockDef.Bootstrap("limestone", Color.Color8(158,153,135).ToColorDef()));
        limestone.Ascii = 'l';
        
        RockDef slate; 
        defs.Add(slate = RockDef.Bootstrap("slate",     Color.Color8(070,070,070).ToColorDef()));
        slate.Ascii = 't';
        
        if (writeToFile)
        {
            var filePath = $"{GodotGlobal.RES_ROOT_PATH}{GodotGlobal.DEFINITIONS_ROOT_PATH}/{nameof(RockDefsCollection)}{GodotGlobal.SUPPORTED_DEF_TYPE}";
            GodotFileUtils.WriteToFile(filePath, JsonConvert.SerializeObject(defs, Formatting.Indented, CoreGlobal.JsonSerializerSettings));
        }
            
        Profiler.End();
        return defs;
    }

    #endregion
    
}