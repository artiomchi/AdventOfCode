#!/usr/bin/env dotnet
#:package Raylib-cs@7.0.2
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;
using Raylib_cs;
using System.Diagnostics;

var sw = Stopwatch.StartNew();
var map = Map.FromString(FileHelpers.ReadInputText("04.txt"));
var parseTime = sw.Elapsed;

Raylib.SetTargetFPS(60);
Raylib.SetConfigFlags(ConfigFlags.HiddenWindow);
Raylib.InitWindow(800, 600, "Raylib Test 2");

var scale = (int)Math.Floor(Math.Min(
    Raylib.GetMonitorWidth(Raylib.GetCurrentMonitor()) * 0.8f / map.Width,
    Raylib.GetMonitorHeight(Raylib.GetCurrentMonitor()) * 0.8f / map.Height));
var spacing = Math.Max(1, scale / 10);
scale -= spacing;
Raylib.CloseWindow();
Raylib.ClearWindowState(ConfigFlags.HiddenWindow);

Raylib.InitWindow(map.Width * (scale + spacing) + 20, map.Height * (scale + spacing) + 20, "AoC 2025 - Day 4");

RenderMap(map);

sw.Restart();
var accessible = RemoveRolls(map);
accessible.DumpAndAssert("Part 1", 13, 1533);
var part1Time = sw.Elapsed;

sw.Restart();
int totalRemoved = accessible, removed;
while ((removed = RemoveRolls(map)) > 0)
{
    totalRemoved += removed;
}

totalRemoved.DumpAndAssert("Part 2", 43, 9206);
var part2Time = sw.Elapsed;

OutputHelpers.PrintTimings(parseTime, part1Time, part2Time);

var cycles = 10;
while (!Raylib.WindowShouldClose())
{
    if (cycles <= 0)
        Raylib.EnableEventWaiting();
    else
        cycles--;
    RenderMap(map);
}
Console.WriteLine("Exiting...");

int RemoveRolls(Map map)
{
    foreach (var point in map.AllPoints())
    {
        if (map[point] != '@')
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
        if (map[point] == 'x')
        {
            accessible++;
            map[point] = '9';
        }
    }

    RenderMap(map);

    return accessible;
}

void RenderMap(Map map)
{
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);
    foreach (var point in map.AllPoints())
    {
        var color = map[point] switch
        {
            '@' => Color.Gold,
            >= '1' and <= '9' => new Color(
                Color.Green.R - (Color.Green.R - Color.DarkGray.R) * (map[point] - '1') / 8,
                Color.Green.G - (Color.Green.G - Color.DarkGray.G) * (map[point] - '1') / 8,
                Color.Green.B - (Color.Green.B - Color.DarkGray.B) * (map[point] - '1') / 8),
            '.' => Color.DarkGray,
            _ => Color.DarkGray
        };
        if (map[point] is >= '1' and <= '9')
        {
            map[point]--;
        }
        if (map[point] == '0')
        {
            map[point] = '.';
        }

        Raylib.DrawRectangle(
            point.X * (scale + spacing) + 10,
            point.Y * (scale + spacing) + 10,
            scale,
            scale,
            color);
    }
    Raylib.EndDrawing();
    Raylib.WaitTime(0.03f);
}
