using Bitspoke.Ludus.Shared.Common.Controllers.Admin;
using Console = Bitspoke.GodotEngine.Components.Console.Console;

namespace Client.Components.Admin;

public partial class AdminConsole : Console
{
    #region Properties

    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    #endregion


    public AdminConsole() : base(new AdminCommandController())
    {
        CommandController.OutputConsole = this;
    }
}