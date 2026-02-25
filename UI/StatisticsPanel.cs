using TravelingSalesman.Models;

namespace TravelingSalesman.UI;

public class StatisticsPanel : Panel
{
    private readonly RichTextBox statsTextBox;
    private Dictionary<string, AlgorithmResult> results = [];
    private int nodeCount;

    public StatisticsPanel()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.White;
        AutoScroll = true;
        Padding = new Padding(20);

        statsTextBox = new RichTextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            Font = new Font("Consolas", 10),
            BackColor = Color.White,
            BorderStyle = BorderStyle.None
        };

        Controls.Add(statsTextBox);
    }

    public void UpdateStatistics(Dictionary<string, AlgorithmResult> algorithmResults, int totalNodes)
    {
        results = new Dictionary<string, AlgorithmResult>(algorithmResults);
        nodeCount = totalNodes;
        GenerateReport();
    }

    private void GenerateReport()
    {
        if (results.Count == 0)
        {
            statsTextBox.Text = "Run algorithms to see detailed statistics...";
            return;
        }

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("═══════════════════════════════════════════════════════════════");
        sb.AppendLine("                    ALGORITHM ANALYSIS REPORT");
        sb.AppendLine("═══════════════════════════════════════════════════════════════\n");

        sb.AppendLine($"Total Nodes: {nodeCount}");
        sb.AppendLine($"Search Space Size: {Factorial(nodeCount):E2}\n");

        var bestResult = results.MinBy(r => r.Value.Distance);
        var worstResult = results.MaxBy(r => r.Value.Distance);
        var fastestResult = results.MinBy(r => r.Value.TimeMs);

        sb.AppendLine("🏆 BEST SOLUTION:");
        sb.AppendLine($"   Algorithm: {bestResult.Key}");
        sb.AppendLine($"   Distance: {bestResult.Value.Distance:F2}");
        sb.AppendLine($"   Time: {bestResult.Value.TimeMs}ms\n");

        sb.AppendLine("⚡ FASTEST ALGORITHM:");
        sb.AppendLine($"   Algorithm: {fastestResult.Key}");
        sb.AppendLine($"   Time: {fastestResult.Value.TimeMs}ms");
        sb.AppendLine($"   Distance: {fastestResult.Value.Distance:F2}\n");

        sb.AppendLine("📊 DETAILED COMPARISON:");
        sb.AppendLine("─────────────────────────────────────────────────────────────");
        sb.AppendLine($"{"Algorithm",-25} {"Distance",12} {"Time",10} {"Quality",10}");
        sb.AppendLine("─────────────────────────────────────────────────────────────");

        foreach (var result in results.OrderBy(r => r.Value.Distance))
        {
            double quality = (1 - (result.Value.Distance - bestResult.Value.Distance) /
                Math.Max(1, bestResult.Value.Distance)) * 100;
            sb.AppendLine($"{result.Key,-25} {result.Value.Distance,12:F2} {result.Value.TimeMs,8}ms {quality,9:F1}%");
        }

        sb.AppendLine("\n📈 PERFORMANCE METRICS:");
        sb.AppendLine("─────────────────────────────────────────────────────────────");
        sb.AppendLine($"Best Distance: {bestResult.Value.Distance:F2}");
        sb.AppendLine($"Worst Distance: {worstResult.Value.Distance:F2}");
        sb.AppendLine($"Distance Range: {worstResult.Value.Distance - bestResult.Value.Distance:F2}");
        sb.AppendLine($"Average Distance: {results.Average(r => r.Value.Distance):F2}");
        sb.AppendLine($"Average Time: {results.Average(r => r.Value.TimeMs):F2}ms");

        if (worstResult.Value.Distance > 0)
        {
            double improvement = ((worstResult.Value.Distance - bestResult.Value.Distance) /
                worstResult.Value.Distance) * 100;
            sb.AppendLine($"Best vs Worst Improvement: {improvement:F2}%");
        }

        statsTextBox.Text = sb.ToString();
    }

    private static double Factorial(int n)
    {
        if (n <= 1) return 1;
        double result = 1;
        for (int i = 2; i <= Math.Min(n, 20); i++)
            result *= i;
        return result;
    }
}
