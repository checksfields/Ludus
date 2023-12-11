using Bitspoke.Core.Definitions.Parts.Graphics;
using Bitspoke.Core.Random;
using Godot;

namespace Bitspoke.Ludus.Client.Components.Nodes;

public static class Transform2DExtension
{

    #region Methods

    public static Transform2D GenerateTransform2D(this GraphicDef graphicsDef, Vector2 location)
    {
        var transform2D = new Transform2D(Mathf.Pi, location);
            
        if (graphicsDef.Scale != null)
            transform2D = transform2D.Scaled(Vector2.One * graphicsDef.Scale.RandRange());

        transform2D.Origin = location;
        
        return transform2D;
    }

    #endregion


}