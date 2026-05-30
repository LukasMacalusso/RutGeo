using RutGeo.Core.Services;

namespace RutGeo.Core.Interfaces;

public interface IRutValidator
{
     RutValidatorResult Validate(string rawRut, IExplanationLog log);
}