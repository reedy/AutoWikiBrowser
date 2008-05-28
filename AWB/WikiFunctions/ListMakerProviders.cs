/*
Copyright (C) 2007 Martin Richards
(C) 2008 Stephen Kennedy, Sam Reed

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

namespace WikiFunctions.Lists
{
    #region ListMakerProviders
    /// <summary>
    /// Gets a list of pages in Named Categories for the ListMaker (Non-Recursive)
    /// </summary>
    public class CategoryListMakerProvider : IListMakerProvider
    {
        protected bool quietMode = false;
        protected bool subCategories = false;

        public CategoryListMakerProvider()
        { }

        /// <param name="QuietMode">Whether errors should be supressed</param>
        public CategoryListMakerProvider(bool QuietMode)
        {
            this.quietMode = QuietMode;
        }

        public virtual List<Article> MakeList(string[] searchCriteria)
        {
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(Tools.RegexReplaceOnArray(searchCriteria, "^" + Variables.NamespacesCaseInsensitive[14], ""));

            List<Article> list = new List<Article>();
            List<string> badcategories = new List<string>();
            List<string> vistedCategories = new List<string>();

            for (int i = 0; i < searchCriteria.Length; i++)
            {
                if (!vistedCategories.Contains(searchCriteria[i]))
                {
                    vistedCategories.Add(searchCriteria[i]);
                    string cmtitle = Tools.WikiEncode(Regex.Replace(searchCriteria[i], Variables.NamespacesCaseInsensitive[14], ""));

                    string url = Variables.URLLong + "api.php?action=query&list=categorymembers&cmtitle=Category:" + cmtitle + "&cmcategory=" + cmtitle + "&format=xml&cmlimit=500";
                    int ns = 0;

                    while (true)
                    {
                        string title = "";
                        string html = Tools.GetHTML(url);
                        if (html.Contains("categorymembers /"))
                        {
                            badcategories.Add(searchCriteria[i]);
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
                                        Array.Resize<string>(ref searchCriteria, searchCriteria.Length + 1);
                                        searchCriteria[searchCriteria.Length - 1] = title.Replace(Variables.Namespaces[14], "");
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
            if (badcategories.Count != 0 && !this.quietMode)
            {
                StringBuilder errorMessage = new StringBuilder("The following Categories are empty or do not exist:");

                foreach (string badcat in badcategories)
                    errorMessage.AppendLine(" ● " + badcat);

                MessageBox.Show(errorMessage.ToString());
            }
            return list;
        }

        public virtual string DisplayText
        { get { return "Category"; } }

        public string UserInputTextBoxText
        { get { return Variables.Namespaces[14]; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeperateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets a list of pages in Named Categories for the ListMaker (Recursive - Will visit ALL subcategories)
    /// </summary>
    internal sealed class CategoryRecursiveListMakerProvider : CategoryListMakerProvider
    {
        public CategoryRecursiveListMakerProvider()
        {
            this.subCategories = true;
            this.quietMode = true;
        }
        public override List<Article> MakeList(string[] searchCriteria)
        {
            List<Article> ret = base.MakeList(searchCriteria);
            return ret;
        }

        public override string DisplayText
        { get { return "Category (recursive)"; } }
    }

    /// <summary>
    /// Gets a list of pages from a text file
    /// </summary>
    internal sealed class TextFileListMakerProvider : IListMakerProvider
    {
        private readonly static Regex RegexFromFile = new Regex("(^[a-z]{2,3}:)|(simple:)", RegexOptions.Compiled);
        private readonly static Regex LoadWikiLink = new Regex(@"\[\[:?(.*?)(?:\]\]|\|)", RegexOptions.Compiled);
        private OpenFileDialog openListDialog;

        public TextFileListMakerProvider()
        {
            openListDialog = new OpenFileDialog();
            openListDialog.Filter = "text files|*.txt|All files|*.*";
            openListDialog.Multiselect = true;
        }

        public List<Article> MakeList(string searchCriteria)
        {
            return MakeList(searchCriteria.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public List<Article> MakeList(string[] searchCriteria)
        {
            List<Article> list = new List<Article>();
            try
            {
                if (openListDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string fileName in searchCriteria)
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
                }
                return list;
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
                return list;
            }
        }
        
        public string DisplayText
        { get { return "Text File"; } }

        public string UserInputTextBoxText
        { get { return "From file:"; } }

        public bool UserInputTextBoxEnabled
        { get { return false; } }

        public void Selected() { }

        public bool RunOnSeperateThread
        { get { return false; } }
    }

    /// <summary>
    /// Gets a list of pages which link to the Named Pages
    /// </summary>
    internal class WhatLinksHereListMakerProvider : IListMakerProvider
    {
        protected bool embedded = false;
        protected bool includeRedirects = false;

        public List<Article> MakeList(string searchCriteria)
        {
            return MakeList(searchCriteria.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public virtual List<Article> MakeList(string[] searchCriteria)
        { 
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(searchCriteria);

            string request = "backlinks";
            string initial = "bl";
            if (embedded)
            {
                request = "embeddedin";
                initial = "ei";
            }
            List<Article> list = new List<Article>();

            foreach (string page in searchCriteria)
            {
                if (page.Trim().Length == 0) continue;
                string url = Variables.URLLong + "api.php?action=query&list=" + request + "&" + initial + "title=" + Tools.RemoveHashFromPageTitle(Tools.WikiEncode(page)) + "&format=xml&" + initial + "limit=500";

                if (includeRedirects)
                    url += "&blredirect";

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
                                    url = Variables.URLLong + "api.php?action=query&list=" + request + "&" + initial + "title=" + Tools.WikiEncode(page) + "&format=xml&" + initial + "limit=500&" + initial + "continue=" + continueFrom;

                                    if (includeRedirects)
                                        url += "&blredirect";

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

        public virtual string DisplayText
        { get { return "What links here"; } }

        public virtual string UserInputTextBoxText
        { get { return "What links to:"; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeperateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets a list of pages which link to the Named Pages (including what links to the redirects)
    /// </summary>
    internal sealed class WhatLinksHereIncludingRedirectsListMakerProvider : WhatLinksHereListMakerProvider
    {
        public WhatLinksHereIncludingRedirectsListMakerProvider()
        {
            this.includeRedirects = true;
        }

        public override List<Article> MakeList(string[] searchCriteria)
        { return base.MakeList(searchCriteria); }

        public override string DisplayText
        { get { return base.DisplayText + " (inc. Redirects)"; } }
    }

    /// <summary>
    /// Gets a list of pages which transclude the Named Pages
    /// </summary>
    internal sealed class WhatTranscludesPageListMakerProvider : WhatLinksHereListMakerProvider
    {
        public WhatTranscludesPageListMakerProvider()
        {
            this.embedded = true;
        }

        public override List<Article> MakeList(string[] searchCriteria)
        { return base.MakeList(searchCriteria); }

        public override string DisplayText
        { get { return "What transcludes page"; } }

        public override string UserInputTextBoxText
        { get { return "What embeds:"; } }
    }

    /// <summary>
    /// Gets a list of all links on the Named Pages
    /// </summary>
    public class LinksOnPageListMakerProvider : IListMakerProvider
    {
        public List<Article> MakeList(string[] searchCriteria)
        { 
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(searchCriteria);

            List<Article> list = new List<Article>();

            foreach (string article in searchCriteria)
            {
                try
                {
                    string url = Variables.URLLong + "api.php?action=query&prop=links&titles=" + Tools.WikiEncode(article) + "&format=xml";
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

       public string DisplayText
        { get { return "Links on page"; } }

        public string UserInputTextBoxText
        { get { return "Links on:"; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeperateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets a list of all Images on the Named Pages
    /// </summary>
    internal sealed class ImagesOnPageListMakerProvider : IListMakerProvider
    {
        public List<Article> MakeList(string[] searchCriteria)
        { 
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(searchCriteria);

            List<Article> list = new List<Article>();

            foreach (string article in searchCriteria)
            {
                string url = Variables.URLLong + "api.php?action=query&prop=images&titles=" + Tools.WikiEncode(article) + "&format=xml";

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

        public string DisplayText
        { get { return "Images on page"; } }

        public string UserInputTextBoxText
        { get { return "Images on:"; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeperateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets a list of all the transclusions on the Named Pages
    /// </summary>
    internal sealed class TransclusionsOnPageListMakerProvider : IListMakerProvider
    {
        public List<Article> MakeList(string[] searchCriteria)
        { 
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(searchCriteria);

            List<Article> list = new List<Article>();

            foreach (string article in searchCriteria)
            {
                string url = Variables.URLLong + "api.php?action=query&prop=templates&titles=" + Tools.WikiEncode(article) + "&format=xml";

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

        public string DisplayText
        { get { return "Transclusions on page"; } }

        public string UserInputTextBoxText
        { get { return "Transclusions on:"; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeperateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets a list of google results based on the named pages
    /// </summary>
    internal sealed class GoogleSearchListMakerProvider : IListMakerProvider
    {
        private static Regex regexGoogle = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+) class=l)",
    RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public List<Article> MakeList(string[] searchCriteria)
        { 
            List<Article> list = new List<Article>();

            foreach (string g in searchCriteria)
            {
                int intStart = 0;
                string google = Tools.WikiEncode(g);
                google = google.Replace("_", " ");
                string url = "http://www.google.com/search?q=" + google + "+site:" + Variables.URL + "&num=100&hl=en&lr=&start=0&sa=N&filter=0";
                string title = "";

                do
                {
                    string googleText = Tools.GetHTML(url, Encoding.Default);

                    //Find each match to the pattern
                    foreach (Match m in regexGoogle.Matches(googleText))
                    {
                        title = m.Groups[1].Value;
                        if (!title.StartsWith(Variables.URL + "/wiki/")) continue;

                        list.Add(new WikiFunctions.Article(Tools.GetPageFromURL(title)));
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
            return Tools.FilterSomeArticles(list);
        }

        public string DisplayText
        { get { return "Google Search"; } }

        public string UserInputTextBoxText
        { get { return "Google Search:"; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeperateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets the user contribs of the Named Users
    /// </summary>
    internal class UserContribsListMakerProvider : IListMakerProvider
    {
        protected bool all = false;

        public virtual List<Article> MakeList(string[] searchCriteria)
        {
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(Tools.RegexReplaceOnArray(searchCriteria, "^" + Variables.NamespacesCaseInsensitive[2], ""));

            List<Article> list = new List<Article>();

            foreach (string u in searchCriteria)
            {
                string title = "";
                int ns = 0;
                string page = u;
                page = Tools.WikiEncode(page);

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

        public virtual string DisplayText
        { get { return "User contribs"; } }

        public string UserInputTextBoxText
        { get { return Variables.Namespaces[2]; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeperateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets ALL the user contribs of the Named Users
    /// </summary>
    internal sealed class UserContribsAllListMakerProvider : UserContribsListMakerProvider
    {
        public UserContribsAllListMakerProvider()
        {
            this.all = true;
        }

        public override List<Article> MakeList(string[] searchCriteria)
        {
            return base.MakeList(searchCriteria);
        }

        public override string DisplayText
        { get { return base.DisplayText + " (all)"; } }
    }

    /// <summary>
    /// Gets the list of pages on the Named Special Pages
    /// </summary>
    internal sealed class SpecialPageListMakerProvider : IListMakerProvider
    {
        readonly static Regex regexli = new Regex("<li>.*</li>", RegexOptions.Compiled);
        readonly static Regex regexe = new Regex("<li>\\(?<a href=\"[^\"]*\" title=\"([^\"]*)\">[^<>]*</a> \\(redirect page\\)", RegexOptions.Compiled);
        readonly static Regex regexe2 = new Regex("<a href=\"[^\"]*\" title=\"([^\"]*)\">[^<>]*</a>", RegexOptions.Compiled);
        readonly static Regex regexLog = new Regex(@"<li>.*?<a .*?</a> \(<a .*?</a>\).*?<a href=""[^""]*""[^>]* title=""([^""]*)"">[^<>]*</a>", RegexOptions.Compiled);

        public List<Article> MakeList(string[] searchCriteria)
        {
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(Tools.RegexReplaceOnArray(searchCriteria, "^" + Variables.NamespacesCaseInsensitive[-1], ""));

            //TODO:Fix!
            List<Article> list = new List<Article>();

            int limit = 1000;

            foreach (string s in searchCriteria)
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
            return Tools.FilterSomeArticles(list);
        }

        public string DisplayText
        { get { return "Special page"; } }

        public string UserInputTextBoxText
        { get { return Variables.Namespaces[-1]; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeperateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets a list of pages which link to the Named Images
    /// </summary>
    public class ImageFileLinksListMakerProvider : IListMakerProvider
    {
        public List<Article> MakeList(string[] searchCriteria)
        {
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(Tools.RegexReplaceOnArray(searchCriteria, "^" + Variables.NamespacesCaseInsensitive[6], ""));

            List<Article> list = new List<Article>();

            foreach (string i in searchCriteria)
            {
                string image = Regex.Replace(i, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase);
                image = Tools.WikiEncode(image);

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

        public string DisplayText
        { get { return "Image file links"; } }

        public string UserInputTextBoxText
        { get { return Variables.Namespaces[6]; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeperateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets a list of pages which are returned from a wiki search of the Named Pages
    /// </summary>
    internal sealed class WikiSearchListMakerProvider : IListMakerProvider
    {
        private static Regex searchRegex = new Regex("<li><a .*? title=\\\"([^\"]*)\">", RegexOptions.Compiled);

        public List<Article> MakeList(string[] searchCriteria)
        {
            //TODO:api.php?
            List<Article> list = new List<Article>();

            string ns = "&ns0=1";

            // explicitly add available namespaces to search options
            foreach (int k in Variables.Namespaces.Keys)
            {
                if (k <= 0) continue;
                ns += "&ns" + k.ToString() + "=1";
            }

            foreach (string s in searchCriteria)
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
                        title = HttpUtility.HtmlDecode(title);
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

        public string DisplayText
        { get { return "Wiki search"; } }

        public string UserInputTextBoxText
        { get { return "Wiki search:"; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeperateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets a list of pages which redirect to the Named Pages
    /// </summary>
    internal sealed class RedirectsListMakerProvider : IListMakerProvider
    {
        public List<Article> MakeList(string[] searchCriteria)
        {
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(searchCriteria);

            List<Article> list = new List<Article>();

            foreach (string onePage in searchCriteria)
            {
                try
                {
                    string title = "";
                    int ns = 0;
                    string page = onePage;
                    page = Tools.WikiEncode(page);
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
                    ErrorHandler.Handle(ex);
                }
            }
            return list;
        }

        public string DisplayText
        { get { return "Redirects"; } }

        public string UserInputTextBoxText
        { get { return "Redirects to:"; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeperateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets all the pages from the Current Users Watchlist
    /// </summary>
    public class MyWatchlistListMakerProvider : IListMakerProvider
    {
        public List<Article> MakeList(string[] searchCriteria)
        { return FromWatchList(); }

        static List<Article> FromWatchList()
        {
            WikiFunctions.Browser.WebControl webbrowser = new WikiFunctions.Browser.WebControl();
            webbrowser.ScriptErrorsSuppressed = true;
            webbrowser.Navigate(Variables.URLLong + "index.php?title=Special:Watchlist&action=raw");
            webbrowser.Wait();

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

        public string DisplayText
        { get { return "My Watchlist"; } }

        public string UserInputTextBoxText
        { get { return ""; } }

        public bool UserInputTextBoxEnabled
        { get { return false; } }

        public void Selected() { }

        public bool RunOnSeperateThread
        { get { return true; } }
    }

    /// <summary>
    /// Runs the Database Scanner
    /// </summary>
    internal sealed class DatabaseScannerListMakerProvider : IListMakerProvider
    {
        private ListBox listMakerListbox;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="lb">List box for DBScanner to add articles to</param>
        public DatabaseScannerListMakerProvider(ListBox lb)
        {
            this.listMakerListbox = lb;
        }

        #region IListMakerProvider Members

        public List<Article> MakeList(string[] searchCriteria)
        {
            WikiFunctions.DBScanner.DatabaseScanner ds = new WikiFunctions.DBScanner.DatabaseScanner(listMakerListbox);
            ds.Show();
            return null;
        }

        public string DisplayText
        { get { return "Database dump"; } }

        public string UserInputTextBoxText
        { get { return ""; } }

        public bool UserInputTextBoxEnabled
        { get { return false; } }

        public void Selected() { }

        public bool RunOnSeperateThread
        { get { return false; } }

        #endregion
    }
    #endregion
}
