#:package CliWrap@3.7.0
using System.Runtime.CompilerServices;
using CliWrap;

// Unfinished days
int[] excluded = [];

int firstDay = 1, lastDay = 25;
var daysArg = args.FirstOrDefault(a => a.All(c => c == '.' || char.IsDigit(c)));
if (daysArg != null)
{
    if (int.TryParse(daysArg, out var day))
    {
        firstDay = lastDay = day;
    }
    else if (daysArg.StartsWith("..") && int.TryParse(daysArg[2..], out day))
    {
        lastDay = day;
    }
    else if (daysArg.EndsWith("..") && int.TryParse(daysArg[..^2], out day))
    {
        firstDay = day;
    }
    else if (
        (day = daysArg.IndexOf('.')) > 0 &&
        daysArg[day..(day + 2)] == ".." &&
        int.TryParse(daysArg[..day], out var day1) &&
        int.TryParse(daysArg[(day + 2)..], out var day2))

    {
        firstDay = day1;
        lastDay = day2;
    }
}
var days = Enumerable.Range(firstDay, lastDay - firstDay + 1).Except(excluded).ToArray();

if (args?.Contains("clean") == true)
{
    Console.WriteLine("Cleaning...");
    Directory.Delete(Path.Combine(GetScriptFolder(), "publish"), true);
}

Console.WriteLine("Pre-compiling...");
foreach (var d in days)
{
    if (File.Exists(Path.Combine(GetScriptFolder(), $"publish\\{d:00}.dll")))
        continue;
    await Cli.Wrap("dotnet")
        .WithArguments($"build -c Release {d:00}.cs")
        .WithWorkingDirectory(GetScriptFolder())
        .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
        .WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput()))
        .ExecuteAsync();
}

foreach (var d in days)
{
    Console.WriteLine(new string('#', 30));
    Console.WriteLine($"Day {d:00}:");
    var result = await Cli.Wrap("dotnet")
        .WithArguments($"run -c Release {d:00}.cs")
        .WithWorkingDirectory(GetScriptFolder())
        .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
        .WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput()))
        .ExecuteAsync();
    Console.WriteLine();
}

static string GetScriptFolder([CallerFilePath] string scriptPath = null!)
    => Path.GetDirectoryName(scriptPath)!;
