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
using WikiFunctions.Background;

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
        ListMaker listMaker;

        ListFilterForm SpecialFilter;

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
            SpecialFilter = new ListFilterForm(lbArticles);

#if !DEBUG
            tbParameters.TabPages.Remove(tabRestrict);
#endif
        }

        public DatabaseScanner(ListMaker lm)
            :this()
        {
            listMaker = lm;
            if (lm != null)
                AWBListbox = lm.Items;
        }

        private void DatabaseScanner_Load(object sender, EventArgs e)
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
                    MessageBox.Show("Please open an \"Pages\" XML data-dump file\r\n\r\nSee the About menu for where to download this file.", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                lbArticles.Items.Clear();
                lblCount.Text = "";

                intMatches = 0;

                UpdateControls(true);

                txtStartFrom.Text = Tools.TurnFirstToUpper(txtStartFrom.Text);
                Start();

                timerProgessUpdate.Enabled = true;
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
        string ArticleContains;
        string ArticleDoesNotContain;
        List<int> namespaces = new List<int>();
        bool ArticleCaseSensitive;

        CrossThreadQueue<string> Queue = new CrossThreadQueue<string>();
        
        private void MakePatterns()
        {
            string strTitleNot = "";
            string strTitle = "";
            RegexOptions titleRegOptions;

            RegexOptions articleRegOptions;

            strTitle = convert(txtTitleContains.Text);
            strTitleNot = convert(txtTitleNotContains.Text);
            ArticleContains = convert(txtArticleDoesContain.Text);
            ArticleDoesNotContain = convert(txtArticleDoesNotContain.Text);

            articleRegOptions = RegexOptions.Compiled;
            titleRegOptions = RegexOptions.Compiled;

            if (!chkCaseSensitive.Checked)
                articleRegOptions |= RegexOptions.IgnoreCase;
            if (chkMulti.Checked)
                articleRegOptions |= RegexOptions.Multiline;
            if (chkSingle.Checked)
                articleRegOptions |= RegexOptions.Singleline;

            ArticleCaseSensitive = chkCaseSensitive.Checked;

            if (chkRegex.Checked)
            {
                ArticleDoesRegex = new Regex(ArticleContains, articleRegOptions);
                ArticleDoesNotRegex = new Regex(ArticleDoesNotContain, articleRegOptions);
                ArticleContains = null;
                ArticleDoesNotContain = null;
            }

            if (!chkTitleRegex.Checked)
            {
                strTitle = Regex.Escape(strTitle);
                strTitleNot = Regex.Escape(strTitleNot);
            }

            if (!chkTitleCase.Checked)
                titleRegOptions = titleRegOptions | RegexOptions.IgnoreCase;

            TitleDoesRegex = new Regex(strTitle, titleRegOptions);
            TitleDoesNotRegex = new Regex(strTitleNot, titleRegOptions);

            namespaces.Clear();

            namespaces.Add(8);
            namespaces.Add(100);

            if (!chkCategoryNamespace.Checked)
                namespaces.Add(14);

            if (!chkImageNamespace.Checked)
                namespaces.Add(6);

            if (!chkTemplateNamespace.Checked)
                namespaces.Add(10);

            if (!chkProjectNamespace.Checked)
                namespaces.Add(4);

            if (!chkMainNamespace.Checked)
                namespaces.Add(0);
        }

        private Dictionary<string, bool> MakeReplacementDictionary(string rule, bool caseSensitive)
        {
            Dictionary<string, bool> dict = new Dictionary<string, bool>(1);
            dict.Add(rule, caseSensitive);

            return dict;
        }

        private void Start()
        {
            MakePatterns();

            StartTime = new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            intLimit = (int)nudLimitResults.Value;

            List<Scan> s = new List<Scan>();

            s.Add(new CheckNamespace(namespaces));

            if (chkIgnoreRedirects.Checked)
                s.Add(new IsNotRedirect());

            if (chkArticleDoesContain.Checked)
            {
                if (ArticleContains != null) // simple search
                    s.Add(new TextContains(MakeReplacementDictionary(ArticleContains, ArticleCaseSensitive)));
                else // regex
                    s.Add(new TextContainsRegex(ArticleDoesRegex));
            }

            if (chkArticleDoesNotContain.Checked)
            {
                if (ArticleDoesNotContain != null)
                    s.Add(new TextDoesNotContain(MakeReplacementDictionary(ArticleDoesNotContain, ArticleCaseSensitive)));
                else
                    s.Add(new TextDoesNotContainRegex(ArticleDoesNotRegex));
            }

            if (chkTitleContains.Checked)
                s.Add(new TitleContains(TitleDoesRegex));

            if (chkTitleDoesNotContain.Checked)
                s.Add(new TitleDoesNotContain(TitleDoesNotRegex));

            if (chkSearchDates.Checked)
                s.Add(new DateRange(dtpFrom.Value, dtpTo.Value));

            if (chkProtection.Checked)
                s.Add(new Restriction(MoveDelete.EditProtectionLevel, MoveDelete.MoveProtectionLevel));

            if (cmboLength.SelectedIndex == 1)
                s.Add(new CountCharacters(MoreLessThan.MoreThan, (int)nudLength.Value));
            else if (cmboLength.SelectedIndex == 2)
                s.Add(new CountCharacters(MoreLessThan.LessThan, (int)nudLength.Value));

            if (cmboLinks.SelectedIndex == 1)
                s.Add(new CountLinks(MoreLessThan.MoreThan, (int)nudLinks.Value));
            else if (cmboLinks.SelectedIndex == 2)
                s.Add(new CountLinks(MoreLessThan.LessThan, (int)nudLinks.Value));

            if (cmboWords.SelectedIndex == 1)
                s.Add(new CountWords(MoreLessThan.MoreThan, (int)nudWords.Value));
            else if (cmboWords.SelectedIndex == 2)
                s.Add(new CountWords(MoreLessThan.LessThan, (int)nudWords.Value));

            Parsers parsers = new Parsers();

            if (chkBadLinks.Checked)
                s.Add(new HasBadLinks());
            if (chkNoBold.Checked)
                s.Add(new HasNoBoldTitle(parsers));
            if (chkSimpleLinks.Checked)
                s.Add(new HasSimpleLinks());
            if (chkHasHTML.Checked)
                s.Add(new HasHTMLEntities(parsers));
            if (chkHeaderError.Checked)
                s.Add(new HasSectionError(parsers));
            if (chkUnbulletedLinks.Checked)
                s.Add(new HasUnbulletedLinks());
            if (chkTypo.Checked)
                s.Add(new Typo());
            if (chkDefaultSort.Checked)
                s.Add(new MissingDefaultsort(parsers));

            Main = new MainProcess(s, fileName, Priority, chkIgnoreComments.Checked, txtStartFrom.Text);
            progressBar.Value = 0;
            Main.StoppedEvent += Stopped;
            Main.OutputQueue = Queue;
            Main.Start();
        }

        private void AddArticle(string article)
        {
            if (string.IsNullOrEmpty(article))
                return;

            Article a = new Article(article);
            lbArticles.Items.Add(a);

            if (AWBListbox != null)
                AWBListbox.Items.Add(a);

            intMatches++;

            if (intMatches >= intLimit)
                Main.Run = false;
        }

        private void Stopped()
        {
            try
            {
                if (Main != null)
                {
                    Main.StoppedEvent -= Stopped;
                }

                progressBar.Value = 0;

                timerProgessUpdate.Enabled = false;

                UpdateList();

                UpdateDBScannerArticleCount();

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

        private void UpdateListMakerCount()
        {
            if (listMaker != null) listMaker.UpdateNumberOfArticles();
        }

        private void RemoveDBScannerListItemsFromAWBListbox()
        {
            if (AWBListbox != null)
            {
                foreach (Article a in lbArticles)
                    AWBListbox.Items.Remove(a);
            }
        }

        #endregion

        # region other

        private void wikifyToList()
        {
            StringBuilder strbList = new StringBuilder("");
            string s = "", l = "", sr = "", strBullet;
            int intHeadingSpace = Convert.ToInt32(nudHeadingSpace.Value);

            if (rdoHash.Checked)
                strBullet = "#";
            else
                strBullet = "*";

            if (chkHeading.Checked)
            {
                int intSection = 0, intSectionNumber = 0;

                strbList.AppendLine("==0==");
                intSectionNumber++;

                foreach(Article a in lbArticles.Items)
                {
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
                }
            }
            else if (chkABCHeader.Checked)
            {
                foreach (Article a in lbArticles.Items)
                {
                    s = a.ToString().Replace("&amp;", "&");

                    if (s.Length > 1)
                        sr = s.Remove(1);
                    else
                        sr = s;

                    if (sr != l)
                        strbList.AppendLine("\r\n== " + sr + " ==");

                    strbList.AppendLine(strBullet + " [[" + s + "]]");

                    l = sr;
                }
            }
            else
            {
                foreach (Article a in lbArticles.Items)
                {
                    strbList.AppendLine(strBullet + " [[" + a.ToString().Replace("&amp;", "&") + "]]");
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
                    this.Text = "Wiki Database Scanner";
            }
        }

        private void SaveConvertedList()
        {
            try
            {
                string strList = txtList.Text;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8))
                    {
                        sw.Write(strList);
                        sw.Close();
                    }
                }
            }
            catch (Exception ex) { ErrorHandler.Handle(ex); }
        }

        private void btnSaveTxtList_Click(object sender, EventArgs e)
        {
            SaveConvertedList();
        }

        private void chkRegex_CheckedChanged(object sender, EventArgs e)
        {
            bool regex = chkRegex.Checked;
            chkMulti.Enabled = regex;
            chkSingle.Enabled = regex;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
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
            nudLength.Enabled = (cmboLength.SelectedIndex != 0);
        }

        private void cmboWords_SelectedIndexChanged(object sender, EventArgs e)
        {
            nudWords.Enabled = (cmboWords.SelectedIndex != 0);
        }

        private void cmboLinks_SelectedIndexChanged(object sender, EventArgs e)
        {
            nudLinks.Enabled = (cmboLinks.SelectedIndex != 0);
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

        private void btnTransfer_Click(object sender, EventArgs e)
        {
            wikifyToList();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (AWBListbox != null)
            {
                AWBListbox.BeginUpdate();
                RemoveDBScannerListItemsFromAWBListbox();
            }

            SpecialFilter.ShowDialog(this);

            if (AWBListbox != null)
            {
                AWBListbox.Items.AddRange(lbArticles.Items);
                AWBListbox.EndUpdate();
            }

            UpdateDBScannerArticleCount();
            UpdateListMakerCount();
        }

        private void lbClear_Click(object sender, EventArgs e)
        {
            lblCount.Text = "";
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

            if (AWBListbox != null)
            {
                AWBListbox.BeginUpdate();

                foreach(Article a in lbArticles.SelectedItems)
                    AWBListbox.Items.Remove(a);

                AWBListbox.EndUpdate();
            }

            int i = lbArticles.SelectedIndex;

            while (lbArticles.SelectedItems.Count > 0)
                lbArticles.Items.Remove(lbArticles.SelectedItem);

            if (lbArticles.Items.Count > i)
                lbArticles.SelectedIndex = i;
            else
                lbArticles.SelectedIndex = i - 1;

            lbArticles.EndUpdate();

            UpdateDBScannerArticleCount();
            UpdateListMakerCount();
        }

        private void UpdateDBScannerArticleCount()
        {
            lblCount.Text = lbArticles.Items.Count.ToString() + " results";
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            copyToolStripMenuItem.Enabled = removeToolStripMenuItem.Enabled =
            openInBrowserToolStripMenuItem.Enabled = (lbArticles.SelectedIndex >= 0);
        }

        private static string convert(string text)
        {
            return text.Replace("\r\n", "\n");
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
            //btnPause.Enabled = (btnStart.Text == "Start");

            if (btnStart.Text == "Start")
                StartButton();
            else
                StopButton();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
        }

        private void StopButton()
        {
            timerProgessUpdate.Enabled = false;
            if (Main != null)
            {
                Main.Stop();
                Main = null;
            }
            TimeSpan endTime = new TimeSpan(DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            endTime = endTime.Subtract(StartTime);
            MessageBox.Show(lbArticles.Items.Count.ToString() + " matches in " + endTime.ToString().TrimEnd('0'));
        }

        private void DatabaseScanner_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to close the Database Scanner?", "Close Scanner?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
            else if (Main != null)
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
            threadPriorityButton.Text = highestToolStripMenuItem.Text;
        }
        
        private void aboveNormalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highestToolStripMenuItem.Checked = normalToolStripMenuItem.Checked =
            belowNormalToolStripMenuItem.Checked = lowestToolStripMenuItem.Checked = false;

            Priority = ThreadPriority.AboveNormal;
            threadPriorityButton.Text = aboveNormalToolStripMenuItem.Text;
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highestToolStripMenuItem.Checked = normalToolStripMenuItem.Checked =
            belowNormalToolStripMenuItem.Checked = lowestToolStripMenuItem.Checked = false;

            Priority = ThreadPriority.Normal;
            threadPriorityButton.Text = normalToolStripMenuItem.Text;
        }

        private void belowNormalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highestToolStripMenuItem.Checked = aboveNormalToolStripMenuItem.Checked =
            normalToolStripMenuItem.Checked = lowestToolStripMenuItem.Checked = false;

            Priority = ThreadPriority.BelowNormal;
            threadPriorityButton.Text = belowNormalToolStripMenuItem.Text;
        }

        private void lowestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highestToolStripMenuItem.Checked = aboveNormalToolStripMenuItem.Checked =
            normalToolStripMenuItem.Checked = belowNormalToolStripMenuItem.Checked = false;

            Priority = ThreadPriority.Lowest;
            threadPriorityButton.Text = lowestToolStripMenuItem.Text;
        }

        private void nudLimitResults_ValueChanged(object sender, EventArgs e)
        {
            intLimit = (int)nudLimitResults.Value;
        }

        #endregion

        #region properties

        private void btnReset_Click(object sender, EventArgs e)
        {
            resetSettings();
        }

        private void resetSettings()
        {
            //menu
            chkIgnoreRedirects.Checked = true;
            chkIgnoreComments.Checked = false;
            chkImageNamespace.Checked = false;
            chkTemplateNamespace.Checked = false;
            chkProjectNamespace.Checked = false;
            chkCategoryNamespace.Checked = false;
            chkMainNamespace.Checked = true;

            //contains
            txtArticleDoesContain.Text = "";
            txtArticleDoesNotContain.Text = "";
            chkArticleDoesContain.Checked = false;
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

            //Restriction
            chkProtection.Checked = false;
            MoveDelete.Reset();

            //extra
            foreach (CheckBox c in flwAWB.Controls)
                c.Checked = false;

            chkSearchDates.Checked = false;

            //results
            chkHeading.Checked = false;
            nudHeadingSpace.Value = 25;
            nudLimitResults.Value = 30000;
            rdoBullet.Checked = false;
            rdoHash.Checked = true;

            fileName = "";
        }

        private void UpdateControls(bool busy)
        {
            gbText.Enabled = gbTitle.Enabled = groupBox4.Enabled = gbAWBSpecific.Enabled = gbNamespace.Enabled =
                gbDate.Enabled = gbProperties.Enabled = btnFilter.Enabled = nudLimitResults.Enabled = txtStartFrom.Enabled = 
                btnReset.Enabled = btnBrowse.Enabled = !busy;
            if (busy) { btnStart.Text = "Stop"; } else { btnStart.Text = "Start"; }
        }
        #endregion

        private void timerProgessUpdate_Tick(object sender, EventArgs e)
        {
            UpdateProgressBar();
            UpdateList();
        }

        private void UpdateList()
        {
            if (Queue.Count == 0) return;
            bool locked = false;

            if (Queue.Count > 1)
            {
                locked = true;
                lbArticles.BeginUpdate();
                if (AWBListbox != null)
                    AWBListbox.BeginUpdate();
            }

            while (Queue.Count > 0)
            {
                AddArticle(Queue.Remove());
            }

            if (locked)
            {
                lbArticles.EndUpdate();
                if (AWBListbox != null)
                    AWBListbox.EndUpdate();
            }

            lblCount.Text = intMatches.ToString();
            UpdateListMakerCount();
        }

        private void UpdateProgressBar()
        {
            double matchesByLimit = (double)intMatches / intLimit;
            double completion = Main.PercentageComplete;

            if (matchesByLimit > completion)
                progressBar.Value = (int)(matchesByLimit * progressBar.Maximum);
            else
                progressBar.Value = (int)(completion * progressBar.Maximum);
        }

        private void btnOpen(object sender, EventArgs e)
        {
            try
            {
                if (openXMLDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = openXMLDialog.FileName;
                    txtDumpLocation.Text = fileName;

                    int dataFound = 0;

                    using (XmlTextReader reader = new XmlTextReader(new StreamReader(fileName)))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name.Equals("sitename"))
                            {
                                txtSitename.Text = reader.ReadString();
                                dataFound++;
                            }
                            else if (reader.Name.Equals("base"))
                            {
                                lnkBase.Text = reader.ReadString();
                                dataFound++;
                            }
                            else if (reader.Name.Equals("generator"))
                            {
                                txtGenerator.Text = reader.ReadString();
                                dataFound++;
                            }
                            else if (reader.Name.Equals("case"))
                            {
                                txtCase.Text = reader.ReadString();
                                dataFound++;
                            }

                            if (dataFound == 4)
                                break;
                        }
                    }
                }
            }
            catch (Exception ex) { ErrorHandler.Handle(ex); }
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            Tools.About();
        }

        private void lnkGenDump_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenURLInBrowser("http://www.mediawiki.org/wiki/Manual:DumpBackup.php");
        }

        private void lnkBase_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenURLInBrowser(lnkBase.Text);
        }

        private void lnkWikiaDumps_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenURLInBrowser("http://www.wikia.com/wiki/Database_download");
        }

        private void lnkWmfDumps_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Tools.OpenURLInBrowser("http://download.wikimedia.org/");
        }

        private void chkSearchDates_CheckedChanged(object sender, EventArgs e)
        {
            dtpFrom.Enabled = dtpTo.Enabled = chkSearchDates.Checked;
        }

        private void btnSaveArticleList_Click(object sender, EventArgs e)
        {
            ListMaker.SaveList(lbArticles);
        }

        private void chkProtection_CheckedChanged(object sender, EventArgs e)
        {
            MoveDelete.Enabled = chkProtection.Checked;
        }
    }
}
