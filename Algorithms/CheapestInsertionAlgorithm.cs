namespace TravelingSalesman.Algorithms;

public class CheapestInsertionAlgorithm : TSPAlgorithm
{
    public CheapestInsertionAlgorithm()
        : base("Cheapest Insertion", Color.FromArgb(96, 125, 139))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];
        if (Nodes.Count == 1) return [0];
        if (Nodes.Count == 2) return [0, 1];

        //TODO: Implement cheapest insertion

        var tour = new List<int> { 0 };
        var remaining = new HashSet<int>(Enumerable.Range(1, Nodes.Count - 1));

        while (remaining.Count > 0)
        {
            var next = remaining.First();
            tour.Add(next);
            remaining.Remove(next);
        }

        ReportProgress(tour);
        return tour;
    }
}
