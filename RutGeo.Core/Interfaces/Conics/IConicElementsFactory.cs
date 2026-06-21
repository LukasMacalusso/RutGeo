using RutGeo.Core.Models;

namespace RutGeo.Core.Interfaces.Conics;

public interface IConicElementsFactory
{
    CircleElements CreateCircleElements(double h, double k, double r2);
    EllipseElements CreateEllipseElements(double h, double k, double a2, double b2);
    HyperbolaElements CreateHyperbolaElements(double h, double k, double a2, double b2, bool isHorizontal);
    ParabolaElements CreateParabolaElements(double h, double k, double p, bool isXSquared);
}
