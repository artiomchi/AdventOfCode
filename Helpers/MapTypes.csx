#nullable enable

readonly public partial record struct Point(int X, int Y)
{
    public static implicit operator Point((int x, int y) p) => new(p.x, p.y);

    public static Point operator +(Point p, Vector v) => new(p.X + v.X, p.Y + v.Y);
    public static Point operator +(Point p, (int x, int y) v) => new(p.X + v.x, p.Y + v.y);
    public static Point operator -(Point p, Vector v) => new(p.X - v.X, p.Y - v.Y);
    public static Point operator -(Point p, (int x, int y) v) => new(p.X - v.x, p.Y - v.y);
    public static Vector operator -(Point a, Point b) => new(a.X - b.X, a.Y - b.Y);
    public static bool operator >(Point a, Point b) => a.X > b.X || a.Y > b.Y;
    public static bool operator <(Point a, Point b) => a.X < b.X || a.Y < b.Y;

    public Point Move(Vector direction, int length = 1)
        => this + direction * length;

    public bool IsInBounds(Map map)
        => IsInBounds(map.Width, map.Height);

    public bool IsInBounds(int width, int height)
        => 0 <= X && X < width && 0 <= Y && Y < height;
}

readonly public partial record struct Vector(int X, int Y)
{
    public static readonly Vector[] Directions = ['^', '>', 'v', '<'];
    public static readonly Vector[] DirectionsAll = ['6', '3', '2', '1', '4', '7', '8', '9'];

    public static implicit operator Vector((int x, int y) v) => new(v.x, v.y);
    public static implicit operator Vector(char direction)
        => direction switch
        {
            '^' or 'w' or '8' => (0, -1),
            '>' or 'd' or '6' => (1, 0),
            'v' or 's' or '5' or '2' => (0, 1),
            '<' or 'a' or '4' => (-1, 0),

            '9' => (1, -1),
            '3' => (1, 1),
            '1' => (-1, 1),
            '7' => (-1, -1),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Invalid direction value")
        };

    public static Vector operator *(Vector p, int multiplier) => new(p.X * multiplier, p.Y * multiplier);

    public Vector ReverseDirection()
        => (X switch { 1 => -1, -1 => 1, _ => X },
            Y switch { 1 => -1, -1 => 1, _ => Y });

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

public partial class Map(int width, int height)
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
        var result = new StringBuilder();
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
                result.Append(Matrix[x, y]);
            result.AppendLine();
        }
        return result.ToString();
    }
}
