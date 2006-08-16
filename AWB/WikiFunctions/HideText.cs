using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WikiFunctions
{
    public class HideText
    {
        public HideText(bool HideExternalLinks, bool LeaveMetaHeadings)
        {
            string Regex = "<nowiki>.*?</nowiki>|<math>.*?</math>|<!--.*?-->|\\[\\[[Ii]mage:.*?\\]\\]";

            if (HideExternalLinks)
                Regex += "|[Hh]ttp://[^\\ ]*|\\[[Hh]ttp:.*?\\]|\\[\\[([a-z]{2,3}|simple|fiu-vro|minnan|roa-rup|tokipona|zh-min-nan):.*\\]\\]";

            NoLinksRegex = new Regex(Regex, RegexOptions.Compiled | RegexOptions.Singleline);
        }

        bool LeaveMetaHeadings = false;

        Dictionary<string, string> NoEditList = new Dictionary<string, string>();
        Regex NoLinksRegex = new Regex("", RegexOptions.Singleline | RegexOptions.Compiled);
        readonly Regex NoWikiIgnoreRegex = new Regex("<!-- ?(categories|\\{\\{.*?stub\\}\\}.*?|other languages|language links|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|inter ?wiki|The below are interlanguage links\\.?) ?-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        public string Hide(string articleText)
        {
            NoEditList.Clear();
            string s = "";

            int i = 0;
            foreach (Match m in NoLinksRegex.Matches(articleText))
            {
                if (LeaveMetaHeadings && NoWikiIgnoreRegex.IsMatch(m.Value))
                    continue;

                s = "⌊⌊⌊⌊" + i.ToString() + "⌋⌋⌋⌋";

                articleText = articleText.Replace(m.Value, s);
                NoEditList.Add(s, m.Value);
                i++;
            }

            return articleText;

        }

        public string AddBack(string articleText)
        {
            foreach (KeyValuePair<string, string> k in NoEditList)
                articleText = articleText.Replace(k.Key, k.Value);

            NoEditList.Clear();
            return articleText;
        }  

    }
}
