using Bitspoke.Core.Random;
using Bitspoke.GodotEngine.Common.Noise;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;

namespace Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Layers;

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
        Noise = new GodotNoise(seed, MapGenStepDef.NoiseDef);
            
        // if (CoreGlobal.DEBUG_ENABLED)
        //     Noise.GenerateImageTexture(325, 325, $"{GodotGlobal.SAVE_ROOT_PATH}/fertility.png");
            
        ProcessCells();
        ProcessCellsOld();
            
        Profiler.End();
    }
        
    private void ProcessCells()
    {
        Profiler.Start();
        var tasks = new List<Task>();
            
        foreach (var cells in Map.Data.CellsContainer.CellsByRegion.Array)
        {
            tasks.Add(Task.Run(() =>
            {
                foreach (var mapCell in cells)
                {
                    mapCell.Fertility = Noise.GetValue(mapCell.Location, GetFertilityValueFunc);
                }
                        
            }));
        }
             
        Task.WaitAll(tasks.ToArray());
            
        Profiler.End(message:"NEW +++");
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
                    mapCell.Fertility = Noise.GetValue(mapCell.Location, GetFertilityValueFunc);
                }
                    
            }));
        }
        Task.WaitAll(tasks.ToArray());
        Profiler.End(message:"OLD +++");
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