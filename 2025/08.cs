#!/usr/bin/env dotnet
#:package System.Numerics.Tensors@9.0.0
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;
using System.Numerics.Tensors;

var sw = Stopwatch.StartNew();
var inputs = FileHelpers.ReadInputLines("08.txt")
    .Select(l => l.Split(',').Select(float.Parse).ToArray())
    .ToArray();
var needSets = inputs.Length > 20 ? 1000 : 10;
var parseTime = sw.Elapsed;

sw.Restart();
Dictionary<(int, int), float> distances = [];
for (int i = 0; i < inputs.Length; i++)
{
    for (int j = i + 1; j < inputs.Length; j++)
    {
        var distance = TensorPrimitives.Distance(inputs[i], inputs[j]);
        distances[(i, j)] = distance;
    }
}
var distanceTime = sw.Elapsed;

sw.Restart();
List<List<int>> circuits = [];
int connected = 0, part1 = 0, part2 = 0;
var part1Time = TimeSpan.Zero;

foreach (var (pair, distance) in distances.OrderBy(kv => kv.Value))
{
    if (connected++ == needSets)
    {
        part1 = circuits.OrderByDescending(l => l.Count).Take(3).Aggregate((int)1, (acc, l) => acc * l.Count);
        part1Time = sw.Elapsed;
        sw.Restart();
    }

    var (i, j) = pair;

    var ii = circuits.FindIndex(l => l.Contains(i));
    var ji = circuits.FindIndex(l => l.Contains(j));

    if (ii < 0 && ji < 0)
    {
        circuits.Add([i, j]);
        continue;
    }
    else if (ii == ji)
    {
        // both in same circuit, do nothing
        continue;
    }
    else if (ii >= 0 && ji >= 0)
    {
        circuits[ii].AddRange(circuits[ji].Except(circuits[ii]));
        circuits.RemoveAt(ji);
    }
    else if (ii >= 0)
    {
        circuits[ii].Add(j);
    }
    else if (ji >= 0)
    {
        circuits[ji].Add(i);
    }

    if (circuits.Count == 1 && circuits[0].Count == inputs.Length)
    {
        part2 = (int)inputs[pair.Item1][0] * (int)inputs[pair.Item2][0];
        break;
    }
}
var part2Time = sw.Elapsed;

part1.DumpAndAssert("Part 1", 40, 54600);
part2.DumpAndAssert("Part 2", 25272, 107256172);

OutputHelpers.PrintTimings(parseTime, distanceTime, part1Time, part2Time);
