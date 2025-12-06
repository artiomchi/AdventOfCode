#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

//int steps = 12, mapSize = 7;
int steps = 1024, mapSize = 71;
var lines = FileHelpers.ReadInputLines("18.txt");

var sw = Stopwatch.StartNew();
var map = new Map(mapSize + 2, mapSize + 2);
for (var i = 0; i < map.Width; i++)
for (var j = 0; j < map.Height; j++)
map[i, j] = i == 0 || j == 0 || i == map.Width - 1 || j == map.Height - 1
    ? '#'
    : ' ';
var parseTime = sw.Elapsed;

// Part 1
sw.Restart();
var fallingBytes = lines
    .Select(l => l.Split(',').Select(int.Parse).ToArray())
    .Select(x => (x: x[1], y: x[0]))
    .ToArray();

foreach (var (x, y) in fallingBytes.Take(steps))
{
    map[x + 1, y + 1] = '#';
}

//var mapContainer = new DumpContainer(map.ToPrettyMap()).Dump();

var start = (1, 1);
var end = (map.Width - 2, map.Height - 2);



var bestDeer = TryEscape();
bestDeer!.Score.DumpAndAssert("Part 1", 22, 246);
var part1Time = sw.Elapsed;

// Part 2
sw.Restart();
foreach (var (x, y) in fallingBytes.Skip(steps))
{
    Point point = (x + 1, y + 1);
    map[point] = '#';

    if (bestDeer.Steps.Contains(point))
        bestDeer = TryEscape(point);

    if (bestDeer == null)
    {
        $"{y},{x}".DumpAndAssert("Part 2", "6,1", "22,50");
        break;
    }
}
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);

Deer TryEscape(Point? lastAdded = null)
{
    var bestScore = int.MaxValue;
    var bestPaths = new Dictionary<(Point, Vector), (int Score, Deer Deer)>();
    var deers = new List<Deer> { new Deer(start) };
    do
    {
        var newDeers = new List<Deer>();
        foreach (var deer in deers)
        {
            if (!deer.Running)
                continue;
            if (bestScore < deer.Score)
            {
                deer.Running = false;
                continue;
            }

            var validDirections = Vector.Directions
                .Select(d => (d, p: deer.Steps[^1].Move(d)))
                .Where(x => !deer.Steps.Contains(x.p) && !(map[x.p] is '#' or 'S'))
                .Where(x => !bestPaths.TryGetValue((x.p, x.d), out var b) || deer.ScorePlus(x.d) < b.Score)
                .ToArray();
            if (validDirections.Length == 0)
            {
                deer.Running = false;
                continue;
            }

            var options = validDirections.Index().Select(x => (x.Index, x.Item.d, x.Item.p, x.Index == 0 ? deer : new Deer(deer))).ToArray();
            foreach (var (i, dir, nextPos, deer2) in options)
            {
                if (i > 0)
                {
                    deer2.Id = deers.Count + newDeers.Count;
                    newDeers.Add(deer2);
                }

                deer2.Steps.Add(nextPos);
                deer2.Score = deer2.ScorePlus(dir);
                deer2.Direction = dir;
                if (nextPos == end)
                {
                    bestScore = Math.Min(bestScore, deer2.Score);
                    deer2.Running = false;
                }
                if (bestPaths.TryGetValue((nextPos, dir), out var best))
                {
                    if (deer2.Score < best.Score)
                    {
                        best.Deer.Running = false;
                        bestPaths[(nextPos, dir)] = (deer2.Score, deer2);
                    }
                }
                else
                {
                    bestPaths[(nextPos, dir)] = (deer2.Score, deer2);
                }
            }
        }
        deers.AddRange(newDeers);
        //await Task.Delay(5);
    } while (deers.Any(d => d.Running));

    var bestDeer = bestPaths
        .Where(p => p.Key.Item1 == end)
        .OrderBy(x => x.Value.Score)
        .Select(x => x.Value.Deer)
        .FirstOrDefault();
    if (bestDeer != null)
    {
        PrintMap();
    }
    else if (lastAdded != null)
    {
        map[lastAdded.Value] = 'B';
        //mapContainer.UpdateContent(map.ToPrettyMap());
    }
    return bestDeer;

    void PrintMap()
    {
        map.Fill(' ', ['#']);
        //mapContainer.UpdateContent(map.ToPrettyMap());
        var targetScore = Math.Min(bestScore, deers.Where(d => d.Running).Min(d => (int?)d.Score) ?? int.MaxValue);
        foreach (var point in deers.Where(d => d.Score == targetScore).Take(1).SelectMany(d => d.Steps).Except([start, end]).Distinct())
        {
            map[point] = 'X';
        }
        //mapContainer.UpdateContent(map.ToPrettyMap());
        foreach (var point in deers.Where(d => d.Running).Select(d => d.Steps[^1]).Except([start, end]))
        {
            map[point] = 'W';
        }
        //mapContainer.UpdateContent(map.ToPrettyMap());
    }
}


class Deer(List<Point> steps, Vector direction, int score)
{
    public Deer(Point start) : this([start], '>', 0) { }
    public Deer(Deer source) : this(new(source.Steps), source.Direction, source.Score) { }

    public int Id { get; set; }
    public List<Point> Steps { get; set; } = steps;
    public Vector Direction { get; set; } = direction;
    public int Score { get; set; } = score;
    public bool Running { get; set; } = true;

    public int ScorePlus(Vector direction) => Score + 1;
}
// partial class Map
// {
//     public object ToPrettyMap2()
//         => ToImage(200);

//     public object ToPrettyMap()
//         => Util.WithStyle(
//             Util.FixedFont(ToPrettyString(c => c switch
//             {
//                 'S' or 'E' => "orange",
//                 'W' => "yellow",
//                 '·' or 'X' => "orange",
//                 'B' => "red",
//                 '#' => "grey",
//                 '^' or '>' or '<' or 'v' => "green",
//                 _ => null
//             })),
//             "line-height: 9px; display: block");
// }
