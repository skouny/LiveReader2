using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Collections.Specialized;
using System.IO;

namespace LiveReader2
{
    public delegate void ActionWriteLog(string format, params object[] args);
    public partial class WebReaderBase : Form
    {
        #region Init
        public SportName Sport;
        public SourceName Source;
        public Dictionary<string, NameValueCollection> Matches;
        public bool Canceled = false;
        DateTime LastRefresh = DateTime.MinValue;
        public virtual string UrlString { get; set; } // Override for url list
        public WebReaderBase(SourceName source, SportName sport, string url = "")
        {
            InitializeComponent();
            Source = source;
            Sport = sport;
            UrlString = url;
        }
        private void WebReaderBase_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("{0} - {1}", Source, Sport);
            LoadSettings();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser.AllowNavigation = false;
            webBrowser.IsWebBrowserContextMenuEnabled = false;
            webBrowser.AllowWebBrowserDrop = false; // ???
            webBrowser.WebBrowserShortcutsEnabled = false;
            webBrowser.Navigate(this.UrlString);
            LastRefresh = DateTime.Now;
        }
        private void WebReaderBase_Shown(object sender, EventArgs e)
        {
            this.Visible = false;
        }
        private void WebReaderBase_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Visible = false;
                e.Cancel = true;
            }
            SaveSettings();
        }
        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (!backgroundWorker.IsBusy && !Canceled)
            {
                backgroundWorker.RunWorkerAsync(webBrowser.Document.Body);
            }
        }
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var tickStart = Environment.TickCount;
            var body = (HtmlElement)e.Argument;
            var webIds = new List<string>();
            // Get matches existing in database
            Matches = GetExistingMatches();
            // Update with matches shown in browser
            int modified = DoUpdate(body, ref webIds);
            // If operation not canceled
            if (!backgroundWorker.CancellationPending)
            {
                // Remove matches that no loger exist in browser
                //foreach (var match in Matches)
                //{
                //    var webId = match.Key;
                //    if (!webIds.Contains(webId))
                //    {
                //        var msg = WebDB.table_live_mix_delete(this.Sport, this.Source, webId);
                //        WriteLog(msg);
                //    }
                //    if (backgroundWorker.CancellationPending) break;
                //}
                // Log the results
                if (ShowLoopLog)
                {
                    WriteLog("Modified: {0}", modified);
                    WriteLog("CPU Time: {0} ms", Environment.TickCount - tickStart);
                }
                // Make a break
                if (!backgroundWorker.CancellationPending) DoLoopBreak();
            }
        }
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (DateTime.Compare(DateTime.Now, LastRefresh.AddMinutes(PageRefresh)) > 0)
                {
                    webBrowser.Refresh(WebBrowserRefreshOption.Completely);
                    LastRefresh = DateTime.Now;
                }
                backgroundWorker.RunWorkerAsync(webBrowser.Document.Body);
            }
            catch (Exception ex) { WriteLog("ERROR! backgroundWorker_RunWorkerCompleted: {0}", ex.Message); }
        }
        #endregion
        #region Input controls
        private void numericUpDownLoopBreak_ValueChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }
        private void numericUpDownMatchBreak_ValueChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }
        private void checkBoxUseParallelProcessing_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDownMatchBreak.Enabled = !checkBoxUseParallelProcessing.Checked;
            SaveSettings();
        }
        private void checkBoxShowLoopLog_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }
        private void checkBoxShowDetailedLog_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }
        private void numericUpDownPageRefresh_ValueChanged(object sender, EventArgs e)
        {
            SaveSettings();
        }
        #endregion
        #region Form Settings
        private void SaveSettings()
        {
            try
            {
                var xDoc = new XElement("Settings");
                xDoc.Add(new XElement("Top", (this.Top > 0) ? this.Top : 0));
                xDoc.Add(new XElement("Left", (this.Left > 0) ? this.Left : 0));
                xDoc.Add(new XElement("Width", (this.Width > 0) ? this.Width : 800));
                xDoc.Add(new XElement("Height", (this.Height > 0) ? this.Height : 600));
                xDoc.Add(new XElement("LoopBreak", numericUpDownLoopBreak.Value));
                xDoc.Add(new XElement("MatchBreak", numericUpDownMatchBreak.Value));
                xDoc.Add(new XElement("UseParallelProcessing", checkBoxUseParallelProcessing.Checked));
                xDoc.Add(new XElement("PageRefresh", numericUpDownPageRefresh.Value));
                xDoc.Add(new XElement("ShowLoopLog", checkBoxShowLoopLog.Checked));
                xDoc.Add(new XElement("ShowDetailedLog", checkBoxShowDetailedLog.Checked));
                xDoc.Save(SettingsFileName);
            }
            catch (Exception ex) { WriteLog("ERROR! SaveSettings: {0}", ex.Message); }
        }
        private void LoadSettings()
        {
            try
            {
                if (File.Exists(SettingsFileName))
                {
                    var xDoc = XDocument.Load(SettingsFileName);
                    this.Top = int.Parse(xDoc.Root.Element("Top").Value);
                    this.Left = int.Parse(xDoc.Root.Element("Left").Value);
                    this.Width = int.Parse(xDoc.Root.Element("Width").Value);
                    this.Height = int.Parse(xDoc.Root.Element("Height").Value);
                    numericUpDownLoopBreak.Value = decimal.Parse(xDoc.Root.Element("LoopBreak").Value);
                    numericUpDownMatchBreak.Value = decimal.Parse(xDoc.Root.Element("MatchBreak").Value);
                    checkBoxUseParallelProcessing.Checked = bool.Parse(xDoc.Root.Element("UseParallelProcessing").Value);
                    numericUpDownPageRefresh.Value = decimal.Parse(xDoc.Root.Element("PageRefresh").Value);
                    checkBoxShowLoopLog.Checked = bool.Parse(xDoc.Root.Element("ShowLoopLog").Value);
                    checkBoxShowDetailedLog.Checked = bool.Parse(xDoc.Root.Element("ShowDetailedLog").Value);
                    //
                    if (this.Top < 0) this.Top = 0;
                    if (this.Left < 0) this.Left = 0;
                }
            }
            catch (Exception ex) { WriteLog("ERROR! LoadSettings: {0}", ex.Message); }
        }
        private string SettingsFileName
        {
            get
            {
                return MyPaths.WebReaderSettingsXml(this.Text);
            }
        }
        #endregion
        #region Thread Safe Setting Properties
        private int LoopBreak()
        { // miliseconds
            try
            {
                return (int)this.Invoke(new Func<int>(() =>
                {
                    return (int)numericUpDownLoopBreak.Value;
                }));
            }
            catch { }
            return 0;
        }
        private int MatchBreak()
        { // miliseconds
            try
            {
                return (int)this.Invoke(new Func<int>(() =>
                {
                    return (int)numericUpDownMatchBreak.Value;
                }));
            }
            catch { }
            return 0;
        }
        private int PageRefresh
        { // minutes
            get
            {
                try
                {
                    return (int)this.Invoke(new Func<int>(() =>
                    {
                        return (int)numericUpDownPageRefresh.Value;
                    }));
                }
                catch { }
                return 0;
            }
        }
        public bool UseParallelProcessing
        {
            get
            {
                try
                {
                    return (bool)this.Invoke(new Func<bool>(() =>
                    {
                        return checkBoxUseParallelProcessing.Checked;
                    }));
                }
                catch { }
                return false;
            }
        }
        public bool ShowLoopLog
        {
            get
            {
                try
                {
                    return (bool)this.Invoke(new Func<bool>(() =>
                    {
                        return checkBoxShowLoopLog.Checked;
                    }));
                }
                catch { }
                return false;
            }
        }
        public bool ShowDetailedLog
        {
            get
            {
                try
                {
                    return (bool)this.Invoke(new Func<bool>(() =>
                    {
                        return checkBoxShowDetailedLog.Checked;
                    }));
                }
                catch { }
                return false;
            }
        }
        #endregion
        #region General Methods
        private void DoBreak(Func<int> calcTime, int step)
        { // do loop break and watch for changes once per sec
            int time = calcTime();
            if (time > 0)
            {
                if (time > 1000)
                {
                    int i = 0;
                    while (i < calcTime())
                    {
                        int ms = calcTime() - i;
                        if (ms > 1000) ms = 1000;
                        System.Threading.Thread.Sleep(ms);
                        i += ms;
                    }
                }
                else
                {
                    System.Threading.Thread.Sleep(time);
                }
            }
        }
        public void DoLoopBreak()
        {
            DoBreak(LoopBreak, 1000);
        }
        public void DoMatchBreak()
        {
            DoBreak(MatchBreak, 100);
        }
        public void WriteLog(string format, params object[] args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    var message = string.Format(format, args);
                    listBoxLog.Items.Insert(0, string.Format("{0:yyyy-MM-dd HH:mm:ss}# {1}", DateTime.Now, message));
                    if (listBoxLog.Items.Count > 100)
                    {
                        listBoxLog.Items.RemoveAt(100);
                    }
                }));
            }
            catch { }
        }
        public bool StartTimePassed(string startTime)
        {
            try
            {
                if (startTime == "0000-00-00 00:00:00")
                {
                    return true;
                }
                else
                {
                    var StartTime = DateTime.Parse(startTime);
                    if (DateTime.Compare(DateTime.Now, StartTime) >= 0) return true;
                }
            }
            catch (Exception ex) { WriteLog("ERROR! StartTimePassed: {0}, Message: {1}", startTime, ex.Message); }
            return false;
        }
        public void Stop()
        {
            this.Canceled = true;
            backgroundWorker.CancelAsync();
            //while (true)
            //{
            //    if (!backgroundWorker.IsBusy) break;
            //    System.Threading.Thread.Sleep(100);
            //}
        }
        #endregion
        #region Update Methods
        private Dictionary<string, NameValueCollection> GetExistingMatches()
        {
            var matches = new Dictionary<string, NameValueCollection>();
            int retry = 0;
            while (retry < 10)
            {
                try
                {
                    var xUrl = WebDB.url_table_live_mix_get(Sport, Source);
                    var xDoc = XDocument.Load(xUrl);
                    foreach (var element in xDoc.Root.Elements())
                    {
                        var match = new NameValueCollection();
                        match["Id"] = element.Attribute("Id").Value;
                        match["Source"] = element.Attribute("Source").Value;
                        match["WebId"] = element.Attribute("WebId").Value;
                        match["StartTime"] = element.Attribute("StartTime").Value;
                        match["Champ"] = element.Attribute("Champ").Value;
                        match["HomeTeam"] = element.Attribute("HomeTeam").Value;
                        match["AwayTeam"] = element.Attribute("AwayTeam").Value;
                        match["Score"] = element.Attribute("Score").Value;
                        match["ScoreHT"] = element.Attribute("ScoreHT").Value;
                        if (Sport == SportName.Football)
                        {
                            match["YellowCards"] = element.Attribute("YellowCards").Value;
                            match["RedCards"] = element.Attribute("RedCards").Value;
                            match["CornerKicks"] = element.Attribute("CornerKicks").Value;
                            match["Shots"] = element.Attribute("Shots").Value;
                            match["ShotsOnGoal"] = element.Attribute("ShotsOnGoal").Value;
                            match["Fouls"] = element.Attribute("Fouls").Value;
                            match["BallPossession"] = element.Attribute("BallPossession").Value;
                            match["Saves"] = element.Attribute("Saves").Value;
                            match["Offsides"] = element.Attribute("Offsides").Value;
                            match["KickOff"] = element.Attribute("KickOff").Value;
                            match["HomeEvents"] = element.Attribute("HomeEvents").Value;
                            match["AwayEvents"] = element.Attribute("AwayEvents").Value;
                        }
                        else if (Sport == SportName.Basket)
                        {
                            match["ScoreQ1"] = element.Attribute("ScoreQ1").Value;
                            match["ScoreQ2"] = element.Attribute("ScoreQ2").Value;
                            match["ScoreQ3"] = element.Attribute("ScoreQ3").Value;
                            match["ScoreQ4"] = element.Attribute("ScoreQ4").Value;
                            match["StandingPoints"] = element.Attribute("StandingPoints").Value;
                        }
                        match["Minute"] = element.Attribute("Minute").Value;
                        match["Status"] = element.Attribute("Status").Value;
                        match["HomeScored"] = element.Attribute("HomeScored").Value;
                        match["AwayScored"] = element.Attribute("AwayScored").Value;
                        match["Modified"] = element.Attribute("Modified").Value;
                        matches.Add(match["WebId"], match);
                    }
                    break;
                }
                catch (Exception ex) { WriteLog("ERROR! GetExistingMatches: {0} [try {1}]", ex.Message, retry++); }
            }
            return matches;
        }
        public virtual int DoUpdate(HtmlElement body, ref List<string> webIds) { return 0; } // this is NOT REAL, must be overriden
        public static NameValueCollection UpdateAndPostMatch(SportName sport, NameValueCollection curMatch, NameValueCollection newMatch, ActionWriteLog writeLog, bool showDetailedLog)
        {
            bool modified = false;
            if (curMatch == null)
            {
                curMatch = newMatch;
                if (showDetailedLog) WriteMatchLog(writeLog, newMatch, "added");
                modified = true;
            }
            else if (UpdateMatch(ref curMatch, newMatch, sport))
            {
                modified = true;
                if (showDetailedLog) WriteMatchLog(writeLog, curMatch, "updated");
            }
            // if modified, return the curMatch
            if (modified)
            {
                return curMatch;
            }
            return null;
        }
        private static bool UpdateMatch(ref NameValueCollection curMatch, NameValueCollection newMatch, SportName sport)
        {
            bool modified = false;
            if (newMatch != null)
            {
                string prevValue = null;
                if (UpdateValue(ref curMatch, newMatch, "StartTime", ref prevValue)) { modified = true; }
                if (UpdateValue(ref curMatch, newMatch, "Champ", ref prevValue)) { modified = true; }
                if (UpdateValue(ref curMatch, newMatch, "HomeTeam", ref prevValue)) { modified = true; }
                if (UpdateValue(ref curMatch, newMatch, "AwayTeam", ref prevValue)) { modified = true; }
                if (UpdateValue(ref curMatch, newMatch, "ScoreHT", ref prevValue)) { modified = true; }
                if (UpdateValue(ref curMatch, newMatch, "Score", ref prevValue))
                {
                    modified = true;
                    if (Score.HomeScored(newMatch["Score"], prevValue))
                    {
                        curMatch["HomeScored"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (Score.AwayScored(newMatch["Score"], prevValue))
                    {
                        curMatch["AwayScored"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
                if (UpdateValue(ref curMatch, newMatch, "Status", ref prevValue)) { modified = true; }
                UpdateValue(ref curMatch, newMatch, "StatusId", ref prevValue); // The same with 'Status', problem if checked.
                if (UpdateValue(ref curMatch, newMatch, "Minute", ref prevValue)) { modified = true; }
                if (sport == SportName.Football)
                {
                    if (UpdateValue(ref curMatch, newMatch, "YellowCards", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "RedCards", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "CornerKicks", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "Shots", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "ShotsOnGoal", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "Fouls", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "BallPossession", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "Saves", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "Offsides", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "KickOff", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "HomeEvents", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "AwayEvents", ref prevValue)) { modified = true; }
                }
                else if (sport == SportName.Basket)
                {
                    if (UpdateValue(ref curMatch, newMatch, "ScoreQ1", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "ScoreQ2", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "ScoreQ3", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "ScoreQ4", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "StandingPoints", ref prevValue)) { modified = true; }
                }
                if (modified)
                {
                    curMatch["Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            return modified;
        }
        private static bool UpdateValue(ref NameValueCollection curMatch, NameValueCollection newMatch, string key, ref string prevValue)
        {
            bool modified = false;
            prevValue = null;
            // if the new match has the key
            if (newMatch.AllKeys.Contains(key))
            {
                // set its value to cur match, if not already has the same value...
                if (curMatch.AllKeys.Contains(key))
                {
                    if (curMatch[key] != newMatch[key])
                    {
                        prevValue = curMatch[key];
                        curMatch[key] = newMatch[key];
                        modified = true;
                    }
                }
                else
                {
                    curMatch[key] = newMatch[key];
                    modified = true;
                }
            }
            return modified;
        }
        private static void WriteMatchLog(ActionWriteLog WriteLog, NameValueCollection newMatch, string action)
        {
            WriteLog("Match {0}: [{1}] {2} - {3} {4} ({5}) {6} {7}", action
                , newMatch["StartTime"], newMatch["HomeTeam"], newMatch["AwayTeam"], newMatch["Score"]
                , newMatch["ScoreHT"], newMatch["Status"], newMatch["Minute"]);
        }
        #endregion
    }
}
