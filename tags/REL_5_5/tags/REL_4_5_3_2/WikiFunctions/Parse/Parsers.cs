/*

Copyright (C) 2007 Martin Richards

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
using System.Web;
using WikiFunctions.Lists;

namespace WikiFunctions.Parse
{
    /// <summary>
    /// Provides functions for editting wiki text, such as formatting and re-categorisation.
    /// </summary>
    public class Parsers
    {
        #region constructor etc.
        public Parsers()
        {
        }

        /// <summary>
        /// Re-organises the Person Data, stub/disambig templates, categories and interwikis
        /// </summary>
        /// <param name="stubWordCount">The number of maximum number of words for a stub.</param>
        /// <param name="addHumanKey"></param>
        public Parsers(int stubWordCount, bool addHumanKey)
        {
            StubMaxWordCount = stubWordCount;
            Sorter.AddCatKey = addHumanKey;
        }

        /// <summary>
        /// 
        /// </summary>
        static Parsers()
        {
            //look bad if changed
            RegexUnicode.Add(new Regex("&(ndash|mdash|minus|times|lt|gt|nbsp|thinsp|shy|lrm|rlm|[Pp]rime|ensp|emsp|#x2011|#820[13]|#8239);", RegexOptions.Compiled), "&amp;$1;");
            //IE6 does like these
            RegexUnicode.Add(new Regex("&#(705|803|596|620|699|700|8652|9408|9848|12288|160|61|x27|39);", RegexOptions.Compiled), "&amp;#$1;");

            //Decoder doesn't like these
            RegexUnicode.Add(new Regex("&#(x109[0-9A-Z]{2});", RegexOptions.Compiled), "&amp;#$1;");
            RegexUnicode.Add(new Regex("&#((?:277|119|84|x1D|x100)[A-Z0-9a-z]{2,3});", RegexOptions.Compiled), "&amp;#$1;");
            RegexUnicode.Add(new Regex("&#(x12[A-Za-z0-9]{3});", RegexOptions.Compiled), "&amp;#$1;");

            //interfere with wiki syntax
            RegexUnicode.Add(new Regex("&#(0?13|126|x5[BD]|x7[bcd]|0?9[13]|0?12[345]|0?0?3[92]);", RegexOptions.Compiled | RegexOptions.IgnoreCase), "&amp;#$1;");

            RegexTagger.Add(new Regex(@"\{\{(template:)?(wikify(-date)?|wfy|wiki)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            if (Variables.LangCode != LangCodeEnum.nl)
                RegexTagger.Add(new Regex(@"\{\{(template:)?(Clean( ?up)?|CU|Tidy)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Cleanup|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Linkless|Orphan)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Orphan|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Uncategori[sz]ed|Uncat|Classify|Category needed|Catneeded|categori[zs]e|nocats?)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Uncategorized|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser#AWB_problems
            // on en wiki {{references}} is a cleanup tag meaning 'this article needs more references' (etc.) whereas on nl wiki (Sjabloon:References) {{references}} acts like <references/>
            if (Variables.LangCode != LangCodeEnum.nl)
                RegexTagger.Add(new Regex(@"\{\{(template:)?(Unreferenced(sect)?|add references|cite[ -]sources?|cleanup-sources?|needs? references|no sources|no references?|not referenced|references|unref|unsourced)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Unreferenced|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Trivia2?|Too ?much ?trivia|Trivia section|Cleanup-trivia)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Trivia|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(deadend|DEP)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{deadend|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(copyedit|g(rammar )?check|copy-edit|cleanup-copyedit|cleanup-english)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{copyedit|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(sources|refimprove|not verified)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{refimprove|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Uncategori[zs]edstub|uncatstub)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Uncategorizedstub|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Importance)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Importance|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Expand)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Expand|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Fact)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Fact|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(COI|Conflict of interest|Selfpromotion)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{COI|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");

            RegexConversion.Add(new Regex(@"\{\{(?:Template:)?(Dab|Disamb|Disambiguation)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Disambig}}");
            RegexConversion.Add(new Regex(@"\{\{(?:Template:)?(Bio-dab|Hndisambig)", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Hndis");

            RegexConversion.Add(new Regex(@"\{\{(?:Template:)?(Prettytable|Prettytable100)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{subst:Prettytable}}");
            RegexConversion.Add(new Regex(@"\{\{(?:[Tt]emplate:)?(PAGENAMEE?\}\}|[Ll]ived\||[Bb]io-cats\|)", RegexOptions.Compiled), "{{subst:$1");

            // clean 'do-attempt =July 2006|att=April 2008' to 'do attempt = April 2008'
            RegexConversion.Add(new Regex(@"({{\s*[Aa]rticle ?issues\s*(?:\|[^{}]*|\|)\s*[Dd]o-attempt\s*=\s*)[^{}\|]+\|\s*att\s*=\s*([^{}\|]+)(?=\||}})"), "$1$2");

            // clean "Copyedit for=grammar|date=April 2009"to "Copyedit=April 2009"
            RegexConversion.Add(new Regex(@"({{\s*[Aa]rticle ?issues\s*(?:\|[^{}]*|\|)\s*[Cc]opyedit\s*)for\s*=\s*[^{}\|]+\|\s*date(\s*=[^{}\|]+)(?=\||}})"), "$1$2");

            // could be multiple to date per template so loop
            for (int a = 0; a < 5; a++)
            {
                // add date to any undated tags within {{Article issues}}
                RegexConversion.Add(new Regex(@"({{\s*[Aa]rticle ?issues\s*(?:\|[^{}]*|\|)\s*)(?![Ee]xpert)" + WikiRegexes.ArticleIssuesTemplatesString + @"\s*(\||}})"), "$1$2={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}$3");
                
                // clean any 'date' word within {{Article issues}} (but not 'update' field), place after the date adding rule above
                RegexConversion.Add(new Regex(@"(?<={{\s*[Aa]rticle ?issues\s*(?:\|[^{}]*?)?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}[^{}]*?){0,4}\|[^{}\|]{3,}?)\bdate"), "");
            }

            // articleissues with one issue -> single issue tag (e.g. {{articleissues|cleanup=January 2008}} to {{cleanup|date=January 2008}} etc.)
            RegexConversion.Add(new Regex(@"\{\{[Aa]rticle ?issues\s*\|\s*([^\|{}=]{3,}?)\s*(=\s*\w{3,10}\s+20\d\d\s*\}\})", RegexOptions.Compiled), "{{$1|date$2");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_empty_.7B.7BArticle_issues.7D.7D
            // articleissues with no issues -> remove tag
            RegexConversion.Add(new Regex(@"\{\{[Aa]rticle ?issues(?:\s*\|\s*(?:section|article)\s*=\s*[Yy])?\s*\}\}"), "");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.7B.7Bcommons.7CCategory:XXX.7D.7D_.3E_.7B.7Bcommonscat.7CXXX.7D.7D
            RegexConversion.Add(new Regex(@"\{\{[Cc]ommons\|\s*[Cc]ategory:\s*([^{}]+?)\s*\}\}", RegexOptions.Compiled), @"{{commons cat|$1}}");

            // tidy up || or |}} (maybe with whitespace between) within templates that don't use null parameters
            RegexConversion.Add(new Regex(@"(\{\{\s*(?:[Cc]it|[Aa]rticle ?issues)[^{}]*)\|\s*(\}\}|\|)"), "$1$2");

            // remove duplicate / populated and null fields in cite/article issues templates
            RegexConversion.Add(new Regex(@"({{\s*(?:[Cc]it|[Aa]rticle ?issues)[^{}]*\|\s*)(\w+)\s*=\s*([^\|}{]+?)\s*\|((?:[^{}]*?\|)?\s*)\2(\s*=\s*)\3(\s*(\||\}\}))"), "$1$4$2$5$3$6"); // duplicate field remover for cite templates
            RegexConversion.Add(new Regex(@"(\{\{\s*(?:[Cc]it|[Aa]rticle ?issues)[^{}]*\|\s*)(\w+)(\s*=\s*[^\|}{]+(?:\|[^{}]+?)?)\|\s*\2\s*=\s*(\||\}\})"), "$1$2$3$4"); // 'field=populated | field=null' drop field=null
            RegexConversion.Add(new Regex(@"(\{\{\s*(?:[Cc]it|[Aa]rticle ?issues)[^{}]*\|\s*)(\w+)\s*=\s*\|\s*((?:[^{}]+?\|)?\s*\2\s*=\s*[^\|}{\s])"), "$1$3"); // 'field=null | field=populated' drop field=null

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Add_.7B.7Botheruse.7D.7D_and_.7B.7B2otheruses.7D.7D_in_the_supported_DABlinks
            RegexConversion.Add(new Regex(@"({{)2otheruses(\s*(?:\|[^{}]*}}|}}))"), "$1Two other uses$2");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#AWB_is_still_using_.7B.7BArticleissues.7D.7D_instead_of_.7B.7BArticle_issues.7D.7D
            // replace any {{articleissues}} with {{article issues}}
            RegexConversion.Add(new Regex(@"(?<={{[Aa]rticle)i(?=ssues.*}})"), " i");
        }

        private static readonly Dictionary<Regex, string> RegexUnicode = new Dictionary<Regex, string>();
        private static readonly Dictionary<Regex, string> RegexConversion = new Dictionary<Regex, string>();
        private static readonly Dictionary<Regex, string> RegexTagger = new Dictionary<Regex, string>();

        private readonly HideText Hider = new HideText();
        private readonly HideText HiderHideExtLinksImages = new HideText(true, true, true);
        public static int StubMaxWordCount = 500;

        /// <summary>
        /// Sort interwiki link order
        /// </summary>
        public bool SortInterwikis
        {
            get { return Sorter.SortInterwikis; }
            set { Sorter.SortInterwikis = value; }
        }

        /// <summary>
        /// The interwiki link order to use
        /// </summary>
        public InterWikiOrderEnum InterWikiOrder
        {
            set { Sorter.InterWikiOrder = value; }
            get { return Sorter.InterWikiOrder; }
        }

        /// <summary>
        /// When set to true, adds key to categories (for people only) when parsed
        /// </summary>
        //public bool AddCatKey { get; set; }

        // should NOT be accessed directly, use Sorter
        private MetaDataSorter metaDataSorter;

        public MetaDataSorter Sorter
        {
            get
            {
                if (metaDataSorter == null) metaDataSorter = new MetaDataSorter();
                return metaDataSorter;
            }
        }

        #endregion

        #region General Parse

        public string HideText(string articleText)
        {
            return Hider.Hide(articleText);
        }

        public string AddBackText(string articleText)
        {
            return Hider.AddBack(articleText);
        }

        public string HideMoreText(string articleText, bool hideOnlyTargetOfWikilink)
        {
            return HiderHideExtLinksImages.HideMore(articleText, hideOnlyTargetOfWikilink);
        }

        public string HideMoreText(string articleText)
        {
            return HiderHideExtLinksImages.HideMore(articleText);
        }

        public string AddBackMoreText(string articleText)
        {
            return HiderHideExtLinksImages.AddBackMore(articleText);
        }

        // NOT covered
        /// <summary>
        /// Re-organises the Person Data, stub/disambig templates, categories and interwikis
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article title.</param>
        /// <returns>The re-organised text.</returns>
        public string SortMetaData(string articleText, string articleTitle)
        {
            return (Variables.Project <= ProjectEnum.species) ? Sorter.Sort(articleText, articleTitle) : articleText;
        }

        private static readonly Regex ApostropheInDecades = new Regex(@"(?<=(?:the |later? |early |mid-)(?:\[?\[?[12]\d\d0\]?\]?))'s(?=\]\])?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RegexHeadings0 = new Regex("(== ?)(see also:?|related topics:?|related articles:?|internal links:?|also see:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RegexHeadings1 = new Regex("(== ?)(external link[s]?|external site[s]?|outside link[s]?|web ?link[s]?|exterior link[s]?):?( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private readonly Regex regexHeadings2 = new Regex("(== ?)(external link:?|external site:?|web ?link:?|exterior link:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RegexHeadings3 = new Regex("(== ?)(referen[sc]e:?)(s? ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RegexHeadings4 = new Regex("(== ?)(source:?)(s? ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RegexHeadings5 = new Regex("(== ?)(further readings?:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RegexHeadings6 = new Regex("(== ?)(Early|Personal|Adult|Later) Life( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RegexHeadings7 = new Regex("(== ?)(Current|Past|Prior) Members( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RegexHeadings8 = new Regex(@"^(=+ ?)'''(.*?)'''( ?=+)\s*?(\r)?$", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex RegexHeadings9 = new Regex("(== ?)track listing( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RegexHeadings10 = new Regex("(== ?)Life and Career( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RegexHeadingsCareer = new Regex("(== ?)([a-zA-Z]+) Career( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex RegexRemoveHeadingsInLinks = new Regex(@"^ *((={1,4})[^\[\]\{\}\|=]*)\[{2}(?:[^\[\]\{\}\|=]+\|)?([^\[\]\{\}\|]+)\]\]([^\{\}=]*\2) *$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex RegexBadHeader = new Regex("^(={1,4} ?(about|description|overview|definition|profile|(?:general )?information|background|intro(?:duction)?|summary|bio(?:graphy)?) ?={1,4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex RegexHeadingWhitespaceBefore = new Regex(@"^ *(==+)(\s*.+?\s*)\1 +(\r|\n)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex RegexHeadingWhitespaceAfter = new Regex(@"^ +(==+)(\s*.+?\s*)\1 *(\r|\n)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex RegexHeadingUpOneLevel = new Regex(@"^=(=+[^=].*?[^=]=+)=(\r\n?|\n)$", RegexOptions.Multiline);

        private static readonly Regex RegexHeadingColonAtEnd = new Regex(@"^(=+)(.+?)\:(\s*\1(?:\r\n?|\n))$", RegexOptions.Multiline);

        /// <summary>
        /// Fix ==See also== and similar section common errors.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle"></param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public static string FixHeadings(string articleText, string articleTitle, out bool noChange)
        {
            string testText = articleText;
            articleText = FixHeadings(articleText, articleTitle);

            noChange = (testText == articleText);

            return articleText.Trim();
        }

        private const int MinCleanupTagsToCombine = 3; // article must have at least this many tags to combine to {{Article issues}}

        private static readonly Regex ArticleIssuesInTitleCase = new Regex(@"({{[Aa]rticle ?issues\|\s*(?:[^{}]+?\|\s*)?)([A-Z])([a-z]+(?: [a-z]+)?\s*=)");

        /// <summary>
        /// Combines multiple cleanup tags into {{article issues}} template, ensures parameters have correct case, removes date parameter where not needed
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string ArticleIssues(string articleText)
        {
            // convert title case parameters within {{Article issues}} to lower case
            foreach (Match m in ArticleIssuesInTitleCase.Matches(articleText))
            {
                string firstPart = m.Groups[1].Value;
                string parameterFirstChar = m.Groups[2].Value.ToLower();
                string lastPart = m.Groups[3].Value;

                articleText = articleText.Replace(m.Value, firstPart + parameterFirstChar + lastPart);
            }

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#Date_parameter_getting_stripped_from_.7B.7Btl.7CArticle_issues.7D.7D
            // remove any date field within  {{Article issues}} if no 'expert' field using it
            if(!Regex.IsMatch(articleText, @"{{\s*[Aa]rticle ?issues[^{}]+?expert"))
                articleText = Regex.Replace(articleText, @"({{\s*[Aa]rticle ?issues\s*(?:\|[^{}]*?)?)\|\s*date\s*=[^{}\|]{0,20}?(\||}})", "$1$2");

            string newTags = "";

            // get the zeroth section (text upto first heading)
            string zerothSection = WikiRegexes.ZerothSection.Match(articleText).Value;

            // get the rest of the article including first heading (may be null if article has no headings)
            string restOfArticle = articleText.Replace(zerothSection, "");

            int tagsToAdd = WikiRegexes.ArticleIssuesTemplates.Matches(zerothSection).Count;

            // if currently no {{Article issues}} and less than the min number of cleanup templates, do nothing
            if (!WikiRegexes.ArticleIssues.IsMatch(zerothSection) && WikiRegexes.ArticleIssuesTemplates.Matches(zerothSection).Count < MinCleanupTagsToCombine)
                return (articleText);

            // only add tags to articleissues if new tags + existing >= MinCleanupTagsToCombine
            if ((Regex.Matches(WikiRegexes.ArticleIssues.Match(zerothSection).Value, WikiRegexes.ArticleIssuesTemplatesString).Count + tagsToAdd) < MinCleanupTagsToCombine || tagsToAdd == 0)
                return (articleText);

                foreach (Match m in WikiRegexes.ArticleIssuesTemplates.Matches(zerothSection))
                {
                    // all fields except COI, OR, POV and ones with BLP should be lower case
                    string singleTag = m.Groups[1].Value;
                    if (!Regex.IsMatch(singleTag, "(COI|OR|POV|BLP)"))
                        singleTag = singleTag.ToLower();

                    newTags += @"|" + singleTag + @" " + m.Groups[2].Value;
                    newTags = newTags.Trim();

                    // remove the single template
                    zerothSection = zerothSection.Replace(m.Value, "");
                }

                // if article currently has {{Article issues}}, add tags to it
                if (WikiRegexes.ArticleIssues.IsMatch(zerothSection))
                    zerothSection = WikiRegexes.ArticleIssues.Replace(zerothSection, "$1" + newTags + @"}}");
                else // add {{article issues}} to top of article, metaDataSorter will arrange correctly later
                    zerothSection = @"{{Article issues" + newTags + "}}\r\n" + zerothSection;

            // Parsers.Conversions will add any missing dates and correct ...|wikify date=May 2008|...
            return (zerothSection + restOfArticle);
        }

        // Covered by: FormattingTests.TestFixHeadings(), incomplete
        /// <summary>
        /// Fix ==See also== and similar section common errors. Removes unecessary introductory headings and cleans excess whitespace.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle"></param>
        /// <returns>The modified article text.</returns>
        public static string FixHeadings(string articleText, string articleTitle)
        {
            articleText = Regex.Replace(articleText, "^={1,4} ?" + Regex.Escape(articleTitle) + " ?={1,4}", "", RegexOptions.IgnoreCase);
            articleText = RegexBadHeader.Replace(articleText, "");

            // loop through in case a heading has mulitple wikilinks in it
            while (RegexRemoveHeadingsInLinks.IsMatch(articleText))
            {
                articleText = RegexRemoveHeadingsInLinks.Replace(articleText, "$1$3$4");
            }

            if (!Regex.IsMatch(articleText, "= ?See also ?="))
                articleText = RegexHeadings0.Replace(articleText, "$1See also$3");

            articleText = RegexHeadings1.Replace(articleText, "$1External links$3");
            //articleText = regexHeadings2.Replace(articleText, "$1External link$3");
            articleText = RegexHeadings3.Replace(articleText, "$1Reference$3");
            articleText = RegexHeadings4.Replace(articleText, "$1Source$3");
            articleText = RegexHeadings5.Replace(articleText, "$1Further reading$3");
            articleText = RegexHeadings6.Replace(articleText, "$1$2 life$3");
            articleText = RegexHeadings7.Replace(articleText, "$1$2 members$3");
            articleText = RegexHeadings8.Replace(articleText, "$1$2$3$4");
            articleText = RegexHeadings9.Replace(articleText, "$1Track listing$2");
            articleText = RegexHeadings10.Replace(articleText, "$1Life and career$2");
            articleText = RegexHeadingsCareer.Replace(articleText, "$1$2 career$3");

            articleText = RegexHeadingWhitespaceBefore.Replace(articleText, "$1$2$1$3");
            articleText = RegexHeadingWhitespaceAfter.Replace(articleText, "$1$2$1$3");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Headlines_end_with_colon_.28WikiProject_Check_Wikipedia_.2357.29
            articleText = RegexHeadingColonAtEnd.Replace(articleText, "$1$2$3");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Section_header_level_.28WikiProject_Check_Wikipedia_.237.29
            // if no level 2 heading in article, remove a level from all headings (i.e. '===blah===' to '==blah==' etc.)
            if (!WikiRegexes.HeadingLevelTwo.IsMatch(articleText))
                articleText = RegexHeadingUpOneLevel.Replace(articleText, "$1$2");

            return articleText;
        }

        // Covered by: LinkTests.FixDates()
        /// <summary>
        /// Fix date and decade formatting errors, and replace <br> and <p> HTML tags
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixDates(string articleText)
        {
            articleText = HideMoreText(articleText);

            articleText = FixDatesRaw(articleText);

            //Remove 2 or more <br />'s
            //This piece's existance here is counter-intuitive, but it requires HideMore()
            //and I don't want to call this slow function yet another time --MaxSem
            articleText = SyntaxRemoveBr.Replace(articleText, "\r\n");
            articleText = SyntaxRemoveParagraphs.Replace(articleText, "\r\n\r\n");

            return AddBackMoreText(articleText);
        }

        private static readonly string Months = @"(?:" + string.Join("|", Variables.MonthNames) + ")";

        private static readonly Regex DiedDateRegex = new Regex(@"('''[^']+'''\s*\()d\.(\s+\[*(?:" + Months + @"\s+0?([1-3]?[0-9])|0?([1-3]?[0-9])\s*" + Months + @")?\]*\s*\[*[1-2]?\d{3}\]*\)\s*)", RegexOptions.IgnoreCase);
        private static readonly Regex DOBRegex = new Regex(@"('''[^']+'''\s*\()b\.(\s+\[*(?:" + Months + @"\s+0?([1-3]?\d)|0?([1-3]?\d)\s*" + Months + @")?\]*\s*\[*[1-2]?\d{3}\]*\)\s*)", RegexOptions.IgnoreCase);
        private static readonly Regex BornDeathRegex = new Regex(@"('''[^']+'''\s*\()(?:[Bb]orn|b\.)\s+(\[*(?:" + Months + @"\s+0?(?:[1-3]?\d)|0?(?:[1-3]?\d)\s*" + Months + @")?\]*,?\s*\[*[1-2]?\d{3}\]*)\s*(.|&.dash;)\s*(?:[Dd]ied|d\.)\s+(\[*(?:" + Months + @"\s+0?(?:[1-3]?\d)|0?(?:[1-3]?\d)\s*" + Months + @")\]*,?\s*\[*[1-2]?\d{3}\]*\)\s*)", RegexOptions.IgnoreCase);

        //Covered by: LinkTests.FixLivingThingsRelatedDates()
        /// <summary>
        /// Replace b. and d. for born/died
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixLivingThingsRelatedDates(string articleText)
        {
            articleText = DiedDateRegex.Replace(articleText, "$1died$2"); //date of death
            articleText = DOBRegex.Replace(articleText, "$1born$2"); //date of birth
            return BornDeathRegex.Replace(articleText, "$1$2 – $4"); //birth and death
        }

        // Covered by: LinkTests.FixDates()
        /// <summary>
        /// Fixes date and decade formatting errors.
        /// Unlike FixDates(), requires wikitext processed with HideMore()
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixDatesRaw(string articleText)
        {
            return ApostropheInDecades.Replace(articleText, "s");
        }

        // NOT covered, unused
        /// <summary>
        /// Footnote formatting errors per [[WP:FN]].
        /// currently too buggy to be included into production builds
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixFootnotes(string articleText)
        {
            // One space/linefeed
            articleText = Regex.Replace(articleText, "[\\n\\r\\f\\t ]+?<ref([ >])", "<ref$1");
            // remove trailing spaces from named refs
            articleText = Regex.Replace(articleText, "<ref ([^>]*[^>])[ ]*>", "<ref $1>");
            // removed superscripted punctuation between refs
            articleText = Regex.Replace(articleText, "(</ref>|<ref[^>]*?/>)<sup>[ ]*[,;-]?[ ]*</sup><ref", "$1<ref");
            articleText = Regex.Replace(articleText, "(</ref>|<ref[^>]*?/>)[ ]*[,;-]?[ ]*<ref", "$1<ref");

            const string factTag = "{{[ ]*(fact|fact[ ]*[\\|][^}]*|facts|citequote|citation needed|cn|verification needed|verify source|verify credibility|who|failed verification|nonspecific|dubious|or|lopsided|GR[ ]*[\\|][ ]*[^ ]+|[c]?r[e]?f[ ]*[\\|][^}]*|ref[ _]label[ ]*[\\|][^}]*|ref[ _]num[ ]*[\\|][^}]*)[ ]*}}";
            articleText = Regex.Replace(articleText, "[\\n\\r\\f\\t ]+?" + factTag, "{{$1}}");

            const string lacksPunctuation = "([^\\.,;:!\\?\"'’])";
            const string questionOrExclam = "([!\\?])";
            //string minorPunctuation = "([\\.,;:])";
            const string anyPunctuation = "([\\.,;:!\\?])";
            const string majorPunctuation = "([,;:!\\?])";
            const string period = "([\\.])";
            const string quote = "([\"'’]*)";
            const string space = "[ ]*";

            const string refTag = "(<ref>([^<]|<[^/]|</[^r]|</r[^e]|</re[^f]|</ref[^>])*?</ref>" + "|<ref[^>]*?[^/]>([^<]|<[^/]|</[^r]|</r[^e]|</re[^f]" + "|</ref[^>])*?</ref>|<ref[^>]*?/>)";

            const string match0A = lacksPunctuation + quote + factTag + space + anyPunctuation;
            const string match0B = questionOrExclam + quote + factTag + space + majorPunctuation;
            //string match0c = minorPunctuation + quote + factTag + space + anyPunctuation;
            const string match0D = questionOrExclam + quote + factTag + space + period;

            const string match1A = lacksPunctuation + quote + refTag + space + anyPunctuation;
            const string match1B = questionOrExclam + quote + refTag + space + majorPunctuation;
            //string match1c = minorPunctuation + quote + refTag + space + anyPunctuation;
            const string match1D = questionOrExclam + quote + refTag + space + period;

            string oldArticleText = "";

            while (oldArticleText != articleText)
            { // repeat for multiple refs together
                oldArticleText = articleText;
                articleText = Regex.Replace(articleText, match0A, "$1$2$4$3");
                articleText = Regex.Replace(articleText, match0B, "$1$2$4$3");
                //articleText = Regex.Replace(articleText, match0c, "$2$4$3");
                articleText = Regex.Replace(articleText, match0D, "$1$2$3");

                articleText = Regex.Replace(articleText, match1A, "$1$2$6$3");
                articleText = Regex.Replace(articleText, match1B, "$1$2$6$3");
                //articleText = Regex.Replace(articleText, match1c, "$2$6$3");
                articleText = Regex.Replace(articleText, match1D, "$1$2$3");
            }

            //articleText = Regex.Replace(articleText, "(==*)<ref", "$1\r\n<ref");
            return articleText;
        }

        private const string OutofOrderRefs = @"(<ref\s+name\s*=\s*(?:""|')?([^<>""]+?)(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>)(\s*{{\s*rp\|[^{}]+}})?(\s*)(<ref\s+name\s*=\s*(?:""|')?([^<>""]+?)(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>)(\s*{{\s*rp\|[^{}]+}})?";
        private static readonly Regex OutofOrderRefs1 = new Regex(@"(<ref>[^<>]+</ref>)(\s*)(<ref\s+name\s*=\s*(?:""|')?([^<>""]+?)(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>)(\s*{{\s*[Rr]p\|[^{}]+}})?", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex OutofOrderRefs2 = new Regex(OutofOrderRefs, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        // regex below ensures a forced match on second and third of consecutive references
        private static readonly Regex OutofOrderRefs3 = new Regex(@"(?<=\s*(?:\/\s*|>[^<>]+</ref)>\s*(?:{{\s*rp\|[^{}]+}})?)" + OutofOrderRefs, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Reorders references so that they appear in numerical order, allows for use of {{rp}}
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string ReorderReferences(string articleText)
        {
            string articleTextBefore;
            for (int i = 0; i < 5; i++) // allows for up to 5 consecutive references
            {
                articleTextBefore = articleText;

                foreach (Match m in OutofOrderRefs1.Matches(articleText))
                {
                    string ref1 = m.Groups[1].Value;
                    int ref1Index = Regex.Match(articleText, @"(?i)<ref\s+name\s*=\s*(?:""|')?" + Regex.Escape(m.Groups[4].Value) + @"(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>").Index;
                    int ref2Index = articleText.IndexOf(ref1);

                    if (ref1Index < ref2Index && ref2Index > 0)
                    {
                        string whitespace = m.Groups[2].Value;
                        string rptemplate = m.Groups[5].Value;
                        string ref2 = m.Groups[3].Value;
                        articleText = articleText.Replace(ref1 + whitespace + ref2 + rptemplate, ref2 + rptemplate + whitespace + ref1);
                    }
                }

                articleText = ReorderRefs(articleText, OutofOrderRefs2);
                articleText = ReorderRefs(articleText, OutofOrderRefs3);

                if (articleTextBefore.Equals(articleText))
                    break;
            }

            return (articleText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText"></param>
        /// <param name="outofOrderRegex"></param>
        /// <returns></returns>
        private static string ReorderRefs(string articleText, Regex outofOrderRegex)
        {
            foreach (Match m in outofOrderRegex.Matches(articleText))
            {
                int ref1Index = Regex.Match(articleText, @"(?i)<ref\s+name\s*=\s*(?:""|')?" + Regex.Escape(m.Groups[2].Value) + @"(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>").Index;
                int ref2Index = Regex.Match(articleText, @"(?i)<ref\s+name\s*=\s*(?:""|')?" + Regex.Escape(m.Groups[6].Value) + @"(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>").Index;

                if (ref1Index > ref2Index && ref1Index > 0)
                {
                    string ref1 = m.Groups[1].Value;
                    string ref2 = m.Groups[5].Value;
                    string whitespace = m.Groups[4].Value;
                    string rptemplate1 = m.Groups[3].Value;
                    string rptemplate2 = m.Groups[7].Value;
                    articleText = articleText.Replace(ref1 + rptemplate1 + whitespace + ref2 + rptemplate2, ref2 + rptemplate2 + whitespace + ref1 + rptemplate1);
                }
            }

            return articleText;
        }

        private static readonly Regex NamedReferences = new Regex(@"(<\s*ref\s+name\s*=\s*(?:""|')?([^<>=\r\n]+?)(?:""|')?\s*>\s*([^<>]+?)\s*<\s*/\s*ref>)", RegexOptions.Singleline);

        // Covered by: DuplicateNamedReferencesTests()
        /// <summary>
        /// Where an unnamed reference is a duplicate of another named reference, set the unnamed one to use the named ref
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>        
        public static string DuplicateNamedReferences(string articleText)
        {
            foreach (Match m in NamedReferences.Matches(articleText))
            {
                string refName = m.Groups[2].Value;
                string unnamedRefValue = m.Groups[3].Value;

                int a = 0;
                string firstref = "";
                // duplicate citation fixer (both named): <ref name="Fred">(...)</ref>....<ref name="Fred">\2</ref> --> ..<ref name="Fred"/>, minimum 25 characters to avoid short refs
                // don't fix the fist match, change the later ones to use <ref name="a"/> format, don't bother to change really short refs (value < 25 chars)
                foreach (Match m2 in Regex.Matches(articleText, @"((<\s*ref\s+name\s*=\s*(?:""|')?" + Regex.Escape(refName) + @"(?:""|')?)\s*>\s*" + Regex.Escape(unnamedRefValue) + @"\s*<\s*/\s*ref>)"))
                {
                    if (unnamedRefValue.Length < 25)
                        break;

                    // mask first match
                    if (a == 0)
                    {
                        firstref = m2.Value;
                        Regex r = new Regex(Regex.Escape(firstref));

                        articleText = r.Replace(articleText, @"⌊⌊⌊⌊@@⌋⌋⌋⌋", 1);
                    }
                    else // replace all duplicates
                        articleText = articleText.Replace(m2.Value, @"<ref name=""" + refName + @"""/>");

                    a++;
                }

                // unmask first match
                articleText = articleText.Replace(@"⌊⌊⌊⌊@@⌋⌋⌋⌋", firstref);

                // duplicate citation fixer (first named): <ref name="Fred">(...)</ref>....<ref>\2</ref> --> ..<ref name="Fred"/>
                // duplicate citation fixer (second named): <ref>(...)</ref>....<ref name="Fred">\2</ref> --> ..<ref name="Fred"/>
                foreach (Match m3 in Regex.Matches(articleText, @"<\s*ref\s*>\s*" + Regex.Escape(unnamedRefValue) + @"\s*<\s*/\s*ref>"))
                {
                    articleText = articleText.Replace(m3.Value, @"<ref name=""" + refName + @"""/>");
                }
            }

            return (articleText);
        }

        /// <summary>
        /// Corrects common formatting errors in dates in external reference citation templates (doesn't link/delink dates)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="noChange"></param>
        /// <returns>The modified article text.</returns>
        public string CiteTemplateDates(string articleText, out bool noChange)
        {
            string articleTextAtStart = articleText;

            articleText = CiteTemplateDates(articleText);

            noChange = (articleTextAtStart == articleText);

            return articleText;
        }

        /// <summary>
        /// Corrects common formatting errors in dates in external reference citation templates (doesn't link/delink dates)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string CiteTemplateDates(string articleText)
        {
            // note some incorrect date formats such as 3-2-2009 are ambiguous as could be 3-FEB-2009 or MAR-2-2009
            // these fixes don't address such errors

            const string siCitStart = @"(?si)(\{\{\s*cit[^{}]*\|\s*";

            // convert invalid date formats like DD-MM-YYYY, MM-DD-YYYY, YYYY-D-M, YYYY-DD-MM, YYYY_MM_DD etc. to iso format of YYYY-MM-DD
            // for accessdate= and archivedate=
            if (Regex.IsMatch(articleText, @"\b(access|archive)date\s*=", RegexOptions.IgnoreCase))
            {
                const string citAccessdate = siCitStart + @"(?:access|archive)date\s*=\s*";
                articleText = Regex.Replace(articleText, citAccessdate + @")(1[0-2])[/_\-\.]?(1[3-9])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3");
                articleText = Regex.Replace(articleText, citAccessdate + @")(1[0-2])[/_\-\.]?([2-3]\d)[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3");
                articleText = Regex.Replace(articleText, citAccessdate + @")(1[0-2])[/_\-\.]?\2[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$3-$2-$2"); // nn-nn-2004 and nn-nn-04 to ISO format (both nn the same)
                articleText = Regex.Replace(articleText, citAccessdate + @")(1[3-9])[/_\-\.]?(1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2");
                articleText = Regex.Replace(articleText, citAccessdate + @")(1[3-9])[/_\-\.]?0?([1-9])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2");
                articleText = Regex.Replace(articleText, citAccessdate + @")(20[01]\d)0?([01]\d)[/_\-\.]([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4");
                articleText = Regex.Replace(articleText, citAccessdate + @")(20[01]\d)[/_\-\.]([01]\d)0?([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4");
                articleText = Regex.Replace(articleText, citAccessdate + @")(20[01]\d)[/_\-\.]?([01]\d)[/_\-\.]?([1-9]\s*(?:\||}}))", "$1$2-$3-0$4");
                articleText = Regex.Replace(articleText, citAccessdate + @")(20[01]\d)[/_\-\.]?([1-9])[/_\-\.]?([0-3]\d\s*(?:\||}}))", "$1$2-0$3-$4");
                articleText = Regex.Replace(articleText, citAccessdate + @")(20[01]\d)[/_\-\.]?([1-9])[/_\-\.]0?([1-9]\s*(?:\||}}))", "$1$2-0$3-0$4");
                articleText = Regex.Replace(articleText, citAccessdate + @")(20[01]\d)[/_\-\.]0?([1-9])[/_\-\.]([1-9]\s*(?:\||}}))", "$1$2-0$3-0$4");
                articleText = Regex.Replace(articleText, citAccessdate + @")(20[01]\d)[/_\.]?([01]\d)[/_\.]?([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4");

                articleText = Regex.Replace(articleText, citAccessdate + @")([2-3]\d)[/_\-\.]?(1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2");
                articleText = Regex.Replace(articleText, citAccessdate + @")([2-3]\d)[/_\-\.]0?([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2");
                articleText = Regex.Replace(articleText, citAccessdate + @")0?([1-9])[/_\-\.]?(1[3-9]|[2-3]\d)[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$2-$3");
                articleText = Regex.Replace(articleText, citAccessdate + @")0?([1-9])[/_\-\.]?0?\2[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$3-0$2-0$2"); // n-n-2004 and n-n-04 to ISO format (both n the same)
            }

            // date=, archivedate=, airdate=, date2=
            if (Regex.IsMatch(articleText, @"{{\s*cit[^{}]*\|\s*(?:archive|air)?date2?\s*=", RegexOptions.IgnoreCase | RegexOptions.Singleline))
            {
                string CitDate = siCitStart + @"(?:archive|air)?date2?\s*=\s*";
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)(200\d|19[7-9]\d)[/_]?([0-1]\d)[/_]?([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4");
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)(1[0-2])[/_\-\.]?([2-3]\d)[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$2-$3");
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)0?([1-9])[/_\-\.]?([2-3]\d)[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$2-$3");
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)([2-3]\d)[/_\-\.]?0?([1-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$3-$2");
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)([2-3]\d)[/_\-\.]?(1[0-2])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$3-$2");
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)(1[0-2])[/_\-\.]([2-3]\d)[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3");
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)0?([1-9])[/_\-\.]([2-3]\d)[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$2-$3");
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)([2-3]\d)[/_\-\.]0?([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2");
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)([2-3]\d)[/_\-\.]?(1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2");
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)(1[0-2])[/_\-\.]?(1[3-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$2-$3");
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)0?([1-9])[/_\-\.](1[3-9])[/_\-\.](19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$2-$3");
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)(1[3-9])[/_\-\.]?0?([1-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$3-$2");
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)(1[3-9])[/_\-\.]?(1[0-2])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$3-$2");
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)(1[0-2])[/_\-\.]?(1[3-9])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3");
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)([1-9])[/_\-\.](1[3-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$2-$3");
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)(1[3-9])[/_\-\.]?([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2");
                articleText = Regex.Replace(articleText, CitDate + @"\[?\[?)(1[3-9])[/_\-\.](1[0-2])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2");
                articleText = Regex.Replace(articleText, CitDate + @")0?([1-9])[/_\-\.]0?\2[/_\-\.](200\d|19[7-9]\d)(?=\s*(?:\||}}))", "$1$3-0$2-0$2"); // n-n-2004 and n-n-1980 to ISO format (both n the same)
                articleText = Regex.Replace(articleText, CitDate + @")0?([1-9])[/_\-\.]0?\2[/_\-\.]([01]\d)(?=\s*(?:\||}}))", "${1}20$3-0$2-0$2"); // n-n-04 to ISO format (both n the same)
                articleText = Regex.Replace(articleText, CitDate + @")(1[0-2])[/_\-\.]?\2[/_\-\.]?(200\d|19[7-9]\d)(?=\s*(?:\||}}))", "$1$3-$2-$2"); // nn-nn-2004 and nn-nn-1980 to ISO format (both nn the same)
                articleText = Regex.Replace(articleText, CitDate + @")(1[0-2])[/_\-\.]?\2[/_\-\.]?([01]\d)(?=\s*(?:\||}}))", "${1}20$3-$2-$2"); // nn-nn-04 to ISO format (both nn the same)
                articleText = Regex.Replace(articleText, CitDate + @")((?:\[\[)?200\d|19[7-9]\d)[/_\-\.]([1-9])[/_\-\.]0?([1-9](?:\]\])?\s*(?:\||}}))", "$1$2-0$3-0$4");
                articleText = Regex.Replace(articleText, CitDate + @")((?:\[\[)?200\d|19[7-9]\d)[/_\-\.]0?([1-9])[/_\-\.]([1-9](?:\]\])?\s*(?:\||}}))", "$1$2-0$3-0$4");
                articleText = Regex.Replace(articleText, CitDate + @")((?:\[\[)?200\d|19[7-9]\d)[/_\-\.]?([0-1]\d)[/_\-\.]?([1-9](?:\]\])?\s*(?:\||}}))", "$1$2-$3-0$4");
                articleText = Regex.Replace(articleText, CitDate + @")((?:\[\[)?200\d|19[7-9]\d)[/_\-\.]?([1-9])[/_\-\.]?([0-3]\d(?:\]\])?\s*(?:\||}}))", "$1$2-0$3-$4");
                articleText = Regex.Replace(articleText, CitDate + @")((?:\[\[)?200\d|19[7-9]\d)([0-1]\d)[/_\-\.]([0-3]\d(?:\]\])?\s*(?:\||}}))", "$1$2-$3-$4");
                articleText = Regex.Replace(articleText, CitDate + @")((?:\[\[)?200\d|19[7-9]\d)[/_\-\.]([0-1]\d)0?([0-3]\d(?:\]\])?\s*(?:\||}}))", "$1$2-$3-$4");

                // date = YYYY-Month-DD fix, on en-wiki only
                if (Variables.LangCode == LangCodeEnum.en)
                {
                    string CitYMonthD = siCitStart + @"(?:archive|air)?date2?\s*=\s*\d{4})[-/\s]";
                    string DTemEnd = @"?[-/\s]([0-3]?\d\s*(?:\||}}))";
                    articleText = Regex.Replace(articleText, CitYMonthD + @"Apr(?:il|\.)" + DTemEnd, "$1-04-$2");
                    articleText = Regex.Replace(articleText, CitYMonthD + @"Aug(?:ust|\.)" + DTemEnd, "$1-08-$2");
                    articleText = Regex.Replace(articleText, CitYMonthD + @"Dec(?:ember|\.)" + DTemEnd, "$1-12-$2");
                    articleText = Regex.Replace(articleText, CitYMonthD + @"Feb(?:r?uary|\.)" + DTemEnd, "$1-02-$2");
                    articleText = Regex.Replace(articleText, CitYMonthD + @"Jan(?:uary|\.)" + DTemEnd, "$1-01-$2");
                    articleText = Regex.Replace(articleText, CitYMonthD + @"Jul(?:y|\.)" + DTemEnd, "$1-07-$2");
                    articleText = Regex.Replace(articleText, CitYMonthD + @"Jun(?:e|\.)" + DTemEnd, "$1-06-$2");
                    articleText = Regex.Replace(articleText, CitYMonthD + @"Mar(?:ch|\.)" + DTemEnd, "$1-03-$2");
                    articleText = Regex.Replace(articleText, CitYMonthD + @"Nov(?:ember|\.)" + DTemEnd, "$1-11-$2");
                    articleText = Regex.Replace(articleText, CitYMonthD + @"Oct(?:ober|\.)" + DTemEnd, "$1-10-$2");
                    articleText = Regex.Replace(articleText, CitYMonthD + @"Sep(?:tember|\.)" + DTemEnd, "$1-09-$2");
                    articleText = Regex.Replace(articleText, CitYMonthD + @"May\." + DTemEnd, "$1-05-$2");
                }
            }

            // all citation dates
            articleText = Regex.Replace(articleText, siCitStart + @"(?:archive|air|access)?date2?\s*=\s*(?:\[\[)?200\d)-([2-3]\d|1[3-9])-(0[1-9]|1[0-2])(\]\])?", "$1-$3-$2$4"); // YYYY-DD-MM to YYYY-MM-DD
            articleText = Regex.Replace(articleText, @"(\{\{\s*cite[^\{\}]*\|\s*(?:archive|air|access)?date2?\s*=\s*(?:(?:200\d|19[7-9]\d)-[01]?\d-[0-3]?\d|[0-3]?\d\s*\w+,?\s*(?:200\d|19[7-9]\d)|\w+\s*[0-3]?\d,?\s*(?:200\d|19[7-9]\d)))(\s*[,-:]?\s+[0-2]?\d\:?[0-5]\d(?:\:?[0-5]\d)?\s*[^\|\}]*)", "$1<!--$2-->", RegexOptions.IgnoreCase | RegexOptions.Singleline); // Removes time from date fields

            return (articleText);
        }

        // Covered by: FormattingTests.TestMdashes()
        /// <summary>
        /// Replaces hyphens and em-dashes with en-dashes, per [[WP:DASH]]
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle"></param>
        /// <param name="nameSpaceKey"></param>
        /// <returns>The modified article text.</returns>
        public string Mdashes(string articleText, string articleTitle, int nameSpaceKey)
        {
            articleText = HideMoreText(articleText);

            //string months = "(" + string.Join("|", Variables.MonthNames) + ")";

            articleText = Regex.Replace(articleText, @"(pages\s*=\s*|pp\.?\s*)(\d+\s*)(?:-|—|&mdash;|&#8212;)(\s*\d+)", @"$1$2–$3", RegexOptions.IgnoreCase);

            articleText = Regex.Replace(articleText, @"([1-9]?\d\s*)(?:-|—|&mdash;|&#8212;)(\s*[1-9]?\d)(\s+|&nbsp;)(years|months|weeks|days|hours|minutes|seconds|kg|mg|kb|km|[Gk]?Hz|miles|mi\.|%)\b", @"$1–$2$3$4");

            //articleText = Regex.Replace(articleText, @"(\[?\[?" + months + @"\ [1-3]?\d\]?\]?,\ \[?\[?[1-2]\d{3}\]?\]?)\s*(?:-|—|&mdash;|&#8212;)\s*(\[?\[?" + months + @"\ [1-3]?\d\]?\]?,\ \[?\[?[1-2]\d{3}\]?\]?)", @"$1–$3");

            articleText = Regex.Replace(articleText, @"(\$[1-9]?\d{1,3}\s*)(?:-|—|&mdash;|&#8212;)(\s*\$?[1-9]?\d{1,3})", @"$1–$2");

            articleText = Regex.Replace(articleText, @"([01]?\d:[0-5]\d\s*([AP]M)\s*)(?:-|—|&mdash;|&#8212;)(\s*[01]?\d:[0-5]\d\s*([AP]M))", @"$1–$3", RegexOptions.IgnoreCase);

            articleText = Regex.Replace(articleText, @"([Aa]ge[sd])\s([1-9]?\d\s*)(?:-|—|&mdash;|&#8212;)(\s*[1-9]?\d)", @"$1 $2–$3");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Match_en_dashes.2Femdashs_from_titles_with_those_in_the_text
            // if title has en or em dashes, apply them to strings matching article title but with hyphens
            if (articleTitle.Contains(@"–") || articleTitle.Contains(@"—"))
                articleText = Regex.Replace(articleText, Regex.Escape(articleTitle.Replace(@"–", @"-").Replace(@"—", @"-")), articleTitle);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Change_--_.28two_dashes.29_to_.E2.80.94_.28em_dash.29
            if (nameSpaceKey == 0)
                articleText = Regex.Replace(articleText, @"(?<=\w)\s*--\s*(?=\w)", @"—");

            return AddBackMoreText(articleText);
        }

        // Covered by: FootnotesTests.TestFixReferenceListTags()
        private static string ReflistMatchEvaluator(Match m)
        {
            // don't change anything if div tags mismatch
            if (DivStart.Matches(m.Value).Count != DivEnd.Matches(m.Value).Count) return m.Value;

            if (m.Value.Contains("references-2column")) return "{{reflist|2}}";

            string s = Regex.Match(m.Value, @"[^-]column-count:[\s]*?(\d*)").Groups[1].Value;
            if (s.Length > 0) return "{{reflist|" + s + "}}";

            s = Regex.Match(m.Value, @"-moz-column-count:[\s]*?(\d*)").Groups[1].Value;
            if (s.Length > 0) return "{{reflist|" + s + "}}";

            return "{{reflist}}";
        }

        /// <summary>
        /// Main regex for {{reflist}} converter
        /// </summary>
        private static readonly Regex ReferenceListTags = new Regex(@"(<(span|div)( class=""(references-small|small|references-2column)|)?"">[\r\n\s]*){1,2}[\r\n\s]*<references[\s]?/>([\r\n\s]*</(span|div)>){1,2}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex BadReferenceListTags = new Regex(@"<references>(</references>)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex DivStart = new Regex(@"<div\b.*?>", RegexOptions.Compiled);
        private static readonly Regex DivEnd = new Regex(@"< ?/ ?div\b.*?>", RegexOptions.Compiled);

        // Covered by: FootnotesTests.TestFixReferenceListTags()
        /// <summary>
        /// Replaces various old reference tag formats, with the new {{reflist}}
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns></returns>
        public static string FixReferenceListTags(string articleText)
        {
            if (BadReferenceListTags.IsMatch(articleText))
                articleText = BadReferenceListTags.Replace(articleText, "<references/>");

            return ReferenceListTags.Replace(articleText, new MatchEvaluator(ReflistMatchEvaluator));
        }

        private static readonly Regex EmptyReferences = new Regex(@"<ref\s+name=[""]?([^<>""]*)[""]?\s*>[\s]*< ?/ ?ref ?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Covered by: FootnotesTests.TestSimplifyReferenceTags()
        /// <summary>
        /// Replaces reference tags in the form <ref name="blah"></ref> with <ref name="blah" />
        /// Removes some of the MW errors that occur from the prior
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns></returns>
        public static string SimplifyReferenceTags(string articleText)
        {
            if (EmptyReferences.Match(articleText).Success)
                articleText = EmptyReferences.Replace(articleText, @"<ref name=""$1"" />");

            return articleText;
        }

        // NOT covered
        /// <summary>
        /// if the article uses cite references but has no recognised template to display the references, add {{reflist}} in the appropriate place
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns></returns>
        public static string AddMissingReflist(string articleText)
        {
            // AddMissingReflist is only called if references template is missing

            // Rename ==Links== to ==External links==
            articleText = Regex.Replace(articleText, @"(?sim)(==+\s*)Links(\s*==+\s*(?:^(?:\*|\d\.?)?\s*\[?\s*http://))", "$1External links$2");

            if (Regex.IsMatch(articleText, @"(?i)==\s*'*\s*References?\s*'*\s*=="))
                articleText = Regex.Replace(articleText, @"(?i)(==+\s*'*\s*References?\s*'*\s*==+)", "$1\r\n{{Reflist}}<!--added under references heading by script-assisted edit-->");
            else
            {
                //now try to move just above external links
                if (Regex.IsMatch(articleText, @"(?im)(^\s*=+\s*(?:External\s+link|Source|Web\s*link)s?\s*=)"))
                {
                    articleText += "\r\n==References==\r\n{{Reflist}}<!--added above External links/Sources by script-assisted edit-->";
                    articleText = Regex.Replace(articleText, @"(?sim)(^\s*=+\s*(?:External\s+link|Source|Web\s*link)s?\s*=+.*?)(\r\n==+References==+\r\n{{Reflist}}<!--added above External links/Sources by script-assisted edit-->)", "$2\r\n$1");
                }
                else
                { // now try to move just above categories
                    if (Regex.IsMatch(articleText, @"(?im)(^\s*\[\[\s*Category\s*:)"))
                    {
                        articleText += "\r\n==References==\r\n{{Reflist}}<!--added above categories/infobox footers by script-assisted edit-->";
                        articleText = Regex.Replace(articleText, @"(?sim)((?:^\{\{[^{}]+?\}\}\s*)*)(^\s*\[\[\s*Category\s*:.*?)(\r\n==+References==+\r\n{{Reflist}}<!--added above categories/infobox footers by script-assisted edit-->)", "$3\r\n$1$2");
                    }
                    //else
                    //{
                    // TODO: relist is missing, but not sure where references should go – at end of article might not be correct
                    //articleText += "\r\n==References==\r\n{{Reflist}}<!--added to end of article by script-assisted edit-->";
                    //articleText = Regex.Replace(articleText, @"(?sim)(^==.*?)(^\{\{[^{}]+?\}\}.*?)(\r\n==+References==+\r\n{{Reflist}}<!--added to end of article by script-assisted edit-->)", "$1\r\n$3\r\n$2");
                    //}
                }
            }
            // remove reflist comment
            articleText = Regex.Replace(articleText, @"(\{\{Reflist\}\})<!--added[^<>]+by script-assisted edit-->", "$1");

            return articleText;
        }

        // whitespace cleaning
        private static readonly Regex RefWhitespace1 = new Regex(@"<\s*(?:\s+ref\s*|\s*ref\s+)>", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex RefWhitespace2 = new Regex(@"<(?:\s*/(?:\s+ref\s*|\s*ref\s+)|\s+/\s*ref\s*)>", RegexOptions.Compiled | RegexOptions.Singleline);
        // remove any whitespace between consecutive references
        private static readonly Regex RefWhitespace3 = new Regex(@"(</ref>|<ref\s*name\s*=[^{}<>]+?\s*\/\s*>) +(?=<ref(?:\s*name\s*=[^{}<>]+?\s*\/?\s*)?>)");
        // ensure a space between a reference and text (reference within a paragrah)
        private static readonly Regex RefWhitespace4 = new Regex(@"(</ref>|<ref\s*name\s*=[^{}<>]+?\s*\/\s*>)(\w)");

        // <ref name="Fred" /ref> --> <ref name="Fred"/>
        private static readonly Regex ReferenceTags1 = new Regex(@"(<\s*ref\s+name\s*=\s*""[^<>=""\/]+?"")\s*/\s*ref\s*>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // <ref name="Fred""> --> <ref name="Fred">
        private static readonly Regex ReferenceTags2 = new Regex(@"(<\s*ref\s+name\s*=\s*""[^<>=""\/]+?"")["">]\s*(/?)>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // <ref name = ”Fred”> --> <ref name="Fred">
        private static readonly Regex ReferenceTags3 = new Regex(@"(<\s*ref\s+name\s*=\s*)[“‘”’]*([^<>=""\/]+?)[“‘”’]+(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static readonly Regex ReferenceTags4 = new Regex(@"(<\s*ref\s+name\s*=\s*)[“‘”’]+([^<>=""\/]+?)[“‘”’]*(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // <ref name = ''Fred'> --> <ref name="Fred"> (two apostrophes)
        private static readonly Regex ReferenceTags5 = new Regex(@"(<\s*ref\s+name\s*=\s*)''+([^<>=""\/]+?)'+(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // <ref name = 'Fred''> --> <ref name="Fred"> (two apostrophes)
        private static readonly Regex ReferenceTags6 = new Regex(@"(<\s*ref\s+name\s*=\s*)'+([^<>=""\/]+?)''+(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // <ref name=foo bar> --> <ref name="foo bar">, match when spaces
        private static readonly Regex ReferenceTags7 = new Regex(@"(<\s*ref\s+name\s*=\s*)([^<>=""'\/]+?\s+[^<>=""'\/]+?)(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // <ref name=foo bar> --> <ref name="foo bar">, match when non-ASCII characters ([\x00-\xff]*)
        private static readonly Regex ReferenceTags8 = new Regex(@"(<\s*ref\s+name\s*=\s*)([^<>=""'\/]*?[^\x00-\xff]+?[^<>=""'\/]*?)(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // <ref name=foo bar"> --> <ref name="foo bar">
        private static readonly Regex ReferenceTags9 = new Regex(@"(<\s*ref\s+name\s*=\s*)['`”]*([^<>=""\/]+?)""(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // <ref name="foo bar> --> <ref name="foo bar">
        private static readonly Regex ReferenceTags10 = new Regex(@"(<\s*ref\s+name\s*=\s*)""([^<>=""\/]+?)['`”]*(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // <ref name "foo bar"> --> <ref name="foo bar">
        private static readonly Regex ReferenceTags11 = new Regex(@"(<\s*ref\s+name\s*)[\+\-]?(\s*""[^<>=""\/]+?""\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // <ref "foo bar"> --> <ref name="foo bar">
        private static readonly Regex ReferenceTags12 = new Regex(@"(<\s*ref\s+)(""[^<>=""\/]+?""\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // ref name typos
        private static readonly Regex ReferenceTags13 = new Regex(@"(<\s*ref\s+n)(me\s*=)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // <ref>...<ref/> --> <ref>...</ref>
        private static readonly Regex ReferenceTags14 = new Regex(@"(<\s*ref(?:\s+name\s*=[^<>]*?)?\s*>[^<>""]+?)<\s*ref\s*/\s*>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // <ref>...</red> --> <ref>...</ref>
        private static readonly Regex ReferenceTags15 = new Regex(@"(<\s*ref(?:\s+name\s*=[^<>]*?)?\s*>[^<>""]+?)<\s*/\s*red\s*>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);

        // <REF> and <Ref> to <ref>
        private static readonly Regex ReferenceTags16 = new Regex(@"(<\s*\/?\s*)(?:R[Ee][Ff]|r[Ee]F)(\s*(?:>|name\s*=))");

        // Covered by TestFixReferenceTags
        /// <summary>
        /// Various fixes to the formatting of <ref> reference tags
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns>The modified article text.</returns>
        public static string FixReferenceTags(string articleText)
        {
            articleText = RefWhitespace1.Replace(articleText, "<ref>");
            articleText = RefWhitespace2.Replace(articleText, "</ref>");
            articleText = RefWhitespace3.Replace(articleText, "$1");
            articleText = RefWhitespace4.Replace(articleText, "$1 $2");

            articleText = ReferenceTags1.Replace(articleText, "$1/>");
            articleText = ReferenceTags2.Replace(articleText, "$1$2>");
            articleText = ReferenceTags3.Replace(articleText, @"$1""$2""$3");
            articleText = ReferenceTags4.Replace(articleText, @"$1""$2""$3");
            articleText = ReferenceTags5.Replace(articleText, @"$1""$2""$3");
            articleText = ReferenceTags6.Replace(articleText, @"$1""$2""$3");
            articleText = ReferenceTags7.Replace(articleText, @"$1""$2""$3");
            articleText = ReferenceTags8.Replace(articleText, @"$1""$2""$3");
            articleText = ReferenceTags9.Replace(articleText, @"$1""$2""$3");
            articleText = ReferenceTags10.Replace(articleText, @"$1""$2""$3");
            articleText = ReferenceTags11.Replace(articleText, @"$1=$2");
            articleText = ReferenceTags12.Replace(articleText, "$1name=$2");
            articleText = ReferenceTags13.Replace(articleText, "$1a$2");
            articleText = ReferenceTags14.Replace(articleText, "$1</ref>");
            articleText = ReferenceTags15.Replace(articleText, "$1</ref>");
            articleText = ReferenceTags16.Replace(articleText, "$1ref$2");

            return articleText;
        }

        // Covered by TestFixDateOrdinalsAndOf
        /// <summary>
        /// Removes ordinals, leading zeros from dates and 'of' between a month and a year, per [[WP:MOSDATE]]; on en wiki only
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <param name="articleTitle">The article's title</param>
        /// <returns>The modified article text.</returns>
        public string FixDateOrdinalsAndOf(string articleText, string articleTitle)
        {
            if (Variables.LangCode != LangCodeEnum.en)
                return articleText;

            // don't match on 'in the June of 2007', 'on the 11th May 2008' etc. as these won't read well if changed
            Regex ofBetweenMonthAndYear = new Regex(@"\b" + WikiRegexes.months + @"\s+of\s+(200\d|1[89]\d\d)\b(?<!\b[Tt]he\s{1,5}\w{3,15}\s{1,5}of\s{1,5}(200\d|1[89]\d\d))");

            Regex ordinalsInDatesAm = new Regex(@"\b" + WikiRegexes.months + @"\s+([0-3]?\d)(?:st|nd|rd|th)\b(?<!\b[Tt]he\s+\w{3,10}\s+(?:[0-3]?\d)(?:st|nd|rd|th)\b)(?:(\s*(?:to|and|.|&.dash;)\s*[0-3]?\d)(?:st|nd|rd|th)\b)?");
            Regex ordinalsInDatesInt = new Regex(@"(?:\b([0-3]?\d)(?:st|nd|rd|th)(\s*(?:to|and|.|&.dash;)\s*))?\b([0-3]?\d)(?:st|nd|rd|th)\s+" + WikiRegexes.months + @"\b(?<!\b[Tt]he\s+(?:[0-3]?\d)(?:st|nd|rd|th)\s+\w{3,10})");

            Regex dateLeadingZerosAm = new Regex(@"\b" + WikiRegexes.months + @"\s+0([1-9])" + @"\b");
            Regex dateLeadingZerosInt = new Regex(@"\b" + @"0([1-9])\s+" + WikiRegexes.months + @"\b");

            // hide items in quotes etc., though this may also hide items within infoboxes etc.
            articleText = HideMoreText(articleText);

            articleText = ofBetweenMonthAndYear.Replace(articleText, "$1 $2");

            // don't apply if article title has a month in it (e.g. [[6th of October City]])
            if (!Regex.IsMatch(articleTitle, @"\b" + Months + @"\b"))
            {
                articleText = ordinalsInDatesAm.Replace(articleText, "$1 $2$3");
                articleText = ordinalsInDatesInt.Replace(articleText, "$1$2$3 $4");
            }

            articleText = dateLeadingZerosAm.Replace(articleText, "$1 $2");
            articleText = dateLeadingZerosInt.Replace(articleText, "$1 $2");

            return AddBackMoreText(articleText);
        }


        // Covered by: FormattingTests.TestFixWhitespace(), incomplete
        /// <summary>
        /// Applies/removes some excess whitespace from the article
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string RemoveWhiteSpace(string articleText)
        {
            //Remove <br /> if followed by double newline
            articleText = Regex.Replace(articleText.Trim(), "<br ?/?>\r\n\r\n", "\r\n\r\n", RegexOptions.IgnoreCase);

            articleText = Regex.Replace(articleText, "\r\n(\r\n)+", "\r\n\r\n");

            articleText = Regex.Replace(articleText, "== ? ?\r\n\r\n==", "==\r\n==");
            articleText = Regex.Replace(articleText, @"==External links==[\r\n\s]*\*", "==External links==\r\n*");
            articleText = Regex.Replace(articleText, @"\r\n\r\n(\* ?\[?http)", "\r\n$1");

            articleText = Regex.Replace(articleText.Trim(), "----+$", "");

            if (articleText.Contains("\r\n|\r\n\r\n")) articleText = articleText.Replace("\r\n|\r\n\r\n", "\r\n|\r\n");
            if (articleText.Contains("\r\n\r\n|")) articleText = articleText.Replace("\r\n\r\n|", "\r\n|");

            return articleText.Trim();
        }

        // NOT covered
        /// <summary>
        /// Applies removes all excess whitespace from the article
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string RemoveAllWhiteSpace(string articleText)
        {//removes all whitespace
            articleText = articleText.Replace("\t", " ");
            articleText = RemoveWhiteSpace(articleText);

            articleText = articleText.Replace("\r\n\r\n*", "\r\n*");

            articleText = Regex.Replace(articleText, "  +", " ");
            articleText = Regex.Replace(articleText, " \r\n", "\r\n");

            articleText = Regex.Replace(articleText, "==\r\n\r\n", "==\r\n");
            articleText = Regex.Replace(articleText, @"==External links==[\r\n\s]*\*", "==External links==\r\n*");

            //fix bullet points – one space after them
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_arbitrary_spaces_after_bullet
            articleText = Regex.Replace(articleText, @"^([\*#]+) +", "$1", RegexOptions.Multiline);
            articleText = Regex.Replace(articleText, @"^([\*#]+)", "$1 ", RegexOptions.Multiline);

            //fix heading space
            articleText = Regex.Replace(articleText, "^(={1,4}) ?(.*?) ?(={1,4})$", "$1$2$3", RegexOptions.Multiline);

            //fix dash spacing
            articleText = Regex.Replace(articleText, " (–|—|&#15[01];|&[nm]dash;|&#821[12];|&#x201[34];) ", "$1");

            return articleText.Trim();
        }

        /// <summary>
        /// Fixes and improves syntax (such as html markup)
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public static string FixSyntax(string ArticleText, out bool NoChange)
        {
            string testText = ArticleText;
            ArticleText = FixSyntax(ArticleText);

            NoChange = (testText == ArticleText);
            return ArticleText;
        }
        // regexes for external link match on balanced bracket
        private static readonly Regex DoubleBracketAtStartOfExternalLink = new Regex(@"\[(\[https?:/(?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex DoubleBracketAtEndOfExternalLink = new Regex(@"(\[https?:/(?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\])\](?!\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex DoubleBracketAtEndOfExternalLinkWithinImage = new Regex(@"(\[https?:/(?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!)))\](?=\]{3})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private const string TemEnd = @"(\s*(?:\||\}\}))";
        private const string CitURL = @"(\{\{\s*cit[^{}]*?\|\s*url\s*=\s*)";
        private static readonly Regex BracketsAtBeginCiteTemplateURL = new Regex(CitURL + @"\[+\s*((?:(?:ht|f)tp://)?[^\[\]<>""\s]+?\s*)\]?" + TemEnd, RegexOptions.IgnoreCase);
        private static readonly Regex BracketsAtEndCiteTemplateURL = new Regex(CitURL + @"\[?\s*((?:(?:ht|f)tp://)?[^\[\]<>""\s]+?\s*)\]+" + TemEnd, RegexOptions.IgnoreCase);

        private static readonly Regex SyntaxRegex4 = new Regex(@"\[\[([^][]*?)\](?=[^\]]*?(?:$|\[|\n))", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex5 = new Regex(@"(?<=(?:^|\]|\n)[^\[]*?)\[([^][]*?)\]\](?!\])", RegexOptions.Compiled);

        private static readonly Regex SyntaxRegex6 = new Regex("\\[?\\[image:(http:\\/\\/.*?)\\]\\]?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex7 = new Regex("\\[\\[ (.*)?\\]\\]", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex8 = new Regex("\\[\\[([A-Za-z]*) \\]\\]", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex9 = new Regex("\\[\\[(.*)?_#(.*)\\]\\]", RegexOptions.Compiled);

        private static readonly Regex SyntaxRegexTemplate = new Regex("(\\{\\{[\\s]*)[Tt]emplate:(.*?\\}\\})", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex11 = new Regex(@"^([#\*:;]+.*?) *<[/\\]?br ?[/\\]?> *\r\n", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // make double spaces within wikilinks just single spaces
        private static readonly Regex SyntaxRegex12 = new Regex(@"(\[\[[^\[\]]+?)\s{2,}([^\[\]]+\]\])", RegexOptions.Compiled);

        private static readonly Regex SyntaxRegexItalic = new Regex("< ?i ?>(.*?)< ?/ ?i ?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexBold = new Regex("< ?b ?>(.*?)< ?/ ?b ?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Matches <p> tags only if current line does not start from ! or | (indicator of table cells)
        private static readonly Regex SyntaxRemoveParagraphs = new Regex(@"(?<!^[!\|].*)</? ?[Pp]>", RegexOptions.Multiline | RegexOptions.Compiled);
        // same for <br>
        private static readonly Regex SyntaxRemoveBr = new Regex(@"(?<!^[!\|].*)(<br[\s/]*> *){2,}", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex MultipleHttpInLink = new Regex(@"([\s\[>=])((?:ht|f)tp:?/+)(\2)+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex PipedExternalLink = new Regex(@"(\[\w+://[^\]\[<>\""\s]*?)\|''", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex MissingColonInHttpLink = new Regex(@"([\s\[>=](?:ht|f))tp//?:?(\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SingleTripleSlashInHttpLink = new Regex(@"([\s\[>=](?:ht|f))tp:(?:/|///)(\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex CellpaddingTypo = new Regex(@"({\s*\|\s*class\s*=\s*""wikitable[^}]*?)cel(?:lpa|pad?)ding\b", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex AccessdateTypo = new Regex(@"(\{\{\s*cit[^{}]*?\|\s*)ac(?:(?:ess?s?|cc?es|cesss|ccess)date|cessdare)(\s*=\s*)", RegexOptions.IgnoreCase);

        private static readonly Regex UppercaseCiteFields = new Regex(@"(\{\{(?:[Cc]ite\s*(?:web|book|news|journal|paper|press release|hansard|encyclopedia)|[Cc]itation)\b\s*[^{}]*\|\s*)(\w*?[A-Z]+\w*)(?<!ISBN)(\s*=\s*[^{}\|]{3,})");

        private static readonly Regex CiteFormatFieldTypo = new Regex(@"(\{\{\s*[Cc]it[^{}]*?\|\s*)(?:fprmat)(\s*=\s*)");

        //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_.3Cfont.3E_tags
        private static readonly Regex RemoveNoPropertyFontTags = new Regex(@"<font>([^<>]+)</font>", RegexOptions.IgnoreCase);

        // for fixing unbalanced brackets
        private static readonly Regex RefTemplateIncorrectBracesAtEnd = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>/]+?\s*)?>\s*)({{\s*[Cc]it[ae][^{}]+?)(?:}\]?|\)\))?(?=\s*</ref>)");
        private static readonly Regex RefExternalLinkUsingBraces = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>]+?\s*)?>\s*){{(\s*https?://[^{}\s\r\n]+)(\s+[^{}]+\s*)?}}(\s*</ref>)");
        private static readonly Regex TemplateIncorrectBracesAtStart = new Regex(@"(?:{\[|\[{)([^{}\[\]]+}})");
        private static readonly Regex CitationTemplateSingleBraceAtStart = new Regex(@"(?<=[^{])({\s*[Cc]it[ae])");
        private static readonly Regex ReferenceTemplateQuadBracesAtEnd = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>]+?\s*)?>\s*{{[^{}]+)}}(}}\s*</ref>)");
        private static readonly Regex CitationTemplateIncorrectBraceAtStart = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>]+?\s*)?>){\[([Cc]it[ae])");
        private static readonly Regex CitationTemplateIncorrectBracesAtEnd = new Regex(@"(<ref(?:\s*name\s*=[^{}<>]+?\s*)?>\s*{{[Cc]it[ae][^{}]+?)(?:}\]|\]}|{})(?=\s*</ref>)");
        private static readonly Regex RefExternalLinkMissingStartBracket = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>]+?\s*)?>[^{}\[\]<>]*?){?(https?://[^{}\[\]<>]+\][^{}\[\]<>]*</ref>)");
        private static readonly Regex RefExternalLinkMissingEndBracket = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>]+?\s*)?>[^{}\[\]<>]*?\[\s*https?://[^{}\[\]<>]+)(</ref>)");
        private static readonly Regex RefCitationMissingOpeningBraces = new Regex(@"(?<=<\s*ref(?:\s+name\s*=[^<>]*?)?\s*>\s*)\(?\(?(?=[Cc]it[ae][^{}]+}}\s*</ref>)");
        private static readonly Regex BracesWithinDefaultsort = new Regex(@"(?<={{DEFAULTSORT[^{}\[\]]+)[\]\[]+}}");

        // for correcting square brackets within external links
        private static readonly Regex SquareBracketsInExternalLinks = new Regex(@"(\[https?://(?>[^\[\]<>]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\])");

        // Covered by: LinkTests.TestFixSyntax(), incomplete
        /// <summary>
        /// Fixes and improves syntax (such as html markup)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixSyntax(string articleText)
        {
            //replace html with wiki syntax
            if (Regex.IsMatch(articleText, "</?i>", RegexOptions.IgnoreCase))
                articleText = SyntaxRegexItalic.Replace(articleText, "''$1''");

            if (Regex.IsMatch(articleText, "</?b>", RegexOptions.IgnoreCase))
                articleText = SyntaxRegexBold.Replace(articleText, "'''$1'''");

            articleText = Regex.Replace(articleText, "^<hr>|^----+", "----", RegexOptions.Multiline);

            //remove appearance of double line break
            articleText = Regex.Replace(articleText, "(^==?[^=]*==?)\r\n(\r\n)?----+", "$1", RegexOptions.Multiline);

            //remove unnecessary namespace
            articleText = SyntaxRegexTemplate.Replace(articleText, "$1$2");

            //remove <br> from lists
            articleText = SyntaxRegex11.Replace(articleText, "$1\r\n");

            //fix uneven bracketing on links
            articleText = DoubleBracketAtStartOfExternalLink.Replace(articleText, "$1");
            articleText = DoubleBracketAtEndOfExternalLink.Replace(articleText, "$1");
            articleText = DoubleBracketAtEndOfExternalLinkWithinImage.Replace(articleText, "$1");

            // (part) wikilinked/external linked URL in cite template, uses MediaWiki regex of [^\[\]<>""\s] for URL bit after http://
            articleText = BracketsAtBeginCiteTemplateURL.Replace(articleText, "$1$2$3");
            articleText = BracketsAtEndCiteTemplateURL.Replace(articleText, "$1$2$3");

            articleText = MultipleHttpInLink.Replace(articleText, "$1$2");

            articleText = PipedExternalLink.Replace(articleText, "$1 ''");

            //repair bad external links
            articleText = SyntaxRegex6.Replace(articleText, "[$1]");

            //repair bad internal links
            articleText = SyntaxRegex7.Replace(articleText, "[[$1]]");
            articleText = SyntaxRegex8.Replace(articleText, "[[$1]]");
            articleText = SyntaxRegex9.Replace(articleText, "[[$1#$2]]");
            articleText = SyntaxRegex12.Replace(articleText, @"$1 $2");

            if (!Regex.IsMatch(articleText, @"HTTP/\d\."))
            {
                articleText = MissingColonInHttpLink.Replace(articleText, "$1tp://$2");
                articleText = SingleTripleSlashInHttpLink.Replace(articleText, "$1tp://$2");
            }

            articleText = Regex.Replace(articleText, "ISBN: ?([0-9])", "ISBN $1");

            articleText = CellpaddingTypo.Replace(articleText, "$1cellpadding");

            articleText = AccessdateTypo.Replace(articleText, "$1accessdate$2");

            // {{cite web}} needs lower case field names; two loops in case a single template has multiple uppercase fields
            // restrict to en-wiki
            // exceptionally, 'ISBN' is allowed
            while (Variables.LangCode == LangCodeEnum.en)
            {
                foreach (Match m in UppercaseCiteFields.Matches(articleText))
                {
                    articleText = articleText.Replace(m.Value, m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value);
                }

                if (!UppercaseCiteFields.IsMatch(articleText))
                    break;
            }

            articleText = CiteFormatFieldTypo.Replace(articleText, "$1format$2");

            articleText = RemoveNoPropertyFontTags.Replace(articleText, "$1");

            // fixes for missing/unbalanced brackets
            articleText = RefTemplateIncorrectBracesAtEnd.Replace(articleText, @"$1}}");
            articleText = RefExternalLinkUsingBraces.Replace(articleText, @"[$1$2]$3");
            articleText = TemplateIncorrectBracesAtStart.Replace(articleText, @"{{$1");
            articleText = CitationTemplateSingleBraceAtStart.Replace(articleText, @"{$1");
            articleText = ReferenceTemplateQuadBracesAtEnd.Replace(articleText, @"$1");
            articleText = CitationTemplateIncorrectBraceAtStart.Replace(articleText, @"{{$1");
            articleText = CitationTemplateIncorrectBracesAtEnd.Replace(articleText, @"$1}}");
            articleText = RefExternalLinkMissingStartBracket.Replace(articleText, @"[$1");
            articleText = RefExternalLinkMissingEndBracket.Replace(articleText, @"]$1");
            articleText = RefCitationMissingOpeningBraces.Replace(articleText, @"{{");
            articleText = BracesWithinDefaultsort.Replace(articleText, @"}}");

            // fixes for square brackets used within external links
            foreach (Match m in SquareBracketsInExternalLinks.Matches(articleText))
            {
                // strip off leading [ and trailing ]
                string externalLink = Regex.Replace(m.Value, @"^\[(\s*http.*?)\]$", "$1", RegexOptions.Singleline);

                // if there are some brackets left then they need fixing; the mediawiki parser finishes the external link
                // at the first ] found
                if (externalLink.Contains("]"))
                {
                    // replace single ] with &#93; when used for brackets in the link description
                    externalLink = Regex.Replace(externalLink, @"([^]])\]([^]]|$)", @"$1&#93;$2");

                    articleText = articleText.Replace(m.Value, @"[" + externalLink + @"]");
                }
            }

            // needs to be applied after SquareBracketsInExternalLinks
            if (!Regex.IsMatch(articleText, "\\[\\[[Ii]mage:[^]]*http"))
            {
                articleText = SyntaxRegex4.Replace(articleText, "[[$1]]");
                articleText = SyntaxRegex5.Replace(articleText, "[[$1]]");
            }

            // if there are some unbalanced brackets, see whether we can fix them
            // the fixes applied might damage correct wiki syntax, hence are only applied if there are unbalanced brackets
            // of the right type
            int bracketLength = 0;
            string articleTextTemp = articleText;
            int unbalancedBracket = UnbalancedBrackets(articleText, ref bracketLength);
            if (unbalancedBracket > -1)
            {
                int firstUnbalancedBracket = unbalancedBracket;
                // if it's ]]_]_ then see if removing bracket makes it all balance
                if (bracketLength == 1 && articleTextTemp[unbalancedBracket].ToString().Equals(@"]") && 
                    articleTextTemp[unbalancedBracket - 1].ToString().Equals(@"]") && articleTextTemp[unbalancedBracket - 2].ToString().Equals(@"]"))
                    articleTextTemp = articleTextTemp.Remove(unbalancedBracket, 1);
                                
                if (bracketLength == 1)
                {
                    // if it's [[blah blah}word]]
                    articleTextTemp = Regex.Replace(articleTextTemp, @"(?<=\[\[[^\[\]{}<>\r\n\|]{1,50})}(?=[^\[\]{}<>\r\n\|]{1,50}\]\])", @"|");

                    // if it's (blah} then see if setting the } to a ) makes it all balance, but not |} which could be wikitables
                    articleTextTemp = Regex.Replace(articleTextTemp, @"(?<=\([^{}<>\(\)]+[^{}<>\(\)\|])}(?=[^{}])", @")");

                    // if it's [blah} then see if setting the } to a ] makes it all balance
                    articleTextTemp = Regex.Replace(articleTextTemp, @"(?<=\[[^{}<>\[\]]+[^{}<>\(\)\|])}(?=[^{}])", @"]");

                    // if it's {blah) then see if setting the { to a ( makes it all balance, but not {| which could be wikitables
                    articleTextTemp = Regex.Replace(articleTextTemp, @"(?<=[^{}<>]){(?=[^{}<>\(\)\|][^{}<>\(\)]+\)[^{}\(\)])", @"(");

                    // if it's ((word) then see if removing the extra opening round bracket makes it all balance
                    if (articleTextTemp.Length > unbalancedBracket && articleTextTemp[unbalancedBracket].ToString().Equals(@"(") && articleText[unbalancedBracket + 1].ToString().Equals(@"("))
                        articleTextTemp = articleTextTemp.Remove(unbalancedBracket, 1);

                    // if it's {[link]] or {[[link]] or [[[link]] then see if setting to [[ makes it all balance
                    articleTextTemp = Regex.Replace(articleTextTemp, @"(?<=[^\[\]{}<>])(?:{\[\[?|\[\[\[)(?=[^\[\]{}<>]+\]\])", @"[[");

                    // could be [[{link]]
                    articleTextTemp = Regex.Replace(articleTextTemp, @"(?<=\[\[){(?=[^{}\[\]<>]+\]\])", "");

                    // external link missing closing ]
                    articleTextTemp = Regex.Replace(articleTextTemp, @"(?<=^ *\* *\[ *https?://[^<>{}\[\]\r\n\s]+[^\[\]\r\n]*)(\s$)", "]$1", RegexOptions.Multiline);

                    // external link missing opening [
                    articleTextTemp = Regex.Replace(articleTextTemp, @"(?<=^ *\*) *(?=https?://[^<>{}\[\]\r\n\s]+[^\[\]\r\n]*\]\s$)", " [", RegexOptions.Multiline);
                }
                
                if (bracketLength == 2)
                {
                    // if it's on double curly brackets, see if one is missing e.g. {{foo} or {{foo]}
                    articleTextTemp = Regex.Replace(articleTextTemp, @"(?<={{[^{}<>]{1,400}[^{}<>\|])(?:\]}|}\]?)(?=[^{}])", @"}}");

                    // might be [[[[link]] or [[link]]]] so see if removing the two found square brackets makes it all balance
                    if(articleTextTemp.Substring(unbalancedBracket, Math.Min(4, articleTextTemp.Length - unbalancedBracket)).Equals("[[[[")
                        || articleTextTemp.Substring(Math.Max(0, unbalancedBracket-2), Math.Min(4, articleTextTemp.Length - unbalancedBracket)).Equals("]]]]"))                    
                        articleTextTemp = articleTextTemp.Remove(unbalancedBracket, 2);
                }

                unbalancedBracket = UnbalancedBrackets(articleTextTemp, ref bracketLength);
                // the change worked if unbalanced bracket location moved considerably (so this one fixed), or all brackets now balance
                if (unbalancedBracket < 0 || Math.Abs(unbalancedBracket - firstUnbalancedBracket) > 300)
                    articleText = articleTextTemp; 
            }

            return articleText.Trim();
        }

        // Covered by: LinkTests.TestCanonicalizeTitle(), incomplete
        /// <summary>
        /// returns URL-decoded link target
        /// </summary>
        public static string CanonicalizeTitle(string title)
        {
            // visible parts of links may contain crap we shouldn't modify, such as
            // refs and external links
            if (!Tools.IsValidTitle(title) || title.Contains(":/")) return title;

            //AnchorDecode(ref title); // disabled, breaks things such as [[Windows#Version_3.11]]

            string s = CanonicalizeTitleRaw(title);
            if (Variables.UnderscoredTitles.Contains(Tools.TurnFirstToUpper(s)))
            {
                return HttpUtility.UrlDecode(title.Replace("+", "%2B"))
                    .Trim(new[] { '_' });
            }
            return s;
        }

        private static readonly Regex DoubleCurlyBrackets = new Regex(@"{{((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}})");
        private static readonly Regex SingleCurlyBrackets = new Regex(@"{((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))})");
        private static readonly Regex DoubleSquareBrackets = new Regex(@"\[\[((?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\]\])");
        private static readonly Regex SingleSquareBrackets = new Regex(@"\[((?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\])");
        private static readonly Regex SingleRoundBrackets = new Regex(@"\(((?>[^\(\)]+|\((?<DEPTH>)|\)(?<-DEPTH>))*(?(DEPTH)(?!))\))");

        /// <summary>
        /// Checks the article text for unbalanced brackets, either square or curly
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="bracketLength">integer to hold length of unbalanced bracket found</param>
        /// <returns>Index of any unbalanced brackets found</returns>
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Missing_opening_or_closing_brackets.2C_table_and_template_markup_.28WikiProject_Check_Wikipedia_.23_10.2C_28.2C_43.2C_46.2C_47.29
        public static int UnbalancedBrackets(string articleText, ref int bracketLength)
        {
            bracketLength = 2;

            int unbalancedfound = UnbalancedBrackets(articleText, @"{{", @"}}", DoubleCurlyBrackets);
            if (unbalancedfound > -1)
                return unbalancedfound;

            unbalancedfound = UnbalancedBrackets(articleText, @"[[", @"]]", DoubleSquareBrackets);
            if (unbalancedfound > -1)
                return unbalancedfound;

            bracketLength = 1;
            
            unbalancedfound = UnbalancedBrackets(articleText, @"{", @"}", SingleCurlyBrackets);
            if (unbalancedfound > -1)
                return unbalancedfound;
            
            unbalancedfound = UnbalancedBrackets(articleText, @"[", @"]", SingleSquareBrackets);
            if (unbalancedfound > -1)
                return unbalancedfound;

            unbalancedfound = UnbalancedBrackets(articleText, @"(", @")", SingleRoundBrackets);
            if (unbalancedfound > -1)
                return unbalancedfound;

            return -1;
        }

        /// <summary>
        /// Checks the article text for unbalanced brackets of the input type
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="openingBrackets"></param>
        /// <param name="closingBrackets"></param>
        /// <param name="bracketsRegex"></param>
        /// <returns>Index of any unbalanced brackets found</returns>
        private static int UnbalancedBrackets(string articleText, string openingBrackets, string closingBrackets, Regex bracketsRegex)
        {
            // &#93; is used to replace the ] in external link text, which gives correct markup
            // replace [...&#93; with spaces to avoid matching as unbalanced brackets
            articleText = Regex.Replace(articleText, @"[^\[\]{}<>]\[[^\[\]{}<>]*?&#93;", @" ");

            if (Regex.Matches(articleText, Regex.Escape(openingBrackets)).Count != Regex.Matches(articleText, Regex.Escape(closingBrackets)).Count)
            {
                // remove all <math>, <code> stuff etc. where curly brackets are used in singles and pairs
                foreach (Match m in WikiRegexes.MathPreSourceCodeComments.Matches(articleText))
                {
                    articleText = articleText.Replace(m.Value, Tools.ReplaceWithSpaces(m.Value));
                }

                if (openingBrackets.Equals(@"["))
                {
                    // need to remove double square brackets first
                    foreach (Match m in DoubleSquareBrackets.Matches(articleText))
                    {
                        articleText = articleText.Replace(m.Value, Tools.ReplaceWithSpaces(m.Value));
                    }
                }

                if (openingBrackets.Equals(@"{"))
                {
                    // need to remove double curly brackets first
                    foreach (Match m in DoubleCurlyBrackets.Matches(articleText))
                    {
                        articleText = articleText.Replace(m.Value, Tools.ReplaceWithSpaces(m.Value));
                    }
                }

                // replace all the valid balanced bracket sets with spaces
                foreach (Match m in bracketsRegex.Matches(articleText))
                {
                    articleText = articleText.Replace(m.Value, Tools.ReplaceWithSpaces(m.Value));
                }

                // now return the unbalanced one that's left
                int open = Regex.Matches(articleText, Regex.Escape(openingBrackets)).Count;
                int closed = Regex.Matches(articleText, Regex.Escape(closingBrackets)).Count;

                if (open == 0 && closed >= 1)
                    return articleText.IndexOf(closingBrackets);
                
                if (open >= 1 && closed == 0)
                    return articleText.IndexOf(openingBrackets);
                
                return -1;
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private static bool IsHex(byte b)
        {
            return ((b >= '0' && b <= '9') || (b >= 'A' && b <= 'F'));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static byte DecodeHex(byte a, byte b)
        {
            return byte.Parse(new string(new[] { (char)a, (char)b }), System.Globalization.NumberStyles.HexNumber);
        }

        // NOT covered, unused
        /// <summary>
        /// Decodes anchor-encoded links. Don't use unless rewrittten not to screw stuff like [[Windows#3.11]]
        /// </summary>
        public static void AnchorDecode(ref string link)
        {
            Match m = WikiRegexes.AnchorEncodedLink.Match(link);
            if (!m.Success) return;

            string anchor = m.Value.Replace('_', ' ');
            byte[] src = Encoding.UTF8.GetBytes(anchor);
            byte[] dest = (byte[])src.Clone();

            int SrcCount, DestCount = 0;

            for (SrcCount = 0; SrcCount < src.Length; SrcCount++)
            {
                if (src[SrcCount] != '.' || !(SrcCount + 3 <= src.Length &&
                    IsHex(src[SrcCount + 1]) && IsHex(src[SrcCount + 2])))
                    // then
                    dest[DestCount] = src[SrcCount];
                else
                {
                    dest[DestCount] = DecodeHex(src[SrcCount + 1], src[SrcCount + 2]);
                    SrcCount += 2;
                }

                DestCount++;
            }

            link = link.Replace(m.Value, Encoding.UTF8.GetString(dest, 0, DestCount));
        }

        private static readonly Regex LinkWhitespace1 = new Regex(@" \[\[ ([^\]]{1,30})\]\]", RegexOptions.Compiled);
        private static readonly Regex LinkWhitespace2 = new Regex(@"(?<=\w)\[\[ ([^\]]{1,30})\]\]", RegexOptions.Compiled);
        private static readonly Regex LinkWhitespace3 = new Regex(@"\[\[([^\]]{1,30}?) {2,10}([^\]]{1,30})\]\]", RegexOptions.Compiled);
        private static readonly Regex LinkWhitespace4 = new Regex(@"\[\[([^\]\|]{1,30}) \]\] ", RegexOptions.Compiled);
        private static readonly Regex LinkWhitespace5 = new Regex(@"\[\[([^\]]{1,30}) \]\](?=\w)", RegexOptions.Compiled);

        private static readonly Regex DateLinkWhitespace1 = new Regex(@"\b(\[\[\d\d? (?:January|February|March|April|May|June|July|August|September|October|November|December)\]\]),? {0,2}(\[\[\d{1,4}\]\])\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex DateLinkWhitespace2 = new Regex(@"\b(\[\[(?:January|February|March|April|May|June|July|August|September|October|November|December) \d\d?\]\]),? {0,2}(\[\[\d{1,4}\]\])\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Covered by LinkTests.TestFixLinkWhitespace()
        /// <summary>
        /// Fix leading, trailing and middle spaces in Wikilinks
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <param name="articleTitle">The article title.</param>
        /// <returns>The modified article text.</returns>
        public static string FixLinkWhitespace(string articleText, string articleTitle)
        {
            //remove undesirable space from beginning of wikilink (space before wikilink) - parse this line first
            if (LinkWhitespace1.Match(articleText).Success)
                articleText = LinkWhitespace1.Replace(articleText, " [[$1]]");

            //remove undesirable space from beginning of wikilink and move it outside link - parse this line second
            if (LinkWhitespace2.Match(articleText).Success)
                articleText = LinkWhitespace2.Replace(articleText, " [[$1]]");

            //remove undesirable double space from middle of wikilink (up to 61 characters in wikilink)
            if (LinkWhitespace3.Match(articleText).Success)
                articleText = LinkWhitespace3.Replace(articleText, "[[$1 $2]]");

            //remove undesirable space from end of wikilink (space after wikilink) - parse this line first
            if (LinkWhitespace4.Match(articleText).Success)
                articleText = LinkWhitespace4.Replace(articleText, "[[$1]] ");

            //remove undesirable space from end of wikilink and move it outside link - parse this line second
            if (LinkWhitespace5.Match(articleText).Success)
                articleText = LinkWhitespace5.Replace(articleText, "[[$1]] ");

            //remove undesirable double space between links in date (day first)
            if (DateLinkWhitespace1.Match(articleText).Success)
                articleText = DateLinkWhitespace1.Replace(articleText, "$1 $2");

            //remove undesirable double space between links in date (day second)
            if (DateLinkWhitespace2.Match(articleText).Success)
                articleText = DateLinkWhitespace2.Replace(articleText, "$1 $2");

            // correct [[page# section]] to [[page#section]]
            Regex sectionLinkWhitespace = new Regex(@"(\[\[" + Regex.Escape(articleTitle) + @"\#)\s+([^\[\]]+\]\])");

            return sectionLinkWhitespace.IsMatch(articleText)
                       ? sectionLinkWhitespace.Replace(articleText, "$1$2")
                       : articleText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public static string FixLinkWhitespace(string articleText)
        {
            return FixLinkWhitespace(articleText, "test");
        }

        // Partially covered by SelfLinkRemoval
        /// <summary>
        /// Fixes link syntax, including removal of self links
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle"></param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public static string FixLinks(string articleText, string articleTitle, out bool noChange)
        {
            string articleTextAtStart = articleText;
            string escTitle = Regex.Escape(articleTitle);

            // remove any self-links, but not other links with different capitaliastion e.g. [[Foo]] vs [[FOO]]
            articleText = Regex.Replace(articleText, @"\[\[\s*" + escTitle + @"\s*\]\]", articleTitle);
            articleText = Regex.Replace(articleText, @"\[\[\s*" + Tools.TurnFirstToLower(escTitle) + @"\s*\]\]", Tools.TurnFirstToLower(articleTitle));

            // remove piped self links by leaving target
            articleText = Regex.Replace(articleText, @"\[\[\s*" + escTitle + @"\s*\|([^\]]+)\]\]", "$1");
            articleText = Regex.Replace(articleText, @"\[\[\s*" + Tools.TurnFirstToLower(escTitle) + @"\s*\|([^\]]+)\]\]", "$1");

            // clean up wikilinks: replace underscores, percentages and URL encoded accents etc.
            StringBuilder sb = new StringBuilder(articleText, (articleText.Length * 11) / 10);

            foreach (Match m in WikiRegexes.WikiLink.Matches(articleText))
            {
                if (m.Groups[1].Value.Length > 0)
                {
                    string y = m.Value.Replace(m.Groups[1].Value, CanonicalizeTitle(m.Groups[1].Value));

                    if (y != m.Value) sb = sb.Replace(m.Value, y);
                }
            }

            noChange = (sb.ToString() == articleTextAtStart);

            return sb.ToString();
        }

        /// <summary>
        /// Performs URL-decoding of a page title, trimming all whitespace
        /// </summary>
        public static string CanonicalizeTitleRaw(string title)
        {
            return CanonicalizeTitleRaw(title, true);
        }

        //NOT covered
        /// <summary>
        /// performs URL-decoding of a page title
        /// </summary>
        /// <param name="title">title to normalise</param>
        /// <param name="trim">whether whitespace should be trimmed</param>
        public static string CanonicalizeTitleRaw(string title, bool trim)
        {
            StringBuilder sb = new StringBuilder(title);//, (int)(title.Length * 1.1
            sb.Replace("+", "%2B");
            sb.Replace('_', ' ');
            title = HttpUtility.UrlDecode(sb.ToString());
            return trim ? title.Trim() : title;
        }

        // Covered by: UtilityFunctionTests.IsCorrectEditSummary()
        /// <summary>
        /// returns true if given string has matching double square brackets
        /// </summary>
        public static bool IsCorrectEditSummary(string s)
        {
            bool res = true;

            if (s.Length > 255) return false;
            int pos = s.IndexOf("[[");
            while (pos >= 0)
            {
                s = s.Remove(0, pos);

                pos = res ? s.IndexOf("]]") : s.IndexOf("[[");

                res = !res;
            }
            return res;
        }

        // Covered by: LinkTests.TestSimplifyLinks()
        /// <summary>
        /// Simplifies some links in article wiki text such as changing [[Dog|Dogs]] to [[Dog]]s
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The simplified article text.</returns>
        public static string SimplifyLinks(string articleText)
        {
            string a = "", b = "";

            try
            {
                foreach (Match m in WikiRegexes.PipedWikiLink.Matches(articleText))
                {
                    string n = m.Value;
                    a = m.Groups[1].Value.Trim();

                    b = (Namespace.Determine(a) != Namespace.Category)
                            ? m.Groups[2].Value.Trim()
                            : m.Groups[2].Value.TrimEnd(new[] { ' ' });

                    if (b.Length == 0) continue;

                    if (a == b || Tools.TurnFirstToLower(a) == b)
                    {
                        articleText = articleText.Replace(n, "[[" + b + "]]");
                    }
                    else if (Tools.TurnFirstToLower(b).StartsWith(Tools.TurnFirstToLower(a), StringComparison.Ordinal))
                    {
                        bool doBreak = false;
                        foreach (char ch in b.Remove(0, a.Length))
                        {
                            if (!char.IsLower(ch))
                            {
                                doBreak = true;
                                break;
                            }
                        }
                        if (doBreak) continue;
                        articleText = articleText.Replace(n, "[[" + b.Substring(0, a.Length) + "]]" + b.Substring(a.Length));
                    }
                    else
                    {
                        string newlink = "[[" + a + "|" + b + "]]";

                        if (newlink != n)
                            articleText = articleText.Replace(n, newlink);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + @"
a='" + a + "',  b='" + b + "'", "SimplifyLinks error");
            }

            return articleText;
        }

        // Covered by: LinkTests.TestStickyLinks()
        /// <summary>
        /// Joins nearby words with links
        ///   e.g. "[[Russian literature|Russian]] literature" to "[[Russian literature]]"
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns>Processed wikitext</returns>
        public static string StickyLinks(string articleText)
        {
            string a = "", b = "";
            try
            {
                foreach (Match m in WikiRegexes.PipedWikiLink.Matches(articleText))
                {
                    a = m.Groups[1].Value;
                    b = m.Groups[2].Value;

                    if (b.Trim().Length == 0 || a.Contains(",")) continue;

                    if (Tools.TurnFirstToLower(a).StartsWith(Tools.TurnFirstToLower(b), StringComparison.Ordinal))
                    {
                        bool hasSpace = a[b.Length] == ' ';
                        string search = @"\[\[" + Regex.Escape(a) + @"\|" + Regex.Escape(b) +
                            @"\]\]" + (hasSpace ? "[ ]+" : "") + Regex.Escape(a.Remove(0,
                            b.Length + (hasSpace ? 1 : 0))) + @"\b";

                        //first char should be capitalized like in the visible part of the link
                        a = a.Remove(0, 1).Insert(0, b[0] + "");
                        articleText = Regex.Replace(articleText, search, "[[" + a + @"]]");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + @"
a='" + a + "',  b='" + b + "'", "StickyLinks error");
            }

            return articleText;
        }

        private static readonly Regex RegexMainArticle = new Regex(@"^:?'{0,5}Main article:\s?'{0,5}\[\[([^\|\[\]]*?)(\|([^\[\]]*?))?\]\]\.?'{0,5}\.?\s*?(?=($|[\r\n]))", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

        // Covered by: FixMainArticleTests
        /// <summary>
        /// Fixes instances of ''Main Article: xxx'' to use {{main|xxx}}
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string FixMainArticle(string articleText)
        {
            return RegexMainArticle.Match(articleText).Groups[2].Value.Length == 0
                       ? RegexMainArticle.Replace(articleText, "{{main|$1}}")
                       : RegexMainArticle.Replace(articleText, "{{main|$1|l1=$3}}");
        }

        // Covered by LinkTests.TestFixEmptyLinksAndTemplates()
        /// <summary>
        /// Removes Empty Links and Template Links
        /// Will Cater for [[]], [[Image:]], [[:Category:]], [[Category:]] and {{}}
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string FixEmptyLinksAndTemplates(string articleText)
        {
            foreach (Match link in WikiRegexes.EmptyLink.Matches(articleText))
            {
                string trim = link.Groups[2].Value.Trim();
                if (string.IsNullOrEmpty(trim) || trim == "|" + Variables.NamespacesCaseInsensitive[Namespace.Image] ||
                    trim == "|" + Variables.NamespacesCaseInsensitive[Namespace.Category] || trim == "|")
                    articleText = articleText.Replace("[[" + link.Groups[1].Value + link.Groups[2].Value + "]]", "");
            }

            if (WikiRegexes.EmptyTemplate.Match(articleText).Success)
                articleText = WikiRegexes.EmptyTemplate.Replace(articleText, "");

            return articleText;
        }

        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public static string BulletExternalLinks(string articleText, out bool noChange)
        {
            string testText = articleText;
            articleText = BulletExternalLinks(articleText);

            noChange = (testText == articleText);

            return articleText;
        }

        private static readonly HideText BulletExternalHider = new HideText(false, true, false);

        // Covered by: LinkTests.TestBulletExternalLinks()
        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string BulletExternalLinks(string articleText)
        {
            Match m = Regex.Match(articleText, @"=\s*(?:external)?\s*links\s*=", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

            if (!m.Success)
                return articleText;

            int intStart = m.Index;

            string articleTextSubstring = articleText.Substring(intStart);
            articleText = articleText.Substring(0, intStart);
            articleTextSubstring = BulletExternalHider.HideMore(articleTextSubstring);
            articleTextSubstring = Regex.Replace(articleTextSubstring, "(\r\n|\n)?(\r\n|\n)(\\[?http)", "$2* $3");

            return articleText + BulletExternalHider.AddBackMore(articleTextSubstring);
        }

        // Covered by: LinkTests.TestFixCategories()
        /// <summary>
        /// Fix common spacing/capitalisation errors in categories; remove diacritics and trailing whitespace from sortkeys (not leading whitespace)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixCategories(string articleText)
        {
            string cat = "[[" + Variables.Namespaces[Namespace.Category];

            // fix extra brackets: three or more at end
            articleText = Regex.Replace(articleText, @"(?<=" + Regex.Escape(cat) + @"[^\r\n\[\]{}<>]+\]\])\]+", "");
            // three or more at start
            articleText = Regex.Replace(articleText, @"\[+(?=" + Regex.Escape(cat) + @"[^\r\n\[\]{}<>]+\]\])", "");

            foreach (Match m in WikiRegexes.LooseCategory.Matches(articleText))
            {
                if (!Tools.IsValidTitle(m.Groups[1].Value)) continue;
                string x = cat + Tools.TurnFirstToUpper(CanonicalizeTitleRaw(m.Groups[1].Value, false).Trim()) + Regex.Replace(Tools.RemoveDiacritics(m.Groups[2].Value), @"(\w+)\s+$", "$1") + "]]";
                if (x != m.Value) articleText = articleText.Replace(m.Value, x);
            }

            return articleText;
        }

        // Covered by: ImageTests.BasicImprovements(), incomplete
        /// <summary>
        /// Fix common spacing/capitalisation errors in images
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixImages(string articleText)
        {
            foreach (Match m in WikiRegexes.LooseImage.Matches(articleText))
            {
                // only apply underscore/URL encoding fixes to image name (group 2)
                string x = "[[" + Namespace.Normalize(m.Groups[1].Value, 6) + CanonicalizeTitle(m.Groups[2].Value).Trim() + m.Groups[3].Value.Trim() + "]]";
                articleText = articleText.Replace(m.Value, x);
            }

            return articleText;
        }

        private static readonly Regex Temperature = new Regex(@"([º°](&nbsp;|)|(&deg;|&ordm;)(&nbsp;|))\s*([CcFf])([^A-Za-z])", RegexOptions.Compiled);

        // NOT covered
        /// <summary>
        /// Fix bad Temperatures
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixTemperatures(string articleText)
        {
            foreach (Match m in Temperature.Matches(articleText))
                articleText = articleText.Replace(m.ToString(), "°" + m.Groups[5].Value.ToUpper() + m.Groups[6].Value);
            return articleText;
        }

        /// <summary>
        /// Apply non-breaking spaces for abbreviated SI units
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixNonBreakingSpaces(string articleText)
        {
            // hide items in quotes etc., though this may also hide items within infoboxes etc.
            articleText = HideMoreText(articleText);

            articleText = WikiRegexes.SiUnitsWithoutNonBreakingSpaces.Replace(articleText, "$1&nbsp;$2");

            return AddBackMoreText(articleText);
        }

        /// <summary>
        /// regex that Matches every template, for GetTemplate
        /// </summary>
        public const string EveryTemplate = @"[^\|\{\}]+";

        // NOT covered
        /// <summary>
        /// extracts template using the given match
        /// </summary>
        private static string ExtractTemplate(string articleText, Match m)
        {
            int i = m.Index + m.Groups[1].Length;

            int brackets = 2;
            while (i < articleText.Length)
            {
                switch (articleText[i])
                {
                    // only sequences of 2 and more brackets should be counted
                    case '{':
                        if ((articleText[i - 1] == '{') || (i + 1 < articleText.Length &&
                            articleText[i + 1] == '{')) brackets++;
                        break;
                    case '}':
                        if ((articleText[i - 1] == '}') || (i + 1 < articleText.Length &&
                            articleText[i + 1] == '}'))
                        {
                            brackets--;
                            if (brackets == 0) return articleText.Substring(m.Index, i - m.Index + 1);
                        }
                        break;
                }
                i++;
            }
            return "";
        }

        // NOT covered
        /// <summary>
        /// finds first occurence of a given template in article text
        /// handles nested templates correctly
        /// </summary>
        /// <param name="articleText">source text</param>
        /// <param name="template">name of template, can be regex without a group capture</param>
        /// <returns>template with all params, enclosed in curly brackets</returns>
        public static string GetTemplate(string articleText, string template)
        {
            articleText = WikiRegexes.Nowiki.Replace(articleText, "");
            articleText = WikiRegexes.Comments.Replace(articleText, "");
            Regex search = new Regex(@"(\{\{\s*" + template + @"\s*)(?:\||\})", RegexOptions.Singleline);

            Match m = search.Match(articleText);

            return m.Success ? ExtractTemplate(articleText, m) : "";
        }

        // NOT covered
        /// <summary>
        /// finds every occurence of a given template in article text
        /// handles nested templates correctly
        /// </summary>
        /// <param name="articleText">source text</param>
        /// <param name="template">name of template, can be regex without a group capture</param>
        /// <returns>template with all params, enclosed in curly brackets</returns>
        public static List<Match> GetTemplates(string articleText, string template)
        {
            MatchCollection nw = WikiRegexes.Nowiki.Matches(articleText);
            MatchCollection cm = WikiRegexes.Comments.Matches(articleText);
            Regex search = new Regex(@"(\{\{\s*" + template + @"\s*)[\|\}]",
                RegexOptions.Singleline);

            List<Match> res = new List<Match>();

            int pos = 0;
            foreach (Match m in search.Matches(articleText))
            {
                if (m.Index < pos) continue;
                foreach (Match m2 in nw) if (m.Index > m2.Index &&
                    m.Index < m2.Index + m2.Length) continue;
                foreach (Match m2 in cm) if (m.Index > m2.Index &&
                    m.Index < m2.Index + m2.Length) continue;

                string s = ExtractTemplate(articleText, m);
                if (string.IsNullOrEmpty(s)) break;
                pos = m.Index + s.Length;
                Match mres = m;
                foreach (Match m2 in Regex.Matches(articleText, Regex.Escape(s)))
                {
                    if (m2.Index == m.Index)
                    {
                        mres = m2;
                        break;
                    }
                }
                res.Add(mres);
            }
            return res;
        }

        /// <summary>
        /// get template name from template call, e.g. "{{template:foobar|123}}"
        ///  to "foobar"
        /// </summary>
        public static string GetTemplateName(string call)
        {
            return WikiRegexes.TemplateCall.Match(call).Groups[1].Value;
        }

        // NOT covered
        /// <summary>
        /// If fromSetting is true, get template name from a setting, i.e. strip formatting/template: call *if any*. If false, passes through to GetTemplateName(string call)
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="fromSetting"></param>
        public static string GetTemplateName(string setting, bool fromSetting)
        {
            if (fromSetting)
            {
                setting = setting.Trim();
                if (string.IsNullOrEmpty(setting)) return "";

                string gtn = GetTemplateName(setting).Trim();
                return string.IsNullOrEmpty(gtn) ? setting : gtn;
            }

            return GetTemplateName(setting);
        }

        //Covered by: UtilityFunctionTests.RemoveEmptyComments()
        /// <summary>
        /// Removes comments with nothing/only whitespace between tags
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text (removed empty comments).</returns>
        public static string RemoveEmptyComments(string articleText)
        {
            return WikiRegexes.EmptyComments.Replace(articleText, "");
        }
        #endregion

        #region other functions

        /// <summary>
        /// Converts HTML entities to unicode, with some deliberate exceptions
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string Unicodify(string articleText, out bool noChange)
        {
            string testText = articleText;
            articleText = Unicodify(articleText);

            noChange = (testText == articleText);

            return articleText;
        }

        // Covered by: UnicodifyTests
        /// <summary>
        /// Converts HTML entities to unicode, with some deliberate exceptions
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string Unicodify(string articleText)
        {
            if (Regex.IsMatch(articleText, "<[Mm]ath>"))
                return articleText;

            articleText = Regex.Replace(articleText, "&#150;|&#8211;|&#x2013;", "&ndash;");
            articleText = Regex.Replace(articleText, "&#151;|&#8212;|&#x2014;", "&mdash;");
            articleText = articleText.Replace(" &amp; ", " & ");
            articleText = articleText.Replace("&amp;", "&amp;amp;");
            articleText = articleText.Replace("&#153;", "™");
            articleText = articleText.Replace("&#149;", "•");

            foreach (KeyValuePair<Regex, string> k in RegexUnicode)
            {
                articleText = k.Key.Replace(articleText, k.Value);
            }
            try
            {
                articleText = HttpUtility.HtmlDecode(articleText);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }

            return articleText;
        }

        // Covered by: BoldTitleTests
        /// <summary>
        /// '''Emboldens''' the first occurence of the article title, if not already bold
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The title of the article.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string BoldTitle(string articleText, string articleTitle, out bool noChange)
        {
            noChange = true;
            string escTitle = Regex.Escape(articleTitle);
            string escTitleNoBrackets = Regex.Escape(Regex.Replace(articleTitle, @" \(.*?\)$", ""));

            string articleTextHidden = HideMoreText(articleText);

            bool ignoredNoChange;
            // remove any self-links
            articleText = FixLinks(articleText, articleTitle, out ignoredNoChange);

            string articleTextAtStart = articleText;

            // first quick check: ignore articles with some bold in first 5% of hidemore article
            int fivepc = articleTextHidden.Length / 20;
            //ArticleText5.Length
            if (articleTextHidden.Substring(0, fivepc).Contains("'''"))
                return articleTextAtStart;

            // ignore date articles (date in American or international format)
            if (WikiRegexes.Dates2.IsMatch(articleTitle) || WikiRegexes.Dates.IsMatch(articleTitle))
                return articleTextAtStart;            

            Regex boldTitleAlready1 = new Regex(@"'''\s*(" + escTitle + "|" + Tools.TurnFirstToLower(escTitle) + @")\s*'''");
            Regex boldTitleAlready2 = new Regex(@"'''\s*(" + escTitleNoBrackets + "|" + Tools.TurnFirstToLower(escTitleNoBrackets) + @")\s*'''");
            Regex boldTitleAlready3 = new Regex(@"^\s*({{[^\{\}]+}}\s*)*'''('')?\s*\w");

            //if title in bold already exists in article, or page starts with something in bold, don't change anything
            if (boldTitleAlready1.IsMatch(articleText) || boldTitleAlready2.IsMatch(articleText)
                || boldTitleAlready3.IsMatch(articleText))
                return articleTextAtStart;

            Regex regexBold = new Regex(@"([^\[]|^)(" + escTitle + "|" + Tools.TurnFirstToLower(escTitle) + ")([ ,.:;])");
            Regex regexBoldNoBrackets = new Regex(@"([^\[]|^)(" + escTitleNoBrackets + "|" + Tools.TurnFirstToLower(escTitleNoBrackets) + ")([ ,.:;])");

            articleTextHidden = HideMoreText(articleText);

            // first try title with brackets removed
            if (regexBoldNoBrackets.IsMatch(articleTextHidden))
            {
                articleText = regexBoldNoBrackets.Replace(articleTextHidden, "$1'''$2'''$3", 1);
                articleText = AddBackMoreText(articleText);

                // check that the bold added is the first bit in bold in the main body of the article
                if (AddedBoldIsValid(articleText, escTitleNoBrackets))
                {
                    noChange = false;
                    return articleText;
                }
                return articleTextAtStart;
            }

            if (regexBold.IsMatch(articleTextHidden))
            {
                articleText = regexBold.Replace(articleTextHidden, "$1'''$2'''$3", 1);
                articleText = AddBackMoreText(articleText);

                // check that the bold added is the first bit in bold in the main body of the article
                if (AddedBoldIsValid(articleText, escTitle))
                {
                    noChange = false;
                    return articleText;
                }
            }
            return articleTextAtStart;
        }

        private static readonly Regex RegexFirstBold = new Regex(@"^(.*?)'''", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Checks that the bold just added to the article is the first bold in the article, and that it's within the first 5% of the HideMore article
        /// </summary>
        private bool AddedBoldIsValid(string articleText, string escapedTitle)
        {
            Regex regexBoldAdded = new Regex(@"^(.*?)'''" + escapedTitle, RegexOptions.Singleline);

            int boldAddedPos = regexBoldAdded.Match(articleText).Length - Regex.Unescape(escapedTitle).Length;

            int firstBoldPos = RegexFirstBold.Match(articleText).Length;

            articleText = HideMoreText(articleText);

            // was bold added in first 5% of article?
            bool inFirst5Percent = articleText.Substring(0, articleText.Length / 20).Contains("'''");

            // check that the bold added is the first bit in bold in the main body of the article, and in first 5% of HideMore article
            return inFirst5Percent && boldAddedPos <= firstBoldPos;
        }

        /// <summary>
        /// Replaces an image in the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="oldImage">The old image to replace.</param>
        /// <param name="newImage">The new image.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public static string ReplaceImage(string oldImage, string newImage, string articleText, out bool noChange)
        {
            string testText = articleText;
            articleText = ReplaceImage(oldImage, newImage, articleText);

            noChange = (testText == articleText);

            return articleText;
        }

        /// <summary>
        /// Replaces an iamge in the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="oldImage">The old image to replace.</param>
        /// <param name="newImage">The new image.</param>
        /// <returns>The new article text.</returns>
        public static string ReplaceImage(string oldImage, string newImage, string articleText)
        {
            articleText = FixImages(articleText);

            //remove image prefix
            oldImage = Tools.WikiDecode(Regex.Replace(oldImage, "^" + Variables.Namespaces[Namespace.File], "", RegexOptions.IgnoreCase));
            newImage = Tools.WikiDecode(Regex.Replace(newImage, "^" + Variables.Namespaces[Namespace.File], "", RegexOptions.IgnoreCase));

            oldImage = Regex.Escape(oldImage).Replace("\\ ", "[ _]");

            oldImage = "((?i:" + WikiRegexes.GenerateNamespaceRegex(Namespace.File, Namespace.Media)
                + @"))\s*:\s*" + Tools.CaseInsensitive(oldImage);
            newImage = "$1:" + newImage;

            articleText = Regex.Replace(articleText, oldImage, newImage);

            return articleText;
        }

        /// <summary>
        /// Removes an image from the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="image">The image to remove.</param>
        /// <param name="commentOut"></param>
        /// <param name="comment"></param>
        /// <returns>The new article text.</returns>
        public static string RemoveImage(string image, string articleText, bool commentOut, string comment)
        {
            //remove image prefix
            image = Tools.WikiDecode(Regex.Replace(image, "^"
                + Variables.NamespacesCaseInsensitive[Namespace.File], "", RegexOptions.IgnoreCase));
            image = Tools.CaseInsensitive(HttpUtility.UrlDecode(Regex.Escape(image).Replace("\\ ", "[ _]")));

            articleText = FixImages(articleText);

            Regex r = new Regex(@"\[\[\s*:?\s*(?i:"
                + WikiRegexes.GenerateNamespaceRegex(Namespace.File, Namespace.Media)
                + @")\s*:\s*" + image + @".*\]\]");

            MatchCollection n = r.Matches(articleText);
            if (n.Count > 0)
            {
                foreach (Match m in n)
                {
                    string match = m.Value;

                    int i = 0;
                    int j = 0;

                    foreach (char c in match)
                    {
                        if (c == '[')
                            j++;
                        else if (c == ']')
                            j--;

                        i++;

                        if (j == 0)
                        {
                            if (match.Length > i)
                                match = match.Remove(i);

                            Regex t = new Regex(Regex.Escape(match));

                            articleText = commentOut
                                              ? t.Replace(articleText, "<!-- " + comment + " " + match + " -->", 1, m.Index)
                                              : t.Replace(articleText, "", 1);

                            break;
                        }
                    }
                }
            }
            else
            {
                r = new Regex("(" + Variables.NamespacesCaseInsensitive[Namespace.File] + ")?" + image);
                n = r.Matches(articleText);

                foreach (Match m in n)
                {
                    Regex t = new Regex(Regex.Escape(m.Value));

                    articleText = commentOut
                                      ? t.Replace(articleText, "<!-- " + comment + " $0 -->", 1, m.Index)
                                      : t.Replace(articleText, "", 1, m.Index);
                }
            }

            return articleText;
        }

        /// <summary>
        /// Removes an iamge in the article.
        /// </summary>
        /// <param name="image">The image to remove.</param>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="comment"></param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <param name="commentOut"></param>
        /// <returns>The new article text.</returns>
        public static string RemoveImage(string image, string articleText, bool commentOut, string comment, out bool noChange)
        {
            string testText = articleText;
            articleText = RemoveImage(image, articleText, commentOut, comment);

            noChange = (testText == articleText);

            return articleText;
        }

        /// <summary>
        /// Adds the category to the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="newCategory">The new category.</param>
        /// <param name="articleTitle"></param>
        /// <param name="noChange"></param>
        /// <returns>The article text.</returns>
        public string AddCategory(string newCategory, string articleText, string articleTitle, out bool noChange)
        {
            string testText = articleText;
            articleText = AddCategory(newCategory, articleText, articleTitle);

            noChange = (testText == articleText);

            return articleText;
        }

        // Covered by: RecategorizerTests.Addition()
        /// <summary>
        /// Adds the category to the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="newCategory">The new category.</param>
        /// <param name="articleTitle"></param>
        /// <returns>The article text.</returns>
        public string AddCategory(string newCategory, string articleText, string articleTitle)
        {
            string oldText = articleText;

            articleText = FixCategories(articleText);

            if (Regex.IsMatch(articleText, @"\[\["
                + Variables.NamespacesCaseInsensitive[Namespace.Category]
                + Regex.Escape(newCategory) + @"[\|\]]"))
            {
                return oldText;
            }

            string cat = "\r\n[[" + Variables.Namespaces[Namespace.Category] + newCategory + "]]";
            cat = Tools.ApplyKeyWords(articleTitle, cat);

            if (Namespace.Determine(articleTitle) == Namespace.Template)
                articleText += "<noinclude>" + cat + "\r\n</noinclude>";
            else
                articleText += cat;

            return SortMetaData(articleText, articleTitle); //Sort metadata ordering so general fixes dont need to be enabled
        }

        // Covered by: RecategorizerTests.Replacement()
        /// <summary>
        /// Re-categorises the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="oldCategory">The old category to replace.</param>
        /// <param name="newCategory">The new category.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The re-categorised article text.</returns>
        public static string ReCategoriser(string oldCategory, string newCategory, string articleText, out bool noChange)
        {
            //remove category prefix
            oldCategory = Regex.Replace(oldCategory, "^"
                + Variables.NamespacesCaseInsensitive[Namespace.Category], "", RegexOptions.IgnoreCase);
            newCategory = Regex.Replace(newCategory, "^"
                + Variables.NamespacesCaseInsensitive[Namespace.Category], "", RegexOptions.IgnoreCase);

            //format categories properly
            articleText = FixCategories(articleText);

            string testText = articleText;

            if (Regex.IsMatch(articleText, "\\[\\["
                + Variables.NamespacesCaseInsensitive[Namespace.Category]
                + Tools.CaseInsensitive(Regex.Escape(newCategory)) + @"\s*(\||\]\])"))
            {
                bool tmp;
                articleText = RemoveCategory(oldCategory, articleText, out tmp);
            }
            else
            {
                oldCategory = Regex.Escape(oldCategory);
                oldCategory = Tools.CaseInsensitive(oldCategory);

                oldCategory = Variables.Namespaces[Namespace.Category] + oldCategory + @"\s*(\||\]\])";
                newCategory = Variables.Namespaces[Namespace.Category] + newCategory + "$1";

                articleText = Regex.Replace(articleText, oldCategory, newCategory);
            }

            noChange = (testText == articleText);

            return articleText;
        }

        // Covered by: RecategorizerTests.Removal()
        /// <summary>
        /// Removes a category from an article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="strOldCat">The old category to remove.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The article text without the old category.</returns>
        public static string RemoveCategory(string strOldCat, string articleText, out bool noChange)
        {
            articleText = FixCategories(articleText);
            string testText = articleText;

            strOldCat = Regex.Escape(strOldCat);
            strOldCat = Tools.CaseInsensitive(strOldCat);

            if (!articleText.Contains("<includeonly>"))
                articleText = Regex.Replace(articleText, "\\[\\["
                    + Variables.NamespacesCaseInsensitive[Namespace.Category] + " ?"
                    + strOldCat + "( ?\\]\\]| ?\\|[^\\|]*?\\]\\])\r\n", "");

            articleText = Regex.Replace(articleText, "\\[\\["
                + Variables.NamespacesCaseInsensitive[Namespace.Category] + " ?"
                + strOldCat + "( ?\\]\\]| ?\\|[^\\|]*?\\]\\])", "");

            noChange = (testText == articleText);

            return articleText;
        }

        private static readonly Regex Catregex = new Regex(@"\[\[\s*" + Variables.NamespacesCaseInsensitive[Namespace.Category] +
                  @"\s*(.*?)\s*(?:|\|([^\|\]]*))\s*\]\]", RegexOptions.Compiled); //TODO:Reassign namespace

        // Covered by: UtilityFunctionTests.ChangeToDefaultSort()
        /// <summary>
        /// Changes an article to use defaultsort when all categories use the same sort field / cleans diacritics from defaultsort/categories
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="noChange">If there is no change (True if no Change)</param>
        /// <returns>The article text possibly using defaultsort.</returns>
        public static string ChangeToDefaultSort(string articleText, string articleTitle, out bool noChange)
        {
            string testText = articleText;
            noChange = true;

            // count categories
            string sort = null;
            bool allsame = true;
            int matches = 0;

            foreach (Match m in Catregex.Matches(articleText))
            {
                string explicitKey = m.Groups[2].Value;
                if (explicitKey.Length == 0) explicitKey = articleTitle;

                if (string.IsNullOrEmpty(sort))
                    sort = explicitKey;

                if (sort != explicitKey && explicitKey != "")
                {
                    allsame = false;
                    break;
                }
                matches++;
            }

            // we don't need to process that {{Lifetime}} crap
            MatchCollection ds = WikiRegexes.Defaultsort.Matches(articleText);
            if (WikiRegexes.Lifetime.IsMatch(articleText) || ds.Count > 1 || (ds.Count == 1 && !ds[0].Value.ToUpper().Contains("DEFAULTSORT")))
                return articleText;


            articleText = TalkPages.TalkPageHeaders.FormatDefaultSort(articleText);

            // match again, after normalisation
            ds = WikiRegexes.Defaultsort.Matches(articleText);
            if (ds.Count > 1)
                return testText;

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#AWB_didn.27t_fix_special_characters_in_a_pipe
            articleText = FixCategories(articleText);

            if (ds.Count == 0)
            {
                if (allsame && matches > 1 && !string.IsNullOrEmpty(sort))
                {
                    if (sort.Length > 4 && // So that this doesn't get confused by sort keys of "*", " ", etc.
                        !sort.StartsWith(" "))
                    // MW bug: DEFAULTSORT doesn't treat leading spaces the same way as categories do
                    {
                        articleText = Catregex.Replace(articleText, "[["
                            + Variables.Namespaces[Namespace.Category] + "$1]]");

                        if (Tools.FixupDefaultSort(sort) != articleTitle)
                            articleText = articleText + "\r\n{{DEFAULTSORT:" + Tools.FixupDefaultSort(sort) + "}}";
                    }
                }
                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Add_defaultsort_to_pages_with_special_letters_and_no_defaultsort
                articleText = DefaultsortTitlesWithDiacritics(articleText, articleTitle, matches);
            }
            else // already has DEFAULTSORT
            {
                string s = Tools.FixupDefaultSort(ds[0].Groups[1].Value).Trim();
                if (s != ds[0].Groups[1].Value && s.Length > 0)
                    articleText = articleText.Replace(ds[0].Value, "{{DEFAULTSORT:" + s + "}}");

            }

            if (ds.Count == 1)
            {
                string defaultsortKey = ds[0].Groups["key"].Value;
                //Removes any explicit keys that are case insensitively the same as the default sort (To help tidy up on pages that already have defaultsort)
                articleText = ExplicitCategorySortkeys(articleText, defaultsortKey);
            }

            noChange = (testText == articleText);
            return articleText;
        }

        /// <summary>
        /// Removes any explicit keys that are case insensitively the same as the default sort (To help tidy up on pages that already have defaultsort)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="defaultsortKey"></param>
        /// <returns>The article text.</returns>
        private static string ExplicitCategorySortkeys(string articleText, string defaultsortKey)
        {
            foreach (Match m in Catregex.Matches(articleText))
            {
                string explicitKey = m.Groups[2].Value;
                if (explicitKey.Length == 0)
                    continue;

                if (string.Compare(explicitKey, defaultsortKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    articleText = articleText.Replace(m.Value,
                        "[[" + Variables.Namespaces[Namespace.Category] + m.Groups[1].Value + "]]");
                }
            }
            return (articleText);
        }

        /// <summary>
        /// if title has diacritics, no defaultsort added yet, adds a defaultsort with cleaned up title as sort key
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="matches">If there is no change (True if no Change)</param>
        /// <returns>The article text possibly using defaultsort.</returns>
        private static string DefaultsortTitlesWithDiacritics(string articleText, string articleTitle, int matches)
        {
            // need some categories and no defaultsort
            if (Tools.FixupDefaultSort(articleTitle) != articleTitle && matches > 0 &&
                !WikiRegexes.Defaultsort.IsMatch(articleText))
            {
                string sortkey = Tools.FixupDefaultSort(articleTitle);

                articleText = articleText + "\r\n{{DEFAULTSORT:" + sortkey + "}}";

                return (ExplicitCategorySortkeys(articleText, sortkey));
            }
            return (articleText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="noChange"></param>
        /// <returns></returns>
        public static string LivingPeople(string articleText, out bool noChange)
        {
            string testText = articleText;

            articleText = LivingPeople(articleText);

            noChange = (testText == articleText);

            return articleText;
        }

        private static readonly Regex LivingPeopleRegex1 = new Regex("\\[\\[ ?Category ?:[ _]?([0-9]{1,2}[ _]century[ _]deaths|[0-9s]{4,5}[ _]deaths|Disappeared[ _]people|Living[ _]people|Year[ _]of[ _]death[ _]missing|Possibly[ _]living[ _]people)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex LivingPeopleRegex2 = new Regex(@"\{\{(Template:)?(Recent ?death|Recentlydeceased)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex LivingPeopleRegex3 = new Regex("\\[\\[ ?Category ?:[ _]?([0-9]{4})[ _]births(\\|.*?)?\\]\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Adds [[Category:Living people]] to articles with a [[Category:XXXX births]] and no living people/deaths category, taking sortkey from births category if present
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The updated article text.</returns>
        public static string LivingPeople(string articleText)
        {
            // don't add living people category if already dead, or thought to be dead
            if (LivingPeopleRegex1.IsMatch(articleText) || LivingPeopleRegex2.IsMatch(articleText) ||
                BornDeathRegex.IsMatch(articleText) || DiedDateRegex.IsMatch(articleText))
                return articleText;

            Match m = LivingPeopleRegex3.Match(articleText);

            // don't add living people category unless 'XXXX births' category is present
            if (!m.Success)
                return articleText;

            string birthCat = m.Value;
            int birthYear = int.Parse(m.Groups[1].Value);

            // if born < 1910 they're likely dead
            if (birthYear < 1910)
                return articleText;

            // use any sortkey from 'XXXX births' category
            string catKey = birthCat.Contains("|") ? Regex.Match(birthCat, "\\|.*?\\]\\]").Value : "]]";

            return articleText + "[[Category:Living people" + catKey;
        }

        /// <summary>
        /// Converts/subst'd some deprecated templates
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public static string Conversions(string articleText, out bool noChange)
        {
            string testText = articleText;
            articleText = Conversions(articleText);

            noChange = (testText == articleText);

            return articleText;
        }

        // NOT covered
        /// <summary>
        /// Converts/subst'd some deprecated templates
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The new article text.</returns>
        public static string Conversions(string articleText)
        {
            //Use proper codes
            //checking first instead of substituting blindly saves some
            //time due to low occurence rate
            if (articleText.Contains("[[zh-tw:")) articleText = articleText.Replace("[[zh-tw:", "[[zh:");
            if (articleText.Contains("[[nb:")) articleText = articleText.Replace("[[nb:", "[[no:");
            if (articleText.Contains("[[dk:")) articleText = articleText.Replace("[[dk:", "[[da:");

            if (articleText.Contains("{{msg:")) articleText = articleText.Replace("{{msg:", "{{");

            foreach (KeyValuePair<Regex, string> k in RegexConversion)
            {
                articleText = k.Key.Replace(articleText, k.Value);
            }

            // {{nofootnotes}} --> {{morefootnotes}}, if some <ref>...</ref> references in article, uses regex from WikiRegexes.Refs
            if (WikiRegexes.Refs.IsMatch(articleText))
                articleText = Regex.Replace(articleText, @"{{nofootnotes}}", "{{morefootnotes}}", RegexOptions.IgnoreCase);

            return articleText;
        }

        // NOT covered
        /// <summary>
        /// Substitutes some user talk templates
        /// </summary>
        /// <param name="talkPageText">The wiki text of the talk page.</param>
        /// <param name="talkPageTitle"></param>
        /// <param name="userTalkTemplatesRegex"></param>
        /// <returns>The new text.</returns>
        public static string SubstUserTemplates(string talkPageText, string talkPageTitle, Regex userTalkTemplatesRegex)
        {
            if (userTalkTemplatesRegex == null) return talkPageText;

            talkPageText = talkPageText.Replace("{{{subst", "REPLACE_THIS_TEXT");
            Dictionary<Regex, string> regexes = new Dictionary<Regex, string>();

            regexes.Add(userTalkTemplatesRegex, "{{subst:$2}}");
            talkPageText = Tools.ExpandTemplate(talkPageText, talkPageTitle, regexes, true);

            talkPageText = Regex.Replace(talkPageText, " \\{\\{\\{2\\|\\}\\}\\}", "");
            talkPageText = talkPageText.Replace("REPLACE_THIS_TEXT", "{{{subst");
            return talkPageText;
        }

        //Covered by TaggerTests
        /// <summary>
        /// If necessary, adds/removes wikify or stub tag
        /// </summary>
        public string Tagger(string articleText, string articleTitle, out bool noChange, ref string summary, bool addTags, bool removeTags)
        {
            if (!addTags && !removeTags)
            {
                noChange = true;
                return articleText;
            }

            string testText = articleText;
            articleText = Tagger(articleText, articleTitle, ref summary, addTags, removeTags);
            articleText = TagUpdater(articleText);

            noChange = (testText == articleText);

            return articleText;
        }

        private static readonly CategoriesOnPageNoHiddenListProvider CategoryProv = new CategoriesOnPageNoHiddenListProvider();
        private static readonly WhatLinksHereListProvider WlhProv = new WhatLinksHereListProvider(1);

        //TODO:Needs re-write
        /// <summary>
        /// If necessary, adds/removes wikify or stub tag
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article title.</param>
        /// <param name="summary"></param>
        /// <param name="addTags"></param>
        /// <param name="removeTags"></param>
        /// <returns>The tagged article.</returns>
        public string Tagger(string articleText, string articleTitle, ref string summary, bool addTags, bool removeTags)
        {
            // don't tag redirects/outside article namespace/no tagging changes
            if (Tools.IsRedirect(articleText) || !Namespace.IsMainSpace(articleTitle)
                || (!addTags && !removeTags))
                return articleText;

            string commentsStripped = WikiRegexes.Comments.Replace(articleText, "");
            Sorter.Interwikis(ref commentsStripped);

            // bulleted or indented text should weigh less than simple text.
            // for example, actor stubs may contain large filmographies
            string crapStripped = WikiRegexes.BulletedText.Replace(commentsStripped, "");
            int words = (Tools.WordCount(commentsStripped) + Tools.WordCount(crapStripped)) / 2;

            // on en wiki, remove expand template when a stub template exists
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_.7B.7Bexpand.7D.7D_when_a_stub_template_exists
            if (removeTags && Variables.LangCode == LangCodeEnum.en && WikiRegexes.Stub.IsMatch(commentsStripped) && WikiRegexes.Expand.IsMatch(commentsStripped))
            {
                articleText = WikiRegexes.Expand.Replace(articleText, "");

                summary += ", removed expand tag";
            }

            // remove stub tags from long articles
            if (removeTags && (words > StubMaxWordCount) && WikiRegexes.Stub.IsMatch(commentsStripped))
            {
                articleText = WikiRegexes.Stub.Replace(articleText, StubChecker).Trim();
                summary += ", removed Stub tag";
            }

            // skip article if contains any template except for stub templates
            // because templates may provide categories/references
            foreach (Match m in WikiRegexes.Template.Matches(articleText))
            {
                if (!(WikiRegexes.Stub.IsMatch(m.Value)
                    || WikiRegexes.Uncat.IsMatch(m.Value)
                    || WikiRegexes.DeadEnd.IsMatch(m.Value)
                    || WikiRegexes.Wikify.IsMatch(m.Value)
                    || WikiRegexes.Orphan.IsMatch(m.Value)
                    || WikiRegexes.ReferenceList.IsMatch(m.Value)
                    || m.Value.Contains("subst")))
                    return articleText;
            }

            double length = articleText.Length + 1,
                   linkCount = Tools.LinkCount(commentsStripped);

            int totalCategories = (!Globals.UnitTestMode)
                                      ? CategoryProv.MakeList(new[] { articleTitle }).Count
                                      : Globals.UnitTestIntValue;

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Archive_19#AWB_problems
            // nl wiki doesn't use {{Uncategorized}} template
            if (addTags && words > 6 && totalCategories == 0
                && !WikiRegexes.Uncat.IsMatch(articleText) && Variables.LangCode != LangCodeEnum.nl)
            {
                if (WikiRegexes.Stub.IsMatch(commentsStripped))
                {
                    // add uncategorized stub tag
                    articleText +=
                        "\r\n\r\n{{Uncategorizedstub|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
                    summary += ", added [[:Category:Uncategorized stubs|uncategorised]] tag";
                }
                else
                {
                    // add uncategorized tag
                    articleText += "\r\n\r\n{{Uncategorized|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
                    summary += ", added [[:Category:Category needed|uncategorised]] tag";
                }
            }
            else if (removeTags && totalCategories > 0
                     && WikiRegexes.Uncat.IsMatch(articleText))
            {
                articleText = WikiRegexes.Uncat.Replace(articleText, "");
                summary += ", removed uncategorised tag";
            }

            if (addTags && commentsStripped.Length <= 300 && !WikiRegexes.Stub.IsMatch(commentsStripped))
            {
                // add stub tag
                articleText = articleText + "\r\n\r\n\r\n{{stub}}";
                summary += ", added stub tag";
            }

            if (addTags && linkCount == 0 && !WikiRegexes.DeadEnd.IsMatch(articleText))
            {
                // add dead-end tag
                articleText = "{{deadend|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + articleText;
                summary += ", added [[:Category:Dead-end pages|deadend]] tag";
            }
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#.7B.7BDeadend.7D.7D_gets_removed_from_categorized_pages
            // don't include categories as 'links'
            else if (removeTags && (linkCount - totalCategories) > 0 && WikiRegexes.DeadEnd.IsMatch(articleText))
            {
                articleText = WikiRegexes.DeadEnd.Replace(articleText, "");
                summary += ", removed deadend tag";
            }

            if (addTags && linkCount < 3 && ((linkCount / length) < 0.0025) && !WikiRegexes.Wikify.IsMatch(articleText))
            {
                // add wikify tag
                articleText = "{{Wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + articleText;
                summary += ", added [[:Category:Articles that need to be wikified|wikify]] tag";
            }
            else if (removeTags && linkCount > 3 && ((linkCount / length) > 0.0025) &&
                     WikiRegexes.Wikify.IsMatch(articleText))
            {
                articleText = WikiRegexes.Wikify.Replace(articleText, "");
                summary += ", removed wikify tag";
            }

            articleText = TagOrphans(articleText, articleTitle, ref summary, addTags, removeTags);

            return articleText;
        }

        private string TagOrphans(string articleText, string articleTitle, ref string summary, bool addTags, bool removeTags)
        {
            // check if not orphaned
            bool orphaned = true;
            if (Globals.UnitTestMode)
            {
                orphaned = Globals.UnitTestBoolValue;
            }
            else
            {
                try
                {
                    foreach (Article a in WlhProv.MakeList(0, articleTitle))
                        if (Namespace.IsMainSpace(a.Name))
                        {
                            orphaned = false;
                            break;
                        }
                }
                catch (Exception ex)
                {
                    // don't mark as orphan in case of exception
                    orphaned = false;
                    ErrorHandler.CurrentPage = articleTitle;
                    ErrorHandler.Handle(ex);
                }
            }

            // add orphan tag if applicable
            if (addTags && orphaned && !WikiRegexes.Orphan.IsMatch(articleText) && !WikiRegexes.OrphanArticleIssues.IsMatch(articleText))
            {
                articleText = "{{orphan|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + articleText;
                summary += ", added [[:Category:Orphaned articles|orphan]] tag";
            }
            else if (removeTags && !orphaned && WikiRegexes.Orphan.IsMatch(articleText))
            {
                articleText = WikiRegexes.Orphan.Replace(articleText, "");
                summary += ", removed orphan tag";
            }
            return articleText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public static string TagUpdater(string articleText)
        {
            // update by-date tags
            foreach (KeyValuePair<Regex, string> k in RegexTagger)
            {
                articleText = k.Key.Replace(articleText, k.Value);
            }
            return articleText;
        }

        private static string StubChecker(Match m)
        {
            // Replace each Regex cc match with the number of the occurrence.
            return Regex.IsMatch(m.Value, Variables.SectStub) ? m.Value : "";
        }

        // Covered by UtilityFunctionTests.NoBotsTests()
        /// <summary>
        /// checks if a user is allowed to edit this article
        /// using {{bots}} and {{nobots}} tags
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="user">Name of this user</param>
        /// <returns>true if you can edit, false otherwise</returns>
        public static bool CheckNoBots(string articleText, string user)
        {
            return
                !Regex.Match(articleText,
                             @"\{\{(nobots|bots\|(allow=none|deny=(?!none).*(" + user.Normalize() +
                             @"|awb|all).*|optout=all))\}\}", RegexOptions.IgnoreCase).Success;
        }

        private static readonly Regex DupeLinks1 = new Regex("\\[\\[([^\\]\\|]+)\\|([^\\]]*)\\]\\](.*[.\n]*)\\[\\[\\1\\|\\2\\]\\]", RegexOptions.Compiled);
        private static readonly Regex DupeLinks2 = new Regex("\\[\\[([^\\]]+)\\]\\](.*[.\n]*)\\[\\[\\1\\]\\]", RegexOptions.Compiled);

        /// <summary>
        /// Remove some of the duplicated wikilinks from the article text
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string RemoveDuplicateWikiLinks(string articleText)
        {
            articleText = DupeLinks1.Replace(articleText, "[[$1|$2]]$3$2");
            return DupeLinks2.Replace(articleText, "[[$1]]$2$1");
        }

        private static readonly Regex ExtToInt1 = new Regex(@"/\w+:\/\/secure\.wikimedia\.org\/(\w+)\/(\w+)\//", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ExtToInt2 = new Regex(@"/http:\/\/(\w+)\.(\w+)\.org\/wiki\/([^#{|}\[\]]*).*REMOVEME/i", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ExtToInt3 = new Regex(@"/http:\/\/(\w+)\.(\w+)\.org\/.*?title=([^#&{|}\[\]]*).*REMOVEME/i", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ExtToInt4 = new Regex(@"/[^\n]*?\[\[([^[\]{|}]+)[^\n]*REMOVEME/g", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ExtToInt5 = new Regex(@"/^ *(w:|wikipedia:|)(en:|([a-z\-]+:)) *REMOVEME/i", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ExtToInt6 = new Regex(@"/^ *(?:wikimedia:(m)eta|wikimedia:(commons)|(wikt)ionary|wiki(?:(n)ews|(b)ooks|(q)uote|(s)ource|(v)ersity))(:[a-z\-]+:)/i", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Covered by UtilityFunctionTests.ExternalURLToInternalLink(), incomplete
        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public static string ExternalURLToInternalLink(string articleText)
        {
            // Convert from the escaped UTF-8 byte code into Unicode
            articleText = HttpUtility.UrlDecode(articleText);
            // Convert secure URLs into non-secure equivalents (note the secure system is considered a 'hack')
            articleText = ExtToInt1.Replace(articleText, "http://$2.$1.org/");
            // Convert http://lang.domain.org/wiki/ into interwiki format
            articleText = ExtToInt2.Replace(articleText, "$2:$1:$3");
            // Scripts paths (/w/index.php?...) into interwiki format
            articleText = ExtToInt3.Replace(articleText, "$2:$1:$3");
            // Remove [[brackets]] from link
            articleText = ExtToInt4.Replace(articleText, "$1");
            // '_' -> ' ' and hard coded home wiki
            articleText = ExtToInt5.Replace(articleText, "$3");
            // Use short prefix form (wiktionary:en:Wiktionary:Main Page -> wikt:en:Wiktionary:Main Page)
            return ExtToInt6.Replace(articleText, "$1$2$3$4$5$6$7$8$9");
        }
        #endregion

        #region Property checkers
        /// <summary>
        /// Checks if the article has a stub template
        /// </summary>
        public static bool HasStubTemplate(string articleText)
        {
            return WikiRegexes.Stub.IsMatch(articleText);
        }

        /// <summary>
        /// Checks if the article is classible as a 'Stub'
        /// </summary>
        public static bool IsStub(string articleText)
        {
            return (HasStubTemplate(articleText) || articleText.Length < StubMaxWordCount);
        }

        /// <summary>
        /// Checks if the article has an Infobox (en wiki)
        /// </summary>
        public static bool HasInfobox(string articleText)
        {
            if (Variables.LangCode != LangCodeEnum.en)
                return false;

            articleText = WikiRegexes.Nowiki.Replace(articleText, "");
            articleText = WikiRegexes.Comments.Replace(articleText, "");

            return WikiRegexes.Infobox.IsMatch(articleText);
        }

        /// <summary>
        /// Check if article has an 'inusetag'
        /// </summary>
        public static bool IsInUse(string articleText)
        {
            return (Variables.LangCode != LangCodeEnum.en) ? false : Variables.InUse.IsMatch(WikiRegexes.Comments.Replace(articleText, ""));
        }

        /// <summary>
        /// Check if the article contains a sic template or bracketed wording, indicating the presence of a deliberate typo
        /// </summary>
        public static bool HasSicTag(string articleText)
        {
            return WikiRegexes.SicTag.IsMatch(articleText);
        }

        /// <summary>
        /// Check if the article contains a {{nofootnotes}} or {{morefootnotes}} template but has 5+ <ref>...</ref> references
        /// </summary>
        public static bool HasMorefootnotesAndManyReferences(string articleText)
        {
            return (WikiRegexes.MoreNoFootnotes.IsMatch(WikiRegexes.Comments.Replace(articleText, "")) && WikiRegexes.Refs.Matches(articleText).Count > 4);
        }

        /// <summary>
        /// Check if the article uses cite references but has no recognised template to display the references; only for en-wiki
        /// </summary>
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.28Yet.29_more_reference_related_changes.
        public static bool IsMissingReferencesDisplay(string articleText)
        {
            if (Variables.LangCode != LangCodeEnum.en)
                return false;

            return !WikiRegexes.ReferencesTemplate.IsMatch(articleText) && Regex.IsMatch(articleText, WikiRegexes.ReferenceEndGR);
        }

        /// <summary>
        /// Check if the article contains a <ref>...</ref> reference after the {{reflist}} to show them
        /// </summary>
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.28Yet.29_more_reference_related_changes.
        public static bool HasRefAfterReflist(string articleText)
        {
            articleText = WikiRegexes.Comments.Replace(articleText, "");
            return (WikiRegexes.RefAfterReflist.IsMatch(articleText) && WikiRegexes.ReferencesTemplate.Matches(articleText).Count == 1);
        }

        #endregion
    }
}
