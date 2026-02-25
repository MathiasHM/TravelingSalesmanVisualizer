using System.Drawing.Drawing2D;
using TravelingSalesman.Models;

namespace TravelingSalesman.UI;

public class AlgorithmVisualizationPanel : Panel
{
    private readonly string algorithmName;
    private readonly Color algorithmColor;
    private AlgorithmResult? result;
    private List<PointF> nodes = [];
    private const int NodeRadius = 5;
    private const int HeaderHeight = 70;

    public AlgorithmVisualizationPanel(string name, Color color)
    {
        algorithmName = name;
        algorithmColor = color;
        Size = new Size(380, 350);
        BackColor = Color.White;
        BorderStyle = BorderStyle.FixedSingle;
        Margin = new Padding(5);
        DoubleBuffered = true;
    }

    public void SetResult(AlgorithmResult res, List<PointF> nodeList, bool showEdges)
    {
        result = res;
        nodes = new List<PointF>(nodeList);
        Invalidate();
    }

    public void Clear()
    {
        result = null;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        DrawHeader(e.Graphics);

        if (result != null && nodes.Count > 0)
        {
            DrawVisualization(e.Graphics);
        }
        else
        {
            DrawPlaceholder(e.Graphics);
        }
    }

    private void DrawHeader(Graphics g)
    {
        using var headerBrush = new SolidBrush(algorithmColor);
        g.FillRectangle(headerBrush, 0, 0, Width, HeaderHeight);

        if (result?.Status == "Running...")
        {
            using var glowBrush = new SolidBrush(Color.FromArgb(40, 255, 255, 255));
            g.FillRectangle(glowBrush, 0, 0, Width, HeaderHeight);
        }

        using var titleFont = new Font("Segoe UI", 11, FontStyle.Bold);
        using var infoFont = new Font("Segoe UI", 8.5f);

        g.DrawString(algorithmName, titleFont, Brushes.White, 10, 10);

        if (result != null)
        {
            string info = $"Distance: {result.Distance:F2}";
            g.DrawString(info, infoFont, Brushes.White, 10, 32);

            string status = result.Status == "Running..." ? $"{result.Status}" : $"Time: {result.TimeMs}ms";
            var statusBrush = result.Status == "Running..." ? Brushes.Yellow : Brushes.White;
            g.DrawString(status, infoFont, statusBrush, 10, 45);
        }
        else
        {
            g.DrawString("No solution yet", infoFont, Brushes.White, 10, 35);
        }
    }

    private void DrawVisualization(Graphics g)
    {
        int visualX = 10;
        int visualY = HeaderHeight + 10;
        int visualWidth = Width - 20;
        int visualHeight = Height - HeaderHeight - 20;

        if (result?.Path != null && result.Path.Count > 1)
        {
            using var pathPen = new Pen(algorithmColor, 1.5f);
            for (int i = 0; i < result.Path.Count - 1; i++)
            {
                var p1 = ScalePoint(nodes[result.Path[i]], visualX, visualY, visualWidth, visualHeight);
                var p2 = ScalePoint(nodes[result.Path[i + 1]], visualX, visualY, visualWidth, visualHeight);
                g.DrawLine(pathPen, p1, p2);
            }

            var lastPoint = ScalePoint(nodes[result.Path[^1]], visualX, visualY, visualWidth, visualHeight);
            var firstPoint = ScalePoint(nodes[result.Path[0]], visualX, visualY, visualWidth, visualHeight);
            g.DrawLine(pathPen, lastPoint, firstPoint);
        }

        using var nodeFont = new Font("Segoe UI", 7);
        using var nodeBrush = new SolidBrush(algorithmColor);

        for (int i = 0; i < nodes.Count; i++)
        {
            var scaledNode = ScalePoint(nodes[i], visualX, visualY, visualWidth, visualHeight);
            var rect = new RectangleF(scaledNode.X - NodeRadius, scaledNode.Y - NodeRadius,
                NodeRadius * 2, NodeRadius * 2);

            g.FillEllipse(Brushes.White, rect);
            using var nodePen = new Pen(algorithmColor, 1.5f);
            g.DrawEllipse(nodePen, rect);
        }
    }

    private void DrawPlaceholder(Graphics g)
    {
        using var font = new Font("Segoe UI", 9, FontStyle.Italic);
        using var brush = new SolidBrush(Color.Gray);
        string message = "Run algorithm to see results";
        var size = g.MeasureString(message, font);
        g.DrawString(message, font, brush,
            (Width - size.Width) / 2,
            (Height + HeaderHeight - size.Height) / 2);
    }

    private PointF ScalePoint(PointF point, int offsetX, int offsetY, int width, int height)
    {
        if (nodes.Count == 0) return point;

        float minX = nodes.Min(p => p.X);
        float maxX = nodes.Max(p => p.X);
        float minY = nodes.Min(p => p.Y);
        float maxY = nodes.Max(p => p.Y);

        float rangeX = Math.Max(1, maxX - minX);
        float rangeY = Math.Max(1, maxY - minY);
        float scaleX = width / rangeX;
        float scaleY = height / rangeY;
        float scale = Math.Min(scaleX, scaleY) * 0.9f;

        float scaledWidth = rangeX * scale;
        float scaledHeight = rangeY * scale;
        float centerOffsetX = (width - scaledWidth) / 2;
        float centerOffsetY = (height - scaledHeight) / 2;

        return new PointF(
            offsetX + centerOffsetX + (point.X - minX) * scale,
            offsetY + centerOffsetY + (point.Y - minY) * scale
        );
    }
}
