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
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WikiFunctions.MWB
{    
    public class InTemplateRule : IRule
    {
        public const string XmlName = "InTemplateRule";

        public List<string> TemplateNames_ = new List<string>();
        public string ReplaceWith_ = "";
        public bool DoReplace_;

        InTemplateRuleControl ruleControl_;

        public override Object Clone()
        {
            InTemplateRule res = (InTemplateRule)MemberwiseClone();
            res.ruleControl_ = null;
            return res;
        }

        public InTemplateRule()
        {
            Name = "In Template Rule";
        }

        public override Control GetControl()
        {
            return ruleControl_;
        }

        public override void ForgetControl()
        {
            ruleControl_ = null;
        }

        public override Control CreateControl(
          IRuleControlOwner owner,
          Control.ControlCollection collection,
          System.Drawing.Point pos
        )
        {
            InTemplateRuleControl rc = new InTemplateRuleControl(owner);
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

            foreach (string template in TemplateNames_)
            {
                text = ApplyOnce(template, tn, text, title);
            }

            return text;
        }

        static string ApplyOnce(string template, TreeNode tn, string text, string title)
        {
            return ApplyInsideTemplate(template, tn, text, title);
        }


        class ParseTemplate
        {
            string template_ = "";
            string text_ = "";
            string title_ = "";
            string result_ = "";

            public ParseTemplate(string template, string text, string title)
            {
                template_ = template;
                text_ = text;
                title_ = title;
            }

            public string Result { get { return result_; } }

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
                            check = CheckIf(template_, tn, t);
                            check_done = true;
                        }

                        if (check)
                        {
                            result_ += ReplaceOn(template_, tn, t, title_);
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
                            result_ += ReplaceOn(template_, tn, t, title_);
                        else
                            result_ += t;
                    }
                    else
                        result_ += ApplyOn(template_, tn, t, title_);

                    result_ += "}}";

                    return;

                }
            }
        }

        static string ApplyInsideTemplate(string template, TreeNode tn, string text, string title)
        {
            ParseTemplate p = new ParseTemplate(template, text, title);

            p.Parse(tn);

            return p.Result;
        }
        
        static bool CheckIf(string template, TreeNode tn, string text)
        {
            if (string.IsNullOrEmpty(template))
                return true;

            string pattern =
              "^[\\s]*" + Tools.CaseInsensitive(template) + "[\\s]*(\\}\\}|\\|)";

            pattern = pattern.Replace(" ", "[ _]+");

            text = WikiRegexes.Comments.Replace(text, "");

            if (!Regex.IsMatch(text, pattern))
                return false;

            return true;
        }
        
        static string ReplaceOn(string template, TreeNode tn, string text, string title)
        {
            InTemplateRule r = (InTemplateRule)tn.Tag;

            foreach (TreeNode t in tn.Nodes)
            {
                IRule sr = (IRule)t.Tag;
                text = sr.Apply(t, text, title);
            }

            if (r.DoReplace_ && !string.IsNullOrEmpty(r.ReplaceWith_))
            {
                if (string.IsNullOrEmpty(template))
                    return text;

                string pattern =
                  @"^([\s]*)" + Tools.CaseInsensitive(template) + @"([\s]*(?:<!--.*-->)?[\s]*(\}\}|\|))";

                pattern = pattern.Replace(" ", "[ _]+");

                text = Regex.Replace(text, pattern, "$1" + r.ReplaceWith_ + "$2");
            }

            return text;
        }


        static string ApplyOn(string template, TreeNode tn, string text, string title)
        {
            if (!CheckIf(template, tn, text))
                return text;

            return ReplaceOn(template, tn, text, title);
        }
    }

}
