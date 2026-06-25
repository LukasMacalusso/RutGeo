using RutGeo.Core.Helpers;
using RutGeo.Core.Interfaces.Functions;
using RutGeo.Core.Models;
using RutGeo.Core.Models.Results;
using RutGeo.Core.Models.Types;
using System;

namespace RutGeo.Core.Services.Functions;

public class FunctionAnalyzer : IFunctionAnalyzer
{
    public LimitAnalysisResult AnalyzeFunctionFromRut(RutValidatorResult rut)
    {
        if (rut == null || string.IsNullOrWhiteSpace(rut.RutBody))
            throw new ArgumentException("RUT body is not valid.");

        int[] digits = RutDigitHelper.GetPaddedDigits(rut.RutBody);

        int d1 = digits[0];
        int d2 = digits[1];
        int d3 = digits[2];
        int d4 = digits[3];
        int d5 = digits[4];
        int d6 = digits[5];
        int d7 = digits[6];
        int d8 = digits[7];

        int a = d3;
        int condition = RutDigitHelper.GetLimitCondition(d8);

        var result = new LimitAnalysisResult
        {
            CriticalPoint = a,
            Condition = condition,
            Digits = digits
        };

        if (condition == 0)
        {
            result.FunctionExpression = $"f(x) = ((x - {a})(x + {d1})) / (x - {a})";
            result.LeftLimit = (a + d1).ToString();
            result.RightLimit = (a + d1).ToString();
            result.LimitExists = true;
            result.LimitValue = (a + d1).ToString();
            result.IsContinuous = false;
            result.FunctionValueAtCriticalPoint = "Indefinido";
            result.DiscontinuityType = DiscontinuityType.Removable;
            result.Justification = $"El límite existe y es igual a {a + d1}, pero la función no está definida en x = {a} debido a que el denominador se anula (0/0). Al simplificar el factor común, se observa que es una discontinuidad removible.";
        }
        else if (condition == 1)
        {
            result.FunctionExpression = $"f(x) = {{ x + {d2}, si x < {a} ; x + {d4}, si x >= {a} }}";
            result.LeftLimit = (a + d2).ToString();
            result.RightLimit = (a + d4).ToString();

            result.LimitExists = (a + d2) == (a + d4);
            if (result.LimitExists)
            {
                result.LimitValue = (a + d2).ToString();
                result.IsContinuous = true;
                result.FunctionValueAtCriticalPoint = (a + d4).ToString();
                result.DiscontinuityType = DiscontinuityType.None;
                result.Justification = $"Los límites laterales coinciden, y el valor de la función en el punto es igual al límite, por lo que la función es continua.";
            }
            else
            {
                result.LimitValue = "No existe";
                result.IsContinuous = false;
                result.FunctionValueAtCriticalPoint = (a + d4).ToString();
                result.DiscontinuityType = DiscontinuityType.Jump;
                result.Justification = $"El límite por la izquierda ({a + d2}) es distinto al límite por la derecha ({a + d4}). Por lo tanto, el límite no existe y hay una discontinuidad de salto en x = {a}.";
            }
        }
        else
        {
            result.FunctionExpression = $"f(x) = {d5 + 1} / (x - {a})";
            result.LeftLimit = "-∞";
            result.RightLimit = "+∞";
            result.LimitExists = false;
            result.LimitValue = "No existe";
            result.IsContinuous = false;
            result.FunctionValueAtCriticalPoint = "Indefinido";
            result.DiscontinuityType = DiscontinuityType.Infinite;
            result.Justification = $"La función crece o decrece sin límite al acercarse a x = {a} debido a que el denominador se anula y el numerador no es cero. Esto produce una asíntota vertical, lo que corresponde a una discontinuidad infinita.";
        }

        return result;
    }

    public static double EvaluateAt(double x, int condition, int[] digits, int criticalPoint)
    {
        return condition switch
        {
            0 => x + digits[0],
            1 => x < criticalPoint ? x + digits[1] : x + digits[3],
            2 => (digits[4] + 1.0) / (x - criticalPoint),
            _ => 0
        };
    }
}
