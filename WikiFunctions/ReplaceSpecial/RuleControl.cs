/*
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
using System.Windows.Forms;
using System.Text.RegularExpressions;
using WikiFunctions.Controls;

namespace WikiFunctions.ReplaceSpecial
{
    public partial class RuleControl : UserControl
    {
        readonly IRuleControlOwner owner_;

        public RuleControl(IRuleControlOwner owner)
        {
            InitializeComponent();

            owner_ = owner;
            Anchor =
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

        public void SaveToRule(Rule rule)
        {
            if (rule == null)
                return;

            rule.Name = NameTextbox.Text.Trim();
            rule.replace_ = ReplaceTextbox.Text.Replace("\r\n", "\n");
            rule.with_ = WithTextbox.Text.Replace("\r\n", "\n");
            rule.ruletype_ = (Rule.T)RuleTypeCombobox.SelectedIndex;
            rule.enabled_ = RuleEnabledCheckBox.Checked;

            rule.regex_ = ReplaceIsRegexCheckbox.Checked;
            rule.regexOptions_ = RegexOptions.None;
            if (!ReplaceIsCaseSensitiveCheckBox.Checked)
                rule.regexOptions_ |= RegexOptions.IgnoreCase;
            if (ReplaceIsSinglelineCheckbox.Checked)
                rule.regexOptions_ |= RegexOptions.Singleline;
            if (ReplaceIsMultilineCheckBox.Checked)
                rule.regexOptions_ |= RegexOptions.Multiline;

            rule.numoftimes_ = (int)NumberOfTimesUpDown.Value;

            rule.ifContains_ = IfContainsTextBox.Text;
            rule.ifNotContains_ = IfNotContainsTextBox.Text;

            rule.ifIsRegex_ = IfIsRegexCheckBox.Checked;
            rule.ifRegexOptions_ = RegexOptions.None;
            if (!IfIsCaseSensitiveCheckBox.Checked)
                rule.ifRegexOptions_ |= RegexOptions.IgnoreCase;
            if (IfIsSinglelineCheckBox.Checked)
                rule.ifRegexOptions_ |= RegexOptions.Singleline;
            if (IfIsMultilineCheckbox.Checked)
                rule.ifRegexOptions_ |= RegexOptions.Multiline;
        }

        public void RestoreFromRule(Rule rule)
        {
            NameTextbox.Text = rule.Name;

            ReplaceTextbox.Text = rule.replace_.Replace("\n", "\r\n");

            WithTextbox.Text = rule.with_.Replace("\n", "\r\n");

            RuleTypeCombobox.SelectedIndex = (int)rule.ruletype_;
            RuleEnabledCheckBox.Checked = rule.enabled_;

            ReplaceIsRegexCheckbox.Checked = rule.regex_;
            ReplaceIsCaseSensitiveCheckBox.Checked = (rule.regexOptions_ & RegexOptions.IgnoreCase) == 0;
            ReplaceIsMultilineCheckBox.Checked = (rule.regexOptions_ & RegexOptions.Multiline) > 0;
            ReplaceIsSinglelineCheckbox.Checked = (rule.regexOptions_ & RegexOptions.Singleline) > 0;

            NumberOfTimesUpDown.Value = rule.numoftimes_;

            IfContainsTextBox.Text = rule.ifContains_;
            IfNotContainsTextBox.Text = rule.ifNotContains_;

            IfIsRegexCheckBox.Checked = rule.ifIsRegex_;
            IfIsCaseSensitiveCheckBox.Checked = (rule.ifRegexOptions_ & RegexOptions.IgnoreCase) == 0;
            IfIsMultilineCheckbox.Checked = (rule.ifRegexOptions_ & RegexOptions.Multiline) > 0;
            IfIsSinglelineCheckBox.Checked = (rule.ifRegexOptions_ & RegexOptions.Singleline) > 0;

            UpdateRegexOptionCheckboxes();
        }

        private void UpdateRegexOptionCheckboxes()
        {
            bool enable = ReplaceIsRegexCheckbox.Checked;
            ReplaceIsMultilineCheckBox.Enabled = enable;
            ReplaceIsSinglelineCheckbox.Enabled = enable;
            TestFind.Enabled = enable;

            enable = IfIsRegexCheckBox.Checked;
            IfIsMultilineCheckbox.Enabled = enable;
            IfIsSinglelineCheckBox.Enabled = enable;
            TestIf.Enabled = enable;
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

        private void NameTextbox_KeyDown(object sender, KeyEventArgs e)
        {            
            if (e.Modifiers == Keys.Control)
            {
                if (e.KeyCode == Keys.A)
                {
                    NameTextbox.SelectAll();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
            }
        }

        private void TestIf_Click(object sender, EventArgs e)
        {
            contextMenu.Show(TestIf, 0, TestIf.Height);
        }

        private void TestFind_Click(object sender, EventArgs e)
        {
            RegexTester.Test(ParentForm, ReplaceTextbox, WithTextbox, ReplaceIsMultilineCheckBox,
                ReplaceIsSinglelineCheckbox, ReplaceIsCaseSensitiveCheckBox);
        }

        private void testIfContainsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegexTester.Test(ParentForm, IfContainsTextBox, null, IfIsMultilineCheckbox,
                IfIsSinglelineCheckBox, IfIsCaseSensitiveCheckBox);
        }

        private void testIfNotContainsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegexTester.Test(ParentForm, IfNotContainsTextBox, null, IfIsMultilineCheckbox,
                IfIsSinglelineCheckBox, IfIsCaseSensitiveCheckBox);
        }

        // event handlers for Ctrl+A / select all
        // as per http://msdn.microsoft.com/en-us/library/system.windows.forms.textboxbase.shortcutsenabled.aspx
        // The TextBox control does not support the CTRL+A shortcut key when the Multiline property value is true.
        private void ReplaceTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                ReplaceTextbox.SelectAll();
        }

        private void WithTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                WithTextbox.SelectAll();
        }

        private void IfContainsTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                IfContainsTextBox.SelectAll();
        }

        private void IfNotContainsTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                IfNotContainsTextBox.SelectAll();
        }
    }
}
