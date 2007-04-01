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
    {
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
        bool Uploadable { get; }
    }

    public class AWBLogListener : ListViewItem, IMyTraceListener
    {
        /*
        ' We need listitem subitems for, only when skipped):
        ' Skipped by
        ' Skip reason
         */

        /* This class will:
         * Use the Logging interface previously defined in wikifunctions2
         * Be written to by AWB during processing and passed to plugins
         * Format itself as a ListViewItem suitable (with subitems where appropriate) for adding to the skipped or processed articles list
         * Handle MouseOver event (display log entry)
         * Handle double click event (open article in browser)
        */

        private bool mSkipped;

        #region AWB Interface
            public AWBLogListener(string ArticleTitle)
            {
                Text = ArticleTitle;
            }

            public bool Skipped
            { // strictly speaking we don't need this, as AWB knows if skipped or not. It does enhance encapsulation though.
                get { return mSkipped; }
            }

            public void UserSkipped()
            {
                Skip("User", "Clicked ignore");
            }

            public void AWBSKipped(string Reason)
            {
                Skip("AWB", Reason);
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
                
            }

            void IMyTraceListener.WriteArticleActionLine(string Line, string PluginName)
            {
                
            }

            void IMyTraceListener.WriteBulletedLine(string Line, bool Bold, bool VerboseOnly)
            {
                
            }

            void IMyTraceListener.WriteBulletedLine(string Line, bool Bold, bool VerboseOnly, bool DateStamp)
            {
                
            }

            void IMyTraceListener.WriteLine(string Line)
            {
                
            }

            void IMyTraceListener.WriteTemplateAdded(string Template, string PluginName)
            {
                
            }

            public void Write(string Text)
            {
                ToolTipText = Text + System.Environment.NewLine + ToolTipText;
            }
        #endregion

        private void Skip(string SkippedBy, string SkipReason)
        {
            mSkipped = true;
            SubItems.Add(SkippedBy);
            SubItems.Add(SkipReason);
            Write(SkippedBy + ": " + SkipReason);
        }

    }
}


