using RutGeo.Core.Helpers;
using RutGeo.Core.Interfaces.Common;
using RutGeo.Core.Interfaces.Generators;
using RutGeo.Core.Models.Equations;
using RutGeo.Core.Models.Results;

namespace RutGeo.Core.Services.Generators;

public class RutEquationGenerator : IRutEquationGenerator
{
    private readonly IExplanationLogger _log;

    public RutEquationGenerator(IExplanationLogger log)
    {
        _log = log;
    }

    public GeneralEquation GenerateGeneralEquation(RutValidatorResult rut)
    {
        _log.StartProcess("Construcción de ecuación general a partir del RUT");

        int validationModulo = GetValidationModulo(rut.Dv);
        int[] d = RutDigitHelper.GetPaddedDigits(rut.RutBody);

        _log.AppendStep($"Dígitos del RUT: d₁={d[0]}, d₂={d[1]}, d₃={d[2]}, d₄={d[3]}, d₅={d[4]}, d₆={d[5]}, d₇={d[6]}, d₈={d[7]}");
        _log.AppendStep($"DV = {rut.Dv} → v = {validationModulo}");

        _log.AppendStep($"A = (d₁ + d₂) / v = ({d[0]} + {d[1]}) / {validationModulo} = {(d[0] + d[1]) / (double)validationModulo:F4}");
        _log.AppendStep($"B = (d₃ + d₄) / v = ({d[2]} + {d[3]}) / {validationModulo} = {(d[2] + d[3]) / (double)validationModulo:F4}");
        _log.AppendStep($"C = -(d₅ + d₆) = -({d[4]} + {d[5]}) = {-(d[4] + d[5])}");
        _log.AppendStep($"D = -(d₇ + d₈) = -({d[6]} + {d[7]}) = {-(d[6] + d[7])}");
        _log.AppendStep($"E = d₁ + d₃ + d₅ + d₇ = {d[0]} + {d[2]} + {d[4]} + {d[6]} = {d[0] + d[2] + d[4] + d[6]}");

        double a = (double)(d[0] + d[1]) / validationModulo;
        double b = (double)(d[2] + d[3]) / validationModulo;
        double c = -(d[4] + d[5]);
        double dd = -(d[6] + d[7]);
        double e = d[0] + d[2] + d[4] + d[6];

        _log.AppendStep("");
        _log.AppendStep("--- Aplicando reglas de ajuste ---");

        string bSignMsg = d[7] % 2 != 0 ? "es impar → B se reemplaza por -B" : "es par → B no cambia";
        _log.AppendStep($"d₈ = {d[7]} → {bSignMsg}");
        b = d[7] % 2 != 0 ? -b : b;

        string eqMsg = d[0] == d[1] ? $"son iguales → se impone B = A = {a:F4}" : "son distintos → B se mantiene";
        _log.AppendStep($"d₁ = {d[0]}, d₂ = {d[1]} → {eqMsg}");
        b = d[0] == d[1] ? a : b;

        int sum56 = d[4] + d[5];
        bool mult3 = sum56 % 3 == 0;
        _log.AppendStep($"d₅ + d₆ = {d[4]} + {d[5]} = {sum56} → {(mult3 ? "es múltiplo de 3" : "no es múltiplo de 3, no se aplica ajuste")}");
        if (mult3)
        {
            string adjMsg = d[6] % 2 == 0 ? "d₇ es par → B = 0 (parábola eje vertical)" : "d₇ es impar → A = 0 (parábola eje horizontal)";
            _log.AppendStep($"  {adjMsg}");
            if (d[6] % 2 != 0) a = 0;
            else b = 0;
        }

        _log.AppendStep("");
        _log.AppendStep("--- Coeficientes finales de la ecuación general ---");
        _log.AppendStep($"A = {a:F4}");
        _log.AppendStep($"B = {b:F4}");
        _log.AppendStep($"C = {c}");
        _log.AppendStep($"D = {dd}");
        _log.AppendStep($"E = {e}");

        return new GeneralEquation
        {
            A = a,
            B = b,
            C = c,
            D = dd,
            E = e
        };
    }

    private static int GetValidationModulo(char dv)
    {
        return dv switch
        {
            '0' => 11,
            'K' => 10,
            _ => (int)char.GetNumericValue(dv)
        };
    }

}
