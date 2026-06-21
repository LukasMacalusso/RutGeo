namespace RutGeo.Core.Models.Results;

public class RutValidatorResult
{
    public bool IsValid { get; init; }
    public string RutBody { get; init; } = string.Empty;
    public char Dv { get; init; }
}
