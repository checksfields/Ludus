using System;
using System.Collections.Generic;
using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Components.Nodes._2D;
using Bitspoke.GodotEngine.Components.Nodes._2D.Mouse;
using Bitspoke.GodotEngine.Components.Nodes.Sprites;
using Bitspoke.Ludus.Shared.Common.Entities;

namespace Bitspoke.Ludus.Client.Components.Nodes.Sprites.Plants;

public abstract partial  class PlantSprite2D : EntitySprite2D
{
    #region Properties
    // none
    #endregion

    #region Constructors and Initialisation
    
    protected PlantSprite2D(LudusEntity entity) : base(entity) { }

    protected PlantSprite2D() : base() { } 

    #endregion

    #region Overrides
    // none
    #endregion
    
    #region Methods
    // none
    #endregion


    
}