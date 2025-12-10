#!/usr/bin/env dotnet
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

var sw = Stopwatch.StartNew();
var machines = FileHelpers.ReadInputLines("10.txt")
    .Select(l => new Machine(l))
    .ToArray();
var parseTime = sw.Elapsed;

sw.Restart();
var buttonPresses = 0;
foreach (var machine in machines)
{
    var presses = 1;
    var options = new HashSet<int>(machine.ButtonWirings);
    while (!options.Contains(machine.Buttons))
    {
        presses++;
        foreach (var option in options.ToArray())
        {
            foreach (var wiring in machine.ButtonWirings)
            {
                var combo = option ^ wiring;
                options.Add(option ^ wiring);
            }
        }
    }
    buttonPresses += presses;
}
var part1Time = sw.Elapsed;
buttonPresses.DumpAndAssert("Part 1", 7, 390);

sw.Restart();
buttonPresses = 0;
void PressButton(short[] state, int[] wiring, short presses)
{
    // replace with foreach
    foreach (var i in wiring)
    {
        state[i]+= presses;
    }
}

static List<int[]> EnumerateCombinations(int target, int options)
{
    var allCombinations = new List<int[]>();
    FindCombinations(target, options, 0, new int[options], allCombinations);
    return allCombinations;
}

static void FindCombinations(int remaining, int options, int startIndex, int[] current, List<int[]> combinations)
{
    if (remaining == 0)
    {
        combinations.Add((int[])current.Clone());
        return;
    }

    for (int i = startIndex; i < options; i++)
    {
        current[i]++;
        FindCombinations( remaining - 1, options, i, current, combinations);
        current[i]--;
    }
}

async Task<int> GetMinPresses(short[] state, int[][] wirings, short[] requirements, int minPresses)
{
    // Remove wirings that would affect joltages that are already satisfied
    wirings = wirings
        .Where(w => w.Any(i => state[i] < requirements[i]))
        .ToArray();

    var impossibleRequirements =requirements.Index()
        .Where(x => x.Item > state[x.Index])
        .Any(x => !wirings.Any(w => w.Contains(x.Index)));
    if (impossibleRequirements)
    {
        return -1;
    }

    // get value that appears in the least number of wirings
    var digit = Enumerable.Range(0, requirements.Length)
        .Where(i => wirings.Any(w => w.Contains(i)))
        .OrderBy(i => wirings.Count(w => w.Contains(i)))
        .DefaultIfEmpty(-1)
        .First();
    if (digit == -1)
    {
        return -1;
    }
    var missingJoltage = requirements[digit] - state[digit];

    // get wirings that contain that digit
    var filteredWirings = wirings
        .Where(w => w.Contains(digit))
        .Select(w => (Wiring: w, MaxPresses: w.Min(x => requirements[x] - state[x])))
        .ToArray();
    if (filteredWirings.Length == 1)
    {
        if (missingJoltage > filteredWirings[0].MaxPresses)
        {
            return -1;
        }

        PressButton(state, filteredWirings[0].Wiring, (short)missingJoltage);
        if (state.SequenceEqual(requirements))
        {
            return missingJoltage;
        }

        if (state.Index().Any(x => x.Item > requirements[x.Index]))
        {
            throw new InvalidOperationException("Overpressed a button");
        }

        wirings = wirings
            .Where(w => w != filteredWirings[0].Wiring)
            .ToArray();
        var result = await GetMinPresses(state, wirings, requirements, minPresses);
        if (result < 0)
        {
            return -1;
        }
        return result + missingJoltage;
    }

    var combos = EnumerateCombinations(missingJoltage, filteredWirings.Length);
    var tasks = combos.Select(combo => Task.Run(async () =>
    {
        var newState = (short[])state.Clone();
        for (int ci = 0; ci < combo.Length; ci++)
        {
            var presses = combo[ci];
            var wiring = filteredWirings[ci].Wiring;
            PressButton(newState, wiring, (short)presses);
        }

        if (newState.SequenceEqual(requirements))
        {
            return combo.Sum();
        }

        if (newState.Index().Any(x => x.Item > requirements[x.Index]))
        {
            return -1;
        }

        var result = await GetMinPresses(newState, wirings.Except(filteredWirings.Select(fw => fw.Wiring)).ToArray(), requirements, minPresses);
        if (result > 0)
        {
            return combo.Sum() + result;
        }
        return -1;
    })).ToArray();
    var minResult = (await Task.WhenAll(tasks))
        .Where(r => r >= 0)
        .DefaultIfEmpty(-1)
        .Min();
    if (minResult != -1)
    {
        minPresses = minResult;
    }

    return minPresses == int.MaxValue ? -1 : minPresses;
}

foreach (var (i, machine) in machines.Index())
{
    var presses = await GetMinPresses(new short[machine.JoltageRequirements.Length], machine.JoltageButtonWirings, machine.JoltageRequirements, int.MaxValue);

    buttonPresses += presses;
    Console.WriteLine($"Machine {i}, presses: {presses}");
}

var part2Time = sw.Elapsed;
buttonPresses.DumpAndAssert("Part 2", 33, 14677);

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);

class Machine
{
    public Machine(string input)
    {
        var match = Regex.Match(input, @"^\[([\.#]+)] (?:\(([\d,]+)\) )+{([\d,]+)}$");
        if (!match.Success)
        {
            throw new InvalidEnumArgumentException("Invalid machine input");
        }

        Buttons = match.Groups[1].Value.Select(c => c == '#').Select((b, i) => b ? 1 << i : 0).Sum();
        ButtonWirings = match.Groups[2].Captures.Select(s => s.Value.Split(',').Select(int.Parse).Sum(i => 1 << i)).ToArray();

        JoltageRequirements = match.Groups[3].Value.Split(',').Select(short.Parse).ToArray();
        JoltageButtonWirings = match.Groups[2].Captures.Select(s => s.Value.Split(',').Select(int.Parse).ToArray()).ToArray();
    }

    public int Buttons { get; init; }
    public int[] ButtonWirings { get; init; }
    public short[] JoltageRequirements { get; init; }
    public int[][] JoltageButtonWirings { get; init; }
}
