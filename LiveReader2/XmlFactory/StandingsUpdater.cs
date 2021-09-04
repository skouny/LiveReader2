using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StandingsUpdater
{
    public static void UpdateFeedUnattended(int retries, ActionWrite log = null)
    {
        int i = 0;
        while (i < retries)
        {
            var message = UpdateFeed();
            if (String.IsNullOrEmpty(message))
            {
                log?.Invoke($"Standings feed is ready!");
                break;
            }
            else
            {
                log?.Invoke($"Standings.xml: {message} [try {i++}]");
            }
        }
    }
    public static string UpdateFeed()
    {
        try
        {
            LiveReader.MyDatabase.XmlFileWrite("Standings.xml", GetFeed, true);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return null;
    }
    public static byte[] GetFeed
    {
        get
        {
            byte[] data = null;
            using (var client = new System.Net.WebClient())
            {
                 data = client.DownloadData(MyURLs.StandingsFeedUrl());
            }
            return data;
        }
    }
}
