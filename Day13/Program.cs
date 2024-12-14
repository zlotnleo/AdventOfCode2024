using System.Text.RegularExpressions;

var clawMachineRegex = ClawMachineRegex();
var clawMachines = File.ReadAllText("input.txt")
    .Split("\n\n")
    .Select(arcadeString => clawMachineRegex.Match(arcadeString).Groups switch
    {
        [_, var ax, var ay, var bx, var by, var px, var py] => (
            int.Parse(ax.Value),
            int.Parse(ay.Value),
            int.Parse(bx.Value),
            int.Parse(by.Value),
            PrizeX: long.Parse(px.Value),
            PrizeY: long.Parse(py.Value)
        )
    })
    .ToList();

var part1 = clawMachines.Sum(GetTokens);
Console.WriteLine(part1);

var part2 = clawMachines.Select(machine => machine with
{
    PrizeX = machine.PrizeX + 10000000000000,
    PrizeY = machine.PrizeY + 10000000000000
}).Sum(GetTokens);
Console.WriteLine(part2);

return;

long GetTokens((int, int, int, int, long, long) clawMachine)
{
    var (ax, ay, bx, by, px, py) = clawMachine;
    /*
     * ax * a + bx * b = px  | * ay
     * ay * a + by * b = py  | * ax
     *
     *   ax * ay * a + ay * bx * b = ay * px
     * - ax * ay * a + ax * by * b = ax * py
     * -------------------------------------
     *   b * (ay * bx - ax * by) = ay * px - ax * py
     *
     * b = (ay * px - ax * py) / (ay * bx - ax * by)
     * a = (px - bx * b) / ax
     */
    var bNumerator = ay * px - ax * py;
    var bDenominator = ay * bx - ax * by;
    if (bNumerator % bDenominator != 0)
    {
        return 0;
    }

    var b = bNumerator / bDenominator;
    var aNumerator = px - bx * b;
    if (aNumerator % ax != 0)
    {
        return 0;
    }

    var a = aNumerator / ax;
    return a * 3 + b;
}

internal partial class Program
{
    [GeneratedRegex("""
                    ^Button A: X\+(\d+), Y\+(\d+)
                    Button B: X\+(\d+), Y\+(\d+)
                    Prize: X=(\d+), Y=(\d+)$
                    """)]
    private static partial Regex ClawMachineRegex();
}