﻿/*
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
using System.Configuration;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Security.Permissions;
using System.Web;
using System.Net;
using System.Linq;
using WikiFunctions;
using WikiFunctions.API;
using WikiFunctions.Lists.Providers;
using WikiFunctions.Plugin;
using WikiFunctions.Parse;
using WikiFunctions.Properties;
using WikiFunctions.Controls;
using WikiFunctions.Background;
using WikiFunctions.Controls.Lists;
using AutoWikiBrowser.Plugins;
using ThreadState = System.Threading.ThreadState;

namespace AutoWikiBrowser
{
    // TODO:Move any code that doesn't need to be directly behind the form to WF or other code files (Preferably WF)
    // TODO:Move regexes declared in method bodies (if not dynamic based on article title, etc), into class body
    // TODO:Move any Regexes to WikiRegexes as required

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public sealed partial class MainForm : Form, IAutoWikiBrowser
    { // this class needs to be public, otherwise we get an exception which recommends setting ComVisibleAttribute to true (which we've already done)
        #region Fields
        private readonly Splash SplashScreen = new Splash();
        private readonly WikiFunctions.Profiles.AWBProfilesForm Profiles;

        private bool Abort, IgnoreNoBots, ClearPageListOnProjectChange, PageReload, doDiffInBotMode, loggingEnabled;

        private string LastArticle = "", mSettingsFile = "";

        private const int MaxRetries = 10;

        private int OldSelection, Retries, SameArticleNudges, actionOnLoad;

        private readonly HideText RemoveText = new HideText(false, true, true);
        private readonly List<string> NoParse = new List<string>();
        private readonly List<string> NoRetf = new List<string>();
        private readonly FindandReplace FindAndReplace = new FindandReplace();
        private readonly SubstTemplates SubstTemplates = new SubstTemplates();
        private RegExTypoFix RegexTypos;
        private readonly SkipOptions Skip = new SkipOptions();
        private readonly WikiFunctions.ReplaceSpecial.ReplaceSpecial RplcSpecial =
            new WikiFunctions.ReplaceSpecial.ReplaceSpecial();
        private readonly Parsers Parser;
        private readonly TimeSpan StartTime = new TimeSpan(DateTime.Now.Ticks);
        private readonly List<string> RecentList = new List<string>();
        private readonly CustomModule CModule = new CustomModule();
        private readonly ExternalProgram ExtProgram = new ExternalProgram();
        private RegexTester RegexTester;
        private bool UserTalkWarningsLoaded;
        private bool TemplateRedirectsLoaded;
        private bool DatedTemplatesLoaded;
        private bool RenamedTemplateParametersLoaded;
        private Regex UserTalkTemplatesRegex;
        private bool Skippable = true;
        private FormWindowState LastState = FormWindowState.Normal; // doesn't look like we can use RestoreBounds for this - any other built in way?

        private ArticleRedirected ArticleWasRedirected;

        private ListComparer Comparer;
        private ListSplitter Splitter;
        private WikiFunctions.DBScanner.DatabaseScanner DBScanner;

        private List<TypoStat> TypoStats;

        private readonly WikiDiff Diff = new WikiDiff();
        private readonly JsAdapter DiffScriptingAdapter;

        /// <summary>
        /// Whether AWB is currently shutting down
        /// </summary>
        private bool ShuttingDown { get; set; }

        private readonly ToolStripMenuItem[] _pasteMoreItems;
        private readonly string[] _pasteMoreItemsPrefixes = {
            "&1. ", "&2. ", "&3. ", "&4. ", "&5. ", "&6. ", "&7. ", "&8. ", "&9. ", "1&0. "
        };
        #endregion

        public Session TheSession
        { get; private set; }

        #region Constructor and MainForm load/resize
        public MainForm()
        {
            CheckSettings();

            DiffScriptingAdapter = new JsAdapter(this);

            Updater.UpdateUpdaterFile();

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
                    Parser = new Parsers(500, false);
                }
                catch (Exception ex)
                {
                    ErrorHandler.HandleException(ex);
                }

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
                Profiles.UserDefaultSettingsLoadRequired += UserDefaultSettingsLoadRequired;

                SplashScreen.SetProgress(15);

                _pasteMoreItems = new[]
                {
                    PasteMore1, PasteMore2, PasteMore3, PasteMore4, PasteMore5, PasteMore6, PasteMore7, PasteMore8, PasteMore9, PasteMore10
                };

                // to avoid saving to app data
                saveXML.InitialDirectory = openXML.InitialDirectory =
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// From https://stackoverflow.com/questions/2269489/c-sharp-user-settings-broken
        /// </remarks>
        public static void CheckSettings()
        {
            try
            {
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            }
            catch (ConfigurationErrorsException ex)
            {
                string filename = string.Empty;
                if (!string.IsNullOrEmpty(ex.Filename))
                {
                    filename = ex.Filename;
                }
                else
                {
                    var innerEx = ex.InnerException as ConfigurationErrorsException;
                    if (innerEx != null && !string.IsNullOrEmpty(innerEx.Filename))
                    {
                        filename = innerEx.Filename;
                    }
                }

                if (!string.IsNullOrEmpty(filename))
                {
                    if (File.Exists(filename))
                    {
                        var fileInfo = new FileInfo(filename);
                        var watcher
                             = new FileSystemWatcher(fileInfo.Directory.FullName, fileInfo.Name);
                        Tools.WriteDebug(string.Format("Deleting corrupt file {0}", filename), ex.Message);
                        File.Delete(filename);
                        if (File.Exists(filename))
                        {
                            watcher.WaitForChanged(WatcherChangeTypes.Deleted);
                        }
                    }
                }
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
                            _profileToLoad = args[i + 1];
                        break;
                }
            }
        }

        private string _settingsFileDisplay;
        string SettingsFile
        {
            set
            {
                mSettingsFile = value;
                _settingsFileDisplay = Program.Name;

                #if DEBUG
                if (Variables.RevisionNumber > 0)
                    _settingsFileDisplay += " rev " + Variables.RevisionNumber;
                #endif

                if (!string.IsNullOrEmpty(value))
                    _settingsFileDisplay += " – " + Path.GetFileName(value);
                Text = _settingsFileDisplay;

                ntfyTray.Text = (_settingsFileDisplay.Length >= 64) ? _settingsFileDisplay.Substring(0, 62) : _settingsFileDisplay;
            }
            get { return mSettingsFile; }
        }

        string _profileToLoad = "";

        private void MainForm_Load(object sender, EventArgs e)
        {
            EditBoxTab.TabPages.Remove(tpTypos);

            StatusLabelText = "Initialising...";
            SplashScreen.SetProgress(20);
            Variables.MainForm = this;
            lblOnlyBots.BringToFront();

            SplashScreen.SetProgress(22);

            try
            {
                // check that we are not using an old OS. 98 seems to mangled some unicode
                if (Environment.OSVersion.Version.Major < 5 && Globals.RunningOnWindows)
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
                articleActionLogControl1.Initialise(listMaker);

                Location = Properties.Settings.Default.WindowLocation;

                Size = Properties.Settings.Default.WindowSize;

                // T99305: No vertical scroll bar in diff window: don't restore AWB Minimized as we lose scrollbars
                WindowState = (Properties.Settings.Default.WindowState == FormWindowState.Minimized ? FormWindowState.Normal : Properties.Settings.Default.WindowState);

                Plugin.LoadPluginsStartup(this, SplashScreen); // progress 25-50 in LoadPlugins()
                LoadPrefs(); // progress 50-59 in LoadPrefs()

                Debug();
                Release();

                SplashScreen.SetProgress(60);
                UpdateButtons(null, null);
                SplashScreen.SetProgress(62);
                LoadRecentSettingsList(); // progress 63-66 in LoadRecentSettingsList()

                Updater.WaitForCompletion();

                ntfyTray.Visible = true;

                SplashScreen.SetProgress(80);

                if ((Updater.Result & Updater.AWBEnabledStatus.Disabled) == Updater.AWBEnabledStatus.Disabled)
                {
                    OldVersion();
                    SplashScreen.Close();
                    return;
                }

                bool optUpdate = (Updater.Result & Updater.AWBEnabledStatus.OptionalUpdate) ==
                                  Updater.AWBEnabledStatus.OptionalUpdate,
                updaterUpdate = (Updater.Result & Updater.AWBEnabledStatus.UpdaterUpdate) ==
                                 Updater.AWBEnabledStatus.UpdaterUpdate;

                if (optUpdate || updaterUpdate)
                {
                    bool runUpdater = false;

                    if (updaterUpdate)
                    {
                        MessageBox.Show("There is an update to the AWB updater. Updating Now", "Updater update");
                        runUpdater = true;
                    }

                    if (!runUpdater &&
                        MessageBox.Show(
                            string.Format(
                                "This version has been superceeded by new versions of AWB: {0}.\r\n\r\nYou may continue to use this version or update to the newest version.\r\n\r\nWould you like to automatically upgrade to the newest version?",
                                string.Join(", ", Updater.NewerVersions)
                               ),
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

                if ((Updater.Result & Updater.AWBEnabledStatus.Error) == Updater.AWBEnabledStatus.Error)
                {
                    lblUserName.BackColor = Color.Red;
                    MessageBox.Show(this,
                                    "Cannot load version check page from Wikipedia. Please verify that you're connected to Internet.",
                                    "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                SplashScreen.SetProgress(90);

                Profiles.Login(_profileToLoad);

                SplashScreen.SetProgress(95);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
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
                if (_minimize) Visible = false;
            }
            else
                LastState = WindowState; // remember if maximised or normal so can restore same when dbl click tray icon
        }
        #endregion

        #region Properties

        internal Article TheArticle { get; private set; }

        /// <summary>
        /// Is AWB running in Bot Mode
        /// </summary>
        private bool BotMode
        {
            get { return chkAutoMode.Checked; }
            set { chkAutoMode.Checked = value; }
        }

        private bool _lowThreadPriority;
        private bool LowThreadPriority
        {
            get { return _lowThreadPriority; }
            set
            {
                _lowThreadPriority = value;
                Thread.CurrentThread.Priority = value ? ThreadPriority.Lowest : ThreadPriority.Normal;
            }
        }

        private int _listComparerUseCurrentArticleList, _listSplitterUseCurrentArticleList, _dbScannerUseCurrentArticleList;

        private bool _flash, _beep;

        /// <summary>
        /// True if user has been warned in AWB session that articles with characters in Unicode private use area can't be saved
        /// </summary>
        private bool _userWarnedAboutUnicodePUA;

        /// <summary>
        /// True if AWB should be minimised to the system tray; False if it should minimise to the taskbar
        /// </summary>
        private bool _minimize;

        private bool _saveArticleList = true;
        private bool _autoSaveEditBoxEnabled;

        private string _autoSaveEditBoxFile = Path.Combine(Application.StartupPath, "Edit Box.txt");

        private bool _suppressUsingAWB;

        private decimal _asEditPeriod = 60;
        private decimal AutoSaveEditBoxPeriod
        {
            get { return _asEditPeriod; }
            set { _asEditPeriod = value; EditBoxSaveTimer.Interval = int.Parse((value * 1000).ToString(CultureInfo.InvariantCulture)); }
        }

        private string StatusLabelText
        {
            get { return lblStatusText.Text; }
            set
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => { StatusLabelText = value; }));
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
                btnStop.Location = value ? new Point(220, 62) : new Point(156, 62);
                btnStop.Size = value ? new Size(51, 23) : new Size(117, 23);

                btnFalsePositive.Visible = btntsFalsePositive.Visible = value;
            }
        }

        bool _timerShown = true;
        private bool ShowMovingAverageTimer
        {
            set
            {
                _timerShown = value;
                ShowTimer();
            }
            get { return _timerShown; }
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
                HandleLogoff();
                return false;
            }
            return true;
        }

        /// <summary>
        /// Displays You've been logged off message, opens profiles screen if not already open
        /// </summary>
        private void HandleLogoff()
        {
            MessageBox.Show("You've been logged off, probably due to loss of session data.\r\nPlease relogin.",
                            "Logged off", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            Stop();

            if (!Profiles.Visible)
                Profiles.ShowDialog(this);
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateEditor()
        {
            TheSession.PreviewComplete += PreviewComplete;
            TheSession.ExceptionCaught += ApiEditExceptionCaught;
            TheSession.SaveComplete += PageSaved;
            TheSession.MaxlagExceeded += MaxlagExceeded;
            TheSession.OpenComplete += OpenComplete;
            TheSession.LoggedOff += LoggedOff;
        }

        private void LoggedOff(AsyncApiEdit sender)
        {
            DisableButtons();
        }

        private void OpenComplete(AsyncApiEdit editor, PageInfo page)
        {
            PageLoaded(page);
        }

        private void MaxlagExceeded(AsyncApiEdit sender, double maxlag, int retryAfter)
        {
            Retries++;

            if (Retries < MaxRetries)
            {
                StartDelayedRestartTimer(retryAfter);
            }
            else
            {
                Stop();
                MessageBox.Show(this, "Maxlag exceeded " + MaxRetries + " times in a row. Processing stopped, please try later when the server is under a less load.", "Stopped", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void ApiEditExceptionCaught(AsyncApiEdit sender, Exception ex)
        {
            if (ex is InterwikiException)
            {
                SkipPage(ex.Message);
            }
            else if (ex is SpamlistException)
            {
                string message = "Text '" + (ex as SpamlistException).URL + "' is blocked by SpamBlacklist extension";

                if (!BotMode)
                {
                    if (!chkSkipSpamFilter.Checked
                        && MessageBox.Show(message + ".\r\nTry and edit again?",
                                           "Spam Blacklist", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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
                        // TODO: must be a less crude way
                        MessageBox.Show(this,
                            "There has been an edit conflict. AWB will now re-apply its changes on the updated page.\r\nPlease re-review the changes before saving. Any Custom edits will be lost, and have to be re-added manually.",
                            "Edit conflict");
                        NudgeTimer.Stop();
                        Start();
                        break;

                    case "writeapidenied":
                        NoWriteApiRight();
                        break;

                    case "customcssprotected":
                        SkipPage("You're not allowed to edit custom CSS pages");
                        break;

                    case "customjsprotected":
                        SkipPage("You're not allowed to edit custom JavaScript pages");
                        break;

                    case "badmd5":
                        SkipPage(
                            "API MD5 hash error: The page you are editing may contain an unsupported or invalid Unicode character");
                        break;

                    case "assertuserfailed":
                        HandleLogoff();
                        break;

                    case "badtoken":
                        // likely session timeout forced by mediawiki, so just reprocess page
                        Tools.WriteDebug("ApiExceptionCaught/badtoken", ex.Message);
                        StartDelayedRestartTimer();
                        break;

                    case "blocked":
                        MessageBox.Show("API reports this user is blocked from editing.",
                            "User blocked", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        Stop();
                        break;

                    case "readonly":
                        MessageBox.Show(((ApiErrorException)ex).ApiErrorMessage,
                            "Wiki read-only", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        StartDelayedRestartTimer();
                        break;

                    case "tpt-target-page":
                        SkipPage("Translation pages cannot currently be edited");
                        break;
                        
                    case "titleblacklist-forbidden-edit":
                        SkipPage("TitleBlacklist prevents this title from being created");
                        break;

                    default:
                        Tools.WriteDebug("ApiExceptionCaught", ex.Message);
                        StartDelayedRestartTimer();
                        break;
                }
            }
            else if (ex is ApiBlankException)
            {
                Tools.WriteDebug("ApiBlankException", ex.Message);
                StartDelayedRestartTimer();
            }
            else if (ex is NewMessagesException)
            {
                WeHaveNewMessages();
            }
            else if (ex is LoggedOffException)
            {
                HandleLogoff();
            }
            else if (ex is CaptchaException)
            {
                MessageBox.Show("Captcha required, is the user account autoconfirmed etc?", "Captcha Required");
                Stop();
            }
            else if (ex is InvalidTitleException)
            {
                SkipPage("Invalid title");
            }
            else if (ex is RedirectToSpecialPageException)
            {
                SkipPage("Page is a redirect to a special page");
            }
            else if (ex is WebException || (ex is IOException && ex.Message.Contains("0x2746")))
            {
                // some 404 error or similar, or "Unable to write data to the transport connection: Unknown error (0x2746)"
                StatusLabelText = ex.Message;
                if (Tools.WriteDebugEnabled)
                    Tools.WriteTextFile(ex.Message, "Log.txt", true);
                StartDelayedRestartTimer();
            }
            else if (ex is SharedRepoException)
            {
                MessageBox.Show("Cannot move this file to the specified target, as it exists in a shared repo (such as commons).", "Target file exists in shared repo");
            }
            else if (ex is MediaWikiSaysNoException)
            {
                MessageBox.Show("MediaWiki prevented you from making that edit. Chances are it's spam or abuse filter related", "MediaWiki says no");
                SkipPage("Edit blocked by spam/abuse filter");
            }
            else
            {
                Stop();
                ErrorHandler.HandleException(ex);
            }
        }

        private void OpenPage(string title)
        {
            StatusLabelText = "Loading...";
            // if "skip if redirect" is on, don't bypass redirects even if "bypass redirects" is on too
            TheSession.Editor.Open(title, followRedirectsToolStripMenuItem.Checked && !chkSkipIfRedirect.Checked);
        }

        private bool _stopProcessing, _inStart, _startAgain;

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            if (TheSession.Status != WikiStatusResult.Registered || TheSession.IsBusy)
                return;
            if (_inStart)
            {
                _startAgain = true;
                return;
            }
            _inStart = true;
            do
            {
                _startAgain = false;
                StartArticleProcessing();
            } while (_startAgain);
            _inStart = false;
        }

        /// <summary>
        /// </summary>
        /// <returns>true if it is ok to call again, or false if processing should now stop</returns>
        private void StartArticleProcessing()
        {
            if (_stopProcessing)
                return;

            try
            {
                Tools.WriteDebug(Name, "Starting");

                Shutdown();

                // Check edit summary
                txtEdit.Enabled = txtReviewEditSummary.Enabled = true;
                SetEditToolBarEnabled(true);

                if (Variables.Project != ProjectEnum.custom && string.IsNullOrEmpty(cmboEditSummary.Text) &&
                    !Plugin.AWBPlugins.Any())
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
                    StopProgressBar();
                    StatusLabelText = "No articles in list, you need to use the Make list";
                    Text = _settingsFileDisplay;
                    listMaker.MakeListEnabled = true;
                    return;
                }

                string title = listMaker.SelectedArticle().Name;

                if (!Tools.IsValidTitle(title))
                {
                    // create TheArticle else skip won't work
                    TheArticle = new Article(title, "");
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

                // reset last article text if now processing a different article
                if (TheArticle != null && !TheArticle.Name.Equals(title))
                    LastArticle = "";

                TheArticle = new Article(title, "");

                // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_12#.27Find.27_sometimes_fails_to_use_the_search_key
                txtEdit.ResetFind();

                NewHistory(title);
                NewWhatLinksHere(title);

                EditBoxSaveTimer.Enabled = _autoSaveEditBoxEnabled;

                // if (dlg != null && dlg.AutoProtectAll)
                //    TheArticle.Protect(TheSession);

                StartProgressBar();

                // Navigate to edit page
                OpenPage(title);
            }
            catch (Exception ex)
            {
                Tools.WriteDebug(Name, "Start() error: " + ex.Message);
                StartDelayedRestartTimer();
            }
        }

        private Dictionary<int, int> unbalancedBracket = new Dictionary<int, int>();
        private Dictionary<int, int> badCiteParameters = new Dictionary<int, int>();
        private Dictionary<int, int> dupeBanerShellParameters = new Dictionary<int, int>();
        private Dictionary<int, int> unclosedTags = new Dictionary<int, int>();
        private Dictionary<int, int> wikilinkedHeaders = new Dictionary<int, int>();
        private Dictionary<int, int> deadLinks = new Dictionary<int, int>();
        private Dictionary<int, int> ambigCiteDates = new Dictionary<int, int>();
        private Dictionary<int, int> targetlessLinks = new Dictionary<int, int>();
        private Dictionary<int, int> doublepipeLinks = new Dictionary<int, int>();
        private Dictionary<int, int> otherErrors = new Dictionary<int, int>();
        private Dictionary<int, int> userSignature = new Dictionary<int, int>();
        private List<string> UnknownWikiProjectBannerShellParameters = new List<string>();
        private List<string> UnknownMultipleIssuesParameters = new List<string>();

        private readonly SortedDictionary<int, int> Errors = new SortedDictionary<int, int>();

        private void SkipRedirect(string reason)
        {
            // if we didn't do this, we were writing the SkipPage info to the AWBLogListener belonging to the object redirect
            // and resident in the MyTrace collection, but then attempting to add TheArticle's log listener to the logging tab
            SkipPage(reason);
        }

        private static readonly Regex UnicodePUA = new Regex(@"\p{IsPrivateUse}", RegexOptions.Compiled);
        private BackgroundRequest RunProcessPageBackground;

        /// <summary>
        /// Invoked on successful page load, performs skip checks and calls main page processing
        /// </summary>
        /// <param name="page"></param>
        private void PageLoaded(PageInfo page)
        {
            if (!LoadSuccessApi())
                return;

            Retries = 0;

            if (_stopProcessing)
                return;

            if (Namespace.IsSpecial(Namespace.Determine(page.Title)))
            {
                SkipPage("Page is a special page");
            }

            TheArticle = new Article(page);

            if (!preParseModeToolStripMenuItem.Checked && !CheckLoginStatus())
                return;

            if (Program.MyTrace.HaveOpenFile)
                Program.MyTrace.WriteBulletedLine("AWB started processing", true, true, true);
            else
                Program.MyTrace.Initialise();

            Text = _settingsFileDisplay + " – " + page.Title;

            bool pageIsRedirect = PageInfo.WasRedirected(page);

            // check for redirects when 'follow redirects' is off
            if (chkSkipIfRedirect.Checked && Tools.IsRedirect(page.Text))
            {
                SkipPage("Page is a redirect");
                return;
            }

            // check for redirect
            if (followRedirectsToolStripMenuItem.Checked && pageIsRedirect && !PageReload)
            {
                if ((page.TitleChangedStatus & PageTitleStatus.RedirectLoop) == PageTitleStatus.RedirectLoop)
                {
                    // ignore recursive redirects
                    SkipRedirect("Recursive redirect");
                    return;
                }

                // No double redirects, API should've resolved it

                if (filterOutNonMainSpaceToolStripMenuItem.Checked
                    && (Namespace.Determine(page.Title) != Namespace.Article))
                {
                    SkipRedirect("Page redirects to non-mainspace");
                    return;
                }

                if (ArticleWasRedirected != null)
                    ArticleWasRedirected(page.OriginalTitle, page.Title);

                listMaker.ReplaceArticle(new Article(page.OriginalTitle), TheArticle);
            }

            /* check for normalized page: since we canonicalize title before API read is requested,
             this shouldn't normally be the case: will be the case for female user page normalization
             example https://de.wikipedia.org/w/api.php?action=query&prop=info|revisions&titles=Benutzer%20Diskussion:MarianneBirkholz&rvprop=timestamp|user|comment|content
             see the <normalized> block */
            if (page.TitleChangedStatus == PageTitleStatus.Normalised)
            {
                listMaker.ReplaceArticle(new Article(page.OriginalTitle), TheArticle);
            }

            ErrorHandler.CurrentRevision = page.RevisionID;

            if (PageReload)
            {
                PageReload = false;
                GetDiff();
                return;
            }

            // pre-processing of article
            if (SkipChecks(!skipIfContains.After, !skipIfNotContains.After))
            {
                return;
            }

            // check not in use
            if (TheArticle.IsInUse)
            {
                if (chkSkipIfInuse.Checked)
                {
                    SkipPage("Page contains {{inuse}}");
                    return;
                }
                if (!BotMode && !preParseModeToolStripMenuItem.Checked)
                {
                    MessageBox.Show("This page has the \"Inuse\" tag, consider skipping it");
                }
            }

            /* skip pages containing any Unicode character in Private Use Area as RichTextBox seems to break these
             * not exactly wrong as PUA characters won't be found in standard text, but not exactly right to break them either
             * Reference: [[Unicode#Character General Category]] PUA is U+E000 to U+F8FF */
            Match uPUA = UnicodePUA.Match(page.Text);
            if (uPUA.Success)
            {
                if (!_userWarnedAboutUnicodePUA && !preParseModeToolStripMenuItem.Checked && !BotMode)
                {
                    // provide text around PUA character so user can find it
                    string uPUAText = page.Text.Substring(uPUA.Index-Math.Min(25, uPUA.Index), Math.Min(25, uPUA.Index) + Math.Min(25, page.Text.Length-uPUA.Index));
                        MessageBox.Show("This page has character(s) in the Unicode Private Use Area so unfortunately can't be edited with AWB. The page will now be skipped. Surrounding text of first PUA character is: " + uPUAText);
                    _userWarnedAboutUnicodePUA = true;
                }

                SkipPage("Page has character in Unicode Private Use Area");
                return;
            }

            // run process page in a background thread
            // use .Complete event to do rest of processing (skip checks, alerts etc.) once thread finished
            if (automaticallyDoAnythingToolStripMenuItem.Checked)
            {
                RunProcessPageBackground = new BackgroundRequest();
                RunProcessPageBackground.Execute(ProcessPageBackground);
                RunProcessPageBackground.Complete += AutomaticallyDoAnythingComplete;
                return;
            }
            CompleteProcessPage();
        }

        /// <summary>
        /// Runs ProcessPage background thread
        /// </summary>
        private void ProcessPageBackground()
        {
            StatusLabelText = (preParseModeToolStripMenuItem.Checked ? "Processing page (pre-parse mode)": "Processing page");
            Application.DoEvents();

            // FIXME: this position is imprefect, since above there is code that can explode, but this way
            // at least we don't get bogus reports of unrelated pages
            ErrorHandler.CurrentPage = TheArticle.Name;

            ProcessPage(TheArticle, true);
        }

        /// <summary>
        /// Carries out skip checks following ProcessPage
        /// Calls CompleteProcessPage if page not skipped
        /// </summary>
        private void RunSkipChecks()
        {
            ErrorHandler.CurrentPage = "";

            UpdateCurrentTypoStats();

            if (!Abort)
            {
                if (TheArticle.SkipArticle)
                {
                    SkipPageReasonAlreadyProvided(); // Don't send a reason; ProcessPage() should already have logged one
                    return;
                }

                if (Skippable)
                {
                    if ((chkSkipNoChanges.Checked || BotMode) && TheArticle.NoArticleTextChanged)
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

                    if (chkSkipCosmetic.Checked
                        && (TheArticle.NoArticleTextChanged || TheArticle.OnlyCosmeticChanged))
                    {
                        SkipPage("Only cosmetic changes made");
                        return;
                    }

                }

                Variables.Profiler.Profile("Skip checks");

                // post-processing
                if (SkipChecks(skipIfContains.After, skipIfNotContains.After))
                {
                    return;
                }

                CompleteProcessPage();
            }
        }
        
        /// <summary>
        /// Event that fires when ProcessPage background thread finishes
        /// </summary>
        /// <param name="req"></param>
        private void AutomaticallyDoAnythingComplete(BackgroundRequest req)
        {
            // must use Invoke so that SkipChecks and CompleteProcessPage are done on main GUI thread
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(RunSkipChecks));
                return;
            }
            RunSkipChecks();
        }

        /// <summary>
        /// Runs after ProcessPage to set edit box text, set alerts, call diff/preview, highlight errors, highlight syntax, 
        /// enable buttons
        /// </summary>
        private void CompleteProcessPage()
        {
            Tools.WriteDebug("PageLoaded", "TheArticle.ArticleText length is " + TheArticle.ArticleText.Length);
            
            // For some reason can get very poor performance setting txtEdit.Text (up to 30 seconds on longest en-wiki articles) after processing a few pages
            // Toggling word wrap seems to solve this, not sure why
            txtEdit.WordWrap = !txtEdit.WordWrap;
            txtEdit.WordWrap = !txtEdit.WordWrap;
            txtEdit.Text = TheArticle.ArticleText;

            Variables.Profiler.Profile("Set edit box text");

            // Update statistics and alerts
            if (!BotMode)
                ArticleInfo(false);

            if (chkSkipIfNoAlerts.Checked && lbAlerts.Items.Count == 0)
            {
                SkipPage("Page has no alerts");
                return;
            }

            Variables.Profiler.Profile("Alerts");

            if (preParseModeToolStripMenuItem.Checked)
            {
                // if we reach here the article has valid changes, so move on to next article

                // if user has loaded a settings file, save it every 10 ignored edits
                if (autoSaveSettingsToolStripMenuItem.Checked && !string.IsNullOrEmpty(SettingsFile) && (NumberOfIgnoredEdits > 5) && (NumberOfIgnoredEdits % 10 == 0))
                    SavePrefs(SettingsFile);

                // request list maker to focus next article in list; if there is a next article process it, otherwise pre-parsing has finished, save settings
                // but don't save when settings have just been saved by logic above
                if (listMaker.NextArticle())
                    Start();
                else
                {
                    Stop();
                    if (autoSaveSettingsToolStripMenuItem.Checked && (NumberOfIgnoredEdits % 10 != 0) && !string.IsNullOrEmpty(SettingsFile))
                        SavePrefs(SettingsFile);
                }

                NumberOfPagesParsed++;
                return;
            }

            if (syntaxHighlightEditBoxToolStripMenuItem.Checked)
                txtEdit.Visible = false;

            if (!Abort)
            {
                UpdateUserNotifications();
                bool diffInBotMode = (BotMode && doDiffInBotMode);
                if (BotMode)
                {
                    // if user pressed Stop while background thread was running, don't overwrite status or go into bot loop
                    if (StatusLabelText != "Stopped")
                    {
                        StatusLabelText = "Ready to save";
                        StartDelayedAutoSaveTimer();
                    }

                    if (!diffInBotMode)
                    {
                        txtReviewEditSummary.Text = MakeDefaultEditSummary();
                        return;
                    }
                }
                else lblBotTimer.Text = ""; // remove for situation where bot mode used then turned off

                switch (actionOnLoad)
                {
                    case 0:
                        GetDiff();

                        if (diffInBotMode)
                        {
                            txtReviewEditSummary.Text = MakeDefaultEditSummary();
                            StopProgressBar();
                            return;
                        }

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

                PageWatched = TheSession.Page.IsWatched;

                Variables.Profiler.Profile("ActionOnLoad");

                txtReviewEditSummary.Text = MakeDefaultEditSummary();

                Variables.Profiler.Profile("Make Edit summary");

                txtEdit.Visible = false;
                // syntax highlighting of edit box based on m:extension:wikEd standards
                if (syntaxHighlightEditBoxToolStripMenuItem.Checked)
                {
                    HighlightSyntax();
                    Variables.Profiler.Profile("Syntax highlighting");

                    if (!focusAtEndOfEditTextBoxToolStripMenuItem.Checked)
                    {
                        txtEdit.SetEditBoxSelection(0, 0);
                        txtEdit.Select(0, 0);
                        txtEdit.ScrollToCaret();
                    }
                }

                if (highlightAllFindToolStripMenuItem.Checked)
                    HighlightAllFind();
                
                // always clear errors in case highlight errors was previously enabled and now turned off by user
                Errors.Clear();

                if (scrollToAlertsToolStripMenuItem.Checked)
                {
                    EditBoxTab.SelectedTab = tpEdit;
                    HighlightErrors();
                }
                // performance: only make text visible once highlighting complete
                txtEdit.Visible = true;
                Variables.Profiler.Profile("Find/alert highlighting");

                if (focusAtEndOfEditTextBoxToolStripMenuItem.Checked)
                {
                    txtEdit.Select(txtEdit.Text.Length, 0);
                    txtEdit.ScrollToCaret();
                }

                btnSave.Select();

                // if user pressed Stop while background thread was running, don't overwrite status here
                if (StatusLabelText != "Stopped")
                    StatusLabelText = "Ready to save";
                StopProgressBar();
            }
            else
            {
                EnableButtons();
                Abort = false;
            }
        }

        private void HighlightErrors()
        {
            foreach (KeyValuePair<int, int> kvp in unbalancedBracket.Where(kvp => !Errors.ContainsKey(kvp.Key)))
            {
                Errors.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<int, int> kvp in badCiteParameters.Where(kvp => !Errors.ContainsKey(kvp.Key)))
            {
                Errors.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<int, int> kvp in dupeBanerShellParameters.Where(kvp => !Errors.ContainsKey(kvp.Key)))
            {
                Errors.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<int, int> kvp in deadLinks.Where(kvp => !Errors.ContainsKey(kvp.Key)))
            {
                Errors.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<int, int> kvp in ambigCiteDates.Where(kvp => !Errors.ContainsKey(kvp.Key)))
            {
                Errors.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<int, int> kvp in unclosedTags.Where(kvp => !Errors.ContainsKey(kvp.Key)))
            {
                Errors.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<int, int> kvp in wikilinkedHeaders.Where(kvp => !Errors.ContainsKey(kvp.Key)))
            {
                Errors.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<int, int> kvp in targetlessLinks.Where(kvp => !Errors.ContainsKey(kvp.Key)))
            {
                Errors.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<int, int> kvp in doublepipeLinks.Where(kvp => !Errors.ContainsKey(kvp.Key)))
            {
                Errors.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<int, int> kvp in otherErrors.Where(kvp => !Errors.ContainsKey(kvp.Key)))
            {
                Errors.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<int, int> kvp in userSignature.Where(kvp => !Errors.ContainsKey(kvp.Key)))
            {
                Errors.Add(kvp.Key, kvp.Value);
            }

            if (Errors.Any())
                HighlightErrors(Errors);
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

        /// <summary>
        /// Skips the article based on protection level and contains/not contains logic
        /// </summary>
        /// <param name="checkContains">Whether to test contains logic</param>
        /// <param name="checkNotContains">Whether to test not contains logic</param>
        /// <returns>Whether the page has been skipped</returns>
        private bool SkipChecks(bool checkContains, bool checkNotContains)
        {
            if (!TheSession.User.CanEditPage(TheSession.Page))
            {
                SkipPage("Page is protected");
                return true;
            }

            if (!TheSession.User.CanCreatePage(TheSession.Page))
            {
                SkipPage("Page is protected from creation");
                return true;
            }

            if (checkContains && skipIfContains.CheckEnabled && skipIfContains.Matches(TheArticle))
            {
                SkipPage(skipIfContains.SkipReason);
                return true;
            }

            if (checkNotContains && skipIfNotContains.CheckEnabled && skipIfNotContains.Matches(TheArticle))
            {
                SkipPage(skipIfNotContains.SkipReason);
                return true;
            }

            return false;
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
            if (_flash) Tools.FlashWindow(this);
            #endif
            if (_beep) Tools.Beep();
        }

        private readonly TalkMessage _dlgTalk = new TalkMessage();

        private void WeHaveNewMessages()
        {
            Stop();
            Focus();
            TheSession.RequireUpdate();

            if (_dlgTalk.Visible) return; // we are already displaying it
            if (_dlgTalk.ShowDialog() == DialogResult.Yes)
            {
                Tools.OpenUserTalkInBrowser(TheSession.User.Name);
            }
        }

        private void NoWriteApiRight()
        {
            Stop();
            MessageBox.Show(this,
                            "This user doesn't have enough privileges to make automatic edits on this wiki.",
                            "Permission error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                ErrorHandler.HandleException(ex);
            }
            NudgeTimer.Reset();
            return true;
        }

        private void PageSaved(AsyncApiEdit sender, SaveInfo saveInfo)
        {
            ClearBrowser();
            txtEdit.Text = "";

            // TODO:Reinstate as needed
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

            // lower restart delay
            if (_restartDelay > 5)
                _restartDelay -= 1;

            NumberOfEdits++;

            LastArticle = "";
            listMaker.Remove(TheArticle);
            NudgeTimer.Stop();
            SameArticleNudges = 0;
            if (EditBoxTab.SelectedTab == tpHistory)
                EditBoxTab.SelectedTab = tpEdit;
            if (loggingEnabled)
            {
                TheArticle.LogListener.NewId = saveInfo.NewId;
                TheArticle.LogListener.URLLong = Variables.URLLong;
                logControl.AddLog(false, TheArticle.LogListener);
            }
            UpdateOverallTypoStats();

            if (!listMaker.Any() && _autoSaveEditBoxEnabled)
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
                // reset timer.
                NumberOfIgnoredEdits++;
                StopDelayedAutoSaveTimer();
                NudgeTimer.Stop();
                txtEdit.Text = "";

                // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_15#Endless_cycle_of_loading_and_skipping
                bool successfullyremoved = listMaker.Remove(TheArticle);

                SameArticleNudges = 0;

                if (!successfullyremoved)
                {
                    TheArticle = null;
                    MessageBox.Show("AWB failed to automatically remove the page from the list while skipping the page. Please remove it manually.", "Page removal from list failed", MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                    Stop();
                }
                else
                {
                    if (loggingEnabled)
                        logControl.AddLog(true, TheArticle.LogListener);
                    TheArticle = null;
                    Retries = 0;
                    Start();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
            }
        }

        private void SkipPage(string reason)
        {
            if (TheArticle == null)
            {
                DisableButtons();
                return;
            }

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
                theArticle.AWBChangeArticleText("Fixes for Unicode compatibility",
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

                if (CModule.ModuleUsable)
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

                if (Plugin.AWBPlugins.Any())
                {
                    foreach (KeyValuePair<string, IAWBPlugin> a in Plugin.AWBPlugins)
                    {
                        theArticle.SendPageToPlugin(a.Value, this);
                        if (theArticle.SkipArticle) return;
                    }
                }
                Variables.Profiler.Profile("Plugins");

                // unicodify whole article
                if (process && chkUnicodifyWhole.Checked)
                {
                    theArticle.Unicodify(Skip.SkipNoUnicode, Parser, RemoveText);
                    Variables.Profiler.Profile("Unicodify");
                }

                // find and replace before general fixes
                // Do not apply skip checks when reparsing
                if (chkFindandReplace.Checked)
                {
                    theArticle.PerformFindAndReplace(FindAndReplace, SubstTemplates, RplcSpecial,
                                                     (mainProcess && chkSkipWhenNoFAR.Checked), (mainProcess && chkSkipOnlyMinorFaR.Checked), false);

                    Variables.Profiler.Profile("F&R");

                    theArticle.DoFaRSkips(FindAndReplace);
                    if (theArticle.SkipArticle)
                        return;
                }

                // replace/add/remove categories
                if (cmboCategorise.SelectedIndex != 0)
                {
                    theArticle.Categorisation((WikiFunctions.Options.CategorisationOptions)
                                              cmboCategorise.SelectedIndex, Parser, chkSkipNoCatChange.Checked,
                                              txtNewCategory.Text.Trim(),
                                              txtNewCategory2.Text.Trim(), chkRemoveSortKey.Checked);
                    if (theArticle.SkipArticle)
                        return;
                    else if (!chkGeneralFixes.Checked)
                        theArticle.AWBChangeArticleText("Fix categories",
                                                        Parsers.FixCategories(theArticle.ArticleText), true);
                }

                Variables.Profiler.Profile("Categories");

                if (process)
                {
                    if (chkGeneralFixes.Checked)
                    {
                        theArticle.PerformUniversalGeneralFixes();
                        Variables.Profiler.Profile("Universal Genfixes");
                    }

                    if (theArticle.CanDoGeneralFixes)
                    {
                        if (chkGeneralFixes.Checked)
                        {
                            if (!TemplateRedirectsLoaded)
                            {
                                LoadTemplateRedirects();
                                Variables.Profiler.Profile("LoadTemplateRedirects");
                            }

                            if (!DatedTemplatesLoaded)
                            {
                                LoadDatedTemplates();
                                Variables.Profiler.Profile("LoadDatedTemplates");
                            }

                            if (!RenamedTemplateParametersLoaded)
                            {
                                LoadRenameTemplateParameters();
                                Variables.Profiler.Profile("LoadRenameTemplateParameters");
                            }

                            theArticle.PerformGeneralFixes(Parser, RemoveText, Skip,
                                                           replaceReferenceTagsToolStripMenuItem.Checked,
                                                           restrictDefaultsortChangesToolStripMenuItem.Checked,
                                                           noMOSComplianceFixesToolStripMenuItem.Checked);
                        }

                        Variables.Profiler.Profile("Mainspace Genfixes");

                        // auto tag
                        if (chkAutoTagger.Checked)
                        {
                            theArticle.AutoTag(Parser, Skip.SkipNoTag, restrictOrphanTaggingToolStripMenuItem.Checked);

                            if (mainProcess && theArticle.SkipArticle)
                                return;
                        }

                        Variables.Profiler.Profile("Auto-tagger");
                    }
                    else if (chkGeneralFixes.Checked)
                    {
                        if (theArticle.NameSpaceKey == Namespace.UserTalk)
                        {
                            if (!UserTalkWarningsLoaded)
                            {
                                LoadUserTalkWarnings();
                                Variables.Profiler.Profile("loadUserTalkWarnings");
                            }

                            theArticle.PerformUserTalkGeneralFixes(RemoveText, UserTalkTemplatesRegex,
                                                                   Skip.SkipNoUserTalkTemplatesSubstd);
                        }
                        else if (theArticle.CanDoTalkGeneralFixes)
                        {
                            theArticle.PerformTalkGeneralFixes(RemoveText);
                        }
                        Variables.Profiler.Profile("Talk Genfixes");
                    }
                }

                // RegexTypoFix
                if (chkRegExTypo.Checked && RegexTypos != null && !BotMode && !Namespace.IsTalk(theArticle.NameSpaceKey))
                {
                    if (!NoRetf.Contains(theArticle.Name))
                    {
                        theArticle.PerformTypoFixes(RegexTypos, chkSkipIfNoRegexTypo.Checked);
                        Variables.Profiler.Profile("Typos");
                        TypoStats = RegexTypos.GetStatistics();
                    }
                    else if (chkSkipIfNoRegexTypo.Checked)
                        TheArticle.Trace.AWBSkipped("No typo fixes (Title blacklisted from RegExTypoFix Typo Fixing)");

                    if (theArticle.SkipArticle)
                    {
                        if (mainProcess)
                        {
                            // update stats only if not called from e.g. 'Re-parse' than could be clicked repeatedly
                            OverallTypoStats.UpdateStats(TypoStats, true);
                            UpdateTypoCount();
                        }
                    }
                }

                // find and replace after general fixes
                // Do not apply skip checks when reparsing
                if (chkFindandReplace.Checked)
                {
                    theArticle.PerformFindAndReplace(FindAndReplace, SubstTemplates, RplcSpecial,
                                                     (mainProcess && chkSkipWhenNoFAR.Checked), (mainProcess && chkSkipOnlyMinorFaR.Checked), true);

                    theArticle.DoFaRSkips(FindAndReplace);

                    Variables.Profiler.Profile("F&R (2nd)");

                    if (theArticle.SkipArticle) return;
                }

                // append/prepend text
                if (chkAppend.Checked)
                {
                    // customized number of newlines
                    string newlines = "";
                    for (int i = 0; i < (int)udNewlineChars.Value; i++)
                        newlines += "\n";

                    if (rdoAppend.Checked)
                        theArticle.AWBChangeArticleText("Appended your message",
                                                        theArticle.ArticleText + newlines + Tools.ApplyKeyWords(theArticle.Name, txtAppendMessage.Text), false);
                    else
                        theArticle.AWBChangeArticleText("Prepended your message",
                                                        Tools.ApplyKeyWords(theArticle.Name, txtAppendMessage.Text) + newlines + theArticle.ArticleText, false);

                    if (chkAppendMetaDataSort.Checked)
                        theArticle.PerformMetaDataSort(Parser);
                }

                Variables.Profiler.Profile("Append Text");

                // replace/remove/comment out images/files
                if (cmboImages.SelectedIndex != 0)
                {
                    theArticle.UpdateImages((WikiFunctions.Options.ImageReplaceOptions)cmboImages.SelectedIndex,
                                            txtImageReplace.Text, txtImageWith.Text, chkSkipNoImgChange.Checked);
                    if (theArticle.SkipArticle)
                        return;
                }

                Variables.Profiler.Profile("Files");

                // disambiguation
                if (!preParseModeToolStripMenuItem.Checked && chkEnableDab.Checked && txtDabLink.Text.Trim().Length > 0 &&
                    txtDabVariants.Text.Trim().Length > 0)
                {
                    if (theArticle.Disambiguate(TheSession, txtDabLink.Text.Trim(), txtDabVariants.Lines, BotMode,
                                                (int)udContextChars.Value, chkSkipNoDab.Checked))
                    {
                        if (theArticle.SkipArticle)
                            return;
                    }
                    else
                    {
                        Abort = true;
                        Stop();
                        return;
                    }
                }
                Variables.Profiler.Profile("Disambiguate");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);

                // don't remove page over regex error – page itself is not at fault
                if (!ex.StackTrace.Contains("System.Text.RegularExpressions"))
                    theArticle.Trace.AWBSkipped("Exception:" + ex.Message);
                else
                    Skippable = false;
                Stop();
                StopDelayedAutoSaveTimer();
            }
            finally
            {
                Variables.Profiler.Flush();
            }
        }

        bool _diffAccessViolationSeen;

        private void GetDiff()
        {
            if (TheArticle == null)
            {
                DisableButtons();
                return;
            }

            try
            {
                if (webBrowser.Document == null)
                {
                    Tools.WriteDebug("GetDiff", "GetDiff called but webBrowser.Document null");
                    return;
                }

                webBrowser.Document.OpenNew(false);
                webBrowser.Document.MouseMove -= Document_MouseMove;
                if (TheArticle.OriginalArticleText.Equals(txtEdit.Text))
                {
                    webBrowser.Document.Write(
                        @"<h2 style='padding-top: .5em;
padding-bottom: .17em;
border-bottom: 1px solid #aaa;
font-size: 150%;'>No changes</h2><p>Press the ""Skip"" button below to skip to the next page.</p>");
                }
                else
                {
                    // when less than 10 edits show user help info on double click to undo etc.
                    webBrowser.Document.Write("<!DOCTYPE HTML PUBLIC \" -//W3C//DTD HTML 4.01 Transitional//EN\" \"http://www.w3.org/TR/html4/loose.dtd\">"
                                              + "<html><head>" +
                                              WikiDiff.DiffHead() + @"</head><body>" +
                                              ((NumberOfEdits < 10)
                                               ? WikiDiff.TableHeader
                                               : WikiDiff.TableHeaderNoMessages) +
                                              Diff.GetDiff(TheArticle.OriginalArticleText, txtEdit.Text, 2) +
                                              "</table></body></html>");
                }

                txtEdit.Focus();
                txtEdit.SelectionLength = 0;
                _diffAccessViolationSeen = false;

                GuiUpdateAfterProcessing();
            }
            catch (Exception ex)
            {
                // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_22#AccessViolationException_in_MainForm.GetDiff
                // catch this and use Refresh x2 to get around it
                // but only try once (diffAccessViolationSeen = false) so we don't get stuck in a loop
                if (ex is AccessViolationException && !_diffAccessViolationSeen)
                {
                    _diffAccessViolationSeen = true;
                    Tools.WriteDebug("GetDiff", "AccessViolationException seen");
 
                    webBrowser.Refresh();
                    webBrowser.Refresh();
                    GetDiff();
                }
                else
                    ErrorHandler.HandleException(ex);
            }
        }

        private string webBrowserMouseOverUrl = "";
        /// <summary>
        /// WebBrowser Document mouse move event: if mouse is over a link, store the URL
        /// Enables use of system browser for right-click Open in New Window option
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void Document_MouseMove(object sender, HtmlElementEventArgs e)
        {
            HtmlDocument thisDoc = (HtmlDocument)sender;
            if (thisDoc != null)
            {
                HtmlElement curElement = thisDoc.GetElementFromPoint(e.ClientMousePosition);
                if (curElement != null && curElement.TagName == "A")
                {
                    webBrowserMouseOverUrl = curElement.GetAttribute("href");
                }
            }
        }

        /// <summary>
        /// WebBrowser NewWindow event (preview): open URL in default browser
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private void webBrowser_NewWindow(System.Object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if (!string.IsNullOrEmpty(webBrowserMouseOverUrl))
            {
                Tools.WriteDebug("webBrowser_NewWindow", webBrowserMouseOverUrl);
                Tools.OpenURLInBrowser(webBrowserMouseOverUrl);
            }
            else
                Tools.WriteDebug("webBrowser_NewWindow", "webBrowserMouseOverUrl length zero");
        }

        private void GetPreview()
        {
            if (TheArticle == null)
            {
                DisableButtons();
                return;
            }

            if (!TheSession.Editor.IsActive)
            {
                StatusLabelText = "Previewing...";
                TheSession.Editor.Preview(TheArticle.Name, txtEdit.Text);
            }
            else
                StatusLabelText = "Editor busy";
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
                                          + "</body></html>"
                                         );

                webBrowser.Document.MouseMove += Document_MouseMove;
            }

            StatusLabelText = "";

            GuiUpdateAfterProcessing();
        }

        /// <summary>
        /// 
        /// </summary>
        private void GuiUpdateAfterProcessing()
        {
            if (_stopProcessing)
                Stop();
            else
            {
                Bleepflash();
                Focus();
                EnableButtons();
                btnSave.Select();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Save()
        {
            // Fail-safe against http://es.wikipedia.org/w/index.php?diff=28114575
            if (TheArticle == null || TheArticle.Name != TheSession.Page.Title)
            {
                DisableButtons();
                string extext = "Attempted to save a wrong page";

                if (TheArticle != null)
                    extext += " (Article name: '" + TheArticle.Name + "', session page title: '" + TheSession.Page.Title +
                        "')";
                else
                    extext += " (the article was null)";

                throw new Exception(extext);
            }

            if (!TheSession.Editor.IsActive)
                StatusLabelText = "Saving...";
            else
            {
                StatusLabelText = "Editor busy";
                return;
            }

            #if DEBUG
            string extext2 = @"Extra validation for debug builds (don't use a debug build if you want to save blank pages): ";
            // further attempts to track down blank page saving issue
            if (string.IsNullOrEmpty(TheArticle.ArticleText))
            {
                extext2 += @"Attempted to save page with zero length ArticleText";
                throw new Exception(extext2);
            }

            if (string.IsNullOrEmpty(txtEdit.Text))
            {
                extext2 += @"Attempted to save page with zero length txtEditText";
                throw new Exception(extext2);
            }
            #endif

            DisableButtons();
            StartProgressBar();
            if (!string.IsNullOrEmpty(txtEdit.Text) ||
                (TheArticle.Exists == Exists.Yes && MessageBox.Show("Do you really want to save a blank page?", "Save?",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                    DialogResult.Yes))
            {
                SaveArticle();
            }
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

            if (!TheSession.Editor.IsActive)
            {
                if (!TheSession.Page.Exists)
                    NumberOfNewPages++;

                // if section edit summary, check only this section has been edited
                if (txtReviewEditSummary.Text.StartsWith(@"/*"))
                {
                    string sectionEditText = Summary.ModifiedSection(TheArticle.OriginalArticleText, txtEdit.Text);

                    if (sectionEditText.Length == 0 || !txtReviewEditSummary.Text.Contains(@"/* " + sectionEditText + @" */"))
                        txtReviewEditSummary.Text = txtReviewEditSummary.Text.Substring(txtReviewEditSummary.Text.IndexOf(@"*/", StringComparison.Ordinal)+2);
                }

                TheSession.Editor.Save(txtEdit.Text, AppendUsingAWBSummary(txtReviewEditSummary.Text), markAllAsMinorToolStripMenuItem.Checked,
                                       opt);
            }
            else
            {
                StatusLabelText = "Editor busy";
                EnableButtons();
            }
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
            readonly MainForm _owner;

            internal JsAdapter(MainForm owner)
            {
                _owner = owner;
            }

            /// <summary>
            /// Reverses the changes to a line of text in the page
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            public void UndoChange(int left, int right)
            {
                _owner.UndoChangeGeneric(DiffChangeMode.Change, left, right);
            }

            /// <summary>
            /// Reverses the deletion of a line of text from the page
            /// </summary>
            /// <param name="left"></param>
            /// <param name="right"></param>
            public void UndoDeletion(int left, int right)
            {
                _owner.UndoChangeGeneric(DiffChangeMode.Deletion, left, right);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="right"></param>
            public void UndoAddition(int right)
            {
                _owner.UndoChangeGeneric(DiffChangeMode.Addition, 0, right);
            }

            /// <summary>
            /// Moves the caret to the input line within the article text box
            /// </summary>
            /// <param name="destLine">the line number the caret should be moved to</param>
            public void GoTo(int destLine)
            {
                _owner.GoTo(destLine);
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
            if (!txtEdit.Enabled)
                return;

            try
            {
                int caretPosition = txtEdit.SelectionStart;
                // see http://stackoverflow.com/questions/1277691/how-to-retrieve-the-scrollbar-position-of-the-webbrowser-control-in-net
                int webBrowserYScroll = webBrowser.Document.GetElementsByTagName("HTML")[0].ScrollTop;
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

                GetDiff();

                if (syntaxHighlightEditBoxToolStripMenuItem.Checked)
                    HighlightSyntax();

                // scroll back to where user was
                object[] ob = {"window.scrollTo(0, " + webBrowserYScroll + @")"};
                webBrowser.Document.InvokeScript("eval", ob);

                // now put caret back where it was
                txtEdit.Select(Math.Min(caretPosition, txtEdit.Text.Length), 0);
                txtEdit.ScrollToCaret();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
            }
        }

        /// <summary>
        /// Moves the caret to the input line within the article text box
        /// </summary>
        /// <param name="destLine">the line number the caret should be moved to</param>
        private void GoTo(int destLine)
        {
            // If some text is selected in the diff/preview, don't focus edit box else selected text will not be copyable by keyboard shortcuts
            if (webBrowser.TextSelected())
                return;

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
                ErrorHandler.HandleException(ex);
            }
        }
        #endregion

        private void PanelShowHide()
        {
            if (panel1.Visible) panel1.Hide();
            else panel1.Show();

            SetBrowserSize();
        }

        private Point _oldPosition;
        private Size _oldSize;
        private void ParametersShowHide()
        {
            enlargeEditAreaToolStripMenuItem.Checked = !enlargeEditAreaToolStripMenuItem.Checked;
            if (listMaker.Visible)
            {
                btntsShowHideParameters.Image = Resources.Showhideparameters2;

                _oldPosition = EditBoxTab.Location;
                EditBoxTab.Location = new Point(listMaker.Location.X, listMaker.Location.Y - 17);

                _oldSize = EditBoxTab.Size;
                EditBoxTab.Size = new Size((EditBoxTab.Size.Width + MainTab.Size.Width + listMaker.Size.Width + 8), EditBoxTab.Size.Height);
            }
            else
            {
                btntsShowHideParameters.Image = Resources.Showhideparameters;

                EditBoxTab.Location = _oldPosition;
                EditBoxTab.Size = _oldSize;
            }
            listMaker.Visible = MainTab.Visible = !listMaker.Visible;
            label8.Visible =  listMaker.Visible;
        }

        private void UpdateStatusUI()
        {
            UpdateUserName();
            UpdateBotStatus();
            UpdateAdminStatus();
        }

        private void UpdateUserNotifications()
        {
            if (!Variables.NotificationsEnabled)
            {
                lblUserNotifications.Visible = false;
                return;
            }
            if (!lblUserNotifications.Visible)
            {
                lblUserNotifications.Visible = true;
            }

            lblUserNotifications.Text = TheSession.User.Notifications.ToString();

            if (TheSession.User.Notifications > 0)
            {
                lblUserNotifications.BackColor = Color.Tomato;
                lblUserNotifications.Font = new Font(lblUserNotifications.Font, FontStyle.Bold);
            }
            else
            {
                lblUserNotifications.BackColor = Color.Gray;
                lblUserNotifications.Font = new Font(lblUserNotifications.Font, FontStyle.Regular);
            }
        }

        private void UpdateUserName()
        {
            if (string.IsNullOrEmpty(TheSession.User.Name))
            {
                lblUserName.Text = Variables.Namespaces.ContainsKey(Namespace.User)
                    ? Variables.Namespaces[Namespace.User]
                    : "User:";
            }
            else
            {
                lblUserName.Text = TheSession.User.Name;
            }

            if (TheSession.Status == WikiStatusResult.Registered)
            {
                lblUserName.BackColor = Color.Green;
                lblUserName.ForeColor = Color.White;
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

            if (Properties.Settings.Default.AskForTerminate)
            {
                TimeSpan time =
                    new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second).Subtract
                    (StartTime);
                dlg = new ExitQuestion(time, NumberOfEdits, "");
                dlg.ShowDialog();
                Properties.Settings.Default.AskForTerminate = !dlg.CheckBoxDontAskAgain;
            }

            // save user persistent settings
            Properties.Settings.Default.Save();

            if (dlg != null)
            {
                switch (dlg.DialogResult)
                {
                    case DialogResult.OK:
                        CloseDownAWB();
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
            else if (!Properties.Settings.Default.AskForTerminate)
            {
                CloseDownAWB();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void CloseDownAWB()
        {
            ShuttingDown = true;

            // TheSession can be null if AWB encounters network problems on startup
            if (TheSession != null)
                TheSession.Editor.Abort();

            SaveRecentSettingsList();
            UsageStats.Do(true);

            ntfyTray.Visible = false;
            ntfyTray.Dispose();
        }

        /// <summary>
        /// Sets the edit summary for the current edit. Returns a section edit summary if edit is to a single section and the section edit summary option is enabled
        /// </summary>
        /// <returns>The completed edit summary</returns>
        private string MakeDefaultEditSummary()
        {
            if (TheArticle == null)
                return "";

            string summary = string.IsNullOrEmpty(cmboEditSummary.Text.Trim()) ? "" : cmboEditSummary.Text.Trim();

            if (!string.IsNullOrEmpty(TheArticle.EditSummary))
            {
                switch (Variables.LangCode)
                {
                    case "ar":
                    case "arz":
                    case "fa":
                        summary += (string.IsNullOrEmpty(summary) ? "" : "، ") + TheArticle.EditSummary;
                        break;
                    default:
                        summary += (string.IsNullOrEmpty(summary) ? "" : ", ") + TheArticle.EditSummary;
                        break;
                }
            }

            // check to see if we have only edited one level-2 section
            if (!noSectionEditSummaryToolStripMenuItem.Checked)
            {
                string sectionEditText = Summary.ModifiedSection(TheArticle.OriginalArticleText, txtEdit.Text);

                if (!string.IsNullOrEmpty(sectionEditText))
                {
                    summary = @"/* " + sectionEditText + @" */ " + summary.TrimStart();
                }
            }
            
            #if DEBUG
            if (!Summary.IsCorrect(summary))
            {
                Tools.WriteDebug("edit summary not correct", summary);
            }
            #endif

            return summary;
        }
        
        /// <summary>
        /// Appends (translation of) "using AWB" wikilinked summary tag to edit summary
        /// </summary>
        /// <param name="summary">The current edit summary</param>
        /// <returns>The updated edit summary</returns>
        private string AppendUsingAWBSummary(string summary)
        {
            if (!(TheSession.User.IsBot && chkSuppressTag.Checked)
                && (Variables.IsWikimediaProject && !_suppressUsingAWB))
                summary = Summary.Trim(summary) + Variables.SummaryTag;
            
            return summary;
        }

        private void chkFindandReplace_CheckedChanged(object sender, EventArgs e)
        {
            btnMoreFindAndReplce.Enabled = btnFindAndReplaceAdvanced.Enabled =
                chkSkipWhenNoFAR.Enabled = chkSkipOnlyMinorFaR.Enabled = btnSubst.Enabled = chkFindandReplace.Checked;
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

            if (!Globals.UsingMono) // fails unexplainably under Mono
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

            UpdateBotTimer();
        }

        private void UpdateAdminStatus()
        {
            // allow protection of non-existent page (salting)
            btnProtect.Enabled = TheSession.User.CanProtectPage(TheSession.Page) && btnSave.Enabled && (TheArticle != null);
            btnMove.Enabled = btnProtect.Enabled && TheSession.Page.Exists;
            btnDelete.Enabled = btntsDelete.Enabled = TheSession.User.CanDeletePage(TheSession.Page) && btnSave.Enabled && (TheArticle != null) && TheSession.Page.Exists;
            bypassAllRedirectsToolStripMenuItem.Enabled = TheSession.User.IsSysop;
        }

        private void chkAutoMode_CheckedChanged(object sender, EventArgs e)
        {
            if (BotMode)
            {
                SetBotModeEnabled(true);
                chkNudge.Checked = true;
                chkNudgeSkip.Checked = false;

                // typo fixing not permitted on Wikimedia projects per [[WP:SPELLBOT]]
                if (chkRegExTypo.Checked && !Variables.IsCustomProject)
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
            label2.Enabled = nudBotSpeed.Enabled = botEditsStop.Enabled
                = lblAutoDelay.Enabled = lblbotEditsStop.Enabled = btnResetNudges.Enabled = lblNudges.Enabled = chkNudge.Enabled
                = chkNudgeSkip.Enabled = chkNudge.Checked = chkShutdown.Enabled = enabled;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox(webBrowserHistory.Version.ToString()).Show();
        }

        public bool CheckStatus(bool login)
        {
            StatusLabelText = "Loading page to check if we are logged in.";

            bool status = false;
            string label = "Software disabled";

            switch (TheSession.Update())
            {
                case WikiStatusResult.Error:
                    MessageBox.Show(
                        "Check page failed to load.\r\n\r\nCheck your Internet is working and that the Wiki is online.",
                        "User check problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case WikiStatusResult.NotLoggedIn:
                    if (!login)
                    {
                        MessageBox.Show(
                            "You are not logged in. The profile screen will now load, enter your name and password, click \"Log in\", wait for it to complete, then start the process again.",
                            "Not logged in", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Profiles.ShowDialog();
                    }

                    break;

                case WikiStatusResult.NotRegistered:
                    MessageBox.Show(TheSession.User.Name + " is not enabled to use this.", "Not enabled",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Tools.OpenURLInBrowser(Variables.URLIndex + "?title=Project:AutoWikiBrowser/CheckPageJSON");

                    break;

                case WikiStatusResult.OldVersion:
                    OldVersion();
                    break;

                case WikiStatusResult.NoRights:
                    NoWriteApiRight();
                    break;

                case WikiStatusResult.Registered:
                    NoParse.Clear();
                    NoRetf.Clear();
                    status = true;
                    label = string.Format("Logged in, user and software enabled. Bot = {0}, Admin = {1}",
                        TheSession.User.IsBot, TheSession.User.IsSysop);

                    NoParse.AddRange(TheSession.NoGenfixes);
                    NoRetf.AddRange(TheSession.NoRETF);

                    break;

                default:
                    throw new Exception("Unknown WikiStatusResult value.");
            }

            // detect writing system
            RightToLeft = Variables.RTL ? RightToLeft.Yes : RightToLeft.No;

            StatusLabelText = label;
            UpdateStatusUI();
            UpdateButtons(null, null);

            return status;
        }

        private void OldVersion()
        {
            lblUserName.BackColor = Color.Red;
            DisableButtons();

            switch (
                MessageBox.Show(
                    "This version of AWB is not enabled, please download the newest version. If you have the newest version, check that Wikipedia is online.\r\n\r\nPlease press \"Yes\" to run the AutoUpdater, \"No\" to load the download page and update manually, or \"Cancel\" to not update (but you will not be able to edit).",
                    "Problem", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information))
            {
                case DialogResult.Yes:
                    RunUpdater();
                    break;
                case DialogResult.No:
                    Tools.OpenURLInBrowser("https://sourceforge.net/projects/autowikibrowser/files/");
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
        }

        private void chkAppend_CheckedChanged(object sender, EventArgs e)
        {
            txtAppendMessage.Enabled = rdoAppend.Enabled = rdoPrepend.Enabled =
                udNewlineChars.Enabled = lblUse.Enabled = lblNewlineCharacters.Enabled = chkAppendMetaDataSort.Enabled = chkAppend.Checked;
        }

        private void wordWrapToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            txtEdit.WordWrap = wordWrapToolStripMenuItem1.Checked;
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

        private const string Words = "Words: ",
        Cats = "Categories: ",
        Imgs = "Images: ",
        Links = "Links: ",
        IWLinks = "Interwiki links: ",
        Dates = "Dates O/I/A: ";

        private void ArticleInfo(bool reset)
        {
            lbDuplicateWikilinks.Items.Clear();
            lbAlerts.Items.Clear();
            ambigCiteDates.Clear();
            badCiteParameters.Clear();
            deadLinks.Clear();
            doublepipeLinks.Clear();
            dupeBanerShellParameters.Clear();
            targetlessLinks.Clear();
            unclosedTags.Clear();
            wikilinkedHeaders.Clear();
            unbalancedBracket.Clear();
            otherErrors.Clear();
            userSignature.Clear();

            if (reset)
            {
                //Resets all the alerts.
                lblWords.Text = Words;
                lblCats.Text = Cats;
                lblImages.Text = Imgs;
                lblLinks.Text = Links;
                lblInterLinks.Text = IWLinks;
                lblDates.Text = Dates;
            }
            else
            {
                string articleText = txtEdit.Text;
                string templates = string.Join("", Parsers.GetAllTemplateDetail(articleText).ToArray());

                int wordCount = Tools.WordCount(articleText);
                int catCount = WikiRegexes.Category.Matches(articleText).Count;
                
                bool hasAlertsOn = !alertPreferences.Any();

                if ((hasAlertsOn || alertPreferences.Contains(12)) && TheArticle.NameSpaceKey == Namespace.Article  && wordCount > Parsers.StubMaxWordCount && WikiRegexes.Stub.IsMatch(templates))
                    lbAlerts.Items.Add("Long article with a stub tag.");

                if ((hasAlertsOn || alertPreferences.Contains(14)) && catCount == 0 && !Namespace.IsTalk(TheArticle.Name))
                    lbAlerts.Items.Add("No category (may be one in a template)");

                // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Replace_nofootnotes_with_morefootnote_if_references_exists
                if ((hasAlertsOn || alertPreferences.Contains(7)) && TheArticle.NameSpaceKey == Namespace.Article && TheArticle.HasMorefootnotesAndManyReferences)
                    lbAlerts.Items.Add("Has 'No/More footnotes' template yet many references");

                // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#.28Yet.29_more_reference_related_changes.
                if ((hasAlertsOn || alertPreferences.Contains(6)) && TheArticle.HasRefAfterReflist)
                    lbAlerts.Items.Add(@"Has a <ref> after <references/>");

                if ((hasAlertsOn || alertPreferences.Contains(3)) && TheArticle.IsDisambiguationPageWithRefs)
                    lbAlerts.Items.Add(@"DAB page with <ref>s");

                if ((hasAlertsOn || alertPreferences.Contains(16)) && TheArticle.NameSpaceKey == Namespace.Article && articleText.StartsWith("=="))
                    lbAlerts.Items.Add("Starts with heading");

                // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Format_references
                if ((hasAlertsOn || alertPreferences.Contains(19)) && TheArticle.HasBareReferences)
                    lbAlerts.Items.Add("Unformatted references");

                // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Detect_multiple_DEFAULTSORT
                if ((hasAlertsOn || alertPreferences.Contains(13)) && WikiRegexes.Defaultsort.Matches(templates).Count > 1)
                    lbAlerts.Items.Add("Multiple DEFAULTSORTs");

                // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Some_additional_edits
                if (hasAlertsOn || alertPreferences.Contains(4))
                {
                    deadLinks = TheArticle.DeadLinks();
                    if (deadLinks.Any())
                        lbAlerts.Items.Add("Dead links" + " (" + deadLinks.Count + ")");
                }

                if (hasAlertsOn || alertPreferences.Contains(1))
                {
                    ambigCiteDates = TheArticle.AmbiguousCiteTemplateDates();
                    if (ambigCiteDates.Any())
                        lbAlerts.Items.Add("Ambiguous citation dates" + " (" + ambigCiteDates.Count + ")");
                }

                if (hasAlertsOn || alertPreferences.Contains(17))
                {
                    unbalancedBracket = TheArticle.UnbalancedBrackets();
                    if (unbalancedBracket.Any())
                        lbAlerts.Items.Add("Unbalanced brackets" + " (" + unbalancedBracket.Count + ")");
                }

                if (hasAlertsOn || alertPreferences.Contains(9))
                {
                    badCiteParameters = TheArticle.BadCiteParameters();
                    if (badCiteParameters.Any())
                        lbAlerts.Items.Add("Invalid citation parameter(s)" + " (" + badCiteParameters.Count + ")");
                }

                if (hasAlertsOn || alertPreferences.Contains(11))
                {
                    targetlessLinks = TheArticle.TargetlessLinks();
                    if (targetlessLinks.Any())
                        lbAlerts.Items.Add("Links with no target" + " (" + targetlessLinks.Count + ")");
                }

                if (hasAlertsOn || alertPreferences.Contains(10))
                {
                    doublepipeLinks = TheArticle.DoublepipeLinks();
                    if (doublepipeLinks.Any())
                        lbAlerts.Items.Add("Links with double pipes" + " (" + doublepipeLinks.Count + ")");
                }

                if (hasAlertsOn || alertPreferences.Contains(5))
                {
                    dupeBanerShellParameters = TheArticle.DuplicateWikiProjectBannerShellParameters();
                    if (dupeBanerShellParameters.Any())
                        lbAlerts.Items.Add("Duplicate parameter(s) in WPBannerShell" + " (" + dupeBanerShellParameters.Count + ")");
                }

                if (hasAlertsOn || alertPreferences.Contains(21))
                {
                    UnknownWikiProjectBannerShellParameters = TheArticle.UnknownWikiProjectBannerShellParameters();
                    if (UnknownWikiProjectBannerShellParameters.Any())
                    {
                        string warn = "Unknown parameters in WPBannerShell: " + " (" +
                            UnknownWikiProjectBannerShellParameters.Count + ") "
                            + string.Join(",", UnknownWikiProjectBannerShellParameters.ToArray());
                        lbAlerts.Items.Add(warn);
                    }
                }

                if (hasAlertsOn || alertPreferences.Contains(20))
                {
                    UnknownMultipleIssuesParameters = TheArticle.UnknownMultipleIssuesParameters();
                    if (UnknownMultipleIssuesParameters.Any())
                    {
                        string warn = "Unknown parameters in Multiple issues: " + " (" +
                            UnknownMultipleIssuesParameters.Count + ") "
                            + string.Join(", ", UnknownMultipleIssuesParameters.ToArray());
                        lbAlerts.Items.Add(warn);
                    }
                }

                if (hasAlertsOn || alertPreferences.Contains(8))
                {
                    wikilinkedHeaders = TheArticle.WikiLinkedHeaders();
                    if (wikilinkedHeaders.Any())
                        lbAlerts.Items.Add("Header(s) with wikilinks" + " (" + wikilinkedHeaders.Count + ")");
                }

                if (hasAlertsOn || alertPreferences.Contains(18))
                {
                    unclosedTags = TheArticle.UnclosedTags();
                    if (unclosedTags.Any())
                        lbAlerts.Items.Add("Unclosed tag(s)" + " (" + unclosedTags.Count + ")");
                }

                if ((hasAlertsOn || alertPreferences.Contains(15)) && TheArticle.HasSeeAlsoAfterNotesReferencesOrExternalLinks)
                {
                    lbAlerts.Items.Add("See also section out of place");

                    // Performance: faster to fetch all headings and filter than apply WikiRegexes.SeeAlso directly
                    Match m = (from Match h in WikiRegexes.Headings.Matches(articleText)
                                              where WikiRegexes.SeeAlso.IsMatch(h.Value)
                                              select h).FirstOrDefault();

                    if (m != null && !otherErrors.ContainsKey(m.Index))
                        otherErrors.Add(m.Index, m.Length);
                }

                // check for {{sic}} tags etc. when doing typo fixes
                if ((hasAlertsOn || alertPreferences.Contains(2) || chkRegExTypo.Checked) && TheArticle.HasSicTag)
                    lbAlerts.Items.Add(@"Contains 'sic' tag");

                // check for [[User: or [[[User talk:
                if ((hasAlertsOn || alertPreferences.Contains(22)) && TheArticle.NameSpaceKey == Namespace.Article)
                {
                    userSignature = TheArticle.UserSignature();
                    if (userSignature.Any())
                        lbAlerts.Items.Add("Editor's signature or link to user space" + " (" + userSignature.Count + ")");
                }

                MatchCollection imagesMC = WikiRegexes.ImagesCountOnly.Matches(articleText);
                lblWords.Text = Words + wordCount;
                lblCats.Text = Cats + catCount;
                lblImages.Text = Imgs + imagesMC.Count;
                lblLinks.Text = Links + Tools.LinkCount(articleText);
                lblInterLinks.Text = IWLinks + Tools.InterwikiCount(articleText);

                // for date types count ignore images and URLs
                string articleTextNoImagesURLs = WikiRegexes.ExternalLinksHTTPOnlyQuick.Replace(Tools.ReplaceWithSpaces(articleText, imagesMC), "");
                Dictionary<Parsers.DateLocale, int> results = Tools.DatesCount(articleTextNoImagesURLs);

                lblDates.Text = Dates + results[Parsers.DateLocale.ISO] + "/" + results[Parsers.DateLocale.International] + "/" + results[Parsers.DateLocale.American];

                // Find multiple wikilinks
                //get all the links, ignore commented out text etc.
                lbDuplicateWikilinks.Items.AddRange(Tools.DuplicateWikiLinks(articleText).ToArray());
            }
            lblDuplicateWikilinks.Visible = lbDuplicateWikilinks.Visible = btnRemove.Visible = (lbDuplicateWikilinks.Items.Count > 0);
        }

        /// <summary>
        /// Focuses the edit box on the next alert after the caret
        /// </summary>
        private void lbAlerts_Click(object sender, EventArgs e)
        {
            EditBoxTab.SelectedTab = tpEdit;

            string a = txtEdit.Text.Substring(0, txtEdit.SelectionStart);
            int b = WikiRegexes.Newline.Matches(a).Count;
            bool done = false;

            foreach (KeyValuePair<int, int> kvp in Errors)
            {
                int current = txtEdit.SelectionStart + b; // offset by number of newlines up to it
                if (kvp.Key > current && kvp.Key < txtEdit.Text.Length)
                {
                    RedSelection(kvp.Key, kvp.Value);
                    txtEdit.ScrollToCaret();
                    done = true;
                    break;
                }
            }

            // if no more alerts after caret, start at beginning
            if (!done)
            {
                txtEdit.SelectionStart = 0;

                foreach (KeyValuePair<int, int> kvp in Errors)
                {
                    if (kvp.Key > txtEdit.SelectionStart && kvp.Key < txtEdit.Text.Length)
                    {
                        RedSelection(kvp.Key, kvp.Value);
                        txtEdit.ScrollToCaret();
                        break;
                    }
                }
            }
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

            if (sender is RichTextBox)
            {
                RichTextBox rtb = (RichTextBox)sender;

                // disable TextChanged temporarily under Mono otherwise get infinite loop
                if(Globals.UsingMono)
                    txtFind.TextChanged -= ResetFind;

                rtb.ResetFormatting();

                if(Globals.UsingMono)
                    txtFind.TextChanged += ResetFind;
            }

            btnFind.Enabled = txtFind.TextLength > 0;

            if (!btnFind.Enabled)
                btnFind.BackColor = SystemColors.ButtonFace;
        }

        private void txtEdit_TextChanged(object sender, EventArgs e)
        {
            txtEdit.ResetFind();

            // when highlight enabled reset back colour for newly inserted text
            /* TODO: does not work fully in that: focus always scrolls to current line unnecessarily
             * text inserted at very end of text box goes before last character
            if (highlightAllFindToolStripMenuItem.Checked)
            {
                txtEdit.SetEditBoxSelection(txtEdit.SelectionStart-1, 1);
                txtEdit.SelectionBackColor = Color.White;
                txtEdit.SetEditBoxSelection(txtEdit.SelectionStart+1, 1);
                txtEdit.DeselectAll();
            }
             */
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
                txtEdit.GoToLine(int.Parse(toolStripTextBox2.Text));
                mnuTextBox.Hide();
            }
        }

        [Conditional("DEBUG")]
        private void Debug()
        {
            Tools.WriteDebugEnabled = true;
            if (!listMaker.Contains(@"Wikipedia:AutoWikiBrowser/Sandbox") && !listMaker.Contains(@"Project:AutoWikiBrowser/Sandbox"))
                listMaker.Add("Project:AutoWikiBrowser/Sandbox");
            lblOnlyBots.Visible = false;
            bypassAllRedirectsToolStripMenuItem.Enabled = true;

            profileTyposToolStripMenuItem.Visible = true;
            toolStripSeparator29.Visible = true;
            invalidateCacheToolStripMenuItem.Visible = true;
            toolStripSeparator32.Visible = true;
            cEvalToolStripMenuItem.Visible = true;

            #if DEBUG
            try
            {
                Variables.Profiler = new Profiler(Path.Combine(Application.StartupPath, "profiling.txt"), true);
            }
            catch
            {
                Variables.Profiler = new Profiler(Path.Combine(AwbDirs.UserData, "profiling.txt"), true);
            }
            #endif
        }

        [Conditional("RELEASE")]
        private void Release()
        {
            if (MainTab.Contains(tpBots) && !Globals.UsingMono)
                MainTab.Controls.Remove(tpBots);
        }

        #endregion

        #region set variables

        private void PreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenPreferences(false);
        }

        List<int> alertPreferences = new List<int>();

        private void OpenPreferences(bool focusSiteTab)
        {
            MyPreferences myPrefs = new MyPreferences(Variables.LangCode, Variables.Project,
                                                      Variables.CustomProject, Variables.Protocol)
                                        {
                                            TextBoxFont = txtEdit.Font,
                                            LowThreadPriority = LowThreadPriority,
                                            PrefFlash = _flash,
                                            PrefBeep = _beep,
                                            PrefMinimize = _minimize,
                                            PrefSaveArticleList = _saveArticleList,

                                            PrefAutoSaveEditBoxEnabled = _autoSaveEditBoxEnabled,
                                            PrefAutoSaveEditBoxFile = _autoSaveEditBoxFile,
                                            PrefAutoSaveEditBoxPeriod = AutoSaveEditBoxPeriod,

                                            PrefIgnoreNoBots = IgnoreNoBots,
                                            PrefClearPageListOnProjectChange = ClearPageListOnProjectChange,

                                            PrefShowTimer = ShowMovingAverageTimer,
                                            PrefAddUsingAWBOnArticleAction = Article.AddUsingAWBOnArticleAction,
                                            PrefSuppressUsingAWB = _suppressUsingAWB,

                                            PrefListComparerUseCurrentArticleList = _listComparerUseCurrentArticleList,
                                            PrefListSplitterUseCurrentArticleList = _listSplitterUseCurrentArticleList,
                                            PrefDBScannerUseCurrentArticleList = _dbScannerUseCurrentArticleList,

                                            PrefDiffInBotMode = doDiffInBotMode,
                                            // show edit page no longer available as an option
                                            PrefOnLoad = actionOnLoad == 2 ? 0 : actionOnLoad,

                                            EnableLogging = loggingEnabled,
                                            FocusSiteTab = focusSiteTab,

                                            PrefDomain = Variables.LoginDomain,

                                            AlertPreferences = alertPreferences
                                        };

            if (myPrefs.ShowDialog(this) == DialogResult.OK)
            {
                txtEdit.Font = myPrefs.TextBoxFont;
                LowThreadPriority = myPrefs.LowThreadPriority;
                _flash = myPrefs.PrefFlash;
                _beep = myPrefs.PrefBeep;
                _minimize = myPrefs.PrefMinimize;
                _saveArticleList = myPrefs.PrefSaveArticleList;
                _autoSaveEditBoxEnabled = myPrefs.PrefAutoSaveEditBoxEnabled;

                if (EditBoxSaveTimer.Enabled && !_autoSaveEditBoxEnabled)
                    EditBoxSaveTimer.Enabled = false;

                AutoSaveEditBoxPeriod = myPrefs.PrefAutoSaveEditBoxPeriod;
                _autoSaveEditBoxFile = myPrefs.PrefAutoSaveEditBoxFile;
                _suppressUsingAWB = myPrefs.PrefSuppressUsingAWB;
                Article.AddUsingAWBOnArticleAction = myPrefs.PrefAddUsingAWBOnArticleAction;

                IgnoreNoBots = myPrefs.PrefIgnoreNoBots;
                ClearPageListOnProjectChange = myPrefs.PrefClearPageListOnProjectChange;

                ShowMovingAverageTimer = myPrefs.PrefShowTimer;

                _listComparerUseCurrentArticleList = myPrefs.PrefListComparerUseCurrentArticleList;
                _listSplitterUseCurrentArticleList = myPrefs.PrefListSplitterUseCurrentArticleList;
                _dbScannerUseCurrentArticleList = myPrefs.PrefDBScannerUseCurrentArticleList;

                doDiffInBotMode = myPrefs.PrefDiffInBotMode;
                actionOnLoad = myPrefs.PrefOnLoad;

                loggingEnabled = myPrefs.EnableLogging;

                Variables.LoginDomain = myPrefs.PrefDomain;

                alertPreferences = myPrefs.AlertPreferences;

                if (myPrefs.Language != Variables.LangCode || myPrefs.Project != Variables.Project
                    || (myPrefs.CustomProject != Variables.CustomProject) || (myPrefs.Protocol != Variables.Protocol))
                {
                    SetProject(myPrefs.Language, myPrefs.Project, myPrefs.CustomProject, myPrefs.Protocol);

                    BotMode = false;
                    lblOnlyBots.Visible = true;

                    if (ClearPageListOnProjectChange)
                        listMaker.Clear();

                    DisableButtons();
                }
            }
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // refresh login status, and reload check list
            CheckStatus(false);

            // refresh typo list
            LoadTypos(true);

            WikiDiff.ResetCustomStyles();

            // refresh talk warnings list
            if (UserTalkWarningsLoaded)
                LoadUserTalkWarnings();

            if (TemplateRedirectsLoaded)
                LoadTemplateRedirects();

            if (DatedTemplatesLoaded)
                LoadDatedTemplates();

            if (RenamedTemplateParametersLoaded)
                LoadRenameTemplateParameters();
        }

        private void resetEditSkippedCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NumberOfEdits > 0)
                UsageStats.Do(false);

            NumberOfEdits = 0;
            NumberOfIgnoredEdits = 0;
            NumberOfEditsPerMinute = 0;
            NumberOfNewPages = 0;
            NumberOfPagesPerMinute = 0;
            NumberOfPagesParsed = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="project"></param>
        /// <param name="customProject"></param>
        /// <param name="protocol"></param>
        private void SetProject(string code, ProjectEnum project, string customProject, string protocol)
        {
            SplashScreen.SetProgress(81);
            // set namespaces
            try
            {
                //set namespaces
                Variables.SetProject(code, project, customProject, protocol);
            }
            catch (WebException ex)
            {
                // Check for HTTP 401 error.
                var resp = (HttpWebResponse) ex.Response;
                if (resp == null) throw;
                switch (resp.StatusCode)
                {
                    case HttpStatusCode.Unauthorized /*401*/:
                        ShowLogin();

                        // Reload project.
                        Variables.SetProject(code, project, customProject, protocol);

                        break;
                }
            }
            catch (UriFormatException)
            {
                MessageBox.Show("Check the site url you entered is valid, and try again!");
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("The interwiki list didn't load correctly. Please check your internet connection, and then restart AWB.");
            }

            if (Variables.TryLoadingAgainAfterLogin)
            {
                MessageBox.Show(
                    "You seem to be accessing a private wiki. Project loading will be attempted again after login.",
                    "Restricted Wiki");
            }

            // set interwikiorder
            switch (Variables.LangCode)
            {
                case "en":
                case "lb":
                case "pl":
                case "no":
                case "sv":
                case "simple":
                    Parser.InterWikiOrder = InterWikiOrderEnum.LocalLanguageAlpha;
                    break;

                case "he":
                case "hu":
                case "te":
                case "yi":
                    Parser.InterWikiOrder = InterWikiOrderEnum.AlphabeticalEnFirst;
                    break;

                case "ms":
                case "et":
                case "nn":
                case "fi":
                case "vi":
                case "ur":
                    Parser.InterWikiOrder = InterWikiOrderEnum.LocalLanguageFirstWord;
                    break;

                default:
                    Parser.InterWikiOrder = InterWikiOrderEnum.Alphabetical;
                    break;
            }
            
            // commons uses alpha as https://commons.wikimedia.org/w/api.php?action=query&meta=allmessages&ammessages=Interwiki%20config-sorting%20order is not set to an override value
            if(project == ProjectEnum.commons)
                Parser.InterWikiOrder = InterWikiOrderEnum.Alphabetical;

            // user interface
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
            else
                lblProject.Text = Variables.IsWikimediaMonolingualProject ? Variables.Project.ToString() : Variables.URL;

            TemplateRedirectsLoaded = false;
            ResetTypoStats();
        }

        #endregion

        // TODO: Cleanup/refactor UI update functions
        #region Enabling/Disabling of buttons

        private void UpdateButtons(object sender, EventArgs e)
        {
            SetStartButton(listMaker.NumberOfArticles > 0);

            lbltsNumberofItems.Text = "Pages: " + listMaker.NumberOfArticles;

            specialFilterToolStripMenuItem1.Enabled = saveListToTextFileToolStripMenuItem.Enabled  
                = clearCurrentListToolStripMenuItem.Enabled = convertFromTalkPagesToolStripMenuItem.Enabled
                = convertToTalkPagesToolStripMenuItem.Enabled = (listMaker.NumberOfArticles > 0);
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

            txtEdit.Enabled = txtReviewEditSummary.Enabled = false;
        }

        private void EnableButtons()
        {
            UpdateButtons(null, null);
            SetButtons(true);
            txtEdit.Enabled = txtReviewEditSummary.Enabled = true;
        }

        private void SetButtons(bool enabled)
        {
            btnIgnore.Enabled = btnPreview.Enabled = btnDiff.Enabled =
                btntsPreview.Enabled = btntsChanges.Enabled = /*listMaker.MakeListEnabled = */
                btntsSave.Enabled = btntsIgnore.Enabled = btnWatch.Enabled = findGroup.Enabled = enabled;

            btnSave.Enabled = enabled && TheArticle != null && !string.IsNullOrEmpty(TheSession.Page.Title);

            // allow protection of non-existent page (salting)
            btnProtect.Enabled = (enabled && TheSession.User.CanProtectPage(TheSession.Page) && TheArticle != null);
            btnMove.Enabled = btnProtect.Enabled && TheSession.Page.Exists;
            btnDelete.Enabled = btntsDelete.Enabled = enabled && TheSession.User.CanDeletePage(TheSession.Page) && TheArticle != null && TheSession.Page.Exists;
            btnFind.Enabled = txtFind.TextLength > 0;

            // if there are find matches, colour the Find button yellow
            if (btnFind.Enabled && txtEdit.FindAll(txtFind.Text, chkFindRegex.Checked, chkFindCaseSensitive.Checked, TheArticle.Name).Any())
                btnFind.BackColor = Color.Yellow;
            else
                btnFind.BackColor = SystemColors.ButtonFace;
        }

        #endregion

        #region Timers

        int _restartDelay = 5, _startInSeconds = 5;
        private void DelayedRestart(object sender, EventArgs e)
        {
            StopDelayedAutoSaveTimer();
            StatusLabelText = "Restarting in " + _startInSeconds;

            if (_startInSeconds == 0)
            {
                StopDelayedRestartTimer();
                Start();
            }
            else
                _startInSeconds--;
        }

        private void StartDelayedRestartTimer()
        {
            //increase the restart delay each time, this is decreased by 1 on each successfull save
            int delay = _restartDelay + 5;
            if (delay > 60)
                delay = 60;

            StartDelayedRestartTimer(delay);
        }

        private void StartDelayedRestartTimer(int delay)
        {
            _startInSeconds = _restartDelay = delay;
            Ticker += DelayedRestart;
        }

        private void StopDelayedRestartTimer()
        {
            Ticker -= DelayedRestart;
            _startInSeconds = _restartDelay;
        }

        private void UpdateBotTimer()
        {
            lblBotTimer.Text = chkAutoMode.Checked ? "Bot timer: " + _intTimer : "";
        }

        private void StopDelayedAutoSaveTimer()
        {
            Ticker -= DelayedAutoSave;
            _intTimer = 0;
        }

        private void StartDelayedAutoSaveTimer()
        {
            Ticker += DelayedAutoSave;
        }

        int _intTimer;
        private void DelayedAutoSave(object sender, EventArgs e)
        {
            if (_intTimer < nudBotSpeed.Value)
            {
                _intTimer++;
                lblBotTimer.BackColor = (_intTimer == 1) ? Color.Red : DefaultBackColor;
            }
            else
            {
                StopDelayedAutoSaveTimer();
                SaveArticle();
            }

            UpdateBotTimer();

            if (botEditsStop.Value > 0 && NumberOfEdits >= botEditsStop.Value)
            {
                Stop();
                StatusLabelText = "Stopped: " + botEditsStop.Value + " edits reached";
            }
        }

        private void ShowTimer()
        {
            lblTimer.Visible = ShowMovingAverageTimer;
            StopSaveInterval();
        }

        int _intStartTimer;
        private void SaveInterval(object sender, EventArgs e)
        {
            _intStartTimer++;
            lblTimer.Text = "Timer: " + _intStartTimer;
        }

        private void StopSaveInterval()
        {
            _intStartTimer = 0;
            lblTimer.Text = "Timer: 0";
            Ticker -= SaveInterval;
        }

        public event EventHandler Ticker;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Ticker != null)
                Ticker(null, null);

            _seconds++;
            if (_seconds == 60)
            {
                _seconds = 0;
                GenerateEditStatistics();
            }
        }

        int _seconds, _lastEditsTotal, _lastPagesTotal;
        private void GenerateEditStatistics()
        {
            //Edits in the last minute
            NumberOfEditsPerMinute = (NumberOfEdits - _lastEditsTotal);

            // pages processed in last minute: edits + skipped in normal mode or number of pages pre-parsed when in pre-parse mode
            NumberOfPagesPerMinute = Math.Max(NumberOfEdits + NumberOfIgnoredEdits + NumberOfPagesParsed - _lastPagesTotal, 0);
            _lastEditsTotal = NumberOfEdits;
            _lastPagesTotal = NumberOfEdits + NumberOfIgnoredEdits + NumberOfPagesParsed;
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
            if (TheArticle != null && TheArticle.Name.Length > 0)
                Tools.WriteTextFileAbsolutePath("#[[" + TheArticle.Name + "]]\r\n", Path.Combine(AwbDirs.UserData, @"False positives.txt"), true);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            BeginProcess();
        }

        /// <summary>
        /// 
        /// </summary>
        private void BeginProcess()
        {
            if (!TheSession.User.IsLoggedIn)
            {
                Profiles.ShowDialog();
                if (!TheSession.User.IsLoggedIn) return;
            }
            else if (
                (RunProcessPageBackground != null &&
                 RunProcessPageBackground.ThreadStatus().IsIn(ThreadState.Running, ThreadState.Background)) ||
                (RunReparseEditBoxBackground != null &&
                 RunReparseEditBoxBackground.ThreadStatus().IsIn(ThreadState.Running, ThreadState.Background))
                )
            {
                StatusLabelText = "Background process running";
                return;
            }
            
            _stopProcessing = false;
            Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            // ask user confirmation if manual changes in edit box (edit box populated and not same as article text)
            if (TheArticle == null || TheArticle.ArticleText.Equals(txtEdit.Text) || txtEdit.Text.Length == 0 ||
            MessageBox.Show("There are manual changes to the page text in the edit box, are you sure you want to stop?", "Confirm stop", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Stop();
        }

        private void btnSave_Click(object sender, EventArgs e)
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
            Start();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteArticle();
            Start();
        }

        private void btnProtect_Click(object sender, EventArgs e)
        {
            ProtectArticle();
            Start();
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

        private void specialFilterToolStripMenuItem_Click(object sender, EventArgs e)
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
            switch (_listComparerUseCurrentArticleList)
            {
                case 0: // Ask
            		if (listMaker.Any() &&
                        MessageBox.Show("Would you like to copy your current Article List to the ListComparer?",
                                        "Copy Article List?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        goto case 1;
                    }

                    goto case 2;

                case 1: // Always
                    Comparer = new ListComparer(listMaker, listMaker.GetArticleList());
                    break;
                case 2: // Never
                    Comparer = new ListComparer(listMaker);
                    break;
            }

            Comparer.Show(this);
        }

        private void launchListSplitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            switch (_listSplitterUseCurrentArticleList)
            {
                case 0: // Ask
                    if (listMaker.Any() &&
                        MessageBox.Show("Would you like to copy your current Article List to the ListSplitter?",
                                        "Copy Article List?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        goto case 1;
                    }

                    goto case 2;

                case 1: // Always
                    Splitter = new ListSplitter(MakePrefs(), listMaker.GetArticleList());
                    break;
                case 2: // Never
                    Splitter = new ListSplitter(MakePrefs());
                    break;
            }

            Splitter.Show(this);
        }

        private void launchDumpSearcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LaunchDumpSearcher();
        }

        private void LaunchDumpSearcher()
        {
            switch (_dbScannerUseCurrentArticleList)
            {
                case 0: // Ask
                    if (MessageBox.Show("Would you like the results to be added to the ListMaker Article List?",
                                        "Add to ListMaker?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                        DialogResult.Yes)
                    {
                        goto case 1;
                    }

                    goto case 2;

                case 1: // Always
                    DBScanner = listMaker.DBScanner();
                    break;
                case 2: // Never
                    DBScanner = new WikiFunctions.DBScanner.DatabaseScanner();
                    break;
            }

            DBScanner.Show();
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
                    BeginProcess();
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
                
                if (e.KeyCode == Keys.B) 
                {
                    lbAlerts_Click(null, null);
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
            DataFormats.Format plainText = DataFormats.GetFormat(DataFormats.Text);
            txtEdit.Paste(plainText);
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
            if (TheArticle != null)
                txtEdit.SelectedText = "{{Hndis|name=" + Tools.MakeHumanCatKey(TheArticle.Name, TheArticle.ArticleText) + "}}";
        }

        private void wikifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Text = "{{Wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + txtEdit.Text;
        }

        private void cleanupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Text = "{{cleanup|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + txtEdit.Text;
        }

        private void speedyDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rectangle scrn = Screen.GetWorkingArea(this);

            var res =
                WikiFunctions.Controls.InputBox.Show(
                    "Enter a reason. Leave blank if you'll edit the reason in the AWB text box",
                    "Speedy deletion",
                    "",
                    null,
                    scrn.Width/2, scrn.Height/3);
            if (res.OK)
            {
                txtEdit.Text = "{{db|" + res.Text.Trim()  + "}}\r\n\r\n" + txtEdit.Text;
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = "{{subst:clear}}";
        }

        private void disambiguationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = "{{Disambiguation}}";
        }

        private void uncategorisedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = "{{Uncategorized|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
        }

        private void bypassAllRedirectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #if !DEBUG
            if (MessageBox.Show("Replacement of links to redirects with direct links is strongly discouraged, " +
                                "however it could be useful in some circumstances. Are you sure you want to continue?",
                                "Bypass redirects", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;
            #endif

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
            txtEdit.SelectedText = WikiRegexes.PersonDataDefault;
            txtEdit.Text = Parsers.PersonData(txtEdit.Text, TheArticle.Name);
        }

        private void humanNameCategoryKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TheArticle != null)
                txtEdit.SelectedText = "{{DEFAULTSORT:" + Tools.MakeHumanCatKey(TheArticle.Name, TheArticle.ArticleText) + "}}";
        }

        private readonly Regex RegexDates = new Regex("[12][0-9]{3}", RegexOptions.Compiled);

        private void birthdeathCatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TheArticle == null)
                return;

            try
            {
                string articleTextLocal = txtEdit.Text;

                // ignore dates in file captions etc.
                articleTextLocal = Tools.ReplaceWithSpaces(articleTextLocal, WikiRegexes.FileNamespaceLink.Matches(articleTextLocal));
                
                // ignore dates from dated maintenance tags etc.
                foreach (Match m2 in WikiRegexes.NestedTemplates.Matches(articleTextLocal))
                {
                    if (Tools.GetTemplateParameterValue(m2.Value, "date").Length > 0)
                        articleTextLocal = articleTextLocal.Replace(m2.Value, "");
                }

                foreach (Match m2 in WikiRegexes.TemplateMultiline.Matches(articleTextLocal))
                {
                    if (Tools.GetTemplateParameterValue(m2.Value, "date").Length > 0)
                        articleTextLocal = articleTextLocal.Replace(m2.Value, "");
                }

                MatchCollection m = RegexDates.Matches(articleTextLocal);

                //find first dates
                string births = "", deaths = "";

                if (m.Count >= 1)
                    births = m[0].Value;
                if (m.Count >= 2)
                    deaths = m[1].Value;

                //make name, surname, firstname
                string name = Tools.MakeHumanCatKey(TheArticle.Name, TheArticle.ArticleText);

                string categories;

                if (string.IsNullOrEmpty(deaths) || int.Parse(deaths) < int.Parse(births) + 20)
                    categories = "[[Category:" + births + " births|" + name + "]]";
                else
                    categories = "[[Category:" + births + " births|" + name + "]]\r\n[[Category:" + deaths + " deaths|" + name + "]]";

                txtEdit.SelectedText = categories;

                bool noChange;
                txtEdit.Text = Parsers.ChangeToDefaultSort(txtEdit.Text, TheArticle.Name, out noChange, restrictDefaultsortChangesToolStripMenuItem.Checked);

                // sort if DEFAULTSORT added to ensure correct placement
                if (!noChange)
                {
                    txtEdit.Text = Parser.SortMetaData(txtEdit.Text, TheArticle.Name);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
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
            TheSession.Site.OpenPageInBrowser(TheArticle.Name);
        }

        private void openTalkPageInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TheSession.Site.OpenPageInBrowser(Tools.ConvertToTalk(TheArticle));
        }

        private void openHistoryMenuItem_Click(object sender, EventArgs e)
        {
            TheSession.Site.OpenPageHistoryInBrowser(TheArticle.Name);
        }

        private void openSelectionInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser#Open_text_selection_in_browser
            // user feedback is better to (try to) open invalid HTTP link as HTTP link than open as wiki page
            if (WikiRegexes.UrlValidator.IsMatch(txtEdit.SelectedText.Trim()) || txtEdit.SelectedText.Trim().StartsWith("http", StringComparison.OrdinalIgnoreCase))
                Tools.OpenURLInBrowser(txtEdit.SelectedText.Trim());
            else
                TheSession.Site.OpenPageInBrowser(txtEdit.SelectedText);
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
            _stopProcessing = true;
            PageReload = false;
            NudgeTimer.Stop();

            // abort any background thread if running
            if (RunReparseEditBoxBackground != null)
                RunReparseEditBoxBackground.Abort();

            if (RunProcessPageBackground != null)
                RunProcessPageBackground.Abort();

            UpdateButtons(null, null);
            DisableButtons();

            if (_intTimer > 0)
            {// stop and reset the bot timer.
                StopDelayedAutoSaveTimer();
                EnableButtons();
                return;
            }

            StopSaveInterval();
            StopDelayedRestartTimer();

            TheSession.Editor.Abort();

            listMaker.Stop();

            if (_autoSaveEditBoxEnabled)
                EditBoxSaveTimer.Enabled = false;

            StatusLabelText = "Stopped";
            ClearBrowser();
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Tools.OpenENArticleInBrowser("Wikipedia:AutoWikiBrowser/User manual", false);
        }

        #region Edit Box Menu
        private void reparseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReparseEditBox();
        }

        private BackgroundRequest RunReparseEditBoxBackground;

        // run process page step of reparse edit box in a background thread
        // use .Complete event to do rest of processing (alerts etc.) once thread finished
        private void ReparseEditBox()
        {
            if (TheArticle == null)
                return;

            if ((RunProcessPageBackground != null && (RunProcessPageBackground.ThreadStatus() == ThreadState.Running 
            || RunProcessPageBackground.ThreadStatus() == ThreadState.Background)) ||
            (RunReparseEditBoxBackground != null && (RunReparseEditBoxBackground.ThreadStatus() == ThreadState.Running 
            || RunReparseEditBoxBackground.ThreadStatus() == ThreadState.Background)))
            {
                StatusLabelText = "Background process running";
                return;
            }

            StatusLabelText = "Processing page";
            StartProgressBar();

            // refresh text from text box to pick up user changes
            TheArticle.AWBChangeArticleText("Reparse", txtEdit.Text, false);

            RunReparseEditBoxBackground = new BackgroundRequest();
            RunReparseEditBoxBackground.Execute(ReparseEditBoxBackground);
            RunReparseEditBoxBackground.Complete += ReparseEditBoxComplete;
        }

        /// <summary>
        /// Runs ProcessPage and UpdateCurrentTypoStats as background jobs for reparse edit box
        /// </summary>
        private void ReparseEditBoxBackground()
        {
            ProcessPage(TheArticle, false);

            ErrorHandler.CurrentPage = "";
        }

        /// <summary>
        /// Second part of reparse edit box to update alerts, article stats etc.
        /// Called after ReparseEditBoxBackground completes
        /// </summary>
        private void ReparseEditBoxPart2()
        {
            UpdateCurrentTypoStats();
            ArticleInfo(false);

            txtEdit.Text = TheArticle.ArticleText;
            txtEdit.Visible = false;

            // clear any red from previous alerts to avoid entire edit box being coloured red after reparse
            txtEdit.SelectAll();
            txtEdit.SelectionBackColor = Color.White;

            if (highlightAllFindToolStripMenuItem.Checked)
                HighlightAllFind();

            Errors.Clear();

            if (scrollToAlertsToolStripMenuItem.Checked)
                HighlightErrors();

            if (syntaxHighlightEditBoxToolStripMenuItem.Checked)
                HighlightSyntax();

            txtEdit.Visible = true;

            GetDiff();

            // refocus edit box and Save button
            if (!focusAtEndOfEditTextBoxToolStripMenuItem.Checked)
            {
                txtEdit.SetEditBoxSelection(0, 0);
                txtEdit.Select(0, 0);
            }
            else
                txtEdit.Select(txtEdit.Text.Length, 0);

            txtEdit.ScrollToCaret();
            btnSave.Select();
            StopProgressBar();
            StatusLabelText = "Ready to save";
        }

        /// <summary>
        /// Called by ReparseEditBox complete event
        /// Calls ReparseEditBoxPart2 to update alerts etc. after re-processing page
        /// </summary>
        /// <param name="req"></param>
        private void ReparseEditBoxComplete(BackgroundRequest req)
        {
            // must use Invoke so that ReparseEditBoxPart2 is done on main GUI thread
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(ReparseEditBoxPart2));
                return;
            }
            ReparseEditBoxPart2();
        }

        private void replaceTextWithLastEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LastArticle.Length > 0)
            {
                txtEdit.Text = LastArticle;

                if (actionOnLoad == 0)
                    GetDiff();
            }
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
                string[] dlgStrings = {
                    dlg.String1, dlg.String2, dlg.String3, dlg.String4, dlg.String5, dlg.String6, dlg.String7, dlg.String8, dlg.String9, dlg.String10
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

        private void txtNewCategory2_DoubleClick(object sender, EventArgs e)
        {
            txtNewCategory2.SelectAll();
        }

        private void cmboEditSummary_MouseMove(object sender, MouseEventArgs e)
        {
            if ((TheArticle != null) && string.IsNullOrEmpty(TheArticle.EditSummary))
                ToolTip.SetToolTip(cmboEditSummary, "");
            else
                ToolTip.SetToolTip(cmboEditSummary, txtReviewEditSummary.Text);
        }
        
        // If user changes default edit summary after article has been processed, refresh editable edit summary; any custom changes to editable edit summary will be lost
        private void cmboEditSummary_TextChanged(object sender, EventArgs e)
        {
            if (txtReviewEditSummary.Enabled)
                txtReviewEditSummary.Text = MakeDefaultEditSummary();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons(null, null);
        }

        bool _loadingTypos;
        private void chkRegExTypo_CheckedChanged(object sender, EventArgs e)
        {
            if (_loadingTypos)
                return;

            if (!chkRegExTypo.Checked)
            {
                chkSkipIfNoRegexTypo.Checked = chkSkipIfNoRegexTypo.Enabled = false;
                return;
            }

            chkSkipIfNoRegexTypo.Enabled = true;

            if (chkRegExTypo.Checked && BotMode)
            {
                MessageBox.Show("RegExTypoFix cannot be used with bot mode on.\r\nBot mode will now be turned off, and Typos loaded.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                BotMode = false;
            }

            LoadTypos(false);
        }

        public void LoadTypos(bool reload)
        {
            // during change of user settings to settings with typos enabled, LoadTypos will be called more than once
            // from LoadPrefs...SetProject with RETF to say reload is needed when RETF next used, and then again from LoadPrefs with RETF enabled
            // so set RegexTypos to null if reload, even if RETF not enabled at the time
            // This logic is needed to support use of SetProject in other scenarios
            if (reload)
            {
                RegexTypos = null;
            }

            if (chkRegExTypo.Checked && RegexTypos == null)
            {
                _loadingTypos = true;
                chkRegExTypo.Checked = false;

                StatusLabelText = "Loading typos";

                // if not logged in will be using default Typos page location, so look up any custom page from CheckPage info
                // if no checkpage then fall back to using default ReftPath
                if (!TheSession.User.IsLoggedIn && !Variables.IsWikipediaEN &&
                    Variables.RetfPath.EndsWith("AutoWikiBrowser/Typos"))
                {
                    try
                    {
                        // Try and use the config json text if it's not empty... Is this actually going to work as expected?
                        // Or should we just unconditionally do the else?
                        if (!string.IsNullOrEmpty(TheSession.ConfigJSONText))
                        {
                            Session.TypoLink(Tools.GetJObjectFromText(TheSession.ConfigJSONText));
                        }
                        else
                        {
                            Session.TypoLink(Tools.GetJObjectFromUrl(Session.ConfigUrl));
                        }
                    }
                    catch
                    {
                        // TODO: Should we be setting a default?
                        // Variables.RetfPath = "Project:AutoWikiBrowser/Typos";
                    }
                    
                }
#if !DEBUG
                string message = @"Check each edit before you make it. Although this has been built to be very accurate there will be errors.";

                if (RegexTypos == null)
                {
                    string s = Variables.RetfPath;

                    if (!s.StartsWith("http"))
                    {
                        // TODO: Try to use TheSession.Site.ArticleUrl for prettier URL
                        s = Variables.NonPrettifiedURL(s);
                    }

                    message += "\r\n\r\nThe newest typos will now be downloaded from " + s + " when you press OK.";
                }

                MessageBox.Show(message, "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
#endif

                RegexTypos = new RegExTypoFix();
                RegexTypos.Complete += RegexTyposComplete;
            }
        }

        private void RegexTyposComplete(BackgroundRequest req)
        {
            if (InvokeRequired)
            {
                Invoke(new BackgroundRequestComplete(RegexTyposComplete), req);
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

            _loadingTypos = false;
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
            BeginProcess();
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
                        lblImageWith.Text = "&With " + Variables.Namespaces[Namespace.File];

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
            if (TheArticle == null)
            {
                DisableButtons();
                return;
            }

            try
            {
                if (!TheSession.Page.Exists)
                {
                    MessageBox.Show("Cannot move a non-existent page");
                    return;
                }

                if (!TheSession.User.CanMovePage(TheSession.Page))
                {
                    MessageBox.Show(
                        "Current user doesn't have enough rights to move \"" + TheSession.Page.Title + "\"",
                        "User rights not sufficient");
                    return;
                }

                string newTitle, msg;
                bool succeed = TheArticle.Move(TheSession, out newTitle);

                if (succeed)
                {
                    Article replacementArticle = new Article(newTitle);

                    msg = "Moved " + TheArticle.Name + " to " + newTitle;

                    listMaker.ReplaceArticle(TheArticle, replacementArticle);
                }
                else
                    msg = "Move of " + TheArticle.Name + " failed!";

                articleActionLogControl1.LogArticleAction(TheArticle.Name, succeed, ArticleAction.Move, msg);
                StatusLabelText = msg;
            }
            catch (ApiErrorException ae)
            {
                switch (ae.ErrorCode)
                {
                    case "missingtitle":
                        StatusLabelText = "Article deleted, cannot move";
                        listMaker.Remove(TheArticle);
                        articleActionLogControl1.LogArticleAction(TheArticle.Name, false, ArticleAction.Move,
                                                                  "Article already deleted, cannot move");
                        break;
                    case "articleexists":
                        StatusLabelText = "Target exists, cannot move";
                        MessageBox.Show(
                            "The destination article already exists and is not a redirect to the source article.\r\nMove not completed",
                            "Target exists");
                        articleActionLogControl1.LogArticleAction(TheArticle.Name, false, ArticleAction.Move,
                                                                  "Target exists");
                        break;
                    default:
                        ErrorHandler.HandleException(ae);
                        break;
                }
            }
            catch (ApiException ae)
            {
                if (ae is InvalidTitleException || ae is InterwikiException)
                {
                    MessageBox.Show(ae.Message, "Invalid Target page");
                    return;
                }
                ErrorHandler.HandleException(ae);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
            }
        }

        private void DeleteArticle()
        {
            if (TheArticle == null)
            {
                DisableButtons();
                return;
            }

            try
            {
                if (!TheSession.Page.Exists)
                {
                    MessageBox.Show("Cannot delete a non-existent page");
                    return;
                }

                if (!TheSession.User.CanDeletePage(TheSession.Page))
                {
                    MessageBox.Show(
                        "Current user doesn't have enough rights to delete \"" + TheSession.Page.Title + "\"",
                        "User rights not sufficient");
                    return;
                }

                string msg;
                bool succeed = TheArticle.Delete(TheSession);

                if (succeed)
                {
                    msg = "Deleted " + TheArticle.Name;
                    listMaker.Remove(TheArticle);
                }
                else
                    msg = "Deletion of " + TheArticle.Name + " failed!";

                StatusLabelText = msg;
                articleActionLogControl1.LogArticleAction(TheArticle.Name, succeed, ArticleAction.Delete, msg);
            }
            catch (ApiErrorException ae)
            {
                if (ae.ErrorCode == "missingtitle")
                {
                    StatusLabelText = "Article already deleted";
                    listMaker.Remove(TheArticle);

                    articleActionLogControl1.LogArticleAction(TheArticle.Name, false, ArticleAction.Delete,
                                                              "Article already deleted");
                    return;
                }

                if (ae.ErrorCode == "bigdelete")
                {
                    StatusLabelText = "You can't delete this page because it has more than 5,000 revisions";
                    listMaker.Remove(TheArticle);

                    articleActionLogControl1.LogArticleAction(TheArticle.Name, false, ArticleAction.Delete,
                                                              "Article can't be deleted");
                    return;
                }
                ErrorHandler.HandleException(ae);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
            }
        }

        private void ProtectArticle()
        {
            if (TheArticle == null)
            {
                DisableButtons();
                return;
            }

            try
            {
                if (!TheSession.User.IsSysop)
                {
                    MessageBox.Show(
                        "Current user doesn't have enough rights to protect \"" + TheSession.Page.Title + "\"",
                        "User rights not sufficient");
                    return;
                }

                string msg;
                bool succeed = TheArticle.Protect(TheSession);

                if (succeed)
                    msg = "Protected " + TheArticle.Name;
                else
                    msg = "Protection of " + TheArticle.Name + " failed!";

                articleActionLogControl1.LogArticleAction(TheArticle.Name, succeed, ArticleAction.Protect, msg);
                StatusLabelText = msg;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
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

                StringBuilder builder = new StringBuilder();
                foreach (
                    Article a in
                    new LinksOnPageListProvider().MakeList(
                        txtDabLink.Text.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries)))
                {
                    uint i;
                    // exclude years
                    if (uint.TryParse(a.Name, out i) && (i < 2100))
                    {
                        continue;
                    }

                    builder.AppendLine(a.Name);
                }
                txtDabVariants.Text = builder.ToString();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
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
                        selectedtext = selectedtext.Substring(0, selectedtext.IndexOf("(", StringComparison.Ordinal));
                    if (selectedtext.Contains(":"))
                        selectedtext = selectedtext.Substring(selectedtext.IndexOf(":", StringComparison.Ordinal)).TrimEnd('|');
                    if (txtEdit.SelectedText == "[[" + selectedtext + "]]")
                    {
                        MessageBox.Show("The selected link could not be removed.");
                        selectedtext = "[[" + selectedtext + "]]";
                    }
                }
                else if (selectedtext.Contains("|"))
                    selectedtext = selectedtext.Substring(selectedtext.IndexOf("|", StringComparison.Ordinal) + 1);

                txtEdit.SelectedText = selectedtext;
                txtEdit.ResetFind();
            }
            else
                MessageBox.Show("Select a link to remove either manually or by clicking a link in the list above.");
        }

        private void runUpdaterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunUpdater();
        }

        private void RunUpdater()
        {
            string file = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "AWBUpdater.exe");

            if (!File.Exists(file))
            {
                MessageBox.Show("Updater doesn't exist, therefore cannot be run");
                return;
            }

            Process.Start(file);
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
            // make sure there was no error and bot mode is still enabled
            if (BotMode)
            {
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
                    _stopProcessing = false;
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
            if (_autoSaveEditBoxFile.Trim().Length > 0) SaveEditBoxText(_autoSaveEditBoxFile);
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
        /// Loads the list of user talk templates from [[WP:AWB/User talk templates]], generates UserTalkTemplatesRegex from them
        /// </summary>
        private void LoadUserTalkWarnings()
        {
            Regex userTalkTemplate = new Regex(@"# ?\[\["
                                               + Variables.NamespacesCaseInsensitive[Namespace.Template] + @"(.*?)\]\]");
            UserTalkTemplatesRegex = null;
            UserTalkWarningsLoaded = true; // or it will retry on each page load

            List<string> userTalkTemplates = new List<string>();

            try
            {
                string text;
                try
                {
                    text = TheSession.Editor.SynchronousEditor.Clone().Open("Project:AutoWikiBrowser/User talk templates", true);
                }
                catch
                {
                    return;
                }

                foreach (Match m in userTalkTemplate.Matches(text))
                {
                    userTalkTemplates.Add(m.Groups[1].Value);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
                UserTalkWarningsLoaded = false;
            }

            if (userTalkTemplates.Any())
                UserTalkTemplatesRegex = Tools.NestedTemplateRegex(userTalkTemplates);
        }

        /// <summary>
        /// Loads the list of template redirects to bypass from [[WP:AWB/Template redirects]]
        /// </summary>
        private void LoadTemplateRedirects()
        {
            string text;
            TemplateRedirectsLoaded = true;
            try
            {
                text = TheSession.Editor.SynchronousEditor.Clone().Open("Project:AutoWikiBrowser/Template redirects", true);
            }
            catch
            {
                text = "";
            }

            // always make this call even if no text found (if changed project from one to another must make sure redirects are cleared)
            WikiRegexes.TemplateRedirects = Parsers.LoadTemplateRedirects(text);
        }

        private void LoadDatedTemplates()
        {
            string text;
            DatedTemplatesLoaded = true;
            try
            {
                text = TheSession.Editor.SynchronousEditor.Clone().Open("Project:AutoWikiBrowser/Dated templates", true);
            }
            catch
            {
                text = "";
            }

            if (text.Length > 0)
                WikiRegexes.DatedTemplates = Parsers.LoadDatedTemplates(text);
        }

        private void LoadRenameTemplateParameters()
        {
            string text;
            RenamedTemplateParametersLoaded = true;
            try
            {
                text = TheSession.Editor.SynchronousEditor.Clone().Open("Project:AutoWikiBrowser/Rename template parameters", true);
            }
            catch
            {
                text = "";
            }

            if (text.Length > 0)
                WikiRegexes.RenamedTemplateParameters = Parsers.LoadRenamedTemplateParameters(text);
        }

        private void undoAllChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TheArticle == null)
                return;

            txtEdit.Text = TheArticle.OriginalArticleText;
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
                                                         + "&action=history&printable=yes") && !string.IsNullOrEmpty(pageTitle)
                       )
                        webBrowserHistory.Navigate(Variables.URLIndex + "?title=" + name
                                                   + "&action=history&printable=yes");
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
                                "&printable=yes") && !string.IsNullOrEmpty(title))
                        webBrowserLinks.Navigate(Variables.URLIndex + "?title=Special:WhatLinksHere/" +
                                                 title + "&printable=yes");
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
                webBrowserHistory.Navigate(Variables.URLIndex + "?title=" + TheArticle.URLEncodedName + "&action=history&printable=yes");
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
            if (!ShuttingDown)
            {
                Profiles.ShowDialog(this);
            }
        }

        private void UserDefaultSettingsLoadRequired(object sender, EventArgs e)
        {
            LoadPrefs(Profiles.SettingsToLoad);
        }

        private void ProfileLoggedIn(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Profiles.SettingsToLoad) && Variables.TryLoadingAgainAfterLogin)
            {
                SetProject(Variables.ReloadProjectSettings.langCode, Variables.ReloadProjectSettings.projectName,
                           Variables.ReloadProjectSettings.customProject, Variables.ReloadProjectSettings.protocol);
            }

            if (TheSession.IsBusy)
                TheSession.Editor.Abort();

            // English Wikipedia does not use {{Wikify}} // {{Persondata}}
            wikifyToolStripMenuItem.Visible = !Variables.IsWikipediaEN;
            metadataTemplateToolStripMenuItem.Visible = !Variables.IsWikipediaEN;

            TheArticle = null;
            txtEdit.Text = "";
            TemplateRedirectsLoaded = false;

            CheckStatus(true);

            UpdateStatusUI();

            StopProgressBar();
            DisableButtons();
            if (TheSession.User.HasMessages)
                WeHaveNewMessages();

            UpdateUserNotifications();
        }

        private void chkMinor_CheckedChanged(object sender, EventArgs e)
        {
            markAllAsMinorToolStripMenuItem.Checked = chkMinor.Checked;
        }

        private void markAllAsMinorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chkMinor.Checked = markAllAsMinorToolStripMenuItem.Checked;
        }

        private void ShowLogin()
        {
            Login login = new Login();
            login.ShowDialog(this);
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
        	get { return (chkShutdown.Checked && !listMaker.Any()); }
        }

        private void Shutdown()
        {
            if (CanShutdown)
            {
                ShutdownTimer.Enabled = true;
                ShutdownTimer.Start();
                ShutdownNotification shut = new ShutdownNotification { ShutdownType = GetShutdownType() };

                switch (shut.ShowDialog(this))
                {
                    case DialogResult.Cancel:
                        ShutdownTimer.Stop();
                        ShutdownTimer.Enabled = false;
                        MessageBox.Show(GetShutdownType() + " aborted!");
                        return;
                    case DialogResult.OK:
                        ShutdownComputer();
                        break;
                }
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
            if (!ShutdownTimer.Enabled)
                return;

            Stop();

            ShutdownTimer.Stop();
            ShutdownTimer.Enabled = false;

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
                if (imgBold.Visible == value)
                {
                    return;
                }

                // move edit summary box for toolbar
                if (value) // Edit toolbar visible
                {
                    txtReviewEditSummary.Location =
                        new Point((int) Math.Round(txtEdit.Location.X + imgBold.Width*12.4),
                                  txtReviewEditSummary.Location.Y);
                    txtReviewEditSummary.Size = new Size((int) Math.Round(txtEdit.Size.Width - imgBold.Width*12.4),
                                                         txtReviewEditSummary.Size.Height);
                }
                else
                {
                    txtReviewEditSummary.Location = new Point(txtEdit.Location.X, txtReviewEditSummary.Location.Y);
                    txtReviewEditSummary.Size = new Size(txtEdit.Size.Width, txtReviewEditSummary.Size.Height);
                }

                imgBold.Visible = imgExtlink.Visible = imgHr.Visible = imgItalics.Visible = imgLink.Visible =
                                                                                            imgMath.Visible =
                                                                                            imgNowiki.Visible =
                                                                                            imgRedirect.Visible =
                                                                                            imgStrike.Visible =
                                                                                            imgSub.Visible =
                                                                                            imgSup.Visible =
                                                                                            imgComment.Visible =
                                                                                            value;
                showHideEditToolbarToolStripMenuItem.Checked = value;
            }
            get { return imgBold.Visible; }
        }

        private void showHideEditToolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditToolBarVisible = !showHideEditToolbarToolStripMenuItem.Checked;
        }
        #endregion

        #region various menus and event handlers
        private void txtFind_MouseHover(object sender, EventArgs e)
        {
            ToolTip.SetToolTip(txtFind, txtFind.Text);
        }

        private void btnWatch_Click(object sender, EventArgs e)
        {
            if (TheArticle == null)
            {
                DisableButtons();
                return;
            }

            if (TheSession.Editor.IsActive)
                return;

            btnWatch.Enabled = false;

            if (PageWatched)
                TheSession.Editor.Unwatch(TheArticle.Name);
            else
                TheSession.Editor.Watch(TheArticle.Name);

            PageWatched = !PageWatched;
            btnWatch.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        private bool PageWatched
        {
            get { return btnWatch.Text != "Watch"; }
            set { btnWatch.Text = value ? "Unwatch" : "Watch"; }
        }

        private static int CompareRegexPairs(KeyValuePair<int, string> x, KeyValuePair<int, string> y)
        {
            return x.Key.CompareTo(y.Key) * -1;
        }

        private void profileTyposToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #if DEBUG
            if (RegexTypos == null)
            {
                MessageBox.Show("No typos loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<KeyValuePair<Regex, string>> typos = RegexTypos.GetTypos();
            if (!typos.Any())
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

            if (MessageBox.Show("Test typo rules for performance (this takes up to 5 minutes)?",
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

            times.Sort(CompareRegexPairs);

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Profiling " + iterations + @" iterations of """ + TheArticle.Name + @"""");

            foreach (KeyValuePair<int, string> p in times) builder.AppendLine(p.ToString());

            Tools.WriteTextFile(builder, "typos.txt", false);

            MessageBox.Show("Results are saved in the file 'typos.txt'", "Profiling complete",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
            #endif
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

        private void cEvalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (CSharpEval cs = new CSharpEval())
            {
                cs.ShowDialog();
            }
        }

        private void ListMakerSourceSelectHandler(object sender, EventArgs e)
        {
            toolStripSeparatorMakeFromTextBox.Visible = listMaker.cmboSourceSelect.Text.Contains("Category");
        }

        private void externalProcessingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExtProgram.Show();
        }

        private readonly CategoryNameForm _catName = new CategoryNameForm();

        private void categoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dires = _catName.ShowDialog();

            if (string.IsNullOrEmpty(_catName.CategoryName) || !dires.Equals(DialogResult.OK))
                return;

            bool pageExists;

            // attempt validation of the category's existence, warn user if it doesn't exist
            try
            {
                // TODO:ApiEdit PageExists/similar function (wrapper for this, we don't need/care about page text)
                IApiEdit editor = TheSession.Editor.SynchronousEditor.Clone();
                editor.Open(_catName.CategoryName, false);

                pageExists = editor.Page.Exists;
            }
            catch
            {
                MessageBox.Show("Unable to check category existence");
                return;
            }

            if (pageExists ||
                MessageBox.Show(_catName.CategoryName + " does not exist. Add it to the page anyway?",
                                "Non-existent category", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                txtEdit.Text += "\r\n\r\n[[" + _catName.CategoryName + "]]";

                // remove any {{uncategorised}} tag now – tagger still counts categories based on saved page revision
                txtEdit.Text = WikiRegexes.Uncat.Replace(txtEdit.Text, "");

                ReparseEditBox();
            }
        }

        private void UsageStatsMenuItem_Click(object sender, EventArgs e)
        {
            UsageStats.OpenUsageStatsURL();
        }

        private void StartProgressBar()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(StartProgressBar));
                return;
            }

            MainFormProgressBar.MarqueeAnimationSpeed = 100;
            MainFormProgressBar.Style = ProgressBarStyle.Marquee;
        }

        private void StopProgressBar()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(StopProgressBar));
                return;
            }

            MainFormProgressBar.MarqueeAnimationSpeed = 0;
            MainFormProgressBar.Style = ProgressBarStyle.Continuous;
        }

        private void BotImage_Click(object sender, EventArgs e)
        {
            Tools.OpenURLInBrowser("https://commons.wikimedia.org/wiki/File:Crystal_Clear_action_run.png");
        }

        private void displayfalsePositivesButtonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddIgnoredToLogFile = displayfalsePositivesButtonToolStripMenuItem.Checked;
        }

        private void HighlightErrors(SortedDictionary<int, int> errors)
        {
            // performance: only highlight first 100 errors
            int done = 0;
            foreach (KeyValuePair<int, int> a in errors)
            {
                RedSelection(a.Key, a.Value);
                done++;
                
                if (done > 100)
                    break;
            }

            // If any text highlighted, don't leave last text selected
            if (done > 0)
                txtEdit.Select(0, 0);
        }

        private void RedSelection(int index, int length)
        {
            if (!txtEdit.Enabled)
                return;

            // numbers in articleText and txtEdit.Edit are offset by the number of newlines before the index of the text
            int newlinesToIndex = WikiRegexes.Newline.Matches(txtEdit.Text.Substring(0, index)).Count;
            int newlinesInSelection = WikiRegexes.Newline.Matches(txtEdit.Text.Substring(index, length)).Count;
            txtEdit.SetEditBoxSelection(index - newlinesToIndex, length - newlinesInSelection, false);
            txtEdit.SelectionBackColor = Color.Tomato;
        }

        private void YellowSelection(int index, int length)
        {
            txtEdit.SetEditBoxSelection(index, length);
            txtEdit.SelectionBackColor = Color.Yellow;
        }

        private void HighlightAllFind()
        {
            if (string.IsNullOrEmpty(txtFind.Text))
                return;

            Dictionary<int, int> found = txtEdit.FindAll(txtFind.Text, chkFindRegex.Checked, chkFindCaseSensitive.Checked, TheArticle.Name);

            foreach (KeyValuePair<int, int> a in found)
                YellowSelection(a.Key, a.Value);

            txtEdit.SetEditBoxSelection(0, 0);
            txtEdit.Select(0, 0);
            txtEdit.ScrollToCaret();
        }

        private void openXML_FileOk(object sender, CancelEventArgs e)
        {
            if (openXML.FileName.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache)))
            {
                // What, no <big>, <font color="red"> and <blink>?
                MessageBox.Show(this, "Please review the custom module code and save the config on your PC manually.\r\n"
                                + "DON'T TRUST ANYTHING YOU FIND ON THE INTERNET UNLESS YOU UNDERSTAND WHAT IT DOES.\r\n"
                                + "Failure to abide by this may result in arbitrary code execution on your machine.",
                                "Security warning - READ THIS", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                e.Cancel = true;
            }
        }

        private void invalidateCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectCache.Global.Invalidate();
        }

        private void clearCurrentListToolStripMenuItem_Click(object sender, EventArgs e)
        {
        	if (listMaker.Any() && MessageBox.Show(this, "Do you want to clear the current list?", "Clear current list", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
                listMaker.Clear();
        }

        private void submitStatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UsageStats.Do(false);
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TheSession.IsBusy)
                TheSession.Editor.Abort();

            TheArticle = null;
            txtEdit.Text = "";

            TheSession.Editor.Logout();

            CheckStatus(true);

            UpdateStatusUI();

            StopProgressBar();
            DisableButtons();
        }

        private void lblUserName_Click(object sender, EventArgs e)
        {
            Profiles.ShowDialog(this);
        }
        
        private void lblUserNotifications_Click(object sender, EventArgs e)
        {
            if (Variables.NotificationsEnabled && TheSession.User.IsLoggedIn)
                Tools.OpenArticleInBrowser("Special:Notifications");
        }

        private void statusBar_MouseHover(object sender, EventArgs e)
        {
            AWBToolTip tt = new AWBToolTip();

            ToolStripStatusLabel item = (sender as ToolStripStatusLabel);

            string text = "";

            switch (item.Name)
            {
            case "lblUserName":
                text = "Click to switch user";
                break;
            case "lblProject":
                text = "Click to switch project";
                break;
            case "lblUserNotifications":
                text = "User notifications";
                break;
            }

            tt.Show(text, item.Owner);
        }
        
        private void editToolBar_MouseHover(object sender, EventArgs e)
        {
            AWBToolTip tt = new AWBToolTip();

            ToolStripButton item = (sender as ToolStripButton);

            string text = "";

            switch (item.Name)
            {
            case "btntsDelete":
                text = "Delete this page";
                break;
            case "btntsIgnore":
                text = "Skip this page without saving and continue on the next";
                break;
            case "btntsSave":
                text = "Save your changes and continue";
                break;
            case "btntsChanges":
                text = "Preview your changes; please use this before saving.";
                break;
            case "btntsPreview":
                text = "Preview your changes";
                break;
            case "btntsStop":
                text = "Stops everything";
                break;
            case "btntsStart":
                text = "Start processing pages";
                break;
            case "btntsShowHideParameters":
                text = "Make the edit box span bottom of window";
                break;
            case "btntsShowHide":
                text = "Show or hide the panel";
                break;
            case "btntsFalsePositive":
                text = "Add to false positives file";
                break;
            }

            tt.Show(text, item.Owner);
        }

        private void lblProject_Click(object sender, EventArgs e)
        {
             OpenPreferences(true);
        }
    }
    #endregion
}
