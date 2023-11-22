using System.Threading.Tasks;
using Bitspoke.Core.Systems.Age;
using Bitspoke.Core.Utils.Primatives.Float;
using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Components.Nodes._2D;
using Bitspoke.GodotEngine.Components.Nodes._2D.Notifiers;
using Bitspoke.Ludus.Client.Components.Regions.Plants;
using Bitspoke.Ludus.Shared.Environment.Map.Regions;
using Bitspoke.Ludus.Shared.Systems.Growth;
using Godot;

namespace Bitspoke.Ludus.Client.Components.Regions.Containers;

public partial class RegionContainer : GodotNode2D
{
    

    #region Properties
    
    public override string NodeName => GetType().Name;
    public override Node Node => this;

    public GodotVisibleOnScreenNotifierComponent VisibleOnScreenNotifierComponent { get; set; }
    public Region Region { get; set; }
    
    public GodotNode2D Regions { get; set; }
    
    public PlantRegionNode? PlantRegionNode { get; set; } = null;
    
    #endregion

    #region Constructors and Initialisation
    
    public RegionContainer(Region region)
    {
        Region = region;
    }
    
    #endregion

    #region Overrides

    public override void _Ready()
    {
        base._Ready();
        
        Position = Region.Dimension.Position * CoreGlobal.STANDARD_CELL_SIZE;
        
        var rect = Region.Dimension;
        var origSize = rect.Size * CoreGlobal.STANDARD_CELL_SIZE; 
        var size = new Vector2I((origSize.X * 1.2f).Ceiling(), (origSize.Y * 1.2f).Ceiling());
        var position = new Vector2I((origSize.X * -0.4f).Floor(), (origSize.Y * -0.5f).Floor());
        
        VisibleOnScreenNotifierComponent.Rect = new Rect2(position, size);
    }

    public bool PlantRegionsReady { get; set; } = false;
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (PlantRegionsReady)
        {
            Regions.AddChild(PlantRegionNode);
            PlantRegionsReady = false;
        }
    }

    // public override void _Draw()
    // {
    //     base._Draw();
    //     DrawRect(VisibleOnScreenNotifierComponent.Rect, Colors.Magenta, false, 4f);
    // }

    public override void Init()
    {
        AddChild(Regions = new DefaultGodotNode2D());
    }
    
    public override void AddComponents()
    {
        this.AddGodotNode(VisibleOnScreenNotifierComponent = new());
        
    }

    public override void ConnectSignals()
    {
        VisibleOnScreenNotifierComponent.ScreenEntered += OnScreenEntered;
        VisibleOnScreenNotifierComponent.ScreenExited += OnScreenExited;
    }

    private void OnGrowthSystemTickComplete()
    {
        PlantRegionNode?.GrowthRefresh();
    }
    
    private void OnAgeSystemTickComplete()
    {
        PlantRegionNode?.AgeRefresh();
    }

    #endregion
    
    #region Methods
    
    private void AddPlantsRegion()
    {
        PlantRegionNode = new(Region);
        PlantRegionsReady = true;
        //Regions.AddChild(PlantRegionNode = new(Region));
        //Regions.AddComponent(PlantRegionNode = new(Region));
    }
    
    private void OnScreenEntered()
    {
        GrowthSystem.Instance.TickComplete += OnGrowthSystemTickComplete;
        AgeSystem.Instance.TickComplete += OnAgeSystemTickComplete;

        Task.Run(AddPlantsRegion);
        //AddPlantsRegion();
    }
    
    private void OnScreenExited()
    {
        GrowthSystem.Instance.TickComplete -= OnGrowthSystemTickComplete;
        AgeSystem.Instance.TickComplete -= OnAgeSystemTickComplete;
        //Regions.QueueFree();
        PlantRegionNode?.QueueFree();
    }
    
    #endregion


    
    
}