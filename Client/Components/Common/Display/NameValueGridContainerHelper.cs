using System;
using System.Collections.Generic;
using Bitspoke.Core.Components.Identity;
using Bitspoke.Core.Components.Life;
using Bitspoke.Core.Components.Location;
using Bitspoke.GodotEngine.Components.Nodes.CanvasItems.Controls.Containers;

namespace Bitspoke.Ludus.Client.Components.Common.Display;

public static class NameValueGridContainerHelper
{
    #region Methods

    public static void AddIDComponent(this NameValueGridContainer container, IDComponent component, int order = -1, bool showDetailed = true)
    {
        //container.AddHeader("ID", false);
        container.AddNameValuePair($"ID:", component.ID.ToString, order != -1 ? order++ : order, !showDetailed);
    }
    
    public static void AddLocationComponent(this NameValueGridContainer container, LocationComponent component, int order = -1, bool showDetailed = true)
    {
        //container.AddHeader("Growth", false);
        container.AddNameValuePair($"Location:", component.Location.ToString, order != -1 ? order++ : order, !showDetailed);
    }
    
    public static void AddGrowthComponent(this NameValueGridContainer container, GrowthComponent component, int order = -1, bool showDetailed = true)
    {
        //container.AddHeader("Growth", false);
        container.AddNameValuePair($"Current Growth:", component.CurrentGrowthPercent.ToString, order != -1 ? order++ : order, !showDetailed);
        
        if (!showDetailed)
            return;
        
        container.AddNameValuePair($"Max Growth:", component.MaxGrowthPercent.ToString,  order != -1 ? order++ : order, false);
        container.AddNameValuePair($"Min Growth:", component.MaxGrowthPercent.ToString,  order != -1 ? order++ : order, false);
        container.AddNameValuePair($"Is Fully Grown:", component.IsFullyGrown.ToString,  order != -1 ? order++ : order);
    }
    
    #endregion

    
}