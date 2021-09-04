using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LiveReader2
{
    public static class MyPaths
    {
        public static string AppDataDir
        {
            get
            {
                var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var myAppDataFolder = Path.Combine(appDataFolder, "LiveReader2");
                if (!Directory.Exists(myAppDataFolder)) Directory.CreateDirectory(myAppDataFolder);
                return myAppDataFolder;
            }
        }
        public static string SettingsXml
        {
            get
            {
                return Path.Combine(AppDataDir, "Settings.xml");
            }
        }
        public static string CredentialsXml
        {
            get
            {
                return Path.Combine(AppDataDir, "Credentials.xml");
            }
        }
        public static string WebReaderSettingsXml(string name)
        {
            return Path.Combine(AppDataDir, string.Format("{0}.xml", name));
        }
    }
}
