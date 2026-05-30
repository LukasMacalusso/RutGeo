using RutGeo.Core.Services;

namespace RutGeo.Core.Interfaces;

public interface IRutEquationGenerator
{
    GeneralEquation GenerateGeneralEquation(RutValidatorResult rut);
}