#!/usr/bin/env dotnet
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var input = FileHelpers.ReadInputText("16.txt").Replace('.', ' ');
var sw = Stopwatch.StartNew();
var map = Map.FromString(input);
var start = map.Find('S');
var end = map.Find('E');
var metaMap = new CellState[map.Width, map.Height];
metaMap.Initialize();
var parseTime = sw.Elapsed;

sw.Restart();
var bestScore = int.MaxValue;
var bestPaths = new Dictionary<(Point, Vector), (int Score, Deer Deer)>();
var deers = new List<Deer> { new(start) };
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
            .Where(x => !bestPaths.TryGetValue((x.p, x.d), out var b) || deer.ScorePlus(x.d) <= b.Score)
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
            if (map[nextPos] == 'E')
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

    foreach (var point in bestPaths.Keys.Select(k => k.Item1).Except([start, end]).Distinct())
    {
        map[point] = '.';
    }
    var targetScore = Math.Min(bestScore, deers.Where(d => d.Running).Min(d => (int?)d.Score) ?? int.MaxValue);
    foreach (var point in deers.Where(d => d.Score == targetScore).SelectMany(d => d.Steps).Except([start, end]).Distinct())
    {
        map[point] = 'X';
    }
    foreach (var point in deers.Where(d => d.Running).Select(d => d.Steps[^1]).Except([start, end]))
    {
        map[point] = 'W';
    }
} while (deers.Any(d => d.Running));
var runTime = sw.Elapsed;

// Part 1
sw.Restart();
var (_, bestDeer) = bestPaths.Where(p => p.Key.Item1 == end).OrderBy(x => x.Value.Score).First().Value;
bestScore.DumpAndAssert("Part 1", 11048, 85420);
var part1Time = sw.Elapsed;

// Part 2
sw.Restart();
var bestDeers = deers.Where(d => d.Steps[^1] == end && d.Score == bestDeer.Score);
bestDeers.SelectMany(d => d.Steps).Distinct().Count().DumpAndAssert("Part 2", 64, 492);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, runTime, part1Time, part2Time);

class Deer(List<Point> steps, Vector direction, int score)
{
    public Deer(Point start) : this([start], '>', 0) { }
    public Deer(Deer source) : this(new(source.Steps), source.Direction, source.Score) {}

    public int Id { get; set; }
    public List<Point> Steps { get; set; } = steps;
    public Vector Direction { get; set; } = direction;
    public int Score { get; set; } = score;
    public bool Running { get; set; } = true;

    public int ScorePlus(Vector direction) => Score + (direction != Direction ? 1001 : 1);
}

class CellState
{
    public int ActiveRuns { get; set; }
    public int MinScore { get; set; }
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
//                 '#' => "grey",
//                 '^' or '>' or '<' or 'v' => "green",
//                 _ => null
//             })),
//             "line-height: 9px; display: block");
// }
