#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var sw = Stopwatch.StartNew();
var input = FileHelpers.ReadInputLines("05.txt");
var ranges = input
    .TakeWhile(s => !string.IsNullOrEmpty(s))
    .Select(l => l.Split('-', 2))
    .Select(x => (start: long.Parse(x[0]), end: long.Parse(x[1])))
    .ToArray();
var ingredients = input
    .SkipWhile(s => !string.IsNullOrEmpty(s))
    .Skip(1)
    .Select(long.Parse)
    .ToArray();
var parseTime = sw.Elapsed;

sw.Restart();
var fresh = ingredients.Count(n => ranges.Any(r => n >= r.start && n <= r.end));
Console.WriteLine($"Part 1: {fresh}");
var part1Time = sw.Elapsed;

sw.Restart();
long allFreshCount = 0, last = 0;
foreach (var (start, end) in ranges.OrderBy(x => x.start))
{
    var first = start > last ? start : last + 1;
    if (first > end)
        continue;

    allFreshCount += end - first + 1;
    last = end;
}

Console.WriteLine($"Part 2: {allFreshCount}");
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);
