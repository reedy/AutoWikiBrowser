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
        public static readonly Regex SimpleWikiLink = new Regex("\\[\\[(.*?)\\]\\]", RegexOptions.Compiled);
        public static readonly Regex WikiLink = new Regex("\\[\\[(.*?)(\\]\\]|\\|)", RegexOptions.Compiled);
        public static readonly Regex Stub = new Regex("\\{\\{.*?[Ss]tub\\}\\}", RegexOptions.Compiled);
        public static readonly Regex Template = new Regex(@"\{\{.*?\}\}", RegexOptions.Compiled);
        public static readonly Regex PipedWikiLink = new Regex("\\[\\[([^[]*?)\\|([^[]*?)\\]\\]", RegexOptions.Compiled);
    }
}
