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
        private static readonly Regex LinkWhitespace1 = new Regex(@"(\W|^)\[\[ ([^\]]+)\]\]", RegexOptions.Compiled);
        private static readonly Regex LinkWhitespace2 = new Regex(@"(?<=\w)\[\[ ([^\]]+)\]\]", RegexOptions.Compiled);
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
                articleText = LinkWhitespace1.Replace(articleText, m => m.Groups[1].Value.Trim() + " [[" + m.Groups[2].Value + "]]");

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
    }
}