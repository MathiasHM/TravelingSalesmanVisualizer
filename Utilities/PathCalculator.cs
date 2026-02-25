namespace TravelingSalesman.Utilities;

public static class PathCalculator
{
    public static double CalculateDistance(List<int> path, List<PointF> nodes)
    {
        if (path.Count == 0) return 0;

        double total = 0;
        for (int i = 0; i < path.Count - 1; i++)
        {
            total += Distance(nodes[path[i]], nodes[path[i + 1]]);
        }
        total += Distance(nodes[path[^1]], nodes[path[0]]);
        return total;
    }

    public static double Distance(PointF p1, PointF p2)
    {
        double dx = p1.X - p2.X;
        double dy = p1.Y - p2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}
