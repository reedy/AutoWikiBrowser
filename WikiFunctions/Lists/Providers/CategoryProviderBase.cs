/*
Copyright (C) 2008 Max Semenik, Sam Reed

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

using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace WikiFunctions.Lists.Providers
{
    public abstract class CategoryProviderBase : ApiListProviderBase
    {
        #region Overrides: <categorymembers>/<cm>
        readonly List<string> pe = new List<string>(new [] { "cm" });
        protected override ICollection<string> PageElements
        {
            get { return pe; }
        }

        readonly List<string> ac = new List<string>(new[] { "categorymembers" });
        protected override ICollection<string> Actions
        {
            get { return ac; }
        }

        public override string UserInputTextBoxText
        {
            get
            {
                string value;
                if (Variables.Namespaces.TryGetValue(Namespace.Category, out value))
                {
                    return value;
                }
                else
                {
                    return Variables.CanonicalNamespaces[Namespace.Category];
                }
            }
        }

        public override void Selected() { }

        public override bool UserInputTextBoxEnabled
        { get { return true; } }

        #endregion

        /// <summary>
        /// Gets the content of the given categor(y/ies)
        /// </summary>
        /// <param name="category">Category name. Must be WITHOUT the Category: prefix</param>
        /// <param name="haveSoFar">Number of pages already retrieved, for upper limit control</param>
        /// <returns>List of pages</returns>
        public List<Article> GetListing(string category, int haveSoFar)
        {
            string title = HttpUtility.UrlEncode(category);

            string url = "&list=categorymembers&cmtitle=Category:" + title + "&cmlimit=max";

            return ApiMakeList(url, 0);
        }

        public List<Article> GetListing(string category)
        {
            return GetListing(category, 0);
        }

        protected readonly List<string> Visited = new List<string>();

        public List<Article> RecurseCategory(string category, int haveSoFar, int depth)
        {
            if (haveSoFar > Limit || depth < 0) return new List<Article>();

            // normalise category name
            category = Tools.TurnFirstToUpper(Tools.WikiDecode(category));
            if (!Visited.Contains(category))
                Visited.Add(category);
            else
                return new List<Article>();

            List<Article> list = GetListing(category, haveSoFar);

            List<Article> fromSubcats = null;
            if (depth > 0 && haveSoFar + list.Count < Limit)
            {
                foreach (Article pg in list)
                {
                    if (haveSoFar + list.Count > Limit) break;

                    if (pg.NameSpaceKey == Namespace.Category && !Visited.Contains(pg.Name))
                    {
                        if (fromSubcats == null) fromSubcats = new List<Article>();
                        fromSubcats.AddRange(RecurseCategory(pg.NamespacelessName, haveSoFar + list.Count, depth - 1));
                    }
                }
            }
            if (fromSubcats != null && fromSubcats.Count > 0) list.AddRange(fromSubcats);

            return list;
        }

        /// <summary>
        /// Normalises category names, removes Category: prefix
        /// </summary>
        /// <param name="source">List of category names</param>
        public static IEnumerable<string> PrepareCategories(IEnumerable<string> source)
        {
            List<string> cats = new List<string>();

            foreach (string cat in source)
            {
                cats.Add(Regex.Replace(Tools.RemoveHashFromPageTitle(Tools.WikiDecode(cat)).Trim(),
                                       "^" + Variables.NamespacesCaseInsensitive[Namespace.Category], "").Trim());
            }

            return cats;
        }

        public override bool StripUrl
        {
            get { return true; }
        }
    }
}
