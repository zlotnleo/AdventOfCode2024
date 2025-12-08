var schematicLookupByIsKey = File.ReadAllText("input.txt")
    .Split("\n\n")
    .Select(schematic => schematic
        .Where(c => c != '\n')
        .Aggregate(0ul, (bits, c) => bits << 1 | (c == '#' ? 1ul : 0ul))
    )
    .ToLookup(s => (s & 1) == 0);

var part1 = schematicLookupByIsKey[true].Sum(key =>
    schematicLookupByIsKey[false].Count(@lock => (@lock & key) == 0)
);
Console.WriteLine(part1);
