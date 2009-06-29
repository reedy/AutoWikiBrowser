/*
Copyright (C) 2007 Sam Reed
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

using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;

using System.Windows.Forms;
using WikiFunctions.Background;

namespace WikiFunctions
{
    public static class Updater
    {
        private static void UpdateFunc()
        {
            try
            {
                string AWBDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
                if (File.Exists(AWBDirectory + "AWBUpdater.exe.new"))
                {
                    File.Copy(AWBDirectory + "AWBUpdater.exe.new", AWBDirectory + "AWBUpdater.exe", true);
                    File.Delete(AWBDirectory + "AWBUpdater.exe.new");
                }
                else
                {
                    bool update = false;

                    string text = Tools.GetHTML("http://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/CheckPage/Version&action=raw");

                    int awbCurrentVersion =
    StringToVersion(Regex.Match(text, @"<!-- Current version: (.*?) -->").Groups[1].Value);
                    int awbNewestVersion =
                        StringToVersion(Regex.Match(text, @"<!-- Newest version: (.*?) -->").Groups[1].Value);
                    int updaterVersion = StringToVersion(Regex.Match(text, @"<!-- Updater version: (.*?) -->").Groups[1].Value);

                    if ((awbCurrentVersion > 4000) || (awbNewestVersion > 4000))
                    {
                        FileVersionInfo awbVersionInfo = FileVersionInfo.GetVersionInfo(AWBDirectory + "AutoWikiBrowser.exe");
                        int awbFileVersion = StringToVersion(awbVersionInfo.FileVersion);

                        if (awbFileVersion < awbCurrentVersion)
                            update = true;
                        else if ((awbFileVersion >= awbCurrentVersion) && (awbFileVersion < awbNewestVersion) &&
                            MessageBox.Show("There is an optional update to AutoWikiBrowser. Would you like to upgrade?", "Optional update", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            update = true;

                        if (!update && (updaterVersion > 1400) &&
                    (updaterVersion > StringToVersion(FileVersionInfo.GetVersionInfo(AWBDirectory + "AWBUpdater.exe").FileVersion)))
                        {
                            MessageBox.Show("There is an Update to the AWB updater. Updating Now", "Updater update", MessageBoxButtons.YesNo);
                            update = true;
                        }

                        if (update)
                            Process.Start(AWBDirectory + "AWBUpdater.exe");
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private static int StringToVersion(string version)
        {
            int res;
            if (!int.TryParse(version.Replace(".", ""), out res))
                res = 0;

            return res;
        }

        private static BackgroundRequest Request;

        /// <summary>
        /// Checks to see if AWBUpdater.exe.new exists, if it does, replace it.
        /// If not, see if the version of AWB Updater is older than the version on the checkpage, and run AWBUpdater if so
        /// </summary>
        public static void UpdateAWB(Tools.SetProgress setProgress)
        {
            setProgress(22);
            Update();
            setProgress(29);
        }
        /// <summary>
        /// Checks to see if AWBUpdater.exe.new exists, if it does, replace it.
        /// If not, see if the version of AWB Updater is older than the version on the checkpage, and run AWBUpdater if so
        /// </summary>
        public static void Update()
        {
            Request = new BackgroundRequest();
            Request.Execute(UpdateFunc);
        }

        /// <summary>
        /// Waits for background AWBUpdater.exe update to complete
        /// </summary>
        public static void WaitForCompletion()
        {
            if (Request == null) return;
            Request.Wait();
            Request = null;
        }
    }
}
