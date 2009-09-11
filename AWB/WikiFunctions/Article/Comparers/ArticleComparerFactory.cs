/*

Copyright (C) 2009

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
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace WikiFunctions
{
    /// <summary>
    /// Factory class for making instances of IArticleComparer
    /// </summary>
    public static class ArticleComparerFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="comparator">The test string</param>
        /// <param name="isCaseSensitive">Whether the comparison should be case sensitive</param>
        /// <param name="isRegex">Whether to employ regular expression matching when comparing the test string</param>
        /// <param name="isSingleLine">Whether to apply the regular expression Single Line option</param>
        /// <param name="isMultiLine">Whether to apply the regular expression Multi Line option</param>
        /// <returns>An instance of IArticleComparer which can carry out the specified comparison</returns>
        public static IArticleComparer Create(string comparator, bool isCaseSensitive, bool isRegex, bool isSingleLine, bool isMultiLine)
        {
            if (comparator == null)
                throw new ArgumentNullException("comparator");

            if (isRegex)
            {
                try
                {
                    RegexOptions opts = RegexOptions.None;
                    if (!isCaseSensitive)
                        opts |= RegexOptions.IgnoreCase;

                    if (isSingleLine)
                        opts |= RegexOptions.Singleline;

                    if (isMultiLine)
                        opts |= RegexOptions.Multiline;

                    return comparator.Contains("%%")
                               ? (IArticleComparer)new DynamicRegexArticleComparer(comparator, opts)
                               : new RegexArticleComparer(new Regex(comparator, opts | RegexOptions.Compiled));
                }
                catch (ArgumentException ex)
                {
                    //TODO: handle things like "bad regex" here
                    // For now, tell the user then let normal exception handling process it as well
                    MessageBox.Show(ex.Message, "Bad Regex");
                    throw;
                }
            }

            if (comparator.Contains("%%"))
                return isCaseSensitive
                    ? (IArticleComparer)new CaseSensitiveArticleComparerWithKeywords(comparator)
                    : new CaseInsensitiveArticleComparerWithKeywords(comparator);

            return isCaseSensitive
                       ? (IArticleComparer)new CaseSensitiveArticleComparer(comparator)
                       : new CaseInsensitiveArticleComparer(comparator);
        }
    }

    /// <summary>
    /// Class for scanning an article for content
    /// </summary>
    public interface IArticleComparer
    {
        /// <summary>
        /// Compares the article text against the criteria provided 
        /// </summary>
        /// <param name="article">An article to check</param>
        /// <returns>Whether the article's text matches the criteria</returns>
        bool Matches(Article article);
    }
}