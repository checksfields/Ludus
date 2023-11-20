using System;
using System.Collections.Generic;
using Bitspoke.Core.Components.Location;
using Bitspoke.GodotEngine.Common.Vector;
using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Components.Nodes._2D;
using Bitspoke.GodotEngine.Components.Nodes._2D.Mouse;
using Bitspoke.GodotEngine.Components.Nodes.Sprites;
using Bitspoke.GodotEngine.Utils.Vector;
using Bitspoke.Ludus.Client.Components.Entities;
using Bitspoke.Ludus.Client.Components.Entities.Plants.Natural;
using Bitspoke.Ludus.Client.Components.Regions.Plants;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Components.Entities.Living;
using Bitspoke.Ludus.Shared.Entities.Definitions.Natural.Plants;
using Bitspoke.Ludus.Shared.Entities.Natural.Plants;
using Godot;

namespace Bitspoke.Ludus.Client.Components.Nodes.Sprites.Plants.Natural;

public partial  class NaturalPlantSprite2D : PlantSprite2D
{
    #region Properties

    public Plant? Plant => (LudusEntity is Plant plant) ? plant : null;
    
    #endregion

    #region Constructors and Initialisation

    public NaturalPlantSprite2D(LudusEntity entity) : base(entity) { }

    public NaturalPlantSprite2D() : base() { }

    #endregion

    #region Override

    public override void Init()
    {
        base.Init();

        this.Texture.ResourceLocalToScene = true;
    }

    public override EntityDetailsDisplayNode GetDetailsNode() => new NaturalPlantDetailsDisplayNode(Plant);
    
    #endregion
    
    
    #region Methods
    
    public override void BuildSprite(Vector2 parentGlobalPosition, Texture2D? texture2D = null)
    {
        var plantDef = (PlantDef) LudusEntity.Def;
        texture2D ??= Find.DB.TextureDB[plantDef.GraphicDef.TextureDef.TextureResourcePath];
        
        var locComp = LudusEntity.GetComponent<LocationComponent>();
        var location = (locComp.Location.ToVector2() * 64) + new Vector2(32, 0);
        var localLocation = location - parentGlobalPosition;
        
        Texture = texture2D;
        GlobalPosition = localLocation;
        ZIndex = (int) Position.Y;
            
        var growthComp = LudusEntity.GetComponent<GrowthComponent>();
        Scale = growthComp?.CurrentGrowthPercent.ToVector2() ?? Vector2.One;
    }

    

    #endregion
    
}