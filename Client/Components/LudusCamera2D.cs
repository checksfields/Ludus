using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Components.Camera._2D;

namespace Bitspoke.Ludus.Client.Components;

public partial class LudusCamera2D : BitspokeCamera2D
{
    public LudusCamera2D(bool isCurrent) : base(isCurrent) { }
    public LudusCamera2D(GodotNodeCollection components) : base(components) { }
}