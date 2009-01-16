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

    public static class TalkPageHeaders
    {
        private static string DefaultSortKey;
        private static bool FoundDefaultSort;
        private static bool FoundSkipTOC;
        private static bool FoundTalkheader;
        private static readonly Regex SkipTOCTemplateRegex = new Regex(
           @"\{\{\s*(template *:)?\s*(skiptotoctalk|Skiptotoc|Skiptotoc-talk)\s*\}\}\s*",
           RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);
        private static readonly Regex TalkheaderTemplateRegex = new Regex(
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
            FoundTalkheader = false; FoundSkipTOC = false; FoundDefaultSort = false;
            ArticleText = TalkheaderTemplateRegex.Replace(ArticleText, new MatchEvaluator(TalkheaderMatchEvaluator), 1);
            ArticleText = SkipTOCTemplateRegex.Replace(ArticleText, new MatchEvaluator(SkipTOCMatchEvaluator), 1);
            if (FoundTalkheader)
                WriteHeaderTemplate("talkheader", ref ArticleText, Trace, PluginName);
            if (FoundSkipTOC)
                WriteHeaderTemplate("skiptotoctalk", ref ArticleText, Trace, PluginName);
            if (MoveDEFAULTSORT != DEFAULTSORT.NoChange)
            {
                ArticleText = WikiRegexes.Defaultsort.Replace(ArticleText, 
                    new MatchEvaluator(DefaultSortMatchEvaluator), 1);
                if (FoundDefaultSort)
                {
                    if (string.IsNullOrEmpty(DefaultSortKey))
                    {
                        if (Trace != null)
                            Trace.WriteArticleActionLine("DEFAULTSORT has no key; removed", PluginName);
                    }
                    else
                    {
                        WriteDefaultSortMagicWord(MoveDEFAULTSORT, Trace, PluginName, ref ArticleText);
                    }
                }
                DefaultSortKey = "";
            }
            if ((!FoundTalkheader && !FoundSkipTOC) && !FoundDefaultSort)
            {
                return false;
            }
            return true;
        }

        public static string FormatDefaultSort(string ArticleText)
        {
            return WikiRegexes.Defaultsort.Replace(ArticleText, "{{DEFAULTSORT:${key}}}");
        }
        
        // Match evaluators:
        private static string DefaultSortMatchEvaluator(Match match)
        {
            FoundDefaultSort = true;
            if (match.Groups["key"].Captures.Count > 0)
                DefaultSortKey = match.Groups["key"].Captures[0].Value.Trim();
            return "";
        }

        private static string SkipTOCMatchEvaluator(Match match)
        {
            FoundSkipTOC = true;
            return "";
        }

        private static string TalkheaderMatchEvaluator(Match match)
        {
            FoundTalkheader = true;
            return "";
        }

        // Helper routines:
        private static void WriteDefaultSortMagicWord(DEFAULTSORT Location, IMyTraceListener Trace, string PluginName, ref string ArticleText)
        {
            string strMovedTo;
            if (Location == DEFAULTSORT.MoveToTop)
            {
                ArticleText = "{{DEFAULTSORT:" + DefaultSortKey + "}}\r\n" + ArticleText;
                strMovedTo = " given top billing";
            }
            else
            {
                ArticleText = ArticleText + "\r\n{{DEFAULTSORT:" + DefaultSortKey + "}}";
                strMovedTo = " sent to the bottom";
            }
            if (Trace != null)
                Trace.WriteArticleActionLine("DEFAULTSORT" + strMovedTo, PluginName, false);
        }

        private static void WriteHeaderTemplate(string Name, ref string ArticleText, IMyTraceListener Trace, string PluginName)
        {
            ArticleText = "{{" + Name + "}}\r\n" + ArticleText;
            if (Trace != null)
                Trace.WriteArticleActionLine("{{tl|" + Name + "}} given top billing", PluginName, false);
        }
    }
}

