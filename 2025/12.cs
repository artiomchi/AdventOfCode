#!/usr/bin/env dotnet
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;
using System.Text.RegularExpressions;

var sw = Stopwatch.StartNew();
var connections = FileHelpers.ReadInputText("12.txt");
var shapes = Regex.Matches(connections, @"\d+:\r?\n((?:[\.#]+\r?\n)+)", RegexOptions.Multiline)
    .Select(m => m.Groups[1].Value.Trim().Split(Environment.NewLine))
    .ToArray();
var regions = Regex.Matches(connections, @"(\d+)x(\d+): ([\d ]+)")
    .Select(m => (
        Width: int.Parse(m.Groups[1].Value),
        Height: int.Parse(m.Groups[2].Value),
        Targets: m.Groups[3].Value.Split(' ').Select(int.Parse).ToArray()))
    .ToArray();
var parseTime = sw.Elapsed;

sw.Restart();
var result = 0;
foreach (var (w, h, targets) in regions)
{
    var totalArea = w * h;
    var totalSpaceNeeded = targets.Index()
        .Sum(x => shapes[x.Index].Count() * shapes[x.Index][0].Length * x.Item);
    if (totalSpaceNeeded <= totalArea)
        result++;
}
var part1Time = sw.Elapsed;

result.DumpAndAssert("Part 1", 2, 579);
OutputHelpers.PrintTimings(parseTime, part1Time);
