using System.Text.Json.Nodes;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Definitions.Parts;
using Bitspoke.Core.Utils.Json;
using Bitspoke.Ludus.Shared.Common.Entities;

namespace Bitspoke.Ludus.Shared.Common.Definitions.Placement;

public class PlacementMaskDef : DefPart
{
    #region Properties
    
    public Dictionary<EntityType, int?> BannedPlacements { get; set; }

    #endregion

    #region Constructors and Initialisation

    public PlacementMaskDef()
    {
        BannedPlacements = new Dictionary<EntityType, int?>();
    }
    
    public override T Clone<T>()
    {
        var def = new PlacementMaskDef();

        def.BannedPlacements = new Dictionary<EntityType, int?>(BannedPlacements);

        return def as T;
    }

    public PlacementMaskDef Clone()
    {
        return Clone<PlacementMaskDef>();
    }
    
    #endregion

    #region Methods
    public override IDef Deserialize(JsonNode node)
    {
        return node.DeserializeAnonymousType(this);
    }
    #endregion



}