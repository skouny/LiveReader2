using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace LiveReader2
{
    public class OpapFootball : WebReaderBase
    {
        #region Init
        public OpapFootball() : base(SourceName.Opap, SportName.Football, "") { }
        List<string> FinalStatusList = new List<string>() { };
        #endregion
        #region Methods
        public override int DoUpdate(HtmlElement body, ref List<string> webIds)
        {
            var manager = new UpdateManager();
            try
            {
                var curDay = Opap.API.GetToday(Opap.API.Sport.soccer);
                foreach (var opapMatch in curDay)
                {
                    var webId = $"{opapMatch.Code}";
                    webIds.Add(webId);
                    // Get the current match if already exists (return null if not exists)
                    var curMatch = (Matches.ContainsKey(webId)) ? Matches[webId] : null;
                    // If current match not exists OR the it is live, make a full read
                    if (curMatch == null || MatchIsLive(curMatch))
                    {
                        // Create new instances for the async task *** SOS ***
                        var taskShowDetailedLog = this.ShowDetailedLog;
                        // Add new task
                        var task = Task<NameValueCollection>.Run(new Func<NameValueCollection>(() =>
                        {
                            var newMatch = ReadMatch(opapMatch);
                            return UpdateAndPostMatch(Sport, curMatch, newMatch, WriteLog, taskShowDetailedLog);
                        }));
                        manager.Tasks.Insert(0, task);
                        // If not using parallel processing
                        if (!UseParallelProcessing)
                        {
                            // wait latest task to complete
                            manager.Tasks[0].Wait();
                            // Do a match break
                            DoMatchBreak();
                        }
                    }
                    // Partial collect & Upload
                    manager.PartialCollectAndUpload(SportName.Football, WriteLog);
                }
                // Full collect & Upload
                manager.FullCollectAndUpload(SportName.Football, WriteLog);
            }
            catch (Exception ex)
            {
                WriteLog("ERROR! DoUpdate: {0}", ex.Message);
            }
            return manager.Modified;
        }
        private bool MatchIsLive(NameValueCollection match)
        { // match is live if StartTime passed and status is not final yet.
            if (FinalStatusList.Contains(match["Status"])) return false;
            return StartTimePassed(match["StartTime"]);
        }
        #endregion
        #region Xml Read
        private static NameValueCollection ReadMatch(Opap.API.Match opapMatch)
        {
            var match = new NameValueCollection();
            match["Source"] = string.Format("{0}", SourceName.Opap);
            match["WebId"] = $"{opapMatch.Code}";
            match["Id"] = string.Format("{0}#{1}", match["Source"], match["WebId"]);
            match["StartTime"] = opapMatch.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            match["Champ"] = opapMatch.ChampName;
            match["HomeTeam"] = opapMatch.HomeTeamUp;
            match["AwayTeam"] = opapMatch.AwayTeamUp;
            match["ScoreHT"] = opapMatch.ScoreHT;
            match["Score"] = opapMatch.ScoreFT;
            match["Status"] = opapMatch.Status;
            match["StatusId"] = opapMatch.HasScoreFT ? MatchStatus.FT.ToString() : FixStatus(opapMatch.Status).ToString();
            match["HomeScored"] = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
            match["AwayScored"] = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
            match["Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return match;
        }
        private static MatchStatus FixStatus(string BetStatus)
        {
            if (BetStatus == "cancelled") return MatchStatus.Cancelled;
            return MatchStatus.Unknown;
        }
        #endregion
    }
}
