/*
DumpSearcher
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
using System.Text.RegularExpressions;

namespace WikiFunctions.DumpSearcher
{
    class Scanners
    {
        Parsers parsers = new Parsers();
        bool skip = true;

        string articleText = "";
        string articleTitle = "";

        public bool Test(string Text, string Title)
        {
            articleText = Text;
            articleTitle = Title;

            if (IgnoreName() && countLinks() && checkLength() && checkDoesContain() && checkDoesNotContain() && simpleLinks() && boldTitle() && badLinks() && containsHTML() && sectionHeaderError() && BulletExternalLinks())// && noBirthCat(articleText
                return true;
            else
                return false;
        }

        int checklength = 0;
        public int CheckLength
        {
            get { return checklength; }
            set { checklength = value; }
        }
        public bool checkLength()
        {
            if (checklength == 0)
                return true;
            else if (checklength == 1 && articleText.Length > numLength.Value)
                return true;
            else if (checklength == 2 && articleText.Length < numLength.Value)
                return true;
            else
                return false;
        }

        int countlinks = false;
        public int CountLinks
        {
            get { return countlinks; }
            set { countlinks = value; }
        }
        int intLinks = 0;
        readonly Regex Regexlinks = new Regex("\\[\\[", RegexOptions.Compiled);
        public bool countLinks()
        {
            if (countlinks == 0)
                return true;
            else
            {
                intLinks = 0;
                foreach (Match m in Regexlinks.Matches(articleText)) intLinks++;

                if (countlinks == 1 && intLinks > nudLinks.Value)
                    return true;
                else if (countlinks == 2 && intLinks < nudLinks.Value)
                    return true;
                else
                    return false;
            }
        }

        bool doescontain = false;
        public bool DoesContain
        {
            get { return doescontain; }
            set { doescontain = value; }
        }
        public bool checkDoesContain()
        {
            if (!doescontain)
                return true;

            if (PRegex.IsMatch(articleText))
                return true;
            else
                return false;
        }

        bool doesnotcontain = false;
        public bool DoesNotContain
        {
            get { return doesnotcontain; }
            set { doesnotcontain = value; }
        }
        public bool checkDoesNotContain()
        {
            if (!doesnotcontain)
                return true;

            if (PNRegex.IsMatch(articleText))
                return false;
            else
                return true;
        }

        public bool IgnoreName()
        {
            if (ignoreRedirectsToolStripMenuItem1.Checked && articleText.StartsWith("#"))
                return false;
            else if (articleTitle.StartsWith(Variables.Namespaces[8]) || articleTitle.StartsWith(Variables.Namespaces[100])) //skip this namespace
                return false;
            else if (ignoreDisambigsToolStripMenuItem.Checked && articleText.Contains("isambig}}"))
                return false;
            else if (ignoreImagesToolStripMenuItem.Checked && articleTitle.StartsWith(Variables.Namespaces[6]))
                return false;
            else if (articleTitle.StartsWith(Variables.Namespaces[12]))
                return false;
            else if (ignoreCategoryNamespaceToolStripMenuItem.Checked && articleTitle.StartsWith(Variables.Namespaces[14]))
                return false;
            else if (ignoreTemplateNamespaceToolStripMenuItem.Checked && articleTitle.StartsWith(Variables.Namespaces[10]))
                return false;
            else if (ignoreWikipediaNamespaceToolStripMenuItem.Checked && articleTitle.StartsWith(Variables.Namespaces[4]))
                return false;
            else if (ignoreMainNamespaceToolStripMenuItem.Checked && Tools.IsMainSpace(articleTitle))
                return false;
            else
            {
                if (TitleContains() && TitleNotContains())
                    return true;
                else
                    return false;
            }
        }

        public bool TitleContains()
        {
            if (!chkCheckTitle.Checked)
                return true;

            else if (chkTitleRegex.Checked)
            {
                if (Regex.IsMatch(articleTitle, strTitle))
                    return true;
                else
                    return false;
            }
            else
            {
                if (articleTitle.Contains(strTitle))
                    return true;
                else
                    return false;
            }
        }

        public bool TitleNotContains()
        {
            if (!chkCheckNotInTitle.Checked)
                return true;

            else if (chkTitleRegex.Checked)
            {
                if (!Regex.IsMatch(articleTitle, strTitleNot))
                    return true;
                else
                    return false;
            }
            else
            {
                if (!articleTitle.Contains(strTitleNot))
                    return true;
                else
                    return false;
            }
        }
        

        Regex RegexSimpleLinks = new Regex("\\[\\[(.*?)\\|(.*?)\\]\\]", RegexOptions.Compiled);
        public bool simpleLinks()
        {
            if (!chkSimpleLinks.Checked)
                return true;
            string n = "";
            string a = "";
            string b = "";

            foreach (Match m in RegexSimpleLinks.Matches(articleText))
            {
                n = m.Value;
                a = m.Groups[1].Value;
                b = m.Groups[2].Value;

                if (a == b ||Tools.TurnFirstToLower(a) == b)
                {
                    return true;
                }
                else if (a + "s" == b || TurnFirstToLower(a) + "s" == b)
                {
                    return true;
                }
            }

            return false;
        }

        public bool noBirthCat()
        {
            if (Regex.IsMatch(articleText, "\\[\\[[Cc]ategory: ?[0-9]{3,4} births"))
                return false;

            string articleTextTemp = articleText;

            if (articleTextTemp.Length > 80)
                articleTextTemp = articleTextTemp.Substring(0, 80);

            if (Regex.IsMatch(articleTextTemp, "(\\(| )[Bb]orn "))
                return true;

            return false;
        }


        public bool boldTitle()
        {
            if (!chkNoBold.Checked)
                return true;

            parsers.BoldTitle(articleText, articleTitle, ref skip);

            return !skip;
        }

        public bool badLinks()
        {
            if (!chkBadLinks.Checked)
                return true;

            parsers.LinkFixer(articleText, ref skip);

            return !skip;
        }

        public bool containsHTML()
        {
            if (!chkHTML.Checked)
                return true;

            parsers.Unicodify(articleText, ref skip);

            return !skip;
        }

        public bool sectionHeaderError()
        {
            if (!chkSectionError.Checked)
                return true;

            if (!Regex.IsMatch(articleText, "= ?See also ?=") && Regex.IsMatch(articleText, "(== ?)([Ss]ee Also:?|[rR]elated [tT]opics:?|[rR]elated [aA]rticles:?|[Ii]nternal [lL]inks:?|[Aa]lso [Ss]ee:?)( ?==)"))
                return true;

            parsers.FixHeadings(articleText, ref skip);

            return !skip;
        }

        Regex bulletRegex = new Regex(@"External [Ll]inks? ? ?={1,4} ? ?(
){0,3}\[?http", RegexOptions.Compiled);
        public bool BulletExternalLinks()
        {
            if (!chkUnbulletedLinks.Checked)
                return true;

            if (bulletRegex.IsMatch(articleText))
                return true;
            else
                return false;
        }

        public bool BulletExternal()
        {
            parsers.BulletExternalLinks(articleText, ref skip);

            return !skip;
        }

    }
}
