using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation.Structures.Natural
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MapGenStepMineralFormationsDef : MapGenStepDef
    {
        #region Properties

        public float? MinElevation { get; set; }

        #endregion

        #region Constructors and Initialisation

        #endregion

        #region Methods

        #endregion

        
    }
}