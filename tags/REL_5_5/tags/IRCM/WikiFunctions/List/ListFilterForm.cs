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
    public partial class ListFilterForm : Form
    {
        private ListBox2 destListBox;

        public ListFilterForm(ListBox2 lb)
        {
            InitializeComponent();

            if (lb == null)
                throw new ArgumentNullException("lb");

            destListBox = lb;

            if (prefs != null)
                Settings = ListFilterForm.prefs;
        }      

        List<Article> list = new List<Article>();
        static WikiFunctions.AWBSettings.SpecialFilterPrefs prefs;

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkRemoveDups.Checked)
                    RemoveDuplicates();

                if (chkSortAZ.Checked)
                    destListBox.Sort();

                list.Clear();

                foreach (Article a in destListBox)
                    list.Add(a);

                bool does = (chkContains.Checked && !string.IsNullOrEmpty(txtContains.Text));
                bool doesnot = (chkNotContains.Checked && !string.IsNullOrEmpty(txtDoesNotContain.Text));

                if (lbRemove.Items.Count > 0)
                    FilterList();

                if (does || doesnot)
                    FilterMatches(does, doesnot);

                if (destListBox.Items.Count > 0 && destListBox.Items[0] is Article)
                    FilterNamespace();

                destListBox.Items.Clear();

                foreach (Article a in list)
                    destListBox.Items.Add(a);

                //Only try to update number of articles using listmaker method IF the parent is indeed a listmaker
                //Causes exception on DBScanner otherwise
                if (destListBox.Parent is ListMaker)
                    (destListBox.Parent as ListMaker).UpdateNumberOfArticles();
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

            foreach (Article a in destListBox)
            {
                if (!list.Contains(a))
                    list.Add(a);
            }
            destListBox.Items.Clear();

            foreach (Article a in list)
                destListBox.Items.Add(a);
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
                // Doesn't seem to work
                //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#Can.27t_find_difference_in_list_filter
                // loaded in:
                // a, b, c
                //       c, d, e
                // expected:
                // a, b,    d, e
                // got:
                // (none)
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
            foreach (Article a in new WikiFunctions.Lists.TextFileListProvider().MakeList())
                lbRemove.Items.Add(a);
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
            chkIsRegex.Enabled = (chkContains.Checked || chkNotContains.Checked);
        }

        private void chkNotContains_CheckedChanged(object sender, EventArgs e)
        {
            txtDoesNotContain.Enabled = chkNotContains.Checked;
            chkIsRegex.Enabled = (chkContains.Checked || chkNotContains.Checked);
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

                chkPortal.Visible = chkPortalTalk.Visible = true;
            }
            else
                chkPortal.Visible = chkPortalTalk.Visible = false;
        }

        private void specialFilter_Load(object sender, EventArgs e)
        {
            cbOpType.SelectedIndex = 0;
        }
        internal void Clear()
        {
            list.Clear();
        }

        private void SpecialFilter_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible) UpdateText();
        }

        private void GetListTags(Control.ControlCollection controls)
        {
            foreach (Control cntrl in controls)
            {
                if (cntrl is FlowLayoutPanel)
                {
                    GetListTags(cntrl.Controls);
                    continue;
                }

                tmp = (cntrl as CheckBox);

                if (tmp != null && tmp.Checked && tmp.Tag != null)
                    prefs.namespaceValues.Add(int.Parse(tmp.Tag.ToString()));
            }
        }

        private void SetListTags(Control.ControlCollection controls)
        {
            foreach (Control cntrl in controls)
            {
                if (cntrl is FlowLayoutPanel)
                {
                    SetListTags(cntrl.Controls);
                    continue;
                }

                tmp = (cntrl as CheckBox);

                if (tmp != null && tmp.Tag != null)
                    tmp.Checked = prefs.namespaceValues.Contains(int.Parse(tmp.Tag.ToString()));
            }
        }

        CheckBox tmp;
        public WikiFunctions.AWBSettings.SpecialFilterPrefs Settings
        {
            get
            {
                prefs = new WikiFunctions.AWBSettings.SpecialFilterPrefs();

                GetListTags(gbNamespaces.Controls);

                prefs.filterTitlesThatContain = chkContains.Checked;
                prefs.filterTitlesThatContainText = txtContains.Text;
                prefs.filterTitlesThatDontContain = chkNotContains.Checked;
                prefs.filterTitlesThatDontContainText = txtDoesNotContain.Text;
                prefs.areRegex = chkIsRegex.Checked;

                prefs.remDupes = chkRemoveDups.Checked;
                prefs.sortAZ = chkSortAZ.Checked;

                prefs.opType = cbOpType.SelectedIndex;

                foreach (string s in lbRemove.Items)
                    prefs.remove.Add(s);

                return prefs;
            }
            set
            {
                prefs = value;

                if ((prefs != null) && (prefs.namespaceValues.Count > 0))
                {
                    SetListTags(gbNamespaces.Controls);
                }

                chkContains.Checked = prefs.filterTitlesThatContain;
                txtContains.Text = prefs.filterTitlesThatContainText;
                chkNotContains.Checked = prefs.filterTitlesThatDontContain;
                txtDoesNotContain.Text = prefs.filterTitlesThatDontContainText;
                chkIsRegex.Checked = prefs.areRegex;

                chkRemoveDups.Checked = prefs.remDupes;
                chkSortAZ.Checked = prefs.sortAZ;

                cbOpType.SelectedIndex = prefs.opType;

                foreach (string s in prefs.remove)
                    lbRemove.Items.Add(s);
            }
        }

        private void chkContents_CheckedChanged(object sender, EventArgs e)
        {
            foreach (CheckBox chk in flwContent.Controls)
            {
                chk.Checked = chkContents.Checked;
            }
        }

        private void chkTalk_CheckedChanged(object sender, EventArgs e)
        {
            foreach (CheckBox chk in flwTalk.Controls)
            {
                chk.Checked = chkTalk.Checked;
            }
        }
        private void Content_CheckedChanged(object sender, EventArgs e)
        {
            // Should check the main checkbox when all sub checkboxes are checked
            // and "Indeterminate" when some of them are checked
            // and unchecked when all the checkboxes are unchecked
            // Doesn't work

            // chkContents.CheckState = CheckState.Indeterminate;
        }
        private void Talk_CheckedChanged(object sender, EventArgs e)
        {
            // chkTalk.CheckState = CheckState.Indeterminate;
        }
    }
}
