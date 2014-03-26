/*
AWBUpdater
Copyright (C) 2009 Sam Reed, Max Semenik

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
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;

using System.Text.RegularExpressions;
using System.Net;

namespace AwbUpdater
{
    internal sealed partial class Updater : Form
    {
        private readonly string _awBdirectory = "", _tempDirectory = "";
        private string AWBZipName = "", _awbWebAddress = "", _updaterZipName = "", _updaterWebAddress = "";

        private IWebProxy _proxy;

        private bool _updaterUpdate, _awbUpdate, _updateSucessful;

        public Updater()
        {
            InitializeComponent();

            Text += " - " + Application.ProductVersion;

            _awBdirectory = Path.GetDirectoryName(Application.ExecutablePath);
            _tempDirectory = Environment.GetEnvironmentVariable("TEMP") ?? "C:\\Windows\\Temp";
            _tempDirectory = Path.Combine(_tempDirectory, "$AWB$Updater$Temp$");
        }

        /// <summary>
        /// Version of the Updater
        /// </summary>
        private static int AssemblyVersion
        {
            get { return StringToVersion(Application.ProductVersion); }
        }

        private void Updater_Load(object sender, EventArgs e)
        {
            tmrTimer.Enabled = true;
            UpdateUI("Initialising...", true);
        }

        /// <summary>
        /// Main program function
        /// </summary>
        private void UpdateAwb()
        {
            try
            {
                _proxy = WebRequest.GetSystemWebProxy();

                if (_proxy.IsBypassed(new Uri("https://en.wikipedia.org")))
                    _proxy = null;

                UpdateUI("Getting current AWB and Updater versions", true);
                AWBVersion();

                if ((!_updaterUpdate && !_awbUpdate) && string.IsNullOrEmpty(_awbWebAddress))
                {
                    ExitEarly();
                    return;
                }
                UpdateUI("Creating a temporary directory", true);
                CreateTempDir();

                UpdateUI("Downloading AWB", true);
                GetAwbFromInternet();

                UpdateUI("Unzipping AWB to the temporary directory", true);
                UnzipAwb();

                UpdateUI("Making sure AWB is closed", true);
                CloseAwb();

                UpdateUI("Copying AWB files from temp to AWB directory...", true);
                CopyFiles();
                UpdateUI("Update successful", true);

                UpdateUI("Cleaning up from update", true);
                KillTempDir();

                UpdateUI("Update finished. You may close this window (AWB Updater) now.", true);
                _updateSucessful = true;
                ReadyToExit();
            }
            catch (AbortException)
            {
                ReadyToExit();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        #region UI functions
        /// <summary>
        /// Multiple use function to update the GUI items
        /// </summary>
        /// <param name="currentStatus">What the updater is currently doing</param>
        /// <param name="newLine">If true, adds new line to log instead of reusing last existing one</param>
        private void UpdateUI(string currentStatus, bool newLine)
        {
            if (newLine)
            {
                lstLog.Items.Add(currentStatus);
            }
            else
            {
                lstLog.Items[lstLog.Items.Count - 1] = currentStatus;
            }
            lstLog.SelectedIndex = lstLog.Items.Count - 1;
            Application.DoEvents();
        }

        private void AppendLine(string line)
        {
            lstLog.Items[lstLog.Items.Count - 1] += line;
        }

        /// <summary>
        /// Close the updater early due to lack of updates
        /// </summary>
        private void ExitEarly()
        {
            UpdateUI("No update available", true);
            StartAwb();
            ReadyToExit();
        }

        /// <summary>
        /// Sets UI to "ready to exit" state
        /// </summary>
        private void ReadyToExit()
        {
            btnCancel.Text = "Close";
            lblStatus.Text = "";
            progressUpdate.Visible = false;
            btnCancel.Enabled = true;
        }
        #endregion

        /// <summary>
        /// Creates the temporary folder if it doesnt already exist. If it does exist, delete all the contents
        /// </summary>
        private void CreateTempDir()
        {
            if (Directory.Exists(_tempDirectory))
            {
                // clear its content just to be sure that no parasitic files are left
                Directory.Delete(_tempDirectory, true);
            }

            try
            {
                Directory.CreateDirectory(_tempDirectory);
            }
            catch (Exception) // UnauthorizedAccessException and IOEXception
            {
                AppendLine("Unable to create temporary directory: " + _tempDirectory);
                throw new AbortException();
            }
            progressUpdate.Value = 10;
        }

        /// <summary>
        /// Checks and compares the current AWB version with the version listed on the checkpage
        /// </summary>
        private void AWBVersion()
        {
            string text;

            UpdateUI("   Retrieving current version...", true);
            try
            {
                HttpWebRequest rq = (HttpWebRequest)WebRequest.Create("https://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/CheckPage/Version&action=raw");

                rq.Proxy = _proxy;

                rq.UserAgent = string.Format("AWBUpdater/{0} ({1}; .NET CLR {2})",
                                             Assembly.GetExecutingAssembly().GetName().Version,
                                             Environment.OSVersion.VersionString, Environment.Version);

                HttpWebResponse response = (HttpWebResponse)rq.GetResponse();

                Stream stream = response.GetResponseStream();
                StreamReader sr = new StreamReader(stream, Encoding.UTF8);

                text = sr.ReadToEnd();

                sr.Close();
                stream.Close();
                response.Close();
            }
            catch
            {
                AppendLine("FAILED");
                throw new AbortException();
            }

            int awbCurrentVersion =
                StringToVersion(Regex.Match(text, @"<!-- Current version: (.*?) -->").Groups[1].Value);
            int awbNewestVersion =
                StringToVersion(Regex.Match(text, @"<!-- Newest version: (.*?) -->").Groups[1].Value);
            int updaterVersion = StringToVersion(Regex.Match(text, @"<!-- Updater version: (.*?) -->").Groups[1].Value);

            try
            {
                _awbUpdate = _updaterUpdate = false;
                FileVersionInfo awbVersionInfo = FileVersionInfo.GetVersionInfo(Path.Combine(_awBdirectory, "AutoWikiBrowser.exe"));
                int awbFileVersion = StringToVersion(awbVersionInfo.FileVersion);

                if (awbFileVersion < awbCurrentVersion)
                    _awbUpdate = true;
                else if ((awbFileVersion >= awbCurrentVersion) &&
                         (awbFileVersion < awbNewestVersion) &&
                         MessageBox.Show("There is an optional update to AutoWikiBrowser. Would you like to upgrade?", "Optional update", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    _awbUpdate = true;

                if (_awbUpdate)
                {
                    AWBZipName = "AutoWikiBrowser" + awbNewestVersion + ".zip";
                    _awbWebAddress = "http://downloads.sourceforge.net/autowikibrowser/" + AWBZipName;
                }
                else if ((updaterVersion > 1400) &&
                         (updaterVersion > AssemblyVersion))
                {
                    _updaterZipName = "AWBUpdater" + updaterVersion + ".zip";
                    _updaterWebAddress = "http://downloads.sourceforge.net/autowikibrowser/" + _updaterZipName;
                    _updaterUpdate = true;
                }
            }
            catch
            {
                UpdateUI("   Unable to find AutoWikiBrowser.exe to query its version", true);
            }

            progressUpdate.Value = 35;
        }

        /// <summary>
        /// Check the addresses for the files are valid (not null or empty), and downloads the files from the internet
        /// </summary>
        private void GetAwbFromInternet()
        {
            WebClient client = new WebClient { Proxy = _proxy };

            if (!string.IsNullOrEmpty(_awbWebAddress))
                client.DownloadFile(_awbWebAddress, Path.Combine(_tempDirectory, AWBZipName));
            else if (!string.IsNullOrEmpty(_updaterWebAddress))
                client.DownloadFile(_updaterWebAddress, Path.Combine(_tempDirectory, _updaterZipName));

            client.Dispose();

            progressUpdate.Value = 50;
        }

        /// <summary>
        /// Checks the zip files exist and calls the functions to unzip them
        /// </summary>
        private void UnzipAwb()
        {
            string zip = Path.Combine(_tempDirectory, AWBZipName);
            if (!string.IsNullOrEmpty(AWBZipName) && File.Exists(zip))
            {
                Extract(zip);
                DeleteAbsoluteIfExists(zip);
            }

            zip = Path.Combine(_tempDirectory, _updaterZipName);
            if (!string.IsNullOrEmpty(_updaterZipName) && File.Exists(zip))
            {
                Extract(zip);
                DeleteAbsoluteIfExists(zip);
            }

            progressUpdate.Value = 70;
        }

        /// <summary>
        /// Code used to unzip the zip files to the temporary directory
        /// </summary>
        /// <param name="file"></param>
        private void Extract(string file)
        {
            try
            {
                new FastZip().ExtractZip(file, _tempDirectory, null);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
                Application.Exit();
            }
        }

        /// <summary>
        /// Looks if AWB or IRCM are open. If they are, tell the user
        /// </summary>
        private void CloseAwb()
        {
            bool awbOpen = false;

            do
            {
                foreach (Process p in Process.GetProcesses())
                {
                    awbOpen = (p.ProcessName == "AutoWikiBrowser" || p.ProcessName == "IRCMonitor");
                    if (awbOpen)
                    {
                        MessageBox.Show("Please save your settings (if you wish) and close " + p.ProcessName + " completely before pressing OK.");
                        break;
                    }
                }
            }
            while (awbOpen);

            progressUpdate.Value = 75;
        }

        private static void DeleteAbsoluteIfExists(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        private void DeleteIfExists(string name)
        {
            string path = Path.Combine(_awBdirectory, name);
            while (true)
            {
                try
                {
                    DeleteAbsoluteIfExists(path);
                }
                catch (UnauthorizedAccessException) //The exception that is thrown when the operating system denies access because of an I/O error or a specific type of security error.
                {
                    MessageBox.Show(this,
                        "Access denied for deleting files. Program Files and such are not the best place to run AWB from.\r\n" +
                        "Please run the updater with Administrator rights.");
                    Fail();
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show(
                        this,
                        "Problem deleting file:\r\n   " + ex.Message + "\r\n\r\n" +
                        "Please close all applications that may use it and press 'Retry' to try again " +
                        "or 'Cancel' to cancel the upgrade.",
                        "Error",
                        MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    {
                        continue;
                    }

                    Fail();
                }

                break;
            }
        }

        private void Fail()
        {
            AppendLine("... FAILED");
            UpdateUI("Update aborted. AutoWikiBrowser may be unfunctional", true);
            KillTempDir();
            ReadyToExit();
            throw new AbortException();
        }

        /// <summary>
        /// Copies files from the temporary to the working directory
        /// </summary>
        private void CopyFiles()
        {
            string updater = Path.Combine(_tempDirectory, "AWBUpdater.exe");
            if (_updaterUpdate || File.Exists(updater))
                CopyFile(updater, Path.Combine(_awBdirectory, "AWBUpdater.exe.new"));

            if (_awbUpdate)
            {
                //Explicit Deletions (Remove these if they exist!!)
                DeleteIfExists("Wikidiff2.dll");

                DeleteIfExists("Diff.dll");

                DeleteIfExists("WikiFunctions2.dll");

                DeleteIfExists("WPAssessmentsCatCreator.dll");

                if (Directory.Exists(Path.Combine(_awBdirectory, "Plugins\\WPAssessmentsCatCreator")))
                    Directory.Delete(Path.Combine(_awBdirectory, "Plugins\\WPAssessmentsCatCreator"), true);

                foreach (string file in Directory.GetFiles(_tempDirectory, "*.*", SearchOption.AllDirectories))
                {
                    if (file.Contains("AWBUpdater"))
                        continue;

                    CopyFile(file,
                             Path.Combine(_awBdirectory, file.Replace(_tempDirectory + "\\", "")));
                }

                string[] pluginFiles = Directory.GetFiles(Path.Combine(_awBdirectory, "Plugins"), "*.*", SearchOption.AllDirectories);

                foreach (string file in Directory.GetFiles(_awBdirectory, "*.*", SearchOption.TopDirectoryOnly))
                {
                    foreach (string pluginFile in pluginFiles)
                    {
                        if (file.Substring(file.LastIndexOf("\\", StringComparison.CurrentCulture)) ==
                            pluginFile.Substring(pluginFile.LastIndexOf("\\", StringComparison.CurrentCulture)))
                        {
                            File.Copy(pluginFile, file, true);
                            break;
                        }
                    }
                }
            }
            progressUpdate.Value = 95;
        }

        private void CopyFile(string source, string destination)
        {
            CreatePath(destination);
            UpdateUI("     " + destination, true);

            // loop until the file is successfully copied, or user is tired of retrying
            while (true)
            {
                try
                {
                    File.Copy(source, destination, true);
                }
                catch (UnauthorizedAccessException) //The exception that is thrown when the operating system denies access because of an I/O error or a specific type of security error.
                {
                    MessageBox.Show(this,
                        "Access denied for copying files. Program Files and such are not the best place to run AWB from.\r\n" +
                        "Please run the updater with Administrator rights.");
                    Fail();
                }
                catch (Exception ex)
                {
                    if (MessageBox.Show(
                            this,
                            "Problem replacing file:\r\n   " + ex.Message + "\r\n\r\n" +
                            "Please close all applications that may use it and press 'Retry' to try again " +
                            "or 'Cancel' to cancel the upgrade.",
                            "Error",
                            MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                    {
                        continue;
                    }

                    Fail();
                }

                break;
            }
        }

        /// <summary>
        /// Creates all subdirectories in the path, if needed
        /// </summary>
        /// <param name="path">Path to process, assumed to start from </param>
        private void CreatePath(string path)
        {
            path = Path.GetDirectoryName(path); // strip filename
            if (path != null && !Directory.Exists(path))
            {
                UpdateUI("   Creating directory " + path + "...", true);
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    AppendLine(" FAILED");
                    UpdateUI("     (" + ex.Message + ")", true);
                }
            }
        }

        /// <summary>
        /// Starts AWB if exists and is not already running
        /// </summary>
        private void StartAwb()
        {
            bool awbOpen = false;
            foreach (Process p in Process.GetProcesses())
            {
                awbOpen = (p.ProcessName == "AutoWikiBrowser");
                if (awbOpen)
                    break;
            }

            if (!awbOpen && File.Exists(_awBdirectory + "AutoWikiBrowser.exe")
                && MessageBox.Show("Would you like to start AWB?", "Start AWB?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Process.Start(_awBdirectory + "AutoWikiBrowser.exe");
            }

            progressUpdate.Value = 99;
        }

        /// <summary>
        /// Deletes the temporary directory
        /// </summary>
        private void KillTempDir()
        {
            if (Directory.Exists(_tempDirectory))
                Directory.Delete(_tempDirectory, true);
            progressUpdate.Value = 100;
        }

        private void tmrTimer_Tick(object sender, EventArgs e)
        {
            tmrTimer.Enabled = false;
            UpdateAwb();
        }

        static int StringToVersion(string version)
        {
            int res;
            if (!int.TryParse(version.Replace(".", ""), out res))
                res = 0;

            return res;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (_updateSucessful) StartAwb();

            Close();
        }
    }

    /// <summary>
    /// This exception stops processing and prepared the updater for exit
    /// </summary>
    [Serializable]
    public class AbortException : Exception
    {
        public AbortException()
        { }

        public AbortException(string message)
            : base(message)
        { }

        public AbortException(string message, Exception innerException) :
            base(message, innerException)
        { }

        protected AbortException(System.Runtime.Serialization.SerializationInfo info,
           System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }
    }
}
