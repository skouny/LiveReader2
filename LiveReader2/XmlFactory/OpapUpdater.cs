using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using play90;
using LiveReader;

public class OpapUpdater
{
    public const int LIVE_UPDATE_DELAY = 2000; // in msec
    public const int TICKER_UPDATE_DELAY = 60; // in sec
    public const int FULL_UPDATE_DELAY = 15; // in min
    private Opap.API.Sport Sport { get; set; }
    private Opap.API.Locale Locale { get; set; }
    private ActionWrite WriteLog { get; set; }
    private Task<Opap.API.Days> OpapFeed { get; set; } // active days
    private Task OpapFullUpdate { get; set; } // next 31 days
    private DateTime LastOpapUpdate { get; set; } // last 31 days update
    private DateTime LastTickerUpdate { get; set; }
    public OpapUpdater(Opap.API.Sport sport, Opap.API.Locale locale, ActionWrite log = null)
    {
        this.Sport = sport;
        this.Locale = locale;
        this.WriteLog = log;
        this.OpapFeed = null;
        this.OpapFullUpdate = null;
        this.LastOpapUpdate = DateTime.MinValue;
        this.LastTickerUpdate = DateTime.MinValue;
    }
    public Task StartAsync()
    {
        return Task.Run(() =>
        {
            while (true)
            {
                this.DoUpdate();
                Thread.Sleep(LIVE_UPDATE_DELAY);
            }
        });
    }
    public void DoUpdate(bool makeFullUpdate = true, bool makeLivestats = true)
    {
        if (makeFullUpdate)
        {
            // Check if it's time to download 31 days feed
            if (this.OpapFeed == null && (DateTime.Now - this.LastOpapUpdate).TotalMinutes > FULL_UPDATE_DELAY)
            {
                this.OpapFeed = Task<Opap.API.Days>.Run<Opap.API.Days>(new Func<Opap.API.Days>(() =>
                {
                    try
                    {
                        this.WriteLog?.Invoke($"Opap feed fetch started");
                        var begin = Environment.TickCount;
                        var opap = Opap.API.GetActiveDays(this.Sport, this.Locale);
                        var took = Environment.TickCount - begin;
                        this.WriteLog?.Invoke($"Opap feed fetched, found {opap.SelectMany(x => x.Value).Count()} matches [{took:#,##0}]");
                        return opap;
                    }
                    catch (Exception ex)
                    {
                        this.WriteLog?.Invoke($"Opap feed fetch error => {ex.Message}");
                    }
                    return null;
                }));
            }
            // If full update not exists or completed
            if (this.OpapFullUpdate == null || this.OpapFullUpdate.IsCompleted)
            {
                // ...and OpapFeed task exists and completed, do full update
                if (this.OpapFeed != null && this.OpapFeed.IsCompleted)
                {
                    var opapFeed = this.OpapFeed.Result;
                    if (opapFeed != null)
                    {
                        var todayKey = Opap.CurrentDay.ToString("yyyy-MM-dd");
                        // First, update current day, sync
                        var todayFeed = opapFeed[todayKey];
                        if (todayFeed != null && todayFeed.Count() > 0)
                        {
                            UpdateDay(this.Sport, this.Locale, todayFeed, this.WriteLog);
                        }
                        // Then, update the remaining days and tasks, async
                        OpapFullUpdate = Task.Run(new Action(() =>
                        {
                            foreach (var dayFeed in opapFeed.Where(x => x.Key != todayKey))
                            {
                                UpdateDay(this.Sport, this.Locale, dayFeed.Value, this.WriteLog);
                            }
                            // Update hacker feed
                            if (this.Sport == Opap.API.Sport.soccer && this.Locale == Opap.API.Locale.el)
                            {
                                UpdateHackerFeed(this.WriteLog);
                            }
                            // Free opap feed task and set last update time
                            this.OpapFeed = null;
                            this.LastOpapUpdate = DateTime.Now;
                        }));
                    }
                    else
                    {
                        this.LastOpapUpdate = DateTime.Now;
                    }
                }
            }
        }
        else // updates with full update disabled
        {
            if ((DateTime.Now - this.LastOpapUpdate).TotalMinutes > FULL_UPDATE_DELAY)
            {
                // Update hacker feed
                if (this.Sport == Opap.API.Sport.soccer && this.Locale == Opap.API.Locale.el)
                {
                    UpdateHackerFeed(this.WriteLog);
                }
                this.LastOpapUpdate = DateTime.Now;
            }
        }
        // livescore update
        this.UpdateLivescore(makeLivestats);
        // ticker update
        if ((DateTime.Now - this.LastTickerUpdate).TotalSeconds > TICKER_UPDATE_DELAY)
        {
            this.UpdateTicker();
        }
    }
    static void UpdateDay(Opap.API.Sport sport, Opap.API.Locale locale, Opap.API.Day feed, ActionWrite log)
    {
        var begin = Environment.TickCount;
        string ms = "";
        try
        {
            // download
            var day = ServerIO.DownloadDay(sport, locale, feed.HumanDay, log);
            ms += $"down: {Environment.TickCount - begin:#,##0}";
            // if not exists, create new
            if (day == null)
            {
                day = new BetDayInfo(feed);
                ms += $", new: {Environment.TickCount - begin:#,##0}";
            }
            else // else, update
            {
                // get weather
                var weather = GetWeather(sport, locale, day.HomeTeams);
                ms += $", weather: {Environment.TickCount - begin:#,##0}";
                // update day with newer opap xml
                day.Update(feed, weather);
                ms += $", update: {Environment.TickCount - begin:#,##0}";
            }
            // upload
            ServerIO.UploadDay(sport, locale, day);
            ms += $", upload: {Environment.TickCount - begin:#,##0}";
            // log
            log?.Invoke($"Day {feed.HumanDay:yyyy-MM-dd} updated with opap feed [{ms}]");
        }
        catch (Exception ex)
        {
            log?.Invoke($"Day {feed.HumanDay:yyyy-MM-dd} error => {ex.Message} [{ms}]");
        }
    }
    static WeatherInfo GetWeather(Opap.API.Sport sport, Opap.API.Locale locale, string HomeTeams)
    {
        return (sport == Opap.API.Sport.soccer && locale == Opap.API.Locale.el) ? WeatherInfo.GetTeams(HomeTeams) : null;
    }
    #region Download / Upload
    private BetDayInfo DownloadDay(DateTime date) { return ServerIO.DownloadDay(this.Sport, this.Locale, date, this.WriteLog); }
    private string UploadDay(BetDayInfo day) { return ServerIO.UploadDay(this.Sport, this.Locale, day); }
    private byte[] DownloadLiveFeed()
    {
        byte[] data;
        using (var client = new TimedWebClient(30000))
        {
            data = client.DownloadData(MyURLs.OpapLiveFeedUrl(this.Sport));
        }
        return data;
    }
    #endregion
    #region Live
    private void UpdateTicker()
    {
        var begin = Environment.TickCount;
        try
        {
            var message = TimedWebClient.DownloadText($@"https://play90.gr/CreateTicker.aspx?Sport={this.Sport}&Locale={this.Locale}");
            this.WriteLog?.Invoke($"Ticker {message} [{Environment.TickCount - begin:#,##0}]");
            this.LastTickerUpdate = DateTime.Now;
        }
        catch (Exception ex)
        {
            this.WriteLog?.Invoke($"Ticker error! => {ex.Message} [{Environment.TickCount - begin:#,##0}]");
        }
    }
    private void UpdateLivescore(bool makeLivestats)
    {
        var begin = Environment.TickCount;
        string ms = "";
        try
        {
            var today = this.DownloadDay(Opap.CurrentDay);
            ms += $"down-day: {Environment.TickCount - begin:#,##0}";
            var data = this.DownloadLiveFeed();
            ms += $", down-live: {Environment.TickCount - begin:#,##0}";
            var live = MatchDay.ReadXml(data);
            ms += $", read-live: {Environment.TickCount - begin:#,##0}";
            if (today != null)
            {
                var stat = this.AddLiveToDay(today, live, makeLivestats);
                var add = Environment.TickCount - begin;
                ms += $", add: {add:#,##0} (live-stats: {stat:#,##0})";
                // 
                var message = this.UploadDay(today);
                ms += $", up-day: {Environment.TickCount - begin:#,##0}";
                if (message.StartsWith("Done!"))
                {
                    this.WriteLog?.Invoke($"Livescore updated [{ms}]");
                }
                else
                {
                    this.WriteLog?.Invoke(message);
                }
            }
            else
            {
                this.WriteLog?.Invoke($"Livescore not updated, today xml not found");
            }
        }
        catch (Exception ex)
        {
            this.WriteLog?.Invoke($"Livescore error => {ex.Message} [{ms}]");
        }
    }
    private int AddLiveToDay(BetDayInfo day, MatchDay live, bool makeLivestats)
    {
        int liveStatTime = 0;
        int newLiveStats = 0;
        foreach (BetMatchInfo dayMatch in day.Matches)
        {
            var query = from i in live.Matches
                        where i.OpapInf.Code == dayMatch.Code.Value.ToString()
                        && i.StartTime.Value == dayMatch.StartTime.Value
                        select i;
            if (query != null && query.Count() > 0)
            {
                var liveMatch = query.First();
                dayMatch.SetScore(liveMatch.Score);
                dayMatch.SetScoreHT(liveMatch.ScoreHT);
                dayMatch.SetStatus(liveMatch.Status.ToString());
                dayMatch.SetMinute(liveMatch.Minute);
                dayMatch.SetYellowCardsHome(liveMatch.HomeTeam.YellowCards);
                dayMatch.SetYellowCardsAway(liveMatch.AwayTeam.YellowCards);
                dayMatch.SetRedCardsHome(liveMatch.HomeTeam.RedCards);
                dayMatch.SetRedCardsAway(liveMatch.AwayTeam.RedCards);
                // Home
                dayMatch.HomeTeam.Standing = liveMatch.HomeTeam.StandingPoints;
                dayMatch.HomeTeam.CornerKicks = liveMatch.HomeTeam.CornerKicks;
                dayMatch.HomeTeam.FirstYellowCard = liveMatch.HomeTeam.FirstYellowCard;
                dayMatch.HomeTeam.LastYellowCard = liveMatch.HomeTeam.LastYellowCard;
                dayMatch.HomeTeam.Shots = liveMatch.HomeTeam.Shots;
                dayMatch.HomeTeam.ShotsOnGoal = liveMatch.HomeTeam.ShotsOnGoal;
                dayMatch.HomeTeam.Fouls = liveMatch.HomeTeam.Fouls;
                dayMatch.HomeTeam.BallPossession = liveMatch.HomeTeam.BallPossession;
                dayMatch.HomeTeam.Saves = liveMatch.HomeTeam.Saves;
                dayMatch.HomeTeam.Offsides = liveMatch.HomeTeam.Offsides;
                dayMatch.HomeTeam.Substitutions = liveMatch.HomeTeam.Substitutions;
                dayMatch.HomeTeam.FirstSubstitution = liveMatch.HomeTeam.FirstSubstitution;
                dayMatch.HomeTeam.LastSubstitution = liveMatch.HomeTeam.LastSubstitution;
                dayMatch.HomeTeam.KickOff = liveMatch.AwayTeam.KickOff;
                UpdateEvents(dayMatch.HomeTeam, liveMatch.HomeTeam);
                // Away
                dayMatch.AwayTeam.Standing = liveMatch.AwayTeam.StandingPoints;
                dayMatch.AwayTeam.CornerKicks = liveMatch.AwayTeam.CornerKicks;
                dayMatch.AwayTeam.FirstYellowCard = liveMatch.AwayTeam.FirstYellowCard;
                dayMatch.AwayTeam.LastYellowCard = liveMatch.AwayTeam.LastYellowCard;
                dayMatch.AwayTeam.Shots = liveMatch.AwayTeam.Shots;
                dayMatch.AwayTeam.ShotsOnGoal = liveMatch.AwayTeam.ShotsOnGoal;
                dayMatch.AwayTeam.Fouls = liveMatch.AwayTeam.Fouls;
                dayMatch.AwayTeam.BallPossession = liveMatch.AwayTeam.BallPossession;
                dayMatch.AwayTeam.Saves = liveMatch.AwayTeam.Saves;
                dayMatch.AwayTeam.Offsides = liveMatch.AwayTeam.Offsides;
                dayMatch.AwayTeam.Substitutions = liveMatch.AwayTeam.Substitutions;
                dayMatch.AwayTeam.FirstSubstitution = liveMatch.AwayTeam.FirstSubstitution;
                dayMatch.AwayTeam.LastSubstitution = liveMatch.AwayTeam.LastSubstitution;
                dayMatch.AwayTeam.KickOff = liveMatch.AwayTeam.KickOff;
                UpdateEvents(dayMatch.AwayTeam, liveMatch.AwayTeam);
                // LiveStats Update
                if (this.Sport == Opap.API.Sport.soccer && makeLivestats && dayMatch.IsLiveStat)
                {
                    if (newLiveStats <= 1)
                    {
                        var liveStatBegin = Environment.TickCount;
                        try
                        {
                            if (dayMatch.LiveStats == null || !dayMatch.LiveStats.IsSet)
                            {
                                dayMatch.LiveStats = new LiveStatsInfo(dayMatch);
                                newLiveStats += 1;
                            }
                            else if (dayMatch.LiveStats.UpdateAll(dayMatch))
                            {
                                newLiveStats += 1;
                            }
                        }
                        catch { }
                        liveStatTime += Environment.TickCount - liveStatBegin;
                    }
                }
            }
        }
        return liveStatTime;
    }
    private void UpdateEvents(BetTeam dayMatchTeam, TeamInfo liveMatchTeam)
    {
        dayMatchTeam.Events.Clear();
        foreach (var liveEvent in liveMatchTeam.Events)
        {
            var newEvent = new BetTeamEvent();
            newEvent.Type = liveEvent.Type;
            newEvent.Minute = liveEvent.Minute;
            newEvent.Value = liveEvent.Value;
            dayMatchTeam.Events.Add(newEvent);
        }
    }
    #endregion
    #region Extra
    static void UpdateHackerFeed(ActionWrite log)
    {
        var begin = Environment.TickCount;
        try
        {
            using (var client = new System.Net.WebClient())
            {
                var today = DateTime.Now;
                for (int i = -2; i <= 1; i++)
                {
                    var day = today.AddDays(i);
                    var message = client.DownloadString(MyURLs.HackerFeedUpdate(day));
                }
                log?.Invoke($"Hacker updated [{Environment.TickCount - begin:#,##0}]");
            }
        }
        catch (Exception ex) { log?.Invoke($"UpdateHackerFeed: Error! => {ex.Message} [{Environment.TickCount - begin:#,##0}]"); }
    }
    #endregion
}