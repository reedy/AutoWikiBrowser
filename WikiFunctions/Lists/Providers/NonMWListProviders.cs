﻿/*
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;

namespace WikiFunctions.Lists.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public class HTMLPageScraperListProvider : IListProvider
    {
        public virtual List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> list = new List<Article>();

            foreach (string url in searchCriteria)
            {
                string urlBuilt = url.Contains("http") ? url : "http://" + url;

                if (!WikiRegexes.UrlValidator.IsMatch(urlBuilt))
                    throw new ArgumentException("Url \"" + urlBuilt + "\" is not valid", "searchCriteria");

                foreach (
                    string entry in
                        Tools.StringBetween(Tools.GetHTML(urlBuilt), "<body>",
                                            "</body>").Split(new[] { "\r\n", "\n" },
                                                             StringSplitOptions.RemoveEmptyEntries))
                {
                    if (entry.Length > 0 && CheckExtra(entry))
                    {
                        list.Add(new Article(ModifyArticleName(entry)));
                    }
                }
            }

            return list;
        }

        protected virtual bool CheckExtra(string entry)
        {
            return (!entry.StartsWith(@"<h1>", StringComparison.OrdinalIgnoreCase));
        }

        protected virtual string ModifyArticleName(string title)
        {
            Parse.Parsers p = new Parse.Parsers();
            title = p.Unicodify(title);

            title = title.Replace(@"&amp;", "&");
            title = title.Replace(@"&quot;", @"""");
            return title.Replace("<br />", "");
        }

        public virtual string DisplayText
        { get { return "HTML Scraper"; } }

        public virtual string UserInputTextBoxText
        { get { return "URL:"; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected()
        { }

        public bool RunOnSeparateThread
        { get { return true; } }

        public virtual bool StripUrl
        { get { return false; } }
    }

    /// <summary>
    /// Gets a list of pages from an online CheckWiki-output type page
    /// </summary>
    public class CheckWikiListProvider : HTMLPageScraperListProvider
    {
        protected override bool CheckExtra(string entry)
        {
            return !(entry.StartsWith("<pre>", StringComparison.OrdinalIgnoreCase) || entry.EndsWith("</pre>", StringComparison.OrdinalIgnoreCase));
        }

        private static readonly Regex Apostrophe = new Regex(@"&#0?39;|&#146;|&amp;#0?39;|&amp;#146;|[`’]", RegexOptions.Compiled);
        protected override string ModifyArticleName(string title)
        {
            title = Apostrophe.Replace(title, "'");

            title = title.Replace(@"&amp;", "&");
            title = title.Replace(@"&quot;", @"""");
            return title.Replace("<br />", "");
        }

        public override string DisplayText
        { get { return "CheckWiki error"; } }
    }

    /// <summary>
    /// Gets a list of pages from an online CheckWiki-output type page -input error number
    /// </summary>
    public class CheckWikiWithNumberListProvider : CheckWikiListProvider
    {
        public override List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> list = new List<Article>();

            foreach (string errornumber in searchCriteria)
            {
                string title = "https://checkwiki.toolforge.org/cgi-bin/checkwiki.cgi?project=" + Variables.LangCode +
                               "wiki&view=bots&id=" + errornumber + "&offset=0";
                list.AddRange(base.MakeList(title));
            }
            return list;
        }

        public override string UserInputTextBoxText
        { get { return "Error number:"; } }

        public override string DisplayText
        { get { return "CheckWiki error (number)"; } }
    }

    /// <summary>
    /// Gets a list of google results based on the named pages
    /// </summary>
    public class GoogleSearchListProvider : IListProvider
    {
        private static readonly Regex RegexGoogle = new Regex(@"href\s*=\s*(?:""(?:/url\?q=)?(?<title>[^""]*)""|(?<title>\S+) class=l)",
                                                              RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> list = new List<Article>();

            foreach (string g in searchCriteria)
            {
                int intStart = 0;
                string google = HttpUtility.UrlEncode(g);

                do
                {
                    string url = string.Format("https://www.google.com/search?q={0}+site:{1}&num=100&hl=en&lr=&start={2}&sa=N&filter=0", google, Variables.URL, intStart);

                    string googleText = Tools.GetHTML(url, Encoding.Default);

                    //Find each match to the pattern
                    foreach (Match m in RegexGoogle.Matches(googleText))
                    {
                        string searchres = m.Groups["title"].Value;
                        
                        if (searchres.Contains(@"&amp;"))
                            searchres = searchres.Substring(0, searchres.IndexOf(@"&amp;", StringComparison.Ordinal));
                        
                        string title = Tools.GetTitleFromURL(searchres);

                        // some google results are double encoded, so WikiDecode again
                        if (!string.IsNullOrEmpty(title))
                            list.Add(new Article(Regex.Replace(Tools.WikiDecode(title), @"\?\w+=.*", "")));
                    }

                    if (!googleText.Contains("img src=\"nav_next.gif\""))
                        break;

                    intStart += 100;

                } while (true);
            }
            return Tools.FilterSomeArticles(list);
        }

        #region ListMaker properties
        public string DisplayText
        { get { return "Google search"; } }

        public string UserInputTextBoxText
        { get { return "Google search:"; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected() { }

        public bool RunOnSeparateThread
        { get { return true; } }

        public virtual bool StripUrl
        { get { return false; } }
        #endregion
    }

    /// <summary>
    /// Gets a list of pages from a UTF-8 encoded text file
    /// </summary>
    public class TextFileListProviderUFT8 : IListProvider
    {
        private static readonly Regex RegexFromFile = new Regex("(^[a-z]{2,3}:)|(simple:)", RegexOptions.Compiled);
        private static readonly Regex LoadWikiLink = new Regex(@"\[\[:?([^\|[\]]+)(?:\]\]|\|)", RegexOptions.Compiled);
        private static readonly OpenFileDialog OpenListDialog = new OpenFileDialog();

        protected Encoding TargetEncoding;

        static TextFileListProviderUFT8()
        {
            OpenListDialog.Filter = "Text files|*.txt|Text files (no validation)|*.txt|All files|*.*";
            OpenListDialog.Multiselect = true;
        }

        public TextFileListProviderUFT8()
        {
            TargetEncoding = Encoding.UTF8;
        }

        public List<Article> MakeList(string searchCriteria)
        {
            return MakeList(searchCriteria.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
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
                if (searchCriteria.Length == 0 && OpenListDialog.ShowDialog() == DialogResult.OK)
                    searchCriteria = OpenListDialog.FileNames;

                foreach (string fileName in searchCriteria)
                {
                    string pageText = File.ReadAllText(fileName, TargetEncoding);

                    switch (OpenListDialog.FilterIndex)
                    {
                        case 2:
                            list.AddRange(pageText.Split(new[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries).Select(s => new Article(Tools.RemoveSyntax(Tools.TurnFirstToUpper(s.Trim())))));
                            break;
                        default:
                            if (LoadWikiLink.IsMatch(pageText))
                            {
                                list.AddRange(from Match m in LoadWikiLink.Matches(pageText) select m.Groups[1].Value into title where !RegexFromFile.IsMatch(title) && !title.StartsWith("#") select new Article(Tools.RemoveSyntax(Tools.TurnFirstToUpper(title))));
                            }
                            else
                            {
                                list.AddRange(from s in pageText.Split(new[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries) where s.Trim().Length != 0 && Tools.IsValidTitle(s) select new Article(Tools.RemoveSyntax(Tools.TurnFirstToUpper(s.Trim()))));
                            }
                            break;
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex);
                return list;
            }
        }

        #region ListMaker properties
        public virtual string DisplayText
        { get { return "Text file (UTF-8)"; } }

        public string UserInputTextBoxText
        { get { return ""; } }

        public bool UserInputTextBoxEnabled
        { get { return false; } }

        public void Selected() { }

        public bool RunOnSeparateThread
        { get { return false; } }

        public virtual bool StripUrl
        { get { return false; } }
        #endregion
    }

    /// <summary>
    /// Gets a list of pages from a Windows 1252 (ANSI) encoded text file
    /// </summary>
    public class TextFileListProviderWindows1252 : TextFileListProviderUFT8
    {
        public TextFileListProviderWindows1252()
        {
            TargetEncoding = Encoding.GetEncoding("windows-1252");
        }

        #region ListMaker properties
        public override string DisplayText
        { get { return "Text file (Windows 1252 / ANSI)"; } }
        #endregion
    }
}
