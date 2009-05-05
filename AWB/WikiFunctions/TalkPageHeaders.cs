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

        public static bool ContainsDefaultSortKeywordOrTemplate(string articleText)
        {
            return WikiRegexes.Defaultsort.IsMatch(articleText);
        }

        public static void ProcessTalkPage(ref string articleText, string pluginName)
        {
            ProcessTalkPage(ref articleText, null, DEFAULTSORT.NoChange, pluginName);
        }

        public static void ProcessTalkPage(ref string articleText, DEFAULTSORT moveDEFAULTSORT, string pluginName)
        {
            ProcessTalkPage(ref articleText, null, moveDEFAULTSORT, pluginName);
        }

        public static bool ProcessTalkPage(ref string articleText, IMyTraceListener trace, 
        DEFAULTSORT moveDEFAULTSORT, string pluginName)
        {
            Processor pr = new Processor();
            articleText = TalkHeaderTemplateRegex.Replace(articleText, 
                new MatchEvaluator(pr.TalkHeaderMatchEvaluator),
                1);
            articleText = SkipTOCTemplateRegex.Replace(articleText, new MatchEvaluator(pr.SkipTOCMatchEvaluator), 1);
            if (pr.FoundTalkHeader)
                WriteHeaderTemplate("talkheader", ref articleText, trace, pluginName);
            if (pr.FoundSkipTOC)
                WriteHeaderTemplate("skiptotoctalk", ref articleText, trace, pluginName);
            if (moveDEFAULTSORT != DEFAULTSORT.NoChange)
            {
                articleText = WikiRegexes.Defaultsort.Replace(articleText, 
                    new MatchEvaluator(pr.DefaultSortMatchEvaluator), 1);
                if (pr.FoundDefaultSort)
                {
                    if (string.IsNullOrEmpty(pr.DefaultSortKey))
                    {
                        if (trace != null)
                            trace.WriteArticleActionLine("DEFAULTSORT has no key; removed", pluginName);
                    }
                    else
                    {
                        articleText = SetDefaultSort(pr.DefaultSortKey, moveDEFAULTSORT, trace, pluginName, articleText);
                    }
                }
            }
            return pr.FoundTalkHeader || pr.FoundSkipTOC || pr.FoundDefaultSort;
        }

        public static string FormatDefaultSort(string articleText)
        {
            return WikiRegexes.Defaultsort.Replace(articleText, "{{DEFAULTSORT:${key}}}");
        }
        
        // Helper routines:
        private static string SetDefaultSort(string key, DEFAULTSORT location, IMyTraceListener trace, string pluginName, string articleText)
        {
            string strMovedTo;
            if (location == DEFAULTSORT.MoveToTop)
            {
                articleText = "{{DEFAULTSORT:" + key + "}}\r\n" + articleText;
                strMovedTo = " given top billing";
            }
            else
            {
                articleText = articleText + "\r\n{{DEFAULTSORT:" + key + "}}";
                strMovedTo = " sent to the bottom";
            }
            if (trace != null)
                trace.WriteArticleActionLine("DEFAULTSORT" + strMovedTo, pluginName, false);

            return articleText;
        }

        private static void WriteHeaderTemplate(string name, ref string articleText, IMyTraceListener trace, string pluginName)
        {
            articleText = "{{" + name + "}}\r\n" + articleText;
            if (trace != null)
                trace.WriteArticleActionLine("{{tl|" + name + "}} given top billing", pluginName, false);
        }
    }
}

