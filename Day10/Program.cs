var map = File.ReadAllLines("input.txt");

var directions = new[] { (0, 1), (0, -1), (1, 0), (-1, 0) };
var reachableFromAndPathCount = new Dictionary<(int, int), (HashSet<(int, int)>, int)>();
var queue = new Queue<(int, int)>();
foreach (var (y, row) in map.Index())
{
    foreach (var (x, elevation) in row.Index())
    {
        if (elevation == '0')
        {
            queue.Enqueue((x, y));
            reachableFromAndPathCount[(x, y)] = ([(x, y)], 1);
        }
    }
}

var totalScore = 0;
var totalPaths = 0;
while (queue.TryDequeue(out var state))
{
    var (x, y) = state;
    var currentElevation = map[y][x];
    var (currentReachableFrom, currentPathCount) = reachableFromAndPathCount[(x, y)];

    if (currentElevation == '9')
    {
        totalPaths += currentPathCount;
        totalScore += currentReachableFrom.Count;
        continue;
    }

    foreach (var (dx, dy) in directions)
    {
        var newX = x + dx;
        var newY = y + dy;
        if (newY < 0 || newY >= map.Length || newX < 0 || newX >= map[0].Length || map[newY][newX] != currentElevation + 1)
        {
            continue;
        }

        if (reachableFromAndPathCount.TryGetValue((newX, newY), out var nextReachableFromAndPathCount))
        {
            var (nextReachableFrom, nextPathCount) = nextReachableFromAndPathCount;
            nextReachableFrom.UnionWith(currentReachableFrom);
            reachableFromAndPathCount[(newX, newY)] = (nextReachableFrom, nextPathCount + currentPathCount);
            continue;
        }

        reachableFromAndPathCount[(newX, newY)] = ([..currentReachableFrom], currentPathCount);
        queue.Enqueue((newX, newY));
    }
}

Console.WriteLine(totalScore);
Console.WriteLine(totalPaths);
