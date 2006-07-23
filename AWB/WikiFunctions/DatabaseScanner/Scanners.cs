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

namespace WikiFunctions.DatabaseScanner
{
    class Scanners
    {
        Parsers parsers = new Parsers();
        bool skip = true;

        string articleText = "";
        string articleTitle = "";

        Regex regComments = new Regex("&lt;!--.*?--&gt;", RegexOptions.Singleline | RegexOptions.Compiled);

        public bool Test(ref string Text, ref string Title)
        {
            articleText = Text;
            articleTitle = Title;

            if (ignorecomments)
                articleText = regComments.Replace(articleText, "");

            if (IgnoreName() && countLinks() && checkLength() && checkDoesContain() && checkDoesNotContain() && simpleLinks() && boldTitle() && badLinks() && containsHTML() && sectionHeaderError() && BulletExternalLinks())// && noBirthCat(articleText
                return true;
            else
                return false;
        }

        bool ignorecomments = false;
        public bool IgnoreComments
        {
            get { return ignorecomments; }
            set { ignorecomments = value; }
        }

        int checklength = 0;
        public int CheckLength
        {
            get { return checklength; }
            set { checklength = value; }
        }
        int length = 0;
        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        private bool checkLength()
        {
            if (checklength == 0)
                return true;
            else if (checklength == 1 && articleText.Length > length)
                return true;
            else if (checklength == 2 && articleText.Length < length)
                return true;
            else
                return false;
        }

        int countlinks = 0;
        public int CountLinks
        {
            get { return countlinks; }
            set { countlinks = value; }
        }
        int links = 0;
        public int Links
        {
            get { return links; }
            set { links = value; }
        }

        int intLinks = 0;
        readonly Regex Regexlinks = new Regex("\\[\\[", RegexOptions.Compiled);
        private bool countLinks()
        {
            if (countlinks == 0)
                return true;
            else
            {
                intLinks = 0;
                foreach (Match m in Regexlinks.Matches(articleText)) intLinks++;

                if (countlinks == 1 && intLinks > links)
                    return true;
                else if (countlinks == 2 && intLinks < links)
                    return true;
                else
                    return false;
            }
        }

        Regex doescontainregex = new Regex("");
        public Regex ArticleDoesContainRegex
        {
            get { return doescontainregex; }
            set { doescontainregex = value; }
        }
        bool doescontain = false;
        public bool ArticleDoesContain
        {
            get { return doescontain; }
            set { doescontain = value; }
        }
        private bool checkDoesContain()
        {
            if (!doescontain)
                return true;

            if (doescontainregex.IsMatch(articleText))
                return true;
            else
                return false;
        }

        Regex doesnotcontainregex = new Regex("");
        public Regex ArticleDoesNotContainRegex
        {
            get { return doesnotcontainregex; }
            set { doesnotcontainregex = value; }
        }
        bool doesnotcontain = false;
        public bool ArticleDoesNotContain
        {
            get { return doesnotcontain; }
            set { doesnotcontain = value; }
        }
        private bool checkDoesNotContain()
        {
            if (!doesnotcontain)
                return true;

            if (doesnotcontainregex.IsMatch(articleText))
                return false;
            else
                return true;
        }

        bool ignoreredirects = false;
        public bool IgnoreRedirects
        {
            get { return ignoreredirects; }
            set { ignoreredirects = value; }
        }

        //namespaces to ignore
        List<int> namespaces = new List<int>();
        public List<int> Namespaces
        {
            get { return namespaces; }
            set { namespaces = value; }

        }

        int NamespaceIndex = 0;
        int i = 0;
        private bool IgnoreName()
        {
            if (ignoreredirects && articleText.StartsWith("#"))
                return false;

            NamespaceIndex = Tools.CalculateNS(articleTitle);

            i = 0;      
            foreach (int j in namespaces)
            {
                if (NamespaceIndex == namespaces[i])
                    return false;
                i++;
            }

            if (TitleContains() && TitleNotContains())
                return true;
            else
                return false;
        }

        Regex titlecontainsregex = new Regex("");
        public Regex TitleContainsRegex
        {
            get { return titlecontainsregex; }
            set { titlecontainsregex = value; }
        }
        bool titlecontainsenabled = false;
        public bool TitleContainsEnabled
        {
            get { return titlecontainsenabled; }
            set { titlecontainsenabled = value; }
        }
        private bool TitleContains()
        {
            if (!titlecontainsenabled)
                return true;

            if (titlecontainsregex.IsMatch(articleTitle))
                return true;
            else
                return false;
        }

        Regex titlenotcontainsregex = new Regex("");
        public Regex TitleNotContainsRegex
        {
            get { return titlenotcontainsregex; }
            set { titlenotcontainsregex = value; }
        }
        bool titlenotcontainsenabled = false;
        public bool TitleNotContainsEnabled
        {
            get { return titlenotcontainsenabled; }
            set { titlenotcontainsenabled = value; }
        }
        private bool TitleNotContains()
        {
            if (!titlenotcontainsenabled)
                return true;

            if (!titlenotcontainsregex.IsMatch(articleTitle))
                return true;
            else
                return false;
        }

        bool simplelinks = false;
        public bool SimpleLinks
        {
            get { return simplelinks; }
            set { simplelinks = value; }
        }
        Regex RegexSimpleLinks = new Regex("\\[\\[(.*?)\\|(.*?)\\]\\]", RegexOptions.Compiled);
        private bool simpleLinks()
        {
            if (!simplelinks)
                return true;
            string n = "";
            string a = "";
            string b = "";

            foreach (Match m in RegexSimpleLinks.Matches(articleText))
            {
                n = m.Value;
                a = m.Groups[1].Value;
                b = m.Groups[2].Value;

                if (a == b || Tools.TurnFirstToLower(a) == b)
                {
                    return true;
                }
                else if (a + "s" == b || Tools.TurnFirstToLower(a) + "s" == b)
                {
                    return true;
                }
            }

            return false;
        }

        bool nobirthcat = false;
        public bool NoBirthCat
        {
            get { return nobirthcat; }
            set { nobirthcat = value; }
        }
        private bool noBirthCat()
        {
            if (!nobirthcat)
                return true;

            if (Regex.IsMatch(articleText, "\\[\\[[Cc]ategory: ?[0-9]{3,4} births"))
                return false;

            string articleTextTemp = articleText;

            if (articleTextTemp.Length > 80)
                articleTextTemp = articleTextTemp.Substring(0, 80);

            if (Regex.IsMatch(articleTextTemp, "(\\(| )[Bb]orn "))
                return true;

            return false;
        }

        bool boldtitle = false;
        public bool NoBold
        {
            get { return boldtitle; }
            set { boldtitle = value; }
        }
        private bool boldTitle()
        {
            if (!boldtitle)
                return true;

            parsers.BoldTitle(articleText, articleTitle, ref skip);

            return !skip;
        }

        bool badlinks = false;
        public bool BadLinks
        {
            get { return badlinks; }
            set { badlinks = value; }
        }
        private bool badLinks()
        {
            if (!badlinks)
                return true;

            parsers.LinkFixer(articleText, ref skip);

            return !skip;
        }

        bool containshtml = false;
        public bool HasHTML
        {
            get { return containshtml; }
            set { containshtml = value; }
        }
        private bool containsHTML()
        {
            if (!containshtml)
                return true;

            parsers.Unicodify(articleText, ref skip);

            return !skip;
        }

        bool headererror = false;
        public bool HeaderError
        {
            get { return headererror; }
            set { headererror = value; }
        }
        private bool sectionHeaderError()
        {
            if (!headererror)
                return true;

            if (!Regex.IsMatch(articleText, "= ?See also ?=") && Regex.IsMatch(articleText, "(== ?)([Ss]ee Also:?|[rR]elated [tT]opics:?|[rR]elated [aA]rticles:?|[Ii]nternal [lL]inks:?|[Aa]lso [Ss]ee:?)( ?==)"))
                return true;

            parsers.FixHeadings(articleText, ref skip);

            return !skip;
        }

        bool bulletexternal = false;
        public bool UnbulletedLinks
        {
            get { return bulletexternal; }
            set { bulletexternal = value; }
        }

        Regex bulletRegex = new Regex(@"External [Ll]inks? ? ?={1,4} ? ?(
){0,3}\[?http", RegexOptions.Compiled);
        private bool BulletExternalLinks()
        {
            if (!bulletexternal)
                return true;

            if (bulletRegex.IsMatch(articleText))
                return true;
            else
                return false;
        }
    }
}
