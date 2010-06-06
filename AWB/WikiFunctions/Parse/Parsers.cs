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
using WikiFunctions.Lists.Providers;

namespace WikiFunctions.Parse
{
    //TODO:Make Regexes Compiled as required
    //TODO:Move Regexes to WikiRegexes as required
    //TODO:Split Parser code into separate files (for manageability), or even into separate classes
    //TODO:Move regexes declared in method bodies (if not dynamic based on article title, etc), into class body

    /// <summary>
    /// Provides functions for editing wiki text, such as formatting and re-categorisation.
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

            RegexTagger.Add(new Regex(@"\{\{\s*(?:template:)?\s*(?:wikify(?:-date)?|wfy|wiki)(\s*\|\s*section)?\s*\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Wikify$1|" + WikiRegexes.DateYearMonthParameter + @"}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Clean( ?up)?|CU|Tidy)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Cleanup|" + WikiRegexes.DateYearMonthParameter + @"}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Orphan)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Orphan|" + WikiRegexes.DateYearMonthParameter + @"}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Uncategori[sz]ed|Uncat|Classify|Category needed|Catneeded|categori[zs]e|nocats?)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Uncategorized|" + WikiRegexes.DateYearMonthParameter + @"}}");

            RegexTagger.Add(new Regex(@"\{\{(template:)?(Unreferenced(sect)?|add references|cite[ -]sources?|cleanup-sources?|needs? references|no sources|no references?|not referenced|references|unref|unsourced)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Unreferenced|" + WikiRegexes.DateYearMonthParameter + @"}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Trivia2?|Too ?much ?trivia|Trivia section|Cleanup-trivia)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Trivia|" + WikiRegexes.DateYearMonthParameter + @"}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(deadend|DEP)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Deadend|" + WikiRegexes.DateYearMonthParameter + @"}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(copyedit|g(rammar )?check|copy-edit|cleanup-copyedit|cleanup-english)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{copyedit|" + WikiRegexes.DateYearMonthParameter + @"}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(sources|refimprove|not verified)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Refimprove|" + WikiRegexes.DateYearMonthParameter + @"}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Uncategori[zs]edstub|uncatstub)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Uncategorizedstub|" + WikiRegexes.DateYearMonthParameter + @"}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Expand)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Expand|" + WikiRegexes.DateYearMonthParameter + @"}}");
            RegexTagger.Add(new Regex(@"\{\{(?:\s*[Tt]emplate:)?(\s*(?:[Cc]n|[Ff]act|[Pp]roveit|[Cc]iteneeded|[Uu]ncited|[Cc]itation needed)\s*(?:\|[^{}]+(?<!\|\s*date\s*=[^{}]+))?)\}\}", RegexOptions.Compiled), "{{$1|" + WikiRegexes.DateYearMonthParameter + @"}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(COI|Conflict of interest|Selfpromotion)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{COI|" + WikiRegexes.DateYearMonthParameter + @"}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?(Intro( |-)?missing|Nointro(duction)?|Lead missing|No ?lead|Missingintro|Opening|No-intro|Leadsection|No lead section)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Intro missing|" + WikiRegexes.DateYearMonthParameter + @"}}");
            RegexTagger.Add(new Regex(@"\{\{(template:)?([Pp]rimary ?[Ss]ources?|[Rr]eliable ?sources)\}\}", RegexOptions.Compiled), "{{Primary sources|" + WikiRegexes.DateYearMonthParameter + @"}}");

            RegexConversion.Add(new Regex(@"\{\{(?:Template:)?(Dab|Disamb|Disambiguation)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Disambig}}");
            RegexConversion.Add(new Regex(@"\{\{(?:Template:)?(Bio-dab|Hndisambig)", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Hndis");

            RegexConversion.Add(new Regex(@"\{\{(?:Template:)?(Prettytable|Prettytable100)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{subst:Prettytable}}");
            RegexConversion.Add(new Regex(@"\{\{(?:[Tt]emplate:)?((?:BASE)?PAGENAMEE?\}\}|[Ll]ived\||[Bb]io-cats\|)", RegexOptions.Compiled), "{{subst:$1");

            // clean 'do-attempt =July 2006|att=April 2008' to 'do attempt = April 2008'
            RegexConversion.Add(new Regex(@"({{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues\s*(?:\|[^{}]*|\|)\s*[Dd]o-attempt\s*=\s*)[^{}\|]+\|\s*att\s*=\s*([^{}\|]+)(?=\||}})", RegexOptions.Compiled), "$1$2");

            // clean "Copyedit for=grammar|date=April 2009"to "Copyedit=April 2009"
            RegexConversion.Add(new Regex(@"({{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues\s*(?:\|[^{}]*|\|)\s*[Cc]opyedit\s*)for\s*=\s*[^{}\|]+\|\s*date(\s*=[^{}\|]+)(?=\||}})", RegexOptions.Compiled), "$1$2");

            // could be multiple to date per template so loop
            for (int a = 0; a < 5; a++)
            {
                // add date to any undated tags within {{Article issues}}
                RegexConversion.Add(new Regex(@"({{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues\s*(?:\|[^{}]*|\|)\s*)(?![Ee]xpert)" + WikiRegexes.ArticleIssuesTemplatesString + @"\s*(\||}})", RegexOptions.Compiled), "$1$2={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}$3");

                // clean any 'date' word within {{Article issues}} (but not 'update' or 'out of date' fields), place after the date adding rule above
                RegexConversion.Add(new Regex(@"(?<={{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues\s*(?:\|[^{}]*?)?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}[^{}]*?){0,4}\|[^{}\|]{3,}?)\b(?i)date(?<!.*out of.*)", RegexOptions.Compiled), "");
            }

            // articleissues with one issue -> single issue tag (e.g. {{articleissues|cleanup=January 2008}} to {{cleanup|date=January 2008}} etc.)
            RegexConversion.Add(new Regex(@"\{\{(?:[Aa]rticle|[Mm]ultiple) ?issues\s*\|\s*([^\|{}=]{3,}?)\s*(=\s*\w{3,10}\s+20\d\d)\s*\}\}", RegexOptions.Compiled), "{{$1|date$2}}");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_empty_.7B.7BArticle_issues.7D.7D
            // articleissues with no issues -> remove tag
            RegexConversion.Add(new Regex(@"\{\{(?:[Aa]rticle|[Mm]ultiple) ?issues(?:\s*\|\s*(?:section|article)\s*=\s*[Yy])?\s*\}\}", RegexOptions.Compiled), "");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.7B.7Bcommons.7CCategory:XXX.7D.7D_.3E_.7B.7Bcommonscat.7CXXX.7D.7D
            RegexConversion.Add(new Regex(@"\{\{[Cc]ommons\|\s*[Cc]ategory:\s*([^{}]+?)\s*\}\}", RegexOptions.Compiled), @"{{Commons category|$1}}");

            //http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Commons_category
            RegexConversion.Add(new Regex(@"(?<={{[Cc]ommons cat(?:egory)?\|\s*)([^{}\|]+?)\s*\|\s*\1\s*}}", RegexOptions.Compiled), @"$1}}");

            // tidy up || or |}} (maybe with whitespace between) within templates that don't use null parameters
            RegexConversion.Add(new Regex(@"(?!{{[Cc]ite wikisource)(\{\{\s*(?:[Cc]it[ae]|[Aa]rticle ?issues)[^{}]*)\|\s*(\}\}|\|)", RegexOptions.Compiled), "$1$2");

            // remove duplicate / populated and null fields in cite/article issues templates
            RegexConversion.Add(new Regex(@"({{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues[^{}]*\|\s*)(\w+)\s*=\s*([^\|}{]+?)\s*\|((?:[^{}]*?\|)?\s*)\2(\s*=\s*)\3(\s*(\||\}\}))", RegexOptions.Compiled), "$1$4$2$5$3$6"); // duplicate field remover for cite templates
            RegexConversion.Add(new Regex(@"(\{\{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues[^{}]*\|\s*)(\w+)(\s*=\s*[^\|}{]+(?:\|[^{}]+?)?)\|\s*\2\s*=\s*(\||\}\})", RegexOptions.Compiled), "$1$2$3$4"); // 'field=populated | field=null' drop field=null
            RegexConversion.Add(new Regex(@"(\{\{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues[^{}]*\|\s*)(\w+)\s*=\s*\|\s*((?:[^{}]+?\|)?\s*\2\s*=\s*[^\|}{\s])", RegexOptions.Compiled), "$1$3"); // 'field=null | field=populated' drop field=null

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#AWB_is_still_using_.7B.7BArticleissues.7D.7D_instead_of_.7B.7BArticle_issues.7D.7D
            // replace any {{articleissues}} with {{article issues}}
            RegexConversion.Add(new Regex(@"(?<={{\s*)(?:[Aa]rticle|[Mm]ultiple) ?(?=issues.*}})", RegexOptions.Compiled), "multiple ");

            // http://en.wikipedia.org/wiki/Template_talk:Citation_needed#Requested_move
            RegexConversion.Add(new Regex(@"{{\s*(?:[Cc]n|[Ff]act|[Pp]roveit|[Cc]iteneeded|[Uu]ncited)(?=\s*[\|}])", RegexOptions.Compiled), @"{{Citation needed");
            
            RegexConversion.Add(new Regex(@"({{\s*[Cc]itation needed\s*\|)\s*(?:[Dd]ate:)?([A-Z][a-z]+ 20\d\d)\s*\|\s*(date\s*=\s*\2\s*}})", RegexOptions.Compiled | RegexOptions.IgnoreCase), @"$1$3");
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
            get { return metaDataSorter ?? (metaDataSorter = new MetaDataSorter()); }
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

        public string HideTextImages(string articleText)
        {
            return HiderHideExtLinksImages.Hide(articleText);
        }

        public string AddBackTextImages(string articleText)
        {
            return HiderHideExtLinksImages.AddBack(articleText);
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
            return SortMetaData(articleText, articleTitle, true);
        }

        /// <summary>
        /// Re-organises the Person Data, stub/disambig templates, categories and interwikis
        /// except when a mainspace article has some 'includeonly' tags etc.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article title.</param>
        /// <param name="fixExcessWhitespace">Whether to request optional excess whitespace to be fixed</param>
        /// <returns>The re-organised text.</returns>
        public string SortMetaData(string articleText, string articleTitle, bool fixOptionalWhitespace)
        {
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Substituted_templates
            // if article contains some substituted template stuff, sorting the data may mess it up (further)
            if (Namespace.IsMainSpace(articleTitle) && NoIncludeIncludeOnlyProgrammingElement(articleText))
                return articleText;

            return (Variables.Project <= ProjectEnum.species) ? Sorter.Sort(articleText, articleTitle, fixOptionalWhitespace) : articleText;
        }

        private static readonly Regex ApostropheInDecades = new Regex(@"(?<=(?:the |later? |early |mid-|[12]\d\d0'?s and )(?:\[?\[?[12]\d\d0\]?\]?))['’]s(?=\]\])?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
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
        /// Combines multiple cleanup tags into {{multiple issues}} template, ensures parameters have correct case, removes date parameter where not needed
        /// only for English-language wikis
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string ArticleIssues(string articleText, string articleTitle)
        {
            if (Variables.LangCode != "en")
                return articleText;

            if (WikiRegexes.ArticleIssues.IsMatch(articleText))
            {
                string aiat = WikiRegexes.ArticleIssues.Match(articleText).Value;
                
                // unref to BLPunref for living person bio articles
                if(Tools.GetTemplateParameterValue(aiat, "unreferenced").Length > 0 && articleText.Contains(@"[[Category:Living people"))
                    articleText = articleText.Replace(aiat, Tools.RenameTemplateParameter(aiat, "unreferenced", "BLPunreferenced"));
                else if(Tools.GetTemplateParameterValue(aiat, "unref").Length > 0 && articleText.Contains(@"[[Category:Living people"))
                    articleText = articleText.Replace(aiat, Tools.RenameTemplateParameter(aiat, "unref", "BLPunreferenced"));
                
                articleText = MetaDataSorter.MoveMaintenanceTags(articleText);
            }

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
            string restOfArticle = articleText.Remove(0, zerothSection.Length);

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
            string ai = WikiRegexes.ArticleIssues.Match(zerothSection).Value;
            if (ai.Length > 0)
                zerothSection = zerothSection.Replace(ai, ai.Substring(0, ai.Length-2) + newTags + @"}}");
            
            else // add {{article issues}} to top of article, metaDataSorter will arrange correctly later
                zerothSection = @"{{Article issues" + newTags + "}}\r\n" + zerothSection;

            // Parsers.Conversions will add any missing dates and correct ...|wikify date=May 2008|...
            return (zerothSection + restOfArticle);
        }
        
        /// <summary>
        /// Performs some cleanup operations on dablinks
        /// Merges some for & about dablinks
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>The updated article text</returns>
        public static string Dablinks(string articleText)
        {
            if(Variables.LangCode != "en")
                return articleText;
            
            string oldArticleText = "";

            string zerothSection = WikiRegexes.ZerothSection.Match(articleText).Value;
            string restOfArticle = articleText.Remove(0, zerothSection.Length);
            articleText = zerothSection;
            
            // conversions
            
            // otheruses4 rename
            articleText = Tools.RenameTemplate(articleText, "otheruses4", "about");
            
            // "{{about|about x..." --> "{{about|x..."
            foreach(Match m in Tools.NestedTemplateRegex("about").Matches(articleText))
            {
                if(m.Groups[3].Value.TrimStart("| ".ToCharArray()).ToLower().StartsWith("about"))
                    articleText = articleText.Replace(m.Value, m.Groups[1].Value + m.Groups[2].Value + Regex.Replace(m.Groups[3].Value, @"^\|\s*[Aa]bout\s*", "|"));
            }

            // merging
            
            // multiple same about into one
            oldArticleText = "";
            while (oldArticleText != articleText)
            {
                oldArticleText = articleText;
                bool doneAboutMerge = false;
                foreach(Match m in Tools.NestedTemplateRegex("about").Matches(articleText))
                {
                    string firstarg = Tools.GetTemplateArgument(m.Value, 1);
                    
                    foreach(Match m2 in Tools.NestedTemplateRegex("about").Matches(articleText))
                    {
                        if(m2.Value == m.Value)
                            continue;
                        
                        // match when reason is the same, not matching on self
                        if(Tools.GetTemplateArgument(m2.Value, 1).Equals(firstarg))
                        {
                            // argument 2 length > 0
                            if(Tools.GetTemplateArgument(m.Value, 2).Length > 0 && Tools.GetTemplateArgument(m2.Value, 2).Length > 0)
                            {
                                articleText = articleText.Replace(m.Value, m.Value.TrimEnd('}') + @"|" + Tools.GetTemplateArgument(m2.Value, 2) + @"|" + Tools.GetTemplateArgument(m2.Value, 3) + @"}}");
                                doneAboutMerge = true;
                            }
                            
                            // argument 2 is null
                            if(Tools.GetTemplateArgument(m.Value, 2).Length == 0 && Tools.GetTemplateArgument(m2.Value, 2).Length == 0)
                            {
                                articleText = articleText.Replace(m.Value, m.Value.TrimEnd('}') + @"|and|" + Tools.GetTemplateArgument(m2.Value, 3) + @"}}");
                                doneAboutMerge = true;
                            }
                        }
                        // match when reason of one is null, the other not
                        else if(Tools.GetTemplateArgument(m2.Value, 1).Length == 0)
                        {
                            // argument 2 length > 0
                            if(Tools.GetTemplateArgument(m.Value, 2).Length > 0 && Tools.GetTemplateArgument(m2.Value, 2).Length > 0)
                            {
                                articleText = articleText.Replace(m.Value, m.Value.TrimEnd('}') + @"|" + Tools.GetTemplateArgument(m2.Value, 2) + @"|" + Tools.GetTemplateArgument(m2.Value, 3) + @"}}");
                                doneAboutMerge = true;
                            }
                        }
                        
                        if(doneAboutMerge)
                        {
                            articleText = articleText.Replace(m2.Value, "");
                            break;
                        }
                    }
                    if(doneAboutMerge)
                        break;
                }
            }
            
            // multiple for into about: rename a 2-argument for into an about with no reason value
            if(Tools.NestedTemplateRegex("for").Matches(articleText).Count > 1 && Tools.NestedTemplateRegex("about").Matches(articleText).Count == 0)
            {
                foreach(Match m in Tools.NestedTemplateRegex("for").Matches(articleText))
                {
                    if(Tools.GetTemplateArgument(m.Value, 3).Length == 0)
                    {
                        articleText = articleText.Replace(m.Value, Tools.RenameTemplate(m.Value, "about|"));
                        break;
                    }
                }
            }
            
            // for into existing about, when about has >=2 arguments
            if(Tools.NestedTemplateRegex("about").Matches(articleText).Count == 1 &&
               Tools.GetTemplateArgument(Tools.NestedTemplateRegex("about").Match(articleText).Value, 2).Length > 0)
            {
                foreach(Match m in Tools.NestedTemplateRegex("for").Matches(articleText))
                {
                    string About = Tools.NestedTemplateRegex("about").Match(articleText).Value;
                    string extra = "";
                    
                    // where about has 2 arguments need extra pipe
                    if(Tools.GetTemplateArgument(Tools.NestedTemplateRegex("about").Match(articleText).Value, 3).Length == 0
                       && Tools.GetTemplateArgument(Tools.NestedTemplateRegex("about").Match(articleText).Value, 4).Length == 0)
                        extra = @"|";
                    
                    // append {{for}} value to the {{about}}
                    if(Tools.GetTemplateArgument(m.Value, 3).Length == 0)
                        articleText = articleText.Replace(About, About.TrimEnd('}') +  extra + m.Groups[3].Value);
                    else // where for has 3 arguments need extra and
                        articleText = articleText.Replace(About, About.TrimEnd('}') +  extra + m.Groups[3].Value.Insert(m.Groups[3].Value.LastIndexOf('|')+1, "and|"));
                    
                    // remove the old {{for}}
                    articleText = articleText.Replace(m.Value, "");
                }
            }
            
            // non-mainspace links need escaping in {{about}}
            foreach(Match m in Tools.NestedTemplateRegex("about").Matches(articleText))
            {
                string aboutcall = m.Value;
                for(int a = 1; a <= Tools.GetTemplateArgumentCount(m.Value); a++)
                {                    
                    string arg = Tools.GetTemplateArgument(aboutcall, a);
                    if(arg.Length > 0 && Namespace.Determine(arg) != Namespace.Mainspace)
                        aboutcall = aboutcall.Replace(arg, @":" + arg);
                }
                
                if(!m.Value.Equals(aboutcall))
                    articleText = articleText.Replace(m.Value, aboutcall);
            }
            
            // multiple {{distinguish}} into one
            oldArticleText = "";
            while (oldArticleText != articleText)
            {
                oldArticleText = articleText;
                bool doneDistinguishMerge = false;
                foreach(Match m in Tools.NestedTemplateRegex("distinguish").Matches(articleText))
                {
                    foreach(Match m2 in Tools.NestedTemplateRegex("distinguish").Matches(articleText))
                    {
                        if(m2.Value.Equals(m.Value))
                            continue;
                        
                        articleText = articleText.Replace(m.Value, m.Value.TrimEnd('}') + m2.Groups[3].Value);
                        
                        doneDistinguishMerge = true;
                        articleText = articleText.Replace(m2.Value, "");
                        break;
                    }
                    
                    if(doneDistinguishMerge)
                        break;
                }
            }
            
            return(articleText + restOfArticle);
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
            if (!WikiRegexes.HeadingLevelTwo.IsMatch(articleTextLocal) && Namespace.IsMainSpace(articleTitle))
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
        private static readonly Regex CommaDates = new Regex(WikiRegexes.Months + @" ?, *([1-3]?\d) ?, ?((?:200|19\d)\d)\b");

        // fixes missing comma or space in American format dates
        private static readonly Regex NoCommaAmericanDates = new Regex(@"\b(" + WikiRegexes.MonthsNoGroup + @" *[1-3]?\d(?:–[1-3]?\d)?)\b *,?([12]\d{3})\b");
        private static readonly Regex NoSpaceAmericanDates = new Regex(@"\b" + WikiRegexes.Months + @"([1-3]?\d(?:–[1-3]?\d)?), *([12]\d{3})\b");

        // fix incorrect comma between month and year in Internaltional-format dates
        private static readonly Regex IncorrectCommaInternationalDates = new Regex(@"\b((?:[1-3]?\d) +" + WikiRegexes.MonthsNoGroup + @") *, *(1\d{3}|20\d{2})\b", RegexOptions.Compiled);

        // date ranges use an en-dash per [[WP:MOSDATE]]
        private static readonly Regex SameMonthInternationalDateRange = new Regex(@"\b([1-3]?\d) *- *([1-3]?\d +" + WikiRegexes.MonthsNoGroup + @")\b", RegexOptions.Compiled);
        private static readonly Regex SameMonthAmericanDateRange = new Regex(@"(" + WikiRegexes.MonthsNoGroup + @" *)([1-3]?\d) *- *([1-3]?\d)\b", RegexOptions.Compiled);

        // 13 July -28 July 2009 -> 13–28 July 2009
        // July 13 - July 28 2009 -> July 13–28, 2009
        private static readonly Regex LongFormatInternationalDateRange = new Regex(@"\b([1-3]?\d) +" + WikiRegexes.Months + @" *(?:-|–|&nbsp;) *([1-3]?\d) +\2,? *([12]\d{3})\b", RegexOptions.Compiled);
        private static readonly Regex LongFormatAmericanDateRange = new Regex(WikiRegexes.Months + @" +([1-3]?\d) +" + @" *(?:-|–|&nbsp;) *\1 +([1-3]?\d) *,? *([12]\d{3})\b", RegexOptions.Compiled);

        private static readonly Regex FullYearRange = new Regex(@"(?:[\(,=;\|]|\b(?:from|between|and|reigned|f?or)) *([12]\d{3}) *- *([12]\d{3}) *(?=\)|[,;\|]|and\b|\s*$)", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SpacedFullYearRange = new Regex(@"([12]\d{3})(?: +– *| *– +)([12]\d{3})", RegexOptions.Compiled);
        private static readonly Regex YearRangeShortenedCentury = new Regex(@"(?:[\(,=;]|\b(?:from|between|and|reigned)) *([12]\d{3}) *- *(\d{2}) *(?=\)|[,;]|and\b|\s*$)", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex YearRangeToPresent = new Regex(@"([12]\d{3}) *- *([Pp]resent)", RegexOptions.Compiled);
        // Covered by: LinkTests.FixDates()
        /// <summary>
        /// Fix date and decade formatting errors, and replace &lt;br&gt; and &lt;p&gt; HTML tags
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixDates(string articleText)
        {
            if (Variables.LangCode != "en")
                return articleText;

            articleText = HideTextImages(articleText);

            articleText = CommaDates.Replace(articleText, @"$1 $2, $3");
            articleText = IncorrectCommaInternationalDates.Replace(articleText, @"$1 $2");

            articleText = SameMonthInternationalDateRange.Replace(articleText, @"$1–$2");

            foreach (Match m in SameMonthAmericanDateRange.Matches(articleText))
            {
                int day1 = Convert.ToInt32(m.Groups[2].Value);
                int day2 = Convert.ToInt32(m.Groups[3].Value);

                if (day2 > day1)
                    articleText = articleText.Replace(m.Value, Regex.Replace(m.Value, @" *- *", @"–"));
            }

            articleText = LongFormatInternationalDateRange.Replace(articleText, @"$1–$3 $2 $4");
            articleText = LongFormatAmericanDateRange.Replace(articleText, @"$1 $2–$3, $4");

            // run this after the date range fixes
            articleText = NoCommaAmericanDates.Replace(articleText, @"$1, $2");
            articleText = NoSpaceAmericanDates.Replace(articleText, @"$1 $2, $3");

            articleText = SpacedFullYearRange.Replace(articleText, @"$1–$2");

            articleText = AddBackTextImages(articleText);

            // fixes bellow need full HideMore
            articleText = HideMoreText(articleText);

            articleText = YearRangeToPresent.Replace(articleText, @"$1–$2");

            // 1965–1968 fixes: only appy year range fix if two years are in order
            foreach (Match m in FullYearRange.Matches(articleText))
            {
                int year1 = Convert.ToInt32(m.Groups[1].Value);
                int year2 = Convert.ToInt32(m.Groups[2].Value);

                if (year2 > year1 && year2 - year1 <= 300)
                    articleText = articleText.Replace(m.Value, Regex.Replace(m.Value, @" *- *", @"–"));
            }

            // 1965–68 fixes
            foreach (Match m in YearRangeShortenedCentury.Matches(articleText))
            {
                int year1 = Convert.ToInt32(m.Groups[1].Value); // 1965
                int year2 = Convert.ToInt32(m.Groups[1].Value.Substring(0, 2) + m.Groups[2].Value); // 68 -> 19 || 68 -> 1968

                if (year2 > year1 && year2 - year1 <= 99)
                    articleText = articleText.Replace(m.Value, Regex.Replace(m.Value, @" *- *", @"–"));
            }

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
        
        private static readonly Regex UnlinkedFloruit = new Regex(@"\(\s*(?:[Ff]l)\.*\s*(\d\d)", RegexOptions.Compiled);

        //Covered by: LinkTests.FixLivingThingsRelatedDates()
        /// <summary>
        /// Replace b. and d. for born/died
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixLivingThingsRelatedDates(string articleText)
        {
            // replace first occurrence of unlinked floruit with linked version, zeroth section only
            if(UnlinkedFloruit.IsMatch(WikiRegexes.ZerothSection.Match(articleText).Value))
                articleText = UnlinkedFloruit.Replace(articleText, @"([[floruit|fl.]] $1", 1);
            
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
            // removed superscripted punctuation between refs
            articleText = WikiRegexes.SuperscriptedPunctuationBetweenRefs.Replace(articleText, "$1<ref");

            articleText = WikiRegexes.WhiteSpaceFactTag.Replace(articleText, "{{$1}}");

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

        private const string OutofOrderRefs = @"(<ref\s+name\s*=\s*(?:""|')?([^<>""=]+?)(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>)(\s*{{\s*rp\|[^{}]+}})?(\s*)(<ref\s+name\s*=\s*(?:""|')?([^<>""=]+?)(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>)(\s*{{\s*rp\|[^{}]+}})?";
        private static readonly Regex OutofOrderRefs1 = new Regex(@"(<ref>[^<>]+</ref>)(\s*)(<ref\s+name\s*=\s*(?:""|')?([^<>""=]+?)(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>)(\s*{{\s*[Rr]p\|[^{}]+}})?", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex OutofOrderRefs2 = new Regex(OutofOrderRefs, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        // regex below ensures a forced match on second and third of consecutive references
        private static readonly Regex OutofOrderRefs3 = new Regex(@"(?<=\s*(?:\/\s*|>[^<>]+</ref)>\s*(?:{{\s*rp\|[^{}]+}})?)" + OutofOrderRefs, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Reorders references so that they appear in numerical order, allows for use of {{rp}}, doesn't modify grouped references [[WP:REFGROUP]]
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string ReorderReferences(string articleText)
        {
            // do not reorder stuff in the <references>...</references> section
            int referencestags = WikiRegexes.ReferencesTemplate.Match(articleText).Index;
            if (referencestags <= 0)
                referencestags = articleText.Length;

            for (int i = 0; i < 5; i++) // allows for up to 5 consecutive references
            {
                string articleTextBefore = articleText;

                foreach (Match m in OutofOrderRefs1.Matches(articleText))
                {
                    string ref1 = m.Groups[1].Value;
                    int ref1Index = Regex.Match(articleText, @"(?si)<ref\s+name\s*=\s*(?:""|')?" + Regex.Escape(m.Groups[4].Value) + @"(?:""|')?\s*(?:\/\s*|>.+?</ref)>").Index;
                    int ref2Index = articleText.IndexOf(ref1);

                    if (ref1Index < ref2Index && ref2Index > 0 && ref1Index > 0 && m.Groups[3].Index < referencestags)
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
        
        private const string RefsPunctuation = @"([,\.:;])";
        private static readonly Regex RefsAfterPunctuationR = new Regex(RefsPunctuation + @" *" + WikiRegexes.Refs, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex RefsBeforePunctuationR = new Regex(WikiRegexes.Refs + @" *" + RefsPunctuation, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
       
        private static readonly Regex RefsAfterDupePunctuation = new Regex(RefsPunctuation + @"\1 *" + WikiRegexes.Refs, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        /// <summary>
        /// Puts &lt;ref&gt; references after punctuation per WP:REFPUNC when this is the majority style in the article
        /// Applies to en-wiki only
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>The updated article text</returns>
        public static string RefsAfterPunctuation(string articleText)
        {
            if(!Variables.LangCode.Equals("en"))
                return articleText;
            
            // check whether refs before punctuation or refs after punctuation is the dominant format
            int RAfter = RefsAfterPunctuationR.Matches(articleText).Count;
            int RBefore = RefsBeforePunctuationR.Matches(articleText).Count;
            
            // require >= 75% refs after punctution to convert the rest
            if(RAfter > RBefore && (RBefore == 0 || RAfter/RBefore > 3))
            {
                string articleTextlocal = "";
                while (!articleTextlocal.Equals(articleText))
                {
                    articleTextlocal = articleText;
                    articleText = RefsBeforePunctuationR.Replace(articleText, "$2$1");
                }
            }
            
            return RefsAfterDupePunctuation.Replace(articleText, "$1$2");
        }

        /// <summary>
        /// reorders references within the article text based on the input regular expression providing matches for references that are out of numerical order
        /// </summary>
        /// <param name="articleText">the wiki text of the article</param>
        /// <param name="outofOrderRegex">a regular expression representing two references that are out of numerical order</param>
        /// <param name="referencestagindex"></param>
        /// <returns>the modified article text</returns>
        private static string ReorderRefs(string articleText, Regex outofOrderRegex, int referencestagindex)
        {
            foreach (Match m in outofOrderRegex.Matches(articleText))
            {
                int ref1Index = Regex.Match(articleText, @"(?si)<ref\s+name\s*=\s*(?:""|')?" + Regex.Escape(m.Groups[2].Value) + @"(?:""|')?\s*(?:\/\s*|>.+?</ref)>").Index;
                int ref2Index = Regex.Match(articleText, @"(?si)<ref\s+name\s*=\s*(?:""|')?" + Regex.Escape(m.Groups[6].Value) + @"(?:""|')?\s*(?:\/\s*|>.+?</ref)>").Index;

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

        private static readonly Regex LongNamedReferences = new Regex(@"(<\s*ref\s+name\s*=\s*(?:""|')?([^<>=\r\n]+?)(?:""|')?\s*>\s*([^<>]{30,}?)\s*<\s*/\s*ref>)", RegexOptions.Compiled);

        // Covered by: DuplicateNamedReferencesTests()
        /// <summary>
        /// Where an unnamed reference is a duplicate of another named reference, set the unnamed one to use the named ref
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string DuplicateNamedReferences(string articleText)
        {
            Dictionary<string, string> NamedRefs = new Dictionary<string, string>();

            foreach (Match m in WikiRegexes.NamedReferences.Matches(articleText))
            {
                string refName = m.Groups[2].Value;
                string namedRefValue = m.Groups[3].Value;
                string name2 = "";

                if (!NamedRefs.ContainsKey(namedRefValue))
                    NamedRefs.Add(namedRefValue, refName);
                else
                {
                    // we've already seen this reference, can condense later ones
                    NamedRefs.TryGetValue(namedRefValue, out name2);

                    if (name2.Equals(refName) && namedRefValue.Length >= 25)
                    {
                        int reflistIndex = WikiRegexes.ReferencesTemplate.Match(articleText).Index;
                        
                        // don't condense refs in {{reflist...}}
                        if(reflistIndex > 0 && m.Index > reflistIndex)
                            continue;
                        
                        if(m.Index > articleText.Length)
                            continue;
                        
                        // duplicate citation fixer (both named): <ref name="Fred">(...)</ref>....<ref name="Fred">\2</ref> --> ..<ref name="Fred"/>, minimum 25 characters to avoid short refs
                        string texttomatch = articleText.Substring(0, m.Index);
                        string textaftermatch = articleText.Substring(m.Index);

                        articleText = texttomatch + textaftermatch.Replace(m.Value, @"<ref name=""" + refName + @"""/>");
                    }
                }

                // duplicate citation fixer (first named): <ref name="Fred">(...)</ref>....<ref>\2</ref> --> ..<ref name="Fred"/>
                // duplicate citation fixer (second named): <ref>(...)</ref>....<ref name="Fred">\2</ref> --> ..<ref name="Fred"/>
                foreach (Match m3 in Regex.Matches(articleText, @"<\s*ref\s*>\s*" + Regex.Escape(namedRefValue) + @"\s*<\s*/\s*ref>"))
                    articleText = articleText.Replace(m3.Value, @"<ref name=""" + refName + @"""/>");

            }

            return articleText;
        }

        private const string RefName = @"(?si)<\s*ref\s+name\s*=\s*(?:""|')?";
        private static readonly Regex UnnamedRef = new Regex(@"<\s*ref\s*>\s*([^<>]+)\s*<\s*/\s*ref>", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Checks for named references
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>Whether the text contains named references</returns>
        public static bool HasNamedReferences(string articleText)
        {
            return WikiRegexes.NamedReferences.IsMatch(WikiRegexes.Comments.Replace(articleText, ""));
        }

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
            /* On en-wiki AWB is asked not to add named references to an article if there are none currently, as some users feel
             * this is a change of citation style, so is against the [[WP:CITE]] "don't change established style" guidelines */
            if (Variables.LangCode == "en" && !HasNamedReferences(articleText))
                return articleText;

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
                if (refs.TryGetValue(hash, out list))
                {
                    haveRefsToFix = true;
                }
                else
                {
                    list = new List<Ref>();
                    refs[hash] = list;
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
                if (friendlyName.Length <= 3 || Regex.IsMatch(result.ToString(), RefName + Regex.Escape(friendlyName) + @"""\s*/?\s*>"))
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
        
        private static readonly Regex PageRef = new Regex(@"\s*(?:(?:[Pp]ages?|[Pp][pg]?[:\.]?)|^)\s*[XVI\d]", RegexOptions.Compiled);

        /// <summary>
        /// Corrects named references where the reference is the same but the reference name is different
        /// </summary>
        /// <param name="articleText">the wiki text of the page</param>
        /// <returns>the updated wiki text</returns>
        public static string SameRefDifferentName(string articleText)
        {
            // refs with same name, but one is very short, so just change to <ref name=foo/> notation
            articleText = SameNamedRefShortText(articleText);

            Dictionary<string, string> NamedRefs = new Dictionary<string, string>();

            foreach (Match m in WikiRegexes.NamedReferences.Matches(articleText))
            {
                string refname = m.Groups[2].Value;
                string refvalue = m.Groups[3].Value;

                string existingname = "";

                if (!NamedRefs.ContainsKey(refvalue))
                {
                    NamedRefs.Add(refvalue, refname);
                    continue;
                }

                NamedRefs.TryGetValue(refvalue, out existingname);

                // don't apply to ibid short ref
                if (existingname.Length > 0 && !existingname.Equals(refname) && !WikiRegexes.IbidOpCitation.IsMatch(refvalue))
                {
                    string newRefName = refname;
                    string oldRefName = existingname;

                    // use longest ref name as the one to keep
                    if ((existingname.Length > refname.Length && !existingname.Contains("autogenerated")
                         && !existingname.Contains("ReferenceA")) || (refname.Contains("autogenerated")
                                                                     || refname.Contains("ReferenceA")))
                    {
                        newRefName = existingname;
                        oldRefName = refname;
                    }

                    Regex a = new Regex(@"<\s*ref\s+name\s*=\s*(?:""|')?" + Regex.Escape(oldRefName) + @"(?:""|')?\s*(?=/\s*>|>\s*" + Regex.Escape(refvalue) + @"\s*</ref>)");

                    articleText = a.Replace(articleText, @"<ref name=""" + newRefName + @"""");
                }
            }

            return DuplicateNamedReferences(articleText);
        }

        /// <summary>
        /// refs with same name, but one is very short, so just change to &lt;ref name=foo/&gt; notation
        /// </summary>
        /// <param name="articleText">the wiki text of the page</param>
        /// <returns>the update wiki text</returns>
        private static string SameNamedRefShortText(string articleText)
        {
            foreach (Match m in LongNamedReferences.Matches(articleText))
            {
                string refname = m.Groups[2].Value;
                string refvalue = m.Groups[3].Value;

                Regex shortNamedReferences = new Regex(@"(<\s*ref\s+name\s*=\s*(?:""|')?(" + Regex.Escape(refname) + @")(?:""|')?\s*>\s*([^<>]{1,9}?|\[?[Ss]ee above\]?)\s*<\s*/\s*ref>)");

                foreach (Match m2 in shortNamedReferences.Matches(articleText))
                {
                    // don't apply if short ref is a page ref
                    if (refvalue.Length > 30 && !PageRef.IsMatch(m2.Groups[3].Value))
                        articleText = articleText.Replace(m2.Value, @"<ref name=""" + refname + @"""/>");
                }
            }

            return articleText;
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
        private static readonly Regex UrlShortDescription = new Regex(@"\s*[^{}<>\n]*?\s*\[*(?:http://www\.|http://|www\.)[^\[\]<>""\s]+?\s+([^{}<>\[\]]{4,35}?)\s*(?:\]|<!--|\u230A\u230A\u230A\u230A)", RegexOptions.Compiled);
        private static readonly Regex UrlDomain = new Regex(@"\s*\w*?[^{}<>]{0,4}?\s*(?:\[?|\{\{\s*cit[^{}<>]*\|\s*url\s*=\s*)\s*(?:http://www\.|http://|www\.)([^\[\]<>""\s\/:]+)", RegexOptions.Compiled);
        private static readonly Regex HarvnbTemplate = new Regex(@"\s*{{[Hh]arv(?:(?:col)?(?:nb|txt))?\s*\|\s*([^{}\|]+?)\s*\|(?:[^{}]*?\|)?\s*(\d{4})\s*(?:\|\s*(?:pp?\s*=\s*)?([^{}\|]+?)\s*)?}}\s*", RegexOptions.Compiled);
        private static readonly Regex WholeShortReference = new Regex(@"\s*([^<>{}]{4,35})\s*", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateUrl = new Regex(@"\s*\{\{\s*cit[^{}<>]*\|\s*url\s*=\s*([^\/<>{}\|]{4,35})", RegexOptions.Compiled);
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
            derivedReferenceName = ExtractReferenceNameComponents(reference, UrlShortDescription, 1);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // website URL first, allowing a name before link
            derivedReferenceName = ExtractReferenceNameComponents(reference, UrlDomain, 1);

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
            derivedReferenceName = ExtractReferenceNameComponents(reference, CiteTemplateUrl, 1);

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

            // generic ReferenceB
            derivedReferenceName = @"ReferenceB";

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // generic ReferenceC
            derivedReferenceName = @"ReferenceC";

            return ReferenceNameValid(articleText, derivedReferenceName) ? derivedReferenceName : "";
        }

        /// <summary>
        /// Checks the validity of a new reference name
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <param name="derivedReferenceName">The reference name</param>
        /// <returns>Whether the article does not already have a reference of that name</returns>
        private static bool ReferenceNameValid(string articleText, string derivedReferenceName)
        {
            return !Regex.IsMatch(articleText, RefName + Regex.Escape(derivedReferenceName) + @"(?:""|')?\s*/?\s*>") && derivedReferenceName.Length >= 3;
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

        private static readonly Regex CiteWeb = Tools.NestedTemplateRegex(new List<string>(new [] { "cite web", "citeweb" } ));
        private static readonly Regex CitationPopulatedParameter = new Regex(@"\|\s*([a-z_0-9-]+)\s*=\s*([^\|}]{3,}?)\s*");

        /// <summary>
        /// Searches for unknown/invalid parameters within citation templates
        /// </summary>
        /// <param name="articleText">the wiki text to search</param>
        /// <returns>Dictionary of parameter index in wiki text, and parameter length</returns>
        public static Dictionary<int, int> BadCiteParameters(string articleText)
        {
            Regex citeWebParameters = new Regex(@"\b(first\d?|last\d?|author|authorlink\d?|coauthors|title|url|archiveurl|work|publisher|location|pages?|language|trans_title|format|doi|date|month|year|archivedate|accessdate|quote|ref|separator|postscript|at)\b");

            Dictionary<int, int> found = new Dictionary<int, int>();
            foreach (Match m in CiteWeb.Matches(articleText))
            {
                foreach (Match m2 in CitationPopulatedParameter.Matches(m.Value))
                {
                    if (!citeWebParameters.IsMatch(m2.Groups[1].Value) && m2.Groups[2].Value.Trim().Length > 2)
                        found.Add(m.Index + m2.Groups[1].Index, m2.Groups[1].Length);
                }
            }
            
            foreach(Match m in WikiRegexes.CiteTemplate.Matches(articleText))
            {
                if(Regex.IsMatch(Tools.GetTemplateName(m.Value), @"[Cc]ite\s*(?:web|book|news|journal|paper|press release|hansard|encyclopedia)"))
                {
                    string pipecleaned = Tools.PipeCleanedTemplate(m.Value, false);
                    if(Regex.Matches(pipecleaned, @"=").Count > 0)
                    {                        
                        int noequals = Regex.Match(pipecleaned, @"\|[^=]+?\|").Index;
                        
                        if(noequals > 0)
                            found.Add(m.Index+noequals, Regex.Match(pipecleaned, @"\|[^=]+?\|").Value.Length);                 
                    }
                }
            }
            return found;
        }

        /// <summary>
        /// Searches for {{dead link}}s
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>Dictionary of dead links found</returns>
        public static Dictionary<int, int> DeadLinks(string articleText)
        {
            articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.Comments);
            Dictionary<int, int> found = new Dictionary<int, int>();

            foreach (Match m in WikiRegexes.DeadLink.Matches(articleText))
            {
                found.Add(m.Index, m.Length);
            }

            return found;
        }

        private const string SiCitStart = @"(?si)(\{\{\s*cit[^{}]*\|\s*";
        private const string CitAccessdate = SiCitStart + @"(?:access|archive)date\s*=\s*";
        private const string CitDate = SiCitStart + @"(?:archive|air)?date2?\s*=\s*";
        private const string CitYMonthD = SiCitStart + @"(?:archive|air|access)?date2?\s*=\s*\d{4})[-/\s]";
        private const string dTemEnd = @"?[-/\s]([0-3]?\d\s*(?:\||}}))";

        private static readonly Regex AccessOrArchiveDate = new Regex(@"\b(access|archive)date\s*=", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly RegexReplacement[] CiteTemplateIncorrectISOAccessdates = new[] {
            new RegexReplacement(CitAccessdate + @")(1[0-2])[/_\-\.]?(1[3-9])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3"),
            new RegexReplacement(CitAccessdate + @")(1[0-2])[/_\-\.]?([2-3]\d)[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3"),
            new RegexReplacement(CitAccessdate + @")(1[0-2])[/_\-\.]?\2[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$3-$2-$2"), // nn-nn-2004 and nn-nn-04 to ISO format (both nn the same)
            new RegexReplacement(CitAccessdate + @")(1[3-9])[/_\-\.]?(1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2"),
            new RegexReplacement(CitAccessdate + @")(1[3-9])[/_\-\.]?0?([1-9])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2"),
            new RegexReplacement(CitAccessdate + @")(20[01]\d)0?([01]\d)[/_\-\.]([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4"),
            new RegexReplacement(CitAccessdate + @")(20[01]\d)[/_\-\.]([01]\d)0?([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4"),
            new RegexReplacement(CitAccessdate + @")(20[01]\d)[/_\-\.]?([01]\d)[/_\-\.]?([1-9]\s*(?:\||}}))", "$1$2-$3-0$4"),
            new RegexReplacement(CitAccessdate + @")(20[01]\d)[/_\-\.]?([1-9])[/_\-\.]?([0-3]\d\s*(?:\||}}))", "$1$2-0$3-$4"),
            new RegexReplacement(CitAccessdate + @")(20[01]\d)[/_\-\.]?([1-9])[/_\-\.]0?([1-9]\s*(?:\||}}))", "$1$2-0$3-0$4"),
            new RegexReplacement(CitAccessdate + @")(20[01]\d)[/_\-\.]0?([1-9])[/_\-\.]([1-9]\s*(?:\||}}))", "$1$2-0$3-0$4"),
            new RegexReplacement(CitAccessdate + @")(20[01]\d)[/_\.]?([01]\d)[/_\.]?([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4"),

            new RegexReplacement(CitAccessdate + @")([2-3]\d)[/_\-\.]?(1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2"),
            new RegexReplacement(CitAccessdate + @")([2-3]\d)[/_\-\.]0?([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2"),
            new RegexReplacement(CitAccessdate + @")0?([1-9])[/_\-\.]?(1[3-9]|[2-3]\d)[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$2-$3"),
            new RegexReplacement(CitAccessdate + @")0?([1-9])[/_\-\.]?0?\2[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$3-0$2-0$2"), // n-n-2004 and n-n-04 to ISO format (both n the same)
        };

        private static readonly Regex CiteTemplateArchiveAirDate = new Regex(@"{{\s*cit[^{}]*\|\s*(?:archive|air)?date2?\s*=", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly RegexReplacement[] CiteTemplateIncorrectISODates = new[] {
            new RegexReplacement(CitDate + @"\[?\[?)(20\d\d|19[7-9]\d)[/_]?([0-1]\d)[/_]?([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[0-2])[/_\-\.]?([2-3]\d)[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)0?([1-9])[/_\-\.]?([2-3]\d)[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)([2-3]\d)[/_\-\.]?0?([1-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)([2-3]\d)[/_\-\.]?(1[0-2])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[0-2])[/_\-\.]([2-3]\d)[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)0?([1-9])[/_\-\.]([2-3]\d)[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)([2-3]\d)[/_\-\.]0?([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)([2-3]\d)[/_\-\.](1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[0-2])[/_\-\.]?(1[3-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)0?([1-9])[/_\-\.](1[3-9])[/_\-\.](19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[3-9])[/_\-\.]?0?([1-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[3-9])[/_\-\.]?(1[0-2])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[0-2])[/_\-\.](1[3-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)([1-9])[/_\-\.](1[3-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[3-9])[/_\-\.]([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[3-9])[/_\-\.](1[0-2])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2"),
            new RegexReplacement(CitDate + @")0?([1-9])[/_\-\.]0?\2[/_\-\.](20\d\d|19[7-9]\d)(?=\s*(?:\||}}))", "$1$3-0$2-0$2"), // n-n-2004 and n-n-1980 to ISO format (both n the same)
            new RegexReplacement(CitDate + @")0?([1-9])[/_\-\.]0?\2[/_\-\.]([01]\d)(?=\s*(?:\||}}))", "${1}20$3-0$2-0$2"), // n-n-04 to ISO format (both n the same)
            new RegexReplacement(CitDate + @")(1[0-2])[/_\-\.]\2[/_\-\.]?(20\d\d|19[7-9]\d)(?=\s*(?:\||}}))", "$1$3-$2-$2"), // nn-nn-2004 and nn-nn-1980 to ISO format (both nn the same)
            new RegexReplacement(CitDate + @")(1[0-2])[/_\-\.]\2[/_\-\.]([01]\d)(?=\s*(?:\||}}))", "${1}20$3-$2-$2"), // nn-nn-04 to ISO format (both nn the same)
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|19[7-9]\d)[/_\-\.]([1-9])[/_\-\.]0?([1-9](?:\]\])?\s*(?:\||}}))", "$1$2-0$3-0$4"),
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|19[7-9]\d)[/_\-\.]0?([1-9])[/_\-\.]([1-9](?:\]\])?\s*(?:\||}}))", "$1$2-0$3-0$4"),
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|19[7-9]\d)[/_\-\.]?([0-1]\d)[/_\-\.]?([1-9](?:\]\])?\s*(?:\||}}))", "$1$2-$3-0$4"),
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|19[7-9]\d)[/_\-\.]?([1-9])[/_\-\.]?([0-3]\d(?:\]\])?\s*(?:\||}}))", "$1$2-0$3-$4"),
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|19[7-9]\d)([0-1]\d)[/_\-\.]([0-3]\d(?:\]\])?\s*(?:\||}}))", "$1$2-$3-$4"),
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|19[7-9]\d)[/_\-\.]([0-1]\d)0?([0-3]\d(?:\]\])?\s*(?:\||}}))", "$1$2-$3-$4"),
        };

        private static readonly RegexReplacement[] CiteTemplateAbbreviatedMonths = new[] {
            new RegexReplacement(CitYMonthD + @"Jan(?:uary|\.)" + dTemEnd, "$1-01-$2"),
            new RegexReplacement(CitYMonthD + @"Feb(?:r?uary|\.)" + dTemEnd, "$1-02-$2"),
            new RegexReplacement(CitYMonthD + @"Mar(?:ch|\.)" + dTemEnd, "$1-03-$2"),
            new RegexReplacement(CitYMonthD + @"Apr(?:il|\.)" + dTemEnd, "$1-04-$2"),
            new RegexReplacement(CitYMonthD + @"May\." + dTemEnd, "$1-05-$2"),
            new RegexReplacement(CitYMonthD + @"Jun(?:e|\.)" + dTemEnd, "$1-06-$2"),
            new RegexReplacement(CitYMonthD + @"Jul(?:y|\.)" + dTemEnd, "$1-07-$2"),
            new RegexReplacement(CitYMonthD + @"Aug(?:ust|\.)" + dTemEnd, "$1-08-$2"),
            new RegexReplacement(CitYMonthD + @"Sep(?:tember|\.)" + dTemEnd, "$1-09-$2"),
            new RegexReplacement(CitYMonthD + @"Oct(?:ober|\.)" + dTemEnd, "$1-10-$2"),
            new RegexReplacement(CitYMonthD + @"Nov(?:ember|\.)" + dTemEnd, "$1-11-$2"),
            new RegexReplacement(CitYMonthD + @"Dec(?:ember|\.)" + dTemEnd, "$1-12-$2"),
        };

        private static readonly Regex CiteTemplateDateYYYYDDMMFormat = new Regex(SiCitStart + @"(?:archive|air|access)?date2?\s*=\s*(?:\[\[)?20\d\d)-([2-3]\d|1[3-9])-(0[1-9]|1[0-2])(\]\])?", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateTimeInDateParameter = new Regex(@"(\{\{\s*cite[^\{\}]*\|\s*(?:archive|air|access)?date2?\s*=\s*(?:(?:20\d\d|19[7-9]\d)-[01]?\d-[0-3]?\d|[0-3]?\d\s*\w+,?\s*(?:20\d\d|19[7-9]\d)|\w+\s*[0-3]?\d,?\s*(?:20\d\d|19[7-9]\d)))(\s*[,-:]?\s+[0-2]?\d[:\.]?[0-5]\d(?:\:?[0-5]\d)?\s*[^\|\}]*)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Corrects common formatting errors in dates in external reference citation templates (doesn't link/delink dates)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string CiteTemplateDates(string articleText)
        {
            if (!Variables.IsWikipediaEN)
                return articleText;

            // cite podcast is non-compliant to citation core standards
            if (Tools.NestedTemplateRegex("cite podcast").IsMatch(articleText))
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
                // provided no ambiguous ones
                if (AccessOrArchiveDate.IsMatch(articleText) && !AmbiguousCiteTemplateDates(articleText))
                    foreach (RegexReplacement rr in CiteTemplateIncorrectISOAccessdates)
                        articleText = rr.Regex.Replace(articleText, rr.Replacement);

                // date=, archivedate=, airdate=, date2=
                if (CiteTemplateArchiveAirDate.IsMatch(articleText) && !AmbiguousCiteTemplateDates(articleText))
                {
                    foreach (RegexReplacement rr in CiteTemplateIncorrectISODates)
                        articleText = rr.Regex.Replace(articleText, rr.Replacement);

                    // date = YYYY-Month-DD fix
                    foreach (RegexReplacement rr in CiteTemplateAbbreviatedMonths)
                        articleText = rr.Regex.Replace(articleText, rr.Replacement);
                }

                // all citation dates
                articleText = CiteTemplateDateYYYYDDMMFormat.Replace(articleText, "$1-$3-$2$4"); // YYYY-DD-MM to YYYY-MM-DD
                articleText = CiteTemplateTimeInDateParameter.Replace(articleText, "$1<!--$2-->"); // Removes time from date fields
            }

            return articleText;
        }


        private static readonly Regex PossibleAmbiguousCiteDate = new Regex(@"(?<={{\s*[Cc]it[ae][^{}]+?\|\s*(?:access|archive|air)?date2?\s*=\s*)(0?[1-9]|1[0-2])[/_\-\.](0?[1-9]|1[0-2])[/_\-\.](20\d\d|19[7-9]\d|[01]\d)\b");

        /// <summary>
        /// Returns whether the input article text contains ambiguous cite template dates in XX-XX-YYYY or XX-XX-YY format
        /// </summary>
        /// <param name="articleText">the article text to search</param>
        /// <returns>If any matches were found</returns>
        public static bool AmbiguousCiteTemplateDates(string articleText)
        {
            return AmbigCiteTemplateDates(articleText).Count > 0;
        }
        
        private static readonly Regex MathSourceCodeNowikiPreTagStart = new Regex(@"<\s*(?:math|source\b[^>]*|code|nowiki|pre)\s*>", RegexOptions.Compiled);
        /// <summary>
        ///  Searches for any unclosed &lt;math&gt;, &lt;source&gt;, &lt;code&gt;, &lt;nowiki&gt; or &lt;pre&gt; tags
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>dictionary of the index and length of any unclosed tags</returns>
        public static Dictionary<int, int> UnclosedTags(string articleText)
        {
            Dictionary<int, int> back = new Dictionary<int, int>();

            // clear out all the matched tags
            articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.UnformattedText);
            articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.Code);
            articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.Source);
            
            foreach(Match m in MathSourceCodeNowikiPreTagStart.Matches(articleText))
            {
                back.Add(m.Index, m.Length);
            }
            return back;
        }
        
        /// <summary>
        /// Returns whether the input article text contains ambiguous cite template dates in XX-XX-YYYY or XX-XX-YY format
        /// </summary>
        /// <param name="articleText">The article text to search</param>
        /// <returns>A dictionary of matches (index and length)</returns>
        public static Dictionary<int, int> AmbigCiteTemplateDates(string articleText)
        {
            Dictionary<int, int> ambigDates = new Dictionary<int, int>();
            
            foreach (Match m in PossibleAmbiguousCiteDate.Matches(articleText))
            {
                // for YYYY-AA-BB date, ambiguous if AA and BB not the same
                if (m.Groups[1].Value != m.Groups[2].Value)
                    ambigDates.Add(m.Index, m.Length);
            }

            return ambigDates;
        }

        // Covered by: FormattingTests.TestMdashes()
        /// <summary>
        /// Replaces hyphens and em-dashes with en-dashes, per [[WP:DASH]]
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article's title</param>
        /// <returns>The modified article text.</returns>
        [Obsolete("namespace key no longer needed as input")]
        public string Mdashes(string articleText, string articleTitle, int nameSpaceKey)
        {
            return Mdashes(articleText, articleTitle);
        }

        private static readonly Regex PageRangeIncorrectMdash = new Regex(@"(pages\s*=\s*|pp\.?\s*)((?:&nbsp;)?\d+\s*)(?:-|—|&mdash;|&#8212;)(\s*\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // avoid dimensions in format 55-66-77
        private static readonly Regex UnitTimeRangeIncorrectMdash = new Regex(@"(?<!-)(\b[1-9]?\d+\s*)(?:-|—|&mdash;|&#8212;)(\s*[1-9]?\d+)(\s+|&nbsp;)((?:years|months|weeks|days|hours|minutes|seconds|[km]g|kb|[ckm]?m|[Gk]?Hz|miles|mi\.|%|feet|foot|ft|met(?:er|re)s)\b|in\))", RegexOptions.Compiled);
        private static readonly Regex DollarAmountIncorrectMdash = new Regex(@"(\$[1-9]?\d{1,3}\s*)(?:-|—|&mdash;|&#8212;)(\s*\$?[1-9]?\d{1,3})", RegexOptions.Compiled);
        private static readonly Regex AMPMIncorrectMdash = new Regex(@"([01]?\d:[0-5]\d\s*([AP]M)\s*)(?:-|—|&mdash;|&#8212;)(\s*[01]?\d:[0-5]\d\s*([AP]M))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex AgeIncorrectMdash = new Regex(@"([Aa]ge[sd])\s([1-9]?\d\s*)(?:-|—|&mdash;|&#8212;)(\s*[1-9]?\d)", RegexOptions.Compiled);
        private static readonly Regex SentenceClauseIncorrectMdash = new Regex(@"(?!xn--)(\w{2})\s*--\s*(?=\w)", RegexOptions.Compiled);
        private static readonly Regex SuperscriptMinus = new Regex(@"(?<=<sup>)(?:-|–|—)(?=\d+</sup>)", RegexOptions.Compiled);

        // Covered by: FormattingTests.TestMdashes()
        /// <summary>
        /// Replaces hyphens and em-dashes with en-dashes, per [[WP:DASH]]
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article's title</param>
        /// <param name="nameSpaceKey">The namespace of the article</param>
        /// <returns>The modified article text.</returns>
        public string Mdashes(string articleText, string articleTitle)
        {
            articleText = HideMoreText(articleText);

            // replace hyphen with dash and convert Pp. to pp.
            foreach (Match m in PageRangeIncorrectMdash.Matches(articleText))
            {
                string pagespart = m.Groups[1].Value;
                if (pagespart.Contains(@"Pp"))
                    pagespart = pagespart.ToLower();

                articleText = articleText.Replace(m.Value, pagespart + m.Groups[2].Value + @"–" + m.Groups[3].Value);
            }

            articleText = PageRangeIncorrectMdash.Replace(articleText, @"$1$2–$3");
            articleText = UnitTimeRangeIncorrectMdash.Replace(articleText, @"$1–$2$3$4");
            articleText = DollarAmountIncorrectMdash.Replace(articleText, @"$1–$2");
            articleText = AMPMIncorrectMdash.Replace(articleText, @"$1–$3");
            articleText = AgeIncorrectMdash.Replace(articleText, @"$1 $2–$3");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Match_en_dashes.2Femdashs_from_titles_with_those_in_the_text
            // if title has en or em dashes, apply them to strings matching article title but with hyphens
            if (articleTitle.Contains(@"–") || articleTitle.Contains(@"—"))
                articleText = Regex.Replace(articleText, Regex.Escape(articleTitle.Replace(@"–", @"-").Replace(@"—", @"-")), articleTitle);

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Change_--_.28two_dashes.29_to_.E2.80.94_.28em_dash.29
            if (Namespace.Determine(articleTitle) == Namespace.Mainspace)
                articleText = SentenceClauseIncorrectMdash.Replace(articleText, @"$1—");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#minuses
            // replace hyphen or en-dash or emdash with Unicode minus (&minus;)
            // [[Wikipedia:MOSNUM#Common_mathematical_symbols]]
            articleText = SuperscriptMinus.Replace(articleText, "−");

            return AddBackMoreText(articleText);
        }

        // Covered by: FootnotesTests.TestFixReferenceListTags()
        private static string ReflistMatchEvaluator(Match m)
        {
            // don't change anything if div tags mismatch
            if (DivStart.Matches(m.Value).Count != DivEnd.Matches(m.Value).Count)
                return m.Value;

            if (m.Value.Contains("references-2column"))
                return "{{reflist|2}}";

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
        private static readonly Regex CategoryToReferences = new Regex(@"(?sim)((?:^\{\{(?![Tt]racklist\b)[^{}]+?\}\}\s*)*)(^\s*\[\[\s*Category\s*:.*?)(\r\n==+References==+\r\n{{Reflist}}<!--added above categories/infobox footers by script-assisted edit-->)", RegexOptions.Compiled);
        //private static readonly Regex AMR8 = new Regex(@"(?sim)(^==.*?)(^\{\{[^{}]+?\}\}.*?)(\r\n==+References==+\r\n{{Reflist}}<!--added to end of article by script-assisted edit-->)", RegexOptions.Compiled);
        private static readonly Regex ReflistByScript = new Regex(@"(\{\{Reflist\}\})<!--added[^<>]+by script-assisted edit-->", RegexOptions.Compiled);

        private static readonly Regex ReferencesMissingSlash = new Regex(@"<references>", RegexOptions.Compiled);
        
        /// <summary>
        /// First checks for a &lt;references&glt; missing '/' to correct, otherwise:
        /// if the article uses cite references but has no recognised template to display the references, add {{reflist}} in the appropriate place
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns></returns>
        public static string AddMissingReflist(string articleText)
        {
            if (!IsMissingReferencesDisplay(articleText))
                return articleText;
            
            if(ReferencesMissingSlash.IsMatch(articleText))
                return ReferencesMissingSlash.Replace(articleText, @"<references/>");

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
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*""[^<>=""\/]+?"")\s*/\s*(?:ref|/)\s*>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), "$1/>"),

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
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)([^<>=""'\/]+?\s+[^<>=""'\/\s]+?)(\s*/?>)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""$2""$3"),

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

            // trailing spaces at the end of a reference, within the reference
            new RegexReplacement(new Regex(@" +</ref>"), "</ref>"),
            
            // Trailing spaces at the beginning of a reference, within the reference
            new RegexReplacement(new Regex(@"(<ref[^<>\{\}\/]*>) +"), "$1"),

            // empty <ref>...</ref> tags
            new RegexReplacement(new Regex(@"<ref>\s*</ref>"), ""),
            
            // <ref name="Fred" Smith> --> <ref name="Fred Smith">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*""[^<>=""\/]+?)""([^<>=""\/]{2,}?)(?<!\s+)(?=\s*/?>)", RegexOptions.Compiled | RegexOptions.IgnoreCase), @"$1$2"""),
            
            // <ref name-"Fred"> --> <ref name="Fred">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*)-", RegexOptions.Compiled), "$1="),
            
            // <ref NAME= --> <ref name=
            new RegexReplacement(new Regex(@"<\s*ref\s+NAME(\s*=)", RegexOptions.Compiled), "<ref name$1"),
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
        private static readonly Regex OfBetweenMonthAndYear = new Regex(@"\b" + WikiRegexes.Months + @"\s+of\s+(20\d\d|1[89]\d\d)\b(?<!\b[Tt]he\s{1,5}\w{3,15}\s{1,5}of\s{1,5}(20\d\d|1[89]\d\d))", RegexOptions.Compiled);
        private static readonly Regex OrdinalsInDatesAm = new Regex(@"(?<!\b[1-3]\d +)\b" + WikiRegexes.Months + @"\s+([0-3]?\d)(?:st|nd|rd|th)\b(?<!\b[Tt]he\s+\w{3,10}\s+(?:[0-3]?\d)(?:st|nd|rd|th)\b)(?:(\s*(?:to|and|.|&.dash;)\s*[0-3]?\d)(?:st|nd|rd|th)\b)?", RegexOptions.Compiled);
        private static readonly Regex OrdinalsInDatesInt = new Regex(@"(?:\b([0-3]?\d)(?:st|nd|rd|th)(\s*(?:to|and|.|&.dash;)\s*))?\b([0-3]?\d)(?:st|nd|rd|th)\s+" + WikiRegexes.Months + @"\b(?<!\b[Tt]he\s+(?:[0-3]?\d)(?:st|nd|rd|th)\s+\w{3,10})", RegexOptions.Compiled);
        private static readonly Regex DateLeadingZerosAm = new Regex(@"(?<!\b[0-3]?\d\s*)\b" + WikiRegexes.Months + @"\s+0([1-9])" + @"\b", RegexOptions.Compiled);
        private static readonly Regex DateLeadingZerosInt = new Regex(@"\b" + @"0([1-9])\s+" + WikiRegexes.Months + @"\b", RegexOptions.Compiled);
        private static readonly Regex MonthsRegex = new Regex(@"\b" + WikiRegexes.MonthsNoGroup + @"\b", RegexOptions.Compiled);
        private static readonly Regex DayOfMonth = new Regex(@"(?<![Tt]he +)\b([1-9]|[12][0-9]|3[01])(?:st|nd|rd|th) +of +" + WikiRegexes.Months, RegexOptions.Compiled);

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
                articleText = DayOfMonth.Replace(articleText, "$1 $2");
            }

            articleText = DateLeadingZerosAm.Replace(articleText, "$1 $2");
            articleText = DateLeadingZerosInt.Replace(articleText, "$1 $2");

            // catch after any other fixes
            articleText = NoCommaAmericanDates.Replace(articleText, @"$1, $2");

            return AddBackMoreText(articleText);
        }

        private static readonly Regex BrTwoNewlines = new Regex("<br ?/?>\r\n\r\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex ThreeOrMoreNewlines = new Regex("\r\n(\r\n)+", RegexOptions.Compiled);
        private static readonly Regex TwoNewlinesInBlankSection = new Regex("== ? ?\r\n\r\n==", RegexOptions.Compiled);
        private static readonly Regex NewlinesBelowExternalLinks = new Regex(@"==External links==[\r\n\s]*\*", RegexOptions.Compiled);
        private static readonly Regex NewlinesBeforeUrl = new Regex(@"\r\n\r\n(\* ?\[?http)", RegexOptions.Compiled);
        private static readonly Regex HorizontalRule = new Regex("----+$", RegexOptions.Compiled);
        private static readonly Regex MultipleTabs = new Regex("  +", RegexOptions.Compiled);
        private static readonly Regex SpaceThenNewline = new Regex(" \r\n", RegexOptions.Compiled);
        private static readonly Regex WikiListWithMultipleSpaces = new Regex(@"^([\*#]+) +", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex WikiList = new Regex(@"^([\*#]+)", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SpacedHeadings = new Regex("^(={1,4}) ?(.*?) ?(={1,4})$", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SpacedDashes = new Regex(" (–|—|&#15[01];|&[nm]dash;|&#821[12];|&#x201[34];) ", RegexOptions.Compiled);
        
        /// <summary>
        /// Applies/removes some excess whitespace from the article
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string RemoveWhiteSpace(string articleText)
        {
            return RemoveWhiteSpace(articleText, true);
        }
        
        // Covered by: FormattingTests.TestFixWhitespace(), incomplete
        /// <summary>
        /// Applies/removes some excess whitespace from the article
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="fixOptionalWhitespace">Whether to remove cosmetic whitespace</param>
        /// <returns>The modified article text.</returns>
        public static string RemoveWhiteSpace(string articleText, bool fixOptionalWhitespace)
        {
            //Remove <br /> if followed by double newline
            articleText = BrTwoNewlines.Replace(articleText.Trim(), "\r\n\r\n");

            articleText = ThreeOrMoreNewlines.Replace(articleText, "\r\n\r\n");

            if(fixOptionalWhitespace)
                articleText = TwoNewlinesInBlankSection.Replace(articleText, "==\r\n==");
            
            articleText = NewlinesBelowExternalLinks.Replace(articleText, "==External links==\r\n*");
            articleText = NewlinesBeforeUrl.Replace(articleText, "\r\n$1");

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
        private const string CitUrl = @"(\{\{\s*cit[ae][^{}]*?\|\s*url\s*=\s*)";
        private static readonly Regex BracketsAtBeginCiteTemplateURL = new Regex(CitUrl + @"\[+\s*((?:(?:ht|f)tp://)?[^\[\]<>""\s]+?\s*)\]?" + TemEnd, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex BracketsAtEndCiteTemplateURL = new Regex(CitUrl + @"\[?\s*((?:(?:ht|f)tp://)?[^\[\]<>""\s]+?\s*)\]+" + TemEnd, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex SyntaxRegexWikilinkMissingClosingBracket = new Regex(@"\[\[([^][]*?)\](?=[^\]]*?(?:$|\[|\n))", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexWikilinkMissingOpeningBracket = new Regex(@"(?<=(?:^|\]|\n)[^\[]*?)\[([^][]*?)\]\](?!\])", RegexOptions.Compiled);

        private static readonly Regex SyntaxRegexExternalLinkToImageURL = new Regex("\\[?\\[image:(http:\\/\\/.*?)\\]\\]?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexSimpleWikilinkStartsWithSpaces = new Regex("\\[\\[ (.*)?\\]\\]", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexSimpleWikilinkEndsWithSpaces = new Regex("\\[\\[([A-Za-z]*) \\]\\]", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexSectionLinkUnnecessaryUnderscore = new Regex("\\[\\[(.*)?_#(.*)\\]\\]", RegexOptions.Compiled);

        private static readonly Regex SyntaxRegexTemplate = new Regex(@"(\{\{\s*)[Tt]emplate\s*:(.*?\}\})", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexListRowBrTag = new Regex(@"^([#\*:;]+.*?) *<[/\\]?br ?[/\\]?> *\r\n", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // make double spaces within wikilinks just single spaces
        private static readonly Regex SyntaxRegexMultipleSpacesInWikilink = new Regex(@"(\[\[[^\[\]]+?) {2,}([^\[\]]+\]\])", RegexOptions.Compiled);

        private static readonly Regex SyntaxRegexItalic = new Regex("< *i *>(.*?)< */ *i *>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexBold = new Regex("< *b *>(.*?)< */ *b *>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Matches <p> tags only if current line does not start from ! or | (indicator of table cells)
        private static readonly Regex SyntaxRemoveParagraphs = new Regex(@"(?<!^[!\|].*)</? ?[Pp]>", RegexOptions.Multiline | RegexOptions.Compiled);
        // same for <br>
        private static readonly Regex SyntaxRemoveBr = new Regex(@"(?<!^[!\|].*)(<br[\s/]*> *){2,}", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex MultipleHttpInLink = new Regex(@"([\s\[>=])((?:ht|f)tp:?/+)(\2)+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex PipedExternalLink = new Regex(@"(\[\w+://[^\]\[<>\""\s]*?\s*)(?: +\||\|([ ']))(?=[^\[\]\|]*\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex MissingColonInHttpLink = new Regex(@"([\s\[>=](?:ht|f))tp(?://?:?|:(?::+//)?)(\w+)", RegexOptions.Compiled);
        private static readonly Regex SingleTripleSlashInHttpLink = new Regex(@"([\s\[>=](?:ht|f))tp:(?:/|///)(\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex CellpaddingTypo = new Regex(@"({\s*\|\s*class\s*=\s*""wikitable[^}]*?)cel(?:lpa|pad?)ding\b", RegexOptions.IgnoreCase | RegexOptions.Singleline);

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
        
        // space needed between word and external link
        private static readonly Regex ExternalLinkWordSpacingBefore = new Regex(@"(\w)(?=\[(?:https?|ftp|mailto|irc|gopher|telnet|nntp|worldwind|news|svn)://.*?\])", RegexOptions.Compiled);
        private static readonly Regex ExternalLinkWordSpacingAfter = new Regex(@"(?<=\[(?:https?|ftp|mailto|irc|gopher|telnet|nntp|worldwind|news|svn)://[^\]\[<>]*?\])(\w)", RegexOptions.Compiled);

        private static readonly Regex WikilinkEndsBr = new Regex(@"<br[\s/]*>\]\]$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        // for correcting square brackets within external links
        private static readonly Regex SquareBracketsInExternalLinks = new Regex(@"(\[https?://(?>[^\[\]<>]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\])", RegexOptions.Compiled);

        // fix incorrect <br> of <br.>, <\br> and <br\>
        private static readonly Regex IncorrectBr = new Regex(@"< *br\. *>|<\\ *br *>|< *br *\\ *>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex SyntaxRegexHorizontalRule = new Regex("^<hr>|^----+", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SyntaxRegexHeadingWithHorizontalRule = new Regex("(^==?[^=]*==?)\r\n(\r\n)?----+", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SyntaxRegexHTTPNumber = new Regex(@"HTTP/\d\.", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexISBN = new Regex("ISBN: ?([0-9])", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexExternalLinkOnWholeLine = new Regex(@"^\[(\s*http.*?)\]$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex SyntaxRegexClosingBracket = new Regex(@"([^]])\]([^]]|$)", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexImageWithHTTP = new Regex("\\[\\[[Ii]mage:[^]]*http", RegexOptions.Compiled);

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
            articleText = SyntaxRegexItalic.Replace(articleText, "''$1''");

            articleText = SyntaxRegexBold.Replace(articleText, "'''$1'''");

            articleText = SyntaxRegexHorizontalRule.Replace(articleText, "----");

            //remove appearance of double line break
            articleText = SyntaxRegexHeadingWithHorizontalRule.Replace(articleText, "$1");

            // double piped links e.g. [[foo||bar]]
            articleText = DoublePipeInWikiLink.Replace(articleText, "|");

            // remove unnecessary namespace
            articleText = SyntaxRegexTemplate.Replace(articleText, "$1$2");

            // remove <br> from lists
            articleText = SyntaxRegexListRowBrTag.Replace(articleText, "$1\r\n");

            //fix uneven bracketing on links
            articleText = DoubleBracketAtStartOfExternalLink.Replace(articleText, "$1");
            articleText = DoubleBracketAtEndOfExternalLink.Replace(articleText, "$1");
            articleText = DoubleBracketAtEndOfExternalLinkWithinImage.Replace(articleText, "$1");

            // (part) wikilinked/external linked URL in cite template, uses MediaWiki regex of [^\[\]<>""\s] for URL bit after http://
            articleText = BracketsAtBeginCiteTemplateURL.Replace(articleText, "$1$2$3");
            articleText = BracketsAtEndCiteTemplateURL.Replace(articleText, "$1$2$3");

            articleText = MultipleHttpInLink.Replace(articleText, "$1$2");

            articleText = PipedExternalLink.Replace(articleText, "$1 $2");

            //repair bad external links
            articleText = SyntaxRegexExternalLinkToImageURL.Replace(articleText, "[$1]");

            //repair bad internal links
            articleText = SyntaxRegexSimpleWikilinkStartsWithSpaces.Replace(articleText, "[[$1]]");
            articleText = SyntaxRegexSimpleWikilinkEndsWithSpaces.Replace(articleText, "[[$1]]");
            articleText = SyntaxRegexSectionLinkUnnecessaryUnderscore.Replace(articleText, "[[$1#$2]]");
            articleText = SyntaxRegexMultipleSpacesInWikilink.Replace(articleText, @"$1 $2");

            if (!SyntaxRegexHTTPNumber.IsMatch(articleText))
            {
                articleText = MissingColonInHttpLink.Replace(articleText, "$1tp://$2");
                articleText = SingleTripleSlashInHttpLink.Replace(articleText, "$1tp://$2");
            }

            articleText = SyntaxRegexISBN.Replace(articleText, "ISBN $1");

            articleText = CellpaddingTypo.Replace(articleText, "$1cellpadding");

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
                string externalLink = SyntaxRegexExternalLinkOnWholeLine.Replace(m.Value, "$1");

                // if there are some brackets left then they need fixing; the mediawiki parser finishes the external link
                // at the first ] found
                if (externalLink.Contains("]"))
                {
                    // replace single ] with &#93; when used for brackets in the link description
                    externalLink = SyntaxRegexClosingBracket.Replace(externalLink, @"$1&#93;$2");

                    articleText = articleText.Replace(m.Value, @"[" + externalLink + @"]");
                }
            }

            // needs to be applied after SquareBracketsInExternalLinks
            if (!SyntaxRegexImageWithHTTP.IsMatch(articleText))
            {
                articleText = SyntaxRegexWikilinkMissingClosingBracket.Replace(articleText, "[[$1]]");
                articleText = SyntaxRegexWikilinkMissingOpeningBracket.Replace(articleText, "[[$1]]");
            }

            // if there are some unbalanced brackets, see whether we can fix them
            articleText = FixUnbalancedBrackets(articleText);

            // http://en.wikipedia.org/wiki/Wikipedia:WikiProject_Check_Wikipedia#Article_with_false_.3Cbr.2F.3E_.28AutoEd.29
            // fix incorrect <br> of <br.>, <\br> and <br\>
            articleText = IncorrectBr.Replace(articleText, "<br />");

            articleText = WordingIntoBareExternalLinks.Replace(articleText, @"[$2 $1]");
            
            articleText = ExternalLinkWordSpacingBefore.Replace(articleText, "$1 ");
            articleText = ExternalLinkWordSpacingAfter.Replace(articleText, " $1");
            
            // WP:CHECKWIKI 065 – http://toolserver.org/~sk/cgi-bin/checkwiki/checkwiki.cgi?project=enwiki&view=only&id=65
            foreach (Match m in WikiRegexes.FileNamespaceLink.Matches(articleText))
            {
                if(WikilinkEndsBr.IsMatch(m.Value))
                    articleText = articleText.Replace(m.Value, WikilinkEndsBr.Replace(m.Value, @"]]"));
            }

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
                if (bracketLength == 1 && unbalancedBracket > 2
                    && articleTextTemp[unbalancedBracket] == ']'
                    && articleTextTemp[unbalancedBracket - 1] == ']'
                    && articleTextTemp[unbalancedBracket - 2] == ']'
                   )
                {
                    articleTextTemp = articleTextTemp.Remove(unbalancedBracket, 1);
                }

                else if (bracketLength == 1)
                {
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

                    // if it's {[link]] or {[[link]] or [[[link]] then see if setting to [[ makes it all balance
                    articleTextTemp = ExtraBracketOnWikilinkOpening.Replace(articleTextTemp, "[[");
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

                    // defaultsort missing }} at end
                    string defaultsort = WikiRegexes.Defaultsort.Match(articleTextTemp).Value;
                    if (!string.IsNullOrEmpty(defaultsort) && !defaultsort.Contains("}}"))
                        articleTextTemp = articleTextTemp.Replace(defaultsort.TrimEnd(), defaultsort.TrimEnd() + "}}");
                }

                unbalancedBracket = UnbalancedBrackets(articleTextTemp, ref bracketLength);
                // the change worked if unbalanced bracket location moved considerably (so this one fixed), or all brackets now balance
                if (unbalancedBracket < 0 || Math.Abs(unbalancedBracket - firstUnbalancedBracket) > 300)
                    articleText = articleTextTemp;
            }

            return articleText;
        }
        
        private static List<string> DateFields = new List<string>(new [] { "date" , "accessdate", "archivedate" ,"airdate" } );
        
        /// <summary>
        /// Updates dates in citation templates to use the strict predominant date format in the article (en wiki only)
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>The updated article text</returns>
        public static string PredominantDates(string articleText)
        {
            if(Variables.LangCode != "en")
                return articleText;
            
            DateLocale predominantLocale = DeterminePredominantDateLocale(articleText, true, true);
            
            if(predominantLocale.Equals(DateLocale.Undetermined))
                return articleText;
            
            foreach (Match m in WikiRegexes.CiteTemplate.Matches(articleText))
            {
                string newValue = m.Value;
                
                foreach(string field in DateFields)
                {
                    string fvalue = Tools.GetTemplateParameterValue(newValue, field);
                    
                    if(WikiRegexes.ISODates.IsMatch(fvalue) || WikiRegexes.AmericanDates.IsMatch(fvalue) || WikiRegexes.InternationalDates.IsMatch(fvalue))
                        newValue = Tools.UpdateTemplateParameterValue(newValue, field, Tools.ISOToENDate(fvalue, predominantLocale));
                }
                
                // merge changes to article text
                if(!m.Value.Equals(newValue))
                    articleText = articleText.Replace(m.Value, newValue);
            }

            return articleText;
        }

        private static readonly Regex AccessdateTypo = new Regex(@"(\{\{\s*cit[^{}]*?\|\s*)ac(?:(?:ess?s?|cc?es|cess[es]|ccess)date|cessda[ry]e|c?essdat|cess(?:daste|ate)|cdessdate|cess?data|cesdsate|cessdaet|cessdatge|cesseddate|cessedon|cess-date)(\s*=\s*)", RegexOptions.IgnoreCase);

        private static readonly Regex PublisherTypo = new Regex(@"(?<={{\s*[Cc]it[ae][^{}\|]*?\|(?:[^{}]+\|)?\s*)(?:p(?:u[ns]|oub)lisher|publihser|pub(?:lication)?|pubslisher|puablisher|publicher|ublisher|publsiher|pusliher|pblisher|pubi?lsher|publishet|puiblisher|puplisher|publiisher|publiser|pulisher|publishser|pulbisher|publisber|publoisher|publishier|pubhlisher|publiaher|publicser|publicsher|publidsherr|publiher|publihsher|publilsher|publiosher|publisaher|publischer|publiseher|publisehr|publiserh|publisger|publishe?|publishey|publlisher|publusher|pubsliher)(\s*=)", RegexOptions.Compiled);

        private static readonly Regex AccessdateSynonyms = new Regex(@"(?<={{\s*[Cc]it[ae][^{}]*?\|\s*)(?:\s*date\s*)?(?:retrieved(?:\s+on)?|(?:last|date) *ac+essed|access\s+date)(?=\s*=\s*)", RegexOptions.Compiled);

        private static readonly Regex UppercaseCiteFields = new Regex(@"(\{\{(?:[Cc]ite\s*(?:web|book|news|journal|paper|press release|hansard|encyclopedia)|[Cc]itation)\b\s*[^{}]*\|\s*)(\w*?[A-Z]+\w*)(?<!(?:IS[BS]N|DOI|PMID))(\s*=\s*[^{}\|]{3,})", RegexOptions.Compiled);

        private static readonly Regex CiteUrl = new Regex(@"\|\s*url\s*=\s*([^\[\]<>""\s]+)", RegexOptions.Compiled);
        private static readonly Regex CiteUrlDomain = new Regex(@"\|\s*url\s*=\s*(?:ht|f)tps?://(?:www\.)?([^\[\]<>""\s/:]+)", RegexOptions.Compiled);

        private static readonly Regex CiteFormatFieldTypo = new Regex(@"(\{\{\s*[Cc]it[^{}]*?\|\s*)(?i)(?:fprmat)(\s*=\s*)", RegexOptions.Compiled);
        private static readonly Regex WorkInItalics = new Regex(@"(\|\s*work\s*=\s*)''([^'{}\|]+)''(?=\s*(?:\||}}))", RegexOptions.Compiled);

        private static readonly Regex CiteTemplateFormatHTML = new Regex(@"\|\s*format\s*=\s*(?:HTML?|\[\[HTML?\]\]|html?)\s*(?=\||}})", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateFormatnull = new Regex(@"\|\s*format\s*=\s*(?=\||}})", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateLangEnglish = new Regex(@"\|\s*language\s*=\s*(?:[Ee]nglish)\s*(?=\||}})", RegexOptions.Compiled);
        private static readonly Regex CiteTemplatePagesPP = new Regex(@"(?<=\|\s*pages?\s*=\s*)p(?:p|gs?)?(?:\.|\b)(?:&nbsp;|\s*)(?=[^{}\|]+(?:\||}}))", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateHTMLURL = new Regex(@"\|\s*url\s*=\s*[^<>{}\s\|]+?\.(?:HTML?|html?)\s*(?:\||}})", RegexOptions.Compiled);
        private static readonly Regex CiteTemplatesJournalVolume = new Regex(@"(?<=\|\s*volume\s*=\s*)vol(?:umes?|\.)?(?:&nbsp;|:)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CiteTemplatesJournalVolumeAndIssue = new Regex(@"(?<=\|\s*volume\s*=\s*[0-9VXMILC]+?)(?:[;,]?\s*(?:no[\.:;]?|(?:numbers?|issue|iss)\s*[:;]?))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CiteTemplatesJournalIssue = new Regex(@"(?<=\|\s*issue\s*=\s*)(?:issues?|(?:nos?|iss)(?:[\.,;:]|\b)|numbers?[\.,;:]?)(?:&nbsp;)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CiteTemplatesPageRange = new Regex(@"(?<=\|\s*pages?\s*=[^\|{}=/\\]*?)\b(\d+)\s*[-—]\s*(\d+)", RegexOptions.Compiled);
        private static readonly Regex CiteTemplatesPageRangeName = new Regex(@"(\|\s*)page(\s*=\s*\d+\s*–\s*\d)", RegexOptions.Compiled);

        private static readonly Regex AccessDateYear = new Regex(@"(?<=\|\s*accessdate\s*=\s*(?:[1-3]?\d\s+" + WikiRegexes.MonthsNoGroup + @"|\s*" + WikiRegexes.MonthsNoGroup + @"\s+[1-3]?\d))(\s*)\|\s*accessyear\s*=\s*(20[01]\d)\s*(\||}})", RegexOptions.Compiled);
        private static readonly Regex AccessDayMonthDay = new Regex(@"\|\s*access(?:daymonth|month(?:day)?|year)\s*=\s*(?=\||}})", RegexOptions.Compiled);
        private static readonly Regex DateLeadingZero = new Regex(@"(?<=\|\s*(?:access|archive)?date\s*=\s*)(?:0([1-9]\s+" + WikiRegexes.MonthsNoGroup + @")|(\s*" + WikiRegexes.MonthsNoGroup + @"\s)+0([1-9],?))(\s+20[01]\d)?(\s*\||}})", RegexOptions.Compiled);
        private static readonly Regex YearInDate = new Regex(@"(\|\s*)date(\s*=\s*[12]\d{3}\s*)(?=\||}})", RegexOptions.Compiled);

        private static readonly Regex DupeFields = new Regex(@"((\|\s*([a-z\d]+)\s*=\s*([^\{\}\|]*?))\s*(?:\|.*?)?)\|\s*\3\s*=\s*([^\{\}\|]*?)\s*(\||}})", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Applies various formatting fixes to citation templates
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The updated wiki text</returns>
        public static string FixCitationTemplates(string articleText)
        {
            if (Variables.LangCode != "en")
                return articleText;

            articleText = AccessdateTypo.Replace(articleText, "$1accessdate$2");

            articleText = PublisherTypo.Replace(articleText, @"publisher$1");

            articleText = AccessdateSynonyms.Replace(articleText, "accessdate");

            // {{cite web}} needs lower case field names; two loops in case a single template has multiple uppercase fields
            // restrict to en-wiki
            // exceptionally, 'ISBN' is allowed
            int matchCount;
            int urlMatches;
            do
            {
                var matches = UppercaseCiteFields.Matches(articleText);
                matchCount = matches.Count;
                urlMatches = 0;

                foreach (Match m in matches)
                {
                    bool urlmatch = CiteUrl.Match(m.Value).Value.Contains(m.Groups[2].Value);

                    // check not within URL
                    if (!urlmatch)
                        articleText = articleText.Replace(m.Value,
                                                          m.Groups[1].Value + m.Groups[2].Value.ToLower() +
                                                          m.Groups[3].Value);
                    else
                        urlMatches++;
                }

            } while (matchCount > 0 && matchCount != urlMatches);

            articleText = CiteFormatFieldTypo.Replace(articleText, "$1format$2");

            foreach (Match m in WikiRegexes.CiteTemplate.Matches(articleText))
            {
                string newValue = m.Value;
                string templatename = Tools.GetTemplateName(newValue);

                // remove the unneeded 'format=HTML' field
                // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_.22format.3DHTML.22_in_citation_templates
                newValue = CiteTemplateFormatHTML.Replace(newValue, "");

                // remove language=English on en-wiki
                if (Variables.LangCode == "en")
                    newValue = CiteTemplateLangEnglish.Replace(newValue, "");

                // remove format= field with null value when URL is HTML page
                if (CiteTemplateHTMLURL.IsMatch(newValue))
                    newValue = CiteTemplateFormatnull.Replace(newValue, "");

                // remove italics for works field -- auto italicised by template
                newValue = WorkInItalics.Replace(newValue, "$1$2");

                // page= and pages= fields don't need p. or pp. in them when nopp not set
                if (Tools.GetTemplateParameterValue(newValue, "nopp").Length == 0 &&
                    !templatename.Equals("cite journal", StringComparison.OrdinalIgnoreCase))
                    newValue = CiteTemplatePagesPP.Replace(newValue, "");
                
                // date = YYYY --> year = YYYY; not for {{cite video}}
                if (!Regex.IsMatch(newValue, @"{{\s*[Cc]ite (?:video|podcast)\b"))
                    newValue = YearInDate.Replace(newValue, "$1year$2");

                // year = ISO date --> date = ISO date
                string TheYear = Tools.GetTemplateParameterValue(newValue, "year");
                if(WikiRegexes.ISODates.IsMatch(TheYear) || WikiRegexes.InternationalDates.IsMatch(TheYear)
                   || WikiRegexes.AmericanDates.IsMatch(TheYear))
                    newValue = Tools.RenameTemplateParameter(newValue, "year", "date");

                // remove duplicated fields, ensure the URL is not touched (may have pipes in)
                if (DupeFields.IsMatch(newValue))
                {
                    string URL = CiteUrl.Match(newValue).Value;
                    string newvaluetemp = newValue;
                    Match m2 = DupeFields.Match(newValue);

                    string val1 = m2.Groups[4].Value.Trim();
                    string val2 = m2.Groups[5].Value.Trim();
                    string firstfieldandvalue = m2.Groups[2].Value;

                    // get rid of second one if second is zero length or contained in first, provided first one not in URL
                    if (val2.Length == 0 || (val1.Length > 0 && val1.Contains(val2))
                        && !URL.Contains(firstfieldandvalue))
                        newvaluetemp = DupeFields.Replace(newValue, @"$1$6", 1);
                    // get rid of first one if first is zero length or contains second, provided second one not in URL
                    else if (val1.Length == 0 || (val2.Length > 0 && val2.Contains(val1))
                             && !URL.Contains(firstfieldandvalue))
                        newvaluetemp = newValue.Remove(m2.Groups[2].Index, m2.Groups[2].Length);

                    // ensure URL not changed
                    if (newvaluetemp != newValue && (URL.Length == 0 || newvaluetemp.Contains(URL)))
                        newValue = newvaluetemp;
                }
                
                // year=YYYY and date=...YYYY -> remove year; not for year=YYYYa
                string year = Tools.GetTemplateParameterValue(newValue, "year");
                
                if(Regex.IsMatch(year, @"^[12]\d{3}$") && Tools.GetTemplateParameterValue(newValue, "date").Contains(year))
                    newValue = Tools.RemoveTemplateParameter(newValue, "year");

                // correct volume=vol 7... and issue=no. 8 for {{cite journal}} only
                if (templatename.Equals("cite journal", StringComparison.OrdinalIgnoreCase))
                {
                    newValue = CiteTemplatesJournalVolume.Replace(newValue, "");
                    newValue = CiteTemplatesJournalIssue.Replace(newValue, "");
                    
                    if(Tools.GetTemplateParameterValue(newValue, "issue").Length ==0)
                        newValue = CiteTemplatesJournalVolumeAndIssue.Replace(newValue, @"| issue = ");
                }

                // {{cite web}} for Google books -> {{cite book}}
                if (Regex.IsMatch(templatename, @"[Cc]ite ?web") && newValue.Contains("http://books.google."))
                    newValue = Tools.RenameTemplate(newValue, templatename, "cite book");

                // remove leading zero in day of month
                newValue = DateLeadingZero.Replace(newValue, @"$1$2$3$4$5");

                if (Regex.IsMatch(templatename, @"[Cc]ite(?: ?web| book| news)"))
                {
                    // remove any empty accessdaymonth, accessmonthday, accessmonth and accessyear
                    newValue = AccessDayMonthDay.Replace(newValue, "");

                    // merge accessdate of 'D Month' or 'Month D' and accessyear of 'YYYY' in cite web
                    newValue = AccessDateYear.Replace(newValue, @" $2$1$3");
                }

                // remove accessyear where accessdate is present and contains said year
                string accessyear = Tools.GetTemplateParameterValue(newValue, "accessyear");
                if (accessyear.Length > 0 && Tools.GetTemplateParameterValue(newValue, "accessdate").Contains(accessyear))
                    newValue = Tools.RemoveTemplateParameter(newValue, "accessyear");

                // catch after any other fixes
                newValue = NoCommaAmericanDates.Replace(newValue, @"$1, $2");

                // page range should have unspaced en-dash; validate that page is range not section link
                string page = Tools.GetTemplateParameterValue(newValue, "page");
                
                if(page.Length == 0)
                    page = Tools.GetTemplateParameterValue(newValue, "pages");
                
                bool pagerangesokay = true;
                Dictionary<int, int> PageRanges = new Dictionary<int, int>();
                
                if(page.Length > 2 && !page.Contains(" to "))
                {
                    foreach (Match pagerange in CiteTemplatesPageRange.Matches(newValue))
                    {
                        string page1 = pagerange.Groups[1].Value;
                        string page2 = pagerange.Groups[2].Value;

                        // convert 350-2 into 350-352 etc.
                        if (page1.Length > page2.Length)
                            page2 = page1.Substring(0, page1.Length - page2.Length) + page2;

                        // check a valid range with difference < 999
                        if (Convert.ToInt32(page1) < Convert.ToInt32(page2) &&
                            Convert.ToInt32(page2) - Convert.ToInt32(page1) < 999)
                            pagerangesokay = true;
                        else
                            pagerangesokay = false;
                        
                        // check range doesn't overlap with another range found
                        foreach (KeyValuePair<int, int> kvp in PageRanges)
                        {
                            // check if page 1 or page 2 within existing range
                            if ((Convert.ToInt32(page1) >= kvp.Key && Convert.ToInt32(page1) <= kvp.Value) || (Convert.ToInt32(page2) >= kvp.Key && Convert.ToInt32(page2) <= kvp.Value))
                            {
                                pagerangesokay = false;
                                break;
                            }
                        }
                        
                        if(!pagerangesokay)
                            break;
                        
                        // add to dictionary of ranges found
                        PageRanges.Add(Convert.ToInt32(page1), Convert.ToInt32(page2));
                    }
                    
                    if(pagerangesokay)
                        newValue = CiteTemplatesPageRange.Replace(newValue, @"$1–$2");
                }

                // page range should use 'pages' parameter not 'page'
                newValue = CiteTemplatesPageRangeName.Replace(newValue, @"$1pages$2");
                
                // remove ordinals from dates
                if(OrdinalsInDatesInt.IsMatch(newValue))
                {
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "date", OrdinalsInDatesInt.Replace(Tools.GetTemplateParameterValue(newValue, "date"), "$1$2$3 $4"));
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "accessdate", OrdinalsInDatesInt.Replace(Tools.GetTemplateParameterValue(newValue, "accessdate"), "$1$2$3 $4"));
                }
                
                if(OrdinalsInDatesAm.IsMatch(newValue))
                {
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "date", OrdinalsInDatesAm.Replace(Tools.GetTemplateParameterValue(newValue, "date"), "$1 $2$3"));
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "accessdate", OrdinalsInDatesAm.Replace(Tools.GetTemplateParameterValue(newValue, "accessdate"), "$1 $2$3"));
                }

                // merge changes to article text
                articleText = articleText.Replace(m.Value, newValue);
            }

            return articleText;
        }

        private static readonly Regex CiteWebOrNews = new Regex(@"[Cc]ite( ?web| news)", RegexOptions.Compiled);
        private static readonly Regex PressPublishers = new Regex(@"(Associated Press|United Press International)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly List<string> WorkParameterAndAliases = new List<string>(new [] { "work", "newspaper", "journal", "periodical", "magazine" } );
        
        /// <summary>
        /// Where the publisher field is used incorrectly instead of the work field in a {{cite web}} or {{cite news}} citation
        /// convert the parameter to be 'work'
        /// Scenarios covered:
        /// * publisher == URL domain, no work= used
        /// </summary>
        /// <param name="citation">the citaiton</param>
        /// <returns>the updated citation</returns>
        public static string CitationPublisherToWork(string citation)
        {
            // only for {{cite web}} or {{cite news}}
            if (!CiteWebOrNews.IsMatch(Tools.GetTemplateName(citation)))
                return citation;
            
            string publisher = Tools.GetTemplateParameterValue(citation, "publisher");

            // nothing to do if no publisher, or publisher is a press publisher
            if (publisher.Length == 0 | PressPublishers.IsMatch(publisher))
                return citation;
            
            List<string> workandaliases = Tools.GetTemplateParametersValues(citation, WorkParameterAndAliases);

            if (string.Join("", workandaliases.ToArray()).Length == 0)
            {
                citation = Tools.RenameTemplateParameter(citation, "publisher", "work");
                citation = WorkInItalics.Replace(citation, "$1$2");
            }

            return citation;
        }

        /// <summary>
        /// The in-use date formats on Wikipedia
        /// </summary>
        public enum DateLocale { International, American, ISO, Undetermined };

        /// <summary>
        /// Determines the predominant date format in the article text (American/International), if available
        /// </summary>
        /// <param name="articleText">the article text</param>
        /// <returns>The date locale determined</returns>
        public static DateLocale DeterminePredominantDateLocale(string articleText)
        {
            return DeterminePredominantDateLocale(articleText, false, false);
        }
        
        /// <summary>
        /// Determines the predominant date format in the article text (American/International/ISO), if available
        /// </summary>
        /// <param name="articleText">the article text</param>
        /// <param name="considerISO">whether to consider ISO as a possible predominant date format</param>
        /// <returns>The date locale determined</returns>
        public static DateLocale DeterminePredominantDateLocale(string articleText, bool considerISO)
        {
            return DeterminePredominantDateLocale(articleText, considerISO, false);
        }

        /// <summary>
        /// Determines the predominant date format in the article text (American/International/ISO), if available
        /// </summary>
        /// <param name="articleText">the article text</param>
        /// <param name="considerISO">whether to consider ISO as a possible predominant date format</param>
        /// <param name="explicitonly">whether to restrict logic to look at {{use xxx dates}} template only</param>
        /// <returns>The date locale determined</returns>
        public static DateLocale DeterminePredominantDateLocale(string articleText, bool considerISO, bool explicitonly)
        {
            // first check for template telling us the preference
            string DatesT = WikiRegexes.UseDatesTemplate.Match(articleText).Groups[2].Value.ToLower();
            
            DatesT = DatesT.Replace("iso", "ymd");
            DatesT = Regex.Match(DatesT, @"(ymd|dmy|mdy)").Value;

            if (Variables.LangCode == "en" && DatesT.Length > 0)
                switch (DatesT)
            {
                case "dmy":
                    return DateLocale.International;
                case "mdy":
                    return DateLocale.American;
                case "ymd":
                    return DateLocale.ISO;
            }
            
            if(explicitonly)
                return DateLocale.Undetermined;

            // secondly count the American and International dates
            int Americans = WikiRegexes.MonthDay.Matches(articleText).Count;
            int Internationals = WikiRegexes.DayMonth.Matches(articleText).Count;

            if (considerISO)
            {
                int ISOs = WikiRegexes.ISODates.Matches(articleText).Count;

                if (ISOs > Americans && ISOs > Internationals)
                    return DateLocale.ISO;
            }

            if (Americans == Internationals)
                return DateLocale.Undetermined;
            if (Americans == 0 && Internationals > 0 || (Internationals / Americans >= 2 && Internationals > 4))
                return DateLocale.International;
            if (Internationals == 0 && Americans > 0 || (Americans / Internationals >= 2 && Americans > 4))
                return DateLocale.American;

            return DateLocale.Undetermined;
        }

        // Covered by: LinkTests.TestCanonicalizeTitle(), incomplete
        /// <summary>
        /// returns URL-decoded link target
        /// </summary>
        public static string CanonicalizeTitle(string title)
        {
            // visible parts of links may contain crap we shouldn't modify, such as refs and external links
            if (!Tools.IsValidTitle(title) || title.Contains(":/"))
                return title;

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

            if (title.StartsWith(":"))
                title = title.Remove(0, 1).Trim();

            var pos = title.IndexOf(':');
            if (pos <= 0)
                return title;

            string titlePart = title.Substring(0, pos + 1);
            foreach (var regex in WikiRegexes.NamespacesCaseInsensitive)
            {
                var m = regex.Value.Match(titlePart);
                if (!m.Success || m.Index != 0)
                    continue;

                title = Variables.Namespaces[regex.Key] + Tools.TurnFirstToUpper(title.Substring(pos + 1).Trim());
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
        private static readonly Regex AmountComparison = new Regex(@"[<>]\s*\d", RegexOptions.Compiled);

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

            if (openingBrackets == "[") // need to remove double square brackets first
                articleText = Tools.ReplaceWithSpaces(articleText, DoubleSquareBrackets);


            if (openingBrackets == "{") // need to remove double curly brackets first
                articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.NestedTemplates);

            // replace all the valid balanced bracket sets with spaces
            articleText = Tools.ReplaceWithSpaces(articleText, bracketsRegex);

            // now return the unbalanced one that's left
            int open = Regex.Matches(articleText, Regex.Escape(openingBrackets)).Count;
            int closed = Regex.Matches(articleText, Regex.Escape(closingBrackets)).Count;

            // for tags don't mark "> 50 cm" as unbalanced
            if (openingBrackets == "<" && AmountComparison.IsMatch(articleText))
                return -1;

            if (open == 0 && closed >= 1)
                return articleText.IndexOf(closingBrackets);

            if (open >= 1 && closed == 0)
                return articleText.IndexOf(openingBrackets);

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
            if (!WikiRegexes.ImageMap.IsMatch(articleText)
                && !WikiRegexes.Noinclude.IsMatch(articleText)
                && !WikiRegexes.Includeonly.IsMatch(articleText))
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

        // Covered by: LinkTests.TestSimplifyLinks()
        /// <summary>
        /// Simplifies some links in article wiki text such as changing [[Dog|Dogs]] to [[Dog]]s
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The simplified article text.</returns>
        public static string SimplifyLinks(string articleText)
        {
            foreach (Match m in WikiRegexes.PipedWikiLink.Matches(articleText))
            {
                string n = m.Value;
                string a = m.Groups[1].Value.Trim();

                string b = (Namespace.Determine(a) != Namespace.Category)
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

            return articleText;
        }

        // Covered by: LinkTests.TestStickyLinks()
        // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#Link_simplification_too_greedy_-_eating_spaces -- disabled as genfix
        /// <summary>
        /// Joins nearby words with links
        ///   e.g. "[[Russian literature|Russian]] literature" to "[[Russian literature]]"
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns>Processed wikitext</returns>
        public static string StickyLinks(string articleText)
        {
            foreach (Match m in WikiRegexes.PipedWikiLink.Matches(articleText))
            {
                string a = m.Groups[1].Value;
                string b = m.Groups[2].Value;

                if (b.Trim().Length == 0 || a.Contains(","))
                    continue;

                if (Tools.TurnFirstToLower(a).StartsWith(Tools.TurnFirstToLower(b), StringComparison.Ordinal))
                {
                    bool hasSpace = false;
                    
                    if (a.Length > b.Length)
                        hasSpace = a[b.Length] == ' ';
                    
                    string search = @"\[\[" + Regex.Escape(a) + @"\|" + Regex.Escape(b) +
                        @"\]\]" + (hasSpace ? "[ ]+" : "") + Regex.Escape(a.Remove(0,
                                                                                   b.Length + (hasSpace ? 1 : 0))) + @"\b";

                    //first char should be capitalized like in the visible part of the link
                    a = a.Remove(0, 1).Insert(0, b[0] + "");
                    articleText = Regex.Replace(articleText, search, "[[" + a + @"]]");
                }
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
            return RegexMainArticle.Replace(articleText,
                                            m => m.Groups[2].Value.Length == 0
                                            ? "{{main|" + m.Groups[1].Value + "}}"
                                            : "{{main|" + m.Groups[1].Value + "|l1=" + m.Groups[3].Value + "}}");
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
        /// <returns>The modified article text.</returns>
        public static string FixCategories(string articleText)
        {
            //Fix for http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#KeyNotFoundException
            //Seems a bit hacky? Cause it shouldn't occur...
            //if (!Variables.Namespaces.ContainsKey(Namespace.Category))
            //    return articleText;

            string cat = "[[" + Variables.Namespaces[Namespace.Category];

            // fix extra brackets: three or more at end
            articleText = Regex.Replace(articleText, @"(?<=" + Regex.Escape(cat) + @"[^\r\n\[\]{}<>]+\]\])\]+", "");
            // three or more at start
            articleText = Regex.Replace(articleText, @"\[+(?=" + Regex.Escape(cat) + @"[^\r\n\[\]{}<>]+\]\])", "");

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
        /// <param name="isMainSpace"></param>
        /// <returns>The modified article text.</returns>
        [Obsolete]
        public static string FixCategories(string articleText, bool isMainSpace)
        {
            return FixCategories(articleText);
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

            articleText = WikiRegexes.MetresFeetConversionNonBreakingSpaces.Replace(articleText, @"$1&nbsp;m");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Pagination
            // add non-breaking space after pp. abbreviation for pages.
            articleText = Regex.Replace(articleText, @"(\b[Pp]?p\.) *(?=[\dIVXCL])", @"$1&nbsp;");

            return AddBackMoreText(articleText);
        }

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
            string articleTextCleaned = WikiRegexes.UnformattedText.Replace(articleText, "");

            if (search.IsMatch(articleTextCleaned))
            {
                // extract from original article text
                Match m = search.Match(articleText);

                return m.Success ? ExtractTemplate(articleText, m) : "";
            }

            return "";
        }

        /// <summary>
        /// Finds every occurrence of a given template in article text, excludes commented out/nowiki'd templates
        /// Handles nested templates and templates with embedded HTML comments correctly.
        /// </summary>
        /// <param name="articleText">Source text</param>
        /// <param name="template">Name of template</param>
        /// <returns>List of matches found</returns>
        public static List<Match> GetTemplates(string articleText, string template)
        {
            return GetTemplates(articleText, Tools.NestedTemplateRegex(template));
        }
        
        /// <summary>
        /// Finds all templates in article text excluding commented out/nowiki'd templates.
        /// Handles nested templates and templates with embedded HTML comments correctly.
        /// </summary>
        /// <param name="articleText">Source text</param>
        /// <returns>List of matches found</returns>
        public static List<Match> GetTemplates(string articleText)
        {
            return GetTemplates(articleText, WikiRegexes.NestedTemplates);
        }
        
        /// <summary>
        /// Finds all templates in article text excluding commented out/nowiki'd templates.
        /// Handles nested templates and templates with embedded HTML comments correctly.
        /// </summary>
        /// <param name="articleText">Source text</param>
        /// <param name="search">nested template regex to use</param>
        /// <returns>List of matches found</returns>
        private static List<Match> GetTemplates(string articleText, Regex search)
        {
            List<Match> templateMatches = new List<Match>();
            string articleTextAtStart = articleText;
            
            // replace with spaces any commented out templates etc., this means index of real matches remains the same as in actual article text
            articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.UnformattedText);

            // return matches found in article text at start, provided they exist in cleaned text
            // i.e. don't include matches for commented out/nowiki'd templates
            foreach (Match m in search.Matches(articleText))
            {
                foreach (Match m2 in search.Matches(articleTextAtStart))
                {
                    if(m2.Index.Equals(m.Index))
                    {
                        templateMatches.Add(m2);
                        break;
                    }
                }
            }

            return templateMatches;
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
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs#Probably_odd_characters_being_treated_even_more_oddly
            articleText = articleText.Replace('\x2029', ' ');

            // http://en.wikipedia.org/wiki/Wikipedia:AWB/B#Line_break_insertion
            // most browsers handle Unicode line separator as whitespace, so should we
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
            string escTitle = Regex.Escape(articleTitle);

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
            HideText Hider2 = new HideText();
            HideText Hider3 = new HideText(true, true, true);
            // clean up bolded self links first
            articleText = BoldedSelfLinks(articleTitle, articleText);

            noChange = true;
            string escTitle = Regex.Escape(articleTitle);
            string escTitleNoBrackets = Regex.Escape(BracketedAtEndOfLine.Replace(articleTitle, ""));

            string articleTextAtStart = articleText;

            string zerothSection = WikiRegexes.ZerothSection.Match(articleText).Value;
            string restOfArticle = articleText.Remove(0, zerothSection.Length);

            // There's a limitation here in that we can't hide image descriptions that may be above lead sentence without hiding the self links we are looking to correct
            string zerothSectionHidden = Hider2.HideMore(zerothSection, false, false);
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

            zerothSection = Hider2.AddBackMore(zerothSectionHidden);

            if (zerothSectionHiddenOriginal != zerothSectionHidden)
            {
                noChange = false;
                return (zerothSection + restOfArticle);
            }

            // ignore date articles (date in American or international format)
            if (WikiRegexes.Dates2.IsMatch(articleTitle) || WikiRegexes.Dates.IsMatch(articleTitle))
                return articleTextAtStart;

            Regex boldTitleAlready1 = new Regex(@"'''\s*(" + escTitle + "|" + Tools.TurnFirstToLower(escTitle) + @")\s*'''");
            Regex boldTitleAlready2 = new Regex(@"'''\s*(" + escTitleNoBrackets + "|" + Tools.TurnFirstToLower(escTitleNoBrackets) + @")\s*'''");

            //if title in bold already exists in article, or page starts with something in bold, don't change anything
            if (boldTitleAlready1.IsMatch(articleText) || boldTitleAlready2.IsMatch(articleText)
                || BoldTitleAlready3.IsMatch(articleText))
                return articleTextAtStart;

            // so no self links to remove, check for the need to add bold
            string articleTextHidden = Hider3.HideMore(articleText);

            // first quick check: ignore articles with some bold in first 5% of hidemore article
            int fivepc = articleTextHidden.Length / 20;

            if (articleTextHidden.Substring(0, fivepc).Contains("'''"))
            {
                //articleText = Hider3.AddBackMore(articleTextHidden);
                return articleTextAtStart;
            }

            Regex regexBoldNoBrackets = new Regex(@"([^\[]|^)(" + escTitleNoBrackets + "|" + Tools.TurnFirstToLower(escTitleNoBrackets) + ")([ ,.:;])");

            // first try title with brackets removed
            if (regexBoldNoBrackets.IsMatch(articleTextHidden))
                articleTextHidden = regexBoldNoBrackets.Replace(articleTextHidden, "$1'''$2'''$3", 1);

            articleText = Hider3.AddBackMore(articleTextHidden);

            // check that the bold added is the first bit in bold in the main body of the article
            if (AddedBoldIsValid(articleText, escTitleNoBrackets))
            {
                noChange = false;
                return articleText;
            }

            return articleTextAtStart;
        }

        private static readonly Regex RegexFirstBold = new Regex(@"^(.*?)'''", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Checks that the bold just added to the article is the first bold in the article, and that it's within the first 5% of the HideMore article OR immediately after the infobox
        /// </summary>
        private bool AddedBoldIsValid(string articleText, string escapedTitle)
        {
            HideText Hider2 = new HideText(true, true, true);
            string articletextoriginal = articleText;
            Regex regexBoldAdded = new Regex(@"^(.*?)'''" + escapedTitle, RegexOptions.Singleline);

            int boldAddedPos = regexBoldAdded.Match(articleText).Length - Regex.Unescape(escapedTitle).Length;

            int firstBoldPos = RegexFirstBold.Match(articleText).Length;

            articleText = Hider2.HideMore(articleText);

            // was bold added in first 5% of article?
            bool inFirst5Percent = articleText.Substring(0, articleText.Length / 20).Contains("'''");

            //articleText = Hider2.AddBackMore(articleText);

            // check that the bold added is the first bit in bold in the main body of the article, and in first 5% of HideMore article
            if (inFirst5Percent && boldAddedPos <= firstBoldPos)
                return true;

            // second check: bold just after infobox
            Regex boldAfterInfobox = new Regex(WikiRegexes.InfoBox + @"\s*'''" + escapedTitle);

            return boldAfterInfobox.IsMatch(articletextoriginal);
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
                                + @")\s*:\s*" + image + @".*\]\]", RegexOptions.Singleline);

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

            articleText = FixCategories(articleText);

            if (Regex.IsMatch(articleText, @"\[\["
                              + Variables.NamespacesCaseInsensitive[Namespace.Category]
                              + Regex.Escape(newCategory) + @"[\|\]]"))
            {
                return oldText;
            }

            string cat = Tools.Newline("[[" + Variables.Namespaces[Namespace.Category] + newCategory + "]]");
            cat = Tools.ApplyKeyWords(articleTitle, cat);

            if (Namespace.Determine(articleTitle) == Namespace.Template)
                articleText += "<noinclude>" + cat + Tools.Newline("</noinclude>");
            else
                articleText += cat;

            return SortMetaData(articleText, articleTitle, false); //Sort metadata ordering so general fixes don't need to be enabled
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

                if (sort != explicitKey && !String.IsNullOrEmpty(explicitKey))
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

            MatchCollection ds = WikiRegexes.Defaultsort.Matches(articleText);
            if (ds.Count > 1 || (ds.Count == 1 && !ds[0].Value.ToUpper().Contains("DEFAULTSORT")))
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

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#AWB_didn.27t_fix_special_characters_in_a_pipe
            articleText = FixCategories(articleText);

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
                        articleText += Tools.Newline("{{DEFAULTSORT:") + Tools.FixupDefaultSort(sort) + "}}";
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

                articleText += Tools.Newline("{{DEFAULTSORT:") + sortkey + "}}";

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
        /// <param name="parseTalkPage"></param>
        /// <returns></returns>
        public static bool IsArticleAboutAPerson(string articleText, string articleTitle, bool parseTalkPage)
        {
            #if DEBUG
            if(Globals.UnitTestMode)
                parseTalkPage = false;
            #endif
            
            if (Variables.LangCode != "en"
                || articleText.Contains(@"[[Category:Multiple people]]")
                || articleText.Contains(@"[[Category:Married couples")
                || articleText.Contains(@"[[Category:Fictional")
                || Regex.IsMatch(articleText, @"\[\[Category:\d{4} animal")
                || articleText.Contains(@"[[fictional character")
                || InUniverse.IsMatch(articleText)
                || articleText.Contains(@"[[Category:Presidencies")
                || articleText.Contains(@"[[Category:Military careers")
                || CategoryCharacters.IsMatch(articleText))
                return false;
            
            if(Tools.GetTemplateParameterValue(Tools.NestedTemplateRegex(new List<string>(new [] { "Infobox musical artist", "Infobox musical artist 2", "Infobox Musical Artist", "Infobox singer", "Infobox Musician", "Infobox musician", "Music artist", "Infobox Musical Artist 2", "Infobox Musicial Artist 2", "Infobox Composer", "Infobox composer", "Infobox Musical artist", "Infobox Band" } )).Match(articleText).Value, "Background").Contains("group_or_band"))
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

            if (WikiRegexes.Persondata.Matches(articleText).Count == 1
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
                    TryGetArticleText(Variables.Namespaces[Namespace.Talk] + articleTitle).Contains(@"{{WPBiography"));
        }

        private static string TryGetArticleText(string title)
        {
            try
            {
                return Variables.MainForm.TheSession.Editor.SynchronousEditor.Clone().Open(title, false);
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// if title has diacritics, no defaultsort added yet, adds a defaultsort with cleaned up title as sort key
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="matches">If there is no change (True if no Change)</param>
        /// <returns>The article text possibly using defaultsort.</returns>
        [Obsolete]
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
            // don't add living people category if already dead, or thought to be dead
            if (WikiRegexes.DeathsOrLivingCategory.IsMatch(articleText) || WikiRegexes.LivingPeopleRegex2.IsMatch(articleText) ||
                BornDeathRegex.IsMatch(articleText) || DiedDateRegex.IsMatch(articleText))
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
            if (birthYear < (DateTime.Now.Year - 121))
                return articleText;

            // use any sortkey from 'XXXX births' category
            string catKey = birthCat.Contains("|") ? BirthsSortKey.Match(birthCat).Value : "]]";

            return articleText + "[[Category:Living people" + catKey;
        }

        private static readonly Regex PersonYearOfBirth = new Regex(@"(?<='''.{0,100}?)\( *[Bb]orn[^\)\.;]{1,150}?(?<!.*(?:[Dd]ied|&[nm]dash;|—).*)([12]?\d{3}(?: BC)?)\b[^\)]{0,200}", RegexOptions.Compiled);
        private static readonly Regex PersonYearOfDeath = new Regex(@"(?<='''.{0,100}?)\([^\(\)]*?[Dd]ied[^\)\.;]+?([12]?\d{3}(?: BC)?)\b", RegexOptions.Compiled);
        private static readonly Regex PersonYearOfBirthAndDeath = new Regex(@"^.{0,100}?'''\s*\([^\)\r\n]*?(?<![Dd]ied)\b([12]?\d{3})\b[^\)\r\n]*?(-|–|—|&[nm]dash;)[^\)\r\n]*?([12]?\d{3})\b[^\)]{0,200}", RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly Regex UncertainWordings = new Regex(@"(?:\b(about|before|after|either|prior to|around|late|[Cc]irca|between|[Bb]irth based on age as of date|\d{3,4}(?:\]\])?/(?:\[\[)?\d{1,4}|or +(?:\[\[)?\d{3,})\b|\d{3} *\?|\bca?(?:'')?\.|\bca\b|\b(bef|abt)\.|~)", RegexOptions.Compiled);
        private static readonly Regex ReignedRuledUnsure = new Regex(@"(?:\?|[Rr](?:uled|eign(?:ed)?\b)|\br\.|(chr|fl(?:\]\])?)\.|\b(?:[Ff]lo(?:urished|ruit)|active)\b)", RegexOptions.Compiled);

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
        /// <param name="parseTalkPage"></param>
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
        private static readonly Regex NotCircaTemplate = new Regex(@"{{(?![Cc]irca)[^{]*?}}", RegexOptions.Compiled);

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
        /// <param name="parseTalkPage"></param>
        /// <returns></returns>
        public static string FixPeopleCategories(string articleText, string articleTitle, bool parseTalkPage)
        {
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
            string fromInfoBox = GetInfoBoxFieldValue(zerothSection, @"(?:(?:[Yy]ear|[Dd]ate)[Oo]f[Bb]irth|[Bb]orn|birth_?date)");

            if (fromInfoBox.Length > 0 && !UncertainWordings.IsMatch(fromInfoBox))
                yearFromInfoBox = YearPossiblyWithBC.Match(fromInfoBox).Value;

            // birth
            if (!WikiRegexes.BirthsCategory.IsMatch(articleText) && (PersonYearOfBirth.Matches(zerothSection).Count == 1
                                                                     || WikiRegexes.DateBirthAndAge.IsMatch(zerothSection) || WikiRegexes.DeathDateAndAge.IsMatch(zerothSection)
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

                    if (WikiRegexes.CircaTemplate.IsMatch(birthpart))
                        alreadyUncertain = true;

                    birthpart = WikiRegexes.TemplateMultiline.Replace(birthpart, " ");

                    // check born info before any untemplated died info
                    if (!(m.Index > PersonYearOfDeath.Match(zerothSection).Index) || !PersonYearOfDeath.IsMatch(zerothSection))
                    {
                        // when there's only an approximate birth year, add the appropriate cat rather than the xxxx birth one
                        if (UncertainWordings.IsMatch(birthpart) || alreadyUncertain)
                        {
                            if (!CategoryMatch(articleText, YearOfBirthMissingLivingPeople) && !CategoryMatch(articleText, YearOfBirthUncertain))
                                articleText += Tools.Newline(@"[[Category:") + YearOfBirthUncertain + CatEnd(sort);
                        }
                        else // after removing dashes, birthpart must still contain year
                            if (!birthpart.Contains(@"?") && Regex.IsMatch(birthpart, @"\d{3,4}"))
                                yearstring = m.Groups[1].Value;
                    }
                }
                // per [[:Category:Living people]], don't apply birth category if born > 121 years ago
                // validate a YYYY date is not in the future
                if (!string.IsNullOrEmpty(yearstring) && yearstring.Length > 2
                    && (!Regex.IsMatch(yearstring, @"\d{4}") || Convert.ToInt32(yearstring) <= DateTime.Now.Year)
                    && !(articleText.Contains(@"[[Category:Living people") && Convert.ToInt32(yearstring) < (DateTime.Now.Year - 121)))
                    articleText += Tools.Newline(@"[[Category:") + yearstring + " births" + CatEnd(sort);
            }

            // scrape any infobox
            yearFromInfoBox = "";
            fromInfoBox = GetInfoBoxFieldValue(articleText, @"(?:(?:[Yy]ear|[Dd]ate)[Oo]f[Dd]eath|[Dd]ied|death_?date)");

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

                // validate a YYYY date is not in the future
                if (!string.IsNullOrEmpty(yearstring) && yearstring.Length > 2
                    && (!Regex.IsMatch(yearstring, @"^\d{4}$") || Convert.ToInt32(yearstring) <= DateTime.Now.Year))
                    articleText += Tools.Newline(@"[[Category:") + yearstring + " deaths" + CatEnd(sort);
            }

            zerothSection = NotCircaTemplate.Replace(zerothSection, " ");
            // birth and death combined
            // if not fully categorised, check it
            if (PersonYearOfBirthAndDeath.IsMatch(zerothSection) && (!WikiRegexes.BirthsCategory.IsMatch(articleText) || !WikiRegexes.DeathsOrLivingCategory.IsMatch(articleText)))
            {
                Match m = PersonYearOfBirthAndDeath.Match(zerothSection);

                string birthyear = m.Groups[1].Value;
                int birthyearint = int.Parse(birthyear);

                string deathyear = m.Groups[3].Value;
                int deathyearint = int.Parse(deathyear);

                // logical valdiation of dates
                if (birthyearint <= deathyearint && (deathyearint - birthyearint) <= 125)
                {
                    string birthpart = zerothSection.Substring(m.Index, m.Groups[2].Index - m.Index),
                    deathpart = zerothSection.Substring(m.Groups[2].Index, (m.Value.Length + m.Index) - m.Groups[2].Index);

                    if (!WikiRegexes.BirthsCategory.IsMatch(articleText))
                    {
                        if (!UncertainWordings.IsMatch(birthpart) && !ReignedRuledUnsure.IsMatch(m.Value) && !Regex.IsMatch(birthpart, @"(?:[Dd](?:ied|\.)|baptised)"))
                            articleText += Tools.Newline(@"[[Category:") + birthyear + @" births" + CatEnd(sort);
                        else
                            if (UncertainWordings.IsMatch(birthpart) && !CategoryMatch(articleText, YearOfBirthMissingLivingPeople) && !CategoryMatch(articleText, YearOfBirthUncertain))
                                articleText += Tools.Newline(@"[[Category:Year of birth uncertain") + CatEnd(sort);
                    }

                    if (!UncertainWordings.IsMatch(deathpart) && !ReignedRuledUnsure.IsMatch(m.Value) && !Regex.IsMatch(deathpart, @"[Bb](?:orn|\.)") && !Regex.IsMatch(birthpart, @"[Dd](?:ied|\.)")
                        && (!WikiRegexes.DeathsOrLivingCategory.IsMatch(articleText) || CategoryMatch(articleText, YearofDeathMissing)))
                        articleText += Tools.Newline(@"[[Category:") + deathyear + @" deaths" + CatEnd(sort);
                }
            }

            // do this check last as IsArticleAboutAPerson can be relatively slow
            if (articleText != articleTextBefore && !IsArticleAboutAPerson(articleTextBefore, articleTitle, parseTalkPage))
                return YearOfBirthMissingCategory(articleTextBefore);

            return YearOfBirthMissingCategory(articleText);
        }

        private static string CatEnd(string sort)
        {
            return ((sort.Length > 3) ? "|" + sort : "") + "]]";
        }

        private const string YearOfBirthMissingLivingPeople = "Year of birth missing (living people)",
        YearOfBirthMissing = "Year of birth missing",
        YearOfBirthUncertain = "Year of birth uncertain",
        YearofDeathMissing = "Year of death missing";

        private static readonly Regex Cat4YearBirths = new Regex(@"\[\[Category:\d{4} births(?:\s*\|[^\[\]]+)? *\]\]", RegexOptions.Compiled);
        private static readonly Regex Cat4YearDeaths = new Regex(@"\[\[Category:\d{4} deaths(?:\s*\|[^\[\]]+)? *\]\]", RegexOptions.Compiled);

        private static string YearOfBirthMissingCategory(string articleText)
        {
            if (Variables.LangCode != "en")
                return articleText;

            // if there is a 'year of birth missing' and a year of birth, remove the 'missing' category
            if (CategoryMatch(articleText, YearOfBirthMissingLivingPeople) && Cat4YearBirths.IsMatch(articleText))
                articleText = RemoveCategory(YearOfBirthMissingLivingPeople, articleText);
            else if (CategoryMatch(articleText, YearOfBirthMissing))
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
        /// Replaces legacy/deprecated language codes in interwikis with correct ones
        /// </summary>
        /// <param name="articleText"></param>
        /// <param name="noChange"></param>
        /// <returns></returns>
        public static string InterwikiConversions(string articleText, out bool noChange)
        {
            string newText = InterwikiConversions(articleText);

            noChange = (newText == articleText);

            return newText;
        }

        /// <summary>
        /// Replaces legacy/deprecated language codes in interwikis with correct ones
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns>Page text</returns>
        public static string InterwikiConversions(string articleText)
        {
            //Use proper codes
            //checking first instead of substituting blindly saves some
            //time due to low occurrence rate
            if (articleText.Contains("[[zh-tw:"))
                articleText = articleText.Replace("[[zh-tw:", "[[zh:");
            if (articleText.Contains("[[nb:"))
                articleText = articleText.Replace("[[nb:", "[[no:");
            if (articleText.Contains("[[dk:"))
                articleText = articleText.Replace("[[dk:", "[[da:");
            return articleText;
        }

        /// <summary>
        /// Converts/subst'd some deprecated templates
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        /// 
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
            if (articleText.Contains("{{msg:"))
                articleText = articleText.Replace("{{msg:", "{{");

            foreach (KeyValuePair<Regex, string> k in RegexConversion)
            {
                articleText = k.Key.Replace(articleText, k.Value);
            }
            
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Add_.7B.7Botheruse.7D.7D_and_.7B.7B2otheruses.7D.7D_in_the_supported_DABlinks
            articleText = Tools.RenameTemplate(articleText, @"2otheruses", "Two other uses");

            // {{nofootnotes}} --> {{morefootnotes}}, if some <ref>...</ref> references in article, uses regex from WikiRegexes.Refs
            if (WikiRegexes.Refs.IsMatch(articleText))
                articleText = Tools.RenameTemplate(articleText, @"nofootnotes", "morefootnotes");

            // {{unreferenced}} --> {{BLP unsourced}} if article has [[Category:Living people]]
            if (Variables.IsWikipediaEN && WikiRegexes.Unreferenced.IsMatch(articleText) && articleText.Contains(@"[[Category:Living people"))
                articleText = Tools.RenameTemplate(articleText, WikiRegexes.Unreferenced.Match(articleText).Groups[1].Value, "BLP unsourced");

            return Dablinks(articleText);
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
        public string Tagger(string articleText, string articleTitle, bool restrictOrphanTagging, out bool noChange, ref string summary)
        {
            string newText = Tagger(articleText, articleTitle, restrictOrphanTagging, ref summary);
            newText = TagUpdater(newText);

            noChange = (newText == articleText);

            return newText;
        }

        private static readonly CategoriesOnPageNoHiddenListProvider CategoryProv = new CategoriesOnPageNoHiddenListProvider();

        private readonly List<string> tagsRemoved = new List<string>();
        private readonly List<string> tagsAdded = new List<string>();

        //TODO:Needs re-write
        /// <summary>
        /// If necessary, adds/removes wikify or stub tag
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article title.</param>
        /// <param name="summary"></param>
        /// <returns>The tagged article.</returns>
        public string Tagger(string articleText, string articleTitle, bool restrictOrphanTagging, ref string summary)
        {
            // don't tag redirects/outside article namespace/no tagging changes
            if (!Namespace.IsMainSpace(articleTitle) || Tools.IsRedirect(articleText))
                return articleText;

            tagsRemoved.Clear();
            tagsAdded.Clear();

            string commentsStripped = WikiRegexes.Comments.Replace(articleText, "");
            Sorter.Interwikis(ref commentsStripped);

            // bulleted or indented text should weigh less than simple text.
            // for example, actor stubs may contain large filmographies
            string crapStripped = WikiRegexes.BulletedText.Replace(commentsStripped, "");
            int words = (Tools.WordCount(commentsStripped) + Tools.WordCount(crapStripped)) / 2;

            // remove stub tags from long articles
            if ((words > StubMaxWordCount) && WikiRegexes.Stub.IsMatch(commentsStripped))
            {
                articleText = WikiRegexes.Stub.Replace(articleText, StubChecker).Trim();
                tagsRemoved.Add("stub");
            }

            // on en wiki, remove expand template when a stub template exists
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Remove_.7B.7Bexpand.7D.7D_when_a_stub_template_exists
            if (Variables.LangCode == "en" && WikiRegexes.Stub.IsMatch(commentsStripped) &&
                WikiRegexes.Expand.IsMatch(commentsStripped))
            {
                articleText = WikiRegexes.Expand.Replace(articleText, "");
                tagsRemoved.Add("expand");
            }

            // do orphan tagging before template analysis for categorisation tags
            articleText = TagOrphans(articleText, articleTitle, restrictOrphanTagging);
            
            articleText = TagRefsIbid(articleText);
            
            articleText = TagEmptySection(articleText);

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
                      || WikiRegexes.NewUnReviewedArticle.IsMatch(m.Value)
                      || m.Value.Contains("subst")))
                {
                    summary = PrepareTaggerEditSummary();

                    return articleText;
                }
            }

            double length = articleText.Length + 1,
            linkCount = Tools.LinkCount(commentsStripped);

            int totalCategories;

            #if DEBUG
            if (Globals.UnitTestMode)
            {
                totalCategories = Globals.UnitTestIntValue;
            }
            else
                #endif
            {
                // {{stub}} --> non-hidden Category:Stubs, don't count this cat
                totalCategories = CategoryProv.MakeList(new[] { articleTitle }).Count
                    - Tools.NestedTemplateRegex("stub").Matches(commentsStripped).Count;
            }
            
            if (commentsStripped.Length <= 300 && !WikiRegexes.Stub.IsMatch(commentsStripped))
            {
                // add stub tag
                articleText += Tools.Newline("{{stub}}", 3);
                tagsAdded.Add("stub");
                commentsStripped = WikiRegexes.Comments.Replace(articleText, "");
            }

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Archive_19#AWB_problems
            // nl wiki doesn't use {{Uncategorized}} template
            if (words > 6 && totalCategories == 0
                && !WikiRegexes.Uncat.IsMatch(articleText) && Variables.LangCode != "nl")
            {
                if (WikiRegexes.Stub.IsMatch(commentsStripped))
                {
                    // add uncategorized stub tag
                    articleText += Tools.Newline("{{Uncategorized stub|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                    tagsAdded.Add("[[CAT:UNCATSTUBS|uncategorised]]");
                }
                else
                {
                    // add uncategorized tag
                    articleText += Tools.Newline("{{Uncategorized|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                    tagsAdded.Add("[[CAT:UNCAT|uncategorised]]");
                }
            }
            else if (totalCategories > 0 && WikiRegexes.Uncat.IsMatch(articleText))
            {
                articleText = WikiRegexes.Uncat.Replace(articleText, "");
                tagsRemoved.Add("uncategorised");
            }

            if (linkCount == 0 && !WikiRegexes.DeadEnd.IsMatch(articleText) && Variables.LangCode != "sv"
                && !Regex.IsMatch(WikiRegexes.ArticleIssues.Match(articleText).Value.ToLower(), @"\bdead ?end\b"))
            {
                // add dead-end tag
                articleText = "{{dead end|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                tagsAdded.Add("[[:Category:Dead-end pages|deadend]]");
            }
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_10#.7B.7BDeadend.7D.7D_gets_removed_from_categorized_pages
            // don't include categories as 'links'
            else if ((linkCount - totalCategories) > 0 && WikiRegexes.DeadEnd.IsMatch(articleText))
            {
                articleText = WikiRegexes.DeadEnd.Replace(articleText, "");
                tagsRemoved.Add("deadend");
            }

            if (linkCount < 3 && ((linkCount / length) < 0.0025) && !WikiRegexes.Wikify.IsMatch(articleText)
                && !WikiRegexes.ArticleIssues.Match(articleText).Value.ToLower().Contains("wikify"))
            {
                // add wikify tag
                articleText = "{{Wikify|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                tagsAdded.Add("[[WP:WFY|wikify]]");
            }
            else if (linkCount > 3 && ((linkCount / length) > 0.0025) &&
                     WikiRegexes.Wikify.IsMatch(articleText))
            {
                articleText = WikiRegexes.Wikify.Replace(articleText, "");
                tagsRemoved.Add("wikify");
            }

            summary = PrepareTaggerEditSummary();

            return articleText;
        }

        private static readonly WhatLinksHereAndPageRedirectsExcludingTheRedirectsListProvider WlhProv = new WhatLinksHereAndPageRedirectsExcludingTheRedirectsListProvider(MinIncomingLinksToBeConsideredAnOrphan);

        private const int MinIncomingLinksToBeConsideredAnOrphan = 3;
        private static readonly Regex Rq = Tools.NestedTemplateRegex("Rq");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="restrictOrphanTagging"></param>
        /// <returns></returns>
        private string TagOrphans(string articleText, string articleTitle, bool restrictOrphanTagging)
        {
            // check if not orphaned
            bool orphaned, orphaned2;
            int incomingLinks = 0;
            #if DEBUG
            if (Globals.UnitTestMode)
            {
                orphaned = orphaned2 = Globals.UnitTestBoolValue;
            }
            else
                #endif
            {
                try
                {
                    incomingLinks = WlhProv.MakeList(Namespace.Article, articleTitle).Count;
                    orphaned = (incomingLinks < MinIncomingLinksToBeConsideredAnOrphan);
                    orphaned2 = restrictOrphanTagging
                        ? (incomingLinks == 0)
                        : orphaned;
                }

                catch (Exception ex)
                {
                    // don't mark as orphan in case of exception
                    orphaned = orphaned2 = false;
                    ErrorHandler.CurrentPage = articleTitle;
                    ErrorHandler.Handle(ex);
                }
            }
            
            if(Variables.LangCode == "ru" && incomingLinks == 0 && Rq.Matches(articleText).Count == 1)
            {
                string rqText = Rq.Match(articleText).Value;
                if(!rqText.Contains("linkless"))
                    return articleText.Replace(rqText, rqText.Replace(@"}}", @"|linkless}}"));
            }

            // add orphan tag if applicable, and no disambig
            if (orphaned2 && !WikiRegexes.Orphan.IsMatch(articleText) && Tools.GetTemplateParameterValue(WikiRegexes.ArticleIssues.Match(articleText).Value, "orphan").Length == 0
                && !WikiRegexes.Disambigs.IsMatch(articleText))
            {
                articleText = "{{orphan|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                tagsAdded.Add("[[CAT:O|orphan]]");
            }
            else if (!orphaned && WikiRegexes.Orphan.IsMatch(articleText))
            {
                articleText = WikiRegexes.Orphan.Replace(articleText, "");
                tagsRemoved.Add("orphan");
            }
            return articleText;
        }
        
        private static readonly Regex IbidOpCitRef = new Regex(@"<\s*ref\b[^<>]*>\s*(ibid\.?|op\.?\s*cit\.?|loc\.?\s*cit\.?)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary>
        /// Tags references of 'ibid' with the {{ibid}} cleanup template, en-wiki mainspace only
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private string TagRefsIbid(string articleText)
        {
            if(Variables.LangCode == "en" && IbidOpCitRef.IsMatch(articleText) && !WikiRegexes.Ibid.IsMatch(articleText))
            {
                tagsAdded.Add("Ibid");
                return @"{{Ibid|" + WikiRegexes.DateYearMonthParameter + @"}}" + articleText;
            }
            
            return articleText;
        }
        
        /// <summary>
        /// Tags empty level-2 sections with {{Empty section}}, en-wiki mainspace only
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>The updated article text</returns>
        private string TagEmptySection(string articleText)
        {
            if(Variables.LangCode != "en")
                return articleText;
            
            string originalarticleText = "";
            int tagsadded = 0;
            
            while(!originalarticleText.Equals(articleText))
            {
                originalarticleText = articleText;
                
                int lastpos = -1;
                foreach (Match m in WikiRegexes.HeadingLevelTwo.Matches(articleText))
                {
                    // empty setion if only whitespace between two level-2 headings
                    if(lastpos > -1 && articleText.Substring(lastpos, (m.Index-lastpos)).Trim().Length == 0)
                    {
                        articleText = articleText.Insert(m.Index, @"{{Empty section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}" + "\r\n");
                        tagsadded++;
                        break;
                    }
                    
                    lastpos = m.Index+m.Length;
                }
            }
            
            if(tagsadded > 0)
                tagsAdded.Add("Empty section (" + tagsadded + ")");
            
            return articleText;
        }

        private string PrepareTaggerEditSummary()
        {
            string summary = "";
            if (tagsRemoved.Count > 0)
            {
                summary = "removed " + Tools.ListToStringCommaSeparator(tagsRemoved) + " tag" +
                    (tagsRemoved.Count == 1 ? "" : "s");
            }

            if (tagsAdded.Count > 0)
            {
                if (!string.IsNullOrEmpty(summary))
                    summary += ", ";

                summary += "added " + Tools.ListToStringCommaSeparator(tagsAdded) + " tag" +
                    (tagsAdded.Count == 1 ? "" : "s");
            }

            return summary;
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

        private static readonly Regex CommonPunctuation = new Regex(@"[""',\.;:`!\(\)\[\]\?\-–/]", RegexOptions.Compiled);
        /// <summary>
        /// For en-wiki tags redirect pages with one or more of the templates from [[Wikipedia:Template messages/Redirect pages]]
        /// </summary>
        /// <param name="articleText">the article text</param>
        /// <param name="articleTitle">the article title</param>
        /// <returns>The updated article text</returns>
        public static string RedirectTagger(string articleText, string articleTitle)
        {
            // only for untagged en-wiki redirects
            if (!Tools.IsRedirect(articleText) || !Variables.IsWikipediaEN || WikiRegexes.Template.IsMatch(articleText))
                return articleText;

            string redirecttarget = Tools.RedirectTarget(articleText);

            // skip self redirects
            if (Tools.TurnFirstToUpperNoProjectCheck(redirecttarget).Equals(Tools.TurnFirstToUpperNoProjectCheck(articleTitle)))
                return articleText;

            // {{R from modification}}
            // difference is extra/removed/changed puntuation
            if (!Tools.NestedTemplateRegex(WikiRegexes.RFromModificationList).IsMatch(articleText)
                && !CommonPunctuation.Replace(redirecttarget, "").Equals(redirecttarget) && CommonPunctuation.Replace(redirecttarget, "").Equals(CommonPunctuation.Replace(articleTitle, "")))
                return (articleText + Tools.Newline(WikiRegexes.RFromModificationString));

            // {{R from title without diacritics}}

            // title and redirect target the same if dacritics removed from redirect target
            if (redirecttarget != Tools.RemoveDiacritics(redirecttarget) && Tools.RemoveDiacritics(redirecttarget) == articleTitle
                && !Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(articleText))
                return (articleText + Tools.Newline(WikiRegexes.RFromTitleWithoutDiacriticsString));
            
            // {{R from other capitalisation}}
            if(redirecttarget.ToLower().Equals(articleTitle.ToLower())
               && !Tools.NestedTemplateRegex(WikiRegexes.RFromOtherCapitaliastionList).IsMatch(articleText))
                return (articleText + Tools.Newline(WikiRegexes.RFromOtherCapitaliastionString));

            return articleText;
        }

        private static string StubChecker(Match m)
        {
            // Replace each Regex cc match with the number of the occurrence.
            return Variables.SectStubRegex.IsMatch(m.Value) ? m.Value : "";
        }

        private static readonly Regex BotsAllow = new Regex(@"{{\s*(?:[Nn]obots|[Bb]ots)\s*\|\s*allow\s*=(.*?)}}", RegexOptions.Singleline | RegexOptions.Compiled);
        
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
            Match bot = BotsAllow.Match(articleText);

            if (bot.Success)
            {
                return
                    (Regex.IsMatch(bot.Groups[1].Value,
                                   @"(?:^|,)\s*(?:" + user.Normalize() + @"|awb)\s*(?:,|$)", RegexOptions.IgnoreCase));
            }

            return
                !Regex.IsMatch(articleText,
                               @"\{\{(nobots|bots\|(allow=none|deny=(?!none).*(" + user.Normalize() +
                               @"|awb|all)|optout=all))\}\}", RegexOptions.IgnoreCase);
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
                : WikiRegexes.InUse.IsMatch(WikiRegexes.UnformattedText.Replace(articleText, ""));
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
                WikiRegexes.ExternalLinksHeaderRegex.Match(articleText).Index;

            // get the references section: to external links or end of article, whichever is earlier
            string refsArea = externalLinksIndex > referencesIndex
                ? articleText.Substring(referencesIndex, (externalLinksIndex - referencesIndex))
                : articleText.Substring(referencesIndex);

            return (WikiRegexes.BareExternalLink.IsMatch(refsArea));
        }

        #endregion
    }
}