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

using WikiFunctions.Logging.Uploader;

namespace WikiFunctions.Logging
{
    /// <summary>
    /// This class logs in wiki format
    /// </summary>
    public class WikiTraceListener : TraceListenerUploadableBase
    {
        protected static readonly System.Globalization.CultureInfo DateFormat = 
            new System.Globalization.CultureInfo("en-US", false); // override user's culture when writing to English Wikipedia; applied only as a formatter so won't affect localisation/UI

        public WikiTraceListener(UploadableLogSettings2 uploadSettings, TraceStatus traceStatus)
            : base(uploadSettings, traceStatus)
        {
            WriteBulletedLine("Logging: WikiFunctions.dll v" + Tools.VersionString, false, false);
        }

        // Formatting:
        /// <summary>
        /// Return a datestamp for the current time
        /// </summary>
        protected virtual string DateStamp()
        {
            return Variables.IsWikipediaEN ? WikiDateStamp() : NonWikiDateStamp();
        }

        /// <summary>
        /// Return a plain text datestamp, for non-EN wikis where we don't know the format for date articles/there aren't such articles
        /// </summary>
        protected virtual string NonWikiDateStamp()
        {
            return System.DateTime.Now.ToString("d MMMM yyyy HH:mm ");
        }

        /// <summary>
        /// Return a current date stamp in EN Wiki format.
        /// </summary>
        /// <remarks>Overrideable (virtual) so inherited classes can easily change format.</remarks>
        protected virtual string WikiDateStamp()
        {
            return System.DateTime.Now.ToString("[[d MMMM]] [[yyyy]] HH:mm ", DateFormat);
        }

        // Overrides:
        public override void WriteBulletedLine(string line, bool bold, bool verboseOnly, bool dateStamp)
        {
            if (verboseOnly && !Verbose)
                return;

            if (dateStamp)
                line = DateStamp() + line;

            if (bold)
                base.WriteLine("*'''" + line + "'''", true);
            else
                base.WriteLine("*" + line, true);
        }

        public override void ProcessingArticle(string fullArticleTitle, int ns)
        {
            CheckCounterForUpload(); // Check counter *before* starting a new article section
            base.WriteLine(GetArticleTemplate(fullArticleTitle, ns), false);
        }

        public override void SkippedArticle(string skippedBy, string reason)
        {
            if (!string.IsNullOrEmpty(reason))
                reason = ": " + reason;
            base.WriteLine("#*''" + skippedBy + ": Skipped" + reason + "''", false);
        }

        public override void SkippedArticleBadTag(string skippedBy, string fullArticleTitle, int ns)
        {
            SkippedArticle(skippedBy, "Bad tag");
        }

        public override void WriteArticleActionLine(string line, string pluginName)
        {
            base.WriteLine("#*" + pluginName + ": " + line.Replace("[[Category:", "[[:Category:"), false);
        }

        public override void WriteTemplateAdded(string template, string pluginName)
        {
            base.WriteLine(string.Format("#*{1}: [[Template:{0}|{0}]] added", template, pluginName), false);
        }

        public override void WriteLine(string line)
        {
            WriteLine(line, true);
        }

        public override void WriteComment(string line)
        {
            base.Write("<!-- " + line + " -->");
        }

        public override void WriteCommentAndNewLine(string line)
        {
            base.WriteLine("<!-- " + line + " -->", false);
        }
    }

    /// <summary>
    /// This class logs in XHTML format
    /// </summary>
    public class XHTMLTraceListener : TraceListenerBase
    {
        protected static int mArticleCount = 1;
        protected static bool mVerbose;

        public XHTMLTraceListener(string filename, bool logVerbose)
            : base(filename)
        {
            mVerbose = logVerbose;

            base.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" " + "\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            base.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" " + "lang=\"en\" dir=\"ltr\">");
            base.WriteLine("<head>");
            base.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            base.WriteLine("<meta name=\"generator\" content=\"WikiFunctions" + Tools.VersionString + "\" />");
            base.WriteLine("<title>AWB log</title>");
            base.WriteLine("</head><body>");
        }

        // Overrides:
        public override void Close()
        {
            base.WriteLine("</body>");
            base.WriteLine("</html>");
            base.Close();
        }

        public override bool Verbose
        {
            get { return mVerbose; }
        }

        public override void WriteBulletedLine(string line, bool bold, bool verboseOnly, bool dateStamp)
        {
            if (verboseOnly && !mVerbose)
                return;

            if (dateStamp)
                line = string.Format("{0:g}: {1}", System.DateTime.Now, line);

            if (bold)
                base.WriteLine("<br/><li><b>" + line + "</b></li>");
            else
                base.WriteLine("<li>" + line + "</li>");
        }

        public override void ProcessingArticle(string fullArticleTitle, int ns)
        {
            base.WriteLine("<br/>" + mArticleCount + ". <a href=\"" + Variables.NonPrettifiedURL(fullArticleTitle) + "\">[[" + fullArticleTitle + "]]</a>");
            mArticleCount += 1;
        }

        public override void SkippedArticle(string skippedBy, string reason)
        {
            if (!string.IsNullOrEmpty(reason))
                reason = ": " + reason;
            base.WriteLine("<li><i>" + skippedBy + ": Skipped" + reason + "</i></li>");
        }

        public override void SkippedArticleBadTag(string skippedBy, string fullArticleTitle, int ns)
        {
            SkippedArticle(skippedBy, "Bad tag");
        }

        public override void WriteArticleActionLine(string line, string pluginName)
        {
            base.WriteLine("<li><i>" + pluginName + ": " + line + "</i></li>");
        }

        public override void WriteTemplateAdded(string template, string pluginName)
        {
            base.WriteLine("<br/><li><i>" + pluginName + ": " + template + "</i></li>");
        }

        public override void WriteComment(string line)
        {
            base.Write("<!-- " + line + " -->");
        }

        public override void WriteCommentAndNewLine(string line)
        {
            base.WriteLine("<!-- " + line + " -->");
        }

        public override bool Uploadable
        {
            get { return false; }
        }
    }
}

