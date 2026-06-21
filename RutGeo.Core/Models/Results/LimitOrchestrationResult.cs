using RutGeo.Core.Models.Types;

namespace RutGeo.Core.Models.Results;

public class LimitOrchestrationResult
{
    public LimitAnalysisResult? AnalysisResult { get; set; }
    public string CaseDescription { get; set; } = string.Empty;
    public string FunctionExpression { get; set; } = string.Empty;
    public string SelectionRule { get; set; } = string.Empty;
    public double[] LeftXValues { get; set; } = [];
    public double[] RightXValues { get; set; } = [];
    public string[] LeftYFormatted { get; set; } = [];
    public string[] RightYFormatted { get; set; } = [];
}
