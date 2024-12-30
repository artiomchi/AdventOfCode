#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;
using System.Text.RegularExpressions;

var part2 = true;
var multiplier = part2 ? 2 : 1;
var empty = ' ';
var lines = File.ReadAllText("2024/inputs/15.real.txt").Replace('.', empty).Split(Environment.NewLine);

var mapLines = lines.TakeWhile(l => l.Length > 0).ToArray();
var map = new Map(mapLines[0].Length * multiplier, mapLines.Length);
var robot = new Point();
foreach (var (y, line) in mapLines.Index())
	foreach (var (x, c) in line.Index())
	{
		if (part2)
		{
			map[x * multiplier, y] = c switch { 'O' => '[', _ => c };
			map[x * multiplier + 1, y] = c switch { 'O' => ']', '@' => empty, _ => c };
		}
		else
		{
			map[x, y] = c;
		}
		
		if (c == '@')
			robot = (x * multiplier, y);
	}

var operations = lines.Skip(mapLines.Length + 1).SelectMany(c => c).ToArray();

//var mapContainer = new DumpContainer(map.ToPrettyMap()).Dump();
foreach (var (i, operation) in operations.Index())
{
	var moved = MapMove(map, robot, operation);
	if (moved)
	{
		robot = robot.Move(operation);
	}
	
	//mapContainer.UpdateContent(map.ToPrettyMap());
	//Util.Progress = i * 100 / operations.Length;
	if (i % 10 == 0)
		await Task.Delay(1);
}

var sum = 0;
for (var x = 1; x < map.Width; x++)
	for (var y = 1; y < map.Height; y++)
		if (map[x,y] is '[' or 'O')
			sum += x + 100 * y;
sum.Dump().Should().BeOneOf(part2 ? [9021, 1521453] : [2028, 10092, 1509074]);


bool MapMove(Map map, Point position, char direction)
{
	var newPosition = position.Move(direction);
	var canMove = map[newPosition.x, newPosition.y] switch
	{
		'#' => false,
		'.' or ' ' => true,
		'O' => CanMove(map, [newPosition], direction),
		'[' => CanMove(map, [newPosition, newPosition.Move('>')], direction),
		']' => CanMove(map, [newPosition.Move('<'), newPosition], direction),
		'@' => throw new InvalidOperationException("Robot already at destination?"),
		_ => throw new InvalidOperationException("What is this in my destination? " + map[newPosition.x, newPosition.y]),
	};
	
	if (canMove)
	{
		var movePoints = map[newPosition.x, newPosition.y] switch
		{
			'.' or ' ' => Array.Empty<Point>(),
			'O' => [newPosition],
			'[' => [newPosition, newPosition.Move('>')],
			']' => [newPosition.Move('<'), newPosition],
			_ => throw new InvalidOperationException("Can't move here"),
		};
		SafeMove(map, movePoints, direction);
		
		// Move the robot
		map[newPosition.x, newPosition.y] = map[position.x, position.y];
		map[position.x, position.y] = empty;
	}
	
	return canMove;
}

bool CanMove(Map map, Point[] positions, char direction)
{
	if (positions.Select(p => p.Move(direction)).Any(p => map[p.x, p.y] == '#'))
		return false;
	
	var positionsBehind = ExpandPositions(map, positions, direction);
	if (positionsBehind.Length == 0)
		return true;
	return CanMove(map, positionsBehind, direction);
}

void SafeMove(Map map, Point[] positions, char direction)
{
	if (positions.Length == 0) return;

	var positionsBehind = ExpandPositions(map, positions, direction);
	SafeMove(map, positionsBehind, direction);

	var moveData = positions
		.Select(p => (p.Move(direction), map[p.x, p.y]))
		.ToArray();
	foreach (var (point, value) in moveData)
	{
		map[point.x, point.y] = value;
	}

	foreach (var point in positions.Except(moveData.Select(m => m.Item1)))
	{
		map[point.x, point.y] = empty;
	}
}

Point[] ExpandPositions(Map map, Point[] positions, char direction)
{
	return positions
		.Select(p => p.Move(direction))
		.SelectMany(p => map[p.x, p.y] switch
		{
			'.' or ' ' => Array.Empty<Point>(),
			'O' => [p],
			'[' => [p, p.Move('>')],
			']' => [p.Move('<'), p],
			_ => throw new InvalidOperationException($"Immovable object? {map[p.x, p.y]} for direction {direction}")
		})
		.Except(positions)
		.Distinct()
		.ToArray();
}

partial class Map
{
	// public object ToPrettyMap()
	// 	=> Util.FixedFont(
	// 		this.ToPrettyString(c => c switch
	// 		{
	// 			'@' => "orange",
	// 			'#' => "grey",
	// 			'O' or '[' or ']' => "green",
	// 			_ => null
	// 		}));
}