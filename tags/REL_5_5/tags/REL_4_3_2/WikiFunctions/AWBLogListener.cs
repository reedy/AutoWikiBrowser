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

        private string mArticle; /* store this locally rather than relying on .Text in case a Plugin changes .Text
                                   * (it shouldn't, and Kingbotk doesn't, but who knows :) ) */
        private bool mSkipped;
        private bool Datestamped, HaveSkipInfo;

        #region AWB Interface
        public bool Skipped
        { get { return mSkipped; } internal set { mSkipped = value; } }

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

        public void AddAndDateStamp(ListView ListView)
        {
            ListViewSubItem dateStamp = new ListViewSubItem();
            dateStamp.Text = DateTime.Now.ToString();

            base.SubItems.Insert(1, dateStamp);
            ListView.Items.Insert(0, this);
            Datestamped = true;
        }

        public string Output(LogFileType LogFileType)
        {
            switch (LogFileType)
            {
                case LogFileType.AnnotatedWikiText:
                    string output = "*" + TimeStamp + ": [[" + mArticle + "]]\r\n";
                    if (mSkipped)
                        output += "'''Skipped''' by: " + SkippedBy + "\r\n" + "Skip reason: " +
                            SkipReason + "\r\n";
                    return output + ToolTipText + "\r\n";

                case LogFileType.PlainText:
                    return mArticle;

                case LogFileType.WikiText:
                    return "#[[:" + mArticle + "]]";

                default:
                    throw new ArgumentOutOfRangeException("LogFileType");
            }
        }

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
            if (string.IsNullOrEmpty(ToolTipText.Trim()))
            { ToolTipText = Text; }
            else
            { ToolTipText = Text + System.Environment.NewLine + ToolTipText; }
        }

        public void WriteLine(string Text, string Sender)
        {
            if (!string.IsNullOrEmpty(Text.Trim())) Write(Sender + ": " + Text);
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
        /// <param name="SubItem"></param>
        /// <returns>-1 if the SubItem doesn't exist</returns>
        private int GetSubItemNumber(SubItem SubItem)
        {
            switch (SubItem)
            {
                case SubItem.SkippedBy:
                    if (Datestamped)
                        return 2;
                    else
                        return 1;

                case SubItem.SkippedReason:
                    if (Datestamped)
                        return 3;
                    else
                        return 2;

                case SubItem.TimeStamp:
                    if (Datestamped)
                        return 1;
                    else
                        return -1;

                default:
                    throw new ArgumentOutOfRangeException("SubItem");
            }
        }

        /// <summary>
        /// Returns the Text from a ListViewItem.SubItems object
        /// </summary>
        private string GetSubItemText(SubItem SubItem)
        {
            switch (SubItem)
            {
                case SubItem.SkippedBy:
                case SubItem.SkippedReason:
                    if (HaveSkipInfo)
                        return base.SubItems[GetSubItemNumber(SubItem)].Text;
                    else
                        return "";

                case SubItem.TimeStamp:
                    if (Datestamped)
                        return base.SubItems[1].Text;
                    else
                        return "";

                default:
                    return base.SubItems[GetSubItemNumber(SubItem)].Text;
            }
        }

        private void SetSubItemText(SubItem SubItem, string value)
        {
            if ((SubItem == SubItem.SkippedBy || SubItem == SubItem.SkippedReason) &! (HaveSkipInfo))
            {
                base.SubItems.Add("SkippedBy");
                base.SubItems.Add("SkipReason");
                HaveSkipInfo = true;
            
            }
            base.SubItems[GetSubItemNumber(SubItem)].Text = value;
        }

        protected void Skip(string mSkippedBy, string mSkipReason)
        {
            SetSubItemText(SubItem.SkippedBy, mSkippedBy);
            SetSubItemText(SubItem.SkippedReason, mSkipReason);
            WriteLine(SkipReason, SkippedBy);
            mSkipped = true;
        }

        // disable access to underlying Items property
        public static new System.Windows.Forms.ListViewItem.ListViewSubItemCollection SubItems
        {
            get { throw new NotImplementedException("The SubItems property should not be accessed directly"); }
        }
    }
}
