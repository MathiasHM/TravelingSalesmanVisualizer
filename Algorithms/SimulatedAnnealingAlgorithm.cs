namespace TravelingSalesman.Algorithms;

public class SimulatedAnnealingAlgorithm : TSPAlgorithm
{
    public double InitialTemperature { get; set; } = 10000;
    public double CoolingRate { get; set; } = 0.995;

    public SimulatedAnnealingAlgorithm()
        : base("Simulated Annealing", Color.FromArgb(156, 39, 176))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];

        //TODO: Implement simulated annealing properly
        //- Start with initial solution (maybe greedy?)

        var nearestNeighbor = new NearestNeighborAlgorithm();
        nearestNeighbor.SetNodes(Nodes);
        var path = nearestNeighbor.Solve();

        ReportProgress(path);
        return path;
    }
}
