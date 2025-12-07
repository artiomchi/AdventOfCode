namespace AoC.Helpers;

public sealed class Map(int width, int height)
{
    private char[,]? _savePoint;
    public char[,] Matrix { get; private set; } = new char[width, height];
    public int Width => width;
    public int Height => height;

    public char this[int x, int y] { get => Matrix[x, y]; set => Matrix[x, y] = value; }
    public char this[Point p] { get => Matrix[p.X, p.Y]; set => Matrix[p.X, p.Y] = value; }

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
        => FromLines(source.Split(Environment.NewLine));
    public static Map FromLines(string[] lines)
    {
        var map = new Map(lines[0].Length, lines.Length);
        foreach (var (y, line) in lines.Index())
            foreach (var (x, c) in line.Index())
                map[x, y] = c;
        map.Save();
        return map;
    }

    public IEnumerable<Point> AllPoints()
    {
        for (var y = 0; y < Height; y++)
            for (var x = 0; x < Width; x++)
                yield return new Point(x, y);
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

    public override string ToString()
    {
        var result = new System.Text.StringBuilder();
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
                result.Append(Matrix[x, y]);
            result.AppendLine();
        }
        return result.ToString();
    }
}
