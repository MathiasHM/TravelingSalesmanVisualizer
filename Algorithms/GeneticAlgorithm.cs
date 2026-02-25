namespace TravelingSalesman.Algorithms;

public class GeneticAlgorithm : TSPAlgorithm
{
    public int PopulationSize { get; set; } = 100;
    public int Generations { get; set; } = 500;
    public double MutationRate { get; set; } = 0.02;

    public GeneticAlgorithm()
        : base("Genetic Algorithm", Color.FromArgb(244, 67, 54))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];

        //TODO: Implement genetic algorithm
        var path = Enumerable.Range(0, Nodes.Count).ToList();
        for (int i = path.Count - 1; i > 0; i--)
        {
            int j = Random.Next(i + 1);
            (path[i], path[j]) = (path[j], path[i]);
        }

        ReportProgress(path);
        return path;
    }
}
