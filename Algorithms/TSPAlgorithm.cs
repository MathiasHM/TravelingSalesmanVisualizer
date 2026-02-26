namespace TravelingSalesman.Algorithms;

public abstract class TSPAlgorithm
{
    protected List<PointF> Nodes { get; private set; } = [];
    protected Random Random { get; } = new();
    protected CancellationToken CancellationToken { get; private set; }

    public string Name { get; }
    public Color Color { get; }
    public Action<List<int>>? OnProgressUpdate { get; set; }

    protected TSPAlgorithm(string name, Color color)
    {
        Name = name;
        Color = color;
    }

    public void SetNodes(List<PointF> nodes)
    {
        Nodes = new List<PointF>(nodes);
    }

    public void SetCancellationToken(CancellationToken token)
    {
        CancellationToken = token;
    }

    public abstract List<int> Solve();

    /* Runs Solve() on a dedicated OS thread so each algorithm is truly isolated.
    TaskCreationOptions.LongRunning bypasses the shared thread pool and provisions
    a new thread, ensuring no two algorithms share CPU time on the same thread
    and that elapsed-time measurements are fair across concurrent runs.*/
    public Task<List<int>> SolveAsync(CancellationToken cancellationToken)
    {
        SetCancellationToken(cancellationToken);
        return Task.Factory.StartNew(
            Solve,
            cancellationToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
    }

    protected void ReportProgress(List<int> currentPath)
    {
        OnProgressUpdate?.Invoke(new List<int>(currentPath));
    }

    protected double CalculatePathDistance(List<int> path)
    {
        if (path.Count == 0) return 0;

        double total = 0;
        for (int i = 0; i < path.Count - 1; i++)
        {
            total += Distance(Nodes[path[i]], Nodes[path[i + 1]]);
        }
        total += Distance(Nodes[path[^1]], Nodes[path[0]]);
        return total;
    }

    protected static double Distance(PointF p1, PointF p2)
    {
        //LEMMA
        double dx = p1.X - p2.X;
        double dy = p1.Y - p2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}
