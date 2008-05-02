using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace WikiFunctions.Parse
{
    /// <summary>
    /// DOCUMENT ME PLEASE! What do I do?!
    /// </summary>
    public class HideText
    {
        public HideText() { }

        public HideText(bool HideExternalLinks, bool LeaveMetaHeadings, bool HideImages)
        {
            this.HideExternal = HideExternalLinks;
            this.bHideImages = HideImages;
            this.LeaveMetaHeadings = LeaveMetaHeadings;
        }

        bool LeaveMetaHeadings;
        bool bHideImages;
        bool HideExternal;

        List<HideObject> NoEditList = new List<HideObject>();
        List<HideObject> NoUnformatted = new List<HideObject>();
        readonly Regex NoWikiIgnoreRegex = new Regex("<!-- ?(cat(egories)?|\\{\\{.*?stub\\}\\}.*?|other languages?|language links?|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|inter ?wikis?|The below are interlanguage links\\.?) ?-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        void Replace(MatchCollection matches, ref string ArticleText)
        {
            string s;
            foreach (Match m in matches)
            {
                s = "⌊⌊⌊⌊" + NoEditList.Count.ToString() + "⌋⌋⌋⌋";
                ArticleText = ArticleText.Replace(m.Value, s);
                NoEditList.Add(new HideObject(s, m.Value));
            }
        }

        public string Hide(string ArticleText)
        {
            NoEditList.Clear();
            string s = "";

            Replace(WikiRegexes.Source.Matches(ArticleText), ref ArticleText);

            foreach (Match m in WikiRegexes.UnFormattedText.Matches(ArticleText))
            {
                if (LeaveMetaHeadings && NoWikiIgnoreRegex.IsMatch(m.Value))
                    continue;

                s = "⌊⌊⌊⌊" + NoEditList.Count.ToString() + "⌋⌋⌋⌋";

                ArticleText = ArticleText.Replace(m.Value, s);
                NoEditList.Add(new HideObject(s, m.Value));
            }

            if (bHideImages)
            {
                Replace(WikiRegexes.Images.Matches(ArticleText), ref ArticleText);
            }

            if (HideExternal)
            {
                Replace(WikiRegexes.ExternalLinks.Matches(ArticleText), ref ArticleText);
                Replace(WikiRegexes.InterWikiLinks.Matches(ArticleText), ref ArticleText);
            }

            return ArticleText;
        }

        public string AddBack(string ArticleText)
        {
            NoEditList.Reverse();

            foreach (HideObject k in NoEditList)
                ArticleText = ArticleText.Replace(k.code, k.text);

            NoEditList.Clear();
            return ArticleText;
        }


        List<HideObject> MoreHide = new List<HideObject>(32);

        void ReplaceMore(ICollection matches, ref string ArticleText)
        {
            string s = "";

            StringBuilder sb = new StringBuilder((int)(ArticleText.Length * 1.1));
            int pos = 0;

            foreach (Match m in matches)
            {
                sb.Append(ArticleText, pos, m.Index - pos);
                s = "⌊⌊⌊⌊M" + MoreHide.Count.ToString() + "⌋⌋⌋⌋";
                sb.Append(s);
                pos = m.Index + m.Value.Length;
                MoreHide.Add(new HideObject(s, m.Value));
            }

            sb.Append(ArticleText, pos, ArticleText.Length - pos);

            ArticleText = sb.ToString();
        }

        static readonly Regex HiddenMoreRegex = new Regex("⌊⌊⌊⌊M(\\d*)⌋⌋⌋⌋", RegexOptions.Compiled);

        /// <summary>
        /// Hides images, external links, templates, headings
        /// </summary>
        public string HideMore(string ArticleText)
        {
            MoreHide.Clear();

            string ArticleTextBefore;
            do
            { // hide nested templates
                ArticleTextBefore = ArticleText;
                List<Match> matches = Parsers.GetTemplates(ArticleText, Parsers.EveryTemplate);
                ReplaceMore(matches, ref ArticleText);
            }
            while (!ArticleTextBefore.Equals(ArticleText));


            ReplaceMore(WikiRegexes.Blockquote.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.Images.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.Source.Matches(ArticleText), ref ArticleText);

            if (HideExternal) ReplaceMore(WikiRegexes.ExternalLinks.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.Headings.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.IndentedText.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.UnFormattedText.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.SimpleWikiLink.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.Cites.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.Refs.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.WikiLink.Matches(ArticleText), ref ArticleText);

            return ArticleText;
        }
        
        /// <summary>
        /// Adds back hidden stuff from HideMore
        /// </summary>
        public string AddBackMore(string ArticleText)
        {
            StringBuilder sb;

            MatchCollection mc;

            while ((mc = HiddenMoreRegex.Matches(ArticleText)).Count > 0)
            {
                sb = new StringBuilder(ArticleText.Length * 2);
                int pos = 0;

                foreach (Match m in mc)
                {
                    sb.Append(ArticleText, pos, m.Index - pos);
                    sb.Append(MoreHide[int.Parse(m.Groups[1].Value)].text);
                    pos = m.Index + m.Value.Length;
                }

                sb.Append(ArticleText, pos, ArticleText.Length - pos);

                ArticleText = sb.ToString();
            }

            MoreHide.Clear();
            return ArticleText;
        }

        public string HideUnformatted(string ArticleText)
        {
            NoUnformatted.Clear();
            string s = "";

            int i = 0;
            foreach (Match m in WikiRegexes.UnFormattedText.Matches(ArticleText))
            {
                s = "⌊⌊⌊⌊" + i.ToString() + "⌋⌋⌋⌋";

                ArticleText = ArticleText.Replace(m.Value, s);
                NoUnformatted.Add(new HideObject(s, m.Value));
                i++;
            }

            return ArticleText;
        }

        public string AddBackUnformatted(string ArticleText)
        {
            NoUnformatted.Reverse();

            foreach (HideObject k in NoUnformatted)
                ArticleText = ArticleText.Replace(k.code, k.text);

            NoUnformatted.Clear();
            return ArticleText;
        }
    }
    struct HideObject
    {
        public HideObject(string code, string text)
        {
            this.code = code;
            this.text = text;
        }

        public string code;
        public string text;

        public override string ToString()
        {
            return code + " --> " + text;
        }
    }

}
