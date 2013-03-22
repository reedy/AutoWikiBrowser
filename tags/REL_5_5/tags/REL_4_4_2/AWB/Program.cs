/*
Autowikibrowser
Copyright (C) 2007 Martin Richards
(C) 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation; either version 2 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

using System;
using System.Windows.Forms;

namespace AutoWikiBrowser
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {		
                System.Threading.Thread.CurrentThread.Name = "Main thread";
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

                if (WikiFunctions.Variables.DetectMono())
                    throw new NotSupportedException("AWB is not currently supported by mono");

                MainForm awb = new MainForm();

                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "/s":
                            try
                            {
                                string tmp = args[i + 1];
                                if (tmp.Contains(".xml") && System.IO.File.Exists(tmp))
                                    awb.SettingsFile = tmp;
                            }
                            catch { }
                            break;
                        case "/u":
                            try { awb.ProfileToLoad = int.Parse(args[i + 1]); }
                            catch { }
                            break;
                    }
                }

                Program.AWB = awb;
                Application.Run(awb);
            }
            catch (Exception ex)
            {
                WikiFunctions.ErrorHandler.Handle(ex);
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            WikiFunctions.ErrorHandler.Handle(e.Exception);
        }

        internal static Version Version { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; } }
        internal static string VersionString { get { return Version.ToString(); } }
        internal const string NAME = "AutoWikiBrowser";
        static internal string UserAgentString { get { return NAME + "/" + VersionString; } }
        static internal WikiFunctions.Plugin.IAutoWikiBrowser AWB;
        static internal Logging.MyTrace MyTrace = new AutoWikiBrowser.Logging.MyTrace();
    }
}