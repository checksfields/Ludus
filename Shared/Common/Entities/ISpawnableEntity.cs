using Bitspoke.Ludus.Shared.Systems.Spawn;

namespace Bitspoke.Ludus.Shared.Common.Entities;

public interface ISpawnableEntity
{
    SpawnSystem GetSpawnSystem(int mapID);
}