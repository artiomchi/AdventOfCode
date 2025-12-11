#!/usr/bin/env dotnet
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var sw = Stopwatch.StartNew();
var connections = FileHelpers.ReadInputLines("11.txt")
    .ToDictionary(l => l[..l.IndexOf(':')], l => l[(l.IndexOf(':') + 2)..].Split(' '));
var parseTime = sw.Elapsed;

sw.Restart();
var paths = Expand([], "you", "out").Sum(p => p.Paths);
var part1Time = sw.Elapsed;
paths.DumpAndAssert("Part 1", 5, 746);

sw.Restart();
connections = FileHelpers.ReadInputLines("11.txt")
    .ToDictionary(l => l[..l.IndexOf(':')], l => l[(l.IndexOf(':') + 2)..].Split(' '));
var parse2Time = sw.Elapsed;

sw.Restart();
var result = Expand([], "svr", "out");
var pathsWithDacAndFft = result.Sum(p => p.ContainsDacAndFft);
var part2Time = sw.Elapsed;
pathsWithDacAndFft.DumpAndAssert("Part 2", 2, 370500293582760);

OutputHelpers.PrintTimings(parseTime, part1Time, parse2Time, part2Time);

IEnumerable<PathInfo> Expand(Dictionary<string, PathInfo> memory, string position, string destination = "out")
{
    if (!connections.TryGetValue(position, out var outgoing))
    {
        yield break;
    }
    foreach (var next in outgoing)
    {
        if (next == position)
        {
            continue;
        }

        var key = $"{position}>{next}";

        if (memory.TryGetValue(key, out var existingInfo))
        {
            yield return existingInfo;
            continue;
        }

        if (next == destination)
        {
            var info = new PathInfo
            {
                Paths = 1,
                ContainsDac = next == "dac" || position == "dac" ? 1 : 0,
                ContainsFft = next == "fft" || position == "fft" ? 1 : 0,
                ContainsDacAndFft = next is "dac" or "fft" && position is "dac" or "fft" ? 1 : 0
            };
            memory[key] = info;
            yield return info;
        }
        else
        {
            var paths = Expand(memory, next, destination).ToArray();
            var info = new PathInfo
            {
                Paths = paths.Sum(p => p.Paths),
                ContainsDac = paths.Sum(p => p.ContainsDac) + (position == "dac" ? paths.Sum(p => p.Paths) : 0),
                ContainsFft = paths.Sum(p => p.ContainsFft) + (position == "fft" ? paths.Sum(p => p.Paths) : 0),
                ContainsDacAndFft =
                    paths.Sum(p => p.ContainsDacAndFft) +
                    (position == "dac" ? paths.Sum(p => p.ContainsFft) : 0) +
                    (position == "fft" ? paths.Sum(p => p.ContainsDac) : 0)
            };
            memory[key] = info;
            yield return info;
        }
    }
}

struct PathInfo
{
    public long Paths;
    public long ContainsDac;
    public long ContainsFft;
    public long ContainsDacAndFft;
}
