using RutGeo.Core.Models.Results;

namespace RutGeo.Core.Interfaces.Functions;

public interface ILimitOrchestrator
{
    LimitOrchestrationResult Execute(RutValidatorResult validatorResult);
}
