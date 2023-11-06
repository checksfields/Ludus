using Bitspoke.Core.Components.Collections;
using Bitspoke.Core.Definitions;
using Bitspoke.Ludus.Shared.Common.Entities;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Entities.Definitions;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public abstract class EntityDef : Def
{
    #region Properties

    public override string ClassName => null;
    public override string? AssemblyName => null;
    
    public abstract EntityType Type { get; set; }
    [JsonIgnore] public abstract int SubTypesFlag { get; }

    [JsonProperty(ItemTypeNameHandling = TypeNameHandling.Auto)]
    public DefCollectionComponent DefCollectionComponent { get; set; } = new();
    
    #endregion

    #region Constructors and Initialisation

    protected void Clone(EntityDef clone)
    {
        base.Clone(clone);
        clone.Type = Type;
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
        
    public virtual void PostLoad() { Log.Debug(); }
    
    public abstract bool HasSubTypeFlag(int? flagAsInt);

    #endregion


}
