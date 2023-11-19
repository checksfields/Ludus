using System.Collections.Generic;
using System.Linq;
using Bitspoke.Core.Components.Life;
using Bitspoke.Core.Components.Location;
using Bitspoke.GodotEngine.Common.Vector;
using Bitspoke.GodotEngine.Components.Nodes._2D;
using Bitspoke.GodotEngine.Components.Nodes.Sprites;
using Bitspoke.GodotEngine.Utils.Vector;
using Bitspoke.Ludus.Client.Components.Nodes.Sprites.Plants.Natural;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Environment.Map;
using Bitspoke.Ludus.Shared.Environment.Map.Regions;
using Godot;

namespace Bitspoke.Ludus.Client.Components.Regions;

public abstract partial class RegionNode : GodotNode2D
{
    #region Properties

    public Map Map { get; set; }
        
    public int RegionID => Region?.Index ?? -1;
    public Region Region { get; set; }
    public Rect2 Dimension { get; set; }

    public int TotalInstances { get; set; }

    public Dictionary<int, RegionLayer> RegionLayers { get; set; }

    public int SpritesCount { get; set; }
    public GodotNode2D Sprites { get; set; }

    public int MeshesCount => RegionLayers.Values
        .Where(w => w.GetType() == typeof(MultiMeshRegionLayer))
        .Sum(c => ((MultiMeshRegionLayer)c).MultiMeshInstance2D.Multimesh.InstanceCount);

    public int ItemCount { get; set; } = 0;

    #endregion

    #region Constructors and Initialisation
    
    protected RegionNode(Region region)
    {
        if (region == null)
            Log.Exception($"Region cannot be null", -9999999);
            
        Region = region;
        Map = Region.Map;
        Dimension = Region.Dimension;
            
        AddComponents();
        ConnectSignals();
    }

    public override void AddComponents() { }
    public override void ConnectSignals() { }
    
    #endregion

    #region Methods

    protected void AddSprites(Texture2D layerTexture, List<LudusEntity> regionEntities)
    {
        // Sprites = null;
        // foreach (Node2D child in GetChildren())
        // {
        //     if (child.Name == "sprite_layer")
        //         Sprites = (Node2D)child;
        // }

        
        if (Sprites == null || !IsInstanceValid(Sprites) || Sprites.IsQueuedForDeletion())
            AddChild(Sprites = new());

        foreach (var ludusEntity in regionEntities)
        {
            var locComp = ludusEntity.GetComponent<LocationComponent>();
            var location = (locComp.Location.ToVector2() * 64) + new Vector2(32, 0);
            var localLocation = location - Sprites.GlobalPosition;

            var sprite = new Sprite2D();
            sprite.Texture = layerTexture;
            sprite.GlobalPosition = localLocation;
            sprite.ZIndex = (int)sprite.Position.Y;
            
            var growthComp = ludusEntity.GetComponent<GrowthComponent>();
            sprite.Scale = growthComp?.CurrentGrowthPercent.ToVector2() ?? Vector2.One;

            Sprites.AddChild(sprite);
        }
    }
    
    #endregion



}