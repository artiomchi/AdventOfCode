#!/usr/bin/env dotnet
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var sw = Stopwatch.StartNew();
var lines = FileHelpers.ReadInputLines("06.txt");
var data = lines
    .Select(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    .ToArray();
var parseTime = sw.Elapsed;

// Part 1
sw.Restart();
var numbers1 = data.Take(data.Length - 1)
    .Select(l => l.Select(int.Parse).ToArray())
    .ToArray();

long total1 = 0;
foreach (var (i, op) in lines[^1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Index())
{
    total1 += numbers1.Aggregate(
        op == "*" ? (long)1 : 0,
        (acc, l) => op == "*" ? acc * l[i] : acc + l[i]);
}

total1.DumpAndAssert("Part 1", 4277556, 5361735137219);
var part1Time = sw.Elapsed;

// Part 2
sw.Restart();
long total2 = 0;
var index = 0;
do
{
    var nextIndex = lines[^1].IndexOfAny(['*', '+'], index + 1);
    var length = nextIndex == -1 ? lines[^1].Length - index : nextIndex - index - 1;

    var numbers2 = Enumerable.Sequence(length - 1, 0, -1)
        .Select(i => lines.Take(lines.Length - 1).Select(l => l.Length > index + i ? l[index + i] : ' '))
        .Select(d => d.Aggregate(0, (acc, d) => d != ' ' ? acc * 10 + (d - '0') : acc))
        .ToArray();
    var op = lines[^1][index];

    total2 += numbers2.Aggregate(
        op == '*' ? (long)1 : 0,
        (acc, l) => op == '*' ? acc * l : acc + l);

    if (nextIndex == -1)
        break;
    index = nextIndex;
}
while (true);
total2.DumpAndAssert("Part 2", 3263827, 11744693538946);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);
