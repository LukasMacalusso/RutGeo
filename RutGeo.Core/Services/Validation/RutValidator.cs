using RutGeo.Core.Interfaces.Validation;
using RutGeo.Core.Interfaces.Common;
using RutGeo.Core.Models.Results;

namespace RutGeo.Core.Services.Validation;

public class RutValidator : IRutValidator
{
    private readonly IExplanationLogger _log;

    public RutValidator(IExplanationLogger log)
    {
        _log = log;
    }

    private const int MinimumLength = 7;
    private const int BaseMultiplier = 2;
    private const int MaxMultiplier = 7;

    public RutValidatorResult Validate(string rawRut)
    {
        _log.StartProcess("Validación de RUT");

        if (!IsInputValid(rawRut)) return CreateFailureResult();

        string cleanRut = GetNormalizedRut(rawRut);

        if (!HasValidLength(cleanRut)) return CreateFailureResult();

        var (body, dv) = ExtractBodyAndDv(cleanRut);

        if (!HasOnlyDigits(body)) return CreateFailureResult();

        return VerifyDv(body, dv);
    }


    private bool IsInputValid(string rawRut)
    {
        if (string.IsNullOrWhiteSpace(rawRut))
        {
            _log.AppendStep("El RUT está vacío.");
            return false;
        }
        return true;
    }


    private string GetNormalizedRut(string rawRut)
    {
        string cleanRut = NormalizeRut(rawRut);
        _log.AppendStep($"RUT limpio: {cleanRut}");
        return cleanRut;
    }
    private static string NormalizeRut(string rawRut)
        => rawRut.Replace(".", string.Empty)
                 .Replace("-", string.Empty)
                 .ToUpperInvariant();
    private bool HasValidLength(string cleanRut)
    {
        if (cleanRut.Length < MinimumLength)
        {
            _log.AppendStep("El RUT debe tener al menos 7 caracteres.");
            return false;
        }
        return true;
    }


    private (string body, char dv) ExtractBodyAndDv(string cleanRut)
    {
        string body = cleanRut[..^1];
        char dv = cleanRut[^1];

        _log.AppendStep($"Cuerpo del RUT: {body}");
        _log.AppendStep($"DV extraído: {dv}");

        return (body, dv);
    }
    private bool HasOnlyDigits(string body)
    {
        foreach (char character in body)
        {
            if (!char.IsDigit(character))
            {
                _log.AppendStep($"Carácter inválido en el cuerpo del RUT: '{character}'.");
                return false;
            }
        }

        return true;
    }


    private RutValidatorResult VerifyDv(string body, char dv)
    {
        int sum = CalculateModulo11Sum(body);
        char expectedDv = CalculateExpectedDv(sum);

        _log.AppendStep($"Suma total: {sum}");
        _log.AppendStep($"DV esperado: {expectedDv}");

        return new RutValidatorResult
        {
            IsValid = expectedDv == dv,
            RutBody = body,
            Dv = dv
        };
    }
    private int CalculateModulo11Sum(string body)
    {
        int sum = 0;
        int multiplier = BaseMultiplier;

        for (int index = body.Length - 1; index >= 0; index--)
        {
            int digit = body[index] - '0';
            int product = digit * multiplier;
            sum += product;
            _log.AppendStep($"Digito {digit} * multiplicador {multiplier} = {product}");

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
