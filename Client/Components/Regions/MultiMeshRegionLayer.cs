using System.Collections.Generic;
using System.Linq;
using Bitspoke.Core.Components.Life;
using Bitspoke.Core.Components.Location;
using Bitspoke.Core.Definitions.Parts.Graphics;
using Bitspoke.Core.Definitions.Parts.Graphics.Textures.Types;
using Bitspoke.Core.Random;
using Bitspoke.Core.Utils.Primatives.Float;
using Bitspoke.GodotEngine.Common.Vector;
using Bitspoke.GodotEngine.Utils.Vector;
using Bitspoke.Ludus.Shared.Common.Entities;
using Client.Components.Node;
using Godot;

namespace Client.Components.Regions;

public partial class MultiMeshRegionLayer : RegionLayer
{
    #region Properties

    public override string LayerName => nameof(MultiMeshRegionLayer);

    public Texture2D LayerTexture { get; set; }
    public MultiMeshInstance2D MultiMeshInstance2D { get; set; }

    public MeshInstance2D MeshInstance2D { get; set; } = new()
        { Mesh = new PlaneMesh { Size = CoreGlobal.STANDARD_CELL_SIZE.ToVector2() } };

    private Dictionary<int, int> AdditionalMeshes { get; set; } = new();

    public Vector2 CellOffset => CoreGlobal.STANDARD_CELL_SIZE.ToVector2() / 2;
    public List<LudusEntity> LayerEntities { get; set; }

    public GraphicDef GraphicDef { get; set; }
        
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
        LayerTexture = Find.DB.TextureDB[GraphicDef.TextureDef.TextureResourcePath];;
    }

    protected override void Init()
    {
        Name = $"{Parent.Name}_{LayerName}_{LayerID}";

        MultiMeshInstance2D = new MultiMeshInstance2D();
        AddChild(MultiMeshInstance2D);

        MultiMeshInstance2D.Multimesh = new MultiMesh();
        MultiMeshInstance2D.Texture = LayerTexture;
        MultiMeshInstance2D.GlobalPosition = Parent.Region.Dimension.Position * CoreGlobal.STANDARD_CELL_SIZE;
        MultiMeshInstance2D.Multimesh.Mesh = MeshInstance2D.Mesh;
            
        var  colour = MultiMeshInstance2D.SelfModulate;
        colour.A = GraphicDef.TextureDef.Opacity;
            
        MultiMeshInstance2D.SelfModulate = colour;
            
        AddMeshes();
    }
        
    #endregion

    #region Methods

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

        foreach (var ludusEntity in LayerEntities)
        {
            var graphicsDef = ludusEntity.Def.GetDefComponent<GraphicDef>();
            var maxCountInCell = ((MultiMeshTextureTypeDetailsDef)graphicsDef.TextureDef.TextureTypeDetails)
                .MaxCountInCell;

            var growth = 1.0f;
            if (ludusEntity.HasComponent<GrowthComponent>())
            {
                growth = ludusEntity.GetComponent<GrowthComponent>().Growth;
            }

            var numberInCell = (growth + maxCountInCell).Ceiling();
            AdditionalMeshes.Add(ludusEntity.IDComponent.ID, numberInCell);
        }

        MultiMeshInstance2D.Multimesh.InstanceCount = AdditionalMeshes.Sum(s => s.Value);

        foreach (var ludusEntity in LayerEntities)
        {
            Rand.PushState();
            Rand.Seed = ludusEntity.GetHashCode();

            var graphicsDef = ludusEntity.Def.GetDefComponent<GraphicDef>();
            var xLocationVariation = ((MultiMeshTextureTypeDetailsDef)graphicsDef.TextureDef.TextureTypeDetails)
                .LocationVariationX;
            var yLocationVariation = ((MultiMeshTextureTypeDetailsDef)graphicsDef.TextureDef.TextureTypeDetails)
                .LocationVariationY;

            var numberInCell = AdditionalMeshes[ludusEntity.IDComponent.ID];

            var locComp = ludusEntity.GetComponent<LocationComponent>();
            var location = (locComp.Location.ToVector2() * CoreGlobal.STANDARD_CELL_SIZE) + CellOffset;
            var localLocation = location - MultiMeshInstance2D.GlobalPosition;

            for (int i = 0; i < numberInCell; i++)
            {
                var xOffset = xLocationVariation.RandRange();
                var yOffset = yLocationVariation.RandRange();
                var loc = localLocation + new Vector2(xOffset, yOffset);
                var transform = graphicsDef.GenerateTransform2D(loc);

                MultiMeshInstance2D.Multimesh.SetInstanceTransform2D(index++, transform);
            }

            Rand.PopState();
        }
    }

    #endregion



}