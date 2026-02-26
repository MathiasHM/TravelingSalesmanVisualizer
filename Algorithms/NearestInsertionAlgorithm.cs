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

        var tour = new List<int>();
        var remaining = new HashSet<int>(Enumerable.Range(0, Nodes.Count));

        //Start with two farthest points
        int node1 = 0, node2 = 1;
        double maxDist = 0;
        for (int i = 0; i < Nodes.Count; i++)
        {
            for (int j = i + 1; j < Nodes.Count; j++)
            {
                var dist = Distance(Nodes[i], Nodes[j]);
                if (dist > maxDist)
                {
                    maxDist = dist;
                    node1 = i;
                    node2 = j;
                }
            }
        }

        tour.Add(node1);
        tour.Add(node2);
        remaining.Remove(node1);
        remaining.Remove(node2);

        while (remaining.Count > 0)
        {
            //Find nearest node to tour
            int nearestNode = -1;
            double minDistToTour = double.MaxValue;

            foreach (var node in remaining)
            {
                foreach (var tourNode in tour)
                {
                    var dist = Distance(Nodes[node], Nodes[tourNode]);
                    if (dist < minDistToTour)
                    {
                        minDistToTour = dist;
                        nearestNode = node;
                    }
                }
            }

            //Find best position to insert
            int bestPos = 0;
            double minIncrease = double.MaxValue;

            for (int i = 0; i < tour.Count; i++)
            {
                int next = (i + 1) % tour.Count;
                double increase = Distance(Nodes[tour[i]], Nodes[nearestNode]) +
                                Distance(Nodes[nearestNode], Nodes[tour[next]]) -
                                Distance(Nodes[tour[i]], Nodes[tour[next]]);

                if (increase < minIncrease)
                {
                    minIncrease = increase;
                    bestPos = i + 1;
                }
            }

            tour.Insert(bestPos, nearestNode);
            remaining.Remove(nearestNode);

            if (remaining.Count % Math.Max(1, Nodes.Count / 10) == 0)
            {
                ReportProgress(tour);
            }
        }

        ReportProgress(tour);
        return tour;
    }
}
