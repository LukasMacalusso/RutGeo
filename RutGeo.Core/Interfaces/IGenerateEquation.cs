using RutGeo.Core.Services;

namespace RutGeo.Core.Interfaces;

public interface IGenerateEquation
{
    Equation Generate(RutValidatorResult rut);
}