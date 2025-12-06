#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;
using System.Text.RegularExpressions;

var input = FileHelpers.ReadInputText("24.txt");
var sw = Stopwatch.StartNew();
var wires = Regex.Matches(input, @"(\w{3}): (\d)").ToDictionary(m => m.Groups[1].Value, m => int.Parse(m.Groups[2].Value));
var wiring = Regex.Matches(input, @"(\w+) (\w+) (\w+) -> (\w+)")
    .Select(m => (
        w1: m.Groups[1].Value,
        op: m.Groups[2].Value,
        w2: m.Groups[3].Value,
        w3: m.Groups[4].Value))
    .ToArray();
var parseTime = sw.Elapsed;

// Part 1
sw.Restart();
RunSimulation();
GetNumber('z').DumpAndAssert("Part 1", 50411513338638);
var part1Time = sw.Elapsed;

/* Part 2

    Building a full adder algo. The operations will look something like below.
    The wire namings are our target, so we'll be building a rename list for troubleshooting simplicity

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

    etc...

    Based on investigating the data, looks like the wires are only swapped within a single cycle.
    As such, we don't need to build a fully generic algo, and can instead work within these constraints.
*/

sw.Restart();
var renames = new Dictionary<string, string>();
var reverse = new Dictionary<string, string>();

// First set should be untouched
Rename(FindWiring("x00", "AND").w3, "c00");

var swaps = new List<string>();
var wireCount = int.Parse(wires.Keys.Where(k => k[0] == 'x').OrderDescending().First()[1..]);
for (var i = 1; i <= wireCount; i++)
{
    var (_, hName) = FindWiring($"x{i:00}", "XOR");
    var (_, iName) = FindWiring($"x{i:00}", "AND");
    var (h1, zName) = FindWiring($"c{i-1:00}", "XOR");
    var (h2, bName) = FindWiring($"c{i-1:00}", "AND");

    // In case H and I are swapped
    if (zName[0] == 'z' && h1 == iName)
    {
        SwapOutputs(hName, iName);

        var tmp = hName;
        hName = iName;
        iName = tmp;
    }

    if (hName[0] != 'z')
        Rename(hName, $"h{i:00}");
    if (iName[0] != 'z')
        Rename(iName, $"i{i:00}");
    if (bName[0] != 'z')
        Rename(bName, $"b{i:00}");

    if (zName[0] != 'z')
    {
        // Let's find what was swapped with Z
        var goodLetter = hName[0] == 'z'
            ? 'h'
            : iName[0] == 'z'
            ? 'i'
            : bName[0] == 'z'
            ? 'b'
            : 'c';
        SwapOutputs($"z{i:00}", GetReverse(zName));

        Rename(zName, $"{goodLetter}{i:00}");
    }

    var cName = FindWiring($"b{i:00}", "OR").w3;
    Rename(cName, $"c{i:00}");

    void SwapOutputs(string o1, string o2)
    {
        var (index1, a) = wiring.Index().Where(x => x.Item.w3 == o1).First();
        var (index2, b) = wiring.Index().Where(x => x.Item.w3 == o2).First();

        wiring[index1] = (a.w1, a.op, a.w2, b.w3);
        wiring[index2] = (b.w1, b.op, b.w2, a.w3);

        swaps.AddRange(o1, o2);
    }
}

RunSimulation();
var expectedResult = GetNumber('x') + GetNumber('y');
GetNumber('z').DumpAndAssert("Part 2 calculation", expectedResult);

string.Join(",", swaps.Order()).DumpAndAssert("Part 2", "gfv,hcm,kfs,tqm,vwr,z06,z11,z16");
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);

// Helpers
void RunSimulation()
{
    // Reset the wires
    foreach (var key in wires.Keys.Where(k => k[0] != 'x' && k[0] != 'y').ToArray())
        wires.Remove(key);

    var remainingWires = wiring.Where(w => !wires.ContainsKey(w.w3));
    while (remainingWires.Any())
    {
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
        }
    }
}

long GetNumber(char wireSet)
    => Convert.ToInt64(new string(wires
    .Where(w => w.Key.StartsWith(wireSet))
    .OrderByDescending(w => w.Key)
    .Select(w => (char)('0' + w.Value))
    .ToArray()), 2);

string GetRename(string w) => renames.TryGetValue(w, out var r) ? r : w;
string GetReverse(string w) => reverse.TryGetValue(w, out var r) ? r : w;
void Rename(string name, string mappedName)
{
    renames[name] = mappedName;
    reverse[mappedName] = name;
}

(string w2, string w3) FindWiring(string w1, string op)
{
    w1 = GetRename(w1);
    return wiring
        .Where(x => x.op == op)
        .Select(x => (w1: GetRename(x.w1), w2: GetRename(x.w2), x.w3))
        .Where(x => w1 == x.w1 || w1 == x.w2)
        .Select(x => (w1 == x.w1 ? x.w2 : x.w1, x.w3))
        .FirstOrDefault();
}
