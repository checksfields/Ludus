using Bitspoke.Core.Common.Collections.Matrices;
using Bitspoke.Core.Components.Collections.Containers;
using Bitspoke.Core.Components.Identity;
using Bitspoke.Core.Random;
using Bitspoke.Core.Utils.Primatives.Int;
using Bitspoke.GodotEngine.Common.Vector;
using Bitspoke.Ludus.Shared.Entities.Definitions.Structures;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Roof;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Terrain;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.MapCells.Components
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MapCellsContainerComponent : CellBucketContainerComponent<MapCell>
    {

        #region Properties
        
        public override string ComponentName => nameof(MapCellsContainerComponent);
        public IDComponent MapID { get; set; }


        [JsonIgnore] public Dictionary<int, float> Elevations => Container.Collection.Values
            .SelectMany(s => s.Values)
            .Where(w => w.Elevation != 0)
            .Select(s2 => s2)
            .OrderBy(o => o.Index)
            .ToDictionary(d => d.Index, d => d.Fertility);

        [JsonIgnore] public Dictionary<int, float> Fertilities => Container.Collection.Values
            .SelectMany(s => s.Values)
            .Where(w => w.Fertility != 0)
            .Select(s2 => s2)
            .OrderBy(o => o.Index)
            .ToDictionary(d => d.Index, d => d.Fertility);

        [JsonIgnore] public List<TerrainDef?> TerrainDefs => Ordered
            .Select(s => s.Value)
            .Where(w => w.TerrainDef != null)
            .Select(s2 => s2.TerrainDef)
            .ToList();
        
        [JsonIgnore] public Dictionary<int, string?> TerrainDefKeys => TerrainDefs
            .Select(s => s)
            .OrderBy(o => o.Index)
            .ToDictionary(d => d.Index, d => d.Key);
        
        // STRUCTURES **************
        [JsonIgnore] public List<StructureDef?> StructureDefs => Container.Collection.Values
            .SelectMany(s => s.Values)
            .Where(w => w.StructureDef != null)
            .Select(s2 => s2.StructureDef)
            .ToList();
        [JsonIgnore] public Dictionary<int, string?> StructureDefKeys => StructureDefs
            .Select(s => s)
            .OrderBy(o => o.Index)
            .ToDictionary(d => d.Index, d => d.Key);


        // ROOF ********************
        [JsonIgnore] public List<RoofDef?> RoofDefs => Ordered
            .Select(s => s.Value)
            .Where(w => w.RoofDef != null)
            .Select(s2 => s2.RoofDef)
            .ToList();
        [JsonIgnore] public Dictionary<int, string?> RoofDefKeys => RoofDefs
            .Select(s => s)
            .OrderBy(o => o.Index)
            .ToDictionary(d => d.Index, d => d.Key);
        
        // PLANTS
        // [JsonIgnore]
        // public Dictionary<int, List<PlantDef>> PlantDefs => Container.Collection.Values
        //     .SelectMany(s => s)
        //     .Where(w => w.Value.HasPlants)
        //     .ToDictionary(d => d.Key, 
        //         d => d.Value.EntityIDs.Entities(Find.Map(MapID.ID)).EntityList
        //                                     .Where(w2 => w2.GetType() == typeof(Plant))
        //                                     .Select(s2 => ((Plant)s2).Def).Cast<PlantDef>()
        //                                     .ToList());
        //
        // [JsonIgnore]
        // public IEnumerable<Dictionary<PlantDef, List<Plant>>> PlantDefs2 => Container.Collection.Values
        //     .SelectMany(s => s.Values)
        //     .Select(w => w.EntityIDs.PlantsByDef(Find.Map(MapID.ID)));
        
        #endregion

        #region Constructors and Initialisation

        public MapCellsContainerComponent(IDComponent mapID) : base(mapID.ID.FindMap()?.Regions.Area ?? 0)
        {
            MapID = mapID;
            InitCollection();
        }

        #endregion

        #region Methods

        public void InitCollection()
        {
            Profiler.Start();
            
            ContainerWidth = MapID.ID.FindMap()?.Size.Width() ?? 0;
            ContainerHeight = MapID.ID.FindMap()?.Size.Height() ?? 0;
            TotalElements = MapID.ID.FindMap()?.Size.Area() ?? 0;
            
            for (var i = 0; i < TotalElements; i++)
            {
                var cellLocation = i.ToVec2Int(ContainerWidth);
                var mapCell = new MapCell(i, MapID.ID);

                Container.Add(i, mapCell);
            }
            Profiler.End(message:"************ Initialised MapCells");


            Profiler.Start();
            
            // set up the frequently accessed collections
            Ordered = All
                .OrderBy(o => o?.Index)
                .Select(s => s)
                .ToDictionary(d => d?.Index ?? -1, d => d);
            
            Profiler.End(message:"Initialised Accessor Collections");
            
            Profiler.Start();
            Randomised = RandomiseCells();
            Profiler.End(message:"Built Randomised Cells");


            Profiler.Start();
            // generate the neighbour cell matrix
            NeighbourMatrix = new CellNeighbourMatrix();
            foreach (var mapCell in Ordered.Values)
            {
                NeighbourMatrix.AddOrUpdate(mapCell.Index, GetAllNeighbourCellsFor(mapCell));
            }
            Profiler.End(message:"Generated NeighbourMatrix");
        }

        private List<MapCell> RandomiseCells()
        {
            return Ordered.Values.OrderBy(r => Rand.NextInt()).ToList();
            
            // Other options here could include a shuffle function
        }

        #endregion



    }
}