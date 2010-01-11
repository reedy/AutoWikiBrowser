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
        readonly Dictionary<string, bool> Conditions;

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
        readonly Dictionary<string, bool> Conditions;

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

        readonly Regex[] Contains;

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

        readonly Regex[] NotContains;

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

        readonly Regex Contains;

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

        readonly Regex NotContains;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            return (!NotContains.IsMatch(ArticleTitle));
        }
    }

    public enum MoreLessThan { LessThan, MoreThan, EqualTo }

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

        readonly MoreLessThan M;
        readonly int test;
        int actual;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            actual = ArticleText.Length;

            if (M == MoreLessThan.MoreThan)
                return (actual > test);
            if (M == MoreLessThan.LessThan)
                return (actual < test);
            
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

        readonly MoreLessThan M;
        readonly int test;
        int actual;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            actual = WikiRegexes.SimpleWikiLink.Matches(ArticleText).Count;

            if (M == MoreLessThan.MoreThan)
                return (actual > test);
            if (M == MoreLessThan.LessThan)
                return (actual < test);
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

        readonly MoreLessThan M;
        readonly int test;
        int actual;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            actual = Tools.WordCount(ArticleText);

            if (M == MoreLessThan.MoreThan)
                return (actual > test);
            if (M == MoreLessThan.LessThan)
                return (actual < test);
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

        readonly List<int> namespaces;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            return (namespaces.Contains(Tools.CalculateNS(ArticleTitle)));
        }
    }

    /// <summary>
    /// Returns whether the article has links that awb would improve/simplify
    /// </summary>
    public class HasBadLinks : Scan
    {
        public static bool FixLinks(string ArticleText)
        {
            foreach (Match m in WikiRegexes.SimpleWikiLink.Matches(ArticleText))
            {
                string y = System.Web.HttpUtility.UrlDecode(m.Value.Replace("+", "%2B"));

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
        bool skip = true;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            Parsers.BoldTitle(ArticleText, ArticleTitle, out skip);

            return !skip;
        }
    }

    /// <summary>
    /// Returns whether the article has html entities
    /// </summary>
    public class HasHTMLEntities : Scan
    {
        bool skip = true;
        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            Parsers.Unicodify(ArticleText, out skip);

            return !skip;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HasSimpleLinks : Scan
    {
        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            foreach (Match m in WikiRegexes.PipedWikiLink.Matches(ArticleText))
            {
                string a = m.Groups[1].Value;
                string b = m.Groups[2].Value;

                if (a == b || Tools.TurnFirstToLower(a) == b)
                    return true;
                if (a + "s" == b || Tools.TurnFirstToLower(a) + "s" == b)
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
        bool skip = true;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            Parsers.FixHeadings(ArticleText, ArticleTitle, out skip);

            return !skip;
        }
    }

    /// <summary>
    /// Returns whether the article has unbulleted link sections
    /// </summary>
    public class HasUnbulletedLinks : Scan
    {
        readonly Regex bulletRegex = new Regex(@"External [Ll]inks? ? ?={1,4} ? ?(
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
        bool skip = true;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            Parsers.LivingPeople(ArticleText, out skip);

            return !skip;
        }
    }

    /// <summary>
    /// Returns whether the article is missing a default sort (ie criteria match so that default sort would be added)
    /// </summary>
    public class MissingDefaultsort : Scan
    {
        bool skip = true;

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            Parsers.ChangeToDefaultSort(ArticleText, ArticleTitle, out skip);

            return !skip;
        }
    }

    /// <summary>
    /// Returns whether the article has typo's that AWB would fix
    /// </summary>
    public class Typo : Scan
    {
        readonly RegExTypoFix retf = new RegExTypoFix(false);

        public override bool Check(ref string ArticleText, ref string ArticleTitle, string ArticleTimestamp, string ArticleRestrictions)
        {
            return retf.DetectTypo(ArticleText, ArticleTitle);
        }
    }

    /// <summary>
    /// Returns whether the article is uncategorised
    /// </summary>
    public class UnCategorised : Scan
    {
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
        readonly DateTime from, to;

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
            return false;
        }
    }

    /// <summary>
    /// Returns whether the article matches the specified move and edit restrictions
    /// </summary>
    public class Restriction : Scan
    {
        readonly string[] Restrictions = new [] { "autoconfirmed", "sysop" };

        const string editRest = "edit=", moveRest = "move=";

        readonly int edit, move;

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

            if (!noEditRestriction && !ArticleRestrictions.Contains(editRest + Restrictions[edit]))
                return false;

            if (!noMoveRestriction && !ArticleRestrictions.Contains(moveRest + Restrictions[move]))
                return false;

            return true;
        }
    }
}
