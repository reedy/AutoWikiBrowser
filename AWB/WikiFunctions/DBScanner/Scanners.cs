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
        public virtual bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            return true;
        }
    }

    /// <summary>
    /// Returns whether the article is not a redirect
    /// </summary>
    public class IsNotRedirect : Scan
    {
        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            return (!Tools.IsRedirect(articleText));
        }
    }
	
	//TODO:Update TextContains etc to use Inheritors of IArticleComparer

	/// <summary>
    /// 
    /// </summary>
    public class TextContains : Scan
    {
        private readonly Dictionary<string, bool> Conditions;

        public TextContains(Dictionary<string, bool> conditions)
        {
            Conditions = conditions;
        }

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            foreach(KeyValuePair<string, bool> p in Conditions)
            {
                if (articleText.IndexOf(
                    Tools.ApplyKeyWords(articleTitle, p.Key), p.Value ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) < 0)
                {
                    return false;
                }
            }

            return true;
        }
    }

	/// <summary>
    /// 
    /// </summary>
    public class TextDoesNotContain : Scan
    {
        private readonly Dictionary<string, bool> Conditions;

        public TextDoesNotContain(Dictionary<string, bool> conditions)
        {
            Conditions = conditions;
        }

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            foreach (KeyValuePair<string, bool> p in Conditions)
            {
                // TODO: other types of comparison may be more appropriate
                if (articleText.IndexOf(
                    Tools.ApplyKeyWords(articleTitle, p.Key), p.Value ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase) >= 0)
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

        private readonly Regex[] Contains;

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            foreach (Regex r in Contains)
            {
                if (!r.IsMatch(articleText))
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

        private readonly Regex[] NotContains;

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            foreach (Regex r in NotContains)
            {
                if (r.IsMatch(articleText))
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
        public TitleContains(Regex containsR)
        {
            Contains = containsR;
        }

        private readonly Regex Contains;

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            return (Contains.IsMatch(articleTitle));
        }
    }

    /// <summary>
    /// Returns whether the article title doesn't match the provided regexes
    /// </summary>
    public class TitleDoesNotContain : Scan
    {
        public TitleDoesNotContain(Regex notContainsR)
        {
            NotContains = notContainsR;
        }

        private readonly Regex NotContains;

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            return (!NotContains.IsMatch(articleTitle));
        }
    }

    public enum MoreLessThan { LessThan, MoreThan, EqualTo }

    /// <summary>
    /// Returns whether the article matches the specified boundaries for number of characters
    /// </summary>
    public class CountCharacters : Scan
    {
        public CountCharacters(MoreLessThan value, int characters)
        {
            M = value;
            Test = characters;
        }

        private readonly MoreLessThan M;
        private readonly int Test;
        private int Actual;

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            Actual = articleText.Length;

            if (M == MoreLessThan.MoreThan)
                return (Actual > Test);
            if (M == MoreLessThan.LessThan)
                return (Actual < Test);
            
            return (Actual == Test);
        }
    }

    /// <summary>
    /// Returns whether the article matches the specified boundaries for number of links
    /// </summary>
    public class CountLinks : Scan
    {
        public CountLinks(MoreLessThan value, int links)
        {
            M = value;
            Test = links;
        }

        private readonly MoreLessThan M;
        private readonly int Test;
        private int Actual;

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            Actual = WikiRegexes.SimpleWikiLink.Matches(articleText).Count;

            if (M == MoreLessThan.MoreThan)
                return (Actual > Test);
            if (M == MoreLessThan.LessThan)
                return (Actual < Test);
            return (Actual == Test);
        }
    }

    /// <summary>
    /// Returns whether the article matches the specified boundaries for number of words
    /// </summary>
    public class CountWords : Scan
    {
        public CountWords(MoreLessThan value, int words)
        {
            M = value;
            Test = words;
        }

        private readonly MoreLessThan M;
        private readonly int Test;
        private int Actual;

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            Actual = Tools.WordCount(articleText);

            if (M == MoreLessThan.MoreThan)
                return (Actual > Test);
            if (M == MoreLessThan.LessThan)
                return (Actual < Test);
            return (Actual == Test);
        }
    }

    /// <summary>
    /// Returns whether the article is in one of the specified Namespaces
    /// </summary>
    public class CheckNamespace : Scan
    {
        public CheckNamespace(List<int> nameSpaces)
        {
            Namespaces = nameSpaces;
        }

        private readonly List<int> Namespaces;

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            return (Namespaces.Contains(Namespace.Determine(articleTitle)));
        }
    }

    /// <summary>
    /// Returns whether the article has links that awb would improve/simplify
    /// </summary>
    public class HasBadLinks : Scan
    {
        private static bool FixLinks(string articleText)
        {
            foreach (Match m in WikiRegexes.SimpleWikiLink.Matches(articleText))
            {
                string y = System.Web.HttpUtility.UrlDecode(m.Value.Replace("+", "%2B"));

                if (m.Value != y)
                    return false;
            }

            return true;
        }

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            return FixLinks(articleText);
        }
    }

    /// <summary>
    /// Returns whether the article has not got its title emboldened in the article
    /// </summary>
    public class HasNoBoldTitle : Scan
    {
        private readonly Parsers P = new Parsers();
        private bool Skip;

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            P.BoldTitle(articleText, articleTitle, out Skip);

            return !Skip;
        }
    }

    /// <summary>
    /// Returns whether CiteTemplateDates fixed something in the article
    /// </summary>
    public class CiteTemplateDates : Scan
    {
        private readonly Parsers P = new Parsers();
        private bool Skip;

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            P.CiteTemplateDates(articleText, out Skip);

            return !Skip;
        }
    }

    /// <summary>
    /// Returns whether FixPeopleCategories did something (birth/death categories for living people altered)
    /// </summary>
    public class PeopleCategories : Scan
    {
        private readonly Parsers P = new Parsers();
        private bool Skip;

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            P.FixPeopleCategories(articleText, articleTitle, out Skip);

            return !Skip;
        }

    }
    
    /// <summary>
    /// Returns whether the article has unbalanced brackets
    /// </summary>
    public class UnbalancedBrackets : Scan
    {
        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            int bracketLength = 0;

            return (Parsers.UnbalancedBrackets(articleText, ref bracketLength) != -1);
        }
    }

    /// <summary>
    /// Returns whether the article has html entities
    /// </summary>
    public class HasHTMLEntities : Scan
    {
        private readonly Parsers P = new Parsers();
        private bool Skip;

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            P.Unicodify(articleText, out Skip);

            return !Skip;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HasSimpleLinks : Scan
    {
        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            foreach (Match m in WikiRegexes.PipedWikiLink.Matches(articleText))
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
        private bool Skip = true;

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            Parsers.FixHeadings(articleText, articleTitle, out Skip);

            return !Skip;
        }
    }

    /// <summary>
    /// Returns whether the article has unbulleted link sections
    /// </summary>
    public class HasUnbulletedLinks : Scan
    {
        private readonly Regex BulletRegex = new Regex(@"External [Ll]inks? ? ?={1,4} ? ?(
){0,3}\[?http", RegexOptions.Compiled);

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            return (BulletRegex.IsMatch(articleText));
        }
    }

    /// <summary>
    /// Returns whether AWB would make changes to an article, adding [[Category:Living people]] to with a [[Category:XXXX births]] and no living people/deaths category
    /// </summary>
    public class LivingPerson : Scan
    {
        private bool Skip = true;

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            Parsers.LivingPeople(articleText, out Skip);

            return !Skip;
        }
    }

    /// <summary>
    /// Returns whether the article is missing a default sort (ie criteria match so that default sort would be added)
    /// </summary>
    public class MissingDefaultsort : Scan
    {
        private bool Skip = true;

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            Parsers.ChangeToDefaultSort(articleText, articleTitle, out Skip);

            return !Skip;
        }
    }

    /// <summary>
    /// Returns whether the article has typo's that AWB would fix
    /// </summary>
    public class Typo : Scan
    {
        private readonly RegExTypoFix Retf = new RegExTypoFix(false);

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            return Retf.DetectTypo(articleText, articleTitle);
        }
    }

    /// <summary>
    /// Returns whether the article is uncategorised
    /// </summary>
    public class UnCategorised : Scan
    {
        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            if (WikiRegexes.Category.IsMatch(articleText))
                return false;

            foreach (Match m in WikiRegexes.Template.Matches(articleText))
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
        private readonly DateTime From, To;

        public DateRange(DateTime from, DateTime to)
        {
            From = from;
            To = to;
        }

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            DateTime timestamp;
            if (DateTime.TryParse(articleTimestamp, out timestamp))
                return ((DateTime.Compare(timestamp, From) >= 0) && (DateTime.Compare(timestamp, To) <= 0));
            return false;
        }
    }

    /// <summary>
    /// Returns whether the article matches the specified move and edit restrictions
    /// </summary>
    public class Restriction : Scan
    {
        private const string EditRest = "edit=", MoveRest = "move=";

        private readonly string Edit, Move;

        public Restriction(string editLevel, string moveLevel)
        {
            Edit = editLevel;
            Move = moveLevel;
        }

        public override bool Check(ref string articleText, ref string articleTitle, string articleTimestamp, string articleRestrictions)
        {
            bool restrictionStringEmpty = (string.IsNullOrEmpty(articleRestrictions));
            bool noEditRestriction = string.IsNullOrEmpty(Edit);
            bool noMoveRestriction = string.IsNullOrEmpty(Move);

            if (restrictionStringEmpty)
            {
                return (noEditRestriction && noMoveRestriction);
            }

            if (!noEditRestriction && !articleRestrictions.Contains(EditRest + Edit))
                return false;

            return noMoveRestriction || articleRestrictions.Contains(MoveRest + Move);
        }
    }
}
