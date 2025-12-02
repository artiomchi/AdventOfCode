#!/usr/bin/dotnet run
#:project ../Helpers/AoC.Helpers.csproj
using AoC.Helpers;

#region Asserts

static void AssertMove(int position, string move, (int position, int zeroes) expected)
{
    var result = ProcessMove(position, move);
    if (result != expected)
    {
        throw new Exception($"Assertion failed for position {position} and move {move}. Expected {expected}, got {result}.");
    }
}

AssertMove(50, "R1000", (50, 10));
AssertMove(50, "R50", (0, 1));
AssertMove(50, "L50", (0, 1));
AssertMove(0, "R50", (50, 0));
AssertMove(0, "L50", (50, 0));
AssertMove(1, "L51", (50, 1));
AssertMove(1, "L151", (50, 2));
AssertMove(1, "L101", (0, 2));
AssertMove(99, "R51", (50, 1));
AssertMove(99, "R151", (50, 2));
AssertMove(99, "R1", (0, 1));
AssertMove(99, "R101", (0, 2));

#endregion

var inputs = FileHelpers.ReadInputLines("01.real.txt");
var position = 50;
var resets = 0;
var zeroes = 0;

foreach (var move in inputs)
{
    var (positionAfterMove, zeroesAfterMove) = ProcessMove(position, move);
    position = positionAfterMove;
    zeroes += zeroesAfterMove;
    resets += position == 0 ? 1 : 0;

    //Console.WriteLine($"Steps: {move}, Position: {position}, Resets: {resets}, Zeroes: {zeroes}");
}

Console.WriteLine($"Final Position: {position}");
Console.WriteLine($"Resets: {resets}");
Console.WriteLine($"Zeroes: {zeroes}");

static (int position, int zeroes) ProcessMove(int position, string move)
{
    int zeroes = 0, initial = position;

    var steps = int.Parse(move[1..]);
    if (steps > 100)
    {
        zeroes += steps / 100;
        steps %= 100;
    }

    if (steps > 0)
    {
        position += move[0] == 'R' ? steps : -steps;
        if (position == 0 ||
            position >= 100 ||
            (position < 0 && initial != 0))
        {
            zeroes++;
        }

        position = position switch
        {
            -100 or 100 => 0,
            < 0 => position + 100,
            > 100 => position - 100,
            _ => position
        };
    }

    return (position, zeroes);
}
