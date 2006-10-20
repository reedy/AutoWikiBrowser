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

    }

}
