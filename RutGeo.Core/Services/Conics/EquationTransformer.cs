using System;
using System.Collections.Generic;
using RutGeo.Core.Interfaces.Conics;
using RutGeo.Core.Interfaces.Common;
using RutGeo.Core.Models;
using RutGeo.Core.Models.Equations;
using RutGeo.Core.Models.Types;

namespace RutGeo.Core.Services.Conics;

public class EquationTransformer : IEquationTransformer
{
    private readonly IConicElementsFactory _elementsFactory;
    private readonly IExplanationLogger _log;

    public EquationTransformer(IConicElementsFactory elementsFactory, IExplanationLogger log)
    {
        _elementsFactory = elementsFactory;
        _log = log;
    }


    public CanonicalEquation TransformToCanonical(GeneralEquation generalEquation, Conic conic)
    {
        _log.StartProcess($"Transformación a Forma Canónica: {conic.Type}");
        string canonicalEquationString = string.Empty;
        ConicElements? elements = null;

        switch (conic.Type)
        {
            case ConicType.Circunferencia:
                (canonicalEquationString, elements) = TransformCircle(generalEquation);
                break;
            case ConicType.Elipse:
                (canonicalEquationString, elements) = TransformEllipse(generalEquation);
                break;
            case ConicType.Hyperbola:
                (canonicalEquationString, elements) = TransformHyperbola(generalEquation);
                break;
            case ConicType.Parabola:
                (canonicalEquationString, elements) = TransformParabola(generalEquation);
                break;
            case ConicType.Desconocida:
            default:
                _log.AppendStep("Error: Ecuación degenerada o tipo de cónica desconocida.");
                break;
        }

        return new CanonicalEquation
        {
            ConicType = conic.Type,
            FormattedString = canonicalEquationString,
            Elements = elements
        };
    }


    public GeneralEquation TransformToGeneral(CanonicalEquation canonicalEquation, Conic conic)
    {
        throw new NotImplementedException();
    }



    private (string, ConicElements?) TransformCircle(GeneralEquation equation)
    {
        double h = -equation.C / (2 * equation.A);
        double k = -equation.D / (2 * equation.A);
        double r2 = h * h + k * k - equation.E / equation.A;

        LogTransformCircleSteps(equation, h, k, r2);

        var elements = _elementsFactory.CreateCircleElements(h, k, r2);

        return ($"(x - {FormatNumber(h)})² + (y - {FormatNumber(k)})² = {FormatNumber(r2)}", elements);
    }
    private void LogTransformCircleSteps(GeneralEquation eq, double h, double k, double r2)
    {
        _log.AppendStep("Agrupar términos en x e y, despejar la constante.");
        _log.AppendEquation($"(x² + {FormatNumber(eq.C / eq.A)}x) + (y² + {FormatNumber(eq.D / eq.A)}y) = {FormatNumber(-eq.E / eq.A)}");

        _log.AppendStep("Completar cuadrados para x e y.");
        _log.AppendEquation($"(x + {FormatNumber(eq.C / (2 * eq.A))})² + (y + {FormatNumber(eq.D / (2 * eq.A))})² = {FormatNumber(r2)}");

        _log.AppendStep("Identificar centro (h, k) y radio al cuadrado (r²).");
        _log.AppendStep($"Centro: ({FormatNumber(h)}, {FormatNumber(k)})");
        _log.AppendStep($"Radio: {FormatNumber(Math.Sqrt(r2))}");
    }


    private (string, ConicElements?) TransformEllipse(GeneralEquation equation)
    {
        double h = -equation.C / (2 * equation.A);
        double k = -equation.D / (2 * equation.B);
        double rhs = -equation.E + equation.A * h * h + equation.B * k * k;
        double a2 = rhs / equation.A;
        double b2 = rhs / equation.B;

        LogTransformEllipseSteps(equation, h, k, rhs);

        var elements = _elementsFactory.CreateEllipseElements(h, k, a2, b2);

        return ($"(x - {FormatNumber(h)})² / {FormatNumber(a2)} + (y - {FormatNumber(k)})² / {FormatNumber(b2)} = 1", elements);
    }
    private void LogTransformEllipseSteps(GeneralEquation equation, double h, double k, double rhs)
    {
        _log.AppendStep("Factorizar coeficientes principales.");
        _log.AppendEquation($"{FormatNumber(equation.A)}(x² + {FormatNumber(equation.C / equation.A)}x) + {FormatNumber(equation.B)}(y² + {FormatNumber(equation.D / equation.B)}y) = {FormatNumber(-equation.E)}");

        _log.AppendStep("Completar cuadrados.");
        _log.AppendEquation($"{FormatNumber(equation.A)}(x - {FormatNumber(h)})² + {FormatNumber(equation.B)}(y - {FormatNumber(k)})² = {FormatNumber(rhs)}");

        _log.AppendStep("Dividir todo por el resultado para igualar a 1.");

    }


    private (string, ConicElements?) TransformHyperbola(GeneralEquation eq)
    {
        double h = -eq.C / (2 * eq.A);
        double k = -eq.D / (2 * eq.B);
        double rhs = -eq.E + eq.A * h * h + eq.B * k * k;

        if (Math.Abs(rhs) < 1e-12)
        {
            _log.AppendStep("El resultado es una hipérbola degenerada (asíntotas).");
            return ("0 = 0 (Degenerada)", null);
        }

        LogTransformHyperbolaSteps(eq, h, k, rhs);

        bool isHorizontal = rhs > 0;
        double a2 = isHorizontal ? (rhs / eq.A) : (rhs / eq.B);
        double b2 = isHorizontal ? (-rhs / eq.B) : (-rhs / eq.A);

        var elements = _elementsFactory.CreateHyperbolaElements(h, k, a2, b2, isHorizontal);

        if (isHorizontal)
        {
            return ($"(x - {FormatNumber(h)})² / {FormatNumber(a2)} - (y - {FormatNumber(k)})² / {FormatNumber(b2)} = 1", elements);
        }

        return ($"(y - {FormatNumber(k)})² / {FormatNumber(a2)} - (x - {FormatNumber(h)})² / {FormatNumber(b2)} = 1", elements);
    }

    private void LogTransformHyperbolaSteps(GeneralEquation eq, double h, double k, double rhs)
    {
        _log.AppendStep("Factorizar y agrupar.");
        _log.AppendEquation($"{FormatNumber(eq.A)}(x² + {FormatNumber(eq.C / eq.A)}x) + {FormatNumber(eq.B)}(y² + {FormatNumber(eq.D / eq.B)}y) = {FormatNumber(-eq.E)}");

        _log.AppendStep("Completar cuadrados.");
        _log.AppendEquation($"{FormatNumber(eq.A)}(x - {FormatNumber(h)})² + {FormatNumber(eq.B)}(y - {FormatNumber(k)})² = {FormatNumber(rhs)}");

        _log.AppendStep("Igualar a 1 dividiendo por el término independiente.");
    }


    private (string, ConicElements?) TransformParabola(GeneralEquation eq)
    {
        if (eq.B == 0)
        {
            double h = -eq.C / (2 * eq.A);
            double k = (-eq.E + eq.A * h * h) / -eq.D;
            double p = -eq.D / (4 * eq.A);

            LogTransformParabolaSteps(eq, h, k, true);

            var elements = _elementsFactory.CreateParabolaElements(h, k, p, true);

            return ($"(x - {FormatNumber(h)})² = {FormatNumber(4 * p)}(y - {FormatNumber(k)})", elements);
        }
        else
        {
            double kAlt = -eq.D / (2 * eq.B);
            double hAlt = (-eq.E + eq.B * kAlt * kAlt) / -eq.C;
            double pAlt = -eq.C / (4 * eq.B);

            LogTransformParabolaSteps(eq, hAlt, kAlt, false);

            var elements = _elementsFactory.CreateParabolaElements(hAlt, kAlt, pAlt, false);

            return ($"(y - {FormatNumber(kAlt)})² = {FormatNumber(4 * pAlt)}(x - {FormatNumber(hAlt)})", elements);
        }
    }

    private void LogTransformParabolaSteps(GeneralEquation eq, double h, double k, bool isXSquared)
    {
        if (isXSquared)
        {
            _log.AppendStep("Aislar la variable al cuadrado (x).");
            _log.AppendEquation($"{FormatNumber(eq.A)}x² + {FormatNumber(eq.C)}x = -{FormatNumber(eq.D)}y - {FormatNumber(eq.E)}");

            _log.AppendStep("Completar el cuadrado para x.");
            _log.AppendEquation($"{FormatNumber(eq.A)}(x - {FormatNumber(h)})² = -{FormatNumber(eq.D)}(y - {FormatNumber(k)})");
        }
        else
        {
            _log.AppendStep("Aislar la variable al cuadrado (y).");
            _log.AppendEquation($"{FormatNumber(eq.B)}y² + {FormatNumber(eq.D)}y = -{FormatNumber(eq.C)}x - {FormatNumber(eq.E)}");

            _log.AppendStep("Completar el cuadrado para y.");
            _log.AppendEquation($"{FormatNumber(eq.B)}(y - {FormatNumber(k)})² = -{FormatNumber(eq.C)}(x - {FormatNumber(h)})");
        }
    }


    private static string FormatNumber(double value)
    {
        return value.ToString("0.##");
    }
}
