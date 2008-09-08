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
    //TODO: normalise usage of FirstToUpperAndRemoveHashOnArray() and alikes


    /// <summary>
    /// Gets a list of pages in Named Categories for the ListMaker (Non-Recursive)
    /// </summary>
    public class CategoryListProvider : CategoryProviderBase
    {
        public override List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> list = new List<Article>();

            foreach (string cat in PrepareCategories(searchCriteria))
            {
                list.AddRange(GetListing(cat, list.Count));
            }

            return list;
        }

        public override string DisplayText
        { get { return "Category"; } }
    }

    /// <summary>
    /// Gets a list of pages in Named Categories for the ListMaker (Recursive - Will visit ALL subcategories)
    /// </summary>
    public class CategoryRecursiveListProvider : CategoryProviderBase
    {
        public const int MaxDepth = 30;

        int m_Depth = MaxDepth;
        /// <summary>
        /// Maximum recursion depth during category scan
        /// </summary>
        public int Depth
        {
            get { return m_Depth; }
            set { m_Depth = Math.Min(value, MaxDepth); }
        }

        public CategoryRecursiveListProvider()
            :this(MaxDepth)
        {
        }

        public CategoryRecursiveListProvider(int depth)
            :base()
        {
            Depth = depth;
            Limit = 200000;
        }

        public override List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> list = new List<Article>();

            lock (Visited)
            {
                Visited.Clear();
                foreach (string cat in PrepareCategories(searchCriteria))
                {
                    list.AddRange(RecurseCategory(cat, list.Count, Depth));
                }
                Visited.Clear();
            }

            return list;
        }

        public override string DisplayText
        { get { return "Category (recursive)"; } }
    }

    /// <summary>
    /// Gets a list of pages in Named Categories for the ListMaker (Recursive - Will visit 1 level of subcategories)
    /// </summary>
    public class CategoryRecursiveOneLevelListProvider : CategoryRecursiveListProvider
    {
        public CategoryRecursiveOneLevelListProvider()
            : base(1)
        { }

        public override string DisplayText
        {
            get { return "Category (recurse 1 level)"; }
        }
    }

    /// <summary>
    /// Gets a list of pages in Named Categories for the ListMaker (Recursive - Will visit the specified number of levels of subcategories)
    /// </summary>
    public class CategoryRecursiveUserDefinedLevelListProvider : CategoryRecursiveListProvider
    {
        public CategoryRecursiveUserDefinedLevelListProvider()
            : base(0)
        { }

        public override List<Article> MakeList(params string[] searchCriteria)
        {
            int userDepth = GetDepthFromUser();
            if (userDepth < 0) return new List<Article>();
            else
                Depth = userDepth;

            return base.MakeList(searchCriteria);
        }

        public int GetDepthFromUser()
        {
            using (WikiFunctions.Controls.LevelNumber num = new WikiFunctions.Controls.LevelNumber())
            {
                if (num.ShowDialog() != DialogResult.OK) return -1;
                return num.Levels;
            }
        }

        public override string DisplayText
        {
            get { return "Category (recurse user defined level)"; }
        }
    }

    /// <summary>
    /// Gets a list of pages from a text file
    /// </summary>
    public class TextFileListProvider : IListProvider
    {
        private readonly static Regex RegexFromFile = new Regex("(^[a-z]{2,3}:)|(simple:)", RegexOptions.Compiled);
        private readonly static Regex LoadWikiLink = new Regex(@"\[\[:?(.*?)(?:\]\]|\|)", RegexOptions.Compiled);
        private OpenFileDialog openListDialog;

        public TextFileListProvider()
        {
            openListDialog = new OpenFileDialog();
            openListDialog.Filter = "Text files|*.txt|All files|*.*";
            openListDialog.Multiselect = true;
        }

        public List<Article> MakeList(string searchCriteria)
        {
            return MakeList(searchCriteria.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public List<Article> MakeList()
        {
            return MakeList(new string[0]);
        }

        public List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> list = new List<Article>();
            try
            {
                if (searchCriteria.Length == 0 && openListDialog.ShowDialog() == DialogResult.OK)
                    searchCriteria = openListDialog.FileNames;

                foreach (string fileName in searchCriteria)
                {
                    string pageText = "";
                    string title = "";

                    using (StreamReader sr = new StreamReader(fileName, Encoding.UTF8))
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
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
                return list;
            }
        }

        #region ListMaker properties
        public string DisplayText
        { get { return "Text File"; } }

        public string UserInputTextBoxText
        { get { return ""; } }

        public bool UserInputTextBoxEnabled
        { get { return false; } }

        public void Selected() { }

        public bool RunOnSeparateThread
        { get { return false; } }
        #endregion
    }

    /// <summary>
    /// Gets a list of pages which link to the Named Pages
    /// </summary>
    public class WhatLinksHereListProvider : ApiListProviderBase
    {
        #region Tags: <backlinks>/<bl>
        static readonly List<string> pe = new List<string>(new string[] { "bl" });
        protected override ICollection<string> PageElements
        {
            get { return pe; }
        }

        static readonly List<string> ac = new List<string>(new string[] { "backlinks" });
        protected override ICollection<string> Actions
        {
            get { throw new NotImplementedException(); }
        }
        #endregion
        
        protected bool IncludeRedirects;

        public override List<Article> MakeList(params string[] searchCriteria)
        { 
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(searchCriteria);

            List<Article> list = new List<Article>();

            foreach (string page in searchCriteria)
            {
                string url = Variables.URLLong + "api.php?action=query&list=backlinks&bltitle=" 
                    + HttpUtility.UrlEncode(page) + "&format=xml&bllimit={limit}";

                if (IncludeRedirects)
                    url += "&blredirect";

                list.AddRange(ApiMakeList(url, list.Count));
            }
            return list;
        }

        #region ListMaker properties
        public override string DisplayText
        { get { return "What links here"; } }

        public override string UserInputTextBoxText
        { get { return "What links to:"; } }

        public override bool UserInputTextBoxEnabled
        { get { return true; } }

        public override void Selected() { }
        #endregion
    }

    /// <summary>
    /// Gets a list of pages which link to the Named Pages (including what links to the redirects)
    /// </summary>
    public class WhatLinksHereIncludingRedirectsListProvider : WhatLinksHereListProvider
    {
        public WhatLinksHereIncludingRedirectsListProvider()
            :base()
        {
            this.IncludeRedirects = true;
        }

        public override string DisplayText
        { get { return base.DisplayText + " (inc. Redirects)"; } }
    }

    /// <summary>
    /// Gets a list of pages which transclude the Named Pages
    /// </summary>
    public class WhatTranscludesPageListMakerProvider : ApiListProviderBase
    {
        #region Tags: <embeddedin>/<ei>
        static readonly List<string> pe = new List<string>(new string[] { "ei" });
        protected override ICollection<string> PageElements
        {
            get { return pe; }
        }

        static readonly List<string> ac = new List<string>(new string[] { "embeddedin" });
        protected override ICollection<string> Actions
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

        public override List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> list = new List<Article>();

            foreach (string page in searchCriteria)
            {
                string url = Variables.URLLong + "api.php?action=query&list=embeddedin&eititle="
                    + HttpUtility.UrlEncode(page) + "&eilimit={limit}&format=xml";

                list.AddRange(ApiMakeList(url, list.Count));
            }

            return list;
        }

        #region ListMaker properties
        public override string DisplayText
        { get { return "What transcludes page"; } }

        public override string UserInputTextBoxText
        { get { return "What embeds:"; } }

        public override bool UserInputTextBoxEnabled
        { get { return true; } }

        public override void Selected()
        { }
        #endregion
    }

    /// <summary>
    /// Gets a list of all links on the Named Pages
    /// </summary>
    public class LinksOnPageListProvider : ApiListProviderBase
    {
        #region Tags: <links>/<pl>
        static readonly List<string> pe = new List<string>(new string[] { "pl" });
        protected override ICollection<string> PageElements
        {
            get { return pe; }
        }

        static readonly List<string> ac = new List<string>(new string[] { "links" });
        protected override ICollection<string> Actions
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

        public override List<Article> MakeList(params string[] searchCriteria)
        { 
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(searchCriteria);

            List<Article> list = new List<Article>();

            foreach (string article in searchCriteria)
            {
                string url = Variables.URLLong + "api.php?action=query&prop=links&titles=" 
                    + HttpUtility.UrlEncode(article) + "&pllimit={limit}&format=xml";

                list.AddRange(ApiMakeList(url, list.Count));
            }

            return list;
        }

        #region ListMaker properties
        public override string DisplayText
        { get { return "Links on page"; } }

        public override string UserInputTextBoxText
        { get { return "Links on:"; } }

        public override bool UserInputTextBoxEnabled
        { get { return true; } }

        public override void Selected() { }
        #endregion
    }

    /// <summary>
    /// Gets a list of all Images on the Named Pages
    /// </summary>
    public class ImagesOnPageListProvider : ApiListProviderBase
    {
        #region Tags: <images>/<im>
        static readonly List<string> pe = new List<string>(new string[] { "im" });
        protected override ICollection<string> PageElements
        {
            get { return pe; }
        }

        static readonly List<string> ac = new List<string>(new string[] { "images" });
        protected override ICollection<string> Actions
        {
            get { return ac; }
        }
        #endregion

        public override List<Article> MakeList(params string[] searchCriteria)
        { 
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(searchCriteria);

            List<Article> list = new List<Article>();

            foreach (string article in searchCriteria)
            {
                string url = Variables.URLLong + "api.php?action=query&prop=images&titles=" 
                    + HttpUtility.UrlEncode(article) + "&imlimit={limit}&format=xml";

                list.AddRange(ApiMakeList(url, list.Count));
            }
            return list;
        }

        #region ListMaker properties
        public override string DisplayText
        { get { return "Images on page"; } }

        public override string UserInputTextBoxText
        { get { return "Images on:"; } }

        public override bool UserInputTextBoxEnabled
        { get { return true; } }

        public override void Selected() { }
        #endregion
    }

    /// <summary>
    /// Gets a list of all the transclusions on the Named Pages
    /// </summary>
    public class TransclusionsOnPageListProvider : ApiListProviderBase
    {
        #region Tags: <templates>/<tl>
        static readonly List<string> pe = new List<string>(new string[] { "tl" });
        protected override ICollection<string> PageElements
        {
            get { return pe; }
        }

        static readonly List<string> ac = new List<string>(new string[] { "templates" });
        protected override ICollection<string> Actions
        {
            get { return ac; }
        }
        #endregion

        public override List<Article> MakeList(params string[] searchCriteria)
        { 
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(searchCriteria);

            List<Article> list = new List<Article>();

            foreach (string article in searchCriteria)
            {
                string url = Variables.URLLong + "api.php?action=query&prop=templates&titles=" 
                    + HttpUtility.UrlEncode(article) + "&tllimit={limit}&format=xml";

                list.AddRange(ApiMakeList(url, list.Count));
            }
            return list;
        }

        #region ListMaker properties
        public override string DisplayText
        { get { return "Transclusions on page"; } }

        public override string UserInputTextBoxText
        { get { return "Transclusions on:"; } }

        public override bool UserInputTextBoxEnabled
        { get { return true; } }

        public override void Selected() { }
        #endregion
    }

    /// <summary>
    /// Gets a list of google results based on the named pages
    /// </summary>
    public class GoogleSearchListProvider : IListProvider
    {
        private static Regex regexGoogle = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+) class=l)",
    RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public List<Article> MakeList(params string[] searchCriteria)
        { 
            List<Article> list = new List<Article>();

            foreach (string g in searchCriteria)
            {
                int intStart = 0;
                string google = HttpUtility.UrlEncode(g);
                google = google.Replace("_", " ");
                string url = "http://www.google.com/search?q=" + google + "+site:" + Variables.URL + "&num=100&hl=en&lr=&start=0&sa=N&filter=0";
                string title = "";

                do
                {
                    string googleText = Tools.GetHTML(url, Encoding.Default);

                    //Find each match to the pattern
                    foreach (Match m in regexGoogle.Matches(googleText))
                    {
                        title = Tools.GetTitleFromURL(m.Groups[1].Value);

                        if (!string.IsNullOrEmpty(title))
                        {
                            list.Add(new WikiFunctions.Article(title));
                        }
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

        #region ListMaker properties
        public string DisplayText
        { get { return "Google Search"; } }

        public string UserInputTextBoxText
        { get { return "Google Search:"; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeparateThread
        { get { return true; } }
        #endregion
    }

    /// <summary>
    /// Gets the user contribs of the Named Users
    /// </summary>
    public class UserContribsListProvider : IListProvider
    {
        protected bool all;

        public virtual List<Article> MakeList(params string[] searchCriteria)
        {
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(searchCriteria);

            List<Article> list = new List<Article>();

            foreach (string u in searchCriteria)
            {
                string title = "";
                int ns = 0;
                string page = Tools.WikiEncode(Regex.Replace(u, Variables.NamespacesCaseInsensitive[14], ""));

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

        #region ListMaker properties
        public virtual string DisplayText
        { get { return "User contribs"; } }

        public string UserInputTextBoxText
        { get { return Variables.Namespaces[2]; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeparateThread
        { get { return true; } }
        #endregion
    }

    /// <summary>
    /// Gets a list of pages which link to the Named Images
    /// </summary>
    public class ImageFileLinksListProvider : ApiListProviderBase
    {
        #region Tags: <imageusage>/<iu>
        static readonly List<string> pe = new List<string>(new string[] { "iu" });
        protected override ICollection<string> PageElements
        {
            get { return pe; }
        }

        static readonly List<string> ac = new List<string>(new string[] { "imageusage" });
        protected override ICollection<string> Actions
        {
            get { return ac; }
        }
        #endregion

        public override List<Article> MakeList(params string[] searchCriteria)
        {
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(searchCriteria);

            List<Article> list = new List<Article>();

            foreach (string i in searchCriteria)
            {
                string image = Regex.Replace(i, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase);
                image = HttpUtility.UrlEncode(image);

                string url = Variables.URLLong + "api.php?action=query&list=imageusage&iutitle=Image:" 
                    + image + "&iulimit={limit}&format=xml";

                list.AddRange(ApiMakeList(url, list.Count));
            }
            return list;
        }

        #region ListMaker properties
        public override string DisplayText
        { get { return "Image file links"; } }

        public override string UserInputTextBoxText
        { get { return Variables.Namespaces[6]; } }

        public override bool UserInputTextBoxEnabled
        { get { return true; } }

        public override void Selected() { }
        #endregion
    }

    /// <summary>
    /// Gets a list of pages which are returned from a wiki search of the Named Pages
    /// </summary>
    public class WikiSearchListProvider : ApiListProviderBase
    {
        #region Tags: <search>/<p>
        static readonly List<string> pe = new List<string>(new string[] { "p" });
        protected override ICollection<string> PageElements
        {
            get { return pe; }
        }

        static readonly List<string> ac = new List<string>(new string[] { "search" });
        protected override ICollection<string> Actions
        {
            get { return ac; }
        }
        #endregion

        public WikiSearchListProvider()
        {
            Limit = 1000; // slow query
        }

        public override List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> list = new List<Article>();

            StringBuilder nsStringBuilder = new StringBuilder("&srnamespace=0|");

            // explicitly add available namespaces to search options
            foreach (int k in Variables.Namespaces.Keys)
            {
                if (k <= 0) continue;
                nsStringBuilder.Append(k + "|");
            }

            string ns = nsStringBuilder.ToString();
            ns = ns.Remove(ns.LastIndexOf('|'));

            foreach (string s in searchCriteria)
            {
                string url = Variables.URLLong + "api.php?action=query&list=search&srwhat=text&srsearch="
                    + s + "&srlimit={limit}&format=xml" + ns;

                list.AddRange(ApiMakeList(url, list.Count));
            }
            return list;
        }

        #region ListMaker properties
        public override string DisplayText
        { get { return "Wiki search"; } }

        public override string UserInputTextBoxText
        { get { return "Wiki search:"; } }

        public override bool UserInputTextBoxEnabled
        { get { return true; } }

        public override void Selected() { }
        #endregion
    }

    /// <summary>
    /// Gets a list of pages which redirect to the Named Pages
    /// </summary>
    public class RedirectsListProvider : ApiListProviderBase
    {
        #region Tags: <backlinks>/<bl>
        static readonly List<string> pe = new List<string>(new string[] { "bl" });
        protected override ICollection<string> PageElements
        {
            get { return pe; }
        }

        static readonly List<string> ac = new List<string>(new string[] { "backlinks" });
        protected override ICollection<string> Actions
        {
            get { return ac; }
        }
        #endregion

        public override List<Article> MakeList(params string[] searchCriteria)
        {
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(searchCriteria);

            List<Article> list = new List<Article>();

            foreach (string onePage in searchCriteria)
            {

                string page = HttpUtility.UrlEncode(onePage);
                string url = Variables.URLLong + "api.php?action=query&list=backlinks&bltitle="
                    + page + "&bllimit={limit}&blfilterredir=redirects&format=xml";

                list.AddRange(ApiMakeList(url, list.Count));
            }
            return list;
        }

        #region ListMaker properties
        public override string DisplayText
        { get { return "Redirects"; } }

        public override string UserInputTextBoxText
        { get { return "Redirects to:"; } }

        public override bool UserInputTextBoxEnabled
        { get { return true; } }

        public override void Selected() { }
        #endregion
    }

    /// <summary>
    /// Gets all the pages from the current user's watchlist
    /// </summary>
    public class MyWatchlistListProvider : IListProvider
    {
        public List<Article> MakeList(params string[] searchCriteria)
        {
            WikiFunctions.Browser.WebControl webbrowser = new WikiFunctions.Browser.WebControl();
            webbrowser.ScriptErrorsSuppressed = true;
            webbrowser.Navigate(Variables.URLLong + "index.php?title=Special:Watchlist&action=raw");
            webbrowser.Wait();

            List<Article> list = new List<Article>();

            HtmlElement textarea = webbrowser.Document.GetElementById("titles");
            string html;
            if (textarea == null || (html = textarea.InnerText) == null) return list;

            try
            {
                string[] splitter = { "\r\n" };
                foreach (string entry in html.Split(splitter, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (entry.Length > 0)
                        list.Add(new Article(entry));
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }

            return list;
        }

        #region ListMaker properties
        public string DisplayText
        { get { return "My Watchlist"; } }

        public string UserInputTextBoxText
        { get { return ""; } }

        public bool UserInputTextBoxEnabled
        { get { return false; } }

        public void Selected() { }

        public bool RunOnSeparateThread
        { get { return true; } }
        #endregion
    }

    /// <summary>
    /// Runs the Database Scanner
    /// </summary>
    public class DatabaseScannerListProvider : IListProvider
    {
        private ListBox listMakerListbox;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="lb">List box for DBScanner to add articles to</param>
        public DatabaseScannerListProvider(ListBox lb)
        {
            this.listMakerListbox = lb;
        }

        public List<Article> MakeList(params string[] searchCriteria)
        {
            WikiFunctions.DBScanner.DatabaseScanner ds = new WikiFunctions.DBScanner.DatabaseScanner(listMakerListbox);
            ds.Show();
            return null;
        }

        #region ListMaker properties
        public string DisplayText
        { get { return "Database dump"; } }

        public string UserInputTextBoxText
        { get { return ""; } }

        public bool UserInputTextBoxEnabled
        { get { return false; } }

        public void Selected() { }

        public bool RunOnSeparateThread
        { get { return false; } }
        #endregion
    }

    /// <summary>
    /// Gets 100 random articles from the 0 namespace
    /// </summary>
    public class RandomPagesListProvider : ApiListProviderBase
    {
        #region Tags: <random>/<page>
        static readonly List<string> pe = new List<string>(new string[] { "page" });
        protected override ICollection<string> PageElements
        {
            get { return pe; }
        }

        static readonly List<string> ac = new List<string>(new string[] { "random" });
        protected override ICollection<string> Actions
        {
            get { return ac; }
        }
        #endregion

        public override List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> list = new List<Article>();

            string url = Variables.URLLong + "api.php?action=query&list=random&rnnamespace=0&rnlimit=10&format=xml";

            for (int i = 0; i < 10; i++)
            {
                list.AddRange(ApiMakeList(url, 0));
            }
            return list;
        }

        #region ListMaker properties
        public override string DisplayText
        {
            get { return "Random Pages"; }
        }

        public override string UserInputTextBoxText
        {
            get { return ""; }
        }

        public override bool UserInputTextBoxEnabled
        {
            get { return false; }
        }

        public override void Selected() { }
        #endregion
    }
}
