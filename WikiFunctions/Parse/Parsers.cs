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
            RegexConversion.Add(new Regex(@"\{\{\s*[Cc]ommons\s?\|\s*[Cc]ategory:\s*([^{}]+?)\s*\}\}", RegexOptions.Compiled), @"{{Commons category|$1}}");

            //https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Commons_category
            RegexConversion.Add(new Regex(@"({{[Cc]ommons cat(?:egory)?\|\s*)([^{}\|]+?)\s*\|\s*\2\s*}}", RegexOptions.Compiled), @"$1$2}}");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_empty_.7B.7BArticle_issues.7D.7D
            // article issues with no issues -> remove tag
            RegexConversion.Add(new Regex(@"\{\{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues(?:\s*\|\s*(?:section|article)\s*=\s*[Yy])?\s*\}\}", RegexOptions.Compiled), "");

            // remove duplicate / populated and null fields in cite/multiple issues templates
            RegexConversion.Add(new Regex(@"({{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues[^{}]*\|\s*)(\w+)\s*=\s*([^\|}{]+?)\s*\|((?:[^{}]*?\|)?\s*)\2(\s*=\s*)\3(\s*(\||\}\}))", RegexOptions.Compiled), "$1$4$2$5$3$6"); // duplicate field remover for cite templates
            RegexConversion.Add(new Regex(@"(\{\{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues[^{}]*\|\s*)(\w+)(\s*=\s*[^\|}{]+(?:\|[^{}]+?)?)\|\s*\2\s*=\s*(\||\}\})", RegexOptions.Compiled), "$1$2$3$4"); // 'field=populated | field=null' drop field=null
            RegexConversion.Add(new Regex(@"(\{\{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues[^{}]*\|\s*)(\w+)\s*=\s*\|\s*((?:[^{}]+?\|)?\s*\2\s*=\s*[^\|}{\s])", RegexOptions.Compiled), "$1$3"); // 'field=null | field=populated' drop field=null

            RegexConversion.Add(new Regex(@"({{\s*[Cc]itation needed\s*\|)\s*(?:[Dd]ate:)?([A-Z][a-z]+ 20\d\d)\s*\|\s*(date\s*=\s*\2\s*}})", RegexOptions.Compiled | RegexOptions.IgnoreCase), @"$1$3");

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
        /// <param name="fixOptionalWhitespace">Whether to request optional excess whitespace to be fixed</param>
        /// <returns>The re-organised text.</returns>
        public string SortMetaData(string articleText, string articleTitle, bool fixOptionalWhitespace)
        {
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Substituted_templates
            // if article contains some substituted template stuff, sorting the data may mess it up (further)
            if (Namespace.IsMainSpace(articleTitle) && NoIncludeIncludeOnlyProgrammingElement(articleText))
                return articleText;

            return (Variables.Project <= ProjectEnum.species) ? Sorter.Sort(articleText, articleTitle, fixOptionalWhitespace) : articleText;
        }

        private static readonly Regex RegexHeadingsSeeAlso = new Regex("^(== *)(?:see also|related topics|related articles|internal links|also see):?( *==)", RegexOptions.IgnoreCase);
        private static readonly Regex RegexHeadingsExternalLink = new Regex("(== *)(external links?|external sites?|outside links?|web ?links?|exterior links?):?( *==)", RegexOptions.IgnoreCase);
        private static readonly Regex RegexHeadingsReferencess = new Regex("(== *)(?:reff?e?rr?en[sc]es?:?)( *==)", RegexOptions.IgnoreCase);
        private static readonly Regex RegexHeadingsSources = new Regex("(== *)(?:sources?:?)( *==)", RegexOptions.IgnoreCase);
        private static readonly Regex RegexHeadingsFurtherReading = new Regex("(== *)(further readings?:?)( *==)", RegexOptions.IgnoreCase);
        private static readonly Regex RegexHeadingsLife = new Regex("(== *)(Early|Personal|Adult|Later) Life( *==)", RegexOptions.IgnoreCase);
        private static readonly Regex RegexHeadingsMembers = new Regex("(== *)(Current|Past|Prior) Members( *==)", RegexOptions.IgnoreCase);
        private static readonly Regex RegexHeadingsBold = new Regex(@"^(=+\s*)(?:'''|<[Bb]>)(.*?)(?:'''|</[Bb]>)(\s*=+\s*)$");
        private static readonly Regex RegexHeadingsTrackListing = new Regex("(== *)track listing( *==)", RegexOptions.IgnoreCase);
        private static readonly Regex RegexHeadingsLifeCareer = new Regex("(== *)Life and Career( *==)", RegexOptions.IgnoreCase);
        private static readonly Regex RegexHeadingsCareer = new Regex("(== ?)([a-zA-Z]+) Career( ?==)", RegexOptions.IgnoreCase);

        private static readonly Regex RegexBadHeaderStartOfAticle = new Regex("^={1,4} ?'*(about|description|overview|definition|profile|(?:general )?information|background|intro(?:duction)?|summary|bio(?:graphy)?)'* ?={1,4}", RegexOptions.IgnoreCase);

        private static readonly Regex RegexHeadingUpOneLevel = new Regex(@"^=(==+[^=].*?[^=]==+)=(\r\n?|\n)$", RegexOptions.Multiline);
        private static readonly Regex ReferencesExternalLinksSeeAlso = new Regex(@"== *([Rr]eferences|[Ee]xternal +[Ll]inks?|[Ss]ee +[Aa]lso) *==\s");
        private static readonly Regex ReferencesExternalLinksSeeAlsoUnbalancedRight = new Regex(@"(== *(?:[Rr]eferences|[Ee]xternal +[Ll]inks?|[Ss]ee +[Aa]lso) *=) *\r\n");

        private static readonly Regex RegexHeadingColonAtEnd = new Regex(@"^(=+)(\s*[^=\s].*?)\:(\s*\1\s*)$");
        private static readonly Regex RegexHeadingWithBold = new Regex(@"(?<====+.*?)(?:'''|<[Bb]>)(.*?)(?:'''|</[Bb]>)(?=.*?===+)");

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

            noChange = newText.Equals(articleText);

            return newText.Trim();
        }

        private static readonly Regex ListOf = new Regex(@"^Lists? of", RegexOptions.Compiled);
        
        private static readonly Regex Anchor2NewlineHeader = new Regex(Tools.NestedTemplateRegex("Anchor") + "\r\n(\r\n)+==", RegexOptions.Multiline);
        private static readonly Regex HeadingsIncorrectWhitespaceBefore = new Regex(@"(?<=\S *(?:(\r\n){3,}|\r\n|\s*< *[Bb][Rr] *\/? *>\s*) *)=");

        // Covered by: FormattingTests.TestFixHeadings(), incomplete
        /// <summary>
        /// Fix ==See also== and similar section common errors. Removes unecessary introductory headings and cleans excess whitespace (but not the optional single space at the start & end of headings).
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">the title of the article</param>
        /// <returns>The modified article text.</returns>
        public static string FixHeadings(string articleText, string articleTitle)
        {
            // remove unnecessary general header from start of article
            articleText = RegexBadHeaderStartOfAticle.Replace(articleText, "");

            // one blank line before each heading per MOS:HEAD
            // avoid special case of indented text that may be code with lost of == that matches a heading
            if (Variables.IsWikipediaEN)
            {
                articleText = Anchor2NewlineHeader.Replace(articleText, m => m.Value.Replace("\r\n\r\n==", "\r\n=="));
                if(HeadingsIncorrectWhitespaceBefore.IsMatch(articleText)) // check for performance
                    articleText = WikiRegexes.HeadingsWhitespaceBefore.Replace(articleText, m => m.Groups[2].Value.Contains("==") ? m.Value : "\r\n\r\n" + m.Groups[1].Value);
            }

            // Fixes after this do not need to be applied to zeroth section (which may be very long on some articles), so for performance only appply to text after zeroth section
            string zerothSection = Tools.GetZerothSection(articleText);
            string restOfArticle = articleText.Substring(zerothSection.Length);
            articleText = restOfArticle;

            // Removes level 2 heading if it matches pagetitle
            articleText = Regex.Replace(articleText, @"^(==) *" + Regex.Escape(articleTitle) + @" *\1\r\n", "", RegexOptions.Multiline);

            articleText = WikiRegexes.Headings.Replace(articleText, m => FixHeadingsME(m));

            // CHECKWIKI error 8. Add missing = in some headers
            articleText = ReferencesExternalLinksSeeAlsoUnbalancedRight.Replace(articleText, "$1=\r\n");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Section_header_level_.28WikiProject_Check_Wikipedia_.237.29
            // CHECKWIKI error 7
            // if no level 2 heading in article, remove a level from all headings (i.e. '===blah===' to '==blah==' etc.)
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Standard_level_2_headers
            // don't consider the "references", "see also", or "external links" level 2 headings when counting level two headings
           	// only apply if all level 3 headings and lower are before the first of references/external links/see also
            if(Namespace.IsMainSpace(articleTitle))
            {
                List<string> theHeadings = (from Match m in WikiRegexes.Headings.Matches(articleText)
                                                        where !ReferencesExternalLinksSeeAlso.IsMatch(m.Value)
                                                        select m.Value).ToList();
                if(!theHeadings.Any(s => WikiRegexes.HeadingLevelTwo.IsMatch(s)))
                {
                    string articleTextLocal = articleText;
                    articleTextLocal = ReferencesExternalLinksSeeAlso.Replace(articleTextLocal, "");

                    string originalarticleText = "";
                    while(!originalarticleText.Equals(articleText))
                    {
                        originalarticleText = articleText;
                        if(!WikiRegexes.HeadingLevelTwo.IsMatch(articleTextLocal))
                        {
                            // get index of last level 3+ heading
                            int upone = 0;
                            foreach(Match m in RegexHeadingUpOneLevel.Matches(articleText))
                            {
                                if(m.Index > upone)
                                    upone = m.Index;
                            }

                            if(!ReferencesExternalLinksSeeAlso.IsMatch(articleText) || (upone < ReferencesExternalLinksSeeAlso.Match(articleText).Index))
                                articleText = RegexHeadingUpOneLevel.Replace(articleText, "$1$2");
                        }

                        articleTextLocal = ReferencesExternalLinksSeeAlso.Replace(articleText, "");
                    }
                }
            }

            return zerothSection + articleText;
        }

        private static readonly Regex SpaceNewLineEnd = new Regex(@" +(\s+)$");

        /// <summary>
        /// Performs various fixes to headings
        /// </summary>
        private static string FixHeadingsME(Match m)
        {
            string hAfter = WikiRegexes.Br.Replace(m.Value, "");
            hAfter = WikiRegexes.Big.Replace(hAfter, "$1").TrimStart(' ');

            // clean whitespace
            hAfter = hAfter.Replace("\t", " ");
            while(SpaceNewLineEnd.IsMatch(hAfter))
                hAfter = SpaceNewLineEnd.Replace(hAfter, "$1");

            // Removes bold from heading - CHECKWIKI error 44
            hAfter = RegexHeadingsBold.Replace(hAfter, "$1$2$3");

            // Removes colon at end of heading  - CHECKWIKI error 57
            hAfter = RegexHeadingColonAtEnd.Replace(hAfter, "$1$2$3");

            hAfter = RegexHeadingsExternalLink.Replace(hAfter, "$1External links$3");

            hAfter = RegexHeadingsFurtherReading.Replace(hAfter, "$1Further reading$3");
            hAfter = RegexHeadingsLife.Replace(hAfter, "$1$2 life$3");
            hAfter = RegexHeadingsMembers.Replace(hAfter, "$1$2 members$3");
            hAfter = RegexHeadingsTrackListing.Replace(hAfter, "$1Track listing$2");
            hAfter = RegexHeadingsLifeCareer.Replace(hAfter, "$1Life and career$2");
            hAfter = RegexHeadingsCareer.Replace(hAfter, "$1$2 career$3");
            hAfter = RegexHeadingsSeeAlso.Replace(hAfter, "$1See also$2");

            // Plural per [[WP:FNNR]]
            hAfter = RegexHeadingsReferencess.Replace(hAfter, m2=> m2.Groups[1].Value + "References" + m2.Groups[2].Value);
            hAfter = RegexHeadingsSources.Replace(hAfter, m2=> m2.Groups[1].Value + "Sources" + m2.Groups[2].Value);

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Bold_text_in_headers
            // Removes bold from level 3 headers and below, as it makes no visible difference
            hAfter = RegexHeadingWithBold.Replace(hAfter, "$1");

            hAfter = WikiRegexes.EmptyBold.Replace(hAfter, "");

            return hAfter;
        }

        private const int MinCleanupTagsToCombine = 2; // article must have at least this many tags to combine to {{multiple issues}}
        private static readonly Regex ExpertSubject = Tools.NestedTemplateRegex("expert-subject");

        /// <summary>
        /// Combines multiple cleanup tags into {{multiple issues}} template, ensures parameters have correct case, removes date parameter where not needed
        /// only for English-language wikis
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public string MultipleIssuesOld(string articleText)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            // convert title case parameters within {{Multiple issues}} to lower case
            foreach (Match m in WikiRegexes.MultipleIssuesInTitleCase.Matches(articleText))
            {
                string firstPart = m.Groups[1].Value;
                string parameterFirstChar = m.Groups[2].Value.ToLower();
                string lastPart = m.Groups[3].Value;

                articleText = articleText.Replace(m.Value, firstPart + parameterFirstChar + lastPart);
            }

            // remove any date field within  {{Multiple issues}} if no 'expert=subject' field using it
            string MICall = WikiRegexes.MultipleIssues.Match(articleText).Value;
            if (MICall.Length > 10 && (Tools.GetTemplateParameterValue(MICall, "expert").Length == 0 ||
                                       MonthYear.IsMatch(Tools.GetTemplateParameterValue(MICall, "expert"))))
                articleText = articleText.Replace(MICall, Tools.RemoveTemplateParameter(MICall, "date"));

            //articleText = SectionTemplates.Replace(articleText, SectionTemplateConversionsME);

            string newTags = "";

            // get the zeroth section (text upto first heading)
            string zerothSection = Tools.GetZerothSection(articleText);

            // get the rest of the article including first heading (may be null if entire article falls in zeroth section)
            string restOfArticle = articleText.Substring(zerothSection.Length);
            string ESDate = "";

            if (ExpertSubject.IsMatch(zerothSection))
            {
                ESDate = Tools.GetTemplateParameterValue(ExpertSubject.Match(zerothSection).Value, "date");
                zerothSection = Tools.RemoveTemplateParameter(zerothSection, "expert-subject", "date");
            }

            int tagsToAdd = WikiRegexes.MultipleIssuesTemplates.Matches(zerothSection).Count;

            // if currently no {{Multiple issues}} and less than the min number of cleanup templates, do nothing
            if (!WikiRegexes.MultipleIssues.IsMatch(zerothSection) && WikiRegexes.MultipleIssuesTemplates.Matches(zerothSection).Count < MinCleanupTagsToCombine)
            {
                // article issues with one issue -> single issue tag (e.g. {{multiple issues|cleanup=January 2008}} to {{cleanup|date=January 2008}} etc.)
                articleText = WikiRegexes.MultipleIssues.Replace(articleText, MultipleIssuesOldSingleTagME);

                return MultipleIssuesBLPUnreferenced(articleText);
            }

            // only add tags to multiple issues if new tags + existing >= MinCleanupTagsToCombine
            MICall = Tools.RenameTemplateParameter(WikiRegexes.MultipleIssues.Match(zerothSection).Value, "OR", "original research");

            if ((WikiRegexes.MultipleIssuesTemplateNameRegex.Matches(MICall).Count + tagsToAdd) < MinCleanupTagsToCombine || tagsToAdd == 0)
            {
                // article issues with one issue -> single issue tag (e.g. {{multiple issues|cleanup=January 2008}} to {{cleanup|date=January 2008}} etc.)
                articleText = WikiRegexes.MultipleIssues.Replace(articleText, (MultipleIssuesOldSingleTagME));

                return MultipleIssuesBLPUnreferenced(articleText);
            }

            foreach (Match m in WikiRegexes.MultipleIssuesTemplates.Matches(zerothSection))
            {
                // all fields except COI, OR, POV and ones with BLP should be lower case
                string singleTag = m.Groups[1].Value;
                string tagValue = m.Groups[2].Value;
                if (!WikiRegexes.CoiOrPovBlp.IsMatch(singleTag))
                    singleTag = singleTag.ToLower();

                string singleTagLower = singleTag.ToLower();

                // tag renaming
                if (singleTagLower.Equals("cleanup-rewrite"))
                    singleTag = "rewrite";
                else if (singleTagLower.Equals("cleanup-laundry"))
                    singleTag = "laundrylists";
                else if (singleTagLower.Equals("cleanup-jargon"))
                    singleTag = "jargon";
                else if (singleTagLower.Equals("primary sources"))
                    singleTag = "primarysources";
                else if (singleTagLower.Equals("news release"))
                    singleTag = "newsrelease";
                else if (singleTagLower.Equals("game guide"))
                    singleTag = "gameguide";
                else if (singleTagLower.Equals("travel guide"))
                    singleTag = "travelguide";
                else if (singleTagLower.Equals("very long"))
                    singleTag = "verylong";
                else if (singleTagLower.Equals("cleanup-reorganise"))
                    singleTag = "restructure";
                else if (singleTagLower.Equals("cleanup-reorganize"))
                    singleTag = "restructure";
                else if (singleTagLower.Equals("cleanup-spam"))
                    singleTag = "spam";
                else if (singleTagLower.Equals("criticism section"))
                    singleTag = "criticisms";
                else if (singleTagLower.Equals("pov-check"))
                    singleTag = "pov-check";
                else if (singleTagLower.Equals("expert-subject"))
                    singleTag = "expert";

                // copy edit|for=grammar --> grammar
                if (singleTag.Replace(" ", "").Equals("copyedit") && Tools.GetTemplateParameterValue(m.Value, "for").Equals("grammar"))
                {
                    singleTag = "grammar";
                    tagValue = Regex.Replace(tagValue, @"for\s*=\s*grammar\s*\|?", "");
                }

                // expert must have a parameter
                if (singleTag == "expert" && tagValue.Trim().Length == 0)
                    continue;

                // for tags with a parameter, that parameter must be the date
                if ((tagValue.Contains("=") && Regex.IsMatch(tagValue, @"(?i)date")) || tagValue.Length == 0 || singleTag == "expert")
                {
                    tagValue = Regex.Replace(tagValue, @"^[Dd]ate\s*=\s*", "= ");

                    // every tag except expert needs a date
                    if (!singleTag.Equals("expert") && tagValue.Length == 0)
                        tagValue = @"= {{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}";
                    else if (!tagValue.Contains(@"="))
                        tagValue = @"= " + tagValue;

                    // don't add duplicate tags
                    if (MICall.Length == 0 || Tools.GetTemplateParameterValue(MICall, singleTag).Length == 0)
                        newTags += @"|" + singleTag + @" " + tagValue;
                }
                else
                    continue;

                newTags = newTags.Trim();

                // remove the single template
                zerothSection = zerothSection.Replace(m.Value, "");
            }

            if (ESDate.Length > 0)
                newTags += ("|date = " + ESDate);

            // if article currently has {{Multiple issues}}, add tags to it
            string ai = WikiRegexes.MultipleIssues.Match(zerothSection).Value;
            if (ai.Length > 0)
                zerothSection = zerothSection.Replace(ai, ai.Substring(0, ai.Length - 2) + newTags + @"}}");

            else // add {{Multiple issues}} to top of article, metaDataSorter will arrange correctly later
                zerothSection = @"{{Multiple issues" + newTags + "}}\r\n" + zerothSection;

            articleText = zerothSection + restOfArticle;

            // Conversions() will add any missing dates and correct ...|wikify date=May 2008|...
            return MultipleIssuesBLPUnreferenced(articleText);
        }

        /// <summary>
        /// Converts old-style multiple issues with one issue to stand alone tag
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private string MultipleIssuesOldSingleTagME(Match m)
        {
			// Performance: nothing to do if no named parameters
			if(Tools.GetTemplateParameterValues(m.Value).Count == 0)
			    return m.Value;

            string newValue = Conversions(Tools.RemoveTemplateParameter(m.Value, "section"));

            if (Tools.GetTemplateArgumentCount(newValue) == 1 && !WikiRegexes.NestedTemplates.IsMatch(Tools.GetTemplateArgument(newValue, 1)))
            {
                string single = Tools.GetTemplateArgument(newValue, 1);

                if (single.Contains(@"="))
                    newValue = @"{{" + single.Substring(0, single.IndexOf("=")).Trim() + @"|date=" + single.Substring(single.IndexOf("=") + 1).Trim() + @"}}";
            }
            else return m.Value;

            return newValue;
        }

        private static readonly Regex MultipleIssuesDate = new Regex(@"(?<={{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues\s*(?:\|[^{}]*?)?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}[^{}]*?){0,4}\|[^{}\|]{3,}?)\b(?i)date(?<!.*out of.*)", RegexOptions.Compiled);

        /// <summary>
        /// In the {{Multiple issues}} template renames unref to BLPunref for living person bio articles
        /// </summary>
        /// <param name="articleText">The page text</param>
        /// <returns>The updated page text</returns>
        private string MultipleIssuesBLPUnreferenced(string articleText)
        {
            articleText = MultipleIssuesDate.Replace(articleText, "");

            if (WikiRegexes.MultipleIssues.IsMatch(articleText))
            {
                string aiat = WikiRegexes.MultipleIssues.Match(articleText).Value;

                // unref to BLPunref for living person bio articles
                if (Tools.GetTemplateParameterValue(aiat, "unreferenced").Length > 0 && articleText.Contains(CategoryLivingPeople))
                    articleText = articleText.Replace(aiat, Tools.RenameTemplateParameter(aiat, "unreferenced", "BLP unsourced"));
                else if (Tools.GetTemplateParameterValue(aiat, "unref").Length > 0 && articleText.Contains(CategoryLivingPeople))
                    articleText = articleText.Replace(aiat, Tools.RenameTemplateParameter(aiat, "unref", "BLP unsourced"));

                articleText = MetaDataSorter.MoveMaintenanceTags(articleText);
            }

            return articleText;
        }

        /// <summary>
        /// Performs cleanup on old-style multiple issues templates:
        /// convert title case parameters within {{Multiple issues}} to lower case
        /// remove any date field within  {{Multiple issues}} if no 'expert=subject' field using it
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private static string MultipleIssuesOldCleanup(string articleText)
        {
            // old-format cleanup: convert title case parameters within {{Multiple issues}} to lower case
            foreach (Match m in WikiRegexes.MultipleIssuesInTitleCase.Matches(articleText))
            {
                string firstPart = m.Groups[1].Value;
                string parameterFirstChar = m.Groups[2].Value.ToLower();
                string lastPart = m.Groups[3].Value;

                articleText = articleText.Replace(m.Value, firstPart + parameterFirstChar + lastPart);
            }

            // old-format cleanup: remove any date field within  {{Multiple issues}} if no 'expert=subject' field using it
            string MICall = WikiRegexes.MultipleIssues.Match(articleText).Value;
            if (MICall.Length > 10 && (Tools.GetTemplateParameterValue(MICall, "expert").Length == 0 ||
                                       MonthYear.IsMatch(Tools.GetTemplateParameterValue(MICall, "expert"))))
                articleText = articleText.Replace(MICall, Tools.RemoveTemplateParameter(MICall, "date"));

            return articleText;
        }

        /// <summary>
        /// Combines maintenance tags into {{multiple issues}} template, for en-wiki only
        /// Operates on a section by section basis through article text
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public string MultipleIssues(string articleText)
        {
            // en wiki only
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            // Performance: get all the templates, only apply mulitple issues functions if relevant templates found
            List<string> allTemplates = GetAllTemplates(articleText).Select(s => "{{" + s + "}}").ToList();
            bool hasMI = allTemplates.Any(s => WikiRegexes.MultipleIssues.IsMatch(s));

            if(hasMI)
            {
                articleText = MultipleIssuesOldCleanup(articleText);

                // Remove multiple issues with zero tags, fix excess newlines
                articleText = WikiRegexes.MultipleIssues.Replace(articleText, MultipleIssuesSingleTag);
            }

            if(hasMI || allTemplates.Any(s => (WikiRegexes.MultipleIssuesArticleMaintenanceTemplates.IsMatch(s) || 
                WikiRegexes.MultipleIssuesSectionMaintenanceTemplates.IsMatch(s))))
            {
                // get sections
                string[] sections = Tools.SplitToSections(articleText);
                StringBuilder newarticleText = new StringBuilder();

                foreach(string s in sections)
                {
                    if(!s.StartsWith("="))
                        newarticleText.Append(MIZerothSection(s, WikiRegexes.MultipleIssuesArticleMaintenanceTemplates));
                    else
                        newarticleText.Append(MILaterSection(s, WikiRegexes.MultipleIssuesSectionMaintenanceTemplates).TrimStart());
                }

                return newarticleText.ToString().TrimEnd();
            }

            return articleText;
        }

        private string MIZerothSection(string zerothsection, Regex Templates)
        {
            // look for maintenance templates
            // cannot support more than one multiple issues template per section
            MatchCollection MIMC = WikiRegexes.MultipleIssues.Matches(zerothsection);
            bool existingMultipleIssues = MIMC.Count > 0;
            if(MIMC.Count > 1)
                return zerothsection;

            string zerothsectionNoMI = Tools.ReplaceWithSpaces(zerothsection, MIMC);

            // count distinct templates
            int totalTemplates = Tools.DeduplicateList((from Match m in Templates.Matches(zerothsectionNoMI) select m.Value).ToList()).Count();

            // multiple issues with one issue -> single issue tag (old style or new style multiple issues)
            if(totalTemplates == 0 && existingMultipleIssues)
            {
                zerothsection = WikiRegexes.MultipleIssues.Replace(zerothsection, MultipleIssuesOldSingleTagME);
                return WikiRegexes.MultipleIssues.Replace(zerothsection, MultipleIssuesSingleTagME);
            }
            
            // if currently no {{Multiple issues}} and less than the min number of maintenance templates, do nothing
            if(!existingMultipleIssues && (totalTemplates < MinCleanupTagsToCombine))
                return zerothsection;

            // if currently has {{Multiple issues}}, add tags to it (new style only), otherwise insert multiple issues with tags.
            // multiple issues with some old style tags would have new style added
            
            if(!existingMultipleIssues)
                zerothsection = @"{{Multiple issues}}" + "\r\n" + zerothsection;
            
            // add each template to MI
            foreach(Match m in Templates.Matches(zerothsectionNoMI))
            {
                zerothsection = zerothsection.Replace(m.Value, "");
                string MI = WikiRegexes.MultipleIssues.Match(zerothsection).Value;
                bool newstyleMI = WikiRegexes.NestedTemplates.IsMatch(MI.Substring(2));
                zerothsection = zerothsection.Replace(MI, Regex.Replace(MI, @"\s*}}$", (newstyleMI ? "" : "|") + "\r\n" + m.Value + "\r\n}}"));
            }
            
            return zerothsection;
        }

        private string MILaterSection(string section, Regex Templates)
        {
            string sectionOriginal = section;
            MatchCollection MIMC = WikiRegexes.MultipleIssues.Matches(section);
            bool existingMultipleIssues = MIMC.Count > 0;
            
            // look for maintenance templates
            // cannot support more than one multiple issues template per section
            if(MIMC.Count > 1)
                return sectionOriginal;
            
            if(existingMultipleIssues)
                section = Tools.ReplaceWithSpaces(section, MIMC);
            
            int templatePortion = 0;

            if(WikiRegexes.NestedTemplates.IsMatch(section))
                templatePortion = Tools.HowMuchStartsWith(section, Templates, true);

            // multiple issues with one issue -> single issue tag (old style or new style multiple issues)
            if(templatePortion == 0)
            {
                if(existingMultipleIssues)
                {
                    sectionOriginal = WikiRegexes.MultipleIssues.Replace(sectionOriginal, MultipleIssuesOldSingleTagME);
                    return WikiRegexes.MultipleIssues.Replace(sectionOriginal, MultipleIssuesSingleTagME);
                }
                return sectionOriginal;
            }

            string heading = WikiRegexes.Headings.Match(section).Value.Trim(),
            sectionPortion = section.Substring(0, templatePortion),
            sectionPortionOriginal = sectionOriginal.Substring(0, templatePortion),
            sectionRest = sectionOriginal.Substring(templatePortion);
            
            int totalTemplates = Templates.Matches(sectionPortion).Count;
            
            // if currently no {{Multiple issues}} and less than the min number of maintenance templates, do nothing
            if(!existingMultipleIssues && (totalTemplates < MinCleanupTagsToCombine))
                return sectionOriginal;

            // if currently has {{Multiple issues}}, add tags to it (new style only), otherwise insert multiple issues with tags. multiple issues with some old style tags would have new style added
            string newsection;
            
            if(existingMultipleIssues) // add each template to MI
            {
                newsection = WikiRegexes.MultipleIssues.Match(sectionPortionOriginal).Value;

                if(newsection.Length > 0)
                {
                    bool newstyleMI = WikiRegexes.NestedTemplates.IsMatch(Tools.GetTemplateArgument(Tools.RemoveTemplateParameter(newsection, "section"), 1));
                    
                    foreach(Match m in Templates.Matches(sectionPortion))
                    {
                        if(!newsection.Contains(m.Value))
                            newsection = newsection.Replace(newsection, Regex.Replace(newsection, @"\s*}}$", (newstyleMI ? "" : "|") + "\r\n" + m.Value + "\r\n}}"));
                    }
                }
                else // MI template later in section, other text in between, cannot do anything with this
                    return sectionOriginal;
            }
            else // create new MI and add each template
            {
                newsection = "{{Multiple issues|section=yes|\r\n";
                
                foreach(Match m in Templates.Matches(sectionPortion))
                    newsection += (m.Value + "\r\n");
                
                newsection += "}}";
            }
            
            return heading + "\r\n" + newsection + "\r\n" + sectionRest;
        }
        
        /// <summary>
        /// Converts new-style multiple issues with one issue to stand alone tag
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private string MultipleIssuesSingleTagME(Match m)
        {
            string newValue = Tools.RemoveTemplateParameter(m.Value, "section");

            if (Tools.GetTemplateArgumentCount(newValue) == 1 && WikiRegexes.NestedTemplates.Matches(Tools.GetTemplateArgument(newValue, 1)).Count == 1)
                return Tools.GetTemplateArgument(newValue, 1);
            
            return m.Value;
        }

        /// <summary>
        /// Removes multiple issues with zero tags
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private string MultipleIssuesSingleTag(Match m)
        {
            string newValue = Tools.RemoveTemplateParameter(Tools.RemoveExcessTemplatePipes(m.Value), "section");

            if (Tools.GetTemplateArgumentCount(newValue) == 0)
                return "";

            // clean excess newlines
            return Regex.Replace(m.Value, "(\r\n)+", "\r\n");
        }

        /// <summary>
        /// Merges multiple {{portal}} templates into a single one, removing any duplicates. En-wiki only.
        /// Restricted to {{portal}} calls with one argument
        /// Article must have existing {{portal}} and/or a 'see also' section
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>The updated article text</returns>
        public static string MergePortals(string articleText)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            string originalArticleText = articleText;
            List<string> Portals = new List<string>();
            int firstPortal = WikiRegexes.PortalTemplate.Match(articleText).Index;

            foreach (Match m in WikiRegexes.PortalTemplate.Matches(articleText))
            {
                string thePortalCall = m.Value, thePortalName = Tools.GetTemplateArgument(m.Value, 1);

                if (!Portals.Contains(thePortalName) && Tools.GetTemplateArgumentCount(thePortalCall) == 1)
                {
                    Portals.Add(thePortalName);
                    articleText = Regex.Replace(articleText, Regex.Escape(thePortalCall) + @"\s*(?:\r\n)?", "");
                }
            }

            if (Portals.Count == 0)
                return articleText;

            // generate portal string
            string PortalsToAdd = "";
            foreach (string portal in Portals)
                PortalsToAdd += ("|" + portal.Trim());

            // merge in new portal if multiple portals
            if (Portals.Count < 2)
                return originalArticleText;

            // first merge to see also section
            if (WikiRegexes.SeeAlso.Matches(articleText).Count == 1)
                return WikiRegexes.SeeAlso.Replace(articleText, "$0" + Tools.Newline(@"{{Portal" + PortalsToAdd + @"}}"));

            // otherwise merge to original location if all portals in same section
            if (Summary.ModifiedSection(originalArticleText, articleText).Length > 0)
                return articleText.Insert(firstPortal, @"{{Portal" + PortalsToAdd + @"}}" + "\r\n");

            return originalArticleText;
        }

        /// <summary>
        /// Performs some cleanup operations on dablinks
        /// Merges some for & about dablinks
        /// Merges multiple distinguish into one
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>The updated article text</returns>
        public static string Dablinks(string articleText)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            string zerothSection = Tools.GetZerothSection(articleText);
            string restOfArticle = articleText.Substring(zerothSection.Length);
            articleText = zerothSection;

            // conversions

            // otheruses4 rename - Wikipedia only
            if (Variables.IsWikipediaEN)
                articleText = Tools.RenameTemplate(articleText, "otheruses4", "about");

            // "{{about|about x..." --> "{{about|x..."
            foreach (Match m in Tools.NestedTemplateRegex("about").Matches(articleText))
            {
                if (m.Groups[3].Value.TrimStart("| ".ToCharArray()).StartsWith("about", StringComparison.OrdinalIgnoreCase))
                    articleText = articleText.Replace(m.Value, m.Groups[1].Value + m.Groups[2].Value + Regex.Replace(m.Groups[3].Value, @"^\|\s*[Aa]bout\s*", "|"));
            }

            // merging

            // multiple same about into one
            string oldArticleText = "";
            while (oldArticleText != articleText)
            {
                oldArticleText = articleText;
                bool doneAboutMerge = false;
                foreach (Match m in Tools.NestedTemplateRegex("about").Matches(articleText))
                {
                    string firstarg = Tools.GetTemplateArgument(m.Value, 1);

                    foreach (Match m2 in Tools.NestedTemplateRegex("about").Matches(articleText))
                    {
                        if (m2.Value == m.Value)
                            continue;

                        // match when reason is the same, not matching on self
                        if (Tools.GetTemplateArgument(m2.Value, 1).Equals(firstarg))
                        {
                            // argument 2 length > 0
                            if (Tools.GetTemplateArgument(m.Value, 2).Length > 0 && Tools.GetTemplateArgument(m2.Value, 2).Length > 0)
                            {
                                articleText = articleText.Replace(m.Value, m.Value.TrimEnd('}') + @"|" + Tools.GetTemplateArgument(m2.Value, 2) + @"|" + Tools.GetTemplateArgument(m2.Value, 3) + @"}}");
                                doneAboutMerge = true;
                            }

                            // argument 2 is null
                            if (Tools.GetTemplateArgument(m.Value, 2).Length == 0 && Tools.GetTemplateArgument(m2.Value, 2).Length == 0)
                            {
                                articleText = articleText.Replace(m.Value, m.Value.TrimEnd('}') + @"|and|" + Tools.GetTemplateArgument(m2.Value, 3) + @"}}");
                                doneAboutMerge = true;
                            }
                        }
                        // match when reason of one is null, the other not
                        else if (Tools.GetTemplateArgument(m2.Value, 1).Length == 0)
                        {
                            // argument 2 length > 0
                            if (Tools.GetTemplateArgument(m.Value, 2).Length > 0 && Tools.GetTemplateArgument(m2.Value, 2).Length > 0)
                            {
                                articleText = articleText.Replace(m.Value, m.Value.TrimEnd('}') + @"|" + Tools.GetTemplateArgument(m2.Value, 2) + @"|" + Tools.GetTemplateArgument(m2.Value, 3) + @"}}");
                                doneAboutMerge = true;
                            }
                        }

                        if (doneAboutMerge)
                        {
                            articleText = articleText.Replace(m2.Value, "");
                            break;
                        }
                    }
                    if (doneAboutMerge)
                        break;
                }
            }

            // multiple for into about: rename a 2-argument for into an about with no reason value
            if (Tools.NestedTemplateRegex("for").Matches(articleText).Count > 1 && Tools.NestedTemplateRegex("about").Matches(articleText).Count == 0)
            {
                foreach (Match m in Tools.NestedTemplateRegex("for").Matches(articleText))
                {
                    if (Tools.GetTemplateArgument(m.Value, 3).Length == 0)
                    {
                        articleText = articleText.Replace(m.Value, Tools.RenameTemplate(m.Value, "about|"));
                        break;
                    }
                }
            }

            // for into existing about, when about has >=2 arguments
            if (Tools.NestedTemplateRegex("about").Matches(articleText).Count == 1 &&
                Tools.GetTemplateArgument(Tools.NestedTemplateRegex("about").Match(articleText).Value, 2).Length > 0)
            {
                foreach (Match m in Tools.NestedTemplateRegex("for").Matches(articleText))
                {
                    string about = Tools.NestedTemplateRegex("about").Match(articleText).Value;
                    
                    // about supports up to 9 arguments
                    if (Tools.GetTemplateArgument(about, 9).Length > 0)
                        continue;
                    
                    string extra = "";

                    // where about has 2 arguments need extra pipe
                    if (Tools.GetTemplateArgument(Tools.NestedTemplateRegex("about").Match(articleText).Value, 3).Length == 0
                        && Tools.GetTemplateArgument(Tools.NestedTemplateRegex("about").Match(articleText).Value, 4).Length == 0)
                        extra = @"|";

                    // append {{for}} value to the {{about}}
                    if (Tools.GetTemplateArgument(m.Value, 3).Length == 0)
                        articleText = articleText.Replace(about, about.TrimEnd('}') + extra + m.Groups[3].Value);
                    else if  (Tools.GetTemplateArgument(m.Value, 4).Length == 0) // where for has 3 arguments need extra and
                        articleText = articleText.Replace(about, about.TrimEnd('}') + extra + m.Groups[3].Value.Insert(m.Groups[3].Value.LastIndexOf('|') + 1, "and|"));
    
                    // if there are 4 arguments do nothing
                    // remove the old {{for}}
                    if  (Tools.GetTemplateArgument(m.Value, 4).Length == 0)
	                    articleText = articleText.Replace(m.Value, "");
                }

                // if for with blank first argument copied over then now need to put "other uses" as the argment
                articleText = Tools.NestedTemplateRegex("about").Replace(articleText, m2 => {
                                                                             string res = m2.Value;
                                                                             if(Tools.GetTemplateArgument(res, 7).Length > 0 && Tools.GetTemplateArgument(res, 6).Length == 0)
                                                                             {
                                                                                 res = res.Insert(Tools.GetTemplateArgumentIndex(res, 6), "other uses");
                                                                             }
                                                                             return res;
                                                                         });
            }

            // non-mainspace links need escaping in {{about}}
            foreach (Match m in Tools.NestedTemplateRegex("about").Matches(articleText))
            {
                string aboutcall = m.Value;
                for (int a = 1; a <= Tools.GetTemplateArgumentCount(m.Value); a++)
                {
                    string arg = Tools.GetTemplateArgument(aboutcall, a);
                    if (arg.Length > 0 && Namespace.Determine(arg) != Namespace.Mainspace)
                        aboutcall = aboutcall.Replace(arg, @":" + arg);
                }

                if (!m.Value.Equals(aboutcall))
                    articleText = articleText.Replace(m.Value, aboutcall);
            }

            // multiple {{distinguish}} into one
            oldArticleText = "";
            while (oldArticleText != articleText)
            {
                oldArticleText = articleText;
                bool doneDistinguishMerge = false;
                foreach (Match m in Tools.NestedTemplateRegex("distinguish").Matches(articleText))
                {
                    foreach (Match m2 in Tools.NestedTemplateRegex("distinguish").Matches(articleText))
                    {
                        if (m2.Value.Equals(m.Value))
                            continue;

                        articleText = articleText.Replace(m.Value, m.Value.TrimEnd('}') + m2.Groups[3].Value);

                        doneDistinguishMerge = true;
                        articleText = articleText.Replace(m2.Value, "");
                        break;
                    }

                    if (doneDistinguishMerge)
                        break;
                }
            }

            return (articleText + restOfArticle);
        }

        private static readonly List<string> SectionMergedTemplates = new List<string>(new[] { "see also", "see also2", "main" });
        private static readonly Regex SectionMergedTemplatesR = Tools.NestedTemplateRegex(SectionMergedTemplates);

        /// <summary>
        /// Merges multiple instances of the same template in the same section
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>The updated article text</returns>
        public static string MergeTemplatesBySection(string articleText)
        {
            string[] articleTextInSections = Tools.SplitToSections(articleText);
            StringBuilder newArticleText = new StringBuilder();

            foreach(string s in articleTextInSections)
            {
                string sectionText = s;
                foreach (string t in SectionMergedTemplates)
                {
                    if(SectionMergedTemplatesR.Matches(sectionText).Count < 2)
                        break;

                    sectionText = MergeTemplates(sectionText, t);
                }
                newArticleText.Append(sectionText);
            }

            return newArticleText.ToString().TrimEnd();
        }

        /// <summary>
        /// Merges all instances of the given template in the given section of the article, only when templates at top of section
        /// </summary>
        /// <param name="sectionText">The article section text</param>
        /// <param name="templateName">The template to merge</param>
        /// <returns>The updated article section text</returns>
        private static string MergeTemplates(string sectionText, string templateName)
        {
            if (!Variables.LangCode.Equals("en"))
                return sectionText;

            string sectionTextOriginal = sectionText;
            Regex TemplateToMerge = Tools.NestedTemplateRegex(templateName);
            string mergedTemplates = "";

            // unless it's zeroth section must remove heading to have templates at start of section
            string heading = "";
            Match h = WikiRegexes.Headings.Match(sectionText);

            if(h.Success && h.Index == 0)
            {
                heading = h.Value.TrimEnd();
                sectionText = sectionText.Substring(h.Length).TrimStart();
            }

            int merged = 0;

            while(TemplateToMerge.IsMatch(sectionText))
            {
                Match m = TemplateToMerge.Match(sectionText);
                
                // only take templates at very start of section (after heading)
                if(m.Index > 0)
                    break;
                
                // if first template just append, if subsequent then merge in value
                if(mergedTemplates.Length == 0)
                    mergedTemplates = m.Value;
                else
                {
                    mergedTemplates = mergedTemplates.Replace(@"}}", m.Groups[3].Value);
                    merged++;
                }
                
                // reove template from section text
                sectionText = sectionText.Substring(m.Length).TrimStart();
            }

            // recompose section: only if a merge has happened
            if(merged > 0)
                return ((heading.Length > 0 ? heading + "\r\n" : "") + mergedTemplates + "\r\n" + sectionText);

            return sectionTextOriginal;
        }

        // fixes extra comma or space in American format dates
        private static readonly Regex IncorrectCommaAmericanDates = new Regex(WikiRegexes.Months + @"[ ,]*([1-3]?\d(?:–[1-3]?\d)?)[ ,]+([12]\d{3})\b");

        // fix incorrect comma between month and year in Internaltional-format dates
        private static readonly Regex IncorrectCommaInternationalDates = new Regex(@"\b((?:[1-3]?\d) +" + WikiRegexes.MonthsNoGroup + @") *, *(1\d{3}|20\d{2})\b", RegexOptions.Compiled);

        // date ranges use an en-dash per [[WP:MOSDATE]]
        private static readonly Regex SameMonthInternationalDateRange = new Regex(@"\b([1-3]?\d) *- *([1-3]?\d +" + WikiRegexes.MonthsNoGroup + @")\b", RegexOptions.Compiled);
        private static readonly Regex SameMonthAmericanDateRange = new Regex(@"(" + WikiRegexes.MonthsNoGroup + @" *)([0-3]?\d) *- *([0-3]?\d)\b(?!\-)", RegexOptions.Compiled);

        // 13 July -28 July 2009 -> 13–28 July 2009
        // July 13 - July 28 2009 -> July 13–28, 2009
        private static readonly Regex LongFormatInternationalDateRange = new Regex(@"\b([1-3]?\d) +" + WikiRegexes.Months + @" *(?:-|–|&nbsp;) *([1-3]?\d) +\2,? *([12]\d{3})\b", RegexOptions.Compiled);
        private static readonly Regex LongFormatAmericanDateRange = new Regex(WikiRegexes.Months + @" +([1-3]?\d) +" + @" *(?:-|–|&nbsp;) *\1 +([1-3]?\d) *,? *([12]\d{3})\b", RegexOptions.Compiled);
        private static readonly Regex EnMonthRange = new Regex(@"\b" + WikiRegexes.Months + @"-" + WikiRegexes.Months + @"\b", RegexOptions.Compiled);

        private static readonly Regex FullYearRange = new Regex(@"((?:[\(,=;\|]|\b(?:from|between|and|reigned|f?or|ca?\.?\]*|circa)) *)([12]\d{3}) *- *([12]\d{3})(?= *(?:\)|[,;\|]|and\b|\s*$))", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SpacedFullYearRange = new Regex(@"(?<!\b(?:ca?\.?\]*|circa) *)([12]\d{3})(?: +– *| *– +)([12]\d{3})", RegexOptions.Compiled);
        private static readonly Regex YearRangeShortenedCentury = new Regex(@"((?:[\(,=;]|\b(?:from|between|and|reigned)) *)([12]\d{3}) *- *(\d{2})(?= *(?:\)|[,;]|and\b|\s*$))", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex DateRangeToPresent = new Regex(@"\b(" + WikiRegexes.MonthsNoGroup + @"|[0-3]?\d,?) +" + @"([12]\d{3}) *- *([Pp]resent\b)", RegexOptions.Compiled);
        // matches 11 May 2010–2012 (unspaced endash in month–year range, should be spaced)
        private static readonly Regex DateRangeToYear = new Regex(@"\b(" + WikiRegexes.MonthsNoGroup + @"|\b" + WikiRegexes.MonthsNoGroup + @"(?:&nbsp;|\s+)[0-3]?\d,?) +" + @"([12]\d{3})[-–]([12]\d{3})\b", RegexOptions.Compiled);
        private static readonly Regex YearRangeToPresent = new Regex(@"\b([12]\d{3}) *- *([Pp]resent\b)", RegexOptions.Compiled);

        private static readonly Regex YearDash = new Regex(@"[12]\d{3}[–-]");
        private static readonly Regex InternationalDateFullUnspacedRange = new Regex(WikiRegexes.InternationalDates + @"[–-]" + WikiRegexes.InternationalDates);
        private static readonly Regex AmericanDateFullUnspacedRange = new Regex(WikiRegexes.AmericanDates + @"[–-]" + WikiRegexes.AmericanDates);

        /// <summary>
        /// Fix date and decade formatting errors: commas in American/international dates, full date ranges, month ranges
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public string FixDatesA(string articleText)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            /* performance check: on most articles no date changes, on long articles HideMore is slow, so if no changes to raw text
             * don't need to perform actual check on HideMore text, and this is faster overall
             * Secondly: faster to apply regexes to each date found than to apply regexes to whole article text
             */
            bool changes = false;
            foreach(Match m in MonthsRegexNoSecondBreak.Matches(articleText))
            {
                // take up to 25 characters before match, unless match within first 25 characters of article
                string before = articleText.Substring(m.Index-Math.Min(25, m.Index), Math.Min(25, m.Index)+m.Length);
                
                string after = FixDatesAInternal(before);

                if(!after.Equals(before))
                {
                    changes = true;
                    break;
                }
            }

            if(!changes)
                return articleText;

            articleText = HideTextImages(articleText);

            articleText = FixDatesAInternal(articleText);

            return AddBackTextImages(articleText);
        }

        private string FixDatesAInternal(string textPortion)
        {
            textPortion = IncorrectCommaInternationalDates.Replace(textPortion, @"$1 $2");

            textPortion = SameMonthInternationalDateRange.Replace(textPortion, @"$1–$2");

            textPortion = SameMonthAmericanDateRange.Replace(textPortion, SameMonthAmericanDateRangeME);

            textPortion = LongFormatInternationalDateRange.Replace(textPortion, @"$1–$3 $2 $4");
            textPortion = LongFormatAmericanDateRange.Replace(textPortion, @"$1 $2–$3, $4");

            // run this after the date range fixes
            textPortion = IncorrectCommaAmericanDates.Replace(textPortion, @"$1 $2, $3");

            // month range
            return EnMonthRange.Replace(textPortion, @"$1–$2");
        }
        
        private static readonly Regex YearRange = new Regex(@"\b[12][0-9]{3}.{0,25}");

        /// <summary>
        /// Fix date and decade formatting errors: date/year ranges to present, full year ranges, performs floruit term wikilinking
        /// </summary>
        /// <param name="articleText"></param>
        /// <param name="CircaLink"></param>
        /// <param name="Floruit"></param>
        /// <returns></returns>
        public string FixDatesB(string articleText, bool CircaLink, bool Floruit)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            for (; ; )
            {
                /* performance check: faster to apply regexes to each year/date found
                 * than to apply regexes to whole article text
                 */
                bool reparse = false;
                foreach(Match m in YearRange.Matches(articleText))
                {
                    // take up to 25 characters before match, unless match within first 25 characters of article
                    string before = articleText.Substring(m.Index-Math.Min(25, m.Index), Math.Min(25, m.Index)+m.Length);
                    
                    string after = FixDatesBInternal(before, CircaLink);

                    if(!after.Equals(before))
                    {
                        reparse = true;
                        articleText = articleText.Replace(before, after);
                        break;
                    }
                }

                if (!reparse)
                    break;
            }

            // replace first occurrence of unlinked floruit with linked version, zeroth section only
            if(Floruit)
                articleText = WikiRegexes.UnlinkedFloruit.Replace(articleText, @"([[floruit|fl.]] $1", 1);

            return articleText;
        }

        private string FixDatesBInternal(string textPortion, bool CircaLink)
        {
            textPortion = DateRangeToPresent.Replace(textPortion, @"$1 $2 – $3");
            textPortion = YearRangeToPresent.Replace(textPortion, @"$1–$2");

            // 1965–1968 fixes: only appy year range fix if two years are in order
            if (!CircaLink)
            {
                textPortion = FullYearRange.Replace(textPortion, FullYearRangeME);
                textPortion = SpacedFullYearRange.Replace(textPortion, SpacedFullYearRangeME);
            }

            // 1965–68 fixes
            textPortion = YearRangeShortenedCentury.Replace(textPortion, YearRangeShortenedCenturyME);

            // endash spacing: date–year --> date – year
            textPortion = DateRangeToYear.Replace(textPortion, @"$1 $2 – $3");

            // full date range spacing: date–date --> date – date
            if(YearDash.IsMatch(textPortion))
            {
                textPortion = InternationalDateFullUnspacedRange.Replace(textPortion, m => m.Value.Replace("-", "–").Replace("–", " – "));
                textPortion = AmericanDateFullUnspacedRange.Replace(textPortion, m => m.Value.Replace("-", "–").Replace("–", " – "));
            }

            return textPortion;
        }
        
        /// <summary>
        /// Remove 2 or more &lt;br /&gt;'s
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public string FixBrParagraphs(string articleText)
        {
            articleText = SyntaxRemoveBr.Replace(articleText, "\r\n\r\n");
            articleText = SyntaxRemoveParagraphs.Replace(articleText, "\r\n\r\n");
            return SyntaxRegexListRowBrTagStart.Replace(articleText, "$1");
        }

        private static string FullYearRangeME(Match m)
        {
            int year1 = Convert.ToInt32(m.Groups[2].Value), year2 = Convert.ToInt32(m.Groups[3].Value);

            if (year2 > year1 && year2 - year1 <= 300)
                return m.Groups[1].Value + m.Groups[2].Value + (m.Groups[1].Value.ToLower().Contains("c") ? @" – " : @"–") + m.Groups[3].Value;

            return m.Value;
        }

        private static string SpacedFullYearRangeME(Match m)
        {
            int year1 = Convert.ToInt32(m.Groups[1].Value), year2 = Convert.ToInt32(m.Groups[2].Value);

            if (year2 > year1 && year2 - year1 <= 300)
                return m.Groups[1].Value + @"–" + m.Groups[2].Value;

            return m.Value;
        }

        private static string YearRangeShortenedCenturyME(Match m)
        {
            int year1 = Convert.ToInt32(m.Groups[2].Value); // 1965
            int year2 = Convert.ToInt32(m.Groups[2].Value.Substring(0, 2) + m.Groups[3].Value); // 68 -> 19 || 68 -> 1968

            if (year2 > year1 && year2 - year1 <= 99)
                return m.Groups[1].Value + m.Groups[2].Value + @"–" + m.Groups[3].Value;

            return m.Value;
        }

        private static string SameMonthAmericanDateRangeME(Match m)
        {
            int day1 = Convert.ToInt32(m.Groups[2].Value), day2 = Convert.ToInt32(m.Groups[3].Value);

            if (day2 > day1)
                return Regex.Replace(m.Value, @" *- *", @"–");

            return m.Value;
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

        private const string InlineCitationCleanupTemplatesRp = @"(?:Author incomplete|Author missing|Citation broken|Citation not found|Clarify|Date missing|Episode|ISBN missing|Page needed|Publisher missing|Season needed|Time needed|Title incomplete|Title missing|Volume needed|Year missing|rp)";
        private const string OutofOrderRefs = @"(<ref\s+name\s*=\s*(?:""|')?([^<>""=]+?)(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>)(\s*{{\s*" + InlineCitationCleanupTemplatesRp + @"\s*\|[^{}]+}})?(\s*)(<ref\s+name\s*=\s*(?:""|')?([^<>""=]+?)(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>)(\s*{{\s*" + InlineCitationCleanupTemplatesRp + @"\s*\|[^{}]+}})?";
        private static readonly Regex OutofOrderRefs1 = new Regex(@"(<ref>[^<>]+</ref>)(\s*)(<ref\s+name\s*=\s*(?:""|')?([^<>""=]+?)(?:""|')?\s*(?:\/\s*|>[^<>]+</ref)>)(\s*{{\s*" + InlineCitationCleanupTemplatesRp + @"\s*\|[^{}]+}})?", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex OutofOrderRefs2 = new Regex(OutofOrderRefs, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        // regex below ensures a forced match on second and third of consecutive references
        private static readonly Regex OutofOrderRefs3 = new Regex(@"(?<=\s*(?:\/\s*|>[^<>]+</ref)>\s*(?:{{\s*" + InlineCitationCleanupTemplatesRp + @"\s*\|[^{}]+}})?)" + OutofOrderRefs, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        private static readonly Regex RefsTag = new Regex(@"<\s*references");

        /// <summary>
        /// Reorders references so that they appear in numerical order, allows for use of {{rp}}, doesn't modify grouped references [[WP:REFGROUP]]
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string ReorderReferences(string articleText)
        {
            // do not reorder stuff in the {{refs}} or <references>...</references> section
            int referencestags = RefsTemplateIndex(articleText);

            // build dictionary of all named references and index of first occurrence
            // No need to go collect refs in named refs section: must be used in article
            Dictionary<string, int> NamedRefsIndexes = new Dictionary<string, int>();
            
            foreach (Match n in WikiRegexes.NamedReferencesIncludingCondensed.Matches(articleText))
            {
                if(n.Index > referencestags)
                    break;

                if(!NamedRefsIndexes.ContainsKey(n.Groups[2].Value))
                    NamedRefsIndexes.Add(n.Groups[2].Value, n.Index);
            }

            for (int i = 0; i < 9; i++) // allows for up to 9 consecutive references
            {
                string articleTextBefore = articleText;

                foreach (Match m in OutofOrderRefs1.Matches(articleText))
                {
                    string ref1 = m.Groups[1].Value;
                    int ref1Index;
                    NamedRefsIndexes.TryGetValue(m.Groups[4].Value, out ref1Index);
                    int ref2Index = m.Index;

                    if (ref1Index < ref2Index && ref1Index > 0 && m.Groups[3].Index < referencestags)
                    {
                        string whitespace = m.Groups[2].Value;
                        string rptemplate = m.Groups[5].Value;
                        string ref2 = m.Groups[3].Value;
                        articleText = articleText.Replace(ref1 + whitespace + ref2 + rptemplate, ref2 + rptemplate + whitespace + ref1);
                    }
                }

                articleText = ReorderRefs(articleText, OutofOrderRefs2, referencestags, NamedRefsIndexes);
                articleText = ReorderRefs(articleText, OutofOrderRefs3, referencestags, NamedRefsIndexes);

                if (articleTextBefore.Equals(articleText))
                    break;
            }

            return articleText;
        }

        /// <summary>
        /// Returns the index of the {{reflist}} or {{refs}} or &lt;references&gt; tags, or the articleText length if no tags found
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private static int RefsTemplateIndex(string articleText)
        {
            Match r1 = WikiRegexes.ReferenceList.Match(articleText);
            if(r1.Success)
                return r1.Index;

            Match r2 = RefsTag.Match(articleText);

            if(r2.Success)
                return r2.Index;

            return articleText.Length;
        }

        private const string RefsPunctuation = @"([,\.;:\?\!])";
        private const string NoPunctuation = @"([^,\.:;\?\!])";
        private static readonly Regex RefsBeforePunctuationR = new Regex(@" *" + WikiRegexes.Refs + @" *" + RefsPunctuation + @"([^,\.:;\?\!]|$)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex RefsBeforePunctuationQuick = new Regex(@"(?<=(?:/|ref) *)> *" + RefsPunctuation);
        private static readonly Regex RefsAfterDupePunctuation = new Regex(NoPunctuation + RefsPunctuation.Replace(@"\!", "") + @"\2 *" + WikiRegexes.Refs, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex RefsAfterDupePunctuationQuick = new Regex(@"(?<![,\.:;\?\!])" + RefsPunctuation.Replace(@"\!", "") + @"\1 *<\s*ref", RegexOptions.IgnoreCase);
        private static readonly Regex Footnote = Tools.NestedTemplateRegex(new[] {"Efn", "Efn-ua", "Efn-lr", "Sfn", "Shortened footnote", "Shortened footnote template", "Sfnb", "Sfnp", "Sfnm", "SfnRef", "Rp"});
        private static readonly Regex PunctuationAfterFootnote = new Regex(@"(?<sfn>" + Footnote + @")(?<punc>[,\.;:\?\!])");
        private static readonly Regex FootnoteAfterDupePunctuation = new Regex(NoPunctuation + RefsPunctuation + @"\2 *(?<sfn>" + Footnote + @")");
        private static readonly Regex FootnoteAfterDupePunctuationQuick = new Regex(RefsPunctuation + @"\1 *" + Footnote);

        /// <summary>
        /// Puts &lt;ref&gt; and {{sfn}} references after punctuation (comma, full stop) per WP:REFPUNC
        /// Applies to en/el wiki only
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>The updated article text</returns>
        public static string RefsAfterPunctuation(string articleText)
        {
            if (!Variables.LangCode.Equals("en") && !Variables.LangCode.Equals("simple"))
                return articleText;

            string articleTextOriginal = articleText;
            bool HasFootnote = Footnote.IsMatch(articleText);

            articleText = RefsBeforePunctuation(articleText);

            if(HasFootnote)
            {
                while(PunctuationAfterFootnote.IsMatch(articleText))
                {
                    articleText = PunctuationAfterFootnote.Replace(articleText, "${punc}${sfn}");
                    articleText = RefsBeforePunctuation(articleText);
                }
            }

            // clean duplicate punctuation before ref, not for !!, could be part of wiki table
            if(RefsAfterDupePunctuationQuick.IsMatch(articleText))
                articleText = RefsAfterDupePunctuation.Replace(articleText, "$1$2$3");
            if(HasFootnote && FootnoteAfterDupePunctuationQuick.IsMatch(articleText))
                articleText = FootnoteAfterDupePunctuation.Replace(articleText, "$1$2${sfn}");

            // if there have been changes need to call FixReferenceTags in case punctation moved didn't have witespace after it  
            if(!articleTextOriginal.Equals(articleText))
                articleText = FixReferenceTags(articleText);

            return articleText;
        }

        private static string RefsBeforePunctuation(string articleText)
        {
            // 'quick' regexes are used for runtime performance saving
            if(RefsBeforePunctuationQuick.IsMatch(articleText))
            {
                while(RefsBeforePunctuationR.IsMatch(articleText))
                {
                    articleText = RefsBeforePunctuationR.Replace(articleText, "$2$1$3");
                    articleText = RefsAfterDupePunctuation.Replace(articleText, "$1$2$3");
                }
            }
            return articleText;
        }

        /// <summary>
        /// reorders references within the article text based on the input regular expression providing matches for references that are out of numerical order
        /// </summary>
        /// <param name="articleText">the wiki text of the article</param>
        /// <param name="outofOrderRegex">a regular expression representing two references that are out of numerical order</param>
        /// <param name="referencestagindex"></param>
        /// <param name="NamedRefsIndexes"></param>
        /// <returns>the modified article text</returns>
        private static string ReorderRefs(string articleText, Regex outofOrderRegex, int referencestagindex, Dictionary<string, int> NamedRefsIndexes)
        {
            foreach (Match m in outofOrderRegex.Matches(articleText))
            {
                if(m.Groups[5].Index < referencestagindex)
                {
                    int ref1Index, ref2Index; 
                    NamedRefsIndexes.TryGetValue(m.Groups[2].Value, out ref1Index);
                    NamedRefsIndexes.TryGetValue(m.Groups[6].Value, out ref2Index);

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
                else break; // all further matches beyond refs template
            }

            return articleText;
        }

        private static readonly Regex LongNamedReferences = new Regex(@"(<\s*ref\s+name\s*=\s*(?:""|')?([^<>=\r\n]+?)(?:""|')?\s*>\s*([^<>]{30,}?)\s*<\s*/\s*ref>)", RegexOptions.Compiled);

        // Covered by: DuplicateNamedReferencesTests()
        /// <summary>
        /// Where an unnamed reference is a duplicate of another named reference, set the unnamed one to use the named ref
        /// Checks for instances of named references with same ref name having different values, does not modify article text in this case
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string DuplicateNamedReferences(string articleText)
        {
            string articleTextOriginal = articleText;
            Dictionary<string, string> NamedRefs = new Dictionary<string, string>();

            for (; ; )
            {
                bool reparse = false;
                NamedRefs.Clear();
                int reflistIndex = RefsTemplateIndex(articleText);

                foreach (Match m in WikiRegexes.NamedReferences.Matches(articleText))
                {
                    string refName = m.Groups[2].Value, namedRefValue = m.Groups[3].Value;

                    if (!NamedRefs.ContainsKey(namedRefValue))
                    {
                        // check for instances of named references with same ref name having different values: requires manual correction of article
                        if(NamedRefs.ContainsValue(refName))
                            return articleTextOriginal;
                        
                        NamedRefs.Add(namedRefValue, refName);
                    }
                    else
                    {
                        // we've already seen this reference, can condense later ones
                        string name2;
                        NamedRefs.TryGetValue(namedRefValue, out name2);

                        if (!string.IsNullOrEmpty(name2) && name2.Equals(refName) && namedRefValue.Length >= 25)
                        {
                            // don't condense refs in {{reflist...}}
                            if (reflistIndex > 0 && m.Index > reflistIndex)
                                continue;

                            if (m.Index > articleText.Length)
                            {
                                reparse = true;
                                break;
                            }

                            // duplicate citation fixer (both named): <ref name="Fred">(...)</ref>....<ref name="Fred">\2</ref> --> ..<ref name="Fred"/>, minimum 25 characters to avoid short refs
                            string texttomatch = articleText.Substring(0, m.Index);
                            string textaftermatch = articleText.Substring(m.Index);

                            if (textaftermatch.Contains(m.Value))
                                articleText = texttomatch + textaftermatch.Replace(m.Value, @"<ref name=""" + refName + @"""/>");
                            else
                            {
                                reparse = true;
                                break;
                            }
                        }
                    }
                }

                // duplicate citation fixer (first named): <ref name="Fred">(...)</ref>....<ref>\2</ref> --> ..<ref name="Fred"/>
                // duplicate citation fixer (second named): <ref>(...)</ref>....<ref name="Fred">\2</ref> --> ..<ref name="Fred"/>
                articleText = WikiRegexes.UnnamedReferences.Replace(articleText, m =>
                                                                    {
                                                                        string newname;
                                                                        if(NamedRefs.TryGetValue(m.Groups[1].Value.Trim(), out newname))
                                                                            return @"<ref name=""" + newname + @"""/>";
                                                                        
                                                                        return m.Value;
                                                                    });

                if (!reparse)
                    break;
            }

            return articleText;
        }

        private const string RefName = @"(?si)<\s*ref\s+name\s*=\s*(?:""|')?";

        /// <summary>
        /// Matches unnamed references i.e. &lt;ref>...&lt;/ref>, group 1 being the ref value
        /// </summary>
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
            if (Variables.LangCode.Equals("en") && !HasNamedReferences(articleText))
                return articleText;

            Dictionary<int, List<Ref>> refs = new Dictionary<int, List<Ref>>();
            bool haveRefsToFix = false;

            // loop through all unnamed refs, add any duplicates to dictionary
            foreach (Match m in UnnamedRef.Matches(articleText))
            {
                string fullReference = m.Value;

                // ref contains ibid/loc cit or page needed, don't combine it, could refer to any ref on page
                if (WikiRegexes.IbidLocCitation.IsMatch(fullReference))
                    continue;

                string refContent = m.Groups[1].Value.Trim();
                int hash = refContent.GetHashCode();
                List<Ref> list;
                if (refs.TryGetValue(hash, out list))
                {
                    list.Add(new Ref { Text = fullReference, InnerText = refContent });
                    haveRefsToFix = true;
                }
                else
                {
                    list = new List<Ref> { new Ref { Text = fullReference, InnerText = refContent } };
                    refs.Add(hash, list);
                }
            }

            if (!haveRefsToFix)
                return articleText;

            StringBuilder result = new StringBuilder(articleText);

            // process each duplicate reference in dictionary
            foreach (KeyValuePair<int, List<Ref>> kvp in refs)
            {
                List<Ref> list = kvp.Value;
                if (list.Count < 2)
                    continue; // nothing to consolidate

                // get the reference name to use
                string friendlyName = DeriveReferenceName(articleText, list[0].InnerText);

                // check reference name was derived
                if (friendlyName.Length <= 3)
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

                articleText = result.ToString();
            }

            return articleText;
        }

        private static readonly Regex PageRef = new Regex(@"\s*(?:(?:[Pp]ages?|[Pp][pg]?[:\.]?)|^)\s*[XVI\d]", RegexOptions.Compiled);
        private static readonly Regex RefNameFromGroup = new Regex(@"<\s*ref\s+name\s*=\s*(?<nm>[^<>]*?)\s*group|group\s*=\s*[^<>]*?\s*name\s*=\s*(?<nm>[^<>]*?)\s*/?\s*>");

        /// <summary>
        /// Corrects named references where the reference is the same but the reference name is different
        /// </summary>
        /// <param name="articleText">the wiki text of the page</param>
        /// <returns>the updated wiki text</returns>
        public static string SameRefDifferentName(string articleText)
        {
            string articleTextOriginal = articleText;
            // refs with same name, but one is very short, so just change to <ref name=foo/> notation
            articleText = SameNamedRefShortText(articleText);

            Dictionary<string, string> NamedRefs = new Dictionary<string, string>();

            // get list of all ref names used in group refs
            List<string> RefsInGroupRef = (from Match m in WikiRegexes.RefsGrouped.Matches(articleText) 
                select RefNameFromGroup.Match(m.Value).Groups["nm"].Value.Trim(@"'""".ToCharArray())).ToList();

            foreach (Match m in WikiRegexes.NamedReferences.Matches(articleText))
            {
                string refname = m.Groups[2].Value, refvalue = m.Groups[3].Value, existingname;

                if (!NamedRefs.ContainsKey(refvalue))
                {
                    NamedRefs.Add(refvalue, refname);
                    continue;
                }

                NamedRefs.TryGetValue(refvalue, out existingname);

                // don't apply to ibid short ref, don't change if ref name used in a group ref
                if (existingname.Length > 0 && !existingname.Equals(refname) && !WikiRegexes.IbidLocCitation.IsMatch(refvalue) && !RefsInGroupRef.Contains(existingname))
                {
                    string newRefName = refname, oldRefName = existingname;

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

            // performance: only call if changes made
            if(!articleTextOriginal.Equals(articleText))
                articleText = DuplicateNamedReferences(articleText);

            return articleText;
        }

        private static readonly Regex ShortNameReference = new Regex(@"(<\s*ref\s+name\s*=\s*(?:""|')?([^<>=\r\n/]+?)(?:""|')?\s*>\s*([^<>]{1,9}?|\[?[Ss]ee above\]?|{{\s*[Cc]ite *\w+\s*}})\s*<\s*/\s*ref>)");

        /// <summary>
        /// refs with same name, but one is very short, so just change to &lt;ref name=foo/&gt; notation
        /// </summary>
        /// <param name="articleText">the wiki text of the page</param>
        /// <returns>the update wiki text</returns>
        private static string SameNamedRefShortText(string articleText)
        {
            // Peformance: get a list of all the short named refs that could be condensed
            // then only attempt replacement if some found and matching long named refs found
            List<string> ShortNamed = (from Match m in ShortNameReference.Matches(articleText) select m.Groups[2].Value).ToList();

            if(ShortNamed.Count > 0)
            {
                foreach (Match m in LongNamedReferences.Matches(articleText))
                {
                    string refname = m.Groups[2].Value;

                   // don't apply if short ref is a page ref
                   if(ShortNamed.Contains(refname) && m.Groups[3].Value.Length > 30)
                      articleText = Regex.Replace(articleText, @"(<\s*ref\s+name\s*=\s*(?:""|')?(" + Regex.Escape(refname) + @")(?:""|')?\s*>\s*([^<>]{1,9}?|\[?[Ss]ee above\]?|{{\s*[Cc]ite *\w+\s*}})\s*<\s*/\s*ref>)",
                                                    m2=> PageRef.IsMatch(m2.Groups[3].Value) ? m2.Value : @"<ref name=""" + refname + @"""/>");
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

            Parsers p = new Parsers();
            derivedName = p.FixDatesA(derivedName);
            derivedName = p.FixDatesB(derivedName, false, false);

            return DateRetrievedOrAccessed.IsMatch(derivedName) ? "" : derivedName;
        }

        private const string NameMask = @"(?-i)\s*(?:sir)?\s*((?:[A-Z]+\.?){0,3}\s*[A-Z][\w-']{2,}[,\.]?\s*(?:\s+\w\.?|\b(?:[A-Z]+\.?){0,3})?(?:\s+[A-Z][\w-']{2,}){0,3}(?:\s+\w(?:\.?|\b)){0,2})\s*(?:[,\.'&;:\[\(“`]|et\s+al)(?i)[^{}<>\n]*?";
        private const string YearMask = @"(\([12]\d{3}\)|\b[12]\d{3}[,\.\)])";
        private const string PageMask = @"('*(?:p+g?|pages?)'*\.?'*(?:&nbsp;)?\s*(?:\d{1,3}|(?-i)[XVICM]+(?i))\.?(?:\s*[-/&\.,]\s*(?:\d{1,3}|(?-i)[XVICM]+(?i)))?\b)";

        private static readonly Regex CitationCiteBook = new Regex(@"{{[Cc]it[ae]((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}})", RegexOptions.Compiled);
        private static readonly Regex CiteTemplatePagesParameter = new Regex(@"(?<=\s*pages?\s*=\s*)([^{}\|<>]+?)(?=\s*(?:\||}}))", RegexOptions.Compiled);
        private static readonly Regex UrlShortDescription = new Regex(@"\s*[^{}<>\n]*?\s*\[*(?:https?://www\.|https?://|www\.)[^\[\]<>""\s]+?\s+([^{}<>\[\]]{4,35}?)\s*(?:\]|<!--|\u230A\u230A\u230A\u230A)", RegexOptions.Compiled);
        private static readonly Regex UrlDomain = new Regex(@"\s*\w*?[^{}<>]{0,4}?\s*(?:\[?|\{\{\s*cit[^{}<>]*\|\s*url\s*=\s*)\s*(?:https?://www\.|https?://|www\.)([^\[\]<>""\s\/:]+)", RegexOptions.Compiled);
        private static readonly Regex HarvnbTemplate = new Regex(@"\s*{{ *(?:[Hh]arv(?:(?:col)?(?:nb|txt)|ard citation no brackets)?|[Ss]fn)\s*\|\s*([^{}\|]+?)\s*\|(?:[^{}]*?\|)?\s*(\d{4})\s*(?:\|\s*(?:pp?\s*=\s*)?([^{}\|]+?)\s*)?}}\s*", RegexOptions.Compiled);
        private static readonly Regex WholeShortReference = new Regex(@"\s*([^<>{}]{4,35})\s*", RegexOptions.Compiled);
        private static readonly Regex CiteTemplateUrl = new Regex(@"\s*\{\{\s*cit[^{}<>]*\|\s*url\s*=\s*([^\/<>{}\|]{4,35})", RegexOptions.Compiled);
        private static readonly Regex NameYearPage = new Regex(NameMask + YearMask + @"[^{}<>\n]*?" + PageMask + @"\s*", RegexOptions.Compiled);
        private static readonly Regex NamePage = new Regex(NameMask + PageMask + @"\s*", RegexOptions.Compiled);
        private static readonly Regex NameYear = new Regex(NameMask + YearMask + @"\s*", RegexOptions.Compiled);
        private static readonly Regex CiteDOIPMID = Tools.NestedTemplateRegex(new[] { "cite doi", "cite pmid" });

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
                string last = Tools.GetTemplateParameterValue(reference, "last");

                if (last.Length < 1)
                {
                    last = Tools.GetTemplateParameterValue(reference, "author");
                }

                if (last.Length > 1)
                {
                    derivedReferenceName = last;
                    string year = Tools.GetTemplateParameterValue(reference, "year");

                    string pages = CiteTemplatePagesParameter.Match(reference).Value.Trim();

                    if (year.Length > 3)
                        derivedReferenceName += " " + year;
                    else
                    {
                        string date = YearOnly.Match(Tools.GetTemplateParameterValue(reference, "date")).Value;

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
                    string title = Tools.GetTemplateParameterValue(reference, "title");

                    if (title.Length > 3 && title.Length < 35)
                        derivedReferenceName = title;
                    derivedReferenceName = CleanDerivedReferenceName(derivedReferenceName);

                    // try publisher
                    if (derivedReferenceName.Length < 4)
                    {
                        title = Tools.GetTemplateParameterValue(reference, "publisher");

                        if (title.Length > 3 && title.Length < 35)
                            derivedReferenceName = title;
                        derivedReferenceName = CleanDerivedReferenceName(derivedReferenceName);
                    }

                    // try work
                    if (derivedReferenceName.Length < 4)
                    {
                        title = Tools.GetTemplateParameterValue(reference, "work");

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

            // cite pmid / cite doi
            derivedReferenceName = Regex.Replace(ExtractReferenceNameComponents(reference, CiteDOIPMID, 3), @"[Cc]ite (pmid|doi)\s*\|\s*", "$1");

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

            // name...year...page. Regex checks to avoid excessive backtracking
            if(Regex.IsMatch(reference, YearMask) && NameYear.IsMatch(reference))
                derivedReferenceName = ExtractReferenceNameComponents(reference, NameYearPage, 3);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // name...page
            if(Regex.IsMatch(reference, PageMask))
                derivedReferenceName = ExtractReferenceNameComponents(reference, NamePage, 2);

            if (ReferenceNameValid(articleText, derivedReferenceName))
                return derivedReferenceName;

            // name...year
            if(Regex.IsMatch(reference, YearMask))
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
        /// Checks the validity of a new reference name:
        /// Name at least 3 characters and not already used in article, not just 'http'
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <param name="derivedReferenceName">The reference name</param>
        /// <returns>Whether the article does not already have a reference of that name</returns>
        private static bool ReferenceNameValid(string articleText, string derivedReferenceName)
        {
            return !Regex.IsMatch(articleText, RefName + Regex.Escape(derivedReferenceName) + @"(?:""|')?\s*/?\s*>") && derivedReferenceName.Length >= 3
                && !derivedReferenceName.Equals("http");
        }

        private static readonly Regex TlOrTlx = Tools.NestedTemplateRegex(new List<string>(new[] { "tl", "tlx" }));
        private static readonly Regex TemplateRedirectsR = new Regex(@"({{ *[Tt]lx? *\|.*}}) *→[ ']*({{ *[Tt]lx? *\| *(.*?) *}})");

        /// <summary>
        /// Processes the text of [[WP:AWB/Template redirects]] into a dictionary of regexes and new template names
        /// Format: {{tl|template 1}}, {{tl|template 2}} → {{tl|actual template}}
        /// </summary>
        public static Dictionary<Regex, string> LoadTemplateRedirects(string text)
        {
            text = WikiRegexes.UnformattedText.Replace(text, "");
            Dictionary<Regex, string> TRs = new Dictionary<Regex, string>();
            List<string> AllRedirectsList = new List<string>();

            foreach (Match m in TemplateRedirectsR.Matches(text))
            {
                string redirects = m.Groups[1].Value, templateName = m.Groups[2].Value;
                templateName = TlOrTlx.Match(templateName).Groups[3].Value.Trim('|').TrimEnd('}').Trim();

                // get all redirects into a list
                List<string> redirectsList = new List<string>();

                foreach (Match r in TlOrTlx.Matches(redirects))
                {
                    redirectsList.Add(Tools.TurnFirstToUpperNoProjectCheck(r.Groups[3].Value.Trim('|').TrimEnd('}').Trim()));
                }

                TRs.Add(Tools.NestedTemplateRegex(redirectsList), templateName);
                AllRedirectsList.AddRange(redirectsList);
            }
            
            WikiRegexes.AllTemplateRedirects = Tools.NestedTemplateRegex(AllRedirectsList);

            // must use separate functions to set value: if HashSets in same function compiler will pre-load them even if not used
            if(Globals.SystemCore3500Available)
                SetAllTemplateRedirectsHashSet(AllRedirectsList);
            else
                SetAllTemplateRedirectsList(AllRedirectsList);

            return TRs;
        }
        
        /// <summary>
        /// Sets the WikiRegexes .AllTemplateRedirects HashSet
        /// </summary>
        /// <param name="RedirectsList"></param>
        private static void SetAllTemplateRedirectsHashSet(List<string> RedirectsList)
        {
            WikiRegexes.AllTemplateRedirectsHS = new HashSet<string>(RedirectsList);
        }
        
        /// <summary>
        /// Sets the WikiRegexes .AllTemplateRedirects List
        /// </summary>
        /// <param name="RedirectsList"></param>
        private static void SetAllTemplateRedirectsList(List<string> RedirectsList)
        {
            WikiRegexes.AllTemplateRedirectsList = RedirectsList;
        }

        /// <summary>
        /// Processes the text of [[WP:AWB/Dated templates]] into a list of template names
        /// Format: * {{tl|Wikify}}
        /// </summary>
        /// <param name="text">The rule page text</param>
        /// <returns>List of templates to match dated templates</returns>
        public static List<string> LoadDatedTemplates(string text)
        {
            text = WikiRegexes.UnformattedText.Replace(text, "");
            return (from Match m in TlOrTlx.Matches(text) select Tools.TurnFirstToUpper(m.Groups[3].Value.Trim('|').TrimEnd('}').Trim())).ToList();
        }

        /// <summary>
        /// Renames templates to bypass template redirects from [[WP:AWB/Template redirects]]
        /// The first letter casing of the existing redirect is kept in the new template name,
        ///  except for acronym templates where first letter uppercase is enforced
        /// Calls TemplateToMagicWord if changes made
        /// </summary>
        /// <param name="articleText">the page text</param>
        /// <param name="TemplateRedirects">Dictionary of redirects and templates</param>
        /// <returns>The updated article text</returns>
        public static string TemplateRedirects(string articleText, Dictionary<Regex, string> TemplateRedirects)
        {
            string newArticleText;
            if(WikiRegexes.AllTemplateRedirects == null)
                return articleText;

            if(Globals.SystemCore3500Available)
                newArticleText = TemplateRedirectsHashSet(articleText, TemplateRedirects);
            else
                newArticleText = TemplateRedirectsList(articleText, TemplateRedirects);

            // call TemplateToMagicWord if changes made
            if (!newArticleText.Equals(articleText))
                return Tools.TemplateToMagicWord(newArticleText);

            return articleText;
        }

        /// <summary>
        /// Most performant version of TemplateRedirects using HashSets
        /// </summary>
        /// <param name="articleText"></param>
        /// <param name="TemplateRedirects"></param>
        /// <returns></returns>
        private static string TemplateRedirectsHashSet(string articleText, Dictionary<Regex, string> TemplateRedirects)
        {
            // performance: first check there's a match between templates used in article and listed template redirects
            // using intersection of HashSet lists of the two
            HashSet<string> TemplatesFound = new HashSet<string>(GetAllTemplates(articleText));
            TemplatesFound.IntersectWith(WikiRegexes.AllTemplateRedirectsHS);

            // run replacements only if matches found
            if(TemplatesFound.Any())
            {
                // performance: secondly filter the TemplateRedirects dictionary down to those rules matching templates used in article
                string all = String.Join(" ", TemplatesFound.Select(s => "{{" + s + "}}").ToArray());
                TemplateRedirects = TemplateRedirects.Where(s => s.Key.IsMatch(all)).ToDictionary(x => x.Key, y => y.Value);

                // performance: thirdly then run replacement for only those matching templates, and only against the matching rules, handle nested templates
                Regex MatchedTemplates = Tools.NestedTemplateRegex(TemplatesFound.ToList());
                while(MatchedTemplates.IsMatch(articleText))
                    articleText = MatchedTemplates.Replace(articleText, m2=>
                                                                              {
                                                                                  string res = m2.Value;
                                                                                  
                                                                                  foreach (KeyValuePair<Regex, string> kvp in TemplateRedirects)
                                                                                  {
                                                                                      res = kvp.Key.Replace(res, m => TemplateRedirectsME(m, kvp.Value));
                                                                                      
                                                                                      // if template name changed and not nested template, done, so break out
                                                                                      if(!res.Equals(m2.Value) && !m2.Groups[3].Value.Contains("{{"))
                                                                                          break;
                                                                                  }
                                                                                  return res;
                                                                              });
            }
            return articleText;
        }

        /// <summary>
        /// (slower) version of TemplateRedirects using List
        /// </summary>
        /// <param name="articleText"></param>
        /// <param name="TemplateRedirects"></param>
        /// <returns></returns>
        private static string TemplateRedirectsList(string articleText, Dictionary<Regex, string> TemplateRedirects)
        {
            List<string> TFH = GetAllTemplates(articleText);

            // if matches found, run replacements
            List<string> TFH2 = new List<string>();
            foreach(string s in TFH)
            {
                if(WikiRegexes.AllTemplateRedirectsList.Contains(s))
                    TFH2.Add(s);
            }

            if(TFH2.Count > 0)
            {
                articleText = Tools.NestedTemplateRegex(TFH2).Replace(articleText, m2=>
                                                                      {
                                                                          string res = m2.Value;
                                                                          
                                                                          foreach (KeyValuePair<Regex, string> kvp in TemplateRedirects)
                                                                          {
                                                                              res = kvp.Key.Replace(res, m => TemplateRedirectsME(m, kvp.Value));
                                                                              
                                                                              // if template name changed and not nested template, done, so break out
                                                                              if(!res.Equals(m2.Value) && !m2.Groups[3].Value.Contains("{{"))
                                                                                  break;
                                                                          }
                                                                          return res;
                                                                      });
            }
            return articleText;
        }

        private static readonly Regex AcronymTemplate = new Regex(@"^[A-Z]{3}");

        private static string TemplateRedirectsME(Match m, string newTemplateName)
        {
            string originalTemplateName = m.Groups[2].Value;

            if (!AcronymTemplate.IsMatch(newTemplateName))
            {
                if (Tools.TurnFirstToUpper(originalTemplateName).Equals(originalTemplateName))
                    newTemplateName = Tools.TurnFirstToUpper(newTemplateName);
                else
                    newTemplateName = Tools.TurnFirstToLower(newTemplateName);
            }

            return (m.Groups[1].Value + newTemplateName + m.Groups[3].Value);
        }

        private static readonly Regex NestedTemplates = new Regex(@"{{\s*([^{}\|]+)((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!)))}}");

        /// <summary>
        /// Extracts a list of all templates used in the input text, supporting any level of template nesting. Template name given in first letter upper
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public static List<string> GetAllTemplates(string articleText)
        {
            if(Globals.SystemCore3500Available)
                return GetAllTemplatesNew(articleText);
            return GetAllTemplatesOld(articleText);
        }

        /// <summary>
        /// Extracts a list of all templates used in the input text, supporting any level of template nesting. Template name given in first letter upper. Most performant version using HashSet.
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private static List<string> GetAllTemplatesNew(string articleText)
        {
            /* performance: process all templates in bulk, extract template contents and reprocess. This is faster than loop applying template match on individual basis. 
            Extract rough template name then get exact template names later, faster to deduplicate then get exact template names */
            // process all templates, handle nested templates to any level of nesting
            List<string> TFH = new List<string>();

            List<Match> nt = new List<Match>();
            HashSet<string> templateContents = new HashSet<string>();

            for(;;)
            {
                nt = (from Match m in NestedTemplates.Matches(articleText) select m).ToList();

                if(!nt.Any())
                    break;

                // add raw template names to list
                TFH.AddRange((from Match m in nt select m.Groups[1].Value).ToList());

                // set text to content of matched templates to process again for any (further) nested templates
                templateContents = new HashSet<string>((from Match m in nt select m.Groups[2].Value).ToList());
                articleText = String.Join(",", templateContents.ToArray());
            }

            // now extract exact template names
            TFH = Tools.DeduplicateList(TFH);

            return Tools.DeduplicateList((from string s in TFH select Tools.TurnFirstToUpper(Tools.GetTemplateName(@"{{" + s + @"}}"))).ToList());
        }

        /// <summary>
        /// Extracts a list of all templates used in the input text. Template name given in first letter upper. .NET 2.0 version not using HashSet
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private static List<string> GetAllTemplatesOld(string articleText)
        {
            /* performance: faster to process all templates, extract rough template name then get exact template names later
             than to get exact template name for each template found */
            // process all templates, handle nested templates to any level of nesting
            List<string> TFH = new List<string>();
            foreach(Match m in NestedTemplates.Matches(articleText))
            {
                TFH.Add(m.Groups[1].Value);
                string template = m.Value.Substring(2);

                while(NestedTemplates.IsMatch(template))
                {
                    Match m2 = NestedTemplates.Match(template);
                    TFH.Add(m2.Groups[1].Value);
                    template = template.Substring(m2.Index + 2);
                }
            }

            // now extract exact template names
            TFH = Tools.DeduplicateList(TFH);
            List<string> TFH2 = new List<string>();
            foreach(string s in TFH)
            {
                TFH2.Add(Tools.TurnFirstToUpper(Tools.GetTemplateName(@"{{" + s + @"}}")));
            }

            return Tools.DeduplicateList(TFH2);
        }

        private static Regex RenameTemplateParametersTemplates;
        private static List<string> RenameTemplateParametersOldParams;

        /// <summary>
        /// Renames parameters in template calls.
        /// Does not rename old to new if new paramter already in use with a value
        /// </summary>
        /// <param name="articleText">The wiki text</param>
        /// <param name="RenamedTemplateParameters">List of templates, old parameter, new parameter</param>
        /// <returns>The updated wiki text</returns>
        public static string RenameTemplateParameters(string articleText, List<WikiRegexes.TemplateParameters> RenamedTemplateParameters)
        {
            if (RenamedTemplateParameters.Count == 0)
                return articleText;

            // build Regex to match templates with parmeters to rename, plus list of old parameter names if not already cached
            if (RenameTemplateParametersTemplates == null)
            {
                List<string> Templates = new List<string>();
                RenameTemplateParametersOldParams = new List<string>();

                foreach (WikiRegexes.TemplateParameters Params in RenamedTemplateParameters)
                {
                    Templates.Add(Params.TemplateName);
                    RenameTemplateParametersOldParams.Add(Params.OldParameter);
                }

                RenameTemplateParametersOldParams = Tools.DeduplicateList(RenameTemplateParametersOldParams);
                RenameTemplateParametersTemplates = Tools.NestedTemplateRegex(Tools.DeduplicateList(Templates));
            }

            return RenameTemplateParametersTemplates.Replace(articleText,
                                                             m => (Globals.SystemCore3500Available ?
                                                                   RenameTemplateParametersHashSetME(m, RenamedTemplateParameters)
                                                                   : RenameTemplateParametersME(m, RenamedTemplateParameters)));
        }

        /// <summary>
        /// Most performant RenameTemplateParameters MatchEvaluator using HashSets
        /// </summary>
        /// <param name="m"></param>
        /// <param name="RenamedTemplateParameters"></param>
        /// <returns></returns>
        private static string RenameTemplateParametersHashSetME(Match m, List<WikiRegexes.TemplateParameters> RenamedTemplateParameters)
        {
            string newvalue = m.Value;

            // performance: check for intersection of bad parameters and parameters used in template
            // rather than simply looping through all parameters in list
            Dictionary<string, string> pv = Tools.GetTemplateParameterValues(m.Value);
            if(RenameTemplateParametersOldParams.Intersect(pv.Keys.ToArray()).Any())
            {
                foreach (WikiRegexes.TemplateParameters Params in RenamedTemplateParameters)
                {
                    if (Params.TemplateName.Equals(Tools.TurnFirstToLower(m.Groups[2].Value)) && pv.ContainsKey(Params.OldParameter))
                    {
                        string newp;
                        pv.TryGetValue(Params.NewParameter, out newp);

                        if(string.IsNullOrEmpty(newp))
                            newvalue = Tools.RenameTemplateParameter(newvalue, Params.OldParameter, Params.NewParameter);
                    }
                }
            }

            return newvalue;
        }

        /// <summary>
        /// Less performant RenameTemplateParameters not using HashSets
        /// </summary>
        /// <param name="m"></param>
        /// <param name="RenamedTemplateParameters"></param>
        /// <returns></returns>
        private static string RenameTemplateParametersME(Match m, List<WikiRegexes.TemplateParameters> RenamedTemplateParameters)
        {
            string templatename = Tools.TurnFirstToLower(m.Groups[2].Value), newvalue = m.Value;

            foreach (WikiRegexes.TemplateParameters Params in RenamedTemplateParameters)
            {
                if (Params.TemplateName.Equals(templatename) && newvalue.Contains(Params.OldParameter)
                    && Tools.GetTemplateParameterValue(m.Value, Params.NewParameter).Length == 0)
                    newvalue = Tools.RenameTemplateParameter(newvalue, Params.OldParameter, Params.NewParameter);
            }

            return newvalue;
        }

        /// <summary>
        /// Loads List of templates, old parameter, new parameter from within {{AWB rename template parameter}}
        /// </summary>
        /// <param name="text">Source page of {{AWB rename template parameter}} rules</param>
        /// <returns>List of templates (first letter lower), old parameter, new parameter</returns>
        public static List<WikiRegexes.TemplateParameters> LoadRenamedTemplateParameters(string text)
        {
            text = WikiRegexes.UnformattedText.Replace(text, "");
            List<WikiRegexes.TemplateParameters> TPs = new List<WikiRegexes.TemplateParameters>();

            foreach (Match m in Tools.NestedTemplateRegex("AWB rename template parameter").Matches(text))
            {
                string templatename = Tools.TurnFirstToLower(Tools.GetTemplateArgument(m.Value, 1)), oldparam = Tools.GetTemplateArgument(m.Value, 2),
                newparam = Tools.GetTemplateArgument(m.Value, 3);

                WikiRegexes.TemplateParameters Params;
                Params.TemplateName = templatename;
                Params.OldParameter = oldparam;
                Params.NewParameter = newparam;

                TPs.Add(Params);
            }

            return TPs;
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

            noChange = newText.Equals(articleText);

            return newText;
        }

        private static readonly Regex CiteArXiv = Tools.NestedTemplateRegex(new[] { "cite arxiv", "cite arXiv" });
        private static readonly Regex CitationPopulatedParameter = new Regex(@"\|\s*([\w_\d- ']+)\s*=\s*([^\|}]+)");
        private static readonly Regex citeWebParameters = new Regex(@"^(access-?date|agency|archive-?date|archive-?url|arxiv|ARXIV|asin|ASIN|asin-tld|ASIN-TLD|at|[Aa]uthor\d*|author\d*-first|author-?format|author\d*-last|author-?link\d*|author\d*-?link|authors|author-mask|author-name-separator|author-separator|bibcode|BIBCODE|date|dead-?url|dictionary|display-?authors|display-?editors|doi|DOI|DoiBroken|doi-broken|doi-broken-date|doi_brokendate|doi-inactive-date|doi_inactivedate|edition|[Ee]ditor|editor\d*|editor\d*-first|editor-?format|EditorGiven\d*|editor\d*-given|editor\d*-last|editor\d*-?link|editor-?mask|editor-name-separator|EditorSurname\d*|editor\d*-surname|editor-first\d*|editor-given\d*|editor-last\d*|editor-surname\d*|editorlink\d*|editors|[Ee]mbargo|encyclopa?edia|first\d*|format|given\d*|id|ID|ignoreisbnerror|ignore-isbn-error|institution|isbn|ISBN|isbn13|ISBN13|issn|ISSN|issue|jfm|JFM|journal|jstor|JSTOR|language|last\d*|lastauthoramp|last-author-amp|lay-?date|lay-?source|lay-?summary|lay-?url|lccn|LCCN|location|magazine|mr|MR|newspaper|no-?pp|number|oclc|OCLC|ol|OL|orig-?year|others|osti|pp?|pages?|people|periodical|place|pmc|PMC|pmid|PMID|postscript|publication-?(?:place|date)|publisher|quotation|quote|[Rr]ef|registration|rfc|RFC|script\-title|separator|series|series-?link|ssrn|SSRN|subscription|surname\d*|title|trans[_-]title|type|url|URL|version|via|volume|website|work|year|zbl|ZBL)\b", RegexOptions.Compiled);
        private static readonly Regex citeArXivParameters = new Regex(@"\b(arxiv|asin|ASIN|author\d*|authorlink\d*|author\d*-link|bibcode|class|coauthors?|date|day|doi|DOI|doi brokendate|doi inactivedate|eprint|first\d*|format|given\d*|id|in|isbn|ISBN|issn|ISSN|jfm|JFM|jstor|JSTOR|language|last\d*|laydate|laysource|laysummary|lccn|LCCN|month|mr|MR|oclc|OCLC|ol|OL|osti|OSTI|pmc|PMC|pmid|PMID|postscript|publication-date|quote|ref|rfc|RFC|separator|seperator|ssrn|SSRN|surname\d*|title|version|year|zbl)\b", RegexOptions.Compiled);
        private static readonly Regex NoEqualsTwoBars = new Regex(@"\|[^=\|]+\|");

        /// <summary>
        /// Searches for unknown/invalid parameters within citation templates
        /// </summary>
        /// <param name="articleText">the wiki text to search</param>
        /// <returns>Dictionary of parameter index in wiki text, and parameter length</returns>
        public static Dictionary<int, int> BadCiteParameters(string articleText)
        {
            Dictionary<int, int> found = new Dictionary<int, int>();

            // unknown parameters in cite arXiv
            foreach (Match m in CiteArXiv.Matches(articleText))
            {
                // ignore parameters in templates within cite
                string cite = @"{{" + Tools.ReplaceWithSpaces(m.Value.Substring(2), WikiRegexes.NestedTemplates.Matches(m.Value.Substring(2)));

                foreach (Match m2 in CitationPopulatedParameter.Matches(cite))
                {
                    if (!citeArXivParameters.IsMatch(m2.Groups[1].Value) && Tools.GetTemplateParameterValue(cite, m2.Groups[1].Value).Length > 0)
                        found.Add(m.Index + m2.Groups[1].Index, m2.Groups[1].Length);
                }
            }

            foreach (Match m in WikiRegexes.CiteTemplate.Matches(articleText))
            {
                // unknown parameters in cite web
                if(m.Groups[2].Value.EndsWith("web"))
                {
                    // ignore parameters in templates within cite
                    string cite = @"{{" + Tools.ReplaceWithSpaces(m.Value.Substring(2), WikiRegexes.NestedTemplates.Matches(m.Value.Substring(2)));

                    foreach (Match m2 in CitationPopulatedParameter.Matches(cite))
                    {
                        if (!citeWebParameters.IsMatch(m2.Groups[1].Value) && Tools.GetTemplateParameterValue(cite, m2.Groups[1].Value).Length > 0)
                            found.Add(m.Index + m2.Groups[1].Index, m2.Groups[1].Length);
                    }
                }

                string pipecleaned = Tools.PipeCleanedTemplate(m.Value, false);

                // no equals between two separator pipes
                if (pipecleaned.Contains("="))
                {
                    Match m2 = NoEqualsTwoBars.Match(pipecleaned);

                    if (m2.Success)
                        found.Add(m.Index + m2.Index, m2.Length);
                }

                // URL has space in it
                int urlpos = m.Value.IndexOf("url");
                if(urlpos > 0)
                {
                    string URL = Tools.GetTemplateParameterValue(m.Value, "url");
                    if (URL.Contains(" ") && WikiRegexes.UnformattedText.Replace(WikiRegexes.NestedTemplates.Replace(URL, ""), "").Trim().Contains(" "))
                    {
                        string fromURL = m.Value.Substring(urlpos); // value of url may be in another earlier parameter, report correct position
                        found.Add(m.Index + urlpos + fromURL.IndexOf(URL), URL.Length);
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
            return DictionaryOfMatches(articleText, WikiRegexes.DeadLink);
        }

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

        private const string SiCitStart = @"(?si)(\|\s*";
        private const string CitAccessdate = SiCitStart + @"(?:access|archive)\-?date\s*=\s*";
        private const string CitDate = SiCitStart + @"(?:archive|air)?date2?\s*=\s*";

        private static readonly RegexReplacement[] CiteTemplateIncorrectISOAccessdates = {
            new RegexReplacement(CitAccessdate + @")(1[0-2])[/_\-\.]?(1[3-9])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3"),
            new RegexReplacement(CitAccessdate + @")(1[0-2])[/_\-\.]?([23]\d)[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3"),
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

            new RegexReplacement(CitAccessdate + @")([23]\d)[/_\-\.](1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2"),
            new RegexReplacement(CitAccessdate + @")([23]\d)[/_\-\.]0?([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2"),
            new RegexReplacement(CitAccessdate + @")0?([1-9])[/_\-\.]?(1[3-9]|[23]\d)[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$2-$3"),
            new RegexReplacement(CitAccessdate + @")0?([1-9])[/_\-\.]?0?\2[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$3-0$2-0$2") // n-n-2004 and n-n-04 to ISO format (both n the same)
        };

        private static readonly RegexReplacement[] CiteTemplateIncorrectISODates = {
            new RegexReplacement(CitDate + @"\[?\[?)(20\d\d|19[7-9]\d)[/_]?([01]\d)[/_]?([0-3]\d\s*(?:\||}}))", "$1$2-$3-$4"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[0-2])[/_\-\.]?([23]\d)[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)0?([1-9])[/_\-\.]?([23]\d)[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)([23]\d)[/_\-\.]?0?([1-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-0$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)([23]\d)[/_\-\.]?(1[0-2])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[0-2])[/_\-\.]([23]\d)[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)0?([1-9])[/_\-\.]([23]\d)[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)([23]\d)[/_\-\.]0?([1-9])[/_\-\.](?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-0$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)([23]\d)[/_\-\.](1[0-2])[/_\-\.]?(?:20)?([01]\d)(?=\s*(?:\||}}))", "${1}20$4-$3-$2"),
            new RegexReplacement(CitDate + @"\[?\[?)(1[0-2])[/_\-\.]?(1[3-9])[/_\-\.]?(19[7-9]\d)(?=\s*(?:\||}}))", "$1$4-$2-$3"),
            new RegexReplacement(CitDate + @"\[?\[?)0?([1-9])[/_\-\.](1[3-9])[/_\-\.](19[7-9]\d|20\d\d)(?=\s*(?:\||}}))", "$1$4-0$2-$3"),
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
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|1[5-9]\d{2})[/_\-\.]([1-9])[/_\-\.]0?([1-9](?:\]\])?\s*(?:\||}}))", "$1$2-0$3-0$4"),
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|1[5-9]\d{2})[/_\-\.]0?([1-9])[/_\-\.]([1-9](?:\]\])?\s*(?:\||}}))", "$1$2-0$3-0$4"),
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|1[5-9]\d{2})[/_\-\.]?([01]\d)[/_\-\.]?([1-9](?:\]\])?\s*(?:\||}}))", "$1$2-$3-0$4"),
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|1[5-9]\d{2})[/_\-\.]?([1-9])[/_\-\.]?([0-3]\d(?:\]\])?\s*(?:\||}}))", "$1$2-0$3-$4"),
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|1[5-9]\d{2})([01]\d)[/_\-\.]([0-3]\d(?:\]\])?\s*(?:\||}}))", "$1$2-$3-$4"),
            new RegexReplacement(CitDate + @")((?:\[\[)?20\d\d|1[5-9]\d{2})[/_\-\.](0[1-9]|1[0-2])0?([0-3]\d(?:\]\])?\s*(?:\||}}))", "$1$2-$3-$4")
        };

        private static readonly Regex CiteTemplateAbbreviatedMonthISO = new Regex(@"(?si)(\|\s*(?:archive|air|access)?date2?\s*=\s*)(\d{4}[-/\s][A-Z][a-z]+\.?[-/\s][0-3]?\d)(\s*(?:\||}}))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex CiteTemplateDateYYYYDDMMFormat = new Regex(SiCitStart + @"(?:archive|air|access)?date2?\s*=\s*(?:\[\[)?20\d\d)-([23]\d|1[3-9])-(0[1-9]|1[0-2])(\]\])?");
        private static readonly Regex CiteTemplateTimeInDateParameter = new Regex(@"(\|\s*(?:archive|air|access)?date2?\s*=\s*(?:(?:20\d\d|19[7-9]\d)-[01]?\d-[0-3]?\d|[0-3]?\d[a-z]{0,2}\s*\w+,?\s*(?:20\d\d|19[7-9]\d)|\w+\s*[0-3]?\d[a-z]{0,2},?\s*(?:20\d\d|19[7-9]\d)))(\s*[,-:]?\s+[0-2]?\d[:\.]?[0-5]\d(?:\:?[0-5]\d)?\s*(?:[^\|\}]*\[\[[^[\]\n]+(?<!\[\[[A-Z]?[a-z-]{2,}:[^[\]\n]+)\]\][^\|\}]*|[^\|\}]*)?)(?<!.*(?:20|1[7-9])\d+\s*)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static readonly Regex WhitespaceEnd = new Regex(@"(\s+)$");
        private static readonly Regex CitePodcast = Tools.NestedTemplateRegex("cite podcast");

        /// <summary>
        /// Corrects common formatting errors in dates in external reference citation templates (doesn't link/delink dates)
        /// note some incorrect date formats such as 3-2-2009 are ambiguous as could be 3-FEB-2009 or MAR-2-2009, these fixes don't address such errors
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string CiteTemplateDates(string articleText)
        {
            // cite podcast is non-compliant to citation core standards
            if (!Variables.IsWikipediaEN || CitePodcast.IsMatch(articleText))
                return articleText;

            string articleTextlocal = "", originalArticleText = articleText;

            // loop in case a single citation has multiple dates to be fixed
            while (!articleTextlocal.Equals(articleText))
            {
                articleTextlocal = articleText;

                // loop in case a single citation has multiple dates to be fixed
                articleText =  WikiRegexes.CiteTemplate.Replace(articleText,  CiteTemplateME);
            }

            // don't apply fixes when ambiguous dates present, for performance only appply this check if changes made
            if(!originalArticleText.Equals(articleText) && AmbiguousCiteTemplateDates(originalArticleText))
                return originalArticleText;

            return articleText;
        }

        /// <summary>
        /// convert invalid date formats like DD-MM-YYYY, MM-DD-YYYY, YYYY-D-M, YYYY-DD-MM, YYYY_MM_DD etc. to iso format of YYYY-MM-DD
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static string CiteTemplateME(Match m)
        {
            string newValue = m.Value;

            Dictionary<string, string> paramsFound = Tools.GetTemplateParameterValues(newValue);            

            string accessdate, date, date2, archivedate, airdate, journal;
            if(!paramsFound.TryGetValue("accessdate", out accessdate) && !paramsFound.TryGetValue("access-date", out accessdate))
                accessdate = "";
            if(!paramsFound.TryGetValue("date", out date))
                date = "";
            if(!paramsFound.TryGetValue("date2", out date2))
                date2 = "";
            if(!paramsFound.TryGetValue("archivedate", out archivedate))
                archivedate = "";
            if(!paramsFound.TryGetValue("airdate", out airdate))
                airdate = "";
            if(!paramsFound.TryGetValue("journal", out journal))
                journal = "";

            List<string> dates = new List<string> {accessdate, archivedate, date, date2, airdate};

            if(CiteTemplateMEParameterToProcess(dates))
            {
                // accessdate=, archivedate=
                newValue = CiteTemplateIncorrectISOAccessdates.Aggregate(newValue, (current, rr) => rr.Regex.Replace(current, rr.Replacement));

                // date=, archivedate=, airdate=, date2=
                newValue = CiteTemplateIncorrectISODates.Aggregate(newValue, (current, rr) => rr.Regex.Replace(current, rr.Replacement));

                newValue = CiteTemplateDateYYYYDDMMFormat.Replace(newValue, "$1-$3-$2$4"); // YYYY-DD-MM to YYYY-MM-DD

                // date = YYYY-Month-DD fix, not for cite journal PubMed date format
                if(journal.Length == 0)
                    newValue = CiteTemplateAbbreviatedMonthISO.Replace(newValue, m2 => m2.Groups[1].Value + Tools.ConvertDate(m2.Groups[2].Value.Replace(".", ""), DateLocale.ISO) + m2.Groups[3].Value);
            }
            // all citation dates: Remove time from date fields
            newValue = CiteTemplateTimeInDateParameter.Replace(newValue, m3 => {
                                                                   // keep end whitespace outside comment
                                                                   string comm = m3.Groups[2].Value, whitespace = "";

                                                                   Match whm = WhitespaceEnd.Match(comm);

                                                                   if(whm.Success)
                                                                   {
                                                                       comm = comm.TrimEnd();
                                                                       whitespace = whm.Groups[1].Value;
                                                                   }

                                                                   return m3.Groups[1].Value + "<!--" + comm + @"-->" + whitespace;
                                                               });

            return newValue;
        }

        private static bool CiteTemplateMEParameterToProcess(List<string> parameters)
        {
            foreach(string s in parameters)
            {
                if(s.Length > 4 && !WikiRegexes.ISODates.IsMatch(s)
                   && !Regex.IsMatch(s, @"^(\d{1,2} *)?" + WikiRegexes.MonthsNoGroup))
                    return true;
            }
            return false;
        }

        private static readonly Regex PossibleAmbiguousCiteDate = new Regex(@"(?<=\|\s*(?:access|archive|air)?date2?\s*=\s*)(0?[1-9]|1[0-2])[/_\-\.](0?[1-9]|1[0-2])[/_\-\.](20\d\d|19[7-9]\d|[01]\d)\b");
        private static readonly Regex PossibleAmbiguousCiteDateQuick = new Regex(@"(\|\s*(?:access|archive|air)?date2?\s*=\s*)(0?[1-9]|1[0-2])[/_\-\.](0?[1-9]|1[0-2])[/_\-\.](20\d\d|19[7-9]\d|[01]\d)\b");

        /// <summary>
        /// Returns whether the input article text contains ambiguous cite template dates in XX-XX-YYYY or XX-XX-YY format
        /// </summary>
        /// <param name="articleText">the article text to search</param>
        /// <returns>If any matches were found</returns>
        public static bool AmbiguousCiteTemplateDates(string articleText)
        {
            return AmbigCiteTemplateDates(articleText).Count > 0;
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

        private static readonly Regex MathSourceCodeNowikiPreTag = new Regex(@"<!--|(<\s*/?\s*(?:math|(?:source|ref|gallery)\b[^>]*|code|nowiki|pre|small|center|sup|sub)\s*(?:>|$))", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SmallStart = new Regex(@"<\s*small\s*>", RegexOptions.IgnoreCase);
        private static readonly Regex SmallEnd = new Regex(@"<\s*/\s*small\s*>", RegexOptions.IgnoreCase);
        private static readonly Regex SmallNoNestedTags = new Regex(@"<\s*small\s*>((?>[^<>]*|<\s*small\s*>(?<DEPTH>)|<\s*/\s*small\s*>(?<-DEPTH>))*(?(DEPTH)(?!)))<\s*/\s*small\s*>", RegexOptions.IgnoreCase);
        private static readonly Regex CenterTag = new Regex(@"<\s*center\s*>((?>(?!<\s*/?\s*center\s*>).|<\s*center\s*>(?<DEPTH>)|<\s*/\s*center\s*>(?<-DEPTH>))*(?(DEPTH)(?!)))<\s*/\s*center\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static readonly Regex SupTag = new Regex(@"<\s*sup\s*>((?>(?!<\s*/?\s*sup\s*>).|<\s*sup\s*>(?<DEPTH>)|<\s*/\s*sup\s*>(?<-DEPTH>))*(?(DEPTH)(?!)))<\s*/\s*sup\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static readonly Regex SubTag = new Regex(@"<\s*sub\s*>((?>(?!<\s*/?\s*sub\s*>).|<\s*sub\s*>(?<DEPTH>)|<\s*/\s*sub\s*>(?<-DEPTH>))*(?(DEPTH)(?!)))<\s*/\s*sub\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static readonly Regex AnyTag = new Regex(@"<([^<>]+)>");
        private static readonly Regex TagToEnd = new Regex(@"<[^>]+$");
        /// <summary>
        ///  Searches for any unclosed &lt;math&gt;, &lt;source&gt;, &lt;ref&gt;, &lt;code&gt;, &lt;nowiki&gt;, &lt;small&gt;, &lt;pre&gt; &lt;center&gt; &lt;sup&gt; &lt;sub&gt; or &lt;gallery&gt; tags and comments
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>dictionary of the index and length of any unclosed tags</returns>
        public static Dictionary<int, int> UnclosedTags(string articleText)
        {
            Dictionary<int, int> back = new Dictionary<int, int>();

            // Peformance: get all tags, compare the count of matched tags of same name
            // Then do full tag search if unmatched tags found

            // get all tags in format <tag...> in article
            List<string> AnyTagList = (from Match m in AnyTag.Matches(articleText)
                select m.Groups[1].Value.Trim().ToLower()).ToList();

            // discard self-closing tags in <tag/> format, discard wiki comments
            AnyTagList = AnyTagList.Where(s => !s.EndsWith("/") && !s.StartsWith("!--")).ToList();

            // remove any text after first space, so we're left with tag name only
            AnyTagList = AnyTagList.Select(s => s.Contains(" ") ? s.Substring(0, s.IndexOf(" ")).Trim() : s).ToList();

            // get the distinct tag names in use, discard <br> tags as not a tag pair
            List<string> DistinctTags = Tools.DeduplicateList(AnyTagList).Where(s => !s.Equals("br")).ToList();

            // determine if unmatched tags by comparing count of opening and closing tags
            bool unmatched = false;
            foreach(string d in DistinctTags)
            {
                int startTagCount, endTagCount;

                if(d.StartsWith("/"))
                {
                    endTagCount = AnyTagList.Where(s => s.Equals(d)).Count();
                    startTagCount = AnyTagList.Where(s => s.Equals(d.TrimStart('/'))).Count();
                }
                else
                {
                    endTagCount = AnyTagList.Where(s => s.Equals("/" + d)).Count();
                    startTagCount = AnyTagList.Where(s => s.Equals(d)).Count();
                }

                if(startTagCount != endTagCount)
                {
                    unmatched = true;
                    break;
                }
            }

            // check for any unmatched tags or unclosed part tag
            if(!unmatched && !TagToEnd.IsMatch(AnyTag.Replace(articleText, "")))
                return back;
            
            // if here then have some unmatched tags, so do full clear down and search

            // clear out all the matched tags: performance of Refs/SourceCode is better if IgnoreCase avoided
            articleText = articleText.ToLower();
            articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.UnformattedText);
            articleText = Tools.ReplaceWithSpaces(articleText, new Regex(WikiRegexes.SourceCode.ToString(), RegexOptions.Singleline));
            articleText = Tools.ReplaceWithSpaces(articleText, new Regex(WikiRegexes.Refs.ToString(), RegexOptions.Singleline));
            articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.GalleryTag, 2);
            articleText = Tools.ReplaceWithSpaces(articleText, CenterTag, 2);
            articleText = Tools.ReplaceWithSpaces(articleText, SupTag, 2);
            articleText = Tools.ReplaceWithSpaces(articleText, SubTag, 2);
            
            // some (badly done) List of pages can have hundreds of unclosed small tags, causes WikiRegexes.Small to backtrack a lot
            // so workaround solution: if > 10 unclosed small tags, only remove small tags without other tags embedded in them
            // Workaround constraint: we might incorrectly report some valid small tags with < or > in them as unclosed
            int smallstart = SmallStart.Matches(articleText).Count;
            if(smallstart > 0)
            {
                if(smallstart > (SmallEnd.Matches(articleText).Count + 5))
                    articleText = Tools.ReplaceWithSpaces(articleText, SmallNoNestedTags);
                else
                    articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.Small);
            }
            
            foreach (Match m in MathSourceCodeNowikiPreTag.Matches(articleText))
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
            if(PossibleAmbiguousCiteDateQuick.IsMatch(articleText)) // check for performance
            {
                foreach(Match m in WikiRegexes.CiteTemplate.Matches(articleText))
                {
                    foreach (Match m2 in PossibleAmbiguousCiteDate.Matches(m.Value))
                    {
                        // for YYYY-AA-BB date, ambiguous if AA and BB not the same
                        if (!m2.Groups[1].Value.Equals(m2.Groups[2].Value))
                            ambigDates.Add(m.Index+m2.Index, m2.Length);
                    }
                }
            }

            return ambigDates;
        }

        private static readonly Regex PageRangeIncorrectMdash = new Regex(@"(pages\s*=\s*|pp\.?\s*)((?:&nbsp;)?\d+\s*)(?:\-\-?|—|&mdash;|&#8212;)(\s*\d+)", RegexOptions.IgnoreCase);

        // avoid dimensions in format 55-66-77
        private static readonly Regex UnitTimeRangeIncorrectMdash = new Regex(@"(?<!-)(\b[1-9]?\d+\s*)(?:-|—|&mdash;|&#8212;)(\s*[1-9]?\d+)(\s+|&nbsp;)((?:years|months|weeks|days|hours|minutes|seconds|[km]g|kb|[ckm]?m|[Gk]?Hz|miles|mi\.|%|feet|foot|ft|met(?:er|re)s)\b|in\))");
        private static readonly Regex UnitTimeRangeIncorrectMdashQuick = new Regex(@"(\b|^)[0-9]+\s*(?:-|—|&mdash;|&#8212;)(\s*[0-9]+)(\s+|&nbsp;)((?:years|months|weeks|days|hours|minutes|seconds|[km]g|kb|[ckm]?m|[Gk]?Hz|miles|mi\.|%|feet|foot|ft|met(?:er|re)s)|in\))");
        private static readonly Regex DollarAmountIncorrectMdash = new Regex(@"(\$[1-9]?\d{1,3}\s*)(?:-|—|&mdash;|&#8212;)(\s*\$?[1-9]?\d{1,3})");
        private static readonly Regex AMPMIncorrectMdash = new Regex(@"([01]?\d:[0-5]\d\s*([AP]M)\s*)(?:-|—|&mdash;|&#8212;)(\s*[01]?\d:[0-5]\d\s*([AP]M))", RegexOptions.IgnoreCase);
        private static readonly Regex AMPMIncorrectMdashQuick = new Regex(@"\b[AP]M\s*[\-—&]", RegexOptions.IgnoreCase);
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
            // replace hyphen with dash and convert Pp. to pp.
            articleText = PageRangeIncorrectMdash.Replace(articleText, m=>
                                                          {
                                                              string pagespart = m.Groups[1].Value;
                                                              if (pagespart.Contains(@"Pp"))
                                                                  pagespart = pagespart.ToLower();

                                                              return pagespart + m.Groups[2].Value + @"–" + m.Groups[3].Value;
                                                          });

            if(UnitTimeRangeIncorrectMdashQuick.IsMatch(articleText))
                articleText = UnitTimeRangeIncorrectMdash.Replace(articleText, @"$1–$2$3$4");

            articleText = DollarAmountIncorrectMdash.Replace(articleText, @"$1–$2");

            if(AMPMIncorrectMdashQuick.IsMatch(articleText))
                articleText = AMPMIncorrectMdash.Replace(articleText, @"$1–$3");

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

        // Covered by: FootnotesTests.TestFixReferenceListTags()
        private static string ReflistMatchEvaluator(Match m)
        {
            // don't change anything if div tags mismatch
            if(DivStart.Matches(m.Value).Count != DivEnd.Matches(m.Value).Count)
                return m.Value;

            // {{reflist}} template not used on sv-wiki
            if(Variables.LangCode == "sv")
                return "<references/>";

            if(m.Value.Contains("references-2column") || m.Value.Contains("column-count:2"))
                return "{{Reflist|2}}";

            return "{{Reflist}}";
        }

        /// <summary>
        /// Main regex for {{Reflist}} converter
        /// </summary>
        private static readonly Regex ReferenceListTags = new Regex(@"(<(span|div)( class=""(references-small|small|references-2column)|)?""(?:\s*style\s*=\s*""[^<>""]+?""\s*)?>[\r\n\s]*){1,2}[\r\n\s]*<references[\s]?/>([\r\n\s]*</(span|div)>){1,2}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex ReferenceListSmallTags = new Regex(@"(<small>[\r\n\s]*){1,2}[\r\n\s]*<references[\s]?/>([\r\n\s]*</small>){1,2}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex DivStart = new Regex(@"<div\b.*?>", RegexOptions.Compiled);
        private static readonly Regex DivEnd = new Regex(@"< ?/ ?div\b.*?>", RegexOptions.Compiled);

        // Covered by: FootnotesTests.TestFixReferenceListTags()
        /// <summary>
        /// Replaces various old reference tag formats, with the new {{Reflist}}, or &lt;references/&gt; for sv-wiki
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns>The updated article text</returns>
        public static string FixReferenceListTags(string articleText)
        {
            // check for performance
            if(articleText.IndexOf(@"<references", StringComparison.OrdinalIgnoreCase) < 0)
                return articleText;

            articleText = ReferenceListSmallTags.Replace(articleText, ReflistMatchEvaluator);
            return ReferenceListTags.Replace(articleText, ReflistMatchEvaluator);
        }

        private static readonly Regex EmptyReferences = new Regex(@"(<ref\s+name\s*=\s*[^<>=\r\n]+?)\s*(?:/\s*)?>\s*<\s*/\s*ref\s*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Covered by: FootnotesTests.TestSimplifyReferenceTags()
        /// <summary>
        /// Replaces reference tags in the form &lt;ref name="blah">&lt;/ref> with &lt;ref name="blah" />
        /// Removes some of the MW errors that occur from the prior
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns>The updated article text</returns>
        public static string SimplifyReferenceTags(string articleText)
        {
            return EmptyReferences.Replace(articleText, @"$1 />");
        }

        private static readonly Regex LinksHeading = new Regex(@"(?sim)(==+\s*)Links(\s*==+\s*(?:^(?:\*|\d\.?)?\s*\[?\s*http://))", RegexOptions.Compiled);
        private static readonly Regex ReferencesHeadingLevel2 = new Regex(@"(?i)==\s*'*\s*References?\s*'*\s*==", RegexOptions.Compiled);
        private static readonly Regex ReferencesHeadingLevelLower = new Regex(@"(?i)(==+\s*'*\s*References?\s*'*\s*==+)", RegexOptions.Compiled);
        private static readonly Regex ExternalLinksHeading = new Regex(@"(?im)(^\s*=+\s*(?:External\s+link|Source|Web\s*link)s?\s*=)", RegexOptions.Compiled);
        private static readonly Regex ExternalLinksToReferences = new Regex(@"(?sim)(^\s*=+\s*(?:External\s+link|Source|Web\s*link)s?\s*=+.*?)(\r\n==+References==+\r\n{{Reflist}})", RegexOptions.Compiled);
        private static readonly Regex Category = new Regex(@"(?im)(^\s*\[\[\s*Category\s*:)", RegexOptions.Compiled);
        private static readonly Regex CategoryToReferences = new Regex(@"(?sim)((?:^\{\{(?!(?:[Tt]racklist|[Ss]\-end)\b)[^{}]+?\}\}\s*)*)(^\s*\[\[\s*Category\s*:.*?)(\r\n==+References==+\r\n{{Reflist}})", RegexOptions.Compiled);

        private static readonly Regex ReferencesMissingSlash = new Regex(@"<\s*[Rr]eferences\s*>", RegexOptions.Compiled);

        /// <summary>
        /// First checks for a &lt;references&lt; missing '/' to correct, otherwise:
        /// if the article uses cite references but has no recognised template to display the references, add {{Reflist}} in the appropriate place
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns>The updated article text</returns>
        public static string AddMissingReflist(string articleText)
        {
            if (!IsMissingReferencesDisplay(articleText) || !Variables.LangCode.Equals("en"))
                return articleText;

            if (ReferencesMissingSlash.IsMatch(articleText))
                return ReferencesMissingSlash.Replace(articleText, @"<references/>");

            // Rename ==Links== to ==External links==
            articleText = LinksHeading.Replace(articleText, "$1External links$2");

            // add to any existing references section if present
            if (ReferencesHeadingLevel2.IsMatch(articleText))
                articleText = ReferencesHeadingLevelLower.Replace(articleText, "$1\r\n{{Reflist}}");
            else
            {
                articleText += "\r\n==References==\r\n{{Reflist}}";

                // now sort metadata in case Category at top of article
                Parsers p = new Parsers();
                articleText = p.SortMetaData(articleText, "A", false);

                // try to move just above external links
                if (ExternalLinksHeading.IsMatch(articleText))
                    articleText = ExternalLinksToReferences.Replace(articleText, "$2\r\n$1");
                else if (Category.IsMatch(articleText))
                    // try to move just above categories
                    articleText = CategoryToReferences.Replace(articleText, "$3\r\n$1$2");
                else // not moved, so extra blank line required before heading
                    articleText = articleText.Replace("\r\n==References==", "\r\n\r\n==References==");
            }

            return articleText;
        }

        private static readonly RegexReplacement[] RefSimple = {
            new RegexReplacement(new Regex(@"<\s*(?:\s+ref\s*|\s*ref\s+)>",  RegexOptions.Singleline), "<ref>"),
            // <ref name="Fred" /ref> --> <ref name="Fred"/>
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*""[^<>=""\/]+?"")\s*/\s*(?:ref|/)\s*>",  RegexOptions.Singleline | RegexOptions.IgnoreCase), "$1/>"),

            // <ref name="Fred""> --> <ref name="Fred">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*""[^<>=""\/]+?"")["">]\s*(/?)>",  RegexOptions.Singleline | RegexOptions.IgnoreCase), "$1$2>"),

            // <ref name = ”Fred”> --> <ref name="Fred">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)(?:[“‘”’]+(?<val>[^<>=""\/]+?)[“‘”’]*|[“‘”’]*(?<val>[^<>=""\/]+?)[“‘”’]+)(\s*/?>)",  RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""${val}""$2"),

            // <ref name = ''Fred'> --> <ref name="Fred"> (two apostrophes) or <ref name = 'Fred''> --> <ref name="Fred"> (two apostrophes)
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)(?:''+(?<val>[^<>=""\/]+?)'+|'+(?<val>[^<>=""\/]+?)''+)(\s*/?>)",  RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""${val}""$2"),

            // <ref name=foo bar> --> <ref name="foo bar">, match when spaces
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)([^<>=""'\/]+?\s+[^<>=""'\/\s]+?)(\s*/?>)",  RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""$2""$3"),

            // <ref name=foo bar> --> <ref name="foo bar">, match when non-ASCII characters ([\x00-\xff]*)
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)([^<>=""'\/]*?[^\x00-\xff]+?[^<>=""'\/]*?)(\s*/?>)",  RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""$2""$3"),

            // <ref name=foo bar"> --> <ref name="foo bar"> or <ref name="foo bar> --> <ref name="foo bar">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*)(?:['`”]*(?<val>[^<>=""\/]+?)""|""(?<val>[^<>=""\/]+?)['`”]*)(\s*/?>)",  RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1""${val}""$2"),

            // <ref name "foo bar"> --> <ref name="foo bar">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*)[\+\-]?(\s*""[^<>=""\/]+?""\s*/?>)",  RegexOptions.Singleline | RegexOptions.IgnoreCase), @"$1=$2"),

            // <ref "foo bar"> --> <ref name="foo bar">
            new RegexReplacement(new Regex(@"(<\s*ref\s+)=?\s*(""[^<>=""\/]+?""\s*/?>)",  RegexOptions.Singleline | RegexOptions.IgnoreCase), "$1name=$2"),

            // ref name typos
            new RegexReplacement(new Regex(@"(<\s*ref\s+n)(me\s*=)",  RegexOptions.IgnoreCase), "$1a$2"),
            
            // <ref name="Fred" Smith> --> <ref name="Fred Smith">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*=\s*""[^<>=""\/]+?)""([^<>=""\/]{2,}?)(?<!\s+)(?=\s*/?>)",  RegexOptions.IgnoreCase), @"$1$2"""),
            
            // <ref name-"Fred"> --> <ref name="Fred">
            new RegexReplacement(new Regex(@"(<\s*ref\s+name\s*)-"), "$1="),
            
            // <ref NAME= --> <ref name=
            // <refname= --> <ref name=
            new RegexReplacement(new Regex(@"<\s*ref(?:\s+NAME|name)(\s*=)"), "<ref name$1"),
            
            // empty ref name: <ref name=>
            new RegexReplacement(new Regex(@"<\s*ref\s+name\s*=\s*>"), "<ref>")
        };

        // Matches possibly bad ref tags, but not the most common valid formats
        private static readonly Regex PossiblyBadRefTags = new Regex(@"<\s*[Rr][Ee][Ff][^<>]*>(?<!(?:<ref name *= *[\w0-9\-.]+( ?/)?>|<ref>|<ref name *= *""[^{}""<>]+""( ?/)?>))");
        // <ref>...<ref/> or <ref>...</red> --> <ref>...</ref>
        private static readonly Regex RedRef = new Regex(@"(<\s*ref(?:\s+name\s*=[^<>]*?)?\s*>[^<>""]+?)<\s*(?:/\s*red|ref\s*/)\s*>", RegexOptions.IgnoreCase);
        private static readonly Regex AllTagsSpace = new Regex(@"<[^<>]+> *");

        // Covered by TestFixReferenceTags
        /// <summary>
        /// Various fixes to the formatting of &lt;ref&gt; reference tags including case conversion and trimming excess whitespace
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <returns>The modified article text.</returns>
        public static string FixReferenceTags(string articleText)
        {
            // Performance strategy: get all tags in article, filter down to tags that look like ref tags, ignore valid tags, only apply regexes where relevant tags are found
            List<string> AllTagsList = Tools.DeduplicateList((from Match m in AllTagsSpace.Matches(articleText)
                where m.Value.IndexOf("re", StringComparison.OrdinalIgnoreCase) > 0 
                && !m.Value.Equals("<ref>") && !m.Value.Equals("</ref>") && !m.Value.StartsWith(@"<references") && !m.Value.Contains(" group")
                                                                          select m.Value).ToList());

            // <REF>, </REF> and <Ref> to lowercase ref
            if(AllTagsList.Any(s => Regex.IsMatch(s, @"R[Ee][Ff]|r[Ee]F")))
                articleText = Regex.Replace(articleText, @"(<\s*\/?\s*)(?:R[Ee][Ff]|r[Ee]F)(\s*(?:>|name\s*=))", "$1ref$2");

            // remove any spaces between consecutive references -- WP:REFPUNC
            if(AllTagsList.Any(s => s.EndsWith(" ")))
                articleText = Regex.Replace(articleText, @"(</ref>|<ref\s*name\s*=[^{}<>]+?\s*\/\s*>) +(?=<ref(?:\s*name\s*=[^{}<>]+?\s*\/?\s*)?>)", "$1");

            // ensure a space between a reference and text (reference within a paragraph) -- WP:REFPUNC
            articleText = Regex.Replace(articleText, @"(</ref>|<ref\s*name\s*=[^{}<>]+?\s*\/\s*>)(\w)", "$1 $2");

            // remove spaces between punctuation and references -- WP:REFPUNC
            if(articleText.Contains(" <ref"))
                articleText = Regex.Replace(articleText, @"(?<=[,\.:;]) +(<ref(?:\s*name\s*=[^{}<>]+?\s*\/?\s*)?>)", "$1");

            // empty <ref>...</ref> tags
            articleText = Regex.Replace(articleText, @"<ref>\s*</ref>", "");

            // Trailing spaces at the beginning of a reference, within the reference
            if(AllTagsList.Any(s => s.EndsWith(" ")))
                articleText = Regex.Replace(articleText, @"(<ref[^<>\{\}\/]*>) +", "$1");

            // whitespace cleaning of </ref>
            if(AllTagsList.Any(s => Regex.IsMatch(s, @"<(?:\s*/(?:\s+ref\s*|\s*ref\s+)|\s+/\s*ref\s*)>")))
                articleText = Regex.Replace(articleText, @"<(?:\s*/(?:\s+ref\s*|\s*ref\s+)|\s+/\s*ref\s*)>", "</ref>");

            // trim trailing spaces at the end of a reference, within the reference
            if(articleText.Contains(@" </ref>"))
                articleText = Regex.Replace(articleText, @" +</ref>", "</ref>");

            if(AllTagsList.Any(s => s.StartsWith("<ref/>") || s.StartsWith("</red>")))
                articleText = RedRef.Replace(articleText, "$1</ref>");

            // Chinese do not use spaces to separate sentences
            if (Variables.LangCode.Equals("zh"))
                articleText = Regex.Replace(articleText, @"(</ref>|<ref\s*name\s*=[^{}<>]+?\s*\/\s*>) +", "$1");

            // Performance: apply ref tag fixes only to ref tags that might be invalid
            if(AllTagsList.Any(s => !Regex.IsMatch(s, @"(?:<ref name *= *[\w0-9\-.]+( ?/)?>|<ref name *= *""[^{}""<>]+""( ?/)?>)|</ref>")))
                articleText = PossiblyBadRefTags.Replace(articleText, FixReferenceTagsME);

            return articleText;
        }

        private static string FixReferenceTagsME(Match m)
        {
            string newValue = m.Value;

            foreach (RegexReplacement rr in RefSimple)
                newValue = rr.Regex.Replace(newValue, rr.Replacement);

            return newValue;
        }

        // don't match on 'in the June of 2007', 'on the 11th May 2008' etc. as these won't read well if changed
        private static readonly Regex OfBetweenMonthAndYear = new Regex(@"\b" + WikiRegexes.Months + @" +of +(20\d\d|1[89]\d\d)\b(?<!\b[Tt]he {1,5}\w{3,15} {1,5}of {1,5}(20\d\d|1[89]\d\d))");
        private static readonly Regex OrdinalsInDatesAm = new Regex(@"(?<!\b[1-3]\d +)\b" + WikiRegexes.Months + @" +([0-3]?\d)(?:st|nd|rd|th)\b(?<!\b[Tt]he +\w{3,10} +(?:[0-3]?\d)(?:st|nd|rd|th)\b)(?:( *(?:to|and|.|&.dash;) *[0-3]?\d)(?:st|nd|rd|th)\b)?");
        private static readonly Regex OrdinalsInDatesInt = new Regex(@"(?:\b([0-3]?\d)(?:st|nd|rd|th)( *(?:to|and|.|&.dash;) *))?\b([0-3]?\d)(?:st|nd|rd|th) +" + WikiRegexes.Months + @"\b(?<!\b[Tt]he +(?:[0-3]?\d)(?:st|nd|rd|th) +\w{3,10})");
        private static readonly Regex DateLeadingZerosAm = new Regex(@"(?<!\b[0-3]?\d *)\b" + WikiRegexes.Months + @" +0([1-9])" + @"\b");
        private static readonly Regex DateLeadingZerosInt = new Regex(@"\b" + @"0([1-9]) +" + WikiRegexes.Months + @"\b");
        private static readonly Regex MonthsRegex = new Regex(@"\b" + WikiRegexes.MonthsNoGroup + @"\b.{0,25}");
        private static readonly Regex MonthsRegexNoSecondBreak = new Regex(@"\b" + WikiRegexes.MonthsNoGroup + @".{0,30}");
        private static readonly Regex DayOfMonth = new Regex(@"(?<![Tt]he +)\b([1-9]|[12]\d|3[01])(?:st|nd|rd|th) +of +" + WikiRegexes.Months);
        private static readonly Regex Ordinal = new Regex(@"\d(?:st|nd|rd|th)");
        private static readonly Regex MonthsAct = new Regex(@"\b(?:January|February|March|April|May|June|July|August|September|October|November|December) Act\b");
        //Ordinal number found inside <sup> tags.
        private static readonly Regex SupOrdinal = new Regex(@"(\d)<sup>(st|nd|rd|th)</sup>", RegexOptions.Compiled);
        private static readonly Regex FixDateOrdinalsAndOfQuick = new Regex(@"[0-9](st|nd|rd|th)|\b0[1-9]\b| of +([0-9]|[A-Z])");

        // Covered by TestFixDateOrdinalsAndOf
        /// <summary>
        /// Removes ordinals, leading zeros from dates and 'of' between a month and a year, per [[WP:MOSDATE]]; on en wiki only
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <param name="articleTitle">The article's title</param>
        /// <returns>The modified article text.</returns>
        public string FixDateOrdinalsAndOf(string articleText, string articleTitle)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;
            
            bool monthsInTitle = MonthsRegex.IsMatch(articleTitle);

            for (; ; )
            {
                bool reparse = false;
                // performance: better to loop through all instances of dates and apply regexes to those than
                // to apply regexes to whole article text
                // Secondly: filter down only to those portions that could be changed
                List<Match> monthsm = (from Match m in MonthsRegex.Matches(articleText) select m).Where(m => 
                    FixDateOrdinalsAndOfQuick.IsMatch(articleText.Substring(m.Index-Math.Min(25, m.Index), Math.Min(25, m.Index)+m.Length))).ToList();

                foreach(Match m in monthsm)
                {
                    // take up to 25 characters before match, unless match within first 25 characters of article
                    string before = articleText.Substring(m.Index-Math.Min(25, m.Index), Math.Min(25, m.Index)+m.Length);

                    if(MonthsAct.IsMatch(before))
                        continue;

                    string after = FixDateOrdinalsAndOfLocal(before, monthsInTitle);
                    
                    if(!after.Equals(before))
                    {
                        reparse = true;
                        articleText = articleText.Replace(before, after);

                        // catch after other fixes
                        articleText = IncorrectCommaAmericanDates.Replace(articleText, @"$1 $2, $3");
                        articleText = IncorrectCommaInternationalDates.Replace(articleText, @"$1 $2");

                        break;
                    }
                }
                if (!reparse)
                    break;
            }
            
            return articleText;
        }
        
        private string FixDateOrdinalsAndOfLocal(string textPortion, bool monthsInTitle)
        {
            textPortion = OfBetweenMonthAndYear.Replace(textPortion, "$1 $2");

            // don't apply if article title has a month in it (e.g. [[6th of October City]])
            // ordinals check for performance
            if (!monthsInTitle && Regex.IsMatch(textPortion, @"[0-9](st|nd|rd|th)"))
            {
                textPortion = OrdinalsInDatesAm.Replace(textPortion, "$1 $2$3");
                textPortion = OrdinalsInDatesInt.Replace(textPortion, "$1$2$3 $4");
                textPortion = DayOfMonth.Replace(textPortion, "$1 $2");
            }

            textPortion = DateLeadingZerosAm.Replace(textPortion, "$1 $2");
            return DateLeadingZerosInt.Replace(textPortion, "$1 $2");
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

        /// <summary>
        /// Fixes and improves syntax (such as html markup)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public static string FixSyntax(string articleText, out bool noChange)
        {
            string newText = FixSyntax(articleText);

            noChange = newText.Equals(articleText);
            return newText;
        }

        // regexes for external link match on balanced bracket
        private static readonly Regex DoubleBracketAtStartOfExternalLink = new Regex(@"\[\[+(https?:/(?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex DoubleBracketAtEndOfExternalLink = new Regex(@"(\[ *https?:/(?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\])\](?!\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex TripleBracketAtEndOfExternalLink = new Regex(@"(\[ *https?:/(?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\])\]\](?!\])", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex DoubleBracketAtEndOfExternalLinkWithinImage = new Regex(@"(\[https?:/(?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!)))\](?=\]{3})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ListExternalLinkEndsCurlyBrace = new Regex(@"^(\* *\[https?://[^<>\[\]]+?)\)\s*$", RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex SyntaxRegexWikilinkMissingClosingBracket = new Regex(@"\[\[([^][]*?)\](?=[^\]]*?(?:$|\[|\n))", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexWikilinkMissingOpeningBracket = new Regex(@"(?<=(?:^|\]|\n)[^\[]*?)\[([^][]*?)\]\](?!\])", RegexOptions.Compiled);

        private static readonly Regex SyntaxRegexExternalLinkToImageURL = new Regex("\\[?\\["+Variables.NamespacesCaseInsensitive[Namespace.File]+":(http:\\/\\/.*?)\\]\\]?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ExternalLinksStart = new Regex(@"^\[ *(?:https?|ftp|mailto|irc|gopher|telnet|nntp|worldwind|news|svn)://", RegexOptions.IgnoreCase);

        private static readonly Regex SyntaxRegexListRowBrTag = new Regex(@"^([#\*:;]+.*?) *(?:<[/\\]?br ?[/\\]? ?>)+ *\r\n", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private static readonly Regex SyntaxRegexListRowBrTagMiddle = new Regex(@"^([#\*:;]+.*?)\s*(?:<[/\\]?br ?[/\\]? ?>)+ *\r\n([#\*:;]+)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        private static readonly Regex SyntaxRegexBrNewline = new Regex(@"<[/\\]?[Bb][Rr] ?[/\\]? ?> *\r\n");

        private static readonly Regex SyntaxRegexListRowBrTagStart = new Regex(@"<[/\\]?br ?[/\\]? ?> *(\r\n[#\*:;]+)", RegexOptions.IgnoreCase);

        private static readonly Regex SyntaxRegexItalicBoldEm = new Regex(@"< *(i|em|b) *>(.*?)< */ *\1 *>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Matches <p> tags only if current line does not start from ! or | (indicator of table cells), plus any spaces after
        private static readonly Regex SyntaxRemoveParagraphs = new Regex(@"(?<!^[!\|].*)</? ?[Pp]> *", RegexOptions.Multiline);
        // Match execss <br> tags only if current line does not start from ! or | (indicator of table cells)
        private static readonly Regex SyntaxRemoveBr = new Regex(@"(?:(?:<br[\s/]*> *){2,}|\r\n<br[\s/]*>\r\n<br[\s/]*>\r\n)(?<!^[!\|].*)", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private static readonly Regex MultipleHttpInLink = new Regex(@"(?<=[\s\[>=])(https?(?::?/+|:/*)) *(\1)+", RegexOptions.IgnoreCase);
        private static readonly Regex MultipleFtpInLink = new Regex(@"(?<=[\s\[>=])(ftp(?::?/+|:/*))(\1)+", RegexOptions.IgnoreCase);
        private static readonly Regex PipedExternalLink = new Regex(@"(\[\w+://[^\]\[<>\""\s]*?\s*)(?: +\||\|([ ']))(?=[^\[\]\|]*\])");

        private static readonly Regex MissingColonInHttpLink = new Regex(@"(?<=[\s\[>=](?:ht|f))(tps?)(?://?:?|:(?::+//)?)(\w+)", RegexOptions.Compiled);
        private static readonly Regex SingleTripleSlashInHttpLink = new Regex(@"(?<=[\s\[>=](?:ht|f))(tps?):(?:/|////?)(\w+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex CellpaddingTypo = new Regex(@"({\s*\|\s*class\s*=\s*""wikitable[^}]*?)cel(?:lpa|pad?)ding\b", RegexOptions.IgnoreCase);
        private static readonly Regex CellpaddingTypoQuick = new Regex(@"\bcel(?:lpa|pad?)ding\b", RegexOptions.IgnoreCase);

        //https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_.3Cfont.3E_tags
        private static readonly Regex RemoveNoPropertyFontTags = new Regex(@"<font>([^<>]+)</font>", RegexOptions.IgnoreCase);

        // for fixing unbalanced brackets
        private static readonly Regex RefTemplateIncorrectBracesAtEnd = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>/]+?\s*)?>\s*)({{\s*[Cc]it[ae][^{}]+?)(?:}\]?|\)\))?(?=\s*</ref>)", RegexOptions.Compiled);
        private static readonly Regex RefExternalLinkUsingBraces = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>]+?\s*)?>\s*){{(\s*https?://[^{}\s\r\n]+)(\s+[^{}]+\s*)?}}(\s*</ref>)", RegexOptions.Compiled);
        private static readonly Regex RefURLMissingHttp = new Regex(@"(<ref(?:\s*name\s*=[^{}<>]+?\s*)?>\[?)\s*www\.", RegexOptions.Compiled);
        private static readonly Regex TemplateIncorrectBracesAtStart = new Regex(@"(?:{\[|\[{)([^{}\[\]]+}})", RegexOptions.Compiled);
        private static readonly Regex CitationTemplateSingleBraceAtStart = new Regex(@"(?<=[^{])({\s*[Cc]it[ae])", RegexOptions.Compiled);
        private static readonly Regex ReferenceTemplateQuadBracesAtEnd = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>]+?\s*)?>\s*{{[^{}]+)}}(}}\s*</ref>)", RegexOptions.Compiled);
        private static readonly Regex ReferenceTemplateQuadBracesAtEndQuick = new Regex(@"}}}}\s*</ref>");
        private static readonly Regex CitationTemplateIncorrectBraceAtStart = new Regex(@"(?<=<ref(?:\s*name\s*=[^{}<>]+?\s*)?>){\[([Cc]it[ae])", RegexOptions.Compiled);
        private static readonly Regex CitationTemplateIncorrectBracesAtEnd = new Regex(@"(<ref(?:\s*name\s*=[^{}<>]+?\s*)?>\s*{{[Cc]it[ae][^{}]+?)(?:}\]|\]}|{})(?=\s*</ref>)", RegexOptions.Compiled);
        private static readonly Regex CitationTemplateIncorrectBracesAtEndQuick = new Regex(@"(?:}\]|\]}|{})(?=\s*</ref>)");
        private static readonly Regex RefExternalLinkMissingStartBracket = new Regex(@"(<ref(?:\s*name\s*=[^{}<>]+?\s*)?>[^{}\[\]<>]*?){?((?:ht|f)tps?://[^{}\[\]<>]+\][^{}\[\]<>]*</ref>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RefExternalLinkMissingEndBracket = new Regex(@"(<ref(?:\s*name\s*=[^{}<>]+?\s*)?>[^{}\[\]<>]*?\[\s*(?:ht|f)tps?://[^{}\[\]<>]+)}?(</ref>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex RefCitationMissingOpeningBraces = new Regex(@"(<\s*ref(?:\s+name\s*=[^<>]*?)?\s*>\s*)\(?\(?([Cc]it[ae][^{}]+}}\s*</ref>)", RegexOptions.Compiled);
        private static readonly Regex DeadlinkOutsideRef = new Regex(@"(</ref>) ?(\{\{[Dd]ead ?link\s*\|\s*date\s*=[^{}\|]+\}\})", RegexOptions.Compiled);

        // refs with wording and bare link: combine the two
        private static readonly Regex WordingIntoBareExternalLinks = new Regex(@"(<ref(?:\s*name\s*=[^{}<>]+?\s*)?>\s*)([^<>{}\[\]\r\n]{3,70}?)[\.,::]?\s*\[\s*((?:[Hh]ttps?|[Ff]tp|[Mm]ailto)://[^\ \n\r<>]+)\s*\](?=\s*</ref>)", RegexOptions.Compiled);

        // space needed between word and external link
        private static readonly Regex ExternalLinkWordSpacingBefore = new Regex(@"(?<=\w)(\[(?:https?|ftp|mailto|irc|gopher|telnet|nntp|worldwind|news|svn)://.*?\])", RegexOptions.Compiled);
        private static readonly Regex ExternalLinkWordSpacingAfter = new Regex(@"(\[(?:https?|ftp|mailto|irc|gopher|telnet|nntp|worldwind|news|svn)://[^\]\[<>]*?\])(\w)", RegexOptions.Compiled);

        private static readonly Regex WikilinkEndsBr = new Regex(@"<br[\s/]*>\s*\]\]$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // for correcting square brackets within external links
        private static readonly Regex SquareBracketsInExternalLinks = new Regex(@"(\[https?://(?>[^\[\]<>]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\])", RegexOptions.Compiled);

        // CHECKWIKI error 2: fix incorrect <br> of <br.>, <\br>, <br\> and <br./> etc.
        private static readonly Regex IncorrectBr = new Regex(@"<(\\ *br *| *br *\\ *| *br\. */?| *br */([a-z/0-9•]|br)| *br *\?|/ *br */?)>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        // CHECKWIKI error 2: https://en.wikipedia.org/wiki/Wikipedia:HTML5#Other_obsolete_attributes
        private static readonly Regex IncorrectBr2 = new Regex(@"<br\s*clear\s*=\s*""?(both|all|left|right)""?\s*\/?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex IncorrectClosingHtmlTags = new Regex(@"< */?(center|gallery|small|sub|sup|i) *[\\/] *>");

        private static readonly Regex SyntaxRegexHorizontalRule = new Regex("^(<hr>|-{5,})", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SyntaxRegexHeadingWithHorizontalRule = new Regex("(^==?[^=]*==?)\r\n(\r\n)?----+", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex SyntaxRegexHTTPNumber = new Regex(@"HTTP/\d\.", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexISBN = new Regex(@"(?:ISBN(?:-1[03])?:|\[\[ISBN\]\])\s*(\d)", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexISBN2 = new Regex(@"ISBN-(?!1[03]\b)", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexISBN3 = new Regex(@"\[\[ISBN\]\]\s\[\[Special\:BookSources[^\|]*\|([^\]]*)\]\]", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexPMID = new Regex(@"(PMID): *(\d)", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexExternalLinkOnWholeLine = new Regex(@"^\[(\s*http.*?)\]$", RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex SyntaxRegexClosingBracket = new Regex(@"([^]])\]([^]]|$)", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexOpeningBracket = new Regex(@"([^[]|^)\[([^[])", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexFileWithHTTP = new Regex("\\[\\["+Variables.NamespacesCaseInsensitive[Namespace.File]+":[^]]*http", RegexOptions.Compiled);
        private static readonly Regex SimpleTags = new Regex(@"<[^>""\-=]+>");

        /// <summary>
        /// Matches double piped links e.g. [[foo||bar]] (CHECKWIKI error 32)
        /// </summary>
        private static readonly Regex DoublePipeInWikiLink = new Regex(@"(?<=\[\[[^\[\[\r\n\|{}]+)\|\|(?=[^\[\[\r\n\|{}]+\]\])", RegexOptions.Compiled);

        /// <summary>
        /// Matches empty gallery, center or blockquote tags (zero or more whitespace)
        /// </summary>
        private static readonly Regex EmptyTags = new Regex(@"<\s*([Gg]allery|[Cc]enter|[Bb]lockquote)\s*>\s*<\s*/\s*\1\s*>", RegexOptions.IgnoreCase);

        private static readonly System.Globalization.CultureInfo BritishEnglish = new System.Globalization.CultureInfo("en-GB");

        // Covered by: LinkTests.TestFixSyntax(), incomplete
        /// <summary>
        /// Fixes and improves syntax (such as html markup)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixSyntax(string articleText)
        {
            if (Variables.LangCode.Equals("en"))
            {
	            // DEFAULTSORT whitespace fix - CHECKWIKI error 88
                articleText = WikiRegexes.Defaultsort.Replace(articleText, DefaultsortME);
                // This category should not be directly added
	            articleText = articleText.Replace(@"[[Category:Disambiguation pages]]", @"{{Disambiguation}}");
            }

            articleText = Tools.TemplateToMagicWord(articleText);

            // get a list of all the simple html tags (not with properties) used in the article, so we can selectively apply HTML tag fixes below
            List<string> SimpleTagsList = Tools.DeduplicateList((from Match m in SimpleTags.Matches(articleText)
                                                                          select Regex.Replace(m.Value, @"\s", "").ToLower()).ToList());

			// fix for <sup/>, <sub/>, <center/>, <small/>, <i/> etc.
            if(SimpleTagsList.Any(s => (s.EndsWith("/>") || s.Contains(@"\"))))
                articleText = IncorrectClosingHtmlTags.Replace(articleText,"</$1>");

            // The <strike> tag is not supported in HTML5. - CHECKWIKI error 42
            if(SimpleTagsList.Any(s => s.Contains("strike")))
            {
                articleText = articleText.Replace(@"<strike>", @"<s>");
                articleText = articleText.Replace(@"</strike>", @"</s>");
            }

            // remove empty <gallery>, <center> or <blockquote> tags, allow for nested tags
            if(SimpleTagsList.Any(s => s.Contains("gallery") || s.Contains("center") || s.Contains("blockquote")))
            {
                while(EmptyTags.IsMatch(articleText))
                    articleText = EmptyTags.Replace(articleText, "");
            }

            // merge italic/bold html tags if there are one after the other
            //https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_21#Another_bug_on_italics
            if(SimpleTagsList.Any(s => s.StartsWith("<b")))
                articleText = articleText.Replace("</b><b>", "");
            if(SimpleTagsList.Any(s => s.StartsWith("<i")))
                articleText = articleText.Replace("</i><i>", "");

            //replace html with wiki syntax - CHECKWIKI error 26 and 38
            if(SimpleTagsList.Any(s => Regex.IsMatch(s, @"<(i|em|b)\b")))
            {
                while(SyntaxRegexItalicBoldEm.IsMatch(articleText))
                    articleText = SyntaxRegexItalicBoldEm.Replace(articleText, BoldItalicME);
            }

            if(SimpleTagsList.Any(s => s.StartsWith("<hr")) || articleText.Contains("-----"))
                articleText = SyntaxRegexHorizontalRule.Replace(articleText, "----");

            //remove appearance of double line break
            articleText = SyntaxRegexHeadingWithHorizontalRule.Replace(articleText, "$1");

            // remove unnecessary namespace
            articleText = RemoveTemplateNamespace(articleText);

            if(SyntaxRegexBrNewline.IsMatch(articleText))
            {
                // remove <br> from lists (end of list line) - CHECKWIKI error 54
                articleText = SyntaxRegexListRowBrTag.Replace(articleText, "$1\r\n");

                // remove <br> from the middle of lists
                articleText = SyntaxRegexListRowBrTagMiddle.Replace(articleText, "$1\r\n$2");
            }

			// CHECKWIKI error 93
            articleText = MultipleHttpInLink.Replace(articleText, "$1");
            articleText = MultipleFtpInLink.Replace(articleText, "$1");
            articleText = WikiRegexes.UrlTemplate.Replace(articleText, m => m.Value.Replace("http://http://", "http://"));

            //repair bad external links
            if(articleText.IndexOf(":http", StringComparison.OrdinalIgnoreCase) > -1)
                articleText = SyntaxRegexExternalLinkToImageURL.Replace(articleText, "[$1]");

            if (!SyntaxRegexHTTPNumber.IsMatch(articleText))
            {
                articleText = MissingColonInHttpLink.Replace(articleText, "$1://$2");
                articleText = SingleTripleSlashInHttpLink.Replace(articleText, "$1://$2");
            }

            if(CellpaddingTypoQuick.IsMatch(articleText))
                articleText = CellpaddingTypo.Replace(articleText, "$1cellpadding");

            if(SimpleTagsList.Any(s => s.Contains("font")))
                articleText = RemoveNoPropertyFontTags.Replace(articleText, "$1");

            // {{Category:foo]] or {{Category:foo}}
            articleText = CategoryCurlyBrackets.Replace(articleText, @"[[$1]]");

            // [[Category:foo}}
            articleText = CategoryCurlyBracketsEnd.Replace(articleText, @"[[$1]]");

            // fixes for missing/unbalanced brackets, for performance only run if article has unbalanced templates
            string withouttemplates = WikiRegexes.NestedTemplates.Replace(articleText, "");

            if(withouttemplates.IndexOf("{{", StringComparison.Ordinal) > -1 || withouttemplates.IndexOf("}}", StringComparison.Ordinal) > -1)
            {
                articleText = RefCitationMissingOpeningBraces.Replace(articleText, @"$1{{$2");
                articleText = RefTemplateIncorrectBracesAtEnd.Replace(articleText, @"$1}}");
                articleText = TemplateIncorrectBracesAtStart.Replace(articleText, @"{{$1");
                articleText = CitationTemplateSingleBraceAtStart.Replace(articleText, @"{$1");
                if(ReferenceTemplateQuadBracesAtEndQuick.IsMatch(articleText))
                    articleText = ReferenceTemplateQuadBracesAtEnd.Replace(articleText, @"$1");
                articleText = CitationTemplateIncorrectBraceAtStart.Replace(articleText, @"{{$1");
                if(CitationTemplateIncorrectBracesAtEndQuick.IsMatch(articleText))
                    articleText = CitationTemplateIncorrectBracesAtEnd.Replace(articleText, @"$1}}");
            }

            articleText = RefExternalLinkUsingBraces.Replace(articleText, @"[$1$2]$3");

            string nobrackets = SingleSquareBrackets.Replace(articleText, "");
            bool orphanedSingleBrackets = (nobrackets.Contains("[") || nobrackets.Contains("]"));

            if(orphanedSingleBrackets)
            {
                articleText = RefExternalLinkMissingStartBracket.Replace(articleText, @"$1[$2");
                articleText = RefExternalLinkMissingEndBracket.Replace(articleText, @"$1]$2");
            }

            // adds missing http:// to bare url references lacking it - CHECKWIKI error 62
            articleText = RefURLMissingHttp.Replace(articleText,@"$1http://www.");

            // fixes for external links: internal square brackets, newlines or pipes - Partially CHECKWIKI error 80
            // Performance: filter down to matches with likely external link (contains //) and has pipe, newline or internal square brackets
            List<Match> ssb = (from Match m in SingleSquareBrackets.Matches(articleText) select m).ToList();
            List<Match> ssbExternalLink = ssb.Where(m => m.Value.Contains("//") && (m.Value.Contains("|") || m.Value.Contains("\r\n") || m.Value.Substring(3).Contains("[") || m.Value.Trim(']').Contains("]"))).ToList();

            foreach(Match m in ssbExternalLink)
            {
               string newvalue = m.Value;

                if(newvalue.Contains("\r\n") && !newvalue.Substring(1).Contains("[") && ExternalLinksStart.IsMatch(newvalue))
                   newvalue = newvalue.Replace("\r\n", " ");
               
                newvalue = SquareBracketsInExternalLinks.Replace(newvalue, SquareBracketsInExternalLinksME);

                newvalue = PipedExternalLink.Replace(newvalue, "$1 $2");

                if (!m.Value.Equals(newvalue))
                    articleText = articleText.Replace(m.Value, newvalue);
            };

            // needs to be applied after SquareBracketsInExternalLinks
            if(orphanedSingleBrackets && !SyntaxRegexFileWithHTTP.IsMatch(articleText))
            {
                articleText = SyntaxRegexWikilinkMissingClosingBracket.Replace(articleText, "[[$1]]");
                articleText = SyntaxRegexWikilinkMissingOpeningBracket.Replace(articleText, "[[$1]]");
            }

            //  CHECKWIKI error 69
            bool isbnDash = articleText.Contains("ISBN-");
            if(isbnDash || articleText.Contains("ISBN:") || ssb.Any(m => m.Value.Equals("[[ISBN]]")))
                articleText = SyntaxRegexISBN.Replace(articleText, "ISBN $1");

            if(isbnDash)
                articleText = SyntaxRegexISBN2.Replace(articleText, "ISBN ");
            
            if(ssb.Any(m => m.Value.Equals("[[ISBN]]")))
                articleText = SyntaxRegexISBN3.Replace(articleText, "ISBN $1");

            if(articleText.Contains("PMID:"))
                articleText = SyntaxRegexPMID.Replace(articleText, "$1 $2");

            // Remove sup tags from ordinals per [[WP:ORDINAL]].
            // CHECKWIKI error 101
            if(SimpleTagsList.Any(s => s.Contains("sup")))
                articleText = SupOrdinal.Replace(articleText, @"$1$2");

            //CHECKWIKI error 86
            bool DoubleBracketHTTP = articleText.IndexOf("[[http", StringComparison.OrdinalIgnoreCase) > -1;
            if(DoubleBracketHTTP)
                articleText = DoubleBracketAtStartOfExternalLink.Replace(articleText, "[$1");

            // if there are some unbalanced brackets, see whether we can fix them
            articleText = FixUnbalancedBrackets(articleText);

            //fix uneven bracketing on links
            if(DoubleBracketHTTP)
                articleText = DoubleBracketAtStartOfExternalLink.Replace(articleText, "[$1");

            nobrackets = SingleSquareBrackets.Replace(articleText, "");
            if(nobrackets.Contains("[") || nobrackets.Contains("]"))
            {
                articleText = DoubleBracketAtEndOfExternalLink.Replace(articleText, "$1");
                articleText = DoubleBracketAtEndOfExternalLinkWithinImage.Replace(articleText, "$1");
            
                articleText = ListExternalLinkEndsCurlyBrace.Replace(articleText, "$1]");
            }            

            // double piped links e.g. [[foo||bar]] - CHECKWIKI error 32
            if(ssb.Any(s => s.Value.Contains("||")))
                articleText = DoublePipeInWikiLink.Replace(articleText, "|");

            // https://en.wikipedia.org/wiki/Wikipedia:WikiProject_Check_Wikipedia#Article_with_false_.3Cbr.2F.3E_.28AutoEd.29
            // fix incorrect <br> of <br.>, <\br> and <br\> - CHECKWIKI error 02
            if(SimpleTagsList.Any(s => (s.Contains("br") && !s.Equals("<br>") && !s.Equals("<br/>"))))
                articleText = IncorrectBr.Replace(articleText, "<br />");

            articleText = IncorrectBr2.Replace(articleText, m=>
                                                {
                                                    if(m.Groups[1].Value == "left")
                                                        return "{{clear|left}}";
                                                    else if(m.Groups[1].Value == "right")
                                                        return "{{clear|right}}";

                                                    return "{{clear}}";
                                                }
                                                );
            
            // CHECKWIKI errors 55, 63, 66, 77
            if(SimpleTagsList.Any(s => s.Contains("small")))
                articleText = FixSmallTags(articleText);

            articleText = WordingIntoBareExternalLinks.Replace(articleText, @"$1[$3 $2]");
            
            articleText = DeadlinkOutsideRef.Replace(articleText,@" $2$1");

            if (!Variables.LangCode.Equals("zh"))
            {
                articleText = ExternalLinkWordSpacingBefore.Replace(articleText, " $1");
                articleText = ExternalLinkWordSpacingAfter.Replace(articleText, "$1 $2");
            }

            // CHECKWIKI error 65: Image description ends with break – https://tools.wmflabs.org/checkwiki/cgi-bin/checkwiki.cgi?project=enwiki&view=only&id=65
            articleText = WikiRegexes.FileNamespaceLink.Replace(articleText, m=> WikilinkEndsBr.Replace(m.Value, @"]]"));

            // workaround for https://phabricator.wikimedia.org/T4700 -- {{subst:}} doesn't work within ref tags
            articleText = FixSyntaxSubstRefTags(articleText);

            // ensure magic word behaviour switches such as __TOC__ are in upper case
            if(articleText.IndexOf("__", StringComparison.Ordinal) > -1)
                articleText = WikiRegexes.MagicWordsBehaviourSwitches.Replace(articleText, m=> @"__" + m.Groups[1].Value.ToUpper() + @"__");

            return articleText.Trim();
        }

        /// <summary>
        /// Trims whitespace around DEFAULTSORT value, ensures 'whitepace only' DEFAULTSORT left unchanged, removes trailing square brackets
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static string DefaultsortME(Match m)
        {
            string returned = @"{{DEFAULTSORT:", key = m.Groups["key"].Value;

            // avoid changing a defaultsort key value that is only whitespace: wrong before, would still be wrong after
            if (key.Trim().Length == 0)
                return m.Value;

            returned += (key.Trim().TrimEnd("[]".ToCharArray()).Trim() + @"}}");

            // handle case where defaultsort ended by newline, preserve newline at end of defaultort returned
            string end = m.Groups["end"].Value;

            if (!end.TrimStart().Equals(@"}}"))
                returned += end;

            return returned;
        }

        /// <summary>
        /// Replaces with three apostrophes (''') if &lt;B> or &lt;b> tag, else just two ('')
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static string BoldItalicME(Match m)
        {
            string ret = (m.Groups[1].Value.Equals("b", StringComparison.OrdinalIgnoreCase) ? "'''" : "''");
            return ret + m.Groups[2].Value +ret;
        }
        
        /// <summary>
        /// Fixes bracket problems within external links, converting internal [ or ] to &#91; or &#93; respectively
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static string SquareBracketsInExternalLinksME(Match m)
        {
            // strip off leading [ and trailing ]
            string externalLink = SyntaxRegexExternalLinkOnWholeLine.Replace(m.Value, "$1");

            // if there are some brackets left then they need fixing; the mediawiki parser finishes the external link at the first ] found
            if (!WikiRegexes.Newline.IsMatch(externalLink) && (externalLink.Contains("]") || externalLink.Contains("[")))
            {
                // replace single ] with &#93; when used for brackets in the link description
                if (externalLink.Contains("]"))
                    externalLink = SyntaxRegexClosingBracket.Replace(externalLink, @"$1&#93;$2");

                if (externalLink.Contains("["))
                    externalLink = SyntaxRegexOpeningBracket.Replace(externalLink, @"$1&#91;$2");
            }
            return (@"[" + externalLink + @"]");
        }

        /// <summary>
        /// Performs fixes to redirect pages:
        /// * removes newline between #REDIRECT and link (CHECKWIKI error 36)
        /// * removes equal sing and double dot between #REDIRECT and link (CHECKWIKI error 36)
        /// * Template to Magic word conversion; removes unnecessary brackets around redirect
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public static string FixSyntaxRedirects(string articleText)
        {
            articleText = WikiRegexes.Redirect.Replace(articleText, m => {
                                                                      string res = m.Value.Replace("\r\n", " ");
                                                                      res = res.Replace("[[[[", "[[");
                                                                      res = res.Replace("]]]]", "]]");
                                                                      res = res.Replace("[[[", "[[");
                                                                      res = res.Replace("]]]", "]]");
                                                                      res = res.Replace("= [[", " [[");
                                                                      res = res.Replace("=[[", " [[");
                                                                      res = res.Replace(": [[", " [[");
                                                                      return res.Replace(":[[", " [[");

        	                                            });

            articleText = Tools.TemplateToMagicWord(articleText);

        	return RemoveTemplateNamespace(articleText);
        }

        /// <summary>
        /// workaround for https://phabricator.wikimedia.org/T4700 -- {{subst:}} doesn't work within ref tags
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        public static string FixSyntaxSubstRefTags(string articleText)
        {
            if (Variables.LangCode.Equals("en") && articleText.Contains(@"{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}"))
            {
                articleText = WikiRegexes.Refs.Replace(articleText, FixSyntaxSubstRefTagsME);

                articleText = WikiRegexes.Images.Replace(articleText, FixSyntaxSubstRefTagsME);

                articleText = WikiRegexes.GalleryTag.Replace(articleText, FixSyntaxSubstRefTagsME);
            }

            return articleText;
        }
        
        private static string FixSyntaxSubstRefTagsME(Match m)
        {
            return m.Value.Replace(@"{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}", DateTime.UtcNow.ToString("MMMM yyyy", BritishEnglish));
        }

        /// <summary>
        /// Removes Template: (or equivalent translation) from start of template calls, canonicalizes template names
        /// </summary>
        /// <param name="articleText">The wiki article text</param>
        /// <returns>The updated article text</returns>
        public static string RemoveTemplateNamespace(string articleText)
        {
            Regex SyntaxRegexTemplate = new Regex(@"(\{\{\s*)" + Variables.NamespacesCaseInsensitive[Namespace.Template] + @"([^\|]*?)(\s*(?:\}\}|\|))", RegexOptions.Singleline);
            
            return (SyntaxRegexTemplate.Replace(articleText, m => m.Groups[1].Value + CanonicalizeTitle(m.Groups[2].Value) + m.Groups[3].Value));
        }

        private static readonly List<Regex> SmallTagRegexes = new List<Regex>();
        private static readonly Regex LegendTemplate = Tools.NestedTemplateRegex("legend");

        /// <summary>
        /// remove &lt;small> in small, ref, sup, sub tags and images, but not within {{legend}} template
        /// CHECKWIKI errors 55, 63, 66, 77
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>The updated article text</returns>
        private static string FixSmallTags(string articleText)
        {
            Match sm = WikiRegexes.Small.Match(articleText);

            // Performance: restrict changes to portion of article text containing small tags
            if(sm.Success)
            {
                int cutoff = Math.Max(0, sm.Index-999); // if <ref><small> then must allow offset before <small> tag
                string beforesmall = articleText.Substring(0, cutoff);
                articleText = articleText.Substring(cutoff);

                // don't apply if there are uncosed tags
                if (UnclosedTags(articleText).Count == 0)
                {
                    foreach(Regex rx in SmallTagRegexes)
                    {
                        articleText = rx.Replace(articleText, FixSmallTagsME);
                    }

                    // fixes for small tags surrounding ref/sup/sub tags
                    articleText = WikiRegexes.Small.Replace(articleText, FixSmallTagsME2);
                }

                return beforesmall + articleText;
            }

            return articleText;
        }
        
        private static readonly Regex TableEnd = new Regex(@"^\|}", RegexOptions.Multiline);

        private static string FixSmallTagsME(Match m)
        {
            // don't remove <small> tags from within {{legend}} where use is not unreasonable
            if (!LegendTemplate.IsMatch(m.Value) && !TableEnd.IsMatch(m.Value))
            {
                Match s = WikiRegexes.Small.Match(m.Value);
                if (s.Success)
                {
                    if (s.Index > 0)
                        return WikiRegexes.Small.Replace(m.Value, "$1");
                    // nested small
                    return m.Value.Substring(0, 7) + WikiRegexes.Small.Replace(m.Value.Substring(7), "$1");
                }
            }
            return m.Value;
        }

        private static string FixSmallTagsME2(Match m)
        {
            string smallContent = m.Groups[1].Value.Trim();

            if(!smallContent.Contains("<"))
                return m.Value;

            if(WikiRegexes.Refs.Match(smallContent).Value.Equals(smallContent) || WikiRegexes.SupSub.Match(smallContent).Value.Equals(smallContent))
                return smallContent;

            return m.Value;
        }

        private static readonly Regex CurlyBraceInsteadOfPipeInWikiLink = new Regex(@"(?<=\[\[[^\[\]{}<>\r\n\|]{1,50})}(?=[^\[\]{}<>\r\n\|]{1,50}\]\])", RegexOptions.Compiled);
        private static readonly Regex CurlyBraceInsteadOfBracketClosing = new Regex(@"(\([^{}<>\(\)]+[^{}<>\(\)\|])}(?=[^{}])", RegexOptions.Compiled);
        private static readonly Regex CurlyBraceInsteadOfSquareBracket = new Regex(@"(?<=\[[^{}<>\[\]]+[^{}<>\(\)\|\]])}(?=[^{}])", RegexOptions.Compiled);
        private static readonly Regex CurlyBraceInsteadOfBracketOpening = new Regex(@"(?<=[^{}<>]){(?=[^{}<>\(\)\|][^{}<>\(\)]+\)[^{}\(\)])", RegexOptions.Compiled);
        private static readonly Regex CurlyBraceInsteadOfBracketOpening2 = new Regex(@"(?<=\[)\[?{(?=[^{}\[\]<>]+\]\])");
        private static readonly Regex ExtraBracketOnWikilinkOpening = new Regex(@"(?<=[^\[\]{}<>])(?:{\[\[?|\[\[\[)(?=[^\[\]{}<>]+\]\])", RegexOptions.Compiled);
        private static readonly Regex ExtraBracketOnWikilinkOpening2 = new Regex(@"(?<=\[\[){(?=[^{}\[\]<>]+\]\])", RegexOptions.Compiled);
        private static readonly Regex ExternalLinkMissingClosing = new Regex(@"(^ *\* *\[ *(?:ht|f)tps?://[^<>{}\[\]\r\n\s]+[^\[\]\r\n]*)(\s$)", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex ExternalLinkMissingOpening = new Regex(@"(?<=^ *\*) *(?=(?:ht|f)tps?://[^<>{}\[\]\r\n\s]+[^\[\]\r\n]*\]\s$)", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex TemplateIncorrectClosingBraces = new Regex(@"(?<={{[^{}<>]{1,400}[^{}<>\|\]])(?:\]}|}\]?|\)\))(?=[^{}]|$)", RegexOptions.Compiled);
        private static readonly Regex TemplateMissingOpeningBrace = new Regex(@"(?<=[^{}<>\|]){(?=[^{}<>]{1,400}}})", RegexOptions.Compiled);
        private static readonly Regex PersondataPODToDEFAULTSORT = new Regex(@"(\|\s*PLACE OF DEATH\s*=\s*[^{}]+?)(\s*{{DEFAULTSORT)", RegexOptions.IgnoreCase);

        private static readonly Regex QuadrupleCurlyBrackets = new Regex(@"(?<=^{{[^{}\r\n]+}})}}(\s)$", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex WikiLinkOpeningClosing = new Regex(@"\[(?:\]| +\[)([^\[\]\r\n]+\]\])", RegexOptions.Compiled);
        private static readonly Regex WikiLinkPunctuation = new Regex(@"(\[\[[^\[\]\r\n]+\])([,.;:""' ]{1,2})(\])");
        private static readonly Regex WikiLinkDoubleOpening = new Regex(@"(\[\[[^\[\]\r\n:]{1,55})\[\[");
        private static readonly Regex UnclosedCatInterwiki = new Regex(@"^(\[\[[^\[\]\r\n]+(?<!File|Image|Media)\:[^\[\]\r\n]+)(\s*)$", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex RefClosingOpeningBracket = new Regex(@"\[(\s*</ref>)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CategoryCurlyBrackets = new Regex(@"{{ *(" + Variables.Namespaces[Namespace.Category] + @"[^{}\[\]]+?)(?:}}|\]\])", RegexOptions.Compiled);
        private static readonly Regex CategoryCurlyBracketsEnd = new Regex(@"\[\[ *(" + Variables.Namespaces[Namespace.Category] + @"[^{}\[\]]+?)(?:}})", RegexOptions.Compiled);
        private static readonly Regex FileImageCurlyBrackets = new Regex(@"{{\s*("+Variables.NamespacesCaseInsensitive[Namespace.File]+@"\s*)", RegexOptions.Compiled);
        private static readonly Regex CiteRefEndsTripleClosingBrace = new Regex(@"([^}])\}(\}\}\s*</ref>)", RegexOptions.Compiled);
        private static readonly Regex CiteRefEndsTripleOpeningBrace = new Regex(@"(>\s*)\{\{\{+(\s*[Cc]ite)", RegexOptions.Compiled);
        private static readonly Regex RefExternalLinkWrongBracket = new Regex(@"(<ref[^<>/]*>)\]", RegexOptions.Compiled);
        private static readonly Regex CurlyToStraightSingleBracket = new Regex(@"([^{}()]){([^{}()]+)\)");

        /// <summary>
        /// Applies some fixes for unbalanced brackets, applied if there are unbalanced brackets
        /// </summary>
        /// <param name="articleText">the article text</param>
        /// <returns>the corrected article text</returns>
        private static string FixUnbalancedBrackets(string articleText)
        {
            string[] sections = Tools.SplitToSections(articleText);
            StringBuilder articleTextReturned = new StringBuilder();

            foreach(string s in sections)
            {
                articleTextReturned.Append(FixUnbalancedBracketsSection(s));
            }

            return articleTextReturned.ToString();
        }

        /// <summary>
        /// Applies some fixes for unbalanced brackets, applied if there are unbalanced brackets
        /// Run at section level: allows unbalanced brackets in other sections not to affect correction of current section
        /// </summary>
        /// <param name="articleText">the article text</param>
        /// <returns>the corrected article text</returns>
        private static string FixUnbalancedBracketsSection(string articleText)
        {
            // if there are some unbalanced brackets, see whether we can fix them
            // the fixes applied might damage correct wiki syntax, hence are only applied if there are unbalanced brackets
            // of the right type
            int bracketLength;
            string articleTextTemp = articleText;
            int unbalancedBracket = UnbalancedBrackets(articleText, out bracketLength);
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
                            articleTextTemp = CurlyBraceInsteadOfBracketClosing.Replace(articleTextTemp, "$1)");

                            // if it's [blah} then see if setting the } to a ] makes it all balance
                            articleTextTemp = CurlyBraceInsteadOfSquareBracket.Replace(articleTextTemp, "]");

                            // if it's }}}</ref>
                            articleTextTemp = CiteRefEndsTripleClosingBrace.Replace(articleTextTemp, "$1$2");

                            break;

                        case '{':
                            // if it's {blah) then see if setting the { to a ( makes it all balance, but not {| which could be wikitables
                            articleTextTemp = CurlyBraceInsteadOfBracketOpening.Replace(articleTextTemp, "(");

                            // could be [[{link]]
                            articleTextTemp = ExtraBracketOnWikilinkOpening2.Replace(articleTextTemp, "");

                            // if it's <ref>{{{
                            articleTextTemp = CiteRefEndsTripleOpeningBrace.Replace(articleTextTemp, "$1{{$2");                            break;

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
                            articleTextTemp = ExternalLinkMissingClosing.Replace(articleTextTemp, "$1]$2");

                            // ref with closing [ in error
                            articleTextTemp = RefClosingOpeningBracket.Replace(articleTextTemp, "]$1");

                            break;

                        case ']':
                            // external link missing opening [
                            articleTextTemp = ExternalLinkMissingOpening.Replace(articleTextTemp, " [");

                            // <ref>http://...
                            articleTextTemp = RefExternalLinkWrongBracket.Replace(articleTextTemp, "$1[");

                            break;

                        case '>':
                            // <ref>>
                            articleTextTemp = articleTextTemp.Replace(@"<ref>>", @"<ref>");
                            break;

                        default:
                            // Chinese language brackets（ and ）[ASCII 65288 and 65289], change if unbalanced
                            if(Variables.LangCode.Equals("en") && Regex.Matches(articleTextTemp, "（").Count
                               != Regex.Matches(articleTextTemp, "）").Count)
                            {
                                articleTextTemp = articleTextTemp.Replace("）", ")");
                                articleTextTemp = articleTextTemp.Replace("（", "(");
                            }
                            break;
                    }

                    // if it's {[link]] or {[[link]] or [[[link]] then see if setting to [[ makes it all balance
                    if (!bracket.Equals('>'))
                        articleTextTemp = ExtraBracketOnWikilinkOpening.Replace(articleTextTemp, "[[");
                }

                if (bracketLength == 2)
                {
                    // persondata
                    if(articleTextTemp.Contains("{{Persondata") && !WikiRegexes.Persondata.IsMatch(articleTextTemp))
                        articleTextTemp = PersondataPODToDEFAULTSORT.Replace(articleTextTemp, @"$1}}$2");

                    articleTextTemp = CurlyBraceInsteadOfBracketClosing.Replace(articleTextTemp, "$1)");
                    
                    // if it's on double curly brackets, see if one is missing e.g. {{foo} or {{foo]}
                    articleTextTemp = TemplateIncorrectClosingBraces.Replace(articleTextTemp, "}}");

                    // {{foo|par=[{Bar]]}}
                    articleTextTemp = CurlyBraceInsteadOfBracketOpening2.Replace(articleTextTemp, @"[");

                    // {foo}}
                    articleTextTemp = TemplateMissingOpeningBrace.Replace(articleTextTemp, "{{");

                    string unbalancedStartBrackets = articleTextTemp.Substring(unbalancedBracket, Math.Min(4, articleTextTemp.Length - unbalancedBracket));
                    string unbalancedEndBrackets = articleTextTemp.Substring(Math.Max(0, unbalancedBracket - 2), Math.Min(4, articleTextTemp.Length - unbalancedBracket+2));
                    // might be [[[[link]] or [[link]]]] so see if removing the two found square brackets makes it all balance
                    if (unbalancedStartBrackets.Equals("[[[[") || unbalancedEndBrackets.Equals("]]]]"))
                    {
                        articleTextTemp = articleTextTemp.Remove(unbalancedBracket, 2);
                    }

                    // wikilink like []foo]]
                    articleTextTemp = WikiLinkOpeningClosing.Replace(articleTextTemp, @"[[$1");

                    articleTextTemp = QuadrupleCurlyBrackets.Replace(articleTextTemp, "$1");
                    
                    // wikilink like [[foo[[
                    articleTextTemp = WikiLinkDoubleOpening.Replace(articleTextTemp, @"$1]]");

                    // unclosed cat/interwiki
                    articleTextTemp = UnclosedCatInterwiki.Replace(articleTextTemp, @"$1]]$2");

                    // {{File: --> [[File:
                    articleTextTemp = FileImageCurlyBrackets.Replace(articleTextTemp, @"[[$1");

                    // {...)
                    articleTextTemp = CurlyToStraightSingleBracket.Replace(articleTextTemp, @"$1($2)");

                    // wikilink like [[foo],]
                    articleTextTemp = WikiLinkPunctuation.Replace(articleTextTemp, "$1$3$2");

                    // external link excess closing braces
                    articleTextTemp = TripleBracketAtEndOfExternalLink.Replace(articleTextTemp, "$1");
                }

                unbalancedBracket = UnbalancedBrackets(articleTextTemp, out bracketLength);
                // the change worked if unbalanced bracket location moved considerably (so this one fixed), or all brackets now balance
                if (unbalancedBracket < 0
                    || (Math.Abs(unbalancedBracket - firstUnbalancedBracket) > 300) &&
                    unbalancedBracket - firstUnbalancedBracket > -300)
                    articleText = articleTextTemp;
            }

            return articleText;
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

        private static readonly Regex CiteUrl = new Regex(@"\|\s*url\s*=\s*([^\[\]<>""\s]+)");

        private static readonly Regex WorkInItalics = new Regex(@"(\|\s*work\s*=\s*)''([^'{}\|]+)''(?=\s*(?:\||}}))");

        private static readonly Regex CiteTemplatePagesPP = new Regex(@"(?<=\|\s*pages?\s*=\s*)p(?:p|gs?)?(?:\.|\b)(?:&nbsp;|\s*)(?=[^{}\|]+(?:\||}}))");
        private static readonly Regex CiteTemplatesJournalVolume = new Regex(@"(?<=\|\s*volume\s*=\s*)vol(?:umes?|\.)?(?:&nbsp;|:)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CiteTemplatesJournalVolumeAndIssue = new Regex(@"(?<=\|\s*volume\s*=\s*[0-9VXMILC]+?)(?:[;,]?\s*(?:nos?[\.:;]?|(?:numbers?|issues?|iss)\s*[:;]?))", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CiteTemplatesJournalIssue = new Regex(@"(?<=\|\s*issue\s*=\s*)(?:issues?|(?:nos?|iss)(?:[\.,;:]|\b)|numbers?[\.,;:]?)(?:&nbsp;)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CiteTemplatesPageRangeName = new Regex(@"(\|\s*)page(\s*=\s*\d+\s*(?:–|, )\s*\d)");

        private static readonly Regex AccessDateYear = new Regex(@"(?<=\|\s*access\-?date\s*=\s*(?:[1-3]?\d\s+" + WikiRegexes.MonthsNoGroup + @"|\s*" + WikiRegexes.MonthsNoGroup + @"\s+[1-3]?\d))(\s*)\|\s*accessyear\s*=\s*(20[01]\d)\s*(\||}})");
        private static readonly Regex AccessDayMonthDay = new Regex(@"\|\s*access(?:daymonth|month(?:day)?|year)\s*=\s*(?=\||}})");
        private static readonly Regex DateLeadingZero = new Regex(@"(?<=\|\s*(?:access|archive)?\-?date\s*=\s*)(?:0([1-9]\s+" + WikiRegexes.MonthsNoGroup + @")|(\s*" + WikiRegexes.MonthsNoGroup + @"\s)+0([1-9],?))(\s+(?:20[01]|1[89]\d)\d)?(\s*(?:\||}}))");

        private static readonly Regex LangTemplate = new Regex(@"(\|\s*language\s*=\s*)({{(\w{2}) icon}}\s*)(?=\||}})");

        private static readonly Regex UnspacedCommaPageRange = new Regex(@"((?:[ ,–]|^)\d+),(\d+(?:[ ,–]|$))");

        private static readonly List<string> ParametersToDequote = new List<string>(new[] { "title", "trans_title" });

        /// <summary>
        /// Applies various formatting fixes to citation templates
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The updated wiki text</returns>
        public static string FixCitationTemplates(string articleText)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            articleText = WikiRegexes.CiteTemplate.Replace(articleText, FixCitationTemplatesME);

            articleText = WikiRegexes.HarvTemplate.Replace(articleText, m =>
                                                           {
                                                               string newValue = FixPageRanges(m.Value, Tools.GetTemplateParameterValues(m.Value));
                                                               string page = Tools.GetTemplateParameterValue(newValue, "p");

                                                               // ignore brackets
                                                               if (page.Contains(@"("))
                                                                   page = page.Substring(0, page.IndexOf(@"("));

                                                               if (Regex.IsMatch(page, @"\d+\s*(?:–|&ndash;|, )\s*\d") && Tools.GetTemplateParameterValue(newValue, "pp").Length == 0)
                                                                   newValue = Tools.RenameTemplateParameter(newValue, "p", "pp");

                                                               return newValue;
                                                           });

            articleText = Tools.NestedTemplateRegex("rp").Replace(articleText, m =>
                                                                  {
                                                                      string pagerange = Tools.GetTemplateArgument(m.Value, 1);
                                                                      if(pagerange.Length > 0)
                                                                          return m.Value.Replace(pagerange, FixPageRangesValue(pagerange));

                                                                      return m.Value;
                                                                  });

            return articleText;
        }

        private static readonly Regex IdISBN = new Regex(@"^ISBN:?\s*([\d \-]+X?)$", RegexOptions.Compiled);
        private static readonly Regex IdASIN = new Regex(@"^ASIN:?\s*([\d \-]+X?)$", RegexOptions.Compiled);
        private static readonly Regex YearOnly = new Regex(@"^[12]\d{3}$", RegexOptions.Compiled);
        private static readonly Regex ISBNDash = new Regex(@"(\d)[–](\d|X$)");
        private static readonly Regex BalancedArrows = new Regex(@"(?:«([^»]+)»|‹([^›]+)›)");

        /// <summary>
        /// Performs fixes to a given citation template call
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static string FixCitationTemplatesME(Match m)
        {
            string newValue = Tools.RemoveExcessTemplatePipes(m.Value);
            string templatename = m.Groups[2].Value;
            
            Dictionary<string, string> paramsFound = new Dictionary<string, string>();
            // remove duplicated fields, ensure the URL is not touched (may have pipes in)
            newValue = Tools.RemoveDuplicateTemplateParameters(newValue, paramsFound);

            // fix cite params not in lower case, allowing for ISBN, DOI identifiers being uppercase, avoiding changing text within malformatted URL
            foreach(string notlowercaseCiteParam in paramsFound.Keys.ToList().Where(p => (p.ToLower() != p) && !Regex.IsMatch(p, @"(?:IS[BS]N|DOI|PMID|OCLC|PMC|LCCN|ASIN|ARXIV|ASIN\-TLD|BIBCODE|ID|ISBN13|JFM|JSTOR|MR|OL|OSTI|RFC|SSRN|URL|ZBL)")
                && !CiteUrl.Match(newValue).Value.Contains(p)).ToList())
                newValue = Tools.RenameTemplateParameter(newValue, notlowercaseCiteParam, notlowercaseCiteParam.ToLower());

            string theURL,
                id,
                format,
                theTitle,
                TheYear,
                lang,
                TheDate,
                TheMonth,
                TheWork,
                nopp,
                TheIssue,
                accessyear,
                accessdate,
                pages,
                page,
                ISBN,
                origyear,
                origdate,
                archiveurl,
                contributionurl;
            if(!paramsFound.TryGetValue("url", out theURL))
                theURL = "";
            if(!paramsFound.TryGetValue("id", out id))
                id = "";
            if(!paramsFound.TryGetValue("format", out format))
                format = "";
            if(!paramsFound.TryGetValue("title", out theTitle))
                theTitle = "";
            if(!paramsFound.TryGetValue("year", out TheYear))
                TheYear = "";
            if(!paramsFound.TryGetValue("date", out TheDate))
                TheDate = "";
            if(!paramsFound.TryGetValue("language", out lang))
                lang = "";
            if(!paramsFound.TryGetValue("month", out TheMonth))
                TheMonth = "";
            if(!paramsFound.TryGetValue("work", out TheWork))
                TheWork = "";
            if(!paramsFound.TryGetValue("nopp", out nopp))
                nopp = "";
            if(!paramsFound.TryGetValue("issue", out TheIssue))
                TheIssue = "";
            if(!paramsFound.TryGetValue("accessyear", out accessyear))
                accessyear = "";
            if(!paramsFound.TryGetValue("accessdate", out accessdate) && !paramsFound.TryGetValue("access-date", out accessdate))
                accessdate = "";
            if(!paramsFound.TryGetValue("pages", out pages))
                pages = "";
            if(!paramsFound.TryGetValue("page", out page))
                page = "";
            if(!paramsFound.TryGetValue("origyear", out origyear))
                origyear = "";
            if(!paramsFound.TryGetValue("origdate", out origdate))
                origdate = "";
            if(!paramsFound.TryGetValue("archiveurl", out archiveurl))
                archiveurl = "";
            if(!paramsFound.TryGetValue("contribution-url", out contributionurl))
                contributionurl = "";
            if(!paramsFound.TryGetValue("isbn", out ISBN) && !paramsFound.TryGetValue("ISBN", out ISBN))
                ISBN = "";

            string theURLoriginal = theURL;

            // remove the unneeded 'format=HTML' field
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Remove_.22format.3DHTML.22_in_citation_templates
            // remove format= field with null value when URL is HTML page
            if(paramsFound.ContainsKey("format"))
            {
                if (format.TrimStart("[]".ToCharArray()).ToUpper().StartsWith("HTM")
                    ||
                    (format.Length == 0 &&
                     theURL.ToUpper().TrimEnd('L').EndsWith("HTM")))
                    newValue = Tools.RemoveTemplateParameter(newValue, "format");
            }

            if(paramsFound.ContainsKey("origdate") && origdate.Length == 0)
            {
                newValue = Tools.RemoveTemplateParameter(newValue, "origdate");
            }

            // newlines to spaces in title field if URL used, otherwise display broken
            if (theTitle.Contains("\r\n") && theURL.Length > 0)
            {
                theTitle = theTitle.Replace("\r\n", " ");
                paramsFound.Remove("title");
                paramsFound.Add("title", theTitle);
                newValue = Tools.UpdateTemplateParameterValue(newValue, "title", theTitle);
            }

            // {{sv icon}} -> sv in language=
            if(lang.Contains("{{"))
            {
                newValue = LangTemplate.Replace(newValue, "$1$3");
                lang = Tools.GetTemplateParameterValue(newValue, "language");
            }

            // remove language=English on en-wiki
            if (lang.Equals("english", StringComparison.OrdinalIgnoreCase) || lang.Equals("en", StringComparison.OrdinalIgnoreCase))
                newValue = Tools.RemoveTemplateParameter(newValue, "language");
            
            // remove italics for work field for book/periodical, but not website -- auto italicised by template
            if (TheWork.Contains("''") && !TheWork.Contains("."))
                newValue = WorkInItalics.Replace(newValue, "$1$2");

            // remove quotes around title field: are automatically added by template markup
            foreach (string dequoteParam in ParametersToDequote)
            {
                string quotetitle;
                if(paramsFound.TryGetValue(dequoteParam, out quotetitle))
                {
                    string before = quotetitle;
                    // convert curly quotes to straight quotes per [[MOS:PUNCT]], but » or › may be section delimeter
                    // so only change those when balanced. Note regular <> characters are not changed.
                    quotetitle = WikiRegexes.CurlyDoubleQuotes.Replace(quotetitle, @"""");
                    quotetitle = BalancedArrows.Replace(quotetitle, @"""$1$2""");

                    if (quotetitle.Contains(@"""") && !quotetitle.Trim('"').Contains(@""""))
                        quotetitle = quotetitle.Trim('"');

                    if(!before.Equals(quotetitle))
                        newValue = Tools.SetTemplateParameterValue(newValue, dequoteParam, quotetitle);
                }
            }

            // page= and pages= fields don't need p. or pp. in them when nopp not set
            if ((pages.Contains("p") || page.Contains("p")) && !templatename.Equals("cite journal", StringComparison.OrdinalIgnoreCase) && nopp.Length == 0)
            {
                newValue = CiteTemplatePagesPP.Replace(newValue, "");
                pages = Tools.GetTemplateParameterValue(newValue, "pages");
                paramsFound.Remove("pages");
                paramsFound.Add("pages", pages);
            }

            // with Lua no need to rename date to year when date = YYYY, just remove year and date duplicating each other
            if (TheDate.Length == 4 && TheYear.Equals(TheDate))
                newValue = Tools.RemoveTemplateParameter(newValue, "date");

            // year = full date --> date = full date
            if (TheYear.Length > 5)
            {
                string TheYearCorected = IncorrectCommaInternationalDates.Replace(TheYear, @"$1 $2");
                TheYearCorected = IncorrectCommaAmericanDates.Replace(TheYearCorected, @"$1 $2, $3");
                
                if(!TheYearCorected.Equals(TheYear))
                {
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "year", TheYearCorected);
                    TheYear = TheYearCorected;
                }
            }
            
            if (TheYear.Length > 5 && (WikiRegexes.ISODates.IsMatch(TheYear) || WikiRegexes.InternationalDates.IsMatch(TheYear)
                                       || WikiRegexes.AmericanDates.IsMatch(TheYear)))
            {
                TheDate = TheYear;
                TheYear = "";
                newValue = Tools.RenameTemplateParameter(newValue, "year", "date");
            }

            // year=YYYY and date=...YYYY -> remove year; not for year=YYYYa
            if (TheYear.Length == 4 && TheDate.Contains(TheYear) && YearOnly.IsMatch(TheYear))
            {
                Parsers p = new Parsers();
                TheDate = p.FixDatesAInternal(TheDate);

                if(WikiRegexes.InternationalDates.IsMatch(TheDate) || WikiRegexes.AmericanDates.IsMatch(TheDate)
                   || WikiRegexes.ISODates.IsMatch(TheDate))
                {
                    TheYear = "";
                    newValue = Tools.RemoveTemplateParameter(newValue, "year");
                }
            }

            // month=Month and date=...Month... OR month=Month and date=same month (by conversion from ISO format)Ors month=nn and date=same month (by conversion to ISO format)
            int num;
            if ((TheMonth.Length > 2 && TheDate.Contains(TheMonth)) // named month within date
                || (TheMonth.Length > 2 && Tools.ConvertDate(TheDate, DateLocale.International).Contains(TheMonth))
                || (int.TryParse(TheMonth, out num) && Regex.IsMatch(Tools.ConvertDate(TheDate, DateLocale.ISO), @"\-0?" + TheMonth + @"\-")))
                newValue = Tools.RemoveTemplateParameter(newValue, "month");

            // date = Month DD and year = YYYY --> date = Month DD, YYYY
            if (!YearOnly.IsMatch(TheDate) && YearOnly.IsMatch(TheYear))
            {
                if (!WikiRegexes.AmericanDates.IsMatch(TheDate) && WikiRegexes.AmericanDates.IsMatch(TheDate + ", " + TheYear))
                {
                    if(!TheDate.Contains(TheYear))
                        newValue = Tools.SetTemplateParameterValue(newValue, "date", TheDate + ", " + TheYear);
                    newValue = Tools.RemoveTemplateParameter(newValue, "year");
                }
                else if (!WikiRegexes.InternationalDates.IsMatch(TheDate) && WikiRegexes.InternationalDates.IsMatch(TheDate + " " + TheYear))
                {
                    if(!TheDate.Contains(TheYear))
                        newValue = Tools.SetTemplateParameterValue(newValue, "date", TheDate + " " + TheYear);
                    newValue = Tools.RemoveTemplateParameter(newValue, "year");
                }
            }

            // correct volume=vol 7... and issue=no. 8 for {{cite journal}} only
            if (templatename.Equals("cite journal", StringComparison.OrdinalIgnoreCase))
            {
                newValue = CiteTemplatesJournalVolume.Replace(newValue, "");
                newValue = CiteTemplatesJournalIssue.Replace(newValue, "");

                if (TheIssue.Length == 0)
                    newValue = CiteTemplatesJournalVolumeAndIssue.Replace(newValue, @"| issue = ");
            }

            // {{cite web}} for Google books -> {{Cite book}}
            if (templatename.Contains("web") && newValue.Contains("http://books.google.") && TheWork.Length == 0)
                newValue = Tools.RenameTemplate(newValue, templatename, "Cite book");

            // remove leading zero in day of month
            if(Regex.IsMatch(newValue, @"\b0[1-9]") && DateLeadingZero.IsMatch(newValue))
            {
                newValue = DateLeadingZero.Replace(newValue, @"$1$2$3$4$5");
                newValue = DateLeadingZero.Replace(newValue, @"$1$2$3$4$5");
                TheDate = Tools.GetTemplateParameterValue(newValue, "date");
                accessdate = Tools.GetTemplateParameterValue(newValue, "accessdate");
            }

            if(paramsFound.Where(s => s.Key.Contains("access") && !s.Key.Contains("date")).Count() > 0)
            {
                if(Regex.IsMatch(templatename, @"[Cc]ite(?: ?web| book| news)"))
                {
                    // remove any empty accessdaymonth, accessmonthday, accessmonth and accessyear
                    newValue = AccessDayMonthDay.Replace(newValue, "");

                    // merge accessdate of 'D Month' or 'Month D' and accessyear of 'YYYY' in cite web
                    if(accessyear.Length == 4)
                        newValue = AccessDateYear.Replace(newValue, @" $2$1$3");
                }

                // remove accessyear where accessdate is present and contains said year
                if (accessyear.Length > 0 && accessdate.Contains(accessyear))
                    newValue = Tools.RemoveTemplateParameter(newValue, "accessyear");
            }

            // fix unspaced comma ranges, avoid pages=12,345 as could be valid page number
            if (pages.Contains(",") && Regex.Matches(pages, @"\b\d{1,2},\d{3}\b").Count == 0)
            {
                while (UnspacedCommaPageRange.IsMatch(pages))
                {
                    pages = UnspacedCommaPageRange.Replace(pages, "$1, $2");
                }
                newValue = Tools.UpdateTemplateParameterValue(newValue, "pages", pages);
                paramsFound.Remove("pages");
                paramsFound.Add("pages", pages);
            }

            // page range should have unspaced en-dash; validate that page is range not section link
            newValue = FixPageRanges(newValue, paramsFound);

            // page range or list should use 'pages' parameter not 'page'
            if (CiteTemplatesPageRangeName.IsMatch(newValue))
            {
                newValue = CiteTemplatesPageRangeName.Replace(newValue, @"$1pages$2");
                newValue = Tools.RemoveDuplicateTemplateParameters(newValue);
            }

            // remove ordinals from dates
            if(Ordinal.IsMatch(TheDate) || Ordinal.IsMatch(accessdate))
            {
                if (OrdinalsInDatesInt.IsMatch(TheDate))
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "date", OrdinalsInDatesInt.Replace(TheDate, "$1$2$3 $4"));
                else if (OrdinalsInDatesAm.IsMatch(TheDate))
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "date", OrdinalsInDatesAm.Replace(TheDate, "$1 $2$3"));

                if (OrdinalsInDatesInt.IsMatch(accessdate))
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "accessdate", OrdinalsInDatesInt.Replace(accessdate, "$1$2$3 $4"));
                else if(OrdinalsInDatesAm.IsMatch(accessdate))
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "accessdate", OrdinalsInDatesAm.Replace(accessdate, "$1 $2$3"));
            }
            // catch after any other fixes
            newValue = IncorrectCommaAmericanDates.Replace(newValue, @"$1 $2, $3");

            // URL starting www needs http://
            if (theURL.StartsWith("www", StringComparison.OrdinalIgnoreCase))
                theURL = "http://" + theURL;

            if(archiveurl.StartsWith("www", StringComparison.OrdinalIgnoreCase))
                newValue = Tools.UpdateTemplateParameterValue(newValue, "archiveurl", "http://" + archiveurl);
            if(contributionurl.StartsWith("www", StringComparison.OrdinalIgnoreCase))
                newValue = Tools.UpdateTemplateParameterValue(newValue, "contribution-url", "http://" + contributionurl);

            // (part) wikilinked/external linked URL in cite template, don't change when named external link format
            if(!theURL.Contains(" "))
                theURL = theURL.Trim('[').Trim(']');

            if(!theURLoriginal.Equals(theURL))
                newValue = Tools.UpdateTemplateParameterValue(newValue, "url", theURL);

            // {{dead link}} should be placed outside citation, not in format field per [[Template:Dead link]]
            if (WikiRegexes.DeadLink.IsMatch(format))
            {
                string deadLink = WikiRegexes.DeadLink.Match(format).Value;

                if (theURL.ToUpper().TrimEnd('L').EndsWith("HTM") && format.Equals(deadLink))
                    newValue = Tools.RemoveTemplateParameter(newValue, "format");
                else
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "format", format.Replace(deadLink, ""));

                newValue += (" " + deadLink);
            }

            //id=ISBN fix
            if (IdISBN.IsMatch(id) && ISBN.Length == 0)
            {
                newValue = Tools.RenameTemplateParameter(newValue, "id", "isbn");
                newValue = Tools.SetTemplateParameterValue(newValue, "isbn", IdISBN.Match(id).Groups[1].Value.Trim());
            }

            //id=ASIN fix
            if (IdASIN.IsMatch(id) && Tools.GetTemplateParameterValue(newValue, "asin").Length == 0 && Tools.GetTemplateParameterValue(newValue, "ASIN").Length == 0)
            {
                newValue = Tools.RenameTemplateParameter(newValue, "id", "asin");
                newValue = Tools.SetTemplateParameterValue(newValue, "asin", IdASIN.Match(id).Groups[1].Value.Trim());
            }

            if(ISBN.Length > 0)
            {
                string ISBNbefore = ISBN;
                // remove ISBN at start, but not if multiple ISBN
                if(ISBN.IndexOf("isbn", StringComparison.OrdinalIgnoreCase) > -1
                   && ISBN.Substring(4).IndexOf("isbn", StringComparison.OrdinalIgnoreCase) == -1)
                    ISBN = Regex.Replace(ISBN, @"^(?i)ISBN\s*", "");

                // trim unneeded characters
                ISBN = ISBN.Trim(".;,:".ToCharArray()).Trim();

                // fix dashes: only hyphens allowed
                while(ISBNDash.IsMatch(ISBN))
                    ISBN = ISBNDash.Replace(ISBN, @"$1-$2");
                ISBN = ISBN.Replace('\x2010', '-');
                ISBN = ISBN.Replace('\x2012', '-');

                if(!ISBN.Equals(ISBNbefore))
                {
                    if(paramsFound.ContainsKey("ISBN"))
                        newValue = Tools.UpdateTemplateParameterValue(newValue, "ISBN", ISBN);
                    else
                        newValue = Tools.UpdateTemplateParameterValue(newValue, "isbn", ISBN);
                }
            }

            // origyear --> year when no year/date
            if (TheYear.Length == 0 && TheDate.Length == 0 && origyear.Length == 4)
            {
                newValue = Tools.RenameTemplateParameter(newValue, "origyear", "year");
                newValue = Tools.RemoveDuplicateTemplateParameters(newValue);
            }

            return newValue;
        }

        private static readonly List<string> PageFields = new List<string>(new[] { "page", "pages", "p", "pp" });
        private static readonly Regex PageRange = new Regex(@"\b(\d+)\s*[-—]+\s*(\d+)", RegexOptions.Compiled);
        private static readonly Regex SpacedPageRange = new Regex(@"(\d+) +(–|&ndash;) +(\d)", RegexOptions.Compiled);

        /// <summary>
        /// Converts hyphens in page ranges in citation template fields to endashes
        /// </summary>
        /// <param name="templateCall">The template call</param>
        /// <param name="Params">Dictionary of template parameters in template call</param>
        /// <returns>The updated template call</returns>
        private static string FixPageRanges(string templateCall, Dictionary<string, string> Params)
        {
            foreach (string pageField in PageFields)
            {
                string pageRange;
                if(Params.TryGetValue(pageField, out pageRange) && pageRange.Length > 0)
                {
                    string res = FixPageRangesValue(pageRange);
                    if(!res.Equals(pageRange))
                        templateCall = Tools.UpdateTemplateParameterValue(templateCall, pageField, res);
                }
            }

            return templateCall;
        }

        private static string FixPageRangesValue(string pageRange)
        {
        	string original = pageRange;
        	// fix spaced page ranges e.g. 15 – 20 --> 15–20
        	if (SpacedPageRange.IsMatch(pageRange))
        		return SpacedPageRange.Replace(pageRange, "$1$2$3");

        	if (pageRange.Length > 2 && !pageRange.Contains(" to "))
        	{
        		bool pagerangesokay = true;
        		Dictionary<int, int> PageRanges = new Dictionary<int, int>();

        		foreach (Match pagerange in PageRange.Matches(pageRange))
        		{
        			string page1 = pagerange.Groups[1].Value;
        			string page2 = pagerange.Groups[2].Value;

        			// convert 350-2 into 350-352 etc.
        			if (page1.Length > page2.Length)
        				page2 = page1.Substring(0, page1.Length - page2.Length) + page2;

        			// check a valid range with difference < 999
        			pagerangesokay = (Convert.ToInt32(page1) < Convert.ToInt32(page2) &&
        			                  Convert.ToInt32(page2) - Convert.ToInt32(page1) < 999);

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

        			if (!pagerangesokay)
        				break;

        			// add to dictionary of ranges found
        			PageRanges.Add(Convert.ToInt32(page1), Convert.ToInt32(page2));
        		}

        		if (pagerangesokay)
        			return PageRange.Replace(pageRange, @"$1–$2");
        	}
        	
        	return original;
        }

        private static readonly Regex CiteWebOrNews = Tools.NestedTemplateRegex(new[] { "cite web", "citeweb", "cite news", "citenews" });
        private static readonly Regex PressPublishers = new Regex(@"(Associated Press|United Press International)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly List<string> WorkParameterAndAliases = new List<string>(new[] { "work", "newspaper", "journal", "periodical", "magazine" });

        /// <summary>
        /// Where the publisher field is used incorrectly instead of the work field in a {{cite web}} or {{cite news}} citation
        /// convert the parameter to be 'work'
        /// Scenarios covered:
        /// * publisher == URL domain, no work= used
        /// </summary>
        /// <param name="citation">the citation</param>
        /// <returns>the updated citation</returns>
        public static string CitationPublisherToWork(string citation)
        {
            // only for {{cite web}} or {{cite news}}
            if (!CiteWebOrNews.IsMatch(citation))
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
        /// Matches the {{birth date}} family of templates
        /// </summary>
        private static readonly Regex BirthDate = Tools.NestedTemplateRegex(new[] { "birth date", "birth-date", "dob", "bda", "birth date and age", "birthdate and age", "Date of birth and age", "BDA", "Birthdateandage",
                                                                                "Birth Date and age", "birthdate" }, true);

        /// <summary>
        /// Matches the {{death  date}} family of templates
        /// </summary>
        private static readonly Regex DeathDate = Tools.NestedTemplateRegex(new[] { "death date", "death-date", "dda", "death date and age", "deathdateandage", "deathdate" }, true);
        private static List<string> PersonDataLowerCaseParameters = new List<string>(new[] { "name", "alternative names", "short description", "date of birth", "place of birth", "date of death", "place of death" });

        /// <summary>
        /// * Adds the default {{persondata}} template to en-wiki mainspace pages about a person that don't already have {{persondata}}
        /// * Attempts to complete blank {{persondata}} fields based on infobox values
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <returns></returns>
        public static string PersonData(string articleText, string articleTitle)
        {
            int personDataCount = WikiRegexes.Persondata.Matches(articleText).Count;
            if (!Variables.IsWikipediaEN
                || personDataCount > 1
                || (articleText.Contains("{{Persondata") && personDataCount == 0)) // skip in case of existing persondata with unbalanced brackets
                return articleText;

            string originalPersonData, newPersonData;

            // add default persondata if missing
            if (personDataCount == 0)
            {
                if (IsArticleAboutAPerson(articleText, articleTitle, true))
                {
                    articleText = articleText + Tools.Newline(WikiRegexes.PersonDataDefault);
                    newPersonData = originalPersonData = WikiRegexes.PersonDataDefault;
                }
                else
                    return articleText;
            }
            else
            {
                originalPersonData = WikiRegexes.Persondata.Match(articleText).Value;
                newPersonData = originalPersonData;
                // use uppercase parameters if making changes, rename any lowercase ones
                if(Tools.GetTemplateParameterValues(newPersonData).Any(s => PersonDataLowerCaseParameters.Contains(s.Key)))
                {
                    newPersonData = Tools.RenameTemplateParameter(newPersonData, "name", "NAME");
                    newPersonData = Tools.RenameTemplateParameter(newPersonData, "alternative names", "ALTERNATIVE NAMES");
                    newPersonData = Tools.RenameTemplateParameter(newPersonData, "short description", "SHORT DESCRIPTION");
                    newPersonData = Tools.RenameTemplateParameter(newPersonData, "date of birth", "DATE OF BIRTH");
                    newPersonData = Tools.RenameTemplateParameter(newPersonData, "place of birth", "PLACE OF BIRTH");
                    newPersonData = Tools.RenameTemplateParameter(newPersonData, "date of death", "DATE OF DEATH");
                    newPersonData = Tools.RenameTemplateParameter(newPersonData, "place of death", "PLACE OF DEATH");
                }
            }

            // attempt completion of some persondata fields

            // name
            if (Tools.GetTemplateParameterValue(newPersonData, "NAME", true).Length == 0)
            {
                string name = WikiRegexes.Defaultsort.Match(articleText).Groups["key"].Value;
                if (name.Contains(" ("))
                    name = name.Substring(0, name.IndexOf(" ("));

                if (name.Length == 0 && Tools.WordCount(articleTitle) == 1)
                    name = articleTitle;

                if (name.Length == 0)
                    name = Tools.MakeHumanCatKey(articleTitle, articleText);

                if (name.Length > 0)
                {
                    name = Tools.ReAddDiacritics(articleTitle, name);
                    newPersonData = Tools.SetTemplateParameterValue(newPersonData, "NAME", name, true);
                }
            }

            // date of birth
            if (Tools.GetTemplateParameterValue(newPersonData, "DATE OF BIRTH", true).Length == 0)
            {
                newPersonData = SetPersonDataDate(newPersonData, "DATE OF BIRTH", GetInfoBoxFieldValue(articleText, WikiRegexes.InfoBoxDOBFields), articleText);

                // as fallback use year from category
                if(Tools.GetTemplateParameterValue(newPersonData, "DATE OF BIRTH", true).Length == 0)
                {
                    Match m = WikiRegexes.BirthsCategory.Match(articleText);

                    if (m.Success)
                    {
                        string year = m.Value.Replace(@"[[Category:", "").TrimEnd(']');
                        if (Regex.IsMatch(year, @"^\d{3,4} (?:BC )?births(\|.*)?$"))
                            newPersonData = Tools.SetTemplateParameterValue(newPersonData, "DATE OF BIRTH", year.Substring(0, year.IndexOf(" births")), true);
                    }
                }
            }

            // date of death
            if (Tools.GetTemplateParameterValue(newPersonData, "DATE OF DEATH", true).Length == 0)
            {
                newPersonData = SetPersonDataDate(newPersonData, "DATE OF DEATH", GetInfoBoxFieldValue(articleText, WikiRegexes.InfoBoxDODFields), articleText);

                // as fallback use year from category
                if (Tools.GetTemplateParameterValue(newPersonData, "DATE OF DEATH", true).Length == 0)
                {
                    Match m = WikiRegexes.DeathsOrLivingCategory.Match(articleText);

                    if (m.Success)
                    {
                        string year = m.Value.Replace(@"[[Category:", "").TrimEnd(']');
                        if (Regex.IsMatch(year, @"^\d{3,4} deaths(\|.*)?$"))
                            newPersonData = Tools.SetTemplateParameterValue(newPersonData, "DATE OF DEATH", year.Substring(0, year.IndexOf(" deaths")), true);
                    }
                }
            }

            // place of birth
            string ExistingPOB = Tools.GetTemplateParameterValue(newPersonData, "PLACE OF BIRTH", true);
            if (ExistingPOB.Length == 0)
            {
                string POB = GetInfoBoxFieldValue(articleText, WikiRegexes.InfoBoxPOBFields);

                // as fallback look up cityofbirth/countryofbirth
                if (POB.Length == 0)
                {
                    string ib = WikiRegexes.InfoBox.Match(articleText).Value;
                    POB = (Tools.GetTemplateParameterValue(ib, "cityofbirth") + ", " + Tools.GetTemplateParameterValue(ib, "countryofbirth")).Trim(',');
                }

                POB = WikiRegexes.FileNamespaceLink.Replace(POB, "").Trim();

                POB = WikiRegexes.NestedTemplates.Replace(WikiRegexes.Br.Replace(POB, " "), "").Trim();
                POB = WikiRegexes.Small.Replace(WikiRegexes.Refs.Replace(POB, ""), "$1").TrimEnd(',');
                POB = POB.Replace(@"???", "").Trim();

                newPersonData = Tools.SetTemplateParameterValue(newPersonData, "PLACE OF BIRTH", POB, true);
            }

            // place of death
            string ExistingPOD = Tools.GetTemplateParameterValue(newPersonData, "PLACE OF DEATH", true);
            if (ExistingPOD.Length == 0)
            {
                string POD = GetInfoBoxFieldValue(articleText, WikiRegexes.InfoBoxPODFields);

                // as fallback look up cityofbirth/countryofbirth
                if (POD.Length == 0)
                {
                    string ib = WikiRegexes.InfoBox.Match(articleText).Value;
                    POD = (Tools.GetTemplateParameterValue(ib, "cityofdeath") + ", " + Tools.GetTemplateParameterValue(ib, "countryofdeath")).Trim(',');
                }

                POD = WikiRegexes.FileNamespaceLink.Replace(POD, "").Trim();
                POD = WikiRegexes.NestedTemplates.Replace(WikiRegexes.Br.Replace(POD, " "), "").Trim();
                POD = WikiRegexes.Small.Replace(WikiRegexes.Refs.Replace(POD, ""), "$1").TrimEnd(',');
				POD = POD.Replace(@"???", "").Trim();

                newPersonData = Tools.SetTemplateParameterValue(newPersonData, "PLACE OF DEATH", POD, true);
            }

            // look for full dates matching birth/death categories
            newPersonData = Tools.RemoveDuplicateTemplateParameters(CompletePersonDataDate(newPersonData, articleText));

            // merge changes
            if (!newPersonData.Equals(originalPersonData) && newPersonData.Length > originalPersonData.Length)
                articleText = articleText.Replace(originalPersonData, newPersonData);

            // remove any <small> tags in persondata
            articleText = WikiRegexes.Persondata.Replace(articleText, pd => {
                                                             string res= WikiRegexes.Small.Replace(pd.Value, "$1");
                                                             res = SmallStart.Replace(res, "");
                                                             return SmallEnd.Replace(res, "");
                                                         });

            return articleText;
        }

        private static readonly List<string> DfMf = new List<string>(new[] { "df", "mf" });

        /// <summary>
        /// Completes a persondata call with a date of birth/death.
        /// </summary>
        /// <param name="personData"></param>
        /// <param name="field"></param>
        /// <param name="sourceValue"></param>
        /// <param name="articletext"></param>
        /// <returns>The updated persondata call</returns>
        private static string SetPersonDataDate(string personData, string field, string sourceValue, string articletext)
        {
            string dateFound = "";

            if (field.Equals("DATE OF BIRTH") && BirthDate.IsMatch(articletext))
            {
                sourceValue = Tools.RemoveTemplateParameters(BirthDate.Match(articletext).Value, DfMf);
                dateFound = Tools.GetTemplateArgument(sourceValue, 1);

                // first argument is a year, or a full date
                if (dateFound.Length < 5)
                    dateFound += ("-" + Tools.GetTemplateArgument(sourceValue, 2) + "-" + Tools.GetTemplateArgument(sourceValue, 3));
            }
            else if (field.Equals("DATE OF DEATH") && DeathDate.Matches(articletext).Count == 1)
            {
                sourceValue = Tools.RemoveTemplateParameters(DeathDate.Match(articletext).Value, DfMf);
                dateFound = Tools.GetTemplateArgument(sourceValue, 1);
                if (dateFound.Length < 5)
                    dateFound += ("-" + Tools.GetTemplateArgument(sourceValue, 2) + "-" + Tools.GetTemplateArgument(sourceValue, 3));
            }
            else if (WikiRegexes.AmericanDates.IsMatch(sourceValue))
                dateFound = WikiRegexes.AmericanDates.Match(sourceValue).Value;
            else if (WikiRegexes.InternationalDates.IsMatch(sourceValue))
                dateFound = WikiRegexes.InternationalDates.Match(sourceValue).Value;
            else if (WikiRegexes.ISODates.IsMatch(sourceValue))
                dateFound = WikiRegexes.ISODates.Match(sourceValue).Value;

            // if date not found yet, fall back to year/month/day of brith fields or birth date in {{dda}}
            if (dateFound.Length == 0)
            {
                if (field.Equals("DATE OF BIRTH"))
                {
                    if (GetInfoBoxFieldValue(articletext, "yearofbirth").Length > 0)
                        dateFound = (GetInfoBoxFieldValue(articletext, "yearofbirth") + "-" + GetInfoBoxFieldValue(articletext, "monthofbirth") + "-" + GetInfoBoxFieldValue(articletext, "dayofbirth")).Trim('-');
                    else if (GetInfoBoxFieldValue(articletext, "yob").Length > 0)
                        dateFound = (GetInfoBoxFieldValue(articletext, "yob") + "-" + GetInfoBoxFieldValue(articletext, "mob") + "-" + GetInfoBoxFieldValue(articletext, "dob")).Trim('-');
                    else if (WikiRegexes.DeathDateAndAge.IsMatch(articletext))
                    {
                        string dda = Tools.RemoveTemplateParameters(WikiRegexes.DeathDateAndAge.Match(articletext).Value, DfMf);
                        dateFound = (Tools.GetTemplateArgument(dda, 4) + "-" + Tools.GetTemplateArgument(dda, 5) + "-" + Tools.GetTemplateArgument(dda, 6)).Trim('-');
                    }
                    else if (GetInfoBoxFieldValue(articletext, "birthyear").Length > 0)
                        dateFound = (GetInfoBoxFieldValue(articletext, "birthyear") + "-" + GetInfoBoxFieldValue(articletext, "birthmonth") + "-" + GetInfoBoxFieldValue(articletext, "birthday")).Trim('-');
                }
                else if (field.Equals("DATE OF DEATH"))
                {
                    if (GetInfoBoxFieldValue(articletext, "yearofdeath").Length > 0)
                        dateFound = (GetInfoBoxFieldValue(articletext, "yearofdeath") + "-" + GetInfoBoxFieldValue(articletext, "monthofdeath") + "-" + GetInfoBoxFieldValue(articletext, "dayofdeath")).Trim('-');
                    else if (GetInfoBoxFieldValue(articletext, "deathyear").Length > 0)
                        dateFound = (GetInfoBoxFieldValue(articletext, "deathyear") + "-" + GetInfoBoxFieldValue(articletext, "deathmonth") + "-" + GetInfoBoxFieldValue(articletext, "deathday")).Trim('-');
                    else if (GetInfoBoxFieldValue(articletext, "yod").Length > 0)
                        dateFound = (GetInfoBoxFieldValue(articletext, "yod") + "-" + GetInfoBoxFieldValue(articletext, "mod") + "-" + GetInfoBoxFieldValue(articletext, "dod")).Trim('-');
                }
            }

            // call parser function for futher date fixes
            if(dateFound.Length > 0)
            {
                dateFound = WikiRegexes.Comments.Replace(CiteTemplateDates(@"{{cite web|date=" + dateFound + @"}}").Replace(@"{{cite web|date=", "").Trim('}'), "");

                dateFound = Tools.ConvertDate(dateFound, DeterminePredominantDateLocale(articletext, false)).Trim('-');

                // check ISO dates valid (in case dda used zeros for month/day)
                if (dateFound.Contains("-") && !WikiRegexes.ISODates.IsMatch(dateFound))
                    return personData;

                return Tools.SetTemplateParameterValue(personData, field, dateFound, true);
            }
            
            return personData;
        }

        private static readonly Regex BracketedBirthDeathDate = new Regex(@"\(([^()]+)\)", RegexOptions.Compiled);
        private static readonly Regex FreeFormatDied = new Regex(@"(?:(?:&nbsp;| )(?:-|–|&ndash;)(?:&nbsp;| )|\b[Dd](?:ied\b|\.))", RegexOptions.Compiled);

        /// <summary>
        /// Sets persondata date of birth/death fields based on unformatted info in zeroth section of article, provided dates match existing birth/death categories
        /// </summary>
        /// <param name="personData">Persondata template call</param>
        /// <param name="articletext">The article text</param>
        /// <returns>The updated persondata template call</returns>
        private static string CompletePersonDataDate(string personData, string articletext)
        {
            // get the existing values
            string existingBirthYear = Tools.GetTemplateParameterValue(personData, "DATE OF BIRTH", true);
            string existingDeathYear = Tools.GetTemplateParameterValue(personData, "DATE OF DEATH", true);

            if (existingBirthYear.Length == 4 || existingDeathYear.Length == 4)
            {
                Parsers p = new Parsers();
                string birthDateFound = "", deathDateFound = "";
                string zerothSection = Tools.GetZerothSection(articletext);

                // remove references, wikilinks, templates
                zerothSection = WikiRegexes.Refs.Replace(zerothSection, " ");
                zerothSection = WikiRegexes.SimpleWikiLink.Replace(zerothSection, " ");

                if (WikiRegexes.CircaTemplate.IsMatch(zerothSection))
                    zerothSection = zerothSection.Substring(0, WikiRegexes.CircaTemplate.Match(zerothSection).Index);

                zerothSection = Tools.NestedTemplateRegex("ndash").Replace(zerothSection, " &ndash;");
                zerothSection = WikiRegexes.NestedTemplates.Replace(zerothSection, " ");
                // clean up any format errors in birth/death dates we may want to use
                zerothSection = p.FixDatesAInternal(zerothSection);

                // look for date in bracketed text, check date matches existing value (from categories)
                foreach (Match m in BracketedBirthDeathDate.Matches(zerothSection))
                {
                    string bValue = m.Value;

                    if (!UncertainWordings.IsMatch(bValue) && !ReignedRuledUnsure.IsMatch(bValue) && !FloruitTemplate.IsMatch(bValue))
                    {

                        string bBorn, bDied = "";
                        // split on died/spaced dash
                        if (FreeFormatDied.IsMatch(bValue))
                        {
                            bBorn = bValue.Substring(0, FreeFormatDied.Match(bValue).Index);
                            bDied = bValue.Substring(FreeFormatDied.Match(bValue).Index);
                        }
                        else
                            bBorn = bValue;

                        // born
                        if (existingBirthYear.Length == 4)
                        {
                            if (WikiRegexes.AmericanDates.Matches(bBorn).Count == 1 && WikiRegexes.AmericanDates.Match(bBorn).Value.Contains(existingBirthYear))
                                birthDateFound = WikiRegexes.AmericanDates.Match(bBorn).Value;
                            else if (WikiRegexes.InternationalDates.Matches(bBorn).Count == 1 && WikiRegexes.InternationalDates.Match(bBorn).Value.Contains(existingBirthYear))
                                birthDateFound = WikiRegexes.InternationalDates.Match(bBorn).Value;
                        }

                        // died
                        if (existingDeathYear.Length == 4)
                        {
                            if (WikiRegexes.AmericanDates.Matches(bDied).Count == 1 && WikiRegexes.AmericanDates.Match(bDied).Value.Contains(existingDeathYear))
                                deathDateFound = WikiRegexes.AmericanDates.Match(bDied).Value;
                            else if (WikiRegexes.InternationalDates.Matches(bDied).Count == 1 && WikiRegexes.InternationalDates.Match(bDied).Value.Contains(existingDeathYear))
                                deathDateFound = WikiRegexes.InternationalDates.Match(bDied).Value;
                        }

                        if (birthDateFound.Length > 0 || deathDateFound.Length > 0)
                            break;
                    }
                }

                if (birthDateFound.Length > 4)
                    personData = Tools.SetTemplateParameterValue(personData, "DATE OF BIRTH", Tools.ConvertDate(birthDateFound, DeterminePredominantDateLocale(articletext, true)), false);

                if (deathDateFound.Length > 4)
                    personData = Tools.SetTemplateParameterValue(personData, "DATE OF DEATH", Tools.ConvertDate(deathDateFound, DeterminePredominantDateLocale(articletext, true)), false);
            }

            return personData;
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

        private static readonly Regex SingleCurlyBrackets = new Regex(@"{((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))})", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static readonly Regex DoubleCurlyBrackets = new Regex(@"{{(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}}", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static readonly Regex DoubleSquareBrackets = new Regex(@"\[\[((?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\]\])", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static readonly Regex SingleSquareBrackets = new Regex(@"\[((?>[^\[\]]+|\[(?<DEPTH>)|\](?<-DEPTH>))*(?(DEPTH)(?!))\])", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static readonly Regex SingleRoundBrackets = new Regex(@"\(((?>[^\(\)]+|\((?<DEPTH>)|\)(?<-DEPTH>))*(?(DEPTH)(?!))\))", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static readonly Regex Tags = new Regex(@"\<((?>[^\<\>]+|\<(?<DEPTH>)|\>(?<-DEPTH>))*(?(DEPTH)(?!))\>)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        private static readonly Regex HideNestedBrackets = new Regex(@"&#9[13];");
        private static readonly Regex AmountComparison = new Regex(@"[<>]\s*\d", RegexOptions.Compiled);
        private static readonly Regex TemplatesWithUnbalancedBrackets = Tools.NestedTemplateRegex(new [] {"LSJ", ")!", "!("});

        /// <summary>
        /// Checks the article text for unbalanced brackets, either square or curly
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="bracketLength">integer to hold length of unbalanced bracket found</param>
        /// <returns>Index of any unbalanced brackets found</returns>
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Missing_opening_or_closing_brackets.2C_table_and_template_markup_.28WikiProject_Check_Wikipedia_.23_10.2C_28.2C_43.2C_46.2C_47.29
        public static int UnbalancedBrackets(string articleText, out int bracketLength)
        {
            // &#91; and &#93; are used to replace the [ or ] in external link text, which gives correct markup
            // replace back to avoid matching as unbalanced brackets
            articleText = HideNestedBrackets.Replace(articleText, m => (m.Value.Contains("93") ? "]    " : "[    "));

            // remove all <math>, <code> stuff etc. where curly brackets are used in singles and pairs
            articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.MathPreSourceCodeComments);
            // some templates deliberately use unbalanced brackets within their parameters
            articleText = Tools.ReplaceWithSpaces(articleText, TemplatesWithUnbalancedBrackets);

            bracketLength = 2;

            int unbalancedfound = UnbalancedBrackets(articleText, "{{", "}}", DoubleCurlyBrackets);
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
        /// <param name="bracketsRegex">Regex that matches correctly balanced (possibly nested) bracket combinations</param>
        /// <returns>Index of the first of any unbalanced brackets found</returns>
        private static int UnbalancedBrackets(string articleText, string openingBrackets, string closingBrackets, Regex bracketsRegex)
        {
            // Performance: use Regex.Split with regexes with no capturing groups to get array of text not matched by the regex
            // Then filter this to see if any unbalanced brackets remain. Return if none remain
            bool hasBrackets = bracketsRegex.Split(articleText).Any(s => (s.IndexOf(openingBrackets, StringComparison.Ordinal) > -1 || s.IndexOf(closingBrackets, StringComparison.Ordinal) > -1));

            if(!hasBrackets)
                return -1;

            // If here then some unbalanced brackets were found, so use (slower) ReplaceWithSpaces so we can find index of unbalanced bracket in original text
            articleText = Tools.ReplaceWithSpaces(articleText, bracketsRegex);

            // for tags don't mark "> 50 cm" as unbalanced
            if (openingBrackets.Equals("<") && AmountComparison.IsMatch(articleText))
                return -1;

            // now return the unbalanced one that's left
            int open = articleText.IndexOf(openingBrackets, StringComparison.Ordinal), closed = articleText.IndexOf(closingBrackets, StringComparison.Ordinal);
            if(open > -1 || closed > -1)
            {
                int openCount = Regex.Matches(articleText, Regex.Escape(openingBrackets)).Count;
                int closedCount = Regex.Matches(articleText, Regex.Escape(closingBrackets)).Count;

                if(openCount == 0 && closedCount >= 1)
                    return closed;
                if(openCount >= 1)
                    return open;
            }
            return -1;
        }

        private static readonly Regex LinkWhitespace1 = new Regex(@"(?: |^)\[\[ ([^\]]{1,30})\]\]", RegexOptions.Compiled);
        private static readonly Regex LinkWhitespace2 = new Regex(@"(?<=\w)\[\[ ([^\]]{1,30})\]\]", RegexOptions.Compiled);
        private static readonly Regex LinkWhitespace3 = new Regex(@"\[\[([^\]]{1,30}?) {2,10}([^\]]{1,30})\]\]", RegexOptions.Compiled);
        private static readonly Regex LinkWhitespace4 = new Regex(@"\[\[([^\]\|]{1,30}) \]\](\s)", RegexOptions.Compiled);
        private static readonly Regex LinkWhitespace5 = new Regex(@"\[\[([^\]]{1,30}) \]\](?=\w)", RegexOptions.Compiled);

        private static readonly Regex DateLinkWhitespace = new Regex(@"(?<=\b\[\[(?:\d\d? " + WikiRegexes.MonthsNoGroup + @"|"  + WikiRegexes.MonthsNoGroup + @" \d\d?)\]\](?:,|,?  ))(\[\[\d{1,4}\]\])\b", RegexOptions.IgnoreCase);
        private static readonly Regex SectionLinkWhitespace = new Regex(@"(\[\[[^\[\]\|]+)(?: +# *| *# +)([^\[\]]+\]\])(?<!\[\[[ACFJ]# .*)", RegexOptions.Compiled);
        private static readonly Regex Hash = new Regex(@"#", RegexOptions.Compiled);

        // Covered by LinkTests.TestFixLinkWhitespace()
        /// <summary>
        /// Fix leading, trailing and middle spaces in Wikilinks
        /// </summary>
        /// <param name="articleText">The wiki text of the article</param>
        /// <param name="articleTitle">The article title.</param>
        /// <returns>The modified article text.</returns>
        public static string FixLinkWhitespace(string articleText, string articleTitle)
        {
            // Performance strategy: get list of all wikilinks, deduplicate, only apply regexes to whole article text if matching wikilinks
            List<string> allWikiLinks = Tools.DeduplicateList((from Match m in WikiRegexes.SimpleWikiLink.Matches(articleText) select m.Value).ToList());

            if(allWikiLinks.Any(s => s.StartsWith("[[ ")))
            {
                //remove undesirable space from beginning of wikilink (space before wikilink) - parse this line first
                articleText = LinkWhitespace1.Replace(articleText, " [[$1]]");

                //remove undesirable space from beginning of wikilink and move it outside link - parse this line second
                articleText = LinkWhitespace2.Replace(articleText, " [[$1]]");
            }

            //remove undesirable double space from middle of wikilink (up to 61 characters in wikilink)
            if(allWikiLinks.Any(s => s.Contains("  ")))
                while(LinkWhitespace3.IsMatch(articleText))
                    articleText = LinkWhitespace3.Replace(articleText, "[[$1 $2]]");

            // Remove underscore before hash
            if(allWikiLinks.Any(s => s.Contains("_#")))
                articleText = WikiRegexes.WikiLinksOnlyPossiblePipe.Replace(articleText, m=> m.Value.Replace("_#", "#"));

            if(allWikiLinks.Any(s => s.EndsWith(" ]]")))
            {
                //remove undesirable space from end of wikilink (whitespace after wikilink) - parse this line first
                articleText = LinkWhitespace4.Replace(articleText, "[[$1]]$2");

                //remove undesirable space from end of wikilink and move it outside link - parse this line second
                articleText = LinkWhitespace5.Replace(articleText, "[[$1]] ");
            }

            //remove undesirable double space between wikilinked dates
            if(allWikiLinks.Any(s => s.Contains("  ")))
                articleText = DateLinkWhitespace.Replace(articleText, "$1");

            // [[link #section]] or [[link# section]] --> [[link#section]], don't change if hash in part of text of section link
            if(allWikiLinks.Any(s => (s.Contains("# ") || s.Contains(" #"))))
                articleText = SectionLinkWhitespace.Replace(articleText, m => (Hash.Matches(m.Value).Count == 1) ? m.Groups[1].Value.TrimEnd() + "#" + m.Groups[2].Value.TrimStart() : m.Value);

            // correct [[page# section]] to [[page#section]]
            if (articleTitle.Length > 0)
            {
                Regex sectionLinkWhitespace = new Regex(@"(\[\[" + Regex.Escape(articleTitle) + @"\#)\s+([^\[\]]+\]\])");

                return sectionLinkWhitespace.Replace(articleText, "$1$2");
            }

            return articleText;
        }

        private static readonly Regex InfoBoxSingleAlbum = Tools.NestedTemplateRegex(new[] { "Infobox Single", "Infobox single", "Infobox album", "Infobox Album" });
        private static readonly Regex TaxoboxColour = Tools.NestedTemplateRegex(new[] { "taxobox colour", "taxobox color" });
        private static readonly Regex TrailingPipe = new Regex(@"\|\s*\]");

        // Partially covered by FixMainArticleTests.SelfLinkRemoval()
        /// <summary>
        /// Fixes link syntax, including removal of self links.
        /// Underscores not removed from link where page in [[Category:Articles with underscores in the title]]
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public static string FixLinks(string articleText, string articleTitle, out bool noChange)
        {
            string articleTextAtStart = articleText;
            string escTitle = Regex.Escape(articleTitle);

            if (InfoBoxSingleAlbum.IsMatch(articleText))
                articleText = FixLinksInfoBoxSingleAlbum(articleText, articleTitle);

            // clean up wikilinks: replace underscores, percentages and URL encoded accents etc.
            List<Match> wikiLinks = (from Match m in WikiRegexes.WikiLink.Matches(articleText) select m).ToList();
            
            // Replace {{!}} with a standard pipe
            List<Match> wikiLinksWithExclamation = wikiLinks.Where(m => m.Value.Contains(@"{{!}}")).ToList();
            
            foreach(Match m in wikiLinksWithExclamation)
            	articleText = articleText.Replace(m.Value, m.Value.Replace(@"{{!}}", "|"));

            // See if any self interwikis that need fixing later
            bool hasAnySelfInterwikis = wikiLinks.Any(m => m.Value.Contains(Variables.LangCode + ":"));

            // Performance: on articles with lots of links better to filter down to those that could be changed by canonicalization, rather than running regex replace against all links
            wikiLinks.RemoveAll(link => link.Value.IndexOfAny("&%_".ToCharArray()) < 0);

            foreach(Match m in wikiLinks)
            {
                string res = WikiRegexes.WikiLink.Replace(m.Value, FixLinksWikilinkCanonicalizeME);
                if(res != m.Value)
                    articleText = articleText.Replace(m.Value, res);
            }

            // First check for performance, second to avoid (dodgy) apostrophe after link
            if(articleText.Contains("|''") && !articleText.Contains(@"]]'"))
                articleText = WikiRegexes.PipedWikiLink.Replace(articleText, FixLinksWikilinkBoldItalicsME);

            // fix excess trailing pipe, TrailingPipe regex for performance
            if(TrailingPipe.IsMatch(articleText))
                articleText = WikiRegexes.PipedWikiLink.Replace(articleText, m => (m.Groups[2].Value.Trim().EndsWith("|") ? "[[" + m.Groups[1].Value + "|" + m.Groups[2].Value.Trim().TrimEnd('|').Trim() + "]]" : m.Value));

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Your_code_creates_page_errors_inside_imagemap_tags.
            // don't apply if there's an imagemap on the page or some noinclude transclusion business
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Includes_and_selflinks
            // TODO, better to not apply to text within imagemaps
            if (Regex.IsMatch(articleText, @"\[\[\s*(" + Tools.CaseInsensitive(escTitle) + @")\s*(?:\]|\|)")
                && !WikiRegexes.ImageMap.IsMatch(articleText)
                && !WikiRegexes.IncludeonlyNoinclude.IsMatch(articleText)
                && !TaxoboxColour.IsMatch(articleText))
            {
                // remove any self-links, but not other links with different capitaliastion e.g. [[Foo]] vs [[FOO]]
                articleText = Regex.Replace(articleText, @"\[\[\s*(" + Tools.CaseInsensitive(escTitle)
                                            + @")\s*\]\]", "$1");

                // remove piped self links by leaving target
                articleText = Regex.Replace(articleText, @"\[\[\s*" + Tools.CaseInsensitive(escTitle)
                                            + @"\s*\|\s*([^\]]+)\s*\]\]", "$1");
            }

            // fix for self interwiki links
            if(hasAnySelfInterwikis)
                articleText = FixSelfInterwikis(articleText);

            noChange = articleText.Equals(articleTextAtStart);
            return articleText;
        }

        private static readonly Regex SectionLink = new Regex(@"(.+)#(.+)");

        /// <summary>
        /// Canonicalize link targets, removes underscores except if page from [[Category:Articles with underscores in the title]]
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static string FixLinksWikilinkCanonicalizeME(Match m)
        {
            string theTarget = m.Groups[1].Value, y = m.Value;
            // don't convert %27%27 -- https://phabricator.wikimedia.org/T10932
            if (theTarget.Length > 0 && !theTarget.Contains("%27%27"))
            {
                string newTarget;
                if(theTarget.Contains("#")) // check for performance
                {
                    Match sl = SectionLink.Match(theTarget);

                    if(sl.Success) // Canonicalize section links in two parts
                        newTarget = CanonicalizeTitle(sl.Groups[1].Value) + "#" + CanonicalizeTitle(sl.Groups[2].Value);
                    else
                        newTarget = CanonicalizeTitle(theTarget);
                }
                else
                    newTarget = CanonicalizeTitle(theTarget);

                return y.Replace(theTarget, newTarget.Replace("%20", " "));
            }

            return y;
        }

        /// <summary>
        /// Converts [[foo|'''foo''']] → '''[[foo|foo]]''' for bold or italics
        /// only simplify where link & target values are the same without bold/italics (first letter case insensitive
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static string FixLinksWikilinkBoldItalicsME(Match m)
        {
            string theLinkText = m.Groups[2].Value, y = m.Value;

            if(theLinkText.Length > 0 && Tools.TurnFirstToUpper(m.Groups[1].Value.Trim()).Equals(Tools.TurnFirstToUpper(m.Groups[2].Value.Trim("'".ToCharArray()).Trim())))
            {
                if(WikiRegexes.Bold.Match(theLinkText).Value.Equals(theLinkText))
                    y = "'''" + y.Replace(theLinkText, WikiRegexes.Bold.Replace(theLinkText, "$1")) + "'''";
                else if(WikiRegexes.Italics.Match(theLinkText).Value.Equals(theLinkText))
                    y = "''" + y.Replace(theLinkText, WikiRegexes.Italics.Replace(theLinkText, "$1")) + "''";
            }

            return y;
        }

        /// <summary>
        /// Reformats self interwikis to be standard links. Only applies to self interwikis before other interwikis (i.e. those in body of article)
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>The updated article text</returns>
        private static string FixSelfInterwikis(string articleText)
        {
            foreach (Match m in WikiRegexes.PossibleInterwikis.Matches(articleText))
            {
                // interwiki should not be to own wiki – convert to standard wikilink
                if (m.Groups[1].Value.Equals(Variables.LangCode))
                    articleText = articleText.Replace(m.Value, @"[[" + m.Groups[2].Value + @"]]");
                else
                    break;
            }

            return articleText;
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
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#.22This_album.2Fsingle.22
            // for this single or this album within the infobox, make bold instead of delinking
            const string infoBoxSingleAlbum = @"(?s)(?<={{[Ii]nfobox (?:[Ss]ingle|[Aa]lbum).*?\|\s*[Tt]his (?:[Ss]ingle|[Aa]lbum)\s*=[^{}]*?)(?:''')?\[\[\s*";
            articleText = Regex.Replace(articleText, infoBoxSingleAlbum + escTitle + @"\s*\]\](?:''')?(?=[^{}\|]*(?:\||}}))", @"'''" + articleTitle + @"'''");
            articleText = Regex.Replace(articleText, infoBoxSingleAlbum + lowerTitle + @"\s*\]\](?:''')?(?=[^{}\|]*(?:\||}}))", @"'''" + lowerTitle + @"'''");
            articleText = Regex.Replace(articleText, infoBoxSingleAlbum + escTitle + @"\s*\|\s*([^\]]+)\s*\]\](?:''')?(?=[^{}\|]*(?:\||}}))", @"'''" + "$1" + @"'''");
            articleText = Regex.Replace(articleText, infoBoxSingleAlbum + lowerTitle + @"\s*\|\s*([^\]]+)\s*\]\](?:''')?(?=[^{}\|]*(?:\||}}))", @"'''" + "$1" + @"'''");

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
        /// Fixes CHECKWIKI error 64
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The simplified article text.</returns>
        public static string SimplifyLinks(string articleText)
        {
            // Performance: first get a list of unique links to avoid processing duplicate links more than once
            List<string> pipedLinks = Tools.DeduplicateList((from Match m in WikiRegexes.PipedWikiLink.Matches(articleText) select m.Value).ToList());

            // Performance: second determine if any links with pipe whitespace to clean
            string Category = Variables.Namespaces.ContainsKey(Namespace.Category) ? Variables.Namespaces[Namespace.Category] : "Category:";
            bool whitepaceTrimNeeded = pipedLinks.Any(s => ((s.Contains("| ") && !s.Contains(Category)) || s.Contains(" |") || (!s.Contains("| ]]") && s.Contains(" ]]"))));
Tools.WriteDebug("SL", whitepaceTrimNeeded.ToString());
            foreach (string pl in pipedLinks)
            {
                Match m = WikiRegexes.PipedWikiLink.Match(pl);
                string a = m.Groups[1].Value.Trim(), b = m.Groups[2].Value;

                // Must retain space after pipe in Category namespace
                if(whitepaceTrimNeeded)
                {
                    b = (Namespace.Determine(a) != Namespace.Category)
                    ? m.Groups[2].Value.Trim()
                    : m.Groups[2].Value.TrimEnd(new[] { ' ' });

                    if (b.Length == 0)
                        continue;
                 }

                string b2 = CanonicalizeTitle(b), a2 = CanonicalizeTitle(a);
                string lb = Tools.TurnFirstToLower(b), la = Tools.TurnFirstToLower(a);

                if (b2.Equals(a) || b2.Equals(la) || a2.Equals(b) || a2.Equals(lb)) // target and text the same after cleanup and case conversion e.g. [[A|a]] or [[Foo_bar|Foo bar]] etc.
                {
                    articleText = articleText.Replace(pl, "[[" + b.Replace("_", " ") + "]]");
                }
                else if (lb.StartsWith(la, StringComparison.Ordinal)) // target is substring of text e.g. [[Dog|Dogs]] --> [[Dog]]s
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
                    articleText = articleText.Replace(pl, "[[" + b.Substring(0, a.Length) + "]]" + b.Substring(a.Length));
                }
                else if(whitepaceTrimNeeded) // whitepsace trimming around the pipe to apply
                {
                    string newlink = "[[" + a + "|" + b + "]]";

                    if (newlink != pl)
                        articleText = articleText.Replace(pl, newlink);
                }
            }

            return articleText;
        }

        // Covered by: LinkTests.TestStickyLinks()
        // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_15#Link_simplification_too_greedy_-_eating_spaces -- disabled as genfix
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
            while(WikiRegexes.EmptyLink.IsMatch(articleText))
                articleText = WikiRegexes.EmptyLink.Replace(articleText, "");
            return WikiRegexes.EmptyTemplate.Replace(articleText, "");
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

        // Covered by: LinkTests.TestBulletExternalLinks()
        /// <summary>
        /// Adds bullet points to external links after "external links" header
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string BulletExternalLinks(string articleText)
        {
            int intStart = ExternalLinksSection.Match(articleText).Index;

            if(intStart > 0)
            {
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

        // Covered by: LinkTests.TestFixCategories()
        /// <summary>
        /// Fix common spacing/capitalisation errors in categories; remove diacritics and trailing whitespace from sortkeys (not leading whitespace)
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The modified article text.</returns>
        public static string FixCategories(string articleText)
        {
            CategoryStart = @"[[" + (Variables.Namespaces.ContainsKey(Namespace.Category) ? Variables.Namespaces[Namespace.Category] : "Category:");

            // Performance: only need to apply changes to portion of article containing categories
            Match cq = WikiRegexes.CategoryQuick.Match(articleText);

            if(cq.Success)
            {
                // Allow some characters before category start in case of excess opening braces
                int cutoff = Math.Max(0, cq.Index-2);
                string cats = articleText.Substring(cutoff);

                // fix extra brackets: three or more at end
                cats = Regex.Replace(cats, @"(" + Regex.Escape(CategoryStart) + @"[^\r\n\[\]{}<>]+\]\])\]+", "$1");
                // three or more at start
                cats = Regex.Replace(cats, @"\[+(?=" + Regex.Escape(CategoryStart) + @"[^\r\n\[\]{}<>]+\]\])", "");

                cats = WikiRegexes.LooseCategory.Replace(cats, LooseCategoryME);
                
                articleText = articleText.Substring(0, cutoff) + cats;
            }
            
            return articleText;
        }

        private static string LooseCategoryME(Match m)
        {
            if (!Tools.IsValidTitle(m.Groups[1].Value))
                return m.Value;

            string sortkey = m.Groups[2].Value;

            // diacritic removal in sortkeys on en-wiki/simple-wiki only
            if (Variables.LangCode.Equals("en") || Variables.LangCode.Equals("simple"))
                sortkey = Tools.CleanSortKey(sortkey);

            return CategoryStart + Tools.TurnFirstToUpper(CanonicalizeTitleRaw(m.Groups[1].Value, false).Trim().TrimStart(':')) +
                WordWhitespaceEndofline.Replace(sortkey, "$1") + "]]";
        }

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

        /// <summary>
        /// Extracts template using the given match.
        /// </summary>
        private static string ExtractTemplate(string articleText, Match m)
        {
            Regex theTemplate = new Regex(Regex.Escape(m.Groups[1].Value) + @"(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}}");
            string res = "";
            foreach (Match n in theTemplate.Matches(articleText))
            {
                if (n.Index == m.Index)
                {
                    res= theTemplate.Match(articleText).Value;
                    break;
                }
            }

            return res;
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
                    if (m2.Index.Equals(m.Index))
                    {
                        templateMatches.Add(m2);
                        break;
                    }
                }
            }

            return templateMatches;
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
        /// Delinks all bolded self links in the article
        /// </summary>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns></returns>
        private static string BoldedSelfLinks(string articleTitle, string articleText)
        {
            string escTitle = Regex.Escape(articleTitle);

            Regex plainLink = new Regex(@"'''\[\[\s*(" + escTitle + @"|" + Tools.TurnFirstToLower(escTitle) + @")\s*\]\]'''");
            Regex pipedLink = new Regex(@"'''\[\[\s*(?:" + escTitle + @"|" + Tools.TurnFirstToLower(escTitle) + @")\s*\|\s*([^\[\]]+?)\s*\]\]'''");

            articleText = plainLink.Replace(articleText, @"'''$1'''");
            articleText = pipedLink.Replace(articleText, @"'''$1'''");

            return articleText;
        }

        private static readonly Regex BracketedAtEndOfLine = new Regex(@" \(.*?\)$", RegexOptions.Compiled);
        private static readonly Regex BoldTitleAlready3 = new Regex(@"^\s*({{[^\{\}]+}}\s*)*'''('')?\s*\w", RegexOptions.Compiled);
        private static readonly Regex BoldTitleAlready4 = new Regex(@"^\s*'''", RegexOptions.Multiline);
        private static readonly Regex DfnTag = new Regex(@"<\s*dfn\s*>", RegexOptions.IgnoreCase);
        private static readonly Regex NihongoTitle = Tools.NestedTemplateRegex("nihongo title");
        // bio used on it-wiki, automatically bolds link
        private static readonly Regex NoBoldTitle = Tools.NestedTemplateRegex(new[] { "year article header", "bio" });

        // Covered by: BoldTitleTests
        /// <summary>
        /// '''Emboldens''' the first occurrence of the article title, if not already bold
        /// 1) Cleans up bolded self wikilinks
        /// 2) Cleans up self wikilinks
        /// 3) '''Emboldens''' the first occurrence of the article title
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The title of the article.</param>
        /// <param name="noChange">Value that indicated whether no change was made.</param>
        /// <returns>The modified article text.</returns>
        public string BoldTitle(string articleText, string articleTitle, out bool noChange)
        {
            noChange = true;
            if(NoBoldTitle.IsMatch(articleText))
                return articleText;

            HideText Hider2 = new HideText();
            HideText Hider3 = new HideText(true, true, true);

            bool includeonlyNoinclude = WikiRegexes.IncludeonlyNoinclude.IsMatch(articleText);

            // 1) clean up bolded self links first
            if(!includeonlyNoinclude)
                articleText = BoldedSelfLinks(articleTitle, articleText);

            // 2) Clean up self wikilinks
            string escTitle = Regex.Escape(articleTitle), escTitleNoBrackets = Regex.Escape(BracketedAtEndOfLine.Replace(articleTitle, ""));

            string articleTextAtStart = articleText, zerothSection = Tools.GetZerothSection(articleText);
            string restOfArticle = articleText.Substring(zerothSection.Length);
            string zerothSectionHidden, zerothSectionHiddenOriginal;

            // first check for any self links and no bold title, if found just convert first link to bold and return
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Includes_and_selflinks
            // don't apply if bold in lead section already or some noinclude transclusion business
            if(!includeonlyNoinclude && !SelfLinks(zerothSection, articleTitle).Equals(zerothSection))
            {
                // There's a limitation here in that we can't hide image descriptions that may be above lead sentence without hiding the self links we are looking to correct
                zerothSectionHidden = Hider2.HideMore(zerothSection, false, false, false);
                zerothSectionHiddenOriginal = zerothSectionHidden;
                zerothSectionHidden = SelfLinks(zerothSectionHidden, articleTitle);
                zerothSection = Hider2.AddBackMore(zerothSectionHidden);

                if (!zerothSectionHiddenOriginal.Equals(zerothSectionHidden))
                {
                    noChange = false;
                    return (zerothSection + restOfArticle);
                }
            }

            // Performance check: if article title not in zeroth section have nothing further to do
            if(zerothSection.IndexOf(BracketedAtEndOfLine.Replace(articleTitle, ""), StringComparison.OrdinalIgnoreCase) < 0)
                return articleTextAtStart;
            
            // 3) '''Emboldens''' the first occurrence of the article title

            // ignore date articles (date in American or international format), nihongo title
            if (WikiRegexes.Dates2.IsMatch(articleTitle) || WikiRegexes.Dates.IsMatch(articleTitle)
                || NihongoTitle.IsMatch(articleText))
                return articleTextAtStart;

            Regex boldTitleAlready1 = new Regex(@"'''\s*(" + escTitle + "|" + Tools.TurnFirstToLower(escTitle) + @")\s*'''");
            Regex boldTitleAlready2 = new Regex(@"'''\s*(" + escTitleNoBrackets + "|" + Tools.TurnFirstToLower(escTitleNoBrackets) + @")\s*'''");

            string articleTextNoInfobox = Tools.ReplaceWithSpaces(articleText, WikiRegexes.InfoBox.Matches(articleText));
            string zerothSectionNoInfobox = Tools.ReplaceWithSpaces(zerothSection, WikiRegexes.InfoBox.Matches(zerothSection));

            // if title in bold already exists in article, or paragraph starts with something in bold, don't change anything
            // ignore any bold in infoboxes
            if (boldTitleAlready1.IsMatch(articleTextNoInfobox) || boldTitleAlready2.IsMatch(articleTextNoInfobox)
                || BoldTitleAlready3.IsMatch(articleTextNoInfobox)
                || BoldTitleAlready4.IsMatch(zerothSectionNoInfobox) || DfnTag.IsMatch(zerothSection))
                return articleTextAtStart;

            // so no self links to remove, check for the need to add bold
            string articleTextNoTemplates = WikiRegexes.NestedTemplates.Replace(articleText, "");

            // first quick check: ignore articles with some bold in first 5% of article, ignoring infoboxes, dablinks etc.
            int fivepc = articleTextNoTemplates.Length / 20;

            if (articleTextNoTemplates.Substring(0, fivepc).Contains("'''"))
                return articleTextAtStart;

            Regex regexBoldNoBrackets = new Regex(@"([^\[]|^)(" + escTitleNoBrackets + "|" + Tools.TurnFirstToLower(escTitleNoBrackets) + ")([ ,.:;])");

            zerothSectionHidden = Hider3.HideMore(zerothSection);
            zerothSectionHiddenOriginal = zerothSectionHidden;

            // first try title with brackets removed
            zerothSectionHidden = regexBoldNoBrackets.Replace(zerothSectionHidden, "$1'''$2'''$3", 1);

            zerothSection = Hider3.AddBackMore(zerothSectionHidden);
            
            articleText = zerothSection + restOfArticle;

            // check that the bold added is the first bit in bold in the main body of the article
            if (!zerothSectionHiddenOriginal.Equals(zerothSectionHidden) && AddedBoldIsValid(articleText, escTitleNoBrackets))
            {
                noChange = false;
                return articleText;
            }

            return articleTextAtStart;
        }
        
        private static string SelfLinks(string zerothSection, string articleTitle)
        {
            string zerothSectionOriginal = zerothSection, escTitle = Regex.Escape(articleTitle);
            
            if (!zerothSection.Contains("'''" + articleTitle + "'''"))
            {
                Regex r1 = new Regex(@"\[\[\s*" + escTitle + @"\s*\]\]");
                Regex r3 = new Regex(@"\[\[\s*" + escTitle + @"\s*\|\s*([^\[\]]+?)\s*\]\]");
                
                zerothSection = r1.Replace(zerothSection, "'''" + articleTitle + @"'''");
                zerothSection = r3.Replace(zerothSection, "'''$1'''");
            }

            if (zerothSectionOriginal.Equals(zerothSection) && !zerothSection.Contains("'''" + Tools.TurnFirstToLower(articleTitle) + "'''"))
            {
                Regex r2 = new Regex(@"\[\[\s*" + Tools.TurnFirstToLower(escTitle) + @"\s*\]\]");
                Regex r4 = new Regex(@"\[\[\s*" + Tools.TurnFirstToLower(escTitle) + @"\s*\|\s*([^\[\]]+?)\s*\]\]");
                
                zerothSection = r2.Replace(zerothSection, "'''" + Tools.TurnFirstToLower(articleTitle) + @"'''");
                zerothSection = r4.Replace(zerothSection, "'''$1'''");
            }
            
            return zerothSection;
        }

        private static readonly Regex RegexFirstBold = new Regex(@"^(.*?)'''", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Checks that the bold just added to the article is the first bold in the article, and that it's within the first 5% of the HideMore article OR immediately after the infobox
        /// </summary>
        private bool AddedBoldIsValid(string articleText, string escapedTitle)
        {
            HideText Hider2 = new HideText(true, true, true);
            Regex RegexBoldAdded = new Regex(@"^(.*?)'''(" + escapedTitle + @")", RegexOptions.Singleline | RegexOptions.IgnoreCase);

            int boldAddedPos = RegexBoldAdded.Match(articleText).Groups[2].Index;

            int firstBoldPos = RegexFirstBold.Match(articleText).Length;

            articleText = WikiRegexes.NestedTemplates.Replace(articleText, "");

            articleText = Hider2.HideMore(articleText);

            // was bold added in first 5% of article?
            bool inFirst5Percent = false;

            int articlelength = articleText.Length;

            if (articlelength > 5)
                inFirst5Percent = articleText.Trim().Substring(0, Math.Max(articlelength / 20, 5)).Contains("'''");

            articleText = Hider2.AddBackMore(articleText);
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

            noChange = newText.Equals(articleText);

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

            return SortMetaData(articleText, articleTitle, false); //Sort metadata ordering so general fixes do not need to be enabled
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

                // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Replacing_categoring_and_keeping_pipes
                if (!removeSortKey)
                    newCategory = Variables.Namespaces[Namespace.Category] + newCategory + "$1";
                else
                    newCategory = Variables.Namespaces[Namespace.Category] + newCategory + @"]]";

                articleText = Regex.Replace(articleText, oldCategory, newCategory);
            }

            noChange = (testText.Equals(articleText));

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

            noChange = (testText.Equals(articleText));

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
            // for performance only search article from first category
            Match cq = WikiRegexes.CategoryQuick.Match(articleText);

            if(cq.Success)
            {
                Regex anyCategory = new Regex(@"\[\[\s*" + Variables.NamespacesCaseInsensitive[Namespace.Category] + @"\s*" + Regex.Escape(categoryName) + @"\s*(?:|\|([^\|\]]*))\s*\]\]", RegexOptions.IgnoreCase);

                return anyCategory.IsMatch(articleText.Substring(cq.Index));
            }

            return false;
        }

        /// <summary>
        /// Returns a concatenated string of all categories in the article
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private static string GetCats(string articleText)
        {
            StringBuilder sb = new StringBuilder();

            foreach(Match m in WikiRegexes.Category.Matches(articleText))
            {
                sb.Append(m.Value);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns whether the article is missing a defaultsort (i.e. criteria match so that defaultsort would be added)
        /// </summary>
        /// <param name="articletext"></param>
        /// <param name="articletitle"></param>
        /// <returns></returns>
        public static bool MissingDefaultSort(string articletext, string articletitle)
        {
            bool Skip, DSbefore = WikiRegexes.Defaultsort.IsMatch(articletext);
            if(!DSbefore)
            {
                articletext = ChangeToDefaultSort(articletext, articletitle, out Skip);
                return (!Skip && WikiRegexes.Defaultsort.IsMatch(articletext));
            }

            return false;
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
        /// <param name="matches">Number of categories with the same or no sortkey</param>
        /// <returns></returns>
        public static string GetCategorySort(string articleText, string articleTitle, out int matches)
        {
            string sort = "";
            bool allsame = true;
            matches = 0;

            articleText = articleText.Replace(@"{{PAGENAME}}", articleTitle);
            articleText = articleText.Replace(@"{{subst:PAGENAME}}", articleTitle);

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
        /// Skips pages using &lt;noinclude&gt;, &lt;includeonly&gt; etc.
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

            if (NoIncludeIncludeOnlyProgrammingElement(articleText))
                return articleText;

            // count categories
            int matches;

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_12#defaultsort_adding_namespace
            if (!Namespace.IsMainSpace(articleTitle))
                articleTitle = Tools.RemoveNamespaceString(articleTitle);

            MatchCollection ds = WikiRegexes.Defaultsort.Matches(articleText);
            if (ds.Count > 1 || (ds.Count == 1 && !ds[0].Value.ToUpper().Contains("DEFAULTSORT")))
            {
                bool allsame2 = false;
                string lastvalue = "";
                // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Detect_multiple_DEFAULTSORT
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

            if (Variables.LangCode.Equals("en"))
                articleText = WikiRegexes.Defaultsort.Replace(articleText, DefaultsortME);

            // match again, after normalisation
            ds = WikiRegexes.Defaultsort.Matches(articleText);

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_9#AWB_didn.27t_fix_special_characters_in_a_pipe
            articleText = FixCategories(articleText);

            if (!restrictDefaultsortChanges)
            {
                // AWB's generation of its own sortkey may be incorrect for people, provide option not to insert in this situation
                if (ds.Count == 0)
                {
                    string sort = GetCategorySort(articleText, articleTitle, out matches);
                    // So that this does not get confused by sort keys of "*", " ", etc.
                    // MW bug: DEFAULTSORT does not treat leading spaces the same way as categories do
                    // if all existing categories use a suitable sortkey, insert that rather than generating a new one
                    // GetCatSortkey just returns articleTitle if cats do not have sortkey, so do not accept this here
                    if (sort.Length > 4 && matches > 1 && !sort.StartsWith(" "))
                    {
                        // remove keys from categories
                        articleText = WikiRegexes.Category.Replace(articleText, "[["
                                                                   + Variables.Namespaces[Namespace.Category] + "$1]]");

                        // set the defaultsort to the existing unique category sort value
                        // do not add a defaultsort if cat sort was the same as article title, now not case sensitive
                        if ((sort != articleTitle && Tools.FixupDefaultSort(sort).ToLower() != articleTitle.ToLower())
                            || (Tools.RemoveDiacritics(sort) != sort && !IsArticleAboutAPerson(articleText, articleTitle, false)))
                            articleText += Tools.Newline("{{DEFAULTSORT:") + Tools.FixupDefaultSort(sort) + "}}";
                    }

                    // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Add_defaultsort_to_pages_with_special_letters_and_no_defaultsort
                    // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Human_DEFAULTSORT
                    articleText = DefaultsortTitlesWithDiacritics(articleText, articleTitle, matches, IsArticleAboutAPerson(articleText, articleTitle, true));
                }
                else if (ds.Count == 1) // already has DEFAULTSORT
                {
                    string s = Tools.FixupDefaultSort(ds[0].Groups[1].Value.TrimStart('|'), (HumanDefaultSortCleanupRequired(ds[0]) && IsArticleAboutAPerson(articleText, articleTitle, true))).Trim();

                    // do not change DEFAULTSORT just for casing
                    if (!s.ToLower().Equals(ds[0].Groups[1].Value.ToLower()) && s.Length > 0 && !restrictDefaultsortChanges)
                        articleText = articleText.Replace(ds[0].Value, "{{DEFAULTSORT:" + s + "}}");

                    // get key value again in case replace above changed it
                    ds = WikiRegexes.Defaultsort.Matches(articleText);
                    string defaultsortKey = ds[0].Groups["key"].Value;

                    //Removes any explicit keys that are case insensitively the same as the default sort (To help tidy up on pages that already have defaultsort)
                    articleText = ExplicitCategorySortkeys(articleText, defaultsortKey);
                }
            }
            noChange = testText.Equals(articleText);
            return articleText;
        }
        
        /// <summary>
        /// Returns whether human name defaultsort cleanup required: contains apostrophe or unspaced comma
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        private static bool HumanDefaultSortCleanupRequired(Match ds)
        {
            return (ds.Groups[1].Value.Contains("'") || Regex.IsMatch(ds.Groups[1].Value, @"\w,\w"));
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
                    || defaultsortKey.StartsWith(explicitKey) || Tools.NestedTemplateRegex("PAGENAME").IsMatch(explicitKey))
                {
                    articleText = articleText.Replace(m.Value,
                                                      "[[" + Variables.Namespaces[Namespace.Category] + m.Groups[1].Value + "]]");
                }
            }
            return (articleText);
        }

        /// <summary>
        /// If title has diacritics, no defaultsort added yet, adds a defaultsort with cleaned up title as sort key
        /// If article is about a person, generates human name sortkey
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="categories">Number of categories on page</param>
        /// <param name="articleAboutAPerson">Whether the article is about a person</param>
        /// <returns>The article text possibly using defaultsort.</returns>
        private static string DefaultsortTitlesWithDiacritics(string articleText, string articleTitle, int categories, bool articleAboutAPerson)
        {
            // need some categories and no defaultsort, and a sortkey not the same as the article title
            if (categories > 0 && !WikiRegexes.Defaultsort.IsMatch(articleText))
            {
                // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Human_DEFAULTSORT
                // if article is about a person, attempt to add a surname, forenames sort key rather than the tidied article title
                string sortkey = articleAboutAPerson ? Tools.MakeHumanCatKey(articleTitle, articleText) : Tools.FixupDefaultSort(articleTitle);

                // sortkeys now not case sensitive
                if (!sortkey.ToLower().Equals(articleTitle.ToLower()) || Tools.RemoveDiacritics(articleTitle) != articleTitle)
                {
                    articleText += Tools.Newline("{{DEFAULTSORT:") + sortkey + "}}";

                    return (ExplicitCategorySortkeys(articleText, sortkey));
                }
            }

            return articleText;
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

        /// <summary>
        /// Adds [[Category:XXXX births]], [[Category:XXXX deaths]] to articles about people where available, for en-wiki only
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="noChange"></param>
        /// <returns></returns>
        [Obsolete]
        [CLSCompliant(false)]
        public string FixPeopleCategories(string articleText, string articleTitle, out bool noChange)
        {
            string newText = FixPeopleCategories(articleText, articleTitle);

            noChange = newText.Equals(articleText);

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

            noChange = newText.Equals(articleText);

            return newText;
        }

        private static readonly Regex LongWikilink = new Regex(@"\[\[[^\[\]\|]{11,}(?:\|[^\[\]]+)?\]\]");
        private static readonly Regex YearPossiblyWithBC = new Regex(@"\d{3,4}(?![\ds])(?: BC)?");
        private static readonly Regex ThreeOrFourDigitNumber = new Regex(@"[0-9]{3,4}");
        private static readonly Regex DiedOrBaptised = new Regex(@"(^.*?)((?:&[nm]dash;|—|–|;|[Dd](?:ied|\.)|baptised).*)");
        private static readonly Regex NotCircaTemplate = new Regex(@"{{(?!(?:[Cc]irca|[Ff]l\.?))[^{]*?}}");
        private static readonly Regex AsOfText = new Regex(@"\bas of\b");
        private static readonly Regex FloruitTemplate = Tools.NestedTemplateRegex(new [] {"fl", "fl."});

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
        /// When page is not mainspace, adds [[:Category rather than [[Category
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="parseTalkPage"></param>
        /// <returns></returns>
        public static string FixPeopleCategories(string articleText, string articleTitle, bool parseTalkPage)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            // Performance: apply births/deaths category check on categories string, not whole article
            string cats = GetCats(articleText);

            bool dolmatch = WikiRegexes.DeathsOrLivingCategory.IsMatch(cats),
            bimatch = WikiRegexes.BirthsCategory.IsMatch(cats);

            // no work to do if already has a birth and a death/living cat
            if(dolmatch && bimatch)
                return YearOfBirthDeathMissingCategory(articleText, cats);

            // over 20 references or long and not DOB/DOD categorised at all yet: implausible
            if ((articleText.Length > 15000 && !bimatch && !dolmatch) || (!dolmatch && WikiRegexes.Refs.Matches(articleText).Count > 20))
                return YearOfBirthDeathMissingCategory(articleText, cats);

            string articleTextBefore = articleText;
            int catCount = WikiRegexes.Category.Matches(articleText).Count;

            // get the zeroth section (text upto first heading)
            string zerothSection = Tools.GetZerothSection(articleText);

            // remove references and long wikilinks (but allow an ISO date) that may contain false positives of birth/death date
            zerothSection = WikiRegexes.Refs.Replace(zerothSection, " ");
            zerothSection = LongWikilink.Replace(zerothSection, " ");

            // ignore dates from dated maintenance tags etc.
            zerothSection = WikiRegexes.NestedTemplates.Replace(zerothSection, m2=> Tools.GetTemplateParameterValue(m2.Value, "date").Length > 0 ? "" : m2.Value);
            zerothSection = WikiRegexes.TemplateMultiline.Replace(zerothSection, m2=> Tools.GetTemplateParameterValue(m2.Value, "date").Length > 0 ? "" : m2.Value);

            string StartCategory = Tools.Newline(@"[[" + (Namespace.IsMainSpace(articleTitle) ? "" : ":") + @"Category:");
            string yearstring, yearFromInfoBox = "", sort = GetCategorySort(articleText);

            bool alreadyUncertain = false;

            // scrape any infobox for birth year
            string fromInfoBox = GetInfoBoxFieldValue(zerothSection, WikiRegexes.InfoBoxDOBFields);

            // ignore as of dates
            if (AsOfText.IsMatch(fromInfoBox))
                fromInfoBox = fromInfoBox.Substring(0, AsOfText.Match(fromInfoBox).Index);

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
                        if (UncertainWordings.IsMatch(birthpart) || alreadyUncertain || FloruitTemplate.IsMatch(birthpart))
                        {
                            if (!CategoryMatch(articleText, YearOfBirthMissingLivingPeople) && !CategoryMatch(articleText, YearOfBirthUncertain))
                                articleText += StartCategory + YearOfBirthUncertain + CatEnd(sort);
                        }
                        else // after removing dashes, birthpart must still contain year
                            if (!birthpart.Contains(@"?") && Regex.IsMatch(birthpart, @"\d{3,4}"))
                                yearstring = m.Groups[1].Value;
                    }
                }

                // per [[:Category:Living people]], don't apply birth category if born > 121 years ago
                // validate a YYYY date is not in the future
                if (!string.IsNullOrEmpty(yearstring) && yearstring.Length > 2
                    && (!YearOnly.IsMatch(yearstring) || Convert.ToInt32(yearstring) <= DateTime.Now.Year)
                    && !(articleText.Contains(CategoryLivingPeople) && Convert.ToInt32(yearstring) < (DateTime.Now.Year - 121)))
                    articleText += StartCategory + yearstring + " births" + CatEnd(sort);
            }

            // scrape any infobox
            yearFromInfoBox = "";
            fromInfoBox = GetInfoBoxFieldValue(articleText, WikiRegexes.InfoBoxDODFields);

            if (fromInfoBox.Length > 0 && !UncertainWordings.IsMatch(fromInfoBox))
                yearFromInfoBox = YearPossiblyWithBC.Match(fromInfoBox).Value;

            if (!WikiRegexes.DeathsOrLivingCategory.IsMatch(RemoveCategory(YearofDeathMissing, articleText)) && (PersonYearOfDeath.IsMatch(zerothSection) || WikiRegexes.DeathDate.IsMatch(zerothSection)
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
                    && (!YearOnly.IsMatch(yearstring) || Convert.ToInt32(yearstring) <= DateTime.Now.Year))
                    articleText += StartCategory + yearstring + " deaths" + CatEnd(sort);
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
                        if (!UncertainWordings.IsMatch(birthpart) && !ReignedRuledUnsure.IsMatch(m.Value) && !Regex.IsMatch(birthpart, @"(?:[Dd](?:ied|\.)|baptised)")
                            && !FloruitTemplate.IsMatch(birthpart))
                            articleText += StartCategory + birthyear + @" births" + CatEnd(sort);
                        else
                            if (UncertainWordings.IsMatch(birthpart) && !CategoryMatch(articleText, YearOfBirthMissingLivingPeople) && !CategoryMatch(articleText, YearOfBirthUncertain))
                                articleText += StartCategory + YearOfBirthUncertain + CatEnd(sort);
                    }

                    if (!UncertainWordings.IsMatch(deathpart) && !ReignedRuledUnsure.IsMatch(m.Value) && !Regex.IsMatch(deathpart, @"[Bb](?:orn|\.)") && !Regex.IsMatch(birthpart, @"[Dd](?:ied|\.)")
                        && (!WikiRegexes.DeathsOrLivingCategory.IsMatch(articleText) || CategoryMatch(articleText, YearofDeathMissing)))
                        articleText += StartCategory + deathyear + @" deaths" + CatEnd(sort);
                }
            }

            // do this check last as IsArticleAboutAPerson can be relatively slow
            if (!articleText.Equals(articleTextBefore) && !IsArticleAboutAPerson(articleTextBefore, articleTitle, parseTalkPage))
                return YearOfBirthDeathMissingCategory(articleTextBefore, cats);

            // {{uncat}} --> {{Improve categories}} if we've added cats
            if (WikiRegexes.Category.Matches(articleText).Count > catCount && WikiRegexes.Uncat.IsMatch(articleText)
                && !WikiRegexes.CatImprove.IsMatch(articleText))
                articleText = Tools.RenameTemplate(articleText, WikiRegexes.Uncat.Match(articleText).Groups[1].Value, "Improve categories");

            return YearOfBirthDeathMissingCategory(articleText, GetCats(articleText));
        }

        private static string CatEnd(string sort)
        {
            return ((sort.Length > 3) ? "|" + sort : "") + "]]";
        }

        private const string YearOfBirthMissingLivingPeople = "Year of birth missing (living people)",
        YearOfBirthMissing = "Year of birth missing",
        YearOfBirthUncertain = "Year of birth uncertain",
        YearofDeathMissing = "Year of death missing";

        private static readonly Regex Cat4YearBirths = new Regex(@"\[\[Category:\d{4} births\s*(?:\||\]\])");
        private static readonly Regex Cat4YearDeaths = new Regex(@"\[\[Category:\d{4} deaths\s*(?:\||\]\])");

        /// <summary>
        /// Removes birth/death missing categories when xxx births/deaths category also present
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns>The updated article text</returns>
        private static string YearOfBirthDeathMissingCategory(string articleText, string cats)
        {
            // if there is a 'year of birth missing' and a year of birth, remove the 'missing' category
            if(Cat4YearBirths.IsMatch(cats))
            {
                if (CategoryMatch(cats, YearOfBirthMissingLivingPeople))
                    articleText = RemoveCategory(YearOfBirthMissingLivingPeople, articleText);
                else if (CategoryMatch(cats, YearOfBirthMissing))
                    articleText = RemoveCategory(YearOfBirthMissing, articleText);
            }

            // if there's a 'year of birth missing' and a 'year of birth uncertain', remove the former
            if (CategoryMatch(cats, YearOfBirthMissing) && CategoryMatch(cats, YearOfBirthUncertain))
                articleText = RemoveCategory(YearOfBirthMissing, articleText);

            // if there's a year of death and a 'year of death missing', remove the latter
            if (Cat4YearDeaths.IsMatch(cats) && CategoryMatch(cats, YearofDeathMissing))
                articleText = RemoveCategory(YearofDeathMissing, articleText);

            return articleText;
        }

        /// <summary>
        /// Returns the value of the given fields from the page's infobox, where available
        /// Returns a null string if the input article has no infobox, or the input field regex doesn't match on the infobox found
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="fields">List of infobox fields to search</param>
        /// <returns>Field value</returns>
        public static string GetInfoBoxFieldValue(string articleText, List<string> fields)
        {
            string infoBox = WikiRegexes.InfoBox.Match(articleText).Value;

            // clean out references and comments
            infoBox = WikiRegexes.Comments.Replace(infoBox, "");
            infoBox = WikiRegexes.Refs.Replace(infoBox, "");

            List<string> FieldsBack = Tools.GetTemplateParametersValues(infoBox, fields, true);

            foreach (string f in FieldsBack)
            {
                if (f.Length > 0)
                    return f;
            }

            return "";
        }

        /// <summary>
        /// Returns the value of the given field from the page's infobox, where available
        /// Returns a null string if the input article has no infobox, or the input field regex doesn't match on the infobox found
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="field">infobox field to search</param>
        /// <returns>Field value</returns>
        public static string GetInfoBoxFieldValue(string articleText, string field)
        {
            return GetInfoBoxFieldValue(articleText, new List<string>(new[] { field }));
        }

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

            bool AmericanDate = WikiRegexes.AmericanDates.IsMatch(dateandage);

            string ISODate = Tools.ConvertDate(dateandage, DateLocale.ISO);

            if (ISODate.Equals(dateandage) && !WikiRegexes.ISODates.IsMatch(dateandage))
                return original;

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
            articleText = articleText.Replace("[[zh-tw:", "[[zh:");
            articleText = articleText.Replace("[[nb:", "[[no:");
            return articleText.Replace("[[dk:", "[[da:");
        }

        /// <summary>
        /// Returns the number of &lt;ref&gt; references in the input text, excluding grouped refs
        /// </summary>
        /// <param name="arcticleText"></param>
        /// <returns></returns>
        private static int TotalRefsNotGrouped(string arcticleText)
        {
            return WikiRegexes.Refs.Matches(arcticleText).Count - WikiRegexes.RefsGrouped.Matches(arcticleText).Count;
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

        private static readonly Regex MultipleIssuesUndatedTags = new Regex(@"({{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues\s*(?:\|[^{}]*(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}[^{}]*)?|\|)\s*)(?![Ee]xpert)" + WikiRegexes.MultipleIssuesTemplatesString + @"\s*(\||}})", RegexOptions.Compiled);
        private static readonly Regex MultipleIssuesDateRemoval = new Regex(@"(?<={{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues\s*(?:\|[^{}]*?)?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}[^{}]*?){0,4}\|[^{}\|]{3,}?)\b(?i)date(?<!.*out of date)", RegexOptions.Compiled);
        private static readonly Regex NoFootnotes = Tools.NestedTemplateRegex("no footnotes");
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
            List<string> alltemplates = Parsers.GetAllTemplates(articleText);

            foreach (KeyValuePair<Regex, string> k in RegexConversion)
            {
                articleText = k.Key.Replace(articleText, k.Value);
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
            articleText = SectionTemplates.Replace(articleText, SectionTemplateConversionsME);

            bool mifound = TemplateExists(alltemplates, WikiRegexes.MultipleIssues);

            if(mifound)
            {
                articleText = WikiRegexes.MultipleIssues.Replace(articleText, m =>
                                                             {
                                                                 return Tools.RemoveExcessTemplatePipes(m.Value);
                                                             });
                // add date to any undated tags within {{Multiple issues}} (loop due to lookbehind in regex)
                while (MultipleIssuesUndatedTags.IsMatch(articleText))
                    articleText = MultipleIssuesUndatedTags.Replace(articleText, "$1$2={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}$3");

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
                else if(mifound) // could be unreferenced parameter in old style MI template
                    articleText = WikiRegexes.MultipleIssues.Replace(articleText, m => Tools.RenameTemplateParameter(m.Value, "unreferenced", "BLP unsourced"));

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

            if(TemplateExists(alltemplates, SectionMergedTemplatesR))
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
        
        /// <summary>
        /// Returns whether the given regex matches any of the (first name upper) templates in the given list
        /// </summary>
        private static bool TemplateExists(List<string> templatesFound, Regex r)
        {
            return templatesFound.Any(s => r.IsMatch(@"{{" + s + "|}}"));
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

        private static readonly Regex TemplateParameter2 = new Regex(@" \{\{\{2\|\}\}\}", RegexOptions.Compiled);

        /// <summary>
        /// Substitutes some user talk templates
        /// </summary>
        /// <param name="talkPageText">The wiki text of the talk page.</param>
        /// <param name="talkPageTitle">The wiki talk page title</param>
        /// <param name="userTalkTemplatesRegex">Dictoinary of regexes matching template calls to substitute</param>
        /// <returns>The updated article text</returns>
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
        /// If necessary, adds/removes various cleanup tags such as wikify, stub, ibid
        /// </summary>
        public string Tagger(string articleText, string articleTitle, bool restrictOrphanTagging, out bool noChange, ref string summary)
        {
            string newText = Tagger(articleText, articleTitle, restrictOrphanTagging, ref summary);

            newText = TagUpdater(newText);

            noChange = newText.Equals(articleText);

            return newText;
        }

        private static readonly CategoriesOnPageNoHiddenListProvider CategoryProv = new CategoriesOnPageNoHiddenListProvider
                                                                                    {
                                                                                        Limit = 10
                                                                                    };
        private static readonly LinksOnPageListProvider LinksOnPageProv = new LinksOnPageListProvider();

        private readonly List<string> tagsRemoved = new List<string>();
        private readonly List<string> tagsAdded = new List<string>();
        private static readonly Regex ImproveCategories = Tools.NestedTemplateRegex("improve categories");
        private static readonly Regex ProposedDeletionDatedEndorsed = Tools.NestedTemplateRegex( new [] {"Proposed deletion/dated", "Proposed deletion endorsed", "Prod blp/dated" });
        private static readonly Regex Unreferenced = Tools.NestedTemplateRegex("unreferenced");
        private static readonly Regex Drugbox = Tools.NestedTemplateRegex(new[] { "Drugbox", "Chembox", "PBB", "PBB Summary" });
        private static readonly Regex MinorPlanetListFooter = Tools.NestedTemplateRegex("MinorPlanetListFooter");
        private static readonly Regex BulletedText = new Regex(@"\r\n[\*#: ].*");

        //TODO:Needs re-write
        /// <summary>
        /// If necessary, adds/removes wikify or stub tag
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">The article title.</param>
        /// <param name="restrictOrphanTagging"></param>
        /// <param name="summary"></param>
        /// <returns>The tagged article.</returns>
        public string Tagger(string articleText, string articleTitle, bool restrictOrphanTagging, ref string summary)
        {
            if(!TaggerPermitted(articleText, articleTitle))
                return articleText;

            tagsRemoved.Clear();
            tagsAdded.Clear();
            int tagsrenamed = 0;

            // Performance: get all templates so most template checks can be against this rather than whole article text
            // Due to old-style Multiple issues template need to add full value of any MI templates back in
            string templates = String.Join(" ", GetAllTemplates(articleText).Select(s => "{{" + s + "}}").ToArray());
            
            if(WikiRegexes.MultipleIssues.IsMatch(templates))
            {
                foreach(Match mi in WikiRegexes.MultipleIssues.Matches(articleText))
                    templates += mi.Value;
            }

            string commentsStripped = WikiRegexes.Comments.Replace(articleText, "");
            string commentsCategoriesStripped = WikiRegexes.Category.Replace(commentsStripped, "");
            if(WikiRegexes.Defaultsort.IsMatch(templates))
                commentsCategoriesStripped = WikiRegexes.Defaultsort.Replace(commentsCategoriesStripped, "");
            Sorter.Interwikis(ref commentsStripped, false);

            // remove stub tags from long articles, don't move section stubs
            if(WikiRegexes.Stub.IsMatch(templates) && WikiRegexes.Stub.IsMatch(commentsStripped))
            {
                // bulleted or indented text should weigh less than simple text.
                // for example, actor stubs may contain large filmographies
                string crapStripped = BulletedText.Replace(WikiRegexes.NestedTemplates.Replace(commentsCategoriesStripped, " "), "");
                int words = (Tools.WordCount(commentsCategoriesStripped, 999) + Tools.WordCount(crapStripped, 999)) / 2;
                if(words > StubMaxWordCount)
                {
                    articleText = WikiRegexes.Stub.Replace(articleText, StubChecker).Trim();

                    if(Variables.LangCode.Equals("ar"))
                    {
                        tagsRemoved.Add("بذرة");
                    }
                    else if(Variables.LangCode.Equals("arz"))
                    {
                        tagsRemoved.Add("تقاوى");
                    }
                    else if(Variables.LangCode.Equals("hy"))
                    {
                        tagsRemoved.Add("Անավարտ");
                    }
                    else
                    {
                        tagsRemoved.Add("stub");
                    }
                }
            }

            // refresh
            if(tagsRemoved.Count > 0)
            {
                commentsStripped = WikiRegexes.Comments.Replace(articleText, "");
                commentsCategoriesStripped = WikiRegexes.Category.Replace(commentsStripped, "");
            }

            //remove disambiguation if disambiguation cleanup exists (en-wiki only)
            if (Variables.LangCode.Equals("en") && WikiRegexes.DisambigsCleanup.IsMatch(templates) && WikiRegexes.DisambigsCleanup.IsMatch(commentsStripped))
            {
                articleText = WikiRegexes.DisambigsGeneral.Replace(articleText, "").Trim();
            }

            // do orphan tagging before template analysis for categorisation tags
            articleText = TagOrphans(articleText, articleTitle, restrictOrphanTagging);

            articleText = TagRefsIbid(articleText);

            articleText = TagEmptySection(articleText);

            int totalCategories;
            // ignore commented out wikilinks, and any in {{Proposed deletion/dated}}
            string forLinkCount = commentsStripped;
            if(ProposedDeletionDatedEndorsed.IsMatch(templates))
                forLinkCount = ProposedDeletionDatedEndorsed.Replace(forLinkCount, "");

            // discount persondata, comments, infoboxes and categories from wikify/underlinked and stub evaluation
            string lengthtext = commentsCategoriesStripped;
            if(WikiRegexes.Persondata.IsMatch(templates))
                lengthtext = WikiRegexes.Persondata.Replace(commentsCategoriesStripped, "");
            if(WikiRegexes.InfoBox.IsMatch(templates))
                lengthtext = WikiRegexes.InfoBox.Replace(lengthtext, "");
            if(Drugbox.IsMatch(templates))
                lengthtext = Drugbox.Replace(lengthtext, "");
            if(WikiRegexes.ReferenceList.IsMatch(templates))
                lengthtext = WikiRegexes.ReferenceList.Replace(lengthtext, "");

            int length = lengthtext.Length + 1;
            int linkLimit = (int)(0.0025 * length)+1;
            int wikiLinkCount = Tools.LinkCount(forLinkCount, linkLimit);
            bool underlinked = (wikiLinkCount < 0.0025 * length);

            #if DEBUG || UNITTEST
            if (Globals.UnitTestMode)
            {
                totalCategories = Globals.UnitTestIntValue;
            }
            else
            #endif
            {
                // stubs add non-hidden stub categories, don't count these in categories count
                // also don't count "Proposed deletion..." cats
                // limitation: in the unlikely event that the article has only redlinked cats then it is {{uncat}} but we won't tag it as such
                totalCategories = RegularCategories(commentsStripped, false).Count;

                // templates may add categories to page that are not [[Category...]] links, so use API call for accurate Category count
                if(totalCategories == 0)
                    totalCategories = RegularCategories(CategoryProv.MakeList(new[] { articleTitle })).Count;

                // If no mainspace links found, use API call to get link count (filter to mainspace links only), as we will not be counting any links transcluded from templates
                if(wikiLinkCount == 0)
                    wikiLinkCount = LinksOnPageProv.MakeList(new[] { articleTitle }).Where(l => (l.NameSpaceKey == Namespace.Mainspace)).Count();
            }

            // remove dead end if > 0 wikilinks on page
            if (wikiLinkCount > 0 && WikiRegexes.DeadEnd.IsMatch(templates))
            {
                if (Variables.LangCode.Equals("ar") || Variables.LangCode.Equals("arz"))
                    articleText = WikiRegexes.DeadEnd.Replace(articleText, "");
                else
                    articleText = WikiRegexes.DeadEnd.Replace(articleText, m => Tools.IsSectionOrReasonTemplate(m.Value, articleText) ? m.Value : m.Groups[1].Value).TrimStart();

                if (!WikiRegexes.DeadEnd.IsMatch(articleText))
                {
                    if (Variables.LangCode.Equals("ar"))
                    {
                        tagsRemoved.Add("نهاية مسدودة");
                    }
                    else if (Variables.LangCode.Equals("arz"))
                    {
                        tagsRemoved.Add("نهاية مسدودة");
                    }
                    else
                    {
                        tagsRemoved.Add("deadend");
                    }
                }
            }

            if (length <= 300 && !WikiRegexes.Stub.IsMatch(commentsCategoriesStripped) &&
                !WikiRegexes.Disambigs.IsMatch(commentsCategoriesStripped) && !WikiRegexes.SIAs.IsMatch(commentsCategoriesStripped) && !WikiRegexes.NonDeadEndPageTemplates.IsMatch(commentsCategoriesStripped))
            {
                // add stub tag. Exclude pages their title starts with "List of..."
                if (!ListOf.IsMatch(articleTitle) && !WikiRegexes.MeaningsOfMinorPlanetNames.IsMatch(articleTitle))
                {
                    if (Variables.LangCode.Equals("ar"))
                    {
                        articleText += Tools.Newline("{{بذرة}}", 3);
                        tagsAdded.Add("بذرة");
                    }
                    else if (Variables.LangCode.Equals("arz"))
                    {
                        articleText += Tools.Newline("{{تقاوى}}", 3);
                        tagsAdded.Add("تقاوى");
                    }
                    else if (Variables.LangCode.Equals("hy"))
                    {
                        articleText += Tools.Newline("{{Անավարտ}}", 3);
                        tagsAdded.Add("Անավարտ");
                    }
                    else
                    {
                        articleText += Tools.Newline("{{stub}}", 3);
                        tagsAdded.Add("stub");
                    }
                    commentsStripped = WikiRegexes.Comments.Replace(articleText, "");
                }
            }

            // rename existing {{improve categories}} else add uncategorized tag
            if (totalCategories == 0 && ImproveCategories.IsMatch(templates))
            {
                articleText = Tools.RenameTemplate(articleText, "improve categories", "Uncategorized");
                templates = Tools.RenameTemplate(templates, "improve categories", "Uncategorized");
            }

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Archive_19#AWB_problems
            // nl wiki doesn't use {{Uncategorized}} template
            // prevent wictionary redirects from being tagged as uncategorised
            if (totalCategories == 0
                && !WikiRegexes.Uncat.IsMatch(templates)
                && Variables.LangCode != "nl")
            {
                // bulleted or indented text should weigh less than simple text.
                // for example, actor stubs may contain large filmographies
                string crapStripped = BulletedText.Replace(WikiRegexes.NestedTemplates.Replace(commentsCategoriesStripped, " "), "");
                int words = (Tools.WordCount(commentsCategoriesStripped, 10) + Tools.WordCount(crapStripped, 10)) / 2;

                if(words > 6)
                {
                    if (WikiRegexes.Stub.IsMatch(commentsStripped))
                    {
                        // add uncategorized stub tag
                        if (Variables.LangCode.Equals("ar"))
                        {
                            articleText += Tools.Newline("{{بذرة غير مصنفة|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[تصنيف:مقالات غير مصنفة|غير مصنفة]]");
                        }
                        else if (Variables.LangCode.Equals("arz"))
                        {
                            articleText += Tools.Newline("{{تقاوى مش متصنفه|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[قالب:تقاوى مش متصنفه|تقاوى مش متصنفه]]");
                        }
                        else if(Variables.LangCode.Equals("hy")) // same template for uncat and uncat stub
                        {
                            articleText += Tools.Newline("{{Կատեգորիա չկա|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("Կատեգորիա չկա");
                        }
                        else if(Variables.LangCode.Equals("sv")) // same template for uncat and uncat stub
                        {
                            articleText += Tools.Newline("{{Okategoriserad|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[Mall:Okategoriserad|okategoriserad]]");
                        }
                        else
                        {
                            articleText += Tools.Newline("{{Uncategorized stub|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[CAT:UNCATSTUBS|uncategorised]]");
                        }
                    }
                    else
                    {
                        if (Variables.LangCode.Equals("ar"))
                        {
                            articleText += Tools.Newline("{{غير مصنفة|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[CAT:UNCAT|مقالات غير مصنفة]]");
                        }
                        else if (Variables.LangCode.Equals("arz"))
                        {
                            articleText += Tools.Newline("{{مش متصنفه|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[CAT:UNCAT|مش متصنفه]]");
                        }
                        else if(Variables.LangCode.Equals("el"))
                        {
                            articleText += Tools.Newline("{{Ακατηγοριοποίητο|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[Πρότυπο:Ακατηγοριοποίητο|ακατηγοριοποίητο]]");
                        }
                        else if(Variables.LangCode.Equals("hy"))
                        {
                            articleText += Tools.Newline("{{Կատեգորիա չկա|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("Կատեգորիա չկա");
                        }
                        else if(Variables.LangCode.Equals("sv"))
                        {
                            articleText += Tools.Newline("{{Okategoriserad|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[Mall:Okategoriserad|okategoriserad]]");
                        }
                        else
                        {
                            articleText += Tools.Newline("{{Uncategorized|", 2) + WikiRegexes.DateYearMonthParameter + @"}}";
                            tagsAdded.Add("[[CAT:UNCAT|uncategorised]]");
                        }
                    }
                }
            }

            // remove {{Uncategorized}} if > 0 real categories (stub categories not counted)
            // rename {{Uncategorized}} to {{Uncategorized stub}} if stub with zero categories (stub categories not counted)
            if (WikiRegexes.Uncat.IsMatch(templates))
            {
                if (totalCategories > 0)
                {
                    articleText = WikiRegexes.Uncat.Replace(articleText, "").TrimStart();
                    if (Variables.LangCode.Equals("ar"))
                        tagsRemoved.Add("غير مصنفة");
                    else if (Variables.LangCode.Equals("arz"))
                        tagsRemoved.Add("مش متصنفه");
                    else
                        tagsRemoved.Add("uncategorised");
                    
                }
                else if (totalCategories == 0 && WikiRegexes.Stub.IsMatch(commentsStripped))
                {
                   // rename uncat to uncat stub if no uncat stub. If uncat and uncat stub, remove uncat.
                    bool uncatstub = false;
                    foreach(Match u in WikiRegexes.Uncat.Matches(articleText))
                    {
                        if(WikiRegexes.Stub.IsMatch(u.Value))
                        {
                            uncatstub = true;
                            break;
                        }
                    }

                    articleText = WikiRegexes.Uncat.Replace(articleText, u2 => {
                                                                if (!uncatstub) // rename
                                                                {
                                                                    tagsrenamed++;
                                                                    if (Variables.LangCode.Equals("ar"))
                                                                        return Tools.RenameTemplate(u2.Value, "بذرة غير مصنفة");
                                                                    if (Variables.LangCode.Equals("arz"))
                                                                        return Tools.RenameTemplate(u2.Value, "تقاوى مش متصنفه");
                                                                    if (Variables.LangCode.Equals("en") || Variables.LangCode.Equals("simple"))
                                                                        return Tools.RenameTemplate(u2.Value, "Uncategorized stub");
                                                                }
                                                                else // already uncat stub so remove plain uncat
                                                                {
                                                                    if(!WikiRegexes.Stub.IsMatch(u2.Value))
                                                                    {
                                                                        if (Variables.LangCode.Equals("ar"))
                                                                            tagsRemoved.Add("غير مصنفة");
                                                                        else if (Variables.LangCode.Equals("arz"))
                                                                            tagsRemoved.Add("مش متصنفه");
                                                                        else
                                                                            tagsRemoved.Add("uncategorised");
                                                                        return "";
                                                                    }
                                                                }
                                                                return u2.Value;
                                                            });
                }
            }

            if (wikiLinkCount == 0 &&
                !WikiRegexes.DeadEnd.IsMatch(articleText) &&
                !WikiRegexes.SIAs.IsMatch(templates) &&
                !WikiRegexes.NonDeadEndPageTemplates.IsMatch(templates) &&
                !WikiRegexes.MeaningsOfMinorPlanetNames.IsMatch(articleTitle)
               )
            {
                // add dead-end tag
                // no blank line between dead end and orphan tags for ar/arz
                if (Variables.LangCode.Equals("ar"))
                {
                    articleText = "{{نهاية مسدودة|" + WikiRegexes.DateYearMonthParameter + "}}\r\n" + (WikiRegexes.Orphan.IsMatch(articleText) ? "" : "\r\n") + articleText;
                    tagsAdded.Add("[[:تصنيف:مقالات نهاية مسدودة|نهاية مسدودة]]");
                    // if dead end then remove underlinked/wikify
                    if(WikiRegexes.Wikify.IsMatch(articleText))
                    {
                        articleText = WikiRegexes.Wikify.Replace(articleText, "").TrimStart();
                        tagsRemoved.Add("ويكي");
                    }
                }
                else if (Variables.LangCode.Equals("arz"))
                {
                    articleText = "{{نهايه مسدوده|" + WikiRegexes.DateYearMonthParameter + "}}\r\n" + articleText;
                    tagsAdded.Add("[[:قالب:نهايه مسدوده|نهايه مسدوده]]");
                    // if dead end then remove underlinked
                    if(WikiRegexes.Wikify.IsMatch(articleText))
                    {
                        articleText = WikiRegexes.Wikify.Replace(articleText, "").TrimStart();
                        tagsRemoved.Add("ويكى");
                    }
                }
                else if (Variables.LangCode != "sv" && !WikiRegexes.Centuryinbox.IsMatch(articleText)
                         && !Regex.IsMatch(WikiRegexes.MultipleIssues.Match(articleText).Value.ToLower(), @"\bdead ?end\b")
                         && !MinorPlanetListFooter.IsMatch(articleText))
                {
                    // Don't add excess newlines between new tags
                    articleText = "{{Dead end|" + WikiRegexes.DateYearMonthParameter + "}}" + (tagsAdded.Count > 0 ? "\r\n" : "\r\n\r\n") + articleText;
                    tagsAdded.Add("[[CAT:DE|deadend]]");
                    // if dead end then remove underlinked
                    if(articleText.IndexOf("underlinked", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        articleText = Tools.NestedTemplateRegex("underlinked").Replace(articleText, "").TrimStart();
                        tagsRemoved.Add("underlinked");
                    }
                }
            }
            // add underlinked/wikify tag, don't add underlinked/wikify if {{dead end}} already present
            // Dont' tag SIA pages, may create wikilinks from templates
            else if (wikiLinkCount < 3 && underlinked && length > 400 && !WikiRegexes.Wikify.IsMatch(articleText)
                     && !WikiRegexes.MultipleIssues.Match(articleText).Value.ToLower().Contains("wikify")
                     && !WikiRegexes.DeadEnd.IsMatch(articleText)
                     && !WikiRegexes.SIAs.IsMatch(articleText)
                     && !WikiRegexes.NonDeadEndPageTemplates.IsMatch(articleText)
                     && !WikiRegexes.MeaningsOfMinorPlanetNames.IsMatch(articleTitle))
            {
                // Avoid excess newlines between templates
                string templateEnd = "}}\r\n" + (articleText.TrimStart().StartsWith(@"{{") ? "" : "\r\n");
                
                if (Variables.LangCode.Equals("ar"))
                {
                    articleText = "{{ويكي|" + WikiRegexes.DateYearMonthParameter + templateEnd + articleText.TrimStart();
                    tagsAdded.Add("[[وب:ويكي|ويكي]]");
                }
                else if (Variables.LangCode.Equals("arz"))
                {
                    articleText = "{{ويكى|" + WikiRegexes.DateYearMonthParameter + templateEnd + articleText;
                    tagsAdded.Add("[[قالب:ويكى|ويكى]]");
                }
                else if (Variables.LangCode.Equals("sv"))
                {
                    articleText = "{{Ickewiki|" + WikiRegexes.DateYearMonthParameter + templateEnd + articleText;
                    tagsAdded.Add("[[WP:PW|ickewiki]]");
                }
                else
                {
                    articleText = "{{Underlinked|" + WikiRegexes.DateYearMonthParameter + templateEnd + articleText;
                    tagsAdded.Add("[[CAT:UL|underlinked]]");
                }
            }
            else if (wikiLinkCount > 3 && !underlinked &&
                     WikiRegexes.Wikify.IsMatch(templates))
            {
                if (Variables.LangCode.Equals("ar") || Variables.LangCode.Equals("arz"))
                    articleText = WikiRegexes.Wikify.Replace(articleText, "");
                else
                    // remove wikify, except section templates or wikify tags with reason parameter specified
                    articleText = WikiRegexes.Wikify.Replace(articleText, m => Tools.IsSectionOrReasonTemplate(m.Value, articleText) ? m.Value : m.Groups[1].Value).TrimStart();

                if (!WikiRegexes.Wikify.IsMatch(articleText))
                {
                    if (Variables.LangCode.Equals("ar"))
                    {
                        tagsRemoved.Add("ويكي");
                    }
                    else if (Variables.LangCode.Equals("arz"))
                    {
                        tagsRemoved.Add("ويكى");
                    }
                    else
                    {
                        tagsRemoved.Add("underlinked");
                    }
                }
            }

            // rename unreferenced --> refimprove if has existing refs, update date
            // if have both unreferenced and refimprove, and have some refs then just remove unreferenced
            if (WikiRegexes.Unreferenced.IsMatch(templates)
                && (TotalRefsNotGrouped(commentsCategoriesStripped) + Tools.NestedTemplateRegex("sfn").Matches(commentsCategoriesStripped).Count) > 0)
            {
                articleText = Unreferenced.Replace(articleText, m2 => 
                                                   {
                                                       if(Tools.NestedTemplateRegex("Refimprove").IsMatch(articleText))
                                                       {
                                                           tagsRemoved.Add("unreferenced");
                                                           return "";
                                                       }

                                                       tagsrenamed++;
                                                       return Tools.UpdateTemplateParameterValue(Tools.RenameTemplate(m2.Value, "refimprove"), "date", "{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}");
                                                   });

                // update tag in old-style multiple issues
                Match m = WikiRegexes.MultipleIssues.Match(articleText);
                if (m.Success && Tools.GetTemplateParameterValue(m.Value, "unreferenced").Length > 0)
                {
                    string newValue = Tools.RenameTemplateParameter(m.Value, "unreferenced", "refimprove");
                    newValue = Tools.UpdateTemplateParameterValue(newValue, "refimprove", "{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}");
                    if (!newValue.Equals(m.Value))
                        articleText = articleText.Replace(m.Value, newValue);
                }
            }

            if (tagsAdded.Count > 0 || tagsRemoved.Count > 0 || tagsrenamed > 0)
            {
                Parsers p = new Parsers();
                HideText hider = new HideText();

                articleText = hider.HideUnformatted(articleText);

                articleText = p.MultipleIssues(articleText);
                articleText = Conversions(articleText);
                articleText = hider.AddBackUnformatted(articleText);

                // sort again in case tag removal requires whitespace cleanup
                // Don't sort interwikis, we can't specify the correct InterWikiSortOrder
                p.SortInterwikis = false;
                articleText = p.Sorter.Sort(articleText, articleTitle);
            }

            summary = PrepareTaggerEditSummary();

            return articleText;
        }

		/// <summary>
		/// Checks whether Tagger is permitted on article.
		/// Allowed on mainspace for non-redirect pages without {{wi}} template
		/// Also allowed for ar-wiki namespace 104
		/// </summary>
		/// <returns>True if Tagger is permitted on article</returns>
		/// <param name='articleText'>Article text</param>
		/// <param name='articleTitle'>Article title</param>
		public static bool TaggerPermitted(string articleText, string articleTitle)
		{
		    if(articleTitle.Equals("Wikipedia:AutoWikiBrowser/Sandbox"))
		        return true;
			// don't tag redirects/outside article namespace/no tagging changes
			// allow for ar-wiki 104
			if(Variables.LangCode.Equals("ar") && Namespace.Determine(articleTitle) == 104 && !WikiRegexes.CEHar.IsMatch(articleText))
				return true;
			if (!Namespace.IsMainSpace(articleTitle) || Tools.IsRedirectOrSoftRedirect(articleText) || WikiRegexes.Wi.IsMatch(articleText) || articleTitle=="Main Page")
				return false;

			return true;
		}

        /// <summary>
        /// Returns the categories that are not stub or proposed deletion categories from the input list
        /// </summary>
        /// <param name="AllCategories">List of all categories</param>
        /// <returns>List of regular categories</returns>
        public static List<Article> RegularCategories(List<Article> AllCategories)
        {
            return (from a in AllCategories
                let name = a.NamespacelessName
                where
                    !name.EndsWith(" stubs") &&
                    !a.Name.EndsWith(":Stubs") &&
                    !name.StartsWith("Proposed deletion") &&
                    !name.Contains("proposed for deletion") &&
                    !name.Contains("proposed deletions") &&
                    !name.Equals("Articles created via the Article Wizard")
                select a).ToList();
        }

        /// <summary>
        /// Returns the categories that are not stub or proposed deletion categories from the input article text
        /// </summary>
        /// <param name="articleText">Wiki text</param>
        /// <returns>List of regular categories</returns>
        public static List<Article> RegularCategories(string articleText, bool hideComments)
        {
            // Don't count commented out categories
            if(hideComments)
                articleText = WikiRegexes.MathPreSourceCodeComments.Replace(articleText, "");

            List<Article> Cats = new List<Article>();

            foreach (Match m in WikiRegexes.Category.Matches(articleText))
            {
                Cats.Add(new Article(m.Groups[1].Value.Trim()));
            }

            return RegularCategories(Cats);
        }

        /// <summary>
        /// Returns the categories that are not stub or proposed deletion categories from the input article text
        /// </summary>
        /// <param name="articleText">Wiki text</param>
        /// <returns>List of regular categories</returns>
        public static List<Article> RegularCategories(string articleText)
        {
            return RegularCategories(articleText, true);
        }

        private static readonly WhatLinksHereAndPageRedirectsExcludingTheRedirectsListProvider WlhProv =
            new WhatLinksHereAndPageRedirectsExcludingTheRedirectsListProvider(MinIncomingLinksToBeConsideredAnOrphan)
            {
                ForceQueryLimit = "10",
                Limit = 10
            };

        private const int MinIncomingLinksToBeConsideredAnOrphan = 3;
        private static readonly Regex Rq = Tools.NestedTemplateRegex("Rq");

        /// <summary>
        /// Tags pages with insufficient incoming page links with the orphan template (localised for ru-wiki).
        /// Removes orphan tag from pages with sufficient incoming page links.
        /// Disambig, SIA pages and soft redirects to Wictionary are never tagged as orphan.
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <param name="articleTitle">Title of the article</param>
        /// <param name="restrictOrphanTagging">Whether to restrict the addition of the orphan tag to pages with zero incoming links only.</param>
        /// <returns>The updated article text</returns>
        private string TagOrphans(string articleText, string articleTitle, bool restrictOrphanTagging)
        {
            // check if not orphaned
            bool orphaned, orphaned2;
            int incomingLinks = 0;
            #if DEBUG || UNITTEST
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
                    ErrorHandler.HandleException(ex);
                }
            }

            if (Variables.LangCode == "ru" && incomingLinks == 0 && Rq.Matches(articleText).Count == 1)
            {
                string rqText = Rq.Match(articleText).Value;
                if (!rqText.Contains("linkless"))
                    return articleText.Replace(rqText, rqText.Replace(@"}}", @"|linkless}}"));
            }

            // add orphan tag if applicable, and no disambig nor SIA
            if (!Variables.LangCode.Equals("sv") && orphaned2 && !WikiRegexes.Orphan.IsMatch(articleText) && Tools.GetTemplateParameterValue(WikiRegexes.MultipleIssues.Match(articleText).Value, "orphan").Length == 0
                && !WikiRegexes.Disambigs.IsMatch(articleText) && !WikiRegexes.SIAs.IsMatch(articleText) && !WikiRegexes.Wi.IsMatch(articleText) && !articleText.Contains(@"[[Category:Disambiguation pages]]"))
            {
                if (Variables.LangCode.Equals("ar"))
                {
                    articleText = "{{يتيمة|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                    tagsAdded.Add("[[تصنيف:يتيمة|يتيمة]]");
                }
                else if (Variables.LangCode.Equals("arz"))
                {
                    articleText = "{{يتيمه|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                    tagsAdded.Add("[[قالب:يتيمه|يتيمه]]");
                }
                else if (Variables.LangCode.Equals("el"))
                {
                    articleText = "{{Ορφανό|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                    tagsAdded.Add("[[Κατηγορία:Ορφανά λήμματα|ορφανό]]");
                }
                else if (Variables.LangCode.Equals("fa"))
                {
                    articleText = "{{یتیم|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                    tagsAdded.Add("[[الگو:یتیم|یتیم]]");
                }
                else if (Variables.LangCode.Equals("hy"))
                {
                    articleText = "{{Որբ|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                    tagsAdded.Add("[[Կատեգորիա:«Որբ» հոդվածներ|Որբ]]");
                }
                else if (Variables.LangCode.Equals("tr"))
                {
                    articleText = "{{Öksüz|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                    tagsAdded.Add("[[CAT:O|orphan]]");
                }
                else
                {
                    articleText = "{{Orphan|" + WikiRegexes.DateYearMonthParameter + "}}\r\n\r\n" + articleText;
                    tagsAdded.Add("[[CAT:O|orphan]]");
                }
            }
            // otherwise consider orphan tag removal
            // do not remove when "few" parameter specified, human review required then
            else if (!orphaned && WikiRegexes.Orphan.IsMatch(articleText)
                     && Tools.GetTemplateParameterValue(WikiRegexes.Orphan.Match(articleText).Value, "few").Length == 0)
            {
                articleText = WikiRegexes.Orphan.Replace(articleText, m => m.Groups["MI"].Value).TrimStart();
                if (Variables.LangCode.Equals("ar"))
                {
                    tagsRemoved.Add("يتيمة");
                }
                else if (Variables.LangCode.Equals("arz"))
                {
                    tagsRemoved.Add("يتيمه");
                }
                else if (Variables.LangCode.Equals("hy"))
                {
                    tagsRemoved.Add("Որբ");
                }
                else
                {
                    tagsRemoved.Add("orphan");
                }
            }

            return articleText;
        }

        private static readonly Regex IbidOpCitRef = new Regex(@"<\s*ref\b[^<>]*>\s*(ibid\.?|op\.?\s*cit\.?|loc\.?\s*cit\.?)\b", RegexOptions.IgnoreCase);
        /// <summary>
        /// Tags references of 'ibid' with the {{ibid}} cleanup template, en-wiki mainspace only
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private string TagRefsIbid(string articleText)
        {
            if (Variables.LangCode == "en" && IbidOpCitRef.IsMatch(articleText) && !WikiRegexes.Ibid.IsMatch(articleText))
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
            // Regex check for performance
            if (!Variables.LangCode.Equals("en") || !Regex.IsMatch(articleText, @"==\s+==[^=]"))
                return articleText;

            string originalarticleText = "";
            int tagsadded = 0;

            while (!originalarticleText.Equals(articleText))
            {
                originalarticleText = articleText;

                int lastpos = -1;
                foreach (Match m in WikiRegexes.HeadingLevelTwo.Matches(Tools.ReplaceWith(articleText, WikiRegexes.UnformattedText, 'x')))
                {
                    // empty setion if only whitespace between two level-2 headings
                    if (lastpos > -1 && articleText.Substring(lastpos, (m.Index - lastpos)).Trim().Length == 0)
                    {
                        articleText = articleText.Insert(m.Index, @"{{Empty section|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}" + "\r\n\r\n");
                        tagsadded++;
                        break;
                    }

                    // don't tag single character headings: alpha list where empty section allowed
                    if (m.Groups[1].Length > 1)
                        lastpos = m.Index + m.Length;
                }
            }

            if (tagsadded > 0)
                tagsAdded.Add("Empty section (" + tagsadded + ")");

            return articleText;
        }

        private string PrepareTaggerEditSummary()
        {
            string summary = "";
            string tags;
            if (tagsRemoved.Count > 0)
            {
            	// Reverse order of words for arwiki and arzwiki
                if (Variables.LangCode.Equals("ar"))
                {
                	if (tagsRemoved.Count == 1)
                		tags = " وسم ";
                	else if (tagsRemoved.Count == 2)
                		tags = " وسمي ";
                	else 
                		tags = " وسوم ";
                    summary = " أزال" + tags + Tools.ListToStringCommaSeparator(tagsRemoved);
                }
                else if (Variables.LangCode.Equals("arz"))
                {
                	if (tagsRemoved.Count == 1)
                		tags = " وسم ";
                	else if (tagsRemoved.Count == 2)
                		tags = " وسمين ";
                	else 
                		tags = " وسوم ";
                    summary = " شال" + tags + Tools.ListToStringCommaSeparator(tagsRemoved);
                }
                else if (Variables.LangCode.Equals("el"))
                {
                	 if(tagsRemoved.Count == 1)
						summary = "αφαιρέθηκε η ετικέτα:" + Tools.ListToStringCommaSeparator(tagsRemoved);
  					else 
						summary = "αφαιρέθηκαν οι ετικέτες:" + Tools.ListToStringCommaSeparator(tagsRemoved);
                }
                else if (Variables.LangCode.Equals("eo"))
                	summary = "forigis " + Tools.ListToStringCommaSeparator(tagsRemoved) + " etikedo" +
                    (tagsRemoved.Count == 1 ? "" : "j");
                else if (Variables.LangCode.Equals("fa"))
                    summary = " برچسب" + Tools.ListToStringCommaSeparator(tagsRemoved) + " حذف شد ";
                else if (Variables.LangCode.Equals("fr"))
                	summary = "retrait " + Tools.ListToStringCommaSeparator(tagsRemoved) + " balise" +
                    (tagsRemoved.Count == 1 ? "" : "s");
                else if (Variables.LangCode.Equals("hy"))
                	summary = "ջնջվեց " + Tools.ListToStringCommaSeparator(tagsRemoved) + " կաղապար" +
                    (tagsRemoved.Count == 1 ? "" : "ներ");
                else if (Variables.LangCode.Equals("sv"))
                {
                	 if(tagsRemoved.Count == 1)
  						tags = Tools.ListToStringCommaSeparator(tagsRemoved) + "-mall";
  					else 
  						tags = Tools.ListToStringWithSeparatorAndWordSuffix(tagsRemoved, "-", ", ", " och ") + "mallar";
                	summary = "tog bort " + tags;
                }

                else if (Variables.LangCode.Equals("tr"))
                {
                	 if(tagsRemoved.Count == 1)
						summary = "removed " + Tools.ListToStringCommaSeparator(tagsRemoved) + " etiketi";
  					else 
						summary = "removed " + Tools.ListToStringCommaSeparator(tagsRemoved) + " etiketleri";
                }
                else
                	summary = "removed " + Tools.ListToStringCommaSeparator(tagsRemoved) + " tag" +
                    (tagsRemoved.Count == 1 ? "" : "s");
            }

            if (tagsAdded.Count > 0)
            {
                if (!string.IsNullOrEmpty(summary))
                {
	                if (Variables.LangCode.Equals("ar") || Variables.LangCode.Equals("arz") || Variables.LangCode.Equals("fa"))
	                    summary += "، ";
                	else
	                    summary += ", ";
                }

                // Reverse order of words for arwiki and arzwiki
                if (Variables.LangCode.Equals("ar"))
                {
                	if (tagsAdded.Count == 1)
                		tags = " وسم ";
                	else if (tagsAdded.Count == 2)
                		tags = " وسمي ";
                	else 
                		tags = " وسوم ";
                    summary += "أضاف " + tags + Tools.ListToStringCommaSeparator(tagsAdded);
                }
                else if (Variables.LangCode.Equals("arz"))
                {
                	if (tagsAdded.Count == 1)
                		tags = " وسم ";
                	else if (tagsAdded.Count == 2)
                		tags = " وسمين ";
                	else 
                		tags = " وسوم ";
                    summary += "زود " + tags + Tools.ListToStringCommaSeparator(tagsAdded);
                }
                else if (Variables.LangCode.Equals("el"))
                {
                	if (tagsAdded.Count == 1)
	                	summary += "προστέθηκε η ετικέτα: " + Tools.ListToStringCommaSeparator(tagsAdded);
                	else 
	                	summary += "προστέθηκαν οι ετικέτες: " + Tools.ListToStringCommaSeparator(tagsAdded);
                }
                	else if (Variables.LangCode.Equals("eo"))
                	summary += "aldonis " + Tools.ListToStringCommaSeparator(tagsAdded) + " etikedo" +
                    (tagsRemoved.Count == 1 ? "" : "j");
                else if (Variables.LangCode.Equals("fa"))
                    summary += "برچسب " + Tools.ListToStringCommaSeparator(tagsAdded) + " اضافه شد ";
                else if (Variables.LangCode.Equals("fr"))
                	summary += "ajout " + Tools.ListToStringCommaSeparator(tagsAdded) + " balise" +
                    (tagsAdded.Count == 1 ? "" : "s");
                else if (Variables.LangCode.Equals("hy"))
                	summary += "ավելացրել է " + Tools.ListToStringCommaSeparator(tagsAdded) + " կաղապար" +
                    (tagsAdded.Count == 1 ? "" : "ներ");
                else if (Variables.LangCode.Equals("sv"))
                {
                	if (tagsAdded.Count == 1)
                		tags = Tools.ListToStringCommaSeparator(tagsAdded) + "-mall";
                	else 
                		tags = Tools.ListToStringWithSeparatorAndWordSuffix(tagsAdded, "-", ", ", " och ") + "mallar";
                	summary += "lade till " + tags;
                }
                else if (Variables.LangCode.Equals("tk"))
                {
                	if (tagsAdded.Count == 1)
	                	summary += "eklendi " + Tools.ListToStringCommaSeparator(tagsAdded) + " etiketi";
                	else 
	                	summary += "eklendi " + Tools.ListToStringCommaSeparator(tagsAdded) + " etiketleri";
                }
                else
                	summary += "added " + Tools.ListToStringCommaSeparator(tagsAdded) + " tag" +
                    (tagsAdded.Count == 1 ? "" : "s");
            }
            return summary;
        }

        private static readonly HideText ht = new HideText();

        /// <summary>
        /// Sets the date (month & year) for undated cleanup tags that take a date, from https://en.wikipedia.org/wiki/Wikipedia:AWB/Dated_templates
        /// Avoids changing tags in unformatted text areas (wiki comments etc.)
        /// Note: https://phabricator.wikimedia.org/T4700 means {{subst:}} within ref tags doesn't work, AWB doesn't do anything about it
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The updated article text</returns>
        public static string TagUpdater(string articleText)
        {
            if(WikiRegexes.DatedTemplates.Any())
            {
                List<string> t = GetAllTemplates(articleText), t2 = new List<string>();

                t2.AddRange(t.Where(s => WikiRegexes.DatedTemplates.Contains(s)));

                // only work to do if article has any of the DatedTemplates in it
                if(t2.Any())
                {
                    string originalArticleText = articleText;
                    articleText = Tools.NestedTemplateRegex(t2).Replace(articleText, TagUpdaterME);

                    // Performance: only worth aplying Hide if we made changes to raw articleText
                    if(!originalArticleText.Equals(articleText))
                    {
                        articleText = ht.Hide(originalArticleText);
                        articleText = Tools.NestedTemplateRegex(t2).Replace(articleText, TagUpdaterME);
                        articleText = FixSyntaxSubstRefTags(articleText);
                        articleText = ht.AddBack(articleText);
                    }
                }
            }

            return articleText;
        }

        private static readonly Regex CurlyBraceEnd = new Regex(@"(?:\| *)?}}$", RegexOptions.Compiled);
        private static readonly Regex MonthYear = new Regex(@"^\s*" + WikiRegexes.MonthsNoGroup + @" +20\d\d\s*$", RegexOptions.Compiled);
        private static readonly Regex DateDash = new Regex(@"(\|\s*[Dd]ate\s*)- *=*", RegexOptions.Compiled);

        /// <summary>
        /// Match evaluator for tag updater
        /// Tags undated tags, corrects incorrect template parameter names, removes template namespace in template name
        /// </summary>
        private static string TagUpdaterME(Match m)
        {
            string templatecall = m.Value;

            // rename incorrect template parameter names
            if (Variables.LangCode.Equals("en"))
            {
                templatecall = Tools.RenameTemplateParameter(templatecall, "Date", "date");
                templatecall = Tools.RenameTemplateParameter(templatecall, "dates", "date");
                
                // date- or Date- --> date=
                if(Tools.GetTemplateArgument(templatecall, 1).ToLower().Replace(" ", "").StartsWith("date-"))
                    templatecall = DateDash.Replace(templatecall, m2 => m2.Groups[1].Value.ToLower() + "=");
            }

            // remove template namespace in template name
            templatecall = RemoveTemplateNamespace(templatecall);

            // check if template already dated (date= parameter, localised for some wikis)
            string dateparam = WikiRegexes.DateYearMonthParameter.Substring(0, WikiRegexes.DateYearMonthParameter.IndexOf("="));

            // rename date= if localized
            if(!dateparam.Equals("date"))
                templatecall = Tools.RenameTemplateParameter(templatecall, "date", dateparam);

            // date tag needed?
            if (Tools.GetTemplateParameterValue(templatecall, dateparam).Length == 0)
            {
                // remove empty 'date='
                templatecall = Tools.RemoveTemplateParameter(templatecall, dateparam);

                // find any dates without date= parameter given, add it
                if (Variables.LangCode.Equals("en") && Tools.GetTemplateArgumentCount(templatecall) == 1)
                {
                    string firstArg = Tools.GetTemplateArgument(templatecall, 1);

                    if (MonthYear.IsMatch(firstArg))
                        templatecall = templatecall.Insert(templatecall.IndexOf(firstArg), "date=");
                    else if (firstArg.Equals(dateparam))
                    {
                        templatecall = templatecall.Insert(templatecall.IndexOf(firstArg) + firstArg.Length, "=");
                        templatecall = Tools.RemoveTemplateParameter(templatecall, dateparam);
                    }
                }

                if (Tools.GetTemplateParameterValue(templatecall, dateparam).Length == 0)
                    return (CurlyBraceEnd.Replace(templatecall, "|" + WikiRegexes.DateYearMonthParameter + "}}"));
            }
            else
            {
                string dateFieldValue = Tools.GetTemplateParameterValue(templatecall, dateparam);

                // May, 2010 --> May 2010
                if (dateFieldValue.Contains(",") && !WikiRegexes.AmericanDates.IsMatch(dateFieldValue))
                {
                    templatecall = Tools.SetTemplateParameterValue(templatecall, dateparam, dateFieldValue.Replace(",", ""));
                    dateFieldValue = Tools.GetTemplateParameterValue(templatecall, dateparam);
                }

                // leading zero removed
                if (dateFieldValue.StartsWith("0"))
                {
                    templatecall = Tools.SetTemplateParameterValue(templatecall, dateparam, dateFieldValue.TrimStart('0'));
                    dateFieldValue = Tools.GetTemplateParameterValue(templatecall, dateparam);
                }

                // full International date?
                if (WikiRegexes.InternationalDates.IsMatch(Regex.Replace(dateFieldValue, @"( [a-z])", u => u.Groups[1].Value.ToUpper())))
                {
                    templatecall = Tools.SetTemplateParameterValue(templatecall, dateparam, dateFieldValue.Substring(dateFieldValue.IndexOf(" ")).Trim());
                    dateFieldValue = Tools.GetTemplateParameterValue(templatecall, dateparam);
                }
                else
                    // ISO date?
                    if (WikiRegexes.ISODates.IsMatch(dateFieldValue))
                {
                    DateTime dt = Convert.ToDateTime(dateFieldValue, BritishEnglish);
                    dateFieldValue = dt.ToString("MMMM yyyy", BritishEnglish);

                    templatecall = Tools.SetTemplateParameterValue(templatecall, dateparam, dateFieldValue);
                }

                // date field starts lower case?
                if (!Variables.LangCode.Equals("zh") && !dateFieldValue.Contains(@"CURRENTMONTH") && !dateFieldValue.Equals(Tools.TurnFirstToUpper(dateFieldValue.ToLower())))
                    templatecall = Tools.SetTemplateParameterValue(templatecall, dateparam, Tools.TurnFirstToUpper(dateFieldValue.ToLower()));
            }

            return templatecall;
        }

        private static readonly Regex CommonPunctuation = new Regex(@"[""',\.;:`!\(\)\[\]\?\-–/]", RegexOptions.Compiled);
        /// <summary>
        /// For en-wiki tags redirect pages with one or more of the templates from [[Wikipedia:Template messages/Redirect pages]]
        /// following [[WP:REDCAT]]
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

            // {{R to other namespace}} or more specific template for project/help/portal/category/template/user/talk
            // See https://en.wikipedia.org/wiki/Template:R_to_other_namespace
            if (Namespace.IsMainSpace(articleTitle) && !Namespace.IsMainSpace(redirecttarget) && !WikiRegexes.NestedTemplates.IsMatch(articleText))
            {
                string template;
                
                switch (Namespace.Determine(redirecttarget))
                {
                    case Namespace.Project :
                        template = "{{R to project namespace}}";
                        break;
                    case Namespace.Help :
                        template = "{{R to help namespace}}";
                        break;
                    case Namespace.FirstCustom :
                        template = "{{R to portal namespace}}";
                        break;
                    case Namespace.Category :
                        template = "{{R to category namespace}}";
                        break;
                    case Namespace.Template :
                        template = "{{R to template namespace}}";
                        break;
                    case Namespace.User :
                        template = "{{R to user namespace}}";
                        break;
                    case Namespace.Talk :
                        template = "{{R to talk namespace}}";
                        break;

                        default :
                            template = "{{R to other namespace}}";
                        break;
                }
                
                return (articleText + " " + template);
            }

            // {{R from modification}}
            // difference is extra/removed/changed puntuation
            if (!Tools.NestedTemplateRegex(WikiRegexes.RFromModificationList).IsMatch(articleText)
                && !CommonPunctuation.Replace(redirecttarget, "").Equals(redirecttarget) && CommonPunctuation.Replace(redirecttarget, "").Equals(CommonPunctuation.Replace(articleTitle, "")))
                return (articleText + " " + WikiRegexes.RFromModificationString);

            // {{R from title without diacritics}}

            // title and redirect target the same if dacritics removed from redirect target
            if (redirecttarget != Tools.RemoveDiacritics(redirecttarget) && Tools.RemoveDiacritics(redirecttarget).Equals(articleTitle)
                && !Tools.NestedTemplateRegex(WikiRegexes.RFromTitleWithoutDiacriticsList).IsMatch(articleText))
                return (articleText + " " + WikiRegexes.RFromTitleWithoutDiacriticsString);

            // {{R from other capitalisation}}
            if (redirecttarget.Equals(articleTitle, StringComparison.OrdinalIgnoreCase)
                && !Tools.NestedTemplateRegex(WikiRegexes.RFromOtherCapitaliastionList).IsMatch(articleText))
                return (articleText + " " + WikiRegexes.RFromOtherCapitalisationString);

            return articleText;
        }

        private static string StubChecker(Match m)
        {
            // if stub tag is a section stub tag, don't remove from section in article
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
            return (WikiRegexes.MoreNoFootnotes.IsMatch(WikiRegexes.Comments.Replace(articleText, "")) && WikiRegexes.Refs.Matches(articleText).Count > 4);
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
            if (!Variables.LangCode.Equals("en"))
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