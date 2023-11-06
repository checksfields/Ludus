using System;
using System.Collections.Generic;
using Bitspoke.Ludus.Shared.Systems.Spawn;

namespace Bitspoke.Ludus.Shared.Common.Entities;

public abstract class LudusSpawnableEntity : LudusEntity, ISpawnableEntity
{
    #region Properties
    // none
    #endregion

    #region Constructors and Initialisation
    // none
    #endregion

    #region Methods

    public abstract SpawnSystem GetSpawnSystem(int mapID);
    
    #endregion


    
}