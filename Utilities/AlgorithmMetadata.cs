namespace TravelingSalesman.Utilities;

public static class AlgorithmMetadata
{
    public record SettingInfo(string PropertyName, string DisplayName, decimal Min, decimal Max, decimal Increment, bool IsDouble = false);
    public record GroupInfo(string Name, string[] Algorithms);

    public static List<GroupInfo> GetAlgorithmGroups() =>
    [
        new("None",        []),
        new("Greedy",      ["Nearest Neighbor", "Greedy", "Nearest Insertion", "Farthest Insertion", "Cheapest Insertion"]),
        new("Local Search",["2-Opt", "3-Opt", "Lin-Kernighan", "Simulated Annealing", "Tabu Search"]),
        new("Evolutionary",["Genetic Algorithm", "Particle Swarm"]),
        new("Swarm",       ["Ant Colony", "Particle Swarm"]),
        new("Exact",       ["Brute Force", "Branch and Bound"]),
        new("All",         ["Nearest Neighbor", "Greedy", "2-Opt", "3-Opt", "Simulated Annealing",
                            "Genetic Algorithm", "Random Search", "Nearest Insertion", "Farthest Insertion",
                            "Cheapest Insertion", "Ant Colony", "Brute Force", "Lin-Kernighan",
                            "Christofides", "Tabu Search", "Particle Swarm", "Branch and Bound"]),
    ];

    public static List<SettingInfo> GetAlgorithmSettings(string algorithmName)
    {
        return algorithmName switch
        {
            "2-Opt" =>
            [
                new("TwoOptMaxIterations", "Max Iterations", 1, 10000, 10)
            ],
            "3-Opt" =>
            [
                new("ThreeOptMaxIterations", "Max Iterations", 1, 1000, 10)
            ],
            "Simulated Annealing" =>
            [
                new("SATemperature", "Initial Temperature", 100, 100000, 1000, true),
                new("SACoolingRate", "Cooling Rate", 0.8m, 0.999m, 0.001m, true)
            ],
            "Genetic Algorithm" =>
            [
                new("GAPopulationSize", "Population Size", 10, 1000, 10),
                new("GAGenerations", "Generations", 10, 10000, 50),
                new("GAMutationRate", "Mutation Rate", 0.001m, 0.5m, 0.01m, true)
            ],
            "Random Search" =>
            [
                new("RandomIterations", "Iterations", 100, 1000000, 1000)
            ],
            "Ant Colony" =>
            [
                new("ACOAnts", "Number of Ants", 5, 200, 5),
                new("ACOIterations", "Iterations", 10, 1000, 10),
                new("ACOAlpha", "Alpha (Pheromone Weight)", 0.1m, 5.0m, 0.1m, true),
                new("ACOBeta", "Beta (Heuristic Weight)", 0.1m, 10.0m, 0.1m, true),
                new("ACOEvaporation", "Evaporation Rate", 0.1m, 0.9m, 0.05m, true),
                new("ACOQ", "Q (Pheromone Deposit)", 10, 1000, 10, true)
            ],
            "Lin-Kernighan" =>
            [
                new("LKMaxIterations", "Max Iterations", 1, 1000, 10)
            ],
            "Tabu Search" =>
            [
                new("TabuMaxIterations", "Max Iterations", 100, 10000, 100),
                new("TabuTenure", "Tabu Tenure", 5, 100, 5)
            ],
            "Particle Swarm" =>
            [
                new("PSParticles", "Number of Particles", 10, 200, 10),
                new("PSIterations", "Iterations", 50, 1000, 50)
            ],
            //Algorithms without configurable parameters
            "Nearest Neighbor" => [],
            "Greedy" => [],
            "Nearest Insertion" => [],
            "Farthest Insertion" => [],
            "Cheapest Insertion" => [],
            "Brute Force" => [],
            "Christofides" => [],
            "Branch and Bound" => [],
            _ => []
        };
    }

    public static string GetWikipediaUrl(string algorithmName)
    {
        return algorithmName switch
        {
            "Nearest Neighbor" => "https://en.wikipedia.org/wiki/Nearest_neighbour_algorithm",
            "Greedy" => "https://en.wikipedia.org/wiki/Greedy_algorithm",
            "Nearest Insertion" => "https://en.wikipedia.org/wiki/Nearest_neighbour_algorithm#Insertion_heuristics",
            "Farthest Insertion" => "https://en.wikipedia.org/wiki/Nearest_neighbour_algorithm#Insertion_heuristics",
            "Cheapest Insertion" => "https://en.wikipedia.org/wiki/Nearest_neighbour_algorithm#Insertion_heuristics",
            "2-Opt" => "https://en.wikipedia.org/wiki/2-opt",
            "3-Opt" => "https://en.wikipedia.org/wiki/3-opt",
            "Simulated Annealing" => "https://en.wikipedia.org/wiki/Simulated_annealing",
            "Genetic Algorithm" => "https://en.wikipedia.org/wiki/Genetic_algorithm",
            "Ant Colony" => "https://en.wikipedia.org/wiki/Ant_colony_optimization_algorithms",
            "Random Search" => "https://en.wikipedia.org/wiki/Random_search",
            "Brute Force" => "https://en.wikipedia.org/wiki/Brute-force_search",
            "Lin-Kernighan" => "https://en.wikipedia.org/wiki/Lin%E2%80%93Kernighan_heuristic",
            "Christofides" => "https://en.wikipedia.org/wiki/Christofides_algorithm",
            "Tabu Search" => "https://en.wikipedia.org/wiki/Tabu_search",
            "Particle Swarm" => "https://en.wikipedia.org/wiki/Particle_swarm_optimization",
            "Branch and Bound" => "https://en.wikipedia.org/wiki/Branch_and_bound",
            _ => "https://en.wikipedia.org/wiki/Travelling_salesman_problem"
        };
    }

    public static List<string> CheckForLongRunningAlgorithms(List<string> enabledAlgorithms, int nodeCount)
    {
        var warnings = new List<string>();

        foreach (var algoName in enabledAlgorithms)
        {
            var estimate = EstimateRuntime(algoName, nodeCount);
            if (estimate != null)
            {
                warnings.Add($"{algoName}: {estimate}");
            }
        }

        return warnings;
    }

    public static string? EstimateRuntime(string algorithmName, int n)
    {
        return algorithmName switch
        {
            "Brute Force" => n switch
            {
                >= 43 => "♾️ Expected completion: After the heat death of the universe ♾️\n(Seriously, the universe will end first)",
                >= 42 => "You are asking the wrong question...",
                >= 22 => "♾️ Expected completion: After the heat death of the universe ♾️\n(Seriously, the universe will end first)",
                >= 21 => "Estimated runtime: Quintillions of years",
                >= 20 => "Estimated runtime: Trillions of years",
                >= 19 => "Estimated runtime: 200,000 years",
                >= 18 => "Estimated runtime: 10,000 years",
                >= 17 => "Estimated runtime: 8 months",
                >= 16 => "Estimated runtime: 2 weeks",
                >= 15 => "Estimated runtime: 1 day",
                >= 14 => "Estimated runtime: 1-2 hours",
                >= 13 => "Estimated runtime: 5-10 minutes",
                _ => null
            },

            "Branch and Bound" => n switch //TODO: fix the estimates, very wrong
            {
                >= 22 => "♾️ Expected completion: After the heat death of the universe ♾️\n(At least no one will collect on the power bill)",
                >= 20 => "Estimated runtime: Thousands of years",
                >= 19 => "Estimated runtime: Several years\n(You'll forget why you started this)",
                >= 18 => "Estimated runtime: Several months",
                >= 17 => "Estimated runtime: Several weeks",
                >= 16 => "Estimated runtime: Several days",
                >= 15 => "Estimated runtime: 6-24 hours",
                >= 14 => "Estimated runtime: 1-4 hours",
                >= 13 => "Estimated runtime: 10-30 minutes",
                >= 12 => "Estimated runtime: 1-5 minutes",
                _ => null
            },

            _ => null
        };
    }
}
