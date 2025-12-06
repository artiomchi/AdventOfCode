#!/usr/bin/env dotnet
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Buffers;
using System.Diagnostics;

var lines = FileHelpers.ReadInputLines("19.txt");
var sw = Stopwatch.StartNew();
var towels = lines[0].Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).OrderByDescending(t => t.Length).ToArray();
var patterns = lines.Skip(2);

var sv = SearchValues.Create(towels, StringComparison.Ordinal);
var parseTime = sw.Elapsed;

sw.Restart();
var knownSets = new Dictionary<string, long>();
long possible = 0, combinations = 0;
foreach (var (index, pattern) in patterns.Index())
{
    var matchingTowels = towels.Where(t => t.All(pattern.Contains)).ToArray();
    if (matchingTowels.Length == 0)
        continue;

    var localCombos = BuildMatch(pattern.AsMemory(), matchingTowels);
    if (localCombos > 0)
        possible++;

    combinations += localCombos;

    long BuildMatch(ReadOnlyMemory<char> pattern, string[] towels)
    {
        if (knownSets.TryGetValue(pattern.ToString(), out var result))
        {
            return result;
        }

        var directMatches = towels.Count(t => t.Length == pattern.Length && pattern.Span.SequenceEqual(t));
        var subMatches = towels
            .Where(t => pattern.Length > t.Length && pattern.Span[0] == t[0])
            .Where(t => pattern[..t.Length].Span.SequenceEqual(t))
            .Sum(t => BuildMatch(pattern[t.Length..], towels));

        result = directMatches + subMatches;
        knownSets[pattern.ToString()] = result;

        return result;
    }
}

possible.DumpAndAssert("Part 1", 6, 283);
combinations.DumpAndAssert("Part 2", 16, 615388132411142);
var solveTime = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, solveTime);
