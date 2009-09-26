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
    //TODO:Make Regexes Compiled as required
    //TODO:Move Regexes to WikiRegexes as required
    //TODO:Split Parser code into separate files (for manageability), or even into separate classes
    //TODO:Move regexes declared in method bodies (if not dynamic based on article title, etc), into class body

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
            if (Variables.LangCode != "nl")
                RegexTagger.Add(new Regex(@"\{\{(template:)?(Clean( ?up)?|CU|Tidy)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Cleanup|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Linkless|Orphan)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Orphan|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Uncategori[sz]ed|Uncat|Classify|Category needed|Catneeded|categori[zs]e|nocats?)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Uncategorized|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser#AWB_problems
            // on en wiki {{references}} is a cleanup tag meaning 'this article needs more references' (etc.) whereas on nl wiki (Sjabloon:References) {{references}} acts like <references/>
            if (Variables.LangCode != "nl")
                RegexTagger.Add(new Regex(@"\{\{(template:)?(Unreferenced(sect)?|add references|cite[ -]sources?|cleanup-sources?|needs? references|no sources|no references?|not referenced|references|unref|unsourced)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Unreferenced|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Trivia2?|Too ?much ?trivia|Trivia section|Cleanup-trivia)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Trivia|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(deadend|DEP)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Deadend|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(copyedit|g(rammar )?check|copy-edit|cleanup-copyedit|cleanup-english)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{copyedit|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(sources|refimprove|not verified)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Refimprove|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Uncategori[zs]edstub|uncatstub)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Uncategorizedstub|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Importance)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Importance|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Expand)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Expand|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Fact)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Fact|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(COI|Conflict of interest|Selfpromotion)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{COI|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");

            RegexConversion.Add(new Regex(@"\{\{(?:Template:)?(Dab|Disamb|Disambiguation)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Disambig}}");
            RegexConversion.Add(new Regex(@"\{\{(?:Template:)?(Bio-dab|Hndisambig)", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Hndis");

            RegexConversion.Add(new Regex(@"\{\{(?:Template:)?(Prettytable|Prettytable100)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{subst:Prettytable}}");
            RegexConversion.Add(new Regex(@"\{\{(?:[Tt]emplate:)?((?:BASE)?PAGENAMEE?\}\}|[Ll]ived\||[Bb]io-cats\|)", RegexOptions.Compiled), "{{subst:$1");

            // clean 'do-attempt =July 2006|att=April 2008' to 'do attempt = April 2008'
            RegexConversion.Add(new Regex(@"({{\s*[Aa]rticle ?issues\s*(?:\|[^{}]*|\|)\s*[Dd]o-attempt\s*=\s*)[^{}\|]+\|\s*att\s*=\s*([^{}\|]+)(?=\||}})", RegexOptions.Compiled), "$1$2");

            // clean "Copyedit for=grammar|date=April 2009"to "Copyedit=April 2009"
            RegexConversion.Add(new Regex(@"({{\s*[Aa]rticle ?issues\s*(?:\|[^{}]*|\|)\s*[Cc]opyedit\s*)for\s*=\s*[^{}\|]+\|\s*date(\s*=[^{}\|]+)(?=\||}})", RegexOptions.Compiled), "$1$2");

            // could be multiple to date per template so loop
            for (int a = 0; a < 5; a++)
            {
                // add date to any undated tags within {{Article issues}}
                RegexConversion.Add(new Regex(@"({{\s*[Aa]rticle ?issues\s*(?:\|[^{}]*|\|)\s*)(?![Ee]xpert)" + WikiRegexes.ArticleIssuesTemplatesString + @"\s*(\||}})", RegexOptions.Compiled), "$1$2={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}$3");

                // clean any 'date' word within {{Article issues}} (but not 'update' or 'out of date' fields), place after the date adding rule above
                RegexConversion.Add(new Regex(@"(?<={{\s*[Aa]rticle ?issues\s*(?:\|[^{}]*?)?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}[^{}]*?){0,4}\|[^{}\|]{3,}?)\b(?i)date(?<!.*out of.*)", RegexOptions.Compiled), "");
            }

            // articleissues with one issue -> single issue tag (e.g. {{articleissues|cleanup=January 2008}} to {{cleanup|date=January 2008}} etc.)
            RegexConversion.Add(new Regex(@"\{\{[Aa]rticle ?issues\s*\|\s*([^\|{}=]{3,}?)\s*(=\s*\w{3,10}\s+20\d\d)\s*\}\}", RegexOptions.Compiled), "{{$1|date$2}}");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_empty_.7B.7BArticle_issues.7D.7D
            // articleissues with no issues -> remove tag
            RegexConversion.Add(new Regex(@"\{\{[Aa]rticle ?issues(?:\s*\|\s*(?:section|article)\s*=\s*[Yy])?\s*\}\}", RegexOptions.Compiled), "");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.7B.7Bcommons.7CCategory:XXX.7D.7D_.3E_.7B.7Bcommonscat.7CXXX.7D.7D
            RegexConversion.Add(new Regex(@"\{\{[Cc]ommons\|\s*[Cc]ategory:\s*([^{}]+?)\s*\}\}", RegexOptions.Compiled), @"{{Commons category|$1}}");

            //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Commons_category
            RegexConversion.Add(new Regex(@"(?<={{[Cc]ommons cat(?:egory)?\|\s*)([^{}\|]+?)\s*\|\s*\1\s*}}", RegexOptions.Compiled), @"$1}}");

            // tidy up || or |}} (maybe with whitespace between) within templates that don't use null parameters
            RegexConversion.Add(new Regex(@"(\{\{\s*(?:[Cc]it|[Aa]rticle ?issues)[^{}]*)\|\s*(\}\}|\|)", RegexOptions.Compiled), "$1$2");

            // remove duplicate / populated and null fields in cite/article issues templates
            RegexConversion.Add(new Regex(@"({{\s*(?:[Cc]it|[Aa]rticle ?issues)[^{}]*\|\s*)(\w+)\s*=\s*([^\|}{]+?)\s*\|((?:[^{}]*?\|)?\s*)\2(\s*=\s*)\3(\s*(\||\}\}))", RegexOptions.Compiled), "$1$4$2$5$3$6"); // duplicate field remover for cite templates
            RegexConversion.Add(new Regex(@"(\{\{\s*(?:[Cc]it|[Aa]rticle ?issues)[^{}]*\|\s*)(\w+)(\s*=\s*[^\|}{]+(?:\|[^{}]+?)?)\|\s*\2\s*=\s*(\||\}\})", RegexOptions.Compiled), "$1$2$3$4"); // 'field=populated | field=null' drop field=null
            RegexConversion.Add(new Regex(@"(\{\{\s*(?:[Cc]it|[Aa]rticle ?issues)[^{}]*\|\s*)(\w+)\s*=\s*\|\s*((?:[^{}]+?\|)?\s*\2\s*=\s*[^\|}{\s])", RegexOptions.Compiled), "$1$3"); // 'field=null | field=populated' drop field=null

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Add_.7B.7Botheruse.7D.7D_and_.7B.7B2otheruses.7D.7D_in_the_supported_DABlinks
            RegexConversion.Add(new Regex(@"({{)2otheruses(\s*(?:\|[^{}]*}}|}}))", RegexOptions.Compiled), "$1Two other uses$2");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#AWB_is_still_using_.7B.7BArticleissues.7D.7D_instead_of_.7B.7BArticle_issues.7D.7D
            // replace any {{articleissues}} with {{article issues}}
            RegexConversion.Add(new Regex(@"(?<={{[Aa]rticle)i(?=ssues.*}})", RegexOptions.Compiled), " i");

            // http://en.wikipedia.org/wiki/Template_talk:Citation_needed#Requested_move
            if (Variables.LangCode == "en")
                RegexConversion.Add(new Regex(@"{{\s*(?:[Cc]n|[Ff]act|[Pp]roveit|[Cc]iteneeded|[Uu]ncited)(?=\s*[\|}])", RegexOptions.Compiled), @"{{Citation needed");
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

        /// <summary>
        /// Re-organises the Person Data, stub/disambig templates, categories and interwikis
        /// except when a mainspace article has some 'includeonly' tags etc.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article title.</param>
        /// <returns>The re-organised text.</returns>
        public string SortMetaData(string articleText, string articleTitle)
        {
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Substituted_templates
            // if article contains some substituted template stuff, sorting the data may mess it up (further)
            if (Namespace.IsMainSpace(articleTitle) && NoIncludeIncludeOnlyProgrammingElement(articleText))
                return articleText;

            return (Variables.Project <= ProjectEnum.species) ? Sorter.Sort(articleText, articleTitle) : articleText;
        }

        private static readonly Regex ApostropheInDecades = new Regex(@"(?<=(?:the |later? |early |mid-|[12]\d\d0'?s and )(?:\[?\[?[12]\d\d0\]?\]?))'s(?=\]\])?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
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

        private static readonly Regex RegexRemoveLinksInHeadings = new Regex(@"^ *((={1,4})[^\[\]\{\}\|=\r\n]*)\[\[(?:[^\[\]\{\}\|=\r\n]+\|)?([^\[\]\{\}\|\r\n]+)\]\]([^\{\}=\r\n]*\2)", RegexOptions.Compiled | RegexOptions.Multiline);

        private static readonly Regex RegexBadHeader = new Regex("^(={1,4} ?(about|description|overview|definition|profile|(?:general )?information|background|intro(?:duction)?|summary|bio(?:graphy)?) ?={1,4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex RegexHeadingWhitespaceBefore = new Regex(@"^ *(==+)(\s*.+?\s*)\1 +(\r|\n)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex RegexHeadingWhitespaceAfter = new Regex(@"^ +(==+)(\s*.+?\s*)\1 *(\r|\n)", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex RegexHeadingUpOneLevel = new Regex(@"^=(==+[^=].*?[^=]==+)=(\r\n?|\n)$", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex ReferencesExternalLinksSeeAlso = new Regex(@"== *([Rr]eferences|[Ee]xternal +[Ll]inks?|[Ss]ee +[Aa]lso) *==\s", RegexOptions.Compiled);

        private static readonly Regex RegexHeadingColonAtEnd = new Regex(@"^(=+)(.+?)\:(\s*\1(?:\r\n?|\n))$", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex RegexHeadingWithBold = new Regex(@"(?<====+.*?)'''(.*?)'''(?=.*?===+)", RegexOptions.Compiled);

        /// <summary>
        /// Fix ==See also== and similar section common errors.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">the title of the article</param>
        /// <param name="noChange">Value that indicates whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public static string FixHeadings(string articleText, string articleTitle, out bool noChange)
        {
            string newText = FixHeadings(articleText, articleTitle);

            noChange = (newText == articleText);

            return newText.Trim();
        }

        private const int MinCleanupTagsToCombine = 3; // article must have at least this many tags to combine to {{Article issues}}

        /// <summary>
        /// Combines multiple cleanup tags into {{article issues}} template, ensures parameters have correct case, removes date parameter where not needed
        /// only for English-language wikis
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string ArticleIssues(string articleText)
        {
            if (Variables.LangCode != "en")
                return articleText;

            // convert title case parameters within {{Article issues}} to lower case
            foreach (Match m in WikiRegexes.ArticleIssuesInTitleCase.Matches(articleText))
            {
                string firstPart = m.Groups[1].Value;
                string parameterFirstChar = m.Groups[2].Value.ToLower();
                string lastPart = m.Groups[3].Value;

                articleText = articleText.Replace(m.Value, firstPart + parameterFirstChar + lastPart);
            }

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Date_parameter_getting_stripped_from_.7B.7BArticle_issues.7D.7D
            // remove any date field within  {{Article issues}} if no 'expert' field using it
            if (!WikiRegexes.ArticleIssuesRegexExpert.IsMatch(articleText))
                articleText = WikiRegexes.ArticleIssuesRegexWithDate.Replace(articleText, "$1$2");

            string newTags = "";

            // get the zeroth section (text upto first heading)
            string zerothSection = WikiRegexes.ZerothSection.Match(articleText).Value;

            // get the rest of the article including first heading (may be null if entire article falls in zeroth section)
            string restOfArticle = zerothSection.Length > 0 ? articleText.Replace(zerothSection, "") : "";

            int tagsToAdd = WikiRegexes.ArticleIssuesTemplates.Matches(zerothSection).Count;

            // if currently no {{Article issues}} and less than the min number of cleanup templates, do nothing
            if (!WikiRegexes.ArticleIssues.IsMatch(zerothSection) && WikiRegexes.ArticleIssuesTemplates.Matches(zerothSection).Count < MinCleanupTagsToCombine)
                return (articleText);

            // only add tags to articleissues if new tags + existing >= MinCleanupTagsToCombine
            if ((WikiRegexes.ArticleIssuesTemplateNameRegex.Matches(WikiRegexes.ArticleIssues.Match(zerothSection).Value).Count + tagsToAdd) < MinCleanupTagsToCombine || tagsToAdd == 0)
                return (articleText);

            foreach (Match m in WikiRegexes.ArticleIssuesTemplates.Matches(zerothSection))
            {
                // all fields except COI, OR, POV and ones with BLP should be lower case
                string singleTag = m.Groups[1].Value;
                string tagValue = m.Groups[2].Value;
                if (!WikiRegexes.CoiOrPovBlp.IsMatch(singleTag))
                    singleTag = singleTag.ToLower();

                // expert must have a parameter
                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_12#Article_Issues
                if (singleTag == "expert" && tagValue.Trim().Length == 0)
                    continue;

                // for tags with a parameter, that parameter must be the date
                if ((tagValue.Contains("=") && Regex.IsMatch(tagValue, @"(?i)date")) || tagValue.Length == 0)
                    newTags += @"|" + singleTag + @" " + tagValue;
                else
                    continue;
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
        /// <param name="articleTitle">the title of the article</param>
        /// <returns>The modified article text.</returns>
        public static string FixHeadings(string articleText, string articleTitle)
        {
            articleText = Regex.Replace(articleText, "^={1,4} ?" + Regex.Escape(articleTitle) + " ?={1,4}", "", RegexOptions.IgnoreCase);
            articleText = RegexBadHeader.Replace(articleText, "");

            // only apply if < 6 matches, otherwise (badly done) articles with 'list of...' and lots of links in headings will be further messed up
            if (RegexRemoveLinksInHeadings.Matches(articleText).Count < 6
                && !(Regex.IsMatch(articleTitle, WikiRegexes.Months) || articleTitle.StartsWith(@"List of") || WikiRegexes.GregorianYear.IsMatch(articleTitle)))
            {
                // loop through in case a heading has mulitple wikilinks in it
                while (RegexRemoveLinksInHeadings.IsMatch(articleText))
                {
                    articleText = RegexRemoveLinksInHeadings.Replace(articleText, "$1$3$4");
                }
            }

            if (!Regex.IsMatch(articleText, "= ?See also ?="))
                articleText = RegexHeadings0.Replace(articleText, "$1See also$3");

            articleText = RegexHeadings1.Replace(articleText, "$1External links$3");
            //articleText = regexHeadings2.Replace(articleText, "$1External link$3");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#ReferenceS
            Match refsHeader = RegexHeadings3.Match(articleText);
            string refsheader1 = refsHeader.Groups[1].Value;
            string refsheader2 = refsHeader.Groups[2].Value;
            string refsheader3 = refsHeader.Groups[3].Value;
            if (refsheader2.Length > 0)
                articleText = articleText.Replace(refsheader1 + refsheader2 + refsheader3,
                                                  refsheader1 + "Reference" + refsheader3.ToLower());

            Match sourcesHeader = RegexHeadings4.Match(articleText);
            string sourcesheader1 = sourcesHeader.Groups[1].Value;
            string sourcesheader2 = sourcesHeader.Groups[2].Value;
            string sourcesheader3 = sourcesHeader.Groups[3].Value;
            if (sourcesheader2.Length > 0)
                articleText = articleText.Replace(sourcesheader1 + sourcesheader2 + sourcesheader3,
                                                  sourcesheader1 + "Source" + sourcesheader3.ToLower());

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
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Standard_level_2_headers
            // don't consider the "references", "see also", or "external links" level 2 headings when counting level two headings
            string articleTextLocal = articleText;
            articleTextLocal = ReferencesExternalLinksSeeAlso.Replace(articleTextLocal, "");

            // only apply if all level 3 headings and lower are before the fist of references/external links/see also
            if (!WikiRegexes.HeadingLevelTwo.IsMatch(articleTextLocal))
            {
                int upone = 0;
                foreach (Match m in RegexHeadingUpOneLevel.Matches(articleText))
                {
                    if (m.Index > upone)
                        upone = m.Index;
                }

                if (!ReferencesExternalLinksSeeAlso.IsMatch(articleText) || (upone < ReferencesExternalLinksSeeAlso.Match(articleText).Index))
                    articleText = RegexHeadingUpOneLevel.Replace(articleText, "$1$2");
            }

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Bold_text_in_headers
            // remove bold from level 3 headers and below, as it makes no visible difference
            articleText = RegexHeadingWithBold.Replace(articleText, "$1");

            return articleText;
        }

        // fixes extra comma in American format dates
        private static Regex CommaDates = new Regex(WikiRegexes.Months + @" ?, *([1-3]?\d) ?, ?((?:200|19\d)\d)\b");

        // Covered by: LinkTests.FixDates()
        /// <summary>
        /// Fix date and decade formatting errors, and replace &lt;br&gt; and &lt;p&gt; HTML tags
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixDates(string articleText)
        {
            if(Variables.LangCode == "en")
                articleText = CommaDates.Replace(articleText, @"$1 $2, $3");

            articleText = HideMoreText(articleText);

            articleText = FixDatesRaw(articleText);

            //Remove 2 or more <br />'s
            //This piece's existance here is counter-intuitive, but it requires HideMore()
            //and I don't want to call this slow function yet another time --MaxSem
            articleText = SyntaxRemoveBr.Replace(articleText, "\r\n");
            articleText = SyntaxRemoveParagraphs.Replace(articleText, "\r\n\r\n");

            return AddBackMoreText(articleText);
        }

        private static readonly Regex DiedDateRegex =
            new Regex(
                @"('''[^']+'''\s*\()d\.(\s+\[*(?:" + WikiRegexes.MonthsNoGroup + @"\s+0?([1-3]?[0-9])|0?([1-3]?[0-9])\s*" +
                WikiRegexes.MonthsNoGroup + @")?\]*\s*\[*[1-2]?\d{3}\]*\)\s*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex DOBRegex =
            new Regex(
                @"('''[^']+'''\s*\()b\.(\s+\[*(?:" + WikiRegexes.MonthsNoGroup + @"\s+0?([1-3]?\d)|0?([1-3]?\d)\s*" +
                WikiRegexes.MonthsNoGroup + @")?\]*\s*\[*[1-2]?\d{3}\]*\)\s*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex BornDeathRegex =
            new Regex(
                @"('''[^']+'''\s*\()(?:[Bb]orn|b\.)\s+(\[*(?:" + WikiRegexes.MonthsNoGroup +
                @"\s+0?(?:[1-3]?\d)|0?(?:[1-3]?\d)\s*" + WikiRegexes.MonthsNoGroup +
                @")?\]*,?\s*\[*[1-2]?\d{3}\]*)\s*(.|&.dash;)\s*(?:[Dd]ied|d\.)\s+(\[*(?:" + WikiRegexes.MonthsNoGroup +
                @"\s+0?(?:[1-3]?\d)|0?(?:[1-3]?\d)\s*" + WikiRegexes.MonthsNoGroup + @")\]*,?\s*\[*[1-2]?\d{3}\]*\)\s*)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
        /// NOT READY FOR PRODUCTION. Footnote formatting errors per [[WP:FN]].
        /// currently too buggy to be included into production builds
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixFootnotes(string articleText)
        {
            // One space/linefeed
            articleText = WikiRegexes.WhitespaceRef.Replace(articleText, "<ref$1");
            // remove trailing spaces from named refs
            articleText = WikiRegexes.RefTagWithParams.Replace(articleText, "<ref $1>");
            // removed superscripted punctuation between refs
            articleText = WikiRegexes.SuperscriptedPunctuationBetweenRefs.Replace(articleText, "$1<ref");
            articleText = WikiRegexes.PunctuationBetweenRefs.Replace(articleText, "$1<ref");

            articleText = WikiRegexes.WhitespaceFactTag.Replace(articleText, "{{$1}}");

            string oldArticleText = "";

            while (oldArticleText != articleText)
            { // repeat for multiple refs together
                oldArticleText = articleText;
                articleText = WikiRegexes.Match0A.Replace(articleText, "$1$2$4$3");
                articleText = WikiRegexes.Match0B.Replace(articleText, "$1$2$4$3");
                //articleText = WikiRegexes.match0C.Replace(articleText, "$2$4$3");
                articleText = WikiRegexes.Match0D.Replace(articleText, "$1$2$3");

                articleText = WikiRegexes.Match1A.Replace(articleText, "$1$2$6$3");
                articleText = WikiRegexes.Match1B.Replace(articleText, "$1$2$6$3");
                //articleText = WikiRegexes.match1C.Replace(articleText, "$2$6$3");
                articleText = WikiRegexes.Match1D.Replace(articleText, "$1$2$3");
            }

            //articleText = WikiRegexes.RefAfterEquals.Replace(articleText, "$1\r\n<ref");
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

            // do not reorder stuff in the <references>...</references> section
            int referencestags = articleText.IndexOf(@"<references>");
            if (referencestags <= 0)
                referencestags = articleText.Length;

            for (int i = 0; i < 5; i++) // allows for up to 5 consecutive references
            {
                articleTextBefore = articleText;

                foreach (Match m in OutofOrderRefs1.Matches(articleText))
                {
                    string ref1 = m.Groups[1].Value;
                    int ref1Index = Regex.Match(articleText, @"(?i)<ref\s+name\s*=\s*(?:""|')?" + Regex.Escape(m.Groups[4].Value) + @"(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>").Index;
                    int ref2Index = articleText.IndexOf(ref1);                    

                    if (ref1Index < ref2Index && ref2Index > 0 && m.Groups[3].Index < referencestags)
                    {
                        string whitespace = m.Groups[2].Value;
                        string rptemplate = m.Groups[5].Value;
                        string ref2 = m.Groups[3].Value;
                        articleText = articleText.Replace(ref1 + whitespace + ref2 + rptemplate, ref2 + rptemplate + whitespace + ref1);
                    }
                }

                articleText = ReorderRefs(articleText, OutofOrderRefs2, referencestags);
                articleText = ReorderRefs(articleText, OutofOrderRefs3, referencestags);

                if (articleTextBefore == articleText)
                    break;
            }

            return articleText;
        }

        /// <summary>
        /// reorders references within the article text based on the input regular expression providing matches for references that are out of numerical order
        /// </summary>
        /// <param name="articleText">the wiki text of the article</param>
        /// <param name="outofOrderRegex">a regular expression representing two references that are out of numerical order</param>
        /// <returns>the modified article text</returns>
        private static string ReorderRefs(string articleText, Regex outofOrderRegex, int referencestagindex)
        {
            foreach (Match m in outofOrderRegex.Matches(articleText))
            {
                int ref1Index = Regex.Match(articleText, @"(?i)<ref\s+name\s*=\s*(?:""|')?" + Regex.Escape(m.Groups[2].Value) + @"(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>").Index;
                int ref2Index = Regex.Match(articleText, @"(?i)<ref\s+name\s*=\s*(?:""|')?" + Regex.Escape(m.Groups[6].Value) + @"(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>").Index;
                
                if (ref1Index > ref2Index && ref1Index > 0 && m.Groups[5].Index < referencestagindex)
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

        private static readonly Regex NamedReferences = new Regex(@"(<\s*ref\s+name\s*=\s*(?:""|')?([^<>=\r\n]+?)(?:""|')?\s*>\s*([^<>]+?)\s*<\s*/\s*ref>)", RegexOptions.Singleline | RegexOptions.Compiled);

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

                        articleText = r.Replace(articleText, "\u230A\u230A\u230A\u230A@@\u230B\u230B\u230B\u230B", 1);
                    }
                    else // replace all duplicates
                        articleText = articleText.Replace(m2.Value, @"<ref name=""" + refName + @"""/>");

                    a++;
                }

                // unmask first match
                articleText = articleText.Replace("\u230A\u230A\u230A\u230A@@\u230B\u230B\u230B\u230B", firstref);

                // duplicate citation fixer (first named): <ref name="Fred">(...)</ref>....<ref>\2</ref> --> ..<ref name="Fred"/>
                // duplicate citation fixer (second named): <ref>(...)</ref>....<ref name="Fred">\2</ref> --> ..<ref name="Fred"/>
                foreach (Match m3 in Regex.Matches(articleText, @"<\s*ref\s*>\s*" + Regex.Escape(unnamedRefValue) + @"\s*<\s*/\s*ref>"))
                {
                    articleText = articleText.Replace(m3.Value, @"<ref name=""" + refName + @"""/>");
                }
            }

            return articleText;
        }

        private const string RefName = @"(?si)<\s*ref\s+name\s*=\s*""";
        private static readonly Regex UnnamedRef = new Regex(@"<\s*ref\s*>\s*([^<>]+)\s*<\s*/\s*ref>", RegexOptions.Singleline | RegexOptions.Compiled);

        private struct Ref
        {
            public string Text;
            public string InnerText;
        }

        /// <summary>
        /// Derives and sets a reference name per [[WP:REFNAME]] for duplicate &lt;ref&gt;s
        /// </summary>
        /// <param name="articleText">the text of the article</param>
        /// <returns>the modified article text</returns>
        public static string DuplicateUnnamedReferences(string articleText)
        {
            var refs = new Dictionary<int, List<Ref>>();
            bool haveRefsToFix = false;
            foreach (Match m in UnnamedRef.Matches(articleText))
            {
                var str = m.Value;

                // ref contains ibid/op cit, don't combine it, could refer to any ref on page
                if (WikiRegexes.IbidOpCitation.IsMatch(str)) continue;
                
                string inner = m.Groups[1].Value.Trim();
                int hash = inner.GetHashCode();
                List<Ref> list;
                if (!refs.ContainsKey(hash))
                {
                    list = new List<Ref>();
                    refs[hash] = list;
                }
                else
                {
                    list = refs[hash];
                    haveRefsToFix = true;
                }

                list.Add(new Ref { Text = str, InnerText = inner });
            }

            if (!haveRefsToFix)
                return articleText;

            StringBuilder result = new StringBuilder(articleText);

            foreach (var list in refs.Values)
            {
                if (list.Count < 2) continue; // nothing to consolidate

                // get the reference name to use
                string friendlyName = DeriveReferenceName(articleText, list[0].InnerText);

                // check reference name not already in use for some other reference
                if (friendlyName.Length <= 3 || Regex.IsMatch(articleText, RefName + Regex.Escape(friendlyName) + @"""\s*/?\s*>"))
                    continue;

                for (int i = 0; i < list.Count; i++)
                {
                    StringBuilder newValue = new StringBuilder();

                    newValue.Append(@"<ref name=""");
                    newValue.Append(friendlyName);
                    newValue.Append('"');

                    if (i == 0)
                    {
                        newValue.Append('>');
                        newValue.Append(list[0].InnerText);
                        newValue.Append("</ref>");

                        Tools.ReplaceOnce(result, list[0].Text, newValue.ToString());
                    }
                    else
                    {
                        newValue.Append("/>");
                        result.Replace(list[i].Text, newValue.ToString());
                    }
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Extracts strings from an input string using the input regex to derive a name for a reference
        /// </summary>
        /// <param name="reference">value of the reference needing a name</param>
        /// <param name="referenceNameMask">regular expression to apply</param>
        /// <param name="components">number of groups to extract</param>
        /// <returns>the derived reference name</returns>
        private static string ExtractReferenceNameComponents(string reference, Regex referenceNameMask, int components)
        {
            string referenceName = "";

            if (referenceNameMask.Matches(reference).Count > 0)
            {
                Match m = referenceNameMask.Match(reference);

                referenceName = m.Groups[1].Value;

                if (components > 1)
                    referenceName += " " + m.Groups[2].Value;

                if (components > 2)
                    referenceName += " " + m.Groups[3].Value;
            }

            return CleanDerivedReferenceName(referenceName);
        }

        private const string CharsToTrim = @".;: {}[]|`?\/$’‘-_–=+,";
        // U230A is Floor Left; U230B is Floor Right
        private static readonly Regex CommentOrFloorNumber = new Regex(@"(\<\!--.*?--\>|" + "\u230A" + @"{3,}\d+" + "\u230B" + "{3,})", RegexOptions.Compiled);
        private static readonly Regex SequenceOfQuotesInDerivedName = new Regex(@"(''+|[“‘”""\[\]\(\)\<\>" + "\u230A\u230B" + "])", RegexOptions.Compiled);
        private static readonly Regex WhitespaceInDerivedName = new Regex(@"(\s{2,}|&nbsp;|\t|\n)", RegexOptions.Compiled);
        private static readonly Regex DateRetrievedOrAccessed = new Regex(@"(?im)(\s*(date\s+)?(retrieved|accessed)\b|^\d+$)", RegexOptions.Compiled);
        /// <summary>
        /// Removes various unwanted punctuation and comment characters from a derived reference name
        /// </summary>
        /// <param name="derivedName">the input reference name</param>
        /// <returns>the cleaned reference name</returns>
        private static string CleanDerivedReferenceName(string derivedName)
        {
            derivedName = WikiRegexes.PipedWikiLink.Replace(derivedName, "$2"); // piped wikilinks -> text value

            derivedName = CommentOrFloorNumber.Replace(derivedName, "");
            // rm comments from ref name, might be masked
            derivedName = derivedName.Trim(CharsToTrim.ToCharArray());
            derivedName = SequenceOfQuotesInDerivedName.Replace(derivedName, ""); // remove chars
            derivedName = WhitespaceInDerivedName.Replace(derivedName, " "); // spacing fixes
            derivedName = derivedName.Replace(@"&ndash;", "–");

            return DateRetrievedOrAccessed.IsMatch(derivedName) ? "" : derivedName;
        }

        private const string NameMask = @"(?-i)\s*(?:sir)?\s*((?:[A-Z]+\.?){0,3}\s*[A-Z][\w-']{2,}[,\.]?\s*(?:\s+\w\.?|\b(?:[A-Z]+\.?){0,3})?(?:\s+[A-Z][\w-']{2,}){0,3}(?:\s+\w(?:\.?|\b)){0,2})\s*(?:[,\.'&;:\[\(“`]|et\s+al)(?i)[^{}<>\n]*?";
        private const string YearMask = @"(\([12]\d{3}\)|\b[12]\d{3}[,\.\)])";
        private const string PageMask = @"('*(?:p+g?|pages?)'*\.?'*(?:&nbsp;)?\s*(?:\d{1,3}|(?-i)[XVICM]+(?i))\.?(?:\s*[-/&\.,]\s*(?:\d{1,3}|(?-i)[XVICM]+(?i)))?\b)";

        private static readonly Regex CitationCiteBook = new Regex(@"{{[Cc]it[ae]((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}})", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateLastParameter = new Regex(@"(?<=\s*last\s*=\s*)([^{}\|<>]+?)(?=\s*(?:\||}}))", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateAuthorParameter = new Regex(@"(?<=\s*author(?:link)?\s*=\s*)([^{}\|<>]+?)(?=\s*(?:\||}}))", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateYearParameter = new Regex(@"(?<=\s*year\s*=\s*)([^{}\|<>]+?)(?=\s*(?:\||}}))", RegexOptions.Compiled);
        private static readonly Regex CiteTemplatePagesParameter = new Regex(@"(?<=\s*pages?\s*=\s*)([^{}\|<>]+?)(?=\s*(?:\||}}))", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateDateParameter = new Regex(@"(?<=\s*date\s*=\s*)(\d{4})(?=\s*(?:\||}}))", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateTitleParameter = new Regex(@"(?<=\s*title\s*=\s*)([^{}\|<>]+?)(?=\s*(?:\||}}))", RegexOptions.Compiled);
        private static readonly Regex CiteTemplatePublisherParameter = new Regex(@"(?<=\s*publisher\s*=\s*)([^{}\|<>]+?)(?=\s*(?:\||}}))", RegexOptions.Compiled);
        private static readonly Regex URLShortDescription = new Regex(@"\s*[^{}<>\n]*?\s*\[*(?:http://www\.|http://|www\.)[^\[\]<>""\s]+?\s+([^{}<>\[\]]{4,35}?)\s*(?:\]|<!--|\u230A\u230A\u230A\u230A)", RegexOptions.Compiled);
        private static readonly Regex URLDomain = new Regex(@"\s*\w*?[^{}<>]{0,4}?\s*(?:\[?|\{\{\s*cit[^{}<>]*\|\s*url\s*=\s*)\s*(?:http://www\.|http://|www\.)([^\[\]<>""\s\/:]+)", RegexOptions.Compiled);
        private static readonly Regex HarvnbTemplate = new Regex(@"\s*{{[Hh]arv(?:(?:col)?nb)?\s*\|\s*([^{}\|]+?)\s*\|\s*(\d{4})\s*(?:\|\s*(?:pp?\s*=\s*)?([^{}\|]+?)\s*)?}}\s*", RegexOptions.Compiled);
        private static readonly Regex WholeShortReference = new Regex(@"\s*([^<>{}]{4,35})\s*", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateURL = new Regex(@"\s*\{\{\s*cit[^{}<>]*\|\s*url\s*=\s*([^\/<>{}\|]{4,35})", RegexOptions.Compiled);
        private static readonly Regex NameYearPage = new Regex(NameMask + YearMask + @"[^{}<>\n]*?" + PageMask + @"\s*", RegexOptions.Compiled);
        private static readonly Regex NamePage = new Regex(NameMask + PageMask + @"\s*", RegexOptions.Compiled);
        private static readonly Regex NameYear = new Regex(NameMask + YearMask + @"\s*", RegexOptions.Compiled);
        /// <summary>
        /// Derives a name for a reference by searching for author names and dates, or website base URL etc.
        /// </summary>
        /// <param name="articleText">text of article, to check the derived name is not already used for some other reference</param>
        /// <param name="reference">the value of the reference a name is needed for</param>
        /// <returns>the derived reference name, or null if none could be determined</returns>
        public static string DeriveReferenceName(string articleText, string reference)
        {
            string derivedReferenceName = "";

            // try parameters from a citation: lastname, year and page
            string citationTemplate = CitationCiteBook.Match(reference).Value;

            if (citationTemplate.Length > 10)
            {
                string last = CiteTemplateLastParameter.Match(reference).Value.Trim();

                if (last.Length < 1)
                {
                    last = CiteTemplateAuthorParameter.Match(reference).Value.Trim();
                }

                if (last.Length > 1)
                {
                    derivedReferenceName = last;
                    string year = CiteTemplateYearParameter.Match(reference).Value.Trim();

                    string pages = CiteTemplatePagesParameter.Match(reference).Value.Trim();

                    if (year.Length > 3)
                        derivedReferenceName += " " + year;
                    else
                    {
                        string date = CiteTemplateDateParameter.Match(reference).Value.Trim();

                        if (date.Length > 3)
                            derivedReferenceName += " " + date;
                    }

                    if (pages.Length > 0)
                        derivedReferenceName += " " + pages;

                    derivedReferenceName = CleanDerivedReferenceName(derivedReferenceName);
                }
                // otherwise try title
                else
                {
                    string title = CiteTemplateTitleParameter.Match(reference).Value.Trim();

                    if (title.Length > 3 && title.Length < 35)
                        derivedReferenceName = title;
                    derivedReferenceName = CleanDerivedReferenceName(derivedReferenceName);

                    // try publisher
                    if (derivedReferenceName.Length < 4)
                    {
                        title = CiteTemplatePublisherParameter.Match(reference).Value.Trim();

                        if (title.Length > 3 && title.Length < 35)
                            derivedReferenceName = title;
                        derivedReferenceName = CleanDerivedReferenceName(derivedReferenceName);
                    }
                }
            }

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // try description of a simple external link
            derivedReferenceName = ExtractReferenceNameComponents(reference, URLShortDescription, 1);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // website URL first, allowing a name before link
            derivedReferenceName = ExtractReferenceNameComponents(reference, URLDomain, 1);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // Harvnb template {{Harvnb|Young|1852|p=50}}
            derivedReferenceName = ExtractReferenceNameComponents(reference, HarvnbTemplate, 3);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // now just try to use the whole reference if it's short (<35 characters)
            if (reference.Length < 35)
                derivedReferenceName = ExtractReferenceNameComponents(reference, WholeShortReference, 1);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            //now try title of a citation
            derivedReferenceName = ExtractReferenceNameComponents(reference, CiteTemplateURL, 1);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // name...year...page
            derivedReferenceName = ExtractReferenceNameComponents(reference, NameYearPage, 3);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // name...page
            derivedReferenceName = ExtractReferenceNameComponents(reference, NamePage, 2);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // name...year
            derivedReferenceName = ExtractReferenceNameComponents(reference, NameYear, 2);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // generic ReferenceA
            derivedReferenceName = @"ReferenceA";

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // generic ReferenceA
            derivedReferenceName = @"ReferenceB";

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // generic ReferenceA
            derivedReferenceName = @"ReferenceC";

            return ReferenceNameValid(articleText, derivedReferenceName) ? derivedReferenceName : "";
        }

        private static bool ReferenceNameValid(string articleText, string derivedReferenceName)
        {
            return !Regex.IsMatch(articleText, RefName + Regex.Escape(derivedReferenceName) + @"""\s*/?\s*>") && derivedReferenceName.Length >= 3;
        }

        /// <summary>
        /// Corrects common formatting errors in dates in external reference citation templates (doesn't link/delink dates)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="noChange"></param>
        /// <returns>The modified article text.</returns>
        public string CiteTemplateDates(string articleText, out bool noChange)
        {
            string newText = CiteTemplateDates(articleText);

            noChange = (newText == articleText);

            return newText;
        }

        private const string SiCitStart = @"(?si)(\{\{\s*cit[^{}]*\|\s*";
        private const string CitAccessdate = SiCitStart + @"(?:access|archive)date\s*=\s*";
        private const string CitDate = SiCitStart + @"(?:archive|air)?date2?\s*=\s*";
        private const string CitYMonthD = SiCitStart + @"(?:archive|air|access)?date2?\s*=\s*\d{4})[-/\s]";
        private const string dTemEnd = @"?[-/\s]([0-3]?\d\s*(?:\||}}))";

        struct RegexReplacement
        {
            public RegexReplacement(Regex regex, string replacement) { Regex = regex; Replacement = replacement; }
            public readonly Regex Regex;
            public readonly string Replacement;
            // This could get extended to be a class with some of the Regex methods, such as IsMatch, Replace, etc, but there's little point
        }

        private static readonly Regex AccessOrArchiveDate = new Regex(@"\b(access|archive)date\s*=", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly RegexReplacement[] CiteTemplateIncorrectISOAccessdates = new[] {
            new RegexReplacement(new Regex(CitAccessdate + @")(1[0-2])[/_\-\.]?(1[3-9])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$4-$2-$3"),
            new RegexReplacement(new Regex(CitAccessdate + @")(1[0-2])[/_\-\.]?([2-3]\d)[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$4-$2-$3"),
            new RegexReplacement(new Regex(CitAccessdate + @")(1[0-2])[/_\-\.]?\2[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$3-$2-$2"), // nn-nn-2004 and nn-nn-04 to ISO format (both nn the same)
            new RegexReplacement(new Regex(CitAccessdate + @")(1[3-9])[/_\-\.]?(1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$4-$3-$2"),
            new RegexReplacement(new Regex(CitAccessdate + @")(1[3-9])[/_\-\.]?0?([1-9])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$4-0$3-$2"),
            new RegexReplacement(new Regex(CitAccessdate + @")(20[01]\d)0?([01]\d)[/_\-\.]([0-3]\d\s*(?:\||}}))", RegexOptions.Compiled), "$1$2-$3-$4"),
            new RegexReplacement(new Regex(CitAccessdate + @")(20[01]\d)[/_\-\.]([01]\d)0?([0-3]\d\s*(?:\||}}))", RegexOptions.Compiled), "$1$2-$3-$4"),
            new RegexReplacement(new Regex(CitAccessdate + @")(20[01]\d)[/_\-\.]?([01]\d)[/_\-\.]?([1-9]\s*(?:\||}}))", RegexOptions.Compiled), "$1$2-$3-0$4"),
            new RegexReplacement(new Regex(CitAccessdate + @")(20[01]\d)[/_\-\.]?([1-9])[/_\-\.]?([0-3]\d\s*(?:\||}}))", RegexOptions.Compiled), "$1$2-0$3-$4"),
            new RegexReplacement(new Regex(CitAccessdate + @")(20[01]\d)[/_\-\.]?([1-9])[/_\-\.]0?([1-9]\s*(?:\||}}))", RegexOptions.Compiled), "$1$2-0$3-0$4"),
            new RegexReplacement(new Regex(CitAccessdate + @")(20[01]\d)[/_\-\.]0?([1-9])[/_\-\.]([1-9]\s*(?:\||}}))", RegexOptions.Compiled), "$1$2-0$3-0$4"),
            new RegexReplacement(new Regex(CitAccessdate + @")(20[01]\d)[/_\.]?([01]\d)[/_\.]?([0-3]\d\s*(?:\||}}))", RegexOptions.Compiled), "$1$2-$3-$4"),

            new RegexReplacement(new Regex(CitAccessdate + @")([2-3]\d)[/_\-\.]?(1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$4-$3-$2"),
            new RegexReplacement(new Regex(CitAccessdate + @")([2-3]\d)[/_\-\.]0?([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$4-0$3-$2"),
            new RegexReplacement(new Regex(CitAccessdate + @")0?([1-9])[/_\-\.]?(1[3-9]|[2-3]\d)[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$4-0$2-$3"),
            new RegexReplacement(new Regex(CitAccessdate + @")0?([1-9])[/_\-\.]?0?\2[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$3-0$2-0$2"), // n-n-2004 and n-n-04 to ISO format (both n the same)
        };

        private static readonly Regex CiteTemplateArchiveAirDate = new Regex(@"{{\s*cit[^{}]*\|\s*(?:archive|air)?date2?\s*=", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly RegexReplacement[] CiteTemplateIncorrectISODates = new[] {
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)(200\d|19[7-9]\d)[/_]?([0-1]\d)[/_]?([0-3]\d\s*(?:\||}}))", RegexOptions.Compiled), "$1$2-$3-$4"),
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)(1[0-2])[/_\-\.]?([2-3]\d)[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "$1$4-$2-$3"),
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)0?([1-9])[/_\-\.]?([2-3]\d)[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "$1$4-0$2-$3"),
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)([2-3]\d)[/_\-\.]?0?([1-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "$1$4-0$3-$2"),
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)([2-3]\d)[/_\-\.]?(1[0-2])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "$1$4-$3-$2"),
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)(1[0-2])[/_\-\.]([2-3]\d)[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$4-$2-$3"),
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)0?([1-9])[/_\-\.]([2-3]\d)[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$4-0$2-$3"),
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)([2-3]\d)[/_\-\.]0?([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$4-0$3-$2"),
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)([2-3]\d)[/_\-\.]?(1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$4-$3-$2"),
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)(1[0-2])[/_\-\.]?(1[3-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "$1$4-$2-$3"),
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)0?([1-9])[/_\-\.](1[3-9])[/_\-\.](19[7-9]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "$1$4-0$2-$3"),
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)(1[3-9])[/_\-\.]?0?([1-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "$1$4-0$3-$2"),
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)(1[3-9])[/_\-\.]?(1[0-2])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "$1$4-$3-$2"),
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)(1[0-2])[/_\-\.]?(1[3-9])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$4-$2-$3"),
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)([1-9])[/_\-\.](1[3-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$4-0$2-$3"),
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)(1[3-9])[/_\-\.]?([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$4-0$3-$2"),
            new RegexReplacement(new Regex(CitDate + @"\[?\[?)(1[3-9])[/_\-\.](1[0-2])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$4-$3-$2"),
            new RegexReplacement(new Regex(CitDate + @")0?([1-9])[/_\-\.]0?\2[/_\-\.](200\d|19[7-9]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "$1$3-0$2-0$2"), // n-n-2004 and n-n-1980 to ISO format (both n the same)
            new RegexReplacement(new Regex(CitDate + @")0?([1-9])[/_\-\.]0?\2[/_\-\.]([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$3-0$2-0$2"), // n-n-04 to ISO format (both n the same)
            new RegexReplacement(new Regex(CitDate + @")(1[0-2])[/_\-\.]?\2[/_\-\.]?(200\d|19[7-9]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "$1$3-$2-$2"), // nn-nn-2004 and nn-nn-1980 to ISO format (both nn the same)
            new RegexReplacement(new Regex(CitDate + @")(1[0-2])[/_\-\.]?\2[/_\-\.]?([01]\d)(?=\s*(?:\||}}))", RegexOptions.Compiled), "${1}20$3-$2-$2"), // nn-nn-04 to ISO format (both nn the same)
            new RegexReplacement(new Regex(CitDate + @")((?:\[\[)?200\d|19[7-9]\d)[/_\-\.]([1-9])[/_\-\.]0?([1-9](?:\]\])?\s*(?:\||}}))", RegexOptions.Compiled), "$1$2-0$3-0$4"),
            new RegexReplacement(new Regex(CitDate + @")((?:\[\[)?200\d|19[7-9]\d)[/_\-\.]0?([1-9])[/_\-\.]([1-9](?:\]\])?\s*(?:\||}}))", RegexOptions.Compiled), "$1$2-0$3-0$4"),
            new RegexReplacement(new Regex(CitDate + @")((?:\[\[)?200\d|19[7-9]\d)[/_\-\.]?([0-1]\d)[/_\-\.]?([1-9](?:\]\])?\s*(?:\||}}))", RegexOptions.Compiled), "$1$2-$3-0$4"),
            new RegexReplacement(new Regex(CitDate + @")((?:\[\[)?200\d|19[7-9]\d)[/_\-\.]?([1-9])[/_\-\.]?([0-3]\d(?:\]\])?\s*(?:\||}}))", RegexOptions.Compiled), "$1$2-0$3-$4"),
            new RegexReplacement(new Regex(CitDate + @")((?:\[\[)?200\d|19[7-9]\d)([0-1]\d)[/_\-\.]([0-3]\d(?:\]\])?\s*(?:\||}}))", RegexOptions.Compiled), "$1$2-$3-$4"),
            new RegexReplacement(new Regex(CitDate + @")((?:\[\[)?200\d|19[7-9]\d)[/_\-\.]([0-1]\d)0?([0-3]\d(?:\]\])?\s*(?:\||}}))", RegexOptions.Compiled), "$1$2-$3-$4"),
        };

        private static readonly RegexReplacement[] CiteTemplateAbbreviatedMonths = new[] {
            new RegexReplacement(new Regex(CitYMonthD + @"Jan(?:uary|\.)" + dTemEnd, RegexOptions.Compiled), "$1-01-$2"),
            new RegexReplacement(new Regex(CitYMonthD + @"Feb(?:r?uary|\.)" + dTemEnd, RegexOptions.Compiled), "$1-02-$2"),
            new RegexReplacement(new Regex(CitYMonthD + @"Mar(?:ch|\.)" + dTemEnd, RegexOptions.Compiled), "$1-03-$2"),
            new RegexReplacement(new Regex(CitYMonthD + @"Apr(?:il|\.)" + dTemEnd, RegexOptions.Compiled), "$1-04-$2"),
            new RegexReplacement(new Regex(CitYMonthD + @"May\." + dTemEnd, RegexOptions.Compiled), "$1-05-$2"),
            new RegexReplacement(new Regex(CitYMonthD + @"Jun(?:e|\.)" + dTemEnd, RegexOptions.Compiled), "$1-06-$2"),
            new RegexReplacement(new Regex(CitYMonthD + @"Jul(?:y|\.)" + dTemEnd, RegexOptions.Compiled), "$1-07-$2"),
            new RegexReplacement(new Regex(CitYMonthD + @"Aug(?:ust|\.)" + dTemEnd, RegexOptions.Compiled), "$1-08-$2"),
            new RegexReplacement(new Regex(CitYMonthD + @"Sep(?:tember|\.)" + dTemEnd, RegexOptions.Compiled), "$1-09-$2"),
            new RegexReplacement(new Regex(CitYMonthD + @"Oct(?:ober|\.)" + dTemEnd, RegexOptions.Compiled), "$1-10-$2"),
            new RegexReplacement(new Regex(CitYMonthD + @"Nov(?:ember|\.)" + dTemEnd, RegexOptions.Compiled), "$1-11-$2"),
            new RegexReplacement(new Regex(CitYMonthD + @"Dec(?:ember|\.)" + dTemEnd, RegexOptions.Compiled), "$1-12-$2"),
        };

        private static readonly Regex CiteTemplateDateYYYYDDMMFormat = new Regex(SiCitStart + @"(?:archive|air|access)?date2?\s*=\s*(?:\[\[)?200\d)-([2-3]\d|1[3-9])-(0[1-9]|1[0-2])(\]\])?", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateTimeInDateParameter = new Regex(@"(\{\{\s*cite[^\{\}]*\|\s*(?:archive|air|access)?date2?\s*=\s*(?:(?:200\d|19[7-9]\d)-[01]?\d-[0-3]?\d|[0-3]?\d\s*\w+,?\s*(?:200\d|19[7-9]\d)|\w+\s*[0-3]?\d,?\s*(?:200\d|19[7-9]\d)))(\s*[,-:]?\s+[0-2]?\d\:?[0-5]\d(?:\:?[0-5]\d)?\s*[^\|\}]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Corrects common formatting errors in dates in external reference citation templates (doesn't link/delink dates)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string CiteTemplateDates(string articleText)
        {
            // cite podcast is non-compliant to citation core standards
            if (Regex.IsMatch(articleText, @"({{\s*[Cc]ite podcast\s*\|)"))
                return articleText;

            // note some incorrect date formats such as 3-2-2009 are ambiguous as could be 3-FEB-2009 or MAR-2-2009
            // these fixes don't address such errors
            string articleTextlocal = "";

            // loop in case a single citation has multiple dates to be fixed
            while (articleTextlocal != articleText)
            {
                articleTextlocal = articleText;

                // convert invalid date formats like DD-MM-YYYY, MM-DD-YYYY, YYYY-D-M, YYYY-DD-MM, YYYY_MM_DD etc. to iso format of YYYY-MM-DD
                // for accessdate= and archivedate=
                if (AccessOrArchiveDate.IsMatch(articleText))
                    foreach (RegexReplacement rr in CiteTemplateIncorrectISOAccessdates)
                        articleText = rr.Regex.Replace(articleText, rr.Replacement);

                // date=, archivedate=, airdate=, date2=
                if (CiteTemplateArchiveAirDate.IsMatch(articleText))
                {
                    foreach (RegexReplacement rr in CiteTemplateIncorrectISODates)
                        articleText = rr.Regex.Replace(articleText, rr.Replacement);

                    // date = YYYY-Month-DD fix, on en-wiki only
                    if (Variables.LangCode == "en")
                        foreach (RegexReplacement rr in CiteTemplateAbbreviatedMonths)
                            articleText = rr.Regex.Replace(articleText, rr.Replacement);
                }

                // all citation dates
                articleText = CiteTemplateDateYYYYDDMMFormat.Replace(articleText, "$1-$3-$2$4"); // YYYY-DD-MM to YYYY-MM-DD
                articleText = CiteTemplateTimeInDateParameter.Replace(articleText, "$1<!--$2-->"); // Removes time from date fields
            }

            return articleText;
        }

        // Covered by: FormattingTests.TestMdashes()
        /// <summary>
        /// Replaces hyphens and em-dashes with en-dashes, per [[WP:DASH]]
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article's title</param>
        /// <returns>The modified article text.</returns>
        [Obsolete("cannot provide correct namespace key")]
        public string Mdashes(string articleText, string articleTitle)
        {
            // use dummy namespace of -1
            return Mdashes(articleText, articleTitle, -1);
        }

        private static readonly Regex PageRangeIncorrectMdash = new Regex(@"(pages\s*=\s*|pp\.?\s*)(\d+\s*)(?:-|—|&mdash;|&#8212;)(\s*\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex TimeRangeIncorrectMdash = new Regex(@"([1-9]?\d\s*)(?:-|—|&mdash;|&#8212;)(\s*[1-9]?\d)(\s+|&nbsp;)(years|months|weeks|days|hours|minutes|seconds|[km]g|kb|[ckm]?m|[Gk]?Hz|miles|mi\.|%|feet|foot|ft)\b", RegexOptions.Compiled);
        private static readonly Regex DollarAmountIncorrectMdash = new Regex(@"(\$[1-9]?\d{1,3}\s*)(?:-|—|&mdash;|&#8212;)(\s*\$?[1-9]?\d{1,3})", RegexOptions.Compiled);
        private static readonly Regex AMPMIncorrectMdash = new Regex(@"([01]?\d:[0-5]\d\s*([AP]M)\s*)(?:-|—|&mdash;|&#8212;)(\s*[01]?\d:[0-5]\d\s*([AP]M))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex AgeIncorrectMdash = new Regex(@"([Aa]ge[sd])\s([1-9]?\d\s*)(?:-|—|&mdash;|&#8212;)(\s*[1-9]?\d)", RegexOptions.Compiled);
        private static readonly Regex SentenceClauseIncorrectMdash = new Regex(@"(?<=\w)\s*--\s*(?=\w)", RegexOptions.Compiled);
        private static readonly Regex SuperscriptMinus = new Regex(@"(?<=<sup>)(?:-|–|—)(?=\d+</sup>)", RegexOptions.Compiled);

        // Covered by: FormattingTests.TestMdashes()
        /// <summary>
        /// Replaces hyphens and em-dashes with en-dashes, per [[WP:DASH]]
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article's title</param>
        /// <param name="nameSpaceKey">The namespace of the article</param>
        /// <returns>The modified article text.</returns>
        public string Mdashes(string articleText, string articleTitle, int nameSpaceKey)
        {
            articleText = HideMoreText(articleText);
            articleText = PageRangeIncorrectMdash.Replace(articleText, @"$1$2–$3");
            articleText = TimeRangeIncorrectMdash.Replace(articleText, @"$1–$2$3$4");
            articleText = DollarAmountIncorrectMdash.Replace(articleText, @"$1–$2");
            articleText = AMPMIncorrectMdash.Replace(articleText, @"$1–$3");
            articleText = AgeIncorrectMdash.Replace(articleText, @"$1 $2–$3");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Match_en_dashes.2Femdashs_from_titles_with_those_in_the_text
            // if title has en or em dashes, apply them to strings matching article title but with hyphens
            if (articleTitle.Contains(@"–") || articleTitle.Contains(@"—"))
                articleText = Regex.Replace(articleText, Regex.Escape(articleTitle.Replace(@"–", @"-").Replace(@"—", @"-")), articleTitle);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Change_--_.28two_dashes.29_to_.E2.80.94_.28em_dash.29
            if (nameSpaceKey == Namespace.Mainspace)
                articleText = SentenceClauseIncorrectMdash.Replace(articleText, @"—");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#minuses
            // replace hyphen or en-dash or emdash with Unicode minus (&minus;)
            // [[Wikipedia:MOSNUM#Common_mathematical_symbols]]
            articleText = SuperscriptMinus.Replace(articleText, "−");

            return AddBackMoreText(articleText);
        }

        private static readonly Regex RME_ColumnCount = new Regex(@"[^-]column-count:[\s]*?(\d*)", RegexOptions.Compiled);
        private static readonly Regex RME_MozColumnCount = new Regex(@"-moz-column-count:[\s]*?(\d*)", RegexOptions.Compiled);
        // Covered by: FootnotesTests.TestFixReferenceListTags()
        private static string ReflistMatchEvaluator(Match m)
        {
            // don't change anything if div tags mismatch
            if (DivStart.Matches(m.Value).Count != DivEnd.Matches(m.Value).Count)
                return m.Value;

            if (m.Value.Contains("references-2column"))
                return "{{reflist|2}}";

            string s = RME_ColumnCount.Match(m.Value).Groups[1].Value;
            if (s.Length > 0)
                return "{{reflist|" + s + "}}";

            s = RME_MozColumnCount.Match(m.Value).Groups[1].Value;
            if (s.Length > 0)
                return "{{reflist|" + s + "}}";

            return "{{reflist}}";
        }

        /// <summary>
        /// Main regex for {{reflist}} converter
        /// </summary>
        private static readonly Regex ReferenceListTags = new Regex(@"(<(span|div)( class=""(references-small|small|references-2column)|)?"">[\r\n\s]*){1,2}[\r\n\s]*<references[\s]?/>([\r\n\s]*</(span|div)>){1,2}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

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
            return ReferenceListTags.Replace(articleText, new MatchEvaluator(ReflistMatchEvaluator));
        }

        private static readonly Regex EmptyReferences = new Regex(@"(<ref\s+name\s*=\s*(?:""|')?[^<>=\r\n]+?(?:""|')?)\s*>\s*<\s*/\s*ref\s*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Covered by: FootnotesTests.TestSimplifyReferenceTags()
        /// <summary>
        /// Replaces reference tags in the form <ref name="blah"></ref> with <ref name="blah" />
        /// Removes some of the MW errors that occur from the prior
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns></returns>
        public static string SimplifyReferenceTags(string articleText)
        {
            return EmptyReferences.Replace(articleText, @"$1 />");
        }

        private static readonly Regex LinksHeading = new Regex(@"(?sim)(==+\s*)Links(\s*==+\s*(?:^(?:\*|\d\.?)?\s*\[?\s*http://))", RegexOptions.Compiled);
        private static readonly Regex ReferencesHeadingLevel2 = new Regex(@"(?i)==\s*'*\s*References?\s*'*\s*==", RegexOptions.Compiled);
        private static readonly Regex ReferencesHeadingLevelLower = new Regex(@"(?i)(==+\s*'*\s*References?\s*'*\s*==+)", RegexOptions.Compiled);
        private static readonly Regex ExternalLinksHeading = new Regex(@"(?im)(^\s*=+\s*(?:External\s+link|Source|Web\s*link)s?\s*=)", RegexOptions.Compiled);
        private static readonly Regex ExternalLinksToReferences = new Regex(@"(?sim)(^\s*=+\s*(?:External\s+link|Source|Web\s*link)s?\s*=+.*?)(\r\n==+References==+\r\n{{Reflist}}<!--added above External links/Sources by script-assisted edit-->)", RegexOptions.Compiled);
        private static readonly Regex Category = new Regex(@"(?im)(^\s*\[\[\s*Category\s*:)", RegexOptions.Compiled);
        private static readonly Regex CategoryToReferences = new Regex(@"(?sim)((?:^\{\{[^{}]+?\}\}\s*)*)(^\s*\[\[\s*Category\s*:.*?)(\r\n==+References==+\r\n{{Reflist}}<!--added above categories/infobox footers by script-assisted edit-->)", RegexOptions.Compiled);
        //private static readonly Regex AMR8 = new Regex(@"(?sim)(^==.*?)(^\{\{[^{}]+?\}\}.*?)(\r\n==+References==+\r\n{{Reflist}}<!--added to end of article by script-assisted edit-->)", RegexOptions.Compiled);
        private static readonly Regex ReflistByScript = new Regex(@"(\{\{Reflist\}\})<!--added[^<>]+by script-assisted edit-->", RegexOptions.Compiled);

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
            articleText = LinksHeading.Replace(articleText, "$1External links$2");

            if (ReferencesHeadingLevel2.IsMatch(articleText))
                articleText = ReferencesHeadingLevelLower.Replace(articleText, "$1\r\n{{Reflist}}<!--added under references heading by script-assisted edit-->");
            else
            {
                //now try to move just above external links
                if (ExternalLinksHeading.IsMatch(articleText))
                {
                    articleText += "\r\n==References==\r\n{{Reflist}}<!--added above External links/Sources by script-assisted edit-->";
                    articleText = ExternalLinksToReferences.Replace(articleText, "$2\r\n$1");
                }
                else
                { // now try to move just above categories
                    if (Category.IsMatch(articleText))
                    {
                        articleText += "\r\n==References==\r\n{{Reflist}}<!--added above categories/infobox footers by script-assisted edit-->";
                        articleText = CategoryToReferences.Replace(articleText, "$3\r\n$1$2");
                    }
                    //else
                    //{
                    // TODO: relist is missing, but not sure where references should go – at end of article might not be correct
                    //articleText += "\r\n==References==\r\n{{Reflist}}<!--added to end of article by script-assisted edit-->";
                    //articleText = AMR8.Replace(articleText, "$1\r\n$3\r\n$2");
                    //}
                }
            }
            // remove reflist comment
            return ReflistByScript.Replace(articleText, "$1");
        }

        private static readonly RegexReplacement[] RefWhitespace = new[] {
        // whitespace cleaning
            new RegexReplacement(new Regex(@"<\s*(?:\s+ref\s*|\s*ref\s+)>", RegexOptions.Compiled | RegexOptions.Singleline), "<ref>"),
            new RegexReplacement(new Regex(@"<(?:\s*/(?:\s+ref\s*|\s*ref\s+)|\s+/\s*ref\s*)>", RegexOptions.Compiled | RegexOptions.Singleline), "</ref>"),
        
        // remove any spaces between consecutive references -- WP:REFPUNC
            new RegexReplacement(new Regex(@"(</ref>|<ref\s*name\s*=[^{}<>]+?\s*\/\s*>) +(?=<ref(?:\s*name\s*=[^{}<>]+?\s*\/?\s*)?>)", RegexOptions.Compiled), "$1"),
        // ensure a space between a reference and text (reference within a paragraph) -- WP:REFPUNC
            new RegexReplacement(new Regex(@"(</ref>|<ref\s*name\s*=[^{}<>]+?\s*\/\s*>)(\w)", RegexOptions.Compiled), "$1 $2"),
        // remove spaces between punctuation and references -- WP:REFPUNC
            new RegexReplacement(new Regex(@"([,\.:;]) +(?=<ref(?:\s*name\s*=[^{}<>]+?\s*\/?\s*)?>)", RegexOptions.Compiled), "$1"),

        // <ref name="Fred" /ref> --> <ref name="Fred"/>
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*""[^<>=""\/]+?"")\s*/\s*ref\s*>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), "$1/>"),

        // <ref name="Fred""> --> <ref name="Fred">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*""[^<>=""\/]+?"")["">]\s*(/?)>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), "$1$2>"),

        // <ref name = ”Fred”> --> <ref name="Fred">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)[“‘”’]*([^<>=""\/]+?)[“‘”’]+(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""$2""$3"),
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)[“‘”’]+([^<>=""\/]+?)[“‘”’]*(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""$2""$3"),

        // <ref name = ''Fred'> --> <ref name="Fred"> (two apostrophes)
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)''+([^<>=""\/]+?)'+(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""$2""$3"),

        // <ref name = 'Fred''> --> <ref name="Fred"> (two apostrophes)
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)'+([^<>=""\/]+?)''+(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""$2""$3"),

        // <ref name=foo bar> --> <ref name="foo bar">, match when spaces
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)([^<>=""'\/]+?\s+[^<>=""'\/]+?)(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""$2""$3"),

        // <ref name=foo bar> --> <ref name="foo bar">, match when non-ASCII characters ([\x00-\xff]*)
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)([^<>=""'\/]*?[^\x00-\xff]+?[^<>=""'\/]*?)(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""$2""$3"),

        // <ref name=foo bar"> --> <ref name="foo bar">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)['`”]*([^<>=""\/]+?)""(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""$2""$3"),

        // <ref name="foo bar> --> <ref name="foo bar">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)""([^<>=""\/]+?)['`”]*(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""$2""$3"),

        // <ref name "foo bar"> --> <ref name="foo bar">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*)[\+\-]?(\s*""[^<>=""\/]+?""\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1=$2"),

        // <ref "foo bar"> --> <ref name="foo bar">
            new RegexReplacement(new Regex(@"(<\s*ref\s+)(""[^<>=""\/]+?""\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), "$1name=$2"),

        // ref name typos
            new RegexReplacement(new Regex(@"(<\s*ref\s+n)(me\s*=)", RegexOptions.Compiled | RegexOptions.IgnoreCase), "$1a$2"),

        // <ref>...<ref/> --> <ref>...</ref>
            new RegexReplacement(new Regex(@"(<\s*ref(?:\s+name\s*=[^<>]*?)?\s*>[^<>""]+?)<\s*ref\s*/\s*>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), "$1</ref>"),

        // <ref>...</red> --> <ref>...</ref>
            new RegexReplacement(new Regex(@"(<\s*ref(?:\s+name\s*=[^<>]*?)?\s*>[^<>""]+?)<\s*/\s*red\s*>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), "$1</ref>"),

        // <REF> and <Ref> to <ref>
            new RegexReplacement(new Regex(@"(<\s*\/?\s*)(?:R[Ee][Ff]|r[Ee]F)(\s*(?:>|name\s*=))"), "$1ref$2"),

            // trailing spaces within ref
            new RegexReplacement(new Regex(@" +</ref>"), "</ref>"),
        };
        // Covered by TestFixReferenceTags
        /// <summary>
        /// Various fixes to the formatting of &lt;ref&gt; reference tags
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns>The modified article text.</returns>
        public static string FixReferenceTags(string articleText)
        {
            foreach (RegexReplacement rr in RefWhitespace)
                articleText = rr.Regex.Replace(articleText, rr.Replacement);

            return articleText;
        }

        // don't match on 'in the June of 2007', 'on the 11th May 2008' etc. as these won't read well if changed
        private static readonly Regex OfBetweenMonthAndYear = new Regex(@"\b" + WikiRegexes.Months + @"\s+of\s+(200\d|1[89]\d\d)\b(?<!\b[Tt]he\s{1,5}\w{3,15}\s{1,5}of\s{1,5}(200\d|1[89]\d\d))", RegexOptions.Compiled);
        private static readonly Regex OrdinalsInDatesAm = new Regex(@"(?<!\b[1-3]\d +)\b" + WikiRegexes.Months + @"\s+([0-3]?\d)(?:st|nd|rd|th)\b(?<!\b[Tt]he\s+\w{3,10}\s+(?:[0-3]?\d)(?:st|nd|rd|th)\b)(?:(\s*(?:to|and|.|&.dash;)\s*[0-3]?\d)(?:st|nd|rd|th)\b)?", RegexOptions.Compiled);
        private static readonly Regex OrdinalsInDatesInt = new Regex(@"(?:\b([0-3]?\d)(?:st|nd|rd|th)(\s*(?:to|and|.|&.dash;)\s*))?\b([0-3]?\d)(?:st|nd|rd|th)\s+" + WikiRegexes.Months + @"\b(?<!\b[Tt]he\s+(?:[0-3]?\d)(?:st|nd|rd|th)\s+\w{3,10})", RegexOptions.Compiled);
        private static readonly Regex DateLeadingZerosAm = new Regex(@"\b" + WikiRegexes.Months + @"\s+0([1-9])" + @"\b", RegexOptions.Compiled);
        private static readonly Regex DateLeadingZerosInt = new Regex(@"\b" + @"0([1-9])\s+" + WikiRegexes.Months + @"\b", RegexOptions.Compiled);
        private static readonly Regex MonthsRegex = new Regex(@"\b" + WikiRegexes.MonthsNoGroup + @"\b", RegexOptions.Compiled);
        // Covered by TestFixDateOrdinalsAndOf
        /// <summary>
        /// Removes ordinals, leading zeros from dates and 'of' between a month and a year, per [[WP:MOSDATE]]; on en wiki only
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <param name="articleTitle">The article's title</param>
        /// <returns>The modified article text.</returns>
        public string FixDateOrdinalsAndOf(string articleText, string articleTitle)
        {
            if (Variables.LangCode != "en")
                return articleText;

            // hide items in quotes etc., though this may also hide items within infoboxes etc.
            articleText = HideMoreText(articleText);

            articleText = OfBetweenMonthAndYear.Replace(articleText, "$1 $2");

            // don't apply if article title has a month in it (e.g. [[6th of October City]])
            if (!MonthsRegex.IsMatch(articleTitle))
            {
                articleText = OrdinalsInDatesAm.Replace(articleText, "$1 $2$3");
                articleText = OrdinalsInDatesInt.Replace(articleText, "$1$2$3 $4");
            }

            articleText = DateLeadingZerosAm.Replace(articleText, "$1 $2");
            articleText = DateLeadingZerosInt.Replace(articleText, "$1 $2");

            return AddBackMoreText(articleText);
        }

        private static readonly Regex BrTwoNewlines = new Regex("<br ?/?>\r\n\r\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex ThreeOrMoreNewlines = new Regex("\r\n(\r\n)+", RegexOptions.Compiled);
        private static readonly Regex TwoNewlinesInBlankSection = new Regex("== ? ?\r\n\r\n==", RegexOptions.Compiled);
        private static readonly Regex NewlinesBelowExternalLinks = new Regex(@"==External links==[\r\n\s]*\*", RegexOptions.Compiled);
        private static readonly Regex NewlinesBeforeURL = new Regex(@"\r\n\r\n(\* ?\[?http)", RegexOptions.Compiled);
        private static readonly Regex HorizontalRule = new Regex("----+$", RegexOptions.Compiled);
        private static readonly Regex MultipleTabs = new Regex("  +", RegexOptions.Compiled);
        private static readonly Regex SpaceThenNewline = new Regex(" \r\n", RegexOptions.Compiled);
        private static readonly Regex WikiListWithMultipleSpaces = new Regex(@"^([\*#]+) +", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex WikiList = new Regex(@"^([\*#]+)", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SpacedHeadings = new Regex("^(={1,4}) ?(.*?) ?(={1,4})$", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SpacedDashes = new Regex(" (–|—|&#15[01];|&[nm]dash;|&#821[12];|&#x201[34];) ", RegexOptions.Compiled);

        // Covered by: FormattingTests.TestFixWhitespace(), incomplete
        /// <summary>
        /// Applies/removes some excess whitespace from the article
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string RemoveWhiteSpace(string articleText)
        {
            //Remove <br /> if followed by double newline
            articleText = BrTwoNewlines.Replace(articleText.Trim(), "\r\n\r\n");

            articleText = ThreeOrMoreNewlines.Replace(articleText, "\r\n\r\n");

            articleText = TwoNewlinesInBlankSection.Replace(articleText, "==\r\n==");
            articleText = NewlinesBelowExternalLinks.Replace(articleText, "==External links==\r\n*");
            articleText = NewlinesBeforeURL.Replace(articleText, "\r\n$1");

            articleText = HorizontalRule.Replace(articleText.Trim(), "");

            if (articleText.Contains("\r\n|\r\n\r\n"))
                articleText = articleText.Replace("\r\n|\r\n\r\n", "\r\n|\r\n");
            if (articleText.Contains("\r\n\r\n|"))
                articleText = articleText.Replace("\r\n\r\n|", "\r\n|");

            return articleText.Trim();
        }

        // covered by RemoveAllWhiteSpaceTests
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

            articleText = MultipleTabs.Replace(articleText, " ");
            articleText = SpaceThenNewline.Replace(articleText, "\r\n");

            articleText = articleText.Replace("==\r\n\r\n", "==\r\n");
            articleText = NewlinesBelowExternalLinks.Replace(articleText, "==External links==\r\n*");

            //fix bullet points – one space after them
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_arbitrary_spaces_after_bullet
            articleText = WikiListWithMultipleSpaces.Replace(articleText, "$1");
            articleText = WikiList.Replace(articleText, "$1 ");

            //fix heading space
            articleText = SpacedHeadings.Replace(articleText, "$1$2$3");

            //fix dash spacing
            articleText = SpacedDashes.Replace(articleText, "$1");

            return articleText.Trim();
        }

        /// <summary>
        /// Fixes and improves syntax (such as html markup)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public static string FixSyntax(string articleText, out bool noChange)
        {
            string newText = FixSyntax(articleText);

            noChange = (newText == articleText);
            return newText;
        }

        // regexes for external link match on balanced bracket
        private static readonly Regex DoubleBracketAtStartOfExternalLink = new Regex(@"\[(\[https?:/(?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex DoubleBracketAtEndOfExternalLink = new Regex(@"(\[https?:/(?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\])\](?!\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex DoubleBracketAtEndOfExternalLinkWithinImage = new Regex(@"(\[https?:/(?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!)))\](?=\]{3})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private const string TemEnd = @"(\s*(?:\||\}\}))";
        private const string CitURL = @"(\{\{\s*cit[ae][^{}]*?\|\s*url\s*=\s*)";
        private static readonly Regex BracketsAtBeginCiteTemplateURL = new Regex(CitURL + @"\[+\s*((?:(?:ht|f)tp://)?[^\[\]<>""\s]+?\s*)\]?" + TemEnd, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex BracketsAtEndCiteTemplateURL = new Regex(CitURL + @"\[?\s*((?:(?:ht|f)tp://)?[^\[\]<>""\s]+?\s*)\]+" + TemEnd, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex SyntaxRegex4 = new Regex(@"\[\[([^][]*?)\](?=[^\]]*?(?:$|\[|\n))", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex5 = new Regex(@"(?<=(?:^|\]|\n)[^\[]*?)\[([^][]*?)\]\](?!\])", RegexOptions.Compiled);

        private static readonly Regex SyntaxRegex6 = new Regex("\\[?\\[image:(http:\\/\\/.*?)\\]\\]?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex7 = new Regex("\\[\\[ (.*)?\\]\\]", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex8 = new Regex("\\[\\[([A-Za-z]*) \\]\\]", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex9 = new Regex("\\[\\[(.*)?_#(.*)\\]\\]", RegexOptions.Compiled);

        private static readonly Regex SyntaxRegexTemplate = new Regex("(\\{\\{[\\s]*)[Tt]emplate:(.*?\\}\\})", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex11 = new Regex(@"^([#\*:;]+.*?) *<[/\\]?br ?[/\\]?> *\r\n", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // make double spaces within wikilinks just single spaces
        private static readonly Regex SyntaxRegex12 = new Regex(@"(\[\[[^\[\]]+?) {2,}([^\[\]]+\]\])", RegexOptions.Compiled);

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

        private static readonly Regex AccessdateTypo = new Regex(@"(\{\{\s*cit[^{}]*?\|\s*)ac(?:(?:ess?s?|cc?es|cess[es]|ccess)date|cessdare)(\s*=\s*)", RegexOptions.IgnoreCase);

        private static readonly Regex AccessdateSynonyms = new Regex(@"(?<={{\s*[Cc]it[ae][^{}]*?\|\s*)(?:\s*date\s*)?(?:retrieved(?:\s+on)?|(?:last|date) *accessed|access\s+date)(?=\s*=\s*)", RegexOptions.Compiled);

        private static readonly Regex UppercaseCiteFields = new Regex(@"(\{\{(?:[Cc]ite\s*(?:web|book|news|journal|paper|press release|hansard|encyclopedia)|[Cc]itation)\b\s*[^{}]*\|\s*)(\w*?[A-Z]+\w*)(?<!ISBN)(\s*=\s*[^{}\|]{3,})", RegexOptions.Compiled);

        private static readonly Regex CiteFormatFieldTypo = new Regex(@"(\{\{\s*[Cc]it[^{}]*?\|\s*)(?:fprmat)(\s*=\s*)", RegexOptions.Compiled);

        //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_.3Cfont.3E_tags
        private static readonly Regex RemoveNoPropertyFontTags = new Regex(@"<font>([^<>]+)</font>", RegexOptions.IgnoreCase);

        // for fixing unbalanced brackets
        private static readonly Regex RefTemplateIncorrectBracesAtEnd = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>/]+?\s*)?>\s*)({{\s*[Cc]it[ae][^{}]+?)(?:}\]?|\)\))?(?=\s*</ref>)", RegexOptions.Compiled);
        private static readonly Regex RefExternalLinkUsingBraces = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>]+?\s*)?>\s*){{(\s*https?://[^{}\s\r\n]+)(\s+[^{}]+\s*)?}}(\s*</ref>)", RegexOptions.Compiled);
        private static readonly Regex TemplateIncorrectBracesAtStart = new Regex(@"(?:{\[|\[{)([^{}\[\]]+}})", RegexOptions.Compiled);
        private static readonly Regex CitationTemplateSingleBraceAtStart = new Regex(@"(?<=[^{])({\s*[Cc]it[ae])", RegexOptions.Compiled);
        private static readonly Regex ReferenceTemplateQuadBracesAtEnd = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>]+?\s*)?>\s*{{[^{}]+)}}(}}\s*</ref>)", RegexOptions.Compiled);
        private static readonly Regex CitationTemplateIncorrectBraceAtStart = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>]+?\s*)?>){\[([Cc]it[ae])", RegexOptions.Compiled);
        private static readonly Regex CitationTemplateIncorrectBracesAtEnd = new Regex(@"(<ref(?:\s*name\s*=[^{}<>]+?\s*)?>\s*{{[Cc]it[ae][^{}]+?)(?:}\]|\]}|{})(?=\s*</ref>)", RegexOptions.Compiled);
        private static readonly Regex RefExternalLinkMissingStartBracket = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>]+?\s*)?>[^{}\[\]<>]*?){?(https?://[^{}\[\]<>]+\][^{}\[\]<>]*</ref>)", RegexOptions.Compiled);
        private static readonly Regex RefExternalLinkMissingEndBracket = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>]+?\s*)?>[^{}\[\]<>]*?\[\s*https?://[^{}\[\]<>]+)(</ref>)", RegexOptions.Compiled);
        private static readonly Regex RefCitationMissingOpeningBraces = new Regex(@"(?<=<\s*ref(?:\s+name\s*=[^<>]*?)?\s*>\s*)\(?\(?(?=[Cc]it[ae][^{}]+}}\s*</ref>)", RegexOptions.Compiled);
        private static readonly Regex BracesWithinDefaultsort = new Regex(@"(?<={{DEFAULTSORT[^{}\[\]]+)[\]\[]+}}", RegexOptions.Compiled);

        // refs with wording and bare link: combine the two
        private static readonly Regex WordingIntoBareExternalLinks = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>]+?\s*)?>\s*)([^<>{}\[\]\r\n]{3,70}?)[\.,::]?\s*\[\s*((?:[Hh]ttp|[Hh]ttps|[Ff]tp|[Mm]ailto)://[^\ \n\r<>]+)\s*\](?=\s*</ref>)", RegexOptions.Compiled);

        // for correcting square brackets within external links
        private static readonly Regex SquareBracketsInExternalLinks = new Regex(@"(\[https?://(?>[^\[\]<>]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\])", RegexOptions.Compiled);

        // fix incorrect <br> of <br.>, <\br> and <br\>
        private static readonly Regex IncorrectBr = new Regex(@"< *br\. *>|<\\ *br *>|< *br *\\ *>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex SyntaxRegex14 = new Regex("</?i>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex SyntaxRegex15 = new Regex("</?b>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex SyntaxRegex16 = new Regex("^<hr>|^----+", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SyntaxRegex17 = new Regex("(^==?[^=]*==?)\r\n(\r\n)?----+", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SyntaxRegex18 = new Regex(@"HTTP/\d\.", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex19 = new Regex("ISBN: ?([0-9])", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex20 = new Regex(@"^\[(\s*http.*?)\]$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex SyntaxRegex21 = new Regex(@"([^]])\]([^]]|$)", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex22 = new Regex("\\[\\[[Ii]mage:[^]]*http", RegexOptions.Compiled);

        private static readonly Regex DoublePipeInWikiLink = new Regex(@"(?<=\[\[[^\[\[\r\n\|{}]+)\|\|(?=[^\[\[\r\n\|{}]+\]\])", RegexOptions.Compiled);

        // Covered by: LinkTests.TestFixSyntax(), incomplete
        /// <summary>
        /// Fixes and improves syntax (such as html markup)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixSyntax(string articleText)
        {
            //replace html with wiki syntax
            if (SyntaxRegex14.IsMatch(articleText))
                articleText = SyntaxRegexItalic.Replace(articleText, "''$1''");

            if (SyntaxRegex15.IsMatch(articleText))
                articleText = SyntaxRegexBold.Replace(articleText, "'''$1'''");

            articleText = SyntaxRegex16.Replace(articleText, "----");

            //remove appearance of double line break
            articleText = SyntaxRegex17.Replace(articleText, "$1");

            // double piped links e.g. [[foo||bar]]
            articleText = DoublePipeInWikiLink.Replace(articleText, "|");

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

            if (!SyntaxRegex18.IsMatch(articleText))
            {
                articleText = MissingColonInHttpLink.Replace(articleText, "$1tp://$2");
                articleText = SingleTripleSlashInHttpLink.Replace(articleText, "$1tp://$2");
            }

            articleText = SyntaxRegex19.Replace(articleText, "ISBN $1");

            articleText = CellpaddingTypo.Replace(articleText, "$1cellpadding");

            articleText = AccessdateTypo.Replace(articleText, "$1accessdate$2");

            articleText = AccessdateSynonyms.Replace(articleText, "accessdate");

            // {{cite web}} needs lower case field names; two loops in case a single template has multiple uppercase fields
            // restrict to en-wiki
            // exceptionally, 'ISBN' is allowed
            while (Variables.LangCode == "en")
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
                string externalLink = SyntaxRegex20.Replace(m.Value, "$1");

                // if there are some brackets left then they need fixing; the mediawiki parser finishes the external link
                // at the first ] found
                if (externalLink.Contains("]"))
                {
                    // replace single ] with &#93; when used for brackets in the link description
                    externalLink = SyntaxRegex21.Replace(externalLink, @"$1&#93;$2");

                    articleText = articleText.Replace(m.Value, @"[" + externalLink + @"]");
                }
            }

            // needs to be applied after SquareBracketsInExternalLinks
            if (!SyntaxRegex22.IsMatch(articleText))
            {
                articleText = SyntaxRegex4.Replace(articleText, "[[$1]]");
                articleText = SyntaxRegex5.Replace(articleText, "[[$1]]");
            }

            // if there are some unbalanced brackets, see whether we can fix them
            articleText = FixUnbalancedBrackets(articleText);

            // http://en.wikipedia.org/wiki/Wikipedia:WikiProject_Check_Wikipedia#Article_with_false_.3Cbr.2F.3E_.28AutoEd.29
            // fix incorrect <br> of <br.>, <\br> and <br\>
            articleText = IncorrectBr.Replace(articleText, "<br />");

            articleText = WordingIntoBareExternalLinks.Replace(articleText, @"[$2 $1]");

            return articleText.Trim();
        }

        private static readonly Regex CurlyBraceInsteadOfPipeInWikiLink = new Regex(@"(?<=\[\[[^\[\]{}<>\r\n\|]{1,50})}(?=[^\[\]{}<>\r\n\|]{1,50}\]\])", RegexOptions.Compiled);
        private static readonly Regex CurlyBraceInsteadOfBracketClosing = new Regex(@"(?<=\([^{}<>\(\)]+[^{}<>\(\)\|])}(?=[^{}])", RegexOptions.Compiled);
        private static readonly Regex CurlyBraceInsteadOfSquareBracket = new Regex(@"(?<=\[[^{}<>\[\]]+[^{}<>\(\)\|])}(?=[^{}])", RegexOptions.Compiled);
        private static readonly Regex CurlyBraceInsteadOfBracketOpening = new Regex(@"(?<=[^{}<>]){(?=[^{}<>\(\)\|][^{}<>\(\)]+\)[^{}\(\)])", RegexOptions.Compiled);
        private static readonly Regex ExtraBracketOnWikilinkOpening = new Regex(@"(?<=[^\[\]{}<>])(?:{\[\[?|\[\[\[)(?=[^\[\]{}<>]+\]\])", RegexOptions.Compiled);
        private static readonly Regex ExtraBracketOnWikilinkOpening2 = new Regex(@"(?<=\[\[){(?=[^{}\[\]<>]+\]\])", RegexOptions.Compiled);
        private static readonly Regex ExternalLinkMissingClosing = new Regex(@"(?<=^ *\* *\[ *https?://[^<>{}\[\]\r\n\s]+[^\[\]\r\n]*)(\s$)", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex ExternalLinkMissingOpening = new Regex(@"(?<=^ *\*) *(?=https?://[^<>{}\[\]\r\n\s]+[^\[\]\r\n]*\]\s$)", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex TemplateIncorrectClosingBraces = new Regex(@"(?<={{[^{}<>]{1,400}[^{}<>\|])(?:\]}|}\]?)(?=[^{}])", RegexOptions.Compiled);
        private static readonly Regex TemplateMissingOpeningBrace = new Regex(@"(?<=[^{}<>\|]){(?=[^{}<>]{1,400}}})", RegexOptions.Compiled);

        private static readonly Regex QuadrupleCurlyBrackets = new Regex(@"(?<=^{{[^{}\r\n]+}})}}(\s)$", RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Applies some fixes for unbalanced brackets, applied if there are unbalanced brackets
        /// </summary>
        /// <param name="articleText">the article text</param>
        /// <returns>the corrected article text</returns>
        private static string FixUnbalancedBrackets(string articleText)
        {
            // if there are some unbalanced brackets, see whether we can fix them
            // the fixes applied might damage correct wiki syntax, hence are only applied if there are unbalanced brackets
            // of the right type
            int bracketLength = 0;
            string articleTextTemp = articleText;
            int unbalancedBracket = UnbalancedBrackets(articleText, ref bracketLength);
            if (unbalancedBracket > -1)
            {
                int firstUnbalancedBracket = unbalancedBracket;
                char bracket = articleTextTemp[unbalancedBracket];

                // if it's ]]_]_ then see if removing bracket makes it all balance
                if (bracketLength == 1
                    && articleTextTemp[unbalancedBracket] == ']'
                    && articleTextTemp[unbalancedBracket - 1] == ']'
                    && articleTextTemp[unbalancedBracket - 2] == ']'
                    )
                {
                    articleTextTemp = articleTextTemp.Remove(unbalancedBracket, 1);
                }

                else if (bracketLength == 1)
                {
                    // if it's {[link]] or {[[link]] or [[[link]] then see if setting to [[ makes it all balance
                    articleTextTemp = ExtraBracketOnWikilinkOpening.Replace(articleTextTemp, "[[");

                    switch (bracket)
                    {
                        case '}':
                            // if it's [[blah blah}word]]
                            articleTextTemp = CurlyBraceInsteadOfPipeInWikiLink.Replace(articleTextTemp, "|");

                            // if it's (blah} then see if setting the } to a ) makes it all balance, but not |} which could be wikitables
                            articleTextTemp = CurlyBraceInsteadOfBracketClosing.Replace(articleTextTemp, ")");

                            // if it's [blah} then see if setting the } to a ] makes it all balance
                            articleTextTemp = CurlyBraceInsteadOfSquareBracket.Replace(articleTextTemp, "]");
                            break;

                        case '{':
                            // if it's {blah) then see if setting the { to a ( makes it all balance, but not {| which could be wikitables
                            articleTextTemp = CurlyBraceInsteadOfBracketOpening.Replace(articleTextTemp, "(");

                            // could be [[{link]]
                            articleTextTemp = ExtraBracketOnWikilinkOpening2.Replace(articleTextTemp, "");
                            break;

                        case '(':
                            // if it's ((word) then see if removing the extra opening round bracket makes it all balance
                            if (articleTextTemp.Length > (unbalancedBracket + 1)
                                && articleText[unbalancedBracket + 1] == '('
                                )
                            {
                                articleTextTemp = articleTextTemp.Remove(unbalancedBracket, 1);
                            }
                            break;

                        case '[':
                            // external link missing closing ]
                            articleTextTemp = ExternalLinkMissingClosing.Replace(articleTextTemp, "]$1");
                            break;

                        case ']':
                            // external link missing opening [
                            articleTextTemp = ExternalLinkMissingOpening.Replace(articleTextTemp, " [");
                            break;

                        case '>':
                            // <ref>>
                            articleTextTemp = articleTextTemp.Replace(@"<ref>>", @"<ref>");
                            break;

                        default:
                            // strange bracket
                            articleTextTemp = articleTextTemp.Replace("）", ")");
                            articleTextTemp = articleTextTemp.Replace("（", "(");
                            break;
                    }
                }

                if (bracketLength == 2)
                {
                    // if it's on double curly brackets, see if one is missing e.g. {{foo} or {{foo]}
                    articleTextTemp = TemplateIncorrectClosingBraces.Replace(articleTextTemp, "}}");

                    // {foo}}
                    articleTextTemp = TemplateMissingOpeningBrace.Replace(articleTextTemp, "{{");

                    // might be [[[[link]] or [[link]]]] so see if removing the two found square brackets makes it all balance
                    if (articleTextTemp.Substring(unbalancedBracket, Math.Min(4, articleTextTemp.Length - unbalancedBracket)) == "[[[["
                        || articleTextTemp.Substring(Math.Max(0, unbalancedBracket - 2), Math.Min(4, articleTextTemp.Length - unbalancedBracket)) == "]]]]")
                    {
                        articleTextTemp = articleTextTemp.Remove(unbalancedBracket, 2);
                    }

                    // {{Category: ?
                    articleTextTemp = articleTextTemp.Replace("{{" + Variables.Namespaces[Namespace.Category], "[[" + Variables.Namespaces[Namespace.Category]);
                    articleTextTemp = QuadrupleCurlyBrackets.Replace(articleTextTemp, "$1");                  
                }

                unbalancedBracket = UnbalancedBrackets(articleTextTemp, ref bracketLength);
                // the change worked if unbalanced bracket location moved considerably (so this one fixed), or all brackets now balance
                if (unbalancedBracket < 0 || Math.Abs(unbalancedBracket - firstUnbalancedBracket) > 300)
                    articleText = articleTextTemp;
            }

            return articleText;
        }

        private static readonly Regex CiteTemplateFormatHTML = new Regex(@"\|\s*format\s*=\s*(?:HTML?|\[\[HTML?\]\]|html?)\s*(?=\||}})", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateFormatnull = new Regex(@"\|\s*format\s*=\s*(?=\||}})", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateLangEnglish = new Regex(@"\|\s*language\s*=\s*(?:[Ee]nglish)\s*(?=\||}})", RegexOptions.Compiled);
        private static readonly Regex CiteTemplatePagesPP = new Regex(@"(?<=\|\s*pages?\s*=\s*)pp?\.\s*(?=[^{}\|]+(?:\||}}))", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateHTMLURL = new Regex(@"\|\s*url\s*=\s*[^<>{}\s\|]+?\.(?:HTML?|html?)\s*(?:\||}})", RegexOptions.Compiled);
        private static readonly Regex CiteTemplates = new Regex(@"{{\s*[Cc]it[ae]((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}})", RegexOptions.Compiled);

        /// <summary>
        /// Applies various formatting fixes to citation templates
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string FixCitationTemplates(string articleText)
        {
            foreach (Match m in CiteTemplates.Matches(articleText))
            {
                string newValue = m.Value;
                // remove the unneeded 'format=HTML' field
                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_.22format.3DHTML.22_in_citation_templates
                newValue = CiteTemplateFormatHTML.Replace(newValue, "");

                // remove language=English on en-wiki
                if (Variables.LangCode == "en")
                    newValue = CiteTemplateLangEnglish.Replace(newValue, "");

                // remove format= field with null value when URL is HTML page
                if (CiteTemplateHTMLURL.IsMatch(newValue))
                    newValue = CiteTemplateFormatnull.Replace(newValue, "");

                // page= and pages= fields don't need p. or pp. in them
                newValue = CiteTemplatePagesPP.Replace(newValue, "");

                articleText = articleText.Replace(m.Value, newValue);
            }

            return articleText;
        }

        // Covered by: LinkTests.TestCanonicalizeTitle(), incomplete
        /// <summary>
        /// returns URL-decoded link target
        /// </summary>
        public static string CanonicalizeTitle(string title)
        {
            // visible parts of links may contain crap we shouldn't modify, such as
            // refs and external links
            if (!Tools.IsValidTitle(title) || title.Contains(":/"))
                return title;

            //AnchorDecode(ref title); // disabled, breaks things such as [[Windows#Version_3.11]]

            Variables.WaitForDelayedRequests();
            string s = CanonicalizeTitleRaw(title);
            if (Variables.UnderscoredTitles.Contains(Tools.TurnFirstToUpper(s)))
            {
                return HttpUtility.UrlDecode(title.Replace("+", "%2B"))
                    .Trim(new[] { '_' });
            }
            return s;
        }

        /// <summary>
        /// Turns a title into its canonical form, could be slow
        /// </summary>
        public static string CanonicalizeTitleAggressively(string title)
        {
            title = Tools.RemoveHashFromPageTitle(title);
            title = Tools.WikiDecode(title).Trim();
            title = Tools.TurnFirstToUpper(title);

            if (!title.Contains(":")) return title;

            string prevTitle = title;
            foreach (var p in Variables.NamespacesCaseInsensitive)
            {
                title = Regex.Replace(title, @"^\s*" + p.Value + @"\s*", "");
                if (title == prevTitle) continue;

                title = Variables.Namespaces[p.Key] + Tools.TurnFirstToUpper(title);
                break;
            }
            return title;
        }

        private static readonly Regex SingleCurlyBrackets = new Regex(@"{((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))})", RegexOptions.Compiled);
        private static readonly Regex DoubleSquareBrackets = new Regex(@"\[\[((?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\]\])", RegexOptions.Compiled);
        private static readonly Regex SingleSquareBrackets = new Regex(@"\[((?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\])", RegexOptions.Compiled);
        private static readonly Regex SingleRoundBrackets = new Regex(@"\(((?>[^\(\)]+|\((?<DEPTH>)|\)(?<-DEPTH>))*(?(DEPTH)(?!))\))", RegexOptions.Compiled);
        private static readonly Regex Tags = new Regex(@"\<((?>[^\<\>]+|\<(?<DEPTH>)|\>(?<-DEPTH>))*(?(DEPTH)(?!))\>)", RegexOptions.Compiled);
        private static readonly Regex HideNestedBrackets = new Regex(@"[^\[\]{}<>]\[[^\[\]{}<>]*?&#93;", RegexOptions.Compiled);

        /// <summary>
        /// Checks the article text for unbalanced brackets, either square or curly
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="bracketLength">integer to hold length of unbalanced bracket found</param>
        /// <returns>Index of any unbalanced brackets found</returns>
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Missing_opening_or_closing_brackets.2C_table_and_template_markup_.28WikiProject_Check_Wikipedia_.23_10.2C_28.2C_43.2C_46.2C_47.29
        public static int UnbalancedBrackets(string articleText, ref int bracketLength)
        {
            // &#93; is used to replace the ] in external link text, which gives correct markup
            // replace [...&#93; with spaces to avoid matching as unbalanced brackets
            articleText = HideNestedBrackets.Replace(articleText, " ");

            // remove all <math>, <code> stuff etc. where curly brackets are used in singles and pairs
            articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.MathPreSourceCodeComments);
            
            bracketLength = 2;

            int unbalancedfound = UnbalancedBrackets(articleText, "{{", "}}", WikiRegexes.NestedTemplates);
            if (unbalancedfound > -1)
                return unbalancedfound;

            unbalancedfound = UnbalancedBrackets(articleText, "[[", "]]", DoubleSquareBrackets);
            if (unbalancedfound > -1)
                return unbalancedfound;

            bracketLength = 1;

            unbalancedfound = UnbalancedBrackets(articleText, "{", "}", SingleCurlyBrackets);
            if (unbalancedfound > -1)
                return unbalancedfound;

            unbalancedfound = UnbalancedBrackets(articleText, "[", "]", SingleSquareBrackets);
            if (unbalancedfound > -1)
                return unbalancedfound;

            unbalancedfound = UnbalancedBrackets(articleText, "(", ")", SingleRoundBrackets);
            if (unbalancedfound > -1)
                return unbalancedfound;

            // look for unbalanced tags
            unbalancedfound = UnbalancedBrackets(articleText, "<", ">", Tags);
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
            //TODO: move everything possible to the parent function, however, it shouldn't be performed blindly,
            //without a performance review
            if (Regex.Matches(articleText, Regex.Escape(openingBrackets)).Count !=
                Regex.Matches(articleText, Regex.Escape(closingBrackets)).Count)
            {
                if (openingBrackets == "[")
                {
                    // need to remove double square brackets first
                    articleText = Tools.ReplaceWithSpaces(articleText, DoubleSquareBrackets);
                }

                if (openingBrackets == "{")
                {
                    // need to remove double curly brackets first
                    articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.NestedTemplates);
                }

                // replace all the valid balanced bracket sets with spaces
                articleText = Tools.ReplaceWithSpaces(articleText, bracketsRegex);

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
        [Obsolete("unfit for production use")]
        public static void AnchorDecode(ref string link)
        {
            Match m = WikiRegexes.AnchorEncodedLink.Match(link);
            if (!m.Success)
                return;

            string anchor = m.Value.Replace('_', ' ');
            byte[] src = Encoding.UTF8.GetBytes(anchor);
            byte[] dest = (byte[])src.Clone();

            int destCount = 0;

            for (int srcCount = 0; srcCount < src.Length; srcCount++)
            {
                if (src[srcCount] != '.' || !(srcCount + 3 <= src.Length &&
                    IsHex(src[srcCount + 1]) && IsHex(src[srcCount + 2])))
                    // then
                    dest[destCount] = src[srcCount];
                else
                {
                    dest[destCount] = DecodeHex(src[srcCount + 1], src[srcCount + 2]);
                    srcCount += 2;
                }

                destCount++;
            }

            link = link.Replace(m.Value, Encoding.UTF8.GetString(dest, 0, destCount));
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
            articleText = LinkWhitespace1.Replace(articleText, " [[$1]]");

            //remove undesirable space from beginning of wikilink and move it outside link - parse this line second
            articleText = LinkWhitespace2.Replace(articleText, " [[$1]]");

            //remove undesirable double space from middle of wikilink (up to 61 characters in wikilink)
            articleText = LinkWhitespace3.Replace(articleText, "[[$1 $2]]");

            //remove undesirable space from end of wikilink (space after wikilink) - parse this line first
            articleText = LinkWhitespace4.Replace(articleText, "[[$1]] ");

            //remove undesirable space from end of wikilink and move it outside link - parse this line second
            articleText = LinkWhitespace5.Replace(articleText, "[[$1]] ");

            //remove undesirable double space between links in date (day first)
            articleText = DateLinkWhitespace1.Replace(articleText, "$1 $2");

            //remove undesirable double space between links in date (day second)
            articleText = DateLinkWhitespace2.Replace(articleText, "$1 $2");

            // correct [[page# section]] to [[page#section]]
            if (articleTitle.Length > 0)
            {
                Regex sectionLinkWhitespace = new Regex(@"(\[\[" + Regex.Escape(articleTitle) + @"\#)\s+([^\[\]]+\]\])");

                return sectionLinkWhitespace.Replace(articleText, "$1$2");
            }

            return articleText;
        }

        /// <summary>
        /// Fix leading, trailing and middle spaces in Wikilinks
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        [Obsolete]
        public static string FixLinkWhitespace(string articleText)
        {
            return FixLinkWhitespace(articleText, "");
        }

        // Partially covered by FixMainArticleTests.SelfLinkRemoval()
        /// <summary>
        /// Fixes link syntax, including removal of self links
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public static string FixLinks(string articleText, string articleTitle, out bool noChange)
        {
            string articleTextAtStart = articleText;
            string escTitle = Regex.Escape(articleTitle);

            if (Regex.IsMatch(articleText, @"{{[Ii]nfobox (?:[Ss]ingle|[Aa]lbum)"))
                articleText = FixLinksInfoBoxSingleAlbum(articleText, articleTitle);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Your_code_creates_page_errors_inside_imagemap_tags.
            // don't apply if there's an imagemap on the page or some noinclude transclusion business
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Includes_and_selflinks
            // TODO, better to not apply to text within imagemaps
            if (!WikiRegexes.ImageMap.IsMatch(articleText) && !WikiRegexes.Noinclude.IsMatch(articleText) && !WikiRegexes.Includeonly.IsMatch(articleText))
            {
                // remove any self-links, but not other links with different capitaliastion e.g. [[Foo]] vs [[FOO]]
                articleText = Regex.Replace(articleText, @"\[\[\s*(" + Tools.CaseInsensitive(escTitle)
                    + @")\s*\]\]", "$1");

                // remove piped self links by leaving target
                articleText = Regex.Replace(articleText, @"\[\[\s*" + Tools.CaseInsensitive(escTitle)
                    + @"\s*\|\s*([^\]]+)\s*\]\]", "$1");
            }

            // clean up wikilinks: replace underscores, percentages and URL encoded accents etc.
            StringBuilder sb = new StringBuilder(articleText, (articleText.Length * 11) / 10);

            foreach (Match m in WikiRegexes.WikiLink.Matches(articleText))
            {
                if (m.Groups[1].Value.Length > 0)
                {
                    string y = m.Value.Replace(m.Groups[1].Value, CanonicalizeTitle(m.Groups[1].Value));

                    if (y != m.Value)
                        sb = sb.Replace(m.Value, y);
                }
            }

            noChange = (sb.ToString() == articleTextAtStart);

            return sb.ToString();
        }

        /// <summary>
        /// Converts self links for the 'this single/album' field of 'infobox single/album' to bold
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <returns></returns>
        private static string FixLinksInfoBoxSingleAlbum(string articleText, string articleTitle)
        {
            string escTitle = Regex.Escape(articleTitle);
            string lowerTitle = Tools.TurnFirstToLower(escTitle);
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#.22This_album.2Fsingle.22
            // for this single or this album within the infobox, make bold instead of delinking
            const string infoBoxSingleAlbum = @"(?s)(?<={{[Ii]nfobox (?:[Ss]ingle|[Aa]lbum).*?\|\s*[Tt]his (?:[Ss]ingle|[Aa]lbum)\s*=[^{}]*?)\[\[\s*";
            articleText = Regex.Replace(articleText, infoBoxSingleAlbum + escTitle + @"\s*\]\](?=[^{}\|]*(?:\||}}))", @"'''" + articleTitle + @"'''");
            articleText = Regex.Replace(articleText, infoBoxSingleAlbum + lowerTitle + @"\s*\]\](?=[^{}\|]*(?:\||}}))", @"'''" + lowerTitle + @"'''");
            articleText = Regex.Replace(articleText, infoBoxSingleAlbum + escTitle + @"\s*\|\s*([^\]]+)\s*\]\](?=[^{}\|]*(?:\||}}))", @"'''" + "$1" + @"'''");
            articleText = Regex.Replace(articleText, infoBoxSingleAlbum + lowerTitle + @"\s*\|\s*([^\]]+)\s*\]\](?=[^{}\|]*(?:\||}}))", @"'''" + "$1" + @"'''");

            return articleText;
        }

        // covered by CanonicalizeTitleRawTests
        /// <summary>
        /// Performs URL-decoding of a page title, trimming all whitespace
        /// </summary>
        public static string CanonicalizeTitleRaw(string title)
        {
            return CanonicalizeTitleRaw(title, true);
        }

        // covered by CanonicalizeTitleRawTests
        /// <summary>
        /// performs URL-decoding of a page title
        /// </summary>
        /// <param name="title">title to normalise</param>
        /// <param name="trim">whether whitespace should be trimmed</param>
        public static string CanonicalizeTitleRaw(string title, bool trim)
        {
            title = HttpUtility.UrlDecode(title.Replace("+", "%2B").Replace('_', ' '));
            return trim ? title.Trim() : title;
        }

        // Covered by: UtilityFunctionTests.IsCorrectEditSummary()
        /// <summary>
        /// returns true if given string has matching double square brackets
        /// </summary>
        public static bool IsCorrectEditSummary(string s)
        {
            if (s.Length > 255)
                return false;

            bool res = true;

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

                    if (b.Length == 0)
                        continue;

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
                        if (doBreak)
                            continue;
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

                    if (b.Trim().Length == 0 || a.Contains(","))
                        continue;

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
            string newText = BulletExternalLinks(articleText);

            noChange = (newText == articleText);

            return newText;
        }

        private static readonly HideText BulletExternalHider = new HideText(false, true, false);

        private static readonly Regex ExternalLinksSection = new Regex(@"=\s*(?:external)?\s*links\s*=", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
        private static readonly Regex NewlinesBeforeHTTP = new Regex("(\r\n|\n)?(\r\n|\n)(\\[?http)", RegexOptions.Compiled);

        // Covered by: LinkTests.TestBulletExternalLinks()
        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string BulletExternalLinks(string articleText)
        {
            Match m = ExternalLinksSection.Match(articleText);

            if (!m.Success)
                return articleText;

            int intStart = m.Index;

            string articleTextSubstring = articleText.Substring(intStart);
            articleText = articleText.Substring(0, intStart);
            articleTextSubstring = BulletExternalHider.HideMore(articleTextSubstring);
            articleTextSubstring = NewlinesBeforeHTTP.Replace(articleTextSubstring, "$2* $3");

            return articleText + BulletExternalHider.AddBackMore(articleTextSubstring);
        }

        private static readonly Regex WordWhitespaceEndofline = new Regex(@"(\w+)\s+$", RegexOptions.Compiled);

        // Covered by: LinkTests.TestFixCategories()
        /// <summary>
        /// Fix common spacing/capitalisation errors in categories; remove diacritics and trailing whitespace from sortkeys (not leading whitespace)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="isMainSpace"></param>
        /// <returns>The modified article text.</returns>
        public static string FixCategories(string articleText, bool isMainSpace)
        {
            string cat = "[[" + Variables.Namespaces[Namespace.Category];

            // fix extra brackets: three or more at end
            articleText = Regex.Replace(articleText, @"(?<=" + Regex.Escape(cat) + @"[^\r\n\[\]{}<>]+\]\])\]+", "");
            // three or more at start
            articleText = Regex.Replace(articleText, @"\[+(?=" + Regex.Escape(cat) + @"[^\r\n\[\]{}<>]+\]\])", "");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Substituted_templates
            // for en-wiki mainspace articles any match on these three regexes means there's some dodgy template programming that should be cleaned up, so add to cleanup category
            // only add cat "if the page is actually transcluded somewhere and only apply [category] if not"
            //       if (isMainSpace && Variables.LangCode == LangCodeEnum.en && NoIncludeIncludeOnlyProgrammingElement(articleText) && !articleText.Contains(@"[[Category:Substituted templates]]"))
            //           articleText += "\r\n" + @"[[Category:Substituted templates]]";

            foreach (Match m in WikiRegexes.LooseCategory.Matches(articleText))
            {
                if (!Tools.IsValidTitle(m.Groups[1].Value))
                    continue;
                string x = cat + Tools.TurnFirstToUpper(CanonicalizeTitleRaw(m.Groups[1].Value, false).Trim()) +
                           WordWhitespaceEndofline.Replace(Tools.RemoveDiacritics(m.Groups[2].Value), "$1") + "]]";
                if (x != m.Value)
                    articleText = articleText.Replace(m.Value, x);
            }

            return articleText;
        }

        /// <summary>
        /// Fix common spacing/capitalisation errors in categories; remove diacritics and trailing whitespace from sortkeys (not leading whitespace)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixCategories(string articleText)
        {
            return FixCategories(articleText, false);
        }

        /// <summary>
        /// Returns whether the article text has a &lt;noinclude&gt; or &lt;includeonly&gt; or '{{{1}}}' etc. which should not appear on the mainspace
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static bool NoIncludeIncludeOnlyProgrammingElement(string articleText)
        {
            return WikiRegexes.Noinclude.IsMatch(articleText) || WikiRegexes.Includeonly.IsMatch(articleText) || Regex.IsMatch(articleText, @"{{{\d}}}");
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

        private static readonly Regex Temperature = new Regex(@"(?:&deg;|&ordm;|º|°)(?:&nbsp;)?\s*([CcFf])(?![A-Za-z])", RegexOptions.Compiled);

        /// <summary>
        /// Fix bad Temperatures
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixTemperatures(string articleText)
        {
            foreach (Match m in Temperature.Matches(articleText))
                articleText = articleText.Replace(m.ToString(), "°" + m.Groups[1].Value.ToUpper());
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

            articleText = WikiRegexes.UnitsWithoutNonBreakingSpaces.Replace(articleText, "$1&nbsp;$2");

            articleText = WikiRegexes.ImperialUnitsInBracketsWithoutNonBreakingSpaces.Replace(articleText, "$1&nbsp;$2");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Pagination
            // add non-breaking space after pp. abbreviation for pages.
            articleText = Regex.Replace(articleText, @"(\b[Pp]?p\.) *(?=[\dIVXCL])", @"$1&nbsp;");

            return AddBackMoreText(articleText);
        }

        /// <summary>
        /// regex that Matches every template, for GetTemplate
        /// </summary>
        public const string EveryTemplate = @"[^\|\{\}]+";

        /// <summary>
        /// Extracts template using the given match.
        /// </summary>
        private static string ExtractTemplate(string articleText, Match m)
        {
            Regex theTemplate = new Regex(Regex.Escape(m.Groups[1].Value) + @"(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}}");

            foreach (Match n in theTemplate.Matches(articleText))
            {
                if (n.Index == m.Index)
                    return theTemplate.Match(articleText).Value;
            }

            return "";
        }

        /// <summary>
        /// Finds first occurrence of a given template in article text.
        /// Handles nested templates correctly.
        /// </summary>
        /// <param name="articleText">Source text</param>
        /// <param name="template">Name of template, can be regex without a group capture</param>
        /// <returns>Template with all params, enclosed in curly brackets</returns>
        public static string GetTemplate(string articleText, string template)
        {
            Regex search = new Regex(@"(\{\{\s*" + Tools.CaseInsensitive(template) + @"\s*)(?:\||\}|<)", RegexOptions.Singleline);

            // remove commented out templates etc. before searching
            string articleTextCleaned = WikiRegexes.UnFormattedText.Replace(articleText, "");

            if (search.IsMatch(articleTextCleaned))
            {
                // extract from original article text
                Match m = search.Match(articleText);

                return m.Success ? ExtractTemplate(articleText, m) : "";
            }

            return "";
        }

        private static readonly Dictionary<string, Regex> CachedGetTemplatesRegexes = new Dictionary<string, Regex>();
        /// <summary>
        /// Finds every occurrence of a given template in article text.
        /// Handles nested templates correctly.
        /// </summary>
        /// <param name="articleText">Source text</param>
        /// <param name="template">Name of template, can be regex without a group capture</param>
        /// <returns>Template with all params, enclosed in curly brackets</returns>
        public static MatchCollection GetTemplates(string articleText, string template)
        {
            // replace with spaces any commented out templates etc., this means index of real matches remains the same as in actual article text
            articleText =  Tools.ReplaceWithSpaces(articleText,WikiRegexes.UnFormattedText);

            string ciTemplateName = Tools.CaseInsensitive(template);
            Regex search;

            lock (CachedGetTemplatesRegexes)
            {
                if (CachedGetTemplatesRegexes.ContainsKey(ciTemplateName))
                    search = CachedGetTemplatesRegexes[ciTemplateName];
                else
                {
                    search = new Regex(@"{{\s*" + ciTemplateName + @"\s*(\|((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}})|}})", RegexOptions.Compiled);
                    CachedGetTemplatesRegexes[ciTemplateName] = search;
                }
            }

            return search.Matches(articleText);
        }

        // covered by GetTemplateNameTests
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
                if (string.IsNullOrEmpty(setting))
                    return "";

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
        /// Performs transformations related to Unicode characters that may cause problems for different clients
        /// </summary>
        public string FixUnicode(string articleText)
        {
            // http://en.wikipedia.org/wiki/Wikipedia:AWB/B#Line_break_insertion
            // most brosers handle Unicode line separator as whitespace, so should we
            // looks like paragraph separator is properly converted by RichEdit itself
            return articleText.Replace('\x2028', ' ');
        }

        /// <summary>
        /// Converts HTML entities to unicode, with some deliberate exceptions
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string Unicodify(string articleText, out bool noChange)
        {
            string newText = Unicodify(articleText);

            noChange = (newText == articleText);

            return newText;
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

        /// <summary>
        /// Delinks all bolded self links in the article
        /// </summary>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        private static string BoldedSelfLinks(string articleTitle, string articleText)
        {
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#If_a_selflink_is_also_bolded.2C_AWB_should_just_remove_the_selflink

            string escTitle = Regex.Escape(articleTitle);
            //string escTitleNoBrackets = Regex.Escape(Regex.Replace(articleTitle, @" \(.*?\)$", ""));

            Regex r1 = new Regex(@"'''\[\[\s*" + escTitle + @"\s*\]\]'''");
            Regex r2 = new Regex(@"'''\[\[\s*" + Tools.TurnFirstToLower(escTitle) + @"\s*\]\]'''");

            if (!WikiRegexes.Noinclude.IsMatch(articleText) && !WikiRegexes.Includeonly.IsMatch(articleText))
            {
                articleText = r1.Replace(articleText, @"'''" + articleTitle + @"'''");

                articleText = r2.Replace(articleText, @"'''" + Tools.TurnFirstToLower(articleTitle) + @"'''");
            }

            return articleText;
        }

        private static readonly Regex BracketedAtEndOfLine = new Regex(@" \(.*?\)$", RegexOptions.Compiled);
        private static readonly Regex BoldTitleAlready3 = new Regex(@"^\s*({{[^\{\}]+}}\s*)*'''('')?\s*\w", RegexOptions.Compiled);
        // Covered by: BoldTitleTests
        /// <summary>
        /// '''Emboldens''' the first occurrence of the article title, if not already bold
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The title of the article.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string BoldTitle(string articleText, string articleTitle, out bool noChange)
        {
            // clean up bolded self links first
            articleText = BoldedSelfLinks(articleTitle, articleText);

            noChange = true;
            string escTitle = Regex.Escape(articleTitle);
            string escTitleNoBrackets = Regex.Escape(BracketedAtEndOfLine.Replace(articleTitle, ""));

            string articleTextAtStart = articleText;

            string zerothSection = WikiRegexes.ZerothSection.Match(articleText).Value;
            string restOfArticle = (zerothSection.Length > 0) ? articleText.Replace(zerothSection, "") : "";

            // There's a limitation here in that we can't hide image descriptions that may be above lead sentence without hiding the self links we are looking to correct
            string zerothSectionHidden = Hider.HideMore(zerothSection, false, false);
            string zerothSectionHiddenOriginal = zerothSectionHidden;

            // first check for any self links and no bold title, if found just convert first link to bold and return
            Regex r1 = new Regex(@"\[\[\s*" + escTitle + @"\s*\]\]");
            Regex r2 = new Regex(@"\[\[\s*" + Tools.TurnFirstToLower(escTitle) + @"\s*\]\]");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Includes_and_selflinks
            // don't apply if bold in lead section already or some noinclude transclusion business
            if (!Regex.IsMatch(zerothSection, "'''" + escTitle + "'''") && !WikiRegexes.Noinclude.IsMatch(articleText) && !WikiRegexes.Includeonly.IsMatch(articleText))
                zerothSectionHidden = r1.Replace(zerothSectionHidden, "'''" + articleTitle + @"'''");
            if (zerothSectionHiddenOriginal == zerothSectionHidden && !Regex.IsMatch(zerothSection, @"'''" + Tools.TurnFirstToLower(escTitle) + @"'''"))
                zerothSectionHidden = r2.Replace(zerothSectionHidden, "'''" + Tools.TurnFirstToLower(articleTitle) + @"'''");

            if (zerothSectionHiddenOriginal != zerothSectionHidden)
            {
                noChange = false;
                return (Hider.AddBackMore(zerothSectionHidden) + restOfArticle);
            }

            // so no self links to remove, check for the need to add bold     
            string articleTextHidden = HideMoreText(articleText);

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

            //if title in bold already exists in article, or page starts with something in bold, don't change anything
            if (boldTitleAlready1.IsMatch(articleText) || boldTitleAlready2.IsMatch(articleText)
                || BoldTitleAlready3.IsMatch(articleText))
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
                return articleTextAtStart;
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
            string newText = ReplaceImage(oldImage, newImage, articleText);

            noChange = (newText == articleText);

            return newText;
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

            return Regex.Replace(articleText, oldImage, newImage);
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
            string newText = RemoveImage(image, articleText, commentOut, comment);

            noChange = (newText == articleText);

            return newText;
        }

        /// <summary>
        /// Adds the category to the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="newCategory">The new category.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="noChange"></param>
        /// <returns>The article text.</returns>
        public string AddCategory(string newCategory, string articleText, string articleTitle, out bool noChange)
        {
            string newText = AddCategory(newCategory, articleText, articleTitle);

            noChange = (newText == articleText);

            return newText;
        }

        // Covered by: RecategorizerTests.Addition()
        /// <summary>
        /// Adds the category to the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="newCategory">The new category.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <returns>The article text.</returns>
        public string AddCategory(string newCategory, string articleText, string articleTitle)
        {
            string oldText = articleText;

            articleText = FixCategories(articleText, Namespace.IsMainSpace(articleText));

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
            return ReCategoriser(oldCategory, newCategory, articleText, out noChange, false);
        }

        // Covered by: RecategorizerTests.Replacement()
        /// <summary>
        /// Re-categorises the article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="oldCategory">The old category to replace.</param>
        /// <param name="newCategory">The new category.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <param name="removeSortKey">If set, any sort key is removed when the category is replaced</param>
        /// <returns>The re-categorised article text.</returns>
        public static string ReCategoriser(string oldCategory, string newCategory, string articleText, out bool noChange, bool removeSortKey)
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

                oldCategory = Variables.Namespaces[Namespace.Category] + oldCategory + @"\s*(\|[^\|\[\]]+\]\]|\]\])";

                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Replacing_categoring_and_keeping_pipes
                if (!removeSortKey)
                    newCategory = Variables.Namespaces[Namespace.Category] + newCategory + "$1";
                else
                    newCategory = Variables.Namespaces[Namespace.Category] + newCategory + @"]]";

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

            articleText = RemoveCategory(strOldCat, articleText);

            noChange = (testText == articleText);

            return articleText;
        }

        /// <summary>
        /// Removes a category from an article.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="strOldCat">The old category to remove.</param>
        /// <returns>The article text without the old category.</returns>
        public static string RemoveCategory(string strOldCat, string articleText)
        {
            strOldCat = Tools.CaseInsensitive(Regex.Escape(strOldCat));

            if (!articleText.Contains("<includeonly>"))
                articleText = Regex.Replace(articleText, "\\[\\["
                    + Variables.NamespacesCaseInsensitive[Namespace.Category] + " ?"
                    + strOldCat + "( ?\\]\\]| ?\\|[^\\|]*?\\]\\])\r\n", "");

            articleText = Regex.Replace(articleText, "\\[\\["
                + Variables.NamespacesCaseInsensitive[Namespace.Category] + " ?"
                + strOldCat + "( ?\\]\\]| ?\\|[^\\|]*?\\]\\])", "");

            return articleText;
        }

        /// <summary>
        /// Returns whether the input string matches the name of a category in use in the input article text string, based on a case insensitive match
        /// </summary>
        /// <param name="articleText">the article text</param>
        /// <param name="categoryName">name of the category</param>
        /// <returns></returns>
        public static bool CategoryMatch(string articleText, string categoryName)
        {
            Regex anyCategory = new Regex(@"\[\[\s*" + Variables.NamespacesCaseInsensitive[Namespace.Category] +
                  @"\s*" + Regex.Escape(categoryName) + @"\s*(?:|\|([^\|\]]*))\s*\]\]", RegexOptions.IgnoreCase);

            return anyCategory.IsMatch(articleText);
        }

        /// <summary>
        /// Changes an article to use defaultsort when all categories use the same sort field / cleans diacritics from defaultsort/categories
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="noChange">If there is no change (True if no Change)</param>
        /// <returns>The article text possibly using defaultsort.</returns>
        public static string ChangeToDefaultSort(string articleText, string articleTitle, out bool noChange)
        {
            return ChangeToDefaultSort(articleText, articleTitle, out noChange, false);
        }

        /// <summary>
        /// Returns the sortkey used by all categories, if 
        /// * all categories use the same sortkey
        /// * no {{DEFAULTSORT}} in article
        /// Otherwise returns null
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string GetCategorySort(string articleText)
        {
            if (WikiRegexes.Defaultsort.Matches(articleText).Count == 1)
                return "";

            int matches;
            const string dummy = @"@@@@";

            string sort = GetCategorySort(articleText, dummy, out matches);

            return sort == dummy ? "" : sort;
        }

        /// <summary>
        /// Returns the sortkey used by all categories, if all categories use the same sortkey
        /// Where no sortkey is used for all categories, returns the articletitle
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static string GetCategorySort(string articleText, string articleTitle, out int matches)
        {
            string sort = "";
            bool allsame = true;
            matches = 0;

            foreach (Match m in WikiRegexes.Category.Matches(articleText))
            {
                string explicitKey = m.Groups[2].Value;
                if (explicitKey.Length == 0)
                    explicitKey = articleTitle;

                if (string.IsNullOrEmpty(sort))
                    sort = explicitKey;

                if (sort != explicitKey && explicitKey != "")
                {
                    allsame = false;
                    break;
                }
                matches++;
            }
            if (allsame && matches > 0)
                return sort;
            return "";
        }

        // Covered by: UtilityFunctionTests.ChangeToDefaultSort()
        /// <summary>
        /// Changes an article to use defaultsort when all categories use the same sort field / cleans diacritics from defaultsort/categories
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="noChange">If there is no change (True if no Change)</param>
        /// <param name="restrictDefaultsortChanges">Prevent insertion of a new {{DEFAULTSORT}} as AWB may not always be right for articles about people</param>
        /// <returns>The article text possibly using defaultsort.</returns>
        public static string ChangeToDefaultSort(string articleText, string articleTitle, out bool noChange, bool restrictDefaultsortChanges)
        {
            string testText = articleText;
            noChange = true;

            // count categories
            int matches;

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_12#defaultsort_adding_namespace
            if (!Namespace.IsMainSpace(articleTitle))
                articleTitle = Tools.RemoveNamespaceString(articleTitle);

            string sort = GetCategorySort(articleText, articleTitle, out matches);

            // clean diacritics from any lifetime template
            if (WikiRegexes.Lifetime.Matches(articleText).Count == 1)
            {
                Match m = WikiRegexes.Lifetime.Match(articleText);
                articleText = articleText.Replace(m.Value, Tools.RemoveDiacritics(m.Value));
                noChange = (testText == articleText);
            }

            // limited support for {{Lifetime}}
            MatchCollection ds = WikiRegexes.Defaultsort.Matches(articleText);
            if (WikiRegexes.Lifetime.IsMatch(articleText) || ds.Count > 1 || (ds.Count == 1 && !ds[0].Value.ToUpper().Contains("DEFAULTSORT")))
            {
                bool allsame2 = false;
                string lastvalue = "";
                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Detect_multiple_DEFAULTSORT
                // if all the defaultsorts are the same just remove all but one
                foreach (Match m in WikiRegexes.Defaultsort.Matches(articleText))
                {
                    if (lastvalue.Length == 0)
                    {
                        lastvalue = m.Value;
                        allsame2 = true;
                    }
                    else
                        allsame2 = (m.Value == lastvalue);
                }
                if (allsame2)
                    articleText = WikiRegexes.Defaultsort.Replace(articleText, "", ds.Count - 1);
                else
                    return articleText;
            }

            articleText = TalkPages.TalkPageHeaders.FormatDefaultSort(articleText);

            // match again, after normalisation
            ds = WikiRegexes.Defaultsort.Matches(articleText);
            if (ds.Count > 1)
                return testText;

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#AWB_didn.27t_fix_special_characters_in_a_pipe
            articleText = FixCategories(articleText, Namespace.IsMainSpace(articleTitle));

            // AWB's generation of its own sortkey may be incorrect for people, provide option not to insert in this situation
            if (ds.Count == 0 && !restrictDefaultsortChanges)
            {
                // So that this doesn't get confused by sort keys of "*", " ", etc.
                // MW bug: DEFAULTSORT doesn't treat leading spaces the same way as categories do
                // if all existing categories use a suitable sortkey, insert that rather than generating a new one
                // GetCatSortkey just returns articleTitle if cats don't have sortkey, so don't accept this here
                if (sort.Length > 4 && matches > 1 && !sort.StartsWith(" "))
                {
                    // remove keys from categories
                    articleText = WikiRegexes.Category.Replace(articleText, "[["
                        + Variables.Namespaces[Namespace.Category] + "$1]]");

                    // set the defaultsort to the existing unique category sort value
                    // don't add a defaultsort if cat sort was the same as article title
                    if (sort != articleTitle && Tools.FixupDefaultSort(sort) != articleTitle)
                        articleText = articleText + "\r\n{{DEFAULTSORT:" + Tools.FixupDefaultSort(sort) + "}}";
                }

                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Add_defaultsort_to_pages_with_special_letters_and_no_defaultsort
                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Human_DEFAULTSORT
                articleText = DefaultsortTitlesWithDiacritics(articleText, articleTitle, matches, IsArticleAboutAPerson(articleText, articleTitle, true));
            }
            else if (ds.Count == 1) // already has DEFAULTSORT
            {
                string s = Tools.FixupDefaultSort(ds[0].Groups[1].Value).Trim();

                if (s != ds[0].Groups[1].Value && s.Length > 0 && !restrictDefaultsortChanges)
                    articleText = articleText.Replace(ds[0].Value, "{{DEFAULTSORT:" + s + "}}");

                // get key value again in case replace above changed it
                ds = WikiRegexes.Defaultsort.Matches(articleText);
                string defaultsortKey = ds[0].Groups["key"].Value;

                //Removes any explicit keys that are case insensitively the same as the default sort (To help tidy up on pages that already have defaultsort)
                articleText = ExplicitCategorySortkeys(articleText, defaultsortKey);
            }

            noChange = (testText == articleText);
            return articleText;
        }

        /// <summary>
        /// Removes any explicit keys that are case insensitively the same as the default sort OR entirely match the start of the defaultsort (To help tidy up on pages that already have defaultsort)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="defaultsortKey"></param>
        /// <returns>The article text.</returns>
        private static string ExplicitCategorySortkeys(string articleText, string defaultsortKey)
        {
            foreach (Match m in WikiRegexes.Category.Matches(articleText))
            {
                string explicitKey = m.Groups[2].Value;
                if (explicitKey.Length == 0)
                    continue;

                if (string.Compare(explicitKey, defaultsortKey, StringComparison.OrdinalIgnoreCase) == 0
                    || defaultsortKey.StartsWith(explicitKey))
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
        /// <param name="articleAboutAPerson">Whether the article is about a person</param>
        /// <returns>The article text possibly using defaultsort.</returns>
        private static string DefaultsortTitlesWithDiacritics(string articleText, string articleTitle, int matches, bool articleAboutAPerson)
        {
            // need some categories and no defaultsort, and a sortkey not the same as the article title
            if (((Tools.FixupDefaultSort(articleTitle) != articleTitle && !articleAboutAPerson) ||
                 (Tools.MakeHumanCatKey(articleTitle) != articleTitle && articleAboutAPerson))
                && matches > 0 && !WikiRegexes.Defaultsort.IsMatch(articleText))
            {
                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Human_DEFAULTSORT
                // if article is about a person, attempt to add a surname, forenames sort key rather than the tidied article title
                string sortkey = articleAboutAPerson
                              ? Tools.MakeHumanCatKey(articleTitle)
                              : Tools.FixupDefaultSort(articleTitle);

                articleText = articleText + "\r\n{{DEFAULTSORT:" + sortkey + "}}";

                return (ExplicitCategorySortkeys(articleText, sortkey));
            }
            return articleText;
        }

        private static readonly Regex InUniverse = new Regex(@"{{[Ii]n-universe", RegexOptions.Compiled);
        private static readonly Regex CategoryCharacters = new Regex(@"\[\[Category:[^\[\]]*?[Cc]haracters", RegexOptions.Compiled);
        private static readonly Regex SeeAlsoOrMain = new Regex(@"{{(?:[Ss]ee\salso|[Mm]ain)\b", RegexOptions.Compiled);
        private static readonly Regex InfoboxFraternity = new Regex(@"{{\s*[Ii]nfobox[\s_]+[Ff]raternity", RegexOptions.Compiled);
        private static readonly Regex BoldedLink = new Regex(@"'''.*?\[\[[^\[\]]+\]\].*?'''", RegexOptions.Compiled);
        private static readonly Regex RefImprove = new Regex(@"{{\s*[Rr]efimproveBLP\b", RegexOptions.Compiled);

        /// <summary>
        /// determines whether the article is about a person by looking for persondata/birth death categories, bio stub etc. for en wiki only
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        [Obsolete]
        public static bool IsArticleAboutAPerson(string articleText)
        {
            return IsArticleAboutAPerson(articleText, "", false);
        }

                /// <summary>
        /// determines whether the article is about a person by looking for persondata/birth death categories, bio stub etc. for en wiki only
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <returns></returns>
        public static bool IsArticleAboutAPerson(string articleText, string articleTitle)
        {
            return IsArticleAboutAPerson(articleText, articleTitle, false);
        }

        /// <summary>
        /// determines whether the article is about a person by looking for persondata/birth death categories, bio stub etc. for en wiki only
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <returns></returns>
        public static bool IsArticleAboutAPerson(string articleText, string articleTitle, bool parseTalkPage)
        {
            if (Variables.LangCode != "en"
                    || articleText.Contains(@"[[Category:Multiple people]]")
                    || articleText.Contains(@"[[Category:Married couples")
                    || articleText.Contains(@"[[Category:Fictional")
                    || articleText.Contains(@"[[fictional character")
                    || InUniverse.IsMatch(articleText)
                    || articleText.Contains(@"[[Category:Presidencies")
                    || articleText.Contains(@"[[Category:Military careers")
                    || CategoryCharacters.IsMatch(articleText))
                return false;

            string zerothSection = WikiRegexes.ZerothSection.Match(articleText).Value;

            // not about a person if it's not the principle article on the subject
            if (SeeAlsoOrMain.IsMatch(zerothSection))
                return false;

            // TODO a workaround for abuse of {{birth date and age}} template by many fraternity articles e.g. [[Zeta Phi Beta]]
            if (InfoboxFraternity.IsMatch(articleText))
                return false;

            int dateBirthAndAgeCount = WikiRegexes.DateBirthAndAge.Matches(articleText).Count;
            int dateDeathAndAgeCount = WikiRegexes.DeathDate.Matches(articleText).Count;

            if (dateBirthAndAgeCount > 1 || dateDeathAndAgeCount > 1)
                return false;

            if (WikiRegexes.Lifetime.IsMatch(articleText)
                    || WikiRegexes.Persondata.Matches(articleText).Count == 1
                    || articleText.Contains(@"-bio-stub}}")
                    || articleText.Contains(@"[[Category:Living people"))
                return true;

            // articles with bold linking to another article may be linking to the main article on the person the article is about
            // e.g. '''military career of [[Napoleon Bonaparte]]'''
            if (BoldedLink.IsMatch(WikiRegexes.Template.Replace(zerothSection, "")))
                return false;

            if (dateBirthAndAgeCount == 1 || dateDeathAndAgeCount == 1)
                return true;

            return WikiRegexes.DeathsOrLivingCategory.IsMatch(articleText)
                    || WikiRegexes.LivingPeopleRegex2.IsMatch(articleText)
                    || WikiRegexes.BirthsCategory.IsMatch(articleText)
                    || WikiRegexes.BLPSources.IsMatch(articleText)
                    || RefImprove.IsMatch(articleText)
                    || (!string.IsNullOrEmpty(articleTitle) && parseTalkPage &&
                        Tools.GetArticleText(Variables.Namespaces[Namespace.Talk] + articleTitle, true).Contains(@"{{WPBiography"));
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
            return DefaultsortTitlesWithDiacritics(articleText, articleTitle, matches, false);
        }

        /// <summary>
        /// Adds [[Category:Living people]] to articles with a [[Category:XXXX births]] and no living people/deaths category, taking sortkey from births category if present
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="noChange"></param>
        /// <returns></returns>
        public static string LivingPeople(string articleText, out bool noChange)
        {
            string newText = LivingPeople(articleText);

            noChange = (newText == articleText);

            return newText;
        }

        private static readonly Regex ThreeOrMoreDigits = new Regex(@"\d{3,}", RegexOptions.Compiled);
        private static readonly Regex BirthsSortKey = new Regex(@"\|.*?\]\]", RegexOptions.Compiled);
        /// <summary>
        /// Adds [[Category:Living people]] to articles with a [[Category:XXXX births]] and no living people/deaths category, taking sortkey from births category if present
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The updated article text.</returns>
        public static string LivingPeople(string articleText)
        {
            // don't add living people category if already dead, or thought to be dead, or there's a lifetime template
            if (WikiRegexes.DeathsOrLivingCategory.IsMatch(articleText) || WikiRegexes.LivingPeopleRegex2.IsMatch(articleText) ||
                BornDeathRegex.IsMatch(articleText) || DiedDateRegex.IsMatch(articleText) || WikiRegexes.Lifetime.IsMatch(articleText))
                return articleText;

            Match m = WikiRegexes.BirthsCategory.Match(articleText);

            // don't add living people category unless 'XXXX births' category is present
            if (!m.Success)
                return articleText;

            string birthCat = m.Value;
            int birthYear = 0;

            string byear = m.Groups[1].Value;

            if (ThreeOrMoreDigits.IsMatch(byear))
                birthYear = int.Parse(byear);

            // per [[:Category:Living people]], don't apply if born > 121 years ago
            if (birthYear < (System.DateTime.Now.Year-121))
                return articleText;

            // use any sortkey from 'XXXX births' category
            string catKey = birthCat.Contains("|") ? BirthsSortKey.Match(birthCat).Value : "]]";

            return articleText + "[[Category:Living people" + catKey;
        }

        private static readonly Regex PersonYearOfBirth = new Regex(@"(?<='''.{0,100}?)\( *[Bb]orn[^\)\.;]{1,150}?(?<!.*(?:[Dd]ied|&[nm]dash;|—).*)([12]?\d{3}(?: BC)?)\b[^\)]{0,200}", RegexOptions.Compiled);
        private static readonly Regex PersonYearOfDeath = new Regex(@"(?<='''.{0,100}?)\([^\(\)]*?[Dd]ied[^\)\.;]+?([12]?\d{3}(?: BC)?)\b", RegexOptions.Compiled);
        private static readonly Regex PersonYearOfBirthAndDeath = new Regex(@"^.{0,100}'''\s*\([^\)\r\n]*?(?<![Dd]ied)\b([12]?\d{3})\b[^\)\r\n]*?(-|–|—|&[nm]dash;)[^\)\r\n]*?([12]?\d{3})\b[^\)]{0,200}", RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex UncertainWordings = new Regex(@"(?:\b(about|before|after|either|prior to|around|late|[Cc]irca|between|\d{3,4}(?:\]\])?/(?:\[\[)?\d{1,4}|or +(?:\[\[)?\d{3,})\b|\d{3} *\?|\bca?(?:'')?\.|\bca\b|\b(bef|abt)\.|~)", RegexOptions.Compiled);
        private static readonly Regex ReignedRuledUnsure = new Regex(@"(?:\?|[Rr](?:uled|eign(?:ed)?\b)|\br\.|(chr|fl(?:\]\])?)\.|\b[Ff]lourished\b)", RegexOptions.Compiled);

        /// <summary>
        /// Adds [[Category:XXXX births]], [[Category:XXXX deaths]] to articles about people where available, for en-wiki only
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="noChange"></param>
        /// <returns></returns>
        [Obsolete]
        public string FixPeopleCategories(string articleText, out bool noChange)
        {
            return FixPeopleCategories(articleText, "", false, out noChange);
        }

        /// <summary>
        /// Adds [[Category:XXXX births]], [[Category:XXXX deaths]] to articles about people where available, for en-wiki only
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="noChange"></param>
        /// <returns></returns>
        [Obsolete]
        public string FixPeopleCategories(string articleText, string articleTitle, out bool noChange)
        {
            string newText = FixPeopleCategories(articleText, articleTitle);

            noChange = (newText == articleText);

            return newText;
        }

        /// <summary>
        /// Adds [[Category:XXXX births]], [[Category:XXXX deaths]] to articles about people where available, for en-wiki only
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="noChange"></param>
        /// <returns></returns>
        public string FixPeopleCategories(string articleText, string articleTitle, bool parseTalkPage, out bool noChange)
        {
            string newText = FixPeopleCategories(articleText, articleTitle, parseTalkPage);

            noChange = (newText == articleText);

            return newText;
        }

        private static readonly Regex LongWikilink = new Regex(@"\[\[[^\[\]\|]{11,}(?:\|[^\[\]]+)?\]\]", RegexOptions.Compiled);
        private static readonly Regex YearPossiblyWithBC = new Regex(@"\d{3,4}(?![\ds])(?: BC)?", RegexOptions.Compiled);
        private static readonly Regex ThreeOrFourDigitNumber = new Regex(@"\d{3,4}", RegexOptions.Compiled);
        private static readonly Regex DiedOrBaptised = new Regex(@"(^.*?)((?:&[nm]dash;|—|–|;|[Dd](?:ied|\.)|baptised).*)", RegexOptions.Compiled);
        private static readonly Regex CircaTemplate = new Regex(@"{{[Cc]irca}}", RegexOptions.Compiled);

        [Obsolete]
        public static string FixPeopleCategories(string articleText)
        {
            return FixPeopleCategories(articleText, "");
        }

        /// <summary>
        /// Adds [[Category:XXXX births]], [[Category:XXXX deaths]] to articles about people where available, for en-wiki only
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <returns></returns>
        public static string FixPeopleCategories(string articleText, string articleTitle)
        {
            return FixPeopleCategories(articleText, articleTitle, false);
        }

        /// <summary>
        /// Adds [[Category:XXXX births]], [[Category:XXXX deaths]] to articles about people where available, for en-wiki only
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <returns></returns>
        public static string FixPeopleCategories(string articleText, string articleTitle, bool parseTalkPage)
        {
            if (Variables.LangCode != "en" || WikiRegexes.Lifetime.IsMatch(articleText))
                return YearOfBirthMissingCategory(articleText);

            // over 20 references or long and not DOB/DOD categorised at all yet: implausible
            if (WikiRegexes.Refs.Matches(articleText).Count > 20 || (articleText.Length > 15000 && !WikiRegexes.BirthsCategory.IsMatch(articleText)
                && !WikiRegexes.DeathsOrLivingCategory.IsMatch(articleText)))
                return YearOfBirthMissingCategory(articleText);

            string articleTextBefore = articleText;

            // get the zeroth section (text upto first heading)
            string zerothSection = WikiRegexes.ZerothSection.Match(articleText).Value;

            // remove references and long wikilinks (but allow an ISO date) that may contain false positives of birth/death date
            zerothSection = WikiRegexes.Refs.Replace(zerothSection, " ");
            zerothSection = LongWikilink.Replace(zerothSection, " ");

            string yearstring, yearFromInfoBox = "";

            string sort = GetCategorySort(articleText);

            bool alreadyUncertain = false;

            // scrape any infobox for birth year
            string fromInfoBox = GetInfoBoxFieldValue(zerothSection, @"(?:[Yy]earofbirth|Born|birth_?date)");

            if (fromInfoBox.Length > 0 && !UncertainWordings.IsMatch(fromInfoBox))
                yearFromInfoBox = YearPossiblyWithBC.Match(fromInfoBox).Value;

            // birth
            if (!WikiRegexes.BirthsCategory.IsMatch(articleText) && (PersonYearOfBirth.Matches(zerothSection).Count == 1 || WikiRegexes.DateBirthAndAge.IsMatch(zerothSection) || WikiRegexes.DeathDateAndAge.IsMatch(zerothSection)
                || ThreeOrFourDigitNumber.IsMatch(yearFromInfoBox)))
            {
                // look for '{{birth date...' template first
                yearstring = WikiRegexes.DateBirthAndAge.Match(articleText).Groups[1].Value;

                // look for '{{death date and age' template second
                if (String.IsNullOrEmpty(yearstring))
                    yearstring = WikiRegexes.DeathDateAndAge.Match(articleText).Groups[2].Value;

                // thirdly use yearFromInfoBox
                if (ThreeOrFourDigitNumber.IsMatch(yearFromInfoBox))
                    yearstring = yearFromInfoBox;

                // look for '(born xxxx)'
                if (String.IsNullOrEmpty(yearstring))
                {
                    Match m = PersonYearOfBirth.Match(zerothSection);

                    // remove part beyond dash or died
                    string birthpart = DiedOrBaptised.Replace(m.Value, "$1");

                    if (CircaTemplate.IsMatch(birthpart))
                        alreadyUncertain = true;

                    birthpart = WikiRegexes.TemplateMultiLine.Replace(birthpart, " ");

                    // check born info before any untemplated died info
                    if (!(m.Index > PersonYearOfDeath.Match(zerothSection).Index) || !PersonYearOfDeath.IsMatch(zerothSection))
                    {
                        // when there's only an approximate birth year, add the appropriate cat rather than the xxxx birth one
                        if (UncertainWordings.IsMatch(birthpart) || alreadyUncertain)
                        {
                            if (!CategoryMatch(articleText, YearOfBirthMissingLivingPeople))
                                articleText += "\r\n" + @"[[Category:Year of birth uncertain" + CatEnd(sort);
                        }
                        else // after removing dashes, birthpart must still contain year
                            if (!birthpart.Contains(@"?") && Regex.IsMatch(birthpart, @"\d{3,4}"))
                                yearstring = m.Groups[1].Value;
                    }
                }
                // per [[:Category:Living people]], don't apply birth category if born > 121 years ago 
                if (!string.IsNullOrEmpty(yearstring) && yearstring.Length > 2
                    && !(articleText.Contains(@"[[Category:Living people") && Convert.ToInt32(yearstring) < (System.DateTime.Now.Year - 121)))
                    articleText += "\r\n" + @"[[Category:" + yearstring + " births" + CatEnd(sort);
            }

            // death

            // scrape any infobox
            yearFromInfoBox = "";
            fromInfoBox = GetInfoBoxFieldValue(articleText, @"(?:[Yy]earofdeath|Died|death_?date)");

            if (fromInfoBox.Length > 0 && !UncertainWordings.IsMatch(fromInfoBox))
                yearFromInfoBox = YearPossiblyWithBC.Match(fromInfoBox).Value;

            if (!WikiRegexes.DeathsOrLivingCategory.IsMatch(articleText) && (PersonYearOfDeath.IsMatch(zerothSection) || WikiRegexes.DeathDate.IsMatch(zerothSection)
                || ThreeOrFourDigitNumber.IsMatch(yearFromInfoBox)))
            {
                // look for '{{death date...' template first
                yearstring = WikiRegexes.DeathDate.Match(articleText).Groups[1].Value;

                // secondly use yearFromInfoBox
                if (ThreeOrFourDigitNumber.IsMatch(yearFromInfoBox))
                    yearstring = yearFromInfoBox;

                // look for '(died xxxx)'
                if (string.IsNullOrEmpty(yearstring))
                {
                    Match m = PersonYearOfDeath.Match(zerothSection);

                    // check died info after any untemplated born info
                    if (m.Index >= PersonYearOfBirth.Match(zerothSection).Index || !PersonYearOfBirth.IsMatch(zerothSection))
                    {
                        if (!UncertainWordings.IsMatch(m.Value) && !m.Value.Contains(@"?"))
                            yearstring = m.Groups[1].Value;
                    }
                }

                if (!string.IsNullOrEmpty(yearstring) && yearstring.Length > 2)
                    articleText += "\r\n" + @"[[Category:" + yearstring + " deaths" + CatEnd(sort);
            }

            zerothSection = WikiRegexes.TemplateMultiLine.Replace(zerothSection, " ");
            // birth and death combined
            // if not fully categorised, check it
            if (PersonYearOfBirthAndDeath.IsMatch(zerothSection) && (!WikiRegexes.BirthsCategory.IsMatch(articleText) || !WikiRegexes.DeathsOrLivingCategory.IsMatch(articleText)))
            {
                Match m = PersonYearOfBirthAndDeath.Match(zerothSection);

                string birthyear = m.Groups[1].Value;
                int birthyearint = int.Parse(birthyear);

                string deathyear = m.Groups[3].Value;
                int deathyearint = int.Parse(deathyear);

                // logical valdiation of dates, and can't have any other born/died before this one
                if (birthyearint <= deathyearint && (deathyearint - birthyearint) <= 125)
                {
                    if (m.Index > PersonYearOfDeath.Match(zerothSection).Index && PersonYearOfDeath.IsMatch(zerothSection))
                        return YearOfBirthMissingCategory(articleText);
                    if (m.Index > PersonYearOfBirth.Match(zerothSection).Index && PersonYearOfBirth.IsMatch(zerothSection))
                        return YearOfBirthMissingCategory(articleText);

                    string birthpart = zerothSection.Substring(m.Index, m.Groups[2].Index - m.Index);

                    string deathpart = zerothSection.Substring(m.Groups[2].Index, (m.Value.Length + m.Index) - m.Groups[2].Index);

                    if (!WikiRegexes.BirthsCategory.IsMatch(articleText))
                    {
                        if (!UncertainWordings.IsMatch(birthpart) && !ReignedRuledUnsure.IsMatch(m.Value) && !Regex.IsMatch(birthpart, @"(?:[Dd](?:ied|\.)|baptised)"))
                            articleText += "\r\n" + @"[[Category:" + birthyear + @" births" + CatEnd(sort);
                        else
                            if (UncertainWordings.IsMatch(birthpart) && !CategoryMatch(articleText, YearOfBirthMissingLivingPeople) && !CategoryMatch(articleText, YearOfBirthUncertain))
                                articleText += "\r\n" + @"[[Category:Year of birth uncertain" + CatEnd(sort);
                    }

                    if (!UncertainWordings.IsMatch(deathpart) && !ReignedRuledUnsure.IsMatch(m.Value) && !Regex.IsMatch(deathpart, @"[Bb](?:orn|\.)") && !Regex.IsMatch(birthpart, @"[Dd](?:ied|\.)")
                        && (!WikiRegexes.DeathsOrLivingCategory.IsMatch(articleText) || CategoryMatch(articleText, YearofDeathMissing)))
                        articleText += "\r\n" + @"[[Category:" + deathyear + @" deaths" + CatEnd(sort);
                }
            }

            // TODO: check for lifetime and explicit XXX births/deaths categories and remove the categories if they co-incide

            // do this check last as IsArticleAboutAPerson can be relatively slow
            if (articleText != articleTextBefore && !IsArticleAboutAPerson(articleTextBefore, articleTitle, parseTalkPage))
                return YearOfBirthMissingCategory(articleTextBefore);

            return YearOfBirthMissingCategory(articleText);
        }

        private static string CatEnd(string sort)
        {
            return ((sort.Length > 3) ? "|" + sort : "") + "]]";
        }

        private const string YearOfBirthMissingLivingPeople = "Year of birth missing (living people)";

        private const string YearOfBirthMissing = "Year of birth missing";

        private const string YearOfBirthUncertain = "Year of birth uncertain";

        private const string YearofDeathMissing = "Year of death missing";

        private static readonly Regex Cat4YearBirths = new Regex(@"\[\[Category:\d{4} births(?:\s*\|[^\[\]]+)? *\]\]", RegexOptions.Compiled);
        private static readonly Regex Cat4YearDeaths = new Regex(@"\[\[Category:\d{4} deaths(?:\s*\|[^\[\]]+)? *\]\]", RegexOptions.Compiled);

        private static string YearOfBirthMissingCategory(string articleText)
        {
            if (Variables.LangCode != "en")
                return articleText;

            // if there is a 'year of birth missing' and a year of birth, remove the 'missing' category
            if (CategoryMatch(articleText, YearOfBirthMissingLivingPeople) && Cat4YearBirths.IsMatch(articleText))
                articleText = RemoveCategory(YearOfBirthMissingLivingPeople, articleText);
            else
                if (CategoryMatch(articleText, YearOfBirthMissing))
                {
                    if (Cat4YearBirths.IsMatch(articleText))
                        articleText = RemoveCategory(YearOfBirthMissing, articleText);
                }

            // if there's a 'year of birth missing' and a 'year of birth uncertain', remove the former
            if (CategoryMatch(articleText, YearOfBirthMissing) && CategoryMatch(articleText, YearOfBirthUncertain))
                articleText = RemoveCategory(YearOfBirthMissing, articleText);

            // if there's a year of death and a 'year of death missing', remove the latter
            if (CategoryMatch(articleText, YearofDeathMissing) && Cat4YearDeaths.IsMatch(articleText))
                articleText = RemoveCategory(YearofDeathMissing, articleText);

            return articleText;
        }

        private static readonly Regex InfoboxValue = new Regex(@"\s*\|[^{}\|=]+?\s*=\s*.*", RegexOptions.Compiled);
        /// <summary>
        /// Returns the value of the given field from the page's infobox, where available
        /// Returns a null string if the input article has no infobox, or the input field regex doesn't match on the infobox found
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="fieldRegex">Regular expression of field names, must not contain regex groups</param>
        /// <returns></returns>
        public static string GetInfoBoxFieldValue(string articleText, string fieldRegex)
        {
            string infoBox = WikiRegexes.InfoBox.Match(articleText).Value;
            string fieldValue;

            // clean out references and comments
            infoBox = WikiRegexes.Comments.Replace(infoBox, "");
            infoBox = WikiRegexes.Refs.Replace(infoBox, "");

            try // in case of parse exception on fieldRegex
            {
                fieldValue = Regex.Match(infoBox, @"^\s*\|?\s*" + fieldRegex + @"\s*=\s*(.*)", RegexOptions.Multiline).Groups[1].Value.Trim();
            }

            catch
            {
                return "";
            }

            if (fieldValue.Length > 0)
            {
                // handle multiple fields on same line
                if (InfoboxValue.IsMatch(fieldValue))
                {
                    // string fieldValueLocal = WikiRegexes.NestedTemplates.Replace(fieldValue, "");

                    // fieldValueLocal = InfoboxValue.Replace(fieldValueLocal, "");
                    return "";
                }

                return fieldValue;
            }

            return "";
        }

        /// <summary>
        /// Converts/subst'd some deprecated templates
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public static string Conversions(string articleText, out bool noChange)
        {
            string newText = Conversions(articleText);

            noChange = (newText == articleText);

            return newText;
        }

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
            if (articleText.Contains("[[zh-tw:"))
                articleText = articleText.Replace("[[zh-tw:", "[[zh:");
            if (articleText.Contains("[[nb:"))
                articleText = articleText.Replace("[[nb:", "[[no:");
            if (articleText.Contains("[[dk:"))
                articleText = articleText.Replace("[[dk:", "[[da:");

            if (articleText.Contains("{{msg:"))
                articleText = articleText.Replace("{{msg:", "{{");

            foreach (KeyValuePair<Regex, string> k in RegexConversion)
            {
                articleText = k.Key.Replace(articleText, k.Value);
            }

            // {{nofootnotes}} --> {{morefootnotes}}, if some <ref>...</ref> references in article, uses regex from WikiRegexes.Refs
            if (WikiRegexes.Refs.IsMatch(articleText))
                articleText = Regex.Replace(articleText, @"{{nofootnotes}}", "{{morefootnotes}}", RegexOptions.IgnoreCase);

            return articleText;
        }

        private static readonly Regex TemplateParameter2 = new Regex(" \\{\\{\\{2\\|\\}\\}\\}", RegexOptions.Compiled);
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
            if (userTalkTemplatesRegex == null)
                return talkPageText;

            talkPageText = talkPageText.Replace("{{{subst", "REPLACE_THIS_TEXT");
            Dictionary<Regex, string> regexes = new Dictionary<Regex, string> { { userTalkTemplatesRegex, "{{subst:$2}}" } };

            talkPageText = Tools.ExpandTemplate(talkPageText, talkPageTitle, regexes, true);

            talkPageText = TemplateParameter2.Replace(talkPageText, "");
            return talkPageText.Replace("REPLACE_THIS_TEXT", "{{{subst");
        }

        //Covered by TaggerTests
        /// <summary>
        /// If necessary, adds/removes wikify or stub tag
        /// </summary>
        public string Tagger(string articleText, string articleTitle, out bool noChange, ref string summary)
        {
            string newText = Tagger(articleText, articleTitle, ref summary);
            newText = TagUpdater(newText);

            noChange = (newText == articleText);

            return newText;
        }

        private static readonly CategoriesOnPageNoHiddenListProvider CategoryProv = new CategoriesOnPageNoHiddenListProvider();

        //TODO:Needs re-write
        /// <summary>
        /// If necessary, adds/removes wikify or stub tag
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article title.</param>
        /// <param name="summary"></param>
        /// <returns>The tagged article.</returns>
        public string Tagger(string articleText, string articleTitle, ref string summary)
        {
            // don't tag redirects/outside article namespace/no tagging changes
            if (Tools.IsRedirect(articleText) || !Namespace.IsMainSpace(articleTitle))
                return articleText;

            string commentsStripped = WikiRegexes.Comments.Replace(articleText, "");
            Sorter.Interwikis(ref commentsStripped);

            // bulleted or indented text should weigh less than simple text.
            // for example, actor stubs may contain large filmographies
            string crapStripped = WikiRegexes.BulletedText.Replace(commentsStripped, "");
            int words = (Tools.WordCount(commentsStripped) + Tools.WordCount(crapStripped)) / 2;

            // on en wiki, remove expand template when a stub template exists
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_.7B.7Bexpand.7D.7D_when_a_stub_template_exists
            if (Variables.LangCode == "en" && WikiRegexes.Stub.IsMatch(commentsStripped) && WikiRegexes.Expand.IsMatch(commentsStripped))
            {
                articleText = WikiRegexes.Expand.Replace(articleText, "");

                summary += ", removed expand tag";
            }

            // remove stub tags from long articles
            if ((words > StubMaxWordCount) && WikiRegexes.Stub.IsMatch(commentsStripped))
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
            if (words > 6 && totalCategories == 0
                && !WikiRegexes.Uncat.IsMatch(articleText) && Variables.LangCode != "nl")
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
            else if (totalCategories > 0
                     && WikiRegexes.Uncat.IsMatch(articleText))
            {
                articleText = WikiRegexes.Uncat.Replace(articleText, "");
                summary += ", removed uncategorised tag";
            }

            if (commentsStripped.Length <= 300 && !WikiRegexes.Stub.IsMatch(commentsStripped))
            {
                // add stub tag
                articleText = articleText + "\r\n\r\n\r\n{{stub}}";
                summary += ", added stub tag";
            }

            if (linkCount == 0 && !WikiRegexes.DeadEnd.IsMatch(articleText))
            {
                // add dead-end tag
                articleText = "{{deadend|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + articleText;
                summary += ", added [[:Category:Dead-end pages|deadend]] tag";
            }
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#.7B.7BDeadend.7D.7D_gets_removed_from_categorized_pages
            // don't include categories as 'links'
            else if ((linkCount - totalCategories) > 0 && WikiRegexes.DeadEnd.IsMatch(articleText))
            {
                articleText = WikiRegexes.DeadEnd.Replace(articleText, "");
                summary += ", removed deadend tag";
            }

            if (linkCount < 3 && ((linkCount / length) < 0.0025) && !WikiRegexes.Wikify.IsMatch(articleText))
            {
                // add wikify tag
                articleText = "{{Wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + articleText;
                summary += ", added [[:Category:Articles that need to be wikified|wikify]] tag";
            }
            else if (linkCount > 3 && ((linkCount / length) > 0.0025) &&
                     WikiRegexes.Wikify.IsMatch(articleText))
            {
                articleText = WikiRegexes.Wikify.Replace(articleText, "");
                summary += ", removed wikify tag";
            }

            return TagOrphans(articleText, articleTitle, ref summary);
        }

        private static readonly WhatLinksHereExcludingPageRedirectsListProvider WlhProv = new WhatLinksHereExcludingPageRedirectsListProvider(MinIncomingLinksToBeConsideredAnOrphan);

        private const int MinIncomingLinksToBeConsideredAnOrphan = 3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="summary"></param>
        /// <returns></returns>
        private string TagOrphans(string articleText, string articleTitle, ref string summary)
        {
            // check if not orphaned
            bool orphaned;
            if (Globals.UnitTestMode)
            {
                orphaned = Globals.UnitTestBoolValue;
            }
            else
            {
                try
                {
                    orphaned = (WlhProv.MakeList(Namespace.Article, articleTitle).Count < MinIncomingLinksToBeConsideredAnOrphan);
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
            if (orphaned && !WikiRegexes.Orphan.IsMatch(articleText) && !WikiRegexes.OrphanArticleIssues.IsMatch(articleText))
            {
                articleText = "{{orphan|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + articleText;
                summary += ", added [[:Category:Orphaned articles|orphan]] tag";
            }
            else if (!orphaned && WikiRegexes.Orphan.IsMatch(articleText))
            {
                articleText = WikiRegexes.Orphan.Replace(articleText, "");
                summary += ", removed orphan tag";
            }
            return articleText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
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
            return Variables.SectStubRegex.IsMatch(m.Value) ? m.Value : "";
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
                !Regex.IsMatch(articleText,
                             @"\{\{(nobots|bots\|(allow=none|deny=(?!none).*(" + user.Normalize() +
                             @"|awb|all).*|optout=all))\}\}", RegexOptions.IgnoreCase);
        }

        private static readonly Regex DuplicatePipedLinks = new Regex(@"\[\[([^\]\|]+)\|([^\]]*)\]\](.*[.\n]*)\[\[\1\|\2\]\]", RegexOptions.Compiled);
        private static readonly Regex DuplicateUnpipedLinks = new Regex(@"\[\[([^\]]+)\]\](.*[.\n]*)\[\[\1\]\]", RegexOptions.Compiled);

        /// <summary>
        /// Remove some of the duplicated wikilinks from the article text
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string RemoveDuplicateWikiLinks(string articleText)
        {
            articleText = DuplicatePipedLinks.Replace(articleText, "[[$1|$2]]$3$2");
            return DuplicateUnpipedLinks.Replace(articleText, "[[$1]]$2$1");
        }

        static readonly Regex ExtToIn = new Regex(@"(?<![*#:;]{2})\[http://([a-z0-9\-]{3})\.(?:(wikt)ionary|wiki(n)ews|wiki(b)ooks|wiki(q)uote|wiki(s)ource|wiki(v)ersity|(w)ikipedia)\.(?:com|net|org)/wiki/([^][{|}\s""]*) +([^\n\]]+)\]", RegexOptions.Compiled);
        static readonly Regex ExtToIn2 = new Regex(@"(?<![*#:;]{2})\[http://(?:(m)eta|(commons)|(incubator)|(quality))\.wikimedia\.(?:com|net|org)/wiki/([^][{|}\s""]*) +([^\n\]]+)\]", RegexOptions.Compiled);
        static readonly Regex ExtToIn3 = new Regex(@"(?<![*#:;]{2})\[http://([a-z0-9\-]+)\.wikia\.(?:com|net|org)/wiki/([^][{|}\s""]+) +([^\n\]]+)\]", RegexOptions.Compiled);

        // Covered by UtilityFunctionTests.ExternalURLToInternalLink(), incomplete
        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string ExternalURLToInternalLink(string articleText)
        {
            articleText = ExtToIn.Replace(articleText, "[[$2$3$4$5$6$7:$1:$8|$9]]");
            articleText = ExtToIn2.Replace(articleText, "[[$1$2$3$4:$5|$6]]");
            return ExtToIn3.Replace(articleText, "[[wikia:$1:$2|$3]]");
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
        /// Checks if the article has an InfoBox (en wiki)
        /// </summary>
        public static bool HasInfobox(string articleText)
        {
            if (Variables.LangCode != "en")
                return false;

            articleText = WikiRegexes.Nowiki.Replace(articleText, "");
            articleText = WikiRegexes.Comments.Replace(articleText, "");

            return WikiRegexes.InfoBox.IsMatch(articleText);
        }

        /// <summary>
        /// Check if article has an 'inusetag'
        /// </summary>
        public static bool IsInUse(string articleText)
        {
            return (Variables.LangCode != "en")
                       ? false
                       : WikiRegexes.InUse.IsMatch(WikiRegexes.Comments.Replace(articleText, ""));
        }

        /// <summary>
        /// Check if the article contains a sic template or bracketed wording, indicating the presence of a deliberate typo
        /// </summary>
        public static bool HasSicTag(string articleText)
        {
            return WikiRegexes.SicTag.IsMatch(articleText);
        }

        /// <summary>
        /// Returns whether the input article text contains any {{dead link}} templates, ignoring comments
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static bool HasDeadLinks(string articleText)
        {
            articleText = WikiRegexes.Comments.Replace(articleText, "");

            return WikiRegexes.DeadLink.IsMatch(articleText);
        }

        /// <summary>
        /// Check if the article contains a {{nofootnotes}} or {{morefootnotes}} template but has 5+ &lt;ref>...&lt;/ref> references
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
            if (Variables.LangCode != "en")
                return false;

            return !WikiRegexes.ReferencesTemplate.IsMatch(articleText) && Regex.IsMatch(articleText, WikiRegexes.ReferenceEndGR);
        }

        /// <summary>
        /// Check if the article contains a &lt;ref>...&lt;/ref> reference after the {{reflist}} to show them
        /// </summary>
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.28Yet.29_more_reference_related_changes.
        public static bool HasRefAfterReflist(string articleText)
        {
            articleText = WikiRegexes.Comments.Replace(articleText, "");
            return (WikiRegexes.RefAfterReflist.IsMatch(articleText) &&
                    WikiRegexes.ReferencesTemplate.Matches(articleText).Count == 1);
        }

        /// <summary>
        /// Returns true if the article contains bare external links in the references section (just the URL link on a line with no description/name)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Format_references
        public static bool HasBareReferences(string articleText)
        {
            int referencesIndex = WikiRegexes.ReferencesRegex.Match(articleText).Index;

            if (referencesIndex < 2)
                return false;

            int externalLinksIndex =
                WikiRegexes.ExternalLinksRegex.Match(articleText).Index;

            // get the references section: to external links or end of article, whichever is earlier
            string refsArea = externalLinksIndex > referencesIndex
                           ? articleText.Substring(referencesIndex, (externalLinksIndex - referencesIndex))
                           : articleText.Substring(referencesIndex);

            return (WikiRegexes.BareExternalLink.IsMatch(refsArea));
        }

        #endregion
    }
}
