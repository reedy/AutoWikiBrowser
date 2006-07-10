//$Header: /cvsroot/mwiki-browser/main/mwiki-browser/RuleFactory.cs,v 1.4 2006/06/30 15:21:18 ligulem Exp $
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
using System.Windows.Forms;
using System.Xml;
using System.Text.RegularExpressions;

namespace WikiFunctions.MWB
{

    class RuleFactory
    {
        public static Rule CreateRule()
        {
            return new Rule();
        }

        public static InTemplateRule CreateInTemplateRule()
        {
            return new InTemplateRule();
        }

        public static TemplateParamRule CreateTemplateParamRule()
        {
            return new TemplateParamRule();
        }

        static public void ReadFromXml(TreeNodeCollection nodes, XmlTextReader rd)
        {
            while (!rd.EOF)
            {
                if (rd.NodeType == XmlNodeType.EndElement)
                    break;
                else if (rd.Name == Rule.XmlName)
                    Rule.ReadFromXml(nodes, rd, rd.IsEmptyElement);
                else if (rd.Name == InTemplateRule.XmlName)
                    InTemplateRule.ReadFromXml(nodes, rd, rd.IsEmptyElement);
                else if (rd.Name == TemplateParamRule.XmlName)
                    TemplateParamRule.ReadFromXml(nodes, rd, rd.IsEmptyElement);
                else
                    break;
                if (!rd.Read())
                    break;
            }
        }

        static public void ReadFromXmlAWB(TreeNodeCollection nodes, XmlTextReader rd)
        {
            bool regex = false;
            bool caseSensitive = false;
            bool multiline = false;
            bool singleline = false;

            if (rd.MoveToAttribute("regex"))
                regex = Convert.ToBoolean(rd.Value);
            if (rd.MoveToAttribute("casesensitive"))
                caseSensitive = Convert.ToBoolean(rd.Value);
            if (rd.MoveToAttribute("multiline"))
                multiline = Convert.ToBoolean(rd.Value);
            if (rd.MoveToAttribute("singleline"))
                singleline = Convert.ToBoolean(rd.Value);

            RegexOptions regexOptions = RegexOptions.None;
            if (!caseSensitive)
                regexOptions |= RegexOptions.IgnoreCase;
            if (multiline)
                regexOptions |= RegexOptions.Multiline;
            if (singleline)
                regexOptions |= RegexOptions.Singleline;

            int i = 1;
            while (rd.Read())
            {
                if (rd.Name != "FAR" && rd.Name != "datagridFAR")
                    break;

                Rule r = new Rule();

                r.Name = "Rule " + i.ToString();

                r.regex_ = regex;
                r.regexOptions_ = regexOptions;

                if (rd.MoveToAttribute("apply"))
                    r.numoftimes_ = Convert.ToInt32(rd.Value);

                if (rd.MoveToAttribute("find"))
                    r.replace_ = rd.Value;

                if (rd.MoveToAttribute("replacewith"))
                    r.with_ = rd.Value;

                TreeNode tn = new TreeNode(r.Name);
                tn.Tag = r;

                nodes.Add(tn);

                ++i;
            }
        }

    }

}
