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
    /// <summary>
    /// To-be base for all API-based providers
    /// </summary>
    public abstract class ApiListMakerProvider : IListProvider
    {
        /// <summary>
        /// Gets the list of XML elements that represent pages,
        /// e.g. <p>, <cm>, <bl> etc
        /// </summary>
        protected abstract string[] PageElements { get; }

        #region To be overridden

        public List<Article> MakeList(string[] searchCriteria)
        {
            throw new NotImplementedException();
        }

        public abstract string DisplayText { get; }

        public abstract string UserInputTextBoxText { get; }

        public abstract bool UserInputTextBoxEnabled { get; }

        public abstract void Selected();

        public abstract bool RunOnSeparateThread { get; }
        
        #endregion
    }


    #region ListMakerProviders
    /// <summary>
    /// Gets a list of pages in Named Categories for the ListMaker (Non-Recursive)
    /// </summary>
    public class CategoryListMakerProvider : IListProvider
    {
        protected bool quietMode;
        protected bool subCategories;
        protected List<string> vistedCategories;

        public CategoryListMakerProvider()
        { }

        /// <param name="QuietMode">Whether errors should be supressed</param>
        public CategoryListMakerProvider(bool QuietMode)
        {
            this.quietMode = QuietMode;
        }

        public virtual List<Article> MakeList(string[] searchCriteria)
        {
            List<string> searchCriteriaList = new List<string>(Tools.FirstToUpperAndRemoveHashOnArray(searchCriteria));

            List<Article> list = new List<Article>();
            List<string> badcategories = new List<string>();
            vistedCategories = new List<string>();

            for (int i = 0; i < searchCriteriaList.Count; i++)
            {
                if (!vistedCategories.Contains(searchCriteriaList[i]))
                {
                    vistedCategories.Add(searchCriteriaList[i]);
                    string cmtitle = Tools.WikiEncode(Regex.Replace(searchCriteriaList[i], Variables.NamespacesCaseInsensitive[14], ""));

                    string url = Variables.URLLong + "api.php?action=query&list=categorymembers&cmtitle=Category:" + cmtitle + "&cmcategory=" + cmtitle + "&format=xml&cmlimit=500";
                    int ns = 0;

                    while (true)
                    {
                        string title = "";
                        string html = Tools.GetHTML(url);
                        if (html.Contains("categorymembers /"))
                        {
                            badcategories.Add(searchCriteriaList[i]);
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

                                    if (subCategories && (ns == 14))
                                    {
                                        searchCriteriaList.Add(title.Replace(Variables.Namespaces[14], ""));
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

        public bool RunOnSeparateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets a list of pages in Named Categories for the ListMaker (Recursive - Will visit ALL subcategories)
    /// </summary>
    public class CategoryRecursiveListMakerProvider : CategoryListMakerProvider
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
    /// Gets a list of pages in Named Categories for the ListMaker (Recursive - Will visit 1 level of subcategories)
    /// </summary>
    public class CategoryRecursiveOneLevelListMakerProvider : CategoryRecursiveUserDefinedLevelListMakerProvider
    {
        public CategoryRecursiveOneLevelListMakerProvider()
            : base(1)
        { }

        public override List<Article> MakeList(string[] searchCriteria)
        {
            return base.MakeList(searchCriteria, true);
        }

        public override string DisplayText
        {
            get { return "Category (recursive 1 level)"; }
        }
    }

    /// <summary>
    /// Gets a list of pages in Named Categories for the ListMaker (Recursive - Will visit the specified number of levels of subcategories)
    /// </summary>
    public class CategoryRecursiveUserDefinedLevelListMakerProvider : CategoryListMakerProvider
    {
        private int level;
        protected List<string> allVistedCategories;

        /// <param name="Level">Levels of Subcategories to visit</param>
        protected CategoryRecursiveUserDefinedLevelListMakerProvider(int Level)
        {
            this.level = Level;
            this.subCategories = false;
            this.quietMode = true;
        }

        public CategoryRecursiveUserDefinedLevelListMakerProvider()
            : this(0)
        { }

        public override List<Article> MakeList(string[] searchCriteria)
        {
            return MakeList(searchCriteria, false);
        }

        /// <param name="levelSet">Whether the level has already been set by the code</param>
        protected List<Article> MakeList(string[] searchCriteria, bool levelSet)
        {
            if (!levelSet)
            {
                using (WikiFunctions.Controls.LevelNumber num = new WikiFunctions.Controls.LevelNumber())
                {
                    num.ShowDialog();
                    level = num.Levels;
                }
            }
            List<Article> articlesToReturn = new List<Article>();
            List<Article> articles = base.MakeList(searchCriteria);

            allVistedCategories = new List<string>(vistedCategories);

            for (int i = 0; i < level; i++)
            {
                articlesToReturn.AddRange(articles);

                List<string> moreCats = new List<string>();

                foreach (Article a in articles)
                {
                    if (a.NameSpaceKey == 14 && !allVistedCategories.Contains(a.ToString()))
                        moreCats.Add(a.ToString());
                }

                articles.Clear();

                if (moreCats.Count == 0)
                    break;

                articles.AddRange(base.MakeList(moreCats.ToArray()));
                allVistedCategories.AddRange(vistedCategories);
            }

            articlesToReturn.AddRange(articles);

            return articlesToReturn;
        }

        public List<Article> MakeList(string[] searchCriteria, int Level)
        {
            this.Level = Level;
            return MakeList(searchCriteria, true);
        }

        /// <summary>
        /// Get/Set Level
        /// </summary>
        public int Level
        {
            get { return this.level; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", "Value is less than 0");

                this.level = value;
            }
        }

        public override string DisplayText
        {
            get { return "Category (recursive user defined level)"; }
        }
    }

    /// <summary>
    /// Gets a list of pages from a text file
    /// </summary>
    public class TextFileListMakerProvider : IListProvider
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

        public List<Article> MakeList()
        {
            return MakeList(new string[0]);
        }

        public List<Article> MakeList(string[] searchCriteria)
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

        public bool RunOnSeparateThread
        { get { return false; } }
    }

    /// <summary>
    /// Gets a list of pages which link to the Named Pages
    /// </summary>
    public class WhatLinksHereListMakerProvider : IListProvider
    {
        protected bool embedded;
        protected bool includeRedirects;

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

        public bool RunOnSeparateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets a list of pages which link to the Named Pages (including what links to the redirects)
    /// </summary>
    public class WhatLinksHereIncludingRedirectsListMakerProvider : WhatLinksHereListMakerProvider
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
    public class WhatTranscludesPageListMakerProvider : WhatLinksHereListMakerProvider
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
    public class LinksOnPageListMakerProvider : IListProvider
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

        public bool RunOnSeparateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets a list of all Images on the Named Pages
    /// </summary>
    public class ImagesOnPageListMakerProvider : IListProvider
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

        public bool RunOnSeparateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets a list of all the transclusions on the Named Pages
    /// </summary>
    public class TransclusionsOnPageListMakerProvider : IListProvider
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

        public bool RunOnSeparateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets a list of google results based on the named pages
    /// </summary>
    public class GoogleSearchListMakerProvider : IListProvider
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

        public bool RunOnSeparateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets the user contribs of the Named Users
    /// </summary>
    public class UserContribsListMakerProvider : IListProvider
    {
        protected bool all;

        public virtual List<Article> MakeList(string[] searchCriteria)
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

        public virtual string DisplayText
        { get { return "User contribs"; } }

        public string UserInputTextBoxText
        { get { return Variables.Namespaces[2]; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeparateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets ALL the user contribs of the Named Users
    /// </summary>
    public class UserContribsAllListMakerProvider : UserContribsListMakerProvider
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
    /// Gets a list of pages which link to the Named Images
    /// </summary>
    public class ImageFileLinksListMakerProvider : IListProvider
    {
        public List<Article> MakeList(string[] searchCriteria)
        {
            searchCriteria = Tools.FirstToUpperAndRemoveHashOnArray(searchCriteria);

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

        public bool RunOnSeparateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets a list of pages which are returned from a wiki search of the Named Pages
    /// </summary>
    public class WikiSearchListMakerProvider : IListProvider
    {
        public List<Article> MakeList(string[] searchCriteria)
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
            string sroffset = "";

            foreach (string s in searchCriteria)
            {
                string search = Tools.WikiEncode(s);

                string url = Variables.URLLong + "api.php?action=query&list=search&srwhat=text&srsearch="
                    + s + "&srlimit=500&format=xml" + ns;
                string title = "";
                int nsBuilder = 0;

                while (true)
                {
                    string html = Tools.GetHTML(url+sroffset);
                    sroffset = "";

                    using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                    {
                        while (reader.Read())
                        {
                            if (reader.Name.Equals("p"))
                            {
                                if (reader.MoveToAttribute("ns"))
                                    nsBuilder = int.Parse(reader.Value);
                                else
                                    nsBuilder = 0;

                                if (reader.MoveToAttribute("title"))
                                {
                                    title = reader.Value.ToString();
                                    list.Add(new WikiFunctions.Article(title, nsBuilder));
                                }
                            }
                            else if (reader.Name.Equals("search") && string.IsNullOrEmpty(sroffset))
                            {
                                sroffset = reader.GetAttribute("sroffset");
                                if (!string.IsNullOrEmpty(sroffset))
                                {
                                    sroffset = "&sroffset=" + sroffset;
                                }
                                else 
                                {
                                    sroffset = "";
                                }
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(sroffset)) break;
                }
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

        public bool RunOnSeparateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets a list of pages which redirect to the Named Pages
    /// </summary>
    public class RedirectsListMakerProvider : IListProvider
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

        public bool RunOnSeparateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets all the pages from the Current Users Watchlist
    /// </summary>
    public class MyWatchlistListMakerProvider : IListProvider
    {
        public List<Article> MakeList(string[] searchCriteria)
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

        public bool RunOnSeparateThread
        { get { return true; } }
    }

    /// <summary>
    /// Runs the Database Scanner
    /// </summary>
    public class DatabaseScannerListMakerProvider : IListProvider
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

        public bool RunOnSeparateThread
        { get { return false; } }

        #endregion
    }

    /// <summary>
    /// Gets 100 random articles from the 0 namespace
    /// </summary>
    public class RandomPagesListMakerProvider : IListProvider
    {
        #region IListProvider Members

        public List<Article> MakeList(string[] searchCriteria)
        {
            List<Article> list = new List<Article>();

            string url = Variables.URLLong + "api.php?action=query&list=random&rnnamespace=0&rnlimit=10&format=xml";

            for (int i = 0; i < 10; i++)
            {
                string html = Tools.GetHTML(url);

                using (XmlTextReader reader = new XmlTextReader(new StringReader(html)))
                {
                    while (reader.Read())
                    {
                        if (reader.Name.Equals("page"))
                        {
                            if (reader.MoveToAttribute("title"))
                            {
                                list.Add(new WikiFunctions.Article(reader.Value.ToString(), 0));
                            }
                        }
                    }
                }
            }
            return list;
        }

        public string DisplayText
        {
            get { return "Random Pages"; }
        }

        public string UserInputTextBoxText
        {
            get { return ""; }
        }

        public bool UserInputTextBoxEnabled
        {
            get { return false; }
        }

        public void Selected() { }

        public bool RunOnSeparateThread
        {
            get { return true; }
        }

        #endregion
    }
    #endregion
}
