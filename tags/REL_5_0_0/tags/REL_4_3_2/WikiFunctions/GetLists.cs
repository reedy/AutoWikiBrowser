/*
WikiFunctions
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
using System.Text;
using System.Net;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Threading;

// TODO: Plenty here could be encapsulated in Article.cs

namespace WikiFunctions.Lists
{
    /// <summary>
    /// Provides functionality to create and manipulate Lists of articles from many different sources
    /// </summary>
    public static class GetLists
    {
        /// <summary>
        /// whether errors shoud be ignored without informing user
        /// </summary>
        public static bool QuietMode;

        public enum From
        {
            Category,
            CategoryRecursive,
            Whatlinkshere,
            WhatTranscludesHere,
            WhatTranscludesThis,
            Redirects,
            ImagesOnPage,
            TextFile,
            Google,
            UserContribs,
            AllUserContribs,
            SpecialPage,
            ImageLinks,
            Watchlist,
            WikiSearch,
            Listusers
        }

        readonly static Regex regexli = new Regex("<li>.*</li>", RegexOptions.Compiled);
        readonly static Regex regexe = new Regex("<li>\\(?<a href=\"[^\"]*\" title=\"([^\"]*)\">[^<>]*</a> \\(redirect page\\)", RegexOptions.Compiled);
        readonly static Regex regexe2 = new Regex("<a href=\"[^\"]*\" title=\"([^\"]*)\">[^<>]*</a>", RegexOptions.Compiled);
        readonly static Regex regexLog = new Regex(@"<li>.*?<a .*?</a> \(<a .*?</a>\).*?<a href=""[^""]*""[^>]* title=""([^""]*)"">[^<>]*</a>", RegexOptions.Compiled);
        readonly static Regex RegexFromFile = new Regex("(^[a-z]{2,3}:)|(simple:)", RegexOptions.Compiled);

        #region From variant
        /// <summary>
        /// Gets a list of pages from any supported kind of source
        /// </summary>
        /// <param name="What">Which source to use</param>
        /// <param name="Limit">Max. number of pages to return, -1 if no limit</param>
        /// <param name="Params">Optional parameters, depend on source</param>
        /// <returns>The list of pages</returns>
        public static List<Article> FromVariant(From what, int limit, params string[] params1)
        {
            switch (what)
            {
                case From.Category:
                    return FromCategory(false, params1);
                case From.CategoryRecursive:
                    return FromCategory(true, params1);
                case From.Google:
                    return FromGoogleSearch(params1);
                case From.ImageLinks:
                    return FromImageLinks(params1);
                case From.ImagesOnPage:
                    return FromImagesOnPage(params1);
                case From.WhatTranscludesThis:
                    return FromTransclusionsOnPage(params1);
                case From.Listusers:
                    return FromListUsers(params1[0], params1[1], limit);
                case From.Redirects:
                    return FromRedirects(params1);
                case From.SpecialPage:
                    return FromSpecialPage(limit, params1);
                case From.TextFile:
                    return FromTextFile(params1);
                case From.UserContribs:
                    return FromUserContribs(params1);
                case From.AllUserContribs:
                    return FromUserContribs(true, params1);
                case From.Watchlist:
                    return FromWatchList();
                case From.Whatlinkshere:
                    return FromWhatLinksHere(false, params1);
                case From.WhatTranscludesHere:
                    return FromWhatLinksHere(true, params1);
                case From.WikiSearch:
                    return FromWikiSearch(params1);
                default:
                    throw new Exception("Invalid argument to GetLists.FromVariant()");
            }
        }

        /// <summary>
        /// Gets a list of pages from any supported kind of source
        /// </summary>
        /// <param name="What">Which source to use</param>
        /// <param name="Params">Optional parameters, depend on source</param>
        /// <returns>The list of pages</returns>
        public static List<Article> FromVariant(From what, params string[] params1)
        {
            return FromVariant(what, -1, params1);
        }
        #endregion

        #region From category

        /// <summary>
        /// Gets a list of articles and sub-categories in a category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="subCategories">Whether to get all sub categories as well.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromCategory(bool subCategories, params string[] categories)
        {
            List<Article> list = new List<Article>();
            List<string> badcategories = new List<string>();
            List<string> vistedCategories = new List<string>();

            for (int i = 0; i < categories.Length; i++)
            {
                if (!vistedCategories.Contains(categories[i]))
                {
                    vistedCategories.Add(categories[i]);
                    string cmtitle = encodeText(Regex.Replace(categories[i], Variables.NamespacesCaseInsensitive[14], ""));

                    string url = Variables.URLLong + "api.php?action=query&list=categorymembers&cmtitle=Category:" + cmtitle + "&cmcategory=" + cmtitle + "&format=xml&cmlimit=500";
                    int ns = 0;

                    while (true)
                    {
                        string title = "";
                        string html = Tools.GetHTML(url);
                        if (html.Contains("categorymembers /"))
                        {
                            badcategories.Add(categories[i]);
                            break;
                        }
                        bool more = false;

                        using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                        {
                            while (reader.Read())
                            {
                                if (reader.Name.Equals("cm"))
                                {
                                    if (reader.MoveToAttribute("ns"))
                                        ns = int.Parse(reader.Value);
                                    else
                                        ns = 0;

                                    if (reader.MoveToAttribute("title"))
                                    {
                                        title = reader.Value.ToString();
                                        list.Add(new WikiFunctions.Article(title, ns));
                                    }

                                    if (subCategories && ns == 14)
                                    {
                                        Array.Resize<string>(ref categories, categories.Length + 1);
                                        categories[categories.Length - 1] = title.Replace(Variables.Namespaces[14], "");
                                    }
                                }
                                else if (reader.Name.Equals("categorymembers"))
                                {
                                    reader.MoveToAttribute("cmcontinue");
                                    if (reader.Value.Length > 0)
                                    {
                                        string continueFrom = Tools.WikiEncode(reader.Value.ToString());
                                        url = Variables.URLLong + "api.php?action=query&list=categorymembers&cmtitle=Category:" + cmtitle + "&cmcategory=" + cmtitle + "&format=xml&cmlimit=500&cmcontinue=" + continueFrom;
                                        more = true;
                                    }
                                }
                            }
                        }
                        if (!more)
                            break;
                    }
                }
            }
            if (badcategories.Count != 0 && !QuietMode)
            {
                StringBuilder errorMessage = new StringBuilder("The following Categories are empty or do not exist:");

                foreach (string badcat in badcategories)
                    errorMessage.AppendLine(" ● " + badcat);

                MessageBox.Show(errorMessage.ToString());
            }
            return list;
        }
        #endregion

        #region From whatlinkshere

        /// <summary>
        /// Gets a list of articles that link to the given page.
        /// </summary>
        /// <param name="embedded">Gets articles that embed (transclude).</param>
        /// <param name="pages">The page to find links to.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromWhatLinksHere(bool embedded, params string[] pages)
        {
            string request = "backlinks";
            string initial = "bl";
            if (embedded)
            {
                request = "embeddedin";
                initial = "ei";
            }
            List<Article> list = new List<Article>();

            foreach (string page in pages)
            {
                if (page.Trim().Length == 0) continue;
                string url = Variables.URLLong + "api.php?action=query&list=" + request + "&" + initial + "title=" + Tools.RemoveHashFromPageTitle(encodeText(page)) + "&format=xml&" + initial + "limit=500";
                string title = "";
                int ns = 0;

                while (true)
                {
                    string html = Tools.GetHTML(url);
                    bool more = false;

                    using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name.Equals(initial))
                            {
                                if (reader.MoveToAttribute("ns"))
                                    ns = int.Parse(reader.Value);
                                else
                                    ns = 0;

                                if (reader.MoveToAttribute("title"))
                                {
                                    title = reader.Value;
                                    list.Add(new WikiFunctions.Article(title, ns));
                                }
                            }
                            else if (reader.Name.Equals(request))
                            {
                                reader.MoveToAttribute(initial + "continue");
                                if (reader.Value.Length != 0)
                                {
                                    string continueFrom = reader.Value;
                                    url = Variables.URLLong + "api.php?action=query&list=" + request + "&" + initial + "title=" + encodeText(page) + "&format=xml&" + initial + "limit=500&" + initial + "continue=" + continueFrom;
                                    more = true;
                                }
                            }
                        }
                    }
                    if (!more)
                        break;
                }
            }
            return list;
        }

        #endregion

        #region From redirects

        /// <summary>
        /// Gets a list of articles that redirect to the given page.
        /// </summary>
        /// <param name="pages">The pages to find redirects to.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromRedirects(params string[] pages)
        {
            return FromRedirects(true, pages);
        }

        /// <summary>
        /// Gets a list of articles that redirects to the given page.
        /// </summary>
        /// <param name="Pages">The pages to find redirects to.</param>
        /// <returns>The list of the articles.</returns>
        /// <param name="HandleErrors">Handle errors, or let them filter down to caller</param>
        public static List<Article> FromRedirects(bool HandleErrors, params string[] pages) // HandleErrors param is used by kingbotk plugin
        {
            List<Article> list = new List<Article>();

            foreach (string onePage in pages)
            {
                try
                {
                    string title = "";
                    int ns = 0;
                    string page = onePage;
                    page = encodeText(page);
                    string url = Variables.URLLong + "api.php?action=query&list=backlinks&bltitle=" + page + "&bllimit=500&blfilterredir=redirects&format=xml";

                    while (true)
                    {
                        bool more = false;
                        string html = Tools.GetHTML(url);

                        using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                        {
                            while (reader.Read())
                            {
                                if (reader.Name.Equals("bl"))
                                {
                                    if (reader.MoveToAttribute("ns"))
                                        ns = int.Parse(reader.Value);
                                    else
                                        ns = 0;

                                    if (reader.MoveToAttribute("title"))
                                    {
                                        title = reader.Value.ToString();
                                        list.Add(new WikiFunctions.Article(title, ns));
                                    }
                                }
                                else if (reader.Name.Equals("backlinks"))
                                {
                                    reader.MoveToAttribute("blcontinue");
                                    if (reader.Value.Length != 0)
                                    {
                                        string continueFrom = reader.Value;
                                        url = Variables.URLLong + "api.php?action=query&list=backlinks&bltitle=" + page + "&bllimit=500&blfilterredir=redirects&format=xml&blcontinue=" + continueFrom;
                                        more = true;
                                    }
                                }
                            }
                            if (!more)
                                break;
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    if (HandleErrors)
                        ErrorHandler.Handle(ex);
                    else
                        throw;
                }
            }
            return list;
        }

        #endregion

        #region From links on page

        /// <summary>
        /// Gets a list of links on a page.
        /// </summary>
        /// <param name="articles">The page to find links on.</param>
        /// <returns>The list of the links.</returns>
        public static List<Article> FromLinksOnPage(params string[] articles)
        {
            List<Article> list = new List<Article>();

            foreach (string article in articles)
            {
                try
                {
                    string url = Variables.URLLong + "api.php?action=query&prop=links&titles=" + encodeText(article) + "&format=xml";
                    string title = "";
                    int ns = 0;

                    while (true)
                    {
                        string html = Tools.GetHTML(url);
                        bool more = false;

                        using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                        {
                            while (reader.Read())
                            {
                                if (reader.Name.Equals("pl"))
                                {
                                    if (reader.MoveToAttribute("ns"))
                                        ns = int.Parse(reader.Value);
                                    else
                                        ns = 0;

                                    if (reader.MoveToAttribute("title"))
                                    {
                                        title = reader.Value.ToString();
                                        list.Add(new WikiFunctions.Article(title, ns));
                                    }
                                }
                            }
                        }
                        if (!more)
                            break;
                    }
                }
                catch (ThreadAbortException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    ErrorHandler.Handle(ex);
                }
            }

            return list;
        }

        #endregion

        #region Images on page

        /// <summary>
        /// Gets a list of Images on a page.
        /// </summary>
        /// <param name="articles">The page to find images on.</param>
        /// <returns>The list of the images.</returns>
        public static List<Article> FromImagesOnPage(params string[] articles)
        {
            List<Article> list = new List<Article>();

            foreach (string article in articles)
            {
                string url = Variables.URLLong + "api.php?action=query&prop=images&titles=" + encodeText(article) + "&format=xml";

                while (true)
                {
                    string html = Tools.GetHTML(url);
                    bool more = false;
                    int ns;
                    string title = "";

                    using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name.Equals("im"))
                            {
                                if (reader.MoveToAttribute("ns"))
                                    ns = int.Parse(reader.Value);
                                else
                                    ns = 0;

                                if (reader.MoveToAttribute("title"))
                                {
                                    title = reader.Value.ToString();
                                    list.Add(new WikiFunctions.Article(title, ns));
                                }
                            }
                        }
                    }
                    if (!more)
                        break;
                }
            }
            return list;
        }

        #endregion

        #region Transclusions on Page

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articles"></param>
        /// <returns></returns>
        public static List<Article> FromTransclusionsOnPage(params string[] articles)
        {
            List<Article> list = new List<Article>();

            foreach (string article in articles)
            {
                string url = Variables.URLLong + "api.php?action=query&prop=templates&titles=" + encodeText(article) + "&format=xml";

                while (true)
                {
                    string html = Tools.GetHTML(url);
                    bool more = false;
                    int ns;
                    string title = "";

                    using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name.Equals("tl"))
                            {
                                if (reader.MoveToAttribute("ns"))
                                    ns = int.Parse(reader.Value);
                                else
                                    ns = 0;

                                if (reader.MoveToAttribute("title"))
                                {
                                    title = reader.Value.ToString();
                                    list.Add(new WikiFunctions.Article(title, ns));
                                }
                            }
                        }
                    }
                    if (!more)
                        break;
                }
            }
            return list;
        }

        #endregion

        #region From text file

        static readonly Regex LoadWikiLink = new Regex(@"\[\[:?(.*?)(?:\]\]|\|)", RegexOptions.Compiled);

        /// <summary>
        /// Gets a list of pages from text file
        /// </summary>
        /// <param name="fileNames">The file path of the list.</param>
        /// <returns>The list of the links.</returns>
        public static List<Article> FromTextFile(params string[] fileNames)
        {
            List<Article> list = new List<Article>();

            foreach (string fileName in fileNames)
            {
                string pageText = "";
                string title = "";

                using (StreamReader sr = new StreamReader(fileName, Encoding.Default))
                {
                    pageText = sr.ReadToEnd();
                    sr.Close();
                }

                if (LoadWikiLink.IsMatch(pageText))
                {
                    foreach (Match m in LoadWikiLink.Matches(pageText))
                    {
                        title = m.Groups[1].Value;
                        if (!RegexFromFile.IsMatch(title) && (!(title.StartsWith("#"))))
                        {
                            list.Add(new WikiFunctions.Article(Tools.RemoveSyntax(Tools.TurnFirstToUpper(title))));
                        }
                    }
                }
                else
                {
                    foreach (string s in pageText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (s.Trim().Length == 0 || !Tools.IsValidTitle(s)) continue;
                        list.Add(new WikiFunctions.Article(Tools.RemoveSyntax(Tools.TurnFirstToUpper(s.Trim()))));
                    }
                }
            }

            return list;
        }

        #endregion

        #region From google

        /// <summary>
        /// Gets a list from a google search of the site.
        /// </summary>
        /// <param name="googles">The term to search for.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromGoogleSearch(params string[] googles)
        {
            List<Article> list = new List<Article>();

            foreach (string g in googles)
            {
                int intStart = 0;
                string google = encodeText(g);
                google = google.Replace("_", " ");
                string url = "http://www.google.com/search?q=" + google + "+site:" + Variables.URL + "&num=100&hl=en&lr=&start=0&sa=N&filter=0";
                string title = "";

                //Regex pattern to find links
                Regex regexGoogle = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+) class=l)",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);

                do
                {
                    string googleText = Tools.GetHTML(url, Encoding.Default);

                    //Find each match to the pattern
                    foreach (Match m in regexGoogle.Matches(googleText))
                    {
                        title = m.Groups[1].Value;
                        if (!title.StartsWith(Variables.URL + "/wiki/")) continue;

                        title = title.Remove(0, (Variables.URL + "/wiki/").Length);

                        title = HttpUtility.UrlDecode(title).Replace('_', ' ');

                        list.Add(new WikiFunctions.Article(title));
                    }

                    if (googleText.Contains("img src=\"nav_next.gif\""))
                    {
                        intStart += 100;
                        url = "http://www.google.com/search?q=" + google + "+site:" + Variables.URL + "&num=100&hl=en&lr=&start=" + intStart.ToString() + "&sa=N&filter=0";
                    }
                    else
                        break;

                } while (true);
            }
            return FilterSomeArticles(list);
        }

        #endregion

        #region From user contributions

        /// <summary>
        /// Gets a list from a users contribs.
        /// </summary>
        /// <param name="users">The name of the user.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromUserContribs(params string[] users)
        {
            return FromUserContribs(false, users);
        }

        /// <summary>
        /// Gets a list from a users contribs.
        /// </summary>
        /// <param name="all">Whether to load all contribs or not</param>
        /// <param name="users">The name of the user.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromUserContribs(bool all, params string[] users)
        {
            List<Article> list = new List<Article>();

            foreach (string u in users)
            {
                string title = "";
                int ns = 0;
                string page = u;
                page = encodeText(page);

                string url = Variables.URLLong + "api.php?action=query&list=usercontribs&ucuser=" + page + "&uclimit=500&format=xml";

                while (true)
                {
                    bool more = false;
                    string html = Tools.GetHTML(url);

                    using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name.Equals("item"))
                            {
                                if (reader.MoveToAttribute("ns"))
                                    ns = int.Parse(reader.Value);
                                else
                                    ns = 0;

                                if (reader.MoveToAttribute("title"))
                                {
                                    title = reader.Value.ToString();
                                    list.Add(new WikiFunctions.Article(title, ns));
                                }
                            }
                            else if (reader.Name.Equals("usercontribs"))
                            {
                                reader.MoveToAttribute("ucstart");
                                if (reader.Value.Length != 0 && (all || list.Count < 2000))
                                {
                                    string continueFrom = reader.Value;
                                    url = Variables.URLLong + "api.php?action=query&list=usercontribs&ucuser=" + page + "&uclimit=500&format=xml&ucstart=" + continueFrom;
                                    more = true;
                                }
                            }
                        }
                        if (!more)
                            break;
                    }
                }
            }

            return list;
        }

        #endregion

        #region From special page

        /// <summary>
        /// Gets a list of links on a special page.
        /// </summary>
        /// <param name="specials">The page to find links on, e.g. "Deadendpages" or "Deadendpages&limit=500&offset=0".</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromSpecialPage(params string[] specials)
        {
            return FromSpecialPage(-1, specials);
        }

        /// <summary>
        /// Gets a list of links on a special page.
        /// </summary>
        /// <param name="limit">The maximum number of results resulted.</param>
        /// <param name="specials">The page to find links on, e.g. "Deadendpages" or "Deadendpages&limit=500&offset=0".</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromSpecialPage(int limit, params string[] specials)
        {
            List<Article> list = new List<Article>();

            if (limit < 0) limit = 1000;

            foreach (string s in specials)
            {
                string special = Regex.Replace(s, "^" + Variables.NamespacesCaseInsensitive[-1], "", RegexOptions.IgnoreCase);

                string url = Variables.URLLong + "index.php?title=Special:" + special;
                if (!url.Contains("&limit=")) url += "&limit=" + limit.ToString();
                string pageText = Tools.GetHTML(url);

                pageText = Tools.StringBetween(pageText, "<!-- start content -->", "<!-- end content -->");
                string title = "";
                int ns = 0;

                if (Regex.IsMatch(s, @"^Log ?[/\?&]", RegexOptions.IgnoreCase) || !regexli.IsMatch(pageText))
                {
                    Regex r = regexe;
                    if (Regex.IsMatch(s, @"^Log ?[/\?&]", RegexOptions.IgnoreCase))
                        r = regexLog;
                    foreach (Match m in r.Matches(pageText))
                    {
                        title = m.Groups[1].Value.Trim();
                        if (title.Length == 0) continue;
                        if (title != "Wikipedia:Special pages" && title != "Wikipedia talk:Special:Lonelypages" && title != "Wikipedia:Offline reports" && title != "Template:Specialpageslist")
                        {
                            title = title.Replace("&amp;", "&").Replace("&quot;", "\"");
                            if (title.Length == 0)
                                continue;

                            ns = Tools.CalculateNS(title);
                            if (ns < 0) continue;
                            list.Add(new WikiFunctions.Article(title, ns));

                            if (limit >= 0 && list.Count >= limit)
                                break;
                        }
                    }
                }
                else
                {
                    foreach (Match m in regexli.Matches(pageText))
                    {
                        foreach (Match m2 in regexe2.Matches(m.Value))
                        {
                            if (m2.Value.Contains("&amp;action=") && !m2.Value.Contains("&amp;action=edit"))
                                continue;

                            title = m2.Groups[1].Value;

                            title = HttpUtility.HtmlDecode(title);

                            if (title.Trim().Length == 0) continue;

                            list.Add(new WikiFunctions.Article(title));

                            if (limit >= 0 && list.Count >= limit)
                                break;
                        }
                    }
                }
            }
            return FilterSomeArticles(list);
        }

        #endregion

        #region From image links

        /// <summary>
        /// Gets a list of articles that use an image.
        /// </summary>
        /// <param name="images">The image.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromImageLinks(params string[] images)
        {
            List<Article> list = new List<Article>();

            foreach (string i in images)
            {
                string image = Regex.Replace(i, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase);
                image = encodeText(image);

                string url = Variables.URLLong + "api.php?action=query&list=imageusage&iutitle=Image:" + image + "&iulimit=500&format=xml";
                string title = "";
                int ns = 0;

                while (true)
                {
                    string html = Tools.GetHTML(url);
                    bool more = false;

                    using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name.Equals("iu"))
                            {
                                if (reader.MoveToAttribute("ns"))
                                    ns = int.Parse(reader.Value);
                                else
                                    ns = 0;

                                if (reader.MoveToAttribute("title"))
                                {
                                    title = reader.Value.ToString();
                                    list.Add(new WikiFunctions.Article(title, ns));
                                }
                            }
                            else if (reader.Name.Equals("imageusage"))
                            {
                                reader.MoveToAttribute("iucontinue");
                                if (reader.Value.Length != 0)
                                {
                                    string continueFrom = reader.Value.ToString();
                                    url = Variables.URLLong + "api.php?action=query&list=imageusage&iutitle=Image:" + image + "&format=xml&iulimit=500&iucontinue=" + continueFrom;
                                    more = true;
                                }
                            }
                        }
                    }
                    if (!more)
                        break;
                }
            }
            return list;
        }

        #endregion

        #region From watchlist

        public static List<Article> FromWatchList()
        {
            WebBrowser webbrowser = new WebBrowser();
            webbrowser.ScriptErrorsSuppressed = true;
            webbrowser.Navigate(Variables.URLLong + "index.php?title=Special:Watchlist&action=raw");
            while (webbrowser.ReadyState != WebBrowserReadyState.Complete) Application.DoEvents();

            string html = webbrowser.Document.GetElementById("titles").InnerText;
            List<Article> list = new List<Article>();

            try
            {
                string[] splitter = { "\r\n" };
                foreach (string entry in html.Split(splitter, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (entry.Length > 0)
                        list.Add(new Article(entry));
                }
            }
            catch { }
            return list;
        }

        #endregion

        #region From wiki search
        /// <summary>
        /// Gets a list from wiki's internal search
        /// </summary>
        /// <param name="terms">The terms to search for.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromWikiSearch(params string[] terms)
        {
            //TODO:api.php?
            List<Article> list = new List<Article>();

            //Regex pattern to find links
            Regex searchRegex = new Regex("<li><a .*? title=\\\"([^\"]*)\">", RegexOptions.Compiled);
            string ns = "&ns0=1";

            // explicitly add available namespaces to search options
            foreach (int k in Variables.Namespaces.Keys)
            {
                if (k <= 0) continue;
                ns += "&ns" + k.ToString() + "=1";
            }

            foreach (string s in terms)
            {
                int intStart = 0;
                string url = Variables.URLLong + "index.php?title=Special:Search&fulltext=Search&search=" + HttpUtility.UrlEncode(s) + "&limit=100&uselang=en" + ns;
                string title = "";

                do
                {
                    int n = list.Count;
                    string searchText = Tools.GetHTML(url);

                    //Find each match to the pattern
                    foreach (Match m in searchRegex.Matches(searchText))
                    {
                        title = m.Groups[1].Value;
                        title = HttpUtility.HtmlDecode(title);//title.Replace("&amp;", "&").Replace("&quot;", "\"").Replace("_", " ");
                        if (title.Contains("\""))
                        {
                            title = title.Replace("'", "");
                        }
                        list.Add(new WikiFunctions.Article(title));
                    }

                    if (list.Count != n)
                    {
                        intStart += 100;
                        url = Variables.URLLong + "index.php?title=Special:Search&fulltext=Search&search=" + HttpUtility.UrlEncode(s) + "&limit=100&uselang=en&offset=" + intStart.ToString() + ns;
                    }
                    else
                        break;

                } while (true);
            }
            return list;
        }
        #endregion

        #region From Special:Listusers
        /// <summary>
        /// Gets a list of users with given parameters
        /// </summary>
        /// <param name="group">user group, e.g. "sysop"</param>
        /// <param name="from">username to start from</param>
        /// <param name="limit">limit of users returned (max. 5000) if value <= 0, maximum assumed</param>
        /// <returns>The list of the articles.</returns>
        /// <remarks>Only used by IRCM</remarks>
        public static List<Article> FromListUsers(string group, string from, int limit)
        {
            //TODO:api.php?
            if (limit == 0 || limit < 0) limit = 5000;
            List<Article> list = new List<Article>();

            try
            {
                string url = Variables.URLLong + "index.php?title=Special:Listusers&group=" + group +
                    "&username=" + HttpUtility.UrlEncode(from) + "&limit=" + limit.ToString();

                string search = Tools.GetHTML(url);
                search = Tools.StringBetween(search, "<!-- start content -->", "<!-- end content -->");
                search = "<div>" + search + "</div>";
                StringReader sr = new StringReader(search);
                XmlDocument xml = new XmlDocument();
                xml.Load(sr);

                foreach (XmlNode n in xml.GetElementsByTagName("li"))
                {
                    list.Add(new WikiFunctions.Article(Variables.Namespaces[2] + n.FirstChild.InnerText));
                }
            }
            finally
            {
            }
            return list;
        }
        #endregion

        #region Other methods

        private static string encodeText(string txt)
        {
            return txt.Replace(' ', '_').Replace("&", "%26").Replace("+", "%2B");
        }

        private static List<Article> FilterSomeArticles(List<Article> UnfilteredArticles)
        {
            //Filter out artcles which we definately do not want to edit and remove duplicates.
            List<Article> items = new List<Article>();

            foreach (Article a in UnfilteredArticles)
            {
                if (a.NameSpaceKey >= 0 && a.NameSpaceKey != 9 && a.NameSpaceKey != 8 && !a.Name.StartsWith("Commons:"))
                {
                    items.Add(a);
                }
            }
            return items;
        }

        /// <summary>
        /// Turns a list of articles into an list of the associated talk pages.
        /// </summary>
        /// <param name="list">The list of articles.</param>
        /// <returns>The list of the talk pages.</returns>
        public static List<Article> ConvertToTalk(List<Article> list)
        {
            List<Article> newList = new List<Article>();

            foreach (Article a in list)
            {
                if (Tools.IsTalkPage(a.NameSpaceKey))
                {
                    newList.Add(a);
                    continue;
                }
                else if (a.NameSpaceKey == 0)
                {
                    newList.Add(new WikiFunctions.Article(Variables.Namespaces[1] + a.Name));
                    continue;
                }
                else
                {
                    string s = Regex.Replace(a.Name, "^" + Variables.Namespaces[a.NameSpaceKey], Variables.Namespaces[a.NameSpaceKey + 1]);
                    newList.Add(new WikiFunctions.Article(s));
                    continue;
                }
            }
            return newList;
        }

        /// <summary>
        /// Turns an article into its associated talk page
        /// </summary>
        /// <param name="a">The Article</param>
        /// <returns>Article Title</returns>
        public static string ConvertToTalk(Article a)
        {
            if (Tools.IsTalkPage(a.NameSpaceKey))
            {
                return a.URLEncodedName;
            }
            else if (a.NameSpaceKey == 0)
            {
                return (Variables.Namespaces[1] + a.Name);
            }
            else
            {
                return Regex.Replace(a.Name, "^" + Variables.Namespaces[a.NameSpaceKey], Variables.Namespaces[a.NameSpaceKey + 1]);
            }
        }

        /// <summary>
        /// Turns a list of talk pages into a list of the associated articles.
        /// </summary>
        /// <param name="list">The list of talk pages.</param>
        /// <returns>The list of articles.</returns>
        public static List<Article> ConvertFromTalk(List<Article> list)
        {
            List<Article> newList = new List<Article>();

            foreach (Article a in list)
            {
                if (Tools.IsTalkPage(a.NameSpaceKey))
                {
                    if (a.NameSpaceKey == 1)
                    {
                        string s = Regex.Replace(a.Name, "^" + Variables.Namespaces[a.NameSpaceKey], "");
                        newList.Add(new WikiFunctions.Article(s));
                    }
                    else
                    {
                        string s = Regex.Replace(a.Name, "^" + Variables.Namespaces[a.NameSpaceKey], Variables.Namespaces[a.NameSpaceKey - 1]);
                        newList.Add(new WikiFunctions.Article(s));
                    }
                }
                else
                    newList.Add(a);
            }
            return newList;
        }

        /// <summary>
        /// Turns a talk page into its associated article
        /// </summary>
        /// <param name="a">The Article</param>
        /// <returns>Article Title</returns>
        public static string ConvertFromTalk(Article a)
        {
            if (Tools.IsTalkPage(a.NameSpaceKey))
            {
                if (a.NameSpaceKey == 1)
                {
                    return Regex.Replace(a.Name, "^" + Variables.Namespaces[a.NameSpaceKey], "");
                }
                else
                {
                    return Regex.Replace(a.Name, "^" + Variables.Namespaces[a.NameSpaceKey], Variables.Namespaces[a.NameSpaceKey - 1]);
                }
            }
            else
                return a.URLEncodedName;
        }
        #endregion
    }

    [Serializable]
    public class PageDoesNotExistException : ApplicationException
    {
        public PageDoesNotExistException() { }
        public PageDoesNotExistException(string message)
            : base(message) { }
        public PageDoesNotExistException(string message, System.Exception inner)
            : base(message, inner) { }
        protected PageDoesNotExistException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
