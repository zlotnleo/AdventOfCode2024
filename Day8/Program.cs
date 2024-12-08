var map = File.ReadAllLines("input.txt");
var antennasByFrequency = map
    .SelectMany((row, y) => row.Select((freq, x) => (Freq: freq, X: x, Y: y)))
    .Where(t => t.Freq != '.')
    .GroupBy(t => t.Freq, t => (t.X, t.Y))
    .Select(g => g.ToArray())
    .ToArray();

var simpleAntinodes = new HashSet<(int, int)>();
var harmonicAntinodes = new HashSet<(int, int)>();
foreach (var antennas in antennasByFrequency)
{
    foreach (var antenna1 in antennas)
    {
        foreach (var antenna2 in antennas)
        {
            if (antenna1 != antenna2)
            {
                if (TryGetSimpleAntinode(antenna1, antenna2, out var simpleAntinode))
                {
                    simpleAntinodes.Add(simpleAntinode);
                }

                harmonicAntinodes.UnionWith(GetHarmonicAntinodes(antenna1, antenna2));
            }
        }
    }
}

var part1 = simpleAntinodes.Count;
Console.WriteLine(part1);

var part2 = harmonicAntinodes.Count;
Console.WriteLine(part2);

return;

bool TryGetSimpleAntinode((int X, int Y) antenna1, (int X, int Y) antenna2, out (int X, int Y) antinode)
{
    var dx = antenna2.X - antenna1.X;
    var dy = antenna2.Y - antenna1.Y;
    antinode = (antenna2.X + dx, antenna2.Y + dy);
    return antinode.Y >= 0 && antinode.Y < map.Length && antinode.X >= 0 && antinode.X < map[0].Length;
}

IEnumerable<(int, int)> GetHarmonicAntinodes((int X, int Y) antenna1, (int X, int Y) antenna2)
{
    var dx = antenna2.X - antenna1.X;
    var dy = antenna2.Y - antenna1.Y;
    var (x, y) = antenna2;
    while (y >= 0 && y < map.Length && x >= 0 && x < map[0].Length)
    {
        yield return (x, y);
        x += dx;
        y += dy;
    }
}