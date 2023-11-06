using Bitspoke.Ludus.Shared.Environment.Map;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells;

namespace Bitspoke.Ludus.Shared.Systems.Spawn;

public abstract class SpawnSystem
{
    #region Properties

    public Map Map { get; set; }
    
    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    public abstract bool CanSpawnAt(MapCell cell);

    #endregion


}