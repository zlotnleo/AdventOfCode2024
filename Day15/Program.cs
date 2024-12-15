var input = File.ReadAllText("input.txt").Split("\n\n");
var inputMap = input[0].Split('\n');
var moves = input[1].Split("\n").SelectMany(s => s).Select(c =>
    c switch { '^' => (0, -1), 'v' => (0, 1), '<' => (-1, 0), '>' => (1, 0) }
).ToArray();

var firstWarehouseMap = CloneMap(inputMap);
PushBoxes(firstWarehouseMap);
var part1 = GetBoxGpsSum(firstWarehouseMap);
Console.WriteLine(part1);

var secondWarehouseMap = ExpandMap(inputMap);
PushBoxes(secondWarehouseMap);
var part2 = GetBoxGpsSum(secondWarehouseMap);
Console.WriteLine(part2);

return;

void PushBoxes(char[][] map)
{
    var (robotX, robotY) = FindRobot(map);
    foreach (var (dx, dy) in moves)
    {
        var slicesToMove = new Stack<List<(int X, int Y)>>();
        slicesToMove.Push([(robotX, robotY)]);

        bool hasHitWall;
        while (true)
        {
            var nextSlice = slicesToMove.Peek()
                .SelectMany(coord => GetPushedCoords(map, coord.X, coord.Y, dx, dy))
                .Distinct()
                .ToList<(int X, int Y)>();

            if (nextSlice.Count == 0)
            {
                hasHitWall = false;
                break;
            }

            if (nextSlice.Any(coord => map[coord.Y][coord.X] == '#'))
            {
                hasHitWall = true;
                break;
            }

            slicesToMove.Push(nextSlice);
        }

        if (hasHitWall)
        {
            continue;
        }

        while (slicesToMove.TryPop(out var slice))
        {
            foreach (var (x, y) in slice)
            {
                map[y + dy][x + dx] = map[y][x];
                map[y][x] = '.';
            }
        }

        robotX += dx;
        robotY += dy;
    }
}

static IEnumerable<(int, int)> GetPushedCoords(char[][] map, int x, int y, int dx, int dy) =>
    map[y + dy][x + dx] switch
    {
        '.' => [],
        '[' when dx == 0 => [(x, y + dy), (x + 1, y + dy)],
        ']' when dx == 0 => [(x, y + dy), (x - 1, y + dy)],
        _ => [(x + dx, y + dy)]
    };

static char[][] CloneMap(string[] map) =>
    map.Select(row => row.ToCharArray()).ToArray();

static char[][] ExpandMap(string[] map) =>
    map.Select(row => row.SelectMany(c =>
        c switch { '@' => "@.", '.' => "..", '#' => "##", 'O' => "[]" }
    ).ToArray()).ToArray();

static (int, int) FindRobot(char[][] map)
{
    for (var y = 0; y < map.Length; y++)
    {
        for (var x = 0; x < map[0].Length; x++)
        {
            if (map[y][x] == '@')
            {
                return (x, y);
            }
        }
    }

    return (-1, -1);
}

static int GetBoxGpsSum(char[][] map) =>
    map.SelectMany((row, y) => row.Select((c, x) => c is 'O' or '[' ? y * 100 + x : 0)).Sum();