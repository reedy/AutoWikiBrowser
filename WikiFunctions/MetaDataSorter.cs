/*
Autowikibrowser
Copyright (C) 2006 Martin Richards

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

namespace WikiFunctions
{
    class MetaDataSorter
    {
        Parsers parser;
        public MetaDataSorter(Parsers p)
        {
            parser = p;
        }

        //Orders them by local language per [[m:Interwiki sorting order]]
        readonly string[] interwikiArray = { "aa", "af", "ak", "als", "am", "ang", "ab", "ar", "an", "roa-rup", "frp", "as", "ast", "gn", "av", "ay", "az", "bm", "bn", "zh-min-nan", "map-bms", "ba", "be", "bh", "bi", "bo", "bs", "br", "bg", "ca", "cv", "ceb", "cs", "ch", "ny", "sn", "tum", "cho", "co", "za", "cy", "da", "pdc", "de", "dv", "arc", "nv", "dz", "mh", "et", "el", "en", "es", "eo", "eu", "ee", "fa", "fo", "fr", "fy", "ff", "fur", "ga", "gv", "gd", "gl", "ki", "gu", "got", "xal", "ko", "ha", "haw", "hy", "hi", "ho", "hr", "io", "ig", "ilo", "id", "ia", "ie", "iu", "ik", "os", "xh", "zu", "is", "it", "he", "jv", "kl", "kn", "kr", "ka", "ks", "csb", "kk", "kw", "rw", "ky", "rn", "sw", "kv", "kg", "ht", "kj", "ku", "lo", "lad", "la", "lv", "lb", "lt", "lij", "li", "ln", "jbo", "lg", "lmo", "hu", "mk", "mg", "ml", "mt", "mi", "mr", "ms", "mo", "mn", "mus", "my", "nah", "na", "fj", "nl", "nds-nl", "cr", "ne", "ja", "nap", "ce", "pih", "no", "nn", "nrm", "oc", "or", "om", "ng", "hz", "ug", "pa", "pi", "pam", "pap", "ps", "km", "nds", "pl", "pms", "pt", "ty", "ksh", "ro", "rmy", "rm", "qu", "ru", "war", "se", "sm", "sa", "sg", "sc", "sco", "st", "tn", "sq", "scn", "si", "simple", "sd", "ss", "sk", "sl", "so", "sr", "sh", "su", "fi", "sv", "tl", "ta", "tt", "te", "tet", "th", "vi", "ti", "tg", "tpi", "to", "chr", "chy", "ve", "tr", "tk", "tw", "udm", "bug", "uk", "ur", "uz", "vec", "vo", "fiu-vro", "wa", "vls", "wo", "ts", "ii", "yi", "yo", "zh-yue", "bat-smg", "zh", "zh-tw", "zh-cn" };
        const string interPattern = "\\[\\[(nds-nl|rmy|lij|bat-smg|map-bms|ksh|pdc|vls|nrm|frp|zh-yue|tet|xal|pap|tokipona|minnan|aa|af|ak|als|am|ang|ab|ar|an|arc|roa-rup|as|ast|gn|av|ay|az|bm|bn|zh-min-nan|ba|be|bh|bi|bo|bs|br|bg|ca|cv|ceb|cs|ch|ny|sn|tum|cho|co|za|cy|da|de|dv|nv|dz|mh|et|el|en|es|eo|eu|ee|fa|fo|fr|fy|ff|fur|ga|gv|gd|gl|ki|gu|got|ko|ha|haw|hy|hi|ho|hr|io|ig|ilo|id|ia|ie|iu|ik|os|xh|zu|is|it|he|jv|kl|kn|kr|ka|ks|csb|kk|kw|rw|ky|rn|sw|kv|kg|ht|kj|ku|lo|lad|la|lv|lb|lt|li|ln|jbo|lg|lmo|hu|mk|mg|ml|mt|mi|mr|ms|mo|mn|mus|my|nah|na|nb|fj|nl|cr|ne|ja|nap|ce|pih|no|nn|oc|or|om|ng|hz|ug|pa|pi|pam|ps|km|nds|pl|pms|pt|ty|ro|rm|qu|ru|war|se|sm|sa|sg|sc|sco|st|tn|sq|scn|si|simple|sd|ss|sk|sl|so|sr|sh|su|fi|sv|tl|ta|tt|te|th|vi|ti|tg|tpi|to|chr|chy|ve|tr|tk|tw|udm|bug|uk|ur|uz|vec|vo|fiu-vro|wa|wo|ts|ii|yi|yo|zh|zh-tw|zh-cn):.*?\\]\\]";

        internal string Sort(string ArticleText, string articleTitle)
        {
            ArticleText = Regex.Replace(ArticleText, "<!-- ?\\[\\[en:.*?\\]\\] ?-->", "");

            string strPersonData = removePersonData(ref ArticleText);
            string strDisambig = removeDisambig(ref ArticleText);
            string strCategories = removeCats(ref ArticleText, articleTitle);
            string strInterwikis = interwikis(ref ArticleText);
            string strStub = removeStubs(ref ArticleText);

            //filter out excess white space and remove "----" from end of article
            ArticleText = Parsers.RemoveWhiteSpace(ArticleText);

            ArticleText += strPersonData + strDisambig + strCategories + strInterwikis + strStub;

            return ArticleText;
        }

        private string removeCats(ref string ArticleText, string articleTitle)
        {
            ArrayList CatArray = new ArrayList();

            foreach (Match m in Regex.Matches(ArticleText, "<!-- ? ?\\[\\[" + Variables.CategoryNS + "[^\\]\\]]*\\]\\].*?-->|\\[\\[" + Variables.CategoryNS + "[^\\]\\]]*\\]\\]( {0,4}<!--.*?-->)?"))
            {
                string x = m.ToString();
                //add to array, replace underscores with spaces, ignore
                if (!Regex.IsMatch(x, "\\[\\[Category:(Pages|Categories|Articles) for deletion\\]\\]"))
                {
                    ArticleText = ArticleText.Replace(x, "");
                    CatArray.Add(x.Replace("_", " "));
                }
            }

            if (parser.addCatKey)
                CatArray = catKeyer(CatArray, articleTitle);

            if (Regex.IsMatch(ArticleText, "<!-- ?categories ?-->", RegexOptions.IgnoreCase))
            {
                string catComment = Regex.Match(ArticleText, "<!-- ?categories ?-->", RegexOptions.IgnoreCase).ToString();
                ArticleText = ArticleText.Replace(catComment, "");
                CatArray.Insert(0, catComment);
            }

            return ArrayToString(CatArray);
        }

        private string removePersonData(ref string ArticleText)
        {
            string strPersonData = "";

            string metaPattern = "(\\{\\{ ?[Pp]ersondata.*?\\}\\})";
            if (Regex.IsMatch(ArticleText, metaPattern, RegexOptions.Singleline))
            {
                strPersonData = Regex.Match(ArticleText, metaPattern, RegexOptions.Singleline).ToString();
                ArticleText = ArticleText.Replace(strPersonData, "");
                strPersonData = "\r\n\r\n" + strPersonData;
            }

            return strPersonData;
        }

        private string removeStubs(ref string ArticleText)
        {
            ArrayList StubArray = new ArrayList();
            string stubPattern = "<!-- ?\\{\\{.*?stub\\}\\}.*?-->|:?\\{\\{.*?stub\\}\\}";
            if (Regex.IsMatch(ArticleText, stubPattern))
            {
                foreach (Match m in Regex.Matches(ArticleText, stubPattern))
                {
                    string x = m.ToString();
                    if (!((Regex.IsMatch(x, "[Ss]ect") || (Regex.IsMatch(x, "tl\\|")))))
                    {
                        StubArray.Add(x);
                        //remove old stub
                        ArticleText = ArticleText.Replace(x, "");
                    }
                }
            }

            if (StubArray.Count != 0)
                return "\r\n" + ArrayToString(StubArray);
            else
                return "";
        }

        private string removeDisambig(ref string ArticleText)
        {
            string strDisambig = "";
            if (Regex.IsMatch(ArticleText, "\\{\\{(Disambig|2CC|3CC|4CC)\\}\\}", RegexOptions.IgnoreCase))
            {
                strDisambig = Regex.Match(ArticleText, "\\{\\{(Disambig|2CC|3CC|4CC)\\}\\}", RegexOptions.IgnoreCase).ToString();
                ArticleText = ArticleText.Replace(strDisambig, "");
                strDisambig = "\r\n\r\n" + strDisambig;
            }

            return strDisambig;
        }

        private ArrayList removeLinkFAs(ref string ArticleText)
        {
            ArrayList LinkFAArrayList = new ArrayList();
            foreach (Match m in Regex.Matches(ArticleText, "(\\{\\{[Ll]ink FA\\|)([a-z\\-]{2,10})[^\\}\\}]*\\}\\}"))
            {
                string x = m.ToString();
                LinkFAArrayList.Add(x);
                //remove old LinkFA
                ArticleText = ArticleText.Replace(x, "");
            }

            return LinkFAArrayList;
        }

        private string interwikis(ref string ArticleText)
        {
            string interwikis = "";

            interwikis = ArrayToString(removeLinkFAs(ref ArticleText)) + ArrayToString(removeInterWikis(ref ArticleText));

            return interwikis;
        }

        private ArrayList removeInterWikis(ref string ArticleText)
        {
            ArrayList InterWikiArray = new ArrayList();

            string strInterLangRegex = "<!-- ?(other languages|language links|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|inter ?wiki|The below are interlanguage links\\.?) ?-->";
            if (Regex.IsMatch(ArticleText, strInterLangRegex, RegexOptions.IgnoreCase))
            {
                string interWikiComment = "";
                interWikiComment = Regex.Match(ArticleText, strInterLangRegex, RegexOptions.IgnoreCase).ToString();
                ArticleText = ArticleText.Replace(interWikiComment, "");
                InterWikiArray.Add(interWikiComment);
            }

            if (parser.sortInterwikiOrder)
            {
                foreach (string s in interwikiArray)
                {
                    Regex rege = new Regex("\\[\\[" + s + ":.*?\\]\\]");

                    //use foreach as some articles have multiple links to same wiki
                    string x = "";
                    foreach (Match m in rege.Matches(ArticleText))
                    {
                        x = m.ToString();
                        ArticleText = rege.Replace(ArticleText, "", 1);
                        x = HttpUtility.HtmlDecode(x).Replace("_", " ");
                        InterWikiArray.Add(x);
                    }
                }
            }
            else
            {
                //keeps existing order
                if (Regex.IsMatch(ArticleText, interPattern))
                {
                    foreach (Match m in Regex.Matches(ArticleText, interPattern))
                    {
                        string x = m.ToString();
                        ArticleText = ArticleText.Replace(x, "");
                        x = HttpUtility.HtmlDecode(x).Replace("_", " ");
                        InterWikiArray.Add(x);

                    }
                }
            }

            return InterWikiArray;
        }

        private string ArrayToString(ArrayList items)
        {//remove duplicates, and return arraylist as string.

            if (items.Count == 0)
                return "";

            string List = "";
            ArrayList uniqueItems = new ArrayList();

            //remove duplicates
            for (int i = 0; i < items.Count; i++)
            {
                if (!uniqueItems.Contains(items[i]))
                    uniqueItems.Add(items[i]);
            }

            //add to string
            foreach (string s in uniqueItems)
            {
                List += "\r\n" + s;
            }

            return "\r\n" + List;
        }

        private ArrayList catKeyer(ArrayList arrayList, string strName)
        {
            // make key
            strName = Regex.Replace(strName, "\\(.*?\\)$", "").Trim();
            if (strName.Contains(" "))
            {
                int intLast = strName.LastIndexOf(" ") + 1;
                string strLastName = strName.Substring(intLast);
                strName = strName.Remove(intLast);
                strName = strLastName + ", " + strName.Trim();
            }

            //add key to cats that need it
            ArrayList newCats = new ArrayList();
            foreach (string s in arrayList)
            {
                string z = s;
                if (!z.Contains("|"))
                {
                    z = z.Replace("]]", "|" + strName + "]]");
                }
                newCats.Add(z);
            }
            return newCats;
        }
    }
}
