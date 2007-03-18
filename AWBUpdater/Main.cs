using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip.Compression;
using System.Text.RegularExpressions;
using System.Net;

namespace AWBUpdater
{
    public partial class Main : Form
    {
        string AWBdirectory;
        string tempDirectory;
        string AWBZipName;
        string WebAddress;

        public Main()
        {
            InitializeComponent();

            AWBdirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            tempDirectory = AWBdirectory + "temp\\";
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
                throw;
            }

            Match m_version = Regex.Match(text, @"&lt;!-- Current version: (.*?) --&gt;");
            try
            {
                if (m_version.Success && m_version.Groups[1].Value.Length == 4)
                {
                    AWBZipName = "AutoWikiBrowser" + m_version.Groups[1].Value.Replace(".", "") + ".zip";
                    WebAddress = "http://umn.dl.sourceforge.net/sourceforge/autowikibrowser/AutoWikiBrowser" + m_version.Groups[1].Value.Replace(".", "") + ".zip";
                }
                else
                    throw new Exception();
            }
            catch
            {
                MessageBox.Show("Error fetching current version number.");
            }
        }

        //TODO: Functionality
        //Pass Where file to be downloaded from, and file name

        private void Form1_Load(object sender, EventArgs e)
        {
            AWBversion();
            createTempDir();
            getAWBFromInternet();
            unzipAWB();
            MessageBox.Show("Please save your settings (if you wish) and close AutoWikiBrowser completely before pressing OK.");
            deleteOldFiles();
            copyFiles();
            MessageBox.Show("AWB Update Successful", "Update Sucessful");
            startAWB();
            killTempDir();
            Application.Exit();
        }

        private void createTempDir()
        {
            if (!(Directory.Exists(tempDirectory)))
                Directory.CreateDirectory(tempDirectory);

            progressUpdate.Value = 5;
        }

        private void getAWBFromInternet()
        {
            System.Net.WebClient Client = new System.Net.WebClient();

            Client.DownloadFile(WebAddress, tempDirectory + AWBZipName);
            Client.Dispose();

            progressUpdate.Value = 50;
        }

        private void unzipAWB()
        {            
            //zipHelper.ExtractZipFile(tempDirectory + AWBZipName, tempDirectory);
            
            using (ZipFile zf = new ZipFile(tempDirectory + AWBZipName))
			{
				foreach ( ZipEntry entry in zf )
				{
                    if (entry.IsFile)
                    	ExtractFile(zf.GetInputStream(entry), entry, tempDirectory);
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
            {
                entryFileName = theEntry.Name;
            }

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
                {
                    MessageBox.Show("Error in updating AWB.");
                }
            }

            if (entryFileName.Length > 0)
            {
                using (FileStream outputStream = File.Create(targetName))
                {
                    StreamUtils.Copy(inputStream, outputStream, new byte[4096]);
                }
            }
        }

        private void deleteOldFiles()
        {
            if (File.Exists(AWBdirectory + "AutoWikiBrowser.exe")) File.Delete(AWBdirectory + "AutoWikiBrowser.exe");

            if (File.Exists(AWBdirectory + "WikiFunctions.dll")) File.Delete(AWBdirectory + "WikiFunctions.dll");

            if (File.Exists(AWBdirectory + "IRCMonitor.exe")) File.Delete(AWBdirectory + "IRCMonitor.exe");

            if (File.Exists(AWBdirectory + "IRCMonitor.exe")) File.Delete(AWBdirectory + "IRCMonitor.exe");

            if (File.Exists(AWBdirectory + "CFD.dll")) File.Delete(AWBdirectory + "CFD.dll");

            if (File.Exists(AWBdirectory + "Plugins\\CFD\\CFD.dll")) File.Delete(AWBdirectory + "Plugins\\CFD\\CFD.dll");

            if (File.Exists(AWBdirectory + "Kingbotk AWB Plugin.dll")) File.Delete(AWBdirectory + "Kingbotk AWB Plugin.dll");

            if (File.Exists(AWBdirectory + "Plugins\\Kingbotk (WikiProject tagging)\\Kingbotk AWB Plugin.dll")) File.Delete(AWBdirectory + "Plugins\\Kingbotk (WikiProject tagging)\\Kingbotk AWB Plugin.dll");

            if (File.Exists(AWBdirectory + "WikiFunctions2.dll")) File.Delete(AWBdirectory + "WikiFunctions2.dll");

            if (File.Exists(AWBdirectory + "Plugins\\Kingbotk (WikiProject tagging)\\WikiFunctions2.dll")) File.Delete(AWBdirectory + "Plugins\\Kingbotk (WikiProject tagging)\\WikiFunctions2.dll");

            progressUpdate.Value = 80;
        }

        private void copyFiles()
        {
            File.Copy(tempDirectory + "AutoWikiBrowser.exe", AWBdirectory + "AutoWikiBrowser.exe");
            File.Copy(tempDirectory + "WikiFunctions.dll", AWBdirectory + "WikiFunctions.dll");
            File.Copy(tempDirectory + "IRCMonitor.exe", AWBdirectory + "IRCMonitor.exe");

            if (File.Exists(AWBdirectory + "CFD.dll"))
                File.Copy(tempDirectory + "Plugins\\CFD\\CFD.dll", AWBdirectory + "CFD.dll");
            else
            {
                if (!(Directory.Exists(AWBdirectory + "\\Plugins\\CFD")))
                    Directory.CreateDirectory(AWBdirectory + "\\Plugins\\CFD");

                File.Copy(tempDirectory + "Plugins\\CFD\\CFD.dll", AWBdirectory + "Plugins\\CFD\\CFD.dll");
            }

            if (File.Exists(AWBdirectory + "Kingbotk AWB Plugin.dll"))
                File.Copy(tempDirectory + "Plugins\\Kingbotk (WikiProject tagging)\\Kingbotk AWB Plugin.dll", AWBdirectory + "Kingbotk AWB Plugin.dll");
            else
            {
                if (!(Directory.Exists(AWBdirectory + "Plugins\\Kingbotk (WikiProject tagging)")))
                    Directory.CreateDirectory(AWBdirectory + "Plugins\\Kingbotk (WikiProject tagging)");

                File.Copy(tempDirectory + "Plugins\\Kingbotk (WikiProject tagging)\\Kingbotk AWB Plugin.dll", AWBdirectory + "Plugins\\Kingbotk (WikiProject tagging)\\Kingbotk AWB Plugin.dll");
            }

            if (File.Exists(AWBdirectory + "WikiFunctions2.dll"))
                File.Copy(tempDirectory + "Plugins\\Kingbotk (WikiProject tagging)\\WikiFunctions2.dll", AWBdirectory + "WikiFunctions2.dll");
            else
            {
                if (!(Directory.Exists(AWBdirectory + "Plugins\\Kingbotk (WikiProject tagging)")))
                    Directory.CreateDirectory(AWBdirectory + "Plugins\\Kingbotk (WikiProject tagging)");

                File.Copy(tempDirectory + "Plugins\\Kingbotk (WikiProject tagging)\\WikiFunctions2.dll", AWBdirectory + "Plugins\\Kingbotk (WikiProject tagging)\\WikiFunctions2.dll");
            }
                        
            progressUpdate.Value = 90;
        }

        private void startAWB()
        {
            System.Diagnostics.Process.Start(AWBdirectory + "AutoWikiBrowser.exe");

            progressUpdate.Value = 95;
        }

        private void killTempDir()
        {
            Directory.Delete(tempDirectory, true);

            progressUpdate.Value = 100;
        }
    }
}