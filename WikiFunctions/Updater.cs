/*
Copyright (C) 2009-2018 Sam Reed
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
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WikiFunctions.Background;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WikiFunctions
{
    public static class Updater
    {
        private static readonly string AWBDirectory;

        /// <summary>
        /// Runs Update() at creation time
        /// </summary>
        static Updater()
        {
            AWBDirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            Result = AWBEnabledStatus.None;
        }

        /// <summary>
        /// Available Enabled statuses for AWB
        /// </summary>
        [Flags]
        public enum AWBEnabledStatus
        {
            None = 0,
            Error = 1,
            Disabled = 2,
            Enabled = 4,
            UpdaterUpdate = 8,
            OptionalUpdate = 12
        }

        /// <summary>
        /// Last AWBEnabledStatus Result from Checkpage Check
        /// </summary>
        public static AWBEnabledStatus Result { get; private set; }

        /// <summary>
        /// Text (JSON) of the Current AWB Global Checkpage (en.wp)
        /// </summary>
        public static string GlobalVersionPage { get; private set; }

        /// <summary>
        /// Do the actual checking for enabledness etc
        /// </summary>
        private static void UpdateFunc()
        {
            try
            {
                string text =
                    Tools.GetHTML(
                        "https://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/CheckPage/VersionJSON&action=raw");
                GlobalVersionPage = text;

                var json = JObject.Parse(text);

                Result = AWBEnabledStatus.Disabled; // Disabled till proven enabled

                var definition = new { version = "", dotnetversion = "", svn = false };
                var enabledVersions = from v in json["enabledversions"] select JsonConvert.DeserializeAnonymousType(v.ToString(), definition);

                string updaterVersion = json["updaterversion"].ToString();

                FileVersionInfo awbVersionInfo =
                    FileVersionInfo.GetVersionInfo(AWBDirectory + "AutoWikiBrowser.exe");

                if (enabledVersions.Any(v => v.version == awbVersionInfo.FileVersion))
                {
                    Result = AWBEnabledStatus.Enabled;
                }

                string updaterFileVersion = FileVersionInfo.GetVersionInfo(AWBDirectory + "AWBUpdater.exe").FileVersion;

                if (Version.Parse(updaterFileVersion) < Version.Parse(updaterVersion))
                {
                    Result |= AWBEnabledStatus.UpdaterUpdate;
                }

                if ((Result & AWBEnabledStatus.Disabled) == AWBEnabledStatus.Disabled)
                {
                    // If it's disabled, updates aren't optional!
                    return;
                }

                var awbVersionParsed = Version.Parse(awbVersionInfo.FileVersion);

                // SVN versions aren't optional updates
                if (enabledVersions.Any(v => (Version.Parse(v.version) > awbVersionParsed && !v.svn)))
                {
                    Result |= AWBEnabledStatus.OptionalUpdate;
                }
            }
            catch
            {
                Result = AWBEnabledStatus.Error;
            }
        }

        private static BackgroundRequest _request;

        /// <summary>
        /// Checks to see if AWBUpdater.exe.new exists, if it does, replace it.
        /// </summary>
        public static void UpdateUpdaterFile()
        {
            if (File.Exists(AWBDirectory + "AWBUpdater.exe.new"))
            {
                File.Copy(AWBDirectory + "AWBUpdater.exe.new", AWBDirectory + "AWBUpdater.exe", true);
                File.Delete(AWBDirectory + "AWBUpdater.exe.new");
            }
        }

        /// <summary>
        /// Background request to check enabled state of AWB
        /// </summary>
        public static void CheckForUpdates()
        {
            if (_request != null)
            {
                return;
            }

            _request = new BackgroundRequest();
            _request.Execute(UpdateFunc);
        }

        /// <summary>
        /// Waits for background enabled check to complete
        /// </summary>
        public static void WaitForCompletion()
        {
            if (_request == null)
            {
                return;
            }
            _request.Wait();
            _request = null;
        }

        /// <summary>
        /// Runs the Updater program
        /// </summary>
        public static void RunUpdater()
        {
            Process.Start(AWBDirectory + "AWBUpdater.exe");
        }
    }
}
