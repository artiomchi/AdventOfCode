#nullable enable
#load "..\Helpers.csx"
#r "nuget: FluentAssertions, 7.0.0"
using FluentAssertions;

var lines = File.ReadAllLines("2024/inputs/23.real.txt");
var input = lines.Select(i => i.Split('-')).ToArray();
var tpcs = input.SelectMany(i => i).Where(i => i[0] == 't').Distinct().ToArray();

var crossMap = new Dictionary<string, HashSet<string>>();
foreach (var set in input)
{
	AddToSet(set[0], set[1]);
	AddToSet(set[1], set[0]);
	
	void AddToSet(string one, string two)
	{
		if (!crossMap.TryGetValue(one, out var connected))
		{
			crossMap[one] = connected = new();
		}
		if (!connected.Contains(two))
		{
			connected.Add(two);
		}
	}
}

var searchCache = new HashSet<string>();
var allSets = crossMap.Keys
	.Where(k => k[0] == 't')
	.SelectMany(pc => GetSets(searchCache, new HashSet<string> { pc }, pc))
	.Where(s => s.Count >= 3)
	.Distinct(HashSet<string>.CreateSetComparer())
	.OrderBy(s => s.Order().First())
	.ThenBy(s => s.Order().Skip(1).First())
	.ToArray();
	
$"Total networks with 3 PCs or more: {allSets.Length:N0}".Dump();

allSets.Where(s => s.Count == 3).Count().Dump("Part 1").Should().BeOneOf(7, 1314);

string.Join(",", allSets.OrderByDescending(s => s.Count).First().Order()).Dump("Part 2").Should().BeOneOf("co,de,ka,ta", "bg,bu,ce,ga,hw,jw,nf,nt,ox,tj,uu,vk,wp");


IEnumerable<HashSet<string>> GetSets(HashSet<string> searchCache, HashSet<string> set, string latest)
{
	foreach (var pc in crossMap[latest])
	{
		if (set.Contains(pc))
			continue;
			
		var found = false;
		var connected = crossMap[pc];
		if (set.All(connected.Contains))
		{
			var cacheKey = string.Join(",",  set.Order().Append(pc));
			if (searchCache.Contains(cacheKey))
				continue;

			searchCache.Add(cacheKey);
			foreach (var newSet in GetSets(searchCache, new HashSet<string>(set.Append(pc)), pc))
			{
				found = true;
				yield return newSet;					
			}
		}
		
		if (!found)
		{
			yield return set;
		}
	}
}