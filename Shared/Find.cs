using Bitspoke.Core.Common.Maths.Geometry;
using Bitspoke.Core.Common.States.Games;
using Bitspoke.Core.Common.TypeDatas;
using Bitspoke.Core.Databases.Definitions;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Systems.Time;
using Bitspoke.GodotEngine.Databases.Resources.Shaders;
using Bitspoke.GodotEngine.Databases.Resources.Textures;
using Bitspoke.Ludus.Shared.Components.Entities.Living;
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
using Bitspoke.Ludus.Shared.Systems.Age;
using Bitspoke.Ludus.Shared.Systems.Growth;
using LudusGameSettingsComponent = Bitspoke.Ludus.Shared.Components.Settings.Game.LudusGameSettingsComponent;

namespace Bitspoke.Ludus.Shared;

public partial class Find : Bitspoke.GodotEngine.Find
{
    public static GameStateManager GameStateManager => GameStateManager.Instance;

    public static LudusGameSettingsComponent LudusGameSettingsComponent => LudusGameSettingsComponent.Instance;

    public static World CurrentWorld => WorldManager.Instance.CurrentWorld;
    public static World World(ulong mapID)
    {
        Log.TODO("Get from worlds collection.  For now just return current world.  Note we may only ever have 1 world per game/save");
        return CurrentWorld;
    }
    public static Map CurrentMap => CurrentWorld?.Maps?.Current ?? null;
    public static Map Map(ulong? mapID) => CurrentWorld?.Maps?[mapID.Value] ?? null;
        
    public class Common
    {
        public static Circle MaxCircle => new (LudusGameSettingsComponent.Instance.MapSize.x);
    }
        
    public partial class DB
    {
        public static readonly TextureDatabase TextureDB = TextureDatabase.Instance;
        public static readonly ShaderDatabase ShaderDB = ShaderDatabase.Instance;
            
        
        public static readonly DefDB DefDB = DefDB.Instance;

            
        // BIOME
        public static Dictionary<string, BiomeDef> BiomeDefs = DefDB.Get<BiomeDef>();
        public static readonly List<BiomeDef> BiomeDefsList = DefDB.GetAsList<BiomeDef>();
            
        // Map
        public static Dictionary<string, MapGenStepDef> MapGenStepDefs = DefDB.Get<MapGenStepDef>();
        public static readonly List<MapGenStepDef> MapGenStepDefsList = DefDB.GetAsList<MapGenStepDef>();
            
        // Layers
        public static readonly List<IDef> LayerDefsList = DefDB.Values.Where(w => typeof(ILayerDefsCollection).IsAssignableFrom(w.GetType())).ToList();
        public static readonly List<IDef> OrderedLayerDefs = LayerDefsList.OrderBy(o => ((ILayerDefsCollection)o).OrderIndex).ToList();

        // Entities
        public static Dictionary<string, EntityDef> EntityDefs = DefDB.Get<EntityDef>();
        public static List<EntityDef> EntityDefsList = DefDB.GetAsList<EntityDef>();
        // Rocks
        public static readonly Dictionary<string, RockDef> RockDefs = DefDB.Get<RockDef>();
        public static readonly List<RockDef> RockDefsList = DefDB.GetAsList<RockDef>();
        // Plants
        public static Dictionary<string, PlantDef> PlantDefs = DefDB.Get<PlantDef>();
        public static List<PlantDef> PlantDefsList = DefDB.GetAsList<PlantDef>();
        //public static List<PlantDef>? WildPlantDefs = DefDB.Get<PlantDef>(p => p?.IsWild ?? false);
        public static List<PlantDef>? WildPlantDefs = DefDB.Get<PlantDef>().Values.Where(p => p?.PlantDetails.IsWild ?? false).ToList();
            
        // Roof
        public static readonly Dictionary<string, RoofDef> RoofDefs = DefDB.Get<RoofDef>();
        public static readonly List<RoofDef> RoofDefsList = DefDB.GetAsList<RoofDef>();
            
            
        // Terrain
        private static Dictionary<string, TerrainDef>? terrainDefs;
        public static Dictionary<string, TerrainDef> TerrainDefs => terrainDefs ??= DefDB.Get<TerrainDef>();
        private static List<TerrainDef>? terrainDefsList;
        public static List<TerrainDef> TerrainDefsList => terrainDefsList ??= DefDB.GetAsList<TerrainDef>();
            
        public partial class TypeData
        {
            public static TypeDataCollection ElevationTypeData = CoreFind.DB.TypeData.All.GetFromDB<TypeDataCollection>(nameof(ElevationTypeData));
            public static TypeDataCollection VegetationDensityTypeData = CoreFind.DB.TypeData.All.GetFromDB<TypeDataCollection>(nameof(VegetationDensityTypeData));
            
        }
        public class Systems
        {
            public static AgeSystem AgeSystem => AgeSystem.Instance;
            public static GrowthSystem GrowthSystem => GrowthSystem.Instance;
        }
        
    }

    
        
}