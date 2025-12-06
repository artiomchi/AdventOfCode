#!/usr/bin/env dotnet
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var input = FileHelpers.ReadInputText("09.txt");
var sw = new Stopwatch();

// Part 1
// ######
{
    // Parse map
    sw.Restart();
    var map = new List<int>();
    foreach (var (index, size) in input.Index())
    {
        map.AddRange(Enumerable.Repeat(index % 2 == 0 ? index / 2 : -1, size - '0'));
    }
    var parseTime = sw.Elapsed;

    // Compact
    sw.Restart();
    for (var i = map.Count - 1; i > 0 && i > map.IndexOf(-1); i--)
    {
        if (map[i] == -1)
            continue;

        map[map.IndexOf(-1)] = map[i];
        map[i] = -1;
    }
    var compactTime = sw.Elapsed;

    // Checksum
    sw.Restart();
    var checksum = map.Index().Aggregate(
        (long)0,
        (s, x) => s + (x.Item > 0 ? x.Item * x.Index : 0));
    var checksumTime = sw.Elapsed;

    checksum.DumpAndAssert("Part 1", 1928, 6384282079460);

    OutputHelpers.PrintTimings(parseTime, compactTime, checksumTime);
}


// Part 1 optimised
// ######
{
    sw.Restart();
    var lastIndex = input.Length - 1 - (1 ^ (input.Length % 2));
    var lastRemaining = input[lastIndex] - '0';

    long checksum = 0;
    long index = 0;
    for (int i = 0; i < input.Length; i++)
    {
        if (i > lastIndex)
            break;

        if (i % 2 == 0)
        {
            var max = index + (lastIndex == i ? lastRemaining : input[i] - '0');
            for (; index < max; index++)
            {

                checksum += index * (i / 2);
            }
            continue;
        }

        for (int rem = input[i] - '0'; rem > 0; rem--)
        {
            if (i > lastIndex)
            {
                index++;
                continue;
            }

            checksum += index++ * (lastIndex / 2);
            lastRemaining--;

            if (lastRemaining == 0)
            {
                lastIndex -= 2;
                lastRemaining = input[lastIndex] - '0';
            }
        }
    }

    checksum.DumpAndAssert("Part 1 Optimised", 1928, 6384282079460);

    OutputHelpers.PrintTimings(sw.Elapsed);
}

// Part 2
// ######
{
    // Parse map
    sw.Restart();
    var map = new List<(int id, int size)>();
    foreach (var (index, size) in input.Index())
    {
        map.Add((index % 2 == 0 ? index / 2 : -1, size - '0'));
    }
    var parseTime = sw.Elapsed;

    // Compact
    sw.Restart();
    for (var id = map.Max(x => x.id); id >= 0; id--)
    {
        var (pos, size) = map.Index().Where(x => x.Item.id == id).Select(x => (x.Index, x.Item.size)).First();

        var freeSpace = map.Index().Where(x => x.Index < pos && x.Item.id == -1 && x.Item.size >= size).FirstOrDefault();
        if (freeSpace.Item.size == 0)
            continue;

        map[pos] = (-1, size);

        map[freeSpace.Index] = (id, size);
        if (freeSpace.Item.size > size)
        {
            map.Insert(freeSpace.Index + 1, (-1, freeSpace.Item.size - size));
        }
    }
    var compactTime = sw.Elapsed;

    // Checksum
    sw.Restart();
    var checksum = map.Aggregate(
        (sum: (long)0, index: 0),
        (x, y) => (x.sum + (y.id > 0 ? Enumerable.Range(x.index, y.size).Select(i => (long)i * y.id).Sum() : 0), x.index + y.size))
        .sum;
    var checksumTime = sw.Elapsed;

    checksum.DumpAndAssert("Part 2", 2858, 6408966547049);

    OutputHelpers.PrintTimings(parseTime, compactTime, checksumTime);
}
