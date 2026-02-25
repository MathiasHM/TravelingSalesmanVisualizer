using System.Drawing.Drawing2D;
using TravelingSalesman.Models;

namespace TravelingSalesman.UI;

public class FullscreenAlgorithmCanvas : Panel
{
    private readonly string algorithmName;
    private readonly Color algorithmColor;
    private AlgorithmResult? result;
    private List<PointF> nodes = [];
    private const int NodeRadius = 8;

    public FullscreenAlgorithmCanvas(string name, Color color)
    {
        algorithmName = name;
        algorithmColor = color;
        BackColor = Color.White;
        DoubleBuffered = true;
        Dock = DockStyle.Fill;
    }

    public void SetResult(AlgorithmResult res, List<PointF> nodeList, bool showEdges)
    {
        result = res;
        nodes = new List<PointF>(nodeList);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        if (result == null || nodes.Count == 0)
        {
            DrawPlaceholder(e.Graphics);
            return;
        }

        DrawVisualization(e.Graphics);
    }

    private void DrawVisualization(Graphics g)
    {
        int margin = 40;
        int visualX = margin;
        int visualY = margin;
        int visualWidth = Width - 2 * margin;
        int visualHeight = Height - 2 * margin;

        if (result?.Path != null && result.Path.Count > 1)
        {
            using var pathPen = new Pen(algorithmColor, 3);
            for (int i = 0; i < result.Path.Count - 1; i++)
            {
                var p1 = ScalePoint(nodes[result.Path[i]], visualX, visualY, visualWidth, visualHeight);
                var p2 = ScalePoint(nodes[result.Path[i + 1]], visualX, visualY, visualWidth, visualHeight);
                g.DrawLine(pathPen, p1, p2);
                DrawArrow(g, pathPen, p1, p2);
            }

            var lastPoint = ScalePoint(nodes[result.Path[^1]], visualX, visualY, visualWidth, visualHeight);
            var firstPoint = ScalePoint(nodes[result.Path[0]], visualX, visualY, visualWidth, visualHeight);
            g.DrawLine(pathPen, lastPoint, firstPoint);
            DrawArrow(g, pathPen, lastPoint, firstPoint);
        }

        using var nodeFont = new Font("Segoe UI", 10, FontStyle.Bold);
        using var nodeBrush = new SolidBrush(algorithmColor);

        for (int i = 0; i < nodes.Count; i++)
        {
            var scaledNode = ScalePoint(nodes[i], visualX, visualY, visualWidth, visualHeight);
            var rect = new RectangleF(scaledNode.X - NodeRadius, scaledNode.Y - NodeRadius,
                NodeRadius * 2, NodeRadius * 2);

            g.FillEllipse(Brushes.White, rect);
            using var nodePen = new Pen(algorithmColor, 2);
            g.DrawEllipse(nodePen, rect);
        }
    }

    private void DrawArrow(Graphics g, Pen pen, PointF from, PointF to)
    {
        float dx = to.X - from.X;
        float dy = to.Y - from.Y;
        float length = (float)Math.Sqrt(dx * dx + dy * dy);

        if (length < 1) return;

        float midX = (from.X + to.X) / 2;
        float midY = (from.Y + to.Y) / 2;
        float angle = (float)Math.Atan2(dy, dx);
        const float arrowSize = 8;

        PointF arrowTip = new(midX, midY);
        PointF arrowLeft = new(
            midX - arrowSize * (float)Math.Cos(angle - Math.PI / 6),
            midY - arrowSize * (float)Math.Sin(angle - Math.PI / 6)
        );
        PointF arrowRight = new(
            midX - arrowSize * (float)Math.Cos(angle + Math.PI / 6),
            midY - arrowSize * (float)Math.Sin(angle + Math.PI / 6)
        );

        g.DrawLine(pen, arrowTip, arrowLeft);
        g.DrawLine(pen, arrowTip, arrowRight);
    }

    private void DrawPlaceholder(Graphics g)
    {
        using var font = new Font("Segoe UI", 12, FontStyle.Italic);
        using var brush = new SolidBrush(Color.Gray);
        string message = "No solution available";
        var size = g.MeasureString(message, font);
        g.DrawString(message, font, brush,
            (Width - size.Width) / 2,
            (Height - size.Height) / 2);
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
