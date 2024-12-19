var input = File.ReadAllLines("input.txt");
var patterns = input[0].Split(", ");
var designs = input.Skip(2).ToArray();

var cacheWaysToMakeDesign = new Dictionary<string, long>();
var waysToMakeEachDesign = designs.Select(GetNumberOfWaysToMakeDesign).ToList();

var part1 = waysToMakeEachDesign.Count(n => n > 0);
Console.WriteLine(part1);

var part2 = waysToMakeEachDesign.Sum();
Console.WriteLine(part2);

return;

long GetNumberOfWaysToMakeDesign(string design)
{
    if (design.Length == 0)
    {
        return 1;
    }

    if (cacheWaysToMakeDesign.TryGetValue(design, out var ways))
    {
        return ways;
    }

    ways = patterns.Where(design.StartsWith)
        .Sum(pattern => GetNumberOfWaysToMakeDesign(design[pattern.Length..]));
    cacheWaysToMakeDesign[design] = ways;
    return ways;
}