namespace TravelingSalesman.Algorithms;

public class BranchAndBoundAlgorithm : TSPAlgorithm
{
    private double bestCost;
    private List<int> bestPath = [];
    private long nodesExplored;
    private long nodesPruned;

    public BranchAndBoundAlgorithm()
        : base("Branch and Bound", Color.FromArgb(63, 81, 181))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];
        if (Nodes.Count == 1) return [0];
        if (Nodes.Count == 2) return [0, 1];

        //Get initial upper bound
        var nn = new NearestNeighborAlgorithm();
        nn.SetNodes(Nodes);
        nn.SetCancellationToken(CancellationToken);
        bestPath = nn.Solve();
        bestCost = CalculatePathDistance(bestPath);

        nodesExplored = 0;
        nodesPruned = 0;

        ReportProgress(bestPath);

        //Speed
        var distMatrix = new double[Nodes.Count, Nodes.Count];
        for (int i = 0; i < Nodes.Count; i++)
        {
            for (int j = 0; j < Nodes.Count; j++)
            {
                distMatrix[i, j] = Distance(Nodes[i], Nodes[j]);
            }
        }

        //Startíng from node 0 reduces branching factor by 1
        var currentPath = new List<int> { 0 };
        var remaining = Enumerable.Range(1, Nodes.Count - 1).ToHashSet();

        BranchAndBound(currentPath, remaining, 0, distMatrix);

        ReportProgress(bestPath);
        return bestPath;
    }

    private void BranchAndBound(List<int> currentPath, HashSet<int> remaining,
        double currentCost, double[,] distMatrix)
    {
        CancellationToken.ThrowIfCancellationRequested();

        nodesExplored++;

        if (nodesExplored % 10000 == 0)
        {
            ReportProgress(bestPath);
        }

        if (remaining.Count == 0)
        {
            //Add cost to return to start
            double totalCost = currentCost + distMatrix[currentPath[^1], currentPath[0]];

            if (totalCost < bestCost)
            {
                bestCost = totalCost;
                bestPath = new List<int>(currentPath);
                ReportProgress(bestPath);
            }
            return;
        }

        //Calculate lower bound using minimum spanning tree + current path
        double lowerBound = currentCost + CalculateLowerBound(currentPath, remaining, distMatrix);

        //Prune if lower bound exceeds best known solution
        if (lowerBound >= bestCost)
        {
            nodesPruned++;
            return;
        }

        //Branch: try adding each remaining node
        var sortedRemaining = remaining
            .OrderBy(node => distMatrix[currentPath[^1], node])
            .ToList();

        foreach (var nextNode in sortedRemaining)
        {
            CancellationToken.ThrowIfCancellationRequested();

            double edgeCost = distMatrix[currentPath[^1], nextNode];
            currentPath.Add(nextNode);
            remaining.Remove(nextNode);

            BranchAndBound(currentPath, remaining, currentCost + edgeCost, distMatrix);

            remaining.Add(nextNode);
            currentPath.RemoveAt(currentPath.Count - 1);
        }
    }

    private double CalculateLowerBound(List<int> currentPath, HashSet<int> remaining, double[,] distMatrix)
    {
        if (remaining.Count == 0)
            return distMatrix[currentPath[^1], currentPath[0]];

        double bound = 0;

        //Minimum cost to leave current node
        int lastNode = currentPath[^1];
        double minFromLast = double.MaxValue;
        foreach (var node in remaining)
        {
            minFromLast = Math.Min(minFromLast, distMatrix[lastNode, node]);
        }
        bound += minFromLast;

        //Minimum cost to return to start from remaining nodes
        double minToStart = double.MaxValue;
        foreach (var node in remaining)
        {
            minToStart = Math.Min(minToStart, distMatrix[node, currentPath[0]]);
        }
        bound += minToStart;

        //MST of remaining nodes (simplified: use sum of two smallest edges for each node)
        if (remaining.Count > 1)
        {
            foreach (var node in remaining)
            {
                var distances = new List<double>();

                //Distance to other remaining nodes
                foreach (var other in remaining)
                {
                    if (other != node)
                    {
                        distances.Add(distMatrix[node, other]);
                    }
                }

                //Take the smallest edge (for MST approximation)
                distances.Sort();
                if (distances.Count >= 1)
                {
                    bound += distances[0];
                }
            }
            //Correct for each edge being counted twice
            bound = minFromLast + minToStart + (bound - minFromLast - minToStart) / 2;
        }

        return bound;
    }
}
