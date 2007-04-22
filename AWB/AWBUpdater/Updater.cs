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

namespace AWBUpdater
{
    public partial class Updater : Form
    {
        string AWBdirectory;
        string tempDirectory;
        string AWBZipName;
        string AWBWebAddress;

        string UpdaterZipName;
        string UpdaterWebAddress;

        bool noUpdates;
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

        private void updateAWB()
        {
            try
            {
                lblCurrentTask.Text = "Getting Current AWB and Updater Versions";
                Application.DoEvents();
                AWBversion();

                if (noUpdates)
                {
                    startAWB();
                    Application.Exit();
                }
                else
                {
                    Application.DoEvents();

                    lblCurrentTask.Text = "Creating a Temporary Directory";
                    Application.DoEvents();
                    createTempDir();

                    lblCurrentTask.Text = "Downloading AWB";
                    Application.DoEvents();
                    getAWBFromInternet();


                    lblCurrentTask.Text = "Unzipping AWB to the Temporary Directory";
                    Application.DoEvents();
                    unzipAWB();

                    MessageBox.Show("Please save your settings (if you wish) and close AutoWikiBrowser completely before pressing OK.");

                    lblCurrentTask.Text = "Making Sure AWB is Closed";
                    Application.DoEvents();
                    closeAWB();

                    lblCurrentTask.Text = "Copying AWB Files from temp to AWB Directory";
                    Application.DoEvents();
                    copyFiles();
                    MessageBox.Show("AWB Update Successful", "Update Sucessful");

                    lblCurrentTask.Text = "Starting AWB";
                    Application.DoEvents();
                    startAWB();

                    lblCurrentTask.Text = "Cleaning up from Update";
                    Application.DoEvents();
                    killTempDir();

                    Application.Exit();
                }
            }
            catch { }
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
            }

            Match m_awbversion = Regex.Match(text, @"&lt;!-- Current version: (.*?) --&gt;");
            Match m_awbnewest = Regex.Match(text, @"&lt;!-- Newest version: (.*?) --&gt;");
            Match m_updversion = Regex.Match(text, @"&lt;!-- Updater version: (.*?) --&gt;");

            if ((m_awbversion.Success && m_awbversion.Groups[1].Value.Length == 4) || (m_awbnewest.Success && m_awbnewest.Groups[1].Value.Length == 4))
            {
                try
                {
                    try
                    {
                        FileVersionInfo versionAWB = FileVersionInfo.GetVersionInfo(AWBdirectory + "AutoWikiBrowser.exe");

                        if ((Convert.ToInt32(versionAWB.FileVersion.Replace(".", ""))) < (Convert.ToInt32(m_awbversion.Groups[1].Value)))
                        {
                            AWBZipName = "AutoWikiBrowser" + m_awbnewest.Groups[1].Value.Replace(".", "") + ".zip";
                            AWBWebAddress = "http://downloads.sourceforge.net/autowikibrowser/" + AWBZipName;
                            awbUpdate = true;
                        }
                        else if ((Convert.ToInt32(versionAWB.FileVersion.Replace(".", "")) >= (Convert.ToInt32(m_awbversion.Groups[1].Value))) && ((Convert.ToInt32(versionAWB.FileVersion.Replace(".", "")) < (Convert.ToInt32(m_awbnewest.Groups[1].Value)))))
                        {
                            if (MessageBox.Show("There is an Optional Update to AutoWikiBrowser. Would you like to Upgrade?", "Optional Update", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                AWBZipName = "AutoWikiBrowser" + m_awbnewest.Groups[1].Value.Replace(".", "") + ".zip";
                                AWBWebAddress = "http://downloads.sourceforge.net/autowikibrowser/" + AWBZipName;
                                awbUpdate = true;
                            }
                            else
                                awbUpdate = false;
                        }
                        else
                            awbUpdate = false;

                        if (m_updversion.Success && m_updversion.Groups[1].Value.Length == 4)
                        {
                            if (Convert.ToInt32(m_updversion.Groups[1].Value) > Convert.ToInt32(AssemblyVersion.Replace(".", "")))
                            {
                                UpdaterZipName = "AWBUpdater" + m_updversion.Groups[1].Value.Replace(".", "") + ".zip";
                                UpdaterWebAddress = "http://downloads.sourceforge.net/autowikibrowser/" + UpdaterZipName;
                                updaterUpdate = true;
                            }
                            else
                            {
                                updaterUpdate = false;
                                UpdaterZipName = "";
                                UpdaterWebAddress = "";
                            }
                        }
                        else
                            updaterUpdate = false;
                    }
                    catch
                    { MessageBox.Show("Unable to find AutoWikiBrowser.exe to query Version No."); }
                }
                catch
                {
                    MessageBox.Show("Error fetching current version number.");
                }

                if (!updaterUpdate && !awbUpdate)
                {
                    MessageBox.Show("Nothing to Update. The Updater will now close");
                    noUpdates = true;
                }

                progressUpdate.Value = 30;
            }

        }

        private void createTempDir()
        {
            if (!(Directory.Exists(tempDirectory)))
                Directory.CreateDirectory(tempDirectory);

            progressUpdate.Value = 35;
        }

        private void getAWBFromInternet()
        {
            System.Net.WebClient Client = new System.Net.WebClient();

            if (AWBWebAddress != "")
                Client.DownloadFile(AWBWebAddress, tempDirectory + AWBZipName);

            if (UpdaterWebAddress != "")
                Client.DownloadFile(UpdaterWebAddress, tempDirectory + UpdaterZipName);

            Client.Dispose();

            progressUpdate.Value = 50;
        }

        private void unzipAWB()
        {
            if (AWBZipName != "")
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

            if (UpdaterZipName != "")
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
            if (Directory.Exists(fullPath) == false)
            {
                try
                {
                    Directory.CreateDirectory(fullPath);
                }
                catch
                { MessageBox.Show("Error in updating AWB."); }
            }

            if (entryFileName.Length > 0)
            {
                using (FileStream outputStream = File.Create(targetName))
                {
                    StreamUtils.Copy(inputStream, outputStream, new byte[4096]);
                }
            }
        }

        private void closeAWB()
        {
            bool AWBOpen = new bool();

            do
            {
                AWBOpen = false;

                foreach (Process p in Process.GetProcesses())
                {
                    if (p.ProcessName == "AutoWikiBrowser")
                    {
                        AWBOpen = true;
                        MessageBox.Show("Please save your settings (if you wish) and close AutoWikiBrowser completely before pressing OK.");
                    }
                    else if(p.ProcessName == "IRCMonitor")
                    {
                        AWBOpen = true;
                        MessageBox.Show("Please save your settings (if you wish) and close IRCMonitor completely before pressing OK.");
                    }
                }
            }
            while (AWBOpen == true);

            progressUpdate.Value = 75;
        }

        private void copyFiles()
        {
            if (updaterUpdate)
            {
                if (File.Exists(tempDirectory + "AWBUpdater.exe"))
                    File.Copy(tempDirectory + "AWBUpdater.exe", AWBdirectory + "AWBUpdater.exe.new", true);
            }

            if (awbUpdate)
            {
                File.Copy(tempDirectory + "AutoWikiBrowser.exe", AWBdirectory + "AutoWikiBrowser.exe", true);
                File.Copy(tempDirectory + "WikiFunctions.dll", AWBdirectory + "WikiFunctions.dll", true);
                File.Copy(tempDirectory + "IRCMonitor.exe", AWBdirectory + "IRCMonitor.exe", true);

                if (File.Exists(AWBdirectory + "CFD.dll"))
                    File.Copy(tempDirectory + "Plugins\\CFD\\CFD.dll", AWBdirectory + "CFD.dll", true);

                if (!(Directory.Exists(AWBdirectory + "\\Plugins\\CFD")))
                    Directory.CreateDirectory(AWBdirectory + "\\Plugins\\CFD");

                File.Copy(tempDirectory + "Plugins\\CFD\\CFD.dll", AWBdirectory + "Plugins\\CFD\\CFD.dll", true);

                if (File.Exists(AWBdirectory + "Kingbotk AWB Plugin.dll"))
                    File.Copy(tempDirectory + "Plugins\\Kingbotk (WikiProject tagging)\\Kingbotk AWB Plugin.dll", AWBdirectory + "Kingbotk AWB Plugin.dll", true);

                if (!(Directory.Exists(AWBdirectory + "Plugins\\Kingbotk (WikiProject tagging)")))
                    Directory.CreateDirectory(AWBdirectory + "Plugins\\Kingbotk (WikiProject tagging)");

                File.Copy(tempDirectory + "Plugins\\Kingbotk (WikiProject tagging)\\Kingbotk AWB Plugin.dll", AWBdirectory + "Plugins\\Kingbotk (WikiProject tagging)\\Kingbotk AWB Plugin.dll", true);

                if (File.Exists(AWBdirectory + "WikiFunctions2.dll"))
                    File.Copy(tempDirectory + "Plugins\\Kingbotk (WikiProject tagging)\\WikiFunctions2.dll", AWBdirectory + "WikiFunctions2.dll", true);

                File.Copy(tempDirectory + "Plugins\\Kingbotk (WikiProject tagging)\\WikiFunctions2.dll", AWBdirectory + "Plugins\\Kingbotk (WikiProject tagging)\\WikiFunctions2.dll", true);

                if (File.Exists(AWBdirectory + "WPAssessmentsCatCreator.dll"))
                    File.Copy(tempDirectory + "Plugins\\WPAssessmentsCatCreator\\WPAssessmentsCatCreator.dll", AWBdirectory + "WPAssessmentsCatCreator.dll", true);

                if (!Directory.Exists(AWBdirectory + "Plugins\\WPAssessmentsCatCreator"))
                    Directory.CreateDirectory(AWBdirectory + "Plugins\\WPAssessmentsCatCreator");

                File.Copy(tempDirectory + "Plugins\\WPAssessmentsCatCreator\\WPAssessmentsCatCreator.dll", AWBdirectory + "Plugins\\WPAssessmentsCatCreator\\WPAssessmentsCatCreator.dll", true);
            }
            progressUpdate.Value = 90;
        }

        private void startAWB()
        {
            bool AWBOpen = false;
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName == "AutoWikiBrowser")
                    AWBOpen = true;
            }

            if (!AWBOpen && MessageBox.Show("Would you like to Start AWB?", "Start AWB?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                System.Diagnostics.Process.Start(AWBdirectory + "AutoWikiBrowser.exe");

            progressUpdate.Value = 95;
        }

        private void killTempDir()
        {
            Directory.Delete(tempDirectory, true);
            progressUpdate.Value = 100;
        }

        private void tmrTimer_Tick(object sender, EventArgs e)
        {
            tmrTimer.Enabled = false;
            updateAWB();
        }
    }
}