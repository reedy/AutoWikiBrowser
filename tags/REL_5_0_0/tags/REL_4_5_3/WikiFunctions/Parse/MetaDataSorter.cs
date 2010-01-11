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
using WikiFunctions.TalkPages;

namespace WikiFunctions.Parse
{
    public enum InterWikiOrderEnum { LocalLanguageAlpha, LocalLanguageFirstWord, Alphabetical, AlphabeticalEnFirst }

    public class MetaDataSorter
    {
        public List<string> PossibleInterwikis;

        public bool SortInterwikis
        { get; set; }

        public bool AddCatKey
        { get; set; }

        public MetaDataSorter()
        {
            SortInterwikis = true;

            if (!LoadInterWikiFromCache())
            {
                LoadInterWikiFromNetwork();
                SaveInterWikiToCache();
            }

            if (InterwikiLocalAlpha == null)
                throw new NullReferenceException("InterwikiLocalAlpha is null");

            //create a comparer
            InterWikiOrder = InterWikiOrderEnum.LocalLanguageAlpha;
        }

        // now will be generated dynamically using Variables.Stub
        readonly Regex InterLangRegex = new Regex("<!-- ?(other languages?|language links?|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|inter ?wikis?|The below are interlanguage links\\.?) ?-->", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        readonly Regex CatCommentRegex = new Regex("<!-- ?cat(egories)? ?-->", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private List<string> InterwikiLocalAlpha;
        private List<string> InterwikiLocalFirst;
        private List<string> InterwikiAlpha;
        private List<string> InterwikiAlphaEnFirst;
        //List<Regex> InterWikisList = new List<Regex>();

        private InterWikiComparer Comparer;
        private InterWikiOrderEnum Order = InterWikiOrderEnum.LocalLanguageAlpha;

        /// <summary>
        /// 
        /// </summary>
        public InterWikiOrderEnum InterWikiOrder
        {//orders from http://meta.wikimedia.org/wiki/Interwiki_sorting_order
            set
            {
                Order = value;

                List<string> seq;
                switch (Order)
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
                return Order;
            }
        }

        private bool Loaded = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="what"></param>
        /// <returns></returns>
        private List<string> Load(string what)
        {
            var result = (List<string>)ObjectCache.Global.Get<List<string>>(Key(what));
            if (result == null)
            {
                Loaded = false;
                return new List<string>();
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveInterWikiToCache()
        {
            ObjectCache.Global.Set(Key("InterwikiLocalAlpha"), InterwikiLocalAlpha);
            ObjectCache.Global.Set(Key("InterwikiLocalFirst"), InterwikiLocalFirst);
            ObjectCache.Global.Set(Key("InterwikiAlpha"), InterwikiAlpha);
            ObjectCache.Global.Set(Key("InterwikiAlphaEnFirst"), InterwikiAlphaEnFirst);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="what"></param>
        /// <returns></returns>
        private static string Key(string what)
        {
            return "MetaDataSorter::" + what;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool LoadInterWikiFromCache()
        {
            InterwikiLocalAlpha = Load("InterwikiLocalAlpha");
            InterwikiLocalFirst = Load("InterwikiLocalFirst");
            InterwikiAlpha = Load("InterwikiAlpha");
            InterwikiAlphaEnFirst = Load("InterwikiAlphaEnFirst");

            return Loaded;
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadInterWikiFromNetwork()
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
                RemExtra(Tools.StringBetween(text, "<!--InterwikiLocalAlphaBegins-->", "<!--InterwikiLocalAlphaEnds-->"));
            string interwikiLocalFirstRaw =
                RemExtra(Tools.StringBetween(text, "<!--InterwikiLocalFirstBegins-->", "<!--InterwikiLocalFirstEnds-->"));

            InterwikiLocalAlpha = new List<string>();

            foreach (string s in interwikiLocalAlphaRaw.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                )
            {
                InterwikiLocalAlpha.Add(s.Trim().ToLower());
            }

            InterwikiLocalFirst = new List<string>();

            foreach (string s in interwikiLocalFirstRaw.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                )
            {
                InterwikiLocalFirst.Add(s.Trim().ToLower());
            }

            InterwikiAlpha = new List<string>(InterwikiLocalFirst);
            InterwikiAlpha.Sort(StringComparer.Create(new System.Globalization.CultureInfo("en-US", true), true));

            InterwikiAlphaEnFirst = new List<string>(InterwikiAlpha);
            InterwikiAlphaEnFirst.Remove("en");
            InterwikiAlphaEnFirst.Insert(0, "en");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string RemExtra(string input)
        {
            return input.Replace("\r\n", "").Replace(">", "").Replace("\n", "");
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
        /// Adds the specified number of newlines to the string
        /// </summary>
        /// <param name="s">Input string</param>
        /// <param name="n">Number of newlines to add</param>
        /// <returns>Input string + (n x newlines)</returns>
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
        /// <param name="articleText"></param>
        /// <param name="articleTitle"></param>
        /// <returns></returns>
        internal string Sort(string articleText, string articleTitle)
        {
            if (Namespace.Determine(articleTitle) == 10) //Dont really want to be fooling around with templates
                return articleText;

            string strSave = articleText;
            try
            {
                articleText = Regex.Replace(articleText, "<!-- ?\\[\\[en:.*?\\]\\] ?-->", "");

                string strPersonData = Newline(RemovePersonData(ref articleText));
                string strDisambig = Newline(RemoveDisambig(ref articleText));
                string strCategories = Newline(RemoveCats(ref articleText, articleTitle));
                string strInterwikis = Newline(Interwikis(ref articleText));

                articleText = MoveDablinks(articleText);

                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Placement_of_portal_template
                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests.html#Move_nofootnotes_to_the_references_section
                if (Variables.LangCode == LangCodeEnum.en)
                {
                    articleText = MovePortalTemplates(articleText);
                    articleText = MoveMoreNoFootnotes(articleText);
                    articleText = MoveExternalLinks(articleText);
                }

                // two newlines here per http://en.wikipedia.org/w/index.php?title=Wikipedia_talk:AutoWikiBrowser&oldid=243224092#Blank_lines_before_stubs
                string strStub = Newline(RemoveStubs(ref articleText), 2);

                //filter out excess white space and remove "----" from end of article
                articleText = Parsers.RemoveWhiteSpace(articleText) + "\r\n";
                articleText += strDisambig;

                switch (Variables.LangCode)
                {
                    case LangCodeEnum.de:
                    case LangCodeEnum.sl:
                        articleText += strStub + strCategories + strPersonData;

                        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser#Removal_of_blank_lines
                        // on de wiki a blank line is desired between persondata and interwikis
                        if (Variables.LangCode == LangCodeEnum.de && strPersonData.Length > 0 && strInterwikis.Length > 0)
                            articleText += "\r\n";
                        break;
                    case LangCodeEnum.pl:
                    case LangCodeEnum.ru:
                    case LangCodeEnum.simple:
                        articleText += strPersonData + strStub + strCategories;
                        break;
                    default:
                        articleText += strPersonData + strCategories + strStub;
                        break;
                }
                return (articleText + strInterwikis);
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("DEFAULTSORT")) ErrorHandler.Handle(ex);
                return strSave;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText"></param>
        /// <param name="articleTitle"></param>
        /// <returns></returns>
        public string RemoveCats(ref string articleText, string articleTitle)
        {
            List<string> categoryList = new List<string>();

            Regex r = new Regex("<!-- ? ?\\[\\[" + Variables.NamespacesCaseInsensitive[Namespace.Category]
                + ".*?(\\]\\]|\\|.*?\\]\\]).*?-->|\\[\\["
                + Variables.NamespacesCaseInsensitive[Namespace.Category]
                + ".*?(\\]\\]|\\|.*?\\]\\])( {0,4}⌊⌊⌊⌊[0-9]{1,4}⌋⌋⌋⌋)?");

            MatchCollection matches = r.Matches(articleText);
            foreach (Match m in matches)
            {
                string x = m.Value;
                //add to array, replace underscores with spaces, ignore=
                if (!Regex.IsMatch(x, "\\[\\[Category:(Pages|Categories|Articles) for deletion\\]\\]"))
                {
                    categoryList.Add(x.Replace("_", " "));
                }
            }

            articleText = Tools.RemoveMatches(articleText, matches);

            if (AddCatKey)
                categoryList = CatKeyer(categoryList, articleTitle);

            if (CatCommentRegex.IsMatch(articleText))
            {
                string catComment = CatCommentRegex.Match(articleText).Value;
                articleText = articleText.Replace(catComment, "");
                categoryList.Insert(0, catComment);
            }

            MatchCollection mc = WikiRegexes.Defaultsort.Matches(articleText);
            if (mc.Count > 1) throw new ArgumentException("Page contains multiple {{DEFAULTSORTS}} tags. Metadata sorting cancelled");

            string defaultSort = "";
            if (mc.Count > 0) defaultSort = mc[0].Value;

            if (!string.IsNullOrEmpty(defaultSort))
                articleText = articleText.Replace(defaultSort, "");

            if (!string.IsNullOrEmpty(defaultSort) && defaultSort.ToUpper().Contains("DEFAULTSORT"))
            {
                defaultSort = TalkPageHeaders.FormatDefaultSort(defaultSort);
            }
            if (!string.IsNullOrEmpty(defaultSort)) defaultSort += "\r\n";

            // on en-wiki find any {{Lifetime}} template and move directly after categories
            string lifetime = "";
            if (Variables.LangCode == LangCodeEnum.en)
            {
                lifetime = WikiRegexes.Lifetime.Match(articleText).Value;

                if (!string.IsNullOrEmpty(lifetime))
                {
                    articleText = articleText.Replace(lifetime, "");

                    // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#Blank_lines_after_Lifetime
                    lifetime += "\r\n";
                }
            }

            return defaultSort + ListToString(categoryList) + lifetime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public static string RemovePersonData(ref string articleText)
        {
            string strPersonData = (Variables.LangCode == LangCodeEnum.de)
                                ? Parsers.GetTemplate(articleText, "[Pp]ersonendaten")
                                : Parsers.GetTemplate(articleText, "[Pp]ersondata");

            if (!string.IsNullOrEmpty(strPersonData))
                articleText = articleText.Replace(strPersonData, "");

            return strPersonData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public static string RemoveStubs(ref string articleText)
        {
            List<string> stubList = new List<string>();
            MatchCollection matches = WikiRegexes.PossiblyCommentedStub.Matches(articleText);
            if (matches.Count == 0) return "";

            string x;
            StringBuilder sb = new StringBuilder(articleText);

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
            articleText = sb.ToString();

            stubList.Reverse();
            return (stubList.Count != 0) ? ListToString(stubList) : "";
        }

        /// <summary>
        /// Removes any disambiguation templates from the article text, to be added at bottom later
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns>Article text stripped of disambiguation templates</returns>
        public static string RemoveDisambig(ref string articleText)
        {
            if (Variables.LangCode != LangCodeEnum.en)
                return "";

            string strDisambig = "";
            if (WikiRegexes.Disambigs.IsMatch(articleText))
            {
                strDisambig = WikiRegexes.Disambigs.Match(articleText).Value;
                articleText = articleText.Replace(strDisambig, "");
            }

            return strDisambig;
        }

        /// <summary>
        /// Moves any disambiguation links in the zeroth section to the top of the article (en only)
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns>Article text with disambiguation links at top</returns>
        public static string MoveDablinks(string articleText)
        {
            // get the zeroth section (text upto first heading)
            string zerothSection = WikiRegexes.ZerothSection.Match(articleText).Value;

            // get the rest of the article including first heading (may be null if article has no headings)
            string restOfArticle = articleText.Replace(zerothSection, "");

            // avoid moving commented out Dablinks
            if (Variables.LangCode != LangCodeEnum.en || !WikiRegexes.Dablinks.IsMatch(WikiRegexes.Comments.Replace(zerothSection, "")))
                return articleText;

            string strDablinks = "";

            foreach (Match m in WikiRegexes.Dablinks.Matches(zerothSection))
            {
                strDablinks = strDablinks + m.Value + "\r\n";
                zerothSection = zerothSection.Replace(m.Value, "");
            }

            articleText = strDablinks + zerothSection + restOfArticle;

            // may now have two newlines between dablinks and rest of article, so cut down to one
            return articleText.Replace(strDablinks + "\r\n", strDablinks);
        }

        private static readonly Regex SeeAlso = new Regex(@"(\s*(==+)\s*see\s+also\s*\2)",  RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex SeeAlsoSection = new Regex(@"(\s*(==+)\s*see\s+also\s*\2 *).*?(?===[^=][^\r\n]*?[^=]==(\r\n?|\n))", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex SeeAlsoToEnd = new Regex(@"(\s*(==+)\s*see\s+also\s*\2 *).*",  RegexOptions.IgnoreCase | RegexOptions.Singleline);
        
        /// <summary>
        /// Moves any {{XX portal}} templates to the 'see also' section, if present (en only)
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns>Article text with {{XX portal}} template correctly placed</returns>
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Placement_of_portal_template
        public static string MovePortalTemplates(string articleText)
        {
            // need to have a 'see also' section to move the portal template to
            if (WikiRegexes.PortalTemplate.Matches(articleText).Count >= 1 && SeeAlso.Matches(articleText).Count == 1)
            {
                foreach (Match m in WikiRegexes.PortalTemplate.Matches(articleText))
                {
                    string portalTemplateFound = m.Value;
                    string seeAlsoSectionString = SeeAlsoSection.Match(articleText).Value;

                    // if SeeAlsoSection didn't match then 'see also' must be last section
                    if (seeAlsoSectionString.Equals(""))
                        seeAlsoSectionString = SeeAlsoToEnd.Match(articleText).Value;

                    // check portal template NOT currently in 'see also'
                    if (!seeAlsoSectionString.Contains(portalTemplateFound.Trim()))
                    {
                        articleText = articleText.Replace(portalTemplateFound + "\r\n", "");
                        articleText = SeeAlso.Replace(articleText, "$0" + "\r\n" + portalTemplateFound);
                    }
                }
            }

            return (articleText);
        }
        
        private static readonly Regex ReferencesSectionRegex = new Regex(@"^== *[Rr]eferences *==\s*", RegexOptions.Multiline);
        private static readonly Regex NotesSectionRegex = new Regex(@"^== *[Nn]otes *==\s*", RegexOptions.Multiline);
        private static readonly Regex FootnotesSectionRegex = new Regex(@"^== *[Ff]ootnotes *==\s*", RegexOptions.Multiline);

        /// <summary>
        /// Moves any {{nofootnotes}} or {{morefootnotes}} to the references section from the zeroth section, if present (en only)
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns>Article text with {{nofootnotes}} or {{morefootnotes}} template correctly placed</returns>
        public static string MoveMoreNoFootnotes(string articleText)
        {
          // no support for more than one of these templates in the article
          string zerothSection = WikiRegexes.ZerothSection.Match(articleText).Value;
          if(WikiRegexes.MoreNoFootnotes.Matches(zerothSection).Count != 1)
            return articleText;          

          // find the template position
          int moreNoFootnotesPosition = WikiRegexes.MoreNoFootnotes.Match(articleText).Index;
          
          // the template must be in one of the 'References', 'Notes' or 'Footnotes' section          
          int referencesSectionPosition = ReferencesSectionRegex.Match(articleText).Index;
          
          if(referencesSectionPosition > 0 && moreNoFootnotesPosition < referencesSectionPosition)
            return MoveMoreNoFootnotesToSection(articleText, 1);
          
          int notesSectionPosition = NotesSectionRegex.Match(articleText).Index;
          
          if(notesSectionPosition > 0 && moreNoFootnotesPosition < notesSectionPosition)
            return MoveMoreNoFootnotesToSection(articleText, 2);
            
          int footnotesSectionPosition = FootnotesSectionRegex.Match(articleText).Index;
          
          if(footnotesSectionPosition > 0 && moreNoFootnotesPosition < footnotesSectionPosition)
            return MoveMoreNoFootnotesToSection(articleText, 3);
            
          return articleText;        
        }
        
        private static string MoveMoreNoFootnotesToSection(string articleText, int section)
        {
            // extract the template
            string moreNoFootnotes = WikiRegexes.MoreNoFootnotes.Match(articleText).Value;
            articleText = articleText.Replace(moreNoFootnotes, "");

            switch (section)
            {
                case 1:
                    return ReferencesSectionRegex.Replace(articleText, "$0" + moreNoFootnotes + "\r\n");
                case 2:
                    return NotesSectionRegex.Replace(articleText, "$0" + moreNoFootnotes + "\r\n");
                case 3:
                    return FootnotesSectionRegex.Replace(articleText, "$0" + moreNoFootnotes + "\r\n");
                default:
                    return articleText;
            }
        }

        private static readonly Regex ExternalLinksSection = new Regex(@"(^== *[Ee]xternal +[Ll]inks? *==.*?)(?=^==+[^=][^\r\n]*?[^=]==+(\r\n?|\n)$)", RegexOptions.Multiline | RegexOptions.Singleline);
        private static readonly Regex ReferencesSection = new Regex(@"(^== *[Rr]eferences *==.*?)(?=^==[^=][^\r\n]*?[^=]==(\r\n?|\n)$)", RegexOptions.Multiline | RegexOptions.Singleline);
        private static readonly Regex ReferencesToEnd = new Regex(@"^== *[Rr]eferences *==\s*" + WikiRegexes.ReferencesTemplates + @"\s*(?={{DEFAULTSORT\:|\[\[Category\:)", RegexOptions.Multiline);

        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Place_.22External_links.22_section_after_.22References.22
        /// <summary>
        /// Ensures the external links section of an article is after the references section
        /// TODO only works when there is another section following the references section
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns>Article text with external links section below the references section</returns>
        public static string MoveExternalLinks(string articleText)
        {
            // is external links section above references?
            string externalLinks = ExternalLinksSection.Match(articleText).Groups[1].Value;
            string references = ReferencesSection.Match(articleText).Groups[1].Value;

            // references may be last section
            if (references.Equals(""))
                references = ReferencesToEnd.Match(articleText).Value;

            if (articleText.IndexOf(externalLinks) < articleText.IndexOf(references) && references.Length > 0 && externalLinks.Length > 0)
            {
                articleText = articleText.Replace(externalLinks, "");
                articleText = articleText.Replace(references, references + externalLinks);
            }
            // newlines are fixed by later logic
            return articleText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private static List<string> RemoveLinkFGAs(ref string articleText)
        {
            List<string> linkFGAList = new List<string>();
            foreach (Match m in WikiRegexes.LinkFGAs.Matches(articleText))
            {
                string x = m.Value;
                linkFGAList.Add(x);
                //remove old LinkFA
                articleText = articleText.Replace(x, "");
            }

            return linkFGAList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public string Interwikis(ref string articleText)
        {
            return ListToString(RemoveLinkFGAs(ref articleText)) + ListToString(RemoveInterWikis(ref articleText));
        }

        private static readonly Regex FastIW = new Regex(@"\[\[\s*([-a-zA-Z]*?)\s*:\s*([^\]]*?)\s*\]\]", RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private List<string> RemoveInterWikis(ref string articleText)
        {
            List<string> interWikiList = new List<string>();
            MatchCollection matches = FastIW.Matches(articleText);
            if (matches.Count == 0) return interWikiList;

            List<Match> goodMatches = new List<Match>(matches.Count);

            foreach (Match m in matches)
            {
                string site = m.Groups[1].Value.Trim().ToLower();
                if (!PossibleInterwikis.Contains(site)) continue;
                goodMatches.Add(m);
                interWikiList.Add("[[" + site + ":" + m.Groups[2].Value.Trim() + "]]");
            }

            articleText = Tools.RemoveMatches(articleText, goodMatches);

            string interWikiComment = "";
            if (InterLangRegex.IsMatch(articleText))
            {
                interWikiComment = InterLangRegex.Match(articleText).Value;
                articleText = articleText.Replace(interWikiComment, "");
            }

            if (SortInterwikis)
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
            string[] textArray = new[] { "[[", match.Groups["site"].ToString().ToLower(), ":", match.Groups["text"].ToString(), "]]" };
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
        /// <param name="list"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private static List<string> CatKeyer(List<string> list, string name)
        {
            name = Tools.MakeHumanCatKey(name); // make key

            //add key to cats that need it
            List<string> newCats = new List<string>();
            foreach (string s in list)
            {
                string z = s;
                if (!z.Contains("|"))
                    z = z.Replace("]]", "|" + name + "]]");

                newCats.Add(z);
            }
            return newCats;
        }
    }
}
