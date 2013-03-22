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

// From the Kingbotk plugin. Converted from VB to C#

using System;
using System.Collections.Generic;
using WikiFunctions;
using WikiFunctions.Logging;
using System.Windows.Forms;
using WikiFunctions.Logging.Uploader;

namespace AutoWikiBrowser.Logging
{
    /// <summary>
    /// Logging manager
    /// </summary>
    internal sealed class MyTrace : TraceManager, IAWBTraceListener
    {
        //private new Dictionary<string, IAWBTraceListener> Listeners = new Dictionary<string, IAWBTraceListener>();

        private LoggingSettings LoggingSettings;
        private static string LogFolder = "";
        private int BusyCounter;
        private bool mIsUploading;
        private bool mIsGettingPassword;
        private bool mStoppedWithConfigError;

        private const string ConWiki = "Wiki";
        private const string ConXhtml = "XHTML";

        // The most important stuff:
        internal void Initialise()
        {
            try
            {
                LoggingSettings.Initialised = true;
                LogFolder = LoggingSettings.Settings.LogFolder;

                if (LoggingSettings.Settings.LogXHTML || LoggingSettings.Settings.LogWiki)
                {
                    if (!System.IO.Directory.Exists(LoggingSettings.Settings.LogFolder))
                    {
                        LogFolder = Application.StartupPath;
                    }

                    if (LoggingSettings.Settings.LogXHTML)
                    {
                        NewXhtmlTraceListener();
                    }
                    if (LoggingSettings.Settings.LogWiki)
                    {
                        NewWikiTraceListener();
                    }
                }
                foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
                {
                    if(t.Key != "AWB")
                        t.Value.WriteBulletedLine(AWBLogListener.LoggingStartButtonClicked, true, false, true);
                }
                ValidateUploadProfile();
            }
            catch (Exception ex)
            {
                ConfigError(ex);
            }
        }

        internal void ConfigError(Exception ex)
        {
            MessageBox.Show(ex.Message);
            mStoppedWithConfigError = true;
            Program.AWB.Stop(ApplicationName);
        }

        private void TraceUploadEventHandler(TraceListenerUploadableBase sender, ref bool success)
        {
            ValidateUploadProfile();
            UploadHandlerReturnVal retval = UploadHandler(sender, LoggingSettings.Settings.LogTitle, 
                LoggingSettings.Settings.WikifiedCategory, LoggingSettings.Settings.GlobbedUploadLocation + "/" + 
                sender.PageName.Replace(LoggingSettings.Props.ConUploadCategoryIsJobName, 
                LoggingSettings.Settings.Category), LoggingSettings.Settings.LinksToLog(), 
                LoggingSettings.Settings.UploadOpenInBrowser, LoggingSettings.Settings.UploadAddToWatchlist, 
                LoggingSettings.Settings.UserName, Variables.AWBVersionString(Program.AWB.AWBVersionString) +
                Plugins.Plugin.GetPluginsWikiTextBlock(), AWBLogListener.AWBLoggingEditSummary +
                AWBLogListener.UploadingLogDefaultEditSummary, AWBLogListener.AWBLoggingEditSummary +
                AWBLogListener.UploadingLogEntryDefaultEditSummary, Program.AWB, LoggingSettings.LoginDetails);
            success = retval.Success;

            if (success)
                ((TraceStatus)sender.TraceStatus).UploadsCount += 1;

            if (LoggingSettings.Settings.DebugUploading)
                WriteUploadLog(retval.PageRetVals, LogFolder);
        }

        protected override bool StartingUpload(TraceListenerUploadableBase sender)
        {
            if (sender.TraceStatus.LogName != ConWiki)
                return false;

            mIsUploading = true;
            LoggingSettings.LedColour = WikiFunctions.Controls.Colour.Blue;
            Application.DoEvents();
            return true;
        }

        protected override void FinishedUpload()
        {
            //if (BusyCounter == 0)
                LoggingSettings.LedColour = WikiFunctions.Controls.Colour.Red;
            //else
            //    LoggingSettings.LEDColour = WikiFunctions.Controls.Colour.Green;

            mIsUploading = false;
        }

        // State:
        internal bool HaveOpenFile
        {
            get
            {
                foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
                {
                    if (t.Key != "AWB") return true; // we're interested in open files, not AWBLogListeners...
                }
                return false;
            }
        }

        /// <summary>
        /// Returns if Logging is currently uploading
        /// </summary>
        internal bool IsUploading
        { get { return mIsUploading; } }

        /// <summary>
        /// Returns if Logging is attempting to get password
        /// </summary>
        internal bool IsGettingPassword
        { get { return mIsGettingPassword; } }

        internal bool StoppedWithConfigError
        { get { return mStoppedWithConfigError; } }

        internal void ValidateUploadProfile()
        {
            try
            {
                if (LoggingSettings.Upload && Uploadable)
                {
                    mIsGettingPassword = true;
                    LoggingSettings.LoginDetails.AWBProfile =
                        WikiFunctions.Profiles.AWBProfiles.GetProfileForLogUploading(Program.AWB.Form);

                    if (!LoggingSettings.LoginDetails.IsSet)
                        throw new System.Configuration.ConfigurationErrorsException("Error getting login details");

                    mIsGettingPassword = false;
                }
            }
            catch (Exception ex)
            {
                mIsGettingPassword = false;
                if (ex.Message != "Log upload profile: User cancelled")
                {
                    ErrorHandler.Handle(ex);
                }
            }

            mStoppedWithConfigError = false;
        }

        // Private:
        private static string GetFilePrefix(string logFolder)
        {
            return string.Format("{1}\\{0:MMM-d yyyy HHmm-ss.FF}", DateTime.Now, logFolder);
        }

        private void NewXhtmlTraceListener()
        {
            AddListener(ConXhtml, new XhtmlTraceListener(GetFilePrefix(LoggingSettings.Settings.LogFolder) + 
                " log.html", LoggingSettings));
        }

        private void NewWikiTraceListener()
        {
            AddListener(ConWiki, new WikiTraceListener(GetFilePrefix(LoggingSettings.Settings.LogFolder) + 
                " log.txt", LoggingSettings));
        }

        private string GetFileNameFromActiveListener(string key)
        {
            return ((ITraceStatusProvider)(Listeners[key])).TraceStatus.FileName;
        }

        private void RemoveListenerAndReplaceWithSameType(string key)
        {
            string str = GetFileNameFromActiveListener(key);
            RemoveListener(key);
            NewWikiTraceListener();
            IMyTraceListener listener;
            if (Listeners.TryGetValue(key, out listener))
                listener.WriteCommentAndNewLine("logging continued from " + str);
        }

        private void Busy()
        {
            if (Listeners.Count == 0)
                return;
            BusyCounter += 1;
            LoggingSettings.LedColour = WikiFunctions.Controls.Colour.Green;
        }

        private void NotBusy()
        {
            if (Listeners.Count == 0)
                return;
            BusyCounter -= 1;
            if (BusyCounter == 0 && (LoggingSettings.LedColour != WikiFunctions.Controls.Colour.Blue))
            {
                LoggingSettings.LedColour = WikiFunctions.Controls.Colour.Red;
            }
        }

        private static bool BadPagesLogToUpload
        {
            get { return false; }
        }
        private bool WikiLogToUpload
        {
            get
            {
                IMyTraceListener listener;
                return (TryGetValue(ConWiki, out listener) && ((WikiTraceListener)listener).TraceStatus.LinesWrittenSinceLastUpload > 1);
            }
        }

        // Overrides:
        public override void AddListener(string key, IMyTraceListener listener)
        {
            if (ContainsKey(key))
            {
                base.RemoveListener(key);
            }

            base.AddListener(key, listener);
            if (listener.Uploadable)
            {
                ((TraceListenerUploadableBase)listener).Upload += TraceUploadEventHandler;
            }
        }
        public override void RemoveListener(string key)
        {
            IMyTraceListener listener;
            if (!Listeners.TryGetValue(key, out listener))
                return;

            if (listener.Uploadable)
            {
                ((TraceListenerUploadableBase)listener).Upload -= TraceUploadEventHandler;
            }

            base.RemoveListener(key);
        }
        public override void Close()
        {
            Busy();

            bool upload = (LoggingSettings.Settings.UploadYN && (BadPagesLogToUpload || WikiLogToUpload) 
                && MessageBox.Show("Upload logs?", "Logging", MessageBoxButtons.YesNo, MessageBoxIcon.Question, 
                MessageBoxDefaultButton.Button1) == DialogResult.Yes);

            foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
            {
                t.Value.WriteCommentAndNewLine("closing all logs");
                if (t.Value.Uploadable)
                {
                    ((TraceListenerUploadableBase)t.Value).Close(upload);
                }
                else
                {
                    t.Value.Close();
                }
            }
            Listeners.Clear();

            NotBusy();
        }

        internal void UploadWikiLog()
        {
            IMyTraceListener listener;
            if (TryGetValue(ConWiki, out listener))
            {
                ((WikiTraceListener)listener).UploadLog();
            }
        }

        #region Generic overrides
        public void AWBSkipped(string reason)
        {
            Busy();
            foreach (KeyValuePair<string, IMyTraceListener> listener in Listeners)
            {
                ((IAWBTraceListener)listener.Value).AWBSkipped(reason);
            }
        }

        public void PluginSkipped()
        {
            Busy();
            foreach (KeyValuePair<string, IMyTraceListener> listener in Listeners)
            {
                ((IAWBTraceListener)listener.Value).PluginSkipped();
            }
        }

        public void UserSkipped()
        {
            Busy();
            foreach (KeyValuePair<string, IMyTraceListener> listener in Listeners)
            {
                ((IAWBTraceListener)listener.Value).UserSkipped();
            }
        }

        public override void ProcessingArticle(string fullArticleTitle, int ns)
        {
            Busy();
            base.ProcessingArticle(fullArticleTitle, ns);
            NotBusy();
        }

        public override void SkippedArticle(string skippedBy, string reason)
        {
            Busy();
            base.SkippedArticle(skippedBy, reason);
            NotBusy();
        }

        public override void SkippedArticleBadTag(string skippedBy, string fullArticleTitle, int ns)
        {
            Busy();
            base.SkippedArticleBadTag(skippedBy, fullArticleTitle, ns);
            NotBusy();
        }

        public override void SkippedArticleRedlink(string skippedBy, string fullArticleTitle, int ns)
        {
            Busy();
            base.SkippedArticleRedlink(skippedBy, fullArticleTitle, ns);
            NotBusy();
        }

        public override void Write(string text)
        {
            Busy();
            base.Write(text);
            NotBusy();
        }

        public override void WriteArticleActionLine(string line, string pluginName)
        {
            Busy();
            base.WriteArticleActionLine(line, pluginName);
            NotBusy();
        }

        public override void WriteArticleActionLine1(string line, string pluginName, bool verboseOnly)
        {
            Busy();
            base.WriteArticleActionLine1(line, pluginName, verboseOnly);
            NotBusy();
        }

        public override void WriteBulletedLine(string line, bool bold, bool verboseOnly, bool dateStamp)
        {
            Busy();
            base.WriteBulletedLine(line, bold, verboseOnly, dateStamp);
            NotBusy();
        }

        public override void WriteComment(string line)
        {
            Busy();
            base.WriteComment(line);
            NotBusy();
        }

        public override void WriteCommentAndNewLine(string line)
        {
            Busy();
            base.WriteCommentAndNewLine(line);
            NotBusy();
        }

        public override void WriteLine(string line)
        {
            Busy();
            base.WriteLine(line);
            NotBusy();
        }

        public override void WriteTemplateAdded(string template, string pluginName)
        {
            Busy();
            base.WriteTemplateAdded(template, pluginName);
            NotBusy();
        }
        #endregion

        // Callback from Settings control:
        internal void PropertiesChange()
        {
            if (LoggingSettings.Settings.LogFolder == LogFolder)
            {
                if (LoggingSettings.Settings.LogXHTML && !ContainsKey(ConXhtml))
                    NewXhtmlTraceListener();
                else if (ContainsKey(ConXhtml))
                    RemoveListener(ConXhtml);

                if (LoggingSettings.Settings.LogWiki && !ContainsKey(ConWiki))
                    NewWikiTraceListener();
                else if (ContainsKey(ConWiki))
                    RemoveListener(ConWiki);
            }
            else if (HaveOpenFile) // folder has changed, close and reopen all active logs
            {
                if (ContainsKey(ConWiki))
                    RemoveListenerAndReplaceWithSameType(ConWiki);
                if (ContainsKey(ConXhtml))
                    RemoveListenerAndReplaceWithSameType(ConXhtml);
            }

            ValidateUploadProfile();
        }

        // Trace listener child classes:
        /// <summary>
        /// Keeps track of logging statistics
        /// </summary>
        /// <remarks></remarks>
        private sealed class TraceStatus : WikiFunctions.Logging.Uploader.TraceStatus
        {
            private readonly Label LinesLabel;
            private readonly Label LinesSinceUploadLabel;
            private readonly Label NumberOfUploadsLabel;
            private static int mUploadCount;

            // Initialisation
            public TraceStatus(Label pLinesLabel, Label pLinesSinceUploadLabel, Label pNumberOfUploadsLabel, 
                bool uploadable, string fileN, string logNameIs) : base(fileN, logNameIs)
            {
                LinesLabel = pLinesLabel;
                LinesSinceUploadLabel = pLinesSinceUploadLabel;
                if (pNumberOfUploadsLabel != null)
                {
                    NumberOfUploadsLabel = pNumberOfUploadsLabel;
                    NumberOfUploadsLabel.Text = mUploadCount.ToString();
                }
                pLinesLabel.Text = "0";
                if (uploadable)
                {
                    pLinesSinceUploadLabel.Text = "0";
                }
            }

            // Overrides:
            public override void Close()
            {
                LinesLabel.Text = "N/A";
                LinesSinceUploadLabel.Text = "N/A";
            }

            public override int LinesWritten
            {
                get { return base.LinesWritten; }
                set
                {
                    base.LinesWritten = value;
                    LinesLabel.Text = value.ToString();
                }
            }

            public override int LinesWrittenSinceLastUpload
            {
                get { return base.LinesWrittenSinceLastUpload; }
                set
                {
                    base.LinesWrittenSinceLastUpload = value;
                    if (LinesSinceUploadLabel != null)
                    {
                        LinesSinceUploadLabel.Text = value.ToString();
                    }
                }
            }

            // Extra:
            public int UploadsCount
            {
                get { return mUploadCount; }
                set
                {
                    mUploadCount = value;
                    NumberOfUploadsLabel.Text = value.ToString();
                }
            }
        }

        /// <summary>
        /// Logs in XHTML
        /// </summary>
        /// <remarks></remarks>
        private sealed class XhtmlTraceListener : XHTMLTraceListener,
            ITraceStatusProvider, IAWBTraceListener
        {
            private readonly TraceStatus mTraceStatus;

            public XhtmlTraceListener(string fileName, LoggingSettings ls)
                : base(fileName, ls.Settings.LogVerbose)
            {
                mTraceStatus = new TraceStatus(ls.XHTMLLinesLabel, null, null, false, fileName, ConXhtml);
            }

            public WikiFunctions.Logging.Uploader.TraceStatus TraceStatus
            {
                get { return mTraceStatus; }
            }

            public void AWBSkipped(string reason)
            {
                SkippedArticle("AWB", reason);
            }

            public void UserSkipped()
            {
                SkippedArticle(AWBLogListener.StringUser, AWBLogListener.StringUserSkipped);
            }

            public void PluginSkipped()
            {
                SkippedArticle(AWBLogListener.StringPlugin, AWBLogListener.StringPluginSkipped);
            }
        }

        /// <summary>
        /// Logs in wiki format
        /// </summary>
        /// <remarks></remarks>
        private sealed class WikiTraceListener : WikiFunctions.Logging.WikiTraceListener, IAWBTraceListener
        {
            public WikiTraceListener(string fileName, LoggingSettings ls)
                : base(ls.Settings, new TraceStatus(ls.WikiLinesLabel, ls.WikiLinesSinceUploadLabel, 
                ls.UploadsCountLabel, ls.Settings.UploadYN, fileName, ConWiki)) { }

            public void AWBSkipped(string reason)
            {
                SkippedArticle("AWB", reason);
            }
            public void UserSkipped()
            { 
                SkippedArticle(AWBLogListener.StringUser, AWBLogListener.StringUserSkipped); 
            }
            public void PluginSkipped()
            {
                SkippedArticle(AWBLogListener.StringPlugin, AWBLogListener.StringPluginSkipped);
            }
        }

        internal LoggingSettings LS
        {
            get { return LoggingSettings; }
            set { LoggingSettings = value; }
        }

        protected override string ApplicationName
        {
            get { return "AutoWikiBrowser logging manager"; }
        }

        // Allow plugins to turn off all logging:
        public void TurnOffLogging()
        {
            Close();
            LS.TurnOffLogging();
        }
    }
}