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

        public static bool ProcessTalkPage(ref string articleText, ref string summary, DEFAULTSORT moveDefaultsort)
        {
            Processor pr = new Processor();
            articleText = WikiRegexes.TalkHeaderTemplate.Replace(articleText, 
                new MatchEvaluator(pr.TalkHeaderMatchEvaluator), 1);

            articleText = WikiRegexes.SkipTOCTemplateRegex.Replace(articleText, new MatchEvaluator(pr.SkipTOCMatchEvaluator), 1);
            
            if (pr.FoundTalkHeader)
                WriteHeaderTemplate("talk header", ref articleText, ref summary);

            if (pr.FoundSkipTOC)
                WriteHeaderTemplate("skip to talk", ref articleText, ref summary);

            if (moveDefaultsort != DEFAULTSORT.NoChange)
            {
                articleText = WikiRegexes.Defaultsort.Replace(articleText, 
                    new MatchEvaluator(pr.DefaultSortMatchEvaluator), 1);
                if (pr.FoundDefaultSort)
                {
                    if (string.IsNullOrEmpty(pr.DefaultSortKey))
                    {
                        AppendToSummary(ref summary, "DEFAULTSORT has no key; removed");
                    }
                    else
                    {
                        articleText = SetDefaultSort(pr.DefaultSortKey, moveDefaultsort, articleText, ref summary);
                    }
                }
            }
            return pr.FoundTalkHeader || pr.FoundSkipTOC || pr.FoundDefaultSort;
        }

        public static string FormatDefaultSort(string articleText)
        {
            return WikiRegexes.Defaultsort.Replace(articleText, "{{DEFAULTSORT:${key}}}");
        }

        private static void AppendToSummary(ref string summary, string newText)
        {
            if (!string.IsNullOrEmpty(summary))
                summary += ", ";

            summary += newText;
            }
        
        // Helper routines:
        private static string SetDefaultSort(string key, DEFAULTSORT location, string articleText, ref string summary)
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

            if (location != DEFAULTSORT.NoChange)
                AppendToSummary(ref summary, "DEFAULTSORT" + movedTo);

            return articleText;
        }

        private static void WriteHeaderTemplate(string name, ref string articleText, ref string summary)
        {
            articleText = "{{" + name + "}}\r\n" + articleText;

            AppendToSummary(ref summary, "{{tl|" + name + "}} given top billing");
        }
    }
}

