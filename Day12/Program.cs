var plotMap = File.ReadAllLines("input.txt");

var regions = GetRegions();

var part1 = regions.Sum(region => region.Count * GetPerimeter(region));
Console.WriteLine(part1);

var part2 = regions.Sum(region => region.Count * CountSides(region));
Console.WriteLine(part2);

return;

List<HashSet<Coord>> GetRegions()
{
    var visited = new HashSet<Coord>();
    return plotMap.SelectMany((row, y) =>
        row.SelectMany((plant, x) =>
            TraverseRegion((x, y), plant, visited)
        )
    ).ToList();
}

IEnumerable<HashSet<Coord>> TraverseRegion(Coord startCoord, char plant, HashSet<Coord> visited)
{
    if (!visited.Add(startCoord))
    {
        return [];
    }

    var region = new HashSet<Coord> { startCoord };
    var queue = new Queue<Coord>();
    queue.Enqueue(startCoord);
    while (queue.TryDequeue(out var coord))
    {
        foreach (var newCoord in coord.Neighbours)
        {
            if (newCoord.Y < 0 || newCoord.Y >= plotMap.Length || newCoord.X < 0 || newCoord.X >= plotMap[0].Length
                || plotMap[newCoord.Y][newCoord.X] != plant)
            {
                continue;
            }

            if (visited.Add(newCoord))
            {
                region.Add(newCoord);
                queue.Enqueue(newCoord);
            }
        }
    }

    return [region];
}

static int GetPerimeter(HashSet<Coord> region) => region.Sum(coord => 4 - coord.Neighbours.Count(region.Contains));

int CountSides(HashSet<Coord> region) =>
    region.Sum(coord =>
        coord.CornerNeighbours.Count(cornerNeighbour => IsCorner(
            cornerNeighbour.Neighbour1,
            cornerNeighbour.Neighbour2,
            cornerNeighbour.DiagonalNeighbour,
            region
        ))
    );

bool IsCorner(Coord neighbour1, Coord neighbour2, Coord diagonalNeighbour, HashSet<Coord> region)
{
    var isNeighbour1InRegion = region.Contains(neighbour1);
    var isNeighbour2InRegion = region.Contains(neighbour2);
    return !isNeighbour1InRegion && !isNeighbour2InRegion
           || isNeighbour1InRegion && isNeighbour2InRegion && !region.Contains(diagonalNeighbour);
}

internal record Coord(int X, int Y)
{
    private static readonly Coord[] NeighbourDirs = [(0, -1), (1, 0), (0, 1), (-1, 0)];

    public static implicit operator Coord((int X, int Y) coord) => new(coord.X, coord.Y);
    public static Coord operator +(Coord a, Coord b) => new(a.X + b.X, a.Y + b.Y);

    public IEnumerable<Coord> Neighbours => NeighbourDirs.Select(dir => this + dir);

    public IEnumerable<(Coord Neighbour1, Coord Neighbour2, Coord DiagonalNeighbour)> CornerNeighbours =>
        Enumerable.Range(0, 4).Select(i => (
            this + NeighbourDirs[i],
            this + NeighbourDirs[(i + 1) % 4],
            this + NeighbourDirs[i] + NeighbourDirs[(i + 1) % 4]
        ));
}