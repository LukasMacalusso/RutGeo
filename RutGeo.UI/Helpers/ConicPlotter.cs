using System.Collections.Generic;
using RutGeo.Core.Helpers;
using RutGeo.Core.Models.Equations;

namespace RutGeo.UI.Helpers;

public static class ConicPlotter
{
    public static void PlotCircle(GeneralEquation eq, List<double> xs, List<double> ys)
    {
        if (RutGeoMath.Abs(eq.A) < RutGeoMath.GeometryEpsilon) return;

        double h = -eq.C / (2 * eq.A);
        double k = -eq.D / (2 * eq.A);
        double r2 = h * h + k * k - eq.E / eq.A;

        if (r2 <= RutGeoMath.GeometryEpsilon) return;

        double r = RutGeoMath.Sqrt(r2);
        for (double t = 0; t <= 2 * RutGeoMath.PI; t += 0.02)
        {
            xs.Add(h + r * RutGeoMath.Cos(t));
            ys.Add(k + r * RutGeoMath.Sin(t));
        }
    }

    public static void PlotEllipse(GeneralEquation eq, List<double> xs, List<double> ys)
    {
        if (RutGeoMath.Abs(eq.A) < RutGeoMath.GeometryEpsilon || RutGeoMath.Abs(eq.B) < RutGeoMath.GeometryEpsilon) return;

        double eh = -eq.C / (2 * eq.A);
        double ek = -eq.D / (2 * eq.B);
        double rhsE = -eq.E + eq.A * eh * eh + eq.B * ek * ek;

        if (rhsE <= RutGeoMath.GeometryEpsilon) return;

        double ea = RutGeoMath.Sqrt(rhsE / eq.A);
        double eb = RutGeoMath.Sqrt(rhsE / eq.B);

        if (ea <= RutGeoMath.GeometryEpsilon || eb <= RutGeoMath.GeometryEpsilon) return;

        for (double t = 0; t <= 2 * RutGeoMath.PI; t += 0.02)
        {
            xs.Add(eh + ea * RutGeoMath.Cos(t));
            ys.Add(ek + eb * RutGeoMath.Sin(t));
        }
    }

    public static void PlotHyperbola(GeneralEquation eq, List<double> xs, List<double> ys)
    {
        if (RutGeoMath.Abs(eq.A) < RutGeoMath.GeometryEpsilon || RutGeoMath.Abs(eq.B) < RutGeoMath.GeometryEpsilon) return;

        double hh = -eq.C / (2 * eq.A);
        double hk = -eq.D / (2 * eq.B);
        double rhsH = -eq.E + eq.A * hh * hh + eq.B * hk * hk;

        if (RutGeoMath.Abs(rhsH) < RutGeoMath.GeometryEpsilon) return;

        if (rhsH > 0)
        {
            double ah = RutGeoMath.Sqrt(rhsH / eq.A);
            double bh = RutGeoMath.Sqrt(RutGeoMath.Abs(rhsH / eq.B));
            for (double t = -1.5; t <= 1.5; t += 0.03)
            {
                double cosT = RutGeoMath.Cos(t);
                if (RutGeoMath.Abs(cosT) > 0.01)
                {
                    xs.Add(hh + ah / cosT);
                    ys.Add(hk + bh * RutGeoMath.Tan(t));
                }
            }
            xs.Add(double.NaN);
            ys.Add(double.NaN);
            for (double t = -1.5; t <= 1.5; t += 0.03)
            {
                double cosT = RutGeoMath.Cos(t);
                if (RutGeoMath.Abs(cosT) > 0.01)
                {
                    xs.Add(hh - ah / cosT);
                    ys.Add(hk + bh * RutGeoMath.Tan(t));
                }
            }
        }
        else
        {
            double ah = RutGeoMath.Sqrt(RutGeoMath.Abs(rhsH / eq.A));
            double bh = RutGeoMath.Sqrt(RutGeoMath.Abs(rhsH / eq.B));
            for (double t = -1.5; t <= 1.5; t += 0.03)
            {
                double cosT = RutGeoMath.Cos(t);
                if (RutGeoMath.Abs(cosT) > 0.01)
                {
                    xs.Add(hh + ah * RutGeoMath.Tan(t));
                    ys.Add(hk + bh / cosT);
                }
            }
            xs.Add(double.NaN);
            ys.Add(double.NaN);
            for (double t = -1.5; t <= 1.5; t += 0.03)
            {
                double cosT = RutGeoMath.Cos(t);
                if (RutGeoMath.Abs(cosT) > 0.01)
                {
                    xs.Add(hh + ah * RutGeoMath.Tan(t));
                    ys.Add(hk - bh / cosT);
                }
            }
        }
    }

    public static void PlotParabola(GeneralEquation eq, List<double> xs, List<double> ys)
    {
        if (RutGeoMath.Abs(eq.B) < RutGeoMath.GeometryEpsilon && RutGeoMath.Abs(eq.D) > RutGeoMath.GeometryEpsilon)
        {
            for (double x = -10; x <= 10; x += 0.1)
            {
                xs.Add(x);
                ys.Add((-eq.A / eq.D) * x * x + (-eq.C / eq.D) * x + (-eq.E / eq.D));
            }
        }
        else if (RutGeoMath.Abs(eq.A) < RutGeoMath.GeometryEpsilon && RutGeoMath.Abs(eq.C) > RutGeoMath.GeometryEpsilon)
        {
            for (double y = -10; y <= 10; y += 0.1)
            {
                ys.Add(y);
                xs.Add((-eq.B / eq.C) * y * y + (-eq.D / eq.C) * y + (-eq.E / eq.C));
            }
        }
    }
}
