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

// From WikiFunctions2.dll. Converted from VB to C#

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

        public WikiTraceListener(UploadableLogSettings2 UploadSettings, TraceStatus TraceStatus)
            : base(UploadSettings, TraceStatus)
        {
            WriteBulletedLine("Logging: WikiFunctions.dll v" + Tools.VersionString, false, false);
        }

        // Formatting:
        /// <summary>
        /// Return a datestamp for the current time
        /// </summary>
        protected virtual string DateStamp()
        {
            if (Variables.IsWikipediaEN)
                return WikiDateStamp();
            
            return NonWikiDateStamp();
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
        public override void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly, bool DateStamp)
        {
            if (VerboseOnly && !Verbose)
                return;

            if (DateStamp)
                Line = this.DateStamp() + Line;

            if (Bold)
                base.WriteLine("*'''" + Line + "'''", true);
            else
                base.WriteLine("*" + Line, true);
        }
        public override void ProcessingArticle(string FullArticleTitle, Namespaces NS)
        {
            CheckCounterForUpload(); // Check counter *before* starting a new article section
            base.WriteLine(GetArticleTemplate(FullArticleTitle, NS), false);
        }
        public override void SkippedArticle(string SkippedBy, string Reason)
        {
            if (!string.IsNullOrEmpty(Reason))
                Reason = ": " + Reason;
            base.WriteLine("#*''" + SkippedBy + ": Skipped" + Reason + "''", false);
        }
        public override void SkippedArticleBadTag(string SkippedBy, string FullArticleTitle, Namespaces NS)
        {
            SkippedArticle(SkippedBy, "Bad tag");
        }
        public override void WriteArticleActionLine(string Line, string PluginName)
        {
            base.WriteLine("#*" + PluginName + ": " + Line.Replace("[[Category:", "[[:Category:"), false);
        }
        public override void WriteTemplateAdded(string Template, string PluginName)
        {
            base.WriteLine(string.Format("#*{1}: [[Template:{0}|{0}]] added", Template, PluginName), false);
        }

        public override void WriteLine(string Line)
        {
            WriteLine(Line, true);
        }

        public override void WriteComment(string Line)
        {
            base.Write("<!-- " + Line + " -->");
        }

        public override void WriteCommentAndNewLine(string Line)
        {
            base.WriteLine("<!-- " + Line + " -->", false);
        }
    }

    /// <summary>
    /// This class logs in XHTML format
    /// </summary>
    public class XHTMLTraceListener : TraceListenerBase
    {

        protected static int mArticleCount = 1;
        protected static bool mVerbose;

        public XHTMLTraceListener(string filename, bool LogVerbose)
            : base(filename)
        {
            mVerbose = LogVerbose;

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
        public override void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly, bool DateStamp)
        {
            if (VerboseOnly && !mVerbose)
                return;
            
            if (DateStamp)
                Line = string.Format("{0:g}: {1}", System.DateTime.Now, Line);
            
            if (Bold)
                base.WriteLine("<br/><li><b>" + Line + "</b></li>");
            else
                base.WriteLine("<li>" + Line + "</li>");
        }
        public override void ProcessingArticle(string FullArticleTitle, Namespaces NS)
        {
            base.WriteLine("<br/>" + mArticleCount + ". <a href=\"" + Variables.NonPrettifiedURL(FullArticleTitle) + "\">[[" + FullArticleTitle + "]]</a>");
            mArticleCount += 1;
        }
        public override void SkippedArticle(string SkippedBy, string Reason)
        {
            if (!string.IsNullOrEmpty(Reason))
                Reason = ": " + Reason;
            base.WriteLine("<li><i>" + SkippedBy + ": Skipped" + Reason + "</i></li>");
        }

        public override void SkippedArticleBadTag(string SkippedBy, string FullArticleTitle, Namespaces NS)
        {
            SkippedArticle(SkippedBy, "Bad tag");
        }

        public override void WriteArticleActionLine(string Line, string PluginName)
        {
            base.WriteLine("<li><i>" + PluginName + ": " + Line + "</i></li>");
        }

        public override void WriteTemplateAdded(string Template, string PluginName)
        {
            base.WriteLine("<br/><li><i>" + PluginName + ": " + Template + "</i></li>");
        }

        public override void WriteComment(string Line)
        {
            base.Write("<!-- " + Line + " -->");
        }

        public override void WriteCommentAndNewLine(string Line)
        {
            base.WriteLine("<!-- " + Line + " -->");
        }

        public override bool Uploadable
        {
            get { return false; }
        }
    }

    // TODO: MySQLTraceListener
    //public class MySQLTraceListener : IMyTraceListener, ITraceStatusProvider
    //{
    //    public WikiFunctions.Logging.Uploader.TraceStatus TraceStatus
    //    {
    //        get { throw new Exception("The method or operation is not implemented."); }
    //    }

    //    #region IMyTraceListener Members
    //        public void Close()
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }

    //        public void Flush()
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }

    //        public void ProcessingArticle(string FullArticleTitle, Namespaces NS)
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }

    //        public void SkippedArticle(string SkippedBy, string Reason)
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }

    //        public void SkippedArticleBadTag(string SkippedBy, string FullArticleTitle, Namespaces NS)
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }

    //        public void SkippedArticleRedlink(string SkippedBy, string FullArticleTitle, Namespaces NS)
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }

    //        public void Write(string Text)
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }

    //        public void WriteArticleActionLine(string Line, string PluginName)
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }

    //        public void WriteArticleActionLine(string Line, string PluginName, bool VerboseOnly)
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }

    //        public void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly)
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }

    //        public void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly, bool DateStamp)
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }

    //        public void WriteComment(string Line)
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }

    //        public void WriteCommentAndNewLine(string Line)
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }

    //        public void WriteLine(string Line)
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }

    //        public void WriteTemplateAdded(string Template, string PluginName)
    //        {
    //            throw new Exception("The method or operation is not implemented.");
    //        }

    //        public bool Uploadable
    //        {
    //            get { return false; }
    //        } // Should take care of it's own upload management; as far as client is concerned it's a static log
    //    #endregion
    //}
}

