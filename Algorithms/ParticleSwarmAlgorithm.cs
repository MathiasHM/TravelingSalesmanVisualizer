namespace TravelingSalesman.Algorithms;

public class ParticleSwarmAlgorithm : TSPAlgorithm
{
    public int Particles { get; set; } = 30;
    public int Iterations { get; set; } = 200;
    public double InertiaWeight { get; set; } = 0.7;
    public double CognitiveWeight { get; set; } = 1.5;
    public double SocialWeight { get; set; } = 1.5;

    public ParticleSwarmAlgorithm()
        : base("Particle Swarm", Color.FromArgb(0, 150, 136))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];
        if (Nodes.Count == 1) return [0];
        if (Nodes.Count == 2) return [0, 1];

        //TODO: Implement particle swarm optimization

        var path = Enumerable.Range(0, Nodes.Count).ToList();
        ReportProgress(path);
        return path;
    }
}
