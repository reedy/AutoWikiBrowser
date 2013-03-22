/*

Copyright (C) 2007 Martin Richards, Max Semenik et al.

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
using System.Xml;
using WikiFunctions.TalkPages;

namespace WikiFunctions.Parse
{
    public static class SiteMatrix
    {
        public static List<string> Languages = new List<string>();
        public static List<string> WikipediaLanguages = new List<string>();
        public static List<string> WiktionaryLanguages = new List<string>();
        public static List<string> WikibooksLanguages = new List<string>();
        public static List<string> WikinewsLanguages = new List<string>();
        public static List<string> WikisourceLanguages = new List<string>();
        public static List<string> WikiquoteLanguages = new List<string>();
        public static List<string> WikiversityLanguages = new List<string>();
        public static Dictionary<string, string> LanguageNames = new Dictionary<string, string>();

        static SiteMatrix()
        {          
            if (Globals.UnitTestMode) // or unit tests gonna run like a turtle
            {
                Languages.AddRange(new [] { "en", "ru", "sq" });
                WikipediaLanguages.AddRange(Languages);
                WiktionaryLanguages.AddRange(Languages);
                WikibooksLanguages.AddRange(Languages);
                WikinewsLanguages.AddRange(Languages);
                WikisourceLanguages.AddRange(Languages);
                WikiquoteLanguages.AddRange(Languages);
                WikiversityLanguages.AddRange(Languages);
            }
            else
            {
//#if DEBUG
//                StringBuilder builder = new StringBuilder();
//#endif

                string strMatrix = Tools.GetHTML("http://en.wikipedia.org/w/api.php?action=sitematrix&format=xml");

                XmlDocument matrix = new XmlDocument();
                matrix.LoadXml(strMatrix);

                foreach (XmlNode lang in matrix.GetElementsByTagName("language"))
                {
                    string langCode = lang.Attributes["code"].Value;
                    string langName = lang.Attributes["name"].Value;

//#if DEBUG
//                    try
//                    {
//                        Variables.ParseLanguage(langCode);
//                    }
//                    catch (ArgumentException)
//                    {
//                        if (!langCode.Contains("-old") && !langCode.Contains("closed") && langCode != "nomcom") //closed/old aren't used. nomcom isnt general access
//                            builder.AppendLine(langCode);
//                    }
//#endif

                    Languages.Add(langCode);
                    LanguageNames[langCode] = langName;

                    foreach (XmlNode site in lang.ChildNodes[0].ChildNodes)
                    {
                        if (site.Name != "site") continue;

                        switch (site.Attributes["code"].Value)
                        {
                            case "wiki":
                                WikipediaLanguages.Add(langCode);
                                break;
                            case "wiktionary":
                                WiktionaryLanguages.Add(langCode);
                                break;
                            case "wikibooks":
                                WikibooksLanguages.Add(langCode);
                                break;
                            case "wikinews":
                                WikinewsLanguages.Add(langCode);
                                break;
                            case "wikisource":
                                WikisourceLanguages.Add(langCode);
                                break;
                            case "wikiquote":
                                WikiquoteLanguages.Add(langCode);
                                break;
                            case "wikiversity":
                                WikiversityLanguages.Add(langCode);
                                break;
                        }
                    }
                }

                //should already be sorted, but we must be sure
                Languages.Sort();
                WikipediaLanguages.Sort();
                WiktionaryLanguages.Sort();
                WikibooksLanguages.Sort();
                WikinewsLanguages.Sort();
                WikisourceLanguages.Sort();
                WikiquoteLanguages.Sort();
                WikiversityLanguages.Sort();

//#if DEBUG
//                if (builder.Length > 0)
//                    Tools.WriteDebug("SiteMatrix - Missing Languages", builder.ToString());
//#endif
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public static List<string> GetProjectLanguages(ProjectEnum project)
        {
            switch (project)
            {
                case ProjectEnum.wikipedia:
                case ProjectEnum.meta:
                case ProjectEnum.commons:
                case ProjectEnum.species:
                    return WikipediaLanguages;

                case ProjectEnum.wiktionary:
                    return WiktionaryLanguages;

                case ProjectEnum.wikisource:
                    return WikisourceLanguages;

                case ProjectEnum.wikibooks:
                    return WikibooksLanguages;

                case ProjectEnum.wikinews:
                    return WikinewsLanguages;

                case ProjectEnum.wikiquote:
                    return WikiquoteLanguages;

                case ProjectEnum.wikiversity:
                    return WikiversityLanguages;

                default:
                    return new List<string>();
            }
        }
    }
    
    internal sealed class InterWikiComparer : IComparer<string>
    {
        readonly Dictionary<string, int> Order = new Dictionary<string, int>();
        public InterWikiComparer(List<string> order, List<string> languages)
        {
            languages = new List<string>(languages); // make a copy
            List<string> unordered = new List<string>(),
                output = new List<string>();

            // remove unneeded languages from order
            for (int i = 0; i < order.Count; )
            {
                if (languages.Contains(order[i])) i++;
                else order.RemoveAt(i);
            }

            foreach(string s in languages)
                if (!order.Contains(s)) unordered.Add(s);

            if(unordered.Count == 0) output = order;
            else for (int i = 0; i < languages.Count; i++)
            {
                if (unordered.Contains(languages[i]))
                {
                    output.Add(languages[i]);
                    unordered.RemoveAt(0);
                }
                else
                {
                    output.Add(order[0]);
                    order.RemoveAt(0);
                }
            }

            int n = 0;
            foreach (string s in output)
            {
                Order.Add("[[" + s, n);
                n++;
            }
        }

        static string RawCode(string iw)
        {
            return iw.Substring(0, iw.IndexOf(':'));
        }

        public int Compare(string x, string y)
        {
            //should NOT be enclosed into try ... catch - I'd like to see exceptions if something goes wrong,
            //not quiet missorting --MaxSem
            int ix = Order[RawCode(x)], iy = Order[RawCode(y)];

            if (ix < iy) return -1;
            if (ix == iy) return 0;
            return 1;
        }
    }

    public enum InterWikiOrderEnum { LocalLanguageAlpha, LocalLanguageFirstWord, Alphabetical, AlphabeticalEnFirst }
    
    public class MetaDataSorter
    {
        readonly Parsers parser;

        public List<string> PossibleInterwikis;
        public MetaDataSorter(Parsers p)
        {
            parser = p;

            LoadInterWiki();

            if (InterwikiLocalAlpha == null)
                throw new NullReferenceException("InterwikiLocalAlpha is null");

            //string s = string.Join("|", SiteMatrix.WikipediaLanguages.ToArray());
            //s = @"\[\[\s?(" + s + @")\s?:\s?([^\]]*)\s?\]\]";

            //create a comparer
            InterWikiOrder = InterWikiOrderEnum.LocalLanguageAlpha;
        }

        // now will be generated dynamically using Variables.Stub
        //Regex StubsRegex = new Regex("<!-- ?\\{\\{.*?stub\\}\\}.*?-->|:?\\{\\{.*?stub\\}\\}");
        readonly Regex InterLangRegex = new Regex("<!-- ?(other languages?|language links?|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|inter ?wikis?|The below are interlanguage links\\.?) ?-->", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex CatCommentRegex = new Regex("<!-- ?cat(egories)? ?-->", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private string[] InterwikiLocalAlpha;
        private string[] InterwikiLocalFirst;
        private string[] InterwikiAlpha;
        private string[] InterwikiAlphaEnFirst; 
        //List<Regex> InterWikisList = new List<Regex>();
        readonly Regex IWSplit = new Regex(",", RegexOptions.Compiled);

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
                            (Exception)null);
                }
                PossibleInterwikis = SiteMatrix.GetProjectLanguages(Variables.Project);
                Comparer = new InterWikiComparer(new List<string>(seq), PossibleInterwikis);
            }
            get
            {
                return order;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadInterWiki()
        {
            string text = !Globals.UnitTestMode
                       ? Tools.GetHTML("http://en.wikipedia.org/w/index.php?title=Wikipedia:AutoWikiBrowser/IW&action=raw")
                       : @"<!--InterwikiLocalAlphaBegins-->
ru, sq, en
<!--InterwikiLocalAlphaEnds-->
<!--InterwikiLocalFirstBegins-->
en, sq, ru
<!--InterwikiLocalFirstEnds-->";

            string interwikiLocalAlphaRaw =
                remExtra(Tools.StringBetween(text, "<!--InterwikiLocalAlphaBegins-->", "<!--InterwikiLocalAlphaEnds-->"));
            string interwikiLocalFirstRaw =
                remExtra(Tools.StringBetween(text, "<!--InterwikiLocalFirstBegins-->", "<!--InterwikiLocalFirstEnds-->"));

            int no = 0;
            int size = IWSplit.Matches(interwikiLocalFirstRaw).Count + 1;

            InterwikiLocalAlpha = new string[IWSplit.Matches(interwikiLocalAlphaRaw).Count + 1];

            foreach (string s in interwikiLocalAlphaRaw.Split(new [] {","}, StringSplitOptions.RemoveEmptyEntries)
                )
            {
                InterwikiLocalAlpha[no] = s.Trim().ToLower();
                no++;
            }

            InterwikiLocalFirst = new string[size];
            no = 0;

            foreach (string s in interwikiLocalFirstRaw.Split(new [] {","}, StringSplitOptions.RemoveEmptyEntries)
                )
            {
                InterwikiLocalFirst[no] = s.Trim().ToLower();
                no++;
            }

            InterwikiAlpha = (string[]) InterwikiLocalFirst.Clone();
            Array.Sort(InterwikiAlpha, StringComparer.Create(new System.Globalization.CultureInfo("en-US", true), true));

            string[] temp = (string[]) InterwikiAlpha.Clone();
            temp[Array.IndexOf(temp, "en")] = "";

            InterwikiAlphaEnFirst = new string[size];
            InterwikiAlphaEnFirst[0] = "en";
            no = 1;

            foreach (string s in temp)
            {
                if (s.Trim().Length > 0)
                {
                    InterwikiAlphaEnFirst[no] = s;
                    no++;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        private static string remExtra(string Input)
        {
            return Input.Replace("\r\n", "").Replace(">", "").Replace("\n", "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string Newline(string s)
        {
            return Newline(s, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static string Newline(string s, int n)
        {
            if (s.Length == 0)
                return s;

            for (int i = 0; i < n; i++)
                s = "\r\n" + s;
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText"></param>
        /// <param name="ArticleTitle"></param>
        /// <returns></returns>
        internal string Sort(string ArticleText, string ArticleTitle)
        {
            if (Tools.CalculateNS(ArticleTitle) == 10) //Dont really want to be fooling around with templates
                return ArticleText;

            string strSave = ArticleText;
            try
            {
                ArticleText = Regex.Replace(ArticleText, "<!-- ?\\[\\[en:.*?\\]\\] ?-->", "");

                string strPersonData = Newline(removePersonData(ref ArticleText));
                string strDisambig = Newline(removeDisambig(ref ArticleText));
                string strCategories = Newline(removeCats(ref ArticleText, ArticleTitle));
                string strInterwikis = Newline(interwikis(ref ArticleText));

                ArticleText = moveDablinks(ArticleText);

                // two newlines here per http://en.wikipedia.org/w/index.php?title=Wikipedia_talk:AutoWikiBrowser&oldid=243224092#Blank_lines_before_stubs
                string strStub = Newline(removeStubs(ref ArticleText), 2);

                //filter out excess white space and remove "----" from end of article
                ArticleText = Parsers.RemoveWhiteSpace(ArticleText) + "\r\n";
                ArticleText += strDisambig;

                switch (Variables.LangCode)
                {
                    case LangCodeEnum.de:
                    case LangCodeEnum.sl:
                        ArticleText += strStub + strCategories + strPersonData;
                        break;
                    case LangCodeEnum.pl:
                    case LangCodeEnum.ru:
                    case LangCodeEnum.simple:
                        ArticleText += strPersonData + strStub + strCategories;
                        break;
                    default:
                        ArticleText += strPersonData + strCategories + strStub;
                        break;
                }
                return (ArticleText + strInterwikis);
            }
            catch(Exception ex)
            {
                if (!ex.Message.Contains("DEFAULTSORT")) ErrorHandler.Handle(ex);
                return strSave;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText"></param>
        /// <param name="ArticleTitle"></param>
        /// <returns></returns>
        public string removeCats(ref string ArticleText, string ArticleTitle)
        {
            List<string> categoryList = new List<string>();
 
            Regex r = new Regex("<!-- ? ?\\[\\[" + Variables.NamespacesCaseInsensitive[Namespace.Category]
                + ".*?(\\]\\]|\\|.*?\\]\\]).*?-->|\\[\\[" 
                + Variables.NamespacesCaseInsensitive[Namespace.Category] 
                + ".*?(\\]\\]|\\|.*?\\]\\])( {0,4}⌊⌊⌊⌊[0-9]{1,4}⌋⌋⌋⌋)?");

            MatchCollection matches = r.Matches(ArticleText);
            foreach (Match m in matches)
            {
                string x = m.Value;
                //add to array, replace underscores with spaces, ignore=
                if (!Regex.IsMatch(x, "\\[\\[Category:(Pages|Categories|Articles) for deletion\\]\\]"))
                {
                    categoryList.Add(x.Replace("_", " "));
                }
            }

            ArticleText = Tools.RemoveMatches(ArticleText, matches);

            if (parser.addCatKey)
                categoryList = catKeyer(categoryList, ArticleTitle);

            if (CatCommentRegex.IsMatch(ArticleText))
            {
                string catComment = CatCommentRegex.Match(ArticleText).Value;
                ArticleText = ArticleText.Replace(catComment, "");
                categoryList.Insert(0, catComment);
            }

            MatchCollection mc = WikiRegexes.Defaultsort.Matches(ArticleText);
            if (mc.Count > 1) throw new ArgumentException("Page contains multiple {{DEFAULTSORTS}} tags. Metadata sorting cancelled");

            string defaultSort = "";
            if (mc.Count > 0) defaultSort = mc[0].Value;

            if (!string.IsNullOrEmpty(defaultSort))
                ArticleText = ArticleText.Replace(defaultSort, "");

            if (!string.IsNullOrEmpty(defaultSort) && defaultSort.ToUpper().Contains("DEFAULTSORT"))
            {
                defaultSort = TalkPageHeaders.FormatDefaultSort(defaultSort);
            }
            if (!string.IsNullOrEmpty(defaultSort)) defaultSort += "\r\n";

            return defaultSort + ListToString(categoryList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText"></param>
        /// <returns></returns>
        public static string removePersonData(ref string ArticleText)
        {
            string strPersonData = (Variables.LangCode == LangCodeEnum.de)
                                ? Parsers.GetTemplate(ArticleText, "[Pp]ersonendaten")
                                : Parsers.GetTemplate(ArticleText, "[Pp]ersondata");

            if (!string.IsNullOrEmpty(strPersonData))
                ArticleText = ArticleText.Replace(strPersonData, "");

            return strPersonData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText"></param>
        /// <returns></returns>
        public static string removeStubs(ref string ArticleText)
        {
            List<string> stubList = new List<string>();
            MatchCollection matches = WikiRegexes.PossiblyCommentedStub.Matches(ArticleText);
            if (matches.Count == 0) return "";

            string x;
            StringBuilder sb = new StringBuilder(ArticleText);

            for (int i = matches.Count - 1; i >= 0; i--)
            {
                Match m = matches[i];
                x = m.Value;
                if (!Regex.IsMatch(x, Variables.SectStub) && !x.Contains("|"))
                {
                    stubList.Add(x);
                    sb.Remove(m.Index, x.Length);
                }
            }
            ArticleText = sb.ToString();

            stubList.Reverse();
            if (stubList.Count != 0)
                return ListToString(stubList);
            
            return "";
        }

        /// <summary>
        /// Removes any disambiguation templates from the article text, to be added at bottom later
        /// </summary>
        /// <param name="ArticleText"></param>
        /// <returns>Article text stripped of disambiguation templates</returns>
        public static string removeDisambig(ref string ArticleText)
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
        
        /// <summary>
        /// Moves any disambiguation links to the top of the article (en only)
        /// </summary>
        /// <param name="ArticleText"></param>
        /// <returns>Article text with disambiguation links at top</returns>
        public static string moveDablinks(string ArticleText)
        {
            // avoid moving commented out Dablinks
            if (Variables.LangCode != LangCodeEnum.en || !WikiRegexes.Dablinks.IsMatch(WikiRegexes.Comments.Replace(ArticleText, "")))
              return ArticleText;

            string strDablinks = "";

            foreach (Match m in WikiRegexes.Dablinks.Matches(ArticleText))
            {
                strDablinks = strDablinks + m.Value + "\r\n";
                ArticleText = ArticleText.Replace(m.Value, "");
            }

            ArticleText = strDablinks + ArticleText;

            // may now have two newlines between dablinks and rest of article, so cut down to one
            return ArticleText.Replace(strDablinks + "\r\n", strDablinks);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText"></param>
        /// <returns></returns>
        private static List<string> removeLinkFAs(ref string ArticleText)
        {
            List<string> linkFAList = new List<string>();
            foreach (Match m in WikiRegexes.LinkFAs.Matches(ArticleText))
            {
                string x = m.Value;
                linkFAList.Add(x);
                //remove old LinkFA
                ArticleText = ArticleText.Replace(x, "");
            }

            return linkFAList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText"></param>
        /// <returns></returns>
        public string interwikis(ref string ArticleText)
        {
            return ListToString(removeLinkFAs(ref ArticleText)) + ListToString(removeInterWikis(ref ArticleText));
        }

        static readonly Regex FastIW = new Regex(@"\[\[\s*([-a-zA-Z]*?)\s*:\s*([^\]]*?)\s*\]\]", RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText"></param>
        /// <returns></returns>
        private List<string> removeInterWikis(ref string ArticleText)
        {
            List<string> interWikiList = new List<string>();
            MatchCollection matches = FastIW.Matches(ArticleText);
            if (matches.Count == 0) return interWikiList;

            List<Match> goodMatches = new List<Match>(matches.Count);

            foreach (Match m in matches)
            {
                string site = m.Groups[1].Value.Trim().ToLower();
                if (!PossibleInterwikis.Contains(site)) continue;
                goodMatches.Add(m);
                interWikiList.Add("[[" + site + ":" + m.Groups[2].Value.Trim() + "]]");
            }

            ArticleText = Tools.RemoveMatches(ArticleText, goodMatches);

            string interWikiComment = "";
            if (InterLangRegex.IsMatch(ArticleText))
            {

                interWikiComment = InterLangRegex.Match(ArticleText).Value;
                ArticleText = ArticleText.Replace(interWikiComment, "");
            }

            if (parser.sortInterwikiOrder)
            {
                interWikiList.Sort(Comparer);
            }

            if (!string.IsNullOrEmpty(interWikiComment)) interWikiList.Insert(0, interWikiComment);

            return interWikiList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public static string IWMatchEval(Match match)
        {
            string[] textArray = new [] { "[[", match.Groups["site"].ToString().ToLower(), ":", match.Groups["text"].ToString(), "]]" };
            return string.Concat(textArray);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private static string ListToString(List<string> items)
        {//remove duplicates, and return List as string.

            if (items.Count == 0)
                return "";

            List<string> uniqueItems = new List<string>();

            //remove duplicates
            foreach (string s in items)
            {
                if (!uniqueItems.Contains(s))
                    uniqueItems.Add(s);
            }

            StringBuilder list = new StringBuilder();
            //add to string
            foreach (string s in uniqueItems)
            {
                list.AppendLine(s);
            }

            return list.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="List"></param>
        /// <param name="strName"></param>
        /// <returns></returns>
        private static List<string> catKeyer(List<string> List, string strName)
        {
            strName = Tools.MakeHumanCatKey(strName); // make key

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
