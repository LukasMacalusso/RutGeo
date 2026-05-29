using RutGeo.Core.Services;

namespace RutGeo.Core.Interfaces;

public interface IEquationGenerator
{
    Equation Generate(RutValidatorResult rut);
}