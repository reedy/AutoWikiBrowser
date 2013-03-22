/*
(C) 2008 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WikiFunctions.API;
using WikiFunctions.Plugin;

namespace WikiFunctions.Logging.Uploader
{
    public struct EditPageRetvals
    {
        public string Article;
        public string ResponseText;
        public string DiffLink;
    }

	/// <summary>
	/// A class which uploads logs to Wikipedia
	/// </summary>
	public class LogUploader
	{
		protected readonly string BotTag;
		protected readonly string TableHeaderUserName;
		protected readonly string TableHeaderNoUserName;
		protected const string NewCell = "\r\n|";

	    private readonly AsyncApiEdit editor;

        protected string UserAgent
        {
            get
            {
                return Tools.DefaultUserAgentString;
            }
        }

		public LogUploader(AsyncApiEdit e)
		{
			BotTag = "|}<!--/bottag-->"; // doing it this way OUGHT to allow inherited classes to override
			TableHeaderUserName = "! Job !! Category !! Page # !! Performed By !! Date";
			TableHeaderNoUserName = "! Job !! Category !! Page # !! Date";

		    editor = e.Clone();
		}

		public void LogIn(string username, string password)
		{
			editor.Login(username, password);
		}

		public virtual void LogIn(UsernamePassword loginDetails)
		{
			if (loginDetails.IsSet)
			{
				LogIn(loginDetails.Username, loginDetails.Password);
			}
			else
			{
				throw new System.Configuration.SettingsPropertyNotFoundException("Login details not found");
			}
		}

        /// <summary>
        /// Upload log to the wiki, and optionally add log entries to central log pages
        /// </summary>
        /// <param name="log">The log text</param>
        /// <param name="logTitle">The log title</param>
        /// <param name="logDetails">Details of the job being logged</param>
        /// <param name="uploadTo">Which page to write the log to</param>
        /// <param name="linksToLog">A collection of LogEntry objects detailing pages to list the log page on. Send an empty collection if not needed.</param>
        /// <param name="pageNumber">Log page number</param>
        /// <param name="startDate">When the job started</param>
        /// <param name="openInBrowser">True if the log page should be opened in the web browser after uploading, otherwise false</param>
        /// <param name="addToWatchlist">True if the log page should be added to the user's watchlist, otherwise false</param>
        /// <param name="username">The user id of the user performing the task</param>
        /// <param name="logHeader">Header text</param>
        /// <param name="addLogTemplate">True if a {{log}} template should be added</param>
        /// <param name="editSummary">The edit summary</param>
        /// <param name="logSummaryEditSummary">The edit summary when listing the log page on the LogEntry pages (if applicable)</param>
        /// <param name="sender"></param>
        /// <param name="addLogArticlesToAnAWBList">True if an IAutoWikiBrowser object is being sent and the AWB log tab should be written to</param>
        /// <param name="awb">An IAutoWikiBrowser object, may be null</param>
        public virtual List<EditPageRetvals> LogIt(string log, string logTitle, string logDetails, string uploadTo, 
            List<LogEntry> linksToLog, int pageNumber, DateTime startDate, bool openInBrowser, 
            bool addToWatchlist, string username, string logHeader, bool addLogTemplate, 
            string editSummary, string logSummaryEditSummary, string sender, bool addLogArticlesToAnAWBList,
            IAutoWikiBrowser awb)
		{
            List<EditPageRetvals> retval = new List<EditPageRetvals>();
            string uploadToNoSpaces = uploadTo.Replace(" ", "_");
            string strLogText = "";
            AWBLogListener awbLogListener = null;
            
            if (DoAWBLogListener(addLogArticlesToAnAWBList, awb))
                awbLogListener = new AWBLogListener(uploadTo);

            if (addLogTemplate)
            {
                strLogText = "{{log|name=" + uploadToNoSpaces + "|page=" + pageNumber + "}}" + Environment.NewLine;
            }
            strLogText += logHeader + log;

            Application.DoEvents();

            try
            {
                editor.Open(uploadToNoSpaces);
                editor.Wait();

                SaveInfo save = editor.SynchronousEditor.Save(strLogText, editSummary, false, WatchOptions.NoChange);

                retval.Add(new EditPageRetvals
                               {
                                   Article = uploadToNoSpaces,
                                   DiffLink = editor.URL + "index.php?oldid=" + save.NewId + "&diff=prev",
                                   ResponseText = save.ResponseXml.OuterXml
                               });
            }
            catch (Exception ex)
            {
                if (awbLogListener != null)
                    AWBLogListenerUploadFailed(ex, sender, awbLogListener, awb);
                throw; // throw error and exit
            }
       
            Application.DoEvents();

            foreach (LogEntry logEntry in linksToLog)
            {
                retval.Add(DoLogEntry(logEntry, logTitle, logDetails, pageNumber, startDate, uploadTo, logSummaryEditSummary,
                    username, addLogArticlesToAnAWBList, awb, sender));
                Application.DoEvents();
            }

            if (openInBrowser)
                OpenLogInBrowser(uploadTo);

            return retval;
        }

        protected virtual EditPageRetvals DoLogEntry(LogEntry logEntry, string logTitle, string logDetails, int pageNumber,
            DateTime startDate, string uploadTo, string editSummary, string username, 
            bool addLogArticlesToAnAWBList, IAutoWikiBrowser awb, string sender)
        {
            AWBLogListener awbLogListener = null;

            try
            {
                editor.Open(logEntry.Location);
                editor.Wait();

                string strExistingText = editor.Page.Text;

                if (DoAWBLogListener(addLogArticlesToAnAWBList, awb))
                {
                    if (string.IsNullOrEmpty(sender))
                        sender = "WikiFunctions DLL";
                    awbLogListener = new AWBLogListener(logEntry.Location);
                }

                Application.DoEvents();

                string tableAddition = "|-" + NewCell + "[[" + uploadTo + "|" + logTitle + "]]" + NewCell +
                    logDetails + NewCell + "[[" + uploadTo + "|" + pageNumber + "]]" +
                    (logEntry.LogUserName ? NewCell + "[[User:" + username + "|" + username + "]]" : "") +
                    NewCell + string.Format("[[{0:d MMMM}]] [[{0:yyyy}]]", startDate) +
                    Environment.NewLine + BotTag;

                SaveInfo save;

                if (strExistingText.Contains(BotTag))
                {
                    save = editor.SynchronousEditor.Save(strExistingText.Replace(BotTag, tableAddition), editSummary, false, WatchOptions.NoChange);
                }
                else
                {
                    save = editor.SynchronousEditor.Save(strExistingText + Environment.NewLine + "<!--bottag-->" +
                                Environment.NewLine + "{| class=\"wikitable\" width=\"100%\"" +
                                Environment.NewLine +
                                (logEntry.LogUserName ? TableHeaderUserName : TableHeaderNoUserName) +
                                Environment.NewLine + tableAddition, editSummary, false, WatchOptions.NoChange);
                }

                EditPageRetvals retval = new EditPageRetvals
                {
                    Article = logEntry.Location,
                    DiffLink = editor.URL + "index.php?oldid=" + save.NewId + "&diff=prev",
                    ResponseText = save.ResponseXml.OuterXml
                };

                try
                {
                    if (awbLogListener != null)
                    {
                        awbLogListener.WriteLine("Log entry uploaded", sender);
                        //AWB.AddLogItem(true, AWBLogListener);
                    }
                }
                catch { } // errors shouldn't happen here, but even if they do we want to avoid entering the outer catch block

                logEntry.Success=true;
                return retval;
            }
            catch (Exception ex)
            {
                if (awbLogListener != null)
                    AWBLogListenerUploadFailed(ex, sender, awbLogListener, awb);
                throw;
            }
        }

		protected virtual void OpenLogInBrowser(string uploadTo)
		{
            Tools.OpenArticleInBrowser(uploadTo);
		}

        private static bool DoAWBLogListener(bool doIt, IAutoWikiBrowser awb)
        { return (doIt && awb != null); }

        private void AWBLogListenerUploadFailed(Exception ex, string sender, AWBLogListener logListener,
            IAutoWikiBrowser AWB)
        {
            logListener.WriteLine("Error: " + ex.Message, sender);
            ((IMyTraceListener)logListener).SkippedArticle(sender, "Error");
            //AWB.AddLogItem(false, logListener);
        }
	}
}

