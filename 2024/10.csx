#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;

var lines = File.ReadAllLines("2024/inputs/10.real.txt");
var map = new int[lines.Length,lines[0].Length];
for (var x = 0; x < lines.Length; x++)
	for (var y = 0; y < lines[0].Length; y++)
		map[x,y] = lines[x][y] - '0';
		
var heads = new HashSet<(int x, int y)>();
for (var x = 0; x < map.GetLength(0); x++)
	for (var y = 0; y < map.GetLength(1); y++)
		if (map[x,y] == 0)
			heads.Add((x, y));

heads.Select(h => countScore(map, h, 1)).Sum().Dump("Part 1").Should().BeOneOf(36, 825);
heads.Select(h => countScore(map, h, 2)).Sum().Dump("Part 2").Should().BeOneOf(81, 1805);

static int countScore(int[,] map, (int x, int y) head, int part)
{
	var positions = new List<(int, int)>([head]);
	for (var i = 1; i <= 9; i++)
	{
		var newPositions = new List<(int, int)>();
		foreach (var (x, y) in positions)
		{
			(int x, int y)[] directions = [(-1, 0), (0, -1), (1, 0), (0, 1)];
			foreach (var (a, b) in directions)
			{
				if (x + a < 0 || y + b < 0 || x + a >= map.GetLength(0) || y + b >= map.GetLength(1))
					continue;
					
				if (map[x + a, y + b] == i)
					newPositions.Add((x + a, y + b));
			}
		}
		
		positions = (part == 1 ? newPositions.Distinct() : newPositions).ToList();
	}
	
	return positions.Count();
}