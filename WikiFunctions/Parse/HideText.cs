﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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
        /// Hides Unformatted text (nowiki, pre, math, html comments, timelines), source tags
        /// </summary>
        /// <param name="hideExternalLinks">Whether external links should be hidden too</param>
        /// <param name="leaveMetaHeadings">Whether to not hide section headings</param>
        /// <param name="hideImages">Whether images should be hidden too</param>
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
        // cached versions of articletext before Hide, and with Hide applied
        // so AddBack can return cached version if no changes
        private string cachedOriginalArticleTextBeforeHide = "";
        private string cachedArticleTextAfterHide = "";

        /// <summary>
        /// Puts back hidden text
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="articleText">The wiki text of the article.</param>
        private void Replace(IEnumerable matches, ref string articleText)
        {
            Replace(matches, ref articleText, HiddenTokens);
        }

        /// <summary>
        /// Puts back hidden text
        /// </summary>
        /// <param name="matches"></param>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="Tokens"> </param>
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
        
        private static readonly Regex CiteTitleYear = new Regex(@"(?<=\|\s*(?:trans_)?title\s*=\s*)[^\|{}<>]*[12][0-9]{3}\b", RegexOptions.Compiled);
        private static readonly Regex MathCodeTypoTemplates = Tools.NestedTemplateRegex(new [] { "math", "code", "As written", "Notatypo", "Not a typo", "Proper name", "Typo" });
        private static readonly Regex RetainBraces = new Regex(@"(?<=\[\[).+(?=\]\])", RegexOptions.Singleline);
        private static readonly Regex RetainStartBraces = new Regex(@"(?<=\[\[).+", RegexOptions.Singleline);
        private static readonly Regex RetainEndBraces = new Regex(@".+(?=\]\])", RegexOptions.Singleline);
        private static readonly Regex All = new Regex(@".+", RegexOptions.Singleline);

        /// <summary>
        /// Hides Unformatted text (nowiki, pre, math, html comments, timelines), source tags
        /// Also hides images and external links if set on call to constructor
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public string Hide(string articleText)
        {
            HiddenTokens.Clear();
            cachedOriginalArticleTextBeforeHide = articleText;

            // performance: get all tags in format <tag...> in article, apply Replace only if needed
            List<string> AnyTagList = (from Match m in AnyTag.Matches(articleText)
                select m.Groups[1].Value.Trim().ToLower()).ToList();

            if (AnyTagList.Any(t => t.Equals("tt") || t.StartsWith("syntaxhighlight") || t.Equals("code") || t.StartsWith("source")))
                Replace(WikiRegexes.SourceCode.Matches(articleText), ref articleText);
            Replace(MathCodeTypoTemplates.Matches(articleText), ref articleText);

            var matches = (from Match m in WikiRegexes.UnformattedText.Matches(articleText) where !LeaveMetaHeadings || !NoWikiIgnoreRegex.IsMatch(m.Value) select m).ToList();
            Replace(matches, ref articleText);

            if (HideExternalLinks)
            {
                Replace(WikiRegexes.ExternalLinks.Matches(articleText), ref articleText);

                List<Match> matches2 = (from Match m in WikiRegexes.PossibleInterwikis.Matches(articleText) where SiteMatrix.Languages.Contains(m.Groups[1].Value.ToLower()) select m).ToList();
                Replace(matches2, ref articleText);
            }
            
            if (HideImages)
            {
                // hide all image links but retain any braces at either end
                articleText = WikiRegexes.Images.Replace(articleText, m => {
                    string res = m.Value;

                    // don't hide URL parameter starting www as FixCitationTemplates will fix these
                    if(res.TrimStart().StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                        return res;

                    if (res.StartsWith("[["))
                    {
                        if (res.EndsWith("]]"))
                            Replace(RetainBraces.Matches(res), ref res);
                        else
                            Replace(RetainStartBraces.Matches(res), ref res);
                    }
                    else if (res.EndsWith("]]"))
                        Replace(RetainEndBraces.Matches(res), ref res);
                    else
                        Replace(All.Matches(res), ref res);
                    return res;});

                if (AnyTagList.Any(t => t.Contains("imagemap")))
                    Replace(WikiRegexes.ImageMap.Matches(articleText), ref articleText);

                // Hide any title or trans_title parameters with dates in them
                Replace(CiteTitleYear.Matches(articleText), ref articleText);

                // gallery tag does not require Image: namespace link before image in gallery, so hide anything before pipe
                if (AnyTagList.Any(t => t.Contains("gallery")))
                    articleText = WikiRegexes.GalleryTag.Replace(articleText, m => {
                        string res = m.Value;
                        Replace(ImageToBar.Matches(res), ref res);
                        return res;});
            }

            cachedArticleTextAfterHide = articleText;
            return articleText;
        }

        public static readonly Regex HiddenRegex = new Regex("⌊⌊⌊⌊(\\d*)⌋⌋⌋⌋", RegexOptions.Compiled);

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
        /// <param name="Tokens"></param>
        /// <returns></returns>
        private string AddBack(string articleText, List<HideObject> Tokens)
        {
            // performance: return cached value if no changes made to articleText
            if (cachedArticleTextAfterHide.Equals(articleText))
            {
                articleText = cachedOriginalArticleTextBeforeHide;

                // clear down
                cachedOriginalArticleTextBeforeHide = "";
                cachedArticleTextAfterHide = "";
            }
            else
            {
                // while loop as there can be nested hiding
                while (HiddenRegex.IsMatch(articleText))
                    articleText = HiddenRegex.Replace(articleText, m => Tokens[int.Parse(m.Groups[1].Value)].Text);
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

        // cached versions of articletext before HideMore, and with HideMore applied
        // so AddBackMore can return cached version if no changes
        private string cachedOriginalArticleTextBeforeHideMore = "";
        private string cachedArticleTextAfterHideMore = "";

        /// <summary>
        /// Replaces hidden images, external links, templates, headings etc.
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

        private static readonly Regex AnyTag = new Regex(@"<([^<>]+)>");
        private static readonly Regex ImageToBar = new Regex(@"^.+?\.[a-zA-Z]{3,4}\s*(?=\||\r\n)", RegexOptions.Multiline);
        private static readonly Regex SimpleRef = new Regex(@"<ref>[^<>]+</ref>");
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
            cachedOriginalArticleTextBeforeHideMore = articleText;

            ReplaceMore(WikiRegexes.NestedTemplates.Matches(articleText), ref articleText);

            ReplaceMore(WikiRegexes.AllTags.Matches(articleText), ref articleText);

            // this replace done for performance: quickly clear out lots of standard refs
            ReplaceMore(SimpleRef.Matches(articleText), ref articleText);

            if (HideExternalLinks)
            {
                // performance: only use all-protocol regex if the uncommon protocols are in use
                ReplaceMore(WikiRegexes.ExternalLinksHTTPOnly.Matches(articleText), ref articleText);

                if (articleText.Contains("//"))
                    ReplaceMore(WikiRegexes.ExternalLinks.Matches(articleText), ref articleText);                    
            }

            ReplaceMore(WikiRegexes.Headings.Matches(articleText), ref articleText);

            ReplaceMore(WikiRegexes.Comments.Matches(articleText), ref articleText);

            ReplaceMore(WikiRegexes.IndentedText.Matches(articleText), ref articleText);

            // This hides internal wikilinks (with or without pipe) with extra word character(s) e.g. [[link]]age, which need hiding even if hiding for typo fixing
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Improve_HideText.HideMore.28.29
            // place this as first wikilink rule as otherwise WikiLinksOnly will grab link without extra word character(s)
            if (hideWikiLinks)
                ReplaceMore(WikiRegexes.WikiLinksOnlyPlusWord.Matches(articleText), ref articleText);

            // if HideOnlyTargetOfWikilink is not set, pipes of links e.g. [[target|pipe]] will be hidden
            // if set then don't mask the pipe of a link so that typo fixing can be done on it
            if (!hideOnlyTargetOfWikilink && hideWikiLinks)
                ReplaceMore(WikiRegexes.SimpleWikiLink.Matches(articleText), ref articleText);

            // if hiding target of wikilinks only, this does not apply to Category links, hide all of these
            if (hideOnlyTargetOfWikilink && hideWikiLinks)
                ReplaceMore(WikiRegexes.Category.Matches(articleText), ref articleText);

            ReplaceMore(WikiRegexes.Refs.Matches(articleText), ref articleText);

            // performance: get all remaining tags in format <tag...> in article, apply ReplaceMore only if needed
            List<string> AnyTagList = (from Match m in AnyTag.Matches(articleText)
                select m.Groups[1].Value.Trim().ToLower()).ToList();

            // gallery tag does not require Image: namespace link before image in gallery, so hide anything before pipe
            if (AnyTagList.Any(t => t.Contains("gallery")))
                articleText = WikiRegexes.GalleryTag.Replace(articleText, m => {
                    string res = m.Value;
                    ReplaceMore(ImageToBar.Matches(res), ref res);
                    return res;});

            // this hides only the target of a link, leaving the pipe exposed
            if (hideWikiLinks)
                ReplaceMore(WikiRegexes.WikiLink.Matches(articleText), ref articleText);

            // Image links within templates already hidden as NestedTemplates hides all of templates
            if (AnyTagList.Any(t => t.Contains("gallery")) || articleText.Contains(@"[[")) // check for performance
                ReplaceMore(WikiRegexes.ImagesNotTemplates.Matches(articleText), ref articleText);

            // hide untemplated quotes between some form of quotation marks (most particularly for typo fixing)
            ReplaceMore(WikiRegexes.UntemplatedQuotes.Matches(articleText), ref articleText);

            if (hideItalics)
                ReplaceMore(WikiRegexes.Italics.Matches(articleText), ref articleText);

            if (AnyTagList.Any(t => t.Contains("p style")))
                ReplaceMore(WikiRegexes.Pstyles.Matches(articleText), ref articleText);

            cachedArticleTextAfterHideMore = articleText;
            return articleText;
        }

        private static readonly Regex HiddenMoreRegex = new Regex("⌊⌊⌊⌊M(\\d*)⌋⌋⌋⌋", RegexOptions.Compiled);

        /// <summary>
        /// Adds back hidden stuff from HideMore
        /// </summary>
        public string AddBackMore(string articleText)
        {
            // performance: return cached value if no changes made to articleText
            if (cachedArticleTextAfterHideMore.Equals(articleText))
            {
                articleText = cachedOriginalArticleTextBeforeHideMore;

                // clear down
                cachedOriginalArticleTextBeforeHideMore = "";
                cachedArticleTextAfterHideMore = "";
            }
            else
            {
                // while loop as there can be nested hiding
                while (HiddenMoreRegex.IsMatch(articleText))
                    articleText = HiddenMoreRegex.Replace(articleText, m => MoreHide[int.Parse(m.Groups[1].Value)].Text);
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
