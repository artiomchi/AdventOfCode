#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var lines = FileHelpers.ReadInputLines("10.txt");
var sw = Stopwatch.StartNew();
var map = new int[lines.Length,lines[0].Length];
for (var x = 0; x < lines.Length; x++)
    for (var y = 0; y < lines[0].Length; y++)
        map[x,y] = lines[x][y] - '0';

var heads = new HashSet<(int x, int y)>();
for (var x = 0; x < map.GetLength(0); x++)
    for (var y = 0; y < map.GetLength(1); y++)
        if (map[x,y] == 0)
            heads.Add((x, y));
var prepTime = sw.Elapsed;

sw.Restart();
heads.Select(h => CountScore(map, h, 1)).Sum().DumpAndAssert("Part 1", 36, 825);
var part1Time = sw.Elapsed;
sw.Restart();
heads.Select(h => CountScore(map, h, 2)).Sum().DumpAndAssert("Part 2", 81, 1805);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(prepTime, part1Time, part2Time);

static int CountScore(int[,] map, (int x, int y) head, int part)
{
    var positions = new List<(int, int)>([head]);
    for (var i = 1; i <= 9; i++)
    {
        var newPositions = new List<(int, int)>();
        foreach (var (x, y) in positions)
        {
            var directions = Vector.Directions;
            foreach (var (a, b) in directions)
            {
                if (x + a < 0 || y + b < 0 || x + a >= map.GetLength(0) || y + b >= map.GetLength(1))
                    continue;

                if (map[x + a, y + b] == i)
                    newPositions.Add((x + a, y + b));
            }
        }

        positions = (part == 1 ? newPositions.Distinct() : newPositions).ToList();
    }

    return positions.Count();
}
