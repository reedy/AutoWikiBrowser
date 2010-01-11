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

using System.Text.RegularExpressions;

namespace WikiFunctions
{
    /// <summary>
    /// 
    /// </summary>
    public class RegexArticleComparer : IArticleComparer
    {
        public RegexArticleComparer(Regex comparator)
        {
            Comparator = comparator;
        }

        public bool Matches(Article article)
        {
            return Comparator.IsMatch(article.ArticleText);
        }

        private readonly Regex Comparator;
    }

    /// <summary>
    /// 
    /// </summary>
    public class DynamicRegexArticleComparer : IArticleComparer
    {
        public DynamicRegexArticleComparer(string comparator, RegexOptions options)
        {
            Comparator = comparator;
            Options = (options & ~RegexOptions.Compiled);
            // Create a regex to try it out. Throws an exception if there's a regex error
            new Regex(Tools.ApplyKeyWords("a", comparator), options);
        }

        public bool Matches(Article article)
        {
            return Regex.IsMatch(article.ArticleText, Tools.ApplyKeyWords(article.Name, Comparator), Options);
        }

        private readonly string Comparator;
        private readonly RegexOptions Options;
    }
}