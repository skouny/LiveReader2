using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Collections.Specialized;

public class TimedWebClient : WebClient
{
    #region Init
    public int Timeout { get; set; }
    public TimedWebClient(int timeout)
    {
        this.Encoding = Encoding.UTF8;
        this.Timeout = timeout;
    }
    protected override WebRequest GetWebRequest(Uri address)
    {
        var request = base.GetWebRequest(address);
        request.Timeout = this.Timeout;
        return request;
    }
    #endregion
    #region Static Methods
    public static byte[] PostData(NameValueCollection data, string url, int timeout = 30000, int retries = 10)
    {
        for (int i = 0; i < retries; i++)
        {
            try
            {
                var client = new TimedWebClient(timeout);
                return client.UploadValues(url, "POST", data);
            }
            catch { }
        }
        return null;
    }
    public static byte[] PostData(byte[] data, string url, int timeout = 30000, int retries = 10)
    {
        for (int i = 0; i < retries; i++)
        {
            try
            {
                var client = new TimedWebClient(timeout);
                return client.UploadData(url, "POST", data);
            }
            catch { }
        }
        return null;
    }
    public static string PostData(string data, string url, int timeout = 30000, int retries = 10)
    {
        for (int i = 0; i < retries; i++)
        {
            try
            {
                var client = new TimedWebClient(timeout);
                return client.UploadString(url, "POST", data);
            }
            catch { }
        }
        return null;
    }
    public static byte[] DownloadBytes(string url, int timeout = 30000, int retries = 10)
    {
        for (int i = 0; i < retries; i++)
        {
            try
            {
                var client = new TimedWebClient(timeout);
                return client.DownloadData(url);
            }
            catch { }
        }
        return null;
    }
    public static Stream DownloadStream(string url, int timeout = 30000, int retries = 10)
    {
        return new MemoryStream(DownloadBytes(url, timeout, retries));
    }
    public static string DownloadHtml(string url, int timeout = 30000, int retries = 10)
    {
        for (int i = 0; i < retries; i++)
        {
            try
            {
                var client = new TimedWebClient(timeout);
                return client.DownloadString(url);
            }
            catch { }
        }
        return null;
    }
    public static string DownloadText(string url, int timeout = 30000, int retries = 10)
    {
        return DownloadHtml(url, timeout, retries);
    }
    #endregion
}
