using System;
using System.Globalization;

namespace RutGeo.Core.Helpers;

public static class ValueComparer
{
    public static bool AreEqual(string user, string expected, double tolerance = 0.5)
    {
        if (string.IsNullOrWhiteSpace(user))
            return false;

        if (TryParseDouble(user, out double userVal) && TryParseDouble(expected, out double expVal))
            return Math.Abs(userVal - expVal) <= tolerance;

        return Normalize(user) == Normalize(expected);
    }

    private static bool TryParseDouble(string s, out double val)
    {
        return double.TryParse(
            s.Trim().Replace(',', '.'),
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out val);
    }

    private static string Normalize(string s)
    {
        return s.Trim().ToLowerInvariant().Replace(" ", "");
    }
}
