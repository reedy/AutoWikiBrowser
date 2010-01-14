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

namespace WikiFunctions.Logging
{
    public enum LogFileType
    {
        PlainText = 1, WikiText, AnnotatedWikiText
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
        void ProcessingArticle(string fullArticleTitle, int ns);
        void SkippedArticle(string skippedBy, string reason);
        void SkippedArticleBadTag(string skippedBy, string fullArticleTitle, int ns);
        void SkippedArticleRedlink(string skippedBy, string fullArticleTitle, int ns);
        void Write(string text);
        void WriteArticleActionLine(string line, string pluginName);
        void WriteArticleActionLine(string line, string pluginName, bool verboseOnly);
        void WriteBulletedLine(string line, bool bold, bool verboseOnly);
        void WriteBulletedLine(string line, bool bold, bool verboseOnly, bool dateStamp);
        void WriteComment(string line);
        void WriteCommentAndNewLine(string line);
        void WriteLine(string line);
        void WriteTemplateAdded(string template, string pluginName);

        // Properties
        /// <summary>
        /// Is this trace listener an upload client?
        /// </summary>
        /// <returns><b>True</b> if the trace listener can upload to Wikipedia</returns>
        bool Uploadable { get; }
    }

    public interface IAWBTraceListener : IMyTraceListener
    {
        void AWBSkipped(string reason);
        void UserSkipped();
        void PluginSkipped();
    }
}
