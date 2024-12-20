var map = File.ReadAllLines("input.txt");

var path = GetPath();

var part1 = CountCheats(2, 100);
Console.WriteLine(part1);

var part2 = CountCheats(20, 100);
Console.WriteLine(part2);

return;

int CountCheats(int maxCheatLen, int minSaving)
{
    var count = 0;
    for (var cheatStartTime = 0; cheatStartTime < path.Count; cheatStartTime++)
    {
        var cheatStartPos = path[cheatStartTime];
        for (var cheatEndTime = cheatStartTime + 1; cheatEndTime < path.Count; cheatEndTime++)
        {
            var cheatEndPos = path[cheatEndTime];
            var cheatLen = Math.Abs(cheatStartPos.X - cheatEndPos.X) + Math.Abs(cheatStartPos.Y - cheatEndPos.Y);
            if (cheatLen <= maxCheatLen)
            {
                var timeSaved = cheatEndTime - cheatStartTime - cheatLen;
                if (timeSaved >= minSaving)
                {
                    count++;
                }
            }
        }
    }

    return count;
}

List<(int X, int Y)> GetPath()
{
    var directions = new (int Dx, int Dy)[] { (0, 1), (0, -1), (1, 0), (-1, 0) };
    var (x, y) = FindStart();
    var dir = (Dx: 0, Dy: 0);
    var path = new List<(int, int)> { (x, y) };
    while (map[y][x] != 'E')
    {
        foreach (var (dx, dy) in directions)
        {
            if (dx == -dir.Dx && dy == -dir.Dy)
            {
                continue;
            }

            var newX = x + dx;
            var newY = y + dy;
            if (map[newY][newX] != '#')
            {
                x = newX;
                y = newY;
                dir = (dx, dy);
                path.Add((newX, newY));
                break;
            }
        }
    }

    return path;
}

(int X, int Y) FindStart()
{
    for (var y = 0; y < map.Length; y++)
    {
        for (var x = 0; x < map[y].Length; x++)
        {
            if (map[y][x] == 'S')
            {
                return (x, y);
            }
        }
    }

    return (-1, -1);
}