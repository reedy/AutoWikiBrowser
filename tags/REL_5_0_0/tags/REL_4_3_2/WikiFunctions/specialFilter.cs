/*
Autowikibrowser
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
using System.Collections;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Threading;
using WikiFunctions;
using WikiFunctions.Controls.Lists;

namespace WikiFunctions.Lists
{
    public partial class SpecialFilter : Form
    {
        public ListBox2 lb;

        public SpecialFilter()
        {
            InitializeComponent();
            if (prefs != null)
                Settings = SpecialFilter.prefs;
            //UpdateText();
        }      

        List<Article> list = new List<Article>();
        static WikiFunctions.AWBSettings.SpecialFilterPrefs prefs;

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkRemoveDups.Checked)
                    RemoveDuplicates();

                list.Clear();

                foreach (Article a in lb)
                    list.Add(a);

                bool does = (chkContains.Checked && !string.IsNullOrEmpty(txtContains.Text));
                bool doesnot = (chkNotContains.Checked && !string.IsNullOrEmpty(txtDoesNotContain.Text));

                if (lbRemove.Items.Count > 0)
                    FilterList();

                if (does || doesnot)
                    FilterMatches(does, doesnot);

                if (lb.Items.Count > 0 && lb.Items[0] is Article)
                    FilterNamespace();

                lb.Items.Clear();

                foreach (Article a in list)
                    lb.Items.Add(a);

                //Only try to update number of articles using listmaker method IF the parent is indeed a listmaker
                //Causes exception on DBScanner otherwise
                if (lb.Parent is ListMaker)
                    (lb.Parent as ListMaker).UpdateNumberOfArticles();
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
            DialogResult = DialogResult.OK;
        }

        public void RemoveDuplicates()
        {
            list.Clear();

            foreach (Article a in lb)
            {
                if (!list.Contains(a))
                    list.Add(a);
            }
            lb.Items.Clear();

            foreach (Article a in list)
                lb.Items.Add(a);
        }

        private void FilterNamespace()
        {
            int i = 0;

            while (i < list.Count)
            {
                if (list[i].NameSpaceKey == 0)
                {
                    if (chkArticle.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 1)
                {
                    if (chkArticleTalk.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 2)
                {
                    if (chkUser.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 3)
                {
                    if (chkUserTalk.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 4)
                {
                    if (chkWikipedia.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 5)
                {
                    if (chkWikipediaTalk.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 6)
                {
                    if (chkImage.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 7)
                {
                    if (chkImageTalk.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 8)
                {
                    if (chkMediaWiki.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 9)
                {
                    if (chkMediaWikiTalk.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 10)
                {
                    if (chkTemplate.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 11)
                {
                    if (chkTemplateTalk.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 12)
                {
                    if (chkHelp.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 13)
                {
                    if (chkHelpTalk.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 14)
                {
                    if (chkCategory.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 15)
                {
                    if (chkCategoryTalk.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 100)
                {
                    if (chkPortal.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey == 101)
                {
                    if (chkPortalTalk.Checked)
                        i++;
                    else
                        list.RemoveAt(i);
                }
                else if (list[i].NameSpaceKey > 101)
                {
                    // Filter out all obscure namespaces
                    list.RemoveAt(i);
                }
                else
                    i++;
            }
        }

        private void FilterMatches(bool does, bool doesnot)
        {
            string strMatch = txtContains.Text;
            string strNotMatch = txtDoesNotContain.Text;

            try
            {
                if (!chkIsRegex.Checked)
                {
                    strMatch = Regex.Escape(strMatch);
                    strNotMatch = Regex.Escape(strNotMatch);
                }

                Regex match = new Regex(strMatch);
                Regex notMatch = new Regex(strNotMatch);

                int i = 0;
                while (i < list.Count)
                {
                    if (does && match.IsMatch(list[i].Name))
                        list.RemoveAt(i);
                    else if (doesnot && !notMatch.IsMatch(list[i].Name))
                        list.RemoveAt(i);
                    else
                        i++;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        private void FilterList()
        {
            // has to be sorted so binary search can work
            List<Article> remove = new List<Article>();
            remove.AddRange(lbRemove);
            remove.Sort();

            if (cbOpType.SelectedIndex == 0)
            {
                // find difference
                List<Article> list2 = new List<Article>();
                foreach (Article a in list)
                    if (BinarySearch(remove, a, 0, remove.Count - 1) == -1)
                        list2.Add(a);
                list = list2;
            }
            else
            {
                // find intersection
                List<Article> list2 = new List<Article>();
                foreach (Article a in list)
                    if (BinarySearch(remove, a, 0, remove.Count - 1) != -1)
                        list2.Add(a);
                list = list2;
            }
        }

        private int BinarySearch(IList<Article> list, Article article, int left, int right)
        {
            if (right < left)
                return -1;
            int mid = (left + right) / 2;
            int compare = String.Compare(list[mid].ToString(), article.ToString(), false, CultureInfo.InvariantCulture);

            if (compare > 0)
                return BinarySearch(list, article, left, mid - 1);
            else if (compare < 0)
                return BinarySearch(list, article, mid + 1, right);
            else
                return mid;
        }

        private void btnGetList_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            List<Article> list2 = new List<Article>();

            try
            {
                if (of.ShowDialog() == DialogResult.OK)
                {
                    list2 = GetLists.FromTextFile(of.FileName);

                    foreach (Article a in list2)
                        lbRemove.Items.Add(a);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lbRemove.Items.Clear();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void chkContains_CheckedChanged(object sender, EventArgs e)
        {
            txtContains.Enabled = chkContains.Checked;
            chkIsRegex.Enabled = chkContains.Checked || chkNotContains.Checked;
        }

        private void chkNotContains_CheckedChanged(object sender, EventArgs e)
        {
            txtDoesNotContain.Enabled = chkNotContains.Checked;
            chkIsRegex.Enabled = chkContains.Checked || chkNotContains.Checked;
        }

        public void UpdateText()
        {
            chkArticleTalk.Text = Variables.Namespaces[1];
            chkUser.Text = Variables.Namespaces[2];
            chkUserTalk.Text = Variables.Namespaces[3];
            chkWikipedia.Text = Variables.Namespaces[4];
            chkWikipediaTalk.Text = Variables.Namespaces[5];
            chkImage.Text = Variables.Namespaces[6];
            chkImageTalk.Text = Variables.Namespaces[7];
            chkMediaWiki.Text = Variables.Namespaces[8];
            chkMediaWikiTalk.Text = Variables.Namespaces[9];
            chkTemplate.Text = Variables.Namespaces[10];
            chkTemplateTalk.Text = Variables.Namespaces[11];
            chkHelp.Text = Variables.Namespaces[12];
            chkHelpTalk.Text = Variables.Namespaces[13];
            chkCategory.Text = Variables.Namespaces[14];
            chkCategoryTalk.Text = Variables.Namespaces[15];
            if (Variables.Namespaces.ContainsKey(100))
            {
                chkPortal.Text = Variables.Namespaces[100];
                chkPortalTalk.Text = Variables.Namespaces[101];
            }

            chkPortal.Visible = (Variables.Namespaces.ContainsKey(100));
            chkPortalTalk.Visible = (Variables.Namespaces.ContainsKey(100));
        }

        #region contextMenu

        private void nonTalkOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetSomeChecks(false);
        }

        private void talkSpaceOnlyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetSomeChecks(true);
        }

        private void SetSomeChecks(bool All)
        {
            chkArticle.Checked = !All;
            chkArticleTalk.Checked = All;
            chkCategory.Checked = !All;
            chkCategoryTalk.Checked = All;
            chkHelp.Checked = !All;
            chkHelpTalk.Checked = All;
            chkImage.Checked = !All;
            chkImageTalk.Checked = All;
            chkMediaWiki.Checked = !All;
            chkMediaWikiTalk.Checked = All;
            chkPortal.Checked = !All;
            chkPortalTalk.Checked = All;
            chkTemplate.Checked = !All;
            chkTemplateTalk.Checked = All;
            chkUser.Checked = !All;
            chkUserTalk.Checked = All;
            chkWikipedia.Checked = !All;
            chkWikipediaTalk.Checked = All;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCheckBoxes(true);
        }

        private void deselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetCheckBoxes(false);
        }

        #endregion

        private void specialFilter_Load(object sender, EventArgs e)
        {
            cbOpType.SelectedIndex = 0;
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            SetCheckBoxes(true);
        }

        private void btnSelectNone_Click(object sender, EventArgs e)
        {
            SetCheckBoxes(false);
        }

        private void SetCheckBoxes(bool All)
        {
            chkArticle.Checked = chkArticleTalk.Checked = chkCategory.Checked = chkCategoryTalk.Checked =
            chkHelp.Checked = chkHelpTalk.Checked = chkImage.Checked = chkImageTalk.Checked = chkMediaWiki.Checked =
            chkMediaWikiTalk.Checked = chkPortal.Checked = chkPortalTalk.Checked = chkTemplate.Checked = 
            chkTemplateTalk.Checked = chkUser.Checked = chkUserTalk.Checked = chkWikipedia.Checked =
            chkWikipediaTalk.Checked = All;
        }

        internal void Clear()
        {
            list.Clear();
        }

        private void btnTalkOnly_Click(object sender, EventArgs e)
        {
            SetSomeChecks(true);
        }

        private void btnNonTalk_Click(object sender, EventArgs e)
        {
            SetSomeChecks(false);
        }

        private void SpecialFilter_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible) UpdateText();
        }

        public WikiFunctions.AWBSettings.SpecialFilterPrefs Settings
        {
            get
            {
                prefs = new WikiFunctions.AWBSettings.SpecialFilterPrefs();

                //CheckBox tmp;
                //foreach (Control chk in groupBox1.Controls)
                //{
                //    tmp = (chk as CheckBox);

                //    if (tmp == null)
                //        continue;
                //    try
                //    {
                //        string name = chk.Text;

                //        if (name.Contains("Article"))
                //            name = name.Replace("Article", "");

                //        prefs.namespaceValues.Add(Tools.CalculateNS(name + ":"), tmp.Checked);
                //    }
                //    catch
                //    {
                //    }
                //}

                prefs.filterTitlesThatContain = chkContains.Checked;
                prefs.filterTitlesThatContainText = txtContains.Text;
                prefs.filterTitlesThatDontContain = chkNotContains.Checked;
                prefs.filterTitlesThatDontContainText = txtDoesNotContain.Text;
                prefs.areRegex = chkIsRegex.Checked;

                prefs.remDupes = chkRemoveDups.Checked;

                prefs.opType = cbOpType.SelectedIndex;

                foreach (string s in lbRemove.Items)
                    prefs.remove.Add(s);

                return prefs;
            }
            set
            {
                prefs = value;

                //if (prefs.namespaceValues.Count > 0)
                //{
                //    CheckBox tmp;
                //    foreach (Control chk in groupBox1.Controls)
                //    {
                //        tmp = chk as CheckBox;

                //        if (tmp == null)
                //            continue;

                //        string name = chk.Text;

                //        if (name.Contains("Article"))
                //            name = name.Replace("Article", "").Trim();

                //        prefs.namespaceValues.Add(Tools.CalculateNS(name + ":"), tmp.Checked);
                //    }
                //}

                chkContains.Checked = prefs.filterTitlesThatContain;
                txtContains.Text = prefs.filterTitlesThatContainText;
                chkNotContains.Checked = prefs.filterTitlesThatDontContain;
                txtDoesNotContain.Text = prefs.filterTitlesThatDontContainText;
                chkIsRegex.Checked = prefs.areRegex;

                chkRemoveDups.Checked = prefs.remDupes;

                cbOpType.SelectedIndex = prefs.opType;

                foreach (string s in prefs.remove)
                    lbRemove.Items.Add(s);
            }
        }
    }
}