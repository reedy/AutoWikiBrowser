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
            RegexUnicode.Add(new Regex("&(ndash|mdash|minus|times|lt|gt|nbsp|thinsp|shy|lrm|rlm|[Pp]rime|ensp|emsp|#x2011|#8201|#8239);", RegexOptions.Compiled), "&amp;$1;");
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
            RegexConversion.Add(new Regex(@"\{\{(?:Template:)?(2cc|2LAdisambig|2LCdisambig|2LC)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{2CC}}");
            RegexConversion.Add(new Regex(@"\{\{(?:Template:)?(3cc|3L[WC]|Tla|Tla-dab|TLA-disambig|TLAdisambig)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{3CC}}");
            RegexConversion.Add(new Regex(@"\{\{(?:Template:)?(4cc|4L[AWC])\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{4CC}}");
            RegexConversion.Add(new Regex(@"\{\{(?:Template:)?(Bio-dab|Hndisambig)", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Hndis");

            RegexConversion.Add(new Regex(@"\{\{(?:Template:)?(Prettytable|Prettytable100)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{subst:Prettytable}}");
            RegexConversion.Add(new Regex(@"\{\{(?:[Tt]emplate:)?(PAGENAMEE?\}\}|[Ll]ived\||[Bb]io-cats\|)", RegexOptions.Compiled), "{{subst:$1");

            // articleissues with one issue -> single issue tag (e.g. {{articleissues|cleanup=January 2008}} to {{cleanup|date=January 2008}} etc.)
            RegexConversion.Add(new Regex(@"\{\{[Aa]rticle ?issues\s*\|\s*([^\|{}=]{3,}?)\s*(=\s*\w{3,10}\s+20\d\d\s*\}\})", RegexOptions.Compiled), "{{$1|date$2");

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.7B.7Bcommons.7CCategory:XXX.7D.7D_.3E_.7B.7Bcommonscat.7CXXX.7D.7D
            RegexConversion.Add(new Regex(@"\{\{[Cc]ommons\|\s*[Cc]ategory:\s*([^{}]+?)\s*\}\}", RegexOptions.Compiled), @"{{commons cat|$1}}");
        }

        private static readonly Dictionary<Regex, string> RegexUnicode = new Dictionary<Regex, string>();
        private static readonly Dictionary<Regex, string> RegexConversion = new Dictionary<Regex, string>();
        private static readonly Dictionary<Regex, string> RegexTagger = new Dictionary<Regex, string>();

        private readonly HideText hider = new HideText();
        private readonly HideText hiderHideExtLinksImages = new HideText(true, true, true);
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
                if (metaDataSorter == null) metaDataSorter = new MetaDataSorter(this);
                return metaDataSorter;
            }
        }

        #endregion

        #region General Parse

        public string HideText(string ArticleText)
        {
            return hider.Hide(ArticleText);
        }

        public string AddBackText(string ArticleText)
        {
            return hider.AddBack(ArticleText);
        }

        public string HideMoreText(string ArticleText, bool HideOnlyTargetOfWikilink)
        {
            return hiderHideExtLinksImages.HideMore(ArticleText, HideOnlyTargetOfWikilink);
        }

        public string HideMoreText(string ArticleText)
        {
            return hiderHideExtLinksImages.HideMore(ArticleText);
        }

        public string AddBackMoreText(string ArticleText)
        {
            return hiderHideExtLinksImages.AddBackMore(ArticleText);
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

        private static readonly Regex regexFixDates0 = new Regex(@"(the |later? |early |mid-)(\[?\[?[12][0-9][0-9]0\]?\]?)'s(\]\])?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private static readonly Regex regexFixDates1 = new Regex("(January|February|March|April|May|June|July|August|September|October|November|December) ([1-9][0-9]?)(?:st|nd|rd|th)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private static readonly Regex regexFixDates2 = new Regex("([1-9][0-9]?)(?:st|nd|rd|th) (January|February|March|April|May|June|July|August|September|October|November|December)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
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
            ArticleText = regexFixDates0.Replace(ArticleText, "$1$2s$3");

            //ArticleText = regexFixDates1.Replace(ArticleText, "$1 $2");
            //ArticleText = regexFixDates2.Replace(ArticleText, "$1 $2");
            return ArticleText;
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

        private static readonly Regex OutofOrderRefs1 = new Regex(@"(<ref>[^<>]+</ref>)(\s*)(<ref\s+name\s*=\s*(""?[^<>""]+?""?)\s*(?:\/\s*|>[^<>]+</ref)>)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex OutofOrderRefs2 = new Regex(@"(<ref\s+name\s*=\s*(""?[^<>""]+?""?)\s*(?:\/\s*|>[^<>]+</ref)>)(\s*)(<ref\s+name\s*=\s*(""?[^<>""]+?""?)\s*(?:\/\s*|>[^<>]+</ref)>)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex OutofOrderRefs3 = new Regex(@"(?<=\s*(?:\/\s*|>[^<>]+</ref)>)(<ref\s+name\s*=\s*(""?[^<>""]+?""?)\s*(?:\/\s*|>[^<>]+</ref)>)(\s*)(<ref\s+name\s*=\s*(""?[^<>""]+?""?)\s*(?:\/\s*|>[^<>]+</ref)>)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Reorders references so that they appear in numerical order
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
                    int Ref1Index = Regex.Match(ArticleText, @"(?i)<ref\s+name\s*=\s*" + Regex.Escape(m.Groups[4].Value) + @"\s*(?:\/\s*|>[^<>]+</ref)>").Index;
                    int Ref2Index = ArticleText.IndexOf(Ref1);

                    if (Ref1Index < Ref2Index && Ref2Index > 0)
                    {
                        string Whitespace = m.Groups[2].Value;
                        string Ref2 = m.Groups[3].Value;
                        ArticleText = ArticleText.Replace(Ref1 + Whitespace + Ref2, Ref2 + Whitespace + Ref1);
                    }
                }

                foreach (Match m in OutofOrderRefs2.Matches(ArticleText))
                {
                    int Ref1Index = Regex.Match(ArticleText, @"(?i)<ref\s+name\s*=\s*" + Regex.Escape(m.Groups[2].Value) + @"\s*(?:\/\s*|>[^<>]+</ref)>").Index;
                    int Ref2Index = Regex.Match(ArticleText, @"(?i)<ref\s+name\s*=\s*" + Regex.Escape(m.Groups[5].Value) + @"\s*(?:\/\s*|>[^<>]+</ref)>").Index;

                    if (Ref1Index > Ref2Index && Ref1Index > 0)
                    {
                        string Ref1 = m.Groups[1].Value;
                        string Ref2 = m.Groups[4].Value;
                        string Whitespace = m.Groups[3].Value;
                        ArticleText = ArticleText.Replace(Ref1 + Whitespace + Ref2, Ref2 + Whitespace + Ref1);
                    }
                }

                foreach (Match m in OutofOrderRefs3.Matches(ArticleText))
                {
                    int Ref1Index = Regex.Match(ArticleText, @"(?i)<ref\s+name\s*=\s*" + Regex.Escape(m.Groups[2].Value) + @"\s*(?:\/\s*|>[^<>]+</ref)>").Index;
                    int Ref2Index = Regex.Match(ArticleText, @"(?i)<ref\s+name\s*=\s*" + Regex.Escape(m.Groups[5].Value) + @"\s*(?:\/\s*|>[^<>]+</ref)>").Index;

                    if (Ref1Index > Ref2Index && Ref1Index > 0)
                    {
                        string Ref1 = m.Groups[1].Value;
                        string Ref2 = m.Groups[4].Value;
                        string Whitespace = m.Groups[3].Value;
                        ArticleText = ArticleText.Replace(Ref1 + Whitespace + Ref2, Ref2 + Whitespace + Ref1);
                    }
                }

                if (ArticleTextBefore.Equals(ArticleText))
                    break;
            }

            return (ArticleText);
        }

        // Covered by: FormattingTests.TestMdashes()
        /// <summary>
        /// Replaces hyphens and em-dashes with en-dashes, per [[WP:DASH]]
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string Mdashes(string ArticleText)
        {
            ArticleText = HideMoreText(ArticleText);

            //string months = "(" + string.Join("|", Variables.MonthNames) + ")";

            ArticleText = Regex.Replace(ArticleText, @"(pages\s*=\s*|pp\.?\s*)(\d+\s*)(?:-|—|&mdash;|&#8212;)(\s*\d+)", @"$1$2–$3", RegexOptions.IgnoreCase);

            ArticleText = Regex.Replace(ArticleText, @"([1-9]?\d\s*)(?:-|—|&mdash;|&#8212;)(\s*[1-9]?\d)(\s+|&nbsp;)(years|months|weeks|days|hours|minutes|seconds|kg|mg|kb|km|[Gk]?Hz|miles|mi\.|%)\b", @"$1–$2$3$4");

            //ArticleText = Regex.Replace(ArticleText, @"(\[?\[?" + months + @"\ [1-3]?\d\]?\]?,\ \[?\[?[1-2]\d{3}\]?\]?)\s*(?:-|—|&mdash;|&#8212;)\s*(\[?\[?" + months + @"\ [1-3]?\d\]?\]?,\ \[?\[?[1-2]\d{3}\]?\]?)", @"$1–$3");

            ArticleText = Regex.Replace(ArticleText, @"(\$[1-9]?\d{1,3}\s*)(?:-|—|&mdash;|&#8212;)(\s*\$?[1-9]?\d{1,3})", @"$1–$2");

            ArticleText = Regex.Replace(ArticleText, @"([01]?\d:[0-5]\d\s*([AP]M)\s*)(?:-|—|&mdash;|&#8212;)(\s*[01]?\d:[0-5]\d\s*([AP]M))", @"$1–$3", RegexOptions.IgnoreCase);

            ArticleText = Regex.Replace(ArticleText, @"([Aa]ge[sd])\s([1-9]?\d\s*)(?:-|—|&mdash;|&#8212;)(\s*[1-9]?\d)", @"$1 $2–$3");

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

            Regex OrdinalsInDatesAm = new Regex(@"\b" + WikiRegexes.months + @"\s+([0-3]?\d)(?:st|nd|rd|th)\b(?<!\b[Tt]he\s+\w{3,10}\s+([0-3]?\d)(?:st|nd|rd|th)\b)");
            Regex OrdinalsInDatesInt = new Regex(@"(?:\b([0-3]?\d)(?:st|nd|rd|th)(\s*(?:to|and|.|&.dash;)\s*))?\b([0-3]?\d)(?:st|nd|rd|th)\s+" + WikiRegexes.months + @"\b(?<!\b[Tt]he\s+(?:[0-3]?\d)(?:st|nd|rd|th)\s+\w{3,10})");

            Regex DateLeadingZerosAm = new Regex(@"\b" + WikiRegexes.months + @"\s+0([1-9])" + @"\b");
            Regex DateLeadingZerosInt = new Regex(@"\b" + @"0([1-9])\s+" + WikiRegexes.months + @"\b");

            // hide items in quotes etc., though this may also hide items within infoboxes etc.
            ArticleText = HideMoreText(ArticleText);

            ArticleText = OfBetweenMonthAndYear.Replace(ArticleText, "$1 $2");

            // don't apply if article title has a month in it (e.g. [[6th of October City]])
            if (!Regex.IsMatch(ArticleTitle, @"\b" + months + @"\b"))
            {
                ArticleText = OrdinalsInDatesAm.Replace(ArticleText, "$1 $2");
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

            //fix bullet points
            ArticleText = Regex.Replace(ArticleText, "^([\\*#]+) ", "$1", RegexOptions.Multiline);
            ArticleText = Regex.Replace(ArticleText, "^([\\*#]+)", "$1 ", RegexOptions.Multiline);

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
        private static readonly Regex DoubleBracketAtStartOfExternalLink = new Regex(@"\[(\[http:/(?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex DoubleBracketAtEndOfExternalLink = new Regex(@"(\[http:/(?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\])\](?!\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex DoubleBracketAtEndOfExternalLinkWithinImage = new Regex(@"(\[http:/(?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!)))\](?=\]{3})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
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
        private static readonly Regex PipedExternalLink = new Regex(@"(\[\w+://[^][<>\""\s]*?)\|''", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex MissingColonInHttpLink = new Regex(@"([\s\[>=](?:ht|f))tp//?:?(\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SingleTripleSlashInHttpLink = new Regex(@"([\s\[>=](?:ht|f))tp:(?:/|///)(\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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

            ArticleText = MultipleHttpInLink.Replace(ArticleText, "$1$2");

            ArticleText = PipedExternalLink.Replace(ArticleText, "$1 ''");

            if (!Regex.IsMatch(ArticleText, "\\[\\[[Ii]mage:[^]]*http"))
            {
                ArticleText = SyntaxRegex4.Replace(ArticleText, "[[$1]]");
                ArticleText = SyntaxRegex5.Replace(ArticleText, "[[$1]]");
            }

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

            if (SectionLinkWhitespace.IsMatch(ArticleText))
                ArticleText = SectionLinkWhitespace.Replace(ArticleText, "$1$2");

            return ArticleText;
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
        /// Fixes link syntax
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public static string FixLinks(string ArticleText, out bool NoChange)
        {
            StringBuilder sb = new StringBuilder(ArticleText, (ArticleText.Length * 11) / 10);

            foreach (Match m in WikiRegexes.WikiLink.Matches(ArticleText))
            {
                if (m.Groups[1].Value.Length > 0)
                {
                    string y = m.Value.Replace(m.Groups[1].Value, CanonicalizeTitle(m.Groups[1].Value));

                    if (y != m.Value) sb = sb.Replace(m.Value, y);
                }
            }

            NoChange = (sb.ToString() == ArticleText);

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

        private static readonly Regex regexMainArticle = new Regex(@"^:?'{0,5}Main article:\s?'{0,5}\[\[([^\|\[\]]*?)(\|([^\[\]]*?))?\]\]\.?'{0,5}\.?\s*?(?=($|[\r\n]))", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

        // Covered by: FixMainArticleTests
        /// <summary>
        /// Fixes instances of ''Main Article: xxx'' to use {{main|xxx}}
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string FixMainArticle(string ArticleText)
        {
            return regexMainArticle.Match(ArticleText).Groups[2].Value.Length == 0
                       ? regexMainArticle.Replace(ArticleText, "{{main|$1}}")
                       : regexMainArticle.Replace(ArticleText, "{{main|$1|l1=$3}}");
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
            articleTextSubstring = BulletExternalHider.AddBackMore(articleTextSubstring);
            ArticleText += articleTextSubstring;

            return ArticleText;
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
        /// regex that matches every template, for GetTemplate
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
                if (string.IsNullOrEmpty(gtn))
                    return setting;

                return gtn;
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
            string ArticleTextAtStart = ArticleText;
            string escTitle = Regex.Escape(ArticleTitle);
            string escTitleNoBrackets = Regex.Escape(Regex.Replace(ArticleTitle, @" \(.*?\)$", ""));

            NoChange = true;

            string ArticleTextHidden = HideMoreText(ArticleText);

            // first quick check: ignore articles with some bold in first 5% of hidemore article
            int fivepc = ArticleTextHidden.Length / 20;
            //ArticleText5.Length
            if (ArticleTextHidden.Substring(0, fivepc).Contains("'''"))
                return ArticleTextAtStart;

            // ignore date articles (date in American or international format)
            if (WikiRegexes.Dates2.IsMatch(ArticleTitle) || WikiRegexes.Dates.IsMatch(ArticleTitle))
                return ArticleTextAtStart;

            // remove any self-links, but not other links with different capitaliastion e.g. [[Foo]] vs [[FOO]]
            // note, removal of self links in iteslf will not cause this method to return a 'change'
            ArticleText = Regex.Replace(ArticleText, @"\[\[\s*" + escTitle + @"\s*\]\]", ArticleTitle);

            // remove piped self links by leaving target
            ArticleText = Regex.Replace(ArticleText, @"\[\[\s*" + escTitle + @"\s*\|([^\]]+)\]\]", "$1");
            ArticleText = Regex.Replace(ArticleText, @"\[\[\s*" + Tools.TurnFirstToLower(escTitle) + @"\s*(?:\]\]|\|)", Tools.TurnFirstToLower(ArticleTitle));

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

        static readonly Regex regexFirstBold = new Regex(@"^(.*?)'''", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Checks that the bold just added to the article is the first bold in the article, and that it's within the first 5% of the HideMore article
        /// </summary>
        private bool AddedBoldIsValid(string ArticleText, string escapedTitle)
        {
            Regex regexBoldAdded = new Regex(@"^(.*?)'''" + escapedTitle, RegexOptions.Singleline);

            int BoldAddedPos = regexBoldAdded.Match(ArticleText).Length - Regex.Unescape(escapedTitle).Length;

            int FirstBoldPos = regexFirstBold.Match(ArticleText).Length;

            ArticleText = HideMoreText(ArticleText);

            // was bold added in first 5% of article?
            bool inFirst5Percent = ArticleText.Substring(0, ArticleText.Length / 20).Contains("'''");

            // check that the bold added is the first bit in bold in the main body of the article, and in first 5% of HideMore article
            return inFirst5Percent && BoldAddedPos <= FirstBoldPos;
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

            // we don't need to process that {{Lifetime}} crap
            MatchCollection ds = WikiRegexes.Defaultsort.Matches(ArticleText);
            if (ds.Count > 1 || (ds.Count == 1 && !ds[0].Value.ToUpper().Contains("DEFAULTSORT"))) return ArticleText;

            if (WikiRegexes.Lifetime.IsMatch(ArticleText))
                return ArticleText;

            ArticleText = TalkPages.TalkPageHeaders.FormatDefaultSort(ArticleText);

            // match again, after normalisation
            ds = WikiRegexes.Defaultsort.Matches(ArticleText);
            if (ds.Count > 1)
                return testText;

            string catregex = @"\[\[\s*" + Variables.NamespacesCaseInsensitive[Namespace.Category] +
                              @"\s*(.*?)\s*(?:|\|([^\|\]]*))\s*\]\]";

            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#AWB_didn.27t_fix_special_characters_in_a_pipe
            ArticleText = FixCategories(ArticleText);

            if (ds.Count == 0)
            {
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

                if (allsame && matches > 1 && !string.IsNullOrEmpty(sort))
                {
                    if (sort.Length > 4 && // So that this doesn't get confused by sort keys of "*", " ", etc.
                        !sort.StartsWith(" "))
                    // MW bug: DEFAULTSORT doesn't treat leading spaces the same way as categories do
                    {
                        ArticleText = Regex.Replace(ArticleText, catregex, "[["
                            + Variables.Namespaces[Namespace.Category] + "$1]]");

                        if (sort != ArticleTitle)
                            ArticleText = ArticleText + "\r\n{{DEFAULTSORT:" + Tools.FixupDefaultSort(sort) + "}}";
                    }
                }
            }
            else // already has DEFAULTSORT
            {
                string s = Tools.FixupDefaultSort(ds[0].Groups[1].Value).Trim();
                if (s != ds[0].Groups[1].Value && s.Length > 0)
                    ArticleText = ArticleText.Replace(ds[0].Value, "{{DEFAULTSORT:" + s + "}}");
            }

            if (ds.Count == 1)
            {
                //Removes any explicit keys that are case insensitively the same as the default sort (To help tidy up on pages that already have defaultsort)
                foreach (Match m in Regex.Matches(ArticleText, catregex))
                {
                    string explicitKey = m.Groups[2].Value;
                    if (explicitKey.Length == 0)
                        continue;

                    if (string.Compare(explicitKey, ds[0].Groups["key"].Value, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        ArticleText = ArticleText.Replace(m.Value,
                            "[[" + Variables.Namespaces[Namespace.Category] + m.Groups[1].Value + "]]");
                    }
                }
            }

            NoChange = (testText == ArticleText);
            return ArticleText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange"></param>
        /// <returns></returns>
        public static string LivingPeople(string ArticleText, out bool NoChange)
        {
            string testText = ArticleText;

            ArticleText = LivingPeople(ArticleText);

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        private static readonly Regex LivingPeopleRegex1 = new Regex("\\[\\[ ?Category ?:[ _]?([0-9]{1,2}[ _]century[ _]deaths|[0-9s]{4,5}[ _]deaths|Disappeared[ _]people|Living[ _]people|Year[ _]of[ _]death[ _]missing|Possibly[ _]living[ _]people)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex LivingPeopleRegex2 = new Regex(@"\{\{(Template:)?(Recent ?death|Recentlydeceased)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex LivingPeopleRegex3 = new Regex("\\[\\[ ?Category ?:[ _]?([0-9]{4})[ _]births(\\|.*?)?\\]\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // NOT covered
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string LivingPeople(string ArticleText)
        {
            if (LivingPeopleRegex1.IsMatch(ArticleText))
                return ArticleText;

            if (LivingPeopleRegex2.IsMatch(ArticleText))
                return ArticleText;

            Match m = LivingPeopleRegex3.Match(ArticleText);

            if (!m.Success)
                return ArticleText;

            string birthCat = m.Value;
            int birthYear = int.Parse(m.Groups[1].Value);

            if (birthYear < 1910)
                return ArticleText;

            string catKey = birthCat.Contains("|") ? Regex.Match(birthCat, "\\|.*?\\]\\]").Value : "]]";

            return ArticleText + "[[Category:Living people" + catKey;
        }

        /// <summary>
        /// Converts/subst'd some deprecated templates
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public static string Conversions(string ArticleText, out bool NoChange)
        {
            string testText = ArticleText;
            ArticleText = Conversions(ArticleText);

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        // NOT covered
        /// <summary>
        /// Converts/subst'd some deprecated templates
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The new article text.</returns>
        public static string Conversions(string ArticleText)
        {
            //Use proper codes
            //checking first instead of substituting blindly saves some
            //time due to low occurence rate
            if (ArticleText.Contains("[[zh-tw:")) ArticleText = ArticleText.Replace("[[zh-tw:", "[[zh:");
            if (ArticleText.Contains("[[nb:")) ArticleText = ArticleText.Replace("[[nb:", "[[no:");
            if (ArticleText.Contains("[[dk:")) ArticleText = ArticleText.Replace("[[dk:", "[[da:");

            if (ArticleText.Contains("{{msg:")) ArticleText = ArticleText.Replace("{{msg:", "{{");

            foreach (KeyValuePair<Regex, string> k in RegexConversion)
            {
                ArticleText = k.Key.Replace(ArticleText, k.Value);
            }

            // {{nofootnotes}} --> {{morefootnotes}}, if some <ref>...</ref> references in article, uses regex from WikiRegexes.Refs
            if (WikiRegexes.Refs.IsMatch(ArticleText))
                ArticleText = Regex.Replace(ArticleText, @"{{nofootnotes}}", "{{morefootnotes}}", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            return ArticleText;
        }

        // NOT covered
        /// <summary>
        /// Substitutes some user talk templates
        /// </summary>
        /// <param name="TalkPageText">The wiki text of the talk page.</param>
        /// <param name="TalkPageTitle"></param>
        /// <param name="userTalkTemplatesRegex"></param>
        /// <returns>The new text.</returns>
        public static string SubstUserTemplates(string TalkPageText, string TalkPageTitle, Regex userTalkTemplatesRegex)
        {
            if (userTalkTemplatesRegex == null) return TalkPageText;

            TalkPageText = TalkPageText.Replace("{{{subst", "REPLACE_THIS_TEXT");
            Dictionary<Regex, string> regexes = new Dictionary<Regex, string>();

            regexes.Add(userTalkTemplatesRegex, "{{subst:$2}}");
            TalkPageText = Tools.ExpandTemplate(TalkPageText, TalkPageTitle, regexes, true);

            TalkPageText = Regex.Replace(TalkPageText, " \\{\\{\\{2\\|\\}\\}\\}", "");
            TalkPageText = TalkPageText.Replace("REPLACE_THIS_TEXT", "{{{subst");
            return TalkPageText;
        }

        //Covered by TaggerTests
        /// <summary>
        /// If necessary, adds/removes wikify or stub tag
        /// </summary>
        public string Tagger(string ArticleText, string ArticleTitle, out bool NoChange, ref string Summary, bool addTags, bool removeTags)
        {
            if (!addTags && !removeTags)
            {
                NoChange = true;
                return ArticleText;
            }

            string testText = ArticleText;
            ArticleText = Tagger(ArticleText, ArticleTitle, ref Summary, addTags, removeTags);
            ArticleText = TagUpdater(ArticleText);

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        private static readonly CategoriesOnPageNoHiddenListProvider categoryProv = new CategoriesOnPageNoHiddenListProvider();
        private static readonly WhatLinksHereListProvider wlhProv = new WhatLinksHereListProvider(1);

        //TODO:Needs re-write
        /// <summary>
        /// If necessary, adds/removes wikify or stub tag
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">The article title.</param>
        /// <param name="Summary"></param>
        /// <param name="addTags"></param>
        /// <param name="removeTags"></param>
        /// <returns>The tagged article.</returns>
        public string Tagger(string ArticleText, string ArticleTitle, ref string Summary, bool addTags, bool removeTags)
        {
            // don't tag redirects/outside article namespace/no tagging changes
            if (Tools.IsRedirect(ArticleText) || !Namespace.IsMainSpace(ArticleTitle)
                || (!addTags && !removeTags))
                return ArticleText;

            string commentsStripped = WikiRegexes.Comments.Replace(ArticleText, "");
            Sorter.interwikis(ref commentsStripped);

            // bulleted or indented text should weigh less than simple text.
            // for example, actor stubs may contain large filmographies
            string crapStripped = WikiRegexes.BulletedText.Replace(commentsStripped, "");
            int words = (Tools.WordCount(commentsStripped) + Tools.WordCount(crapStripped)) / 2;

            // remove stub tags from long articles
            if (removeTags && (words > StubMaxWordCount) && WikiRegexes.Stub.IsMatch(commentsStripped))
            {
                ArticleText = WikiRegexes.Stub.Replace(ArticleText, stubChecker).Trim();
                Summary += ", removed Stub tag";
            }

            // skip article if contains any template except for stub templates
            foreach (Match m in WikiRegexes.Template.Matches(ArticleText))
            {
                if (!(WikiRegexes.Stub.IsMatch(m.Value)
                    || WikiRegexes.Uncat.IsMatch(m.Value)
                    || WikiRegexes.DeadEnd.IsMatch(m.Value)
                    || WikiRegexes.Wikify.IsMatch(m.Value)
                    || WikiRegexes.Orphan.IsMatch(m.Value)
                    || WikiRegexes.ReferenceList.IsMatch(m.Value)
                    || m.Value.Contains("subst")))
                    return ArticleText;
            }

            double length = ArticleText.Length + 1,
                   linkCount = Tools.LinkCount(commentsStripped);

            int totalCategories = (!Globals.UnitTestMode)
                                      ? categoryProv.MakeList(new[] { ArticleTitle }).Count
                                      : Globals.UnitTestIntValue;

            // TODO: update link when it's archived
            // http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser#AWB_problems
            // nl wiki doesn't use {{Uncategorized}} template
            if (addTags && words > 6 && totalCategories == 0
                && !WikiRegexes.Uncat.IsMatch(ArticleText) && Variables.LangCode != LangCodeEnum.nl)
            {
                if (WikiRegexes.Stub.IsMatch(commentsStripped))
                {
                    // add uncategorized stub tag
                    ArticleText +=
                        "\r\n\r\n{{Uncategorizedstub|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
                    Summary += ", added [[:Category:Uncategorized stubs|uncategorised]] tag";
                }
                else
                {
                    // add uncategorized tag
                    ArticleText += "\r\n\r\n{{Uncategorized|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
                    Summary += ", added [[:Category:Category needed|uncategorised]] tag";
                }
            }
            else if (removeTags && totalCategories > 0
                     && WikiRegexes.Uncat.IsMatch(ArticleText))
            {
                ArticleText = WikiRegexes.Uncat.Replace(ArticleText, "");
                Summary += ", removed uncategorised tag";
            }

            if (addTags && commentsStripped.Length <= 300 && !WikiRegexes.Stub.IsMatch(commentsStripped))
            {
                // add stub tag
                ArticleText = ArticleText + "\r\n\r\n\r\n{{stub}}";
                Summary += ", added stub tag";
            }

            if (addTags && linkCount == 0 && !WikiRegexes.DeadEnd.IsMatch(ArticleText))
            {
                // add dead-end tag
                ArticleText = "{{deadend|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + ArticleText;
                Summary += ", added [[:Category:Dead-end pages|deadend]] tag";
            }
            else if (removeTags && linkCount > 0 && WikiRegexes.DeadEnd.IsMatch(ArticleText))
            {
                ArticleText = WikiRegexes.DeadEnd.Replace(ArticleText, "");
                Summary += ", removed deadend tag";
            }

            if (addTags && linkCount < 3 && ((linkCount / length) < 0.0025) && !WikiRegexes.Wikify.IsMatch(ArticleText))
            {
                // add wikify tag
                ArticleText = "{{Wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + ArticleText;
                Summary += ", added [[:Category:Articles that need to be wikified|wikify]] tag";
            }
            else if (removeTags && linkCount > 3 && ((linkCount / length) > 0.0025) &&
                     WikiRegexes.Wikify.IsMatch(ArticleText))
            {
                ArticleText = WikiRegexes.Wikify.Replace(ArticleText, "");
                Summary += ", removed wikify tag";
            }

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
                    foreach (Article a in wlhProv.MakeList(0, ArticleTitle))
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
                    ErrorHandler.CurrentPage = ArticleTitle;
                    ErrorHandler.Handle(ex);
                }
            }

            // add orphan tag if applicable
            if (addTags && orphaned && !WikiRegexes.Orphan.IsMatch(ArticleText) && !WikiRegexes.OrphanArticleIssues.IsMatch(ArticleText))
            {
                ArticleText = "{{orphan|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + ArticleText;
                Summary += ", added [[:Category:Orphaned articles|orphan]] tag";
            }
            else if (removeTags && !orphaned && WikiRegexes.Orphan.IsMatch(ArticleText))
            {
                ArticleText = WikiRegexes.Orphan.Replace(ArticleText, "");
                Summary += ", removed orphan tag";
            }

            return ArticleText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText"></param>
        /// <returns></returns>
        public static string TagUpdater(string ArticleText)
        {
            // update by-date tags
            foreach (KeyValuePair<Regex, string> k in RegexTagger)
            {
                ArticleText = k.Key.Replace(ArticleText, k.Value);
            }
            return ArticleText;
        }

        private static string stubChecker(Match m)
        {
            // Replace each Regex cc match with the number of the occurrence.
            return Regex.IsMatch(m.Value, Variables.SectStub) ? m.Value : "";
        }

        // Covered by UtilityFunctionTests.NoBotsTests()
        /// <summary>
        /// checks if a user is allowed to edit this article
        /// using {{bots}} and {{nobots}} tags
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="user">Name of this user</param>
        /// <returns>true if you can edit, false otherwise</returns>
        public static bool CheckNoBots(string ArticleText, string user)
        {
            return
                !Regex.Match(ArticleText,
                             @"\{\{(nobots|bots\|(allow=none|deny=(?!none).*(" + user.Normalize() +
                             @"|awb|all).*|optout=all))\}\}", RegexOptions.IgnoreCase).Success;
        }

        private static readonly Regex dupeLinks1 = new Regex("\\[\\[([^\\]\\|]+)\\|([^\\]]*)\\]\\](.*[.\n]*)\\[\\[\\1\\|\\2\\]\\]", RegexOptions.Compiled);
        private static readonly Regex dupeLinks2 = new Regex("\\[\\[([^\\]]+)\\]\\](.*[.\n]*)\\[\\[\\1\\]\\]", RegexOptions.Compiled);

        /// <summary>
        /// Remove some of the duplicated wikilinks from the article text
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string RemoveDuplicateWikiLinks(string ArticleText)
        {
            ArticleText = dupeLinks1.Replace(ArticleText, "[[$1|$2]]$3$2");
            return dupeLinks2.Replace(ArticleText, "[[$1]]$2$1");
        }

        public static readonly Regex ExtToInt1 = new Regex(@"/\w+:\/\/secure\.wikimedia\.org\/(\w+)\/(\w+)\//", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static readonly Regex ExtToInt2 = new Regex(@"/http:\/\/(\w+)\.(\w+)\.org\/wiki\/([^#{|}\[\]]*).*REMOVEME/i", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static readonly Regex ExtToInt3 = new Regex(@"/http:\/\/(\w+)\.(\w+)\.org\/.*?title=([^#&{|}\[\]]*).*REMOVEME/i", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static readonly Regex ExtToInt4 = new Regex(@"/[^\n]*?\[\[([^[\]{|}]+)[^\n]*REMOVEME/g", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static readonly Regex ExtToInt5 = new Regex(@"/^ *(w:|wikipedia:|)(en:|([a-z\-]+:)) *REMOVEME/i", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static readonly Regex ExtToInt6 = new Regex(@"/^ *(?:wikimedia:(m)eta|wikimedia:(commons)|(wikt)ionary|wiki(?:(n)ews|(b)ooks|(q)uote|(s)ource|(v)ersity))(:[a-z\-]+:)/i", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Covered by UtilityFunctionTests.ExternalURLToInternalLink(), incomplete
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText"></param>
        /// <returns></returns>
        public static string ExternalURLToInternalLink(string ArticleText)
        {
            // Convert from the escaped UTF-8 byte code into Unicode
            ArticleText = HttpUtility.UrlDecode(ArticleText);
            // Convert secure URLs into non-secure equivalents (note the secure system is considered a 'hack')
            ArticleText = ExtToInt1.Replace(ArticleText, "http://$2.$1.org/");
            // Convert http://lang.domain.org/wiki/ into interwiki format
            ArticleText = ExtToInt2.Replace(ArticleText, "$2:$1:$3");
            // Scripts paths (/w/index.php?...) into interwiki format
            ArticleText = ExtToInt3.Replace(ArticleText, "$2:$1:$3");
            // Remove [[brackets]] from link
            ArticleText = ExtToInt4.Replace(ArticleText, "$1");
            // '_' -> ' ' and hard coded home wiki
            ArticleText = ExtToInt5.Replace(ArticleText, "$3");
            // Use short prefix form (wiktionary:en:Wiktionary:Main Page -> wikt:en:Wiktionary:Main Page)
            return ExtToInt6.Replace(ArticleText, "$1$2$3$4$5$6$7$8$9");
        }
        #endregion

        #region Property checkers
        /// <summary>
        /// Checks if the article has a stub template
        /// </summary>
        public static bool HasStubTemplate(string ArticleText)
        {
            return WikiRegexes.Stub.IsMatch(ArticleText);
        }

        /// <summary>
        /// Checks if the article is classible as a 'Stub'
        /// </summary>
        public static bool IsStub(string ArticleText)
        {
            return (HasStubTemplate(ArticleText) || ArticleText.Length < StubMaxWordCount);
        }

        /// <summary>
        /// Checks if the article has an Infobox (en wiki)
        /// </summary>
        public static bool HasInfobox(string ArticleText)
        {
            if (Variables.LangCode != LangCodeEnum.en)
                return false;

            ArticleText = WikiRegexes.Nowiki.Replace(ArticleText, "");
            ArticleText = WikiRegexes.Comments.Replace(ArticleText, "");

            return WikiRegexes.Infobox.IsMatch(ArticleText);
        }

        /// <summary>
        /// Check if article has an 'inusetag'
        /// </summary>
        public static bool IsInUse(string ArticleText)
        {
            return (Variables.LangCode != LangCodeEnum.en) ? false : Variables.InUse.IsMatch(WikiRegexes.Comments.Replace(ArticleText, ""));
        }

        /// <summary>
        /// Check if the article contains a sic template or bracketed wording, indicating the presence of a deliberate typo
        /// </summary>
        public static bool HasSicTag(string ArticleText)
        {
            return WikiRegexes.SicTag.IsMatch(ArticleText);
        }

        /// <summary>
        /// Check if the article contains a {{nofootnotes}} or {{morefootnotes}} template but has 5+ <ref>...</ref> references
        /// </summary>
        public static bool HasMorefootnotesAndManyReferences(string ArticleText)
        {
            return (WikiRegexes.MoreNoFootnotes.IsMatch(WikiRegexes.Comments.Replace(ArticleText, "")) && WikiRegexes.Refs.Matches(ArticleText).Count > 4);
        }
        /// <summary>
        /// Check if the article uses cite references but has no recognised template to display the references; only for en-wiki
        /// </summary>
        public static bool IsMissingReferencesDisplay(string ArticleText)
        {
            if (Variables.LangCode != LangCodeEnum.en)
                return false;

            return !WikiRegexes.ReferencesTemplate.IsMatch(ArticleText) && Regex.IsMatch(ArticleText, @"</ref>");
        }

        #endregion
    }
}
