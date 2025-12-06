#!/usr/bin/env dotnet
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;
using System.Text.RegularExpressions;

var input = FileHelpers.ReadInputText("13.txt");
var sw = Stopwatch.StartNew();
var machines = Regex.Matches(input, @"Button A: X\+(?<ax>\d+), Y\+(?<ay>\d+)\s+Button B: X\+(?<bx>\d+), Y\+(?<by>\d+)\s+Prize: X=(?<px>\d+), Y=(?<py>\d+)")
    .Select(m => new Machine
    {
        ButtonA = new Point(int.Parse(m.Groups["ax"].Value), int.Parse(m.Groups["ay"].Value)),
        ButtonB = new Point(int.Parse(m.Groups["bx"].Value), int.Parse(m.Groups["by"].Value)),
        Prize = new Point(int.Parse(m.Groups["px"].Value), int.Parse(m.Groups["py"].Value)),
    })
    .ToArray();
var parseTime = sw.Elapsed;

sw.Restart();
Solve(machines, false).DumpAndAssert("Part 1", 480, 27157);
var part1Time = sw.Elapsed;
sw.Restart();
Solve(machines, true).DumpAndAssert("Part 2", 875318608908, 104015411578548);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);

static long Solve(Machine[] machines, bool part2)
{
    var multiplier = part2
        ? 10_000_000_000_000
        : 0;
    long total = 0;
    foreach (var machine in machines)
    {
        var (a, d) = machine.ButtonA;
        var (b, e) = machine.ButtonB;
        var c = machine.Prize.X + multiplier;
        var f = machine.Prize.Y + multiplier;

        var x = (c * e - f * b) / (a * e - b * d);
        var y = (c - a * x) / b;

        if (a * x + b * y == c && d * x + e * y == f)
        {
            total += x * 3 + y;
        }
    }
    return total;
}

class Machine
{
    public Point ButtonA { get; set; }
    public Point ButtonB { get; set; }
    public Point Prize { get; set; }
}
