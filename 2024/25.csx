#nullable enable
#load "..\Helpers.csx"
#r "nuget: System.Numerics.Tensors, 9.0.0"
using System.Numerics.Tensors;

var input = ReadInputText("25.real.txt");
var sw = Stopwatch.StartNew();
var lines = input.Split(Environment.NewLine + Environment.NewLine);
var locks = lines.Where(l => l[0] == '#').Select(ParseLock);
var keys = lines.Where(l => l[0] == '.').Select(ParseLock);
var parseTime = sw.Elapsed;

sw.Restart();
var matches =
    from l in locks
    from k in keys
    where IsMatch(l, k)
    select (l, k);

matches.Count().DumpAndAssert("Part 1", 3, 3196);
var solveTime = sw.Elapsed;

PrintTimings(parseTime, solveTime);

bool IsMatch(int[] l, int[] k)
{
    var tmp = new int[5];
    TensorPrimitives.Add<int>(l, k, tmp);
    return tmp.All(i => i <= 5);
}

int[] ParseLock(string input)
{
    var lines = input.Split(Environment.NewLine);
    return Enumerable.Range(0, 5)
        .Select(i => lines
            .Select(l => l[i])
            .Count(i => i == '#') - 1)
        .ToArray();
}
