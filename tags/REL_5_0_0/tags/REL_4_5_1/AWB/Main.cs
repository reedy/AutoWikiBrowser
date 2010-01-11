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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Diagnostics;
using WikiFunctions;
using WikiFunctions.API;
using WikiFunctions.Plugin;
using WikiFunctions.Parse;
using WikiFunctions.Properties;
using WikiFunctions.Browser;
using WikiFunctions.Controls;
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
        private static readonly Splash splash = new Splash();
        private static WikiFunctions.Profiles.AWBProfilesForm profiles;

        private static bool Abort;

        private static string LastArticle = "";
        private static string mSettingsFile = "";
        private static string LastMove = "", LastDelete = "", LastProtect = "";

        private static int oldselection;
        private static int retries;

        private static bool PageReload;
        private static int mnudges;
        private static int sameArticleNudges;

        private static readonly HideText RemoveText = new HideText(false, true, false);
        private static readonly List<string> noParse = new List<string>();
        private static readonly FindandReplace findAndReplace = new FindandReplace();
        private static readonly SubstTemplates substTemplates = new SubstTemplates();
        private static RegExTypoFix RegexTypos;
        private static readonly SkipOptions Skip = new SkipOptions();
        private static readonly WikiFunctions.ReplaceSpecial.ReplaceSpecial replaceSpecial =
            new WikiFunctions.ReplaceSpecial.ReplaceSpecial();
        private static Parsers parsers;
        private static readonly TimeSpan StartTime =
            new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        private static readonly List<string> RecentList = new List<string>();
        private static readonly CustomModule cModule = new CustomModule();
        private static readonly ExternalProgram externalProgram = new ExternalProgram();
        internal static RegexTester regexTester = new RegexTester();
        private static bool userTalkWarningsLoaded;
        private static Regex userTalkTemplatesRegex;
        private static bool mErrorGettingLogInStatus;
        private static bool skippable = true;
        private static FormWindowState LastState = FormWindowState.Normal; // doesn't look like we can use RestoreBounds for this - any other built in way?

        private ArticleRedirected ArticleWasRedirected;

        private ListComparer listComparer;
        private ListSplitter splitter;

        List<TypoStat> typoStats;

        private static readonly Help helpForm = new Help();

        private readonly WikiDiff diff = new WikiDiff();

        /// <summary>
        /// Whether AWB is currently shutting down
        /// </summary>
        public bool Shutdown { get; private set; }

        #endregion

        #region Constructor and MainForm load/resize
        public MainForm()
        {
            splash.Show(this);
            RightToLeft = System.Globalization.CultureInfo.CurrentCulture.TextInfo.IsRightToLeft
                ? RightToLeft.Yes : RightToLeft.No;
            InitializeComponent();
            Tools.RegistryMigration();
            splash.SetProgress(5);
            try
            {
                Icon = Resources.AWBIcon;
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
                try
                {
                    parsers = new Parsers(Properties.Settings.Default.StubMaxWordCount,
                        Properties.Settings.Default.AddHummanKeyToCats);
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
                listMaker.UserInputTextBox.ContextMenuStrip = mnuMakeFromTextBox;
                listMaker.BusyStateChanged += SetProgressBar;
                listMaker.NoOfArticlesChanged += UpdateButtons;
                listMaker.StatusTextChanged += UpdateListStatus;
                listMaker.cmboSourceSelect.SelectedIndexChanged += ListMakerSourceSelectHandler;

                profiles = new WikiFunctions.Profiles.AWBProfilesForm(webBrowserEdit);
                profiles.LoadProfile += LoadProfileSettings;

                splash.SetProgress(15);

                WikiFunctions.Profiles.AWBProfiles.ResetTempPassword();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        private static string SettingsFileDisplay;
        internal string SettingsFile
        {
            set
            {
                mSettingsFile = value;
                SettingsFileDisplay = Program.NAME;
                if (!string.IsNullOrEmpty(value))
                    SettingsFileDisplay += " - " + value.Remove(0, value.LastIndexOf("\\") + 1);
                Text = SettingsFileDisplay;

                ntfyTray.Text = (SettingsFileDisplay.Length > 64) ? SettingsFileDisplay.Substring(0, 63) : SettingsFileDisplay;
            }
            get { return mSettingsFile; }
        }

        int userProfileToLoad = -1;
        public int ProfileToLoad
        {
            set { userProfileToLoad = value; }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            EditBoxTab.TabPages.Remove(tpTypos);

            StatusLabelText = "Initialising...";
            splash.SetProgress(20);
            Variables.MainForm = this;
            lblOnlyBots.BringToFront();
            Updater.UpdateAWB(splash.SetProgress); // progress 22-29 in UpdateAWB()
            splash.SetProgress(30);

            Program.MyTrace.LS = loggingSettings1;

            try
            {
                //check that we are not using an old OS. 98 seems to mangled some unicode
                if (Environment.OSVersion.Version.Major < 5)
                {
                    MessageBox.Show(
                        "You appear to be using an older operating system, this software may have trouble with some unicode fonts on operating systems older than Windows 2000, the start button has been disabled.",
                        "Operating system", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetStartButton(false);
                    listMaker.MakeListEnabled = false;
                }

                webBrowserDiff.Navigate("about:blank");
                webBrowserDiff.ObjectForScripting = this;

                splash.SetProgress(35);
                if (Properties.Settings.Default.LogInOnStart)
                    CheckStatus(false);

                logControl.Initialise(listMaker);

                Location = Properties.Settings.Default.WindowLocation;

                Size = Properties.Settings.Default.WindowSize;

                WindowState = Properties.Settings.Default.WindowState;

                Debug();
                Release();

                Plugin.LoadPluginsStartup(this, splash); // progress 65-79 in LoadPlugins()

                LoadPrefs(); // progress 80-85 in LoadPrefs()

                splash.SetProgress(86);
                UpdateButtons(null, null);
                splash.SetProgress(88);
                LoadRecentSettingsList(); // progress 89-94 in LoadRecentSettingsList()
                splash.SetProgress(95);

                switch (Variables.User.CheckEnabled())
                {
                    case WikiStatusResult.OldVersion:
                        OldVersion();
                        break;
                    case WikiStatusResult.Error:
                        lblUserName.BackColor = Color.Red;
                        MessageBox.Show(this, "Cannot load version check page from Wikipedia. "
                                              + "Please verify that you're connected to Internet.", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

                if (userProfileToLoad != -1)
                {
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
            if (WindowState == FormWindowState.Minimized)
            {
                if (Minimize) Visible = false;
            }
            else
                LastState = WindowState; // remember if maximised or normal so can restore same when dbl click tray icon
        }
        #endregion

        #region Properties

        internal ArticleEX TheArticle { get; private set; }

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
                Thread.CurrentThread.Priority = value ? ThreadPriority.Lowest : ThreadPriority.Normal;
            }
        }

        private bool Flash;
        private bool Beep;

        /// <summary>
        /// True if AWB should be minimised to the system tray; False if it should minimise to the taskbar
        /// </summary>
        private bool Minimize;

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

        private bool SaveArticleList = true;
        private bool AutoSaveEditBoxEnabled;

        private string AutoSaveEditBoxFile = Application.StartupPath + "\\Edit Box.txt";

        private bool SuppressUsingAWB;

        /// <summary>
        /// Setting: Add "Using AWB" to the summary when deleting or protecting an article?
        /// </summary>
        private bool AddUsingAWBOnArticleAction;

        private decimal dAutoSaveEditPeriod = 60;
        private decimal AutoSaveEditBoxPeriod
        {
            get { return dAutoSaveEditPeriod; }
            set { dAutoSaveEditPeriod = value; EditBoxSaveTimer.Interval = int.Parse((value * 1000).ToString()); }
        }

        internal void SetStatusLabelText(string text)
        {
            StatusLabelText = text;
        }

        internal string StatusLabelText
        {
            get { return lblStatusText.Text; }
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new GenericDelegate1Parm(SetStatusLabelText), new object[] { value });
                    return;
                }

                if (string.IsNullOrEmpty(value))
                    lblStatusText.Text = Program.NAME + " " + Program.VersionString;
                else
                    lblStatusText.Text = value;
                Application.DoEvents();
            }
        }

        private bool IgnoreNoBots;
        private bool AddIgnoredToLogFile
        {
            set
            {
                btnStop.Location = (value) ? new Point(220, 62) : new Point(156, 62);
                btnStop.Size = (value) ? new Size(51, 23) : new Size(117, 23);

                btnFalsePositive.Visible = btntsFalsePositive.Visible = value;
            }
        }

        bool showTimer = true;
        private bool ShowMovingAverageTimer
        {
            set
            {
                showTimer = value;
                ShowTimer();
            }
            get { return showTimer; }
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

        private ApiEdit apiEdit = new ApiEdit(Variables.URLLong, Variables.PHP5);
        private void StartAPITextLoad(string title)
        {
            string pageText = apiEdit.Open(title);

            if (!LoadSuccessAPI())
                return;

            CaseWasLoad(pageText);
        }

        private bool stopProcessing;

        private void Start()
        {
            if (stopProcessing)
                return;

            try
            {
                Tools.WriteDebug(Name, "Starting");

                CanShutdown();

                //check edit summary
                txtEdit.Enabled = true;
                SetEditToolBarEnabled(true);

                if (Variables.Project != ProjectEnum.custom && string.IsNullOrEmpty(cmboEditSummary.Text) && Plugin.Items.Count == 0)
                {
                    MessageBox.Show("Please enter an edit summary.", "Edit summary", MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                    Stop();
                    return;
                }

                if (!string.IsNullOrEmpty(cmboEditSummary.Text) && !cmboEditSummary.Items.Contains(cmboEditSummary.Text))
                    cmboEditSummary.Items.Add(cmboEditSummary.Text);

                txtReviewEditSummary.Text = "";

                StopDelayedRestartTimer();
                DisableButtons();

                skippable = true;
                txtEdit.Clear();

                ArticleInfo(true);

                if (listMaker.NumberOfArticles < 1)
                {
                    webBrowserEdit.Busy = false;
                    StopSaveInterval();
                    lblTimer.Text = "";
                    StatusLabelText = "No articles in list, you need to use the Make list";
                    Text = Program.NAME;
                    listMaker.MakeListEnabled = true;
                    return;
                }

                webBrowserEdit.Busy = true;

                if (!Tools.IsValidTitle(listMaker.SelectedArticle().Name))
                {
                    SkipPage("Invalid page title");
                    return;
                }

                if (BotMode)
                    NudgeTimer.StartMe();

                TheArticle = new ArticleEX(listMaker.SelectedArticle().Name);

                NewHistory();

                EditBoxSaveTimer.Enabled = AutoSaveEditBoxEnabled;

                //if (dlg != null && dlg.AutoProtectAll)
                //    webBrowserEdit.ProtectPage(TheArticle.Name, dlg.Summary, dlg.EditProtectionLevel, dlg.MoveProtectionLevel, dlg.ProtectExpiry);

                //Navigate to edit page
                if (preParseModeToolStripMenuItem.Checked)
                    StartAPITextLoad(TheArticle.Name);
                else
                {
                    webBrowserEdit.BringToFront();

                    if (webBrowserEdit.IsBusy)
                        webBrowserEdit.Stop();

                    if (webBrowserEdit.Document != null)
                        webBrowserEdit.Document.Write("");

                    //check we are logged in
                    if (!Variables.User.WikiStatus && !CheckStatus(false))
                        return;

                    webBrowserEdit.Busy = true;

                    //Navigate to edit page
                    webBrowserEdit.LoadEditPage(TheArticle.Name);
                }
            }
            catch (Exception ex)
            {
                Tools.WriteDebug(Name, "Start() error: " + ex.Message);
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
            listMaker.Remove(TheArticle);
            Start();
        }

        private void CaseWasLoad(object sender, EventArgs e)
        {
            if (!LoadSuccess()) return;

            CaseWasLoad(webBrowserEdit.GetArticleText());
        }

        private void CaseWasLoad(string articleText)
        {
            if (stopProcessing)
                return;

            if (!CheckLoginStatus()) return;

            if (Program.MyTrace.HaveOpenFile)
                Program.MyTrace.WriteBulletedLine("AWB started processing", true, true, true);
            else
                Program.MyTrace.Initialise();

            Text = SettingsFileDisplay + " - " + TheArticle.Name;

            bool articleIsRedirect = Tools.IsRedirect(articleText);

            if (chkSkipIfRedirect.Checked && articleIsRedirect)
            {
                SkipPage("Page is a redirect");
                return;
            }

            //check for redirect
            if (bypassRedirectsToolStripMenuItem.Checked && articleIsRedirect && !PageReload)
            {
                // Warning: Creating an ArticleEX causes a new AWBLogListener to be created and it becomes the active listener in MyTrace; be careful we're writing to the correct log listener
                ArticleEX redirect = new ArticleEX(Tools.RedirectTarget(articleText));

                if (redirect.Name.Trim() != "" && Tools.IsValidTitle(redirect.Name))
                {
                    if (filterOutNonMainSpaceToolStripMenuItem.Checked && (redirect.NameSpaceKey != 0))
                    {
                        listMaker.Remove(TheArticle); // or we get stuck in a loop
                        TheArticle = redirect; // if we didn't do this, we were writing the SkipPage info to the AWBLogListener belonging to the object redirect and resident in the MyTrace collection, but then attempting to add TheArticle's log listener to the logging tab
                        SkipPage("Page is not in mainspace");
                        return;
                    }

                    if (redirect.Name == TheArticle.Name)
                    {//ignore recursive redirects
                        TheArticle = redirect;
                        SkipPage("Recursive redirect");
                        return;
                    }
                    if (ArticleWasRedirected != null)
                        ArticleWasRedirected(TheArticle.Name, redirect.Name);

                    listMaker.ReplaceArticle(TheArticle, new Article(redirect.Name));
                    TheArticle = new ArticleEX(redirect.Name);

                    if (preParseModeToolStripMenuItem.Checked)
                        StartAPITextLoad(redirect.Name);
                    else
                        webBrowserEdit.LoadEditPage(redirect.Name);

                    return;
                }
            }

            if (!preParseModeToolStripMenuItem.Checked && webBrowserEdit.EditBoxTag.Contains("readonly=\"readonly\""))
            {
                if (!webBrowserEdit.UserAllowedToEdit())
                {
                    NudgeTimer.Stop();
                    SkipPage("Database is locked");
                    return;
                }
            }

            TheArticle.OriginalArticleText = articleText;

            int.TryParse(webBrowserEdit.GetScriptingVar("wgCurRevisionId"), out ErrorHandler.CurrentRevision);

            if (PageReload)
            {
                PageReload = false;
                GetDiff();
                return;
            }

            if (!preParseModeToolStripMenuItem.Checked && SkipChecks()) // normal mode
                return;

            //check not in use
            if (TheArticle.IsInUse)
                if (chkSkipIfInuse.Checked)
                {
                    SkipPage("Page contains {{inuse}}");
                    return;
                }
                else if (!BotMode && !preParseModeToolStripMenuItem.Checked)
                    MessageBox.Show("This page has the \"Inuse\" tag, consider skipping it");

            // check for {{sic}} tags etc. when doing typo fixes and not in pre-parse mode
            if(chkRegExTypo.Checked && !preParseModeToolStripMenuItem.Checked && TheArticle.HasSicTag)
                MessageBox.Show(@"This page contains a 'sic' tag or template, please take extra care when correcting typos.", "'sic' tag in page", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            if (automaticallyDoAnythingToolStripMenuItem.Checked)
            {
                StatusLabelText = "Processing page";
                Application.DoEvents();

                //FIXME: this position is imprefect, since above there is code that can explode, but this way
                //at least we don't get bogus reports of unrelated pages
                ErrorHandler.CurrentPage = TheArticle.Name;

                ProcessPage(TheArticle, true);

                ErrorHandler.CurrentPage = "";

                UpdateWebBrowserStatus(null, null);
                UpdateCurrentTypoStats();

                if (!Abort)
                {
                    if (TheArticle.SkipArticle)
                    {
                        SkipPageReasonAlreadyProvided(); // Don't send a reason; ProcessPage() should already have logged one
                        return;
                    }

                    if (skippable && (chkSkipNoChanges.Checked || BotMode) && TheArticle.NoArticleTextChanged)
                    {
                        SkipPage("No change");
                        return;
                    }

                    if (chkSkipWhitespace.Checked && chkSkipCasing.Checked && TheArticle.OnlyWhiteSpaceAndCasingChanged)
                    {
                        SkipPage("Only whitespace/casing changed");
                        return;
                    }

                    if (chkSkipWhitespace.Checked && TheArticle.OnlyWhiteSpaceChanged)
                    {
                        SkipPage("Only whitespace changed");
                        return;
                    }

                    if (chkSkipCasing.Checked && TheArticle.OnlyCasingChanged)
                    {
                        SkipPage("Only casing changed");
                        return;
                    }
                    
                    if(chkSkipMinorGeneralFixes.Checked && chkGeneralFixes.Checked && TheArticle.OnlyMinorGeneralFixesChanged)
                    {
                        SkipPage("Only minor general fix changes");
                        return;
                    }

                    if (chkSkipGeneralFixes.Checked && chkGeneralFixes.Checked && TheArticle.OnlyGeneralFixesChanged)
                    {
                        SkipPage("Only general fix changes");
                        return;
                    }
                }
            }

            if (preParseModeToolStripMenuItem.Checked)
            {
                if (SkipChecks())
                    return;

                // if we reach here the article has valid changes, so move on to next article

                // if user has loaded a settings file, save it every 10 ignored edits
                if (!string.IsNullOrEmpty(SettingsFile) && (NumberOfIgnoredEdits > 5) && (NumberOfIgnoredEdits % 10 == 0))
                    SavePrefs(SettingsFile);

                // request list maker to focus next article in list; if there is a next article process it, otherwise pre-parsing has finished, save settings
                // but don't save when settings have just been saved by logic above
                if (listMaker.NextArticle())
                    Start();
                else
                {
                    Stop();
                    if (!string.IsNullOrEmpty(SettingsFile) && !(NumberOfIgnoredEdits % 10 == 0))
                        SavePrefs(SettingsFile);
                }

                return;
            }

            webBrowserEdit.SetArticleText(TheArticle.ArticleText);
            txtEdit.Text = TheArticle.ArticleText;

            //Update statistics and alerts
            ArticleInfo(false);

            if (chkSkipNoPageLinks.Checked && (lblLinks.Text == "Links: 0"))
            {
                SkipPage("Page contains no links");
                return;
            }

            if (!Abort)
            {
                if (BotMode)
                {
                    StartDelayedAutoSaveTimer();
                    return;
                }

                switch (toolStripComboOnLoad.SelectedIndex)
                {
                    case 0:
                        GetDiff();
                        break;
                    case 1:
                        GetPreview();
                        break;
                    case 2:
                        GuiUpdateAfterProcessing();

                        txtEdit.Focus();
                        txtEdit.SelectionLength = 0;
                        break;
                }

                SetWatchButton(webBrowserEdit.IsWatched());

                txtReviewEditSummary.Text = MakeSummary();

                if (focusAtEndOfEditTextBoxToolStripMenuItem.Checked)
                {
                    txtEdit.Select(txtEdit.Text.Length, 0);
                    txtEdit.ScrollToCaret();
                }
                else
                    btnSave.Focus();
            }
            else
            {
                EnableButtons();
                Abort = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Whether the page has been skipped</returns>
        private bool SkipChecks()
        {
            if (chkSkipIfContains.Checked && TheArticle.SkipIfContains(txtSkipIfContains.Text,
                chkSkipIsRegex.Checked, chkSkipCaseSensitive.Checked, true))
            {
                SkipPage("Page contains: " + txtSkipIfContains.Text);
                return true;
            }

            if (chkSkipIfNotContains.Checked && TheArticle.SkipIfContains(txtSkipIfNotContains.Text,
                chkSkipIsRegex.Checked, chkSkipCaseSensitive.Checked, false))
            {
                SkipPage("Page does not contain: " + txtSkipIfNotContains.Text);
                return true;
            }

            if (!Skip.SkipIf(TheArticle.OriginalArticleText))
            {
                SkipPage("skipIf custom code");
                return true;
            }

            return false;
        }

        private void Bleepflash()
        {
            if (ContainsFocus) return;
            if (Flash) Tools.FlashWindow(this);
            if (Beep) Tools.Beep();
        }

        private readonly TalkMessage DlgTalk = new TalkMessage();

        private bool LoadSuccess()
        {
            try
            {
                if (!webBrowserEdit.Url.ToString().StartsWith(Variables.URLLong))
                {
                    SkipPage("Interwiki in page title");
                    return false;
                }

                if (webBrowserEdit.DocumentText.Contains("<div class=\"permissions-errors\">"))
                {
                    SkipPage("User doesn't have permissions to edit this page.");
                    return false;
                }

                string HTML = null;
                if (webBrowserEdit.Document != null && webBrowserEdit.Document.Body != null)
                    HTML = webBrowserEdit.Document.Body.InnerHtml;

                if (string.IsNullOrEmpty(HTML) || IsReadOnlyDB(HTML))
                {
                    if (retries < 10)
                    {
                        StartDelayedRestartTimer(null, null);
                        retries++;
                        Start();
                        return false;
                    }

                    retries = 0;
                    if (!string.IsNullOrEmpty(HTML))
                        SkipPage("Database is locked, tried 10 times");
                    else
                    {
                        MessageBox.Show("Loading edit page failed after 10 retries. Processing stopped.", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Stop();
                    }
                    return false;
                }

                if (HTML.Contains("Sorry! We could not process your edit due to a loss of session data. Please try again. If it still doesn't work, try logging out and logging back in."))
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

                if (!preParseModeToolStripMenuItem.Checked && webBrowserEdit.NewMessage)
                { //check if we have any messages
                    NudgeTimer.Stop();
                    Variables.User.WikiStatus = false;
                    UpdateButtons(null, null);
                    webBrowserEdit.Document.Write("");
                    Focus();

                    if (DlgTalk.ShowDialog() == DialogResult.Yes)
                        Tools.OpenUserTalkInBrowser();
                    else
                        Process.Start("iexplore", Variables.GetUserTalkURL());
                    return false;
                }
                if (!webBrowserEdit.HasArticleTextBox)
                {
                    if (!BotMode)
                    {
                        SkipPage("There was a problem loading the page");
                        return false;
                    }

                    StatusLabelText = "There was a problem loading the page. Re-starting.";
                    StartDelayedRestartTimer(null, null);
                    return false;
                }

                HtmlElement wpt = webBrowserEdit.Document.GetElementById("wpTextbox1");
                bool wpTextbox1IsNull = (wpt != null && wpt.InnerText == null);

                if (wpTextbox1IsNull && radSkipNonExistent.Checked)
                {//check if it is a non-existent page, if so then skip it automatically.
                    SkipPage("Non-existent page");
                    return false;
                }
                if (!wpTextbox1IsNull && radSkipExistent.Checked)
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

        private bool LoadSuccessAPI()
        {
            try
            {
                //if (webBrowserEdit.DocumentText.Contains("<div class=\"permissions-errors\">"))
                //{
                //    SkipPage("User doesn't have permissions to edit this page.");
                //    return false;
                //}

                if (!apiEdit.Exists && radSkipNonExistent.Checked)
                {//check if it is a non-existent page, if so then skip it automatically.
                    SkipPage("Non-existent page");
                    return false;
                }
                if (apiEdit.Exists && radSkipExistent.Checked)
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
            txtEdit.Focus();
            txtEdit.SelectionLength = 0;

            GuiUpdateAfterProcessing();
        }

        static readonly Regex spamUrlRegex = new Regex("<p>The following link has triggered our spam protection filter:<strong>(.*?)</strong><br/?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private void CaseWasSaved(object sender, EventArgs e)
        {
            if (webBrowserEdit.DocumentText.Contains("<H1 class=firstHeading>Edit conflict: "))
            {//if session data is lost, if data is lost then save after delay with tmrAutoSaveDelay
                MessageBox.Show("There has been an Edit Conflict. AWB will now re-apply its changes on the updated page. \n\r Please re-review the changes before saving. Any Custom edits will be lost, and have to be re-added manually.", "Edit Conflict");
                NudgeTimer.Stop();
                Start();
                return;
            }
            if (!BotMode && webBrowserEdit.DocumentText.Contains("<DIV id=spamprotected>"))
            {//check edit wasn't blocked due to spam filter
                if (!chkSkipSpamFilter.Checked)
                {
                    Match m = spamUrlRegex.Match(webBrowserEdit.DocumentText);

                    string messageBoxText = "Edit has been blocked by spam blacklist.\r\n";

                    if (m.Success)
                        messageBoxText += "Spam URL: " + m.Groups[1].Value.Trim() + "\r\n";

                    if (MessageBox.Show(messageBoxText + "Try and edit again?", "Spam blacklist", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        Start();
                        return;
                    }
                }
                
                SkipPage("Edit blocked by spam protection filter");
                return;
            }

            if (webBrowserEdit.DocumentText.Contains("<DIV CLASS=PREVIEWNOTE"))
            {//if session data is lost, if data is lost then save after delay with tmrAutoSaveDelay
                StartDelayedRestartTimer(null, null);
                return;
            }

            if (webBrowserEdit.DocumentText.Contains("<h1 id=\"FoundationName\">Wikimedia Foundation</h1>"))
            {//WMF Error
                StartDelayedRestartTimer(null, null);
                return;
            }

            if (IsReadOnlyDB(webBrowserEdit.DocumentText))
            {
                StartDelayedRestartTimer(null, null);
                return;
            }

            //lower restart delay
            if (intRestartDelay > 5)
                intRestartDelay -= 1;

            NumberOfEdits++;

            LastArticle = "";
            listMaker.Remove(TheArticle);
            NudgeTimer.Stop();
            sameArticleNudges = 0;
            if (EditBoxTab.SelectedTab == tpHistory)
                EditBoxTab.SelectedTab = tpEdit;
            logControl.AddLog(false, TheArticle.LogListener);
            UpdateOverallTypoStats();

            if (listMaker.Count == 0 && AutoSaveEditBoxEnabled)
                EditBoxSaveTimer.Enabled = false;
            retries = 0;

            // if user has loaded a settings file, save it every 10 edits if autosavesettings is set
            if (autoSaveSettingsToolStripMenuItem.Checked && !string.IsNullOrEmpty(SettingsFile) && (NumberOfEdits > 5) && (NumberOfEdits % 10 == 0))
                SavePrefs(SettingsFile);

            Start();
        }

        private void CaseWasNull(object sender, EventArgs e)
        {
            if (webBrowserEdit.DocumentText.Contains("var wgAction = \"submitlogin\";"))
            {
                StatusLabelText = "Signed in, now re-starting";

                if (!Variables.User.WikiStatus)
                    CheckStatus(false);
            }
        }

        private static bool IsReadOnlyDB(string html)
        {//Read-Only DB - http://en.wikipedia.org/wiki/MediaWiki:Readonlytext
            return (html.Contains("<div class=\"mw-readonly-error\">"));
        }

        private void SkipPageReasonAlreadyProvided()
        {
            try
            {
                //reset timer.
                NumberOfIgnoredEdits++;
                StopDelayedAutoSaveTimer();
                NudgeTimer.Stop();
                listMaker.Remove(TheArticle);
                sameArticleNudges = 0;
                logControl.AddLog(true, TheArticle.LogListener);
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

        /// <summary>
        /// Fully processes a page, applying all needed changes
        /// </summary>
        /// <param name="theArticle">Page to process</param>
        /// <param name="mainProcess">True if the page is being processed for save as usual,
        /// otherwise (Re-parse in context menu, prefetch, etc) false</param>
        private void ProcessPage(ArticleEX theArticle, bool mainProcess)
        {
            bool process = true;
            typoStats = null;

#if DEBUG
            Variables.Profiler.Start("ProcessPage(\"" + theArticle.Name + "\")");
#endif

            try
            {
                if (noParse.Contains(theArticle.Name))
                    process = false;

                if (!IgnoreNoBots &&
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

                    Variables.Profiler.Profile("F&R");

                    if (theArticle.SkipArticle) return;
                }

                // RegexTypoFix
                if (chkRegExTypo.Checked && RegexTypos != null && !BotMode && !Tools.IsTalkPage(theArticle.NameSpaceKey))
                {
                    theArticle.PerformTypoFixes(RegexTypos, chkSkipIfNoRegexTypo.Checked);
                    Variables.Profiler.Profile("Typos");
                    typoStats = RegexTypos.GetStatistics();
                    if (theArticle.SkipArticle)
                    {
                        if (mainProcess)
                        {   // update stats only if not called from e.g. 'Re-parse' than could be clicked repeatedly
                            OverallTypoStats.UpdateStats(typoStats, true);
                            UpdateTypoCount();
                        }
                        return;
                    }
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
                    if (process)
                    {
                        theArticle.AutoTag(parsers, Skip.SkipNoTag, chkAutoTagger.Checked, chkGeneralFixes.Checked);
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
                    if (!userTalkWarningsLoaded)
                    {
                        LoadUserTalkWarnings();
                        Variables.Profiler.Profile("loadUserTalkWarnings");
                    }

                    theArticle.PerformUserTalkGeneralFixes(RemoveText, userTalkTemplatesRegex, Skip.SkipNoUserTalkTemplatesSubstd);
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
                            theArticle.ArticleText + newlines + Tools.ApplyKeyWords(theArticle.Name, txtAppendMessage.Text), false);
                    else
                        theArticle.AWBChangeArticleText("Prepended your message",
                            Tools.ApplyKeyWords(theArticle.Name, txtAppendMessage.Text) + newlines + theArticle.ArticleText, false);
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
                        Abort = true;
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
                if (webBrowserDiff.Document == null)
                    return;

                webBrowserDiff.Document.OpenNew(false);
                if (TheArticle.OriginalArticleText == txtEdit.Text)
                {
                    webBrowserDiff.Document.Write(
                        @"<h2 style='padding-top: .5em;
padding-bottom: .17em;
border-bottom: 1px solid #aaa;
font-size: 150%;'>No changes</h2><p>Press the ""Ignore"" button below to skip to the next page.</p>");
                }
                else
                {
                    webBrowserDiff.Document.Write("<!DOCTYPE HTML PUBLIC \" -//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">"
                                                  + "<html><head>" +
                                                  WikiDiff.DiffHead() + @"</head><body>" + WikiDiff.TableHeader +
                                                  diff.GetDiff(TheArticle.OriginalArticleText, txtEdit.Text, 2) +
                                                  @"</table><!--<script language='Javascript'>
// Scroll part of the way into the table, disabled due to other interface problems
diffNode=document.getElementById('wikiDiff');
var diffTopY = 0;
while(diffNode) {
    diffTopY += diffNode.offsetTop;
    diffNode = diffNode.offsetParent;
}                        		      
window.scrollTo(0, diffTopY);
</script>--></body></html>");
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
            if (webBrowserEdit.Loading) return;
            webBrowserEdit.BringToFront();
            webBrowserEdit.SetArticleText(txtEdit.Text);

            LastArticle = txtEdit.Text;

            skippable = false;
            webBrowserEdit.ShowPreview();

            GuiUpdateAfterProcessing();
        }

        private void GuiUpdateAfterProcessing()
        {
            Bleepflash();
            Focus();
            EnableButtons();
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

            if (ShowMovingAverageTimer)
            {
                StopSaveInterval();
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
                TheArticle.EditSummary = "";
                GetDiff();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        public void UndoDeletion(int left, int right)
        {
            try
            {
                txtEdit.Text = diff.UndoDeletion(left, right);
                TheArticle.EditSummary = "";
                GetDiff();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        public void UndoAddition(int right)
        {
            try
            {
                txtEdit.Text = diff.UndoAddition(right);
                TheArticle.EditSummary = "";
                GetDiff();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
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

        Point oldPosition;
        Size oldSize;
        private void ParametersShowHide()
        {
            enlargeEditAreaToolStripMenuItem.Checked = !enlargeEditAreaToolStripMenuItem.Checked;
            if (groupBox2.Visible)
            {
                btntsShowHideParameters.Image = Resources.Showhideparameters2;

                oldPosition = EditBoxTab.Location;
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
            StatusLabelText = listMaker.Status;
        }

        private void UpdateCurrentTypoStats()
        {
            CurrentTypoStats.UpdateStats(typoStats, false);
        }

        private void UpdateOverallTypoStats()
        {
            if (chkRegExTypo.Checked)
                OverallTypoStats.UpdateStats(typoStats, false);
            UpdateTypoCount();
        }

        private void UpdateTypoCount()
        {
            if (OverallTypoStats.Saves > 0)
            {
                //work around CS1690 warning
                int total = OverallTypoStats.TotalTypos;
                lblOverallTypos.Text = total.ToString();

                int selfMatches = OverallTypoStats.SelfMatches;
                lblNoChange.Text = selfMatches.ToString();

                lblTypoRatio.Text = OverallTypoStats.TyposPerSave;
            }
            else
                lblTypoRatio.Text = "0";
        }

        private void ResetTypoStats()
        {
            CurrentTypoStats.ClearStats();
            OverallTypoStats.ClearStats();
            UpdateTypoCount();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExitQuestion dlg = null;
            WebControl.Shutdown = true;

            Properties.Settings.Default.WindowState = WindowState;

            if (WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.WindowSize = Size;
                Properties.Settings.Default.WindowLocation = Location;
            }
            else
            {
                Properties.Settings.Default.WindowSize = RestoreBounds.Size;
                Properties.Settings.Default.WindowLocation = RestoreBounds.Location;
            }

            Properties.Settings.Default.Save();

            if (Properties.Settings.Default.AskForTerminate)
            {
                TimeSpan time = new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                time = time.Subtract(StartTime);
                dlg = new ExitQuestion(time, NumberOfEdits, "");
                dlg.ShowDialog();
                Properties.Settings.Default.AskForTerminate = !dlg.CheckBoxDontAskAgain;
            }

            if (!Properties.Settings.Default.AskForTerminate || (dlg != null && dlg.DialogResult == DialogResult.OK))
            {
                Shutdown = true;
                // save user persistent settings
                Properties.Settings.Default.Save();

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
            if (webBrowserEdit.Document != null && webBrowserEdit.DocumentText.Contains("wpMinoredit"))
            {
                // Warning: Plugins can call SetMinor and SetWatch, so only turn these *on* not off
                if (markAllAsMinorToolStripMenuItem.Checked)
                    webBrowserEdit.SetMinor(true);
                if (addAllToWatchlistToolStripMenuItem.Checked)
                    webBrowserEdit.SetWatch(true);
                if (dontAddToWatchlistToolStripMenuItem.Checked)
                    webBrowserEdit.SetWatch(false);

                webBrowserEdit.SetSummary(MakeSummary());
            }
        }

        private string MakeSummary()
        {
            if (TheArticle == null)
                return "";

            string tag = cmboEditSummary.Text + TheArticle.EditSummary;

            if ((Variables.User.IsBot && chkSuppressTag.Checked)
                || (!Variables.IsWikimediaProject && SuppressUsingAWB))
                return tag;

            int maxSummaryLength = (200 - (Variables.SummaryTag.Length + 1));

            if (tag.Length >= maxSummaryLength)
            {
                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#Edit_summary_issue
                // replace last wikilink with dots as an attempt to prevent broken wikilinks in edit summary
                if (tag.EndsWith(@"]]"))
                    tag = Regex.Replace(tag, @"\s*\[\[.+?\]\]$", "...");

                tag = tag.Substring(0, maxSummaryLength);
            }

            tag += Variables.SummaryTag;

            return tag;
        }

        private void chkFindandReplace_CheckedChanged(object sender, EventArgs e)
        {
            btnMoreFindAndReplce.Enabled = chkFindandReplace.Checked;
            btnFindAndReplaceAdvanced.Enabled = chkFindandReplace.Checked;
            chkSkipWhenNoFAR.Enabled = chkFindandReplace.Checked;
            btnSubst.Enabled = chkFindandReplace.Checked;
        }
        
        private void chkSkipGeneralFixes_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSkipGeneralFixes.Checked)
                chkSkipMinorGeneralFixes.Enabled = chkSkipMinorGeneralFixes.Checked = false;
            else
                chkSkipMinorGeneralFixes.Enabled = true;
        }

        private void cmboCategorise_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtNewCategory.Enabled = chkSkipNoCatChange.Enabled = (cmboCategorise.SelectedIndex > 0);

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
        }

        private void web4Completed(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            StopProgressBar();
        }

        private void web4Starting(object sender, WebBrowserNavigatingEventArgs e)
        {
            StartProgressBar();
        }

        private void UpdateBotStatus(object sender, EventArgs e)
        {
            chkAutoMode.Enabled = chkSuppressTag.Enabled = Variables.User.IsBot;

            lblOnlyBots.Visible = !Variables.User.IsBot;

            if (Variables.User.IsBot && !MainTab.TabPages.Contains(tpBots))
            {
                MainTab.TabPages.Insert(MainTab.TabPages.IndexOf(tpStart), tpBots);
            }
            else
            {
                if (MainTab.TabPages.Contains(tpBots))
                    MainTab.Controls.Remove(tpBots);

                if (BotMode)
                    BotMode = false;
            }
        }

        private void UpdateAdminStatus(object sender, EventArgs e)
        {
            btnProtect.Enabled = btnMove.Enabled = btnDelete.Enabled = btntsDelete.Enabled = (Variables.User.IsAdmin && btnSave.Enabled && (TheArticle != null));
        }

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
                    MessageBox.Show("Sorry, bot mode cannot be used with RegExTypoFix.\r\nRegExTypoFix will now be turned off", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
            label2.Enabled /*= chkSuppressTag.Enabled*/ = nudBotSpeed.Enabled
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

            bool b = false;
            string label = "Software disabled";

            switch (Variables.User.UpdateWikiStatus())
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
            RightToLeft = Variables.RTL ? RightToLeft.Yes : RightToLeft.No;

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
            Close();
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
            CategoryLeave(txtNewCategory);
        }

        private void txtNewCategory2_Leave(object sender, EventArgs e)
        {
            CategoryLeave(txtNewCategory2);
        }

        private static void CategoryLeave(TextBox catTextBox)
        {
            catTextBox.Text = catTextBox.Text.Trim('[', ']');
            catTextBox.Text = Regex.Replace(catTextBox.Text, "^" +
                Variables.NamespacesCaseInsensitive[Namespace.Category], "");
            catTextBox.Text = Tools.TurnFirstToUpper(catTextBox.Text);
        }

        private void ArticleInfo(bool reset)
        {
            lblWarn.Text = "";
            lbDuplicateWikilinks.Items.Clear();

            if (reset)
            {
                //Resets all the alerts.
                lblWords.Text = "Words: ";
                lblCats.Text = "Categories: ";
                lblImages.Text = "Images: ";
                lblLinks.Text = "Links: ";
                lblInterLinks.Text = "Interwiki links: ";
            }
            else
            {
                string articleText = txtEdit.Text;

                int intWords = Tools.WordCount(articleText);
                int intCats = WikiRegexes.Category.Matches(articleText).Count;
                int intImages = WikiRegexes.Images.Matches(articleText).Count;
                int intInterLinks = Tools.InterwikiCount(articleText);
                int intLinks = WikiRegexes.WikiLinksOnly.Matches(articleText).Count;

                intLinks = intLinks - intInterLinks - intImages - intCats;

                if (TheArticle.NameSpaceKey == 0 && (WikiRegexes.Stub.IsMatch(articleText)) && (intWords > 500))
                    lblWarn.Text = "Long article with a stub tag.\r\n";

                if (!(Regex.IsMatch(articleText, "\\[\\[" + Variables.Namespaces[Namespace.Category],
                    RegexOptions.IgnoreCase)))
                {
                    lblWarn.Text += "No category (although one may be in a template)\r\n";
                }

                if (articleText.StartsWith("=="))
                    lblWarn.Text += "Starts with heading.";

                lblWords.Text = "Words: " + intWords;
                lblCats.Text = "Categories: " + intCats;
                lblImages.Text = "Images: " + intImages;
                lblLinks.Text = "Links: " + intLinks;
                lblInterLinks.Text = "Interwiki links: " + intInterLinks;

                //Find multiple links                
                ArrayList arrayLinks = new ArrayList();

                //get all the links
                foreach (Match m in WikiRegexes.WikiLink.Matches(articleText))
                {
                    string x = m.Groups[1].Value;
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
            }
            lblDuplicateWikilinks.Visible = lbDuplicateWikilinks.Visible = btnRemove.Visible = (lbDuplicateWikilinks.Items.Count > 0);
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

            // After manual changes, automatic edit summary may be inaccurate, removing it altogether
            if (TheArticle != null)
                TheArticle.EditSummary = "";
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
            listMaker.Add("Project:AutoWikiBrowser/Sandbox");
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

        [Conditional("RELEASE")]
        public void Release()
        {
            if (MainTab.Contains(tpBots))
                MainTab.Controls.Remove(tpBots);
        }

        #endregion

        #region set variables

        private void PreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyPreferences myPrefs = new MyPreferences(Variables.LangCode, Variables.Project,
                Variables.CustomProject, txtEdit.Font, LowThreadPriority, Flash, Beep,
                Minimize, SaveArticleList, TimeOut, AutoSaveEditBoxEnabled, AutoSaveEditBoxFile,
                AutoSaveEditBoxPeriod, SuppressUsingAWB, AddUsingAWBOnArticleAction, IgnoreNoBots,
                ShowMovingAverageTimer, Variables.PHP5);

            if (myPrefs.ShowDialog(this) == DialogResult.OK)
            {
                txtEdit.Font = myPrefs.TextBoxFont;
                LowThreadPriority = myPrefs.LowThreadPriority;
                Flash = myPrefs.PrefFlash;
                Beep = myPrefs.PrefBeep;
                Minimize = myPrefs.PrefMinimize;
                SaveArticleList = myPrefs.PrefSaveArticleList;
                TimeOut = myPrefs.PrefTimeOutLimit;
                AutoSaveEditBoxEnabled = myPrefs.PrefAutoSaveEditBoxEnabled;
                AutoSaveEditBoxPeriod = myPrefs.PrefAutoSaveEditBoxPeriod;
                AutoSaveEditBoxFile = myPrefs.PrefAutoSaveEditBoxFile;
                SuppressUsingAWB = myPrefs.PrefSuppressUsingAWB;
                AddUsingAWBOnArticleAction = myPrefs.PrefAddUsingAWBOnArticleAction;
                IgnoreNoBots = myPrefs.PrefIgnoreNoBots;
                ShowMovingAverageTimer = myPrefs.PrefShowTimer;

                if (myPrefs.Language != Variables.LangCode || myPrefs.Project != Variables.Project)
                {
                    Variables.PHP5 = myPrefs.PrefPHP5;
                    SetProject(myPrefs.Language, myPrefs.Project, myPrefs.CustomProject);

                    Variables.User.WikiStatus = false;
                    BotMode = false;
                    lblOnlyBots.Visible = true;
                    Variables.User.IsBot = false;
                    Variables.User.IsAdmin = false;
                    apiEdit = new ApiEdit(Variables.URLLong, Variables.PHP5);
                }
            }
            ListMaker.AddRemoveRedirects();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //refresh login status, and reload check list
            CheckStatus(false);

            //refresh typo list
            LoadTypos(true);

            //refresh talk warnings list
            if (userTalkWarningsLoaded)
                LoadUserTalkWarnings();
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
                    humanNameDisambigTagToolStripMenuItem.Visible = birthdeathCatsToolStripMenuItem.Visible = false;
                    chkAutoTagger.Checked = false;
                }
                else if (!humanNameDisambigTagToolStripMenuItem.Visible)
                {
                    humanNameDisambigTagToolStripMenuItem.Visible = birthdeathCatsToolStripMenuItem.Visible = true;
                }

                userTalkWarningsLoaded = false; // force reload

                if (!Variables.IsCustomProject && !Variables.IsWikia && !Variables.IsWikimediaMonolingualProject)
                    lblProject.Text = Variables.LangCodeEnumString() + "." + Variables.Project;
                else lblProject.Text = Variables.IsWikimediaMonolingualProject ? Variables.Project.ToString() : Variables.URL;

                ResetTypoStats();
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("The interwiki list didn't load correctly. Please check your internet connection, and then restart AWB");
            }
        }

        #endregion

        #region Enabling/Disabling of buttons

        private void UpdateButtons(object sender, EventArgs e)
        {
            SetStartButton(listMaker.NumberOfArticles > 0);

            lbltsNumberofItems.Text = "Pages: " + listMaker.NumberOfArticles;
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

            if (listMaker.NumberOfArticles == 0)
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
            btntsPreview.Enabled = btntsChanges.Enabled = listMaker.MakeListEnabled =
            btntsSave.Enabled = btntsIgnore.Enabled = /*btnWatch.Enabled = */ findGroup.Enabled = enabled;

            btnDelete.Enabled = btntsDelete.Enabled = btnMove.Enabled = btnProtect.Enabled = (enabled && Variables.User.IsAdmin && (TheArticle != null));
        }

        #endregion

        #region Timers

        int intRestartDelay = 5, intStartInSeconds = 5;
        private void DelayedRestart(object sender, EventArgs e)
        {
            StopDelayedAutoSaveTimer();
            StatusLabelText = "Restarting in " + intStartInSeconds;

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

            if (intRestartDelay > 60)
                intRestartDelay = 60;
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
            lblBotTimer.Text = "Bot timer: " + intTimer;
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
                lblBotTimer.BackColor = (intTimer == 1) ? Color.Red : DefaultBackColor;
            }
            else
            {
                StopDelayedAutoSaveTimer();
                SaveArticle();
            }

            lblBotTimer.Text = "Bot timer: " + intTimer;
        }

        private void ShowTimer()
        {
            lblTimer.Visible = ShowMovingAverageTimer;
            StopSaveInterval();
        }

        int intStartTimer;
        private void SaveInterval(object sender, EventArgs e)
        {
            intStartTimer++;
            lblTimer.Text = "Timer: " + intStartTimer;
        }

        private void StopSaveInterval()
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

        int seconds, lastTotal;
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
            stopProcessing = false;
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
            listMaker.FilterNonMainAuto = filterOutNonMainSpaceToolStripMenuItem.Checked;

            if (filterOutNonMainSpaceToolStripMenuItem.Checked)
                listMaker.FilterNonMainArticles();
        }

        private void removeDuplicatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listMaker.FilterDuplicates = removeDuplicatesToolStripMenuItem.Checked;

            if (removeDuplicatesToolStripMenuItem.Checked)
                listMaker.RemoveListDuplicates();
        }

        private void specialFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            listMaker.Filter();
        }

        private void convertToTalkPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listMaker.ConvertToTalkPages();
        }

        private void convertFromTalkPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listMaker.ConvertFromTalkPages();
        }

        private void sortAlphabeticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listMaker.AutoAlpha = sortAlphabeticallyToolStripMenuItem.Checked;

            if (sortAlphabeticallyToolStripMenuItem.Checked)
                listMaker.AlphaSortList();
        }

        private void saveListToTextFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listMaker.SaveList();
        }

        private void launchListComparerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listMaker.Count > 0 && MessageBox.Show("Would you like to copy your current Article List to the ListComparer?", "Copy Article List?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                listComparer = new ListComparer(listMaker.GetArticleList());
            else
                listComparer = new ListComparer();

            listComparer.Show(this);
        }

        private void launchListSplitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WikiFunctions.AWBSettings.UserPrefs p = MakePrefs();

            if (listMaker.Count > 0 && MessageBox.Show("Would you like to copy your current Article List to the ListSplitter?", "Copy Article List?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                splitter = new ListSplitter(p, WikiFunctions.AWBSettings.UserPrefs.SavePluginSettings(p), listMaker.GetArticleList());
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
            WikiFunctions.DBScanner.DatabaseScanner ds =
                MessageBox.Show("Would you like the results to be added to the ListMaker Article List?",
                                "Add to ListMaker?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                DialogResult.Yes
                    ? listMaker.DBScanner()
                    : new WikiFunctions.DBScanner.DatabaseScanner();
            ds.Show();
            UpdateButtons(null, null);
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
                if (e.KeyCode == Keys.S)
                {
                    if (btnSave.Enabled)
                        Save();
                    else if (btnStart.Enabled)
                        Start();

                    e.SuppressKeyPress = true;
                    return;
                }

                if (e.KeyCode == Keys.G)
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
                    if (TheArticle != null)
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
            txtEdit.Text = "{{db|" + Tools.VBInputBox("Enter a reason. Leave blank if you'll edit " +
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
            text = Parsers.Unicodify(text);
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

                string categories;

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

        private void mnuTextBox_Opening(object sender, CancelEventArgs e)
        {
            txtEdit.Focus();

            cutToolStripMenuItem.Enabled = copyToolStripMenuItem.Enabled =
                                           openSelectionInBrowserToolStripMenuItem.Enabled =
                                           (!string.IsNullOrEmpty(txtEdit.SelectedText));

            undoToolStripMenuItem.Enabled = txtEdit.CanUndo;

            openPageInBrowserToolStripMenuItem.Enabled = openHistoryMenuItem.Enabled =
                                                         openTalkPageInBrowserToolStripMenuItem.Enabled =
                                                         (TheArticle != null && !string.IsNullOrEmpty(TheArticle.Name));

            replaceTextWithLastEditToolStripMenuItem.Enabled = (!string.IsNullOrEmpty(LastArticle));
        }

        private void openPageInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.OpenURLInBrowser(Variables.URLIndex + "?title=" + TheArticle.URLEncodedName);
        }

        private void openTalkPageInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.OpenURLInBrowser(Variables.URLIndex + "?title=" + Tools.ConvertToTalk(TheArticle));
        }

        private void openHistoryMenuItem_Click(object sender, EventArgs e)
        {
            Tools.OpenURLInBrowser(Variables.URLIndex + "?title=" + TheArticle.URLEncodedName + "&action=history");
        }

        private void openSelectionInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.OpenURLInBrowser(Variables.URLIndex + "?title=" + txtEdit.SelectedText);
        }

        private void chkGeneralParse_CheckedChanged(object sender, EventArgs e)
        {
            alphaSortInterwikiLinksToolStripMenuItem.Enabled = chkSkipGeneralFixes.Enabled = chkSkipMinorGeneralFixes.Enabled = chkGeneralFixes.Checked;
            
            if (chkSkipGeneralFixes.Checked)
              chkSkipMinorGeneralFixes.Enabled = false;
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
            stopProcessing = true;
            PageReload = false;
            NudgeTimer.Stop();
            UpdateButtons(null, null);
            if (intTimer > 0)
            {//stop and reset the bot timer.
                StopDelayedAutoSaveTimer();
                EnableButtons();
                return;
            }

            StopSaveInterval();
            StopDelayedRestartTimer();
            if (webBrowserEdit.IsBusy)
                webBrowserEdit.Stop2();
            if (Variables.User.webBrowserLogin.IsBusy)
                Variables.User.webBrowserLogin.Stop();

            listMaker.Stop();

            if (AutoSaveEditBoxEnabled)
                EditBoxSaveTimer.Enabled = false;

            StatusLabelText = "Stopped";
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            helpForm.Show();
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
            ErrorHandler.CurrentPage = TheArticle.Name;
            ProcessPage(a, false);
            ErrorHandler.CurrentPage = "";
            UpdateCurrentTypoStats();
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
            if ((TheArticle != null) && string.IsNullOrEmpty(TheArticle.EditSummary))
                ToolTip.SetToolTip(cmboEditSummary, "");
            else
                ToolTip.SetToolTip(cmboEditSummary, MakeSummary());
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons(null, null);
        }

        bool loadingTypos;
        private void chkRegExTypo_CheckedChanged(object sender, EventArgs e)
        {
            if (loadingTypos)
                return;

            if (chkRegExTypo.Checked && BotMode)
            {
                MessageBox.Show("RegExTypoFix cannot be used with bot mode on.\r\nBot mode will now be turned off, and Typos loaded.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                BotMode = false;
            }
            LoadTypos(false);
        }

        private void LoadTypos(bool Reload)
        {
            if (chkRegExTypo.Checked && (RegexTypos == null || Reload))
            {
                loadingTypos = true;
                chkRegExTypo.Checked = false;

                StatusLabelText = "Loading typos";

#if !DEBUG
                string message = @"1. Check each edit before you make it. Although this has been built to be very accurate there will be errors.

2. Optional: Select [[WP:AWB/T|Typo fixing]] as the edit summary. This lets everyone know where to bring issues with the typo correction.";

                if (RegexTypos == null)
                {
                    string s = Variables.RetfPath;

                    if (!s.StartsWith("http:"))
                        s = Variables.URL + "/wiki/" + s;

                    message += "\r\n\r\nThe newest typos will now be downloaded from " + s + " when you press OK.";
                }

                MessageBox.Show(message, "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif

                RegexTypos = new RegExTypoFix();
                RegexTypos.Complete += RegexTypos_Complete;
            }
        }

        private delegate void GenericDelegate();
        private delegate void GenericDelegate1Parm(string parm);

        private void RegexTypos_Complete(BackgroundRequest req)
        {
            if (InvokeRequired)
            {
                Invoke(new BackgroundRequestComplete(RegexTypos_Complete), new object[] {req});
                return;
            }

            chkRegExTypo.Checked = chkSkipIfNoRegexTypo.Enabled = RegexTypos.TyposLoaded;

            if (RegexTypos.TyposLoaded)
            {
                StatusLabelText = RegexTypos.TyposCount + " typos loaded";
                if (!EditBoxTab.TabPages.Contains(tpTypos)) EditBoxTab.TabPages.Add(tpTypos);
                ResetTypoStats();
            }
            else
            {
                RegexTypos = null;
                if (EditBoxTab.TabPages.Contains(tpTypos)) EditBoxTab.TabPages.Remove(tpTypos);
            }

            loadingTypos = false;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenENArticleInBrowser("Wikipedia:AutoWikiBrowser/Typos", false);
        }

        private void webBrowserEdit_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            StopProgressBar();
        }

        private void webBrowserEdit_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            webBrowserEdit.BringToFront();
            StartProgressBar();
        }

        private void dumpHTMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Text = webBrowserEdit.DocumentText;
        }

        private void logOutDebugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Variables.User.WikiStatus = false;
        }

        private void summariesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SummaryEditor se = new SummaryEditor())
            {
                string[] summaries = new string[cmboEditSummary.Items.Count];
                cmboEditSummary.Items.CopyTo(summaries, 0);
                se.Summaries.Lines = summaries;
                se.Summaries.Select(0, 0);

                string prevSummary = cmboEditSummary.SelectedText;

                if (se.ShowDialog(this) == DialogResult.OK)
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
            stopProcessing = false;
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
                    webBrowserEdit.Height = StatusMain.Location.Y - 48;
            }
            else
            {
                webBrowserEdit.Location = new Point(webBrowserEdit.Location.X, 25);
                if (panel1.Visible)
                    webBrowserEdit.Height = panel1.Location.Y - 25;
                else
                    webBrowserEdit.Height = StatusMain.Location.Y - 25;
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
            switch (cmboImages.SelectedIndex)
            {
                case 0:
                    {
                        lblImageWith.Text = "";

                        txtImageReplace.Enabled = txtImageWith.Enabled = chkSkipNoImgChange.Enabled = false;
                        break;
                    }
                case 1:
                    {
                        lblImageWith.Text = "With Image:";

                        txtImageWith.Enabled = txtImageReplace.Enabled = chkSkipNoImgChange.Enabled = true;
                        break;
                    }
                case 2:
                    {
                        lblImageWith.Text = "";

                        txtImageWith.Enabled = false;
                        txtImageReplace.Enabled = true;
                        chkSkipNoImgChange.Enabled = true;
                        break;
                    }
                case 3:
                    {
                        lblImageWith.Text = "Comment:";

                        txtImageWith.Enabled = txtImageReplace.Enabled = chkSkipNoImgChange.Enabled = true;
                        break;
                    }
            }
        }

        private void txtImageReplace_Leave(object sender, EventArgs e)
        {
            txtImageReplace.Text = Regex.Replace(txtImageReplace.Text, "^" 
                + Variables.Namespaces[Namespace.File], "", RegexOptions.IgnoreCase);
        }

        private void txtImageWith_Leave(object sender, EventArgs e)
        {
            txtImageWith.Text = Regex.Replace(txtImageWith.Text, "^" 
                + Variables.Namespaces[Namespace.File], "", RegexOptions.IgnoreCase);
        }

        private void SetProgressBar(object sender, EventArgs e)
        {
            if (listMaker.BusyStatus)
            {
                StartProgressBar();
            }
            else
            {
                StopProgressBar();
            }
        }

        #endregion

        #region ArticleActions
        // TODO: Since this is essentially/conceptually Article.Delete(), Article.Move() etc shouldn't this region be encapsulated?

        private static ArticleActionDialog dlgArticleAction;

        private void MoveArticle()
        {
            dlgArticleAction = new ArticleActionDialog(1);

            try
            {
                dlgArticleAction.NewTitle = TheArticle.Name;
                dlgArticleAction.Summary = LastMove;

                if (dlgArticleAction.ShowDialog(this) == DialogResult.OK)
                {
                    LastMove = dlgArticleAction.Summary;
                    webBrowserEdit.MovePage(TheArticle.Name, dlgArticleAction.NewTitle, ArticleActionSummary);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
            finally
            {
                dlgArticleAction.Dispose();
            }
        }

        private void DeleteArticle()
        {
            dlgArticleAction = new ArticleActionDialog(2);

            try
            {
                dlgArticleAction.Summary = LastDelete;

                if (dlgArticleAction.ShowDialog(this) == DialogResult.OK)
                {
                    LastDelete = dlgArticleAction.Summary;
                    webBrowserEdit.DeletePage(TheArticle.Name, ArticleActionSummary);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
            finally
            {
                dlgArticleAction.Dispose();
            }
        }

        private void ProtectArticle()
        {
            dlgArticleAction = new ArticleActionDialog(3);

            try
            {
                dlgArticleAction.Summary = LastProtect;

                if (dlgArticleAction.ShowDialog(this) == DialogResult.OK)
                {
                    LastProtect = dlgArticleAction.Summary;
                    webBrowserEdit.ProtectPage(TheArticle.Name, ArticleActionSummary, dlgArticleAction.EditProtectionLevel, dlgArticleAction.MoveProtectionLevel, dlgArticleAction.ProtectExpiry, dlgArticleAction.CascadingProtection);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        private string ArticleActionSummary
        {
            get
            {
                if (AddUsingAWBOnArticleAction)
                    return dlgArticleAction.Summary + " (" + Variables.SummaryTag.Trim() + ")";
                return dlgArticleAction.Summary;
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
            if (txtDabLink.Text.Length == 0) txtDabLink.Text = listMaker.SourceText;
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

                txtDabVariants.Text = "";
                foreach (
                    Article a in
                        new WikiFunctions.Lists.LinksOnPageListProvider().MakeList(
                            txtDabLink.Text.Split(new [] {'|'}, StringSplitOptions.RemoveEmptyEntries)))
                {
                    uint i;
                    // exclude years
                    if (uint.TryParse(a.Name, out i) && (i < 2100)) continue;

                    // disambigs typically link to pages in the same namespace only
                    if (new Article(name).NameSpaceKey != a.NameSpaceKey) continue;

                    txtDabVariants.Text += a.Name + "\r\n";
                }
                txtDabVariants.Text = txtDabVariants.Text.Trim();
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
            Visible = true;
            WindowState = LastState;
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visible = false;
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();
            Application.Exit();
        }

        private void mnuNotify_Opening(object sender, CancelEventArgs e)
        {
            SetMenuVisibility(Visible);
        }

        private void SetMenuVisibility(bool visible)
        {
            showToolStripMenuItem.Enabled = !visible || WindowState == FormWindowState.Minimized;
            hideToolStripMenuItem.Enabled = visible;
        }

        public void NotifyBalloon(string message, ToolTipIcon icon)
        {
            ntfyTray.BalloonTipText = message;
            ntfyTray.BalloonTipIcon = icon;
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

                string file = Path.GetDirectoryName(Application.ExecutablePath) + "\\AWBUpdater.exe";

                if (!File.Exists(file))
                {
                    MessageBox.Show("Updater doesn't exist, therefore cannot be run");
                    return;
                }

                Process.Start(file);

                Close();
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

        #region Edit Box Saver
        private void EditBoxSaveTimer_Tick(object sender, EventArgs e)
        {
            if (AutoSaveEditBoxFile.Trim().Length > 0) SaveEditBoxText(AutoSaveEditBoxFile);
        }

        private void SaveEditBoxText(string path)
        {
            Tools.WriteTextFileAbsolutePath(txtEdit.Text, path, false);
        }
        #endregion

        private void saveTextToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveListDialog.ShowDialog() == DialogResult.OK)
                SaveEditBoxText(saveListDialog.FileName);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void LoadUserTalkWarnings()
        {
            Regex userTalkTemplate = new Regex(@"# ?\[\["
                + Variables.NamespacesCaseInsensitive[Namespace.Template] + @"(.*?)\]\]");
            StringBuilder builder = new StringBuilder();

            userTalkTemplatesRegex = null;
            userTalkWarningsLoaded = true; // or it will retry on each page load
            try
            {
                string text;
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
                userTalkWarningsLoaded = false;
            }
            if (builder.Length > 1)
            {
                builder.Remove((builder.Length - 1), 1);
                userTalkTemplatesRegex =
                    new Regex(
                        @"\{\{ ?(" + Variables.NamespacesCaseInsensitive[Namespace.Template] + ")? ?((" + builder +
                        ") ?(\\|.*?)?) ?\\}\\}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            }
        }

        private void undoAllChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Text = TheArticle.OriginalArticleText;
            TheArticle.EditSummary = "";
        }

        private void reloadEditPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PageReload = true;
            webBrowserEdit.LoadEditPage(TheArticle.Name);
            TheArticle.OriginalArticleText = webBrowserEdit.GetArticleText();
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
                    if (webBrowserHistory.Url != new Uri(Variables.URLIndex + "?title=" + TheArticle.URLEncodedName + "&action=history&useskin=myskin") && !string.IsNullOrEmpty(TheArticle.URLEncodedName))
                        webBrowserHistory.Navigate(Variables.URLIndex + "?title=" + TheArticle.URLEncodedName + "&action=history&useskin=myskin");
                }
                else
                    webBrowserHistory.Navigate("about:blank");
            }
            catch
            {
                webBrowserHistory.Navigate("about:blank");

                if (webBrowserHistory.Document != null)
                    webBrowserHistory.Document.Write("<html><body><p>Unable to load history</p></body></html>");
            }
        }

        private void webBrowserHistory_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string histHtml = webBrowserHistory.DocumentText;
            const string startMark = "<!-- start content -->";
            const string endMark = "<!-- end content -->";
            if (histHtml.Contains(startMark) && histHtml.Contains(endMark))
                histHtml = histHtml.Substring(histHtml.IndexOf(startMark),
                                              histHtml.IndexOf(endMark) - histHtml.IndexOf(startMark));
            histHtml = histHtml.Replace("<A ", "<a target=\"blank\" ");
            histHtml = histHtml.Replace("<FORM ", "<form target=\"blank\" ");
            histHtml = histHtml.Replace("<DIV id=histlegend", "<div id=histlegend style=\"display:none;\"");
            histHtml = "<h3>" + TheArticle.Name + "</h3>" + histHtml;

            if (webBrowserHistory.Document != null && webBrowserHistory.Document.Body != null)
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
                webBrowserHistory.Navigate(Variables.URLIndex + "?title=" + TheArticle.URLEncodedName + "&action=history&useskin=myskin");
            }
            catch
            {
                webBrowserHistory.Navigate("about:blank");
                if (webBrowserHistory.Document != null)
                    webBrowserHistory.Document.Write("<html><body><p>Unable to load history</p></body></html>");
            }
        }

        private void mnuHistory_Opening(object sender, CancelEventArgs e)
        {
            openInBrowserToolStripMenuItem.Enabled = refreshHistoryToolStripMenuItem.Enabled = (TheArticle != null);
        }
        #endregion

        private void profilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
            if (chkShutdown.Checked && listMaker.Count == 0)
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
            if (radStandby.Checked)
                return "Standby";
            if (radRestart.Checked)
                return "Restart";
            if (radHibernate.Checked)
                return "Hibernate";

            return "";
        }

        private void ShutdownComputer()
        {
            if (radHibernate.Checked)
                Application.SetSuspendState(PowerState.Hibernate, true, true);
            else if (radRestart.Checked)
                Process.Start("shutdown", "-r");
            else if (radShutdown.Checked)
                Process.Start("shutdown", "-s");
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
            EditToolBarAction("'''Bold text'''", 12, 9, "'''");
        }

        private void imgItalics_Click(object sender, EventArgs e)
        {
            EditToolBarAction("''Italic text''", 13, 11, "''");
        }

        private void imgLink_Click(object sender, EventArgs e)
        {
            EditToolBarAction("[[Link title]]", 12, 10, "[[", "]]");
        }

        private void imgExtlink_Click(object sender, EventArgs e)
        {
            EditToolBarAction("[http://www.example.com link title]", 34, 33, "[", "]");
        }

        private void imgMath_Click(object sender, EventArgs e)
        {
            EditToolBarAction("<math>Insert formula here</math>", 26, 19, "<math>", "</math>");
        }

        private void imgNowiki_Click(object sender, EventArgs e)
        {
            EditToolBarAction("<nowiki>Insert non-formatted text here</nowiki>", 39, 30, "<nowiki>", "</nowiki>");
        }

        private void imgHr_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = txtEdit.SelectedText + "\r\n----\r\n";
        }

        private void imgRedirect_Click(object sender, EventArgs e)
        {
            EditToolBarAction("#REDIRECT [[Insert text]]", 13, 11, "#REDIRECT [[", "]]");
        }

        private void imgStrike_Click(object sender, EventArgs e)
        {
            EditToolBarAction("<s>Strike-through text</s>", 23, 19, "<s>", "</s>");
        }

        private void imgSup_Click(object sender, EventArgs e)
        {
            EditToolBarAction("<sup>Superscript text</sup>", 22, 16, "<sup>", "</sup");
        }

        private void imgSub_Click(object sender, EventArgs e)
        {
            EditToolBarAction("<sub>Subscript text</sub>", 20, 14, "<sub>", "</sub");
        }

        private void imgComment_Click(object sender, EventArgs e)
        {
            EditToolBarAction("<!-- Comment -->", 11, 7, "<!-- ", " -->");
        }

        /// <summary>
        /// Applys EditToolBar button action
        /// </summary>
        /// <param name="noSelection">String to display if no text already select</param>
        /// <param name="selectionStartOffset">Start position to highlight from end of noSelection</param>
        /// <param name="selectionLength">Length of selection of text to replace</param>
        /// <param name="selectionBeforeAfter">String if there is a selection to display before and after selected text</param>
        private void EditToolBarAction(string noSelection, int selectionStartOffset, int selectionLength,
    string selectionBeforeAfter)
        {
            EditToolBarAction(noSelection, selectionStartOffset, selectionLength, selectionBeforeAfter, selectionBeforeAfter);
        }

        /// <summary>
        /// Applys EditToolBar button action
        /// </summary>
        /// <param name="noSelection">String to display if no text already select</param>
        /// <param name="selectionStartOffset">Start position to highlight from end of noSelection</param>
        /// <param name="selectionLength">Length of selection of text to replace</param>
        /// <param name="selectionBefore">String to display before user selected text</param>
        /// <param name="selectionAfter">String to display after user selected text</param>
        private void EditToolBarAction(string noSelection, int selectionStartOffset, int selectionLength,
            string selectionBefore, string selectionAfter)
        {
            if (txtEdit.SelectionLength == 0)
            {
                txtEdit.SelectedText = noSelection;
                txtEdit.SelectionStart = txtEdit.SelectionStart - selectionStartOffset;
                txtEdit.SelectionLength = selectionLength;
            }
            else
            {
                txtEdit.SelectedText = selectionBefore + txtEdit.SelectedText + selectionAfter;
            }
        }

        private void SetEditToolBarEnabled(bool enabled)
        {
            imgBold.Enabled = imgExtlink.Enabled = imgHr.Enabled = imgItalics.Enabled = imgLink.Enabled =
            imgMath.Enabled = imgNowiki.Enabled = imgRedirect.Enabled = imgStrike.Enabled = imgSub.Enabled =
            imgSup.Enabled = imgComment.Enabled = enabled;
        }

        private bool EditToolBarVisible
        {
            set
            {
                if (imgBold.Visible != value)
                {
                    if (value)
                    {
                        txtEdit.Location = new Point(txtEdit.Location.X, txtEdit.Location.Y + 30);
                        txtEdit.Size = new Size(txtEdit.Size.Width, txtEdit.Size.Height - 30);
                    }
                    else
                    {
                        txtEdit.Location = new Point(txtEdit.Location.X, txtEdit.Location.Y - 30);
                        txtEdit.Size = new Size(txtEdit.Size.Width, txtEdit.Size.Height + 30);
                    }

                    imgBold.Visible = imgExtlink.Visible = imgHr.Visible = imgItalics.Visible = imgLink.Visible =
                    imgMath.Visible = imgNowiki.Visible = imgRedirect.Visible = imgStrike.Visible = imgSub.Visible =
                    imgSup.Visible = imgComment.Visible = value;
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
            ToolTip.SetToolTip(txtFind, txtFind.Text);
        }

        private void btnWatch_Click(object sender, EventArgs e)
        {
            //webBrowserEdit.WatchUnwatch();
            //SetWatchButton(btnWatch.Text == "Watch");
        }

        private void SetWatchButton(bool watch)
        {
            btnWatch.Text = watch ? "Unwatch" : "Watch";
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

            int iterations = 1000000/text.Length;
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
                times.Add(new KeyValuePair<int, string>((int) watch.ElapsedMilliseconds, p.Key + " > " + p.Value));
            }

            times.Sort(new Comparison<KeyValuePair<int, string>>(CompareRegexPairs));

            StringBuilder builder = new StringBuilder();

            foreach (KeyValuePair<int, string> p in times) builder.AppendLine(p.ToString());

            Tools.WriteTextFile(builder, "typos.txt", false);

            MessageBox.Show("Results are saved in the file 'typos.txt'", "Profiling complete",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void loadPluginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PluginManager.LoadNewPlugin(this);
        }

        private void managePluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new PluginManager(this).ShowDialog(this);
        }

        private void menuitemMakeFromTextBoxUndo_Click(object sender, EventArgs e)
        {
            listMaker.UserInputTextBox.Undo();
        }

        private void menuitemMakeFromTextBoxCut_Click(object sender, EventArgs e)
        {
            listMaker.UserInputTextBox.Cut();
        }

        private void menuitemMakeFromTextBoxCopy_Click(object sender, EventArgs e)
        {
            listMaker.UserInputTextBox.Copy();
        }

        private void menuitemMakeFromTextBoxPaste_Click(object sender, EventArgs e)
        {
            listMaker.UserInputTextBox.Paste();
        }

        private void mnuCopyToCategoryLog_Click(object sender, EventArgs e)
        {
            loggingSettings1.LoggingCategoryTextBox.Text = (listMaker.UserInputTextBox.SelectionLength > 0) ? listMaker.UserInputTextBox.SelectedText : listMaker.UserInputTextBox.Text;
        }

        private void ListMakerSourceSelectHandler(object sender, EventArgs e)
        {
            toolStripSeparatorMakeFromTextBox.Visible = mnuCopyToCategoryLog.Visible =
                listMaker.cmboSourceSelect.Text.Contains("Category");
        }

        private void externalProcessingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            externalProgram.Show();
        }

        readonly CategoryNameForm CatName = new CategoryNameForm();

        private void categoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CatName.ShowDialog();

            if (string.IsNullOrEmpty(CatName.CategoryName)) return;

            txtEdit.Text += "\r\n\r\n[[" + CatName.CategoryName + "]]";
            reparseEditBox();
        }

        private void UsageStatsMenuItem_Click(object sender, EventArgs e)
        { UsageStats.OpenUsageStatsURL(); }

        private void StartProgressBar()
        {
            if (InvokeRequired)
            {
                Invoke(new GenericDelegate(StartProgressBar));
                return;
            }

            MainFormProgressBar.MarqueeAnimationSpeed = 100;
            MainFormProgressBar.Style = ProgressBarStyle.Marquee;
        }

        private void StopProgressBar()
        {
            if (InvokeRequired)
            {
                Invoke(new GenericDelegate(StopProgressBar));
                return;
            }

            MainFormProgressBar.MarqueeAnimationSpeed = 0;
            MainFormProgressBar.Style = ProgressBarStyle.Continuous;
        }

        private void addAllToWatchlistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dontAddToWatchlistToolStripMenuItem.Checked)
                dontAddToWatchlistToolStripMenuItem.Checked = false;
        }

        private void dontAddToWatchlistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (addAllToWatchlistToolStripMenuItem.Checked)
                addAllToWatchlistToolStripMenuItem.Checked = false;
        }

        private void BotImage_Click(object sender, EventArgs e)
        {
            Tools.OpenURLInBrowser("http://commons.wikimedia.org/wiki/Image:Crystal_Clear_action_run.png");
        }

        private void displayfalsePositivesButtonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddIgnoredToLogFile = displayfalsePositivesButtonToolStripMenuItem.Checked;
        }
    }
        #endregion
}
