using System;
using System.Collections.Generic;
using Bitspoke.GodotEngine.Components;
using Bitspoke.GodotEngine.Components.Nodes._2D;
using Bitspoke.Ludus.Client.Components.Nodes.Sprites.Plants.Natural;
using Bitspoke.Ludus.Shared.Common.Entities;
using Godot;

namespace Bitspoke.Ludus.Client.Components.Nodes.Sprites;

public static class EntitySpriteExtension
{
    #region Methods

    //public static T AddSprite<T>(this GodotNode2D container, LudusEntity ludusEntity, Texture2D texture2D) where T : EntitySprite2D, new()
    public static T AddSprite<T>(this GodotNode2D container, LudusEntity ludusEntity, Texture2D? texture2D = null) where T : EntitySprite2D, new()
    {
        var sprite2D = ludusEntity.BuildSprite<T>(container.GlobalPosition, texture2D);
        container.AddGodotNode(sprite2D);
        return sprite2D;
    }
    
    // public static T BuildSprite<T>(this LudusEntity ludusEntity, Texture2D texture2D) where T : EntitySprite2D, new()
    public static T BuildSprite<T>(this LudusEntity ludusEntity, Vector2 parentGlobalPosition, Texture2D? texture2D = null) where T : EntitySprite2D, new()
    {
        var sprite2D = new T { LudusEntity = ludusEntity };
        //sprite2D.BuildSprite(texture2D);
        sprite2D.BuildSprite(parentGlobalPosition, texture2D);
        return sprite2D;
    }
    
    #endregion

    
}