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
using System.Windows.Forms;

using WikiFunctions.Plugin;

namespace AutoWikiBrowser
{
    internal sealed partial class SkipOptions : Form, ISkipOptions
    {
        public SkipOptions()
        {
            InitializeComponent();
        }

        #region Properties

        public bool SkipNoUnicode
        {
            get { return chkNoUnicode.Checked; }
        }

        public bool SkipNoTag
        {
            get { return chkNoTag.Checked; }
        }

        public bool SkipNoHeaderError
        {
            get { return chkNoHeaderError.Checked; }
        }

        public bool SkipNoBoldTitle
        {
            get { return chkNoBoldTitle.Checked; }
        }

        public bool SkipNoBulletedLink
        {
            get { return chkNoBulletedLink.Checked; }
        }

        public bool SkipNoBadLink
        {
            get { return chkNoBadLink.Checked; }
        }

        public bool SkipNoDefaultSortAdded
        {
            get { return chkDefaultSortAdded.Checked; }
        }

        public bool SkipNoUserTalkTemplatesSubstd
        {
            get { return chkUserTalkTemplates.Checked; }
        }

        public bool SkipNoCiteTemplateDatesFixed
        {
            get { return chkCiteTemplateDates.Checked; }
        }

        public bool SkipNoPeopleCategoriesFixed
        {
            get { return chkPeopleCategories.Checked; }
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
                List<int> ret = new List<int>();
                foreach (CheckBox c in CheckBoxPanel.Controls)
                {
                    if (c.Checked)
                        ret.Add((int)c.Tag);
                }

                return ret;
            }
            set
            {
                if (value.Count <= 0) return;
                foreach (CheckBox c in CheckBoxPanel.Controls)
                {
                    c.Checked = value.Contains((int)c.Tag);
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
            foreach (CheckBox c in CheckBoxPanel.Controls)
            {
                c.Checked = value;
            }
        }
    }
}