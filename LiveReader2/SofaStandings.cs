using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace LiveReader2
{
    public partial class SofaStandings : Form
    {
        private const int MAX_RETRIES = 5;
        private ChromiumWebBrowser Browser { get; set; } = null;
        public SofaStandings()
        {
            InitializeComponent();
        }
        private void FormSofaStandings_Load(object sender, EventArgs e)
        {
            workerMain.RunWorkerAsync();
        }
        private void workerMain_DoWork(object sender, DoWorkEventArgs e)
        {
#if !DEBUG
            try {
#endif
            var leagues = WebDB.sofa_football_leagues; //.Where(x => x.Id == "171").ToList();
            this.WriteLog($"Found {leagues.Count} leagues");
            for (int i = 0; i < leagues.Count; i++)
            {
                var league = leagues[i];
                bool updated = false;
                for (int r = 0; r < MAX_RETRIES; r++)
                {
                    if (r > 0) this.WriteLog($"Reloading league {league.Id}, group {league.GroupIndex}... [{r}]");
                    else this.WriteLog($"Loading league {league.Id}, group {league.GroupIndex}...");
                    var htmlString = this.GetLeagueSource(league);
                    if (htmlString != null && htmlString.Length > 0)
                    {
                        var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                        htmlDoc.LoadHtml(htmlString);
                        if (UpdateLeagueInDB(htmlDoc, league, WriteLog))
                        {
                            WriteLog($"Updated {i + 1}/{leagues.Count}: {league.Url}");
                            StandingsUpdater.UpdateFeedUnattended(10, this.WriteLog); // Update the feed
                            updated = true;
                            break;
                        }
                    }
                }
                if (!updated)
                {
                    WriteLogCritical($"League {league.Id}, group {league.GroupIndex} failed! => {league.Url}");
                }
            }
            this.WriteLog("Database update done!");
            
#if !DEBUG
            } catch (Exception ex) { this.WriteLog($"Error => {ex.Message}"); }
#endif
        }
        private void workerMain_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        private void workerMain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        private string GetLeagueSource(SofaLeague league)
        {
            // create, load or reload
            if (BrowserIsNull) // create
            {
                // create the browser
                this.Invoke(new Action(() =>
                {
                    this.Browser = new ChromiumWebBrowser(league.Url);
                    this.Browser.Dock = DockStyle.Fill;
                    this.Browser.BrowserSettings.ImageLoading = CefState.Disabled;
                    this.splitContainerMain.Panel1.Controls.Clear();
                    this.splitContainerMain.Panel1.Controls.Add(this.Browser);
                }));
                // wait to be initialized
                while (!this.BrowserIsInitialized)
                {
                    System.Threading.Thread.Sleep(1000);
                }
            }
            else if (this.BrowserAddress != league.Url) // load
            {
                this.Invoke(new Action(() =>
                {
                    this.Browser.Load(league.Url);
                }));
            }
            else // reload
            {
                this.Invoke(new Action(() =>
                {
                    this.Browser.Reload();
                }));
            }
            // wait to finish
            while (this.BrowserIsLoading)
            {
                System.Threading.Thread.Sleep(1000);
            }
            // wait extra time for ajax
            System.Threading.Thread.Sleep(3000);
            // return
            return this.Browser.GetSourceAsync().Result;
        }
        #region Cross thread Browser control
        private bool BrowserIsNull { get { return (bool)this.Invoke(new Func<bool>(() => { return (this.Browser == null); })); } }
        private bool BrowserIsInitialized { get { return (bool)this.Invoke(new Func<bool>(() => { return this.Browser.IsBrowserInitialized; })); } }
        private string BrowserAddress { get { return (string)this.Invoke(new Func<string>(() => { return this.Browser.Address; })); } }
        private bool BrowserIsLoading { get { return (bool)this.Invoke(new Func<bool>(() => { return this.Browser.IsLoading; })); } }
        #endregion
        static bool UpdateLeagueInDB(HtmlAgilityPack.HtmlDocument htmlDoc, SofaLeague league, ActionWrite log)
        {
            try
            {
                var standings_content = htmlDoc.DocumentNode.Descendants("div").Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "standings js-standings");
                var standings_table = standings_content.ToArray()[league.ContentIndex].Descendants("div").Where(x => x.Attributes.Contains("class") && x.Attributes["class"].Value == "standings-table");
                var standing_overall = standings_table.ElementAt(0).Elements("div");//.First();
                var standing_home = standings_table.ElementAt(1).Elements("div");
                var standing_away = standings_table.ElementAt(2).Elements("div");
                var places_count = standing_overall.Count();
                var team_record = new Dictionary<string, NameValueCollection>();
                for (int i = 0; i < places_count; i++)
                {
                    #region Overall
                    var cells_overall = standing_overall.ElementAt(i).Elements("div").ToArray();
                    var team_name_overall = cells_overall[2].InnerText.Trim();
                    var team_place_overall = cells_overall[0].InnerText.Trim();
                    if (!team_record.ContainsKey(team_name_overall))
                    {
                        team_record[team_name_overall] = CreateTeamRecord(league, team_name_overall);
                    }
                    team_record[team_name_overall]["Id"] = string.Format("{0}:{1}#{2}", league.Id, league.GroupIndex, team_place_overall);  // ** Primary Key ** (overall only)
                    team_record[team_name_overall]["Place"] = team_place_overall;
                    var details_overall = cells_overall[3].Descendants("span").ToArray();
                    team_record[team_name_overall]["TotalGames"] = details_overall[0].InnerText.Trim();
                    team_record[team_name_overall]["TotalWins"] = details_overall[1].InnerText.Trim();
                    team_record[team_name_overall]["TotalDraws"] = details_overall[2].InnerText.Trim();
                    team_record[team_name_overall]["TotalLosts"] = details_overall[3].InnerText.Trim();
                    var goals_overall = details_overall[4].InnerText.Trim().Split(':');
                    var goalsFor_overall = int.Parse(goals_overall[0]);
                    var goalsAgn_overall = int.Parse(goals_overall[1]);
                    var goalsDif_overall = goalsFor_overall - goalsAgn_overall;
                    team_record[team_name_overall]["TotalGoalsFor"] = goalsFor_overall.ToString();
                    team_record[team_name_overall]["TotalGoalsAgainst"] = goalsAgn_overall.ToString();
                    team_record[team_name_overall]["TotalGoalDeference"] = goalsDif_overall.ToString();
                    team_record[team_name_overall]["TotalPoints"] = cells_overall[5].InnerText.Trim();
                    #endregion
                    #region Home
                    var cells_home = standing_home.ElementAt(i).Elements("div").ToArray();
                    var team_name_home = cells_home[2].InnerText.Trim();
                    var team_place_home = cells_home[0].InnerText.Trim();
                    if (!team_record.ContainsKey(team_name_home))
                    {
                        team_record[team_name_home] = CreateTeamRecord(league, team_name_home);
                    }
                    team_record[team_name_home]["PlaceHome"] = team_place_home;
                    var details_home = cells_home[3].Descendants("span").ToArray();
                    team_record[team_name_home]["HomeGames"] = details_home[0].InnerText.Trim();
                    team_record[team_name_home]["HomeWins"] = details_home[1].InnerText.Trim();
                    team_record[team_name_home]["HomeDraws"] = details_home[2].InnerText.Trim();
                    team_record[team_name_home]["HomeLosts"] = details_home[3].InnerText.Trim();
                    var goals_home = details_home[4].InnerText.Trim().Split(':');
                    var goalsFor_home = int.Parse(goals_home[0]);
                    var goalsAgn_home = int.Parse(goals_home[1]);
                    var goalsDif_home = goalsFor_home - goalsAgn_home;
                    team_record[team_name_home]["HomeGoalsFor"] = goalsFor_home.ToString();
                    team_record[team_name_home]["HomeGoalsAgainst"] = goalsAgn_home.ToString();
                    team_record[team_name_home]["HomeGoalDeference"] = goalsDif_home.ToString();
                    team_record[team_name_home]["HomePoints"] = cells_home[5].InnerText.Trim();
                    #endregion
                    #region Away
                    var cells_away = standing_away.ElementAt(i).Elements("div").ToArray();
                    var team_name_away = cells_away[2].InnerText.Trim();
                    var team_place_away = cells_away[0].InnerText.Trim();
                    if (!team_record.ContainsKey(team_name_away))
                    {
                        team_record[team_name_away] = CreateTeamRecord(league, team_name_away);
                    }
                    team_record[team_name_away]["PlaceAway"] = team_place_away;
                    var details_away = cells_away[3].Descendants("span").ToArray();
                    team_record[team_name_away]["AwayGames"] = details_away[0].InnerText.Trim();
                    team_record[team_name_away]["AwayWins"] = details_away[1].InnerText.Trim();
                    team_record[team_name_away]["AwayDraws"] = details_away[2].InnerText.Trim();
                    team_record[team_name_away]["AwayLosts"] = details_away[3].InnerText.Trim();
                    var goals_away = details_away[4].InnerText.Trim().Split(':');
                    var goalsFor_away = int.Parse(goals_away[0]);
                    var goalsAgn_away = int.Parse(goals_away[1]);
                    var goalsDif_away = goalsFor_away - goalsAgn_away;
                    team_record[team_name_away]["AwayGoalsFor"] = goalsFor_away.ToString();
                    team_record[team_name_away]["AwayGoalsAgainst"] = goalsAgn_away.ToString();
                    team_record[team_name_away]["AwayGoalDeference"] = goalsDif_away.ToString();
                    team_record[team_name_away]["AwayPoints"] = cells_away[5].InnerText.Trim();
                    #endregion
                }
                // Add or update each record in database
                foreach (var record in team_record.Values)
                {
                    string message = WebDB.sofa_football_standing_update(record);
                    if (message.Length > 0) log?.Invoke($"POST ERROR! UpdateLeague {league.Url}: {message}");
                }
                // if there are more places, remove them (from an other year)
                var cleanup_msg = WebDB.sofa_football_standing_cleanup(league.Id, league.GroupIndex, places_count);
            }
            catch { return false; }
            return true;
        }
        private static NameValueCollection CreateTeamRecord(SofaLeague league, string team_name)
        {
            var record = new NameValueCollection();
            record["ChampId"] = league.Id;
            record["GroupIndex"] = league.GroupIndex;
            record["TeamName"] = team_name;
            var latestMatches = SportsDB.GetLatestMatches(league.Id, team_name, 12);
            var formInfo = SportsDB.GetFormInfo(league.Id, team_name);
            var latestMatchesXml = latestMatches.GetXmlString;
            record["Matches"] = latestMatchesXml;
            //var parseMatches = SportsDB.MatchCollection.ReadXml(latestMatchesXml); // test
            if (formInfo != null)
            {
                record["FormInfo"] = formInfo;
            }
            else if (latestMatches.Matches.Count > 0)
            {
                var day = latestMatches.Matches[0].StartTime.ToString("yyyy-MM-dd");
                var code = latestMatches.Matches[0].WebId;
                record["FormInfo"] = $"{day}:{code}";
            }
            return record;
        }
        private void WriteLog(string format, params object[] args)
        {
            WriteLog(string.Format(format, args));
        }
        private void WriteLog(string message)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    listBoxLog.Items.Insert(0, $"{ DateTime.Now:yyyy-MM-dd HH:mm:ss}# {message}");
                    if (listBoxLog.Items.Count > 1000)
                    {
                        listBoxLog.Items.RemoveAt(1000);
                    }
                }));
            }
            catch { }
        }
        private void WriteLogCritical(string message)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    listBoxLogCritical.Items.Insert(0, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}# {message}");
                    if (listBoxLog.Items.Count > 1000)
                    {
                        listBoxLog.Items.RemoveAt(1000);
                    }
                }));
            }
            catch { }
        }
    }
}