using RutGeo.Core.Interfaces;
using RutGeo.Core.Models;
using System;

namespace RutGeo.Core.Services
{
    public class FunctionAnalyzer : IFunctionAnalyzer
    {
        public LimitAnalysisResult AnalyzeFunctionFromRut(RutValidatorResult rut)
        {
            ValidateRut(rut);

            int[] digits = GetPaddedDigits(rut.RutBody);

            int d1 = digits[0];
            int d2 = digits[1];
            int d3 = digits[2];
            int d4 = digits[3];
            int d5 = digits[4];
            int d6 = digits[5];
            int d7 = digits[6];
            int d8 = digits[7];

            int a = d3;
            int condition = d8 % 3;

            return condition switch
            {
                0 => AnalyzeRemovableDiscontinuity(a, d1),
                1 => AnalyzeJumpDiscontinuity(a, d2, d4),
                _ => AnalyzeInfiniteDiscontinuity(a, d5)
            };
        }

        private static LimitAnalysisResult AnalyzeRemovableDiscontinuity(int a, int d1)
        {
            return new LimitAnalysisResult
            {
                CriticalPoint = a,
                FunctionExpression = $"f(x) = ((x - {a})(x + {d1})) / (x - {a})",
                LeftLimit = (a + d1).ToString(),
                RightLimit = (a + d1).ToString(),
                LimitExists = true,
                LimitValue = (a + d1).ToString(),
                IsContinuous = false,
                FunctionValueAtCriticalPoint = "Indefinido",
                DiscontinuityType = DiscontinuityType.Removable,
                Justification = $"El límite existe y es igual a {a + d1}, pero la función no está definida en x = {a} debido a que el denominador se anula (0/0). Al simplificar el factor común, se observa que es una discontinuidad removible."
            };
        }

        private static LimitAnalysisResult AnalyzeJumpDiscontinuity(int a, int d2, int d4)
        {
            var result = new LimitAnalysisResult
            {
                CriticalPoint = a,
                FunctionExpression = $"f(x) = {{ x + {d2}, si x < {a} ; x + {d4}, si x >= {a} }}",
                LeftLimit = (a + d2).ToString(),
                RightLimit = (a + d4).ToString(),
                LimitExists = (a + d2) == (a + d4),
                FunctionValueAtCriticalPoint = (a + d4).ToString()
            };

            if (result.LimitExists)
            {
                result.LimitValue = (a + d2).ToString();
                result.IsContinuous = true;
                result.DiscontinuityType = DiscontinuityType.None;
                result.Justification = "Los límites laterales coinciden, y el valor de la función en el punto es igual al límite, por lo que la función es continua.";
            }
            else
            {
                result.LimitValue = "No existe";
                result.IsContinuous = false;
                result.DiscontinuityType = DiscontinuityType.Jump;
                result.Justification = $"El límite por la izquierda ({a + d2}) es distinto al límite por la derecha ({a + d4}). Por lo tanto, el límite no existe y hay una discontinuidad de salto en x = {a}.";
            }

            return result;
        }

        private static LimitAnalysisResult AnalyzeInfiniteDiscontinuity(int a, int d5)
        {
            return new LimitAnalysisResult
            {
                CriticalPoint = a,
                FunctionExpression = $"f(x) = {d5 + 1} / (x - {a})",
                LeftLimit = "-∞",
                RightLimit = "+∞",
                LimitExists = false,
                LimitValue = "No existe",
                IsContinuous = false,
                FunctionValueAtCriticalPoint = "Indefinido",
                DiscontinuityType = DiscontinuityType.Infinite,
                Justification = $"La función crece o decrece sin límite al acercarse a x = {a} debido a que el denominador se anula y el numerador no es cero. Esto produce una asíntota vertical, lo que corresponde a una discontinuidad infinita."
            };
        }

        private static int[] GetPaddedDigits(string body)
        {
            string paddedBody = body.PadLeft(8, '0');
            int[] digits = new int[paddedBody.Length];

            for (int i = 0; i < paddedBody.Length; i++)
            {
                digits[i] = paddedBody[i] - '0';
            }

            return digits;
        }

        private static void ValidateRut(RutValidatorResult rut)
        {
            if (rut == null || string.IsNullOrWhiteSpace(rut?.RutBody))
                throw new ArgumentException("RUT body is not valid.");
        }
    }
}
