/*

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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using WikiFunctions.Lists.Providers;

namespace WikiFunctions.Parse
{

    /// <summary>
    /// Provides functions for editing wiki text, such as formatting and re-categorisation.
    /// </summary>
    public partial class Parsers
    {

        /// <summary>
        /// Adds the category to the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="newCategory">The new category.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="noChange"></param>
        /// <returns>The article text.</returns>
        public string AddCategory(string newCategory, string articleText, string articleTitle, out bool noChange)
        {
            string newText = AddCategory(newCategory, articleText, articleTitle);

            noChange = newText.Equals(articleText);

            return newText;
        }

        // Covered by: RecategorizerTests.Addition()
        /// <summary>
        /// Adds the category to the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="newCategory">The new category.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <returns>The article text.</returns>
        public string AddCategory(string newCategory, string articleText, string articleTitle)
        {
            string oldText = articleText;

            articleText = FixCategories(articleText);

            if (Regex.IsMatch(articleText, @"\[\["
                              + Variables.NamespacesCaseInsensitive[Namespace.Category]
                              + Regex.Escape(newCategory) + @"[\|\]]"))
            {
                return oldText;
            }

            string cat = Tools.Newline("[[" + Variables.Namespaces[Namespace.Category] + newCategory + "]]");
            cat = Tools.ApplyKeyWords(articleTitle, cat);

            if (Namespace.Determine(articleTitle) == Namespace.Template)
                articleText += "<noinclude>" + cat + Tools.Newline("</noinclude>");
            else
                articleText += cat;

            return SortMetaData(articleText, articleTitle, false); //Sort metadata ordering so general fixes do not need to be enabled
        }

        // Covered by: RecategorizerTests.Replacement()
        /// <summary>
        /// Re-categorises the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="oldCategory">The old category to replace.</param>
        /// <param name="newCategory">The new category.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The re-categorised article text.</returns>
        public static string ReCategoriser(string oldCategory, string newCategory, string articleText, out bool noChange)
        {
            return ReCategoriser(oldCategory, newCategory, articleText, out noChange, false);
        }

        // Covered by: RecategorizerTests.Replacement()
        /// <summary>
        /// Re-categorises the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="oldCategory">The old category to replace.</param>
        /// <param name="newCategory">The new category.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <param name="removeSortKey">If set, any sort key is removed when the category is replaced</param>
        /// <returns>The re-categorised article text.</returns>
        public static string ReCategoriser(string oldCategory, string newCategory, string articleText, out bool noChange, bool removeSortKey)
        {
            //remove category prefix
            oldCategory = Regex.Replace(oldCategory, "^"
                                        + Variables.NamespacesCaseInsensitive[Namespace.Category], "", RegexOptions.IgnoreCase);
            newCategory = Regex.Replace(newCategory, "^"
                                        + Variables.NamespacesCaseInsensitive[Namespace.Category], "", RegexOptions.IgnoreCase);

            //format categories properly
            articleText = FixCategories(articleText);

            string testText = articleText;

            if (Regex.IsMatch(articleText, "\\[\\["
                              + Variables.NamespacesCaseInsensitive[Namespace.Category]
                              + Tools.CaseInsensitive(Regex.Escape(newCategory)) + @"\s*(\||\]\])"))
            {
                bool tmp;
                articleText = RemoveCategory(oldCategory, articleText, out tmp);
            }
            else
            {
                oldCategory = Regex.Escape(oldCategory);
                oldCategory = Tools.CaseInsensitive(oldCategory);

                oldCategory = Variables.Namespaces[Namespace.Category] + oldCategory + @"\s*(\|[^\|\[\]]+\]\]|\]\])";

                // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Replacing_categoring_and_keeping_pipes
                if (!removeSortKey)
                    newCategory = Variables.Namespaces[Namespace.Category] + newCategory + "$1";
                else
                    newCategory = Variables.Namespaces[Namespace.Category] + newCategory + @"]]";

                articleText = Regex.Replace(articleText, oldCategory, newCategory);
            }

            noChange = (testText.Equals(articleText));

            return articleText;
        }

        // Covered by: RecategorizerTests.Removal()
        /// <summary>
        /// Removes a category from an article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="strOldCat">The old category to remove.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The article text without the old category.</returns>
        public static string RemoveCategory(string strOldCat, string articleText, out bool noChange)
        {
            articleText = FixCategories(articleText);
            string testText = articleText;

            articleText = RemoveCategory(strOldCat, articleText);

            noChange = (testText.Equals(articleText));

            return articleText;
        }

        /// <summary>
        /// Removes a category from an article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="strOldCat">The old category to remove.</param>
        /// <returns>The article text without the old category.</returns>
        public static string RemoveCategory(string strOldCat, string articleText)
        {
            strOldCat = Tools.CaseInsensitive(Regex.Escape(strOldCat));

            if (!articleText.Contains("<includeonly>"))
                articleText = Regex.Replace(articleText, "\\[\\["
                                            + Variables.NamespacesCaseInsensitive[Namespace.Category] + " ?"
                                            + strOldCat + "( ?\\]\\]| ?\\|[^\\|]*?\\]\\])\r\n", "");

            articleText = Regex.Replace(articleText, "\\[\\["
                                        + Variables.NamespacesCaseInsensitive[Namespace.Category] + " ?"
                                        + strOldCat + "( ?\\]\\]| ?\\|[^\\|]*?\\]\\])", "");

            return articleText;
        }

        /// <summary>
        /// Returns whether the input string matches the name of a category in use in the input article text string, based on a case insensitive match
        /// </summary>
        /// <param name="articleText">the article text</param>
        /// <param name="categoryName">name of the category</param>
        /// <returns></returns>
        public static bool CategoryMatch(string articleText, string categoryName)
        {
            // for performance only search article from first category
            Match cq = WikiRegexes.CategoryQuick.Match(articleText);

            if(cq.Success)
            {
                Regex anyCategory = new Regex(@"\[\[\s*" + Variables.NamespacesCaseInsensitive[Namespace.Category] + @"\s*" + Regex.Escape(categoryName) + @"\s*(?:|\|([^\|\]]*))\s*\]\]", RegexOptions.IgnoreCase);

                return anyCategory.IsMatch(articleText.Substring(cq.Index));
            }

            return false;
        }

        /// <summary>
        /// Returns a concatenated string of all categories in the article
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private static string GetCats(string articleText)
        {
            return string.Join("", GetAllWikiLinks(articleText).Where(l => l.Contains(":") && WikiRegexes.Category.IsMatch(l)).ToArray());
        }

        /// <summary>
        /// Returns whether the article is missing a defaultsort (i.e. criteria match so that defaultsort would be added)
        /// </summary>
        /// <param name="articletext"></param>
        /// <param name="articletitle"></param>
        /// <returns></returns>
        public static bool MissingDefaultSort(string articletext, string articletitle)
        {
            bool Skip, DSbefore = WikiRegexes.Defaultsort.IsMatch(articletext);
            if(!DSbefore)
            {
                articletext = ChangeToDefaultSort(articletext, articletitle, out Skip);
                return (!Skip && WikiRegexes.Defaultsort.IsMatch(articletext));
            }

            return false;
        }

        /// <summary>
        /// Changes an article to use defaultsort when all categories use the same sort field / cleans diacritics from defaultsort/categories
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="noChange">If there is no change (True if no Change)</param>
        /// <returns>The article text possibly using defaultsort.</returns>
        public static string ChangeToDefaultSort(string articleText, string articleTitle, out bool noChange)
        {
            return ChangeToDefaultSort(articleText, articleTitle, out noChange, false);
        }

        /// <summary>
        /// Returns the sortkey used by all categories, if
        /// * all categories use the same sortkey
        /// * no {{DEFAULTSORT}} in article
        /// Otherwise returns null
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string GetCategorySort(string articleText)
        {
            if (WikiRegexes.Defaultsort.Matches(articleText).Count == 1)
                return "";

            int matches;
            const string dummy = @"@@@@";

            string sort = GetCategorySort(articleText, dummy, out matches);

            return sort == dummy ? "" : sort;
        }

        /// <summary>
        /// Returns the sortkey used by all categories, if all categories use the same sortkey
        /// Where no sortkey is used for all categories, returns the articletitle
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="matches">Number of categories with the same or no sortkey</param>
        /// <returns></returns>
        public static string GetCategorySort(string articleText, string articleTitle, out int matches)
        {
            string sort = "";
            bool allsame = true;
            matches = 0;

            articleText = articleText.Replace(@"{{PAGENAME}}", articleTitle);
            articleText = articleText.Replace(@"{{subst:PAGENAME}}", articleTitle);

            foreach (Match m in WikiRegexes.Category.Matches(articleText))
            {
                string explicitKey = m.Groups[2].Value;
                if (explicitKey.Length == 0)
                    explicitKey = articleTitle;

                if (string.IsNullOrEmpty(sort))
                    sort = explicitKey;

                if (sort != explicitKey && !String.IsNullOrEmpty(explicitKey))
                {
                    allsame = false;
                    break;
                }
                matches++;
            }
            if (allsame && matches > 0)
                return sort;
            return "";
        }

        // Covered by: UtilityFunctionTests.ChangeToDefaultSort()
        /// <summary>
        /// Changes an article to use defaultsort when all categories use the same sort field / cleans diacritics from defaultsort/categories
        /// Skips pages using &lt;noinclude&gt;, &lt;includeonly&gt; etc.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="noChange">If there is no change (True if no Change)</param>
        /// <param name="restrictDefaultsortChanges">Prevent insertion of a new {{DEFAULTSORT}} as AWB may not always be right for articles about people</param>
        /// <returns>The article text possibly using defaultsort.</returns>
        public static string ChangeToDefaultSort(string articleText, string articleTitle, out bool noChange, bool restrictDefaultsortChanges)
        {
            string originalArticleText = articleText;
            noChange = true;

            // count categories
            int matches;

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_12#defaultsort_adding_namespace
            articleTitle = Tools.RemoveNamespaceString(articleTitle);

            MatchCollection ds = WikiRegexes.Defaultsort.Matches(articleText);
            if (ds.Count > 1 || (ds.Count == 1 && !ds[0].Value.ToUpper().Contains("DEFAULTSORT")))
            {
                bool allsame2 = false;
                string lastvalue = "";
                // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Detect_multiple_DEFAULTSORT
                // if all the defaultsorts are the same just remove all but one
                foreach (Match m in ds)
                {
                    if (lastvalue.Length == 0)
                    {
                        lastvalue = m.Value;
                        allsame2 = true;
                    }
                    else
                        allsame2 = (m.Value == lastvalue);
                }
                if (allsame2)
                    articleText = WikiRegexes.Defaultsort.Replace(articleText, "", ds.Count - 1);
                else
                    return articleText;
            }

            // Performance: only apply replace and refresh if defaultsort would change
            if(ds.Count > 0 && Variables.LangCode.Equals("en") && !DefaultsortME(ds[0]).Equals(ds[0].Value))
            {
                articleText = WikiRegexes.Defaultsort.Replace(articleText, DefaultsortME);

                // match again, after normalisation
                ds = WikiRegexes.Defaultsort.Matches(articleText);
            }
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#AWB_didn.27t_fix_special_characters_in_a_pipe
            articleText = FixCategories(articleText);

            if (!restrictDefaultsortChanges)
            {
                // AWB's generation of its own sortkey may be incorrect for people, provide option not to insert in this situation
                if (ds.Count == 0)
                {
                    string sort = GetCategorySort(articleText, articleTitle, out matches);
                    // So that this does not get confused by sort keys of "*", " ", etc.
                    // MW bug: DEFAULTSORT does not treat leading spaces the same way as categories do
                    // if all existing categories use a suitable sortkey, insert that rather than generating a new one
                    // GetCatSortkey just returns articleTitle if cats do not have sortkey, so do not accept this here
                    if (sort.Length > 4 && matches > 1 && !sort.StartsWith(" "))
                    {
                        // remove keys from categories
                        articleText = WikiRegexes.Category.Replace(articleText, "[["
                                                                   + Variables.Namespaces[Namespace.Category] + "$1]]");

                        // set the defaultsort to the existing unique category sort value
                        // do not add a defaultsort if cat sort was the same as article title, now not case sensitive
                        if ((sort != articleTitle && Tools.FixupDefaultSort(sort).ToLower() != articleTitle.ToLower())
                            || (Tools.RemoveDiacritics(sort) != sort && !IsArticleAboutAPerson(articleText, articleTitle, false)))
                            articleText += Tools.Newline("{{DEFAULTSORT:") + Tools.FixupDefaultSort(sort) + "}}";
                    }

                    // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Add_defaultsort_to_pages_with_special_letters_and_no_defaultsort
                    // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Human_DEFAULTSORT
                    articleText = DefaultsortTitlesWithDiacritics(articleText, articleTitle, matches, IsArticleAboutAPerson(articleText, articleTitle, true));
                }
                else if (ds.Count == 1) // already has DEFAULTSORT
                {
                    string s = Tools.FixupDefaultSort(ds[0].Groups[1].Value.TrimStart('|'), (HumanDefaultSortCleanupRequired(ds[0]) && IsArticleAboutAPerson(articleText, articleTitle, true))).Trim();

                    // do not change DEFAULTSORT just for casing
                    if (!s.ToLower().Equals(ds[0].Groups[1].Value.ToLower()) && s.Length > 0 && !restrictDefaultsortChanges)
                        articleText = articleText.Replace(ds[0].Value, "{{DEFAULTSORT:" + s + "}}");

                    // get key value again in case replace above changed it
                    ds = WikiRegexes.Defaultsort.Matches(articleText);
                    string defaultsortKey = ds[0].Groups["key"].Value;

                    //Removes any explicit keys that are case insensitively the same as the default sort (To help tidy up on pages that already have defaultsort)
                    articleText = ExplicitCategorySortkeys(articleText, defaultsortKey);
                }
            }

            noChange = originalArticleText.Equals(articleText);

            // Performance: run relatively slow NoIncludeIncludeOnlyProgrammingElement check only if needed
            if(!noChange && NoIncludeIncludeOnlyProgrammingElement(originalArticleText))
            {
                noChange = true;
                return originalArticleText;
            }

            return articleText;
        }
        
        /// <summary>
        /// Returns whether human name defaultsort cleanup required: contains apostrophe or unspaced comma
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        private static bool HumanDefaultSortCleanupRequired(Match ds)
        {
            return (ds.Groups[1].Value.Contains("'") || Regex.IsMatch(ds.Groups[1].Value, @"\w,\w"));
        }

        /// <summary>
        /// Removes any explicit keys that are case insensitively the same as the default sort OR entirely match the start of the defaultsort (To help tidy up on pages that already have defaultsort)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="defaultsortKey"></param>
        /// <returns>The article text.</returns>
        private static string ExplicitCategorySortkeys(string articleText, string defaultsortKey)
        {
            foreach (Match m in WikiRegexes.Category.Matches(articleText))
            {
                string explicitKey = m.Groups[2].Value;
                if (explicitKey.Length == 0)
                    continue;

                if (string.Compare(explicitKey, defaultsortKey, StringComparison.OrdinalIgnoreCase) == 0
                    || defaultsortKey.StartsWith(explicitKey) || Tools.NestedTemplateRegex("PAGENAME").IsMatch(explicitKey))
                {
                    articleText = articleText.Replace(m.Value,
                                                      "[[" + Variables.Namespaces[Namespace.Category] + m.Groups[1].Value + "]]");
                }
            }
            return (articleText);
        }

        /// <summary>
        /// If title has diacritics, no defaultsort added yet, adds a defaultsort with cleaned up title as sort key
        /// If article is about a person, generates human name sortkey
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="categories">Number of categories on page</param>
        /// <param name="articleAboutAPerson">Whether the article is about a person</param>
        /// <returns>The article text possibly using defaultsort.</returns>
        private static string DefaultsortTitlesWithDiacritics(string articleText, string articleTitle, int categories, bool articleAboutAPerson)
        {
            // need some categories and no defaultsort, and a sortkey not the same as the article title
            if (categories > 0 && !WikiRegexes.Defaultsort.IsMatch(articleText))
            {
                // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Human_DEFAULTSORT
                // if article is about a person, attempt to add a surname, forenames sort key rather than the tidied article title
                string sortkey = articleAboutAPerson ? Tools.MakeHumanCatKey(articleTitle, articleText) : Tools.FixupDefaultSort(articleTitle);

                // sortkeys now not case sensitive
                if (!sortkey.ToLower().Equals(articleTitle.ToLower()) || Tools.RemoveDiacritics(articleTitle) != articleTitle)
                {
                    articleText += Tools.Newline("{{DEFAULTSORT:") + sortkey + "}}";

                    return (ExplicitCategorySortkeys(articleText, sortkey));
                }
            }

            return articleText;
        }

        /// <summary>
        /// Adds [[Category:XXXX births]], [[Category:XXXX deaths]] to articles about people where available, for en-wiki only
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="noChange"></param>
        /// <returns></returns>
        [Obsolete]
        [CLSCompliant(false)]
        public string FixPeopleCategories(string articleText, string articleTitle, out bool noChange)
        {
            string newText = FixPeopleCategories(articleText, articleTitle);

            noChange = newText.Equals(articleText);

            return newText;
        }

        /// <summary>
        /// Adds [[Category:XXXX births]], [[Category:XXXX deaths]] to articles about people where available, for en-wiki only
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="parseTalkPage"></param>
        /// <param name="noChange"></param>
        /// <returns></returns>
        public string FixPeopleCategories(string articleText, string articleTitle, bool parseTalkPage, out bool noChange)
        {
            string newText = FixPeopleCategories(articleText, articleTitle, parseTalkPage);

            noChange = newText.Equals(articleText);

            return newText;
        }

        private static readonly Regex LongWikilink = new Regex(@"\[\[[^\[\]\|]{11,}(?:\|[^\[\]]+)?\]\]");
        private static readonly Regex YearPossiblyWithBC = new Regex(@"\d{3,4}(?![\ds])(?: BC)?");
        private static readonly Regex ThreeOrFourDigitNumber = new Regex(@"[0-9]{3,4}");
        private static readonly Regex DiedOrBaptised = new Regex(@"(^.*?)((?:&[nm]dash;|—|–|;|[Dd](?:ied|\.)|baptised).*)");
        private static readonly Regex NotCircaTemplate = new Regex(@"{{(?!(?:[Cc]irca|[Ff]l\.?))[^{]*?}}");
        private static readonly Regex AsOfText = new Regex(@"\bas of\b");
        private static readonly Regex FloruitTemplate = Tools.NestedTemplateRegex(new [] {"fl", "fl."});

        /// <summary>
        /// Adds [[Category:XXXX births]], [[Category:XXXX deaths]] to articles about people where available, for en-wiki only
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <returns></returns>
        public static string FixPeopleCategories(string articleText, string articleTitle)
        {
            return FixPeopleCategories(articleText, articleTitle, false);
        }

        /// <summary>
        /// Adds [[Category:XXXX births]], [[Category:XXXX deaths]] to articles about people where available, for en-wiki only
        /// When page is not mainspace, adds [[:Category rather than [[Category
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="parseTalkPage"></param>
        /// <returns></returns>
        public static string FixPeopleCategories(string articleText, string articleTitle, bool parseTalkPage)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            // Performance: apply births/deaths category check on categories string, not whole article
            string cats = GetCats(articleText);

            bool dolmatch = WikiRegexes.DeathsOrLivingCategory.IsMatch(cats),
            bimatch = WikiRegexes.BirthsCategory.IsMatch(cats);

            // no work to do if already has a birth and a death/living cat
            if(dolmatch && bimatch)
                return YearOfBirthDeathMissingCategory(articleText, cats);

            // over 20 references or long and not DOB/DOD categorised at all yet: implausible
            if ((articleText.Length > 15000 && !bimatch && !dolmatch) || (!dolmatch && WikiRegexes.Refs.Matches(articleText).Count > 20))
                return YearOfBirthDeathMissingCategory(articleText, cats);

            string articleTextBefore = articleText;
            int catCount = WikiRegexes.Category.Matches(articleText).Count;

            // get the zeroth section (text upto first heading)
            string zerothSection = Tools.GetZerothSection(articleText);

            // remove references and long wikilinks (but allow an ISO date) that may contain false positives of birth/death date
            zerothSection = WikiRegexes.Refs.Replace(zerothSection, " ");
            zerothSection = LongWikilink.Replace(zerothSection, " ");

            // ignore dates from dated maintenance tags etc.
            zerothSection = WikiRegexes.NestedTemplates.Replace(zerothSection, m2=> Tools.GetTemplateParameterValue(m2.Value, "date").Length > 0 ? "" : m2.Value);
            zerothSection = WikiRegexes.TemplateMultiline.Replace(zerothSection, m2=> Tools.GetTemplateParameterValue(m2.Value, "date").Length > 0 ? "" : m2.Value);

            string StartCategory = Tools.Newline(@"[[" + (Namespace.IsMainSpace(articleTitle) ? "" : ":") + @"Category:");
            string yearstring, yearFromInfoBox = "", sort = GetCategorySort(articleText);

            bool alreadyUncertain = false;

            // scrape any infobox for birth year
            string fromInfoBox = GetInfoBoxFieldValue(zerothSection, WikiRegexes.InfoBoxDOBFields);

            // ignore as of dates
            if (AsOfText.IsMatch(fromInfoBox))
                fromInfoBox = fromInfoBox.Substring(0, AsOfText.Match(fromInfoBox).Index);

            if (fromInfoBox.Length > 0 && !UncertainWordings.IsMatch(fromInfoBox))
                yearFromInfoBox = YearPossiblyWithBC.Match(fromInfoBox).Value;

            // birth
            if (!WikiRegexes.BirthsCategory.IsMatch(articleText) && (PersonYearOfBirth.Matches(zerothSection).Count == 1
                                                                     || WikiRegexes.DateBirthAndAge.IsMatch(zerothSection)
                                                                     || WikiRegexes.DeathDateAndAge.IsMatch(zerothSection)
                                                                     || ThreeOrFourDigitNumber.IsMatch(yearFromInfoBox)))
            {
                // look for '{{birth date...' template first
                yearstring = WikiRegexes.DateBirthAndAge.Match(articleText).Groups[1].Value;

                // look for '{{death date and age' template second
                if (String.IsNullOrEmpty(yearstring))
                    yearstring = WikiRegexes.DeathDateAndAge.Match(articleText).Groups[2].Value;

                // thirdly use yearFromInfoBox
                if (ThreeOrFourDigitNumber.IsMatch(yearFromInfoBox))
                    yearstring = yearFromInfoBox;

                // look for '(born xxxx)'
                if (String.IsNullOrEmpty(yearstring))
                {
                    Match m = PersonYearOfBirth.Match(zerothSection);

                    // remove part beyond dash or died
                    string birthpart = DiedOrBaptised.Replace(m.Value, "$1");

                    if (WikiRegexes.CircaTemplate.IsMatch(birthpart))
                        alreadyUncertain = true;

                    birthpart = WikiRegexes.TemplateMultiline.Replace(birthpart, " ");

                    // check born info before any untemplated died info
                    if (!(m.Index > PersonYearOfDeath.Match(zerothSection).Index) || !PersonYearOfDeath.IsMatch(zerothSection))
                    {
                        // when there's only an approximate birth year, add the appropriate cat rather than the xxxx birth one
                        if (UncertainWordings.IsMatch(birthpart) || alreadyUncertain || FloruitTemplate.IsMatch(birthpart))
                        {
                            if (!CategoryMatch(articleText, YearOfBirthMissingLivingPeople) && !CategoryMatch(articleText, YearOfBirthUncertain))
                                articleText += StartCategory + YearOfBirthUncertain + CatEnd(sort);
                        }
                        else // after removing dashes, birthpart must still contain year
                            if (!birthpart.Contains(@"?") && Regex.IsMatch(birthpart, @"\d{3,4}"))
                                yearstring = m.Groups[1].Value;
                    }
                }

                // per [[:Category:Living people]], don't apply birth category if born > 121 years ago
                // validate a YYYY date is not in the future
                if (!string.IsNullOrEmpty(yearstring) && yearstring.Length > 2
                    && (!YearOnly.IsMatch(yearstring) || Convert.ToInt32(yearstring) <= DateTime.Now.Year)
                    && !(articleText.Contains(CategoryLivingPeople) && Convert.ToInt32(yearstring) < (DateTime.Now.Year - 121)))
                    articleText += StartCategory + yearstring + " births" + CatEnd(sort);
            }

            // scrape any infobox
            yearFromInfoBox = "";
            fromInfoBox = GetInfoBoxFieldValue(articleText, WikiRegexes.InfoBoxDODFields);

            if (fromInfoBox.Length > 0 && !UncertainWordings.IsMatch(fromInfoBox))
                yearFromInfoBox = YearPossiblyWithBC.Match(fromInfoBox).Value;

            if (!WikiRegexes.DeathsOrLivingCategory.IsMatch(RemoveCategory(YearofDeathMissing, articleText)) && (PersonYearOfDeath.IsMatch(zerothSection) || WikiRegexes.DeathDate.IsMatch(zerothSection)
                                                                                                                 || ThreeOrFourDigitNumber.IsMatch(yearFromInfoBox)))
            {
                // look for '{{death date...' template first
                yearstring = WikiRegexes.DeathDate.Match(articleText).Groups[1].Value;

                // secondly use yearFromInfoBox
                if (ThreeOrFourDigitNumber.IsMatch(yearFromInfoBox))
                    yearstring = yearFromInfoBox;

                // look for '(died xxxx)'
                if (string.IsNullOrEmpty(yearstring))
                {
                    Match m = PersonYearOfDeath.Match(zerothSection);

                    // check died info after any untemplated born info
                    if (m.Index >= PersonYearOfBirth.Match(zerothSection).Index || !PersonYearOfBirth.IsMatch(zerothSection))
                    {
                        if (!UncertainWordings.IsMatch(m.Value) && !m.Value.Contains(@"?"))
                            yearstring = m.Groups[1].Value;
                    }
                }

                // validate a YYYY date is not in the future
                if (!string.IsNullOrEmpty(yearstring) && yearstring.Length > 2
                    && (!YearOnly.IsMatch(yearstring) || Convert.ToInt32(yearstring) <= DateTime.Now.Year))
                    articleText += StartCategory + yearstring + " deaths" + CatEnd(sort);
            }

            zerothSection = NotCircaTemplate.Replace(zerothSection, " ");
            // birth and death combined
            // if not fully categorised, check it
            if (PersonYearOfBirthAndDeath.IsMatch(zerothSection) && (!WikiRegexes.BirthsCategory.IsMatch(articleText) || !WikiRegexes.DeathsOrLivingCategory.IsMatch(articleText)))
            {
                Match m = PersonYearOfBirthAndDeath.Match(zerothSection);

                string birthyear = m.Groups[1].Value;
                int birthyearint = int.Parse(birthyear);

                string deathyear = m.Groups[3].Value;
                int deathyearint = int.Parse(deathyear);

                // logical valdiation of dates
                if (birthyearint <= deathyearint && (deathyearint - birthyearint) <= 125)
                {
                    string birthpart = zerothSection.Substring(m.Index, m.Groups[2].Index - m.Index),
                    deathpart = zerothSection.Substring(m.Groups[2].Index, (m.Value.Length + m.Index) - m.Groups[2].Index);

                    if (!WikiRegexes.BirthsCategory.IsMatch(articleText))
                    {
                        if (!UncertainWordings.IsMatch(birthpart) && !ReignedRuledUnsure.IsMatch(m.Value) && !Regex.IsMatch(birthpart, @"(?:[Dd](?:ied|\.)|baptised)")
                            && !FloruitTemplate.IsMatch(birthpart))
                            articleText += StartCategory + birthyear + @" births" + CatEnd(sort);
                        else
                            if (UncertainWordings.IsMatch(birthpart) && !CategoryMatch(articleText, YearOfBirthMissingLivingPeople) && !CategoryMatch(articleText, YearOfBirthUncertain))
                                articleText += StartCategory + YearOfBirthUncertain + CatEnd(sort);
                    }

                    if (!UncertainWordings.IsMatch(deathpart) && !ReignedRuledUnsure.IsMatch(m.Value) && !Regex.IsMatch(deathpart, @"[Bb](?:orn|\.)") && !Regex.IsMatch(birthpart, @"[Dd](?:ied|\.)")
                        && (!WikiRegexes.DeathsOrLivingCategory.IsMatch(articleText) || CategoryMatch(articleText, YearofDeathMissing)))
                        articleText += StartCategory + deathyear + @" deaths" + CatEnd(sort);
                }
            }

            // do this check last as IsArticleAboutAPerson can be relatively slow
            if (!articleText.Equals(articleTextBefore) && !IsArticleAboutAPerson(articleTextBefore, articleTitle, parseTalkPage))
                return YearOfBirthDeathMissingCategory(articleTextBefore, cats);

            // {{uncat}} --> {{Improve categories}} if we've added cats
            if (WikiRegexes.Category.Matches(articleText).Count > catCount && WikiRegexes.Uncat.IsMatch(articleText)
                && !WikiRegexes.CatImprove.IsMatch(articleText))
                articleText = Tools.RenameTemplate(articleText, WikiRegexes.Uncat.Match(articleText).Groups[1].Value, "Improve categories");

            return YearOfBirthDeathMissingCategory(articleText, GetCats(articleText));
        }

        private static string CatEnd(string sort)
        {
            return ((sort.Length > 3) ? "|" + sort : "") + "]]";
        }

        private const string YearOfBirthMissingLivingPeople = "Year of birth missing (living people)",
        YearOfBirthMissing = "Year of birth missing",
        YearOfBirthUncertain = "Year of birth uncertain",
        YearofDeathMissing = "Year of death missing";

        private static readonly Regex Cat4YearBirths = new Regex(@"\[\[Category:\d{4} births\s*(?:\||\]\])");
        private static readonly Regex Cat4YearDeaths = new Regex(@"\[\[Category:\d{4} deaths\s*(?:\||\]\])");

        /// <summary>
        /// Removes birth/death missing categories when xxx births/deaths category also present
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns>The updated article text</returns>
        private static string YearOfBirthDeathMissingCategory(string articleText, string cats)
        {
            // if there is a 'year of birth missing' and a year of birth, remove the 'missing' category
            if(Cat4YearBirths.IsMatch(cats))
            {
                if (CategoryMatch(cats, YearOfBirthMissingLivingPeople))
                    articleText = RemoveCategory(YearOfBirthMissingLivingPeople, articleText);
                else if (CategoryMatch(cats, YearOfBirthMissing))
                    articleText = RemoveCategory(YearOfBirthMissing, articleText);
            }

            // if there's a 'year of birth missing' and a 'year of birth uncertain', remove the former
            if (CategoryMatch(cats, YearOfBirthMissing) && CategoryMatch(cats, YearOfBirthUncertain))
                articleText = RemoveCategory(YearOfBirthMissing, articleText);

            // if there's a year of death and a 'year of death missing', remove the latter
            if (Cat4YearDeaths.IsMatch(cats) && CategoryMatch(cats, YearofDeathMissing))
                articleText = RemoveCategory(YearofDeathMissing, articleText);

            return articleText;
        }

        // Covered by: LinkTests.TestFixCategories()
        /// <summary>
        /// Fix common spacing/capitalisation errors in categories; remove diacritics and trailing whitespace from sortkeys (not leading whitespace)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixCategories(string articleText)
        {
            CategoryStart = @"[[" + (Variables.Namespaces.ContainsKey(Namespace.Category) ? Variables.Namespaces[Namespace.Category] : "Category:");

            // Performance: only need to apply changes to portion of article containing categories
            Match cq = WikiRegexes.CategoryQuick.Match(articleText);

            if(cq.Success)
            {
                // Allow some characters before category start in case of excess opening braces
                int cutoff = Math.Max(0, cq.Index-2);
                string cats = articleText.Substring(cutoff);
                string catsOriginal = cats;

                // fix extra brackets: three or more at end
                cats = Regex.Replace(cats, @"(" + Regex.Escape(CategoryStart) + @"[^\r\n\[\]{}<>]+\]\])\]+", "$1");
                // three or more at start
                cats = Regex.Replace(cats, @"\[+(?=" + Regex.Escape(CategoryStart) + @"[^\r\n\[\]{}<>]+\]\])", "");

                cats = WikiRegexes.LooseCategory.Replace(cats, LooseCategoryME);

                // Performance: return original text if no changes
                if(cats.Equals(catsOriginal))
                    return articleText;

                articleText = articleText.Substring(0, cutoff) + cats;
            }
            
            return articleText;
        }

        private static string LooseCategoryME(Match m)
        {
            if (!Tools.IsValidTitle(m.Groups[1].Value))
                return m.Value;

            string sortkey = m.Groups[2].Value;

            if(!string.IsNullOrEmpty(sortkey))
            {
                // diacritic removal in sortkeys on en-wiki/simple-wiki only
                if(Variables.LangCode.Equals("en") || Variables.LangCode.Equals("simple"))
                    sortkey = Tools.CleanSortKey(sortkey);

                sortkey = WordWhitespaceEndofline.Replace(sortkey, "$1");
            }

            return CategoryStart + Tools.TurnFirstToUpper(CanonicalizeTitleRaw(m.Groups[1].Value, false).Trim().TrimStart(':')) + sortkey + "]]";
        }
    }
}
