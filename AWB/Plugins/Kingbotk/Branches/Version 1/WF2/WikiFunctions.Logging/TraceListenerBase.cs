namespace WikiFunctions.Logging
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Threading;
    using WikiFunctions;

    public abstract class TraceListenerBase : StreamWriter, IMyTraceListener
    {
        private static Regex $STATIC$GetArticleTemplate$02EE1131$reg;
        private static StaticLocalInitFlag $STATIC$GetArticleTemplate$02EE1131$reg$Init = new StaticLocalInitFlag();

        public TraceListenerBase(string filename) : base(filename, false, Encoding.UTF8)
        {
        }

        public override void Close()
        {
            base.Close();
        }

        public override void Flush()
        {
            base.Flush();
        }

        public static string GetArticleTemplate(string ArticleFullTitle, Namespaces NS)
        {
            string templ;
            Monitor.Enter($STATIC$GetArticleTemplate$02EE1131$reg$Init);
            try
            {
                if ($STATIC$GetArticleTemplate$02EE1131$reg$Init.State == 0)
                {
                    $STATIC$GetArticleTemplate$02EE1131$reg$Init.State = 2;
                    $STATIC$GetArticleTemplate$02EE1131$reg = new Regex("( talk)?:");
                }
                else if ($STATIC$GetArticleTemplate$02EE1131$reg$Init.State == 2)
                {
                    throw new IncompleteInitialization();
                }
            }
            finally
            {
                $STATIC$GetArticleTemplate$02EE1131$reg$Init.State = 1;
                Monitor.Exit($STATIC$GetArticleTemplate$02EE1131$reg$Init);
            }
            switch (NS)
            {
                case Namespaces.Main:
                    return ("#{{subst:la|" + ArticleFullTitle + "}}");

                case Namespaces.Talk:
                    return ("#{{subst:lat|" + Tools.RemoveNamespaceString(ArticleFullTitle).Trim() + "}}");
            }
            int namesp = (int) NS;
            string strnamespace = $STATIC$GetArticleTemplate$02EE1131$reg.Replace(Variables.Namespaces[(int) NS], "");
            if ((namesp % 2) == 1)
            {
                templ = "lnt";
            }
            else
            {
                templ = "ln";
            }
            return ("#{{subst:" + templ + "|" + strnamespace + "|" + Tools.RemoveNamespaceString(ArticleFullTitle).Trim() + "}}");
        }

        public abstract void ProcessingArticle(string FullArticleTitle, Namespaces NS);
        public abstract void SkippedArticle(string SkippedBy, string Reason);
        public abstract void SkippedArticleBadTag(string SkippedBy, string FullArticleTitle, Namespaces NS);
        public virtual void SkippedArticleRedlink(string SkippedBy, string FullArticleTitle, Namespaces NS)
        {
            this.SkippedArticle(SkippedBy, "Attached article doesn't exist - maybe deleted?");
        }

        public override void Write(string value)
        {
            base.Write(value);
        }

        public abstract void WriteArticleActionLine(string Line, string PluginName);
        public void WriteArticleActionLineVerbose(string Line, string PluginName, bool VerboseOnly)
        {
            if (!VerboseOnly || this.Verbose)
            {
                this.WriteArticleActionLine(Line, PluginName);
            }
        }

        public void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly)
        {
            this.WriteBulletedLine(Line, Bold, VerboseOnly, false);
        }

        public abstract void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly, bool DateStamp);
        public abstract void WriteComment(string Line);
        public abstract void WriteCommentAndNewLine(string Line);
        public override void WriteLine(string value)
        {
            base.WriteLine(value);
        }

        public abstract void WriteTemplateAdded(string Template, string PluginName);

        public abstract bool Uploadable { get; }

        public abstract bool Verbose { get; }

        public abstract bool WikiFunctions.Logging.IMyTraceListener.Uploadable { get; }
    }
}

