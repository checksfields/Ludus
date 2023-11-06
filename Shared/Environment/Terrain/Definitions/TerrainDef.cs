using Bitspoke.Core.Definitions;
using Bitspoke.Core.Definitions.Parts.Graphics;
using Bitspoke.Ludus.Shared.Common.Definitions.Placement;
using Newtonsoft.Json;

namespace Bitspoke.Ludus.Shared.Environment.Terrain.Definitions
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class TerrainDef : Def
    {
        #region Properties
        public override string Key { get; set; }
        public override string ClassName => GetType().FullName;
        public override string? AssemblyName => GetType().Assembly.GetName().Name;

        public float? Fertility { get; set; } = null;
        public bool IsNaturalStructure { get; set; } = false;
        public List<PlacementMaskDef> AcceptablePlacementMaskDefs { get; set; } = null;

        public GraphicDef GraphicDef { get; set; } = null;

        #endregion

        #region Constructors and Initialisation

        #endregion

        #region Methods

        #endregion

    }
}