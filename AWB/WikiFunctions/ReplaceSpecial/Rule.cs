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
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WikiFunctions.Parse;


namespace WikiFunctions.MWB
{
    public class Rule : IRule
    {
        public const string XmlName = "rule";

        public enum T { OnWholePage = 0, InsideTemplate };

        public T ruletype_ = T.OnWholePage;
        public string replace_ = "";
        public string with_ = "";
        public bool regex_ = false;
        public RegexOptions regexOptions_ = RegexOptions.None;
        public int numoftimes_ = 1;

        public string ifContains_ = "";
        public string ifNotContains_ = "";
        public bool ifIsRegex_ = false;
        public RegexOptions ifRegexOptions_ = RegexOptions.None;


        RuleControl ruleControl_ = null;

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

        public override Control CreateControl(
          IRuleControlOwner owner,
          Control.ControlCollection collection,
          System.Drawing.Point pos
        )
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
            if (text == null || text == "")
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
            else if (r.ruletype_ == T.InsideTemplate)
                return ApplyInsideTemplate(tn, text, title);
            else
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

        static string ApplyInsideTemplate(TreeNode tn, string text, string title)
        {
            ParseTemplate p = new ParseTemplate(text, title);

            p.Parse(tn);

            return p.Result;
        }



        static bool CheckIf(TreeNode tn, string text)
        {
            Rule r = (Rule)tn.Tag;

            if (r.ifContains_ != "")
            {
                if (!Regex.IsMatch(text, r.ifContains_, r.ifRegexOptions_))
                    return false;
            }
            if (r.ifNotContains_ != "")
            {
                if (Regex.IsMatch(text, r.ifNotContains_, r.ifRegexOptions_))
                    return false;
            }

            return true;
        }


        static string ReplaceOn(TreeNode tn, string text, string title)
        {
            Rule r = (Rule)tn.Tag;

            //if (r.replace_ != null && r.with_ != null && r.replace_ != "" && r.with_ != "")
            if (r.replace_ != null && r.replace_ != "")
            {
                string replace = r.replace_.Replace("%%title%%", title).Replace("%%key%%", Tools.MakeHumanCatKey(title));

                string with = r.with_.Replace("%%title%%", title).Replace("%%key%%", Tools.MakeHumanCatKey(title));

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


        static string ApplyOn(TreeNode tn, string text, string title)
        {
            if (!CheckIf(tn, text))
                return text;

            return ReplaceOn(tn, text, title);
        }

        static public void ReadFromXml(TreeNodeCollection nodes, XmlTextReader rd, bool is_empty)
        {
            string name = "missing name";

            if (rd.MoveToAttribute("name"))
                name = rd.Value;

            Rule r = new Rule();
            r.Name = name;
            TreeNode tn = new TreeNode(name);
            tn.Tag = r;
            nodes.Add(tn);

            if (rd.MoveToAttribute("type"))
                r.ruletype_ = (Rule.T)Convert.ToInt32(rd.Value);
            if (rd.MoveToAttribute("enabled"))
                r.enabled_ = Convert.ToBoolean(rd.Value);
            if (rd.MoveToAttribute("times"))
                r.numoftimes_ = Convert.ToInt32(rd.Value);

            if (is_empty)
                return;

            rd.Read();
            while (!rd.EOF)
            {
                if (rd.NodeType == XmlNodeType.EndElement)
                    break;
                else if (rd.Name == RegexXml.Name)
                {
                    r.regex_ = true;
                    r.regexOptions_ = RegexXml.Read(rd);
                }
                else if (rd.Name == "replace")
                {
                    if (!rd.IsEmptyElement)
                    {
                        r.replace_ = Utility.ReadAllElementContent(ref rd);
                        continue;
                    }
                }
                else if (rd.Name == "with")
                {
                    if (!rd.IsEmptyElement)
                    {
                        r.with_ = Utility.ReadAllElementContent(ref rd);
                        continue;
                    }
                }
                else if (rd.Name == "if")
                {
                    if (!rd.IsEmptyElement)
                    {
                        r.ReadIfFromXml(rd);
                        continue;
                    }
                }
                else
                {
                    if (!rd.IsEmptyElement)
                    {
                        RuleFactory.ReadFromXml(tn.Nodes, rd);
                        continue;
                    }
                }
                rd.Read();
            }
        }

        void ReadIfFromXml(XmlTextReader rd)
        {
            if (rd.Name != "if" || rd.IsEmptyElement)
                return;

            bool ok = rd.Read();
            for (; ok; )
            {
                if (rd.Name == "if" && rd.NodeType == XmlNodeType.EndElement)
                {
                    ok = rd.Read();
                    return;
                }
                else if (rd.Name == RegexXml.Name)
                {
                    this.ifIsRegex_ = true;
                    this.ifRegexOptions_ = RegexXml.Read(rd);
                }
                else if (rd.Name == "contains")
                {
                    if (!rd.IsEmptyElement)
                    {
                        this.ifContains_ = rd.ReadElementContentAsString();
                        continue;
                    }
                }
                else if (rd.Name == "notcontains")
                {
                    if (!rd.IsEmptyElement)
                    {
                        this.ifNotContains_ = rd.ReadElementContentAsString();
                        continue;
                    }
                }
                else
                    break;
                ok = rd.Read();
            }
        }

    }

}
