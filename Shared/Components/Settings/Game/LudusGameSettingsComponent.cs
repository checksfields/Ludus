using Bitspoke.Core.Common.States.Games;
using Bitspoke.Core.Common.Vector;
using Bitspoke.Core.Types.Game.States;
using Bitspoke.GodotEngine.Components.Settings.Game;
using Bitspoke.Ludus.Shared;
using Bitspoke.Ludus.Shared.Common.TypeDatas.Game.States;
using Godot;

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

    #region Overrides

    public override void Init()
    {
        base.Init();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        
        if (@event is not InputEventKey)
            return;
        
        var supportedGameStates = new List<string>
        {
            LudusGameStatesTypeData.IN_GAME_KEY, 
            LudusGameStatesTypeData.MAIN_KEY
        };
        
        if (!GameStateManager.IsCurrentState(supportedGameStates))
            return;
		
        if (Input.IsKeyPressed(Key.Escape))
            TogglePopup();
    }

    #endregion
    
    #region Methods

    public void DestroyInstance()
    {
        instance = null;
    }

    #endregion


}