namespace TravelingSalesman.Algorithms;

public class BruteForceAlgorithm : TSPAlgorithm
{
    public BruteForceAlgorithm()
        : base("Brute Force", Color.FromArgb(96, 125, 139))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];
        if (Nodes.Count == 1) return [0];
        if (Nodes.Count == 2) return [0, 1];

        //Initialize with simple path
        var bestPath = Enumerable.Range(0, Nodes.Count).ToList();
        var bestDistance = double.MaxValue;

        long totalPermutations = Factorial(Nodes.Count - 1);
        long permutationCount = 0;
        long reportInterval = Math.Max(1, totalPermutations / 50);
        var remaining = Enumerable.Range(1, Nodes.Count - 1).ToList();
        PermuteAndFindBest(remaining, new List<int> { 0 }, ref bestPath,
            ref bestDistance, ref permutationCount, reportInterval);

        return bestPath;
    }

    private void PermuteAndFindBest(List<int> remaining, List<int> currentPath,
        ref List<int> bestPath, ref double bestDistance,
        ref long permutationCount, long reportInterval)
    {
        CancellationToken.ThrowIfCancellationRequested();

        if (remaining.Count == 0)
        {
            //path complete
            double distance = CalculatePathDistance(currentPath);

            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestPath = new List<int>(currentPath);
                ReportProgress(bestPath);
            }

            permutationCount++;

            if (permutationCount % reportInterval == 0)
            {
                ReportProgress(bestPath);
            }

            return;
        }

        for (int i = 0; i < remaining.Count; i++)
        {
            int nextNode = remaining[i];
            currentPath.Add(nextNode);
            var newRemaining = new List<int>(remaining);
            newRemaining.RemoveAt(i);
            PermuteAndFindBest(newRemaining, currentPath, ref bestPath,
                ref bestDistance, ref permutationCount, reportInterval);
            currentPath.RemoveAt(currentPath.Count - 1);
        }
    }

    private long Factorial(int n)
    {
        if (n <= 1) return 1;
        long result = 1;
        for (int i = 2; i <= n; i++)
        {
            result *= i;
        }
        return result;
    }
}
