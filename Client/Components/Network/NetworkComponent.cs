using Bitspoke.Core.Common.Logging;
using Bitspoke.Core.Models.Server;
using Bitspoke.Core.Models.User;
using Bitspoke.Core.Signal;
using Bitspoke.GodotEngine.Components.Nodes;
using Bitspoke.GodotEngine.Components.Security;
using Bitspoke.GodotEngine.Components.Server.Game;
using Bitspoke.GodotEngine.Components.Server.Gateway;
using Godot;

namespace Client.Components.Network;

public partial class NetworkComponent : GodotNode2D
{
    #region Properties

    private static NetworkComponent? instance = null;
    public static NetworkComponent Instance {
        get
        {
            if (instance == null)
                Log.Exception("Instance has not been set.  Instance must be set during the Godot process life cycle", 1000000);

            return instance!;
        }
    }
        
    public GatewayComponent? GatewayComp { get; set; }
    public AuthenticationComponent? AuthenticationComp { get; set; }
    public GameServerComponent? GameServerComp { get; set; }
        
    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    #endregion


    protected override void Init()
    {
        instance = this;
    }

    protected override void AddComponents()
    {
        AddChild(GatewayComp = new GatewayComponent(new ServerConnection(1910, "127.0.0.1")));
        AddChild(AuthenticationComp = new AuthenticationComponent());
        GetTree().Root.CallDeferred("add_child", GameServerComp = new GameServerComponent(new ServerConnection(1909, "127.0.0.1")));
    }

    protected override void ConnectSignals()
    {
        //SignalManager.Connect(new SignalDetails(READY, typeof(GodotNode2D), this, nameof(OnReady)));
        SignalManager.Connect(new SignalDetails(AuthenticationComponent.AUTHENTICATION_SUCCESS, typeof(AuthenticationComponent), this, nameof(OnAuthenticationSuccess)));
    }

    public override void _Ready()
    {
        base._Ready();
        RunTests();
    }

    private void OnAuthenticationSuccess(string token)
    {
        Log.Info();
            
        // Connect to the game server
        SignalManager.Emit(GameServerComponent.GAME_SERVER_CONNECT_REQUEST, token);
    }
        
    private void RunTests()
    {
        if (!CoreGlobal.RUN_TESTS_ENABLED)
            return;
            
        Log.Info();
            
        AuthenticationComp?.Authenticate(new AuthenticationRequest() { User = new User() { Password = "password".Sha256Text(), UserName = "username" } });
    }
}