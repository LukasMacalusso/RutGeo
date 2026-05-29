using RutGeo.Core.Services;

namespace RutGeo.Core.Interfaces;

public interface IConic
{
    RutValidatorResult Rut { get; }
    Equation Equation { get; }
    string Type { get; }
}
