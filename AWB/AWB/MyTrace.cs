using System;
using System.Collections.Generic;
using System.Text;
using WikiFunctions;
using WikiFunctions.Logging;
using WikiFunctions.Plugin;
using System.Windows.Forms;

namespace AutoWikiBrowser.Logging
{
    /// <summary>
    /// Logging manager
    /// </summary>
    internal sealed class MyTrace : TraceManager
    {
        //private const string conWiki = "Wiki";

        //private bool mIsUploading = false;
        //private LoggingSettings LoggingSettings;
        //private static string LogFolder = "";

        //internal void Initialise()
        //{
        //    LoggingSettings.Initialised = true;
        //}

        //internal bool HaveOpenFile
        //{ get { return (Listeners.Count > 0); } }

        //protected override string ApplicationName
        //{
        //    get { return "AutoWikiBrowser logging manager"; }
        //}

        //protected override bool StartingUpload(TraceListenerUploadableBase Sender)
        //{
        //    if (Sender.TraceStatus.LogName != conWiki)
        //        return false;

        //    mIsUploading = true;
        //    LoggingSettings.LEDColour = WikiFunctions.Controls.Colour.Blue;
        //    Application.DoEvents();
        //    return true;
        //}

        private PluginLogging LoggingSettings;
        private static string LogFolder;
        private WikiFunctions.Browser.WebControl webcontrol;
        private bool LoggedIn;
        private string LoggedInUser;
        private int BusyCounter = 0;
        private bool mIsUploading;

        private const string conWiki = "Wiki";
        private const string conXHTML = "XHTML";

        // The most important stuff:
        internal void Initialise()
        {
            LoggingSettings.Initialised = true;
            LogFolder = LoggingSettings.Settings.LogFolder;

            if (LoggingSettings.Settings.LogXHTML || LoggingSettings.Settings.LogWiki)
            {
                if (!(IO.Directory.Exists(LoggingSettings.Settings.LogFolder)))
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
                t.Value.WriteBulletedLine("Start button clicked. Initialising log.", true, false, true);
            }
            CheckWeHaveLogInDetails();
        }

        private void TraceUploadEventHandler(TraceListenerUploadableBase Sender, ref bool Success)
        {

            Success = base.UploadHandler(Sender, LoggingSettings.Settings.LogTitle, LoggingSettings.Settings.WikifiedCategory, LoggingSettings.Settings.GlobbedUploadLocation + "/" + Sender.PageName.Replace(PluginLogging.Props.conUploadCategoryIsJobName, LoggingSettings.Settings.Category), LoggingSettings.Settings.LinksToLog(), LoggingSettings.Settings.UploadOpenInBrowser, LoggingSettings.Settings.UploadAddToWatchlist, PluginLogging.Props.UserName, "*" + PluginManager.conWikiPlugin + " version " + PluginLogging.Props.PluginVersion + System.Environment.NewLine + "*[[WP:AWB|AWB]] version " + PluginLogging.Props.AWBVersion + System.Environment.NewLine, PluginManager.conWikiPluginBrackets + " " + LogUploader.conUploadingDefaultEditSummary, PluginManager.conWikiPluginBrackets + " " + LogUploader.conAddingLogEntryDefaultEditSummary, PluginManager.AWBForm, LoggingSettings.LoginDetails);

            if (Success)
            {
                ((TraceStatus)Sender.TraceStatus).UploadsCount += 1;
            }
        }
        protected override bool StartingUpload(TraceListenerUploadableBase Sender)
        {
            if (!(Sender.TraceStatus.LogName == conWiki))
            {
                return false;
            }

            mIsUploading = true;
            LoggingSettings.LEDColour = Colour.Blue;
            Application.DoEvents();
            //INSTANT C# NOTE: Inserted the following 'return' since all code paths must return a value in C#:
            return false;
        }
        protected override void FinishedUpload()
        {
            if (BusyCounter == 0)
                LoggingSettings.LEDColour = Colour.Red;
            else
                LoggingSettings.LEDColour = Colour.Green;

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
                LoggingSettings.LoginDetails = new LoginForm().GetUsernamePassword;
                if (!LoggingSettings.LoginDetails.IsSet)
                {
                    throw new System.Configuration.ConfigurationErrorsException("Error getting login details");
                }
            }
        }
        private static string GetFilePrefix(string LogFolder)
        {
            return string.Format("{1}\\{0:MMM-d yyyy HHmm-ss.FF}", System.DateTime.Now, LogFolder);
        }
        private void NewXHTMLTraceListener()
        {
            AddListener(conXHTML, new XHTMLTraceListener(GetFilePrefix(LoggingSettings.Settings.LogFolder) + " log.html", LoggingSettings));
        }
        private void NewWikiTraceListener()
        {
            AddListener(conWiki, new WikiTraceListener(GetFilePrefix(LoggingSettings.Settings.LogFolder) + " log.txt", LoggingSettings));
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
            {
                return;
            }
            BusyCounter += 1;
            LoggingSettings.LEDColour = Colour.Green;
        }
        private void NotBusy()
        {
            if (Listeners.Count == 0)
            {
                return;
            }
            BusyCounter -= 1;
            if (BusyCounter == 0 && !(LoggingSettings.LEDColour == Colour.Blue))
            {
                LoggingSettings.LEDColour = Colour.Red;
            }
        }

        private bool BadPagesLogToUpload
        {
            get { return false; }
        }
        private bool WikiLogToUpload
        {
            get
            {
                if (ContainsKey(conWiki) && ((WikiTraceListener)(Listeners[conWiki])).TraceStatus.LinesWrittenSinceLastUpload > 1)
                {
                    return true;
                }
                else
                    return false;
            }
        }

        // Overrides:
        public override void AddListener(string Key, IMyTraceListener Listener)
        {
            if (Key == "AWB" && base.ContainsKey("AWB"))
            {
                base.RemoveListener("AWB");
            }

            base.AddListener(Key, Listener);
            if (Listener.Uploadable)
            {
                ((TraceListenerUploadableBase)Listener).Upload += new System.EventHandler(this.TraceUploadEventHandler);
            }
        }
        public override void RemoveListener(string Key)
        {
            IMyTraceListener Listener = Listeners[Key];

            if (Listener.Uploadable)
            {
                ((TraceListenerUploadableBase)Listener).Upload -= new System.EventHandler(this.TraceUploadEventHandler);
            }

            base.RemoveListener(Key);
        }
        public override void Close()
        {
            Busy();

            bool upload = false;

            if (LoggingSettings.Settings.UploadYN && (BadPagesLogToUpload || WikiLogToUpload) && MessageBox.Show("Upload logs?", "Logging", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                upload = true;
            }

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

        // Friend:
        internal void UploadWikiLog()
        {
            if (ContainsKey(conWiki))
            {
                ((WikiTraceListener)(Listeners[conWiki])).UploadLog();
            }
        }

        #region Generic overrides
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
                {
                    if (!(ContainsKey(conXHTML)))
                    {
                        NewXHTMLTraceListener();
                    }
                }
                else if (ContainsKey(conXHTML))
                {
                    RemoveListener(conXHTML);
                }

                if (LoggingSettings.Settings.LogWiki)
                {
                    if (!(ContainsKey(conWiki)))
                    {
                        NewWikiTraceListener();
                    }
                }
                else if (ContainsKey(conWiki))
                {
                    RemoveListener(conWiki);
                }

            }
            else if (HaveOpenFile) // folder has changed, close and reopen all active logs
            {
                if (ContainsKey(conWiki))
                {
                    RemoveListenerAndReplaceWithSameType(conWiki);
                }
                if (ContainsKey(conXHTML))
                {
                    RemoveListenerAndReplaceWithSameType(conXHTML);
                }
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
            public TraceStatus(Label pLinesLabel, Label pLinesSinceUploadLabel, Label pNumberOfUploadsLabel, bool Uploadable, string FileN, string LogNameIs)
                : base(FileN, LogNameIs)
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
        private sealed class XHTMLTraceListener : XHTMLTraceListener, WikiFunctions.Logging.Uploader.ITraceStatusProvider
        {
            private TraceStatus mTraceStatus;

            public XHTMLTraceListener(string FileName, PluginLogging LS)
                : base(FileName, LS.Settings.LogVerbose)
            {
                mTraceStatus = new TraceStatus(LS.XHTMLLinesLabel, null, null, false, FileName, conXHTML);
            }
            public WikiFunctions.Logging.Uploader.TraceStatus TraceStatus
            {
                get { return mTraceStatus; }
            }
        }

        /// <summary>
        /// Logs in wiki format
        /// </summary>
        /// <remarks></remarks>
        private sealed class WikiTraceListener : WikiTraceListener
        {

            public WikiTraceListener(string FileName, PluginLogging LS)
                : base(LS.Settings, new TraceStatus(LS.WikiLinesLabel, LS.WikiLinesSinceUploadLabel, LS.UploadsCountLabel, LS.Settings.UploadYN, FileName, conWiki))
            {
            }

            public override void CheckCounterForUpload()
            {
                // Explicitly define to allow breakpoint
                base.CheckCounterForUpload();
            }
        }

        internal PluginLogging LS
        {
            set { LoggingSettings = value; }
        }

        protected override string ApplicationName
        {
            get { return "Kingbotk Plugin logging manager"; }
        }
    }
}