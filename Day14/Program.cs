using System.Text.RegularExpressions;

const int width = 101;
const int height = 103;

var robotRegex = RobotRegex();
var robots = File.ReadAllLines("input.txt")
    .Select(robotDescription => robotRegex.Match(robotDescription).Groups switch
    {
        [_, var px, var py, var vx, var vy] =>
            (int.Parse(px.Value), int.Parse(py.Value), int.Parse(vx.Value), int.Parse(vy.Value))
    }).ToArray();

var part1 = robots.Select(robot => GetPositionAfterSeconds(robot, 100))
    .Where(robot => robot.X != width / 2 && robot.Y != height / 2)
    .GroupBy(robot => (robot.X < width / 2, robot.Y < height / 2))
    .Select(group => group.Count())
    .Aggregate(1, (a, b) => a * b);
Console.WriteLine(part1);

var (part2, easterEggRobots) = Nat().Select(n =>
    (Seconds: n, Robots: robots.Select(robot => GetPositionAfterSeconds(robot, n)).ToArray())
).First(t => t.Robots.Distinct().Count() == robots.Length);
Console.WriteLine(part2);

for (var y = 0; y < height; y++)
{
    for (var x = 0; x < width; x++)
    {
        Console.Write(easterEggRobots.Contains((x, y)) ? '#' : ' ');
    }
    Console.WriteLine();
}

return;

IEnumerable<int> Nat()
{
    for (var n = 0;; n++)
    {
        yield return n;
    }
    // ReSharper disable once IteratorNeverReturns
}

(int X, int Y) GetPositionAfterSeconds((int, int, int, int) robot, int seconds)
{
    var (px, py, vx, vy) = robot;

    var x = (px + vx * seconds) % width;
    if (x < 0)
    {
        x += width;
    }

    var y = (py + vy * seconds) % height;
    if (y < 0)
    {
        y += height;
    }

    return (x, y);
}

internal partial class Program
{
    [GeneratedRegex(@"p=(\d+),(\d+) v=(-?\d+),(-?\d+)")]
    private static partial Regex RobotRegex();
}