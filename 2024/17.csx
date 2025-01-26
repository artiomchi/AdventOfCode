#load "..\Helpers.csx"
using System.Text.RegularExpressions;

var input = ReadInputText("17.real.txt");
var sw = Stopwatch.StartNew();
var match = Regex.Match(input, @"A: (?<a>\d+)\s+\w+ B: (?<b>\d+)\s+\w+ C: (?<c>\d+)\s+Program: (?<i>[\d,]+)");
var ra = int.Parse(match.Groups["a"].Value);
var rb = int.Parse(match.Groups["b"].Value);
var rc = int.Parse(match.Groups["c"].Value);
var inputs = match.Groups["i"].Value.Split(',').Select(int.Parse).ToArray();

// Parse prog
for (int i = 0; i < inputs.Length / 2; i++)
{
    var literal = inputs[i * 2 + 1];
    WriteLine(inputs[i * 2] switch
    {
        0 => $"adv {ComboDef()}\t\teax /= 2 ^ {ComboDef()}",
        1 => $"bxl {literal}\t\tebx ^= {literal}",
        2 => $"bst {ComboDef()}\t\tebx = {ComboDef()} % 8",
        3 => $"jnz " + literal,
        4 => $"bxc ecx\t\tebx ^= ecx",
        5 => $"out {ComboDef()}",
        6 => $"bdv {ComboDef()}\t\tebx = eax / 2 ^ {ComboDef()}",
        7 => $"cdv {ComboDef()}\t\tecx = eax / 2 ^ {ComboDef()}",
        _ => "ERR"
    });
    string ComboDef()
        => literal switch
        {
            <= 3 => literal.ToString(),
            4 => "eax",
            5 => "ebx",
            6 => "ecx",
            _ => throw new NotImplementedException("Combo operand reserved and not implemented: " + literal),
        };
}
WriteLine("---");
var parseTime = sw.Elapsed;

// Part 1
sw.Restart();
string.Join(",", RunProgram(ra, inputs)).DumpAndAssert("Part 1", "4,6,3,5,6,3,5,2,1,0", "2,1,4,7,6,0,3,1,4");
var part1Time = sw.Elapsed;

string.Join(',',inputs).Dump("target");

// Part 2
long min = 0, max = 0;
long current = 1;
do
{
    var result = RunProgram(current, inputs);
    if (result.Count == inputs.Length - 1)
        min = current;
    else if (result.Count == inputs.Length + 1)
        max = current;
    current *= 8;
}
while (min == 0 || max == 0);

for (int i = 1; i <= inputs.Length; i++)
{
    (min, max) = FindMatchingSubset(min, max, i);
    if (min < 0 || max < 0)
        throw new Exception();
}

min.DumpAndAssert("Part 2", 266932601404433);
var part2Time = sw.Elapsed;

PrintTimings(parseTime, part1Time, part2Time);

bool Test(long ra, int[] inputs, int digits)
{
    var match = inputs.TakeLast(digits);
    try
    {
        var result = RunProgram(ra, inputs);
        return result.Count == inputs.Length && result.TakeLast(digits).SequenceEqual(match);
    }
    catch
    {
        return false;
    }
}

(long min, long max) FindMatchingSubset(long min, long max, int digits)
{
    var step = Math.Max((max - min) / 20, 1);
    var pos = min;
    long validPos = -1;
    while (pos <= max)
    {
        if (Test(pos, inputs, digits))
        {
            validPos = pos;
            break;
        }
        pos += step;
    }
    if (validPos == -1)
        return (-1, -1);

    long start = -1, end = -1;

    // Binary search to find the start of the subset
    long left = min, right = validPos;
    while (left <= right)
    {
        long mid = left + (right - left) / 2;
        if (Test(mid, inputs, digits))
        {
            start = mid;       // Potential start found
            right = mid - 1;   // Search the left half for an earlier match
        }
        else
        {
            left = mid + 1;    // Ignore the left half
        }
    }

    // Binary search to find the end of the subset
    left = start;
    right = max;
    while (left <= right)
    {
        long mid = left + (right - left) / 2;
        if (Test(mid, inputs, digits))
        {
            end = mid;         // Potential end found
            left = mid + 1;    // Search the right half for a later match
        }
        else
        {
            right = mid - 1;   // Ignore the right half
        }
    }

    return (start, end);
}


//var start = 985476453;
//
//Enumerable.Range(0, 100)
//    .Select(i => (i, a: start + i * 8))
//    .Select(x => $"{x.i}, {x.a}, {x.a % 8} => {string.Join(',', RunProgram(x.a, 0, 0, inputs, out var res, false) != null ? res : [])}")
//    .Dump();
//
//Util.HorizontalRun(true,
//    RunProgram(5, rb, rc, inputs, out _, false),
//    RunProgram(45, rb, rc, inputs, out _, false),
//    RunProgram(69, rb, rc, inputs, out _, false),
//    RunProgram(29315438, rb, rc, inputs, out _, false),
//    RunProgram(985476453 + 42 + 42 + 42 + 42, rb, rc, inputs, out _, false)
//    ).Dump();

//Enumerable.Range(0, 65)
//    .Select(i => $"{i} => {string.Join(',', RunProgram(i, 0, 0, inputs, false))}")
//    .Dump();
//
//string.Join(',', RunProgram((int)Math.Pow(8, inputs.Length), 0, 0, inputs, false)).Dump();

// Part 1
//var result = RunProgram(ra, rb, rc, inputs);
//string.Join(",", result).Dump("Part 1");

// Part 2
//result = RunProgram(5, rb, rc, inputs);
//result.Should().BeEquivalentTo(inputs);

(int, int) FindRanges(int[] inputs, int index)
{
    var start = (long)Math.Pow(8, inputs.Length + 1);
    do {
        var result = RunProgram(start, inputs);
        if (result.Count < inputs.Length)
            start *= 8;
        if (result.Count >= inputs.Length)
            break;
    }
    while(true);

    throw new NotImplementedException();
}

static List<int> RunProgram(long ra, int[] inputs, bool print = false, long rb = 0, long rc = 0)
{
    var pt = 0;
    var outputs = new List<int>();
    var ops = new List<(string op, string desc, string output)>();
    long rm;
    while (pt < inputs.Length)
    {
        switch (inputs[pt])
        {
            case 0:
                rm = ra;
                ra /= (long)Math.Pow(2, Combo());
                ops.Add(($"adv {ComboName()}", $"A = A({rm}) / 2 ^ {ComboName()} = {ra}", ""));
                break;
            case 1:
                rm = rb;
                rb = (rb ^ Literal());// % short.MaxValue;
                ops.Add(($"bxl {Literal()}", $"B = B({rm}) ^ {Literal()} = {rb}", ""));
                break;
            case 2:
                rb = Combo() % 8;
                ops.Add(($"bst {ComboName()} % 8", $"B = {ComboName()} % 8 = {rb}", ""));
                break;
            case 3:
                if (ra != 0)
                {
                    ops.Add(($"jnz {Literal()}", $"A = {ra}", ""));
                    pt = Literal();
                    continue;
                }
                else
                {
                    ops.Add(($"noop", $"A = {ra} => noop / jnz {Literal()}", ""));
                }
                break;
            case 4:
                rm = rb;
                rb = (rb ^ rc);// % short.MaxValue;
                ops.Add(($"bxc C({rc})", $"B = B({rm}) ^ C({rc}) = {rb}", ""));
                break;
            case 5:
                outputs.Add((int)(Combo() % 8));
                ops.Add(($"out {ComboName()} % 8", "", $"{Combo() % 8}"));
                break;
            case 6:
                rm = rb;
                rb = ra / (long)Math.Pow(2, Combo());
                ops.Add(($"bdv {ComboName()}", $"B = A({ra}) / 2 ^ {ComboName()} = {rb}", ""));
                break;
            case 7:
                rm = rc;
                rc = ra / (long)Math.Pow(2, Combo());
                ops.Add(($"cdv {ComboName()}", $"C = A({ra}) / 2 ^ {ComboName()} = {rc}", ""));
                break;
            default:
                throw new InvalidOperationException("Unknown instruction: " + inputs[pt]);
        }
        pt += 2;

        int Literal() => inputs[pt + 1];
        long Combo()
        {
            var value = inputs[pt + 1];
            return value switch
            {
                <= 3 => value,
                4 => ra,
                5 => rb,
                6 => rc,
                _ => throw new NotImplementedException("Combo operand reserved and not implemented: " + value),
            };
        }
        string ComboName()
        {
            var value = inputs[pt + 1];
            return value switch
            {
                <= 3 => value.ToString(),
                4 => $"A({ra})",
                5 => $"B({rb})",
                6 => $"C({rc})",
                _ => throw new NotImplementedException("Combo operand reserved and not implemented: " + value),
            };
        }
    }
    if (print)
    {
        $"A: {ra}, B: {rb}, C: {rc}, O: {string.Join(",", outputs)}".Dump();
        ops.Dump();
    }
    return outputs;
}
