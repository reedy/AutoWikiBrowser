using System;
using System.Collections.Generic;
using System.Text;

using System.Collections.Specialized;
using System.Windows.Forms;
using WikiFunctions;
using WikiFunctions.Plugin;

namespace WikiFunctions.Plugins.ListMaker.TypoScan
{
    class TypoScanAWBPlugin : IAWBPlugin
    {
        #region IAWBPlugin Members

        internal static readonly string URL = "http://typoscan.reedyboy.net/index.php?finished=true";

        internal static IAutoWikiBrowser AWB;
        internal static Dictionary<string, int> PageList = new Dictionary<string, int>();
        internal static List<string> FinishedPages = new List<string>();

        private ToolStripMenuItem pluginMenuItem = new ToolStripMenuItem("TypoScan plugin");
        private ToolStripMenuItem pluginUploadMenuItem = new ToolStripMenuItem("Upload finished to server now");

        public void Initialise(IAutoWikiBrowser sender)
        {
            AWB = sender;
            AWB.LogControl.LogAdded += new WikiFunctions.Logging.LogControl.LogAddedToControl(LogControl_LogAdded);

            pluginMenuItem.DropDownItems.Add(pluginUploadMenuItem);
            pluginUploadMenuItem.Click += new EventHandler(pluginUploadMenuItem_Click);
            sender.PluginsToolStripMenuItem.DropDownItems.Add(pluginMenuItem);
        }

        private void pluginUploadMenuItem_Click(object sender, EventArgs e)
        {
            UploadFinishedToServer();
        }

        private void LogControl_LogAdded(bool Skipped, WikiFunctions.Logging.AWBLogListener LogListener)
        {
            if (PageList.ContainsKey(LogListener.Text))
            {
                int articleID;
                PageList.TryGetValue(LogListener.Text, out articleID);
                FinishedPages.Add(articleID.ToString());
            }
        }

        public string Name
        {
            get { return "TypoScan AWB Plugin]]"; }
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
        { }

        public object[] SaveSettings()
        {
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

        private void UploadFinishedToServer()
        {
            string pages = string.Join(",", FinishedPages.ToArray());

            NameValueCollection postVars = new NameValueCollection();

            postVars.Add("articles", pages);

            Tools.PostData(postVars, URL);

            FinishedPages.Clear();
        }
        #endregion
    }
}
