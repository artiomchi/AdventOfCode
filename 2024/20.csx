#load "..\Helpers.csx"

var input = ReadInputText("20.real.txt");

var sw = Stopwatch.StartNew();
var map = Map.FromString(input.Replace('.', ' '));

var current = map.Find('S');
var steps = new List<Point> { current };
while (map[current] != 'E')
{
    current = Vector.Directions
        .Select(d => current.Move(d))
        .Where(p => map[p] != '#')
        .Where(p => steps.Count == 1 || p != steps[^2])
        .Single();
    steps.Add(current);
    //map[current] = '.';
    //mapContainer.UpdateContent(map.ToPrettyMap());
    //await Task.Delay(1);
}
//mapContainer.UpdateContent(map.ToPrettyMap());
var positions = steps.ToDictionary(s => s, s => steps.IndexOf(s));
var parseTime = sw.Elapsed;

// Part 1
sw.Restart();
var skips1 = new List<int>();
foreach (var (i, pos) in steps.Take(steps.Count - 3).Index())
{
    var jumps = Vector.Directions
        .Select(d => pos.Move(d))
        .Where(p => map[p] == '#' && 0 < p.X && p.X < map.Height - 1 && 0 < p.Y && p.Y < map.Width - 1)
        .SelectMany(p1 => Vector.Directions
            .Select(d => p1.Move(d))
            .Where(p2 => map[p2] is not '#')
            .Select(p2 => (p1, p2, skip: positions[p2] - i - 2))
            .Where(x => x.skip > 0))
        .ToArray();

    foreach (var (p1, p2, skip) in jumps)
    {
        skips1.Add(skip);
        //$"Jump {pos} => {p1} => {p2} skips {skip}".Dump();
    }
}

//map.Matrix.Dump();
// skips1.GroupBy(i => i)
//     .OrderBy(g => g.Key)
//     .Select(g => $"There are {g.Count()} cheats that save {g.Key} picoseconds.")
//     .ToArray()
//     .Dump(0);

skips1.Count(i => i >= 100).DumpAndAssert("Part 1", 0, 1393);
var part1Time = sw.Elapsed;

// Part 2
sw.Restart();
var skips2 = new List<(Point p1, Point p2, int skips)>();
foreach (var (i, pos) in steps.Take(steps.Count - 3).Index())
{
    for (var x = -21; x <= 21; x++)
        for (var y = -21; y <= 21; y++)
        {
            var moved = Math.Abs(x) + Math.Abs(y);
            if (moved < 2 || moved > 20)
                continue;

            var newPos = pos + (x, y);
            if (!newPos.IsInBounds(map) || map[newPos] is 'S' or '#')
                continue;

            var skip = positions[newPos] - i - moved;
            if (skip <= 0)
                continue;

            skips2.Add((pos, newPos, skip));
        }
}

//var debugContainer = new DumpContainer();

// skips2
//     .Select(s => s.skips)
//     .Where(i => i >= 50)
//     .GroupBy(i => i)
//     .OrderBy(g => g.Key)
//     .Select(g =>
//     {
//         var text = $"There are {g.Count()} cheats that save {g.Key} picoseconds.";
//         var link = new Hyperlinq(
//             () =>
//             {
//                 debugContainer.ClearContent();
//                 foreach (var (i, (p1, p2, _)) in skips2.Where(s => s.skips == g.Key).Index())
//                 {
//                     map.Reset();
//                     map[p1] = '1';
//                     map[p2] = '2';
//                     debugContainer.AppendContent(i + 1);
//                     debugContainer.AppendContent(map.ToPrettyMap());
//                 }
//             },
//             text);
//         return link;
//     })
//     .ToArray()
//     .Dump(0);

skips2.Select(s => s.skips).Count(i => i >= 100).DumpAndAssert("Part 2", 0, 990096);
var part2Time = sw.Elapsed;

PrintTimings(parseTime, part1Time, part2Time);

//debugContainer.Dump("Debug");

// partial class Map
// {
//     public object ToPrettyMap()
//         => Util.WithStyle(
//             Util.FixedFont(ToPrettyString(c => c switch
//             {
//                 'S' or 'E' => "orange",
//                 '1' or '2' => "yellow",
//                 '·' or 'X' => "orange",
//                 'B' => "red",
//                 '#' => "grey",
//                 '^' or '>' or '<' or 'v' => "green",
//                 _ => null
//             })),
//             "line-height: 9px; display: block");
// }
