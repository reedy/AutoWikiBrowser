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

using System;
using System.Collections.Generic;
using WikiFunctions.Logging.Uploader;
using System.Windows.Forms;

namespace WikiFunctions.Logging
{
	/// <summary>
	/// An inheritable implementation of a Logging manager, built around a generic collection of IMyTraceListener objects and String keys
	/// </summary>
    public abstract class TraceManager : IMyTraceListener
	{
		// Listeners:
        protected readonly Dictionary<string, IMyTraceListener> Listeners = new Dictionary<string, IMyTraceListener>();

        /// <summary>
        /// Override this if you want to programatically add an event handler
        /// </summary>
        /// <param name="key"></param>
        /// <param name="listener"></param>
		public virtual void AddListener(string key, IMyTraceListener listener)
		{
			Listeners.Add(key, listener);
		}

        /// <summary>
        /// Override this if you want to programatically remove an event handler
        /// </summary>
        /// <param name="key">Key to remove</param>
		public virtual void RemoveListener(string key)
		{
			Listeners[key].Close();
			Listeners.Remove(key);
		}

	    protected bool TryGetValue(string key, out IMyTraceListener listener)
		{
			return Listeners.TryGetValue(key, out listener);
		}

        public bool ContainsKey(string key)
        {
            return Listeners.ContainsKey(key);
        }

		public bool ContainsValue(IMyTraceListener listener)
		{
			return Listeners.ContainsValue(listener);
		}

		// IMyTraceListener:
		public virtual void Close()
		{
			foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
			{
				t.Value.Close();
			}
		}

		public virtual void Flush()
		{
			foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
			{
				t.Value.Flush();
			}
		}

        public virtual void ProcessingArticle(string fullArticleTitle, int ns)
		{
			foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
			{
				t.Value.ProcessingArticle(fullArticleTitle, ns);
			}
		}

		public virtual void WriteBulletedLine(string line, bool bold, bool verboseOnly, bool dateStamp)
		{
			foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
			{
				t.Value.WriteBulletedLine(line, bold, verboseOnly, dateStamp);
			}
		}

		public virtual void WriteBulletedLine(string line, bool bold, bool verboseOnly)
		{
			WriteBulletedLine(line, bold, verboseOnly, false);
		}

		public virtual void SkippedArticle(string skippedBy, string reason)
		{
			foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
			{
				t.Value.SkippedArticle(skippedBy, reason);
			}
		}

        public virtual void SkippedArticleBadTag(string skippedBy, string fullArticleTitle, int ns)
		{
			foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
			{
				t.Value.SkippedArticleBadTag(skippedBy, fullArticleTitle, ns);
			}
		}

        public virtual void SkippedArticleRedlink(string skippedBy, string fullArticleTitle, int ns)
        {
            foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
            {
                t.Value.SkippedArticleRedlink(skippedBy, fullArticleTitle, ns);
            }
        }

        public virtual void WriteArticleActionLine(string line, string pluginName)
        {
            foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
            {
                t.Value.WriteArticleActionLine(line, pluginName);
            }
        }

        public virtual void WriteTemplateAdded(string template, string pluginName)
        {
            foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
            {
                t.Value.WriteTemplateAdded(template, pluginName);
            }
        }

        public virtual void WriteArticleActionLine(string line, string pluginName, bool verboseOnly)
        {
            WriteArticleActionLine1(line, pluginName, verboseOnly);
        }

        public virtual void WriteArticleActionLine1(string line, string pluginName, bool verboseOnly)
        {
            if (verboseOnly)
            {
                foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
                {
                    t.Value.WriteArticleActionLine(line, pluginName, true);
                }
            }
            else
            {
                foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
                {
                    t.Value.WriteArticleActionLine(line, pluginName);
                }
            }
        }

        public virtual bool Uploadable
        {
            get
            {
                foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
                {
                    if (t.Value.Uploadable) return true;
                }
                return false;
            }
        }
        public virtual void Write(string text)
        {
            foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
            {
                t.Value.Write(text);
            }
        }

        public virtual void WriteComment(string line)
        {
            foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
            {
                t.Value.WriteComment(line);
            }
        }

        public virtual void WriteCommentAndNewLine(string line)
        {
            foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
            {
                t.Value.WriteCommentAndNewLine(line);
            }
        }

        public virtual void WriteLine(string line)
        {
            foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
            {
                t.Value.WriteLine(line);
            }
        }

        public struct UploadHandlerReturnVal
        {
            public bool Success;
            public List<EditPageRetvals> PageRetVals;
        }

        /// <summary>
        /// A fully featured upload-event handler
        /// </summary>
        protected virtual UploadHandlerReturnVal UploadHandler(TraceListenerUploadableBase sender, string logTitle, 
            string logDetails, string uploadToWithoutPageNumber, List<LogEntry> linksToLog, bool openInBrowser,
            bool addToWatchlist, string username, string logHeader, string editSummary,
            string logSummaryEditSummary, Plugin.IAutoWikiBrowser awb, 
            UsernamePassword loginDetails)
        {
            UploadHandlerReturnVal retval = new UploadHandlerReturnVal {Success = false};

            if (StartingUpload(sender))
            {
                string pageName = uploadToWithoutPageNumber + " " + sender.TraceStatus.PageNumber;
                UploadingPleaseWaitForm waitForm = new UploadingPleaseWaitForm();
                LogUploader uploader = new LogUploader(awb.TheSession.Editor);

                waitForm.Show();

                try
                {
                    uploader.LogIn(loginDetails);
                    Application.DoEvents();

                    retval.PageRetVals = uploader.LogIt(sender.TraceStatus.LogUpload, logTitle, logDetails, pageName, linksToLog,
                        sender.TraceStatus.PageNumber, sender.TraceStatus.StartDate, openInBrowser,
                        addToWatchlist, username, "{{log|name=" + uploadToWithoutPageNumber + "|page=" +
                        sender.TraceStatus.PageNumber + "}}" + Environment.NewLine + logHeader,
                        false, editSummary, logSummaryEditSummary, ApplicationName, true, awb);

                    retval.Success = true;
                }
                catch (Exception ex)
                {
                    ErrorHandler.Handle(ex);

                    retval.Success = false;
                }
                finally
                {
                    if (retval.Success)                       
                        sender.WriteCommentAndNewLine("Log uploaded to " + pageName);
                    else
                        sender.WriteCommentAndNewLine(
                           "LOG UPLOADING FAILED. Please manually upload this section to " + pageName);
                }

                waitForm.Dispose();
                FinishedUpload();
            }
            return retval;
        }

        public virtual void WriteUploadLog(List<EditPageRetvals> pageRetVals, string logFolder)
        {
            try
            {
                System.IO.StreamWriter io =
                    new System.IO.StreamWriter(logFolder + "\\Log uploading " +
                    DateTime.Now.Ticks + ".txt");

                foreach (EditPageRetvals editPageRetval in pageRetVals)
                {
                    io.WriteLine("***********************************************************************************");
                    io.WriteLine("Page: " + editPageRetval.Article);
                    io.WriteLine("Diff link: " + editPageRetval.DiffLink);
                    io.WriteLine("Server response: ");
                    io.WriteLine(editPageRetval.ResponseText);
                    io.WriteLine();
                    io.WriteLine();
                    io.WriteLine();
                    io.WriteLine();
                }

                io.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating upload log: " + ex.Message, "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        protected abstract string ApplicationName { get; }
        protected abstract bool StartingUpload(TraceListenerUploadableBase sender);
        protected virtual void FinishedUpload() { }
    }
}

