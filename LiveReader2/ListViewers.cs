using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using play90;

public class ListViewDay
{
    public static void Init(ListView listView)
    {
        // General Settings
        listView.View = View.Details;
        listView.FullRowSelect = true;
        listView.MultiSelect = true;
        listView.GridLines = true;
        listView.ShowGroups = false;
        listView.HideSelection = false;
        listView.DoubleClick += (sender, e) =>
        {

        };
        // Add Columns
        listView.Columns.Clear();
        AddColumn(listView, "Code", 60);
        AddColumn(listView, "StartTime", 80);
        AddColumn(listView, "Champ", 80);
        AddColumn(listView, "HomeTeam", 200);
        AddColumn(listView, "AwayTeam", 200);
        AddColumn(listView, "ScoreHT", 80);
        AddColumn(listView, "Score", 80);
        AddColumn(listView, "Status", 100);
    }
    public static void Update(ListView listView, BetDayInfo day, bool hideCompleted, ActionWrite log = null)
    {
        // Refresh
        listView.BeginUpdate();
        try
        {
            listView.Items.Clear();
            // Auto Init
            if (listView.Columns.Count == 0) Init(listView);
            // Rows
            if (day != null && day.Matches != null)
            {
                foreach (var match in day.Matches.OrderBy(x => x.StartTimeValue))
                {
                    if (!hideCompleted || !MatchIsOpapCompleted(match))
                    {
                        AddRow(listView, match);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            log?.Invoke($"ListViewDay.Update: Error! => {ex.Message}");
        }
        finally
        {
            listView.EndUpdate();
        }
    }
    private static bool MatchIsOpapCompleted(BetMatchInfo match)
    {
        if (match.OpapStatus != null)
        {
            if (match.OpapStatus.Value == "completed") return true;
            //if (match.OpapStatus.Value == "blocked") return true;
            //if (match.OpapStatus.Value == "cancelled") return true;
        }
        return false;
    }
    private static void AddColumn(ListView listView, string text, int width, HorizontalAlignment align = HorizontalAlignment.Center)
    {
        var column = new ColumnHeader();
        column.Width = width;
        column.Text = text;
        column.TextAlign = align;
        listView.Columns.Add(column);
    }
    private static void AddRow(ListView listView, BetMatchInfo match)
    {
        var row = new ListViewItem();
        row.Tag = match;
        row.Text = $"{match.Code.Value}";
        row.SubItems.Add($"{match.StartTimeValue:HH:mm}");
        row.SubItems.Add($"{match.Champ.Value}");
        row.SubItems.Add($"{match.HomeTeam.Name}");
        row.SubItems.Add($"{match.AwayTeam.Name}");
        row.SubItems.Add($"{match.ScoreHT}");
        row.SubItems.Add($"{match.Score}");
        row.SubItems.Add($"{match.OpapStatus?.Value}");
        listView.Items.Add(row);
    }
}
public class ListViewForms
{
    public static void Init(ListView listView)
    {
        // General Settings
        listView.View = View.Details;
        listView.FullRowSelect = true;
        listView.MultiSelect = true;
        listView.GridLines = true;
        listView.ShowGroups = false;
        listView.HideSelection = false;
        listView.DoubleClick += (sender, e) =>
        {
            
        };
        // Add Columns
        listView.Columns.Clear();
        AddColumn(listView, "Code", 80);
        AddColumn(listView, "Length", 70);
        AddColumn(listView, "Modified", 150);
    }
    public static void Update(ListView listView, BetDayInfo day, List<ServerIO.File> existinglist)
    {
        // Refresh
        listView.BeginUpdate();
        listView.Items.Clear();
        // Auto Init
        if (listView.Columns.Count == 0) Init(listView);
        // Rows
        if (day != null && day.Matches != null)
        {
            foreach (var match in day.Matches)
            {
                var existing = existinglist.Where(x => x.Name.Contains($"{match.Code.Value}"));
                if (existing != null && existing.Count() > 0)
                {
                    var file = existing.First();
                    AddRow(listView, match, Color.LightGreen, file);
                }
                else
                {
                    AddRow(listView, match, Color.Tomato);
                }
            }
        }
        listView.EndUpdate();
    }
    private static void AddColumn(ListView listView, string text, int width, HorizontalAlignment align = HorizontalAlignment.Center)
    {
        var column = new ColumnHeader();
        column.Width = width;
        column.Text = text;
        column.TextAlign = align;
        listView.Columns.Add(column);
    }
    private static void AddRow(ListView listView, BetMatchInfo match, Color backColor, ServerIO.File file = null)
    {
        var row = new ListViewItem();
        row.Tag = match;
        row.BackColor = backColor;
        row.Text = $"{match.Code.Value}";
        if (file != null)
        {
            row.SubItems.Add($"{file.Length:#,##0}");
            row.SubItems.Add($"{file.Modified:yyyy-MM-dd HH:mm:ss}");
        }
        listView.Items.Add(row);
    }
}
public class ListViewBetMix
{
    public static void Init(ListView listView)
    {
        // General Settings
        listView.View = View.Details;
        listView.FullRowSelect = true;
        listView.MultiSelect = true;
        listView.GridLines = true;
        listView.ShowGroups = false;
        listView.HideSelection = false;
        listView.DoubleClick += (sender, e) =>
        {

        };
        // Add Columns
        listView.Columns.Clear();
        AddColumn(listView, "File", 80);
        AddColumn(listView, "Length", 70);
        AddColumn(listView, "Modified", 150);
    }
    public static void Update(ListView listView, BetDayInfo day, List<ServerIO.File> existinglist)
    {
        // Refresh
        listView.BeginUpdate();
        listView.Items.Clear();
        // Auto Init
        if (listView.Columns.Count == 0) Init(listView);
        // 
        if (day != null && day.Matches != null && day.Matches.Count > 0)
        {
            var existing = existinglist.Where(x => x.Name.Contains($"BetMix"));
            if (existing != null && existing.Count() > 0)
            {
                var file = existing.First();
                AddRow(listView, Color.LightGreen, file);
            }
            else
            {
                AddRow(listView, Color.Tomato);
            }
        }
        // 
        listView.EndUpdate();
    }
    private static void AddColumn(ListView listView, string text, int width, HorizontalAlignment align = HorizontalAlignment.Center)
    {
        var column = new ColumnHeader();
        column.Width = width;
        column.Text = text;
        column.TextAlign = align;
        listView.Columns.Add(column);
    }
    private static void AddRow(ListView listView, Color backColor, ServerIO.File file = null)
    {
        var row = new ListViewItem();
        row.BackColor = backColor;
        row.Text = $"BetMix.xml";
        if (file != null)
        {
            row.SubItems.Add($"{file.Length:#,##0}");
            row.SubItems.Add($"{file.Modified:yyyy-MM-dd HH:mm:ss}");
        }
        listView.Items.Add(row);
    }
}