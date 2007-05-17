using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WikiFunctions.Logging.Uploader;
using AutoWikiBrowser.Logging;
using WikiFunctions;
using WikiFunctions.Plugin;

namespace AutoWikiBrowser
{
    /// <summary>
    /// Logging-configuration usercontrol
    /// </summary>
    internal sealed partial class LoggingSettings : UserControl
    {
        private bool mStartingUp;
        internal UsernamePassword LoginDetails = new UsernamePassword();
        private TextBox mCategoryTextBox;
        private bool mInitialised;
        internal Props Settings = new Props();

        public LoggingSettings(TextBox CategoryTextBox, IAutoWikiBrowser Main)
        {
            mStartingUp = true;
            mCategoryTextBox = CategoryTextBox;

            InitializeComponent();

            mStartingUp = false;
        }

        internal string LogFolder
        {
            set
            {
                Settings.LogFolder = value;
                FolderTextBox.Text = value;
            }
        }
        internal bool Initialised
        {
            get { return mInitialised; }
            set { mInitialised = value; }
        }
        internal bool IgnoreEvents
        {
            set { mStartingUp = value; }
        }
        internal WikiFunctions.Controls.Colour LEDColour
        {
            get { return Led1.Colour; }
            set { Led1.Colour = value; }
        }

        #region XML
        internal void ReadXML(System.Xml.XmlTextReader Reader, MyTrace MyTrace)
        {
            Props NewProps = new Props();

            NewProps.ReadXML(Reader); // ReadXML values into a new settings object

            if (!(Settings.Equals(NewProps)))
            {
                if (Settings.UploadYN && Settings.LogWiki)
                {
                    MyTrace.UploadWikiLog();
                }

                ApplySettingsToControls(NewProps);
                GetSettingsFromControls();
            }
        }
        internal void WriteXML(System.Xml.XmlTextWriter Writer)
        {
            Settings.WriteXML(Writer);
        }
        internal void Reset()
        {
            Props NewProps = new Props();

            if (!ApplyButton.Enabled && !(Settings.Equals(NewProps)))
            {
                WeHaveUnappliedChanges();
            }

            // Settings = NewProps ' No, that doesn't happen until the apply button is clicked
            ApplySettingsToControls(NewProps);
        }
        #endregion

        #region Event handlers - supporting routines
        private void ApplySettingsToControls(Props SettingsObject)
        {
            FolderTextBox.Text = SettingsObject.LogFolder;
            VerboseCheckBox.Checked = SettingsObject.LogVerbose;
            WikiLogCheckBox.Checked = SettingsObject.LogWiki;
            XHTMLLogCheckBox.Checked = SettingsObject.LogXHTML;
            UploadCheckBox.Checked = SettingsObject.UploadYN;
            UploadWatchlistCheckBox.Checked = SettingsObject.UploadAddToWatchlist;
            UploadJobNameTextBox.Text = SettingsObject.UploadJobName;
            UploadLocationTextBox.Text = SettingsObject.UploadLocation;
            UploadMaxLinesControl.Value = System.Convert.ToDecimal(SettingsObject.UploadMaxLines);
            UploadOpenInBrowserCheckBox.Checked = SettingsObject.UploadOpenInBrowser;
            UploadWikiProjectCheckBox.Checked = SettingsObject.UploadToWikiProjects;
        }
        private void GetSettingsFromControls()
        {
            DisableApplyButton();
            if (!(FolderTextBox.Text.Trim() == ""))
            {
                if (System.IO.Directory.Exists(FolderTextBox.Text))
                {
                    LogFolder = FolderTextBox.Text;
                }
                else
                {
                    GlobalObjects.AWB.NotifyBalloon("Folder doesn't exist, using previous setting (" + 
                        Settings.LogFolder + ")", ToolTipIcon.Warning);
                    FolderTextBox.Text = Settings.LogFolder;
                }
            }

            bool blnJobNameHasChanged = (!(Settings.UploadJobName == UploadJobNameTextBox.Text)) || 
                (UploadJobNameTextBox.Text == Props.conUploadCategoryIsJobName && 
                !(Settings.Category == mCategoryTextBox.Text));

            Settings.LogVerbose = VerboseCheckBox.Checked;
            Settings.LogWiki = WikiLogCheckBox.Checked;
            Settings.LogXHTML = XHTMLLogCheckBox.Checked;
            Settings.UploadYN = UploadCheckBox.Checked;
            Settings.UploadAddToWatchlist = UploadWatchlistCheckBox.Checked;
            Settings.UploadJobName = UploadJobNameTextBox.Text;
            Settings.UploadLocation = UploadLocationTextBox.Text;
            Settings.UploadMaxLines = System.Convert.ToInt32(UploadMaxLinesControl.Value);
            Settings.Category = mCategoryTextBox.Text;
            Settings.UploadOpenInBrowser = UploadOpenInBrowserCheckBox.Checked;
            Settings.UploadToWikiProjects = UploadWikiProjectCheckBox.Checked;

            if (mInitialised)
            {
                GlobalObjects.MyTrace.PropertiesChange(blnJobNameHasChanged);
            }
        }
        internal void WeHaveUnappliedChanges()
        {
            if (!mStartingUp)
            {
                if (GlobalObjects.MyTrace.HaveOpenFile)
                {
                    ApplyButton.Enabled = true;
                    ApplyButton.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    GetSettingsFromControls();
                }
            }
        }
        private void DisableApplyButton()
        {
            ApplyButton.Enabled = false;
            ApplyButton.BackColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.Control);
        }
        private void EnableDisableUploadControls(bool Enabled)
        {
            UploadJobNameTextBox.Enabled = Enabled;
            UploadLocationTextBox.Enabled = Enabled;
            UploadOpenInBrowserCheckBox.Enabled = Enabled;
            UploadWatchlistCheckBox.Enabled = Enabled;
            UploadWikiProjectCheckBox.Enabled = Enabled;
            UploadMaxLinesControl.Enabled = Enabled;
        }
        #endregion

        #region Event Handlers
		private void ApplyButton_Click(object sender, EventArgs e)
		{
			if (UploadJobNameTextBox.Text.Trim() == "")
			{
				MessageBox.Show("Please enter a job name", "Job name missing", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			else
			{
				GetSettingsFromControls();
			}
		}
		private void LocationReset(object sender, EventArgs e)
		{
			UploadLocationTextBox.Text = Props.conUploadToUserSlashLogsToken;
		}
		private void JobNameReset(object sender, EventArgs e)
		{
			UploadJobNameTextBox.Text = Props.conUploadCategoryIsJobName;
		}
		private void MaxLinesReset(object sender, EventArgs e)
		{
			UploadMaxLinesControl.Value = 1000;
		}
		private void SetLinesToMaximum(object sender, EventArgs e)
		{
			UploadMaxLinesControl.Value = UploadMaxLinesControl.Maximum;
		}
		private void CloseAllButton_Click(object sender, EventArgs e)
		{
			MyTrace.Close();
		}
		private void LogBadTagsCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			WeHaveUnappliedChanges();
		}
		private void WikiLogCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			if (WikiLogCheckBox.Checked && ! UploadCheckBox.Enabled)
			{
				UploadCheckBox.Enabled = true;
				EnableDisableUploadControls(UploadCheckBox.Checked);
			}
			else if (! WikiLogCheckBox.Checked && UploadCheckBox.Enabled)
			{
				UploadCheckBox.Enabled = false;
				EnableDisableUploadControls(false);
			}

			WeHaveUnappliedChanges();
		}
		private void UploadCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			EnableDisableUploadControls(UploadCheckBox.Checked);
			WeHaveUnappliedChanges();
		}
		private void XHTMLLogCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			WeHaveUnappliedChanges();
		}
		private void VerboseCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			WeHaveUnappliedChanges();
		}
		private void FolderTextBox_TextChanged(object sender, EventArgs e)
		{
			WeHaveUnappliedChanges();
		}
		private void FolderButton_Click(object sender, EventArgs e)
		{
			if (FolderBrowserDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				FolderTextBox.Text = FolderBrowserDialog1.SelectedPath;
				WeHaveUnappliedChanges();
			}
		}
		private void UploadWikiProjectCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			WeHaveUnappliedChanges();
		}
		private void UploadWatchlistCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			WeHaveUnappliedChanges();
		}
		private void UploadOpenInBrowserCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			WeHaveUnappliedChanges();
		}
		private void UploadLocationTextBox_TextChanged(object sender, EventArgs e)
		{
			WeHaveUnappliedChanges();
		}
		private void UploadJobNameTextBox_TextChanged(object sender, EventArgs e)
		{
			WeHaveUnappliedChanges();
		}
		private void UploadMaxLines_ValueChanged(object sender, EventArgs e)
		{
			WeHaveUnappliedChanges();
		}
#endregion

        internal sealed class Props : UploadableLogSettings2
        {

            private bool mLogBadPages = true;
            private bool mUploadToWikiProjects = true;
            private string mCategory = "";
            internal const string conUploadToUserSlashLogsToken = "$USER/Logs";
            internal const string conUploadCategoryIsJobName = "$CATEGORY";
            private static readonly string mPluginVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            private static string mUserName = "";

            internal Props()
                : base()
            {
                mUploadLocation = conUploadToUserSlashLogsToken;
                mUploadJobName = conUploadCategoryIsJobName;
            }

            internal bool Equals(Props Compare)
            {
                return (((((Operators.CompareString(Compare.LogFolder, this.LogFolder, false) == 0) &&
                    (Compare.LogVerbose == this.LogVerbose)) && ((Compare.LogWiki == this.LogWiki) &&
                    (Compare.LogXHTML == this.LogXHTML))) &&
                    (((Compare.UploadAddToWatchlist == this.UploadAddToWatchlist) &&
                    (Operators.CompareString(Compare.UploadJobName, this.UploadJobName, false) == 0)) &&
                    ((Operators.CompareString(Compare.UploadLocation, this.UploadLocation, false) == 0) &&
                    (Compare.UploadMaxLines == this.UploadMaxLines)))) &&
                    (((Compare.UploadOpenInBrowser == this.UploadOpenInBrowser) &&
                    (Compare.UploadToWikiProjects == this.UploadToWikiProjects)) &&
                    ((Compare.UploadYN == this.UploadYN) &&
                    (Operators.CompareString(Compare.Category, this.Category, false) == 0))));
            }

            #region Additional properties:
            internal static string PluginVersion
            {
                get { return mPluginVersion; }
            }
            internal static string AWBVersion
            {
                get { return Application.ProductVersion.ToString(); }
            }
            internal bool UploadToWikiProjects
            {
                get { return mUploadToWikiProjects; }
                set { mUploadToWikiProjects = value; }
            }
            internal List<LogEntry> LinksToLog
            {
                get
                {
                    List<LogEntry> tempLinksToLog = null;
                    tempLinksToLog = new List<LogEntry>();
                    tempLinksToLog.Add(new LogEntry(GlobbedUploadLocation, false));
                    if (mUploadToWikiProjects)
                    {
                        foreach (PluginBase Plugin in PluginManager.ActivePlugins)
                        {
                            if (Plugin.HasSharedLogLocation)
                            {
                                tempLinksToLog.Add(new LogEntry(Plugin.SharedLogLocation, true));
                            }
                        }
                    }
                    if (tempLinksToLog.Count > 1 && UserName == "")
                    {
                        throw new System.Configuration.SettingsPropertyNotFoundException("The plugin hasn't received the username from AWB");
                    }
                    return tempLinksToLog;
                }
            }
            internal string GlobbedUploadLocation
            {
                get { return mUploadLocation.Replace("$USER", "User:" + UserName); }
            }
            internal static string UserName
            {
                get { return mUserName; }
                set { mUserName = value; }
            }
            internal string Category
            {
                get { return mCategory; }
                set { mCategory = value; }
            }
            internal string WikifiedCategory
            {
                get
                {
                    if (mCategory == "")
                    {
                        return "";
                    }
                    else
                    {
                        return "[[:Category:" + mCategory + "|" + mCategory + "]]";
                    }
                }
            }
            internal string LogTitle
            {
                get { return Tools.RemoveInvalidChars(mUploadJobName.Replace(conUploadCategoryIsJobName, mCategory)); }
            }
            #endregion

            #region XML interface
            private const string conLogBadPages = "LogBadPages";
            private const string conLogXHTML = "LogXHTML";
            private const string conLogWiki = "LogWiki";
            private const string conLogVerbose = "LogVerbose";
            private const string conLogFolder = "LogFolder";
            private const string conLogUploadMaxLines = "LogUploadMaxLines";
            private const string conLogUploadYN = "LogUploadYN";
            private const string conLogUploadToWikiProjects = "LogUploadWPs";
            private const string conLogUploadWatchlist = "LogUploadWatch";
            private const string conLogUploadOpenInBrowser = "LogUploadBrowser";
            private const string conLogUploadLocation = "LogUploadLoc";
            private const string conLogUploadJobName = "LogUploadJob";
            public void ReadXML(System.Xml.XmlTextReader Reader)
            {
                mLogFolder = PluginManager.XMLReadString(Reader, conLogFolder, LogFolder);
                mLogVerbose = PluginManager.XMLReadBoolean(Reader, conLogVerbose, LogVerbose);
                mLogWiki = PluginManager.XMLReadBoolean(Reader, conLogWiki, LogWiki);
                mLogXHTML = PluginManager.XMLReadBoolean(Reader, conLogXHTML, LogXHTML);
                mUploadYN = PluginManager.XMLReadBoolean(Reader, conLogUploadYN, UploadYN);
                mUploadAddToWatchlist = PluginManager.XMLReadBoolean(Reader, conLogUploadWatchlist, UploadAddToWatchlist);
                mUploadJobName = PluginManager.XMLReadString(Reader, conLogUploadJobName, UploadJobName);
                mUploadLocation = PluginManager.XMLReadString(Reader, conLogUploadLocation, UploadLocation);
                mUploadMaxLines = PluginManager.XMLReadInteger(Reader, conLogUploadMaxLines, UploadMaxLines);
                mUploadOpenInBrowser = PluginManager.XMLReadBoolean(Reader, conLogUploadOpenInBrowser, UploadOpenInBrowser);
                mUploadToWikiProjects = PluginManager.XMLReadBoolean(Reader, conLogUploadToWikiProjects, UploadToWikiProjects);
            }
            public void WriteXML(System.Xml.XmlTextWriter Writer)
            {
                Writer.WriteAttributeString(conLogFolder, LogFolder);
                Writer.WriteAttributeString(conLogVerbose, LogVerbose.ToString());
                Writer.WriteAttributeString(conLogWiki, LogWiki.ToString());
                Writer.WriteAttributeString(conLogXHTML, LogXHTML.ToString());
                Writer.WriteAttributeString(conLogUploadJobName, UploadJobName);
                Writer.WriteAttributeString(conLogUploadLocation, UploadLocation);
                Writer.WriteAttributeString(conLogUploadMaxLines, UploadMaxLines.ToString());
                Writer.WriteAttributeString(conLogUploadOpenInBrowser, UploadOpenInBrowser.ToString());
                Writer.WriteAttributeString(conLogUploadToWikiProjects, UploadToWikiProjects.ToString());
                Writer.WriteAttributeString(conLogUploadWatchlist, UploadAddToWatchlist.ToString());
                Writer.WriteAttributeString(conLogUploadYN, UploadYN.ToString());
            }
            #endregion
        }
    }
}
