using RutGeo.Core.Helpers;
using RutGeo.Core.Models.Types;
using RutGeo.Core.Models.Equations;

namespace RutGeo.Core.Models;

public class Conic 
{
    public GeneralEquation GeneralEquation { get; }

    public ConicType Type
    {
        get
        {
            if (!RutGeoMath.IsNearZero(GeneralEquation.A) && RutGeoMath.IsNearZero(GeneralEquation.B) && !RutGeoMath.IsNearZero(GeneralEquation.D)) return ConicType.Parabola;
            if (RutGeoMath.IsNearZero(GeneralEquation.A) && !RutGeoMath.IsNearZero(GeneralEquation.B) && !RutGeoMath.IsNearZero(GeneralEquation.C)) return ConicType.Parabola;

            if (GeneralEquation.A * GeneralEquation.B < 0) return ConicType.Hyperbola;
            if (RutGeoMath.IsNearZero(GeneralEquation.A - GeneralEquation.B) && !RutGeoMath.IsNearZero(GeneralEquation.A)) return ConicType.Circunferencia;
            if (GeneralEquation.A * GeneralEquation.B > 0) return ConicType.Elipse;

            return ConicType.Desconocida;
        }
    }


    public Conic(GeneralEquation generalEquation)
    {
        GeneralEquation = generalEquation;
    }
}
