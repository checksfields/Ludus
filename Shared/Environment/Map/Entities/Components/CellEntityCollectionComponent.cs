using Bitspoke.GodotEngine.Components.Nodes;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells;

namespace Bitspoke.Ludus.Shared.Environment.Map.Entities.Components;

public partial class CellEntityCollectionComponent : GodotNode
{
    #region Properties

    public MapCell MapCell { get; set; }
    public override string Name => GetType().Name;
    
    #endregion

    #region Constructors and Initialisation
    // none
    #endregion

    #region Methods

    public override void Init() {}
    public override void AddComponents() {}
    public override void ConnectSignals() {}
    
    public void Clear() { throw new NotImplementedException(); }

    public bool Contains(LudusEntity item) { throw new NotImplementedException(); }

    public void Add(LudusEntity entity, MapCell mapCell)
    {
        if (!ValidateAdd(entity, mapCell))
            return;
        
        MapCell = mapCell;
        MapCell.Entities?.Add(entity);
    }

    public bool ValidateAdd(LudusEntity entity, MapCell mapCell)
    {
        var canAdd = true;
        if (entity is LudusSpawnableEntity)
        {
            canAdd = ((LudusSpawnableEntity)entity).GetSpawnSystem(mapCell.MapID).CanSpawnAt(mapCell);
        }

        return canAdd;
    }

    public bool Remove(LudusEntity entity)
    {
        throw new NotImplementedException();
    }

    public int Count { get; }
    
    #endregion
}