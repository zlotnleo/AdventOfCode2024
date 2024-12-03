using System.Text.RegularExpressions;

var memory = File.ReadAllText("input.txt");

var part1 = 0;
var part2 = 0;

var isEnabled = true;
for (var match = InstructionRegex().Match(memory); match.Success; match = match.NextMatch())
{
    switch (match.ValueSpan)
    {
        case "don't()":
            isEnabled = false;
            break;
        case "do()":
            isEnabled = true;
            break;
        default:
            var product = int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);
            part1 += product;
            if (isEnabled)
            {
                part2 += product;
            }

            break;
    }
}

Console.WriteLine(part1);
Console.WriteLine(part2);

internal partial class Program
{
    [GeneratedRegex(@"mul\((\d{1,3}),(\d{1,3})\)|do(n't)?\(\)")]
    private static partial Regex InstructionRegex();
}