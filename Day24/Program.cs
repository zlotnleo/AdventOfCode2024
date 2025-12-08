using System.Diagnostics.CodeAnalysis;

var input = File.ReadAllText("input.txt").Split("\n\n");
var initialValues = input[0].Split("\n").Select(initialiser => initialiser.Split(": "))
    .ToDictionary(initialiser => initialiser[0], initialiser => initialiser[1] == "1");

var connections = input[1].Split("\n")
    .Select(connection => connection.Split(" "))
    .ToDictionary(parts => parts[4], parts => (
        parts[0],
        parts[2],
        parts[1] switch
        {
            "AND" => Operator.And,
            "OR" => Operator.Or,
            "XOR" => Operator.Xor
        }
    ));

var wiresUsedForEachOutput = connections.Keys
    .Where(k => k.StartsWith('z'))
    .ToDictionary(output => output, GetWiresUsed);

AssertEachOutputOnlyUsesSameOrLowerBitsFromInputs(wiresUsedForEachOutput);
// Since each output bit doesn't depend on any input bits higher than itself, no swaps can occur
// between two wires used to produce different output bits

void Investigate()
{
    var prevCarryIn = "";
    var swaps = new List<(string, string)>();
    var carries = new List<string>();
    for (var i = 0; i <= 45; i++)
    {
        carries.Add(prevCarryIn);
        var output = $"z{i:00}";
        if (IsCorrect(output, i, prevCarryIn, out var nextCarryIn))
        {
            prevCarryIn = nextCarryIn;
            continue;
        }

        Print(output, prevCarryIn);
        Console.WriteLine($"{output} can't be verified automatically. Is it correct? (y/n)");
        if (Console.ReadKey().Key == ConsoleKey.Y)
        {
            Console.WriteLine("Which is the carry-in?");
            prevCarryIn = Console.ReadLine()!;
            continue;
        }

        Console.WriteLine("Which to swap?");
        var w1 = Console.ReadLine()!;
        var w2 = Console.ReadLine()!;
        (connections[w1], connections[w2]) = (connections[w2], connections[w1]);
        swaps.Add((w1, w2));

        if (IsCorrect(output, i, prevCarryIn, out var nextCarryOut))
        {
            Console.WriteLine("Verified!");
            prevCarryIn = nextCarryOut;
            continue;
        }

        Print(output, prevCarryIn);
        Console.WriteLine("Unable to verify. Which is the carry-in?");
        prevCarryIn = Console.ReadLine()!;
    }
    
    Console.WriteLine("Swaps:");
    foreach (var (s1, s2) in swaps)
    {
        Console.WriteLine($"{s1}, {s2}");
    }

    Console.WriteLine("Carries:");
    foreach (var (i, carry) in carries.Index())
    {
        Console.WriteLine($"{i}: {carry}");
    }
}

Swap("cmv", "z17");
Swap("rmj", "z23");
Swap("rdg", "z30");
Swap("mwp", "btb");
// Investigate();
// Print("z38", "bjh");
// Print("z39", "bjh");

List<string> answer =
[
    "cmv", "z17",
    "rmj", "z23",
    "rdg", "z30",
    "mwp", "btb",
];
answer.Sort();
Console.WriteLine(string.Join(",", answer));

return;

void Swap(string w1, string w2)
{
    (connections[w1], connections[w2]) = (connections[w2], connections[w1]);
}

bool IsCorrect(string wire, int bit, string prevCarryIn, [NotNullWhen(true)] out string? carryIn)
{
    carryIn = null;
    if (!connections.TryGetValue(wire, out var connection))
    {
        return false;
    }

    var (i1, i2, op) = connection;
    if (op != Operator.Xor)
    {
        return false;
    }

    if (IsOperationOfInputBits(i1, bit, Operator.Xor))
    {
        if (IsCarry(i2, bit, prevCarryIn))
        {
            carryIn = i2;
            return true;
        }
    }
    else if (IsOperationOfInputBits(i2, bit, Operator.Xor))
    {
        if (IsCarry(i1, bit, prevCarryIn))
        {
            carryIn = i1;
            return true;
        }
    }

    return false;
}

bool IsCarry(string wire, int bit, string prevCarryIn) =>
    IsMatchPredicate(
        wire,
        w => IsOperationOfInputBits(w, bit - 1, Operator.And),
        w => IsMatchPredicate(
            w,
            w1 => w1 == prevCarryIn,
            w1 => IsOperationOfInputBits(w1, bit - 1, Operator.Xor),
            Operator.And
        ),
        Operator.Or
    );

bool IsMatchPredicate(string wire, Func<string, bool> isMatch1, Func<string, bool> isMatch2, Operator expectedOp) =>
    connections.TryGetValue(wire, out var connection)
    && connection is var (i1, i2, op)
    && op == expectedOp
    && (isMatch1(i1) && isMatch2(i2) || isMatch1(i2) && isMatch2(i1));

bool IsOperationOfInputBits(string wire, int bit, Operator expectedOp) =>
    IsMatch(wire, $"x{bit:00}", $"y{bit:00}", expectedOp);

bool IsMatch(string wire, string expectedIn1, string expectedIn2, Operator expectedOp) =>
    connections.TryGetValue(wire, out var connection)
    && connection is var (i1, i2, op)
    && op == expectedOp
    && (i1 == expectedIn1 && i2 == expectedIn2 || i1 == expectedIn2 || i2 == expectedIn1);

void Print(string wire, string prevCarryIn, int level = 0)
{
    Console.Write(new string(' ', level * 4));
    if (wire == prevCarryIn)
    {
        Console.WriteLine("Carry In");
    }
    else if (!connections.TryGetValue(wire, out var connection))
    {
        Console.WriteLine(wire);
    }
    else
    {
        var (i1, i2, op) = connection;
        Console.WriteLine($"{wire} {op.Name}");
        Print(i1, prevCarryIn, level + 1);
        Print(i2, prevCarryIn, level + 1);
    }
}

HashSet<string> GetWiresUsed(string wire)
{
    var visited = new HashSet<string>();
    var queue = new Queue<string>();
    queue.Enqueue(wire);
    while (queue.Count > 0)
    {
        var current = queue.Dequeue();
        if (!visited.Add(current) || !connections.TryGetValue(current, out var currentConnection))
        {
            continue;
        }

        var (input1, input2, _) = currentConnection;
        queue.Enqueue(input1);
        queue.Enqueue(input2);
    }

    return visited;
}

void AssertEachOutputOnlyUsesSameOrLowerBitsFromInputs(Dictionary<string, HashSet<string>> wires)
{
    foreach (var (output, wiresUsed) in wires)
    {
        var outputBitNumber = int.Parse(output[1..]);
        var maxInputBitNumber = wiresUsed.Where(wire => wire[0] is 'x' or 'y')
            .Select(wire => int.Parse(wire[1..]))
            .Max();
        if (maxInputBitNumber > outputBitNumber)
        {
            throw new Exception("Assertion fail");
        }
    }
}


// var tree = BuildWireTree(connections);

// var reverseConnections = connections.ToDictionary(connection => connection.Value, connection => connection.Key);

// Console.WriteLine(Part1(initialValues, connections));

// var tmp = Enumerable.Range(0, 46).Select(i => DependsOn($"z{i:00}", connections)).ToList();

// Part2(connections, tree);

return;

void Part2(Dictionary<string, (string, string, Operator)> connections, Dictionary<string, IWire> wireTree)
{
    var expectedZ01Tree = new Gate(Operator.Xor, null,
        new Gate(Operator.And, null,
            new Input("x00"),
            new Input("y00")
        ),
        new Gate(Operator.Xor, null,
            new Input("x01"),
            new Input("y01")
        )
    );
    var actualZ01Tree = wireTree["z01"];
    var tmp = DoTreesMatch(expectedZ01Tree, actualZ01Tree);
    _ = 3;
}

bool DoTreesMatch(IWire tree1, IWire tree2) => (tree1, tree2) switch
{
    (Gate(var op1, _, _, _), Gate(var op2, _, _, _)) when op1 != op2 => false,
    (Gate(_, var o1, var i11, var i12), Gate(_, var o2, var i21, var i22)) =>
        DoWireNamesMatch(o1, o2) && (DoTreesMatch(i11, i21) && DoTreesMatch(i12, i22) ||
                                     DoTreesMatch(i11, i22) && DoTreesMatch(i12, i21)),
    _ => DoWireNamesMatch(tree1.WireOutput, tree2.WireOutput)
};

bool DoWireNamesMatch(string? wn1, string? wn2) => wn1 is null || wn2 is null || wn1.Equals(wn2);


long Part1(Dictionary<string, bool> initialValues, Dictionary<string, (string, string, Operator)> connections)
{
    var values = new Dictionary<string, bool>(initialValues);
    var outputs = connections.Keys.Where(k => k.StartsWith('z')).OrderDescending().ToList();

    var result = 0L;
    foreach (var output in outputs)
    {
        result <<= 1;
        if (Eval(output, connections, values))
        {
            result |= 1;
        }
    }

    return result;
}

bool Eval(string wire, Dictionary<string, (string, string, Operator)> connections, Dictionary<string, bool> values)
{
    if (values.TryGetValue(wire, out var value))
    {
        return value;
    }

    var (inputWire1, inputWire2, operation) = connections[wire];
    return values[wire] = operation.Apply(
        Eval(inputWire1, connections, values),
        Eval(inputWire2, connections, values)
    );
}

Dictionary<string, IWire> BuildWireTree(Dictionary<string, (string, string, Operator)> connections)
{
    var tree = new Dictionary<string, IWire>();
    foreach (var outputWire in connections.Keys.Where(k => k.StartsWith('z')))
    {
        Visit(outputWire);
    }

    return tree;

    IWire Visit(string wire)
    {
        if (tree.TryGetValue(wire, out var cached))
        {
            return cached;
        }

        if (!connections.TryGetValue(wire, out var connection))
        {
            return new Input(wire);
        }

        var (i1, i2, op) = connection;
        return tree[wire] = new Gate(op, wire, Visit(i1), Visit(i2));
    }
}

internal record Operator(string Name, Func<bool, bool, bool> Apply)
{
    public static readonly Operator And = new("And", (a, b) => a && b);
    public static readonly Operator Or = new("Or", (a, b) => a || b);
    public static readonly Operator Xor = new("Xor", (a, b) => a ^ b);
}

interface IWire
{
    string? WireOutput { get; }
}

record struct Gate(Operator Operator, string? WireOutput, IWire Input1, IWire Input2) : IWire;

record struct Input(string? WireOutput) : IWire;