using System.Drawing.Drawing2D;
using TravelingSalesman.Models;

namespace TravelingSalesman.UI;

public class ComparisonCanvas : Panel
{
    private List<PointF> nodes = [];
    private Dictionary<string, (AlgorithmResult result, Color color)> results = [];
    private const int NodeRadius = 6;
    private int draggedNodeIndex = -1;
    private bool isDragging = false;

    public event EventHandler? NodesChanged;

    public ComparisonCanvas()
    {
        BackColor = Color.White;
        DoubleBuffered = true;
    }

    public List<PointF> GetNodes()
    {
        return new List<PointF>(nodes);
    }

    public void SetNodes(List<PointF> nodeList)
    {
        nodes = new List<PointF>(nodeList);
        Invalidate();
    }

    public void SetResults(Dictionary<string, (AlgorithmResult, Color)> resultDict)
    {
        results = new Dictionary<string, (AlgorithmResult, Color)>(resultDict);
        Invalidate();
    }

    public void Clear()
    {
        results.Clear();
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        if (results.Count == 0)
        {
            DrawNodesOnly(e.Graphics);
        }
        else
        {
            DrawComparison(e.Graphics);
        }
    }

    private void DrawComparison(Graphics g)
    {
        var sortedResults = results.OrderBy(r => r.Value.result.Distance).ToList();
        int panelCount = Math.Min(3, sortedResults.Count);
        int panelWidth = Width / panelCount;

        for (int i = 0; i < sortedResults.Count; i++)
        {
            var (name, (result, color)) = sortedResults[i];
            int xOffset = (i % panelCount) * panelWidth;
            int yOffset = (i / panelCount) * (Height / ((sortedResults.Count + panelCount - 1) / panelCount));
            int height = Height / ((sortedResults.Count + panelCount - 1) / panelCount);

            DrawAlgorithmPanel(g, name, result, color, xOffset, yOffset, panelWidth, height);
        }
    }

    private void DrawAlgorithmPanel(Graphics g, string name, AlgorithmResult result, Color color,
        int x, int y, int width, int height)
    {
        g.FillRectangle(new SolidBrush(Color.FromArgb(250, 250, 250)), x, y, width, height);
        g.DrawRectangle(Pens.LightGray, x, y, width - 1, height - 1);

        const int headerHeight = 50;
        using var headerBrush = new SolidBrush(color);
        g.FillRectangle(headerBrush, x, y, width, headerHeight);

        using var titleFont = new Font("Segoe UI", 10, FontStyle.Bold);
        using var infoFont = new Font("Segoe UI", 8);

        g.DrawString(name, titleFont, Brushes.White, x + 10, y + 8);
        g.DrawString($"Distance: {result.Distance:F2}", infoFont, Brushes.White, x + 10, y + 26);
        g.DrawString($"Time: {result.TimeMs}ms", infoFont, Brushes.White, x + 10, y + 38);

        int visualX = x + 10;
        int visualY = y + headerHeight + 10;
        int visualWidth = width - 20;
        int visualHeight = height - headerHeight - 20;

        DrawPathVisualization(g, result.Path, color, visualX, visualY, visualWidth, visualHeight);
    }

    private void DrawPathVisualization(Graphics g, List<int> path, Color color,
        int x, int y, int width, int height)
    {
        if (nodes.Count == 0) return;

        if (path.Count > 1)
        {
            using var pathPen = new Pen(color, 2);
            for (int i = 0; i < path.Count - 1; i++)
            {
                var p1 = ScalePoint(nodes[path[i]], x, y, width, height);
                var p2 = ScalePoint(nodes[path[i + 1]], x, y, width, height);
                g.DrawLine(pathPen, p1, p2);
                DrawArrow(g, pathPen, p1, p2);
            }

            var lastPoint = ScalePoint(nodes[path[^1]], x, y, width, height);
            var firstPoint = ScalePoint(nodes[path[0]], x, y, width, height);
            g.DrawLine(pathPen, lastPoint, firstPoint);
            DrawArrow(g, pathPen, lastPoint, firstPoint);
        }

        using var nodeFont = new Font("Segoe UI", 7);
        using var nodeBrush = new SolidBrush(color);

        for (int i = 0; i < nodes.Count; i++)
        {
            var scaledNode = ScalePoint(nodes[i], x, y, width, height);
            var rect = new RectangleF(scaledNode.X - NodeRadius, scaledNode.Y - NodeRadius,
                NodeRadius * 2, NodeRadius * 2);

            g.FillEllipse(Brushes.White, rect);
            g.DrawEllipse(new Pen(color, 2), rect);
        }
    }

    private void DrawNodesOnly(Graphics g)
    {
        if (nodes.Count == 0) return;

        using var font = new Font("Segoe UI", 9);
        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            var rect = new RectangleF(node.X - NodeRadius, node.Y - NodeRadius,
                NodeRadius * 2, NodeRadius * 2);

            g.FillEllipse(Brushes.Red, rect);
            g.DrawEllipse(Pens.DarkRed, rect);
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
        const float arrowSize = 6;

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

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.Button == MouseButtons.Left && results.Count == 0)
        {
            //Check if clicking near a node to start dragging
            for (int i = 0; i < nodes.Count; i++)
            {
                if (Math.Abs(nodes[i].X - e.X) < 10 && Math.Abs(nodes[i].Y - e.Y) < 10)
                {
                    draggedNodeIndex = i;
                    isDragging = true;
                    Cursor = Cursors.Hand;
                    return;
                }
            }
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        if (isDragging && draggedNodeIndex >= 0 && draggedNodeIndex < nodes.Count && results.Count == 0)
        {
            nodes[draggedNodeIndex] = new PointF(e.X, e.Y);
            NodesChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }
        else if (results.Count == 0)
        {
            //Update cursor when hovering over nodes
            bool overNode = false;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (Math.Abs(nodes[i].X - e.X) < 10 && Math.Abs(nodes[i].Y - e.Y) < 10)
                {
                    overNode = true;
                    break;
                }
            }
            Cursor = overNode ? Cursors.Hand : Cursors.Default;
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);

        if (isDragging)
        {
            isDragging = false;
            draggedNodeIndex = -1;
            Cursor = Cursors.Default;
        }
    }
}
