#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;
using System.Text.RegularExpressions;

var input = File.ReadAllText("2024/inputs/03.real.txt");

// Part 1
var matches = Regex.Matches(input, @"mul\((\d{1,3}),(\d{1,3})\)");
var result = matches.Select(m => Convert.ToInt32(m.Groups[1].Value) * Convert.ToInt32(m.Groups[2].Value)).Sum();
result.Dump("Part 1").Should().BeOneOf(161, 188192787);

// Part 2
matches = Regex.Matches(input, @"mul\((\d{1,3}),(\d{1,3})\)|don't\(\)|do\(\)");
result = matches.Aggregate(
	(true, 0),
	(x, r) => (x.Item1, x.Item2, r) switch
	{
		(_, _, _) when r.Value == "don't()" => (false, x.Item2),
		(_, _, _) when r.Value == "do()" => (true, x.Item2),
		(true, _, _) => (true, (x.Item2 + Convert.ToInt32(r.Groups[1].Value) * Convert.ToInt32(r.Groups[2].Value))),
		_ => x,
	})
	.Item2;
result.Dump("Part 2").Should().BeOneOf(48, 113965544);
