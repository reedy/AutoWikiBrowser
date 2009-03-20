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

using System.Text;
using System.Text.RegularExpressions;

namespace WikiFunctions
{
    /// <summary>
    /// Provides some common static regexes 
    /// </summary>
    public static class WikiRegexes
    {
        public static void MakeLangSpecificRegexes()
        {
            TemplateStart = @"\{\{\s*(:?" + Variables.NamespacesCaseInsensitive[Namespace.Template] + ")?";

            Category = new Regex(@"\[\[" + Variables.NamespacesCaseInsensitive[Namespace.Category] 
                + @"(.*?)\]\]", RegexOptions.Compiled);

            // images mask was [^\]]*?(?:\[\[?.*?(?:\[\[.*?\]\].*?)?\]\]?[^\]]*?)*)\]\]
            // now instead use allowed character list, then a file extension (these are mandatory on mediawiki), then optional closing ]]
            // this allows typo fixing and find&replace to operate on image descriptions
            // TODO: replace these two with direct returns from API, when available
            // or, alternatively, an image filename has to have a pipe or ]] after it if using the [[Image: start, so just set first one to 
            // @"[^\[\]\|\{\}]+\.[a-zA-Z]{3,4}\b(?:\s*(?:\]\]|\|))
            Images =
                new Regex(
                    @"\[\[" + Variables.NamespacesCaseInsensitive[Namespace.File] +
                    @"[ \%\!""$&'\(\)\*,\-.\/0-9:;=\?@A-Z\\\^_`a-z~\x80-\xFF\+]+\.[a-zA-Z]{3,4}\b(?:\s*(?:\]\]|\|))?|<[Gg]allery\b([^>]*?)>[\s\S]*?</ ?[Gg]allery>|\|\s*(?:[Pp]hoto|[Ii]mg|[Ii]mage\d*|[Cc]over)(?:[_ ]\w+)?\s*=[^\|{}]+?\.[a-zA-Z]{3,4}\s*(?=\||}})",
                    RegexOptions.Compiled | RegexOptions.Singleline);

            Stub = new Regex(@"{{.*?" + Variables.Stub + @"}}", RegexOptions.Compiled);

            PossiblyCommentedStub =
                new Regex(
                    @"(<!-- ?\{\{[^}]*?" + Variables.Stub + @"\b\}\}.*?-->|\{\{[^}]*?" + Variables.Stub + @"\}\})",
                    RegexOptions.Compiled);

            TemplateCall = new Regex(TemplateStart + @"\s*([^\]\|]*)\s*(.*)}}",
                                     RegexOptions.Compiled | RegexOptions.Singleline);

            LooseCategory =
                new Regex(@"\[\[[\s_]*" + Variables.NamespacesCaseInsensitive[Namespace.Category] 
                    + @"[\s_]*([^\|]*?)(|\|.*?)\]\]",
                    RegexOptions.Compiled);

            LooseImage = new Regex(@"\[\[\s*?(" + Variables.NamespacesCaseInsensitive[Namespace.File] 
                + @")\s*([^\|\]]+)(.*?)\]\]",
                RegexOptions.Compiled);

            months = "(" + string.Join("|", Variables.MonthNames) + ")";

            Dates = new Regex("^(0?[1-9]|[12][0-9]|3[01]) " + months + "$", RegexOptions.Compiled);
            Dates2 = new Regex("^" + months + " (0?[1-9]|[12][0-9]|3[01])$", RegexOptions.Compiled);

            string s = Variables.MagicWords.ContainsKey("redirect")
                    ? string.Join("|", Variables.MagicWords["redirect"].ToArray()).Replace("#", "")
                    : "REDIRECT";

            Redirect = new Regex(@"#(?:" + s + @")\s*:?\s*\[\[\s*:?\s*([^\|]*?)\s*(|\|.*?)]\]",
                                 RegexOptions.IgnoreCase | RegexOptions.Multiline);

            switch (Variables.LangCode)
            {
                case LangCodeEnum.ru:
                    s = "([Dd]isambiguation|[Dd]isambig|[Нн]еоднозначность)";
                    break;
                default:
                    s = "([234]CC|[Dd]isambig|[Gg]eodis|[Hh]ndis|[Ss]urname|[Nn]umberdis|[Rr]oaddis|[Ll]etter-disambig)";
                    break;
            }
            Disambigs = new Regex(TemplateStart + s + "}}", RegexOptions.Compiled);

            if (Variables.MagicWords.ContainsKey("defaultsort"))
                s = "(?i:" + string.Join("|", Variables.MagicWords["defaultsort"].ToArray()).Replace(":", "") + ")";
            else
                s = (Variables.LangCode == LangCodeEnum.en)
                    ? "(?:(?i:defaultsort(key|CATEGORYSORT)?))"
                    : "(?i:defaultsort)";

            Defaultsort = new Regex(TemplateStart + s + @"\s*[:|](?<key>[^\}]*)}}",
                                    RegexOptions.Compiled | RegexOptions.ExplicitCapture);

            //if (Variables.URL == Variables.URLLong)
            //    s = Regex.Escape(Variables.URL);
            //else
            //{
            int pos = Tools.FirstDifference(Variables.URL, Variables.URLLong);
            s = Regex.Escape(Variables.URLLong.Substring(0, pos));
            s += "(?:" + Regex.Escape(Variables.URLLong.Substring(pos)) + @"index\.php(?:\?title=|/)|"
                 + Regex.Escape(Variables.URL.Substring(pos)) + "/wiki/" + ")";
            //}
            ExtractTitle = new Regex("^" + s + "([^?&]*)$", RegexOptions.Compiled);

            string cat = Variables.NamespacesCaseInsensitive[Namespace.Category],
                img = Variables.NamespacesCaseInsensitive[Namespace.Image];

            EmptyLink = new Regex("\\[\\[(:?" + cat + "|" + img + "|)(|" + img + "|" + cat + "|.*?)\\]\\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            EmptyTemplate = new Regex(@"{{(" + Variables.NamespacesCaseInsensitive[Namespace.Template] + @")?[|\s]*}}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public static string months;

        // Covered by RegexTests.GenerateNamespaceRegex()
        /// <summary>
        /// Generates a regex template for all variants of one or more namespace,
        /// e.g. "File|Image"
        /// </summary>
        /// <param name="namespaces">One or more namespaces to process</param>
        public static string GenerateNamespaceRegex(params int[] namespaces)
        {
            StringBuilder sb = new StringBuilder(100 * namespaces.Length);
            foreach (int ns in namespaces)
            {
                if (ns == Namespace.Article) continue;

                if (sb.Length > 0) sb.Append('|');

                string nsName = Variables.Namespaces[ns];
                sb.Append(Tools.StripNamespaceColon(nsName));
                if (Variables.CanonicalNamespaces.ContainsKey(ns) 
                    && Variables.CanonicalNamespaces[ns] != nsName)
                {
                    sb.Append('|');
                    sb.Append(Tools.StripNamespaceColon(Variables.CanonicalNamespaces[ns]));
                }

                if (Variables.NamespaceAliases.ContainsKey(ns))
                    foreach (string s in Variables.NamespaceAliases[ns])
                    {
                        sb.Append('|');
                        sb.Append(Tools.StripNamespaceColon(s));
                    }
            }

            sb.Replace(" ", "[ _]");
            return sb.ToString();
        }

        /// <summary>
        /// Piece of template call, including curly brace and possible namespace
        /// </summary>
        public static string TemplateStart;

        /// <summary>
        /// Matches all wikilinks, categories, images etc. with nested links on same line
        /// </summary>
        public static readonly Regex SimpleWikiLink = new Regex(@"\[\[((?>[^\[\]\n]+|\[\[(?<DEPTH>)|\]\](?<-DEPTH>))*(?(DEPTH)(?!)))\]\]", RegexOptions.Compiled);

        /// <summary>
        /// Matches only internal wiki links
        /// </summary>
        public static readonly Regex WikiLinksOnly = new Regex(@"\[\[[^[\]\n]*?\]\]", RegexOptions.Compiled);

        /// <summary>
        /// Matches only internal wikilinks (with or without pipe) with extra word character(s) e.g. [[link]]age or [[here|link]]age
        /// http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Improve_HideText.HideMore.28.29
        /// </summary>
        public static readonly Regex WikiLinksOnlyPlusWord = new Regex(@"\[\[[^\[\]\n]+\]\]\w+", RegexOptions.Compiled);

        /// <summary>
        /// Group 1 Matches only the target of the wikilink
        /// </summary>
        public static readonly Regex WikiLink = new Regex(@"\[\[(.*?)(?:\]\]|\|)", RegexOptions.Compiled);

        /// <summary>
        /// Matches piped wikilinks, group 1 is target, group 2 the text
        /// </summary>
        public static readonly Regex PipedWikiLink = new Regex(@"\[\[([^[\]\n]*?)\|([^[\]\n]*?)\]\]", RegexOptions.Compiled);

        /// <summary>
        /// Matches unpiped wikilinks, group 1 is text
        /// </summary>
        public static readonly Regex UnPipedWikiLink = new Regex(@"\[\[([^\|]*?)\]\]", RegexOptions.Compiled);

        /// <summary>
        /// Matches link targets with encoded anchors
        /// </summary>
        public static readonly Regex AnchorEncodedLink = new Regex(@"#.*(_|\.[0-9A-Z]{2}).*", RegexOptions.Compiled);

        /// <summary>
        /// Matches {{DEFAULTSORT}}
        /// </summary>
        public static Regex Defaultsort;

        /// <summary>
        /// Matches all headings
        /// </summary>
        public static readonly Regex Heading = new Regex(@"^(=+)(.*?)(=+)", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// Matches headings of all levels
        /// </summary>
        public static readonly Regex Headings = new Regex(@"^={1,6}.*={1,6}\s*$", RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Matches text indented with a :
        /// </summary>
        public static readonly Regex IndentedText = new Regex(@"^:.*", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// Matches indented, bulleted or numbered text
        /// </summary>
        public static readonly Regex BulletedText = new Regex(@"^[\*#: ]+.*?$", RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Matches single line templates, NOT nested templates
        /// </summary>
        public static readonly Regex Template = new Regex(@"{{[^{\n]*?}}", RegexOptions.Compiled);

        /// <summary>
        /// Matches single and multiline templates, NOT nested templates
        /// </summary>
        public static readonly Regex TemplateMultiLine = new Regex(@"{{[^{]*?}}", RegexOptions.Compiled);

        /// <summary>
        /// Matches external links
        /// </summary>
        public static readonly Regex ExternalLinks = new Regex(@"(?:[Hh]ttp|[Hh]ttps|[Ff]tp|[Mm]ailto)://[^\ \n<>]*|\[(?:[Hh]ttp|[Hh]ttps|[Ff]tp|[Mm]ailto):.*?\]", RegexOptions.Compiled);

        /// <summary>
        /// Matches links that may be interwikis, i.e. containing colon
        /// </summary>
        public static readonly Regex PossibleInterwikis = new Regex(@"\[\[\s*([-a-z]+)\s*:\s*([^\]]*?)\s*\]\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Matches unformatted text regions: nowiki, pre, math, html comments, timelines
        /// </summary>
        public static readonly Regex UnFormattedText = new Regex(@"<nowiki>.*?</nowiki>|<pre>.*?</pre>|<math>.*?</math>|<!--.*?-->|<timeline>.*?</timeline>", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Matches <blockquote> tags
        /// </summary>
        public static readonly Regex Blockquote = new Regex(@"<\s*blockquote\s*>(.*?)<\s*/\s*blockquote\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary> 
        /// Matches <poem> tags
        /// </summary>
        public static readonly Regex Poem = new Regex(@"<\s*poem\s*>(.*?)<\s*/\s*poem\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Matches redirects
        /// Don't use directly, use Tools.IsRedirect() and Tools.RedirectTargetInstead
        /// </summary>
        public static Regex Redirect;

        /// <summary>
        /// Matches words
        /// </summary>
        public static readonly Regex RegexWordCount = new Regex(@"\w+", RegexOptions.Compiled);

        /// <summary>
        /// Matches <source></source> tags
        /// </summary>
        public static readonly Regex Source = new Regex(@"<\s*source(?:\s.*?|)>(.*?)<\s*/\s*source\s*>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches <code></code> tags
        /// </summary>
        public static readonly Regex Code = new Regex(@"<\s*code\s*>(.*?)<\s*/\s*code\s*>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches Dates like 21 January
        /// </summary>
        public static Regex Dates;

        /// <summary>
        /// Matches Dates like January 21
        /// </summary>
        public static Regex Dates2;

        /// <summary>
        /// Matches categories
        /// </summary>
        public static Regex Category;

        /// <summary>
        /// Matches images
        /// </summary>
        public static Regex Images;

        /// <summary>
        /// Matches disambig templates (en only)
        /// </summary>
        public static Regex Disambigs;

        /// <summary>
        /// Matches stubs
        /// </summary>
        public static Regex Stub;

        /// <summary>
        /// Matches both commented and uncommented stubs
        /// </summary>
        public static Regex PossiblyCommentedStub;

        /// <summary>
        /// 
        /// </summary>
        public static Regex LooseCategory;

        /// <summary>
        /// 
        /// </summary>
        public static Regex LooseImage;

        /// <summary>
        /// Matches quotations outside of templates but within a pair of quotation marks, notably exlcuding straight single quotes
        /// </summary>
        /// see http://en.wikipedia.org/wiki/Quotation_mark_glyphs
        /// http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Ignoring_spelling_errors_within_quotation_marks.3F
        public static readonly Regex UntemplatedQuotes = new Regex(@"\s[""«»‘’“”‛‟‹›“”„‘’`’“‘”].{1,500}?[""«»‘’“”‛‟‹›“”„‘’`’“‘”]", RegexOptions.Compiled);
        
        // covered by TestFixNonBreakingSpaces
        /// <summary>
        /// Matches abbreviated SI units without a non-breaking space
        /// </summary>
        /// http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Non_breaking_spaces
        public static readonly Regex SiUnitsWithoutNonBreakingSpaces = new Regex(@"\b(\d?\.?\d+)\s*((?:[cmknuµ])(?:m|g)|m?mol|cd)\b", RegexOptions.Compiled);

        #region en only
        /// <summary>
        /// Matches sic either in template or as bracketed text, also related {{typo}} template
        /// </summary>
        public static readonly Regex SicTag = new Regex(@"({{(?:[Ss]ic|[Tt]ypo)(?:\||}})|([\(\[{]\s*[Ss]ic!?\s*[\)\]}]))", RegexOptions.Compiled);

        /// <summary>
        /// Matches {{nofootnotes}} OR {{morefootnotes}} templates
        /// </summary>
        public static readonly Regex MoreNoFootnotes = new Regex(@"{{([Mm]ore|[Nn]o)footnotes}}", RegexOptions.Compiled);
        /// <summary>
        /// Matches any of the recognised templates for displaying cite references
        /// </summary>
        public static readonly Regex ReferencesTemplate = new Regex(@"(\{\{\s*ref(?:-?li(?:st|nk)|erence)[^{}]*\}\}|<references\s*/>|\{\{refs)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches {{lifetime}} and its aliases
        /// </summary>
        public static Regex Lifetime = new Regex(@"{{(?:[Ll]ifetime|BIRTH-DEATH-SORT|BD)\s*\|[^\}]*}}", RegexOptions.Compiled);

        /// <summary>
        /// Matches persondata (en only)
        /// </summary>
        public static readonly Regex Persondata = new Regex(@"{{ ?[Pp]ersondata.*?}}", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Matches {{Link FA|xxx}}, {{Link GA|xxx}}
        /// </summary>
        public static readonly Regex LinkFGAs = new Regex(@"{{[Ll]ink [FG]A\|.*?}}", RegexOptions.Compiled);

        /// <summary>
        /// Matches {{Deadend|xxx}} (en only)
        /// </summary>
        public static readonly Regex DeadEnd = new Regex(@"{{Deadend\|?(date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|.*?)?}}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex Wikify = new Regex(@"{{Wikify\|?(date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|.*?)?}}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex Orphan = new Regex(@"{{[Oo]rphan\|?(date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|.*?)?}}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        /// <summary>
        /// matches orphan tag within {{Article issues}} template
        /// </summary>
        public static readonly Regex OrphanArticleIssues = new Regex(@"{{\s*article\s*issues\s*\|[^{}]*?\borphan\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex Uncat = new Regex(@"{{Uncategori[zs]ed\|?(date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|.*?)?}}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex ReferenceList = new Regex("{{(reflist|references-small|references-2column)}}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        /// <summary>
        /// Checks for presence of infobox in article
        /// </summary>
        public static readonly Regex Infobox = new Regex(@"{{(?:\s*[Ii]nfobox\s|[^{}\|]+?[Ii]nfobox\s*\|).*?}}", RegexOptions.Compiled | RegexOptions.Singleline);
        
        // covered by DablinksTests
        /// <summary>
        /// Finds article disamiguation links from http://en.wikipedia.org/wiki/Wikipedia:Template_messages/General#Disambiguation_and_redirection (en only)
        /// </summary>
        public static readonly Regex Dablinks = new Regex(@"{{\s*(?:[Ff]or2?|[Dd]ablink|[Dd]istinguish2?|[Oo]therpeople[1-4]|[Oo]therpersons|[Oo]therplaces[23]?|[Oo]theruses-number|[Oo]theruses[1-4]?|[Rr]edirect-acronym|[Rr]edirect[2-4]?|[Aa]mbiguous link|[Dd]isambig-acronym)\s*(?:\|[^{}]*(?:{{[^{}]*}}[^{}]*)?)?}}", RegexOptions.Compiled);
        #endregion

        /// <summary>
        /// matches &lt;!-- comments --&gt;
        /// </summary>
        public static readonly Regex Comments = new Regex(@"<!--.*?-->", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex EmptyComments = new Regex(@"<!--[^\S\r\n]*-->", RegexOptions.Compiled);

        /// <summary>
        /// matches &lt;ref&gt; tags
        /// </summary>
        public static readonly Regex Refs = new Regex(@"(<ref\b[^>]*?>.*?<\s*/\s*ref\s*>|<ref\s+name\s*=\s*.*?/\s*>)", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// matches &lt;cite&gt; tags
        /// </summary>
        public static readonly Regex Cites = new Regex(@"<cite[^>]*?>[^<]*<\s*/cite\s*>", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// matches &lt;nowiki&gt; tags
        /// </summary>
        public static readonly Regex Nowiki = new Regex("<nowiki>.*?</nowiki>", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// matches template
        /// </summary>
        public static Regex TemplateCall;

        /// <summary>
        /// for checkpage parsing
        /// </summary>
        public static Regex NoGeneralFixes = new Regex("<!--No general fixes:.*?-->", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// For extraction of page title from URLs
        /// </summary>
        public static Regex ExtractTitle;

        /// <summary>
        /// 
        /// </summary>
        public static Regex EmptyLink;

        /// <summary>
        /// 
        /// </summary>
        public static Regex EmptyTemplate;
    }
}
