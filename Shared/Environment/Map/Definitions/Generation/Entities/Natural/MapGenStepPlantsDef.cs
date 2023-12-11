using System.Text.Json.Nodes;
using Bitspoke.Core.Definitions;
using Bitspoke.Core.Utils.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation.Entities.Natural;

public class MapGenStepPlantsDef : MapGenStepDef
{
    #region Properties

    public float IndependentSpawnChance { get; set; }
    
    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Overrides

    public override IDef Deserialize(JsonNode node)
    {
        return node.DeserializeAnonymousType(this);
    }

    #endregion
    
    #region Methods

    #endregion

    
}