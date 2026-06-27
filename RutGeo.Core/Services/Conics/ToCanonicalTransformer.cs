using RutGeo.Core.Helpers;
using RutGeo.Core.Interfaces.Conics;
using RutGeo.Core.Interfaces.Common;
using RutGeo.Core.Models;
using RutGeo.Core.Models.Equations;
using RutGeo.Core.Models.Types;

namespace RutGeo.Core.Services.Conics;

public class ToCanonicalTransformer : IToCanonicalTransformer
{
    private readonly IConicElementsFactory _elementsFactory;
    private readonly IExplanationLogger _log;

    public ToCanonicalTransformer(IConicElementsFactory elementsFactory, IExplanationLogger log)
    {
        _elementsFactory = elementsFactory;
        _log = log;
    }

    public CanonicalEquation TransformToCanonical(GeneralEquation generalEquation, Conic conic)
    {
        _log.StartProcess($"Transformación a Forma Canónica: {conic.Type}");

        (string canonicalEquationString, ConicElements? elements) = conic.Type switch
        {
            ConicType.Circunferencia => TransformCircle(generalEquation),
            ConicType.Elipse => TransformEllipse(generalEquation),
            ConicType.Hyperbola => TransformHyperbola(generalEquation),
            ConicType.Parabola => TransformParabola(generalEquation),
            _ => LogAndReturnEmpty()
        };

        return new CanonicalEquation
        {
            ConicType = conic.Type,
            FormattedString = canonicalEquationString,
            Elements = elements
        };
    }

    private (string, ConicElements?) LogAndReturnEmpty()
    {
        _log.AppendStep("Error: Ecuación degenerada o tipo de cónica desconocida.");
        return (string.Empty, null);
    }

    private (string, ConicElements?) TransformCircle(GeneralEquation equation)
    {
        double h = -equation.C / (2 * equation.A);
        double k = -equation.D / (2 * equation.A);
        double r2 = h * h + k * k - equation.E / equation.A;

        LogTransformCircleSteps(equation, h, k, r2);

        if (r2 < 0)
        {
            _log.AppendStep("La circunferencia no tiene solución real (r² < 0).");
            return ($"{TransformationHelper.FormatBracket('x', h)}² + {TransformationHelper.FormatBracket('y', k)}² = {TransformationHelper.FormatNumber(r2)} (Sin solución real)", null);
        }

        var elements = _elementsFactory.CreateCircleElements(h, k, r2);

        return ($"{TransformationHelper.FormatBracket('x', h)}² + {TransformationHelper.FormatBracket('y', k)}² = {TransformationHelper.FormatNumber(r2)}", elements);
    }

    private void LogTransformCircleSteps(GeneralEquation eq, double h, double k, double r2)
    {
        _log.AppendStep("Agrupar términos en x e y, despejar la constante.");
        _log.AppendEquation($"(x² + {TransformationHelper.FormatNumber(eq.C / eq.A)}x) + (y² + {TransformationHelper.FormatNumber(eq.D / eq.A)}y) = {TransformationHelper.FormatNumber(-eq.E / eq.A)}");

        _log.AppendStep("Completar cuadrados para x e y.");
        _log.AppendEquation($"(x + {TransformationHelper.FormatNumber(eq.C / (2 * eq.A))})² + (y + {TransformationHelper.FormatNumber(eq.D / (2 * eq.A))})² = {TransformationHelper.FormatNumber(r2)}");
    }


    private (string, ConicElements?) TransformEllipse(GeneralEquation equation)
    {
        double h = -equation.C / (2 * equation.A);
        double k = -equation.D / (2 * equation.B);
        double rhs = -equation.E + equation.A * h * h + equation.B * k * k;

        LogTransformEllipseSteps(equation, h, k, rhs);

        if (rhs <= 0)
        {
            _log.AppendStep("La elipse no tiene solución real (término independiente ≤ 0).");
            return ($"{TransformationHelper.FormatBracket('x', h)}² / {TransformationHelper.FormatNumber(rhs / equation.A)} + {TransformationHelper.FormatBracket('y', k)}² / {TransformationHelper.FormatNumber(rhs / equation.B)} = 1 (Sin solución real)", null);
        }

        double a2 = rhs / equation.A;
        double b2 = rhs / equation.B;
        var elements = _elementsFactory.CreateEllipseElements(h, k, a2, b2);

        return ($"{TransformationHelper.FormatBracket('x', h)}² / {TransformationHelper.FormatNumber(a2)} + {TransformationHelper.FormatBracket('y', k)}² / {TransformationHelper.FormatNumber(b2)} = 1", elements);
    }

    private void LogTransformEllipseSteps(GeneralEquation equation, double h, double k, double rhs)
    {
        _log.AppendStep("Factorizar coeficientes principales.");
        _log.AppendEquation($"{TransformationHelper.FormatNumber(equation.A)}(x² + {TransformationHelper.FormatNumber(equation.C / equation.A)}x) + {TransformationHelper.FormatNumber(equation.B)}(y² + {TransformationHelper.FormatNumber(equation.D / equation.B)}y) = {TransformationHelper.FormatNumber(-equation.E)}");

        _log.AppendStep("Completar cuadrados.");
        _log.AppendEquation($"{TransformationHelper.FormatNumber(equation.A)}(x - {TransformationHelper.FormatNumber(h)})² + {TransformationHelper.FormatNumber(equation.B)}(y - {TransformationHelper.FormatNumber(k)})² = {TransformationHelper.FormatNumber(rhs)}");

        _log.AppendStep("Dividir todo por el resultado para igualar a 1.");
    }


    private (string, ConicElements?) TransformHyperbola(GeneralEquation eq)
    {
        double h = -eq.C / (2 * eq.A);
        double k = -eq.D / (2 * eq.B);
        double rhs = -eq.E + eq.A * h * h + eq.B * k * k;

        if (RutGeoMath.Abs(rhs) < RutGeoMath.NearZeroThreshold)
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
            return ($"{TransformationHelper.FormatBracket('x', h)}² / {TransformationHelper.FormatNumber(a2)} - {TransformationHelper.FormatBracket('y', k)}² / {TransformationHelper.FormatNumber(b2)} = 1", elements);

        return ($"{TransformationHelper.FormatBracket('y', k)}² / {TransformationHelper.FormatNumber(a2)} - {TransformationHelper.FormatBracket('x', h)}² / {TransformationHelper.FormatNumber(b2)} = 1", elements);
    }

    private void LogTransformHyperbolaSteps(GeneralEquation eq, double h, double k, double rhs)
    {
        _log.AppendStep("Factorizar y agrupar.");
        _log.AppendEquation($"{TransformationHelper.FormatNumber(eq.A)}(x² + {TransformationHelper.FormatNumber(eq.C / eq.A)}x) + {TransformationHelper.FormatNumber(eq.B)}(y² + {TransformationHelper.FormatNumber(eq.D / eq.B)}y) = {TransformationHelper.FormatNumber(-eq.E)}");

        _log.AppendStep("Completar cuadrados.");
        _log.AppendEquation($"{TransformationHelper.FormatNumber(eq.A)}(x - {TransformationHelper.FormatNumber(h)})² + {TransformationHelper.FormatNumber(eq.B)}(y - {TransformationHelper.FormatNumber(k)})² = {TransformationHelper.FormatNumber(rhs)}");

        _log.AppendStep("Igualar a 1 dividiendo por el término independiente.");
    }


    private (string, ConicElements?) TransformParabola(GeneralEquation eq)
    {
        if (eq.B == 0)
        {
            double h = -eq.C / (2 * eq.A);
            double k = (-eq.E + eq.A * h * h) / eq.D;
            double p = -eq.D / (4 * eq.A);

            LogTransformParabolaSteps(eq, h, k, true);

            var elements = _elementsFactory.CreateParabolaElements(h, k, p, true);

            return ($"{TransformationHelper.FormatBracket('x', h)}² = {TransformationHelper.FormatNumber(4 * p)}{TransformationHelper.FormatBracket('y', k)}", elements);
        }
        else
        {
            double kAlt = -eq.D / (2 * eq.B);
            double hAlt = (-eq.E + eq.B * kAlt * kAlt) / eq.C;
            double pAlt = -eq.C / (4 * eq.B);

            LogTransformParabolaSteps(eq, hAlt, kAlt, false);

            var elements = _elementsFactory.CreateParabolaElements(hAlt, kAlt, pAlt, false);

            return ($"{TransformationHelper.FormatBracket('y', kAlt)}² = {TransformationHelper.FormatNumber(4 * pAlt)}{TransformationHelper.FormatBracket('x', hAlt)}", elements);
        }
    }

    private void LogTransformParabolaSteps(GeneralEquation eq, double h, double k, bool isXSquared)
    {
        if (isXSquared)
        {
            _log.AppendStep("Aislar la variable al cuadrado (x).");
            _log.AppendEquation($"{TransformationHelper.FormatNumber(eq.A)}x² + {TransformationHelper.FormatNumber(eq.C)}x = -{TransformationHelper.FormatNumber(eq.D)}y - {TransformationHelper.FormatNumber(eq.E)}");

            _log.AppendStep("Completar el cuadrado para x.");
            _log.AppendEquation($"{TransformationHelper.FormatNumber(eq.A)}(x - {TransformationHelper.FormatNumber(h)})² = -{TransformationHelper.FormatNumber(eq.D)}(y - {TransformationHelper.FormatNumber(k)})");
        }
        else
        {
            _log.AppendStep("Aislar la variable al cuadrado (y).");
            _log.AppendEquation($"{TransformationHelper.FormatNumber(eq.B)}y² + {TransformationHelper.FormatNumber(eq.D)}y = -{TransformationHelper.FormatNumber(eq.C)}x - {TransformationHelper.FormatNumber(eq.E)}");

            _log.AppendStep("Completar el cuadrado para y.");
            _log.AppendEquation($"{TransformationHelper.FormatNumber(eq.B)}(y - {TransformationHelper.FormatNumber(k)})² = -{TransformationHelper.FormatNumber(eq.C)}(x - {TransformationHelper.FormatNumber(h)})");
        }
    }
}
