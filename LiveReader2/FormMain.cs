using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Net;
using System.IO;
using play90;

namespace LiveReader2
{
    public partial class FormMain : Form
    {
        public const int LIVE_UPDATE_DELAY = 2000; // in msec
        public class CreateArgument
        {
            public DateTime Date { get; set; }
            public bool Refresh { get; set; }
            public CreateArgument(DateTime date, bool refresh)
            {
                this.Date = date;
                this.Refresh = refresh;
            }
        }
        #region Init
        private WebReaderControl webReaderControlOpapFootball { get; set; }
        private WebReaderControl webReaderControlNowGoalFootball { get; set; }
        private WebReaderControl webReaderControlFlashScoreFootball { get; set; }
        private WebReaderControl webReaderControlFutbol24Football { get; set; }
        private WebReaderControl webReaderControlOpapBasket { get; set; }
        private WebReaderControl webReaderControlNowGoalBasket { get; set; }
        private SofaScores formSofaScores { get; set; }
        private SofaStandings formSofaStandings { get; set; }
        private DateTime nextRestart;
        private DateTime nextUpdate;
        public FormMain()
        {
            InitializeComponent();
            webReaderControlOpapFootball = CreateWebReaderControl(SourceName.Opap, SportName.Football);
            webReaderControlNowGoalFootball = CreateWebReaderControl(SourceName.NowGoal, SportName.Football);
            webReaderControlFlashScoreFootball = CreateWebReaderControl(SourceName.FlashScore, SportName.Football);
            webReaderControlFutbol24Football = CreateWebReaderControl(SourceName.Futbol24, SportName.Football);
            webReaderControlOpapBasket = CreateWebReaderControl(SourceName.Opap, SportName.Basket);
            webReaderControlNowGoalBasket = CreateWebReaderControl(SourceName.NowGoal, SportName.Basket);
            AddWebReaderControl(webReaderControlOpapFootball);
            AddWebReaderControl(webReaderControlNowGoalFootball);
            AddWebReaderControl(webReaderControlFlashScoreFootball);
            AddWebReaderControl(webReaderControlFutbol24Football);
            AddWebReaderControl(webReaderControlOpapBasket);
            AddWebReaderControl(webReaderControlNowGoalBasket);
            AddWebReaderControl(CreateSofaStandingsButton());
            AddWebReaderControl(CreateSofaScoresButton());
            // 
            var t = DateTime.Now.AddDays(1);
            nextRestart = new DateTime(t.Year, t.Month, t.Day, 10, 30, 0);
            nextUpdate = DateTime.Now;
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            LoadSettings();
            notifyMain.Visible = true;
            labelVersion.Text = string.Format("Version: {0}", Application.ProductVersion);
            // start workers
            this.InitFootballGr();
            this.InitBasketGr();
            this.InitFormMakerGr();
            this.InitBetMixGr();
            timerMain.Enabled = true;
        }
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                base.Hide();
                e.Cancel = true;
            }
        }
        #endregion
        #region General
        private void timerMain_Tick(object sender, EventArgs e)
        {
            // Restart
            if (DateTime.Now > nextRestart)
            {
                this.CleanupAndRestart();
            }
            else
            {
                WriteStatusRestart(nextRestart - DateTime.Now);
            }
            // Update
            if (DateTime.Now > nextUpdate && !workerUpdate.IsBusy)
            {
                workerUpdate.RunWorkerAsync();
            }
            else
            {
                if (nextUpdate > DateTime.Now) WriteStatusUpdate(nextUpdate - DateTime.Now);
            }
        }
        private void workerUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                // Update SportsDB
                WriteStatusUpdate("Updating...");
                //var currentDayId = OpapJson.CurrentDayId;
                var days = Opap.API.GetTodayYesterday(Opap.API.Sport.soccer);
                foreach (var day in days)
                {
                    if (day.Value.Count > 0)
                    {
                        int i = 0;
                        foreach (var jsonMatch in day.Value)
                        {
                            var match = new SportsDB.Match(jsonMatch);
                            match.InsertOrUpdate();
                            WriteStatusUpdate($"Match => {day.Key} [{++i}/{day.Value.Count}]");
                        }
                        // update mix
                        WriteStatusUpdate($"Updating Mix {day}...");
                        SportsDB.Day.UpdateMixes(workerUpdate, day.Key);
                    }
                }
                WriteStatusUpdate("Updated!");
            }
            catch { }
        }
        private void ThisShowHide()
        {
            if (this.Visible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }
        private void Exit()
        {
            SaveSettings();
            this.Dispose();
        }
        private void SaveSettings()
        {
            try
            {
                var xDoc = new XElement("Settings");
                xDoc.Add(new XElement("Top", (this.Top > 0 && this.Top < Screen.PrimaryScreen.Bounds.Height) ? this.Top : 0));
                xDoc.Add(new XElement("Left", (this.Left > 0 && this.Left < Screen.PrimaryScreen.Bounds.Width) ? this.Left : 0));
                xDoc.Add(new XElement("Width", (this.Width > 600) ? this.Width : 600));
                xDoc.Add(new XElement("Height", (this.Height > 400) ? this.Height : 400));
                xDoc.Add(new XElement("OpapFootballAutoStart", webReaderControlOpapFootball.AutoStart));
                xDoc.Add(new XElement("OpapFootballAutoRestart", webReaderControlOpapFootball.AutoRestart));
                xDoc.Add(new XElement("NowGoalFootballAutoStart", webReaderControlNowGoalFootball.AutoStart));
                xDoc.Add(new XElement("NowGoalFootballAutoRestart", webReaderControlNowGoalFootball.AutoRestart));
                xDoc.Add(new XElement("SportingBetFootballAutoStart", webReaderControlFlashScoreFootball.AutoStart));
                xDoc.Add(new XElement("SportingBetFootballAutoRestart", webReaderControlFlashScoreFootball.AutoRestart));
                xDoc.Add(new XElement("Futbol24FootballAutoStart", webReaderControlFutbol24Football.AutoStart));
                xDoc.Add(new XElement("Futbol24FootballAutoRestart", webReaderControlFutbol24Football.AutoRestart));
                xDoc.Add(new XElement("OpapBasketAutoStart", webReaderControlOpapBasket.AutoStart));
                xDoc.Add(new XElement("OpapBasketAutoRestart", webReaderControlOpapBasket.AutoRestart));
                xDoc.Add(new XElement("NowGoalBasketAutoStart", webReaderControlNowGoalBasket.AutoStart));
                xDoc.Add(new XElement("NowGoalBasketAutoRestart", webReaderControlNowGoalBasket.AutoRestart));
                xDoc.Save(MyPaths.SettingsXml);
            }
            catch { }
        }
        private void LoadSettings()
        {
            try
            {
                var xDoc = XDocument.Load(MyPaths.SettingsXml);
                this.Top = int.Parse(xDoc.Root.Element("Top").Value);
                this.Left = int.Parse(xDoc.Root.Element("Left").Value);
                this.Width = int.Parse(xDoc.Root.Element("Width").Value);
                this.Height = int.Parse(xDoc.Root.Element("Height").Value);
                webReaderControlOpapFootball.AutoStart = bool.Parse(xDoc.Root.Element("OpapFootballAutoStart").Value);
                webReaderControlOpapFootball.AutoRestart = decimal.Parse(xDoc.Root.Element("OpapFootballAutoRestart").Value);
                webReaderControlNowGoalFootball.AutoStart = bool.Parse(xDoc.Root.Element("NowGoalFootballAutoStart").Value);
                webReaderControlNowGoalFootball.AutoRestart = decimal.Parse(xDoc.Root.Element("NowGoalFootballAutoRestart").Value);
                webReaderControlFlashScoreFootball.AutoStart = bool.Parse(xDoc.Root.Element("SportingBetFootballAutoStart").Value);
                webReaderControlFlashScoreFootball.AutoRestart = decimal.Parse(xDoc.Root.Element("SportingBetFootballAutoRestart").Value);
                webReaderControlFutbol24Football.AutoStart = bool.Parse(xDoc.Root.Element("Futbol24FootballAutoStart").Value);
                webReaderControlFutbol24Football.AutoRestart = decimal.Parse(xDoc.Root.Element("Futbol24FootballAutoRestart").Value);
                webReaderControlOpapBasket.AutoStart = bool.Parse(xDoc.Root.Element("OpapBasketAutoStart").Value);
                webReaderControlOpapBasket.AutoRestart = decimal.Parse(xDoc.Root.Element("OpapBasketAutoRestart").Value);
                webReaderControlNowGoalBasket.AutoStart = bool.Parse(xDoc.Root.Element("NowGoalBasketAutoStart").Value);
                webReaderControlNowGoalBasket.AutoRestart = decimal.Parse(xDoc.Root.Element("NowGoalBasketAutoRestart").Value);
            }
            catch { }
        }
        private void CleanupAndRestart()
        {
            WebDB.table_live_mix_delete(SportName.Football);
            WebDB.table_live_mix_delete(SportName.Basket);
            Application.Restart();
        }
        private void WriteStatusRestart(TimeSpan t)
        {
            WriteStatusRestart(string.Format("{0:00}h {1:00}m {2:00}s", t.Hours, t.Minutes, t.Seconds));
        }
        private void WriteStatusRestart(string s)
        {
            this.Invoke(new Action(() =>
            {
                labelRestart.Text = string.Format("Restart: {0}", s);
            }));
        }
        private void WriteStatusUpdate(TimeSpan t)
        {
            WriteStatusUpdate(string.Format("{0:00}h {1:00}m {2:00}s", t.Hours, t.Minutes, t.Seconds));
        }
        private void WriteStatusUpdate(string s)
        {
            this.Invoke(new Action(() =>
            {
                labelUpdate.Text = string.Format("Update: {0}", s);
            }));
        }
        #endregion
        #region ToolBar
        private void buttonRestart_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
        private void buttonCleanupAndRestart_Click(object sender, EventArgs e)
        {
            this.CleanupAndRestart();
        }
        #endregion
        #region Menu Main
        private void showHideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThisShowHide();
        }
        private void updateStandingsFeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var message = StandingsUpdater.UpdateFeed();
            if (String.IsNullOrEmpty(message))
            {
                MessageBox.Show("OK!");
            }
            else
            {
                MessageBox.Show(message);
            }
        }
        private void clearFootballToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(WebDB.table_live_mix_delete(SportName.Football));
        }
        private void clearBasketToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(WebDB.table_live_mix_delete(SportName.Basket));
        }
        private void restartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
        private void cleanUpRestartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CleanupAndRestart();
        }
        private void openDataDirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(MyPaths.AppDataDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Exit();
        }
        #endregion
        #region Tab WebReaders
        private void AddWebReaderControl(Control control)
        {
            var controls = tabWebReaders.Controls;
            var bottomControl = BottomContol(controls);
            // Add
            controls.Add(control);
            // Set top position
            if (bottomControl == null)
            {
                control.Top = 0;
            }
            else
            {
                control.Top = bottomControl.Top + bottomControl.Height + 8;
            }
            // Set width with anchor
            control.Width = tabWebReaders.ClientSize.Width;
            control.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
        }
        private Control BottomContol(Control.ControlCollection controls)
        {
            Control bottomControl = null;
            foreach (Control control in controls)
            {
                if (bottomControl == null || bottomControl.Top < control.Top)
                {
                    bottomControl = control;
                }
            }
            return bottomControl;
        }
        private WebReaderControl CreateWebReaderControl(SourceName source, SportName sport)
        {
            var control = new WebReaderControl();
            control.AutoRestart = new decimal(new int[] { 0, 0, 0, 0 });
            control.AutoStart = false;
            control.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            control.Height = 70;
            control.Source = source;
            control.Sport = sport;
            control.Started = new System.DateTime(((long)(0)));
            return control;
        }
        private Button CreateSofaStandingsButton()
        {
            var button = new Button();
            button.Height = 40;
            button.Text = "Sofa Standings";
            button.Enabled = true;
            button.Click += (sender, e) =>
            {
                if (formSofaStandings == null || formSofaStandings.IsDisposed)
                {
                    formSofaStandings = new SofaStandings();
                }
                formSofaStandings.Show(this);
            };
            return button;
        }
        private Button CreateSofaScoresButton()
        {
            var button = new Button();
            button.Height = 40;
            button.Text = "Sofa Scores";
            button.Enabled = false;
            button.Click += (sender, e) =>
            {
                if (formSofaScores == null || formSofaScores.IsDisposed)
                {
                    formSofaScores = new SofaScores();
                }
                formSofaScores.Show();
            };
            return button;
        }
        #endregion
        #region Tab Footbal GR
        private void InitFootballGr()
        {
            workerFootballGr.RunWorkerAsync();
            textDayFootballGr.Text = Opap.CurrentDay.ToString("yyyy-MM-dd");
        }
        private void RefreshFootballGr()
        {
            refreshFootballGr.Enabled = false;
            try
            {
                var sport = Opap.API.Sport.soccer;
                var locale = Opap.API.Locale.el;
                var date = DateTime.Parse(textDayFootballGr.Text);
                var day = ServerIO.DownloadDay(sport, locale, date, WriteLogFootballGr);
                ListViewDay.Update(viewFootballGr, day, hideCompletedFootballGr.Checked, WriteLogFootballGr);
            }
            catch
            {
                viewFootballGr.Items.Clear();
            }
            finally
            {
                refreshFootballGr.Enabled = true;
            }
        }
        private void refreshFootballGr_Click(object sender, EventArgs e)
        {
            this.RefreshFootballGr();
        }
        private void updateDayFootballGr_Click(object sender, EventArgs e)
        {
            updateDayFootballGr.Enabled = false;
            var sport = Opap.API.Sport.soccer;
            var locale = Opap.API.Locale.el;
            var date = DateTime.Parse(textDayFootballGr.Text);
            var day = ServerIO.DownloadDay(sport, locale, date, WriteLogFootballGr);
            var feed = Opap.API.GetDay(textDayFootballGr.Text, sport, locale);
            if (MessageBox.Show($"CouponDay has {day?.Matches.Count} matches.{Environment.NewLine}OpapFeed has {feed.Count} matches.{Environment.NewLine}I m going to add/remove matches according to OpapFeed, are you sure?", "Unrecoverable modification warning!!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (day == null)
                {
                    day = new BetDayInfo(feed);
                }
                else
                {
                    day.Update(feed, null, true);
                }
                ServerIO.UploadDay(sport, locale, day);
                this.RefreshFootballGr();
            }
            updateDayFootballGr.Enabled = true;
        }
        private void hideCompletedFootballGr_CheckedChanged(object sender, EventArgs e)
        {
            this.RefreshFootballGr();
        }
        private void textDayFootballGr_TextChanged(object sender, EventArgs e)
        {
            this.RefreshFootballGr();
        }
        private void workerFootballGr_DoWork(object sender, DoWorkEventArgs e)
        {
            var updater = new OpapUpdater(Opap.API.Sport.soccer, Opap.API.Locale.el, WriteLogFootballGr);
            while (true)
            {
                updater.DoUpdate(this.DoFullUpdate, this.DoLivestats);
                Thread.Sleep(LIVE_UPDATE_DELAY);
            }
        }
        private void workerFootballGr_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        private void workerFootballGr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        private void calendarFootballGr_DateSelected(object sender, DateRangeEventArgs e)
        {
            textDayFootballGr.Text = calendarFootballGr.SelectionRange.Start.ToString("yyyy-MM-dd");
            calendarFootballGr.Visible = false;
        }
        private void calendarFootballGr_MouseLeave(object sender, EventArgs e)
        {
            calendarFootballGr.Visible = false;
        }
        private void textDayFootballGr_MouseEnter(object sender, EventArgs e)
        {
            if (!calendarFootballGr.Visible)
            {
                calendarFootballGr.Left = textDayFootballGr.Left;
                calendarFootballGr.Top = textDayFootballGr.Top + textDayFootballGr.Height;
                calendarFootballGr.Visible = true;
            }
        }
        private void WriteLogFootballGr(string format, params object[] args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    var message = string.Format(format, args);
                    logFootballGr.Items.Insert(0, string.Format("{0:yyyy-MM-dd HH:mm:ss}# {1}", DateTime.Now, message));
                    if (logFootballGr.Items.Count > 1000) logFootballGr.Items.RemoveAt(1000);
                }));
            }
            catch { }
        }
        private bool DoFullUpdate { get { return (bool)this.Invoke(new Func<bool>(() => { return (checkFullUpdate.Checked); })); } }
        private bool DoLivestats { get { return (bool)this.Invoke(new Func<bool>(() => { return (checkLivestats.Checked); })); } }
        #endregion
        #region Tab Basket GR
        private void InitBasketGr()
        {
            workerBasketGr.RunWorkerAsync();
            textDayBasketGr.Text = Opap.CurrentDay.ToString("yyyy-MM-dd");
        }
        private void RefreshBasketGr()
        {
            refreshBasketGr.Enabled = false;
            try
            {
                var sport = Opap.API.Sport.basketball;
                var locale = Opap.API.Locale.el;
                var date = DateTime.Parse(textDayBasketGr.Text);
                var day = ServerIO.DownloadDay(sport, locale, date, WriteLogBasketGr);
                ListViewDay.Update(viewBasketGr, day, hideCompletedBasketGr.Checked, WriteLogBasketGr);
            }
            catch
            {
                viewBasketGr.Items.Clear();
            }
            finally
            {
                refreshBasketGr.Enabled = true;
            }
        }
        private void updateDayBasketGr_Click(object sender, EventArgs e)
        {
            updateDayBasketGr.Enabled = false;
            var sport = Opap.API.Sport.basketball;
            var locale = Opap.API.Locale.el;
            var date = DateTime.Parse(textDayBasketGr.Text);
            var day = ServerIO.DownloadDay(sport, locale, date, WriteLogBasketGr);
            var feed = Opap.API.GetDay(textDayBasketGr.Text,sport, locale);
            if (MessageBox.Show($"CouponDay has {day?.Matches.Count} matches.{Environment.NewLine}OpapFeed has {feed.Count} matches.{Environment.NewLine}I m going to add/remove matches according to OpapFeed, are you sure?", "Unrecoverable modification warning!!", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (day == null)
                {
                    day = new BetDayInfo(feed);
                }
                else
                {
                    day.Update(feed, null, true);
                }
                ServerIO.UploadDay(sport, locale, day);
                this.RefreshBasketGr();
            }
            updateDayBasketGr.Enabled = true;
        }
        private void refreshBasketGr_Click(object sender, EventArgs e)
        {
            this.RefreshBasketGr();
        }
        private void hideCompletedBasketGr_CheckedChanged(object sender, EventArgs e)
        {
            this.RefreshBasketGr();
        }
        private void textDayBasketGr_TextChanged(object sender, EventArgs e)
        {
            this.RefreshBasketGr();
        }
        private void workerBasketGr_DoWork(object sender, DoWorkEventArgs e)
        {
            var updater = new OpapUpdater(Opap.API.Sport.basketball, Opap.API.Locale.el, WriteLogBasketGr);
            while (true)
            {
                updater.DoUpdate();
                Thread.Sleep(LIVE_UPDATE_DELAY);
            }
        }
        private void workerBasketGr_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        private void workerBasketGr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        private void calendarBasketGr_DateSelected(object sender, DateRangeEventArgs e)
        {
            textDayBasketGr.Text = calendarBasketGr.SelectionRange.Start.ToString("yyyy-MM-dd");
            calendarBasketGr.Visible = false;
        }
        private void calendarBasketGr_MouseLeave(object sender, EventArgs e)
        {
            calendarBasketGr.Visible = false;
        }
        private void textDayBasketGr_MouseEnter(object sender, EventArgs e)
        {
            if (!calendarBasketGr.Visible)
            {
                calendarBasketGr.Left = textDayBasketGr.Left;
                calendarBasketGr.Top = textDayBasketGr.Top + textDayBasketGr.Height;
                calendarBasketGr.Visible = true;
            }
        }
        private void WriteLogBasketGr(string format, params object[] args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    var message = string.Format(format, args);
                    logBasketGr.Items.Insert(0, string.Format("{0:yyyy-MM-dd HH:mm:ss}# {1}", DateTime.Now, message));
                    if (logBasketGr.Items.Count > 1000) logBasketGr.Items.RemoveAt(1000);
                }));
            }
            catch { }
        }
        #endregion
        #region Tab FormMaker
        private void InitFormMakerGr()
        {
            textDayFormMakerGr.Text = Opap.CurrentDay.ToString("yyyy-MM-dd");
        }
        private void RefreshFormsGr()
        {
            refreshFormMakerGr.Enabled = false;
            try
            {
                var sport = Opap.API.Sport.soccer;
                var locale = Opap.API.Locale.el;
                var date = DateTime.Parse(textDayFormMakerGr.Text);
                var day = ServerIO.DownloadDay(sport, locale, date, WriteLogFormMakerGr);
                var existing = ServerIO.ExistingForms(sport, locale, date);
                ListViewForms.Update(viewFormMakerGr, day, existing);
            }
            catch
            {
                viewFormMakerGr.Items.Clear();
            }
            finally
            {
                refreshFormMakerGr.Enabled = true;
            }
        }
        private void textDayFormMakerGr_TextChanged(object sender, EventArgs e)
        {
            this.RefreshFormsGr();
        }
        private void refreshFormMakerGr_Click(object sender, EventArgs e)
        {
            this.RefreshFormsGr();
        }
        private void createFormMakerGr_Click(object sender, EventArgs e)
        {
            createFormMakerGr.Enabled = false;
            recreateFormMakerGr.Enabled = false;
            var date = DateTime.Parse(textDayFormMakerGr.Text);
            workerFormMakerGr.RunWorkerAsync(new CreateArgument(date, false));
        }
        private void recreateFormMakerGr_Click(object sender, EventArgs e)
        {
            createFormMakerGr.Enabled = false;
            recreateFormMakerGr.Enabled = false;
            var date = DateTime.Parse(textDayFormMakerGr.Text);
            workerFormMakerGr.RunWorkerAsync(new CreateArgument(date, true));
        }
        private void workerFormMakerGr_DoWork(object sender, DoWorkEventArgs e)
        {
            var sport = Opap.API.Sport.soccer;
            var locale = Opap.API.Locale.el;
            var arg = (CreateArgument)e.Argument;
            var day = ServerIO.DownloadDay(sport, locale, arg.Date, WriteLogFormMakerGr);
            this.WriteLogFormMakerGr($"Day {day.Date:yyyy-MM-dd} fetched, has {day.Matches.Count} matches");
            foreach (var match in day.Matches)
            {
                try
                {


                    if (!arg.Refresh && ServerIO.FormExists(sport, locale, day.Date, match.Code.Value))
                    {
                        this.WriteLogFormMakerGr($"Form exists {day.Date:yyyy-MM-dd}:{match.Code.Value}");
                    }
                    else
                    {
                        // create
                        var element = FormMaker.CreateForm(day.Date, match);
                        // save
                        ServerIO.UploadForm(sport, locale, day.Date, match.Code.Value, element);
                        // log
                        this.WriteLogFormMakerGr($"Form created {day.Date:yyyy-MM-dd}:{match.Code.Value}");
                    }
                }
                catch (Exception ex)
                {
                    this.WriteLogFormMakerGr($"Error! Form: {day.Date:yyyy-MM-dd}:{match.Code.Value} => {ex.Message}");
                }
            }
            // log
            this.WriteLogFormMakerGr($"Day {day.Date:yyyy-MM-dd} is done");
        }
        private void workerFormMakerGr_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        private void workerFormMakerGr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            createFormMakerGr.Enabled = true;
            recreateFormMakerGr.Enabled = true;
            this.RefreshFormsGr();
        }
        private void calendarFormMaker_DateSelected(object sender, DateRangeEventArgs e)
        {
            textDayFormMakerGr.Text = calendarFormMaker.SelectionRange.Start.ToString("yyyy-MM-dd");
            calendarFormMaker.Visible = false;
        }
        private void calendarFormMaker_MouseLeave(object sender, EventArgs e)
        {
            calendarFormMaker.Visible = false;
        }
        private void textDayFormMakerGr_MouseEnter(object sender, EventArgs e)
        {
            if (!calendarFormMaker.Visible)
            {
                calendarFormMaker.Left = textDayFormMakerGr.Left;
                calendarFormMaker.Top = textDayFormMakerGr.Top + textDayFormMakerGr.Height;
                calendarFormMaker.Visible = true;
            }
        }
        private void WriteLogFormMakerGr(string format, params object[] args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    var message = string.Format(format, args);
                    logFormMakerGr.Items.Insert(0, string.Format("{0:yyyy-MM-dd HH:mm:ss}# {1}", DateTime.Now, message));
                    if (logFormMakerGr.Items.Count > 1000) logFormMakerGr.Items.RemoveAt(1000);
                }));
            }
            catch { }
        }
        #endregion
        #region Tab BetMix Maker
        private void InitBetMixGr()
        {
            textDayBetMixGr.Text = Opap.CurrentDay.ToString("yyyy-MM-dd");
        }
        private void RefreshBetMixGr()
        {
            refreshBetMix.Enabled = false;
            try
            {
                var sport = Opap.API.Sport.soccer;
                var locale = Opap.API.Locale.el;
                var date = DateTime.Parse(textDayBetMixGr.Text);
                var day = ServerIO.DownloadDay(sport, locale, date, WriteLogBetMixGr);
                var existing = ServerIO.ExistingForms(sport, locale, date);
                ListViewBetMix.Update(viewBetMixGr, day, existing);
            }
            catch
            {
                viewBetMixGr.Items.Clear();
            }
            finally
            {
                refreshBetMix.Enabled = true;
            }
        }
        private void textDayBetMixGr_TextChanged(object sender, EventArgs e)
        {
            this.RefreshBetMixGr();
        }
        private void refreshBetMix_Click(object sender, EventArgs e)
        {
            this.RefreshBetMixGr();
        }
        private void createBetMixGr_Click(object sender, EventArgs e)
        {
            createBetMixGr.Enabled = false;
            recreateBetMixGr.Enabled = false;
            var date = DateTime.Parse(textDayBetMixGr.Text);
            workerBetMixGr.RunWorkerAsync(new CreateArgument(date, false));
        }
        private void recreateBetMixGr_Click(object sender, EventArgs e)
        {
            createBetMixGr.Enabled = false;
            recreateBetMixGr.Enabled = false;
            var date = DateTime.Parse(textDayBetMixGr.Text);
            workerBetMixGr.RunWorkerAsync(new CreateArgument(date, true));
        }
        private void workerBetMixGr_DoWork(object sender, DoWorkEventArgs e)
        {
            var sport = Opap.API.Sport.soccer;
            var locale = Opap.API.Locale.el;
            var arg = (CreateArgument)e.Argument;
            var day = ServerIO.DownloadDay(sport, locale, arg.Date, WriteLogBetMixGr);
            this.WriteLogBetMixGr($"Day {day.Date:yyyy-MM-dd} fetched, has {day.Matches.Count} matches");
            //
            if (!arg.Refresh && ServerIO.BetMixExists(sport, locale, day.Date))
            {
                this.WriteLogBetMixGr($"BetMix exists {day.Date:yyyy-MM-dd}");
            }
            else
            {
                // create
                var element = BetMixMaker.CreateBetMix(day, this.WriteLogBetMixGr);
                // save
                ServerIO.UploadBetMix(sport, locale, day.Date, element);
                // log
                this.WriteLogBetMixGr($"BetMix saved {day.Date:yyyy-MM-dd}");
            }
        }
        private void workerBetMixGr_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        private void workerBetMixGr_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            createBetMixGr.Enabled = true;
            recreateBetMixGr.Enabled = true;
            this.RefreshBetMixGr();
        }
        private void textDayBetMixGr_MouseEnter(object sender, EventArgs e)
        {
            if (!calendarBetMixGr.Visible)
            {
                calendarBetMixGr.Left = textDayBetMixGr.Left;
                calendarBetMixGr.Top = textDayBetMixGr.Top + textDayBetMixGr.Height;
                calendarBetMixGr.Visible = true;
            }
        }
        private void calendarBetMixGr_DateSelected(object sender, DateRangeEventArgs e)
        {
            textDayBetMixGr.Text = calendarBetMixGr.SelectionRange.Start.ToString("yyyy-MM-dd");
            calendarBetMixGr.Visible = false;
        }
        private void calendarBetMixGr_MouseLeave(object sender, EventArgs e)
        {
            calendarBetMixGr.Visible = false;
        }
        private void WriteLogBetMixGr(string format, params object[] args)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    var message = string.Format(format, args);
                    logBetMixGr.Items.Insert(0, string.Format("{0:yyyy-MM-dd HH:mm:ss}# {1}", DateTime.Now, message));
                    if (logBetMixGr.Items.Count > 1000) logBetMixGr.Items.RemoveAt(1000);
                }));
            }
            catch { }
        }
        #endregion
        #region NotifyIcon
        private void notifyMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) ThisShowHide();
        }
        private void notifyMain_MouseClick(object sender, MouseEventArgs e)
        {
            ThisShowHide();
        }
        #endregion
    }
}
