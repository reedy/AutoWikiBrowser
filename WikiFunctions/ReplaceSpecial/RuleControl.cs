/*
    Derived from Autowikibrowser
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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;


namespace WikiFunctions.MWB
{

    public partial class RuleControl : UserControl
    {
        IRuleControlOwner owner_ = null;


        public RuleControl(IRuleControlOwner owner)
        {
            InitializeComponent();

            owner_ = owner;
            this.Anchor =
              AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
        }


        public void SetName(string name)
        {
            NameTextbox.Text = name;
        }

        public void SelectName()
        {
            NameTextbox.Select();
            NameTextbox.SelectAll();
        }

        public void SaveToRule(Rule r)
        {
            if (r == null)
                return;

            r.Name = NameTextbox.Text.Trim();
            r.replace_ = ReplaceTextbox.Text;
            r.with_ = WithTextbox.Text;
            r.ruletype_ = (Rule.T)RuleTypeCombobox.SelectedIndex;
            r.enabled_ = RuleEnabledCheckBox.Checked;

            r.regex_ = ReplaceIsRegexCheckbox.Checked;
            r.regexOptions_ = RegexOptions.None;
            if (!ReplaceIsCaseSensitiveCheckBox.Checked)
                r.regexOptions_ |= RegexOptions.IgnoreCase;
            if (ReplaceIsSinglelineCheckbox.Checked)
                r.regexOptions_ |= RegexOptions.Singleline;
            if (ReplaceIsMultilineCheckBox.Checked)
                r.regexOptions_ |= RegexOptions.Multiline;

            r.numoftimes_ = (int)NumberOfTimesUpDown.Value;

            r.ifContains_ = IfContainsTextBox.Text;
            r.ifNotContains_ = IfNotContainsTextBox.Text;

            r.ifIsRegex_ = IfIsRegexCheckBox.Checked;
            r.ifRegexOptions_ = RegexOptions.None;
            if (!IfIsCaseSensitiveCheckBox.Checked)
                r.ifRegexOptions_ |= RegexOptions.IgnoreCase;
            if (IfIsSinglelineCheckBox.Checked)
                r.ifRegexOptions_ |= RegexOptions.Singleline;
            if (IfIsMultilineCheckbox.Checked)
                r.ifRegexOptions_ |= RegexOptions.Multiline;
        }


        public void RestoreFromRule(Rule r)
        {
            NameTextbox.Text = r.Name;
            //ReplaceTextbox.Text = r.replace_;
            ReplaceTextbox.Text = Regex.Replace(r.replace_, @"
", "\r\n");
            WithTextbox.Text = Regex.Replace(r.with_, @"
", "\r\n");
            RuleTypeCombobox.SelectedIndex = (int)r.ruletype_;
            RuleEnabledCheckBox.Checked = r.enabled_;

            ReplaceIsRegexCheckbox.Checked = r.regex_;
            ReplaceIsCaseSensitiveCheckBox.Checked = (r.regexOptions_ & RegexOptions.IgnoreCase) == 0;
            ReplaceIsMultilineCheckBox.Checked = (r.regexOptions_ & RegexOptions.Multiline) > 0;
            ReplaceIsSinglelineCheckbox.Checked = (r.regexOptions_ & RegexOptions.Singleline) > 0;

            NumberOfTimesUpDown.Value = r.numoftimes_;

            IfContainsTextBox.Text = r.ifContains_;
            IfNotContainsTextBox.Text = r.ifNotContains_;

            IfIsRegexCheckBox.Checked = r.ifIsRegex_;
            IfIsCaseSensitiveCheckBox.Checked = (r.ifRegexOptions_ & RegexOptions.IgnoreCase) == 0;
            IfIsMultilineCheckbox.Checked = (r.ifRegexOptions_ & RegexOptions.Multiline) > 0;
            IfIsSinglelineCheckBox.Checked = (r.ifRegexOptions_ & RegexOptions.Singleline) > 0;

            UpdateRegexOptionCheckboxes();
        }


        void UpdateRegexOptionCheckboxes()
        {
            bool enable = ReplaceIsRegexCheckbox.Checked;
            ReplaceIsCaseSensitiveCheckBox.Enabled = enable;
            ReplaceIsMultilineCheckBox.Enabled = enable;
            ReplaceIsSinglelineCheckbox.Enabled = enable;

            enable = IfIsRegexCheckBox.Checked;
            IfIsCaseSensitiveCheckBox.Enabled = enable;
            IfIsMultilineCheckbox.Enabled = enable;
            IfIsSinglelineCheckBox.Enabled = enable;
        }


        private void ReplaceIsRegexCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRegexOptionCheckboxes();
        }


        private void IfIsRegexCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRegexOptionCheckboxes();
        }

        private void NameTextbox_TextChanged(object sender, EventArgs e)
        {
            owner_.NameChanged(this, NameTextbox.Text.Trim());
        }

        private void NameTextbox_DoubleClick(object sender, EventArgs e)
        {
            NameTextbox.SelectAll();
        }
    }

}
