using System.Runtime.Serialization;
using System.Text;

namespace RutGeo.Core.Services
{
    public class RutValidatorResult
    {
        public bool IsValid { get; set; }   //Rut validity
        public string RutBody { get; set; } = string.Empty;
        public char Dv { get; set; } // check digit
    }

    public class RutValidator
    {
        public RutValidatorResult Validate(string rawRut)
        {
            var result = new RutValidatorResult();
            var log = new StringBuilder();

            // Clean Rut
            string clean = rawRut.Replace(".","").Replace("-","").ToUpper();
            
            // Falta agregar el log

            if(clean.Length < 7)
            {
                result.IsValid = false;
                // Mensaje de error
                return result;
            }

            //Divide body and DV
            string body = clean.Substring(0,(clean.Length-1));
            char DV = clean[-1];

            result.RutBody = body;
            result.Dv = DV;

            //Agregar log


            // Module 11
            int sum = 0;
            int multiplier = 2;
            // Secuence 2,3,4,5,6,7,2,3

            for (int i = body.Length -1; i >= 0; i--)
            {
                // Validate numbers into rut body
                if (!char.IsDigit(body[i]))
                {
                    result.IsValid = false;
                    // Mensaje de error
                    return result;
                }

                int digit = int.Parse(body[i].ToString());
                int product = digit * multiplier;
                sum += product;

                multiplier = multiplier == 7 ? 2 : multiplier+1 ;
            }
            
        // Calculate residue and DV expected

        int remainder = sum%11;
        int DvValue = 11 - remainder;

        char DvExpected;
        if (DvValue == 11) DvExpected = '0';
        else if (DvValue == 10) DvExpected = 'K';
        else DvExpected = DvValue.ToString()[0];

        // Implementar validación final

        }

    }
}