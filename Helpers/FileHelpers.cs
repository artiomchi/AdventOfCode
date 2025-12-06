using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace AoC.Helpers;

public class FileHelpers
{
    public static string ReadInputText(string fileName, [CallerFilePath] string scriptPath = null!)
        => File.ReadAllText(GetInputFilePath(fileName, scriptPath));
    public static string[] ReadInputLines(string fileName, [CallerFilePath] string scriptPath = null!)
        => File.ReadAllLines(GetInputFilePath(fileName, scriptPath));

    private static string GetInputFilePath(string fileName, string scriptPath)
    {
        var filePath = Path.Combine(Path.GetDirectoryName(scriptPath)!, ".inputs", fileName);
        if (!File.Exists(filePath))
        {
            var match = Regex.Match(fileName, @"^(\d{2})(\.sample)?\.txt$", RegexOptions.IgnoreCase);
            if (!match.Success)
                throw new FileNotFoundException($"Input file not found: {filePath}");

            if (match.Groups[2].Success)
                throw new FileNotFoundException($"Sample input file not found: {filePath}");

            var yearPart = Path.GetFileName(Path.GetDirectoryName(scriptPath)!);
            if (!int.TryParse(yearPart, out var year))
                throw new FileNotFoundException($"Script is not located in a year folder: {scriptPath}");

            var sessionFileName = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(scriptPath)!, "..", ".session"));
            if (!File.Exists(sessionFileName))
                throw new FileNotFoundException($"Session file not found: {sessionFileName}. Can not download new input file.");

            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, $"https://adventofcode.com/{year}/day/{int.Parse(match.Groups[1].Value)}/input");
            request.Headers.Add("Cookie", $"session={File.ReadAllText(sessionFileName).Trim()}");
            request.Headers.TryAddWithoutValidation("User-Agent", "AoC input downloader for github.com/artiomchi/AdventOfCode");
            using var response = client.SendAsync(request).GetAwaiter().GetResult();
            response.EnsureSuccessStatusCode();

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            File.WriteAllText(filePath, response.Content.ReadAsStringAsync().GetAwaiter().GetResult().TrimEnd());
            Console.WriteLine($"Downloaded input file to: {filePath}");
        }

        return filePath;
    }
}
