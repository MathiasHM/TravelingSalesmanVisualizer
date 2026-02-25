namespace TravelingSalesman.Algorithms;

//Random search - just generates random tours and keeps the best one
public class RandomAlgorithm : TSPAlgorithm
{
    public int Iterations { get; set; } = 1000000;

    public RandomAlgorithm()
        : base("Random Search", Color.FromArgb(158, 158, 158))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];

        var bestPath = Enumerable.Range(0, Nodes.Count).ToList();
        var bestDistance = CalculatePathDistance(bestPath);
        int reportInterval = Math.Max(1, Iterations / 40);

        ReportProgress(bestPath);

        for (int i = 0; i < Iterations; i++)
        {
            CancellationToken.ThrowIfCancellationRequested();

            var path = Enumerable.Range(0, Nodes.Count).ToList();

            //Shuffle
            for (int j = path.Count - 1; j > 0; j--)
            {
                int k = Random.Next(j + 1);
                (path[j], path[k]) = (path[k], path[j]);
            }

            var distance = CalculatePathDistance(path);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestPath = path;

                if (i % reportInterval == 0)
                {
                    ReportProgress(bestPath);
                }
            }
        }

        return bestPath;
    }
}
