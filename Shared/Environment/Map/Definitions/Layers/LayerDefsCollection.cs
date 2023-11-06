using Bitspoke.Core.Definitions;
using Bitspoke.Core.Definitions.Collections;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers;

public abstract class LayerDefsCollection<T> : DefCollection<T>, ILayerDefsCollection where T : IDef
{
    #region Properties

    [JsonRequired] public override string Key { get; set; }
    [JsonRequired] public int OrderIndex { get; set; }

    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    #endregion

    #region Bootstrap

    
    
    #endregion
}