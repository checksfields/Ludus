using Bitspoke.Core.Components;

namespace Bitspoke.Ludus.Shared.Common.Components;

public abstract class LudusComponent : Component
{
    #region Properties

    #endregion

    #region Constructors and Initialisation

    public override void ConnectSignals() { }
    public override void Init(params object[] args) { }
    public override void AddComponents() {  }
    public override void PostInit() { }
    
    #endregion

    #region Methods

    #endregion
}