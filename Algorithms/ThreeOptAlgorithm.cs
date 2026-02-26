namespace TravelingSalesman.Algorithms;

public class ThreeOptAlgorithm : TSPAlgorithm
{
    public int MaxIterations { get; set; } = 50;

    public ThreeOptAlgorithm()
        : base("3-Opt", Color.FromArgb(255, 87, 34))
    {
    }

    public override List<int> Solve()
    {
        if (Nodes.Count == 0) return [];
        if (Nodes.Count < 4) return Enumerable.Range(0, Nodes.Count).ToList();

        var twoOpt = new TwoOptAlgorithm();
        twoOpt.SetNodes(Nodes);
        twoOpt.SetCancellationToken(CancellationToken);
        var path = twoOpt.Solve();

        ReportProgress(path);

        bool improved = true;
        int iterations = 0;
        int reportInterval = Math.Max(1, MaxIterations / 15);

        try
        {
            while (improved && iterations < MaxIterations)
            {
                CancellationToken.ThrowIfCancellationRequested();

                improved = false;
                int n = path.Count;

                //Indices i < j < k each mark the start of one cut edge.
                for (int i = 0; i < n - 2; i++)
                {
                    CancellationToken.ThrowIfCancellationRequested(); //To prevent hanging on large amount of nodes
                    for (int j = i + 2; j < n - 1; j++)
                    {
                        for (int k = j + 2; k < n; k++)
                        {
                            int a = path[i], b = path[i + 1];
                            int c = path[j], d = path[j + 1];
                            int e = path[k], f = path[(k + 1) % n];

                            /* d0 = cost of the three edges being cut.
                            Because every other edge in the tour is unaffected by any
                            reconnection, the tour length change equals (dX - d0) for
                            whichever alternative edge set dX is chosen.*/
                            double d0 = Distance(Nodes[a], Nodes[b]) +
                                        Distance(Nodes[c], Nodes[d]) +
                                        Distance(Nodes[e], Nodes[f]);

                            /* 2-opt cases
                            Segments stay in their original order; one or both are reversed
                            in place. These are equivalent to single or paired 2-opt moves
                            and will be no-ops when starting from a 2-opt-optimal tour, but
                            they can become relevant again after a true 3-opt move is applied.*/
                            double d1 = Distance(Nodes[a], Nodes[c]) + Distance(Nodes[b], Nodes[d]) + Distance(Nodes[e], Nodes[f]);
                            double d2 = Distance(Nodes[a], Nodes[b]) + Distance(Nodes[c], Nodes[e]) + Distance(Nodes[d], Nodes[f]);
                            double d3 = Distance(Nodes[a], Nodes[c]) + Distance(Nodes[b], Nodes[e]) + Distance(Nodes[d], Nodes[f]);

                            // 3-opt cases
                            double d4 = Distance(Nodes[a], Nodes[d]) + Distance(Nodes[e], Nodes[b]) + Distance(Nodes[c], Nodes[f]);
                            double d5 = Distance(Nodes[a], Nodes[d]) + Distance(Nodes[e], Nodes[c]) + Distance(Nodes[b], Nodes[f]);
                            double d6 = Distance(Nodes[a], Nodes[e]) + Distance(Nodes[d], Nodes[b]) + Distance(Nodes[c], Nodes[f]);

                            double best = d0; //Initial cost
                            int bestCase = 0;
                            if (d1 < best) { best = d1; bestCase = 1; }
                            if (d2 < best) { best = d2; bestCase = 2; }
                            if (d3 < best) { best = d3; bestCase = 3; }
                            if (d4 < best) { best = d4; bestCase = 4; }
                            if (d5 < best) { best = d5; bestCase = 5; }
                            if (d6 < best) { best = d6; bestCase = 6; }

                            if (bestCase == 0)
                                continue;

                            /* Reconstruct the path for the winning case.
                            Cases 1-3 only reverse segments in place, so a copy
                            followed by List.Reverse() suffices.
                            Cases 4-6 reorder segments, requiring explicit concatenation.*/
                            List<int> newPath;
                            switch (bestCase)
                            {
                                case 1:
                                    newPath = path.ToList();
                                    newPath.Reverse(i + 1, j - i);
                                    break;
                                case 2:
                                    newPath = path.ToList();
                                    newPath.Reverse(j + 1, k - j);
                                    break;
                                case 3:
                                    newPath = path.ToList();
                                    newPath.Reverse(i + 1, j - i);
                                    newPath.Reverse(j + 1, k - j);
                                    break;
                                case 4:
                                    newPath = new List<int>(n);
                                    for (int x = 0; x <= i; x++) newPath.Add(path[x]);
                                    for (int x = j + 1; x <= k; x++) newPath.Add(path[x]);
                                    for (int x = i + 1; x <= j; x++) newPath.Add(path[x]);
                                    for (int x = k + 1; x < n; x++) newPath.Add(path[x]);
                                    break;
                                case 5:
                                    newPath = new List<int>(n);
                                    for (int x = 0; x <= i; x++) newPath.Add(path[x]);
                                    for (int x = j + 1; x <= k; x++) newPath.Add(path[x]);
                                    for (int x = j; x >= i + 1; x--) newPath.Add(path[x]);
                                    for (int x = k + 1; x < n; x++) newPath.Add(path[x]);
                                    break;
                                default:
                                    newPath = new List<int>(n);
                                    for (int x = 0; x <= i; x++) newPath.Add(path[x]);
                                    for (int x = k; x >= j + 1; x--) newPath.Add(path[x]);
                                    for (int x = i + 1; x <= j; x++) newPath.Add(path[x]);
                                    for (int x = k + 1; x < n; x++) newPath.Add(path[x]);
                                    break;
                            }

                            path = newPath;
                            improved = true;
                            goto NextIteration;
                        }
                    }
                }

            NextIteration:
                iterations++;

                if (iterations % reportInterval == 0 || iterations == MaxIterations)
                {
                    ReportProgress(path);
                }
            }
        }
        catch (OperationCanceledException) { }

        return path;
    }
}
