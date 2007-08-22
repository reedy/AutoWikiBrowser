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
    internal sealed class InterWikiComparer : IComparer<string>
    {
        Dictionary<string, int> Order = new Dictionary<string, int>();
        public InterWikiComparer(string[] order)
        {
            int n = 0;
            foreach (string s in order)
            {
                Order.Add("[[" + s, n);
                n++;
            }
        }

        string RawCode(string iw)
        {
            //int i = iw.IndexOf(':');
            //string s = iw.Substring(0, i);
            return iw.Substring(0, iw.IndexOf(':'));
            //return s;
        }

        public int Compare(string x, string y)
        {
            int ix = Order[RawCode(x)], iy = Order[RawCode(y)];

            if (ix < iy) return -1;
            else if (ix == iy) return 0;
            else return 1;
        }
    }

    public enum InterWikiOrderEnum : byte { LocalLanguageAlpha, LocalLanguageFirstWord, Alphabetical, AlphabeticalEnFirst }
    class MetaDataSorter
    {
        Parsers parser;
        public MetaDataSorter(Parsers p)
        {
            parser = p;

            LoadInterWiki();

            //InterWikisList.Clear();
            //foreach (string s in InterwikiLocalAlpha)
            //    //InterWikisList.Add(new Regex("\\[\\[" + s + ":.*?\\]\\]", RegexOptions.Compiled));
            //    InterWikisList.Add(new Regex("\\[\\[(?<site>" + s + "):(?<text>.*?)\\]\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase));

            string s = string.Join("|", InterwikiLocalAlpha);
            s = @"\[\[\s*(" + s + @")\s*:\s*([^\]]*)\s*\]\]";
            FastIW = new Regex(s, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            //create a comparer
            InterWikiOrder = InterWikiOrderEnum.LocalLanguageAlpha;
        }

        // now will be generated dynamically using Variables.Stub
        //Regex StubsRegex = new Regex("<!-- ?\\{\\{.*?stub\\}\\}.*?-->|:?\\{\\{.*?stub\\}\\}");
        Regex InterLangRegex = new Regex("<!-- ?(other languages|language links|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|inter ?wiki|The below are interlanguage links\\.?) ?-->", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex CatCommentRegex = new Regex("<!-- ?categories ?-->", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private string[] InterwikiLocalAlpha;
        private string[] InterwikiLocalFirst;
        private string[] InterwikiAlpha;
        private string[] InterwikiAlphaEnFirst; 
        //List<Regex> InterWikisList = new List<Regex>();
        Regex IWSplit = new Regex(",", RegexOptions.Compiled);

        Regex FastIW;


        private InterWikiComparer Comparer;
        private InterWikiOrderEnum order = InterWikiOrderEnum.LocalLanguageAlpha;
        public InterWikiOrderEnum InterWikiOrder
        {//orders from http://meta.wikimedia.org/wiki/Interwiki_sorting_order
            set
            {
                order = value;

                string[] seq;
                switch (order)
                {
                    case InterWikiOrderEnum.Alphabetical:
                        seq = InterwikiAlpha;
                        break;
                    case InterWikiOrderEnum.AlphabeticalEnFirst:
                        seq = InterwikiAlphaEnFirst;
                        break;
                    case InterWikiOrderEnum.LocalLanguageAlpha:
                        seq = InterwikiLocalAlpha;
                        break;
                    case InterWikiOrderEnum.LocalLanguageFirstWord:
                        seq = InterwikiLocalFirst;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("MetaDataSorter.InterWikiOrder",
                            (System.Exception)null);
                }
                Comparer = new InterWikiComparer(seq);
            }
            get
            {
                return order;
            }
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

                int no = 0;
                int size = IWSplit.Matches(InterwikiLocalFirstRaw).Count + 1;
                
                InterwikiLocalAlpha = new string[IWSplit.Matches(InterwikiLocalAlphaRaw).Count + 1];

                foreach (string s in InterwikiLocalAlphaRaw.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    InterwikiLocalAlpha[no] = s.Trim().ToLower();
                    no++;
                }

                InterwikiLocalFirst = new string[size];
                no = 0;

                foreach (string s in InterwikiLocalFirstRaw.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    InterwikiLocalFirst[no] = s.Trim().ToLower();
                    no++;
                }

                InterwikiAlpha = InterwikiLocalFirst;
                Array.Sort(InterwikiAlpha);

                string[] Temp = InterwikiAlpha;
                Temp[Array.IndexOf(Temp, "en")] = "";

                InterwikiAlphaEnFirst = new string[size + 1];
                InterwikiAlphaEnFirst[0] = "en";
                no = 1;

                foreach (string s in Temp)
                {
                    if (s.Trim() != "")
                        InterwikiAlphaEnFirst[no] = s;
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
                case LangCodeEnum.de:
                    ArticleText += strStub + strCategories + strPersonData;
                    break;
                case LangCodeEnum.pl:
                    ArticleText += strPersonData + strStub + strCategories;
                    break;
                case LangCodeEnum.ru:
                    ArticleText += strPersonData + strStub + strCategories;
                    break;
                case LangCodeEnum.simple:
                    ArticleText += strPersonData + strStub + strCategories;
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

            Regex r = new Regex("<!-- ? ?\\[\\[" + Variables.NamespacesCaseInsensitive[14] + ".*?(\\]\\]|\\|.*?\\]\\]).*?-->|\\[\\[" + Variables.NamespacesCaseInsensitive[14] + ".*?(\\]\\]|\\|.*?\\]\\])( {0,4}⌊⌊⌊⌊[0-9]{1,4}⌋⌋⌋⌋)?");

            foreach (Match m in r.Matches(ArticleText))
            {
                x = m.Value;
                //add to array, replace underscores with spaces, ignore=
                if (!Regex.IsMatch(x, "\\[\\[Category:(Pages|Categories|Articles) for deletion\\]\\]"))
                {
                    CategoryList.Add(x.Replace("_", " "));
                }
            }

            ArticleText = r.Replace(ArticleText, "");

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

            foreach(Match m in FastIW.Matches(ArticleText))
            {
                InterWikiList.Add("[[" + m.Groups[1].Value.ToLower() + ":" + m.Groups[2].Value + "]]");
            }

            if (InterLangRegex.IsMatch(ArticleText))
            {
                string interWikiComment = "";

                interWikiComment = InterLangRegex.Match(ArticleText).Value;
                ArticleText = ArticleText.Replace(interWikiComment, "");
                InterWikiList.Add(interWikiComment);
            }

            ArticleText = FastIW.Replace(ArticleText, "");

            if (parser.sortInterwikiOrder)
            {
                InterWikiList.Sort(Comparer);
            }
            else
            {
                //keeps existing order
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
