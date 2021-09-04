using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using play90;

public class ServerIO
{
    public const string SERVER_PAGE = "https://play90.gr/xFile.aspx";
    //public const string SERVER_PAGE = "http://localhost:58407/xFile.aspx";
    #region Types
    public class Directory
    {
        public Directory(string name)
        {
            this.Name = name;
        }
        public string Name { get; set; }
    }
    public class File
    {
        public File(string name, long length, DateTime modified)
        {
            this.Name = name;
            this.Length = length;
            this.Modified = modified;
        }
        public string Name { get; set; }
        public long Length { get; set; }
        public DateTime Modified { get; set; }
    }
    public class DirectoryInfo
    {
        public DirectoryInfo()
        {
            this.Dirs = new List<Directory>();
            this.Files = new List<File>();
        }
        public List<Directory> Dirs { get; set; }
        public List<File> Files { get; set; }
    }
    #endregion
    public static string GetSportDir(Opap.API.Sport sport)
    {
        switch (sport)
        {
            case Opap.API.Sport.soccer: return "Football";
            case Opap.API.Sport.basketball: return "Basket";
            default: return $"{sport}";
        }
    }
    public static string GetLocaleDir(Opap.API.Locale locale)
    {
        switch (locale)
        {
            case Opap.API.Locale.el: return "gr";
            case Opap.API.Locale.en: return "en";
            default: return $"{locale}";
        }
    }
    #region Download
    public static string Download(string path)
    {
        var x = new XRequest();
        x.Path = path;
        var text = x.Post(SERVER_PAGE);
        if (text != null && text.StartsWith("Path not found!")) // Not exists
        {
            return null;
        }
        return text;
    }
    public static BetDayInfo DownloadDay(Opap.API.Sport sport, Opap.API.Locale locale, DateTime date, ActionWrite log = null)
    {
        var x = new XRequest();
        x.Path = $@"play90Worker\Data\{GetSportDir(sport)}\{GetLocaleDir(locale)}\{date.Year:0000}\{date.Month:00}\{date.Day:00}.xml";
        var xmlString = x.Post(SERVER_PAGE);
        if (string.IsNullOrWhiteSpace(xmlString)) // Nothing
        {
            log?.Invoke("Nothing");
        }
        else if (xmlString.StartsWith("<?xml")) // OK
        {
            //this.WriteLog?.Invoke("OK");
            return BetDayInfo.ReadXmlString(xmlString);
        }
        else // Message
        {
            log?.Invoke(xmlString);
        }
        return null;
    }
    #endregion
    #region Upload
    public static string Upload(string path, string text)
    {
        var x = new XRequest();
        x.Path = path;
        x.Text = text;
        return x.Post(SERVER_PAGE);
    }
    public static string UploadDay(Opap.API.Sport sport, Opap.API.Locale locale, BetDayInfo day)
    {
        var x = new XRequest();
        x.Path = $@"play90Worker\Data\{GetSportDir(sport)}\{GetLocaleDir(locale)}\{day.Date.Year:0000}\{day.Date.Month:00}\{day.Date.Day:00}.xml";
        x.Text = day.GetXmlString;
        var message = x.Post(SERVER_PAGE);
        return message;
    }
    public static string UploadForm(Opap.API.Sport sport, Opap.API.Locale locale, DateTime date, int code, XElement element)
    {
        var x = new XRequest();
        x.Path = $@"play90Worker\Data\{GetSportDir(sport)}\{GetLocaleDir(locale)}\{date.Year:0000}\{date.Month:00}\{date.Day:00}\{code}.xml";
        x.Text = element.ToString();
        return x.Post(SERVER_PAGE);
    }
    public static string UploadBetMix(Opap.API.Sport sport, Opap.API.Locale locale, DateTime date, XElement element)
    {
        var x = new XRequest();
        x.Path = $@"play90Worker\Data\{GetSportDir(sport)}\{GetLocaleDir(locale)}\{date.Year:0000}\{date.Month:00}\{date.Day:00}\BetMix.xml";
        x.Text = element.ToString();
        return x.Post(SERVER_PAGE);
    }
    #endregion
    #region Exists
    public static bool FileExists(string path)
    {
        var x = new XRequest();
        x.Path = path;
        x.Action = "exists?";
        return bool.Parse(x.Post(SERVER_PAGE));
    }
    public static bool FormExists(Opap.API.Sport sport, Opap.API.Locale locale, DateTime date, int code)
    {
        var path = $@"play90Worker\Data\{GetSportDir(sport)}\{GetLocaleDir(locale)}\{date.Year:0000}\{date.Month:00}\{date.Day:00}\{code}.xml";
        return FileExists(path);
    }
    public static bool BetMixExists(Opap.API.Sport sport, Opap.API.Locale locale, DateTime date)
    {
        var path = $@"play90Worker\Data\{GetSportDir(sport)}\{GetLocaleDir(locale)}\{date.Year:0000}\{date.Month:00}\{date.Day:00}\BetMix.xml";
        return FileExists(path);
    }
    #endregion
    #region Modified
    public static DateTime FileModified(string path)
    {
        var x = new XRequest();
        x.Path = path;
        x.Action = "modified";
        return DateTime.Parse(x.Post(SERVER_PAGE));
    }
    public static DateTime FormModified(Opap.API.Sport sport, Opap.API.Locale locale, DateTime date, int code)
    {
        var path = $@"play90Worker\Data\{GetSportDir(sport)}\{GetLocaleDir(locale)}\{date.Year:0000}\{date.Month:00}\{date.Day:00}\{code}.xml";
        return FileModified(path);
    }
    #endregion
    #region Delete
    public static string Delete(string path)
    {
        var x = new XRequest();
        x.Path = path;
        x.Action = "delete";
        return x.Post(SERVER_PAGE);
    }
    #endregion
    #region List
    public static DirectoryInfo DirList(string path)
    {
        DirectoryInfo dir = new ServerIO.DirectoryInfo();
        var x = new XRequest();
        x.Path = path;
        x.Action = "ls";
        var text = x.Post(SERVER_PAGE);
        var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var fields = line.Split('\t');
            switch (fields[0])
            {
                case "DIR":
                    dir.Dirs.Add(new ServerIO.Directory(fields[1]));
                    break;
                case "FILE":
                    dir.Files.Add(new ServerIO.File(fields[1], long.Parse(fields[2]), DateTime.Parse(fields[3])));
                    break;
            }
        }
        return dir;
    }
    public static List<File> ExistingForms(Opap.API.Sport sport, Opap.API.Locale locale, DateTime date)
    {
        var path = $@"play90Worker\Data\{GetSportDir(sport)}\{GetLocaleDir(locale)}\{date.Year:0000}\{date.Month:00}\{date.Day:00}";
        var dir = DirList(path);
        return dir.Files;
    }
    #endregion
}