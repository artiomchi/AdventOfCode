#!/usr/bin/env dotnet
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var sw = Stopwatch.StartNew();
var inputs = FileHelpers.ReadInputLines("09.txt")
    .Select(l => l.Split(',').Select(int.Parse).ToArray())
    .ToArray();
var parseTime = sw.Elapsed;

sw.Restart();
long maxArea1 = 0;
for (int i = 0; i < inputs.Length; i++)
{
    for (int j = i + 1; j < inputs.Length; j++)
    {
        var area = (long)Math.Abs(inputs[i][0] - inputs[j][0] + 1) * Math.Abs(inputs[i][1] - inputs[j][1] + 1);
        maxArea1 = Math.Max(maxArea1, area);
    }
}
var part1Time = sw.Elapsed;
maxArea1.DumpAndAssert("Part 1", 50, 4763509452);

sw.Restart();
long maxArea2 = 0;
for (int i = 0; i < inputs.Length; i++)
{
    for (int j = i + 1; j < inputs.Length; j++)
    {
        // If any other point is inside the rectangle, skip it
        if (inputs.Any(k =>
            k != inputs[i] && k != inputs[j] &&
            k[0] > Math.Min(inputs[i][0], inputs[j][0]) &&
            k[0] < Math.Max(inputs[i][0], inputs[j][0]) &&
            k[1] > Math.Min(inputs[i][1], inputs[j][1]) &&
            k[1] < Math.Max(inputs[i][1], inputs[j][1])))
        {
            continue;
        }

        var lines = new Line[]
        {
            new(inputs[i][0], inputs[i][1], inputs[j][0], inputs[i][1]),
            new(inputs[j][0], inputs[i][1], inputs[j][0], inputs[j][1]),
            new(inputs[j][0], inputs[j][1], inputs[i][0], inputs[j][1]),
            new(inputs[i][0], inputs[j][1], inputs[i][0], inputs[i][1]),
        };

        var multipleIntersections = false;
        for (int k = 0; k < inputs.Length; k++)
        {
            var k2 = k == inputs.Length - 1 ? 0 : k + 1;
            var testLine = new Line(inputs[k][0], inputs[k][1], inputs[k2][0], inputs[k2][1]);
            if (lines.Contains(testLine))
            {
                continue;
            }

            var intersections = lines
                .Count(l => AreLinesCrossing(l, testLine));
            if (intersections > 1)
            {
                multipleIntersections = true;
                break;
            }
        }
        if (multipleIntersections)
        {
            continue;
        }


        var area = (long)(Math.Abs(inputs[i][0] - inputs[j][0]) + 1) * (Math.Abs(inputs[i][1] - inputs[j][1]) + 1);
        maxArea2 = Math.Max(maxArea2, area);
    }
}
var part2Time = sw.Elapsed;
maxArea2.DumpAndAssert("Part 2", 24, 1516897893);

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);

static bool AreLinesCrossing(Line lineA, Line lineB)
{
    if ((lineA.X1 == lineA.X2 && lineB.X1 == lineB.X2) ||
        (lineA.Y1 == lineA.Y2 && lineB.Y1 == lineB.Y2))
    {
        // lines are parallel or overlapping, not an issue
        return false;
    }

    var points = new HashSet<(int x, int y)>
    {
        (lineA.X1, lineA.Y1),
        (lineA.X2, lineA.Y2),
        (lineB.X1, lineB.Y1),
        (lineB.X2, lineB.Y2),
    };
    if (points.Count < 4)
    {
        // lines share a point, not an issue
        return false;
    }

    if (lineA.X1 == lineA.X2)
    {
        return
            lineA.X1 >= Math.Min(lineB.X1, lineB.X2) &&
            lineA.X1 <= Math.Max(lineB.X1, lineB.X2) &&
            lineB.Y1 >= Math.Min(lineA.Y1, lineA.Y2) &&
            lineB.Y1 <= Math.Max(lineA.Y1, lineA.Y2);
    }
    else
    {
        return
            lineB.X1 >= Math.Min(lineA.X1, lineA.X2) &&
            lineB.X1 <= Math.Max(lineA.X1, lineA.X2) &&
            lineA.Y1 >= Math.Min(lineB.Y1, lineB.Y2) &&
            lineA.Y1 <= Math.Max(lineB.Y1, lineB.Y2);
    }
}

public record struct Line(int X1, int Y1, int X2, int Y2);
