namespace TravelingSalesman.Algorithms;

public class LinKernighanAlgorithm : TSPAlgorithm
{
    public int MaxIterations { get; set; } = 50;

    public LinKernighanAlgorithm()
        : base("Lin-Kernighan", Color.FromArgb(255, 193, 7))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];
        if (Nodes.Count == 1) return [0];
        if (Nodes.Count == 2) return [0, 1];

        //TODO: Implement Lin-Kernighan heuristic
        //wut is k?
        //research
        var nn = new NearestNeighborAlgorithm();
        nn.SetNodes(Nodes);
        nn.SetCancellationToken(CancellationToken);
        var tour = nn.Solve();

        ReportProgress(tour);
        return tour;
    }
}
