using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using CefSharp;
using CefSharp.WinForms;

namespace LiveReader2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Settings.PrevInstance)
            {
                // Wait if its restart
                for (int i = 0; i < 10; i++)
                {
                    if (!Settings.PrevInstance) break;
                    System.Threading.Thread.Sleep(2000);
                }
                // Check again
                if (Settings.PrevInstance) return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (WebAuthenticator.Authenticate())
            {
                Cef.EnableHighDPISupport();
                var settings = new CefSettings()
                {
                    CachePath = Path.Combine(Application.LocalUserAppDataPath, "Cache"),
                    IgnoreCertificateErrors = true
                };
                if (Cef.Initialize(settings))
                {
                    Application.Run(new FormMain());

                    //Application.Run(new SofaScores());
                    //Application.Run(new WebReader());

                    Cef.Shutdown();
                }
                else
                {
                    throw new Exception("Unable to Initialize Cef!!");
                }
            }
        }
    }
}
