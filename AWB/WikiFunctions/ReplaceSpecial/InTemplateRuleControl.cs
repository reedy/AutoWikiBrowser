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

namespace WikiFunctions.ReplaceSpecial
{
    public partial class InTemplateRuleControl : UserControl
    {
        readonly IRuleControlOwner Owner;

        public InTemplateRuleControl(IRuleControlOwner owner)
        {
            InitializeComponent();

            Owner = owner;
            Anchor =
              AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            UpdateEndabledStates();
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

        public void SaveToRule(InTemplateRule r)
        {
            if (r == null)
                return;

            r.enabled_ = RuleEnabledCheckBox.Checked;
            r.SetName(NameTextbox.Text.Trim());
            r.ReplaceWith_ = ReplaceWithTextBox.Text.Trim();
            r.DoReplace_ = ReplaceCheckBox.Checked;

            r.TemplateNames_.Clear();
            foreach (string s in AliasesListBox.Items)
            {
                r.TemplateNames_.Add(s);
            }
        }

        public void RestoreFromRule(InTemplateRule r)
        {
            NameTextbox.Text = r.Name;
            RuleEnabledCheckBox.Checked = r.enabled_;
            ReplaceWithTextBox.Text = r.ReplaceWith_;
            ReplaceCheckBox.Checked = r.DoReplace_;

            AliasesListBox.BeginUpdate();
            AliasesListBox.Items.Clear();
            foreach (string s in r.TemplateNames_)
            {
                AliasesListBox.Items.Add(s);
            }
            AliasesListBox.EndUpdate();

            UpdateEndabledStates();
        }

        private void NameTextbox_TextChanged(object sender, EventArgs e)
        {
            Owner.NameChanged(this, NameTextbox.Text.Trim());
        }

        private void NameTextbox_DoubleClick(object sender, EventArgs e)
        {
            NameTextbox.SelectAll();
        }

        private void ReplaceCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEndabledStates();
        }

        void UpdateEndabledStates()
        {
            ReplaceWithTextBox.Enabled = ReplaceCheckBox.Checked;
            DeleteButton.Enabled = AliasesListBox.SelectedItem != null;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            string alias = AliasTextBox.Text;
            if (string.IsNullOrEmpty(alias))
                return;
            if (!AliasesListBox.Items.Contains(alias))
            {
                AliasesListBox.Items.Add(alias);
            }
            AliasTextBox.Text = "";
            AliasTextBox.Select();
            UpdateEndabledStates();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (AliasesListBox.SelectedItem == null)
                return;

            int i = AliasesListBox.SelectedIndex;

            AliasesListBox.Items.Remove(AliasesListBox.SelectedItem);

            int count = AliasesListBox.Items.Count;
            if (count != 0)
            {
                if (i > count - 1)
                    i = count - 1;
                AliasesListBox.SelectedIndex = i;
            }
            UpdateEndabledStates();
        }

        private void AliasesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEndabledStates();
        }

    }
}
