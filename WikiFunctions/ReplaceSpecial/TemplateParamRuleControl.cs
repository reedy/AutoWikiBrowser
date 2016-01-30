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
    public partial class TemplateParamRuleControl : UserControl
    {
        readonly IRuleControlOwner owner_;

        public TemplateParamRuleControl(IRuleControlOwner owner)
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

        public void SaveToRule(TemplateParamRule rule)
        {
            if (rule == null)
                return;

            rule.enabled_ = RuleEnabledCheckBox.Checked;
            rule.Name = NameTextbox.Text.Trim();
            rule.ParamName = ParamNameTextBox.Text.Trim();
            rule.NewParamName = ChangeNameToTextBox.Text.Trim();
        }

        public void RestoreFromRule(TemplateParamRule rule)
        {
            if (rule == null)
                return;

            RuleEnabledCheckBox.Checked = rule.enabled_;
            NameTextbox.Text = rule.Name;
            ParamNameTextBox.Text = rule.ParamName;
            ChangeNameToTextBox.Text = rule.NewParamName;
        }

        private void NameTextbox_TextChanged(object sender, EventArgs e)
        {
            owner_.NameChanged(this, NameTextbox.Text.Trim());
        }
    }
}
