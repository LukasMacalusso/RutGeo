namespace RutGeo.Core.Helpers;

public static class SignCleaner
{
    public static string Clean(string input)
    {
        return input
            .Replace("+ -", "- ")
            .Replace("- -", "+ ")
            .Replace("+ +", "+ ")
            .Replace("- +", "- ");
    }
}
