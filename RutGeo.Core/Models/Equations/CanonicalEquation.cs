using RutGeo.Core.Models;
using RutGeo.Core.Models.Types;

namespace RutGeo.Core.Models.Equations;

public class CanonicalEquation
{
    public ConicType ConicType { get; init; }
    public string FormattedString { get; init; } = string.Empty;
    public ConicElements? Elements { get; init; }
}
