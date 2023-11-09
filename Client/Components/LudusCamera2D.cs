using Bitspoke.Core.Common.Logging;
using Bitspoke.Core.Components;
using Bitspoke.GodotEngine.Components.Camera._2D;

namespace Client.Components;

public partial class LudusCamera2D : BitspokeCamera2D
{
    public LudusCamera2D(ComponentCollection components) : base(components)
    {
        Log.Info();
        Name = nameof(LudusCamera2D);
    }
}