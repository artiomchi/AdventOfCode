#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;

var input = FileHelpers.ReadInputLines("03.real.txt");

long total1 = 0, total2 = 0;

foreach (var line in input)
{
    var digits = line.Select(c => c - '0').ToArray();

    // Part 1
    {
        var first = digits[..^1].Max();
        var second = digits.SkipWhile(d => d != first).Skip(1).Max();
        total1 += first * 10 + second;
    }

    // Part 2
    {
        var index = 0;
        var remaining = 12;
        long bank = 0;

        while (remaining > 0)
        {
            remaining--;

            var next = digits[index..^remaining].Max();
            bank = bank * 10 + next;
            index += digits[index..].IndexOf(next) + 1;
        }

        total2 += bank;
    }
}

Console.WriteLine($"Total joltage 1: {total1}");
Console.WriteLine($"Total joltage 2: {total2}");
