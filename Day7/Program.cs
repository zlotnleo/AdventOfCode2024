using System.Collections.Immutable;

var equations = File.ReadAllLines("input.txt")
    .Select(line => line.Split(' '))
    .Select(split => (
        Target: long.Parse(split[0][..^1]),
        Numbers: ImmutableStack.CreateRange(split.Skip(1).Select(long.Parse))
    ));

var equationsByPossibility = equations.ToLookup(
    eq => IsPossible(eq.Target, eq.Numbers, false)
);
var sumOfPossibleTargets = equationsByPossibility[true].Sum(eq => eq.Target);
var part1 = sumOfPossibleTargets;
Console.WriteLine(part1);

var sumOfTargetsMadePossibleByConcatenation = equationsByPossibility[false]
    .Where(eq => IsPossible(eq.Target, eq.Numbers, true))
    .Sum(eq => eq.Target);

var part2 = sumOfPossibleTargets + sumOfTargetsMadePossibleByConcatenation;
Console.WriteLine(part2);

return;

bool IsPossible(long target, ImmutableStack<long> numbers, bool allowConcat)
{
    numbers = numbers.Pop(out var lastNumber);
    if (numbers.IsEmpty)
    {
        return target == lastNumber;
    }

    return target >= lastNumber && IsPossible(target - lastNumber, numbers, allowConcat)
           || target % lastNumber == 0 && IsPossible(target / lastNumber, numbers, allowConcat)
           || allowConcat && TryDecatenateNumber(target, lastNumber, out var nextTarget) && IsPossible(nextTarget, numbers, allowConcat);
}

bool TryDecatenateNumber(long target, long number, out long nextTarget)
{
    while (number > 0)
    {
        if (number % 10 != target % 10)
        {
            nextTarget = -1;
            return false;
        }

        number /= 10;
        target /= 10;
    }

    nextTarget = target;
    return true;
}
