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
using System.Collections.Generic;
using System.Text;

namespace WikiFunctions.Logging
{
    public class AWBLogListener : ListViewItem, IAWBTraceListener
    {    /* This class will:
         * Use the Logging interface previously defined in wikifunctions2
         * Be written to by AWB during processing and passed to plugins
         * Format itself as a ListViewItem suitable (with subitems where appropriate) for adding to the skipped 
           or processed articles list
         * Handle MouseOver event (display log entry)
         * Handle double click event (open article in browser)
        */

        private string mArticle; /* store this locally rather than relying on .Text in case a Plugin changes .Text
                                   * (it shouldn', and Kingbotk doesn't, but who knows :) */
        private bool mSkipped;

        #region AWB Interface
        public bool Skipped
        { get { return mSkipped; } }

        public AWBLogListener(string ArticleTitle)
        {
            Text = ArticleTitle;
            mArticle = ArticleTitle;
        }

        public void UserSkipped()
        {
            Skip(Variables.StringUser, Variables.StringUserSkipped);
        }

        public void AWBSkipped(string Reason)
        {
            Skip("AWB", Reason);
        }

        public void PluginSkipped()
        {
            Skip(Variables.StringPlugin, Variables.StringPluginSkipped);
        }

        public void OpenInBrowser()
        {
            Tools.OpenArticleInBrowser(mArticle);
        }

        public void OpenHistoryInBrowser()
        {
            Tools.OpenArticleHistoryInBrowser(mArticle);
        }

        public ListViewItem AddAndDateStamp(ListView ListView)
        {
            ListViewItem item = ListView.Items.Insert(0, this);
            item.SubItems.Add(DateTime.Now.ToString());
            return item;
        }

        public string Output(LogFileType LogFileType)
        {
            switch (LogFileType)
            {
                case LogFileType.AnnotatedWikiText:
                    string Output = "*" + TimeStamp + ": [[" + mArticle + "]]\r\n";
                    if (mSkipped)
                        Output += "'''Skipped''' by: " + SkippedBy + "\r\n" + "Skip reason: " +
                            SkipReason + "\r\n";
                    return Output + ToolTipText + "\r\n";

                case LogFileType.PlainText:
                    return mArticle + "\r\n";

                case LogFileType.WikiText:
                    return "#[[" + mArticle + "]]\r\n";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string SkipReason
        {
            get { return GetSubItemText(3); }
            protected set
            {
                try
                {
                    base.SubItems[3].Text = value;
                }
                catch (Exception ex)
                { // TODO: Remove this errhandler if no user reports this error; it's just that we had a problem with an items[3] somewhere
                    MessageBox.Show("Error in logging object (set_SkipReason), please report this: " +
                        ex.Message, "Error", MessageBoxButtons.OK);
                }
            }
        }

        public string TimeStamp
        {
            get { return GetSubItemText(1); }
        }

        public string SkippedBy
        {
            get { return GetSubItemText(2); }
        }
        #endregion

        #region IMyTraceListener Members
        void IMyTraceListener.Close() { }
        void IMyTraceListener.Flush() { }
        void IMyTraceListener.ProcessingArticle(string FullArticleTitle, Namespaces NS) { }
        void IMyTraceListener.WriteComment(string Line) { }
        void IMyTraceListener.WriteCommentAndNewLine(string Line) { }

        void IMyTraceListener.SkippedArticle(string SkippedBy, string Reason)
        {
            Skip(SkippedBy, Reason);
        }

        void IMyTraceListener.SkippedArticleBadTag(string SkippedBy, string FullArticleTitle, Namespaces NS)
        {
            Skip(SkippedBy, "Bad tag");
        }

        void IMyTraceListener.SkippedArticleRedlink(string SkippedBy, string FullArticleTitle, Namespaces NS)
        {
            Skip(SkippedBy, "Red link (article deleted)");
        }

        bool IMyTraceListener.Uploadable
        {
            get { return false; }
        }

        void IMyTraceListener.WriteArticleActionLine(string Line, string PluginName, bool VerboseOnly)
        {
            if (!VerboseOnly) WriteLine(Line, PluginName);
        }

        void IMyTraceListener.WriteArticleActionLine(string Line, string PluginName)
        {
            WriteLine(Line, PluginName);
        }

        void IMyTraceListener.WriteBulletedLine(string Line, bool Bold, bool VerboseOnly)
        {
            if (!VerboseOnly) Write(Line);
        }

        void IMyTraceListener.WriteBulletedLine(string Line, bool Bold, bool VerboseOnly, bool DateStamp)
        {
            if (!VerboseOnly) Write(Line);
        }

        void IMyTraceListener.WriteLine(string Line)
        {
            Write(Line);
        }

        void IMyTraceListener.WriteTemplateAdded(string Template, string PluginName)
        {
            WriteLine("{{" + Template + "}} added", PluginName);
        }

        public void Write(string Text)
        {
            if (ToolTipText.Trim() == "")
            { ToolTipText = Text; }
            else
            { ToolTipText = Text + System.Environment.NewLine + ToolTipText; }
        }

        public void WriteLine(string Text, string Sender)
        {
            if (Text.Trim() != "") Write(Sender + ": " + Text);
        }
        #endregion

        private string GetSubItemText(int SubItem)
        {
            try
            {
                return base.SubItems[SubItem].Text;
            }
            catch (Exception ex)
            { // TODO: Remove this errhandler if no user reports this error; it's just that we had a problem with an items[3] somewhere
                MessageBox.Show("Error in logging object (GetSubItemText), please report this: " + ex.Message, "Error", MessageBoxButtons.OK);
                return "";
            }
        }

        protected void Skip(string SkippedBy, string SkipReason)
        {
            base.SubItems.Add(SkippedBy);
            base.SubItems.Add(SkipReason);
            WriteLine(SkipReason, SkippedBy);
            mSkipped = true;
        }
    }
}
