namespace TravelingSalesman.Models;

public class AlgorithmSettings
{
    //2-Opt
    public int TwoOptMaxIterations { get; set; } = 100;

    //3-Opt
    public int ThreeOptMaxIterations { get; set; } = 50;

    //SA
    public double SATemperature { get; set; } = 10000;
    public double SACoolingRate { get; set; } = 0.995;

    //GA
    public int GAPopulationSize { get; set; } = 100;
    public int GAGenerations { get; set; } = 500;
    public double GAMutationRate { get; set; } = 0.02;

    //Random
    public int RandomIterations { get; set; } = 10000;

    //Ant Colony settings
    public int ACOAnts { get; set; } = 20;
    public int ACOIterations { get; set; } = 100;
    public double ACOAlpha { get; set; } = 1.0;
    public double ACOBeta { get; set; } = 2.0;
    public double ACOEvaporation { get; set; } = 0.5;
    public double ACOQ { get; set; } = 100.0;

    //L-K
    public int LKMaxIterations { get; set; } = 50;

    //Tabu
    public int TabuMaxIterations { get; set; } = 500;
    public int TabuTenure { get; set; } = 20;

    //PS
    public int PSParticles { get; set; } = 30;
    public int PSIterations { get; set; } = 200;

    //Algorithm selection on startup
    public Dictionary<string, bool> EnabledAlgorithms { get; set; } = new()
    {
        ["Nearest Neighbor"] = true,
        ["Greedy"] = false,
        ["2-Opt"] = true,
        ["3-Opt"] = false,
        ["Simulated Annealing"] = false,
        ["Genetic Algorithm"] = false,
        ["Random Search"] = true,
        ["Nearest Insertion"] = false,
        ["Farthest Insertion"] = false,
        ["Cheapest Insertion"] = false,
        ["Ant Colony"] = false,
        ["Brute Force"] = false,
        ["Lin-Kernighan"] = false,
        ["Christofides"] = false,
        ["Tabu Search"] = false,
        ["Particle Swarm"] = false,
        ["Branch and Bound"] = false
    };

    public void Reset()
    {
        TwoOptMaxIterations = 100;
        ThreeOptMaxIterations = 50;
        SATemperature = 10000;
        SACoolingRate = 0.995;
        GAPopulationSize = 100;
        GAGenerations = 500;
        GAMutationRate = 0.02;
        RandomIterations = 10000;
        ACOAnts = 20;
        ACOIterations = 100;
        ACOAlpha = 1.0;
        ACOBeta = 2.0;
        ACOEvaporation = 0.5;
        ACOQ = 100.0;
        LKMaxIterations = 50;
        TabuMaxIterations = 500;
        TabuTenure = 20;
        PSParticles = 30;
        PSIterations = 200;
    }
}

