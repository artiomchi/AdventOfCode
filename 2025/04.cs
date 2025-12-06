#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using System.Diagnostics;

var sw = Stopwatch.StartNew();
var map = Map.FromString(FileHelpers.ReadInputText("04.txt"));
var parseTime = sw.Elapsed;

sw.Restart();
var accessible = RemoveRolls(map);
Console.WriteLine("Accessible positions: " + accessible);
var part1Time = sw.Elapsed;

sw.Restart();
int totalRemoved = accessible, removed;
while ((removed = RemoveRolls(map)) > 0)
{
    totalRemoved += removed;
}

Console.WriteLine("Total removed positions: " + totalRemoved);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);

static int RemoveRolls(Map map)
{
    foreach (var point in map.AllPoints())
    {
        if (map[point] == '.')
        {
            continue;
        }

        var adjacent = Vector.DirectionsAll.Count(direction =>
        {
            var neighbor = point + direction;
            if (!neighbor.IsInBounds(map))
            {
                return false;
            }

            return map[neighbor] is '@' or 'x';
        });

        if (adjacent < 4)
        {
            map[point] = 'x';
        }
    }

    var accessible = 0;
    foreach (var point in map.AllPoints())
    {
        if (map[point] is 'x')
        {
            accessible++;
            map[point] = '.';
        }
    }
    return accessible;
}
