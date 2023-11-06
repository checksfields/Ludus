using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bitspoke.Ludus.Shared.Common.Reports;

// CORE: Move to core?
[Flags]
[JsonConverter(typeof(StringEnumConverter))]
public enum ReportStatus
{
    None,
    Rejected,
    RejectedWithMessages,
    Accepted,
    AcceptedWithMessages
}