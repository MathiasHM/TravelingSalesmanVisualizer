namespace TravelingSalesman.Algorithms;

public class NearestInsertionAlgorithm : TSPAlgorithm
{
    public NearestInsertionAlgorithm()
        : base("Nearest Insertion", Color.FromArgb(103, 58, 183))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];
        if (Nodes.Count == 1) return [0];
        if (Nodes.Count == 2) return [0, 1];

        //TODO: Implement nearest insertion properly
        var tour = new List<int> { 0, 1 };
        var remaining = new HashSet<int>(Enumerable.Range(2, Nodes.Count - 2));

        while (remaining.Count > 0)
        {
            var nearestNode = remaining.First();
            tour.Add(nearestNode);
            remaining.Remove(nearestNode);
        }

        ReportProgress(tour);
        return tour;
    }
}
