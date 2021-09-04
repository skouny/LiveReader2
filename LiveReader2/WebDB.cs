using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Specialized;
using System.Xml.Linq;

public enum SportName { None, Football, Basket };
public enum SourceName { None, Opap, NowGoal, Futbol24, FlashScore };
public enum MatchStatus
{
    Unknown = 0, Waiting = 1, Qrt1 = 2, Half1 = 3, Qrt2 = 4, HT = 5, Qrt3 = 6, Half2 = 7, Qrt4 = 8,
    FT = 9, OT = 10, Pause = 11, Cancelled = 12, Abd = 13
};
public class SofaLeague
{
    public string Id { get; set; }
    public string GroupIndex { get; set; }
    public string Url { get; set; }
    public uint ContentIndex { get; set; }
}
public class WebDB
{
    public const string SERVER_ADDRESS = "https://livereader.play90.gr";
    public const int TIMEOUT = 10000;
    public static string ServerPath
    {
        get
        {
            return string.Format(@"{0}/livereader2", SERVER_ADDRESS);
        }
    }
    #region tables live mix
    public static string url_table_live_mix_get(SportName sport, SourceName source)
    {
        return string.Format(@"{0}/table_live_mix_get.php?sport={1}&source={2}", ServerPath, sport, source);
    }
    public static string table_live_mix_update(SportName sport, NameValueCollection data)
    {
        var msg = "";
        try
        {
            using (var client = new TimedWebClient(TIMEOUT))
            {
                var address = string.Format(@"{0}/table_live_mix_update.php?sport={1}", ServerPath, sport);
                var bytes = client.UploadValues(address, "POST", data);
                msg = Encoding.UTF8.GetString(bytes);
            }
        }
        catch { }
        return msg;
    }
    public static string table_live_mix_delete(SportName sport, SourceName source, string webId = null)
    {
        var msg = "";
        try
        {
            using (var client = new TimedWebClient(TIMEOUT))
            {
                string address;
                if (webId == null)
                {
                    address = string.Format(@"{0}/table_live_mix_delete.php?sport={1}&source={2}"
                        , ServerPath, sport, source);
                }
                else
                {
                    address = string.Format(@"{0}/table_live_mix_delete.php?sport={1}&source={2}&webid={3}"
                        , ServerPath, sport, source, webId);
                }
                msg = client.DownloadString(address);
            }
        }
        catch { }
        return msg;
    }
    public static string table_live_mix_delete(SportName sport)
    {
        var msg = "";
        try
        {
            using (var client = new TimedWebClient(TIMEOUT))
            {
                var address = string.Format(@"{0}/table_live_mix_delete.php?sport={1}", ServerPath, sport);
                msg = client.DownloadString(address);
            }
        }
        catch { }
        return msg;
    }
    #endregion
    #region Sofa Football Standings
    public static List<SofaLeague> sofa_football_leagues
    {
        get
        {
            var leagues = new List<SofaLeague>();
            var xDoc = XDocument.Load(url_sofa_football_leagues);
            foreach (var xLeague in xDoc.Root.Elements())
            {
                var league = new SofaLeague();
                league.Id = xLeague.Attribute("Id").Value;
                league.GroupIndex = xLeague.Attribute("GroupIndex").Value;
                league.Url = xLeague.Attribute("Url").Value;
                league.ContentIndex = uint.Parse(xLeague.Attribute("ContentIndex").Value);
                leagues.Add(league);
            }
            return leagues;
        }
    }
    public static string sofa_football_standing_update(NameValueCollection record)
    {
        var msg = "";
        using (var client = new TimedWebClient(TIMEOUT))
        {
            var bytes = client.UploadValues(url_sofa_football_standing_update, "POST", record);
            msg = Encoding.UTF8.GetString(bytes);
        }
        return msg;
    }
    public static string sofa_football_standing_cleanup(string champ_id, string group_index, int places_count)
    {
        string result = "";
        try
        {
            using (var client = new TimedWebClient(TIMEOUT))
            {
                result = client.DownloadString(string.Format("{0}?champ_id={1}&group_index={2}&places_count={3}"
                    , url_sofa_football_standing_cleanup
                    , champ_id, group_index, places_count));
            }
        }
        catch { }
        return result;
    }
    public static string url_sofa_football_leagues
    {
        get
        {
            return string.Format(@"{0}/sofa_football_leagues.php", ServerPath);
        }
    }
    public static string url_sofa_football_standing_update
    {
        get
        {
            return string.Format(@"{0}/sofa_football_standing_update.php", ServerPath);
        }
    }
    public static string url_sofa_football_standing_cleanup
    {
        get
        {
            return string.Format(@"{0}/sofa_football_standing_cleanup.php", ServerPath);
        }
    }
    #endregion
    #region BetVirus Football Standings
    public static string football_betvirus_standings_update(NameValueCollection data)
    {
        var msg = "";
        try
        {
            using (var client = new TimedWebClient(TIMEOUT))
            {
                var bytes = client.UploadValues(url_football_betvirus_standings_update, "POST", data);
                msg = Encoding.UTF8.GetString(bytes);
            }
        }
        catch { }
        return msg;
    }
    public static List<string> football_betvirus_leagueIDs
    {
        get
        {
            var leagueIDs = new List<string>();
            try
            {
                var xDoc = XDocument.Load(url_football_betvirus_leagues_get);
                foreach (var xLeague in xDoc.Root.Elements())
                {
                    var leagueId = xLeague.Attribute("Id").Value;
                    leagueIDs.Add(leagueId);
                }
            }
            catch { }
            return leagueIDs;
        }
    }
    public static NameValueCollection football_betvirus_standings_record(string leagueId, string teamName)
    {
        var record = new NameValueCollection();
        try
        {
            var uri = string.Format(@"{0}?LeagueId={1}&TeamName={2}", url_football_betvirus_standings_get, leagueId, teamName);
            var xDoc = XDocument.Load(uri);
            if (xDoc.Root.HasElements)
            {
                var element = xDoc.Root.Element("Team");
                record["Id"] = element.Attribute("Id").Value;
                record["LeagueId"] = element.Attribute("LeagueId").Value;
                record["TeamName"] = element.Attribute("TeamName").Value;
                record["TeamId"] = element.Attribute("TeamId").Value;
            }
        }
        catch { }
        return record;
    }
    public static string url_football_betvirus_leagues_get
    {
        get
        {
            return string.Format(@"{0}/football_betvirus_leagues_get.php", ServerPath);
        }
    }
    public static string url_football_betvirus_standings_get
    {
        get
        {
            return string.Format(@"{0}/football_betvirus_standings_get.php", ServerPath);
        }
    }
    public static string url_football_betvirus_standings_update
    {
        get
        {
            return string.Format(@"{0}/football_betvirus_standings_update.php", ServerPath);
        }
    }
    #endregion
    #region Hacker
    public static async void opap_hacker_update_async()
    {
        await Task.Run(() =>
        {
            opap_hacker_update();
        });
    }
    public static string opap_hacker_update()
    {
        string result = "";
        try
        {
            using (var client = new TimedWebClient(15 * 60 * 1000))
            {
                result = client.DownloadString(url_opap_hacker);
            }
        }
        catch { }
        return result;
    }
    public static string url_opap_hacker
    {
        get
        {
            return string.Format(@"{0}/opap_json.php", ServerPath);
        }
    }
    #endregion
}
