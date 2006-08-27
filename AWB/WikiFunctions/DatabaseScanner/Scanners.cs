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
using WikiFunctions.Parse;

namespace WikiFunctions.DatabaseScanner
{
    public abstract class Scan
    {
        public virtual bool Check(ref string articleText, ref string articleTitle)
        {
            return true;
        }
    }

    public class IsNotRedirect : Scan
    {
        public override bool Check(ref string articleText, ref string articleTitle)
        {
            if (Tools.IsRedirect(articleText))
                return false;
            else
                return true;
        }
    }

    public class TextDoesContain : Scan
    {
        public TextDoesContain(params Regex[] ContainsR)
        {
            Contains = ContainsR;
        }

        Regex[] Contains;

        public override bool Check(ref string articleText, ref string articleTitle)
        {
            foreach (Regex r in Contains)
            {
                if (!r.IsMatch(articleText))
                    return false;
            }

            return true;
        }
    }

    public class TextDoesNotContain : Scan
    {
        public TextDoesNotContain(params Regex[] NotContainsR)
        {
            NotContains = NotContainsR;
        }

        Regex[] NotContains;

        public override bool Check(ref string articleText, ref string articleTitle)
        {
            foreach (Regex r in NotContains)
            {
                if (r.IsMatch(articleText))
                    return false;                   
            }

            return true;
        }
    }

    public class TitleDoesContain : Scan
    {
        public TitleDoesContain(Regex ContainsR)
        {
            Contains = ContainsR;
        }

        Regex Contains;

        public override bool Check(ref string articleText, ref string articleTitle)
        {
            if (Contains.IsMatch(articleTitle))
                return true;
            else
                return false;
        }
    }

    public class TitleDoesNotContain : Scan
    {
        public TitleDoesNotContain(Regex NotContainsR)
        {
            NotContains = NotContainsR;
        }

        Regex NotContains;

        public override bool Check(ref string articleText, ref string articleTitle)
        {
            if (NotContains.IsMatch(articleTitle))
                return false;
            else
                return true;
        }
    }

    public enum MoreLessThan : byte { LessThan, MoreThan, EqualTo }

    public class CountCharacters : Scan
    {
        public CountCharacters(MoreLessThan Value, int Characters)
        {
            M = Value;
            test = Characters;
        }

        MoreLessThan M;
        int test = 0;
        int actual = 0;

        public override bool Check(ref string articleText, ref string articleTitle)
        {
            actual = articleText.Length;

            if (M == MoreLessThan.MoreThan)
            {
                if (actual > test)
                    return true;
                else
                    return false;
            }
            else if (M == MoreLessThan.LessThan)
            {
                if (actual < test)
                    return true;
                else
                    return false;
            }
            else
            {
                if (actual == test)
                    return true;
                else
                    return false;
            }
        }
    }

    public class CountLinks : Scan
    {
        public CountLinks(MoreLessThan Value, int Links)
        {
            M = Value;
            test = Links;
        }

        MoreLessThan M;
        int test = 0;
        int actual = 0;
        readonly Regex Regexlinks = new Regex("\\[\\[", RegexOptions.Compiled);

        public override bool Check(ref string articleText, ref string articleTitle)
        {
            MatchCollection m = Regexlinks.Matches(articleText);
            actual = m.Count;

            if (M == MoreLessThan.MoreThan)
            {
                if (actual > test)
                    return true;
                else
                    return false;
            }
            else if (M == MoreLessThan.LessThan)
            {
                if (actual < test)
                    return true;
                else
                    return false;
            }
            else
            {
                if (actual == test)
                    return true;
                else
                    return false;
            }
        }
    }

    public class CountWords : Scan
    {
        public CountWords(MoreLessThan Value, int Words)
        {
            M = Value;
            test = Words;
        }

        MoreLessThan M;
        int test = 0;
        int actual = 0;

        public override bool Check(ref string articleText, ref string articleTitle)
        {
            actual = Tools.WordCount(articleText);

            if (M == MoreLessThan.MoreThan)
            {
                if (actual > test)
                    return true;
                else
                    return false;
            }
            else if (M == MoreLessThan.LessThan)
            {
                if (actual < test)
                    return true;
                else
                    return false;
            }
            else
            {
                if (actual == test)
                    return true;
                else
                    return false;
            }
        }
    }

    public class CheckNamespace : Scan
    {
        public CheckNamespace(List<int> NameSpaces)
        {
            namespaces = NameSpaces;
        }

        List<int> namespaces = new List<int>();
        int i = 0;
        int NamespaceIndex = 0;

        public override bool Check(ref string articleText, ref string articleTitle)
        {
            i = 0;
            NamespaceIndex = Tools.CalculateNS(articleTitle);
            foreach (int j in namespaces)
            {
                if (NamespaceIndex == namespaces[i])
                    return false;
                i++;
            }

            return true;
        }
    }

    public class HasBadLinks : Scan
    {
        public HasBadLinks(Parsers p)
        {
            parsers = p;
        }

        bool skip = true;
        Parsers parsers;
        public override bool Check(ref string articleText, ref string articleTitle)
        {
            parsers.FixLinks(articleText, ref skip);
            return !skip;
        }
    }

    public class HasNoBoldTitle : Scan
    {
        public HasNoBoldTitle(Parsers p)
        {
            parsers = p;
        }

        bool skip = true;
        Parsers parsers;
        public override bool Check(ref string articleText, ref string articleTitle)
        {
            parsers.BoldTitle(articleText, articleTitle, ref skip);

            return !skip;
        }
    }

    public class HasHTMLEntities : Scan
    {
        public HasHTMLEntities(Parsers p)
        {
            parsers = p;
        }

        bool skip = true;
        Parsers parsers;
        public override bool Check(ref string articleText, ref string articleTitle)
        {
            parsers.Unicodify(articleText, ref skip);

            return !skip;
        }
    }

    public class HasSimpleLinks : Scan
    {
        public HasSimpleLinks(Parsers p)
        {
            parsers = p;
        }

        Parsers parsers;
        Regex RegexSimpleLinks = new Regex("\\[\\[([^[]*?)\\|([^[]*?)\\]\\]", RegexOptions.Compiled);
        public override bool Check(ref string articleText, ref string articleTitle)
        {
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
    }

    public class HasSectionError : Scan
    {
        public HasSectionError(Parsers p)
        {
            parsers = p;
        }

        bool skip = true;
        Parsers parsers;
        public override bool Check(ref string articleText, ref string articleTitle)
        {
            parsers.FixHeadings(articleText, ref skip);

            return !skip;
        }
    }

    public class HasUnbulletedLinks : Scan
    {
        public HasUnbulletedLinks(Parsers p)
        {
            parsers = p;
        }

        Regex bulletRegex = new Regex(@"External [Ll]inks? ? ?={1,4} ? ?(
){0,3}\[?http", RegexOptions.Compiled);

        Parsers parsers;
        public override bool Check(ref string articleText, ref string articleTitle)
        {
            if (bulletRegex.IsMatch(articleText))
                return true;
            else
                return false;
        }
    }

    public class LivingPerson : Scan
    {
        public LivingPerson(Parsers p)
        {
            parsers = p;
        }

        bool skip = true;
        Parsers parsers;
        public override bool Check(ref string articleText, ref string articleTitle)
        {
            parsers.LivingPeople(articleText, ref skip);

            return !skip;
        }
    }

    public class Typo : Scan
    {
        public Typo()
        {            
        }

        RegExTypoFix retf = new RegExTypoFix();
        bool skip = true;
        string a = "";

        public override bool Check(ref string articleText, ref string articleTitle)
        {
            retf.PerformTypoFixes(articleText, ref skip, ref a);
            return !skip;
        }
    }

}
