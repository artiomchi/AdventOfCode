#r "nuget: CliWrap, 3.7.0"
using System.Runtime.CompilerServices;
using CliWrap;

// Unfinished days
int[] excluded = [24];

int firstDay = 1, lastDay = 25;
var daysArg = Args.FirstOrDefault(a => a.All(c => c == '.' || char.IsDigit(c)));
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

if (Args?.Contains("clean") == true)
{
    WriteLine("Cleaning...");
    Directory.Delete(Path.Combine(GetScriptFolder(), "publish"), true);
}

WriteLine("Pre-compiling...");
foreach (var d in days)
{
    if (File.Exists(Path.Combine(GetScriptFolder(), $"publish\\{d:00}.dll")))
        continue;
    await Cli.Wrap("dotnet")
        .WithArguments($"script publish -c Release --dll {d:00}.csx")
        .WithWorkingDirectory(GetScriptFolder())
        .WithStandardErrorPipe(PipeTarget.ToStream(OpenStandardError()))
        .WithStandardOutputPipe(PipeTarget.ToStream(OpenStandardOutput()))
        .ExecuteAsync();
}

foreach (var d in days)
{
    WriteLine(new string('#', 30));
    WriteLine($"Day {d:00}:");
    var result = await Cli.Wrap("dotnet.exe")
        .WithArguments($"script exec publish\\{d:00}.dll")
        .WithWorkingDirectory(GetScriptFolder())
        .WithStandardErrorPipe(PipeTarget.ToStream(OpenStandardError()))
        .WithStandardOutputPipe(PipeTarget.ToStream(OpenStandardOutput()))
        .ExecuteAsync();
    WriteLine();
}

public static string GetScriptFolder([CallerFilePath] string scriptPath = null!)
    => Path.GetDirectoryName(scriptPath)!;
