using Bitspoke.GodotEngine.Components.Nodes;
using Bitspoke.Ludus.Shared.Entities.Containers;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells.Components;
using Bitspoke.Ludus.Shared.Environment.Map.Regions.Components;

namespace Bitspoke.Ludus.Shared.Environment.Map.Entities.Components;

public partial class MapDataCollectionComponent : GodotNode
{
    #region Properties

    private Map? Map { get; set; } = null;
    public override string Name => GetType().Name;

    #region Containers

    public EntitiesContainer EntitiesContainer { get; set; }
    public CellsContainer    CellsContainer    { get; set; }
    public RegionsContainer  RegionsContainer  { get; set; }

    #endregion


    #endregion

    #region Constructors and Initialisation

    public MapDataCollectionComponent(Map map)
    {
        Map = map;
    }
    
    #endregion

    #region Methods

    protected override void Init() {}
    protected override void AddComponents() {}
    protected override void ConnectSignals() {}
    
    public void InitialiseContainers()
    {
        Profiler.Start();
        
        EntitiesContainer = new EntitiesContainer();
        RegionsContainer  = new RegionsContainer(Map);
        CellsContainer    = new CellsContainer(Map);

        Profiler.End(message:"+++++++++++++++++++++");
    }

    #endregion
    
}