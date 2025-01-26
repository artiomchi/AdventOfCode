#nullable enable
#load "Helpers\OutputHelpers.csx"
#load "Helpers\MapTypes.csx"

using System.Runtime.CompilerServices;

public static string InputFilePath(string fileName, [CallerFilePath] string scriptPath = null!)
    => Path.Combine(Path.GetDirectoryName(scriptPath)!, "inputs", fileName);
public static string ReadInputText(string fileName, [CallerFilePath] string scriptPath = null!)
    => File.ReadAllText(InputFilePath(fileName, scriptPath));
public static string[] ReadInputLines(string fileName, [CallerFilePath] string scriptPath = null!)
    => File.ReadAllLines(InputFilePath(fileName, scriptPath));
