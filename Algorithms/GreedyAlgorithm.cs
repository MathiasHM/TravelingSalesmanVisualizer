namespace TravelingSalesman.Algorithms;

public class GreedyAlgorithm : TSPAlgorithm
{
    public GreedyAlgorithm()
        : base("Greedy", Color.FromArgb(33, 150, 243))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];

        //TODO: Implement greedy algorithm

        var path = Enumerable.Range(0, Nodes.Count).ToList();
        ReportProgress(path);
        return path;
    }
}

