using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace LiveReader2
{
    public class Futbol24Football : WebReaderBase
    {
        #region Init
        const string URL_STRING = @"http://www.futbol24.com/Live/";
        List<string> FinalStatusList = new List<string>() { "FT", "AET", "Postp."};
        public Futbol24Football() : base(SourceName.Futbol24, SportName.Football, URL_STRING) { }
        #endregion
        #region Methods
        public override int DoUpdate(HtmlElement body, ref List<string> webIds)
        {
            var manager = new UpdateManager();
            try
            {
                var date = GetDate(body, WriteLog);
                var tablelive = GetTargetElement(body, "table", "f24com_tablelive", WriteLog);
                if (tablelive != null)
                {
                    foreach (HtmlElement element in tablelive.FirstChild.Children)
                    {
                        var task = this.ReadMatchElement(element, date, ref  webIds);
                        if (task != null)
                        {
                            manager.Tasks.Insert(0, task);
                            // If not using parallel processing
                            if (!UseParallelProcessing)
                            {
                                // Wait latest task to complete
                                manager.Tasks[0].Wait();
                                // Do a match break
                                DoMatchBreak();
                            }
                        }
                        // Partial collect & Upload
                        manager.PartialCollectAndUpload(SportName.Football, WriteLog);
                    }
                }
                var tableresults = GetTargetElement(body, "table", "f24com_tableresults", WriteLog);
                if (tableresults != null)
                {
                    foreach (HtmlElement element in tableresults.Children[1].Children)
                    {
                        var task = this.ReadMatchElement(element, date, ref  webIds);
                        if (task != null)
                        {
                            manager.Tasks.Insert(0, task);
                            // If not using parallel processing
                            if (!UseParallelProcessing)
                            {
                                // Wait latest task to complete
                                manager.Tasks[0].Wait();
                                // Do a match break
                                DoMatchBreak();
                            }
                        }
                        // Partial collect & Upload
                        manager.PartialCollectAndUpload(SportName.Football, WriteLog);
                    }
                }
                // Full collect & Upload
                manager.FullCollectAndUpload(SportName.Football, WriteLog);
            }
            catch (Exception ex) { WriteLog("ERROR! DoUpdate: {0}", ex.Message); }
            return manager.Modified;
        }
        private Task<NameValueCollection> ReadMatchElement(HtmlElement element, DateTime date, ref  List<string> webIds)
        {
            Task<NameValueCollection> task = null;
            try
            {
                if (element.Children.Count >= 6) // Match row
                {
                    // Fast read (only the webId)
                    var webId = GetWebId(element, WriteLog);
                    webIds.Add(webId);
                    // Get the current match if already exists (return null if not exists)
                    var curMatch = (Matches.ContainsKey(webId)) ? Matches[webId] : null;
                    // If current match not exists OR it is live, make a full read
                    if (curMatch == null || MatchIsLive(curMatch))
                    {
                        // Create new instances for the async task *** SOS ***
                        var taskShowDetailedLog = this.ShowDetailedLog;
                        // Add new task
                        task = Task.Run<NameValueCollection>(new Func<NameValueCollection>(() =>
                        {
                            var newMatch = ReadMatch(element, webId, date, WriteLog);
                            return UpdateAndPostMatch(Sport, curMatch, newMatch, WriteLog, taskShowDetailedLog);
                        }));
                    }
                }
            }
            catch (Exception ex) { WriteLog("ERROR! ReadMatchElement: {0}", ex.Message); }
            return task;
        }
        private bool MatchIsLive(NameValueCollection match)
        {
            var startTime = DateTimeParse(match["StartTime"]);
            // if status is final
            if (FinalStatusList.Contains(match["Status"]))
            {
                // match is not live, but keep update it until 3 hours from start time
                if (startTime.AddHours(3) > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // if has no start time, match is live
            if (startTime == DateTime.MinValue) return true;
            // if StartTime passed, match is live
            return StartTimePassed(match["StartTime"]);
        }
        private DateTime DateTimeParse(string datetime)
        {
            try
            {
                return DateTime.Parse(datetime);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        #endregion
        #region Html Read
        private static HtmlElement GetTargetElement(HtmlElement parentElement, string tagName, string elementId, ActionWriteLog WriteLog)
        {
            try
            {
                if (parentElement != null)
                {
                    var query = from HtmlElement element in parentElement.GetElementsByTagName(tagName)
                                where element.Id == elementId
                                select element;
                    if (query != null && query.Any())
                    {
                        return query.First();
                    }
                }
            }
            catch (Exception ex) { WriteLog("ERROR! GetTargetElement: {0}", ex.Message); }
            return null;
        }
        private static DateTime GetDate(HtmlElement body, ActionWriteLog WriteLog)
        {
            try
            {
                if (body != null)
                {
                    var query = from HtmlElement element in body.GetElementsByTagName("div")
                                where element.Id == "f24com_date"
                                select element;
                    if (query != null && query.Any())
                    {
                        var dateText = query.First().FirstChild.InnerText;
                        if (dateText != null && dateText.Length > 0)
                        {
                            //var date = DateTime.ParseExact(dateText, "dd.MM.yyyy", System.Globalization.CultureInfo.CurrentCulture); // <= Wrong! After midnight we have wrong day!
                            var date = Opap.CurrentDay;
                            return date;
                        }
                    }
                }
            }
            catch (Exception ex) { WriteLog("ERROR! GetDate: {0}", ex.Message); }
            return Opap.CurrentDay;
        }
        private static string GetWebId(HtmlElement element, ActionWriteLog WriteLog)
        {
            try
            {
                string s = element.Id.Split('_')[2];
                return s;
            }
            catch (Exception ex) { WriteLog("ERROR! GetWebId: {0}", ex.Message); }
            return "";
        }
        private static NameValueCollection ReadMatch(HtmlElement element, string webId, DateTime date, ActionWriteLog WriteLog)
        { // DO NOT use WebMatch as param... it is CONFUSING!!!
            var match = new NameValueCollection();
            try
            {
                if (match != null && element != null && element.Children != null)
                {
                    var tdCollection = element.Children;
                    if (tdCollection.Count >= 6)
                    {
                        match["WebId"] = webId;
                        match["Source"] = string.Format("{0}", SourceName.Futbol24);
                        match["Id"] = string.Format("{0}#{1}", match["Source"], match["WebId"]);
                        match["Champ"] = tdCollection[1].FirstChild.InnerText.Trim();
                        match["StartTime"] = $"{date:yyyy-MM-dd} 08:00:00"; // default
                        var tdStartTime = tdCollection[2].InnerText.Trim();
                        var minute = "";
                        var status = MatchStatus.Unknown;
                        if (tdStartTime.Contains(":")) // it is start time
                        {
                            var starttime = FixStartTime(tdStartTime, date, WriteLog);
                            match["StartTime"] = starttime.ToString("yyyy-MM-dd HH:mm:ss");
                            status = MatchStatus.Waiting;
                        }
                        else if (tdStartTime.Contains("'")) // it is minute
                        {
                            minute = FixMinute(tdStartTime, WriteLog);
                            status = FixStatusByMinute(minute, WriteLog);
                        }
                        else // it is status
                        {
                            status = FixStatus(tdStartTime, WriteLog);
                        }
                        match["Minute"] = minute;
                        match["Status"] = tdStartTime;
                        match["StatusId"] = status.ToString();
                        match["HomeTeam"] = GetInnerTextByClassName(tdCollection[3], "team", WriteLog);
                        if (tdCollection[4].Children.Count == 2)
                        {
                            match["ScoreHT"] = GetInnerTextByClassName(tdCollection[4], "result2", WriteLog).Replace("(", "").Replace(")", "").Trim();
                            if (match["ScoreHT"] == "-") match["ScoreHT"] = ""; // <=***
                            if (match["ScoreHT"].Contains(','))
                            {
                                var s = match["ScoreHT"].Split(',');
                                match["ScoreHT"] = s[0];
                                match["Score"] = s[1];
                                match["Status"] = "FT";
                                match["StatusId"] = MatchStatus.FT.ToString();
                            }
                            else
                            {
                                match["Score"] = GetInnerTextByClassName(tdCollection[4], "result1", WriteLog);
                            }
                        }
                        else if (tdCollection[4].Children.Count == 1)
                        {
                            match["Score"] = GetInnerTextByClassName(tdCollection[4], "result", WriteLog).Replace("w.o.", "").Trim();
                        }
                        match["AwayTeam"] = GetInnerTextByClassName(tdCollection[5], "team", WriteLog);
                        match["HomeScored"] = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
                        match["AwayScored"] = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
                        match["Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
            }
            catch (Exception ex) { WriteLog("ERROR! ReadMatch: {0}", ex.Message); }
            return match;
        }
        private static DateTime FixStartTime(string text, DateTime date, ActionWriteLog WriteLog)
        {
            var startTime = new DateTime(date.Year, date.Month, date.Day, 8, 0, 0);
            try
            {
                var s = text.Split(':');
                if (s.Length == 2)
                {
                    var hour = int.Parse(s[0]);
                    var minute = int.Parse(s[1]);
                    startTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
                    if (hour >= 8)
                    {
                        return startTime;
                    }
                    else
                    {
                        return startTime.AddDays(1);
                    }
                }
            }
            catch (Exception ex) { WriteLog("ERROR! FixStartTime: {0}", ex.Message); }
            return startTime;
        }
        private static string FixMinute(string text, ActionWriteLog WriteLog)
        {
            try
            {
                var s = text.Replace("'", "").Trim();
                if (s != "")
                {
                    return s;
                }
            }
            catch (Exception ex) { WriteLog("ERROR! FixMinute: {0}", ex.Message); }
            return "";
        }
        private static MatchStatus FixStatus(string text, ActionWriteLog WriteLog)
        {
            var status = MatchStatus.Unknown;
            try
            {
                switch (text)
                {
                    case "": status = MatchStatus.Waiting; break;
                    case "HT": status = MatchStatus.HT; break;
                    case "FT": status = MatchStatus.FT; break;
                    case "AP": status = MatchStatus.FT; break;
                    case "Part1": status = MatchStatus.Half1; break;
                    case "Half": status = MatchStatus.HT; break;
                    case "Part2": status = MatchStatus.Half2; break;
                    case "Ot": status = MatchStatus.OT; break;
                    case "Postp.": status = MatchStatus.Cancelled; break;
                    case "Cancel": status = MatchStatus.Cancelled; break;
                    case "ABD": status = MatchStatus.Cancelled; break;
                    case "Pause": status = MatchStatus.Pause; break;
                    case "Pend.": status = MatchStatus.Cancelled; break;
                    case "CANC": status = MatchStatus.Cancelled; break;
                    case "P-P": status = MatchStatus.Cancelled; break;
                    case "undefined": status = MatchStatus.Unknown; break;
                    case "W.O.": status = MatchStatus.FT; break;
                    case "Pen.": status = MatchStatus.FT; break;
                    case "FT Only": status = MatchStatus.Waiting; break;
                    case "ANL": status = MatchStatus.Unknown; break;
                    default: WriteLog("ERROR! Unknown status = {0}", text); break;
                }
            }
            catch (Exception ex) { WriteLog("ERROR! FixStatus: {0}", ex.Message); }
            return status;
        }
        private static MatchStatus FixStatusByMinute(string minute, ActionWriteLog WriteLog)
        {
            try
            {
                int min = int.Parse(minute.Replace("+", ""));
                if (min > 0)
                {
                    if (min <= 45) return MatchStatus.Half1;
                    if (min >= 46) return MatchStatus.Half2;
                }
            }
            catch (Exception ex) { WriteLog("ERROR! FixStatusByMinute: {0}", ex.Message); }
            return MatchStatus.Unknown;
        }
        private static string GetInnerTextByClassName(HtmlElement currentElement, string className, ActionWriteLog WriteLog)
        {
            try
            {
                foreach (HtmlElement element in currentElement.Children)
                {
                    if (element.GetAttribute("classname").Contains(className))
                    {
                        return element.InnerText.Trim();
                    }
                }
            }
            catch { }
            return "";
        }
        #endregion
    }
}
