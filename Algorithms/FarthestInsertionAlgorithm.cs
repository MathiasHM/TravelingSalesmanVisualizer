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

        var tour = new List<int>();
        var remaining = new HashSet<int>(Enumerable.Range(0, Nodes.Count));

        //Start with the two farthest points.
        int node1 = 0, node2 = 1;
        double maxDist = 0;
        for (int i = 0; i < Nodes.Count; i++)
        {
            for (int j = i + 1; j < Nodes.Count; j++)
            {
                double dist = Distance(Nodes[i], Nodes[j]);
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
            /*For every remaining node, find the minimum distance to any tour node.
            Then pick the node whose minimum distance is the largest.*/
            int farthestNode = -1;
            double maxMinDist = double.MinValue;

            foreach (var node in remaining)
            {
                double minDistToTour = double.MaxValue;
                foreach (var tourNode in tour)
                {
                    double dist = Distance(Nodes[node], Nodes[tourNode]);
                    if (dist < minDistToTour)
                        minDistToTour = dist;
                }

                if (minDistToTour > maxMinDist)
                {
                    maxMinDist = minDistToTour;
                    farthestNode = node;
                }
            }

            //Find the cheapest position to insert
            int bestPos = 0;
            double minIncrease = double.MaxValue;

            for (int i = 0; i < tour.Count; i++)
            {
                int next = (i + 1) % tour.Count;
                double increase = Distance(Nodes[tour[i]], Nodes[farthestNode]) +
                                  Distance(Nodes[farthestNode], Nodes[tour[next]]) -
                                  Distance(Nodes[tour[i]], Nodes[tour[next]]);

                if (increase < minIncrease)
                {
                    minIncrease = increase;
                    bestPos = i + 1;
                }
            }

            tour.Insert(bestPos, farthestNode);
            remaining.Remove(farthestNode);

            if (remaining.Count % Math.Max(1, Nodes.Count / 10) == 0)
                ReportProgress(tour);
        }

        ReportProgress(tour);
        return tour;
    }
}