using Bitspoke.Core.Common.Grids;
using Bitspoke.Core.Common.Grids.GridItems;
using Bitspoke.Core.Components;
using Bitspoke.Ludus.Shared.Environment.Map.Definitions.Layers.Terrain;

namespace Bitspoke.Ludus.Shared.Environment.Terrain.Components;

public class TerrainComponent : Component
{
    #region Properties

    public override string ComponentName => nameof(TerrainComponent);

    public DefGrid<TerrainDef> Primary   { get; set; }
    public DefGrid<TerrainDef> Secondary { get; set; }
        
    #endregion

    #region Constructors and Initialisation

    public override void ConnectSignals() { }
    public override void Init(params object[] args) { }
    public override void AddComponents() { }
    public override void PostInit() { }
        
    #endregion

    #region Methods

    public TerrainDef Get(int index, bool includeSecondary = false)
    {
        try
        {
            if (Primary[index] == default && includeSecondary)
                return Secondary[index].Def;

            return Primary[index].Def;
        }
        catch (Exception e)
        {
            Log.Exception(e.Message, -9999999);
        }

        return default;
    }
        
    public void Set(int index, TerrainDef def)
    {
        // guard validation
        if (def == default)
            Log.Exception($"TerrainDef is default for Order {index}", -99999999);

        var gridItem = new DefGridItem<TerrainDef>(def);
            
        // TODO: Implement adding to Secondary Grid
        Log.TODO("Implement adding to Secondary grid");

        Primary.GridItems.Add(index, gridItem);
    }
        
    #endregion


        
}