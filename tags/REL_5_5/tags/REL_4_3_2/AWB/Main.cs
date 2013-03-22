/*
Autowikibrowser
Copyright (C) 2007 Martin Richards
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

#undef INSTASTATS // turn on here and in stats.cs to make AWB log (empty) stats at startup

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Web;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
using WikiFunctions;
using WikiFunctions.Plugin;
using WikiFunctions.Parse;
using WikiFunctions.Lists;
using WikiFunctions.Logging;
using WikiFunctions.Properties;
using WikiFunctions.Browser;
using WikiFunctions.Controls;
using System.Collections.Specialized;
using WikiFunctions.Background;
using System.Security.Permissions;
using WikiFunctions.Controls.Lists;
using AutoWikiBrowser.Plugins;

namespace AutoWikiBrowser
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public sealed partial class MainForm : Form, IAutoWikiBrowser
    { // this class needs to be public, otherwise we get an exception which recommends setting ComVisibleAttribute to true (which we've already done)
        #region Fields
        private static Splash splash = new Splash();
        private static WikiFunctions.Profiles.AWBProfilesForm profiles;

        private static bool Abort;

        private static string LastArticle = "";
        private static string mSettingsFile = "";
        private static string mSettingsFileDisplay = "";
        private static string LastMove = "";
        private static string LastDelete = "";
        private static string LastProtect = "";

        private static int oldselection;
        private static int retries;

        private static bool PageReload;
        private static int mnudges;
        private static int sameArticleNudges;

        private static HideText RemoveText = new HideText(false, true, false);
        private static List<string> noParse = new List<string>();
        private static FindandReplace findAndReplace = new FindandReplace();
        private static SubstTemplates substTemplates = new SubstTemplates();
        private static RegExTypoFix RegexTypos;
        private static SkipOptions Skip = new SkipOptions();
        private static WikiFunctions.MWB.ReplaceSpecial replaceSpecial =
            new WikiFunctions.MWB.ReplaceSpecial();
        private static Parsers parsers;
        private static TimeSpan StartTime =
            new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        private static List<string> RecentList = new List<string>();
        private static CustomModule cModule = new CustomModule();
        private static ExternalProgram externalProgram = new ExternalProgram();
        internal static RegexTester regexTester = new RegexTester();
        private static bool userTalkWarningsLoaded;
        private static Regex userTalkTemplatesRegex;
        private static bool mErrorGettingLogInStatus;
        private static bool skippable = true;
        private static FormWindowState LastState = FormWindowState.Normal; // doesn't look like we can use RestoreBounds for this - any other built in way?

        private ListComparer lc;
        private ListSplitter splitter;

        private static Help h = new Help();

        private WikiDiff diff = new WikiDiff();

        #endregion

        #region Constructor and MainForm load/resize
        public MainForm()
        {
            splash.Show(this);
            RightToLeft = System.Globalization.CultureInfo.CurrentCulture.TextInfo.IsRightToLeft
                ? RightToLeft.Yes : RightToLeft.No;
            InitializeComponent();
            splash.SetProgress(5);
            try
            {
                Icon = WikiFunctions.Properties.Resources.AWBIcon;
                lblUserName.Alignment = ToolStripItemAlignment.Right;
                lblProject.Alignment = ToolStripItemAlignment.Right;
                lblTimer.Alignment = ToolStripItemAlignment.Right;
                lblEditsPerMin.Alignment = ToolStripItemAlignment.Right;
                lblIgnoredArticles.Alignment = ToolStripItemAlignment.Right;
                lblEditCount.Alignment = ToolStripItemAlignment.Right;

                btntsShowHide.Image = Resources.Showhide;
                btntsShowHideParameters.Image = Resources.Showhideparameters;
                btntsSave.Image = Resources.Save;
                btntsIgnore.Image = Resources.RightArrow;
                btntsStop.Image = Resources.Stop;
                btntsPreview.Image = Resources.preview;
                btntsChanges.Image = Resources.changes;
                btntsFalsePositive.Image = Resources.RollBack;
                btntsStart.Image = Resources.Run;
                btntsDelete.Image = Resources.Vista_trashcan_empty;

                splash.SetProgress(10);
                int stubcount = 500;
                bool catkey = false;
                try
                {
                    stubcount = AutoWikiBrowser.Properties.Settings.Default.StubMaxWordCount;
                    catkey = AutoWikiBrowser.Properties.Settings.Default.AddHummanKeyToCats;
                    parsers = new Parsers(stubcount, catkey);
                }
                catch (Exception ex)
                {
                    parsers = new Parsers();
                    ErrorHandler.Handle(ex);
                }

                toolStripComboOnLoad.SelectedIndex = 0;
                cmboCategorise.SelectedIndex = 0;
                cmboImages.SelectedIndex = 0;
                lblStatusText.AutoSize = true;
                lblBotTimer.AutoSize = true;

                Variables.User.UserNameChanged += UpdateUserName;
                Variables.User.BotStatusChanged += UpdateBotStatus;
                Variables.User.AdminStatusChanged += UpdateAdminStatus;
                Variables.User.WikiStatusChanged += UpdateWikiStatus;

                Variables.User.webBrowserLogin.DocumentCompleted += web4Completed;
                Variables.User.webBrowserLogin.Navigating += web4Starting;

                webBrowserEdit.Deleted += CaseWasDelete;
                webBrowserEdit.Loaded += CaseWasLoad;
                webBrowserEdit.Saved += CaseWasSaved;
                webBrowserEdit.None += CaseWasNull;
                webBrowserEdit.Fault += StartDelayedRestartTimer;
                webBrowserEdit.StatusChanged += UpdateWebBrowserStatus;
                listMaker1.txtSelectSource.ContextMenuStrip = mnuMakeFromTextBox;
                listMaker1.BusyStateChanged += SetProgressBar;
                listMaker1.NoOfArticlesChanged += UpdateButtons;
                listMaker1.StatusTextChanged += UpdateListStatus;
                listMaker1.cmboSourceSelect.SelectedIndexChanged += new EventHandler(ListMakerSourceSelectHandler);
                //Text = "AutoWikiBrowser - Default.xml";
                splash.SetProgress(15);

                WikiFunctions.Profiles.AWBProfiles.ResetTempPassword();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        internal string SettingsFile
        {
            set
            {
                mSettingsFile = value;
                mSettingsFileDisplay = Program.Name;
                if (!string.IsNullOrEmpty(value))
                    mSettingsFileDisplay += " - " + value.Remove(0, value.LastIndexOf("\\") + 1);
                this.Text = SettingsFileDisplay;

                if (SettingsFileDisplay.Length > 64)
                    ntfyTray.Text = SettingsFileDisplay.Substring(0, 63); // 64 char limit
                else
                    ntfyTray.Text = SettingsFileDisplay;
            }
            get { return mSettingsFile; }
        }

        private static string SettingsFileDisplay
        {
            get { return mSettingsFileDisplay; }
        }

        int userProfileToLoad = -1;
        public int ProfileToLoad
        {
            set { userProfileToLoad = value; }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            StatusLabelText = "Initialising...";
            splash.SetProgress(20);
            Variables.MainForm = this;
            lblOnlyBots.BringToFront();
            Updater.UpdateAWB(new Tools.SetProgress(splash.SetProgress)); // progress 22-29 in UpdateAWB()
            splash.SetProgress(30);

            Program.MyTrace.LS = loggingSettings1;

            try
            {
                //check that we are not using an old OS. 98 seems to mangled some unicode
                if (Environment.OSVersion.Version.Major < 5)
                {
                    MessageBox.Show("You appear to be using an older operating system, this software may have trouble with some unicode fonts on operating systems older than Windows 2000, the start button has been disabled.", "Operating system", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetStartButton(false);
                }
                else
                    listMaker1.MakeListEnabled = true;

                webBrowserDiff.Navigate("about:blank");
                webBrowserDiff.ObjectForScripting = this;

                splash.SetProgress(35);
                if (Properties.Settings.Default.LogInOnStart)
                    CheckStatus(false);

                LogControl1.Initialise(listMaker1);

                if (Properties.Settings.Default.WindowLocation != null)
                    this.Location = Properties.Settings.Default.WindowLocation;

                if (Properties.Settings.Default.WindowSize != null)
                    this.Size = Properties.Settings.Default.WindowSize;

                this.WindowState = Properties.Settings.Default.WindowState;

                Debug();
                Plugin.LoadPluginsStartup(this, splash); // progress 65-79 in LoadPlugins()

                LoadPrefs(); // progress 80-85 in LoadPrefs()

                splash.SetProgress(86);
                UpdateButtons(null, null);
                splash.SetProgress(88);
                LoadRecentSettingsList(); // progress 89-94 in LoadRecentSettingsList()
                splash.SetProgress(95);

                WikiStatusResult res = Variables.User.CheckEnabled();
                if (res == WikiStatusResult.OldVersion)
                    OldVersion();
                else if (res == WikiStatusResult.Error)
                {
                    lblUserName.BackColor = Color.Red;
                    MessageBox.Show(this, "Cannot load version check page from Wikipedia. "
                        + "Please verify that you're connected to Internet.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (userProfileToLoad != -1)
                {
                    profiles = new WikiFunctions.Profiles.AWBProfilesForm(webBrowserEdit);
                    profiles.LoadProfile += LoadProfileSettings;
                    profiles.login(userProfileToLoad);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }

            UsageStats.Initialise();

            StatusLabelText = "";
            splash.SetProgress(100);
            splash.Close();

#if DEBUG && INSTASTATS
            UsageStats.Do(false);
#endif
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                if (Minimize) this.Visible = false;
            }
            else
                LastState = this.WindowState; // remember if maximised or normal so can restore same when dbl click tray icon
        }
        #endregion

        #region Properties

        private ArticleEX stredittingarticle; // = new ArticleWithLogging(""); 
        internal ArticleEX TheArticle
        {
            get { return stredittingarticle; }
            private set { stredittingarticle = value; }
        }

        /// <summary>
        /// Is AWB running in Bot Mode
        /// </summary>
        private bool BotMode
        {
            get { return chkAutoMode.Checked; }
            set { chkAutoMode.Checked = value; }
        }

        private bool bLowThreadPriority;
        private bool LowThreadPriority
        {
            get { return bLowThreadPriority; }
            set
            {
                bLowThreadPriority = value;
                if (value)
                    Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                else
                    Thread.CurrentThread.Priority = ThreadPriority.Normal;
            }
        }

        private bool bFlash;
        private bool Flash
        {
            get { return bFlash; }
            set { bFlash = value; }
        }

        private bool bBeep;
        private bool Beep
        {
            get { return bBeep; }
            set { bBeep = value; }
        }

        private bool bMinimize;
        /// <summary>
        /// Returns True if AWB should be minimised to the system tray; False if it should minimise to the taskbar
        /// </summary>
        private bool Minimize
        {
            get { return bMinimize; }
            set { bMinimize = value; }
        }

        private decimal dTimeOut = 30;
        private decimal TimeOut
        {
            get { return dTimeOut; }
            set
            {
                dTimeOut = value;
                webBrowserEdit.TimeoutLimit = int.Parse(value.ToString());
            }
        }

        private bool bSaveArticleList = true;
        private bool SaveArticleList
        {
            get { return bSaveArticleList; }
            set { bSaveArticleList = value; }
        }

        private bool bAutoSaveEdit;
        private bool AutoSaveEditBoxEnabled
        {
            get { return bAutoSaveEdit; }
            set { bAutoSaveEdit = value; }
        }

        private string sAutoSaveEditFile = Application.StartupPath + "\\Edit Box.txt";
        private string AutoSaveEditBoxFile
        {
            get { return sAutoSaveEditFile; }
            set { sAutoSaveEditFile = value; }
        }

        private bool bSupressUsingAWB;
        private bool SupressUsingAWB
        {
            get { return bSupressUsingAWB; }
            set { bSupressUsingAWB = value; }
        }

        private decimal dAutoSaveEditPeriod = 60;
        private decimal AutoSaveEditBoxPeriod
        {
            get { return dAutoSaveEditPeriod; }
            set { dAutoSaveEditPeriod = value; EditBoxSaveTimer.Interval = int.Parse((value * 1000).ToString()); }
        }

        internal string StatusLabelText
        {
            get { return lblStatusText.Text; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    lblStatusText.Text = Program.Name + " " + Program.VersionString;
                else
                    lblStatusText.Text = value;
                Application.DoEvents();
            }
        }
        #endregion

        #region MainProcess

        /// <summary>
        /// checks if we are still logged in
        /// asks to relogin if needed
        /// </summary>
        private bool CheckLoginStatus()
        {
            if (webBrowserEdit.UserName != Variables.User.Name)
            {
                MessageBox.Show("You've been logged off, probably due to loss of session data.\r\n" +
                    "Please relogin.", "Logged off", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Stop();
                CheckStatus(true);
                return false;
            }
            return true;
        }

        private void Start()
        {
            try
            {
                Tools.WriteDebug(this.Name, "Starting");

                CanShutdown();

                //check edit summary
                txtEdit.Enabled = true;
                SetEditToolBarEnabled(true);
                txtEdit.Text = "";
                webBrowserEdit.BringToFront();
                if (string.IsNullOrEmpty(cmboEditSummary.Text) && Plugin.Items.Count == 0)
                    MessageBox.Show("Please enter an edit summary.", "Edit summary", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                if (!cmboEditSummary.Items.Contains(cmboEditSummary.Text))
                    cmboEditSummary.Items.Add(cmboEditSummary.Text);

                StopDelayedRestartTimer();
                DisableButtons();
                //TheArticle.EditSummary = "";
                skippable = true;
                txtEdit.Clear();

                if (webBrowserEdit.IsBusy)
                    webBrowserEdit.Stop();

                if (webBrowserEdit.Document != null)
                    webBrowserEdit.Document.Write("");

                //check we are logged in
                if (!Variables.User.WikiStatus && !CheckStatus(false))
                    return;

                ArticleInfo(true);

                if (listMaker1.NumberOfArticles < 1)
                {
                    webBrowserEdit.Busy = false;
                    StopSaveInterval(null, null);
                    lblTimer.Text = "";
                    StatusLabelText = "No articles in list, you need to use the Make list";
                    this.Text = Program.Name;
                    webBrowserEdit.Document.Write("");
                    listMaker1.MakeListEnabled = true;
                    return;
                }
                else
                    webBrowserEdit.Busy = true;

                TheArticle = new ArticleEX(listMaker1.SelectedArticle().Name);
                ErrorHandler.CurrentArticle = TheArticle.Name;

                NewHistory();

                if (!Tools.IsValidTitle(TheArticle.Name))
                {
                    SkipPage("Invalid page title");
                    return;
                }
                if (BotMode)
                    NudgeTimer.StartMe();

                EditBoxSaveTimer.Enabled = AutoSaveEditBoxEnabled;

                //if (dlg != null && dlg.AutoProtectAll)
                //    webBrowserEdit.ProtectPage(TheArticle.Name, dlg.Summary, dlg.EditProtectionLevel, dlg.MoveProtectionLevel, dlg.ProtectExpiry);

                //Navigate to edit page
                webBrowserEdit.LoadEditPage(TheArticle.Name);
            }
            catch (Exception ex)
            {
                Tools.WriteDebug(this.Name, "Start() error: " + ex.Message);
                StartDelayedRestartTimer(null, null);
            }

            if (Program.MyTrace.StoppedWithConfigError)
            {
                try
                { Program.MyTrace.ValidateUploadProfile(); }
                catch (Exception ex)
                { Program.MyTrace.ConfigError(ex); }
            }
        }

        private void CaseWasDelete(object sender, EventArgs e)
        {
            listMaker1.Remove(TheArticle);
            Start();
        }

        private void CaseWasLoad(object sender, EventArgs e)
        {
            if (!LoadSuccess())
                return;

            if (!CheckLoginStatus()) return;

            if (Program.MyTrace.HaveOpenFile)
                Program.MyTrace.WriteBulletedLine("AWB started processing", true, true, true);
            else
                Program.MyTrace.Initialise();

            string strTemp = webBrowserEdit.GetArticleText();

            this.Text = SettingsFileDisplay + " - " + TheArticle.Name;

            //check for redirect
            if (bypassRedirectsToolStripMenuItem.Checked && Tools.IsRedirect(strTemp) && !PageReload)
            {
                // Warning: Creating an ArticleEX causes a new AWBLogListener to be created and it becomes the active listener in MyTrace; be careful we're writing to the correct log listener
                ArticleEX redirect = new ArticleEX(Tools.RedirectTarget(strTemp));

                if (filterOutNonMainSpaceToolStripMenuItem.Checked && (redirect.NameSpaceKey != 0))
                {
                    listMaker1.Remove(TheArticle); // or we get stuck in a loop
                    TheArticle = redirect; // if we didn't do this, we were writing the SkipPage info to the AWBLogListener belonging to the object redirect and resident in the MyTrace collection, but then attempting to add TheArticle's log listener to the logging tab
                    SkipPage("Article is not in mainspace");
                    return;
                }

                if (redirect.Name == TheArticle.Name)
                {//ignore recursive redirects
                    TheArticle = redirect;
                    SkipPage("Recursive redirect");
                    return;
                }

                listMaker1.ReplaceArticle(TheArticle, redirect);
                TheArticle = new ArticleEX(redirect.Name);

                webBrowserEdit.LoadEditPage(redirect.Name);
                return;
            }

            if (webBrowserEdit.Document.Body.InnerHtml.Contains("readOnly"))
            {
                if (!Variables.User.IsAdmin)
                {
                    NudgeTimer.Stop();
                    SkipPage("Page is protected");
                    return;
                }
            }

            TheArticle.OriginalArticleText = strTemp;

            int.TryParse(webBrowserEdit.GetScriptingVar("wgCurRevisionId"), out ErrorHandler.CurrentRevision);

            if (PageReload)
            {
                PageReload = false;
                GetDiff();
                return;
            }

            if (chkSkipIfContains.Checked && TheArticle.SkipIfContains(txtSkipIfContains.Text,
                chkSkipIsRegex.Checked, chkSkipCaseSensitive.Checked, true))
            {
                SkipPage("Article contains: " + txtSkipIfContains.Text);
                return;
            }

            if (chkSkipIfNotContains.Checked && TheArticle.SkipIfContains(txtSkipIfNotContains.Text,
                chkSkipIsRegex.Checked, chkSkipCaseSensitive.Checked, false))
            {
                SkipPage("Article does not contain: " + txtSkipIfNotContains.Text);
                return;
            }

            if (!Skip.SkipIf(TheArticle.OriginalArticleText))
            {
                SkipPage("skipIf custom code");
                return;
            }

            //check not in use
            if (TheArticle.IsInUse)
                if (chkSkipIfInuse.Checked)
                {
                    SkipPage("Page contains {{inuse}}");
                    return;
                }
                else if (!BotMode)
                    MessageBox.Show("This page has the \"Inuse\" tag, consider skipping it");

            ErrorHandler.CurrentArticle = "";

            if (!doNotAutomaticallyDoAnythingToolStripMenuItem.Checked)
            {
                StatusLabelText = "Processing page";
                Application.DoEvents();

                ProcessPage();

                UpdateWebBrowserStatus(null, null);

                if (!Abort)
                {
                    if (TheArticle.SkipArticle)
                    {
                        SkipPageReasonAlreadyProvided(); // Don't send a reason; ProcessPage() should already have logged one
                        return;
                    }

                    if (skippable && chkSkipNoChanges.Checked &&
    TheArticle.ArticleText == TheArticle.OriginalArticleText)
                    {
                        SkipPage("No change");
                        return;
                    }

                }
                else if (chkSkipWhitespace.Checked &&
                    (string.Compare(Regex.Replace(TheArticle.OriginalArticleText, @"\s+", @""), Regex.Replace(TheArticle.ArticleText, @"\s+", @"")) == 0))
                {
                    SkipPage("Only whitespace changed");
                    return;
                }
            }

            webBrowserEdit.SetArticleText(TheArticle.ArticleText);
            TheArticle.SaveSummary();
            txtEdit.Text = TheArticle.ArticleText;

            //Update statistics and alerts
            ArticleInfo(false);

            if (!Abort)
            {
                if (toolStripComboOnLoad.SelectedIndex == 0)
                    GetDiff();
                else if (toolStripComboOnLoad.SelectedIndex == 1)
                    GetPreview();
                else if (toolStripComboOnLoad.SelectedIndex == 2)
                {
                    if (BotMode)
                    {
                        StartDelayedAutoSaveTimer();
                        return;
                    }

                    Bleepflash();

                    this.Focus();
                    txtEdit.Focus();
                    txtEdit.SelectionLength = 0;

                    EnableButtons();
                }
                SetWatchButton(webBrowserEdit.IsWatched());

                if (focusAtEndOfEditTextBoxToolStripMenuItem.Checked)
                {
                    txtEdit.Select(txtEdit.Text.Length, 0);
                    txtEdit.ScrollToCaret();
                }
            }
            else
            {
                EnableButtons();
                Abort = false;
            }
        }

        private void Bleepflash()
        {
            if (!this.ContainsFocus)
            {
                if (Flash) Tools.FlashWindow(this);
                if (Beep) Tools.Beep1();
            }
        }

        private bool LoadSuccess()
        {
            try
            {
                string HTML = null;
                if (webBrowserEdit.Document != null && webBrowserEdit.Document.Body != null)
                    HTML = webBrowserEdit.Document.Body.InnerHtml;

                if (string.IsNullOrEmpty(HTML) || HTML.Contains("The Wikipedia database is temporarily in read-only mode for the following reason"))
                {//http://en.wikipedia.org/wiki/MediaWiki:Readonlytext

                    if (retries < 10)
                    {
                        StartDelayedRestartTimer(null, null);
                        retries++;
                        Start();
                        return false;
                    }
                    else
                    {
                        retries = 0;
                        if (!string.IsNullOrEmpty(HTML)) SkipPage("Database is locked, tried 10 times");
                        else
                        {
                            MessageBox.Show("Loading edit page failed after 10 retries. Processing stopped.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Stop();
                        }
                        return false;
                    }
                }
                else if (HTML.Contains("Sorry! We could not process your edit due to a loss of session data. Please try again. If it still doesn't work, try logging out and logging back in."))
                {
                    Save();
                    return false;
                }

                //check we are still logged in
                try
                {
                    if (!webBrowserEdit.GetLogInStatus())
                    {
                        Variables.User.LoggedIn = false;
                        NudgeTimer.Stop();
                        Start();
                        return false;
                    }
                }
                catch
                {
                    // No point writing to log listener I think, as it gets destroyed when we Stop?
                    if (mErrorGettingLogInStatus)
                    {
                        MessageBox.Show("Error getting login status", "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        // Stop AWB and reset the state variable:
                        Stop(); mErrorGettingLogInStatus = false;
                    }
                    else
                    {
                        mErrorGettingLogInStatus = true; // prevent start / error / start / error loop
                        Stop();
                        Start();
                    }
                    return false;
                }

                mErrorGettingLogInStatus = false;

                if (webBrowserEdit.NewMessage)
                {//check if we have any messages
                    NudgeTimer.Stop();
                    Variables.User.WikiStatus = false;
                    UpdateButtons(null, null);
                    webBrowserEdit.Document.Write("");
                    this.Focus();

                    using (DlgTalk DlgTalk = new DlgTalk())
                    {
                        if (DlgTalk.ShowDialog() == DialogResult.Yes)
                            Tools.OpenUserTalkInBrowser();
                        else
                            Process.Start("iexplore", Variables.GetUserTalkURL());
                    }
                    return false;
                }
                if (!webBrowserEdit.HasArticleTextBox)
                {
                    if (!BotMode)
                    {
                        MessageBox.Show("There was a problem loading the page. Re-start the process", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    StatusLabelText = "There was a problem loading the page. Re-starting.";
                    StartDelayedRestartTimer(null, null);
                    return false;
                }

                bool wpTextbox1IsNull = (webBrowserEdit.Document.GetElementById("wpTextbox1").InnerText == null);

                if (wpTextbox1IsNull && chkSkipNonExistent.Checked)
                {//check if it is a non-existent page, if so then skip it automatically.
                    SkipPage("Non-existent page");
                    return false;
                }
                if (!wpTextbox1IsNull && chkSkipExistent.Checked)
                {
                    SkipPage("Existing page");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
            NudgeTimer.Reset();
            return true;
        }

        private void CaseWasDiff()
        {
            if (BotMode)
            {
                StartDelayedAutoSaveTimer();
                return;
            }

            Bleepflash();

            this.Focus();
            txtEdit.Focus();
            txtEdit.SelectionLength = 0;

            EnableButtons();
        }

        static readonly Regex spamUrlRegex = new Regex("<p>The following link has triggered our spam protection filter:<strong>(.*?)</strong><br/?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private void CaseWasSaved(object sender, EventArgs e)
        {
            if (webBrowserEdit.Document.Body.InnerHtml.Contains("<H1 class=firstHeading>Edit conflict: "))
            {//if session data is lost, if data is lost then save after delay with tmrAutoSaveDelay
                MessageBox.Show("There has been an Edit Conflict. AWB will now re-apply its changes on the updated page. \n\r Please re-review the changes before saving. Any Custom edits will be lost, and have to be re-added manually.", "Edit Conflict");
                NudgeTimer.Stop();
                Start();
                return;
            }
            else if (!BotMode && webBrowserEdit.Document.Body.InnerHtml.Contains("<DIV id=spamprotected>"))
            {//check edit wasn't blocked due to spam filter
                if (!chkSkipSpamFilter.Checked)
                {
                    Match m = spamUrlRegex.Match(webBrowserEdit.Document.Body.InnerHtml);

                    string messageBoxText = "Edit has been blocked by spam blacklist.\r\n";

                    if (m.Success)
                        messageBoxText += "Spam URL: " + m.Groups[1].Value.Trim() + "\r\n";

                    if (MessageBox.Show(messageBoxText += "Try and edit again?", "Spam blacklist", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        Start();
                    else
                        SkipPage("Edit blocked by spam protection filter");
                }
                else
                    SkipPage("Edit blocked by spam protection filter");

                return;
            }
            else if (webBrowserEdit.Document.Body.InnerHtml.Contains("<DIV CLASS=PREVIEWNOTE"))
            {//if session data is lost, if data is lost then save after delay with tmrAutoSaveDelay
                StartDelayedRestartTimer(null, null);
                return;
            }

            //lower restart delay
            if (intRestartDelay > 5)
                intRestartDelay -= 1;

            NumberOfEdits++;

            LastArticle = "";
            listMaker1.Remove(TheArticle);
            NudgeTimer.Stop();
            sameArticleNudges = 0;
            if (EditBoxTab.SelectedTab == tpHistory)
                EditBoxTab.SelectedTab = tpEdit;
            LogControl1.AddLog(false, TheArticle.LogListener);

            if (listMaker1.Count == 0 && AutoSaveEditBoxEnabled)
                EditBoxSaveTimer.Enabled = false;
            retries = 0;
            Start();
        }

        private void CaseWasNull(object sender, EventArgs e)
        {
            if (webBrowserEdit.Document.Body.InnerHtml.Contains("<B>You have successfully signed in to Wikipedia as"))
            {
                StatusLabelText = "Signed in, now re-starting";

                if (!Variables.User.WikiStatus)
                    CheckStatus(false);
            }
        }

        private void SkipPageReasonAlreadyProvided()
        {
            try
            {
                //reset timer.
                NumberOfIgnoredEdits++;
                StopDelayedAutoSaveTimer();
                NudgeTimer.Stop();
                listMaker1.Remove(TheArticle);
                sameArticleNudges = 0;
                LogControl1.AddLog(true, TheArticle.LogListener);
                retries = 0;
                Start();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        private void SkipPage(string reason)
        {
            switch (reason)
            {
                case "user":
                    TheArticle.Trace.UserSkipped();
                    break;

                case "plugin":
                    TheArticle.Trace.PluginSkipped();
                    break;

                default:
                    if (!string.IsNullOrEmpty(reason))
                        TheArticle.Trace.AWBSkipped(reason);
                    break;
            }

            SkipPageReasonAlreadyProvided();
        }

        private void ProcessPage()
        {
            ProcessPage(TheArticle);
        }

        private void ProcessPage(ArticleEX theArticle)
        {
            bool process = true;

#if DEBUG
            Variables.Profiler.Start("ProcessPage(\"" + theArticle.Name + "\")");
#endif

            try
            {
                if (noParse.Contains(theArticle.Name))
                    process = false;

                if (!ignoreNoBotsToolStripMenuItem.Checked &&
                    !Parsers.CheckNoBots(theArticle.ArticleText, Variables.User.Name))
                {
                    theArticle.AWBSkip("Restricted by {{bots}}/{{nobots}}");
                    return;
                }

                Variables.Profiler.Profile("Initial skip checks");

                if (cModule.ModuleEnabled && cModule.Module != null)
                {
                    theArticle.SendPageToCustomModule(cModule.Module);
                    if (theArticle.SkipArticle) return;
                }

                Variables.Profiler.Profile("Custom module");

                if (externalProgram.ModuleEnabled)
                {
                    theArticle.SendPageToCustomModule(externalProgram);
                    if (theArticle.SkipArticle) return;
                }

                Variables.Profiler.Profile("External Program");

                if (Plugin.Items.Count > 0)
                {
                    foreach (KeyValuePair<string, IAWBPlugin> a in Plugin.Items)
                    {
                        theArticle.SendPageToPlugin(a.Value, this);
                        if (theArticle.SkipArticle) return;
                    }
                }
                Variables.Profiler.Profile("Plugins");

                // unicodify whole article
                if (chkUnicodifyWhole.Checked && process)
                {
                    theArticle.HideMoreText(RemoveText);
                    Variables.Profiler.Profile("HideMoreText");

                    theArticle.Unicodify(Skip.SkipNoUnicode, parsers);
                    Variables.Profiler.Profile("Unicodify");

                    theArticle.UnHideMoreText(RemoveText);
                    Variables.Profiler.Profile("UnHideMoreText");
                }

                // find and replace before general fixes
                if (chkFindandReplace.Checked && !findAndReplace.AfterOtherFixes)
                {
                    theArticle.PerformFindAndReplace(findAndReplace, substTemplates, replaceSpecial,
                        chkSkipWhenNoFAR.Checked);
                    if (theArticle.SkipArticle) return;
                }

                Variables.Profiler.Profile("F&R");

                // RegexTypoFix
                if (chkRegExTypo.Checked && RegexTypos != null && !BotMode && !Tools.IsTalkPage(theArticle.NameSpaceKey))
                {
                    theArticle.PerformTypoFixes(RegexTypos, chkSkipIfNoRegexTypo.Checked);
                    Variables.Profiler.Profile("Typos");
                    if (theArticle.SkipArticle) return;
                }

                // replace/add/remove categories
                if (cmboCategorise.SelectedIndex != 0)
                {
                    theArticle.Categorisation((WikiFunctions.Options.CategorisationOptions)
                        cmboCategorise.SelectedIndex, parsers, chkSkipNoCatChange.Checked, txtNewCategory.Text.Trim(),
                        txtNewCategory2.Text.Trim());
                    if (theArticle.SkipArticle) return;
                    else if (!chkGeneralFixes.Checked) theArticle.AWBChangeArticleText("Fix categories", Parsers.FixCategories(theArticle.ArticleText), true);
                }

                Variables.Profiler.Profile("Categories");

                if (theArticle.CanDoGeneralFixes)
                {
                    // auto tag
                    if (process && chkAutoTagger.Checked)
                    {
                        theArticle.AutoTag(parsers, Skip.SkipNoTag);
                        if (theArticle.SkipArticle) return;
                    }

                    Variables.Profiler.Profile("Auto-tagger");

                    if (process && chkGeneralFixes.Checked)
                    {
                        theArticle.PerformGeneralFixes(parsers, RemoveText, Skip, replaceReferenceTagsToolStripMenuItem.Checked);
                    }
                }
                else if (process && chkGeneralFixes.Checked && theArticle.NameSpaceKey == 3)
                {
                    theArticle.HideText(RemoveText);

                    Variables.Profiler.Profile("HideText");

                    if (!userTalkWarningsLoaded)
                    {
                        LoadUserTalkWarnings();
                        Variables.Profiler.Profile("loadUserTalkWarnings");
                    }

                    theArticle.AWBChangeArticleText("Subst user talk warnings",
                        Parsers.SubstUserTemplates(theArticle.ArticleText, theArticle.Name, userTalkTemplatesRegex), true);

                    Variables.Profiler.Profile("SubstUserTemplates");

                    theArticle.UnHideText(RemoveText);

                    Variables.Profiler.Profile("UnHideText");
                }

                // find and replace after general fixes
                if (chkFindandReplace.Checked && findAndReplace.AfterOtherFixes)
                {
                    theArticle.PerformFindAndReplace(findAndReplace, substTemplates, replaceSpecial,
                        chkSkipWhenNoFAR.Checked);

                    Variables.Profiler.Profile("F&R (2nd)");

                    if (theArticle.SkipArticle) return;
                }

                // append/prepend text
                if (chkAppend.Checked)
                {
                    // customized number of newlines
                    String newlines = "";
                    for (int i = 0; i < (int)udNewlineChars.Value; i++)
                        newlines += "\r\n";

                    if (rdoAppend.Checked)
                        theArticle.AWBChangeArticleText("Appended your message",
                            theArticle.ArticleText + newlines + txtAppendMessage.Text, false);
                    else
                        theArticle.AWBChangeArticleText("Prepended your message",
                            txtAppendMessage.Text + newlines + theArticle.ArticleText, false);
                }

                // replace/remove/comment out images
                if (cmboImages.SelectedIndex != 0)
                {
                    theArticle.UpdateImages((WikiFunctions.Options.ImageReplaceOptions)cmboImages.SelectedIndex,
                        parsers, txtImageReplace.Text, txtImageWith.Text, chkSkipNoImgChange.Checked);
                    if (theArticle.SkipArticle) return;
                }

                Variables.Profiler.Profile("Images");

                // disambiguation
                if (chkEnableDab.Checked && txtDabLink.Text.Trim().Length > 0 &&
                    txtDabVariants.Text.Trim().Length > 0)
                {
                    if (theArticle.Disambiguate(txtDabLink.Text.Trim(), txtDabVariants.Lines, BotMode,
                        (int)udContextChars.Value, chkSkipNoDab.Checked))
                    {
                        if (theArticle.SkipArticle) return;
                    }
                    else
                    {
                        Stop();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
                theArticle.Trace.AWBSkipped("Exception:" + ex.Message);
            }
            finally
            {
                Variables.Profiler.Flush();
            }
        }

        private void GetDiff()
        {
            try
            {
                webBrowserDiff.BringToFront();
                webBrowserDiff.Document.OpenNew(false);

                if (TheArticle.OriginalArticleText == txtEdit.Text)
                {
                    webBrowserDiff.Document.Write(@"<h2 style='padding-top: .5em;
padding-bottom: .17em;
border-bottom: 1px solid #aaa;
font-size: 150%;'>No changes</h2><p>Press the ""Ignore"" button below to skip to the next page.</p>");
                }
                else
                {
                    webBrowserDiff.Document.Write("<html><head>" +
                        WikiDiff.DiffHead() + @"</head><body>" + WikiDiff.TableHeader +
                        diff.GetDiff(TheArticle.OriginalArticleText, txtEdit.Text, 2) +
                        @"</table></body></html>");
                }

                CaseWasDiff();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        private void GetPreview()
        {
            webBrowserEdit.BringToFront();
            webBrowserEdit.SetArticleText(txtEdit.Text);

            LastArticle = txtEdit.Text;

            skippable = false;
            webBrowserEdit.ShowPreview();
            EnableButtons();
            Bleepflash();
        }

        private void Save()
        {
            DisableButtons();
            if (txtEdit.Text.Length > 0)
                SaveArticle();
            else if (MessageBox.Show("Do you really want to save a blank page?", "Save?",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                SaveArticle();
            else
                SkipPage("Nothing to save - blank page");
        }

        private void SaveArticle()
        {
            webBrowserEdit.BringToFront();

            //remember article text in case it is lost, this is set to "" again when the article title is removed
            LastArticle = txtEdit.Text;

            if (showTimerToolStripMenuItem.Checked)
            {
                StopSaveInterval(null, null);
                Ticker += SaveInterval;
            }

            try
            {
                SetCheckBoxes();

                webBrowserEdit.SetArticleText(txtEdit.Text);
                webBrowserEdit.Save();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        #endregion

        #region extra stuff

        #region Diff
        public void UndoChange(int left, int right)
        {
            try
            {
                txtEdit.Text = diff.UndoChange(left, right);
                GetDiff();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
                return;
            }
        }

        public void UndoDeletion(int left, int right)
        {
            try
            {
                txtEdit.Text = diff.UndoDeletion(left, right);
                GetDiff();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
                return;
            }
        }

        public void UndoAddition(int right)
        {
            try
            {
                txtEdit.Text = diff.UndoAddition(right);
                GetDiff();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
                return;
            }
        }

        public void GoTo(int destLine)
        {
            try
            {
                EditBoxTab.SelectedTab = tpEdit;
                txtEdit.Select();
                if (destLine < 0) return;

                MatchCollection mc = Regex.Matches(txtEdit.Text, "\r\n");
                destLine = Math.Min(mc.Count, destLine);

                if (destLine == 0) txtEdit.Select(0, 0);
                else
                    txtEdit.Select(mc[destLine - 1].Index + 2, 0);
                txtEdit.ScrollToCaret();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }
        #endregion

        private void PanelShowHide()
        {
            if (panel1.Visible) panel1.Hide();
            else panel1.Show();

            SetBrowserSize();
        }

        Point oldPosition = new Point();
        Size oldSize = new Size();
        private void ParametersShowHide()
        {
            enlargeEditAreaToolStripMenuItem.Checked = !enlargeEditAreaToolStripMenuItem.Checked;
            if (groupBox2.Visible)
            {
                btntsShowHideParameters.Image = Resources.Showhideparameters2;

                oldPosition = EditBoxTab.Location; ;
                EditBoxTab.Location = new Point(groupBox2.Location.X, groupBox2.Location.Y - 5);

                oldSize = EditBoxTab.Size;
                EditBoxTab.Size = new Size((EditBoxTab.Size.Width + MainTab.Size.Width + groupBox2.Size.Width + 8), EditBoxTab.Size.Height);
            }
            else
            {
                btntsShowHideParameters.Image = Resources.Showhideparameters;

                EditBoxTab.Location = oldPosition;
                EditBoxTab.Size = oldSize;
            }
            groupBox2.Visible = MainTab.Visible = !groupBox2.Visible;
        }

        private void UpdateUserName(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Variables.User.Name))
            {
                lblUserName.BackColor = Color.Red;
                lblUserName.Text = "User:";
            }
            else
            {
                lblUserName.BackColor = Color.Green;
                lblUserName.Text = Variables.User.Name;
            }
        }

        private void UpdateWebBrowserStatus(object sender, EventArgs e)
        {
            StatusLabelText = webBrowserEdit.Status;
        }

        private void UpdateListStatus(object sender, EventArgs e)
        {
            StatusLabelText = listMaker1.Status;
        }

        private void MainFormClosing(object sender, FormClosingEventArgs e)
        {
            ExitQuestion dlg = null;
            WebControl.Shutdown = true;

            Properties.Settings.Default.WindowState = this.WindowState;

            if (this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.WindowSize = this.Size;
                Properties.Settings.Default.WindowLocation = this.Location;
            }
            else
            {
                Properties.Settings.Default.WindowSize = this.RestoreBounds.Size;
                Properties.Settings.Default.WindowLocation = this.RestoreBounds.Location;
            }

            Properties.Settings.Default.Save();

            if (!AutoWikiBrowser.Properties.Settings.Default.DontAskForTerminate)
            {
                TimeSpan time = new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                time = time.Subtract(StartTime);
                dlg = new ExitQuestion(time, NumberOfEdits, "");
                dlg.ShowDialog();
                AutoWikiBrowser.Properties.Settings.Default.DontAskForTerminate = dlg.CheckBoxDontAskAgain;
            }

            if (AutoWikiBrowser.Properties.Settings.Default.DontAskForTerminate || dlg.DialogResult == DialogResult.OK)
            {
                // save user persistent settings
                AutoWikiBrowser.Properties.Settings.Default.Save();

                if (webBrowserEdit.IsBusy)
                    webBrowserEdit.Stop2();
                if (Variables.User.webBrowserLogin.IsBusy)
                    Variables.User.webBrowserLogin.Stop();

                SaveRecentSettingsList();
                UsageStats.Do(true);
            }
            else
            {
                e.Cancel = true;
                return;
            }
        }

        private void SetCheckBoxes()
        {
            if (webBrowserEdit.Document != null && webBrowserEdit.Document.Body.InnerHtml.Contains("wpMinoredit"))
            {
                // Warning: Plugins can call SetMinor and SetWatch, so only turn these *on* not off
                if (markAllAsMinorToolStripMenuItem.Checked) webBrowserEdit.SetMinor(true);
                if (addAllToWatchlistToolStripMenuItem.Checked) webBrowserEdit.SetWatch(true);
                webBrowserEdit.SetSummary(MakeSummary());
            }
        }

        private string MakeSummary()
        {
            string tag = cmboEditSummary.Text + TheArticle.SavedSummary;
            if (!BotMode || !chkSuppressTag.Checked || 
                (!Variables.IsWikimediaProject && !SupressUsingAWB))
            {
                if (tag.Length >= 255)
                    tag = tag.Substring(0, (255 - (Variables.SummaryTag.Length + 1)));

                tag += " " + Variables.SummaryTag;
            }

            return tag;
        }

        private void chkFindandReplace_CheckedChanged(object sender, EventArgs e)
        {
            btnMoreFindAndReplce.Enabled = chkFindandReplace.Checked;
            btnFindAndReplaceAdvanced.Enabled = chkFindandReplace.Checked;
            chkSkipWhenNoFAR.Enabled = chkFindandReplace.Checked;
            btnSubst.Enabled = chkFindandReplace.Checked;
        }

        private void cmboCategorise_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmboCategorise.SelectedIndex > 0)
            {
                txtNewCategory.Enabled = true;
                if (cmboCategorise.SelectedIndex == 1)
                {
                    label1.Text = "with Category:";
                    txtNewCategory2.Enabled = true;
                }
                else
                {
                    label1.Text = "";
                    txtNewCategory2.Enabled = false;
                }
                chkSkipNoCatChange.Enabled = true;
            }
            else
            {
                chkSkipNoCatChange.Enabled = false;
                label1.Text = "";
                txtNewCategory2.Enabled = false;
                txtNewCategory.Enabled = false;
            }
        }

        private void web4Completed(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            toolStripProgressBar1.MarqueeAnimationSpeed = 0;
            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
        }

        private void web4Starting(object sender, WebBrowserNavigatingEventArgs e)
        {
            toolStripProgressBar1.MarqueeAnimationSpeed = 100;
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
        }

        private void UpdateBotStatus(object sender, EventArgs e)
        {
            chkAutoMode.Enabled = Variables.User.IsBot;
            if (BotMode)
                BotMode = Variables.User.IsBot;
            lblOnlyBots.Visible = !Variables.User.IsBot;
        }

        private void UpdateAdminStatus(object sender, EventArgs e) { }

        private void UpdateWikiStatus(object sender, EventArgs e) { }

        private void chkAutoMode_CheckedChanged(object sender, EventArgs e)
        {
            if (BotMode)
            {
                SetBotModeEnabled(true);
                chkNudge.Checked = true;
                chkNudgeSkip.Checked = false; // default to false until such time as the settings file has this! mets! :P

                if (chkRegExTypo.Checked)
                {
                    MessageBox.Show("Auto cannot be used with RegExTypoFix.\r\nRegExTypoFix will now be turned off", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    chkRegExTypo.Checked = false;
                }
            }
            else
            {
                SetBotModeEnabled(false);
                StopDelayedAutoSaveTimer();
            }
        }

        private void SetBotModeEnabled(bool enabled)
        {
            label2.Enabled = chkSuppressTag.Enabled = nudBotSpeed.Enabled
            = lblAutoDelay.Enabled = btnResetNudges.Enabled = lblNudges.Enabled = chkNudge.Enabled
            = chkNudgeSkip.Enabled = chkNudge.Checked = chkShutdown.Enabled = enabled;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeSpan time = new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute,
                DateTime.Now.Second).Subtract(StartTime);
            new AboutBox(webBrowserEdit.Version.ToString(), time, NumberOfEdits).Show();
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        { CheckStatus(true); }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Would you really like to logout?", "Logout", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                chkAutoMode.Enabled = false;
                BotMode = false;
                lblOnlyBots.Visible = true;
                webBrowserEdit.BringToFront();
                webBrowserEdit.LoadLogOut();
                webBrowserEdit.Wait();
                Variables.User.UpdateWikiStatus();
            }
        }

        public bool CheckStatus(bool Login)
        {
            StatusLabelText = "Loading page to check if we are logged in.";
            WikiStatusResult result = Variables.User.UpdateWikiStatus();

            bool b = false;
            string label = "Software disabled";

            switch (result)
            {
                case WikiStatusResult.Error:
                    lblUserName.BackColor = Color.Red;
                    lblUserName.Text = "User:";
                    MessageBox.Show("Check page failed to load.\r\n\r\nCheck your Internet Explorer is working and that the Wikipedia servers are online, also try clearing Internet Explorer cache.", "User check problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case WikiStatusResult.NotLoggedIn:
                    lblUserName.BackColor = Color.Red;
                    lblUserName.Text = "User:";
                    if (!Login)
                        MessageBox.Show("You are not logged in. The log in screen will now load, enter your name and password, click \"Log in\", wait for it to complete, then start the process again.\r\n\r\nIn the future you can make sure this won't happen by logging in to Wikipedia using Microsoft Internet Explorer.", "Not logged in", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    webBrowserEdit.LoadLogInPage();
                    webBrowserEdit.BringToFront();
                    break;

                case WikiStatusResult.NotRegistered:
                    lblUserName.BackColor = Color.Red;
                    MessageBox.Show(Variables.User.Name + " is not enabled to use this.", "Not enabled", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Tools.OpenURLInBrowser(Variables.URL + "/wiki/Project:AutoWikiBrowser/CheckPage");
                    break;

                case WikiStatusResult.OldVersion:
                    OldVersion();
                    break;

                case WikiStatusResult.Registered:
                    b = true;
                    label = string.Format("Logged in, user and software enabled. Bot = {0}, Admin = {1}", Variables.User.IsBot, Variables.User.IsAdmin);
                    lblUserName.BackColor = Color.LightGreen;

                    //Get list of articles not to apply general fixes to.
                    Match n = WikiRegexes.NoGeneralFixes.Match(Variables.User.CheckPageText);
                    if (n.Success)
                    {
                        foreach (Match link in WikiRegexes.UnPipedWikiLink.Matches(n.Value))
                            if (!noParse.Contains(link.Groups[1].Value))
                                noParse.Add(link.Groups[1].Value);
                    }
                    break;
            }

            // detect writing system
            if (Variables.RTL) RightToLeft = RightToLeft.Yes;
            else RightToLeft = RightToLeft.No;

            StatusLabelText = label;
            UpdateButtons(null, null);

            return b;
        }

        private void OldVersion()
        {
            if (!WebControl.Shutdown)
            {
                lblUserName.BackColor = Color.Red;

                DialogResult yesnocancel = MessageBox.Show("This version is not enabled, please download the newest version. If you have the newest version, check that Wikipedia is online.\r\n\r\nPlease press \"Yes\" to run the AutoUpdater, \"No\" to load the download page and update manually, or \"Cancel\" to not update (but you will not be able to edit).", "Problem", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                if (yesnocancel == DialogResult.Yes)
                    RunUpdater();
                else if (yesnocancel == DialogResult.No)
                {
                    Tools.OpenURLInBrowser("http://sourceforge.net/project/showfiles.php?group_id=158332");
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void chkAppend_CheckedChanged(object sender, EventArgs e)
        {
            txtAppendMessage.Enabled = rdoAppend.Enabled = rdoPrepend.Enabled =
            udNewlineChars.Enabled = lblUse.Enabled = lblNewlineCharacters.Enabled = chkAppend.Checked;
        }

        private void wordWrapToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            txtEdit.WordWrap = wordWrapToolStripMenuItem1.Checked;
        }

        private void chkIgnoreIfContains_CheckedChanged(object sender, EventArgs e)
        {
            txtSkipIfContains.Enabled = chkSkipIfContains.Checked;
        }

        private void chkOnlyIfContains_CheckedChanged(object sender, EventArgs e)
        {
            txtSkipIfNotContains.Enabled = chkSkipIfNotContains.Checked;
        }

        private void txtNewCategory_Leave(object sender, EventArgs e)
        {
            txtNewCategory.Text = txtNewCategory.Text.Trim('[', ']');
            txtNewCategory.Text = Regex.Replace(txtNewCategory.Text, "^" + Variables.NamespacesCaseInsensitive[14], "");
            txtNewCategory.Text = Tools.TurnFirstToUpper(txtNewCategory.Text);
        }

        private void txtNewCategory2_Leave(object sender, EventArgs e)
        {
            txtNewCategory2.Text = txtNewCategory2.Text.Trim('[', ']');
            txtNewCategory2.Text = Regex.Replace(txtNewCategory2.Text, "^" + Variables.NamespacesCaseInsensitive[14], "");
            txtNewCategory2.Text = Tools.TurnFirstToUpper(txtNewCategory2.Text);
        }

        private void ArticleInfo(bool reset)
        {
            string articleText = txtEdit.Text;
            int intWords = 0;
            int intCats = 0;
            int intImages = 0;
            int intLinks = 0;
            int intInterLinks = 0;
            lblWarn.Text = "";

            if (reset)
            {
                //Resets all the alerts.
                lblWords.Text = "Words: ";
                lblCats.Text = "Categories: ";
                lblImages.Text = "Images: ";
                lblLinks.Text = "Links: ";
                lblInterLinks.Text = "Interwiki links: ";

                lbDuplicateWikilinks.Items.Clear();
                lblDuplicateWikilinks.Visible = false;
                lbDuplicateWikilinks.Visible = false;
                btnRemove.Visible = false;
            }
            else
            {
                intWords = Tools.WordCount(articleText);

                intCats = Regex.Matches(articleText, "\\[\\[" + Variables.Namespaces[14], RegexOptions.IgnoreCase).Count;

                intImages = Regex.Matches(articleText, "\\[\\[" + Variables.Namespaces[6], RegexOptions.IgnoreCase).Count;

                intInterLinks = WikiRegexes.InterWikiLinks.Matches(articleText).Count;

                intLinks = WikiRegexes.WikiLinksOnly.Matches(articleText).Count;

                intLinks = intLinks - intInterLinks - intImages - intCats;

                if (TheArticle.NameSpaceKey == 0 && (WikiRegexes.Stub.IsMatch(articleText)) && (intWords > 500))
                    lblWarn.Text = "Long article with a stub tag.\r\n";

                if (!(Regex.IsMatch(articleText, "\\[\\[" + Variables.Namespaces[14], RegexOptions.IgnoreCase)))
                    lblWarn.Text += "No category (although one may be in a template)\r\n";

                if (articleText.StartsWith("=="))
                    lblWarn.Text += "Starts with heading.";

                lblWords.Text = "Words: " + intWords.ToString();
                lblCats.Text = "Categories: " + intCats.ToString();
                lblImages.Text = "Images: " + intImages.ToString();
                lblLinks.Text = "Links: " + intLinks.ToString();
                lblInterLinks.Text = "Interwiki links: " + intInterLinks.ToString();

                //Find multiple links                
                lbDuplicateWikilinks.Items.Clear();
                ArrayList arrayLinks = new ArrayList();
                string x = "";
                //get all the links
                foreach (Match m in WikiRegexes.WikiLink.Matches(articleText))
                {
                    x = m.Groups[1].Value;
                    if (!WikiRegexes.Dates.IsMatch(x) && !WikiRegexes.Dates2.IsMatch(x))
                        arrayLinks.Add(x);
                }

                lbDuplicateWikilinks.Sorted = true;

                //add the duplicate articles to the listbox
                foreach (string z in arrayLinks)
                {
                    if ((arrayLinks.IndexOf(z) < arrayLinks.LastIndexOf(z)) && (!lbDuplicateWikilinks.Items.Contains(z)))
                        lbDuplicateWikilinks.Items.Add(z);
                }
                arrayLinks = null;

                if (lbDuplicateWikilinks.Items.Count > 0)
                {
                    lblDuplicateWikilinks.Visible = true;
                    lbDuplicateWikilinks.Visible = true;
                    btnRemove.Visible = true;
                }
            }
        }

        private void lbDuplicateWikilinks_Click(object sender, EventArgs e)
        {
            EditBoxTab.SelectedTab = tpEdit;
            int selection = lbDuplicateWikilinks.SelectedIndex;
            if (selection != oldselection)
                Find.ResetFind();
            if (lbDuplicateWikilinks.SelectedIndex != -1)
            {
                string strLink = Regex.Escape(lbDuplicateWikilinks.SelectedItem.ToString());
                Find.Find1("\\[\\[" + strLink + "(\\|.*?)?\\]\\]", true, true, txtEdit, TheArticle.Name);
                btnRemove.Enabled = true;
            }
            else
            {
                Find.ResetFind();
                btnRemove.Enabled = false;
            }

            ArticleInfo(false);
            try
            {
                if (lbDuplicateWikilinks.Items.Count != selection + 2)
                    lbDuplicateWikilinks.SelectedIndex = selection + 2;
                else
                    lbDuplicateWikilinks.SelectedIndex = selection + 1;
                lbDuplicateWikilinks.SelectedIndex = selection;
            }
            catch
            {
                lbDuplicateWikilinks.SelectedIndex = lbDuplicateWikilinks.Items.Count - 1;
            }
            oldselection = selection;
        }

        private void ResetFind(object sender, EventArgs e)
        {
            Find.ResetFind();
        }

        private void txtEdit_TextChanged(object sender, EventArgs e)
        {
            Find.ResetFind();
            try
            {
                TheArticle.EditSummary = "";
            }
            catch { }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            EditBoxTab.SelectedTab = tpEdit;
            Find.Find1(txtFind.Text, chkFindRegex.Checked, chkFindCaseSensitive.Checked, txtEdit, TheArticle.Name);
        }

        private void toolStripTextBox2_Click(object sender, EventArgs e)
        {
            toolStripTextBox2.Text = "";
        }

        private void toolStripTextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) && e.KeyChar != 8)
                e.Handled = true;

            if (e.KeyChar == '\r' && toolStripTextBox2.Text.Length > 0)
            {
                e.Handled = true;
                GoToLine();
                mnuTextBox.Hide();
            }
        }

        private void GoToLine()
        {
            int i = 1;
            int intLine = int.Parse(toolStripTextBox2.Text);
            int intStart = 0;
            int intEnd = 0;

            foreach (Match m in Regex.Matches(txtEdit.Text, "^.*?$", RegexOptions.Multiline))
            {
                if (i == intLine)
                {
                    intStart = m.Index;
                    intEnd = intStart + m.Length;
                    break;
                }
                i++;
            }

            txtEdit.Select(intStart, intEnd - intStart);
            txtEdit.ScrollToCaret();
            txtEdit.Focus();
        }

        [Conditional("DEBUG")]
        public void Debug()
        {
            Tools.WriteDebugEnabled = true;
            listMaker1.Add("Project:AutoWikiBrowser/Sandbox");
            //Variables.User.WikiStatus = true; // Stop logging in and the username code doesn't work!
            lblOnlyBots.Visible = false;
            dumpHTMLToolStripMenuItem.Visible = true;
            logOutDebugToolStripMenuItem.Visible = true;
            bypassAllRedirectsToolStripMenuItem.Enabled = true;
            profileTyposToolStripMenuItem.Visible = true;
            toolStripSeparator29.Visible = true;

#if DEBUG
            Variables.Profiler = new Profiler("profiling.txt", true);
#endif
        }

        #endregion

        #region set variables

        private void PreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyPreferences myPrefs = new MyPreferences(Variables.LangCode, Variables.Project,
                Variables.CustomProject, txtEdit.Font, LowThreadPriority, Flash, Beep,
                Minimize, SaveArticleList, TimeOut, AutoSaveEditBoxEnabled, AutoSaveEditBoxFile,
                AutoSaveEditBoxPeriod, SupressUsingAWB);

            if (myPrefs.ShowDialog(this) == DialogResult.OK)
            {
                txtEdit.Font = myPrefs.TextBoxFont;
                LowThreadPriority = myPrefs.LowThreadPriority;
                Flash = myPrefs.PerfFlash;
                Beep = myPrefs.PerfBeep;
                Minimize = myPrefs.PerfMinimize;
                SaveArticleList = myPrefs.PerfSaveArticleList;
                TimeOut = myPrefs.PerfTimeOutLimit;
                AutoSaveEditBoxEnabled = myPrefs.PerfAutoSaveEditBoxEnabled;
                AutoSaveEditBoxPeriod = myPrefs.PerfAutoSaveEditBoxPeriod;
                AutoSaveEditBoxFile = myPrefs.PerfAutoSaveEditBoxFile;
                SupressUsingAWB = myPrefs.PerfSupressUsingAWB;

                if (myPrefs.Language != Variables.LangCode || myPrefs.Project != Variables.Project || myPrefs.CustomProject != Variables.CustomProject)
                {
                    SetProject(myPrefs.Language, myPrefs.Project, myPrefs.CustomProject);

                    Variables.User.WikiStatus = false;
                    BotMode = false;
                    lblOnlyBots.Visible = true;
                    Variables.User.IsBot = false;
                    Variables.User.IsAdmin = false;
                }
            }
            myPrefs = null;

            listMaker1.AddRemoveRedirects();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //refresh typo list
            LoadTypos(true);

            //refresh talk warnings list
            if (userTalkWarningsLoaded)
                LoadUserTalkWarnings();

            //refresh login status, and reload check list
            if (!Variables.User.WikiStatus)
            {
                if (!CheckStatus(false))
                    return;
            }
        }

        private void SetProject(LangCodeEnum code, ProjectEnum project, string customProject)
        {
            splash.SetProgress(81);
            try
            {
                //set namespaces
                Variables.SetProject(code, project, customProject);

                //set interwikiorder
                switch (Variables.LangCode)
                {
                    case LangCodeEnum.en:
                    case LangCodeEnum.pl:
                    case LangCodeEnum.simple:
                        parsers.InterWikiOrder = InterWikiOrderEnum.LocalLanguageAlpha;
                        break;

                    case LangCodeEnum.he:
                    case LangCodeEnum.hu:
                        parsers.InterWikiOrder = InterWikiOrderEnum.AlphabeticalEnFirst;
                        break;

                    default:
                        parsers.InterWikiOrder = InterWikiOrderEnum.Alphabetical;
                        break;
                }

                //user interface
                if (!Variables.IsWikipediaEN)
                {
                    // TODO: Hide or disable some of the text box context menu stuff, which is likely WP-EN only (and do the opposite for WP-EN)
                    chkAutoTagger.Checked = false;
                    //chkGeneralFixes.Checked = false; // helluva works everywhere.. more or less
                }

                userTalkWarningsLoaded = false; // force reload

                if (!Variables.IsCustomProject && !Variables.IsWikia && !Variables.IsWikimediaMonolingualProject)
                    lblProject.Text = Variables.LangCode.ToString().ToLower() + "." + Variables.Project;
                else if (Variables.IsWikimediaMonolingualProject)
                    lblProject.Text = Variables.Project.ToString();
                else
                    lblProject.Text = Variables.URL;
            }
            catch (ArgumentNullException ex)
            {
                if (ex is ArgumentNullException)
                    MessageBox.Show("The interwiki list didn't load correctly. Please check your internet connection, and then restart AWB");
                else
                    throw;
            }
        }

        #endregion

        #region Enabling/Disabling of buttons

        private void UpdateButtons(object sender, EventArgs e)
        {
            bool enabled = listMaker1.NumberOfArticles > 0;
            SetStartButton(enabled);

            //listMaker1.ButtonsEnabled = enabled;
            lbltsNumberofItems.Text = "Articles: " + listMaker1.NumberOfArticles.ToString();
            bypassAllRedirectsToolStripMenuItem.Enabled = Variables.User.IsAdmin;
        }

        private void SetStartButton(bool enabled)
        {
            /* Please don't remove the If statements; otherwise the EnabledChange event fires even if the button
             * button was already named. The Kingbotk plugin attaches to that event. */
            if (!btnStart.Enabled) btnStart.Enabled = enabled;
            if (!btntsStart.Enabled) btntsStart.Enabled = enabled;
        }

        private void DisableButtons()
        {
            SetStartButton(false);
            SetButtons(false);

            if (listMaker1.NumberOfArticles == 0)
                btnIgnore.Enabled = false;

            if (cmboEditSummary.Focused) txtEdit.Focus();
        }

        private void EnableButtons()
        {
            UpdateButtons(null, null);
            SetButtons(true);
        }

        private void SetButtons(bool enabled)
        {
            btnSave.Enabled = btnIgnore.Enabled = btnPreview.Enabled = btnDiff.Enabled =
            btntsPreview.Enabled = btntsChanges.Enabled = listMaker1.MakeListEnabled =
            btntsSave.Enabled = btntsIgnore.Enabled = btnMove.Enabled = btntsDelete.Enabled = btnDelete.Enabled =
            btnWatch.Enabled = btnProtect.Enabled = findGroup.Enabled = enabled;
        }

        #endregion

        #region Timers

        int intRestartDelay = 5;
        int intStartInSeconds = 5;
        private void DelayedRestart(object sender, EventArgs e)
        {
            StopDelayedAutoSaveTimer();
            StatusLabelText = "Restarting in " + intStartInSeconds.ToString();

            if (intStartInSeconds == 0)
            {
                StopDelayedRestartTimer();
                Start();
            }
            else
                intStartInSeconds--;
        }
        private void StartDelayedRestartTimer(object sender, EventArgs e)
        {
            intStartInSeconds = intRestartDelay;
            Ticker += DelayedRestart;
            //increase the restart delay each time, this is decreased by 1 on each successfull save
            intRestartDelay += 5;
        }
        private void StartDelayedRestartTimer(object sender, EventArgs e, int delay)
        {
            intStartInSeconds = delay;
            Ticker += DelayedRestart;
        }
        private void StopDelayedRestartTimer()
        {
            Ticker -= DelayedRestart;
            intStartInSeconds = intRestartDelay;
        }

        private void StopDelayedAutoSaveTimer()
        {
            Ticker -= DelayedAutoSave;
            intTimer = 0;
            lblBotTimer.Text = "Bot timer: " + intTimer.ToString();
        }

        private void StartDelayedAutoSaveTimer()
        {
            Ticker += DelayedAutoSave;
        }

        int intTimer;
        private void DelayedAutoSave(object sender, EventArgs e)
        {
            if (intTimer < nudBotSpeed.Value)
            {
                intTimer++;
                if (intTimer == 1)
                    lblBotTimer.BackColor = Color.Red;
                else
                    lblBotTimer.BackColor = DefaultBackColor;
            }
            else
            {
                StopDelayedAutoSaveTimer();
                SaveArticle();
            }

            lblBotTimer.Text = "Bot timer: " + intTimer.ToString();
        }

        private void showTimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowTimer();
        }

        private void ShowTimer()
        {
            lblTimer.Visible = showTimerToolStripMenuItem.Checked;
            StopSaveInterval(null, null);
        }

        int intStartTimer;
        private void SaveInterval(object sender, EventArgs e)
        {
            intStartTimer++;
            lblTimer.Text = "Timer: " + intStartTimer.ToString();
        }
        private void StopSaveInterval(object sender, EventArgs e)
        {
            intStartTimer = 0;
            lblTimer.Text = "Timer: 0";
            Ticker -= SaveInterval;
        }

        public delegate void Tick(object sender, EventArgs e);
        public event Tick Ticker;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Ticker != null)
                Ticker(null, null);

            seconds++;
            if (seconds == 60)
            {
                seconds = 0;
                EditsPerMin();
            }
        }

        int seconds;
        int lastTotal;
        private void EditsPerMin()
        {
            int editsInLastMin = NumberOfEdits - lastTotal;
            NumberOfEditsPerMinute = editsInLastMin;
            lastTotal = NumberOfEdits;
        }

        #endregion

        #region menus and buttons

        private void makeModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cModule.Show();
        }

        private void btnMoreSkip_Click(object sender, EventArgs e)
        {
            Skip.ShowDialog();
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            GetPreview();
        }

        private void btnDiff_Click(object sender, EventArgs e)
        {
            GetDiff();
        }

        private void FalsePositiveClick(object sender, EventArgs e)
        {
            if (TheArticle.Name.Length > 0)
                Tools.WriteTextFile("#[[" + TheArticle.Name + "]]\r\n", @"False positives.txt", true);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            SkipPage("user");
        }

        private void btnMove_Click(object sender, EventArgs e)
        {
            MoveArticle();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteArticle();
        }

        private void btnProtect_Click(object sender, EventArgs e)
        {
            ProtectArticle();
            //Start();
        }

        private void filterOutNonMainSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listMaker1.FilterNonMainAuto = filterOutNonMainSpaceToolStripMenuItem.Checked;

            if (filterOutNonMainSpaceToolStripMenuItem.Checked)
                listMaker1.FilterNonMainArticles();
        }

        private void removeDuplicatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listMaker1.FilterDuplicates = removeDuplicatesToolStripMenuItem.Checked;

            if (removeDuplicatesToolStripMenuItem.Checked)
                listMaker1.removeListDuplicates();
        }

        private void specialFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            listMaker1.Filter();
        }

        private void convertToTalkPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listMaker1.ConvertToTalkPages();
        }

        private void convertFromTalkPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listMaker1.ConvertFromTalkPages();
        }

        private void sortAlphabeticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listMaker1.AutoAlpha = sortAlphabeticallyToolStripMenuItem.Checked;

            if (sortAlphabeticallyToolStripMenuItem.Checked)
                listMaker1.AlphaSortList();
        }

        private void saveListToTextFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listMaker1.SaveList();
        }

        private void launchListComparerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listMaker1.Count > 0 && MessageBox.Show("Would you like to copy your current Article List to the ListComparer?", "Copy Article List?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                lc = new ListComparer(listMaker1.GetArticleList());
            else
                lc = new ListComparer();

            lc.Show(this);
        }

        private void launchListSplitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WikiFunctions.AWBSettings.UserPrefs p = MakePrefs();

            if (listMaker1.Count > 0 && MessageBox.Show("Would you like to copy your current Article List to the ListSplitter?", "Copy Article List?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                splitter = new ListSplitter(p, WikiFunctions.AWBSettings.UserPrefs.SavePluginSettings(p), listMaker1.GetArticleList());
            else
                splitter = new ListSplitter(p, WikiFunctions.AWBSettings.UserPrefs.SavePluginSettings(p));

            splitter.Show(this);
        }

        private void launchDumpSearcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LaunchDumpSearcher();
        }

        private void LaunchDumpSearcher()
        {
            WikiFunctions.DBScanner.DatabaseScanner ds = new WikiFunctions.DBScanner.DatabaseScanner();
            ds.Show();
            UpdateButtons(null, null);
        }

        private void addIgnoredToLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (addIgnoredToLogFileToolStripMenuItem.Checked)
            {
                btnFalsePositive.Visible = btntsFalsePositive.Visible = true;
                btnStop.Location = new System.Drawing.Point(220, 62);
                btnStop.Size = new System.Drawing.Size(51, 23);
            }
            else
            {
                btnFalsePositive.Visible = btntsFalsePositive.Visible = false;
                btnStop.Location = new System.Drawing.Point(156, 62);
                btnStop.Size = new System.Drawing.Size(117, 23);
            }
        }

        private void alphaSortInterwikiLinksToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            parsers.sortInterwikiOrder = alphaSortInterwikiLinksToolStripMenuItem.Checked;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape && btnStop.Enabled)
            {
                Stop();
                e.SuppressKeyPress = true;
                return;
            }

            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.S && btnSave.Enabled)
                {
                    Save();
                    e.SuppressKeyPress = true;
                    return;
                }
                else if (e.KeyCode == Keys.S && btnStart.Enabled)
                {
                    Start();
                    e.SuppressKeyPress = true;
                    return;
                }
                if (e.KeyCode == Keys.I && btnIgnore.Enabled)
                {
                    SkipPage("user");
                    e.SuppressKeyPress = true;
                    return;
                }
                if (e.KeyCode == Keys.D && btnDiff.Enabled)
                {
                    GetDiff();
                    e.SuppressKeyPress = true;
                    return;
                }
                if (e.KeyCode == Keys.E && btnPreview.Enabled)
                {
                    GetPreview();
                    e.SuppressKeyPress = true;
                    return;
                }
                if (e.KeyCode == Keys.F)
                {
                    Find.Find1(txtFind.Text, chkFindRegex.Checked, chkFindCaseSensitive.Checked,
                        txtEdit, TheArticle.Name);
                    e.SuppressKeyPress = true;
                    return;
                }
            }
        }

        private void cmbEditSummary_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !cmboEditSummary.Items.Contains(cmboEditSummary.Text))
            {
                e.SuppressKeyPress = true;
                cmboEditSummary.Items.Add(cmboEditSummary.Text);
            }
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = Tools.HTMLListToWiki(txtEdit.SelectedText, "*");
        }

        private void listToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = Tools.HTMLListToWiki(txtEdit.SelectedText, "#");
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.SelectAll();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Undo();
        }

        private void humanNameDisambigTagToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = "{{Hndis|name=" + Tools.MakeHumanCatKey(TheArticle.Name) + "}}";
        }

        private void wikifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Text = "{{Wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + txtEdit.Text;
        }

        private void cleanupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Text = "{{cleanup|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + txtEdit.Text;
        }

        private void expandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Text = "{{Expand}}\r\n\r\n" + txtEdit.Text;
        }

        private void speedyDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: When saving, if this code has been called, we could check we're not saving a db tag with no reason provided (this would be most useful if this code is used as part of a bigger AFD/prod/db solution)
            // TODO: This and many other handlers like it are EN only; controls should be invisible when not on EN or the code internationalised/strings moved elsewhere
            Rectangle scrn = Screen.GetWorkingArea(this);
            txtEdit.Text = "{{db|" + Microsoft.VisualBasic.Interaction.InputBox("Enter a reason. Leave blank if you'll edit " +
            "the reason in the AWB text box", "Speedy deletion", "", scrn.Width / 2, scrn.Height / 3) + "}}\r\n\r\n" + txtEdit.Text;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = "{{subst:clear}}";
        }

        private void uncategorisedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = "{{Uncategorized|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
        }

        private void bypassAllRedirectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //txtEdit.Text = parsers.BypassRedirects(txtEdit.Text);
            if (MessageBox.Show("Replacement of links to redirects with direct links is strongly discouraged, " +
                "however it could be useful in some circumstances. Are you sure you want to continue?",
                "Bypass redirects", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)

                return;

            BackgroundRequest r = new BackgroundRequest();

            Enabled = false;
            r.BypassRedirects(txtEdit.Text);
            while (!r.Done) Application.DoEvents();
            Enabled = true;

            txtEdit.Text = (string)r.Result;
        }

        private void unicodifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = txtEdit.SelectedText;
            text = parsers.Unicodify(text);
            txtEdit.SelectedText = text;
        }

        private void metadataTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = "{{Persondata\r\n|NAME=\r\n|ALTERNATIVE NAMES=\r\n|SHORT DESCRIPTION=\r\n|DATE OF BIRTH=\r\n|PLACE OF BIRTH=\r\n|DATE OF DEATH=\r\n|PLACE OF DEATH=\r\n}}";
        }

        private void humanNameCategoryKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = "{{DEFAULTSORT:" + Tools.MakeHumanCatKey(TheArticle.Name) + "}}";
        }

        private void birthdeathCatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //find first dates
            string strBirth = "";
            string strDeath = "";
            Regex regexDates = new Regex("[1-2][0-9]{3}");

            try
            {
                MatchCollection m = regexDates.Matches(txtEdit.Text);

                if (m.Count >= 1)
                    strBirth = m[0].Value;
                if (m.Count >= 2)
                    strDeath = m[1].Value;

                //make name, surname, firstname
                string strName = Tools.MakeHumanCatKey(TheArticle.Name);

                string categories = "";

                if (strDeath.Length == 0 || int.Parse(strDeath) < int.Parse(strBirth) + 20)
                    categories = "[[Category:" + strBirth + " births|" + strName + "]]";
                else
                    categories = "[[Category:" + strBirth + " births|" + strName + "]]\r\n[[Category:" + strDeath + " deaths|" + strName + "]]";

                txtEdit.SelectedText = categories;
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        private void stubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = toolStripTextBox1.Text;
        }
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                txtEdit.Focus();

                cutToolStripMenuItem.Enabled = copyToolStripMenuItem.Enabled =
                    openSelectionInBrowserToolStripMenuItem.Enabled = (txtEdit.SelectedText.Length > 0);

                undoToolStripMenuItem.Enabled = txtEdit.CanUndo;

                openPageInBrowserToolStripMenuItem.Enabled = TheArticle.Name.Length > 0;
                openTalkPageInBrowserToolStripMenuItem.Enabled = TheArticle.Name.Length > 0;
                openHistoryMenuItem.Enabled = TheArticle.Name.Length > 0;
                replaceTextWithLastEditToolStripMenuItem.Enabled = LastArticle.Length > 0;
            }
            catch { }
        }

        private void openPageInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.OpenURLInBrowser(Variables.URLLong + "index.php?title=" + TheArticle.URLEncodedName);

        }

        private void openTalkPageInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.OpenURLInBrowser(Variables.URLLong + "index.php?title=" + GetLists.ConvertToTalk(TheArticle));
        }

        private void openHistoryMenuItem_Click(object sender, EventArgs e)
        {
            Tools.OpenURLInBrowser(Variables.URLLong + "index.php?title=" + TheArticle.URLEncodedName + "&action=history");
        }

        private void openSelectionInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.OpenURLInBrowser(Variables.URLLong + "index.php?title=" + txtEdit.SelectedText);
        }

        private void chkGeneralParse_CheckedChanged(object sender, EventArgs e)
        {
            alphaSortInterwikiLinksToolStripMenuItem.Enabled = chkGeneralFixes.Checked;
        }

        private void btnFindAndReplaceAdvanced_Click(object sender, EventArgs e)
        {
            if (!replaceSpecial.Visible)
                replaceSpecial.Show();
            else
                replaceSpecial.Hide();
        }

        private void btnMoreFindAndReplce_Click(object sender, EventArgs e)
        {
            findAndReplace.ShowDialog(this);
        }

        private void Stop()
        {
            PageReload = false;
            NudgeTimer.Stop();
            UpdateButtons(null, null);
            if (intTimer > 0)
            {//stop and reset the bot timer.
                StopDelayedAutoSaveTimer();
                EnableButtons();
                return;
            }

            StopSaveInterval(null, null);
            StopDelayedRestartTimer();
            if (webBrowserEdit.IsBusy)
                webBrowserEdit.Stop2();
            if (Variables.User.webBrowserLogin.IsBusy)
                Variables.User.webBrowserLogin.Stop();

            listMaker1.Stop();

            if (AutoSaveEditBoxEnabled)
                EditBoxSaveTimer.Enabled = false;

            StatusLabelText = "Stopped";
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(h);
        }

        #region Edit Box Menu
        private void reparseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reparseEditBox();
        }

        private void reparseEditBox()
        {
            ArticleEX a = new ArticleEX(TheArticle.Name);

            a.OriginalArticleText = txtEdit.Text;
            ErrorHandler.CurrentArticle = TheArticle.Name;
            ProcessPage(a);
            ErrorHandler.CurrentArticle = "";
            txtEdit.Text = a.ArticleText;
            GetDiff();
        }

        private void replaceTextWithLastEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LastArticle.Length > 0)
                txtEdit.Text = LastArticle;
        }

        #region PasteMore
        private void PasteMore1_DoubleClick(object sender, EventArgs e)
        {
            txtEdit.SelectedText = PasteMore1.Text;
            mnuTextBox.Hide();
        }

        private void PasteMore2_DoubleClick(object sender, EventArgs e)
        {
            txtEdit.SelectedText = PasteMore2.Text;
            mnuTextBox.Hide();
        }

        private void PasteMore3_DoubleClick(object sender, EventArgs e)
        {
            txtEdit.SelectedText = PasteMore3.Text;
            mnuTextBox.Hide();
        }

        private void PasteMore4_DoubleClick(object sender, EventArgs e)
        {
            txtEdit.SelectedText = PasteMore4.Text;
            mnuTextBox.Hide();
        }

        private void PasteMore5_DoubleClick(object sender, EventArgs e)
        {
            txtEdit.SelectedText = PasteMore5.Text;
            mnuTextBox.Hide();
        }

        private void PasteMore6_DoubleClick(object sender, EventArgs e)
        {
            txtEdit.SelectedText = PasteMore6.Text;
            mnuTextBox.Hide();
        }

        private void PasteMore7_DoubleClick(object sender, EventArgs e)
        {
            txtEdit.SelectedText = PasteMore7.Text;
            mnuTextBox.Hide();
        }

        private void PasteMore8_DoubleClick(object sender, EventArgs e)
        {
            txtEdit.SelectedText = PasteMore8.Text;
            mnuTextBox.Hide();
        }

        private void PasteMore9_DoubleClick(object sender, EventArgs e)
        {
            txtEdit.SelectedText = PasteMore9.Text;
            mnuTextBox.Hide();
        }

        private void PasteMore10_DoubleClick(object sender, EventArgs e)
        {
            txtEdit.SelectedText = PasteMore10.Text;
            mnuTextBox.Hide();
        }
        #endregion

        private void removeAllExcessWhitespaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = RemoveText.Hide(txtEdit.Text);
            text = Parsers.RemoveAllWhiteSpace(text);
            text = RemoveText.AddBack(text);

            txtEdit.Text = text;
        }
        #endregion

        private void txtNewCategory_DoubleClick(object sender, EventArgs e)
        {
            txtNewCategory.SelectAll();
        }

        private void cmboEditSummary_MouseMove(object sender, MouseEventArgs e)
        {
            string editSummary = "";

            if (TheArticle != null)
                editSummary = TheArticle.EditSummary;

            if (string.IsNullOrEmpty(editSummary))
                toolTip1.SetToolTip(cmboEditSummary, "");
            else
                toolTip1.SetToolTip(cmboEditSummary, MakeSummary());
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons(null, null);
        }

        private void chkRegExTypo_CheckedChanged(object sender, EventArgs e)
        {
            if (BotMode && chkRegExTypo.Checked)
            {
                MessageBox.Show("RegexTypoFix cannot be used with auto save on.\r\nAutosave will now be turned off, and Typos loaded.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                BotMode = false;
                //return;
            }
            LoadTypos(false);
            chkSkipIfNoRegexTypo.Enabled = chkRegExTypo.Checked;
        }

        private void LoadTypos(bool Reload)
        {
            if (chkRegExTypo.Checked)
            {
                StatusLabelText = "Loading typos";

                string s = Variables.RETFPath;

                if (!s.StartsWith("http:")) s = Variables.URL + "/wiki/" + s;

                string message = @"1. Check each edit before you make it. Although this has been built to be very accurate there is always the possibility of an error which requires your attention.

2. Optional: Select [[WP:AWB/T|Typo fixing]] as the edit summary. This lets everyone know where to bring issues with the typo correction.";

                if (RegexTypos == null)
                    message += "\r\n\r\nThe newest typos will now be downloaded from " + s + " when you press OK.";

                MessageBox.Show(message, "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                if (RegexTypos == null || Reload)
                {
                    RegexTypos = new RegExTypoFix();
                    if (RegexTypos.TyposLoaded)
                    {
                        StatusLabelText = RegexTypos.TyposCount.ToString() + " typos loaded";
                    }
                    else
                    {
                        chkRegExTypo.Checked = chkSkipIfNoRegexTypo.Enabled = false;
                        RegexTypos = null;
                    }
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenURLInBrowser("http://en.wikipedia.org/wiki/Wikipedia:AutoWikiBrowser/Typos");
        }

        private void webBrowserEdit_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            toolStripProgressBar1.MarqueeAnimationSpeed = 0;
            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
        }

        private void webBrowserEdit_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            webBrowserEdit.BringToFront();
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
            toolStripProgressBar1.MarqueeAnimationSpeed = 100;
        }

        private void dumpHTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Text = webBrowserEdit.Document.Body.InnerHtml;
        }

        private void logOutDebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Variables.User.WikiStatus = false;
        }

        private void summariesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SummaryEditor se = new SummaryEditor();

            string[] summaries = new string[cmboEditSummary.Items.Count];
            cmboEditSummary.Items.CopyTo(summaries, 0);
            se.Summaries.Lines = summaries;
            se.Summaries.Select(0, 0);

            string prevSummary = cmboEditSummary.SelectedText;

            if (se.ShowDialog() == DialogResult.OK)
            {
                cmboEditSummary.Items.Clear();

                foreach (string s in se.Summaries.Lines)
                {
                    if (string.IsNullOrEmpty(s.Trim())) continue;
                    cmboEditSummary.Items.Add(s.Trim());
                }

                if (cmboEditSummary.Items.Contains(prevSummary))
                    cmboEditSummary.SelectedText = prevSummary;
                else cmboEditSummary.SelectedItem = 0;
            }
        }

        private void showHidePanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PanelShowHide();
        }

        private void enlargeEditAreaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParametersShowHide();
        }

        #endregion

        #region tool bar stuff

        private void btnShowHide_Click(object sender, EventArgs e)
        {
            PanelShowHide();
        }

        private void btntsShowHideParameters_Click(object sender, EventArgs e)
        {
            ParametersShowHide();
        }

        private void btntsStart_Click(object sender, EventArgs e)
        {
            Start();
        }

        private void btntsSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void btntsIgnore_Click(object sender, EventArgs e)
        {
            SkipPage("user");
        }

        private void btntsStop_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void btntsPreview_Click(object sender, EventArgs e)
        {
            GetPreview();
        }

        private void btntsChanges_Click(object sender, EventArgs e)
        {
            GetDiff();
        }

        private void btntsDelete_Click(object sender, EventArgs e)
        {
            DeleteArticle();
        }

        private void SetBrowserSize()
        {
            if (toolStrip.Visible)
            {
                webBrowserEdit.Location = new Point(webBrowserEdit.Location.X, 48);
                if (panel1.Visible)
                    webBrowserEdit.Height = panel1.Location.Y - 48;
                else
                    webBrowserEdit.Height = statusStrip1.Location.Y - 48;
            }
            else
            {
                webBrowserEdit.Location = new Point(webBrowserEdit.Location.X, 25);
                if (panel1.Visible)
                    webBrowserEdit.Height = panel1.Location.Y - 25;
                else
                    webBrowserEdit.Height = statusStrip1.Location.Y - 25;
            }

            webBrowserDiff.Location = webBrowserEdit.Location;
            webBrowserDiff.Size = webBrowserEdit.Size;
        }

        private void enableTheToolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EnableToolBar = enableTheToolbarToolStripMenuItem.Checked;
        }

        private bool EnableToolBar
        {
            get { return toolStrip.Visible; }
            set
            {
                toolStrip.Visible = enableTheToolbarToolStripMenuItem.Checked = value;
                SetBrowserSize();
            }
        }

        #endregion

        #region Images

        private void cmboImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmboImages.SelectedIndex == 0)
            {
                lblImageWith.Text = "";

                txtImageReplace.Enabled = txtImageWith.Enabled = chkSkipNoImgChange.Enabled = false;
            }
            else if (cmboImages.SelectedIndex == 1)
            {
                lblImageWith.Text = "With Image:";

                txtImageWith.Enabled = txtImageReplace.Enabled = chkSkipNoImgChange.Enabled = true;
            }
            else if (cmboImages.SelectedIndex == 2)
            {
                lblImageWith.Text = "";

                txtImageWith.Enabled = false;
                txtImageReplace.Enabled = true;
                chkSkipNoImgChange.Enabled = true;
            }
            else if (cmboImages.SelectedIndex == 3)
            {
                lblImageWith.Text = "Comment:";

                txtImageWith.Enabled = txtImageReplace.Enabled = chkSkipNoImgChange.Enabled = true;
            }
        }

        private void txtImageReplace_Leave(object sender, EventArgs e)
        {
            txtImageReplace.Text = Regex.Replace(txtImageReplace.Text, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase);
        }

        private void txtImageWith_Leave(object sender, EventArgs e)
        {
            txtImageWith.Text = Regex.Replace(txtImageWith.Text, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase);
        }

        private void SetProgressBar(object sender, EventArgs e)
        {
            if (listMaker1.BusyStatus)
            {
                toolStripProgressBar1.MarqueeAnimationSpeed = 100;
                toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
            }
            else
            {
                toolStripProgressBar1.MarqueeAnimationSpeed = 0;
                toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
            }
        }

        #endregion

        #region ArticleActions
        private void MoveArticle()
        {
            MoveDeleteDialog dlg = new MoveDeleteDialog(1);

            try
            {
                dlg.NewTitle = TheArticle.Name;
                dlg.Summary = LastMove;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    LastMove = dlg.Summary;
                    webBrowserEdit.MovePage(TheArticle.Name, dlg.NewTitle, dlg.Summary);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
            finally
            {
                dlg.Dispose();
            }
        }

        private void DeleteArticle()
        {
            MoveDeleteDialog dlg = new MoveDeleteDialog(2);

            try
            {
                dlg.Summary = LastDelete;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    LastDelete = dlg.Summary;
                    webBrowserEdit.DeletePage(TheArticle.Name, dlg.Summary);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
            finally
            {
                dlg.Dispose();
            }
        }
        static MoveDeleteDialog dlg;

        private void ProtectArticle()
        {
            if (dlg == null)
                dlg = new MoveDeleteDialog(3);

            try
            {
                dlg.Summary = LastProtect;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    LastProtect = dlg.Summary;
                    webBrowserEdit.ProtectPage(TheArticle.Name, dlg.Summary, dlg.EditProtectionLevel, dlg.MoveProtectionLevel, dlg.ProtectExpiry);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        #endregion

        private void btnSubst_Click(object sender, EventArgs e)
        {
            substTemplates.ShowDialog();
        }

        private void launchRegexTester(object sender, EventArgs e)
        {
            regexTester = new RegexTester();

            if (txtEdit.SelectionLength > 0 && MessageBox.Show("Would you like to transfer the currently selected Article Text to the Regex Tester?", "Transfer Article Text?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                regexTester.ArticleText = txtEdit.SelectedText;

            regexTester.Show();
        }

        private void chkLock_CheckedChanged(object sender, EventArgs e)
        {
            cmboEditSummary.Visible = !chkLock.Checked;
            lblSummary.Text = cmboEditSummary.Text;
            lblSummary.Visible = chkLock.Checked;
        }

        private void txtDabLink_TextChanged(object sender, EventArgs e)
        {
            btnLoadLinks.Enabled = !string.IsNullOrEmpty(txtDabLink.Text.Trim());
        }

        private void txtDabLink_Enter(object sender, EventArgs e)
        {
            if (txtDabLink.Text.Length == 0) txtDabLink.Text = listMaker1.SourceText;
        }

        private void chkEnableDab_CheckedChanged(object sender, EventArgs e)
        {
            panelDab.Enabled = chkEnableDab.Checked;
        }

        private void btnLoadLinks_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtDabLink.Text.Trim();
                if (name.Contains("|")) name = name.Substring(0, name.IndexOf('|') - 1);
                Article link = new WikiFunctions.Article(name);
                List<Article> l = GetLists.FromLinksOnPage(txtDabLink.Text);
                txtDabVariants.Text = "";
                foreach (Article a in l)
                {
                    uint i;
                    // exclude years
                    if (uint.TryParse(a.Name, out i) && (i < 2100)) continue;

                    // disambigs typically link to pages in the same namespace only
                    if (link.NameSpaceKey != a.NameSpaceKey) continue;

                    txtDabVariants.Text += a.Name + "\r\n";
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        private void txtDabLink_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '\r':
                    e.Handled = true;
                    btnLoadLinks_Click(this, null);
                    break;
            }
        }

        private void txtFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '\r':
                    e.Handled = true;
                    btnFind_Click(this, null);
                    break;
            }
        }

        #region Notify Tray
        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        { // also handles double click of the tray icon
            this.Visible = true;
            this.WindowState = LastState;
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void mnuNotify_Opening(object sender, CancelEventArgs e)
        {
            SetMenuVisibility(this.Visible);
        }

        private void SetMenuVisibility(bool visible)
        {
            showToolStripMenuItem.Enabled = !visible || this.WindowState == FormWindowState.Minimized;
            hideToolStripMenuItem.Enabled = visible;
        }

        public void NotifyBalloon(string Message, ToolTipIcon Icon)
        {
            ntfyTray.BalloonTipText = Message;
            ntfyTray.BalloonTipIcon = Icon;
            ntfyTray.ShowBalloonTip(10000);
        }
        #endregion

        private void btnRemove_Click(object sender, EventArgs e)
        {
            EditBoxTab.SelectedTab = tpEdit;
            string selectedtext = txtEdit.SelectedText;
            if (selectedtext.StartsWith("[[") && selectedtext.EndsWith("]]"))
            {
                selectedtext = selectedtext.Trim('[').Trim(']');
                if (selectedtext.EndsWith("|"))
                {
                    if (selectedtext.Contains("(") && selectedtext.Contains(")"))
                        selectedtext = selectedtext.Substring(0, selectedtext.IndexOf("("));
                    if (selectedtext.Contains(":"))
                        selectedtext = selectedtext.Substring(selectedtext.IndexOf(":")).TrimEnd('|');
                    if ("[[" + selectedtext + "]]" == txtEdit.SelectedText)
                    {
                        MessageBox.Show("The selected link could not be removed.");
                        selectedtext = "[[" + selectedtext + "]]";
                    }
                }
                else if (selectedtext.Contains("|"))
                    selectedtext = selectedtext.Substring(selectedtext.IndexOf("|") + 1);

                txtEdit.SelectedText = selectedtext;
            }
            else
                MessageBox.Show("Please select a link to remove either manually or by clicking a link in the list above.");
        }

        private void runUpdaterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunUpdater();
        }

        private void RunUpdater()
        {
            if (MessageBox.Show("AWB needs to be closed. To do this now, click 'yes'. If you need to save your settings, do this now, the updater will not complete until AWB is closed.", "Close AWB?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Updater.WaitForCompletion();
                try
                {
                    System.Diagnostics.Process.Start(Path.GetDirectoryName(Application.ExecutablePath) + "\\AWBUpdater.exe");
                }
                catch { }
                this.Close();
                Application.Exit();
            }
        }

        private void btnResetNudges_Click(object sender, EventArgs e)
        {
            Nudges = 0;
            sameArticleNudges = 0;
            lblNudges.Text = NudgeTimerString + "0";
        }

        #region "Nudge timer"
        private const string NudgeTimerString = "Total nudges: ";

        private void NudgeTimer_Tick(object sender, NudgeTimer.NudgeTimerEventArgs e)
        {
            //make sure there was no error and bot mode is still enabled
            if (BotMode)
            {
                if (Program.MyTrace.IsUploading || Program.MyTrace.IsGettingPassword)
                {
                    // Don't nudge when a log is uploading in the background or it is attempting to get a password from the user, just wait for it
                    e.Cancel = true; return;
                }

                bool cancel;
                // Tell plugins we're about to nudge, and give them the opportunity to cancel:
                foreach (KeyValuePair<string, IAWBPlugin> a in Plugin.Items)
                {
                    a.Value.Nudge(out cancel);
                    if (cancel)
                    {
                        e.Cancel = true; return;
                    }
                }

                // Update stats and nudge:
                Nudges++;
                lblNudges.Text = NudgeTimerString + Nudges;
                NudgeTimer.Stop();
                if (chkNudgeSkip.Checked && sameArticleNudges > 0)
                {
                    sameArticleNudges = 0;
                    SkipPage("There was an error saving the page twice");
                }
                else
                {
                    sameArticleNudges++;
                    Stop();
                    Start();
                }

                // Inform plugins:
                foreach (KeyValuePair<string, IAWBPlugin> a in Plugin.Items)
                { a.Value.Nudged(Nudges); }
            }
        }

        public int Nudges
        {
            get { return mnudges; }
            private set { mnudges = value; }
        }
        #endregion

        /// <summary>
        /// Save List Box to a text file
        /// </summary>
        /// <param name="listbox"></param>
        public void SaveList(ListBox listbox)
        {
            try
            {
                StringBuilder strList = new StringBuilder("");
                StreamWriter sw;
                string strListFile;
                if (saveListDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (String a in listbox.Items)
                        strList.AppendLine(a);
                    strListFile = saveListDialog.FileName;
                    sw = new StreamWriter(strListFile, false, Encoding.UTF8);
                    sw.Write(strList);
                    sw.Close();
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        #region Edit Box Saver
        private void EditBoxSaveTimer_Tick(object sender, EventArgs e)
        {
            if (AutoSaveEditBoxFile.Trim().Length > 0) SaveEditBoxText(AutoSaveEditBoxFile);
        }

        private void SaveEditBoxText(string path)
        {
            try
            {
                StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
                sw.Write(txtEdit.Text);
                sw.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }
        #endregion

        private void saveTextToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveListDialog.ShowDialog() == DialogResult.OK)
                SaveEditBoxText(saveListDialog.FileName);
        }

        private static void LoadUserTalkWarnings()
        {
            Regex userTalkTemplate = new Regex(@"# \[\[" + Variables.NamespacesCaseInsensitive[10] + @"(.*?)\]\]");
            StringBuilder builder = new StringBuilder(@"\{\{ ?(" + Variables.NamespacesCaseInsensitive[10] + ")? ?((");

            userTalkTemplatesRegex = null;
            userTalkWarningsLoaded = true; // or it will retry on each page load
            try
            {
                string text = "";
                try
                {
                    text = Tools.GetArticleText("Project:AutoWikiBrowser/User talk templates");
                }
                catch
                {
                    return;
                }
                foreach (Match m in userTalkTemplate.Matches(text))
                {
                    builder.Append(Regex.Escape(m.Groups[1].Value) + "|");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
            if (builder.Length > 1)
            {
                builder.Remove((builder.Length - 1), 1);
                builder.Append(") ?(\\|.*?)?) ?\\}\\}");
                userTalkTemplatesRegex = new Regex(builder.ToString(), RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
        }

        private void undoAllChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Text = TheArticle.OriginalArticleText;
        }

        private void reloadEditPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PageReload = true;
            webBrowserEdit.LoadEditPage(TheArticle.Name);
            TheArticle.OriginalArticleText = webBrowserEdit.GetArticleText();
        }

        private void chkSkipNonExistent_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSkipNonExistent.Checked)
                chkSkipExistent.Checked = false;
        }

        private void chkSkipExistent_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSkipExistent.Checked)
                chkSkipNonExistent.Checked = false;
        }

        #region History
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            NewHistory();
        }

        private void NewHistory()
        {
            try
            {
                if (EditBoxTab.SelectedTab == tpHistory)
                {
                    if (webBrowserHistory.Url != new System.Uri(Variables.URLLong + "index.php?title=" + TheArticle.URLEncodedName + "&action=history&useskin=myskin") && !string.IsNullOrEmpty(TheArticle.URLEncodedName))
                        webBrowserHistory.Navigate(Variables.URLLong + "index.php?title=" + TheArticle.URLEncodedName + "&action=history&useskin=myskin");
                }
            }
            catch
            {
                webBrowserHistory.Navigate("about:blank");
                webBrowserHistory.Document.Write("<html><body><p>Unable to load history</p></body></html>");
            }
        }

        private void webBrowserHistory_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string histHtml = webBrowserHistory.Document.Body.InnerHtml;
            string startMark = "<!-- start content -->";
            string endMark = "<!-- end content -->";
            if (histHtml.Contains(startMark) && histHtml.Contains(endMark))
                histHtml = histHtml.Substring(histHtml.IndexOf(startMark), histHtml.IndexOf(endMark) - histHtml.IndexOf(startMark));
            histHtml = histHtml.Replace("<A ", "<a target=\"blank\" ");
            histHtml = histHtml.Replace("<FORM ", "<form target=\"blank\" ");
            histHtml = histHtml.Replace("<DIV id=histlegend", "<div id=histlegend style=\"display:none;\"");
            histHtml = "<h3>" + TheArticle.Name + "</h3>" + histHtml;
            webBrowserHistory.Document.Body.InnerHtml = histHtml;
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.OpenArticleHistoryInBrowser(TheArticle.Name);
        }

        private void refreshHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                webBrowserHistory.Navigate(Variables.URLLong + "index.php?title=" + TheArticle.URLEncodedName + "&action=history&useskin=myskin");
            }
            catch
            {
                webBrowserHistory.Navigate("about:blank");
                webBrowserHistory.Document.Write("<html><body><p>Unable to load history</p></body></html>");
            }
        }

        private void mnuHistory_Opening(object sender, CancelEventArgs e)
        {
            openInBrowserToolStripMenuItem.Enabled = refreshHistoryToolStripMenuItem.Enabled = (TheArticle != null);
        }
        #endregion

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Tools.OpenURLInBrowser("http://commons.wikimedia.org/wiki/Image:Crystal_Clear_action_run.png");
        }

        private void profilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            profiles = new WikiFunctions.Profiles.AWBProfilesForm(webBrowserEdit);
            profiles.LoadProfile += LoadProfileSettings;
            profiles.ShowDialog(this);
        }

        private void LoadProfileSettings(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(profiles.SettingsToLoad))
                LoadPrefs(profiles.SettingsToLoad);
        }

        private void chkMinor_CheckedChanged(object sender, EventArgs e)
        {
            markAllAsMinorToolStripMenuItem.Checked = chkMinor.Checked;
        }

        private void markAllAsMinorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chkMinor.Checked = markAllAsMinorToolStripMenuItem.Checked;
        }

        #region Shutdown
        private void chkShutdown_CheckedChanged(object sender, EventArgs e)
        {
            EnableDisableShutdownControls(chkShutdown.Checked);
        }

        private void EnableDisableShutdownControls(bool enabled)
        {
            radShutdown.Enabled = radStandby.Enabled = radRestart.Enabled
            = radHibernate.Enabled = radShutdown.Checked = enabled;
        }

        private void CanShutdown()
        {
            if (chkShutdown.Checked && listMaker1.Count == 0)
            {
                ShutdownTimer.Enabled = true;
                ShutdownNotification shut = new ShutdownNotification();
                shut.ShutdownType = GetShutdownType();

                DialogResult result = shut.ShowDialog(this);

                if (result == DialogResult.Cancel)
                {
                    ShutdownTimer.Enabled = false;
                    MessageBox.Show(GetShutdownType() + " aborted!");
                }
                else if (result == DialogResult.Yes)
                    if (GetShutdownType() == "Standby" || GetShutdownType() == "Hibernate")
                    {
                        shut.Close();
                        shut.Dispose();
                        ShutdownTimer.Enabled = false;
                    }
                ShutdownComputer();
            }
        }

        private string GetShutdownType()
        {
            if (radShutdown.Checked)
                return "Shutdown";
            else if (radStandby.Checked)
                return "Standby";
            else if (radRestart.Checked)
                return "Restart";
            else if (radHibernate.Checked)
                return "Hibernate";
            else
                return "";
        }

        private void ShutdownComputer()
        {
            if (radHibernate.Checked)
                Application.SetSuspendState(PowerState.Hibernate, true, true);
            else if (radRestart.Checked)
                System.Diagnostics.Process.Start("shutdown", "-r");
            else if (radShutdown.Checked)
                System.Diagnostics.Process.Start("shutdown", "-s");
            else if (radStandby.Checked)
                Application.SetSuspendState(PowerState.Suspend, true, true);
        }

        private void ShutdownTimer_Tick(object sender, EventArgs e)
        {
            ShutdownComputer();
        }
        #endregion

        #region EditToolbar
        private void imgBold_Click(object sender, EventArgs e)
        {
            if (txtEdit.SelectionLength == 0)
            {
                txtEdit.SelectedText = "'''Bold text'''";
                txtEdit.SelectionStart = txtEdit.SelectionStart - 12;
                txtEdit.SelectionLength = 9;
            }
            else
            {
                txtEdit.SelectedText = "'''" + txtEdit.SelectedText + "'''";
            }
        }

        private void imgItalics_Click(object sender, EventArgs e)
        {
            if (txtEdit.SelectionLength == 0)
            {
                txtEdit.SelectedText = "''Italic text''";
                txtEdit.SelectionStart = txtEdit.SelectionStart - 13;
                txtEdit.SelectionLength = 11;
            }
            else
            {
                txtEdit.SelectedText = "''" + txtEdit.SelectedText + "''";
            }
        }

        private void imgLink_Click(object sender, EventArgs e)
        {
            if (txtEdit.SelectionLength == 0)
            {
                txtEdit.SelectedText = "[[Link title]]";
                txtEdit.SelectionStart = txtEdit.SelectionStart - 12;
                txtEdit.SelectionLength = 10;
            }
            else
            {
                txtEdit.SelectedText = "[[" + txtEdit.SelectedText + "]]";
            }
        }

        private void imgExtlink_Click(object sender, EventArgs e)
        {
            if (txtEdit.SelectionLength == 0)
            {
                txtEdit.SelectedText = "[http://www.example.com link title]";
                txtEdit.SelectionStart = txtEdit.SelectionStart - 34;
                txtEdit.SelectionLength = 33;
            }
            else
            {
                txtEdit.SelectedText = "[" + txtEdit.SelectedText + "]";
            }
        }

        private void imgMath_Click(object sender, EventArgs e)
        {
            if (txtEdit.SelectionLength == 0)
            {
                txtEdit.SelectedText = "<math>Insert formula here</math>";
                txtEdit.SelectionStart = txtEdit.SelectionStart - 26;
                txtEdit.SelectionLength = 19;
            }
            else
            {
                txtEdit.SelectedText = "<math>" + txtEdit.SelectedText + "</math>";
            }
        }

        private void imgNowiki_Click(object sender, EventArgs e)
        {
            if (txtEdit.SelectionLength == 0)
            {
                txtEdit.SelectedText = "<nowiki>Insert non-formatted text here</nowiki>";
                txtEdit.SelectionStart = txtEdit.SelectionStart - 39;
                txtEdit.SelectionLength = 30;
            }
            else
            {
                txtEdit.SelectedText = "<nowiki>" + txtEdit.SelectedText + "</nowiki>";
            }
        }

        private void imgHr_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = txtEdit.SelectedText + "\r\n----\r\n";
        }

        private void imgRedirect_Click(object sender, EventArgs e)
        {
            if (txtEdit.SelectionLength == 0)
            {
                txtEdit.SelectedText = "#REDIRECT [[Insert text]]";
                txtEdit.SelectionStart = txtEdit.SelectionStart - 13;
                txtEdit.SelectionLength = 11;
            }
            else
            {
                txtEdit.SelectedText = "#REDIRECT [[" + txtEdit.SelectedText + "]]";
            }
        }

        private void imgStrike_Click(object sender, EventArgs e)
        {
            if (txtEdit.SelectionLength == 0)
            {
                txtEdit.SelectedText = "<s>Strike-through text</s>";
                txtEdit.SelectionStart = txtEdit.SelectionStart - 23;
                txtEdit.SelectionLength = 19;
            }
            else
            {
                txtEdit.SelectedText = "<s>" + txtEdit.SelectedText + "</s>";
            }
        }

        private void imgSup_Click(object sender, EventArgs e)
        {
            if (txtEdit.SelectionLength == 0)
            {
                txtEdit.SelectedText = "<sup>Superscript text</sup>";
                txtEdit.SelectionStart = txtEdit.SelectionStart - 22;
                txtEdit.SelectionLength = 16;
            }
            else
            {
                txtEdit.SelectedText = "<sup>" + txtEdit.SelectedText + "</sup>";
            }
        }

        private void imgSub_Click(object sender, EventArgs e)
        {
            if (txtEdit.SelectionLength == 0)
            {
                txtEdit.SelectedText = "<sub>Subscript text</sub>";
                txtEdit.SelectionStart = txtEdit.SelectionStart - 20;
                txtEdit.SelectionLength = 14;
            }
            else
            {
                txtEdit.SelectedText = "<sub>" + txtEdit.SelectedText + "</sub>";
            }
        }

        private void SetEditToolBarEnabled(bool enabled)
        {
            imgBold.Enabled = imgExtlink.Enabled = imgHr.Enabled = imgItalics.Enabled = imgLink.Enabled =
            imgMath.Enabled = imgNowiki.Enabled = imgRedirect.Enabled = imgStrike.Enabled = imgSub.Enabled =
            imgSup.Enabled = enabled;
        }

        private bool EditToolBarVisible
        {
            set
            {
                if (imgBold.Visible != value)
                {
                    if (value)
                    {
                        txtEdit.Location = new Point(txtEdit.Location.X, txtEdit.Location.Y + 32);
                        txtEdit.Size = new Size(txtEdit.Size.Width, txtEdit.Size.Height - 32);
                    }
                    else
                    {
                        txtEdit.Location = new Point(txtEdit.Location.X, txtEdit.Location.Y - 32);
                        txtEdit.Size = new Size(txtEdit.Size.Width, txtEdit.Size.Height + 32);
                    }

                    imgBold.Visible = imgExtlink.Visible = imgHr.Visible = imgItalics.Visible = imgLink.Visible =
                    imgMath.Visible = imgNowiki.Visible = imgRedirect.Visible = imgStrike.Visible = imgSub.Visible =
                    imgSup.Visible = value;
                }
                showHideEditToolbarToolStripMenuItem.Checked = value;
            }
            get { return imgBold.Visible; }
        }

        private void showHideEditToolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showHideEditToolbarToolStripMenuItem.Checked = !showHideEditToolbarToolStripMenuItem.Checked;
            EditToolBarVisible = showHideEditToolbarToolStripMenuItem.Checked;
        }
        #endregion

        #region various menus and event handlers
        private void txtFind_MouseHover(object sender, EventArgs e)
        {
            toolTip1.SetToolTip(txtFind, txtFind.Text);
        }

        private void btnWatch_Click(object sender, EventArgs e)
        {
            //webBrowserEdit.WatchUnwatch();
            //SetWatchButton(btnWatch.Text == "Watch");
        }

        private void SetWatchButton(bool watch)
        {
            if (watch)
                btnWatch.Text = "Unwatch";
            else
                btnWatch.Text = "Watch";
        }

        private static int CompareRegexPairs(KeyValuePair<int, string> x, KeyValuePair<int, string> y)
        {
            return x.Key.CompareTo(y.Key) * -1;
        }

        private void profileTyposToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RegexTypos == null)
            {
                MessageBox.Show("No typos loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<KeyValuePair<Regex, string>> typos = RegexTypos.GetTypos();
            if (typos.Count == 0)
            {
                MessageBox.Show("No typos loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string text = txtEdit.Text;
            if (!txtEdit.Enabled || text.Length == 0)
            {
                MessageBox.Show("No article text", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Test typo rules for performance (this may take considerable time)?",
                "Test typos", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            int iterations = 1000000 / text.Length;
            if (iterations > 500) iterations = 500;

            List<KeyValuePair<int, string>> times = new List<KeyValuePair<int, string>>();

            foreach (KeyValuePair<Regex, string> p in typos)
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                for (int i = 0; i < iterations; i++)
                {
                    p.Key.IsMatch(text);
                }
                times.Add(new KeyValuePair<int, string>((int)watch.ElapsedMilliseconds,
                    p.Key.ToString() + " > " + p.Value));
            }

            times.Sort(new Comparison<KeyValuePair<int, string>>(CompareRegexPairs));

            using (StreamWriter sw = new StreamWriter("typos.txt", false, Encoding.UTF8))
            {
                foreach (KeyValuePair<int, string> p in times) sw.WriteLine(p);
            }

            MessageBox.Show("Results are saved in the file 'typos.txt'", "Profiling complete",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void loadPluginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PluginManager.LoadNewPlugin(this);
        }

        private void managePluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PluginManager manager = new PluginManager(this);
            manager.ShowDialog(this);
        }

        private void menuitemMakeFromTextBoxUndo_Click(object sender, EventArgs e)
        {
            listMaker1.txtSelectSource.Undo();
        }

        private void menuitemMakeFromTextBoxCut_Click(object sender, EventArgs e)
        {
            listMaker1.txtSelectSource.Cut();
        }

        private void menuitemMakeFromTextBoxCopy_Click(object sender, EventArgs e)
        {
            listMaker1.txtSelectSource.Copy();
        }

        private void menuitemMakeFromTextBoxPaste_Click(object sender, EventArgs e)
        {
            listMaker1.txtSelectSource.Paste();
        }

        private void mnuCopyToCategoryLog_Click(object sender, EventArgs e)
        {
            if (listMaker1.txtSelectSource.SelectionLength > 0)
                loggingSettings1.LoggingCategoryTextBox.Text = listMaker1.txtSelectSource.SelectedText;
            else
                loggingSettings1.LoggingCategoryTextBox.Text = listMaker1.txtSelectSource.Text;
        }

        private void ListMakerSourceSelectHandler(object sender, EventArgs e)
        {
            bool cats = (listMaker1.SelectedSource == SourceType.Category || listMaker1.SelectedSource == SourceType.CategoryRecursive);
            toolStripSeparatorMakeFromTextBox.Visible = cats;
            mnuCopyToCategoryLog.Visible = cats;
        }

        private void externalProcessingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            externalProgram.Show();
        }

        frmCategoryName CatName = new frmCategoryName();

        private void categoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CatName.ShowDialog();

            if (!string.IsNullOrEmpty(CatName.CategoryName))
            {
                txtEdit.Text += "\r\n\r\n[[" + CatName.CategoryName + "]]";
                reparseEditBox();
            }
        }

        private void UsageStatsMenuItem_Click(object sender, EventArgs e)
        { UsageStats.OpenUsageStatsURL(); }


    }
        #endregion
}
