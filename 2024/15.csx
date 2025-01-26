#load "..\Helpers.csx"
using System.Text.RegularExpressions;

const char Empty = ' ';
var lines = ReadInputText("15.real.txt").Replace('.', Empty).Split(Environment.NewLine);
var sw = Stopwatch.StartNew();

var mapLines = lines.TakeWhile(l => l.Length > 0).ToArray();
var operations = lines.Skip(mapLines.Length + 1).SelectMany(c => c).ToArray();
var parseTime = sw.Elapsed;

// Part 1
sw.Restart();
var map = Map.FromLines(mapLines);
Process(map, operations, false).DumpAndAssert("Part 1", 2028, 10092, 1509074);
var part1Time = sw.Elapsed;

// Part 2
sw.Restart();
map = new Map(mapLines[0].Length * 2, mapLines.Length);
foreach (var (y, line) in mapLines.Index())
    foreach (var (x, c) in line.Index())
    {
        map[x * 2, y] = c switch { 'O' => '[', _ => c };
        map[x * 2 + 1, y] = c switch { 'O' => ']', '@' => Empty, _ => c };
    }
Process(map, operations, true).DumpAndAssert("Part 2", 9021, 1521453);
var part2Time = sw.Elapsed;

PrintTimings(parseTime, part1Time, part2Time);

static int Process(Map map, char[] operations, bool part2)
{
    var robot = map.Find('@');

    foreach (var operation in operations)
    {
        var moved = MapMove(map, robot, operation);
        if (moved)
        {
            robot = robot.Move(operation);
        }
    }

    var sum = 0;
    for (var x = 1; x < map.Width; x++)
        for (var y = 1; y < map.Height; y++)
            if (map[x,y] is '[' or 'O')
                sum += x + 100 * y;
    return sum;
}

static bool MapMove(Map map, Point position, char direction)
{
    var newPosition = position.Move(direction);
    var canMove = map[newPosition.X, newPosition.Y] switch
    {
        '#' => false,
        '.' or ' ' => true,
        'O' => CanMove(map, [newPosition], direction),
        '[' => CanMove(map, [newPosition, newPosition.Move('>')], direction),
        ']' => CanMove(map, [newPosition.Move('<'), newPosition], direction),
        '@' => throw new InvalidOperationException("Robot already at destination?"),
        _ => throw new InvalidOperationException("What is this in my destination? " + map[newPosition]),
    };

    if (canMove)
    {
        var movePoints = map[newPosition] switch
        {
            '.' or ' ' => Array.Empty<Point>(),
            'O' => [newPosition],
            '[' => [newPosition, newPosition.Move('>')],
            ']' => [newPosition.Move('<'), newPosition],
            _ => throw new InvalidOperationException("Can't move here"),
        };
        SafeMove(map, movePoints, direction);

        // Move the robot
        map[newPosition] = map[position];
        map[position] = Empty;
    }

    return canMove;
}

static bool CanMove(Map map, Point[] positions, char direction)
{
    if (positions.Select(p => p.Move(direction)).Any(p => map[p] == '#'))
        return false;

    var positionsBehind = ExpandPositions(map, positions, direction);
    if (positionsBehind.Length == 0)
        return true;
    return CanMove(map, positionsBehind, direction);
}

static void SafeMove(Map map, Point[] positions, char direction)
{
    if (positions.Length == 0) return;

    var positionsBehind = ExpandPositions(map, positions, direction);
    SafeMove(map, positionsBehind, direction);

    var moveData = positions
        .Select(p => (p.Move(direction), map[p]))
        .ToArray();
    foreach (var (point, value) in moveData)
    {
        map[point] = value;
    }

    foreach (var point in positions.Except(moveData.Select(m => m.Item1)))
    {
        map[point] = Empty;
    }
}

static Point[] ExpandPositions(Map map, Point[] positions, char direction)
{
    return positions
        .Select(p => p.Move(direction))
        .SelectMany(p => map[p] switch
        {
            '.' or ' ' => Array.Empty<Point>(),
            'O' => [p],
            '[' => [p, p.Move('>')],
            ']' => [p.Move('<'), p],
            _ => throw new InvalidOperationException($"Immovable object? {map[p]} for direction {direction}")
        })
        .Except(positions)
        .Distinct()
        .ToArray();
}
