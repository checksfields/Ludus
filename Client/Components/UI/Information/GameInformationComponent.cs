using Bitspoke.Core.Common.States.Games;
using Bitspoke.Core.Signal;
using Bitspoke.Core.Systems.Time;
using Bitspoke.Core.Types.Game.States;
using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Components.Nodes;
using Bitspoke.GodotEngine.Components.Nodes.CanvasItems;
using Bitspoke.GodotEngine.Components.Nodes.CanvasItems.Controls;
using Bitspoke.GodotEngine.Components.Nodes.CanvasItems.Controls.Containers;
using Bitspoke.GodotEngine.Components.Nodes.CanvasItems.Controls.Labels;
using Bitspoke.GodotEngine.Components.Nodes.CanvasLayers;
using Bitspoke.Ludus.Shared.Environment.Map;
using Godot;

namespace Bitspoke.Ludus.Client.Components.UI.Information;

public partial class GameInformationComponent : GodotCanvasLayer
{
    #region Properties

    public override string NodeName => GetType().Name;
    public override Node Node => this;

    private GodotPanelContainer MainContainer { get; set; }
    private GodotMarginContainer MarginContainer { get; set; }
    private GodotVBoxContainer VBoxContainer { get; set; }
    private GodotGridContainer GridContainer { get; set; }
    
    private HeaderLabel TitleLabel { get; set; }
    
    private DefaultLabel CurrentStateLabel { get; set; }
    private DefaultLabel CurrentStateValueLabel { get; set; }
    
    private DefaultLabel PlantCountLabel { get; set; }
    private DefaultLabel PlantCountValueLabel { get; set; }
    
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
        
        GridContainer.AddGodotNode(CurrentStateLabel = new DefaultLabel { Text = "Current Game State:"});
        GridContainer.AddGodotNode(CurrentStateValueLabel = new DefaultLabel { Text = "Not Set"});
        
        GridContainer.AddGodotNode(PlantCountLabel = new DefaultLabel { Text = "Plant Count:"});
        GridContainer.AddGodotNode(PlantCountValueLabel = new DefaultLabel { Text = "Not Set"});
        
        // var plantCountInfo = VBoxContainer.AddContainer<NameValueGridContainer>(plantCountInfo =>
        // {
        //     plantCountInfo.AddThemeConstantOverride("separation", -5);
        //     plantCountInfo.UseTimerSystem = false;
        //     plantCountInfo.AddHeader("Plant Count:", false);
        //     plantCountInfo.AddNameValuePair("Plants:", () => Find.CurrentMap?.Data.EntitiesContainer.EntitiesList.Count.ToString() ?? "Not Set");
        // });
        
    }

    public override void ConnectSignals()
    {
        GameStateManager.GameStateChanged += OnGameStateChanged;
    }

    private void OnGameStateChanged(GameStateTypeData gameState)
    {
        CurrentStateValueLabel.Text = gameState.Key ?? "ERROR: -9999999";
    }

    public double UpdateTicks { get; set; } = 600;
    public double ElapsedTicks { get; set; } = 0;
    
    public override void _PhysicsProcess(double delta)
    {
        ElapsedTicks++;
        if (ElapsedTicks >= UpdateTicks)
        {
            ElapsedTicks = 0;
            PlantCountValueLabel.Text = Find.CurrentMap?.Data.EntitiesContainer.EntitiesList.Count.ToString() ?? "Not Set";
        }
    }

    #endregion
    
    #region Methods
    // none
    #endregion


    
    
}