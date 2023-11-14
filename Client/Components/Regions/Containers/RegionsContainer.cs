using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Components.Nodes;
using Bitspoke.Ludus.Shared.Environment.Map;
using Godot.Collections;

namespace Bitspoke.Ludus.Client.Components.Regions.Containers;

public partial class RegionsContainer : GodotNode2D
{
    #region Properties
    
    public override string Name => $"{this.GetType().Name}_{GetInstanceId()}";

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
                this.AddComponent(new RegionContainer(Map.Data.RegionsContainer.Regions[regionIndex]));
        });
    }
    
    #endregion
    
}