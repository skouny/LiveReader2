using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

public class XRequest
{
    public string Path { get; set; }
    public string Action { get; set; }
    public string Flag { get; set; }
    public string Text { get; set; }
    public XRequest() { }
    public string RequestText { get { return $"Path:{this.Path}\tAction:{this.Action}\tFlag:{this.Flag}{Environment.NewLine}{this.Text}"; } }
    public byte[] RequestBytes { get { return Encoding.UTF8.GetBytes(this.RequestText); } }
    public string Post(string url, bool gzip = true)
    {
        var bytes = this.RequestBytes;
        if (gzip) bytes = GZip.CompressBytes(bytes);
        var responseBytes = TimedWebClient.PostData(bytes, url);
        if (gzip) responseBytes = GZip.DecompressBytes(responseBytes);
        return Encoding.UTF8.GetString(responseBytes);
    }
    public static string StreamText(Stream stream)
    {
        string text = null;
        using (var reader = new StreamReader(stream))
        {
            text = reader.ReadToEnd();
        }
        return text;
    }
    public static byte[] StreamBytes(Stream stream)
    {
        byte[] bytes = null;
        using (var reader = new BinaryReader(stream))
        {
            bytes = reader.ReadBytes((int)stream.Length);
        }
        return bytes;
    }
    public static XRequest Read(Stream inputStream, bool gzip = true)
    {
        var xRequest = new XRequest();
        var bytes = StreamBytes(inputStream);
        if (gzip) bytes = GZip.DecompressBytes(bytes);
        var text = Encoding.UTF8.GetString(bytes);
        var lines = text.Split(new string[] { Environment.NewLine }, 2, StringSplitOptions.None);
        if (lines.Length == 2)
        {
            var attributes = lines[0].Split('\t');
            foreach (var attribute in attributes)
            {
                var pair = attribute.Split(new string[] { ":" }, 2, StringSplitOptions.None);
                if (pair.Length == 2)
                {
                    var name = pair[0].ToLower();
                    var value = pair[1];
                    switch (name)
                    {
                        case "path": xRequest.Path = value; break;
                        case "action": xRequest.Action = value; break;
                        case "flag": xRequest.Flag = value; break;
                    }
                }
            }
            xRequest.Text = lines[1];
        }
        return xRequest;
    }
}