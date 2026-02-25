namespace TravelingSalesman.Algorithms;

public class ThreeOptAlgorithm : TSPAlgorithm
{
    public int MaxIterations { get; set; } = 50;

    public ThreeOptAlgorithm()
        : base("3-Opt", Color.FromArgb(255, 87, 34))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];
        if (Nodes.Count < 4) return Enumerable.Range(0, Nodes.Count).ToList();

        //TODO: Implement 3-opt
        //Like 2-opt but 3

        var twoOpt = new TwoOptAlgorithm();
        twoOpt.SetNodes(Nodes);
        twoOpt.SetCancellationToken(CancellationToken);
        var path = twoOpt.Solve();

        ReportProgress(path);
        return path;
    }
}
