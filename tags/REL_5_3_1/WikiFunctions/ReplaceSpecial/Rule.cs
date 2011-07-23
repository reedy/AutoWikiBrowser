/*
    Derived from Autowikibrowser
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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WikiFunctions.Parse;

namespace WikiFunctions.ReplaceSpecial
{
    public class Rule : IRule
    {
        public enum T { OnWholePage = 0, InsideTemplate };

        public T ruletype_ = T.OnWholePage;

        public string replace_ = "", with_ = "", ifContains_ = "", ifNotContains_ = "";

        public bool regex_, ifIsRegex_;

        public int numoftimes_ = 1;

        public RegexOptions ifRegexOptions_ = RegexOptions.None, regexOptions_ = RegexOptions.None;

        RuleControl ruleControl_;

        public override Object Clone()
        {
            Rule res = (Rule)MemberwiseClone();
            res.ruleControl_ = null;
            return res;
        }

        public Rule()
        {
            Name = "Rule";
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
            RuleControl rc = new RuleControl(owner) {Location = pos};
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
            if (string.IsNullOrEmpty(text) || !enabled_)
                return text;

            int apply = numoftimes_;

            if (apply > 100)
                apply = 100;
            if (apply <= 0)
                return text;

            for (int j = 0; j != apply; ++j)
            {
                text = ApplyOnce(tn, text, title);
            }
            return text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tn"></param>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        static string ApplyOnce(TreeNode tn, string text, string title)
        {
            Rule r = (Rule)tn.Tag;

            if (r.ruletype_ == T.OnWholePage)
                return ApplyOn(tn, text, title);
            
            return (r.ruletype_ == T.InsideTemplate) ? ApplyInsideTemplate(tn, text, title) : text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tn"></param>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private static string ApplyInsideTemplate(TreeNode tn, string text, string title)
        {
            string result = text;
            foreach (Match m in Parsers.GetTemplates(text))
            {
                if (CheckIf(tn, m.Value))
                {
                    result = result.Replace(m.Value, ApplyOn(tn, m.Value, title));
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tn"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private static bool CheckIf(TreeNode tn, string text)
        {
            Rule r = (Rule) tn.Tag;

            StringComparison sc = (((int) r.ifRegexOptions_ & (int) RegexOptions.IgnoreCase) != 0)
                                      ? StringComparison.OrdinalIgnoreCase
                                      : StringComparison.Ordinal;

            if (!string.IsNullOrEmpty(r.ifContains_))
            {
                if ((r.ifIsRegex_ && !Regex.IsMatch(text, r.ifContains_, r.ifRegexOptions_))
                    || (!r.ifIsRegex_ && text.IndexOf(r.ifContains_, sc) < 0))
                    return false;
            }
            if (!string.IsNullOrEmpty(r.ifNotContains_))
            {
                if ((r.ifIsRegex_ && Regex.IsMatch(text, r.ifNotContains_, r.ifRegexOptions_))
                    || (!r.ifIsRegex_ && text.IndexOf(r.ifNotContains_, sc) >= 0))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tn"></param>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private static string ReplaceOn(TreeNode tn, string text, string title)
        {
            Rule r = (Rule)tn.Tag;

            if (!string.IsNullOrEmpty(r.replace_))
            {
                string replace = Tools.ApplyKeyWords(title, r.replace_, true);

                string with = Tools.ApplyKeyWords(title, r.with_);

                if (!r.regex_)
                    replace = Regex.Escape(replace);

                //stop \r\n being interpreted literally
                with = with.Replace(@"\r", "\r").Replace(@"\n", "\n");

                text = Regex.Replace(text, replace, with, r.regexOptions_);
            }

            foreach (TreeNode t in tn.Nodes)
            {
                IRule sr = (IRule)t.Tag;
                text = sr.Apply(t, text, title);
            }

            return text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tn"></param>
        /// <param name="text"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        private static string ApplyOn(TreeNode tn, string text, string title)
        {
            return !CheckIf(tn, text) ? text : ReplaceOn(tn, text, title);
        }
    }
}
