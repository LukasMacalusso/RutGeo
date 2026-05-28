using System.Text;
namespace RutGeo.Core.Services
{
    public class Equation()
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }
        public double D { get; set; }
        public double E { get; set; }
        public string EquationString = $"NoImplementedYet";

        public static Equation Generate(RutValidatorResult rut)
        {

            var Idk = new Equation();

            int v = (int)char.GetNumericValue(rut.Dv);
            int d1 = (int)char.GetNumericValue(rut.RutBody[0]);
            int d2 = (int)char.GetNumericValue(rut.RutBody[1]);
            int d3 = (int)char.GetNumericValue(rut.RutBody[2]);
            int d4 = (int)char.GetNumericValue(rut.RutBody[3]);
            int d5 = (int)char.GetNumericValue(rut.RutBody[4]);
            int d6 = (int)char.GetNumericValue(rut.RutBody[5]);
            int d7 = (int)char.GetNumericValue(rut.RutBody[6]);
            int d8 = (int)char.GetNumericValue(rut.RutBody[7]);

            Idk.A = (d1+d2)/v;
            Idk.B = (d3+d4)/v;
            Idk.C = -(d5+d6);
            Idk.D = -(d7+d8);
            Idk.E = d1+d3+d5+d7;

            return Idk;
        }

    }
    
}

