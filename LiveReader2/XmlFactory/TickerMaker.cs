using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using LiveReader;
using play90;

public static class TickerMaker
{
    public static void CreateTicker(Opap.API.Sport sport, Opap.API.Locale locale, ActionWrite log)
    {
        try
        {
            var day = ServerIO.DownloadDay(sport, locale, Opap.CurrentDay);
            if (day != null)
            {
                string pathH = $@"play90Worker\Data\{ServerIO.GetSportDir(sport)}\{ServerIO.GetLocaleDir(locale)}\Ticker.txt";
                string pathWH = $@"play90Worker\Data\{ServerIO.GetSportDir(sport)}\{ServerIO.GetLocaleDir(locale)}\TickerW.txt";
                string pathV = $@"play90Worker\Data\{ServerIO.GetSportDir(sport)}\{ServerIO.GetLocaleDir(locale)}\TickerV.txt";
                string pathWV = $@"play90Worker\Data\{ServerIO.GetSportDir(sport)}\{ServerIO.GetLocaleDir(locale)}\TickerVW.txt";
                CreateTickerMain(pathH, pathWH, sport, locale, day, "span");
                CreateTickerMain(pathV, pathWV, sport, locale, day, "div");
                CreateTickerOddChanges(sport, locale, day);
                CreateTickerBlockedMatches(sport, locale, day);
                CreateTickerCancelledMatches(sport, locale, day);
            }
        }
        catch (Exception ex) { log?.Invoke($"CreateTicker error => {ex.Message}"); }
    }
    static void CreateTickerMain(string path, string pathW, Opap.API.Sport sport, Opap.API.Locale locale, BetDayInfo day, string element = "span")
    {
        try
        {
            StringWriter writer = new StringWriter();
            var query = from match in day.Matches
                        let scoreDate = GetLastDate(match.HomeTeam.LastScored, match.AwayTeam.LastScored)
                        where !match.HasFinished && scoreDate != DateTime.MinValue && WithInLast30Min(scoreDate)
                        orderby scoreDate descending
                        select match;
            // Begin Text
            writer.WriteLine(TickerBeginMsg(locale, element));
            // Main Text
            if (query != null && query.Count() > 0)
            {
                var matches = query.ToList();
                foreach (BetMatchInfo match in matches)
                {
                    #region Match Teams
                    writer.Write("<{0} class='tickerMatch'>", element);
                    // HomeTeam Name
                    if (WithInLast30Min(match.HomeTeam.LastScored))
                        writer.Write("<span class='tickerGoal'>{0}</span>", match.HomeTeam.Name);
                    else
                        writer.Write("{0}", match.HomeTeam.Name);
                    writer.Write(" - ");
                    // AwayTeam Name
                    if (WithInLast30Min(match.AwayTeam.LastScored))
                        writer.Write("<span class='tickerGoal'>{0}</span>", match.AwayTeam.Name);
                    else
                        writer.Write("{0}", match.AwayTeam.Name);
                    writer.Write("</span>");
                    #endregion
                    #region Score
                    writer.Write("<{0} class='tickerScore'>", element);
                    // HomeTeam Score
                    if (WithInLast30Min(match.HomeTeam.LastScored))
                        writer.Write("<span class='tickerGoal'>{0}</span>", match.HomeTeam.Score);
                    else
                        writer.Write("{0}", match.HomeTeam.Score);
                    writer.Write(" - ");
                    // AwayTeam Score
                    if (WithInLast30Min(match.AwayTeam.LastScored))
                        writer.Write("<span class='tickerGoal'>{0}</span>", match.AwayTeam.Score);
                    else
                        writer.Write("{0}", match.AwayTeam.Score);
                    // T symbol
                    if (match.HasFinished) { writer.Write(" T"); }
                    writer.Write("</{0}>", element);
                    writer.Write("</{0}>", element);
                    writer.WriteLine();
                    #endregion
                    // Delete ticker for widget
                   ServerIO.Delete(pathW);
                }
            }
            else
            {
                writer.Write("<{0}>{1}</{0}>", element, Lexicon.Word(LexiconKey.NoChanges, locale));
                // Create ticker for widget
                CreateTickerForWidget(pathW, sport, locale, day, element);
            }
            // End Text
            writer.WriteLine(TickerEndMsg(locale, element));
            // Write File
            ServerIO.Upload(path, writer.ToString());
        }
        catch { }
    }
    static void CreateTickerForWidget(string pathW, Opap.API.Sport sport, Opap.API.Locale locale, BetDayInfo day, string element = "span")
    {
        try
        {
            StringWriter writer = new StringWriter();
            var query = from match in day.Matches
                        where !match.HasFinished
                        orderby match.StartTime.Value ascending
                        select match;
            // Begin Text
            writer.WriteLine(TickerBeginMsg(locale, element));
            // Main Text
            if (query != null && query.Count() > 0)
            {
                var matches = query.ToList();
                foreach (BetMatchInfo match in matches)
                {
                    writer.WriteLine("<{0} class='tickerMatch'>{1}. {2} - {3}: <span class='tickerGoal'>{4:HH:mm}</span></{0}>"
                        , element
                        , match.Code.Value
                        , match.HomeTeam.Name
                        , match.AwayTeam.Name
                        , match.StartTime.Value);
                }
            }
            else
            {
                writer.WriteLine("<{0}>{1}</{0}>", element, Lexicon.Word(LexiconKey.NoMatchesToday, locale));
            }
            // End Text
            writer.WriteLine(TickerEndMsg(locale, element));
            // Write File
            ServerIO.Upload(pathW, writer.ToString());
        }
        catch { }
    }
    static void CreateTickerOddChanges(Opap.API.Sport sport, Opap.API.Locale locale, BetDayInfo day)
    {
        try
        {
            string path = $@"play90Worker\Data\{ServerIO.GetSportDir(sport)}\{ServerIO.GetLocaleDir(locale)}\TickerOddChanges.txt";
            StringWriter writer = new StringWriter();
            var query = from match in day.Matches
                        where match.Odd1.Changes.Count > 0 || match.OddX.Changes.Count > 0 || match.Odd2.Changes.Count > 0
                        orderby match.Code.Value ascending
                        select match;
            // Main Text
            if (query != null && query.Count() > 0)
            {
                var matches = query.ToList();
                foreach (BetMatchInfo match in matches)
                {
                    // Begin tag
                    writer.Write("<span class='tickerOddChange'>");
                    // Code - Match
                    writer.Write("{0}. {1} - {2}: Από "
                        , match.Code.Value
                        , match.HomeTeam.Name
                        , match.AwayTeam.Name
                        );
                    // Begin Odds
                    writer.Write("{0}-{1}-{2}"
                        , string.Format("<span class='odd'>{0:0.00}</span>", match.Odd1.BeginValue)
                        , string.Format("<span class='odd'>{0:0.00}</span>", match.OddX.BeginValue)
                        , string.Format("<span class='odd'>{0:0.00}</span>", match.Odd2.BeginValue)
                        );
                    // Previous Odds
                    //writer.Write("{0}-{1}-{2}"
                    //    , GetChangedOddElement(match.Odd1.PreviousValue, match.Odd1.BeginValue)
                    //    , GetChangedOddElement(match.OddX.PreviousValue, match.OddX.BeginValue)
                    //    , GetChangedOddElement(match.Odd2.PreviousValue, match.Odd2.BeginValue)
                    //    );
                    // Separator
                    writer.Write(" σε ");
                    // Latest Odds
                    writer.Write("{0}-{1}-{2}"
                        , GetChangedOddElement(match.Odd1.Value, match.Odd1.BeginValue)
                        , GetChangedOddElement(match.OddX.Value, match.OddX.BeginValue)
                        , GetChangedOddElement(match.Odd2.Value, match.Odd2.BeginValue)
                        );
                    // End tag
                    writer.Write("</span>");
                }
            }
            // Write File
            ServerIO.Upload(path, writer.ToString());
        }
        catch { }
    }
    static void CreateTickerBlockedMatches(Opap.API.Sport sport, Opap.API.Locale locale, BetDayInfo day)
    {
        try
        {
            string path = $@"play90Worker\Data\{ServerIO.GetSportDir(sport)}\{ServerIO.GetLocaleDir(locale)}\TickerBlockedMatches.txt";
            StringWriter writer = new StringWriter();
            var query = from match in day.Matches
                        where match.OpapStatus != null && match.OpapStatus.Value == "blocked"
                        orderby match.Code.Value ascending
                        select match;
            // Main Text
            if (query != null && query.Count() > 0)
            {
                var matches = query.ToList();
                foreach (BetMatchInfo match in matches)
                {
                    // Begin tag
                    writer.Write("<span class='tickerBlockedMatch'>");
                    // Code - Match
                    writer.Write("{0}. {1} - {2}: <span class='red'>ΦΡΑΓΜΕΝΟΣ</span>"
                        , match.Code.Value
                        , match.HomeTeam.Name
                        , match.AwayTeam.Name
                        );
                    // End tag
                    writer.Write("</span>");
                }
            }
            // Write File
            ServerIO.Upload(path, writer.ToString());
        }
        catch { }
    }
    static void CreateTickerCancelledMatches(Opap.API.Sport sport, Opap.API.Locale locale, BetDayInfo day)
    {
        try
        {
            string path = $@"play90Worker\Data\{ServerIO.GetSportDir(sport)}\{ServerIO.GetLocaleDir(locale)}\TickerCancelledMatches.txt";
            StringWriter writer = new StringWriter();
            var query = from match in day.Matches
                        where match.OpapStatus != null && match.OpapStatus.Value == "cancelled"
                        orderby match.Code.Value ascending
                        select match;
            // Main Text
            if (query != null && query.Count() > 0)
            {
                var matches = query.ToList();
                foreach (BetMatchInfo match in matches)
                {
                    // Begin tag
                    writer.Write("<span class='tickerCancelledMatch'>");
                    // Code - Match
                    writer.Write("{0}. {1} - {2}: <span class='red'>ΑΝΑΒΛΗΘΗΚΕ</span>"
                        , match.Code.Value
                        , match.HomeTeam.Name
                        , match.AwayTeam.Name
                        );
                    // End tag
                    writer.Write("</span>");
                }
            }
            // Write File
            ServerIO.Upload(path, writer.ToString());
        }
        catch { }
    }
    static string GetChangedOddElement(float currentOdd, float beginOdd)
    {
        if (currentOdd == beginOdd)
        {
            return string.Format("<span class='odd'>{0:0.00}</span>", currentOdd);
        }
        else if (currentOdd > beginOdd)
        {
            return string.Format("<span class='odd red'>{0:0.00}</span>", currentOdd);
        }
        else
        {
            return string.Format("<span class='odd blue'>{0:0.00}</span>", currentOdd);
        }
    }
    static bool WithInLast30Min(DateTime time)
    {
        if (time != null)
        {
            if (DateTime.Compare(time.AddMinutes(30), DateTime.Now) > 0)
            {
                return true;
            }
        }
        return false;
    }
    static DateTime GetLastDate(DateTime t1, DateTime t2)
    {
        int i = DateTime.Compare(t1, t2);
        if (i > 0) return t1;
        else return t2;
    }
    static string GetViewingDayFilename(Opap.API.Sport sport, Opap.API.Locale locale)
    {
        var date = Opap.CurrentDay;
        return $@"play90Worker\Data\{ServerIO.GetSportDir(sport)}\{ServerIO.GetLocaleDir(locale)}\{date.Year:0000}\{date.Month:00}\{date.Day:00}.xml";
    }
    static string TickerBeginMsg(Opap.API.Locale locale, string element = "span")
    {
        string path = (locale == Opap.API.Locale.el) ? $@"play90Worker\TickerBegin.txt" : $@"play90Worker\TickerIntBegin.txt";
        var text = ServerIO.Download(path);
        if (text != null)
        {
            if (text.Trim().Length > 0)
            {
                return string.Format("<{0} class='tickerMsgBegin'>{1}</{0}>", element, text);
            }
        }
        return "";
    }
    static string TickerEndMsg(Opap.API.Locale locale, string element = "span")
    {
        if (TickerEndExpiration(locale) > DateTime.Now)
        {
            string path = (locale == Opap.API.Locale.el) ? $@"play90Worker\TickerEnd.txt" : $@"play90Worker\TickerIntEnd.txt";
            var text = ServerIO.Download(path);
            if (text != null)
            {
                if (text.Trim().Length > 0)
                {
                    return string.Format("<{0} class='tickerMsgEnd'>{1}</{0}>", element, text);
                }
            }
        }
        return "";
    }
    static DateTime TickerEndExpiration(Opap.API.Locale locale)
    {
        try
        {
            string path = (locale == Opap.API.Locale.el) ? $@"play90Worker\TickerEndExpiration.txt" : $@"play90Worker\TickerIntEndExpiration.txt";
            var text = ServerIO.Download(path);
            if (text != null)
            {
                if (text.Trim().Length > 0)
                {
                    return DateTime.Parse(text);
                }
            }
        }
        catch { }
        return DateTime.MinValue;
    }
}