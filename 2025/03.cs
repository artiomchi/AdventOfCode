#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var sw = Stopwatch.StartNew();
var input = FileHelpers.ReadInputLines("03.txt")
    .Select(l => l.Select(c => c - '0').ToArray())
    .ToArray();
var parseTime = sw.Elapsed;

long total1 = 0, total2 = 0;

sw.Restart();
foreach (var digits in input)
{
    var first = digits[..^1].Max();
    var second = digits.SkipWhile(d => d != first).Skip(1).Max();
    total1 += first * 10 + second;
}
total1.DumpAndAssert("Part 1", 357, 17524);
var part1Time = sw.Elapsed;

sw.Restart();
foreach (var digits in input)
{
    var index = 0;
    var remaining = 12;
    long bank = 0;

    while (remaining > 0)
    {
        remaining--;

        var next = digits[index..^remaining].Max();
        bank = bank * 10 + next;
        index += digits[index..].IndexOf(next) + 1;
    }

    total2 += bank;
}
total2.DumpAndAssert("Part 2", 3121910778619, 173848577117276);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);
