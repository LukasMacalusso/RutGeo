using System;
using RutGeo.Core.Helpers;
using RutGeo.Core.Interfaces.Functions;
using RutGeo.Core.Models.Results;
using RutGeo.Core.Models.Types;

namespace RutGeo.Core.Services.Functions;

public class LimitOrchestrator : ILimitOrchestrator
{
    private readonly IFunctionAnalyzer _functionAnalyzer;

    public LimitOrchestrator(IFunctionAnalyzer functionAnalyzer)
    {
        _functionAnalyzer = functionAnalyzer;
    }

    public LimitOrchestrationResult Execute(RutValidatorResult validatorResult)
    {
        var analysisResult = _functionAnalyzer.AnalyzeFunctionFromRut(validatorResult);

        if (analysisResult == null)
            return new LimitOrchestrationResult();

        int condition = analysisResult.Condition;
        int a = analysisResult.CriticalPoint;
        int[] d = analysisResult.Digits;

        int d8 = d.Length >= 8 ? d[7] : 0;
        string selectionRule = condition switch
        {
            0 => $"d₈ = {d8} es múltiplo de 3 → Se genera discontinuidad removible",
            1 => $"d₈ = {d8} deja residuo 1 al dividirse por 3 → Se genera discontinuidad de salto",
            2 => $"d₈ = {d8} deja residuo 2 al dividirse por 3 → Se genera discontinuidad infinita",
            _ => ""
        };

        string caseDescription = analysisResult.DiscontinuityType switch
        {
            DiscontinuityType.Removable =>
                $"Caso 1: Discontinuidad Removible en x = {a}",
            DiscontinuityType.Jump =>
                $"Caso 2: Discontinuidad de Salto en x = {a}",
            DiscontinuityType.Infinite =>
                $"Caso 3: Discontinuidad Infinita en x = {a}",
            _ => $"Función continua en x = {a}"
        };

        double[] leftX = { a - 1, a - 0.1, a - 0.01, a - 0.001 };
        double[] rightX = { a + 0.001, a + 0.01, a + 0.1, a + 1 };

        string[] leftY = new string[leftX.Length];
        string[] rightY = new string[rightX.Length];

        for (int i = 0; i < leftX.Length; i++)
            leftY[i] = FormatEval(leftX[i], condition, d, a);

        for (int i = 0; i < rightX.Length; i++)
            rightY[i] = FormatEval(rightX[i], condition, d, a);

        return new LimitOrchestrationResult
        {
            AnalysisResult = analysisResult,
            CaseDescription = caseDescription,
            FunctionExpression = analysisResult.FunctionExpression,
            SelectionRule = selectionRule,
            LeftXValues = leftX,
            RightXValues = rightX,
            LeftYFormatted = leftY,
            RightYFormatted = rightY
        };
    }

    private static string FormatEval(double x, int condition, int[] d, int a)
    {
        try
        {
            double val = FunctionAnalyzer.EvaluateAt(x, condition, d, a);
            if (RutGeoMath.IsInfinity(val)) return val > 0 ? "+∞" : "-∞";
            if (RutGeoMath.IsNaN(val)) return "Indefinido";
            return val.ToString("F4");
        }
        catch (DivideByZeroException)
        {
            return "Indefinido";
        }
    }
}
