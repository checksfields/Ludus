using Bitspoke.Ludus.Client.Components.Nodes.Sprites.Plants;
using Bitspoke.Ludus.Client.Components.Nodes.Sprites.Plants.Natural;

namespace Bitspoke.Ludus.Client.Components.Entities.Plants.Natural;

public partial  class NaturalPlantNode : PlantNode
{
    #region Properties

    public NaturalPlantSprite2D Sprite2D { get; set; }
    
    #endregion

    #region Constructors and Initialisation
    // none
    #endregion

    #region Overrides
    // none
    #endregion
    
    #region Methods
    // none
    public override PlantSprite2D GetSprite2D() => Sprite2D;
    #endregion

    
}