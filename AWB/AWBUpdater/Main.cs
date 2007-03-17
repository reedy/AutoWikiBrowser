using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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

            AWBdirectory = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
            tempDirectory = AWBdirectory + "\\temp";
        }

        //TODO: Functionality
        //Download Zip File from Internet
        //Unzip File
        //Copy Files to AWB Directory
        //Reload AWB

        private void Form1_Load(object sender, EventArgs e)
        {
            createTempDir();
            getAWBFromInternet();
            unzipAWB();
            copyFiles();
            startAWB();
            killTempDir();

            Application.Exit();
        }

        private void createTempDir()
        {
            if (!(System.IO.Directory.Exists(tempDirectory)))
                System.IO.Directory.CreateDirectory(tempDirectory);
        }

        private void getAWBFromInternet()
        {
            System.Net.WebClient Client = new System.Net.WebClient();

            Client.DownloadFile(WebAddress, tempDirectory + AWBZipName);

            Client.Dispose();
        }

        private void unzipAWB()
        {
            zipHelper.ExtractZipFile(tempDirectory + AWBZipName, tempDirectory + AWBZipName.Replace(".zip", ""));
        }

        private void copyFiles()
        {
            System.IO.File.Copy(tempDirectory + "AutoWikiBrowser.exe", AWBdirectory + "AutoWikiBrowser.exe");
            System.IO.File.Copy(tempDirectory + "WikiFunctions.dll", AWBdirectory + "WikiFunctions.dll");
            System.IO.File.Copy(tempDirectory + "IRCMonitor.exe", AWBdirectory + "IRCMonitor.exe");

            if (System.IO.File.Exists(AWBdirectory + "\\CFD.dll"))
                            System.IO.File.Copy(tempDirectory + "\\Plugins\\CFD\\CFD.dll", AWBdirectory + "CFD.dll");
            else
                            System.IO.File.Copy(tempDirectory + "\\Plugins\\CFD\\CFD.dll", AWBdirectory + "\\Plugins\\CFD\\CFD.dll");

            if (System.IO.File.Exists(AWBdirectory + "\\Kingbotk AWB Plugin.dll"))
                            System.IO.File.Copy(tempDirectory + "\\Plugins\\Kingbotk (WikiProject tagging)\\Kingbotk AWB Plugin.dll", AWBdirectory + "Kingbotk AWB Plugin.dll");
            else
                            System.IO.File.Copy(tempDirectory + "\\Plugins\\Kingbotk (WikiProject tagging)\\Kingbotk AWB Plugin.dll", AWBdirectory + "\\Plugins\\Kingbotk (WikiProject tagging)\\Kingbotk AWB Plugin.dll");

            if (System.IO.File.Exists(AWBdirectory + "\\WikiFunctions2.dll"))
                            System.IO.File.Copy(tempDirectory + "\\Plugins\\Kingbotk (WikiProject tagging)\\WikiFunctions2.dll", AWBdirectory + "WikiFunctions2.dll");
            else
                            System.IO.File.Copy(tempDirectory + "\\Plugins\\Kingbotk (WikiProject tagging)\\WikiFunctions2.dll", AWBdirectory + "\\Plugins\\Kingbotk (WikiProject tagging)\\WikiFunctions2.dll");
        }

        private void startAWB()
        {
            System.Diagnostics.Process.Start(AWBdirectory + "AutoWikiBrowser.exe");
        }

        private void killTempDir()
        {
            System.IO.Directory.Delete(tempDirectory);
        }
    }
}