//Lasciate ogne speranza, voi ch'intrate
using TravelingSalesman.Algorithms;
using TravelingSalesman.Models;
using TravelingSalesman.UI;
using TravelingSalesman.Utilities;

namespace TravelingSalesman;

public class MainForm : Form
{
    private readonly AlgorithmSettings settings = new();
    private readonly List<PointF> nodes = [];
    private readonly Dictionary<string, AlgorithmResult> results = [];
    private readonly Dictionary<string, TSPAlgorithm> algorithms = [];
    private readonly Dictionary<string, AlgorithmVisualizationPanel> algorithmPanels = [];
    private readonly Random random = new();

    private ComparisonCanvas comparisonCanvas = null!;
    private ResultsGrid resultsGrid = null!;
    private StatisticsPanel statisticsPanel = null!;
    private FlowLayoutPanel individualAlgorithmsPanel = null!;
    private TabControl tabControl = null!;

    private NumericUpDown numNodes = null!;
    private readonly Dictionary<string, CheckBox> algorithmCheckboxes = [];

    private Panel? fullscreenPanel;
    private string? fullscreenAlgorithmName;
    private CancellationTokenSource? cancellationTokenSource;

    public MainForm()
    {
        InitializeAlgorithms();
        InitializeComponent();
        Load += (s, e) => GenerateRandomNodes();
    }

    private void InitializeAlgorithms()
    {
        //Basic algorithms that work
        algorithms["Nearest Neighbor"] = new NearestNeighborAlgorithm();
        algorithms["Random Search"] = new RandomAlgorithm();

        //TODO: Finish implementing these
        algorithms["Greedy"] = new GreedyAlgorithm();
        algorithms["2-Opt"] = new TwoOptAlgorithm();
        algorithms["3-Opt"] = new ThreeOptAlgorithm();
        algorithms["Simulated Annealing"] = new SimulatedAnnealingAlgorithm();
        algorithms["Genetic Algorithm"] = new GeneticAlgorithm();
        algorithms["Nearest Insertion"] = new NearestInsertionAlgorithm();
        algorithms["Farthest Insertion"] = new FarthestInsertionAlgorithm();
        algorithms["Cheapest Insertion"] = new CheapestInsertionAlgorithm();
        algorithms["Ant Colony"] = new AntColonyAlgorithm();
        algorithms["Brute Force"] = new BruteForceAlgorithm();
        algorithms["Lin-Kernighan"] = new LinKernighanAlgorithm();
        algorithms["Christofides"] = new ChristofidesAlgorithm();
        algorithms["Tabu Search"] = new TabuSearchAlgorithm();
        algorithms["Particle Swarm"] = new ParticleSwarmAlgorithm();
        algorithms["Branch and Bound"] = new BranchAndBoundAlgorithm();
    }

    private void InitializeComponent()
    {
        Text = "TSP Algorithm Comparator";
        Size = new Size(1600, 1000);
        WindowState = FormWindowState.Maximized;
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(245, 245, 245);

        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 1,
            ColumnCount = 2
        };

        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 320));

        var contentLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1
        };

        contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70));
        contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30));

        contentLayout.Controls.Add(CreateContentArea(), 0, 0);
        contentLayout.Controls.Add(CreateResultsPanel(), 0, 1);

        mainLayout.Controls.Add(contentLayout, 0, 0);
        mainLayout.Controls.Add(CreateSidebarPanel(), 1, 0);

        Controls.Add(mainLayout);
    }

    private Panel CreateSidebarPanel()
    {
        var sidebar = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(240, 240, 240),
            Padding = new Padding(10)
        };

        var scrollContainer = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 1,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(0, 0, 0, 10)
        };

        var lblTitle = new Label
        {
            Text = "TSP Algorithm\nComparator",
            Dock = DockStyle.Top,
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            ForeColor = Color.FromArgb(0, 120, 215),
            TextAlign = ContentAlignment.TopLeft,
            Height = 60,
            Margin = new Padding(0, 0, 0, 15)
        };

        layout.Controls.Add(lblTitle);
        layout.Controls.Add(CreateActionsGroup());
        layout.Controls.Add(CreateGraphConfigGroup());
        layout.Controls.Add(CreateAlgorithmSelectionGroup());
        layout.Controls.Add(CreateQuickSelectGroup());

        scrollContainer.Controls.Add(layout);
        scrollContainer.AutoScrollMargin = new Size(0, 10);
        layout.Layout += (s, e) =>
            scrollContainer.AutoScrollMinSize = new Size(0, layout.PreferredSize.Height + layout.Padding.Vertical);
        sidebar.Controls.Add(scrollContainer);

        return sidebar;
    }

    private GroupBox CreateGraphConfigGroup()
    {
        var group = new GroupBox
        {
            Text = "Graph Configuration",
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Padding = new Padding(10),
            Margin = new Padding(0, 0, 0, 10)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            RowCount = 5,
            ColumnCount = 2,
            Padding = new Padding(5)
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));

        var lblNodes = new Label
        {
            Text = "Number of Nodes:",
            AutoSize = true,
            Font = new Font("Segoe UI", 9, FontStyle.Regular),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        numNodes = new NumericUpDown
        {
            Minimum = 3,
            Maximum = 999,
            Value = 20,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9)
        };

        var btnRandomize = new Button
        {
            Text = "🔄 Randomize",
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(0, 150, 136),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Height = 32
        };
        btnRandomize.FlatAppearance.BorderSize = 0;
        btnRandomize.Click += (s, e) => GenerateRandomNodes();

        layout.Controls.Add(lblNodes, 0, 0);
        layout.SetColumnSpan(lblNodes, 2);
        layout.Controls.Add(numNodes, 0, 1);
        layout.SetColumnSpan(numNodes, 2);
        layout.Controls.Add(btnRandomize, 0, 2);
        layout.SetColumnSpan(btnRandomize, 2);

        group.Controls.Add(layout);
        return group;
    }

    private GroupBox CreateAlgorithmSelectionGroup()
    {
        var group = new GroupBox
        {
            Text = "Algorithm Selection",
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Padding = new Padding(10),
            Margin = new Padding(0, 0, 0, 10)
        };

        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            RowCount = 1,
            ColumnCount = 1
        };

        mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var checkboxPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(2)
        };

        var sortedAlgorithms = algorithms.Keys.OrderBy(k => k).ToList();
        foreach (var algoName in sortedAlgorithms)
        {
            var checkbox = new CheckBox
            {
                Text = "",
                AutoSize = true,
                Checked = settings.EnabledAlgorithms.GetValueOrDefault(algoName, false),
                Margin = new Padding(3, 3, 5, 2),
                Width = 16
            };

            var link = new LinkLabel
            {
                Text = algoName,
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5f),
                Margin = new Padding(0, 4, 3, 2),
                Cursor = Cursors.Hand,
                LinkColor = algorithms[algoName].Color,
                ActiveLinkColor = algorithms[algoName].Color,
                VisitedLinkColor = algorithms[algoName].Color,
                LinkBehavior = LinkBehavior.HoverUnderline
            };

            var rowPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 1, 0, 0)
            };
            rowPanel.Controls.Add(checkbox);
            rowPanel.Controls.Add(link);

            algorithmCheckboxes[algoName] = checkbox;

            var attributePanel = CreateAttributeEditorPanel(algoName);
            attributePanel.Visible = checkbox.Checked;
            attributePanel.Margin = new Padding(5, 0, 0, 4);

            checkbox.CheckedChanged += (s, e) =>
            {
                settings.EnabledAlgorithms[algoName] = checkbox.Checked;
                attributePanel.Visible = checkbox.Checked;
            };

            link.Click += (s, e) =>
            {
                var wikipediaUrl = AlgorithmMetadata.GetWikipediaUrl(algoName);
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = wikipediaUrl,
                        UseShellExecute = true
                    });
                }
                catch
                {
                    MessageBox.Show($"Could not open browser. URL: {wikipediaUrl}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            var tooltip = new ToolTip();
            tooltip.SetToolTip(link, "Click to view on Wikipedia");

            checkboxPanel.Controls.Add(rowPanel);
            checkboxPanel.Controls.Add(attributePanel);
        }

        mainLayout.Controls.Add(checkboxPanel, 0, 0);

        group.Controls.Add(mainLayout);
        return group;
    }

    private Panel CreateAttributeEditorPanel(string algorithmName)
    {
        var settingInfos = AlgorithmMetadata.GetAlgorithmSettings(algorithmName);

        if (settingInfos.Count == 0)
        {
            var noParamsPanel = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0, 2, 5, 2)
            };
            noParamsPanel.Controls.Add(new Label
            {
                Text = "No configurable parameters",
                AutoSize = true,
                Font = new Font("Segoe UI", 8f, FontStyle.Italic),
                ForeColor = Color.Gray
            });
            return noParamsPanel;
        }

        var layout = new TableLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            Padding = new Padding(0, 2, 5, 2)
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95));

        int row = 0;
        foreach (var settingInfo in settingInfos)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var label = new Label
            {
                Text = settingInfo.DisplayName + ":",
                AutoSize = true,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 8f),
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(2, 3, 2, 2)
            };

            var propertyInfo = typeof(AlgorithmSettings).GetProperty(settingInfo.PropertyName);
            if (propertyInfo == null) continue;

            var numericValue = Convert.ToDecimal(propertyInfo.GetValue(settings));

            var numeric = new NumericUpDown
            {
                Minimum = settingInfo.Min,
                Maximum = settingInfo.Max,
                Value = numericValue,
                Increment = settingInfo.Increment,
                DecimalPlaces = settingInfo.IsDouble ? (settingInfo.Increment < 0.01m ? 3 : 2) : 0,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 8f),
                Margin = new Padding(2, 2, 2, 2)
            };

            numeric.ValueChanged += (s, e) =>
            {
                if (settingInfo.IsDouble)
                    propertyInfo.SetValue(settings, (double)numeric.Value);
                else
                    propertyInfo.SetValue(settings, (int)numeric.Value);
            };

            numeric.TextChanged += (s, e) =>
            {
                if (decimal.TryParse(numeric.Text, out var val) && val > numeric.Maximum)
                    numeric.Value = numeric.Maximum;
            };

            layout.Controls.Add(label, 0, row);
            layout.Controls.Add(numeric, 1, row);
            row++;
        }

        return layout;
    }

    private GroupBox CreateQuickSelectGroup()
    {
        var group = new GroupBox
        {
            Text = "Quick Select",
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Padding = new Padding(10),
            Margin = new Padding(0, 0, 0, 10)
        };

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(2)
        };

        group.SizeChanged += (s, e) =>
        {
            var availableWidth = Math.Max(0, group.ClientSize.Width - group.Padding.Horizontal - 6);
            if (availableWidth > 0)
            {
                flow.MaximumSize = new Size(availableWidth, 0);
            }
        };

        foreach (var groupInfo in AlgorithmMetadata.GetAlgorithmGroups())
        {
            var btn = new Button
            {
                Text = groupInfo.Name,
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8f),
                Margin = new Padding(2),
                Padding = new Padding(6, 3, 6, 3),
                BackColor = Color.FromArgb(225, 225, 225),
                ForeColor = Color.FromArgb(50, 50, 50),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(180, 180, 180);
            btn.FlatAppearance.BorderSize = 1;

            btn.Click += (s, e) =>
            {
                var selected = new HashSet<string>(groupInfo.Algorithms);
                foreach (var (name, cb) in algorithmCheckboxes)
                    cb.Checked = selected.Contains(name);
            };

            flow.Controls.Add(btn);
        }

        group.Controls.Add(flow);
        return group;
    }

    private GroupBox CreateActionsGroup()
    {
        var group = new GroupBox
        {
            Text = "Actions",
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Padding = new Padding(10),
            Margin = new Padding(0, 0, 0, 15)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            RowCount = 2,
            ColumnCount = 1,
            Padding = new Padding(5)
        };

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

        var btnSolveAll = new Button
        {
            Text = "Solve Selected",
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(0, 120, 215),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10f, FontStyle.Bold),
            Margin = new Padding(0, 0, 0, 5)
        };
        btnSolveAll.FlatAppearance.BorderSize = 0;
        btnSolveAll.Click += BtnSolveAll_Click;

        var btnClear = new Button
        {
            Text = "Clear Results",
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(244, 67, 54),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
        btnClear.FlatAppearance.BorderSize = 0;
        btnClear.Click += (s, e) => ClearResults();

        layout.Controls.Add(btnSolveAll, 0, 0);
        layout.Controls.Add(btnClear, 0, 1);

        group.Controls.Add(layout);
        return group;
    }

    private Panel CreateContentArea()
    {
        var container = new Panel { Dock = DockStyle.Fill };

        tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9)
        };

        var tabEditor = new TabPage("Editor");

        var editorLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1
        };

        editorLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        editorLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));

        comparisonCanvas = new ComparisonCanvas { Dock = DockStyle.Fill };
        comparisonCanvas.MouseClick += Canvas_MouseClick;
        comparisonCanvas.NodesChanged += Canvas_NodesChanged;

        var hintPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(255, 255, 240),
            Padding = new Padding(10)
        };

        var lblEditorHint = new Label
        {
            Text = "Left-click drag to move nodes • Left-click to add • Right-click to remove",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9, FontStyle.Italic),
            ForeColor = Color.FromArgb(100, 100, 100),
            TextAlign = ContentAlignment.MiddleCenter
        };

        hintPanel.Controls.Add(lblEditorHint);

        editorLayout.Controls.Add(comparisonCanvas, 0, 0);
        editorLayout.Controls.Add(hintPanel, 0, 1);

        tabEditor.Controls.Add(editorLayout);
        tabControl.TabPages.Add(tabEditor);

        var tabComparison = new TabPage("Algorithm Comparison");
        individualAlgorithmsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Color.FromArgb(250, 250, 250),
            Padding = new Padding(10)
        };

        foreach (var (name, algorithm) in algorithms)
        {
            var panel = new AlgorithmVisualizationPanel(name, algorithm.Color);
            panel.Click += (s, e) => ShowFullscreenAlgorithm(name);
            panel.Cursor = Cursors.Hand;

            var tooltip = new ToolTip();
            tooltip.SetToolTip(panel, "Click to view fullscreen");

            algorithmPanels[name] = panel;
            individualAlgorithmsPanel.Controls.Add(panel);
        }

        tabComparison.Controls.Add(individualAlgorithmsPanel);
        tabControl.TabPages.Add(tabComparison);

        var tabStatistics = new TabPage("Statistics & Analysis");
        statisticsPanel = new StatisticsPanel { Dock = DockStyle.Fill };
        tabStatistics.Controls.Add(statisticsPanel);
        tabControl.TabPages.Add(tabStatistics);

        container.Controls.Add(tabControl);
        return container;
    }

    private void UpdateComparisonView()
    {
        if (results.Count == 0) return;

        var resultsWithColors = results.ToDictionary(
            r => r.Key,
            r => (r.Value, algorithms[r.Key].Color)
        );
        comparisonCanvas.SetResults(resultsWithColors);
    }

    private void RefreshAllVisuals()
    {
        comparisonCanvas.SetNodes(nodes);

        foreach (var panel in algorithmPanels.Values)
        {
            panel.Clear();
        }

        tabControl.Invalidate(true);
    }

    private void ClearResults()
    {
        cancellationTokenSource?.Cancel();

        results.Clear();
        comparisonCanvas.Clear();

        if (fullscreenPanel != null)
        {
            ExitFullscreen();
        }

        foreach (var panel in algorithmPanels.Values)
        {
            panel.Clear();
            panel.Visible = true;
        }

        resultsGrid.UpdateResults(results);
        statisticsPanel.UpdateStatistics(results, nodes.Count);
    }

    private void GenerateRandomNodes()
    {
        cancellationTokenSource?.Cancel();

        nodes.Clear();
        int count = (int)numNodes.Value;

        int maxWidth = comparisonCanvas?.Width ?? 1200;
        int maxHeight = comparisonCanvas?.Height ?? 700;

        int margin = 50;
        int usableWidth = Math.Max(200, maxWidth - 2 * margin);
        int usableHeight = Math.Max(200, maxHeight - 2 * margin);

        for (int i = 0; i < count; i++)
        {
            nodes.Add(new PointF(
                random.Next(margin, margin + usableWidth),
                random.Next(margin, margin + usableHeight)
            ));
        }

        ClearResults();
        RefreshAllVisuals();
    }

    private void Canvas_MouseClick(object? sender, MouseEventArgs e)
    {
        if (sender is not ComparisonCanvas canvas) return;

        if (results.Count > 0)
        {
            MessageBox.Show("Cannot modify nodes after solving. Please clear results first.",
                "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (e.Button == MouseButtons.Left)
        {
            //Check if we clicked on an existing node (would be dragged, not added)
            bool clickedOnNode = false;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (Math.Abs(nodes[i].X - e.X) < 10 && Math.Abs(nodes[i].Y - e.Y) < 10)
                {
                    clickedOnNode = true;
                    break;
                }
            }

            //Only add new node if not clicking on existing node
            if (!clickedOnNode)
            {
                if (nodes.Count >= numNodes.Maximum)
                {
                    MessageBox.Show($"Maximum number of nodes ({numNodes.Maximum}) reached.",
                        "Maximum Nodes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                nodes.Add(new PointF(e.X, e.Y));
                numNodes.Value = nodes.Count;
                RefreshAllVisuals();
            }
        }
        else if (e.Button == MouseButtons.Right)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                if (Math.Abs(nodes[i].X - e.X) < 10 && Math.Abs(nodes[i].Y - e.Y) < 10)
                {
                    nodes.RemoveAt(i);
                    numNodes.Value = nodes.Count;
                    RefreshAllVisuals();
                    break;
                }
            }
        }
    }

    private void Canvas_NodesChanged(object? sender, EventArgs e)
    {
        if (sender is ComparisonCanvas canvas)
        {
            var updatedNodes = canvas.GetNodes();
            nodes.Clear();
            nodes.AddRange(updatedNodes);
        }
    }

    private void ShowFullscreenAlgorithm(string algorithmName)
    {
        if (!results.ContainsKey(algorithmName)) return;

        fullscreenAlgorithmName = algorithmName;
        var result = results[algorithmName];
        var color = algorithms[algorithmName].Color;

        fullscreenPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1
        };

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var headerPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = color,
            Padding = new Padding(15)
        };

        var headerLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight
        };

        var btnBack = new Button
        {
            Text = "← Back",
            Width = 80,
            Height = 35,
            BackColor = Color.FromArgb(255, 255, 255, 255),
            ForeColor = color,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Margin = new Padding(0, 2, 15, 0),
            Cursor = Cursors.Hand
        };
        btnBack.FlatAppearance.BorderSize = 0;
        btnBack.Click += (s, e) => ExitFullscreen();

        var lblAlgoName = new Label
        {
            Text = algorithmName,
            AutoSize = true,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.White,
            Margin = new Padding(0, 6, 20, 0)
        };

        var lblDistance = new Label
        {
            Text = $"Distance: {result.Distance:F2}",
            AutoSize = true,
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Margin = new Padding(0, 9, 20, 0)
        };

        var lblTime = new Label
        {
            Text = $"Time: {result.TimeMs}ms",
            AutoSize = true,
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.White,
            Margin = new Padding(0, 9, 0, 0)
        };

        headerLayout.Controls.AddRange([btnBack, lblAlgoName, lblDistance, lblTime]);
        headerPanel.Controls.Add(headerLayout);

        var fullscreenCanvas = new FullscreenAlgorithmCanvas(algorithmName, color);
        fullscreenCanvas.SetResult(result, nodes, false);

        layout.Controls.Add(headerPanel, 0, 0);
        layout.Controls.Add(fullscreenCanvas, 0, 1);
        fullscreenPanel.Controls.Add(layout);

        tabControl.TabPages[1].Controls.Clear();
        tabControl.TabPages[1].Controls.Add(fullscreenPanel);
    }

    private void ExitFullscreen()
    {
        if (fullscreenPanel == null) return;

        tabControl.TabPages[1].Controls.Clear();
        tabControl.TabPages[1].Controls.Add(individualAlgorithmsPanel);
        fullscreenPanel = null;
        fullscreenAlgorithmName = null;
    }

    private Panel CreateResultsPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(10)
        };

        resultsGrid = new ResultsGrid { Dock = DockStyle.Fill };
        panel.Controls.Add(resultsGrid);

        return panel;
    }

    private async void BtnSolveAll_Click(object? sender, EventArgs e)
    {
        if (sender is Button btn) btn.Enabled = false;
        try
        {
            if (nodes.Count < 3)
            {
                MessageBox.Show("Please add at least 3 nodes.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var enabledAlgorithms = settings.EnabledAlgorithms
                .Where(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .ToList();

            if (enabledAlgorithms.Count == 0)
            {
                MessageBox.Show("Please select at least one algorithm.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //Check for long-running algorithms
            var warnings = AlgorithmMetadata.CheckForLongRunningAlgorithms(enabledAlgorithms, nodes.Count);
            if (warnings.Count > 0)
            {
                var warningMessage = "Performance Warning!\n\n" +
                    string.Join("\n\n", warnings) +
                    "\n\nDo you want to proceed anyway?";

                var result = MessageBox.Show(warningMessage, "Long Runtime Expected",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                    return;
            }

            ClearResults();

            if (fullscreenPanel != null)
            {
                ExitFullscreen();
            }

            foreach (var panel in algorithmPanels.Values)
            {
                panel.Visible = false;
            }

            foreach (var algoName in enabledAlgorithms)
            {
                if (algorithmPanels.TryGetValue(algoName, out var panel))
                {
                    panel.Visible = true;
                }
            }

            tabControl.SelectedIndex = 1;

            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // Each algorithm gets its own dedicated OS thread via SolveAsync (LongRunning).
            // The Select starts all of them concurrently; WhenAll waits for every one to finish.
            var tasks = enabledAlgorithms
                .Where(name => algorithms.ContainsKey(name))
                .Select(name => RunAlgorithmAsync(algorithms[name], name, cancellationToken))
                .ToList();

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (OperationCanceledException) { }
        }
        finally
        {
            if (sender is Button btn2) btn2.Enabled = true;
        }
    }

    private async Task RunAlgorithmAsync(TSPAlgorithm algorithm, string algoName, CancellationToken cancellationToken)
    {
        // Setup runs on the UI thread before the dedicated algorithm thread is created,
        // satisfying the happens-before requirement for all fields Solve() will read.
        algorithm.SetNodes(nodes);
        ConfigureAlgorithmSettings(algorithm);

        var sw = System.Diagnostics.Stopwatch.StartNew();

        // Called from the algorithm's background thread; Invoke marshals back to the UI thread.
        algorithm.OnProgressUpdate = path =>
        {
            if (cancellationToken.IsCancellationRequested) return;
            var distance = PathCalculator.CalculateDistance(path, nodes);
            var intermediateResult = new AlgorithmResult(path, distance, sw.ElapsedMilliseconds, "Running...");
            try
            {
                Invoke(() =>
                {
                    if (cancellationToken.IsCancellationRequested) return;
                    results[algoName] = intermediateResult;
                    if (algorithmPanels.TryGetValue(algoName, out var panel))
                        panel.SetResult(intermediateResult, nodes, false);
                    resultsGrid.UpdateResults(results);
                });
            }
            catch (ObjectDisposedException) { }
        };

        try
        {
            // SolveAsync runs Solve() on a dedicated OS thread (LongRunning).
            var finalPath = await algorithm.SolveAsync(cancellationToken);
            sw.Stop();
            algorithm.OnProgressUpdate = null;

            if (cancellationToken.IsCancellationRequested) return;

            var finalDistance = PathCalculator.CalculateDistance(finalPath, nodes);
            var finalResult = new AlgorithmResult(finalPath, finalDistance, sw.ElapsedMilliseconds);

            results[algoName] = finalResult;
            if (algorithmPanels.TryGetValue(algoName, out var panel))
                panel.SetResult(finalResult, nodes, false);
            resultsGrid.UpdateResults(results);
            statisticsPanel.UpdateStatistics(results, nodes.Count);
        }
        catch (OperationCanceledException)
        {
            sw.Stop();
            algorithm.OnProgressUpdate = null;
        }
    }

    private void ConfigureAlgorithmSettings(TSPAlgorithm algorithm)
    {
        if (algorithm is TwoOptAlgorithm twoOpt)
            twoOpt.MaxIterations = settings.TwoOptMaxIterations;
        else if (algorithm is ThreeOptAlgorithm threeOpt)
            threeOpt.MaxIterations = settings.ThreeOptMaxIterations;
        else if (algorithm is SimulatedAnnealingAlgorithm sa)
            sa.InitialTemperature = settings.SATemperature;
        else if (algorithm is GeneticAlgorithm ga)
        {
            ga.PopulationSize = settings.GAPopulationSize;
            ga.Generations = settings.GAGenerations;
        }
        else if (algorithm is RandomAlgorithm random)
            random.Iterations = settings.RandomIterations;
        else if (algorithm is AntColonyAlgorithm aco)
            aco.Ants = settings.ACOAnts;
        else if (algorithm is LinKernighanAlgorithm lk)
            lk.MaxIterations = settings.LKMaxIterations;
        else if (algorithm is TabuSearchAlgorithm tabu)
        {
            tabu.MaxIterations = settings.TabuMaxIterations;
            tabu.TabuTenure = settings.TabuTenure;
        }
        else if (algorithm is ParticleSwarmAlgorithm ps)
        {
            ps.Particles = settings.PSParticles;
            ps.Iterations = settings.PSIterations;
        }
    }
}