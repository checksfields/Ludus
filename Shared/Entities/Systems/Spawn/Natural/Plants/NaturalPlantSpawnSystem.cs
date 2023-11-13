using Bitspoke.Core.Common.Collections.Pools;
using Bitspoke.Core.Common.Maths.Geometry;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Profiling;
using Bitspoke.Core.Utils.Collections;
using Bitspoke.Core.Utils.Math;
using Bitspoke.Core.Utils.Primatives.Float;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Common.Reports;
using Bitspoke.Ludus.Shared.Entities.Containers.Extensions;
using Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;
using Bitspoke.Ludus.Shared.Entities.Natural.Plants;
using Bitspoke.Ludus.Shared.Environment.Map;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells;
using Bitspoke.Ludus.Shared.Environment.Map.Regions;
using Godot;
using Range = Bitspoke.Core.Common.Values.Range;

namespace Bitspoke.Ludus.Shared.Entities.Systems.Spawn.Natural.Plants;

public class NaturalPlantSpawnSystem : NaturalEntitySpawnSystem<Plant>
{
    #region Properties

    public float BiomePlantDensity => Find.DB.BiomeDefs[Map.MapInitConfig.BiomeKey]?.BiomePlantsDef?.Density ?? 0.7f;
    // TODO: plant density can change based on the current game environment / conditions ... summer v's winter for eg
    public float GameConditionsPlantDensityModifier => 1f;
    public float CurrentPlantDensity => BiomePlantDensity * GameConditionsPlantDensityModifier;
    public float? CurrentTargetPlantCountForMap { get; set; }

    public double CellsInRadiusToMapAreaRatio { get; set; }

    // these lists are reused in a method called in an iterative method ... 
    // given the lists are small it is best to clear them in the method rather than instantiating new 
    // lists: 
    // https://stackoverflow.com/questions/10901020/which-is-faster-clear-collection-or-instantiate-new
    public Dictionary<Def, List<float>> DistancesOfNearbySimilarPlants { get; set; }
    public Dictionary<Def,float> PlantDefMedianDistanceToSimilarPlants { get; set; }

    public double debugTotalProcessClustersTime = 0;

    //public Dictionary<string, FrequencyDef>? FrequencyDefs { get; set; } = null;
    
    #endregion

    #region Constructors and Initialisation

    public NaturalPlantSpawnSystem(Map map)
    {
        Map = map;
        DistancesOfNearbySimilarPlants = new Dictionary<Def, List<float>>();
        PlantDefMedianDistanceToSimilarPlants = new Dictionary<Def, float>();
        
        
        CellsInRadiusToMapAreaRatio = Circle.CellsInRadius(20) / (double)Map.Area;
    }
    
    #endregion

    #region Methods
    
    public override bool CanSpawnAt(MapCell mapCell)
    {
        bool isValid = Validate(mapCell);
        if (!isValid)
            return false;
        
        if (IsSaturatedAt(mapCell))
            return false;
        
        // get the list of possible plants that can spawn here
        var plantDefs = CandidatePlantDefsFor(mapCell);
        
        // if we have none ... just return false ... can't spawn here
        if (plantDefs == null || plantDefs.Count < 1)
            return false;

        // TODO: Performance Bottle Neck - Removed for now
        Profiler.Start();
        // handle any clusters
        // TODO: Add back in
        ProcessClusters(mapCell);
        debugTotalProcessClustersTime += Profiler.End(log:false);

        // calculate the weights for only the plants that can exist here
        var defWeights = PlantSelectionWeights(mapCell, plantDefs);
        
        var result = new KeyValuePair<Def, float>();
        var found = defWeights.TryRandomElementByWeight((x => x.Value), out result);

        if (!found)
            return false;

        var plant = new Plant((PlantDef) result.Key);

        plant.LocationComponent.Index = mapCell.Index;
        plant.LocationComponent.Location = mapCell.Location;
        
        //Map.Plants.Add(plant);
        Map.Entities.Add(plant, mapCell);
        //mapCell.GenericEntityContainer.Add(Map.Plants[plant.IDComponent.ID]);
        //mapCell.EntityIDs.Add(plant.IDComponent.ID, EntityType.Plant);

        plant = null;
        //Map.Plants.Remove(plant);

        // TODO: Fix
        return true;
    }
    
    public bool CanSpawnAt2(MapCell mapCell)
    {
        bool isValid = Validate(mapCell);
        if (!isValid)
            return false;
        
        if (IsSaturatedAt(mapCell))
            return false;
        
        // get the list of possible plants that can spawn here
        var plantDefs = CandidatePlantDefsFor(mapCell);
        
        // if we have none ... just return false ... can't spawn here
        if (plantDefs == null || plantDefs.Count < 1)
            return false;

        // TODO: Performance Bottle Neck - Removed for now
        Profiler.Start();
        // handle any clusters
        // TODO: Add back in
        ProcessClusters(mapCell);
        debugTotalProcessClustersTime += Profiler.End(log:false);

        // calculate the weights for only the plants that can exist here
        var defWeights = PlantSelectionWeights(mapCell, plantDefs);
        
        var result = new KeyValuePair<Def, float>();
        var found = defWeights.TryRandomElementByWeight((x => x.Value), out result);

        if (!found)
            return false;

        var plant = new Plant((PlantDef) result.Key);

        plant.LocationComponent.Index = mapCell.Index;
        plant.LocationComponent.Location = mapCell.Location;
        
        //Map.Plants.Add(plant);
        Map.Data.EntitiesContainer.Add(plant, mapCell);
        //mapCell.GenericEntityContainer.Add(Map.Plants[plant.IDComponent.ID]);
        //mapCell.EntityIDs.Add(plant.IDComponent.ID, EntityType.Plant);

        plant = null;
        //Map.Plants.Remove(plant);

        // TODO: Fix
        return true;
    }

    private Dictionary<Def, float> PlantSelectionWeights(MapCell mapCell, List<PlantDef>? plantDefs)
    {
        var weight = 0f;
        var defWeights = new Dictionary<Def, float>();
        foreach (var plantDef in plantDefs)
        {
            weight = CalculatePlantSelectionWeight(mapCell, plantDef);
            defWeights.Add(plantDef, weight);
        }

        return defWeights;
    }

    public float CalculatePlantSelectionWeight(MapCell mapCell, PlantDef plantDef)
    {
        var weight = 0f;

        var plantWeight = Map.BiomeDef.BiomePlantsDef.PlantWeights[plantDef.Key]?.Frequency ?? 0f;
        if (plantWeight <= 0f)
            return weight;
        
        var plantWeightPercent = plantWeight / Map.BiomeDef.BiomePlantsDef.TotalWeight;
        
        var plantsByDef = Map.Plants.PlantsByDef();
        //var plantDefs = Map.Cells.PlantDefs();
        var currentPlantsOfDef = plantsByDef.ContainsKey(plantDef) ? plantsByDef[plantDef].Count : 0;
        //var currentPlantsOfDef = plantsByDef?[plantDef]?.Count ?? 0;
        var currentTotalPlants = Map.Plants.Count;

        var r = 0.5f;
        // if 50% of the plants are spawned ...
        if (currentTotalPlants > CurrentTargetPlantCountForMap / 2f)
        {
            r = currentPlantsOfDef / currentTotalPlants / plantWeightPercent;
            plantWeight *= r;
        }

        plantWeight = CalculatePlantSectionWeightForClusterDistribution(plantWeight, plantDef, r);
        plantWeight = CalculatePlantSelectionWeightForNormalDistribution(plantWeight);
        weight = plantWeight;

        return weight;
    }

    private float CalculatePlantSectionWeightForClusterDistribution(float plantWeight, PlantDef plantDef, float r)
    {
        if (plantDef.CanCluster && r < 1.10000000000001d)
            return plantWeight;

        var totalWeight = Map.BiomeDef.BiomePlantsDef.TotalWeight;
        var plantWeightPercent = plantWeight / totalWeight;
        var totalOtherWeight = totalWeight - plantWeight;
        var plantClustrerWeight = plantWeight * plantDef.ClusterDef.Wieght;
        var clusterWeight = totalOtherWeight + plantClustrerWeight;
        var commonality = (double) plantWeight * plantDef.ClusterDef.Wieght / clusterWeight;
        
        
        
        float outTo1 = (float) (1.0 / Math.PI * Math.Pow(plantDef.ClusterDef.Radius, 2));
        float outTo2 = new Range(plantWeightPercent, 1f).Lerp(new Range(1f, outTo1), (float) commonality);

        var medianDistance = 0f;
        if (PlantDefMedianDistanceToSimilarPlants.TryGetValue(plantDef, out medianDistance))
        {
            var squareRootMedianDistance = medianDistance.Sqrt();
            var range = new Range(plantDef.ClusterDef.Radius * 0.9f, plantDef.ClusterDef.Radius * 1.1f);
            var lerpRange = new Range(plantDef.ClusterDef.Wieght, outTo2);
            plantWeight = range.Lerp(lerpRange, squareRootMedianDistance);
        }
        else
            plantWeight *= outTo2; 
        
        
        
        return plantWeight;
    }

    private float CalculatePlantSelectionWeightForNormalDistribution(float plantWeight)
    {
        // TODO: Implement
        return plantWeight;
    }

    private bool Validate(MapCell mapCell)
    {
        // validation check
        var densityOk = CurrentPlantDensity > 0.0;
        var hasPlant = mapCell.HasPlants;
        var hasCover = false; // c.GetCover(this.map) != null
        var hasStructure = mapCell.HasNaturalStructure;
        var notFertile = mapCell.Fertility <= 0.0;
        
        // TODO: add additional checks as needed

        if (!densityOk || hasPlant || hasCover || hasStructure || notFertile)
            return false;

        
        
        return true;
    }

    private bool IsSaturatedAt(MapCell mapCell)
    {
        
        if (  (CurrentTargetPlantCountForMap * CellsInRadiusToMapAreaRatio <= 4.0) 
            || Map.BiomeDef.BiomePlantsDef.WildPlantsIgnoreFertility)
        {
            // how many plants are already on the map ... 
            var totalMapPlants = Map.Plants.Count;
            // we are saturated if we have equal to or more plants on the map than we are targeting
            var mapIsSaturated = totalMapPlants >= CurrentTargetPlantCountForMap;
            
            if (CoreGlobal.DEBUG_ENABLED && mapIsSaturated)
                Log.Info($"MapCell[{mapCell.Index}]: Map is saturated with plants.");
            
            return mapIsSaturated;
        }

        // TODO: Fine grain Region saturation
        
        
        float numDesiredPlantsLocally = 0.0f;
        int numPlants = 0;
        
        
        // if this region saturated
        var mapRegion = Map.Regions.GetRegion(mapCell.Location.ToVec3Int());
        //var mapRegion = Map.Regions.MapRegions[mapCell.RegionIndex];
        numDesiredPlantsLocally = GetDesiredPlantsCountIn(mapRegion, mapCell, CurrentPlantDensity);
        //numPlants += mapRegion.EntitiesBy(EntityType.Plant).Count;
        
        
        
        var plants = Map.GetEntities<Plant>(EntityType.Plant);
        numPlants += mapRegion.EntitiesBy(EntityType.Plant).Count;

        return (double)numPlants >= (double)numDesiredPlantsLocally;
    }
    
    public float GetDesiredPlantsCountIn(Region reg, MapCell mapCell, float plantDensity)
    {
        return Mathf.Min((float)(reg.GetDesiredPlantsCount() * plantDensity * mapCell.Fertility), (float)reg.Dimension.Area);
    }


    private List<PlantDef>? ClusteredPlantDefs = new();
    // get the cells in any potential cluster 
    private void ProcessClusters(MapCell mapCell)
    {
        // these lists are reused in a method called in an iterative method ... 
        // given the lists are small it is best to clear them in the method rather than instantiating new 
        // lists: 
        // https://stackoverflow.com/questions/10901020/which-is-faster-clear-collection-or-instantiate-new
        DistancesOfNearbySimilarPlants.Clear();
        //DistancesOfNearbySimilarPlantsList.Clear();
        PlantDefMedianDistanceToSimilarPlants.Clear();
        ClusteredPlantDefs.Clear();

        var cellRadius = Map.BiomeDef?.BiomePlantsDef.MaxClusterRadius.Ceiling() ?? 0;
        var cellsInRadius = Circle.MAX.GetCellCountForRadius(cellRadius);

        // for each cell in the radius ...
        //Profiler.Start();
        for (int index = 0; index < cellsInRadius; ++index)
        {
            // ... map it to the cell we are processing
            var mapPosition = mapCell.Location + Circle.MAX.Cells[index].ToVec2Int();
            if (mapPosition.IsInBounds(Map.Width))
            {
                // cell is in bounds ... find it in the map's cell container
                var cellIndex = mapPosition.ToIndex(Map.Width);
                
                // for any entities in this target cell that are wild plants and are clustered ...
                //foreach (var entity in targetCell.EntityContainer.WildClusteredPlants()!)
                var cellEntities = Map.Entities.GetByCellIndex(cellIndex); 
                ClusteredPlantDefs = cellEntities?.PlantDefs(true, true) ?? new();
                if (ClusteredPlantDefs == null)
                    continue;
                
                foreach (var def in ClusteredPlantDefs)
                {
                    //Profiler.Start();
                    // how far are the cells apart
                    var distanceSquared = mapCell.Location.ToVec3Int().DistanceTo(mapPosition.ToVec3Int());
                    //Profiler.End(message:"1");

                    //Profiler.Start();
                    var distances = DistancesOfNearbySimilarPlants.ContainsKey(def) ? DistancesOfNearbySimilarPlants[def] : null;
                    //Profiler.End(message:"2");

                    //Profiler.Start();
                    if (distances == null)
                    {
                        distances = Pool<List<float>>.Instance.New();
                        DistancesOfNearbySimilarPlants.Add(def, distances);
                        //DistancesOfNearbySimilarPlantsList.Add(new(def, distances));
                    }
                    //Profiler.End(message:"3");

                    //Profiler.Start();
                    distances.Add(distanceSquared);
                    //Profiler.End(message:"4");
                    //Log.Error("****************");

                }
            }
        }
        //Profiler.End(message:$"Processed {cellsInRadius} cellsInRadius");
        
        foreach (var distancesOfNearbySimilarPlant in DistancesOfNearbySimilarPlants)
        {
            var distances = distancesOfNearbySimilarPlant.Value;
            distances.Sort();
            PlantDefMedianDistanceToSimilarPlants.Add(distancesOfNearbySimilarPlant.Key, distances[distances.Count/2]);
            distances.Clear();
            Pool<List<float>>.Instance.Return(distances);
        }

        // for (var index = 0; index < DistancesOfNearbySimilarPlantsList.Count; index++)
        // {
        //     var distanceAndDef = DistancesOfNearbySimilarPlantsList[index];
        //     var distanceList = distanceAndDef.Value;
        //     distanceList.Sort();
        //     
        //     distanceAndDef = DistancesOfNearbySimilarPlantsList[index];
        //     var key = distanceAndDef.Key;
        //     var dist = distanceList[distanceList.Count / 2];
        //     
        //     PlantDefMedianDistanceToSimilarPlants.Add(key, dist);
        //     distanceList.Clear();
        //     Pool<List<float>>.Instance.Return(distanceList);
        // }
    }

    private List<PlantDef> CandidatePlantDefsFor(MapCell mapCell)
    {
        var plantDefs = new List<PlantDef>();

        // plants supported by the biome
        var defs = Map.BiomeDef.BiomePlantsDef.PlantWeights;
        foreach (var vegetationDef in defs)
        {
            var plantDef = Find.DB.PlantDefs[vegetationDef.Key];
            var placementReport = plantDef.CanPlaceAt(mapCell);
            
            if (placementReport.Status == ReportStatus.Accepted)
                plantDefs.Add(plantDef);
        }

        return plantDefs;
    }

    public float CalculateTargetPlantCountForMap()
    {
        Profiler.Start();
        var targetPlantCount = 0f;
        var totalCellsWithNoPlants = 0;
        
        foreach (var mapCell in Map.Cells.Ordered.Values)
        {
            var cellPlantCount = CalculatePlantDensityAt(mapCell);
            
            if (CoreGlobal.DEBUG_ENABLED)
                if (cellPlantCount <= 0) totalCellsWithNoPlants++;
            
            
            targetPlantCount += cellPlantCount;
        }
        
        Profiler.End();
        
        Log.Debug($"Total MapCells with 0 Plants: {totalCellsWithNoPlants}");
        
        return targetPlantCount;
    }

    private float CalculatePlantDensityAt(MapCell cell)
    {
        var cellFertility = cell.TerrainDef?.Fertility ?? 0f;
        cellFertility *= cellFertility;
        
        // return the min of 1f and the calc density ... can't have a density > 1f
        return Math.Min(cellFertility * CurrentPlantDensity, 1f);
    }
    
    #endregion


    
}