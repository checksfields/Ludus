using Bitspoke.Core.Random;
using Bitspoke.GodotEngine.Common.Noise;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;
using Bitspoke.Ludus.Shared.Environment.World.TypeData;
using Godot;

namespace Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Layers;

public class MapGenStepElevationLayer : MapGenStepLayer
{
    #region Properties

    private GodotNoise? Noise { get; set; }
    private NoiseTexture2D NoiseTexture2D { get; set; }
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
            
        var seedElevation = Rand.NextInt(int.MaxValue);
        Noise = new GodotNoise(seedElevation, MapGenStepDef.NoiseDef);

        NoiseTexture2D = Noise.GenerateNoiseTexture2D(Map.Width, Map.Height);

        if (CoreGlobal.DEBUG_ENABLED)
            NoiseTexture2D.Changed += () =>
            {
                Profiler.Start(additionalKey: "elevation_noise");
                var img = NoiseTexture2D.GetImage();
                img.ClearMipmaps();
                var fileName = $"{GodotGlobal.SAVE_ROOT_PATH}/elevation.png";
                var x = img.SavePng(fileName);
                Profiler.End(message:"+++ elevation_noise", additionalKey:"elevation_noise");
            };
            
            
        ElevationTypeDataKey = Map.MapInitConfig.ElevationTypeDataKey;
        ElevationModifier = Find.DB.TypeData.ElevationTypeData[ElevationTypeDataKey].GetValue<float>();

        // TODO: REFACTOR - remove Map.Cells as it is replaced with Map.Data.CellsContainer 
        ProcessCellsOld();
        ProcessCells();
            
        Profiler.End(message:"+++ Process Cells");
    }

    private void ProcessCells()
    {
        Profiler.Start();
            
        var bucketCells = Map.Data.CellsContainer.GetCellBucket(GodotGlobal.CPU_CORES);
        var tasks = new List<Task>();
            
        foreach (var bucketCellBitspokeArray in bucketCells.Values)
        {
            tasks.Add(Task.Run(() =>
            {
                foreach (var mapCell in bucketCellBitspokeArray)
                    mapCell.Elevation = Noise.GetValue(mapCell.Location, GetElevationValueFunc);
            }));
        }
            
        Task.WaitAll(tasks.ToArray());
            
        Profiler.End(message:"+++ (benchmark 80-90 ms)");
    }
        
    private void ProcessCellsOld()
    {
        Profiler.Start();
            
        var tasks = new List<Task>();
        foreach (var bucket in Map.Cells.Buckets)
        {
            tasks.Add(Task.Run(() =>
            {
                foreach (var mapCell in bucket.Values)
                {
                    mapCell.Elevation = Noise.GetValue(mapCell.Location, GetElevationValueFunc);
                    //mapCell.Elevation = NoiseTexture2D.Noise.GetNoise2D(mapCell.Location.x, mapCell.Location.y);
                    //mapCell.Elevation = GetElevationValueFunc(0,0, mapCell.Elevation);
                }
            }));
        }
        Task.WaitAll(tasks.ToArray());
        Profiler.End(message:"OLD +++");
    }
        
    private float GetElevationValueFunc(int x, int y, float value)
    {
        //Log.Debug($"Raw Noise Value: {value}");
            
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
            
        //Log.Debug($"Calculated Noise Value: {value}");
            
        return value;
    }

        
    #endregion


        

        
}