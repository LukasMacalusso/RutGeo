using RutGeo.Core.Models.Results;

namespace RutGeo.Core.Interfaces.Validation;

public interface IRutValidator
{
     RutValidatorResult Validate(string rawRut);
}