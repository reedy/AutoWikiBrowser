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
        readonly static Regex regexe = new Regex("<li>\\(?<a href=\"[^\"]*\" title=\"([^\"]*)\">[^<>]*</a>( \\(transclusion\\))?", RegexOptions.Compiled);
        readonly static Regex wikiLinkReg = new Regex("\\[\\[(.*?)(\\]\\]|\\|)", RegexOptions.Compiled);
        readonly static Regex regexe2 = new Regex("<a href=\"[^\"]*\" title=\"([^\"]*)\">[^<>]*</a>");

        /// <summary>
        /// Gets a list of articles and sub-categories in a category.
        /// </summary>
        /// <param name="Category">The category.</param>
        /// <param name="SubCategories">Whether to get all sub categories as well.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromCategory(bool SubCategories, params string[] Categories)
        {
            List<Article> list = new List<Article>();
           
            for(int i = 0; i < Categories.Length; i++)
            {
                string origURL = Variables.URL + "/query.php?what=category&cptitle=" + encodeText(Categories[i]) + "&format=xml&cplimit=500";
                string URL = origURL;
                int ns = 0;
                string title = "";

                while (true)
                {
                    string html = Tools.GetHTML(URL);
                    if (html.Contains("<error>emptyresult</error>"))
                        MessageBox.Show("The category " + Categories[i] + " does not exist. Make sure it is spelt correctly. If you want a stub category remember to type the category name and not the stub name.");

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

                            if (reader.Name == "ns")
                                ns = int.Parse(reader.ReadString());

                            if (reader.Name == "title")
                            {
                                title = reader.ReadString();
                                list.Add(new Article(title, ns));

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

        /// <summary>
        /// Gets a list of articles that link to the given page.
        /// </summary>
        /// <param name="Page">The page to find links to.</param>
        /// <param name="RedirectsOnly">Only list redirects.</param>
        /// <param name="Embedded">Gets articles that embed (transclude).</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromWhatLinksHere(bool Embedded, params string[] Pages)
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
                string OrigURL = Variables.URL + "query.php?what=" + request + "&titles=" + encodeText(Page) + "&" + initial + "limit=500&" + initial + "filter=all&format=xml";
                string URL = OrigURL;
                string title = "";
                int ns = 0;

                while (true)
                {
                    string html = Tools.GetHTML(URL);
                    if (html.Contains("<" + request + " error=\"emptyrequest\" />"))
                        throw new PageDoeNotExistException("No pages link to " + Page + ". Make sure it is spelt correctly.");

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
                                if (reader.AttributeCount > 1)
                                {
                                    reader.MoveToAttribute("ns");
                                    ns = int.Parse(reader.Value);
                                }
                                else
                                    ns = 0;

                                title = reader.ReadString();
                                list.Add(new Article(title, ns));
                            }
                        }
                    }

                    if (!more)
                        break;
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a list of links on a page.
        /// </summary>
        /// <param name="Article">The page to find links on.</param>
        /// <returns>The list of the links.</returns>
        public static List<Article> FromLinksOnPage(params string[] Articles)
        {
            List<Article> list = new List<Article>();

            foreach (string Article in Articles)
            {
                string OrigURL = Variables.URL + "query.php?what=links&titles=" + encodeText(Article) + "&format=xml";
                string URL = OrigURL;
                string title = "";
                int ns = 0;

                string html = Tools.GetHTML(URL);
                if (!html.Contains("<links>"))
                    throw new PageDoeNotExistException(Article + " either does not exist or has no links. Make sure it is spelt correctly.");

                using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                {
                    while (reader.Read())
                    {
                        if (reader.Name == ("l"))
                        {
                            if (reader.AttributeCount > 1)
                            {
                                reader.MoveToAttribute("ns");
                                ns = int.Parse(reader.Value);
                            }
                            else
                                ns = 0;

                            title = reader.ReadString();
                            list.Add(new Article(title, ns));
                        }
                    }
                }
            }

            return list;
        }

        static readonly Regex RegexFromFile = new Regex("(^[a-z]{2,3}:)|(simple:)", RegexOptions.Compiled);

        /// <summary>
        /// Gets a list of [[wiki]] style links from txt file.
        /// </summary>
        /// <param name="fileName">The file path of the list.</param>
        /// <returns>The list of the links.</returns>
        public static List<Article> FromTextFile(params string[] FileNames)
        {
            List<Article> list = new List<Article>();

            foreach (string fileName in FileNames)
            {
                string PageText = "";
                string title = "";

                StreamReader sr = new StreamReader(fileName, Encoding.Default);
                PageText = sr.ReadToEnd();
                sr.Close();

                foreach (Match m in wikiLinkReg.Matches(PageText))
                {
                    title = m.Groups[1].Value;
                    if (!RegexFromFile.IsMatch(title) && (!(title.StartsWith("#"))))
                    {
                        title = Tools.TurnFirstToUpper(title);
                        list.Add(new Article(title));
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a list from a google search of the site.
        /// </summary>
        /// <param name="Google">The term to search for.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromGoogleSearch(params string[] Googles)
        {
            List<Article> list = new List<Article>();

            foreach (string G in Googles)
            {
                int intStart = 0;
                string Google = encodeText(G);
                Google = Google.Replace("_", " ");
                string URL = "http://www.google.com/search?q=" + Google + "+site:" + Variables.URLShort + "&num=100&hl=en&lr=&start=0&sa=N";
                string title = "";

                do
                {
                    string GoogleText = Tools.GetHTML(URL, Encoding.Default);

                    GoogleText = HttpUtility.HtmlDecode(GoogleText);

                    //Remove googles bold highlighting.
                    GoogleText = Regex.Replace(GoogleText, "<[bB]>|</[Bb]>", "");

                    //Regex pattern to find links
                    Regex RegexGoogle = new Regex("\">([^<]*) - " + Variables.Namespaces[4].TrimEnd(':'), RegexOptions.Compiled);

                    //Find each match to the pattern
                    foreach (Match m in RegexGoogle.Matches(GoogleText))
                    {
                        title = m.Groups[1].Value;
                        title = title.Replace("&amp;", "&").Replace("&quot;", "\"").Replace("_", " ");
                        if (title.Contains("\""))
                        {
                            title = title.Replace("'", "");
                        }
                        list.Add(new Article(title));
                    }

                    if (GoogleText.Contains("<br>Next</a>"))
                    {
                        intStart += 100;
                        URL = "http://www.google.com/search?q=" + Google + "+site:" + Variables.URLShort + "&num=100&hl=en&lr=&start=" + intStart.ToString() + "&sa=N";
                    }
                    else
                        break;

                } while (true);
            }

            return FilterSomeArticles(list);
        }

        /// <summary>
        /// Gets a list from a users contribs.
        /// </summary>
        /// <param name="User">The name of the user.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromUserContribs(params string[] Users)
        {
            List<Article> list = new List<Article>();

            foreach (string U in Users)
            {
                string User = Regex.Replace(U, "^" + Variables.Namespaces[2], "", RegexOptions.IgnoreCase);
                User = encodeText(User);

                string PageText = Tools.GetHTML(Variables.URL + "index.php?title=Special:Contributions&target=" + User + "&offset=0&limit=2500");
                Regex RegexUserContribs = new Regex("<li>.*? title=\"([^\"]*)\">[^<>]*</a>", RegexOptions.Compiled);
                string title = "";

                foreach (Match m in RegexUserContribs.Matches(PageText))
                {
                    title = m.Groups[1].Value;

                    list.Add(new Article(title));
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a list of links on a special page.
        /// </summary>
        /// <param name="Special">The page to find links on, e.g. "Deadendpages" or "Deadendpages&limit=500&offset=0".</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromSpecialPage(params string[] Specials)
        {
            List<Article> list = new List<Article>();

            foreach (string S in Specials)
            {
                string Special = Regex.Replace(S, "^" + Variables.Namespaces[-1], "", RegexOptions.IgnoreCase);
                string PageText = Tools.GetHTML(Variables.URL + "index.php?title=Special:" + Special);

                PageText = PageText.Substring(PageText.IndexOf("<!-- start content -->"), PageText.IndexOf("<!-- end content -->") - PageText.IndexOf("<!-- start content -->"));
                string title = "";
                int ns = 0;

                if (regexe.IsMatch(PageText))
                {
                    foreach (Match m in regexe.Matches(PageText))
                    {
                        title = m.Groups[1].Value;
                        list.Add(new Article(title));
                    }
                }
                else
                {
                    foreach (Match m in regexe2.Matches(PageText))
                    {
                        title = m.Groups[1].Value;
                        if (title != "Wikipedia:Special pages" && title != "Wikipedia talk:Special:Lonelypages" && title != "Wikipedia:Offline reports" && title != "Template:Specialpageslist")
                        {
                            ns = Tools.CalculateNS(title);
                            list.Add(new Article(title, ns));
                        }
                    }
                }
            }
            return FilterSomeArticles(list);
        }

        /// <summary>
        /// Gets a list of articles that use an image.
        /// </summary>
        /// <param name="Image">The image.</param>
        /// <returns>The list of the articles.</returns>
        public static List<Article> FromImageLinks(params string[] Images)
        {
            List<Article> list = new List<Article>();

            foreach (string I in Images)
            {
                string Image = Regex.Replace(I, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase);
                Image = encodeText(Image);

                string URL = Variables.URL + "query.php?what=imagelinks&titles=Image:" + Image + "&illimit=500&format=xml";
                string title = "";
                int ns = 0;

                while (true)
                {
                    string html = Tools.GetHTML(URL);
                    if (!html.Contains("<imagelinks>"))
                        throw new PageDoeNotExistException("The image " + Image + " does not exist. Make sure it is spelt correctly.");

                    bool more = false;

                    using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                    {
                        while (reader.Read())
                        {
                            if (reader.LocalName.Equals("query"))
                            {
                                reader.ReadToFollowing("imagelinks");
                                reader.MoveToAttribute("next");
                                URL = Variables.URL + "query.php?what=imagelinks&titles=Image:" + Image + "&illimit=500&format=xml&ilcontfrom=" + reader.Value;
                                more = true;
                                reader.ReadToFollowing("page");
                            }

                            if (reader.Name.Equals("il"))
                            {
                                if (reader.AttributeCount > 1)
                                {
                                    reader.MoveToAttribute("ns");
                                    ns = int.Parse(reader.Value);
                                }
                                else
                                    ns = 0;

                                title = reader.ReadString();
                                list.Add(new Article(title, ns));
                            }
                        }
                    }

                    if (!more)
                        break;
                }
            }

            return list;
        }

        readonly static Regex regexWatchList = new Regex("<LI><INPUT type=checkbox value=(.*?) name=id\\[\\]", RegexOptions.Compiled);
        public static List<Article> FromWatchList()
        {
            WebBrowser webbrowser = new WebBrowser();
            webbrowser.ScriptErrorsSuppressed = true;
            webbrowser.Navigate("http://en.wikipedia.org/wiki/Special:Watchlist/edit");
            while (webbrowser.ReadyState != WebBrowserReadyState.Complete) Application.DoEvents();

            string html = webbrowser.Document.Body.InnerHtml;
            string title = "";
            List<Article> list = new List<Article>();

            if (!html.Contains("<LI id=pt-logout"))
            {
                throw new PageDoeNotExistException("Please make sure you are logged into Wikipedia in Internet Explorer so your watch list can be obtained");
            }

            foreach (Match m in regexWatchList.Matches(html))
            {
                title = m.Groups[1].Value.Trim('"');
                title = title.Replace("&amp;", "&").Replace("&quot;", "\"");
                list.Add(new Article(title));
            }

            return list;
        }

        private static string encodeText(string txt)
        {
            txt = txt.Replace("{{", Variables.Namespaces[10]).Replace("}}", "");
            txt = txt.Replace(" ", "_");
            txt = HttpUtility.UrlEncode(txt);
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
                if (a.NameSpaceKey % 2 == 1)
                {
                    newList.Add(a);
                    continue;
                }
                else if (a.NameSpaceKey == 2 || a.NameSpaceKey == 4 || a.NameSpaceKey == 6 || a.NameSpaceKey == 8 || a.NameSpaceKey == 10 || a.NameSpaceKey == 12 || a.NameSpaceKey == 14 || a.NameSpaceKey == 16 || a.NameSpaceKey == 100)
                {
                    newList.Add(new Article(a.Name.Replace(":", Variables.TalkNS)));
                    continue;
                }
                else
                {
                    newList.Add(new Article(Variables.Namespaces[1] + a.Name));
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
                newList.Add(new Article(a.Name.Replace(Variables.Namespaces[1], "").Replace(Variables.TalkNS, ":")));
            }

            return newList;
        }

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
    public class PageDoeNotExistException : ApplicationException
    {
        public PageDoeNotExistException() { }
        public PageDoeNotExistException(string message) : base(message) { }

        public PageDoeNotExistException(string message, System.Exception inner) : base(message, inner) { }
        protected PageDoeNotExistException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }    
}
