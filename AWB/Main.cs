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

[assembly: CLSCompliant(true)]
namespace AutoWikiBrowser
{
    public partial class MainForm : Form
    {
        #region constructor etc.

        public MainForm()
        {
            InitializeComponent();

            btntsShowHide.Image = Resources.btnshowhide_image;
            btntsSave.Image = Resources.btntssave_image;
            btntsIgnore.Image = Resources.GoLtr;
            btntsStop.Image = Resources.Stop;
            btntsPreview.Image = Resources.preview;
            btntsChanges.Image = Resources.changes;
            btnFalsePositive.Image = Resources.RolledBack;
            //btnSave.Image = Resources.btntssave_image;
            //btnIgnore.Image = Resources.GoLtr;

            int stubcount = 500;
            bool catkey = false;
            try
            {
                stubcount = AutoWikiBrowser.Properties.Settings.Default.StubMaxWordCount;
                catkey = AutoWikiBrowser.Properties.Settings.Default.AddHummanKeyToCats;
                parsers = new Parsers(stubcount, catkey);

                if (AutoWikiBrowser.Properties.Settings.Default.LowThreadPriority)
                    Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                //read and set project from user persistent settings (was saved on last exit)
                LangCodeEnum l = (LangCodeEnum)Enum.Parse(typeof(LangCodeEnum), AutoWikiBrowser.Properties.Settings.Default.Language);
                ProjectEnum p = (ProjectEnum)Enum.Parse(typeof(ProjectEnum), AutoWikiBrowser.Properties.Settings.Default.Project);
                SetProject(l, p);
            }
            catch (Exception ex)
            {
                parsers = new Parsers();
                MessageBox.Show(ex.Message);
            }

            cmboSourceSelect.SelectedIndex = 0;
            cmboCategorise.SelectedIndex = 0;
            cmboImages.SelectedIndex = 0;
            lblStatusText.AutoSize = true;
            lblBotTimer.AutoSize = true;

            UpdateButtons();

            webBrowserLogin.ScriptErrorsSuppressed = true;
            webBrowserLogin.DocumentCompleted += web4Completed;
            webBrowserLogin.Navigating += web4Starting;

            webBrowserEdit.Loaded += CaseWasLoad;
            webBrowserEdit.Diffed += CaseWasDiff;
            webBrowserEdit.Saved += CaseWasSaved;
            webBrowserEdit.None += CaseWasNull;
            webBrowserEdit.Fault += StartDelayedRestartTimer;
            webBrowserEdit.StatusChanged += UpdateStatus;
        }

        readonly Regex WikiLinkRegex = new Regex("\\[\\[(.*?)(\\]\\]|\\|)", RegexOptions.Compiled);
        readonly Regex RedirectRegex = new Regex("^#redirect:? ?\\[\\[(.*?)\\]\\]", RegexOptions.IgnoreCase);
        string LastArticle = "";
        string strSettingsFile = "";
        string strListFile = "";
        bool boolSaved = true;
        ArrayList noParse = new ArrayList();
        FindandReplace findAndReplace = new FindandReplace();
        RegExTypoFix RegexTypos;
        WikiFunctions.MWB.ReplaceSpecial replaceSpecial = new WikiFunctions.MWB.ReplaceSpecial();
        Parsers parsers;
        WebControl webBrowserLogin = new WebControl();
        TimeSpan StartTime = new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        int intEdits = 0;

        Article stredittingarticle = new Article("");
        public Article EdittingArticle
        {
            get { return stredittingarticle; }
            private set { stredittingarticle = value; }
        }

        string strUserName = "";
        string UserName
        {
            get { return strUserName; }
            set
            {
                strUserName = value;
                lblUserName.Text = Variables.Namespaces[2] + value;
            }
        }

        private bool WikiStatusBool = false;
        internal bool wikiStatusBool
        {
            get { return WikiStatusBool; }
            set { WikiStatusBool = value; }
        }

        private int NumberOfArticles
        {
            set { lblNumberOfArticles.Text = value.ToString(); }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //add articles to avoid (in future may be populated from checkpage
            //noParse.Add("User:Bluemoose/Sandbox");

            //check that we are not using an old OS. 98 seems to mangled some unicode.
            if (Environment.OSVersion.Version.Major < 5)
            {
                MessageBox.Show("You appear to be using an older operating system, this software may have trouble with some unicode fonts on operating systems older than Windows 2000, the start button has been disabled.", "Operating system", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnStart.Enabled = false;
            }
            else
                btnMakeList.Enabled = true;

            Debug();
            loadDefaultSettings();
            UpdateButtons();
        }

        #endregion

        #region MainProcess

        private void Start()
        {
            try
            {
                //check edit summary
                if (cmboEditSummary.Text == "")
                    MessageBox.Show("Please enter an edit summary.", "Edit summary", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                StopDelayedRestartTimer();

                DisableButtons();
                parsers.EditSummary = "";
                findAndReplace.EditSummary = "";
                if (RegexTypos != null)
                    RegexTypos.EditSummary = "";

                skippable = true;

                txtEdit.Clear();

                if (webBrowserEdit.IsBusy)
                    webBrowserEdit.Stop2();

                if (webBrowserEdit.Document != null)
                    webBrowserEdit.Document.Write("");

                //check we are logged in
                if (!WikiStatus())
                    return;

                ArticleInfo(true);

                if (lbArticles.Items.Count < 1)
                {
                    stopSaveInterval();
                    lblTimer.Text = "";
                    lblStatusText.Text = "No articles in list, you need to use the Make list";
                    this.Text = "AutoWikiBrowser";
                    webBrowserEdit.Document.Write("");
                    btnMakeList.Enabled = true;
                    return;
                }

                if (lbArticles.SelectedItem == null)
                    lbArticles.SelectedIndex = 0;

                EdittingArticle = (Article)lbArticles.SelectedItem;// lbArticles.SelectedItem.ToString();

                //Navigate to edit page
                webBrowserEdit.LoadEditPage(EdittingArticle.URLEncodedName);
            }
            catch
            {
                StartDelayedRestartTimer();
            }
        }

        private void CaseWasLoad()
        {
            string strText = "";
            Article Redirect = new Article();

            if (!loadSuccess())
                return;

            strText = webBrowserEdit.GetArticleText();

            this.Text = "AutoWikiBrowser" + strSettingsFile + " - " + EdittingArticle.Name;

            //check not in use
            if (Regex.IsMatch(strText, "\\{\\{[Ii]nuse"))
            {
                if (!chkAutoMode.Checked)
                    MessageBox.Show("This page has the \"Inuse\" tag, consider skipping it");
            }

            Match m = RedirectRegex.Match(strText);
            //check for redirect
            if (bypassRedirectsToolStripMenuItem.Checked && m.Success)
            {
                Redirect.Name = m.Groups[1].Value;

                if (Redirect.Name == EdittingArticle.Name)
                {//ignore recursice redirects
                    SkipPage();
                    return;
                }

                int intPos = 0;
                intPos = lbArticles.Items.IndexOf(EdittingArticle);

                RemoveEdittingArticle();
                lbArticles.ClearSelected();
                lbArticles.Items.Insert(intPos, EdittingArticle);

                lbArticles.SelectedItem = Redirect;
                webBrowserEdit.LoadEditPage(HttpUtility.UrlEncode(Redirect.Name));
                return;
            }

            if (chkIgnoreIfContains.Checked && IgnoreIfContains(EdittingArticle + strText,
            txtIgnoreIfContains.Text, chkIgnoreIsRegex.Checked, chkIgnoreCaseSensitive.Checked))
            {
                SkipPage();
                return;
            }

            if (chkOnlyIfContains.Checked && IgnoreIfDoesntContain(EdittingArticle + strText,
            txtOnlyIfContains.Text, chkIgnoreIsRegex.Checked, chkIgnoreCaseSensitive.Checked))
            {
                SkipPage();
                return;
            }

            if (!skipIf(strText))
            {
                SkipPage();
                return;
            }

            bool skip = false;
            if (!doNotAutomaticallyDoAnythingToolStripMenuItem.Checked)
            {
                string strOrigText = strText;
                strText = Process(strText, ref skip);

                if (skippable && chkSkipNoChanges.Checked)
                {
                    if (strText == strOrigText)
                    {
                        SkipPage();
                        return;
                    }
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
            else
                GetDiff(previewInsteadOfDiffToolStripMenuItem.Checked);
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
                string HTMLsub = HTML.Remove(HTML.IndexOf("<!-- start content -->"));
                if (HTMLsub.Contains("<DIV class=usermessage>"))
                {//check if we have any messages
                    wikiStatusBool = false;
                    UpdateButtons();
                    webBrowserEdit.Document.Write("");
                    this.Focus();

                    dlgTalk DlgTalk = new dlgTalk();
                    if (DlgTalk.ShowDialog() == DialogResult.Yes)
                        System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/User_talk:" + UserName);
                    else
                        System.Diagnostics.Process.Start("IExplore", "http://en.wikipedia.org/wiki/User_talk:" + UserName);

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
                if (webBrowserEdit.Document.GetElementById("wpTextbox1").InnerText == null && ignoreNonexistentPagesToolStripMenuItem.Checked)
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

        private bool skipIf(string articleText)
        {//custom code to skip articles can be added here
            return true;

        }

        bool skippable = true;
        private void CaseWasDiff()
        {
            this.ActiveControl = txtEdit;
            txtEdit.Select(0, 0);

            if (diffChecker(webBrowserEdit.Document.Body.InnerHtml))
            {//check if there are no changes and we want to skip
                SkipPage();
                return;
            }

            if (!AutoWikiBrowser.Properties.Settings.Default.DisableDiffScrollDown)
            {
                webBrowserEdit.Document.GetElementById("contentSub").ScrollIntoView(true);
            }

            if (chkAutoMode.Checked)
            {
                startDelayedAutoSaveTimer();
                return;
            }

            if (!this.ContainsFocus)
            {
                Tools.FlashWindow(this);
                Tools.Beep1();
            }

            EnableButtons();
        }

        private bool diffChecker(string strHTML)
        {//check diff to see if it should be skipped

            if (!skippable || !chkSkipNoChanges.Checked || previewInsteadOfDiffToolStripMenuItem.Checked || doNotAutomaticallyDoAnythingToolStripMenuItem.Checked)
                return false;

            if (!strHTML.Contains("class=diff-context") && !strHTML.Contains("class=diff-deletedline"))
                return true;

            strHTML = strHTML.Replace("<SPAN class=diffchange></SPAN>", "");
            strHTML = Regex.Match(strHTML, "<TD align=left colSpan=2.*?</DIV>", RegexOptions.Singleline).ToString();

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

            if (webBrowserEdit.Document.Body.InnerHtml.Contains("<DIV CLASS=PREVIEWNOTE><P><STRONG>Sorry! We could not process your edit due to a loss of session data."))
            {//if session data is lost, if data is lost then save after delay with tmrAutoSaveDelay
                StartDelayedRestartTimer();
                return;
            }

            //lower restart delay
            if (intRestartDelay > 5)
                intRestartDelay -= 1;

            intEdits++;

            LastArticle = "";
            RemoveEdittingArticle();
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
                stopDelayedTimer();
                webBrowserEdit.Document.Write("");
                RemoveEdittingArticle();
                Start();
            }
            catch { }
        }

        private string Process(string articleText, ref bool skip)
        {
            string testText; // use to check if changes are made
            bool process = true;

            try
            {
                if (noParse.Contains(EdittingArticle))
                    process = false;

                if (chkUnicodifyWhole.Checked && process)
                    articleText = parsers.Unicodify(articleText);

                if (cmboImages.SelectedIndex == 1)
                {
                    articleText = parsers.ReImager(txtImageReplace.Text, txtImageWith.Text, articleText, ref skip);
                    if (skip)
                        return articleText;
                    else
                        skip = false;
                }
                else if (cmboImages.SelectedIndex == 2)
                {
                    articleText = parsers.RemoveImage(txtImageReplace.Text, articleText, ref skip);
                    if (skip)
                        return articleText;
                    else
                        skip = false;
                }

                if (cmboCategorise.SelectedIndex == 1)
                {
                    articleText = parsers.ReCategoriser(txtSelectSource.Text, txtNewCategory.Text, articleText, ref skip);
                    if (skip)
                        return articleText;
                    else
                        skip = false;
                }
                else if (cmboCategorise.SelectedIndex == 2 && txtNewCategory.Text.Length > 0)
                {
                    string cat = "[[" + Variables.Namespaces[14] + txtNewCategory.Text + "]]";
                    cat = Tools.ApplyKeyWords(EdittingArticle.Name, cat);

                    if (EdittingArticle.NameSpaceKey == 10)
                        articleText += "<noinclude>\r\n" + cat + "\r\n</noinclude>";
                    else
                        articleText += cat;
                }
                else if (cmboCategorise.SelectedIndex == 3 && txtNewCategory.Text.Length > 0)
                {
                    articleText = parsers.RemoveCategory(txtNewCategory.Text, articleText, ref skip);
                    if (skip)
                        return articleText;
                    else
                        skip = false;
                }

                if (chkFindandReplace.Checked)
                {
                    testText = articleText;
                    articleText = findAndReplace.MultipleFindAndReplce(articleText, EdittingArticle.Name);
                    articleText = replaceSpecial.ApplyRules(articleText, EdittingArticle.Name);

                    if (chkIgnoreWhenNoFAR.Checked && (testText == articleText))
                    {
                        skip = true;
                        return articleText;
                    }
                }

                if (chkRegExTypo.Checked && RegexTypos != null)
                {
                    articleText = RegexTypos.PerformTypoFixes(articleText, ref skip);
                    if (skip && chkRegexTypoSkip.Checked)
                        return articleText;
                    else
                        skip = false;
                }

                if (process && chkAutoTagger.Checked)
                    articleText = parsers.Tagger(articleText, EdittingArticle.Name);

                if (process && chkGeneralParse.Checked && (EdittingArticle.NameSpaceKey == 0 || (EdittingArticle.Name.Contains("Sandbox") || EdittingArticle.Name.Contains("sandbox"))))
                {
                    articleText = parsers.RemoveNowiki(articleText);

                    if (Variables.LangCode == "en")
                    {//en only
                        articleText = parsers.Conversions(articleText);
                        articleText = parsers.RemoveBadHeaders(articleText, EdittingArticle.Name);
                        articleText = parsers.LivingPeople(articleText);
                        articleText = parsers.FixCats(articleText);
                        articleText = parsers.FixHeadings(articleText);

#if DEBUG
                        articleText = parsers.MinorThings(articleText);
#endif
                    }
                    articleText = parsers.FixSyntax(articleText);
                    articleText = parsers.FixLinks(articleText);
                    articleText = parsers.BulletExternalLinks(articleText);
                    articleText = parsers.SortMetaData(articleText, EdittingArticle.Name);
                    articleText = parsers.BoldTitle(articleText, EdittingArticle.Name);
                    articleText = parsers.LinkSimplifier(articleText);

                    articleText = parsers.AddNowiki(articleText);
                }

                if (process && chkGeneralParse.Checked && EdittingArticle.NameSpaceKey == 3)
                    articleText = parsers.SubstUserTemplates(articleText);

                if (chkAppend.Checked)
                {
                    if (Tools.IsNotTalk(EdittingArticle.Name))
                    {
                        MessageBox.Show("Messages should only be appended to talk pages.");
                    }
                    else if (rdoAppend.Checked)
                        articleText += "\r\n\r\n" + txtAppendMessage.Text;
                    else
                        articleText = txtAppendMessage.Text + "\r\n\r\n" + articleText;
                }

                return articleText;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return articleText;
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            ShowPreview();
        }

        private void btnDiff_Click(object sender, EventArgs e)
        {
            ShowDiff();
        }

        private void ShowPreview()
        {
            DisableButtons();
            LastArticle = txtEdit.Text; // added by Adrian 2006-03-12

            skippable = false;
            GetDiff(true);
        }

        private void ShowDiff()
        {
            DisableButtons();
            LastArticle = txtEdit.Text; // added by Adrian 2006-03-12

            GetDiff(false);
        }

        private void GetDiff(bool boolPreview)
        {
            if (webBrowserEdit.Document == null)
                return;

            if (!webBrowserEdit.CanDiff && !webBrowserEdit.CanPreview)
                return;

            //get either diff or preiew.
            webBrowserEdit.SetArticleText(txtEdit.Text);

            if (boolPreview)
                webBrowserEdit.ShowPreview();
            else
                webBrowserEdit.ShowDiff();
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
            catch { }
        }

        private void RemoveEdittingArticle()
        {
            boolSaved = false;
            if (lbArticles.Items.Contains(EdittingArticle))
            {
                while (lbArticles.SelectedItems.Count > 0)
                    lbArticles.SetSelected(lbArticles.SelectedIndex, false);

                txtNewArticle.Text = EdittingArticle.Name;

                int intPosition = lbArticles.Items.IndexOf(EdittingArticle);

                lbArticles.Items.Remove(EdittingArticle);

                if (lbArticles.Items.Count == intPosition)
                    intPosition--;

                if (lbArticles.Items.Count > 0)
                    lbArticles.SelectedIndex = intPosition;
            }

            UpdateButtons();
        }

        #endregion

        #region MakeList

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtNewArticle.Text.Length == 0)
            {
                UpdateButtons();
                return;
            }

            boolSaved = false;
            AddToList(Tools.TurnFirstToUpper(txtNewArticle.Text));
            txtNewArticle.Text = "";
            UpdateButtons();
        }

        private void btnRemoveArticle_Click(object sender, EventArgs e)
        {
            try
            {
                boolSaved = false;

                lbArticles.BeginUpdate();
                int i = lbArticles.SelectedIndex;

                if (lbArticles.SelectedItems.Count > 0)
                    txtNewArticle.Text = lbArticles.SelectedItem.ToString();

                while (lbArticles.SelectedItems.Count > 0)
                    lbArticles.Items.Remove(lbArticles.SelectedItem);

                if (lbArticles.Items.Count > i)
                    lbArticles.SelectedIndex = i;
                else
                    lbArticles.SelectedIndex = i - 1;

                lbArticles.EndUpdate();
            }
            catch
            { }

            UpdateButtons();
        }

        private void btnArticlesListClear_Click(object sender, EventArgs e)
        {
            boolSaved = false;
            lbArticles.Items.Clear();

            UpdateButtons();
        }

        Thread ListerThread = null;
        private void btnMakeList_Click(object sender, EventArgs e)
        {
            txtSelectSource.Text = txtSelectSource.Text.Trim('[', ']');
            if (cmboSourceSelect.SelectedIndex == 0)
                txtSelectSource.Text = Regex.Replace(txtSelectSource.Text, "^" + Variables.Namespaces[14], "", RegexOptions.IgnoreCase);
            else if (cmboSourceSelect.SelectedIndex == 6)
                txtSelectSource.Text = Regex.Replace(txtSelectSource.Text, "^" + Variables.Namespaces[2], "", RegexOptions.IgnoreCase);
            else if (cmboSourceSelect.SelectedIndex == 7)
                txtSelectSource.Text = Regex.Replace(txtSelectSource.Text, "^" + Variables.Namespaces[-1], "", RegexOptions.IgnoreCase);
            else if (cmboSourceSelect.SelectedIndex == 8)
                txtSelectSource.Text = Regex.Replace(txtSelectSource.Text, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase);
            else if (cmboSourceSelect.SelectedIndex == 9)
            {
                launchDumpSearcher();
                return;
            }

            txtSelectSource.Text = Tools.TurnFirstToUpper(txtSelectSource.Text);
            txtSelectSource.AutoCompleteCustomSource.Add(txtSelectSource.Text);

            //make sure there is some text.
            if (txtSelectSource.Text.Length == 0 && txtSelectSource.Enabled)
            {
                MessageBox.Show("Please enter some text", "No text", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!WikiStatus())
                return;

            if (cmboSourceSelect.SelectedIndex == 4)
            {
                try
                {
                    OpenFileDialog openListDialog = new OpenFileDialog();
                    openListDialog.Filter = "text files|*.txt|All files|*.*";
                    openListDialog.Multiselect = true;

                    this.Focus();
                    if (openListDialog.ShowDialog() == DialogResult.OK)
                    {
                        AddArticleListToList(GetLists.FromTextFile(openListDialog.FileNames));
                        strListFile = openListDialog.FileName;
                    }
                    UpdateButtons();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                return;
            }
            else if (cmboSourceSelect.SelectedIndex == 10)
            {
                AddArticleListToList(GetLists.FromWatchList());
                UpdateButtons();
                return;
            }

            string[] s = txtSelectSource.Text.Split('|');

            MakeList(cmboSourceSelect.SelectedIndex, s);
        }

        private void MakeList(int Source, string[] SourceValues)
        {
            intSourceIndex = Source;
            strSouce = SourceValues;

            ThreadStart thr_Process = new ThreadStart(MakeList2);
            ListerThread = new Thread(thr_Process);
            ListerThread.IsBackground = true;
            ListerThread.Start();
        }

        int intSourceIndex = 0;
        string[] strSouce;
        private void MakeList2()
        {
            boolSaved = false;
            StartProgressBar();

            try
            {
                switch (intSourceIndex)
                {
                    case 0:
                        AddArticleListToList(GetLists.FromCategory(strSouce));
                        break;
                    case 1:
                        AddArticleListToList(GetLists.FromWhatLinksHere(false, strSouce));
                        break;
                    case 2:
                        AddArticleListToList(GetLists.FromWhatLinksHere(true, strSouce));
                        break;
                    case 3:
                        AddArticleListToList(GetLists.FromLinksOnPage(strSouce));
                        break;
                    //4 from text file
                    case 5:
                        AddArticleListToList(GetLists.FromGoogleSearch(strSouce));
                        break;
                    case 6:
                        AddArticleListToList(GetLists.FromUserContribs(strSouce));
                        break;
                    case 7:
                        AddArticleListToList(GetLists.FromSpecialPage(strSouce));
                        break;
                    case 8:
                        AddArticleListToList(GetLists.FromImageLinks(strSouce));
                        break;
                    //9 from datadump
                    //10 from watchlist
                    default:
                        break;
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (PageDoeNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Page does not exist error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unexpected error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                StopProgressBar();
            }
        }

        private delegate void SetProgBarDelegate();
        private void StopProgressBar()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new SetProgBarDelegate(StopProgressBar));
                return;
            }
            if (!toolStripProgressBar1.IsDisposed)
            {
                this.Cursor = Cursors.Default;
                btnMakeList.Enabled = true;
                lblStatusText.Text = "List complete!";
                toolStripProgressBar1.MarqueeAnimationSpeed = 0;
                toolStripProgressBar1.Style = ProgressBarStyle.Continuous;
                UpdateButtons();
            }
        }

        private delegate void StartProgBarDelegate();
        private void StartProgressBar()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new StartProgBarDelegate(StartProgressBar));
                return;
            }
            this.Cursor = Cursors.WaitCursor;
            lblStatusText.Text = "Getting list";
            btnMakeList.Enabled = false;
            toolStripProgressBar1.MarqueeAnimationSpeed = 100;
            toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
        }

        private delegate void AddToListDel(string s);
        private void AddToList(string s)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new AddToListDel(AddToList), s);
                return;
            }

            Article a = new Article(s);
            lbArticles.Items.Add(a);
            UpdateButtons();
        }

        private delegate void AddArticleListDel(List<Article> l);
        private void AddArticleListToList(List<Article> l)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new AddArticleListDel(AddArticleListToList), l);
                return;
            }

            lbArticles.BeginUpdate();

            foreach (Article a in l)
            {
                if (!lbArticles.Items.Contains(a))
                    lbArticles.Items.Add(a);
            }

            lbArticles.EndUpdate();

            UpdateButtons();
        }

        private List<Article> ArticleListFromListBox()
        {
            List<Article> list = new List<Article>();


            foreach (Article a in lbArticles.Enumerate())
            {
                list.Add(a);
            }

            return list;
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

        private void convertToTalkPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Article> list = ArticleListFromListBox();
            list = GetLists.ConvertToTalk(list);
            lbArticles.Items.Clear();
            AddArticleListToList(list);
        }

        private void convertFromTalkPagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Article> list = ArticleListFromListBox();
            list = GetLists.ConvertFromTalk(list);
            lbArticles.Items.Clear();
            AddArticleListToList(list);
        }

        private void fromCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] c = new string[lbArticles.SelectedItems.Count];

            int i = 0;
            foreach (object o in lbArticles.SelectedItems)
            {
                c[i] = o.ToString();
                i++;
            }

            MakeList(0, c);
        }

        private void fromWhatlinkshereToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] c = new string[lbArticles.SelectedItems.Count];

            int i = 0;
            foreach (object o in lbArticles.SelectedItems)
            {
                c[i] = o.ToString();
                i++;
            }

            MakeList(1, c);
        }

        private void fromLinksOnPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] c = new string[lbArticles.SelectedItems.Count];

            int i = 0;
            foreach (object o in lbArticles.SelectedItems)
            {
                c[i] = o.ToString();
                i++;
            }

            MakeList(3, c);
        }

        private void fromImageLinksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] c = new string[lbArticles.SelectedItems.Count];

            int i = 0;
            foreach (object o in lbArticles.SelectedItems)
            {
                c[i] = o.ToString();
                i++;
            }

            MakeList(8, c);
        }
       
        #endregion

        #region extra stuff

        private void UpdateStatus()
        {
            lblStatusText.Text = webBrowserEdit.Status;
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
            ExitQuestion dlg = new ExitQuestion(Time, intEdits, msg);
            dlg.ShowDialog();
            if (dlg.DialogResult == DialogResult.OK)
            {
                AutoWikiBrowser.Properties.Settings.Default.DontAskForTerminate
                = dlg.checkBoxDontAskAgain;

                // save user persistent settings
                AutoWikiBrowser.Properties.Settings.Default.Save();
            }
            else
            {
                e.Cancel = true;
            }
            dlg = null;

            Stop();
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

        private string EditSummary
        {
            get
            {
                string tag = "";
                if (findAndReplace.AppendToSummary)
                    tag = tag += findAndReplace.EditSummary;
                if (chkRegExTypo.Checked && RegexTypos != null)
                    tag = tag += RegexTypos.EditSummary;

                return tag;
            }
        }

        private string MakeSummary()
        {
            string tag = cmboEditSummary.Text + parsers.EditSummary;
            tag = tag += EditSummary;
            if (!chkSuppressTag.Enabled || !chkSuppressTag.Checked)
                tag += " " + Variables.SummaryTag;

            return tag;
        }

        private void chkFindandReplace_CheckedChanged(object sender, EventArgs e)
        {
            btnMoreFindAndReplce.Enabled = chkFindandReplace.Checked;
            btnFindAndReplaceAdvanced.Enabled = chkFindandReplace.Checked;
            chkIgnoreWhenNoFAR.Enabled = chkFindandReplace.Checked;
        }

        private void cmboCategorise_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmboCategorise.SelectedIndex > 0)
            {
                if (cmboCategorise.SelectedIndex == 1 && (txtSelectSource.Text.Length == 0 || cmboSourceSelect.SelectedIndex != 0))
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

                //stop the process stage being confused when the webbrowser document completed event fires.
                webBrowserEdit.ProcessStage = enumProcessStage.none;
                string strInnerHTML;

                //don't require to log in in other languages.
                if (Variables.LangCode != "en" || Variables.Project != "wikipedia")
                {
                    lblStatusText.Text = "Loading page to check if we are logged in";
                    webBrowserLogin.Navigate(Variables.URLShort + "/wiki/Main_Page");
                    //wait to load
                    while (webBrowserLogin.ReadyState != WebBrowserReadyState.Complete) Application.DoEvents();
                    strInnerHTML = webBrowserLogin.Document.Body.InnerHtml;

                    if (!strInnerHTML.Contains("<LI id=pt-logout"))
                    {//see if we are logged in
                        MessageBox.Show("You are not logged in. The log in screen will now load, enter your name and password, click \"Log in\", wait for it to complete, then start the process again.\r\n\r\nIn the future you can make sure this won't happen by logging in to Wikipedia using Microsoft Internet Explorer.", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        webBrowserEdit.LoadLogInPage();
                        return false;
                    }
                    else
                    {
                        wikiStatusBool = true;
                        chkAutoMode.Enabled = true;
                        return true;
                    }
                }

                //load check page
                lblStatusText.Text = "Loading page to check if we are logged in and bot is enabled";
                webBrowserLogin.Navigate("http://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/CheckPage&action=edit");
                //wait to load
                while (webBrowserLogin.ReadyState != WebBrowserReadyState.Complete) Application.DoEvents();

                strInnerHTML = webBrowserLogin.Document.Body.InnerHtml;

                if (!webBrowserLogin.LoggedIn)
                {//see if we are logged in
                    MessageBox.Show("You are not logged in. The log in screen will now load, enter your name and password, click \"Log in\", wait for it to complete, then start the process again.\r\n\r\nIn the future you can make sure this won't happen by logging in to Wikipedia using Microsoft Internet Explorer.", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    webBrowserEdit.LoadLogInPage();
                    return false;
                }
                else if (!strInnerHTML.Contains("enabledusersbegins"))
                {
                    MessageBox.Show("Check page failed to load.\r\n\r\nCheck your Internet Explorer is working and that the Wikipedia servers are online, also try clearing Internet Explorer cache.", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                else if (!strInnerHTML.Contains(Assembly.GetExecutingAssembly().GetName().Version.ToString() + " enabled"))
                {//see if this version is enabled
                    MessageBox.Show("This version is not enabled, please download the newest version. If you have the newest version, check that Wikipedia is online.", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/Wikipedia:AutoWikiBrowser");
                    return false;
                }
                else
                {//see if we are allowed to use this softare

                    UserName = webBrowserLogin.GetUserName();
                    strInnerHTML = strInnerHTML.Substring(strInnerHTML.IndexOf("enabledusersbegins"), strInnerHTML.IndexOf("enabledusersends") - strInnerHTML.IndexOf("enabledusersbegins"));
                    string strBotUsers = strInnerHTML.Substring(strInnerHTML.IndexOf("enabledbots"), strInnerHTML.IndexOf("enabledbotsends") - strInnerHTML.IndexOf("enabledbots"));

                    if (UserName.Length > 0 && strInnerHTML.Contains("* " + UserName + "\r\n") || strInnerHTML.Contains("Everybody enabled = true"))
                    {
                        if (strBotUsers.Contains("* " + UserName + "\r\n"))
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
                        MessageBox.Show("You are not enabled to use this.", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/Wikipedia:AutoWikiBrowser/CheckPage");
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
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
                stopDelayedTimer();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TimeSpan Time = new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            Time = Time.Subtract(StartTime);
            AboutBox About = new AboutBox(webBrowserEdit.Version.ToString(), Time, intEdits);
            About.Show();
        }

        private void cmbSourceSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmboSourceSelect.SelectedIndex)
            {
                case 0:
                    lblSourceSelect.Text = Variables.Namespaces[14];
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    return;
                case 1:
                    lblSourceSelect.Text = "What links to";
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    return;
                case 2:
                    lblSourceSelect.Text = "What embeds";
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    return;
                case 3:
                    lblSourceSelect.Text = "Links on";
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    return;
                case 4:
                    lblSourceSelect.Text = "From file";
                    txtSelectSource.Enabled = false;
                    chkWLHRedirects.Visible = false;
                    return;
                case 5:
                    lblSourceSelect.Text = "Google search";
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    return;
                case 6:
                    lblSourceSelect.Text = Variables.Namespaces[2];
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    return;
                case 7:
                    lblSourceSelect.Text = Variables.Namespaces[-1];
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    return;
                case 8:
                    lblSourceSelect.Text = Variables.Namespaces[6];
                    txtSelectSource.Enabled = true;
                    chkWLHRedirects.Visible = false;
                    return;
                default:
                    lblSourceSelect.Text = "";
                    txtSelectSource.Enabled = false;
                    chkWLHRedirects.Visible = false;
                    return;
            }
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
            txtIgnoreIfContains.Enabled = chkIgnoreIfContains.Checked;
        }

        private void chkOnlyIfContains_CheckedChanged(object sender, EventArgs e)
        {
            txtOnlyIfContains.Enabled = chkOnlyIfContains.Checked;
        }

        private void saveListToTextFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveList();
        }
        private void SaveList()
        {//Save lbArticles list to text file.
            try
            {
                StringBuilder strList = new StringBuilder("");

                foreach (Article a in lbArticles.Enumerate())
                {
                    strList.Append("# [[" + a.Name + "]]\r\n");
                }

                if (strListFile.Length > 0)
                    saveListDialog.FileName = strListFile;

                if (saveListDialog.ShowDialog() == DialogResult.OK)
                {
                    strListFile = saveListDialog.FileName;
                    StreamWriter sw = new StreamWriter(strListFile, false, Encoding.UTF8);
                    sw.Write(strList);
                    sw.Close();
                    boolSaved = true;
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

        private void FilterArticles()
        {
            //filter out non-mainspace articles
            int i = 0;
            string s = "";

            while (i < lbArticles.Items.Count)
            {
                s = lbArticles.Items[i].ToString();

                if (!Tools.IsMainSpace(s))
                    lbArticles.Items.Remove(lbArticles.Items[i]);
                else //move on
                    i++;
            }
            UpdateButtons();
        }

        private void specialFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            specialFilter();
        }

        private void specialFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            specialFilter();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            specialFilter();
        }

        private void specialFilter()
        {
            specialFilter SepcialFilter = new specialFilter(lbArticles);
            SepcialFilter.ShowDialog();
            UpdateButtons();
        }

        private void txtNewCategory_Leave(object sender, EventArgs e)
        {
            txtNewCategory.Text = txtNewCategory.Text.Trim('[', ']');
            txtNewCategory.Text = Regex.Replace(txtNewCategory.Text, "^[Cc]ategory:", "");
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

                foreach (Match m in Regex.Matches(ArticleText, "\\[\\[([a-z]{2,3}|simple|fiu-vro|minnan|roa-rup|tokipona|zh-min-nan):.*\\]\\]"))
                    intInterLinks++;

                foreach (Match m in Regex.Matches(ArticleText, "\\[\\["))
                    intLinks++;

                intLinks = intLinks - intInterLinks - intImages - intCats;

                if (EdittingArticle.NameSpaceKey == 0 && (Regex.IsMatch(ArticleText, "[Ss]tub\\}\\}")) && (intWords > 500))
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
                foreach (Match m in WikiLinkRegex.Matches(ArticleText))
                {
                    x = m.Groups[1].Value;
                    if (!Regex.IsMatch(x, "^(January|February|March|April|May|June|July|August|September|October|November|December) [0-9]{1,2}$") && !Regex.IsMatch(x, "^[0-9]{1,2} (January|February|March|April|May|June|July|August|September|October|November|December)$"))
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
            if (!Regex.IsMatch(e.KeyChar.ToString(), "[0-9]") && e.KeyChar != 8)
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

        private bool IgnoreIfContains(string strArticle, string strFind, bool Regexe, bool caseSensitive)
        {
            if (strFind.Length > 0)
            {
                RegexOptions RegOptions;

                if (caseSensitive)
                    RegOptions = RegexOptions.None;
                else
                    RegOptions = RegexOptions.IgnoreCase;

                strFind = Tools.ApplyKeyWords(EdittingArticle.Name, strFind);

                if (!Regexe)
                    strFind = Regex.Escape(strFind);

                if (Regex.IsMatch(strArticle, strFind, RegOptions))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }


        private bool IgnoreIfDoesntContain(string strArticle, string strFind, bool Regexe, bool caseSensitive)
        {
            if (strFind.Length > 0)
            {
                RegexOptions RegOptions;

                if (caseSensitive)
                    RegOptions = RegexOptions.None;
                else
                    RegOptions = RegexOptions.IgnoreCase;

                strFind = Tools.ApplyKeyWords(EdittingArticle.Name, strFind);

                if (!Regexe)
                    strFind = Regex.Escape(strFind);

                if (Regex.IsMatch(strArticle, strFind, RegOptions))
                    return false;
                else
                    return true;
            }
            else
                return false;
        }

        [Conditional("DEBUG")]
        public void Debug()
        {//stop logging in when de-bugging
            AddToList("User:Bluemoose/Sandbox");
            wikiStatusBool = true;
            chkAutoMode.Enabled = true;
            chkQuickSave.Enabled = true;
        }

        #endregion

        #region set variables

        private void selectProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProjectSelect Proj = new ProjectSelect(Variables.LangCode, Variables.Project);
            Proj.ShowDialog();
            if (Proj.DialogResult == DialogResult.OK)
            {
                wikiStatusBool = false;
                chkQuickSave.Checked = false;
                chkQuickSave.Enabled = false;
                chkAutoMode.Checked = false;
                chkAutoMode.Enabled = false;

                SetProject(Proj.Language, Proj.Project);
                if (Proj.SetAsDefault)
                {
                    AutoWikiBrowser.Properties.Settings.Default.Language = Variables.LangCode;
                    AutoWikiBrowser.Properties.Settings.Default.Project = Variables.Project;
                    AutoWikiBrowser.Properties.Settings.Default.Save();
                }
            }
            Proj = null;
        }

        private void SetProject(LangCodeEnum Code, ProjectEnum Project)
        {
            //set namespaces
            Variables.SetProject(Code, Project);

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
                chkGeneralParse.Checked = false;
            }
            lblProject.Text = Variables.LangCode + "." + Variables.Project;
        }

        #endregion

        #region Enabling/Disabling of buttons

        private void UpdateButtons()
        {
            NumberOfArticles = lbArticles.Items.Count;
            bool enabled = lbArticles.Items.Count > 0;
            btnStart.Enabled = enabled;
            btnFilter.Enabled = enabled;
            btnRemoveArticle.Enabled = enabled;
            btnArticlesListClear.Enabled = enabled;
            btnArticlesListSave.Enabled = enabled;
        }

        private void DisableStartButton()
        {
            btnStart.Enabled = false;
        }

        private void DisableButtons()
        {
            DisableStartButton();
            btnSave.Enabled = false;

            if (lbArticles.Items.Count == 0)
                btnIgnore.Enabled = false;

            btnPreview.Enabled = false;
            btnDiff.Enabled = false;
            btntsPreview.Enabled = false;
            btntsChanges.Enabled = false;

            btnMakeList.Enabled = false;

            btntsSave.Enabled = false;
            btntsIgnore.Enabled = false;
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

            btnMakeList.Enabled = true;

            btntsSave.Enabled = true;
            btntsIgnore.Enabled = true;

        }

        #endregion

        #region timers

        int intRestartDelay = 5;
        int intStartInSeconds = 5;
        private void DelayedRestart()
        {
            stopDelayedTimer();
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

        private void stopDelayedTimer()
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
                stopDelayedTimer();
                SaveArticle();
            }

            lblBotTimer.Text = "Bot timer: " + intTimer.ToString();
        }

        private void showTimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopSaveInterval();
        }
        int intStartTimer = 0;
        private void SaveInterval()
        {
            intStartTimer++;
            lblTimer.Text = intStartTimer.ToString();
        }
        private void stopSaveInterval()
        {
            intStartTimer = 0;
            lblTimer.Text = intStartTimer.ToString();
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
        }

        #endregion

        #region menus and buttons

        private void lbArticles_MouseMove(object sender, MouseEventArgs e)
        {
            string strTip = "";

            //Get the item
            int nIdx = lbArticles.IndexFromPoint(e.Location);
            if ((nIdx >= 0) && (nIdx < lbArticles.Items.Count))
                strTip = lbArticles.Items[nIdx].ToString();

            toolTip1.SetToolTip(lbArticles, strTip);
        }

        private void lbArticles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                btnRemoveArticle.PerformClick();
        }

        private void txtNewArticle_MouseMove(object sender, MouseEventArgs e)
        {
            toolTip1.SetToolTip(txtNewArticle, txtNewArticle.Text);
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
            WikiFunctions.DatabaseScanner.DatabaseScanner ds = new WikiFunctions.DatabaseScanner.DatabaseScanner(lbArticles);
            ds.Show();
            UpdateButtons();
        }

        private void addIgnoredToLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnFalsePositive.Visible = addIgnoredToLogFileToolStripMenuItem.Checked;
            tsbuttonFalsePositive.Visible = addIgnoredToLogFileToolStripMenuItem.Checked;
        }

        private void alphaSortInterwikiLinksToolStripMenuItem_CheckStateChanged(object sender, EventArgs e)
        {
            parsers.sortInterwikiOrder = alphaSortInterwikiLinksToolStripMenuItem.Checked;
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

        private void txtSelectSource_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                e.Handled = true;
                btnMakeList.PerformClick();
            }
        }
        private void txtNewArticle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                e.Handled = true;
                btnAdd.PerformClick();
            }
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = Tools.HTMLToWiki(txtEdit.SelectedText, "*");
        }

        private void listToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            txtEdit.SelectedText = Tools.HTMLToWiki(txtEdit.SelectedText, "#");
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
            txtEdit.Text = "{{Wikify-date|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + txtEdit.Text;
        }

        private void cleanupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtEdit.Text = "{{cleanup-date|{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + txtEdit.Text;
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
            txtEdit.SelectedText = "[[Category:Category needed]]";
        }

        private void unicodifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string text = txtEdit.SelectedText;
            text = parsers.Unicodify(text);
            txtEdit.SelectedText = text;
        }

        private void btnFilterList_Click(object sender, EventArgs e)
        {
            FilterArticles();
        }

        private void filterOutNonMainSpaceArticlesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterArticles();
        }

        private void sortAlphebeticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbArticles.Sorted = true;
            lbArticles.Sorted = false;
        }

        private void clearTheListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbArticles.Items.Clear();
            UpdateButtons();
        }

        private void saveListToTextFileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveList();
        }

        private void saveListToTextFileToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SaveList();
        }

        private void filterOutNonMainSpaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FilterArticles();
        }

        private void sortAlphabeticallyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbArticles.Sorted = true;
            lbArticles.Sorted = false;
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnRemoveArticle.PerformClick();
        }

        private void mnuListBox_Opening(object sender, CancelEventArgs e)
        {
            bool boolEnabled = lbArticles.Items.Count > 0;

            if (lbArticles.SelectedItems.Count == 1)
            {
                if (lbArticles.SelectedItem.ToString().StartsWith(Variables.Namespaces[14]))
                    fromCategoryToolStripMenuItem.Enabled = true;
                else
                    fromCategoryToolStripMenuItem.Enabled = false;

                if (lbArticles.SelectedItem.ToString().StartsWith(Variables.Namespaces[6]))
                    fromImageLinksToolStripMenuItem.Enabled = true;
                else
                    fromImageLinksToolStripMenuItem.Enabled = false;
            }

            addSelectedToListToolStripMenuItem.Enabled = lbArticles.SelectedItems.Count > 0;

            removeToolStripMenuItem.Enabled = lbArticles.SelectedItem != null;
            clearToolStripMenuItem1.Enabled = boolEnabled;
            filterOutNonMainSpaceArticlesToolStripMenuItem.Enabled = boolEnabled;
            convertToTalkPagesToolStripMenuItem.Enabled = boolEnabled;
            convertFromTalkPagesToolStripMenuItem.Enabled = boolEnabled;
            sortAlphebeticallyMenuItem.Enabled = boolEnabled;
            saveListToTextFileToolStripMenuItem1.Enabled = boolEnabled;
            specialFilterToolStripMenuItem.Enabled = boolEnabled;
        }

        private void clearToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            btnArticlesListClear.PerformClick();
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
            ArrayList dates = new ArrayList();
            string pattern = "[1-2][0-9]{3}";

            try
            {
                foreach (Match m in Regex.Matches(txtEdit.Text, pattern))
                {
                    string s = m.ToString();
                    dates.Add(s);
                }

                if (dates.Count >= 1)
                    strBirth = dates[0].ToString();
                if (dates.Count >= 2)
                    strDeath = dates[1].ToString();

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

            if (EdittingArticle.Name.Length > 0)
                openPageInBrowserToolStripMenuItem.Enabled = true;
            else
                openPageInBrowserToolStripMenuItem.Enabled = false;

            if (LastArticle.Length > 0)
                replaceTextWithLastEditToolStripMenuItem.Enabled = true;
            else
                replaceTextWithLastEditToolStripMenuItem.Enabled = false;
        }

        private void openPageInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Variables.URL + "index.php?title=" + EdittingArticle.URLEncodedName);
        }

        private void previewInsteadOfDiffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            chkSkipNoChanges.Enabled = !previewInsteadOfDiffToolStripMenuItem.Checked;
        }

        private void chkGeneralParse_CheckedChanged(object sender, EventArgs e)
        {
            alphaSortInterwikiLinksToolStripMenuItem.Enabled = chkGeneralParse.Checked;
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
                stopDelayedTimer();
                EnableButtons();
                return;
            }

            if (ListerThread != null)
                ListerThread.Abort();

            stopSaveInterval();
            StopDelayedRestartTimer();
            if (webBrowserEdit.IsBusy)
                webBrowserEdit.Stop2();

            webBrowserLogin.Stop();
            lblStatusText.Text = "Stopped";

            StopProgressBar();
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/Wikipedia:AutoWikiBrowser#User_manual");
        }

        private void reparseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool b = true;
            txtEdit.Text = Process(txtEdit.Text, ref b);
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
            txtEdit.Text = parsers.RemoveAllWhiteSpace(txtEdit.Text);
        }

        private void txtSelectSource_DoubleClick(object sender, EventArgs e)
        {
            txtSelectSource.SelectAll();
        }

        private void txtNewArticle_DoubleClick(object sender, EventArgs e)
        {
            txtNewArticle.SelectAll();
        }

        private void txtNewCategory_DoubleClick(object sender, EventArgs e)
        {
            txtNewCategory.SelectAll();
        }

        private void btnArticlesListSave_Click(object sender, EventArgs e)
        {
            SaveList();
        }

        private void cmboEditSummary_MouseMove(object sender, MouseEventArgs e)
        {
            if (EditSummary == "")
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

                MessageBox.Show(@"
1.  Check each edit before you make it. Although this has been built to be very accurate there is always the possibility of an error which requires your attention to watch out for.

2. Optional:  Select [[WP:RETF|RegExTypoFix]] as the edit summary in the drop down box when doing spelling corrections.  This lets everyone know where to bring issues with the typo correction. 

3. The newest typos will now be downloaded from http://en.wikipedia.org/wiki/Wikipedia:AutoWikiBrowser/Typos

Thank you for taking the time to help the encyclopedia. RegExTypoFix is developed by hand by User:Mboverload.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                if (RegexTypos == null)
                {
                    RegexTypos = new RegExTypoFix();
                    lblStatusText.Text = RegexTypos.NumberofTypos.ToString() + " typos loaded";
                }
            }

            chkRegexTypoSkip.Enabled = chkRegExTypo.Checked;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/User:Mboverload/RegExTypoFix");
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
            ShowPreview();
        }

        private void btntsChanges_Click(object sender, EventArgs e)
        {
            ShowDiff();
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
            else
            {
                lblImageReplace.Text = "Remove image:";
                lblImageWith.Text = "";
                txtImageWith.Visible = false;
                txtImageReplace.Enabled = true;
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

        #endregion

    }
}