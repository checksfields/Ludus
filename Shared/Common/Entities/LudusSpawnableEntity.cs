using Bitspoke.Ludus.Shared.Environment.Map.MapCells;
using Bitspoke.Ludus.Shared.Systems.Spawn;

namespace Bitspoke.Ludus.Shared.Common.Entities;

public abstract class LudusSpawnableEntity : LudusEntity, ISpawnableEntity
{
    #region Properties
    // none
    public ulong ID_New { get; set; } = 0u;
    public MapCell MapCell { get; set; }
    
    #endregion

    #region Constructors and Initialisation
    // none
    #endregion

    #region Methods

    public abstract SpawnSystem GetSpawnSystem(ulong mapID);
    
    #endregion


    
}