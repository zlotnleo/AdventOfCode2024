var graph = new Dictionary<string, HashSet<string>>();
var connections = File.ReadAllLines("input.txt")
    .Select(line => line.Split('-'))
    .ToList();
foreach (var connection in connections)
{
    AddConnection(connection[0], connection[1]);
    AddConnection(connection[1], connection[0]);
}

Console.WriteLine(Part1());
Console.WriteLine(Part2());


return;

int Part1() =>
    connections
        .Where(c => c[0].StartsWith('t') || c[1].StartsWith('t'))
        .SelectMany(c =>
            graph[c[0]].Intersect(graph[c[1]]).Select(c2 => StringifyClique([c[0], c[1], c2]))
        ).Distinct().Count();

string Part2()
{
    List<string> maxClique = [];
    var verticesToConsider = graph.Keys.ToHashSet();
    while (verticesToConsider.Count > 0)
    {
        var vertex = verticesToConsider.First();
        var cliqueFromVertex = MaximalCliqueFromVertex(vertex);

        if (maxClique.Count < cliqueFromVertex.Count)
        {
            maxClique = cliqueFromVertex;
        }

        verticesToConsider.ExceptWith(cliqueFromVertex);
    }

    return StringifyClique(maxClique);
}

List<string> MaximalCliqueFromVertex(string vertex)
{
    List<string> clique = [vertex];
    var candidates = graph[vertex];
    while (candidates.Count > 0)
    {
        var nextElement = candidates.First();
        clique.Add(nextElement);
        candidates.IntersectWith(graph[nextElement]);
    }

    return clique;
}

string StringifyClique(IEnumerable<string> clique) => string.Join(',', clique.Order());

void AddConnection(string from, string to)
{
    if (graph.TryGetValue(from, out var nodes))
    {
        nodes.Add(to);
    }
    else
    {
        graph[from] = [to];
    }
}