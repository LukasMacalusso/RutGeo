using RutGeo.Core.Services;

namespace RutGeo.Core.Interfaces;

public interface IGenerateEquiation
{
    Equation Generate(RutValidatorResult rut);
}