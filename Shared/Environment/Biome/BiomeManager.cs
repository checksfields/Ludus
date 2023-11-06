using Bitspoke.Ludus.Shared.Environment.Biome.Definitions;

namespace Bitspoke.Ludus.Shared.Environment.Biome
{
    public class BiomeManager
    {
        #region Properties

        private static BiomeManager instance { get; set; } = null;
        public static BiomeManager Instance {
            get
            {
                if (instance == null)
                    instance = new BiomeManager();

                return instance;
            }
        }

        //public Dictionary<int, Biome> MapBiomes { get; set; }

        public BiomeDefsCollection BiomeDefsCollection { get; set; }

        #endregion

        #region Constructors and Initialisation

        #endregion

        #region Methods

        #endregion


    }
}