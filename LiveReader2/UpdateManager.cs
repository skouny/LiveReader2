using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

public class UpdateManager
{
    const int MAX_QUEUE_TIME = 2000;
    const int MAX_QUEUE_SIZE = 50;
    public int Modified { get; set; } = 0;
    public List<NameValueCollection> UploadQueue { get; set; } = new List<NameValueCollection>();
    public int TimeCounter { get; set; } = Environment.TickCount;
    public List<Task<NameValueCollection>> Tasks { get; set; } = new List<Task<NameValueCollection>>();
    public void PartialCollectAndUpload(SportName sport, ActionWrite WriteLog)
    {
        foreach (var task in this.Tasks.ToArray())
        {
            if (task.IsCompleted && task.Result != null)
            {
                this.Modified += 1;
                this.UploadQueue.Add(task.Result);
                this.Tasks.Remove(task);
            }
        }
        if (this.UploadQueue.Count > MAX_QUEUE_SIZE || Environment.TickCount - this.TimeCounter > MAX_QUEUE_TIME)
        {
            UploadMatchesLive(sport, this.UploadQueue, WriteLog);
            this.UploadQueue.Clear();
            this.TimeCounter = Environment.TickCount;
        }
    }
    public void FullCollectAndUpload(SportName sport, ActionWrite WriteLog)
    {
        Task.WaitAll(this.Tasks.ToArray());
        foreach (var task in this.Tasks)
        {
            if (task.Result != null)
            {
                this.Modified += 1;
                this.UploadQueue.Add(task.Result);
            }
        }
        UploadMatchesLive(sport, this.UploadQueue, WriteLog);
    }
    #region SQL
    public static void UploadMatchesLive(SportName sport, NameValueCollection record, ActionWrite WriteLog)
    {
        var records = new List<NameValueCollection> { record };
        UploadMatchesLive(sport, records, WriteLog);
    }
    public static void UploadMatchesLive(SportName sport, List<NameValueCollection> records, ActionWrite WriteLog)
    {
        if (records == null || records.Count == 0) return;
        try
        {
            var begin = Environment.TickCount;
            using (var connection = new MySqlConnection(MyURLs.CONNECTION_STRING_LIVEREADER2))
            {
                connection.Open();
                UploadMatchesLive(sport, connection, records);
                connection.Close();
            }
            WriteLog?.Invoke("UploadMatchesLive: OK [{0:#,##0} ms]", Environment.TickCount - begin);
        }
        catch (Exception ex)
        {
            WriteLog?.Invoke("UploadMatchesLive: {0}", ex.Message);
        }
    }
    public static void UploadMatchesLive(SportName sport, MySqlConnection connection, List<NameValueCollection> records)
    {
        if (records == null || records.Count == 0) return;
        string[] fields = new string[] { };
        if (sport == SportName.Football)
        {
            fields = new string[] { "Id", "Source", "SourceId", "WebId", "StartTime", "Champ", "HomeTeam", "AwayTeam", "ScoreHT", "Score"
                , "HomeScored", "AwayScored", "Status", "StatusId", "Minute", "Modified"
                , "YellowCards", "RedCards", "CornerKicks", "Shots", "ShotsOnGoal", "Fouls"
                , "BallPossession", "Saves", "Offsides", "KickOff", "HomeEvents", "AwayEvents"
            };
        }
        else if (sport == SportName.Basket)
        {
            fields = new string[] { "Id", "Source", "SourceId", "WebId", "StartTime", "Champ", "HomeTeam", "AwayTeam", "ScoreHT", "Score"
                , "HomeScored", "AwayScored", "Status", "StatusId", "Minute", "Modified"
                , "ScoreQ1", "ScoreQ2", "ScoreQ3", "ScoreQ4", "StandingPoints"
            };
        }
        else
        {
            return;
        }
        // 
        string fields_insert = "";
        string fields_update = "";
        for (int i = 0; i < fields.Count(); i++)
        {
            var field = fields[i];
            if (i == 0)
            {
                fields_insert = $"`{field}`";
                fields_update = $"`{field}`=VALUES(`{field}`)";
            }
            else
            {
                fields_insert += $",`{field}`";
                fields_update += $",`{field}`=VALUES(`{field}`)";
            }
        }
        // Execute
        using (var command = connection.CreateCommand())
        using (var writer = new StringWriter())
        {
            // create value groups and add parameters
            string value_groups = "";
            for (int x = 0; x < records.Count(); x++)
            {
                var record = records[x];
                string value_group = "";
                for (int y = 0; y < fields.Count(); y++)
                {
                    var field = fields[y];
                    var parameterName = $"@{field}{x}";
                    command.Parameters.AddWithValue(parameterName, record[field]);
                    if (y == 0)
                    {
                        value_group = $"{parameterName}";
                    }
                    else
                    {
                        value_group += $",{parameterName}";
                    }
                }
                if (x == 0)
                {
                    value_groups = $"({value_group})";
                }
                else
                {
                    value_groups += $",({value_group})";
                }
            }
            if (sport == SportName.Football)
            {
                writer.WriteLine("INSERT INTO `football_live_mix` ({0})", fields_insert);
            }
            else if (sport == SportName.Basket)
            {
                writer.WriteLine("INSERT INTO `basket_live_mix` ({0})", fields_insert);
            }
            writer.WriteLine("VALUES {0}", value_groups);
            writer.WriteLine("ON DUPLICATE KEY UPDATE {0};", fields_update);
            command.CommandText = writer.ToString();
            command.ExecuteNonQuery();
        }
    }
    #endregion
}
