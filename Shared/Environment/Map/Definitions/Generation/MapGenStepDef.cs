using Bitspoke.Core.Definitions.Generation;
using Bitspoke.Core.Definitions.Parts.Common.Noise;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Generation
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class MapGenStepDef : GenStepDef
    {
        #region Properties

        public override string Key { get; set; }
        public NoiseDef NoiseDef { get; set; }
        
        #endregion

        #region Constructors and Initialisation

        #endregion

        #region Methods

        #endregion

        
    }
}