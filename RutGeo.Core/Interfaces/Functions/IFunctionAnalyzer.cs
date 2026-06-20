using RutGeo.Core.Models;
using RutGeo.Core.Models.Results;

namespace RutGeo.Core.Interfaces.Functions
{
    public interface IFunctionAnalyzer
    {
        LimitAnalysisResult AnalyzeFunctionFromRut(RutValidatorResult rut);
    }
}
