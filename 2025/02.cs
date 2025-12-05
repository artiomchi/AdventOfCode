#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;

#region Asserts

static void AssertNumber(int number, bool expected, bool part2 = false)
{
    var result = IsValidNumber1(number);
    var part1Expected = part2 && !expected ? true : expected;
    if (result != part1Expected)
    {
        throw new Exception($"Assertion failed for number {number} (first part). Expected valid={part1Expected}.");
    }

    var result2 = IsValidNumber2(number);
    if (result2 != expected)
    {
        throw new Exception($"Assertion failed for number {number} (second part). Expected valid={expected}.");
    }
}

AssertNumber(11, false, false);
AssertNumber(12, true, false);
AssertNumber(22, false);
AssertNumber(111, false, true);
AssertNumber(1010, false);
AssertNumber(222222, false);
AssertNumber(2222222, false, true);
AssertNumber(1188511885, false);
AssertNumber(118118, false);
AssertNumber(446446, false);
AssertNumber(38593859, false);
AssertNumber(385938597, true);
AssertNumber(118118119, true);
AssertNumber(118118118, false, true);

#endregion

var input = FileHelpers.ReadInputText("02.real.txt")
    .Split(['\n', '\r', ','], StringSplitOptions.RemoveEmptyEntries)
    .ToArray();

long result1 = 0, result2 = 0;

foreach (var range in input)
{
    var (start, end) = range.Split('-') switch
    {
        var parts when parts.Length == 2 => (long.Parse(parts[0]), long.Parse(parts[1])),
        _ => throw new Exception($"Invalid range: {range}")
    };

    for (long i = start; i <= end; i++)
    {
        if (!IsValidNumber1(i))
        {
            result1 += i;
        }
        if (!IsValidNumber2(i))
        {
            result2 += i;
        }
    }

}

Console.WriteLine($"Result: {result1}");
Console.WriteLine($"Result: {result2}");

static bool IsValidNumber1(long number)
{
    var digits = number.ToString();
    if (digits.Length % 2 != 0)
    {
        return true;
    }

    var half = digits.Length / 2;
    return digits[0..half] != digits[half..];
}

static bool IsValidNumber2(long number)
{
    var digits = number.ToString();
    for (int i = 1; i <= digits.Length / 2; i++)
    {
        if (digits.Length % i > 0)
        {
            continue; // Cannot form pairs of this size
        }

        var sequenceFound = true;
        for (int j = 1; j < digits.Length / i; j++)
        {
            if (digits[0..i] != digits[(j * i)..((j + 1) * i)])
            {
                sequenceFound = false;
                break;
            }
        }

        if (sequenceFound)
        {
            return false;
        }
    }

    return true;
}
