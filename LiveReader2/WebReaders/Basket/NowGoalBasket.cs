using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace LiveReader2
{
    public class NowGoalBasket : WebReaderBase
    {
        #region Init
        const string URL_STRING = @"http://www.nowgoal.com/nba.htm";
        List<string> FinalStatusList = new List<string>() { "Finished" };
        public NowGoalBasket() : base(SourceName.NowGoal, SportName.Basket, URL_STRING) { }
        #endregion
        #region Methods
        public override int DoUpdate(HtmlElement body, ref  List<string> webIds)
        {
            var manager = new UpdateManager();
            try
            {
                var target = GetTargetElement(body, WriteLog);
                if (target != null)
                {
                    var elements = GetMatchElements(target, WriteLog);
                    if (elements != null)
                    {
                        string champ = null;
                        foreach (HtmlElement element in elements)
                        {
                            if (element.Id == null)
                            {
                                champ = GetChampName(element, WriteLog);
                            }
                            else
                            {
                                // fast read (only the webId)
                                var webId = GetWebId(element, WriteLog);
                                webIds.Add(webId);
                                // Get the current match if already exists (return null if not exists)
                                var curMatch = (Matches.ContainsKey(webId)) ? Matches[webId] : null;
                                // If current match not exists OR the it is live, make a full read
                                if (curMatch == null || MatchIsLive(curMatch))
                                {
                                    // Create new instances for the async task *** SOS ***
                                    var taskShowDetailedLog = this.ShowDetailedLog;
                                    var taskChamp = champ;
                                    // Add new task
                                    var task = Task.Run<NameValueCollection>(new Func<NameValueCollection>(() =>
                                    {
                                        var newMatch = ReadMatch(element, webId, taskChamp, WriteLog);
                                        return UpdateAndPostMatch(Sport, curMatch, newMatch, WriteLog, taskShowDetailedLog);
                                    }));
                                    manager.Tasks.Insert(0, task);
                                    // If not useing parallel processing
                                    if (!UseParallelProcessing)
                                    {
                                        // wait latest task to complete
                                        manager.Tasks[0].Wait();
                                        // Do a match break
                                        DoMatchBreak();
                                    }
                                }
                            }
                            // Partial collect & Upload
                            manager.PartialCollectAndUpload(SportName.Basket, WriteLog);
                        }
                        // Full collect & Upload
                        manager.FullCollectAndUpload(SportName.Basket, WriteLog);
                    }
                }
            }
            catch (Exception ex) { WriteLog("ERROR! DoUpdate: {0}", ex.Message); }
            return manager.Modified;
        }
        private bool MatchIsLive(NameValueCollection match)
        { // match is live if StartTime passed and status is not final yet.
            if (FinalStatusList.Contains(match["Status"])) return false;
            return StartTimePassed(match["StartTime"]);
        }
        #endregion
        #region Html Read
        private static NameValueCollection ReadMatch(HtmlElement element, string webId, string champ, ActionWriteLog WriteLog)
        { // DO NOT use WebMatch as param... it is CONFUSING!!!
            var match = new NameValueCollection();
            try
            {
                if (element != null && element.FirstChild != null && element.FirstChild.Children != null)
                {
                    var trCollection = element.FirstChild.Children;
                    if (trCollection.Count > 3)
                    {
                        match["WebId"] = webId;
                        match["Source"] = string.Format("{0}", SourceName.NowGoal);
                        match["Id"] = string.Format("{0}#{1}", match["Source"], match["WebId"]);
                        match["Champ"] = champ;
                        string[] DateAndTimeAndStatus = GetStartTimeAndStatus(trCollection[0], WriteLog);
                        match["StartTime"] = DateTime.Parse(GetStartTime(DateAndTimeAndStatus[0], WriteLog)).ToString("yyyy-MM-dd HH:mm:ss");
                        match["Status"] = DateAndTimeAndStatus[1];
                        match["StatusId"] = FixStatus(match["Status"], WriteLog).ToString();
                        match["Minute"] = DateAndTimeAndStatus[2];
                        string[] homeData = GetHomeTeamData(trCollection[1], WriteLog);
                        string[] awayData = GetAwayTeamData(trCollection[2], WriteLog);
                        match["HomeTeam"] = homeData[0];
                        match["AwayTeam"] = awayData[0];
                        match["StandingPoints"] = string.Format("{0}-{1}", homeData[1], awayData[1]);
                        match["ScoreQ1"] = string.Format("{0}-{1}", homeData[2], awayData[2]);
                        match["ScoreQ2"] = string.Format("{0}-{1}", homeData[3], awayData[3]);
                        match["ScoreQ3"] = string.Format("{0}-{1}", homeData[4], awayData[4]);
                        match["ScoreQ4"] = string.Format("{0}-{1}", homeData[5], awayData[5]);
                        match["Score"] = string.Format("{0}-{1}", homeData[6], awayData[6]);
                        match["ScoreHT"] = string.Format("{0}-{1}", homeData[7], awayData[7]);
                        match["HomeScored"] = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
                        match["AwayScored"] = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
                        match["Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
            }
            catch (Exception ex) { WriteLog("ERROR! ReadMatch: {0}", ex.Message); }
            return match;
        }
        private static HtmlElement GetTargetElement(HtmlElement body, ActionWriteLog WriteLog)
        {
            try
            {
                if (body != null)
                {
                    var target = body.Document.GetElementById("live");
                    if (target != null)
                    {
                        return target;
                    }
                }
            }
            catch (Exception ex) { WriteLog("ERROR! GetTargetElement: {0}", ex.Message); }
            return null;
        }
        private static HtmlElementCollection GetMatchElements(HtmlElement target, ActionWriteLog WriteLog)
        {
            try
            {
                return target.Children;
            }
            catch (Exception ex) { WriteLog("ERROR! GetMatchElements: {0}", ex.Message); }
            return null;
        }
        private static string GetWebId(HtmlElement element, ActionWriteLog WriteLog)
        {
            var webId = "";
            try
            {
                webId = element.Id.Split('_')[1];
            }
            catch (Exception ex) { WriteLog("ERROR! GetWebId: {0}", ex.Message); }
            return webId;
        }
        private static string GetChampName(HtmlElement element, ActionWriteLog WriteLog)
        {
            try
            {
                return element.FirstChild.FirstChild.FirstChild.FirstChild.FirstChild.FirstChild.InnerText.Trim();
            }
            catch (Exception ex) { WriteLog("ERROR! GetChampName: {0}", ex.Message); }
            return "";
        }
        private static string[] GetStartTimeAndStatus(HtmlElement tr, ActionWriteLog WriteLog)
        {
            string StartTime = "", Status = "", Minute = "";
            try
            {
                HtmlElement td = tr.FirstChild;
                string[] Text = td.InnerText.Trim().Split(' ');
                int i = Text.Length;
                StartTime = FillNowDate(Text[0], WriteLog) + " " + Text[1];
                if (i > 2)
                {
                    if (Text[i - 1].Contains(':') || Text[i - 1].Contains('.'))
                    {
                        Minute = Text[i - 1];
                        Status = string.Join(" ", Text, 2, i - 3).Trim();
                    }
                    else
                    {
                        Status = string.Join(" ", Text, 2, i - 2).Trim();
                    }
                }
            }
            catch (Exception ex) { WriteLog("ERROR! GetStartTimeAndStatus: {0}", ex.Message); }
            return new string[] { StartTime, Status, Minute };
        }
        private static string GetStartTime(string s, ActionWriteLog WriteLog)
        {
            var startTime = DateTime.MinValue;
            try
            {
                startTime = DateTime.Parse(s);
                if (startTime.Minute == 59) startTime = startTime.AddMinutes(1);
            }
            catch (Exception ex) { WriteLog("ERROR! GetDate: {0}", ex.Message); }
            return startTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        private static string[] GetHomeTeamData(HtmlElement tr, ActionWriteLog WriteLog)
        {
            string Name = "", Points = "", Quarter1 = "", Quarter2 = "", Quarter3 = "", Quarter4 = "", ScoreOT = "", Score = "", ScoreHT = "";
            try
            {
                var tdCollection = tr.GetElementsByTagName("td");
                var i = tdCollection.Count;
                var s = GetNameAndPoints(GetInnerText(tdCollection[1].FirstChild, WriteLog), WriteLog);
                Name = s[0];
                Points = s[1];
                if (i == 15)
                {
                    Quarter1 = GetInnerText(tdCollection[2], WriteLog);
                    Quarter2 = GetInnerText(tdCollection[3], WriteLog);
                    Quarter3 = GetInnerText(tdCollection[4], WriteLog);
                    Quarter4 = GetInnerText(tdCollection[5], WriteLog);
                    ScoreOT = GetInnerText(tdCollection[6], WriteLog);
                    Score = GetInnerText(tdCollection[7], WriteLog);
                    ScoreHT = GetInnerText(tdCollection[8].FirstChild, WriteLog);
                }
                else if (i == 14)
                {
                    Quarter1 = GetInnerText(tdCollection[2], WriteLog);
                    Quarter2 = GetInnerText(tdCollection[3], WriteLog);
                    Quarter3 = GetInnerText(tdCollection[4], WriteLog);
                    Quarter4 = GetInnerText(tdCollection[5], WriteLog);
                    Score = GetInnerText(tdCollection[6], WriteLog);
                    ScoreHT = GetInnerText(tdCollection[7].FirstChild, WriteLog);
                }
                else
                {
                    Quarter1 = GetInnerText(tdCollection[2], WriteLog);
                    Quarter3 = GetInnerText(tdCollection[3], WriteLog);
                    Score = GetInnerText(tdCollection[4], WriteLog);
                    ScoreHT = GetInnerText(tdCollection[5].FirstChild, WriteLog);
                }
            }
            catch (Exception ex) { WriteLog("ERROR! GetHomeTeamData: {0}", ex.Message); }
            return new string[] { Name, Points, Quarter1, Quarter2, Quarter3, Quarter4, Score, ScoreHT };
        }
        private static string[] GetAwayTeamData(HtmlElement tr, ActionWriteLog WriteLog)
        {
            string Name = "", Points = "", Quarter1 = "", Quarter2 = "", Quarter3 = "", Quarter4 = "", ScoreOT = "", Score = "", ScoreHT = "";
            try
            {
                var tdCollection = tr.GetElementsByTagName("td");
                var i = tdCollection.Count;
                var s = GetNameAndPoints(GetInnerText(tdCollection[0].FirstChild, WriteLog), WriteLog);
                Name = s[0];
                Points = s[1];
                if (i == 13)
                {
                    Quarter1 = GetInnerText(tdCollection[1], WriteLog);
                    Quarter2 = GetInnerText(tdCollection[2], WriteLog);
                    Quarter3 = GetInnerText(tdCollection[3], WriteLog);
                    Quarter4 = GetInnerText(tdCollection[4], WriteLog);
                    ScoreOT = GetInnerText(tdCollection[5], WriteLog);
                    Score = GetInnerText(tdCollection[6], WriteLog);
                    ScoreHT = GetInnerText(tdCollection[7].FirstChild, WriteLog);
                }
                else if (i == 12)
                {
                    Quarter1 = GetInnerText(tdCollection[1], WriteLog);
                    Quarter2 = GetInnerText(tdCollection[2], WriteLog);
                    Quarter3 = GetInnerText(tdCollection[3], WriteLog);
                    Quarter4 = GetInnerText(tdCollection[4], WriteLog);
                    Score = GetInnerText(tdCollection[5], WriteLog);
                    ScoreHT = GetInnerText(tdCollection[6].FirstChild, WriteLog);
                }
                else
                {
                    Quarter1 = GetInnerText(tdCollection[1], WriteLog);
                    Quarter3 = GetInnerText(tdCollection[2], WriteLog);
                    Score = GetInnerText(tdCollection[3], WriteLog);
                    ScoreHT = GetInnerText(tdCollection[4].FirstChild, WriteLog);
                }
            }
            catch (Exception ex) { WriteLog("ERROR! GetAwayTeamData: {0}", ex.Message); }
            return new string[] { Name, Points, Quarter1, Quarter2, Quarter3, Quarter4, Score, ScoreHT };
        }
        private static string FillNowDate(string MM_dd, ActionWriteLog WriteLog)
        {
            try
            {
                string today = DateTime.Today.ToString("yyyy-MM-dd");
                string tomorow = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");
                string yesterday = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
                if (today.Contains(MM_dd)) return today;
                if (tomorow.Contains(MM_dd)) return tomorow;
                if (yesterday.Contains(MM_dd)) return yesterday;
            }
            catch (Exception ex) { WriteLog("ERROR! FillNowDate: {0}", ex.Message); }
            return "";
        }
        private static string GetInnerText(HtmlElement element, ActionWriteLog WriteLog)
        {
            try
            {
                if (element != null && element.InnerText != null) { return element.InnerText.Trim(); }
            }
            catch (Exception ex) { WriteLog("ERROR! GetInnerText: {0}", ex.Message); }
            return "";
        }
        private static string[] GetNameAndPoints(string name, ActionWriteLog WriteLog)
        {
            try
            {
                if (name.EndsWith("]"))
                {
                    string[] s = name.Split('[');
                    return new string[] { s[0], s[1].Replace("]", "") };
                }
            }
            catch (Exception ex) { WriteLog("ERROR! GetNameAndPoints: {0}", ex.Message); }
            return new string[] { name, "" };
        }
        private static MatchStatus FixStatus(string status, ActionWriteLog WriteLog)
        {
            try
            {
                switch (status.Trim())
                {
                    case "": return MatchStatus.Waiting;
                    case "1st Qtr": return MatchStatus.Qrt1;
                    case "2nd Qtr": return MatchStatus.Qrt2;
                    case "3rd Qtr": return MatchStatus.Qrt3;
                    case "4th Qtr": return MatchStatus.Qrt4;
                    case "Half Time": return MatchStatus.HT;
                    case "Hafe Time": return MatchStatus.HT; // *** yes hafe time is written!!! ***
                    case "Finished": return MatchStatus.FT;
                    case "Pause": return MatchStatus.Pause;
                    case "1OT": return MatchStatus.FT;
                    case "Undecided": return MatchStatus.Cancelled;
                    case "undefined": return MatchStatus.Unknown;
                    default:
                        WriteLog("Unknown status = {0}", status);
                        break;
                }
            }
            catch (Exception ex) { WriteLog("ERROR! FixStatus: {0} [status = {1}]", ex.Message, status); }
            return MatchStatus.Unknown;
        }
        #endregion
    }
}
