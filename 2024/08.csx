#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;

var map = File.ReadAllLines("2024/inputs/08.real.txt");
var antinodes1 = new bool[map.Length, map[0].Length];
var antinodes2 = new bool[map.Length, map[0].Length];

var frequencies = map
	.SelectMany((row, x) => row
		.Select((c, y) => (x, y, c)))
		.Where(x => char.IsLetterOrDigit(x.c))
	.ToLookup(x => x.c, x => (x.x, x.y));
var antennaSets = frequencies
	.SelectMany(g => g
		.SelectMany(a => frequencies[g.Key]
			.Where(b => b.x > a.x)
			.Select(b => (a, b))));
foreach (var (a, b) in antennaSets)
{
	var dist = (x: b.x - a.x, y: b.y - a.y);
	
	int x, y, mul = 0;
	while (inBounds(x = a.x - dist.x * mul, y = a.y - dist.y * mul))
	{
		if (mul == 1)
			antinodes1[x, y] = true;
		antinodes2[x, y] = true;
		mul++;
	}

	mul = 1;
	while (inBounds(x = a.x + dist.x * mul, y = a.y + dist.y * mul))
	{
		if (mul == 2)
			antinodes1[x, y] = true;
		antinodes2[x, y] = true;
		mul++;
	}
}

antinodes1.OfType<bool>().Count(n => n).Dump("Part 1").Should().BeOneOf(14, 329);
antinodes2.OfType<bool>().Count(n => n).Dump("Part 2").Should().BeOneOf(34, 1190);

// helpers
bool inBounds(int x, int y) { return x >= 0 && y >= 0 && x < map.Length && y < map[0].Length; }
