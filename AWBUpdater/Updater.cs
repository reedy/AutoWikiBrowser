/*
AWBUpdater
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System.Text.RegularExpressions;
using System.Net;

namespace AwbUpdater
{
    internal sealed partial class Updater : Form
    {
        string AWBdirectory = "";
        string tempDirectory = "";

        string AWBZipName = "";
        string AWBWebAddress = "";

        string UpdaterZipName = "";
        string UpdaterWebAddress = "";

        bool noUpdates = false;
        bool updaterUpdate = false;
        bool awbUpdate = false;

        public Updater()
        {
            InitializeComponent();

            AWBdirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            tempDirectory = AWBdirectory + "temp\\";
        }

        public static string AssemblyVersion
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        private void Updater_Load(object sender, EventArgs e)
        {
            tmrTimer.Enabled = true;
        }

        private void UpdateAwb()
        {
            try
            {
                lblCurrentTask.Text = "Getting Current AWB and Updater Versions";
                Application.DoEvents();
                AWBversion();

                if (noUpdates && !NotNullOrEmpty(AWBWebAddress))
                    ExitEarly();
                else
                {
                    Application.DoEvents();

                    lblCurrentTask.Text = "Creating a Temporary Directory";
                    Application.DoEvents();
                    CreateTempDir();

                    lblCurrentTask.Text = "Downloading AWB";
                    Application.DoEvents();
                    GetAwbFromInternet();

                    lblCurrentTask.Text = "Unzipping AWB to the Temporary Directory";
                    Application.DoEvents();
                    UnzipAwb();

                    lblCurrentTask.Text = "Making Sure AWB is Closed";
                    Application.DoEvents();
                    CloseAwb();

                    lblCurrentTask.Text = "Copying AWB Files from temp to AWB Directory";
                    Application.DoEvents();
                    CopyFiles();
                    MessageBox.Show("AWB Update Successful", "Update Sucessful");

                    lblCurrentTask.Text = "Starting AWB";
                    Application.DoEvents();
                    StartAwb();

                    lblCurrentTask.Text = "Cleaning up from Update";
                    Application.DoEvents();
                    KillTempDir();

                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        private bool NotNullOrEmpty(string check)
        {
            return (check != "" && check != null);
        }

        private void ExitEarly()
        {
            MessageBox.Show("Nothing to Update. The Updater will now close");
            StartAwb();
            Application.Exit();
        }

        public void AWBversion()
        {
            string text = "";

            try
            {
                HttpWebRequest rq = (HttpWebRequest)WebRequest.Create("http://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/CheckPage/Version&action=edit");

                rq.Proxy.Credentials = CredentialCache.DefaultCredentials;
                rq.UserAgent = "AWBUpdater " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

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
                MessageBox.Show("Error fetching current version number.");
                Application.Exit();
            }

            Match m_awbversion = Regex.Match(text, @"&lt;!-- Current version: (.*?) --&gt;");
            Match m_awbnewest = Regex.Match(text, @"&lt;!-- Newest version: (.*?) --&gt;");
            Match m_updversion = Regex.Match(text, @"&lt;!-- Updater version: (.*?) --&gt;");

            if ((m_awbversion.Success && m_awbversion.Groups[1].Value.Length == 4) || (m_awbnewest.Success && m_awbnewest.Groups[1].Value.Length == 4))
            {
                try
                {
                    awbUpdate = updaterUpdate = false;
                    FileVersionInfo versionAWB = FileVersionInfo.GetVersionInfo(AWBdirectory + "AutoWikiBrowser.exe");

                    if ((Convert.ToInt32(versionAWB.FileVersion.Replace(".", ""))) < (Convert.ToInt32(m_awbversion.Groups[1].Value)))
                        awbUpdate = true;
                    else if ((Convert.ToInt32(versionAWB.FileVersion.Replace(".", "")) >= (Convert.ToInt32(m_awbversion.Groups[1].Value))) &&
                        ((Convert.ToInt32(versionAWB.FileVersion.Replace(".", "")) < (Convert.ToInt32(m_awbnewest.Groups[1].Value)))) &&
                        MessageBox.Show("There is an Optional Update to AutoWikiBrowser. Would you like to Upgrade?", "Optional Update", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        awbUpdate = true;

                    if (awbUpdate)
                    {
                        AWBZipName = "AutoWikiBrowser" + m_awbnewest.Groups[1].Value.Replace(".", "") + ".zip";
                        AWBWebAddress = "http://downloads.sourceforge.net/autowikibrowser/" + AWBZipName;
                    }

                    if (m_updversion.Success && m_updversion.Groups[1].Value.Length == 4 &&
                        Convert.ToInt32(m_updversion.Groups[1].Value) > Convert.ToInt32(AssemblyVersion.Replace(".", "")))
                    {
                        UpdaterZipName = "AWBUpdater" + m_updversion.Groups[1].Value.Replace(".", "") + ".zip";
                        UpdaterWebAddress = "http://downloads.sourceforge.net/autowikibrowser/" + UpdaterZipName;
                        updaterUpdate = true;
                    }
                }
                catch
                { MessageBox.Show("Unable to find AutoWikiBrowser.exe to query Version No."); }

                if (!updaterUpdate && !awbUpdate)                   
                    noUpdates = true;

                progressUpdate.Value = 30;
            }
        }

        private void CreateTempDir()
        {
            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);

            progressUpdate.Value = 35;
        }

        private void GetAwbFromInternet()
        {
            System.Net.WebClient client = new System.Net.WebClient();

            if (!NotNullOrEmpty(UpdaterWebAddress))
                client.DownloadFile(AWBWebAddress, tempDirectory + AWBZipName);

            if (!NotNullOrEmpty(UpdaterWebAddress))
                client.DownloadFile(UpdaterWebAddress, tempDirectory + UpdaterZipName);

            client.Dispose();

            progressUpdate.Value = 50;
        }

        private void UnzipAwb()
        {
            bool badUpdate = false;

            if (AWBZipName != "" && File.Exists(tempDirectory + AWBZipName))
            {
                using (ZipFile zf = new ZipFile(tempDirectory + AWBZipName))
                {
                    foreach (ZipEntry entry in zf)
                    {
                        if (entry.IsFile)
                            ExtractFile(zf.GetInputStream(entry), entry, tempDirectory);
                    }
                }
            }
            else
                badUpdate = true;

            if (UpdaterZipName != "" && File.Exists(tempDirectory + UpdaterZipName))
            {
                using (ZipFile zf = new ZipFile(tempDirectory + UpdaterZipName))
                {
                    foreach (ZipEntry entry in zf)
                    {
                        if (entry.IsFile)
                            ExtractFile(zf.GetInputStream(entry), entry, tempDirectory);
                    }
                }
            }
            else
                badUpdate = true;

            if (badUpdate)
            {
                MessageBox.Show(@"Something has gone wrong with the downloading of the files. Please restart the updater to try again.

AWBUpdater will now close!");
                Application.Exit();
            }
            
            progressUpdate.Value = 70;
        }

        private void ExtractFile(Stream inputStream, ZipEntry theEntry, string targetDir)
        {
            // try and sort out the correct place to save this entry
            string entryFileName;

            if (Path.IsPathRooted(theEntry.Name))
            {
                string workName = Path.GetPathRoot(theEntry.Name);
                workName = theEntry.Name.Substring(workName.Length);
                entryFileName = Path.Combine(Path.GetDirectoryName(workName), Path.GetFileName(theEntry.Name));
            }
            else
                entryFileName = theEntry.Name;

            string targetName = Path.Combine(targetDir, entryFileName);
            string fullPath = Path.GetDirectoryName(Path.GetFullPath(targetName));

            // Could be an option or parameter to allow failure or try creation
            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);


            if (entryFileName.Length > 0)
            {
                using (FileStream outputStream = File.Create(targetName))
                {
                    StreamUtils.Copy(inputStream, outputStream, new byte[4096]);
                }
            }
        }

        private void CloseAwb()
        {
            bool awbOpen = new bool();

            do
            {
                awbOpen = false;

                foreach (Process p in Process.GetProcesses())
                {
                    if (p.ProcessName == "AutoWikiBrowser" || p.ProcessName == "IRCMonitor")
                    {
                        awbOpen = true;
                        MessageBox.Show("Please save your settings (if you wish) and close " + p.ProcessName +" completely before pressing OK.");
                    }
                }
            }
            while (awbOpen == true);

            progressUpdate.Value = 75;
        }

        private void CopyFiles()
        {
            if (updaterUpdate && File.Exists(tempDirectory + "AWBUpdater.exe"))
                File.Copy(tempDirectory + "AWBUpdater.exe", AWBdirectory + "AWBUpdater.exe.new", true);

            if (awbUpdate)
            {
                File.Copy(tempDirectory + "AutoWikiBrowser.exe", AWBdirectory + "AutoWikiBrowser.exe", true);
                File.Copy(tempDirectory + "WikiFunctions.dll", AWBdirectory + "WikiFunctions.dll", true);
                File.Copy(tempDirectory + "IRCMonitor.exe", AWBdirectory + "IRCMonitor.exe", true);
                File.Copy(tempDirectory + "Wikidiff2.dll", AWBdirectory + "Wikidiff2.dll", true);
                File.Copy(tempDirectory + "AutoWikiBrowser.exe.config", AWBdirectory + "AutoWikiBrowser.exe.config", true);

                File.Copy(tempDirectory + "gpl-2.0.txt", AWBdirectory + "gpl-2.0.txt", true);
                File.Copy(tempDirectory + "gpl-3.0.txt", AWBdirectory + "gpl-3.0.txt", true);

                if (File.Exists(AWBdirectory + "CFD.dll"))
                    File.Copy(tempDirectory + "Plugins\\CFD\\CFD.dll", AWBdirectory + "CFD.dll", true);

                if (!Directory.Exists(AWBdirectory + "\\Plugins\\CFD"))
                    Directory.CreateDirectory(AWBdirectory + "\\Plugins\\CFD");

                File.Copy(tempDirectory + "Plugins\\CFD\\CFD.dll", AWBdirectory + "Plugins\\CFD\\CFD.dll", true);

                if (File.Exists(AWBdirectory + "Kingbotk AWB Plugin.dll"))
                    File.Copy(tempDirectory + "Plugins\\Kingbotk\\Kingbotk AWB Plugin.dll", AWBdirectory + "Kingbotk AWB Plugin.dll", true);

                if (!Directory.Exists(AWBdirectory + "Plugins\\Kingbotk"))
                    Directory.CreateDirectory(AWBdirectory + "Plugins\\Kingbotk");

                File.Copy(tempDirectory + "Plugins\\Kingbotk\\Kingbotk AWB Plugin.dll", AWBdirectory + "Plugins\\Kingbotk\\Kingbotk AWB Plugin.dll", true);

                if (File.Exists(AWBdirectory + "WikiFunctions2.dll"))
                    File.Delete(AWBdirectory + "WikiFunctions2.dll");

                if (File.Exists(AWBdirectory + "WPAssessmentsCatCreator.dll"))
                    File.Copy(tempDirectory + "Plugins\\WPAssessmentsCatCreator\\WPAssessmentsCatCreator.dll", AWBdirectory + "WPAssessmentsCatCreator.dll", true);

                if (!Directory.Exists(AWBdirectory + "Plugins\\WPAssessmentsCatCreator"))
                    Directory.CreateDirectory(AWBdirectory + "Plugins\\WPAssessmentsCatCreator");

                File.Copy(tempDirectory + "Plugins\\WPAssessmentsCatCreator\\WPAssessmentsCatCreator.dll", AWBdirectory + "Plugins\\WPAssessmentsCatCreator\\WPAssessmentsCatCreator.dll", true);
            }
            progressUpdate.Value = 90;
        }

        private void StartAwb()
        {
            bool awbOpen = false;
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName == "AutoWikiBrowser")
                    awbOpen = true;
            }

            if (!awbOpen && MessageBox.Show("Would you like to Start AWB?", "Start AWB?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                System.Diagnostics.Process.Start(AWBdirectory + "AutoWikiBrowser.exe");

            progressUpdate.Value = 95;
        }

        private void KillTempDir()
        {
            Directory.Delete(tempDirectory, true);
            progressUpdate.Value = 100;
        }

        private void tmrTimer_Tick(object sender, EventArgs e)
        {
            tmrTimer.Enabled = false;
            UpdateAwb();
        }
    }
}