#load "..\Helpers.csx"

var lines = ReadInputLines("01.real.txt");
var sw = Stopwatch.StartNew();
var (list1, list2) = lines.Aggregate(
    (l1: new List<int>(), l2: new List<int>()),
    (x, l) =>
    {
        var nums = l.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        x.l1.Add(int.Parse(nums[0]));
        x.l2.Add(int.Parse(nums[1]));
        return x;
    });
list1.Sort();
list2.Sort();
var parseTime = sw.Elapsed;

// Part 1
sw.Restart();
Enumerable.Range(0, list1.Count)
    .Aggregate(0, (s, i) => s + Math.Abs(list1[i] - list2[i]))
    .DumpAndAssert("Part 1", 11, 1830467);
var part1Time = sw.Elapsed;

// Part 2
sw.Restart();
var list2Counts = list2.GroupBy(i => i).ToDictionary(g => g.Key, g => g.Count());
Enumerable.Range(0, list1.Count)
    .Aggregate(0, (s, i) => s + list1[i] * list2Counts.GetValueOrDefault(list1[i]))
    .DumpAndAssert("Part 2", 31, 26674158);
var part2Time = sw.Elapsed;

PrintTimings(parseTime, part1Time, part2Time);
