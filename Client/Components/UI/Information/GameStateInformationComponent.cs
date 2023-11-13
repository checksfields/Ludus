using Bitspoke.Core.Common.States.Games;
using Bitspoke.Core.Signal;
using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Components.Nodes.CanvasItems;
using Bitspoke.GodotEngine.Components.Nodes.CanvasItems.Controls;
using Bitspoke.GodotEngine.Components.Nodes.CanvasItems.Controls.Containers;
using Bitspoke.GodotEngine.Components.Nodes.CanvasItems.Controls.Labels;
using Bitspoke.GodotEngine.Components.Nodes.CanvasLayers;
using Godot;

namespace Bitspoke.Ludus.Client.Components.UI.Information;

public partial class GameStateInformationComponent : GodotCanvasLayer
{
    #region Properties

    public override string Name => GetType().Name;

    private GodotPanelContainer MainContainer { get; set; }
    private GodotMarginContainer MarginContainer { get; set; }
    private GodotVBoxContainer VBoxContainer { get; set; }
    private GodotGridContainer GridContainer { get; set; }
    
    private HeaderLabel TitleLabel { get; set; }
    
    private DefaultLabel PreviousStateLabel { get; set; }
    private DefaultLabel PreviousStateValueLabel { get; set; }
    private DefaultLabel CurrentStateLabel { get; set; }
    private DefaultLabel CurrentStateValueLabel { get; set; }
    
    #endregion

    #region Constructors and Initialisation
    // none
    #endregion

    #region Overrides
    
    public override void Init()
    {
        Offset = GodotGlobal.UI.Layout.OFFSET_SCREEN_TOP_RIGHT;
        //Scale = new Vector2(0.5f, 0.5f);
    }

    public override void AddComponents()
    {
        MainContainer = this.AddContainer<GodotPanelContainer>(container =>
        {
            container.SetLayoutTopRight();
            container.SetOpacity(0f);
        });
        
        MarginContainer = MainContainer.AddContainer<GodotMarginContainer>(container =>
        {
            container.SetOpacity(0.5f, true);
        });
        
        VBoxContainer = MarginContainer.AddContainer<GodotVBoxContainer>(container =>
        {
            container.AddThemeConstantOverride("separation", -5);
        });
        
        GridContainer = VBoxContainer.AddContainer<GodotGridContainer>(container =>
        {
            container.Columns = 2;
            container.AddThemeConstantOverride("v_separation", -5);
        });
        
        GridContainer.AddComponent(CurrentStateLabel = new DefaultLabel { Text = "Current Game State:"});
        GridContainer.AddComponent(CurrentStateValueLabel = new DefaultLabel { Text = "Not Set"});
        
        GridContainer.AddComponent(PreviousStateLabel = new DefaultLabel { Text = "Previous Game State:"});
        GridContainer.AddComponent(PreviousStateValueLabel = new DefaultLabel { Text = "Not Set"});
    }

    public override void ConnectSignals()
    {
        this.ConnectSignal(typeof(GameStateManager), nameof(GameStateManager.GameStateChange), nameof(OnGameStateChanged));
    }

    private void OnGameStateChanged()
    {
        CurrentStateValueLabel.Text = CoreFind.Managers.GameStateManager?.CurrentState?.Key ?? "ERROR: -9999999";
        PreviousStateValueLabel.Text = CoreFind.Managers.GameStateManager?.PreviousState?.Key ?? "Not Set";
    }
    
    #endregion
    
    #region Methods

    private GodotPanelContainer AddPanelContainer(Node parent)
    {
        var container = new GodotPanelContainer();

        container.SetLayoutTopRight();
        container.SetOpacity(0f);
        
        parent.AddComponent(container);
        return container;
    }
    
    #endregion


    
    
}