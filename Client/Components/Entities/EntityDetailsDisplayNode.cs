using Bitspoke.Core.Components.Identity;
using Bitspoke.Core.Components.Location;
using Bitspoke.GodotEngine.Components.Nodes._2D;
using Bitspoke.GodotEngine.Components.Nodes.CanvasItems;
using Bitspoke.GodotEngine.Components.Nodes.CanvasItems.Controls.Containers;
using Bitspoke.Ludus.Client.Components.Common.Display;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Entities.Natural.Plants;
using Godot;

namespace Bitspoke.Ludus.Client.Components.Entities;

public abstract partial class EntityDetailsDisplayNode : GodotNode2D
{
    #region Properties

    public LudusEntity LudusEntity { get; set; }
    
    protected GodotPanelContainer  MainContainer   { get; set; }
    protected GodotMarginContainer MarginContainer { get; set; }
    protected GodotVBoxContainer   VBoxContainer   { get; set; }
    
    protected NameValueGridContainer Details { get; set; }
    
    #endregion

    #region Constructors and Initialisation

    protected EntityDetailsDisplayNode(LudusEntity? entity) : base()
    {
        if (entity == null)
            Log.Exception($"LudusEntity is null", -9999999);
        
        LudusEntity = entity;
    }

    #endregion

    #region Methods

    public override void _EnterTree()
    {
        base._EnterTree();
        
        Scale = Vector2.One;
        
        MainContainer   = this.AddContainer<GodotPanelContainer>();
        MarginContainer = MainContainer.AddContainer<GodotMarginContainer>(container => { container.SetOpacity(0.5f, true); });
        VBoxContainer   = MarginContainer.AddContainer<GodotVBoxContainer>();
        Details = VBoxContainer.AddContainer<NameValueGridContainer>();
        //this.ZIndex = 1000;
        BuildNode();
    }

    public virtual void BuildNode()
    {
        Details.AddThemeConstantOverride("separation", -5);
        Details.AddHeader("Entity:", false);
        
        Details.AddLocationComponent(LudusEntity.GetComponent<LocationComponent>());
        Details.AddIDComponent(LudusEntity.GetComponent<IDComponent>());
    }

    #endregion


}