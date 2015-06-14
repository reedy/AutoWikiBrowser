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
	}
}
