using Bitspoke.GodotEngine.Components.Nodes._2D;

namespace Bitspoke.Ludus.Client.Components.Regions;

public abstract partial class RegionLayer : GodotNode2D
{
    public RegionNode Parent => base.GetParent<RegionNode>();
    public int LayerID { get; set; } = -1;

    public abstract string LayerName { get; protected set; }
    public abstract void Refresh();
}