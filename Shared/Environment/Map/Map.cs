using Bitspoke.Core.Common.Collections;
using Bitspoke.Core.Common.Vector;
using Bitspoke.Core.Components.Identity;
using Bitspoke.Core.Entities.Containers;
using Bitspoke.GodotEngine.Common.Vector;
using Bitspoke.GodotEngine.Utils.Vector;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Entities.Natural.Plants;
using Bitspoke.Ludus.Shared.Entities.Systems.Spawn.Natural.Plants;
using Bitspoke.Ludus.Shared.Environment.Biome.Definitions;
using Bitspoke.Ludus.Shared.Environment.Map.Components;
using Bitspoke.Ludus.Shared.Environment.Map.Entities.Components;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells.Components;
using Bitspoke.Ludus.Shared.Environment.Map.Regions.Components;
using Bitspoke.Ludus.Shared.Systems.Spawn;
using Godot;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map
{
    public class Map : LudusEntity
    {
        #region Properties

        public override string EntityName => $"{nameof(Map)}_{MapID.ID}";
        
        public MapInitConfig MapInitConfig { get; set; } = null;
        [JsonIgnore] public Vector2I Size => MapInitConfig?.Size ?? Vector2I.Zero;
        [JsonIgnore] public int Width => Size.Width();
        [JsonIgnore] public int Height => Size.Height();
        [JsonIgnore] public int Area => Width * Height;

        public int TotalCells => Area;
        public int TotalRegions => CalculateTotalRegions();

        [JsonIgnore] public int Seed => MapInitConfig?.SeedPart ?? 0;
        
        public BiomeDef? BiomeDef { get; set; } = null;

        [JsonIgnore] public IDComponent MapID => base.IDComponent;
        [JsonIgnore] public IDComponent WorldID => MapInitConfig?.WorldID ?? IDComponent.DEFAULT_ENTITY_ID;
        [JsonIgnore] public MapCellsContainerComponent  Cells    => Components.Get<MapCellsContainerComponent>();
        [JsonIgnore] public MapEntityContainerComponent Entities => Components.Get<MapEntityContainerComponent>();
        [JsonIgnore] public MapRegionsComponent         Regions  => Components.Get<MapRegionsComponent>();
        
        public MapDataCollectionComponent Data { get; protected set; }
        
        [JsonIgnore] public Dictionary<string, SpawnSystem> SpawnSystems { get; set; }
        
        // COMMON ENTITY ACCESSORS
        [JsonIgnore] public Dictionary<EntityType, IEntityContainer> CommonEntities { get; set; }

        [JsonIgnore] public List<LudusEntity> AllCommonEntities => CommonEntities.Values.SelectMany(s => s.EntityList).Cast<LudusEntity>().ToList();
        
        [JsonIgnore] public EntitiesContainer<Plant>? Plants => CommonEntities[EntityType.Plant] as EntitiesContainer<Plant>;
        //[JsonIgnore] public Dictionary<int, List<PlantDef>> PlantDefs => Cells.PlantDefs;

        [JsonIgnore] public BucketCollection<int, LudusEntity> EntityCollection { get; set; }

        #endregion

        #region Constructors and Initialisation

        public Map(MapInitConfig initConfig) : base()
        {
            MapInitConfig = initConfig;

            var world = WorldID.ID.FindWorld();
            if (world != null)
                world.Maps[MapID.ID] = this;
            
            BiomeDef = Find.DB.BiomeDefs.Get<BiomeDef>(initConfig.BiomeKey);
            AddComponentsPostConstructor();
            AddSystems();
                
            Data = new MapDataCollectionComponent(this);
            Data.InitialiseContainers();
        }

        protected override void Init()
        {
            CommonEntities = new Dictionary<EntityType, IEntityContainer>();
            CommonEntities.Add(EntityType.Plant, new EntitiesContainer<Plant>());
        }
        protected override void ConnectSignals() { }

        public override void AddComponents()
        {
            Components.Add(new DataComponent());
        }
        
        public void AddComponentsPostConstructor()
        {
            Components.Add(new MapRegionsComponent(MapID));
            Components.Add(new MapCellsContainerComponent(MapID));

            
            Components.Add(new MapEntityContainerComponent(MapID));
        }
        
        private void AddSystems()
        {
            SpawnSystems = new System.Collections.Generic.Dictionary<string, SpawnSystem>();
            SpawnSystems.Add(nameof(NaturalPlantSpawnSystem), new NaturalPlantSpawnSystem(this));
        }

        #endregion

        #region Methods

        public T GetSpawnSystem<T>() where T : SpawnSystem
        {
            return (T) SpawnSystems[typeof(T).Name];
        }

        public EntitiesContainer<T> GetEntities<T>(EntityType type) where T : LudusEntity
        {
            var key = typeof(T).Name;
            //return (List<T>) CommonEntities[type];
            return (EntitiesContainer<T>) CommonEntities[type];
        }
        
        public ImageTexture GenerateTerrainTexture(Rect2 area)
        {
            //Profiler.Start();
            // Setup dimensions
            var start = area.Position.ToVec2Int();
            var size = area.Size.ToVec2Int();

            // Create image
            Image img = new Image();
            var orderIndexArray = Cells.TerrainDefs.Select(s => (byte) s.OrderIndex).ToArray();
            List<byte> data = new List<byte>();
            
            for (int y = start.y; y < start.y + size.y; y++)
            {
                for (int x = start.x; x < start.x + size.x; x++)
                {
                    var cell = new Vec2Int(x, y);
                    if (cell.IsInBounds(Width))
                    {
                        var index = cell.ToIndex(Width);
                        data.Add(orderIndexArray[index] > 5 ? (byte) 4 : (byte) orderIndexArray[index]);
                    }
                }
            }
            
            img = Image.CreateFromData((int)size.x, (int)size.y, false, Image.Format.R8, data.ToArray());
            //img.SavePng($"user://mapData_{area.GetHashCode()}.png");
            
            // for (var i = 0; i < orderIndexArray.Length; i++)
            // {
            //     if (orderIndexArray[i] > 5) orderIndexArray[i] = 4;
            // }
            //
            // img.CreateFromData((int)size.x, (int)size.y, false, Image.Format.R8, orderIndexArray);
            // img.SavePng("user://mapData.png");

            ImageTexture texture = ImageTexture.CreateFromImage(img);
            //Profiler.End();
            return texture;
        }
        
        private int CalculateTotalRegions()
        {
            // TODO: Fix me
            return 0;
        }
        
        #endregion
        
    }
}