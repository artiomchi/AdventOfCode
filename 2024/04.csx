#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;

var matrix = File.ReadAllLines("2024/inputs/04.real.txt").Select(s => s.Trim().ToArray()).ToArray();

// Part 1
var matches = 0;
(int, int)[] directions = [(1, 0), (1, 1), (0, 1), (-1, 1), (-1, 0), (-1, -1), (0, -1), (1, -1)];
foreach (var (i, line) in matrix.Index())
	foreach (var (j, c) in line.Index())
	{
		if (c != 'X')
			continue;
			
		foreach (var (di, dj) in directions)
		{
			if (i + di * 3 >= matrix.Length || i + di * 3 < 0 ||
				j + dj * 3 >= line.Length || j + dj * 3 < 0)
				continue;

			if (matrix[i + di][j + dj] == 'M' && matrix[i + di * 2][j + dj * 2] == 'A' && matrix[i + di * 3][j + dj * 3] == 'S')
				matches++;
		}
	}
	
matches.Dump("Part 1").Should().BeOneOf(18, 2633);

// Part 2
matches = 0;
foreach (var (i, line) in matrix.Index())
	foreach (var (j, c) in line.Index())
	{
		if (i == 0 || j == 0 || i == matrix.Length - 1 || j == line.Length - 1 || c != 'A')
			continue;

		string[] mass = [
			new string([matrix[i - 1][j - 1], matrix[i + 1][j + 1]]),
			new string([matrix[i + 1][j + 1], matrix[i - 1][j - 1]]),
			new string([matrix[i - 1][j + 1], matrix[i + 1][j - 1]]),
			new string([matrix[i + 1][j - 1], matrix[i - 1][j + 1]]),
		];
		if (mass.Count(s => s == "MS") == 2)
			matches++;
	}
	
matches.Dump("Part 2").Should().BeOneOf(9, 1936);
