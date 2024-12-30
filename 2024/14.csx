#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;
using System.Text.RegularExpressions;

var input = File.ReadAllText("2024/inputs/14.real.txt");

var mapMatch = Regex.Match(input, @"x=(?<x>\d+) y=(?<y>\d+)");
var map = (x: int.Parse(mapMatch.Groups["x"].Value), y: int.Parse(mapMatch.Groups["y"].Value));

var robots = Regex.Matches(input, @"p=(?<px>[-\d]+),(?<py>[-\d]+) v=(?<vx>[-\d]+),(?<vy>[-\d]+)")
	.Select(m => new Robot(
		new Point(Convert.ToInt32(m.Groups["px"].Value), Convert.ToInt32(m.Groups["py"].Value)),
		new Velocity(Convert.ToInt32(m.Groups["vx"].Value), Convert.ToInt32(m.Groups["vy"].Value))))
	.ToArray();

// Part 1
foreach (var robot in robots)
{
	robot.Move(map, 100);
}
new[] {
	robots.Where(r => r.Pos.x < map.x / 2 && r.Pos.y < map.y / 2).Count(),
	robots.Where(r => r.Pos.x > map.x / 2 && r.Pos.y < map.y / 2).Count(),
	robots.Where(r => r.Pos.x < map.x / 2 && r.Pos.y > map.y / 2).Count(),
	robots.Where(r => r.Pos.x > map.x / 2 && r.Pos.y > map.y / 2).Count(),
}.Aggregate(1, (s, i) => s * i).Dump("Part 1").Should().BeOneOf(12, 219512160);

// Part 2
foreach (var robot in robots) robot.Reset();
var seconds = 0;
while (++seconds < 10_000)
{
	var current = new Map(map.x, map.y).Fill();
	foreach (var robot in robots)
	{
		robot.Move(map);
		current[robot.Pos.x, robot.Pos.y] = 'X';
	}

	var maxSequence = 0;
	for (var y = 0; y < map.y; y++)
	{
		var sequence = 0;
		for (var x = 0; x < map.x; x++)
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
		seconds.Dump("Part 2").Should().Be(6398);
		//current.ToImage(200).Dump();
		break;
	}
}

class Robot(Point pos, Velocity vel)
{
	private Point InitialPosition {get;set;} = pos;
	public Point Pos { get; set; } = pos;
	public Velocity Vel { get; } = vel;
	
	public void Move((int x, int y) map, int seconds = 1) => Pos = (Pos + (Vel * seconds)).Normalise(map);
	public void Reset() => Pos = InitialPosition;
}

partial record struct Point
{
	//public Point Normalise((int x, int y) map) => (x % map.x + (x < 0 ? map.x : 0), y % map.y + (y < 0 ? map.y : 0));
	public Point Normalise((int x, int y) map)
	{
		var nx = x % map.x;
		if (nx < 0) nx += map.x;
		var ny = y % map.y;
		if (ny < 0) ny += map.y;
		return (nx, ny);
	}
}