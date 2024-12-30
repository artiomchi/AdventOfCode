#nullable enable

using System.Runtime.CompilerServices;

public static T Dump<T>(this T instance, string? title = null)
{
    if (title != null)
    {
        Console.WriteLine(title + ":");
    }
    Console.WriteLine(instance);
    return instance;
}

public static string InputFilePath(string fileName, [CallerFilePath] string scriptPath = null!)
	=> Path.Combine(Path.GetDirectoryName(scriptPath)!, "inputs", fileName);
public static string ReadInputText(string fileName, [CallerFilePath] string scriptPath = null!)
	=> File.ReadAllText(InputFilePath(fileName, scriptPath));
public static string[] ReadInputLines(string fileName, [CallerFilePath] string scriptPath = null!)
	=> File.ReadAllLines(InputFilePath(fileName, scriptPath));

readonly record struct Point(int x, int y)
{
	public static readonly char[] Directions = ['^', '>', 'v', '<'];
	
	public static implicit operator Point((int x, int y) p) => new Point(p.x, p.y);

	//public static Point operator *(Point p, long multiplier) => new Point(p.x * multiplier, p.y * multiplier);
	//public static Point operator /(Point p, long multiplier) => new Point(p.x / multiplier, p.y / multiplier);
	//public static long operator /(Point a, Point b) => Math.Min(a.x / b.x, a.y / b.y);

	public static Point operator +(Point a, Point b) => new Point(a.x + b.x, a.y + b.y);
	public static Point operator +(Point a, (int x, int y) b) => new Point(a.x + b.x, a.y + b.y);
	public static Point operator -(Point a, Point b) => new Point(a.x - b.x, a.y - b.y);
	public static Point operator -(Point a, (int x, int y) b) => new Point(a.x - b.x, a.y - b.y);
	public static bool operator >(Point a, Point b) => a.x > b.x || a.y > b.y;
	public static bool operator <(Point a, Point b) => a.x < b.x || a.y < b.y;
	
	public static Point operator +(Point p, Velocity v) => new Point(p.x + v.x, p.y + v.y);

	public Point Move(char direction)
		=> direction switch
		{
			'^' or 'w' or '8' => (x, y - 1),
			'>' or 'd' or '6' => (x + 1, y),
			'v' or 's' or '5' or '2' => (x, y + 1),
			'<' or 'a' or '4' => (x - 1, y),
			_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown value for direction"),
		};
	public Point BoundedMove(Map map, char direction)
	{
		var result = Move(direction);
		return map.IsInBounds(result) ? result : this;
	}
		
	public static char ReverseDirection(char direction)
		=> direction switch
		{
			'^' => 'v',
			'>' => '<',
			'v' => '^',
			'<' => '>',
			_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Unknown value for direction"),
		};
}

partial record struct Velocity(int x, int y)
{
	public static implicit operator Velocity((int x, int y) v) => new Velocity(v.x, v.y);
	public static Velocity operator *(Velocity p, int multiplier) => new Velocity(p.x * multiplier, p.y * multiplier);
}


partial class Map(int x, int y)
{
	private char[,]? _savePoint;
	public char[,] Matrix { get; private set; } = new char[x, y];
	public int Width => x;
	public int Height => y;
	
	public char this[int x, int y] { get => Matrix[x, y]; set => Matrix[x, y] = value; }
	public char this[Point p] { get => Matrix[p.x, p.y]; set => Matrix[p.x, p.y] = value; }

	public Map Fill(char value = ' ', char[]? walls = null)
	{
		for (var x = 0; x < Width; x++)
			for (var y = 0; y < Height; y++)
				if (walls == null || !walls.Contains(Matrix[x, y]))
					Matrix[x, y] = value;
		return this;
	}

	public Point? TryFind(char value)
	{
		for (var x = 0; x < Width; x++)
			for (var y = 0; y < Height; y++)
				if (Matrix[x, y] == value)
					return (x, y);
		return null;
	}
	
	public Point Find(char value)
		=> TryFind(value) ?? throw new Exception("Could not find point");

	public static Map FromString(string source)
	{
		var lines = source.Split(Environment.NewLine);
		var map = new Map(lines[0].Length, lines.Length);
		foreach (var (y, line) in lines.Index())
			foreach (var (x, c) in line.Index())
				map[x, y] = c;
		map.Save();
		return map;
	}
	
	public void Save()
	{
		_savePoint = (char[,])Matrix.Clone();
	}
	
	public void Reset()
	{
		if (_savePoint is null)
			throw new InvalidOperationException("Not saved yet");
		Matrix = (char[,])_savePoint.Clone();
	}

	public bool IsInBounds(Point point)
		=> 0 <= point.x && point.x < Width && 0 <= point.y && point.y < Height;

	// public object ToImage(int percentScale) => ToImage(Util.ScaleMode.ResizeTo(Convert.ToInt32(Width) * percentScale / 100));
	// public object ToImage(Util.ScaleMode scaleMode = default)
	// {
	// 	if (Width > int.MaxValue || Height > int.MaxValue)
	// 		throw new InvalidOperationException("Map is too large to render");
			
	// 	using var bitmap = new Bitmap(Convert.ToInt32(Width), Convert.ToInt32(Height));
	// 	for (var x = 0; x < Height; x++)
	// 		for (var y = 0; y < Width; y++)
	// 			if (Matrix[x, y] != ' ')
	// 				bitmap.SetPixel(x, y, Color.White);
		
	// 	using var ms = new MemoryStream();
	// 	bitmap.Save(ms, ImageFormat.Png);
	// 	ms.Position = 0;
	// 	return Util.Image(ms.ToArray(), scaleMode);
	// }
	
	// public object ToPrettyString(Func<char, string?> colourFunc)
	// {
	// 	var result = new StringBuilder();
	// 	for (var x = 0; x < Height; x++)
	// 	{
	// 		for (var y = 0; y < Width; y++)
	// 		{
	// 			var c = Matrix[x,y];
	// 			var cstring = c == ' ' ? "&nbsp;" : c.ToString();
	// 			var colour = colourFunc(c);
	// 			if (colour is not null)
	// 			{
	// 				result.Append($@"<font color=""{colour}"">{cstring}</font>");
	// 			}
	// 			else
	// 			{
	// 				result.Append(cstring);
	// 			}
	// 		}
	// 		result.AppendLine("<br/>");
	// 	}
	// 	return Util.RawHtml(result.ToString());
	// }
	
	// public object ToPrettyString2(Func<char, string?> colourFunc)
	// {
	// 	var result = new StringBuilder();
	// 	for (var x = 0; x < Height; x++)
	// 	{
	// 		for (var y = 0; y < Width; y++)
	// 		{
	// 			var c = Matrix[x,y];
	// 			var cstring = c == ' ' ? "&nbsp;" : c.ToString();
	// 			var colour = colourFunc(c);
	// 			if (colour is not null)
	// 			{
	// 				result.Append($@"<font color=""{colour}"">{cstring}</font>");
	// 			}
	// 			else
	// 			{
	// 				result.Append(cstring);
	// 			}
	// 		}
	// 		result.AppendLine("<br/>");
	// 	}
	// 	return Util.RawHtml(result.ToString());
	// }
	
	public override string ToString()
	{
		var result = new StringBuilder();
		for (var y = 0; y < Height; y++)
		{
			for (var x = 0; x < Width; x++)
				result.Append(Matrix[x, y]);
			result.AppendLine();
		}
		return result.ToString();
	}

	// public object ToDump()
	// {
	// 	return Util.FixedFont(ToString());
	// }
}
