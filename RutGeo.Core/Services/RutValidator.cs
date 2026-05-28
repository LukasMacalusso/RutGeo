using System.Text;

namespace RutGeo.Core.Services
{
    public class RutValidatorResult
    {
        public bool IsValid { get; set; }   //Rut validity
        public string RutBody { get; set; } = string.Empty;
        public char Dv { get; set; } // check digit
        public string StepByStepLog { get; set; } = string.Empty;
    }

    public class RutValidator
    {
        public RutValidatorResult Validate(string rawRut)
        {
            var result = new RutValidatorResult();
            var log = new StringBuilder();

            if (string.IsNullOrWhiteSpace(rawRut))
            {
                log.AppendLine("El RUT está vacío.");
                result.StepByStepLog = log.ToString();
                result.IsValid = false;
                return result;
            }

            // Clean Rut
            string clean = rawRut.Replace(".", string.Empty).Replace("-", string.Empty).ToUpperInvariant();
            log.AppendLine($"RUT limpio: {clean}");

            if (clean.Length < 7)
            {
                log.AppendLine("El RUT debe tener al menos 7 caracteres.");
                result.StepByStepLog = log.ToString();
                result.IsValid = false;
                return result;
            }

            // Divide body and DV
            string body = clean[..^1];
            char DV = clean[^1];

            result.RutBody = body;
            result.Dv = DV;
            log.AppendLine($"Cuerpo del RUT: {body}");
            log.AppendLine($"DV extraído: {DV}");


            // Module 11
            int sum = 0;
            int multiplier = 2;
            // Secuence 2,3,4,5,6,7,2,3

            for (int i = body.Length -1; i >= 0; i--)
            {
                // Validate numbers into rut body
                if (!char.IsDigit(body[i]))
                {
                    log.AppendLine($"Carácter inválido en el cuerpo del RUT: '{body[i]}'.");
                    result.StepByStepLog = log.ToString();
                    result.IsValid = false;
                    return result;
                }

                int digit = int.Parse(body[i].ToString());
                int product = digit * multiplier;
                sum += product;
                log.AppendLine($"Digito {digit} * multiplicador {multiplier} = {product}");

                multiplier = multiplier == 7 ? 2 : multiplier + 1;
            }
            
        // Calculate residue and DV expected

        int remainder = sum % 11;
        int DvValue = 11 - remainder;

        char DvExpected;
        if (DvValue == 11) DvExpected = '0';
        else if (DvValue == 10) DvExpected = 'K';
        else DvExpected = DvValue.ToString()[0];

        log.AppendLine($"Suma total: {sum}");
        log.AppendLine($"Resto módulo 11: {remainder}");
        log.AppendLine($"DV esperado: {DvExpected}");

        // Final validation

        result.IsValid = (DvExpected == DV);
        result.StepByStepLog = log.ToString();
        return result;

        }

    }
}