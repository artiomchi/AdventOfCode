#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var keypadMapSrc = """
789
456
123
 0A
""";
var arrowsMapSrc = """
 ^A
<v>
""";

var input = FileHelpers.ReadInputText("21.txt");

// Part 1
var sw = Stopwatch.StartNew();
Assert(input, 3).DumpAndAssert("Part 1", 126384, 231564);
var part1Time = sw.Elapsed;

// Part 2
sw.Restart();
Assert(input, 25 + 1).DumpAndAssert("Part 2", 154115708116294, 281212077733592);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(part1Time, part2Time);

long Assert(string input, int robotCount)
{
    var robots = Enumerable.Range(0, robotCount + 1)
        .Aggregate(
            new List<Robot>(),
            (l, i) =>
            {
                l.Add(new Robot(i == robotCount ? keypadMapSrc : arrowsMapSrc, l.Count > 0 ? l[^1] : null));
                return l;
            });
    var doorRobot = robots[^1];

    long result = 0;
    foreach (var combo in input.Split(Environment.NewLine))
    {
        var moves = doorRobot.Enter(combo);
        var nums = int.Parse(new string(combo.Where(c => char.IsDigit(c)).ToArray()));
        result += nums * moves;
    }
    return result;
}

public class Robot
{
    private readonly Dictionary<(char from, char to), long> _cache = new();
    private readonly Map _map;
    private readonly Robot? _chain;
    private readonly Dictionary<char, Point> _positions;

    public bool Bottom => _chain is null;

    public Robot(string source, Robot? chain = null)
    {
        _map = Map.FromString(source);
        _chain = chain;
        _positions = source.Except(['\n', '\r']).ToDictionary(c => c, c => _map.Find(c));
    }

    public long Enter(string keys)
    {
        if (_chain is null)
        {
            return keys.Length;
        }

        var pos = 'A';
        var result = 0L;
        foreach (var key in keys)
        {
            result += EnterKey(pos, key);
            pos = key;
        }
        return result;
    }

    private long EnterKey(char pos, char key)
    {
        if (_chain is null) return 1;

        if (_cache.TryGetValue((pos, key), out var length))
            return length;

        var posPt =  _positions[pos];
        var destPt = _positions[key];
        var vector = destPt - posPt;

        var moveX = new string(vector.X > 0 ? '>' : '<', Math.Abs((int)vector.X));
        var moveY = new string(vector.Y > 0 ? 'v' : '^', Math.Abs((int)vector.Y));

        (_, length) =
            new[]
            {
                _map[posPt.X, destPt.Y] != ' ' ? $"{moveY}{moveX}A" : null,
                _map[destPt.X, posPt.Y] != ' ' ? $"{moveX}{moveY}A" : null,
            }
            .Where(x => x != null)
            .Distinct()
            .Select(o => (o!, l: _chain.Enter(o!)))
            .OrderBy(x => x.l)
            .First();

        _cache[(pos, key)] = length;
        return length;
    }
}
