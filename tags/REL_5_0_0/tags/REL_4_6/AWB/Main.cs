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
        private readonly Splash SplashScreen = new Splash();
        private readonly WikiFunctions.Profiles.AWBProfilesForm Profiles;

        private bool Abort;
        private bool IgnoreNoBots;

        private string LastArticle = "";
        private string mSettingsFile = "";
        private string LastMove = "", LastDelete = "", LastProtect = "";

        private int OldSelection;
        private int Retries;

        private bool PageReload;
        private int SameArticleNudges;

        private readonly HideText RemoveText = new HideText(false, true, false);
        private readonly List<string> NoParse = new List<string>();
        private readonly FindandReplace FindAndReplace = new FindandReplace();
        private readonly SubstTemplates SubstTemplates = new SubstTemplates();
        private RegExTypoFix RegexTypos;
        private readonly SkipOptions Skip = new SkipOptions();
        private readonly WikiFunctions.ReplaceSpecial.ReplaceSpecial RplcSpecial =
            new WikiFunctions.ReplaceSpecial.ReplaceSpecial();
        private readonly Parsers Parser;
        private readonly TimeSpan StartTime =
            new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        private readonly List<string> RecentList = new List<string>();
        private readonly CustomModule CModule = new CustomModule();
        private readonly ExternalProgram ExtProgram = new ExternalProgram();
        private RegexTester regexTester;
        private bool UserTalkWarningsLoaded;
        private Regex UserTalkTemplatesRegex;
        private bool mErrorGettingLogInStatus;
        private bool skippable = true;
        private FormWindowState LastState = FormWindowState.Normal; // doesn't look like we can use RestoreBounds for this - any other built in way?

        private ArticleRedirected ArticleWasRedirected;

        private ListComparer Comparer;
        private ListSplitter Splitter;

        List<TypoStat> TypoStats;

        private readonly Help HelpForm = new Help();

        private readonly WikiDiff Diff = new WikiDiff();

        /// <summary>
        /// Whether AWB is currently shutting down
        /// </summary>
        private bool ShuttingDown { get; set; }

        #endregion

        #region Constructor and MainForm load/resize
        public MainForm()
        {
            SplashScreen.Show(this);
            RightToLeft = System.Globalization.CultureInfo.CurrentCulture.TextInfo.IsRightToLeft
                ? RightToLeft.Yes : RightToLeft.No;
            InitializeComponent();
            SplashScreen.SetProgress(5);
            try
            {
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

                SplashScreen.SetProgress(10);
                try
                {
                    Parser = new Parsers(Properties.Settings.Default.StubMaxWordCount,
                        Properties.Settings.Default.AddHummanKeyToCats);
                }
                catch (Exception ex)
                {
                    Parser = new Parsers();
                    ErrorHandler.Handle(ex);
                }

                toolStripComboOnLoad.SelectedIndex = 0;
                cmboCategorise.SelectedIndex = 0;
                cmboImages.SelectedIndex = 0;

                Variables.User.UserNameChanged += UpdateUserName;
                Variables.User.BotStatusChanged += UpdateBotStatus;
                Variables.User.AdminStatusChanged += UpdateAdminStatus;
                //Variables.User.WikiStatusChanged += UpdateWikiStatus;

                Variables.User.WebBrowserLogin.DocumentCompleted += WebLoginCompleted;
                Variables.User.WebBrowserLogin.Navigating += WebLoginStarting;

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

                Profiles = new WikiFunctions.Profiles.AWBProfilesForm(webBrowserEdit);
                Profiles.LoadProfile += LoadProfileSettings;

                SplashScreen.SetProgress(15);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        public void ParseCommandLine(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "/s":
                        if ((i + 1) < args.Length)
                        {
                            string fileName = args[i + 1];

                            if (string.IsNullOrEmpty(Path.GetExtension(fileName)) && !File.Exists(fileName)) 
                                fileName += ".xml";

                            if (File.Exists(fileName))
                                SettingsFile = fileName;
                        }
                        break;
                    case "/u":
                        if ((i + 1) < args.Length)
                            ProfileToLoad = args[i + 1];
                        break;
                }
            }
        }

        private string SettingsFileDisplay;
        string SettingsFile
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

        string ProfileToLoad = "";

        private void MainForm_Load(object sender, EventArgs e)
        {
            EditBoxTab.TabPages.Remove(tpTypos);

            StatusLabelText = "Initialising...";
            SplashScreen.SetProgress(20);
            Variables.MainForm = this;
            lblOnlyBots.BringToFront();
            Updater.UpdateAWB(SplashScreen.SetProgress); // progress 22-29 in UpdateAWB()
            SplashScreen.SetProgress(30);

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

                SplashScreen.SetProgress(35);
                if (Properties.Settings.Default.LogInOnStart)
                    CheckStatus(false);

                logControl.Initialise(listMaker);

                Location = Properties.Settings.Default.WindowLocation;

                Size = Properties.Settings.Default.WindowSize;

                WindowState = Properties.Settings.Default.WindowState;

                Debug();
                Release();

                Plugin.LoadPluginsStartup(this, SplashScreen); // progress 65-79 in LoadPlugins()

                LoadPrefs(); // progress 80-85 in LoadPrefs()
                CreateEditor();

                SplashScreen.SetProgress(86);
                UpdateButtons(null, null);
                SplashScreen.SetProgress(88);
                LoadRecentSettingsList(); // progress 89-94 in LoadRecentSettingsList()
                SplashScreen.SetProgress(95);

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

                Profiles.Login(ProfileToLoad);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }

            UsageStats.Initialise();

            StatusLabelText = "";
            SplashScreen.SetProgress(100);
            SplashScreen.Close();

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

        private bool LowThrdPriority;
        private bool LowThreadPriority
        {
            get { return LowThrdPriority; }
            set
            {
                LowThrdPriority = value;
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

        private decimal ASEditPeriod = 60;
        private decimal AutoSaveEditBoxPeriod
        {
            get { return ASEditPeriod; }
            set { ASEditPeriod = value; EditBoxSaveTimer.Interval = int.Parse((value * 1000).ToString()); }
        }

        private void SetStatusLabelText(string text)
        {
            StatusLabelText = text;
        }

        private string StatusLabelText
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

        private bool AddIgnoredToLogFile
        {
            set
            {
                btnStop.Location = (value) ? new Point(220, 62) : new Point(156, 62);
                btnStop.Size = (value) ? new Size(51, 23) : new Size(117, 23);

                btnFalsePositive.Visible = btntsFalsePositive.Visible = value;
            }
        }

        bool TimerShown = true;
        private bool ShowMovingAverageTimer
        {
            set
            {
                TimerShown = value;
                ShowTimer();
            }
            get { return TimerShown; }
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

        private AsyncApiEdit APIEdit;

        private void CreateEditor()
        {
            APIEdit = new AsyncApiEdit(Variables.URLLong, this, Variables.PHP5);
            APIEdit.PreviewComplete += PreviewComplete;

            APIEdit.ExceptionCaught += APIEditExceptionCaught;
        }

        private void APIEditExceptionCaught(AsyncApiEdit sender, Exception ex)
        {
            StartDelayedRestartTimer(null, null);
        }

        private void StartAPITextLoad(string title)
        {
            APIEdit.Open(title);

            APIEdit.Wait();

            // if MAXLAG exceeded then API will report error, so AWB should go into restart timer to try again in a few seconds
            if (APIEdit.State == AsyncApiEdit.EditState.Failed)
            {
                StartDelayedRestartTimer(null, null);
                return;
            }

            if (!LoadSuccessAPI())
                return;

            CaseWasLoad(APIEdit.Page.Text);
        }

        private bool StopProcessing;

        private void Start()
        {
            if (StopProcessing)
                return;

            try
            {
                Tools.WriteDebug(Name, "Starting");

                Shutdown();

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
                NewWhatLinksHere();

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

        // counts number of redirects so that we catch double redirects
        private int Redirects;

        private void CaseWasLoad(string articleText)
        {
            if (StopProcessing)
                return;

            if (!preParseModeToolStripMenuItem.Checked && !CheckLoginStatus()) return;

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

            if (articleIsRedirect)
                Redirects++;
            else
                Redirects = 0;

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

                    // don't allow redirects to a redirect as we could go round in circles
                    if (Redirects > 1)
                    {
                        SkipPage("Double redirect");
                        return;
                    }

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

                    if (chkSkipMinorGeneralFixes.Checked && chkGeneralFixes.Checked && TheArticle.OnlyMinorGeneralFixesChanged)
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

            // check for {{sic}} tags etc. when doing typo fixes and not in pre-parse mode
            if (chkRegExTypo.Checked && !preParseModeToolStripMenuItem.Checked && TheArticle.HasSicTag)
                MessageBox.Show(@"This page contains a 'sic' tag or template, please take extra care when correcting typos.", "'sic' tag in page", MessageBoxButtons.OK, MessageBoxIcon.Warning);

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

            if (syntaxHighlightEditBoxToolStripMenuItem.Checked)
                txtEdit.Visible = false;
            
            webBrowserEdit.SetArticleText(TheArticle.ArticleText);
            txtEdit.Text = TheArticle.ArticleText;

            //Update statistics and alerts
            if (!BotMode)
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

                Variables.Profiler.Profile("Make Edit summary");

                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Detect_multiple_DEFAULTSORT
                if (WikiRegexes.Defaultsort.Matches(txtEdit.Text).Count > 1)
                    lblWarn.Text += "Multiple DEFAULTSORTs found\r\n";

                int bracketLength = 0;
                int unbalancedBracket = TheArticle.UnbalancedBrackets(ref bracketLength);
                if(unbalancedBracket > 0)
                    lblWarn.Text += "Unbalanced brackets found\r\n";

                Variables.Profiler.Profile("Unbalanced brackets");

                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Some_additional_edits
                if (TheArticle.HasDeadLinks)
                    lblWarn.Text += "Dead links found\r\n";

                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Working_with_Alerts
                if (chkSkipIfNoAlerts.Checked && lblWarn.Text.Length == 0)
                {
                    SkipPage("Page has no alerts");
                    return;
                }
                
                // syntax highlighting of edit box based on m:extension:wikEd standards
                if (syntaxHighlightEditBoxToolStripMenuItem.Checked)
                {                    
                    txtEdit.Visible = false;

                    Variables.Profiler.Profile("Alerts");
                    txtEdit = HighlightSyntax(txtEdit);
                    Variables.Profiler.Profile("Syntax highlighting");

                    if (!focusAtEndOfEditTextBoxToolStripMenuItem.Checked)
                    {
                        txtEdit.SetEditBoxSelection(0, 0);
                        txtEdit.Select(0, 0);
                        txtEdit.ScrollToCaret();
                    }

                    txtEdit.Visible = true;
                }

                if (focusAtEndOfEditTextBoxToolStripMenuItem.Checked)
                {
                    txtEdit.Select(txtEdit.Text.Length, 0);
                    txtEdit.ScrollToCaret();
                }
                else
                {
                    if (unbalancedBracket < 0)
                        btnSave.Focus();
                    else if (scrollToUnbalancedBracketsToolStripMenuItem.Checked)
                    {
                        EditBoxTab.SelectedTab = tpEdit;

                        // indexes in articleText and txtEdit.Edit are offset by the number of newlines before the index of the unbalanced brackets
                        // so allow for this when highlighting the unbalanced bracket
                        string a = txtEdit.Text.Substring(0, unbalancedBracket);
                        int b = Regex.Matches(a, "\n").Count;
                        txtEdit.SetEditBoxSelection(unbalancedBracket - b, bracketLength);
                        txtEdit.SelectionBackColor = Color.Red;
                    }
                }
            }
            else
            {
                EnableButtons();
                Abort = false;
            }
        }

        /// <summary>
        /// Applies syntax highlighting to the input ArticleTextBox 
        /// </summary>
        /// <param name="txtEditLocal"></param>
        /// <returns></returns>
        private ArticleTextBox HighlightSyntax(ArticleTextBox txtEditLocal)
        {
            // temporarily disable TextChanged firing to help performance of this function
            txtEditLocal.TextChanged -= txtEdit_TextChanged;

            // TODO: regexes to be moved to WikiRegexes where appropriate and covered by unit tests
            Font currentFont = txtEditLocal.SelectionFont;
            Font boldFont = new Font(currentFont.FontFamily, currentFont.Size, FontStyle.Bold);
            Font italicFont = new Font(currentFont.FontFamily, currentFont.Size, FontStyle.Italic);
            Font boldItalicFont = new Font(currentFont.FontFamily, currentFont.Size, FontStyle.Bold | FontStyle.Italic);

            // headings text in bold
            foreach (Match m in WikiRegexes.Heading.Matches(txtEditLocal.RawText))
            {
                txtEditLocal.SetEditBoxSelection(m.Groups[2].Index, m.Groups[2].Length);
                txtEditLocal.SelectionFont = boldFont;
            }

            // templates grey background
            foreach (Match m in WikiRegexes.NestedTemplates.Matches(txtEditLocal.RawText))
            {
                txtEditLocal.SetEditBoxSelection(m.Index, m.Length);
                txtEditLocal.SelectionBackColor = Color.LightGray;
            }

            // * items grey background
            Regex StarRows = new Regex(@"^ *(\*)(.*)", RegexOptions.Multiline);
            foreach (Match m in StarRows.Matches(txtEditLocal.RawText))
            {
                txtEditLocal.SetEditBoxSelection(m.Index, m.Length);
                txtEditLocal.SelectionBackColor = Color.LightGray;

                txtEditLocal.SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                txtEditLocal.SelectionFont = boldFont;
            }

            // template names dark blue font
            foreach (Match m in WikiRegexes.TemplateName.Matches(txtEditLocal.RawText))
            {
                txtEditLocal.SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                txtEditLocal.SelectionColor = Color.DarkBlue;
            }

            // refs grey background
            foreach (Match m in WikiRegexes.Refs.Matches(txtEditLocal.RawText))
            {
                txtEditLocal.SetEditBoxSelection(m.Index, m.Length);
                txtEditLocal.SelectionBackColor = Color.LightGray;
            }

            // external links grey background, blue bold
            foreach (Match m in WikiRegexes.ExternalLinks.Matches(txtEditLocal.RawText))
            {
                txtEditLocal.SetEditBoxSelection(m.Index, m.Length);
                txtEditLocal.SelectionColor = Color.Blue;
                txtEditLocal.SelectionFont = boldFont;
            }

            // images green background
            //foreach (Match m in WikiRegexes.Images.Matches(txtEdit.RawText))
            //{
            //    txtEdit.SetEditBoxSelection(m.Index, m.Length);
            //    txtEdit.SelectionBackColor = Color.Green;

            //}

            // italics
            Regex Italics = new Regex(@"''(.+?)''");
            foreach (Match m in Italics.Matches(txtEditLocal.RawText))
            {
                txtEditLocal.SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                txtEditLocal.SelectionFont = italicFont;
            }

            // bold  
            Regex Bold = new Regex(@"'''(.+?)'''");
            foreach (Match m in Bold.Matches(txtEditLocal.RawText))
            {
                // reset anything incorrectly done by italics  earlier
                txtEditLocal.SetEditBoxSelection(m.Index, m.Length);
                txtEditLocal.SelectionFont = currentFont;

                txtEditLocal.SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                txtEditLocal.SelectionFont = boldFont;
            }

            // bold italics 
            Regex BoldItalics = new Regex(@"'''''(.+?)'''''");
            foreach (Match m in BoldItalics.Matches(txtEditLocal.RawText))
            {
                // reset anything incorrectly done by italics/bold earlier
                txtEditLocal.SetEditBoxSelection(m.Index, m.Length);
                txtEditLocal.SelectionFont = currentFont;

                txtEditLocal.SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                txtEditLocal.SelectionFont = boldItalicFont;
            }

            // piped wikilink text in blue, piped part in bold
            foreach (Match m in WikiRegexes.PipedWikiLink.Matches(txtEditLocal.RawText))
            {
                txtEditLocal.SetEditBoxSelection(m.Groups[2].Index, m.Groups[2].Length);
                txtEditLocal.SelectionColor = Color.Blue;
                txtEditLocal.SelectionFont = boldFont;

                txtEditLocal.SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                txtEditLocal.SelectionColor = Color.Blue;
            }

            // unpiped wikilinks in blue and bold
            foreach (Match m in WikiRegexes.UnPipedWikiLink.Matches(txtEditLocal.RawText))
            {
                txtEditLocal.SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                txtEditLocal.SelectionColor = Color.Blue;
                txtEditLocal.SelectionFont = boldFont;
            }

            // pipe trick: in blue bold too
            foreach (Match m in WikiRegexes.WikiLinksOnlyPlusWord.Matches(txtEditLocal.RawText))
            {
                txtEditLocal.SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                txtEditLocal.SelectionColor = Color.Blue;
                txtEditLocal.SelectionFont = boldFont;
            }

            // cats grey background
            foreach (Match m in WikiRegexes.Category.Matches(txtEditLocal.RawText))
            {
                txtEditLocal.SetEditBoxSelection(m.Index, m.Length);
                txtEditLocal.SelectionBackColor = Color.LightGray;
                txtEditLocal.SelectionFont = currentFont;
                txtEditLocal.SelectionColor = Color.Black;

                txtEditLocal.SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                txtEditLocal.SelectionColor = Color.Blue;
            }

            // interwikis dark grey background
            Regex Interwiki = new Regex(@"\[\[([a-z-]{2,6}\:)([^\[\]\r\n]+)\]\]");
            foreach (Match m in Interwiki.Matches(txtEditLocal.RawText))
            {
                txtEditLocal.SetEditBoxSelection(m.Index, m.Length);
                txtEditLocal.SelectionBackColor = Color.Gray;
                txtEditLocal.SelectionFont = currentFont;

                txtEditLocal.SetEditBoxSelection(m.Groups[2].Index, m.Groups[2].Length);
                txtEditLocal.SelectionColor = Color.Blue;

                txtEditLocal.SetEditBoxSelection(m.Groups[1].Index, m.Groups[1].Length);
                txtEditLocal.SelectionColor = Color.Black;
            }

            // comments dark orange background
            foreach (Match m in WikiRegexes.Comments.Matches(txtEditLocal.RawText))
            {
                txtEditLocal.SetEditBoxSelection(m.Index, m.Length);
                txtEditLocal.SelectionBackColor = Color.PaleGoldenrod;
            }

            txtEditLocal.TextChanged += txtEdit_TextChanged;

            return txtEditLocal;
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

                string html = null;
                if (webBrowserEdit.Document != null && webBrowserEdit.Document.Body != null)
                    html = webBrowserEdit.Document.Body.InnerHtml;

                if (string.IsNullOrEmpty(html) || IsReadOnlyDB(html))
                {
                    if (Retries < 10)
                    {
                        StartDelayedRestartTimer(null, null);
                        Retries++;
                        Start();
                        return false;
                    }

                    Retries = 0;
                    if (!string.IsNullOrEmpty(html))
                        SkipPage("Database is locked, tried 10 times");
                    else
                    {
                        MessageBox.Show("Loading edit page failed after 10 retries. Processing stopped.", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Stop();
                    }
                    return false;
                }

                if (html.Contains("Sorry! We could not process your edit due to a loss of session data. Please try again. If it still doesn't work, try logging out and logging back in."))
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
                if (!APIEdit.Page.Exists && radSkipNonExistent.Checked)
                {//check if it is a non-existent page, if so then skip it automatically.
                    SkipPage("Non-existent page");
                    return false;
                }
                if (APIEdit.Page.Exists && radSkipExistent.Checked)
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

        private static readonly Regex SpamUrlRegex = new Regex("<p>The following link has triggered our spam protection filter:<strong>(.*?)</strong><br/?>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private void CaseWasSaved(object sender, EventArgs e)
        {
            try
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
                        Match m = SpamUrlRegex.Match(webBrowserEdit.DocumentText);

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
            }
            catch (Exception/* ext*/)
            {
                /* TODO find source of error
                 * what we get here is a: 
                 * 
                 * System.IO.FileNotFoundException
                 *    at System.Windows.Forms.UnsafeNativeMethods.IPersistStreamInit.Save(IStream pstm, Boolean fClearDirty)
   at System.Windows.Forms.WebBrowser.get_DocumentStream()
   at System.Windows.Forms.WebBrowser.get_DocumentText()
   at AutoWikiBrowser.MainForm.CaseWasSaved(Object sender, EventArgs e)
                 * 
                 * which can be triggered when we look at webBrowserEdit.DocumentText
                 * for some unknown reason
                 * could be to do with internet connection speed
                 */
                Start();
            }

            //lower restart delay
            if (IntRestartDelay > 5)
                IntRestartDelay -= 1;

            NumberOfEdits++;

            LastArticle = "";
            listMaker.Remove(TheArticle);
            NudgeTimer.Stop();
            SameArticleNudges = 0;
            if (EditBoxTab.SelectedTab == tpHistory)
                EditBoxTab.SelectedTab = tpEdit;
            logControl.AddLog(false, TheArticle.LogListener);
            UpdateOverallTypoStats();

            if (listMaker.Count == 0 && AutoSaveEditBoxEnabled)
                EditBoxSaveTimer.Enabled = false;
            Retries = 0;

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
                SameArticleNudges = 0;
                logControl.AddLog(true, TheArticle.LogListener);
                Retries = 0;
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
        private void ProcessPage(Article theArticle, bool mainProcess)
        {
            bool process = true;
            TypoStats = null;

#if DEBUG
            Variables.Profiler.Start("ProcessPage(\"" + theArticle.Name + "\")");
#endif

            try
            {
                if (NoParse.Contains(theArticle.Name))
                    process = false;

                if (!IgnoreNoBots &&
                    !Parsers.CheckNoBots(theArticle.ArticleText, Variables.User.Name))
                {
                    theArticle.AWBSkip("Restricted by {{bots}}/{{nobots}}");
                    return;
                }

                Variables.Profiler.Profile("Initial skip checks");

                if (CModule.ModuleEnabled && CModule.Module != null)
                {
                    theArticle.SendPageToCustomModule(CModule.Module);
                    if (theArticle.SkipArticle) return;
                }

                Variables.Profiler.Profile("Custom module");

                if (ExtProgram.ModuleEnabled)
                {
                    theArticle.SendPageToCustomModule(ExtProgram);
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

                    theArticle.Unicodify(Skip.SkipNoUnicode, Parser);
                    Variables.Profiler.Profile("Unicodify");

                    theArticle.UnHideMoreText(RemoveText);
                    Variables.Profiler.Profile("UnHideMoreText");
                }

                // find and replace before general fixes
                if (chkFindandReplace.Checked && !FindAndReplace.AfterOtherFixes)
                {
                    theArticle.PerformFindAndReplace(FindAndReplace, SubstTemplates, RplcSpecial,
                        chkSkipWhenNoFAR.Checked);

                    Variables.Profiler.Profile("F&R");

                    if (theArticle.SkipArticle) return;
                }

                // RegexTypoFix
                if (chkRegExTypo.Checked && RegexTypos != null && !BotMode && !Namespace.IsTalk(theArticle.NameSpaceKey))
                {
                    theArticle.PerformTypoFixes(RegexTypos, chkSkipIfNoRegexTypo.Checked);
                    Variables.Profiler.Profile("Typos");
                    TypoStats = RegexTypos.GetStatistics();
                    if (theArticle.SkipArticle)
                    {
                        if (mainProcess)
                        {   // update stats only if not called from e.g. 'Re-parse' than could be clicked repeatedly
                            OverallTypoStats.UpdateStats(TypoStats, true);
                            UpdateTypoCount();
                        }
                        return;
                    }
                }

                // replace/add/remove categories
                if (cmboCategorise.SelectedIndex != 0)
                {
                    theArticle.Categorisation((WikiFunctions.Options.CategorisationOptions)
                        cmboCategorise.SelectedIndex, Parser, chkSkipNoCatChange.Checked, txtNewCategory.Text.Trim(),
                        txtNewCategory2.Text.Trim(), chkRemoveSortKey.Checked);
                    if (theArticle.SkipArticle) return;
                    else if (!chkGeneralFixes.Checked) theArticle.AWBChangeArticleText("Fix categories", Parsers.FixCategories(theArticle.ArticleText, Namespace.IsMainSpace(theArticle.Name)), true);
                }

                Variables.Profiler.Profile("Categories");

                if (theArticle.CanDoGeneralFixes)
                {
                    // auto tag
                    if (process && chkAutoTagger.Checked)
                    {
                        theArticle.AutoTag(Parser, Skip.SkipNoTag);
                        if (theArticle.SkipArticle) return;
                    }

                    Variables.Profiler.Profile("Auto-tagger");

                    if (process && chkGeneralFixes.Checked)
                    {
                        theArticle.PerformGeneralFixes(Parser, RemoveText, Skip, replaceReferenceTagsToolStripMenuItem.Checked, restrictDefaultsortChangesToolStripMenuItem.Checked, noMOSComplianceFixesToolStripMenuItem.Checked);
                    }
                }
                else if (process && chkGeneralFixes.Checked && theArticle.NameSpaceKey == 3)
                {
                    if (!UserTalkWarningsLoaded)
                    {
                        LoadUserTalkWarnings();
                        Variables.Profiler.Profile("loadUserTalkWarnings");
                    }

                    theArticle.PerformUserTalkGeneralFixes(RemoveText, UserTalkTemplatesRegex, Skip.SkipNoUserTalkTemplatesSubstd);
                }

                // find and replace after general fixes
                if (chkFindandReplace.Checked && FindAndReplace.AfterOtherFixes)
                {
                    theArticle.PerformFindAndReplace(FindAndReplace, SubstTemplates, RplcSpecial,
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
                        txtImageReplace.Text, txtImageWith.Text, chkSkipNoImgChange.Checked);
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
                Stop();
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
                    // when less than 10 edits show user help info on double click to undo etc.
                    webBrowserDiff.Document.Write("<!DOCTYPE HTML PUBLIC \" -//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">"
                                                  + "<html><head>" +
                                                  WikiDiff.DiffHead() + @"</head><body>" + ((NumberOfEdits < 10) ? WikiDiff.TableHeader : WikiDiff.TableHeaderNoMessages) +
                                                  Diff.GetDiff(TheArticle.OriginalArticleText, txtEdit.Text, 2) +
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
            if (!APIEdit.IsActive) APIEdit.Preview(TheArticle.Name, txtEdit.Text);
        }

        private void PreviewComplete(AsyncApiEdit sender, string result)
        {
            LastArticle = txtEdit.Text;

            skippable = false;

            if (webBrowserDiff.Document != null)
            {
                webBrowserDiff.Document.OpenNew(false);
                webBrowserDiff.Document.Write("<html><head>"
                    + sender.HtmlHeaders
                    + "</head><body style=\"background:white; margin:10px; text-align:left;\">"
                    + result
                    + "</html>"
                    );
                webBrowserDiff.BringToFront();
            }

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public void UndoChange(int left, int right)
        {
            try
            {
                txtEdit.Text = Diff.UndoChange(left, right);
                TheArticle.EditSummary = "";
                GetDiff();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public void UndoDeletion(int left, int right)
        {
            try
            {
                txtEdit.Text = Diff.UndoDeletion(left, right);
                TheArticle.EditSummary = "";
                GetDiff();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="right"></param>
        public void UndoAddition(int right)
        {
            try
            {
                txtEdit.Text = Diff.UndoAddition(right);
                TheArticle.EditSummary = "";
                GetDiff();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destLine"></param>
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

        private Point OldPosition;
        private Size OldSize;
        private void ParametersShowHide()
        {
            enlargeEditAreaToolStripMenuItem.Checked = !enlargeEditAreaToolStripMenuItem.Checked;
            if (groupBox2.Visible)
            {
                btntsShowHideParameters.Image = Resources.Showhideparameters2;

                OldPosition = EditBoxTab.Location;
                EditBoxTab.Location = new Point(groupBox2.Location.X, groupBox2.Location.Y - 5);

                OldSize = EditBoxTab.Size;
                EditBoxTab.Size = new Size((EditBoxTab.Size.Width + MainTab.Size.Width + groupBox2.Size.Width + 8), EditBoxTab.Size.Height);
            }
            else
            {
                btntsShowHideParameters.Image = Resources.Showhideparameters;

                EditBoxTab.Location = OldPosition;
                EditBoxTab.Size = OldSize;
            }
            groupBox2.Visible = MainTab.Visible = !groupBox2.Visible;
        }

        private void UpdateUserName(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Variables.User.Name))
            {
                lblUserName.BackColor = Color.Red;
                lblUserName.Text = Variables.Namespaces[Namespace.User];
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
            CurrentTypoStats.UpdateStats(TypoStats, false);
        }

        private void UpdateOverallTypoStats()
        {
            if (chkRegExTypo.Checked)
                OverallTypoStats.UpdateStats(TypoStats, false);
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
                ShuttingDown = true;
                // save user persistent settings
                Properties.Settings.Default.Save();

                try
                {
                    if (webBrowserEdit.IsBusy)
                        webBrowserEdit.Stop2();
                    if (Variables.User.WebBrowserLogin.IsBusy)
                        Variables.User.WebBrowserLogin.Stop();
                }
                catch
                { } // simply suppress IE silliness

                SaveRecentSettingsList();
                UsageStats.Do(true);
            }
            else
            {
                e.Cancel = true;
                return;
            }
            ntfyTray.Dispose();
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

            // check to see if we have only edited one level 2 section
            if (!noSectionEditSummaryToolStripMenuItem.Checked)
            {
                string sectionEditText = SectionEditSummary(TheArticle.OriginalArticleText, txtEdit.Text);

                if (!sectionEditText.Equals(""))
                    tag = @"/* " + sectionEditText + @" */" + tag;
            }

            if ((Variables.User.IsBot && chkSuppressTag.Checked)
                || (!Variables.IsWikimediaProject && SuppressUsingAWB))
                return tag;

            int maxSummaryLength = (200 - (Variables.SummaryTag.Length + 1));

            if (tag.Length >= maxSummaryLength)
            {
                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#Edit_summary_issue
                // replace last wikilink with dots as an attempt to prevent broken wikilinks in edit summary
                if (tag.EndsWith(@"]]"))
                    tag = Regex.Replace(tag, @"\s*\[\[.+?\]\]$", "...");

                if (tag.Length >= maxSummaryLength)
                    tag = tag.Substring(0, maxSummaryLength);
            }

            return tag + Variables.SummaryTag;
        }

        private string SectionEditSummary(string originalArticleTextLocal, string articleTextLocal)
        {
            // TODO: could add recursion to look for edits to only a level 3 section within a level 2 etc.

            // if edit only affects one level 2 heading, add /* heading  title */ to make a section edit
            if (!WikiRegexes.HeadingLevelTwo.IsMatch(TheArticle.OriginalArticleText))
                return ("");

            string[] levelTwoHeadingsBefore = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            string[] levelTwoHeadingsAfter = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

            int before = 0, after = 0;

            string zerothSectionBefore = WikiRegexes.ArticleToFirstLevelTwoHeading.Match(originalArticleTextLocal).Value;
            if (!string.IsNullOrEmpty(zerothSectionBefore))
                originalArticleTextLocal = originalArticleTextLocal.Replace(zerothSectionBefore, "");

            string zerothSectionAfter = WikiRegexes.ArticleToFirstLevelTwoHeading.Match(articleTextLocal).Value;
            if (!string.IsNullOrEmpty(zerothSectionAfter))
                articleTextLocal = articleTextLocal.Replace(zerothSectionAfter, "");

            // can't provide a section edit summary if there are changes in text before first level 2 heading
            if (!string.IsNullOrEmpty(zerothSectionBefore) && !zerothSectionBefore.Equals(zerothSectionAfter))
                return ("");

            // get sections for article text before any AWB changes
            foreach (Match m in WikiRegexes.SectionLevelTwo.Matches(originalArticleTextLocal))
            {
                levelTwoHeadingsBefore[before] = null;
                levelTwoHeadingsBefore[before] = m.Value;
                originalArticleTextLocal = originalArticleTextLocal.Replace(m.Value, "");
                before++;

                if (before == 20)
                    return ("");
            }
            // add the last section to the array
            levelTwoHeadingsBefore[before] = originalArticleTextLocal;

            // get sections for article text after AWB changes
            foreach (Match m in WikiRegexes.SectionLevelTwo.Matches(articleTextLocal))
            {
                levelTwoHeadingsAfter[after] = m.Value;
                articleTextLocal = articleTextLocal.Replace(m.Value, "");
                after++;
            }
            // add the last section to the array
            levelTwoHeadingsAfter[after] = articleTextLocal;

            // if number of sections has changed, can't provide section edit summary
            if (!(after == before))
                return ("");

            int sectionsChanged = 0, sectionChangeNumber = 0;

            for (int i = 0; i <= after; i++)
            {
                if (!(levelTwoHeadingsBefore[i] == levelTwoHeadingsAfter[i]))
                {
                    sectionsChanged++;
                    sectionChangeNumber = i;
                }

                // if multiple level 2 sections changed, can't provide section edit summary
                if (sectionsChanged == 2)
                    return ("");
            }

            // so SectionsChanged == 1, get heading name from LevelTwoHeadingsBefore
            return WikiRegexes.HeadingLevelTwo.Match(levelTwoHeadingsBefore[sectionChangeNumber]).Groups[1].Value.Trim();
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
                chkRemoveSortKey.Enabled = true;
            }
            else
            {
                label1.Text = "";
                txtNewCategory2.Enabled = false;
                chkRemoveSortKey.Enabled = false;
            }
        }

        private void WebLoginCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            StopProgressBar();
        }

        private void WebLoginStarting(object sender, WebBrowserNavigatingEventArgs e)
        {
            StartProgressBar();
        }

        private void UpdateBotStatus(object sender, EventArgs e)
        {
            chkAutoMode.Enabled = chkSuppressTag.Enabled = Variables.User.IsBot;

            lblOnlyBots.Visible = !Variables.User.IsBot;

            if (Variables.User.IsBot)
            {
                if (!MainTab.TabPages.Contains(tpBots))
                    MainTab.TabPages.Insert(MainTab.TabPages.IndexOf(tpStart), tpBots);
            }
            else
            {
                BotMode = false;
                if (MainTab.TabPages.Contains(tpBots))
                    MainTab.Controls.Remove(tpBots);
            }
        }

        private void UpdateAdminStatus(object sender, EventArgs e)
        {
            btnProtect.Enabled = btnMove.Enabled = btnDelete.Enabled = btntsDelete.Enabled = (Variables.User.IsAdmin && btnSave.Enabled && (TheArticle != null));
        }

        //private void UpdateWikiStatus(object sender, EventArgs e) { }

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

        public bool CheckStatus(bool login)
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
                    if (!login)
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
                            if (!NoParse.Contains(link.Groups[1].Value))
                                NoParse.Add(link.Groups[1].Value);
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

                switch (
                    MessageBox.Show(
                        "This version is not enabled, please download the newest version. If you have the newest version, check that Wikipedia is online.\r\n\r\nPlease press \"Yes\" to run the AutoUpdater, \"No\" to load the download page and update manually, or \"Cancel\" to not update (but you will not be able to edit).",
                        "Problem", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information))
                {
                    case DialogResult.Yes:
                        RunUpdater();
                        break;
                    case DialogResult.No:
                        Tools.OpenURLInBrowser("http://sourceforge.net/project/showfiles.php?group_id=158332");
                        break;
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

        private void CategoryLeave(object sender, EventArgs e)
        {
            TextBox cat = sender as TextBox;

            if (cat != null)
            {
                string text = cat.Text.Trim('[', ']');

                text = Regex.Replace(text, "^" +
                                                   Variables.NamespacesCaseInsensitive[Namespace.Category], "");
                cat.Text = Tools.TurnFirstToUpper(text);
            }
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

                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Replace_nofootnotes_with_morefootnote_if_references_exists
                if (TheArticle.HasMorefootnotesAndManyReferences)
                    lblWarn.Text += @"Has 'No/More footnotes' template yet many references" + "\r\n";

                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.28Yet.29_more_reference_related_changes.
                if (TheArticle.HasRefAfterReflist)
                    lblWarn.Text += @"Has a <ref> after <references/>" + "\r\n";

                if (articleText.StartsWith("=="))
                    lblWarn.Text += "Starts with heading.";

                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Format_references
                if(TheArticle.HasBareReferences)
                    lblWarn.Text += @"Unformatted references found" + "\r\n";

                lblWords.Text = "Words: " + intWords;
                lblCats.Text = "Categories: " + intCats;
                lblImages.Text = "Images: " + intImages;
                lblLinks.Text = "Links: " + intLinks;
                lblInterLinks.Text = "Interwiki links: " + intInterLinks;

                //Find multiple links                
                ArrayList arrayLinks = new ArrayList();
                ArrayList arrayLinks2 = new ArrayList();

                //get all the links
                foreach (Match m in WikiRegexes.WikiLink.Matches(articleText))
                {
                    string x = m.Groups[1].Value;
                    if (!WikiRegexes.Dates.IsMatch(x) && !WikiRegexes.Dates2.IsMatch(x))
                        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Multiple_links
                        // make first character uppercase so that [[proton]] and [[Proton]] are marked as duplicate
                        x = Tools.TurnFirstToUpper(x);
                        arrayLinks.Add(x);
                }

                lbDuplicateWikilinks.Sorted = true;

                //add the duplicate articles to the listbox
                foreach (string z in arrayLinks)
                {
                    if ((arrayLinks.IndexOf(z) < arrayLinks.LastIndexOf(z)) && (!arrayLinks2.Contains(z)))
                    {
                        arrayLinks2.Add(z);
                        // include count of links in form Proton (3)
                        lbDuplicateWikilinks.Items.Add(z + @" (" + (Regex.Matches(articleText, @"\[\[" + Regex.Escape(z) + @"(\|.*?)?\]\]").Count + Regex.Matches(articleText, @"\[\[" + Regex.Escape(Tools.TurnFirstToLower(z)) + @"(\|.*?)?\]\]").Count) + @")");
                    }
                }
            }
            lblDuplicateWikilinks.Visible = lbDuplicateWikilinks.Visible = btnRemove.Visible = (lbDuplicateWikilinks.Items.Count > 0);
        }

        private void lbDuplicateWikilinks_Click(object sender, EventArgs e)
        {
            EditBoxTab.SelectedTab = tpEdit;
            int selection = lbDuplicateWikilinks.SelectedIndex;
            if (selection != OldSelection)
                txtEdit.ResetFind();
            if (lbDuplicateWikilinks.SelectedIndex != -1)
            {
                string strLink = lbDuplicateWikilinks.SelectedItem.ToString();

                // remove the duplicate link count added to the end above
                strLink = Regex.Replace(strLink, @" \(\d+\)$", "");

                // perform case sensitive search, but make search on first character of link case insensitive
                // as first character may have been converted to upper case
                txtEdit.Find("\\[\\[(?i)" + Regex.Escape(strLink[0].ToString()) + @"(?-i)" + Regex.Escape(strLink.Remove(0, 1)) + "(\\|.*?)?\\]\\]", true, true, TheArticle.Name);
                btnRemove.Enabled = true;
            }
            else
            {
                txtEdit.ResetFind();
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
            OldSelection = selection;
        }

        private void ResetFind(object sender, EventArgs e)
        {
            txtEdit.ResetFind();
        }

        private void txtEdit_TextChanged(object sender, EventArgs e)
        {
            txtEdit.ResetFind();

            // After manual changes, automatic edit summary may be inaccurate, removing it altogether
            if (TheArticle != null && TheArticle.ArticleText != txtEdit.Text)
                TheArticle.EditSummary = "";
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            EditBoxTab.SelectedTab = tpEdit;
            txtEdit.Find(txtFind.Text, chkFindRegex.Checked, chkFindCaseSensitive.Checked, TheArticle.Name);
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
        private void Debug()
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
        private void Release()
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

                if (myPrefs.Language != Variables.LangCode || myPrefs.Project != Variables.Project
                    || (myPrefs.CustomProject != Variables.CustomProject))
                {
                    Variables.PHP5 = myPrefs.PrefPHP5;
                    SetProject(myPrefs.Language, myPrefs.Project, myPrefs.CustomProject);

                    Variables.User.WikiStatus = false;
                    BotMode = false;
                    lblOnlyBots.Visible = true;
                    Variables.User.IsBot = false;
                    Variables.User.IsAdmin = false;
                    CreateEditor();
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
            if (UserTalkWarningsLoaded)
                LoadUserTalkWarnings();
        }

        private void SetProject(LangCodeEnum code, ProjectEnum project, string customProject)
        {
            SplashScreen.SetProgress(81);
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
                        Parser.InterWikiOrder = InterWikiOrderEnum.LocalLanguageAlpha;
                        break;

                    case LangCodeEnum.he:
                    case LangCodeEnum.hu:
                        Parser.InterWikiOrder = InterWikiOrderEnum.AlphabeticalEnFirst;
                        break;

                    default:
                        Parser.InterWikiOrder = InterWikiOrderEnum.Alphabetical;
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

                UserTalkWarningsLoaded = false; // force reload

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

        int IntRestartDelay = 5, IntStartInSeconds = 5;
        private void DelayedRestart(object sender, EventArgs e)
        {
            StopDelayedAutoSaveTimer();
            StatusLabelText = "Restarting in " + IntStartInSeconds;

            if (IntStartInSeconds == 0)
            {
                StopDelayedRestartTimer();
                Start();
            }
            else
                IntStartInSeconds--;
        }

        private void StartDelayedRestartTimer(object sender, EventArgs e)
        {
            IntStartInSeconds = IntRestartDelay;
            Ticker += DelayedRestart;
            //increase the restart delay each time, this is decreased by 1 on each successfull save
            IntRestartDelay += 5;

            if (IntRestartDelay > 60)
                IntRestartDelay = 60;
        }

        private void StopDelayedRestartTimer()
        {
            Ticker -= DelayedRestart;
            IntStartInSeconds = IntRestartDelay;
        }

        private void StopDelayedAutoSaveTimer()
        {
            Ticker -= DelayedAutoSave;
            IntTimer = 0;
            lblBotTimer.Text = "Bot timer: " + IntTimer;
        }

        private void StartDelayedAutoSaveTimer()
        {
            Ticker += DelayedAutoSave;
        }

        int IntTimer;
        private void DelayedAutoSave(object sender, EventArgs e)
        {
            if (IntTimer < nudBotSpeed.Value)
            {
                IntTimer++;
                lblBotTimer.BackColor = (IntTimer == 1) ? Color.Red : DefaultBackColor;
            }
            else
            {
                StopDelayedAutoSaveTimer();
                SaveArticle();
            }

            lblBotTimer.Text = "Bot timer: " + IntTimer;
        }

        private void ShowTimer()
        {
            lblTimer.Visible = ShowMovingAverageTimer;
            StopSaveInterval();
        }

        int IntStartTimer;
        private void SaveInterval(object sender, EventArgs e)
        {
            IntStartTimer++;
            lblTimer.Text = "Timer: " + IntStartTimer;
        }

        private void StopSaveInterval()
        {
            IntStartTimer = 0;
            lblTimer.Text = "Timer: 0";
            Ticker -= SaveInterval;
        }

        public delegate void Tick(object sender, EventArgs e);
        public event Tick Ticker;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Ticker != null)
                Ticker(null, null);

            Seconds++;
            if (Seconds == 60)
            {
                Seconds = 0;
                EditsPerMin();
            }
        }

        int Seconds, LastTotal;
        private void EditsPerMin()
        {
            int editsInLastMin = NumberOfEdits - LastTotal;
            NumberOfEditsPerMinute = editsInLastMin;
            LastTotal = NumberOfEdits;
        }

        #endregion

        #region menus and buttons

        private void makeModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CModule.Show();
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
            StopProcessing = false;
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
                Comparer = new ListComparer(listMaker.GetArticleList());
            else
                Comparer = new ListComparer();

            Comparer.Show(this);
        }

        private void launchListSplitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WikiFunctions.AWBSettings.UserPrefs p = MakePrefs();

            if (listMaker.Count > 0 && MessageBox.Show("Would you like to copy your current Article List to the ListSplitter?", "Copy Article List?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Splitter = new ListSplitter(p, WikiFunctions.AWBSettings.UserPrefs.SavePluginSettings(p), listMaker.GetArticleList());
            else
                Splitter = new ListSplitter(p, WikiFunctions.AWBSettings.UserPrefs.SavePluginSettings(p));

            Splitter.Show(this);
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
            Parser.SortInterwikis = alphaSortInterwikiLinksToolStripMenuItem.Checked;
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
                        txtEdit.Find(txtFind.Text, chkFindRegex.Checked, chkFindCaseSensitive.Checked, TheArticle.Name);
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
            r.Wait();
            Enabled = true;

            txtEdit.Text = (string)r.Result;
        }

        private void unicodifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = txtEdit.SelectedText;
            text = Parser.Unicodify(text);
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

        private readonly Regex RegexDates = new Regex("[1-2][0-9]{3}");

        private void birthdeathCatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //find first dates
            string strBirth = "";
            string strDeath = "";

            try
            {
                MatchCollection m = RegexDates.Matches(txtEdit.Text);

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

                bool noChange;
                txtEdit.Text = Parsers.ChangeToDefaultSort(txtEdit.Text, TheArticle.Name, out noChange, restrictDefaultsortChangesToolStripMenuItem.Checked);
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
            if (!RplcSpecial.Visible)
                RplcSpecial.Show();
            else
                RplcSpecial.Hide();
        }

        private void btnMoreFindAndReplce_Click(object sender, EventArgs e)
        {
            FindAndReplace.ShowDialog(this);
        }

        private void Stop()
        {
            StopProcessing = true;
            PageReload = false;
            NudgeTimer.Stop();
            UpdateButtons(null, null);
            if (IntTimer > 0)
            {//stop and reset the bot timer.
                StopDelayedAutoSaveTimer();
                EnableButtons();
                return;
            }

            StopSaveInterval();
            StopDelayedRestartTimer();
            if (webBrowserEdit.IsBusy)
                webBrowserEdit.Stop2();
            if (Variables.User.WebBrowserLogin.IsBusy)
                Variables.User.WebBrowserLogin.Stop();

            listMaker.Stop();

            if (AutoSaveEditBoxEnabled)
                EditBoxSaveTimer.Enabled = false;

            StatusLabelText = "Stopped";
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            HelpForm.Show();
        }

        #region Edit Box Menu
        private void reparseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReparseEditBox();
        }

        private void ReparseEditBox()
        {
            ArticleEX a = new ArticleEX(TheArticle.Name) {OriginalArticleText = txtEdit.Text};

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

            txtEdit.Text = RemoveText.AddBack(text);
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

        bool LoadingTypos;
        private void chkRegExTypo_CheckedChanged(object sender, EventArgs e)
        {
            if (LoadingTypos)
                return;

            if (chkRegExTypo.Checked && BotMode)
            {
                MessageBox.Show("RegExTypoFix cannot be used with bot mode on.\r\nBot mode will now be turned off, and Typos loaded.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                BotMode = false;
            }
            LoadTypos(false);
        }

        private void LoadTypos(bool reload)
        {
            if (chkRegExTypo.Checked && (RegexTypos == null || reload))
            {
                LoadingTypos = true;
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
                RegexTypos.Complete += RegexTyposComplete;
            }
        }

        private delegate void GenericDelegate();
        private delegate void GenericDelegate1Parm(string parm);

        private void RegexTyposComplete(BackgroundRequest req)
        {
            if (InvokeRequired)
            {
                Invoke(new BackgroundRequestComplete(RegexTyposComplete), new object[] { req });
                return;
            }

            chkRegExTypo.Checked = chkSkipIfNoRegexTypo.Enabled = RegexTypos.TyposLoaded;

            if (RegexTypos.TyposLoaded)
            {
                StatusLabelText = RegexTypos.TypoCount + " typos loaded";
                if (!EditBoxTab.TabPages.Contains(tpTypos)) EditBoxTab.TabPages.Add(tpTypos);
                ResetTypoStats();
            }
            else
            {
                RegexTypos = null;
                if (EditBoxTab.TabPages.Contains(tpTypos)) EditBoxTab.TabPages.Remove(tpTypos);
            }

            LoadingTypos = false;
        }

        private void ProfileToLoad_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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

                if (se.ShowDialog(this) != DialogResult.OK) return;

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
            StopProcessing = false;
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
        private void MoveArticle()
        {
            using (ArticleActionDialog dlgArticleAction = new ArticleActionDialog(ArticleAction.Delete))
            {
                try
                {
                    dlgArticleAction.NewTitle = TheArticle.Name;
                    dlgArticleAction.Summary = LastMove;

                    if (dlgArticleAction.ShowDialog(this) == DialogResult.OK)
                    {
                        LastMove = dlgArticleAction.Summary;
                        webBrowserEdit.MovePage(TheArticle.Name, dlgArticleAction.NewTitle,
                                                ArticleActionSummary(dlgArticleAction));
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.Handle(ex);
                }
            }
        }

        private void DeleteArticle()
        {
            using (ArticleActionDialog dlgArticleAction = new ArticleActionDialog(ArticleAction.Delete))
            {
                try
                {
                    dlgArticleAction.Summary = LastDelete;

                    if (dlgArticleAction.ShowDialog(this) == DialogResult.OK)
                    {
                        LastDelete = dlgArticleAction.Summary;
                        webBrowserEdit.DeletePage(TheArticle.Name, ArticleActionSummary(dlgArticleAction));
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.Handle(ex);
                }
            }
        }

        private void ProtectArticle()
        {
            using (ArticleActionDialog dlgArticleAction = new ArticleActionDialog(ArticleAction.Delete))
            {
                try
                {
                    dlgArticleAction.Summary = LastProtect;

                    if (dlgArticleAction.ShowDialog(this) == DialogResult.OK)
                    {
                        LastProtect = dlgArticleAction.Summary;
                        webBrowserEdit.ProtectPage(TheArticle.Name, ArticleActionSummary(dlgArticleAction),
                                                   dlgArticleAction.EditProtectionLevel,
                                                   dlgArticleAction.MoveProtectionLevel, dlgArticleAction.ProtectExpiry,
                                                   dlgArticleAction.CascadingProtection);
                    }
                }
                catch (Exception ex)
                {
                    ErrorHandler.Handle(ex);
                }
            }
        }

        private string ArticleActionSummary(ArticleActionDialog dlgArticleAction)
        {
            return AddUsingAWBOnArticleAction
                       ? dlgArticleAction.Summary + " (" + Variables.SummaryTag.Trim() + ")"
                       : dlgArticleAction.Summary;
        }

        #endregion

        private void btnSubst_Click(object sender, EventArgs e)
        {
            SubstTemplates.ShowDialog();
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
                            txtDabLink.Text.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)))
                {
                    uint i;
                    // exclude years
                    if (uint.TryParse(a.Name, out i) && (i < 2100)) continue;

                    // disambigs typically link to pages in the same namespace only
                    if (Namespace.Determine(name) != a.NameSpaceKey) continue;

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
            SameArticleNudges = 0;
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

                // Tell plugins we're about to nudge, and give them the opportunity to cancel:
                foreach (KeyValuePair<string, IAWBPlugin> a in Plugin.Items)
                {
                    bool cancel;
                    a.Value.Nudge(out cancel);
                    
                    if (cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                }

                // Update stats and nudge:
                Nudges++;
                lblNudges.Text = NudgeTimerString + Nudges;
                NudgeTimer.Stop();
                if (chkNudgeSkip.Checked && SameArticleNudges > 0)
                {
                    SameArticleNudges = 0;
                    SkipPage("There was an error saving the page twice");
                }
                else
                {
                    SameArticleNudges++;
                    Stop();
                    Start();
                }

                // Inform plugins:
                foreach (KeyValuePair<string, IAWBPlugin> a in Plugin.Items)
                { a.Value.Nudged(Nudges); }
            }
        }
        public int Nudges { get; private set; }
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
        private void LoadUserTalkWarnings()
        {
            Regex userTalkTemplate = new Regex(@"# ?\[\["
                + Variables.NamespacesCaseInsensitive[Namespace.Template] + @"(.*?)\]\]");
            StringBuilder builder = new StringBuilder();

            UserTalkTemplatesRegex = null;
            UserTalkWarningsLoaded = true; // or it will retry on each page load
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
                UserTalkWarningsLoaded = false;
            }
            if (builder.Length > 1)
            {
                builder.Remove((builder.Length - 1), 1);
                UserTalkTemplatesRegex =
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
            if (EditBoxTab.SelectedTab == tpHistory)
                NewHistory();
            else if (EditBoxTab.SelectedTab == tpLinks)
                NewWhatLinksHere();
        }

        private void NewHistory()
        {
            try
            {
                if (EditBoxTab.SelectedTab == tpHistory && TheArticle != null)
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
            if (webBrowserHistory.Document != null && webBrowserHistory.Document.Body != null)
                webBrowserHistory.Document.Body.InnerHtml = ProcessHTMLForDisplay(webBrowserHistory.DocumentText);
        }

        private void NewWhatLinksHere()
        {
            try
            {
                if (EditBoxTab.SelectedTab == tpLinks && TheArticle != null)
                {
                    if (webBrowserLinks.Url !=
                        new Uri(Variables.URLIndex + "?title=Special:WhatLinksHere/" + TheArticle.URLEncodedName +
                                "&useskin=myskin") && !string.IsNullOrEmpty(TheArticle.URLEncodedName))
                        webBrowserLinks.Navigate(Variables.URLIndex + "?title=Special:WhatLinksHere/" +
                                                 TheArticle.URLEncodedName + "&useskin=myskin");
                }
                else
                    webBrowserLinks.Navigate("about:blank");
            }
            catch
            {
                webBrowserLinks.Navigate("about:blank");

                if (webBrowserLinks.Document != null)
                    webBrowserLinks.Document.Write("<html><body><p>Unable to load What Links Here</p></body></html>");
            }
        }

        private void webBrowserLinks_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowserLinks.Document != null && webBrowserLinks.Document.Body != null)
                webBrowserLinks.Document.Body.InnerHtml = ProcessHTMLForDisplay(webBrowserLinks.DocumentText);
        }

        private const string StartMark = "<!-- start content -->", EndMark = "<!-- end content -->";

        private string ProcessHTMLForDisplay(string linksHtml)
        {
            if (linksHtml.Contains(StartMark) && linksHtml.Contains(EndMark))
                linksHtml = Tools.StringBetween(linksHtml, StartMark, EndMark);

            linksHtml = linksHtml.Replace("<A ", "<a target=\"blank\" ");
            linksHtml = linksHtml.Replace("<FORM ", "<form target=\"blank\" ");
            return "<h3>" + TheArticle.Name + "</h3>" + linksHtml;
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
            Profiles.ShowDialog(this);
        }

        private void LoadProfileSettings(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Profiles.SettingsToLoad))
                LoadPrefs(Profiles.SettingsToLoad);
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

        private bool CanShutdown
        {
            get { return (chkShutdown.Checked && listMaker.Count == 0); }   
        }

        private void Shutdown()
        {
            if (CanShutdown)
            {
                ShutdownTimer.Enabled = true;
                ShutdownNotification shut = new ShutdownNotification {ShutdownType = GetShutdownType()};

                switch (shut.ShowDialog(this))
                {
                    case DialogResult.Cancel:
                        ShutdownTimer.Enabled = false;
                        MessageBox.Show(GetShutdownType() + " aborted!");
                        break;
                    case DialogResult.Yes:
                        if (GetShutdownType() == "Standby" || GetShutdownType() == "Hibernate")
                        {
                            shut.Close();
                            shut.Dispose();
                            ShutdownTimer.Enabled = false;
                        }
                        break;
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

            return radHibernate.Checked ? "Hibernate" : "";
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
            EditToolBarAction("<sup>Superscript text</sup>", 22, 16, "<sup>", "</sup>");
        }

        private void imgSub_Click(object sender, EventArgs e)
        {
            EditToolBarAction("<sub>Subscript text</sub>", 20, 14, "<sub>", "</sub>");
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
                times.Add(new KeyValuePair<int, string>((int)watch.ElapsedMilliseconds, p.Key + " > " + p.Value));
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
            ExtProgram.Show();
        }

        readonly CategoryNameForm CatName = new CategoryNameForm();

        private void categoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CatName.ShowDialog();

            if (string.IsNullOrEmpty(CatName.CategoryName)) return;

            txtEdit.Text += "\r\n\r\n[[" + CatName.CategoryName + "]]";
            ReparseEditBox();
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

        private void scrollToUnbalancedBracketsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scrollToUnbalancedBracketsToolStripMenuItem.Checked)
                focusAtEndOfEditTextBoxToolStripMenuItem.Checked = false;
        }

        private void focusAtEndOfEditTextBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (focusAtEndOfEditTextBoxToolStripMenuItem.Checked)
                scrollToUnbalancedBracketsToolStripMenuItem.Checked = false;
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
