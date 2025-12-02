using System.Runtime.CompilerServices;

namespace AoC.Helpers;

public class FileHelpers
{
    public static string InputFilePath(string fileName, [CallerFilePath] string scriptPath = null!)
        => Path.Combine(Path.GetDirectoryName(scriptPath)!, "inputs", fileName);
    public static string ReadInputText(string fileName, [CallerFilePath] string scriptPath = null!)
        => File.ReadAllText(InputFilePath(fileName, scriptPath));
    public static string[] ReadInputLines(string fileName, [CallerFilePath] string scriptPath = null!)
        => File.ReadAllLines(InputFilePath(fileName, scriptPath));
}
