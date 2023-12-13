using Bitspoke.Core.Profiling;
using Bitspoke.GodotEngine.Components.Nodes;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Entities.Containers;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells.Components;
using Bitspoke.Ludus.Shared.Environment.Map.Regions.Components;
using Godot;

namespace Bitspoke.Ludus.Shared.Environment.Map.Entities.Components;

public partial class MapDataCollectionComponent : GodotNode
{
    #region Properties

    private Map? Map { get; set; } = null;
    public override string NodeName => GetType().Name;
    public override Node Node => this;

    // in celsius
    public float Temp { get; set; }
    // 0.0f - 1.0f
    public float Light { get; set; }
    // 0.0f - 1.0f
    public float Moisture { get; set; }
    
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

    public void RemoveEntity(LudusEntity entity)
    {
        MapCell? cell = null;
        if (entity is LudusSpawnableEntity)
            cell = ((LudusSpawnableEntity)entity).MapCell;

        
        EntitiesContainer.Remove(entity, cell);

        if (cell != null)
        {
            RegionsContainer.Regions[cell.RegionIndex]?.ClearEntitiesCache();
            //CellsContainer.CellsByRegion.
        }
        
        
    }
    
    
    public override void Init()
    {
        // TODO - Tier 1 - Calculate Environmental Variables based on season, time of day and current events
        
        
    }
    public override void AddComponents() {}
    public override void ConnectSignals() {}
    
    public void InitialiseContainers()
    {
        Profiler.Start();
        
        RegionsContainer  = new RegionsContainer(Map);
        CellsContainer    = new CellsContainer(Map);
        EntitiesContainer = new EntitiesContainer(Map);

        Profiler.End(message:"+++++++++++++++++++++");
    }

    private void SetEnvironmentalVariables()
    {
        Temp = 30.0f;
        Light = 0.5f;
        Moisture = 0.5f;
    }

    #endregion
    
}