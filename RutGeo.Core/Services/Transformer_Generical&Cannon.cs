using System.Text;
using RutGeo.Core.Interfaces;

namespace RutGeo.Core.Services;

public class Transformer : ITransformer
{
    public Equation Equation { get; }
    public Conic Conic { get; }

    public Transformer(Equation equation, Conic conic)
    {
        Equation = equation;
        Conic = conic;
    }

    public string StepByStep
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            
            switch (Conic.Type)
            {
                case "Parábola":
                    AppendParabolaCanonical(sb);
                    AppendParabolaGeneral(sb);
                    break;
                case "Hipérbola":
                    AppendHyperbolaCanonical(sb);
                    AppendHyperbolaGeneral(sb);
                    break;
                case "Circunferencia":
                    AppendCircleCanonical(sb);
                    AppendCircleGeneral(sb);
                    break;
                case "Elipse":
                    AppendEllipseCanonical(sb);
                    AppendEllipseGeneral(sb);
                    break;
            }
            return sb.ToString();
        }
    }
   


    private static string Fmt(double valor)
    {
        return valor.ToString("0.##");
    }
    private void AppendCircleCanonical(StringBuilder sb)
    {
        double a = Equation.A, c = Equation.C, d = Equation.D, e = Equation.E;
        double h = -c / (2 * a), k = -d / (2 * a);
        double r2 = h * h + k * k - e / a;

        sb.AppendLine("Paso 1: Dividir toda la ecuación por A:");
        sb.AppendLine($"  x² + y² + ({Fmt(c)}/{Fmt(a)})x + ({Fmt(d)}/{Fmt(a)})y + ({Fmt(e)}/{Fmt(a)}) = 0");
        sb.AppendLine($"  x² + y² + {Fmt(c / a)}x + {Fmt(d / a)}y + {Fmt(e / a)} = 0");
        sb.AppendLine();

        sb.AppendLine("Paso 2: Agrupar términos en x e y, despejar la constante:");
        sb.AppendLine($"  (x² + {Fmt(c / a)}x) + (y² + {Fmt(d / a)}y) = {Fmt(-e / a)}");
        sb.AppendLine();

        sb.AppendLine("Paso 3: Completar cuadrados:");
        sb.AppendLine($"  (x + {Fmt(c / (2 * a))})² - ({Fmt(c / (2 * a))})² + (y + {Fmt(d / (2 * a))})² - ({Fmt(d / (2 * a))})² = {Fmt(-e / a)}");
        sb.AppendLine();

        sb.AppendLine("Paso 4: Despejar los cuadrados:");
        sb.AppendLine($"  (x + {Fmt(c / (2 * a))})² + (y + {Fmt(d / (2 * a))})² = {Fmt(r2)}");
        sb.AppendLine();

        sb.AppendLine("Paso 5: Identificar centro y radio:");
        sb.AppendLine($"  h = {Fmt(h)}, k = {Fmt(k)}");
        sb.AppendLine($"  r² = {Fmt(r2)} → r = {Fmt(Math.Sqrt(r2))}");
        sb.AppendLine();

        sb.AppendLine("Forma canónica:");
        sb.AppendLine($"  (x - ({Fmt(-h)}))² + (y - ({Fmt(-k)}))² = {Fmt(r2)}");
        sb.AppendLine($"  (x - {Fmt(h)})² + (y - {Fmt(k)})² = {Fmt(r2)}");
    }

    private void AppendCircleGeneral(StringBuilder sb)
    {
        double a = Equation.A, c = Equation.C, d = Equation.D, e = Equation.E;
        double h = -c / (2 * a), k = -d / (2 * a);
        double r2 = h * h + k * k - e / a;

        sb.AppendLine("Forma canónica:");
        sb.AppendLine($"  (x - {Fmt(h)})² + (y - {Fmt(k)})² = {Fmt(r2)}");
        sb.AppendLine();

        sb.AppendLine("Paso 1: Desarrollar los cuadrados:");
        sb.AppendLine($"  (x² - {Fmt(2 * h)}x + {Fmt(h * h)}) + (y² - {Fmt(2 * k)}y + {Fmt(k * k)}) = {Fmt(r2)}");
        sb.AppendLine();

        sb.AppendLine("Paso 2: Agrupar términos:");
        sb.AppendLine($"  x² + y² - {Fmt(2 * h)}x - {Fmt(2 * k)}y + ({Fmt(h * h + k * k)}) = {Fmt(r2)}");
        sb.AppendLine();

        sb.AppendLine("Paso 3: Multiplicar por A y reordenar:");
        sb.AppendLine($"  {Fmt(a)}x² + {Fmt(a)}y² + {Fmt(c)}x + {Fmt(d)}y + {Fmt(e)} = 0");
        sb.AppendLine();

        sb.AppendLine("Ecuación general obtenida:");
        sb.AppendLine($"  {Equation.EquationString}");
    }

    // ─── ELIPSE ────────────────────────────────────────────────────

    private void AppendEllipseCanonical(StringBuilder sb)
    {
        double a = Equation.A, b = Equation.B, c = Equation.C, d = Equation.D, e = Equation.E;
        double h = -c / (2 * a), k = -d / (2 * b);
        double rhs = -e + a * h * h + b * k * k;
        double a2 = rhs / a, b2 = rhs / b;

        sb.AppendLine("Paso 1: Agrupar términos en x e y:");
        sb.AppendLine($"  ({Fmt(a)}x² + {Fmt(c)}x) + ({Fmt(b)}y² + {Fmt(d)}y) = {Fmt(-e)}");
        sb.AppendLine();

        sb.AppendLine("Paso 2: Factorizar coeficientes:");
        sb.AppendLine($"  {Fmt(a)}(x² + {Fmt(c / a)}x) + {Fmt(b)}(y² + {Fmt(d / b)}y) = {Fmt(-e)}");
        sb.AppendLine();

        sb.AppendLine("Paso 3: Completar cuadrados:");
        sb.AppendLine($"  {Fmt(a)}[(x + {Fmt(h)})² - {Fmt(h * h)}] + {Fmt(b)}[(y + {Fmt(k)})² - {Fmt(k * k)}] = {Fmt(-e)}");
        sb.AppendLine();

        sb.AppendLine("Paso 4: Despejar:");
        sb.AppendLine($"  {Fmt(a)}(x + {Fmt(-h)})² + {Fmt(b)}(y + {Fmt(-k)})² = {Fmt(rhs)}");
        sb.AppendLine();

        sb.AppendLine("Paso 5: Dividir para obtener la forma canónica:");
        sb.AppendLine($"  (x + {Fmt(-h)})² / {Fmt(a2)} + (y + {Fmt(-k)})² / {Fmt(b2)} = 1");
        sb.AppendLine();

        sb.AppendLine("Forma canónica:");
        sb.AppendLine($"  (x - {Fmt(h)})² / {Fmt(a2)} + (y - {Fmt(k)})² / {Fmt(b2)} = 1");
    }

    private void AppendEllipseGeneral(StringBuilder sb)
    {
        double a = Equation.A, b = Equation.B, c = Equation.C, d = Equation.D, e = Equation.E;
        double h = -c / (2 * a), k = -d / (2 * b);
        double rhs = -e + a * h * h + b * k * k;
        double a2 = rhs / a, b2 = rhs / b;

        sb.AppendLine("Forma canónica:");
        sb.AppendLine($"  (x - {Fmt(h)})² / {Fmt(a2)} + (y - {Fmt(k)})² / {Fmt(b2)} = 1");
        sb.AppendLine();

        sb.AppendLine("Paso 1: Multiplicar ambos lados por el denominador:");
        sb.AppendLine($"  (x - {Fmt(h)})² * {Fmt(b2)} + (y - {Fmt(k)})² * {Fmt(a2)} = {Fmt(a2 * b2)}");
        sb.AppendLine();

        sb.AppendLine("Paso 2: Dividir y reordenar:");
        sb.AppendLine($"  {Fmt(b2)}(x² - {Fmt(2 * h)}x + {Fmt(h * h)}) + {Fmt(a2)}(y² - {Fmt(2 * k)}y + {Fmt(k * k)}) = {Fmt(a2 * b2)}");
        sb.AppendLine();

        sb.AppendLine("Paso 3: Multiplicar y agrupar:");
        sb.AppendLine($"  {Fmt(b2)}x² + {Fmt(a2)}y² - {Fmt(2 * h * b2)}x - {Fmt(2 * k * a2)}y + ({Fmt(b2 * h * h + a2 * k * k - a2 * b2)}) = 0");
        sb.AppendLine();

        sb.AppendLine("Paso 4: Escalar para obtener los coeficientes originales:");
        double factor = a / b2;
        sb.AppendLine($"  Multiplicar por {Fmt(factor)}:");
        sb.AppendLine();

        sb.AppendLine("Ecuación general obtenida:");
        sb.AppendLine($"  {Equation.EquationString}");
    }

    // ─── HIPÉRBOLA ─────────────────────────────────────────────────

    private void AppendHyperbolaCanonical(StringBuilder sb)
    {
        double a = Equation.A, b = Equation.B, c = Equation.C, d = Equation.D, e = Equation.E;
        double h = -c / (2 * a), k = -d / (2 * b);
        double rhs = -e + a * h * h + b * k * k;

        sb.AppendLine("Paso 1: Agrupar términos en x e y:");
        sb.AppendLine($"  ({Fmt(a)}x² + {Fmt(c)}x) + ({Fmt(b)}y² + {Fmt(d)}y) = {Fmt(-e)}");
        sb.AppendLine();

        sb.AppendLine("Paso 2: Factorizar coeficientes:");
        sb.AppendLine($"  {Fmt(a)}(x² + {Fmt(c / a)}x) + {Fmt(b)}(y² + {Fmt(d / b)}y) = {Fmt(-e)}");
        sb.AppendLine();

        sb.AppendLine("Paso 3: Completar cuadrados:");
        sb.AppendLine($"  {Fmt(a)}[(x + {Fmt(h)})² - {Fmt(h * h)}] + {Fmt(b)}[(y + {Fmt(k)})² - {Fmt(k * k)}] = {Fmt(-e)}");
        sb.AppendLine();

        sb.AppendLine("Paso 4: Despejar:");
        sb.AppendLine($"  {Fmt(a)}(x - {Fmt(h)})² + {Fmt(b)}(y - {Fmt(k)})² = {Fmt(rhs)}");
        sb.AppendLine();

        if (Math.Abs(rhs) < 1e-12)
        {
            sb.AppendLine("El resultado es una hipérbola degenerada (asíntotas).");
            return;
        }

        sb.AppendLine("Paso 5: Dividir para obtener la forma canónica:");
        double a2, b2;
        if (rhs > 0)
        {
            a2 = rhs / a;
            b2 = -rhs / b;
            sb.AppendLine($"  (x - {Fmt(h)})² / {Fmt(a2)} - (y - {Fmt(k)})² / {Fmt(b2)} = 1");
        }
        else
        {
            a2 = -rhs / a;
            b2 = rhs / b;
            sb.AppendLine($"  -(x - {Fmt(h)})² / {Fmt(a2)} + (y - {Fmt(k)})² / {Fmt(b2)} = 1");
        }
        sb.AppendLine();

        sb.AppendLine("Forma canónica:");
        if (rhs > 0)
            sb.AppendLine($"  (x - {Fmt(h)})² / {Fmt(a2)} - (y - {Fmt(k)})² / {Fmt(b2)} = 1");
        else
            sb.AppendLine($"  (y - {Fmt(k)})² / {Fmt(b2)} - (x - {Fmt(h)})² / {Fmt(a2)} = 1");
    }

    private void AppendHyperbolaGeneral(StringBuilder sb)
    {
        double a = Equation.A, b = Equation.B, c = Equation.C, d = Equation.D, e = Equation.E;
        double h = -c / (2 * a), k = -d / (2 * b);
        double rhs = -e + a * h * h + b * k * k;

        if (Math.Abs(rhs) < 1e-12)
        {
            sb.AppendLine("Hipérbola degenerada, no se puede invertir el proceso.");
            return;
        }

        double a2 = rhs > 0 ? rhs / a : -rhs / a;
        double b2 = rhs > 0 ? -rhs / b : rhs / b;

        sb.AppendLine("Forma canónica:");
        if (rhs > 0)
            sb.AppendLine($"  (x - {Fmt(h)})² / {Fmt(a2)} - (y - {Fmt(k)})² / {Fmt(b2)} = 1");
        else
            sb.AppendLine($"  (y - {Fmt(k)})² / {Fmt(b2)} - (x - {Fmt(h)})² / {Fmt(a2)} = 1");
        sb.AppendLine();

        sb.AppendLine("Paso 1: Multiplicar ambos lados por los denominadores:");
        sb.AppendLine("  Desarrollar algebraicamente hasta obtener la ecuación general.");
        sb.AppendLine();

        sb.AppendLine("Ecuación general obtenida:");
        sb.AppendLine($"  {Equation.EquationString}");
    }

    // ─── PARÁBOLA ──────────────────────────────────────────────────

    private void AppendParabolaCanonical(StringBuilder sb)
    {
        double a = Equation.A, b = Equation.B, c = Equation.C, d = Equation.D, e = Equation.E;

        if (b == 0)
        {
            double h = -c / (2 * a);
            double rhsConst = -e + a * h * h;
            double p = -d / (4 * a);
            double k = -rhsConst / d;

            sb.AppendLine("Paso 1: Aislar términos con x:");
            sb.AppendLine($"  {Fmt(a)}x² + {Fmt(c)}x = -{Fmt(d)}y - {Fmt(e)}");
            sb.AppendLine();

            sb.AppendLine("Paso 2: Factorizar A:");
            sb.AppendLine($"  {Fmt(a)}(x² + {Fmt(c / a)}x) = -{Fmt(d)}y - {Fmt(e)}");
            sb.AppendLine();

            sb.AppendLine("Paso 3: Completar cuadrado en x:");
            sb.AppendLine($"  {Fmt(a)}[(x + {Fmt(h)})² - {Fmt(h * h)}] = -{Fmt(d)}y - {Fmt(e)}");
            sb.AppendLine();

            sb.AppendLine("Paso 4: Despejar:");
            sb.AppendLine($"  {Fmt(a)}(x - {Fmt(-h)})² = -{Fmt(d)}(y - {Fmt(k)})");
            sb.AppendLine();

            sb.AppendLine("Paso 5: Identificar parámetros:");

            // Actually let me recalculate more carefully
            // h = -c/(2a), so (x - h)² where h = -c/(2a) means (x + c/(2a))²
            // (x + c/(2a))² = -(d/a)(y + e/d - c²/(4ad))
            // h = -c/(2a), k = -e/d + c²/(4ad), 4p = -(d/a)

            sb.AppendLine($"  h = {Fmt(-c / (2 * a))}");
            sb.AppendLine($"  k = {Fmt(-e / d + c * c / (4 * a * d))}");
            sb.AppendLine($"  4p = {Fmt(-d / a)} → p = {Fmt(-d / (4 * a))}");
            sb.AppendLine();

            sb.AppendLine("Forma canónica:");
            sb.AppendLine($"  (x - {Fmt(h)})² = {Fmt(4 * p)}(y - {Fmt(k)})");
        }
        else
        {
            double k = -d / (2 * b);
            double rhsConst = -e + b * k * k;
            double p = -c / (4 * b);
            double h = -rhsConst / c;

            sb.AppendLine("Paso 1: Aislar términos con y:");
            sb.AppendLine($"  {Fmt(b)}y² + {Fmt(d)}y = -{Fmt(c)}x - {Fmt(e)}");
            sb.AppendLine();

            sb.AppendLine("Paso 2: Factorizar B:");
            sb.AppendLine($"  {Fmt(b)}(y² + {Fmt(d / b)}y) = -{Fmt(c)}x - {Fmt(e)}");
            sb.AppendLine();

            sb.AppendLine("Paso 3: Completar cuadrado en y:");
            sb.AppendLine($"  {Fmt(b)}[(y + {Fmt(k)})² - {Fmt(k * k)}] = -{Fmt(c)}x - {Fmt(e)}");
            sb.AppendLine();

            sb.AppendLine("Paso 4: Despejar:");
            sb.AppendLine($"  {Fmt(b)}(y - {Fmt(-k)})² = -{Fmt(c)}(x - {Fmt(h)})");
            sb.AppendLine();

            sb.AppendLine("Paso 5: Identificar parámetros:");
            sb.AppendLine($"  k = {Fmt(-d / (2 * b))}");
            sb.AppendLine($"  h = {Fmt(-e / c + d * d / (4 * b * c))}");
            sb.AppendLine($"  4p = {Fmt(-c / b)} → p = {Fmt(-c / (4 * b))}");
            sb.AppendLine();

            sb.AppendLine("Forma canónica:");
            sb.AppendLine($"  (y - {Fmt(k)})² = {Fmt(4 * (-c / (4 * b)))}(x - {Fmt(h)})");
        }
    }

    private void AppendParabolaGeneral(StringBuilder sb)
    {
        double a = Equation.A, b = Equation.B, c = Equation.C, d = Equation.D, e = Equation.E;

        sb.AppendLine("Forma canónica (desarrollada desde la general):");
        sb.AppendLine();

        if (b == 0)
        {
            double h = -c / (2 * a);
            double p = -d / (4 * a);
            double k = (-e + a * h * h) / (-d);

            sb.AppendLine($"  (x - {Fmt(h)})² = {Fmt(4 * p)}(y - {Fmt(k)})");
            sb.AppendLine();
            sb.AppendLine("Paso 1: Desarrollar el cuadrado:");
            sb.AppendLine($"  x² - {Fmt(2 * h)}x + {Fmt(h * h)} = {Fmt(4 * p)}y - {Fmt(4 * p * k)}");
            sb.AppendLine();
            sb.AppendLine("Paso 2: Multiplicar por A y reordenar:");
            sb.AppendLine($"  {Fmt(a)}x² + {Fmt(c)}x + {Fmt(d)}y + {Fmt(e)} = 0");
            sb.AppendLine();
            sb.AppendLine("Ecuación general obtenida:");
            sb.AppendLine($"  {Equation.EquationString}");
        }
        else
        {
            double k = -d / (2 * b);
            double p = -c / (4 * b);
            double h = (-e + b * k * k) / (-c);

            sb.AppendLine($"  (y - {Fmt(k)})² = {Fmt(4 * p)}(x - {Fmt(h)})");
            sb.AppendLine();
            sb.AppendLine("Paso 1: Desarrollar el cuadrado:");
            sb.AppendLine($"  y² - {Fmt(2 * k)}y + {Fmt(k * k)} = {Fmt(4 * p)}x - {Fmt(4 * p * h)}");
            sb.AppendLine();
            sb.AppendLine("Paso 2: Multiplicar por B y reordenar:");
            sb.AppendLine($"  {Fmt(b)}y² + {Fmt(d)}y + {Fmt(c)}x + {Fmt(e)} = 0");
            sb.AppendLine();
            sb.AppendLine("Ecuación general obtenida:");
            sb.AppendLine($"  {Equation.EquationString}");
        }
    }
}



