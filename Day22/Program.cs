var initialSecretNumbers = File.ReadAllLines("input.txt").Select(long.Parse).ToList();

// Price difference is between -9 and 9, 19 possible values
// Add 9 to each so that the range is 0 to 18
// Then we can treat the sequence of 4 price differences as a 4-digit base-19 number
var part1 = 0L;
var totalBananasByEncodedSequence = new int[19 * 19 * 19 * 19];
foreach (var initialSecretNumber in initialSecretNumbers)
{
    var seen = new HashSet<int>(2000);
    var sequence = 0;
    var secretNumber = initialSecretNumber;
    var prevPrice = (int) secretNumber % 10;
    for (var i = 0; i < 2000; i++)
    {
        secretNumber = NextSecretNumber(secretNumber);
        var price = (int) secretNumber % 10;

        sequence = sequence % 6859 * 19 + price - prevPrice + 9;
        prevPrice = price;

        if (i >= 3 && seen.Add(sequence))
        {
            totalBananasByEncodedSequence[sequence] += price;
        }
    }

    part1 += secretNumber;
}

Console.WriteLine(part1);

var part2 = totalBananasByEncodedSequence.Max();
Console.WriteLine(part2);

return;

long NextSecretNumber(long secretNumber)
{
    secretNumber ^= secretNumber << 6;
    secretNumber %= 16777216;
    secretNumber ^= secretNumber >> 5;
    secretNumber %= 16777216;
    secretNumber ^= secretNumber << 11;
    secretNumber %= 16777216;
    return secretNumber;
}
