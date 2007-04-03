/*
Autowikibrowser
Copyright (C) 2006 Martin Richards
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
using WikiFunctions.Browser;
using WikiFunctions.Controls;
using System.Collections.Specialized;
using WikiFunctions.Background;

[assembly: CLSCompliant(true)]
namespace AutoWikiBrowser
{
    public partial class MainForm : Form, IAWBMainForm
    {
        #region constructor etc.

        public MainForm()
        {
            InitializeComponent();

            try
            {
                lblUserName.Alignment = ToolStripItemAlignment.Right;
                lblProject.Alignment = ToolStripItemAlignment.Right;
                lblTimer.Alignment = ToolStripItemAlignment.Right;
                lblEditsPerMin.Alignment = ToolStripItemAlignment.Right;
                lblIgnoredArticles.Alignment = ToolStripItemAlignment.Right;
                lblEditCount.Alignment = ToolStripItemAlignment.Right;

                btntsShowHide.Image = Resources.btnshowhide_image;
                btntsShowHideParameters.Image = Resources.btnshowhideparameters_image;
                btntsSave.Image = Resources.btntssave_image;
                btntsIgnore.Image = Resources.GoLtr;
                btntsStop.Image = Resources.Stop;
                btntsPreview.Image = Resources.preview;
                btntsChanges.Image = Resources.changes;
                btntsFalsePositive.Image = Resources.RolledBack;
                btntsStart.Image = Resources.Run;

                //btnSave.Image = Resources.btntssave_image;
                //btnIgnore.Image = Resources.GoLtr;

                //btnDiff.Image = Resources.changes;
                //btnPreview.Image = Resources.preview;

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
                    MessageBox.Show(ex.Message);
                }

                toolStripComboOnLoad.SelectedIndex = 0;
                cmboCategorise.SelectedIndex = 0;
                cmboImages.SelectedIndex = 0;
                lblStatusText.AutoSize = true;
                lblBotTimer.AutoSize = true;

                Variables.User.UserNameChanged += UpdateUserName;
                Variables.User.BotStatusChanged += UpdateBotStatus;
                Variables.User.AdminStatusChanged += UpdateAdminStatus;

                Variables.User.webBrowserLogin.DocumentCompleted += web4Completed;
                Variables.User.webBrowserLogin.Navigating += web4Starting;

                webBrowserEdit.Loaded += CaseWasLoad;
                webBrowserEdit.Diffed += CaseWasDiff;
                webBrowserEdit.Saved += CaseWasSaved;
                webBrowserEdit.None += CaseWasNull;
                webBrowserEdit.Fault += StartDelayedRestartTimer;
                webBrowserEdit.StatusChanged += UpdateWebBrowserStatus;

                listMaker1.BusyStateChanged += SetProgressBar;
                listMaker1.NoOfArticlesChanged += UpdateButtons;
                listMaker1.StatusTextChanged += UpdateListStatus;
                Text = "AutoWikiBrowser - Default.xml";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        bool Abort = false;

        string LastArticle = "";
        string SettingsFile = "";
        string LastMove = "";
        string LastDelete = "";
        string skipReason = "";

        int oldselection = 0;

        int mnudges = 0;
        int sameArticleNudges = 0;

        bool boolSaved = true;
        HideText RemoveText = new HideText(false, true, false);
        List<string> noParse = new List<string>();
        FindandReplace findAndReplace = new FindandReplace();
        SubstTemplates substTemplates = new SubstTemplates();
        RegExTypoFix RegexTypos;
        SkipOptions Skip = new SkipOptions();
        WikiFunctions.MWB.ReplaceSpecial replaceSpecial = new WikiFunctions.MWB.ReplaceSpecial();
        Parsers parsers;
        TimeSpan StartTime = new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        StringCollection RecentList = new StringCollection();
        CustomModule cModule = new CustomModule();
        public RegexTester regexTester = new RegexTester();
        AWBLogListener LogListener;

        private void MainForm_Load(object sender, EventArgs e)
        {
            // hide this tab until it's fully written
            //tabControl1.TabPages.Remove(tpDab);

            //check that we are not using an old OS. 98 seems to mangled some unicode

            updateUpdater();

            //splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            try
            {
                if (Environment.OSVersion.Version.Major < 5)
                {
                    MessageBox.Show("You appear to be using an older operating system, this software may have trouble with some unicode fonts on operating systems older than Windows 2000, the start button has been disabled.", "Operating system", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetStartButton(false);
                }
                else
                    listMaker1.MakeListEnabled = true;

                if (AutoWikiBrowser.Properties.Settings.Default.LogInOnStart)
                    CheckStatus();

                Debug();
                LoadPlugins();
                LoadPrefs();
                UpdateButtons();
                LoadRecentSettingsList();

                if (Variables.User.checkEnabled() == WikiStatusResult.OldVersion)
                    oldVersion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if ((Minimize) && (this.WindowState == FormWindowState.Minimized))
            {
                this.Visible = false;
            }
            else
            {
                //splitContainer1.
            }
        }

        #endregion

        #region Properties

        Article stredittingarticle = new Article("");
        public Article EdittingArticle
        {
            get { return stredittingarticle; }
            private set { stredittingarticle = value; }
        }

        int intEdits = 0;
        private int NumberOfEdits
        {
            get { return intEdits; }
            set
            {
                intEdits = value;
                lblEditCount.Text = "Edits: " + value.ToString();

            }
        }

        int intIgnoredEdits = 0;
        private int NumberOfIgnoredEdits
        {
            get { return intIgnoredEdits; }
            set
            {
                intIgnoredEdits = value;
                lblIgnoredArticles.Text = "Ignored: " + value.ToString();
            }
        }

        int intEditsPerMin = 0;
        private int NumberOfEditsPerMinute
        {
            get { return intEditsPerMin; }
            set
            {
                intEditsPerMin = value;
                lblEditsPerMin.Text = "Edits/min: " + value.ToString();
            }
        }

        bool bLowThreadPriority = false;
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

        //bool bFlashAndBeep = true;
        private bool FlashAndBeep
        {
            //get { return bFlashAndBeep; }
            set { bFlash = value; bBeep = value; }
        }

        bool bFlash = false;
        private bool Flash
        {
            get { return bFlash; }
            set { bFlash = value; }
        }

        bool bBeep = false;
        private bool Beep
        {
            get { return bBeep; }
            set { bBeep = value; }
        }

        bool bMinimize = false;
        private bool Minimize
        {
            get { return bMinimize; }
            set { bMinimize = value; }
        }

        decimal dTimeOut = 30;
        private decimal TimeOut
        {
            get { return dTimeOut; }
            set
            {
                dTimeOut = value;
                webBrowserEdit.TimeoutLimit = int.Parse(value.ToString());
            }
        }

        bool bSaveArticleList = false;
        private bool SaveArticleList
        {
            get { return bSaveArticleList; }
            set { bSaveArticleList = value; }
        }

        bool bOverrideWatchlist = false;
        private bool OverrideWatchlist
        {
            get { return bOverrideWatchlist; }
            set { bOverrideWatchlist = value; }
        }

        bool bAutoSaveEdit = false;
        private bool AutoSaveEditBoxEnabled
        {
            get { return bAutoSaveEdit; }
            set { bAutoSaveEdit = value; }
        }

        string sAutoSaveEditFile = "Edit Box.txt";
        private string AutoSaveEditBoxFile
        {
            get { return sAutoSaveEditFile; }
            set { sAutoSaveEditFile = value; }
        }

        decimal dAutoSaveEditPeriod = 60;
        private decimal AutoSaveEditBoxPeriod
        {
            get { return dAutoSaveEditPeriod; }
            set { dAutoSaveEditPeriod = value; EditBoxSaveTimer.Interval = int.Parse((value * 1000).ToString()); }
        }

        #endregion

        #region MainProcess

        private void Start()
        {
            try
            {
                Tools.WriteDebug(this.Name, "Starting");

                //check edit summary
                if (cmboEditSummary.Text == "" && AWBPlugins.Count == 0)
                    MessageBox.Show("Please enter an edit summary.", "Edit summary", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                StopDelayedRestartTimer();
                DisableButtons();
                EdittingArticle.EditSummary = "";
                skippable = true;
                txtEdit.Clear();

                if (webBrowserEdit.IsBusy)
                    webBrowserEdit.Stop();

                if (webBrowserEdit.Document != null)
                    webBrowserEdit.Document.Write("");

                //check we are logged in
                if (!Variables.User.WikiStatus && !CheckStatus())
                    return;

                ArticleInfo(true);

                if (listMaker1.NumberOfArticles < 1)
                {
                    webBrowserEdit.Busy = false;
                    stopSaveInterval();
                    lblTimer.Text = "";
                    lblStatusText.Text = "No articles in list, you need to use the Make list";
                    this.Text = "AutoWikiBrowser";
                    webBrowserEdit.Document.Write("");
                    listMaker1.MakeListEnabled = true;
                    return;
                }
                else
                {
                    webBrowserEdit.Busy = true;
                }

                EdittingArticle = listMaker1.SelectedArticle();

                LogListener = new AWBLogListener(EdittingArticle.Name);

                if (!Tools.IsValidTitle(EdittingArticle.Name))
                {
                    SkipPage("Invalid page title");
                    return;
                }
                if (chkAutoMode.Checked)
                    SaveTimer.Start();

                if (AutoSaveEditBoxEnabled)
                    EditBoxSaveTimer.Enabled = true;

                //Navigate to edit page
                webBrowserEdit.LoadEditPage(EdittingArticle.Name);
            }
            catch (Exception ex)
            {
                Tools.WriteDebug(this.Name, "Start() error: " + ex.Message);
                StartDelayedRestartTimer();
            }
        }

        private void CaseWasLoad()
        {
            string strText = "";

            if (!loadSuccess())
                return;

            strText = webBrowserEdit.GetArticleText();

            this.Text = "AutoWikiBrowser" + SettingsFile + " - " + EdittingArticle.Name;

            //check not in use
            if (Regex.IsMatch(strText, "\\{\\{[Ii]nuse"))
            {
                if (!chkAutoMode.Checked)
                    MessageBox.Show("This page has the \"Inuse\" tag, consider skipping it");
            }

            //check for redirect
            if (bypassRedirectsToolStripMenuItem.Checked && Tools.IsRedirect(strText))
            {
                Article Redirect = new Article(Tools.RedirectTarget(strText));

                if (Redirect.Name == EdittingArticle.Name)
                {//ignore recursice redirects
                    SkipPage("Recursive redirect");
                    return;
                }

                listMaker1.ReplaceArticle(EdittingArticle, Redirect);
                EdittingArticle = Redirect;

                webBrowserEdit.LoadEditPage(Redirect.Name);
                return;
            }

            //LogListener = new AWBLogListener(EdittingArticle.Name);

            if (chkSkipIfContains.Checked && Skip.SkipIfContains(strText, EdittingArticle.Name,
            txtSkipIfContains.Text, chkSkipIsRegex.Checked, chkSkipCaseSensitive.Checked, true))
            {
                SkipPage("Article contains: " + txtSkipIfContains.Text);
                return;
            }

            if (chkSkipIfNotContains.Checked && Skip.SkipIfContains(strText, EdittingArticle.Name,
            txtSkipIfNotContains.Text, chkSkipIsRegex.Checked, chkSkipCaseSensitive.Checked, false))
            {
                SkipPage("Article doesn not contain: " + txtSkipIfNotContains.Text);
                return;
            }

            if (!Skip.skipIf(strText))
            {
                //TODO: Set Skip Reason
                SkipPage("");
                return;
            }

            bool skip = false;
            if (!doNotAutomaticallyDoAnythingToolStripMenuItem.Checked)
            {
                string strOrigText = strText;
                strText = Process(strText, out skip);

                if (!Abort && skippable && chkSkipNoChanges.Checked && strText == strOrigText)
                {
                    SkipPage("No changes made");
                    return;
                }
            }

            if (!Abort && skip)
            {
                SkipPage(skipReason);
                return;
            }

            webBrowserEdit.SetArticleText(strText);
            string SavedSummary = EdittingArticle.EditSummary;
            txtEdit.Text = strText;
            EdittingArticle.EditSummary = SavedSummary;

            //Update statistics and alerts
            ArticleInfo(false);

            if (!Abort)
            {
                if (chkAutoMode.Checked && chkQuickSave.Checked)
                    startDelayedAutoSaveTimer();
                else if (toolStripComboOnLoad.SelectedIndex == 0)
                    GetDiff();
                else if (toolStripComboOnLoad.SelectedIndex == 1)
                    GetPreview();
                else if (toolStripComboOnLoad.SelectedIndex == 2)
                {
                    if (chkAutoMode.Checked)
                    {
                        startDelayedAutoSaveTimer();
                        return;
                    }

                    bleepflash();

                    this.Focus();
                    txtEdit.Focus();
                    txtEdit.SelectionLength = 0;

                    EnableButtons();
                }
            }
            else
            {
                EnableButtons();
                Abort = false;
            }
            
        }

        private void bleepflash()
        {
            if (!this.ContainsFocus && (Beep && Flash))
            {
                Tools.FlashWindow(this);
                Tools.Beep1();
            }
            else if (!this.ContainsFocus && Flash)
            {
                Tools.FlashWindow(this);
            }
            else if (!this.ContainsFocus && Beep)
            {
                Tools.Beep1();
            }
        }

        private bool loadSuccess()
        {
            try
            {
                string HTML = webBrowserEdit.Document.Body.InnerHtml;
                if (HTML.Contains("The Wikipedia database is currently locked, and is not accepting any edits or other modifications."))
                {//http://en.wikipedia.org/wiki/MediaWiki:Readonlytext
                    StartDelayedRestartTimer();
                    Start();
                    return false;
                }

                if (HTML.Contains("readOnly"))
                {
                    if (Variables.User.IsAdmin)
                    {
                        return true;
                    }
                    else
                    {
                        SaveTimer.Stop();
                        SkipPage("Page is protected");
                        return false;
                    }
                }
                //check we are still logged in
                if (!webBrowserEdit.GetLogInStatus())
                {
                    Variables.User.LoggedIn = false;
                    SaveTimer.Stop();
                    Start();
                    return false;
                
                }

                if (webBrowserEdit.NewMessage)
                {//check if we have any messages
                    SaveTimer.Stop();
                    Variables.User.WikiStatus = false;
                    UpdateButtons();
                    webBrowserEdit.Document.Write("");
                    this.Focus();

                    dlgTalk DlgTalk = new dlgTalk();
                    if (DlgTalk.ShowDialog() == DialogResult.Yes)
                        System.Diagnostics.Process.Start(Variables.URLLong + "index.php?title=User_talk:" + Variables.User.Name + "&action=purge");
                    else
                        System.Diagnostics.Process.Start("IExplore", Variables.URLLong + "index.php?title=User_talk:" + Variables.User.Name + "&action=purge");

                    DlgTalk = null;
                    return false;
                }
                if (!webBrowserEdit.HasArticleTextBox)
                {
                    if (!chkAutoMode.Checked)
                    {
                        MessageBox.Show("There was a problem loading the page. Re-start the process", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    lblStatusText.Text = "There was a problem loading the page. Re-starting.";
                    StartDelayedRestartTimer();
                    return false;
                }
                if (webBrowserEdit.Document.GetElementById("wpTextbox1").InnerText == null && chkSkipNonExistent.Checked)
                {//check if it is a non-existent page, if so then skip it automatically.
                    SkipPage("Non-existent page");
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return true;
        }

        bool skippable = true;
        private void CaseWasDiff()
        {
            if (diffChecker(webBrowserEdit.Document.Body.InnerHtml))
            {//check if there are no changes and we want to skip
                SkipPage("No changes made");
                return;
            }

            if (chkAutoMode.Checked)
            {
                startDelayedAutoSaveTimer();
                return;
            }

            bleepflash();

            this.Focus();            
            txtEdit.Focus();
            txtEdit.SelectionLength = 0;
            
            EnableButtons();
        }

        private bool diffChecker(string strHTML)
        {//check diff to see if it should be skipped
            
            if (!skippable || !chkSkipNoChanges.Checked || toolStripComboOnLoad.SelectedIndex != 0 || doNotAutomaticallyDoAnythingToolStripMenuItem.Checked)
                return false;

            //if (!strHTML.Contains("class=diff-context") && !strHTML.Contains("class=diff-deletedline"))
            //    return true;

            strHTML = strHTML.Replace("<SPAN class=diffchange></SPAN>", "");
            strHTML = Regex.Match(strHTML, "<TD align=left colSpan=2.*?</DIV>", RegexOptions.Singleline).Value;

            //check for no changes, or no new lines (that have text on the new line)
            if (strHTML.Contains("<SPAN class=diffchange>") || Regex.IsMatch(strHTML, "class=diff-deletedline>[^<]") || Regex.IsMatch(strHTML, "<TD colSpan=2>&nbsp;</TD>\r\n<TD>\\+</TD>\r\n<TD class=diff-addedline>[^<]"))
                return false;

            return true;
        }

        private void CaseWasSaved()
        {
            if (webBrowserEdit.Document.Body.InnerHtml.Contains("<H1 class=firstHeading>Edit conflict: "))
            {//if session data is lost, if data is lost then save after delay with tmrAutoSaveDelay
                MessageBox.Show("There has been an Edit Conflict. AWB will now re-apply its changes on the updated page. \n\r Please re-review the changes before saving. Any Custom edits will be lost, and have to be re-added manually.", "Edit Conflict");
                SaveTimer.Stop();
                Start();
                return;
            }
            else if (!chkAutoMode.Checked && webBrowserEdit.Document.Body.InnerHtml.Contains("<A class=extiw title=m:spam_blacklist href=\"http://meta.wikimedia.org/wiki/spam_blacklist\">"))
            {//check edit wasn't blocked due to spam filter
                if (MessageBox.Show("Edit has been blocked by spam blacklist. Try and edit again?", "Spam blacklist", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    Start();
                    return;
                }
            }
            else if (webBrowserEdit.Document.Body.InnerHtml.Contains("<DIV CLASS=PREVIEWNOTE"))
            {//if session data is lost, if data is lost then save after delay with tmrAutoSaveDelay
                StartDelayedRestartTimer();
                return;
            }

            //lower restart delay
            if (intRestartDelay > 5)
                intRestartDelay -= 1;

            NumberOfEdits++;

            LastArticle = "";
            listMaker1.Remove(EdittingArticle);
            //lvSaved.Items.Add(DateTime.Now.ToLongTimeString() + " : " + EdittingArticle.Name);
            SaveTimer.Stop();
            sameArticleNudges = 0;

            lvSaved.Items.Insert(0, LogListener);
            resizeListView(lvSaved);

            Start();
        }

        private void CaseWasNull()
        {
            if (webBrowserEdit.Document.Body.InnerHtml.Contains("<B>You have successfully signed in to Wikipedia as"))
            {
                lblStatusText.Text = "Signed in, now re-starting";

                if (!Variables.User.WikiStatus)
                {
                    CheckStatus();
                }
            }
        }

        private void SkipPage(string reason)
        {
            try
            {
                //reset timer.
                NumberOfIgnoredEdits++;
                stopDelayedAutoSaveTimer();
                SaveTimer.Stop();
                listMaker1.Remove(EdittingArticle);
                //lvIgnored.Items.Add(DateTime.Now.ToLongTimeString() + " : " + EdittingArticle.Name);
                sameArticleNudges = 0;

                switch (reason)
                {
                    case "user":
                        LogListener.UserSkipped();
                        break;

                    case "plugin":
                        LogListener.PluginSkipped();
                        break;

                    case "": break;

                    default:
                        LogListener.AWBSkipped(reason);
                        break;
                }

                lvIgnored.Items.Insert(0, LogListener);
                resizeListView(lvIgnored);
                
                Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string Process(string articleText, out bool SkipArticle)
        {
            bool process = true;

            try
            {
                if (noParse.Contains(EdittingArticle.Name))
                    process = false;

                // {{bots}}/{{nobots}}
                if (!ignoreNoBotsToolStripMenuItem.Checked && !Parsers.CheckNoBots(articleText, Variables.User.Name))
                {
                    SkipArticle = true;
                    skipReason = "Bot Edits not Allowed";
                    return articleText;
                }

                if (cModule.ModuleEnabled && cModule.Module != null)
                {
                    string tempSummary = "";
                    articleText = cModule.Module.ProcessArticle(articleText, EdittingArticle.Name, EdittingArticle.NameSpaceKey, out tempSummary, out SkipArticle);
                    if (SkipArticle)
                        return articleText;
                    else if (tempSummary.Length > 0)
                    {
                        EdittingArticle.EditSummary += " " + tempSummary.Trim();
                    }
                }

                if (AWBPlugins.Count > 0)
                {
                    SkipArticle = false;
                    string tempSummary = "";

                    foreach (KeyValuePair<string, IAWBPlugin> a in AWBPlugins)
                    {
                        tempSummary = "";

                        articleText = a.Value.ProcessArticle(articleText, EdittingArticle.Name, EdittingArticle.NameSpaceKey, out tempSummary, out SkipArticle, LogListener);
                        if (SkipArticle)
                            return articleText;
                        else if (tempSummary.Length > 0)
                        {
                            EdittingArticle.EditSummary += " " + tempSummary.Trim();
                        }
                    }
                }

                if (chkUnicodifyWhole.Checked && process)
                {
                    articleText = parsers.Unicodify(articleText, out SkipArticle);
                    if (Skip.SkipNoUnicode && SkipArticle)
                    {
                        skipReason = "No Unicodification";
                        return articleText;
                    }
                }

                if (cmboImages.SelectedIndex == 1)
                {
                    articleText = parsers.ReplaceImage(txtImageReplace.Text, txtImageWith.Text, articleText, out SkipArticle);
                    if (SkipArticle && chkSkipNoImgChange.Checked)
                    {
                        skipReason = "No Image Changed";
                        return articleText;
                    }
                }
                else if (cmboImages.SelectedIndex == 2)
                {
                    articleText = parsers.RemoveImage(txtImageReplace.Text, articleText, false, txtImageWith.Text, out SkipArticle);
                    if (SkipArticle && chkSkipNoImgChange.Checked)
                    {
                        skipReason = "No Image Changed";
                        return articleText;
                    }
                }
                else if (cmboImages.SelectedIndex == 3)
                {
                    articleText = parsers.RemoveImage(txtImageReplace.Text, articleText, true, txtImageWith.Text, out SkipArticle);
                    if (SkipArticle && chkSkipNoImgChange.Checked)
                    {
                        skipReason = "No Image Changed";
                        return articleText;
                    }
                }

                if (cmboCategorise.SelectedIndex == 1)
                {
                    articleText = parsers.ReCategoriser(txtNewCategory.Text, txtNewCategory2.Text, articleText, out SkipArticle);
                    if (SkipArticle && chkSkipNoCatChange.Checked)
                    {
                        skipReason = "No Category Changed";
                        return articleText;
                    }
                }
                else if (cmboCategorise.SelectedIndex == 2 && txtNewCategory.Text.Length > 0)
                {
                    articleText = parsers.AddCategory(txtNewCategory.Text, articleText, EdittingArticle.Name);
                }
                else if (cmboCategorise.SelectedIndex == 3 && txtNewCategory.Text.Length > 0)
                {
                    articleText = parsers.RemoveCategory(txtNewCategory.Text, articleText, out SkipArticle);
                    if (SkipArticle && chkSkipNoCatChange.Checked)
                    {
                        skipReason = "No Category Changed";
                        return articleText;
                    }
                }

                if (chkFindandReplace.Checked && !findAndReplace.AfterOtherFixes)
                {
                    SkipArticle = false;
                    articleText = PerformFindAndReplace(articleText, out SkipArticle);
                    if (SkipArticle)
                    {
                        skipReason = "No Find And Replace Changes";
                        return articleText;
                    }
                }

                if (chkRegExTypo.Checked && RegexTypos != null && !chkAutoMode.Checked && !Tools.IsTalkPage(EdittingArticle.NameSpaceKey))
                {
                    string tempSummary = "";
                    articleText = RegexTypos.PerformTypoFixes(articleText, out SkipArticle, out tempSummary);
                    if (SkipArticle && chkSkipIfNoRegexTypo.Checked)
                    {
                        skipReason = "No Typo Fixes";
                        return articleText;
                    }
                    else
                        EdittingArticle.EditSummary += tempSummary;
                }

                if (EdittingArticle.NameSpaceKey == 0 || EdittingArticle.NameSpaceKey == 14 || EdittingArticle.Name.Contains("Sandbox") || EdittingArticle.Name.Contains("Sandbox"))
                {
                    if (process && chkAutoTagger.Checked)
                    {
                        articleText = parsers.Tagger(articleText, EdittingArticle.Name, out SkipArticle, ref EdittingArticle.EditSummary);
                        if (Skip.SkipNoTag && SkipArticle)
                        {
                            skipReason = "No Tag changed";
                            return articleText;
                        }
                    }

                    if (process && chkGeneralFixes.Checked)
                    {
                        articleText = RemoveText.Hide(articleText);

                        if (Variables.LangCode == LangCodeEnum.en)
                        {//en only
                            articleText = parsers.Conversions(articleText);
                            //articleText = parsers.FixHeadings(articleText);
                            articleText = parsers.FixDates(articleText);
                            articleText = parsers.FixFootnotes(articleText);
                            articleText = parsers.LivingPeople(articleText, out SkipArticle);
                            articleText = parsers.ChangeToDefaultSort(articleText, EdittingArticle.Name);
                            articleText = parsers.FixHeadings(articleText, EdittingArticle.Name, out SkipArticle);
                            if (Skip.SkipNoHeaderError && SkipArticle)
                            {
                                skipReason = "No Header Errors";
                                return articleText;
                            }
                        }
                        articleText = parsers.FixCategories(articleText);
                        articleText = parsers.FixImages(articleText);
                        articleText = parsers.FixSyntax(articleText);

                        articleText = parsers.FixLinks(articleText, out SkipArticle);
                        if (Skip.SkipNoBadLink && SkipArticle)
                        {
                            skipReason = "No Bad Links";
                            return articleText;
                        }

                        articleText = parsers.BulletExternalLinks(articleText, out SkipArticle);
                        if (Skip.SkipNoBulletedLink && SkipArticle)
                        {
                            skipReason = "No Missing Bulleted Links";
                            return articleText;
                        }

                        articleText = parsers.SortMetaData(articleText, EdittingArticle.Name);

                        //articleText = RemoveText.HideMore(articleText);
                        articleText = parsers.BoldTitle(articleText, EdittingArticle.Name, out SkipArticle);
                        //articleText = RemoveText.AddBackMore(articleText);
                        if (Skip.SkipNoBoldTitle && SkipArticle)
                        {
                            skipReason = "No Titles to bolden";
                            return articleText;
                        }

                        articleText = parsers.StickyLinks(parsers.SimplifyLinks(articleText));

                        articleText = RemoveText.AddBack(articleText);
                    }
                }
                else if (process && chkGeneralFixes.Checked && EdittingArticle.NameSpaceKey == 3)
                {
                    articleText = RemoveText.Hide(articleText);
                    articleText = parsers.SubstUserTemplates(articleText, EdittingArticle.Name);
                    articleText = RemoveText.AddBack(articleText);
                }

                if (chkAppend.Checked)
                {
                    if (rdoAppend.Checked)
                        articleText += "\r\n\r\n" + txtAppendMessage.Text;
                    else
                        articleText = txtAppendMessage.Text + "\r\n\r\n" + articleText;
                }

                if (chkFindandReplace.Checked && findAndReplace.AfterOtherFixes)
                {
                    SkipArticle = false;
                    articleText = PerformFindAndReplace(articleText, out SkipArticle);
                    if (SkipArticle)
                    {
                        skipReason = "No Find And Replace Changes";
                        return articleText;
                    }
                }

                if (chkEnableDab.Checked && txtDabLink.Text.Trim() != "" &&
                    txtDabVariants.Text.Trim() != "")
                {
                    SkipArticle = false;
                    DabForm df = new DabForm();
                    articleText = df.Disambiguate(articleText, EdittingArticle.Name, txtDabLink.Text.Trim(), 
                        txtDabVariants.Lines, (int)udContextChars.Value, chkAutoMode.Checked, out SkipArticle);

                    Abort = df.Abort;
                    if (Abort) Stop();

                    if (SkipArticle && chkSkipNoDab.Checked)
                    {
                        skipReason = "No Disambiguation";
                        return articleText;
                    }
                }

                SkipArticle = false;
                return articleText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SkipArticle = false;
                skipReason = "Error";
                return articleText;
            }
        }

        private string PerformFindAndReplace(string articleText, out bool SkipArticle)
        {
            string articleTitle = EdittingArticle.Name;
            string testText = articleText;
            articleText = findAndReplace.MultipleFindAndReplce(articleText, EdittingArticle.Name, ref EdittingArticle.EditSummary);
            articleText = replaceSpecial.ApplyRules(articleText, EdittingArticle.Name);
            articleText = substTemplates.SubstituteTemplates(articleText, articleTitle);

            if (chkSkipWhenNoFAR.Checked && (testText == articleText))
            {
                SkipArticle = true;
                return articleText;
            }
            else
            {
                SkipArticle = false;
                return articleText;
            }
        }

        private void GetDiff()
        {
            webBrowserEdit.SetArticleText(txtEdit.Text);

            DisableButtons();
            LastArticle = txtEdit.Text;

            webBrowserEdit.ShowDiff();
        }

        private void GetPreview()
        {
            webBrowserEdit.SetArticleText(txtEdit.Text);

            DisableButtons();
            LastArticle = txtEdit.Text;

            skippable = false;
            webBrowserEdit.ShowPreview();
        }

        private void Save()
        {
            DisableButtons();
            if (txtEdit.Text.Length > 0)
                SaveArticle();
            else if (MessageBox.Show("Do you really want to save a blank page?", "Save?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                SaveArticle();
            else
                SkipPage("Nothing to save - blank page");
        }

        private void SaveArticle()
        {
            //remember article text in case it is lost, this is set to "" again when the article title is removed
            LastArticle = txtEdit.Text;

            if (showTimerToolStripMenuItem.Checked)
            {
                stopSaveInterval();
                ticker += SaveInterval;
            }

            try
            {
                setCheckBoxes();

                webBrowserEdit.SetArticleText(txtEdit.Text);
                webBrowserEdit.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region extra stuff

        private void panelShowHide()
        {
            if (splitContainer1.Visible)
            { splitContainer1.Hide(); }
            else
            { splitContainer1.Show(); }
            setBrowserSize();
        }

        private void parametersShowHide()
        {
            if (splitContainer1.Panel1Collapsed == false)
                splitContainer1.Panel1Collapsed = true;
            else
                splitContainer1.Panel1Collapsed = false;
        }
        
        private void UpdateUserName(object sender, EventArgs e)
        {
            lblUserName.Text = Variables.User.Name;
        }

        private void UpdateWebBrowserStatus()
        {
            lblStatusText.Text = webBrowserEdit.Status;
        }

        private void UpdateListStatus()
        {
            lblStatusText.Text = listMaker1.Status;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (AutoWikiBrowser.Properties.Settings.Default.DontAskForTerminate)
            {
                // save user persistent settings
                AutoWikiBrowser.Properties.Settings.Default.Save();
                return;
            }
            string msg = "";
            if (boolSaved == false)
                msg = "You have changed the list since last saving it!\r\n";

            TimeSpan Time = new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            Time = Time.Subtract(StartTime);
            ExitQuestion dlg = new ExitQuestion(Time, NumberOfEdits, msg);
            dlg.ShowDialog();
            if (dlg.DialogResult == DialogResult.OK)
            {
                AutoWikiBrowser.Properties.Settings.Default.DontAskForTerminate = dlg.checkBoxDontAskAgain;

                // save user persistent settings
                AutoWikiBrowser.Properties.Settings.Default.Save();
            }
            else
            {
                e.Cancel = true;
                dlg = null;
                return;
            }

            if (webBrowserEdit.IsBusy)
                webBrowserEdit.Stop2();
            if (Variables.User.webBrowserLogin.IsBusy)
                Variables.User.webBrowserLogin.Stop();

            SaveRecentSettingsList();
        }

        private void setCheckBoxes()
        {
            if (webBrowserEdit.Document.Body.InnerHtml.Contains("wpMinoredit"))
            {
                if (markAllAsMinorToolStripMenuItem.Checked)
                    webBrowserEdit.SetMinor(true);
                if (addAllToWatchlistToolStripMenuItem.Checked)
                    webBrowserEdit.SetWatch(true);
                if (!addAllToWatchlistToolStripMenuItem.Checked && bOverrideWatchlist)
                    webBrowserEdit.SetWatch(false);
                webBrowserEdit.SetSummary(MakeSummary());
            }
        }

        private string MakeSummary()
        {
            string tag = cmboEditSummary.Text + EdittingArticle.EditSummary;
            if (!chkAutoMode.Checked || !chkSuppressTag.Checked)
                tag += " " + Variables.SummaryTag;

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
            if (chkAutoMode.Checked)
                chkAutoMode.Checked = Variables.User.IsBot;
            if (chkQuickSave.Checked)
                chkQuickSave.Checked = Variables.User.IsBot;
            lblOnlyBots.Visible = !Variables.User.IsBot;
        }

        private void UpdateAdminStatus(object sender, EventArgs e)
        {
        }

        private void chkAutoMode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoMode.Checked)
            {
                label2.Enabled = true;
                chkSuppressTag.Enabled = true;
                chkQuickSave.Enabled = true;
                nudBotSpeed.Enabled = true;
                lblAutoDelay.Enabled = true;
                btnResetNudges.Enabled = true;
                label3.Enabled = true;
                lblNudges.Enabled = true;
                nudNudgeTime.Enabled = true;
                chkNudge.Enabled = true;
                chkNudgeSkip.Enabled = true;
                chkNudge.Checked = true;
                chkNudgeSkip.Checked = true;

                if (chkRegExTypo.Checked)
                {
                    MessageBox.Show("Auto cannot be used with RegExTypoFix.\r\nRegExTypoFix will now be turned off", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    chkRegExTypo.Checked = false;
                }
            }
            else
            {
                label2.Enabled = false;
                chkSuppressTag.Enabled = false;
                chkQuickSave.Enabled = false;
                nudBotSpeed.Enabled = false;
                lblAutoDelay.Enabled = false;
                btnResetNudges.Enabled = false;
                label3.Enabled = false;
                lblNudges.Enabled = false;
                nudNudgeTime.Enabled = false;
                chkNudge.Enabled = false;
                chkNudgeSkip.Enabled = false;
                stopDelayedAutoSaveTimer();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeSpan Time = new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            Time = Time.Subtract(StartTime);
            AboutBox About = new AboutBox(webBrowserEdit.Version.ToString(), Time, NumberOfEdits);
            About.Show();
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (Variables.User.WikiStatus)
            // {
            CheckStatus();
            //  }
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Would you really like to logout?", "Logout", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                chkAutoMode.Enabled = false;
                chkAutoMode.Checked = false;
                chkQuickSave.Checked = false;
                lblOnlyBots.Visible = true;
                webBrowserEdit.LoadLogOut();
                Variables.User.UpdateWikiStatus();
            }
        }

        private bool CheckStatus()
        {
            lblStatusText.Text = "Loading page to check if we are logged in.";
            WikiStatusResult Result = Variables.User.UpdateWikiStatus();

            bool b = false;
            string label = "Software disabled";

            switch (Result)
            {
                case WikiStatusResult.Error:
                    lblUserName.BackColor = Color.Red;
                    MessageBox.Show("Check page failed to load.\r\n\r\nCheck your Internet Explorer is working and that the Wikipedia servers are online, also try clearing Internet Explorer cache.", "User check problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case WikiStatusResult.NotLoggedIn:
                    lblUserName.BackColor = Color.Red;
                    MessageBox.Show("You are not logged in. The log in screen will now load, enter your name and password, click \"Log in\", wait for it to complete, then start the process again.\r\n\r\nIn the future you can make sure this won't happen by logging in to Wikipedia using Microsoft Internet Explorer.", "Not logged in", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    webBrowserEdit.LoadLogInPage();
                    break;

                case WikiStatusResult.NotRegistered:
                    lblUserName.BackColor = Color.Red;
                    MessageBox.Show(Variables.User.Name + " is not enabled to use this.", "Not enabled", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    System.Diagnostics.Process.Start(Variables.URL + "/wiki/Project:AutoWikiBrowser/CheckPage");

                    break;

                case WikiStatusResult.OldVersion:
                    oldVersion();

                    break;

                case WikiStatusResult.Registered:
                    b = true;
                    label = string.Format("Logged in, user and software enabled. Bot = {0}, Admin = {1}", Variables.User.IsBot, Variables.User.IsAdmin);
                    lblUserName.BackColor = Color.LightGreen;

                    //Get list of articles not to apply general fixes to.
                    Match n = Regex.Match(Variables.User.CheckPageText, "<!--No general fixes:.*?-->", RegexOptions.Singleline);
                    if (n.Success)
                    {
                        foreach (Match link in WikiRegexes.UnPipedWikiLink.Matches(n.Value))
                            if (!noParse.Contains(link.Groups[1].Value))
                                noParse.Add(link.Groups[1].Value);
                    }
                    break;
            }

            lblStatusText.Text = label;
            UpdateButtons();

            return b;
        }

        private void oldVersion()
        {
            lblUserName.BackColor = Color.Red;

            DialogResult yesnocancel = MessageBox.Show("This version is not enabled, please download the newest version. If you have the newest version, check that Wikipedia is online.\r\n\r\nPlease press \"Yes\" to run the AutoUpdater, \"No\" to load the download page and update manually, or \"Cancel\" to not update (but you will not be able to edit).", "Problem", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (yesnocancel == DialogResult.Yes)
                runUpdater();

            if (yesnocancel == DialogResult.No)
                System.Diagnostics.Process.Start("http://sourceforge.net/project/showfiles.php?group_id=158332");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkAppend_CheckedChanged(object sender, EventArgs e)
        {
            txtAppendMessage.Enabled = chkAppend.Checked;
            rdoAppend.Enabled = chkAppend.Checked;
            rdoPrepend.Enabled = chkAppend.Checked;
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
            string ArticleText = txtEdit.Text;
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
                lblInterLinks.Text = "Inter links: ";

                lbDuplicateWikilinks.Items.Clear();
                lblDuplicateWikilinks.Visible = false;
                lbDuplicateWikilinks.Visible = false;
                btnRemove.Visible = false;
            }
            else
            {
                intWords = Tools.WordCount(ArticleText);

                foreach (Match m in Regex.Matches(ArticleText, "\\[\\[" + Variables.Namespaces[14], RegexOptions.IgnoreCase))
                    intCats++;

                foreach (Match m in Regex.Matches(ArticleText, "\\[\\[" + Variables.Namespaces[6], RegexOptions.IgnoreCase))
                    intImages++;

                foreach (Match m in WikiRegexes.InterWikiLinks.Matches(ArticleText))
                    intInterLinks++;

                foreach (Match m in WikiRegexes.WikiLinksOnly.Matches(ArticleText))
                    intLinks++;

                intLinks = intLinks - intInterLinks - intImages - intCats;

                if (EdittingArticle.NameSpaceKey == 0 && (WikiRegexes.Stub.IsMatch(ArticleText)) && (intWords > 500))
                    lblWarn.Text = "Long article with a stub tag.\r\n";

                if (!(Regex.IsMatch(ArticleText, "\\[\\[" + Variables.Namespaces[14], RegexOptions.IgnoreCase)))
                    lblWarn.Text += "No category (although one may be in a template)\r\n";

                if (ArticleText.StartsWith("=="))
                    lblWarn.Text += "Starts with heading.";

                lblWords.Text = "Words: " + intWords.ToString();
                lblCats.Text = "Categories: " + intCats.ToString();
                lblImages.Text = "Images: " + intImages.ToString();
                lblLinks.Text = "Links: " + intLinks.ToString();
                lblInterLinks.Text = "Inter links: " + intInterLinks.ToString();

                //Find multiple links                
                lbDuplicateWikilinks.Items.Clear();
                ArrayList ArrayLinks = new ArrayList();
                string x = "";
                //get all the links
                foreach (Match m in WikiRegexes.WikiLink.Matches(ArticleText))
                {
                    x = m.Groups[1].Value;
                    if (!WikiRegexes.Dates.IsMatch(x) && !WikiRegexes.Dates2.IsMatch(x))
                        ArrayLinks.Add(x);
                }

                lbDuplicateWikilinks.Sorted = true;

                //add the duplicate articles to the listbox
                foreach (string z in ArrayLinks)
                {
                    if ((ArrayLinks.IndexOf(z) < ArrayLinks.LastIndexOf(z)) && (!lbDuplicateWikilinks.Items.Contains(z)))
                        lbDuplicateWikilinks.Items.Add(z);
                }
                ArrayLinks = null;

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
            int selection = lbDuplicateWikilinks.SelectedIndex;
            if (selection != oldselection)
            {
                resetFind();
            }
            if (lbDuplicateWikilinks.SelectedIndex != -1)
            {
                string strLink = Regex.Escape(lbDuplicateWikilinks.SelectedItem.ToString());
                find("\\[\\[" + strLink + "(\\|.*?)?\\]\\]", true, true);
                btnRemove.Enabled = true;
            }
            else
            {
                resetFind();
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

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            resetFind();
        }

        private void chkFindRegex_CheckedChanged(object sender, EventArgs e)
        {
            resetFind();
        }
        private void txtEdit_TextChanged(object sender, EventArgs e)
        {
            resetFind();
            EdittingArticle.EditSummary = "";
        }
        private void chkFindCaseSensitive_CheckedChanged(object sender, EventArgs e)
        {
            resetFind();
        }
        private void resetFind()
        {
            regexObj = null;
            matchObj = null;
        }
        private void btnFind_Click(object sender, EventArgs e)
        {
            lblDone.Text = "";
            find(txtFind.Text, chkFindRegex.Checked, chkFindCaseSensitive.Checked);
        }

        private Regex regexObj;
        private Match matchObj;

        private void find(string strRegex, bool isRegex, bool caseSensive)
        {
            string ArticleText = txtEdit.Text;

            RegexOptions regOptions;

            if (caseSensive)
                regOptions = RegexOptions.None;
            else
                regOptions = RegexOptions.IgnoreCase;

            strRegex = Tools.ApplyKeyWords(EdittingArticle.Name, strRegex);

            if (!isRegex)
                strRegex = Regex.Escape(strRegex);

            if (matchObj == null || regexObj == null)
            {
                int findStart = txtEdit.SelectionStart;

                regexObj = new Regex(strRegex, regOptions);
                matchObj = regexObj.Match(ArticleText, findStart);
                txtEdit.SelectionStart = matchObj.Index;
                txtEdit.SelectionLength = matchObj.Length;
                txtEdit.Focus();
                txtEdit.ScrollToCaret();
                return;
            }
            else
            {
                if (matchObj.NextMatch().Success)
                {
                    matchObj = matchObj.NextMatch();
                    txtEdit.SelectionStart = matchObj.Index;
                    txtEdit.SelectionLength = matchObj.Length;
                    txtEdit.Focus();
                    txtEdit.ScrollToCaret();
                }
                else
                {
                    lblDone.Text = "Done";
                    txtEdit.SelectionStart = 0;
                    txtEdit.SelectionLength = 0;
                    txtEdit.Focus();
                    txtEdit.ScrollToCaret();
                    resetFind();
                }
            }
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
        {//stop logging in when de-bugging
            Tools.WriteDebugEnabled = true;
            listMaker1.Add("Wikipedia:AutoWikiBrowser/Sandbox");
            Variables.User.WikiStatus = true;
            Variables.User.IsBot = true;
            Variables.User.IsAdmin = true;
            chkQuickSave.Enabled = true;
            lblOnlyBots.Visible = false;
            dumpHTMLToolStripMenuItem.Visible = true;
            logOutDebugToolStripMenuItem.Visible = true;
            bypassAllRedirectsToolStripMenuItem.Enabled = true;

            //MessageBox.Show(Tools.MakeHumanCatKey("Peter Boyle III"));
        }

        #endregion

        #region set variables

        private void PreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyPreferences MyPrefs = new MyPreferences(Variables.LangCode, Variables.Project, Variables.CustomProject, webBrowserEdit.EnhanceDiffEnabled, webBrowserEdit.ScrollDown, webBrowserEdit.DiffFontSize, txtEdit.Font, LowThreadPriority, Flash, Beep, Minimize, SaveArticleList, OverrideWatchlist, TimeOut, AutoSaveEditBoxEnabled, AutoSaveEditBoxFile, AutoSaveEditBoxPeriod);

            if (MyPrefs.ShowDialog(this) == DialogResult.OK)
            {
                webBrowserEdit.EnhanceDiffEnabled = MyPrefs.EnhanceDiff;
                webBrowserEdit.ScrollDown = MyPrefs.ScrollDown;
                webBrowserEdit.DiffFontSize = MyPrefs.DiffFontSize;
                txtEdit.Font = MyPrefs.TextBoxFont;
                LowThreadPriority = MyPrefs.LowThreadPriority;
                //FlashAndBeep = MyPrefs.FlashAndBeep;
                Flash = MyPrefs.perfFlash;
                Beep = MyPrefs.perfBeep;
                Minimize = MyPrefs.perfMinimize;
                SaveArticleList = MyPrefs.perfSaveArticleList;
                OverrideWatchlist = MyPrefs.perfOverrideWatchlist;
                TimeOut = MyPrefs.perfTimeOutLimit;

                if (MyPrefs.Language != Variables.LangCode || MyPrefs.Project != Variables.Project || MyPrefs.CustomProject != Variables.CustomProject)
                {
                    SetProject(MyPrefs.Language, MyPrefs.Project, MyPrefs.CustomProject);

                    Variables.User.WikiStatus = false;
                    chkQuickSave.Checked = false;
                    chkAutoMode.Checked = false;
                    lblOnlyBots.Visible = true;
                    Variables.User.IsBot = false;
                    Variables.User.IsAdmin = false;
                }
            }
            MyPrefs = null;

            listMaker1.AddRemoveRedirects();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //refresh typo list
            loadTypos(true);

            //refresh login status, and reload check list
            if (!Variables.User.WikiStatus)
            {
                if (!CheckStatus())
                    return;
            }
        }

        private void SetProject(LangCodeEnum Code, ProjectEnum Project, string CustomProject)
        {
            //set namespaces
            Variables.SetProject(Code, Project, CustomProject);

            //set interwikiorder
            if (Code == LangCodeEnum.en || Code == LangCodeEnum.pl)
                parsers.InterWikiOrder = InterWikiOrderEnum.LocalLanguageAlpha;
            //else if (Code == "fi")
            //    parsers.InterWikiOrder = InterWikiOrderEnum.LocalLanguageFirstWord;
            else
                parsers.InterWikiOrder = InterWikiOrderEnum.Alphabetical;

            if (Code != LangCodeEnum.en || Project != ProjectEnum.wikipedia)
            {
                chkAutoTagger.Checked = false;
                chkGeneralFixes.Checked = false;
            }
            if (Project != ProjectEnum.custom) lblProject.Text = Variables.LangCode.ToString().ToLower() + "." + Variables.Project;
            else lblProject.Text = Variables.URL;
        }

        #endregion

        #region Enabling/Disabling of buttons

        private void UpdateButtons()
        {
            bool enabled = listMaker1.NumberOfArticles > 0;
            SetStartButton(enabled);

            //    btnMove.Visible = Variables.User.IsAdmin;
            //    btnDelete.Visible = Variables.User.IsAdmin;

            listMaker1.ButtonsEnabled = enabled;
            lbltsNumberofItems.Text = "Articles: " + listMaker1.NumberOfArticles.ToString();
            bypassAllRedirectsToolStripMenuItem.Enabled = Variables.User.IsAdmin;
        }

        private void SetStartButton(bool enabled)
        {
            btnStart.Enabled = enabled;
            btntsStart.Enabled = enabled;
        }

        private void DisableButtons()
        {
            SetStartButton(false);

            if (listMaker1.NumberOfArticles == 0)
                btnIgnore.Enabled = false;

            btnPreview.Enabled = false;
            btnDiff.Enabled = false;
            btntsPreview.Enabled = false;
            btntsChanges.Enabled = false;

            listMaker1.MakeListEnabled = false;

            btnSave.Enabled = false;
            btntsSave.Enabled = false;

            btnMove.Enabled = false;
            btnDelete.Enabled = false;

            if (cmboEditSummary.Focused) txtEdit.Focus();
        }

        private void EnableButtons()
        {
            UpdateButtons();
            btnSave.Enabled = true;
            btnIgnore.Enabled = true;
            btnPreview.Enabled = true;
            btnDiff.Enabled = true;
            btntsPreview.Enabled = true;
            btntsChanges.Enabled = true;

            listMaker1.MakeListEnabled = true;

            btntsSave.Enabled = true;
            btntsIgnore.Enabled = true;

            btnMove.Enabled = true;
            btnDelete.Enabled = true;
        }

        #endregion

        #region timers

        int intRestartDelay = 5;
        int intStartInSeconds = 5;
        private void DelayedRestart()
        {
            stopDelayedAutoSaveTimer();
            lblStatusText.Text = "Restarting in " + intStartInSeconds.ToString();

            if (intStartInSeconds == 0)
            {
                StopDelayedRestartTimer();
                Start();
            }
            else
                intStartInSeconds--;
        }
        private void StartDelayedRestartTimer()
        {
            intStartInSeconds = intRestartDelay;
            ticker += DelayedRestart;
            //increase the restart delay each time, this is decreased by 1 on each successfull save
            intRestartDelay += 5;
        }
        private void StartDelayedRestartTimer(int Delay)
        {
            intStartInSeconds = Delay;
            ticker += DelayedRestart;
        }
        private void StopDelayedRestartTimer()
        {
            ticker -= DelayedRestart;
            intStartInSeconds = intRestartDelay;
        }

        private void stopDelayedAutoSaveTimer()
        {
            ticker -= DelayedAutoSave;
            intTimer = 0;
            lblBotTimer.Text = "Bot timer: " + intTimer.ToString();
        }

        private void startDelayedAutoSaveTimer()
        {
            ticker += DelayedAutoSave;
        }

        int intTimer = 0;
        private void DelayedAutoSave()
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
                stopDelayedAutoSaveTimer();
                SaveArticle();
            }

            lblBotTimer.Text = "Bot timer: " + intTimer.ToString();
        }

        private void showTimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showTimer();
        }

        private void showTimer()
        {
            lblTimer.Visible = showTimerToolStripMenuItem.Checked;
            stopSaveInterval();
        }

        int intStartTimer = 0;
        private void SaveInterval()
        {
            intStartTimer++;
            lblTimer.Text = "Timer: " + intStartTimer.ToString();
        }
        private void stopSaveInterval()
        {
            intStartTimer = 0;
            lblTimer.Text = "Timer: 0";
            ticker -= SaveInterval;
        }

        public delegate void Tick();
        public event Tick ticker;
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (ticker != null)
            {
                ticker();
            }

            seconds++;
            if (seconds == 60)
            {
                seconds = 0;
                EditsPerMin();
            }
        }

        int seconds = 0;
        int lastTotal = 0;
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

        private void btnFalsePositive_Click(object sender, EventArgs e)
        {
            FalsePositive();
        }

        private void tsbuttonFalsePositive_Click(object sender, EventArgs e)
        {
            FalsePositive();
        }

        private void FalsePositive()
        {
            if (EdittingArticle.Name.Length > 0)
                Tools.WriteLog("#[[" + EdittingArticle.Name + "]]\r\n", @"False positives.txt");
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

        private void filterOutNonMainSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //listMaker1.FilterNonMainArticles();
            if (filterOutNonMainSpaceToolStripMenuItem.Checked)
            {
                listMaker1.FilterNonMainAuto = true;
                listMaker1.FilterNonMainArticles();
            }
            else
                listMaker1.FilterNonMainAuto = false;
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
            //listMaker1.AlphaSortList();
            if (sortAlphabeticallyToolStripMenuItem.Checked)
            {
                listMaker1.AutoAlpha = true;
                listMaker1.AlphaSortList();
            }
            else
                listMaker1.AutoAlpha = false;
        }

        private void saveListToTextFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listMaker1.SaveList();
        }

        private void launchListComparerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListComparer lc = new ListComparer();
            lc.Show();
        }

        private void launchDumpSearcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            launchDumpSearcher();
        }

        private void launchDumpSearcher()
        {
            WikiFunctions.DatabaseScanner.DatabaseScanner ds = new WikiFunctions.DatabaseScanner.DatabaseScanner();
            ds.Show();
            UpdateButtons();
        }

        private void addIgnoredToLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnFalsePositive.Visible = addIgnoredToLogFileToolStripMenuItem.Checked;
            btntsFalsePositive.Visible = addIgnoredToLogFileToolStripMenuItem.Checked;
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
                    find(txtFind.Text, chkFindRegex.Checked, chkFindCaseSensitive.Checked);
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

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if ((webBrowserEdit.Document != null)) webBrowserEdit.Document.ExecCommand("Copy", false, System.DBNull.Value);
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
            txtEdit.SelectedText = "{{Hndis|name=" + Tools.MakeHumanCatKey(EdittingArticle.Name) + "}}";
        }

        private void wikifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Text = "{{Wikify|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + txtEdit.Text;
        }

        private void cleanupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Text = "{{cleanup|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + txtEdit.Text;
        }

        private void expandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Text = "{{Expand}}\r\n\r\n" + txtEdit.Text;
        }

        private void speedyDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Text = "{{Delete}}\r\n\r\n" + txtEdit.Text;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = "{{subst:clear}}";
        }

        private void uncategorisedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = "{{Uncategorized|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
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
            txtEdit.SelectedText = "{{DEFAULTSORT:" + Tools.MakeHumanCatKey(EdittingArticle.Name) + "}}";
        }

        private void birthdeathCatsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //find first dates
            string strBirth = "";
            string strDeath = "";
            Regex RegexDates = new Regex("[1-2][0-9]{3}");

            try
            {
                MatchCollection m = RegexDates.Matches(txtEdit.Text);

                if (m.Count >= 1)
                    strBirth = m[0].Value;
                if (m.Count >= 2)
                    strDeath = m[1].Value;

                //make name, surname, firstname
                string strName = Tools.MakeHumanCatKey(EdittingArticle.Name);

                string Categories = "";

                if (strDeath.Length == 0 || int.Parse(strDeath) < int.Parse(strBirth) + 20)
                    Categories = "[[Category:" + strBirth + " births|" + strName + "]]";
                else
                    Categories = "[[Category:" + strBirth + " births|" + strName + "]]\r\n[[Category:" + strDeath + " deaths|" + strName + "]]";

                txtEdit.SelectedText = Categories;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void stubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = toolStripTextBox1.Text;
        }
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            txtEdit.Focus();

            if (txtEdit.SelectedText.Length > 0)
            {
                cutToolStripMenuItem.Enabled = true;
                copyToolStripMenuItem.Enabled = true;
                openSelectionInBrowserToolStripMenuItem.Enabled = true;
            }
            else
            {
                cutToolStripMenuItem.Enabled = false;
                copyToolStripMenuItem.Enabled = false;
                openSelectionInBrowserToolStripMenuItem.Enabled = false;
            }

            undoToolStripMenuItem.Enabled = txtEdit.CanUndo;

            openPageInBrowserToolStripMenuItem.Enabled = EdittingArticle.Name.Length > 0;
            openHistoryMenuItem.Enabled = EdittingArticle.Name.Length > 0;
            replaceTextWithLastEditToolStripMenuItem.Enabled = LastArticle.Length > 0;
        }

        private void openPageInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Variables.URLLong + "index.php?title=" + EdittingArticle.URLEncodedName);
        }

        private void openHistoryMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Variables.URLLong + "index.php?title=" + EdittingArticle.URLEncodedName + "&action=history");
        }

        private void openSelectionInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Variables.URLLong + "index.php?title=" + txtEdit.SelectedText);
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
            if (!findAndReplace.Visible)
                findAndReplace.ShowDialog();
            else
                findAndReplace.Hide();
        }

        private void Stop()
        {
            SaveTimer.Stop();
            UpdateButtons();
            if (intTimer > 0)
            {//stop and reset the bot timer.
                stopDelayedAutoSaveTimer();
                EnableButtons();
                return;
            }

            stopSaveInterval();
            StopDelayedRestartTimer();
            if (webBrowserEdit.IsBusy)
                webBrowserEdit.Stop2();
            if (Variables.User.webBrowserLogin.IsBusy)
                Variables.User.webBrowserLogin.Stop();

            listMaker1.Stop();

            if (AutoSaveEditBoxEnabled)
                EditBoxSaveTimer.Enabled = false;

            lblStatusText.Text = "Stopped";
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/Wikipedia:AutoWikiBrowser/User_manual");
        }

        private void reparseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool b = true;
            txtEdit.Text = Process(txtEdit.Text, out b);
        }

        private void replaceTextWithLastEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LastArticle.Length > 0)
                txtEdit.Text = LastArticle;
        }

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

        private void removeAllExcessWhitespaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = txtEdit.Text;

            text = RemoveText.Hide(text);
            text = parsers.RemoveAllWhiteSpace(text);
            text = RemoveText.AddBack(text);

            txtEdit.Text = text;
        }

        private void txtNewCategory_DoubleClick(object sender, EventArgs e)
        {
            txtNewCategory.SelectAll();
        }

        private void cmboEditSummary_MouseMove(object sender, MouseEventArgs e)
        {
            if (EdittingArticle.EditSummary == "")
                toolTip1.SetToolTip(cmboEditSummary, "");
            else
                toolTip1.SetToolTip(cmboEditSummary, MakeSummary());
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void chkRegExTypo_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoMode.Checked && chkRegExTypo.Checked)
            {
                MessageBox.Show("RegexTypoFix cannot be used with auto save on.\r\nAutosave will now be turned off, and Typos loaded.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                chkAutoMode.Checked = false;
                //return;
            }
                loadTypos(false);
                chkSkipIfNoRegexTypo.Enabled = chkRegExTypo.Checked;
        }

        private void loadTypos(bool Reload)
        {
            if (chkRegExTypo.Checked)
            {
                lblStatusText.Text = "Loading typos";

                string s = Variables.RETFPath;

                if (!s.StartsWith("http:")) s = Variables.URL + "/wiki/" + Tools.WikiEncode(s);

                string message = @"1. Check each edit before you make it. Although this has been built to be very accurate there is always the possibility of an error which requires your attention.

2. Optional: Select [[WP:AWB/T|Typo fixing]] as the edit summary. This lets everyone know where to bring issues with the typo correction.";

                if (RegexTypos == null)
                    message += "\r\n\r\nThe newest typos will now be downloaded from " + s + " when you press OK.";

                MessageBox.Show(message, "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                if (RegexTypos == null || Reload)
                {
                    RegexTypos = new RegExTypoFix();
                    lblStatusText.Text = RegexTypos.Typos.Count.ToString() + " typos loaded";
                }
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/User:Mboverload/RegExTypoFix");
        }

        private void webBrowserEdit_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            toolStripProgressBar1.MarqueeAnimationSpeed = 0;
            toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
        }

        private void webBrowserEdit_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
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

            string PrevSummary = cmboEditSummary.SelectedText;

            if (se.ShowDialog() == DialogResult.OK)
            {
                cmboEditSummary.Items.Clear();

                foreach (string s in se.Summaries.Lines)
                {
                    if (s.Trim() == "") continue;

                    cmboEditSummary.Items.Add(s.Trim());
                }

                if (cmboEditSummary.Items.Contains(PrevSummary))
                    cmboEditSummary.SelectedText = PrevSummary;
                else cmboEditSummary.SelectedItem = 0;
            }
        }

        private void showHidePanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            panelShowHide();
        }

        #endregion

        #region tool bar stuff

        private void btnShowHide_Click(object sender, EventArgs e)
        {
            panelShowHide();
        }

        private void btntsShowHideParameters_Click(object sender, EventArgs e)
        {
            parametersShowHide();
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

        private void setBrowserSize()
        {
            if (toolStrip.Visible)
            {
                webBrowserEdit.Location = new Point(webBrowserEdit.Location.X, 48);
                if (splitContainer1.Visible)
                    webBrowserEdit.Height = splitContainer1.Location.Y - 48;
                else
                    webBrowserEdit.Height = statusStrip1.Location.Y - 48;

            }
            else
            {
                webBrowserEdit.Location = new Point(webBrowserEdit.Location.X, 25);
                if (splitContainer1.Visible)
                    webBrowserEdit.Height = splitContainer1.Location.Y - 25;
                else
                    webBrowserEdit.Height = statusStrip1.Location.Y - 25;
            }
        }

        private void enableTheToolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enableToolBar = enableTheToolbarToolStripMenuItem.Checked;
        }

        private bool boolEnableToolbar = false;
        private bool enableToolBar
        {
            get { return boolEnableToolbar; }
            set
            {
                if (value == true)
                    toolStrip.Show();
                else
                    toolStrip.Hide();
                setBrowserSize();
                enableTheToolbarToolStripMenuItem.Checked = value;
                boolEnableToolbar = value;
            }
        }

        #endregion

        #region Images

        private void cmboImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmboImages.SelectedIndex == 0)
            {
                lblImageWith.Text = "";

                txtImageReplace.Enabled = false;
                txtImageWith.Enabled = false;
                chkSkipNoImgChange.Enabled = false;
            }
            else if (cmboImages.SelectedIndex == 1)
            {
                lblImageWith.Text = "With Image:";

                txtImageWith.Enabled = true;
                txtImageReplace.Enabled = true;
                chkSkipNoImgChange.Enabled = true;
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
                
                txtImageWith.Enabled = true;
                txtImageReplace.Enabled = true;
                chkSkipNoImgChange.Enabled = true;
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

        private void SetProgressBar()
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

        #region Plugin

        Dictionary<string, IAWBPlugin> AWBPlugins = new Dictionary<string, IAWBPlugin>();
        private void LoadPlugins()
        {
            try
            {
                string path = Application.StartupPath;
                string[] pluginFiles = Directory.GetFiles(path, "*.DLL");

                foreach (string s in pluginFiles)
                {
                    if (s.EndsWith("DotNetWikiBot.dll"))
                        continue;

                    string imFile = Path.GetFileName(s);

                    Assembly asm = Assembly.LoadFile(path + "\\" + imFile);

                    if (asm != null)
                    {
                        Type[] types = asm.GetTypes();

                        foreach (Type t in types)
                        {
                            Type g = t.GetInterface("IAWBPlugin");

                            if (g != null)
                            {
                                IAWBPlugin awb = (IAWBPlugin)Activator.CreateInstance(t);
                                AWBPlugins.Add(awb.Name, awb);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Problem loading plugin");
            }

            foreach (KeyValuePair<string, IAWBPlugin> a in AWBPlugins)
            {
                a.Value.Start += Start;
                a.Value.Save += Save;
                a.Value.Skip += SkipPage;
                a.Value.Stop += Stop;
                a.Value.Diff += GetDiff;
                a.Value.Preview += GetPreview;

                a.Value.Initialise(listMaker1, webBrowserEdit, pluginsToolStripMenuItem, mnuTextBox, tabControl1, this, txtEdit);
            }

            pluginsToolStripMenuItem.Visible = AWBPlugins.Count > 0;
        }

        private void MoveArticle()
        {
            MoveDeleteDialog dlg = new MoveDeleteDialog(true);

            try
            {
                dlg.NewTitle = EdittingArticle.Name;
                dlg.Summary = LastMove;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    LastMove = dlg.Summary;
                    webBrowserEdit.MovePage(EdittingArticle.Name, dlg.NewTitle, dlg.Summary);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dlg.Dispose();
            }
        }

        private void DeleteArticle()
        {
            MoveDeleteDialog dlg = new MoveDeleteDialog(false);

            try
            {
                dlg.Summary = LastDelete;

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    LastDelete = dlg.Summary;
                    webBrowserEdit.DeletePage(EdittingArticle.Name, dlg.Summary);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                dlg.Dispose();
            }
        }

        #endregion

        private void btnSubst_Click(object sender, EventArgs e)
        {
            substTemplates.ShowDialog();
        }

        private void testRegexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            regexTester.ShowDialog();
        }

        private void chkLock_CheckedChanged(object sender, EventArgs e)
        {
            cmboEditSummary.Visible = !chkLock.Checked;
            lblSummary.Text = cmboEditSummary.Text;
            lblSummary.Visible = chkLock.Checked;
        }

        private void txtDabLink_TextChanged(object sender, EventArgs e)
        {
            btnLoadLinks.Enabled = txtDabLink.Text.Trim() != "";
        }

        private void txtDabLink_Enter(object sender, EventArgs e)
        {
            if (txtDabLink.Text == "") txtDabLink.Text = listMaker1.SourceText;
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
                Article link = new Article(name);
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
                MessageBox.Show(ex.Message);
            }
            finally
            {
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

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                toolStripHide();
            }
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.Visible = false;
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ntfyTray_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!this.Visible)
            {
                toolStripHide();
            }
            else
                this.Visible = false;
        }

        private void toolStripHide()
        {
            this.Visible = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        }

        private void updateUpdater()
        {
            Updater Updater = new Updater();
            Updater.Update();
        }

        public void NotifyBalloon(string Message, ToolTipIcon Icon)
        {
            ntfyTray.BalloonTipText = Message;
            ntfyTray.BalloonTipIcon = Icon;
            ntfyTray.ShowBalloonTip(10000);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
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
            runUpdater();
        }

        private void runUpdater()
        {
            System.Diagnostics.Process.Start(Path.GetDirectoryName(Application.ExecutablePath) + "\\AWBUpdater.exe");

            DialogResult closeAWB = MessageBox.Show("AWB needs to be closed. To do this now, click 'yes'. If you need to save your settings, do this now, the updater will not complete until AWB is closed.", "Close AWB?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if (closeAWB == DialogResult.Yes)
                Application.Exit();
        }

        private void btnResetNudges_Click(object sender, EventArgs e)
        {
            Nudges = 0;
            sameArticleNudges = 0;
            lblNudges.Text = NudgeTimerString + "0";
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            SaveTimer.Interval = (int)nudNudgeTime.Value * 60000;
        }


        #region "Nudge timer"
            private const string NudgeTimerString = "Total nudges: ";

            private void SaveTimer_Tick(object sender, EventArgs e)
            {
                //make sure there was no error and bot mode is still enabled
                if (chkAutoMode.Checked)
                {
                    bool CancelNudge = false;

                    // Tell plugins we're about to nudge, and give them the opportunity to cancel:
                    foreach (KeyValuePair<string, IAWBPlugin> a in AWBPlugins)
                    {
                        a.Value.Nudge(out CancelNudge);
                        if (CancelNudge) return;
                    }

                    // Update stats and nudge:
                    Nudges++;
                    lblNudges.Text = NudgeTimerString + Nudges;
                    SaveTimer.Stop();
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
                    foreach (KeyValuePair<string, IAWBPlugin> a in AWBPlugins)
                    { a.Value.Nudged(Nudges); }
                }
            }

            public int Nudges {
                get { return mnudges; }
                private set { mnudges = value; }
            }
        #endregion

        #region IAWBMainForm:
            TabPage IAWBMainForm.MoreOptionsTab { get { return tpMoreOptions; } }
            TabPage IAWBMainForm.OptionsTab { get { return tpSetOptions; } }
            TabPage IAWBMainForm.StartTab { get { return tpStart; } }
            TabPage IAWBMainForm.DabTab { get { return tpDab; } }
            TabPage IAWBMainForm.BotTab { get { return tpBots; } }
            CheckBox IAWBMainForm.BotModeCheckbox { get { return chkAutoMode; } }
            Button IAWBMainForm.PreviewButton { get { return btnPreview; } }
            Button IAWBMainForm.SaveButton { get { return btnSave; } }
            Button IAWBMainForm.SkipButton { get { return btnIgnore; } }
            Button IAWBMainForm.StopButton { get { return btnStop; } }
            Button IAWBMainForm.DiffButton { get { return btnDiff; } }
            Button IAWBMainForm.StartButton { get { return btnStart; } }
            ComboBox IAWBMainForm.EditSummary { get { return cmboEditSummary; } }
            StatusStrip IAWBMainForm.StatusStrip { get { return statusStrip1; } }
            NotifyIcon IAWBMainForm.NotifyIcon { get { return ntfyTray; } }
            Boolean IAWBMainForm.SkipNonExistentPages
            {
                get { return chkSkipNonExistent.Checked; }
                set { chkSkipNonExistent.Checked = value; }
            }
            ToolStripMenuItem IAWBMainForm.HelpToolStripMenuItem { get { return helpToolStripMenuItem; } }
        #endregion

        #region Logs

        private void btnClearSaved_Click(object sender, EventArgs e)
        {
            lvSaved.Items.Clear();
        }

        private void btnClearIgnored_Click(object sender, EventArgs e)
        {
            lvIgnored.Items.Clear();
        }

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
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveListView(ListView listview)
        {
            try
            {
                StringBuilder strList = new StringBuilder("");
                StreamWriter sw;
                string strListFile;
                if (saveListDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (ListViewItem a in listview.Items)
                    {
                        string text = a.Text;
                        if (a.SubItems.Count > 0)
                        {
                            for (int i = 1; i < a.SubItems.Count; i++)
                            {
                                text += " " + a.SubItems[i].Text;
                            }
                        }
                        strList.AppendLine(text);
                    }
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
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSaveSaved_Click(object sender, EventArgs e)
        {
            SaveListView(lvSaved);
        }

        private void btnSaveIgnored_Click(object sender, EventArgs e)
        {
            SaveListView(lvIgnored);
        }

            #endregion

        private void btnAddtoList_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem article in lvIgnored.Items)
                listMaker1.Add(new Article(article.Text));
        }

        private void resizeListView(ListView lstView)
        {
            int width; int width2;
            foreach (ColumnHeader head in lstView.Columns)
            {
                head.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
                width = head.Width;

                head.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                width2 = head.Width;

                if (width2 < width)
                    head.AutoResize(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        private void lvSaved_MouseMove(object sender, MouseEventArgs e)
        {
            //string strTip = "";

            //Get the item
            //int nIdx = lvSaved.IndexFromPoint(e.Location);
            //if ((nIdx >= 0) && (nIdx < lvSaved.Items.Count))
            //    strTip = lvSaved.Items[nIdx].ToString();
            //if (strTip != strlbSavedTooltip)
            //    toolTip1.SetToolTip(lvSaved, strTip);
            //strlbSavedTooltip = strTip;
        }

        private void lvIgnored_MouseMove(object sender, MouseEventArgs e)
        {
            //string strTip = "";

            //Get the item
            //int nIdx = lvSaved.IndexFromPoint(e.Location);
            //if ((nIdx >= 0) && (nIdx < lvSaved.Items.Count))
            //    strTip = lvSaved.Items[nIdx].ToString();
            //if (strTip != strlbSavedTooltip)
            //    toolTip1.SetToolTip(lvSaved, strTip);
            //strlbSavedTooltip = strTip;

        }

        private void LogLists_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                ((AWBLogListener)((ListView)sender).FocusedItem).OpenInBrowser();
            }
            catch { }
        }

        private void EditBoxSaveTimer_Tick(object sender, EventArgs e)
        {
            saveEditBoxText(Application.StartupPath + AutoSaveEditBoxFile);
        }

        private void saveEditBoxText(string path)
        {
            try
            {
                StreamWriter sw;
                sw = new StreamWriter(path.ToString(), false, Encoding.UTF8);
                sw.Write(txtEdit.Text);
                sw.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.Message, "File error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void saveTextToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveListDialog.ShowDialog() == DialogResult.OK)
                saveEditBoxText(saveListDialog.FileName);
        }


    }
}
