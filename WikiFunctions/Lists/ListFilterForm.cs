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
using WikiFunctions.Controls.Lists;
using WikiFunctions.Lists.Providers;
using System.Linq;

namespace WikiFunctions.Lists
{
    public partial class ListFilterForm : Form
    {
        private readonly ListBoxArticle _destListBox;

        string _project = Variables.URL;

        public ListFilterForm(ListBoxArticle lb)
        {
            InitializeComponent();

            if (lb == null)
                throw new ArgumentNullException("lb");

            _destListBox = lb;

            if (_prefs != null)
                Settings = _prefs;
        }

        private List<Article> _list = new List<Article>();
        private static AWBSettings.SpecialFilterPrefs _prefs;

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkRemoveDups.Checked)
                    RemoveDuplicates();

                _list.Clear();

                _list.AddRange(_destListBox);

                bool does = (chkContains.Checked && !string.IsNullOrEmpty(txtContains.Text));
                bool doesnot = (chkNotContains.Checked && !string.IsNullOrEmpty(txtDoesNotContain.Text));

                if (lbRemove.Items.Count > 0)
                    FilterList();

                if (does || doesnot)
                    FilterMatches(does, doesnot);

                if (_destListBox.Items.Count > 0 && _destListBox.Items[0] is Article)
                    FilterNamespace();

                if(_list.Count != _destListBox.Items.Count)
                {
                    _destListBox.BeginUpdate();
                    _destListBox.Items.Clear();
                    _destListBox.Items.AddRange(_list.ToArray());
                    _destListBox.EndUpdate();
                }

                if (chkSortAZ.Checked)
                    _destListBox.Sort();

                //Only try to update number of articles using listmaker method IF the parent is indeed a listmaker
                //Causes exception on DBScanner otherwise
                if (_destListBox.Parent is ListMaker)
                    (_destListBox.Parent as ListMaker).UpdateNumberOfArticles();

            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
            }
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// Removes duplicate articles from the listbox
        /// </summary>
        public void RemoveDuplicates()
        {
            if(Globals.SystemCore3500Available)
                RemoveDuplicatesNew();
            else
                RemoveDuplicatesOld();
        }

        private void RemoveDuplicatesNew()
        {
            ClearAndAdd(_destListBox.Distinct().ToArray());
        }

        private void RemoveDuplicatesOld()
        {
            _list.Clear();

            foreach (Article a in _destListBox)
            {
                if (!_list.Contains(a))
                    _list.Add(a);
            }

            ClearAndAdd(_list.ToArray());
        }

        private void ClearAndAdd(Article[] newlist)
        {
            // Avoid performance penalty of AddRange if deduplication didn't remove any articles
            if(_destListBox.Items.Count != newlist.Length)
            {
                _destListBox.BeginUpdate();
                _destListBox.Items.Clear();
                _destListBox.Items.AddRange(newlist);
                _destListBox.EndUpdate();
            }
        }

        private void FilterNamespace()
        {
            List<int> selectedNS = pageNamespaces.GetSelectedNamespaces();

            if (selectedNS.Count == 0)
                return;

            _list.RemoveAll(a => !selectedNS.Contains(a.NameSpaceKey));
        }

        private void FilterMatches(bool does, bool doesnot)
        {
            if (!does && !doesnot)
                return;

            try
            {
                Regex match = null, notMatch = null;

                if (does)
                    match = new Regex(!chkIsRegex.Checked ? Regex.Escape(txtContains.Text) : txtContains.Text,
                                      RegexOptions.Compiled);

                if (doesnot)
                    notMatch = new Regex(
                        !chkIsRegex.Checked ? Regex.Escape(txtDoesNotContain.Text) : txtDoesNotContain.Text,
                        RegexOptions.Compiled);

                _list.RemoveAll(a => (does && match.IsMatch(a.Name) || doesnot && !notMatch.IsMatch(a.Name)));
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
            }
        }

        private void FilterList()
        {
            if(Globals.SystemCore3500Available)
                FilterListNew();
            else
                FilterListOld();
        }
        
        private void FilterListOld()
        {
            List<Article> remove = new List<Article>();
            remove.AddRange(lbRemove);

            List<Article> list2 = new List<Article>();

            if (cbOpType.SelectedIndex == 0)
            {
                // symmetric difference

                /* The symmetric difference of two sets is the set of elements which are in either of the sets and not in their intersection.
                    For example, the symmetric difference of the sets {1,2,3} and {3,4} is {1,2,4} */

                foreach (Article a in _list)
                    if (!remove.Contains(a))
                        list2.Add(a);
                    else
                        remove.Remove(a);

                foreach (Article a in remove)
                    if (!_list.Contains(a))
                        list2.Add(a);
            }
            else
            {
                // find intersection
                foreach (Article a in _list)
                    if (remove.Contains(a))
                        list2.Add(a);
            }
            _list = list2;
        }
        
        private void FilterListNew()
        {
            List<Article> remove = new List<Article>(lbRemove);

            HashSet<Article> list = new HashSet<Article>(_list);

            if (cbOpType.SelectedIndex == 0)
            {
                /* The symmetric difference of two sets is the set of elements which are in either
                 * of the sets and not in their intersection. For example, the symmetric difference
                 * of the sets {1,2,3} and {3,4} is {1,2,4}.
                 */
                list.ExceptWith(remove);
            }
            else
            {
                // find intersection
                list.IntersectWith(remove);
            }
            _list = new List<Article>(list);
        }

        private void btnGetList_Click(object sender, EventArgs e)
        {
            lbRemove.Items.AddRange(new TextFileListProviderUFT8().MakeList().ToArray());
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
            _list.Clear();
        }

        private void SpecialFilter_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible && _project != Variables.URL)
            {
                _project = Variables.URL;
                pageNamespaces.Populate();
            }
        }

        [Browsable(false)]
        [Localizable(false)]
        public AWBSettings.SpecialFilterPrefs Settings
        {
            get
            {
                _prefs = new AWBSettings.SpecialFilterPrefs
                {
                    namespaceValues = pageNamespaces.GetSelectedNamespaces(),
                    filterTitlesThatContain = chkContains.Checked,
                    filterTitlesThatContainText = txtContains.Text,
                    filterTitlesThatDontContain = chkNotContains.Checked,
                    filterTitlesThatDontContainText = txtDoesNotContain.Text,
                    areRegex = chkIsRegex.Checked,
                    remDupes = chkRemoveDups.Checked,
                    sortAZ = chkSortAZ.Checked,
                    opType = cbOpType.SelectedIndex
                };

                foreach (Article a in lbRemove.Items)
                {
                    _prefs.remove.Add(a.Name);
                }

                return _prefs;
            }
            set
            {
                if (value == null || DesignMode)
                    return;

                _prefs = value;

                if (_prefs.namespaceValues == null)
                    _prefs.namespaceValues = new List<int>(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 10, 11, 14, 15 });

                if (_prefs.namespaceValues.Count > 0)
                    pageNamespaces.SetSelectedNamespaces(_prefs.namespaceValues);

                chkContains.Checked = _prefs.filterTitlesThatContain;
                txtContains.Text = _prefs.filterTitlesThatContainText;
                chkNotContains.Checked = _prefs.filterTitlesThatDontContain;
                txtDoesNotContain.Text = _prefs.filterTitlesThatDontContainText;
                chkIsRegex.Checked = _prefs.areRegex;

                chkRemoveDups.Checked = _prefs.remDupes;
                chkSortAZ.Checked = _prefs.sortAZ;

                cbOpType.SelectedIndex = _prefs.opType;

                foreach (string s in _prefs.remove)
                    lbRemove.Items.Add(new Article(s));
            }
        }
    }
}
