using System.Collections.Immutable;

var directions = new[] { (0, -1), (1, 0), (0, 1), (-1, 0) };

var initialGrid = File.ReadAllLines("input.txt")
    .Select(row => row.ToImmutableList())
    .ToImmutableList();

var guard = FindGuard();

var part1 = Patrol(initialGrid).VisitedStates.Select(state => (state.X, state.Y)).Distinct().Count();
Console.WriteLine(part1);

var part2 = initialGrid.SelectMany((row, y) =>
        Enumerable.Range(0, row.Count).Where(x => row[x] != '#')
            .Select(x => initialGrid.SetItem(y, row.SetItem(x, '#')))
    ).AsParallel()
    .Count(updatedGrid => Patrol(updatedGrid).HasLoop);
Console.WriteLine(part2);

return;

(bool HasLoop, HashSet<(int X, int Y, int DirIndex)> VisitedStates) Patrol(
    IReadOnlyList<IReadOnlyList<char>> grid)
{
    var dirIndex = 0;
    var (x, y) = guard;
    var visited = new HashSet<(int, int, int)>();
    while (visited.Add((x, y, dirIndex)))
    {
        var (dx, dy) = directions[dirIndex];
        var nextX = x + dx;
        var nextY = y + dy;

        if (nextY < 0 || nextY >= grid.Count || nextX < 0 || nextX >= grid[0].Count)
        {
            return (false, visited);
        }

        if (grid[nextY][nextX] == '#')
        {
            dirIndex = (dirIndex + 1) % directions.Length;
        }
        else
        {
            x = nextX;
            y = nextY;
        }
    }

    return (true, visited);
}

(int X, int Y) FindGuard()
{
    foreach (var (y, row) in initialGrid.Index())
    {
        var x = row.IndexOf('^');
        if (x >= 0)
        {
            return (x, y);
        }
    }

    throw new Exception("No guard");
}