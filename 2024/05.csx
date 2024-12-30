#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;

var lines = File.ReadAllLines("2024/inputs/05.real.txt");
var pageRules = lines.TakeWhile(l => l.Length > 0).Select(l => l.Split('|').Select(i => Convert.ToInt32(i)).ToArray()).ToLookup(l => l[1], l => l[0]);
var updates = lines.SkipWhile(l => l.Length > 0).Skip(1).Select(l => l.Split(',').Select(i => Convert.ToInt32(i)).ToArray()).ToArray();

// Part 1
var totalValid = updates.Where(isUpdateValid).Sum(u => u[u.Length / 2]);
totalValid.Dump("Part 1").Should().BeOneOf(143, 5329);

// Part 2
var totalInvalid = updates
	.Where(u => !isUpdateValid(u))
	.Sum(u => u
		.Order(Comparer<int>.Create((a, b) => a == b ? 0 : pageRules[a].Contains(b) ? 1 : -1))
		.ElementAt(u.Length / 2));
totalInvalid.Dump("Part 2").Should().BeOneOf(123, 5833);

bool isUpdateValid(int[] update)
	=> !update.Index().Any(x => pageRules[x.Item].Intersect(update.Skip(x.Index)).Any());