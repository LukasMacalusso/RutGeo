using RutGeo.Core.Interfaces;

namespace RutGeo.Core.Services;

public class RutValidatorResult
{
    public bool IsValid { get; init; }
    public string RutBody { get; init; } = string.Empty;
    public char Dv { get; init; }
}

public class RutValidator : IRutValidator
{
    private const int MinimumLength = 7;
    private const int BaseMultiplier = 2;
    private const int MaxMultiplier = 7;

    public RutValidatorResult Validate(string rawRut, IExplanationLog log)
    {
        log.StartProcess("Validación de RUT");

        if (string.IsNullOrWhiteSpace(rawRut))
        {
            log.AppendStep("El RUT está vacío.");
            return CreateFailureResult();
        }

        string cleanRut = NormalizeRut(rawRut);
        log.AppendStep($"RUT limpio: {cleanRut}");

        if (cleanRut.Length < MinimumLength)
        {
            log.AppendStep("El RUT debe tener al menos 7 caracteres.");
            return CreateFailureResult();
        }

        string body = cleanRut[..^1];
        char dv = cleanRut[^1];
        
        log.AppendStep($"Cuerpo del RUT: {body}");
        log.AppendStep($"DV extraído: {dv}");

        if (!HasOnlyDigits(body, log))
        {
            return CreateFailureResult();
        }

        int sum = CalculateModulo11Sum(body, log);
        char expectedDv = CalculateExpectedDv(sum);

        log.AppendStep($"Suma total: {sum}");
        log.AppendStep($"DV esperado: {expectedDv}");

        return new RutValidatorResult
        {
            IsValid = expectedDv == dv,
            RutBody = body,
            Dv = dv
        };
    }

    private static string NormalizeRut(string rawRut)
        => rawRut.Replace(".", string.Empty)
                 .Replace("-", string.Empty)
                 .ToUpperInvariant();

    private static bool HasOnlyDigits(string body, IExplanationLog log)
    {
        foreach (char character in body)
        {
            if (!char.IsDigit(character))
            {
                log.AppendStep($"Carácter inválido en el cuerpo del RUT: '{character}'.");
                return false;
            }
        }

        return true;
    }

    private static int CalculateModulo11Sum(string body, IExplanationLog log)
    {
        int sum = 0;
        int multiplier = BaseMultiplier;

        for (int index = body.Length - 1; index >= 0; index--)
        {
            int digit = body[index] - '0';
            int product = digit * multiplier;
            sum += product;
            log.AppendStep($"Digito {digit} * multiplicador {multiplier} = {product}");

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

    private static RutValidatorResult CreateFailureResult()
    {
        return new RutValidatorResult
        {
            IsValid = false
        };
    }
}
