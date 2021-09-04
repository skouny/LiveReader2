using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;
using System.Xml.Linq;

namespace LiveReader2
{
    public class NowGoalFootball : WebReaderBase
    {
        #region Init
        const string URL_STRING = @"http://www.nowgoal.com/?type=all";
        List<string> FinalStatusList = new List<string>() { "FT", "Pend.", "Abd" };
        public NowGoalFootball() : base(SourceName.NowGoal, SportName.Football, URL_STRING)
        {
            
        }
        #endregion
        #region Methods
        public override int DoUpdate(HtmlElement body, ref List<string> webIds)
        {
            var manager = new UpdateManager();
            try
            {
                var target = GetTargetElement(body, WriteLog);
                if (target != null)
                {
                    // Get the match list
                    var elements = GetMatchElements(target, WriteLog);
                    if (elements != null)
                    {
                        //var tasks = new List<Task<NameValueCollection>>();
                        var startDate = GetDate(elements[1], WriteLog);
                        var curDate = startDate;
                        for (int i = 2; i < elements.Count; i++)
                        {
                            int tdCount = elements[i].Children.Count;
                            if (tdCount == 1) // Results or Date
                            {
                                string firstChildText = elements[i].FirstChild.InnerText;
                                if (firstChildText != null && firstChildText.Trim().Length > 0)
                                {
                                    // Results
                                    if (firstChildText.Trim() == "Results")
                                    {
                                        if (curDate == startDate && curDate.Hour < 7)
                                        {
                                            curDate = startDate.AddDays(-1);
                                        }
                                        else
                                        {
                                            curDate = startDate;
                                        }
                                    }
                                    // Date
                                    else if (firstChildText.Contains("/"))
                                    {
                                        curDate = GetDate(elements[i], WriteLog);
                                    }
                                }
                            }
                            else if (tdCount == 2) // Date (or empty) row
                            {
                                string firstChildText = elements[i].FirstChild.InnerText;
                                if (firstChildText != null && firstChildText.Trim().Length > 0)
                                {
                                    curDate = GetDate(elements[i], WriteLog);
                                }
                            }
                            else if (tdCount >= 8) // Match row
                            {
                                var element = elements[i];
                                // Fast read (only the webId)
                                var webId = GetWebId(element, WriteLog);
                                webIds.Add(webId);
                                // Get the current match if already exists (return null if not exists)
                                var curMatch = (Matches.ContainsKey(webId)) ? Matches[webId] : null;
                                // If current match not exists OR the it is live, make a full read
                                if (curMatch == null || MatchIsLive(curMatch))
                                {
                                    // Create new instances for the async task *** SOS ***
                                    var taskShowDetailedLog = this.ShowDetailedLog;
                                    var taskDate = curDate;
                                    // Add new task
                                    var task = Task.Run<NameValueCollection>(new Func<NameValueCollection>(() =>
                                    {
                                        var newMatch = ReadMatch(element, webId, taskDate, WriteLog);
                                        return UpdateAndPostMatch(Sport, curMatch, newMatch, WriteLog, taskShowDetailedLog);
                                    }));
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
                            }
                            // Partial collect & Upload
                            manager.PartialCollectAndUpload(SportName.Football, WriteLog);
                        }
                        // Full collect & Upload
                        manager.FullCollectAndUpload(SportName.Football, WriteLog);
                    }
                }
            }
            catch (Exception ex) { WriteLog("ERROR! DoUpdate: {0}", ex.Message); }
            return manager.Modified;
        }
        private bool MatchIsLive(NameValueCollection match)
        {
            // if status is final
            if (FinalStatusList.Contains(match["Status"]))
            {
                // match is not live, but keep update it until 3 hours from start time
                if (DateTime.Parse(match["StartTime"]).AddHours(3) > DateTime.Now)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return StartTimePassed(match["StartTime"]);
        }
        #endregion
        #region Html Read
        private static HtmlElement GetTargetElement(HtmlElement body, ActionWriteLog WriteLog)
        {
            try
            {
                if (body != null)
                {
                    var target = body.Document.GetElementById("table_live");
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
                return target.FirstChild.Children;
            }
            catch (Exception ex) { WriteLog("ERROR! GetMatchElements: {0}", ex.Message); }
            return null;
        }
        private static string GetWebId(HtmlElement element, ActionWriteLog WriteLog)
        {
            try
            {
                string s = element.Id.Split('_')[1];
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
                    if (tdCollection.Count > 7)
                    {
                        match["WebId"] = webId;
                        match["Source"] = string.Format("{0}", SourceName.NowGoal);
                        match["Id"] = string.Format("{0}#{1}", match["Source"], match["WebId"]);
                        match["Champ"] = tdCollection[1].InnerText.Trim();
                        match["StartTime"] = GetStartTime(tdCollection[2], date, WriteLog).ToString("yyyy-MM-dd HH:mm:ss");
                        match["Status"] = tdCollection[3].InnerText.Trim();
                        object[] s = FixStatus(match["Status"], WriteLog);
                        var status = (MatchStatus)s[0];
                        match["StatusId"] = string.Format("{0}", status);
                        match["Minute"] = (string)s[1];
                        string[] home = GetTeam(tdCollection[4], WriteLog);
                        string[] away = GetTeam(tdCollection[6], WriteLog);
                        match["HomeTeam"] = home[0];
                        match["AwayTeam"] = away[0];
                        match["YellowCards"] = string.Format("{0}-{1}", home[1], away[1]);
                        match["RedCards"] = string.Format("{0}-{1}", home[2], away[2]);
                        if (status != MatchStatus.Cancelled && status != MatchStatus.Abd)
                        {
                            match["Score"] = GetScore(tdCollection[5].InnerText, WriteLog);
                            match["ScoreHT"] = GetScore(tdCollection[7].Children[1].InnerText, WriteLog);
                        }
                        //NowGoalFootballDetails.Read(webId, ref match, WriteLog); // <= VEEEEEEEEEERY slow!!
                        match["HomeScored"] = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
                        match["AwayScored"] = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
                        match["Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
            }
            catch (Exception ex) { WriteLog("ERROR! ReadMatch: {0}", ex.Message); }
            return match;
        }
        private static DateTime GetStartTime(HtmlElement td, DateTime date, ActionWriteLog WriteLog)
        {
            var startTime = DateTime.MinValue;
            try
            {
                var time = td.InnerText.Trim();
                string s = string.Format("{0:yyyy-MM-dd} {1}", date, time);
                startTime = DateTime.Parse(s);
                if (startTime.Minute == 59) startTime = startTime.AddMinutes(1);
                return startTime;
            }
            catch (Exception ex) { WriteLog("ERROR! GetStartTime: {0}", ex.Message); }
            return startTime;
        }
        private static string GetScore(string s, ActionWriteLog WriteLog)
        {
            string score = string.Empty;
            try
            {
                score = s?.Replace(" ", string.Empty).Trim();
                if (score?.Length < 3) score = string.Empty;
            }
            catch (Exception ex) { WriteLog("ERROR! GetScore: {0}", ex.Message); }
            return score;
        }
        private static int GetHour24(string time, ActionWriteLog WriteLog)
        {
            try
            {
                return int.Parse(time.Split(':')[0]);
            }
            catch (Exception ex) { WriteLog("ERROR! GetHour24: {0}", ex.Message); }
            return 24;
        }
        private static string GetDateText(HtmlElement tr, ActionWriteLog WriteLog)
        {
            try
            {
                var text = tr.FirstChild.InnerText.Split(' ')[0].Trim();
                if (text == "Results")
                {
                    return string.Format("{0:M/d/yyyy}", DateTime.Now);
                }
                else
                {
                    return text;
                }
            }
            catch (Exception ex) { WriteLog("ERROR! GetDateText: {0}", ex.Message); }
            return null;
        }
        private static DateTime GetDate(string strDate, ActionWriteLog WriteLog)
        {
            try
            {
                return DateTime.ParseExact(strDate, "M/d/yyyy",
                    System.Globalization.CultureInfo.CurrentCulture);
            }
            catch (Exception ex) { WriteLog("ERROR! GetDate: {0}", ex.Message); }
            return DateTime.MinValue;
        }
        private static DateTime GetDate(HtmlElement tr, ActionWriteLog WriteLog)
        {
            var text = GetDateText(tr, WriteLog);
            return GetDate(text, WriteLog);
        }
        private static string[] GetTeam(HtmlElement td, ActionWriteLog WriteLog)
        {
            string Name = "", YellowCards = "", RedCards = "";
            try
            {
                var aCollection = td.Children;
                foreach (HtmlElement a in aCollection)
                {
                    if (a.Id.StartsWith("team"))
                    {
                        Name = a.InnerText.Replace("(N)", "");
                    }
                    else if (a.Id.StartsWith("yellow"))
                    {
                        var imgCollection = a.Children;
                        if (imgCollection.Count > 0)
                        {
                            YellowCards = imgCollection[0].GetAttribute("src").Replace(@"http://www.nowgoal.com/images/yellow", "").Replace(@"http://www.nowgoal.com/images/yellow", "").Replace(".gif", "");
                        }
                        else
                        {
                            YellowCards = "";
                        }
                    }
                    else if (a.Id.StartsWith("redcard"))
                    {
                        var imgCollection = a.Children;
                        if (imgCollection.Count > 0)
                        {
                            RedCards = imgCollection[0].GetAttribute("src").Replace(@"http://www.nowgoal.com/images/redcard", "").Replace(@"http://www.nowgoal.com/images/redcard", "").Replace(".gif", "");
                        }
                        else
                        {
                            RedCards = "";
                        }
                    }
                }
            }
            catch (Exception ex) { WriteLog("ERROR! GetTeam: {0}", ex.Message); }
            return new string[] { Name, YellowCards, RedCards };
        }
        private static object[] FixStatus(string value, ActionWriteLog WriteLog)
        {
            var Status = MatchStatus.Unknown;
            var Minute = "";
            try
            {
                switch (value)
                {
                    case "": Status = MatchStatus.Waiting; break;
                    case "HT": Status = MatchStatus.HT; break;
                    case "FT": Status = MatchStatus.FT; break;
                    case "Part1": Status = MatchStatus.Half1; break;
                    case "Half": Status = MatchStatus.HT; break;
                    case "Part2": Status = MatchStatus.Half2; break;
                    case "Ot": Status = MatchStatus.OT; break;
                    case "Postp.": Status = MatchStatus.Cancelled; break;
                    case "Cancel": Status = MatchStatus.Cancelled; break;
                    case "Abd": Status = MatchStatus.Abd; break;
                    case "Pause": Status = MatchStatus.Pause; break;
                    case "Pend.": Status = MatchStatus.Cancelled; break;
                    case "undefined": Status = MatchStatus.Unknown; break;
                    default:
                        int minute = GetMinuteInt(value, WriteLog);
                        if (minute > 0)
                        {
                            if (minute <= 45) Status = MatchStatus.Half1;
                            if (minute >= 46) Status = MatchStatus.Half2;
                            Minute = minute.ToString();
                        }
                        else
                        {
                            WriteLog("ERROR! Unknown status = {0}", value);
                        }
                        break;
                }
            }
            catch (Exception ex) { WriteLog("ERROR! FixStatus: {0}", ex.Message); }
            return new object[] { Status, Minute };
        }
        private static int GetMinuteInt(string status, ActionWriteLog WriteLog)
        {
            string s = "";
            try
            {
                s = status.Replace("+", "");
                if (s != "")
                {
                    return int.Parse(s);
                }
            }
            catch (Exception ex) { WriteLog("ERROR! GetMinuteInt: {0}", ex.Message); }
            return 0;
        }
        #endregion
    }
    public class NowGoalFootballDetails
    {
        #region Methods
        public static void Read(string webId, ref NameValueCollection match, ActionWriteLog WriteLog)
        {
            try
            {
                var htmlDoc = new HtmlAgilityPack.HtmlDocument(); // [ http://htmlagilitypack.codeplex.com/ ]
                var url = $@"http://www.nowgoal.com/detail/{webId}.html";
                var html_code = TimedWebClient.DownloadHtml(url);
                if (html_code != null && html_code.Length > 0)
                {
                    htmlDoc.LoadHtml(html_code);
                    var document = htmlDoc.DocumentNode;
                    var html = document.ChildNodes.FindFirst("html");
                    var body = html.ChildNodes.FindFirst("body");
                    var div_main = body.ChildNodes.Where(x => x.Id == "main").First();
                    var div_match_data = div_main.ChildNodes.Where(x => x.Id == "matchData").First();
                    foreach (var element in div_match_data.Elements("div"))
                    {
                        var table = element.Element("table");
                        if (table != null)
                        {
                            var tr = table.Element("tr");
                            if (tr != null)
                            {
                                var th = tr.Element("th");
                                if (th != null)
                                {
                                    var title = th.InnerText;
                                    switch (title)
                                    {
                                        case "Key Events":
                                            ReadEvents(table, ref match, WriteLog);
                                            break;
                                        case "Tech Statistics":
                                            ReadStatistics(table, ref match, WriteLog);
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { WriteLog("ERROR! DownloadDetails: {0}", ex.Message); }
        }
        #endregion
        #region Html Read
        private static void ReadEvents(HtmlAgilityPack.HtmlNode table, ref NameValueCollection match, ActionWriteLog WriteLog)
        {
            try
            {
                var HomeEvents = new List<XElement>();
                var AwayEvents = new List<XElement>();
                foreach (HtmlAgilityPack.HtmlNode tr in table.Elements("tr"))
                {
                    var tds = tr.Elements("td");
                    if (tds != null && tds.Count() >= 5)
                    {
                        var td_array = tds.ToArray();
                        var valueHome = ReadEventValue(td_array[0], WriteLog);
                        var typeHome = ReadEventType(td_array[1], WriteLog);
                        var minute = ReadMinute(td_array[2], WriteLog);
                        var typeAway = ReadEventType(td_array[3], WriteLog);
                        var valueAway = ReadEventValue(td_array[4], WriteLog);
                        if (minute > 0)
                        {
                            if (typeHome.Length > 0)
                            {
                                HomeEvents.Add(new XElement("Event", new XAttribute("Minute", minute), new XAttribute("Type", typeHome), new XAttribute("Value", valueHome)));
                            }
                            if (typeAway.Length > 0)
                            {
                                AwayEvents.Add(new XElement("Event", new XAttribute("Minute", minute), new XAttribute("Type", typeAway), new XAttribute("Value", valueAway)));
                            }
                        }
                    }
                }
                match["HomeEvents"] = XElementListToString(HomeEvents);
                match["AwayEvents"] = XElementListToString(AwayEvents);
            }
            catch (Exception ex) { WriteLog("ERROR! ReadEvents: {0}", ex.Message); }
        }
        private static void ReadStatistics(HtmlAgilityPack.HtmlNode table, ref NameValueCollection match, ActionWriteLog WriteLog)
        {
            try
            {
                foreach (var tr in table.Elements("tr"))
                {
                    var tds = tr.Elements("td").ToArray();
                    if (tds != null && tds.Count() >= 5) // title line
                    {
                        var valHome = System.Net.WebUtility.HtmlDecode(tds[1].InnerText).Trim();
                        var valName = System.Net.WebUtility.HtmlDecode(tds[2].InnerText).Trim();
                        var valAway = System.Net.WebUtility.HtmlDecode(tds[3].InnerText).Trim();
                        switch (valName)
                        {
                            case "Tech Statistics": // Title row
                                break;
                            case "Kick-off":
                                match["KickOff"] = string.Format("{0}-{1}", valHome, valAway);
                                break;
                            case "Possession":
                                match["BallPossession"] = string.Format("{0}-{1}", valHome, valAway);
                                break;
                            case "Fouls":
                                match["Fouls"] = string.Format("{0}-{1}", valHome, valAway);
                                break;
                            case "Shots":
                                match["Shots"] = string.Format("{0}-{1}", valHome, valAway);
                                break;
                            case "Shots on Goal":
                                match["ShotsOnGoal"] = string.Format("{0}-{1}", valHome, valAway);
                                break;
                            case "Off Target": break;
                            case "Hit the post": break;
                            case "Corner Kicks":
                                match["CornerKicks"] = string.Format("{0}-{1}", valHome, valAway);
                                break;
                            case "Corner Kicks(Extra time)":  break;
                            case "Offsides":
                                match["Offsides"] = string.Format("{0}-{1}", valHome, valAway);
                                break;
                            case "Yellow Cards":
                                //Match["YellowCards"] = string.Format("{0}-{1}", valHome, valAway);
                                break;
                            case "Yellow Cards(Extra time)": break;
                            case "First Yellow Card": break;
                            case "Last Yellow Card": break;
                            case "Red Cards":
                                //Match["RedCards"] = string.Format("{0}-{1}", valHome, valAway);
                                break;
                            case "Head the ball":  break;
                            case "Head Success": break;
                            case "Saves":
                                match["Saves"] = string.Format("{0}-{1}", valHome, valAway);
                                break;
                            case "Blocked":
                                break;
                            case "Tackles":
                                break;
                            case "Dribbles":
                                break;
                            case "Throw ins":
                                break;
                            case "Pass":
                                break;
                            case "Pass Success":
                                break;
                            case "Substitutions":
                                match["Substitutions"] = string.Format("{0}-{1}", valHome, valAway);
                                break;
                            case "First Substitution":
                                break;
                            case "Last Substitution":
                                break;
                            default:
                                WriteLog("ReadStats: Unknown valName => {0}", valName);
                                break;
                        }
                    }
                }
            }
            catch (Exception ex) { WriteLog("ERROR! ReadStats: {0}", ex.Message); }
        }
        private static string XElementListToString(List<XElement> xlist)
        {
            string s = "";
            foreach (var x in xlist)
            {
                s += x.ToString();
            }
            return s;
        }
        private static string ReadEventType(HtmlAgilityPack.HtmlNode td, ActionWriteLog WriteLog)
        {
            try
            {
                var text = System.Net.WebUtility.HtmlDecode(td.InnerHtml).Trim();
                if (text.Length > 0)
                {
                    var value = td.Element("img").GetAttributeValue("src", "");
                    switch (value)
                    {
                        case "/images/bf_img/1.png": return "goal";
                        case "/images/bf_img/2.png": return "red-card";
                        case "/images/bf_img/3.png": return "yellow-card";
                        case "/images/bf_img/4.png": return "in";
                        case "/images/bf_img/5.png": return "out";
                        case "/images/bf_img/6.png": return "";
                        case "/images/bf_img/7.png": return "penalty-goal";
                        case "/images/bf_img/8.png": return "own-goal";
                        case "/images/bf_img/9.png": return "yellow2red-card";
                        case "/images/bf_img/11.png": return "substitute";
                        case "/images/bf_img/12.png": return "assist";
                        case "/images/bf_img/13.png": return "penalty-missed";
                        case "/images/bf_img/14.png": return "panalty-saved";
                        case "/images/bf_img/15.png": return "shot-on-post";
                        case "/images/bf_img/16.png": return "man-of-the-match";
                        case "/images/bf_img/17.png": return "error-lead-to-goal";
                        case "/images/bf_img/18.png": return "last-man-tackle";
                        case "/images/bf_img/19.png": return "clearence-off-the-line";
                        case "/images/bf_img/20.png": return "foul-lead-to-panalty";
                        case "/images/bf_img/55.png": return "mark";
                        default:
                            WriteLog("ReadEventValue: Unknown valName => {0}", value);
                            return "?";
                    }
                }
            }
            catch (Exception ex) { WriteLog("ERROR! ReadEventType: {0}", ex.Message); }
            return "";
        }
        private static string ReadEventValue(HtmlAgilityPack.HtmlNode td, ActionWriteLog WriteLog)
        {
            try
            {
                var a = td.Elements("a");
                if (a == null || a.Count() == 0)
                {
                    return "";
                }
                else if (a.Count() == 1)
                {
                    return System.Net.WebUtility.HtmlDecode(a.First().InnerText).Trim();
                }
                else if (a.Count() == 2)
                {
                    var ar = a.ToArray();
                    return string.Format("{0} => {1}"
                        , System.Net.WebUtility.HtmlDecode(ar[0].InnerText).Trim()
                        , System.Net.WebUtility.HtmlDecode(ar[1].InnerText).Trim()
                        );
                }
            }
            catch (Exception ex) { WriteLog("ERROR! ReadEventValue: {0}", ex.Message); }
            return "";
        }
        private static int ReadMinute(HtmlAgilityPack.HtmlNode tdNode, ActionWriteLog WriteLog)
        {
            try
            {
                var strMinute = System.Net.WebUtility.HtmlDecode(tdNode.InnerText).Trim().Replace("'", "");
                if (strMinute.Length > 0)
                {
                    return int.Parse(strMinute);
                }
            }
            catch (Exception ex) { WriteLog("ERROR! ReadMinute: {0}", ex.Message); }
            return 0;
        }
        private static string ReadKickOff(HtmlAgilityPack.HtmlNode tr, ActionWriteLog WriteLog)
        {
            try
            {
                var strNode = System.Net.WebUtility.HtmlDecode(tr.InnerHtml).Trim();
                if (strNode.Length > 0)
                {
                    switch (tr.FirstChild.GetAttributeValue("src", ""))
                    {
                        case "/images/55.gif": return "1";
                        default: return "?";
                    }
                }
            }
            catch (Exception ex) { WriteLog("ERROR! ReadKickOff: {0}", ex.Message); }
            return "";
        }
        #endregion
    }
}
