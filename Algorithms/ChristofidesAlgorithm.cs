namespace TravelingSalesman.Algorithms;

public class ChristofidesAlgorithm : TSPAlgorithm
{
    public ChristofidesAlgorithm()
        : base("Christofides", Color.FromArgb(63, 81, 181))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];
        if (Nodes.Count == 1) return [0];
        if (Nodes.Count == 2) return [0, 1];

        CancellationToken.ThrowIfCancellationRequested();

        //TODO: Implement Christofides algorithm
        //Steps (from paper):
        //1. Build minimum spanning tree
        //2. Find vertices with odd degree
        //3. Min-weight perfect matching on odd vertices
        //4. Combine MST + matching = Eulerian multigraph
        //5. Find Eulerian circuit
        //6. Convert to Hamiltonian (CYCLE) by skipping repeated vertices (:3)

        //Do as one of the last ones

        var nn = new NearestNeighborAlgorithm();
        nn.SetNodes(Nodes);
        nn.SetCancellationToken(CancellationToken);
        var tour = nn.Solve();

        ReportProgress(tour);
        return tour;
    }
}

