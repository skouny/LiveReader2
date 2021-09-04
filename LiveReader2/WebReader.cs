using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Newtonsoft.Json;
using CefSharp;
using CefSharp.WinForms;

namespace LiveReader2
{
    public partial class WebReader : Form
    {
        public class Matches : Dictionary<string, Matches.Match>
        {
            public class Match
            {
                public DateTime StartTime { get; set; }
                public string Champ { get; set; }
                public string Status { get; set; }
                public string HomeTeam { get; set; }
                public string HomeRedCards { get; set; }
                public string HomeScore { get; set; }
                public string HomeScoreET { get; set; }
                public string HomeScoreHT { get; set; }
                public string AwayTeam { get; set; }
                public string AwayRedCards { get; set; }
                public string AwayScore { get; set; }
                public string AwayScoreET { get; set; }
                public string AwayScoreHT { get; set; }
                public bool HasScoreHT()
                {
                    if (String.IsNullOrWhiteSpace(this.HomeScoreHT)) return false;
                    if (String.IsNullOrWhiteSpace(this.AwayScoreHT)) return false;
                    return true;
                }
                public bool KeepScoreHT(Match oldMatch = null)
                {
                    if (this.HasScoreHT()) return false;
                    if (FixStatus(this.Status) == MatchStatus.HT)
                    {
                        this.HomeScoreHT = this.HomeScore;
                        this.AwayScoreHT = this.AwayScore;
                        return true;
                    }
                    if (oldMatch != null && oldMatch.HasScoreHT())
                    {
                        this.HomeScoreHT = oldMatch.HomeScoreHT;
                        this.AwayScoreHT = oldMatch.AwayScoreHT;
                        return true;
                    }
                    return false;
                }
                public string Log
                {
                    get
                    {
                        return $"{HomeTeam} - {AwayTeam}: {HomeScore}-{AwayScore} ({HomeScoreHT}-{AwayScoreHT}), ET: {HomeScoreET}-{AwayScoreET}, RedCards: {HomeRedCards}-{AwayRedCards}, Minute: {Status}";
                    }
                }
                public bool Compare(Match match)
                {
                    if (this.StartTime != match.StartTime) return false;
                    if (this.Champ != match.Champ) return false;
                    if (this.Status != match.Status) return false;
                    if (this.HomeTeam != match.HomeTeam) return false;
                    if (this.HomeRedCards != match.HomeRedCards) return false;
                    if (this.HomeScore != match.HomeScore) return false;
                    if (this.HomeScoreET != match.HomeScoreET) return false;
                    if (this.HomeScoreHT != match.HomeScoreHT) return false;
                    if (this.AwayTeam != match.AwayTeam) return false;
                    if (this.AwayRedCards != match.AwayRedCards) return false;
                    if (this.AwayScore != match.AwayScore) return false;
                    if (this.AwayScoreET != match.AwayScoreET) return false;
                    if (this.AwayScoreHT != match.AwayScoreHT) return false;
                    return true;
                }
                #region Convert
                public NameValueCollection Convert(string webId, string champ, ActionWriteLog WriteLog)
                {
                    var match = new NameValueCollection();
                    match["WebId"] = webId;
                    match["Source"] = $"{SourceName.FlashScore}";
                    match["Id"] = $"{match["Source"]}#{match["WebId"]}";
                    match["Champ"] = champ;
                    match["StartTime"] = $"{this.StartTime:yyyy-MM-dd HH:mm:ss}";
                    match["HomeTeam"] = this.HomeTeam;
                    match["AwayTeam"] = this.AwayTeam;
                    int.TryParse(this.HomeRedCards, out int homeRedCards);
                    int.TryParse(this.AwayRedCards, out int awayRedCards);
                    match["RedCards"] = $"{homeRedCards}-{awayRedCards}";
                    match["Score"] = $"{this.HomeScore}-{this.AwayScore}";
                    if (this.Status != null && int.TryParse(this.Status.Replace("'", "").Split('+')[0], out int minute))
                    {
                        match["Minute"] = $"{minute}";
                        match["Status"] = $"";
                        match["StatusId"] = $"{FixStatusByMinute(minute, WriteLog)}";
                    }
                    else
                    {
                        minute = 0;
                        match["Minute"] = "";
                        match["Status"] = (this.Status != null) ? this.Status : "";
                        match["StatusId"] = $"{FixStatus(this.Status, WriteLog)}";
                    }
                    if (match["Status"] == "HalfTime" || match["Status"] == "Half Time")
                    {
                        match["ScoreHT"] = match["Score"];
                    }
                    else if (minute >= 46 && !String.IsNullOrWhiteSpace(this.HomeScoreHT) && !String.IsNullOrWhiteSpace(this.AwayScoreHT))
                    {
                        match["ScoreHT"] = $"{this.HomeScoreHT}-{this.AwayScoreHT}";
                    }
                    match["HomeScored"] = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
                    match["AwayScored"] = DateTime.MinValue.ToString("yyyy-MM-dd HH:mm:ss");
                    match["Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    return match;
                }
                private static MatchStatus FixStatus(string text, ActionWriteLog WriteLog = null)
                {
                    var status = MatchStatus.Unknown;
                    try
                    {
                        switch (text)
                        {
                            case null:
                            case "": status = MatchStatus.Waiting; break;
                            case "FRO": status = MatchStatus.Waiting; break;
                            case "Delayed": status = MatchStatus.Waiting; break;
                            case "Awaiting updates": status = MatchStatus.Waiting; break;
                            case "To finish": status = MatchStatus.Waiting; break;
                            case "Tofinish": status = MatchStatus.Waiting; break;
                            case "Finished": status = MatchStatus.FT; break;
                            case "After ET": status = MatchStatus.FT; break;
                            case "AfterET": status = MatchStatus.FT; break;
                            case "After Pen.": status = MatchStatus.FT; break;
                            case "AfterPen.": status = MatchStatus.FT; break;
                            case "Awarded": status = MatchStatus.FT; break;
                            case "Half Time": status = MatchStatus.HT; break;
                            case "HalfTime": status = MatchStatus.HT; break;
                            case "Cancelled": status = MatchStatus.Cancelled; break;
                            case "Postponed": status = MatchStatus.Cancelled; break;
                            case "Abandoned": status = MatchStatus.Cancelled; break;
                            default: WriteLog?.Invoke("ERROR! Unknown status = {0}", text); break;
                        }
                    }
                    catch (Exception ex) { WriteLog?.Invoke("ERROR! FixStatus: {0}", ex.Message); }
                    return status;
                }
                private static MatchStatus FixStatusByMinute(int minute, ActionWriteLog WriteLog)
                {
                    try
                    {
                        if (minute <= 45) return MatchStatus.Half1;
                        if (minute >= 46) return MatchStatus.Half2;
                    }
                    catch (Exception ex) { WriteLog("ERROR! FixStatusByMinute: {0}", ex.Message); }
                    return MatchStatus.Unknown;
                }
                #endregion
            }
        }
        public class ChromiumTab
        {
            #region Init
            public int DayChangeHour = 8;
            public Dictionary<string, string> XPath { get; set; } = new Dictionary<string, string>();
            public TabPage Tab { get; set; } = new TabPage();
            public SplitContainer TabSpliter { get; set; } = new SplitContainer() { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal };
            public ListBox ListLog { get; set; } = new ListBox() { Dock = DockStyle.Fill, IntegralHeight = false };
            public ChromiumWebBrowser Browser { get; set; }
            public BackgroundWorker Worker { get; set; } = new BackgroundWorker();
            public string JsonString { get; set; }
            public string JsonStringAsync
            {
                get
                {
                    return (string)this.Tab.Invoke(new Func<string>(() =>
                    {
                        return this.JsonString;
                    }));
                }
                set
                {
                    this.Tab.Invoke(new Action(() =>
                    {
                        this.JsonString = value;
                    }));
                }
            }
            public Matches JsonData { get; set; }
            public Matches JsonDataAsync
            {
                get
                {
                    return (Matches)this.Tab.Invoke(new Func<Matches>(() =>
                    {
                        return this.JsonData;
                    }));
                }
                set
                {
                    this.Tab.Invoke(new Action(() =>
                    {
                        this.JsonData = value;
                    }));
                }
            }
            public DateTime Modified { get; set; } = DateTime.MinValue;
            public ChromiumTab(string text)
            {
                // Add Controls
                this.TabSpliter.Panel2.Controls.Add(this.ListLog);
                this.Tab.Controls.Add(this.TabSpliter);
                // Set
                this.Tab.Text = text;
                this.TabSpliter.SplitterDistance = this.TabSpliter.Height;
            }
            #endregion
            #region Methods
            public DateTime Today
            {
                get
                {
                    var today = DateTime.Now;
                    if (today.Hour < this.DayChangeHour) return today.Date.AddDays(-1);
                    return today.Date;
                }
            }
            public DateTime Tomorrow
            {
                get
                {
                    return this.Today.AddDays(1);
                }
            }
            public DateTime Yesterday
            {
                get
                {
                    return this.Today.AddDays(-1);
                }
            }
            public void WriteLog(string message)
            {
                if (!this.Tab.IsHandleCreated) return;
                this.Tab.Invoke(new Action(() =>
                {
                    ListLog.Items.Insert(0, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}# {message}");
                    if (ListLog.Items.Count > 1000)
                    {
                        ListLog.Items.RemoveAt(1000);
                    }
                }));
            }
            public void WriteLog0(string format, params object[] args)
            {
                this.Tab.Invoke(new Action(() =>
                {
                    var message = string.Format(format, args);
                    ListLog.Items.Insert(0, string.Format("{0:yyyy-MM-dd HH:mm:ss}# {1}", DateTime.Now, message));
                    if (ListLog.Items.Count > 100)
                    {
                        ListLog.Items.RemoveAt(100);
                    }
                }));
            }
            #endregion
            #region Browser Cross Thread Properties
            public bool BrowserIsInitialized
            {
                get
                {
                    if (!this.Tab.IsHandleCreated) return false;
                    return (bool)this.Tab.Invoke(new Func<bool>(() =>
                    {
                        if (this.Browser == null) return false;
                        return this.Browser.IsBrowserInitialized;
                    }));
                }
            }
            public string BrowserAddress
            {
                get
                {
                    return (string)this.Tab.Invoke(new Func<string>(() =>
                    {
                        return this.Browser.Address;
                    }));
                }
            }
            public bool BrowserIsLoading
            {
                get
                {
                    return (bool)this.Tab.Invoke(new Func<bool>(() =>
                    {
                        return this.Browser.IsLoading;
                    }));
                }
            }
            #endregion
            #region Browser Cross Thread Methods
            public void BrowserInitialize(string url)
            {
                this.Tab.Invoke(new Action(() =>
                {
                    this.Browser = new ChromiumWebBrowser(url);
                    this.Browser.Dock = DockStyle.Fill;
                    this.Browser.CreateControl();
                    this.TabSpliter.Panel1.Controls.Clear();
                    this.TabSpliter.Panel1.Controls.Add(this.Browser);
                    this.Browser.Show();
                    this.Browser.AddressChanged += (senser, e) =>
                    {
                        this.Tab.Invoke(new Action(() =>
                        {
                            //labelURL.Text = this.Browser.Address;
                        }));
                    };
                }));
            }
            public void BrowserLoad(string url)
            {
                this.Tab.Invoke(new Action(() =>
                {
                    if (this.Browser.IsBrowserInitialized)
                    {
                        this.Browser.Load(url);
                    }
                }));
            }
            public void BrowserReload()
            {
                this.Tab.Invoke(new Action(() =>
                {
                    this.Browser.Reload();
                }));
            }
            public Task<JavascriptResponse> BrowserEvaluateScriptAsync(string script)
            {
                return (Task<JavascriptResponse>)this.Tab.Invoke(new Func<Task<JavascriptResponse>>(() =>
                {
                    if (this.Browser.CanExecuteJavascriptInMainFrame)
                    {
                        return this.Browser.EvaluateScriptAsync(script);
                    }
                    return null;
                }));
            }
            #endregion
            #region Browser Actions
            public void BrowserNavigate(string url, int extraTimeout = 0)
            {
                if (this.BrowserIsInitialized)
                {
                    this.BrowserLoad(url);
                }
                else
                {
                    this.BrowserInitialize(url);
                }
                this.BrowserWait(extraTimeout);
            }
            public string BrowserExecuteString(string script)
            {
                var evaluate = this.BrowserEvaluateScriptAsync(script);
                if (evaluate != null)
                {
                    var response = evaluate.Result;
                    // Read responce
                    if (response.Success)
                    {
                        var result = (string)response.Result;
                        if (!String.IsNullOrWhiteSpace(result))
                        {
                            return result;
                        }
                    }
                    else
                    {
                        this.WriteLog($"BrowserExecuteString: {response.Message}");
                    }
                }
                // Return
                return null;
            }
            public bool BrowserExecuteBool(string script)
            {
                var result = this.BrowserExecuteString(script);
                if (!String.IsNullOrWhiteSpace(result))
                {
                    return bool.Parse(result);
                }
                // Return
                return false;
            }
            public void BrowserWait(int extraTimeout = 0)
            {
                while (!this.BrowserIsInitialized || this.BrowserIsLoading)
                {
                    System.Threading.Thread.Sleep(1000);
                }
                if (extraTimeout > 0)
                {
                    System.Threading.Thread.Sleep(extraTimeout);
                }
            }
            #endregion
            #region DOM Procedures
            /// <summary>
            /// 
            /// </summary>
            public bool DomElementExists(string xpath)
            {
                // Write script
                var script = new StringBuilder();
                script.Append(@"(function(){");
                script.Append($"    var x = document.evaluate('{xpath}', document, null, 9, null).singleNodeValue;");
                script.Append(@"    if (x) return 'True';");
                script.Append(@"    return 'False';");
                script.Append(@"})();");
                // Execute
                return this.BrowserExecuteBool(script.ToString());
            }
            public bool DomElementExists(string xpath, int retries, int breakMilliseconds = 1000)
            {
                int i = 0;
                while (true)
                {
                    i++;
                    // Try
                    if (this.DomElementExists(xpath))
                    {
                        return true;
                    }
                    // Retry?
                    if (retries > 0 && retries >= i)
                    {
                        return false;
                    }
                    // Take a breath
                    this.BrowserWait(breakMilliseconds);
                }
            }
            public bool DomElementExists(string iframe, string xpath)
            {
                // Write script
                var script = new StringBuilder();
                script.Append(@"(function(){");
                script.Append($"    var iframe = document.evaluate('{iframe}', document, null, 9, null).singleNodeValue;");
                script.Append(@"    if (iframe) {");
                script.Append($"        var x = document.evaluate('{xpath}', iframe.contentDocument, null, 9, null).singleNodeValue;");
                script.Append(@"        if (x) return 'True';");
                script.Append(@"    }");
                script.Append(@"    return 'False';");
                script.Append(@"})();");
                // Execute
                return this.BrowserExecuteBool(script.ToString());
            }
            public bool DomElementExists(string iframe, string xpath, int retries, int breakMilliseconds = 1000)
            {
                int i = 0;
                while (true)
                {
                    i++;
                    // Try
                    if (this.DomElementExists(iframe, xpath))
                    {
                        return true;
                    }
                    // Retry?
                    if (retries > 0 && retries >= i)
                    {
                        return false;
                    }
                    // Take a breath
                    this.BrowserWait(breakMilliseconds);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public bool DomElementClick(string xpath)
            {
                // Write script
                var script = new StringBuilder();
                script.Append(@"(function(){");
                script.Append($"    var x = document.evaluate('{xpath}', document, null, 9, null).singleNodeValue;");
                script.Append(@"    if (x) {");
                script.Append(@"        x.click();");
                script.Append(@"        return 'True';");
                script.Append(@"    }");
                script.Append(@"    return 'False';");
                script.Append(@"})();");
                // Execute
                return this.BrowserExecuteBool(script.ToString());
            }
            public bool DomElementClick(string xpath, int retries, int breakMilliseconds = 1000)
            {
                int i = 0;
                while (true)
                {
                    i++;
                    // Try
                    if (this.DomElementClick(xpath))
                    {
                        return true;
                    }
                    // Retry?
                    if (retries > 0 && retries >= i)
                    {
                        return false;
                    }
                    // Take a breath
                    this.BrowserWait(breakMilliseconds);
                }
            }
            public bool DomElementClick(string iframe, string xpath)
            {
                // Write script
                var script = new StringBuilder();
                script.Append(@"(function(){");
                script.Append($"    var iframe = document.evaluate('{iframe}', document, null, 9, null).singleNodeValue;");
                script.Append(@"    if (iframe) {");
                script.Append($"        var x = document.evaluate('{xpath}', iframe.contentDocument, null, 9, null).singleNodeValue;");
                script.Append(@"        if (x) {");
                script.Append(@"            x.click();");
                script.Append(@"            return 'True';");
                script.Append(@"        }");
                script.Append(@"    }");
                script.Append(@"    return 'False';");
                script.Append(@"})();");
                // Execute
                return this.BrowserExecuteBool(script.ToString());
            }
            public bool DomElementClick(string iframe, string xpath, int retries, int breakMilliseconds = 1000)
            {
                int i = 0;
                while (true)
                {
                    i++;
                    // Try
                    if (this.DomElementClick(iframe, xpath))
                    {
                        return true;
                    }
                    // Retry?
                    if (retries > 0 && retries >= i)
                    {
                        return false;
                    }
                    // Take a breath
                    this.BrowserWait(breakMilliseconds);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public bool DomSetElementFocus(string xpath)
            {
                // Write script
                var script = new StringBuilder();
                script.Append(@"(function(){");
                script.Append($"    var x = document.evaluate('{xpath}', document, null, 9, null).singleNodeValue;");
                script.Append(@"    if (x) {");
                script.Append($"        x.focus();");
                script.Append(@"        return 'True';");
                script.Append(@"    }");
                script.Append(@"    return 'False';");
                script.Append(@"})();");
                // Execute
                return this.BrowserExecuteBool(script.ToString());
            }
            public bool DomSetElementFocus(string xpath, int retries, int breakMilliseconds = 1000)
            {
                int i = 0;
                while (true)
                {
                    i++;
                    // Try
                    if (this.DomSetElementFocus(xpath))
                    {
                        return true;
                    }
                    // Retry?
                    if (retries > 0 && retries >= i)
                    {
                        return false;
                    }
                    // Take a breath
                    this.BrowserWait(breakMilliseconds);
                }
            }
            public bool DomSetElementFocus(string iframe, string xpath)
            {
                // Write script
                var script = new StringBuilder();
                script.Append(@"(function(){");
                script.Append($"    var iframe = document.evaluate('{iframe}', document, null, 9, null).singleNodeValue;");
                script.Append(@"    if (iframe) {");
                script.Append($"        var x = document.evaluate('{xpath}', iframe.contentDocument, null, 9, null).singleNodeValue;");
                script.Append(@"        if (x) {");
                script.Append($"            x.focus();");
                script.Append(@"            return 'True';");
                script.Append(@"        }");
                script.Append(@"    }");
                script.Append(@"    return 'False';");
                script.Append(@"})();");
                // Execute
                return this.BrowserExecuteBool(script.ToString());
            }
            public bool DomSetElementFocus(string iframe, string xpath, int retries, int breakMilliseconds = 1000)
            {
                int i = 0;
                while (true)
                {
                    i++;
                    // Try
                    if (this.DomSetElementFocus(iframe, xpath))
                    {
                        return true;
                    }
                    // Retry?
                    if (retries > 0 && retries >= i)
                    {
                        return false;
                    }
                    // Take a breath
                    this.BrowserWait(breakMilliseconds);
                }
            }
            #endregion
            #region DOM Set Functions
            /// <summary>
            /// 
            /// </summary>
            public bool DomSetElementValue(string xpath, string value)
            {
                // Write script
                var script = new StringBuilder();
                script.Append(@"(function(){");
                script.Append($"    var x = document.evaluate('{xpath}', document, null, 9, null).singleNodeValue;");
                script.Append(@"    if (x) {");
                script.Append($"        x.value = '{value}';");
                //script.Append($"        x.onchange();"); // Fire change event (not fired when value is changed by javascript)
                script.Append(@"        return 'True';");
                script.Append(@"    }");
                script.Append(@"    return 'False';");
                script.Append(@"})();");
                // Execute
                return this.BrowserExecuteBool(script.ToString());
            }
            public bool DomSetElementValue(string xpath, string value, int retries)
            {
                int i = 0;
                while (true)
                {
                    i++;
                    // Try
                    if (this.DomSetElementValue(xpath, value))
                    {
                        return true;
                    }
                    // Retry?
                    if (retries > 0 && retries >= i)
                    {
                        return false;
                    }
                    // Take a breath
                    this.BrowserWait(1000);
                }
            }
            public bool DomSetElementValue(string iframe, string xpath, string value)
            {
                // Write script
                var script = new StringBuilder();
                script.Append(@"(function(){");
                script.Append($"    var iframe = document.evaluate('{iframe}', document, null, 9, null).singleNodeValue;");
                script.Append(@"    if (iframe) {");
                script.Append($"        var x = document.evaluate('{xpath}', iframe.contentDocument, null, 9, null).singleNodeValue;");
                script.Append(@"        if (x) {");
                script.Append($"            x.value = '{value}';");
                //script.Append($"            x.onchange();"); // Fire change event (not fired when value is changed by javascript)
                script.Append(@"            return 'True';");
                script.Append(@"        }");
                script.Append(@"    }");
                script.Append(@"    return 'False';");
                script.Append(@"})();");
                // Execute
                return this.BrowserExecuteBool(script.ToString());
            }
            public bool DomSetElementValue(string iframe, string xpath, string value, int retries)
            {
                int i = 0;
                while (true)
                {
                    i++;
                    // Try
                    if (this.DomSetElementValue(iframe, xpath, value))
                    {
                        return true;
                    }
                    // Retry?
                    if (retries > 0 && retries >= i)
                    {
                        return false;
                    }
                    // Take a breath
                    this.BrowserWait(1000);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public bool DomSetInputBox(string xpath, string value)
            {
                return this.DomSetElementValue(xpath, value);
            }
            public bool DomSetInputBox(string xpath, string value, int retries)
            {
                return this.DomSetElementValue(xpath, value, retries);
            }
            public bool DomSetInputBox(string iframe, string xpath, string value)
            {
                return this.DomSetElementValue(iframe, xpath, value);
            }
            public bool DomSetInputBox(string iframe, string xpath, string value, int retries)
            {
                return this.DomSetElementValue(iframe, xpath, value, retries);
            }
            #endregion
            #region DOM Get Functions
            /// <summary>
            /// 
            /// </summary>
            public string DomGetElementText(string xpath)
            {
                // Write script
                var script = new StringBuilder();
                script.Append(@"(function(){");
                script.Append($"    var x = document.evaluate('{xpath}', document, null, 9, null).singleNodeValue;");
                script.Append(@"    if (x) {");
                script.Append(@"        return x.innerText.replace(/(\r\n|\n|\r)/gm,'').trim();");
                script.Append(@"    }");
                script.Append(@"})();");
                // Execute
                return this.BrowserExecuteString(script.ToString());
            }
            public string DomGetElementText(string xpath, int retries)
            {
                int i = 0;
                while (true)
                {
                    i++;
                    // Try
                    var result = this.DomGetElementText(xpath);
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        return result;
                    }
                    // Retry?
                    if (retries > 0 && retries >= i)
                    {
                        return null;
                    }
                    // Take a breath
                    this.BrowserWait(1000);
                }
            }
            public string DomGetElementText(string iframe, string xpath)
            {
                // Write script
                var script = new StringBuilder();
                script.Append(@"(function(){");
                script.Append($"    var iframe = document.evaluate('{iframe}', document, null, 9, null).singleNodeValue;");
                script.Append(@"    if (iframe) {");
                script.Append($"        var x = document.evaluate('{xpath}', iframe.contentDocument, null, 9, null).singleNodeValue;");
                script.Append(@"        if (x) {");
                script.Append(@"            return x.innerText.replace(/(\r\n|\n|\r)/gm,'').trim();");
                script.Append(@"        }");
                script.Append(@"    }");
                script.Append(@"})();");
                // Execute
                return this.BrowserExecuteString(script.ToString());
            }
            public string DomGetElementText(string iframe, string xpath, int retries)
            {
                int i = 0;
                while (true)
                {
                    i++;
                    // Try
                    var result = this.DomGetElementText(iframe, xpath);
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        return result;
                    }
                    // Retry?
                    if (retries > 0 && retries >= i)
                    {
                        return null;
                    }
                    // Take a breath
                    this.BrowserWait(1000);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public string DomGetSelectText(string xpath)
            {
                // Write script
                var script = new StringBuilder();
                script.Append(@"(function(){");
                script.Append($"    var x = document.evaluate('{xpath}', document, null, 9, null).singleNodeValue;");
                script.Append(@"    if (x) {");
                script.Append("         return x.options[x.selectedIndex].text;");
                script.Append(@"    }");
                script.Append(@"})();");
                // Execute
                return this.BrowserExecuteString(script.ToString());
            }
            public string DomGetSelectText(string xpath, int retries)
            {
                int i = 0;
                while (true)
                {
                    i++;
                    // Try
                    var result = this.DomGetSelectText(xpath);
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        return result;
                    }
                    // Retry?
                    if (retries > 0 && retries >= i)
                    {
                        return null;
                    }
                    // Take a breath
                    this.BrowserWait(1000);
                }
            }
            public string DomGetSelectText(string iframe, string xpath)
            {
                // Write script
                var script = new StringBuilder();
                script.Append(@"(function(){");
                script.Append($"    var iframe = document.evaluate('{iframe}', document, null, 9, null).singleNodeValue;");
                script.Append(@"    if (iframe) {");
                script.Append($"        var x = document.evaluate('{xpath}', iframe.contentDocument, null, 9, null).singleNodeValue;");
                script.Append(@"        if (x) {");
                script.Append("             return x.options[x.selectedIndex].text;");
                script.Append(@"        }");
                script.Append(@"    }");
                script.Append(@"})();");
                // Execute
                return this.BrowserExecuteString(script.ToString());
            }
            public string DomGetSelectText(string iframe, string xpath, int retries)
            {
                int i = 0;
                while (true)
                {
                    i++;
                    // Try
                    var result = this.DomGetSelectText(iframe, xpath);
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        return result;
                    }
                    // Retry?
                    if (retries > 0 && retries >= i)
                    {
                        return null;
                    }
                    // Take a breath
                    this.BrowserWait(1000);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public string DomGetTableCSV(string xpath)
            {
                // Write script
                var script = new StringBuilder();
                script.Append(@"(function(){");
                script.Append($"    var table = document.evaluate('{xpath}', document, null, 9, null).singleNodeValue;");
                script.Append(@"    if (table) {");
                script.Append(@"        var csv = '';");
                script.Append(@"        for (var i = 0; i < table.rows.length; i++) {");
                script.Append(@"            var row = table.rows[i];");
                script.Append(@"            for (var j = 0; j < row.cells.length; j++) {");
                script.Append(@"                csv += row.cells[j].innerText.replace(/(\r\n|\n|\r)/gm,'').trim() + '\t';");
                script.Append(@"            }");
                script.Append(@"            csv += '\r\n';");
                script.Append(@"        }");
                script.Append(@"    return csv;");
                script.Append(@"    }");
                script.Append(@"})();");
                // Execute
                return this.BrowserExecuteString(script.ToString());
            }
            public string DomGetTableCSV(string xpath, int retries)
            {
                int i = 0;
                while (true)
                {
                    i++;
                    // Try
                    var result = this.DomGetTableCSV(xpath);
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        return result;
                    }
                    // Retry?
                    if (retries > 0 && retries >= i)
                    {
                        return null;
                    }
                    // Take a breath
                    this.BrowserWait(1000);
                }
            }
            public string DomGetTableCSV(string iframe, string xpath)
            {
                // Write script
                var script = new StringBuilder();
                script.Append(@"(function(){");
                script.Append($"    var iframe = document.evaluate('{iframe}', document, null, 9, null).singleNodeValue;");
                script.Append(@"    if (iframe) {");
                script.Append($"        var table = document.evaluate('{xpath}', iframe.contentDocument, null, 9, null).singleNodeValue;");
                script.Append(@"        if (table) {");
                script.Append(@"            var csv = '';");
                script.Append(@"            for (var i = 0; i < table.rows.length; i++) {");
                script.Append(@"                var row = table.rows[i];");
                script.Append(@"                for (var j = 0; j < row.cells.length; j++) {");
                script.Append(@"                    csv += row.cells[j].innerText.replace(/(\r\n|\n|\r)/gm,'').trim() + '\t';");
                script.Append(@"                }");
                script.Append(@"                csv += '\r\n';");
                script.Append(@"            }");
                script.Append(@"            return csv;");
                script.Append(@"        }");
                script.Append(@"    }");
                script.Append(@"})();");
                // Execute
                return this.BrowserExecuteString(script.ToString());
            }
            public string DomGetTableCSV(string iframe, string xpath, int retries)
            {
                int i = 0;
                while (true)
                {
                    i++;
                    // Try
                    var result = this.DomGetTableCSV(iframe, xpath);
                    if (!String.IsNullOrWhiteSpace(result))
                    {
                        return result;
                    }
                    // Retry?
                    if (retries > 0 && retries >= i)
                    {
                        return null;
                    }
                    // Take a breath
                    this.BrowserWait(1000);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public string DomDownloadText(string url)
            {
                // Write script
                var script = new StringBuilder();
                script.Append(@"(function(){");
                script.Append(@"    var xhttp = new XMLHttpRequest();");
                script.Append($"    xhttp.open(\"GET\", \"{url}\", false);");
                script.Append(@"    xhttp.send();");
                script.Append(@"    return xhttp.responseText;");
                script.Append(@"})();");
                // Execute
                return this.BrowserExecuteString(script.ToString());
            }
            #endregion
        }
        #region Init
        //private ChromiumTab BrowserTab0 { get; set; } = new ChromiumTab("Football.Opap");
        private ChromiumTab BrowserTab1 { get; set; } = new ChromiumTab("Football.FlashScore.Today");
        private ChromiumTab BrowserTab2 { get; set; } = new ChromiumTab("Football.FlashScore.Τomorrow");
        //private ChromiumTab BrowserTab3 { get; set; } = new ChromiumTab("Football.Futbal24");
        //private ChromiumTab BrowserTab4 { get; set; } = new ChromiumTab("Basket.Opap");
        //private ChromiumTab BrowserTab5 { get; set; } = new ChromiumTab("Basket.FlashScore.Today");
        //private ChromiumTab BrowserTab6 { get; set; } = new ChromiumTab("Basket.FlashScore.Τomorrow");
        public WebReader()
        {
            this.InitializeComponent();
            this.InitTabs();
        }
        public void InitTabs()
        {
            this.Width = 1050;
            this.Height = 800;
            // Add
            //this.tabControlMain.TabPages.Add(this.BrowserTab0.Tab);
            this.tabControlMain.TabPages.Add(this.BrowserTab1.Tab);
            this.tabControlMain.TabPages.Add(this.BrowserTab2.Tab);
            //this.tabControlMain.TabPages.Add(this.BrowserTab3.Tab);
            //this.tabControlMain.TabPages.Add(this.BrowserTab4.Tab);
            //this.tabControlMain.TabPages.Add(this.BrowserTab5.Tab);
            //this.tabControlMain.TabPages.Add(this.BrowserTab6.Tab);
            // Football.Opap
            //this.BrowserTab0.Worker.DoWork += (sender, e) =>
            //{

            //};
            // Football.FlashScore.Today
            this.BrowserTab1.Worker.DoWork += (sender, e) =>
            {
                // Init
                var tab = this.BrowserTab1;
                tab.DayChangeHour = 4;
                tab.XPath["Yesterday"] = "//*[@id=\"live-table\"]//*[contains(@title, \"Previous day\")]/div";
                tab.XPath["Today"] = "//*[@id=\"live-table\"]//*[contains(@class, \"calendar__datepicker\")]";
                tab.XPath["Tomorrow"] = "//*[@id=\"live-table\"]//*[contains(@title, \"Next day\")]/div";
                var takeAction = (Action<DateTime, Matches>)e.Argument;
                // Load
                var tickBegin = Environment.TickCount;
                tab.BrowserNavigate("https://www.flashscore.com/");
                var took = Environment.TickCount - tickBegin;
                tab.WriteLog($"Web Site Ready => Took: {took:#,##0}ms");
                while (true)
                {
                    try
                    {
                        // Ensure the correct day is selected
                        var selectedDay = this.ParseDay(tab.DomGetElementText(tab.XPath["Today"]), "dd/MM");
                        var targetDay = tab.Today;
                        if (selectedDay == targetDay)
                        {
                            // Read Data
                            tickBegin = Environment.TickCount;
                            var jsonString = tab.BrowserExecuteString(Properties.Resources.FlashScoreFootball);
                            // If Changed
                            if (tab.JsonStringAsync != jsonString)
                            {
                                // Set
                                tab.JsonStringAsync = jsonString;
                                tab.Modified = DateTime.Now;
                                // Deserialize
                                var newData = JsonConvert.DeserializeObject<Matches>(jsonString);
                                tab.WriteLog($"Data Changed => Length: {jsonString.Length:#,##0}, Matches: {newData.Values.Count():#,##0}, Took: {(Environment.TickCount - tickBegin):#,##0}ms");
                                // Process
                                var oldData = tab.JsonDataAsync;
                                foreach (var key in newData.Keys)
                                {
                                    var oldMatch = (oldData != null && oldData.ContainsKey(key)) ? oldData[key] : null;
                                    // Keep ScoreHT (not always available in flashscore.com, so remember it)
                                    newData[key].KeepScoreHT(oldMatch);
                                    // Log match changes
                                    if (oldMatch != null && !newData[key].Compare(oldMatch))
                                    {
                                        tab.WriteLog($"Match Changed: {newData[key].Log}");
                                    }
                                }
                                // Assign new data
                                tab.JsonDataAsync = newData;
                                // Take Action
                                takeAction(selectedDay, newData);
                            }
                            // Refresh automatically if frozen
                            else if (tab.Modified > DateTime.MinValue && (DateTime.Now - tab.Modified).TotalMinutes > 5)
                            {
                                var query = tab.JsonDataAsync.Where(x => DateTime.Now > x.Value.StartTime && DateTime.Now < x.Value.StartTime.AddHours(2));
                                if (query.Count() > 0)
                                {
                                    tab.DomElementClick(tab.XPath["Tomorrow"]);
                                    tab.WriteLog($"Data Freezed: Tomorrow Clicked");
                                    System.Threading.Thread.Sleep(3000);
                                }
                            }
                        }
                        else if (selectedDay > targetDay)
                        {
                            tab.DomElementClick(tab.XPath["Yesterday"]);
                            tab.WriteLog($"Selected Day: {selectedDay:yyyy-MM-dd}, Target Day: {targetDay:yyyy-MM-dd}, Yesterday Clicked");
                            System.Threading.Thread.Sleep(3000);
                        }
                        else if (selectedDay < targetDay)
                        {
                            tab.DomElementClick(tab.XPath["Tomorrow"]);
                            tab.WriteLog($"Selected Day: {selectedDay:yyyy-MM-dd}, Target Day: {targetDay:yyyy-MM-dd}, Tomorrow Clicked");
                            System.Threading.Thread.Sleep(3000);
                        }
                    }
                    catch (Exception ex)
                    {
                        tab.WriteLog($"Loop Error => {ex.Message}");
                    }
                    finally // Break
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            };
            // Football.FlashScore.Τomorrow
            this.BrowserTab2.Worker.DoWork += (sender, e) =>
            {
                var tab = this.BrowserTab2;
                tab.DayChangeHour = 4;
                tab.XPath["Yesterday"] = "//*[@id=\"live-table\"]//*[contains(@title, \"Previous day\")]/div";
                tab.XPath["Today"] = "//*[@id=\"live-table\"]//*[contains(@class, \"calendar__datepicker\")]";
                tab.XPath["Tomorrow"] = "//*[@id=\"live-table\"]//*[contains(@title, \"Next day\")]/div";
                var takeAction = (Action<DateTime, Matches>)e.Argument;
                // Load
                var tickBegin = Environment.TickCount;
                tab.BrowserNavigate("https://www.flashscore.com/");
                var took = Environment.TickCount - tickBegin;
                tab.WriteLog($"Web Site Ready => Took: {took:#,##0}ms");
                while (true)
                {
                    try
                    {
                        // Ensure the correct day is selected
                        var selectedDay = this.ParseDay(tab.DomGetElementText(tab.XPath["Today"]), "dd/MM");
                        var targetDay = tab.Tomorrow;
                        if (selectedDay == targetDay)
                        {
                            // Read Data
                            tickBegin = Environment.TickCount;
                            var jsonString = tab.BrowserExecuteString(Properties.Resources.FlashScoreFootball);
                            // If Changed
                            if (tab.JsonStringAsync != jsonString)
                            {
                                // Set
                                tab.JsonStringAsync = jsonString;
                                tab.Modified = DateTime.Now;
                                // Deserialize
                                var newData = JsonConvert.DeserializeObject<Matches>(jsonString);
                                tab.WriteLog($"Data Changed => Length: {jsonString.Length:#,##0}, Matches: {newData.Values.Count():#,##0}, Took: {(Environment.TickCount - tickBegin):#,##0}ms");
                                // Process
                                var oldData = tab.JsonDataAsync;
                                foreach (var key in newData.Keys)
                                {
                                    var oldMatch = (oldData != null && oldData.ContainsKey(key)) ? oldData[key] : null;
                                    // Keep ScoreHT (not always available in flashscore.com, so remember it)
                                    newData[key].KeepScoreHT(oldMatch);
                                    // Log match changes
                                    if (oldMatch != null && !newData[key].Compare(oldMatch))
                                    {
                                        tab.WriteLog($"Match Changed: {newData[key].Log}");
                                    }
                                }
                                // Assign new data
                                tab.JsonDataAsync = newData;
                                // Take Action
                                takeAction(selectedDay, newData);
                            }
                            // Refresh automatically if frozen
                            else if (tab.Modified > DateTime.MinValue && (DateTime.Now - tab.Modified).TotalMinutes > 5)
                            {
                                var query = tab.JsonDataAsync.Where(x => DateTime.Now > x.Value.StartTime && DateTime.Now < x.Value.StartTime.AddHours(2));
                                if (query.Count() > 0)
                                {
                                    tab.DomElementClick(tab.XPath["Tomorrow"]);
                                    tab.WriteLog($"Data Freezed: Tomorrow Clicked");
                                    System.Threading.Thread.Sleep(3000);
                                }
                            }
                        }
                        else if (selectedDay > targetDay)
                        {
                            tab.DomElementClick(tab.XPath["Yesterday"]);
                            tab.WriteLog($"Selected Day: {selectedDay:yyyy-MM-dd}, Target Day: {targetDay:yyyy-MM-dd}, Yesterday Clicked");
                            System.Threading.Thread.Sleep(3000);
                        }
                        else if (selectedDay < targetDay)
                        {
                            tab.DomElementClick(tab.XPath["Tomorrow"]);
                            tab.WriteLog($"Selected Day: {selectedDay:yyyy-MM-dd}, Target Day: {targetDay:yyyy-MM-dd}, Tomorrow Clicked");
                            System.Threading.Thread.Sleep(3000);
                        }
                    }
                    catch (Exception ex)
                    {
                        tab.WriteLog($"Loop Error => {ex.Message}");
                    }
                    finally // Break
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            };
            
            // Run with callback
            this.Load += (sender, e) =>
            {
                //this.BrowserTab0.Worker.RunWorkerAsync(new Action<DateTime, Matches>((selectedDay, data) =>
                //{

                //}));
                this.BrowserTab1.Worker.RunWorkerAsync(new Action<DateTime, Matches>((selectedDay, data) =>
                {
                    var tab = this.BrowserTab1;
                    // Post changes to database
                    var existingMatches = GetExistingMatches(SportName.Football, SourceName.FlashScore, selectedDay, tab.WriteLog0);
                    var changedMatches = new List<NameValueCollection>();
                    foreach (var match in data)
                    {
                        var newMatch = match.Value.Convert(match.Key, match.Value.Champ, tab.WriteLog0);
                        if (existingMatches.ContainsKey(match.Key))
                        {
                            var curMatch = existingMatches[match.Key];
                            // Update
                            if (UpdateMatch(ref curMatch, newMatch, SportName.Football))
                            {
                                changedMatches.Add(curMatch);
                                tab.WriteLog($"Match Changed: {match.Value.Log}");
                            }
                            // Remove
                            existingMatches.Remove(match.Key);
                        }
                        else
                        {
                            changedMatches.Add(newMatch);
                        }
                    }
                    // Upload
                    UpdateManager.UploadMatchesLive(SportName.Football, changedMatches, tab.WriteLog0);
                    // Remove the rest
                    foreach (var existingMatch in existingMatches)
                    {
                        var msg = WebDB.table_live_mix_delete(SportName.Football, SourceName.FlashScore, existingMatch.Key);
                        tab.WriteLog(msg);
                    }
                }));
                this.BrowserTab2.Worker.RunWorkerAsync(new Action<DateTime, Matches>((selectedDay, data) =>
                {
                    var tab = this.BrowserTab2;
                    // Post changes to database
                    var existingMatches = GetExistingMatches(SportName.Football, SourceName.FlashScore, selectedDay, tab.WriteLog0);
                    var changedMatches = new List<NameValueCollection>();
                    foreach (var match in data)
                    {
                        var newMatch = match.Value.Convert(match.Key, match.Value.Champ, tab.WriteLog0);
                        if (existingMatches.ContainsKey(match.Key))
                        {
                            var curMatch = existingMatches[match.Key];
                            // Update
                            if (UpdateMatch(ref curMatch, newMatch, SportName.Football))
                            {
                                changedMatches.Add(curMatch);
                                tab.WriteLog($"Match Changed: {match.Value.Log}");
                            }
                            // Remove
                            existingMatches.Remove(match.Key);
                        }
                        else
                        {
                            changedMatches.Add(newMatch);
                        }
                    }
                    // Upload
                    UpdateManager.UploadMatchesLive(SportName.Football, changedMatches, tab.WriteLog0);
                    // Remove the rest
                    foreach (var existingMatch in existingMatches)
                    {
                        var msg = WebDB.table_live_mix_delete(SportName.Football, SourceName.FlashScore, existingMatch.Key);
                        tab.WriteLog(msg);
                    }
                }));
                //this.BrowserTab3.Worker.RunWorkerAsync(new Action<DateTime, Matches>((selectedDay, data) =>
                //{

                //}));
                //this.BrowserTab4.Worker.RunWorkerAsync(new Action<DateTime, Matches>((selectedDay, data) =>
                //{

                //}));
                //this.BrowserTab5.Worker.RunWorkerAsync(new Action<DateTime, Matches>((selectedDay, data) =>
                //{

                //}));
                //this.BrowserTab6.Worker.RunWorkerAsync(new Action<DateTime, Matches>((selectedDay, data) =>
                //{

                //}));
            };
        }
        #endregion
        #region Update Methods
        public static Dictionary<string, NameValueCollection> GetExistingMatches(SportName sport, SourceName source, DateTime selectedDay, ActionWriteLog WriteLog)
        {
            var matches = new Dictionary<string, NameValueCollection>();
            int retry = 0;
            while (retry < 10)
            {
                try
                {
                    var xUrl = WebDB.url_table_live_mix_get(sport, source);
                    var xDoc = XDocument.Load(xUrl);
                    foreach (var element in xDoc.Root.Elements().Where(x => selectedDay.Date == DateTime.Parse(x.Attribute("StartTime").Value).Date))
                    {
                        var match = new NameValueCollection();
                        match["Id"] = element.Attribute("Id").Value;
                        match["Source"] = element.Attribute("Source").Value;
                        match["WebId"] = element.Attribute("WebId").Value;
                        match["StartTime"] = element.Attribute("StartTime").Value;
                        match["Champ"] = element.Attribute("Champ").Value;
                        match["HomeTeam"] = element.Attribute("HomeTeam").Value;
                        match["AwayTeam"] = element.Attribute("AwayTeam").Value;
                        match["Score"] = element.Attribute("Score").Value;
                        match["ScoreHT"] = element.Attribute("ScoreHT").Value;
                        if (sport == SportName.Football)
                        {
                            match["YellowCards"] = element.Attribute("YellowCards").Value;
                            match["RedCards"] = element.Attribute("RedCards").Value;
                            match["CornerKicks"] = element.Attribute("CornerKicks").Value;
                            match["Shots"] = element.Attribute("Shots").Value;
                            match["ShotsOnGoal"] = element.Attribute("ShotsOnGoal").Value;
                            match["Fouls"] = element.Attribute("Fouls").Value;
                            match["BallPossession"] = element.Attribute("BallPossession").Value;
                            match["Saves"] = element.Attribute("Saves").Value;
                            match["Offsides"] = element.Attribute("Offsides").Value;
                            match["KickOff"] = element.Attribute("KickOff").Value;
                            match["HomeEvents"] = element.Attribute("HomeEvents").Value;
                            match["AwayEvents"] = element.Attribute("AwayEvents").Value;
                        }
                        else if (sport == SportName.Basket)
                        {
                            match["ScoreQ1"] = element.Attribute("ScoreQ1").Value;
                            match["ScoreQ2"] = element.Attribute("ScoreQ2").Value;
                            match["ScoreQ3"] = element.Attribute("ScoreQ3").Value;
                            match["ScoreQ4"] = element.Attribute("ScoreQ4").Value;
                            match["StandingPoints"] = element.Attribute("StandingPoints").Value;
                        }
                        match["Minute"] = element.Attribute("Minute").Value;
                        match["Status"] = element.Attribute("Status").Value;
                        match["HomeScored"] = element.Attribute("HomeScored").Value;
                        match["AwayScored"] = element.Attribute("AwayScored").Value;
                        match["Modified"] = element.Attribute("Modified").Value;
                        matches.Add(match["WebId"], match);
                    }
                    break;
                }
                catch (Exception ex) { WriteLog("ERROR! GetExistingMatches: {0} [try {1}]", ex.Message, retry++); }
            }
            return matches;
        }
        public static bool UpdateMatch(ref NameValueCollection curMatch, NameValueCollection newMatch, SportName sport)
        {
            bool modified = false;
            if (newMatch != null)
            {
                string prevValue = null;
                if (UpdateValue(ref curMatch, newMatch, "StartTime", ref prevValue)) { modified = true; }
                if (UpdateValue(ref curMatch, newMatch, "Champ", ref prevValue)) { modified = true; }
                if (UpdateValue(ref curMatch, newMatch, "HomeTeam", ref prevValue)) { modified = true; }
                if (UpdateValue(ref curMatch, newMatch, "AwayTeam", ref prevValue)) { modified = true; }
                if (UpdateValue(ref curMatch, newMatch, "ScoreHT", ref prevValue)) { modified = true; }
                if (UpdateValue(ref curMatch, newMatch, "Score", ref prevValue))
                {
                    modified = true;
                    if (Score.HomeScored(newMatch["Score"], prevValue))
                    {
                        curMatch["HomeScored"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    if (Score.AwayScored(newMatch["Score"], prevValue))
                    {
                        curMatch["AwayScored"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }
                if (UpdateValue(ref curMatch, newMatch, "Status", ref prevValue)) { modified = true; }
                UpdateValue(ref curMatch, newMatch, "StatusId", ref prevValue); // The same with 'Status', problem if checked.
                if (UpdateValue(ref curMatch, newMatch, "Minute", ref prevValue)) { modified = true; }
                if (sport == SportName.Football)
                {
                    if (UpdateValue(ref curMatch, newMatch, "YellowCards", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "RedCards", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "CornerKicks", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "Shots", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "ShotsOnGoal", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "Fouls", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "BallPossession", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "Saves", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "Offsides", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "KickOff", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "HomeEvents", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "AwayEvents", ref prevValue)) { modified = true; }
                }
                else if (sport == SportName.Basket)
                {
                    if (UpdateValue(ref curMatch, newMatch, "ScoreQ1", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "ScoreQ2", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "ScoreQ3", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "ScoreQ4", ref prevValue)) { modified = true; }
                    if (UpdateValue(ref curMatch, newMatch, "StandingPoints", ref prevValue)) { modified = true; }
                }
                if (modified)
                {
                    curMatch["Modified"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            return modified;
        }
        private static bool UpdateValue(ref NameValueCollection curMatch, NameValueCollection newMatch, string key, ref string prevValue)
        {
            bool modified = false;
            string newValue = null;
            prevValue = null;
            // if the new match has the key
            if (newMatch.AllKeys.Contains(key))
            {
                // set its value to cur match, if not already has the same value...
                if (curMatch.AllKeys.Contains(key))
                {
                    if (curMatch[key] != newMatch[key])
                    {
                        prevValue = curMatch[key];
                        newValue = newMatch[key];
                        curMatch[key] = newMatch[key];
                        modified = true;
                    }
                }
                else
                {
                    curMatch[key] = newMatch[key];
                    modified = true;
                }
            }
            return modified;
        }
        private static void WriteMatchLog(ActionWriteLog WriteLog, NameValueCollection newMatch, string action)
        {
            WriteLog("Match {0}: [{1}] {2} - {3} {4} ({5}) {6} {7}", action
                , newMatch["StartTime"], newMatch["HomeTeam"], newMatch["AwayTeam"], newMatch["Score"]
                , newMatch["ScoreHT"], newMatch["Status"], newMatch["Minute"]);
        }
        #endregion
        #region ToolBox
        public DateTime ParseDay(string s, string format)
        {
            if (format == "dd/MM") // parse dd/MM date (flashscore.com)
            {
                var today = DateTime.Now.Date;
                var d = s.Split(' ')[0].Split('/');
                var day = int.Parse(d[0]);
                var month = int.Parse(d[1]);
                var year = today.Year;
                if (month == 1 && today.Month == 12)
                {
                    year += 1;
                }
                else if (month == 12 && today.Month == 1)
                {
                    year -= 1;
                }
                return new DateTime(year, month, day);
            }
            else
            {
                return DateTime.Parse(s);
            }
        }
        #endregion
    }
}