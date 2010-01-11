/*
(C) 2007 Stephen Kennedy (Kingboyk) http://www.sdk-software.com/

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

/* From WikiFunctions2.dll. Converted from VB to C# in Reflector.
   Some of this is currently only suitable for enwiki. */

using System.Text.RegularExpressions;
using WikiFunctions.Logging;

namespace WikiFunctions.TalkPages
{
    public enum DEFAULTSORT
    {
        NoChange,
        MoveToTop,
        MoveToBottom
    }

    internal class Processor
    {
        public string DefaultSortKey;
        public bool FoundDefaultSort;
        public bool FoundSkipTOC;
        public bool FoundTalkHeader;

        // Match evaluators:
        public string DefaultSortMatchEvaluator(Match match)
        {
            FoundDefaultSort = true;
            if (match.Groups["key"].Captures.Count > 0)
                DefaultSortKey = match.Groups["key"].Captures[0].Value.Trim();
            return "";
        }

        public string SkipTOCMatchEvaluator(Match match)
        {
            FoundSkipTOC = true;
            return "";
        }

        public string TalkHeaderMatchEvaluator(Match match)
        {
            FoundTalkHeader = true;
            return "";
        }
    }

    public static class TalkPageHeaders
    {
        private static readonly Regex SkipTOCTemplateRegex = new Regex(
           @"\{\{\s*(template *:)?\s*(skiptotoctalk|Skiptotoc|Skiptotoc-talk)\s*\}\}\s*",
           RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        private static readonly Regex TalkHeaderTemplateRegex = new Regex(
           @"\{\{\s*(template *:)?\s*(talkheader|Talkheaderlong|Comment Guidelines|Categorytalkheader|Newtalk|Templatetalkheader|Talkheader2|Talkheader3|Talkpagelong|Talk box|Talkpageheader|TalkHeader|User Talkheader)\s*\}\}\s*", 
           RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        // Public methods:
        public static bool ContainsDefaultSortKeywordOrTemplate(string ArticleText)
        {
            return WikiRegexes.Defaultsort.IsMatch(ArticleText);
        }

        public static void ProcessTalkPage(ref string ArticleText, string PluginName)
        {
            ProcessTalkPage(ref ArticleText, null, DEFAULTSORT.NoChange, PluginName);
        }

        public static void ProcessTalkPage(ref string ArticleText, DEFAULTSORT MoveDEFAULTSORT, string PluginName)
        {
            ProcessTalkPage(ref ArticleText, null, MoveDEFAULTSORT, PluginName);
        }

        public static bool ProcessTalkPage(ref string ArticleText, IMyTraceListener Trace, 
        DEFAULTSORT MoveDEFAULTSORT, string PluginName)
        {
            Processor pr = new Processor();
            ArticleText = TalkHeaderTemplateRegex.Replace(ArticleText, 
                new MatchEvaluator(pr.TalkHeaderMatchEvaluator),
                1);
            ArticleText = SkipTOCTemplateRegex.Replace(ArticleText, new MatchEvaluator(pr.SkipTOCMatchEvaluator), 1);
            if (pr.FoundTalkHeader)
                WriteHeaderTemplate("talkheader", ref ArticleText, Trace, PluginName);
            if (pr.FoundSkipTOC)
                WriteHeaderTemplate("skiptotoctalk", ref ArticleText, Trace, PluginName);
            if (MoveDEFAULTSORT != DEFAULTSORT.NoChange)
            {
                ArticleText = WikiRegexes.Defaultsort.Replace(ArticleText, 
                    new MatchEvaluator(pr.DefaultSortMatchEvaluator), 1);
                if (pr.FoundDefaultSort)
                {
                    if (string.IsNullOrEmpty(pr.DefaultSortKey))
                    {
                        if (Trace != null)
                            Trace.WriteArticleActionLine("DEFAULTSORT has no key; removed", PluginName);
                    }
                    else
                    {
                        ArticleText = SetDefaultSort(pr.DefaultSortKey, MoveDEFAULTSORT, Trace, PluginName, ArticleText);
                    }
                }
            }
            return pr.FoundTalkHeader || pr.FoundSkipTOC || pr.FoundDefaultSort;
        }

        public static string FormatDefaultSort(string ArticleText)
        {
            return WikiRegexes.Defaultsort.Replace(ArticleText, "{{DEFAULTSORT:${key}}}");
        }
        
        // Helper routines:
        private static string SetDefaultSort(string key, DEFAULTSORT Location, IMyTraceListener Trace, string PluginName, string ArticleText)
        {
            string strMovedTo;
            if (Location == DEFAULTSORT.MoveToTop)
            {
                ArticleText = "{{DEFAULTSORT:" + key + "}}\r\n" + ArticleText;
                strMovedTo = " given top billing";
            }
            else
            {
                ArticleText = ArticleText + "\r\n{{DEFAULTSORT:" + key + "}}";
                strMovedTo = " sent to the bottom";
            }
            if (Trace != null)
                Trace.WriteArticleActionLine("DEFAULTSORT" + strMovedTo, PluginName, false);

            return ArticleText;
        }

        private static void WriteHeaderTemplate(string Name, ref string ArticleText, IMyTraceListener Trace, string PluginName)
        {
            ArticleText = "{{" + Name + "}}\r\n" + ArticleText;
            if (Trace != null)
                Trace.WriteArticleActionLine("{{tl|" + Name + "}} given top billing", PluginName, false);
        }
    }
}

