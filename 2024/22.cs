#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var lines = FileHelpers.ReadInputLines("22.txt");

// Part 1
var sw = Stopwatch.StartNew();
lines
    .Select(long.Parse)
    .Select(s => Enumerable.Range(0, 2000).Aggregate(s, (s, _) => Process(s)))
    .Sum()
    .DumpAndAssert("Part 1", 37327623, 37990510, 21147129593);
var part1Time = sw.Elapsed;

// Part 2
sw.Restart();
var nums = lines
    .Select(long.Parse)
    .Select(s =>
    {
        var numbers = Enumerable.Range(0, 2000)
            .Aggregate(
                new List<long> { s },
                (s, _) =>
                {
                    s.Add(Process(s[^1]));
                    return s;
                });

        return numbers
            .Skip(4)
            .Index()
            .Select(x => (
                n1: numbers[x.Index + 1] % 10 - numbers[x.Index] % 10,
                n2: numbers[x.Index + 2] % 10 - numbers[x.Index + 1] % 10,
                n3: numbers[x.Index + 3] % 10 - numbers[x.Index + 2] % 10,
                n4: numbers[x.Index + 4] % 10 - numbers[x.Index + 3] % 10,
                price: numbers[x.Index + 4] % 10))
            .GroupBy(x => (x.n1, x.n2, x.n3, x.n4), x => x.price)
            .ToDictionary(x => x.Key, x => x.First());
    })
    .ToArray();
var best = nums
    .SelectMany(n => n.Select(x => x.Key))
    .Distinct()
    .Select(n => (
        set: n,
        total: nums.Sum(x => (x.TryGetValue(n, out var val) ? val : 0))))
    .OrderByDescending(n => n.total)
    .First()
    .total
    .DumpAndAssert("Part 2", 24, 23, 2445);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(part1Time, part2Time);

long Process(long secret)
{
    secret = (secret ^ (secret * 64)) % 16777216;
    secret = (secret ^ (long)Math.Floor(secret / 32f)) % 16777216;
    secret = (secret ^ (secret * 2048)) % 16777216;
    return secret;
}
