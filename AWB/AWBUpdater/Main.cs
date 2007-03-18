using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.IO;

namespace AWBUpdater
{
    public partial class Main : Form
    {
        string AWBdirectory;
        string tempDirectory;
        string AWBZipName = "AutoWikiBrowser3140.zip";
        string WebAddress = "http://192.168.0.10/AutoWikiBrowser3140.zip";

        public Main()
        {
            InitializeComponent();

            AWBdirectory = Path.GetDirectoryName(Application.ExecutablePath) + "\\";
            tempDirectory = AWBdirectory + "temp\\";
        }

        //TODO: Functionality
        //Pass Where file to be downloaded from, and file name

        private void Form1_Load(object sender, EventArgs e)
        {
            createTempDir();
            getAWBFromInternet();
            unzipAWB();
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
            zipHelper.ExtractZipFile(tempDirectory + AWBZipName, tempDirectory);

            progressUpdate.Value = 70;
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