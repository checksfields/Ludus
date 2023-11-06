using System;
using System.Collections.Generic;
using Bitspoke.Core.Common.Collections.Arrays;
using Bitspoke.Core.Common.Vector;
using Bitspoke.Core.Components.Identity;
using Bitspoke.Core.Utils.Primatives.Float;
using Bitspoke.GodotEngine.Common.Vector;
using Godot;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Regions.Components;

public class RegionsContainer
{
    #region Properties

    [JsonIgnore] private Map Map { get; }
    
    public Vector2I RegionSize { get; set; } = Global.DEFAULT_MAP_REGION_DIMENSIONS;
    public int Width => ((float) Map.Width / RegionSize.X).Ceiling();
    public int Height => ((float) Map.Height / RegionSize.Y).Ceiling();
    [JsonIgnore] public int Area => Width * Height;
    
    public BitspokeArray<Region?> Regions { get; set; }
    public Region? this[int regionIndex] => Regions?[regionIndex] ?? null;
    
    #endregion

    #region Constructors and Initialisation

    public RegionsContainer(int mapID, Vector2I? regionSize = null) : this(Find.Map(mapID), regionSize)
    {
    }
    
    public RegionsContainer(Map map, Vector2I? regionSize = null)
    {
        Map = map;
        if (regionSize != null)
            RegionSize = regionSize.Value;
        
        InitialiseRegions();
    }
    
    public void InitialiseRegions()
    {
        // validate
        var moduloX = Map.Width % RegionSize.X;
        var moduloY = Map.Height % RegionSize.Y;
        
        if (moduloX != 0 || moduloY != 0)
            Log.Exception($"Region Size is inconsistent with Map Size", -9999999);

        var numberOfRegions = (Map.Width / RegionSize.X) * (Map.Height / RegionSize.Y);
        Regions = new BitspokeArray<Region?>(numberOfRegions);
        
        for (int y = 0; y < Map.Height / RegionSize.Y; y++)
        {
            for (int x = 0; x < Map.Width / RegionSize.X; x++)
            {
                var xStart = x * RegionSize.X;
                var yStart = y * RegionSize.Y;
                
                var rect = new Rect2(xStart, yStart,RegionSize.X, RegionSize.Y);
                var index = x + y * Width;

                var region = new Region(index, Map, rect);
                Regions[index] = region;
            }
        }
    } 
    #endregion

    #region Methods
    
    public Region? GetRegionByCellLocation(Vector2I cellLocation)
    {
        var regionX = cellLocation.X / RegionSize.X;
        var regionY = cellLocation.Y / RegionSize.Y;
        var index = new Vector2I(regionX, regionY).ToIndex(Width);
        
        return Regions[index];
    }

    #endregion

    
}