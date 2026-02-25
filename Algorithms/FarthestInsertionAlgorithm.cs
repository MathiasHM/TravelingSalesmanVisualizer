namespace TravelingSalesman.Algorithms;

public class FarthestInsertionAlgorithm : TSPAlgorithm
{
    public FarthestInsertionAlgorithm()
        : base("Farthest Insertion", Color.FromArgb(0, 188, 212))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];
        if (Nodes.Count == 1) return [0];
        if (Nodes.Count == 2) return [0, 1];

        //TODO: Implement farthest insertion
        //Copy paste nearest tbh

        var tour = new List<int> { 0, 1 };
        var remaining = new HashSet<int>(Enumerable.Range(2, Nodes.Count - 2));

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
