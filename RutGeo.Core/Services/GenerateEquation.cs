using RutGeo.Core.Interfaces;

namespace RutGeo.Core.Services
{
    public class Equation
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

    public class EquationGenerator : IGenerateEquation
    {
        public Equation Generate(RutValidatorResult rut)
        {
            var equation = new Equation();

            int v;
            if (rut.Dv == '0')
                v = 11;
            else if (rut.Dv == 'K')
                v = 10;
            else
                v = (int)char.GetNumericValue(rut.Dv);

            string body = rut.RutBody.PadLeft(8, '0');
            int[] d = new int[body.Length];
            for (int i = 0; i < body.Length; i++)
                d[i] = body[i] - '0';

            equation.A = (double)(d[0] + d[1]) / v;
            equation.B = (double)(d[2] + d[3]) / v;
            equation.C = -(d[4] + d[5]);
            equation.D = -(d[6] + d[7]);
            equation.E = d[0] + d[2] + d[4] + d[6];

            if (d[7] % 2 != 0)
                equation.B = -equation.B;

            if (d[0] == d[1])
                equation.B = equation.A;

            if ((d[4] + d[5]) % 3 == 0)
                if (d[6] % 2 != 0)
                    equation.A = 0;
                else
                    equation.B = 0;

            return equation;
        }
    }
}

