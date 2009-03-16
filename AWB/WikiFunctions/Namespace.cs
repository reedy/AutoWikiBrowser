/*

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
using System.Collections.Generic;

namespace WikiFunctions
{
    /// <summary>
    /// Contains constants for canonical namespace numbers
    /// </summary>
    public static class Namespace
    {
        public static readonly int Media = -2;
        public static readonly int Special = -1;
        public static readonly int Article = 0;
        public static readonly int Talk = 1;
        public static readonly int User = 2;
        public static readonly int UserTalk = 3;
        public static readonly int Project = 4;
        public static readonly int ProjectTalk = 5;
        public static readonly int File = 6;
        public static readonly int FileTalk = 7;
        public static readonly int MediaWiki = 8;
        public static readonly int MediaWikiTalk = 9;
        public static readonly int Template = 10;
        public static readonly int TemplateTalk = 11;
        public static readonly int Help = 12;
        public static readonly int HelpTalk = 13;
        public static readonly int Category = 14;
        public static readonly int CategoryTalk = 15;

        public static readonly int FirstCustom = 100;
        public static readonly int FirstCustomTalk = 101;

        // Aliases

        public static readonly int Mainspace = Article;
        public static readonly int Image = File;
        public static readonly int ImageTalk = FileTalk;

        // Covered by: NamespaceTests.Determine
        /// <summary>
        /// Deduces the namespace number.
        /// TODO: doesn't recognise acceptable deviations such as "template:foo" or "Category : bar"
        /// </summary>
        public static int Determine(string ArticleTitle)
        {
            if (!ArticleTitle.Contains(":"))
                return 0;

            foreach (KeyValuePair<int, string> k in Variables.Namespaces)
            {
                if (ArticleTitle.StartsWith(k.Value))
                    return ArticleTitle.Length > k.Value.Length ? k.Key : 0;
            }

            foreach (KeyValuePair<int, List<string>> k in Variables.NamespaceAliases)
            {
                foreach (string s in k.Value)
                {
                    if (ArticleTitle.StartsWith(s))
                        return ArticleTitle.Length > s.Length ? k.Key : 0;
                }
            }

            foreach (KeyValuePair<int, string> k in Variables.CanonicalNamespaces)
            {
                if (ArticleTitle.StartsWith(k.Value))
                    return ArticleTitle.Length > k.Value.Length ? k.Key : 0;
            }

            return 0;
        }

        // Covered by NamespaceTests.IsMainSpace()
        /// <summary>
        /// Tests title to make sure it is main space
        /// </summary>
        /// <param name="ArticleTitle">The title.</param>
        public static bool IsMainSpace(string ArticleTitle)
        {
            return Determine(ArticleTitle) == 0;
        }

        /// <summary>
        /// Tests title to make sure it is main space
        /// </summary>
        public static bool IsMainSpace(int ns)
        {
            return ns == Article;
        }

        // Covered by NamespaceTests.IsImportant()
        /// <summary>
        /// Tests title to make sure it is either main, image, category or template namespace.
        /// </summary>
        /// <param name="ArticleTitle">The title.</param>
        public static bool IsImportant(string ArticleTitle)
        {
            return IsImportant(Determine(ArticleTitle));
        }

        /// <summary>
        /// Tests title to make sure it is either main, image, category or template namespace.
        /// </summary>
        /// <param name="key">Namespace key</param>
        public static bool IsImportant(int key)
        {
            return (key == Article || key == File
                || key == Template || key == Category);
        }

        // Covered by NamespaceTests.IsTalk()
        /// <summary>
        /// Tests title to make sure it is a talk page.
        /// </summary>
        /// <param name="ArticleTitle">The title.</param>
        public static bool IsTalk(string ArticleTitle)
        {
            return IsTalk(Determine(ArticleTitle));
        }

        // Covered by NamespaceTests.IsTalk()
        /// <summary>
        /// Tests title to make sure it is a talk page.
        /// </summary>
        /// <param name="key">The namespace key</param>
        public static bool IsTalk(int key)
        {
            return (key % 2 == 1);
        }

        // Covered by NamespaceTests.IsUserSpace()
        /// <summary>
        /// returns true if current page is a userpage
        /// </summary>
        public static bool IsUserSpace(string ArticleTitle)
        {
            return IsUserTalk(ArticleTitle) || IsUserPage(ArticleTitle);
        }

        // Covered by NamespaceTests.IsUserTalk()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleTitle"></param>
        /// <returns></returns>
        public static bool IsUserTalk(string ArticleTitle)
        {
            return ArticleTitle.StartsWith(Variables.Namespaces[UserTalk]);
        }

        // Covered by NamespaceTests.IsUserPage()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleTitle"></param>
        /// <returns></returns>
        public static bool IsUserPage(string ArticleTitle)
        {
            return Determine(ArticleTitle) == User;
        }

        private static readonly Regex NormalizeColon = new Regex(@"\s*:$", RegexOptions.Compiled);

        // Covered by NamespaceTests.NormalizeNamespace()
        /// <summary>
        /// Normalizes a namespace string, but does not changes it to default namespace name
        /// </summary>
        public static string Normalize(string ns, int nsId)
        {
            ns = Tools.WikiDecode(NormalizeColon.Replace(ns, ":"));
            if (Variables.Namespaces[nsId].Equals(ns, StringComparison.InvariantCultureIgnoreCase))
                return Variables.Namespaces[nsId];

            foreach (string s in Variables.NamespaceAliases[nsId])
            {
                if (s.Equals(ns, StringComparison.InvariantCultureIgnoreCase))
                    return s;
            }

            // fail
            return ns;
        }
    }
}