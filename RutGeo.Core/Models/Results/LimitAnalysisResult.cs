using RutGeo.Core.Models.Types;

namespace RutGeo.Core.Models.Results;

public class LimitAnalysisResult
{
    public int[] Digits { get; set; } = [];
    public int Condition { get; set; }
    public int CriticalPoint { get; set; }
    public string FunctionExpression { get; set; } = string.Empty;
    public string LeftLimit { get; set; } = string.Empty;
    public string RightLimit { get; set; } = string.Empty;
    public bool LimitExists { get; set; }
    public string LimitValue { get; set; } = string.Empty;
    public bool IsContinuous { get; set; }
    public string FunctionValueAtCriticalPoint { get; set; } = string.Empty;
    public DiscontinuityType DiscontinuityType { get; set; }
    public string Justification { get; set; } = string.Empty;
}
