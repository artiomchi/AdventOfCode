#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;
using System.Text.RegularExpressions;

var input = FileHelpers.ReadInputText("03.txt");
var sw = new Stopwatch();

// Part 1
sw.Restart();
var matches = Regex.Matches(input, @"mul\((\d{1,3}),(\d{1,3})\)");
var result = matches.Select(m => int.Parse(m.Groups[1].Value) * int.Parse(m.Groups[2].Value)).Sum();
result.DumpAndAssert("Part 1", 161, 188192787);
var part1Time = sw.Elapsed;

// Part 2
sw.Restart();
matches = Regex.Matches(input, @"mul\((\d{1,3}),(\d{1,3})\)|don't\(\)|do\(\)");
result = matches.Aggregate(
    (true, 0),
    (x, r) => (x.Item1, x.Item2, r) switch
    {
        (_, _, _) when r.Value == "don't()" => (false, x.Item2),
        (_, _, _) when r.Value == "do()" => (true, x.Item2),
        (true, _, _) => (true, x.Item2 + int.Parse(r.Groups[1].Value) * int.Parse(r.Groups[2].Value)),
        _ => x,
    })
    .Item2;
result.DumpAndAssert("Part 2", 48, 113965544);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(part1Time, part2Time);
