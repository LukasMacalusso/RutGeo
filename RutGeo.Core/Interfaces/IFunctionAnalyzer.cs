using RutGeo.Core.Models;
using RutGeo.Core.Services;

namespace RutGeo.Core.Interfaces
{
    public interface IFunctionAnalyzer
    {
        LimitAnalysisResult AnalyzeFunctionFromRut(RutValidatorResult rut);
    }
}
