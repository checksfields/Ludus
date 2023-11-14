using Bitspoke.Core.Common.Vector;
using Bitspoke.Core.Components.Identity;
using Bitspoke.Core.Utils.Primatives.Float;
using Bitspoke.GodotEngine.Common.Vector;
using Bitspoke.Ludus.Shared.Common.Components;
using Godot;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Regions.Components;

public class MapRegionsComponent : LudusComponent
{
    #region Properties

    public override string ComponentName => nameof(MapRegionsComponent);
    
    [JsonIgnore] private Map Map { get; }
    public Vec3Int? RegionSize { get; set; } = Global.DEFAULT_MAP_REGION_DIMENSIONS.ToVec2Int().ToVec3Int();
    public int Width => ((float)Map.Width / RegionSize.x).Ceiling();
    public int Height => ((float)Map.Height / RegionSize.y).Ceiling();
    public int Area => Width * Height;
    public Dictionary<int, Region> MapRegions { get; set; }
    
    #endregion

    #region Constructors and Initialisation

    public MapRegionsComponent(IDComponent mapID, Vec3Int? regionSize = null)
    {
        Map = Find.Map(mapID.ID);
        if (regionSize != null)
            RegionSize = regionSize;
        
        CreateRegions();
    }
    
    #endregion

    #region Methods

    public void CreateRegions()
    {
        MapRegions = new Dictionary<int, Region>();
        
        // validate
        var moduloX = Map.Width % RegionSize.x;
        var moduloY = Map.Height % RegionSize.y;
        
        if (moduloX != 0 || moduloY != 0)
            Log.Exception($"Region Size is inconsistent with Map Size", -9999999);

        for (int y = 0; y < Map.Height / RegionSize.y; y++)
        {
            for (int x = 0; x < Map.Width / RegionSize.x; x++)
            {
                var xStart = x * RegionSize.x;
                var yStart = y * RegionSize.y;
                
                var rect = new Rect2I(xStart, yStart,RegionSize.x, RegionSize.x);
                var index = x + y * Width;

                var region = new Region(index, Map, rect);
                MapRegions.Add(index, region);
            }
        }
    }

    public Region GetRegion(Vec3Int location)
    {
        var regionX = location.x / RegionSize.x;
        var regionY = location.y / RegionSize.y;
        var index = new Vec3Int(regionX, regionY).ToIndex(Width);
        
        return MapRegions[index];
    }
    
    
    
    #endregion


    
}