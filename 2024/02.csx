#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;

var input = ReadInputLines("02.real.txt")
	.Select(line => line.Trim()
		.Split(' ', StringSplitOptions.RemoveEmptyEntries)
		.Select(s => Convert.ToInt32(s))
		.ToArray())
	.ToArray();

// Part 1
var results = input.Select(v => ValidateLevel(v));
var result = results.Count(r => r).Dump("Part 1").Should().BeOneOf(2, 534);

// Part 2
results = input.Select(v => ValidateLevelWithDampener(v));
results.Count(r => r).Dump("Part 2").Should().BeOneOf(4, 577);


static bool ValidateLevel(int[] values)
	=> values.Aggregate(
		((int?)null, (bool?)null),
		(r, v) => (r.Item1, r.Item2, v) switch
		{
			( < 0, _, _) => (-1, null),
			(null, null, _) => (v, null),
			({ }, _, _) when r.Item1 == v => (-2, null),
			({ }, _, _) when Math.Abs(r.Item1.Value - v) > 3 => (-3, null),
			({ }, null, _) => (v, r.Item1 < v),
			({ }, { }, _) when r.Item1 < v != r.Item2 => (-4, null),
			_ => (v, r.Item2)
		}).Item1 >= 0;
		
static bool ValidateLevelWithDampener(int[] values)
	=> ValidateLevel(values) ? true : Enumerable.Range(0, values.Length).Any(i => { var result = values.ToList(); result.RemoveAt(i); return ValidateLevel(result.ToArray()); });
