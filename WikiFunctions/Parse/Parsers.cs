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
        private static readonly Regex ReferencesExternalLinksSeeAlso = new Regex(@"== *([Rr]eferences|[Ee]xternal +[Ll]inks|[Ss]ee +[Aa]lso) *==\s");
        private static readonly Regex ReferencesExternalLinksSeeAlsoUnbalancedRight = new Regex(@"(== *(?:[Rr]eferences|[Ee]xternal +[Ll]inks?|[Ss]ee +[Aa]lso) *=) *\r\n");

        private static readonly Regex RegexHeadingColonAtEnd = new Regex(@"^(=+)(\s*[^=\s].*?)\:(\s*\1\s*)$");
        private static readonly Regex RegexHeadingWithBold = new Regex(@"(?<====+.*?)(?:'''|<[Bb]>)(.*?)(?:'''|</[Bb]>)(?=.*?===+)");
        private static readonly List<string> BadHeadings = new List<string>(new [] {"career", "track listing", " members", "further reading", "related ", " life", "source", " links", "external", "also", "reff", "refer", "refr", "<", "\t", "'''", ":"});

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

            // Get all the custom headings, ignoring normal References, External links sections etc.
            List<string> customHeadings = Tools.DeduplicateList((from Match m in WikiRegexes.Headings.Matches(articleText) where !ReferencesExternalLinksSeeAlso.IsMatch(m.Value) select m.Value.ToLower()).ToList());

            // Removes level 2 heading if it matches pagetitle
            if(customHeadings.Any(h => h.Contains(articleTitle.ToLower())))
                articleText = Regex.Replace(articleText, @"^(==) *" + Regex.Escape(articleTitle) + @" *\1\r\n", "", RegexOptions.Multiline);

            // Performance: apply fixes to all headings only if a custom heading matches for the bad headings words
            if(customHeadings.Any(h => BadHeadings.Any(b => h.Contains(b))))
                articleText = WikiRegexes.Headings.Replace(articleText, FixHeadingsME);

            // CHECKWIKI error 8. Add missing = in some headers
            if(customHeadings.Any(h => Regex.Matches(h, "=").Count == 3))
                articleText = ReferencesExternalLinksSeeAlsoUnbalancedRight.Replace(articleText, "$1=\r\n");

            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Section_header_level_.28WikiProject_Check_Wikipedia_.237.29
            // CHECKWIKI error 7
            // if no level 2 heading in article, remove a level from all headings (i.e. '===blah===' to '==blah==' etc.)
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests/Archive_5#Standard_level_2_headers
            // don't consider the "references", "see also", or "external links" level 2 headings when counting level two headings
           	// only apply if all level 3 headings and lower are before the first of references/external links/see also
            if(Namespace.IsMainSpace(articleTitle))
            {
                if(!customHeadings.Any(s => WikiRegexes.HeadingLevelTwo.IsMatch(s)))
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

            return articleText;
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

            return WikiRegexes.EmptyBold.Replace(hAfter, "");
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

            // merge in new portal if multiple portals
            if (Portals.Count < 2)
                return originalArticleText;

            // generate portal string
            string portalsToAdd = Portals.Aggregate("", (current, portal) => current + ("|" + portal));

            // first merge to see also section
            if (WikiRegexes.SeeAlso.Matches(articleText).Count == 1)
                return WikiRegexes.SeeAlso.Replace(articleText, "$0" + Tools.Newline(@"{{Portal" + portalsToAdd + @"}}"));

            // otherwise merge to original location if all portals in same section
            if (Summary.ModifiedSection(originalArticleText, articleText).Length > 0)
                return articleText.Insert(firstPortal, @"{{Portal" + portalsToAdd + @"}}" + "\r\n");

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
            bool hasDash = (textPortion.Contains("-") || textPortion.Contains("–")), hasComma = textPortion.Contains(",");
            
            if(hasComma)
                textPortion = IncorrectCommaInternationalDates.Replace(textPortion, @"$1 $2");

            if(hasDash)
            {
                textPortion = SameMonthInternationalDateRange.Replace(textPortion, @"$1–$2");

                textPortion = SameMonthAmericanDateRange.Replace(textPortion, SameMonthAmericanDateRangeME);

                textPortion = LongFormatInternationalDateRange.Replace(textPortion, @"$1–$3 $2 $4");
                textPortion = LongFormatAmericanDateRange.Replace(textPortion, @"$1 $2–$3, $4");
            }

            // run this after the date range fixes
            textPortion = IncorrectCommaAmericanDates.Replace(textPortion, @"$1 $2, $3");

            // month range
            if(hasDash)
                textPortion = EnMonthRange.Replace(textPortion, @"$1–$2");
        
            return textPortion;
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
            // check for performance
            if(SyntaxRemoveBrQuick.IsMatch(articleText))
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

        private static readonly Regex NestedTemplates = new Regex(@"{{\s*([^{}\|]*)((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!)))}}");

        /// <summary>
        /// Extracts a list of all templates used in the input text, supporting any level of template nesting. Template name given in first letter upper
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns>List of all templates in text</returns>
        public static List<string> GetAllTemplates(string articleText)
        {
            if(Globals.SystemCore3500Available)
                return GetAllTemplatesNew(articleText);
            return GetAllTemplatesOld(articleText);
        }

        private static Queue<KeyValuePair<string, List<string>>> GetAllTemplatesNewQueue = new Queue<KeyValuePair<string, List<string>>>();
        private static Queue<KeyValuePair<string, List<string>>> GetAllTemplatesDetailNewQueue = new Queue<KeyValuePair<string, List<string>>>();

        /// <summary>
        /// Extracts a list of all templates used in the input text, supporting any level of template nesting. Template name given in first letter upper. Most performant version using HashSet.
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private static List<string> GetAllTemplatesNew(string articleText)
        {
            // For peformance, use cached result if available: articletext plus List of template names
            List<string> found = GetAllTemplatesNewQueue.FirstOrDefault(q => q.Key.Equals(articleText)).Value;
            if(found != null)
                return found;
                
            /* performance: process all templates in bulk, extract template contents and reprocess. This is faster than loop applying template match on individual basis. 
            Extract rough template name then get exact template names later, faster to deduplicate then get exact template names */
            // process all templates, handle nested templates to any level of nesting
            List<string> TemplateNames = new List<string>();
            List<string> TemplateDetail = new List<string>();
            HashSet<string> innerTemplateContents = new HashSet<string>();
            string originalarticleText = articleText;

            for(;;)
            {
                List<Match> nt = (from Match m in NestedTemplates.Matches(articleText) select m).ToList();

                if(!nt.Any())
                    break;

                TemplateDetail.AddRange(nt.Select(x => x.Value));

                // add raw template names to list
                TemplateNames.AddRange(nt.Select(m => m.Groups[1].Value).ToList());

                // set text to content of matched templates to process again for any (further) nested templates
                innerTemplateContents = new HashSet<string>(nt.Select(m => m.Groups[2].Value).ToList());
                articleText = String.Join(",", innerTemplateContents.ToArray());
            }

            // now extract exact template names
            List<string> FinalTemplateNames = TemplateNames.Distinct().Select(s => Tools.TurnFirstToUpper(Tools.GetTemplateName(@"{{" + s + @"}}"))).Distinct().ToList();

            TemplateDetail = Tools.DeduplicateList(TemplateDetail);

            // cache new results, then dequeue oldest if cache full
            GetAllTemplatesNewQueue.Enqueue(new KeyValuePair<string, List<string>>(originalarticleText,  FinalTemplateNames));
            if(GetAllTemplatesNewQueue.Count > 10)
                GetAllTemplatesNewQueue.Dequeue();

            GetAllTemplatesDetailNewQueue.Enqueue(new KeyValuePair<string, List<string>>(originalarticleText,  TemplateDetail));
            if(GetAllTemplatesDetailNewQueue.Count > 10)
                GetAllTemplatesDetailNewQueue.Dequeue();

            return FinalTemplateNames;
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

        /// <summary>
        /// Extracts a list of all template calls in the input text, supporting any level of template nesting.
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns>List of all templates calls in text</returns>
        public static List<string> GetAllTemplateDetail(string articleText)
        {
            GetAllTemplates(articleText);

            List<string> found = GetAllTemplatesDetailNewQueue.FirstOrDefault(q => q.Key.Equals(articleText)).Value;
            if(found == null)
                found = new List<string>();

            return found;
        }

        private static Queue<KeyValuePair<string, List<Match>>> GetUnnamedRefsQueue = new Queue<KeyValuePair<string, List<Match>>>();

        /// <summary>
        /// Extracts a list of all unnamed refs used in the input text
        /// </summary>
        /// <param name="articleText"></param>
        private static List<Match> GetUnnamedRefs(string articleText)
        {
            // For peformance, use cached result if available: articletext plus List matches
            List<Match> refsList = GetUnnamedRefsQueue.FirstOrDefault(q => q.Key.Equals(articleText)).Value;
            if(refsList != null)
                return refsList;

            refsList = (from Match m in WikiRegexes.UnnamedReferences.Matches(articleText) select m).ToList();

            // cache new results, then dequeue oldest if cache full
            GetUnnamedRefsQueue.Enqueue(new KeyValuePair<string, List<Match>>(articleText,  refsList));
            if(GetUnnamedRefsQueue.Count > 10)
                GetUnnamedRefsQueue.Dequeue();

            return refsList;
        }

        private static Queue<KeyValuePair<string, List<Match>>> GetNamedRefsQueue = new Queue<KeyValuePair<string, List<Match>>>();

        /// <summary>
        /// Extracts a list of all named refs, including condensed used in the input text
        /// </summary>
        /// <param name="articleText"></param>
        private static List<Match> GetNamedRefs(string articleText)
        {
            // For peformance, use cached result if available: articletext plus List matches
            List<Match> refsList = GetNamedRefsQueue.FirstOrDefault(q => q.Key.Equals(articleText)).Value;
            if(refsList != null)
                return refsList;

            refsList = (from Match m in WikiRegexes.NamedReferencesIncludingCondensed.Matches(articleText) select m).ToList();

            // cache new results, then dequeue oldest if cache full
            GetNamedRefsQueue.Enqueue(new KeyValuePair<string, List<Match>>(articleText,  refsList));
            if(GetNamedRefsQueue.Count > 10)
                GetNamedRefsQueue.Dequeue();

            return refsList;
        }

        private static List<string> RenameTemplateParametersOldParams = new List<string>();

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

            // Performance: now filter templates with parameters to rename against templates used on the page
            // so only templates used on page are looked for
            List<string> templatesToProcess = GetAllTemplates(articleText).Select(t => Tools.TurnFirstToLower(t)).ToList();

            // filter the parameters set down to only those templates used on the page
            RenamedTemplateParameters = RenamedTemplateParameters.Where(t => templatesToProcess.Contains(t.TemplateName)).ToList();

            RenameTemplateParametersOldParams = Tools.DeduplicateList(RenamedTemplateParameters.Select(x => x.OldParameter).ToList());

            templatesToProcess = Tools.DeduplicateList(RenamedTemplateParameters.Select(x => x.TemplateName).ToList());

            if(!templatesToProcess.Any())
                return articleText;

            Regex r = Tools.NestedTemplateRegex(templatesToProcess);

            // Now process distinct templates used in articles using GetAllTemplateDetail
            foreach(string s in GetAllTemplateDetail(articleText))
            {
                string res = r.Replace(s,
                                         m => (Globals.SystemCore3500Available ?
                                               RenameTemplateParametersHashSetME(m, RenamedTemplateParameters)
                                               : RenameTemplateParametersME(m, RenamedTemplateParameters)));
                                                                   
                if(!s.Equals(res))
                    articleText = articleText.Replace(s, res);                
            }

            return articleText;
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
            List<string> oldP = RenameTemplateParametersOldParams.Intersect(pv.Keys.ToArray()).ToList();
            if(oldP.Any())
            {
                string tname = Tools.TurnFirstToLower(Tools.GetTemplateName(@"{{" + m.Groups[2].Value + @"}}"));
                foreach (WikiRegexes.TemplateParameters Params in RenamedTemplateParameters.Where(r => oldP.Contains(r.OldParameter) && r.TemplateName.Equals(tname)))
                {
                    string newp;
                    pv.TryGetValue(Params.NewParameter, out newp);

                    if(string.IsNullOrEmpty(newp))
                        newvalue = Tools.RenameTemplateParameter(newvalue, Params.OldParameter, Params.NewParameter);
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
        /// Loads List of templates (first letter lower), old parameter, new parameter from within {{AWB rename template parameter}}
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
        private static readonly Regex citeWebParameters = new Regex(@"^(access-?date|agency|archive-?date|archive-?url|arxiv|ARXIV|asin|ASIN|asin-tld|ASIN-TLD|at|[Aa]uthor\d*|author\d*-first|author-?format|author\d*-last|author-?link\d*|author\d*-?link|authors|author-mask|author-name-separator|author-separator|bibcode|BIBCODE|date|dead-?url|dictionary|display-?authors|display-?editors|doi|DOI|DoiBroken|doi-broken|doi-broken-date|doi_brokendate|doi-inactive-date|doi_inactivedate|edition|[Ee]ditor|editor\d*|editor\d*-first|editor-?format|EditorGiven\d*|editor\d*-given|editor\d*-last|editor\d*-?link|editor-?mask|editor-name-separator|EditorSurname\d*|editor\d*-surname|editor-first\d*|editor-given\d*|editor-last\d*|editor-surname\d*|editorlink\d*|editors|[Ee]mbargo|encyclopa?edia|first\d*|format|given\d*|id|ID|ignoreisbnerror|ignore-isbn-error|institution|isbn|ISBN|isbn13|ISBN13|issn|ISSN|issue|jfm|JFM|journal|jstor|JSTOR|language|last\d*|lastauthoramp|last-author-amp|lay-?date|lay-?source|lay-?summary|lay-?url|lccn|LCCN|location|magazine|mode|mr|MR|newspaper|no-?pp|number|oclc|OCLC|ol|OL|orig-?year|others|osti|pp?|pages?|people|periodical|place|pmc|PMC|pmid|PMID|postscript|publication-?(?:place|date)|publisher|quotation|quote|[Rr]ef|registration|rfc|RFC|script\-title|separator|series|series-?link|ssrn|SSRN|subscription|surname\d*|title|trans[_-]title|type|url|URL|version|via|volume|website|work|year|zbl|ZBL)\b", RegexOptions.Compiled);
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
            if (!Variables.IsWikipediaEN || TemplateExists(GetAllTemplates(articleText), CitePodcast))
                return articleText;

            string originalArticleText = articleText;

            // loop in case a single citation has multiple dates to be fixed
            foreach (string s in GetAllTemplateDetail(articleText))
            {
                string res = s, original = "";
                while(!res.Equals(original))
                {
                    original = res;
                    res = WikiRegexes.CiteTemplate.Replace(res, CiteTemplateME);
                }

                if(!res.Equals(s))
                    articleText = articleText.Replace(s, res);
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
        private static readonly Regex CenterTag = new Regex(@"<\s*center\s*>((?>(?!<\s*/?\s*center\s*>).|<\s*center\s*>(?<DEPTH>)|<\s*/\s*center\s*>(?<-DEPTH>))*(?(DEPTH)(?!)))<\s*/\s*center\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static readonly Regex SupTag = new Regex(@"<\s*sup\s*>((?>(?!<\s*/?\s*sup\s*>).|<\s*sup\s*>(?<DEPTH>)|<\s*/\s*sup\s*>(?<-DEPTH>))*(?(DEPTH)(?!)))<\s*/\s*sup\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static readonly Regex SubTag = new Regex(@"<\s*sub\s*>((?>(?!<\s*/?\s*sub\s*>).|<\s*sub\s*>(?<DEPTH>)|<\s*/\s*sub\s*>(?<-DEPTH>))*(?(DEPTH)(?!)))<\s*/\s*sub\s*>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
        private static readonly Regex AnyTag = new Regex(@"<([^<>]+)>");
        private static readonly Regex TagToEnd = new Regex(@"<[^>]+$");
        private static readonly Regex SimpleTagPair = new Regex(@"<([^<>]+)>[^<>]+</\1>");

        /// <summary>
        ///  Searches for any unclosed &lt;math&gt;, &lt;source&gt;, &lt;ref&gt;, &lt;code&gt;, &lt;nowiki&gt;, &lt;small&gt;, &lt;pre&gt; &lt;center&gt; &lt;sup&gt; &lt;sub&gt; or &lt;gallery&gt; tags and comments
        /// </summary>
        /// <param name="articleText">The article text</param>
        /// <returns>dictionary of the index and length of any unclosed tags</returns>
        public static Dictionary<int, int> UnclosedTags(string articleText)
        {
            Dictionary<int, int> back = new Dictionary<int, int>();

            // Performance: get all tags, compare the count of matched tags of same name
            // Then do full tag search if unmatched tags found

            // get all tags in format <tag...> in article
            List<string> AnyTagList = (from Match m in AnyTag.Matches(articleText)
                select m.Groups[1].Value.Trim().ToLower()).ToList();

            // discard self-closing tags in <tag/> format, discard wiki comments
            AnyTagList = AnyTagList.Where(s => !s.EndsWith("/") && !s.StartsWith("!--")).ToList();

            // remove any text after first space, so we're left with tag name only
            AnyTagList = AnyTagList.Select(s => s.Contains(" ") ? s.Substring(0, s.IndexOf(" ")).Trim() : s).ToList();

            // discard <br> and <p> tags as not a tag pair
            AnyTagList = AnyTagList.Where(s => !s.Equals("br") && !s.Equals("p")).ToList();

            // Count the tag names in use, determine if unmatched tags by comparing count of opening and closing tags
            bool unmatched = false;
            Dictionary<string, int> tagCounts = AnyTagList.GroupBy(x => x).ToDictionary(x => x.Key, y => y.Count());
            foreach(KeyValuePair<string, int> kvp in tagCounts)
            {
                int matchedCount = 0;
                string othertag = kvp.Key.StartsWith("/") ? kvp.Key.TrimStart('/') : "/" + kvp.Key;
                if(tagCounts.TryGetValue(othertag, out matchedCount) && matchedCount == kvp.Value)
                    continue;

                unmatched = true;
                break;
            }

            // check for any unmatched tags or unclosed part tag
            if(!unmatched && !TagToEnd.IsMatch(AnyTag.Replace(articleText, "")))
                return back;
            
            // if here then have some unmatched tags, so do full clear down and search
            // performance of Refs/SourceCode is better if IgnoreCase avoided
            articleText = articleText.ToLower();
            articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.UnformattedText);
            articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.GalleryTag, 2);
            articleText = Tools.ReplaceWithSpaces(articleText, new Regex(WikiRegexes.Refs.ToString(), RegexOptions.Singleline));

            // some (badly done) List of pages can have hundreds of unclosed small or center tags, causes regex bactracking when using <DEPTH>
            // so workaround solution: if > 10 unclosed tags, only remove tags without other tags embedded in them
            // Workaround constraint: we might incorrectly report some valid tags with < or > in them as unclosed
            if(AnyTagList.Where(s => !s.StartsWith("/")).Count() > (AnyTagList.Where(s => s.StartsWith("/")).Count() + 10))
            {
                while(SimpleTagPair.IsMatch(articleText))
                    articleText = Tools.ReplaceWithSpaces(articleText, SimpleTagPair);
            } 
            else
            {
                articleText = Tools.ReplaceWithSpaces(articleText, new Regex(WikiRegexes.SourceCode.ToString(), RegexOptions.Singleline));
                articleText = Tools.ReplaceWithSpaces(articleText, CenterTag, 2);
                articleText = Tools.ReplaceWithSpaces(articleText, WikiRegexes.Small);
                articleText = Tools.ReplaceWithSpaces(articleText, SupTag, 2);
                articleText = Tools.ReplaceWithSpaces(articleText, SubTag, 2);
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
            return RefSimple.Aggregate(m.Value, (current, rr) => rr.Regex.Replace(current, rr.Replacement));
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

        private static readonly Regex SyntaxRegexWikilinkMissingClosingBracket = new Regex(@"\[\[([^][]*?)\|?\](?=[^\]]*?(?:$|\[|\n))", RegexOptions.Compiled);
        private static readonly Regex SyntaxRegexWikilinkMissingOpeningBracket = new Regex(@"(?<=(?:^|\]|\n)[^\[]*?)\[([^][]*?)\]\](?!\])", RegexOptions.Compiled);

        private static readonly Regex SyntaxRegexExternalLinkToImageURL = new Regex("\\[?\\["+Variables.NamespacesCaseInsensitive[Namespace.File]+"(http:\\/\\/.*?)\\]\\]?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
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
        private static readonly Regex SyntaxRemoveBrQuick = new Regex(@"<br[\s/]*>\s*<br[\s/]*>", RegexOptions.IgnoreCase);

        private static readonly Regex MultipleHttpInLink = new Regex(@"(?<=[\s\[>=])(https?(?::?/+|:/*)) *(\1)+", RegexOptions.IgnoreCase);
        private static readonly Regex MultipleFtpInLink = new Regex(@"(?<=[\s\[>=])(ftp(?::?/+|:/*))(\1)+", RegexOptions.IgnoreCase);
        private static readonly Regex PipedExternalLink = new Regex(@"(\[\w+://[^\]\[<>\""\s]*?\s*)(?: +\||\|([ ']))(?=[^\[\]\|]*\])");
        private static readonly Regex HttpLinks = new Regex(@"http[htps:/ ]+");

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
        private static readonly Regex SyntaxRegexISBN = new Regex(@"(?:ISBN(?:-1[03])?:|\[\[ISBN\]\]|ISBN\t)\s*(\d)", RegexOptions.Compiled);
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
            List<string> alltemplates = GetAllTemplates(articleText);

            if (Variables.LangCode.Equals("en"))
            {
                // DEFAULTSORT whitespace fix - CHECKWIKI error 88
                articleText = WikiRegexes.Defaultsort.Replace(articleText, DefaultsortME);
                // This category should not be directly added
                articleText = articleText.Replace(@"[[Category:Disambiguation pages]]", @"{{Disambiguation}}");
            }

            if(TemplateExists(alltemplates, WikiRegexes.MagicWordTemplates))
                articleText = Tools.TemplateToMagicWord(articleText);

            // get a list of all the simple html tags (not with properties) used in the article, so we can selectively apply HTML tag fixes below
            List<string> SimpleTagsList = Tools.DeduplicateList((from Match m in SimpleTags.Matches(articleText) select m.Value).ToList());
            SimpleTagsList = Tools.DeduplicateList(SimpleTagsList.Select(s => Regex.Replace(s, @"\s", "").ToLower()).ToList());

            // fix for <sup/>, <sub/>, <center/>, <small/>, <i/> etc.
            if(SimpleTagsList.Any(s => !s.Equals("<br/>") && (s.EndsWith("/>") || s.Contains(@"\"))))
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
            if(SimpleTagsList.Any(s => s.StartsWith("<b") && !s.StartsWith("<br")))
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
            bool badHttpLinks = Tools.DeduplicateList((from Match m in HttpLinks.Matches(articleText.ToLower()) select m.Value).ToList()).Where(s => !Regex.IsMatch(s, @"^https?://[htps]*$")).Any();

            if(badHttpLinks)
                articleText = MultipleHttpInLink.Replace(articleText, "$1");

            articleText = MultipleFtpInLink.Replace(articleText, "$1");

            if(badHttpLinks && TemplateExists(alltemplates, WikiRegexes.UrlTemplate))
                articleText = WikiRegexes.UrlTemplate.Replace(articleText, m => m.Value.Replace("http://http://", "http://"));

            if (badHttpLinks && !SyntaxRegexHTTPNumber.IsMatch(articleText))
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

            MatchCollection ssbMc = SingleSquareBrackets.Matches(articleText);
            string nobrackets = Tools.ReplaceWithSpaces(articleText, ssbMc);
            bool orphanedSingleBrackets = (nobrackets.Contains("[") || nobrackets.Contains("]"));

            if(orphanedSingleBrackets)
            {
                articleText = RefExternalLinkMissingStartBracket.Replace(articleText, @"$1[$2");
                articleText = RefExternalLinkMissingEndBracket.Replace(articleText, @"$1]$2");
                
                // refresh
                ssbMc = SingleSquareBrackets.Matches(articleText);
            }

            // fixes for external links: internal square brackets, newlines or pipes - Partially CHECKWIKI error 80
            // Performance: filter down to matches with likely external link (contains //) and has pipe, newline or internal square brackets
            List<Match> ssb = (from Match m in ssbMc select m).ToList();
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
            }

            // needs to be applied after SquareBracketsInExternalLinks
            if(orphanedSingleBrackets && !SyntaxRegexFileWithHTTP.IsMatch(articleText))
            {
                articleText = SyntaxRegexWikilinkMissingClosingBracket.Replace(articleText, "[[$1]]");
                articleText = SyntaxRegexWikilinkMissingOpeningBracket.Replace(articleText, "[[$1]]");
            }

            // adds missing http:// to bare url references lacking it - CHECKWIKI error 62
            articleText = RefURLMissingHttp.Replace(articleText, @"$1http://www.");

            //repair bad Image/external links, ssb check for performance
            if(ssb.Any(m => m.Value.Contains(":") && m.Value.ToLower().Contains(":http")))
                articleText = SyntaxRegexExternalLinkToImageURL.Replace(articleText, "[$1]");

            //  CHECKWIKI error 69
            bool isbnDash = articleText.Contains("ISBN-");
            if(isbnDash || articleText.Contains("ISBN:") || articleText.Contains("ISBN\t") || ssb.Any(m => m.Value.Equals("[[ISBN]]")))
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
            bool doubleBracketHttp = articleText.IndexOf("[[http", StringComparison.OrdinalIgnoreCase) > -1;
            if(doubleBracketHttp)
                articleText = DoubleBracketAtStartOfExternalLink.Replace(articleText, "[$1");

            // if there are some unbalanced brackets, see whether we can fix them
            articleText = FixUnbalancedBrackets(articleText);

            //fix uneven bracketing on links
            if(doubleBracketHttp)
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

            articleText = IncorrectBr2.Replace(articleText, m =>
                                                {
                                                    if(m.Groups[1].Value == "left")
                                                        return "{{clear|left}}";
                                                    if(m.Groups[1].Value == "right")
                                                        return "{{clear|right}}";

                                                    return "{{clear}}";
                                                }
                                                );

            // CHECKWIKI errors 55, 63, 66, 77
            if(SimpleTagsList.Any(s => s.Contains("small")))
                articleText = FixSmallTags(articleText);

            articleText = WordingIntoBareExternalLinks.Replace(articleText, @"$1[$3 $2]");

            if(TemplateExists(alltemplates, WikiRegexes.DeadLink))
                articleText = DeadlinkOutsideRef.Replace(articleText, @" $2$1");

            if(!Variables.LangCode.Equals("zh"))
            {
                articleText = ExternalLinkWordSpacingBefore.Replace(articleText, " $1");
                articleText = ExternalLinkWordSpacingAfter.Replace(articleText, "$1 $2");
            }

            // CHECKWIKI error 65: Image description ends with break – https://tools.wmflabs.org/checkwiki/cgi-bin/checkwiki.cgi?project=enwiki&view=only&id=65
            if(ssb.Any(s => s.Value.Contains("<")))
                articleText = WikiRegexes.FileNamespaceLink.Replace(articleText, m=> WikilinkEndsBr.Replace(m.Value, @"]]"));

            // workaround for https://phabricator.wikimedia.org/T4700 -- {{subst:}} doesn't work within ref tags
            articleText = FixSyntaxSubstRefTags(articleText);

            // ensure magic word behaviour switches such as __TOC__ are in upper case
            if(nobrackets.IndexOf("__", StringComparison.Ordinal) > -1)
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

        private static readonly Regex RedirectBracketsWithPrefix = new Regex(@"[=:] ?\[\[", RegexOptions.Compiled);
        private static readonly Regex TooManyOpenSquareBrackets = new Regex(@"\[{3,4}", RegexOptions.Compiled);
        private static readonly Regex TooManyCloseSquareBrackets = new Regex(@"\]{3,4}", RegexOptions.Compiled);

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
            articleText = WikiRegexes.Redirect.Replace(articleText, m =>
                TooManyCloseSquareBrackets.Replace(
                    TooManyOpenSquareBrackets.Replace(
                        RedirectBracketsWithPrefix.Replace(m.Value.Replace("\r\n", " "), " [["), "[["), "]]"));

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
                    articleText = SmallTagRegexes.Aggregate(articleText, (current, rx) => rx.Replace(current, FixSmallTagsME));

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
                            articleTextTemp = CiteRefEndsTripleOpeningBrace.Replace(articleTextTemp, "$1{{$2");

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
        private static readonly Regex rpTemplate = Tools.NestedTemplateRegex("rp");

        /// <summary>
        /// Applies various formatting fixes to citation templates
        /// </summary>
        /// <param name="articleText">The wiki text of the article.</param>
        /// <returns>The updated wiki text</returns>
        public static string FixCitationTemplates(string articleText)
        {
            if (!Variables.LangCode.Equals("en"))
                return articleText;

            if(TemplateExists(GetAllTemplates(articleText), WikiRegexes.CiteTemplate))
            {
                foreach (string s in GetAllTemplateDetail(articleText))
                {
                    string res = s, original = "";
                    while(!res.Equals(original))
                    {
                        original = res;
                        res = WikiRegexes.CiteTemplate.Replace(res, FixCitationTemplatesME);
                    }

                    if(!res.Equals(s))
                        articleText = articleText.Replace(s, res);
                }
            }

            if(TemplateExists(GetAllTemplates(articleText), WikiRegexes.HarvTemplate))
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
            
            if(TemplateExists(GetAllTemplates(articleText), rpTemplate))
                articleText = rpTemplate.Replace(articleText, m =>
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

            // Performance: check through whole text counting single brackets, only run detailed checks if find unbalanced brackets
            int square=0, curly=0, round=0, chevron=0;
            bool hasUnbalanced = false;
            
            foreach(char c in articleText.ToCharArray())
            {
                if(c == '[')
                    square++;
                else if(c == ']')
                    square--;
                else if(c == '{')
                    curly++;
                else if(c == '}')
                    curly--;
                else if(c == '(')
                    round++;
                else if(c == ')')
                    round--;
                else if(c == '<')
                    chevron++;
                else if(c == '>')
                    chevron--;
                
                // if more closing that opening then have found unbalanced brackets
                if(square < 0 || curly < 0 || round < 0 || chevron < 0)
                {
                    hasUnbalanced = true;
                    break;
                }
            }
            
            // if > 0 residual, means more opening brackets than closing
            if(!hasUnbalanced && square == 0 && curly == 0 && round == 0 && chevron == 0)
                return -1;

            // if here have found an unbalanced single bracket so run the compare
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
            List<string> allWikiLinks = GetAllWikiLinks(articleText);

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

            if(TemplateExists(GetAllTemplates(articleText), InfoBoxSingleAlbum))
                articleText = FixLinksInfoBoxSingleAlbum(articleText, articleTitle);

            // clean up wikilinks: replace underscores, percentages and URL encoded accents etc.
            List<string> wikiLinks = GetAllWikiLinks(articleText);
            
            // Replace {{!}} with a standard pipe
            foreach(string e in wikiLinks.Where(l => l.Contains(@"{{!}}") && !l.Contains("|")))
                articleText = articleText.Replace(e, e.Replace(@"{{!}}", "|"));

            // See if any self interwikis that need fixing later
            bool hasAnySelfInterwikis = wikiLinks.Any(l => l.Contains(Variables.LangCode + ":"));

            // Performance: on articles with lots of links better to filter down to those that could be changed by canonicalization, rather than running regex replace against all links
            foreach(string l in wikiLinks.Where(link => link.IndexOfAny("&%_".ToCharArray()) > -1))
            {
                string res = WikiRegexes.WikiLink.Replace(l, FixLinksWikilinkCanonicalizeME);
                if(res != l)
                    articleText = articleText.Replace(l, res);
            }

            // First check for performance, second to avoid (dodgy) apostrophe after link
            if(wikiLinks.Any(link => link.Contains("|''")) && !articleText.Contains(@"]]'"))
                articleText = WikiRegexes.PipedWikiLink.Replace(articleText, FixLinksWikilinkBoldItalicsME);

            // fix excess trailing pipe, TrailingPipe regex for performance
            if(wikiLinks.Any(link => link.Contains("|") && TrailingPipe.IsMatch(link)))
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
            List<string> pipedLinks = GetAllWikiLinks(articleText).Where(link => link.Contains("|")).ToList();

            // Performance: second determine if any links with pipe whitespace to clean
            string Category = Variables.Namespaces.ContainsKey(Namespace.Category) ? Variables.Namespaces[Namespace.Category] : "Category:";
            bool whitepaceTrimNeeded = pipedLinks.Any(s => ((s.Contains("| ") && !s.Contains(Category)) || s.Contains(" |") || (!s.Contains("| ]]") && s.Contains(" ]]"))));

            foreach (string pl in pipedLinks)
            {
                Match m = WikiRegexes.PipedWikiLink.Match(pl);

                // don't process if only matched part of link eg link is [[Image:...]] link with nested wikilinks
                if(pl.Length != m.Length)
                    continue;

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

                string lb = Tools.TurnFirstToLower(b), la = Tools.TurnFirstToLower(a);
                
                if(pl.IndexOfAny("&%_".ToCharArray()) > -1) // check for performance
                {
                    string b2 = CanonicalizeTitle(b), a2 = CanonicalizeTitle(a);

                    if (b2.Equals(a) || b2.Equals(la) || a2.Equals(b) || a2.Equals(lb)) // target and text the same after cleanup and case conversion e.g. [[A|a]] or [[Foo_bar|Foo bar]] etc.
                    {
                        articleText = articleText.Replace(pl, "[[" + b.Replace("_", " ") + "]]");
                    }
                }
                
                if (lb.StartsWith(la, StringComparison.Ordinal)) // target is substring of text e.g. [[Dog|Dogs]] --> [[Dog]]s
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

        private static Queue<KeyValuePair<string, List<string>>> GetAllWikiLinksQueue = new Queue<KeyValuePair<string, List<string>>>();

        /// <summary>
        /// Extracts a list of all distinct wikilinks used in the input text
        /// </summary>
        /// <param name="articleText"></param>
        /// <returns></returns>
        private static List<string> GetAllWikiLinks(string articleText)
        {
            // For peformance, use cached result if available: articletext plus List of wikilinks
            List<string> found = GetAllWikiLinksQueue.FirstOrDefault(q => q.Key.Equals(articleText)).Value;
            if(found != null)
                return found;

            string text = articleText;
            List<string> allLinks = new List<string>();

            for(;;)
            {
                List<Match> linkMatches = (from Match m in WikiRegexes.SimpleWikiLink.Matches(text) select m).ToList();

                if(!linkMatches.Any())
                    break;

                allLinks.AddRange(linkMatches.Select(m => m.Value).ToList());

                // set text to content of matched links to process again for any (further) nested links
                text = String.Join(",",  linkMatches.Select(m => m.Groups[1].Value).ToArray());
            }

            allLinks = Tools.DeduplicateList(allLinks);

            // cache new results, then dequeue oldest if cache full
            GetAllWikiLinksQueue.Enqueue(new KeyValuePair<string, List<string>>(articleText,  allLinks));
            if(GetAllWikiLinksQueue.Count > 10)
                GetAllWikiLinksQueue.Dequeue();

            return allLinks;
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
                string catsOriginal = cats;

                // fix extra brackets: three or more at end
                cats = Regex.Replace(cats, @"(" + Regex.Escape(CategoryStart) + @"[^\r\n\[\]{}<>]+\]\])\]+", "$1");
                // three or more at start
                cats = Regex.Replace(cats, @"\[+(?=" + Regex.Escape(CategoryStart) + @"[^\r\n\[\]{}<>]+\]\])", "");

                cats = WikiRegexes.LooseCategory.Replace(cats, LooseCategoryME);

                // Performance: return original text if no changes
                if(cats.Equals(catsOriginal))
                    return articleText;

                articleText = articleText.Substring(0, cutoff) + cats;
            }
            
            return articleText;
        }

        private static string LooseCategoryME(Match m)
        {
            if (!Tools.IsValidTitle(m.Groups[1].Value))
                return m.Value;

            string sortkey = m.Groups[2].Value;

            if(!string.IsNullOrEmpty(sortkey))
            {
                // diacritic removal in sortkeys on en-wiki/simple-wiki only
                if(Variables.LangCode.Equals("en") || Variables.LangCode.Equals("simple"))
                    sortkey = Tools.CleanSortKey(sortkey);

                sortkey = WordWhitespaceEndofline.Replace(sortkey, "$1");
            }

            return CategoryStart + Tools.TurnFirstToUpper(CanonicalizeTitleRaw(m.Groups[1].Value, false).Trim().TrimStart(':')) + sortkey + "]]";
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
            List<string> alltemplates = GetAllTemplates(articleText);

            if(TemplateExists(alltemplates, NoBoldTitle))
                return articleText;

            HideText Hider2 = new HideText(), Hider3 = new HideText(true, true, true);

            // 1) clean up bolded self links first, provided no noinclude use in article
            string afterSelfLinks = BoldedSelfLinks(articleTitle, articleText);
            
            if(!afterSelfLinks.Equals(articleText) && !WikiRegexes.IncludeonlyNoinclude.IsMatch(articleText))
                articleText = afterSelfLinks;

            // 2) Clean up self wikilinks
            string articleTextAtStart = articleText, zerothSection = Tools.GetZerothSection(articleText);
            string restOfArticle = articleText.Substring(zerothSection.Length);
            string zerothSectionHidden, zerothSectionHiddenOriginal;

            // first check for any self links and no bold title, if found just convert first link to bold and return
            // https://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Bugs/Archive_11#Includes_and_selflinks
            // don't apply if bold in lead section already or some noinclude transclusion business
            if(!SelfLinks(zerothSection, articleTitle).Equals(zerothSection) && !WikiRegexes.IncludeonlyNoinclude.IsMatch(articleText))
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
                || TemplateExists(alltemplates, NihongoTitle))
                return articleTextAtStart;

            string escTitle = Regex.Escape(articleTitle), escTitleNoBrackets = Regex.Escape(BracketedAtEndOfLine.Replace(articleTitle, ""));
            Regex boldTitleAlready1 = new Regex(@"'''\s*(" + escTitle + "|" + Tools.TurnFirstToLower(escTitle) + @")\s*'''");
            Regex boldTitleAlready2 = new Regex(@"'''\s*(" + escTitleNoBrackets + "|" + Tools.TurnFirstToLower(escTitleNoBrackets) + @")\s*'''");

            // if title in bold already exists in article, or paragraph starts with something in bold, don't change anything
            // ignore any bold in infoboxes
            if(BoldTitleAlready4.IsMatch(Tools.ReplaceWithSpaces(zerothSection, WikiRegexes.InfoBox.Matches(zerothSection))) || DfnTag.IsMatch(zerothSection))
                return articleTextAtStart;
            
            string articleTextNoInfobox = Tools.ReplaceWithSpaces(articleText, WikiRegexes.InfoBox.Matches(articleText));
            if (boldTitleAlready1.IsMatch(articleTextNoInfobox) || boldTitleAlready2.IsMatch(articleTextNoInfobox)
                || BoldTitleAlready3.IsMatch(articleTextNoInfobox))
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
                                                                     || WikiRegexes.DateBirthAndAge.IsMatch(zerothSection)
                                                                     || WikiRegexes.DeathDateAndAge.IsMatch(zerothSection)
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
            List<string> possibleInterwiki = GetAllWikiLinks(articleText).Where(l => l.Contains(":")).ToList();

            if(possibleInterwiki.Any(l => l.StartsWith(@"[[zh-tw:")))
                articleText = articleText.Replace("[[zh-tw:", "[[zh:");
            if(possibleInterwiki.Any(l => l.StartsWith(@"[[nb:")))
                articleText = articleText.Replace("[[nb:", "[[no:");
            if(possibleInterwiki.Any(l => l.StartsWith(@"[[dk:")))
                articleText = articleText.Replace("[[dk:", "[[da:");

            return articleText;
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
        
        /// <summary>
        /// Returns whether the given regex matches any of the (first name upper) templates in the given list
        /// </summary>
        private static bool TemplateExists(List<string> templatesFound, Regex r)
        {
            return templatesFound.Any(s => r.IsMatch(@"{{" + s + "|}}"));
        }

        /// <summary>
        /// Returns the count of matches for the given regex against the (first name upper) templates in the given list
        /// </summary>
        private static int TemplateCount(List<string> templatesFound, Regex r)
        {
            return templatesFound.Where(s => r.IsMatch(@"{{" + s + "|}}")).Count();
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