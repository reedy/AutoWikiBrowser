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
            TemplateStart = @"\{\{\s*(:?" + Variables.NamespacesCaseInsensitive[10] + ")?";

            Category = new Regex(@"\[\[" + Variables.NamespacesCaseInsensitive[14] + @"(.*?)\]\]", RegexOptions.Compiled);
            Images = new Regex(@"\[\[" + Variables.NamespacesCaseInsensitive[6] + @"([^\]]*?(?:\[\[?.*?(?:\[\[.*?\]\].*?)?\]\]?[^\]]*?)*)\]\]|<[Gg]allery\b([^>]*?)>[\s\S]*?</ ?[Gg]allery>", RegexOptions.Compiled);
            Stub = new Regex(@"{{.*?" + Variables.Stub + @"}}", RegexOptions.Compiled);
            PossiblyCommentedStub = new Regex(@"(<!-- ?\{\{[^}]*?" + Variables.Stub + @"\b\}\}.*?-->|\{\{[^}]*?" + Variables.Stub + @"\}\})", RegexOptions.Compiled);
            TemplateCall = new Regex(TemplateStart + @"\s*([^\]\|]*)\s*(.*)}}", RegexOptions.Compiled | RegexOptions.Singleline);

            LooseCategory = new Regex(@"\[\[[\s_]*" + Variables.NamespacesCaseInsensitive[14] + @"[\s_]*([^\|]*?)(|\|.*?)\]\]", RegexOptions.Compiled);
            LooseImage = new Regex(@"\[\[\s*?(" + Variables.NamespacesCaseInsensitive[6] + @")\s*([^\|\]]*?)(.*?)\]\]", RegexOptions.Compiled);

            string s;
            switch (Variables.LangCode)
            {
                case LangCodeEnum.ar:
                    s = "(?:تحويل|REDIRECT)";
                    break;
                case LangCodeEnum.bg:
                    s = "(?:redirect|пренасочване|виж)";
                    break;
                case LangCodeEnum.fi:
                    s = "(?:OHJAUS|UUDELLEENOHJAUS|REDIRECT)";
                    break;
                case LangCodeEnum.he:
                    s = "(?:הפניה|REDIRECT)";
                    break;
                case LangCodeEnum.Is:
                    s = "(?:tilvísun|TILVÍSUN|REDIRECT)";
                    break;
                case LangCodeEnum.nl:
                    s = "(?:REDIRECT|DOORVERWIJZING)";
                    break;
                case LangCodeEnum.ru:
                    s = "(?:REDIRECT|ПЕРЕНАПРАВЛЕНИЕ|ПЕРЕНАПР)";
                    break;
                case LangCodeEnum.sk:
                    s = "(?:redirect|presmeruj)";
                    break;
                case LangCodeEnum.uk:
                    s = "(?:REDIRECT|ПЕРЕНАПРАВЛЕННЯ|ПЕРЕНАПР)";
                    break;
                default:
                    s = "REDIRECT";
                    break;
            }
            Redirect = new Regex(@"#" + s + @"\s*:?\s*\[\[\s*:?\s*([^\|]*?)\s*(|\|.*?)]\]", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            if (Variables.LangCode == LangCodeEnum.ru)
                Disambigs = new Regex(TemplateStart + @"([Dd]isambiguation|[Dd]isambig|[Нн]еоднозначность)}}", RegexOptions.Compiled);
            else
                Disambigs = new Regex(@"{{([234]CC|[Dd]isambig|[Gg]eodis|[Hh]ndis|[Ss]urname|[Nn]umberdis|[Rr]oaddis|[Ll]etter-disambig)}}", RegexOptions.Compiled);

            s = (Variables.LangCode == LangCodeEnum.en) ? "(?:(?i:defaultsort|lifetime|BIRTH-DEATH-SORT)|BD)" : "(?i:defaultsort)";

            Defaultsort = new Regex(TemplateStart + s + @"\s*[:|](?<key>[^\}]*)}}", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

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
        }

        /// <summary>
        /// Piece of template call, including curly brace and possible namespace
        /// </summary>
        public static string TemplateStart;

        /// <summary>
        /// Matches all wikilinks, categories, images etc.
        /// </summary>
        public static readonly Regex SimpleWikiLink = new Regex(@"\[\[(.*?)\]\]", RegexOptions.Compiled);

        /// <summary>
        /// Matches only internal wiki links
        /// </summary>
        public static readonly Regex WikiLinksOnly = new Regex(@"\[\[[^[\]]*?\]\]", RegexOptions.Compiled);

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
        /// Matches single line templates
        /// </summary>
        public static readonly Regex Template = new Regex(@"{{[^{\n]*?}}", RegexOptions.Compiled);

        /// <summary>
        /// Matches single and multiline templates
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
        public static readonly Regex Dates = new Regex("^[0-9]{1,2} (January|February|March|April|May|June|July|August|September|October|November|December)$", RegexOptions.Compiled);

        /// <summary>
        /// Matches Dates like January 21
        /// TODO: internationalise
        /// </summary>
        public static readonly Regex Dates2 = new Regex("^(January|February|March|April|May|June|July|August|September|October|November|December) [0-9]{1,2}$", RegexOptions.Compiled);

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

        #region en only
        /// <summary>
        /// Matches persondata (en only)
        /// </summary>
        public static readonly Regex Persondata = new Regex(@"{{ ?[Pp]ersondata.*?}}", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Matches {{Link FA|xxx}} (en only)
        /// </summary>
        public static readonly Regex LinkFAs = new Regex(@"{{[Ll]ink FA\|.*?}}", RegexOptions.Compiled);

        /// <summary>
        /// Matches {{Deadend|xxx}} (en only)
        /// </summary>
        public static readonly Regex DeadEnd = new Regex(@"{{[Dd]eadend\|.*?}}", RegexOptions.Compiled);
        #endregion

        /// <summary>
        /// matches <!-- comments -->
        /// </summary>
        public static readonly Regex Comments = new Regex(@"<!--.*?-->", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// 
        /// </summary>
        public static readonly Regex EmptyComments = new Regex(@"<!--[^\S\r\n]*-->", RegexOptions.Compiled);

        /// <summary>
        /// matches <ref> tags
        /// </summary>
        public static readonly Regex Refs = new Regex(@"(<ref\b[^>]*?>[^<]*<\s*/\s*ref\s*>|<ref\s+name\s*=\s*.*?/\s*>)", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// matches <cite> tags
        /// </summary>
        public static readonly Regex Cites = new Regex(@"<cite[^>]*?>[^<]*<\s*/cite\s*>", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// matches <nowiki> tags
        /// </summary>
        public static readonly Regex Nowiki = new Regex("<nowiki>.*?</nowiki>", RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// matches user groups
        /// </summary>
        public static readonly Regex wgUserGroups = new Regex(@"^var\s*wgUserGroups\s*=\s*\[(.*\])", RegexOptions.Compiled);

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
    }
}
