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
        /// Matches single line templates
        /// </summary>
        public static readonly Regex Template = new Regex(@"\{\{.*?\}\}", RegexOptions.Compiled);
        
        /// <summary>
        /// Matches single and multiline templates
        /// </summary>
        public static readonly Regex TemplateMultiLine = new Regex(@"\{\{[^\\}]*?\}\}", RegexOptions.Compiled);

        /// <summary>
        /// Matches external links
        /// </summary>
        public static readonly Regex ExternalLinks = new Regex(@"[Hh]ttp://[^\ \n]*|\[[Hh]ttp:.*?\]", RegexOptions.Compiled);

        /// <summary>
        /// Matches interwiki links
        /// </summary>
        public static readonly Regex InterWikiLinks = new Regex(@"\[\[([a-z]{2,3}|simple|fiu-vro|minnan|roa-rup|tokipona|zh-min-nan):.*\]\]", RegexOptions.Compiled);

        /// <summary>
        /// Matches unformatted text regions: nowiki, pre, math, html comments, timelines
        /// </summary>
        public static readonly Regex UnFormattedText = new Regex(@"<nowiki>.*?</nowiki>|<pre>.*?</pre>|<math>.*?</math>|<!--.*?-->|<timeline>.*?</timeline>", RegexOptions.Singleline | RegexOptions.Compiled);

        /// <summary>
        /// Matches redirects
        /// </summary>
        public static readonly Regex RedirectRegex = new Regex("^#redirect.*?\\[\\[(.*?)\\]\\]", RegexOptions.IgnoreCase);

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
        /// Matches categories (en only)
        /// </summary>
        public static readonly Regex Category = new Regex(@"\[\[[Cc]ategory:(.*?)\]\]", RegexOptions.Compiled);

        /// <summary>
        /// Matches persondata
        /// </summary>
        public static readonly Regex Persondata = new Regex(@"\{\{ ?[Pp]ersondata.*?\}\}", RegexOptions.Singleline);
        
        #endregion
        //???
    
    }
}
