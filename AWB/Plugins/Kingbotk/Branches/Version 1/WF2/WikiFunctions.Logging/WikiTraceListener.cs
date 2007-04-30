namespace WikiFunctions.Logging
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Runtime.InteropServices;
    using WikiFunctions;
    using WikiFunctions.Logging.Uploader;

    public class WikiTraceListener : TraceListenerUploadableBase
    {
        protected const string mDateFormat = "[[d MMMM]] [[yyyy]] HH:mm ";

        public WikiTraceListener(UploadableLogSettings2 UploadSettings, TraceStatus TraceStatus) : base(UploadSettings, TraceStatus)
        {
            this.WriteBulletedLine("Logging: [[User:Kingbotk/Plugin/WikiFunctions2|WikiFunctions2]].dll v" + WikiFunctions2.Version.ToString(), false, false);
        }

        public override void ProcessingArticle(string ArticleFullTitle, Namespaces NS)
        {
            this.CheckCounterForUpload();
            base.WriteLine(TraceListenerBase.GetArticleTemplate(ArticleFullTitle, NS), false);
        }

        public override void SkippedArticle(string SkippedBy, string Reason)
        {
            if (Operators.CompareString(Reason, "", false) != 0)
            {
                Reason = ": " + Reason;
            }
            base.WriteLine("#*''" + SkippedBy + ": Skipped" + Reason + "''", false);
        }

        public override void SkippedArticleBadTag(string SkippedBy, string FullArticleTitle, Namespaces NS)
        {
            this.SkippedArticle(SkippedBy, "Bad tag");
        }

        public override void WriteArticleActionLine(string Line, string PluginName)
        {
            base.WriteLine("#*" + PluginName + ": " + Line.Replace("[[Category:", "[[:Category:"), false);
        }

        public override void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly, bool DateStamp)
        {
            if (!VerboseOnly || this.Verbose)
            {
                if (DateStamp)
                {
                    Line = DateTime.Now.ToString("[[d MMMM]] [[yyyy]] HH:mm ") + Line;
                }
                if (Bold)
                {
                    base.WriteLine("*'''" + Line + "'''", true);
                }
                else
                {
                    base.WriteLine("*" + Line, true);
                }
            }
        }

        public override void WriteComment(string Line)
        {
            base.Write("<!-- " + Line + " -->");
        }

        public override void WriteCommentAndNewLine(string Line)
        {
            base.WriteLine("<!-- " + Line + " -->", false);
        }

        public override void WriteLine(string value, [Optional, DefaultParameterValue(true)] bool CheckCounter)
        {
            base.WriteLine(value, CheckCounter);
        }

        public override void WriteTemplateAdded(string Template, string PluginName)
        {
            base.WriteLine(string.Format("#*{1}: [[Template:{0}|{0}]] added", Template, PluginName), false);
        }
    }
}

