using Bitspoke.Core.Common.Vector;
using Bitspoke.Core.Components.Identity;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Environment.Map;

namespace Bitspoke.Ludus.Shared.Environment.World
{
    public class World : LudusEntity
    {
        #region Properties

        public IDComponent WorldID => base.IDComponent;
        public override string EntityName => nameof(World);
        public WorldInitConfig? WorldInitConfig { get; set; } = null;

        public Vec2Int Dimension { get; set; }

        public MapContainer Maps { get; set; }
        
        #endregion

        #region Constructors and Initialisation

        public World(WorldInitConfig initConfig) : base()
        {
            InitFromConfig(initConfig);
        }

        protected override void Init()
        {
            
        }
        protected override void ConnectSignals() { }
        public override void AddComponents() { }
        
        #endregion

        #region Methods

        public void InitFromConfig(WorldInitConfig initConfig)
        {
            WorldInitConfig = initConfig;
            Dimension = WorldInitConfig.Dimensions;
            
            Maps = new MapContainer(Dimension.Area);
        }
        
        #endregion


        
    }
}