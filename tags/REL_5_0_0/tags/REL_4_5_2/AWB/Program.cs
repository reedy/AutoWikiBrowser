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
                Application.ThreadException += Application_ThreadException;

                if (WikiFunctions.Variables.UsingMono)
                {
                    MessageBox.Show("AWB is not currently supported by mono", "Not supported",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                MainForm awb = new MainForm();
                AWB = awb;
                awb.ParseCommandLine(args);

                Application.Run(awb);
            }
            catch (Exception ex)
            {
                WikiFunctions.ErrorHandler.Handle(ex);
            }
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            WikiFunctions.ErrorHandler.Handle(e.Exception);
        }

        internal static Version Version { get { return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version; } }
        internal static string VersionString { get { return Version.ToString(); } }
        internal const string NAME = "AutoWikiBrowser";
        internal static string UserAgentString { get { return NAME + "/" + VersionString; } }
        internal static WikiFunctions.Plugin.IAutoWikiBrowser AWB;
        internal static Logging.MyTrace MyTrace = new Logging.MyTrace();
    }
}