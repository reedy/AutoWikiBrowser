using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using WikiFunctions;
using WikiFunctions.Logging;

using Logging.Uploader;

namespace Logging
{
    /// <summary>
    /// This abstract class can be used to build trace listener classes, or you can build a class from scratch and implement IMyTraceListener
    /// </summary>
    public abstract class TraceListenerBase : System.IO.StreamWriter, WikiFunctions.Logging.IMyTraceListener
    {

		// Initialisation
//INSTANT C# NOTE: These were formerly VB static local variables:
private static Regex GetArticleTemplate_reg = new Regex("( talk)?:");

		public TraceListenerBase(string filename) : base(filename, false, System.Text.Encoding.UTF8)
		{
		}

#region IMyTraceListener interface
		public override void Close()
		{
			base.Close();
		}
		public override void Flush()
		{
			base.Flush();
		}
		public abstract void ProcessingArticle(string FullArticleTitle, Namespaces NS);
		public abstract void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly, bool DateStamp);
		public void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly)
		{
			WriteBulletedLine(Line, Bold, VerboseOnly, false);
		}
		public abstract void SkippedArticle(string SkippedBy, string Reason);
		public abstract void SkippedArticleBadTag(string SkippedBy, string FullArticleTitle, Namespaces NS);
		public abstract void WriteArticleActionLine(string Line, string PluginName);
		public abstract void WriteTemplateAdded(string Template, string PluginName);
		public abstract void WriteComment(string Line);
		public abstract void WriteCommentAndNewLine(string Line);
		public virtual void SkippedArticleRedlink(string SkippedBy, string FullArticleTitle, Namespaces NS)
		{
			SkippedArticle(SkippedBy, "Attached article doesn't exist - maybe deleted?");
		}
		public void WriteArticleActionLine(string Line, string PluginName, bool VerboseOnly)
		{
			WriteArticleActionLineVerbose(Line, PluginName, VerboseOnly);
		}
		public void WriteArticleActionLineVerbose(string Line, string PluginName, bool VerboseOnly)
		{
			if (VerboseOnly && ! Verbose)
			{
				return;
			}
			WriteArticleActionLine(Line, PluginName);
		}
		public abstract bool Uploadable {get;}
		public override void Write(string value)
		{
			base.Write(value);
		}
		public override void WriteLine(string value)
		{
			base.WriteLine(value);
		}
#endregion

		// Protected and public members:
		public static string GetArticleTemplate(string ArticleFullTitle, Namespaces NS)
		{
			int namesp = 0;
			string strnamespace = null;
			string templ = null;
//INSTANT C# NOTE: VB local static variable moved to class level
//			Static reg As new Regex("( talk)?:")

//INSTANT C# NOTE: The following VB 'Select Case' included range-type or non-constant 'Case' expressions and was converted to C# 'if-else' logic:
//			Select Case NS
//ORIGINAL LINE: Case Namespaces.Main
			if (NS == Namespaces.Main)
			{
					return "#{{subst:la|" + ArticleFullTitle + "}}";
			}
//ORIGINAL LINE: Case Namespaces.Talk
			else if (NS == Namespaces.Talk)
			{
					return "#{{subst:lat|" + WikiFunctions.Tools.RemoveNamespaceString(ArticleFullTitle).Trim() + "}}";
			}
//ORIGINAL LINE: Case Else
			else
			{
					namesp = (int)NS;
                    strnamespace = GetArticleTemplate_reg.Replace(WikiFunctions.Variables.Namespaces.Item(NS), "");

					if (namesp % 2 == 1) // talk
					{
						templ = "lnt";
					}
					else // not talk
					{
						templ = "ln";
					}

					return "#{{subst:" + templ + "|" + strnamespace + "|" + WikiFunctions.Tools.RemoveNamespaceString(ArticleFullTitle).Trim() + "}}";
			}
		}
		public abstract bool Verbose {get;}
	}

	/// <summary>
	/// An abstract class for building auto-uploading trace listeners
	/// </summary>
    public abstract class TraceListenerUploadableBase : TraceListenerBase, ITraceStatusProvider
	{

        protected TraceStatus mTraceStatus;
        protected UploadableLogSettings2 mUploadSettings;

		public delegate void UploadEventHandler(TraceListenerUploadableBase Sender, ref bool Success);
		public event UploadEventHandler Upload;

		// Initialisation:
        public TraceListenerUploadableBase(UploadableLogSettings2 UploadSettings, TraceStatus TraceStatus)
            : base(TraceStatus.FileName)
		{
			mTraceStatus = TraceStatus;
			mUploadSettings = UploadSettings;
		}

		// Overrides & Shadowing:
		public override bool Uploadable
		{
			get
			{
				return true;
			}
		}
		public override bool Verbose
		{
			get
			{
				return mUploadSettings.LogVerbose;
			}
		}

		public virtual new void WriteLine(string Line)
		{
			WriteLine(Line, true);
		}

//INSTANT C# NOTE: C# does not support optional parameters. Overloaded method(s) are created above.
//ORIGINAL LINE: Public Overridable Shadows Sub WriteLine(ByVal Line As String, Optional ByVal CheckCounter As Boolean = true)
		public virtual new void WriteLine(string Line, bool CheckCounter)
		{
			base.WriteLine(Line);

			mTraceStatus.LogUpload += Line + System.Environment.NewLine;
			mTraceStatus.LinesWritten += 1;

			if (CheckCounter)
			{
				CheckCounterForUpload();
			}
		}
		protected virtual bool IsReadyToUpload
		{
			get
			{
				return mTraceStatus.LinesWrittenSinceLastUpload >= mUploadSettings.UploadMaxLines;
			}
		}
		public virtual void CheckCounterForUpload()
		{
			if (IsReadyToUpload)
			{
				UploadLog();
			}
		}
		public new void Close(bool Upload)
		{
			if (Upload)
			{
				UploadLog();
			}
			mTraceStatus.Close();
			base.Close();
		}

		// Methods:

		public virtual bool UploadLog()
		{
			return UploadLog(false);
		}

//INSTANT C# NOTE: C# does not support optional parameters. Overloaded method(s) are created above.
//ORIGINAL LINE: Public Overridable Function UploadLog(Optional ByVal NewJob As Boolean = false) As Boolean
		public virtual bool UploadLog(bool NewJob)
		{
			if (Upload != null)
				Upload(this, UploadLog);
			// TODO: Logging: Get result, reset TraceStatus, write success/failure to log
			if (NewJob)
			{
				mTraceStatus.PageNumber = 1;
				mTraceStatus.StartDate = System.DateTime.Now;
			}
			else
			{
				mTraceStatus.PageNumber += 1;
				mTraceStatus.LinesWrittenSinceLastUpload = 0;
				mTraceStatus.LogUpload = "";
			}
			//INSTANT C# NOTE: Inserted the following 'return' since all code paths must return a value in C#:
			return false;
		}

		// Properties:
		public Uploader.TraceStatus TraceStatus
		{
			get
			{
				return mTraceStatus;
			}
		}
		public UploadableLogSettings2 UploadSettings
		{
			get
			{
				return mUploadSettings;
			}
		}
		public virtual string PageName
		{
			get
			{
				return string.Format("{0:ddMMyy} {1}", mTraceStatus.StartDate, mUploadSettings.UploadJobName);
			}
		}
	}
}
