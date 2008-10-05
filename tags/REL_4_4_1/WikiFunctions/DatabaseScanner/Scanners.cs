/*
DumpSearcher
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
using System.Text.RegularExpressions;
using WikiFunctions.Parse;

namespace WikiFunctions.DBScanner
{
    /// <summary>
    /// Abstract base class for Scan objects
    /// </summary>
    public abstract class Scan
    {
        public virtual bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            return true;
        }
    }

    /// <summary>
    /// Returns whether the article is not a redirect
    /// </summary>
    public class IsNotRedirect : Scan
    {
        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            return (!Tools.IsRedirect(ArticleText));
        }
    }

    public class TextContains : Scan
    {
        Dictionary<string, bool> Conditions;

        public TextContains(Dictionary<string, bool> conditions)
        {
            Conditions = conditions;
        }

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            foreach(KeyValuePair<string, bool> p in Conditions)
            {
                if (ArticleText.IndexOf(
                    Tools.ApplyKeyWords(ArticleTitle, p.Key), p.Value ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) < 0)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class TextDoesNotContain : Scan
    {
        Dictionary<string, bool> Conditions;

        public TextDoesNotContain(Dictionary<string, bool> conditions)
        {
            Conditions = conditions;
        }

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            foreach (KeyValuePair<string, bool> p in Conditions)
            {
                // TODO: other types of comparison may be more appropriate
                if (ArticleText.IndexOf(
                    Tools.ApplyKeyWords(ArticleTitle, p.Key), p.Value ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    return false;
                }
            }

            return true;
        }
    }

    /// <summary>
    /// Returns whether the article matches the provided regexes
    /// </summary>
    public class TextContainsRegex : Scan
    {
        public TextContainsRegex(params Regex[] containsR)
        {
            Contains = containsR;
        }

        Regex[] Contains;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            foreach (Regex r in Contains)
            {
                if (!r.IsMatch(ArticleText))
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Returns whether the article doesn't match the provided regexes
    /// </summary>
    public class TextDoesNotContainRegex : Scan
    {
        public TextDoesNotContainRegex(params Regex[] notContainsR)
        {
            NotContains = notContainsR;
        }

        Regex[] NotContains;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            foreach (Regex r in NotContains)
            {
                if (r.IsMatch(ArticleText))
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Returns whether the article title matches the provided regexes
    /// </summary>
    public class TitleContains : Scan
    {
        public TitleContains(Regex ContainsR)
        {
            Contains = ContainsR;
        }

        Regex Contains;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            return (Contains.IsMatch(ArticleTitle));
        }
    }

    /// <summary>
    /// Returns whether the article title doesn't match the provided regexes
    /// </summary>
    public class TitleDoesNotContain : Scan
    {
        public TitleDoesNotContain(Regex NotContainsR)
        {
            NotContains = NotContainsR;
        }

        Regex NotContains;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            return (!NotContains.IsMatch(ArticleTitle));
        }
    }

    public enum MoreLessThan : int { LessThan, MoreThan, EqualTo }

    /// <summary>
    /// Returns whether the article matches the specified boundaries for number of characters
    /// </summary>
    public class CountCharacters : Scan
    {
        public CountCharacters(MoreLessThan Value, int Characters)
        {
            M = Value;
            test = Characters;
        }

        MoreLessThan M;
        int test;
        int actual;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            actual = ArticleText.Length;

            if (M == MoreLessThan.MoreThan)
                return (actual > test);
            else if (M == MoreLessThan.LessThan)
                return (actual < test);
            else
                return (actual == test);
        }
    }

    /// <summary>
    /// Returns whether the article matches the specified boundaries for number of links
    /// </summary>
    public class CountLinks : Scan
    {
        public CountLinks(MoreLessThan Value, int Links)
        {
            M = Value;
            test = Links;
        }

        MoreLessThan M;
        int test;
        int actual;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            actual = WikiRegexes.SimpleWikiLink.Matches(ArticleText).Count;

            if (M == MoreLessThan.MoreThan)
                return (actual > test);
            else if (M == MoreLessThan.LessThan)
                return (actual < test);
            else
                return (actual == test);
        }
    }

    /// <summary>
    /// Returns whether the article matches the specified boundaries for number of words
    /// </summary>
    public class CountWords : Scan
    {
        public CountWords(MoreLessThan Value, int Words)
        {
            M = Value;
            test = Words;
        }

        MoreLessThan M;
        int test;
        int actual;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            actual = Tools.WordCount(ArticleText);

            if (M == MoreLessThan.MoreThan)
                return (actual > test);
            else if (M == MoreLessThan.LessThan)
                return (actual < test);
            else
                return (actual == test);
        }
    }

    /// <summary>
    /// Returns whether the article is in one of the specified namespaces
    /// </summary>
    public class CheckNamespace : Scan
    {
        public CheckNamespace(List<int> NameSpaces)
        {
            namespaces = NameSpaces;
        }

        List<int> namespaces = new List<int>();
        int NamespaceIndex;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            NamespaceIndex = Tools.CalculateNS(ArticleTitle);
            for (int i = 0; i < namespaces.Count; i++)
            {
                if (NamespaceIndex == namespaces[i])
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Returns whether the article has links that awb would improve/simplify
    /// </summary>
    public class HasBadLinks : Scan
    {
        public HasBadLinks() { }

        public static bool FixLinks(string ArticleText)
        {
            string y = "";

            foreach (Match m in WikiRegexes.SimpleWikiLink.Matches(ArticleText))
            {
                y = m.Value;
                y = y.Replace("+", "%2B");
                y = System.Web.HttpUtility.UrlDecode(y);

                if (m.Value != y)
                    return false;
            }

            return true;
        }

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            return FixLinks(ArticleText);
        }
    }

    /// <summary>
    /// Returns whether the article has not got its title emboldened in the article
    /// </summary>
    public class HasNoBoldTitle : Scan
    {
        public HasNoBoldTitle(Parsers p)
        {
            parsers = p;
        }

        bool skip = true;
        Parsers parsers;
        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            parsers.BoldTitle(ArticleText, ArticleTitle, out skip);

            return !skip;
        }
    }

    /// <summary>
    /// Returns whether the article has html entities
    /// </summary>
    public class HasHTMLEntities : Scan
    {
        public HasHTMLEntities(Parsers p)
        {
            parsers = p;
        }

        bool skip = true;
        Parsers parsers;
        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            parsers.Unicodify(ArticleText, out skip);

            return !skip;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HasSimpleLinks : Scan
    {
        public HasSimpleLinks() { }

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            string a = "";
            string b = "";

            foreach (Match m in WikiRegexes.PipedWikiLink.Matches(ArticleText))
            {
                a = m.Groups[1].Value;
                b = m.Groups[2].Value;

                if (a == b || Tools.TurnFirstToLower(a) == b)
                    return true;
                else if (a + "s" == b || Tools.TurnFirstToLower(a) + "s" == b)
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HasSectionError : Scan
    {
        public HasSectionError(Parsers p)
        {
            parsers = p;
        }

        bool skip = true;
        Parsers parsers;
        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            parsers.FixHeadings(ArticleText, ArticleTitle, out skip);

            return !skip;
        }
    }

    /// <summary>
    /// Returns whether the article has unbulleted link sections
    /// </summary>
    public class HasUnbulletedLinks : Scan
    {
        public HasUnbulletedLinks() { }

        Regex bulletRegex = new Regex(@"External [Ll]inks? ? ?={1,4} ? ?(
){0,3}\[?http", RegexOptions.Compiled);

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            return (bulletRegex.IsMatch(ArticleText));
        }
    }

    /// <summary>
    /// Returns whether the article is of a living person
    /// </summary>
    public class LivingPerson : Scan
    {
        public LivingPerson(Parsers p)
        {
            parsers = p;
        }

        bool skip = true;
        Parsers parsers;
        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            parsers.LivingPeople(ArticleText, out skip);

            return !skip;
        }
    }

    /// <summary>
    /// Returns whether the article is missing a default sort (ie criteria match so that default sort would be added)
    /// </summary>
    public class MissingDefaultsort : Scan
    {
        public MissingDefaultsort(Parsers p)
        {
            parsers = p;
        }

        bool skip = true;
        Parsers parsers;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            parsers.ChangeToDefaultSort(ArticleText, ArticleTitle, out skip);

            return !skip;
        }
    }

    /// <summary>
    /// Returns whether the article has typo's that AWB would fix
    /// </summary>
    public class Typo : Scan
    {
        public Typo() { }

        RegExTypoFix retf = new RegExTypoFix();

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            return retf.DetectTypo(ArticleText);
        }
    }

    /// <summary>
    /// Returns whether the article is uncategorised
    /// </summary>
    public class UnCategorised : Scan
    {
        public UnCategorised() { }

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            if (WikiRegexes.Category.IsMatch(ArticleText))
                return false;

            foreach (Match m in WikiRegexes.Template.Matches(ArticleText))
            {
                if (!m.Value.Contains("stub"))
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Returns if the article was last edited between the specified dates
    /// </summary>
    public class DateRange : Scan
    {
        DateTime from, to;
        public DateRange(DateTime from, DateTime to)
        {
            this.from = from;
            this.to = to;
        }

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            DateTime timestamp;
            if (DateTime.TryParse(ArticleTimestamp, out timestamp))
                return ((DateTime.Compare(timestamp, from) >= 0) && (DateTime.Compare(timestamp, to) <= 0));
            else
                return false;
        }
    }

    /// <summary>
    /// Returns whether the article matches the specified move and edit restrictions
    /// </summary>
    public class Restriction : Scan
    {
        string[] Restrictions = new string[] { "autoconfirmed", "sysop" };

        string editRest = "edit=";
        string moveRest = "move=";

        int edit, move;
        public Restriction(int EditLevel, int MoveLevel)
        {
            edit = (EditLevel - 1);
            move = (MoveLevel - 1);
        }

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            bool restrictionStringEmpty = (string.IsNullOrEmpty(ArticleRestrictions));
            bool noEditRestriction = (edit == -1);
            bool noMoveRestriction = (move == -1);

            if (restrictionStringEmpty)
            {
                return (noEditRestriction && noMoveRestriction);
            }
            else
            {
                if (!noEditRestriction && !ArticleRestrictions.Contains(editRest + Restrictions[edit]))
                    return false;

                if (!noMoveRestriction && !ArticleRestrictions.Contains(moveRest + Restrictions[move]))
                    return false;

                return true;
            }
        }
    }
}
