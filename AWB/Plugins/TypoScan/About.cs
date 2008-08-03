using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WikiFunctions.Plugins.ListMaker.TypoScan
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            lblSaved.Text = TypoScanAWBPlugin.SavedThisSession.ToString();
            lblSkipped.Text = TypoScanAWBPlugin.SkippedThisSession.ToString();
            lblLoaded.Text = TypoScanAWBPlugin.PageList.Count.ToString();
        }

        private void linkReedy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkReedy.LinkVisited = true;
            Tools.OpenENArticleInBrowser("Reedy", true);
        }

        private void linkMboverload_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkMboverload.LinkVisited = true;
            Tools.OpenENArticleInBrowser("Mboverload", true);
        }

        private void linkTypoScanPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkTypoScanPage.LinkVisited = true;
            Tools.OpenENArticleInBrowser("Wikipedia:TypoScan", false);
        }

        private void linkStats_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkStats.LinkVisited = true;
            Tools.OpenURLInBrowser("http://typoscan.reedyboy.net");
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
