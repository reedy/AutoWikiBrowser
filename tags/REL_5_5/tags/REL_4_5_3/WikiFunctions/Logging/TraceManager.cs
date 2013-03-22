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

// Originally from WikiFunctions2.dll (converted from VB to C#).

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
        protected Dictionary<string, IMyTraceListener> Listeners = new Dictionary<string, IMyTraceListener>();

		public virtual void AddListener(string Key, IMyTraceListener Listener)
		{
			// Override this if you want to programatically add an event handler
			Listeners.Add(Key, Listener);
		}
		public virtual void RemoveListener(string Key)
		{
			// Override this if you want to programatically remove an event handler
			Listeners[Key].Close();
			Listeners.Remove(Key);
		}
		public bool ContainsKey(string Key)
		{
			return Listeners.ContainsKey(Key);
		}
		public bool ContainsValue(IMyTraceListener Listener)
		{
			return Listeners.ContainsValue(Listener);
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
		public virtual void ProcessingArticle(string FullArticleTitle, Namespaces NS)
		{
			foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
			{
				t.Value.ProcessingArticle(FullArticleTitle, NS);
			}
		}
		public virtual void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly, bool DateStamp)
		{
			foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
			{
				t.Value.WriteBulletedLine(Line, Bold, VerboseOnly, DateStamp);
			}
		}
		public virtual void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly)
		{
			WriteBulletedLine(Line, Bold, VerboseOnly, false);
		}
		public virtual void SkippedArticle(string SkippedBy, string Reason)
		{
			foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
			{
				t.Value.SkippedArticle(SkippedBy, Reason);
			}
		}
		public virtual void SkippedArticleBadTag(string SkippedBy, string FullArticleTitle, Namespaces NS)
		{
			foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
			{
				t.Value.SkippedArticleBadTag(SkippedBy, FullArticleTitle, NS);
			}
		}

        public virtual void SkippedArticleRedlink(string SkippedBy, string FullArticleTitle, Namespaces NS)
        {
            foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
            {
                t.Value.SkippedArticleRedlink(SkippedBy, FullArticleTitle, NS);
            }
        }
        public virtual void WriteArticleActionLine(string Line, string PluginName)
        {
            foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
            {
                t.Value.WriteArticleActionLine(Line, PluginName);
            }
        }
        public virtual void WriteTemplateAdded(string Template, string PluginName)
        {
            foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
            {
                t.Value.WriteTemplateAdded(Template, PluginName);
            }
        }
        public virtual void WriteArticleActionLine(string Line, string PluginName, bool VerboseOnly)
        {
            WriteArticleActionLine1(Line, PluginName, VerboseOnly);
        }
        public virtual void WriteArticleActionLine1(string Line, string PluginName, bool VerboseOnly)
        {
            if (VerboseOnly)
            {
                foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
                {
                    t.Value.WriteArticleActionLine(Line, PluginName, true);
                }
            }
            else
            {
                foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
                {
                    t.Value.WriteArticleActionLine(Line, PluginName);
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
        public virtual void WriteComment(string Line)
        {
            foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
            {
                t.Value.WriteComment(Line);
            }
        }
        public virtual void WriteCommentAndNewLine(string Line)
        {
            foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
            {
                t.Value.WriteCommentAndNewLine(Line);
            }
        }
        public virtual void WriteLine(string Line)
        {
            foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
            {
                t.Value.WriteLine(Line);
            }
        }

        public struct UploadHandlerReturnVal
        {
            public bool Success;
            public List<Editor.EditPageRetvals> PageRetVals;
        }

        /// <summary>
        /// A fully featured upload-event handler
        /// </summary>
        protected virtual UploadHandlerReturnVal UploadHandler(TraceListenerUploadableBase Sender, string LogTitle, 
            string LogDetails, string UploadToWithoutPageNumber, List<LogEntry> LinksToLog, bool OpenInBrowser,
            bool AddToWatchlist, string Username, string LogHeader, string EditSummary,
            string LogSummaryEditSummary, Plugin.IAutoWikiBrowser AWB, 
            UsernamePassword LoginDetails)
        {
            UploadHandlerReturnVal retval = new UploadHandlerReturnVal();
            retval.Success = false;

            if (StartingUpload(Sender))
            {
                string pageName = UploadToWithoutPageNumber + " " + Sender.TraceStatus.PageNumber;
                UploadingPleaseWaitForm waitForm = new UploadingPleaseWaitForm();
                LogUploader uploader = new LogUploader();

                waitForm.Show();

                try
                {
                    uploader.LogIn(LoginDetails);
                    Application.DoEvents();

                    retval.PageRetVals = uploader.LogIt(Sender.TraceStatus.LogUpload, LogTitle, LogDetails, pageName, LinksToLog,
                        Sender.TraceStatus.PageNumber, Sender.TraceStatus.StartDate, OpenInBrowser,
                        AddToWatchlist, Username, "{{log|name=" + UploadToWithoutPageNumber + "|page=" +
                        Sender.TraceStatus.PageNumber + "}}" + Environment.NewLine + LogHeader,
                        false, EditSummary, LogSummaryEditSummary, ApplicationName, true, AWB);

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
                        Sender.WriteCommentAndNewLine("Log uploaded to " + pageName);
                    else
                        Sender.WriteCommentAndNewLine(
                           "LOG UPLOADING FAILED. Please manually upload this section to " + pageName);
                }

                waitForm.Dispose();
                FinishedUpload();
            }
            return retval;
        }

        public virtual void WriteUploadLog(List<Editor.EditPageRetvals> PageRetVals, string LogFolder)
        {
            try
            {
                System.IO.StreamWriter io =
                    new System.IO.StreamWriter(LogFolder + "\\Log uploading " +
                    DateTime.Now.Ticks + ".txt");

                foreach (Editor.EditPageRetvals editPageRetval in PageRetVals)
                {
                    io.WriteLine("***********************************************************************************");
                    io.WriteLine("Page: " + editPageRetval.article);
                    io.WriteLine("Diff link: " + editPageRetval.difflink);
                    io.WriteLine("Server response: ");
                    io.WriteLine(editPageRetval.responsetext);
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
        protected abstract bool StartingUpload(TraceListenerUploadableBase Sender);
        protected virtual void FinishedUpload() { }
    }
}

