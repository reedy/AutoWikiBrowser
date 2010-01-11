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

/* Some of this is currently only suitable for enwiki. */

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
        public static bool ContainsDefaultSortKeywordOrTemplate(string articleText)
        {
            return WikiRegexes.Defaultsort.IsMatch(articleText);
        }

        public static bool ProcessTalkPage(ref string articleText, IMyTraceListener trace, 
        DEFAULTSORT moveDefaultsort, string pluginName)
        {
            Processor pr = new Processor();
            articleText = WikiRegexes.TalkHeaderTemplate.Replace(articleText, 
                new MatchEvaluator(pr.TalkHeaderMatchEvaluator),
                1);
            articleText = WikiRegexes.SkipTOCTemplateRegex.Replace(articleText, new MatchEvaluator(pr.SkipTOCMatchEvaluator), 1);
            if (pr.FoundTalkHeader)
                WriteHeaderTemplate("talkheader", ref articleText, trace, pluginName);
            if (pr.FoundSkipTOC)
                WriteHeaderTemplate("skiptotoctalk", ref articleText, trace, pluginName);
            if (moveDefaultsort != DEFAULTSORT.NoChange)
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
                        articleText = SetDefaultSort(pr.DefaultSortKey, moveDefaultsort, trace, pluginName, articleText);
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
            string movedTo;
            switch (location)
            {
                case DEFAULTSORT.MoveToTop:
                    articleText = "{{DEFAULTSORT:" + key + "}}\r\n" + articleText;
                    movedTo = " given top billing";
                    break;
                case DEFAULTSORT.MoveToBottom:
                    articleText = articleText + "\r\n{{DEFAULTSORT:" + key + "}}";
                    movedTo = " sent to the bottom";
                    break;
                default:
                    movedTo = "";
                    break;
            }

            if (location != DEFAULTSORT.NoChange && trace != null)
                trace.WriteArticleActionLine("DEFAULTSORT" + movedTo, pluginName, false);

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

