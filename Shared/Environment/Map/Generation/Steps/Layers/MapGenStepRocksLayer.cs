﻿using Bitspoke.Core.Definitions.Parts.Common.Noise;
using Bitspoke.Core.Profiling;
using Bitspoke.Core.Random;
using Bitspoke.GodotEngine.Common.Noise;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;

namespace Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Layers;

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
        Profile(() => { 

            // generate the noises we will need
            var strataNoises = new Dictionary<string, GodotNoise>();
            foreach (var availableRockDefKey in Map.MapInitConfig.AvailableRockDefKeys)
            {
                var rockDef = Find.DB.RockDefs[availableRockDefKey];
                Log.Debug($"Processing {rockDef.Key} RockDef");

                var seed = Rand.NextInt();
                var noise = new GodotNoise(seed, NoiseDef.DEFAULT);
                    
                // if (CoreGlobal.DEBUG_ENABLED)
                //     noise.GenerateImageTexture(325, 325, $"user://Saves/{rockDef.Key}_noise.png");
                    
                strataNoises.Add(rockDef.Key, noise);
            }
                
            ProcessCells(strataNoises);
            //ProcessCellsOld(strataNoises);

        });
    }
        
    private void ProcessCells(Dictionary<string, GodotNoise> strataNoises)
    {
        Profiler.Start();
            
        var tasks = new List<Task>();
        foreach (var cells in Map.Data.CellsContainer.CellsByRegion.Array)
        {
            tasks.Add(Task.Run(() =>
            {
                foreach (var mapCell in cells.Values)
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
                }
            }));
        }
        Task.WaitAll(tasks.ToArray());
            
        Profiler.End(message:"NEW +++");
    }
        
    // private void ProcessCellsOld(Dictionary<string, GodotNoise> strataNoises)
    // {
    //     Profiler.Start();
    //         
    //     var tasks = new List<Task>();
    //     foreach (var bucket in Map.Cells.Buckets)
    //     {
    //         tasks.Add(Task.Run(() =>
    //         {
    //             foreach (var mapCell in bucket.Values)
    //             {
    //                 string? stratumDefKey = null;
    //                 float stratumDefValue = float.MinValue;
    //                 foreach (var defNoise in strataNoises)
    //                 {
    //                     var noiseValue = defNoise.Value.GetValue(mapCell.Location);
    //                     if (noiseValue > stratumDefValue)
    //                     {
    //                         stratumDefKey = defNoise.Key;
    //                         stratumDefValue = noiseValue;
    //                     }
    //                 }
    //
    //                 mapCell.Stratum = stratumDefKey;
    //             }
    //         }));
    //     }
    //     Task.WaitAll(tasks.ToArray());
    //         
    //     Profiler.End(message:"OLD +++");
    // }

    #endregion


        

        
}