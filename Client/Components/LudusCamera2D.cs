using Bitspoke.Core.Components;
using Bitspoke.Core.Components.Collections;
using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Components.Camera._2D;

namespace Bitspoke.Ludus.Client.Components;

public partial class LudusCamera2D : BitspokeCamera2D
{
    public LudusCamera2D(GodotComponentCollection components) : base(components)
    {
        Log.Info();
        Name = nameof(LudusCamera2D);
    }
}