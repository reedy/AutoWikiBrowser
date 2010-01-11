/*
TypoScan AWBPlugin
Copyright (C) 2008 Sam Reed, Max Semenik

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

using System.Collections.Specialized;
using System.Windows.Forms;
using WikiFunctions.Plugin;

namespace WikiFunctions.Plugins.ListMaker.TypoScan
{
    class TypoScanAWBPlugin : IAWBPlugin
    {
        #region IAWBPlugin Members
        internal static IAutoWikiBrowser AWB;
        internal static Dictionary<string, int> PageList = new Dictionary<string, int>();
        internal static List<string> SavedPages = new List<string>();

        internal static List<string> SkippedPages = new List<string>();
        internal static List<string> SkippedReasons = new List<string>();

        internal static List<string> SavedPagesThisSession = new List<string>();
        internal static List<string> SkippedPagesThisSession = new List<string>();
        internal static int UploadedThisSession;

        internal static DateTime CheckoutTime;

        private ToolStripMenuItem pluginMenuItem = new ToolStripMenuItem("TypoScan plugin");
        private ToolStripMenuItem pluginUploadMenuItem = new ToolStripMenuItem("Upload finished articles to server now");
        private ToolStripMenuItem pluginReAddArticlesMenuItem = new ToolStripMenuItem("Re-add Unprocessed TypoScan articles to ListMaker");
        private ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("About the TypoScan plugin");

        public void Initialise(IAutoWikiBrowser sender)
        {
            AWB = sender;
            AWB.LogControl.LogAdded += new WikiFunctions.Logging.LogControl.LogAddedToControl(LogControl_LogAdded);
            AWB.AddMainFormClosingEventHandler(new FormClosingEventHandler(UploadFinishedArticlesToServer));
            AWB.AddArticleRedirectedEventHandler(new ArticleRedirected(ArticleRedirected));

            pluginMenuItem.DropDownItems.Add(pluginUploadMenuItem);
            pluginMenuItem.DropDownItems.Add(pluginReAddArticlesMenuItem);
            pluginUploadMenuItem.Click += new EventHandler(pluginUploadMenuItem_Click);

            pluginReAddArticlesMenuItem.Click += new EventHandler(pluginReAddArticlesMenuItem_Click);
            sender.PluginsToolStripMenuItem.DropDownItems.Add(pluginMenuItem);

            aboutMenuItem.Click += new EventHandler(aboutMenuItem_Click);
            sender.HelpToolStripMenuItem.DropDownItems.Add(aboutMenuItem);
        }

        private void ArticleRedirected(string oldTitle, string newTitle)
        {
            int id;
            if (PageList.TryGetValue(oldTitle, out id))
            {
                PageList.Remove(oldTitle);
                PageList.Add(newTitle, id);
            }
        }

        private void pluginReAddArticlesMenuItem_Click(object sender, EventArgs e)
        {
            foreach (string a in PageList.Keys)
            {
                if (SkippedPagesThisSession.Contains(a) || SavedPagesThisSession.Contains(a))
                    continue;

                AWB.ListMaker.Add(new Article(a));
            }
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            new About().Show();
        }

        private void pluginUploadMenuItem_Click(object sender, EventArgs e)
        {
            UploadFinishedArticlesToServer();
        }

        private void LogControl_LogAdded(bool Skipped, WikiFunctions.Logging.AWBLogListener LogListener)
        {
            int articleID;
            if ((PageList.Count > 0) && (PageList.TryGetValue(LogListener.Text, out articleID)))
            {
                if (Skipped)
                {
                    SkippedPages.Add(articleID.ToString());
                    SkippedReasons.Add(LogListener.SkipReason);
                    SkippedPagesThisSession.Add(articleID.ToString());
                }
                else
                {
                    SavedPages.Add(articleID.ToString());
                    SavedPagesThisSession.Add(articleID.ToString());
                }

                if (EditAndIgnoredPages >= 25)
                    UploadFinishedArticlesToServer();
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
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
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
            UploadFinishedArticlesToServer();
        }

        private static void UploadFinishedArticlesToServer()
        {
            int editsAndIgnored = EditAndIgnoredPages;
            if (editsAndIgnored == 0)
                return;

            AWB.StartProgressBar();
            AWB.StatusLabelText = "Uploading " + editsAndIgnored + " TypoScan articles to server...";

            NameValueCollection postVars = new NameValueCollection();

            postVars.Add("articles", string.Join(",", SavedPages.ToArray()));
            postVars.Add("skipped", string.Join(",", SkippedPages.ToArray()));
            postVars.Add("skipreason", string.Join(",", SkippedReasons.ToArray()));

            if (!AWB.Privacy)
                postVars.Add("user", Variables.User.Name);
            else
                postVars.Add("user", "[withheld]");

            try
            {
                string result = Tools.PostData(postVars, Common.GetUrlFor("finished"));
                if (string.IsNullOrEmpty(Common.CheckOperation(result)))
                {
                    UploadedThisSession += editsAndIgnored;
                    SavedPages.Clear();
                    SkippedPages.Clear();
                    SkippedReasons.Clear();

                    if ((UploadedThisSession % 100) == 0)
                        CheckoutTime = new DateTime();
                }
            }
            catch (System.IO.IOException ex)
            {
                Tools.WriteDebug("TypoScanAWBPlugin", ex.Message);
            }
            catch (System.Net.WebException we)
            {
                Tools.WriteDebug("TypoScanAWBPlugin", we.Message);
            }
            AWB.StopProgressBar();
            AWB.StatusLabelText = "";
        }
        #endregion

        internal static int EditAndIgnoredPages
        {
            get { return (SavedPages.Count + SkippedPages.Count); }
        }
    }
}