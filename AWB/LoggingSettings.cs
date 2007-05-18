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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WikiFunctions.Logging.Uploader;
using AutoWikiBrowser.Logging;
using WikiFunctions;
using WikiFunctions.AWBSettings;
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
        private bool mInitialised;
        internal Props Settings = new Props();

        public LoggingSettings() : base()
        {
            mStartingUp = true;
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

        public void TurnOffLogging()
        {
            Settings.LogXHTML = false;
            Settings.LogWiki = false;
            IgnoreEvents = true;
            WikiLogCheckBox.Checked = false;
            XHTMLLogCheckBox.Checked = false;
            IgnoreEvents = false;
        }

        #region Settings
        // TODO: Ought to be able to serialise the Props class? Ask Max/Mets
        public LoggingPrefs SerialisableSettings
        {
            get
            {
                LoggingPrefs prefs = new LoggingPrefs();
                prefs.LogFolder = FolderTextBox.Text;
                prefs.LogVerbose = VerboseCheckBox.Checked;
                prefs.LogWiki = WikiLogCheckBox.Checked;
                prefs.LogXHTML = XHTMLLogCheckBox.Checked;
                prefs.UploadJobName = UploadJobNameTextBox.Text;
                prefs.UploadLocation = UploadLocationTextBox.Text;
                prefs.UploadMaxLines = int.Parse(UploadMaxLinesControl.Value.ToString());
                prefs.UploadOpenInBrowser = UploadOpenInBrowserCheckBox.Checked;
                prefs.UploadToWikiProjects = UploadWikiProjectCheckBox.Checked;
                prefs.UploadAddToWatchlist = UploadWatchlistCheckBox.Checked;
                prefs.UploadYN = UploadCheckBox.Checked;
                prefs.Category = UploadJobNameTextBox.Text;

                return prefs;
            }
            set
            {
                LoggingPrefs prefs = value;

                FolderTextBox.Text = prefs.LogFolder;
                VerboseCheckBox.Checked = prefs.LogVerbose;
                WikiLogCheckBox.Checked = prefs.LogWiki;
                XHTMLLogCheckBox.Checked = prefs.LogXHTML;
                UploadJobNameTextBox.Text = prefs.UploadJobName;
                UploadLocationTextBox.Text = prefs.UploadLocation;
                UploadMaxLinesControl.Value = prefs.UploadMaxLines;
                UploadOpenInBrowserCheckBox.Checked = prefs.UploadOpenInBrowser;
                UploadWikiProjectCheckBox.Checked = prefs.UploadToWikiProjects;
                UploadWatchlistCheckBox.Checked = prefs.UploadAddToWatchlist;
                UploadCheckBox.Checked = prefs.UploadYN;
                UploadJobNameTextBox.Text = prefs.Category;
            }
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
                !(Settings.Category == GlobalObjects.AWB.CategoryTextBox.Text));

            Settings.LogVerbose = VerboseCheckBox.Checked;
            Settings.LogWiki = WikiLogCheckBox.Checked;
            Settings.LogXHTML = XHTMLLogCheckBox.Checked;
            Settings.LogSQL = SQLLogCheckBox.Checked;
            Settings.UploadYN = UploadCheckBox.Checked;
            Settings.UploadAddToWatchlist = UploadWatchlistCheckBox.Checked;
            Settings.UploadJobName = UploadJobNameTextBox.Text;
            Settings.UploadLocation = UploadLocationTextBox.Text;
            Settings.UploadMaxLines = System.Convert.ToInt32(UploadMaxLinesControl.Value);
            Settings.Category = GlobalObjects.AWB.CategoryTextBox.Text;
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
        private void WeHaveUnappliedChanges(object sender, EventArgs e)
        { WeHaveUnappliedChanges(); }
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
			GlobalObjects.MyTrace.Close();
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
		private void FolderButton_Click(object sender, EventArgs e)
		{
			if (FolderBrowserDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				FolderTextBox.Text = FolderBrowserDialog1.SelectedPath;
				WeHaveUnappliedChanges();
			}
		}
#endregion

        internal sealed class Props : UploadableLogSettings2
        {
            private bool mUploadToWikiProjects = true;
            private string mCategory = "";
            internal const string conUploadToUserSlashLogsToken = "$USER/Logs";
            internal const string conUploadCategoryIsJobName = "$CATEGORY";
            private static string mUserName = "";

            internal Props()
                : base()
            {
                mUploadLocation = conUploadToUserSlashLogsToken;
                mUploadJobName = conUploadCategoryIsJobName;
            }

            internal bool Equals(Props Compare)
            {
                return ((Compare.LogFolder == LogFolder) && (Compare.LogVerbose == LogVerbose) 
                    && (Compare.LogWiki == LogWiki) && (Compare.LogXHTML == LogXHTML)
                    && (Compare.UploadAddToWatchlist == UploadAddToWatchlist) && (Compare.UploadJobName == UploadJobName)
                    && (Compare.UploadLocation == UploadLocation) && (Compare.UploadMaxLines == UploadMaxLines)
                    && (Compare.UploadOpenInBrowser == UploadOpenInBrowser)
                    && (Compare.UploadToWikiProjects == UploadToWikiProjects) && (Compare.UploadYN == UploadYN)
                    && (Compare.Category == Category));
            }

            #region Additional properties:
            internal bool UploadToWikiProjects
            {
                get { return mUploadToWikiProjects; }
                set { mUploadToWikiProjects = value; }
            }
            internal List<LogEntry> LinksToLog()
            {
                List<LogEntry> tempLinksToLog = new List<LogEntry>();
                // TODO: We could create a temporary AWB logs page and have alpha version upload to that?
                tempLinksToLog.Add(new LogEntry(GlobbedUploadLocation, false));

                // If "uploading to WikiProjects" (or other plugin-specified locations), get details from plugins:
                if (mUploadToWikiProjects)
                    ((MainForm)GlobalObjects.AWB).GetLogUploadLocationsEvent(tempLinksToLog);

                if (tempLinksToLog.Count > 1 && UserName == "")
                {
                    throw new System.Configuration.SettingsPropertyNotFoundException(
                        "We don't have a username");
                    // TODO: Username stuff can now get better integrated with AWB, and preferably AWBProfiles
                }
                return tempLinksToLog;
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
        }
    }
}
