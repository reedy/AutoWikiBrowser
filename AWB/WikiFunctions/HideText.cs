using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WikiFunctions.Parse
{
    public class HideText
    {
        public HideText(){ }

        public HideText(bool HideExternalLinks, bool LeaveMetaHeadings, bool HideImages)
        {
            this.HideExternal = HideExternalLinks;
            this.bHideImages = HideImages;
            this.LeaveMetaHeadings = LeaveMetaHeadings;
        }

        bool LeaveMetaHeadings = false;
        bool bHideImages = false;
        bool HideExternal = false;

        Dictionary<string, string> NoEditList = new Dictionary<string, string>();
        Regex ImagesRegex = new Regex("\\[\\[[Ii]mage:.*?\\]\\]", RegexOptions.Singleline | RegexOptions.Compiled);
        
                
        readonly Regex NoWikiIgnoreRegex = new Regex("<!-- ?(categories|\\{\\{.*?stub\\}\\}.*?|other languages|language links|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|inter ?wiki|The below are interlanguage links\\.?) ?-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        public string Hide(string ArticleText)
        {
            NoEditList.Clear();
            string s = "";

            int i = 0;
            foreach (Match m in WikiRegexes.UnFormattedText.Matches(ArticleText))
            {
                if (LeaveMetaHeadings && NoWikiIgnoreRegex.IsMatch(m.Value))
                    continue;

                s = "⌊⌊⌊⌊" + i.ToString() + "⌋⌋⌋⌋";

                ArticleText = ArticleText.Replace(m.Value, s);
                NoEditList.Add(s, m.Value);
                i++;
            }

            if (bHideImages)
            {
                foreach (Match m in ImagesRegex.Matches(ArticleText))
                {
                    s = "⌊⌊⌊⌊" + i.ToString() + "⌋⌋⌋⌋";

                    ArticleText = ArticleText.Replace(m.Value, s);
                    NoEditList.Add(s, m.Value);
                    i++;
                }
            }

            if (HideExternal)
            {
                foreach (Match m in WikiRegexes.ExternalLinks.Matches(ArticleText))
                {
                    s = "⌊⌊⌊⌊" + i.ToString() + "⌋⌋⌋⌋";

                    ArticleText = ArticleText.Replace(m.Value, s);
                    NoEditList.Add(s, m.Value);
                    i++;
                }
                foreach (Match m in WikiRegexes.InterWikiLinks.Matches(ArticleText))
                {
                    s = "⌊⌊⌊⌊" + i.ToString() + "⌋⌋⌋⌋";

                    ArticleText = ArticleText.Replace(m.Value, s);
                    NoEditList.Add(s, m.Value);
                    i++;
                }
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


        Dictionary<string, string> HideList = new Dictionary<string, string>();
        Regex MoreRegex = new Regex("\\[\\[[Ii]mage:.*\\]\\]|[Hh]ttp://[^\\ \n]*|\\[[Hh]ttp:.*?\\]|\\{\\{[^\\}]*?\\}\\}|^:.*", RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Hides images, external links and templates
        /// </summary>
        public string HideMore(string ArticleText)
        {
            HideList.Clear();
            string s = "";

            int i = 0;
            foreach (Match m in MoreRegex.Matches(ArticleText))
            {
                s = "⌊⌊⌊⌊M" + i.ToString() + "⌋⌋⌋⌋";

                ArticleText = ArticleText.Replace(m.Value, s);
                HideList.Add(s, m.Value);
                i++;
            }

            return ArticleText;
        }

        /// <summary>
        /// Adds back hidden stuff from HideMore
        /// </summary>
        public string AddBackMore(string ArticleText)
        {
            foreach (KeyValuePair<string, string> k in HideList)
                ArticleText = ArticleText.Replace(k.Key, k.Value);

            HideList.Clear();
            return ArticleText;
        }
    }
}
