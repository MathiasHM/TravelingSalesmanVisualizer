using TravelingSalesman.Models;

namespace TravelingSalesman.UI;

public class ResultsGrid : DataGridView
{
    public ResultsGrid()
    {
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        AllowUserToAddRows = false;
        AllowUserToDeleteRows = false;
        ReadOnly = true;
        SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        BackgroundColor = Color.White;
        Font = new Font("Consolas", 9);
        RowHeadersVisible = false;

        Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Algorithm",
            Name = "Algorithm",
            FillWeight = 30
        });
        Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Distance",
            Name = "Distance",
            FillWeight = 15
        });
        Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Time (ms)",
            Name = "Time",
            FillWeight = 15
        });
        Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Status",
            Name = "Status",
            FillWeight = 15
        });
        Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Quality",
            Name = "Quality",
            FillWeight = 15
        });
    }

    public void UpdateResults(Dictionary<string, AlgorithmResult> results)
    {
        Rows.Clear();

        if (results.Count == 0) return;

        var bestDistance = results.Min(r => r.Value.Distance);
        var worstDistance = results.Max(r => r.Value.Distance);

        foreach (var (name, result) in results.OrderBy(r => r.Value.Distance))
        {
            double quality = worstDistance > 0
                ? (1 - (result.Distance - bestDistance) / (worstDistance - bestDistance)) * 100
                : 100;

            var row = new DataGridViewRow();
            row.CreateCells(this);
            row.Cells[0].Value = name;
            row.Cells[1].Value = $"{result.Distance:F2}";
            row.Cells[2].Value = result.TimeMs;
            row.Cells[3].Value = result.Status;
            row.Cells[4].Value = $"{quality:F1}%";

            if (result.Distance == bestDistance)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(200, 255, 200);
                row.DefaultCellStyle.Font = new Font(Font, FontStyle.Bold);
            }
            else if (result.Distance == worstDistance && results.Count > 1)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
            }

            Rows.Add(row);
        }
    }
}
