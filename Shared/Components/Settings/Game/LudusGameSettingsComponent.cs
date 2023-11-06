using Bitspoke.Core.Common.Vector;
using Bitspoke.GodotEngine.Components.Settings.Game;
using Bitspoke.Ludus.Shared;

namespace Shared.Components.Settings.Game;

public partial class LudusGameSettingsComponent : GameSettingsComponent
{
    #region Properties

    private static LudusGameSettingsComponent? instance { get; set; } = null;
    public static LudusGameSettingsComponent Instance => instance ?? new LudusGameSettingsComponent();


    public Vec3Int? MapSize { get; set; } = Global.MAX_MAP_DIMENSIONS;

    #endregion

    #region Constructors and Initialisation

    public LudusGameSettingsComponent()
    {
        if (instance != null)
            Log.Exception("Cannot have more than one instance of GameSettings per game.", -9999999);
        instance = this;
    }
    
    #endregion

    #region Methods

    public void DestroyInstance()
    {
        instance = null;
    }

    #endregion


}