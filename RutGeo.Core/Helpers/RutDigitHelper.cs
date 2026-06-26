namespace RutGeo.Core.Helpers;

public static class RutDigitHelper
{
    public static int[] GetPaddedDigits(string body)
    {
        string paddedBody = body.PadLeft(8, '0');
        int[] digits = new int[paddedBody.Length];
        for (int i = 0; i < paddedBody.Length; i++)
            digits[i] = paddedBody[i] - '0';
        return digits;
    }

    public static int GetLimitCondition(int d8) => d8 % 3;
}
