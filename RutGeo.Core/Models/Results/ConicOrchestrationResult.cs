using RutGeo.Core.Models.Equations;

namespace RutGeo.Core.Models.Results;

public class ConicOrchestrationResult
{
    public GeneralEquation? GeneralEquation { get; set; }
    public Conic? Conic { get; set; }
    public CanonicalEquation? CanonicalEquation { get; set; }
    public string TransformationSteps { get; set; } = string.Empty;
}
