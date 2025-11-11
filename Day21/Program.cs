var numericKeypadSequences = GetNumericKeypadSequences();
var directionalKeypadSequences = GetDirectionalKeypadSequences();
var cache = new Dictionary<(string, int), long>();
var codes = File.ReadAllLines("input.txt");

var part1 = Solve(3);
Console.WriteLine(part1);

var part2 = Solve(26);
Console.WriteLine(part2);

return;

long Solve(int directionalKeypadCount)
{
    return codes.Select(code => int.Parse(code[..^1]) * GetSequenceLength(code, directionalKeypadCount)).Sum();
}

long GetSequenceLength(string code, int directionalKeypadCount)
{
    var length = 0L;
    var prevButton = 'A';
    foreach (var nextButton in code)
    {
        length += GetDirectionalKeypadSequenceLength(
            numericKeypadSequences[(prevButton, nextButton)],
            directionalKeypadCount
        );
        prevButton = nextButton;
    }

    return length;
}

long GetDirectionalKeypadSequenceLength(string sequence, int levels)
{
    if (levels == 1)
    {
        return sequence.Length;
    }

    if (cache.TryGetValue((sequence, levels), out var sequenceLength))
    {
        return sequenceLength;
    }

    var length = 0L;
    var prevButton = 'A';
    foreach (var nextButton in sequence)
    {
        length += GetDirectionalKeypadSequenceLength(
            directionalKeypadSequences[(prevButton, nextButton)],
            levels - 1
        );
        prevButton = nextButton;
    }

    cache[(sequence, levels)] = length;
    return length;
}

Dictionary<(char, char), string> GetNumericKeypadSequences()
{
    var numericKeypad = new List<(char C, int X, int Y)>
    {
        ('7', 0, 3), ('8', 1, 3), ('9', 2, 3),
        ('4', 0, 2), ('5', 1, 2), ('6', 2, 2),
        ('1', 0, 1), ('2', 1, 1), ('3', 2, 1),
        /*        */ ('0', 1, 0), ('A', 2, 0)
    };

    var sequences = new Dictionary<(char, char), string>();
    foreach (var (from, fromX, fromY) in numericKeypad)
    {
        foreach (var (to, toX, toY) in numericKeypad)
        {
            var dx = toX - fromX;
            var dy = toY - fromY;
            var horizontal = dx >= 0 ? new string('>', dx) : new string('<', -dx);
            var vertical = dy >= 0 ? new string('^', dy) : new string('v', -dy);
            sequences[(from, to)] = dx switch
            {
                _ when fromX == 0 && toY == 0 => $"{horizontal}{vertical}A",
                _ when fromY == 0 && toX == 0 => $"{vertical}{horizontal}A",
                < 0 => $"{horizontal}{vertical}A",
                >= 0 => $"{vertical}{horizontal}A",
            };
        }
    }

    return sequences;
}

Dictionary<(char, char), string> GetDirectionalKeypadSequences()
{
    var directionalKeypad = new List<(char C, int X, int Y)>
    {
        /*         */ ('^', 1, 1), ('A', 2, 1),
        ('<', 0, 0), ('v', 1, 0), ('>', 2, 0),
    };

    var sequences = new Dictionary<(char, char), string>();
    foreach (var (from, fromX, fromY) in directionalKeypad)
    {
        foreach (var (to, toX, toY) in directionalKeypad)
        {
            var dx = toX - fromX;
            var dy = toY - fromY;
            var horizontal = dx >= 0 ? new string('>', dx) : new string('<', -dx);
            var vertical = dy >= 0 ? new string('^', dy) : new string('v', -dy);
            sequences[(from, to)] = dx switch
            {
                _ when fromX == 0 => $"{horizontal}{vertical}A",
                _ when toX == 0 => $"{vertical}{horizontal}A",
                < 0 => $"{horizontal}{vertical}A",
                >= 0 => $"{vertical}{horizontal}A",
            };
        }
    }

    return sequences;
}
