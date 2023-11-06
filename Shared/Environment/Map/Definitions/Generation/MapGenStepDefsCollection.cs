using Bitspoke.Core.Definitions.Collections;
using Bitspoke.Core.Definitions.Parts.Common.Noise;
using Bitspoke.GodotEngine.Utils.Files;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation.Entities.Natural;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation.Layers;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation.Structures.Natural;
using Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Entities;
using Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Layers;
using Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Structures.Natural;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation
{
    public class MapGenStepDefsCollection : DefCollection<MapGenStepDef>
    {
        #region Properties

        public override string Key { get; set; } = nameof(MapGenStepDefsCollection);
        public override string ClassName => GetType().FullName;
        public override string? AssemblyName => GetType().Assembly.GetName().Name;

        #endregion

        #region Constructors and Initialisation

        #endregion

        #region Methods

        #endregion

        
        #region Bootstrap

        public static MapGenStepDefsCollection Bootstrap(bool writeToFile = false)
        {
            Profiler.Start();
            
            var defs = new MapGenStepDefsCollection();

            var noiseDef = NoiseDef.Bootstrap(scale:0.5f, bias:0.5f);
            //noiseDef.Frequency = 50f;
            //noiseDef.Lacunarity = 2f;
            noiseDef.Frequency = 0.025f;
            noiseDef.Persistence = 0.25f;
            var orderIndex = 0;

            var elevationLayerDef = new MapGenStepDef {
                Key = $"{nameof(MapGenStepDef)}_Elevation",
                OrderIndex = orderIndex += 100,
                CanParallelProcess = true,
                NoiseDef = noiseDef,
                GenStepClassName = typeof(MapGenStepElevationLayer).FullName,
                GenStepAssemblyName = typeof(MapGenStepElevationLayer).Assembly.GetName().Name,
            };
            defs.Add(elevationLayerDef);
            
            var fertilityLayerDef = new MapGenStepDef {
                Key = $"{nameof(MapGenStepDef)}_Fertility",
                OrderIndex = orderIndex += 100,
                CanParallelProcess = true,
                NoiseDef = noiseDef,
                GenStepClassName = typeof(MapGenStepFertilityLayer).FullName,
                GenStepAssemblyName = typeof(MapGenStepFertilityLayer).Assembly.GetName().Name,
            };
            defs.Add(fertilityLayerDef);

            var rocksLayerDef = new MapGenStepDef {
                Key = $"{nameof(MapGenStepDef)}_Rocks",
                OrderIndex = orderIndex += 100,
                CanParallelProcess = true,
                GenStepClassName = typeof(MapGenStepRocksLayer).FullName,
                GenStepAssemblyName = typeof(MapGenStepRocksLayer).Assembly.GetName().Name
            };
            defs.Add(rocksLayerDef);

            var minElevation = 0.59f;
            var rockFormationsDef = new MapGenStepRockFormationsDef {
                Key = $"{nameof(MapGenStepDef)}_RockFormations",
                OrderIndex = orderIndex += 100,
                GenStepClassName = typeof(MapGenStepRockFormations).FullName,
                GenStepAssemblyName = typeof(MapGenStepRockFormations).Assembly.GetName().Name,
                WaitForAntecedentStepsDelayMs = 25,
                AntecedentStepKeys = new List<string>
                {
                    elevationLayerDef.Key,
                    fertilityLayerDef.Key,
                    rocksLayerDef.Key
                },
                MinElevation = minElevation,
            };
            defs.Add(rockFormationsDef);
            
            var mineralFormationsDef = new MapGenStepMineralFormationsDef {
                Key = $"{nameof(MapGenStepDef)}_Minerals",
                OrderIndex = orderIndex += 100,
                GenStepClassName = typeof(MapGenStepMineralFormations).FullName,
                GenStepAssemblyName = typeof(MapGenStepMineralFormations).Assembly.GetName().Name,
                WaitForAntecedentStepsDelayMs = 25,
                AntecedentStepKeys = new List<string>
                {
                    rockFormationsDef.Key
                },
                MinElevation = minElevation,
            };
            defs.Add(mineralFormationsDef);
            
            var roofsMapGenStepDef = new MapGenStepRoofLayerDef() {
                Key = $"{nameof(MapGenStepDef)}_Roof",
                OrderIndex = orderIndex += 100,
                GenStepClassName = typeof(MapGenStepRoofLayer).FullName,
                GenStepAssemblyName = typeof(MapGenStepRoofLayer).Assembly.GetName().Name,
                WaitForAntecedentStepsDelayMs = 25,
                AntecedentStepKeys = new List<string>
                {
                    elevationLayerDef.Key,
                    rocksLayerDef.Key,
                    rockFormationsDef.Key
                },
                MinElevation = minElevation * 1.05f,
                MinRoofSize = 20,
            };
            defs.Add(roofsMapGenStepDef);
            
            var terrainLayerDef = new MapGenStepDef {
                Key = $"{nameof(MapGenStepDef)}_Terrain",
                OrderIndex = orderIndex += 100,
                GenStepClassName = typeof(MapGenStepTerrainLayer).FullName,
                GenStepAssemblyName = typeof(MapGenStepTerrainLayer).Assembly.GetName().Name,
                WaitForAntecedentStepsDelayMs = 25,
                AntecedentStepKeys = new List<string>
                {
                    elevationLayerDef.Key,
                    fertilityLayerDef.Key,
                    rocksLayerDef.Key,
                    rockFormationsDef.Key
                }
            };
            defs.Add(terrainLayerDef);
            
            var riversDef = new MapGenStepDef {
                Key = $"{nameof(MapGenStepDef)}_Rivers",
                OrderIndex = orderIndex += 100,
                GenStepClassName = typeof(MapGenStepRivers).FullName,
                GenStepAssemblyName = typeof(MapGenStepRivers).Assembly.GetName().Name,
                WaitForAntecedentStepsDelayMs = 25,
                AntecedentStepKeys = new List<string>
                {
                    terrainLayerDef.Key
                }
            };
            defs.Add(riversDef);
            
            var terrainPatchesDef = new MapGenStepDef {
                Key = $"{nameof(MapGenStepDef)}_TerrainPatches",
                OrderIndex = orderIndex += 100,
                GenStepClassName = typeof(MapGenStepTerrainPatches).FullName,
                GenStepAssemblyName = typeof(MapGenStepTerrainPatches).Assembly.GetName().Name,
                WaitForAntecedentStepsDelayMs = 25,
                AntecedentStepKeys = new List<string>
                {
                    terrainLayerDef.Key
                }
            };
            defs.Add(terrainPatchesDef);
            
            var rockChunksDef = new MapGenStepDef {
                Key = $"{nameof(MapGenStepDef)}_RockChunks",
                OrderIndex = orderIndex += 100,
                GenStepClassName = typeof(MapGenStepRockChunks).FullName,
                GenStepAssemblyName = typeof(MapGenStepRockChunks).Assembly.GetName().Name,
                WaitForAntecedentStepsDelayMs = 25,
                AntecedentStepKeys = new List<string>
                {
                    terrainLayerDef.Key
                }
            };
            defs.Add(rockChunksDef);
            
            var plantsDef = new MapGenStepPlantsDef {
                Key = $"{nameof(MapGenStepDef)}_Plants",
                OrderIndex = orderIndex += 100,
                GenStepClassName = typeof(MapGenStepPlants).FullName,
                GenStepAssemblyName = typeof(MapGenStepPlants).Assembly.GetName().Name,
                WaitForAntecedentStepsDelayMs = 25,
                AntecedentStepKeys = new List<string>
                {
                    terrainLayerDef.Key
                },
                IndependentSpawnChance = 0.999f
            };
            defs.Add(plantsDef);
            
            var animalDef = new MapGenStepDef {
                Key = $"{nameof(MapGenStepDef)}_Animals",
                OrderIndex = orderIndex += 100,
                GenStepClassName = typeof(MapGenStepAnimals).FullName,
                GenStepAssemblyName = typeof(MapGenStepAnimals).Assembly.GetName().Name,
                WaitForAntecedentStepsDelayMs = 25,
                AntecedentStepKeys = new List<string>
                {
                    terrainLayerDef.Key
                }
            };
            defs.Add(animalDef);
            
            var fogDef = new MapGenStepDef {
                Key = $"{nameof(MapGenStepDef)}_Fog",
                OrderIndex = orderIndex += 100,
                GenStepClassName = typeof(MapGenStepFogLayer).FullName,
                GenStepAssemblyName = typeof(MapGenStepFogLayer).Assembly.GetName().Name,
                WaitForAntecedentStepsDelayMs = 25,
                AntecedentStepKeys = new List<string>
                {
                    terrainLayerDef.Key
                }
            };
            defs.Add(fogDef);
            
            if (writeToFile)
            {
                var filePath = $"{GodotGlobal.RES_ROOT_PATH}{GodotGlobal.DEFINITIONS_ROOT_PATH}/{nameof(MapGenStepDefsCollection)}{GodotGlobal.SUPPORTED_DEF_TYPE}";
                GodotFileUtils.WriteToFile(filePath, JsonConvert.SerializeObject(defs, Formatting.Indented, CoreGlobal.JsonSerializerSettings));
            }
            
            Profiler.End();
            return defs;
        }

        #endregion

    }
}