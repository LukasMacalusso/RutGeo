using System;
using System.Collections.Generic;
using RutGeo.Core.Interfaces;

namespace RutGeo.Core.Services;

public class Equation
{
    public double A { get; init; }
    public double B { get; init; }
    public double C { get; init; }
    public double D { get; init; }
    public double E { get; init; }

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

public class EquationGenerator : IEquationGenerator
{
    public Equation Generate(RutValidatorResult rut)
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

        return new Equation
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

    private static (double a, double b) AdjustForDivisibleByThreeSum(double a, double b, int fifthDigit, int sixthDigit, int seventhDigit)
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
