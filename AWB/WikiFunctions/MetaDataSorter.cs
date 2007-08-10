/*

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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Web;

namespace WikiFunctions.Parse
{
    public enum InterWikiOrderEnum : byte { LocalLanguageAlpha, LocalLanguageFirstWord, Alphabetical, AlphabeticalEnFirst }
    class MetaDataSorter
    {
        Parsers parser;
        public MetaDataSorter(Parsers p)
        {
            parser = p;

            LoadInterWiki();

            InterWikisList.Clear();
            foreach (string s in InterwikiLocalAlpha)
                //InterWikisList.Add(new Regex("\\[\\[" + s + ":.*?\\]\\]", RegexOptions.Compiled));
                InterWikisList.Add(new Regex("\\[\\[(?<site>" + s + "):(?<text>.*?)\\]\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase));
        }

        // now will be generated dynamically using Variables.Stub
        //Regex StubsRegex = new Regex("<!-- ?\\{\\{.*?stub\\}\\}.*?-->|:?\\{\\{.*?stub\\}\\}");
        Regex InterLangRegex = new Regex("<!-- ?(other languages|language links|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|inter ?wiki|The below are interlanguage links\\.?) ?-->", RegexOptions.IgnoreCase);
        Regex CatCommentRegex = new Regex("<!-- ?categories ?-->", RegexOptions.IgnoreCase);

        private string[] InterwikiLocalAlpha;
        private string[] InterwikiLocalFirst;
        private string[] InterwikiAlpha;
        private string[] InterwikiAlphaEnFirst; 
        List<Regex> InterWikisList = new List<Regex>();
        Regex IWSplit = new Regex(",", RegexOptions.Compiled);

        private InterWikiOrderEnum order = InterWikiOrderEnum.LocalLanguageAlpha;
        public InterWikiOrderEnum InterWikiOrder
        {//orders from http://meta.wikimedia.org/wiki/Interwiki_sorting_order
            set
            {
                if (order != value)
                {
                    order = value;
                    if (value == InterWikiOrderEnum.LocalLanguageAlpha)
                    {
                        InterWikisList.Clear();
                        foreach (string s in InterwikiLocalAlpha)
                            //InterWikisList.Add(new Regex("\\[\\[" + s + ":.*?\\]\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase));
                            InterWikisList.Add(new Regex("\\[\\[(?<site>" + s + "):(?<text>.*?)\\]\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase));
                    }
                    else if (value == InterWikiOrderEnum.LocalLanguageFirstWord)
                    {
                        InterWikisList.Clear();
                        foreach (string s in InterwikiLocalFirst)
                            //InterWikisList.Add(new Regex("\\[\\[" + s + ":.*?\\]\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase));
                            InterWikisList.Add(new Regex("\\[\\[(?<site>" + s + "):(?<text>.*?)\\]\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase));
                    }
                    else if (value == InterWikiOrderEnum.Alphabetical)
                    {
                        InterWikisList.Clear();
                        foreach (string s in InterwikiAlpha)
                            //InterWikisList.Add(new Regex("\\[\\[" + s + ":.*?\\]\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase));
                            InterWikisList.Add(new Regex("\\[\\[(?<site>" + s + "):(?<text>.*?)\\]\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase));
                    }
                    else if (value == InterWikiOrderEnum.AlphabeticalEnFirst)
                    {
                        InterWikisList.Clear();
                        foreach (string s in InterwikiAlphaEnFirst)
                            //InterWikisList.Add(new Regex("\\[\\[" + s + ":.*?\\]\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase));
                            InterWikisList.Add(new Regex("\\[\\[(?<site>" + s + "):(?<text>.*?)\\]\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase));
                    }
                }
            }
            get
            { return order; }
        }

        private void LoadInterWiki()
        {
            try
            {
                WikiFunctions.Browser.WebControl webBrowser = new WikiFunctions.Browser.WebControl();
                webBrowser.Navigate("http://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/IW&action=edit");
                webBrowser.Wait();
                string text = webBrowser.GetArticleText();

                string InterwikiLocalAlphaRaw = remExtra(Tools.StringBetween(text, "<!--InterwikiLocalAlphaBegins-->", "<!--InterwikiLocalAlphaEnds-->").Replace("<!--InterwikiLocalAlphaBegins-->", ""));
                string InterwikiLocalFirstRaw = remExtra(Tools.StringBetween(text, "<!--InterwikiLocalFirstBegins-->", "<!--InterwikiLocalFirstEnds-->").Replace("<!--InterwikiLocalFirstBegins--", ""));
                string InterwikiAlphaRaw = remExtra(Tools.StringBetween(text, "<!--InterwikiAlphaBegins-->", "<!--InterwikiAlphaEnds-->").Replace("<!--InterwikiAlphaBegins-->", ""));
                string InterwikiAlphaEnFirstRaw = remExtra(Tools.StringBetween(text, "<!--InterwikiAlphaEnFirstBegins-->", "<!--InterwikiAlphaEnFirstEnds-->").Replace("<!--InterwikiAlphaEnFirstBegins-->", ""));

                int no = 0;
                
                InterwikiLocalAlpha = new string[IWSplit.Matches(InterwikiLocalAlphaRaw).Count + 1];

                foreach (string s in InterwikiLocalAlphaRaw.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    InterwikiLocalAlpha[no] = s.Trim();
                    no++;
                }

                InterwikiLocalFirst = new string[IWSplit.Matches(InterwikiLocalFirstRaw).Count + 1];
                no = 0;

                foreach (string s in InterwikiLocalFirstRaw.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    InterwikiLocalFirst[no] = s.Trim();
                    no++;
                }

                InterwikiAlpha = new string[IWSplit.Matches(InterwikiAlphaRaw).Count + 1];
                no = 0;

                foreach (string s in InterwikiAlphaRaw.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    InterwikiAlpha[no] = s.Trim();
                    no++;
                }

                InterwikiAlphaEnFirst = new string[IWSplit.Matches(InterwikiAlphaEnFirstRaw).Count + 1];
                no = 0;

                foreach (string s in InterwikiAlphaEnFirstRaw.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    InterwikiAlphaEnFirst[no] = s.Trim();
                    no++;
                }
            }
            catch { }
        }

        private string remExtra(string Input)
        {
            return Input.Replace("\r\n", "").Replace(">", "");
        }

        private string Newline(string s)
        {
            return s == "" ? s : "\r\n" + s;
        }
                               
        internal string Sort(string ArticleText, string ArticleTitle)
        {
            ArticleText = Regex.Replace(ArticleText, "<!-- ?\\[\\[en:.*?\\]\\] ?-->", "");

            string strPersonData = Newline(removePersonData(ref ArticleText));
            string strDisambig = Newline(removeDisambig(ref ArticleText));
            string strCategories = Newline(removeCats(ref ArticleText, ArticleTitle));
            string strInterwikis = Newline(interwikis(ref ArticleText));
            string strStub = Newline(removeStubs(ref ArticleText));

            //filter out excess white space and remove "----" from end of article
            ArticleText = Parsers.RemoveWhiteSpace(ArticleText) + "\r\n";
            ArticleText += strDisambig;

            switch (Variables.LangCode)
            {
                case LangCodeEnum.pl:
                    ArticleText += strPersonData + strStub + strCategories;
                    break;
                case LangCodeEnum.ru:
                    ArticleText += strPersonData + strStub + strCategories;
                    break;
                case LangCodeEnum.simple:
                    ArticleText += strPersonData + strStub + strCategories;
                    break;
                case LangCodeEnum.de:
                    ArticleText += strStub + strCategories + strPersonData;
                    break;
                default:
                    ArticleText += strPersonData + strCategories + strStub;
                    break;
            }
            return ArticleText + strInterwikis;
        }

        private string removeCats(ref string ArticleText, string ArticleTitle)
        {
            List<string> CategoryList = new List<string>();
            string x = "";

            foreach (Match m in Regex.Matches(ArticleText, "<!-- ? ?\\[\\[" + Variables.NamespacesCaseInsensitive[14] + ".*?(\\]\\]|\\|.*?\\]\\]).*?-->|\\[\\[" + Variables.NamespacesCaseInsensitive[14] + ".*?(\\]\\]|\\|.*?\\]\\])( {0,4}⌊⌊⌊⌊[0-9]{1,4}⌋⌋⌋⌋)?"))
            {
                x = m.Value;
                //add to array, replace underscores with spaces, ignore=
                if (!Regex.IsMatch(x, "\\[\\[Category:(Pages|Categories|Articles) for deletion\\]\\]"))
                {
                    ArticleText = ArticleText.Replace(x, "");
                    CategoryList.Add(x.Replace("_", " "));
                }
            }

            if (parser.addCatKey)
                CategoryList = catKeyer(CategoryList, ArticleTitle);

            if (CatCommentRegex.IsMatch(ArticleText))
            {
                string catComment = CatCommentRegex.Match(ArticleText).Value;
                ArticleText = ArticleText.Replace(catComment, "");
                CategoryList.Insert(0, catComment);
            }

            string defaultSort = WikiRegexes.Defaultsort.Match(ArticleText).Value;
            if (defaultSort != "")
                ArticleText = ArticleText.Replace(defaultSort, "");
            defaultSort = WikiRegexes.Defaultsort.Replace(defaultSort, "{{DEFAULTSORT:${key}}}");
            if (defaultSort != "") defaultSort += "\r\n";

            return defaultSort + ListToString(CategoryList);
        }

        private string removePersonData(ref string ArticleText)
        {
            string strPersonData = Parsers.GetTemplate(ArticleText, "[Pp]ersondata");

            if (strPersonData != "")
                ArticleText = ArticleText.Replace(strPersonData, "");

            return strPersonData;
        }

        private string removeStubs(ref string ArticleText)
        {
            Regex StubsRegex = new Regex("<!-- ?\\{\\{.*?" + Variables.Stub + "b\\}\\}.*?-->|:?\\{\\{.*?" + Variables.Stub + "\\}\\}");

            List<string> StubList = new List<string>();
            MatchCollection n = StubsRegex.Matches(ArticleText);
            string x = "";

            foreach (Match m in n)
            {
                x = m.Value;
                if (!((Regex.IsMatch(x, Variables.SectStub) || (Regex.IsMatch(x, "tl\\|")))))
                {
                    StubList.Add(x);
                    //remove old stub
                    ArticleText = ArticleText.Replace(x, "");
                }
            }

            if (StubList.Count != 0)
                return ListToString(StubList);
            else
                return "";
        }

        private string removeDisambig(ref string ArticleText)
        {
            if (Variables.LangCode != LangCodeEnum.en)
                return "";

            string strDisambig = "";
            if (WikiRegexes.Disambigs.IsMatch(ArticleText))
            {
                strDisambig = WikiRegexes.Disambigs.Match(ArticleText).Value;
                ArticleText = ArticleText.Replace(strDisambig, "");
            }

            return strDisambig;
        }

        private List<string> removeLinkFAs(ref string ArticleText)
        {
            List<string> LinkFAList = new List<string>();
            string x = "";
            foreach (Match m in WikiRegexes.LinkFAs.Matches(ArticleText))
            {
                x = m.Value;
                LinkFAList.Add(x);
                //remove old LinkFA
                ArticleText = ArticleText.Replace(x, "");
            }

            return LinkFAList;
        }

        private string interwikis(ref string ArticleText)
        {
            string interwikis = ListToString(removeLinkFAs(ref ArticleText)) + ListToString(removeInterWikis(ref ArticleText));
            return interwikis;
        }

        private List<string> removeInterWikis(ref string ArticleText)
        {
            List<string> InterWikiList = new List<string>();
            //Regex interwikiregex = new Regex(@"\[\[(?<site>.*?):(?<text>.*?)\]\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            for (int i = 0; i != (InterWikisList.Count -1); i++)
            {
                //ArticleText = interwikiregex.Replace(ArticleText, new MatchEvaluator(MetaDataSorter.IWMatchEval));
                ArticleText = InterWikisList[i].Replace(ArticleText, new MatchEvaluator(MetaDataSorter.IWMatchEval));
            }

            if (InterLangRegex.IsMatch(ArticleText))
            {
                string interWikiComment = "";

                interWikiComment = InterLangRegex.Match(ArticleText).Value;
                ArticleText = ArticleText.Replace(interWikiComment, "");
                InterWikiList.Add(interWikiComment);
            }
           
            if (parser.sortInterwikiOrder)
            {
                string x;
                foreach (Regex rege in InterWikisList)
                {
                    //use foreach as some articles have multiple links to same wiki
                    x = "";
                    foreach (Match m in rege.Matches(ArticleText))
                    {
                        x = m.Value;
                        ArticleText = rege.Replace(ArticleText, "", 1);
                        x = HttpUtility.HtmlDecode(x).Replace("_", " ");
                        InterWikiList.Add(x);
                    }
                }
            }
            else
            {
                string x;
                //keeps existing order
                if (WikiRegexes.InterWikiLinks.IsMatch(ArticleText))
                {
                    foreach (Match m in WikiRegexes.InterWikiLinks.Matches(ArticleText))
                    {
                        x = m.Value;
                        ArticleText = ArticleText.Replace(x, "");
                        x = HttpUtility.HtmlDecode(x).Replace("_", " ");
                        InterWikiList.Add(x);
                    }
                }
            }

            return InterWikiList;
        }

        public static string IWMatchEval(Match match)
        {
            string[] textArray = new string[] { "[[", match.Groups["site"].ToString().ToLower(), ":", match.Groups["text"].ToString(), "]]" };
            return string.Concat(textArray);
        }

        private string ListToString(List<string> items)
        {//remove duplicates, and return List as string.

            if (items.Count == 0)
                return "";

            string List = "";
            List<string> uniqueItems = new List<string>();

            //remove duplicates
            foreach (string s in items)
            {
                if (!uniqueItems.Contains(s))
                    uniqueItems.Add(s);
            }

            //add to string
            foreach (string s in uniqueItems)
            {
                List += s + "\r\n";
            }

            return List;
        }

        private List<string> catKeyer(List<string> List, string strName)
        {
            // make key
            strName = Tools.MakeHumanCatKey(strName);

            //add key to cats that need it
            List<string> newCats = new List<string>();
            foreach (string s in List)
            {
                string z = s;
                if (!z.Contains("|"))
                    z = z.Replace("]]", "|" + strName + "]]");

                newCats.Add(z);
            }
            return newCats;
        }
    }
}
