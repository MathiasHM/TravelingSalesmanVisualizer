namespace TravelingSalesman.Algorithms;

public class AntColonyAlgorithm : TSPAlgorithm
{
    public int Ants { get; set; } = 20;
    public int Iterations { get; set; } = 100;
    public double Alpha { get; set; } = 1.0;  //Pheromone importance
    public double Beta { get; set; } = 2.0;   //Distance importance
    public double Evaporation { get; set; } = 0.5;
    public double Q { get; set; } = 100.0;    //Pheromone constant

    public AntColonyAlgorithm()
        : base("Ant Colony", Color.FromArgb(121, 85, 72))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];
        if (Nodes.Count == 1) return [0];

        //TODO:Implement ant colony optimization

        var nn = new NearestNeighborAlgorithm();
        nn.SetNodes(Nodes);
        var path = nn.Solve();

        ReportProgress(path);
        return path;
    }
}

