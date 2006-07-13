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

namespace WikiFunctions
{
    /// <summary>
    /// Provides functionality to create and manipulate arrayLists of articles from many different sources
    /// </summary>
    public class GetLists
    {
        public GetLists()
        {
        }

        readonly Regex regexe = new Regex("<li>\\(?<a href=\"[^\"]*\" title=\"([^\"]*)\">[^<>]*</a>( \\(transclusion\\))?", RegexOptions.Compiled);
        readonly Regex wikiLinkReg = new Regex("\\[\\[(.*?)(\\]\\]|\\|)", RegexOptions.Compiled);
        readonly Regex regexe2 = new Regex("<a href=\"[^\"]*\" title=\"([^\"]*)\">[^<>]*</a>");

        /// <summary>
        /// Gets a list of articles and sub-categories in a category.
        /// </summary>
        /// <param name="Category">The category.</param>
        /// <returns>The Dictionary (string, int) of the articles.</returns>
        public Dictionary<string, int> FromCategory(string Category)
        {
            Dictionary<string, int> list = new Dictionary<string, int>();
            string origURL = Variables.URL + "/query.php?what=category&cptitle=" + encodeText(Category) + "&format=xml&cplimit=500";
            string URL = origURL;
            int ns = 0;

            while (true)
            {
                string html = Tools.GetHTML(URL);
                if (html.Contains("<error>emptyresult</error>"))
                    throw new Exception("The category " + Category + " does not exist. Make sure it is spelt correctly. If you want a stub category remember to type the category name and not the stub name.");

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
                            list.Add(reader.ReadString(), ns);
                        }
                    }
                }

                if (!more)
                    break;
            }

            return list;
        }

        /// <summary>
        /// Gets a list of articles that link to the given page.
        /// </summary>
        /// <param name="Page">The page to find links to.</param>
        /// <param name="RedirectsOnly">Only list redirects.</param>
        /// <param name="Embedded">Gets articles that embed (transclude).</param>
        /// <returns>The ArrayList of the articles.</returns>
        public Dictionary<string, int> FromWhatLinksHere(string Page, bool Embedded)
        {
            string request = "backlinks";
            string initial = "bl";
            if (Embedded)
            {
                request = "embeddedin";
                initial = "ei";
            }

            string OrigURL = Variables.URL + "query.php?what=" + request + "&titles=" + encodeText(Page) + "&" + initial + "limit=500&format=xml";
            string URL = OrigURL;
            Dictionary<string, int> list = new Dictionary<string, int>();
            string title = "";
            int ns = 0;

            while (true)
            {
                string html = Tools.GetHTML(URL);
                if (html.Contains("<" + request + " error=\"emptyrequest\" />"))
                    throw new Exception("No pages link to " + Page + ". Make sure it is spelt correctly.");

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
                                URL = OrigURL + "&blcontfrom=" + reader.Value;
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

                            if (title.Length > 0 )
                                list.Add(title, ns);
                        }
                    }
                }

                if (!more)
                    break;
            }

            return list;
        }

        /// <summary>
        /// Gets a list of links on a page.
        /// </summary>
        /// <param name="Article">The page to find links on.</param>
        /// <returns>The ArrayList of the links.</returns>
        public ArrayList FromLinksOnPage(string Article)
        {
            Article = encodeText(Article);
            ArrayList ArticleArray = new ArrayList();
            string x = "";

            string PageText = Tools.GetHTML(Variables.URL + "index.php?title=" + Article + "&action=edit");

            if (PageText.Contains("<title>Article not found - Wikipedia, the free encyclopedia</title>"))
                throw new Exception("The page " + Article + " does not exist. Make sure it is spelt correctly.");

            PageText = PageText.Substring(PageText.IndexOf("<textarea"));
            PageText = PageText.Substring(0, PageText.IndexOf("</textarea>"));

            foreach (Match m in wikiLinkReg.Matches(PageText))
            {
                x = m.Groups[1].Value;

                if (!(Regex.IsMatch(x, "(^[a-z]{2,3}:)|(simple:)")) && (!(x.StartsWith("#"))))
                    ArticleArray.Add(Tools.TurnFirstToUpper(x));
            }
            return FilterSomeArticles(ArticleArray);
        }

        /// <summary>
        /// Gets a list of [[wiki]] style links from txt file.
        /// </summary>
        /// <param name="fileName">The file path of the list.</param>
        /// <returns>The ArrayList of the links.</returns>
        public ArrayList FromTextFile(string fileName)
        {
            ArrayList ArticleArray = new ArrayList();
            string PageText = "";
            string x = "";

            StreamReader sr = new StreamReader(fileName, Encoding.UTF8);
            PageText = sr.ReadToEnd();
            sr.Close();

            foreach (Match m in wikiLinkReg.Matches(PageText))
            {
                x = m.Groups[1].Value;
                if (!(Regex.IsMatch(x, "(^[a-z]{2,3}:)|(simple:)")) && (!(x.StartsWith("#"))) && !ArticleArray.Contains(x))
                    ArticleArray.Add(Tools.TurnFirstToUpper(x));
            }

            return ArticleArray;
        }

        /// <summary>
        /// Gets a list from a google search of the site.
        /// </summary>
        /// <param name="Google">The term to search for.</param>
        /// <returns>The ArrayList of the articles.</returns>
        public ArrayList FromGoogleSearch(string Google)
        {
            int intStart = 0;
            Google = encodeText(Google);
            Google = Google.Replace("_", " ");
            string URL = "http://www.google.com/search?q=" + Google + "+site:" + Variables.URLShort + "&num=100&hl=en&lr=&start=0&sa=N";
            ArrayList ArticleArray = new ArrayList();

            do
            {
                string GoogleText = Tools.GetHTML(URL);

                GoogleText = HttpUtility.HtmlDecode(GoogleText);

                //Remove googles bold highlighting.
                GoogleText = Regex.Replace(GoogleText, "<[bB]>|</[Bb]>", "");

                //Regex pattern to find links
                string pattern = "\">([^<]*) - " + Variables.Namespaces[4].Remove(3);

                //Find each match to the pattern
                foreach (Match m in Regex.Matches(GoogleText, pattern))
                {
                    //add it to the list
                    ArticleArray.Add(m.Groups[1].Value);
                }

                if (GoogleText.Contains("<br>Next</a>"))
                {
                    intStart += 100;
                    URL = "http://www.google.com/search?q=" + Google + "+site:" + Variables.URLShort + "&num=100&hl=en&lr=&start=" + intStart.ToString() + "&sa=N";
                }
                else
                    break;

            } while (true);

            return FilterSomeArticles(ArticleArray);
        }

        /// <summary>
        /// Gets a list from a users contribs.
        /// </summary>
        /// <param name="User">The name of the user.</param>
        /// <returns>The ArrayList of the articles.</returns>
        public ArrayList FromUserContribs(string User)
        {
            User = Regex.Replace(User, "^" + Variables.Namespaces[2], "", RegexOptions.IgnoreCase);
            User = encodeText(User);
            ArrayList ArticleArray = new ArrayList();

            string PageText = Tools.GetHTML(Variables.URL + "index.php?title=Special:Contributions&target=" + User + "&offset=0&limit=2500");

            foreach (Match m in Regex.Matches(PageText, "<li>.*? title=\"([^\"]*)\">[^<>]*</a>"))
            {
                ArticleArray.Add(m.Groups[1].Value);
            }

            return FilterSomeArticles(ArticleArray);
        }

        /// <summary>
        /// Gets a list of links on a special page.
        /// </summary>
        /// <param name="Special">The page to find links on, e.g. "Deadendpages" or "Deadendpages&limit=500&offset=0".</param>
        /// <returns>The ArrayList of the articles.</returns>
        public ArrayList FromSpecialPage(string Special)
        {
            Special = Regex.Replace(Special, "^" + Variables.Namespaces[-1], "", RegexOptions.IgnoreCase);
            ArrayList ArticleArray = new ArrayList();

            string PageText = Tools.GetHTML(Variables.URL + "index.php?title=Special:" + Special);

            PageText = PageText.Substring(PageText.IndexOf("<!-- start content -->"), PageText.IndexOf("<!-- end content -->") - PageText.IndexOf("<!-- start content -->"));
            string s = "";

            if (regexe.IsMatch(PageText))
            {
                foreach (Match m in regexe.Matches(PageText))
                {
                    s = m.Groups[1].Value;
                    ArticleArray.Add(s);
                }
            }
            else
            {
                foreach (Match m in regexe2.Matches(PageText))
                {
                    s = m.Groups[1].Value;
                    if (s != "Wikipedia:Special pages" && s != "Wikipedia talk:Special:Lonelypages" && s != "Wikipedia:Offline reports" && s != "Template:Specialpageslist")
                        ArticleArray.Add(s);
                }
            }

            return FilterSomeArticles(ArticleArray);
        }

        /// <summary>
        /// Gets a list of articles that use an image.
        /// </summary>
        /// <param name="Image">The image.</param>
        /// <returns>The ArrayList of the articles.</returns>
        public Dictionary<string, int> FromImageLinks(string Image)
        {
            Image = Regex.Replace(Image, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase);
            Image = encodeText(Image);

            string URL = Variables.URL + "query.php?what=imagelinks&titles=Image:"+ Image +"&illimit=500&format=xml";
            Dictionary<string, int> list = new Dictionary<string, int>();
            string title = "";
            int ns = 0;   

            while (true)
            {
                string html = Tools.GetHTML(URL);
                if (!html.Contains("<imagelinks>"))
                    throw new Exception("The image " + Image + " does not exist. Make sure it is spelt correctly.");

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

                            if (title.Length > 0)
                                list.Add(title, ns);
                        }
                    }
                }

                if (!more)
                    break;
            }

            return list;
        }

        readonly Regex regexWatchList = new Regex("<LI><INPUT type=checkbox value=(.*?) name=id\\[\\]", RegexOptions.Compiled);
        public ArrayList FromWatchList()
        {
            WebBrowser webbrowser = new WebBrowser();
            webbrowser.ScriptErrorsSuppressed = true;
            webbrowser.Navigate("http://en.wikipedia.org/wiki/Special:Watchlist/edit");
            while (webbrowser.ReadyState != WebBrowserReadyState.Complete) Application.DoEvents();

            string html = webbrowser.Document.Body.InnerHtml;
            string article = "";
            ArrayList list = new ArrayList();

            if (!html.Contains("<LI id=pt-logout"))
            {
                MessageBox.Show("Please make sure you are logged into Wikipedia in Internet Explorer so your watch list can be obtained", "Log in", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return list;
            }

            foreach (Match m in regexWatchList.Matches(html))
            {
                article = m.Groups[1].Value.Trim('"');
                article = article.Replace("&amp;", "&").Replace("&quot;", "\"");
                list.Add(article);
            }

            return list;
        }

        private string encodeText(string txt)
        {
            txt = txt.Replace("{{", Variables.Namespaces[10]).Replace("}}", "");
            txt = txt.Replace(" ", "_");
            txt = HttpUtility.UrlEncode(txt);
            return txt;
        }

        private ArrayList FilterSomeArticles(ArrayList UnfilteredArticles)
        {
            //Filter out artcles which we definately do not want to edit and remove duplicates.
            ArrayList items = new ArrayList();
            foreach (string s in UnfilteredArticles)
            {
                if ((!(s.StartsWith(Variables.Namespaces[-1]) || s.StartsWith(Variables.Namespaces[100]) || s.StartsWith("Commons:") || s.StartsWith(Variables.Namespaces[8]))))// && (!items.Contains(s))
                {
                    string z = s.Replace("&amp;", "&").Replace("&quot;", "\"").Replace("_", " ");
                    if (z.Contains("\""))
                    {
                        z = z.Replace("'", "");
                    }
                    items.Add(z);
                }
            }

            return items;
        }

        /// <summary>
        /// Turns an arraylist of articles into an arraylist of the associated talk pages.
        /// </summary>
        /// <param name="list">The arrayList of articles.</param>
        /// <returns>The ArrayList of the talk pages.</returns>
        public ArrayList convertToTalk(ArrayList list)
        {
            ArrayList newList = new ArrayList();
            int ns = 0;
            foreach (string s in list)
            {
                ns = Tools.CalculateNS(s);

                if (ns % 2 == 1)
                {
                    newList.Add(s);
                    continue;
                }
                else if (ns == 2 || ns == 4 || ns == 6 || ns == 8 || ns == 10 || ns == 12 || ns == 14 || ns == 16 || ns == 100)
                {
                    newList.Add(s.Replace(":", Variables.TalkNS));
                    continue;
                }
                else
                {
                    newList.Add(Variables.Namespaces[1] + s);
                    continue;
                }
            }

            return newList;
        }

        /// <summary>
        /// Turns an arraylist of talk pages into an arraylist of the associated articles.
        /// </summary>
        /// <param name="list">The arrayList of talk pages.</param>
        /// <returns>The ArrayList of articles.</returns>
        public ArrayList convertFromTalk(ArrayList list)
        {
            ArrayList newList = new ArrayList();

            foreach (string s in list)
            {
                newList.Add(s.Replace(Variables.Namespaces[1], "").Replace(Variables.TalkNS, ":"));
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
}
