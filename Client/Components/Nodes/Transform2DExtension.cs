using Bitspoke.Core.Definitions.Parts.Graphics;
using Bitspoke.Core.Random;
using Godot;

namespace Bitspoke.Ludus.Client.Components.Nodes;

public static class Transform2DExtension
{

    #region Methods

    public static Transform2D GenerateTransform2D(this GraphicDef graphicsDef, Vector2 location)
    {
        var transform = new Transform2D(3.14159f, location);
            
        if (graphicsDef.ScaleRange != null)
            transform = transform.Scaled(Vector2.One * Rand.NextFloat(graphicsDef.ScaleRange.Min, graphicsDef.ScaleRange.Max));

        return transform;
    }

    #endregion


}