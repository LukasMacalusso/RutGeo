using RutGeo.Core.Models;
using RutGeo.Core.Models.Equations;
using RutGeo.Core.Models.Results;

namespace RutGeo.Core.Interfaces.Conics;

public interface IConicOrchestrator
{
    ConicOrchestrationResult Execute(RutValidatorResult validatorResult);
}
