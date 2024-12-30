#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;
using System.Text.RegularExpressions;

var input = File.ReadAllText("2024/inputs/13.real.txt");

var machines = Regex.Matches(input, @"Button A: X\+(?<ax>\d+), Y\+(?<ay>\d+)\s+Button B: X\+(?<bx>\d+), Y\+(?<by>\d+)\s+Prize: X=(?<px>\d+), Y=(?<py>\d+)")
	.Select(m => new Machine
	{
		ButtonA = new Point(Convert.ToInt32(m.Groups["ax"].Value), Convert.ToInt32(m.Groups["ay"].Value)),
		ButtonB = new Point(Convert.ToInt32(m.Groups["bx"].Value), Convert.ToInt32(m.Groups["by"].Value)),
		Prize = new Point(Convert.ToInt32(m.Groups["px"].Value), Convert.ToInt32(m.Groups["py"].Value)),
	})
	.ToArray();

Solve(machines, false).Dump("Part 1").Should().BeOneOf(480, 27157);
Solve(machines, true).Dump("Part 2").Should().BeOneOf(875318608908, 104015411578548);

long Solve(Machine[] machines, bool part2)
{
	var multiplier = part2
		? 10_000_000_000_000
		: 0;
	long total = 0;
	foreach (var machine in machines)
	{
		var a = machine.ButtonA.x;
		var b = machine.ButtonB.x;
		var c = machine.Prize.x + multiplier;
		var d = machine.ButtonA.y;
		var e = machine.ButtonB.y;
		var f = machine.Prize.y + multiplier;

		var x = (c*e-f*b)/(a*e-b*d);
		var y = (c - a*x) / b;
		
		if (a*x + b*y == c && d*x + e*y == f)
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
