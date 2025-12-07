namespace AoC.Helpers;

readonly public record struct Point(int X, int Y)
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
