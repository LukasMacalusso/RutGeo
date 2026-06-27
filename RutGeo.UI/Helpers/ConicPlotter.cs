using System.Collections.Generic;
using RutGeo.Core.Helpers;
using RutGeo.Core.Models.Equations;

namespace RutGeo.UI.Helpers;

public static class ConicPlotter
{
    public static void PlotCircle(GeneralEquation eq, List<double> xs, List<double> ys)
    {
        if (RutGeoMath.Abs(eq.A) < RutGeoMath.GeometryEpsilon) return;

        double centerX = -eq.C / (2 * eq.A);
        double centerY = -eq.D / (2 * eq.A);
        double radiusSq = centerX * centerX + centerY * centerY - eq.E / eq.A;

        if (radiusSq <= RutGeoMath.GeometryEpsilon) return;

        double radius = RutGeoMath.Sqrt(radiusSq);
        for (double angle = 0; angle <= 2 * RutGeoMath.PI; angle += 0.02)
        {
            xs.Add(centerX + radius * RutGeoMath.Cos(angle));
            ys.Add(centerY + radius * RutGeoMath.Sin(angle));
        }
    }

    public static void PlotEllipse(GeneralEquation eq, List<double> xs, List<double> ys)
    {
        if (RutGeoMath.Abs(eq.A) < RutGeoMath.GeometryEpsilon || RutGeoMath.Abs(eq.B) < RutGeoMath.GeometryEpsilon) return;

        double centerX = -eq.C / (2 * eq.A);
        double centerY = -eq.D / (2 * eq.B);
        double rhs = -eq.E + eq.A * centerX * centerX + eq.B * centerY * centerY;

        if (rhs <= RutGeoMath.GeometryEpsilon) return;

        double semiAxisA = RutGeoMath.Sqrt(rhs / eq.A);
        double semiAxisB = RutGeoMath.Sqrt(rhs / eq.B);

        if (semiAxisA <= RutGeoMath.GeometryEpsilon || semiAxisB <= RutGeoMath.GeometryEpsilon) return;

        for (double angle = 0; angle <= 2 * RutGeoMath.PI; angle += 0.02)
        {
            xs.Add(centerX + semiAxisA * RutGeoMath.Cos(angle));
            ys.Add(centerY + semiAxisB * RutGeoMath.Sin(angle));
        }
    }

    public static void PlotHyperbola(GeneralEquation eq, List<double> xs, List<double> ys)
    {
        if (RutGeoMath.Abs(eq.A) < RutGeoMath.GeometryEpsilon || RutGeoMath.Abs(eq.B) < RutGeoMath.GeometryEpsilon) return;

        double centerX = -eq.C / (2 * eq.A);
        double centerY = -eq.D / (2 * eq.B);
        double rhs = -eq.E + eq.A * centerX * centerX + eq.B * centerY * centerY;

        if (RutGeoMath.Abs(rhs) < RutGeoMath.GeometryEpsilon) return;

        double limitT = 1.56;
        double stepT = 0.01;

        if (rhs > 0)
        {
            double semiAxisA = RutGeoMath.Sqrt(rhs / eq.A);
            double semiAxisB = RutGeoMath.Sqrt(RutGeoMath.Abs(rhs / eq.B));
            for (double t = -limitT; t <= limitT; t += stepT)
            {
                double cosT = RutGeoMath.Cos(t);
                if (RutGeoMath.Abs(cosT) > 0.01)
                {
                    xs.Add(centerX + semiAxisA / cosT);
                    ys.Add(centerY + semiAxisB * RutGeoMath.Tan(t));
                }
            }
            xs.Add(double.NaN);
            ys.Add(double.NaN);
            for (double t = -limitT; t <= limitT; t += stepT)
            {
                double cosT = RutGeoMath.Cos(t);
                if (RutGeoMath.Abs(cosT) > 0.01)
                {
                    xs.Add(centerX - semiAxisA / cosT);
                    ys.Add(centerY + semiAxisB * RutGeoMath.Tan(t));
                }
            }
        }
        else
        {
            double semiAxisA = RutGeoMath.Sqrt(RutGeoMath.Abs(rhs / eq.A));
            double semiAxisB = RutGeoMath.Sqrt(RutGeoMath.Abs(rhs / eq.B));
            for (double t = -limitT; t <= limitT; t += stepT)
            {
                double cosT = RutGeoMath.Cos(t);
                if (RutGeoMath.Abs(cosT) > 0.01)
                {
                    xs.Add(centerX + semiAxisA * RutGeoMath.Tan(t));
                    ys.Add(centerY + semiAxisB / cosT);
                }
            }
            xs.Add(double.NaN);
            ys.Add(double.NaN);
            for (double t = -limitT; t <= limitT; t += stepT)
            {
                double cosT = RutGeoMath.Cos(t);
                if (RutGeoMath.Abs(cosT) > 0.01)
                {
                    xs.Add(centerX + semiAxisA * RutGeoMath.Tan(t));
                    ys.Add(centerY - semiAxisB / cosT);
                }
            }
        }
    }

    public static void PlotParabola(GeneralEquation eq, List<double> xs, List<double> ys)
    {
        double range = 2000;

        if (RutGeoMath.Abs(eq.B) < RutGeoMath.GeometryEpsilon && RutGeoMath.Abs(eq.D) > RutGeoMath.GeometryEpsilon)
        {
            double vertexX = -eq.C / (2 * eq.A);
            for (double x = vertexX - range; x <= vertexX + range; x += 0.1)
            {
                xs.Add(x);
                ys.Add((-eq.A / eq.D) * x * x + (-eq.C / eq.D) * x + (-eq.E / eq.D));
            }
        }
        else if (RutGeoMath.Abs(eq.A) < RutGeoMath.GeometryEpsilon && RutGeoMath.Abs(eq.C) > RutGeoMath.GeometryEpsilon)
        {
            double vertexY = -eq.D / (2 * eq.B);
            for (double y = vertexY - range; y <= vertexY + range; y += 0.1)
            {
                ys.Add(y);
                xs.Add((-eq.B / eq.C) * y * y + (-eq.D / eq.C) * y + (-eq.E / eq.C));
            }
        }
    }
}
