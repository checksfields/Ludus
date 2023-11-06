namespace Bitspoke.Ludus.Shared.Common.Reports;

// CORE: Move to core?
public abstract class Report
{
    #region Properties
    
    public List<string> Messages { get; set; }
    public ReportStatus Status { get; set; }
    
    #endregion

    #region Constructors and Initialisation

    public Report()
    {
        Init();
    }

    public void Init()
    {
        Messages = new List<string>();
    }
    
    #endregion

    #region Methods

    #endregion


    
}