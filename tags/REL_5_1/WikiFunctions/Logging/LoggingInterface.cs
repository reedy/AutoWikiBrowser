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
        /// <summary>
        /// 
        /// </summary>
        void Close();

        /// <summary>
        /// 
        /// </summary>
        void Flush();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullArticleTitle"></param>
        /// <param name="ns"></param>
        void ProcessingArticle(string fullArticleTitle, int ns);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skippedBy"></param>
        /// <param name="reason"></param>
        void SkippedArticle(string skippedBy, string reason);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skippedBy"></param>
        /// <param name="fullArticleTitle"></param>
        /// <param name="ns"></param>
        void SkippedArticleBadTag(string skippedBy, string fullArticleTitle, int ns);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skippedBy"></param>
        /// <param name="fullArticleTitle"></param>
        /// <param name="ns"></param>
        void SkippedArticleRedlink(string skippedBy, string fullArticleTitle, int ns);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        void Write(string text);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="pluginName"></param>
        void WriteArticleActionLine(string line, string pluginName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="pluginName"></param>
        /// <param name="verboseOnly"></param>
        void WriteArticleActionLine(string line, string pluginName, bool verboseOnly);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="bold"></param>
        /// <param name="verboseOnly"></param>
        void WriteBulletedLine(string line, bool bold, bool verboseOnly);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="bold"></param>
        /// <param name="verboseOnly"></param>
        /// <param name="dateStamp"></param>
        void WriteBulletedLine(string line, bool bold, bool verboseOnly, bool dateStamp);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        void WriteComment(string line);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        void WriteCommentAndNewLine(string line);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        void WriteLine(string line);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="pluginName"></param>
        void WriteTemplateAdded(string template, string pluginName);

        // Properties
        /// <summary>
        /// Is this trace listener an upload client?
        /// </summary>
        /// <returns>&lt;b>True&lt;/b> if the trace listener can upload to Wikipedia</returns>
        bool Uploadable { get; }
    }

    public interface IAWBTraceListener : IMyTraceListener
    {
        void AWBSkipped(string reason);
        void UserSkipped();
        void PluginSkipped();
    }
}
