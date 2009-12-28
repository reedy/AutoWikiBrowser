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
using System.Collections.Generic;
using System.IO;
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
        public List<Article> MakeList(params string[] searchCriteria)
        {
            List<Article> list = new List<Article>();

            foreach (string url in searchCriteria)
            {
                foreach (
                    string entry in
                        Tools.StringBetween(Tools.GetHTML(url.Contains("http") ? url : "http://" + url), "<body>",
                                            "</body>").Split(new[] {"\r\n", "\n"},
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
            return !entry.StartsWith(@"<h1>");
        }

        protected virtual string ModifyArticleName(string title)
        {
            title = title.Replace(@"&amp;", "&");
            title = title.Replace(@"&quot;", @"""");
            return title.Replace("<br />", "");
        }

        public virtual string DisplayText
        { get { return "HTML Scraper"; } }

        public string UserInputTextBoxText
        { get { return "URL:"; } }

        public bool UserInputTextBoxEnabled
        { get { return true; } }

        public void Selected()
        { }

        public bool RunOnSeparateThread
        { get { return true; } }
    }

    /// <summary>
    /// Gets a list of pages from an online CheckWiki-output type page
    /// </summary>
    public class CheckWikiListProvider : HTMLPageScraperListProvider
    {
        protected override bool CheckExtra(string entry)
        {
            return !entry.Contains("Error number");
        }

        protected override string ModifyArticleName(string title)
        {
            return title.Replace("<br />", "");
        }

        public override string DisplayText
        { get { return "CheckWiki error"; } }
    }

    /// <summary>
    /// Gets a list of google results based on the named pages
    /// </summary>
    public class GoogleSearchListProvider : IListProvider
    {
        private static readonly Regex RegexGoogle = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+) class=l)",
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
                    string url = "http://www.google.com/search?q=" + google + "+site:" + Variables.URL +
                                 "&num=100&hl=en&lr=&start=" + intStart + "&sa=N&filter=0";

                    string googleText = Tools.GetHTML(url, Encoding.Default);

                    //Find each match to the pattern
                    foreach (Match m in RegexGoogle.Matches(googleText))
                    {
                        string title = Tools.GetTitleFromURL(m.Groups[1].Value);

                        if (!string.IsNullOrEmpty(title))
                            list.Add(new Article(title));
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
        #endregion
    }

    /// <summary>
    /// Gets a list of pages from a text file
    /// </summary>
    public class TextFileListProvider : IListProvider
    {
        private readonly static Regex RegexFromFile = new Regex("(^[a-z]{2,3}:)|(simple:)", RegexOptions.Compiled);
        private readonly static Regex LoadWikiLink = new Regex(@"\[\[:?(.*?)(?:\]\]|\|)", RegexOptions.Compiled);
        private readonly OpenFileDialog OpenListDialog = new OpenFileDialog();

        public TextFileListProvider()
        {
            OpenListDialog.Filter = "Text files|*.txt|Text files (no validation)|*.txt|All files|*.*";
            OpenListDialog.Multiselect = true;
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
                    string pageText, title;

                    using (StreamReader sr = new StreamReader(fileName, Encoding.UTF8))
                    {
                        pageText = sr.ReadToEnd();
                        sr.Close();
                    }

                    switch (OpenListDialog.FilterIndex)
                    {
                        case 2:
                            foreach (string s in pageText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                list.Add(new Article(Tools.RemoveSyntax(Tools.TurnFirstToUpper(s.Trim()))));
                            }
                            break;
                        default:
                            if (LoadWikiLink.IsMatch(pageText))
                            {
                                foreach (Match m in LoadWikiLink.Matches(pageText))
                                {
                                    title = m.Groups[1].Value;
                                    if (!RegexFromFile.IsMatch(title) && (!(title.StartsWith("#"))))
                                    {
                                        list.Add(new Article(Tools.RemoveSyntax(Tools.TurnFirstToUpper(title))));
                                    }
                                }
                            }
                            else
                            {
                                foreach (string s in pageText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    if (s.Trim().Length == 0 || !Tools.IsValidTitle(s)) continue;
                                    list.Add(new Article(Tools.RemoveSyntax(Tools.TurnFirstToUpper(s.Trim()))));
                                }
                            }
                            break;
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
        { get { return "Text file"; } }

        public string UserInputTextBoxText
        { get { return ""; } }

        public bool UserInputTextBoxEnabled
        { get { return false; } }

        public void Selected() { }

        public bool RunOnSeparateThread
        { get { return false; } }
        #endregion
    }
}