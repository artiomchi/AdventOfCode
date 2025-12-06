#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;
using System.Numerics;

var input = FileHelpers.ReadInputText("11.txt");
var sw = Stopwatch.StartNew();
var data = input.Split(' ')
    .Select(long.Parse)
    .GroupBy(s => s)
    .ToDictionary(g => g.Key, g => (long)g.Count());
var parseTime = sw.Elapsed;

sw.Restart();
var part1Time = TimeSpan.Zero;
for (var i = 0; i < 75; i++)
{
    var newData = new Dictionary<long, long>();
    foreach (var stone in data.Keys)
    {
        var newStones = stone switch
        {
            0 => [1],
            _ when Math.Log10(stone) % 2 >= 1 => Split(stone),
            _ => [stone * 2024]
        };
        foreach (var num in newStones)
        {
            newData[num] = newData.TryGetValue(num, out var count) ? count + data[stone] : data[stone];
        }
    }
    data = newData;

    if (i == 24)
    {
        data.Values.Sum().DumpAndAssert("Part 1", 55312, 186424);
        part1Time = sw.Elapsed;
    }
}

data.Values.Sum().DumpAndAssert("Part 2", 219838428124832);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);

long[] Split(long number)
{
    var splitNum = (long)Math.Pow(10, Math.Floor(Math.Log10(number) + 1) / 2);
    return [number / splitNum, number % splitNum];
}
