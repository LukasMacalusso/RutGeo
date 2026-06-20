using RutGeo.Core.Services;

namespace RutGeo.Core.Interfaces;

public interface IEquationGenerator
{
    GeneralEquation Generate(RutValidatorResult rut);
}
