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
            Category = new Regex(@"\[\[" + Variables.NamespacesCaseInsensitive[14] + @"(.*?)\]\]|<[Gg]allery\b([^>]*?)>[\s\S]*?</ ?[Gg]allery>", RegexOptions.Compiled);
            Images = new Regex(@"\[\[" + Variables.NamespacesCaseInsensitive[6] + @"(.*?)\]\]|<[Gg]allery\b([^>]*?)>[\s\S]*?</ ?[Gg]allery>", RegexOptions.Compiled);
            Stub = new Regex(@"{{.*?" + Variables.Stub + @"}}", RegexOptions.Compiled);
            PossiblyCommentedStub = new Regex(@"(<!-- ?\{\{[^}]*?" + Variables.Stub + @"\b\}\}.*?-->|\{\{[^}]*?" + Variables.Stub + @"\}\})", RegexOptions.Compiled);
            string s = Variables.NamespacesCaseInsensitive[10];
            if (s[0] == '(') s = s.Insert(s.Length - 1, "|");
            else s = "(?:" + s + "|)";
            TemplateCall = new Regex(@"{{\s*" + s + @"\s*([^\]\|]*)\s*(.*)}}", RegexOptions.Compiled | RegexOptions.Singleline);

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
            Redirect = new Regex(@"^#" + s + @".*?\[\[(.*?)\]\]", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            if (Variables.LangCode == LangCodeEnum.ru)
            {
                Disambigs = new Regex(@"{{([Dd]isambiguation|[Dd]isambig|[Нн]еоднозначность])}}", RegexOptions.Compiled);
            }
            else
                Disambigs = new Regex(@"{{([234]CC|[Dd]isambig|[Gg]eodis|[Hh]ndis|[Ss]urname|[Nn]umberdis|[Rr]oaddis|[Ll]etter-disambig)}}", RegexOptions.Compiled);
        }

        /// <summary>
        /// Matches all wikilinks, categories, images etc.
        /// </summary>
        public static readonly Regex SimpleWikiLink = new Regex(@"\[\[(.*?)\]\]", RegexOptions.Compiled);

        /// <summary>
        /// Matches only internal wiki links
        /// </summary>
        public static readonly Regex WikiLinksOnly = new Regex(@"\[\[[^\]]*?\]\]", RegexOptions.Compiled);

        /// <summary>
        /// Group 1 Matches only the target of the wikilink
        /// </summary>
        public static readonly Regex WikiLink = new Regex(@"\[\[(.*?)(?:\]\]|\|)", RegexOptions.Compiled);

        /// <summary>
        /// Matches piped wikilinks, group 1 is target, group 2 the text
        /// </summary>
        public static readonly Regex PipedWikiLink = new Regex(@"\[\[([^[]*?)\|([^[]*?)\]\]", RegexOptions.Compiled);

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
        public static readonly Regex Defaultsort = new Regex(
            @"{{\s*(template\s*:\s*)*\s*defaultsort\s*(:|\|)(?<key>[^\}]*)}}",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        /// <summary>
        /// Matches all headings
        /// </summary>
        public static readonly Regex Heading = new Regex(@"^(=+)(.*?)(=+)", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// Matches = headings =
        /// </summary>
        public static readonly Regex Heading1 = new Regex(@"^(=)(.*?)(=)", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// Matches == headings ==
        /// </summary>
        public static readonly Regex Heading2 = new Regex(@"^(==)(.*?)(==)", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// Matches === headings ===
        /// </summary>
        public static readonly Regex Heading3 = new Regex(@"^(===)(.*?)(===)", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// Matches ==== headings ====
        /// </summary>
        public static readonly Regex Heading4 = new Regex(@"^(====)(.*?)(====)", RegexOptions.Compiled | RegexOptions.Multiline);

        /// <summary>
        /// Matches ===== headings =====
        /// </summary>
        public static readonly Regex Heading5 = new Regex(@"^(=====)(.*?)(=====)", RegexOptions.Compiled | RegexOptions.Multiline);

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
        /// Matches interwiki links
        /// </summary>
        public static readonly Regex InterWikiLinks = new Regex(@"\[\[(nds-nl|rmy|lij|bat-smg|map-bms|ksh|pdc|vls|nrm|frp|zh-yue|tet|xal|pap|tokipona|minnan|aa|af|ak|als|am|ang|ab|ar|an|arc|roa-rup|as|ast|gn|av|ay|az|bm|bn|zh-min-nan|ba|be|be-x-old|bh|bi|bo|bs|br|bg|ca|cv|ceb|cs|ch|cu|ny|sn|tum|cho|co|za|cy|da|de|dv|nv|dz|mh|et|el|en|es|eo|eu|ee|fa|fo|fr|fy|ff|fur|ga|gv|gd|gl|ki|gu|got|ko|ha|haw|hy|hi|ho|hr|io|ig|ilo|id|ia|ie|iu|ik|os|xh|zu|is|it|he|jv|kl|kn|kr|ka|ks|csb|kk|kw|rw|ky|rn|sw|kv|kg|ht|kj|ku|lo|lad|la|lv|lb|lt|li|ln|jbo|lg|lmo|hu|mk|mg|ml|mt|mi|mr|ms|mo|mn|mus|my|nah|na|nb|fj|nl|cr|ne|ja|nap|ce|pih|nb|no|nn|oc|or|om|ng|hz|ug|pa|pi|pam|ps|km|nds|pl|pms|pt|ty|ro|rm|qu|ru|war|se|sm|sa|sg|sc|sco|st|tn|sq|scn|si|simple|sd|ss|sk|sl|so|sr|sh|su|fi|sv|tl|ta|tt|te|th|vi|ti|tg|tpi|to|chr|chy|ve|tr|tk|tw|udm|bug|uk|ur|uz|vec|vo|fiu-vro|wa|wo|ts|ii|yi|yo|zh|zh-tw|zh-cn|wuu|mzn|new|lbe|eml|bxr|hsb|nov|pag|bar|bpy|diq|zea|roa-tara|cbk-zam|zh-classical|cu|ru-sib|glk|cdo):.*?\]\]", RegexOptions.Compiled);
        
        /// <summary>
        /// Matches unformatted text regions: nowiki, pre, math, html comments, timelines
        /// </summary>
        public static readonly Regex UnFormattedText = new Regex(@"<nowiki>.*?</nowiki>|<pre>.*?</pre>|<math>.*?</math>|<!--.*?-->|<timeline>.*?</timeline>", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Matches <blockquote> tags
        /// </summary>
        public static readonly Regex Blockquote = new Regex(@"< ?blockquote ?>(.*?)< ?/ ?blockquote ?>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Matches redirects
        /// </summary>
        public static Regex Redirect;
        // remove this line after February 2008! // = new Regex("^#redirect.*?\\[\\[(.*?)\\]\\]", RegexOptions.IgnoreCase | RegexOptions.Singleline); //temp note: Sam and I are assuming that MakeLangSpecificRegexes() will always be called (after Sam's change in next or previous revision); if that's not so, declare a constant with the redirect regex text in it so we don't again have the problem of the regexes being different here and in that procedure

        /// <summary>
        /// Matches words
        /// </summary>
        public static readonly Regex RegexWordCount = new Regex(@"\w+", RegexOptions.Compiled);

        /// <summary>
        /// Matches IP addresses
        /// </summary>
        public static readonly Regex IPAddress = new Regex(@"[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}", RegexOptions.Compiled);

        /// <summary>
        /// Matches <source></source> tags
        /// </summary>
        public static readonly Regex Source = new Regex(@"<\s*source(?:\s.*?|)>(.*?)<\s*/\s*source\s*>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches Dates like 21 January
        /// </summary>
        public static readonly Regex Dates = new Regex("^[0-9]{1,2} (January|February|March|April|May|June|July|August|September|October|November|December)$", RegexOptions.Compiled);

        /// <summary>
        /// Matches Dates like January 21
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

        /// <summary>
        /// matches <!-- comments -->
        /// </summary>
        public static readonly Regex Comments = new Regex(@"<!--.*?-->", RegexOptions.Compiled | RegexOptions.Singleline);

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
        
        #endregion

        /// <summary>
        /// matches template
        /// </summary>
        public static Regex TemplateCall;

        /// <summary>
        /// for checkpage parsing
        /// </summary>
        public static Regex NoGeneralFixes = new Regex("<!--No general fixes:.*?-->", RegexOptions.Singleline | RegexOptions.Compiled);
 
    }
}
