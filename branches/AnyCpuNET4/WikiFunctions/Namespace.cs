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
using System.Collections.ObjectModel;

namespace WikiFunctions
{
    /// <summary>
    /// Contains constants for canonical namespace numbers
    /// </summary>
    public static class Namespace
    {
        public const int Media = -2;
        public const int Special = -1;
        public const int Article = 0;
        public const int Talk = 1;
        public const int User = 2;
        public const int UserTalk = 3;
        public const int Project = 4;
        public const int ProjectTalk = 5;
        public const int File = 6;
        public const int FileTalk = 7;
        public const int MediaWiki = 8;
        public const int MediaWikiTalk = 9;
        public const int Template = 10;
        public const int TemplateTalk = 11;
        public const int Help = 12;
        public const int HelpTalk = 13;
        public const int Category = 14;
        public const int CategoryTalk = 15;

        public const int FirstCustom = 100;
        public const int FirstCustomTalk = 101;

        // Aliases

        public static readonly int Mainspace = Article;
        public static readonly int Image = File;
        public static readonly int ImageTalk = FileTalk;

        /// <summary>
        /// List of namespaces that we expect to be present in every wiki.
        /// For compatibility with our practice with Variables.Namespaces et al, mainspace is not present.
        /// </summary>
        public static ReadOnlyCollection<int> StandardNamespaces
        { get; private set; }
        
        static Namespace()
        {
            var ns = new List<int>();
            ns.Add(Media);
            ns.Add(Special);
            for (int i = Talk; i <= CategoryTalk; i++)
                ns.Add(i);
            StandardNamespaces = new ReadOnlyCollection<int>(ns);
        }

        // Covered by: NamespaceTests.Determine
        /// <summary>
        /// Deduces the namespace number from the input article title.
        /// </summary>
        public static int Determine(string articleTitle)
        {
            articleTitle = Tools.TurnFirstToUpper(Regex.Replace(articleTitle, @"\s*:\s*", ":"));
            
            articleTitle = Tools.WikiDecode(articleTitle);
            
            if (!articleTitle.Contains(":"))
                return 0;

            foreach (KeyValuePair<int, string> k in Variables.Namespaces)
            {
                if (articleTitle.StartsWith(k.Value))
                    return articleTitle.Length >= k.Value.Length ? k.Key : 0;
            }

            foreach (KeyValuePair<int, List<string>> k in Variables.NamespaceAliases)
            {
                foreach (string s in k.Value)
                {
                    if (articleTitle.StartsWith(s))
                        return articleTitle.Length >= s.Length ? k.Key : 0;
                }
            }

            foreach (KeyValuePair<int, string> k in Variables.CanonicalNamespaces)
            {
                if (articleTitle.StartsWith(k.Value))
                    return articleTitle.Length >= k.Value.Length ? k.Key : 0;
            }

            return 0;
        }

        // Covered by NamespaceTests.IsMainSpace()
        /// <summary>
        /// Tests title to make sure it is main space
        /// </summary>
        /// <param name="articleTitle">The title.</param>
        public static bool IsMainSpace(string articleTitle)
        {
            return Determine(articleTitle) == 0;
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
        /// <param name="articleTitle">The title.</param>
        public static bool IsImportant(string articleTitle)
        {
            return IsImportant(Determine(articleTitle));
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
        /// <param name="articleTitle">The title.</param>
        public static bool IsTalk(string articleTitle)
        {
            return IsTalk(Determine(articleTitle));
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
        public static bool IsUserSpace(string articleTitle)
        {
            return IsUserTalk(articleTitle) || IsUserPage(articleTitle);
        }

        // Covered by NamespaceTests.IsUserTalk()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleTitle">Title of the article</param>
        /// <returns></returns>
        public static bool IsUserTalk(string articleTitle)
        {
            return articleTitle.StartsWith(Variables.Namespaces[UserTalk]);
        }

        // Covered by NamespaceTests.IsUserPage()
        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleTitle">Title of the article</param>
        /// <returns></returns>
        public static bool IsUserPage(string articleTitle)
        {
            return Determine(articleTitle) == User;
        }

        // Covered by NamespaceTests.IsSpecial()
        /// <summary>
        /// Returns true if the given namespace is non-editable, i.e. Special or Media
        /// </summary>
        public static bool IsSpecial(int ns)
        {
            return ns < 0;
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

        /// <summary>
        /// Checks if given namespaces are sufficient for AWB to function properly and 
        /// that their format is expected.
        /// </summary>
        /// <param name="namespaces">namespaces to verify</param>
        /// <returns>true if the namespace list is valid</returns>
        public static bool VerifyNamespaces(Dictionary<int, string> namespaces)
        {
            foreach (var ns in StandardNamespaces)
            {
                if (!namespaces.ContainsKey(ns)) return false;
            }

            if (namespaces.ContainsKey(Mainspace)) return false;

            foreach (var s in namespaces.Values)
            {
                if (s.Length < 2 || !s.EndsWith(":"))
                    return false;
            }

            return true;
        }
    }
}