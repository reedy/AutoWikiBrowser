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
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;
using WikiFunctions.Controls.Lists;

namespace WikiFunctions.Lists
{
    public partial class ListFilterForm : Form
    {
        private readonly ListBox2 destListBox;

        public ListFilterForm(ListBox2 lb)
        {
            InitializeComponent();

            if (lb == null)
                throw new ArgumentNullException("lb");

            destListBox = lb;

            if (prefs != null)
                Settings = prefs;
        }      

        List<Article> list = new List<Article>();
        static AWBSettings.SpecialFilterPrefs prefs;

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
            List<int> selectedNS = pageNamespaces.GetSelectedNamespaces();

            int i = 0;

            while (i < list.Count)
            {
                if (!selectedNS.Contains(list[i].NameSpaceKey))
                    list.RemoveAt(i);
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

            List<Article> list2 = new List<Article>();

            if (cbOpType.SelectedIndex == 0)
            {
                // find difference Doesn't seem to work
                //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#Can.27t_find_difference_in_list_filter
                // loaded in:
                // a, b, c
                //       c, d, e
                // expected:
                // a, b,    d, e
                // got:
                // (none)
                foreach (Article a in list)
                    if (BinarySearch(remove, a, 0, remove.Count - 1) == -1)
                        list2.Add(a);
            }
            else
            {
                // find intersection
                foreach (Article a in list)
                    if (BinarySearch(remove, a, 0, remove.Count - 1) != -1)
                        list2.Add(a);
            }
            list = list2;
        }

        private static int BinarySearch(IList<Article> articleList, Article article, int left, int right)
        {
            if (right < left)
                return -1;
            int mid = (left + right) / 2;
            int compare = String.Compare(articleList[mid].ToString(), article.ToString(), false, CultureInfo.InvariantCulture);

            if (compare > 0)
                return BinarySearch(articleList, article, left, mid - 1);
            if (compare < 0)
                return BinarySearch(articleList, article, mid + 1, right);
            return mid;
        }

        private void btnGetList_Click(object sender, EventArgs e)
        {
            foreach (Article a in new TextFileListProvider().MakeList())
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
            if (Visible) pageNamespaces.UpdateText();
        }

        [Browsable(false)]
        [Localizable(false)]
        public AWBSettings.SpecialFilterPrefs Settings
        {
            get
            {
                prefs = new AWBSettings.SpecialFilterPrefs();

                prefs.namespaceValues = pageNamespaces.GetSelectedNamespaces();

                prefs.filterTitlesThatContain = chkContains.Checked;
                prefs.filterTitlesThatContainText = txtContains.Text;
                prefs.filterTitlesThatDontContain = chkNotContains.Checked;
                prefs.filterTitlesThatDontContainText = txtDoesNotContain.Text;
                prefs.areRegex = chkIsRegex.Checked;

                prefs.remDupes = chkRemoveDups.Checked;
                prefs.sortAZ = chkSortAZ.Checked;

                prefs.opType = cbOpType.SelectedIndex;

                foreach (Article a in lbRemove.Items)
                {
                    prefs.remove.Add(a.Name);
                }

                return prefs;
            }
            set
            {
                if (value == null || DesignMode)
                    return;

                prefs = value;

                if (prefs.namespaceValues == null)
                    prefs.namespaceValues = new List<int>(new [] { 0, 1, 2, 3, 4, 5, 6, 7, 10, 11, 14, 15 });

                if (prefs.namespaceValues.Count > 0)
                    pageNamespaces.SetSelectedNamespaces(prefs.namespaceValues);

                chkContains.Checked = prefs.filterTitlesThatContain;
                txtContains.Text = prefs.filterTitlesThatContainText;
                chkNotContains.Checked = prefs.filterTitlesThatDontContain;
                txtDoesNotContain.Text = prefs.filterTitlesThatDontContainText;
                chkIsRegex.Checked = prefs.areRegex;

                chkRemoveDups.Checked = prefs.remDupes;
                chkSortAZ.Checked = prefs.sortAZ;

                cbOpType.SelectedIndex = prefs.opType;

                foreach (string s in prefs.remove)
                    lbRemove.Items.Add(new Article(s));
            }
        }
    }
}
