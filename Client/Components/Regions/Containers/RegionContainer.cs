using Bitspoke.Core.Utils.Primatives.Float;
using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Components.Nodes._2D;
using Bitspoke.GodotEngine.Components.Nodes._2D.Notifiers;
using Bitspoke.Ludus.Client.Components.Regions.Plants;
using Bitspoke.Ludus.Shared.Environment.Map.Regions;
using Godot;

namespace Bitspoke.Ludus.Client.Components.Regions.Containers;

public partial class RegionContainer : GodotNode2D
{
    

    #region Properties
    
    public override string NodeName => GetType().Name;
    public override Node Node => this;

    public GodotVisibleOnScreenNotifierComponent VisibleOnScreenNotifierComponent { get; set; }
    public Region Region { get; set; }
    
    public DefaultGodotNode2D Regions { get; set; }
    
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
        var size = new Vector2I((origSize.X * 1.5f).Ceiling(), (origSize.Y * 1.5f).Ceiling());
        var position = new Vector2I((origSize.X * -0.5f).Floor(), (origSize.Y * -0.5f).Floor());
        
        VisibleOnScreenNotifierComponent.Rect = new Rect2(position, size);
    }
    
    public override void Init() { }
    public override void AddComponents()
    {
        this.AddGodotNode(VisibleOnScreenNotifierComponent = new());
        
    }

    public override void ConnectSignals()
    {
        VisibleOnScreenNotifierComponent.ScreenEntered += OnScreenEntered;
        VisibleOnScreenNotifierComponent.ScreenExited += OnScreenExited;
    }

    
    
    #endregion
    
    #region Methods
    
    private void AddRegion()
    {
        this.AddGodotNode(Regions =  new DefaultGodotNode2D());
        AddPlantsRegion();
        
    }

    private void AddPlantsRegion()
    {
        Regions.AddGodotNode(PlantRegionNode = new(Region), true);
        //Regions.AddComponent(PlantRegionNode = new(Region));
    }
    
    private void OnScreenEntered()
    {
        AddRegion();
    }
    
    private void OnScreenExited()
    {
        Regions.QueueFree();
    }
    
    #endregion


    
    
}