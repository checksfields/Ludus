﻿using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Bitspoke.Core.Common.Localisation;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Definitions.Parts.Common;
using Bitspoke.Core.Definitions.Parts.Entity.Living;
using Bitspoke.Core.Definitions.Parts.Graphics;
using Bitspoke.Core.Utils.Enums;
using Bitspoke.Core.Utils.Json;
using Bitspoke.Ludus.Shared.Common.Components.Movement;
using Bitspoke.Ludus.Shared.Common.Definitions.Movement;
using Bitspoke.Ludus.Shared.Common.Definitions.Placement;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Common.Entities.Utils;
using Bitspoke.Ludus.Shared.Common.Reports;
using Bitspoke.Ludus.Shared.Common.Reports.Placement;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells;

namespace Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;

public class PlantDef : EntityDef, IGraphicDef
{
    #region Properties
    
    [JsonIgnore] public override EntityType Type { get; set; } = EntityType.Plant;
    [JsonIgnore] public override int        SubTypesFlag => SubTypes.Int();
    public PlantType SubTypes      { get; set; } = PlantType.Undefined;

    [JsonIgnore] public bool  IsWild     => PlantDetails.IsWild;
    [JsonIgnore] public bool  CanCluster => PlantDetails.CanCluster;
    [JsonIgnore] public float Order      { get; set; }

    [JsonIgnore] public RangeDef<float>?  FertilityRange  => GetDefComponent<RangeDef<float>>("fertilityrange");
    [JsonIgnore] public MovementCostDef?  MovementCostDef => GetDefComponent<MovementCostDef>();
    [JsonIgnore] public PlacementMaskDef? PlacementMask   => GetDefComponent<PlacementMaskDef>();
    //[JsonIgnore] public ClusterDef?       ClusterDef      => GetDefComponent<ClusterDef>();
    //[JsonIgnore] public GraphicDef?       GraphicDef      => GetDefComponent<GraphicDef>();

    // NEW Format
    [JsonPropertyName("Graphic")]
    public GraphicDef Graphic
    {
        get => GetDefComponent<GraphicDef>();
        set => TryAddDefComponent(value);
    }

    [JsonPropertyName("Plant")]     public PlantDetailsDef PlantDetails { get; set; }
    
    [JsonPropertyName("Age")]       
    public AgeDef Age 
    {
        get => GetDefComponent<AgeDef>();
        set => TryAddDefComponent(value);
    }

    [JsonPropertyName("Grow")]
    public GrowthDef? Growth
    {
        get => GetDefComponent<GrowthDef>();
        set => TryAddDefComponent(value);
    }


    [JsonPropertyName("Placement")] public PlacementMaskDef Placement { get; set; }
    
    #endregion

    #region Constructors and Initialisation

    public PlantDef()
    {
        var placementMask = new PlacementMaskDef();
        placementMask.BannedPlacements.Add(EntityType.Item, -1);
        placementMask.BannedPlacements.Add(EntityType.Pawn, -1);
        placementMask.BannedPlacements.Add(EntityType.Structure, -1);
        placementMask.BannedPlacements.Add(EntityType.Plant, (PlantType.Tree).Int());
        
        AddDefComponent(placementMask);
    }

    public PlantDef Clone()
    {
        var clone = new PlantDef();
        base.Clone(clone);

        //clone.SubTypes   = SubTypes;
        //clone.IsWild     = IsWild;
        clone.Order      = Order;

        clone.Graphic = Graphic.Clone();
        clone.PlantDetails = PlantDetails.Clone();
        clone.Age = Age.Clone();
        clone.Growth = Growth.Clone();
        
        return clone;
    }
    
    #endregion

    #region Methods
    
    public override IDef Deserialize(JsonNode node)
    {
        return node.DeserializeAnonymousType(this);
    }
    
    public PlacementReport CanPlaceAt(MapCell mapCell)
    {
        var placementReport = new PlacementReport();
        var mapCellEntities = mapCell.EntitiesNew;
        
        // MAP FERTILITY
        var fertility = mapCell.Fertility;
        if (fertility < FertilityRange?.Min)
        {
            placementReport.Status = ReportStatus.RejectedWithMessages;
            placementReport.Messages.Add("MapCellFertilityToLow");
        }
        // if (fertility > FertilityRange?.Max)
        // {
        //     placementReport.Status = ReportStatus.RejectedWithMessages;
        //     placementReport.Messages.Add("MapCellFertilityToHigh");
        // }
        
        // TERRAIN FERTILITY
        fertility = Find.DB.TerrainDefs[mapCell.TerrainDefKey].Fertility.Value; 
        if (fertility < FertilityRange?.Min)
        {
            placementReport.Status = ReportStatus.RejectedWithMessages;
            placementReport.Messages.Add("TerrainFertilityToLow");
        }
        // if (fertility > FertilityRange?.Max)
        // {
        //     placementReport.Status = ReportStatus.RejectedWithMessages;
        //     placementReport.Messages.Add("TerrainFertilityToHigh");
        // }
        
        if (mapCell.EntitiesNew?.IsFull ?? false)
        {
            placementReport.Status = ReportStatus.RejectedWithMessages;
            placementReport.Messages.Add("CellIsFull");
        }
        
        if (placementReport.Status.HasFlag(ReportStatus.Rejected) || placementReport.Status.HasFlag(ReportStatus.RejectedWithMessages))
            return placementReport;

        if (mapCellEntities == null || mapCellEntities.Count < 1)
        {
            placementReport.Status = ReportStatus.Accepted;
            return placementReport;
        }
        
        var isPlantImpassable = MovementCostDef?.Is(MovementCostType.Impassable) ?? false;
        
        //check the entities in this cell again
        foreach (var ent in mapCellEntities)
        {
            var entity = (LudusEntity) ent;
            var isBlockedBy = IsBlockedBy(entity);
            
            if (isBlockedBy)
            {
                placementReport.Status = ReportStatus.RejectedWithMessages;
                placementReport.Messages.Add("BlockedBy".Translate(messageArgs: entity.EntityName));
            }

            var isEntityImpassable = entity.MovementCost()?.Is(MovementCostType.Impassable) ?? false;
                
            // if the plant is not something we can pass through ...  
            if (isPlantImpassable)
            {
                // ... is this entity something that would cause an issue by placing it here
                
            }
        }

        placementReport.Status = ReportStatus.Accepted;
        return placementReport;
    }

    public bool IsBlockedBy(LudusEntity entity)
    {
        bool isBlocked = false;
        foreach (var placementMask in PlacementMask.BannedPlacements)
        {
            if (entity.Def.Type == placementMask.Key)
            {
                if (placementMask.Value == -1)
                    return true;
        
                var hasSubTypeFlag = entity.Def.HasSubTypeFlag(placementMask.Value);
                isBlocked &= hasSubTypeFlag;
            }
            
        }
        
        return isBlocked;
    }

    public override bool HasSubTypeFlag(int? flagAsInt)
    {
        var flag = (PlantType) flagAsInt.Value;
        var isSubType = (SubTypes & flag) != 0;
        
        return isSubType;
    }

    private bool Is(PlantType type)
    {
        return Type.HasFlag(type);
    }
    
    #endregion
}