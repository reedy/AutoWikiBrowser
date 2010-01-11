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

namespace WikiFunctions.MWB
{
    public class TemplateParamRule : IRule
    {
        public const string XmlName = "TemplateParamRule";

        public string ParamName_ = "";
        public string NewParamName_ = "";

        TemplateParamRuleControl ruleControl_;

        public override Object Clone()
        {
            TemplateParamRule res = (TemplateParamRule)MemberwiseClone();
            res.ruleControl_ = null;
            return res;
        }

        public TemplateParamRule()
        {
            Name = "Template Parameter Rule";
        }

        public override Control GetControl()
        {
            return ruleControl_;
        }

        public override void ForgetControl()
        {
            ruleControl_ = null;
        }

        public override Control CreateControl(IRuleControlOwner owner, Control.ControlCollection collection, System.Drawing.Point pos)
        {
            TemplateParamRuleControl rc = new TemplateParamRuleControl(owner);
            rc.Location = pos;
            rc.RestoreFromRule(this);
            DisposeControl();
            ruleControl_ = rc;
            collection.Add(rc);
            return rc;
        }

        public override void Save()
        {
            if (ruleControl_ == null)
                return;
            ruleControl_.SaveToRule(this);
        }

        public override void Restore()
        {
            if (ruleControl_ == null)
                return;
            ruleControl_.RestoreFromRule(this);
        }

        public override void SelectName()
        {
            if (ruleControl_ == null)
                return;
            ruleControl_.SelectName();
        }

        public override string Apply(TreeNode tn, string text, string title)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            if (!enabled_)
                return text;

            string pattern = "(\\|[\\s]*)" + ParamName_ + "([\\s]*=)";

            text = Regex.Replace(text, pattern, "$1" + NewParamName_ + "$2");

            foreach (TreeNode t in tn.Nodes)
            {
                IRule sr = (IRule)t.Tag;
                text = sr.Apply(t, text, title);
            }

            return text;
        }
    }
}