using System.Collections.Generic;
using RutGeo.Core.Helpers;
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
        _log.StartProcess($"Procedimiento inverso: {conic.Type}");

        return conic.Type switch
        {
            ConicType.Circunferencia => InverseCircle(canonicalEquation),
            ConicType.Elipse => InverseEllipse(canonicalEquation),
            ConicType.Hyperbola => InverseHyperbola(canonicalEquation),
            ConicType.Parabola => InverseParabola(canonicalEquation),
            _ => throw new NotImplementedException()
        };
    }

    private GeneralEquation InverseCircle(CanonicalEquation canon)
    {
        var c = (CircleElements)canon.Elements!;
        double h = c.Center!.X;
        double k = c.Center.Y;
        double r = c.Radius;
        double h2 = h * h;
        double k2 = k * k;
        double r2 = r * r;

        double A = 1;
        double B = 1;
        double Cc = -2 * h;
        double Dc = -2 * k;
        double Ec = h2 + k2 - r2;

        _log.AppendStep("Partir de la forma canónica:");
        _log.AppendEquation($"(x - {FormatNumber(h)})² + (y - {FormatNumber(k)})² = {FormatNumber(r2)}");
        _log.AppendStep("Expandir los cuadrados:");
        _log.AppendEquation($"x² - {FormatNumber(2 * h)}x + {FormatNumber(h2)} + y² - {FormatNumber(2 * k)}y + {FormatNumber(k2)} = {FormatNumber(r2)}");
        _log.AppendStep("Agrupar términos e igualar a cero:");
        _log.AppendEquation($"x² + y² + {FormatNumber(Cc)}x + {FormatNumber(Dc)}y + {FormatNumber(Ec)} = 0");

        return new GeneralEquation { A = A, B = B, C = Cc, D = Dc, E = Ec };
    }

    private GeneralEquation InverseEllipse(CanonicalEquation canon)
    {
        var e = (EllipseElements)canon.Elements!;
        double h = e.Center!.X;
        double k = e.Center.Y;
        double a = e.MajorAxisLength / 2;
        double b = e.MinorAxisLength / 2;
        double a2 = a * a;
        double b2 = b * b;

        bool isHorizontal = e.MajorVertices[0].Y == k;

        _log.AppendStep("Partir de la forma canónica:");
        if (isHorizontal)
            _log.AppendEquation($"(x - {FormatNumber(h)})² / {FormatNumber(a2)} + (y - {FormatNumber(k)})² / {FormatNumber(b2)} = 1");
        else
            _log.AppendEquation($"(x - {FormatNumber(h)})² / {FormatNumber(b2)} + (y - {FormatNumber(k)})² / {FormatNumber(a2)} = 1");

        double A, B, Cc, Dc, Ec;
        if (isHorizontal)
        {
            _log.AppendStep("Multiplicar por el producto a²·b²:");
            _log.AppendEquation($"{FormatNumber(b2)}(x - {FormatNumber(h)})² + {FormatNumber(a2)}(y - {FormatNumber(k)})² = {FormatNumber(a2 * b2)}");
            _log.AppendStep("Expandir los cuadrados:");
            _log.AppendEquation($"{FormatNumber(b2)}x² - {FormatNumber(2 * b2 * h)}x + {FormatNumber(b2 * h * h)} + {FormatNumber(a2)}y² - {FormatNumber(2 * a2 * k)}y + {FormatNumber(a2 * k * k)} = {FormatNumber(a2 * b2)}");
            _log.AppendStep("Agrupar e igualar a cero:");
            A = b2; B = a2;
            Cc = -2 * b2 * h;
            Dc = -2 * a2 * k;
            Ec = b2 * h * h + a2 * k * k - a2 * b2;
        }
        else
        {
            _log.AppendStep("Multiplicar por el producto a²·b²:");
            _log.AppendEquation($"{FormatNumber(a2)}(x - {FormatNumber(h)})² + {FormatNumber(b2)}(y - {FormatNumber(k)})² = {FormatNumber(a2 * b2)}");
            _log.AppendStep("Expandir los cuadrados:");
            _log.AppendEquation($"{FormatNumber(a2)}x² - {FormatNumber(2 * a2 * h)}x + {FormatNumber(a2 * h * h)} + {FormatNumber(b2)}y² - {FormatNumber(2 * b2 * k)}y + {FormatNumber(b2 * k * k)} = {FormatNumber(a2 * b2)}");
            _log.AppendStep("Agrupar e igualar a cero:");
            A = a2; B = b2;
            Cc = -2 * a2 * h;
            Dc = -2 * b2 * k;
            Ec = a2 * h * h + b2 * k * k - a2 * b2;
        }

        _log.AppendEquation($"{FormatNumber(A)}x² + {FormatNumber(B)}y² + {FormatNumber(Cc)}x + {FormatNumber(Dc)}y + {FormatNumber(Ec)} = 0");
        return new GeneralEquation { A = A, B = B, C = Cc, D = Dc, E = Ec };
    }

    private GeneralEquation InverseHyperbola(CanonicalEquation canon)
    {
        var h = (HyperbolaElements)canon.Elements!;
        double hc = h.Center!.X;
        double k = h.Center.Y;
        double a = h.TransverseAxisLength / 2;
        double b = h.ConjugateAxisLength / 2;
        double a2 = a * a;
        double b2 = b * b;

        bool isHorizontal = h.Vertices[0].Y == k;

        _log.AppendStep("Partir de la forma canónica:");
        if (isHorizontal)
            _log.AppendEquation($"(x - {FormatNumber(hc)})² / {FormatNumber(a2)} - (y - {FormatNumber(k)})² / {FormatNumber(b2)} = 1");
        else
            _log.AppendEquation($"(y - {FormatNumber(k)})² / {FormatNumber(a2)} - (x - {FormatNumber(hc)})² / {FormatNumber(b2)} = 1");

        double A, B, Cc, Dc, Ec;
        if (isHorizontal)
        {
            _log.AppendStep("Multiplicar por a²·b²:");
            _log.AppendEquation($"{FormatNumber(b2)}(x - {FormatNumber(hc)})² - {FormatNumber(a2)}(y - {FormatNumber(k)})² = {FormatNumber(a2 * b2)}");
            _log.AppendStep("Expandir:");
            _log.AppendEquation($"{FormatNumber(b2)}x² - {FormatNumber(2 * b2 * hc)}x + {FormatNumber(b2 * hc * hc)} - {FormatNumber(a2)}y² + {FormatNumber(2 * a2 * k)}y - {FormatNumber(a2 * k * k)} = {FormatNumber(a2 * b2)}");
            _log.AppendStep("Agrupar:");
            A = b2; B = -a2;
            Cc = -2 * b2 * hc;
            Dc = 2 * a2 * k;
            Ec = b2 * hc * hc - a2 * k * k - a2 * b2;
        }
        else
        {
            _log.AppendStep("Multiplicar por a²·b²:");
            _log.AppendEquation($"{FormatNumber(b2)}(y - {FormatNumber(k)})² - {FormatNumber(a2)}(x - {FormatNumber(hc)})² = {FormatNumber(a2 * b2)}");
            _log.AppendStep("Expandir:");
            _log.AppendEquation($"{FormatNumber(b2)}y² - {FormatNumber(2 * b2 * k)}y + {FormatNumber(b2 * k * k)} - {FormatNumber(a2)}x² + {FormatNumber(2 * a2 * hc)}x - {FormatNumber(a2 * hc * hc)} = {FormatNumber(a2 * b2)}");
            _log.AppendStep("Agrupar:");
            A = -a2; B = b2;
            Cc = 2 * a2 * hc;
            Dc = -2 * b2 * k;
            Ec = b2 * k * k - a2 * hc * hc - a2 * b2;
        }

        _log.AppendEquation($"{FormatNumber(A)}x² + {FormatNumber(B)}y² + {FormatNumber(Cc)}x + {FormatNumber(Dc)}y + {FormatNumber(Ec)} = 0");
        return new GeneralEquation { A = A, B = B, C = Cc, D = Dc, E = Ec };
    }

    private GeneralEquation InverseParabola(CanonicalEquation canon)
    {
        var p = (ParabolaElements)canon.Elements!;
        double h = p.Vertex.X;
        double k = p.Vertex.Y;
        double focal = p.FocalDistance;
        bool isVertical = p.AxisOfSymmetry.A == 1 && p.AxisOfSymmetry.B == 0;

        _log.AppendStep("Partir de la forma canónica:");
        if (isVertical)
            _log.AppendEquation($"(x - {FormatNumber(h)})² = {FormatNumber(4 * focal)}(y - {FormatNumber(k)})");
        else
            _log.AppendEquation($"(y - {FormatNumber(k)})² = {FormatNumber(4 * focal)}(x - {FormatNumber(h)})");

        double A, B, Cc, Dc, Ec;
        if (isVertical)
        {
            _log.AppendStep("Expandir el cuadrado y distribuir:");
            _log.AppendEquation($"x² - {FormatNumber(2 * h)}x + {FormatNumber(h * h)} = {FormatNumber(4 * focal)}y - {FormatNumber(4 * focal * k)}");
            _log.AppendStep("Agrupar todo a un lado:");
            A = 1; B = 0;
            Cc = -2 * h;
            Dc = -4 * focal;
            Ec = h * h + 4 * focal * k;
        }
        else
        {
            _log.AppendStep("Expandir el cuadrado y distribuir:");
            _log.AppendEquation($"y² - {FormatNumber(2 * k)}y + {FormatNumber(k * k)} = {FormatNumber(4 * focal)}x - {FormatNumber(4 * focal * h)}");
            _log.AppendStep("Agrupar todo a un lado:");
            A = 0; B = 1;
            Cc = -4 * focal;
            Dc = -2 * k;
            Ec = k * k + 4 * focal * h;
        }

        _log.AppendEquation($"{FormatNumber(A)}x² + {FormatNumber(B)}y² + {FormatNumber(Cc)}x + {FormatNumber(Dc)}y + {FormatNumber(Ec)} = 0");
        return new GeneralEquation { A = A, B = B, C = Cc, D = Dc, E = Ec };
    }



    private (string, ConicElements?) TransformCircle(GeneralEquation equation)
    {
        double h = -equation.C / (2 * equation.A);
        double k = -equation.D / (2 * equation.A);
        double r2 = h * h + k * k - equation.E / equation.A;

        LogTransformCircleSteps(equation, h, k, r2);

        var elements = _elementsFactory.CreateCircleElements(h, k, r2);

        return ($"{FormatBracket('x', h)}² + {FormatBracket('y', k)}² = {FormatNumber(r2)}", elements);
    }
    private void LogTransformCircleSteps(GeneralEquation eq, double h, double k, double r2)
    {
        _log.AppendStep("Agrupar términos en x e y, despejar la constante.");
        _log.AppendEquation($"(x² + {FormatNumber(eq.C / eq.A)}x) + (y² + {FormatNumber(eq.D / eq.A)}y) = {FormatNumber(-eq.E / eq.A)}");

        _log.AppendStep("Completar cuadrados para x e y.");
        _log.AppendEquation($"(x + {FormatNumber(eq.C / (2 * eq.A))})² + (y + {FormatNumber(eq.D / (2 * eq.A))})² = {FormatNumber(r2)}");

        _log.AppendStep("Identificar centro (h, k) y radio al cuadrado (r²).");
        _log.AppendStep($"Centro: ({FormatNumber(h)}, {FormatNumber(k)})");
        _log.AppendStep($"Radio: {FormatNumber(RutGeoMath.Sqrt(r2))}");
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

        return ($"{FormatBracket('x', h)}² / {FormatNumber(a2)} + {FormatBracket('y', k)}² / {FormatNumber(b2)} = 1", elements);
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
        {
            return ($"{FormatBracket('x', h)}² / {FormatNumber(a2)} - {FormatBracket('y', k)}² / {FormatNumber(b2)} = 1", elements);
        }

        return ($"{FormatBracket('y', k)}² / {FormatNumber(a2)} - {FormatBracket('x', h)}² / {FormatNumber(b2)} = 1", elements);
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
            double k = (-eq.E + eq.A * h * h) / eq.D;
            double p = -eq.D / (4 * eq.A);

            LogTransformParabolaSteps(eq, h, k, true);

            var elements = _elementsFactory.CreateParabolaElements(h, k, p, true);

            return ($"{FormatBracket('x', h)}² = {FormatNumber(4 * p)}{FormatBracket('y', k)}", elements);
        }
        else
        {
            double kAlt = -eq.D / (2 * eq.B);
            double hAlt = (-eq.E + eq.B * kAlt * kAlt) / eq.C;
            double pAlt = -eq.C / (4 * eq.B);

            LogTransformParabolaSteps(eq, hAlt, kAlt, false);

            var elements = _elementsFactory.CreateParabolaElements(hAlt, kAlt, pAlt, false);

            return ($"{FormatBracket('y', kAlt)}² = {FormatNumber(4 * pAlt)}{FormatBracket('x', hAlt)}", elements);
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

    private static string FormatBracket(char variable, double value)
    {
        if (value >= 0)
            return $"({variable} - {FormatNumber(value)})";
        return $"({variable} + {FormatNumber(-value)})";
    }
}
