using System.Collections;

var reports = File.ReadAllLines("input.txt")
    .Select(line => line.Split().Select(int.Parse).ToArray())
    .ToList();

var part1 = reports.Count(IsSafe);
Console.WriteLine(part1);

var part2 = reports.Count(IsSafeWithDampener);
Console.WriteLine(part2);

return;

bool IsSafeWithDampener(IReadOnlyList<int> report) =>
    IsSafe(report) || Enumerable.Range(0, report.Count)
        .Select(i => new SkippedIndexReadOnlyIntList(report, i))
        .Any(IsSafe);

bool IsSafe(IReadOnlyList<int> report)
{
    var diffSign = Math.Sign(report[1] - report[0]);
    for (var i = 1; i < report.Count; i++)
    {
        var diff = report[i] - report[i - 1];
        if (diff * diffSign <= 0 || Math.Abs(diff) is not (>= 1 and <= 3))
        {
            return false;
        }
    }

    return true;
}

internal class SkippedIndexReadOnlyIntList(IReadOnlyList<int> list, int skippedIndex) : IReadOnlyList<int>
{
    private readonly IReadOnlyList<int> list = list;

    public int this[int i] => list[i < skippedIndex ? i : i + 1];

    public int Count => list.Count - 1;

    public IEnumerator<int> GetEnumerator() => throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
