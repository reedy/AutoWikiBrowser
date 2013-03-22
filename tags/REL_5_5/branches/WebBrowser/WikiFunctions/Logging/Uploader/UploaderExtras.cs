using System;
using System.Windows.Forms;

namespace WikiFunctions.Logging.Uploader
{
    /// <summary>
    /// Object which contains details of target pages for log entries
    /// </summary>
    public class LogEntry
    {
        public LogEntry(string pLocation, bool pUserName)
        {
            Location = pLocation;
            LogUserName = pUserName;
        }

        public string Location { get; private set; }
        public bool LogUserName { get; private set; }
        public bool Success { get; protected internal set; }
    }

    /// <summary>
    /// Stores the user's login details/cookies
    /// </summary>
    public class UsernamePassword
    {
        private string mUserName = "", mPassword = "";

        public virtual string Username
        { get { return mUserName; } set { mUserName = value; } }

        public virtual string Password
        { get { return mPassword; } set { mPassword = value; } }

        public virtual bool IsSet
        { get { return (!string.IsNullOrEmpty(Password) && !string.IsNullOrEmpty(Username)); } }

        public bool HaveCookies { get { return Cookies.Count > 0; } }

        public System.Net.CookieCollection Cookies { get; set; }
    }

    /// <summary>
    /// Stores the user's login Profile/cookies
    /// </summary>
    public sealed class UsernamePassword2 : UsernamePassword
    {
        private Profiles.AWBProfile Profile;

        public override string Password
        { get { return Profile.Password; } set { Profile.Password = value; } }

        public override string Username
        { get { return Profile.Username; } set { Profile.Username = value; } }

        public Profiles.AWBProfile AWBProfile
        { get { return Profile; } set { Profile = value; } }
    }

    /// <summary>
    /// A simple settings class for logging solutions
    /// </summary>
    public class UploadableLogSettings
    {
        protected bool mLogVerbose;
        protected string mLogFolder = Application.StartupPath;
        protected int mUploadMaxLines = 1000;
        protected bool mUploadYN;
        protected bool mUploadOpenInBrowser;

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
        protected bool mLogXHTML;
        protected bool mLogWiki;
        protected bool mUploadAddToWatchlist = true;
        protected bool mLogSQL;
        protected string mUploadLocation;
        protected string mUploadJobName;

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
        TraceStatus TraceStatus { get; }
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
        protected DateTime mDate = DateTime.Now;

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

        public virtual DateTime StartDate
        {
            get { return mDate; }
            set { mDate = value; }
        }
    }
}