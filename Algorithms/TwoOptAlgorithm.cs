namespace TravelingSalesman.Algorithms;

public class TwoOptAlgorithm : TSPAlgorithm
{
    public int MaxIterations { get; set; } = 100;

    public TwoOptAlgorithm()
        : base("2-Opt", Color.FromArgb(255, 152, 0))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];

        var nearestNeighbor = new NearestNeighborAlgorithm();
        nearestNeighbor.SetNodes(Nodes);
        nearestNeighbor.SetCancellationToken(CancellationToken);
        var path = nearestNeighbor.Solve();

        ReportProgress(path);

        bool improved = true;
        int iterations = 0;
        int reportInterval = Math.Max(1, MaxIterations / 20);

        while (improved && iterations < MaxIterations)
        {
            CancellationToken.ThrowIfCancellationRequested();

            improved = false;
            for (int i = 1; i < path.Count - 1; i++)
            {
                for (int j = i + 1; j < path.Count; j++)
                {
                    double currentDist =
                        Distance(Nodes[path[i - 1]], Nodes[path[i]]) +
                        Distance(Nodes[path[j]], Nodes[path[(j + 1) % path.Count]]);

                    double newDist =
                        Distance(Nodes[path[i - 1]], Nodes[path[j]]) +
                        Distance(Nodes[path[i]], Nodes[path[(j + 1) % path.Count]]);

                    if (newDist < currentDist)
                    {
                        path.Reverse(i, j - i + 1);
                        improved = true;
                    }
                }
            }
            iterations++;

            if (iterations % reportInterval == 0 || iterations == MaxIterations)
            {
                ReportProgress(path);
            }
        }

        return path;
    }
}
