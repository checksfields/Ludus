using Bitspoke.Core.Random;
using Bitspoke.GodotEngine.Common.Noise;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;

namespace Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Layers
{
    public class MapGenStepFertilityLayer : MapGenStepLayer
    {
        #region Properties

        public override string StepName => GetType().Name;
        private GodotNoise Noise { get; set; }

        #endregion

        #region Constructors and Initialisation
        
        public MapGenStepFertilityLayer(Map map, MapGenStepDef mapGenStepDef) : base(map, mapGenStepDef)
        {
        }
        
        #endregion

        #region Methods
        
        protected override void StepGenerate()
        {
            Profiler.Start();
            
            var seed = Rand.NextInt(int.MaxValue);
            Log.Debug($"Seed: {seed}");
            Noise = new GodotNoise(seed, MapGenStepDef.NoiseDef);
            //Noise = new GodotNoise(seed, NoiseDef.DEFAULT);
            
            if (CoreGlobal.DEBUG_ENABLED)
                Noise.GenerateImageTexture(325, 325, $"{GodotGlobal.SAVE_ROOT_PATH}/fertility.png");
            
            var tasks = new List<Task>();
            foreach (var bucket in Map.Cells.Buckets)
            {
                tasks.Add(Task.Run(() =>
                {
                    foreach (var mapCell in bucket.Values)
                    {
                        var fertilityNoiseValue = Noise.GetValue(mapCell.Location, GetFertilityValueFunc);
                        mapCell.Fertility = fertilityNoiseValue;
                        lock (mapCell.Values)
                            mapCell.Values.Add("Fertility", mapCell.Fertility);
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            
            Profiler.End();
        }
        
        private float GetFertilityValueFunc(int x, int y, float value)
        {
            // scale and bias
            value *= Noise.Scale;
            value += Noise.Bias;

            return value;
        }
        
        #endregion


        

        
    }
}