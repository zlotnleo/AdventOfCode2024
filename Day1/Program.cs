var firstList = new List<int>();
var secondList = new List<int>();
foreach (var line in File.ReadAllLines("input.txt"))
{
    var values = line.Split("   ");
    firstList.Add(int.Parse(values[0]));
    secondList.Add(int.Parse(values[1]));
}

firstList.Sort();
secondList.Sort();

var part1 = firstList.Zip(secondList, (first, second) => Math.Abs(first - second)).Sum();
Console.WriteLine(part1);

var secondListCounts = secondList.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
var part2 = firstList.Select(x => x * secondListCounts.GetValueOrDefault(x, 0)).Sum();
Console.WriteLine(part2);
