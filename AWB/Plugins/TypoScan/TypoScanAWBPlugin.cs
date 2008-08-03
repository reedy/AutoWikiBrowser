using System;
using System.Collections.Generic;
using System.Text;

using System.Collections.Specialized;
using System.Windows.Forms;
using WikiFunctions;
using WikiFunctions.Plugin;

using WikiFunctions.AWBSettings;

namespace WikiFunctions.Plugins.ListMaker.TypoScan
{
    class TypoScanAWBPlugin : IAWBPlugin
    {
        #region IAWBPlugin Members

        internal static readonly string URL = "http://typoscan.reedyboy.net/index.php?action=finished";

        internal static IAutoWikiBrowser AWB;
        internal static Dictionary<string, int> PageList = new Dictionary<string, int>();
        internal static List<string> EditedPages = new List<string>();

        internal static List<string> SkippedPages = new List<string>();
        internal static List<string> SkippedReasons = new List<string>();

        private ToolStripMenuItem pluginMenuItem = new ToolStripMenuItem("TypoScan plugin");
        private ToolStripMenuItem pluginUploadMenuItem = new ToolStripMenuItem("Upload finished articles to server now");
        private ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("About the TypoScan plugin");

        public void Initialise(IAutoWikiBrowser sender)
        {
            AWB = sender;
            AWB.LogControl.LogAdded += new WikiFunctions.Logging.LogControl.LogAddedToControl(LogControl_LogAdded);
            AWB.AddMainFormClosingEventHandler(new FormClosingEventHandler(UploadFinishedArticlesToServer));

            pluginMenuItem.DropDownItems.Add(pluginUploadMenuItem);
            pluginUploadMenuItem.Click += new EventHandler(pluginUploadMenuItem_Click);
            sender.PluginsToolStripMenuItem.DropDownItems.Add(pluginMenuItem);

            aboutMenuItem.Click += new EventHandler(aboutMenuItem_Click);
            sender.HelpToolStripMenuItem.DropDownItems.Add(aboutMenuItem);
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            new About().Show();
        }

        private void pluginUploadMenuItem_Click(object sender, EventArgs e)
        {
            UploadFinishedArticlesToServer(false);
        }

        private void LogControl_LogAdded(bool Skipped, WikiFunctions.Logging.AWBLogListener LogListener)
        {
            if (PageList.ContainsKey(LogListener.ArticleTitle))
            {
                int articleID;
                PageList.TryGetValue(LogListener.Text, out articleID);
                if (Skipped)
                {
                    SkippedPages.Add(articleID.ToString());
                    SkippedReasons.Add(LogListener.SkipReason);
                }
                else
                    EditedPages.Add(articleID.ToString());

                if (EditAndIgnoredPages() >= 25)
                    UploadFinishedArticlesToServer(false);
            }
        }

        public string Name
        {
            get { return "TypoScan AWB Plugin"; }
        }

        public string WikiName
        {
            get
            {
                return "[[Wikipedia:TypoScan|TypoScan AWB Plugin]], Plugin version " +
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string ProcessArticle(IAutoWikiBrowser sender, ProcessArticleEventArgs eventargs)
        {
            return eventargs.ArticleText;
        }

        public void LoadSettings(object[] prefs)
        {
            //if (prefs == null) return;

            //foreach (object o in prefs)
            //{
            //    PrefsKeyPair p = o as PrefsKeyPair;
            //    if (p == null) continue;

            //    switch (p.Name.ToLower())
            //    {
            //        case "pagelist":
            //            PageList = (Dictionary<string, int>)p.Setting;
            //            break;
            //    }
            //}
        }

        public object[] SaveSettings()
        {
            //PrefsKeyPair[] prefs = new PrefsKeyPair[1];
            //prefs[0] = new PrefsKeyPair("PageList", PageList);

            //return prefs;
            return new object[0];
        }

        public void Reset()
        { }

        public void Nudge(out bool Cancel)
        {
            Cancel = false;
        }

        public void Nudged(int Nudges)
        { }

        private void UploadFinishedArticlesToServer(object sender, FormClosingEventArgs e)
        {
            UploadFinishedArticlesToServer(true);
        }

        private void UploadFinishedArticlesToServer(bool appExit)
        {
            if (EditAndIgnoredPages() == 0)
                return;

            AWB.StartProgressBar();
            AWB.StatusLabelText = "Uploading " + EditAndIgnoredPages() + " TypoScan articles to server...";

            NameValueCollection postVars = new NameValueCollection();

            postVars.Add("articles", string.Join(",", EditedPages.ToArray()));
            postVars.Add("skipped", string.Join(",", SkippedPages.ToArray()));
            postVars.Add("skipreason", string.Join(",", SkippedReasons.ToArray()));

            if (!AWB.Privacy)
                postVars.Add("user", Variables.User.Name);
            else
                postVars.Add("user", "<Withheld>");

            try
            {
                string result = Tools.PostData(postVars, URL);
                if (result.Contains("Articles Updated"))
                {
                    EditedPages.Clear();
                    SkippedPages.Clear();
                    SkippedReasons.Clear();
                }
            }
            catch (System.Net.WebException we) 
            {
                if (appExit) ErrorHandler.Handle(we);
            }
            AWB.StopProgressBar();
            AWB.StatusLabelText = "";
        }
        #endregion

        private int EditAndIgnoredPages()
        {
            return (EditedPages.Count + SkippedPages.Count);
        }
    }
}
