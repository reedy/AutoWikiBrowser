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

#region Namespaces
    namespace WikiFunctions
    { // This is needed by the Kingbotk plugin. It has to live here rather than in WikiFunctions2 to get a clean compile.
        public enum Namespaces
        {
            Category = 14,
            CategoryTalk = 15,
            Help = 12,
            HelpTalk = 13,
            Image = 6,
            ImageTalk = 7,
            Main = 0,
            Media = -2,
            Mediawiki = 8,
            MediawikiTalk = 9,
            Portal = 100,
            PortalTalk = 0x65,
            Project = 4,
            ProjectTalk = 5,
            Special = -1,
            Talk = 1,
            Template = 10,
            TemplateTalk = 11,
            User = 2,
            UserTalk = 3
        }
    }
#endregion

    namespace WikiFunctions.Logging
{
        public enum LogFileType
        {
            PlainText=1, WikiText, AnnotatedWikiText
        }

    /// <summary>
    /// This interface is implemented by all TraceListener objects
    /// </summary>
    public interface IMyTraceListener
    {
        // This interface was moved from WikiFunctions2.dll shipped with the Kingbotk plugin
        // Please don't alter it unless absolutely necessary

        // Methods
        void Close();
        void Flush();
        void ProcessingArticle(string FullArticleTitle, Namespaces NS);
        void SkippedArticle(string SkippedBy, string Reason);
        void SkippedArticleBadTag(string SkippedBy, string FullArticleTitle, Namespaces NS);
        void SkippedArticleRedlink(string SkippedBy, string FullArticleTitle, Namespaces NS);
        void Write(string Text);
        void WriteArticleActionLine(string Line, string PluginName);
        void WriteArticleActionLine(string Line, string PluginName, bool VerboseOnly);
        void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly);
        void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly, bool DateStamp);
        void WriteComment(string Line);
        void WriteCommentAndNewLine(string Line);
        void WriteLine(string Line);
        void WriteTemplateAdded(string Template, string PluginName);

        // Properties
        /// <summary>
        /// Is this trace listener an upload client?
        /// </summary>
        /// <returns><b>True</b> if the trace listener can upload to Wikipedia</returns>
        bool Uploadable { get; }
    }

    public class AWBLogListener : ListViewItem, IMyTraceListener
    {    /* This class will:
         * Use the Logging interface previously defined in wikifunctions2
         * Be written to by AWB during processing and passed to plugins
         * Format itself as a ListViewItem suitable (with subitems where appropriate) for adding to the skipped 
           or processed articles list
         * Handle MouseOver event (display log entry)
         * Handle double click event (open article in browser)
        */

        protected string mArticle; /* store this locally rather than relying on .Text in case a Plugin changes .Text
                                   * (it shouldn', and Kingbotk doesn't, but who knows :) */
        private bool mSkipped;

        #region AWB Interface
            public AWBLogListener(string ArticleTitle)
            {
                Text = ArticleTitle;
                mArticle = ArticleTitle;
            }

            public void UserSkipped()
            {
                Skip("User", "Clicked ignore");
            }

            public void AWBSkipped(string Reason)
            {
                Skip("AWB", Reason);
            }

            public void PluginSkipped()
            {
                Skip("Plugin", "Plugin sent skip event");
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
                ListViewSubItem DateStamp = new ListViewSubItem();
                DateStamp.Text = DateTime.Now.ToString();

                ListView.Items.Insert(0, this).SubItems.Insert(1, DateStamp);
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
                protected set { base.SubItems[3].Text = value; }
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
        #endregion

        private string GetSubItemText(int SubItem)
        {
            try
            {
                return base.SubItems[SubItem].Text;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error in logging object: " + ex.Message, "Error", MessageBoxButtons.OK);
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

        protected void WriteLine(string Text, string Sender)
        {
            if (Text.Trim() != "") Write(Sender + ": " + Text);
        }
 
        // disable access to underlying Items property to stop Reedy Boy accessing it ;)
        public new System.Windows.Forms.ListViewItem.ListViewSubItemCollection SubItems
        {
            get { throw new NotImplementedException("The SubItems property should not be accessed directly"); }
        }
    }
}


