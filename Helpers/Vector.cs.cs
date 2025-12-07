namespace AoC.Helpers;

readonly public record struct Vector(int X, int Y)
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
