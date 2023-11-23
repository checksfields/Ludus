using Bitspoke.Core.Common.States.Games;
using Bitspoke.Core.Components.Location;
using Bitspoke.Core.Definitions.Parts.Entity.Living;
using Bitspoke.Core.Random;
using Bitspoke.Core.Utils.Primatives.Float;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Common.TypeDatas.Game.States;
using Bitspoke.Ludus.Shared.Components.Entities.Living;
using Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;
using Bitspoke.Ludus.Shared.Systems.Spawn;
using Bitspoke.Ludus.Shared.Systems.Spawn.Natural.Plants;
using Godot;

namespace Bitspoke.Ludus.Shared.Entities.Natural.Plants;

public class Plant : LudusSpawnableEntity
{
    #region Properties
    
    public override string EntityName => nameof(Plant);

    public PlantDef? PlantDef => (Def is PlantDef def) ? def : null;
    
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
        
        // if (GameStateManager.IsCurrentState(LudusGameStatesTypeData.MAP_GENERATION_KEY))
        // {
        //     InitAge();
        //     InitGrowth();
        // }
        
        AddAgeComponent(); 
        AddGrowthComponent();
    }

    private void AddGrowthComponent()
    {
        if (!Def.HasDefComponent<LifeCycleDef>()) 
            return;

        var growthComponent = new GrowthComponent(this);
        
        var initialAge = 0f;
        if (GameStateManager.IsCurrentState(LudusGameStatesTypeData.MAP_GENERATION_KEY))
        {
            var lifeCycleDef = Def.GetDefComponent<LifeCycleDef>();
            growthComponent.CurrentGrowthPercent = Mathf.Clamp(lifeCycleDef.InitialGrowthRange.RandRange(), 0, 1);
        }
        else
            // TODO: Implement for not map gen events (such as in-game spawn)
            Log.TODO("Implement for not map gen events (such as in-game spawn)");
        
        Components.Add(growthComponent);
    }

    private void AddAgeComponent()
    {
        if (!Def.HasDefComponent<AgeDef>()) 
            return;

        var ageComponent = new AgeComponent(this);
        var ageDef =  Def.GetDefComponent<AgeDef>();
        
        ulong maxAge = ageDef.MaxAgeRange.RandRange();
        ulong initialAge = 0u;
        
        if (GameStateManager.IsCurrentState(LudusGameStatesTypeData.MAP_GENERATION_KEY))
        {
            initialAge = ageDef.InitialAgeRange.RandRange();
            initialAge = Math.Min(initialAge, maxAge);
            ulong ageDiffAbs = (ulong) Math.Abs((long)initialAge - (long) maxAge);
            if (ageDiffAbs < CoreGlobal.CalendarConstants.TICKS_PER_HOUR) 
                initialAge = (ulong) (maxAge * Rand.NextFloat(0.9f, 0.95f)).Ceiling();
        }
        else
            // TODO: Implement for not map gen events (such as in-game spawn)
            Log.TODO("Implement for not map gen events (such as in-game spawn)");
        
        ageComponent.Age =  initialAge;
        ageComponent.MaxAge =  maxAge;
        
        Components.Add(ageComponent);
    }
    
    protected override void ConnectSignals() { }

    public override void AddComponents()
    {
        Components.Add(new LocationComponent());
        
        
    }
    
    #endregion

    #region Methods

    private SpawnSystem? spawnSystem { get; set; } = null;
    public override SpawnSystem GetSpawnSystem(ulong mapID)
    {
        if (spawnSystem == null)
            spawnSystem = Find.Map(mapID).GetSpawnSystem<NaturalPlantSpawnSystem>();

        return spawnSystem;
    }
    
    #endregion
    
}