#!/usr/bin/env dotnet
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var lines = FileHelpers.ReadInputLines("05.txt");
var sw = Stopwatch.StartNew();
var pageRules = lines.TakeWhile(l => l.Length > 0).Select(l => l.Split('|').Select(int.Parse).ToArray()).ToLookup(l => l[1], l => l[0]);
var updates = lines.SkipWhile(l => l.Length > 0).Skip(1).Select(l => l.Split(',').Select(int.Parse).ToArray()).ToArray();
var parseTime = sw.Elapsed;

// Part 1
sw.Restart();
var totalValid = updates.Where(IsUpdateValid).Sum(u => u[u.Length / 2]);
totalValid.DumpAndAssert("Part 1", 143, 5329);
var part1Time = sw.Elapsed;

// Part 2
sw.Restart();
var totalInvalid = updates
    .Where(u => !IsUpdateValid(u))
    .Sum(u => u
        .Order(Comparer<int>.Create((a, b) => a == b ? 0 : pageRules[a].Contains(b) ? 1 : -1))
        .ElementAt(u.Length / 2));
totalInvalid.DumpAndAssert("Part 2", 123, 5833);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);

bool IsUpdateValid(int[] update)
    => !update.Index().Any(x => pageRules[x.Item].Intersect(update.Skip(x.Index)).Any());
