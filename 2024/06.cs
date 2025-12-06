#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var lines = FileHelpers.ReadInputLines("06.txt");
var sw = new Stopwatch();

// Part 1
sw.Restart();
var matrix = GetMap();
SimulateRun(matrix);
matrix.Sum(l => l.Count(p => char.IsNumber(p))).DumpAndAssert("Part 1", 41, 5404);
var part1Time = sw.Elapsed;

// Part 2
sw.Restart();
var loops = 0;
for (int i = 1; i < matrix.Length - 1; i++)
    for (int j = 1; j < matrix[0].Length - 1; j++)
    {
        matrix = GetMap();
        if (matrix[i][j] == '^')
            continue;

        matrix[i][j] = '#';
        if (!SimulateRun(matrix))
            loops++;
    }
loops.DumpAndAssert("Part 2", 6, 1984);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(part1Time, part2Time);

// Helpers
char[][] GetMap() => lines
    .Prepend(new string('|', lines[0].Length))
    .Append(new string('|', lines[0].Length))
    .Select(s => ('|' + s + '|').ToArray()).ToArray();
static (int x, int y) GetPos(char[][] matrix) => matrix.Index().Where(m => m.Item.Contains('^')).Select(x => (x.Index, x.Item.Index().First(c => c.Item == '^').Index)).First();

static bool SimulateRun(char[][] matrix)
{
    var dir = (x: -1, y: 0);
    var pos = GetPos(matrix);
    while (true)
    {
        var steps = int.TryParse(matrix[pos.x][pos.y].ToString(), out var st) ? st + 1 : 1;
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
