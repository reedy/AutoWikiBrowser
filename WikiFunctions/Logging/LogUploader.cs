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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using WikiFunctions.Plugin;

namespace WikiFunctions.Logging.Uploader
{
	/// <summary>
	/// Object which contains details of target pages for log entries
	/// </summary>
	public class LogEntry
	{
		private string mLocation;
		private bool mUserName;
        private bool mSuccess = false;

		public LogEntry(string pLocation, bool pUserName)
		{
			mLocation = pLocation;
			mUserName = pUserName;
		}

        public string Location { get { return mLocation; } }
        public bool LogUserName { get { return mUserName; } }
        public bool Success { get { return mSuccess; } protected internal set { mSuccess = value; } }
	}

	/// <summary>
	/// Stores the user's login details/cookies
	/// </summary>
	public class UsernamePassword
	{
        private string mUserName = "", mPassword = "";
        private bool mHaveCookies;
        private System.Net.CookieCollection mCookies;

		public virtual string Username
        { get { return mUserName; } set { mUserName = value; } }

        public virtual string Password
        { get { return mPassword; } set { mPassword = value; } }

        public virtual bool IsSet
        { get { return (Password != "" && Username != ""); } }

        public bool HaveCookies
        { get { return mHaveCookies; } set { mHaveCookies = value; } }

        public System.Net.CookieCollection Cookies
        { get { return mCookies; } set { mCookies = value; } }
	}

    /// <summary>
    /// Stores the user's login Profile/cookies
    /// </summary>
    public sealed class UsernamePassword2 : UsernamePassword
    {
        private AWBProfiles.AWBProfile mAWBProfile;

        public override string  Password
        { get { return mAWBProfile.Password; } set { mAWBProfile.Password = value; } }

        public override string  Username
        { get { return mAWBProfile.Username; } set { mAWBProfile.Username = value; } }

        public AWBProfiles.AWBProfile AWBProfile
        { get { return mAWBProfile; } set { mAWBProfile = value; } }
    }

	/// <summary>
	/// A simple settings class for logging solutions
	/// </summary>
	public class UploadableLogSettings
	{
		protected bool mLogVerbose = true;
		protected string mLogFolder = System.Windows.Forms.Application.StartupPath;
		protected int mUploadMaxLines = 1000;
		protected bool mUploadYN;
		protected bool mUploadOpenInBrowser = false;

        public virtual bool LogVerbose
        {
            get { return mLogVerbose; }
            set { mLogVerbose = value; }
        }
        public virtual int UploadMaxLines
        {
            get { return mUploadMaxLines; }
            set { mUploadMaxLines = value; }
        }
        public virtual bool UploadYN
        {
            get { return mUploadYN; }
            set { mUploadYN = value; }
        }
        public virtual bool UploadOpenInBrowser
        {
            get { return mUploadOpenInBrowser; }
            set { mUploadOpenInBrowser = value; }
        }
        public virtual string LogFolder
        {
            get { return mLogFolder; }
            set { mLogFolder = value; }
        }
	}

        	/// <summary>
	/// An extended base class with extra properties for a comprehensive logging solution
	/// </summary>
    public class UploadableLogSettings2 : UploadableLogSettings
    {
        protected bool mLogXHTML = false;
        protected bool mLogWiki = true;
        protected bool mUploadAddToWatchlist = true;
        protected bool mLogSQL = false;
        protected string mUploadLocation = "";
        protected string mUploadJobName = "";

        public virtual bool LogXHTML
        {
            get { return mLogXHTML; }
            set { mLogXHTML = value; }
        }
        public virtual bool LogWiki
        {
            get { return mLogWiki; }
            set { mLogWiki = value; }
        }
        public virtual bool LogSQL
        {
            get { return mLogSQL; }
            set { mLogSQL = value; }
        }
        public virtual bool UploadAddToWatchlist
        {
            get { return mUploadAddToWatchlist; }
            set { mUploadAddToWatchlist = value; }
        }
        public virtual string UploadLocation
        {
            get { return mUploadLocation; }
            set { mUploadLocation = value; }
        }
        public virtual string UploadJobName
        {
            get { return mUploadJobName; }
            set { mUploadJobName = value; }
        }
    }

	/// <summary>
	/// Implemented by classes which expose a TraceStatus object
	/// </summary>
	public interface ITraceStatusProvider
	{
		TraceStatus TraceStatus {get;}
	}

        /// <summary>
	/// A class which keeps track of statistics and not-yet-uploaded log entries
	/// </summary>
    public class TraceStatus
    {
        protected int mLinesWritten;
        protected int mLinesWrittenSinceLastUpload;
        protected int mNextPageNumber = 1;
        protected string mFileName;
        protected string mLogUpload;
        protected string mLogName;
        protected System.DateTime mDate = System.DateTime.Now;
        public TraceStatus(string FileNameIs, string LogNameIs)
        {
            FileName = FileNameIs;
            LogName = LogNameIs;
        }
        public virtual void Close()
        {
        }
        public virtual int LinesWritten // overridden in plugin, but calls mybase
        {
            get { return mLinesWritten; }
            set
            {
                mLinesWritten = value;
                LinesWrittenSinceLastUpload += 1;
            }
        }
        public virtual int LinesWrittenSinceLastUpload // overridden in plugin, but calls mybase
        {
            get { return mLinesWrittenSinceLastUpload; }
            set { mLinesWrittenSinceLastUpload = value; }
        }
        public virtual int PageNumber
        {
            get { return mNextPageNumber; }
            set { mNextPageNumber = value; }
        }
        public virtual string FileName
        {
            get { return mFileName; }
            set { mFileName = value; }
        }
        public virtual string LogUpload
        {
            get { return mLogUpload; }
            set { mLogUpload = value; }
        }
        public virtual string LogName
        {
            get { return mLogName; }
            set { mLogName = value; }
        }
        public virtual System.DateTime StartDate
        {
            get { return mDate; }
            set { mDate = value; }
        }
    }

        	/// <summary>
	/// A class which uploads logs to Wikipedia
	/// </summary>
	public class LogUploader : WikiFunctions.Editor
	{
		protected readonly string BotTag;
		protected readonly string TableHeaderUserName;
		protected readonly string TableHeaderNoUserName;
		protected const string NewLine = "\r\n";
		protected const string NewCell = "\r\n|";
		public const string conAddingLogEntryDefaultEditSummary = "Adding log entry";
		public const string conUploadingDefaultEditSummary = "Uploading log";

		public LogUploader() : base()
		{
			BotTag = "|}<!--/bottag-->"; // doing it this way OUGHT to allow inherited classes to override
			TableHeaderUserName = "! Job !! Category !! Page # !! Performed By !! Date";
			TableHeaderNoUserName = "! Job !! Category !! Page # !! Date";
		}
		public virtual new System.Net.CookieCollection LogIn(string Username, string Password)
		{
			base.LogIn(Username, Password);
			return logincookies;
		}
		public virtual void LogIn(System.Net.CookieCollection Cookies)
		{
			logincookies = Cookies;
		}
		public virtual void LogIn(UsernamePassword LoginDetails)
		{
			if (LoginDetails.HaveCookies)
			{
				LogIn(LoginDetails.Cookies);
			}
			else if (LoginDetails.IsSet)
			{
				LoginDetails.Cookies = LogIn(LoginDetails.Username, LoginDetails.Password);
			}
			else
			{
				throw new System.Configuration.SettingsPropertyNotFoundException("Login details not found");
			}
		}

        /// <summary>
        /// Upload log to the wiki, and optionally add log entries to central log pages
        /// </summary>
        /// <param name="Log">The log text</param>
        /// <param name="LogTitle">The log title</param>
        /// <param name="LogDetails">Details of the job being logged</param>
        /// <param name="UploadTo">Which page to write the log to</param>
        /// <param name="LinksToLog">A collection of LogEntry objects detailing pages to list the log page on. Send an empty collection if not needed.</param>
        /// <param name="PageNumber">Log page number</param>
        /// <param name="StartDate">When the job started</param>
        /// <param name="OpenInBrowser">True if the log page should be opened in the web browser after uploading, otherwise false</param>
        /// <param name="AddToWatchlist">True if the log page should be added to the user's watchlist, otherwise false</param>
        /// <param name="Username">The user id of the user performing the task</param>
        /// <param name="LogHeader">Header text</param>
        /// <param name="AddLogTemplate">True if a {{log}} template should be added</param>
        /// <param name="EditSummary">The edit summary</param>
        /// <param name="LogSummaryEditSummary">The edit summary when listing the log page on the LogEntry pages (if applicable)</param>
        /// <param name="AddLogArticlesToAnAWBList">True if an IAutoWikiBrowser object is being sent and the AWB log tab should be written to</param>
        /// <param name="AWB">An IAutoWikiBrowser object, may be null</param>
		public virtual void LogIt(string Log, string LogTitle, string LogDetails, string UploadTo, 
            List<LogEntry> LinksToLog, int PageNumber, System.DateTime StartDate, bool OpenInBrowser, 
            bool AddToWatchlist, string Username, string LogHeader, bool AddLogTemplate, 
            string EditSummary, string LogSummaryEditSummary, string sender, bool AddLogArticlesToAnAWBList,
            IAutoWikiBrowser AWB)
		{
            string UploadToNoSpaces = UploadTo.Replace(" ", "_");
            string strLogText = "";
            AWBLogListener AWBLogListener = null;
            
            if (DoAWBLogListener(AddLogArticlesToAnAWBList, AWB))
                AWBLogListener = new AWBLogListener(UploadTo);

            if (AddLogTemplate)
            {
                strLogText = "{{log|name=" + UploadToNoSpaces + "|page=" + PageNumber.ToString() 
                    + "}}" + NewLine;
            }
            strLogText += LogHeader + Log;

            Application.DoEvents();

            try
            {
                if (AddToWatchlist)
                {
                    base.EditPage(UploadToNoSpaces, strLogText, EditSummary, false, true);
                }
                else
                {
                    base.EditPage(UploadToNoSpaces, strLogText, EditSummary, false);
                }
            }
            catch (Exception ex)
            {
                if (AWBLogListener != null)
                    AWBLogListenerUploadFailed(ex, sender, AWBLogListener, AWB);
                throw ex; // throw error and exit
            }

            //HACK
            //if (AWBLogListener != null)
            //{
            //    AWBLogListener.WriteLine("Log uploaded", sender);
            //}
            //AWB.AddLogItem(Article);
            //AWB.AddLogItem(Article);
        
            Application.DoEvents();

            foreach (LogEntry LogEntry in LinksToLog)
            {
                DoLogEntry(LogEntry, LogTitle, LogDetails, PageNumber, StartDate, UploadTo, LogSummaryEditSummary,
                    Username, AddLogArticlesToAnAWBList, AWB, sender);
                Application.DoEvents();
            }

            if (OpenInBrowser)
            {
                OpenLogInBrowser(UploadTo);
            }
        }

        #region LogIt() overloads
        // this messy block is because VB has "optional" parameters, and C# doesn't.
        public virtual void LogIt(string Log, string LogTitle, string LogDetails, string UploadTo,
            List<LogEntry> LinksToLog, int PageNumber, System.DateTime StartDate, bool OpenInBrowser,
            bool AddToWatchlist, string Username, string LogHeader, bool AddLogTemplate, string EditSummary)
        {
            LogIt(Log, LogTitle, LogDetails, UploadTo, LinksToLog, PageNumber, StartDate, OpenInBrowser,
                AddToWatchlist, Username, LogHeader, AddLogTemplate, EditSummary,
                conAddingLogEntryDefaultEditSummary, "", false, null);
        }

        public virtual void LogIt(string Log, string LogTitle, string LogDetails, string UploadTo,
            List<LogEntry> LinksToLog, int PageNumber, System.DateTime StartDate, bool OpenInBrowser,
            bool AddToWatchlist, string Username, string LogHeader, bool AddLogTemplate)
        {
            LogIt(Log, LogTitle, LogDetails, UploadTo, LinksToLog, PageNumber, StartDate, OpenInBrowser,
                AddToWatchlist, Username, LogHeader, AddLogTemplate, conUploadingDefaultEditSummary,
                conAddingLogEntryDefaultEditSummary, "", false, null);
        }

        public virtual void LogIt(string Log, string LogTitle, string LogDetails, string UploadTo,
            List<LogEntry> LinksToLog, int PageNumber, System.DateTime StartDate, bool OpenInBrowser,
            bool AddToWatchlist, string Username, string LogHeader)
        {
            LogIt(Log, LogTitle, LogDetails, UploadTo, LinksToLog, PageNumber, StartDate, OpenInBrowser, 
                AddToWatchlist, Username, LogHeader, true, conUploadingDefaultEditSummary, 
                conAddingLogEntryDefaultEditSummary, "", false, null);
        }

		public virtual void LogIt(string Log, string LogTitle, string LogDetails, string UploadTo, 
            LogEntry LinkToLog, int PageNumber, System.DateTime StartDate, bool OpenInBrowser, 
            bool AddToWatchlist, string Username)
		{
			LogIt(Log, LogTitle, LogDetails, UploadTo, LinkToLog, PageNumber, StartDate, OpenInBrowser, 
                AddToWatchlist, Username, "");
		}

        public virtual void LogIt(string Log, string LogTitle, string LogDetails, string UploadTo, 
            LogEntry LinkToLog, int PageNumber, System.DateTime StartDate, bool OpenInBrowser, 
            bool AddToWatchlist, string Username, string LogHeader)
		{
			List<LogEntry> LinksToLog = new List<LogEntry>();
			LinksToLog.Add(LinkToLog);

			LogIt(Log, LogTitle, LogDetails, UploadTo, LinksToLog, PageNumber, StartDate, OpenInBrowser, 
                AddToWatchlist, Username, LogHeader);
		}
		public virtual void LogIt(string Log, string LogTitle, string LogDetails, string UploadTo, 
            string LogEntryLocation, int PageNumber, System.DateTime StartDate)
		{
			List<LogEntry> LinksToLog = new List<LogEntry>();
			LinksToLog.Add(new LogEntry(LogEntryLocation, false));

			LogIt(Log, LogTitle, LogDetails, UploadTo, LinksToLog, PageNumber, StartDate, false, false, "", "");
        }
        #endregion

        protected virtual void DoLogEntry(LogEntry LogEntry, string LogTitle, string LogDetails, int PageNumber,
            System.DateTime StartDate, string UploadTo, string EditSummary, string Username, 
            bool AddLogArticlesToAnAWBList, IAutoWikiBrowser AWB, string sender)
        {
            LogEntry.Success = DoLogEntry(LogTitle, LogDetails, PageNumber, StartDate, UploadTo, LogEntry.Location, 
                LogEntry.LogUserName, EditSummary, Username, AddLogArticlesToAnAWBList, AWB, sender);
        }

        protected virtual bool DoLogEntry(string LogTitle, string LogDetails, int PageNumber, 
            System.DateTime StartDate, string UploadTo, string Location, bool UserNameCell,
            string EditSummary, string Username, bool AddLogArticlesToAnAWBList,
            IAutoWikiBrowser AWB, string sender)
		{
            AWBLogListener AWBLogListener = null;

            try
            {
                string strExistingText = WikiFunctions.Editor.GetWikiText(Location);

                if (DoAWBLogListener(AddLogArticlesToAnAWBList, AWB))
                {
                    if (sender == "")
                        sender = "WikiFunctions DLL";
                    AWBLogListener = new AWBLogListener(Location);
                }

                Application.DoEvents();

                string TableAddition = "|-" + NewCell + "[[" + UploadTo + "|" + LogTitle + "]]" + NewCell +
                    LogDetails + NewCell + "[[" + UploadTo + "|" + PageNumber.ToString() + "]]" +
                    (UserNameCell ? NewCell + "[[User:" + Username + "|" + Username + "]]" : "").ToString() +
                    NewCell + string.Format("[[{0:d MMMM}]] [[{0:yyyy}]]", StartDate) +
                    System.Environment.NewLine + BotTag;

                if (strExistingText.Contains(BotTag))
                {
                    base.EditPage(Location, strExistingText.Replace(BotTag, TableAddition), EditSummary, false, true);
                }
                else
                {
                    base.EditPageAppend(Location, System.Environment.NewLine + "<!--bottag-->" +
                        System.Environment.NewLine + "{| class=\"wikitable\" width=\"100%\"" +
                        System.Environment.NewLine +
                        (UserNameCell ? TableHeaderUserName : TableHeaderNoUserName).ToString() +
                        System.Environment.NewLine + TableAddition, EditSummary, false);
                }
                try
                {
                    if (AWBLogListener != null)
                    {
                        AWBLogListener.WriteLine("Log entry uploaded", sender);
                        //AWB.AddLogItem(true, AWBLogListener);
                    }
                }
                catch { } // errors shouldn't happen here, but even if they do we want to avoid entering the outer catch block

                return true;
            }
            catch (Exception ex)
            {
                if (AWBLogListener != null)
                    AWBLogListenerUploadFailed(ex, sender, AWBLogListener, AWB);
                throw ex;
            }
		}
		protected virtual void OpenLogInBrowser(string UploadTo)
		{
            Tools.OpenArticleInBrowser(UploadTo);
		}
        private bool DoAWBLogListener(bool DoIt, IAutoWikiBrowser AWB)
        { return (DoIt && AWB != null); }

        private void AWBLogListenerUploadFailed(Exception ex, string sender, AWBLogListener AWBLogListener,
            IAutoWikiBrowser AWB)
        {
            AWBLogListener.WriteLine("Error: " + ex.Message, sender);
            ((IMyTraceListener)AWBLogListener).SkippedArticle(sender, "Error");
            //AWB.AddLogItem(false, AWBLogListener);
        }
	}
}

