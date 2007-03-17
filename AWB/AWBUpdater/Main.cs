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
        }

        //TODO: Functionality
        //Download Zip File from Internet
        //Unzip File
        //Copy Files to AWB Directory
        //Reload AWB

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void getAWBFromInternet()
        {
            System.Net.WebClient Client = new System.Net.WebClient();

            Client.DownloadFile(WebAddress, tempDirectory);

            Client.Dispose();
        }

        private void unzipAWB()
        {
            zipHelper.ExtractZipFile(tempDirectory + AWBZipName, tempDirectory);
        }

        private void copyFiles()
        {
            System.IO.File.Copy(tempDirectory + "AutoWikiBrowser.exe", AWBdirectory + "AutoWikiBrowser.exe");
            System.IO.File.Copy(tempDirectory + "WikiFunctions.dll", AWBdirectory + "WikiFunctions.dll");
            System.IO.File.Copy(tempDirectory + "IRCMonitor.exe", AWBdirectory + "IRCMonitor.exe");

            System.IO.File.Copy(tempDirectory + "CFD.dll", AWBdirectory + "CFD.dll");
        }

        private void startAWB()
        {
            System.Diagnostics.Process.Start(AWBdirectory + "AutoWikiBrowser.exe");
        }
    }
}