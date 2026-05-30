using RutGeo.Core.Interfaces;

namespace RutGeo.Core.Services
{
    public class RutEquationGenerator : IRutEquationGenerator
    {
        public GeneralEquation GenerateGeneralEquation(RutValidatorResult rut)
        {
            int validationModulo = GetValidationModulo(rut.Dv);
            int[] digits = GetPaddedDigits(rut.RutBody);

            double a = CalculateBaseCoefficient(digits[0], digits[1], validationModulo);
            double b = CalculateBaseCoefficient(digits[2], digits[3], validationModulo);
            double c = -(digits[4] + digits[5]);
            double d = -(digits[6] + digits[7]);
            double e = digits[0] + digits[2] + digits[4] + digits[6];

            b = AdjustBForOddEighthDigit(b, digits[7]);
            b = AdjustBForEqualFirstDigits(a, b, digits[0], digits[1]);

            (a, b) = AdjustForDivisibleByThreeSum(a, b, digits[4], digits[5], digits[6]);

            return new GeneralEquation
            {
                A = a,
                B = b,
                C = c,
                D = d,
                E = e
            };
        }

        private static int GetValidationModulo(char dv)
        {
            return dv switch
            {
                '0' => 11,
                'K' => 10,
                _ => (int)char.GetNumericValue(dv)
            };
        }

        private static int[] GetPaddedDigits(string body)
        {
            string paddedBody = body.PadLeft(8, '0');
            int[] digits = new int[paddedBody.Length];

            for (int i = 0; i < paddedBody.Length; i++)
            {
                digits[i] = paddedBody[i] - '0';
            }

            return digits;
        }

        private static double CalculateBaseCoefficient(int digitOne, int digitTwo, int validationModulo)
        {
            return (double)(digitOne + digitTwo) / validationModulo;
        }

        private static double AdjustBForOddEighthDigit(double b, int eighthDigit)
        {
            return eighthDigit % 2 != 0 ? -b : b;
        }

        private static double AdjustBForEqualFirstDigits(double a, double b, int firstDigit, int secondDigit)
        {
            return firstDigit == secondDigit ? a : b;
        }

        private static (double a, double b) AdjustForDivisibleByThreeSum(double a, double b, int fifthDigit,
            int sixthDigit, int seventhDigit)
        {
            if ((fifthDigit + sixthDigit) % 3 == 0)
            {
                if (seventhDigit % 2 != 0)
                    return (0, b);

                return (a, 0);
            }

            return (a, b);
        }
    }
}
