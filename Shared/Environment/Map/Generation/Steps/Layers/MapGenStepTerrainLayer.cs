﻿using Bitspoke.Core.Profiling;
using Bitspoke.Core.Utils.Primatives.Float;
using Bitspoke.Ludus.Shared.Entities.Definitions.Structures.Natural;
using Bitspoke.Ludus.Shared.Entities.Definitions.Structures.Natural.Rocks.Definitions;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Terrain;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells;
using TerrainDef = Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Terrain.TerrainDef;

namespace Bitspoke.Ludus.Shared.Environment.Map.Generation.Steps.Layers;

public class MapGenStepTerrainLayer : MapGenStepLayer
{

    #region Properties

    public override string StepName => GetType().Name;

    #endregion

    #region Constructors and Initialisation
        
    public MapGenStepTerrainLayer(Map map, MapGenStepDef mapGenStepDef) : base(map, mapGenStepDef)
    {
    }
        
    #endregion

    #region Methods

        
        
    protected override void StepGenerate()
    {
        Profiler.Start();
        ProcessCells();
        Profiler.End();
    }

    private void ProcessCells()
    {
        Profiler.Start();
        foreach (var mapCell in Map.Data.CellsContainer.Cells.Array)
        {
            mapCell.TerrainDef = GetTerrainLayerDefFor(mapCell)?.Clone() ?? null;
            if (mapCell.TerrainDef == null)
            {
                Log.Warning($"Could not generate a terrain for cell: {mapCell}", -9999999);
                mapCell.TerrainDef = TerrainDefsCollection.DEFAULT_DEF;
            }
        }
        Profiler.End(message:"NEW +++");
    }
        
    private void ProcessCellsOld()
    {
        Profiler.Start();
        foreach (var mapCell in Map.Data.CellsContainer.Cells)
        {
            mapCell.TerrainDef = GetTerrainLayerDefFor(mapCell)?.Clone() ?? null;
            if (mapCell.TerrainDef == null)
            {
                Log.Warning($"Could not generate a terrain for cell: {mapCell}", -9999999);
                mapCell.TerrainDef = TerrainDefsCollection.DEFAULT_DEF;
            }
                
            mapCell.TerrainDef.Index = mapCell.Index;
        }
        Profiler.End(message:"OLD +++");
    }
        
    private TerrainDef GetTerrainLayerDefFor(MapCell mapCell)
    {
        TerrainDef terrainDef = null;
            
        // have we already assigned a natural structure to this cell, if so get its 
        // associated terrain def
        terrainDef ??= GetTerrainDefUsingStructuresFor(mapCell);

        // handle elevation 
        terrainDef ??= GetTerrainDefUsingElevationFor(mapCell);
            
        // so by now we have the natural structures in place and set the underlying terrain to the 
        // matching type.  we have also handled the terrains based on the elevation data layer.  what is left
        // is everything with an elevation below the lowest value in the biome def's elevation terrain layers
            
        // handle fertility 
        terrainDef ??= GetTerrainDefUsingFertilityFor(mapCell);
            
        return terrainDef;
    }

    private TerrainDef? GetTerrainDefUsingStructuresFor(MapCell mapCell)
    {
        TerrainDef terrainDef = null;
         
        // have we already assigned a natural structure to this cell, if so get its 
        // associated terrain def
        if (mapCell.HasNaturalStructure)
        {
            var associatedTerrainKey = ((NaturalStructureDef)mapCell.StructureDef).AssociatedTerrainDefKey;
            if (!string.IsNullOrEmpty(associatedTerrainKey))
                terrainDef = Find.DB.TerrainDefs[associatedTerrainKey];
        }

        return terrainDef;
    }
        
    private TerrainDef? GetTerrainDefUsingElevationFor(MapCell mapCell)
    {
        TerrainDef? terrainDef = null;
         
        var maxElevation = float.MinValue;
        foreach (var biomeDefElevationTerrainDef in Map.BiomeDef.ElevationTerrainDefs)
        {
            if (mapCell.Elevation.IsBetween(biomeDefElevationTerrainDef.Value))
            {
                terrainDef = Find.DB.TerrainDefs[biomeDefElevationTerrainDef.Key];
                break;
            }

            if (biomeDefElevationTerrainDef.Value.Max > maxElevation)
                maxElevation = biomeDefElevationTerrainDef.Value.Max;
        }

        if (terrainDef == null && mapCell.Elevation >= maxElevation)
        {
            Find.DB.RockDefs.TryGetValue(mapCell.Stratum, out var rockDef);
            Find.DB.TerrainDefs.TryGetValue(rockDef?.AssociatedTerrainDefKey ?? string.Empty, out terrainDef);
        }
            
        return terrainDef;
    }

    private TerrainDef? GetTerrainDefUsingFertilityFor(MapCell mapCell)
    {
        TerrainDef terrainDef = null;
         
        foreach (var biomeDefFertilityTerrainDef in Map.BiomeDef.FertilityTerrainDefs)
        {
            if (mapCell.Fertility.IsBetween(biomeDefFertilityTerrainDef.Value))
            {
                terrainDef = Find.DB.TerrainDefs[biomeDefFertilityTerrainDef.Key];
                break;
            }
        }

        return terrainDef;
    }
        
    #endregion
        
}