using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Components.Nodes._2D;
using Bitspoke.Ludus.Client.Components.Regions.Containers;
using Bitspoke.Ludus.Shared.Environment.Map;
using Godot;
using Godot.Collections;

namespace Bitspoke.Ludus.Client.Components.Nodes.Containers;

public abstract partial class NodeContainer<[MustBeVariant] T> : GodotNode2D where T : GodotNode2D

{
    #region Properties
    
    public override string NodeName => GetType().Name;
    public override Node Node => this;

    public Map Map { get; set; }
    public Dictionary<int, T> ContainerNodes { get; private set; } = new();

    #endregion

    #region Constructors and Initialisation

    public NodeContainer(Map map)
    {
        Map = map;
    }
    
    #endregion

    #region Overrides

    public override void _Ready()
    {
        base._Ready();
        
        InitialiseNodes();
    }

    public override void Init() { }
    public override void AddComponents() { }
    public override void ConnectSignals() { }

    #endregion
    
    #region Methods

    private void InitialiseNodes()
    {
        Profile(() => { 
            for (var regionIndex = 0; regionIndex < Map.Data.RegionsContainer.Regions.Length; regionIndex++)
                this.AddGodotNode(new RegionContainer(Map.Data.RegionsContainer.Regions[regionIndex]));
        });
    }
    
    #endregion
    
}