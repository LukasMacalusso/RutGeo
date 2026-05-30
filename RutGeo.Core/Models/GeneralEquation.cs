namespace RutGeo.Core.Services;
public class GeneralEquation
{
    public double A { get; set; }
    public double B { get; set; }
    public double C { get; set; }
    public double D { get; set; }
    public double E { get; set; }
    public string EquationString
    {
        get
        {
            var terms = new List<string>();
            AddTerm(terms, A, "x²");
            AddTerm(terms, B, "y²");
            AddTerm(terms, C, "x");
            AddTerm(terms, D, "y");
            AddTerm(terms, E, null);

            string result = terms.Count == 0 ? "0" : string.Concat(terms);
            if (result.StartsWith(" + "))
                result = result[3..];
            return result + " = 0";
        }
    }

    private static void AddTerm(List<string> terms, double coefficient, string? variable)
    {
        if (coefficient == 0) return;
        string sign = coefficient < 0 ? " - " : " + ";
        double abs = Math.Abs(coefficient);
        string value = abs == (int)abs ? ((int)abs).ToString() : abs.ToString();
        string term = variable is null ? value : (value == "1" ? variable : value + variable);
        terms.Add(sign + term);
    }
}
