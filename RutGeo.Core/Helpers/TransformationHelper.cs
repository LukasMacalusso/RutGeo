namespace RutGeo.Core.Helpers;

public static class TransformationHelper
{
    public static string FormatNumber(double value)
    {
        return value.ToString("0.##");
    }

    public static string FormatBracket(char variable, double value)
    {
        if (value == 0)
            return variable.ToString();
        if (value > 0)
            return $"({variable} - {FormatNumber(value)})";
        return $"({variable} + {FormatNumber(-value)})";
    }
}
