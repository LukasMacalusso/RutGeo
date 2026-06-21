using RutGeo.Core.Models.Equations;
using RutGeo.Core.Models.Results;

namespace RutGeo.Core.Interfaces.Generators;

public interface IRutEquationGenerator
{
    GeneralEquation GenerateGeneralEquation(RutValidatorResult rut);
}