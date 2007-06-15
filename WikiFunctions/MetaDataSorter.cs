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

            InterWikisList.Clear();
            foreach (string s in InterwikiLocalAlpha)
                //InterWikisList.Add(new Regex("\\[\\[" + s + ":.*?\\]\\]", RegexOptions.Compiled));
                InterWikisList.Add(new Regex("\\[\\[(?<site>" + s + "):(?<text>.*?)\\]\\]", RegexOptions.Compiled | RegexOptions.IgnoreCase));
        }

        // now will be generated dynamically using Variables.Stub
        //Regex StubsRegex = new Regex("<!-- ?\\{\\{.*?stub\\}\\}.*?-->|:?\\{\\{.*?stub\\}\\}");
        Regex InterLangRegex = new Regex("<!-- ?(other languages|language links|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|inter ?wiki|The below are interlanguage links\\.?) ?-->", RegexOptions.IgnoreCase);
        Regex CatCommentRegex = new Regex("<!-- ?categories ?-->", RegexOptions.IgnoreCase);

        readonly string[] InterwikiLocalAlpha = new string[] { "aa", "af", "ak", "als", "am", "ang", "ab", "ar", "an", "roa-rup", "frp", "as", "ast", "gn", "av", "ay", "az", "bm", "bn", "zh-min-nan", "map-bms", "ba", "be", "be-x-old", "bh", "bi", "bar", "bo", "bs", "br", "bg", "bxr", "ca", "cv", "ceb", "cs", "ch", "cbk-zam", "ny", "sn", "tum", "cho", "co", "za", "cy", "da", "pdc", "de", "dv", "arc", "nv", "dz", "mh", "et", "el", "eml", "en", "es", "eo", "eu", "ee", "fa", "fo", "fr", "fy", "ff", "fur", "ga", "gv", "gd", "gl", "ki", "glk", "gu", "got", "zh-classical", "xal", "ko", "ha", "hak", "haw", "hy", "hi", "ho", "hsb", "hr", "io", "ig", "ilo", "bpy", "id", "ia", "ie", "iu", "ik", "os", "xh", "zu", "is", "it", "he", "jv", "kl", "kn", "kr", "ka", "ks", "csb", "kk", "kw", "rw", "ky", "rn", "sw", "kv", "kg", "ht", "kj", "ku", "lad", "lbe", "lo", "la", "lv", "lb", "lt", "lij", "li", "ln", "jbo", "lg", "lmo", "hu", "mk", "mg", "ml", "mt", "mi", "mr", "mzn", "ms", "cdo", "mo", "mn", "mus", "my", "nah", "na", "fj", "nl", "nds-nl", "cr", "ne", "new", "ja", "nap", "ce", "pih", "no", "nn", "nrm", "nov", "oc", "or", "om", "ng", "hz", "ug", "pa", "pi", "pam", "pag", "pap", "ps", "km", "pms", "nds", "pl", "pt", "ty", "ksh", "ro", "rmy", "rm", "qu", "ru", "war", "se", "sm", "sa", "sg", "sc", "sco", "st", "tn", "sq", "ru-sib", "scn", "si", "simple", "sd", "ss", "sk", "cu", "sl", "so", "sr", "sh", "su", "fi", "sv", "tl", "ta", "roa-tara", "tt", "te", "tet", "th", "vi", "ti", "tg", "tpi", "to", "chr", "chy", "ve", "tr", "tk", "tw", "udm", "bug", "uk", "ur", "uz", "vec", "vo", "fiu-vro", "wa", "vls", "wo", "wuu", "ts", "ii", "yi", "yo", "zh-yue", "diq", "zea", "bat-smg", "zh", "zh-tw", "zh-cn" };
        readonly string[] InterwikiLocalFirst = new string[] { "aa", "af", "ak", "als", "am", "ang", "ab", "ar", "an", "roa-rup", "frp", "as", "ast", "gn", "av", "ay", "az", "id", "ms", "bm", "bn", "zh-min-nan", "ban", "map-bms", "jv", "su", "bug", "ba", "be", "be-x-old", "bh", "mt", "bi", "bo", "bs", "br", "bg", "ca", "ceb", "cv", "cs", "ch", "ny", "sn", "tum", "cho", "co", "za", "cy", "da", "pdc", "de", "dv", "nv", "dz", "mh", "et", "na", "el", "en", "es", "eo", "eu", "ee", "to", "fa", "fo", "fr", "fy", "ff", "fur", "ga", "gv", "sm", "gd", "gl", "gay", "ki", "gu", "got", "ko", "ha", "hak", "haw", "hy", "hi", "ho", "hr", "io", "ig", "ia", "ie", "iu", "ik", "os", "xh", "zu", "is", "it", "he", "kl", "xal", "kn", "kr", "ka", "ks", "csb", "kw", "rw", "ky", "rn", "sw", "kv", "kg", "ht", "kj", "ku", "lad", "lo", "la", "lv", "lb", "lt", "lij", "li", "ln", "jbo", "lg", "lmo", "hu", "mk", "mg", "ml", "mi", "mr", "chm", "mo", "mn", "mus", "my", "nah", "fj", "nap", "nds-nl", "nl", "cr", "ne", "ja", "ce", "pih", "nb", "no", "nn", "nrm", "oc", "or", "om", "ng", "hz", "ug", "uz", "pa", "kk", "pi", "pam", "pap", "ps", "km", "pms", "nds", "pl", "pt", "ty", "ksh", "ro", "rm", "rmy", "qu", "ru", "se", "sa", "sg", "sc", "sco", "st", "tn", "sq", "scn", "si", "simple", "sd", "ss", "sk", "sl", "so", "sr", "sh", "fi", "sv", "tl", "ta", "tt", "te", "tet", "th", "vi", "ti", "tlh", "tg", "tpi", "chr", "chy", "ve", "tr", "tk", "tw", "udm", "uk", "ur", "vec", "vo", "fiu-vro", "wa", "war", "vls", "wo", "ts", "ii", "yi", "yo", "zh-yue", "bat-smg", "zh", "zh-tw", "zh-cn" };
        readonly string[] InterwikiAlpha = new string[] { "aa", "ab", "af", "ak", "als", "am", "an", "ang", "ar", "as", "ast", "av", "ay", "az", "ba", "bar", "bat-smg", "be", "be-x-old", "bg", "bh", "bi", "bm", "bn", "bo", "bpy", "br", "bs", "bug", "bxr", "ca", "cbk-zam", "cdo", "ce", "ceb", "ch", "cho", "chr", "chy", "co", "cr", "cs", "csb", "cu", "cv", "cy", "da", "de", "diq", "dv", "dz", "ee", "el", "eml", "en", "eo", "es", "et", "eu", "fa", "ff", "fi", "fiu-vro", "fj", "fo", "fr", "frp", "fur", "fy", "ga", "gd", "gl", "glk", "gn", "got", "gu", "gv", "ha", "hak", "haw", "he", "hi", "ho", "hr", "hsb", "ht", "hu", "hy", "hz", "ia", "id", "ie", "ig", "ii", "ik", "ilo", "io", "is", "it", "iu", "ja", "jbo", "jv", "ka", "kg", "ki", "kj", "kk", "kl", "km", "kn", "ko", "kr", "ks", "ksh", "ku", "kv", "kw", "ky", "la", "lad", "lb", "lbe", "lg", "li", "lij", "lmo", "ln", "lo", "lt", "lv", "map-bms", "mg", "mh", "mi", "mk", "ml", "mn", "mo", "mr", "ms", "mt", "mus", "my", "mzn", "na", "nah", "nap", "nds", "nds-nl", "ne", "new", "ng", "nl", "nn", "no", "nov", "nrm", "nv", "ny", "oc", "om", "or", "os", "pa", "pag", "pam", "pap", "pdc", "pi", "pih", "pl", "pms", "ps", "pt", "qu", "rm", "rmy", "rn", "ro", "roa-rup", "roa-tara", "ru", "ru-sib", "rw", "sa", "sc", "scn", "sco", "sd", "se", "sg", "sh", "si", "simple", "sk", "sl", "sm", "sn", "so", "sq", "sr", "ss", "st", "su", "sv", "sw", "ta", "te", "tet", "tg", "th", "ti", "tk", "tl", "tlh", "tn", "to", "tpi", "tr", "ts", "tt", "tum", "tw", "ty", "udm", "ug", "uk", "ur", "uz", "ve", "vec", "vi", "vls", "vo", "wa", "war", "wo", "wuu", "xal", "xh", "yi", "yo", "za", "zea", "zh", "zh-classical", "zh-cn", "zh-min-nan", "zh-tw", "zh-yue", "zu" };
        readonly string[] InterwikiAlphaEnFirst = new string[] { "en", "aa", "ab", "af", "ak", "als", "am", "an", "ang", "ar", "as", "ast", "av", "ay", "az", "ba", "bar", "bat-smg", "be", "be-x-old", "bg", "bh", "bi", "bm", "bn", "bo", "bpy", "br", "bs", "bug", "bxr", "ca", "cbk-zam", "cdo", "ce", "ceb", "ch", "cho", "chr", "chy", "co", "cr", "cs", "csb", "cu", "cv", "cy", "da", "de", "diq", "dv", "dz", "ee", "el", "eml", "eo", "es", "et", "eu", "fa", "ff", "fi", "fiu-vro", "fj", "fo", "fr", "frp", "fur", "fy", "ga", "gd", "gl", "glk", "gn", "got", "gu", "gv", "ha", "hak", "haw", "he", "hi", "ho", "hr", "hsb", "ht", "hu", "hy", "hz", "ia", "id", "ie", "ig", "ii", "ik", "ilo", "io", "is", "it", "iu", "ja", "jbo", "jv", "ka", "kg", "ki", "kj", "kk", "kl", "km", "kn", "ko", "kr", "ks", "ksh", "ku", "kv", "kw", "ky", "la", "lad", "lb", "lbe", "lg", "li", "lij", "lmo", "ln", "lo", "lt", "lv", "map-bms", "mg", "mh", "mi", "mk", "ml", "mn", "mo", "mr", "ms", "mt", "mus", "my", "mzn", "na", "nah", "nap", "nds", "nds-nl", "ne", "new", "ng", "nl", "nn", "no", "nov", "nrm", "nv", "ny", "oc", "om", "or", "os", "pa", "pag", "pam", "pap", "pdc", "pi", "pih", "pl", "pms", "ps", "pt", "qu", "rm", "rmy", "rn", "ro", "roa-rup", "roa-tara", "ru", "ru-sib", "rw", "sa", "sc", "scn", "sco", "sd", "se", "sg", "sh", "si", "simple", "sk", "sl", "sm", "sn", "so", "sq", "sr", "ss", "st", "su", "sv", "sw", "ta", "te", "tet", "tg", "th", "ti", "tk", "tl", "tlh", "tn", "to", "tpi", "tr", "ts", "tt", "tum", "tw", "ty", "udm", "ug", "uk", "ur", "uz", "ve", "vec", "vi", "vls", "vo", "wa", "war", "wo", "wuu", "xal", "xh", "yi", "yo", "za", "zea", "zh", "zh-classical", "zh-cn", "zh-min-nan", "zh-tw", "zh-yue", "zu" };
        List<Regex> InterWikisList = new List<Regex>();

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

            switch (Variables.LangCode)
            {
                case LangCodeEnum.pl:
                    ArticleText += strDisambig + strPersonData + strStub + strCategories + strInterwikis;
                    break;
                case LangCodeEnum.ru:
                    ArticleText += strDisambig + strPersonData + strStub + strCategories + strInterwikis;
                    break;
                case LangCodeEnum.simple:
                    ArticleText += strDisambig + strPersonData + strStub + strCategories + strInterwikis;
                    break;
                default:
                    ArticleText += strDisambig + strPersonData + strCategories + strStub + strInterwikis;
                    break;
            }

            return ArticleText;
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
            {
                ArticleText = ArticleText.Replace(defaultSort, "");
            }
            defaultSort = WikiRegexes.Defaultsort.Replace(defaultSort, "{{DEFAULTSORT:${key}}}");
            if (defaultSort != "") defaultSort += "\r\n";

            return defaultSort + ListToString(CategoryList);
        }

        private string removePersonData(ref string ArticleText)
        {
            /*
            string strPersonData = "";

            Match m = WikiRegexes.Persondata.Match(ArticleText);

            if (m.Success)
            {
                strPersonData = m.Value;
                ArticleText = ArticleText.Replace(strPersonData, "");
                strPersonData = "\r\n\r\n" + strPersonData;
            }

            return strPersonData;*/

            string strPersonData = Parsers.GetTemplate(ArticleText, "[Pp]ersondata");

            if (strPersonData != "")
            {
                ArticleText = ArticleText.Replace(strPersonData, "");
            }

            return strPersonData;
        }

        private string removeStubs(ref string ArticleText)
        {
            //if (Variables.LangCode != LangCodeEnum.en)
            //    return "";

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
