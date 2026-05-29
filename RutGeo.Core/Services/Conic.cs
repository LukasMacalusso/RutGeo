using System.Text;
using RutGeo.Core.Interfaces;

namespace RutGeo.Core.Services;

public sealed class Conic : IConic
{
    public RutValidatorResult Rut { get; }
    public Equation Equation { get; }

    public string Type
    {
        get
        {
            if ((Equation.A == 0) ^ (Equation.B == 0)) return "Parábola";
            if (Equation.A * Equation.B < 0) return "Hipérbola";
            if (Equation.A == Equation.B && Equation.A != 0) return "Circunferencia";
            if (Equation.A * Equation.B > 0) return "Elipse";
            return string.Empty;
        }
    }
    public Conic(RutValidatorResult rut, Equation equation)
    {
        Rut = rut;
        Equation = equation;
    }
}