using BaseManager = Bitspoke.GodotEngine.Managers.Manager;
using Bitspoke.Ludus.Shared.Environment.Map.Generation;

namespace Bitspoke.Ludus.Shared.Environment.Map;

public partial class MapManager : BaseManager
{
    #region Properties
        
    private static MapManager instance { get; set; } = null;
    public static MapManager Instance
    {
        get
        {
            if (instance == null)
                instance = new MapManager();

            return instance;
        }
    }
    
    public override string Name => GetType().Name;
        
    #endregion

    #region Constructors and Initialisation
        
    #endregion

    #region Overrides

    protected override void Init() {}
    protected override void AddComponents() {}
    protected override void ConnectSignals() {}

    #endregion
    
    #region Methods

    public static Map GenerateMap(MapInitConfig initConfig)
    {
        return MapGenerator.Generate(initConfig);
    }
        
    #endregion


        
}