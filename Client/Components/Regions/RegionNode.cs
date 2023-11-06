using System.Collections.Generic;
using System.Linq;
using Bitspoke.Core.Common.Logging;
using Bitspoke.Core.Components.Location;
using Bitspoke.GodotEngine.Common.Vector;
using Bitspoke.GodotEngine.Components.Nodes;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Environment.Map;
using Bitspoke.Ludus.Shared.Environment.Map.Regions;
using Godot;

namespace Client.Components.Regions
{

    public abstract partial class RegionNode : GodotNode2D
    {
        #region Properties

        public Map Map { get; set; }
        
        public int RegionID => Region?.Index ?? -1;
        public Region Region { get; set; }
        public Rect2 Dimension { get; set; }

        public VisibleOnScreenNotifier2D VisibleOnScreenNotifier3D { get; set; }
        public int TotalInstances { get; set; }

        public Dictionary<int, RegionLayer> RegionLayers { get; set; }

        public int SpritesCount { get; set; }

        public int MeshesCount => RegionLayers.Values
            .Where(w => w.GetType() == typeof(MultiMeshRegionLayer))
            .Sum(c => ((MultiMeshRegionLayer)c).MultiMeshInstance2D.Multimesh.InstanceCount);

        public int ItemCount { get; set; } = 0;

        #endregion

        #region Constructors and Initialisation

        public RegionNode() : base()
        {

        }

        // public override void _Draw()
        // {
        //     base._Draw();
        //     if (CoreGlobal.DEBUG_ENABLED)
        //     {
        //         
        //         var size = Region.Dimension.Size * CoreGlobal.STANDARD_CELL_SIZE;
        //         var rect2 = new Rect2(Vector2.Zero, size);
        //         DrawRect(rect2, Colors.Red, false, width:1f);
        //     }
        // }

        protected RegionNode(Region region) : this()
        {
            if (region == null)
                Log.Exception($"Region cannot be null", -9999999);
            
            Region = region;
            Map = Region.Map;
            Dimension = Region.Dimension;
            
            AddComponents();
            ConnectSignals();
        }

        protected override void AddComponents()
        {
            VisibleOnScreenNotifier3D = new VisibleOnScreenNotifier2D();
            AddChild(VisibleOnScreenNotifier3D);

            var size = Dimension.Size * 64 * 1.2f;
            var position = new Vector2(size.X/2 * -1, size.Y/2 * -1);
            
            VisibleOnScreenNotifier3D.Rect = new Rect2(position, size);
        }

        protected override void ConnectSignals()
        {
            //VisibleOnScreenNotifier3D.Connect("screen_entered",new Callable(this,"show"));
            VisibleOnScreenNotifier3D.Connect("screen_entered",new Callable(this,nameof(OnShowBase)));
            //VisibleOnScreenNotifier3D.Connect("screen_exited",new Callable(this,"hide"));
            VisibleOnScreenNotifier3D.Connect("screen_exited",new Callable(this,nameof(OnHideBase)));
        }

        private void OnShowBase() => OnShow();
        protected abstract void OnShow();
        
        private void OnHideBase() => OnHide();
        protected abstract void OnHide();

        #endregion

        #region Methods

        protected void AddSprites(Texture2D layerTexture, Region region, List<LudusEntity> regionEntities)
        {
            Node2D? spriteLayer = null;
            foreach (Node2D child in GetChildren())
            {
                if (child.Name == "sprite_layer")
                    spriteLayer = (Node2D)child;
            }

            if (spriteLayer == null)
                AddChild(spriteLayer = new Node2D() { Name = "sprite_layer" });

            foreach (var ludusEntity in regionEntities)
            {
                var locComp = ludusEntity.GetComponent<LocationComponent>();
                var location = (locComp.Location.ToVector2() * 64) + new Vector2(32, 0);
                var localLocation = location - spriteLayer.GlobalPosition;

                var sprite = new Sprite2D();
                sprite.Texture = layerTexture;
                sprite.Position = localLocation;
                sprite.ZIndex = (int)sprite.Position.Y;
                //sprite.ZAsRelative = true;

                spriteLayer.AddChild(sprite);
            }
        }

        // protected void AddLayer(Texture2D layerTexture, Region region, List<LudusEntity> regionEntities) //, Region region)
        // {
        //     MultiMeshRegionLayer layer = new MultiMeshRegionLayer(0, layerTexture, regionEntities);
        //     AddChild(layer);
        // }

        #endregion



    }
}