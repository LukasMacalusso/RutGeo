using System.Text;
using RutGeo.Core.Interfaces;

namespace RutGeo.Core.Services;

public class RutValidatorResult
{
    public bool IsValid { get; set; }
    public string RutBody { get; set; } = string.Empty;
    public char Dv { get; set; }
    public string StepByStepLog { get; set; } = string.Empty;
}

public sealed class RutValidator : IRutValidator
{
    private const int MinimumLength = 7;
    private const int BaseMultiplier = 2;
    private const int MaxMultiplier = 7;

    public RutValidatorResult Validate(string rawRut)
    {
        var result = new RutValidatorResult();
        var log = new StringBuilder();

        if (string.IsNullOrWhiteSpace(rawRut))
        {
            log.AppendLine("El RUT está vacío.");
            return BuildFailureResult(result, log);
        }

        string cleanRut = NormalizeRut(rawRut);
        log.AppendLine($"RUT limpio: {cleanRut}");

        if (cleanRut.Length < MinimumLength)
        {
            log.AppendLine("El RUT debe tener al menos 7 caracteres.");
            return BuildFailureResult(result, log);
        }

        string body = cleanRut[..^1];
        char dv = cleanRut[^1];

        result.RutBody = body;
        result.Dv = dv;
        log.AppendLine($"Cuerpo del RUT: {body}");
        log.AppendLine($"DV extraído: {dv}");

        if (!HasOnlyDigits(body, log))
        {
            return BuildFailureResult(result, log);
        }

        int sum = CalculateSum(body, log);
        char expectedDv = CalculateExpectedDv(sum);

        log.AppendLine($"Suma total: {sum}");
        log.AppendLine($"DV esperado: {expectedDv}");

        result.IsValid = expectedDv == dv;
        result.StepByStepLog = log.ToString();
        return result;
    }

    private static string NormalizeRut(string rawRut)
        => rawRut.Replace(".", string.Empty)
                 .Replace("-", string.Empty)
                 .ToUpperInvariant();

    private static bool HasOnlyDigits(string body, StringBuilder log)
    {
        foreach (char character in body)
        {
            if (!char.IsDigit(character))
            {
                log.AppendLine($"Carácter inválido en el cuerpo del RUT: '{character}'.");
                return false;
            }
        }

        return true;
    }

    private static int CalculateSum(string body, StringBuilder log)
    {
        int sum = 0;
        int multiplier = BaseMultiplier;

        for (int index = body.Length - 1; index >= 0; index--)
        {
            int digit = body[index] - '0';
            int product = digit * multiplier;
            sum += product;
            log.AppendLine($"Digito {digit} * multiplicador {multiplier} = {product}");

            multiplier = multiplier == MaxMultiplier ? BaseMultiplier : multiplier + 1;
        }

        return sum;
    }

    private static char CalculateExpectedDv(int sum)
    {
        int remainder = sum % 11;
        int dvValue = 11 - remainder;

        return dvValue switch
        {
            11 => '0',
            10 => 'K',
            _ => dvValue.ToString()[0]
        };
    }

    private static RutValidatorResult BuildFailureResult(RutValidatorResult result, StringBuilder log)
    {
        result.IsValid = false;
        result.StepByStepLog = log.ToString();
        return result;
    }
}