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
    public interface IMyTraceListener
    {
        // Methods
        void Close();
        void Flush();
        void ProcessingArticle(string FullArticleTitle, Namespaces NS);
        void SkippedArticle(string SkippedBy, string Reason);
        void SkippedArticleBadTag(string SkippedBy, string FullArticleTitle, Namespaces NS);
        void SkippedArticleRedlink(string FullArticleTitle, Namespaces NS);
        void Write(string Text);
        void WriteArticleActionLine(string Line, string PluginName);
        void WriteArticleActionLine(string Line, string PluginName, bool VerboseOnly);
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
         * Use the Logging interface previously defined in wikifunctions2 from the Kingbotk plugin
         * Be written to by AWB during processing and passed to plugins
         * Format itself as a ListViewItem suitable (with subitems where appropriate) for adding to the skipped or processed articles list
         * Handle MouseOver event (display log entry)
         * Handle double click event (open article in browser)
        */

        private bool mSkipped; private string mLogText; private string mSkippedBy;

        public bool Skipped
        {
            get { return mSkipped; }
        }

        public AWBLogListener(string ArticleTitle)
        {

        }

        #region IMyTraceListener Members

        void IMyTraceListener.Close() { }
        void IMyTraceListener.Flush() { }
        void IMyTraceListener.ProcessingArticle(string FullArticleTitle, Namespaces NS) { }

        void IMyTraceListener.SkippedArticle(string SkippedBy, string Reason)
        {
            mSkipped = true;
            mSkippedBy = Reason;
            mLogText = "";// SkippedBy + ": " + Reason + newline + mLogText;
        }

        void IMyTraceListener.SkippedArticleBadTag(string SkippedBy, string FullArticleTitle, Namespaces NS)
        {
            
        }

        void IMyTraceListener.SkippedArticleRedlink(string FullArticleTitle, Namespaces NS)
        {
            
        }

        bool IMyTraceListener.Uploadable
        {
            get { return false; }
        }

        void IMyTraceListener.Write(string Text)
        {
            
        }

        void IMyTraceListener.WriteArticleActionLine(string Line, string PluginName, bool VerboseOnly)
        {
            
        }

        void IMyTraceListener.WriteArticleActionLine(string Line, string PluginName)
        {
            
        }

        void IMyTraceListener.WriteBulletedLine(string Line, bool Bold, bool VerboseOnly, bool DateStamp)
        {
            
        }

        void IMyTraceListener.WriteComment(string Line)
        {
            
        }

        void IMyTraceListener.WriteCommentAndNewLine(string Line)
        {
            
        }

        void IMyTraceListener.WriteLine(string Line)
        {
            
        }

        void IMyTraceListener.WriteTemplateAdded(string Template, string PluginName)
        {
            
        }

        void createListViewItem(string ArticleTitle, string SkippedBy, string SkipReason)
        {
            ListViewItem item = new ListViewItem(ArticleTitle);

                item.SubItems.Add(SkippedBy);
                item.SubItems.Add(SkipReason);
        }

        #endregion
    }
}

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

