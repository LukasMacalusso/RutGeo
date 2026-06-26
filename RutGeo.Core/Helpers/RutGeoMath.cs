namespace RutGeo.Core.Helpers;

public static class RutGeoMath
{
    public const double PI = 3.14159265358979323846;
    public const double Epsilon = 1e-15;
    public const double NearZeroThreshold = 1e-12;
    public const double GeometryEpsilon = 1e-9;
    public const double InfinityThreshold = 1e15;
    private const int TaylorTerms = 10;
    private const int NewtonIterations = 10;

    public static double Abs(double x)
    {
        return x < 0 ? -x : x;
    }

    public static double Sqrt(double x)
    {
        if (x < 0) return double.NaN;
        if (x < Epsilon) return 0;

        double guess = x;
        for (int i = 0; i < NewtonIterations; i++)
            guess = (guess + x / guess) / 2;
        return guess;
    }

    public static double Sin(double x)
    {
        x = ReduceAngle(x);
        double term = x;
        double sum = x;
        for (int n = 1; n < TaylorTerms; n++)
        {
            term *= -x * x / ((2 * n) * (2 * n + 1));
            sum += term;
        }
        return sum;
    }

    public static double Cos(double x)
    {
        x = ReduceAngle(x);
        double term = 1;
        double sum = 1;
        for (int n = 1; n < TaylorTerms; n++)
        {
            term *= -x * x / ((2 * n - 1) * (2 * n));
            sum += term;
        }
        return sum;
    }

    public static double Tan(double x)
    {
        double s = Sin(x);
        double c = Cos(x);
        if (IsNearZero(c)) return double.PositiveInfinity;
        return s / c;
    }

    public static bool IsInfinity(double x)
    {
        return Abs(x) > InfinityThreshold;
    }

    public static bool IsNaN(double x)
    {
        return x.CompareTo(x) != 0;
    }

    public static double ReduceAngle(double x)
    {
        double twoPi = 2 * PI;
        x -= (int)(x / twoPi) * twoPi;
        if (x > PI) x -= twoPi;
        else if (x < -PI) x += twoPi;
        return x;
    }

    public static bool IsNearZero(double x)
    {
        return Abs(x) < NearZeroThreshold;
    }
}
