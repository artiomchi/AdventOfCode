#nullable enable
#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;
using System.Text.RegularExpressions;

var input = File.ReadAllText("2024/inputs/24.test.txt");
var wires = Regex.Matches(input, @"(\w{3}): (\d)").ToDictionary(m => m.Groups[1].Value, m => int.Parse(m.Groups[2].Value));
var swaps = new List<(string a, string b)>
{
	("z06", "vwr"),
	("z11", "tqm"),
	("z16", "kfs"),
	("gfv", "hcm"),
	//("z06", "tz06")
};
string.Join(",", swaps.SelectMany(x => new[] { x.a, x.b }).Order()).Dump("Part 2");
string[] debug = ["thv", "gfv", "hcm"];
var wiring = Regex.Matches(input, @"(\w+) (\w+) (\w+) -> (\w+)")
	.Select(m => (
		w1: m.Groups[1].Value,
		op: m.Groups[2].Value,
		w2: m.Groups[3].Value,
		w3: Swap(m.Groups[4].Value)))
	.ToArray();
var wires2 = new Dictionary<string, string>();
string Swap(string w)
{
	var match = swaps.FirstOrDefault(x => x.a == w || x.b == w);
	if (match.a == null)
	{
		return w;
	}
	return match.a == w ? match.b : match.a;
}

var remainingWires = wiring.Where(w => !wires.ContainsKey(w.w3));
while (remainingWires.Any())
{
	var matched = 0;
	foreach (var (w1, op, w2, w3) in remainingWires)
	{
		if (!wires.ContainsKey(w1) || !wires.ContainsKey(w2))
			continue;
			
		wires[w3] = op switch
		{
			"AND" => wires[w1] & wires[w2],
			"OR" => wires[w1] | wires[w2],
			"XOR" => wires[w1] ^ wires[w2],
			_ => throw new ArgumentOutOfRangeException(nameof(op), op, "Unknown gate")
		};
		wires2[w3] =
			(wires2.TryGetValue(w1, out var w1v) ? w1v : w1) +
			$" {op} " + 
			(wires2.TryGetValue(w2, out var w2v) ? w2v : w2);
		matched++;
	}
}

wires.OrderBy(w => w.Key).ToArray().Dump(0);
wires2.Where(w => w.Key[0] == 'z').OrderBy(w => w.Key).Select(w => $"{w.Key} = {w.Value}").ToArray().Dump(0);

GetNumber('z').Dump("Part 1");

(GetNumber('x') + GetNumber('y')).Dump("Part 2: x + y");

var logic = """
x00 XOR y00 -> z00
x00 AND y00 -> c00

x01 XOR y01 -> h01
x01 AND y01 -> i01
c00 XOR h01 -> z01
c00 AND h01 -> b01
b01 OR  i01 -> c01

x02 XOR y02 -> h02
x02 AND y02 -> i02
c01 XOR h02 -> z02
c01 AND h02 -> b02
b02 OR  i02 -> c02

x03 XOR y03 -> h03
x03 AND y03 -> i03
c02 XOR h03 -> z03
c02 AND h03 -> b03
b03 OR  i03 -> c03

c03 OR  n00 -> z04
""";

wiring.Where(w => w.w3[0] == 'z' && w.op != "XOR").Dump("bad1", 0);

var renames = new Dictionary<string, string>();
var reverse = new Dictionary<string, string>();
var warnings = new Dictionary<string, string>();
var counters = new Dictionary<string, int>();
var manual = new List<(string a, string b)>
{
	//("I06", "vwr"), // z06
	//("B11", "tqm"), // z11 ? possible
	//("z16", "kfs"),
	//("I36", "gfv"),
	//("H36", "hcm"),
};
try
{
	var wireCount = int.Parse(wires.Keys.Where(k => k[0] == 'x').OrderDescending().First()[1..]);
	Map(0, "x00", "AND", "y00", "C00");
	for (int i = 1; i <= wireCount; i++)
	{
		Map(i, $"x{i:00}", "XOR", $"y{i:00}", $"H{i:00}");
		Map(i, $"x{i:00}", "AND", $"y{i:00}", $"I{i:00}");
	}

	for (int i = 1; i <= wireCount; i++)
	{
		Map(i, $"C{i - 1:00}", "XOR", $"H{i:00}", $"z{i:00}");
		Map(i, $"C{i - 1:00}", "AND", $"H{i:00}", $"B{i:00}");
		Map(i, $"B{i:00}", "OR", $"I{i:00}", $"C{i:00}");
	}
}
finally
{
	wiring.Where(w => w.w3[0] == 'z' && w.op != "XOR").Dump("bad after renames");

	foreach (var (w1, op, w2, w3) in wiring)
	{
		CountWires(w1);
		CountWires(w2);

		void CountWires(string wire)
		{
			wire = Rename(wire);
			if (!counters.TryGetValue(wire, out var c))
			{
				c = 0;
			}
			counters[wire] = ++c;
		}
	}
	counters
		.Where(c => c.Key[0] is 'B' or 'I' or 'H' or 'C')
		.Where(c => c.Value != (c.Key[0] is 'B' or 'I' ? 1 : 2))
		.Dump("bad counters");

	warnings.Dump("warnings");
	
	wiring.Where(w => debug.Contains(w.w1) || debug.Contains(w.w2)).Dump("debug", 0);
	
	var newWiring = wiring
		.Select(w =>
		{
			var wires = new[] { Reverse(w.w1), Reverse(w.w2) }.Order().ToArray();
			if (!int.TryParse(wires[1][1..], out var i)) i = -1;
			var (w1, op, w2, w3) = (wires[0], w.op, wires[1], Reverse(w.w3));
			var exp = new string([w1[0], op[0], w2[0]]) switch
			{
				"xXy" => $"H{i:00}",
				"xAy" => $"I{i:00}",
				"CXH" => $"z{i:00}",
				"CAH" => $"B{i:00}",
				"BOI" => $"C{i:00}",
				_ => "ERR",
			};
			var expVal = exp;
			if (renames.ContainsKey(expVal))
				exp += $" {renames[expVal]}";
			if (w3 != expVal)
				exp = "!!! " + exp;
			return (w1, op, w2, w3, exp, i);
		})
		.OrderBy(w => w.i >= 0 ? w.i : int.MaxValue)
		.ThenBy(w => w.w1[0] != 'x')
		.ThenBy(w => w.w1[0] != 'y')
		.ThenBy(w => w.w1[0] != 'C')
		.ThenBy(w => w.w1[0] != 'H' ? w.w1 : "Z")
		.ThenBy(w => w.w1[0] != 'I' ? w.w1 : "Z")
		.ThenByDescending(w => w.op);
	newWiring.Dump("ALL Wirings", 0);
	newWiring
		.GroupBy(w => w.i)
		.SkipWhile(g => g.Key == 0 || g.All(w => w.exp[0] != '!'))
		.Dump();
	renames.Dump();
}


void Map(int i, string ow1, string op, string ow2, string ow3)
{
	var w1 = Rename(ow1);
	var w2 = Rename(ow2);
	var w3 = Rename(ow3);

	var connection = wiring
		.Where(w => (w.w1, w.op, w.w2) == (w1, op, w2) || (w.w1, w.op, w.w2) == (w2, op, w1))
		.SingleOrDefault();
	if (connection.w1 is null)
		throw new Exception($"Oops at {i}: {w1} ({ow1}) {op} {w2} ({ow2}) -> {w3} ({ow3})");
	var cw3 = connection.w3;
	//if (cw3[0] == 'z' && w3[0] != 'z')
	//{
	//	if (TrySwap(cw3, out var n))
	//		cw3 = n;
	//	else
	//	{
	//		warnings[cw3 + ".1"] = $"mapping to z, when should be something else ({w3})";
	//		return;
	//	}
	//	//if (renames.TryGetValue(cw3, out var ncw3))
	//	//{
	//	//	swaps[ncw3] = cw3;
	//	//	cw3 = 
	//	//}
	//	//throw new Exception($"Trying to map to z at {i}: {w1} {op} {w2} -> {w3}");
	//}
	//if (w3[0] == 'z')
	//{
	//	if (TrySwap(cw3, out var n))
	//		w3 = n;
	//	else
	//	{
	//		warnings[w3 + ".2"] = $"trying to use z when should be something else {cw3}";
	//		return;
	//	}
	//	//if (renames.TryGetValue(cw3, out var ncw3))
	//	//{
	//	//	swaps[ncw3] = cw3;
	//	//	cw3 = 
	//	//}
	//	//throw new Exception($"Trying to map to z at {i}: {w1} {op} {w2} -> {w3}");
	//}
	
	if (w3 == cw3 && w3[0] == 'z')
	{
		return;
	}
	
	if (w3 == cw3 && w3[0] != cw3[0])
		throw new Exception("lol " + w3);
		
	if (cw3.Length > 3 && cw3[0] != w3[0])
	{
		warnings[cw3 + ".3"] = $"{cw3} but expecting {w3}";
	}
	
	if (w3[0] == 'z' || cw3[0] == 'z')
	{
		return;
		Util.Break();
	}
	
	renames[w3] = cw3;
	reverse[cw3] = w3;
}
string Rename(string w) => renames.TryGetValue(w, out var r) ? r : w;
string Reverse(string w) => reverse.TryGetValue(w, out var r) ? r : w;
bool TrySwap(string w, out string o)
{
	var match = manual.FirstOrDefault(x => x.a == w || x.b == w);
	if (match.a == null)
	{
		o = w;
		return false;
	}
	o = match.a == w ? match.b : match.a;
	return true;
}

Int64 GetNumber(char wireSet)
	=> Convert.ToInt64(new string(wires
	.Where(w => w.Key.StartsWith(wireSet))
	.OrderByDescending(w => w.Key)
	.Select(w => (char)('0' + w.Value))
	.ToArray()), 2);