#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var map = FileHelpers.ReadInputLines("08.txt");
var antinodes1 = new bool[map.Length, map[0].Length];
var antinodes2 = new bool[map.Length, map[0].Length];
var sw = Stopwatch.StartNew();

var frequencies = map
    .SelectMany((row, x) => row
        .Select((c, y) => (x, y, c)))
        .Where(x => char.IsLetterOrDigit(x.c))
    .ToLookup(x => x.c, x => new Point(x.x, x.y));
var antennaSets = frequencies
    .SelectMany(g => g
        .SelectMany(a => frequencies[g.Key]
            .Where(b => b.X > a.X)
            .Select(b => (a, b))));
foreach (var (a, b) in antennaSets)
{
    var dist = b - a;
    int x, y, mul = 0;
    while (IsInBounds((x, y) = a - dist * mul))
    {
        if (mul == 1)
            antinodes1[x, y] = true;
        antinodes2[x, y] = true;
        mul++;
    }

    mul = 1;
    while (IsInBounds((x, y) = a + dist * mul))
    {
        if (mul == 2)
            antinodes1[x, y] = true;
        antinodes2[x, y] = true;
        mul++;
    }
}

antinodes1.OfType<bool>().Count(n => n).DumpAndAssert("Part 1", 14, 329);
antinodes2.OfType<bool>().Count(n => n).DumpAndAssert("Part 2", 34, 1190);

OutputHelpers.PrintTimings(sw.Elapsed);

// helpers
bool IsInBounds(Point p) { return p.X >= 0 && p.Y >= 0 && p.X < map.Length && p.Y < map[0].Length; }
