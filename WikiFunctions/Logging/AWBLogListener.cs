/*
(C) 2007 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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
using System.Windows.Forms;

namespace WikiFunctions.Logging
{
    [Serializable]
    public class AWBLogListener : ListViewItem, IAWBTraceListener
    {    /* This class will:
         * Use the Logging interface previously defined in wikifunctions2
         * Be written to by AWB during processing and passed to plugins
         * Format itself as a ListViewItem suitable (with subitems where appropriate) for adding to the skipped 
           or processed articles list
         * Handle MouseOver event (display log entry)
         * Handle double click event (open article in browser)
        */

        public const string UploadingLogEntryDefaultEditSummary = "Adding log entry",
                     UploadingLogDefaultEditSummary = "Uploading log",
                     LoggingStartButtonClicked = "Initialising log.",
                     StringUser = "User",
                     StringUserSkipped = "Clicked ignore",
                     StringPlugin = "Plugin",
                     StringPluginSkipped = "Plugin sent skip event";
        
        public static string AWBLoggingEditSummary
        { get { return "(" + Variables.WPAWB + " Logging) "; } }

        private bool Datestamped, HaveSkipInfo;

        #region AWB Interface
        public bool Skipped { get; internal set; }

        public AWBLogListener(string articleTitle)
        {
            Text = articleTitle;
            ArticleTitle = articleTitle;
        }

        public void UserSkipped()
        {
            Skip(StringUser, StringUserSkipped);
        }

        public void AWBSkipped(string reason)
        {
            Skip("AWB", reason);
        }

        public void PluginSkipped()
        {
            Skip(StringPlugin, StringPluginSkipped);
        }

        public void OpenInBrowser()
        {
            Tools.OpenArticleInBrowser(ArticleTitle);
        }

        public void OpenHistoryInBrowser()
        {
            Tools.OpenArticleHistoryInBrowser(ArticleTitle);
        }

        public void AddAndDateStamp(ListView listView)
        {
            ListViewSubItem dateStamp = new ListViewSubItem {Text = DateTime.Now.ToString()};

            base.SubItems.Insert(1, dateStamp);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#ArgumentException_in_AWBLogListener.AddAndDateStamp
            // TODO resolve exception by prevention rather than simply catching
            try
            {
                listView.Items.Insert(0, this);
            }

            catch { }

            Datestamped = true;
        }

        public string Output(LogFileType logFileType)
        {
            switch (logFileType)
            {
                case LogFileType.AnnotatedWikiText:
                    string output = "*" + TimeStamp + ": [[" + ArticleTitle + "]]\r\n";
                    if (Skipped)
                        output += "'''Skipped''' by: " + SkippedBy + "\r\n" + "Skip reason: " +
                            SkipReason + "\r\n";
                    return output + ToolTipText + "\r\n";

                case LogFileType.PlainText:
                    return ArticleTitle;

                case LogFileType.WikiText:
                    return "#[[:" + ArticleTitle + "]]";

                default:
                    throw new ArgumentOutOfRangeException("LogFileType");
            }
        }

        public string ArticleTitle { get; private set; }

        public string SkipReason
        {
            get { return GetSubItemText(SubItem.SkippedReason); }
            protected set { SetSubItemText(SubItem.SkippedReason, value); }
        }

        public string TimeStamp
        {
            get { return GetSubItemText(SubItem.TimeStamp); }
        }

        public string SkippedBy
        {
            get { return GetSubItemText(SubItem.SkippedBy); }
            protected set { SetSubItemText(SubItem.SkippedBy, value); }
        }
        #endregion

        #region IMyTraceListener Members
        void IMyTraceListener.Close() { }
        void IMyTraceListener.Flush() { }
        void IMyTraceListener.ProcessingArticle(string fullArticleTitle, int ns) { }
        void IMyTraceListener.WriteComment(string line) { }
        void IMyTraceListener.WriteCommentAndNewLine(string line) { }

        void IMyTraceListener.SkippedArticle(string skippedBy, string reason)
        {
            Skip(skippedBy, reason);
        }

        void IMyTraceListener.SkippedArticleBadTag(string skippedBy, string fullArticleTitle, int ns)
        {
            Skip(skippedBy, "Bad tag");
        }

        void IMyTraceListener.SkippedArticleRedlink(string skippedBy, string fullArticleTitle, int ns)
        {
            Skip(skippedBy, "Red link (article deleted)");
        }

        bool IMyTraceListener.Uploadable
        {
            get { return false; }
        }

        void IMyTraceListener.WriteArticleActionLine(string line, string pluginName, bool verboseOnly)
        {
            if (!verboseOnly) WriteLine(line, pluginName);
        }

        void IMyTraceListener.WriteArticleActionLine(string line, string pluginName)
        {
            WriteLine(line, pluginName);
        }

        void IMyTraceListener.WriteBulletedLine(string line, bool bold, bool verboseOnly)
        {
            if (!verboseOnly) Write(line);
        }

        void IMyTraceListener.WriteBulletedLine(string line, bool bold, bool verboseOnly, bool dateStamp)
        {
            if (!verboseOnly) Write(line);
        }

        void IMyTraceListener.WriteLine(string line)
        {
            Write(line);
        }

        void IMyTraceListener.WriteTemplateAdded(string template, string pluginName)
        {
            WriteLine("{{" + template + "}} added", pluginName);
        }

        public void Write(string text)
        {
            if (string.IsNullOrEmpty(ToolTipText.Trim()))
            { ToolTipText = text; }
            else
            { ToolTipText = text + Environment.NewLine + ToolTipText; }
        }

        public void WriteLine(string text, string sender)
        {
            if (!string.IsNullOrEmpty(text.Trim())) Write(sender + ": " + text);
        }
        #endregion

        private enum SubItem
        {
            SkippedBy,
            SkippedReason,
            TimeStamp
        };

        /// <summary>
        /// Returns the ListViewItem.SubItems number for a specified piece of information
        /// </summary>
        /// <param name="subItem"></param>
        /// <returns>-1 if the SubItem doesn't exist</returns>
        private int GetSubItemNumber(SubItem subItem)
        {
            switch (subItem)
            {
                case SubItem.SkippedBy:
                    return Datestamped ? 2 : 1;

                case SubItem.SkippedReason:
                    return Datestamped ? 3 : 2;

                case SubItem.TimeStamp:
                    return (Datestamped) ? 1 : -1;

                default:
                    throw new ArgumentOutOfRangeException("SubItem");
            }
        }

        /// <summary>
        /// Returns the Text from a ListViewItem.SubItems object
        /// </summary>
        private string GetSubItemText(SubItem subItem)
        {
            switch (subItem)
            {
                case SubItem.SkippedBy:
                case SubItem.SkippedReason:
                    return HaveSkipInfo ? base.SubItems[GetSubItemNumber(subItem)].Text : "";

                case SubItem.TimeStamp:
                    return Datestamped ? base.SubItems[1].Text : "";

                default:
                    return base.SubItems[GetSubItemNumber(subItem)].Text;
            }
        }

        private void SetSubItemText(SubItem subItem, string value)
        {
            if ((subItem == SubItem.SkippedBy || subItem == SubItem.SkippedReason) &! (HaveSkipInfo))
            {
                base.SubItems.Add("SkippedBy");
                base.SubItems.Add("SkipReason");
                HaveSkipInfo = true;
            }

            base.SubItems[GetSubItemNumber(subItem)].Text = value;
        }

        protected void Skip(string mSkippedBy, string mSkipReason)
        {
            SetSubItemText(SubItem.SkippedBy, mSkippedBy);
            SetSubItemText(SubItem.SkippedReason, mSkipReason);
            WriteLine(SkipReason, SkippedBy);
            Skipped = true;
        }

        // disable access to underlying Items property
        public static new ListViewSubItemCollection SubItems
        {
            get { throw new NotImplementedException("The SubItems property should not be accessed directly"); }
        }
    }
}
