using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WikiFunctions.Parse
{
    public class HideText
    {
        public HideText(bool HideExternalLinks, bool LeaveMetaHeadings, bool HideImages)
        {
            string Regex = "<nowiki>.*?</nowiki>|<pre>.*?</pre>|<math>.*?</math>|<!--.*?-->";

            if (HideImages)
                Regex += "|\\[\\[[Ii]mage:.*?\\]\\]";

            if (HideExternalLinks)
                Regex += "|[Hh]ttp://[^\\ ]*|\\[[Hh]ttp:.*?\\]|\\[\\[([a-z]{2,3}|simple|fiu-vro|minnan|roa-rup|tokipona|zh-min-nan):.*\\]\\]";

            NoLinksRegex = new Regex(Regex, RegexOptions.Compiled | RegexOptions.Singleline);

            this.LeaveMetaHeadings = LeaveMetaHeadings;
        }

        bool LeaveMetaHeadings = false;

        Dictionary<string, string> NoEditList = new Dictionary<string, string>();
        Regex NoLinksRegex = new Regex("", RegexOptions.Singleline | RegexOptions.Compiled);
        readonly Regex NoWikiIgnoreRegex = new Regex("<!-- ?(categories|\\{\\{.*?stub\\}\\}.*?|other languages|language links|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|inter ?wiki|The below are interlanguage links\\.?) ?-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        public string Hide(string ArticleText)
        {
            NoEditList.Clear();
            string s = "";

            int i = 0;
            foreach (Match m in NoLinksRegex.Matches(ArticleText))
            {
                if (LeaveMetaHeadings && NoWikiIgnoreRegex.IsMatch(m.Value))
                    continue;

                s = "⌊⌊⌊⌊" + i.ToString() + "⌋⌋⌋⌋";

                ArticleText = ArticleText.Replace(m.Value, s);
                NoEditList.Add(s, m.Value);
                i++;
            }

            return ArticleText;

        }

        public string AddBack(string ArticleText)
        {
            foreach (KeyValuePair<string, string> k in NoEditList)
                ArticleText = ArticleText.Replace(k.Key, k.Value);

            NoEditList.Clear();
            return ArticleText;
        }  

    }
}
