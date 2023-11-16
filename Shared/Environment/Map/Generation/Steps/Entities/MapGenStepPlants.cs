using Bitspoke.Core.Common.Collections.Pools;
using Bitspoke.Core.Random;
using Bitspoke.Ludus.Shared.Entities.Systems.Spawn.Natural.Plants;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation.Entities.Natural;

namespace Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Entities;

public class MapGenStepPlants : MapGenStep
{
    #region Properties

    public override string StepName => nameof(MapGenStepPlants);

    public MapGenStepPlantsDef MapGenStepPlantsDef => (MapGenStepPlantsDef) MapGenStepDef;
    
    #endregion

    #region Constructors and Initialisation

    public MapGenStepPlants(Map map, MapGenStepDef mapGenStepDef) : base(map, mapGenStepDef) { }
    
    #endregion

    #region Methods

    protected override void StepGenerate()
    {
        
        var spawnSystem = Map.GetSpawnSystem<NaturalPlantSpawnSystem>();
        spawnSystem.CurrentTargetPlantCountForMap = spawnSystem.CalculateTargetPlantCountForMap();
        
        Log.Debug($"Current Target Plant Count for Map: {spawnSystem.CurrentTargetPlantCountForMap}");
        
        var primer = Pool<List<float>>.Instance.New();
        Pool<List<float>>.Instance.Return(primer);
        
        var randomCells = Map.Data.CellsContainer.Cells.RandomisedArray;
        
        // TODO: Alternative Processing - remove when happy with other versions
        // **** 20231114 Benchmark Run 1 = 6431 ms
        // **** 20231114 Benchmark Run 2 = 6360 ms
        // **** 20231114 Benchmark Run 3 = 6455 ms
        // Profile(message: "**** Total CanSpawnAt in loop of randoms[]", toProfile:() => {
        //     var randoms = new List<int>();
        //     for (int i = 0; i < randomCells.Length; i++)
        //     {
        //         if (Rand.Chance(MapGenStepPlantsDef.IndependentSpawnChance)) randoms.Add(i);
        //     }
        //     
        //     foreach (var i in randoms)
        //     {
        //             var canSpawnAt = spawnSystem.CanSpawnAt(randomCells[i]);
        //
        //             if (canSpawnAt) {
        //                 //plantSpawnLocations++;
        //             }
        //     }
        // });

        // TODO: Alternative Processing - remove when happy with other versions
        // // **** 20231114 Benchmark Run 1 = 7616 ms
        // // **** 20231114 Benchmark Run 2 = 7285 ms
        // Profile(message: "**** Total CanSpawnAt in loop of randomCellsInBuckets", toProfile: () => {
        //     var cellBuckets = Map.Data.CellsContainer.GetCellBucket(GodotGlobal.CPU_CORES);
        //     foreach (var cells in cellBuckets.Values)
        //     {
        //             var randomCellInBucket = cells.RandomisedArray;
        //             foreach (var randomCell in randomCellInBucket) {
        //                 if (Rand.Chance(MapGenStepPlantsDef.IndependentSpawnChance))
        //                 {
        //                     var canSpawnAt = spawnSystem.CanSpawnAt(randomCell);
        //                     if (canSpawnAt)
        //                     {
        //                         //plantSpawnLocations++;
        //                     }
        //                 }
        //             }
        //     }
        // });
        
        // **** 20231114 Benchmark Run 1 = 6690 ms
        // **** 20231114 Benchmark Run 2 = 8054 ms
        // **** 20231114 Benchmark Run 3 = 7110 ms
        // **** 20231114 Benchmark Run 4 = 5931 ms
        // **** 20231114 Benchmark Run 5 = 6411 ms
        Profile(message: "**** Total CanSpawnAt in loop of randomCells", toProfile: () => {
            foreach (var randomCell in randomCells) {
                if (Rand.Chance(MapGenStepPlantsDef.IndependentSpawnChance))
                {
                    var canSpawnAt = spawnSystem.CanSpawnAt(randomCell);
                    if (canSpawnAt)
                    {
                        //plantSpawnLocations++;
                    }
                }
            }
        });
     }

    #endregion




}