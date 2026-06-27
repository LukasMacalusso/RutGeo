using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace RutGeo.Core.Helpers;

public static class ValueComparer
{
    public static bool AreEqual(string user, string expected, double tolerance = 0.3)
    {
        if (string.IsNullOrWhiteSpace(user))
            return false;

        double[] userNums = ExtractNumbers(user);
        double[] expNums = ExtractNumbers(expected);

        if (userNums.Length > 0 && expNums.Length > 0)
        {
            int minLen = Math.Min(userNums.Length, expNums.Length);
            for (int i = 0; i < minLen; i++)
            {
                if (RutGeoMath.Abs(userNums[i] - expNums[i]) > tolerance)
                    return false;
            }
            return true;
        }

        return Normalize(user) == Normalize(expected);
    }

    private static double[] ExtractNumbers(string s)
    {
        string normalized = s.Trim().Replace(',', '.');
        var matches = Regex.Matches(normalized, @"-?\d+\.?\d*");
        var result = new List<double>(matches.Count);
        foreach (Match match in matches)
        {
            if (double.TryParse(match.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
                result.Add(val);
        }
        return result.ToArray();
    }

    private static string Normalize(string s)
    {
        return s.Trim().ToLowerInvariant().Replace(" ", "");
    }
}
