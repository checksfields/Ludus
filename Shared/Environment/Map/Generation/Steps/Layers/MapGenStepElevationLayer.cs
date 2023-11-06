using Bitspoke.Core.Random;
using Bitspoke.GodotEngine.Common.Noise;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;
using Bitspoke.Ludus.Shared.Environment.World.TypeData;

namespace Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Layers
{
    public class MapGenStepElevationLayer : MapGenStepLayer
    {
        #region Properties

        private GodotNoise? Noise { get; set; }
        private string ElevationTypeDataKey { get; set; }
        private float ElevationModifier { get; set; }

        public override string StepName => GetType().Name;
        
        #endregion

        #region Constructors and Initialisation

        public MapGenStepElevationLayer(Map map, MapGenStepDef mapGenStepDef) : base(map, mapGenStepDef)
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
                Noise.GenerateImageTexture(325, 325, $"{GodotGlobal.SAVE_ROOT_PATH}/elevation.png");
            
            ElevationTypeDataKey = Map.MapInitConfig.ElevationTypeDataKey;
            ElevationModifier = Find.TypeData.ElevationTypeData[ElevationTypeDataKey].GetValue<float>();

            var tasks = new List<Task>();
            foreach (var bucket in Map.Cells.Buckets)
            {
                tasks.Add(Task.Run(() =>
                {
                    foreach (var mapCell in bucket.Values)
                    {
                        var elevationNoiseValue = Noise.GetValue(mapCell.Location, GetElevationValueFunc);
                        mapCell.Elevation = elevationNoiseValue;
                        lock (mapCell.Values)
                            mapCell.Values.Add("Elevation", mapCell.Elevation);
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());

            Profiler.End();
        }
        
        private float GetElevationValueFunc(int x, int y, float value)
        {
            // scale and bias
            value *= Noise.Scale;
            value += Noise.Bias;
            
            // multiply
            value *= ElevationModifier;
            
            if (ElevationTypeDataKey == ElevationTypeData.MOUNTAINS_KEY ||
                ElevationTypeDataKey == ElevationTypeData.LARGE_MOUNTAINS_KEY)
            {
                // TODO: Implement
            }
            
            return value;
        }
        
        
        #endregion


        

        
    }
}