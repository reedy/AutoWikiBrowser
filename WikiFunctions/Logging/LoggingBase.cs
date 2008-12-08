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

using System.Text.RegularExpressions;
using WikiFunctions.Logging.Uploader;

namespace WikiFunctions.Logging
{
    /// <summary>
    /// This abstract class can be used to build trace listener classes, or you can build a class from scratch and implement IMyTraceListener
    /// </summary>
    public abstract class TraceListenerBase : System.IO.StreamWriter, WikiFunctions.Logging.IMyTraceListener
    {
		// Initialisation
        private static Regex GetArticleTemplateRegex = new Regex("( talk)?:", RegexOptions.Compiled);

        protected TraceListenerBase(string filename)
            : base(filename, false, System.Text.Encoding.UTF8)
        {
        }

        #region IMyTraceListener interface
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
#endregion

		// Protected and public members:
		public static string GetArticleTemplate(string ArticleFullTitle, Namespaces NS)
		{
			int namesp = 0;
			string strnamespace = null;
			string templ = null;

            switch (NS)
            {
                case Namespaces.Main:
					return "#{{subst:la|" + ArticleFullTitle + "}}";

                case Namespaces.Talk:
					return "#{{subst:lat|" + Tools.RemoveNamespaceString(ArticleFullTitle).Trim() + "}}";

                default:
					namesp = (int)NS;
                    strnamespace = GetArticleTemplateRegex.Replace(Variables.Namespaces[(int) NS], "");

					if (namesp % 2 == 1) // talk
					{
						templ = "lnt";
					}
					else // not talk
					{
						templ = "ln";
					}

					return "#{{subst:" + templ + "|" + strnamespace + "|" + 
                        Tools.RemoveNamespaceString(ArticleFullTitle).Trim() + "}}";
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
        protected TraceListenerUploadableBase(UploadableLogSettings2 UploadSettings, TraceStatus TraceStatus)
            : base(TraceStatus.FileName)
		{
			mTraceStatus = TraceStatus;
			mUploadSettings = UploadSettings;
		}

		// Overrides & Shadowing:
		public override bool Uploadable
		{
            get { return true; }
		}
		public override bool Verbose
		{
            get { return mUploadSettings.LogVerbose; }
		}

		public virtual new void WriteLine(string Line)
		{
			WriteLine(Line, true);
		}

		public virtual void WriteLine(string Line, bool CheckCounter)
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
		public void Close(bool Upload)
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

		public virtual bool UploadLog(bool NewJob)
		{
            bool retval = false;
			if (Upload != null)	Upload(this, ref retval);

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
			return retval;
		}

		// Properties:
		public TraceStatus TraceStatus
		{
            get { return mTraceStatus; }
		}
		public UploadableLogSettings2 UploadSettings
		{
            get { return mUploadSettings; }
		}
		public virtual string PageName
		{
            get { return string.Format("{0:ddMMyy} {1}", mTraceStatus.StartDate, mUploadSettings.UploadJobName); }
		}
	}
}
