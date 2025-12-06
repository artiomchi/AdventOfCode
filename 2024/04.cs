#!/usr/bin/env dotnet
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var lines = FileHelpers.ReadInputLines("04.txt");
var sw = Stopwatch.StartNew();
var matrix = lines.Select(s => s.Trim().ToArray()).ToArray();
var parseTime = sw.Elapsed;

// Part 1
sw.Restart();
var matches = 0;
foreach (var (i, line) in matrix.Index())
    foreach (var (j, c) in line.Index())
    {
        if (c != 'X')
            continue;

        foreach (var (di, dj) in Vector.DirectionsAll)
        {
            if (i + di * 3 >= matrix.Length || i + di * 3 < 0 ||
                j + dj * 3 >= line.Length || j + dj * 3 < 0)
                continue;

            if (matrix[i + di][j + dj] == 'M' && matrix[i + di * 2][j + dj * 2] == 'A' && matrix[i + di * 3][j + dj * 3] == 'S')
                matches++;
        }
    }

matches.DumpAndAssert("Part 1", 18, 2633);
var part1Time = sw.Elapsed;

// Part 2
sw.Restart();
matches = 0;
foreach (var (i, line) in matrix.Index())
    foreach (var (j, c) in line.Index())
    {
        if (i == 0 || j == 0 || i == matrix.Length - 1 || j == line.Length - 1 || c != 'A')
            continue;

        string[] mass = [
            new string([matrix[i - 1][j - 1], matrix[i + 1][j + 1]]),
            new string([matrix[i + 1][j + 1], matrix[i - 1][j - 1]]),
            new string([matrix[i - 1][j + 1], matrix[i + 1][j - 1]]),
            new string([matrix[i + 1][j - 1], matrix[i - 1][j + 1]]),
        ];
        if (mass.Count(s => s == "MS") == 2)
            matches++;
    }

matches.DumpAndAssert("Part 2", 9, 1936);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);
