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
        public virtual bool Check(ArticleInfo article)
        {
            return true;
        }
    }

    /// <summary>
    /// Returns whether the article is not a redirect
    /// </summary>
    public class IsNotRedirect : Scan
    {
        public override bool Check(ArticleInfo article)
        {
            return (!Tools.IsRedirect(article.Text));
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

        public override bool Check(ArticleInfo article)
        {
            foreach(KeyValuePair<string, bool> p in Conditions)
            {
                if (article.Text.IndexOf(
                    Tools.ApplyKeyWords(article.Title, p.Key), p.Value ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) < 0)
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

        public override bool Check(ArticleInfo article)
        {
            foreach (KeyValuePair<string, bool> p in Conditions)
            {
                // TODO: other types of comparison may be more appropriate
                if (article.Text.IndexOf(
                    Tools.ApplyKeyWords(article.Title, p.Key), p.Value ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase) >= 0)
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

        public override bool Check(ArticleInfo article)
        {
            foreach (Regex r in Contains)
            {
                if (!r.IsMatch(article.Text))
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

        public override bool Check(ArticleInfo article)
        {
            foreach (Regex r in NotContains)
            {
                if (r.IsMatch(article.Text))
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

        public override bool Check(ArticleInfo article)
        {
            return (Contains.IsMatch(article.Title));
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

        public override bool Check(ArticleInfo article)
        {
            return (!NotContains.IsMatch(article.Title));
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

        public override bool Check(ArticleInfo article)
        {
            Actual = article.Text.Length;

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

        public override bool Check(ArticleInfo article)
        {
            Actual = WikiRegexes.SimpleWikiLink.Matches(article.Text).Count;

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

        public override bool Check(ArticleInfo article)
        {
            Actual = Tools.WordCount(article.Text);

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

        public override bool Check(ArticleInfo article)
        {
            return (Namespaces.Contains(Namespace.Determine(article.Title)));
        }
    }

    /// <summary>
    /// Returns whether the article has links that awb would improve/simplify
    /// </summary>
    public class HasBadLinks : Scan
    {
        public override bool Check(ArticleInfo article)
        {
            foreach (Match m in WikiRegexes.SimpleWikiLink.Matches(article.Text))
            {
                string y = System.Web.HttpUtility.UrlDecode(m.Value.Replace("+", "%2B"));

                if (m.Value != y)
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Returns whether the article has not got its title emboldened in the article
    /// </summary>
    public class HasNoBoldTitle : Scan
    {
        private readonly Parsers P = new Parsers();
        private bool Skip;

        public override bool Check(ArticleInfo article)
        {
            P.BoldTitle(article.Text, article.Title, out Skip);

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

        public override bool Check(ArticleInfo article)
        {
            P.CiteTemplateDates(article.Text, out Skip);

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

        public override bool Check(ArticleInfo article)
        {
            P.FixPeopleCategories(article.Text, article.Title, out Skip);

            return !Skip;
        }

    }
    
    /// <summary>
    /// Returns whether the article has unbalanced brackets
    /// </summary>
    public class UnbalancedBrackets : Scan
    {
        public override bool Check(ArticleInfo article)
        {
            int bracketLength = 0;

            return (Parsers.UnbalancedBrackets(article.Text, ref bracketLength) != -1);
        }
    }

    /// <summary>
    /// Returns whether the article has html entities
    /// </summary>
    public class HasHTMLEntities : Scan
    {
        private readonly Parsers P = new Parsers();
        private bool Skip;

        public override bool Check(ArticleInfo article)
        {
            P.Unicodify(article.Text, out Skip);

            return !Skip;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HasSimpleLinks : Scan
    {
        public override bool Check(ArticleInfo article)
        {
            foreach (Match m in WikiRegexes.PipedWikiLink.Matches(article.Text))
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

        public override bool Check(ArticleInfo article)
        {
            Parsers.FixHeadings(article.Text, article.Title, out Skip);

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

        public override bool Check(ArticleInfo article)
        {
            return (BulletRegex.IsMatch(article.Text));
        }
    }

    /// <summary>
    /// Returns whether AWB would make changes to an article, adding [[Category:Living people]] to with a [[Category:XXXX births]] and no living people/deaths category
    /// </summary>
    public class LivingPerson : Scan
    {
        private bool Skip = true;

        public override bool Check(ArticleInfo article)
        {
            Parsers.LivingPeople(article.Text, out Skip);

            return !Skip;
        }
    }

    /// <summary>
    /// Returns whether the article is missing a default sort (ie criteria match so that default sort would be added)
    /// </summary>
    public class MissingDefaultsort : Scan
    {
        private bool Skip = true;

        public override bool Check(ArticleInfo article)
        {
            Parsers.ChangeToDefaultSort(article.Text, article.Title, out Skip);

            return !Skip;
        }
    }

    /// <summary>
    /// Returns whether the article has typo's that AWB would fix
    /// </summary>
    public class Typo : Scan
    {
        private readonly RegExTypoFix Retf = new RegExTypoFix(false);

        public override bool Check(ArticleInfo article)
        {
            return Retf.DetectTypo(article.Text, article.Title);
        }
    }

    /// <summary>
    /// Returns whether the article is uncategorised
    /// </summary>
    public class UnCategorised : Scan
    {
        public override bool Check(ArticleInfo article)
        {
            if (WikiRegexes.Category.IsMatch(article.Text))
                return false;

            foreach (Match m in WikiRegexes.Template.Matches(article.Text))
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

        public override bool Check(ArticleInfo article)
        {
            DateTime timestamp;
            if (DateTime.TryParse(article.Timestamp, out timestamp))
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

        public override bool Check(ArticleInfo article)
        {
            bool restrictionStringEmpty = (string.IsNullOrEmpty(article.Restrictions));
            bool noEditRestriction = string.IsNullOrEmpty(Edit);
            bool noMoveRestriction = string.IsNullOrEmpty(Move);

            if (restrictionStringEmpty)
            {
                return (noEditRestriction && noMoveRestriction);
            }

            if (!noEditRestriction && !article.Restrictions.Contains(EditRest + Edit))
                return false;

            return noMoveRestriction || article.Restrictions.Contains(MoveRest + Move);
        }
    }
}
