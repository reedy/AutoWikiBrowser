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
using System.Collections.Generic;

namespace WikiFunctions
{
    /// <summary>
    /// Provides some common static regexes 
    /// </summary>
    public static class WikiRegexes
    {
        public static void MakeLangSpecificRegexes()
        {
            NamespacesCaseInsensitive = new Dictionary<int,Regex>();
            foreach (var p in Variables.NamespacesCaseInsensitive)
            {
                NamespacesCaseInsensitive[p.Key] = new Regex(p.Value, RegexOptions.Compiled);
            }

            TemplateStart = @"\{\{\s*(:?" + Variables.NamespacesCaseInsensitive[Namespace.Template] + ")?";

            Category = new Regex(@"\[\[\s*" + Variables.NamespacesCaseInsensitive[Namespace.Category] +
                                 @"\s*(.*?)\s*(?:|\|([^\|\]]*))\s*\]\]", RegexOptions.Compiled);

            // Use allowed character list, then a file extension (these are mandatory on mediawiki), then optional closing ]]
            // this allows typo fixing and find&replace to operate on image descriptions
            // or, alternatively, an image filename has to have a pipe or ]] after it if using the [[Image: start, so just set first one to 
            // @"[^\[\]\|\{\}]+\.[a-zA-Z]{3,4}\b(?:\s*(?:\]\]|\|))
            // handles <gallery> and {{gallery}} too
            Images =
                new Regex(
                    @"\[\[\s*" + Variables.NamespacesCaseInsensitive[Namespace.File] +
                    @"[ \%\!""$&'\(\)\*,\-.\/0-9:;=\?@A-Z\\\^_`a-z~\x80-\xFF\+]+\.[a-zA-Z]{3,4}\b(?:\s*(?:\]\]|\|))?|<[Gg]allery\b([^>]*?)>[\s\S]*?</ ?[Gg]allery>|{{\s*[Gg]allery\s*(?:\|(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!)))?}}|(?<=\|\s*[a-zA-Z\d_ ]+\s*=)[^\|{}]+?\.[a-zA-Z]{3,4}(?=\s*(?:\||}}))",
                    RegexOptions.Compiled | RegexOptions.Singleline);
            
            FileNamespaceLink = new Regex(@"\[\[\s*" + Variables.NamespacesCaseInsensitive[Namespace.File] +
                                          @"((?>[^\[\]]+|\[\[(?<DEPTH>)|\]\](?<-DEPTH>))*(?(DEPTH)(?!)))\]\]", RegexOptions.Compiled);

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
            
            InternationalDates = new Regex(@"\b([1-9]|[12][0-9]|3[01]) +" + Months + @" +([12]\d{3})\b", RegexOptions.Compiled);
            AmericanDates = new Regex(Months + @" +([1-9]|[12][0-9]|3[01]),? +([12]\d{3})\b", RegexOptions.Compiled);

            DayMonth = new Regex(@"\b([1-9]|[12][0-9]|3[01]) +" + Months + @"\b", RegexOptions.Compiled);
            MonthDay = new Regex(Months + @" +([1-9]|[12][0-9]|3[01])\b", RegexOptions.Compiled);
            
            DayMonthRangeSpan = new Regex(@"\b((?:[1-9]|[12][0-9]|3[01])(?:–|&ndash;|{{ndash}}|\/)(?:[1-9]|[12][0-9]|3[01])) " + Months + @"\b", RegexOptions.Compiled);
            
            MonthDayRangeSpan = new Regex(Months + @" ((?:[1-9]|[12][0-9]|3[01])(?:–|&ndash;|{{ndash}}|\/)(?:[1-9]|[12][0-9]|3[01]))\b", RegexOptions.Compiled);

            List<string> magic;
            string s = Variables.MagicWords.TryGetValue("redirect", out magic)
                           ? string.Join("|", magic.ToArray()).Replace("#", "")
                           : "REDIRECT";

            Redirect = new Regex(@"#(?:" + s + @")\s*:?\s*\[\[\s*:?\s*([^\|\[\]]*?)\s*(\|.*?)?\]\]", RegexOptions.IgnoreCase);

            switch (Variables.LangCode)
            {
                case "ru":
                    s = "([Dd]isambiguation|[Dd]isambig|[Нн]еоднозначность|[Мm]ногозначность)";
                    break;
                default:
                    s = "([Dd]isamb(?:ig(?:uation)?)?|[Dd]ab|[Mm]athdab|[Ss]urname|(?:[Nn]umber|[Rr]oad?|[Hh]ospital|[Gg]eo|[Hh]n|[Ss]chool)dis|SIA|[Ll]etter-disambig|[Ss]hipindex|[Mm]ountainindex|[[Aa]irport disambig|[Cc]allsigndis|[Dd]isambig-cleanup)";
                    break;
            }
            Disambigs = new Regex(TemplateStart + s + @"\s*(?:\|[^{}]*?)?}}", RegexOptions.Compiled);

            if (Variables.MagicWords.TryGetValue("defaultsort", out magic))
                s = "(?i:" + string.Join("|", magic.ToArray()).Replace(":", "") + ")";
            else
                s = (Variables.LangCode == "en")
                    ? "(?:(?i:defaultsort(key|CATEGORYSORT)?))"
                    : "(?i:defaultsort)";

            Defaultsort = new Regex(TemplateStart + s + @"\s*[:|](?<key>(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))|[^\}\r\n]*?)(?:}}|\r|\n)",
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
            
            // set orphan, wikify, uncat templates & dateparameter string
            string orphantemplate = "", uncattemplate = "";
            switch(Variables.LangCode)
            {
                case "sv":
                    orphantemplate = @"Föräldralös";
                    uncattemplate = "[Oo]kategoriserad";
                    Wikify = new Regex(@"{{\s*Ickewiki(?:\s*\|\s*(date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|.*?))?}}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    DateYearMonthParameter = @"datum={{subst:CURRENTYEAR}}-{{subst:CURRENTMONTH}}";
                    break;
                default:
                    orphantemplate = "Orphan";
                    uncattemplate = @"([Uu]ncat|[Cc]lassify|[Cc]at[Nn]eeded|[Uu]ncategori[sz]ed|[Cc]ategori[sz]e|[Cc]ategories needed|[Cc]ategory ?needed|[Cc]ategory requested|[Cc]ategories requested|[Nn]ocats?|[Uu]ncat-date|[Uu]ncategorized-date|[Nn]eeds cats?|[Cc]ats? needed|[Uu]ncategori[sz]edstub)";
                    Wikify = new Regex(@"({{\s*Wikify(?:\s*\|\s*(date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|.*?))?}}|(?<={{\s*(?:Article|Multiple)\s*issues\b[^{}]*?)\|\s*wikify\s*=[^{}\|]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    DateYearMonthParameter = @"date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}";
                    break;
            }
            Orphan = new Regex(@"{{\s*" + orphantemplate + @"(?:\s*\|\s*(" + DateYearMonthParameter + @"|.*?))?}}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            Uncat = new Regex(@"{{\s*" + uncattemplate + @"((\s*\|[^{}]+)?\s*|\s*\|((?>[^\{\}]+|\{\{(?<DEPTH>)|\}\}(?<-DEPTH>))*(?(DEPTH)(?!))))\}\}", RegexOptions.Compiled);
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
                string canNS;
                if (Variables.CanonicalNamespaces.TryGetValue(ns, out canNS)
                    && canNS != nsName)
                {
                    sb.Append('|');
                    sb.Append(Tools.StripNamespaceColon(canNS));
                }

                List<string> nsAlias;
                if (Variables.NamespaceAliases.TryGetValue(ns, out nsAlias))
                    foreach (string s in nsAlias)
                    {
                        sb.Append('|');
                        sb.Append(Tools.StripNamespaceColon(s));
                    }
            }

            sb.Replace(" ", "[ _]");
            return sb.ToString();
        }


        /// <summary>
        /// Variables.NamespacesCaseInsensitive compiled into regexes
        /// </summary>
        public static Dictionary<int, Regex> NamespacesCaseInsensitive;

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
        /// Matches only internal wiki links, possibly with pipe; group 1 is target, group 2 is pipe text, if piped link
        /// </summary>
        public static readonly Regex WikiLinksOnlyPossiblePipe = new Regex(@"\[\[([^[\]\|\n]+)(?<!\[\[[A-Z]?[a-z-]{2,}:[^[\]\n]+)(?:\|([^[\]\|\n]+))?\]\]", RegexOptions.Compiled);

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
        public static readonly Regex Headings = new Regex(@"^={1,6}(.*?)={1,6}\s*$", RegexOptions.Multiline | RegexOptions.Compiled);
        
        /// <summary>
        /// Matches level 2 headings
        /// </summary>
        public static readonly Regex HeadingLevelTwo = new Regex(@"^==([^=](?:.*?[^=])?)==\s*$", RegexOptions.Multiline);
        
        /// <summary>
        /// Matches level 3 headings
        /// </summary>
        public static readonly Regex HeadingLevelThree = new Regex(@"^===([^=](?:.*?[^=])?)===\s*$", RegexOptions.Multiline);
        
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
        /// 
        /// </summary>
        public static readonly Regex TemplateEnd = new Regex(@" *}}$", RegexOptions.Compiled);

        /// <summary>
        /// Matches single and multiline templates, NOT nested templates
        /// </summary>
        public static readonly Regex TemplateMultiline = new Regex(@"{{[^{]*?}}", RegexOptions.Compiled);

        /// <summary>
        /// Matches single and multiline templates, AND those with nested templates
        /// </summary>
        public static readonly Regex NestedTemplates = new Regex(@"{{(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}}");

        /// <summary>
        /// Matches templates: group 1 matches the names of templates. Not for templates including the template namespace prefix
        /// </summary>
        public static readonly Regex TemplateName = new Regex(@"{{\s*([^\|{}]+?)(?:\s*<!--.*?-->\s*)?\s*(?:\||}})");

        /// <summary>
        /// Matches external links
        /// </summary>
        public static readonly Regex ExternalLinks = new Regex(@"(https?|ftp|mailto|irc|gopher|telnet|nntp|worldwind|news|svn)://(?:[\w\._\-~!/\*""'\(\):;@&=+$,\\\?%#\[\]]+?(?=}})|[\w\._\-~!/\*""'\(\):;@&=+$,\\\?%#\[\]]*)|\[(https?|ftp|mailto|irc|gopher|telnet|nntp|worldwind|news|svn)://.*?\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches links that may be interwikis, i.e. containing colon, group 1 being the wiki language, group 2 being the link target
        /// </summary>
        public static readonly Regex PossibleInterwikis = new Regex(@"\[\[\s*([-a-z]{2,12})\s*:\s*([^\]]*?)\s*\]\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Matches unformatted text regions: nowiki, pre, math, html comments, timelines
        /// </summary>
        public static readonly Regex UnformattedText = new Regex(@"<nowiki>.*?</\s*nowiki>|<pre\b.*?>.*?</\s*pre>|<math\b.*?>.*?</\s*math>|<!--.*?-->|<timeline\b.*?>.*?</\s*timeline>", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Matches &lt;blockquote> tags
        /// </summary>
        public static readonly Regex Blockquote = new Regex(@"<\s*blockquote\s*>(.*?)<\s*/\s*blockquote\s*>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary> 
        /// Matches &ltpoem> tags
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
        
        public const string RFromModificationString = @"{{R from modification}}";        
        public static readonly List<string> RFromModificationList = new List<string>(@"R from alternative punctuation,R mod,R from modifcation,R from modification,R from alternate punctuation".Split(','));
        
        public const string RFromTitleWithoutDiacriticsString = @"{{R from title without diacritics}}";
        public static readonly List<string> RFromTitleWithoutDiacriticsList = new List<string>(@"R from name without diacritics,R from original name without diacritics,R from title without diacritics,R to accents,R to diacritics,R to title with diacritics,R to unicode,R to unicode name,R without diacritics,RDiacr,Redirects from title without diacritics".Split(','));
        
        public const string RFromOtherCapitaliastionString = @"{{R from other capitalisation}}";
        public static readonly List<string> RFromOtherCapitaliastionList = new List<string>(@"R cap,R for alternate capitalisation,R for alternate capitalization,R for alternative capitalisation,R for alternative capitalization,R from Capitalisation,R from Capitalization,R from alt cap,R from alt case,R from alternate capitalisation,R from alternate capitalization,R from alternative capitalisation,R from alternative capitalization,R from another capitalisation,R from another capitalization,R from cap,R from capitalisation,R from capitalization,R from caps,R from different capitalization ,R from lowercase,R from miscapitalization,R from other Capitalization,R from other capitalisation,R from other capitalization,R from other caps,R to Capitalization,R to alternate capitalisation,Redirect from capitalisation".Split(','));
                
        /// <summary>
        /// Matches words
        /// </summary>
        public static readonly Regex RegexWord = new Regex(@"\w+", RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex Newline = new Regex("\\n", RegexOptions.Compiled);

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
        /// Matches images (file namespace links and parameters in infoboxes etc.)
        /// </summary>
        public static Regex Images;
        
        /// <summary>
        /// Matches links to the file namespace (images etc.)
        /// </summary>
        public static Regex FileNamespaceLink;

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
        public static readonly Regex UnitsWithoutNonBreakingSpaces = new Regex(@"\b(\d?\.?\d+)\s*((?:[cmknuµ])(?:[mgWN])|m?mol|cd|mi|lb[fs]?|b?hp|mph|inch(?:es)?|ft|[kGM]Hz|gram(?:me)?s?)\b(?<!(\d?\.?\d+)mm)", RegexOptions.Compiled);

        // covered by TestFixNonBreakingSpaces
        /// <summary>
        /// Matches "50m (170&nbsp;ft)"
        /// </summary>
        public static readonly Regex MetresFeetConversionNonBreakingSpaces = new Regex(@"(\d+(?:\.\d+)?) ?m(?= \(\d+(?:\.\d+)?&nbsp;ft\.?\))");

        /// <summary>
        /// Matches abbreviated in, feet or foot when in brackets e.g. (3 in); avoids false positives such as "3 in 4..."
        /// </summary>
        public static readonly Regex ImperialUnitsInBracketsWithoutNonBreakingSpaces = new Regex(@"(\(\d+(?:\.\d+)?(?:\s*(?:-|–|&mdash;)\s*\d+(?:\.\d+)?)?)\s*((?:in|feet|foot|oz)\))", RegexOptions.Compiled);

        #region en only
        /// <summary>
        /// Matches sic either in template or as bracketed text, also related {{typo}} template
        /// </summary>
        public static readonly Regex SicTag = new Regex(@"({{\s*(?:[Ss]ic|[Tt]ypo)(?:\||}})|([\(\[{]\s*[Ss]ic!?\s*[\)\]}]))", RegexOptions.Compiled);
        
        /// <summary>
        /// Matches the {{use dmy dates}} family of templates
        /// </summary>
        public static readonly Regex UseDatesTemplate = new Regex(@"{{\s*[Uu]se (dmy|mdy|ymd) dates\s*}}", RegexOptions.Compiled);
        
        /// <summary>
        /// Matches dates in American format – "Month dd, YYYY"
        /// </summary>
        public static Regex AmericanDates;
        
        /// <summary>
        /// Matches dates in International format – "dd Month YYYY"
        /// </summary>
        public static Regex InternationalDates;
        
        /// <summary>
        /// Matches month day combinations in American format – "Month dd"
        /// </summary>
        public static Regex MonthDay;
        
        /// <summary>
        /// Matches day month combinations in International format – "dd Month"
        /// </summary>
        public static Regex DayMonth;
        
        /// <summary>
        /// Matches month day ranges – "May 13–17"
        /// </summary>
        public static Regex MonthDayRangeSpan;
        
        /// <summary>
        /// Matches day month ranges – "13–17 May"
        /// </summary>
        public static Regex DayMonthRangeSpan;
        
        // strictly should accept year form 1583
        /// <summary>
        /// Matches ISO 8601 format dates – YYYY-DD-MM – between 1600 and 2099
        /// </summary>
        public static readonly Regex ISODates = new Regex(@"\b(20\d\d|1[6-9]\d\d)-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])\b", RegexOptions.Compiled);
        
        /// <summary>
        /// Matches the {{talk header}} templates and its redirects
        /// </summary>
        public static readonly Regex TalkHeaderTemplate = new Regex(@"\{\{\s*(?:template *:)?\s*(talk[ _]?(page)?(header)?)\s*(?:\|[^{}]*)?\}\}\s*", 
           RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex SkipTOCTemplateRegex = new Regex(
            @"\{\{\s*(template *:)?\s*(skiptotoctalk|Skiptotoc|Skip to talk)\s*\}\}\s*",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches {{nofootnotes}} OR {{morefootnotes}} templates
        /// </summary>
        public static readonly Regex MoreNoFootnotes = new Regex(@"{{\s*([Mm]ore|[Nn]o) ?footnotes[^{}]*}}", RegexOptions.Compiled);

        /// <summary>
        /// Matches the various {{BLP unsourced}} templates
        /// </summary>
        public static readonly Regex BLPSources = new Regex(@"{{\s*([Bb](LP|lp) ?(sources|[Uu]n(sourced|ref(?:erenced)?))|[Uu]n(sourced|referenced) ?[Bb](LP|lp))\b", RegexOptions.Compiled);

        public const string ReferencesTemplates = @"(\{\{\s*(?:ref(?:-?li(?:st|nk)|erence)|[Ll]istaref)(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))\}\}|<\s*references\s*/>|\{\{refs|<\s*references\s*>.*</\s*references\s*>)";
        public const string ReferenceEndGR = @"(?:</ref>|{{GR\|\d}})";

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
        /// Matches a bare external link (URL only, no title) within a &lt;ref&gt; tag, group 1 being the URL
        /// </summary>
        public static readonly Regex BareRefExternalLink = new Regex(@"<\s*ref\b[^<>]*>\s*\[*\s*((?:https?|ftp|mailto)://[^\ \n\r<>]+?)\s*\]*[\.,:;\s]*<\s*/\s*ref\s*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Matches an external link (URL only, no title) within a &lt;ref&gt; tag with a bot generated title or no title, group 1 being the URL
        /// </summary>
        public static readonly Regex BareRefExternalLinkBotGenTitle = new Regex(@"<\s*ref\b[^<>]*>\s*\[*\s*((?:https?|ftp|mailto)://[^\ \n\r<>]+?)\s*(?:\s+[^<>{}]+?<!--\s*Bot generated title\s*-->)?\]*[\.,:;\s]*<\s*/\s*ref\s*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        /// <summary>
        /// Matches the various citation templates {{citation}}, {{cite web}} etc. on en-wiki
        /// </summary>
        public static readonly Regex CiteTemplate = new Regex(@"{{\s*([Cc]it[ae][^{}\|]*?)\s*(\|(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!)))?}}", RegexOptions.Compiled);
        
        /// <summary>
        /// Matches persondata (en only)
        /// </summary>
        public static readonly Regex Persondata = Tools.NestedTemplateRegex(@"Persondata");

        /// <summary>
        /// Comment often put on the line before the Persondata template on the en-wiki
        /// </summary>
        public const string PersonDataCommentEN = @"<!-- Metadata: see [[Wikipedia:Persondata]] -->
";
        /// <summary>
        /// Matches the various categories for dead people on en wiki, and the living people category
        /// </summary>
        public static readonly Regex DeathsOrLivingCategory = new Regex(@"\[\[\s*Category *:[ _]?(\d{1,2}\w{0,2}[- _]century(?: BC)?[ _]deaths|[0-9s]{3,5}(?: BC)?[ _]deaths|Disappeared[ _]people|Living[ _]people|Year[ _]of[ _]death[ _](?:missing|unknown)|Possibly[ _]living[ _]people)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        /// <summary>
        /// Matches the {{recentlydeceased}} templates and its redirects
        /// </summary>
        public static readonly Regex LivingPeopleRegex2 = new Regex(@"\{\{\s*(Template:)?(Recent ?death|Recentlydeceased)\}\}", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        
        /// <summary>
        /// Matches the XXXX births / xxth Century / XXXX BC births categories (en only)
        /// </summary>
        public static readonly Regex BirthsCategory = new Regex(@"\[\[ ?Category ?:[ _]?(?:(\d{3,4})(?:s| BC)?|\d{1,2}\w{0,2}[- _]century)[ _]births(\|.*?)?\]\]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Matches the various {{birth date and age}} templates, group 1 being the year of birth
        /// </summary>
        public static readonly Regex DateBirthAndAge = new Regex(@"{{\s*[Bb](?:(?:da|irth date(?: and age)?)\s*(?:\|\s*[md]f\s*=\s*y(?:es)?\s*)?\|\s*|irth-date\s*\|[^{}\|]*?)(?:year *= *)?\b([12]\d{3})\s*(?:\||}})");

        /// <summary>
        /// Matches the various {{death date}} templates, group 1 being the year of death
        /// </summary>
        public static readonly Regex DeathDate = new Regex(@"{{\s*[Dd]eath(?: date(?: and age)?\s*(?:\|\s*[md]f\s*=\s*y(?:es)?\s*)?\|\s*|-date\s*\|[^{}\|]*?)(?:year *= *)?\b([12]\d{3})\s*(?:\||}})");

        /// <summary>
        /// Matches the {{death date and age}} template, group 1 being the year of death, group 2 being the year of birth
        /// </summary>
        public static readonly Regex DeathDateAndAge = new Regex(@"{{\s*[Dd](?:eath[ -]date and age|da)\s*\|(?:[^{}]*?\|)?\s*([12]\d{3})\s*\|[^{}]+?\|\s*([12]\d{3})\s*\|");

        /// <summary>
        /// Matches {{Link FA|xxx}}, {{Link GA|xxx}}, {{Link FL|xxx}}
        /// </summary>
        public static readonly Regex LinkFGAs = new Regex(@"{{\s*[Ll]ink (?:[FG]A|FL)\|.*?}}", RegexOptions.Compiled | RegexOptions.RightToLeft);

        /// <summary>
        /// Matches {{Lien BA}}, {{Lien AdQ}}, {{Lien PdQ}} in French
        /// </summary>
        public static readonly Regex LinkFGAsFrench = new Regex(@"{{\s*[Ll]ien (?:BA|[PA]dQ)\|.*?}}", RegexOptions.Compiled | RegexOptions.RightToLeft);
        
        /// <summary>
        /// Matches {{{Link AdQ}} and {{Link FA}} in Italian
        /// </summary>
        public static readonly Regex LinkFGAsItalian = new Regex(@"{{\s*[Ll]ink (FA|AdQ)\|.*?}}", RegexOptions.Compiled | RegexOptions.RightToLeft);

        /// <summary>
        /// Matches {{{Destacado}} and {{Link FA}} in Spanish
        /// </summary>
        public static readonly Regex LinkFGAsSpanish = new Regex(@"{{\s*([Ll]ink FA|[Dd]estacado)\|.*?}}", RegexOptions.Compiled | RegexOptions.RightToLeft);

        /// <summary>
        /// Matches {{{Enllaç AD}} and {{Link FA}} in Spanish
        /// </summary>
        public static readonly Regex LinkFGAsCatalan = new Regex(@"{{\s*([Ll]ink FA|[Ee]nllaç AD)\|.*?}}", RegexOptions.Compiled | RegexOptions.RightToLeft);
        
        /// <summary>
        /// Matches the {{Link FA|xxx}} template used on ar-wiki
        /// </summary>
        public static readonly Regex LinkFGAsArabic = new Regex(@"{{\s*وصلة مقالة مختارة\s*\|.*?}}", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Matches {{Deadend|xxx}} (en only)
        /// </summary>
        public static readonly Regex DeadEnd = new Regex(@"({{\s*([Dd]ead ?end|[Ii]nternal ?links|[Nn]uevointernallinks|[Dd]ep)(\|(?:[^{}]+|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}))?}}|(?<={{\s*(?:[Aa]rticle|[Mm]ultiple)\s*issues\b[^{}]*?)\|\s*dead ?end\s*=[^{}\|]+)", RegexOptions.Compiled);

        /// <summary>
        /// Matches {{wikify}} tag (including within {{multiple issues}} for en-wiki)
        /// </summary>
        public static Regex Wikify;

        /// <summary>
        /// Matches {{dead link}} template and its redirects
        /// </summary>
        public static readonly Regex DeadLink = new Regex(@"{{\s*((?:[Dd]ead|[Bb]roken) ?link|[Ll]ink ?broken|404|[Dd]l(?:-s)?|[Cc]leanup-link)\s*(\|((?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}})|}})");

        /// <summary>
        /// Matches {{expand}} tag and its redirects and also {{expand}} within {{multiple issues}}
        /// </summary>
        public static readonly Regex Expand = new Regex(@"({{\s*(?:Expand-?article|Expand|Develop|Elaborate|Expansion)(?:\s*\|\s*(?:date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}|.*?))?}}|(?<={{(?:Article|Multiple)\s*issues\b[^{}]*?)\|\s*expand\s*=[^{}\|]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches {{orphan}} tag
        /// </summary>
        public static Regex Orphan;

        /// <summary>
        /// matches orphan tag within {{Article issues}} template
        /// </summary>
        public static readonly Regex OrphanArticleIssues = new Regex(@"{{\s*(?:article|multiple)\s*issues\s*\|[^{}]*?\borphan\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// matches uncategorised templates: {{Uncat}}, {{Uncaegorised}}, {{Uncategorised stub}} allowing for nested subst: {{uncategorised|date={{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}}}
        /// </summary>
        public static Regex Uncat;

        /// <summary>
        /// matches {{Article issues}} template
        /// </summary>
        public static readonly Regex ArticleIssues = new Regex(@"({{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues(?:\s*\|[^{}]*)?\s*)}}", RegexOptions.Compiled);

        /// <summary>
        /// matches {{New unreviewed article}} template
        /// </summary>
        public static readonly Regex NewUnReviewedArticle = new Regex(@"({{\s*[Nn]ew unreviewed article(?:\s*\|[^{}]*)?\s*)}}", RegexOptions.Compiled);

        /// <summary>
        /// The cleanup templates that can be moved into the {{article issues}} template
        /// </summary>
        public const string ArticleIssuesTemplatesString = @"(3O|[Aa]dvert|[Aa]utobiography|[Bb]iased|[Bb]lpdispute|BLPrefimprove|BLPsources|BLPunref(?:erenced)?|BLPunsourced|[Cc]itations missing|[Cc]itationstyle|[Cc]itecheck|[Cc]leanup|COI|[Cc]oi|[Cc]olloquial|[Cc]onfusing|[Cc]ontext|[Cc]ontradict|[Cc]opyedit|[Cc]riticisms|[Cc]rystal|[Dd]ead ?end|[Dd]isputed|[Dd]o-attempt|[Ee]ssay(?:\-like)?|[Ee]xamplefarm|[Ee]xpand|[Ee]xpert|[Ee]xternal links|[Ff]ancruft|[Ff]ansite|[Ff]iction|[Gg]ameguide|[Gg]lobalize|[Gg]rammar|[Hh]istinfo|[Hh]oax|[Hh]owto|[Ii]nappropriate person|[Ii]n-universe|[Ii]ncomplete|[Ii]ntro(?: length|\-too(?:long|short))|[Ii]ntromissing|[Ii]ntrorewrite|[Jj]argon|[Ll]aundry(?:lists)?|[Ll]ike ?resume|[Ll]ong|[Nn]ewsrelease|[Nn]otab(?:le|ility)|[Oo]nesource|[Oo]riginal[ -][Rr]esearch|[Oo]rphan|[Oo]ut of date|[Pp]eacock|[Pp]lot|N?POV|n?pov|[Pp]rimarysources|[Pp]rose(?:line)?|[Qq]uotefarm|[Rr]ecent(?:ism)?|[Rr]efimprove(?:BLP)?|[Rr]estructure|[Rr]eview|[Rr]ewrite|[Rr]oughtranslation|[Ss]ections|[Ss]elf-published|[Ss]pam|[Ss]tory|[Ss]ynthesis|[Tt]echnical|[Ii]nappropriate tone|[Tt]one|[Tt]oo(?:short|long)|[Tt]ravelguide|[Tt]rivia|[Uu]nbalanced|[Uu]nencyclopedic|[Uu]nref(?:erenced(?:BLP)?|BLP)?|[Uu]pdate|[Ww]easel|[Ww]ikify)";

        /// <summary>
        /// Matches the cleanup templates that can be moved into the {{article issues}} template
        /// </summary>
        public static readonly Regex ArticleIssuesTemplateNameRegex = new Regex(ArticleIssuesTemplatesString, RegexOptions.Compiled);

        /// <summary>
        /// Matches COI|OR|POV|BLP
        /// </summary>
        public static readonly Regex CoiOrPovBlp = new Regex("(COI|OR|POV|BLP)", RegexOptions.Compiled);

        public static string DateYearMonthParameter;
        
        /// <summary>
        /// matches the cleanup templates that can be moved into the {{article issues}} template, notably does not match templates with multiple parameters
        /// </summary>
        public static readonly Regex ArticleIssuesTemplates = new Regex(@"{{" + ArticleIssuesTemplatesString + @"\s*(?:\|\s*([^{}\|]*?(?:{{subst:CURRENTMONTHNAME}} {{subst:CURRENTYEAR}}[^{}\|]*?)?))?\s*}}");

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex ReferenceList = new Regex(@"{{\s*(reflist|references-small|references-2column)}}", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

        /// <summary>
        /// Matches infoboxes, group 1 being the template name of the infobox
        /// </summary>
        public static readonly Regex InfoBox = new Regex(@"{{\s*([Ii]nfobox[\s_][^{}\|]+?|[^{}\|]+?[Ii]nfobox)\s*\|(?>[^\{\}]+|\{(?<DEPTH>)|\}(?<-DEPTH>))*(?(DEPTH)(?!))}}");
        
        /// <summary>
        /// Matches the {{circa}} template
        /// </summary>
        public static readonly Regex CircaTemplate = Tools.NestedTemplateRegex(@"Circa");

        // covered by DablinksTests
        /// <summary>
        /// Finds article disamiguation links from http://en.wikipedia.org/wiki/Wikipedia:Template_messages/General#Disambiguation_and_redirection (en only)
        /// </summary>
        public static readonly Regex Dablinks = new Regex(@"{{\s*(?:[Aa]bout|[Ff]or[2-3]?|[Dd]ablink|[Dd]istinguish2?|[Oo]therpeople[1-4]|[Oo]ther ?persons|[Oo]ther ?places[23]?|[Oo]ther ?ships|[Oo]theruses-number|[Oo]ther ?use(?:s[2-5]|s)?|2otheruses|[Rr]edirect-acronym|[Rr]edirect[2-4]?|[Aa]mbiguous link|[Dd]isambig-acronym)\s*(?:\|[^{}]*(?:{{[^{}]*}}[^{}]*)?)?}}", RegexOptions.Compiled);
        
        /// <summary>
        /// Matches the maintenance tags (en-wiki only) such as orphan, cleanup
        /// </summary>
        public static readonly Regex MaintenanceTemplates = Tools.NestedTemplateRegex(new List<string>(@"orphan,BLPunsourced,cleanup".Split(',')));
        
        /// <summary>
        /// Matches the {{Unreferenced}} template
        /// </summary>
        public static readonly Regex Unreferenced = new Regex(@"{{\s*([Uu]nreferenced|[Uu]nsourced|[Uu]nverified|[Uu]nref|[Rr]eferences|[Uu]ncited-article|[Cc]itesources|[Nn][Rr]|[Nn]o references|[Uu]nrefarticle|[Nn]o ?refs?|[Nn]oreferences|[Cc]leanup-cite|[Rr]eferences needed)\s*(?:\|.*?)?}}", RegexOptions.Singleline);
        /// <summary>
        /// Matches {{XX Portal}} templates
        /// </summary>
        public static readonly Regex PortalTemplate = new Regex(@"{{\s*[Pp]ortal(?:par)?(?:\|[^{}]+)?}}", RegexOptions.RightToLeft);
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
        public static readonly Regex Refs = new Regex(@"(<ref\s+(?:name|group)\s*=\s*[^<>]*?/\s*>|<ref\b[^>/]*?>.*?<\s*/\s*ref\s*>)", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// matches &lt;cite&gt; tags
        /// </summary>
        public static readonly Regex Cites = new Regex(@"<cite[^>]*?>[^<]*<\s*/cite\s*>", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// matches &lt;nowiki&gt; tags
        /// </summary>
        public static readonly Regex Nowiki = new Regex("<nowiki>.*?</nowiki>", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// matches templates, including templates with the template namespace prefix
        /// </summary>
        public static Regex TemplateCall;

        /// <summary>
        /// No General Fixes regex for checkpage parsing
        /// </summary>
        public static readonly Regex NoGeneralFixes = new Regex("<!--No general fixes:.*?-->", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// No RegexTypoFix regex for checkpage parsing
        /// </summary>
        public static readonly Regex NoRETF = new Regex("<!--No RETF:.*?-->", RegexOptions.Singleline | RegexOptions.Compiled);

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
        public static readonly Regex ExternalLinksHeaderRegex = new Regex(@"== *External +links? *==", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.RightToLeft);

        /// <summary>
        /// Matches parameters within the {{article issues}} template using title case (invalid casing)
        /// </summary>
        public static readonly Regex ArticleIssuesInTitleCase = new Regex(@"({{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues\|\s*(?:[^{}]+?\|\s*)?)([A-Z])([a-z]+(?: [a-z]+)?\s*=)", RegexOptions.Compiled);

        /// <summary>
        /// Matches the {{article issues}} template using the 'expert' parameter
        /// </summary>
        public static readonly Regex ArticleIssuesRegexExpert = new Regex(@"{{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues[^{}]+?expert", RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex ArticleIssuesRegexWithDate = new Regex(@"({{\s*(?:[Aa]rticle|[Mm]ultiple) ?issues\s*(?:\|[^{}]*?)?)\|\s*date\s*=[^{}\|]{0,20}?(\||}})", RegexOptions.Compiled);

        /// <summary>
        /// Matches a number between 1000 and 2999
        /// </summary>
        public static readonly Regex GregorianYear = new Regex(@"\b[12]\d{3}\b", RegexOptions.Compiled);

        #region Parsers.FixFootnotes **NOT READY FOR PRODUCTION**
        // removed superscripted punctuation between refs
        public static readonly Regex SuperscriptedPunctuationBetweenRefs = new Regex("(</ref>|<ref[^>]*?/>)<sup>[ ]*[,;-]?[ ]*</sup><ref", RegexOptions.Compiled);

        private const string FactTag = "{{[ ]*(fact|fact[ ]*[\\|][^}]*|facts|citequote|citation needed|cn|verification needed|verify source|verify credibility|who|failed verification|nonspecific|dubious|or|lopsided|GR[ ]*[\\|][ ]*[^ ]+|[c]?r[e]?f[ ]*[\\|][^}]*|ref[ _]label[ ]*[\\|][^}]*|ref[ _]num[ ]*[\\|][^}]*)[ ]*}}";
        public static readonly Regex WhiteSpaceFactTag = new Regex("[\\n\\r\\f\\t ]+?" + FactTag, RegexOptions.Compiled);

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
        public static readonly Regex IbidOpCitation = new Regex(@"\b(ibid|op.{1,4}cit)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches the {{Ibid}} template
        /// </summary>
        public static readonly Regex Ibid = Tools.NestedTemplateRegex(@"Ibid");
        
        /// <summary>
        /// Matches the {{Inuse}} template
        /// </summary>
        public static readonly Regex InUse = Tools.NestedTemplateRegex(@"Inuse");

        /// <summary>
        /// Matches consecutive whitespace
        /// </summary>
        public static readonly Regex WhiteSpace = new Regex(@"\s+", RegexOptions.Compiled);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// From http://www.dreamincode.net/code/snippet3490.htm
        /// </remarks>
        public static readonly Regex UrlValidator =
            new Regex(
                @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s]$",
                RegexOptions.Compiled);
    }
}
