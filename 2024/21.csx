#nullable enable
#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;

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

var input = File.ReadAllText("2024/inputs/21.real.txt");
Assert(input, 3).Dump("Part 1").Should().BeOneOf(126384, 231564);
Assert(input, 25 + 1).Dump("Part 2").Should().BeOneOf(154115708116294, 281212077733592);


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
		$"Combo: {combo}, nums: {nums}, moves: {moves}".Dump();
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

		var moveX = new string(vector.x > 0 ? '>' : '<', Math.Abs((int)vector.x));
		var moveY = new string(vector.y > 0 ? 'v' : '^', Math.Abs((int)vector.y));

		(_, length) =
			new[]
			{
				_map[posPt.x, destPt.y] != ' ' ? $"{moveY}{moveX}A" : null,
				_map[destPt.x, posPt.y] != ' ' ? $"{moveX}{moveY}A" : null,
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