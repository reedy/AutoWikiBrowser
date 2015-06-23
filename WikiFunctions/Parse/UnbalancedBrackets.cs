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
            AnyTagList = AnyTagList.FindAll(s => !s.EndsWith("/") && !s.StartsWith("!--"));

            // remove any text after first space, so we're left with tag name only
            AnyTagList = AnyTagList.Select(s => s.Contains(" ") ? s.Substring(0, s.IndexOf(" ")).Trim() : s).ToList();

            // discard <br> and <p> tags as not a tag pair
            AnyTagList = AnyTagList.FindAll(s => !s.Equals("br") && !s.Equals("p"));

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

    }
}
