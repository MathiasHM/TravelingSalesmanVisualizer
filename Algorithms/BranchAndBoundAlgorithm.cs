namespace TravelingSalesman.Algorithms;

public class BranchAndBoundAlgorithm : TSPAlgorithm
{
    private double bestCost;
    private List<int> bestPath = [];
    private long nodesExplored;
    private long nodesPruned;

    public BranchAndBoundAlgorithm()
        : base("Branch and Bound", Color.FromArgb(63, 81, 181))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];
        if (Nodes.Count == 1) return [0];
        if (Nodes.Count == 2) return [0, 1];

        //TODO: Implement branch and bound algorithm
        //This is going to be tricky, use MST for lower bound?

        var nn = new NearestNeighborAlgorithm();
        nn.SetNodes(Nodes);
        nn.SetCancellationToken(CancellationToken);
        bestPath = nn.Solve();

        ReportProgress(bestPath);
        return bestPath;
    }
}
