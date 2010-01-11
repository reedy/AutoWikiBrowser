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
        public const string XmlName = "rule";

        public enum T { OnWholePage = 0, InsideTemplate };

        public T ruletype_ = T.OnWholePage;
        public string replace_ = "";
        public string with_ = "";
        public bool regex_;
        public RegexOptions regexOptions_ = RegexOptions.None;
        public int numoftimes_ = 1;

        public string ifContains_ = "";
        public string ifNotContains_ = "";
        public bool ifIsRegex_;
        public RegexOptions ifRegexOptions_ = RegexOptions.None;

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
            RuleControl rc = new RuleControl(owner);
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

        static string ApplyOnce(TreeNode tn, string text, string title)
        {
            Rule r = (Rule)tn.Tag;

            if (r.ruletype_ == T.OnWholePage)
                return ApplyOn(tn, text, title);
            if (r.ruletype_ == T.InsideTemplate)
                return ApplyInsideTemplate(tn, text, title);

            return text;
        }

        class ParseTemplate
        {
            string text_ = "";
            string title_ = "";
            string result_ = "";

            public ParseTemplate(string text, string title)
            {
                text_ = text;
                title_ = title;
            }

            public string Result { get { return result_; } }

            public void Parse(TreeNode tn)
            {
                result_ = text_;
                foreach(Match m in Parsers.GetTemplates(text_, Parsers.EveryTemplate))
                {
                    if(CheckIf(tn, m.Value))
                    {
                        result_ = result_.Replace(m.Value, ApplyOn(tn, m.Value, title_));
                    }
                }
            }

            /*
            public void Parse(TreeNode tn)
            {
                for (; ; )
                {
                    int i = text_.IndexOf("{{");
                    if (i < 0)
                    {
                        result_ += text_;
                        return;
                    }

                    i += 2;
                    result_ += text_.Substring(0, i);

                    text_ = text_.Substring(i);
                    Inside(tn);
                }
            }

            void Inside(TreeNode tn)
            {
                bool check_done = false;
                bool check = true;
                string t = "";

                for (; ; )
                {
                    int i = text_.IndexOf("}}");
                    if (i < 0)
                        return; // error: template not closed

                    int j = text_.IndexOf("{{");

                    if (j != -1 && j < i)
                    {
                        t = text_.Substring(0, j);
                        j += 2;
                        text_ = text_.Substring(j);

                        if (!check_done)
                        {
                            check = CheckIf(tn, t);
                            check_done = true;
                        }

                        if (check)
                        {
                            result_ += ReplaceOn(tn, t, title_);
                        }
                        else
                        {
                            result_ += t;
                        }
                        result_ += "{{";
                        Inside(tn);
                        continue;
                    }

                    t = text_.Substring(0, i);
                    i += 2;
                    text_ = text_.Substring(i);

                    if (check_done)
                    {
                        if (check)
                            result_ += ReplaceOn(tn, t, title_);
                        else
                            result_ += t;
                    }
                    else
                        result_ += ApplyOn(tn, t, title_);

                    result_ += "}}";

                    return;

                }
            }*/
        }

        private static string ApplyInsideTemplate(TreeNode tn, string text, string title)
        {
            ParseTemplate p = new ParseTemplate(text, title);

            p.Parse(tn);

            return p.Result;
        }

        private static bool CheckIf(TreeNode tn, string text)
        {
            Rule r = (Rule)tn.Tag;

            StringComparison sc = (((int)r.ifRegexOptions_ & (int)RegexOptions.IgnoreCase) != 0)?StringComparison.OrdinalIgnoreCase:StringComparison.Ordinal;

            if (!string.IsNullOrEmpty(r.ifContains_))
            {
                if ((r.ifIsRegex_ && !Regex.IsMatch(text, r.ifContains_, r.ifRegexOptions_))
                    || (!r.ifIsRegex_ && text.IndexOf(r.ifContains_, sc)<0))
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

        private static string ReplaceOn(TreeNode tn, string text, string title)
        {
            Rule r = (Rule)tn.Tag;

            if (!string.IsNullOrEmpty(r.replace_))
            {
                string replace = Tools.ApplyKeyWords(title, r.replace_);

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

        private static string ApplyOn(TreeNode tn, string text, string title)
        {
            if (!CheckIf(tn, text))
                return text;

            return ReplaceOn(tn, text, title);
        }
    }
}
