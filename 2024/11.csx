#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;
using System.Numerics;

var input = File.ReadAllText("2024/inputs/11.real.txt");

var data = input.Split(' ')
	.Select(s => Convert.ToInt64(s))
	.GroupBy(s => s)
	.ToDictionary(g => g.Key, g => (long)g.Count());
	
for (var i = 0; i < 75; i++)
{
	var newData = new Dictionary<long, long>();
	foreach (var stone in data.Keys)
	{
		var newStones = stone switch
		{
			0 => [1],
			_ when Math.Log10(stone) % 2 >= 1 => split(stone),
			_ => [stone * 2024]
		};
		foreach (var num in newStones)
		{
			newData[num] = newData.TryGetValue(num, out var count) ? count + data[stone] : data[stone];
		}
	}
	data = newData;

	if (i == 24)
		data.Values.Sum().Dump("Part 1").Should().BeOneOf(55312, 186424);
}

data.Values.Sum().Dump("Part 2").Should().BeOneOf(219838428124832);

long[] split(long number)
{
	var splitNum = Convert.ToInt64(Math.Pow(10, Math.Floor(Math.Log10(number) + 1) / 2));
	return [number / splitNum, number % splitNum];
}
