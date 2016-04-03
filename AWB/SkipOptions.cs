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
using System.Linq;
using System.Windows.Forms;
using WikiFunctions;
using WikiFunctions.Plugin;

namespace AutoWikiBrowser
{
    internal sealed partial class SkipOptions : Form, ISkipOptions
    {
        public SkipOptions()
        {
            InitializeComponent();

            foreach (KeyValuePair<int, string> kvp in skipOptions)
            {
                skipListBox.Items.Add(new CheckedBoxItem
                {
                    ID = kvp.Key,
                    Description = kvp.Value
                });
            }
        }

        private readonly Dictionary<int, string> skipOptions = new Dictionary<int, string>
        {
            {1, "Title boldened"},
            {2, "External link bulleted"},
            {3, "Bad links fixed"},
            {4, "Unicodification"},
            {5, "Auto tag changes"},
            {6, "Header error fixed"},
            {7, "{{defaultsort}} added"},
            {8, "User talk templates subst'd"},
            {9, "Citation templates dates fixed"},
            {10, "Human category changes"},
        };

        #region Properties

        // NOTE 0 based indexing

        public bool SkipNoBoldTitle
        {
            get { return skipListBox.GetItemChecked(0); }
        }

        public bool SkipNoBulletedLink
        {
            get { return skipListBox.GetItemChecked(1); }
        }

        public bool SkipNoBadLink
        {
            get { return skipListBox.GetItemChecked(2); }
        }

        public bool SkipNoUnicode
        {
            get { return skipListBox.GetItemChecked(3); }
        }

        public bool SkipNoTag
        {
            get { return skipListBox.GetItemChecked(4); }
        }

        public bool SkipNoHeaderError
        {
            get { return skipListBox.GetItemChecked(5); }
        }

        public bool SkipNoDefaultSortAdded
        {
            get { return skipListBox.GetItemChecked(6); }
        }

        public bool SkipNoUserTalkTemplatesSubstd
        {
            get { return skipListBox.GetItemChecked(7); }
        }

        public bool SkipNoCiteTemplateDatesFixed
        {
            get { return skipListBox.GetItemChecked(8); }
        }

        public bool SkipNoPeopleCategoriesFixed
        {
            get { return skipListBox.GetItemChecked(9); }
        }
        #endregion

        #region Methods

        private void SkipOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        public List<int> SelectedItems
        {
            get
            {
                List<int> items = new List<int>();
                for (int i = 0; i < skipListBox.Items.Count; i++)
                {
                    if (skipListBox.GetItemChecked(i))
                    {
                        CheckedBoxItem cbi = (CheckedBoxItem)skipListBox.Items[i];
                        items.Add(cbi.ID);
                    }
                }
                return items;
            }
            set
            {
                CheckAll.Checked = false;
                CheckNone.Checked = false;

                for (int i = 0; i < skipListBox.Items.Count; i++)
                {
                    CheckedBoxItem cbi = (CheckedBoxItem) skipListBox.Items[i];
                    skipListBox.SetItemChecked(i, value.Contains(cbi.ID));
                }
            }
        }

        #endregion

        private void CheckAll_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckAll.Checked)
            {
                CheckNone.Checked = false;
                SetCheckboxes(true);
            }
        }

        private void CheckNone_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckNone.Checked)
            {
                CheckAll.Checked = false;
                SetCheckboxes(false);
            }
        }

        private void SetCheckboxes(bool value)
        {
            for (int i = 0; i < skipListBox.Items.Count; i++)
            {
                skipListBox.SetItemChecked(i, value);
            }
        }
    }
}