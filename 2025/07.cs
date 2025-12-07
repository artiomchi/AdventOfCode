#!/usr/bin/env dotnet
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var sw = Stopwatch.StartNew();
var inputs = FileHelpers.ReadInputText("07.txt");
var map = Map.FromString(inputs);
var parseTime = sw.Elapsed;

sw.Restart();
var start = map.Find('S');
Dictionary<Point, long> beams = new() { [start] = 1 };
var splits = 0;
for (int i = start.Y + 1; i < map.Height - 1; i++)
{
    var next = new Dictionary<Point, long>();
    void AddBeam(Point point, long timelines)
    {
        if (next.TryGetValue(point, out var existing))
        {
            next[point] = existing + timelines;
        }
        else
        {
            next[point] = timelines;
        }
    }

    foreach (var (beam, timelines) in beams)
    {
        var below = beam.Move('v');
        switch (map[below])
        {
            case '.':
                AddBeam(below, timelines);
                continue;
            case '^':
                AddBeam(below.Move('<'), timelines);
                AddBeam(below.Move('>'), timelines);
                splits++;
                continue;
        }
    }

    beams = next;
}
var solveTime = sw.Elapsed;
splits.DumpAndAssert("Part 1", 21, 1553);
beams.Values.Sum().DumpAndAssert("Part 2", 40, 15811946526915);

OutputHelpers.PrintTimings(parseTime, solveTime);
