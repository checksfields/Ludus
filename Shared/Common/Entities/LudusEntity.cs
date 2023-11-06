using Bitspoke.Core.Components;
using Bitspoke.Core.Entities;
using Bitspoke.Ludus.Shared.Entities.Definitions;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Common.Entities;

public abstract class LudusEntity : Entity
{
    #region Properties

    [JsonIgnore] public EntityDef Def { get; set; }
    protected string EntityDefKey => Def.Key;

    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    public T? GetComponent<T>() where T : IComponent
    {
        if (!HasComponent<T>())
            Log.Error($"Entity {EntityName} does not contain a component of type {typeof(T).Name}", -9999999);

        return Components.Get<T>();
    }

    public override int GetHashCode()
    {
        return EntityName.GetHashCode() * IDComponent.ID.GetHashCode();
    }
    
    #endregion
}