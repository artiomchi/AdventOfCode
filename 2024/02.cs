#!/usr/bin/env dotnet
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
input.Count(v => ValidateLevel(v)).DumpAndAssert("Part 1", 2, 534);
var part1Time = sw.Elapsed;

// Part 2
sw.Restart();
input.Count(v => ValidateLevelWithDampener(v)).DumpAndAssert("Part 2", 4, 577);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);

static bool ValidateLevel(ICollection<int> values)
    => values.Aggregate(
        (last: (int?)null, increasing: (bool?)null),
        (r, v) => (r.last, r.increasing, v) switch
        {
            ( < 0, _, _) => (r.last.Value, null),                           // Failure fast exit
            (null, null, _) => (v, null),                                   // First pass
            ({ }, _, _) when r.last == v => (-2, null),                     // Same value
            ({ }, _, _) when Math.Abs(r.last.Value - v) > 3 => (-3, null),  // Too fast
            ({ }, null, _) => (v, r.last < v),                              // Second value (learning whether we're increasing or not)
            ({ }, { }, _) when r.last < v != r.increasing => (-4, null),    // Direction swap
            _ => (v, r.increasing)
        }).last >= 0;

static bool ValidateLevelWithDampener(int[] values)
    => ValidateLevel(values) ||
        Enumerable.Range(0, values.Length)
        .Any(i =>
        {
            var result = values.ToList();
            result.RemoveAt(i);
            return ValidateLevel(result);
        });
