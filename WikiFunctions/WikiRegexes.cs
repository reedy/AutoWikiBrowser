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

            Category = new Regex(@"\[\[\s*" + Variables.NamespacesCaseInsensitive[Namespace.Category] +
                                 @"\s*(.*?)\s*(?:|\|([^\|\]]*))\s*\]\]", RegexOptions.Compiled);

            // Use allowed character list, then a file extension (these are mandatory on mediawiki), then optional closing ]]
            // this allows typo fixing and find&replace to operate on image descriptions
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

            Months = "(" + string.Join("|", Variables.MonthNames) + ")";
            MonthsNoGroup = "(?:" + string.Join("|", Variables.MonthNames) + ")";

            Dates = new Regex("^(0?[1-9]|[12][0-9]|3[01]) " + Months + "$", RegexOptions.Compiled);
            Dates2 = new Regex("^" + Months + " (0?[1-9]|[12][0-9]|3[01])$", RegexOptions.Compiled);

            string s = Variables.MagicWords.ContainsKey("redirect")
                    ? string.Join("|", Variables.MagicWords["redirect"].ToArray()).Replace("#", "")
                    : "REDIRECT";

            Redirect = new Regex(@"#(?:" + s + @")\s*:?\s*\[\[\s*:?\s*([^\|\[\]]*?)\s*(\|.*?)?\]\]", RegexOptions.IgnoreCase);

            switch (Variables.LangCode)
            {
                case "ru":
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
                s = (Variables.LangCode == "en")
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

        /// <summary>
        /// Matches the month names and provides a capturing group when used in a regular expression
        /// </summary>
        public static string Months;

        /// <summary>
        /// Matches the month names, without providing a capturing group when used in a regular expression
        /// </summary>
        public static string MonthsNoGroup;

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
        public static readonly Regex WikiLinksOnly = new Regex(@"\[\[[^[\]\n]+(?<!\[\[[A-Z]?[a-z-]{2,}:[^[\]\n]+)\]\]", RegexOptions.Compiled);

        /// <summary>
        /// Matches only internal wikilinks (with or without pipe) with extra word character(s) e.g. [[link]]age or [[here|link]]age
        /// http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Improve_HideText.HideMore.28.29
        /// </summary>
        public static readonly Regex WikiLinksOnlyPlusWord = new Regex(@"\[\[[^\[\]\n]+\]\](\w+)", RegexOptions.Compiled);

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
        public static readonly Regex UnPipedWikiLink = new Regex(@"\[\[([^\|\n]*?)\]\]", RegexOptions.Compiled);

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
        /// Matches level 2 headings
        /// </summary>
        public static readonly Regex HeadingLevelTwo = new Regex(@"^==([^=].*?[^=])==\s*$", RegexOptions.Multiline);
        
        /// <summary>
        /// Matches the whole of a level 2 section including heading and any subsections up to but not including the next level 2 section
        /// </summary>
        public static readonly Regex SectionLevelTwo = new Regex(@"^==[^=][^\r\n]*?[^=]==.*?(?=^==[^=][^\r\n]*?[^=]==(\r\n?|\n)$)", RegexOptions.Multiline | RegexOptions.Singleline);
        
        /// <summary>
        /// Matches the first section of an article, if the article has sections, else the whole article
        /// </summary>
        public static readonly Regex ZerothSection = new Regex("(^.+?(?===+)|^.+$)", RegexOptions.Singleline);
        
        /// <summary>
        /// Matches article text up to but not including first level 2 heading
        /// </summary>
        public static readonly Regex ArticleToFirstLevelTwoHeading = new Regex(@"^.*?(?=[^=]==[^=][^\r\n]*?[^=]==(\r\n?|\n))", RegexOptions.Singleline);
        
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
        /// Matches single and multiline templates, AND those with nested templates
        /// </summary>
        public static readonly Regex NestedTemplates = new Regex(@"{{(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}}");

        /// <summary>
        /// Matches templates: group 1 matches the names of templates
        /// </summary>
        public static readonly Regex TemplateName = new Regex(@"{{\s*([^\|{}]+?)(?=\s*(?:\||}}))");

        /// <summary>
        /// Matches external links
        /// </summary>
        public static readonly Regex ExternalLinks = new Regex(@"(?:[Hh]ttp|[Hh]ttps|[Ff]tp|[Mm]ailto)://[^\ \n<>]*|\[(?:[Hh]ttp|[Hh]ttps|[Ff]tp|[Mm]ailto):.*?\]", RegexOptions.Compiled);

        /// <summary>
        /// Matches links that may be interwikis, i.e. containing colon, group 1 being the wiki language, group 2 being the link target
        /// </summary>
        public static readonly Regex PossibleInterwikis = new Regex(@"\[\[\s*([-a-z]{2,12})\s*:\s*([^\]]*?)\s*\]\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Matches unformatted text regions: nowiki, pre, math, html comments, timelines
        /// </summary>
        public static readonly Regex UnFormattedText = new Regex(@"<nowiki>.*?</\s*nowiki>|<pre\b.*?>.*?</\s*pre>|<math\b.*?>.*?</\s*math>|<!--.*?-->|<timeline\b.*?>.*?</\s*timeline>", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Matches <blockquote> tags
        /// </summary>
        public static readonly Regex Blockquote = new Regex(@"<\s*blockquote\s*>(.*?)<\s*/\s*blockquote\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary> 
        /// Matches <poem> tags
        /// </summary>
        public static readonly Regex Poem = new Regex(@"<\s*poem\s*>(.*?)<\s*/\s*poem\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Matches &lt;imagemap&gt; tags
        /// </summary>
        public static readonly Regex ImageMap = new Regex(@"<\s*imagemap\b[^<>]*>(.*?)<\s*/\s*imagemap\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Matches &lt;noinclude&gt; tags
        /// </summary>
        public static readonly Regex Noinclude = new Regex(@"<\s*noinclude\s*>(.*?)<\s*/\s*noinclude\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Matches &lt;includeonly&gt; and &lt;onlyinclude&gt; tags
        /// </summary>
        public static readonly Regex Includeonly = new Regex(@"(?:<\s*includeonly\s*>.*?<\s*/\s*includeonly\s*>|<\s*onlyinclude\s*>.*?<\s*/\s*onlyinclude\s*>)", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Matches redirects
        /// Don't use directly, use Tools.IsRedirect() and Tools.RedirectTargetInstead
        /// </summary>
        public static Regex Redirect;

        /// <summary>
        /// Matches words
        /// </summary>
        public static readonly Regex RegexWord = new Regex(@"\w+", RegexOptions.Compiled);

        /// <summary>
        /// Matches words, and allows words with apostrophes to be treated as one whole word
        /// </summary>
        public static readonly Regex RegexWordApostrophes = new Regex(@"\w+(?:'\w+)?", RegexOptions.Compiled);

        /// <summary>
        /// Matches &lt;source&gt;&lt;/source&gt; tags
        /// </summary>
        public static readonly Regex Source = new Regex(@"<\s*source(?:\s.*?|)>(.*?)<\s*/\s*source\s*>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches &lt;code&gt;&lt;/code&gt; tags
        /// </summary>
        public static readonly Regex Code = new Regex(@"<\s*code\s*>(.*?)<\s*/\s*code\s*>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches math, pre, source, code tags or comments
        /// </summary>
        public static readonly Regex MathPreSourceCodeComments = new Regex(@"<pre>.*?</pre>|<!--.*?-->|<math>.*?</math>|" + Code + @"|" + Source, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

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
        public static readonly Regex UntemplatedQuotes = new Regex(@"(?<=[^\w])[""«»‘’“”‛‟‹›“”„‘’`’“‘”].{1,500}?[""«»‘’“”‛‟‹›“”„‘’`’“‘”](?=[^\w])", RegexOptions.Compiled | RegexOptions.Singleline);

        // covered by TestFixNonBreakingSpaces
        /// <summary>
        /// Matches abbreviated SI units without a non-breaking space, notably does not correct millimetres without a space due to firearms articles using this convention
        /// </summary>
        /// http://en.wikipedia.org/wiki/Wikipedia_talk:AutoWikiBrowser/Feature_requests#Non_breaking_spaces
        public static readonly Regex UnitsWithoutNonBreakingSpaces = new Regex(@"\b(\d?\.?\d+)\s*((?:[cmknuµ])(?:[mgWN])|m?mol|cd|mi|lb[fs]?|b?hp|mph|inch(?:es)?|ft|[kGM]Hz)\b(?<!(\d?\.?\d+)mm)", RegexOptions.Compiled);

        // covered by TestFixNonBreakingSpaces
        /// <summary>
        /// Matches "50m (170&nbsp;ft)"
        /// </summary>
        public static readonly Regex MetresFeetConversionNonBreakingSpaces = new Regex(@"(\d+(?:\.\d+)?) ?m(?= \(\d+(?:\.\d+)?&nbsp;ft\.?\))");

        /// <summary>
        /// Matches abbreviated in, feet or foot when in brackets e.g. (3 in); avoids false positives such as "3 in 4..."
        /// </summary>
        public static readonly Regex ImperialUnitsInBracketsWithoutNonBreakingSpaces = new Regex(@"(\(\d+(?:\.\d+)?(?:\s*(?:-|–|&mdash;)\s*\d+(?:\.\d+)?)?)\s*((?:in|feet|foot)\))", RegexOptions.Compiled);

        #region en only
        /// <summary>
        /// Matches sic either in template or as bracketed text, also related {{typo}} template
        /// </summary>
        public static readonly Regex SicTag = new Regex(@"({{(?:[Ss]ic|[Tt]ypo)(?:\||}})|([\(\[{]\s*[Ss]ic!?\s*[\)\]}]))", RegexOptions.Compiled);

        /// <summary>
        /// Matches {{nofootnotes}} OR {{morefootnotes}} templates
        /// </summary>
        public static readonly Regex MoreNoFootnotes = new Regex(@"{{([Mm]ore|[Nn]o) ?footnotes[^{}]*}}", RegexOptions.Compiled);

        /// <summary>
        /// Matches the various {{BLP unsourced}} templates
        /// </summary>
        public static readonly Regex BLPSources = new Regex(@"{{\s*([Bb](LP|lp) ?(sources|[Uu]n(sourced|ref(?:erenced)?))|[Uu]n(sourced|referenced) ?[Bb](LP|lp))\b", RegexOptions.Compiled);

        public const string ReferencesTemplates = @"(\{\{\s*ref(?:-?li(?:st|nk)|erence)(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))\}\}|<references\s*/>|\{\{refs|<references>.*</references>)";
        public const string ReferenceEndGR = @"</ref>|{{GR\|\d}}";

        /// <summary>
        /// Matches any of the recognised templates for displaying cite references
        /// </summary>
        public static readonly Regex ReferencesTemplate = new Regex(ReferencesTemplates, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches any of the recognised templates for displaying cite references followed by a &gt;ref&lt; reference
        /// </summary>
        public static readonly Regex RefAfterReflist = new Regex(ReferencesTemplates + ".*?" + ReferenceEndGR, RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches a line with a bare external link (no description or name of link)
        /// </summary>
        public static readonly Regex BareExternalLink = new Regex(@"^ *\*? *(?:[Hh]ttp|[Hh]ttps|[Ff]tp|[Mm]ailto)://[^\ \n\r<>]+\s+$", RegexOptions.Multiline);

        /// <summary>
        /// Matches {{lifetime}} and its aliases
        /// </summary>
        public static readonly Regex Lifetime = new Regex(@"{{(?:[Ll]ifetime|BIRTH-DEATH-SORT|BD)\s*\|[^\}]*}}", RegexOptions.Compiled | RegexOptions.RightToLeft);

        /// <summary>
        /// Matches the sorkey in a {{lifetime}} template and its aliases
        /// </summary>
        public static readonly Regex LifetimeSortkey = new Regex(@"{{(?:[Ll]ifetime|BIRTH-DEATH-SORT|BD)\s*\|[^\}\|]*\|[^\}\|]*\|\s*([^\}\|]+?)\s*}}", RegexOptions.Compiled | RegexOptions.RightToLeft);

        /// <summary>
        /// Matches persondata (en only)
        /// </summary>
        public static readonly Regex Persondata = new Regex(@"{{ ?[Pp]ersondata.*?}}", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Comment often put on the line before the Persondata template on the en-wiki
        /// </summary>
        public const string PersonDataCommentEN = @"<!-- Metadata: see [[Wikipedia:Persondata]] -->
";
        /// <summary>
        /// Matches the various categories for dead people on en wiki, and the living people category
        /// </summary>
        public static readonly Regex DeathsOrLivingCategory = new Regex(@"\[\[ ?Category ?:[ _]?(\d{1,2}\w{0,2}[- _]century(?: BC)?[ _]deaths|[0-9s]{3,5}(?: BC)?[ _]deaths|Disappeared[ _]people|Living[ _]people|Year[ _]of[ _]death[ _]missing|Possibly[ _]living[ _]people)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        /// <summary>
        /// Matches the {{recentlydeceased}} templates and its redirects
        /// </summary>
        public static readonly Regex LivingPeopleRegex2 = new Regex(@"\{\{(Template:)?(Recent ?death|Recentlydeceased)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static readonly Regex BirthsCategory = new Regex(@"\[\[ ?Category ?:[ _]?(?:(\d{3,4})(?:s| BC)?|\d{1,2}\w{0,2}[- _]century)[ _]births(\|.*?)?\]\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Matches the various {{birth date and age}} templates, group 1 being the year of birth
        /// </summary>
        public static readonly Regex DateBirthAndAge = new Regex(@"{{[Bb]irth(?: date(?: and age)?\s*(?:\|\s*[md]f\s*=\s*y(?:es)?\s*)?\|\s*|-date\s*\|[^{}\|]*?)(?:year *= *)?\b([12]\d{3})\s*(?:\||}})");

        /// <summary>
        /// Matches the various {{death date}} templates, group 1 being the year of death
        /// </summary>
        public static readonly Regex DeathDate = new Regex(@"{{[Dd]eath(?: date(?: and age)?\s*(?:\|\s*[md]f\s*=\s*y(?:es)?\s*)?\|\s*|-date\s*\|[^{}\|]*?)(?:year *= *)?\b([12]\d{3})\s*(?:\||}})");

        /// <summary>
        /// Matches the {{death date and age}} template, group 1 being the year of death, group 2 being the year of birth
        /// </summary>
        public static readonly Regex DeathDateAndAge = new Regex(@"{{[Dd](?:eath date and age|da)\s*\|(?:[^{}]*?\|)?\s*([12]\d{3})\s*\|[^{}]+?\|\s*([12]\d{3})\s*\|");

        /// <summary>
        /// Matches {{Link FA|xxx}}, {{Link GA|xxx}}
        /// </summary>
        public static readonly Regex LinkFGAs = new Regex(@"{{[Ll]ink [FG]A\|.*?}}", RegexOptions.Compiled | RegexOptions.RightToLeft);

        /// <summary>
        /// Matches {{Lien BA}}, {{Lien AdQ}}, {{Lien PdQ}} in French
        /// </summary>
        public static readonly Regex LinkFGAsFrench = new Regex(@"{{[Ll]ien (?:BA|[PA]dQ)\|.*?}}", RegexOptions.Compiled | RegexOptions.RightToLeft);

        /// <summary>
        /// Matches {{Deadend|xxx}} (en only)
        /// </summary>
        public static readonly Regex DeadEnd = new Regex(@"({{([Dd]ead ?end|[Ii]nternal ?links|[Nn]uevointernallinks|[Dd]ep)(\|(?:[^{}]+|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}))?}}|(?<={{[Aa]rticle\s*issues\b[^{}]*?)\|\s*deadend\s*=[^{}\|]+)", RegexOptions.Compiled);

        /// <summary>
        /// Matches {{wikify}} tag including within {{article issues}}
        /// </summary>
        public static readonly Regex Wikify = new Regex(@"({{Wikify(?:\s*\|\s*(date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|.*?))?}}|(?<={{Article\s*issues\b[^{}]*?)\|\s*wikify\s*=[^{}\|]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches {{dead link}} template and its redirects
        /// </summary>
        public static readonly Regex DeadLink = new Regex(@"{{((?:[Dd]ead|[Bb]roken) ?link|[Ll]ink ?broken|404|[Dd]l(?:-s)?|[Cc]leanup-link)\s*(\|((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}})|}})");

        /// <summary>
        /// Matches {{expand}} tag and its redirects and also {{expand}} within {{article issues}}
        /// </summary>
        public static readonly Regex Expand = new Regex(@"({{(?:Expand-?article|Expand|Develop|Elaborate|Expansion)(?:\s*\|\s*(?:date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|.*?))?}}|(?<={{Article\s*issues\b[^{}]*?)\|\s*expand\s*=[^{}\|]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches {{orphan}} tag
        /// </summary>
        public static readonly Regex Orphan = new Regex(@"{{[Oo]rphan(?:\s*\|\s*(date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|.*?))?}}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// matches orphan tag within {{Article issues}} template
        /// </summary>
        public static readonly Regex OrphanArticleIssues = new Regex(@"{{\s*article\s*issues\s*\|[^{}]*?\borphan\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// matches uncategorised templates: {{Uncat}}, {{Uncaegorised}}, {{Uncategorised stub}} allowing for nested subst: {{uncategorised|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}
        /// </summary>
        public static readonly Regex Uncat = new Regex(@"{{([Uu]ncat|[Cc]lassify|[Cc]at[Nn]eeded|[Uu]ncategori[sz]ed|[Cc]ategori[sz]e|[Cc]ategories needed|[Cc]ategory ?needed|[Cc]ategory requested|[Cc]ategories requested|[Nn]ocats?|[Uu]ncat-date|[Uu]ncategorized-date|[Nn]eeds cats?|[Cc]ats? needed|[Uu]ncategori[sz]edstub)((\s*\|[^{}]+)?\s*|\s*\|((?>[^\{\}]+|\{\{(?<DEPTH>)|\}\}(?<-DEPTH>))*(?(DEPTH)(?!))))\}\}", RegexOptions.Compiled);

        /// <summary>
        /// matches {{Article issues}} template
        /// </summary>
        public static readonly Regex ArticleIssues = new Regex(@"({{\s*[Aa]rticle ?issues(?:\s*\|[^{}]*)?\s*)}}");

        /// <summary>
        /// The cleanup templates that can be moved into the {{article issues}} template
        /// </summary>
        public const string ArticleIssuesTemplatesString = @"(3O|[Aa]dvert|[Aa]utobiography|[Bb]iased|[Bb]lpdispute|BLPrefimprove|BLPsources|BLPunref(?:erenced)?|BLPunsourced|[Cc]itations missing|[Cc]itationstyle|[Cc]itecheck|[Cc]leanup|COI|[Cc]oi|[Cc]olloquial|[Cc]onfusing|[Cc]ontext|[Cc]ontradict|[Cc]opyedit|[Cc]riticisms|[Cc]rystal|[Dd]eadend|[Dd]isputed|[Dd]o-attempt|[Ee]ssay(?:\-like)?|[Ee]xamplefarm|[Ee]xpand|[Ee]xpert|[Ee]xternal links|[Ff]ancruft|[Ff]ansite|[Ff]iction|[Gg]ameguide|[Gg]lobalize|[Gg]rammar|[Hh]istinfo|[Hh]oax|[Hh]owto|[Ii]nappropriate person|[Ii]n-universe|[Ii]mportance|[Ii]ncomplete|[Ii]ntro(?: length|\-too(?:long|short))|[Ii]ntromissing|[Ii]ntrorewrite|[Jj]argon|[Ll]aundry(?:lists)?|[Ll]ikeresume|[Ll]ong|[Nn]ewsrelease|[Nn]otab(?:le|ility)|[Oo]nesource|OR|[Oo]riginal research|[Oo]rphan|[Oo]r|[Oo]ut of date|[Pp]eacock|[Pp]lot|N?POV|n?pov|[Pp]rimarysources|[Pp]rose(?:line)?|[Qq]uotefarm|[Rr]ecent|[Rr]efimprove(?:BLP)?|[Rr]estructure|[Rr]eview|[Rr]ewrite|[Rr]oughtranslation|[Ss]ections|[Ss]elf-published|[Ss]pam|[Ss]tory|[Ss]ynthesis|[Tt]echnical|[Tt]one|[Tt]oo(?:short|long)|[Tt]ravelguide|[Tt]rivia|[Uu]nbalanced|[Uu]nencyclopedic|[Uu]nref(?:erenced(?:BLP)?|BLP)?|[Uu]pdate|[Ww]easel|[Ww]ikify)";

        /// <summary>
        /// RMatches the cleanup templates that can be moved into the {{article issues}} template
        /// </summary>
        public static readonly Regex ArticleIssuesTemplateNameRegex = new Regex(ArticleIssuesTemplatesString, RegexOptions.Compiled);

        /// <summary>
        /// Matches COI|OR|POV|BLP
        /// </summary>
        public static readonly Regex CoiOrPovBlp = new Regex("(COI|OR|POV|BLP)", RegexOptions.Compiled);

        
        /// <summary>
        /// matches the cleanup templates that can be moved into the {{article issues}} template, notably does not match templates with multiple parameters
        /// </summary>
        public static readonly Regex ArticleIssuesTemplates = new Regex(@"{{" + ArticleIssuesTemplatesString + @"\s*(?:\|\s*([^{}\|]*?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}[^{}\|]*?)?))?\s*}}");

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex ReferenceList = new Regex("{{(reflist|references-small|references-2column)}}", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

        /// <summary>
        /// Matches infoboxes, group 1 being the template name of the infobox
        /// </summary>
        public static readonly Regex InfoBox = new Regex(@"{{\s*([Ii]nfobox[\s_][^{}\|]+?|[^{}\|]+?[Ii]nfobox)\s*\|(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}}");

        // covered by DablinksTests
        /// <summary>
        /// Finds article disamiguation links from http://en.wikipedia.org/wiki/Wikipedia:Template_messages/General#Disambiguation_and_redirection (en only)
        /// </summary>
        public static readonly Regex Dablinks = new Regex(@"{{\s*(?:[Ff]or2?|[Dd]ablink|[Dd]istinguish2?|[Oo]therpeople[1-4]|[Oo]therpersons|[Oo]therplaces[23]?|[Oo]theruses-number|[Oo]theruse(?:s[1-4])?|2otheruses|[Rr]edirect-acronym|[Rr]edirect[2-4]?|[Aa]mbiguous link|[Dd]isambig-acronym)\s*(?:\|[^{}]*(?:{{[^{}]*}}[^{}]*)?)?}}", RegexOptions.Compiled);
        
        /// <summary>
        /// Matches {{XX Portal}} templates
        /// </summary>
        public static readonly Regex PortalTemplate = new Regex(@"{{[Pp]ortal(?:\|[^{}]+)?}}", RegexOptions.RightToLeft);
        #endregion

        /// <summary>
        /// matches &lt;!-- comments --&gt;
        /// </summary>
        public static readonly Regex Comments = new Regex(@"<!--.*?-->", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// matches text within &lt;p style...&gt;...&lt;/p&gt; HTML tags
        /// </summary>
        public static readonly Regex Pstyles = new Regex(@"<p style\s*=\s*[^<>]+>.*?<\s*/p>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches empty comments (zero or more whitespace)
        /// </summary>
        public static readonly Regex EmptyComments = new Regex(@"<!--[^\S\r\n]*-->", RegexOptions.Compiled);

        /// <summary>
        /// matches &lt;ref&gt; tags, including named references
        /// </summary>
        public static readonly Regex Refs = new Regex(@"(<ref\s+name\s*=\s*[^<>]*?/\s*>|<ref\b[^>/]*?>.*?<\s*/\s*ref\s*>)", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
        public static readonly Regex NoGeneralFixes = new Regex("<!--No general fixes:.*?-->", RegexOptions.Singleline | RegexOptions.Compiled);

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

        /// <summary>
        /// Matches bold italic text, group 1 being the text in bold italics
        /// </summary>
        public static readonly Regex BoldItalics = new Regex(@"'''''(.+?)'''''");

        /// <summary>
        /// Matches italic text, group 1 being the text in italics
        /// </summary>
        public static readonly Regex Italics = new Regex(@"''(.+?)''");

        /// <summary>
        /// Matches bold text, group 1 being the text in bold
        /// </summary>
        public static readonly Regex Bold = new Regex(@"'''(.+?)'''");

        /// <summary>
        /// Matches a row beginning with an asterisk, allowing for spaces before
        /// </summary>
        public static readonly Regex StarRows = new Regex(@"^ *(\*)(.*)", RegexOptions.Multiline);

        /// <summary>
        /// Matches the References level 2 heading
        /// </summary>
        public static readonly Regex ReferencesRegex = new Regex(@"== *References *==", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

        /// <summary>
        /// Matches the external links level 2 heading
        /// </summary>
        public static readonly Regex ExternalLinksRegex = new Regex(@"== *External +links? *==", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

        /// <summary>
        /// Matches parameters within the {{article issues}} template using title case (invalid casing)
        /// </summary>
        public static readonly Regex ArticleIssuesInTitleCase = new Regex(@"({{[Aa]rticle ?issues\|\s*(?:[^{}]+?\|\s*)?)([A-Z])([a-z]+(?: [a-z]+)?\s*=)", RegexOptions.Compiled);

        /// <summary>
        /// Matches the {{article issues}} template using the 'expert' parameter
        /// </summary>
        public static readonly Regex ArticleIssuesRegexExpert = new Regex(@"{{\s*[Aa]rticle ?issues[^{}]+?expert", RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex ArticleIssuesRegexWithDate = new Regex(@"({{\s*[Aa]rticle ?issues\s*(?:\|[^{}]*?)?)\|\s*date\s*=[^{}\|]{0,20}?(\||}})", RegexOptions.Compiled);

        /// <summary>
        /// Matches a number between 1000 and 2999
        /// </summary>
        public static readonly Regex GregorianYear = new Regex(@"\b[12]\d{3}\b", RegexOptions.Compiled);

        #region Parsers.FixFootnotes **NOT READY FOR PRODUCTION**
        // One space/linefeed
        public static readonly Regex WhitespaceRef = new Regex("[\\n\\r\\f\\t ]+?<ref([ >])", RegexOptions.Compiled);
        // remove trailing spaces from named refs
        public static readonly Regex RefTagWithParams = new Regex("<ref ([^>]*[^>])[ ]*>", RegexOptions.Compiled);
        // removed superscripted punctuation between refs
        public static readonly Regex SuperscriptedPunctuationBetweenRefs = new Regex("(</ref>|<ref[^>]*?/>)<sup>[ ]*[,;-]?[ ]*</sup><ref", RegexOptions.Compiled);
        public static readonly Regex PunctuationBetweenRefs = new Regex("(</ref>|<ref[^>]*?/>)[ ]*[,;-]?[ ]*<ref", RegexOptions.Compiled);

        private const string FactTag = "{{[ ]*(fact|fact[ ]*[\\|][^}]*|facts|citequote|citation needed|cn|verification needed|verify source|verify credibility|who|failed verification|nonspecific|dubious|or|lopsided|GR[ ]*[\\|][ ]*[^ ]+|[c]?r[e]?f[ ]*[\\|][^}]*|ref[ _]label[ ]*[\\|][^}]*|ref[ _]num[ ]*[\\|][^}]*)[ ]*}}";
        public static readonly Regex WhitespaceFactTag = new Regex("[\\n\\r\\f\\t ]+?" + FactTag, RegexOptions.Compiled);

        private const string LacksPunctuation = "([^\\.,;:!\\?\"'’])";
        private const string QuestionOrExclam = "([!\\?])";
        //private const string MinorPunctuation = "([\\.,;:])";
        private const string AnyPunctuation = "([\\.,;:!\\?])";
        private const string MajorPunctuation = "([,;:!\\?])";
        private const string Period = "([\\.])";
        private const string Quote = "([\"'’]*)";
        private const string Space = "[ ]*";

        private const string RefTag =
            "(<ref>([^<]|<[^/]|</[^r]|</r[^e]|</re[^f]|</ref[^>])*?</ref>" +
            "|<ref[^>]*?[^/]>([^<]|<[^/]|</[^r]|</r[^e]|</re[^f]" + "|</ref[^>])*?</ref>|<ref[^>]*?/>)";

        public static readonly Regex Match0A = new Regex(LacksPunctuation + Quote + FactTag + Space + AnyPunctuation, RegexOptions.Compiled);
        public static readonly Regex Match0B = new Regex(QuestionOrExclam + Quote + FactTag + Space + MajorPunctuation, RegexOptions.Compiled);
        //public static readonly Regex match0C = new Regex(MinorPunctuation + Quote + FactTag + space + AnyPunctuation, RegexOptions.Compiled);
        public static readonly Regex Match0D = new Regex(QuestionOrExclam + Quote + FactTag + Space + Period, RegexOptions.Compiled);

        public static readonly Regex Match1A = new Regex(LacksPunctuation + Quote + RefTag + Space + AnyPunctuation, RegexOptions.Compiled);
        public static readonly Regex Match1B = new Regex(QuestionOrExclam + Quote + RefTag + Space + MajorPunctuation, RegexOptions.Compiled);
        //public static readonly Regex match1C = new Regex(MinorPunctuation + Quote + RefTag + space + AnyPunctuation, RegexOptions.Compiled);
        public static readonly Regex Match1D = new Regex(QuestionOrExclam + Quote + RefTag + Space + Period, RegexOptions.Compiled);

        //public static readonly Regex RefAfterEquals = new Regex("(==*)<ref", RegexOptions.Compiled);
        #endregion

        /// <summary>
        /// matches "ibid" and "op cit"
        /// </summary>
        public static readonly Regex IbidOpCitation = new Regex(@"(?is)\b(ibid|op.{1,4}cit)\b", RegexOptions.Compiled);

        /// <summary>
        /// Matches the {{Inuse}} template
        /// </summary>
        public static readonly Regex InUse = new Regex(@"{{\s*[Ii]nuse\s*[\}\|]", RegexOptions.Compiled);

        /// <summary>
        /// Matches consecutive whitespace
        /// </summary>
        public static readonly Regex WhiteSpace = new Regex(@"\s+", RegexOptions.Compiled);
    }
}
