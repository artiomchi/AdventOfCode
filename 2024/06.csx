#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;

var lines = File.ReadAllLines("2024/inputs/06.real.txt");

// Part 1
var matrix = getMap();
simulateRun(matrix);
var steps = matrix.Sum(l => l.Count(p => char.IsNumber(p))).Dump("Part 1").Should().BeOneOf(41, 5404);

// Part 2
var sw = Stopwatch.StartNew();
var loops = 0;
for (int i = 1; i < matrix.Length - 1; i++)
	for (int j = 1; j < matrix[0].Length - 1; j++)
	{
		matrix = getMap();
		if (matrix[i][j] == '^')
			continue;
			
		matrix[i][j] = '#';
		if (!simulateRun(matrix))
			loops++;
	}
loops.Dump("Part 2").Should().BeOneOf(6, 1984);
sw.Elapsed.Dump();

// Funcs
char[][] getMap() => lines
	.Prepend(new string('|', lines[0].Length))
	.Append(new string('|', lines[0].Length))
	.Select(s => ('|' + s + '|').ToArray()).ToArray();
static (int x, int y) getPos(char[][] matrix) => matrix.Index().Where(m => m.Item.Contains('^')).Select(x => (x.Index, x.Item.Index().First(c => c.Item == '^').Index)).First();

static bool simulateRun(char[][] matrix)
{
	var dir = (x: -1, y: 0);
	var pos = getPos(matrix);
	while (true)
	{
		var steps = Int32.TryParse(matrix[pos.x][pos.y].ToString(), out var st) ? st + 1 : 1;
		if (steps > 4)
			return false;
		matrix[pos.x][pos.y] = steps.ToString()[0];
		while (matrix[pos.x + dir.x][pos.y + dir.y] == '#')
		{
			dir = (dir.y, -dir.x);
		}
		pos = (pos.x + dir.x, pos.y + dir.y);
		if (matrix[pos.x][pos.y] == '|')
			break;
	}
	
	return true;
}