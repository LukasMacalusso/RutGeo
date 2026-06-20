using System;
using System.Collections.Generic;
using RutGeo.Core.Interfaces;
using RutGeo.Core.Models;

namespace RutGeo.Core.Services;

public class EquationTransformer : IEquationTransformer
{
    public CanonicalEquation TransformToCanonical(GeneralEquation generalEquation, Conic conic, IExplanationLog log)
    {
        log.StartProcess($"Transformación a Forma Canónica: {conic.Type}");
        string canonicalEquationString = string.Empty;
        ConicsElements? elements = null;

        switch (conic.Type)
        {
            case ConicType.Circunferencia:
                (canonicalEquationString, elements) = TransformCircle(generalEquation, log);
                break;
            case ConicType.Elipse:
                (canonicalEquationString, elements) = TransformEllipse(generalEquation, log);
                break;
            case ConicType.Hyperbola:
                (canonicalEquationString, elements) = TransformHyperbola(generalEquation, log);
                break;
            case ConicType.Parabola:
                (canonicalEquationString, elements) = TransformParabola(generalEquation, log);
                break;
            case ConicType.Desconocida:
            default:
                log.AppendStep("Error: Ecuación degenerada o tipo de cónica desconocida.");
                break;
        }

        return new CanonicalEquation
        {
            ConicType = conic.Type,
            FormattedString = canonicalEquationString,
            Elements = elements
        };
    }

    public GeneralEquation TransformToGeneral(CanonicalEquation canonicalEquation, Conic conic, IExplanationLog log)
    {
        throw new NotImplementedException();
    }

    private (string, ConicsElements?) TransformCircle(GeneralEquation equation, IExplanationLog log)
    {
        double h = -equation.C / (2 * equation.A);
        double k = -equation.D / (2 * equation.A);
        double r2 = h * h + k * k - equation.E / equation.A;

        LogTransformCircleSteps(log, equation, h, k, r2);

        var elements = new CircleElements
        {
            Center = new Point2D(h, k),
            Radius = Math.Sqrt(r2)
        };

        return ($"(x - {FormatNumber(h)})² + (y - {FormatNumber(k)})² = {FormatNumber(r2)}", elements);
    }
    private void LogTransformCircleSteps(IExplanationLog log, GeneralEquation eq, double h, double k, double r2)
    {
        log.AppendStep("Agrupar términos en x e y, despejar la constante.");
        log.AppendEquation($"(x² + {FormatNumber(eq.C / eq.A)}x) + (y² + {FormatNumber(eq.D / eq.A)}y) = {FormatNumber(-eq.E / eq.A)}");

        log.AppendStep("Completar cuadrados para x e y.");
        log.AppendEquation($"(x + {FormatNumber(eq.C / (2 * eq.A))})² + (y + {FormatNumber(eq.D / (2 * eq.A))})² = {FormatNumber(r2)}");

        log.AppendStep("Identificar centro (h, k) y radio al cuadrado (r²).");
        log.AppendStep($"Centro: ({FormatNumber(h)}, {FormatNumber(k)})");
        log.AppendStep($"Radio: {FormatNumber(Math.Sqrt(r2))}");
    }


    private (string, ConicsElements?) TransformEllipse(GeneralEquation equation, IExplanationLog log)
    {
        double h = -equation.C / (2 * equation.A);
        double k = -equation.D / (2 * equation.B);
        double rhs = -equation.E + equation.A * h * h + equation.B * k * k;
        double a2 = rhs / equation.A;
        double b2 = rhs / equation.B;

        LogTransformEllipseSteps(log, equation, h, k, rhs);

        double a = Math.Sqrt(a2);
        double b = Math.Sqrt(b2);
        bool isHorizontal = a2 >= b2;
        double c2 = Math.Abs(a2 - b2);
        double c = Math.Sqrt(c2);

        var elements = new EllipseElements
        {
            Center = new Point2D(h, k),
            MajorAxisLength = isHorizontal ? 2 * a : 2 * b,
            MinorAxisLength = isHorizontal ? 2 * b : 2 * a,
            Eccentricity = c / (isHorizontal ? a : b),
            Foci = isHorizontal ? new List<Point2D> { new Point2D(h - c, k), new Point2D(h + c, k) }
                                : new List<Point2D> { new Point2D(h, k - c), new Point2D(h, k + c) },
            MajorVertices = isHorizontal ? new List<Point2D> { new Point2D(h - a, k), new Point2D(h + a, k) }
                                         : new List<Point2D> { new Point2D(h, k - b), new Point2D(h, k + b) },
            MinorVertices = isHorizontal ? new List<Point2D> { new Point2D(h, k - b), new Point2D(h, k + b) }
                                         : new List<Point2D> { new Point2D(h - a, k), new Point2D(h + a, k) }
        };

        return ($"(x - {FormatNumber(h)})² / {FormatNumber(a2)} + (y - {FormatNumber(k)})² / {FormatNumber(b2)} = 1", elements);
    }
    private void LogTransformEllipseSteps(IExplanationLog log, GeneralEquation equation, double h, double k, double rhs)
    {
        log.AppendStep("Factorizar coeficientes principales.");
        log.AppendEquation($"{FormatNumber(equation.A)}(x² + {FormatNumber(equation.C / equation.A)}x) + {FormatNumber(equation.B)}(y² + {FormatNumber(equation.D / equation.B)}y) = {FormatNumber(-equation.E)}");

        log.AppendStep("Completar cuadrados.");
        log.AppendEquation($"{FormatNumber(equation.A)}(x - {FormatNumber(h)})² + {FormatNumber(equation.B)}(y - {FormatNumber(k)})² = {FormatNumber(rhs)}");

        log.AppendStep("Dividir todo por el resultado para igualar a 1.");

    }


    private (string, ConicsElements?) TransformHyperbola(GeneralEquation eq, IExplanationLog log)
    {
        double h = -eq.C / (2 * eq.A);
        double k = -eq.D / (2 * eq.B);
        double rhs = -eq.E + eq.A * h * h + eq.B * k * k;

        if (Math.Abs(rhs) < 1e-12)
        {
            log.AppendStep("El resultado es una hipérbola degenerada (asíntotas).");
            return ("0 = 0 (Degenerada)", null);
        }

        LogTransformHyperbolaSteps(log, eq, h, k, rhs);

        bool isHorizontal = rhs > 0;
        double a2 = isHorizontal ? (rhs / eq.A) : (rhs / eq.B);
        double b2 = isHorizontal ? (-rhs / eq.B) : (-rhs / eq.A);

        double a = Math.Sqrt(a2);
        double b = Math.Sqrt(b2);
        double c2 = a2 + b2;
        double c = Math.Sqrt(c2);

        var elements = new HyperbolaElements
        {
            Center = new Point2D(h, k),
            TransverseAxisLength = 2 * a,
            ConjugateAxisLength = 2 * b,
            Eccentricity = c / a,
            Foci = isHorizontal ? new List<Point2D> { new Point2D(h - c, k), new Point2D(h + c, k) }
                                : new List<Point2D> { new Point2D(h, k - c), new Point2D(h, k + c) },
            Vertices = isHorizontal ? new List<Point2D> { new Point2D(h - a, k), new Point2D(h + a, k) }
                                    : new List<Point2D> { new Point2D(h, k - a), new Point2D(h, k + a) },
            Asymptotes = isHorizontal
                ? new List<Line2D> { new Line2D(b, -a, a * k - b * h), new Line2D(b, a, -a * k - b * h) }
                : new List<Line2D> { new Line2D(a, -b, b * k - a * h), new Line2D(a, b, -b * k - a * h) }
        };

        if (isHorizontal)
        {
            return ($"(x - {FormatNumber(h)})² / {FormatNumber(a2)} - (y - {FormatNumber(k)})² / {FormatNumber(b2)} = 1", elements);
        }

        return ($"(y - {FormatNumber(k)})² / {FormatNumber(a2)} - (x - {FormatNumber(h)})² / {FormatNumber(b2)} = 1", elements);
    }

    private void LogTransformHyperbolaSteps(IExplanationLog log, GeneralEquation eq, double h, double k, double rhs)
    {
        log.AppendStep("Factorizar y agrupar.");
        log.AppendEquation($"{FormatNumber(eq.A)}(x² + {FormatNumber(eq.C / eq.A)}x) + {FormatNumber(eq.B)}(y² + {FormatNumber(eq.D / eq.B)}y) = {FormatNumber(-eq.E)}");

        log.AppendStep("Completar cuadrados.");
        log.AppendEquation($"{FormatNumber(eq.A)}(x - {FormatNumber(h)})² + {FormatNumber(eq.B)}(y - {FormatNumber(k)})² = {FormatNumber(rhs)}");

        log.AppendStep("Igualar a 1 dividiendo por el término independiente.");
    }


    private (string, ConicsElements?) TransformParabola(GeneralEquation eq, IExplanationLog log)
    {
        if (eq.B == 0)
        {
            double h = -eq.C / (2 * eq.A);
            double k = (-eq.E + eq.A * h * h) / -eq.D;
            double p = -eq.D / (4 * eq.A);

            LogTransformParabolaSteps(log, eq, h, k, true);

            var elements = new ParabolaElements
            {
                Center = new Point2D(h, k),
                Vertex = new Point2D(h, k),
                Focus = new Point2D(h, k + p),
                Directrix = new Line2D(0, 1, -(k - p)), // y - (k-p) = 0
                AxisOfSymmetry = new Line2D(1, 0, -h),  // x - h = 0
                FocalDistance = p
            };

            return ($"(x - {FormatNumber(h)})² = {FormatNumber(4 * p)}(y - {FormatNumber(k)})", elements);
        }
        else
        {
            double kAlt = -eq.D / (2 * eq.B);
            double hAlt = (-eq.E + eq.B * kAlt * kAlt) / -eq.C;
            double pAlt = -eq.C / (4 * eq.B);

            LogTransformParabolaSteps(log, eq, hAlt, kAlt, false);

            var elements = new ParabolaElements
            {
                Center = new Point2D(hAlt, kAlt),
                Vertex = new Point2D(hAlt, kAlt),
                Focus = new Point2D(hAlt + pAlt, kAlt),
                Directrix = new Line2D(1, 0, -(hAlt - pAlt)), // x - (h-p) = 0
                AxisOfSymmetry = new Line2D(0, 1, -kAlt),     // y - k = 0
                FocalDistance = pAlt
            };

            return ($"(y - {FormatNumber(kAlt)})² = {FormatNumber(4 * pAlt)}(x - {FormatNumber(hAlt)})", elements);
        }
    }

    private void LogTransformParabolaSteps(IExplanationLog log, GeneralEquation eq, double h, double k, bool isXSquared)
    {
        if (isXSquared)
        {
            log.AppendStep("Aislar la variable al cuadrado (x).");
            log.AppendEquation($"{FormatNumber(eq.A)}x² + {FormatNumber(eq.C)}x = -{FormatNumber(eq.D)}y - {FormatNumber(eq.E)}");

            log.AppendStep("Completar el cuadrado para x.");
            log.AppendEquation($"{FormatNumber(eq.A)}(x - {FormatNumber(h)})² = -{FormatNumber(eq.D)}(y - {FormatNumber(k)})");
        }
        else
        {
            log.AppendStep("Aislar la variable al cuadrado (y).");
            log.AppendEquation($"{FormatNumber(eq.B)}y² + {FormatNumber(eq.D)}y = -{FormatNumber(eq.C)}x - {FormatNumber(eq.E)}");

            log.AppendStep("Completar el cuadrado para y.");
            log.AppendEquation($"{FormatNumber(eq.B)}(y - {FormatNumber(k)})² = -{FormatNumber(eq.C)}(x - {FormatNumber(h)})");
        }
    }


    private static string FormatNumber(double value)
    {
        return value.ToString("0.##");
    }
}
