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
using WikiFunctions.Controls;
using WikiFunctions.Background;
using System.Security.Permissions;
using WikiFunctions.Controls.Lists;
using AutoWikiBrowser.Plugins;
using System.Web;

namespace AutoWikiBrowser
{
    //TODO:Move any code that doesn't need to be directly behind the form to WF or other code files (Preferably WF)
    //TODO:Move regexes declared in method bodies (if not dynamic based on article title, etc), into class body
    //TODO:Move any Regexes to WikiRegexes as required

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public sealed partial class MainForm : Form, IAutoWikiBrowser
    { // this class needs to be public, otherwise we get an exception which recommends setting ComVisibleAttribute to true (which we've already done)
        #region Fields
        private readonly Splash SplashScreen = new Splash();
        private readonly WikiFunctions.Profiles.AWBProfilesForm Profiles;

        private bool Abort;
        private bool IgnoreNoBots;

        private string LastArticle = "";
        private string mSettingsFile = "";

        private const int MaxRetries = 10;

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
        private RegexTester RegexTester;
        private bool UserTalkWarningsLoaded;
        private Regex UserTalkTemplatesRegex;
        private bool Skippable = true;
        private FormWindowState LastState = FormWindowState.Normal; // doesn't look like we can use RestoreBounds for this - any other built in way?

        private ArticleRedirected ArticleWasRedirected;

        private ListComparer Comparer;
        private ListSplitter Splitter;

        List<TypoStat> TypoStats;

        private readonly Help HelpForm = new Help();

        private readonly WikiDiff Diff = new WikiDiff();
        private readonly JsAdapter DiffScriptingAdapter;

        /// <summary>
        /// Whether AWB is currently shutting down
        /// </summary>
        private bool ShuttingDown { get; set; }

        private readonly ToolStripMenuItem[] PasteMoreItems;
        private readonly string[] PasteMoreItemsPrefixes = new[] {
            "&1. ", "&2. ", "&3. ", "&4. ", "&5. ", "&6. ", "&7. ", "&8. ", "&9. ", "1&0. ", 
        };
        #endregion

        public Session TheSession
        { get; private set; }

        #region Constructor and MainForm load/resize
        public MainForm()
        {
            DiffScriptingAdapter = new JsAdapter(this);

            SplashScreen.Show(this);
            RightToLeft = System.Globalization.CultureInfo.CurrentCulture.TextInfo.IsRightToLeft
                ? RightToLeft.Yes : RightToLeft.No;

            SplashScreen.SetProgress(1);

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
                addToWatchList.SelectedIndex = 3;
                cmboCategorise.SelectedIndex = 0;
                cmboImages.SelectedIndex = 0;

                listMaker.UserInputTextBox.ContextMenuStrip = mnuMakeFromTextBox;
                listMaker.BusyStateChanged += SetProgressBar;
                listMaker.NoOfArticlesChanged += UpdateButtons;
                listMaker.StatusTextChanged += UpdateListStatus;
                listMaker.cmboSourceSelect.SelectedIndexChanged += ListMakerSourceSelectHandler;

                TheSession = new Session(this);
                CreateEditor();

                Profiles = new WikiFunctions.Profiles.AWBProfilesForm(TheSession);
                Profiles.LoggedIn += ProfileLoggedIn;

                SplashScreen.SetProgress(15);

                PasteMoreItems = new[]
                {
                    PasteMore1, PasteMore2, PasteMore3, PasteMore4, PasteMore5, PasteMore6, PasteMore7, PasteMore8, PasteMore9, PasteMore10, 
                };

                // to avoid saving to app data
                saveXML.InitialDirectory = openXML.InitialDirectory =
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
                SettingsFileDisplay = Program.Name;
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

            SplashScreen.SetProgress(22);

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

                webBrowser.Navigate("about:blank");
                webBrowser.ObjectForScripting = DiffScriptingAdapter;

                SplashScreen.SetProgress(25);

                logControl.Initialise(listMaker);

                Location = Properties.Settings.Default.WindowLocation;

                Size = Properties.Settings.Default.WindowSize;

                WindowState = Properties.Settings.Default.WindowState;

                Debug();
                Release();

                Plugin.LoadPluginsStartup(this, SplashScreen); // progress 25-50 in LoadPlugins()

                LoadPrefs(); // progress 50-59 in LoadPrefs()

                SplashScreen.SetProgress(60);
                UpdateButtons(null, null);
                SplashScreen.SetProgress(62);
                LoadRecentSettingsList(); // progress 63-66 in LoadRecentSettingsList()

                Updater.UpdateUpdaterFile(SplashScreen.SetProgress); // progress 67-70 in UpdateUpdaterFile()

                if (Updater.Result == Updater.AWBEnabledStatus.None || Updater.Result == Updater.AWBEnabledStatus.Error)
                    Updater.Update();

                Updater.WaitForCompletion();

                SplashScreen.SetProgress(80);

                bool optUpdate = ((Updater.Result & Updater.AWBEnabledStatus.OptionalUpdate) ==
                                 Updater.AWBEnabledStatus.OptionalUpdate),
                     updaterUpdate = ((Updater.Result & Updater.AWBEnabledStatus.UpdaterUpdate) ==
                                     Updater.AWBEnabledStatus.UpdaterUpdate);

                if (optUpdate || updaterUpdate)
                {
                    bool runUpdater = false;

                    if (updaterUpdate)
                    {
                        MessageBox.Show("There is an Update to the AWB updater. Updating Now", "Updater update",
                                        MessageBoxButtons.YesNo);
                        runUpdater = true;
                    }

                    if (!runUpdater &&
                        MessageBox.Show(
                            "This version has been superceeded by a new version. You may continue to use this version or update to the newest version.\r\n\r\nWould you like to automatically upgrade to the newest version?",
                            "Upgrade?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        runUpdater = true;

                    if (runUpdater)
                    {
                        Updater.RunUpdater();
            			SplashScreen.Close();
                        CloseAWB();
                        return;
                    }
                }

                if ((Updater.Result & Updater.AWBEnabledStatus.Disabled) == Updater.AWBEnabledStatus.Disabled)
                    OldVersion();

                if ((Updater.Result & Updater.AWBEnabledStatus.Error) == Updater.AWBEnabledStatus.Error)
                {
                    lblUserName.BackColor = Color.Red;
                    MessageBox.Show(this, "Cannot load version check page from Wikipedia. "
                                          + "Please verify that you're connected to Internet.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                SplashScreen.SetProgress(90);

                Profiles.Login(ProfileToLoad);

                SplashScreen.SetProgress(95);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }

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

        private bool SaveArticleList = true;
        private bool AutoSaveEditBoxEnabled;

        private string AutoSaveEditBoxFile = Application.StartupPath + "\\Edit Box.txt";

        private bool SuppressUsingAWB;

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
                    lblStatusText.Text = Program.Name + " " + Program.VersionString;
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
            if (!TheSession.User.IsLoggedIn)
            {
                MessageBox.Show("You've been logged off, probably due to loss of session data.\r\n" +
                    "Please relogin.", "Logged off", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Stop();
                CheckStatus(false);
                return false;
            }
            return true;
        }

        private void CreateEditor()
        {
            TheSession.PreviewComplete += PreviewComplete;
            TheSession.ExceptionCaught += ApiEditExceptionCaught;
            TheSession.SaveComplete += PageSaved;
            TheSession.MaxlagExceeded += MaxlagExceeded;
            TheSession.OpenComplete += OpenComplete;
        }

        private void OpenComplete(AsyncApiEdit editor, PageInfo page)
        {
            PageLoaded(page);
        }

        private void MaxlagExceeded(AsyncApiEdit sender, int maxlag, int retryAfter)
        {
            Retries++;

            if (Retries < MaxRetries)
            {
                StartDelayedRestartTimer(retryAfter);
            }
            else
            {
                Stop();
                MessageBox.Show(this, "Maxlag exceeded " + MaxRetries + " times in a row. Processing stopped, " +
                    "please try later when the server is under a less load.", "Stopped", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ApiEditExceptionCaught(AsyncApiEdit sender, Exception ex)
        {
            if (ex is InterwikiException)
                SkipPage(ex.Message);
            else if (ex is SpamlistException)
            {
                string message = "Text '" + (ex as SpamlistException).URL + "' is blocked by spam blacklist";

                if (!BotMode)
                {
                    if (!chkSkipSpamFilter.Checked
                        && MessageBox.Show(message + ".\r\nTry and edit again?",
                            "Spam blacklist", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        Start();
                        return;
                    }
                }
                SkipPage(message);
            }
            else if (ex is ApiErrorException)
            {
                switch ((ex as ApiErrorException).ErrorCode)
                {
                    case "editconflict":
                        //TODO: must be a less crude way
                        MessageBox.Show(this, "There has been an edit conflict. AWB will now re-apply its changes on the updated page. \r\n Please re-review the changes before saving. Any Custom edits will be lost, and have to be re-added manually.", "Edit conflict");
                        NudgeTimer.Stop();
                        Start();
                        break;

                    default:
                        StartDelayedRestartTimer();
                        break;
                }
            }
            else if (ex is NewMessagesException)
            {
                WeHaveNewMessages();
            }
            else
            {
                Stop();
                ErrorHandler.Handle(ex);
                //StartDelayedRestartTimer();
            }
        }

        private void OpenPage(string title)
        {
            StatusLabelText = "Loading...";
            TheSession.Editor.Open(title);
        }

        private bool StopProcessing;
        private bool InStart, StartAgain;

        private void Start()
        {
            if (TheSession.Status != WikiStatusResult.Registered) return;
            if (InStart)
            {
                StartAgain = true;
                return;
            }
            InStart = true;
            do
            {
                StartAgain = false;
                StartProcess();
            } while (StartAgain);
            InStart = false;
        }

        /// <summary>
        /// </summary>
        /// <returns>true if it is ok to call again, or false if processing should now stop</returns>
        private void StartProcess()
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

                if (Variables.Project != ProjectEnum.custom && string.IsNullOrEmpty(cmboEditSummary.Text) &&
                    Plugin.AWBPlugins.Count == 0)
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

                Skippable = true;
                txtEdit.Clear();

                ArticleInfo(true);

                if (listMaker.NumberOfArticles < 1)
                {
                    StopSaveInterval();
                    lblTimer.Text = "";
                    StatusLabelText = "No articles in list, you need to use the Make list";
                    Text = Program.Name;
                    listMaker.MakeListEnabled = true;
                    return;
                }

                string title = listMaker.SelectedArticle().Name;

                if (!Tools.IsValidTitle(title))
                {
                    SkipPage("Invalid page title");
                    return;
                }

                string fixedTitle = Parsers.CanonicalizeTitleAggressively(title);
                if (fixedTitle != title)
                {
                    listMaker.ReplaceArticle(listMaker.SelectedArticle(), new Article(fixedTitle));
                    title = fixedTitle;
                }

                if (BotMode)
                    NudgeTimer.StartMe();

                TheArticle = new ArticleEX(title, "");

                //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#.27Find.27_sometimes_fails_to_use_the_search_key
                txtEdit.ResetFind();

                NewHistory(title);
                NewWhatLinksHere(title);

                EditBoxSaveTimer.Enabled = AutoSaveEditBoxEnabled;

                //if (dlg != null && dlg.AutoProtectAll)
                //    TheArticle.Protect(TheSession);

                StartProgressBar();

                //Navigate to edit page
                OpenPage(title);
            }
            catch (Exception ex)
            {
                Tools.WriteDebug(Name, "Start() error: " + ex.Message);
                StartDelayedRestartTimer();
            }

            if (Program.MyTrace.StoppedWithConfigError)
            {
                try
                { Program.MyTrace.ValidateUploadProfile(); }
                catch (Exception ex)
                { Program.MyTrace.ConfigError(ex); }
            }
        }

        // counts number of redirects so that we catch double redirects
        private int Redirects;
        private int UnbalancedBracket, BracketLength;

        private void SkipRedirect(string redirectTitle, string reason)
        {
            listMaker.Remove(TheArticle); // or we get stuck in a loop
            TheArticle = new ArticleEX(redirectTitle, "");
            // if we didn't do this, we were writing the SkipPage info to the AWBLogListener belonging to the object redirect and resident in the MyTrace collection, but then attempting to add TheArticle's log listener to the logging tab
            SkipPage(reason);
        }

        private void PageLoaded(PageInfo page)
        {
            if (!LoadSuccessApi())
                return;

            Retries = 0;

            StopProgressBar();

            if (StopProcessing)
                return;

            TheArticle = new ArticleEX(page);

            if (!preParseModeToolStripMenuItem.Checked && !CheckLoginStatus()) return;

            if (Program.MyTrace.HaveOpenFile)
                Program.MyTrace.WriteBulletedLine("AWB started processing", true, true, true);
            else
                Program.MyTrace.Initialise();

            Text = SettingsFileDisplay + " - " + page.Title;

            bool articleIsRedirect = Tools.IsRedirect(page.Text);

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
                string redirect = Parsers.CanonicalizeTitleAggressively(Tools.RedirectTarget(page.Text));

                if (redirect != "" && Tools.IsValidTitle(redirect))
                {
                    if (filterOutNonMainSpaceToolStripMenuItem.Checked 
                        && (Namespace.Determine(redirect) != Namespace.Article))
                    {
                        SkipRedirect(redirect, "Page is not in mainspace");
                        return;
                    }

                    if (redirect == TheArticle.Name)
                    {
                        //ignore recursive redirects
                        SkipRedirect(redirect, "Recursive redirect");
                        return;
                    }

                    if (ArticleWasRedirected != null)
                        ArticleWasRedirected(TheArticle.Name, redirect);

                    listMaker.ReplaceArticle(TheArticle, new Article(redirect));
                    TheArticle = new ArticleEX(redirect, "");

                    // don't allow redirects to a redirect as we could go round in circles
                    if (Redirects > 1)
                    {
                        SkipPage("Double redirect");
                        return;
                    }

                    OpenPage(redirect);

                    return;
                }
            }

            ErrorHandler.CurrentRevision = page.RevisionID;

            if (PageReload)
            {
                PageReload = false;
                GetDiff();
                return;
            }

            if (!preParseModeToolStripMenuItem.Checked && SkipChecks(!chkSkipAfterProcessing.Checked)) // normal mode, pre-processing of article
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

                UpdateCurrentTypoStats();

                if (!Abort)
                {
                    if (TheArticle.SkipArticle)
                    {
                        SkipPageReasonAlreadyProvided(); // Don't send a reason; ProcessPage() should already have logged one
                        return;
                    }

                    if (Skippable && (chkSkipNoChanges.Checked || BotMode) && TheArticle.NoArticleTextChanged)
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

                    if (chkSkipNoPageLinks.Checked 
                        && (WikiRegexes.WikiLinksOnly.Matches(TheArticle.ArticleText).Count == 0))
                    {
                        SkipPage("Page contains no links");
                        return;
                    }

                    // post-processing
                    if (chkSkipAfterProcessing.Checked && SkipChecks(true))
                        return;
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
                    if (autoSaveSettingsToolStripMenuItem.Checked && !(NumberOfIgnoredEdits % 10 == 0) && !string.IsNullOrEmpty(SettingsFile))
                        SavePrefs(SettingsFile);
                }

                return;
            }

            if (syntaxHighlightEditBoxToolStripMenuItem.Checked)
                txtEdit.Visible = false;

            txtEdit.Text = TheArticle.ArticleText;

            //Update statistics and alerts
            if (!BotMode)
                ArticleInfo(false);

            if (!Abort)
            {
                bool diffInBotMode = (toolStripDiffInBotMode.Enabled && toolStripDiffInBotMode.Checked);
                if (BotMode)
                {
                    StartDelayedAutoSaveTimer();

                    if (!diffInBotMode)
                        return;
                }

                switch (toolStripComboOnLoad.SelectedIndex)
                {
                    case 0:
                        GetDiff();

                        if (diffInBotMode)
                            return;

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

                SetWatchButton(TheSession.Page.IsWatched);

                txtReviewEditSummary.Text = MakeSummary();

                Variables.Profiler.Profile("Make Edit summary");

                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Working_with_Alerts
                if (chkSkipIfNoAlerts.Checked && lblWarn.Text.Length == 0)
                {
                    SkipPage("Page has no alerts");
                    return;
                }

                Variables.Profiler.Profile("Alerts");

                // syntax highlighting of edit box based on m:extension:wikEd standards
                if (syntaxHighlightEditBoxToolStripMenuItem.Checked)
                {
                    txtEdit.Visible = false;

                    HighlightSyntax();
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
                    if (UnbalancedBracket < 0)
                        btnSave.Focus();
                    else if (scrollToUnbalancedBracketsToolStripMenuItem.Checked)
                    {
                        EditBoxTab.SelectedTab = tpEdit;

                        HighlightUnbalancedBrackets();
                    }
                }
                StatusLabelText = "Ready to save";
            }
            else
            {
                EnableButtons();
                Abort = false;
            }
        }

        /// <summary>
        /// Applies syntax highlighting to the Edit Text Box
        /// </summary>
        /// <returns></returns>
        private void HighlightSyntax()
        {
            // temporarily disable TextChanged firing to help performance of this function
            txtEdit.TextChanged -= txtEdit_TextChanged;

            txtEdit.HighlightSyntax();

            txtEdit.TextChanged += txtEdit_TextChanged;
        }

        private IArticleComparer ContainsComparer;
        private IArticleComparer NotContainsComparer;
        /// <summary>
        /// Skips the article based on protection level and contains/not contains logic
        /// </summary>
        /// <param name="checkContainsNotContains">whether to test contains/not contains logic</param>
        /// <returns>Whether the page has been skipped</returns>
        private bool SkipChecks(bool checkContainsNotContains)
        {
            if (!TheSession.User.CanEditPage(TheSession.Page))
            {
                SkipPage("Page is protected");
                return true;
            }

            if (ContainsComparer == null)
                MakeSkipChecks();

            if (checkContainsNotContains)
            {
                if (chkSkipIfContains.Checked && ContainsComparer.Matches(TheArticle))
                {
                    SkipPage("Page contains: " + txtSkipIfContains.Text);
                    return true;
                }

                if (chkSkipIfNotContains.Checked && !NotContainsComparer.Matches(TheArticle))
                {
                    SkipPage("Page does not contain: " + txtSkipIfNotContains.Text);
                    return true;
                }
            }

            if (!Skip.SkipIf(TheArticle.OriginalArticleText))
            {
                SkipPage("skipIf custom code");
                return true;
            }

            return false;
        }

        private void InvalidateSkipChecks()
        {
            ContainsComparer = null;
            NotContainsComparer = null;
        }

        private void MakeSkipChecks()
        {
            ContainsComparer = ArticleComparerFactory.Create(txtSkipIfContains.Text,
                                                             chkSkipCaseSensitive.Checked,
                                                             chkSkipIsRegex.Checked,
                                                             false, // singleline
                                                             false); // multiline
            NotContainsComparer = ArticleComparerFactory.Create(txtSkipIfNotContains.Text,
                                                                chkSkipCaseSensitive.Checked,
                                                                chkSkipIsRegex.Checked,
                                                                false, // singleline
                                                                false); // multiline
        }

        /// <summary>
        /// Skips the article based on protection level and contains/not contains logic
        /// </summary>
        /// <returns>Whether the page has been skipped</returns>
        private bool SkipChecks()
        {
            return SkipChecks(true);
        }

        private void ClearBrowser()
        {
            if (webBrowser.Document != null)
            {
                webBrowser.Document.OpenNew(true);
            }
        }

        private void Bleepflash()
        {
            if (ContainsFocus) return;
#if !MONO
            if (Flash) Tools.FlashWindow(this);
#endif
            if (Beep) Tools.Beep();
        }

        private readonly TalkMessage DlgTalk = new TalkMessage();

        private void WeHaveNewMessages()
        {
            Stop();
            Focus();
            TheSession.RequireUpdate();

            if (DlgTalk.ShowDialog() == DialogResult.Yes)
                Tools.OpenUserTalkInBrowser(TheSession.User.Name);
            else
                Process.Start("iexplore", Variables.GetUserTalkURL(TheSession.User.Name));
        }

        private bool LoadSuccessApi()
        {
            try
            {
                if (!TheSession.Editor.Page.Exists && radSkipNonExistent.Checked)
                {
                    SkipPage("Non-existent page");
                    return false;
                }
                if (TheSession.Editor.Page.Exists && radSkipExistent.Checked)
                {
                    SkipPage("Existing page");
                    return false;
                }

                if (!preParseModeToolStripMenuItem.Checked && TheSession.User.HasMessages)
                {
                    WeHaveNewMessages();
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

        private void PageSaved(AsyncApiEdit sender, SaveInfo saveInfo)
        {
            ClearBrowser();

            //TODO:Reinstate as needed
            //try
            //{
            //    if (IsReadOnlyDB())
            //    {
            //        StartDelayedRestartTimer(null, null);
            //        return;
            //    }
            //}
            //catch (Exception)
            //{
            //    Start();
            //}

            //lower restart delay
            if (RestartDelay > 5)
                RestartDelay -= 1;

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
            if (autoSaveSettingsToolStripMenuItem.Checked && (NumberOfEdits % 10 == 0) && !string.IsNullOrEmpty(SettingsFile) && (NumberOfEdits > 5))
                SavePrefs(SettingsFile);

            Start();
        }

        private void SkipPageReasonAlreadyProvided()
        {
            try
            {
                TheSession.Editor.Reset();
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
                // Must be performed regardless of general fixes, otherwise there may be breakage
                theArticle.AWBChangeArticleText("Fixes for Unicode comaptibility",
                    Parser.FixUnicode(theArticle.ArticleText),
                    true);

                if (NoParse.Contains(theArticle.Name))
                    process = false;

                if (!IgnoreNoBots &&
                    !Parsers.CheckNoBots(theArticle.ArticleText, TheSession.User.Name))
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

                if (Plugin.AWBPlugins.Count > 0)
                {
                    foreach (KeyValuePair<string, IAWBPlugin> a in Plugin.AWBPlugins)
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
                    string newlines = "";
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
                if (!preParseModeToolStripMenuItem.Checked && chkEnableDab.Checked && txtDabLink.Text.Trim().Length > 0 &&
                    txtDabVariants.Text.Trim().Length > 0)
                {
                    if (theArticle.Disambiguate(TheSession, txtDabLink.Text.Trim(), txtDabVariants.Lines, BotMode,
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
                webBrowser.BringToFront();
                if (webBrowser.Document == null)
                    return;

                webBrowser.Document.OpenNew(false);
                if (TheArticle.OriginalArticleText == txtEdit.Text)
                {
                    webBrowser.Document.Write(
                        @"<h2 style='padding-top: .5em;
padding-bottom: .17em;
border-bottom: 1px solid #aaa;
font-size: 150%;'>No changes</h2><p>Press the ""Ignore"" button below to skip to the next page.</p>");
                }
                else
                {
                    // when less than 10 edits show user help info on double click to undo etc.
                    webBrowser.Document.Write("<!DOCTYPE HTML PUBLIC \" -//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">"
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

                txtEdit.Focus();
                txtEdit.SelectionLength = 0;

                GuiUpdateAfterProcessing();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        private void GetPreview()
        {
            //TODO:We need some GUI feedback that preview is happening.. Status label and bar thingy?
            if (!TheSession.Editor.IsActive) TheSession.Editor.Preview(TheArticle.Name, txtEdit.Text);
        }

        private void PreviewComplete(AsyncApiEdit sender, string result)
        {
            LastArticle = txtEdit.Text;

            Skippable = false;

            if (webBrowser.Document != null)
            {
                webBrowser.Document.OpenNew(false);
                webBrowser.Document.Write("<html><head>"
                    + sender.HtmlHeaders
                    + "</head><body style=\"background:white; margin:10px; text-align:left;\">"
                    + result
                    + "</html>"
                    );
                webBrowser.BringToFront();
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
            // Fail-safe against http://es.wikipedia.org/w/index.php?diff=28114575
            if (TheArticle == null || TheArticle.Name != TheSession.Page.Title)
                throw new Exception("Attempted to save a wrong page");

            StatusLabelText = "Saving...";
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
            //remember article text in case it is lost, this is set to "" again when the article title is removed
            LastArticle = txtEdit.Text;

            if (ShowMovingAverageTimer)
            {
                StopSaveInterval();
                Ticker += SaveInterval;
            }
            WatchOptions opt;

            switch (addToWatchList.SelectedIndex)
            {
                case 0:
                    opt = WatchOptions.Watch;
                    break;
                case 1:
                    opt = WatchOptions.Unwatch;
                    break;
                case 3:
                    opt = WatchOptions.UsePreferences;
                    break;
                default:
                    opt = WatchOptions.NoChange;
                    break;
            }

            TheSession.Editor.Save(txtEdit.Text, MakeSummary(), markAllAsMinorToolStripMenuItem.Checked,
                                   opt);
        }

        #endregion

        #region extra stuff

        #region Diff

        private enum DiffChangeMode { Deletion, Change, Addition };

        /// <summary>
        /// This class serves as a proxy between the main window and WebBrowser, isolating the former
        /// from malicious site JS calls of window.external.
        /// </summary>
        [System.Runtime.InteropServices.ComVisibleAttribute(true)]
        public class JsAdapter
        {
            readonly MainForm Owner;

            internal JsAdapter(MainForm owner)
            {
                Owner = owner;
            }

            /// <summary>
            /// Reverses the changes to a line of text in the page
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            public void UndoChange(int left, int right)
            {
                Owner.UndoChangeGeneric(DiffChangeMode.Change, left, right);
            }

            /// <summary>
            /// Reverses the deletion of a line of text from the page
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            public void UndoDeletion(int left, int right)
            {
                Owner.UndoChangeGeneric(DiffChangeMode.Deletion, left, right);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="right"></param>
            public void UndoAddition(int right)
            {
                Owner.UndoChangeGeneric(DiffChangeMode.Addition, 0, right);
            }

            /// <summary>
            /// Moves the caret to the input line within the article text box
            /// </summary>
            /// <param name="destLine">the line number the caret should be moved to</param>
            public void GoTo(int destLine)
            {
                Owner.GoTo(destLine);
            }
        }

        /// <summary>
        /// Reverses the change, addition or deletion of a line of text in the page
        /// </summary>
        /// <param name="changeType"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        private void UndoChangeGeneric(DiffChangeMode changeType, int left, int right)
        {
            try
            {
                int caretPosition = txtEdit.SelectionStart;
                GetDiff(); // to pick up any manual changes from edit box

                switch (changeType)
                {
                    case DiffChangeMode.Change:
                        txtEdit.Text = Diff.UndoChange(left, right);
                        break;
                    case DiffChangeMode.Deletion:
                        txtEdit.Text = Diff.UndoDeletion(left, right);
                        break;
                    case DiffChangeMode.Addition:
                        txtEdit.Text = Diff.UndoAddition(right);
                        break;
                }

                TheArticle.EditSummary = "";
                GetDiff();

                // now put caret back where it was
                txtEdit.Select(Math.Min(caretPosition, txtEdit.Text.Length), 0);
                txtEdit.ScrollToCaret();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        /// <summary>
        /// Moves the caret to the input line within the article text box
        /// </summary>
        /// <param name="destLine">the line number the caret should be moved to</param>
        private void GoTo(int destLine)
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
                    txtEdit.Select(mc[destLine - 1].Index + 2 - destLine, 0);
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
            if (listMaker.Visible)
            {
                btntsShowHideParameters.Image = Resources.Showhideparameters2;

                OldPosition = EditBoxTab.Location;
                EditBoxTab.Location = new Point(listMaker.Location.X, listMaker.Location.Y - 5);

                OldSize = EditBoxTab.Size;
                EditBoxTab.Size = new Size((EditBoxTab.Size.Width + MainTab.Size.Width + listMaker.Size.Width + 8), EditBoxTab.Size.Height);
            }
            else
            {
                btntsShowHideParameters.Image = Resources.Showhideparameters;

                EditBoxTab.Location = OldPosition;
                EditBoxTab.Size = OldSize;
            }
            listMaker.Visible = MainTab.Visible = !listMaker.Visible;
        }

        private void UpdateStatusUI()
        {
            UpdateUserName();
            UpdateBotStatus();
            UpdateAdminStatus();
        }

        private void UpdateUserName()
        {
            if (string.IsNullOrEmpty(TheSession.User.Name))
            {
                lblUserName.Text = Variables.Namespaces[Namespace.User];
            }
            else
            {
                lblUserName.Text = TheSession.User.Name;
            }

            if (TheSession.Status == WikiStatusResult.Registered)
            {
                lblUserName.BackColor = Color.Green;
                btnStart.Enabled = true;
            }
            else
            {
                lblUserName.BackColor = Color.Red;
                btnStart.Enabled = false;
            }
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

                TheSession.Editor.Abort();

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

            if ((TheSession.User.IsBot && chkSuppressTag.Checked)
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

        private void UpdateBotStatus()
        {
            bool bot = TheSession.IsBot;
            chkAutoMode.Enabled = chkSuppressTag.Enabled = bot;

            lblOnlyBots.Visible = !bot;

            if (!Variables.UsingMono) // fails unexplainably under Mono
            {
                if (bot)
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
        }

        private void UpdateAdminStatus()
        {
            btnProtect.Enabled = btnMove.Enabled = btnDelete.Enabled = btntsDelete.Enabled =
                TheSession.IsSysop && btnSave.Enabled && (TheArticle != null);
        }

        private void chkAutoMode_CheckedChanged(object sender, EventArgs e)
        {
            if (BotMode)
            {
                SetBotModeEnabled(true);
                chkNudge.Checked = true;
                chkNudgeSkip.Checked = false;

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
            new AboutBox(webBrowserHistory.Version.ToString(), time, NumberOfEdits).Show();
        }

        public bool CheckStatus(bool fromLoginForm)
        {
            StatusLabelText = "Loading page to check if we are logged in.";

            bool b = false;
            string label = "Software disabled";

            switch (TheSession.Update())
            {
                case WikiStatusResult.Error:
                    MessageBox.Show("Check page failed to load.\r\n\r\nCheck your Internet is working and that the Wikipedia servers are online.", "User check problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case WikiStatusResult.NotLoggedIn:
                    if (!fromLoginForm)
                    {
                        MessageBox.Show("You are not logged in. The profile screen will now load, enter your name and password, click \"Log in\", wait for it to complete, then start the process again.", "Not logged in", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Profiles.ShowDialog();
                    }
                    break;

                case WikiStatusResult.NotRegistered:
                    MessageBox.Show(TheSession.User.Name + " is not enabled to use this.", "Not enabled", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    string site = Variables.Project == ProjectEnum.wikia
                        ? "http://www.wikia.com/wiki/"
                        : Variables.URLIndex + "?title=";
                    Tools.OpenURLInBrowser(site + "Project:AutoWikiBrowser/CheckPage");
                    break;

                case WikiStatusResult.OldVersion:
                    OldVersion();
                    break;

                case WikiStatusResult.Registered:
                    b = true;
                    label = string.Format("Logged in, user and software enabled. Bot = {0}, Admin = {1}", TheSession.User.IsBot, TheSession.User.IsSysop);

                    //Get list of articles not to apply general fixes to.
                    Match n = WikiRegexes.NoGeneralFixes.Match(TheSession.CheckPageText);
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
            UpdateStatusUI();
            UpdateButtons(null, null);

            return b;
        }

        private void OldVersion()
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseAWB();
        }

        private void CloseAWB()
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

                text = Regex.Replace(text, "^" + Variables.NamespacesCaseInsensitive[Namespace.Category], "");
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

                if (TheArticle.NameSpaceKey == Namespace.Article && WikiRegexes.Stub.IsMatch(articleText) &&
                    intWords > 500)
                    lblWarn.Text = "Long article with a stub tag.\r\n";

                if (intCats == 0)
                    lblWarn.Text += "No category (although one may be in a template)\r\n";

                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Replace_nofootnotes_with_morefootnote_if_references_exists
                if (TheArticle.HasMorefootnotesAndManyReferences)
                    lblWarn.Text += @"Has 'No/More footnotes' template yet many references" + "\r\n";

                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.28Yet.29_more_reference_related_changes.
                if (TheArticle.HasRefAfterReflist)
                    lblWarn.Text += @"Has a <ref> after <references/>" + "\r\n";

                if (articleText.StartsWith("=="))
                    lblWarn.Text += "Starts with heading.";

                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Format_references
                if (TheArticle.HasBareReferences)
                    lblWarn.Text += @"Unformatted references found" + "\r\n";

                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Detect_multiple_DEFAULTSORT
                if (WikiRegexes.Defaultsort.Matches(txtEdit.Text).Count > 1)
                    lblWarn.Text += "Multiple DEFAULTSORTs found\r\n";

                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Some_additional_edits
                if (TheArticle.HasDeadLinks)
                    lblWarn.Text += "Dead links found\r\n";

                UnbalancedBracket = TheArticle.UnbalancedBrackets(ref BracketLength);
                if (UnbalancedBracket > 0)
                    lblWarn.Text += "Unbalanced brackets found\r\n";

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
                        lbDuplicateWikilinks.Items.Add(z + @" (" +
                                                       (Regex.Matches(articleText,
                                                                      @"\[\[" + Regex.Escape(z) + @"(\|.*?)?\]\]").Count +
                                                        Regex.Matches(articleText,
                                                                      @"\[\[" + Regex.Escape(Tools.TurnFirstToLower(z)) +
                                                                      @"(\|.*?)?\]\]").Count) + @")");
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
            bypassAllRedirectsToolStripMenuItem.Enabled = true;
            profileTyposToolStripMenuItem.Visible = true;

#if DEBUG
            Variables.Profiler = new Profiler("profiling.txt", true);
#endif
        }

        [Conditional("RELEASE")]
        private void Release()
        {
            if (MainTab.Contains(tpBots) && !Variables.UsingMono)
                MainTab.Controls.Remove(tpBots);
        }

        #endregion

        #region set variables

        private void PreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyPreferences myPrefs = new MyPreferences(Variables.LangCode, Variables.Project,
                Variables.CustomProject, txtEdit.Font, LowThreadPriority, Flash, Beep,
                Minimize, SaveArticleList, AutoSaveEditBoxEnabled, AutoSaveEditBoxFile,
                AutoSaveEditBoxPeriod, SuppressUsingAWB, Article.AddUsingAWBOnArticleAction, IgnoreNoBots,
                ShowMovingAverageTimer, Variables.PHP5);

            if (myPrefs.ShowDialog(this) == DialogResult.OK)
            {
                txtEdit.Font = myPrefs.TextBoxFont;
                LowThreadPriority = myPrefs.LowThreadPriority;
                Flash = myPrefs.PrefFlash;
                Beep = myPrefs.PrefBeep;
                Minimize = myPrefs.PrefMinimize;
                SaveArticleList = myPrefs.PrefSaveArticleList;
                AutoSaveEditBoxEnabled = myPrefs.PrefAutoSaveEditBoxEnabled;
                AutoSaveEditBoxPeriod = myPrefs.PrefAutoSaveEditBoxPeriod;
                AutoSaveEditBoxFile = myPrefs.PrefAutoSaveEditBoxFile;
                SuppressUsingAWB = myPrefs.PrefSuppressUsingAWB;
                Article.AddUsingAWBOnArticleAction = myPrefs.PrefAddUsingAWBOnArticleAction;
                IgnoreNoBots = myPrefs.PrefIgnoreNoBots;
                ShowMovingAverageTimer = myPrefs.PrefShowTimer;

                if (myPrefs.Language != Variables.LangCode || myPrefs.Project != Variables.Project
                    || (myPrefs.CustomProject != Variables.CustomProject))
                {
                    Variables.PHP5 = myPrefs.PrefPHP5;
                    SetProject(myPrefs.Language, myPrefs.Project, myPrefs.CustomProject);

                    BotMode = false;
                    lblOnlyBots.Visible = true;

                    //CreateEditor();
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

        private void SetProject(string code, ProjectEnum project, string customProject)
        {
            SplashScreen.SetProgress(81);
            try
            {
                //set namespaces
                Variables.SetProject(code, project, customProject);

                //set interwikiorder
                switch (Variables.LangCode)
                {
                    case "en":
                    case "pl":
                    case "simple":
                        Parser.InterWikiOrder = InterWikiOrderEnum.LocalLanguageAlpha;
                        break;

                    case "he":
                    case "hu":
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
                    lblProject.Text = Variables.LangCode + "." + Variables.Project;
                else lblProject.Text = Variables.IsWikimediaMonolingualProject ? Variables.Project.ToString() : Variables.URL;

                ResetTypoStats();
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("The interwiki list didn't load correctly. Please check your internet connection, and then restart AWB");
            }
        }

        #endregion

        //TODO: Cleanup/refactor UI update functions
        #region Enabling/Disabling of buttons

        private void UpdateButtons(object sender, EventArgs e)
        {
            SetStartButton(listMaker.NumberOfArticles > 0);

            lbltsNumberofItems.Text = "Pages: " + listMaker.NumberOfArticles;
            bypassAllRedirectsToolStripMenuItem.Enabled = TheSession.User.IsSysop;
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

            txtEdit.Enabled = false;
        }

        private void EnableButtons()
        {
            UpdateButtons(null, null);
            SetButtons(true);
            txtEdit.Enabled = true;
        }

        private void SetButtons(bool enabled)
        {
            btnSave.Enabled = btnIgnore.Enabled = btnPreview.Enabled = btnDiff.Enabled =
            btntsPreview.Enabled = btntsChanges.Enabled = listMaker.MakeListEnabled =
            btntsSave.Enabled = btntsIgnore.Enabled = /*btnWatch.Enabled = */ findGroup.Enabled = enabled;

            btnDelete.Enabled = btntsDelete.Enabled = btnMove.Enabled = btnProtect.Enabled = (enabled && TheSession.User.IsSysop && (TheArticle != null));
        }

        #endregion

        #region Timers

        int RestartDelay = 5;
        int StartInSeconds = 5;
        private void DelayedRestart(object sender, EventArgs e)
        {
            StopDelayedAutoSaveTimer();
            StatusLabelText = "Restarting in " + StartInSeconds;

            if (StartInSeconds == 0)
            {
                StopDelayedRestartTimer();
                Start();
            }
            else
                StartInSeconds--;
        }

        private void StartDelayedRestartTimer()
        {
            //increase the restart delay each time, this is decreased by 1 on each successfull save
            int delay = RestartDelay + 5;
            if (delay > 60)
                delay = 60;

            StartDelayedRestartTimer(delay);
        }

        private void StartDelayedRestartTimer(int delay)
        {
            StartInSeconds = RestartDelay = delay;
            Ticker += DelayedRestart;
        }

        private void StopDelayedRestartTimer()
        {
            Ticker -= DelayedRestart;
            StartInSeconds = RestartDelay;
        }

        private void UpdateBotTimer()
        {
            lblBotTimer.Text = chkAutoMode.Checked ? "Bot timer: " + IntTimer : "";
        }

        private void StopDelayedAutoSaveTimer()
        {
            Ticker -= DelayedAutoSave;
            IntTimer = 0;
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

            UpdateBotTimer();
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
                GenerateEditStatistics();
            }
        }

        int Seconds, LastEditsTotal, LastPagesTotal;
        private void GenerateEditStatistics()
        {
            NumberOfEditsPerMinute = (NumberOfEdits - LastEditsTotal); //Edits in the last minute
            NumberOfPagesPerMinute = (NumberOfEdits + NumberOfIgnoredEdits - LastPagesTotal);
            LastEditsTotal = NumberOfEdits;
            LastPagesTotal = NumberOfEdits + NumberOfIgnoredEdits;
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
            if (!TheSession.User.IsLoggedIn)
            {
                Profiles.ShowDialog();
                if (!TheSession.User.IsLoggedIn) return;
            }
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
            r.BypassRedirects(txtEdit.Text, TheSession.Editor.SynchronousEditor.Clone());
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

        private readonly Regex RegexDates = new Regex("[1-2][0-9]{3}", RegexOptions.Compiled);

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

        //TODO: Doesn't always stop
        private void Stop()
        {
            Retries = 0;
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

            TheSession.Editor.Abort();

            listMaker.Stop();

            if (AutoSaveEditBoxEnabled)
                EditBoxSaveTimer.Enabled = false;

            StatusLabelText = "Stopped";
            ClearBrowser();
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            HelpForm.ShowDialog();
        }

        #region Edit Box Menu
        private void reparseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReparseEditBox();
        }

        private void ReparseEditBox()
        {
            ArticleEX a = new ArticleEX(TheArticle.Name, txtEdit.Text);
            ArticleEX theArtricleOriginal = TheArticle;
            ErrorHandler.CurrentPage = TheArticle.Name;
            ProcessPage(a, false);
            ErrorHandler.CurrentPage = "";
            UpdateCurrentTypoStats();

            // provide article statistics based on new article, not the existing one
            TheArticle = a;
            ArticleInfo(false);
            TheArticle = theArtricleOriginal;

            txtEdit.Text = a.ArticleText;

            if (UnbalancedBracket >= 0 && scrollToUnbalancedBracketsToolStripMenuItem.Checked)
            {
                HighlightUnbalancedBrackets();
            }

            if (syntaxHighlightEditBoxToolStripMenuItem.Checked)
            {
                txtEdit.Visible = false;
                HighlightSyntax();
                txtEdit.Visible = true;
            }

            GetDiff();
        }

        private void replaceTextWithLastEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LastArticle.Length > 0)
                txtEdit.Text = LastArticle;
        }

        #region PasteMore
        private void PasteMore_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (sender as ToolStripMenuItem);

            if (item != null)
                txtEdit.SelectedText = (string)item.Tag;

            mnuTextBox.Hide();
        }

        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigurePasteMoreItems dlg = new ConfigurePasteMoreItems((string)PasteMore1.Tag,
                                                                      (string)PasteMore2.Tag,
                                                                      (string)PasteMore3.Tag,
                                                                      (string)PasteMore4.Tag,
                                                                      (string)PasteMore5.Tag,
                                                                      (string)PasteMore6.Tag,
                                                                      (string)PasteMore7.Tag,
                                                                      (string)PasteMore8.Tag,
                                                                      (string)PasteMore9.Tag,
                                                                      (string)PasteMore10.Tag);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string[] dlgStrings = new[] {
                    dlg.String1, dlg.String2, dlg.String3, dlg.String4, dlg.String5, dlg.String6, dlg.String7, dlg.String8, dlg.String9, dlg.String10, 
                };
                for (int i = 0; i < 10; ++i)
                    SetPasteMoreText(i, dlgStrings[i]);
            }
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

        private void summariesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SummaryEditor se = new SummaryEditor())
            {
                string[] summaries = new string[cmboEditSummary.Items.Count];
                cmboEditSummary.Items.CopyTo(summaries, 0);
                se.Summaries.Lines = summaries;
                se.Summaries.Select(0, 0);

                string prevSummary = cmboEditSummary.SelectedText;

                if (se.ShowDialog(this) != DialogResult.OK)
                    return;

                cmboEditSummary.Items.Clear();

                foreach (string s in se.Summaries.Lines)
                    if (!string.IsNullOrEmpty(s.Trim()))
                        cmboEditSummary.Items.Add(s.Trim());

                if (cmboEditSummary.Items.Contains(prevSummary))
                    cmboEditSummary.SelectedText = prevSummary;
                else
                    cmboEditSummary.SelectedItem = 0;
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
                webBrowser.Location = new Point(webBrowser.Location.X, 48);
                if (panel1.Visible)
                    webBrowser.Height = panel1.Location.Y - 48;
                else
                    webBrowser.Height = StatusMain.Location.Y - 48;
            }
            else
            {
                webBrowser.Location = new Point(webBrowser.Location.X, 25);
                if (panel1.Visible)
                    webBrowser.Height = panel1.Location.Y - 25;
                else
                    webBrowser.Height = StatusMain.Location.Y - 25;
            }
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
        private void MoveArticle()
        {
            try
            {
                if (!TheSession.User.CanMovePage(TheSession.Page))
                {
                    MessageBox.Show(
                        "Current user doesn't have enough rights to move \"" + TheSession.Page.Title + "\"",
                        "User rights not sufficient");
                    return;
                }

                TheArticle.Move(TheSession);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        private void DeleteArticle()
        {
            try
            {
                if (TheArticle.Delete(TheSession))
                {
                    listMaker.Remove(TheArticle);
                    Start();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        private void ProtectArticle()
        {
            try
            {
                TheArticle.Protect(TheSession);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }
        #endregion

        private void btnSubst_Click(object sender, EventArgs e)
        {
            SubstTemplates.ShowDialog();
        }

        private void launchRegexTester(object sender, EventArgs e)
        {
            RegexTester = new RegexTester();

            if (txtEdit.SelectionLength > 0 && MessageBox.Show("Would you like to transfer the currently selected Article Text to the Regex Tester?", "Transfer Article Text?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                RegexTester.ArticleText = txtEdit.SelectedText;

            RegexTester.Show();
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
                foreach (KeyValuePair<string, IAWBPlugin> a in Plugin.AWBPlugins)
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
                foreach (KeyValuePair<string, IAWBPlugin> a in Plugin.AWBPlugins)
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
                    text = TheSession.Editor.SynchronousEditor.Open("Project:AutoWikiBrowser/User talk templates");
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

        #region History
        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TheArticle == null)
                return;

            if (EditBoxTab.SelectedTab == tpHistory)
                NewHistory(TheArticle.Name);
            else if (EditBoxTab.SelectedTab == tpLinks)
                NewWhatLinksHere(TheArticle.Name);
        }

        private void NewHistory(string pageTitle)
        {
            try
            {
                if (EditBoxTab.SelectedTab == tpHistory && pageTitle != null)
                {
                    string name = HttpUtility.UrlEncode(pageTitle);
                    if (webBrowserHistory.Url != new Uri(Variables.URLIndex + "?title=" + name
                        + "&action=history&useskin=myskin") && !string.IsNullOrEmpty(pageTitle)
                        )
                        webBrowserHistory.Navigate(Variables.URLIndex + "?title=" + name 
                            + "&action=history&useskin=myskin");
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

        private void NewWhatLinksHere(string title)
        {
            try
            {
                if (EditBoxTab.SelectedTab == tpLinks && title != null)
                {
                    title = HttpUtility.UrlEncode(title);
                    if (webBrowserLinks.Url !=
                        new Uri(Variables.URLIndex + "?title=Special:WhatLinksHere/" + title +
                                "&useskin=myskin") && !string.IsNullOrEmpty(title))
                        webBrowserLinks.Navigate(Variables.URLIndex + "?title=Special:WhatLinksHere/" +
                                                 title + "&useskin=myskin");
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

            linksHtml = linksHtml.Replace("<A ", "<a target=\"_blank\" ");
            linksHtml = linksHtml.Replace("<FORM ", "<form target=\"_blank\" ");
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

        private void ProfileLoggedIn(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Profiles.SettingsToLoad))
                LoadPrefs(Profiles.SettingsToLoad);

            CheckStatus(true);

            UpdateStatusUI();

            StopProgressBar();
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
                ShutdownNotification shut = new ShutdownNotification { ShutdownType = GetShutdownType() };

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
            string redirect = Variables.MagicWords["redirect"][0].ToUpper();
            EditToolBarAction(redirect + " [[Insert text]]", 13, 11, redirect + " [[", "]]");
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
        /// Applies EditToolBar button action
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
            bool watch = (btnWatch.Text == "Watch");

            if (watch)
                TheSession.Editor.Watch(TheArticle.Name);
            else
                TheSession.Editor.Unwatch(TheArticle.Name);

            SetWatchButton(watch);
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

        private readonly CategoryNameForm CatName = new CategoryNameForm();

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

        private void BotImage_Click(object sender, EventArgs e)
        {
            Tools.OpenURLInBrowser("http://commons.wikimedia.org/wiki/Image:Crystal_Clear_action_run.png");
        }

        private void displayfalsePositivesButtonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddIgnoredToLogFile = displayfalsePositivesButtonToolStripMenuItem.Checked;
        }

        private void HighlightUnbalancedBrackets()
        {
            // indexes in articleText and txtEdit.Edit are offset by the number of newlines before the index of the unbalanced brackets
            // so allow for this when highlighting the unbalanced bracket
            string a = txtEdit.Text.Substring(0, UnbalancedBracket);
            int b = Regex.Matches(a, "\n").Count;
            txtEdit.SetEditBoxSelection(UnbalancedBracket - b, BracketLength);
            txtEdit.SelectionBackColor = Color.Red;
        }

        private void txtSkipIfContains_TextChanged(object sender, EventArgs e)
        {
            InvalidateSkipChecks();
        }

        private void txtSkipIfNotContains_TextChanged(object sender, EventArgs e)
        {
            InvalidateSkipChecks();
        }

        private void chkSkipIsRegex_CheckedChanged(object sender, EventArgs e)
        {
            InvalidateSkipChecks();
        }

        private void chkSkipCaseSensitive_CheckedChanged(object sender, EventArgs e)
        {
            InvalidateSkipChecks();
        }

        private void toolStripComboOnLoad_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolStripDiffInBotMode.Enabled = (toolStripComboOnLoad.SelectedIndex == 0);
        }
    }
        #endregion
}
