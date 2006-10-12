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
            Category = new Regex(@"\[\[" + Variables.NamespacesCaseInsensitive[14] + @"(.*?)\]\]", RegexOptions.Compiled);
            Images = new Regex(@"\[\[" + Variables.NamespacesCaseInsensitive[6] + @"(.*?)\]\]", RegexOptions.Compiled);
        }

        /// <summary>
        /// Matches all wikilinks, categories, images etc.
        /// </summary>
        public static readonly Regex SimpleWikiLink = new Regex(@"\[\[(.*?)\]\]", RegexOptions.Compiled);

        /// <summary>
        /// Matches only internal wiki links
        /// </summary>
        public static readonly Regex WikiLinksOnly = new Regex("\\[\\[[^:]*?\\]\\]", RegexOptions.Compiled);

        /// <summary>
        /// Group 1 Matches only the target of the wikilink
        /// </summary>
        public static readonly Regex WikiLink = new Regex(@"\[\[(.*?)(?:\]\]|\|)", RegexOptions.Compiled);

        /// <summary>
        /// Matches piped wikilinks, group 1 is target, group 2 the text
        /// </summary>
        public static readonly Regex PipedWikiLink = new Regex(@"\[\[([^[]*?)\|([^[]*?)\]\]", RegexOptions.Compiled);
        
        /// <summary>
        /// Matches == headings ==
        /// </summary>
        public static readonly Regex Heading = new Regex(@"^(=+)(.*?)(=+)", RegexOptions.Compiled);

        /// <summary>
        /// Matches text indented with a :
        /// </summary>
        public static readonly Regex IndentedText = new Regex(@"^:.*", RegexOptions.Compiled | RegexOptions.Multiline);
        
        /// <summary>
        /// Matches single line templates
        /// </summary>
        public static readonly Regex Template = new Regex(@"\{\{.*?\}\}", RegexOptions.Compiled);
        
        /// <summary>
        /// Matches single and multiline templates
        /// </summary>
        public static readonly Regex TemplateMultiLine = new Regex(@"\{\{.*?\}\}", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        /// Matches external links
        /// </summary>
        public static readonly Regex ExternalLinks = new Regex(@"[Hh]ttp://[^\ \n]*|\[[Hh]ttp:.*?\]", RegexOptions.Compiled);

        /// <summary>
        /// Matches interwiki links
        /// </summary>
        public static readonly Regex InterWikiLinks = new Regex(@"\[\[(nds-nl|rmy|lij|bat-smg|map-bms|ksh|pdc|vls|nrm|frp|zh-yue|tet|xal|pap|tokipona|minnan|aa|af|ak|als|am|ang|ab|ar|an|arc|roa-rup|as|ast|gn|av|ay|az|bm|bn|zh-min-nan|ba|be|bh|bi|bo|bs|br|bg|ca|cv|ceb|cs|ch|ny|sn|tum|cho|co|za|cy|da|de|dv|nv|dz|mh|et|el|en|es|eo|eu|ee|fa|fo|fr|fy|ff|fur|ga|gv|gd|gl|ki|gu|got|ko|ha|haw|hy|hi|ho|hr|io|ig|ilo|id|ia|ie|iu|ik|os|xh|zu|is|it|he|jv|kl|kn|kr|ka|ks|csb|kk|kw|rw|ky|rn|sw|kv|kg|ht|kj|ku|lo|lad|la|lv|lb|lt|li|ln|jbo|lg|lmo|hu|mk|mg|ml|mt|mi|mr|ms|mo|mn|mus|my|nah|na|nb|fj|nl|cr|ne|ja|nap|ce|pih|nb|no|nn|oc|or|om|ng|hz|ug|pa|pi|pam|ps|km|nds|pl|pms|pt|ty|ro|rm|qu|ru|war|se|sm|sa|sg|sc|sco|st|tn|sq|scn|si|simple|sd|ss|sk|sl|so|sr|sh|su|fi|sv|tl|ta|tt|te|th|vi|ti|tg|tpi|to|chr|chy|ve|tr|tk|tw|udm|bug|uk|ur|uz|vec|vo|fiu-vro|wa|wo|ts|ii|yi|yo|zh|zh-tw|zh-cn|wuu|mzn|new|lbe|eml|bxr|hsb|nov|pag|bar|bpy|diq|zea|roa-tara|cbk-zam|zh-classical|cu|ru-sib|glk|cdo):.*?\]\]", RegexOptions.Compiled);
        
        /// <summary>
        /// Matches unformatted text regions: nowiki, pre, math, html comments, timelines
        /// </summary>
        public static readonly Regex UnFormattedText = new Regex(@"<nowiki>.*?</nowiki>|<pre>.*?</pre>|<math>.*?</math>|<!--.*?-->|<timeline>.*?</timeline>", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Matches redirects
        /// </summary>
        public static readonly Regex RedirectRegex = new Regex("^#redirect.*?\\[\\[(.*?)\\]\\]", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// Matches words
        /// </summary>
        public static readonly Regex RegexWordCount = new Regex("[a-zA-Z]+", RegexOptions.Compiled);

        /// <summary>
        /// Matches IP addresses
        /// </summary>
        public static readonly Regex IPAddress = new Regex("[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}", RegexOptions.Compiled);

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
        public static Regex Category = new Regex(@"\[\[[Cc]ategory:(.*?)\]\]", RegexOptions.Compiled);

        /// <summary>
        /// Matches images
        /// </summary>
        public static Regex Images = new Regex(@"\[\[[Ii]mage:.*\]\]", RegexOptions.Compiled);
        
        #region en only

        /// <summary>
        /// Matches disambig templates (en only)
        /// </summary>
        public static readonly Regex Disambigs = new Regex(@"\{\{([234]CC|[Dd]isambig|[Gg]eodis|[Hh]ndis|[Ss]urname|[Nn]umberdis|[Rr]oaddis)\}\}", RegexOptions.Compiled);

        /// <summary>
        /// Matches stubs (en only)
        /// </summary>
        public static readonly Regex Stub = new Regex(@"\{\{.*?[Ss]tub\}\}", RegexOptions.Compiled);

        /// <summary>
        /// Matches persondata (en only)
        /// </summary>
        public static readonly Regex Persondata = new Regex(@"\{\{ ?[Pp]ersondata.*?\}\}", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Matches {{Link FA|xxx}} (en only)
        /// </summary>
        public static readonly Regex LinkFAs = new Regex(@"\{\{[Ll]ink FA\|.*?\}\}", RegexOptions.Compiled);
        
        #endregion
    
    }
}
