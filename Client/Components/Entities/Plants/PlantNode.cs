using Bitspoke.GodotEngine.Components.Nodes._2D;
using Bitspoke.Ludus.Client.Components.Nodes.Sprites.Plants;

namespace Bitspoke.Ludus.Client.Components.Entities.Plants;

public abstract partial class PlantNode : GodotNode2D
{
    #region Properties
    // none
    public bool MouseOver { get; set; }
    
    #endregion

    #region Constructors and Initialisation
    // none
    #endregion

    #region Overrides

    public override void Init()
    {
        base.Init();
        
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        var mousePos = GetViewport().GetMousePosition();
        var spriteRect = GetSprite2D().GetRect();
        spriteRect.Position += GlobalPosition;
        spriteRect.HasPoint(mousePos);
        
        
    }

    #endregion
    
    #region Methods
    // none
    public abstract PlantSprite2D GetSprite2D();

    #endregion
}