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

        readonly bool LeaveMetaHeadings, HideImages, HideExternalLinks;

        readonly List<HideObject> HiddenTokens = new List<HideObject>();
        static readonly Regex NoWikiIgnoreRegex = new Regex("<!-- ?(cat(egories)?|\\{\\{.*?stub\\}\\}.*?|other languages?|language links?|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|inter ?wikis?|The below are interlanguage links\\.?) ?-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="ArticleText"></param>
        private void Replace(IEnumerable matches, ref string ArticleText)
        {
            foreach (Match m in matches)
            {
                string s = "⌊⌊⌊⌊" + HiddenTokens.Count + "⌋⌋⌋⌋";
                ArticleText = ArticleText.Replace(m.Value, s);
                HiddenTokens.Add(new HideObject(s, m.Value));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText"></param>
        /// <returns></returns>
        public string Hide(string ArticleText)
        {
            HiddenTokens.Clear();

            Replace(WikiRegexes.Source.Matches(ArticleText), ref ArticleText);

            foreach (Match m in WikiRegexes.UnFormattedText.Matches(ArticleText))
            {
                if (LeaveMetaHeadings && NoWikiIgnoreRegex.IsMatch(m.Value))
                    continue;

                string s = "⌊⌊⌊⌊" + HiddenTokens.Count + "⌋⌋⌋⌋";

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText"></param>
        /// <returns></returns>
        public string AddBack(string ArticleText)
        {
            HiddenTokens.Reverse();

            foreach (HideObject k in HiddenTokens)
                ArticleText = ArticleText.Replace(k.code, k.text);

            HiddenTokens.Clear();
            return ArticleText;
        }

        #region Separate hiding of unformatted text
        readonly List<HideObject> HiddenUnformattedText = new List<HideObject>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText"></param>
        /// <returns></returns>
        public string HideUnformatted(string ArticleText)
        {
            HiddenUnformattedText.Clear();

            int i = 0;
            foreach (Match m in WikiRegexes.UnFormattedText.Matches(ArticleText))
            {
                string s = "⌊⌊⌊⌊" + i + "⌋⌋⌋⌋";

                ArticleText = ArticleText.Replace(m.Value, s);
                HiddenUnformattedText.Add(new HideObject(s, m.Value));
                i++;
            }

            return ArticleText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText"></param>
        /// <returns></returns>
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
        readonly List<HideObject> MoreHide = new List<HideObject>(32);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="ArticleText"></param>
        private void ReplaceMore(ICollection matches, ref string ArticleText)
        {
            StringBuilder sb = new StringBuilder((int)(ArticleText.Length * 1.1));
            int pos = 0;

            foreach (Match m in matches)
            {
                sb.Append(ArticleText, pos, m.Index - pos);
                string s = "⌊⌊⌊⌊M" + MoreHide.Count + "⌋⌋⌋⌋";
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
        public string HideMore(string ArticleText, bool HideOnlyTargetOfWikilink)
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

            // This hides internal wikilinks (with or without pipe) with extra word character(s) e.g. [[link]]age, which need hiding even if hiding for typo fixing 
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Improve_HideText.HideMore.28.29
            // place this as first wikilink rule as otherwise WikiLinksOnly will grab link without extra word character(s)
            ReplaceMore(WikiRegexes.WikiLinksOnlyPlusWord.Matches(ArticleText), ref ArticleText);

            // if HideOnlyTargetOfWikilink is not set, pipes of links e.g.  [[target|pipe]] will be hidden
            // if set then don't mask the pipe of a link so that typo fixing can be done on it
            if (!HideOnlyTargetOfWikilink)
            {
                ReplaceMore(WikiRegexes.WikiLinksOnly.Matches(ArticleText), ref ArticleText);

                ReplaceMore(WikiRegexes.SimpleWikiLink.Matches(ArticleText), ref ArticleText);
            }

            ReplaceMore(WikiRegexes.Cites.Matches(ArticleText), ref ArticleText);

            ReplaceMore(WikiRegexes.Refs.Matches(ArticleText), ref ArticleText);

            // this hides only the target of a link, leaving the pipe exposed
            ReplaceMore(WikiRegexes.WikiLink.Matches(ArticleText), ref ArticleText);

            //TODO: replace with gallery-only regex, all normal images should be hidden by now as simple wikilinks
            ReplaceMore(WikiRegexes.Images.Matches(ArticleText), ref ArticleText);

            // hide untemplated quotes between some form of quotation masks (most particularly for typo fixing)
            ReplaceMore(WikiRegexes.UntemplatedQuotes.Matches(ArticleText), ref ArticleText);

            return ArticleText;
        }

        /// <summary>
        /// Hides images, external links, templates, headings
        /// </summary>
        public string HideMore(string ArticleText)
        {
            return HideMore(ArticleText, false);
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
