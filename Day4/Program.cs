var grid = File.ReadAllLines("input.txt");

var part1 = FindPattern([
    "XMAS"
]) + FindPattern([
    "X",
    "M",
    "A",
    "S"
]) + FindPattern([
    "X   ",
    " M  ",
    "  A ",
    "   S"
]);
Console.WriteLine(part1);

var part2 = FindPattern([
    "M M",
    " A ",
    "S S"
]) + FindPattern([
    "M S",
    " A ",
    "M S"
]);
Console.WriteLine(part2);

return;

int FindPattern(string[] basePattern)
{
    var patterns = new HashSet<string[]>(PatternEqualityComparer.Instance)
    {
        basePattern,
        basePattern.Reverse().ToArray()
    };

    var horizontalFlip = basePattern.Select(row => new string(row.Reverse().ToArray())).ToArray();
    patterns.Add(horizontalFlip);
    patterns.Add(horizontalFlip.Reverse().ToArray());

    var count = 0;
    foreach (var pattern in patterns)
    {
        for (var y = 0; y < grid.Length - pattern.Length + 1; y++)
        {
            for (var x = 0; x < grid[y].Length - pattern[0].Length + 1; x++)
            {
                if (IsMatch(y, x, pattern))
                {
                    count++;
                }
            }
        }
    }

    return count;
}

bool IsMatch(int y, int x, string[] pattern)
{
    for (var i = 0; i < pattern.Length; i++)
    {
        for (var j = 0; j < pattern[0].Length; j++)
        {
            if (pattern[i][j] != ' ' && pattern[i][j] != grid[y + i][x + j])
            {
                return false;
            }
        }
    }

    return true;
}

internal class PatternEqualityComparer : IEqualityComparer<string[]>
{
    public static PatternEqualityComparer Instance { get; } = new();

    private PatternEqualityComparer()
    {
    }

    public bool Equals(string[]? x, string[]? y) =>
        ReferenceEquals(x, y) || x != null && y != null && x.SequenceEqual(y);

    public int GetHashCode(string[] obj) => obj.Aggregate(0, HashCode.Combine);
}