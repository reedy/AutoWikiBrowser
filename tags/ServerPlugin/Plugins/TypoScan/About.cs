using System;
using System.Windows.Forms;

namespace WikiFunctions.Plugins.ListMaker.TypoScan
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            SetText();
        }

        private void SetText()
        {
            lblSaved.Text = TypoScanAWBPlugin.SavedPagesThisSession.Count.ToString();
            lblSkipped.Text = TypoScanAWBPlugin.SkippedPagesThisSession.Count.ToString();
            lblLoaded.Text = TypoScanAWBPlugin.PageList.Count.ToString();
            lblUploaded.Text = TypoScanAWBPlugin.UploadedThisSession.ToString();
            lblToUpload.Text = TypoScanAWBPlugin.EditAndIgnoredPages.ToString();
            DateTime checkInTime = TypoScanAWBPlugin.CheckoutTime.AddHours(3);

            if (checkInTime > DateTime.Now)
            {
                TimeSpan span = checkInTime.Subtract(DateTime.Now);
                SetTimeText(span.Hours, span.Minutes, span.Seconds);
            }
            else
                SetTimeText(0, 0, 0);
        }

        private void SetTimeText(double hours, int minutes, int seconds)
        {
            lblTimeLeft.Text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
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
            Tools.OpenURLInBrowser(Common.GetUrlFor("stats"));
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            SetText();
        }
    }
}
