#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;
using System.Buffers;

var lines = File.ReadAllLines("2024/inputs/19.real.txt");
var towels = lines[0].Split([' ', ','], StringSplitOptions.RemoveEmptyEntries).OrderByDescending(t => t.Length).ToArray();
var patterns = lines.Skip(2);

var sv = SearchValues.Create(towels, StringComparison.Ordinal);

var knownSets = new Dictionary<string, long>();

long possible = 0, combinations = 0;
foreach (var (index, pattern) in patterns.Index())
{
	//Util.Progress = index * 100 / patterns.Count();
	
	var matchingTowels = towels.Where(t => t.All(pattern.Contains)).ToArray();
	if (matchingTowels.Length == 0)
		continue;
		
	var localCombos = BuildMatch(pattern.AsMemory(), matchingTowels);
	if (localCombos > 0)
		possible++;
		
	combinations += localCombos;
	
	long BuildMatch(ReadOnlyMemory<char> pattern, string[] towels)
	{
		if (knownSets.TryGetValue(pattern.ToString(), out var result))
		{
			return result;
		}
		
		var directMatches = towels.Count(t => t.Length == pattern.Length && pattern.Span.SequenceEqual(t));
		var subMatches = towels
			.Where(t => pattern.Length > t.Length && pattern.Span[0] == t[0])
			.Where(t => pattern[..t.Length].Span.SequenceEqual(t))
			.Sum(t => BuildMatch(pattern[t.Length..], towels));
			
		result = directMatches + subMatches;
		knownSets[pattern.ToString()] = result;

		return result;
	}
}

possible.Dump("Part 1").Should().BeOneOf(6, 283);
combinations.Dump("Part 2").Should().BeOneOf(16, 615388132411142);