#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;
using System.Text.RegularExpressions;

//var map = (Width: 11, Height: 7); // Sample input
var map = (Width: 101, Height: 103);
var input = FileHelpers.ReadInputText("14.txt");
var sw = Stopwatch.StartNew();

var robots = Regex.Matches(input, @"p=(?<px>[-\d]+),(?<py>[-\d]+) v=(?<vx>[-\d]+),(?<vy>[-\d]+)")
    .Select(m => new Robot(
        new Point(int.Parse(m.Groups["px"].Value), int.Parse(m.Groups["py"].Value)),
        new Vector(int.Parse(m.Groups["vx"].Value), int.Parse(m.Groups["vy"].Value))))
    .ToArray();
var parseTime = sw.Elapsed;

// Part 1
sw.Restart();
foreach (var robot in robots)
{
    robot.Move(map, 100);
}
new[] {
    robots.Where(r => r.Pos.X < map.Width / 2 && r.Pos.Y < map.Height / 2).Count(),
    robots.Where(r => r.Pos.X > map.Width / 2 && r.Pos.Y < map.Height / 2).Count(),
    robots.Where(r => r.Pos.X < map.Width / 2 && r.Pos.Y > map.Height / 2).Count(),
    robots.Where(r => r.Pos.X > map.Width / 2 && r.Pos.Y > map.Height / 2).Count(),
}.Aggregate(1, (s, i) => s * i).DumpAndAssert("Part 1", 12, 219512160);
var part1Time = sw.Elapsed;

// Part 2
sw.Restart();
foreach (var robot in robots) robot.Reset();
var seconds = 0;
while (++seconds < 10_000)
{
    var current = new Map(map.Width, map.Height).Fill();
    foreach (var robot in robots)
    {
        robot.Move(map);
        current[robot.Pos.X, robot.Pos.Y] = 'X';
    }

    var maxSequence = 0;
    for (var y = 0; y < map.Height; y++)
    {
        var sequence = 0;
        for (var x = 0; x < map.Width; x++)
            if (current[x,y] == 'X')
                sequence++;
            else
            {
                maxSequence = Math.Max(maxSequence, sequence);
                sequence = 0;
            }
        maxSequence = Math.Max(maxSequence, sequence);
    }

    if (maxSequence > 10)
    {
        seconds.DumpAndAssert("Part 2", 6398);
        break;
    }
}
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);

class Robot(Point pos, Vector vel)
{
    private Point InitialPosition { get; set; } = pos;
    public Point Pos { get; set; } = pos;
    public Vector Vel { get; } = vel;

    public void Move((int x, int y) map, int seconds = 1) => Pos = (Pos + (Vel * seconds)).Normalise(map);
    public void Reset() => Pos = InitialPosition;
}

static class LocalExtensions
{
    extension(Point point)
    {
        public Point Normalise((int width, int height) map)
        {
            var nx = point.X % map.width;
            if (nx < 0) nx += map.width;
            var ny = point.Y % map.height;
            if (ny < 0) ny += map.height;
            return (nx, ny);
        }
    }
}
