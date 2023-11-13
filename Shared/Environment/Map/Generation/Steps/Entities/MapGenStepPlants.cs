using Bitspoke.Core.Common.Collections.Pools;
using Bitspoke.Core.Profiling;
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
        Profiler.Start();
        var spawnSystem = Map.GetSpawnSystem<NaturalPlantSpawnSystem>();
        spawnSystem.CurrentTargetPlantCountForMap = spawnSystem.CalculateTargetPlantCountForMap();
        
        Log.Debug($"Current Target Plant Count for Map: {spawnSystem.CurrentTargetPlantCountForMap}");
        
        var primer = Pool<List<float>>.Instance.New();
        Pool<List<float>>.Instance.Return(primer);
        
        Log.TODO("Add NaturalPlantSpawnSystem.ProcessClusters back in.");

        
        var randomCells = Map.Data.CellsContainer.Cells.RandomisedArray;
        //var randomCells = Map.Cells.Randomised.ToArray();
        
        Profiler.Start(additionalKey:"GetRandomCells");
        var randoms = new List<int>();
        for (int i = 0; i < randomCells.Length; i++)
        {
            if (Rand.Chance(MapGenStepPlantsDef.IndependentSpawnChance)) randoms.Add(i);
        }
        Profiler.End(message:"#### Map.Cells.Randomised ####>", additionalKey:"GetRandomCells");

        Profiler.Start(additionalKey:"ProcessCells");
        
        foreach (var i in randoms)
        {
            //Profiler.Start(additionalKey:"CanSpawnAt");
            //var canSpawnAt = spawnSystem.CanSpawnAt(randomCells[i]);
            var canSpawnAt = spawnSystem.CanSpawnAt2(randomCells[i]);
            //Profiler.End(message:"#### spawnSystem.CanSpawnAt ####>", additionalKey:"CanSpawnAt");

            if (canSpawnAt)
            {
                //plantSpawnLocations++;
            }
            
           //Map.Data.Add(null, randomCells[i]);
        }
        
//        // var randomCells2 = Map.Data.CellsContainer.Cells.RandomisedArray;
        // for (var i = 0; i < randomCells2.Length; i++)
        // {
        //     if (Rand.Chance(MapGenStepPlantsDef.IndependentSpawnChance))
        //     {
        //         spawnSystem.CanSpawnAt2(randomCells2[i]);
        //     }
        // }
        
        
        
        // for (int i = 0; i < randomCells.Length; i++)
        // {
        //     var chance = Rand.Chance(MapGenStepPlantsDef.IndependentSpawnChance); 
        //     if (chance)
        //     {
        //         var canSpawnAt = spawnSystem.CanSpawnAt(randomCells[i]);
        //         if (canSpawnAt)
        //         {
        //             //plantSpawnLocations++;
        //         }
        //     }
        // }
        
        // foreach (var mapCell in randomCells)
        // {
        //     var chance = Rand.Chance(MapGenStepPlantsDef.IndependentSpawnChance); 
        //     if (chance)
        //     {
        //         var canSpawnAt = spawnSystem.CanSpawnAt(mapCell);
        //         if (canSpawnAt)
        //         {
        //             //plantSpawnLocations++;
        //         }
        //     }
        // }
        
        Profiler.End(message:"#### mapCell in randomCells ####>", additionalKey:"ProcessCells");

        if (CoreGlobal.DEBUG_ENABLED)
            Log.Debug($"Total Processing time for SpawnSystem.ProcessClusters: {spawnSystem.debugTotalProcessClustersTime}ms of {Map.Cells.Ordered.Count} cells @ {spawnSystem.debugTotalProcessClustersTime/Map.Cells.Ordered.Count}ms/cell");
        
        
        Profiler.End();
    }

    #endregion




}