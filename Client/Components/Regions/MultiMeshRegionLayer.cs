using System.Collections.Generic;
using System.Linq;
using Bitspoke.Core.Components.Location;
using Bitspoke.Core.Definitions.Parts.Graphics;
using Bitspoke.Core.Definitions.Parts.Graphics.Textures.Types;
using Bitspoke.Core.Utils.Primatives.Float;
using Bitspoke.GodotEngine.Common.Vector;
using Bitspoke.GodotEngine.Utils.Vector;
using Bitspoke.Ludus.Client.Components.Nodes;
using Bitspoke.Ludus.Shared.Common.Entities;
using Bitspoke.Ludus.Shared.Components.Entities.Living;
using Godot;

namespace Bitspoke.Ludus.Client.Components.Regions;

public partial class MultiMeshRegionLayer : RegionLayer
{
    #region Properties

    public override string LayerName { get; protected set; }

    public Texture2D LayerTexture { get; set; }
    public MultiMeshInstance2D MultiMeshInstance2D { get; set; }

    public MeshInstance2D MeshInstance2D { get; set; } = new()
        { Mesh = new PlaneMesh { Size = CoreGlobal.STANDARD_CELL_SIZE.ToVector2(), Orientation = PlaneMesh.OrientationEnum.Z } };

    private Dictionary<ulong, int> AdditionalMeshes { get; set; } = new();

    public Vector2 CellOffset => CoreGlobal.STANDARD_CELL_SIZE.ToVector2() / 2;
    public List<LudusEntity> LayerEntities { get; set; }

    public GraphicDef GraphicDef { get; set; }
    
    public override string NodeName => GetType().Name;
    public override Node Node => this;
    
    #endregion

    #region Constructors and Initialisation

    public MultiMeshRegionLayer() : base()
    {

    }

    public MultiMeshRegionLayer(int layerID, GraphicDef graphicDef, List<LudusEntity> layerEntities) : this()
    {
        LayerID = layerID;
        LayerEntities = layerEntities;
        GraphicDef = graphicDef;
        LayerTexture = Find.DB.TextureDB[GraphicDef.TextureDef.TextureResourcePath];
    }

    public override void Init()
    {
        LayerName = $"{Parent.Name}_{Name}_{LayerID}";
        
        MultiMeshInstance2D = new MultiMeshInstance2D();
        AddChild(MultiMeshInstance2D);
    
        MultiMeshInstance2D.Multimesh = new MultiMesh();
        MultiMeshInstance2D.Texture = LayerTexture;
        //MultiMeshInstance2D.GlobalPosition = Parent.Region.Dimension.Position * CoreGlobal.STANDARD_CELL_SIZE;
        MultiMeshInstance2D.Multimesh.Mesh = MeshInstance2D.Mesh;
            
        var  colour = MultiMeshInstance2D.SelfModulate;
        colour.A = GraphicDef.TextureDef.Opacity;
            
        MultiMeshInstance2D.SelfModulate = colour;
        
        //Profile(message:$"{LayerName}", toProfile:AddMeshes);
        AddMeshes();
    }
        
    #endregion

    #region Methods

    public override void AddComponents() {}
    public override void ConnectSignals() {}
    
    public void Update(List<LudusEntity> layerEntities, bool generateMeshes = true)
    {
        LayerEntities = layerEntities;

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
        
        if (LayerEntities.Count < 1)
            return;
        
        // we only access one type of entity at a time (one def type)
        var graphicsDef = LayerEntities[0].Def.GetDefComponent<GraphicDef>();
        MultiMeshTextureTypeDetailsDef multiMeshTextureTypeDef = (MultiMeshTextureTypeDetailsDef) graphicsDef.TextureDef.TextureTypeDetails;
        var maxCountInCell = multiMeshTextureTypeDef.MaxCountInCell;
        var xLocationVariation = multiMeshTextureTypeDef.LocationVariationX;
        var yLocationVariation = multiMeshTextureTypeDef.LocationVariationY;

        
        // **** 20231115 Benchmark @ 0 ms
        foreach (var ludusEntity in LayerEntities)
        {
            var growth = 1.0f;
            if (ludusEntity.HasComponent<GrowthComponent>())
                growth = ludusEntity.GetComponent<GrowthComponent>().CurrentGrowthPercent;
            
            var numberInCell = (growth * maxCountInCell).Ceiling();
            AdditionalMeshes.Add(ludusEntity.ID, numberInCell);
        }
        
        MultiMeshInstance2D.Multimesh.InstanceCount = AdditionalMeshes.Sum(s => s.Value);

        // **** 20231115 Benchmark @ 2-8 ms
        // Profile(() => {
        foreach (var ludusEntity in LayerEntities)
        {
            // Rand.PushState();
            // Rand.Seed = ludusEntity.GetHashCode();

            var numberInCell = 0;
            var localLocation = Vector2.Zero;
            
            // **** 20231115 Benchmark @ 0 ms
            numberInCell = AdditionalMeshes[ludusEntity.ID];
            var locComp = ludusEntity.GetComponent<LocationComponent>();
            var location = (locComp.Location.ToVector2() * CoreGlobal.STANDARD_CELL_SIZE) + CellOffset;
            localLocation = location - MultiMeshInstance2D.GlobalPosition;
            
            // **** 20231115 Benchmark @ 0 ms
            for (int i = 0; i < numberInCell; i++)
            {
                var xOffset = xLocationVariation.RandRange();
                var yOffset = yLocationVariation.RandRange();
                var loc = localLocation + new Vector2(xOffset, yOffset);
                var transform2D = graphicsDef.GenerateTransform2D(loc);
                
                MultiMeshInstance2D.Multimesh.SetInstanceTransform2D(index++, transform2D);
            }
           
            // Rand.PopState();
        }
        // });
    }

    #endregion



}