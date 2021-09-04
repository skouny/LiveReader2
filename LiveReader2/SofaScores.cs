using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using CefSharp;
using CefSharp.WinForms;

namespace LiveReader2
{
    public partial class SofaScores : Form
    {
        #region Declarations
        private enum CurrentStep
        {
            NothingDone,
            DayLoading, DayLoaded,
            DayReading, DayReaded,
            MatchLoading, MatchLoaded,
            MatchReading, MatchReaded,
            AllDone
        }
        private class WebMatch
        {
            public SportsDB.Match Match { get; set; }
            public WebMatch()
            {
                this.Match = new SportsDB.Match();
                this.Match.Source = "SofaScore";
            }
            public void AddDetails(string csv, ActionWrite WriteWarning)
            {
                var lines = csv.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Length > 0)
                    {
                        var cells = line.Split('\t');
                        if (cells.Length > 1)
                        {
                            var name = cells[0];
                            switch (name)
                            {
                                case "HomeTeam": // WARNING!! HomeTeam may be reversed!! So you have to re-set it!!
                                    this.Match.HomeTeam = cells[1];
                                    break;
                                case "AwayTeam": // WARNING!! AwayTeam may be reversed!! So you have to re-set it!!
                                    this.Match.AwayTeam = cells[1];
                                    break;
                                case "HomeScoreFT": // WARNING!! HomeScoreFT may be reversed!! So you have to re-set it!!
                                    this.Match.HomeScoreFT = FixInt(cells[1]);
                                    break;
                                case "AwayScoreFT": // WARNING!! AwayScoreFT may be reversed!! So you have to re-set it!!
                                    this.Match.AwayScoreFT = FixInt(cells[1]);
                                    break;
                                case "HomeScoreHT": this.Match.HomeScoreHT = FixInt(cells[1]); break;
                                case "AwayScoreHT": this.Match.AwayScoreHT = FixInt(cells[1]); break;
                                case "Incident":
                                    var matchIncident = new SportsDB.MatchIncident();
                                    matchIncident.Side = FixTeamSide(cells[1], this.Url, WriteWarning);
                                    matchIncident.Name = cells[2];
                                    matchIncident.Minute = cells[3];
                                    matchIncident.Score = cells[4];
                                    matchIncident.Player = cells[5];
                                    matchIncident.Details = cells[6];
                                    this.Match.Incidents.Add(matchIncident);
                                    break;
                                case "Stat":
                                    var stat = cells[1];
                                    var home = cells[2];
                                    var away = cells[3];
                                    switch (stat)
                                    {
                                        case "Ball possession":
                                            this.Match.HomeBallPossession = FixInt(home);
                                            this.Match.AwayBallPossession = FixInt(away);
                                            break;
                                        case "Total shots":
                                            this.Match.HomeTotalShots = FixInt(home);
                                            this.Match.AwayTotalShots = FixInt(away);
                                            break;
                                        case "Shots on target":
                                            this.Match.HomeShotsOnTarget = FixInt(home);
                                            this.Match.AwayShotsOnTarget = FixInt(away);
                                            break;
                                        case "Shots off target":
                                            this.Match.HomeShotsOffTarget = FixInt(home);
                                            this.Match.AwayShotsOffTarget = FixInt(away);
                                            break;
                                        case "Blocked shots":
                                            this.Match.HomeBlockedShots = FixInt(home);
                                            this.Match.AwayBlockedShots = FixInt(away);
                                            break;
                                        case "Shots inside box":
                                            this.Match.HomeShotsInsideBox = FixInt(home);
                                            this.Match.AwayShotsInsideBox = FixInt(away);
                                            break;
                                        case "Shots outside box":
                                            this.Match.HomeShotsOutsideBox = FixInt(home);
                                            this.Match.AwayShotsOutsideBox = FixInt(away);
                                            break;
                                        case "Goalkeeper saves":
                                            this.Match.HomeGoalkeeperSaves = FixInt(home);
                                            this.Match.AwayGoalkeeperSaves = FixInt(away);
                                            break;
                                        case "Corner kicks":
                                            this.Match.HomeCornerKicks = FixInt(home);
                                            this.Match.AwayCornerKicks = FixInt(away);
                                            break;
                                        case "Passes":
                                            this.Match.HomePasses = FixInt(home);
                                            this.Match.AwayPasses = FixInt(away);
                                            break;
                                        case "Accurate passes":
                                            this.Match.HomeAccuratePasses = FixInt(home);
                                            this.Match.AwayAccuratePasses = FixInt(away);
                                            break;
                                        case "Fast breaks":
                                            this.Match.HomeFastBreaks = FixInt(home);
                                            this.Match.AwayFastBreaks = FixInt(away);
                                            break;
                                        case "Offsides":
                                            this.Match.HomeOffsides = FixInt(home);
                                            this.Match.AwayOffsides = FixInt(away);
                                            break;
                                        case "Fouls":
                                            this.Match.HomeFouls = FixInt(home);
                                            this.Match.AwayFouls = FixInt(away);
                                            break;
                                        case "Yellow cards":
                                            this.Match.HomeYellowCards = FixInt(home);
                                            this.Match.AwayYellowCards = FixInt(away);
                                            break;
                                        case "Red cards":
                                            this.Match.HomeRedCards = FixInt(home);
                                            this.Match.AwayRedCards = FixInt(away);
                                            break;
                                        case "Duels won":
                                            this.Match.HomeDuelsWon = FixInt(home);
                                            this.Match.AwayDuelsWon = FixInt(away);
                                            break;
                                        case "Aerials won":
                                            this.Match.HomeAerialsWon = FixInt(home);
                                            this.Match.AwayAerialsWon = FixInt(away);
                                            break;
                                        default: break;
                                    }
                                    break;
                                case "Votes":
                                    switch (cells[1])
                                    {
                                        case "1": this.Match.Votes1 = FixLong(cells[2]); break;
                                        case "X": this.Match.VotesX = FixLong(cells[2]); break;
                                        case "2": this.Match.Votes2 = FixLong(cells[2]); break;
                                    }
                                    break;
                                default:
                                    WriteWarning("WebMatch.AddDetails: Unknown name => {0}, url: {1}{2}"
                                        , name, Environment.NewLine, this.Url);
                                    break;
                            }
                        }
                    }
                }
            }
            public string Country { get; set; }
            public string Url { get; set; }
            public string Log { get { return $"[{this.Country}] {this.Match.Log}"; } }
            #region Fix Web Info
            public static SportsDB.Status FixStatus(string status, string matchUrl, ActionWrite WriteWarning)
            {
                switch (status)
                {
                    case "HT": return SportsDB.Status.HT;
                    case "FT": return SportsDB.Status.FT;
                    case "AET": return SportsDB.Status.ET;
                    case "AP": return SportsDB.Status.PK;
                    case "Canceled": return SportsDB.Status.Cancelled;
                    case "Cancelled": return SportsDB.Status.Cancelled;
                    case "Postponed": return SportsDB.Status.Postponed;
                    case "Abandoned": return SportsDB.Status.Abandoned;
                    case "-": return SportsDB.Status.Pending;
                    default:
                        WriteWarning("WebMatch.FixStatus: Unknown Status => {0}, url: {1}{2}"
                            , status, Environment.NewLine, matchUrl);
                        return SportsDB.Status.Unknown;
                }
            }
            public static Nullable<uint> FixInt(string s)
            {
                if (s != null && s.Length > 0)
                {
                    try
                    {
                        return uint.Parse(s);
                    }
                    catch { }
                }
                return null;
            }
            public static Nullable<ulong> FixLong(string s)
            {
                if (s != null && s.Length > 0)
                {
                    try
                    {
                        return ulong.Parse(s);
                    }
                    catch { }
                }
                return null;
            }
            public static double FixOdd(string s)
            {
                if (s != null && s.Length > 0)
                {
                    try
                    {
                        return double.Parse(s.Replace(".", ","));
                    }
                    catch { }
                }
                return 0;
            }
            public static SportsDB.TeamSide FixTeamSide(string value, string matchUrl, ActionWrite WriteWarning)
            {
                try
                {
                    if (value.Trim() == "-") return SportsDB.TeamSide.None;
                    return (SportsDB.TeamSide)Enum.Parse(typeof(SportsDB.TeamSide), value);
                }
                catch
                {
                    WriteWarning("WebMatch.FixTeamSide: Error parsing value => {0}, url: {1}{2}"
                        , value, Environment.NewLine, matchUrl);
                    return SportsDB.TeamSide.None;
                }
            }
            #endregion
        }
        private class WebData
        {
            public WebData() { }
            public WebData(string csv,string date, ActionWrite WriteWarning)
            {
                this.Matches = new List<WebMatch>();
                var lines = csv.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Length > 0)
                    {
                        var cells = line.Split('\t');
                        if (cells.Length == 10)
                        {
                            var match = new WebMatch();
                            match.Match.WebId = cells[0];
                            match.Country = cells[1];
                            match.Match.Champ = cells[2];
                            match.Match.StartTime = DateTime.Parse(string.Format("{0} {1}", date, cells[3]));
                            match.Match.HomeTeam = cells[4];
                            match.Match.AwayTeam = cells[5];
                            match.Match.HomeScoreFT = WebMatch.FixInt(cells[6]);
                            match.Match.AwayScoreFT = WebMatch.FixInt(cells[7]);
                            match.Url = cells[9];
                            match.Match.Status = WebMatch.FixStatus(cells[8], match.Url, WriteWarning);
                            this.Matches.Add(match);
                        }
                    }
                }
            }
            public List<WebMatch> Matches { get; set; }
            public List<string> Urls
            {
                get
                {
                    return this.Matches.Select(x => x.Url).ToList();
                }
            }
        }
        #endregion
        #region Init
        private ChromiumWebBrowser Browser { get; set; }
        private TaskCompletionSource<bool> BrowserLoadingComplete { get; set; }
        private CurrentStep Step { get; set; }
        private WebData Data { get; set; }
        private WebMatch CurrentMatch { get; set; }
        private List<WebMatch> RemainingMatches { get; set; }
        private List<Task> Tasks { get; set; } = new List<Task>();
        public SofaScores()
        {
            InitializeComponent();
        }
        private void SofaScores_Load(object sender, EventArgs e)
        {
            // Read CurrentDay
            string currentDay = SportsDB.GetFlag("CurrentDay");
            if (String.IsNullOrEmpty(currentDay))
            {
                currentDay = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }
            this.textCurrentDay.Text = currentDay;
            // Init browser and timer
            this.InitBrowser();
            this.Step = CurrentStep.NothingDone;
            timerMain.Interval = 1000;
        }
        private void SofaScores_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }
        #endregion
        #region Browser Control
        private void InitBrowser()
        {
            if (this.Browser == null) // create browser
            {
                this.Browser = new ChromiumWebBrowser("");
                this.Browser.Dock = DockStyle.Fill;
                this.Browser.BrowserSettings.ImageLoading = CefState.Disabled;
                this.panelBrowser.Controls.Clear();
                this.panelBrowser.Controls.Add(this.Browser);
                this.Browser.LoadingStateChanged += Browser_LoadingStateChanged;
            }
        }
        private Task LoadPageAsync(string url)
        {
            this.BrowserLoadingComplete = new TaskCompletionSource<bool>();
            EventHandler<LoadingStateChangedEventArgs> handler = null;
            handler = (sender, args) =>
            {
                //Wait for while page to finish loading not just the first frame
                if (!args.IsLoading)
                {
                    this.Browser.LoadingStateChanged -= handler;
                    //BrowserLoadingComplete.TrySetResult(true);
                }
            };
            if (!string.IsNullOrEmpty(url))
            {
                this.Browser.Load(url);
            }
            return BrowserLoadingComplete.Task;
        }
        private void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading || e.Browser.IsLoading)
            {
                timerLoadComplete.Stop();
                timerLoadComplete.Interval = 100;
            }
            else
            {
                timerLoadComplete.Interval = 1000;
                timerLoadComplete.Start();
            }
        }
        private void timerLoadComplete_Tick(object sender, EventArgs e)
        {
            this.BrowserLoadingComplete.TrySetResult(true);
        }
        private void timerMain_Tick(object sender, EventArgs e)
        {
            try
            {
                switch (this.Step)
                {
                    case CurrentStep.NothingDone:
                        this.LoadDay();
                        break;
                    case CurrentStep.DayLoaded:
                        this.ReadDay();
                        break;
                    case CurrentStep.DayReaded:
                        this.LoadNextDay();
                        //this.LoadNextMatch();
                        break;
                    case CurrentStep.MatchLoaded:
                        //this.ReadMatch();
                        break;
                    case CurrentStep.MatchReaded:
                        //this.LoadNextMatch();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                timerMain.Enabled = false;
                this.WriteLog("Timer: Step => {0}, Error => {1}", this.Step, ex.Message);
            }
        }
        #endregion
        #region Navigate Methods
        private void GoToStep(CurrentStep step)
        {
            this.Step = step;
            timerMain.Interval = 1000;
            timerMain.Enabled = true;
        }
        private async void LoadDay()
        {
            this.Step = CurrentStep.DayLoading;
            await this.LoadPageAsync(string.Format("http://www.sofascore.com/football/{0}", this.textCurrentDay.Text));
            this.Step = CurrentStep.DayLoaded;
            //WriteLog("{0}", this.Step);
        }
        private void LoadNextDay()
        {
            this.textCurrentDay.Text = DateTime.Parse(textCurrentDay.Text).AddDays(-1).ToString("yyyy-MM-dd");
            SportsDB.SetFlag("CurrentDay", this.textCurrentDay.Text);
            this.LoadDay();
        }
        private async void ReadDay()
        {
            this.Step = CurrentStep.DayReading;
            var script = this.GetJavascript("ReadDay.js");
            retry:
            var task = this.Browser.EvaluateScriptAsync(script);
            task.Wait();
            var response = task.Result;
            var result = response.Success ? (response.Result ?? "null") : response.Message;
            var csv = (string)result;
            if (csv.StartsWith("ERROR!! "))
            {
                this.Data = null;
                //this.WriteLog("{0}", csv);
                this.WriteLog("ReadDay: retrying...");
                Task.Run(() => { System.Threading.Thread.Sleep(1000); }).Wait();
                goto retry;
            }
            else
            {
                this.Data = new WebData(csv, this.textCurrentDay.Text, WriteWarning);
                this.RemainingMatches = new List<WebMatch>();
                foreach (var webMatch in this.Data.Matches)
                {
                    if (webMatch.Match.Status == SportsDB.Status.FT)
                    {
                        this.Tasks.Add(webMatch.Match.InsertOrUpdateAsync(this.WriteLog));
                        // limit parallel tasks
                        var max_tasks = 10;
                        while (this.Tasks.Count > max_tasks)
                        {
                            foreach (var item in this.Tasks.ToArray())
                            {
                                if (item.IsCompleted)
                                {
                                    this.Tasks.Remove(item);
                                }
                            }
                            if (this.Tasks.Count > max_tasks)
                            {
                                await Task.Run(() => { System.Threading.Thread.Sleep(100); });
                            }
                        }
                    }
                }
            }
            // Update step and log it
            this.Step = CurrentStep.DayReaded;
            //this.WriteLog("{0}", this.Step);
            GC.Collect();
        }
        private async void LoadNextMatch()
        {
            if (this.RemainingMatches != null && this.RemainingMatches.Count > 0)
            {
                this.Step = CurrentStep.MatchLoading;
                this.CurrentMatch = this.RemainingMatches[0];
                await this.LoadPageAsync(this.CurrentMatch.Url);
                this.RemainingMatches.Remove(this.CurrentMatch);
                this.Step = CurrentStep.MatchLoaded;
                //WriteLog("{0}", this.Step);
            }
            else
            {
                this.LoadNextDay();
            }
        }
        private void ReadMatch()
        {
            this.Step = CurrentStep.MatchReading;
            var script = this.GetJavascript("ReadMatch.js");
            var task = this.Browser.EvaluateScriptAsync(script);
            task.Wait();
            var response = task.Result;
            var result = response.Success ? (response.Result ?? "null") : response.Message;
            var csv = (string)result;
            if (csv.StartsWith("ERROR!! "))
            {
                this.Stop();
                this.WriteWarning("ReadMatch: {0}", csv);
            }
            else
            {
                this.CurrentMatch.AddDetails(csv, WriteWarning);
                var match = this.CurrentMatch.Match;
                this.CurrentMatch.Match = null;
                // update database async
                //SportsDB.InsertOrUpdateMatchAsync(match, this.WriteLog);
            }
            // Update step and log it
            this.Step = CurrentStep.MatchReaded;
            //this.WriteLog("{0}", this.Step);
            GC.Collect();
        }
        #endregion
        #region General Methods
        private string GetJavascript(string filename)
        {
#if (DEBUG)
            return File.ReadAllText(string.Format(@"C:\SourceCode\LiveReader2\LiveReader2\Javascripts\{0}", filename));
#else
            string result = "";
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var name = string.Format("{0}.Javascripts.{1}", assembly.GetName().Name, filename);
            using (Stream stream = assembly.GetManifestResourceStream(name))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            return result;
#endif
        }
        private void WriteText(string text)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    listLog.Items.Insert(0, text);
                }));
            }
            catch { }
        }
        private void WriteLog(string format, params object[] args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    var message = string.Format(format, args);
                    listLog.Items.Insert(0, string.Format("{0:yyyy-MM-dd HH:mm:ss}# {1}", DateTime.Now, message));
                    if (listLog.Items.Count > 1000)
                    {
                        listLog.Items.RemoveAt(1000);
                    }
                }));
            }
            catch { }
        }
        private void WriteWarning(string format, params object[] args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    var message = string.Format(format, args);
                    listWarning.Items.Insert(0, string.Format("{0:yyyy-MM-dd HH:mm:ss}# {1}", DateTime.Now, message));
                }));
            }
            catch { }
        }
        #endregion
        private void Start()
        {
            timerMain.Start();
            buttonStart.Text = "Stop!";
            buttonStart.ForeColor = Color.Red;
            SportsDB.SetFlag("CurrentDay", this.textCurrentDay.Text);
        }
        private void Stop()
        {
            timerMain.Stop();
            buttonStart.Text = "Start!";
            buttonStart.ForeColor = Color.Green;
        }
        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (timerMain.Enabled)
            {
                this.Stop();
            }
            else
            {
                this.Start();
            }
        }
        private void listLog_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                MessageText.Show("{0}", (string)listLog.SelectedItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void listDayErrors_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                MessageText.Show("{0}", (string)listWarning.SelectedItem);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void readDayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.ReadDay();
        }
        private void readMatchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //this.ReadMatch();
        }
        private void viewUrlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageText.Show(Browser.Address);
        }
        private void cachePathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Path.Combine(Application.LocalUserAppDataPath, "Cache"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
