using RutGeo.Core.Models;
using RutGeo.Core.Services;

namespace RutGeo.Core.Models;

public enum ConicType
{
    Parabola,
    Hyperbola,
    Circunferencia,
    Elipse,
    Desconocida
}

public class Conic 
{
    public GeneralEquation GeneralEquation { get; }

public ConicType Type
{
    get
    {
        // Una parábola DEBE tener un término cuadrático y el término lineal opuesto.
        if (GeneralEquation.A != 0 && GeneralEquation.B == 0 && GeneralEquation.D != 0) return ConicType.Parabola;
        if (GeneralEquation.A == 0 && GeneralEquation.B != 0 && GeneralEquation.C != 0) return ConicType.Parabola;

        if (GeneralEquation.A * GeneralEquation.B < 0) return ConicType.Hyperbola;
        if (GeneralEquation.A == GeneralEquation.B && GeneralEquation.A != 0) return ConicType.Circunferencia;
        if (GeneralEquation.A * GeneralEquation.B > 0) return ConicType.Elipse;

        // Si es una parábola sin D o sin C (líneas paralelas), caerá aquí de forma segura.
        return ConicType.Desconocida;
    }
}


    public Conic(GeneralEquation generalEquation)
    {
        GeneralEquation = generalEquation;
    }
}
