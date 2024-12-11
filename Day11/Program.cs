var initialStones = File.ReadAllText("input.txt").Split(' ').Select(long.Parse).ToArray();

var splitsIntoStoneCountAfterBlinks = new Dictionary<(long, int), long>();

var part1 = initialStones.Sum(stone => GetStoneCountAfterBlinks(stone, 25));
Console.WriteLine(part1);

var part2 = initialStones.Sum(stone => GetStoneCountAfterBlinks(stone, 75));
Console.WriteLine(part2);

return;

long GetStoneCountAfterBlinks(long stone, int blinks)
{
    if (blinks == 0)
    {
        return 1;
    }

    if (splitsIntoStoneCountAfterBlinks.TryGetValue((stone, blinks), out var count))
    {
        return count;
    }

    count = stone switch
    {
        0 => GetStoneCountAfterBlinks(1, blinks - 1),
        _ when TryEvenSplit(stone, out var nexStone1, out var nextStone2) =>
            GetStoneCountAfterBlinks(nexStone1, blinks - 1)
            + GetStoneCountAfterBlinks(nextStone2, blinks - 1),
        _ => GetStoneCountAfterBlinks(stone * 2024, blinks - 1)
    };
    splitsIntoStoneCountAfterBlinks[(stone, blinks)] = count;
    return count;
}

bool TryEvenSplit(long n, out long firstHalf, out long secondHalf)
{
    var digits = new long[20];
    var count = 0;
    while (n > 0)
    {
        digits[count++] = n % 10;
        n /= 10;
    }

    firstHalf = 0;
    secondHalf = 0;
    if (count % 2 != 0)
    {
        return false;
    }

    for (var i = 0; i < count / 2; i++)
    {
        firstHalf = firstHalf * 10 + digits[count - i - 1];
        secondHalf = secondHalf * 10 + digits[count / 2 - i - 1];
    }

    return true;
}