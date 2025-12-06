#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var lines = FileHelpers.ReadInputLines("02.txt");
var sw = Stopwatch.StartNew();
var input = lines
    .Select(line => line.Trim()
        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Select(int.Parse)
        .ToArray())
    .ToArray();
var parseTime = sw.Elapsed;

// Part 1
sw.Restart();
input.Count(v => ValidateLevel(v, false)).DumpAndAssert("Part 1", 2, 534);
var part1Time = sw.Elapsed;

// Part 2
sw.Restart();
input.Count(v => ValidateLevel(v, true)).DumpAndAssert("Part 2", 4, 577);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);

// Helpers
static bool ValidateLevel(int[] values, bool dampener)
{
    // Check if the sequence is incrementing or decrementing by comparing 4 sets of values.
    // For a valid set of inputs (with a dampener) at most only one of them will be incorrect
    // Using 4 instead of 3 checks, to avoid the result being "poisoned" by one bad value being used in multiple checks
    int incCount = 0;
    if (values[^1] > values[0]) incCount++;
    if (values[^1] > values[1]) incCount++;
    if (values[^2] > values[0]) incCount++;
    if (values[^2] > values[1]) incCount++;
    bool increasing = incCount > 2;

    var valid0 = IsValid(values[0], values[1], increasing);
    var valid1 = IsValid(values[1], values[2], increasing);
    var valid2 = IsValid(values[2], values[3], increasing);

    // Fast check for first value being invalid
    if (!valid0 && dampener)
    {
        // If second check is invalid too, try removing it
        if (!valid1)
        {
            valid1 = IsValid(values[0], values[2], increasing);
            if (!valid1)
                return false;
        }

        valid0 = true;
        dampener = false;
    }

    // Quick failure check at the beginning of the sequence
    if (!valid0 || !valid1 || !valid2)
    {
        if (!dampener || // No tolerance
            !(valid0 || valid1 || valid2) || // All 3 bad
            valid0 ^ valid1 ^ valid2) // 2 are bad
            return false;
    }

    // Check for second value being invalid. If invalid, adjust the check, then verify we don't have two failures
    if (!valid1)
    {
        dampener = false;
        valid1 = IsValid(values[0], values[2], increasing);
        if (!valid1)
            return false;
    }

    // Run validation with a sliding 2 check set.
    // If the first of the two is not valid, (and we have a dampener) we can ignore it and readjust
    // If no dampener, and the new check is not valid - we exit
    for (int i = 3; i < values.Length - 1; i++)
    {
        valid1 = valid2;

        if (!valid1)
        {
            dampener = false;
            valid2 = IsValid(values[i - 1], values[i + 1], increasing);
        }
        else
        {
            valid2 = IsValid(values[i], values[i + 1], increasing);
        }

        if (!dampener && !valid2)
            return false;
    }

    return true;

    static bool IsValid(int prev, int next, bool increasing)
        =>
        prev != next &&
        Math.Abs(prev - next) <= 3 &&
        prev < next == increasing;
}
