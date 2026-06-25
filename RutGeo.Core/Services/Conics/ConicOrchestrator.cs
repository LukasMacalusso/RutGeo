using RutGeo.Core.Interfaces.Common;
using RutGeo.Core.Interfaces.Conics;
using RutGeo.Core.Interfaces.Generators;
using RutGeo.Core.Models;
using RutGeo.Core.Models.Results;

namespace RutGeo.Core.Services.Conics;

public class ConicOrchestrator : IConicOrchestrator
{
    private readonly IRutEquationGenerator _equationGenerator;
    private readonly IToCanonicalTransformer _toCanonicalTransformer;
    private readonly IToGeneralTransformer _toGeneralTransformer;
    private readonly IExplanationLogger _log;

    public ConicOrchestrator(
        IRutEquationGenerator equationGenerator,
        IToCanonicalTransformer toCanonicalTransformer,
        IToGeneralTransformer toGeneralTransformer,
        IExplanationLogger log)
    {
        _equationGenerator = equationGenerator;
        _toCanonicalTransformer = toCanonicalTransformer;
        _toGeneralTransformer = toGeneralTransformer;
        _log = log;
    }

    public ConicOrchestrationResult Execute(RutValidatorResult validatorResult)
    {
        var result = new ConicOrchestrationResult();

        result.GeneralEquation = _equationGenerator.GenerateGeneralEquation(validatorResult);
        if (result.GeneralEquation == null)
            return result;

        int logBeforeFwd = _log.GetFullLog().Length;
        result.Conic = new Conic(result.GeneralEquation);
        result.CanonicalEquation = _toCanonicalTransformer.TransformToCanonical(result.GeneralEquation, result.Conic);
        string afterFwdLog = _log.GetFullLog();
        string canonicalSteps = afterFwdLog.Length > logBeforeFwd
            ? afterFwdLog.Substring(logBeforeFwd).TrimStart()
            : afterFwdLog.TrimStart();

        int logBeforeInv = _log.GetFullLog().Length;
        _toGeneralTransformer.TransformToGeneral(result.CanonicalEquation, result.Conic);
        string afterInvLog = _log.GetFullLog();
        string inverseSteps = afterInvLog.Length > logBeforeInv
            ? afterInvLog.Substring(logBeforeInv).TrimStart()
            : "";

        result.TransformationSteps = canonicalSteps + "\n" + inverseSteps;

        return result;
    }
}
