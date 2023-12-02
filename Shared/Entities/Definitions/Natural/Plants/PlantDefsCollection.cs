using Bitspoke.Core.Definitions.Collections;
using Bitspoke.Core.Definitions.Parts.Common;
using Bitspoke.Core.Definitions.Parts.Entity.Living;
using Bitspoke.Core.Definitions.Parts.Graphics;
using Bitspoke.Core.Definitions.Parts.Graphics.Textures;
using Bitspoke.Core.Definitions.Parts.Graphics.Textures.Types;
using Bitspoke.Core.Profiling;
using Bitspoke.Core.Systems.Calendar;
using Bitspoke.Core.Utils.Primatives.Float;
using Bitspoke.GodotEngine.Utils.IO;
using Bitspoke.Ludus.Shared.Common.Components.Movement;
using Bitspoke.Ludus.Shared.Common.Definitions.Movement;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;

public class PlantDefsCollection : DefCollection<PlantDef>
{
    #region Properties

    public override string Key { get; set; } = nameof(PlantDefsCollection);
    public override string ClassName => GetType().FullName;
    public override string? AssemblyName => GetType().Assembly.GetName().Name;
    
    #endregion

    #region Constructors and Initialisation

    
    
    #endregion

    #region Methods

    #endregion

    #region Bootstrap

    public static PlantDefsCollection Bootstrap(bool writeToFile = false)
    {
        Profiler.Start();

        var defs = new PlantDefsCollection();

        var key = "";

        defs.Add(key = "grass", Bootstrap_BasicGrass(key, "grassa"));
        defs.Add(key = "tallgrass", Bootstrap_BasicTallGrass(key, "grassb"));
        defs.Add(key = "brambles", Bootstrap_Brambles(key, "bramblesa"));
        defs.Add(key = "bush", Bootstrap_BasicBush(key, "busha"));
        
        defs.Add(key = "dandelion", Bootstrap_Dandelion(key, "dandelion"));
        
        defs.Add(key = "treeoak",     Bootstrap_BasicTree(key, "treeoaka"));
        defs.Add(key = "treepoplar", Bootstrap_BasicTree(key, "treepoplara"));
        
        defs.Add(key = "healroot", Bootstrap_Healroot(key, "healroota"));
        defs.Add(key = "berryplant", Bootstrap_Berry(key, "berrybusha"));

        
        
        if (writeToFile)
        {
            var filePath = $"{GodotGlobal.RES_ROOT_PATH}{GodotGlobal.DEFINITIONS_ROOT_PATH}/{nameof(PlantDefsCollection)}{GodotGlobal.SUPPORTED_DEF_TYPE}";
            GodotFileUtils.WriteToFile(filePath, JsonConvert.SerializeObject(defs, Formatting.Indented, CoreGlobal.JsonSerializerSettings));
        }
            
        Profiler.End();
        return defs;
    }

    private static PlantDef Bootstrap_BasicTree(string key, string textureKey)
    {
        var plantDef = new PlantDef()
        {
            Key = key,
            IsWild = true,
            SubTypes = PlantType.Tree | PlantType.Deciduous,
            Ascii = 'T',
            Order = 3.0f,
        };
        plantDef.AddDefComponent(new ClusterDef() { Radius = 8, Wieght = 20});
        plantDef.AddDefComponent("fertilityrange", new RangeDef<float>(0.6f, 1f));
        plantDef.AddDefComponent(new MovementCostDef() { Type = MovementCostType.Impassable, Cost = 1f });
        var graphicDef_Tree = new GraphicDef();
        graphicDef_Tree.TextureDef = new TextureDef()
        {
            TextureResourcePath = $"Entities/Plants/Natural/Tree/{textureKey}",
            TextureTypeDetails = new SingleTextureTypeDetailsDef()
        };
        plantDef.AddDefComponent(graphicDef_Tree);
        
        AddAgeDef(plantDef);
        AddLifeCycleDef(plantDef);
        
        return plantDef;
    }
    
    private static PlantDef Bootstrap_BasicBush(string key, string textureKey)
    {
        var plantDef = new PlantDef()
        {
            Key = key,
            IsWild = true,
            SubTypes = PlantType.Bush,
            Ascii = 'B',
            Order = 2.0f,
        };
        plantDef.AddDefComponent(new ClusterDef() { Radius = 4, Wieght = 5});
        plantDef.AddDefComponent("fertilityrange", new RangeDef<float>(0.45f, 1f));
        var graphicDef_Bush = new GraphicDef();
        graphicDef_Bush.ScaleRange = new RangeDef<float>(0.7f, 1.1f);
        graphicDef_Bush.TextureDef = new TextureDef()
        {
            TextureResourcePath = "Entities/Plants/Natural/Bush/busha",
            TextureTypeDetails = new SingleTextureTypeDetailsDef()
        };
        plantDef.AddDefComponent(graphicDef_Bush);
        
        AddAgeDef(plantDef, 0.075f);
        AddLifeCycleDef(plantDef);
        
        return plantDef;
    }
    
    private static PlantDef Bootstrap_BasicGrass(string key, string textureKey)
    {
        var plantDef = new PlantDef()
        {
            Key = key,
            IsWild = true,
            SubTypes = PlantType.Cover,
            Ascii = 'g',
            Order = 1.0f,
        };
        plantDef.AddDefComponent(new ClusterDef() { Radius = 4, Wieght = 10});
        plantDef.AddDefComponent("fertilityrange", new RangeDef<float>(0.35f, 1f));
        
        var graphicDef_Grass = new GraphicDef();
        graphicDef_Grass.ScaleRange = new RangeDef<float>(0.4f, 0.6f);
        graphicDef_Grass.TextureDef = new TextureDef()
        {
            TextureResourcePath = "Entities/Plants/Natural/Grass/grassa",
            TextureTypeDetails = new MultiMeshTextureTypeDetailsDef()
            {
                MaxCountInCell = 9,
                LocationVariationX = new RangeDef<int>(-24, 24),
                LocationVariationY = new RangeDef<int>(-24, 24),
            },
            Opacity = 0.65f,
        };
        plantDef.AddDefComponent(graphicDef_Grass);

        AddAgeDef(plantDef, 0.1f);
        AddLifeCycleDef(plantDef);
     
        return plantDef;
    }
    
    private static PlantDef Bootstrap_BasicTallGrass(string key, string textureKey)
    {
        var plantDef = new PlantDef()
        {
            Key = key,
            IsWild = true,
            SubTypes = PlantType.Cover,
            Ascii = 'G',
            Order = 1.0f,
        };
        plantDef.AddDefComponent(new ClusterDef() { Radius = 4, Wieght = 10});
        plantDef.AddDefComponent("fertilityrange", new RangeDef<float>(0.35f, 1f));
        
        var graphicDef_Grass = new GraphicDef();
        graphicDef_Grass.ScaleRange = new RangeDef<float>(0.6f, 0.85f);
        graphicDef_Grass.TextureDef = new TextureDef()
        {
            TextureResourcePath = $"Entities/Plants/Natural/Grass/{textureKey}",
            TextureTypeDetails = new MultiMeshTextureTypeDetailsDef()
            {
                MaxCountInCell = 9,
                LocationVariationX = new RangeDef<int>(-24, 24),
                LocationVariationY = new RangeDef<int>(-24, 24),
            },
            Opacity = 0.65f,
        };
        plantDef.AddDefComponent(graphicDef_Grass);

        AddAgeDef(plantDef, 0.09f);
        AddLifeCycleDef(plantDef, 3f);

        return plantDef;
    }
    
    private static PlantDef Bootstrap_Brambles(string key, string textureKey)
    {
        var plantDef = new PlantDef()
        {
            Key = key,
            IsWild = true,
            SubTypes = PlantType.Cover,
            Ascii = 'r',
            Order = 1.0f,
        };
        plantDef.AddDefComponent(new ClusterDef() { Radius = 4, Wieght = 10});
        plantDef.AddDefComponent("fertilityrange", new RangeDef<float>(0.7f, 1f));
        
        var graphicDef_Brambles = new GraphicDef();
        graphicDef_Brambles.ScaleRange = new RangeDef<float>(0.7f, 0.85f);
        graphicDef_Brambles.TextureDef = new TextureDef()
        {
            TextureResourcePath = $"Entities/Plants/Natural/Brambles/{textureKey}",
            TextureTypeDetails = new MultiMeshTextureTypeDetailsDef()
            {
                MaxCountInCell = 4,
                LocationVariationX = new RangeDef<int>(-24, 24),
                LocationVariationY = new RangeDef<int>(-24, 24),
            }
        };
        plantDef.AddDefComponent(graphicDef_Brambles);

        AddAgeDef(plantDef, 0.08f);
        AddLifeCycleDef(plantDef, 3f);
     
        return plantDef;
    }
    
    private static PlantDef Bootstrap_Healroot(string key, string textureKey)
    {
        var plantDef = new PlantDef()
        {
            Key = key,
            IsWild = true,
            SubTypes = PlantType.Bush | PlantType.Harvestable,
            Ascii = 'H',
            Order = 3.0f,
        };
        plantDef.AddDefComponent(new ClusterDef() { Radius = 8, Wieght = 20});
        plantDef.AddDefComponent("fertilityrange", new RangeDef<float>(0.7f, 1f));
        plantDef.AddDefComponent(new MovementCostDef() { Type = MovementCostType.Impassable, Cost = 1f });
        var graphicDef_Healroot = new GraphicDef();
        graphicDef_Healroot.ScaleRange = new RangeDef<float>(0.3f, 1.0f);
        graphicDef_Healroot.TextureDef = new TextureDef()
        {
            TextureResourcePath = $"Entities/Plants/Natural/Healroot/{textureKey}",
            TextureTypeDetails = new SingleTextureTypeDetailsDef()
        };
        plantDef.AddDefComponent(graphicDef_Healroot);
        
        AddAgeDef(plantDef, 0.075f);
        AddLifeCycleDef(plantDef, 10f);
        
        return plantDef;
    }
    
    private static PlantDef Bootstrap_Berry(string key, string textureKey)
    {
        var plantDef = new PlantDef()
        {
            Key = key,
            IsWild = true,
            SubTypes = PlantType.Bush | PlantType.Harvestable,
            Ascii = 'S',
            Order = 3.0f,
        };
        plantDef.AddDefComponent(new ClusterDef() { Radius = 8, Wieght = 20});
        plantDef.AddDefComponent("fertilityrange", new RangeDef<float>(0.5f, 1f));
        plantDef.AddDefComponent(new MovementCostDef() { Type = MovementCostType.Impassable, Cost = 1f });
        var graphicDef_Berry = new GraphicDef();
        graphicDef_Berry.ScaleRange = new RangeDef<float>(0.3f, 0.6f);
        graphicDef_Berry.TextureDef = new TextureDef()
        {
            TextureResourcePath = $"Entities/Plants/Natural/BerryPlant/{textureKey}",
            TextureTypeDetails = new SingleTextureTypeDetailsDef()
        };
        plantDef.AddDefComponent(graphicDef_Berry);
        
        AddAgeDef(plantDef, 0.075f);
        AddLifeCycleDef(plantDef, 6f);
        
        return plantDef;
    }
    
    private static PlantDef Bootstrap_Dandelion(string key, string textureKey)
    {
        var plantDef = new PlantDef()
        {
            Key = key,
            IsWild = true,
            SubTypes = PlantType.Cover,
            Ascii = 'D',
            Order = 3.0f,
        };
        plantDef.AddDefComponent(new ClusterDef() { Radius = 8, Wieght = 20});
        plantDef.AddDefComponent("fertilityrange", new RangeDef<float>(0.7f, 1f));
        plantDef.AddDefComponent(new MovementCostDef() { Type = MovementCostType.Impassable, Cost = 1f });
        var graphicDef_Dandelion = new GraphicDef();
        graphicDef_Dandelion.ScaleRange = new RangeDef<float>(0.3f, 0.4f);
        graphicDef_Dandelion.TextureDef = new TextureDef()
        {
            TextureResourcePath = $"Entities/Plants/Natural/Dandelion/{textureKey}",
            TextureTypeDetails = new MultiMeshTextureTypeDetailsDef()
            {
                MaxCountInCell = 25,
                LocationVariationX = new RangeDef<int>(-24, 24),
                LocationVariationY = new RangeDef<int>(-24, 24),
            }
        };
        plantDef.AddDefComponent(graphicDef_Dandelion);
        
        AddAgeDef(plantDef, 0.1f);
        AddLifeCycleDef(plantDef);
        
        return plantDef;
    }

    private static void AddAgeDef(PlantDef def, float? ageToAppendOnTick = null)
    {
        var ageDef = new AgeDef();

        if (ageToAppendOnTick == null)
            ageToAppendOnTick = 0.01f;
        
        ageDef.AgeToAppendOnTick = ageToAppendOnTick.Value;
        
        ageDef.MinAgeRange = new RangeDef<ulong>(0u, CoreGlobal.CalendarConstants.TICKS_PER_DAY);
        var maxAgeMin = (ulong)(CoreGlobal.CalendarConstants.TICKS_PER_YEAR * 0.7f).Ceiling();
        ageDef.MaxAgeRange = new RangeDef<ulong>(maxAgeMin, CoreGlobal.CalendarConstants.TICKS_PER_YEAR);
        ageDef.InitialAgeRange = new RangeDef<ulong>(0u, maxAgeMin);
        
        def.AddDefComponent(ageDef);
    }
    
    private static void AddLifeCycleDef(PlantDef def, 
        float growDays = 2.5f, 
        RangeDef<float> initialGrowRange = null,
        float growthDaysPercentageOfLifespan = 0.125f)
    {
        if (initialGrowRange == null)
            initialGrowRange = new RangeDef<float>(0.7f, 1.5f);
        
        LifeCycleDef growthDef;
        def.AddDefComponent(growthDef = new LifeCycleDef()
        {
            GrowDays = growDays,
            InitialGrowthRange = initialGrowRange,
            GrowthDaysPercentageOfLifespan = growthDaysPercentageOfLifespan
        });
    }
    
    
    #endregion
    
}