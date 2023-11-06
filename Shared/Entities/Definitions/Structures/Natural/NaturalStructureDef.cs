namespace Bitspoke.Ludus.Shared.Entities.Definitions.Structures.Natural;

public abstract class NaturalStructureDef : StructureDef
{
    #region Properties

    public string? AssociatedTerrainDefKey { get; set; }
    
    #endregion

    #region Constructors and Initialisation

    protected void Clone(NaturalStructureDef clone)
    {
        base.Clone(clone);

        clone.AssociatedTerrainDefKey = AssociatedTerrainDefKey;
    }
    
    #endregion

    #region Methods

    #endregion

    
}