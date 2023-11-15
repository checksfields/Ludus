using Bitspoke.Core.Components.Collections;
using Bitspoke.Core.Components.Collections.Definitions;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Definitions.Parts.Graphics;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public abstract class LayerDef : Def
{
    #region Properties

    [JsonRequired] public override string Key { get; set; }
    public override string ClassName => null;
    public override string? AssemblyName => null;
    
    public bool? IsNatural { get; set; }
    public List<string> AvailableAffordanceKeys { get; set; }
    public GraphicDef GraphicDef { get; set; }
    
    [JsonProperty(ItemTypeNameHandling = TypeNameHandling.Auto)]
    public DefCollectionComponent DefCollectionComponent { get; set; } = new();
    
    #endregion

    #region Constructors and Initialisation

    protected void Clone(LayerDef toClone, LayerDef clone)
    {
        base.Clone(clone);
        
        clone.GraphicDef = GraphicDef;
        clone.IsNatural = IsNatural;
        clone.AvailableAffordanceKeys = AvailableAffordanceKeys;
    }
    
    #endregion

    #region Methods
    public void AddDefComponent(IDef def) => DefCollectionComponent.Add(def.GetType().Name, def);
    public void AddDefComponent(string key, IDef def) => DefCollectionComponent.Add(key, def);
    
    public bool HasDefComponent<T>() => HasDefComponent(typeof(T).Name);
    public bool HasDefComponent(string key) => DefCollectionComponent.Contains(key);
    
    public IDef? GetDefComponent(string key) => DefCollectionComponent.Contains(key) ? DefCollectionComponent[key] : default;
    public T? GetDefComponent<T>() where T : IDef => GetDefComponent<T>(typeof(T).Name);
    public T? GetDefComponent<T>(string key) where T : IDef => DefCollectionComponent.Contains(key) ? (T) DefCollectionComponent[key]! : default;
    
    #endregion
}