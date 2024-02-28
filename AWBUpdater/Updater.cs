﻿/*
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
using System.Linq;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;

using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace AWBUpdater
{
    internal sealed partial class Updater : Form
    {
        private readonly string _awbDirectory = "", _tempDirectory = "";
        private string _zipName = "";

        private IWebProxy _proxy;

        private UpdateStatus _updateStatus = UpdateStatus.None;

        private const string CHECKPAGE_URL =
            "https://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/CheckPage/VersionJSON&action=raw";

        private const string SOURCEFORGE_URL =
            "http://downloads.sourceforge.net/project/autowikibrowser/autowikibrowser";

        [Flags]
        public enum UpdateStatus
        {
            None = 0,
            Error = 1,
            RequiredUpdate = 2,
            OptionalUpdate = 4,
            OptionalUpdateDeclined = 8,
            UpdaterUpdate = 16,
            UpdateSuccessful = 32,
        }

        public Updater()
        {
            InitializeComponent();

            Text += " - " + Application.ProductVersion;

            _awbDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            _tempDirectory = Environment.GetEnvironmentVariable("TEMP") ?? "C:\\Windows\\Temp";
            _tempDirectory = Path.Combine(_tempDirectory, "$AWB$Updater$Temp$");
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

                if ((_updateStatus & (UpdateStatus.OptionalUpdate | UpdateStatus.RequiredUpdate |
                                      UpdateStatus.UpdaterUpdate)) == 0)
                {
                    ExitEarly();
                    return;
                }

                UpdateUI("Creating a temporary directory", true);
                CreateTempDir();

                UpdateUI("Downloading", true);
                GetZipFromInternet();

                UpdateUI("Unzipping to the temp directory", true);
                UnzipFile();

                if ((_updateStatus & (UpdateStatus.RequiredUpdate | UpdateStatus.OptionalUpdate)) != 0)
                {
                    UpdateUI("Making sure AWB is closed", true);
                    CloseAwb();
                }

                UpdateUI("Copying files from temp directory to the AWB directory...", true);
                CopyFiles();
                UpdateUI("Update successful", true);

                UpdateUI("Cleaning up from update", true);
                KillTempDir();

                UpdateUI("Update finished. You may close this window (AWB Updater) now.", true);
                _updateStatus = UpdateStatus.UpdateSuccessful;

                ReadyToExit();
            }
            catch (AbortException)
            {
                ReadyToExit();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
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
            switch (_updateStatus)
            {
                case UpdateStatus.None:
                    UpdateUI("No update available", true);
                    break;

                case UpdateStatus.OptionalUpdateDeclined:
                    UpdateUI("Optional update declined", true);
                    break;
            }

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
        /// Creates the temporary folder if it doesn't already exist. If it does exist, delete all the contents
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
            catch (Exception)
            {
                // UnauthorizedAccessException and IOException
                UpdateUI("Unable to create temporary directory: " + _tempDirectory, true);
                throw new AbortException();
            }

            progressUpdate.Value = 10;
        }

        /// <summary>
        /// Checks and compares the current AWB version with the version listed on the checkpage
        /// </summary>
        private void AWBVersion()
        {
            string json;

            UpdateUI("   Retrieving current version...", true);
            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                HttpWebRequest rq = (HttpWebRequest) WebRequest.Create(CHECKPAGE_URL);

                rq.Proxy = _proxy;

                rq.UserAgent = string.Format("AWBUpdater/{0} ({1}; .NET CLR {2})",
                    Assembly.GetExecutingAssembly().GetName().Version,
                    Environment.OSVersion.VersionString, Environment.Version);

                HttpWebResponse response = (HttpWebResponse) rq.GetResponse();

                using (Stream stream = response.GetResponseStream())
                using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                {
                    json = sr.ReadToEnd();

                    sr.Close();
                    stream.Close();
                    response.Close();
                }
            }
            catch
            {
                AppendLine("FAILED");
                throw new AbortException();
            }

            try
            {
                FileVersionInfo awbVersionInfo =
                    FileVersionInfo.GetVersionInfo(Path.Combine(_awbDirectory, "AutoWikiBrowser.exe"));

                JavaScriptSerializer jss = new JavaScriptSerializer();
                var updaterData = jss.Deserialize<RootObject>(json);

                string versionToUpdateAWBTo = "";

                if (updaterData.enabledversions.All(v => v.version != awbVersionInfo.FileVersion))
                {
                    // The version of AWB in the directory definitely isn't enabled
                    _updateStatus = UpdateStatus.RequiredUpdate;

                    versionToUpdateAWBTo = updaterData.enabledversions.Where(x => !x.dev)
                        .OrderByDescending(x => x.version).First().version;
                }
                else
                {
                    var newerVersions = updaterData.enabledversions
                        .Where(x => !x.dev && new Version(x.version) > new Version(awbVersionInfo.FileVersion))
                        .OrderByDescending(x => x.version).ToList();

                    if (newerVersions.Any())
                    {
                        _updateStatus = UpdateStatus.OptionalUpdateDeclined;

                        if (newerVersions.Count > 1)
                        {
                            using (VersionChooser chooser = new VersionChooser(newerVersions))
                            {
                                if (chooser.ShowDialog() == DialogResult.OK &&
                                    !string.IsNullOrEmpty(chooser.SelectedVersion))
                                {
                                    _updateStatus = UpdateStatus.OptionalUpdate;
                                    versionToUpdateAWBTo = chooser.SelectedVersion;
                                }
                            }
                        }
                        else if (newerVersions.Count == 1 &&
                                 MessageBox.Show(
                                     string.Format(
                                         "There is an optional update to AutoWikiBrowser. Would you like to upgrade to {0}?",
                                         newerVersions.First().version),
                                     "Optional update", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            _updateStatus = UpdateStatus.OptionalUpdate;
                            versionToUpdateAWBTo = newerVersions.First().version;
                        }
                    }
                }

                if ((_updateStatus & (UpdateStatus.RequiredUpdate | UpdateStatus.OptionalUpdate)) != 0)
                {
                    _zipName = "AutoWikiBrowser" + VersionToFileVersion(versionToUpdateAWBTo) + ".zip";
                }
                else if (new Version(updaterData.updaterversion) >
                         new Version(Assembly.GetExecutingAssembly().GetName().Version.ToString()))
                {
                    _zipName = "AWBUpdater" + VersionToFileVersion(updaterData.updaterversion) + ".zip";
                    _updateStatus = UpdateStatus.UpdaterUpdate;
                }
            }
            catch
            {
                _updateStatus = UpdateStatus.Error;
                UpdateUI("   Unable to find AutoWikiBrowser.exe to query its version", true);
            }

            progressUpdate.Value = 35;
        }

        /// <summary>
        /// Check the addresses for the files are valid (not null or empty), and downloads the files from the internet
        /// </summary>
        private void GetZipFromInternet()
        {
            WebClient client = new WebClient {Proxy = _proxy};

            if (!string.IsNullOrEmpty(_zipName))
            {
                actuallyDownloadZip(client, _zipName, Path.Combine(_tempDirectory, _zipName));
            }

            client.Dispose();

            progressUpdate.Value = 50;
        }

        private void actuallyDownloadZip(WebClient client, string file, string target)
        {
            var fileWithoutExt = file.Replace(".zip", "");
            var fileUrl = SOURCEFORGE_URL + $"/{fileWithoutExt}/{file}";

            try
            {
                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                var unixTime = (int) t.TotalSeconds;
                var url = string.Format(
                    "{0}?r={1}&ts={2}",
                    fileUrl,
                    HttpUtility.UrlEncode(SOURCEFORGE_URL + $"/{fileWithoutExt}/"),
                    unixTime
                );
                client.DownloadFile(url, target);
            }
            catch (WebException webEx)
            {
                UpdateUI(string.Format("Download of `{0}` failed: {1}", fileUrl, webEx.Message), true);
            }
        }

        /// <summary>
        /// Checks the zip files exist and calls the functions to unzip them
        /// </summary>
        private void UnzipFile()
        {
            string zip = Path.Combine(_tempDirectory, _zipName);
            if (!string.IsNullOrEmpty(zip) && File.Exists(zip))
            {
                Extract(zip);
                DeleteAbsoluteIfExists(zip);
            }
            else
            {
                UpdateUI("File not unzipped...", true);
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
            catch (ZipException)
            {
                UpdateUI(Path.GetFileName(file) + " seems to be corrupt. Deleting the zip.", true);
                UpdateUI("Please confirm that sourceforge is up.", true);
                DeleteAbsoluteIfExists(file);
                throw new AbortException();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
                Application.Exit();
            }
        }

        /// <summary>
        /// Looks if AWB is open. If it is, tell the user
        /// </summary>
        private void CloseAwb()
        {
            bool awbOpen = false;

            do
            {
                foreach (Process p in Process.GetProcesses())
                {
                    awbOpen = p.ProcessName == "AutoWikiBrowser";
                    if (awbOpen)
                    {
                        MessageBox.Show("Please save your settings (if you wish) and close " + p.ProcessName +
                                        " completely before pressing OK.");
                        break;
                    }
                }
            } while (awbOpen);

            progressUpdate.Value = 75;
        }

        private static void DeleteAbsoluteIfExists(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }

        private void DeleteIfExists(string name)
        {
            string path = Path.Combine(_awbDirectory, name);
            while (true)
            {
                try
                {
                    DeleteAbsoluteIfExists(path);
                }
                catch
                    (UnauthorizedAccessException)
                {
                    // The exception that is thrown when the operating system denies access because of an I/O error or a specific type of security error.
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
            if ((_updateStatus & UpdateStatus.UpdaterUpdate) == UpdateStatus.UpdaterUpdate || File.Exists(updater))
            {
                CopyFile(updater, Path.Combine(_awbDirectory, "AWBUpdater.exe.new"));
            }

            if ((_updateStatus & (UpdateStatus.OptionalUpdate | UpdateStatus.RequiredUpdate)) != 0)
            {
                // Explicit Deletions (Remove these if they exist!!)
                DeleteIfExists("Wikidiff2.dll");

                DeleteIfExists("Diff.dll");

                DeleteIfExists("WikiFunctions2.dll");

                DeleteIfExists("WPAssessmentsCatCreator.dll");

                if (Directory.Exists(Path.Combine(_awbDirectory, "Plugins\\WPAssessmentsCatCreator")))
                {
                    Directory.Delete(Path.Combine(_awbDirectory, "Plugins\\WPAssessmentsCatCreator"), true);
                }

                foreach (string file in Directory.GetFiles(_tempDirectory, "*.*", SearchOption.AllDirectories))
                {
                    if (file.Contains("AWBUpdater"))
                    {
                        continue;
                    }

                    CopyFile(file,
                        Path.Combine(_awbDirectory, file.Replace(_tempDirectory + "\\", "")));
                }

                string[] pluginFiles = Directory.GetFiles(Path.Combine(_awbDirectory, "Plugins"), "*.*",
                    SearchOption.AllDirectories);

                foreach (string file in Directory.GetFiles(_awbDirectory, "*.*", SearchOption.TopDirectoryOnly))
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
                catch (UnauthorizedAccessException)
                {
                    //The exception that is thrown when the operating system denies access because of an I/O error or a specific type of security error.
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
                    AppendLine("... FAILED");
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
                {
                    break;
                }
            }

            if (!awbOpen && File.Exists(_awbDirectory + "AutoWikiBrowser.exe")
                         && MessageBox.Show("Would you like to start AWB?", "Start AWB?", MessageBoxButtons.YesNo) ==
                         DialogResult.Yes)
            {
                Process.Start(_awbDirectory + "AutoWikiBrowser.exe");
            }

            progressUpdate.Value = 99;
        }

        /// <summary>
        /// Deletes the temporary directory
        /// </summary>
        private void KillTempDir()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }

            progressUpdate.Value = 100;
        }

        private void tmrTimer_Tick(object sender, EventArgs e)
        {
            tmrTimer.Enabled = false;
            UpdateAwb();
        }

        private static string VersionToFileVersion(string version)
        {
            return version.Replace(".", "");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if ((_updateStatus & (UpdateStatus.OptionalUpdate | UpdateStatus.RequiredUpdate)) != 0)
            {
                StartAwb();
            }

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
        {
        }

        public AbortException(string message)
            : base(message)
        {
        }

        public AbortException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        protected AbortException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
