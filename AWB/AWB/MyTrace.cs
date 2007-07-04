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
using System.Text;
using WikiFunctions;
using WikiFunctions.Logging;
using WikiFunctions.Plugin;
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
        private int BusyCounter = 0;
        private bool mIsUploading = false;

        private const string conWiki = "Wiki";
        private const string conXHTML = "XHTML";

        // The most important stuff:
        internal void Initialise()
        {
            try
            {
                LoggingSettings.Initialised = true;
                LogFolder = LoggingSettings.Settings.LogFolder;

                if (LoggingSettings.Settings.LogXHTML || LoggingSettings.Settings.LogWiki)
                {
                    if (!(System.IO.Directory.Exists(LoggingSettings.Settings.LogFolder)))
                    {
                        LogFolder = Application.StartupPath;
                    }

                    if (LoggingSettings.Settings.LogXHTML)
                    {
                        NewXHTMLTraceListener();
                    }
                    if (LoggingSettings.Settings.LogWiki)
                    {
                        NewWikiTraceListener();
                    }
                }
                foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
                {
                    ((IAWBTraceListener)t.Value).WriteBulletedLine(Variables.LoggingStartButtonClicked, true, false, true);
                }
                CheckWeHaveLogInDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TraceUploadEventHandler(TraceListenerUploadableBase Sender, ref bool Success)
        {
            Success = base.UploadHandler(Sender, LoggingSettings.Settings.LogTitle, 
                LoggingSettings.Settings.WikifiedCategory, LoggingSettings.Settings.GlobbedUploadLocation + "/" + 
                Sender.PageName.Replace(LoggingSettings.Props.conUploadCategoryIsJobName, 
                LoggingSettings.Settings.Category), LoggingSettings.Settings.LinksToLog(), 
                LoggingSettings.Settings.UploadOpenInBrowser, LoggingSettings.Settings.UploadAddToWatchlist, 
                LoggingSettings.Props.UserName, Variables.AWBVersionString(GlobalObjects.AWB.AWBVersionString) +
                Plugins.Plugin.GetPluginsWikiTextBlock(), Variables.AWBLoggingEditSummary +
                Variables.UploadingLogDefaultEditSummary, Variables.AWBLoggingEditSummary +
                Variables.UploadingLogEntryDefaultEditSummary, GlobalObjects.AWB, LoggingSettings.LoginDetails);

            if (Success)
            {
                ((TraceStatus)Sender.TraceStatus).UploadsCount += 1;
            }
        }
        protected override bool StartingUpload(TraceListenerUploadableBase Sender)
        {
            if (!(Sender.TraceStatus.LogName == conWiki))
                return false;

            mIsUploading = true;
            LoggingSettings.LEDColour = WikiFunctions.Controls.Colour.Blue;
            Application.DoEvents();
            return true;
        }

        protected override void FinishedUpload()
        {
            if (BusyCounter == 0)
                LoggingSettings.LEDColour = WikiFunctions.Controls.Colour.Red;
            else
                LoggingSettings.LEDColour = WikiFunctions.Controls.Colour.Green;

            mIsUploading = false;
        }

        // State:
        internal bool HaveOpenFile
        {
            get { return Listeners.Count > 0; }
        }

        internal bool IsUploading
        {
            get { return mIsUploading; }
        }

        // Private:
        private void CheckWeHaveLogInDetails()
        {
            if (this.Uploadable && !LoggingSettings.LoginDetails.IsSet)
            {
                // TODO: This needs to be imported from plugin and fixed, but what about AWBProfiles? Encyryption? etc
                //LoggingSettings.LoginDetails = new LoginForm().GetUsernamePassword;
                //if (!LoggingSettings.LoginDetails.IsSet)
                //{
                //    throw new System.Configuration.ConfigurationErrorsException("Error getting login details");
                //}
            }
        }
        private static string GetFilePrefix(string LogFolder)
        {
            return string.Format("{1}\\{0:MMM-d yyyy HHmm-ss.FF}", System.DateTime.Now, LogFolder);
        }
        private void NewXHTMLTraceListener()
        {
            AddListener(conXHTML, new XHTMLTraceListener(GetFilePrefix(LoggingSettings.Settings.LogFolder) + 
                " log.html", LoggingSettings));
        }
        private void NewWikiTraceListener()
        {
            AddListener(conWiki, new WikiTraceListener(GetFilePrefix(LoggingSettings.Settings.LogFolder) + 
                " log.txt", LoggingSettings));
        }
        private string GetFileNameFromActiveListener(string Key)
        {
            return ((ITraceStatusProvider)(Listeners[Key])).TraceStatus.FileName;
        }
        private void RemoveListenerAndReplaceWithSameType(string Key)
        {
            string str = GetFileNameFromActiveListener(Key);
            RemoveListener(Key);
            NewWikiTraceListener();
            Listeners[Key].WriteCommentAndNewLine("logging continued from " + str);
        }
        private void Busy()
        {
            if (Listeners.Count == 0)
                return;
            BusyCounter += 1;
            LoggingSettings.LEDColour = WikiFunctions.Controls.Colour.Green;
        }
        private void NotBusy()
        {
            if (Listeners.Count == 0)
                return;
            BusyCounter -= 1;
            if (BusyCounter == 0 && !(LoggingSettings.LEDColour == WikiFunctions.Controls.Colour.Blue))
            {
                LoggingSettings.LEDColour = WikiFunctions.Controls.Colour.Red;
            }
        }

        private bool BadPagesLogToUpload
        {
            get { return false; }
        }
        private bool WikiLogToUpload
        {
            get { return (ContainsKey(conWiki) && ((WikiTraceListener)(Listeners[conWiki])).TraceStatus.LinesWrittenSinceLastUpload > 1); }
        }

        // Overrides:
        public override void AddListener(string Key, IMyTraceListener Listener)
        {
            if (base.ContainsKey(Key))
            {
                base.RemoveListener(Key);
            }

            base.AddListener(Key, Listener);
            if (Listener.Uploadable)
            {
                ((TraceListenerUploadableBase)Listener).Upload += 
                    new WikiFunctions.Logging.TraceListenerUploadableBase.UploadEventHandler(
                    this.TraceUploadEventHandler);
            }
        }
        public override void RemoveListener(string Key)
        {
            if (!Listeners.ContainsKey(Key)) return;
            IMyTraceListener Listener = Listeners[Key];

            if (Listener.Uploadable)
            {
                ((TraceListenerUploadableBase)Listener).Upload -= 
                    new WikiFunctions.Logging.TraceListenerUploadableBase.UploadEventHandler(
                    this.TraceUploadEventHandler);
            }

            base.RemoveListener(Key);
        }
        public override void Close()
        {
            Busy();

            bool upload = false;

            if (LoggingSettings.Settings.UploadYN && (BadPagesLogToUpload || WikiLogToUpload) && MessageBox.Show("Upload logs?", "Logging", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                upload = true;

            foreach (KeyValuePair<string, IMyTraceListener> t in Listeners)
            {
                ((IAWBTraceListener)t.Value).WriteCommentAndNewLine("closing all logs");
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

        // Friend:
        internal void UploadWikiLog()
        {
            if (ContainsKey(conWiki))
            {
                ((WikiTraceListener)(Listeners[conWiki])).UploadLog();
            }
        }

        #region Generic overrides
        public void AWBSkipped(string Reason)
        {
            Busy();
            foreach (KeyValuePair<string, IMyTraceListener> Listener in Listeners)
            {
                ((IAWBTraceListener)Listener.Value).AWBSkipped(Reason);
            }
        }
        public void PluginSkipped()
        {
            Busy();
            foreach (KeyValuePair<string, IMyTraceListener> Listener in Listeners)
            {
                ((IAWBTraceListener)Listener.Value).PluginSkipped();
            }
        }
        public void UserSkipped()
        {
            Busy();
            foreach (KeyValuePair<string, IMyTraceListener> Listener in Listeners)
            {
                ((IAWBTraceListener)Listener.Value).UserSkipped();
            }
        }
        public override void ProcessingArticle(string FullArticleTitle, WikiFunctions.Namespaces NS)
        {
            Busy();
            base.ProcessingArticle(FullArticleTitle, NS);
            NotBusy();
        }
        public override void SkippedArticle(string SkippedBy, string Reason)
        {
            Busy();
            base.SkippedArticle(SkippedBy, Reason);
            NotBusy();
        }
        public override void SkippedArticleBadTag(string SkippedBy, string FullArticleTitle, WikiFunctions.Namespaces NS)
        {
            Busy();
            base.SkippedArticleBadTag(SkippedBy, FullArticleTitle, NS);
            NotBusy();
        }
        public override void SkippedArticleRedlink(string SkippedBy, string FullArticleTitle, WikiFunctions.Namespaces NS)
        {
            Busy();
            base.SkippedArticleRedlink(SkippedBy, FullArticleTitle, NS);
            NotBusy();
        }
        public override void Write(string Text)
        {
            Busy();
            base.Write(Text);
            NotBusy();
        }
        public override void WriteArticleActionLine(string Line, string PluginName)
        {
            Busy();
            base.WriteArticleActionLine(Line, PluginName);
            NotBusy();
        }
        public override void WriteArticleActionLine1(string Line, string PluginName, bool VerboseOnly)
        {
            Busy();
            base.WriteArticleActionLine1(Line, PluginName, VerboseOnly);
            NotBusy();
        }
        public override void WriteBulletedLine(string Line, bool Bold, bool VerboseOnly, bool DateStamp)
        {
            Busy();
            base.WriteBulletedLine(Line, Bold, VerboseOnly, DateStamp);
            NotBusy();
        }
        public override void WriteComment(string Line)
        {
            Busy();
            base.WriteComment(Line);
            NotBusy();
        }
        public override void WriteCommentAndNewLine(string Line)
        {
            Busy();
            base.WriteCommentAndNewLine(Line);
            NotBusy();
        }
        public override void WriteLine(string Line)
        {
            Busy();
            base.WriteLine(Line);
            NotBusy();
        }
        public override void WriteTemplateAdded(string Template, string PluginName)
        {
            Busy();
            base.WriteTemplateAdded(Template, PluginName);
            NotBusy();
        }
        #endregion

        // Callback from Settings control:
        internal void PropertiesChange(bool JobNameHasChanged)
        {
            if (LoggingSettings.Settings.LogFolder == LogFolder)
            {
                if (LoggingSettings.Settings.LogXHTML)
                    if (!(ContainsKey(conXHTML)))
                        NewXHTMLTraceListener();
                else if (ContainsKey(conXHTML))
                    RemoveListener(conXHTML);

                if (LoggingSettings.Settings.LogWiki)
                    if (!(ContainsKey(conWiki)))
                        NewWikiTraceListener();
                else if (ContainsKey(conWiki))
                    RemoveListener(conWiki);
            }
            else if (HaveOpenFile) // folder has changed, close and reopen all active logs
            {
                if (ContainsKey(conWiki))
                    RemoveListenerAndReplaceWithSameType(conWiki);
                if (ContainsKey(conXHTML))
                    RemoveListenerAndReplaceWithSameType(conXHTML);
            }

            CheckWeHaveLogInDetails();
        }

        // Trace listener child classes:
        /// <summary>
        /// Keeps track of logging statistics
        /// </summary>
        /// <remarks></remarks>
        private sealed class TraceStatus : WikiFunctions.Logging.Uploader.TraceStatus
        {
            private Label LinesLabel;
            private Label LinesSinceUploadLabel;
            private Label NumberOfUploadsLabel;
            private static int mUploadCount;

            // Initialisation
            public TraceStatus(Label pLinesLabel, Label pLinesSinceUploadLabel, Label pNumberOfUploadsLabel, 
                bool Uploadable, string FileN, string LogNameIs) : base(FileN, LogNameIs)
            {
                LinesLabel = pLinesLabel;
                LinesSinceUploadLabel = pLinesSinceUploadLabel;
                if (pNumberOfUploadsLabel != null)
                {
                    NumberOfUploadsLabel = pNumberOfUploadsLabel;
                    NumberOfUploadsLabel.Text = mUploadCount.ToString();
                }
                pLinesLabel.Text = "0";
                if (Uploadable)
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
        private sealed class XHTMLTraceListener : WikiFunctions.Logging.XHTMLTraceListener,
            WikiFunctions.Logging.Uploader.ITraceStatusProvider, IAWBTraceListener
        {
            private TraceStatus mTraceStatus;

            public XHTMLTraceListener(string FileName, LoggingSettings LS)
                : base(FileName, LS.Settings.LogVerbose)
            {
                mTraceStatus = new TraceStatus(LS.XHTMLLinesLabel, null, null, false, FileName, conXHTML);
            }
            public WikiFunctions.Logging.Uploader.TraceStatus TraceStatus
            {
                get { return mTraceStatus; }
            }
            public void AWBSkipped(string Reason)
            {
                this.SkippedArticle("AWB", Reason);
            }
            public void UserSkipped()
            { this.SkippedArticle(Variables.StringUser, Variables.StringUserSkipped); }
            public void PluginSkipped()
            { this.SkippedArticle(Variables.StringPlugin, Variables.StringPluginSkipped); }
        }

        /// <summary>
        /// Logs in wiki format
        /// </summary>
        /// <remarks></remarks>
        private sealed class WikiTraceListener : WikiFunctions.Logging.WikiTraceListener, IAWBTraceListener
        {
            public WikiTraceListener(string FileName, LoggingSettings LS)
                : base(LS.Settings, new TraceStatus(LS.WikiLinesLabel, LS.WikiLinesSinceUploadLabel, 
                LS.UploadsCountLabel, LS.Settings.UploadYN, FileName, conWiki)) { }

            public void AWBSkipped(string Reason)
            {
                this.SkippedArticle("AWB", Reason);
            }
            public void UserSkipped()
            { this.SkippedArticle(Variables.StringUser, Variables.StringUserSkipped); }
            public void PluginSkipped()
            { this.SkippedArticle(Variables.StringPlugin, Variables.StringPluginSkipped); }
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
            Close(); LS.TurnOffLogging();
        }

        internal static AWBLogListener InitialiseLogListener(ArticleEx Article)
        { return null;/* Article.InitialiseLogListener("AWB", GlobalObjects.MyTrace);*/ }
        // TODO: At some point we need to *remove* the listener for the article ("AWB")
        // Plugin did it at the end of ProcessArticle(). We also do it, but a bit late, in AddListener().
    }
}