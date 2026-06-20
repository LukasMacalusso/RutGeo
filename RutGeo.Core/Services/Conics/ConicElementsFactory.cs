using System;
using System.Collections.Generic;
using RutGeo.Core.Interfaces.Conics;
using RutGeo.Core.Models;

namespace RutGeo.Core.Services.Conics;

public class ConicElementsFactory : IConicElementsFactory
{
    public CircleElements CreateCircleElements(double h, double k, double r2)
    {
        return new CircleElements
        {
            Center = new Point2D(h, k),
            Radius = Math.Sqrt(r2)
        };
    }

    public EllipseElements CreateEllipseElements(double h, double k, double a2, double b2)
    {
        double a = Math.Sqrt(a2);
        double b = Math.Sqrt(b2);
        bool isHorizontal = a2 >= b2;
        double c2 = Math.Abs(a2 - b2);
        double c = Math.Sqrt(c2);

        return new EllipseElements
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
    }

    public HyperbolaElements CreateHyperbolaElements(double h, double k, double a2, double b2, bool isHorizontal)
    {
        double a = Math.Sqrt(a2);
        double b = Math.Sqrt(b2);
        double c2 = a2 + b2;
        double c = Math.Sqrt(c2);

        return new HyperbolaElements
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
    }

    public ParabolaElements CreateParabolaElements(double h, double k, double p, bool isXSquared)
    {
        if (isXSquared)
        {
            return new ParabolaElements
            {
                Center = new Point2D(h, k),
                Vertex = new Point2D(h, k),
                Focus = new Point2D(h, k + p),
                Directrix = new Line2D(0, 1, -(k - p)), // y - (k-p) = 0
                AxisOfSymmetry = new Line2D(1, 0, -h),  // x - h = 0
                FocalDistance = p
            };
        }
        else
        {
            return new ParabolaElements
            {
                Center = new Point2D(h, k),
                Vertex = new Point2D(h, k),
                Focus = new Point2D(h + p, k),
                Directrix = new Line2D(1, 0, -(h - p)), // x - (h-p) = 0
                AxisOfSymmetry = new Line2D(0, 1, -k),  // y - k = 0
                FocalDistance = p
            };
        }
    }
}
