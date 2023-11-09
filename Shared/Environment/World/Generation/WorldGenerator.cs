namespace Bitspoke.Ludus.Shared.Environment.World.Generation;

public class WorldGenerator
{
    #region Properties

    #endregion

    #region Constructors and Initialisation

        
        
    #endregion

    #region Methods

    public static World Generate(WorldInitConfig initConfig)
    {
            
        var world = new World(initConfig);

        return world;
    }
        
    #endregion

        
}