/*
WikiFunctions
Copyright (C) 2006 Martin Richards

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
        public static bool QuietMode = false;

        readonly static Regex regexli = new Regex("<li>.*</li>", RegexOptions.Compiled);
        readonly static Regex regexe = new Regex("<li>\\(?<a href=\"[^\"]*\" title=\"([^\"]*)\">[^<>]*</a> \\(redirect page\\)", RegexOptions.Compiled);
        readonly static Regex regexe2 = new Regex("<a href=\"[^\"]*\" title=\"([^\"]*)\">[^<>]*</a>", RegexOptions.Compiled);
        readonly static Regex RegexFromFile = new Regex("(^[a-z]{2,3}:)|(simple:)", RegexOptions.Compiled);
        readonly static Regex regexWatchList = new Regex("<LI><INPUT type=checkbox value=(.*?) name=id\\[\\]", RegexOptions.Compiled);

        #region From category

        /// <summary>
        /// Gets a list of articles and sub-categories in a category.
        /// </summary>
        /// <param name="Category">The category.</param>
        /// <param name="SubCategories">Whether to get all sub categories as well.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromCategory(bool SubCategories, params string[] Categories)
        {
            return FromCategory(SubCategories, -1, Categories);
        }

        /// <summary>
        /// Gets a list of articles and sub-categories in a category.
        /// </summary>
        /// <param name="Category">The category.</param>
        /// <param name="SubCategories">Whether to get all sub categories as well.</param>
        /// <param name="Limit">The maximum number of results resulted..</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromCategory(bool SubCategories, int Limit, params string[] Categories)
        {
            List<Article> list = new List<Article>();

            List<string> titles = new List<string>();

            for (int i = 0; i < Categories.Length; i++)
            {
                string origURL = Variables.URLLong + "/query.php?what=category&cptitle=Category:" + encodeText(Categories[i]) + "&format=xml&cplimit=500";
                string URL = origURL;
                int ns = 0;
                string title = "";

                while (true)
                {
                    string html = Tools.GetHTML(URL);
                    if (html.Contains("<error>emptyresult</error>") && !QuietMode)
                        MessageBox.Show("The category " + Categories[i] + " is empty or does not exist. Make sure it is spelt correctly. If you want a stub category remember to type the category name and not the stub name.");

                    bool more = false;
                   
                    using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                    {
                        while (reader.Read())
                        {
                            if (reader.LocalName.Equals("query"))
                            {
                                reader.ReadToFollowing("category");
                                reader.MoveToAttribute("next");
                                URL = origURL + "&cpfrom=" + reader.Value;
                                more = true;
                                reader.ReadToFollowing("page");
                            }
                            else if (reader.Name == "ns")
                            {
                                ns = int.Parse(reader.ReadString());
                            }
                            else if (reader.Name == "title")
                            {
                                title = reader.ReadString();
                                Article a = new Article(title, ns);
                                if (titles.Contains(title)) continue;
                                list.Add(a);
                                titles.Add(title);

                                if (Limit >= 0 && list.Count >= Limit)
                                {
                                    more = false;
                                    break;
                                }

                                if (SubCategories && ns == 14)
                                {
                                    Array.Resize<string>(ref Categories, Categories.Length + 1);
                                    Categories[Categories.Length - 1] = title.Replace(Variables.Namespaces[14], "");
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

        #region From whatlinkshere

        /// <summary>
        /// Gets a list of articles that link to the given page.
        /// </summary>
        /// <param name="Page">The page to find links to.</param>
        /// <param name="Embedded">Gets articles that embed (transclude).</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromWhatLinksHere(bool Embedded, params string[] Pages)
        {
            return FromWhatLinksHere(Embedded, -1, Pages);
        }

        /// <summary>
        /// Gets a list of articles that link to the given page.
        /// </summary>
        /// <param name="Pages">The page to find links to.</param>
        /// <param name="Embedded">Gets articles that embed (transclude).</param>
        /// <param name="Limit">The maximum number of results resulted..</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromWhatLinksHere(bool Embedded, int Limit, params string[] Pages)
        {
            string request = "backlinks";
            string initial = "bl";
            if (Embedded)
            {
                request = "embeddedin";
                initial = "ei";
            }
            List<Article> list = new List<Article>();

            foreach (string Page in Pages)
            {
                string OrigURL = Variables.URLLong + "query.php?what=" + request + "&titles=" + encodeText(Page) + "&" + initial + "limit=500&" + initial + "filter=all&format=xml";
                string URL = OrigURL;
                string title = "";
                int ns = 0;

                while (true)
                {
                    string html = Tools.GetHTML(URL);
                    if (html.Contains("<" + request + " error=\"emptyrequest\" />"))
                        throw new PageDoesNotExistException("No pages link to " + Page + ". Make sure it is spelt correctly.");

                    bool more = false;

                    using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                    {
                        while (reader.Read())
                        {
                            if (reader.LocalName.Equals("query"))
                            {
                                reader.ReadToFollowing(request);
                                if (reader.HasAttributes)
                                {
                                    reader.MoveToAttribute("next");
                                    URL = OrigURL + "&" + initial + "contfrom=" + reader.Value;

                                    more = true;
                                }
                            }

                            if (reader.Name.Equals(initial))
                            {
                                if (reader.MoveToAttribute("ns"))
                                    ns = int.Parse(reader.Value);
                                else
                                    ns = 0;

                                title = reader.ReadString();
                                list.Add(new Article(title, ns));

                                if (Limit >= 0 && list.Count >= Limit)
                                {
                                    more = false;
                                    break;
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
        /// <param name="Pages">The page to find redirects to.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromRedirects(params string[] Pages)
        {
            return FromRedirects(-1, Pages);
        }

        /// <summary>
        /// Gets a list of articles that link to the given page.
        /// </summary>
        /// <param name="Page">The page to find links to.</param>
        /// <param name="RedirectsOnly">Only list redirects.</param>
        /// <param name="Embedded">Gets articles that embed (transclude).</param>
        /// <param name="Limit">The maximum number of results resulted..</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromRedirects(int Limit, params string[] Pages)
        {
            List<Article> list = new List<Article>();

            foreach (string page in Pages)
            {
                string Page = page;
                Page = encodeText(Page);
                string URL = Variables.URLLong + "index.php?title=Special:Whatlinkshere&target=" + Page + "&limit=5000&offset=0";
                
                do
                {
                    string PageText = Tools.GetHTML(URL);

                    if (PageText.Contains("No pages link to here."))
                        throw new Exception("No pages link to " + Page + ". Make sure it is spelt correctly.");

                    foreach (Match m in regexe.Matches(PageText))
                    {
                        string pagetitle = m.Groups[1].Value;
                        bool added = false;
                        foreach (KeyValuePair<int, string> Namespace in Variables.Namespaces)
                        {
                            if (pagetitle.Contains(Namespace.Value))
                            {
                                list.Add(new Article(pagetitle, Namespace.Key));
                                added = true;
                            }
                        }
                        if (!added)
                            list.Add(new Article(pagetitle, 0));
                    }
                    if (PageText.Contains(">next 5,000<"))
                    {
                        Match m = Regex.Match(PageText, "from=(.*?)\" title=\"(.*?)\"" + ">next 5,000<", RegexOptions.RightToLeft);
                        string strPage = m.Groups[1].Value;
                        URL = Variables.URLLong + "index.php?title=Special:Whatlinkshere&target=" + Page + "&limit=5000&from=" + strPage;
                    }
                    else
                        break;

                } while (true);
            }

            return list;
        }

        #endregion

        #region From links on page

        /// <summary>
        /// Gets a list of links on a page.
        /// </summary>
        /// <param name="Article">The page to find links on.</param>
        /// <returns>The list of the links.</returns>
        public static List<Article> FromLinksOnPage(params string[] Articles)
        {
            return FromLinksOnPage(-1, Articles);
        }

        /// <summary>
        /// Gets a list of links on a page.
        /// </summary>
        /// <param name="Limit">The maximum number of results resulted.</param>
        /// <param name="Article">The page to find links on.</param>
        /// <returns>The list of the links.</returns>
        public static List<Article> FromLinksOnPage(int Limit, params string[] Articles)
        {
            List<Article> list = new List<Article>();

            foreach (string Article in Articles)
            {
                try
                {
                    string OrigURL = Variables.URLLong + "query.php?what=links&titles=" + encodeText(Article) + "&format=xml";
                    string URL = OrigURL;
                    string title = "";
                    int ns = 0;

                    string html = Tools.GetHTML(URL);
                    if (!html.Contains("<links>"))
                        throw new PageDoesNotExistException("\"" + Article + "\" either does not exist or has no links. Make sure it is spelt correctly.");

                    using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name == ("l"))
                            {
                                if (reader.MoveToAttribute("ns"))
                                    ns = int.Parse(reader.Value);
                                else
                                    ns = 0;

                                title = reader.ReadString();
                                list.Add(new Article(title, ns));

                                if (Limit >= 0 && list.Count >= Limit)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            return list;
        }

        #endregion

        #region From text file

        /// <summary>
        /// Gets a list of [[wiki]] style links from txt file.
        /// </summary>
        /// <param name="fileName">The file path of the list.</param>
        /// <returns>The list of the links.</returns>
        public static List<Article> FromTextFile(params string[] FileNames)
        {
            return FromTextFile(-1, FileNames);
        }

        /// <summary>
        /// Gets a list of [[wiki]] style links from txt file.
        /// </summary>
        /// <param name="Limit">The maximum number of results resulted.</param>
        /// <param name="fileName">The file path of the list.</param>
        /// <returns>The list of the links.</returns>
        public static List<Article> FromTextFile(int Limit, params string[] FileNames)
        {
            List<Article> list = new List<Article>();

            foreach (string fileName in FileNames)
            {
                string PageText = "";
                string title = "";

                StreamReader sr = new StreamReader(fileName, Encoding.Default);
                PageText = sr.ReadToEnd();
                sr.Close();

                if (WikiRegexes.WikiLink.IsMatch(PageText))
                {
                    foreach (Match m in WikiRegexes.WikiLink.Matches(PageText))
                    {
                        title = m.Groups[1].Value;
                        if (!RegexFromFile.IsMatch(title) && (!(title.StartsWith("#"))))
                        {
                            title = Tools.TurnFirstToUpper(title);
                            list.Add(new Article(title));

                            if (Limit >= 0 && list.Count >= Limit)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    foreach (string s in PageText.Split(new string[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (s.Trim() == "" || !Tools.IsValidTitle(s)) continue;
                        title = Tools.TurnFirstToUpper(s.Trim());
                        list.Add(new Article(title));

                        if (Limit >= 0 && list.Count >= Limit)
                        {
                            break;
                        }
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
        /// <param name="Google">The term to search for.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromGoogleSearch(params string[] Googles)
        {
            return FromGoogleSearch(-1, Googles);
        }

        /// <summary>
        /// Gets a list from a google search of the site.
        /// </summary>
        /// <param name="Limit">The maximum number of results resulted.</param>
        /// <param name="Google">The term to search for.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromGoogleSearch(int Limit, params string[] Googles)
        {
            List<Article> list = new List<Article>();

            foreach (string G in Googles)
            {
                int intStart = 0;
                string Google = encodeText(G);
                Google = Google.Replace("_", " ");
                string URL = "http://www.google.com/search?q=" + Google + "+site:" + Variables.URL + "&num=100&hl=en&lr=&start=0&sa=N&filter=0";
                string title = "";

                //Regex pattern to find links
                Regex RegexGoogle = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+) class=l)",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);

                do
                {
                    string GoogleText = Tools.GetHTML(URL, Encoding.Default);

                    //Find each match to the pattern
                    foreach (Match m in RegexGoogle.Matches(GoogleText))
                    {
                        title = m.Groups[1].Value;
                        if (!title.StartsWith(Variables.URL + "/wiki/")) continue;

                        title = title.Remove(0, (Variables.URL + "/wiki/").Length);

                        title = HttpUtility.UrlDecode(title).Replace('_', ' ');

                        list.Add(new Article(title));

                        if (Limit >= 0 && list.Count >= Limit)
                        {
                            break;
                        }
                    }

                    if (GoogleText.Contains("<br>Next</a>"))
                    {
                        intStart += 100;
                        URL = "http://www.google.com/search?q=" + Google + "+site:" + Variables.URL + "&num=100&hl=en&lr=&start=" + intStart.ToString() + "&sa=N&filter=0";
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
        /// <param name="User">The name of the user.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromUserContribs(params string[] Users)
        {
            return FromUserContribs(-1, Users);
        }

        /// <summary>
        /// Gets a list from a users contribs.
        /// </summary>
        /// <param name="Limit">The maximum number of results resulted.</param>
        /// <param name="User">The name of the user.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromUserContribs(int Limit, params string[] Users)
        {
            List<Article> list = new List<Article>();

            foreach (string U in Users)
            {
                string User = Regex.Replace(U, "^" + Variables.Namespaces[2], "", RegexOptions.IgnoreCase);
                User = encodeText(User);

                string PageText = Tools.GetHTML(Variables.URLLong + "index.php?title=Special:Contributions&target=" + User + "&offset=0&limit=2500");
                Regex RegexUserContribs = new Regex("<li>.*? title=\"([^\"]*)\">[^<>]*</a>", RegexOptions.Compiled);
                string title = "";

                foreach (Match m in RegexUserContribs.Matches(PageText))
                {
                    title = m.Groups[1].Value;

                    list.Add(new Article(title));

                    if (Limit >= 0 && list.Count >= Limit)
                    {
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
        /// <param name="Special">The page to find links on, e.g. "Deadendpages" or "Deadendpages&limit=500&offset=0".</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromSpecialPage(params string[] Specials)
        {
            return FromSpecialPage(-1, Specials);
        }

        /// <summary>
        /// Gets a list of links on a special page.
        /// </summary>
        /// <param name="Limit">The maximum number of results resulted.</param>
        /// <param name="Special">The page to find links on, e.g. "Deadendpages" or "Deadendpages&limit=500&offset=0".</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromSpecialPage(int Limit, params string[] Specials)
        {
            List<Article> list = new List<Article>();

            if (Limit < 0) Limit = 1000;

            foreach (string S in Specials)
            {
                string Special = Regex.Replace(S, "^" + Variables.Namespaces[-1], "", RegexOptions.IgnoreCase);

                string url = Variables.URLLong + "index.php?title=Special:" + Special;
                if (!url.Contains("&limit=")) url += "&limit=" + Limit.ToString();
                string PageText = Tools.GetHTML(url);

                PageText = Tools.StringBetween(PageText, "<!-- start content -->", "<!-- end content -->");
                string title = "";
                int ns = 0;

                if (regexli.IsMatch(PageText))
                {
                    foreach (Match m in regexli.Matches(PageText))
                    {
                        foreach (Match m2 in regexe2.Matches(m.Value))
                        {
                            if (m2.Value.Contains("&amp;action=") && !m2.Value.Contains("&amp;action=edit"))
                                continue;

                            title = m2.Groups[1].Value;

                            title = HttpUtility.HtmlDecode(title);

                            if (title.Trim() == "") continue;

                            list.Add(new Article(title));
                            break;
                        }
                    }
                }
                else
                /*if (regexe.IsMatch(PageText))
                {
                    foreach (Match m in regexe.Matches(PageText))
                    {
                        title = m.Groups[1].Value;
                        if (title == "")
                            continue;
                        title = HttpUtility.HtmlDecode(title);
                        list.Add(new Article(title));

                        if (Limit >= 0 && list.Count >= Limit)
                        {
                            break;
                        }
                    }
                }*/
                {
                    foreach (Match m in regexe2.Matches(PageText))
                    {
                        title = m.Groups[1].Value;
                        if (title.Trim() == "") continue;
                        if (title != "Wikipedia:Special pages" && title != "Wikipedia talk:Special:Lonelypages" && title != "Wikipedia:Offline reports" && title != "Template:Specialpageslist")
                        {
                            title = title.Replace("&amp;", "&").Replace("&quot;", "\"");
                            if (title == "")
                                continue;

                            ns = Tools.CalculateNS(title);
                            if (ns < 0) continue;
                            list.Add(new Article(title, ns));

                            if (Limit >= 0 && list.Count >= Limit)
                            {
                                break;
                            }
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
        /// <param name="Image">The image.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromImageLinks(params string[] Images)
        {
            return FromImageLinks(-1, Images);
        }

        /// <summary>
        /// Gets a list of articles that use an image.
        /// </summary>
        /// <param name="Limit">The maximum number of results resulted.</param>
        /// <param name="Image">The image.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromImageLinks(int Limit, params string[] Images)
        {
            List<Article> list = new List<Article>();

            foreach (string I in Images)
            {
                string Image = Regex.Replace(I, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase);
                Image = encodeText(Image);

                string URL = Variables.URLLong + "query.php?what=imagelinks&titles=Image:" + Image + "&illimit=500&format=xml";
                string title = "";
                int ns = 0;

                while (true)
                {
                    string html = Tools.GetHTML(URL);
                    if (!html.Contains("<imagelinks>"))
                        throw new PageDoesNotExistException("The image " + Image + " does not exist. Make sure it is spelt correctly.");

                    bool more = false;

                    using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                    {
                        while (reader.Read())
                        {
                            if (reader.LocalName.Equals("query"))
                            {
                                reader.ReadToFollowing("imagelinks");
                                reader.MoveToAttribute("next");
                                URL = Variables.URLLong + "query.php?what=imagelinks&titles=Image:" + Image + "&illimit=500&format=xml&ilcontfrom=" + reader.Value;
                                more = true;
                                reader.ReadToFollowing("page");
                            }

                            if (reader.Name.Equals("il"))
                            {
                                if (reader.MoveToAttribute("ns"))
                                    ns = int.Parse(reader.Value);
                                else
                                    ns = 0;

                                title = reader.ReadString();
                                list.Add(new Article(title, ns));

                                if (Limit >= 0 && list.Count >= Limit)
                                {
                                    more = false;
                                    break;
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
            webbrowser.Navigate(Variables.URLLong + "index.php?title=Special:Watchlist/edit");
            while (webbrowser.ReadyState != WebBrowserReadyState.Complete) Application.DoEvents();

            string html = webbrowser.Document.Body.InnerHtml;
            string title = "";
            List<Article> list = new List<Article>();

            if (!html.Contains("<LI id=pt-logout"))
            {
                throw new PageDoesNotExistException("Please make sure you are logged into Wikipedia in Internet Explorer so your watch list can be obtained");
            }

            foreach (Match m in regexWatchList.Matches(html))
            {
                title = m.Groups[1].Value.Trim('"');
                title = title.Replace("&amp;", "&").Replace("&quot;", "\"");
                list.Add(new Article(title));
            }

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
            return FromWikiSearch(-1, terms);
        }

        /// <summary>
        /// Gets a list from wiki's internal search
        /// </summary>
        /// <param name="Limit">The maximum number of results resulted.</param>
        /// <param name="terms">The terms to search for.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromWikiSearch(int Limit, params string[] terms)
        {
            List<Article> list = new List<Article>();

            //Regex pattern to find links
            Regex SearchRegex = new Regex("<li style=\"padding-bottom: 1em;\"><a .*? title=\"([^\"]*)\">", RegexOptions.Compiled);
            string ns = "&ns0=1";

            // explicitly add available namespaces to search options
            foreach(int k in Variables.Namespaces.Keys)
            {
                if (k <= 0) continue;
                ns += "&ns" + k.ToString() + "=1";
            }

            foreach (string s in terms)
            {
                int intStart = 0;
                string URL = Variables.URLLong + "/index.php?title=Special:Search&fulltext=Search&search=" + HttpUtility.UrlEncode(s) + "&limit=100&uselang=en" + ns;
                string title = "";

                do
                {
                    int n = list.Count ;
                    string SearchText = Tools.GetHTML(URL);

                    //Find each match to the pattern
                    foreach (Match m in SearchRegex.Matches(SearchText))
                    {
                        title = m.Groups[1].Value;
                        title = HttpUtility.HtmlDecode(title);//title.Replace("&amp;", "&").Replace("&quot;", "\"").Replace("_", " ");
                        if (title.Contains("\""))
                        {
                            title = title.Replace("'", "");
                        }
                        list.Add(new Article(title));

                        if (Limit >= 0 && list.Count >= Limit)
                        {
                            break;
                        }
                    }

                    if (list.Count != n)
                    {
                        intStart += 100;
                        URL = Variables.URLLong + "/index.php?title=Special:Search&fulltext=Search&search=" + HttpUtility.UrlEncode(s) + "&limit=100&uselang=en&offset=" + intStart.ToString() + ns;
                    }
                    else
                        break;

                } while (true);
            }

            return list;//FilterSomeArticles(list);
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
        public static List<Article> FromListusers(string group, string from, int limit)
        {
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
                    list.Add(new Article(Variables.Namespaces[2] + n.FirstChild.InnerText));
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
            txt = txt.Replace(' ', '_').Replace("&", "%26");
            //txt = HttpUtility.UrlEncode(txt);
            return txt;
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
                    newList.Add(new Article(Variables.Namespaces[1] + a.Name));
                    continue;
                }
                else
                {
                    string s = Regex.Replace(a.Name, "^" + Variables.Namespaces[a.NameSpaceKey], Variables.Namespaces[a.NameSpaceKey + 1]);
                    newList.Add(new Article(s));
                    continue;
                }
            }

            return newList;
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
                        newList.Add(new Article(s));
                    }
                    else
                    {
                        string s = Regex.Replace(a.Name, "^" + Variables.Namespaces[a.NameSpaceKey], Variables.Namespaces[a.NameSpaceKey - 1]);
                        newList.Add(new Article(s));
                    }
                }
                else
                    newList.Add(a);
            }

            return newList;
        }

        #endregion

        #region old methods

        /*
        
        /// <summary>
        /// Gets a list of articles and sub-categories in a category.
        /// </summary>
        /// <param name="Category">The category.</param>
        /// <returns>The ArrayList of the articles.</returns>
        public ArrayList FromCategory(string Category)
        {
            Category = Regex.Replace(Category, "^" + Variables.CategoryNS, "", RegexOptions.IgnoreCase);
            Category = encodeText(Category);

            string URL = Variables.URL + "index.php?title=Category:" + Category + "&from=";
            ArrayList ArticleArray = new ArrayList();

            do
            {
                string PageText = Tools.GetHTML(URL);

                if (PageText.Contains("<div class=\"noarticletext\">"))
                    throw new Exception("That category does not exist. Make sure it is spelt correctly. If you want a stub category remember to type the category name and not the stub name.");

                //cut out part we want
                PageText = PageText.Substring(PageText.IndexOf("<!-- Saved in parser cache with key "));
                PageText = PageText.Substring(0, PageText.IndexOf("<div class=\"printfooter\">"));

                foreach (Match m in regexe.Matches(PageText))
                {
                    ArticleArray.Add(m.Groups[1].Value);
                }

                if (Regex.IsMatch(PageText, "&amp;from=(.*?)\" title=\"", RegexOptions.RightToLeft))
                {
                    Match m = Regex.Match(PageText, "&amp;from=(.*?)\" title=\"", RegexOptions.RightToLeft);
                    string strPage = m.Groups[1].Value;
                    URL = Variables.URL + "index.php?title=Category:" + Category + "&from=" + strPage;
                }
                else
                    break;
            }
            while (true);

            return FilterSomeArticles(ArticleArray);
        }

        /// <summary>
        /// Gets a list of articles that link to the given page.
        /// </summary>
        /// <param name="Page">The page to find links to.</param>
        /// <param name="Page">Only list "inclusions".</param>
        /// <returns>The ArrayList of the articles.</returns>
        public ArrayList FromWhatLinksHere(string Page, bool incOnly)
        {
            Page = encodeText(Page);
            string URL = Variables.URL + "index.php?title=Special:Whatlinkshere&target=" + Page + "&limit=5000&offset=0";
            ArrayList ArticleArray = new ArrayList();

            do
            {
                string PageText = Tools.GetHTML(URL);

                if (PageText.Contains("No pages link to here."))
                    throw new Exception("No pages link to " + Page + ". Make sure it is spelt correctly.");

                foreach (Match m in regexe.Matches(PageText))
                {
                    if (!incOnly)
                        ArticleArray.Add(m.Groups[1].Value);
                    else if (m.Groups[2].Value == " (transclusion)")
                        ArticleArray.Add(m.Groups[1].Value);
                }

                if (PageText.Contains(">next 5,000<") && Variables.LangCode == "en")
                {
                    Match m = Regex.Match(PageText, "from=(.*?)\"" + ">next 5,000<", RegexOptions.RightToLeft);
                    string strPage = m.Groups[1].Value;
                    URL = Variables.URL + "index.php?title=Special:Whatlinkshere&target=" + Page + "&limit=5000&from=" + strPage;
                }
                else
                    break;

            } while (true);

            return FilterSomeArticles(ArticleArray);
        }
         
        ///// <summary>
        ///// Gets a list of links on a page.
        ///// </summary>
        ///// <param name="Article">The page to find links on.</param>
        ///// <returns>The ArrayList of the links.</returns>
        //public ArrayList FromLinksOnPage(string Article)
        //{
        //    Article = encodeText(Article);
        //    ArrayList ArticleArray = new ArrayList();
        //    string x = "";

        //    string PageText = Tools.GetHTML(Variables.URL + "index.php?title=" + Article + "&action=edit");

        //    if (PageText.Contains("<title>Article not found - Wikipedia, the free encyclopedia</title>"))
        //        throw new Exception("The page " + Article + " does not exist. Make sure it is spelt correctly.");

        //    PageText = PageText.Substring(PageText.IndexOf("<textarea"));
        //    PageText = PageText.Substring(0, PageText.IndexOf("</textarea>"));

        //    foreach (Match m in wikiLinkReg.Matches(PageText))
        //    {
        //        x = m.Groups[1].Value;

        //        if (!(Regex.IsMatch(x, "(^[a-z]{2,3}:)|(simple:)")) && (!(x.StartsWith("#"))))
        //            ArticleArray.Add(Tools.TurnFirstToUpper(x));
        //    }
        //    return FilterSomeArticles(ArticleArray);
        //}

        /// <summary>
        /// Gets a list of articles that use an image.
        /// </summary>
        /// <param name="Image">The image.</param>
        /// <returns>The ArrayList of the articles.</returns>
        public ArrayList FromImageLinks(string Image)
        {
            Image = Regex.Replace(Image, "^" + Variables.ImageNS, "", RegexOptions.IgnoreCase);
            Image = encodeText(Image);
            ArrayList ArticleArray = new ArrayList();

            string PageText = Tools.GetHTML(Variables.URL + "index.php?title=Image:" + Image);

            PageText = PageText.Substring(PageText.IndexOf("<h2 id=\"filelinks\">"));

            if (!PageText.Contains("<li>"))
                throw new Exception("That image does not exist. Make sure it is spelt correctly.");

            foreach (Match m in regexe.Matches(PageText))
            {
                ArticleArray.Add(m.Groups[1].Value);
            }

            return FilterSomeArticles(ArticleArray);
        }
        
        */

        #endregion

    }

    [Serializable]
    public class PageDoesNotExistException : ApplicationException
    {
        public PageDoesNotExistException() { }
        public PageDoesNotExistException(string message) : base(message) { }

        public PageDoesNotExistException(string message, System.Exception inner) : base(message, inner) { }
        protected PageDoesNotExistException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
