#!/usr/bin/env dotnet
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var lines = FileHelpers.ReadInputLines("07.txt");
var sw = Stopwatch.StartNew();
var equasions = lines.Select(l => l.Split(new[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(i => Convert.ToInt64(i)).ToArray()).ToArray();
var parseTime = sw.Elapsed;

// Part 1
sw.Restart();
long result = 0;
foreach (var equasion in equasions)
{
    if (Validate(equasion[0], equasion[1], equasion[2..]))
    {
        result += equasion[0];
        continue;
    }
}
result.DumpAndAssert("Part 1", 3749, 5540634308362);
var part1Time = sw.Elapsed;

// Part 2
sw.Restart();
result = 0;
foreach (var equasion in equasions)
{
    if (Validate(equasion[0], equasion[1], equasion[2..], 3))
    {
        result += equasion[0];
        continue;
    }
}
result.DumpAndAssert("Part 2", 11387, 472290821152397);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);

static bool Validate(long expected, long acc, ReadOnlySpan<long> values, int operations = 2)
{
    if (acc > expected)
        return false;
    if (values.Length == 0)
        return expected == acc;

    var next = values[0];
    var remaining = values[1..];

    return
        Validate(expected, acc + next, remaining, operations) ||
        Validate(expected, acc * next, remaining, operations) ||
        (operations > 2 && Validate(expected, acc * (long)Math.Pow(10, (int)Math.Log10(next) + 1) + next, remaining, operations));
}
