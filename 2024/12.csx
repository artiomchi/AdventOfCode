#nullable enable
#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;

var lines = File.ReadAllLines("2024/inputs/12.real.txt");

var map = lines.Index()
	.Select(x => x.Item.Index().Select(y => new Cell(x.Index, y.Index, y.Item)).ToArray()).ToArray();
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
			.SelectMany(c => Enum.GetValues<Dir>()
				.Select(d => (dir: d, cell: GetCell(map, c.P, d)))
				.Where(t => t.cell == null || t.cell.Plant != map[x][y].Plant)
				.Select(t => t.dir switch
				{
					Dir.L => (a: (c.P.x, c.P.y), b: (c.P.x, c.P.y + 1)),
					Dir.U => (a: (c.P.x, c.P.y), b: (c.P.x + 1, c.P.y)),
					Dir.R => (a: (c.P.x + 1, c.P.y), b: (c.P.x + 1, c.P.y + 1)),
					_ => (a: (c.P.x, c.P.y + 1), b: (c.P.x + 1, c.P.y + 1)),	
				})
				.Select(t => Ord(t)))
			.ToList();
		var add = groupCells.Length * fencePieces.Count;
		totalByPieces += add;

		add = 0;
		var sides = 0;
		while (fencePieces.Count > 0)
		{
			var turns = 0;
			var current = fencePieces.OrderBy(t => t.a.x).ThenBy(t => t.a.y).ThenBy(x => x.hor).First();
			fencePieces.Remove(current);
			var edge = current.b;
			var prev = current;
			var start = current.a;
			var dir = Dir.R;
			do
			{
				var newDir = dir;
				Line? next;
				Point newEdge;
				do {
					newDir = Rotate(newDir);
					newEdge = Move(edge, newDir);
					next = fencePieces
						.Where(p => (p.a == edge && p.b == newEdge) || (p.a == newEdge && p.b == edge))
						.Select(p => (Line?)p)
						.FirstOrDefault();
				}
				while (next is null);
				fencePieces.Remove(next.Value);
				if (current.hor != next.Value.hor)
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

		//$"{cell.Plant} size: {groupCells.Length}, sides: {sides}, total: {add}".Dump();
	}

totalByPieces.Dump("Part 1").Should().BeOneOf(140, 772, 1930, 1477762, 1184);
totalBySides.Dump("Part 2").Should().BeOneOf(80, 436, 368, 1206, 923480);

enum Dir { U, D, L, R }
static Point Vector(Dir direction)
	=> direction switch
	{
		Dir.L => (-1, 0),
		Dir.U => (0, -1),
		Dir.R => (1, 0),
		_ => (0, 1),
	};
static Dir Rotate(Dir direction)
	=> direction switch
	{
		Dir.R => Dir.D,
		Dir.D => Dir.L,
		Dir.L => Dir.U,
		_ => Dir.R
	};
static Point Move(Point point, Dir direction, int length = 1)
	=> (point.x + Vector(direction).x * length, point.y + Vector(direction).y * length);
static Line Ord(Line line)
	=> line.a.x < line.b.x
	? line
	: line.a.x == line.b.x && line.a.y < line.b.y
	? line
	: (line.b, line.a);

static Cell? GetCell(Cell[][] map, Point point, Dir direction)
{
	return direction switch
	{
		Dir.L when point.x > 0 => map[point.x - 1][point.y],
		Dir.U when point.y > 0 => map[point.x][point.y - 1],
		Dir.R when point.x < map.Length - 1 => map[point.x + 1][point.y],
		Dir.D when point.y < map[0].Length - 1 => map[point.x][point.y + 1],
		_ => null
	};
}

static IEnumerable<Cell> LinkedCells(Cell[][] map, Point point)
{
	return Enum.GetValues<Dir>()
		.Select(d => GetCell(map, point, d))
		.Where(d => d?.Plant == map[point.x][point.y].Plant && d.Group == null)
		.SelectMany(c =>
		{
			c!.Group = map[point.x][point.y].Group;
			return LinkedCells(map, c.P).Append(c);
		});
}

// You can define other methods, fields, classes and namespaces here
// record struct Point(int x, int y)
// {
// 	public static implicit operator Point((int x, int y) p) => new Point(p.x, p.y);
// }

record struct Line(Point a, Point b)
{
	public bool hor => a.y == b.y;
	public static implicit operator Line((Point a, Point b) l) => new Line(l.a, l.b);
}

class Cell(int x, int y, char plant)
{
	public Point P { get; set; } = (x, y);
	public char Plant { get; set; } = plant;
	public Guid? Group { get; set; }
}
