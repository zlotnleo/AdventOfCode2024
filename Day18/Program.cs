var directions = new[] { (0, 1), (0, -1), (-1, 0), (1, 0) };

var allCorruptedCoords = File.ReadAllLines("input.txt")
    .Select(line => line.Split(','))
    .Select(split => (X: int.Parse(split[0]), Y: int.Parse(split[1])))
    .ToList();

const int size = 71;
var part1 = GetPath(1024);
Console.WriteLine(part1);

var l = 1025;
var r = allCorruptedCoords.Count;
while (l < r)
{
    var mid = (l + r) / 2;
    if (GetPath(mid) == -1)
    {
        r = mid;
    }
    else
    {
        l = mid + 1;
    }
}

var blockingCoord = allCorruptedCoords[l - 1];
var part2 = $"{blockingCoord.X},{blockingCoord.Y}";
Console.WriteLine(part2);

return;

int GetPath(int coordsToTake)
{
    var corruptedCoords = allCorruptedCoords.Take(coordsToTake).ToHashSet();

    var target = (size - 1, size - 1);
    var visited = new HashSet<(int, int)>();
    var queue = new Queue<(int, int, int)>();
    queue.Enqueue((0, 0, 0));
    while (queue.TryDequeue(out var state))
    {
        var (x, y, len) = state;
        if ((x, y) == target)
        {
            return len;
        }

        if (!visited.Add((x, y)))
        {
            continue;
        }

        foreach (var (dx, dy) in directions)
        {
            var newX = x + dx;
            var newY = y + dy;
            if (newX is >= 0 and < size && newY is >= 0 and < size && !corruptedCoords.Contains((newX, newY)))
            {
                queue.Enqueue((newX, newY, len + 1));
            }
        }
    }

    return -1;
}