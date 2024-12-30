#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;

var equasions = File.ReadAllLines("2024/inputs/07.real.txt").Select(l => l.Split([':', ' '], StringSplitOptions.RemoveEmptyEntries).Select(i => Convert.ToInt64(i)).ToArray()).ToArray();

long result = 0;
foreach (var equasion in equasions)
{
	if (Validate(equasion[0], equasion[1], equasion[2..]))
	{
		result += equasion[0];
		continue;
	}
}
result.Dump("Part 1").Should().BeOneOf(3749, 5540634308362);


result = 0;
foreach (var equasion in equasions)
{
	if (Validate(equasion[0], equasion[1], equasion[2..], 3))
	{
		result += equasion[0];
		continue;
	}
}
result.Dump("Part 2").Should().BeOneOf(11387, 472290821152397);

static bool Validate(long expected, long acc, ReadOnlySpan<long> values, int operations = 2)
{
	if (acc > expected)
		return false;
	if (values.Length == 0)
		return expected == acc;
		
	var next = values[0];
	var remaining = values[1..];
	
	return
		Validate(expected, acc + next, remaining, operations) ||
		Validate(expected, acc * next, remaining, operations) ||
		(operations > 2 ? Validate(expected, acc * (long)Math.Pow(10, next.ToString().Length) + next, remaining, operations) : false);
}
