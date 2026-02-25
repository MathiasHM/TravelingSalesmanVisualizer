namespace TravelingSalesman.Algorithms;

//Greedy nearest neighbor - always go to closest unvisited node
public class NearestNeighborAlgorithm : TSPAlgorithm
{
    public NearestNeighborAlgorithm()
        : base("Nearest Neighbor", Color.FromArgb(76, 175, 80))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];

        var path = new List<int> { 0 };
        var unvisited = new HashSet<int>(Enumerable.Range(1, Nodes.Count - 1));

        while (unvisited.Count > 0)
        {
            int current = path[^1];
            int nearest = unvisited.MinBy(i => Distance(Nodes[current], Nodes[i]));
            path.Add(nearest);
            unvisited.Remove(nearest);

            //Report progress occasionally
            if (unvisited.Count % Math.Max(1, Nodes.Count / 10) == 0) //
            {
                ReportProgress(path);
            }
        }

        ReportProgress(path);
        return path;
    }
}
