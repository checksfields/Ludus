using Bitspoke.Core.Common.States.Games;
using Bitspoke.Core.Common.Values;
using Bitspoke.Core.Components.Location;
using Bitspoke.Core.Definitions.Parts.Entity.Living;
using Bitspoke.Core.Random;
using Bitspoke.Core.Utils.Primatives.Float;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Common.TypeDatas.Game.States;
using Bitspoke.Ludus.Shared.Components.Entities.Living;
using Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells;
using Bitspoke.Ludus.Shared.Systems.Spawn;
using Bitspoke.Ludus.Shared.Systems.Spawn.Natural.Plants;
using Godot;
using Range = Bitspoke.Core.Common.Values.Range;

namespace Bitspoke.Ludus.Shared.Entities.Natural.Plants;

public class Plant : LudusSpawnableEntity
{
    #region Properties
    
    public override string EntityName => nameof(Plant);

    public PlantDef? PlantDef => (Def is PlantDef def) ? def : null;
    
    public LocationComponent? LocationComponent => GetComponent<LocationComponent>();
    
    private GrowthDef? GrowthDef { get; set; } = null;
    public GrowthComponent?   GrowthComponent   => GetComponent<GrowthComponent>();
    
    private AgeDef? AgeDef { get; set; } = null;
    public AgeComponent?      AgeComponent      => GetComponent<AgeComponent>();
    
    #endregion

    #region Constructors and Initialisation

    public Plant() : base() { }
    
    public Plant(PlantDef def, MapCell containingCell) : this()
    {
        Init(def, containingCell);
    }
    
    protected override void Init() { }

    protected void Init(PlantDef def, MapCell containingCell)
    {
        Def = def;
        MapCell = containingCell;
        
        // if (GameStateManager.IsCurrentState(LudusGameStatesTypeData.MAP_GENERATION_KEY))
        // {
        //     InitAge();
        //     InitGrowth();
        // }
        
        AddGrowthComponent();        
        AddAgeComponent(); 

    }

    private void AddGrowthComponent()
    {
        if (!Def.HasDefComponent<GrowthDef>()) 
            return;

        GrowthDef = Def.GetDefComponent<GrowthDef>();
        var growthComponent = new GrowthComponent(this);

        
        var initialAge = 0f;
        if (GameStateManager.IsCurrentState(LudusGameStatesTypeData.MAP_GENERATION_KEY))
        {
            var biomeKey = MapCell.Map.BiomeDef.Key;
            var growDays = PlantGlobal.DefaultMinGrowDays;
            GrowthDef?.DaysByBiome.TryGetValue(biomeKey, out growDays);
            
            growthComponent.GrowDays = growDays;
            growthComponent.CurrentGrowDaysInTicks = (growDays * PlantGlobal.InitialGrowthRange.Rand()) * CoreGlobal.CalendarConstants.TICKS_PER_DAY;
        }
        else
            // TODO: Implement for not map gen events (such as in-game spawn)
            Log.TODO("Implement for not map gen events (such as in-game spawn)");
        
        Components.Add(growthComponent);
    }

    private void AddAgeComponent()
    {
        if (!Def.HasDefComponent<AgeDef>())
        {
            Log.Warning("Adding an AgeComponent requires an AgeDef be loaded in the Def.", -9999999);
            return;
        }

        // can't calculate the age of without the growth component
        if (GrowthComponent == null)
        {
            Log.Warning("AgeComponent requires the GrowthComponent has already been added/processed.", -9999999);
            return;
        }

        AgeDef =  Def.GetDefComponent<AgeDef>();

        var ageComponent = new AgeComponent(this);
        
        //ulong maxAge = ageDef.MaxAgeRange.RandRange();
        //ulong initialAge = 0u;
        
        if (GameStateManager.IsCurrentState(LudusGameStatesTypeData.MAP_GENERATION_KEY))
        {
            var lifeSpanMultiplier = AgeDef.LifespanGrowDaysMultiplier * (1 + AgeDef.LifespanGrowDaysMultiplierVariation.RandRange());
            var maxAgeInDays = lifeSpanMultiplier * GrowthComponent.GrowDays;
            var growDaysAsPercentOfMaxAge = GrowthComponent.GrowDays / maxAgeInDays;
            var minAgeAsPercent = Math.Max(growDaysAsPercentOfMaxAge, PlantGlobal.InitialAgeRange.Min);
            var initialAge = new Range(maxAgeInDays*minAgeAsPercent, maxAgeInDays).Rand();
            
            // var ageDiffAbs = (ulong) Math.Abs((long)initialAge - (long) maxAge);
            // if (ageDiffAbs < CoreGlobal.CalendarConstants.TICKS_PER_HOUR) 
            //     initialAge = (ulong) (maxAge * Rand.NextFloat(0.9f, 0.95f)).Ceiling();
            
            ageComponent.CurrentAgeInTicks =  (ulong) (initialAge * CoreGlobal.CalendarConstants.TICKS_PER_DAY);
            ageComponent.MaxAge =  maxAgeInDays;
            ageComponent.MaxAgeInTicks =  (ulong) (maxAgeInDays * CoreGlobal.CalendarConstants.TICKS_PER_DAY);
        }
        else
            // TODO: Implement for not map gen events (such as in-game spawn)
            Log.TODO("Implement for not map gen events (such as in-game spawn)");
        
        
        
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