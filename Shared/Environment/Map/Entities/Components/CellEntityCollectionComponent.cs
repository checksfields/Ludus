using System;
using System.Collections;
using System.Collections.Generic;
using Bitspoke.Core.Common.Collections.Dictionaries;
using Bitspoke.Core.Common.Collections.Lists;
using Bitspoke.Core.Components;
using Bitspoke.GodotEngine.Components.Nodes;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Entities.Systems.Spawn.Natural.Plants;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells;
using Bitspoke.Ludus.Shared.Systems.Spawn;

namespace Bitspoke.Ludus.Shared.Environment.Map.Entities.Components;

public partial class CellEntityCollectionComponent : GodotNode
{
    #region Properties

    public MapCell MapCell { get; set; }
    
    #endregion

    #region Constructors and Initialisation
    // none
    #endregion

    #region Methods

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