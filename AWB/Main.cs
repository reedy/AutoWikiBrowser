/*
Autowikibrowser
Copyright (C) 2006 Martin Richards

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
using WikiFunctions.Browser;
using System.Collections.Specialized;

[assembly: CLSCompliant(true)]
namespace AutoWikiBrowser
{
    public partial class MainForm : Form
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
                btntsSave.Image = Resources.btntssave_image;
                btntsIgnore.Image = Resources.GoLtr;
                btntsStop.Image = Resources.Stop;
                btntsPreview.Image = Resources.preview;
                btntsChanges.Image = Resources.changes;
                btntsFalsePositive.Image = Resources.RolledBack;
                btntsStart.Image = Resources.Run;
                //btnSave.Image = Resources.btntssave_image;
                //btnIgnore.Image = Resources.GoLtr;

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

                cmboCategorise.SelectedIndex = 0;
                cmboImages.SelectedIndex = 0;
                lblStatusText.AutoSize = true;
                lblBotTimer.AutoSize = true;

                Variables.UserNameChanged += UpdateUserName;

                webBrowserLogin.ScriptErrorsSuppressed = true;
                webBrowserLogin.DocumentCompleted += web4Completed;
                webBrowserLogin.Navigating += web4Starting;

                webBrowserEdit.Loaded += CaseWasLoad;
                webBrowserEdit.Diffed += CaseWasDiff;
                webBrowserEdit.Saved += CaseWasSaved;
                webBrowserEdit.None += CaseWasNull;
                webBrowserEdit.Fault += StartDelayedRestartTimer;
                webBrowserEdit.StatusChanged += UpdateWebBrowserStatus;
                
                listMaker1.BusyStateChanged += SetProgressBar;
                listMaker1.NoOfArticlesChanged += UpdateButtons;
                listMaker1.StatusTextChanged += UpdateListStatus;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        string LastArticle = "";
        string SettingsFile = "";
        bool boolSaved = true;
        HideText RemoveText = new HideText(false, true, false);
        List<string> noParse = new List<string>();
        FindandReplace findAndReplace = new FindandReplace();
        RegExTypoFix RegexTypos;
        SkipOptions Skip = new SkipOptions();
        WikiFunctions.MWB.ReplaceSpecial replaceSpecial = new WikiFunctions.MWB.ReplaceSpecial();
        Parsers parsers;
        WebControl webBrowserLogin = new WebControl();
        TimeSpan StartTime = new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        StringCollection RecentList = new StringCollection();
        CustomModule cModule = new CustomModule();

        WikiFunctions.DotNetWikiBot DNWB;

        private void MainForm_Load(object sender, EventArgs e)
        {
            //add articles to avoid (in future may be populated from checkpage
            //noParse.Add("User:Bluemoose/Sandbox");

            //check that we are not using an old OS. 98 seems to mangled some unicode.
            try
            {
                if (Environment.OSVersion.Version.Major < 5)
                {
                    MessageBox.Show("You appear to be using an older operating system, this software may have trouble with some unicode fonts on operating systems older than Windows 2000, the start button has been disabled.", "Operating system", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SetStartButton(false);
                }
                else
                    listMaker1.MakeListEnabled = true;

                Debug();

                if (AutoWikiBrowser.Properties.Settings.Default.LogInOnStart)
                    WikiStatus();

                LoadPlugins();
                LoadPrefs();
                UpdateButtons();
                LoadRecentSettingsList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

        bool bFlashAndBeep = true;
        private bool FlashAndBeep
        {
            get { return bFlashAndBeep; }
            set { bFlashAndBeep = value; }
        }

        private bool WikiStatusBool = false;
        internal bool wikiStatusBool
        {
            get { return WikiStatusBool; }
            set
            {
                WikiStatusBool = value;
                if (value == true)
                    lblUserName.BackColor = Color.LightGreen;
                else
                    lblUserName.BackColor = Color.Red;
            }
        }

        #endregion

        #region MainProcess

        private void Start()
        {
            try
            {
                Tools.WriteDebug(this.Name, "Starting");

                //check edit summary
                if (cmboEditSummary.Text == "")
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
                if (!WikiStatus())
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

                if (!Tools.IsValidTitle(EdittingArticle.Name))
                {
                    SkipPage();
                    return;
                }

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
                    SkipPage();
                    return;
                }

                listMaker1.ReplaceArticle(EdittingArticle, Redirect);
                EdittingArticle = Redirect;

                webBrowserEdit.LoadEditPage(Redirect.Name);
                return;
            }

            if (chkSkipIfContains.Checked && Skip.SkipIfContains(strText, EdittingArticle.Name,
            txtSkipIfContains.Text, chkSkipIsRegex.Checked, chkSkipCaseSensitive.Checked, true))
            {
                SkipPage();
                return;
            }

            if (chkSkipIfNotContains.Checked && Skip.SkipIfContains(strText, EdittingArticle.Name,
            txtSkipIfNotContains.Text, chkSkipIsRegex.Checked, chkSkipCaseSensitive.Checked, false))
            {
                SkipPage();
                return;
            }

            if (!Skip.skipIf(strText))
            {
                SkipPage();
                return;
            }

            bool skip = false;
            if (!doNotAutomaticallyDoAnythingToolStripMenuItem.Checked)
            {
                string strOrigText = strText;
                strText = Process(strText, out skip);

                if (skippable && chkSkipNoChanges.Checked && strText == strOrigText)
                {
                    SkipPage();
                    return;
                }
            }

            if (skip)
            {
                SkipPage();
                return;
            }

            webBrowserEdit.SetArticleText(strText);
            txtEdit.Text = strText;
            //Update statistics and alerts
            ArticleInfo(false);

            if (chkAutoMode.Checked && chkQuickSave.Checked)
                startDelayedAutoSaveTimer();
            else if (previewInsteadOfDiffToolStripMenuItem.Checked)
                GetPreview();
            else
                GetDiff();
        }

        private bool loadSuccess()
        {
            try
            {
                string HTML = webBrowserEdit.Document.Body.InnerHtml;
                if (HTML.Contains("The Wikipedia database is currently locked, and is not accepting any edits or other modifications."))
                {//http://en.wikipedia.org/wiki/MediaWiki:Readonlytext
                    StartDelayedRestartTimer(60);
                    return false;
                }
                if (HTML.Contains("<H1 class=firstHeading>View source</H1>"))
                {
                    SkipPage();
                    return false;
                }
                //check we are still logged in
                if (!webBrowserEdit.LoggedIn)
                {
                    wikiStatusBool = false;
                    Start();
                    return false;
                }

                if (webBrowserEdit.NewMessage)
                {//check if we have any messages
                    wikiStatusBool = false;
                    UpdateButtons();
                    webBrowserEdit.Document.Write("");
                    this.Focus();

                    dlgTalk DlgTalk = new dlgTalk();
                    if (DlgTalk.ShowDialog() == DialogResult.Yes)
                        System.Diagnostics.Process.Start(Variables.URLLong + "index.php?title=User_talk:" + Variables.UserName + "&action=purge");
                    else
                        System.Diagnostics.Process.Start("IExplore", Variables.URLLong + "index.php?title=User_talk:" + Variables.UserName + "&action=purge");

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
                    SkipPage();
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
                SkipPage();
                return;
            }

            if (chkAutoMode.Checked)
            {
                startDelayedAutoSaveTimer();
                return;
            }

            if (!this.ContainsFocus && FlashAndBeep)
            {
                Tools.FlashWindow(this);
                Tools.Beep1();
            }

            this.Focus();

            EnableButtons();
        }

        private bool diffChecker(string strHTML)
        {//check diff to see if it should be skipped

            if (!skippable || !chkSkipNoChanges.Checked || previewInsteadOfDiffToolStripMenuItem.Checked || doNotAutomaticallyDoAnythingToolStripMenuItem.Checked)
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
            Start();
        }

        private void CaseWasNull()
        {
            if (webBrowserEdit.Document.Body.InnerHtml.Contains("<B>You have successfully signed in to Wikipedia as"))
            {
                lblStatusText.Text = "Signed in, now re-starting";
                WikiStatus();
            }
        }

        private void SkipPage()
        {
            try
            {
                //reset timer.
                NumberOfIgnoredEdits++;
                stopDelayedAutoSaveTimer();
                listMaker1.Remove(EdittingArticle);
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

                        articleText = a.Value.ProcessArticle(articleText, EdittingArticle.Name, EdittingArticle.NameSpaceKey, out tempSummary, out SkipArticle);
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
                        return articleText;
                }

                if (cmboImages.SelectedIndex == 1)
                {
                    articleText = parsers.ReplaceImage(txtImageReplace.Text, txtImageWith.Text, articleText, out SkipArticle);
                    if (SkipArticle)
                        return articleText;
                }
                else if (cmboImages.SelectedIndex == 2)
                {
                    articleText = parsers.RemoveImage(txtImageReplace.Text, articleText, false, txtImageWith.Text, out SkipArticle);
                    if (SkipArticle)
                        return articleText;
                }
                else if (cmboImages.SelectedIndex == 3)
                {
                    articleText = parsers.RemoveImage(txtImageReplace.Text, articleText, true, txtImageWith.Text, out SkipArticle);
                    if (SkipArticle)
                        return articleText;
                }

                if (cmboCategorise.SelectedIndex == 1)
                {
                    articleText = parsers.ReCategoriser(listMaker1.SourceText, txtNewCategory.Text, articleText, out SkipArticle);
                    if (SkipArticle)
                        return articleText;
                }
                else if (cmboCategorise.SelectedIndex == 2 && txtNewCategory.Text.Length > 0)
                {
                    articleText = parsers.AddCategory(txtNewCategory.Text, articleText, EdittingArticle.Name);
                }
                else if (cmboCategorise.SelectedIndex == 3 && txtNewCategory.Text.Length > 0)
                {
                    articleText = parsers.RemoveCategory(txtNewCategory.Text, articleText, out SkipArticle);
                    if (SkipArticle)
                        return articleText;
                }

                if (chkFindandReplace.Checked && !findAndReplace.AfterOtherFixes)
                {
                    SkipArticle = false;
                    articleText = PerformFindAndReplace(articleText, out SkipArticle);
                    if (SkipArticle)
                        return articleText;
                }

                if (chkRegExTypo.Checked && RegexTypos != null)
                {
                    articleText = RegexTypos.PerformTypoFixes(articleText, out SkipArticle, ref EdittingArticle.EditSummary);
                    if (SkipArticle && chkSkipIfNoRegexTypo.Checked)
                        return articleText;
                }

                if (EdittingArticle.NameSpaceKey == 0 || EdittingArticle.NameSpaceKey == 14 || EdittingArticle.Name.Contains("Sandbox") || EdittingArticle.Name.Contains("sandbox"))
                {
                    if (process && chkAutoTagger.Checked)
                    {
                        articleText = parsers.Tagger(articleText, EdittingArticle.Name, out SkipArticle, ref EdittingArticle.EditSummary);
                        if (Skip.SkipNoTag && SkipArticle)
                            return articleText;
                    }

                    if (process && chkGeneralFixes.Checked)
                    {
                        articleText = RemoveText.Hide(articleText);

                        if (Variables.LangCode == LangCodeEnum.en)
                        {//en only
                            articleText = parsers.Conversions(articleText);
                            articleText = parsers.LivingPeople(articleText, out SkipArticle);

                            articleText = parsers.FixHeadings(articleText, EdittingArticle.Name, out SkipArticle);
                            if (Skip.SkipNoHeaderError && SkipArticle)
                                return articleText;
                        }
                        articleText = parsers.FixCategories(articleText);
                        articleText = parsers.FixSyntax(articleText);

                        articleText = parsers.FixLinks(articleText, out SkipArticle);
                        if (Skip.SkipNoBadLink && SkipArticle)
                            return articleText;

                        articleText = parsers.BulletExternalLinks(articleText, out SkipArticle);
                        if (Skip.SkipNoBulletedLink && SkipArticle)
                            return articleText;

                        articleText = parsers.SortMetaData(articleText, EdittingArticle.Name);

                        articleText = parsers.BoldTitle(articleText, EdittingArticle.Name, out SkipArticle);
                        if (Skip.SkipNoBoldTitle && SkipArticle)
                            return articleText;

                        articleText = parsers.LinkSimplifier(articleText);
                        articleText = parsers.FixHeadings(articleText);

                        articleText = RemoveText.AddBack(articleText);
                    }
                }
                else if (process && chkGeneralFixes.Checked && EdittingArticle.NameSpaceKey == 3)
                {
                    articleText = RemoveText.Hide(articleText);
                    articleText = parsers.SubstUserTemplates(articleText);
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
                        return articleText;
                }

                SkipArticle = false;
                return articleText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SkipArticle = false;
                return articleText;
            }
        }

        private string PerformFindAndReplace(string articleText, out bool SkipArticle)
        {
            string testText = articleText;
            articleText = findAndReplace.MultipleFindAndReplce(articleText, EdittingArticle.Name, ref EdittingArticle.EditSummary);
            articleText = replaceSpecial.ApplyRules(articleText, EdittingArticle.Name);

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
            else if (MessageBox.Show("Do you really want to save a blank page?", "Really save??", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                SaveArticle();
            else
                SkipPage();
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

        private void UpdateUserName(object sender, EventArgs e)
        {
            lblUserName.Text = Variables.UserName;
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
            if (webBrowserLogin.IsBusy)
                webBrowserLogin.Stop();

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
        }

        private void cmboCategorise_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmboCategorise.SelectedIndex > 0)
            {
                if (cmboCategorise.SelectedIndex == 1 && (listMaker1.SourceText.Length == 0 || listMaker1.SelectedSource != SourceType.Category))
                {
                    cmboCategorise.SelectedIndex = 0;
                    MessageBox.Show("Please create a list of articles from a category first", "Make list", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                    txtNewCategory.Enabled = true;
            }
            else
                txtNewCategory.Enabled = false;
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

        private bool WikiStatus()
        {//this checks if you are logged in, registered and have the newest version. Some bits disabled.
            try
            {
                //check if we need to bother checking or not
                if (wikiStatusBool)
                    return true;

                string strText = String.Empty;
                lblStatusText.Text = "Loading page to check if we are logged in";
                //load check page
                webBrowserLogin.Navigate(Variables.URLLong + "index.php?title=Project:AutoWikiBrowser/CheckPage&action=edit");
                //wait to load
                while (webBrowserLogin.ReadyState != WebBrowserReadyState.Complete) Application.DoEvents();

                strText = webBrowserLogin.GetArticleText();
                Variables.UserName = webBrowserLogin.UserName();

                //see if we are logged in
                if (!webBrowserLogin.LoggedIn)
                {
                    MessageBox.Show("You are not logged in. The log in screen will now load, enter your name and password, click \"Log in\", wait for it to complete, then start the process again.\r\n\r\nIn the future you can make sure this won't happen by logging in to Wikipedia using Microsoft Internet Explorer.", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    webBrowserEdit.LoadLogInPage();
                    return false;
                }

                //see if there is a message
                Match m = Regex.Match(strText, "<!--Message:(.*?)-->");
                if (m.Success && m.Groups[1].Value.Trim().Length > 0)
                {
                    MessageBox.Show(m.Groups[1].Value, "Automated message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                //Get list of articles not to apply general fixes to.
                Match n = Regex.Match(strText, "<!--No general fixes:.*?-->", RegexOptions.Singleline);
                if (n.Success)
                {
                    foreach (Match link in WikiRegexes.UnPipedWikiLink.Matches(n.Value))
                        if(!noParse.Contains(link.Groups[1].Value))
                            noParse.Add(link.Groups[1].Value);
                }

                //don't require approval in in other languages.
                if (strText.Length < 1)
                {
                    wikiStatusBool = true;
                    chkAutoMode.Enabled = true;
                    return true;
                }
                else if(strText.Contains("<!--All users enabled-->"))
                {//see if all users enabled
                    wikiStatusBool = true;
                        chkAutoMode.Enabled = true;
                        return true;
                }
                else
                {
                    if (!m.Success)
                    {
                        MessageBox.Show("Check page failed to load.\r\n\r\nCheck your Internet Explorer is working and that the Wikipedia servers are online, also try clearing Internet Explorer cache.", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                    //see if this version is enabled
                    else if (!strText.Contains(Assembly.GetExecutingAssembly().GetName().Version.ToString() + " enabled"))
                    {
                        MessageBox.Show("This version is not enabled, please download the newest version. If you have the newest version, check that Wikipedia is online.", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        System.Diagnostics.Process.Start("http://sourceforge.net/project/showfiles.php?group_id=158332");
                        return false;
                    }
                    //see if we are allowed to use this softare
                    else
                    {
                        string strBotUsers = strText.Substring(strText.IndexOf("enabledbots"), strText.IndexOf("enabledbotsends") - strText.IndexOf("enabledbots"));

                        if (Variables.UserName.Length > 0 && strText.Contains("* " + Variables.UserName + "\r\n") || strText.Contains("Everybody enabled = true"))
                        {
                            if (strBotUsers.Contains("* " + Variables.UserName + "\r\n"))
                            {//enable botmode
                                chkAutoMode.Enabled = true;
                            }

                            wikiStatusBool = true;
                            lblStatusText.Text = "Logged in, user enabled and software enabled";
                            UpdateButtons();
                            return true;
                        }
                        else
                        {
                            MessageBox.Show(Variables.UserName + " is not enabled to use this.", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            System.Diagnostics.Process.Start(Variables.URL + "/wiki/Project:AutoWikiBrowser/CheckPage");
                            return false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        private void chkAutoMode_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoMode.Checked)
            {
                chkQuickSave.Enabled = true;
                nudBotSpeed.Enabled = true;
                lblBotTimer.Enabled = true;
                chkSkipNoChanges.Checked = true;
                chkSuppressTag.Enabled = true;
                chkRegExTypo.Checked = false;
            }
            else
            {
                chkQuickSave.Enabled = false;
                nudBotSpeed.Enabled = false;
                lblBotTimer.Enabled = false;
                chkSuppressTag.Enabled = false;
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
            WikiStatus();
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
                    lblWarn.Text += "No category.\r\n";

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
                }
            }
        }

        private void lbDuplicateWikilinks_Click(object sender, EventArgs e)
        {
            if (lbDuplicateWikilinks.SelectedIndex != -1)
            {
                string strLink = Regex.Escape(lbDuplicateWikilinks.SelectedItem.ToString());
                find("\\[\\[" + strLink + "(\\|.*?)?\\]\\]", true, true);
            }
            else
                resetFind();

            ArticleInfo(false);
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
                    resetFind();
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
            listMaker1.Add("User:Bluemoose/Sandbox");
            wikiStatusBool = true;
            chkAutoMode.Enabled = true;
            chkQuickSave.Enabled = true;
            dumpHTMLToolStripMenuItem.Visible = true;
        }

        #endregion

        #region set variables

        private void selectProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MyPreferences MyPrefs = new MyPreferences(Variables.LangCode, Variables.Project, Variables.CustomProject, webBrowserEdit.EnhanceDiffEnabled, webBrowserEdit.ScrollDown, webBrowserEdit.DiffFontSize, txtEdit.Font, LowThreadPriority, FlashAndBeep);

            if (MyPrefs.ShowDialog() == DialogResult.OK)
            {
                webBrowserEdit.EnhanceDiffEnabled = MyPrefs.EnhanceDiff;
                webBrowserEdit.ScrollDown = MyPrefs.ScrollDown;
                webBrowserEdit.DiffFontSize = MyPrefs.DiffFontSize;
                txtEdit.Font = MyPrefs.TextBoxFont;
                LowThreadPriority = MyPrefs.LowThreadPriority;
                FlashAndBeep = MyPrefs.FlashAndBeep;

                wikiStatusBool = false;
                chkQuickSave.Checked = false;
                chkQuickSave.Enabled = false;
                chkAutoMode.Checked = false;
                chkAutoMode.Enabled = false;

                SetProject(MyPrefs.Language, MyPrefs.Project, MyPrefs.CustomProject);
            }
            MyPrefs = null;
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
            if (Project != ProjectEnum.custom) lblProject.Text = Variables.LangCode + "." + Variables.Project;
            else lblProject.Text = Variables.URL;
        }

        #endregion

        #region Enabling/Disabling of buttons

        private void UpdateButtons()
        {
            bool enabled = listMaker1.NumberOfArticles > 0;
            SetStartButton(enabled);
            listMaker1.ButtonsEnabled = enabled;
            lbltsNumberofItems.Text = "Articles: " + listMaker1.NumberOfArticles.ToString();
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
            intRestartDelay += 3;
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
            SkipPage();
        }

        private void filterOutNonMainSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listMaker1.FilterNonMainArticles();
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
            listMaker1.AlphaSortList();
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
                    SkipPage();
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
            txtEdit.SelectedText = Tools.MakeHumanCatKey(EdittingArticle.Name);
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
            }
            else
            {
                cutToolStripMenuItem.Enabled = false;
                copyToolStripMenuItem.Enabled = false;
            }

            undoToolStripMenuItem.Enabled = txtEdit.CanUndo;

            openPageInBrowserToolStripMenuItem.Enabled = EdittingArticle.Name.Length > 0;
            replaceTextWithLastEditToolStripMenuItem.Enabled = LastArticle.Length > 0;
        }

        private void openPageInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Variables.URLLong + "index.php?title=" + EdittingArticle.URLEncodedName);
        }

        private void previewInsteadOfDiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chkSkipNoChanges.Enabled = !previewInsteadOfDiffToolStripMenuItem.Checked;
        }

        private void chkGeneralParse_CheckedChanged(object sender, EventArgs e)
        {
            alphaSortInterwikiLinksToolStripMenuItem.Enabled = chkGeneralFixes.Checked;
        }

        private void btnFindAndReplaceAdvanced_Click(object sender, EventArgs e)
        {
            if (!replaceSpecial.Visible)
                replaceSpecial.ShowDialog();
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
            if (webBrowserLogin.IsBusy)
                webBrowserLogin.Stop();

            listMaker1.Stop();
            lblStatusText.Text = "Stopped";
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/Wikipedia:AutoWikiBrowser#User_manual");
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
            if (chkAutoMode.Checked)
            {
                MessageBox.Show("RegexTypoFix cannot be used with auto save on.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (chkRegExTypo.Checked)
            {
                lblStatusText.Text = "Loading typos";

                string message = @"1. Check each edit before you make it. Although this has been built to be very accurate there is always the possibility of an error which requires your attention.

2. Optional: Select [[WP:AWB/T|Typo fixing]] as the edit summary. This lets everyone know where to bring issues with the typo correction.";

                if (RegexTypos == null)
                    message += "\r\n\r\nThe newest typos will now be downloaded from " + Variables.URLLong + Variables.Namespaces[4] + "AutoWikiBrowser/Typos";

                MessageBox.Show(message, "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                if (RegexTypos == null)
                {
                    RegexTypos = new RegExTypoFix();
                    lblStatusText.Text = RegexTypos.Typos.Count.ToString() + " typos loaded";
                }
            }

            chkSkipIfNoRegexTypo.Enabled = chkRegExTypo.Checked;
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

        #endregion

        #region tool bar stuff

        private void btnShowHide_Click(object sender, EventArgs e)
        {
            if (panel2.Visible)
            {
                panel2.Hide();
            }
            else
            {
                panel2.Show();
            }
            setBrowserSize();
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
            SkipPage();
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
                if (panel2.Visible)
                    webBrowserEdit.Height = panel2.Location.Y - 48;
                else
                    webBrowserEdit.Height = statusStrip1.Location.Y - 48;

            }
            else
            {
                webBrowserEdit.Location = new Point(webBrowserEdit.Location.X, 25);
                if (panel2.Visible)
                    webBrowserEdit.Height = panel2.Location.Y - 25;
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
                lblImageReplace.Text = "";
                lblImageWith.Text = "";
                txtImageWith.Visible = true;
                txtImageReplace.Enabled = false;
                txtImageWith.Enabled = false;
            }
            else if (cmboImages.SelectedIndex == 1)
            {
                lblImageReplace.Text = "Replace image:";
                lblImageWith.Text = "With Image:";

                txtImageWith.Visible = true;
                txtImageReplace.Enabled = true;
                txtImageWith.Enabled = true;

            }
            else if (cmboImages.SelectedIndex == 2)
            {
                lblImageReplace.Text = "Remove image:";
                lblImageWith.Text = "";

                txtImageWith.Visible = false;
                txtImageReplace.Enabled = true;
                txtImageWith.Enabled = false;

            }
            else if (cmboImages.SelectedIndex == 3)
            {
                lblImageReplace.Text = "Remove image:";
                lblImageWith.Text = "Comment:";

                txtImageWith.Visible = true;
                txtImageReplace.Enabled = true;
                txtImageWith.Enabled = true;

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

        #endregion

        private void LogginDNWB()
        {
            try
            {
                LoginDlg lg = new LoginDlg();
                lg.UserName = Variables.UserName;

                if (lg.ShowDialog() == DialogResult.OK)
                {
                    DNWB = new DotNetWikiBot(Variables.URL, lg.UserName, lg.Password);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MoveArticle(string NewTitle)
        {
            if (DNWB == null)
                LogginDNWB();

            try
            {
                DNWB.MovePage(EdittingArticle.Name, NewTitle, "test");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
