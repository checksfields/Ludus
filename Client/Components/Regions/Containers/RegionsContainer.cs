using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Components.Nodes._2D;
using Bitspoke.Ludus.Shared.Environment.Map;
using Godot;
using Godot.Collections;

namespace Bitspoke.Ludus.Client.Components.Regions.Containers;

public partial class RegionsContainer : GodotNode2D
{
    #region Properties
    
    public override string NodeName => GetType().Name;
    public override Node Node => this;

    public Map Map { get; set; }
    public Dictionary<int, RegionContainer> RegionNodes { get; private set; } = new();

    #endregion

    #region Constructors and Initialisation

    public RegionsContainer(Map map)
    {
        Map = map;
    }
    
    #endregion

    #region Overrides

    public override void _Ready()
    {
        base._Ready();
        
        AddRegionContainers();
    }

    public override void Init() { }
    public override void AddComponents() { }
    public override void ConnectSignals() { }

    #endregion
    
    #region Methods

    private void AddRegionContainers()
    {
        Profile(() => { 
            for (var regionIndex = 0; regionIndex < Map.Data.RegionsContainer.Regions.Length; regionIndex++)
                this.AddGodotNode(new RegionContainer(Map.Data.RegionsContainer.Regions[regionIndex]));
        });
    }
    
    #endregion
    
}