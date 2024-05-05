﻿/*
(C) 2007 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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

/* Some of this is currently only suitable for enwiki. */

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WikiFunctions.TalkPages
{
    public enum DEFAULTSORT
    {
        NoChange,
        MoveToTop,
        MoveToBottom
    }

    internal class Processor
    {
        public string DefaultSortKey;
        public bool FoundDefaultSort, FoundSkipToTalk;

        // Match evaluators:
        public string DefaultSortMatchEvaluator(Match match)
        {
            FoundDefaultSort = true;
            if (match.Groups["key"].Captures.Count > 0)
                DefaultSortKey = match.Groups["key"].Captures[0].Value.Trim();
            return "";
        }

        public string SkipTOCMatchEvaluator(Match match)
        {
            FoundSkipToTalk = true;
            return "";
        }
    }

    public static class TalkPageFixes
    {
        /// <summary>
        /// Processes talk pages: moves any talk page header template, moves any default, adds a section heading if missing
        /// </summary>
        /// <param name="articleText">The talk page text</param>
        /// <param name="moveDefaultsort">The action to take over any defaultsort on the page</param>
        /// <returns>The updated talk page text</returns>
        public static bool ProcessTalkPage(ref string articleText, DEFAULTSORT moveDefaultsort)
        {
            Processor pr = new Processor();

            articleText = WikiRegexes.SkipTOCTemplateRegex.Replace(articleText, pr.SkipTOCMatchEvaluator, 1);
            string zerothSectionOriginal = WikiRegexes.ZerothSection.Match(articleText).Value,
            articleTextOriginal = articleText;

            // https://en.wikipedia.org/w/index.php?title=Wikipedia:Talk_page_layout&oldid=998119817#Lead_(bannerspace)
            // Correct order (though, not completely up to date!) per WP:TPL is
            // 1. {{GA nominee}}, {{Featured article candidates}}, or {{Peer review}}
            // 2. {{Skip to talk}}
            // 3. {{Talk header}}
            // 4. 
            // 5. {{community article probation}}, {{censor}}, {{BLP others}} and other high-priority/importance, warning/attention templates.
            // 6. Specific talk page guideline banners, such as {{Not a forum}}, {{Recurring themes}}, {{FAQ}}, {{Round in circles}}, etc.
            // 7. Language related talk page guideline banners, such as {{American English}}
            // 8. Any "article history" banner or "article milestone" banner (e.g., {{DYK talk}}, {{On this day}}, {{ITN talk}})
            // 9. WikiProject banner shell - Any WikiProject banners
            // 10. {{Image requested}}
            // 11. {{Press}} and {{Connected contributor}}
            // 12. {{To do}}
            // 13. {{Find sources notice}}, {{Reliable sources for medical articles}}
            // 14. {{Copied}}
            // 15. {{Split from}}, {{Split to}}
            // 16. {{Merge from}}, {{Merge-to}}

            // The article text below is reordered from last (on the list above) to first.

            string wpbsBefore = WikiRegexes.WikiProjectBannerShellTemplate.Match(articleText).Value;
            bool blanklinesinwpbsBefore = wpbsBefore.Contains("\r\n\r\n");

            articleText = MoveTalkTemplates(articleText, MergeTemplates);
            articleText = MoveTalkTemplates(articleText, SplitTemplates);
            articleText = MoveTalkTemplates(articleText, Copied);
            articleText = MoveTalkTemplates(articleText, FindSource);
            articleText = MoveTalkTemplate(articleText, TodoTemplate);
            articleText = MoveTalkTemplates(articleText, PressConnected);
            articleText = MoveTalkTemplate(articleText, ImageRequested);

            // if template moving leaves blank lines in WPBS then clean this up
            if (wpbsBefore.Length > 0 && !blanklinesinwpbsBefore)
            {
                articleText = WikiRegexes.WikiProjectBannerShellTemplate.Replace(articleText, m => m.Value.Replace("\r\n\r\n", "\r\n"));
            }
 
            articleText = WikiProjectBannerShell(articleText);

            articleText = MoveTalkTemplate(articleText, WikiRegexes.WikiProjectBannerShellTemplate);
            if (!WikiRegexes.WikiProjectBannerShellTemplate.IsMatch(articleText))
            {
                articleText = MoveTalkTemplates(articleText, WikiProjects);
            }
            articleText = MoveTalkTemplates(articleText, MilestoneTemplates);
            articleText = MoveTalkTemplates(articleText, TalkHistoryBTemplates);
            articleText = MoveTalkTemplates(articleText, TalkHistoryTemplates);
            articleText = MoveTalkTemplate(articleText, EnglishVariationsTemplates);
            articleText = MoveTalkTemplates(articleText, TalkGuidelineTemplates);
            articleText = MoveTalkTemplates(articleText, TalkWarningTemplates);

            // 3. {{Talk header}}
            articleText = MoveTalkTemplate(articleText, WikiRegexes.TalkHeaderTemplate);

            // 2. {{Skip to talk}}
            if (pr.FoundSkipToTalk)
            {
                WriteHeaderTemplate("Skip to talk", ref articleText);
            }

            // 1. {{GA nominee}}
            articleText = MoveTalkTemplate(articleText, GANomineeTemplate);

            if (moveDefaultsort != DEFAULTSORT.NoChange)
            {
                articleText = WikiRegexes.Defaultsort.Replace(articleText, pr.DefaultSortMatchEvaluator, 1);
                if (pr.FoundDefaultSort && !string.IsNullOrEmpty(pr.DefaultSortKey))
                {
                    articleText = SetDefaultSort(pr.DefaultSortKey, moveDefaultsort, articleText);
                }
            }

            articleText = AddMissingFirstCommentHeader(articleText);
            
            articleText = WPBiography(articleText);

            articleText = WPSongs(articleText);

            articleText = WPJazz(articleText);

            // remove redundant Template: in templates in zeroth section
            // clean up excess blank lines after template move
            string zerothSection = WikiRegexes.ZerothSection.Match(articleText).Value;
            if (zerothSection.Length > 0)
            {
                // have we only added whitespace? then reset articletext
                if (zerothSection.Length > zerothSectionOriginal.Length && 
                   WikiRegexes.WhiteSpace.Replace(zerothSection, "").Equals(WikiRegexes.WhiteSpace.Replace(zerothSectionOriginal, "")))
                {
                    articleText = articleTextOriginal;
                }
                else 
                {
                    string zerothbefore = zerothSection;
                    // clean excess blank lines at end of zeroth section, leave only one newline
                    // not when later sections: would remove blank line before heading
                    if (zerothSection.Length == articleText.Length &&
                       (zerothSection.Length - zerothSection.Trim().Length) > 2)
                    {
                        zerothSection = zerothSection.Trim() + "\r\n";
                    }

                    zerothSection = WikiRegexes.ThreeOrMoreNewlines.Replace(zerothSection, "\r\n\r\n");
                    zerothSection = Parse.Parsers.RemoveTemplateNamespace(zerothSection);
                    articleText = articleText.Replace(zerothbefore, zerothSection);
                }
            }


            return pr.FoundSkipToTalk || pr.FoundDefaultSort;
        }

        /// <summary>
        /// Formats the default sort: ensures in {{DEFAULTSORT:key}} format
        /// </summary>
        /// <returns>
        /// The updated article text.
        /// </returns>
        /// <param name='articleText'>
        /// The article text.
        /// </param>
        public static string FormatDefaultSort(string articleText)
        {
            return WikiRegexes.Defaultsort.Replace(articleText, "{{DEFAULTSORT:${key}}}");
        }

        /// <summary>
        /// Moves the default sort.
        /// </summary>
        /// <returns>
        /// The updated article text.
        /// </returns>
        /// <param name='key'>
        /// Current defaultsort key.
        /// </param>
        /// <param name='location'>
        /// Required defaultsort location (top or bottom).
        /// </param>
        /// <param name='articleText'>
        /// Article text.
        /// </param>
        private static string SetDefaultSort(string key, DEFAULTSORT location, string articleText)
        {
            switch (location)
            {
                case DEFAULTSORT.MoveToTop:
                    return "{{DEFAULTSORT:" + key + "}}\r\n" + articleText;

                case DEFAULTSORT.MoveToBottom:
                    return articleText + "\r\n{{DEFAULTSORT:" + key + "}}";
            }

            return "";
        }

        /// <summary>
        /// Writes the input template to the top of the input page; updates the input edit summary
        /// </summary>
        /// <param name="name">template name to write</param>
        /// <param name="articleText">article text to update</param>
        private static void WriteHeaderTemplate(string name, ref string articleText)
        {
            articleText = "{{" + name + "}}\r\n" + articleText;
        }

        private static readonly Regex GANomineeTemplate = Tools.NestedTemplateRegex(new [] { "GA nominee", "GAnominee", "GA" });
        private static readonly Regex TalkWarningTemplates = Tools.NestedTemplateRegex(new[] { "Community article probation", "Censor", "Controversial", "BLP others", "COI editnotice", "Notice", "warning", "Austrian economics sanctions" });
        private static readonly Regex TalkGuidelineTemplates = Tools.NestedTemplateRegex(new[] { "Not a forum", "Recurring themes", "FAQ", "Round in circles", "Calm", "Pbneutral" });
        private static readonly Regex EnglishVariationsTemplates = Tools.NestedTemplateRegex(new[] { "American English", "Australian English", "British English", "British English Oxford spelling", "Canadian English", "Hiberno-English", "Indian English", "Malaysian English", "Malawian English", "New Zealand English", "Pakistani English", "Philippine English", "Scottish English", "South African English", "Trinidadian English" });
        private static readonly Regex TalkHistoryTemplates = Tools.NestedTemplateRegex(new[] { "Article history", "ArticleHistory" });
        private static readonly Regex TalkHistoryBTemplates = Tools.NestedTemplateRegex(new[] { "FailedGA", "Old prod", "Old prod full", "Oldprodfull", "Afd-merged-from", "Old AfD multi", "Old AfD", "Oldafdfull ", "Old peer review", "Old CfD", "Old RfD", "Old XfD multi", "Old XfD" });
        private static readonly Regex MilestoneTemplates = Tools.NestedTemplateRegex(new[] { "DYK talk", "ITN talk", "On this day" });
        private static readonly Regex ImageRequested = Tools.NestedTemplateRegex(new[] { "Image requested", "Reqphoto" });
        private static readonly Regex PressConnected = Tools.NestedTemplateRegex(new[] { "Press", "Connected contributor", "Wikipedian-bio", "Notable Wikipedian" });
        private static readonly Regex TodoTemplate = Tools.NestedTemplateRegex(new[] { "To do", "Todo", "To-do" });
        private static readonly Regex FindSource = Tools.NestedTemplateRegex(new[] { "Find sources notice", "Reliable sources for medical articles", "Friendly search suggestions" });
        private static readonly Regex Copied = Tools.NestedTemplateRegex(new[] { "Copied" });
        private static readonly Regex SplitTemplates = Tools.NestedTemplateRegex(new[] { "Split from", "Split to" });
        private static readonly Regex MergeTemplates = Tools.NestedTemplateRegex(new[] { "Merge from", "Merge-to" });
        private static readonly Regex WikiProjects = new Regex(Tools.NestedTemplateRegex("foo").ToString().Replace(@"[Ff]oo", @"[Ww]ikiProject\b[^{}\|]+"));

        /// <summary>
        /// Moves the input template to the top of the talk page
        /// </summary>
        /// <param name="articleText">the talk page text</param>
        /// <param name="r">Regex matching the template to be moved</param>
        /// <returns>the update talk page text</returns>
        private static string MoveTalkTemplate(string articleText, Regex r)
        {
            Match m = r.Match(articleText);

            if (m.Success && m.Index > 0)
            {
                // remove existing template – handle case where not at beginning of line
                articleText = articleText.Replace(m.Value, articleText.Contains("\r\n" + m.Value) ? "" : "\r\n");

                // write existing template to top
                articleText = m.Value.TrimEnd() + "\r\n" + articleText.TrimStart();

                // ensure any talk header template is now named {{talk header}}
                if (m.Groups[1].Value.ToLower().Contains("talk"))
                    articleText = Tools.RenameTemplate(articleText, m.Groups[1].Value, "Talk header", false);
            }

            return articleText;
        }

        /// <summary>
        /// Moves the input templates to the top of the talk page
        /// </summary>
        /// <param name="articleText">the talk page text</param>
        /// <param name="r">Regex matching the templates to be moved</param>
        /// <returns>the update talk page text</returns>
        private static string MoveTalkTemplates(string articleText, Regex r)
        {
            string originalArticletext = articleText;

            // get the zeroth section (text upto first heading)
            string zerothSection = WikiRegexes.ZerothSection.Match(articleText).Value;

            // avoid moving commented out tags
            if (!Variables.LangCode.Equals("en") || !r.IsMatch(WikiRegexes.Comments.Replace(zerothSection, "")))
                return articleText;

            // get the rest of the article including first heading (may be null if article has no headings)
            string restOfArticle = articleText.Substring(zerothSection.Length);

            string strTags = "";

            foreach (Match m in r.Matches(zerothSection))
            {
                strTags += m.Value + "\r\n";

                // additionally, remove whitespace after tag
                zerothSection = Regex.Replace(zerothSection, Regex.Escape(m.Value) + @" *(?:\r\n)?", "");
            }

            articleText = strTags + zerothSection + restOfArticle;

            // avoid moving commented out tags, round 2
            if (Tools.UnformattedTextNotChanged(originalArticletext, articleText))
                return articleText;

            return originalArticletext;
        }

        private static readonly Regex FirstComment = new Regex(@"^ {0,4}[:\*\w'""](?<!_)", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SingleCurlyBrackets = new Regex(@"{((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))})");

        /// <summary>
        /// Adds a section 2 heading before the first comment if the talk page does not have one
        /// </summary>
        /// <param name="articleText">The talk page text</param>
        /// <returns>The updated article text</returns>
        private static string AddMissingFirstCommentHeader(string articleText)
        {
            // don't match on lines within templates or wiki tables
            string articleTextTemplatesSpaced = Tools.ReplaceWithSpaces(articleText, SingleCurlyBrackets.Matches(articleText));
            articleTextTemplatesSpaced = Tools.ReplaceWithSpaces(articleTextTemplatesSpaced, WikiRegexes.UnformattedText.Matches(articleTextTemplatesSpaced));
            articleTextTemplatesSpaced = Tools.ReplaceWithSpaces(articleTextTemplatesSpaced, WikiRegexes.GalleryTag.Matches(articleTextTemplatesSpaced));

            if (FirstComment.IsMatch(articleTextTemplatesSpaced))
            {
                int firstCommentIndex = FirstComment.Match(articleTextTemplatesSpaced).Index;

                int firstLevelTwoHeading = WikiRegexes.HeadingLevelTwo.IsMatch(articleText) ? WikiRegexes.HeadingLevelTwo.Match(articleText).Index : 99999999;

                if (firstCommentIndex < firstLevelTwoHeading)
                {
                    // is there a heading level 3? If yes, change to level 2
                    string articletexttofirstcomment = articleText.Substring(0, firstCommentIndex);

                    articleText = WikiRegexes.HeadingLevelThree.IsMatch(articletexttofirstcomment) ? WikiRegexes.HeadingLevelThree.Replace(articleText, @"==$1==", 1) : articleText.Insert(firstCommentIndex, "\r\n==Untitled==\r\n").TrimStart();
                    articleText = articleText.Replace("\r\n\r\n\r\n==Untitled", "\r\n\r\n==Untitled");
                }
            }

            return articleText;
        }

        private static readonly List<string> BannerShellRedirects = new List<string>(new[] {
            "WPBS", "Wpbs",
            "WPB", "Wpb",
            "WP banner shell", "WP Banner Shell",
            "WPBannerShell",
            "WikiProject Banner Shell", "WikiProjectBannerShell", "Wikiprojectbannershell",
            "WikiProject Banners", "WikiProjectBanners",
            "WPBannerShell",
            "Bannershell", "banner shell",
            "Shell","asffss"
        });
        private static readonly List<string> Nos = new List<string>(new[] { "blp", "blpo", "activepol" });
        private static readonly Regex BLPRegex = Tools.NestedTemplateRegex(new[] { "blp", "BLP", "Blpinfo" });
        private static readonly Regex BLPORegex = Tools.NestedTemplateRegex(new[] { "blpo", "BLPO", "BLP others" });
        private static readonly Regex ActivepolRegex = Tools.NestedTemplateRegex(new[] { "activepol", "active politician", "activepolitician" });
        private static readonly Regex WPBiographyR = Tools.NestedTemplateRegex(new[] { "WPBiography", "Wikiproject Biography", "WikiProject Biography", "WPBIO", "Bio" });
        private static readonly Regex WPSongsR = Tools.NestedTemplateRegex(new[] { "WikiProject Songs", "WikiProjectSongs", "WP Songs", "Song", "WPSongs", "Songs", "WikiProject Song" });
        private static readonly Regex WPJazzR = Tools.NestedTemplateRegex(new[] { "WikiProject Jazz", "WPJAZZ", "WPJazz", "WP Jazz", "Wikiproject Jazz", "WikiProject Jazz music", "Jazz-music-project" });
        private static readonly Regex WPAlbumR = Tools.NestedTemplateRegex(new[] { "WikiProject Albums", "Albums", "WP Albums", "WPAlbums", "Album", "WPALBUMS" });
        private static readonly Regex SirRegex = Tools.NestedTemplateRegex(new[] { "sir", "Single infobox request" });

        /// <summary>
        /// Performs fixes to the WikiProject banner shell template:
        /// Add explicit call to first unnamed parameter 1= if missing/has no value
        /// Remove duplicate parameters
        /// Moves any other WikiProjects into WPBS
        /// </summary>
        /// <param name="articletext">The talk page text</param>
        /// <returns>The updated talk page text</returns>
        public static string WikiProjectBannerShell(string articletext)
        {
            if (!Variables.LangCode.Equals("en"))
                return articletext;

            articletext = AddWikiProjectBannerShell(articletext);

            if (!WikiRegexes.WikiProjectBannerShellTemplate.IsMatch(articletext))
                return articletext;

            // rename redirects
            articletext = BannerShellRedirects.Aggregate(articletext,
                (current, redirect) => Tools.RenameTemplate(current, redirect, "WikiProject banner shell", false));

            foreach (Match m in WikiRegexes.WikiProjectBannerShellTemplate.Matches(articletext))
            {
                string newValue = m.Value;
                newValue = Tools.RemoveExcessTemplatePipes(newValue);
                string arg1 = Tools.GetTemplateParameterValue(newValue, "1");
                bool shellTemplate = Tools.GetTemplateName(newValue).ToLower().EndsWith("shell");
                
                // Add explicit call to first unnamed parameter 1= if missing/has no value
                if (arg1.Length == 0)
                {
                    int argCount = Tools.GetTemplateArgumentCount(newValue);

                    for (int arg = 1; arg <= argCount; arg++)
                    {
                        string argValue = Tools.GetTemplateArgument(newValue, arg);

                        if (argValue.StartsWith(@"{{"))
                        {
                            newValue = newValue.Insert(Tools.GetTemplateArgumentIndex(newValue, arg), "1=");
                            break;
                        }
                    }
                }
                
                // remove duplicate parameters
                newValue = Tools.RemoveDuplicateTemplateParameters(newValue);

                // clean blp=no, blpo=no, activepol=no, collapsed=no
                foreach (string theNo in Nos)
                {
                    if (Tools.GetTemplateParameterValue(newValue, theNo).Equals("no"))
                        newValue = Tools.RemoveTemplateParameter(newValue, theNo);
                }
                
                string collapsedParam = Tools.GetTemplateParameterValue(newValue, "collapsed");
                if (shellTemplate && collapsedParam.Equals("no"))
                    newValue = Tools.RemoveTemplateParameter(newValue, "collapsed");
                else  if (!shellTemplate && collapsedParam.Equals("yes"))
                    newValue = Tools.RemoveTemplateParameter(newValue, "collapsed");

                // If {{blpo}} then add blpo=yes to WPBS and remove {{blpo}}
                Match blpom = BLPORegex.Match(articletext);
                if (blpom.Success)
                {
                    newValue = Tools.SetTemplateParameterValue(newValue, "blpo", "yes");
                    articletext = articletext.Replace(blpom.Value, "");
                }

                // If {{BLP}} then add blp=yes to WPBS and remove {{BLP}}
                Match blpm = BLPRegex.Match(articletext);
                if (blpm.Success)
                {
                    newValue = Tools.SetTemplateParameterValue(newValue, "blp", "yes");
                    articletext = articletext.Replace(blpm.Value, "");
                }

                // If {{activepol}} then add activepol=yes to WPBS and remove {{activepol}}
                Match activepolm = ActivepolRegex.Match(articletext);
                if (activepolm.Success)
                {
                    newValue = Tools.SetTemplateParameterValue(newValue, "activepol", "yes");
                    articletext = articletext.Replace(activepolm.Value, "");
                }
                                
                // merge changes to article text
                if (!newValue.Equals(m.Value))
                    articletext = articletext.Replace(m.Value, newValue);
            }
            
            // Move WikiProjects into WPBS
            if (WikiRegexes.WikiProjectBannerShellTemplate.Matches(articletext).Count == 1)
            {
                string WPBS = WikiRegexes.WikiProjectBannerShellTemplate.Match(articletext).Value, newParams = "";
                
                foreach (Match m in WikiRegexes.NestedTemplates.Matches(articletext))
                {
                    string templateName = Tools.TurnFirstToUpper(Tools.GetTemplateName(m.Value));
                    if ((templateName.StartsWith("WikiProject ") || templateName == "Vital article") && !WPBS.Contains(m.Value))
                    {
                        articletext = articletext.Replace(m.Value, "");
                        newParams += Tools.Newline(m.Value);
                    }
                }
                if (newParams.Length > 0)
                    articletext = articletext.Replace(WPBS, Tools.SetTemplateParameterValue(WPBS, "1", Tools.GetTemplateParameterValue(WPBS, "1") + newParams)).TrimStart();
            }
            
            // check living, activepol, blpo flags against WPBiography
            foreach (Match m in WikiRegexes.WikiProjectBannerShellTemplate.Matches(articletext))
            {
                string newValue = m.Value;
                string arg1 = Tools.GetTemplateParameterValue(newValue, "1");
                
                Match m2 = WPBiographyR.Match(arg1);

                if (m2.Success)
                {
                    string WPBiographyCall = m2.Value;

                    string livingParam = Tools.GetTemplateParameterValue(WPBiographyCall, "living");
                    if (livingParam.Equals("yes"))
                        newValue = Tools.SetTemplateParameterValue(newValue, "blp", "yes");
                    else if (livingParam.Equals("no"))
                    {
                        if (Tools.GetTemplateParameterValue(newValue, "blp").Equals("yes"))
                            newValue = Tools.RemoveTemplateParameter(newValue, "blp");
                    }

                    string activepolParam = Tools.GetTemplateParameterValue(WPBiographyCall, "activepol");
                    if (activepolParam.Equals("yes"))
                        newValue = Tools.SetTemplateParameterValue(newValue, "activepol", "yes");
                    else if (activepolParam.Equals("no"))
                    {
                        if (Tools.GetTemplateParameterValue(newValue, "activepol").Equals("yes"))
                            newValue = Tools.RemoveTemplateParameter(newValue, "activepol");
                    }

                    if (Tools.GetTemplateParameterValue(WPBiographyCall, "blpo").Equals("yes"))
                        newValue = Tools.SetTemplateParameterValue(newValue, "blpo", "yes");
                }
                                
                // merge changes to article text
                if (!newValue.Equals(m.Value))
                    articletext = articletext.Replace(m.Value, newValue);
            }

            return articletext;
        }
        
        /// <summary>
        /// Moves WPBiography with living=yes above any WikiProject templates per Wikipedia:TPL#Talk_page_layout
        /// Does various fixes to WPBiography
        /// en-wiki only
        /// </summary>
        /// <param name="articletext">The talk page text</param>
        /// <returns>The updated talk page text</returns>
        public static string WPBiography(string articletext)
        {
            if (!Variables.LangCode.Equals("en"))
                return articletext;
            
            Match m = WPBiographyR.Match(articletext);            
            
            if (m.Success)
            {
                string newvalue = m.Value;
                
                // If {{activepol}} then add living=yes, activepol=yes, politician-work-group=yes to WPBiography and remove {{activepol}}
                Match activepolm = ActivepolRegex.Match(articletext);
                if (activepolm.Success)
                {
                    newvalue = Tools.SetTemplateParameterValue(newvalue, "living", "yes");
                    newvalue = Tools.SetTemplateParameterValue(newvalue, "activepol", "yes");
                    newvalue = Tools.SetTemplateParameterValue(newvalue, "politician-work-group", "yes");
                    
                    articletext = ActivepolRegex.Replace(articletext, "");
                }
                
                // If {{BLP}} then add living=yes to WPBiography and remove {{BLP}}
                Match blpm = BLPRegex.Match(articletext);
                if (blpm.Success & !Tools.GetTemplateParameterValue(newvalue, "living").ToLower().StartsWith("n"))
                {
                    newvalue = Tools.SetTemplateParameterValue(newvalue, "living", "yes");
                    articletext = BLPRegex.Replace(articletext, "");
                }
                
                if (newvalue.Length > 0 && !m.Value.Equals(newvalue))
                    articletext = articletext.Replace(m.Value, newvalue);
            }

            // refresh
            m = WPBiographyR.Match(articletext);
            
            if (!m.Success || !Tools.GetTemplateParameterValue(m.Value, "living").ToLower().StartsWith("y"))
                return articletext;
            
            // remove {{blp}} if {{WPBiography|living=yes}}
            articletext = BLPRegex.Replace(articletext, "");
            
            // remove {{activepol}} if {{WPBiography|activepol=yes}}
            articletext = ActivepolRegex.Replace(articletext, "");
            
            // move above any other WikiProject
            if (!WikiRegexes.WikiProjectBannerShellTemplate.IsMatch(articletext))
            {
                foreach (Match n in WikiRegexes.NestedTemplates.Matches(articletext))
                {
                    if (n.Index < m.Index && Tools.GetTemplateName(n.Value).StartsWith("WikiProject "))
                    {
                        articletext = articletext.Replace(m.Value, "");
                        articletext = articletext.Insert(n.Index, m.Value + "\r\n");
                        break;
                    }
                }
            }
            
            return articletext;
        }
        
        
         /// <summary>
        /// Does various fixes to WPSongs
        /// en-wiki only
        /// </summary>
        /// <param name="articletext">The talk page text</param>
        /// <returns>The updated talk page text</returns>
        public static string WPSongs(string articletext)
        {
            if (!Variables.LangCode.Equals("en"))
                return articletext;
            
            Match m = WPSongsR.Match(articletext);
            
            if (m.Success)
            {
                string newvalue = m.Value;
                   
                // Remove needs-infobox=no
                if (Tools.GetTemplateParameterValue(newvalue, "needs-infobox").Equals("no"))
                    newvalue = Tools.RemoveTemplateParameter(newvalue, "needs-infobox");
                // Remove importance. WPSongs doesn't do importance
                newvalue = Tools.RemoveTemplateParameter(newvalue, "importance");

                articletext = articletext.Replace(m.Value, newvalue);

                // If {{sir}} then add needs-infobox=yes to WPsongs and remove {{sir}}
                Match sirm = SirRegex.Match(articletext);
                if (sirm.Success)
                {
                    newvalue = Tools.SetTemplateParameterValue(newvalue, "needs-infobox", "yes");
                    articletext = articletext.Replace(m.Value, newvalue);
                    articletext = SirRegex.Replace(articletext, "");
                }
                
            }

            return articletext;
        }

        /// <summary>
        /// Does various fixes to WPJazz
        /// en-wiki only
        /// </summary>
        /// <param name="articletext">The talk page text</param>
        /// <returns>The updated talk page text</returns>
        public static string WPJazz(string articletext)
        {
            if (!Variables.LangCode.Equals("en"))
                return articletext;

            Match m = WPJazzR.Match(articletext);

            if (m.Success)
            {
                string newvalue = m.Value;

                // Remove needs-infobox=no
                if (Tools.GetTemplateParameterValue(newvalue, "needs-infobox").Equals("no"))
                    newvalue = Tools.RemoveTemplateParameter(newvalue, "needs-infobox");

                articletext = articletext.Replace(m.Value, newvalue);

                // If {{WPSongs}} then add song=yes to WPJazz
                Match songs = WPSongsR.Match(articletext);
                if (songs.Success)
                {
                    newvalue = Tools.SetTemplateParameterValue(newvalue, "song", "yes");
                    articletext = articletext.Replace(m.Value, newvalue);
                }

                // If {{WPAlbums}} then add album=yes to WPJazz
                Match album = WPAlbumR.Match(articletext);
                if (album.Success)
                {
                    newvalue = Tools.SetTemplateParameterValue(newvalue, "album", "yes");
                    articletext = articletext.Replace(m.Value, newvalue);
                }
            }

            return articletext;
        }
        
        private const int WikiProjectsWPBS = 2;
        /// <summary>
        /// Regex to match WikiProject templates: WikiProject, Wikiproject, wiki project, wiki Project etc.
        /// </summary>
        private static readonly Regex WikiProject = new Regex(@"^[Ww]iki ?[Pp]roject ");
        
        /// <summary>
        /// Adds WikiProject banner shell when needed (>= 3 WikiProject templates and no WikiProject banner shell)
        /// </summary>
        /// <param name="articletext">The talk page text</param>
        /// <returns>The updated talk page text</returns>
        private static string AddWikiProjectBannerShell(string articletext)
        {
            int wikiProjectTemplates = 0;
            string WPBS1 = "", articletextLocal = articletext;
            
            if (!WikiRegexes.WikiProjectBannerShellTemplate.IsMatch(articletextLocal))
            {
                foreach (Match m in WikiRegexes.NestedTemplates.Matches(articletextLocal))
                {
                    if (!WikiProject.IsMatch(Tools.GetTemplateName(m.Value)))
                        continue;
                    
                    wikiProjectTemplates++;
                    WPBS1 += Tools.Newline(m.Value);
                    articletextLocal = articletextLocal.Replace(m.Value, "");
                }
                
                if (wikiProjectTemplates > WikiProjectsWPBS)
                {
                    // add a WikiProject banner shell
                    articletext = @"{{WikiProject banner shell|1=" + WPBS1 + Tools.Newline(@"}}")
                        + articletextLocal;

                    // clean up excess whitespace
                    string zerothSection = WikiRegexes.ZerothSection.Match(articletext).Value;
                    string restOfArticle = articletext.Replace(zerothSection, "");

                    zerothSection = WikiRegexes.ThreeOrMoreNewlines.Replace(zerothSection, "\r\n\r\n");

                    articletext = zerothSection + restOfArticle;
                }
            }
            
            return articletext;
        }
    }
}