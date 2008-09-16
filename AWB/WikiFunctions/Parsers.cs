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
using System.Configuration;
using System.Collections;
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
        {//default constructor
            MakeRegexes();
        }

        /// <summary>
        /// Re-organises the Person Data, stub/disambig templates, categories and interwikis
        /// </summary>
        /// <param name="StubWordCount">The number of maximum number of words for a stub.</param>
        public Parsers(int StubWordCount, bool AddHumanKey)
        {
            StubMaxWordCount = StubWordCount;
            addCatKey = AddHumanKey;
            MakeRegexes();
        }

        private void MakeRegexes()
        {
            //look bad if changed
            RegexUnicode.Add(new Regex("&(ndash|mdash|minus|times|lt|gt|nbsp|thinsp|shy|lrm|rlm|[Pp]rime|ensp|emsp);", RegexOptions.Compiled), "&amp;$1;");
            //IE6 does like these
            RegexUnicode.Add(new Regex("&#(705|803|596|620|699|700|8652|9408|9848|12288|160|61|x27|39);", RegexOptions.Compiled), "&amp;#$1;");

            //Decoder doesn't like these
            RegexUnicode.Add(new Regex("&#(x109[0-9A-Z]{2});", RegexOptions.Compiled), "&amp;#$1;");
            RegexUnicode.Add(new Regex("&#((?:277|119|84|x1D|x100)[A-Z0-9a-z]{2,3});", RegexOptions.Compiled), "&amp;#$1;");
            RegexUnicode.Add(new Regex("&#(x12[A-Za-z0-9]{3});", RegexOptions.Compiled), "&amp;#$1;");

            //interfere with wiki syntax
            RegexUnicode.Add(new Regex("&#(0?13|126|x5D|x5B|x7b|x7c|x7d|0?9[13]|0?12[345]|0?0?3[92]);", RegexOptions.Compiled | RegexOptions.IgnoreCase), "&amp;#$1;");

            RegexTagger.Add(new Regex("\\{\\{(template:)?(wikify|wikify-date|wfy|wiki)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?(Clean ?up|CU|Clean|Tidy)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Cleanup|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?(Linkless|Orphan)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Orphan|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?(Uncategori[sz]ed|Uncat|Classify|Category needed|Catneeded|categori[zs]e|nocats?)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Uncategorized|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?(Unreferenced|add references|cite[ -]sources?|cleanup-sources?|needs? references|no sources|no references?|not referenced|references|sources|unref|Unreferencedsect|unsourced)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Unreferenced|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?(Trivia|Trivia2|Too much trivia|Trivia section|Cleanup-trivia|Toomuchtrivia)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Trivia|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?(deadend|DEP)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{deadend|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");
            RegexTagger.Add(new Regex("\\{\\{(template:)?(copyedit|g(rammar )?check|copy-edit|cleanup-copyedit|cleanup-english)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{copyedit|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}");

            RegexConversion.Add(new Regex("\\{\\{(?:Template:)?(Dab|Disamb|Disambiguation)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Disambig}}");
            RegexConversion.Add(new Regex("\\{\\{(?:Template:)?(2cc|2LAdisambig|2LCdisambig|2LC)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{2CC}}");
            RegexConversion.Add(new Regex("\\{\\{(?:Template:)?(3cc|3LW|Tla|Tla-dab|TLA-disambig|TLAdisambig|3LC)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{3CC}}");
            RegexConversion.Add(new Regex("\\{\\{(?:Template:)?(4cc|4LW|4LA|4LC)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{4CC}}");
            RegexConversion.Add(new Regex("\\{\\{(?:Template:)?(Bio-dab|Hndisambig)", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{Hndis");

            RegexConversion.Add(new Regex("\\{\\{(?:Template:)?(Prettytable|Prettytable100|Pt)\\}\\}", RegexOptions.IgnoreCase | RegexOptions.Compiled), "{{subst:Prettytable}}");
            RegexConversion.Add(new Regex("\\{\\{(?:[Tt]emplate:)?(PAGENAMEE?\\}\\}|[Ll]ived\\||[Bb]io-cats\\|)", RegexOptions.Compiled), "{{subst:$1");
        }

        private Dictionary<Regex, string> RegexUnicode = new Dictionary<Regex, string>();
        private Dictionary<Regex, string> RegexConversion = new Dictionary<Regex, string>();
        private Dictionary<Regex, string> RegexTagger = new Dictionary<Regex, string>();

        private HideText hider = new HideText();
        private string testText = "";
        public static int StubMaxWordCount = 500;

        /// <summary>
        /// Sort interwiki link order
        /// </summary>
        public bool sortInterwikiOrder
        {
            get { return boolInterwikiOrder; }
            set { boolInterwikiOrder = value; }
        }

        private bool boolInterwikiOrder = true;

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
        public bool addCatKey
        {
            get { return boolAddCatKey; }
            set { boolAddCatKey = value; }
        }

        private bool boolAddCatKey;

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

        // NOT covered
        /// <summary>
        /// Re-organises the Person Data, stub/disambig templates, categories and interwikis
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">The article title.</param>
        /// <param name="sortWikis">True, sort interwiki order per pywiki bots, false keep current order.</param>
        /// <returns>The re-organised text.</returns>
        public string SortMetaData(string ArticleText, string ArticleTitle)
        {
            if (Variables.Project <= ProjectEnum.species)
                return Sorter.Sort(ArticleText, ArticleTitle);
            else
                return ArticleText;
        }

        private readonly Regex regexFixDates0 = new Regex(@"(the |later? |early |mid-)(\[?\[?[12][0-9][0-9]0\]?\]?)'s(\]\])?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private readonly Regex regexFixDates1 = new Regex("(January|February|March|April|May|June|July|August|September|October|November|December) ([1-9][0-9]?)(?:st|nd|rd|th)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private readonly Regex regexFixDates2 = new Regex("([1-9][0-9]?)(?:st|nd|rd|th) (January|February|March|April|May|June|July|August|September|October|November|December)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex regexHeadings0 = new Regex("(== ?)(see also:?|related topics:?|related articles:?|internal links:?|also see:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex regexHeadings1 = new Regex("(== ?)(external link[s]?|external site[s]?|outside link[s]?|web ?link[s]?|exterior link[s]?):?( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private readonly Regex regexHeadings2 = new Regex("(== ?)(external link:?|external site:?|web ?link:?|exterior link:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex regexHeadings3 = new Regex("(== ?)(referen[sc]e:?)(s? ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex regexHeadings4 = new Regex("(== ?)(source:?)(s? ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex regexHeadings5 = new Regex("(== ?)(further readings?:?)( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex regexHeadings6 = new Regex("(== ?)(Early|Personal|Adult|Later) Life( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex regexHeadings7 = new Regex("(== ?)(Current|Past|Prior) Members( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex regexHeadings8 = new Regex(@"^(=+)'''(.*?)'''(=+)\s*?(\r)?$", RegexOptions.Multiline | RegexOptions.Compiled);
        private readonly Regex regexHeadings9 = new Regex("(== ?)track listing( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex regexHeadings10 = new Regex("(== ?)Life and Career( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex regexHeadingsCareer = new Regex("(== ?)([a-zA-Z]+) Career( ?==)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly Regex RegexBadHeader = new Regex("^(={1,4} ?(about|description|overview|definition|profile|(?:general )?information|background|intro(?:duction)?|summary|bio(?:graphy)?) ?={1,4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Fix ==See also== and similar section common errors.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string FixHeadings(string ArticleText, string ArticleTitle, out bool NoChange)
        {
            testText = ArticleText;
            ArticleText = FixHeadings(ArticleText, ArticleTitle);

            NoChange = (testText == ArticleText);

            return ArticleText.Trim();
        }

        // NOT covered
        /// <summary>
        /// Fix ==See also== and similar section common errors. Removes unecessary introductary headings.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixHeadings(string ArticleText, string ArticleTitle)
        {
            ArticleText = Regex.Replace(ArticleText, "^={1,4} ?" + Regex.Escape(ArticleTitle) + " ?={1,4}", "", RegexOptions.IgnoreCase);
            ArticleText = RegexBadHeader.Replace(ArticleText, "");

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

            return ArticleText;
        }

        // Covered by: LinkTests.FixDates()
        /// <summary>
        /// Fix date and decade formatting errors.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixDates(string ArticleText)
        {
            HideText hidetext = new HideText();
            ArticleText = hidetext.HideMore(ArticleText);
            {
                ArticleText = FixDatesRaw(ArticleText);

                //Remove 2 or more <br />'s
                //This piece's existance here is counter-intuitive, but it requires HideMore()
                //and I don't want to call this slow function yet another time --MaxSem
                ArticleText = SyntaxRemoveBr.Replace(ArticleText, "\r\n");
                ArticleText = SyntaxRemoveParagraphs.Replace(ArticleText, "\r\n\r\n");

            }
            ArticleText = hidetext.AddBackMore(ArticleText);
            return ArticleText;
        }

        static Regex DiedDateRegex = new Regex(@"('''[^']+'''\s*\()d.(\s+\[*(?:(?:January|February|March|April|May|June|July|August|September|October|November|December)\s+0?([1-3]?[0-9])|0?([1-3]?[0-9])\s*(?:January|February|March|April|May|June|July|August|September|October|November|December))?\]*\s*\[*[1-2]?[0-9][0-9][0-9]\]*\)\s*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static Regex DOBRegex = new Regex(@"('''[^']+'''\s*\()b.(\s+\[*(?:(?:January|February|March|April|May|June|July|August|September|October|November|December)\s+0?([1-3]?[0-9])|0?([1-3]?[0-9])\s*(?:January|February|March|April|May|June|July|August|September|October|November|December))?\]*\s*\[*[1-2]?[0-9][0-9][0-9]\]*\)\s*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static Regex BornDeathRegex = new Regex(@"('''[^']+'''\s*\()(?:[Bb]orn|b\.)\s+(\[*(?:(?:January|February|March|April|May|June|July|August|September|October|November|December)\s+0?(?:[1-3]?[0-9])|0?(?:[1-3]?[0-9])\s*(?:January|February|March|April|May|June|July|August|September|October|November|December))?\]*,?\s*\[*[1-2]?[0-9][0-9][0-9]\]*)\s*(.|&.dash;)\s*(?:[Dd]ied|d\.)\s+(\[*(?:(?:January|February|March|April|May|June|July|August|September|October|November|December)\s+0?(?:[1-3]?[0-9])|0?(?:[1-3]?[0-9])\s*(?:January|February|March|April|May|June|July|August|September|October|November|December))\]*,?\s*\[*[1-2]?[0-9][0-9][0-9]\]*\)\s*)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Replace b. and d. for born/died
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixLivingThingsRelatedDates(string ArticleText)
        {
            ArticleText = DiedDateRegex.Replace(ArticleText, "$1died$2"); //date of death
            ArticleText = DOBRegex.Replace(ArticleText, "$1born$2"); //date of birth
            return BornDeathRegex.Replace(ArticleText, "$1$2 – $4"); //birth and death, with +? lazy regex match
        }

        // Covered by: LinkTests.FixDates()
        /// <summary>
        /// Fixes date and decade formatting errors.
        /// Unlike FixDates(), requires wikitext processed with HideMore()
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixDatesRaw(string ArticleText)
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

        // Covered by: FootnotesTests.TestFixReferenceListTags()
        private string ReflistMatchEvaluator(Match m)
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

        private static readonly Regex DivStart = new Regex(@"<div\b.*?>", RegexOptions.Compiled);
        private static readonly Regex DivEnd = new Regex(@"< ?/ ?div\b.*?>", RegexOptions.Compiled);

        // Covered by: FootnotesTests.TestFixReferenceListTags()
        /// <summary>
        /// Replaces various old reference tag formats, with the new {{reflist}}
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article</param>
        /// <returns></returns>
        public string FixReferenceListTags(string ArticleText)
        {
            return ReferenceListTags.Replace(ArticleText, new MatchEvaluator(ReflistMatchEvaluator));
        }

        private static Regex EmptyReferences = new Regex(@"<ref\s+name=[""]?([^<>""]*)[""]?\s*>[\s]*< ?/ ?ref ?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
        public string FixSyntax(string ArticleText, out bool NoChange)
        {
            testText = ArticleText;
            ArticleText = FixSyntax(ArticleText);

            NoChange = (testText == ArticleText);
            return ArticleText;
        }

        private static readonly Regex SyntaxRegex1 = new Regex(@"\[\[http:\/\/([^][]*?)\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex2fix = new Regex("\\[http:\\/\\/([^][]*?)\\]\\]\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex2 = new Regex("\\[http:\\/\\/([^][]*?)\\]\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex3 = new Regex("\\[\\[http:\\/\\/(.*?)\\]\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex4 = new Regex(@"\[\[([^][]*?)\](?=[^\]]*?(?:$|\[|\n))", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex5 = new Regex(@"(?<=(?:^|\]|\n)[^\[]*?)\[([^][]*?)\]\](?!\])", RegexOptions.Compiled);

        private static readonly Regex SyntaxRegex6 = new Regex("\\[?\\[image:(http:\\/\\/.*?)\\]\\]?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex7 = new Regex("\\[\\[ (.*)?\\]\\]", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex8 = new Regex("\\[\\[([A-Za-z]*) \\]\\]", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex9 = new Regex("\\[\\[(.*)?_#(.*)\\]\\]", RegexOptions.Compiled);

        private static readonly Regex SyntaxRegexTemplate = new Regex("(\\{\\{[\\s]*)[Tt]emplate:(.*?\\}\\})", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex SyntaxRegex11 = new Regex(@"^([#\*:;]+.*?) *<[/\\]?br ?[/\\]?> *\r\n", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex SyntaxRegexItalic = new Regex("< ?i ?>(.*?)< ?/ ?i ?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexBold = new Regex("< ?b ?>(.*?)< ?/ ?b ?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Matches <p> tags only if current line does not start from ! or | (indicator of table cells)
        private static readonly Regex SyntaxRemoveParagraphs = new Regex(@"(?<!^[!\|].*)</? ?[Pp]>", RegexOptions.Multiline | RegexOptions.Compiled);
        // same shit for <br>
        private static readonly Regex SyntaxRemoveBr = new Regex(@"(?<!^[!\|].*)(<br[\s/]*> *){2,}", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        //private static readonly Regex InOpenBrackets = new Regex(@"\[\[[^\]]{,100}", RegexOptions.RightToLeft | RegexOptions.Compiled);

        private static readonly Regex MutlipleHttpInLink = new Regex("(http:?/+)+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex PipedExternalLink = new Regex(@"(\[\w+://[^][<>\""\s]*?)\|''", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Covered by: LinkTests.TestFixSyntax(), incomplete
        /// <summary>
        /// Fixes and improves syntax (such as html markup)
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixSyntax(string ArticleText)
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

            //can cause problems
            //ArticleText = Regex.Replace(ArticleText, "^<[Hh]2>(.*?)</[Hh]2>", "==$1==", RegexOptions.Multiline);
            //ArticleText = Regex.Replace(ArticleText, "^<[Hh]3>(.*?)</[Hh]3>", "===$1===", RegexOptions.Multiline);
            //ArticleText = Regex.Replace(ArticleText, "^<[Hh]4>(.*?)</[Hh]4>", "====$1====", RegexOptions.Multiline);

            //fix uneven bracketing on links
            ArticleText = SyntaxRegex1.Replace(ArticleText, "[http://$1]");
            ArticleText = SyntaxRegex2fix.Replace(ArticleText, "[http://$1]]]]");
            ArticleText = SyntaxRegex2.Replace(ArticleText, "[http://$1]");
            ArticleText = SyntaxRegex3.Replace(ArticleText, "[http://$1]");

            ArticleText = MutlipleHttpInLink.Replace(ArticleText, "http://");
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
                return System.Web.HttpUtility.UrlDecode(title.Replace("+", "%2B"))
                    .Trim(new char[] { '_' });
            }
            else return s;
        }

        private static bool IsHex(byte b)
        {
            return ((b >= '0' && b <= '9') || (b >= 'A' && b <= 'F'));
        }

        private static byte DecodeHex(byte a, byte b)
        {
            string s = new string(new char[] { (char)a, (char)b });

            return byte.Parse(s, System.Globalization.NumberStyles.HexNumber);
        }

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
        private static readonly Regex LinkWhitespace4 = new Regex(@"\[\[([^\]]{1,30}) \]\] ", RegexOptions.Compiled);
        private static readonly Regex LinkWhitespace5 = new Regex(@"\[\[([^\]]{1,30}) \]\](?=\w)", RegexOptions.Compiled);

        private static readonly Regex DateLinkWhitespace1 = new Regex(@"\b(\[\[\d\d? (?:January|February|March|April|May|June|July|August|September|October|November|December)\]\]),? {0,2}(\[\[\d{1,4}\]\])\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex DateLinkWhitespace2 = new Regex(@"\b(\[\[(?:January|February|March|April|May|June|July|August|September|October|November|December) \d\d?\]\]),? {0,2}(\[\[\d{1,4}\]\])\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Covered by LinkTests.TestFixLinkWhitespace()
        /// <summary>
        /// Fix leading, trailing and middle spaces in Wikilinks
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article</param>
        /// <returns>The modified article text.</returns>
        public static string FixLinkWhitespace(string ArticleText)
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

            return ArticleText;
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

            string y = "";

            //string cat = "[[" + Variables.Namespaces[14];

            foreach (Match m in WikiRegexes.WikiLink.Matches(ArticleText))
            {
                if (m.Groups[1].Value.Length > 0) y = m.Value.Replace(m.Groups[1].Value, CanonicalizeTitle(m.Groups[1].Value));
                else continue;

                if (y != m.Value) sb = sb.Replace(m.Value, y);
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
            if (trim) return title.Trim();
            else return title;
        }

        // Covered by: UtilityFunctionTests.IsCorrectEditSummary()
        /// <summary>
        /// returns true if given string has matching double square brackets
        /// </summary>
        public static bool IsCorrectEditSummary(string s)
        {
            bool res = true;

            int pos = s.IndexOf("[[");
            while (pos >= 0)
            {
                s = s.Remove(0, pos);

                if (res)
                    pos = s.IndexOf("]]");
                else
                    pos = s.IndexOf("[[");

                res = !res;
            }
            return res;
        }

        /// <summary>
        /// Simplifies some links in article wiki text such as changing [[Dog|Dogs]] to [[Dog]]s
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The simplified article text.</returns>
        public string LinkSimplifier(string ArticleText, out bool NoChange)
        {
            testText = ArticleText;
            ArticleText = SimplifyLinks(ArticleText);

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        // Covered by: LinkTests.TestSimplifyLinks()
        /// <summary>
        /// Simplifies some links in article wiki text such as changing [[Dog|Dogs]] to [[Dog]]s
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The simplified article text.</returns>
        public static string SimplifyLinks(string ArticleText)
        {
            string n = "", a = "", b = "", k = "";

            try
            {
                foreach (Match m in WikiRegexes.PipedWikiLink.Matches(ArticleText))
                {
                    n = m.Value;
                    a = m.Groups[1].Value;
                    b = m.Groups[2].Value;

                    if (b.Trim().Length == 0) continue;

                    if (a == b || Tools.TurnFirstToLower(a) == b)
                    {
                        k = WikiRegexes.PipedWikiLink.Replace(n, "[[$2]]");
                        ArticleText = ArticleText.Replace(n, k);
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
                        k = "[[" + b.Substring(0, a.Length) + "]]" + b.Substring(a.Length);
                        ArticleText = ArticleText.Replace(n, k);
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

        private static Regex regexMainArticle = new Regex(@"^:?'{0,5}Main article:\s?'{0,5}\[\[([^\|\[\]]*?)(\|([^\[\]]*?))?\]\]\.?'{0,5}\.?\s*?(?=($|[\r\n]))", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

        // Covered by: FixMainArticleTests
        /// <summary>
        /// Fixes instances of ''Main Article: xxx'' to use {{main|xxx}}
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string FixMainArticle(string ArticleText)
        {
            if (regexMainArticle.Match(ArticleText).Groups[2].Value.Length == 0)
                return regexMainArticle.Replace(ArticleText, "{{main|$1}}");
            else
                return regexMainArticle.Replace(ArticleText, "{{main|$1|l1=$3}}");
        }

        static Regex emptyTemplate = new Regex(@"{{[|\s]*}}", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // NOT covered
        /// <summary>
        /// Removes Empty Links and Template Links
        /// Will Cater for [[]], [[Image:]], [[:Category:]], [[Category:]] and {{}}
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string FixEmptyLinksAndTemplates(string ArticleText)
        {
            string cat = Variables.Namespaces[14];
            string img = Variables.Namespaces[6];

            Regex emptyLink = new Regex("\\[\\[(:?" + cat + "|" + img + "|)(|" + img + "|" + cat + "|.*?)\\]\\]", RegexOptions.IgnoreCase);

            string trim;

            foreach (Match link in emptyLink.Matches(ArticleText))
            {
                trim = link.Groups[2].Value.Trim();
                if (string.IsNullOrEmpty(trim) || trim == "|" + img || trim == "|" + cat || trim == "|")
                    ArticleText = ArticleText.Replace("[[" + link.Groups[1].Value + link.Groups[2].Value + "]]", "");
            }

            if (emptyTemplate.Match(ArticleText).Success)
            {
                ArticleText = emptyTemplate.Replace(ArticleText, "");
            }
            return ArticleText;
        }

        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string BulletExternalLinks(string ArticleText, out bool NoChange)
        {
            testText = ArticleText;
            ArticleText = BulletExternalLinks(ArticleText);

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        // Covered by: LinkTests.TestBulletExternalLinks()
        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string BulletExternalLinks(string ArticleText)
        {
            int intStart = 0;
            string articleTextSubstring = "";

            Match m = Regex.Match(ArticleText, @"=\s*(?:external)?\s*links\s*=", RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

            if (!m.Success)
                return ArticleText;

            intStart = m.Index;

            articleTextSubstring = ArticleText.Substring(intStart);
            ArticleText = ArticleText.Substring(0, intStart);
            HideText ht = new HideText(false, true, false);
            articleTextSubstring = ht.HideMore(articleTextSubstring);
            articleTextSubstring = Regex.Replace(articleTextSubstring, "(\r\n|\n)?(\r\n|\n)(\\[?http)", "$2* $3");
            articleTextSubstring = ht.AddBackMore(articleTextSubstring);
            ArticleText += articleTextSubstring;

            return ArticleText;
        }

        // Covered by: LinkTests.TestFixCategories()
        /// <summary>
        /// Fix common spacing/capitalisation errors in categories
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixCategories(string ArticleText)
        {
            Regex catregex = new Regex(@"\[\[[\s_]*" + Variables.NamespacesCaseInsensitive[14] + @"[\s_]*([^\|]*?)(|\|.*?)\]\]");
            string cat = "[[" + Variables.Namespaces[14];
            string x = "";

            foreach (Match m in catregex.Matches(ArticleText))
            {
                if (!Tools.IsValidTitle(m.Groups[1].Value)) continue;
                x = cat + Tools.TurnFirstToUpper(CanonicalizeTitleRaw(m.Groups[1].Value, false).Trim()) + m.Groups[2].Value + "]]";
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
            Regex imgregex = new Regex(@"\[\[\s*?" + Variables.NamespacesCaseInsensitive[6] + @"\s*([^\|\]]*?)(.*?)\]\]");
            string img = "[[" + Variables.Namespaces[6];
            string x = "";

            foreach (Match m in imgregex.Matches(ArticleText))
            {
                x = img + CanonicalizeTitle(m.Groups[1].Value).Trim() + m.Groups[2].Value.Trim() + "]]";
                ArticleText = ArticleText.Replace(m.Value, x);
            }

            return ArticleText;
        }

        private static Regex Temperature = new Regex(@"([º°](&nbsp;|)|(&deg;|&ordm;)(&nbsp;|))\s*([CcFf])([^A-Za-z])", RegexOptions.Compiled);

        // NOT covered
        /// <summary>
        /// Fix bad Temperatures
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixTemperatures(string ArticleText)
        {
            foreach (Match m in Temperature.Matches(ArticleText))
                ArticleText = ArticleText.Replace(m.ToString(), "°" + m.Groups[5].Value.ToUpper() + m.Groups[6].Value);
            return ArticleText;
        }

        /// <summary>
        /// regex that matches every template, for GetTemplate
        /// </summary>
        public static string EveryTemplate = @"[^\|\{\}]+";

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
            Regex search = new Regex(@"(\{\{\s*" + Template + @"\s*)(?:\||\})",
                RegexOptions.Singleline);

            Match m = search.Match(ArticleText);

            if (!m.Success) return "";

            return ExtractTemplate(ArticleText, m);
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
        public static string GetTemplateName(string setting, bool fromSetting)
        {
            if (fromSetting)
            {
                setting = setting.Trim();
                if (string.IsNullOrEmpty(setting)) return "";

                string gtn = GetTemplateName(setting).Trim();
                if (string.IsNullOrEmpty(gtn))
                    return setting;
                else
                    return gtn;
            }
            else
                return GetTemplateName(setting);
        }

        private static Regex EmptyComments = new Regex(@"<!--[^\S\r\n]*-->", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        //Covered by: UtilityFunctionTests.RemoveEmptyComments()
        /// <summary>
        /// Removes comments with nothing/only whitespace between tags
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns>The modified article text (removed empty comments).</returns>
        public static string RemoveEmptyComments(string ArticleText)
        {
            return EmptyComments.Replace(ArticleText, "");
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
            testText = ArticleText;
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

        // Covered by: BoldTitleTests, incomplete
        /// <summary>
        /// '''Emboldens''' the first occurence of the article title, if not already bold
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">The title of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string BoldTitle(string ArticleText, string ArticleTitle, out bool NoChange)
        {
            //ignore date articles
            if (WikiRegexes.Dates2.IsMatch(ArticleTitle))
            {
                NoChange = true;
                return ArticleText;
            }

            string escTitle = Regex.Escape(ArticleTitle).Replace(@"\ ", "[ _]");

            //remove self links first
            Regex tregex = new Regex("\\[\\[(" + Tools.CaseInsensitive(escTitle) + ")\\]\\]");
            if (!ArticleText.Contains("'''"))
            {
                ArticleText = tregex.Replace(ArticleText, "'''$1'''", 1);
            }
            else
            {
                ArticleText = ArticleText.Replace("[[" + ArticleTitle + "]]", ArticleTitle);
                ArticleText = ArticleText.Replace("[[" + Tools.TurnFirstToLower(ArticleTitle) + "]]", Tools.TurnFirstToLower(ArticleTitle));
            }

            if (Regex.IsMatch(ArticleText, "^(\\[\\[|\\*|:)") || Regex.IsMatch(ArticleText, "''' ?" + escTitle + " ?'''", RegexOptions.IgnoreCase))
            {
                NoChange = true;
                return ArticleText;
            }

            ArticleText = hider.HideMore(ArticleText);

            escTitle = Regex.Replace(ArticleTitle, " \\(.*?\\)$", "");
            escTitle = Regex.Escape(escTitle);

            Regex regexBold = new Regex("([^\\[]|^)(" + escTitle + ")([ ,.:;])", RegexOptions.IgnoreCase);

            string strSecondHalf = "";
            if (ArticleText.Length > 80)
            {
                strSecondHalf = ArticleText.Substring(80);
                ArticleText = ArticleText.Substring(0, 80);
            }

            if (ArticleText.Contains("'''"))
            {
                ArticleText = ArticleText + strSecondHalf;
                ArticleText = hider.AddBackMore(ArticleText);
                NoChange = true;
                return ArticleText;
            }

            if (regexBold.IsMatch(ArticleText))
            {
                NoChange = false;
                if (ArticleText.IndexOf(ArticleTitle) == 0)
                    ArticleText = regexBold.Replace(ArticleText, "$1'''$2'''$3", 1);
            }
            else
                NoChange = true;

            ArticleText = ArticleText + strSecondHalf;
            ArticleText = hider.AddBackMore(ArticleText);

            return ArticleText;
        }

        /// <summary>
        /// Replaces an image in the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="OldImage">The old image to replace.</param>
        /// <param name="NewImage">The new image.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public string ReplaceImage(string OldImage, string NewImage, string ArticleText, out bool NoChange)
        {
            testText = ArticleText;
            ArticleText = ReplaceImage(OldImage, NewImage, ArticleText);

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        // NOT covered
        /// <summary>
        /// Replaces an iamge in the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="OldImage">The old image to replace.</param>
        /// <param name="NewImage">The new image.</param>
        /// <returns>The new article text.</returns>
        public static string ReplaceImage(string OldImage, string NewImage, string ArticleText)
        {
            //remove image prefix
            OldImage = Regex.Replace(OldImage, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase).Replace("_", " ");
            NewImage = Regex.Replace(NewImage, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase).Replace("_", " ");

            OldImage = Regex.Escape(OldImage).Replace("\\ ", "[ _]");

            OldImage = Variables.NamespacesCaseInsensitive[6] + Tools.CaseInsensitive(OldImage);
            NewImage = Variables.Namespaces[6] + NewImage;

            ArticleText = Regex.Replace(ArticleText, OldImage, NewImage);

            return ArticleText;
        }

        // NOT covered
        /// <summary>
        /// Removes an image from the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="Image">The image to remove.</param>
        /// <returns>The new article text.</returns>
        public static string RemoveImage(string Image, string ArticleText, bool CommentOut, string Comment)
        {
            //remove image prefix
            Image = Regex.Replace(Image, "^" + Variables.Namespaces[6], "", RegexOptions.IgnoreCase).Replace("_", " ");
            Image = Regex.Escape(Image).Replace("\\ ", "[ _]");
            Image = Tools.CaseInsensitive(Image);

            Regex r = new Regex("\\[\\[" + Variables.NamespacesCaseInsensitive[6] + Image + ".*\\]\\]");
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

                            if (CommentOut)
                                ArticleText = t.Replace(ArticleText, "<!-- " + Comment + " " + match + " -->", 1, m.Index);
                            else
                                ArticleText = t.Replace(ArticleText, "", 1);

                            break;
                        }
                    }
                }
            }
            else
            {
                r = new Regex("(" + Variables.NamespacesCaseInsensitive[6] + ")?" + Image);
                n = r.Matches(ArticleText);

                foreach (Match m in n)
                {
                    Regex t = new Regex(Regex.Escape(m.Value));

                    if (CommentOut)
                        ArticleText = t.Replace(ArticleText, "<!-- " + Comment + " $0 -->", 1, m.Index);
                    else
                        ArticleText = t.Replace(ArticleText, "", 1, m.Index);
                }
            }

            return ArticleText;
        }

        /// <summary>
        /// Removes an iamge in the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="OldImage">The image to remove.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public string RemoveImage(string Image, string ArticleText, bool CommentOut, string Comment, out bool NoChange)
        {
            testText = ArticleText;
            ArticleText = RemoveImage(Image, ArticleText, CommentOut, Comment);

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        /// <summary>
        /// Adds the category to the article.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NewCategory">The new category.</param>
        /// <returns>The article text.</returns>
        public string AddCategory(string NewCategory, string ArticleText, string ArticleTitle, out bool NoChange)
        {
            testText = ArticleText;
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
        /// <returns>The article text.</returns>
        public static string AddCategory(string NewCategory, string ArticleText, string ArticleTitle)
        {
            string oldText = ArticleText;

            ArticleText = FixCategories(ArticleText);

            if (Regex.IsMatch(ArticleText, "\\[\\[ ?[Cc]ategory ?: ?" + Regex.Escape(NewCategory) + @"\s*[\|\]]"))
                return oldText;

            string cat = "\r\n[[" + Variables.Namespaces[14] + NewCategory + "]]";
            cat = Tools.ApplyKeyWords(ArticleTitle, cat);

            if (Tools.CalculateNS(ArticleTitle) == 10 /*template*/)
                ArticleText += "<noinclude>" + cat + "\r\n</noinclude>";
            else
                ArticleText += cat;

            return ArticleText;
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
        public string ReCategoriser(string OldCategory, string NewCategory, string ArticleText, out bool NoChange)
        {
            //remove category prefix
            OldCategory = Regex.Replace(OldCategory, "^" + Variables.NamespacesCaseInsensitive[14], "", RegexOptions.IgnoreCase);
            NewCategory = Regex.Replace(NewCategory, "^" + Variables.NamespacesCaseInsensitive[14], "", RegexOptions.IgnoreCase);

            //format categories properly
            ArticleText = FixCategories(ArticleText);

            testText = ArticleText;

            if (Regex.IsMatch(ArticleText, "\\[\\[" + Variables.NamespacesCaseInsensitive[14] + Tools.CaseInsensitive(Regex.Escape(NewCategory)) + @"\s*(\||\]\])"))
            {
                bool tmp;
                ArticleText = RemoveCategory(OldCategory, ArticleText, out tmp);
            }
            else
            {
                OldCategory = Regex.Escape(OldCategory);
                OldCategory = Tools.CaseInsensitive(OldCategory);

                OldCategory = Variables.Namespaces[14] + OldCategory + @"\s*(\||\]\])";
                NewCategory = Variables.Namespaces[14] + NewCategory + "$1";

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
        public string RemoveCategory(string strOldCat, string ArticleText, out bool NoChange)
        {
            ArticleText = FixCategories(ArticleText);
            testText = ArticleText;

            strOldCat = Regex.Escape(strOldCat);
            strOldCat = Tools.CaseInsensitive(strOldCat);

            if (!ArticleText.Contains("<includeonly>"))
                ArticleText = Regex.Replace(ArticleText, "\\[\\[" + Variables.NamespacesCaseInsensitive[14] + " ?" + strOldCat + "( ?\\]\\]| ?\\|[^\\|]*?\\]\\])\r\n", "");

            ArticleText = Regex.Replace(ArticleText, "\\[\\[" + Variables.NamespacesCaseInsensitive[14] + " ?" + strOldCat + "( ?\\]\\]| ?\\|[^\\|]*?\\]\\])", "");

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        // Covered by: UtilityFunctionTests.ChangeToDefaultSort()
        /// <summary>
        /// Changes an article to use defaultsort when all categories use the same sort field.
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">Title of the article</param>
        /// <param name="NoChange">If there is no change (True if no Change)</param>
        /// <returns>The article text possibly using defaultsort.</returns>
        public string ChangeToDefaultSort(string ArticleText, string ArticleTitle, out bool NoChange)
        {
            testText = ArticleText;
            NoChange = true;

            // we don't need to process that {{lifetime}} crap
            MatchCollection ds = WikiRegexes.Defaultsort.Matches(ArticleText);
            if (ds.Count > 1 || (ds.Count == 1 && !ds[0].Value.ToUpper().Contains("DEFAULTSORT"))) return ArticleText;

            ArticleText = TalkPages.TalkPageHeaders.FormatDefaultSort(ArticleText);

            // match again, after normalisation
            ds = WikiRegexes.Defaultsort.Matches(ArticleText);
            if (ds.Count > 1)
                return testText;

            if (ds.Count == 0)
            {
                string sort = null;
                bool allsame = true;
                int matches = 0;

                //format categories properly
                ArticleText = FixCategories(ArticleText);
                testText = ArticleText;

                string s = @"\[\[\s*" + Variables.NamespacesCaseInsensitive[14] + @"\s*(.*?)\s*(?:|\|([^\|\]]*))\s*\]\]";

                MatchCollection cats = Regex.Matches(ArticleText, s);

                foreach (Match m in cats)
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
                        !sort.StartsWith(" ")) // MW bug: DEFAULTSORT doesn't treat leading spaces the same way 
                    //as categories do
                    {
                        foreach (Match m in cats)
                        {
                            ArticleText = Regex.Replace(ArticleText, s, "[[" + Variables.Namespaces[14] + "$1]]");
                        }
                        if (sort != ArticleTitle)
                            ArticleText = ArticleText + "\r\n{{DEFAULTSORT:" + Tools.RemoveDiacritics(sort) + "}}";
                    }
                }
            }
            else // already has DEFAULTSORT
            {
                string s = Tools.RemoveDiacritics(ds[0].Groups[1].Value).Trim();
                if (s != ds[0].Groups[1].Value && s.Length > 0)
                    ArticleText = ArticleText.Replace(ds[0].Value, "{{DEFAULTSORT:" + s + "}}");
            }

            ////defaultsort category cleaner
            //Regex defaultSortCategoryCleaner = new Regex(@"{{DEFAULTSORT\:([^{}]+)}}(.*?\[\[" + Variables.Namespaces[14] + @"\:[^{}]+)\|\1\]\]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            //ArticleText = defaultSortCategoryCleaner.Replace(ArticleText, "{{DEFAULTSORT:$1}}$2]]");

            NoChange = (testText == ArticleText);
            return ArticleText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange"></param>
        /// <returns></returns>
        public string LivingPeople(string ArticleText, out bool NoChange)
        {
            NoChange = true;
            testText = ArticleText;

            ArticleText = LivingPeople(ArticleText);

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        private static Regex LivingPeopleRegex1 = new Regex("\\[\\[ ?Category ?:[ _]?([0-9]{1,2}[ _]century[ _]deaths|[0-9s]{4,5}[ _]deaths|Disappeared[ _]people|Living[ _]people|Year[ _]of[ _]death[ _]missing|Possibly[ _]living[ _]people)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex LivingPeopleRegex2 = new Regex(@"\{\{(Template:)?(Recent ?death|Recentlydeceased)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static Regex LivingPeopleRegex3 = new Regex("\\[\\[ ?Category ?:[ _]?([0-9]{4})[ _]births(\\|.*?)?\\]\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
            string catKey = "";

            if (birthYear < 1910)
                return ArticleText;

            if (birthCat.Contains("|"))
                catKey = Regex.Match(birthCat, "\\|.*?\\]\\]").Value;
            else
                catKey = "]]";

            ArticleText += "[[Category:Living people" + catKey;

            return ArticleText;
        }

        /// <summary>
        /// Converts/subst'd some deprecated templates
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="NoChange">Value that indicated whether no change was made.</param>
        /// <returns>The new article text.</returns>
        public string Conversions(string ArticleText, out bool NoChange)
        {
            testText = ArticleText;
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
        public string Conversions(string ArticleText)
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

            return ArticleText;
        }

        // NOT covered
        /// <summary>
        /// Substitutes some user talk templates
        /// </summary>
        /// <param name="TalPageText">The wiki text of the talk page.</param>
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

        /// <summary>
        /// If necessary, adds/removes wikify or stub tag
        /// </summary>
        public string Tagger(string ArticleText, string ArticleTitle, out bool NoChange, ref string Summary)
        {
            testText = ArticleText;
            ArticleText = Tagger(ArticleText, ArticleTitle, ref Summary);

            NoChange = (testText == ArticleText);

            return ArticleText;
        }

        /// <summary>
        /// If necessary, adds/removes wikify or stub tag
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="ArticleTitle">The article title.</param>
        /// <returns>The tagged article.</returns>
        public string Tagger(string ArticleText, string ArticleTitle, ref string Summary)
        {
            // don't tag redirects
            if (Tools.IsRedirect(ArticleText))
                return ArticleText;

            // don't tag outside article namespace
            if (!Tools.IsMainSpace(ArticleTitle))
                return ArticleText;

            string commentsStripped = WikiRegexes.Comments.Replace(ArticleText, "");
            Sorter.interwikis(ref commentsStripped);

            // bulleted or indented text should weigh less than simple text.
            // for example, actor stubs may contain large filmographies
            string crapStripped = WikiRegexes.BulletedText.Replace(commentsStripped, "");
            int words = (Tools.WordCount(commentsStripped) + Tools.WordCount(crapStripped)) / 2;

            // update by-date tags
            foreach (KeyValuePair<Regex, string> k in RegexTagger)
            {
                ArticleText = k.Key.Replace(ArticleText, k.Value);
            }

            // remove stub tags from long articles
            if ((words > StubMaxWordCount) && WikiRegexes.Stub.IsMatch(commentsStripped))
            {
                MatchEvaluator stubEvaluator = new MatchEvaluator(stubChecker);
                ArticleText = WikiRegexes.Stub.Replace(ArticleText, stubEvaluator).Trim();
            }

            // skip article if contains any template except for stub templates
            foreach (Match m in WikiRegexes.Template.Matches(ArticleText))
            {
                if (!m.Value.Contains("stub"))
                    return ArticleText;
            }

            double length = ArticleText.Length + 1;
            double linkCount = Tools.LinkCount(commentsStripped);
            double ratio = linkCount / length;

            string catHTML = "<div id=\"catlinks\"></div>";
            if (!WikiRegexes.Category.IsMatch(commentsStripped))
            {
                catHTML = Tools.GetHTML(Variables.URLLong + "index.php?title=" + HttpUtility.UrlEncode(ArticleTitle));
            }

            if (words > 6 && (catHTML.IndexOf("<div id=\"catlinks\">") == -1) && !Regex.IsMatch(ArticleText, @"\{\{[Uu]ncategori[zs]ed"))
            {
                if (WikiRegexes.Stub.IsMatch(commentsStripped))
                { // add uncategorized stub tag
                    ArticleText += "\r\n\r\n{{Uncategorizedstub|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
                    Summary += ", added [[:Category:Uncategorized stubs|uncategorised]] tag";
                }
                else
                { // add uncategorized tag
                    ArticleText += "\r\n\r\n{{Uncategorized|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}";
                    Summary += ", added [[:Category:Category needed|uncategorised]] tag";
                }
            }
            else if (commentsStripped.Length <= 300 && !WikiRegexes.Stub.IsMatch(commentsStripped))
            { // add stub tag
                ArticleText = ArticleText + "\r\n\r\n\r\n{{stub}}";
                Summary += ", added stub tag";
            }

            if (linkCount == 0 && !WikiRegexes.DeadEnd.IsMatch(ArticleText))
            { // add dead-end tag
                ArticleText = "{{deadend|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + ArticleText;
                Summary += ", added [[:Category:Dead-end pages|deadend]] tag";
            }

            if (linkCount < 3 && (ratio < 0.0025))
            { // add wikify tag
                ArticleText = "{{Wikify|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + ArticleText;
                Summary += ", added [[:Category:Articles that need to be wikified|wikify]] tag";
            }

            // check if not orphaned
            bool orphaned = true;
            try
            {
                List<Article> links = new WikiFunctions.Lists.WhatLinksHereListProvider().MakeList(ArticleTitle);
                foreach (Article a in links)
                    if (Tools.IsMainSpace(a.Name) && !Tools.IsRedirect(a.Name))
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
            // add orphan tag if applicable
            if (orphaned)
            {
                ArticleText = "{{orphan|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}\r\n\r\n" + ArticleText;
                Summary += ", added [[:Category:Orphaned articles|orphan]] tag";
            }

            return ArticleText;
        }

        private string stubChecker(Match m)
        {// Replace each Regex cc match with the number of the occurrence.
            if (Regex.IsMatch(m.Value, Variables.SectStub))
                return m.Value;
            else
                return "";
        }

        private static Regex RemoveNowiki = new Regex("<nowiki>.*?</nowiki>", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex RemoveComments = new Regex(@"<!--.*?-->", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex Bots = new Regex(@"\{\{\s*([Bb]ots|[Nn]obots)\s*(|\|[^\}]*)\}\}", RegexOptions.Compiled);
        private static Regex Allow = new Regex(@"\|\s*allow\s*=\s*([^\|\}]*)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex Deny = new Regex(@"\|\s*deny\s*=\s*([^\|\}]*)", RegexOptions.Compiled | RegexOptions.Singleline);

        // NOT covered
        /// <summary>
        /// checks if a user is allowed to edit this article
        /// using {{bots}} and {{nobots}} tags
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <param name="Username">name of this user</param>
        /// <returns>true if you can edit, false otherwise</returns>
        public static bool CheckNoBots(string ArticleText, string Username)
        {
            bool awbAllowed = false;
            ArticleText = RemoveComments.Replace(ArticleText, "");
            ArticleText = RemoveNowiki.Replace(ArticleText, "");
            Match m = Bots.Match(ArticleText);
            if (Tools.CaseInsensitiveStringCompare(m.Groups[1].Value, "Nobots")) return false;

            string s = Allow.Match(m.Groups[2].Value).Groups[1].Value.Trim();
            if (s.Length > 0)
            {
                foreach (string u in s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (u == Username) return true;

                    //AWB bots are allowed, but this specific user may be not
                    if (u == "AWB") awbAllowed = true;
                }
            }

            s = Deny.Match(m.Groups[2].Value).Groups[1].Value.Trim();
            if (s.Length > 0)
            {
                foreach (string u in s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (u == Username) return false;
                    if ((u == "all" || u == "AWB") && !awbAllowed) return false;
                }
            }

            return true;
        }

        private static Regex dupeLinks1 = new Regex("\\[\\[([^\\]\\|]+)\\|([^\\]]*)\\]\\](.*[.\n]*)\\[\\[\\1\\|\\2\\]\\]", RegexOptions.Compiled);
        private static Regex dupeLinks2 = new Regex("\\[\\[([^\\]]+)\\]\\](.*[.\n]*)\\[\\[\\1\\]\\]", RegexOptions.Compiled);

        /// <summary>
        /// Remove some of the duplicated wikilinks from the article text
        /// </summary>
        /// <param name="ArticleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string RemoveDuplicateWikiLinks(string ArticleText)
        {
            ArticleText = dupeLinks1.Replace(ArticleText, "[[$1|$2]]$3$2");
            ArticleText = dupeLinks2.Replace(ArticleText, "[[$1]]$2$1");

            return ArticleText;
        }

        public static Regex ExtToInt1 = new Regex(@"/\w+:\/\/secure\.wikimedia\.org\/(\w+)\/(\w+)\//", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex ExtToInt2 = new Regex(@"/http:\/\/(\w+)\.(\w+)\.org\/wiki\/([^#{|}\[\]]*).*REMOVEME/i", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex ExtToInt3 = new Regex(@"/http:\/\/(\w+)\.(\w+)\.org\/.*?title=([^#&{|}\[\]]*).*REMOVEME/i", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex ExtToInt4 = new Regex(@"/[^\n]*?\[\[([^[\]{|}]+)[^\n]*REMOVEME/g", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex ExtToInt5 = new Regex(@"/^ *(w:|wikipedia:|)(en:|([a-z\-]+:)) *REMOVEME/i", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex ExtToInt6 = new Regex(@"/^ *(?:wikimedia:(m)eta|wikimedia:(commons)|(wikt)ionary|wiki(?:(n)ews|(b)ooks|(q)uote|(s)ource|(v)ersity))(:[a-z\-]+:)/i", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string ExternalURLToInternalLink(string ArticleText)
        {
            // Convert from the escaped UTF-8 byte code into Unicode
            ArticleText = System.Web.HttpUtility.UrlDecode(ArticleText);
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
            return (HasStubTemplate(ArticleText) || ArticleText.Length < Parsers.StubMaxWordCount);
        }

        public static bool HasInfobox(string ArticleText)
        {
            /* TODO: Code to check if an article has an infobox; code to get an article in the background 
             * and return an Article or simpler object so that kingbotk can tag as stub/infobox needed 
             * based on article contents (as promised by Martinp23 :P, please) */
            return false;
        }

        // NOT covered
        /// <summary>
        /// Check if article has an 'inusetag'
        /// </summary>
        public static bool IsInUse(string ArticleText)
        {
            if (Variables.LangCode != LangCodeEnum.en) return false;
            else return Variables.InUse.IsMatch(ArticleText);
        }
        #endregion
    }
}
