namespace TravelingSalesman.Models;

public record AlgorithmResult(List<int> Path, double Distance, long TimeMs, string Status = "Completed");
