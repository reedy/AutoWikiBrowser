using System;
using System.Collections.Generic;
using System.Text;
using WikiFunctions;
using WikiFunctions.Logging;
using WikiFunctions.Logging.Uploader;

namespace WikiFunctions.Logging
{
	/// <summary>
	/// This class logs in wiki format
	/// </summary>
	public class WikiTraceListener : TraceListenerUploadableBase
	{

		protected const string mDateFormat = "[[d MMMM]] [[yyyy]] HH:mm ";

        public WikiTraceListener(UploadableLogSettings2 UploadSettings, TraceStatus TraceStatus)
            : base(UploadSettings, TraceStatus)
		{
			WriteBulletedLine("Logging: [[User:Kingbotk/Plugin/WikiFunctions2|WikiFunctions2]].dll v" + Tools.Version.ToString(), false, false);
		}

		// Overrides:
		public override void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly, bool DateStamp)
		{
			if (VerboseOnly && ! Verbose)
			{
				return;
			}
			if (DateStamp)
			{
				Line = System.DateTime.Now.ToString(mDateFormat) + Line;
			}
			if (Bold)
			{
                base.WriteLine("*'''" + Line + "'''", true);
            }
                else base.WriteLine("*" + Line, true) ;
		}
		public override void ProcessingArticle(string ArticleFullTitle, Namespaces NS)
		{
			CheckCounterForUpload(); // Check counter *before* starting a new article section
			base.WriteLine(GetArticleTemplate(ArticleFullTitle, NS), false);
		}
		public override void SkippedArticle(string SkippedBy, string Reason)
		{
			if (! (Reason == ""))
			{
				Reason = ": " + Reason;
			}
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

		public override void WriteLine(string value)
		{
			WriteLine(value, true);
		}

//INSTANT C# NOTE: C# does not support optional parameters. Overloaded method(s) are created above.
//ORIGINAL LINE: Public Overrides Sub WriteLine(ByVal value As String, Optional ByVal CheckCounter As Boolean = true)
		public override void WriteLine(string value, bool CheckCounter)
		{
			base.WriteLine(value, CheckCounter);
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

		public XHTMLTraceListener(string filename, bool LogVerbose) : base(filename)
		{
			mVerbose = LogVerbose;

			base.WriteLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" " + "\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
			base.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" " + "lang=\"en\" dir=\"ltr\">");
			base.WriteLine("<head>");
			base.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
			base.WriteLine("<meta name=\"generator\" content=\"SDK Software WikiFunctions2" + Tools.Version.ToString() + "\" />");
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
			get
			{
				return mVerbose;
			}
		}
		public override void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly, bool DateStamp)
		{
			if (VerboseOnly && ! mVerbose)
			{
				return;
			}
			if (DateStamp)
			{
				Line = string.Format("{0:g}: {1}", System.DateTime.Now, Line);
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
		public override void ProcessingArticle(string ArticleFullTitle, Namespaces NS)
		{
			base.WriteLine("<br/>" + mArticleCount.ToString() + ". <a href=\"" + WikiFunctions.Variables.GetPageURL(ArticleFullTitle) + "\">[[" + ArticleFullTitle + "]]</a>");
			mArticleCount += 1;
		}
		public override void SkippedArticle(string SkippedBy, string Reason)
		{
			if (! (Reason == ""))
			{
				Reason = ": " + Reason;
			}
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
		public override void WriteLine(string value)
		{
			base.WriteLine(value);
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
			get
			{
				return false;
			}
		}
	}
}
