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

namespace WikiFunctions
{
    /// <summary>
    /// 
    /// </summary>
    public class CaseSensitiveArticleComparer : IArticleComparer
    {
        public CaseSensitiveArticleComparer(string comparator)
        {
            Comparator = comparator;
        }

        public bool Matches(Article article)
        {
            return article.ArticleText.Contains(Comparator);
        }

        private readonly string Comparator;
    }

    /// <summary>
    /// 
    /// </summary>
    public class CaseInsensitiveArticleComparer : IArticleComparer
    {
        public CaseInsensitiveArticleComparer(string comparator)
        {
            Comparator = comparator;
        }

        public bool Matches(Article article)
        {
            return article.ArticleText.IndexOf(Comparator, StringComparison.CurrentCultureIgnoreCase) >= 0;
            // or should that be OrdinalIgnoreCase?
        }

        private readonly string Comparator;
    }

    /// <summary>
    /// 
    /// </summary>
    public class CaseSensitiveArticleComparerWithKeywords : IArticleComparer
    {
        public CaseSensitiveArticleComparerWithKeywords(string comparator)
        {
            Comparator = comparator;
        }

        public bool Matches(Article article)
        {
            return article.ArticleText.Contains(Tools.ApplyKeyWords(article.Name, Comparator));
        }

        private readonly string Comparator;
    }

    /// <summary>
    /// 
    /// </summary>
    public class CaseInsensitiveArticleComparerWithKeywords : IArticleComparer
    {
        public CaseInsensitiveArticleComparerWithKeywords(string comparator)
        {
            Comparator = comparator;
        }

        public bool Matches(Article article)
        {
            return article.ArticleText.IndexOf(Tools.ApplyKeyWords(article.Name, Comparator), StringComparison.CurrentCultureIgnoreCase) >= 0;
            // or should that be OrdinalIgnoreCase?
        }

        private readonly string Comparator;
    }
}