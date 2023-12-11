using System;
using Rangef = Bitspoke.Core.Common.Values.Range;

namespace Bitspoke.Ludus.Shared.Entities.Natural.Plants;

public class PlantGlobal
{
    #region Properties

    public static float DefaultMinGrowDays { get; set; } = 1f;
    
    public static Rangef InitialGrowthRange { get; set; } = new( 0.05f, 1.0f );
    public static Rangef InitialAgeRange { get; set; } = new( 0.25f, 0.85f );

    #endregion

    #region Constructors and Initialisation

    #endregion

    #region Methods

    #endregion


}