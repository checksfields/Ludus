using Bitspoke.Core.Common.Vector;
using Bitspoke.Core.Entities.Containers;
using Bitspoke.Core.Models.Cells;
using Bitspoke.Core.Utils.Primatives.Int;
using Bitspoke.GodotEngine.Common.Vector;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Entities.Containers;
using Bitspoke.Ludus.Shared.Entities.Containers.Extensions;
using Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;
using Bitspoke.Ludus.Shared.Entities.Definitions.Structures;
using Bitspoke.Ludus.Shared.Entities.Definitions.Structures.Natural;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Roof;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Terrain;
using Bitspoke.Ludus.Shared.Environment.Map.Regions;
using Bitspoke.Ludus.Shared.Environment.Map.Regions.Components;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.MapCells
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MapCell : Cell
    {

        #region Properties
        
        [JsonIgnore] public int RegionIndex { get; set; }
        [JsonIgnore] public Region? Region => Map?.Data.RegionsContainer[RegionIndex] ?? null; 

        
        [JsonIgnore] public int MapID { get; set; }
        [JsonIgnore] public Map? Map => Find.Map(MapID);

        public float Elevation { get; set; }
        public float Fertility { get; set; }
        public string Stratum { get; set; }

        public bool IsValid => Validate();
        
        public Dictionary<string, object?> Values { get; set; }

        [JsonIgnore] public TerrainDef? TerrainDef { get; set; }
        public string? TerrainDefKey => TerrainDef?.Key ?? null;

        [JsonIgnore] public StructureDef StructureDef { get; set; }
        public string? StructureDefKey => StructureDef?.Key ?? null;
        [JsonIgnore] public bool HasNaturalStructure => !string.IsNullOrEmpty(StructureDefKey) && StructureDef is NaturalStructureDef;
        
        [JsonIgnore] public RoofDef? RoofDef { get; set; }
        public string? RoofDefKey => RoofDef?.Key ?? null;

        //public EntityIDContainer<EntityType> EntityIDs { get; set; }
        public EntitiesContainer<LudusEntity>? EntityContainer => Map.Entities.GetByCellIndex(Index);
        public List<LudusEntity>? Entities => EntityContainer?.EntitiesList ?? null;
        // public EntityContainer<LudusEntity>? EntityContainer => Map.Entities.GetByCellIndex(Index);
        public EntitiesContainer? EntitiesNew { get; set; } = new();

        
        //public Dictionary<int, EntityType> EntityIDs { get; set; }
        public bool HasPlants => Entities?.HasPlant() ?? false;

        //[JsonIgnore] public GenericEntityContainer GenericEntityContainer { get; set; }
        // public List<int>? EntityIDs => GenericEntityContainer?.EntityIDs ?? null;

        public List<PlantDef> GetPlantsDefs(bool isWild = true, bool isClusterable = false)
        {
            var plantDefsToReturn = new List<PlantDef>();
            var plantDefs = Entities.PlantDefs();
            foreach (var plantDef in plantDefs)
            {
                
                var addTo = false;
                addTo |= (isWild && plantDef.IsWild);
                addTo |= (isClusterable && plantDef.CanCluster);
                plantDefsToReturn.Add(plantDef);
            }
            
            return plantDefsToReturn;
        }
        
        #endregion

        #region Constructors and Initialisation

        public MapCell(int index, int mapID) : base()
        {
            Index = index;
            MapID = mapID;
            Location = index.ToVec2Int(Map.Width);
            RegionIndex = Map.Data?.RegionsContainer?.GetRegionByCellLocation(Location.ToVector2I())?.Index ?? -1;
            
            Init();
        }

        private void Init()
        {
            Values = new Dictionary<string, object?>();
        }
        
        #endregion

        #region Methods
        

        public override int GetHashCode()
        {
            return Index;
        }

        public override string ToString()
        {
            return $"Index: {Index}, Elevation: {Elevation}, Fertility: {Fertility}, HasNaturalStructure: {HasNaturalStructure}";
        }

        public bool Validate()
        {
            // TODO: Implement
            //Log.TODO("Implement");
            return true;
        }

        #endregion


    }
}