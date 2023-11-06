namespace Bitspoke.Ludus.Shared.Common.Controllers.Admin;

public class AdminCommandControllerSignals
{
    public static string QUIT_REQUEST = "QuitRequest";
    
    public static string SET_PLANTS_VISIBLE => nameof(SetPlantsVisible);
    private delegate void SetPlantsVisible(bool visible, string? plantDefKey = null);
    
    public static string SET_TERRAIN_LAYER_VISIBLE => nameof(SetTerrainLayerVisible);
    private delegate void SetTerrainLayerVisible(bool visible, string? terrainDefKey = null);

}