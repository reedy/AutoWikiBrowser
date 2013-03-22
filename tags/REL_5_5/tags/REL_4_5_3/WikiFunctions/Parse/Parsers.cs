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
                RegexConversion.Add(new Regex(@"({{\s*[Aa]rticle ?issues\s*\|[^{}]*?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}[^{}]*?)?\|[^{}\|]{3,}?)\bdate"), "$1");
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

        public string HideText(string ArticleText)
        {
            return Hider.Hide(ArticleText);
        }

        public string AddBackText(string ArticleText)
        {
            return Hider.AddBack(ArticleText);
        }

        public string HideMoreText(string ArticleText, bool HideOnlyTargetOfWikilink)
        {
            return HiderHideExtLinksImages.HideMore(ArticleText, HideOnlyTargetOfWikilink);
        }

        public string HideMoreText(string ArticleText)
        {
            return HiderHideExtLinksImages.HideMore(ArticleText);
        }

        public string AddBackMoreText(string ArticleText)
        {
            return HiderHideExtLinksImages.AddBackMore(ArticleText);
        }

        // NOT covered
        /// <summary>
        /// Re-organises the Person Data, stub/disambig templates, categories and interwikis
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">The article title.</param>
        /// <returns>The re-organised text.</returns>
        public string SortMetaData(string ArticleText, string ArticleTitle)
        {
            return (Variables.Project <= ProjectEnum.species) ? Sorter.Sort(ArticleText, ArticleTitle) : ArticleText;
        }

        private static readonly Regex ApostropheInDecades = new Regex(@"(?<=(?:the |later? |early |mid-)(?:\[?\[?[12]\d\d0\]?\]?))'s(?=\]\])?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex regexHeadings0 = new Regex("(== ?)(see also:?|related topics:?|related articles:?|internal links:?|also see:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex regexHeadings1 = new Regex("(== ?)(external link[s]?|external site[s]?|outside link[s]?|web ?link[s]?|exterior link[s]?):?( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private readonly Regex regexHeadings2 = new Regex("(== ?)(external link:?|external site:?|web ?link:?|exterior link:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex regexHeadings3 = new Regex("(== ?)(referen[sc]e:?)(s? ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex regexHeadings4 = new Regex("(== ?)(source:?)(s? ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex regexHeadings5 = new Regex("(== ?)(further readings?:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex regexHeadings6 = new Regex("(== ?)(Early|Personal|Adult|Later) Life( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex regexHeadings7 = new Regex("(== ?)(Current|Past|Prior) Members( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex regexHeadings8 = new Regex(@"^(=+ ?)'''(.*?)'''( ?=+)\s*?(\r)?$", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex regexHeadings9 = new Regex("(== ?)track listing( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex regexHeadings10 = new Regex("(== ?)Life and Career( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex regexHeadingsCareer = new Regex("(== ?)([a-zA-Z]+) Career( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex regexRemoveHeadingsInLinks = new Regex(@"^ *((={1,4})[^\[\]\{\}\|=]*)\[{2}(?:[^\[\]\{\}\|=]+\|)?([^\[\]\{\}\|]+)\]\]([^\{\}=]*\2) *$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex RegexBadHeader = new Regex("^(={1,4} ?(about|description|overview|definition|profile|(?:general )?information|background|intro(?:duction)?|summary|bio(?:graphy)?) ?={1,4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex regexHeadingWhitespaceBefore = new Regex(@"^ *(==+)(\s*.+?\s*)\1 +(\r|\n)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex regexHeadingWhitespaceAfter = new Regex(@"^ +(==+)(\s*.+?\s*)\1 *(\r|\n)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex regexHeadingUpOneLevel = new Regex(@"^=(=+[^=].*?[^=]=+)=(\r\n?|\n)$", RegexOptions.Multiline);

        private static readonly Regex regexHeadingColonAtEnd = new Regex(@"^(=+)(.+?)\:(\s*\1(?:\r\n?|\n))$", RegexOptions.Multiline);

        /// <summary>
        /// Fix ==See also== and similar section common errors.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle"></param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public static string FixHeadings(string ArticleText, string ArticleTitle, out bool NoChange)
        {
            string testText = ArticleText;
            ArticleText = FixHeadings(ArticleText, ArticleTitle);

            NoChange = (testText == ArticleText);

            return ArticleText.Trim();
        }

        private const int MIN_CLEANUP_TAGS_TO_COMBINE = 3; // article must have at least this many tags to combine to {{Article issues}}

        private static readonly Regex ArticleIssuesInTitleCase = new Regex(@"({{[Aa]rticle ?issues\|\s*(?:[^{}]+?\|\s*)?)([A-Z])([a-z]+(?: [a-z]+)?\s*=)");

        /// <summary>
        /// Combines multiple cleanup tags into {{article issues}} template, ensures parameters have correct case, removes date parameter where not needed
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string ArticleIssues(string ArticleText)
        {
            // convert title case parameters within {{Article issues}} to lower case
            foreach (Match m in ArticleIssuesInTitleCase.Matches(ArticleText))
            {
                string FirstPart = m.Groups[1].Value;
                string ParameterFirstChar = m.Groups[2].Value.ToLower();
                string LastPart = m.Groups[3].Value;

                ArticleText = ArticleText.Replace(m.Value, FirstPart + ParameterFirstChar + LastPart);
            }

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#Date_parameter_getting_stripped_from_.7B.7Btl.7CArticle_issues.7D.7D
            // remove any date field within  {{Article issues}} if no 'expert' field using it
            if(!Regex.IsMatch(ArticleText, @"{{\s*[Aa]rticle ?issues[^{}]+?expert"))
                ArticleText = Regex.Replace(ArticleText, @"({{\s*[Aa]rticle ?issues\s*(?:\|[^{}]*?)?)\|\s*date\s*=[^{}\|]{0,20}?(\||}})", "$1$2");

            string NewTags = "";

            // get the zeroth section (text upto first heading)
            string ZerothSection = WikiRegexes.ZerothSection.Match(ArticleText).Value;

            // get the rest of the article including first heading (may be null if article has no headings)
            string RestOfArticle = ArticleText.Replace(ZerothSection, "");

            int TagsToAdd = WikiRegexes.ArticleIssuesTemplates.Matches(ZerothSection).Count;

            // if currently no {{Article issues}} and less than the min number of cleanup templates, do nothing
            if (!WikiRegexes.ArticleIssues.IsMatch(ZerothSection) && WikiRegexes.ArticleIssuesTemplates.Matches(ZerothSection).Count < MIN_CLEANUP_TAGS_TO_COMBINE)
                return (ArticleText);

            // only add tags to articleissues if new tags + existing >= MIN_CLEANUP_TAGS_TO_COMBINE
            if ((Regex.Matches(WikiRegexes.ArticleIssues.Match(ZerothSection).Value, WikiRegexes.ArticleIssuesTemplatesString).Count + TagsToAdd) < MIN_CLEANUP_TAGS_TO_COMBINE || TagsToAdd == 0)
                return (ArticleText);

                foreach (Match m in WikiRegexes.ArticleIssuesTemplates.Matches(ZerothSection))
                {
                    // all fields except COI, OR, POV and ones with BLP should be lower case
                    string SingleTag = m.Groups[1].Value;
                    if (!Regex.IsMatch(SingleTag, "(COI|OR|POV|BLP)"))
                        SingleTag = SingleTag.ToLower();

                    NewTags += @"|" + SingleTag + @" " + m.Groups[2].Value;
                    NewTags = NewTags.Trim();

                    // remove the single template
                    ZerothSection = ZerothSection.Replace(m.Value, "");
                }

                // if article currently has {{Article issues}}, add tags to it
                if (WikiRegexes.ArticleIssues.IsMatch(ZerothSection))
                    ZerothSection = WikiRegexes.ArticleIssues.Replace(ZerothSection, "$1" + NewTags + @"}}");
                else // add {{article issues}} to top of article, metaDataSorter will arrange correctly later
                    ZerothSection = @"{{Article issues" + NewTags + "}}\r\n" + ZerothSection;

            // Parsers.Conversions will add any missing dates and correct ...|wikify date=May 2008|...
            return (ZerothSection + RestOfArticle);
        }

        // Covered by: FormattingTests.TestFixHeadings(), incomplete
        /// <summary>
        /// Fix ==See also== and similar section common errors. Removes unecessary introductory headings and cleans excess whitespace.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle"></param>
        /// <returns>The modified article text.</returns>
        public static string FixHeadings(string ArticleText, string ArticleTitle)
        {
            ArticleText = Regex.Replace(ArticleText, "^={1,4} ?" + Regex.Escape(ArticleTitle) + " ?={1,4}", "", RegexOptions.IgnoreCase);
            ArticleText = RegexBadHeader.Replace(ArticleText, "");

            // loop through in case a heading has mulitple wikilinks in it
            while (regexRemoveHeadingsInLinks.IsMatch(ArticleText))
            {
                ArticleText = regexRemoveHeadingsInLinks.Replace(ArticleText, "$1$3$4");
            }

            if (!Regex.IsMatch(ArticleText, "= ?See also ?="))
                ArticleText = regexHeadings0.Replace(ArticleText, "$1See also$3");

            ArticleText = regexHeadings1.Replace(ArticleText, "$1External links$3");
            //ArticleText = regexHeadings2.Replace(ArticleText, "$1External link$3");
            ArticleText = regexHeadings3.Replace(ArticleText, "$1Reference$3");
            ArticleText = regexHeadings4.Replace(ArticleText, "$1Source$3");
            ArticleText = regexHeadings5.Replace(ArticleText, "$1Further reading$3");
            ArticleText = regexHeadings6.Replace(ArticleText, "$1$2 life$3");
            ArticleText = regexHeadings7.Replace(ArticleText, "$1$2 members$3");
            ArticleText = regexHeadings8.Replace(ArticleText, "$1$2$3$4");
            ArticleText = regexHeadings9.Replace(ArticleText, "$1Track listing$2");
            ArticleText = regexHeadings10.Replace(ArticleText, "$1Life and career$2");
            ArticleText = regexHeadingsCareer.Replace(ArticleText, "$1$2 career$3");

            ArticleText = regexHeadingWhitespaceBefore.Replace(ArticleText, "$1$2$1$3");
            ArticleText = regexHeadingWhitespaceAfter.Replace(ArticleText, "$1$2$1$3");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Headlines_end_with_colon_.28WikiProject_Check_Wikipedia_.2357.29
            ArticleText = regexHeadingColonAtEnd.Replace(ArticleText, "$1$2$3");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Section_header_level_.28WikiProject_Check_Wikipedia_.237.29
            // if no level 2 heading in article, remove a level from all headings (i.e. '===blah===' to '==blah==' etc.)
            if (!WikiRegexes.HeadingLevelTwo.IsMatch(ArticleText))
                ArticleText = regexHeadingUpOneLevel.Replace(ArticleText, "$1$2");

            return ArticleText;
        }

        // Covered by: LinkTests.FixDates()
        /// <summary>
        /// Fix date and decade formatting errors, and replace <br> and <p> HTML tags
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixDates(string ArticleText)
        {
            ArticleText = HideMoreText(ArticleText);

            ArticleText = FixDatesRaw(ArticleText);

            //Remove 2 or more <br />'s
            //This piece's existance here is counter-intuitive, but it requires HideMore()
            //and I don't want to call this slow function yet another time --MaxSem
            ArticleText = SyntaxRemoveBr.Replace(ArticleText, "\r\n");
            ArticleText = SyntaxRemoveParagraphs.Replace(ArticleText, "\r\n\r\n");

            return AddBackMoreText(ArticleText);
        }

        static string months = @"(?:" + string.Join("|", Variables.MonthNames) + ")";

        private static readonly Regex DiedDateRegex = new Regex(@"('''[^']+'''\s*\()d\.(\s+\[*(?:" + months + @"\s+0?([1-3]?[0-9])|0?([1-3]?[0-9])\s*" + months + @")?\]*\s*\[*[1-2]?\d{3}\]*\)\s*)", RegexOptions.IgnoreCase);
        private static readonly Regex DOBRegex = new Regex(@"('''[^']+'''\s*\()b\.(\s+\[*(?:" + months + @"\s+0?([1-3]?\d)|0?([1-3]?\d)\s*" + months + @")?\]*\s*\[*[1-2]?\d{3}\]*\)\s*)", RegexOptions.IgnoreCase);
        private static readonly Regex BornDeathRegex = new Regex(@"('''[^']+'''\s*\()(?:[Bb]orn|b\.)\s+(\[*(?:" + months + @"\s+0?(?:[1-3]?\d)|0?(?:[1-3]?\d)\s*" + months + @")?\]*,?\s*\[*[1-2]?\d{3}\]*)\s*(.|&.dash;)\s*(?:[Dd]ied|d\.)\s+(\[*(?:" + months + @"\s+0?(?:[1-3]?\d)|0?(?:[1-3]?\d)\s*" + months + @")\]*,?\s*\[*[1-2]?\d{3}\]*\)\s*)", RegexOptions.IgnoreCase);

        //Covered by: LinkTests.FixLivingThingsRelatedDates()
        /// <summary>
        /// Replace b. and d. for born/died
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixLivingThingsRelatedDates(string ArticleText)
        {
            ArticleText = DiedDateRegex.Replace(ArticleText, "$1died$2"); //date of death
            ArticleText = DOBRegex.Replace(ArticleText, "$1born$2"); //date of birth
            return BornDeathRegex.Replace(ArticleText, "$1$2 – $4"); //birth and death
        }

        // Covered by: LinkTests.FixDates()
        /// <summary>
        /// Fixes date and decade formatting errors.
        /// Unlike FixDates(), requires wikitext processed with HideMore()
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixDatesRaw(string ArticleText)
        {
            return ApostropheInDecades.Replace(ArticleText, "s");
        }

        // NOT covered, unused
        /// <summary>
        /// Footnote formatting errors per [[WP:FN]].
        /// currently too buggy to be included into production builds
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixFootnotes(string ArticleText)
        {
            // One space/linefeed
            ArticleText = Regex.Replace(ArticleText, "[\\n\\r\\f\\t ]+?<ref([ >])", "<ref$1");
            // remove trailing spaces from named refs
            ArticleText = Regex.Replace(ArticleText, "<ref ([^>]*[^>])[ ]*>", "<ref $1>");
            // removed superscripted punctuation between refs
            ArticleText = Regex.Replace(ArticleText, "(</ref>|<ref[^>]*?/>)<sup>[ ]*[,;-]?[ ]*</sup><ref", "$1<ref");
            ArticleText = Regex.Replace(ArticleText, "(</ref>|<ref[^>]*?/>)[ ]*[,;-]?[ ]*<ref", "$1<ref");

            const string factTag = "{{[ ]*(fact|fact[ ]*[\\|][^}]*|facts|citequote|citation needed|cn|verification needed|verify source|verify credibility|who|failed verification|nonspecific|dubious|or|lopsided|GR[ ]*[\\|][ ]*[^ ]+|[c]?r[e]?f[ ]*[\\|][^}]*|ref[ _]label[ ]*[\\|][^}]*|ref[ _]num[ ]*[\\|][^}]*)[ ]*}}";
            ArticleText = Regex.Replace(ArticleText, "[\\n\\r\\f\\t ]+?" + factTag, "{{$1}}");

            const string lacksPunctuation = "([^\\.,;:!\\?\"'’])";
            const string questionOrExclam = "([!\\?])";
            //string minorPunctuation = "([\\.,;:])";
            const string anyPunctuation = "([\\.,;:!\\?])";
            const string majorPunctuation = "([,;:!\\?])";
            const string period = "([\\.])";
            const string quote = "([\"'’]*)";
            const string space = "[ ]*";

            const string refTag = "(<ref>([^<]|<[^/]|</[^r]|</r[^e]|</re[^f]|</ref[^>])*?</ref>" + "|<ref[^>]*?[^/]>([^<]|<[^/]|</[^r]|</r[^e]|</re[^f]" + "|</ref[^>])*?</ref>|<ref[^>]*?/>)";

            const string match0a = lacksPunctuation + quote + factTag + space + anyPunctuation;
            const string match0b = questionOrExclam + quote + factTag + space + majorPunctuation;
            //string match0c = minorPunctuation + quote + factTag + space + anyPunctuation;
            const string match0d = questionOrExclam + quote + factTag + space + period;

            const string match1a = lacksPunctuation + quote + refTag + space + anyPunctuation;
            const string match1b = questionOrExclam + quote + refTag + space + majorPunctuation;
            //string match1c = minorPunctuation + quote + refTag + space + anyPunctuation;
            const string match1d = questionOrExclam + quote + refTag + space + period;

            string oldArticleText = "";

            while (oldArticleText != ArticleText)
            { // repeat for multiple refs together
                oldArticleText = ArticleText;
                ArticleText = Regex.Replace(ArticleText, match0a, "$1$2$4$3");
                ArticleText = Regex.Replace(ArticleText, match0b, "$1$2$4$3");
                //ArticleText = Regex.Replace(ArticleText, match0c, "$2$4$3");
                ArticleText = Regex.Replace(ArticleText, match0d, "$1$2$3");

                ArticleText = Regex.Replace(ArticleText, match1a, "$1$2$6$3");
                ArticleText = Regex.Replace(ArticleText, match1b, "$1$2$6$3");
                //ArticleText = Regex.Replace(ArticleText, match1c, "$2$6$3");
                ArticleText = Regex.Replace(ArticleText, match1d, "$1$2$3");
            }

            //ArticleText = Regex.Replace(ArticleText, "(==*)<ref", "$1\r\n<ref");
            return ArticleText;
        }

        private static readonly string OutofOrderRefs = @"(<ref\s+name\s*=\s*(?:""|')?([^<>""]+?)(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>)(\s*{{\s*rp\|[^{}]+}})?(\s*)(<ref\s+name\s*=\s*(?:""|')?([^<>""]+?)(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>)(\s*{{\s*rp\|[^{}]+}})?";
        private static readonly Regex OutofOrderRefs1 = new Regex(@"(<ref>[^<>]+</ref>)(\s*)(<ref\s+name\s*=\s*(?:""|')?([^<>""]+?)(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>)(\s*{{\s*[Rr]p\|[^{}]+}})?", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex OutofOrderRefs2 = new Regex(OutofOrderRefs, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        // regex below ensures a forced match on second and third of consecutive references
        private static readonly Regex OutofOrderRefs3 = new Regex(@"(?<=\s*(?:\/\s*|>[^<>]+</ref)>\s*(?:{{\s*rp\|[^{}]+}})?)" + OutofOrderRefs, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Reorders references so that they appear in numerical order, allows for use of {{rp}}
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string ReorderReferences(string ArticleText)
        {
            string ArticleTextBefore;
            for (int i = 0; i < 5; i++) // allows for up to 5 consecutive references
            {
                ArticleTextBefore = ArticleText;

                foreach (Match m in OutofOrderRefs1.Matches(ArticleText))
                {
                    string Ref1 = m.Groups[1].Value;
                    int Ref1Index = Regex.Match(ArticleText, @"(?i)<ref\s+name\s*=\s*(?:""|')?" + Regex.Escape(m.Groups[4].Value) + @"(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>").Index;
                    int Ref2Index = ArticleText.IndexOf(Ref1);

                    if (Ref1Index < Ref2Index && Ref2Index > 0)
                    {
                        string Whitespace = m.Groups[2].Value;
                        string Rptemplate = m.Groups[5].Value;
                        string Ref2 = m.Groups[3].Value;
                        ArticleText = ArticleText.Replace(Ref1 + Whitespace + Ref2 + Rptemplate, Ref2 + Rptemplate + Whitespace + Ref1);
                    }
                }

                ArticleText = ReorderRefs(ArticleText, OutofOrderRefs2);
                ArticleText = ReorderRefs(ArticleText, OutofOrderRefs3);

                if (ArticleTextBefore.Equals(ArticleText))
                    break;
            }

            return (ArticleText);
        }

        private static string ReorderRefs(string ArticleText, Regex OutofOrderRegex)
        {
            foreach (Match m in OutofOrderRegex.Matches(ArticleText))
            {
                int Ref1Index = Regex.Match(ArticleText, @"(?i)<ref\s+name\s*=\s*(?:""|')?" + Regex.Escape(m.Groups[2].Value) + @"(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>").Index;
                int Ref2Index = Regex.Match(ArticleText, @"(?i)<ref\s+name\s*=\s*(?:""|')?" + Regex.Escape(m.Groups[6].Value) + @"(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>").Index;

                if (Ref1Index > Ref2Index && Ref1Index > 0)
                {
                    string Ref1 = m.Groups[1].Value;
                    string Ref2 = m.Groups[5].Value;
                    string Whitespace = m.Groups[4].Value;
                    string Rptemplate1 = m.Groups[3].Value;
                    string Rptemplate2 = m.Groups[7].Value;
                    ArticleText = ArticleText.Replace(Ref1 + Rptemplate1 + Whitespace + Ref2 + Rptemplate2, Ref2 + Rptemplate2 + Whitespace + Ref1 + Rptemplate1);
                }
            }

            return ArticleText;
        }

        private static readonly Regex NamedReferences = new Regex(@"(<\s*ref\s+name\s*=\s*(?:""|')?([^<>=\r\n]+?)(?:""|')?\s*>\s*([^<>]+?)\s*<\s*/\s*ref>)", RegexOptions.Singleline);

        // Covered by: DuplicateNamedReferencesTests()
        /// <summary>
        /// Where an unnamed reference is a duplicate of another named reference, set the unnamed one to use the named ref
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>        
        public static string DuplicateNamedReferences(string ArticleText)
        {
            foreach (Match m in NamedReferences.Matches(ArticleText))
            {
                string RefName = m.Groups[2].Value;
                string UnnamedRefValue = m.Groups[3].Value;

                int a = 0;
                string firstref = "";
                // duplicate citation fixer (both named): <ref name="Fred">(...)</ref>....<ref name="Fred">\2</ref> --> ..<ref name="Fred"/>, minimum 25 characters to avoid short refs
                // don't fix the fist match, change the later ones to use <ref name="a"/> format, don't bother to change really short refs (value < 25 chars)
                foreach (Match m2 in Regex.Matches(ArticleText, @"((<\s*ref\s+name\s*=\s*(?:""|')?" + Regex.Escape(RefName) + @"(?:""|')?)\s*>\s*" + Regex.Escape(UnnamedRefValue) + @"\s*<\s*/\s*ref>)"))
                {
                    if (UnnamedRefValue.Length < 25)
                        break;

                    // mask first match
                    if (a == 0)
                    {
                        firstref = m2.Value;
                        Regex r = new Regex(Regex.Escape(firstref));

                        ArticleText = r.Replace(ArticleText, @"⌊⌊⌊⌊@@⌋⌋⌋⌋", 1);
                    }
                    else // replace all duplicates
                        ArticleText = ArticleText.Replace(m2.Value, @"<ref name=""" + RefName + @"""/>");

                    a++;
                }

                // unmask first match
                ArticleText = ArticleText.Replace(@"⌊⌊⌊⌊@@⌋⌋⌋⌋", firstref);

                // duplicate citation fixer (first named): <ref name="Fred">(...)</ref>....<ref>\2</ref> --> ..<ref name="Fred"/>
                // duplicate citation fixer (second named): <ref>(...)</ref>....<ref name="Fred">\2</ref> --> ..<ref name="Fred"/>
                foreach (Match m3 in Regex.Matches(ArticleText, @"<\s*ref\s*>\s*" + Regex.Escape(UnnamedRefValue) + @"\s*<\s*/\s*ref>"))
                {
                    ArticleText = ArticleText.Replace(m3.Value, @"<ref name=""" + RefName + @"""/>");
                }
            }

            return (ArticleText);
        }

        /// <summary>
        /// Corrects common formatting errors in dates in external reference citation templates (doesn't link/delink dates)
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string CiteTemplateDates(string ArticleText, out bool NoChange)
        {
            string ArticleTextAtStart = ArticleText;

            ArticleText = CiteTemplateDates(ArticleText);

            NoChange = (ArticleTextAtStart == ArticleText);

            return ArticleText;
        }

        /// <summary>
        /// Corrects common formatting errors in dates in external reference citation templates (doesn't link/delink dates)
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string CiteTemplateDates(string ArticleText)
        {
            // note some incorrect date formats such as 3-2-2009 are ambiguous as could be 3-FEB-2009 or MAR-2-2009
            // these fixes don't address such errors

            const string SICitStart = @"(?si)(\{\{\s*cit[^{}]*\|\s*";

            // convert invalid date formats like DD-MM-YYYY, MM-DD-YYYY, YYYY-D-M, YYYY-DD-MM, YYYY_MM_DD etc. to iso format of YYYY-MM-DD
            // for accessdate= and archivedate=
            if (Regex.IsMatch(ArticleText, @"\b(access|archive)date\s*=", RegexOptions.IgnoreCase))
            {
                string CitAccessdate = SICitStart + @"(?:access|archive)date\s*=\s*";
                ArticleText = Regex.Replace(ArticleText, CitAccessdate + @")(1[0-2])[/_\-\.]?(1[3-9])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3");
                ArticleText = Regex.Replace(ArticleText, CitAccessdate + @")(1[0-2])[/_\-\.]?([2-3]\d)[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3");
                ArticleText = Regex.Replace(ArticleText, CitAccessdate + @")(1[0-2])[/_\-\.]?\2[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$3-$2-$2"); // nn-nn-2004 and nn-nn-04 to ISO format (both nn the same)
                ArticleText = Regex.Replace(ArticleText, CitAccessdate + @")(1[3-9])[/_\-\.]?(1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2");
                ArticleText = Regex.Replace(ArticleText, CitAccessdate + @")(1[3-9])[/_\-\.]?0?([1-9])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2");
                ArticleText = Regex.Replace(ArticleText, CitAccessdate + @")(20[01]\d)0?([01]\d)[/_\-\.]([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4");
                ArticleText = Regex.Replace(ArticleText, CitAccessdate + @")(20[01]\d)[/_\-\.]([01]\d)0?([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4");
                ArticleText = Regex.Replace(ArticleText, CitAccessdate + @")(20[01]\d)[/_\-\.]?([01]\d)[/_\-\.]?([1-9]\s*(?:\||}}))", "$1$2-$3-0$4");
                ArticleText = Regex.Replace(ArticleText, CitAccessdate + @")(20[01]\d)[/_\-\.]?([1-9])[/_\-\.]?([0-3]\d\s*(?:\||}}))", "$1$2-0$3-$4");
                ArticleText = Regex.Replace(ArticleText, CitAccessdate + @")(20[01]\d)[/_\-\.]?([1-9])[/_\-\.]0?([1-9]\s*(?:\||}}))", "$1$2-0$3-0$4");
                ArticleText = Regex.Replace(ArticleText, CitAccessdate + @")(20[01]\d)[/_\-\.]0?([1-9])[/_\-\.]([1-9]\s*(?:\||}}))", "$1$2-0$3-0$4");
                ArticleText = Regex.Replace(ArticleText, CitAccessdate + @")(20[01]\d)[/_\.]?([01]\d)[/_\.]?([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4");

                ArticleText = Regex.Replace(ArticleText, CitAccessdate + @")([2-3]\d)[/_\-\.]?(1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2");
                ArticleText = Regex.Replace(ArticleText, CitAccessdate + @")([2-3]\d)[/_\-\.]0?([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2");
                ArticleText = Regex.Replace(ArticleText, CitAccessdate + @")0?([1-9])[/_\-\.]?(1[3-9]|[2-3]\d)[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$2-$3");
                ArticleText = Regex.Replace(ArticleText, CitAccessdate + @")0?([1-9])[/_\-\.]?0?\2[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$3-0$2-0$2"); // n-n-2004 and n-n-04 to ISO format (both n the same)
            }

            // date=, archivedate=, airdate=, date2=
            if (Regex.IsMatch(ArticleText, @"{{\s*cit[^{}]*\|\s*(?:archive|air)?date2?\s*=", RegexOptions.IgnoreCase | RegexOptions.Singleline))
            {
                string CitDate = SICitStart + @"(?:archive|air)?date2?\s*=\s*";
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)(200\d|19[7-9]\d)[/_]?([0-1]\d)[/_]?([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4");
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)(1[0-2])[/_\-\.]?([2-3]\d)[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$2-$3");
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)0?([1-9])[/_\-\.]?([2-3]\d)[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$2-$3");
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)([2-3]\d)[/_\-\.]?0?([1-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$3-$2");
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)([2-3]\d)[/_\-\.]?(1[0-2])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$3-$2");
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)(1[0-2])[/_\-\.]([2-3]\d)[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3");
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)0?([1-9])[/_\-\.]([2-3]\d)[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$2-$3");
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)([2-3]\d)[/_\-\.]0?([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2");
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)([2-3]\d)[/_\-\.]?(1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2");
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)(1[0-2])[/_\-\.]?(1[3-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$2-$3");
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)0?([1-9])[/_\-\.](1[3-9])[/_\-\.](19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$2-$3");
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)(1[3-9])[/_\-\.]?0?([1-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$3-$2");
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)(1[3-9])[/_\-\.]?(1[0-2])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$3-$2");
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)(1[0-2])[/_\-\.]?(1[3-9])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3");
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)([1-9])[/_\-\.](1[3-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$2-$3");
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)(1[3-9])[/_\-\.]?([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2");
                ArticleText = Regex.Replace(ArticleText, CitDate + @"\[?\[?)(1[3-9])[/_\-\.](1[0-2])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2");
                ArticleText = Regex.Replace(ArticleText, CitDate + @")0?([1-9])[/_\-\.]0?\2[/_\-\.](200\d|19[7-9]\d)(?=\s*(?:\||}}))", "$1$3-0$2-0$2"); // n-n-2004 and n-n-1980 to ISO format (both n the same)
                ArticleText = Regex.Replace(ArticleText, CitDate + @")0?([1-9])[/_\-\.]0?\2[/_\-\.]([01]\d)(?=\s*(?:\||}}))", "${1}20$3-0$2-0$2"); // n-n-04 to ISO format (both n the same)
                ArticleText = Regex.Replace(ArticleText, CitDate + @")(1[0-2])[/_\-\.]?\2[/_\-\.]?(200\d|19[7-9]\d)(?=\s*(?:\||}}))", "$1$3-$2-$2"); // nn-nn-2004 and nn-nn-1980 to ISO format (both nn the same)
                ArticleText = Regex.Replace(ArticleText, CitDate + @")(1[0-2])[/_\-\.]?\2[/_\-\.]?([01]\d)(?=\s*(?:\||}}))", "${1}20$3-$2-$2"); // nn-nn-04 to ISO format (both nn the same)
                ArticleText = Regex.Replace(ArticleText, CitDate + @")((?:\[\[)?200\d|19[7-9]\d)[/_\-\.]([1-9])[/_\-\.]0?([1-9](?:\]\])?\s*(?:\||}}))", "$1$2-0$3-0$4");
                ArticleText = Regex.Replace(ArticleText, CitDate + @")((?:\[\[)?200\d|19[7-9]\d)[/_\-\.]0?([1-9])[/_\-\.]([1-9](?:\]\])?\s*(?:\||}}))", "$1$2-0$3-0$4");
                ArticleText = Regex.Replace(ArticleText, CitDate + @")((?:\[\[)?200\d|19[7-9]\d)[/_\-\.]?([0-1]\d)[/_\-\.]?([1-9](?:\]\])?\s*(?:\||}}))", "$1$2-$3-0$4");
                ArticleText = Regex.Replace(ArticleText, CitDate + @")((?:\[\[)?200\d|19[7-9]\d)[/_\-\.]?([1-9])[/_\-\.]?([0-3]\d(?:\]\])?\s*(?:\||}}))", "$1$2-0$3-$4");
                ArticleText = Regex.Replace(ArticleText, CitDate + @")((?:\[\[)?200\d|19[7-9]\d)([0-1]\d)[/_\-\.]([0-3]\d(?:\]\])?\s*(?:\||}}))", "$1$2-$3-$4");
                ArticleText = Regex.Replace(ArticleText, CitDate + @")((?:\[\[)?200\d|19[7-9]\d)[/_\-\.]([0-1]\d)0?([0-3]\d(?:\]\])?\s*(?:\||}}))", "$1$2-$3-$4");

                // date = YYYY-Month-DD fix, on en-wiki only
                if (Variables.LangCode == LangCodeEnum.en)
                {
                    string CitYMonthD = SICitStart + @"(?:archive|air)?date2?\s*=\s*\d{4})[-/\s]";
                    string DTemEnd = @"?[-/\s]([0-3]?\d\s*(?:\||}}))";
                    ArticleText = Regex.Replace(ArticleText, CitYMonthD + @"Apr(?:il|\.)" + DTemEnd, "$1-04-$2");
                    ArticleText = Regex.Replace(ArticleText, CitYMonthD + @"Aug(?:ust|\.)" + DTemEnd, "$1-08-$2");
                    ArticleText = Regex.Replace(ArticleText, CitYMonthD + @"Dec(?:ember|\.)" + DTemEnd, "$1-12-$2");
                    ArticleText = Regex.Replace(ArticleText, CitYMonthD + @"Feb(?:r?uary|\.)" + DTemEnd, "$1-02-$2");
                    ArticleText = Regex.Replace(ArticleText, CitYMonthD + @"Jan(?:uary|\.)" + DTemEnd, "$1-01-$2");
                    ArticleText = Regex.Replace(ArticleText, CitYMonthD + @"Jul(?:y|\.)" + DTemEnd, "$1-07-$2");
                    ArticleText = Regex.Replace(ArticleText, CitYMonthD + @"Jun(?:e|\.)" + DTemEnd, "$1-06-$2");
                    ArticleText = Regex.Replace(ArticleText, CitYMonthD + @"Mar(?:ch|\.)" + DTemEnd, "$1-03-$2");
                    ArticleText = Regex.Replace(ArticleText, CitYMonthD + @"Nov(?:ember|\.)" + DTemEnd, "$1-11-$2");
                    ArticleText = Regex.Replace(ArticleText, CitYMonthD + @"Oct(?:ober|\.)" + DTemEnd, "$1-10-$2");
                    ArticleText = Regex.Replace(ArticleText, CitYMonthD + @"Sep(?:tember|\.)" + DTemEnd, "$1-09-$2");
                    ArticleText = Regex.Replace(ArticleText, CitYMonthD + @"May\." + DTemEnd, "$1-05-$2");
                }
            }

            // all citation dates
            ArticleText = Regex.Replace(ArticleText, SICitStart + @"(?:archive|air|access)?date2?\s*=\s*(?:\[\[)?200\d)-([2-3]\d|1[3-9])-(0[1-9]|1[0-2])(\]\])?", "$1-$3-$2$4"); // YYYY-DD-MM to YYYY-MM-DD
            ArticleText = Regex.Replace(ArticleText, @"(\{\{\s*cite[^\{\}]*\|\s*(?:archive|air|access)?date2?\s*=\s*(?:(?:200\d|19[7-9]\d)-[01]?\d-[0-3]?\d|[0-3]?\d\s*\w+,?\s*(?:200\d|19[7-9]\d)|\w+\s*[0-3]?\d,?\s*(?:200\d|19[7-9]\d)))(\s*[,-:]?\s+[0-2]?\d\:?[0-5]\d(?:\:?[0-5]\d)?\s*[^\|\}]*)", "$1<!--$2-->", RegexOptions.IgnoreCase | RegexOptions.Singleline); // Removes time from date fields

            return (ArticleText);
        }

        // Covered by: FormattingTests.TestMdashes()
        /// <summary>
        /// Replaces hyphens and em-dashes with en-dashes, per [[WP:DASH]]
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string Mdashes(string ArticleText, string ArticleTitle, int NameSpaceKey)
        {
            ArticleText = HideMoreText(ArticleText);

            //string months = "(" + string.Join("|", Variables.MonthNames) + ")";

            ArticleText = Regex.Replace(ArticleText, @"(pages\s*=\s*|pp\.?\s*)(\d+\s*)(?:-|—|&mdash;|&#8212;)(\s*\d+)", @"$1$2–$3", RegexOptions.IgnoreCase);

            ArticleText = Regex.Replace(ArticleText, @"([1-9]?\d\s*)(?:-|—|&mdash;|&#8212;)(\s*[1-9]?\d)(\s+|&nbsp;)(years|months|weeks|days|hours|minutes|seconds|kg|mg|kb|km|[Gk]?Hz|miles|mi\.|%)\b", @"$1–$2$3$4");

            //ArticleText = Regex.Replace(ArticleText, @"(\[?\[?" + months + @"\ [1-3]?\d\]?\]?,\ \[?\[?[1-2]\d{3}\]?\]?)\s*(?:-|—|&mdash;|&#8212;)\s*(\[?\[?" + months + @"\ [1-3]?\d\]?\]?,\ \[?\[?[1-2]\d{3}\]?\]?)", @"$1–$3");

            ArticleText = Regex.Replace(ArticleText, @"(\$[1-9]?\d{1,3}\s*)(?:-|—|&mdash;|&#8212;)(\s*\$?[1-9]?\d{1,3})", @"$1–$2");

            ArticleText = Regex.Replace(ArticleText, @"([01]?\d:[0-5]\d\s*([AP]M)\s*)(?:-|—|&mdash;|&#8212;)(\s*[01]?\d:[0-5]\d\s*([AP]M))", @"$1–$3", RegexOptions.IgnoreCase);

            ArticleText = Regex.Replace(ArticleText, @"([Aa]ge[sd])\s([1-9]?\d\s*)(?:-|—|&mdash;|&#8212;)(\s*[1-9]?\d)", @"$1 $2–$3");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Match_en_dashes.2Femdashs_from_titles_with_those_in_the_text
            // if title has en or em dashes, apply them to strings matching article title but with hyphens
            if (ArticleTitle.Contains(@"–") || ArticleTitle.Contains(@"—"))
                ArticleText = Regex.Replace(ArticleText, Regex.Escape(ArticleTitle.Replace(@"–", @"-").Replace(@"—", @"-")), ArticleTitle);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Change_--_.28two_dashes.29_to_.E2.80.94_.28em_dash.29
            if (NameSpaceKey == 0)
                ArticleText = Regex.Replace(ArticleText, @"(?<=\w)\s*--\s*(?=\w)", @"—");

            return AddBackMoreText(ArticleText);
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
        /// <param name="ArticleText">The wiki text of the article</param>
        /// <returns></returns>
        public static string FixReferenceListTags(string ArticleText)
        {
            if (BadReferenceListTags.IsMatch(ArticleText))
                ArticleText = BadReferenceListTags.Replace(ArticleText, "<references/>");

            return ReferenceListTags.Replace(ArticleText, new MatchEvaluator(ReflistMatchEvaluator));
        }

        private static readonly Regex EmptyReferences = new Regex(@"<ref\s+name=[""]?([^<>""]*)[""]?\s*>[\s]*< ?/ ?ref ?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Covered by: FootnotesTests.TestSimplifyReferenceTags()
        /// <summary>
        /// Replaces reference tags in the form <ref name="blah"></ref> with <ref name="blah" />
        /// Removes some of the MW errors that occur from the prior
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article</param>
        /// <returns></returns>
        public static string SimplifyReferenceTags(string ArticleText)
        {
            if (EmptyReferences.Match(ArticleText).Success)
                ArticleText = EmptyReferences.Replace(ArticleText, @"<ref name=""$1"" />");

            return ArticleText;
        }

        // NOT covered
        /// <summary>
        /// if the article uses cite references but has no recognised template to display the references, add {{reflist}} in the appropriate place
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article</param>
        /// <returns></returns>
        public static string AddMissingReflist(string ArticleText)
        {
            // AddMissingReflist is only called if references template is missing

            // Rename ==Links== to ==External links==
            ArticleText = Regex.Replace(ArticleText, @"(?sim)(==+\s*)Links(\s*==+\s*(?:^(?:\*|\d\.?)?\s*\[?\s*http://))", "$1External links$2");

            if (Regex.IsMatch(ArticleText, @"(?i)==\s*'*\s*References?\s*'*\s*=="))
                ArticleText = Regex.Replace(ArticleText, @"(?i)(==+\s*'*\s*References?\s*'*\s*==+)", "$1\r\n{{Reflist}}<!--added under references heading by script-assisted edit-->");
            else
            {
                //now try to move just above external links
                if (Regex.IsMatch(ArticleText, @"(?im)(^\s*=+\s*(?:External\s+link|Source|Web\s*link)s?\s*=)"))
                {
                    ArticleText += "\r\n==References==\r\n{{Reflist}}<!--added above External links/Sources by script-assisted edit-->";
                    ArticleText = Regex.Replace(ArticleText, @"(?sim)(^\s*=+\s*(?:External\s+link|Source|Web\s*link)s?\s*=+.*?)(\r\n==+References==+\r\n{{Reflist}}<!--added above External links/Sources by script-assisted edit-->)", "$2\r\n$1");
                }
                else
                { // now try to move just above categories
                    if (Regex.IsMatch(ArticleText, @"(?im)(^\s*\[\[\s*Category\s*:)"))
                    {
                        ArticleText += "\r\n==References==\r\n{{Reflist}}<!--added above categories/infobox footers by script-assisted edit-->";
                        ArticleText = Regex.Replace(ArticleText, @"(?sim)((?:^\{\{[^{}]+?\}\}\s*)*)(^\s*\[\[\s*Category\s*:.*?)(\r\n==+References==+\r\n{{Reflist}}<!--added above categories/infobox footers by script-assisted edit-->)", "$3\r\n$1$2");
                    }
                    //else
                    //{
                    // TODO: relist is missing, but not sure where references should go – at end of article might not be correct
                    //ArticleText += "\r\n==References==\r\n{{Reflist}}<!--added to end of article by script-assisted edit-->";
                    //ArticleText = Regex.Replace(ArticleText, @"(?sim)(^==.*?)(^\{\{[^{}]+?\}\}.*?)(\r\n==+References==+\r\n{{Reflist}}<!--added to end of article by script-assisted edit-->)", "$1\r\n$3\r\n$2");
                    //}
                }
            }
            // remove reflist comment
            ArticleText = Regex.Replace(ArticleText, @"(\{\{Reflist\}\})<!--added[^<>]+by script-assisted edit-->", "$1");

            return ArticleText;
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
        /// <param name="ArticleText">The wiki text of the article</param>
        /// <returns>The modified article text.</returns>
        public static string FixReferenceTags(string ArticleText)
        {
            ArticleText = RefWhitespace1.Replace(ArticleText, "<ref>");
            ArticleText = RefWhitespace2.Replace(ArticleText, "</ref>");
            ArticleText = RefWhitespace3.Replace(ArticleText, "$1");
            ArticleText = RefWhitespace4.Replace(ArticleText, "$1 $2");

            ArticleText = ReferenceTags1.Replace(ArticleText, "$1/>");
            ArticleText = ReferenceTags2.Replace(ArticleText, "$1$2>");
            ArticleText = ReferenceTags3.Replace(ArticleText, @"$1""$2""$3");
            ArticleText = ReferenceTags4.Replace(ArticleText, @"$1""$2""$3");
            ArticleText = ReferenceTags5.Replace(ArticleText, @"$1""$2""$3");
            ArticleText = ReferenceTags6.Replace(ArticleText, @"$1""$2""$3");
            ArticleText = ReferenceTags7.Replace(ArticleText, @"$1""$2""$3");
            ArticleText = ReferenceTags8.Replace(ArticleText, @"$1""$2""$3");
            ArticleText = ReferenceTags9.Replace(ArticleText, @"$1""$2""$3");
            ArticleText = ReferenceTags10.Replace(ArticleText, @"$1""$2""$3");
            ArticleText = ReferenceTags11.Replace(ArticleText, @"$1=$2");
            ArticleText = ReferenceTags12.Replace(ArticleText, "$1name=$2");
            ArticleText = ReferenceTags13.Replace(ArticleText, "$1a$2");
            ArticleText = ReferenceTags14.Replace(ArticleText, "$1</ref>");
            ArticleText = ReferenceTags15.Replace(ArticleText, "$1</ref>");
            ArticleText = ReferenceTags16.Replace(ArticleText, "$1ref$2");

            return ArticleText;
        }

        // Covered by TestFixDateOrdinalsAndOf
        /// <summary>
        /// Removes ordinals, leading zeros from dates and 'of' between a month and a year, per [[WP:MOSDATE]]; on en wiki only
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article</param>
        /// <param name="ArticleTitle">The article's title</param>
        /// <returns>The modified article text.</returns>
        public string FixDateOrdinalsAndOf(string ArticleText, string ArticleTitle)
        {
            if (Variables.LangCode != LangCodeEnum.en)
                return ArticleText;

            // don't match on 'in the June of 2007', 'on the 11th May 2008' etc. as these won't read well if changed
            Regex OfBetweenMonthAndYear = new Regex(@"\b" + WikiRegexes.months + @"\s+of\s+(200\d|1[89]\d\d)\b(?<!\b[Tt]he\s{1,5}\w{3,15}\s{1,5}of\s{1,5}(200\d|1[89]\d\d))");

            Regex OrdinalsInDatesAm = new Regex(@"\b" + WikiRegexes.months + @"\s+([0-3]?\d)(?:st|nd|rd|th)\b(?<!\b[Tt]he\s+\w{3,10}\s+(?:[0-3]?\d)(?:st|nd|rd|th)\b)(?:(\s*(?:to|and|.|&.dash;)\s*[0-3]?\d)(?:st|nd|rd|th)\b)?");
            Regex OrdinalsInDatesInt = new Regex(@"(?:\b([0-3]?\d)(?:st|nd|rd|th)(\s*(?:to|and|.|&.dash;)\s*))?\b([0-3]?\d)(?:st|nd|rd|th)\s+" + WikiRegexes.months + @"\b(?<!\b[Tt]he\s+(?:[0-3]?\d)(?:st|nd|rd|th)\s+\w{3,10})");

            Regex DateLeadingZerosAm = new Regex(@"\b" + WikiRegexes.months + @"\s+0([1-9])" + @"\b");
            Regex DateLeadingZerosInt = new Regex(@"\b" + @"0([1-9])\s+" + WikiRegexes.months + @"\b");

            // hide items in quotes etc., though this may also hide items within infoboxes etc.
            ArticleText = HideMoreText(ArticleText);

            ArticleText = OfBetweenMonthAndYear.Replace(ArticleText, "$1 $2");

            // don't apply if article title has a month in it (e.g. [[6th of October City]])
            if (!Regex.IsMatch(ArticleTitle, @"\b" + months + @"\b"))
            {
                ArticleText = OrdinalsInDatesAm.Replace(ArticleText, "$1 $2$3");
                ArticleText = OrdinalsInDatesInt.Replace(ArticleText, "$1$2$3 $4");
            }

            ArticleText = DateLeadingZerosAm.Replace(ArticleText, "$1 $2");
            ArticleText = DateLeadingZerosInt.Replace(ArticleText, "$1 $2");

            return AddBackMoreText(ArticleText);
        }


        // Covered by: FormattingTests.TestFixWhitespace(), incomplete
        /// <summary>
        /// Applies/removes some excess whitespace from the article
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string RemoveWhiteSpace(string ArticleText)
        {
            //Remove <br /> if followed by double newline
            ArticleText = Regex.Replace(ArticleText.Trim(), "<br ?/?>\r\n\r\n", "\r\n\r\n", RegexOptions.IgnoreCase);

            ArticleText = Regex.Replace(ArticleText, "\r\n(\r\n)+", "\r\n\r\n");

            ArticleText = Regex.Replace(ArticleText, "== ? ?\r\n\r\n==", "==\r\n==");
            ArticleText = Regex.Replace(ArticleText, @"==External links==[\r\n\s]*\*", "==External links==\r\n*");
            ArticleText = Regex.Replace(ArticleText, @"\r\n\r\n(\* ?\[?http)", "\r\n$1");

            ArticleText = Regex.Replace(ArticleText.Trim(), "----+$", "");

            if (ArticleText.Contains("\r\n|\r\n\r\n")) ArticleText = ArticleText.Replace("\r\n|\r\n\r\n", "\r\n|\r\n");
            if (ArticleText.Contains("\r\n\r\n|")) ArticleText = ArticleText.Replace("\r\n\r\n|", "\r\n|");

            return ArticleText.Trim();
        }

        // NOT covered
        /// <summary>
        /// Applies removes all excess whitespace from the article
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string RemoveAllWhiteSpace(string ArticleText)
        {//removes all whitespace
            ArticleText = ArticleText.Replace("\t", " ");
            ArticleText = RemoveWhiteSpace(ArticleText);

            ArticleText = ArticleText.Replace("\r\n\r\n*", "\r\n*");

            ArticleText = Regex.Replace(ArticleText, "  +", " ");
            ArticleText = Regex.Replace(ArticleText, " \r\n", "\r\n");

            ArticleText = Regex.Replace(ArticleText, "==\r\n\r\n", "==\r\n");
            ArticleText = Regex.Replace(ArticleText, @"==External links==[\r\n\s]*\*", "==External links==\r\n*");

            //fix bullet points – one space after them
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_arbitrary_spaces_after_bullet
            ArticleText = Regex.Replace(ArticleText, @"^([\*#]+) +", "$1", RegexOptions.Multiline);
            ArticleText = Regex.Replace(ArticleText, @"^([\*#]+)", "$1 ", RegexOptions.Multiline);

            //fix heading space
            ArticleText = Regex.Replace(ArticleText, "^(={1,4}) ?(.*?) ?(={1,4})$", "$1$2$3", RegexOptions.Multiline);

            //fix dash spacing
            ArticleText = Regex.Replace(ArticleText, " (–|—|&#15[01];|&[nm]dash;|&#821[12];|&#x201[34];) ", "$1");

            return ArticleText.Trim();
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
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixSyntax(string ArticleText)
        {
            //replace html with wiki syntax
            if (Regex.IsMatch(ArticleText, "</?i>", RegexOptions.IgnoreCase))
                ArticleText = SyntaxRegexItalic.Replace(ArticleText, "''$1''");

            if (Regex.IsMatch(ArticleText, "</?b>", RegexOptions.IgnoreCase))
                ArticleText = SyntaxRegexBold.Replace(ArticleText, "'''$1'''");

            ArticleText = Regex.Replace(ArticleText, "^<hr>|^----+", "----", RegexOptions.Multiline);

            //remove appearance of double line break
            ArticleText = Regex.Replace(ArticleText, "(^==?[^=]*==?)\r\n(\r\n)?----+", "$1", RegexOptions.Multiline);

            //remove unnecessary namespace
            ArticleText = SyntaxRegexTemplate.Replace(ArticleText, "$1$2");

            //remove <br> from lists
            ArticleText = SyntaxRegex11.Replace(ArticleText, "$1\r\n");

            //fix uneven bracketing on links
            ArticleText = DoubleBracketAtStartOfExternalLink.Replace(ArticleText, "$1");
            ArticleText = DoubleBracketAtEndOfExternalLink.Replace(ArticleText, "$1");
            ArticleText = DoubleBracketAtEndOfExternalLinkWithinImage.Replace(ArticleText, "$1");

            // (part) wikilinked/external linked URL in cite template, uses MediaWiki regex of [^\[\]<>""\s] for URL bit after http://
            ArticleText = BracketsAtBeginCiteTemplateURL.Replace(ArticleText, "$1$2$3");
            ArticleText = BracketsAtEndCiteTemplateURL.Replace(ArticleText, "$1$2$3");

            ArticleText = MultipleHttpInLink.Replace(ArticleText, "$1$2");

            ArticleText = PipedExternalLink.Replace(ArticleText, "$1 ''");

            //repair bad external links
            ArticleText = SyntaxRegex6.Replace(ArticleText, "[$1]");

            //repair bad internal links
            ArticleText = SyntaxRegex7.Replace(ArticleText, "[[$1]]");
            ArticleText = SyntaxRegex8.Replace(ArticleText, "[[$1]]");
            ArticleText = SyntaxRegex9.Replace(ArticleText, "[[$1#$2]]");
            ArticleText = SyntaxRegex12.Replace(ArticleText, @"$1 $2");

            if (!Regex.IsMatch(ArticleText, @"HTTP/\d\."))
            {
                ArticleText = MissingColonInHttpLink.Replace(ArticleText, "$1tp://$2");
                ArticleText = SingleTripleSlashInHttpLink.Replace(ArticleText, "$1tp://$2");
            }

            ArticleText = Regex.Replace(ArticleText, "ISBN: ?([0-9])", "ISBN $1");

            ArticleText = CellpaddingTypo.Replace(ArticleText, "$1cellpadding");

            ArticleText = AccessdateTypo.Replace(ArticleText, "$1accessdate$2");

            // {{cite web}} needs lower case field names; two loops in case a single template has multiple uppercase fields
            // restrict to en-wiki
            // exceptionally, 'ISBN' is allowed
            while (Variables.LangCode == LangCodeEnum.en)
            {
                foreach (Match m in UppercaseCiteFields.Matches(ArticleText))
                {
                    ArticleText = ArticleText.Replace(m.Value, m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value);
                }

                if (!UppercaseCiteFields.IsMatch(ArticleText))
                    break;
            }

            ArticleText = CiteFormatFieldTypo.Replace(ArticleText, "$1format$2");

            ArticleText = RemoveNoPropertyFontTags.Replace(ArticleText, "$1");

            // fixes for missing/unbalanced brackets
            ArticleText = RefTemplateIncorrectBracesAtEnd.Replace(ArticleText, @"$1}}");
            ArticleText = RefExternalLinkUsingBraces.Replace(ArticleText, @"[$1$2]$3");
            ArticleText = TemplateIncorrectBracesAtStart.Replace(ArticleText, @"{{$1");
            ArticleText = CitationTemplateSingleBraceAtStart.Replace(ArticleText, @"{$1");
            ArticleText = ReferenceTemplateQuadBracesAtEnd.Replace(ArticleText, @"$1");
            ArticleText = CitationTemplateIncorrectBraceAtStart.Replace(ArticleText, @"{{$1");
            ArticleText = CitationTemplateIncorrectBracesAtEnd.Replace(ArticleText, @"$1}}");
            ArticleText = RefExternalLinkMissingStartBracket.Replace(ArticleText, @"[$1");
            ArticleText = RefExternalLinkMissingEndBracket.Replace(ArticleText, @"]$1");
            ArticleText = RefCitationMissingOpeningBraces.Replace(ArticleText, @"{{");
            ArticleText = BracesWithinDefaultsort.Replace(ArticleText, @"}}");

            // fixes for square brackets used within external links
            foreach (Match m in SquareBracketsInExternalLinks.Matches(ArticleText))
            {
                // strip off leading [ and trailing ]
                string ExternalLink = Regex.Replace(m.Value, @"^\[(\s*http.*?)\]$", "$1", RegexOptions.Singleline);

                // if there are some brackets left then they need fixing; the mediawiki parser finishes the external link
                // at the first ] found
                if (ExternalLink.Contains("]"))
                {
                    // replace single ] with &#93; when used for brackets in the link description
                    ExternalLink = Regex.Replace(ExternalLink, @"([^]])\]([^]]|$)", @"$1&#93;$2");

                    ArticleText = ArticleText.Replace(m.Value, @"[" + ExternalLink + @"]");
                }
            }

            // needs to be applied after SquareBracketsInExternalLinks
            if (!Regex.IsMatch(ArticleText, "\\[\\[[Ii]mage:[^]]*http"))
            {
                ArticleText = SyntaxRegex4.Replace(ArticleText, "[[$1]]");
                ArticleText = SyntaxRegex5.Replace(ArticleText, "[[$1]]");
            }

            // if there are some unbalanced brackets, see whether we can fix them
            // the fixes applied might damage correct wiki syntax, hence are only applied if there are unbalanced brackets
            // of the right type
            int BracketLength = 0;
            string ArticleTextTemp = ArticleText;
            int UnbalancedBracket = Parsers.UnbalancedBrackets(ArticleText, ref BracketLength);
            if (UnbalancedBracket > -1)
            {
                int FirstUnbalancedBracket = UnbalancedBracket;
                // if it's ]]_]_ then see if removing bracket makes it all balance
                if (BracketLength == 1 && ArticleTextTemp[UnbalancedBracket].ToString().Equals(@"]") && 
                    ArticleTextTemp[UnbalancedBracket - 1].ToString().Equals(@"]") && ArticleTextTemp[UnbalancedBracket - 2].ToString().Equals(@"]"))
                    ArticleTextTemp = ArticleTextTemp.Remove(UnbalancedBracket, 1);
                                
                if (BracketLength == 1)
                {
                    // if it's [[blah blah}word]]
                    ArticleTextTemp = Regex.Replace(ArticleTextTemp, @"(?<=\[\[[^\[\]{}<>\r\n\|]{1,50})}(?=[^\[\]{}<>\r\n\|]{1,50}\]\])", @"|");

                    // if it's (blah} then see if setting the } to a ) makes it all balance, but not |} which could be wikitables
                    ArticleTextTemp = Regex.Replace(ArticleTextTemp, @"(?<=\([^{}<>\(\)]+[^{}<>\(\)\|])}(?=[^{}])", @")");

                    // if it's [blah} then see if setting the } to a ] makes it all balance
                    ArticleTextTemp = Regex.Replace(ArticleTextTemp, @"(?<=\[[^{}<>\[\]]+[^{}<>\(\)\|])}(?=[^{}])", @"]");

                    // if it's {blah) then see if setting the { to a ( makes it all balance, but not {| which could be wikitables
                    ArticleTextTemp = Regex.Replace(ArticleTextTemp, @"(?<=[^{}<>]){(?=[^{}<>\(\)\|][^{}<>\(\)]+\)[^{}\(\)])", @"(");

                    // if it's ((word) then see if removing the extra opening round bracket makes it all balance
                    if (ArticleTextTemp.Length > UnbalancedBracket && ArticleTextTemp[UnbalancedBracket].ToString().Equals(@"(") && ArticleText[UnbalancedBracket + 1].ToString().Equals(@"("))
                        ArticleTextTemp = ArticleTextTemp.Remove(UnbalancedBracket, 1);

                    // if it's {[link]] or {[[link]] or [[[link]] then see if setting to [[ makes it all balance
                    ArticleTextTemp = Regex.Replace(ArticleTextTemp, @"(?<=[^\[\]{}<>])(?:{\[\[?|\[\[\[)(?=[^\[\]{}<>]+\]\])", @"[[");

                    // could be [[{link]]
                    ArticleTextTemp = Regex.Replace(ArticleTextTemp, @"(?<=\[\[){(?=[^{}\[\]<>]+\]\])", "");

                    // external link missing closing ]
                    ArticleTextTemp = Regex.Replace(ArticleTextTemp, @"(?<=^ *\* *\[ *https?://[^<>{}\[\]\r\n\s]+[^\[\]\r\n]*)(\s$)", "]$1", RegexOptions.Multiline);

                    // external link missing opening [
                    ArticleTextTemp = Regex.Replace(ArticleTextTemp, @"(?<=^ *\*) *(?=https?://[^<>{}\[\]\r\n\s]+[^\[\]\r\n]*\]\s$)", " [", RegexOptions.Multiline);
                }
                
                if (BracketLength == 2)
                {
                    // if it's on double curly brackets, see if one is missing e.g. {{foo} or {{foo]}
                    ArticleTextTemp = Regex.Replace(ArticleTextTemp, @"(?<={{[^{}<>]+)(?:\]?}|}\])(?=[^{}])", @"}}");

                    // might be [[[[link]] or [[link]]]] so see if removing the two found square brackets makes it all balance
                    if(ArticleTextTemp.Substring(UnbalancedBracket, Math.Min(4, ArticleTextTemp.Length - UnbalancedBracket)).Equals("[[[[")
                        || ArticleTextTemp.Substring(Math.Max(0, UnbalancedBracket-2), Math.Min(4, ArticleTextTemp.Length - UnbalancedBracket)).Equals("]]]]"))                    
                        ArticleTextTemp = ArticleTextTemp.Remove(UnbalancedBracket, 2);
                }

                UnbalancedBracket = Parsers.UnbalancedBrackets(ArticleTextTemp, ref BracketLength);
                // the change worked if unbalanced bracket location moved considerably (so this one fixed), or all brackets now balance
                if (UnbalancedBracket < 0 || Math.Abs(UnbalancedBracket - FirstUnbalancedBracket) > 300)
                    ArticleText = ArticleTextTemp; 
            }

            return ArticleText.Trim();
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
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="BracketLength">integer to hold length of unbalanced bracket found</param>
        /// <returns>Index of any unbalanced brackets found</returns>
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Missing_opening_or_closing_brackets.2C_table_and_template_markup_.28WikiProject_Check_Wikipedia_.23_10.2C_28.2C_43.2C_46.2C_47.29
        public static int UnbalancedBrackets(string ArticleText, ref int BracketLength)
        {
            BracketLength = 2;

            int unbalancedfound = UnbalancedBrackets(ArticleText, @"{{", @"}}", DoubleCurlyBrackets);
            if (unbalancedfound > -1)
                return unbalancedfound;

            unbalancedfound = UnbalancedBrackets(ArticleText, @"[[", @"]]", DoubleSquareBrackets);
            if (unbalancedfound > -1)
                return unbalancedfound;

            BracketLength = 1;
            
            unbalancedfound = UnbalancedBrackets(ArticleText, @"{", @"}", SingleCurlyBrackets);
            if (unbalancedfound > -1)
                return unbalancedfound;
            
            unbalancedfound = UnbalancedBrackets(ArticleText, @"[", @"]", SingleSquareBrackets);
            if (unbalancedfound > -1)
                return unbalancedfound;

            unbalancedfound = UnbalancedBrackets(ArticleText, @"(", @")", SingleRoundBrackets);
            if (unbalancedfound > -1)
                return unbalancedfound;

            return -1;
        }

        /// <summary>
        /// Checks the article text for unbalanced brackets of the input type
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>Index of any unbalanced brackets found</returns>
        private static int UnbalancedBrackets(string ArticleText, string openingbrackets, string closingbrackets, Regex BracketsRegex)
        {
            // &#93; is used to replace the ] in external link text, which gives correct markup
            // replace [...&#93; with spaces to avoid matching as unbalanced brackets
            ArticleText = Regex.Replace(ArticleText, @"[^\[\]{}<>]\[[^\[\]{}<>]*?&#93;", @" ");

            if (Regex.Matches(ArticleText, Regex.Escape(openingbrackets)).Count != Regex.Matches(ArticleText, Regex.Escape(closingbrackets)).Count)
            {
                // remove all <math>, <code> stuff etc. where curly brackets are used in singles and pairs
                foreach (Match m in WikiRegexes.MathPreSourceCode.Matches(ArticleText))
                {
                    ArticleText = ArticleText.Replace(m.Value, Tools.ReplaceWithSpaces(m.Value));
                }

                if (openingbrackets.Equals(@"["))
                {
                    // need to remove double square brackets first
                    foreach (Match m in DoubleSquareBrackets.Matches(ArticleText))
                    {
                        ArticleText = ArticleText.Replace(m.Value, Tools.ReplaceWithSpaces(m.Value));
                    }
                }

                if (openingbrackets.Equals(@"{"))
                {
                    // need to remove double curly brackets first
                    foreach (Match m in DoubleCurlyBrackets.Matches(ArticleText))
                    {
                        ArticleText = ArticleText.Replace(m.Value, Tools.ReplaceWithSpaces(m.Value));
                    }
                }

                // replace all the valid balanced bracket sets with spaces
                foreach (Match m in BracketsRegex.Matches(ArticleText))
                {
                    ArticleText = ArticleText.Replace(m.Value, Tools.ReplaceWithSpaces(m.Value));
                }

                // now return the unbalanced one that's left
                int open = Regex.Matches(ArticleText, Regex.Escape(openingbrackets)).Count;
                int closed = Regex.Matches(ArticleText, Regex.Escape(closingbrackets)).Count;

                if (open == 0 && closed >= 1)
                    return ArticleText.IndexOf(closingbrackets);
                
                if (open >= 1 && closed == 0)
                    return ArticleText.IndexOf(openingbrackets);
                
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
        /// <param name="ArticleText">The wiki text of the article</param>
        /// <param name="ArticleTitle">The article title.</param>
        /// <returns>The modified article text.</returns>
        public static string FixLinkWhitespace(string ArticleText, string ArticleTitle)
        {
            //remove undesirable space from beginning of wikilink (space before wikilink) - parse this line first
            if (LinkWhitespace1.Match(ArticleText).Success)
                ArticleText = LinkWhitespace1.Replace(ArticleText, " [[$1]]");

            //remove undesirable space from beginning of wikilink and move it outside link - parse this line second
            if (LinkWhitespace2.Match(ArticleText).Success)
                ArticleText = LinkWhitespace2.Replace(ArticleText, " [[$1]]");

            //remove undesirable double space from middle of wikilink (up to 61 characters in wikilink)
            if (LinkWhitespace3.Match(ArticleText).Success)
                ArticleText = LinkWhitespace3.Replace(ArticleText, "[[$1 $2]]");

            //remove undesirable space from end of wikilink (space after wikilink) - parse this line first
            if (LinkWhitespace4.Match(ArticleText).Success)
                ArticleText = LinkWhitespace4.Replace(ArticleText, "[[$1]] ");

            //remove undesirable space from end of wikilink and move it outside link - parse this line second
            if (LinkWhitespace5.Match(ArticleText).Success)
                ArticleText = LinkWhitespace5.Replace(ArticleText, "[[$1]] ");

            //remove undesirable double space between links in date (day first)
            if (DateLinkWhitespace1.Match(ArticleText).Success)
                ArticleText = DateLinkWhitespace1.Replace(ArticleText, "$1 $2");

            //remove undesirable double space between links in date (day second)
            if (DateLinkWhitespace2.Match(ArticleText).Success)
                ArticleText = DateLinkWhitespace2.Replace(ArticleText, "$1 $2");

            // correct [[page# section]] to [[page#section]]
            Regex SectionLinkWhitespace = new Regex(@"(\[\[" + Regex.Escape(ArticleTitle) + @"\#)\s+([^\[\]]+\]\])");

            return SectionLinkWhitespace.IsMatch(ArticleText)
                       ? SectionLinkWhitespace.Replace(ArticleText, "$1$2")
                       : ArticleText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText"></param>
        /// <returns></returns>
        public static string FixLinkWhitespace(string ArticleText)
        {
            return FixLinkWhitespace(ArticleText, "test");
        }

        // NOT covered
        /// <summary>
        /// Fixes link syntax, including removal of self links
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public static string FixLinks(string ArticleText, string ArticleTitle, out bool NoChange)
        {
            string ArticleTextAtStart = ArticleText;
            string escTitle = Regex.Escape(ArticleTitle);
            //string escTitleNoBrackets = Regex.Escape(Regex.Replace(ArticleTitle, @" \(.*?\)$", ""));

            // remove any self-links, but not other links with different capitaliastion e.g. [[Foo]] vs [[FOO]]
            // note, removal of self links in iteslf will not cause this method to return a 'change'
            ArticleText = Regex.Replace(ArticleText, @"\[\[\s*" + escTitle + @"\s*\]\]", ArticleTitle);
            ArticleText = Regex.Replace(ArticleText, @"\[\[\s*" + Tools.TurnFirstToLower(escTitle) + @"\s*\]\]", Tools.TurnFirstToLower(ArticleTitle));

            // remove piped self links by leaving target
            ArticleText = Regex.Replace(ArticleText, @"\[\[\s*" + escTitle + @"\s*\|([^\]]+)\]\]", "$1");
            ArticleText = Regex.Replace(ArticleText, @"\[\[\s*" + Tools.TurnFirstToLower(escTitle) + @"\s*\|([^\]]+)\]\]", "$1");

            StringBuilder sb = new StringBuilder(ArticleText, (ArticleText.Length * 11) / 10);

            foreach (Match m in WikiRegexes.WikiLink.Matches(ArticleText))
            {
                if (m.Groups[1].Value.Length > 0)
                {
                    string y = m.Value.Replace(m.Groups[1].Value, CanonicalizeTitle(m.Groups[1].Value));

                    if (y != m.Value) sb = sb.Replace(m.Value, y);
                }
            }

            NoChange = (sb.ToString() == ArticleTextAtStart);

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
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The simplified article text.</returns>
        public static string SimplifyLinks(string ArticleText)
        {
            string a = "", b = "";

            try
            {
                foreach (Match m in WikiRegexes.PipedWikiLink.Matches(ArticleText))
                {
                    string n = m.Value;
                    a = m.Groups[1].Value.Trim();

                    b = (Namespace.Determine(a) != Namespace.Category)
                            ? m.Groups[2].Value.Trim()
                            : m.Groups[2].Value.TrimEnd(new[] { ' ' });

                    if (b.Length == 0) continue;

                    if (a == b || Tools.TurnFirstToLower(a) == b)
                    {
                        ArticleText = ArticleText.Replace(n, "[[" + b + "]]");
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
                        ArticleText = ArticleText.Replace(n, "[[" + b.Substring(0, a.Length) + "]]" + b.Substring(a.Length));
                    }
                    else
                    {
                        string newlink = "[[" + a + "|" + b + "]]";

                        if (newlink != n)
                            ArticleText = ArticleText.Replace(n, newlink);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + @"
a='" + a + "',  b='" + b + "'", "SimplifyLinks error");
            }

            return ArticleText;
        }

        // Covered by: LinkTests.TestStickyLinks()
        /// <summary>
        /// Joins nearby words with links
        ///   e.g. "[[Russian literature|Russian]] literature" to "[[Russian literature]]"
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article</param>
        /// <returns>Processed wikitext</returns>
        public static string StickyLinks(string ArticleText)
        {
            string a = "", b = "";
            try
            {
                foreach (Match m in WikiRegexes.PipedWikiLink.Matches(ArticleText))
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
                        ArticleText = Regex.Replace(ArticleText, search, "[[" + a + @"]]");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message + @"
a='" + a + "',  b='" + b + "'", "StickyLinks error");
            }

            return ArticleText;
        }

        private static readonly Regex RegexMainArticle = new Regex(@"^:?'{0,5}Main article:\s?'{0,5}\[\[([^\|\[\]]*?)(\|([^\[\]]*?))?\]\]\.?'{0,5}\.?\s*?(?=($|[\r\n]))", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

        // Covered by: FixMainArticleTests
        /// <summary>
        /// Fixes instances of ''Main Article: xxx'' to use {{main|xxx}}
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string FixMainArticle(string ArticleText)
        {
            return RegexMainArticle.Match(ArticleText).Groups[2].Value.Length == 0
                       ? RegexMainArticle.Replace(ArticleText, "{{main|$1}}")
                       : RegexMainArticle.Replace(ArticleText, "{{main|$1|l1=$3}}");
        }

        // Covered by LinkTests.TestFixEmptyLinksAndTemplates()
        /// <summary>
        /// Removes Empty Links and Template Links
        /// Will Cater for [[]], [[Image:]], [[:Category:]], [[Category:]] and {{}}
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string FixEmptyLinksAndTemplates(string ArticleText)
        {
            foreach (Match link in WikiRegexes.EmptyLink.Matches(ArticleText))
            {
                string trim = link.Groups[2].Value.Trim();
                if (string.IsNullOrEmpty(trim) || trim == "|" + Variables.NamespacesCaseInsensitive[Namespace.Image] ||
                    trim == "|" + Variables.NamespacesCaseInsensitive[Namespace.Category] || trim == "|")
                    ArticleText = ArticleText.Replace("[[" + link.Groups[1].Value + link.Groups[2].Value + "]]", "");
            }

            if (WikiRegexes.EmptyTemplate.Match(ArticleText).Success)
                ArticleText = WikiRegexes.EmptyTemplate.Replace(ArticleText, "");

            return ArticleText;
        }

        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public static string BulletExternalLinks(string ArticleText, out bool NoChange)
        {
            string testText = ArticleText;
            ArticleText = BulletExternalLinks(ArticleText);

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        static readonly HideText BulletExternalHider = new HideText(false, true, false);

        // Covered by: LinkTests.TestBulletExternalLinks()
        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string BulletExternalLinks(string ArticleText)
        {
            Match m = Regex.Match(ArticleText, @"=\s*(?:external)?\s*links\s*=", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

            if (!m.Success)
                return ArticleText;

            int intStart = m.Index;

            string articleTextSubstring = ArticleText.Substring(intStart);
            ArticleText = ArticleText.Substring(0, intStart);
            articleTextSubstring = BulletExternalHider.HideMore(articleTextSubstring);
            articleTextSubstring = Regex.Replace(articleTextSubstring, "(\r\n|\n)?(\r\n|\n)(\\[?http)", "$2* $3");

            return ArticleText + BulletExternalHider.AddBackMore(articleTextSubstring);
        }

        // Covered by: LinkTests.TestFixCategories()
        /// <summary>
        /// Fix common spacing/capitalisation errors in categories; remove diacritics and trailing whitespace from sortkeys (not leading whitespace)
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixCategories(string ArticleText)
        {
            string cat = "[[" + Variables.Namespaces[Namespace.Category];

            // fix extra brackets: three or more at end
            ArticleText = Regex.Replace(ArticleText, @"(?<=" + Regex.Escape(cat) + @"[^\r\n\[\]{}<>]+\]\])\]+", "");
            // three or more at start
            ArticleText = Regex.Replace(ArticleText, @"\[+(?=" + Regex.Escape(cat) + @"[^\r\n\[\]{}<>]+\]\])", "");

            foreach (Match m in WikiRegexes.LooseCategory.Matches(ArticleText))
            {
                if (!Tools.IsValidTitle(m.Groups[1].Value)) continue;
                string x = cat + Tools.TurnFirstToUpper(CanonicalizeTitleRaw(m.Groups[1].Value, false).Trim()) + Regex.Replace(Tools.RemoveDiacritics(m.Groups[2].Value), @"(\w+)\s+$", "$1") + "]]";
                if (x != m.Value) ArticleText = ArticleText.Replace(m.Value, x);
            }

            return ArticleText;
        }

        // Covered by: ImageTests.BasicImprovements(), incomplete
        /// <summary>
        /// Fix common spacing/capitalisation errors in images
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixImages(string ArticleText)
        {
            foreach (Match m in WikiRegexes.LooseImage.Matches(ArticleText))
            {
                // only apply underscore/URL encoding fixes to image name (group 2)
                string x = "[[" + Namespace.Normalize(m.Groups[1].Value, 6) + CanonicalizeTitle(m.Groups[2].Value).Trim() + m.Groups[3].Value.Trim() + "]]";
                ArticleText = ArticleText.Replace(m.Value, x);
            }

            return ArticleText;
        }

        private static readonly Regex Temperature = new Regex(@"([º°](&nbsp;|)|(&deg;|&ordm;)(&nbsp;|))\s*([CcFf])([^A-Za-z])", RegexOptions.Compiled);

        // NOT covered
        /// <summary>
        /// Fix bad Temperatures
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixTemperatures(string ArticleText)
        {
            foreach (Match m in Temperature.Matches(ArticleText))
                ArticleText = ArticleText.Replace(m.ToString(), "°" + m.Groups[5].Value.ToUpper() + m.Groups[6].Value);
            return ArticleText;
        }

        /// <summary>
        /// Apply non-breaking spaces for abbreviated SI units
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixNonBreakingSpaces(string ArticleText)
        {
            // hide items in quotes etc., though this may also hide items within infoboxes etc.
            ArticleText = HideMoreText(ArticleText);

            ArticleText = WikiRegexes.SiUnitsWithoutNonBreakingSpaces.Replace(ArticleText, "$1&nbsp;$2");

            return AddBackMoreText(ArticleText);
        }

        /// <summary>
        /// regex that Matches every template, for GetTemplate
        /// </summary>
        public const string EveryTemplate = @"[^\|\{\}]+";

        // NOT covered
        /// <summary>
        /// extracts template using the given match
        /// </summary>
        private static string ExtractTemplate(string ArticleText, Match m)
        {
            int i = m.Index + m.Groups[1].Length;

            int brackets = 2;
            while (i < ArticleText.Length)
            {
                switch (ArticleText[i])
                {
                    // only sequences of 2 and more brackets should be counted
                    case '{':
                        if ((ArticleText[i - 1] == '{') || (i + 1 < ArticleText.Length &&
                            ArticleText[i + 1] == '{')) brackets++;
                        break;
                    case '}':
                        if ((ArticleText[i - 1] == '}') || (i + 1 < ArticleText.Length &&
                            ArticleText[i + 1] == '}'))
                        {
                            brackets--;
                            if (brackets == 0) return ArticleText.Substring(m.Index, i - m.Index + 1);
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
        /// <param name="ArticleText">source text</param>
        /// <param name="Template">name of template, can be regex without a group capture</param>
        /// <returns>template with all params, enclosed in curly brackets</returns>
        public static string GetTemplate(string ArticleText, string Template)
        {
            ArticleText = WikiRegexes.Nowiki.Replace(ArticleText, "");
            ArticleText = WikiRegexes.Comments.Replace(ArticleText, "");
            Regex search = new Regex(@"(\{\{\s*" + Template + @"\s*)(?:\||\})", RegexOptions.Singleline);

            Match m = search.Match(ArticleText);

            return m.Success ? ExtractTemplate(ArticleText, m) : "";
        }

        // NOT covered
        /// <summary>
        /// finds every occurence of a given template in article text
        /// handles nested templates correctly
        /// </summary>
        /// <param name="ArticleText">source text</param>
        /// <param name="Template">name of template, can be regex without a group capture</param>
        /// <returns>template with all params, enclosed in curly brackets</returns>
        public static List<Match> GetTemplates(string ArticleText, string Template)
        {
            MatchCollection nw = WikiRegexes.Nowiki.Matches(ArticleText);
            MatchCollection cm = WikiRegexes.Comments.Matches(ArticleText);
            Regex search = new Regex(@"(\{\{\s*" + Template + @"\s*)[\|\}]",
                RegexOptions.Singleline);

            List<Match> res = new List<Match>();

            int pos = 0;
            foreach (Match m in search.Matches(ArticleText))
            {
                if (m.Index < pos) continue;
                foreach (Match m2 in nw) if (m.Index > m2.Index &&
                    m.Index < m2.Index + m2.Length) continue;
                foreach (Match m2 in cm) if (m.Index > m2.Index &&
                    m.Index < m2.Index + m2.Length) continue;

                string s = ExtractTemplate(ArticleText, m);
                if (string.IsNullOrEmpty(s)) break;
                pos = m.Index + s.Length;
                Match mres = m;
                foreach (Match m2 in Regex.Matches(ArticleText, Regex.Escape(s)))
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
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text (removed empty comments).</returns>
        public static string RemoveEmptyComments(string ArticleText)
        {
            return WikiRegexes.EmptyComments.Replace(ArticleText, "");
        }
        #endregion

        #region other functions

        /// <summary>
        /// Converts HTML entities to unicode, with some deliberate exceptions
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string Unicodify(string ArticleText, out bool NoChange)
        {
            string testText = ArticleText;
            ArticleText = Unicodify(ArticleText);

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        // Covered by: UnicodifyTests
        /// <summary>
        /// Converts HTML entities to unicode, with some deliberate exceptions
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string Unicodify(string ArticleText)
        {
            if (Regex.IsMatch(ArticleText, "<[Mm]ath>"))
                return ArticleText;

            ArticleText = Regex.Replace(ArticleText, "&#150;|&#8211;|&#x2013;", "&ndash;");
            ArticleText = Regex.Replace(ArticleText, "&#151;|&#8212;|&#x2014;", "&mdash;");
            ArticleText = ArticleText.Replace(" &amp; ", " & ");
            ArticleText = ArticleText.Replace("&amp;", "&amp;amp;");
            ArticleText = ArticleText.Replace("&#153;", "™");
            ArticleText = ArticleText.Replace("&#149;", "•");

            foreach (KeyValuePair<Regex, string> k in RegexUnicode)
            {
                ArticleText = k.Key.Replace(ArticleText, k.Value);
            }
            try
            {
                ArticleText = HttpUtility.HtmlDecode(ArticleText);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex);
            }

            return ArticleText;
        }

        // Covered by: BoldTitleTests
        /// <summary>
        /// '''Emboldens''' the first occurence of the article title, if not already bold
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">The title of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string BoldTitle(string ArticleText, string ArticleTitle, out bool NoChange)
        {
            NoChange = true;
            string escTitle = Regex.Escape(ArticleTitle);
            string escTitleNoBrackets = Regex.Escape(Regex.Replace(ArticleTitle, @" \(.*?\)$", ""));

            string ArticleTextHidden = HideMoreText(ArticleText);

            bool IgnoredNoChange = false;
            // remove any self-links
            ArticleText = FixLinks(ArticleText, ArticleTitle, out IgnoredNoChange);

            string ArticleTextAtStart = ArticleText;

            // first quick check: ignore articles with some bold in first 5% of hidemore article
            int fivepc = ArticleTextHidden.Length / 20;
            //ArticleText5.Length
            if (ArticleTextHidden.Substring(0, fivepc).Contains("'''"))
                return ArticleTextAtStart;

            // ignore date articles (date in American or international format)
            if (WikiRegexes.Dates2.IsMatch(ArticleTitle) || WikiRegexes.Dates.IsMatch(ArticleTitle))
                return ArticleTextAtStart;            

            Regex BoldTitleAlready1 = new Regex(@"'''\s*(" + escTitle + "|" + Tools.TurnFirstToLower(escTitle) + @")\s*'''");
            Regex BoldTitleAlready2 = new Regex(@"'''\s*(" + escTitleNoBrackets + "|" + Tools.TurnFirstToLower(escTitleNoBrackets) + @")\s*'''");
            Regex BoldTitleAlready3 = new Regex(@"^\s*({{[^\{\}]+}}\s*)*'''('')?\s*\w");

            //if title in bold already exists in article, or page starts with something in bold, don't change anything
            if (BoldTitleAlready1.IsMatch(ArticleText) || BoldTitleAlready2.IsMatch(ArticleText)
                || BoldTitleAlready3.IsMatch(ArticleText))
                return ArticleTextAtStart;

            Regex regexBold = new Regex(@"([^\[]|^)(" + escTitle + "|" + Tools.TurnFirstToLower(escTitle) + ")([ ,.:;])");
            Regex regexBoldNoBrackets = new Regex(@"([^\[]|^)(" + escTitleNoBrackets + "|" + Tools.TurnFirstToLower(escTitleNoBrackets) + ")([ ,.:;])");

            ArticleTextHidden = HideMoreText(ArticleText);

            // first try title with brackets removed
            if (regexBoldNoBrackets.IsMatch(ArticleTextHidden))
            {
                ArticleText = regexBoldNoBrackets.Replace(ArticleTextHidden, "$1'''$2'''$3", 1);
                ArticleText = AddBackMoreText(ArticleText);

                // check that the bold added is the first bit in bold in the main body of the article
                if (AddedBoldIsValid(ArticleText, escTitleNoBrackets))
                {
                    NoChange = false;
                    return ArticleText;
                }
                return ArticleTextAtStart;
            }

            if (regexBold.IsMatch(ArticleTextHidden))
            {
                ArticleText = regexBold.Replace(ArticleTextHidden, "$1'''$2'''$3", 1);
                ArticleText = AddBackMoreText(ArticleText);

                // check that the bold added is the first bit in bold in the main body of the article
                if (AddedBoldIsValid(ArticleText, escTitle))
                {
                    NoChange = false;
                    return ArticleText;
                }
            }
            return ArticleTextAtStart;
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
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="OldImage">The old image to replace.</param>
        /// <param name="NewImage">The new image.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public static string ReplaceImage(string OldImage, string NewImage, string ArticleText, out bool NoChange)
        {
            string testText = ArticleText;
            ArticleText = ReplaceImage(OldImage, NewImage, ArticleText);

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        /// <summary>
        /// Replaces an iamge in the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="OldImage">The old image to replace.</param>
        /// <param name="NewImage">The new image.</param>
        /// <returns>The new article text.</returns>
        public static string ReplaceImage(string OldImage, string NewImage, string ArticleText)
        {
            ArticleText = FixImages(ArticleText);

            //remove image prefix
            OldImage = Tools.WikiDecode(Regex.Replace(OldImage, "^" + Variables.Namespaces[Namespace.File], "", RegexOptions.IgnoreCase));
            NewImage = Tools.WikiDecode(Regex.Replace(NewImage, "^" + Variables.Namespaces[Namespace.File], "", RegexOptions.IgnoreCase));

            OldImage = Regex.Escape(OldImage).Replace("\\ ", "[ _]");

            OldImage = "((?i:" + WikiRegexes.GenerateNamespaceRegex(Namespace.File, Namespace.Media)
                + @"))\s*:\s*" + Tools.CaseInsensitive(OldImage);
            NewImage = "$1:" + NewImage;

            ArticleText = Regex.Replace(ArticleText, OldImage, NewImage);

            return ArticleText;
        }

        /// <summary>
        /// Removes an image from the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="Image">The image to remove.</param>
        /// <param name="CommentOut"></param>
        /// <param name="Comment"></param>
        /// <returns>The new article text.</returns>
        public static string RemoveImage(string Image, string ArticleText, bool CommentOut, string Comment)
        {
            //remove image prefix
            Image = Tools.WikiDecode(Regex.Replace(Image, "^"
                + Variables.NamespacesCaseInsensitive[Namespace.File], "", RegexOptions.IgnoreCase));
            Image = Tools.CaseInsensitive(HttpUtility.UrlDecode(Regex.Escape(Image).Replace("\\ ", "[ _]")));

            ArticleText = FixImages(ArticleText);

            Regex r = new Regex(@"\[\[\s*:?\s*(?i:"
                + WikiRegexes.GenerateNamespaceRegex(Namespace.File, Namespace.Media)
                + @")\s*:\s*" + Image + @".*\]\]");

            MatchCollection n = r.Matches(ArticleText);
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

                            ArticleText = CommentOut
                                              ? t.Replace(ArticleText, "<!-- " + Comment + " " + match + " -->", 1, m.Index)
                                              : t.Replace(ArticleText, "", 1);

                            break;
                        }
                    }
                }
            }
            else
            {
                r = new Regex("(" + Variables.NamespacesCaseInsensitive[Namespace.File] + ")?" + Image);
                n = r.Matches(ArticleText);

                foreach (Match m in n)
                {
                    Regex t = new Regex(Regex.Escape(m.Value));

                    ArticleText = CommentOut
                                      ? t.Replace(ArticleText, "<!-- " + Comment + " $0 -->", 1, m.Index)
                                      : t.Replace(ArticleText, "", 1, m.Index);
                }
            }

            return ArticleText;
        }

        /// <summary>
        /// Removes an iamge in the article.
        /// </summary>
        /// <param name="Image">The image to remove.</param>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="Comment"></param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <param name="CommentOut"></param>
        /// <returns>The new article text.</returns>
        public static string RemoveImage(string Image, string ArticleText, bool CommentOut, string Comment, out bool NoChange)
        {
            string testText = ArticleText;
            ArticleText = RemoveImage(Image, ArticleText, CommentOut, Comment);

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        /// <summary>
        /// Adds the category to the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NewCategory">The new category.</param>
        /// <param name="ArticleTitle"></param>
        /// <param name="NoChange"></param>
        /// <returns>The article text.</returns>
        public string AddCategory(string NewCategory, string ArticleText, string ArticleTitle, out bool NoChange)
        {
            string testText = ArticleText;
            ArticleText = AddCategory(NewCategory, ArticleText, ArticleTitle);

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        // Covered by: RecategorizerTests.Addition()
        /// <summary>
        /// Adds the category to the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NewCategory">The new category.</param>
        /// <param name="ArticleTitle"></param>
        /// <returns>The article text.</returns>
        public string AddCategory(string NewCategory, string ArticleText, string ArticleTitle)
        {
            string oldText = ArticleText;

            ArticleText = FixCategories(ArticleText);

            if (Regex.IsMatch(ArticleText, @"\[\["
                + Variables.NamespacesCaseInsensitive[Namespace.Category]
                + Regex.Escape(NewCategory) + @"[\|\]]"))
            {
                return oldText;
            }

            string cat = "\r\n[[" + Variables.Namespaces[Namespace.Category] + NewCategory + "]]";
            cat = Tools.ApplyKeyWords(ArticleTitle, cat);

            if (Namespace.Determine(ArticleTitle) == Namespace.Template)
                ArticleText += "<noinclude>" + cat + "\r\n</noinclude>";
            else
                ArticleText += cat;

            return SortMetaData(ArticleText, ArticleTitle); //Sort metadata ordering so general fixes dont need to be enabled
        }

        // Covered by: RecategorizerTests.Replacement()
        /// <summary>
        /// Re-categorises the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="OldCategory">The old category to replace.</param>
        /// <param name="NewCategory">The new category.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The re-categorised article text.</returns>
        public static string ReCategoriser(string OldCategory, string NewCategory, string ArticleText, out bool NoChange)
        {
            //remove category prefix
            OldCategory = Regex.Replace(OldCategory, "^"
                + Variables.NamespacesCaseInsensitive[Namespace.Category], "", RegexOptions.IgnoreCase);
            NewCategory = Regex.Replace(NewCategory, "^"
                + Variables.NamespacesCaseInsensitive[Namespace.Category], "", RegexOptions.IgnoreCase);

            //format categories properly
            ArticleText = FixCategories(ArticleText);

            string testText = ArticleText;

            if (Regex.IsMatch(ArticleText, "\\[\\["
                + Variables.NamespacesCaseInsensitive[Namespace.Category]
                + Tools.CaseInsensitive(Regex.Escape(NewCategory)) + @"\s*(\||\]\])"))
            {
                bool tmp;
                ArticleText = RemoveCategory(OldCategory, ArticleText, out tmp);
            }
            else
            {
                OldCategory = Regex.Escape(OldCategory);
                OldCategory = Tools.CaseInsensitive(OldCategory);

                OldCategory = Variables.Namespaces[Namespace.Category] + OldCategory + @"\s*(\||\]\])";
                NewCategory = Variables.Namespaces[Namespace.Category] + NewCategory + "$1";

                ArticleText = Regex.Replace(ArticleText, OldCategory, NewCategory);
            }

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        // Covered by: RecategorizerTests.Removal()
        /// <summary>
        /// Removes a category from an article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="strOldCat">The old category to remove.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The article text without the old category.</returns>
        public static string RemoveCategory(string strOldCat, string ArticleText, out bool NoChange)
        {
            ArticleText = FixCategories(ArticleText);
            string testText = ArticleText;

            strOldCat = Regex.Escape(strOldCat);
            strOldCat = Tools.CaseInsensitive(strOldCat);

            if (!ArticleText.Contains("<includeonly>"))
                ArticleText = Regex.Replace(ArticleText, "\\[\\["
                    + Variables.NamespacesCaseInsensitive[Namespace.Category] + " ?"
                    + strOldCat + "( ?\\]\\]| ?\\|[^\\|]*?\\]\\])\r\n", "");

            ArticleText = Regex.Replace(ArticleText, "\\[\\["
                + Variables.NamespacesCaseInsensitive[Namespace.Category] + " ?"
                + strOldCat + "( ?\\]\\]| ?\\|[^\\|]*?\\]\\])", "");

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        private static readonly string catregex = @"\[\[\s*" + Variables.NamespacesCaseInsensitive[Namespace.Category] +
                  @"\s*(.*?)\s*(?:|\|([^\|\]]*))\s*\]\]";

        // Covered by: UtilityFunctionTests.ChangeToDefaultSort()
        /// <summary>
        /// Changes an article to use defaultsort when all categories use the same sort field / cleans diacritics from defaultsort/categories
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">Title of the article</param>
        /// <param name="NoChange">If there is no change (True if no Change)</param>
        /// <returns>The article text possibly using defaultsort.</returns>
        public static string ChangeToDefaultSort(string ArticleText, string ArticleTitle, out bool NoChange)
        {
            string testText = ArticleText;
            NoChange = true;

            // count categories
            string sort = null;
            bool allsame = true;
            int matches = 0;

            foreach (Match m in Regex.Matches(ArticleText, catregex))
            {
                string explicitKey = m.Groups[2].Value;
                if (explicitKey.Length == 0) explicitKey = ArticleTitle;

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
            MatchCollection ds = WikiRegexes.Defaultsort.Matches(ArticleText);
            if (WikiRegexes.Lifetime.IsMatch(ArticleText) || ds.Count > 1 || (ds.Count == 1 && !ds[0].Value.ToUpper().Contains("DEFAULTSORT")))
                return ArticleText;


            ArticleText = TalkPages.TalkPageHeaders.FormatDefaultSort(ArticleText);

            // match again, after normalisation
            ds = WikiRegexes.Defaultsort.Matches(ArticleText);
            if (ds.Count > 1)
                return testText;

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#AWB_didn.27t_fix_special_characters_in_a_pipe
            ArticleText = FixCategories(ArticleText);

            if (ds.Count == 0)
            {
                if (allsame && matches > 1 && !string.IsNullOrEmpty(sort))
                {
                    if (sort.Length > 4 && // So that this doesn't get confused by sort keys of "*", " ", etc.
                        !sort.StartsWith(" "))
                    // MW bug: DEFAULTSORT doesn't treat leading spaces the same way as categories do
                    {
                        ArticleText = Regex.Replace(ArticleText, catregex, "[["
                            + Variables.Namespaces[Namespace.Category] + "$1]]");

                        if (Tools.FixupDefaultSort(sort) != ArticleTitle)
                            ArticleText = ArticleText + "\r\n{{DEFAULTSORT:" + Tools.FixupDefaultSort(sort) + "}}";
                    }
                }
                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Add_defaultsort_to_pages_with_special_letters_and_no_defaultsort
                ArticleText = DefaultsortTitlesWithDiacritics(ArticleText, ArticleTitle, matches);
            }
            else // already has DEFAULTSORT
            {
                string s = Tools.FixupDefaultSort(ds[0].Groups[1].Value).Trim();
                if (s != ds[0].Groups[1].Value && s.Length > 0)
                    ArticleText = ArticleText.Replace(ds[0].Value, "{{DEFAULTSORT:" + s + "}}");

            }

            if (ds.Count == 1)
            {
                string DefaultsortKey = ds[0].Groups["key"].Value;
                //Removes any explicit keys that are case insensitively the same as the default sort (To help tidy up on pages that already have defaultsort)
                ArticleText = ExplicitCategorySortkeys(ArticleText, DefaultsortKey);
            }

            NoChange = (testText == ArticleText);
            return ArticleText;
        }

        /// <summary>
        /// Removes any explicit keys that are case insensitively the same as the default sort (To help tidy up on pages that already have defaultsort)
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The article text.</returns>
        private static string ExplicitCategorySortkeys(string ArticleText, string DefaultsortKey)
        {
            foreach (Match m in Regex.Matches(ArticleText, catregex))
            {
                string explicitKey = m.Groups[2].Value;
                if (explicitKey.Length == 0)
                    continue;

                if (string.Compare(explicitKey, DefaultsortKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    ArticleText = ArticleText.Replace(m.Value,
                        "[[" + Variables.Namespaces[Namespace.Category] + m.Groups[1].Value + "]]");
                }
            }
            return (ArticleText);
        }

        /// <summary>
        /// if title has diacritics, no defaultsort added yet, adds a defaultsort with cleaned up title as sort key
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">Title of the article</param>
        /// <param name="Matches">If there is no change (True if no Change)</param>
        /// <returns>The article text possibly using defaultsort.</returns>
        private static string DefaultsortTitlesWithDiacritics(string ArticleText, string ArticleTitle, int Matches)
        {
            // need some categories and no defaultsort
            if (Tools.FixupDefaultSort(ArticleTitle) != ArticleTitle && Matches > 0 &&
                !WikiRegexes.Defaultsort.IsMatch(ArticleText))
            {
                string sortkey = Tools.FixupDefaultSort(ArticleTitle);

                ArticleText = ArticleText + "\r\n{{DEFAULTSORT:" + sortkey + "}}";

                return (ExplicitCategorySortkeys(ArticleText, sortkey));
            }
            return (ArticleText);
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
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string RemoveDuplicateWikiLinks(string ArticleText)
        {
            ArticleText = DupeLinks1.Replace(ArticleText, "[[$1|$2]]$3$2");
            return DupeLinks2.Replace(ArticleText, "[[$1]]$2$1");
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
