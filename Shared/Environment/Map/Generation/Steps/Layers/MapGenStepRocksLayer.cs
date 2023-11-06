using Bitspoke.Core.Definitions.Parts.Common.Noise;
using Bitspoke.Core.Random;
using Bitspoke.GodotEngine.Common.Noise;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;

namespace Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Layers
{
    public class MapGenStepRocksLayer : MapGenStepLayer
    {
        #region Properties

        private GodotNoise? Noise { get; set; }

        public override string StepName => GetType().Name;
        
        #endregion

        #region Constructors and Initialisation

        public MapGenStepRocksLayer(Map map, MapGenStepDef mapGenStepDef) : base(map, mapGenStepDef)
        {
        }
        
        #endregion

        #region Methods
        
        protected override void StepGenerate()
        {
            Profiler.Start();

            // generate the noises we will need
            var strataNoises = new Dictionary<string, GodotNoise>(); 
            foreach (var availableRockDefKey in Map.MapInitConfig.AvailableRockDefKeys)
            {
                var rockDef = Find.DB.RockDefs[availableRockDefKey];
                Log.Debug($"Processing {rockDef.Key} RockDef");

                var seed = Rand.NextInt();
                Log.Debug($"Seed: {seed}");
                var noise = new GodotNoise(seed, NoiseDef.DEFAULT);
                
                if (CoreGlobal.DEBUG_ENABLED)
                    noise.GenerateImageTexture(325, 325, $"user://Saves/{rockDef.Key}_noise.png");
                
                strataNoises.Add(rockDef.Key, noise);
            }

            // for each cell in the map ...
            var tasks = new List<Task>();
            foreach (var bucket in Map.Cells.Buckets)
            {
                tasks.Add(Task.Run(() =>
                {
                    foreach (var mapCell in bucket.Values)
                    {
                        string? stratumDefKey = null;
                        float stratumDefValue = float.MinValue;
                        foreach (var defNoise in strataNoises)
                        {
                            var noiseValue = defNoise.Value.GetValue(mapCell.Location);
                            if (noiseValue > stratumDefValue)
                            {
                                stratumDefKey = defNoise.Key;
                                stratumDefValue = noiseValue;
                            }
                        }

                        mapCell.Stratum = stratumDefKey;
                        mapCell.Values.Add("Stratum", mapCell.Stratum);
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());

            Profiler.End();
        }

        #endregion


        

        
    }
}