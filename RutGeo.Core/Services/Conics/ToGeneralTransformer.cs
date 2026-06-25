using RutGeo.Core.Helpers;
using RutGeo.Core.Interfaces.Conics;
using RutGeo.Core.Interfaces.Common;
using RutGeo.Core.Models;
using RutGeo.Core.Models.Equations;
using RutGeo.Core.Models.Types;

namespace RutGeo.Core.Services.Conics;

public class ToGeneralTransformer : IToGeneralTransformer
{
    private readonly IExplanationLogger _log;

    public ToGeneralTransformer(IExplanationLogger log)
    {
        _log = log;
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
        _log.AppendEquation($"(x - {TransformationHelper.FormatNumber(h)})² + (y - {TransformationHelper.FormatNumber(k)})² = {TransformationHelper.FormatNumber(r2)}");
        _log.AppendStep("Expandir los cuadrados:");
        _log.AppendEquation($"x² - {TransformationHelper.FormatNumber(2 * h)}x + {TransformationHelper.FormatNumber(h2)} + y² - {TransformationHelper.FormatNumber(2 * k)}y + {TransformationHelper.FormatNumber(k2)} = {TransformationHelper.FormatNumber(r2)}");
        _log.AppendStep("Agrupar términos e igualar a cero:");
        _log.AppendEquation($"x² + y² + {TransformationHelper.FormatNumber(Cc)}x + {TransformationHelper.FormatNumber(Dc)}y + {TransformationHelper.FormatNumber(Ec)} = 0");

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
            _log.AppendEquation($"(x - {TransformationHelper.FormatNumber(h)})² / {TransformationHelper.FormatNumber(a2)} + (y - {TransformationHelper.FormatNumber(k)})² / {TransformationHelper.FormatNumber(b2)} = 1");
        else
            _log.AppendEquation($"(x - {TransformationHelper.FormatNumber(h)})² / {TransformationHelper.FormatNumber(b2)} + (y - {TransformationHelper.FormatNumber(k)})² / {TransformationHelper.FormatNumber(a2)} = 1");

        double A, B, Cc, Dc, Ec;
        if (isHorizontal)
        {
            _log.AppendStep("Multiplicar por el producto a²·b²:");
            _log.AppendEquation($"{TransformationHelper.FormatNumber(b2)}(x - {TransformationHelper.FormatNumber(h)})² + {TransformationHelper.FormatNumber(a2)}(y - {TransformationHelper.FormatNumber(k)})² = {TransformationHelper.FormatNumber(a2 * b2)}");
            _log.AppendStep("Expandir los cuadrados:");
            _log.AppendEquation($"{TransformationHelper.FormatNumber(b2)}x² - {TransformationHelper.FormatNumber(2 * b2 * h)}x + {TransformationHelper.FormatNumber(b2 * h * h)} + {TransformationHelper.FormatNumber(a2)}y² - {TransformationHelper.FormatNumber(2 * a2 * k)}y + {TransformationHelper.FormatNumber(a2 * k * k)} = {TransformationHelper.FormatNumber(a2 * b2)}");
            _log.AppendStep("Agrupar e igualar a cero:");
            A = b2; B = a2;
            Cc = -2 * b2 * h;
            Dc = -2 * a2 * k;
            Ec = b2 * h * h + a2 * k * k - a2 * b2;
        }
        else
        {
            _log.AppendStep("Multiplicar por el producto a²·b²:");
            _log.AppendEquation($"{TransformationHelper.FormatNumber(a2)}(x - {TransformationHelper.FormatNumber(h)})² + {TransformationHelper.FormatNumber(b2)}(y - {TransformationHelper.FormatNumber(k)})² = {TransformationHelper.FormatNumber(a2 * b2)}");
            _log.AppendStep("Expandir los cuadrados:");
            _log.AppendEquation($"{TransformationHelper.FormatNumber(a2)}x² - {TransformationHelper.FormatNumber(2 * a2 * h)}x + {TransformationHelper.FormatNumber(a2 * h * h)} + {TransformationHelper.FormatNumber(b2)}y² - {TransformationHelper.FormatNumber(2 * b2 * k)}y + {TransformationHelper.FormatNumber(b2 * k * k)} = {TransformationHelper.FormatNumber(a2 * b2)}");
            _log.AppendStep("Agrupar e igualar a cero:");
            A = a2; B = b2;
            Cc = -2 * a2 * h;
            Dc = -2 * b2 * k;
            Ec = a2 * h * h + b2 * k * k - a2 * b2;
        }

        _log.AppendEquation($"{TransformationHelper.FormatNumber(A)}x² + {TransformationHelper.FormatNumber(B)}y² + {TransformationHelper.FormatNumber(Cc)}x + {TransformationHelper.FormatNumber(Dc)}y + {TransformationHelper.FormatNumber(Ec)} = 0");
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
            _log.AppendEquation($"(x - {TransformationHelper.FormatNumber(hc)})² / {TransformationHelper.FormatNumber(a2)} - (y - {TransformationHelper.FormatNumber(k)})² / {TransformationHelper.FormatNumber(b2)} = 1");
        else
            _log.AppendEquation($"(y - {TransformationHelper.FormatNumber(k)})² / {TransformationHelper.FormatNumber(a2)} - (x - {TransformationHelper.FormatNumber(hc)})² / {TransformationHelper.FormatNumber(b2)} = 1");

        double A, B, Cc, Dc, Ec;
        if (isHorizontal)
        {
            _log.AppendStep("Multiplicar por a²·b²:");
            _log.AppendEquation($"{TransformationHelper.FormatNumber(b2)}(x - {TransformationHelper.FormatNumber(hc)})² - {TransformationHelper.FormatNumber(a2)}(y - {TransformationHelper.FormatNumber(k)})² = {TransformationHelper.FormatNumber(a2 * b2)}");
            _log.AppendStep("Expandir:");
            _log.AppendEquation($"{TransformationHelper.FormatNumber(b2)}x² - {TransformationHelper.FormatNumber(2 * b2 * hc)}x + {TransformationHelper.FormatNumber(b2 * hc * hc)} - {TransformationHelper.FormatNumber(a2)}y² + {TransformationHelper.FormatNumber(2 * a2 * k)}y - {TransformationHelper.FormatNumber(a2 * k * k)} = {TransformationHelper.FormatNumber(a2 * b2)}");
            _log.AppendStep("Agrupar:");
            A = b2; B = -a2;
            Cc = -2 * b2 * hc;
            Dc = 2 * a2 * k;
            Ec = b2 * hc * hc - a2 * k * k - a2 * b2;
        }
        else
        {
            _log.AppendStep("Multiplicar por a²·b²:");
            _log.AppendEquation($"{TransformationHelper.FormatNumber(b2)}(y - {TransformationHelper.FormatNumber(k)})² - {TransformationHelper.FormatNumber(a2)}(x - {TransformationHelper.FormatNumber(hc)})² = {TransformationHelper.FormatNumber(a2 * b2)}");
            _log.AppendStep("Expandir:");
            _log.AppendEquation($"{TransformationHelper.FormatNumber(b2)}y² - {TransformationHelper.FormatNumber(2 * b2 * k)}y + {TransformationHelper.FormatNumber(b2 * k * k)} - {TransformationHelper.FormatNumber(a2)}x² + {TransformationHelper.FormatNumber(2 * a2 * hc)}x - {TransformationHelper.FormatNumber(a2 * hc * hc)} = {TransformationHelper.FormatNumber(a2 * b2)}");
            _log.AppendStep("Agrupar:");
            A = -a2; B = b2;
            Cc = 2 * a2 * hc;
            Dc = -2 * b2 * k;
            Ec = b2 * k * k - a2 * hc * hc - a2 * b2;
        }

        _log.AppendEquation($"{TransformationHelper.FormatNumber(A)}x² + {TransformationHelper.FormatNumber(B)}y² + {TransformationHelper.FormatNumber(Cc)}x + {TransformationHelper.FormatNumber(Dc)}y + {TransformationHelper.FormatNumber(Ec)} = 0");
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
            _log.AppendEquation($"(x - {TransformationHelper.FormatNumber(h)})² = {TransformationHelper.FormatNumber(4 * focal)}(y - {TransformationHelper.FormatNumber(k)})");
        else
            _log.AppendEquation($"(y - {TransformationHelper.FormatNumber(k)})² = {TransformationHelper.FormatNumber(4 * focal)}(x - {TransformationHelper.FormatNumber(h)})");

        double A, B, Cc, Dc, Ec;
        if (isVertical)
        {
            _log.AppendStep("Expandir el cuadrado y distribuir:");
            _log.AppendEquation($"x² - {TransformationHelper.FormatNumber(2 * h)}x + {TransformationHelper.FormatNumber(h * h)} = {TransformationHelper.FormatNumber(4 * focal)}y - {TransformationHelper.FormatNumber(4 * focal * k)}");
            _log.AppendStep("Agrupar todo a un lado:");
            A = 1; B = 0;
            Cc = -2 * h;
            Dc = -4 * focal;
            Ec = h * h + 4 * focal * k;
        }
        else
        {
            _log.AppendStep("Expandir el cuadrado y distribuir:");
            _log.AppendEquation($"y² - {TransformationHelper.FormatNumber(2 * k)}y + {TransformationHelper.FormatNumber(k * k)} = {TransformationHelper.FormatNumber(4 * focal)}x - {TransformationHelper.FormatNumber(4 * focal * h)}");
            _log.AppendStep("Agrupar todo a un lado:");
            A = 0; B = 1;
            Cc = -4 * focal;
            Dc = -2 * k;
            Ec = k * k + 4 * focal * h;
        }

        _log.AppendEquation($"{TransformationHelper.FormatNumber(A)}x² + {TransformationHelper.FormatNumber(B)}y² + {TransformationHelper.FormatNumber(Cc)}x + {TransformationHelper.FormatNumber(Dc)}y + {TransformationHelper.FormatNumber(Ec)} = 0");
        return new GeneralEquation { A = A, B = B, C = Cc, D = Dc, E = Ec };
    }
}
