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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using WikiFunctions.Lists.Providers;

namespace WikiFunctions.Parse
{
    //TODO:Move Regexes to WikiRegexes as required
    //TODO:Move regexes declared in method bodies (if not dynamic based on article title, etc), into class body

    /// <summary>
    /// Provides functions for editing wiki text, such as formatting and re-categorisation.
    /// </summary>
    public partial class Parsers
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
            RegexUnicode.Add(new Regex("&(ndash|mdash|minus|times|lt|gt|nbsp|thinsp|zwnj|shy|lrm|rlm|[Pp]rime|ensp|emsp|#x2011|#820[13]|#8239);"), "&amp;$1;");
            //IE6 does like these
            RegexUnicode.Add(new Regex("&#(705|803|596|620|699|700|8652|9408|9848|12288|160|61|x27|39);"), "&amp;#$1;");

            //Decoder doesn't like these
            RegexUnicode.Add(new Regex("&#(x109[0-9A-Z]{2});"), "&amp;#$1;");
            RegexUnicode.Add(new Regex("&#((?:277|119|84|x1D|x100)[A-Z0-9a-z]{2,3});"), "&amp;#$1;");
            RegexUnicode.Add(new Regex("&#(x12[A-Za-z0-9]{3}|65536|769);"), "&amp;#$1;");

            // Can't change 5-digit Hex strings, get broken by HttpUtility.HtmlDecode
            RegexUnicode.Add(new Regex("&#(x[A-Fa-f0-9]{5});"), "&amp;#$1;");

            //interfere with wiki syntax
            RegexUnicode.Add(new Regex("&#(0?13|126|x5[BD]|x7[bcd]|0?9[13]|0?12[345]|0?0?3[92]);", RegexOptions.IgnoreCase), "&amp;#$1;");

            // get badmd5 error from API on save due to WebRequest::normalizeUnicode (https://svn.wikimedia.org/doc/classWebRequest.html#ac1b762873fc2f0fe661499cfc116a9da) in API code
            RegexUnicode.Add(new Regex("&#(x232[A9]);"), "&amp;#$1;");
            
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Greedy_regex_for_unicode_characters
            // .NET doesn't seem to like the Unicode versions of these – deleted from edit box
            RegexUnicode.Add(new Regex("&#(x2[0-9AB][0-9A-Fa-f]{3});"), "&amp;#$1;");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.7B.7Bcommons.7CCategory:XXX.7D.7D_.3E_.7B.7Bcommonscat.7CXXX.7D.7D
            RegexConversion.Add(new Regex(@"\{\{\s*[Cc]ommons\s?\|\s*[Cc]ategory:\s*([^{}]+?)\s*\}\}"), @"{{Commons category|$1}}");

            //https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Commons_category
            RegexConversion.Add(new Regex(@"({{[Cc]ommons cat(?:egory)?\|\s*)([^{}\|]+?)\s*\|\s*\2\s*}}"), @"$1$2}}");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_empty_.7B.7BArticle_issues.7D.7D
            // article issues with no issues -> remove tag
            RegexConversion.Add(new Regex(@"\{\{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues(?:\s*\|\s*(?:section|article)\s*=\s*[Yy])?\s*\}\}"), "");

            // remove duplicate / populated and null fields in multiple issues templates
            RegexConversion.Add(new Regex(@"({{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues[^{}]*\|\s*)(\w+)\s*=\s*([^\|}{]+?)\s*\|((?:[^{}]*?\|)?\s*)\2(\s*=\s*)\3(\s*(\||\}\}))"), "$1$4$2$5$3$6"); // duplicate field remover
            RegexConversion.Add(new Regex(@"(\{\{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues[^{}]*\|\s*)(\w+)(\s*=\s*[^\|}{]+(?:\|[^{}]+?)?)\|\s*\2\s*=\s*(\||\}\})"), "$1$2$3$4"); // 'field=populated | field=null' drop field=null
            RegexConversion.Add(new Regex(@"(\{\{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues[^{}]*\|\s*)(\w+)\s*=\s*\|\s*((?:[^{}]+?\|)?\s*\2\s*=\s*[^\|}{\s])"), "$1$3"); // 'field=null | field=populated' drop field=null

            RegexConversion.Add(new Regex(@"({{\s*[Cc]itation needed\s*\|)\s*(?:[Dd]ate:)?([A-Z][a-z]+ 20\d\d)\s*\|\s*(date\s*=\s*\2\s*}})", RegexOptions.IgnoreCase), @"$1$3");

            SmallTagRegexes.Add(WikiRegexes.SupSub);
            SmallTagRegexes.Add(WikiRegexes.FileNamespaceLink);
            SmallTagRegexes.Add(WikiRegexes.Refs);
            SmallTagRegexes.Add(WikiRegexes.Small);
        }

        /// <summary>
        /// Dictionary of HTML-encoded characters that mustn't be converted by Unicodify function
        /// </summary>
        private static readonly Dictionary<Regex, string> RegexUnicode = new Dictionary<Regex, string>();
        private static readonly Dictionary<Regex, string> RegexConversion = new Dictionary<Regex, string>();

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
        /// Remove 2 or more &lt;br /&gt;'s
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public string FixBrParagraphs(string articleText)
        {
            // check for performance
            if(SyntaxRemoveBrQuick.IsMatch(articleText))
                articleText = SyntaxRemoveBr.Replace(articleText, "\r\n\r\n");

            articleText = SyntaxRemoveParagraphs.Replace(articleText, "\r\n\r\n");
            return SyntaxRegexListRowBrTagStart.Replace(articleText, "$1");
        }

        private static readonly Regex BoldToBracket = new Regex(@"('''(?:[^']+|.*?[^'])'''\s*\()(.*)");

        private static readonly Regex DiedDateRegex =
            new Regex(
                @"^d\.\s*(\[*(?:" + WikiRegexes.MonthsNoGroup + @"\s+0?([1-3]?\d)|0?([1-3]?\d)\s*" +
                WikiRegexes.MonthsNoGroup + @")?\]*\s*\[*[12]?\d{3}\]*[\),])", RegexOptions.IgnoreCase);

        private static readonly Regex DOBRegex =
            new Regex(
                @"^(?:b\.|[Bb]orn(?::+| +on))\s*(\[*(?:" + WikiRegexes.MonthsNoGroup + @"\s+0?([1-3]?\d)|0?([1-3]?\d)\s*" +
                WikiRegexes.MonthsNoGroup + @")?[\]\s,]*\[*[12]?\d{3}\]*(?:[\),]| +in +))", RegexOptions.IgnoreCase);

        private static readonly Regex DOBRegexDash =
            new Regex(
                @"(?<![\*#].*)('''(?:[^']+|.*?[^'])'''\s*\()(\[*(?:" + WikiRegexes.MonthsNoGroup + @"\s+0?([1-3]?\d)|0?([1-3]?\d)\s*" +
                WikiRegexes.MonthsNoGroup + @")?\]*\s*\[*[12]?\d{3}\]*)\s*(?:\-|–|&ndash;)\s*\)", RegexOptions.IgnoreCase);
        
        private static readonly Regex DOBRegexDashQuick = new Regex(@"(?<=(?:\-|–|&ndash;)\s*)\)");

        private static readonly Regex BornDeathRegex =
            new Regex(
                @"^(?:[Bb]orn|b\.)\s*(\[*(?:" + WikiRegexes.MonthsNoGroup +
                @"\s+0?(?:[1-3]?\d)|0?(?:[1-3]?\d)\s*" + WikiRegexes.MonthsNoGroup +
                @")?\]*,?\s*\[*[12]?\d{3}\]*)\s*(.|&.dash;)\s*(?:[Dd]ied|d\.)\s+(\[*(?:" + WikiRegexes.MonthsNoGroup +
                @"\s+0?(?:[1-3]?\d)|0?(?:[1-3]?\d)\s*" + WikiRegexes.MonthsNoGroup + @")\]*,?\s*\[*[12]?\d{3}\]*\)\s*)",
                RegexOptions.IgnoreCase);

        //Covered by: LinkTests.FixLivingThingsRelatedDates()
        /// <summary>
        /// Replace b. and d. for born/died, or date– for born per [[WP:BORN]]
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixLivingThingsRelatedDates(string articleText)
        {
            // Three born/died regexes wrapped like this for performance
            if(Regex.IsMatch(articleText, @"'''\s*\([BbDd]"))
                articleText = BoldToBracket.Replace(articleText, m=>
                                                    {
                                                        string newvalue = m.Groups[2].Value;

                                                        newvalue = DiedDateRegex.Replace(newvalue, "died $1"); // date of death
                                                        newvalue = DOBRegex.Replace(newvalue, "born $1"); // date of birth
                                                        newvalue = BornDeathRegex.Replace(newvalue, "$1 – $3"); // birth and death
                                                        return  m.Groups[1].Value + newvalue;
                                                    });

            if (DOBRegexDashQuick.IsMatch(articleText) && !DOBRegexDash.IsMatch(WikiRegexes.InfoBox.Match(articleText).Value))
                articleText = DOBRegexDash.Replace(articleText, "$1born $2)"); // date of birth – dash

            return articleText;
        }

        /// <summary>
        /// Searches for {{dead link}}s
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>Dictionary of dead links found</returns>
        public static Dictionary<int, int> DeadLinks(string articleText)
        {
            if(!TemplateExists(GetAllTemplates(articleText), WikiRegexes.DeadLink))
                return new Dictionary<int, int>();

            return DictionaryOfMatches(articleText, WikiRegexes.DeadLink);
        }

        /// <summary>
        /// Returns a dictionary of the match index and lenght of all matches of the input regex in the input text
        /// </summary>
        /// <returns>The of matches.</returns>
        /// <param name="articleText">Article text.</param>
        /// <param name="r">The red component.</param>
        private static Dictionary<int, int> DictionaryOfMatches(string articleText, Regex r)
        {
            articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.Comments);
            Dictionary<int, int> found = new Dictionary<int, int>();

            foreach (Match m in r.Matches(articleText))
            {
                found.Add(m.Index, m.Length);
            }

            return found;
        }

        /// <summary>
        /// Searches for wikilinks with no target e.g. [[|foo]]
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>Dictionary of links with no target found</returns>
        public static Dictionary<int, int> TargetLessLinks(string articleText)
        {
            return DictionaryOfMatches(articleText, WikiRegexes.TargetLessLink);
        }

        /// <summary>
        /// Searches for wikilinks with double pipes
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>Dictionary of links with double pipes found</returns>
        public static Dictionary<int, int> DoublePipeLinks(string articleText)
        {
            // Performance strategy: get list of all internal wikilinks, filter to those with two | in
            if((from Match m in WikiRegexes.SimpleWikiLink.Matches(articleText) where m.Value.Count(s => s == '|') > 1 select m.Value).ToList().Any())
            return DictionaryOfMatches(articleText, WikiRegexes.DoublePipeLink);

            return (new Dictionary<int, int>());
        }

        /// <summary>
        /// Checks position of See also section relative to Notes, references, external links sections
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>Whether 'see also' is after any of the sections</returns>
        public static bool HasSeeAlsoAfterNotesReferencesOrExternalLinks(string articleText)
        {
            int seeAlso = WikiRegexes.SeeAlso.Match(articleText).Index;
            if (seeAlso <= 0)
                return false;

            int externalLinks = WikiRegexes.ExternalLinksHeader.Match(articleText).Index;
            if (externalLinks > 0 && seeAlso > externalLinks)
                return true;

            int references = WikiRegexes.ReferencesHeader.Match(articleText).Index;
            if (references > 0 && seeAlso > references)
                return true;

            int notes = WikiRegexes.NotesHeader.Match(articleText).Index;
            if (notes > 0 && seeAlso > notes)
                return true;

            return false;
        }

        private static readonly Regex PageRangeIncorrectMdash = new Regex(@"(pages\s*=\s*|pp\.?\s*)((?:&nbsp;)?\d+\s*)(?:\-\-?|—|&mdash;|&#8212;)(\s*\d+)", RegexOptions.IgnoreCase);

        // avoid dimensions in format 55-66-77
        private static readonly Regex UnitTimeRangeIncorrectMdash = new Regex(@"(?<!-)(\b[1-9]?\d+\s*)(?:-|—|&mdash;|&#8212;)(\s*[1-9]?\d+)(\s+|&nbsp;)((?:years|months|weeks|days|hours|minutes|seconds|[km]g|kb|[ckm]?m|[Gk]?Hz|miles|mi\.|%|feet|foot|ft|met(?:er|re)s)\b|in\))");
        private static readonly Regex DollarAmountIncorrectMdash = new Regex(@"(\$[1-9]?\d{1,3}\s*)(?:-|—|&mdash;|&#8212;)(\s*\$?[1-9]?\d{1,3})");
        private static readonly Regex AMPMIncorrectMdash = new Regex(@"([01]?\d:[0-5]\d\s*([AP]M)\s*)(?:-|—|&mdash;|&#8212;)(\s*[01]?\d:[0-5]\d\s*([AP]M))", RegexOptions.IgnoreCase);
        private static readonly Regex AgeIncorrectMdash = new Regex(@"([Aa]ge[sd])\s([1-9]?\d\s*)(?:-|—|&mdash;|&#8212;)(\s*[1-9]?\d)");
        private static readonly Regex SentenceClauseIncorrectMdash = new Regex(@"(?!xn--)(\w{2}|⌊⌊⌊⌊M\d+⌋⌋⌋⌋)\s*--\s*(\w)");
        private static readonly Regex SuperscriptMinus = new Regex(@"(<sup>)(?:-|–|—)(?=\d+</sup>)");

        // Covered by: FormattingTests.TestMdashes()
        /// <summary>
        /// Replaces hyphens and em-dashes with en-dashes, per [[WP:DASH]]
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article's title</param>
        /// <returns>The modified article text.</returns>
        public string Mdashes(string articleText, string articleTitle)
        {
            const int backlength = 12;
            // performance: faster to pick out end of ranges and substring than run each regex in turn
            List<string> dashed = (from Match m in Regex.Matches(articleText, @"(?:—|-|&#8212;|&mdash;)+\s*[0-9].{0,12}")
                select (m.Index > backlength ? articleText.Substring(m.Index-backlength, m.Length+backlength) : articleText.Substring(0, m.Length+m.Index))).ToList();

            dashed = Tools.DeduplicateList(dashed);

            // replace hyphen with dash and convert Pp. to pp.
            if(dashed.Any(s => PageRangeIncorrectMdash.IsMatch(s)))
                articleText = PageRangeIncorrectMdash.Replace(articleText, m=>
                                                              {
                                                                  string pagespart = m.Groups[1].Value;
                                                                  if (pagespart.Contains(@"Pp"))
                                                                      pagespart = pagespart.ToLower();

                                                                  return pagespart + m.Groups[2].Value + @"–" + m.Groups[3].Value;
                                                              });

            if(dashed.Any(s => UnitTimeRangeIncorrectMdash.IsMatch(s)))
                articleText = UnitTimeRangeIncorrectMdash.Replace(articleText, @"$1–$2$3$4");

            if(dashed.Any(s => DollarAmountIncorrectMdash.IsMatch(s)))
                articleText = DollarAmountIncorrectMdash.Replace(articleText, @"$1–$2");

            if(dashed.Any(s => AMPMIncorrectMdash.IsMatch(s)))
                articleText = AMPMIncorrectMdash.Replace(articleText, @"$1–$3");

            if(dashed.Any(s => AgeIncorrectMdash.IsMatch(s)))
                articleText = AgeIncorrectMdash.Replace(articleText, @"$1 $2–$3");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Match_en_dashes.2Femdashs_from_titles_with_those_in_the_text
            // if title has en or em dashes, apply them to strings matching article title but with hyphens
            if (articleTitle.Contains(@"–") || articleTitle.Contains(@"—"))
                articleText = Regex.Replace(articleText, Regex.Escape(articleTitle.Replace(@"–", @"-").Replace(@"—", @"-")), articleTitle);

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Change_--_.28two_dashes.29_to_.E2.80.94_.28em_dash.29
            // convert two dashes to emdash if surrouned by alphanumeric characters, except convert to endash if surrounded by numbers. -- checked for performance
            if (Namespace.Determine(articleTitle) == Namespace.Mainspace && articleText.Contains("--"))
                articleText = SentenceClauseIncorrectMdash.Replace(articleText, m => m.Groups[1].Value + ((Regex.IsMatch(m.Groups[1].Value, @"^\d+$") && Regex.IsMatch(m.Groups[2].Value, @"^\d+$")) ? @"–" : @"—") + m.Groups[2].Value);

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#minuses
            // replace hyphen or en-dash or emdash with Unicode minus (&minus;)
            // [[Wikipedia:MOSNUM#Common_mathematical_symbols]]
            return SuperscriptMinus.Replace(articleText, "$1−");
        }

        private static readonly Regex BrTwoNewlines = new Regex("(?:<br */?> *)\r\n\r\n", RegexOptions.IgnoreCase);
        private static readonly Regex FourOrMoreNewlines = new Regex("(\r\n){4,}");
        private static readonly Regex NewlinesBelowExternalLinks = new Regex(@"==External links==[\r\n\s]*\*");
        private static readonly Regex NewlinesBeforeUrl = new Regex(@"\r\n\r\n(\* ?\[?http)");
        private static readonly Regex HorizontalRule = new Regex("----+$");
        private static readonly Regex MultipleTabs = new Regex("  +", RegexOptions.Compiled);
        private static readonly Regex SpacesThenTwoNewline = new Regex(" +\r\n\r\n", RegexOptions.Compiled);
        private static readonly Regex WikiListWithMultipleSpaces = new Regex(@"^([\*#]+) +", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SpacedDashes = new Regex(" (—|&#15[01];|&mdash;|&#821[12];|&#x201[34];) ", RegexOptions.Compiled);

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
            //Remove <br /> if followed by double newline, NOT in blockquotes
            if(BrTwoNewlines.IsMatch(articleText) && !WikiRegexes.Blockquote.IsMatch(articleText))
            {
                while(BrTwoNewlines.IsMatch(articleText))
                    articleText = BrTwoNewlines.Replace(articleText.Trim(), "\r\n\r\n");
            }

            while(SpacesThenTwoNewline.IsMatch(articleText))
                articleText = SpacesThenTwoNewline.Replace(articleText, "\r\n\r\n");

            // remove excessive newlines
            // Don't apply within <poem> tags
            // retain one or two newlines before stub
            if(articleText.Contains("\r\n\r\n\r\n"))
            {
                bool OK = true;
                int p = articleText.IndexOf("poem", StringComparison.OrdinalIgnoreCase);

                if(p > -1)
                {
                    foreach(Match m in WikiRegexes.Poem.Matches(articleText.Substring(Math.Max(p-50,0))))
                    {
                        if(m.Value.Contains("\r\n\r\n"))
                        {
                            OK = false;
                            break;
                        }
                    }
                }

                if(OK)
                {
                    if (WikiRegexes.Stub.IsMatch(articleText))
                        articleText = FourOrMoreNewlines.Replace(articleText, "\r\n\r\n");
                    else
                        articleText = WikiRegexes.ThreeOrMoreNewlines.Replace(articleText, "\r\n\r\n");
                }
            }

            articleText = NewlinesBelowExternalLinks.Replace(articleText, "==External links==\r\n*");
            articleText = NewlinesBeforeUrl.Replace(articleText, "\r\n$1");

            articleText = HorizontalRule.Replace(articleText.Trim(), "");

            return articleText.Replace("\r\n|\r\n\r\n", "\r\n|\r\n").Trim();
        }

        // covered by RemoveAllWhiteSpaceTests
        /// <summary>
        /// Applies removes all excess whitespace from the article
        /// Not called by general fixes
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string RemoveAllWhiteSpace(string articleText)
        {
            articleText = articleText.Replace("\t", " ");
            articleText = RemoveWhiteSpace(articleText);

            articleText = articleText.Replace("\r\n\r\n*", "\r\n*");

            articleText = MultipleTabs.Replace(articleText, " ");

            articleText = articleText.Replace("==\r\n\r\n", "==\r\n");
            articleText = NewlinesBelowExternalLinks.Replace(articleText, "==External links==\r\n*");

            // fix bullet points – one space after them not multiple
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_arbitrary_spaces_after_bullet
            articleText = WikiListWithMultipleSpaces.Replace(articleText, "$1 ");

            //fix dash spacing
            articleText = SpacedDashes.Replace(articleText, "$1");

            return articleText.Trim();
        }

        private static readonly List<string> DateFields = new List<string>(new[] { "date", "accessdate", "access-date", "archivedate", "airdate" });

        /// <summary>
        /// Updates dates in citation templates to use the strict predominant date format in the article (en wiki only)
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>The updated article text</returns>
        public static string PredominantDates(string articleText)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            DateLocale predominantLocale = DeterminePredominantDateLocale(articleText, true, true);

            if (predominantLocale.Equals(DateLocale.Undetermined))
                return articleText;

            foreach (Match m in WikiRegexes.CiteTemplate.Matches(articleText))
            {
                string newValue = m.Value;

                foreach (string field in DateFields)
                {
                    string fvalue = Tools.GetTemplateParameterValue(newValue, field);

                    if (WikiRegexes.ISODates.IsMatch(fvalue) || WikiRegexes.AmericanDates.IsMatch(fvalue) || WikiRegexes.InternationalDates.IsMatch(fvalue))
                        newValue = Tools.UpdateTemplateParameterValue(newValue, field, Tools.ConvertDate(fvalue, predominantLocale));
                }

                // merge changes to article text
                if (!m.Value.Equals(newValue))
                    articleText = articleText.Replace(m.Value, newValue);
            }

            return articleText;
        }

        /// <summary>
        /// The in-use date formats on the English Wikipedia
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

            if (explicitonly)
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

            if (Americans != Internationals)
            {
                if (Americans == 0 && Internationals > 0 || (Internationals / Americans >= 2 && Internationals > 4))
                    return DateLocale.International;
                if (Internationals == 0 && Americans > 0 || (Americans / Internationals >= 2 && Americans > 4))
                    return DateLocale.American;
            }

            // check for explicit df or mf in brith/death templates
            if (Tools.GetTemplateParameterValue(BirthDate.Match(articleText).Value, "df").StartsWith("y")
                || Tools.GetTemplateParameterValue(DeathDate.Match(articleText).Value, "df").StartsWith("y"))
                return DateLocale.International;
            
            if (Tools.GetTemplateParameterValue(BirthDate.Match(articleText).Value, "mf").StartsWith("y")
                || Tools.GetTemplateParameterValue(DeathDate.Match(articleText).Value, "mf").StartsWith("y"))
                return DateLocale.American;

            return DateLocale.Undetermined;
        }

        // Covered by: LinkTests.TestCanonicalizeTitle(), incomplete
        /// <summary>
        /// returns URL-decoded link target
        /// Handles titles from [[Category:Articles with underscores in the title]]
        /// </summary>
        public static string CanonicalizeTitle(string title)
        {
            // visible parts of links may contain text we shouldn't modify, such as refs and external links
            if (!Tools.IsValidTitle(title) || title.Contains(":/"))
                return title;

            Variables.WaitForDelayedRequests();
            string s = CanonicalizeTitleRaw(title);
            if (Variables.UnderscoredTitles.Contains(Tools.TurnFirstToUpper(s)))
            {
                return HttpUtility.UrlDecode(title.Replace("+", "%2B")).Trim(new[] { '_' });
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

        private static readonly Regex RegexMainArticle = new Regex(@"^:?'{0,5}Main article:\s?'{0,5}\[\[([^\|\[\]]*?)(\|([^\[\]]*?))?\]\]\.?'{0,5}\.?\s*?(?=($|[\r\n]))", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex SeeAlsoLink = new Regex(@"^:?'{0,5}See also:\s?'{0,5}\[\[([^\|\[\]]*?)(\|([^\[\]]*?))?\]\]\.?'{0,5}\.?\s*?(?=($|[\r\n]))", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex SeeAlsoMainArticleQuick = new Regex(@"\r\n[:']*(See also|Main article):", RegexOptions.IgnoreCase);

        // Covered by: FixMainArticleTests
        /// <summary>
        /// Fixes instances of ''Main Article: xxx'' to use {{main|xxx}}, ''see also:'' to use {{see also|xxx}}
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string FixMainArticle(string articleText)
        {
            if(SeeAlsoMainArticleQuick.IsMatch(articleText))
            {
                articleText = SeeAlsoLink.Replace(articleText,
                                                  m => m.Groups[2].Value.Length == 0
                                                  ? "{{See also|" + m.Groups[1].Value + "}}"
                                                  : "{{See also|" + m.Groups[1].Value + "|l1=" + m.Groups[3].Value + "}}");

                articleText = RegexMainArticle.Replace(articleText,
                                                       m => m.Groups[2].Value.Length == 0
                                                       ? "{{Main|" + m.Groups[1].Value + "}}"
                                                       : "{{Main|" + m.Groups[1].Value + "|l1=" + m.Groups[3].Value + "}}");
            }
            return articleText;
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
            if(GetAllWikiLinks(articleText).Any(link => WikiRegexes.EmptyLink.IsMatch(link)))
            {
                while(WikiRegexes.EmptyLink.IsMatch(articleText))
                    articleText = WikiRegexes.EmptyLink.Replace(articleText, "");
            }

            if(TemplateExists(GetAllTemplates(articleText), WikiRegexes.EmptyTemplate))
                articleText = WikiRegexes.EmptyTemplate.Replace(articleText, "");

            return articleText;
        }

        /// <summary>
        /// Deduplicates multiple maintenance tags. Uses earliest of date parameters, merges all other parameters.
        /// Does nothing if parameter values other than date parameter are conflicting
        /// </summary>
        /// <returns>The revised maintenance tags.</returns>
        /// <param name="tags">Maintenance tags list</param>
        public static List<string> DeduplicateMaintenanceTags(List<string> tags)
        {
            List<string> newtags = new List<string>();
            List<string> originalTags = tags;

            // if any tag has unnamed param as firt argument, sort tags with longest part before first = to be first, so we retain the unnamed param
            if(tags.Any(t => Regex.IsMatch(t, @"\|[^=\|}}]+\|")))
                tags = tags.OrderByDescending(s => (s.Contains("=") ? s.Substring(0, s.IndexOf("=")).Length : s.Length)).ToList();

            foreach(string t in tags)
            {
                string existingTag = newtags.Where(nt => Tools.TurnFirstToLower(Tools.GetTemplateName(nt)) == Tools.TurnFirstToLower(Tools.GetTemplateName(t))).FirstOrDefault();

                if(existingTag != null)
                {
                    string existingTagOriginal = existingTag;
                    Dictionary<string, string> tparams = Tools.GetTemplateParameterValues(t);

                    foreach(KeyValuePair<string, string> kvp in tparams)
                    {
                        if(kvp.Value.Length == 0)
                            continue;

                        string existingParamValue = Tools.GetTemplateParameterValue(existingTag, kvp.Key);

                        if(existingParamValue.Length == 0)
                            existingTag = Tools.SetTemplateParameterValue(existingTag, kvp.Key, kvp.Value);
                        else if(existingParamValue.Equals(kvp.Value))
                                continue;
                        else
                        {
                            // conflicting parameter values

                            // if param not date cannot handle, return
                            if(kvp.Key != "date")
                                return originalTags;

                            // if param is date, take earlier date
                            try
                            {
                            DateTime existingDate = Convert.ToDateTime("1 " + existingParamValue);
                            DateTime tagDate = Convert.ToDateTime("1 " + kvp.Value);

                            if(tagDate < existingDate)
                                existingTag = Tools.SetTemplateParameterValue(existingTag, kvp.Key, kvp.Value);
                            }
                            catch
                            {
                                return originalTags;
                            }
                        }
                    }

                    newtags.Remove(existingTagOriginal);
                    newtags.Add(existingTag);

                }
                else
                    newtags.Add(t);
            }

            return newtags;
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

            noChange = newText.Equals(articleText);

            return newText;
        }

        private static readonly HideText BulletExternalHider = new HideText(false, true, false);

        private static readonly Regex ExternalLinksSection = new Regex(@"=\s*(?:external)?\s*links\s*=", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
        private static readonly Regex NewlinesBeforeHTTP = new Regex("(\r\n|\n)?(\r\n|\n)(\\[?http)", RegexOptions.Compiled);
        private static readonly Regex HeadingQuick = new Regex(@"^=+[^=\r\n]+=", RegexOptions.Multiline);

        // Covered by: LinkTests.TestBulletExternalLinks()
        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string BulletExternalLinks(string articleText)
        {
            // Performance: get all headings, filter to any external link ones
            List<Match> h = (from Match m in HeadingQuick.Matches(articleText)
                where m.Value.ToLower().Contains("external") && ExternalLinksSection.IsMatch(m.Value)
                                      select m).ToList();

            if(h.Any())
            {            
                int intStart = h.FirstOrDefault().Index;

                string articleTextSubstring = articleText.Substring(intStart);

                if(NewlinesBeforeHTTP.IsMatch(articleTextSubstring))
                {
                    articleText = articleText.Substring(0, intStart);
                    articleTextSubstring = BulletExternalHider.HideMore(articleTextSubstring);
                    articleTextSubstring = NewlinesBeforeHTTP.Replace(articleTextSubstring, "$2* $3");

                    return articleText + BulletExternalHider.AddBackMore(articleTextSubstring);
                }
            }
            
            return articleText;
        }

        private static readonly Regex WordWhitespaceEndofline = new Regex(@"(\w+)\s+$", RegexOptions.Compiled);
        private static string CategoryStart;


        private static readonly Regex TripleBraceNum = new Regex(@"{{{\d}}}", RegexOptions.Compiled);

        /// <summary>
        /// Returns whether the article text has a &lt;noinclude&gt; or &lt;includeonly&gt; or '{{{1}}}' etc. which should not appear on the mainspace
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static bool NoIncludeIncludeOnlyProgrammingElement(string articleText)
        {
            return WikiRegexes.IncludeonlyNoinclude.IsMatch(articleText) || TripleBraceNum.IsMatch(articleText);
        }

        // Covered by: ImageTests.BasicImprovements(), incomplete
        /// <summary>
        /// Fix common spacing/capitalisation errors in images
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixImages(string articleText)
        {
            return WikiRegexes.LooseImage.Replace(articleText, FixImagesME);
        }

        private static string FixImagesME(Match m)
        {
            string imageName = m.Groups[2].Value;
            // only apply underscore/URL encoding fixes to image name (group 2)
            // don't convert %27%27 -- https://phabricator.wikimedia.org/T10932
            string x = "[[" + Namespace.Normalize(m.Groups[1].Value, 6) + (imageName.Contains("%27%27") ? imageName : CanonicalizeTitle(imageName).Trim()) + m.Groups[3].Value.Trim() + "]]";

            return x;
        }

        private static readonly Regex Temperature = new Regex(@"(?:&deg;|&ordm;|º|°)(?:&nbsp;)?\s*([CcFf])(?![A-Za-z])", RegexOptions.Compiled);

        /// <summary>
        /// Fix bad Temperatures
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixTemperatures(string articleText)
        {
            return Temperature.Replace(articleText, m => "°" + m.Groups[1].Value.ToUpper());
        }

        /// <summary>
        /// Apply non-breaking spaces for abbreviated SI units, imperial units, pp for pages. Remove incorrect space before % symbol
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string FixNonBreakingSpaces(string articleText)
        {
            // Chinese do not use spaces to separate sentences/words
            if (!Variables.LangCode.Equals("zh"))
            {
                // only apply um (micrometre) fix on English wiki to avoid German word "um"
                // Check on number plus word for performance
                List<string> NumText = Tools.DeduplicateList((from Match m in Regex.Matches(articleText, @"\b[0-9]+[ \u00a0]*[B-Wc-zµ/°]{1,4}\b") select m.Value).ToList());

                if(NumText.Any(s => WikiRegexes.UnitsWithoutNonBreakingSpaces.IsMatch(s)))
                    articleText = WikiRegexes.UnitsWithoutNonBreakingSpaces.Replace(articleText, m => (m.Groups[2].Value.StartsWith("um") && !Variables.LangCode.Equals("en")) ? m.Value : m.Groups[1].Value + "&nbsp;" + m.Groups[2].Value);

                if(NumText.Any(s => (s.EndsWith("in") || s.EndsWith("ft") || s.EndsWith("oz"))))
                    articleText = WikiRegexes.ImperialUnitsInBracketsWithoutNonBreakingSpaces.Replace(articleText, "$1&nbsp;$2");

                if(articleText.Contains(@"&nbsp;ft")) // check for performance
                    articleText = WikiRegexes.MetresFeetConversionNonBreakingSpaces.Replace(articleText, @"$1&nbsp;m");

                // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Pagination
                // add non-breaking space after pp. abbreviation for pages.
                if(Regex.IsMatch(articleText, @"(p\.) *[0-9IVXCL][^S]")) // check for performance
                    articleText = Regex.Replace(articleText, @"(\b[Pp]?p\.) *(?=[\dIVXCL][^S])", @"$1&nbsp;");
            }

            // Add non-breaking space to 12-hour clock times [[MOS:TIME]].
            // It works only for dotted lower-case a.m. or p.m.
            if (Variables.LangCode.Equals("en") || Variables.LangCode.Equals("simple"))
            {
                if(articleText.Contains(".m.")) // check for performance. Set &nbsp; and trim excess leading zero
                    articleText = WikiRegexes.ClockTime.Replace(articleText, m => Regex.Replace(m.Groups[1].Value, @"0(\d:)", "$1") + "&nbsp;" + m.Groups[2].Value);

                // Removes space or non-breaking space from percent per [[WP:PERCENT]].
                // Avoid doing this for more spaces to prevent false positives.
                // Don't fix space in all wikis. For instance sv:Procent requires a space inbetween
                if(Regex.IsMatch(articleText, @"%(\p{P}|\)?\s)")) // check for performance
                    articleText = WikiRegexes.Percent.Replace(articleText, " $1%$3");

                // Removes space or non-breaking space from percent per [[WP:CURRENCY]]  if they consist of a nonalphabetic symbol only.
                // Avoid doing this for more spaces to prevent false positives.
                // Don't fix space in all wikis.
                articleText = WikiRegexes.Currency.Replace(articleText, "$1$3");
            }

            return articleText;
        }

        // Covered by: UtilityFunctionTests.RemoveEmptyComments()
        /// <summary>
        /// Removes HTML comments with nothing/only whitespace between tags
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text (removed empty comments).</returns>
        public static string RemoveEmptyComments(string articleText)
        {
            return WikiRegexes.EmptyComments.Replace(articleText, "");
        }
        #endregion

        #region other functions
        private static readonly Regex LineSeparatorZeroWidthSpaceStartOfLine = new Regex(@"^(\u2028|\u200B)+", RegexOptions.Multiline);
        //   private static readonly Regex ZeroWidthSpace = new Regex(@"^(\u200B)+", RegexOptions.Multiline);

        /// <summary>
        /// Performs transformations related to Unicode characters that may cause problems for different clients
        /// Removes line separator Unicode characters at start of paragraph, otherwise converts line separator, paragraph separator Unicode characters to spaces.
        /// </summary>
        public string FixUnicode(string articleText)
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_15#Probably_odd_characters_being_treated_even_more_oddly
            articleText = articleText.Replace('\x2029', ' ');

            // https://en.wikipedia.org/wiki/Wikipedia:AWB/B#Line_break_insertion
            // most browsers handle Unicode line separators as whitespace, so should we
            // check if paragraph separators are properly converted by RichEdit itself
            // if line separator is at start of line, remove it, otherwise (within paragraph), convert it to space
            // remove zero width space - CHECKWIKI error 16
            // this character should not be used in domain names.
            // Browsers are blacklisting it because of the potential for phishing.
            articleText = LineSeparatorZeroWidthSpaceStartOfLine.Replace(articleText, "");
            return articleText.Replace('\x2028', ' ');
            
            //MOS:NBSP states that "A literal hard space, such as one of the Unicode non-breaking space characters, should not be used"
            //In an ideal situation we should remove unicode nbsp with space and then readd html nbsp using FixSyntax where really needed
            //return articleText.Replace('\x00a0', ' ');
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

            noChange = newText.Equals(articleText);

            return newText;
        }

        private static readonly Regex NDash = new Regex("&#150;|&#8211;|&#x2013;", RegexOptions.Compiled);
        private static readonly Regex MDash = new Regex("&#151;|&#8212;|&#x2014;", RegexOptions.Compiled);
        private static readonly Regex MathTagStart = new Regex("<[Mm]ath>", RegexOptions.Compiled);

        // Covered by: UnicodifyTests
        /// <summary>
        /// Converts HTML entities to unicode, with some deliberate exceptions
        /// Does not change 5-character HTML hex entities
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string Unicodify(string articleText)
        {
            if (MathTagStart.IsMatch(articleText))
                return articleText;

            articleText = NDash.Replace(articleText, "&ndash;");
            articleText = MDash.Replace(articleText, "&mdash;");
            articleText = articleText.Replace(" &amp; ", " & ");
            articleText = articleText.Replace("&amp;", "&amp;amp;");
            articleText = articleText.Replace("&#153;", "™");
            articleText = articleText.Replace("&#149;", "•");

            articleText = RegexUnicode.Aggregate(articleText, (current, k) => k.Key.Replace(current, k.Value));

            // this seems to support 5-character HTML hex entities e.g. U+FB17: ARMENIAN SMALL LIGATURE MEN XEH / &#xFB17; supported OK
            // But 5-character e.g. &#x10A80; is not supported, gets broken as leading 1 removed
            articleText = HttpUtility.HtmlDecode(articleText);

            return articleText;
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

            noChange = newText.Equals(articleText);

            return newText;
        }

        /// <summary>
        /// Replaces an image in the article.
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
            if (image.Length == 0)
                return articleText;

            //remove image prefix
            image = Tools.WikiDecode(Regex.Replace(image, "^"
                                                   + Variables.NamespacesCaseInsensitive[Namespace.File], "", RegexOptions.IgnoreCase));

            // make image name first-letter case insensitive
            image = Tools.CaseInsensitive(HttpUtility.UrlDecode(Regex.Escape(image).Replace("\\ ", "[ _]")));

            articleText = FixImages(articleText);

            // look for standard [[Image:blah...]] links
            Regex r = new Regex(@"\[\[\s*:?\s*(?i:"
                                + WikiRegexes.GenerateNamespaceRegex(Namespace.File, Namespace.Media)
                                + @")\s*:\s*" + image + @"((?>[^\[\]]+|\[\[(?<DEPTH>)|\]\](?<-DEPTH>))*(?(DEPTH)(?!)))\]\]", RegexOptions.Singleline);

            // fall back to Image:blah... syntax used in galleries etc., or just image name (infoboxes etc.)
            if (r.Matches(articleText).Count == 0)
                r = new Regex("(" + Variables.NamespacesCaseInsensitive[Namespace.File] + ")?" + image + @"(?: *\|[^\r\n=]+(?=\s*$))?", RegexOptions.Multiline);

            return r.Replace(articleText, m => (commentOut ? "<!-- " + comment + " " + m.Value + " -->" : ""));
        }

        /// <summary>
        /// Removes an image in the article.
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

            noChange = newText.Equals(articleText);

            return newText;
        }

        private static readonly Regex InUniverse = Tools.NestedTemplateRegex(@"In-universe");
        private static readonly Regex CategoryCharacters = new Regex(@"\[\[Category:[^\[\]]*?[Cc]haracters", RegexOptions.Compiled);
        private static readonly Regex SeeAlsoOrMain = Tools.NestedTemplateRegex(new[] { "See also", "Seealso", "Main" });
        private static readonly Regex RefImproveBLP = Tools.NestedTemplateRegex("RefimproveBLP");

        private static readonly Regex IMA = Tools.NestedTemplateRegex(new[] { "Infobox musical artist", "Infobox singer" });

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

        private static readonly Regex BLPUnsourcedSection = Tools.NestedTemplateRegex(new [] { "BLP unsourced section","BLP sources section" });
        private static readonly Regex NotPersonArticles = new Regex(@"(^(((?:First )?(?:Premiership|Presidency|Governor|Mayoralty)|Murder|Atlanta murders|Disappearance|Suicide|Adoption) of|Deaths|The |Second |Brothers |Attack on|[12]\d{3}\b|\d{2,} )|Assembly of|(Birth|Death) rates|(discography|(?:film|bibli)ography| deaths| rebellion| haunting| native| children| campaign(?:, \d+)?| groups| (?:families|boom|case|syndrome|family|murders|people|sisters|brothers|quartet|team|twins|martyrs|mystery|center|\((?:artists|publisher|\w* ?(team|family))\)))(?: \(|$)|[^\(]*\w+,? (and|&|from) \w+|.* (in |Service))", RegexOptions.IgnoreCase);
        private static readonly MetaDataSorter MDS = new MetaDataSorter();
        private static readonly Regex NobleFamilies = new Regex(@"\[\[Category:[^\[\]\|]*(([nN]oble|[Rr]oyal) families| families(\||\]\]))");
        private static readonly Regex NotAboutAPersonCategories = new Regex(@"\[\[Category:(\d{4} (establishments|animal|introductions)|.*(?:Animation|Business|Comedy|Criminal|Entertainer|Filmmaking|Tribes|Screenwriting|Sibling|Sibling musical|Sports|Writing) (duos|trios)|Articles about multiple people|Positions |Groups |Married couples|Fictional|Presidencies|Companies|Military careers|Parables of|[^\[\]\|\r\n]*(?:[Mm]usic(?:al)? groups| bands| gods| groups|(?<!Living) people| troupes| nicknames| given names| pageants| teams and stables| magazines| titles| populated places)|Internet memes|[^\[\]\|\r\n]*Diaspora|Performing groups|Military animals|Collective pseudonyms|Sibling filmmakers|Surnames|Baronies)", RegexOptions.IgnoreCase);
        private static readonly Regex CLSAR = Tools.NestedTemplateRegex(@"Infobox Chinese-language singer and actor");
        private static readonly Regex NotPersonInfoboxes = Tools.NestedTemplateRegex(new [] { "Infobox cricketer tour biography", "Infobox political party", "Infobox settlement", "italic title", "Infobox animal" } );

        /// <summary>
        /// determines whether the article is about a person by looking for persondata/birth death categories, bio stub etc. for en wiki only
        /// Should only return true if the article is the principle article about the individual (not early life/career/discography etc.)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="parseTalkPage"></param>
        /// <returns></returns>
        public static bool IsArticleAboutAPerson(string articleText, string articleTitle, bool parseTalkPage)
        {
            if (!Variables.LangCode.Equals("en")
                || Namespace.Determine(articleTitle).Equals(Namespace.Category)
                || NotPersonArticles.IsMatch(articleTitle)
                || ListOf.IsMatch(articleTitle)
                || articleText.Contains(@"[[fictional character")
                || WikiRegexes.Disambigs.IsMatch(articleText)
                || InUniverse.IsMatch(articleText)
                || NotAboutAPersonCategories.IsMatch(articleText)
                || NobleFamilies.IsMatch(articleText)
                || CategoryCharacters.IsMatch(articleText)
                || WikiRegexes.InfoBox.Match(articleText).Groups[1].Value.ToLower().Contains("organization")
                || NotPersonInfoboxes.IsMatch(articleText)
                || WikiRegexes.SIAs.IsMatch(articleText)
                || WikiRegexes.PeopleInfoboxTemplates.Matches(articleText).Count > 1
               )
                return false;

            Match m2 = IMA.Match(articleText);

            if (m2.Success)
            {
                string MABackground =
                    Tools.GetTemplateParameterValue(m2.Value,
                                                    "Background", true);

                if (MABackground.Contains("band") || MABackground.Contains("classical_ensemble") || MABackground.Contains("temporary"))
                    return false;
            }

            string CLSA = CLSAR.Match(articleText).Value;
            if (CLSA.Length > 0)
            {
                if (Tools.GetTemplateParameterValue(CLSA, "currentmembers").Length > 0
                    || Tools.GetTemplateParameterValue(CLSA, "pastmembers").Length > 0)
                    return false;
            }

            string zerothSection = Tools.GetZerothSection(articleText);

            // not about a person if it's not the principle article on the subject
            if (SeeAlsoOrMain.IsMatch(zerothSection))
                return false;

            // not about one person if multiple different birth or death date templates
            List<string> BD = new List<string>();
            foreach (Match m in BirthDate.Matches(articleText))
            {
                if (BD.Count > 0 && !BD.Contains(m.Value))
                    return false;

                BD.Add(m.Value);
            }

            List<string> DD = new List<string>();
            foreach (Match m in DeathDate.Matches(articleText))
            {
                if (DD.Count > 0 && !DD.Contains(m.Value))
                    return false;

                DD.Add(m.Value);
            }

            // fix for duplicate living people categories being miscounted as article about multiple people
            string cats = MDS.RemoveCats(ref articleText, articleTitle);
            articleText += cats;

            if (WikiRegexes.DeathsOrLivingCategory.Matches(cats).Count > 1)
                return false;

            if (WikiRegexes.Persondata.Matches(articleText).Count == 1
                || articleText.Contains(@"-bio-stub}}")
                || articleText.Contains(@"-politician-stub}}")
                || articleText.Contains(@"-writer-stub}}")
                || cats.Contains(CategoryLivingPeople)
                || WikiRegexes.PeopleInfoboxTemplates.Matches(zerothSection).Count == 1
                || CategoryMatch(cats, YearOfBirthMissingLivingPeople))
                return true;

            // articles with bold linking to another article may be linking to the main article on the person the article is about
            // e.g. '''military career of [[Napoleon Bonaparte]]'''
            string zerothSectionNoTemplates = WikiRegexes.Template.Replace(zerothSection, "");
            foreach (Match m in WikiRegexes.Bold.Matches(zerothSectionNoTemplates))
            {
                if (WikiRegexes.WikiLink.IsMatch(m.Value))
                    return false;
            }

            int dateBirthAndAgeCount = BirthDate.Matches(zerothSection).Count;
            int dateDeathCount = DeathDate.Matches(zerothSection).Count;

            if (dateBirthAndAgeCount == 1 || dateDeathCount == 1)
                return true;

            if (WikiRegexes.InfoBox.IsMatch(zerothSection) && !WikiRegexes.PeopleInfoboxTemplates.IsMatch(zerothSection))
                return false;

            return WikiRegexes.DeathsOrLivingCategory.IsMatch(cats)
                || WikiRegexes.LivingPeopleRegex2.IsMatch(articleText)
                || WikiRegexes.BirthsCategory.IsMatch(cats)
                || WikiRegexes.PeopleFromCategory.IsMatch(cats)
                || WikiRegexes.BLPSources.IsMatch(BLPUnsourcedSection.Replace(articleText, ""))
                || RefImproveBLP.IsMatch(articleText);
        }

        /// <summary>
        /// Adds [[Category:Living people]] to articles with a [[Category:XXXX births]] and no living people/deaths category, taking sortkey from births category if present
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The page title of the article.</param>
        /// <param name="noChange"></param>
        /// <returns></returns>
        public static string LivingPeople(string articleText, string articleTitle, out bool noChange)
        {
            string newText = LivingPeople(articleText, articleTitle);

            noChange = newText.Equals(articleText);

            return newText;
        }

        private static readonly Regex ThreeOrMoreDigits = new Regex(@"[0-9]{3,}", RegexOptions.Compiled);
        private static readonly Regex BirthsSortKey = new Regex(@"\|.*?\]\]", RegexOptions.Compiled);
        /// <summary>
        /// Adds [[Category:Living people]] to articles with a [[Category:XXXX births]] and no living people/deaths category, taking sortkey from births category if present
        /// When page is not mainspace, adds [[:Category rather than [[Category
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The page title of the article.</param>
        /// <returns>The updated article text.</returns>
        public static string LivingPeople(string articleText, string articleTitle)
        {
            if(Variables.LangCode.Equals("sco"))
                return articleText;

            Match m = WikiRegexes.BirthsCategory.Match(GetCats(articleText));

            // do not add living people category unless 'XXXX births' category is present
            if (!m.Success)
                return articleText;

            // do not add living people category if already dead, or thought to be dead
            if (WikiRegexes.DeathsOrLivingCategory.IsMatch(articleText) || WikiRegexes.LivingPeopleRegex2.IsMatch(articleText) ||
                Regex.IsMatch(articleText, BornDeathRegex.ToString().Replace("^", @"('''(?:[^']+|.*?[^'])'''\s*\()"))
                || Regex.IsMatch(articleText, DiedDateRegex.ToString().Replace("^", @"('''(?:[^']+|.*?[^'])'''\s*\()")))
                return articleText;
            
            string birthCat = m.Value;
            int birthYear = 0;

            string byear = m.Groups[1].Value;

            if (ThreeOrMoreDigits.IsMatch(byear))
                birthYear = int.Parse(byear);

            // per [[:Category:Living people]] and [[WP:BDP]], do not apply if born > 115 years ago
            if (birthYear < (DateTime.Now.Year - 115))
                return articleText;

            // use any sortkey from 'XXXX births' category
            string catKey = birthCat.Contains("|") ? BirthsSortKey.Match(birthCat).Value : "]]";

            return articleText + "[[" + (Namespace.IsMainSpace(articleTitle) ? "" : ":") + "Category:Living people" + catKey;
        }

        private static readonly Regex PersonYearOfBirth = new Regex(@"(?<='''.{0,100}?)\( *[Bb]orn[^\)\.;]{1,150}?(?<!.*(?:[Dd]ied|&[nm]dash;|—).*)([12]?\d{3}(?: BC)?)\b[^\)]{0,200}");
        private static readonly Regex PersonYearOfDeath = new Regex(@"(?<='''.{0,100}?)\([^\(\)]*?[Dd]ied[^\)\.;]+?([12]?\d{3}(?: BC)?)\b");
        private static readonly Regex PersonYearOfBirthAndDeath = new Regex(@"^.{0,100}?'''\s*\([^\)\r\n]*?(?<![Dd]ied)\b([12]?\d{3})\b[^\)\r\n]*?(-|–|—|&[nm]dash;)[^\)\r\n]*?([12]?\d{3})\b[^\)]{0,200}", RegexOptions.Singleline );

        private static readonly Regex UncertainWordings = new Regex(@"(?:\b(about|abt|approx\.?|before|\?\?\?|by|or \d+|later|after|near|either|probably|missing|prior to|around|late|[Cc]irca|between|be?tw\.?|[Bb]irth based on age as of date|\d{3,4}(?:\]\])?/(?:\[\[)?\d{1,4}|or +(?:\[\[)?\d{3,})\b|\d{3} *\?|\bca?(?:'')?\.|\b[Cc]a?\b|\b(bef|abt|est)\.|~|/|''fl''\.?)", RegexOptions.IgnoreCase);
        private static readonly Regex ReignedRuledUnsure = new Regex(@"(?:\?|[Rr](?:uled|eign(?:ed)?\b)|\br\.|(chr|fl(?:\]\])?)\.|\b(?:[Ff]lo(?:urished|ruit)|active|baptized)\b)");


        private static readonly Regex AgeBrackets = new Regex(@"(?:< *[Bb][Rr] */? *> *)?\s*[,;]?\s*\(? *[Aa]ged? +\d{1,3}(?: +or +\d{1,3})? *\)?$", RegexOptions.Compiled);

        /// <summary>
        /// takes input string of date and age e.g. "11 May 1990 (age 21)" and converts to {{birth date and age|1990|5|11}}
        /// </summary>
        /// <param name="dateandage"></param>
        /// <returns></returns>
        public static string FormatToBDA(string dateandage)
        {
            Parsers p = new Parsers();
            string original = dateandage;
            // clean up date format if possible
            dateandage = p.FixDateOrdinalsAndOf(" " + dateandage, "test");

            // remove date wikilinks
            dateandage = WikiRegexes.WikiLinksOnlyPossiblePipe.Replace(dateandage, "$1").Trim();

            // string must end with (age xx)
            if (!AgeBrackets.IsMatch(dateandage))
                return original;

            dateandage = AgeBrackets.Replace(dateandage, "");

            string ISODate = Tools.ConvertDate(dateandage, DateLocale.ISO);

            if (ISODate.Equals(dateandage) && !WikiRegexes.ISODates.IsMatch(dateandage))
                return original;

            bool AmericanDate = WikiRegexes.AmericanDates.IsMatch(dateandage);

            // we have ISO date, convert with {{birth date and age}}, American date, set mf=y
            return @"{{birth date and age|" + (AmericanDate ? "mf=y|" : "df=y|") + ISODate.Replace("-", "|") + @"}}";
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

            noChange = newText.Equals(articleText);

            return newText;
        }

        /// <summary>
        /// Replaces legacy/deprecated language codes in interwikis with correct ones
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns>Page text</returns>
        public static string InterwikiConversions(string articleText)
        {
            List<string> possibleInterwiki = GetAllWikiLinks(articleText).FindAll(l => l.Contains(":"));

            if(possibleInterwiki.Any(l => l.StartsWith(@"[[zh-tw:")))
                articleText = articleText.Replace("[[zh-tw:", "[[zh:");
            if(possibleInterwiki.Any(l => l.StartsWith(@"[[nb:")))
                articleText = articleText.Replace("[[nb:", "[[no:");
            if(possibleInterwiki.Any(l => l.StartsWith(@"[[dk:")))
                articleText = articleText.Replace("[[dk:", "[[da:");

            return articleText;
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

            noChange = newText.Equals(articleText);

            return newText;
        }

        private static readonly Regex MultipleIssuesDateRemoval = new Regex(@"(?<={{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues\s*(?:\|[^{}]*?)?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}[^{}]*?){0,4}\|[^{}\|]{3,}?)\b(?i)date(?<!.*out of date)", RegexOptions.Compiled);
        private static readonly Regex NoFootnotes = Tools.NestedTemplateRegex("no footnotes");
        private static readonly Regex ConversionsCnCommons = Tools.NestedTemplateRegex( new [] {"citation needed", "commons", "commons cat", "commons category" });
        private const string CategoryLivingPeople = @"[[Category:Living people";

        /// <summary>
        /// Converts/subst'd some deprecated templates
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The new article text.</returns>
        public static string Conversions(string articleText)
        {
            articleText = articleText.Replace("{{msg:", "{{");

            // Performance: get all the templates so template changing functions below only called when template(s) present in article
            List<string> alltemplates = GetAllTemplates(articleText);

            bool mifound = TemplateExists(alltemplates, WikiRegexes.MultipleIssues);

            if(mifound || TemplateExists(alltemplates, ConversionsCnCommons))
            {
                foreach (KeyValuePair<Regex, string> k in RegexConversion)
                {
                    articleText = k.Key.Replace(articleText, k.Value);
                }
            }

            bool BASEPAGENAMEInRefs = false;

            if(TemplateExists(alltemplates, WikiRegexes.BASEPAGENAMETemplates))
            {
                foreach (Match m in WikiRegexes.Refs.Matches(articleText))
                {
                    if (WikiRegexes.BASEPAGENAMETemplates.IsMatch(m.Value))
                    {
                        BASEPAGENAMEInRefs = true;
                        break;
                    }
                }

                if (!BASEPAGENAMEInRefs)
                {
                    foreach (string T in WikiRegexes.BASEPAGENAMETemplatesL)
                        articleText = Tools.RenameTemplate(articleText, T, "subst:" + T);
                }
            }

            // {{no footnotes}} --> {{more footnotes}}, if some <ref>...</ref> or {{sfn}} references in article, uses regex from WikiRegexes.Refs
            // does not change templates with section / reason tags
            if (TemplateExists(alltemplates, NoFootnotes) && (TemplateExists(alltemplates, Footnote) || TotalRefsNotGrouped(articleText) > 0))
                articleText = NoFootnotes.Replace(articleText, m => OnlyArticleBLPTemplateME(m, "more footnotes"));

            // {{foo|section|...}} --> {{foo section|...}} for unreferenced, wikify, refimprove, BLPsources, expand, BLP unsourced
            if(TemplateExists(alltemplates, SectionTemplates))
                articleText = SectionTemplates.Replace(articleText, SectionTemplateConversionsME);

            if(mifound)
            {
                articleText = WikiRegexes.MultipleIssues.Replace(articleText, m =>
                                                             {
                                                                 return Tools.RemoveExcessTemplatePipes(m.Value);
                                                             });

                // clean any 'date' word within {{Multiple issues}} (but not 'update' or 'out of date' fields), place after the date adding rule above (loop due to lookbehind in regex)
                while (MultipleIssuesDateRemoval.IsMatch(articleText))
                    articleText = MultipleIssuesDateRemoval.Replace(articleText, "");
            }

            // fixes if article has [[Category:Living people]]
            if(Variables.IsWikipediaEN && CategoryMatch(articleText, "Living people"))
            {
                // {{unreferenced}} --> {{BLP unsourced}} if article has [[Category:Living people]], and no free-text first argument to {{unref}}
                MatchCollection unrefm = Tools.NestedTemplateRegex("unreferenced").Matches(articleText);
                if(unrefm.Count == 1)
                {
                    if (Tools.TurnFirstToLower(Tools.GetTemplateArgument(unrefm[0].Value, 1)).StartsWith("date")
                        || Tools.GetTemplateArgumentCount(unrefm[0].Value) == 0)
                        articleText = Tools.RenameTemplate(articleText, "unreferenced", "BLP unsourced", false);
                }

                articleText = Tools.RenameTemplate(articleText, "unreferenced section", "BLP unsourced section", false);
                articleText = Tools.RenameTemplate(articleText, "primary sources", "BLP primary sources", false);

                // {{refimprove}} --> {{BLP sources}} if article has [[Category:Living people]], and no free-text first argument to {{refimprove}}
                MatchCollection mc = Tools.NestedTemplateRegex("refimprove").Matches(articleText);
                if(mc.Count == 1)
                {
                    string refimprove = mc[0].Value;
                    if ((Tools.TurnFirstToLower(Tools.GetTemplateArgument(refimprove, 1)).StartsWith("date")
                         || Tools.GetTemplateArgumentCount(refimprove) == 0))
                        {
                            // if also have existing BLP sources then remove refimprove
                            if(Tools.NestedTemplateRegex("BLP sources").IsMatch(articleText))
                            articleText = Tools.NestedTemplateRegex("refimprove").Replace(articleText, "");
                            else
                            articleText = Tools.RenameTemplate(articleText, "refimprove", "BLP sources", false);
                        }
                }

                articleText = Tools.RenameTemplate(articleText, "refimprove section", "BLP sources section", false);
            }

            if(TemplateExists(alltemplates, WikiRegexes.PortalTemplate))
                articleText = MergePortals(articleText);

            if(alltemplates.Count(s => SectionMergedTemplatesR.IsMatch(@"{{" + s + "|}}")) > 1)
                articleText = MergeTemplatesBySection(articleText);

            // clean up Template:/underscores in infobox names
            if(TemplateExists(alltemplates, WikiRegexes.InfoBox))
                articleText = WikiRegexes.InfoBox.Replace(articleText, m =>
                                                      {
                                                          string newName = CanonicalizeTitle(m.Groups[1].Value);
                                                          return (newName.Equals(m.Groups[1].Value) ? m.Value : Tools.RenameTemplate(m.Value, newName));
                                                      });

            if(TemplateExists(alltemplates, WikiRegexes.Dablinks))
                articleText = Dablinks(articleText);

            return articleText;
        }

        private static readonly Regex SectionTemplates = Tools.NestedTemplateRegex(new[] { "unreferenced", "refimprove", "BLP sources", "expand", "BLP unsourced" });

        /// <summary>
        /// Converts templates such as {{foo|section|...}} to {{foo section|...}}
        /// </summary>
        /// <param name="m">Template call</param>
        /// <returns>The updated emplate call</returns>
        private static string SectionTemplateConversionsME(Match m)
        {
            string newValue = m.Value, existingName = Tools.GetTemplateName(newValue);
            if (Tools.GetTemplateArgument(newValue, 1).Equals("section") || Tools.GetTemplateArgument(newValue, 1).Equals("Section") || Tools.GetTemplateParameterValue(newValue, "type").ToLower().Equals("section"))
                newValue = Tools.RenameTemplate(Regex.Replace(newValue, @"\|\s*[Ss]ection\s*\|", "|"), existingName + " section");

            if (Tools.GetTemplateParameterValue(newValue, "type").ToLower().Equals("section"))
            {
                newValue = Tools.RemoveTemplateParameter(newValue, "section");
                newValue = Tools.RenameTemplate(Regex.Replace(newValue, @"\|\s*type\s*=\s*[Ss]ection\s*\|", "|"), existingName + " section");
            }

            // for {{Unreferenced}} additionally convert list parameter
            if (existingName.ToLower().Equals("unreferenced") && Tools.GetTemplateArgument(newValue, 1).Equals("list"))
                newValue = Tools.RenameTemplate(Regex.Replace(newValue, @"\|\s*list\s*\|", "|"), existingName + " section");

            // for {{Unreferenced}} auto=yes is deprecated parameter per [[Template:Unreferenced_stub#How_to_use]]
            if (existingName.ToLower().Equals("unreferenced") && Tools.GetTemplateParameterValue(newValue, "auto").ToLower().Equals("yes"))
                newValue = Tools.RemoveTemplateParameter(newValue, "auto");

            return newValue;
        }

        /// <summary>
        /// Renames template if the only name arguments are BLP=, date= and article=, or there are no arguments
        /// </summary>
        /// <param name="m"></param>
        /// <param name="newTemplateName"></param>
        /// <returns></returns>
        private static string OnlyArticleBLPTemplateME(Match m, string newTemplateName)
        {
            string newValue = Tools.RemoveTemplateParameter(m.Value, "BLP");
            newValue = Tools.RemoveTemplateParameter(newValue, "article");
            newValue = Tools.RemoveTemplateParameter(newValue, "date");

            if (Tools.GetTemplateArgumentCount(newValue) > 0)
                return m.Value;

            return Tools.RenameTemplate(m.Value, newTemplateName);
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
            articleText = WikiRegexes.UnformattedText.Replace(articleText, "");
            Match bot = BotsAllow.Match(articleText);

            if (bot.Success)
            {
                return
                    (Regex.IsMatch(bot.Groups[1].Value,
                                   @"(?:^|,)\s*(?:" + user.Normalize() + @"|awb)\s*(?:,|$)", RegexOptions.IgnoreCase));
            }

            return
                !Regex.IsMatch(articleText,
                               @"\{\{\s*(?:nobots|(nobots|bots)\|(allow=none|deny=(?!none).*(" + user.Normalize() +
                               @"|awb|all)|optout=all))\s*(?:\}\}|,)", RegexOptions.IgnoreCase);
        }

        private static readonly Regex ExtToIn = new Regex(@"(?<![*#:;]{2})\[https?://([a-z0-9\-]{2})\.(?:(wikt)ionary|wiki(n)ews|wiki(b)ooks|wiki(q)uote|wiki(s)ource|wiki(v)ersity|(w)ikipedia)\.(?:com|net|org)/w(?:iki)?/([^][{|}\s""]*) +([^\n\]]+)\]", RegexOptions.Compiled);
        private static readonly Regex MetaCommonsIncubatorQualityExternalLink = new Regex(@"(?<![*#:;]{2})\[http://(?:(m)eta|(commons)|(incubator)|(quality))\.wikimedia\.(?:com|net|org)/w(?:iki)?/([^][{|}\s""]*) +([^\n\]]+)\]", RegexOptions.Compiled);
        private static readonly Regex WikiaExternalLink = new Regex(@"(?<![*#:;]{2})\[http://([a-z0-9\-]+)\.wikia\.(?:com|net|org)/wiki/([^][{|}\s""]+) +([^\n\]]+)\]", RegexOptions.Compiled);

        // Covered by UtilityFunctionTests.ExternalURLToInternalLink(), incomplete
        /// <summary>
        /// Converts external links to Wikimedia projects into internal links
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static string ExternalURLToInternalLink(string articleText)
        {
            // TODO wikitravel support?
            articleText = ExtToIn.Replace(articleText, "[[$2$3$4$5$6$7$8:$1:$9|$10]]");
            articleText = MetaCommonsIncubatorQualityExternalLink.Replace(articleText, "[[$1$2$3$4:$5|$6]]");
            articleText = WikiaExternalLink.Replace(articleText, "[[wikia:$1:$2|$3]]");

            Regex SameLanguageLink = new Regex(@"(\[\[(?:wikt|[nbqsvw]):)" + Variables.LangCode + @":([^\[\]\|]+\|[^\[\]\|]+\]\])");

            return SameLanguageLink.Replace(articleText, "$1$2");
        }

        private static readonly Regex coinsR = new Regex(@"""coins"": ""([^""]+)");
        private static readonly Regex coinsParam = new Regex(@"[&;]rft\.([a-z0-9]+)=([^&]+)");

        /// <summary>
        /// Returns a dictionary of COinS parameter and value from the input text
        /// Only first set of COinS data is processed
        /// </summary>
        /// <param name="text">Raw HTML containing COinS data</param>
        /// <returns></returns>
        public static Dictionary<string, string> ExtractCOinS(string text)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            // match only the COinS section of the text
            text = coinsR.Match(text).Groups[1].Value;
            text = HttpUtility.UrlDecode(text);

            foreach(Match m in coinsParam.Matches(text))
            {
                if(!data.ContainsKey(m.Groups[1].Value))
                    data.Add(m.Groups[1].Value, m.Groups[2].Value);
            }

            return data;
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
            if (!Variables.LangCode.Equals("en"))
                return false;

            return WikiRegexes.InfoBox.IsMatch(WikiRegexes.UnformattedText.Replace(articleText, ""));
        }

        /// <summary>
        /// Check if article has an 'in use' or 'in creation' tag
        /// </summary>
        public static bool IsInUse(string articleText)
        {
            return WikiRegexes.InUse.IsMatch(WikiRegexes.UnformattedText.Replace(articleText, ""));
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
            return WikiRegexes.DeadLink.IsMatch(WikiRegexes.Comments.Replace(articleText, ""));
        }

        /// <summary>
        /// Returns whether the input article text contains any wikilinks with no target
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static bool HasTargetLessLinks(string articleText)
        {
            return WikiRegexes.TargetLessLink.IsMatch(WikiRegexes.Comments.Replace(articleText, ""));
        }

        /// <summary>
        /// Returns whether the input article text contains any wikilinks with double pipes
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        public static bool HasDoublePipeLinks(string articleText)
        {
            return WikiRegexes.DoublePipeLink.IsMatch(WikiRegexes.Comments.Replace(articleText, ""));
        }

        /// <summary>
        /// Check if the article contains a {{no footnotes}} or {{more footnotes}} template but has 5+ &lt;ref>...&lt;/ref> references
        /// </summary>
        public static bool HasMorefootnotesAndManyReferences(string articleText)
        {
            return (WikiRegexes.MoreNoFootnotes.IsMatch(WikiRegexes.Comments.Replace(string.Join("", GetAllTemplateDetail(articleText).ToArray()), "")) && WikiRegexes.Refs.Matches(articleText).Count > 4);
        }

        private static readonly Regex GRTemplateDecimal = new Regex(@"{{GR\|\d}}", RegexOptions.Compiled);
        private static readonly Regex ReflistQuick = new Regex(@"\{\{\s*(?:ref(?:-?li(?:st|nk)|erence)|[Ll]istaref)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Searches for link to user and/or user talk namespace
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>Dictionary of links to user or user talk namespace</returns>
        public static Dictionary<int, int> UserSignature(string articleText)
        {
            return DictionaryOfMatches(articleText, WikiRegexes.UserSignature);
        }

        /// <summary>
        /// Check if the article uses cite references but has no recognised template to display the references; only for en-wiki
        /// </summary>
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.28Yet.29_more_reference_related_changes.
        public static bool IsMissingReferencesDisplay(string articleText)
        {
            if (!Variables.LangCode.Equals("en") || TemplateExists(GetAllTemplates(articleText), ReflistQuick))
                return false;

            // list-defined references with unbalanced { or } in cite template can cause ReferencesTemplate not to match, hence quick check
            return !ReflistQuick.IsMatch(articleText) && !WikiRegexes.ReferencesTemplate.IsMatch(articleText) && (TotalRefsNotGrouped(articleText) > 0 | GRTemplateDecimal.IsMatch(articleText));
        }

        /// <summary>
        /// Check if the article contains a &lt;ref>...&lt;/ref> reference after the {{reflist}} to show them
        /// </summary>
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#.28Yet.29_more_reference_related_changes.
        public static bool HasRefAfterReflist(string articleText)
        {
             if (!Variables.LangCode.Equals("en"))
                return false;

            int refstemplateindex = 0, reflength = 0;
            foreach(Match m in WikiRegexes.ReferencesTemplate.Matches(articleText))
            {
                if(refstemplateindex > 0)
                    return false; // multiple {{reflist}} etc. in page, not supported for check

                refstemplateindex= m.Index;
                reflength = m.Length;
            }
            articleText = articleText.Substring(refstemplateindex+reflength);
            articleText = WikiRegexes.Comments.Replace(articleText, "");

            return Regex.IsMatch(articleText, WikiRegexes.ReferenceEndGR);
        }

        /// <summary>
        /// Returns true if the article contains bare external links in the references section (just the URL link on a line with no description/name)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Format_references
        public static bool HasBareReferences(string articleText)
        {
            int referencesIndex = WikiRegexes.ReferencesHeader.Match(articleText).Index;

            if (referencesIndex < 2)
                return false;

            int externalLinksIndex =
                WikiRegexes.ExternalLinksHeader.Match(articleText).Index;

            // get the references section: to external links or end of article, whichever is earlier
            string refsArea = externalLinksIndex > referencesIndex
                ? articleText.Substring(referencesIndex, (externalLinksIndex - referencesIndex))
                : articleText.Substring(referencesIndex);

            return (WikiRegexes.BareExternalLink.IsMatch(refsArea));
        }

        #endregion
    }
}