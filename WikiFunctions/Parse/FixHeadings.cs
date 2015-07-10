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

    /// <summary>
    /// Provides functions for editing wiki text, such as formatting and re-categorisation.
    /// </summary>
    public partial class Parsers
    {

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
            List<string> customHeadings = (from Match m in WikiRegexes.Headings.Matches(articleText) where !ReferencesExternalLinksSeeAlso.IsMatch(m.Value) select m.Value.ToLower()).ToList();

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
    }
}
