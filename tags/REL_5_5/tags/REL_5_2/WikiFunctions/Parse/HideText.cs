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
        /// <summary>
        /// 
        /// </summary>
        public HideText() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hideExternalLinks"></param>
        /// <param name="leaveMetaHeadings"></param>
        /// <param name="hideImages"></param>
        public HideText(bool hideExternalLinks, bool leaveMetaHeadings, bool hideImages)
        {
            HideExternalLinks = hideExternalLinks;
            LeaveMetaHeadings = leaveMetaHeadings;
            HideImages = hideImages;
        }

        private readonly bool LeaveMetaHeadings, HideImages, HideExternalLinks;

        private static readonly Regex NoWikiIgnoreRegex = new Regex("<!-- ?(cat(egories)?|\\{\\{.*?stub\\}\\}.*?|other languages?|language links?|inter ?(language|wiki)? ?links|inter ?wiki ?language ?links|inter ?wikis?|The below are interlanguage links\\.?) ?-->", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #region Hide
        private readonly List<HideObject> HiddenTokens = new List<HideObject>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="articleText">The wiki text of the article.</param>
        private void Replace(IEnumerable matches, ref string articleText)
        {            
            Replace(matches, ref articleText, HiddenTokens);
        }
        
         /// <summary>
        /// 
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="articleText">The wiki text of the article.</param>
        private void Replace(IEnumerable matches, ref string articleText, List<HideObject> Tokens)
        {
            StringBuilder sb = new StringBuilder((int)(articleText.Length * 1.1));
            int pos = 0;

            foreach (Match m in matches)
            {
                sb.Append(articleText, pos, m.Index - pos);
                string s = "⌊⌊⌊⌊" + Tokens.Count + "⌋⌋⌋⌋";
                sb.Append(s);
                pos = m.Index + m.Value.Length;
                Tokens.Add(new HideObject(s, m.Value));
            }

            sb.Append(articleText, pos, articleText.Length - pos);

            articleText = sb.ToString();
        }

        /// <summary>
        /// Hides Unformatted text (nowiki, pre, math, html comments, timelines), source tags
        /// Also hides images and external links if set on call to constructor
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public string Hide(string articleText)
        {
            HiddenTokens.Clear();

            Replace(WikiRegexes.Source.Matches(articleText), ref articleText);

            var matches = new List<Match>();
            foreach (Match m in WikiRegexes.UnformattedText.Matches(articleText))
            {
                if (LeaveMetaHeadings && NoWikiIgnoreRegex.IsMatch(m.Value))
                    continue;

                matches.Add(m);
            }
            Replace(matches, ref articleText);

            if (HideExternalLinks)
            {
                Replace(WikiRegexes.ExternalLinks.Matches(articleText), ref articleText);

                List<Match> matches2 = new List<Match>();
                foreach (Match m in WikiRegexes.PossibleInterwikis.Matches(articleText))
                {
                    if (SiteMatrix.Languages.Contains(m.Groups[1].Value.ToLower()))
                        matches2.Add(m);
                }
                Replace(matches2, ref articleText);
            }
            
            if (HideImages)
            {
                Replace(WikiRegexes.Images.Matches(articleText), ref articleText);
            }

            return articleText;
        }

        private static readonly Regex HiddenRegex = new Regex("⌊⌊⌊⌊(\\d*)⌋⌋⌋⌋", RegexOptions.Compiled);

        /// <summary>
        /// Adds stuff removed by Hide back
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public string AddBack(string articleText)
        {
            return AddBack(articleText, HiddenTokens);
        }
        
        /// <summary>
        /// Adds stuff removed by Hide back
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        private string AddBack(string articleText, List<HideObject> Tokens)
        {
            MatchCollection mc;

            while ((mc = HiddenRegex.Matches(articleText)).Count > 0)
            {
                StringBuilder sb = new StringBuilder(articleText.Length * 2);
                int pos = 0;

                foreach (Match m in mc)
                {
                    sb.Append(articleText, pos, m.Index - pos);
                    sb.Append(Tokens[int.Parse(m.Groups[1].Value)].Text);
                    pos = m.Index + m.Value.Length;
                }

                sb.Append(articleText, pos, articleText.Length - pos);

                articleText = sb.ToString();
            }

            Tokens.Clear();
            return articleText;
        }
        #endregion

        #region Unformatted Text
        private readonly List<HideObject> HiddenUnformattedText = new List<HideObject>();

        /// <summary>
        /// Hides unformatted text regions: nowiki, pre, math, html comments, timelines
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public string HideUnformatted(string articleText)
        {
            HiddenUnformattedText.Clear();
            
            Replace(WikiRegexes.UnformattedText.Matches(articleText), ref articleText, HiddenUnformattedText);

            return articleText;
        }

        /// <summary>
        /// Adds things removed by HideUnformatted back
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public string AddBackUnformatted(string articleText)
        {
            return AddBack(articleText, HiddenUnformattedText);
        }
        #endregion

        #region More thorough hiding
        private readonly List<HideObject> MoreHide = new List<HideObject>(32);

        /// <summary>
        /// Replaces back hidden images, external links, templates, headings etc.
        /// (ie what was hidden by HideMore)
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="articleText">The wiki text of the article.</param>
        private void ReplaceMore(ICollection matches, ref string articleText)
        {
            StringBuilder sb = new StringBuilder((int)(articleText.Length * 1.1));
            int pos = 0;

            foreach (Match m in matches)
            {
                sb.Append(articleText, pos, m.Index - pos);
                string s = "⌊⌊⌊⌊M" + MoreHide.Count + "⌋⌋⌋⌋";
                sb.Append(s);
                pos = m.Index + m.Value.Length;
                MoreHide.Add(new HideObject(s, m.Value));
            }

            sb.Append(articleText, pos, articleText.Length - pos);

            articleText = sb.ToString();
        }

        /// <summary>
        /// Hides images, external links, templates, headings
        /// </summary>
        /// <param name="articleText">the text of the article</param>
        public string HideMore(string articleText)
        {
            return HideMore(articleText, false, true, true);
        }

        /// <summary>
        /// Hides images, external links, templates, headings
        /// </summary>
        /// <param name="articleText">the text of the article</param>
        /// <param name="hideOnlyTargetOfWikilink">whether to hide only the target of a wikilink (so that fixes such as typo corrections may be applied to the piped part of the link)</param>
        public string HideMore(string articleText, bool hideOnlyTargetOfWikilink)
        {
            return HideMore(articleText, hideOnlyTargetOfWikilink, true, true);
        }
        
        /// <summary>
        /// Hides images, external links, templates, headings and italics
        /// </summary>
        /// <param name="articleText">the text of the article</param>
        /// <param name="hideOnlyTargetOfWikilink">whether to hide only the target of a wikilink (so that fixes such as typo corrections may be applied to the piped part of the link)</param>
        /// <param name="hideWikiLinks">whether to hide all wikilinks including those with words attached outside the link</param>
        public string HideMore(string articleText, bool hideOnlyTargetOfWikilink, bool hideWikiLinks)
        {
            return HideMore(articleText, hideOnlyTargetOfWikilink, hideWikiLinks, true);
        }

        /// <summary>
        /// Hides images, external links, templates, headings
        /// </summary>
        /// <param name="articleText">the text of the article</param>
        /// <param name="hideOnlyTargetOfWikilink">whether to hide only the target of a wikilink (so that fixes such as typo corrections may be applied to the piped part of the link)</param>
        /// <param name="hideWikiLinks">whether to hide all wikilinks including those with words attached outside the link</param>
        /// <param name="hideItalics">whether to hide italics</param>
        /// <returns>the modified article text</returns>
        public string HideMore(string articleText, bool hideOnlyTargetOfWikilink, bool hideWikiLinks, bool hideItalics)
        {
            MoreHide.Clear();

            ReplaceMore(WikiRegexes.NestedTemplates.Matches(articleText), ref articleText);

            ReplaceMore(WikiRegexes.Blockquote.Matches(articleText), ref articleText);

            ReplaceMore(WikiRegexes.Source.Matches(articleText), ref articleText);

            ReplaceMore(WikiRegexes.Code.Matches(articleText), ref articleText);

            ReplaceMore(WikiRegexes.Noinclude.Matches(articleText), ref articleText);

            ReplaceMore(WikiRegexes.Includeonly.Matches(articleText), ref articleText);

            if (HideExternalLinks)
                ReplaceMore(WikiRegexes.ExternalLinks.Matches(articleText), ref articleText);

            ReplaceMore(WikiRegexes.Headings.Matches(articleText), ref articleText);

            ReplaceMore(WikiRegexes.UnformattedText.Matches(articleText), ref articleText);

            ReplaceMore(WikiRegexes.IndentedText.Matches(articleText), ref articleText);

            // This hides internal wikilinks (with or without pipe) with extra word character(s) e.g. [[link]]age, which need hiding even if hiding for typo fixing
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Improve_HideText.HideMore.28.29
            // place this as first wikilink rule as otherwise WikiLinksOnly will grab link without extra word character(s)
            if(hideWikiLinks)
                ReplaceMore(WikiRegexes.WikiLinksOnlyPlusWord.Matches(articleText), ref articleText);

            // if HideOnlyTargetOfWikilink is not set, pipes of links e.g. [[target|pipe]] will be hidden
            // if set then don't mask the pipe of a link so that typo fixing can be done on it
            if (!hideOnlyTargetOfWikilink && hideWikiLinks)
            {
                ReplaceMore(WikiRegexes.WikiLinksOnly.Matches(articleText), ref articleText);

                ReplaceMore(WikiRegexes.SimpleWikiLink.Matches(articleText), ref articleText);
            }

            ReplaceMore(WikiRegexes.Cites.Matches(articleText), ref articleText);

            ReplaceMore(WikiRegexes.Refs.Matches(articleText), ref articleText);

            // this hides only the target of a link, leaving the pipe exposed
            if(hideWikiLinks)
                ReplaceMore(WikiRegexes.WikiLink.Matches(articleText), ref articleText);

            ReplaceMore(WikiRegexes.Images.Matches(articleText), ref articleText);

            // hide untemplated quotes between some form of quotation marks (most particularly for typo fixing)
            ReplaceMore(WikiRegexes.UntemplatedQuotes.Matches(articleText), ref articleText);
            
            if(hideItalics)
                ReplaceMore(WikiRegexes.Italics.Matches(articleText), ref articleText);

            ReplaceMore(WikiRegexes.Pstyles.Matches(articleText), ref articleText);

            return articleText;
        }

        private static readonly Regex HiddenMoreRegex = new Regex("⌊⌊⌊⌊M(\\d*)⌋⌋⌋⌋", RegexOptions.Compiled);

        /// <summary>
        /// Adds back hidden stuff from HideMore
        /// </summary>
        public string AddBackMore(string articleText)
        {
            MatchCollection mc;

            while ((mc = HiddenMoreRegex.Matches(articleText)).Count > 0)
            {
                StringBuilder sb = new StringBuilder(articleText.Length * 2);
                int pos = 0;

                foreach (Match m in mc)
                {
                    sb.Append(articleText, pos, m.Index - pos);
                    sb.Append(MoreHide[int.Parse(m.Groups[1].Value)].Text);
                    pos = m.Index + m.Value.Length;
                }

                sb.Append(articleText, pos, articleText.Length - pos);

                articleText = sb.ToString();
            }

            MoreHide.Clear();
            return articleText;
        }
        #endregion

    }

    struct HideObject
    {
        public HideObject(string code, string text)
        {
            Code = code;
            Text = text;
        }

        public readonly string Code, Text;

        public override string ToString()
        {
            return Code + " --> " + Text;
        }
    }
}
