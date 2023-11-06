﻿using Bitspoke.Core.Common.Maths.Geometry;
using Bitspoke.Core.Common.States.Games;
using Bitspoke.Core.Common.TypeDatas;
using Bitspoke.Core.Databases.Definitions;
using Bitspoke.Core.Databases.TypeData;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Systems.Time;
using Bitspoke.GodotEngine.Databases.Resources.Shaders;
using Bitspoke.GodotEngine.Databases.Resources.Textures;
using Bitspoke.Ludus.Shared.Entities.Definitions;
using Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;
using Bitspoke.Ludus.Shared.Entities.Definitions.Structures.Natural.Rocks.Definitions;
using Bitspoke.Ludus.Shared.Environment.Biome.Definitions;
using Bitspoke.Ludus.Shared.Environment.Map;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Roof;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Terrain;
using Bitspoke.Ludus.Shared.Environment.World;
using Shared.Components.Settings.Game;

namespace Bitspoke.Ludus.Shared
{
    public static class Find
    {
        public static GameStateManager GameStateManager => GameStateManager.Instance;

        public static LudusGameSettingsComponent LudusGameSettingsComponent => LudusGameSettingsComponent.Instance;

        public static World CurrentWorld => WorldManager.Instance.CurrentWorld;
        public static World World(int mapID)
        {
            Log.TODO("Get from worlds collection.  For now just return current world.  Note we may only ever have 1 world per game/save");
            return CurrentWorld;
        }
        public static World FindWorld(this int worldID) => World(worldID);
        
        public static Map CurrentMap => CurrentWorld?.Maps?.Current ?? null;
        public static Map Map(int mapID) => CurrentWorld?.Maps?[mapID] ?? null;
        public static Map FindMap(this int mapID) => Map(mapID);

        public class Common
        {
            public static Circle MaxCircle => new (LudusGameSettingsComponent.Instance.MapSize.x);
        }
        
        public class DB
        {
            public static readonly TextureDatabase TextureDB = TextureDatabase.Instance;
            public static readonly ShaderDatabase ShaderDB = ShaderDatabase.Instance;
            
            
            public static readonly DefDatabase DefDB = DefDatabase.Instance;
            
            // BIOME
            public static readonly BiomeDefsCollection BiomeDefs = DefDB.GetFromDB<BiomeDefsCollection>();
            
            // Map
            public static readonly MapGenStepDefsCollection MapGenStepDefs = DefDB.GetFromDB<MapGenStepDefsCollection>();
            
            // Layers
            public static readonly List<Def> LayerDefsList = DefDB.Values.Where(w => typeof(ILayerDefsCollection).IsAssignableFrom(w.GetType())).ToList();
            public static readonly List<Def> OrderedLayerDefs = LayerDefsList.OrderBy(o => ((ILayerDefsCollection)o).OrderIndex).ToList();

            // Entities
            public static Dictionary<string, EntityDef> EntityDefs = DefDB.Get<EntityDef>();
            // Rocks
            public static readonly RockDefsCollection RockDefs = DefDB.GetFromDB<RockDefsCollection>();
            public static readonly List<RockDef> RockDefsList = RockDefs.Defs.Values.ToList();
            // Plants
            public static Dictionary<string, PlantDef> PlantDefs = DefDB.Get<PlantDef>();
            public static Dictionary<string, PlantDef>? WildPlantDefs = DefDB.Get<PlantDef>(p => p?.IsWild ?? false);
            public static readonly List<PlantDef> PlantDefsList = PlantDefs.Values.ToList();
            
            // Roof
            public static readonly RoofDefsCollection RoofDefs = DefDB.GetFromDB<RoofDefsCollection>();
            
            
            // Terrain
            private static Dictionary<string, TerrainDef>? terrainDefs;
            public static Dictionary<string, TerrainDef> TerrainDefs => terrainDefs ??= DefDB.Get<TerrainDef>();
            
            public static List<TerrainDef> TerrainDefsList = TerrainDefs.Values.ToList();
            
        }

        public class TypeData
        {
            public static TypeDataDatabase TypeDataDB = TypeDataDatabase.Instance;
            
            public static TypeDataCollection ElevationTypeData = TypeDataDB.GetFromDB<TypeDataCollection>(nameof(ElevationTypeData));
            public static TypeDataCollection VegetationDensityTypeData = TypeDataDB.GetFromDB<TypeDataCollection>(nameof(VegetationDensityTypeData));
            
        }

        public class Systems
        {
            public static TimeSystem TimeSystem => TimeSystem.Instance;
        }
        
    }
}