using System.Collections.Generic;
using Bitspoke.Core.Definitions.Parts.Graphics;
using Bitspoke.GodotEngine.Common.Vector;
using Bitspoke.GodotEngine.Utils.Vector;
using Bitspoke.Ludus.Shared.Environment.Map.MapCells;
using Godot;

namespace Bitspoke.Ludus.Client.Components.Regions.Debug;

public partial class DebugMultiMeshRegionLayer : RegionLayer
{
    #region Properties

    public override string LayerName { get; protected set; }
    
    public Texture2D LayerTexture { get; set; }
    public MultiMeshInstance2D MultiMeshInstance2D { get; set; }
    public MeshInstance2D MeshInstance2D { get; set; } = new() { Mesh = new PlaneMesh { Size = CoreGlobal.STANDARD_CELL_SIZE.ToVector2() } };

    private Dictionary<int, int> AdditionalMeshes { get; set; } = new();

    public Vector2 CellOffset => CoreGlobal.STANDARD_CELL_SIZE.ToVector2() / 2;
    public List<MapCell> LayerMapCells { get; set; }
    
    public override string Name => GetType().Name;
    
    #endregion

    #region Constructors and Initialisation

    public DebugMultiMeshRegionLayer(int layerID, Texture2D layerTexture, List<MapCell> mapCells)
    {
        LayerID = layerID;
        LayerTexture = layerTexture;
        LayerMapCells = mapCells;
    }
    
    public override void Init()
    {
        LayerName = $"{Parent.Name}_{Name}_{LayerID}";
        
        MultiMeshInstance2D = new MultiMeshInstance2D();
        AddChild(MultiMeshInstance2D);
        
        MultiMeshInstance2D.Multimesh = new MultiMesh();
        MultiMeshInstance2D.Texture = LayerTexture;
        MultiMeshInstance2D.GlobalPosition = Parent.Region.Dimension.Position * CoreGlobal.STANDARD_CELL_SIZE;
        MultiMeshInstance2D.Multimesh.Mesh = MeshInstance2D.Mesh;

        AddMeshes();
    }
    
    #endregion

    #region Methods

    public override void AddComponents() {}
    public override void ConnectSignals() {}
    
    public void Update(List<MapCell> layerMapCells, bool generateMeshes = true)
    {
        LayerMapCells = layerMapCells;
        
        if (generateMeshes)
            Refresh();
    }

    public override void Refresh()
    {
        AddMeshes();
    }
    
    private void AddMeshes()
    {
        AdditionalMeshes.Clear();
        var index = 0;
        
        MultiMeshInstance2D.Multimesh.InstanceCount = LayerMapCells.Count;
        
        foreach (var mapCell in LayerMapCells)
        {
            
            var graphicsDef = mapCell.TerrainDef.GetDefComponent<GraphicDef>();
            
            
            var location = (mapCell.Location.ToVector2() * CoreGlobal.STANDARD_CELL_SIZE) + CellOffset;
            var localLocation = location - MultiMeshInstance2D.GlobalPosition;
            
            //var transform = graphicsDef.GenerateTransform2D(localLocation);
            var transform = new Transform2D(3.14159f, localLocation);
            
            MultiMeshInstance2D.Multimesh.SetInstanceTransform2D(index++, transform);
        }
    }
    
    #endregion


}