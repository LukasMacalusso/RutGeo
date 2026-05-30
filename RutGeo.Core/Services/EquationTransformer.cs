using System;
using RutGeo.Core.Interfaces;
using RutGeo.Core.Models;

namespace RutGeo.Core.Services;

public class EquationTransformer : IEquationTransformer
{
    public CanonicalEquation TransformToCanonical(GeneralEquation generalEquation, Conic conic, IExplanationLog log)
    {
        log.StartProcess($"Transformación a Forma Canónica: {conic.Type}");
        string canonicalEquationString = string.Empty;

        switch (conic.Type)
        {
            case ConicType.Circunferencia:
                canonicalEquationString = TransformCircle(generalEquation, log);
                break;
            case ConicType.Elipse:
                canonicalEquationString = TransformEllipse(generalEquation, log);
                break;
            case ConicType.Hyperbola:
                canonicalEquationString = TransformHyperbola(generalEquation, log);
                break;
            case ConicType.Parabola:
                canonicalEquationString = TransformParabola(generalEquation, log);
                break;
            case ConicType.Desconocida:
            default:
                log.AppendStep("Error: Ecuación degenerada o tipo de cónica desconocida.");
                break;
        }

        return new CanonicalEquation
        {
            ConicType = conic.Type,
            FormattedString = canonicalEquationString
        };
    }

    public GeneralEquation TransformToGeneral(CanonicalEquation canonicalEquation, Conic conic, IExplanationLog log)
    {
        throw new NotImplementedException();
    }

    private string TransformCircle(GeneralEquation eq, IExplanationLog log)
    {
        double h = -eq.C / (2 * eq.A);
        double k = -eq.D / (2 * eq.A);
        double r2 = h * h + k * k - eq.E / eq.A;

        log.AppendStep("Agrupar términos en x e y, despejar la constante.");
        log.AppendEquation($"(x² + {FormatNumber(eq.C / eq.A)}x) + (y² + {FormatNumber(eq.D / eq.A)}y) = {FormatNumber(-eq.E / eq.A)}");
        
        log.AppendStep("Completar cuadrados para x e y.");
        log.AppendEquation($"(x + {FormatNumber(eq.C / (2 * eq.A))})² + (y + {FormatNumber(eq.D / (2 * eq.A))})² = {FormatNumber(r2)}");
        
        log.AppendStep("Identificar centro (h, k) y radio al cuadrado (r²).");
        log.AppendStep($"Centro: ({FormatNumber(h)}, {FormatNumber(k)})");
        log.AppendStep($"Radio: {FormatNumber(Math.Sqrt(r2))}");

        return $"(x - {FormatNumber(h)})² + (y - {FormatNumber(k)})² = {FormatNumber(r2)}";
    }

    private string TransformEllipse(GeneralEquation eq, IExplanationLog log)
    {
        double h = -eq.C / (2 * eq.A);
        double k = -eq.D / (2 * eq.B);
        double rhs = -eq.E + eq.A * h * h + eq.B * k * k;
        double a2 = rhs / eq.A;
        double b2 = rhs / eq.B;

        log.AppendStep("Factorizar coeficientes principales.");
        log.AppendEquation($"{FormatNumber(eq.A)}(x² + {FormatNumber(eq.C / eq.A)}x) + {FormatNumber(eq.B)}(y² + {FormatNumber(eq.D / eq.B)}y) = {FormatNumber(-eq.E)}");
        
        log.AppendStep("Completar cuadrados.");
        log.AppendEquation($"{FormatNumber(eq.A)}(x - {FormatNumber(h)})² + {FormatNumber(eq.B)}(y - {FormatNumber(k)})² = {FormatNumber(rhs)}");
        
        log.AppendStep("Dividir todo por el resultado para igualar a 1.");

        return $"(x - {FormatNumber(h)})² / {FormatNumber(a2)} + (y - {FormatNumber(k)})² / {FormatNumber(b2)} = 1";
    }

    private string TransformHyperbola(GeneralEquation eq, IExplanationLog log)
    {
        double h = -eq.C / (2 * eq.A);
        double k = -eq.D / (2 * eq.B);
        double rhs = -eq.E + eq.A * h * h + eq.B * k * k;

        if (Math.Abs(rhs) < 1e-12)
        {
            log.AppendStep("El resultado es una hipérbola degenerada (asíntotas).");
            return "0 = 0 (Degenerada)";
        }

        log.AppendStep("Factorizar y agrupar.");
        log.AppendEquation($"{FormatNumber(eq.A)}(x² + {FormatNumber(eq.C / eq.A)}x) + {FormatNumber(eq.B)}(y² + {FormatNumber(eq.D / eq.B)}y) = {FormatNumber(-eq.E)}");

        log.AppendStep("Completar cuadrados.");
        log.AppendEquation($"{FormatNumber(eq.A)}(x - {FormatNumber(h)})² + {FormatNumber(eq.B)}(y - {FormatNumber(k)})² = {FormatNumber(rhs)}");

        log.AppendStep("Igualar a 1 dividiendo por el término independiente.");

        if (rhs > 0)
        {
            return $"(x - {FormatNumber(h)})² / {FormatNumber(rhs / eq.A)} - (y - {FormatNumber(k)})² / {FormatNumber(-rhs / eq.B)} = 1";
        }

        return $"(y - {FormatNumber(k)})² / {FormatNumber(rhs / eq.B)} - (x - {FormatNumber(h)})² / {FormatNumber(-rhs / eq.A)} = 1";
    }

    private string TransformParabola(GeneralEquation eq, IExplanationLog log)
    {
        if (eq.B == 0)
        {
            double h = -eq.C / (2 * eq.A);
            double k = (-eq.E + eq.A * h * h) / -eq.D;
            double p = -eq.D / (4 * eq.A);

            log.AppendStep("Aislar la variable al cuadrado (x).");
            log.AppendEquation($"{FormatNumber(eq.A)}x² + {FormatNumber(eq.C)}x = -{FormatNumber(eq.D)}y - {FormatNumber(eq.E)}");

            log.AppendStep("Completar el cuadrado para x.");
            log.AppendEquation($"{FormatNumber(eq.A)}(x - {FormatNumber(h)})² = -{FormatNumber(eq.D)}(y - {FormatNumber(k)})");

            return $"(x - {FormatNumber(h)})² = {FormatNumber(4 * p)}(y - {FormatNumber(k)})";
        }

        double kAlt = -eq.D / (2 * eq.B);
        double hAlt = (-eq.E + eq.B * kAlt * kAlt) / -eq.C;
        double pAlt = -eq.C / (4 * eq.B);

        log.AppendStep("Aislar la variable al cuadrado (y).");
        log.AppendEquation($"{FormatNumber(eq.B)}y² + {FormatNumber(eq.D)}y = -{FormatNumber(eq.C)}x - {FormatNumber(eq.E)}");

        log.AppendStep("Completar el cuadrado para y.");
        log.AppendEquation($"{FormatNumber(eq.B)}(y - {FormatNumber(kAlt)})² = -{FormatNumber(eq.C)}(x - {FormatNumber(hAlt)})");

        return $"(y - {FormatNumber(kAlt)})² = {FormatNumber(4 * pAlt)}(x - {FormatNumber(hAlt)})";
    }

    private static string FormatNumber(double value)
    {
        return value.ToString("0.##");
    }
}
