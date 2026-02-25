namespace TravelingSalesman.Algorithms;

public class TabuSearchAlgorithm : TSPAlgorithm
{
    public int MaxIterations { get; set; } = 500;
    public int TabuTenure { get; set; } = 20;

    public TabuSearchAlgorithm()
        : base("Tabu Search", Color.FromArgb(233, 30, 99))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];
        if (Nodes.Count == 1) return [0];
        if (Nodes.Count == 2) return [0, 1];

        //TODO: Implement tabu search
        //looks fun

        var nn = new NearestNeighborAlgorithm();
        nn.SetNodes(Nodes);
        nn.SetCancellationToken(CancellationToken);
        var path = nn.Solve();

        ReportProgress(path);
        return path;
    }
}
