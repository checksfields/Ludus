using System.ComponentModel;
using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Components.Nodes._2D.Mouse;
using Bitspoke.GodotEngine.Components.Nodes.Sprites;
using Bitspoke.GodotEngine.Utils.Rect;
using Bitspoke.Ludus.Shared.Common.Entities;
using Godot;
using Bitspoke.Ludus.Client.Components.Entities;
using Bitspoke.Ludus.Client.Components.Entities.Plants.Natural;

namespace Bitspoke.Ludus.Client.Components.Nodes.Sprites;

public abstract partial  class EntitySprite2D : GodotSprite2D
{
    #region Properties

    [ImmutableObject(true)] public LudusEntity LudusEntity { get; set; }
    
    #endregion

    #region Constructors and Initialisation

    protected EntitySprite2D(LudusEntity entity) : base()
    {
        LudusEntity = entity;
    }
    public EntitySprite2D() : base() { }
    
    #endregion

    #region Overrides

    public abstract void BuildSprite(Vector2 parentGlobalPosition, Texture2D texture2D = null);
    
    public override void AddComponents()
    {
        base.AddComponents();
        
        this.AddGodotNode(new MouseOverNode2D(GetRect().Size, OnMouseEntered, OnMouseExited));
    }
    
    #endregion

    #region Methods

    public abstract EntityDetailsDisplayNode GetDetailsNode();
    
    protected virtual void OnMouseEntered()
    {
        var detailsNode = GetDetailsNode();
        detailsNode.GlobalPosition = (GlobalPosition - GetViewport().GetCamera2D().Position) * GetViewport().GetCamera2D().Zoom;
        GodotGlobal.Actions.UIMouseOverEnter.Invoke(detailsNode);
    }

    protected virtual void OnMouseExited()
    {
        GodotGlobal.Actions.UIMouseOverExit.Invoke();
    }

    #endregion


}