using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace WikiFunctions.Parse
{
    /// <summary>
    /// This class provides functions for 'hiding' certain syntax by replacing it with unique tokens
    /// and then adding it back after an operation was performed on text
    /// </summary>
    public sealed class HideText
    {
        public HideText() { }

        public HideText(bool hideExternalLinks, bool leaveMetaHeadings, bool hideImages)
        {
            HideExternalLinks = hideExternalLinks;
            LeaveMetaHeadings = leaveMetaHeadings;
            HideImages = hideImages;
        }

        bool LeaveMetaHeadings;
        bool HideImages;
        bool HideExternalLinks;

        List<HideObject> HiddenTokens = new List<HideObject>();
        static readonly Regex NoWikiIgnoreRegex = new Regex("<!-- ?(cat(egories)?|\\{\\{.*?stub\\}\\}.*?|other languages?|language links?|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|inter ?wikis?|The below are interlanguage links\\.?) ?-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private void Replace(IEnumerable matches, ref string ArticleText)
        {
            string s;
            foreach (Match m in matches)
            {
                s = "⌊⌊⌊⌊" + HiddenTokens.Count.ToString() + "⌋⌋⌋⌋";
                ArticleText = ArticleText.Replace(m.Value, s);
                HiddenTokens.Add(new HideObject(s, m.Value));
            }
        }

        public string Hide(string ArticleText)
        {
            HiddenTokens.Clear();
            string s = "";

            Replace(WikiRegexes.Source.Matches(ArticleText), ref ArticleText);

            foreach (Match m in WikiRegexes.UnFormattedText.Matches(ArticleText))
            {
                if (LeaveMetaHeadings && NoWikiIgnoreRegex.IsMatch(m.Value))
                    continue;

                s = "⌊⌊⌊⌊" + HiddenTokens.Count.ToString() + "⌋⌋⌋⌋";

                ArticleText = ArticleText.Replace(m.Value, s);
                HiddenTokens.Add(new HideObject(s, m.Value));
            }

            if (HideImages)
            {
                Replace(WikiRegexes.Images.Matches(ArticleText), ref ArticleText);
            }

            if (HideExternalLinks)
            {
                Replace(WikiRegexes.ExternalLinks.Matches(ArticleText), ref ArticleText);
                List<Match> matches = new List<Match>();
                foreach (Match m in WikiRegexes.PossibleInterwikis.Matches(ArticleText))
                {
                    if (SiteMatrix.Languages.Contains(m.Groups[1].Value.ToLower()))
                        matches.Add(m);
                }
                Replace(matches, ref ArticleText);
            }

            return ArticleText;
        }

        public string AddBack(string ArticleText)
        {
            HiddenTokens.Reverse();

            foreach (HideObject k in HiddenTokens)
                ArticleText = ArticleText.Replace(k.code, k.text);

            HiddenTokens.Clear();
            return ArticleText;
        }

        #region Separate hiding of unformatted text
        List<HideObject> HiddenUnformattedText = new List<HideObject>();

        public string HideUnformatted(string ArticleText)
        {
            HiddenUnformattedText.Clear();
            string s = "";

            int i = 0;
            foreach (Match m in WikiRegexes.UnFormattedText.Matches(ArticleText))
            {
                s = "⌊⌊⌊⌊" + i.ToString() + "⌋⌋⌋⌋";

                ArticleText = ArticleText.Replace(m.Value, s);
                HiddenUnformattedText.Add(new HideObject(s, m.Value));
                i++;
            }

            return ArticleText;
        }

        public string AddBackUnformatted(string ArticleText)
        {
            HiddenUnformattedText.Reverse();

            foreach (HideObject k in HiddenUnformattedText)
                ArticleText = ArticleText.Replace(k.code, k.text);

            HiddenUnformattedText.Clear();
            return ArticleText;
        }
        #endregion

        #region More thorough hiding
        List<HideObject> MoreHide = new List<HideObject>(32);

        private void ReplaceMore(ICollection matches, ref string ArticleText)
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

            ReplaceMore(WikiRegexes.Source.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.Code.Matches(ArticleText), ref ArticleText);

            if (HideExternalLinks) ReplaceMore(WikiRegexes.ExternalLinks.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.Headings.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.UnFormattedText.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.IndentedText.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.WikiLinksOnly.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.SimpleWikiLink.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.Cites.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.Refs.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.WikiLink.Matches(ArticleText), ref ArticleText);

            //TODO: replace with gallery-only regex, all normal images should be hidden by now as simple wikilinks
            ReplaceMore(WikiRegexes.Images.Matches(ArticleText), ref ArticleText);

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
        #endregion

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
