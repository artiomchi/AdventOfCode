#!/usr/bin/env dotnet
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var lines = FileHelpers.ReadInputLines("12.txt");
var sw = Stopwatch.StartNew();
var map = lines.Index()
    .Select(x => x.Item.Index().Select(y => new Cell(x.Index, y.Index, y.Item)).ToArray()).ToArray();
var parseTime = sw.Elapsed;

sw.Restart();
var groups = new Dictionary<Guid, int>();
var totalByPieces = 0;
var totalBySides = 0;
foreach (var (x, row) in map.Index())
    foreach (var (y, cell) in row.Index())
    {
        if (cell.Group != null)
            continue;

        cell.Group = Guid.NewGuid();
        var groupCells = LinkedCells(map, (x, y)).Append(cell).ToArray();

        var fencePieces = groupCells
            .SelectMany(c => Vector.Directions
                .Select(d => (dir: d, cell: GetCell(map, c.P, d)))
                .Where(t => t.cell == null || t.cell.Plant != map[x][y].Plant)
                .Select(t => (Line)(t.dir switch
                {
                    (-1, 0) => (c.P, c.P + (0, 1)),
                    (0, -1) => (c.P, c.P + (1, 0)),
                    (1, 0) => (c.P + (1, 0), c.P + (1, 1)),
                    _ => (c.P + (0, 1), c.P + (1, 1)),
                }))
                .Select(t => t.Normalise()))
            .ToList();
        var add = groupCells.Length * fencePieces.Count;
        totalByPieces += add;

        add = 0;
        var sides = 0;
        while (fencePieces.Count > 0)
        {
            var turns = 0;
            var current = fencePieces.OrderBy(t => t.A.X).ThenBy(t => t.A.Y).ThenBy(x => x.IsHorizontal).First();
            fencePieces.Remove(current);
            var edge = current.B;
            var prev = current;
            var start = current.A;
            Vector dir = '>';
            do
            {
                var newDir = dir;
                Line? next;
                Point newEdge;
                do {
                    newDir = newDir.Rotate();
                    newEdge = edge.Move(newDir);
                    next = fencePieces
                        .Where(p => (p.A == edge && p.B == newEdge) || (p.A == newEdge && p.B == edge))
                        .Select(p => (Line?)p)
                        .FirstOrDefault();
                }
                while (next is null);
                fencePieces.Remove(next.Value);
                if (current.IsHorizontal != next.Value.IsHorizontal)
                    turns++;
                prev = current;
                current = next.Value;
                edge = newEdge;
                dir = newDir;
            }
            while (start != edge);
            add += groupCells.Length * (turns + 1);
            sides += turns + 1;
        }
        totalBySides += add;
    }

totalByPieces.DumpAndAssert("Part 1", 140, 772, 1930, 1477762, 1184);
totalBySides.DumpAndAssert("Part 2", 80, 436, 368, 1206, 923480);
var partsTime = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, partsTime);

static Cell? GetCell(Cell[][] map, Point point, Vector direction)
{
    point = point.Move(direction);
    return point.IsInBounds(map.Length, map[0].Length)
        ? map[point.X][point.Y]
        : null;
}

static IEnumerable<Cell> LinkedCells(Cell[][] map, Point point)
{
    return Vector.Directions
        .Select(d => GetCell(map, point, d))
        .Where(c => c?.Plant == map[point.X][point.Y].Plant && c.Group == null)
        .SelectMany(c =>
        {
            c!.Group = map[point.X][point.Y].Group;
            return LinkedCells(map, c.P).Append(c);
        });
}

readonly record struct Line(Point A, Point B)
{
    public bool IsHorizontal => A.Y == B.Y;

    public static implicit operator Line((Point a, Point b) l) => new(l.a, l.b);

    public Line Normalise()
        => A.X < B.X || (A.X == B.X && A.Y < B.Y)
        ? this
        : (B, A);
}

class Cell(int x, int y, char plant)
{
    public Point P { get; set; } = (x, y);
    public char Plant { get; set; } = plant;
    public Guid? Group { get; set; }
}

static class LocalExtensions
{
    extension(Vector vector)
    {
        public Vector Rotate()
            => vector switch
            {
                (1, 0) => 'v',
                (0, 1) => '<',
                (-1, 0) => '^',
                _ => '>'
            };
    }
}
