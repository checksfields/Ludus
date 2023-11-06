using Bitspoke.Core.Definitions;
using Bitspoke.Core.Definitions.Parts.Common;

namespace Bitspoke.Ludus.Shared.Environment.Biome.Definitions
{
    public class BiomeDef : Def
    {
        #region Properties

        public override string Key { get; set; }
        public override string ClassName => GetType().FullName;
        public override string AssemblyName => GetType().Assembly.GetName().Name;

        
        public BiomePlantDef BiomePlantsDef { get; set; }
        public BiomeAnimalsDef    BiomeAnimalsDef { get; set; }
        
        public Dictionary<string, ProbabilityDef> ElevationTerrainDefs { get; set; } 
        public Dictionary<string, ProbabilityDef> FertilityTerrainDefs { get; set; } 
        
        public List<PatchDef> TerrainPatchDefs { get; set; }
        
        #endregion

        #region Constructors and Initialisation

        public BiomeDef()
        {
            Init();
        }

        private void Init()
        {
            ElevationTerrainDefs = new Dictionary<string, ProbabilityDef>();
            FertilityTerrainDefs = new Dictionary<string, ProbabilityDef>();
            TerrainPatchDefs     = new List<PatchDef>();
        }
        
        #endregion

        #region Methods

        #endregion

        #region Bootstrap

        

        #endregion

    }
}