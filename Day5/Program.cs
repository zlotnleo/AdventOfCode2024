var input = File.ReadAllText("input.txt").Split("\n\n");
var orderingRules = input[0].Split('\n')
    .Select(l => l.Split('|').Select(int.Parse).ToArray())
    .GroupBy(split => split[0])
    .ToDictionary(g => g.Key, g => g.Select(rule => rule[1]).ToHashSet());
var pagesToProduce = input[1].Split('\n')
    .Select(l => l.Split(',').Select(int.Parse).ToList());

var pageComparer = new PageComparer(orderingRules);
var validityPartition = pagesToProduce.Select(pages =>
    {
        var isValid = Enumerable.Range(0, pages.Count - 1).All(i => pageComparer.Compare(pages[i], pages[i + 1]) <= 0);
        if (!isValid)
        {
            pages.Sort(pageComparer);
        }

        return (IsValid: isValid, MiddlePage: pages[pages.Count / 2]);
    })
    .GroupBy(t => t.IsValid, t => t.MiddlePage)
    .ToDictionary(g => g.Key, g => g.Sum());

var part1 = validityPartition[true];
Console.WriteLine(part1);

var part2 = validityPartition[false];
Console.WriteLine(part2);

internal class PageComparer(Dictionary<int, HashSet<int>> orderingRules) : IComparer<int>
{
    public int Compare(int x, int y) =>
        orderingRules.TryGetValue(x, out var hasToBeAfterX) && hasToBeAfterX.Contains(y) ? -1 :
        orderingRules.TryGetValue(y, out var hasToBeAfterY) && hasToBeAfterY.Contains(x) ? 1 : 0;
}