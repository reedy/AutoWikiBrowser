namespace WikiFunctions.Logging
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using WikiFunctions;

    public class XHTMLTraceListener : TraceListenerBase
    {
        protected static int mArticleCount = 1;
        protected static bool mVerbose;

        public XHTMLTraceListener(string filename, bool LogVerbose) : base(filename)
        {
            mVerbose = LogVerbose;
            base.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            base.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" lang=\"en\" dir=\"ltr\">");
            base.WriteLine("<head>");
            base.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            base.WriteLine("<meta name=\"generator\" content=\"SDK Software WikiFunctions2" + WikiFunctions2.Version.ToString() + "\" />");
            base.WriteLine("<title>AWB log</title>");
            base.WriteLine("</head><body>");
        }

        public override void Close()
        {
            base.WriteLine("</body>");
            base.WriteLine("</html>");
            base.Close();
        }

        public override void ProcessingArticle(string ArticleFullTitle, Namespaces NS)
        {
            base.WriteLine("<br/>" + mArticleCount.ToString() + ". <a href=\"" + TraceListenerBase.GetURL(ArticleFullTitle) + "\">[[" + ArticleFullTitle + "]]</a>");
            mArticleCount++;
        }

        public override void SkippedArticle(string SkippedBy, string Reason)
        {
            if (Operators.CompareString(Reason, "", false) != 0)
            {
                Reason = ": " + Reason;
            }
            base.WriteLine("<li><i>" + SkippedBy + ": Skipped" + Reason + "</i></li>");
        }

        public override void SkippedArticleBadTag(string SkippedBy, string FullArticleTitle, Namespaces NS)
        {
            this.SkippedArticle(SkippedBy, "Bad tag");
        }

        public override void WriteArticleActionLine(string Line, string PluginName)
        {
            base.WriteLine("<li><i>" + PluginName + ": " + Line + "</i></li>");
        }

        public override void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly, bool DateStamp)
        {
            if (!VerboseOnly || mVerbose)
            {
                if (DateStamp)
                {
                    Line = string.Format("{0:g}: {1}", DateTime.Now, Line);
                }
                if (Bold)
                {
                    base.WriteLine("<br/><li><b>" + Line + "</b></li>");
                }
                else
                {
                    base.WriteLine("<li>" + Line + "</li>");
                }
            }
        }

        public override void WriteComment(string Line)
        {
            base.Write("<!-- " + Line + " -->");
        }

        public override void WriteCommentAndNewLine(string Line)
        {
            base.WriteLine("<!-- " + Line + " -->");
        }

        public override void WriteLine(string value)
        {
            base.WriteLine(value);
        }

        public override void WriteTemplateAdded(string Template, string PluginName)
        {
            base.WriteLine("<br/><li><i>" + PluginName + ": " + Template + "</i></li>");
        }

        public override bool Uploadable
        {
            get
            {
                return false;
            }
        }

        public override bool Verbose
        {
            get
            {
                return mVerbose;
            }
        }
    }
}

