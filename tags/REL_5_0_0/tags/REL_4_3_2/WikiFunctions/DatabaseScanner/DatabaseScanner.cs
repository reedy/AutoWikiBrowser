/*
Database Scanner
Copyright (C) 2007 Martin Richards

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
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.Threading;
using WikiFunctions.Parse;
using WikiFunctions.Lists;
using WikiFunctions.Controls.Lists;

namespace WikiFunctions.DBScanner
{
    /// <summary>
    /// Provides a form and functions for searching XML data dumps
    /// </summary>
    public partial class DatabaseScanner : Form
    {
        MainProcess Main;
        TimeSpan StartTime;
        ListBox AWBListbox;

        SpecialFilter SpecialFilter = new SpecialFilter();

        ThreadPriority priority = ThreadPriority.Normal;
        ThreadPriority Priority
        {
            get { return priority; }
            set
            {
                if (Main != null)
                    Main.Priority = value;
                priority = value;
            }
        }

        int intMatches;
        int intLimit = 100000;

        public DatabaseScanner()
        {
            InitializeComponent();
        }

        public DatabaseScanner(ListBox l)
        {
            InitializeComponent();
            AWBListbox = l;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmboLength.SelectedIndex = 0;
            cmboLinks.SelectedIndex = 0;
            cmboWords.SelectedIndex = 0;
        }

        #region main process

        private void StartButton()
        {
            try
            {
                if (Main != null)
                    return;

                if (fileName.Length == 0)
                {
                    MessageBox.Show("Please open an \"Articles\" XML data-dump file from the file menu\r\n\r\nSee the About menu for where to download this file.", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                lbArticles.Items.Clear();
                lblCount.Text = "";

                intMatches = 0;

                timerProgessUpdate.Enabled = true;

                UpdateControls(true);

                txtStartFrom.Text = Tools.TurnFirstToUpper(txtStartFrom.Text);
                Start();

            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        Regex TitleDoesRegex;
        Regex TitleDoesNotRegex;
        Regex ArticleDoesRegex;
        Regex ArticleDoesNotRegex;
        List<int> namespaces = new List<int>();
        private void makePatterns()
        {
            string strTitleNot = "";
            string strTitle = "";
            RegexOptions titleRegOptions;

            string strArticleDoes = "";
            string strArticleDoesNot = "";
            RegexOptions articleRegOptions;

            strTitle = convert(txtTitleContains.Text);
            strTitleNot = convert(txtTitleNotContains.Text);
            strArticleDoes = convert(txtArticleDoesContain.Text);
            strArticleDoesNot = convert(txtArticleDoesNotContain.Text);

            articleRegOptions = RegexOptions.Compiled;
            titleRegOptions = RegexOptions.Compiled;

            if (!chkRegex.Checked)
            {
                strArticleDoes = Regex.Escape(strArticleDoes);
                strArticleDoesNot = Regex.Escape(strArticleDoesNot);
            }

            if (!chkTitleRegex.Checked)
            {
                strTitle = Regex.Escape(strTitle);
                strTitleNot = Regex.Escape(strTitleNot);
            }

            if (!chkCaseSensitive.Checked)
                articleRegOptions = articleRegOptions | RegexOptions.IgnoreCase;
            if (chkMulti.Checked)
                articleRegOptions = articleRegOptions | RegexOptions.Multiline;
            if (chkSingle.Checked)
                articleRegOptions = articleRegOptions | RegexOptions.Singleline;

            if (!chkTitleCase.Checked)
                titleRegOptions = titleRegOptions | RegexOptions.IgnoreCase;

            ArticleDoesRegex = new Regex(strArticleDoes, articleRegOptions);
            ArticleDoesNotRegex = new Regex(strArticleDoesNot, articleRegOptions);

            TitleDoesRegex = new Regex(strTitle, titleRegOptions);
            TitleDoesNotRegex = new Regex(strTitleNot, titleRegOptions);

            namespaces.Clear();

            namespaces.Add(8);
            namespaces.Add(100);

            if (ignoreCategoryNamespaceToolStripMenuItem.Checked)
                namespaces.Add(14);

            if (ignoreImagesToolStripMenuItem.Checked)
                namespaces.Add(6);

            if (ignoreTemplateNamespaceToolStripMenuItem.Checked)
                namespaces.Add(10);

            if (ignoreWikipediaNamespaceToolStripMenuItem.Checked)
                namespaces.Add(4);

            if (ignoreMainNamespaceToolStripMenuItem.Checked)
                namespaces.Add(0);
        }

        private void Start()
        {
            makePatterns();

            StartTime = new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            intLimit = (int)nudLimitResults.Value;

            List<Scan> s = new List<Scan>();

            s.Add(new CheckNamespace(namespaces));

            if (ignoreRedirectsToolStripMenuItem1.Checked)
                s.Add(new IsNotRedirect());

            if (chkArticleDoesContain.Checked)
                s.Add(new TextDoesContain(ArticleDoesRegex));

            if (chkArticleDoesNotContain.Checked)
                s.Add(new TextDoesNotContain(ArticleDoesNotRegex));

            if (chkTitleContains.Checked)
                s.Add(new TitleDoesContain(TitleDoesRegex));

            if (chkTitleDoesNotContain.Checked)
                s.Add(new TitleDoesNotContain(TitleDoesNotRegex));

            if (cmboLength.SelectedIndex != 0)
            {
                if (cmboLength.SelectedIndex == 1)
                    s.Add(new CountCharacters(MoreLessThan.MoreThan, (int)nudLength.Value));
                else if (cmboLength.SelectedIndex == 2)
                    s.Add(new CountCharacters(MoreLessThan.LessThan, (int)nudLength.Value));
            }

            if (cmboLinks.SelectedIndex != 0)
            {
                if (cmboLinks.SelectedIndex == 1)
                    s.Add(new CountLinks(MoreLessThan.MoreThan, (int)nudLinks.Value));
                else if (cmboLinks.SelectedIndex == 2)
                    s.Add(new CountLinks(MoreLessThan.LessThan, (int)nudLinks.Value));
            }

            if (cmboWords.SelectedIndex != 0)
            {
                if (cmboWords.SelectedIndex == 1)
                    s.Add(new CountWords(MoreLessThan.MoreThan, (int)nudWords.Value));
                else if (cmboWords.SelectedIndex == 2)
                    s.Add(new CountWords(MoreLessThan.LessThan, (int)nudWords.Value));
            }

            Parsers parsers = new Parsers();

            if (rdoBadLinks.Checked)
                s.Add(new HasBadLinks());
            else if (rdoNoBold.Checked)
                s.Add(new HasNoBoldTitle(parsers));
            else if (rdoSimpleLinks.Checked)
                s.Add(new HasSimpleLinks());
            else if (rdoHasHTML.Checked)
                s.Add(new HasHTMLEntities(parsers));
            else if (rdoHeaderError.Checked)
                s.Add(new HasSectionError(parsers));
            else if (rdoUnbulletedLinks.Checked)
                s.Add(new HasUnbulletedLinks());
            else if (rdoTypo.Checked)
                s.Add(new Typo());

            Main = new MainProcess(s, fileName, Priority, ignoreCommentsToolStripMenuItem.Checked, txtStartFrom.Text);
            progressBar1.Maximum = (int)(Main.stream.Length / 1024);
            Main.FoundArticle += MessageReceived;
            Main.StoppedEvent += Stopped;
            Main.Start();
        }

        private void MessageReceived(object msg)
        {
            Article a = new Article(msg.ToString());
            lbArticles.Items.Add(a);

            if (AWBListbox != null)
                AWBListbox.Items.Add(a);

            intMatches++;

            if (intMatches >= intLimit)
                Main.Run = false;
            lblCount.Text = intMatches.ToString();
        }

        private void Stopped()
        {
            try
            {
                if (Main != null)
                {
                    Main.FoundArticle -= MessageReceived;
                    Main.StoppedEvent -= Stopped;
                }

                progressBar1.Value = 0;

                timerProgessUpdate.Enabled = false;

                lblCount.Text = lbArticles.Items.Count.ToString() + " results";

                TimeSpan endTime = new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
                endTime = endTime.Subtract(StartTime);

                if (Main != null && Main.Message)
                    MessageBox.Show(lbArticles.Items.Count.ToString() + " matches in " + endTime.ToString().TrimEnd('0'));

                Main = null;
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
            finally
            {
                UpdateControls(false);
            }
        }

        #endregion

        # region other

        private void wikifyToList()
        {
            StringBuilder strbList = new StringBuilder("");
            int i = 0;
            string s = "";
            string l = "";
            string sr = "";
            string strBullet = "#";
            int intSection = 0;
            int intSectionNumber = 0;
            int intHeadingSpace = Convert.ToInt32(nudHeadingSpace.Value);

            if (rdoHash.Checked)
                strBullet = "#";
            else
                strBullet = "*";

            if (chkHeading.Checked)
            {
                strbList.AppendLine("==0==");
                intSectionNumber++;

                while (i < lbArticles.Items.Count)
                {
                    Article a = lbArticles.Items[i] as Article;
                    s = a.ToString().Replace("&amp;", "&");
                    if (a.NameSpaceKey == 6) s = ":" + s; //images should be inlined

                    strbList.AppendLine(strBullet + " [[" + s + "]]");

                    intSection++;
                    if (intSection == intHeadingSpace)
                    {
                        strbList.AppendLine("\r\n==" + intSectionNumber + "==");
                        intSectionNumber++;
                        intSection = 0;
                    }

                    i++;
                }
            }
            else if (chkABCHeader.Checked)
            {
                while (i < lbArticles.Items.Count)
                {
                    s = lbArticles.Items[i].ToString();

                    s = s.Replace("&amp;", "&");

                    if (s.Length > 1)
                        sr = s.Remove(1);
                    else
                        sr = s;

                    if (sr != l)
                        strbList.AppendLine("\r\n== " + sr + " ==");

                    strbList.AppendLine(strBullet + " [[" + s + "]]");

                    l = sr;

                    i++;
                }
            }
            else
            {
                while (i < lbArticles.Items.Count)
                {
                    s = lbArticles.Items[i].ToString();

                    s = s.Replace("&amp;", "&");

                    strbList.AppendLine(strBullet + " [[" + s + "]]");

                    intSection++;
                    i++;
                }
            }

            txtList.Text = strbList.ToString().Trim();
        }

        string strfileName = "";
        private string fileName
        {
            get { return strfileName; }
            set
            {
                strfileName = value;

                if (value.Length > 0)
                {
                    string shortened = value.Substring(fileName.LastIndexOf("\\") + 1);
                    this.Text = "Wiki Database Scanner - " + shortened;
                }
                else
                    this.Text = "Wiki Data Database Scanner";
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (openXMLDialog.ShowDialog() == DialogResult.OK)
                    fileName = openXMLDialog.FileName;
            }
            catch (Exception ex) { ErrorHandler.Handle(ex); }
        }

        private void Save()
        {
            try
            {
                string strList = txtList.Text;

                if (saveFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter sw = new StreamWriter(saveFileDialog2.FileName, false, Encoding.UTF8);
                    sw.Write(strList);
                    sw.Close();
                }
            }
            catch (Exception ex) { ErrorHandler.Handle(ex); }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.About();
        }

        private void chkRegex_CheckedChanged(object sender, EventArgs e)
        {
            bool regex = chkRegex.Checked;
            chkMulti.Enabled = regex;
            chkSingle.Enabled = regex;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lblCount.Text = "";
            txtList.Clear();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            txtList.SelectAll();
            txtList.Copy();
            txtList.Select(0, 0);
        }

        private void chkDoesContain_CheckedChanged(object sender, EventArgs e)
        {
            txtArticleDoesContain.Enabled = chkArticleDoesContain.Checked;
        }

        private void chkDoesNotContain_CheckedChanged(object sender, EventArgs e)
        {
            txtArticleDoesNotContain.Enabled = chkArticleDoesNotContain.Checked;
        }

        private void chkCheckTitle_CheckedChanged(object sender, EventArgs e)
        {
            txtTitleContains.Enabled = chkTitleContains.Checked;
        }

        private void chkCheckNotInTitle_CheckedChanged(object sender, EventArgs e)
        {
            txtTitleNotContains.Enabled = chkTitleDoesNotContain.Checked;
        }

        private void cmboLength_SelectedIndexChanged(object sender, EventArgs e)
        {
            nudLength.Enabled = !(cmboLength.SelectedIndex == 0);
        }

        private void cmboWords_SelectedIndexChanged(object sender, EventArgs e)
        {
            nudWords.Enabled = !(cmboWords.SelectedIndex == 0);
        }

        private void cmboLinks_SelectedIndexChanged(object sender, EventArgs e)
        {
            nudLinks.Enabled = !(cmboLinks.SelectedIndex == 0);
        }

        private void chkHeading_CheckedChanged(object sender, EventArgs e)
        {
            nudHeadingSpace.Enabled = chkHeading.Checked;

            if (chkHeading.Checked)
                chkABCHeader.Checked = false;
        }

        private void chkABCHeader_CheckedChanged(object sender, EventArgs e)
        {
            if (chkABCHeader.Checked)
                chkHeading.Checked = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            wikifyToList();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            SpecialFilter.lb = lbArticles;
            SpecialFilter.ShowDialog(this);
            lblCount.Text = lbArticles.Items.Count.ToString();
        }

        private void AlphaList_Click(object sender, EventArgs e)
        {
            lbArticles.Sorted = true;
            lbArticles.Sorted = false;
        }

        private void lbClear_Click(object sender, EventArgs e)
        {
            lbArticles.Items.Clear();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(lbArticles.SelectedItem.ToString(), true);
        }

        private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tools.OpenArticleInBrowser(lbArticles.SelectedItem.ToString());
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbArticles.BeginUpdate();
            int i = lbArticles.SelectedIndex;

            while (lbArticles.SelectedItems.Count > 0)
                lbArticles.Items.Remove(lbArticles.SelectedItem);

            if (lbArticles.Items.Count > i)
                lbArticles.SelectedIndex = i;
            else
                lbArticles.SelectedIndex = i - 1;

            lbArticles.EndUpdate();

            lblCount.Text = lbArticles.Items.Count.ToString();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            copyToolStripMenuItem.Enabled = removeToolStripMenuItem.Enabled =
            openInBrowserToolStripMenuItem.Enabled = (lbArticles.SelectedIndex >= 0);
        }

        private static string convert(string text)
        {
            return text.Replace(@"\r\n", @"
");
        }

        private void txtTitleContains_Leave(object sender, EventArgs e)
        {
            txtTitleContains.Text = convert(txtTitleContains.Text);
        }

        private void txtTitleNotContains_Leave(object sender, EventArgs e)
        {
            txtTitleNotContains.Text = convert(txtTitleNotContains.Text);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "Start")
                StartButton();
            else
                StopButton();
        }

        private void StopButton()
        {
            if (Main != null)
            {
                Main.Stop();
                Main = null;
            }
            TimeSpan endTime = new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            endTime = endTime.Subtract(StartTime);
            MessageBox.Show(lbArticles.Items.Count.ToString() + " matches in " + endTime.ToString().TrimEnd('0'));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Main != null)
            {
                Main.Message = false;
                Main.Stop();
                Main = null;
            }
        }

        private void listComparerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListComparer lc = new ListComparer();
            lc.Show();
        }

        private void highestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboveNormalToolStripMenuItem.Checked = normalToolStripMenuItem.Checked =
            belowNormalToolStripMenuItem.Checked = lowestToolStripMenuItem.Checked = false;

            Priority = ThreadPriority.Highest;
        }

        private void aboveNormalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highestToolStripMenuItem.Checked = normalToolStripMenuItem.Checked =
            belowNormalToolStripMenuItem.Checked = lowestToolStripMenuItem.Checked = false;

            Priority = ThreadPriority.AboveNormal;
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highestToolStripMenuItem.Checked = normalToolStripMenuItem.Checked =
            belowNormalToolStripMenuItem.Checked = lowestToolStripMenuItem.Checked = false;

            Priority = ThreadPriority.Normal;
        }

        private void belowNormalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highestToolStripMenuItem.Checked = aboveNormalToolStripMenuItem.Checked =
            normalToolStripMenuItem.Checked = lowestToolStripMenuItem.Checked = false;

            Priority = ThreadPriority.BelowNormal;
        }

        private void lowestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highestToolStripMenuItem.Checked = aboveNormalToolStripMenuItem.Checked =
            normalToolStripMenuItem.Checked = belowNormalToolStripMenuItem.Checked = false;

            Priority = ThreadPriority.Lowest;
        }

        private void nudLimitResults_ValueChanged(object sender, EventArgs e)
        {
            intLimit = (int)nudLimitResults.Value;
        }

        #endregion

        #region properties

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            resetSettings();
        }

        private void resetSettings()
        {
            //menu
            ignoreRedirectsToolStripMenuItem1.Checked = true;
            ignoreCommentsToolStripMenuItem.Checked = false;
            ignoreImagesToolStripMenuItem.Checked = true;
            ignoreTemplateNamespaceToolStripMenuItem.Checked = true;
            ignoreWikipediaNamespaceToolStripMenuItem.Checked = true;
            ignoreCategoryNamespaceToolStripMenuItem.Checked = true;
            ignoreMainNamespaceToolStripMenuItem.Checked = false;

            //contains
            txtArticleDoesContain.Text = "";
            txtArticleDoesNotContain.Text = "";
            chkArticleDoesContain.Checked = false; ;
            chkArticleDoesNotContain.Checked = false;

            chkRegex.Checked = false;
            chkCaseSensitive.Checked = false;
            chkMulti.Checked = false;
            chkSingle.Checked = false;

            //title
            txtTitleContains.Text = "";
            txtTitleNotContains.Text = "";
            chkTitleContains.Checked = false;
            chkTitleDoesNotContain.Checked = false;
            chkTitleRegex.Checked = false;

            //characters and links
            cmboLength.SelectedIndex = 0;
            cmboLinks.SelectedIndex = 0;
            nudLength.Value = 1000;
            nudLinks.Value = 5;

            //extra
            rdoNone.Checked = true;

            //results
            chkHeading.Checked = false;
            nudHeadingSpace.Value = 25;
            nudLimitResults.Value = 30000;
            rdoBullet.Checked = false;
            rdoHash.Checked = true;

            fileName = ""; ;
        }

        private void UpdateControls(bool busy)
        {
            groupBox1.Enabled = groupBox2.Enabled = groupBox4.Enabled = groupBox5.Enabled = 
            btnFilter.Enabled = nudLimitResults.Enabled = txtStartFrom.Enabled = !busy;
            if (busy) { btnStart.Text = "Stop"; } else { btnStart.Text = "Start"; }
        }
        #endregion

        private void timerProgessUpdate_Tick(object sender, EventArgs e)
        {
            updateProgressBar();
        }

        private void updateProgressBar()
        {
            // update progress bar
            double matchesByLimit = (double)intMatches / intLimit;
            if (matchesByLimit > (double)Main.stream.Position / Main.stream.Length)
                progressBar1.Value = (int)(matchesByLimit * (Main.stream.Length / 1024));
            else
                progressBar1.Value = (int)(Main.stream.Position / 1024);
        }
    }
}