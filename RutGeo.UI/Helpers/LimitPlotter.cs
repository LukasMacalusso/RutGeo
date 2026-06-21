using System.Collections.Generic;
using RutGeo.Core.Helpers;

namespace RutGeo.UI.Helpers;

public static class LimitPlotter
{
    public static void PlotLimitFunction(int condition, int criticalPoint, int[] digits, List<double> xs, List<double> ys)
    {
        double range = 5;
        double step = 0.02;

        if (condition == 0)
        {
            for (double x = criticalPoint - range; x <= criticalPoint + range; x += step)
            {
                if (RutGeoMath.Abs(x - criticalPoint) > 0.001)
                {
                    xs.Add(x);
                    ys.Add(x + digits[0]);
                }
            }
        }
        else if (condition == 1)
        {
            for (double x = criticalPoint - range; x < criticalPoint; x += step)
            {
                xs.Add(x);
                ys.Add(x + digits[1]);
            }
            for (double x = criticalPoint; x <= criticalPoint + range; x += step)
            {
                xs.Add(x);
                ys.Add(x + digits[3]);
            }
        }
        else
        {
            double numer = digits[4] + 1.0;
            for (double x = criticalPoint - range; x < criticalPoint - 0.01; x += step)
            {
                xs.Add(x);
                ys.Add(numer / (x - criticalPoint));
            }
            for (double x = criticalPoint + 0.01; x <= criticalPoint + range; x += step)
            {
                xs.Add(x);
                ys.Add(numer / (x - criticalPoint));
            }
        }
    }
}
