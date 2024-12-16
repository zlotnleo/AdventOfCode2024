using System.Collections.Immutable;

var map = File.ReadAllLines("input.txt");
var start = Find('S');
var end = Find('E');

int? lowestCost = null;
var shortestPathsTiles = new HashSet<(int, int)>();

var directions = new[] { (1, 0), (0, 1), (-1, 0), (0, -1) };
var visitedAtCost = new Dictionary<(int, int, int), int>();
var pq = new PriorityQueue<(int, int, int, ImmutableStack<(int, int)>), int>();
pq.Enqueue((start.X, start.Y, 0, ImmutableStack<(int, int)>.Empty.Push(start)), 0);

while (pq.TryDequeue(out var state, out var cost))
{
    var (x, y, dirIndex, path) = state;
    if (x == end.X && y == end.Y)
    {
        lowestCost ??= cost;
        if (cost > lowestCost)
        {
            break;
        }

        shortestPathsTiles.UnionWith(path);
    }

    if (visitedAtCost.TryGetValue((x, y, dirIndex), out var prevCost) && prevCost < cost)
    {
        continue;
    }

    visitedAtCost[(x, y, dirIndex)] = cost;

    pq.Enqueue((x, y, (dirIndex + 1) % 4, path), cost + 1000);
    pq.Enqueue((x, y, (dirIndex + 3) % 4, path), cost + 1000);

    var (dx, dy) = directions[dirIndex];
    if (map[y + dy][x + dx] != '#')
    {
        pq.Enqueue((x + dx, y + dy, dirIndex, path.Push((x + dx, y + dy))), cost + 1);
    }
}

var part1 = lowestCost!.Value;
Console.WriteLine(part1);

var part2 = shortestPathsTiles.Count;
Console.WriteLine(part2);

return;

(int X, int Y) Find(char c)
{
    for(var y = 0; y < map.Length; y++)
    {
        if (map[y].IndexOf(c) is >= 0 and var x)
        {
            return (x, y);
        }
    }

    return (-1, -1);
}