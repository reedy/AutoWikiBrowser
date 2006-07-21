/*
DumpSearcher
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
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.Threading;

namespace WikiFunctions
{
    /// <summary>
    /// Provides a form and functions for searching XML data dumps
    /// </summary>
    public partial class DumpSearcher : Form
    {
        public delegate void Kick(string article);
        public event Kick foundarticle;

        Parsers parsers = new Parsers();
        bool skip = true;

        public DumpSearcher()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmboLength.SelectedIndex = 0;
            cmboLinks.SelectedIndex = 0;
            loadSettings();
        }

        #region main process

        int intMatches = 0;
        int intLimit = 0;
        int intTimer = 0;
        Thread PThread = null;

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                if (PThread != null)
                    return;

                boolRun = true;

                if (fileName.Length == 0)
                {
                    MessageBox.Show("Please open an \"Articles\" XML data-dump file from the file menu\r\n\r\nSee the About menu for where to download this file.", "Problem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    stream = new FileStream(fileName, FileMode.Open);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    return;
                }

                groupBox4.Enabled = false;
                btnStart.Text = "Working";
                btnStart.Enabled = false;
                lbArticles.Items.Clear();
                lblCount.Text = "";

                intMatches = 0;
                intTimer = 0;
                intLimit = Convert.ToInt32(nudLimitResults.Value);

                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnimationSpeed = 100;

                timer1.Enabled = true;
                
                ThreadStart thr_Process = new ThreadStart(Process);
                PThread = new Thread(thr_Process);
                PThread.IsBackground = true;
                PThread.Name = "pt";
                PThread.Start();
            }
            catch { }
        }

        bool boolRun = true;
        bool boolMessage = true;

        string articleTitle = "";
        string articleText = "";

        string strTitleNot = "";
        string strTitle = "";

        Regex PRegex;
        Regex PNRegex;

        RegexOptions ArticleRegOptions;
        private void makePatterns()
        {
            strTitle = convert(txtTitleContains.Text);
            strTitleNot = convert(txtTitleNotContains.Text);

            ArticleRegOptions = RegexOptions.ExplicitCapture;

            string pattern = convert(txtPattern.Text);
            string patternNot = convert(txtPatternNot.Text);


            if (!chkRegex.Checked)
            {
                pattern = Regex.Escape(pattern);
                patternNot = Regex.Escape(patternNot);
            }

            if (!chkCaseSensitive.Checked)
                ArticleRegOptions = ArticleRegOptions | RegexOptions.IgnoreCase;

            if (chkMulti.Checked)
                ArticleRegOptions = ArticleRegOptions | RegexOptions.Multiline;

            if (chkSingle.Checked)
                ArticleRegOptions = ArticleRegOptions | RegexOptions.Singleline;

            PRegex = new Regex(pattern, ArticleRegOptions);
            PNRegex = new Regex(patternNot, ArticleRegOptions);
        }

        Stream stream;
        private void Process()
        {
            makePatterns();
            Regex regComments = new Regex("&lt;!--.*?--&gt;", RegexOptions.Singleline);
            try
            {
                using (XmlTextReader reader = new XmlTextReader(stream))
                {
                    reader.WhitespaceHandling = WhitespaceHandling.None;
                    while (reader.Read() && boolRun)
                    {
                        if (reader.LocalName.Equals("page"))
                        {
                            reader.ReadToFollowing("title");
                            articleTitle = reader.ReadInnerXml();
                            reader.ReadToFollowing("text");
                            articleText = reader.ReadInnerXml();

                            if (ignoreCommentsToolStripMenuItem.Checked)
                                articleText = regComments.Replace(articleText, "");

                            if (IgnoreName() && countLinks() && checkLength() && checkDoesContain() && checkDoesNotContain() && simpleLinks() && boldTitle() && badLinks() && containsHTML() && sectionHeaderError())// && noBirthCat(articleText)
                            {
                                lbArticles.Items.Add(articleTitle);

                                this.foundarticle(articleTitle);

                                intMatches++;
                                lblCount.Text = intMatches.ToString();

                                if (intMatches >= intLimit)
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (boolMessage)
                    MessageBox.Show("Problem on " + articleTitle + "\r\n\r\n" + ex.Message.ToString());
            }
            finally
            {
                StopProgressBar();
            }
        }

        private delegate void SetProgBarDelegate();
        private void StopProgressBar()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new SetProgBarDelegate(StopProgressBar));
                    return;
                }

                if (PThread != null)
                    PThread = null;

                progressBar1.MarqueeAnimationSpeed = 0;
                progressBar1.Style = ProgressBarStyle.Continuous;

                timer1.Enabled = false;
                groupBox4.Enabled = true;
                btnStart.Enabled = true;
                btnStart.Text = "Start";
                lblCount.Text = lbArticles.Items.Count.ToString() + " results";
                if (boolMessage)
                    MessageBox.Show(lbArticles.Items.Count.ToString() + " matches in " + intTimer.ToString() + " seconds");
            }
            catch (Exception ex)
            {
                if (boolMessage)
                    MessageBox.Show(ex.Message.ToString());
            }
        }

        #endregion

        #region checkers
        private bool checkLength()
        {
            if (cmboLength.SelectedIndex == 0)
                return true;
            else if (cmboLength.SelectedIndex == 1 && articleText.Length > numLength.Value)
                return true;
            else if (cmboLength.SelectedIndex == 2 && articleText.Length < numLength.Value)
                return true;
            else
                return false;
        }

        private bool countLinks()
        {
            if (cmboLinks.SelectedIndex == 0)
                return true;
            else
            {
                int intLinks = 0;
                foreach (Match m in Regex.Matches(articleText, "\\[\\[")) intLinks++;

                if (cmboLinks.SelectedIndex == 1 && intLinks > nudLinks.Value)
                    return true;
                else if (cmboLinks.SelectedIndex == 2 && intLinks < nudLinks.Value)
                    return true;
                else
                    return false;
            }
        }

        private bool checkDoesContain()
        {
            if (!chkDoesContain.Checked)
                return true;

            if (PRegex.IsMatch(articleText))
                return true;
            else
                return false;
        }

        private bool checkDoesNotContain()
        {
            if (!chkDoesNotContain.Checked)
                return true;

            if (PNRegex.IsMatch(articleText))
                return false;
            else
                return true;
        }

        private bool IgnoreName()
        {
            if (ignoreRedirectsToolStripMenuItem1.Checked && articleText.StartsWith("#"))
                return false;
            else if (articleTitle.StartsWith(Variables.Namespaces[8]) || articleTitle.StartsWith(Variables.Namespaces[100])) //skip this namespace
                return false;
            else if (ignoreDisambigsToolStripMenuItem.Checked && articleText.Contains("isambig}}"))
                return false;
            else if (ignoreImagesToolStripMenuItem.Checked && articleTitle.StartsWith(Variables.Namespaces[6]))
                return false;
            else if (articleTitle.StartsWith(Variables.Namespaces[12]))
                return false;
            else if (ignoreCategoryNamespaceToolStripMenuItem.Checked && articleTitle.StartsWith(Variables.Namespaces[14]))
                return false;
            else if (ignoreTemplateNamespaceToolStripMenuItem.Checked && articleTitle.StartsWith(Variables.Namespaces[10]))
                return false;
            else if (ignoreWikipediaNamespaceToolStripMenuItem.Checked && articleTitle.StartsWith(Variables.Namespaces[4]))
                return false;
            else if (ignoreMainNamespaceToolStripMenuItem.Checked && Tools.IsMainSpace(articleTitle))
                return false;
            else
            {
                if (TitleContains() && TitleNotContains())
                    return true;
                else
                    return false;
            }
        }

        private bool TitleContains()
        {
            if (!chkCheckTitle.Checked)
                return true;

            else if (chkTitleRegex.Checked)
            {
                if (Regex.IsMatch(articleTitle, strTitle))
                    return true;
                else
                    return false;
            }
            else
            {
                if (articleTitle.Contains(strTitle))
                    return true;
                else
                    return false;
            }
        }

        private bool TitleNotContains()
        {
            if (!chkCheckNotInTitle.Checked)
                return true;

            else if (chkTitleRegex.Checked)
            {
                if (!Regex.IsMatch(articleTitle, strTitleNot))
                    return true;
                else
                    return false;
            }
            else
            {
                if (!articleTitle.Contains(strTitleNot))
                    return true;
                else
                    return false;
            }
        }

        #endregion

        #region custom checkers

        private bool simpleLinks()
        {
            if (!chkSimpleLinks.Checked)
                return true;
            string n = "";
            string a = "";
            string b = "";

            foreach (Match m in Regex.Matches(articleText, "\\[\\[(.*?)\\|(.*?)\\]\\]"))
            {
                n = m.ToString();
                a = m.Groups[1].Value;
                b = m.Groups[2].Value;

                if (a == b || TurnFirstToLower(a) == b)
                {
                    return true;
                }
                else if (a + "s" == b || TurnFirstToLower(a) + "s" == b)
                {
                    return true;
                }
            }

            return false;
        }

        private string TurnFirstToLower(string input)
        {
            //turns first character to lowercase
            input = input.Trim();

            if (input.Length > 0)
            {
                string temp = input.Substring(0, 1);
                return temp.ToLower() + input.Remove(0, 1);
            }
            else
                return input;
        }

        private bool noBirthCat()
        {
            if (Regex.IsMatch(articleText, "\\[\\[[Cc]ategory: ?[0-9]{3,4} births"))
                return false;

            string articleTextTemp = articleText;

            if (articleTextTemp.Length > 80)
                articleTextTemp = articleTextTemp.Substring(0, 80);

            if (Regex.IsMatch(articleTextTemp, "(\\(| )[Bb]orn "))
                return true;

            return false;
        }


        private bool boldTitle()
        {
            if (!chkNoBold.Checked)
                return true;

            parsers.BoldTitle(articleText, articleTitle, ref skip);

            return !skip;
        }

        private bool badLinks()
        {
            if (!chkBadLinks.Checked)
                return true;

            parsers.LinkFixer(articleText, ref skip);

            return !skip;
        }
        
        private bool containsHTML()
        {
            if (!chkHTML.Checked)
                return true;

            parsers.Unicodify(articleText, ref skip);

            return !skip;
        }

        private bool sectionHeaderError()
        {
            if (!chkSectionError.Checked)
                return true;

            if (!Regex.IsMatch(articleText, "= ?See also ?=") && Regex.IsMatch(articleText, "(== ?)([Ss]ee Also:?|[rR]elated [tT]opics:?|[rR]elated [aA]rticles:?|[Ii]nternal [lL]inks:?|[Aa]lso [Ss]ee:?)( ?==)"))
                return true;

            parsers.FixHeadings(articleText, ref skip);

            return !skip;
        }

        private bool BulletExternal()
        {
            parsers.BulletExternalLinks(articleText, ref skip);

            return !skip;
        }

        #endregion

        # region other

        private void timer1_Tick(object sender, EventArgs e)
        {
            intTimer++;
        }

        private void wikifyToList()
        {
            bool b = true;

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
                strbList.Append("==0==\r\n");
                intSectionNumber++;
            }

            if (b)
            {
                while (i < lbArticles.Items.Count)
                {
                    s = lbArticles.Items[i].ToString();

                    s = s.Replace("&amp;", "&");

                    strbList.Append(strBullet + " [[" + s + "]]\r\n");

                    intSection++;
                    if (chkHeading.Checked && intSection == intHeadingSpace)
                    {
                        strbList.Append("\r\n==" + intSectionNumber + "==\r\n");
                        intSectionNumber++;
                        intSection = 0;
                    }

                    i++;
                }
            }
            else
            {
                while (i < lbArticles.Items.Count)
                {
                    s = lbArticles.Items[i].ToString();

                    s = s.Replace("&amp;", "&");

                    if (s.Length > 1)
                    {
                        sr = s.Remove(1);
                    }
                    else
                        sr = s;

                    if (sr != l)
                        strbList.Append("\r\n== " + sr + " ==\r\n");

                    strbList.Append(strBullet + " [[" + s + "]]\r\n");

                    l = sr;
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
                    this.Text = "Wiki Data Dump Searcher - " + shortened;
                }
                else
                    this.Text = "Wiki Data Dump Searcher";
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (openXMLDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = openXMLDialog.FileName;
                }
            }
            catch { }
        }

        private void Save()
        {
            try
            {
                string strList = txtList.Text;

                if (saveFileDialog2.ShowDialog() == DialogResult.OK)
                {
                    StreamWriter sw = new StreamWriter(saveFileDialog2.FileName);
                    sw.Write(strList);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
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
            try
            {
                AboutBox About = new AboutBox();
                About.Show();
            }
            catch { }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                if (PThread != null && PThread.IsAlive)
                    PThread.Abort();
            }
            catch { }
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
            txtPattern.Enabled = chkDoesContain.Checked;
        }

        private void chkDoesNotContain_CheckedChanged(object sender, EventArgs e)
        {
            txtPatternNot.Enabled = chkDoesNotContain.Checked;
        }

        private void chkCheckTitle_CheckedChanged(object sender, EventArgs e)
        {
            txtTitleContains.Enabled = chkCheckTitle.Checked;
        }

        private void chkCheckNotInTitle_CheckedChanged(object sender, EventArgs e)
        {
            txtTitleNotContains.Enabled = chkCheckNotInTitle.Checked;
        }

        private void cmboLength_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmboLength.SelectedIndex == 0)
                numLength.Enabled = false;
            else
                numLength.Enabled = true;

        }

        private void cmboLinks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmboLinks.SelectedIndex == 0)
                nudLinks.Enabled = false;
            else
                nudLinks.Enabled = true;
        }

        private void chkHeading_CheckedChanged(object sender, EventArgs e)
        {
            nudHeadingSpace.Enabled = chkHeading.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            wikifyToList();
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

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lbArticles.Items.RemoveAt(lbArticles.SelectedIndex);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (lbArticles.SelectedIndex >= 0)
            {
                copyToolStripMenuItem.Enabled = true;
                removeToolStripMenuItem.Enabled = true;
            }
            else
            {
                copyToolStripMenuItem.Enabled = false;
                removeToolStripMenuItem.Enabled = false;
            }
        }

        private string convert(string text)
        {
            text = text.Replace("&", "&amp;");
            text = text.Replace("<", "&lt;");
            text = text.Replace(">", "&gt;");

            return text;
        }

        private void txtTitleContains_Leave(object sender, EventArgs e)
        {
            txtTitleContains.Text = convert(txtTitleContains.Text);
        }

        private void txtTitleNotContains_Leave(object sender, EventArgs e)
        {
            txtTitleNotContains.Text = convert(txtTitleNotContains.Text);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            boolRun = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            boolMessage = false;
            boolRun = false;

            saveSettings();
        }

        private void listComparerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WikiFunctions.ListComparer LC = new WikiFunctions.ListComparer();
            LC.Show();
        }

        private void highestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboveNormalToolStripMenuItem.Checked = false;
            normalToolStripMenuItem.Checked = false;
            belowNormalToolStripMenuItem.Checked = false;
            lowestToolStripMenuItem.Checked = false;

            if (PThread == null)
                return;

            PThread.Priority = ThreadPriority.Highest;
        }

        private void aboveNormalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highestToolStripMenuItem.Checked = false;
            normalToolStripMenuItem.Checked = false;
            belowNormalToolStripMenuItem.Checked = false;
            lowestToolStripMenuItem.Checked = false;

            if (PThread == null)
                return;

            PThread.Priority = ThreadPriority.AboveNormal;
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highestToolStripMenuItem.Checked = false;
            aboveNormalToolStripMenuItem.Checked = false;
            belowNormalToolStripMenuItem.Checked = false;
            lowestToolStripMenuItem.Checked = false;

            if (PThread == null)
                return;

            PThread.Priority = ThreadPriority.Normal;
        }

        private void belowNormalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highestToolStripMenuItem.Checked = false;
            aboveNormalToolStripMenuItem.Checked = false;
            normalToolStripMenuItem.Checked = false;
            lowestToolStripMenuItem.Checked = false;

            if (PThread == null)
                return;

            PThread.Priority = ThreadPriority.BelowNormal;
        }

        private void lowestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            highestToolStripMenuItem.Checked = false;
            aboveNormalToolStripMenuItem.Checked = false;
            normalToolStripMenuItem.Checked = false;
            belowNormalToolStripMenuItem.Checked = false;

            if (PThread == null)
                return;

            PThread.Priority = ThreadPriority.Lowest;
        }
        #endregion

        #region properties

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            resetSettings();
        }

        private void saveSettings()
        {
            //menu
            //WikiDumpSearcher.Properties.Settings.Default.IgnoreRedirects = ignoreRedirectsToolStripMenuItem1.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.IgnoreComments = ignoreCommentsToolStripMenuItem.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.IgnoreDisambigs = ignoreDisambigsToolStripMenuItem.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.IgnoreImage = ignoreImagesToolStripMenuItem.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.IgnoreTemplate = ignoreTemplateNamespaceToolStripMenuItem.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.IgnoreWikipedia = ignoreWikipediaNamespaceToolStripMenuItem.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.IngoreCategory = ignoreCategoryNamespaceToolStripMenuItem.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.IgnoreMain = ignoreMainNamespaceToolStripMenuItem.Checked;

            //WikiDumpSearcher.Properties.Settings.Default.ThreadPriority = Threadpriority;

            ////contains
            //WikiDumpSearcher.Properties.Settings.Default.DoesContain = txtPattern.Text;
            //WikiDumpSearcher.Properties.Settings.Default.DoesNotContain = txtPatternNot.Text;
            //WikiDumpSearcher.Properties.Settings.Default.DoesContainEnabled = chkDoesContain.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.DoesNotContainEnabled = chkDoesNotContain.Checked;

            //WikiDumpSearcher.Properties.Settings.Default.IsRegex = chkRegex.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.IsCaseSens = chkCaseSensitive.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.IsMulti = chkMulti.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.IsSingle = chkSingle.Checked;

            ////title
            //WikiDumpSearcher.Properties.Settings.Default.TitleContains = txtTitleContains.Text;
            //WikiDumpSearcher.Properties.Settings.Default.TitleDoesNotContain = txtTitleNotContains.Text;
            //WikiDumpSearcher.Properties.Settings.Default.TitleContainsEnabled = chkCheckTitle.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.TitleNotContainsEnabled = chkCheckNotInTitle.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.TitleIsRegex = chkTitleRegex.Checked;

            ////characters and links
            //WikiDumpSearcher.Properties.Settings.Default.CountCharatersIndex = cmboLength.SelectedIndex;
            //WikiDumpSearcher.Properties.Settings.Default.CountLinksIndex = cmboLinks.SelectedIndex;
            //WikiDumpSearcher.Properties.Settings.Default.Characters = numLength.Value;
            //WikiDumpSearcher.Properties.Settings.Default.Links = nudLinks.Value;

            ////extra
            //WikiDumpSearcher.Properties.Settings.Default.BoldTitle = chkNoBold.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.BadLinks = chkBadLinks.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.HTMLEntities = chkHTML.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.SimpleLinks = chkSimpleLinks.Checked;

            ////results
            //WikiDumpSearcher.Properties.Settings.Default.AddHeading = chkHeading.Checked;
            //WikiDumpSearcher.Properties.Settings.Default.HeadingGap = nudHeadingSpace.Value;
            //WikiDumpSearcher.Properties.Settings.Default.ResultsLimit = nudLimitResults.Value;
            //WikiDumpSearcher.Properties.Settings.Default.BulletHash = rdoBullet.Checked;

            //WikiDumpSearcher.Properties.Settings.Default.FileName = fileName;

            //WikiDumpSearcher.Properties.Settings.Default.Save();
        }

        private void loadSettings()
        {
            //menu
            //ignoreRedirectsToolStripMenuItem1.Checked = WikiDumpSearcher.Properties.Settings.Default.IgnoreRedirects;
            //ignoreCommentsToolStripMenuItem.Checked = WikiDumpSearcher.Properties.Settings.Default.IgnoreComments;
            //ignoreDisambigsToolStripMenuItem.Checked = WikiDumpSearcher.Properties.Settings.Default.IgnoreDisambigs;
            //ignoreImagesToolStripMenuItem.Checked = WikiDumpSearcher.Properties.Settings.Default.IgnoreImage;
            //ignoreTemplateNamespaceToolStripMenuItem.Checked = WikiDumpSearcher.Properties.Settings.Default.IgnoreTemplate;
            //ignoreWikipediaNamespaceToolStripMenuItem.Checked = WikiDumpSearcher.Properties.Settings.Default.IgnoreWikipedia;
            //ignoreCategoryNamespaceToolStripMenuItem.Checked = WikiDumpSearcher.Properties.Settings.Default.IngoreCategory;
            //ignoreMainNamespaceToolStripMenuItem.Checked = WikiDumpSearcher.Properties.Settings.Default.IgnoreMain;

            //Threadpriority = WikiDumpSearcher.Properties.Settings.Default.ThreadPriority;

            ////contains
            //txtPattern.Text = WikiDumpSearcher.Properties.Settings.Default.DoesContain;
            //txtPatternNot.Text = WikiDumpSearcher.Properties.Settings.Default.DoesNotContain;
            //chkDoesContain.Checked = WikiDumpSearcher.Properties.Settings.Default.DoesContainEnabled;
            //chkDoesNotContain.Checked = WikiDumpSearcher.Properties.Settings.Default.DoesNotContainEnabled;

            //chkRegex.Checked = WikiDumpSearcher.Properties.Settings.Default.IsRegex;
            //chkCaseSensitive.Checked = WikiDumpSearcher.Properties.Settings.Default.IsCaseSens;
            //chkMulti.Checked = WikiDumpSearcher.Properties.Settings.Default.IsMulti;
            //chkSingle.Checked = WikiDumpSearcher.Properties.Settings.Default.IsSingle;

            ////title
            //txtTitleContains.Text = WikiDumpSearcher.Properties.Settings.Default.TitleContains;
            //txtTitleNotContains.Text = WikiDumpSearcher.Properties.Settings.Default.TitleDoesNotContain;
            //chkCheckTitle.Checked = WikiDumpSearcher.Properties.Settings.Default.TitleContainsEnabled;
            //chkCheckNotInTitle.Checked = WikiDumpSearcher.Properties.Settings.Default.TitleNotContainsEnabled;
            //chkTitleRegex.Checked = WikiDumpSearcher.Properties.Settings.Default.TitleIsRegex;

            ////characters and links
            //cmboLength.SelectedIndex = WikiDumpSearcher.Properties.Settings.Default.CountCharatersIndex;
            //cmboLinks.SelectedIndex = WikiDumpSearcher.Properties.Settings.Default.CountLinksIndex;
            //numLength.Value = WikiDumpSearcher.Properties.Settings.Default.Characters;
            //nudLinks.Value = WikiDumpSearcher.Properties.Settings.Default.Links;

            ////extra
            //chkNoBold.Checked = WikiDumpSearcher.Properties.Settings.Default.BoldTitle;
            //chkBadLinks.Checked = WikiDumpSearcher.Properties.Settings.Default.BadLinks;
            //chkHTML.Checked = WikiDumpSearcher.Properties.Settings.Default.HTMLEntities;
            //chkSimpleLinks.Checked = WikiDumpSearcher.Properties.Settings.Default.SimpleLinks;

            ////results
            //chkHeading.Checked = WikiDumpSearcher.Properties.Settings.Default.AddHeading;
            //nudHeadingSpace.Value = WikiDumpSearcher.Properties.Settings.Default.HeadingGap;
            //nudLimitResults.Value = WikiDumpSearcher.Properties.Settings.Default.ResultsLimit;
            //rdoBullet.Checked = WikiDumpSearcher.Properties.Settings.Default.BulletHash;

            //fileName = WikiDumpSearcher.Properties.Settings.Default.FileName;
        }

        private void resetSettings()
        {
            //menu
            ignoreRedirectsToolStripMenuItem1.Checked = true;
            ignoreCommentsToolStripMenuItem.Checked = false;
            ignoreDisambigsToolStripMenuItem.Checked = false;
            ignoreImagesToolStripMenuItem.Checked = true;
            ignoreTemplateNamespaceToolStripMenuItem.Checked = true;
            ignoreWikipediaNamespaceToolStripMenuItem.Checked = true;
            ignoreCategoryNamespaceToolStripMenuItem.Checked = true;
            ignoreMainNamespaceToolStripMenuItem.Checked = false;

            //contains
            txtPattern.Text = "";
            txtPatternNot.Text = "";
            chkDoesContain.Checked = false; ;
            chkDoesNotContain.Checked = false;

            chkRegex.Checked = false;
            chkCaseSensitive.Checked = false;
            chkMulti.Checked = false;
            chkSingle.Checked = false;

            //title
            txtTitleContains.Text = "";
            txtTitleNotContains.Text = "";
            chkCheckTitle.Checked = false;
            chkCheckNotInTitle.Checked = false;
            chkTitleRegex.Checked = false;

            //characters and links
            cmboLength.SelectedIndex = 0;
            cmboLinks.SelectedIndex = 0;
            numLength.Value = 1000;
            nudLinks.Value = 5;

            //extra
            chkNoBold.Checked = false;
            chkBadLinks.Checked = false;
            chkHTML.Checked = false;
            chkSimpleLinks.Checked = false;

            //results
            chkHeading.Checked = false;
            nudHeadingSpace.Value = 25;
            nudLimitResults.Value = 30000;
            rdoBullet.Checked = false;
            rdoHash.Checked = true;

            fileName = ""; ;
        }

        #endregion
    }
}