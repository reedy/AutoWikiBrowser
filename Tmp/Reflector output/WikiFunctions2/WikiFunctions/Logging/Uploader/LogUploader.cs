namespace WikiFunctions.Logging.Uploader
{
    using Microsoft.VisualBasic;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using WikiFunctions;
    using WikiFunctions.Logging;

    public class LogUploader : Editor
    {
        protected readonly string BotTag = "|}<!--/bottag-->";
        public const string conAddingLogEntryDefaultEditSummary = "Adding log entry";
        public const string conUploadingDefaultEditSummary = "Uploading log";
        protected const string NewCell = "\r\n|";
        protected const string NewLine = "\r\n";
        protected readonly string TableHeaderNoUserName = "! Job !! Category !! Page # !! Date";
        protected readonly string TableHeaderUserName = "! Job !! Category !! Page # !! Performed By !! Date";

        protected virtual void DoLogEntry(string LogTitle, string LogDetails, int PageNumber, DateTime StartDate, string UploadTo, string Location, bool UserNameCell, string EditSummary, [Optional, DefaultParameterValue("")] string Username)
        {
            string strExistingText = Editor.GetWikiText(Location);
            Application.DoEvents();
            string TableAddition = "|-\r\n|[[" + UploadTo + "|" + LogTitle + "]]\r\n|" + LogDetails + "\r\n|[[" + UploadTo + "|" + PageNumber.ToString() + "]]" + Interaction.IIf(UserNameCell, "\r\n|[[User:" + Username + "|" + Username + "]]", "").ToString() + "\r\n|" + string.Format("[[{0:d MMMM}]] [[{0:yyyy}]]", StartDate) + "\r\n" + this.BotTag;
            if (strExistingText.Contains(this.BotTag))
            {
                base.EditPage(Location, strExistingText.Replace(this.BotTag, TableAddition), EditSummary, false, true);
            }
            else
            {
                base.EditPageAppend(Location, "\r\n<!--bottag-->\r\n{| class=\"wikitable\" width=\"100%\"\r\n" + Interaction.IIf(UserNameCell, this.TableHeaderUserName, this.TableHeaderNoUserName).ToString() + "\r\n" + TableAddition, EditSummary, false);
            }
        }

        public virtual void LogIn(CookieCollection Cookies)
        {
            base.logincookies = Cookies;
        }

        public virtual void LogIn(UsernamePassword LoginDetails)
        {
            UsernamePassword VB$t_ref$L0 = LoginDetails;
            if (VB$t_ref$L0.HaveCookies)
            {
                this.LogIn(VB$t_ref$L0.Cookies);
            }
            else
            {
                if (!VB$t_ref$L0.IsSet)
                {
                    throw new SettingsPropertyNotFoundException("Login details not found");
                }
                VB$t_ref$L0.Cookies = this.LogIn(VB$t_ref$L0.Username, VB$t_ref$L0.Password);
            }
            VB$t_ref$L0 = null;
        }

        public virtual CookieCollection LogIn(string Username, string Password)
        {
            base.LogIn(Username, Password);
            return base.logincookies;
        }

        public virtual void LogIt(string Log, string LogTitle, string LogDetails, string UploadTo, string LogEntryLocation, int PageNumber, DateTime StartDate)
        {
            List<LogEntry> LinksToLog = new List<LogEntry>();
            LinksToLog.Add(new LogEntry(LogEntryLocation, false));
            this.LogIt(Log, LogTitle, LogDetails, UploadTo, LinksToLog, PageNumber, StartDate, false, false, "", "", true, "Uploading log", "Adding log entry");
        }

        public virtual void LogIt(string Log, string LogTitle, string LogDetails, string UploadTo, LogEntry LinkToLog, int PageNumber, DateTime StartDate, bool OpenInBrowser, bool AddToWatchlist, string Username, [Optional, DefaultParameterValue("")] string LogHeader)
        {
            List<LogEntry> LinksToLog = new List<LogEntry>();
            LinksToLog.Add(LinkToLog);
            this.LogIt(Log, LogTitle, LogDetails, UploadTo, LinksToLog, PageNumber, StartDate, OpenInBrowser, AddToWatchlist, Username, LogHeader, true, "Uploading log", "Adding log entry");
        }

        public virtual void LogIt(TraceListenerUploadableBase Sender, string LogTitle, string LogDetails, string UploadToWithoutPageNumber, List<LogEntry> LinksToLog, bool OpenInBrowser, bool AddToWatchlist, string Username, string LogHeader, string EditSummary, string LogSummaryEditSummary)
        {
            this.LogIt(Sender.TraceStatus.LogUpload, LogTitle, LogDetails, UploadToWithoutPageNumber + " " + Sender.TraceStatus.PageNumber.ToString(), LinksToLog, Sender.TraceStatus.PageNumber, Sender.TraceStatus.StartDate, OpenInBrowser, AddToWatchlist, Username, "{{log|name=" + UploadToWithoutPageNumber + "|page=" + Sender.TraceStatus.PageNumber.ToString() + "}}\r\n" + LogHeader, false, EditSummary, LogSummaryEditSummary);
        }

        public virtual void LogIt(string Log, string LogTitle, string LogDetails, string UploadTo, List<LogEntry> LinksToLog, int PageNumber, DateTime StartDate, bool OpenInBrowser, bool AddToWatchlist, string Username, string LogHeader, [Optional, DefaultParameterValue(true)] bool AddLogTemplate, [Optional, DefaultParameterValue("Uploading log")] string EditSummary, [Optional, DefaultParameterValue("Adding log entry")] string LogSummaryEditSummary)
        {
            string UploadToNoSpaces = UploadTo.Replace(" ", "_");
            string strLogText = "";
            if (AddLogTemplate)
            {
                strLogText = "{{log|name=" + UploadToNoSpaces + "|page=" + PageNumber.ToString() + "}}\r\n";
            }
            strLogText = strLogText + LogHeader + Log;
            Application.DoEvents();
            if (AddToWatchlist)
            {
                base.EditPage(UploadToNoSpaces, strLogText, EditSummary, false, true);
            }
            else
            {
                base.EditPage(UploadToNoSpaces, strLogText, EditSummary, false);
            }
            Application.DoEvents();
            foreach (LogEntry LogEntry in LinksToLog)
            {
                this.DoLogEntry(LogTitle, LogDetails, PageNumber, StartDate, UploadTo, LogEntry.Location, LogEntry.UserName, LogSummaryEditSummary, Username);
                Application.DoEvents();
            }
            if (OpenInBrowser)
            {
                this.OpenLogInBrowser(UploadTo);
            }
        }

        protected virtual void OpenLogInBrowser(string UploadTo)
        {
            Process.Start("http://en.wikipedia.org/wiki/" + UploadTo);
        }
    }
}

