using Bitspoke.Core.Common.States.Games;
using Bitspoke.Core.Components.Life;
using Bitspoke.Core.Components.Location;
using Bitspoke.Core.Definitions.Parts.Entity.Living;
using Bitspoke.Core.Random;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Common.TypeDatas.Game.States;
using Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;
using Bitspoke.Ludus.Shared.Entities.Systems.Spawn.Natural.Plants;
using Bitspoke.Ludus.Shared.Systems.Spawn;
using Godot;

namespace Bitspoke.Ludus.Shared.Entities.Natural.Plants;

public class Plant : LudusSpawnableEntity
{
    #region Properties
    
    public override string EntityName => nameof(Plant);

    public LocationComponent LocationComponent => GetComponent<LocationComponent>();
    public GrowthComponent   GrowthComponent   => GetComponent<GrowthComponent>();
    public AgeComponent      AgeComponent      => GetComponent<AgeComponent>();
    
    #endregion

    #region Constructors and Initialisation

    public Plant() : base() { }
    
    public Plant(PlantDef def) : this()
    {
        Init(def);
    }
    
    protected override void Init() { }

    protected void Init(PlantDef def)
    {
        Def = def;
        
        if (GameStateManager.IsCurrentState(LudusGameStatesTypeData.MAP_GENERATION_KEY))
        {
            if (Def.HasDefComponent<LifeCycleDef>())
            {
                var lifeCycleDef = Def.GetDefComponent<LifeCycleDef>();
                var lifespanDays = lifeCycleDef.LifespanDays;
                
                GrowthComponent.Growth =  Mathf.Clamp(lifeCycleDef.InitialGrowthRange.RandRange(), 0, 1);
                var lifespanTicks = (int)(lifespanDays * Global.PLANT_GROW_DAY_TICKS);
                AgeComponent.Age =  Rand.NextInt(0, Mathf.Max(lifespanTicks - 50, 0));
            }
        }
    }
    
    protected override void ConnectSignals() { }

    public override void AddComponents()
    {
        Components.Add(new LocationComponent());
        Components.Add(new GrowthComponent());
        Components.Add(new AgeComponent());
    }
    
    #endregion

    #region Methods

    private SpawnSystem? spawnSystem { get; set; } = null;
    public override SpawnSystem GetSpawnSystem(int mapID)
    {
        if (spawnSystem == null)
            spawnSystem = Find.Map(mapID).GetSpawnSystem<NaturalPlantSpawnSystem>();

        return spawnSystem;
    }
    
    #endregion
    
}