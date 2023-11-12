using Bitspoke.Ludus.Shared.Common.Controllers.Admin;
using Console = Bitspoke.GodotEngine.Components.Console.Console;

namespace Bitspoke.Ludus.Client.Components.Admin;

public partial class AdminConsole : Console
{
    #region Properties

    public override string Name => nameof(AdminConsole);
    

    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Overrides

    public override void AddComponents() {}
    public override void ConnectSignals() {}

    #endregion
    
    #region Methods

    #endregion


    public AdminConsole() : base(new AdminCommandController())
    {
        CommandController.OutputConsole = this;
    }

}