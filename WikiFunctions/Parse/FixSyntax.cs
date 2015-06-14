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
            List<string> alltemplatesDetail = GetAllTemplateDetail(articleText);
            MatchCollection ssbMc = SingleSquareBrackets.Matches(articleText);

            if (Variables.LangCode.Equals("en"))
            {
                // DEFAULTSORT whitespace fix - CHECKWIKI error 88
                articleText = FixSyntaxDefaultSort(articleText);

                // This category should not be directly added
                if((from Match m in ssbMc where m.Value.Equals(@"[[Category:Disambiguation pages]]") select m).Any())
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
            if(alltemplatesDetail.Where(t => Regex.IsMatch(t, Variables.NamespacesCaseInsensitive[Namespace.Template])).Any())
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
        /// Applies fixes to any DEFAULTSORT templates in the input text
        /// </summary>
        /// <returns>The updated article text</returns>
        /// <param name="articleText">Article text.</param>
        private static string FixSyntaxDefaultSort(string articleText)
        {
            // Performance: check DEFAULTSORT from cache, to avoid processing articleText if no changes to make
            List<string> alltemplates = GetAllTemplateDetail(articleText).Where(t => WikiRegexes.Defaultsort.IsMatch(t)).ToList();

            // must apply DefaultsortME if no existing DEFAULTSORT as it may be a template with unclosed braces
            if(!alltemplates.Any() || alltemplates.Any(s => !s.Equals(WikiRegexes.Defaultsort.Replace(s, DefaultsortME))))
                articleText = WikiRegexes.Defaultsort.Replace(articleText, DefaultsortME);

            return articleText;
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
    }
}
