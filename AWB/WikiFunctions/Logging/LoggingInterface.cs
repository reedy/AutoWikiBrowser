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
        void ProcessingArticle(string fullArticleTitle, Namespaces ns);
        void SkippedArticle(string skippedBy, string reason);
        void SkippedArticleBadTag(string skippedBy, string fullArticleTitle, Namespaces ns);
        void SkippedArticleRedlink(string skippedBy, string fullArticleTitle, Namespaces ns);
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
